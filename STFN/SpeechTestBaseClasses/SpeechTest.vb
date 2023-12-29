Public MustInherit Class SpeechTest

#Region "Initialization"

    Public Sub New(ByVal SpeechMaterialName As String)
        Me.SpeechMaterialName = SpeechMaterialName
        LoadSpeechMaterialSpecification(SpeechMaterialName)
    End Sub

#End Region


#Region "SpeechMaterial"

    Private Function LoadSpeechMaterialSpecification(ByVal SpeechMaterialName As String, Optional ByVal EnforceReloading As Boolean = False) As Boolean

        If LoadedSpeechMaterialSpecifications.ContainsKey(SpeechMaterialName) = False Or EnforceReloading = True Then

            'Removes the SpeechMaterial with SpeechMaterialName if already present
            LoadedSpeechMaterialSpecifications.Remove(SpeechMaterialName)

            'Looking for the speech material
            OstfBase.LoadAvailableSpeechMaterialSpecifications()
            For Each Test In OstfBase.AvailableSpeechMaterials
                If Test.Name = SpeechMaterialName Then
                    'Adding it if found
                    LoadedSpeechMaterialSpecifications.Add(SpeechMaterialName, Test)
                    Exit For
                End If
            Next
        End If

        'Returns true if added (or already present) or false if not found
        Return LoadedSpeechMaterialSpecifications.ContainsKey(SpeechMaterialName)

    End Function


    ''' <summary>
    ''' An object shared between all instances of Speechtest that hold every loaded SpeechtestSpecification and 
    ''' Speech Material component to prevent the need for re-loading between tests. 
    ''' (Note that this also means that test specifications and speech material components should not be altered once loaded.
    ''' </summary>
    ''' <returns></returns>
    Private Shared Property LoadedSpeechMaterialSpecifications As New SortedList(Of String, SpeechMaterialSpecification)

    'A shared function to load tests
    Public ReadOnly Property AvailableSpeechMaterialSpecifications() As List(Of String)
        Get
            Dim OutputList As New List(Of String)
            OstfBase.LoadAvailableSpeechMaterialSpecifications()
            For Each test In OstfBase.AvailableSpeechMaterials
                OutputList.Add(test.Name)
            Next
            Return OutputList
        End Get
    End Property

    ''' <summary>
    ''' The SpeechMaterialName of the currently implemented speech material specification
    ''' </summary>
    ''' <returns></returns>
    Public Property SpeechMaterialName As String


    Public Property SpeechMaterialSpecification As SpeechMaterialSpecification
        Get
            If LoadedSpeechMaterialSpecifications.ContainsKey(SpeechMaterialName) Then
                Return LoadedSpeechMaterialSpecifications(SpeechMaterialName)
            Else
                Return Nothing
            End If
        End Get
        Set(value As SpeechMaterialSpecification)
            LoadedSpeechMaterialSpecifications(SpeechMaterialName) = value
        End Set
    End Property

    Public ReadOnly Property SpeechMaterial As SpeechMaterialComponent
        Get
            If SpeechMaterialSpecification Is Nothing Then
                Return Nothing
            Else
                If SpeechMaterialSpecification.SpeechMaterial Is Nothing Then
                    SpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(SpeechMaterialSpecification.GetSpeechMaterialFilePath(), SpeechMaterialSpecification.GetTestRootPath())
                    SpeechMaterialSpecification.SpeechMaterial = SpeechMaterial
                    SpeechMaterial.ParentTestSpecification = SpeechMaterialSpecification
                End If

                If SpeechMaterialSpecification.SpeechMaterial Is Nothing Then
                    Return Nothing
                Else
                    Return SpeechMaterialSpecification.SpeechMaterial
                End If
            End If
        End Get
    End Property

#End Region

#Region "MediaSets"

    Public ReadOnly Property AvailableMediasets() As List(Of MediaSet)
        Get
            SpeechMaterial.ParentTestSpecification.LoadAvailableMediaSetSpecifications()
            Return SpeechMaterial.ParentTestSpecification.MediaSets
        End Get
    End Property


    Public ReadOnly Property AvailablePresets() As List(Of SmcPresets.Preset)
        Get
            Dim Output = New List(Of SmcPresets.Preset)
            For Each Preset In SpeechMaterial.Presets
                Output.Add(Preset)
            Next
            Return Output
        End Get
    End Property

    Public ReadOnly Property AvailableTestListsNames() As List(Of String)
        Get
            Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)
            Dim Output As New List(Of String)
            For Each List In AllLists
                Output.Add(List.PrimaryStringRepresentation)
            Next
            Return Output
        End Get
    End Property

#End Region


#Region "SoundScene"

    Public MustOverride ReadOnly Property MaximumSoundFieldSpeechLocations As Integer
    Public MustOverride ReadOnly Property MaximumSoundFieldMaskerLocations As Integer
    Public MustOverride ReadOnly Property MaximumSoundFieldBackgroundNonSpeechLocations As Integer
    Public MustOverride ReadOnly Property MaximumSoundFieldBackgroundSpeechLocations As Integer

    Public MustOverride ReadOnly Property AllowsReferenceLevelControl As Boolean
    Public MustOverride ReadOnly Property CanHaveTargets As Boolean
    Public MustOverride ReadOnly Property CanHaveMaskers As Boolean
    Public MustOverride ReadOnly Property CanHaveBackgroundNonSpeech As Boolean
    Public MustOverride ReadOnly Property CanHaveBackgroundSpeech As Boolean


    Public ReadOnly Property CurrentlySupportedIrSets As List(Of BinauralImpulseReponseSet)
        Get

            Dim SupportedIrNames As New List(Of String)
            If CustomizableTestOptions.SelectedMediaSet IsNot Nothing Then
                SupportedIrNames = OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(CustomizableTestOptions.SelectedMediaSet.WaveFileSampleRate)
            ElseIf CustomizableTestOptions.SelectedMediaSets IsNot Nothing Then
                SupportedIrNames = OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(CustomizableTestOptions.SelectedMediaSets(0).WaveFileSampleRate)
            Else
                SupportedIrNames = OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(AvailableMediasets(0).WaveFileSampleRate)
            End If

            Dim Output As New List(Of BinauralImpulseReponseSet)
            Dim AvaliableSets = DirectionalSimulator.GetAllDirectionalSimulationSets()
            For Each AvaliableSet In AvaliableSets
                If SupportedIrNames.Contains(AvaliableSet.Key) Then
                    Output.Add(AvaliableSet.Value)
                End If
            Next
            Return Output
        End Get
    End Property

    ''' <summary>
    ''' Returns the set of transducers from OstfBase.AvaliableTransducers expected to work with the currently connected hardware.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property CurrentlySupportedTransducers As List(Of OstfBase.AudioSystemSpecification)
        Get
            Dim Output = New List(Of OstfBase.AudioSystemSpecification)
            Dim AllTransducers = OstfBase.AvaliableTransducers

            'Adding only transducers that can be used with the current sound system.
            For Each Transducer In AllTransducers
                If Transducer.CanPlay() = True Then Output.Add(Transducer)
            Next

            Return Output
        End Get
    End Property

#End Region


    Public Property SoundOverlapDuration As Double = 0

#Region "Test protocol"

    Public Shared Randomizer As Random = New Random

    Public MustOverride ReadOnly Property AvailableTestModes As List(Of TestModes)

    Public Enum TestModes
        ConstantStimuli
        AdaptiveSpeech
        AdaptiveNoise
        AdaptiveDirectionality
    End Enum

    Public MustOverride ReadOnly Property AvailableTestProtocols() As List(Of TestProtocol)

    Public MustOverride ReadOnly Property UseKeyWordScoring As Utils.TriState
    Public MustOverride ReadOnly Property UseListOrderRandomization As Utils.TriState
    Public MustOverride ReadOnly Property UseWithinListRandomization As Utils.TriState
    Public MustOverride ReadOnly Property UseAcrossListRandomization As Utils.TriState
    Public MustOverride ReadOnly Property UseFreeRecall As Utils.TriState
    Public MustOverride ReadOnly Property UseDidNotHearAlternative As Utils.TriState
    Public MustOverride ReadOnly Property AvailableFixedResponseAlternativeCounts() As List(Of Integer)
    Public MustOverride ReadOnly Property UseContralateralMasking As Utils.TriState
    Public MustOverride ReadOnly Property AvailablePhaseAudiometryTypes() As List(Of BmldModes)
    Public MustOverride ReadOnly Property UsePhaseAudiometry As Utils.TriState


#End Region

#Region "RunningTest"

    Public CurrentTestTrial As TestTrial

#End Region

#Region "TestResults"

    Public Shared Function GetAverageScore(ByVal Trials As IEnumerable(Of TestTrial)) As Double?

        Dim ScoreList As New List(Of Integer)
        For Each Trial In Trials
            If Trial.IsCorrect = True Then
                ScoreList.Add(1)
            Else
                ScoreList.Add(0)
            End If
        Next
        If ScoreList.Count > 0 Then
            Return ScoreList.Average
        Else
            Return Nothing
        End If

    End Function

    Public Shared Function GetNumbersOfCorrectTrials(ByVal Trials As IEnumerable(Of TestTrial)) As Double?

        Dim ScoreList As New List(Of Integer)
        For Each Trial In Trials
            If Trial.IsCorrect = True Then
                ScoreList.Add(1)
            Else
                ScoreList.Add(0)
            End If
        Next
        If ScoreList.Count > 0 Then
            Return ScoreList.Sum
        Else
            Return Nothing
        End If

    End Function



#End Region

#Region "Settings"

    Public Property CustomizableTestOptions As CustomizableTestOptions

#End Region


#Region "MustOverride members used in derived classes"

    Public MustOverride Function InitializeCurrentTest() As Boolean

    ''' <summary>
    ''' This method must be implemented in the derived class and must return a decision on what steps to take next. If the next step to take involves a new test trial this method is also responsible for referencing the next test trial in the CurrentTestTrial field.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <returns></returns>
    Public MustOverride Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

    Public Enum SpeechTestReplies
        ContinueTrial
        GotoNextTrial
        TestIsCompleted
        AbortTest
    End Enum


    Public MustOverride Function GetResults() As TestResults

    Public MustOverride Function SaveResults(TestResults As TestResults) As Boolean

#End Region


End Class

