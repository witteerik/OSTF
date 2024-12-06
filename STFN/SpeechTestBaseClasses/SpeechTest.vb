Public MustInherit Class SpeechTest


#Region "Instructions"

    Public MustOverride ReadOnly Property TesterInstructions As String
    Public MustOverride ReadOnly Property ParticipantInstructions As String

#End Region

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

    ''' <summary>
    ''' Holds the level step size available in the customizable test options instance used in the test
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride ReadOnly Property LevelStepSize As Double

    Public MustOverride ReadOnly Property MaximumSoundFieldSpeechLocations As Integer
    Public MustOverride ReadOnly Property MaximumSoundFieldMaskerLocations As Integer
    Public MustOverride ReadOnly Property MaximumSoundFieldBackgroundNonSpeechLocations As Integer
    Public MustOverride ReadOnly Property MaximumSoundFieldBackgroundSpeechLocations As Integer

    Public MustOverride ReadOnly Property MinimumSoundFieldSpeechLocations As Integer
    Public MustOverride ReadOnly Property MinimumSoundFieldMaskerLocations As Integer
    Public MustOverride ReadOnly Property MinimumSoundFieldBackgroundNonSpeechLocations As Integer
    Public MustOverride ReadOnly Property MinimumSoundFieldBackgroundSpeechLocations As Integer

    Public MustOverride ReadOnly Property HasOptionalPractiseTest As Boolean
    Public MustOverride ReadOnly Property AllowsUseRetsplChoice As Boolean
    Public MustOverride ReadOnly Property AllowsManualPreSetSelection As Boolean
    Public MustOverride ReadOnly Property AllowsManualStartListSelection As Boolean
    Public MustOverride ReadOnly Property AllowsManualMediaSetSelection As Boolean
    Public MustOverride ReadOnly Property AllowsManualReferenceLevelSelection As Boolean
    Public MustOverride ReadOnly Property AllowsManualSpeechLevelSelection As Boolean
    Public MustOverride ReadOnly Property AllowsManualMaskingLevelSelection As Boolean
    Public MustOverride ReadOnly Property AllowsManualBackgroundLevelSelection As Boolean
    Public MustOverride ReadOnly Property SupportsPrelistening As Boolean
    Public MustOverride ReadOnly Property CanHaveTargets As Boolean
    Public MustOverride ReadOnly Property CanHaveMaskers As Boolean
    Public MustOverride ReadOnly Property CanHaveBackgroundNonSpeech As Boolean
    Public MustOverride ReadOnly Property CanHaveBackgroundSpeech As Boolean
    Public MustOverride ReadOnly Property UseSoundFieldSimulation As Utils.TriState

    Public MustOverride ReadOnly Property SupportsManualPausing As Boolean


    Public ReadOnly Property CurrentlySupportedIrSets As List(Of BinauralImpulseReponseSet)
        Get
            Dim Output As New List(Of BinauralImpulseReponseSet)

            If OstfBase.AllowDirectionalSimulation = True Then
                Dim SupportedIrNames As New List(Of String)
                If CustomizableTestOptions.SelectedMediaSet IsNot Nothing Then
                    SupportedIrNames = OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(CustomizableTestOptions.SelectedMediaSet.WaveFileSampleRate)
                ElseIf CustomizableTestOptions.SelectedMediaSets IsNot Nothing Then
                    SupportedIrNames = OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(CustomizableTestOptions.SelectedMediaSets(0).WaveFileSampleRate)
                Else
                    SupportedIrNames = OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(AvailableMediasets(0).WaveFileSampleRate)
                End If

                Dim AvaliableSets = DirectionalSimulator.GetAllDirectionalSimulationSets()
                For Each AvaliableSet In AvaliableSets
                    If SupportedIrNames.Contains(AvaliableSet.Key) Then
                        Output.Add(AvaliableSet.Value)
                    End If
                Next
            End If

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

    ''' <summary>
    ''' The sound player crossfade overlap to be used between trials, fade-in and fade-out
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Property SoundOverlapDuration As Double

#Region "Test protocol"

    Public Shared Randomizer As Random = New Random

    Public MustOverride ReadOnly Property AvailableTestModes As List(Of TestModes)

    Public Enum TestModes
        ConstantStimuli
        AdaptiveSpeech
        AdaptiveNoise
        AdaptiveDirectionality
        Custom
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

    ''' <summary>
    ''' If True, speech and noise levels should be interpreted as dB HL. If False, speech and noise levels should be interpreted as dB SPL.
    ''' </summary>
    ''' <returns></returns>
    Public Property UseRetsplCorrection As Boolean
    Public MustOverride ReadOnly Property MinimumLevel As Double
    Public MustOverride ReadOnly Property MaximumLevel As Double

#End Region

#Region "RunningTest"

    Public CurrentTestTrial As TestTrial

    ''' <summary>
    ''' This feid can be used to store information that should be shown on screen during pause. 
    ''' </summary>
    Public PauseInformation As String = ""

    Public AbortInformation As String = ""

    Public MustOverride ReadOnly Property HistoricTrialCount As Integer


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

    ''' <summary>
    ''' Initializes the current test
    ''' </summary>
    ''' <returns>A tuple in which the boolean value indicates success, and the string is an optional message that may be relayed to the user.</returns>
    Public MustOverride Function InitializeCurrentTest() As Tuple(Of Boolean, String)

    ''' <summary>
    ''' This method must be implemented in the derived class and must return a decision on what steps to take next. If the next step to take involves a new test trial this method is also responsible for referencing the next test trial in the CurrentTestTrial field.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <returns></returns>
    Public MustOverride Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

    Public MustOverride Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)

    Public Enum SpeechTestReplies
        ContinueTrial
        GotoNextTrial
        PauseTestingWithCustomInformation
        TestIsCompleted
        AbortTest
    End Enum

    Public MustOverride Sub FinalizeTest()

    Public MustOverride Function GetResultStringForGui() As String

    Public MustOverride Function GetExportString() As String

    Public MustOverride ReadOnly Property FilePathRepresentation As String

    Public Function SaveTableFormatedTestResults() As Boolean

        'Skipping saving data if it's the demo ptc ID
        If SharedSpeechTestObjects.CurrentParticipantID.Trim = SharedSpeechTestObjects.NoTestId Then Return True

        If SharedSpeechTestObjects.TestResultsRootFolder = "" Then
            Messager.MsgBox("Unable to save the results to file due to missing test results output folder. This should have been selected first startup of the app!")
            Return False
        End If

        If IO.Directory.Exists(SharedSpeechTestObjects.TestResultsRootFolder) = False Then
            Try
                IO.Directory.CreateDirectory(SharedSpeechTestObjects.TestResultsRootFolder)
            Catch ex As Exception
                Messager.MsgBox("Unable to save the results to as the test results output folder (" & SharedSpeechTestObjects.TestResultsRootFolder & "). The path does not exist, and could not be created!")
            End Try
            Return False
        End If

        Dim OutputPath = IO.Path.Combine(SharedSpeechTestObjects.TestResultsRootFolder, Me.FilePathRepresentation)
        Dim OutputFilename = Me.FilePathRepresentation & "_Results_" & SharedSpeechTestObjects.CurrentParticipantID

        'Ensures that an old file with the same filename is not overwritten by adding a number to existing files
        OutputFilename = Utils.CheckFileNameConflict(OutputFilename)

        Dim TestResultsString = GetExportString()
        Utils.SendInfoToLog(TestResultsString, OutputFilename, OutputPath, False, True)

        Return True
    End Function


#End Region

#Region "Pretest"

    Public MustOverride Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)

#End Region

End Class

