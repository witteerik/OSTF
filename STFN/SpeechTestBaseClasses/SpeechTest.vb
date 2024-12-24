Imports STFN.Audio.SoundScene
Imports System.ComponentModel
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization
Imports System.Xml.Serialization


<Serializable>
Public MustInherit Class SpeechTest
    Implements INotifyPropertyChanged

    Public MustOverride ReadOnly Property FilePathRepresentation As String

#Region "Initialization"

    Public Sub New(ByVal SpeechMaterialName As String)
        Me.SpeechMaterialName = SpeechMaterialName
        LoadSpeechMaterialSpecification(SpeechMaterialName)
    End Sub

#End Region


#Region "SpeechMaterial"

    Private Function LoadSpeechMaterialSpecification(ByVal SpeechMaterialName As String, Optional ByVal EnforceReloading As Boolean = False) As Boolean

        'Selecting the first available speech material if not specified in the calling code.
        If SpeechMaterialName = "" Then

            Messager.MsgBox("No speech material is selected!" & vbCrLf & "Attempting to select the first speech material available.", Messager.MsgBoxStyle.Information, "Speech material not selected!")

            If AvailableSpeechMaterialSpecifications.Count = 0 Then
                Messager.MsgBox("No speech material is available!" & vbCrLf & "Cannot continue.", Messager.MsgBoxStyle.Information, "Missing speech material!")
                Return False
            Else
                SpeechMaterialName = AvailableSpeechMaterialSpecifications(0)
            End If
        End If

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
    <SkipExport>
    Private Shared Property LoadedSpeechMaterialSpecifications As New SortedList(Of String, SpeechMaterialSpecification)

    'A shared function to load tests
    <SkipExport>
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


#Region "MediaSets"

    <SkipExport>
    Public ReadOnly Property AvailableMediasets() As List(Of MediaSet)
        Get
            SpeechMaterial.ParentTestSpecification.LoadAvailableMediaSetSpecifications()
            Return SpeechMaterial.ParentTestSpecification.MediaSets
        End Get
    End Property

    <SkipExport>
    Public ReadOnly Property AvailablePresets() As List(Of SmcPresets.Preset)
        Get
            Dim Output = New List(Of SmcPresets.Preset)
            For Each AvailablePreset In SpeechMaterial.Presets
                Output.Add(AvailablePreset)
            Next
            Return Output
        End Get
    End Property

    <SkipExport>
    Public Property AvailableExperimentNumbers As Integer() = {}

    <SkipExport>
    Public ReadOnly Property AvailablePractiseListsNames() As List(Of String)
        Get
            Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)
            Dim Output As New List(Of String)
            For Each List In AllLists
                If List.IsPractiseComponent = True Then
                    Output.Add(List.PrimaryStringRepresentation)
                End If
            Next
            Return Output
        End Get
    End Property

    <SkipExport>
    Public ReadOnly Property AvailableTestListsNames() As List(Of String)
        Get
            Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)
            Dim Output As New List(Of String)
            For Each List In AllLists
                If List.IsPractiseComponent = False Then
                    Output.Add(List.PrimaryStringRepresentation)
                End If
            Next
            Return Output
        End Get
    End Property

#End Region
#End Region

#Region "IrSets"

    <SkipExport>
    Public ReadOnly Property CurrentlySupportedIrSets As List(Of BinauralImpulseReponseSet)
        Get
            Dim Output As New List(Of BinauralImpulseReponseSet)

            If OstfBase.AllowDirectionalSimulation = True Then
                Dim SupportedIrNames As New List(Of String)
                If MediaSet IsNot Nothing Then
                    SupportedIrNames = OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(MediaSet.WaveFileSampleRate)
                ElseIf MediaSets.Count > 0 Then
                    SupportedIrNames = OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(MediaSets(0).WaveFileSampleRate)
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
    <SkipExport>
    Public ReadOnly Property CurrentlySupportedTransducers As List(Of OstfBase.AudioSystemSpecification)
        Get
            Dim Output = New List(Of OstfBase.AudioSystemSpecification)
            Dim AllTransducers = OstfBase.AvaliableTransducers

            'Adding only transducers that can be used with the current sound system.
            For Each AvailableTransducer In AllTransducers
                If AvailableTransducer.CanPlay() = True Then Output.Add(AvailableTransducer)
            Next

            Return Output
        End Get
    End Property

#End Region


#Region "GuiInteraction"


    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    ''' <summary>
    '''Set to True to inactivate GUI updates of the test options from the selected test.
    '''The reason we may need to inactivate the GUI connection is that when the GUI is updated asynchronosly, some objects needed during testing may not have been set before they are needed.
    '''Therefore, this value should be changed to True whenever a test is started or resumed, and optionally to False when the test is completed or paused.
    ''' </summary>
    ''' <returns></returns>
    Public SkipGuiUpdates As Boolean = False

    Public Sub OnPropertyChanged(<CallerMemberName> Optional name As String = "")
        If SkipGuiUpdates = False Then
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
        End If
    End Sub

#Region "GuiTexts"


    <SkipExport>
    Public Property TesterInstructionsButtonText As String = "Instruktioner för inställningar"

    <SkipExport>
    Public Property ParticipantInstructionsButtonText As String = "Patientinstruktioner"

    <SkipExport>
    Public Property IsPractiseTestTitle As String = "Övningstest"

    <SkipExport>
    Public Property SelectedPresetTitle As String = "Förval"

    <SkipExport>
    Public Property ExperimentNumberTitle As String = "Experiment nr"

    <SkipExport>
    Public Property StartListTitle As String = "StartLista"

    <SkipExport>
    Public Property SelectedMediaSetTitle As String = "Mediaset"

    <SkipExport>
    Public Property SelectedMediaSetsTitle As String = "Mediaset"

    <SkipExport>
    Public ReadOnly Property ReferenceLevelTitle As String
        Get
            Select Case GuiLanguage
                Case Utils.Constants.Languages.Swedish
                    If LevelsAreIn_dBHL = True Then
                        Return "Referensnivå (dB HL)"
                    Else
                        Return "Referensnivå (dB SPL)"
                    End If
                Case Else
                    If LevelsAreIn_dBHL = True Then
                        Return "Reference level (dB HL)"
                    Else
                        Return "Reference level (dB SPL)"
                    End If
            End Select
        End Get
    End Property

    <SkipExport>
    Public ReadOnly Property TargetLevelTitle As String
        Get
            Select Case GuiLanguage
                Case Utils.Constants.Languages.Swedish
                    If LevelsAreIn_dBHL = True Then
                        Return "Talnivå (dB HL)"
                    Else
                        Return "Talnivå (dB SPL)"
                    End If
                Case Else
                    If LevelsAreIn_dBHL = True Then
                        Return "Speech level (dB HL)"
                    Else
                        Return "Speech level (dB SPL)"
                    End If
            End Select
        End Get
    End Property

    <SkipExport>
    Public ReadOnly Property MaskingLevelTitle As String
        Get
            Select Case GuiLanguage
                Case Utils.Constants.Languages.Swedish
                    If LevelsAreIn_dBHL = True Then
                        Return "Maskeringsnivå (dB HL)"
                    Else
                        Return "Maskeringsnivå (dB SPL)"
                    End If
                Case Else
                    If LevelsAreIn_dBHL = True Then
                        Return "Masking level (dB HL)"
                    Else
                        Return "Masking level (dB SPL)"
                    End If
            End Select
        End Get
    End Property

    <SkipExport>
    Public ReadOnly Property BackgroundLevelTitle As String
        Get
            Select Case GuiLanguage
                Case Utils.Constants.Languages.Swedish
                    If LevelsAreIn_dBHL = True Then
                        Return "Bakgrundsnivå (dB HL)"
                    Else
                        Return "Bakgrundsnivå (dB SPL)"
                    End If
                Case Else
                    If LevelsAreIn_dBHL = True Then
                        Return "Background level (dB HL)"
                    Else
                        Return "Background level (dB SPL)"
                    End If
            End Select
        End Get
    End Property

    <SkipExport>
    Public ReadOnly Property ContralateralMaskingLevelTitle As String
        Get
            Select Case GuiLanguage
                Case Utils.Constants.Languages.Swedish
                    If LevelsAreIn_dBHL = True Then
                        Return "Kontralat. maskeringsnivå (dB HL)"
                    Else
                        Return "Kontralat. maskeringsnivå (dB SPL)"
                    End If
                Case Else
                    If LevelsAreIn_dBHL = True Then
                        Return "Contralat. masking level (dB HL)"
                    Else
                        Return "Contralat. masking level (dB SPL)"
                    End If
            End Select
        End Get
    End Property

    <SkipExport>
    Public Property SelectedTestModeTitle As String = "Testläge"

    <SkipExport>
    Public Property SelectedTestProtocolTitle As String = "Testprotokoll"

    <SkipExport>
    Public Property KeyWordScoringTitle As String = "Rätta på nyckelord"

    <SkipExport>
    Public Property ListOrderRandomizationTitle As String = "Slumpa listordning"

    <SkipExport>
    Public Property WithinListRandomizationTitle As String = "Slumpa inom listor"

    <SkipExport>
    Public Property AcrossListsRandomizationTitle As String = "Slumpa mellan listor"

    <SkipExport>
    Public Property IsFreeRecallTitle As String = "Fri rapportering"

    <SkipExport>
    Public Property IncludeDidNotHearResponseAlternativeTitle As String = "Visa ? som alternativ"

    <SkipExport>
    Public Property FixedResponseAlternativeCountTitle As String = "Antal svarsalternativ"

    <SkipExport>
    Public Property TransducerTitle As String = "Ljudgivare"

    <SkipExport>
    Public Property LevelsAreIn_dBHLTitle As String = "Ange nivåer i dB HL"

    <SkipExport>
    Public Property SimulatedSoundFieldTitle As String = "Simulera ljudfält"

    <SkipExport>
    Public Property IrSetTitle As String = "HRIR"

    <SkipExport>
    Public Property TargetLocationsTitle As String = "Placering av talkälla/or"

    <SkipExport>
    Public Property MaskerLocationsTitle As String = "Placering av maskeringsljud"

    <SkipExport>
    Public Property BackgroundNonSpeechLocationsTitle As String = "Placering av bakgrundsljud"

    <SkipExport>
    Public Property BackgroundSpeechLocationsTitle As String = "Placering av bakgrundstal"

    <SkipExport>
    Public Property ContralateralMaskingTitle As String = "Kontralateral maskering"

    <SkipExport>
    Public Property LockContralateralMaskingTitle As String = "Koppla till talnivå"

    <SkipExport>
    Public Property PhaseAudiometryTitle As String = "Fasaudiometri"

    <SkipExport>
    Public Property PhaseAudiometryTypeTitle As String = "Fasaudiometrityp"


    <SkipExport>
    Public Property PreListenTitle As String = "Provlyssna"

    <SkipExport>
    Public Property PreListenPlayButtonTitle As String = "Spela nästa"

    <SkipExport>
    Public Property PreListenStopButtonTitle As String = "Stop"

    <SkipExport>
    Public Property PreListenLouderButtonTitle As String = "Öka nivån"

    <SkipExport>
    Public Property PreListenSofterButtonTitle As String = "Minska nivån"



#End Region


#Region "GuiSettings"

    ''' <summary>
    ''' Holds the reference level step size available for the settings GUI 
    ''' </summary>
    ''' <returns></returns>
    Public Property ReferenceLevel_StepSize As Double = 1

    ''' <summary>
    ''' Holds the target level step size available for the settings GUI 
    ''' </summary>
    ''' <returns></returns>
    Public Property TargetLevel_StepSize As Double = 1

    ''' <summary>
    ''' Holds the masker level step size available for the settings GUI 
    ''' </summary>
    ''' <returns></returns>
    Public Property MaskingLevel_StepSize As Double = 1

    ''' <summary>
    ''' Holds the background sounds level step size available for the settings GUI 
    ''' </summary>
    ''' <returns></returns>
    Public Property BackgroundLevel_StepSize As Double = 1

    ''' <summary>
    ''' Holds the contralateral masker level step size available for the settings GUI 
    ''' </summary>
    ''' <returns></returns>
    Public Property ContralateralMaskingLevel_StepSize As Double = 1

    Public Property MaximumSoundFieldSpeechLocations As Integer = 1

    Public Property MaximumSoundFieldMaskerLocations As Integer = 5

    Public Property MaximumSoundFieldBackgroundNonSpeechLocations As Integer = 5

    Public Property MaximumSoundFieldBackgroundSpeechLocations As Integer = 5

    Public Property MinimumSoundFieldSpeechLocations As Integer = 1

    Public Property MinimumSoundFieldMaskerLocations As Integer = 0

    Public Property MinimumSoundFieldBackgroundNonSpeechLocations As Integer = 0

    Public Property MinimumSoundFieldBackgroundSpeechLocations As Integer = 0

    Public Property ShowGuiChoice_PractiseTest As Boolean = False

    Public Property ShowGuiChoice_dBHL As Boolean = False

    Public Property ShowGuiChoice_PreSet As Boolean = False

    Public Property ShowGuiChoice_StartList As Boolean = False

    Public Property ShowGuiChoice_MediaSet As Boolean = False

    Public Property ShowGuiChoice_SoundFieldSimulation As Boolean = False

    Public Property ShowGuiChoice_ReferenceLevel As Boolean = False

    Public Overridable ReadOnly Property ShowGuiChoice_TargetLevel As Boolean
        Get
            If CanHaveTargets() = True Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public Overridable ReadOnly Property ShowGuiChoice_MaskingLevel As Boolean
        Get
            If CanHaveMaskers() = True Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public Overridable ReadOnly Property ShowGuiChoice_BackgroundLevel As Boolean
        Get
            If CanHaveBackgroundNonSpeech() = True Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public Overridable ReadOnly Property ShowGuiChoice_ContralateralMaskingLevel As Boolean
        Get
            If CanHaveContralateralMasking() = True Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public Overridable ReadOnly Property ShowGuiChoice_ContralateralMasking As Boolean
        Get
            If CanHaveContralateralMasking() = True Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public Property ShowGuiChoice_TargetLocations As Boolean
        Get
            Return _ShowGuiChoice_TargetLocations
        End Get
        Set(value As Boolean)
            If CanHaveTargets() = True Then
                _ShowGuiChoice_TargetLocations = value
            Else
                _ShowGuiChoice_TargetLocations = False
            End If
            OnPropertyChanged()
        End Set
    End Property
    Private _ShowGuiChoice_TargetLocations As Boolean

    Public Property ShowGuiChoice_MaskerLocations As Boolean
        Get
            Return _ShowGuiChoice_MaskerLocations
        End Get
        Set(value As Boolean)
            If CanHaveMaskers() = True Then
                _ShowGuiChoice_MaskerLocations = value
            Else
                _ShowGuiChoice_MaskerLocations = False
            End If
            OnPropertyChanged()
        End Set
    End Property
    Private _ShowGuiChoice_MaskerLocations As Boolean

    Public Property ShowGuiChoice_BackgroundNonSpeechLocations As Boolean
        Get
            Return _ShowGuiChoice_BackgroundNonSpeechLocations
        End Get
        Set(value As Boolean)
            If CanHaveBackgroundNonSpeech() = True Then
                _ShowGuiChoice_BackgroundNonSpeechLocations = value
            Else
                _ShowGuiChoice_BackgroundNonSpeechLocations = False
            End If
            OnPropertyChanged()
        End Set
    End Property
    Private _ShowGuiChoice_BackgroundNonSpeechLocations As Boolean

    Public Property ShowGuiChoice_BackgroundSpeechLocations As Boolean
        Get
            Return _ShowGuiChoice_BackgroundSpeechLocations
        End Get
        Set(value As Boolean)
            If CanHaveBackgroundSpeech() = True Then
                _ShowGuiChoice_BackgroundSpeechLocations = value
            Else
                _ShowGuiChoice_BackgroundSpeechLocations = False
            End If
            OnPropertyChanged()
        End Set
    End Property
    Private _ShowGuiChoice_BackgroundSpeechLocations As Boolean


    <SkipExport>
    Public Property ShowGuiChoice_KeyWordScoring As Boolean = False

    <SkipExport>
    Public Property ShowGuiChoice_ListOrderRandomization As Boolean = False

    <SkipExport>
    Public Property ShowGuiChoice_WithinListRandomization As Boolean = False

    <SkipExport>
    Public Property ShowGuiChoice_AcrossListRandomization As Boolean = False

    <SkipExport>
    Public Property ShowGuiChoice_FreeRecall As Boolean = False

    <SkipExport>
    Public Property ShowGuiChoice_DidNotHearAlternative As Boolean = False

    <SkipExport>
    Public Property ShowGuiChoice_PhaseAudiometry As Boolean = False


#End Region


#Region "SoundSourceLocationCandidates"

    'The location candidates contains information for displaying the sound source locations graphically
    'They can be collected using a call to either
    'BinauralImpulseReponseSet.GetVisualSoundSourceLocations() or
    'CurrentSpeechTest.Transducer.GetVisualSoundSourceLocations()
    'When used in the GUI, these call are implemented automatically in the TestOptionsView, and done when needed.
    'If not used from the GUI, a call to PopulateSoundSourceLocationCandidates is needed

    Public Sub PopulateSoundSourceLocationCandidates()

        If Transducer IsNot Nothing Then

            If SimulatedSoundField = False Then

                SignalLocationCandidates = Transducer.GetVisualSoundSourceLocations()
                MaskerLocationCandidates = Transducer.GetVisualSoundSourceLocations()
                BackgroundNonSpeechLocationCandidates = Transducer.GetVisualSoundSourceLocations()
                BackgroundSpeechLocationCandidates = Transducer.GetVisualSoundSourceLocations()

            Else

                If IrSet IsNot Nothing Then
                    SignalLocationCandidates = IrSet.GetVisualSoundSourceLocations()
                    MaskerLocationCandidates = IrSet.GetVisualSoundSourceLocations()
                    BackgroundNonSpeechLocationCandidates = IrSet.GetVisualSoundSourceLocations()
                    BackgroundSpeechLocationCandidates = IrSet.GetVisualSoundSourceLocations()

                End If
            End If
        End If

    End Sub

    ''' <summary>
    ''' Should specify the locations from which the signals should come
    ''' </summary>
    ''' <returns></returns>
    <SkipExport>
    Public Property SignalLocationCandidates As List(Of Audio.SoundScene.VisualSoundSourceLocation)
        Get
            Return _SignalLocationCandidates
        End Get
        Set(value As List(Of Audio.SoundScene.VisualSoundSourceLocation))
            _SignalLocationCandidates = value
            OnPropertyChanged()
        End Set
    End Property
    Private _SignalLocationCandidates As New List(Of Audio.SoundScene.VisualSoundSourceLocation)

    ''' <summary>
    ''' Should specify the locations from which the Maskers should come
    ''' </summary>
    ''' <returns></returns>
    <SkipExport>
    Public Property MaskerLocationCandidates As List(Of Audio.SoundScene.VisualSoundSourceLocation)
        Get
            Return _MaskerLocationCandidates
        End Get
        Set(value As List(Of Audio.SoundScene.VisualSoundSourceLocation))
            _MaskerLocationCandidates = value
            OnPropertyChanged()
        End Set
    End Property
    Private _MaskerLocationCandidates As New List(Of Audio.SoundScene.VisualSoundSourceLocation)

    ''' <summary>
    ''' Should specify the locations from which the background (non-speech) sounds should come
    ''' </summary>
    ''' <returns></returns>
    <SkipExport>
    Public Property BackgroundNonSpeechLocationCandidates As List(Of Audio.SoundScene.VisualSoundSourceLocation)
        Get
            Return _BackgroundNonSpeechLocationCandidates
        End Get
        Set(value As List(Of Audio.SoundScene.VisualSoundSourceLocation))
            _BackgroundNonSpeechLocationCandidates = value
            OnPropertyChanged()
        End Set
    End Property
    Private _BackgroundNonSpeechLocationCandidates As New List(Of Audio.SoundScene.VisualSoundSourceLocation)

    ''' <summary>
    ''' Should specify the locations from which the background speech sounds should come
    ''' </summary>
    ''' <returns></returns>
    <SkipExport>
    Public Property BackgroundSpeechLocationCandidates As List(Of Audio.SoundScene.VisualSoundSourceLocation)
        Get
            Return _BackgroundSpeechLocationCandidates
        End Get
        Set(value As List(Of Audio.SoundScene.VisualSoundSourceLocation))
            _BackgroundSpeechLocationCandidates = value
            OnPropertyChanged()
        End Set
    End Property
    Private _BackgroundSpeechLocationCandidates As New List(Of Audio.SoundScene.VisualSoundSourceLocation)

#End Region

#End Region


#Region "TestSettings"


    ''' <summary>
    ''' If specified, should indicate the name of the selected pretest (Optional).
    ''' </summary>
    ''' <returns></returns>
    Public Property Preset As SmcPresets.Preset
        Get
            Return _Preset
        End Get
        Set(value As SmcPresets.Preset)
            _Preset = value
            OnPropertyChanged()
        End Set
    End Property
    Private _Preset As SmcPresets.Preset

    ''' <summary>
    ''' If specified, should contain the (one-based) index of the current experiment in the series of experiments in the current data collection (Optional).
    ''' </summary>
    ''' <returns></returns>
    Public Property ExperimentNumber As Integer
        Get
            Return _ExperimentNumber
        End Get
        Set(value As Integer)
            _ExperimentNumber = value
            OnPropertyChanged()
        End Set
    End Property
    Private _ExperimentNumber As Integer


    ''' <summary>
    ''' If specified, should contain the name of the list to present first (Optional).
    ''' </summary>
    ''' <returns></returns>
    Public Property StartList As String
        Get
            Return _StartList
        End Get
        Set(value As String)
            _StartList = value
            OnPropertyChanged()
        End Set
    End Property
    Private _StartList As String


    ''' <summary>
    ''' Should contain the media sets to be included in the test (Obligatory). (Note that this determines the type of signal, masking and background sounds used, and should be used for instance to select type of masking sounds (i.e. babble noise, SWN, etc., which then need to be implemented as separate MediaSets for each SpeechMaterial)
    ''' </summary>
    ''' <returns></returns>
    Public Property MediaSet As MediaSet
        Get
            Return _MediaSet
        End Get
        Set(value As MediaSet)
            _MediaSet = value
            OnPropertyChanged()
        End Set
    End Property
    Private _MediaSet As MediaSet = Nothing


    ''' <summary>
    ''' Should contain the media sets to be included in the test (Obligatory). (Note that this determines the type of signal, masking and background sounds used, and should be used for instance to select type of masking sounds (i.e. babble noise, SWN, etc., which then need to be implemented as separate MediaSets for each SpeechMaterial)
    ''' </summary>
    ''' <returns></returns>
    Public Property MediaSets As MediaSetLibrary
        Get
            Return _MediaSets
        End Get
        Set(value As MediaSetLibrary)

            'TODO: This is not yet implemented in the TestOptions GUI

            _MediaSets = value
            OnPropertyChanged()
        End Set
    End Property
    Private _MediaSets As New MediaSetLibrary


    Private _MinimumReferenceLevel As Double = 0
    Public Property MinimumReferenceLevel As Double
        Get
            Return _MinimumReferenceLevel
        End Get
        Set(value As Double)
            _MinimumReferenceLevel = value
            OnPropertyChanged()
        End Set
    End Property

    Private _MaximumReferenceLevel As Double = 80
    Public Property MaximumReferenceLevel As Double
        Get
            Return _MaximumReferenceLevel
        End Get
        Set(value As Double)
            _MaximumReferenceLevel = value
            OnPropertyChanged()
        End Set
    End Property

    Private _MinimumLevel_Targets As Double = 0

    Public Property MinimumLevel_Targets As Double
        Get
            Return _MinimumLevel_Targets
        End Get
        Set(value As Double)
            _MinimumLevel_Targets = value
            OnPropertyChanged()
        End Set
    End Property

    Private _MaximumLevel_Targets As Double = 80
    Public Property MaximumLevel_Targets As Double
        Get
            Return _MaximumLevel_Targets
        End Get
        Set(value As Double)
            _MaximumLevel_Targets = value
            OnPropertyChanged()
        End Set
    End Property

    Private _MinimumLevel_Maskers As Double = 0
    Public Property MinimumLevel_Maskers As Double
        Get
            Return _MinimumLevel_Maskers
        End Get
        Set(value As Double)
            _MinimumLevel_Maskers = value
            OnPropertyChanged()
        End Set
    End Property

    Private _MaximumLevel_Maskers As Double = 80
    Public Property MaximumLevel_Maskers As Double
        Get
            Return _MaximumLevel_Maskers
        End Get
        Set(value As Double)
            _MaximumLevel_Maskers = value
            OnPropertyChanged()
        End Set
    End Property

    Private _MinimumLevel_Background As Double = 0
    Public Property MinimumLevel_Background As Double
        Get
            Return _MinimumLevel_Background
        End Get
        Set(value As Double)
            _MinimumLevel_Background = value
            OnPropertyChanged()
        End Set
    End Property

    Private _MaximumLevel_Background As Double = 80
    Public Property MaximumLevel_Background As Double
        Get
            Return _MaximumLevel_Background
        End Get
        Set(value As Double)
            _MaximumLevel_Background = value
            OnPropertyChanged()
        End Set
    End Property

    Private _MinimumLevel_ContralateralMaskers As Double = 0
    Public Property MinimumLevel_ContralateralMaskers As Double
        Get
            Return _MinimumLevel_ContralateralMaskers
        End Get
        Set(value As Double)
            _MinimumLevel_ContralateralMaskers = value
            OnPropertyChanged()
        End Set
    End Property

    Private _MaximumLevel_ContralateralMaskers As Double = 80
    Public Property MaximumLevel_ContralateralMaskers As Double
        Get
            Return _MaximumLevel_ContralateralMaskers
        End Get
        Set(value As Double)
            _MaximumLevel_ContralateralMaskers = value
            OnPropertyChanged()
        End Set
    End Property


    Public Property ReferenceLevel As Double
        Get
            Return _ReferenceLevel
        End Get
        Set(value As Double)
            _ReferenceLevel = Math.Round(Math.Round(value / ReferenceLevel_StepSize) * ReferenceLevel_StepSize)
            _ReferenceLevel = Math.Min(_ReferenceLevel, MaximumReferenceLevel)
            OnPropertyChanged()
        End Set
    End Property
    Private _ReferenceLevel As Double = 68.34


    Public Property TargetLevel As Double
        Get
            Return _TargetLevel
        End Get
        Set(value As Double)
            _TargetLevel = Math.Round(Math.Round(value / TargetLevel_StepSize) * TargetLevel_StepSize)
            _TargetLevel = Math.Min(_TargetLevel, MaximumLevel_Targets)
            OnPropertyChanged()
        End Set
    End Property
    Private _TargetLevel As Double = 65


    Public Property MaskingLevel As Double
        Get
            Return _MaskingLevel
        End Get
        Set(value As Double)
            _MaskingLevel = Math.Round(Math.Round(value / MaskingLevel_StepSize) * MaskingLevel_StepSize)
            _MaskingLevel = Math.Min(_MaskingLevel, MaximumLevel_Maskers)
            OnPropertyChanged()
        End Set
    End Property
    Private _MaskingLevel As Double = 65


    Public Property BackgroundLevel As Double
        Get
            Return _BackgroundLevel
        End Get
        Set(value As Double)
            _BackgroundLevel = Math.Round(Math.Round(value / BackgroundLevel_StepSize) * BackgroundLevel_StepSize)
            _BackgroundLevel = Math.Min(_BackgroundLevel, MaximumLevel_Background)
            OnPropertyChanged()
        End Set
    End Property
    Private _BackgroundLevel As Double = 50


    Public Property ContralateralMaskingLevel As Double
        Get
            Return _ContralateralMaskingLevel
        End Get
        Set(value As Double)
            _ContralateralMaskingLevel = Math.Round(Math.Round(value / ContralateralMaskingLevel_StepSize) * ContralateralMaskingLevel_StepSize)
            _ContralateralMaskingLevel = Math.Min(_ContralateralMaskingLevel, MaximumLevel_ContralateralMaskers)
            OnPropertyChanged()
        End Set
    End Property
    Private _ContralateralMaskingLevel As Double = 25


    ''' <summary>
    ''' The TestMode property is used to determine which type of test protocol that should be used
    ''' </summary>
    ''' <returns></returns>
    Public Property TestMode As SpeechTest.TestModes
        Get
            Return _TestMode
        End Get
        Set(value As SpeechTest.TestModes)
            _TestMode = value
            OnPropertyChanged()
        End Set
    End Property
    Private _TestMode As SpeechTest.TestModes


    Public Property TestProtocol As TestProtocol
        Get
            Return _TestProtocol
        End Get
        Set(value As TestProtocol)
            _TestProtocol = value
            OnPropertyChanged()
        End Set
    End Property
    Private _TestProtocol As TestProtocol


    Public Property KeyWordScoring As Boolean
        Get
            Return _KeyWordScoring
        End Get
        Set(value As Boolean)
            _KeyWordScoring = value
            OnPropertyChanged()
        End Set
    End Property
    Private _KeyWordScoring As Boolean = False


    Public Property ListOrderRandomization As Boolean
        Get
            Return _ListOrderRandomization
        End Get
        Set(value As Boolean)
            _ListOrderRandomization = value
            OnPropertyChanged()
        End Set
    End Property
    Private _ListOrderRandomization As Boolean = False


    Public Property WithinListRandomization As Boolean
        Get
            Return _WithinListRandomization
        End Get
        Set(value As Boolean)
            _WithinListRandomization = value
            OnPropertyChanged()
        End Set
    End Property
    Private _WithinListRandomization As Boolean = False


    Public Property AcrossListsRandomization As Boolean
        Get
            Return _AcrossListsRandomization
        End Get
        Set(value As Boolean)
            _AcrossListsRandomization = value
            OnPropertyChanged()
        End Set
    End Property
    Private _AcrossListsRandomization As Boolean = False

    Public Property IsFreeRecall As Boolean
        Get
            Return _IsFreeRecall
        End Get
        Set(value As Boolean)
            _IsFreeRecall = value
            OnPropertyChanged()
        End Set
    End Property
    Private _IsFreeRecall As Boolean = False


    Public Property IncludeDidNotHearResponseAlternative As Boolean
        Get
            Return _IncludeDidNotHearResponseAlternative
        End Get
        Set(value As Boolean)
            _IncludeDidNotHearResponseAlternative = value
            OnPropertyChanged()
        End Set
    End Property
    Private _IncludeDidNotHearResponseAlternative As Boolean = False


    Public Property FixedResponseAlternativeCount As Integer
        Get
            Return _FixedResponseAlternativeCount
        End Get
        Set(value As Integer)
            _FixedResponseAlternativeCount = value
            OnPropertyChanged()
        End Set
    End Property
    Private _FixedResponseAlternativeCount As Integer = 0


    Public Property Transducer As AudioSystemSpecification
        Get
            Return _Transducer
        End Get
        Set(value As AudioSystemSpecification)
            _Transducer = value

            'Inactivates the use of simulated sound field is the transducer is not headphones
            If _Transducer.IsHeadphones() = False Then SimulatedSoundField = False
            OnPropertyChanged()
        End Set
    End Property
    Private _Transducer As AudioSystemSpecification

    ''' <summary>
    ''' If True, speech and noise levels should be interpreted as dB HL. If False, speech and noise levels should be interpreted as dB SPL.
    ''' </summary>
    ''' <returns></returns>
    Public Property LevelsAreIn_dBHL As Boolean
        Get
            Return _LevelsAreIn_dBHL
        End Get
        Set(value As Boolean)

            'Changing the value only if AllowsUseRetsplChoice is true
            If ShowGuiChoice_dBHL = True Then

                _LevelsAreIn_dBHL = value

                If value = True And SimulatedSoundField = True Then
                    'Inactivates sound field simulation if dB HL values should be used
                    SimulatedSoundField = False
                End If

            End If

            OnPropertyChanged()
        End Set
    End Property
    Private _LevelsAreIn_dBHL As Boolean = False

    Public Property SimulatedSoundField As Boolean
        Get
            Return _SimulatedSoundField
        End Get
        Set(value As Boolean)
            _SimulatedSoundField = value

            If value = True And LevelsAreIn_dBHL = True Then
                'Inactivates UseRetsplCorrection to prohibit the use of dB HL in sound field simulations
                LevelsAreIn_dBHL = False
            End If

            OnPropertyChanged()
        End Set
    End Property
    Private _SimulatedSoundField As Boolean = False

    Public Property IrSet As BinauralImpulseReponseSet
        Get
            Return _IrSet
        End Get
        Set(value As BinauralImpulseReponseSet)
            _IrSet = value
            OnPropertyChanged()
        End Set
    End Property
    Private _IrSet As BinauralImpulseReponseSet = Nothing


    'Returns the selected SelectedPresentationMode indirectly based on the UseSimulatedSoundField property. In the future if more options are supported, this will have to be exposed to the GUI.
    Public ReadOnly Property PresentationMode As SoundPropagationTypes
        Get
            If SimulatedSoundField = False Then
                Return SoundPropagationTypes.PointSpeakers
            Else
                Return SoundPropagationTypes.SimulatedSoundField
            End If
        End Get
    End Property


    ''' <summary>
    ''' Returns the selected locations from which the signals should come. 
    ''' If not selected in the GUI, selection of sound-source candidates must be doen in code, after a call to PopulateSoundSourceLocationCandidates (after the selection of transducer and, if needed, IrSet.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property SignalLocations As List(Of Audio.SoundScene.SoundSourceLocation)
        Get
            Dim Output As New List(Of Audio.SoundScene.SoundSourceLocation)
            For Each location In _SignalLocationCandidates
                If location.Selected = True Then Output.Add(location.ParentSoundSourceLocation)
            Next
            Return Output
        End Get
    End Property

    ''' <summary>
    ''' Should specify the locations from which the maskers should come
    ''' If not selected in the GUI, selection of sound-source candidates must be doen in code, after a call to PopulateSoundSourceLocationCandidates (after the selection of transducer and, if needed, IrSet.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MaskerLocations As List(Of Audio.SoundScene.SoundSourceLocation)
        Get
            Dim Output As New List(Of Audio.SoundScene.SoundSourceLocation)
            For Each Item In _MaskerLocationCandidates
                If Item.Selected = True Then Output.Add(Item.ParentSoundSourceLocation)
            Next
            Return Output
        End Get
    End Property


    ''' <summary>
    ''' Should specify the locations from which the BackgroundNonSpeech non-speech sounds should come
    ''' If not selected in the GUI, selection of sound-source candidates must be doen in code, after a call to PopulateSoundSourceLocationCandidates (after the selection of transducer and, if needed, IrSet.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property BackgroundNonSpeechLocations As List(Of Audio.SoundScene.SoundSourceLocation)
        Get
            Dim Output As New List(Of Audio.SoundScene.SoundSourceLocation)
            For Each Item In _BackgroundNonSpeechLocationCandidates
                If Item.Selected = True Then Output.Add(Item.ParentSoundSourceLocation)
            Next
            Return Output
        End Get
    End Property


    ''' <summary>
    ''' Should specify the locations from which the BackgroundSpeech speech sounds should come
    ''' If not selected in the GUI, selection of sound-source candidates must be doen in code, after a call to PopulateSoundSourceLocationCandidates (after the selection of transducer and, if needed, IrSet.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property BackgroundSpeechLocations As List(Of Audio.SoundScene.SoundSourceLocation)
        Get
            Dim Output As New List(Of Audio.SoundScene.SoundSourceLocation)
            For Each Item In _BackgroundSpeechLocationCandidates
                If Item.Selected = True Then Output.Add(Item.ParentSoundSourceLocation)
            Next
            Return Output
        End Get
    End Property


    ''' <summary>
    ''' This is intended as a shortcut which must override the MaskerLocations property, but can only be specified if SelectedPresentationMode is PointSpeakers at -90 and 90 degrees with distance of 0 (i.e. headphones), and where the signal i set to come from only one side.
    ''' </summary>
    ''' <returns></returns>
    Public Property ContralateralMasking As Boolean
        Get
            Return _ContralateralMasking
        End Get
        Set(value As Boolean)
            _ContralateralMasking = value
            OnPropertyChanged()
        End Set
    End Property
    Private _ContralateralMasking As Boolean = False




    ''' <summary>
    ''' This is intended as a shortcut which must override all of the SignalLocations, MaskerLocations and BackgroundLocations properties. It can only be specified if SelectedPresentationMode is either SimulatedSoundField with locations along the mid-sagittal plane, or PointSpeakers at -90 and 90 degrees with distance of 0 (i.e. headphones), and where the signal i set to come from only one side.
    ''' </summary>
    ''' <returns></returns>
    Public Property PhaseAudiometry As Boolean
        Get
            Return _PhaseAudiometry
        End Get
        Set(value As Boolean)
            _PhaseAudiometry = value
            OnPropertyChanged()
        End Set
    End Property
    Private _PhaseAudiometry As Boolean = False


    ''' <summary>
    ''' Determines which type of phase audiometry to use, if UsePhaseAudiometry is True.
    ''' </summary>
    ''' <returns></returns>
    Public Property PhaseAudiometryType As BmldModes
        Get
            Return _PhaseAudiometryType
        End Get
        Set(value As BmldModes)
            _PhaseAudiometryType = value
            OnPropertyChanged()
        End Set
    End Property
    Private _PhaseAudiometryType As BmldModes


#End Region



#Region "SoundSceneGeneration"


    ''' <summary>
    ''' The sound player crossfade overlap to be used between trials, fade-in and fade-out
    ''' </summary>
    ''' <returns></returns>
    Public Property SoundOverlapDuration As Double = 0.1


    Public Function CanHaveTargets() As Boolean
        If SpeechMaterial IsNot Nothing Then
            If MediaSet IsNot Nothing Then
                If MediaSet.MediaAudioItems > 0 Then
                    Return True
                End If
            Else
                If AvailableMediasets.Count > 0 Then
                    If AvailableMediasets(0).MediaAudioItems > 0 Then
                        Return True
                    End If
                End If
            End If
        End If
        'Returns False otherwise
        Return False
    End Function

    Public Function CanHaveMaskers() As Boolean
        If SpeechMaterial IsNot Nothing Then
            If MediaSet IsNot Nothing Then
                If MediaSet.MaskerAudioItems > 0 Then
                    Return True
                End If
            Else
                If AvailableMediasets.Count > 0 Then
                    If AvailableMediasets(0).MaskerAudioItems > 0 Then
                        Return True
                    End If
                End If
            End If
        End If
        'Returns False otherwise
        Return False
    End Function

    Public Function CanHaveBackgroundNonSpeech() As Boolean

        If SpeechMaterial IsNot Nothing Then
            If MediaSet IsNot Nothing Then
                'TODO: This is not a good solution, as it doesn't really specify the number of available sound files. Consider adding BackgroundNonspeechAudioItems to the MediaSet specification
                If MediaSet.BackgroundNonspeechParentFolder.Trim <> "" Then
                    Return True
                End If
            Else
                If AvailableMediasets.Count > 0 Then
                    If AvailableMediasets(0).BackgroundNonspeechParentFolder.Trim <> "" Then
                        Return True
                    End If
                End If
            End If
        End If
        'Returns False otherwise
        Return False
    End Function

    Public Function CanHaveBackgroundSpeech() As Boolean

        If SpeechMaterial IsNot Nothing Then
            If MediaSet IsNot Nothing Then
                'TODO: This is not a good solution, as it doesn't really specify the number of available sound files. Consider adding BackgroundNonspeechAudioItems to the MediaSet specification
                If MediaSet.BackgroundSpeechParentFolder.Trim <> "" Then
                    Return True
                End If
            Else
                If AvailableMediasets.Count > 0 Then
                    If AvailableMediasets(0).BackgroundSpeechParentFolder.Trim <> "" Then
                        Return True
                    End If
                End If
            End If
        End If
        'Returns False otherwise
        Return False
    End Function

    Public Function CanHaveContralateralMasking() As Boolean
        If SpeechMaterial IsNot Nothing Then
            If MediaSet IsNot Nothing Then
                If MediaSet.ContralateralMaskerAudioItems > 0 Then
                    Return True
                End If
            Else
                If AvailableMediasets.Count > 0 Then
                    If AvailableMediasets(0).ContralateralMaskerAudioItems > 0 Then
                        Return True
                    End If
                End If
            End If
        End If
        'Returns False otherwise
        Return False
    End Function

    ''' <summary>
    ''' Returns the difference between the ContralateralMaskingLevel and the SpeechLevel (i.e. ContralateralMaskingLevel - SpeechLevel)
    ''' </summary>
    ''' <returns></returns>
    Public Function ContralateralLevelDifference() As Double?
        If ContralateralMasking = True Then
            Return ContralateralMaskingLevel - TargetLevel
        Else
            Return Nothing
        End If
    End Function




    ''' <summary>
    ''' This sub mixes targets, maskers, background-non-speech, background-speech and contralateral maskers as assigned in TestOptions sound sources. The mixed sound is stored in the CurrentTestTrial.Sound.
    ''' </summary>
    ''' <param name="UseNominalLevels">If True, applied gains are based on the nominal levels stored in the SMA object of each sound. If False, sound levels are re-calculated.</param>
    ''' <param name="MaximumSoundDuration">The intended duration (ins seconds) of the mixed sound.</param>
    ''' <param name="TargetLevel">The combined level of all targets.</param>
    ''' <param name="TargetPresentationTime">The insertion time of the targets</param>
    ''' <param name="MaskerLevel">The combined level of all maskers.</param>
    ''' <param name="MaskerPresentationTime">The insertion time of the maskers</param>
    ''' <param name="BackgroundNonSpeechLevel">The combined level of all background non-speech sources.</param>
    ''' <param name="BackgroundNonSpeechPresentationTime">The insertion time of the background non-speech sounds</param>
    ''' <param name="BackgroundSpeechLevel">The combined level of all background speech sources</param>
    ''' <param name="BackgroundSpeechPresentationTime">The insertion time of the background-speech sounds</param>
    ''' <param name="ContralateralMaskerLevel">The level of the contralateral masker. Should be specified without correction for efficient masking (EM). (EM is added internally in the function.)</param>
    ''' <param name="ContralateralMaskerPresentationTime">The insertion time of the contralateral maskers</param>
    ''' <param name="FadeSpecs_Target">Optional fading specifications for the targets. If not specified, the function supplies default values that will be used.</param>
    ''' <param name="FadeSpecs_Masker">Optional fading specifications for the maskers. If not specified, the function supplies default values that will be used.</param>
    ''' <param name="FadeSpecs_BackgroundNonSpeech">Optional fading specifications for the background non-speech sounds. If not specified, the function supplies default values that will be used.</param>
    ''' <param name="FadeSpecs_BackgroundSpeech">Optional fading specifications for the background-speech sounds. If not specified, the function supplies default values that will be used.</param>
    ''' <param name="FadeSpecs_ContralateralMasker">Optional fading specifications for the contralateral masker. If not specified, the function supplies default values that will be used.</param>
    ''' <param name="ExportSounds">Can be used to debug or analyse presented sounds. Default value is False. Sounds are stored into the current log folder.</param>
    Protected Sub MixStandardTestTrialSound(ByVal UseNominalLevels As Boolean, ByVal MaximumSoundDuration As Double,
                                  ByVal TargetLevel As Double, ByVal TargetPresentationTime As Double,
                                  Optional ByVal MaskerLevel As Nullable(Of Double) = Nothing, Optional ByVal MaskerPresentationTime As Double = 0,
                                  Optional ByVal BackgroundNonSpeechLevel As Nullable(Of Double) = Nothing, Optional ByVal BackgroundNonSpeechPresentationTime As Double = 0,
                                  Optional ByVal BackgroundSpeechLevel As Nullable(Of Double) = Nothing, Optional ByVal BackgroundSpeechPresentationTime As Double = 0,
                                  Optional ByVal ContralateralMaskerLevel As Nullable(Of Double) = Nothing, Optional ByVal ContralateralMaskerPresentationTime As Double = 0,
                                  Optional ByRef FadeSpecs_Target As List(Of STFN.Audio.DSP.Transformations.FadeSpecifications) = Nothing,
                                  Optional ByRef FadeSpecs_Masker As List(Of STFN.Audio.DSP.Transformations.FadeSpecifications) = Nothing,
                                  Optional ByRef FadeSpecs_BackgroundNonSpeech As List(Of STFN.Audio.DSP.Transformations.FadeSpecifications) = Nothing,
                                  Optional ByRef FadeSpecs_BackgroundSpeech As List(Of STFN.Audio.DSP.Transformations.FadeSpecifications) = Nothing,
                                  Optional ByRef FadeSpecs_ContralateralMasker As List(Of STFN.Audio.DSP.Transformations.FadeSpecifications) = Nothing,
                                            Optional ExportSounds As Boolean = False)

        'TODO: This function is not finished, it still need implementation of BackgroundNonSpeech and BackgroundSpeech

        'Calculates the EM corrected Contralateral masker level (the level supplied should not be EM corrected but be as it would appear on an audiometer attenuator
        Dim ContralateralMaskerLevel_EmCorrected As Double = ContralateralMaskerLevel + MediaSet.EffectiveContralateralMaskingGain

        'Mix the signal using DuxplexMixer CreateSoundScene
        'Sets a List of SoundSceneItem in which to put the sounds to mix
        Dim ItemList = New List(Of SoundSceneItem)
        Dim LevelGroup As Integer = 1 ' The level group value is used to set the added sound level of items sharing the same (arbitrary) LevelGroup value to the indicated sound level. (Thus, the sounds with the same LevelGroup value are measured together.)
        Dim CurrentSampleRate As Integer = -1

        'Determining test ear and stores in the current test trial (This should perhaps be moved outside this function. On the other hand it's good that it's always detemined when sounds are mixed, though all tests need to implement this or call this code)
        Dim CurrentTestEar As Utils.SidesWithBoth = Utils.SidesWithBoth.Both ' Assuming both, and overriding if needed
        If Transducer.IsHeadphones = True Then
            If SimulatedSoundField = False Then
                Dim HasLeftSideTarget As Boolean = False
                Dim HasRightSideTarget As Boolean = False

                For Each SignalLocation In SignalLocations
                    If SignalLocation.HorizontalAzimuth > 0 Then
                        'At least one signal location is to the right
                        HasRightSideTarget = True
                    End If
                    If SignalLocation.HorizontalAzimuth < 0 Then
                        'At least one signal location is to the left
                        HasLeftSideTarget = True
                    End If
                Next

                'Overriding the value Both if signal is only the left or only the right side
                If HasLeftSideTarget = True And HasRightSideTarget = False Then
                    CurrentTestEar = Utils.Constants.SidesWithBoth.Left
                ElseIf HasLeftSideTarget = False And HasRightSideTarget = True Then
                    CurrentTestEar = Utils.Constants.SidesWithBoth.Right
                End If
            End If
        End If
        CurrentTestTrial.TestEar = CurrentTestEar


        ' **TARGET SOUNDS**
        If SignalLocations.Count > 0 Then

            'Getting the target sound (i.e. test words)
            Dim TargetSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(MediaSet, 0, 1, , , , , False, False, False, , , False)

            'Storing the samplerate
            CurrentSampleRate = TargetSound.WaveFormat.SampleRate

            'Setting the insertion sample of the target
            Dim TargetStartSample As Integer = Math.Floor(TargetPresentationTime * CurrentSampleRate)

            'Setting the TargetStartMeasureSample (i.e. the sample index in the TargetSound, not in the final mix)
            Dim TargetStartMeasureSample As Integer = 0

            'Getting the TargetMeasureLength from the length of the sound files (i.e. everything is measured)
            Dim TargetMeasureLength As Integer = TargetSound.WaveData.SampleData(1).Length

            'Sets up default fading specifications for the target
            If FadeSpecs_Target Is Nothing Then
                FadeSpecs_Target = New List(Of STFN.Audio.DSP.Transformations.FadeSpecifications)
                FadeSpecs_Target.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.002))
                FadeSpecs_Target.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.002))
            End If

            'Combining targets with the selected SignalLocations
            Dim Targets As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation))
            For Each SignalLocation In SignalLocations
                'Re-using the same target in all selected locations
                Targets.Add(New Tuple(Of Audio.Sound, SoundSourceLocation)(TargetSound, SignalLocation))
            Next

            'Adding the targets sources to the ItemList
            For Index = 0 To Targets.Count - 1
                ItemList.Add(New SoundSceneItem(Targets(Index).Item1, 1, TargetLevel, LevelGroup, Targets(Index).Item2, SoundSceneItem.SoundSceneItemRoles.Target, TargetStartSample, TargetStartMeasureSample, TargetMeasureLength,, FadeSpecs_Target))
            Next

            'Incrementing LevelGroup 
            LevelGroup += 1

            'Storing data in the CurrentTestTrial
            CurrentTestTrial.LinguisticSoundStimulusStartTime = TargetPresentationTime
            CurrentTestTrial.LinguisticSoundStimulusDuration = TargetSound.WaveData.SampleData(1).Length / TargetSound.WaveFormat.SampleRate

        End If


        ' **MASKER SOUNDS**
        If MaskerLocations.Count > 0 Then

            'Ensures that MaskerLevel has a value
            If MaskerLevel.HasValue = False Then Throw New ArgumentException("MaskerLevel value cannot be Nothing!")

            'Getting the masker sound
            Dim MaskerSound = CurrentTestTrial.SpeechMaterialComponent.GetMaskerSound(MediaSet, 0)

            'Storing the samplerate
            If CurrentSampleRate = -1 Then CurrentSampleRate = MaskerSound.WaveFormat.SampleRate

            'Setting the insertion sample of the masker
            Dim MaskerStartSample As Integer = Math.Floor(MaskerPresentationTime * CurrentSampleRate)

            'Setting the MaskerStartMeasureSample (i.e. the sample index in the MaskerSound, not in the final mix)
            Dim MaskerStartMeasureSample As Integer = 0

            'Getting the MaskerMeasureLength from the length of the sound files (i.e. everything is measured)
            Dim MaskerMeasureLength As Integer = MaskerSound.WaveData.SampleData(1).Length

            'Sets up fading specifications for the masker
            If FadeSpecs_Masker Is Nothing Then
                FadeSpecs_Masker = New List(Of STFN.Audio.DSP.Transformations.FadeSpecifications)
                FadeSpecs_Masker.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.002))
                FadeSpecs_Masker.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.002))
            End If

            'Combining maskers with the selected SignalLocations
            Dim Maskers As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation))

            'Randomizing a start sample in the first half of the masker signal, and then picking different sections of the masker sound, two seconds apart for the different locations.
            Dim RandomStartReadIndex As Integer = Randomizer.Next(0, (MaskerSound.WaveData.SampleData(1).Length / 2) - 1)
            Dim InterMaskerStepLength As Integer = CurrentSampleRate * 2
            Dim IndendedMaskerLength As Integer = MaximumSoundDuration * CurrentSampleRate - MaskerStartSample

            'Calculating the needed sound length and checks that the masker sound is long enough
            Dim NeededSoundLength As Integer = RandomStartReadIndex + (MaskerLocations.Count + 1) * InterMaskerStepLength + IndendedMaskerLength + 10
            If MaskerSound.WaveData.SampleData(1).Length < NeededSoundLength Then
                Throw New Exception("The masker sound specified is too short for the intended maximum sound duration and " & MaskerLocations.Count & " sound sources!")
            End If

            'Picking the masker sounds and combining them with their selected locations
            For Index = 0 To MaskerLocations.Count - 1

                Dim StartCopySample As Integer = RandomStartReadIndex + Index * InterMaskerStepLength
                Dim CurrentSourceMaskerSound = Audio.DSP.CopySection(MaskerSound, StartCopySample, IndendedMaskerLength, 1)

                'Copying the SMA object to retain the nominal level (although other time data and other related stuff will be incorrect, if not adjusted for)
                CurrentSourceMaskerSound.SMA = MaskerSound.SMA.CreateCopy(CurrentSourceMaskerSound)

                'Picking the masker sound
                Maskers.Add(New Tuple(Of Audio.Sound, SoundSourceLocation)(CurrentSourceMaskerSound, MaskerLocations(Index)))

            Next

            'Adding the maskers sources to the ItemList
            For Index = 0 To Maskers.Count - 1
                ItemList.Add(New SoundSceneItem(Maskers(Index).Item1, 1, MaskerLevel, LevelGroup, Maskers(Index).Item2, SoundSceneItem.SoundSceneItemRoles.Masker, MaskerStartSample, MaskerStartMeasureSample, MaskerMeasureLength,, FadeSpecs_Masker))
            Next

            'Incrementing LevelGroup 
            LevelGroup += 1

        End If

        'TODO: implement BackgroundNonSpeech and BackgroundSpeech here

        ' **CONTRALATERAL MASKER**
        If ContralateralMasking = True Then

            'Ensures that ContralateralMaskerLevel has a value
            If ContralateralMaskerLevel.HasValue = False Then Throw New ArgumentException("ContralateralMaskerLevel value cannot be Nothing!")

            'Ensures that head phones are used
            If Transducer.IsHeadphones = False Then
                Throw New Exception("Contralateral masking cannot be used without headphone presentation.")
            End If

            'Ensures that it's not a simulated sound field
            If SimulatedSoundField = True Then
                Throw New Exception("Contralateral masking cannot be used in a simulated sound field!")
            End If

            'Getting the contralateral masker sound 
            Dim FullContralateralMaskerSound = CurrentTestTrial.SpeechMaterialComponent.GetContralateralMaskerSound(MediaSet, 0)

            'Storing the samplerate
            If CurrentSampleRate = -1 Then CurrentSampleRate = FullContralateralMaskerSound.WaveFormat.SampleRate

            'Setting the insertion sample of the contralateral masker
            Dim ContralateralMaskerStartSample As Integer = Math.Floor(ContralateralMaskerPresentationTime * CurrentSampleRate)

            'Setting the ContralateralMaskerStartMeasureSample (i.e. the sample index in the ContralateralMaskerSound, not in the final mix)
            Dim ContralateralMaskerStartMeasureSample As Integer = 0

            'Picking a random section of the ContralateralMaskerSound, starting in the first half
            Dim RandomStartReadIndex As Integer = Randomizer.Next(0, (FullContralateralMaskerSound.WaveData.SampleData(1).Length / 2) - 1)
            Dim IndendedMaskerLength As Integer = MaximumSoundDuration * CurrentSampleRate - ContralateralMaskerStartSample

            'Calculating the needed sound length and checks that the contralateral masker sound is long enough
            Dim NeededSoundLength As Integer = RandomStartReadIndex + IndendedMaskerLength + 10
            If FullContralateralMaskerSound.WaveData.SampleData(1).Length < NeededSoundLength Then
                Throw New Exception("The contralateral masker sound specified is too short for the intended maximum sound duration!")
            End If

            'Gets a copy of the sound section
            Dim ContralateralMaskerSound = Audio.DSP.CopySection(FullContralateralMaskerSound, RandomStartReadIndex, IndendedMaskerLength, 1)

            'Copying the SMA object to retain the nominal level (although other time data and other related stuff will be incorrect, if not adjusted for)
            ContralateralMaskerSound.SMA = FullContralateralMaskerSound.SMA.CreateCopy(ContralateralMaskerSound)

            'Getting the ContralateralMaskerMeasureLength from the length of the sound files (i.e. everything is measured)
            Dim ContralateralMaskerMeasureLength As Integer = ContralateralMaskerSound.WaveData.SampleData(1).Length

            'Sets up fading specifications for the contralateral masker
            If FadeSpecs_ContralateralMasker Is Nothing Then
                FadeSpecs_ContralateralMasker = New List(Of STFN.Audio.DSP.Transformations.FadeSpecifications)
                FadeSpecs_ContralateralMasker.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.002))
                FadeSpecs_ContralateralMasker.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.002))
            End If

            'Determining which side to put the contralateral masker
            Dim ContralateralMaskerLocation As New SoundSourceLocation With {.Distance = 0, .Elevation = 0}
            If CurrentTestEar = Utils.Constants.SidesWithBoth.Left Then
                'Putting contralateral masker in right ear
                ContralateralMaskerLocation.HorizontalAzimuth = 90
            ElseIf CurrentTestEar = Utils.Constants.SidesWithBoth.Right Then
                'Putting contralateral masker in left ear
                ContralateralMaskerLocation.HorizontalAzimuth = -90
            Else
                'This shold never happen...
                Throw New Exception("Contralateral noise cannot be used when the target signal is on both sides!")
            End If

            'Adding the contralateral maskers sources to the ItemList
            ItemList.Add(New SoundSceneItem(ContralateralMaskerSound, 1, ContralateralMaskerLevel_EmCorrected, LevelGroup, ContralateralMaskerLocation, SoundSceneItem.SoundSceneItemRoles.ContralateralMasker, ContralateralMaskerStartSample, ContralateralMaskerStartMeasureSample, ContralateralMaskerMeasureLength,, FadeSpecs_ContralateralMasker))

            'Incrementing LevelGroup 
            LevelGroup += 1

        End If


        Dim CurrentSoundPropagationType As SoundPropagationTypes = SoundPropagationTypes.PointSpeakers
        If SimulatedSoundField Then
            CurrentSoundPropagationType = SoundPropagationTypes.SimulatedSoundField
            'TODO: This needs to be modified if/when more SoundPropagationTypes are starting to be supported
        End If

        'Creating the mix by calling CreateSoundScene of the current Mixer
        CurrentTestTrial.Sound = Transducer.Mixer.CreateSoundScene(ItemList, UseNominalLevels, LevelsAreIn_dBHL, CurrentSoundPropagationType, Transducer.LimiterThreshold, ExportSounds, CurrentTestTrial.Spelling)


        'TODO: Reasonably this method should only store values into the CurrentTestTrial that are derived within this function! Leaving these for now
        CurrentTestTrial.MediaSetName = MediaSet.MediaSetName
        CurrentTestTrial.UseContralateralNoise = ContralateralMasking
        CurrentTestTrial.EfficientContralateralMaskingTerm = MediaSet.EffectiveContralateralMaskingGain

    End Sub




#End Region



#Region "Test protocol"

    Public Property TesterInstructions As String = ""

    Public Property ParticipantInstructions As String = ""

    Protected RandomSeed As Integer? = Nothing

    Public Shared Randomizer As Random = New Random


    ''' <summary>
    ''' Should indicate whether the test is a practise test or not (Obligatory).
    ''' </summary>
    ''' <returns></returns>
    Public Property IsPractiseTest As Boolean
        Get
            Return _IsPractiseTest
        End Get
        Set(value As Boolean)
            _IsPractiseTest = value
            OnPropertyChanged()
        End Set
    End Property
    Private _IsPractiseTest As Boolean = False


    Public Property SupportsManualPausing As Boolean = False


    <SkipExport>
    Public Property SupportsPrelistening As Boolean = False


    <SkipExport>
    Public Property AvailableTestModes As List(Of TestModes) = New List(Of TestModes) From {TestModes.ConstantStimuli, TestModes.AdaptiveNoise, TestModes.AdaptiveSpeech}

    Public Enum TestModes
        ConstantStimuli
        AdaptiveSpeech
        AdaptiveNoise
        AdaptiveDirectionality
        Custom
    End Enum

    <SkipExport>
    Public Property AvailableTestProtocols As List(Of TestProtocol) = New List(Of TestProtocol)

    <SkipExport>
    Public Property AllTestProtocols As List(Of TestProtocol) = New List(Of TestProtocol) From {
                New SrtSwedishHint2018_TestProtocol,
                New BrandKollmeier2002_TestProtocol,
                New FixedLengthWordsInNoise_WithPreTestLevelAdjustment_TestProtocol,
                New HagermanKinnefors1995_TestProtocol,
                New SrtAsha1979_TestProtocol,
                New SrtChaiklinFontDixon1967_TestProtocol,
                New SrtChaiklinVentry1964_TestProtocol,
                New SrtIso8253_TestProtocol,
                New SrtSwedishHint2018_TestProtocol}


    <SkipExport>
    Public Property AvailableFixedResponseAlternativeCounts As List(Of Integer) = New List(Of Integer)

    <SkipExport>
    Public Property AvailablePhaseAudiometryTypes As List(Of BmldModes) = New List(Of BmldModes)


#End Region

#Region "RunningTest"

    Public CurrentTestTrial As TestTrial

    ''' <summary>
    ''' This feid can be used to store information that should be shown on screen during pause. 
    ''' </summary>
    Public PauseInformation As String = ""

    Public AbortInformation As String = ""

    Public Property HistoricTrialCount As Integer = 0

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

    Public MustOverride Function GetTestResultsExportString() As String

    Public MustOverride Function GetTestTrialResultExportString() As String



    Public Function SaveTestTrialResults() As Boolean

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
                Messager.MsgBox("Unable to save the results to the test results output folder (" & SharedSpeechTestObjects.TestResultsRootFolder & "). The path does not exist, and could not be created!")
            End Try
            Return False
        End If

        Dim OutputPath = IO.Path.Combine(SharedSpeechTestObjects.TestResultsRootFolder, Me.FilePathRepresentation)
        Dim OutputFilename = Me.FilePathRepresentation & "_TrialResults_" & SharedSpeechTestObjects.CurrentParticipantID

        Dim TestTrialResultsString = GetTestTrialResultExportString()
        Utils.SendInfoToLog(TestTrialResultsString, OutputFilename, OutputPath, False, True, False, True, True)

        Return True

    End Function

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
                Messager.MsgBox("Unable to save the results to the test results output folder (" & SharedSpeechTestObjects.TestResultsRootFolder & "). The path does not exist, and could not be created!")
            End Try
            Return False
        End If

        Dim OutputPath = IO.Path.Combine(SharedSpeechTestObjects.TestResultsRootFolder, Me.FilePathRepresentation)
        Dim OutputFilename = Me.FilePathRepresentation & "_Results_" & SharedSpeechTestObjects.CurrentParticipantID

        Dim TestResultsString = GetTestResultsExportString()
        Utils.SendInfoToLog(TestResultsString, OutputFilename, OutputPath, False, True, False, False, True)

        Return True
    End Function


#End Region

#Region "Pretest"

    Public MustOverride Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)

#End Region

    Public Function ExportColumnHeadings() As String

        Dim OutputList As New List(Of String)

        'Adding property names
        Dim properties As PropertyInfo() = GetType(SpeechTest).GetProperties()

        ' Iterating through each property
        For Each [property] As PropertyInfo In properties

            ' Getting the name of the property
            Dim propertyName As String = [property].Name
            OutputList.Add(propertyName)

        Next

        Return String.Join(vbTab, OutputList)

    End Function

    Public Function ExportSettingsAsTextRow() As String

        Dim OutputList As New List(Of String)

        Dim properties As PropertyInfo() = GetType(SpeechTest).GetProperties()

        ' Iterating through each property
        For Each [property] As PropertyInfo In properties

            ' Getting the name of the property
            Dim propertyName As String = [property].Name

            ' Getting the value of the property for the current instance 
            Dim propertyValue As Object = [property].GetValue(Me)

            If propertyValue IsNot Nothing Then

                ' Check if the property value is a List(Of T)
                Dim propertyType As Type = propertyValue.GetType()
                If propertyType.IsGenericType AndAlso propertyType.GetGenericTypeDefinition() = GetType(List(Of)) Then

                    ' Iterate through the List(Of T) and call ToString on each item
                    Dim items As New List(Of String)
                    Dim enumerable As IEnumerable = DirectCast(propertyValue, IEnumerable)
                    For Each item In enumerable
                        items.Add(item.ToString())
                    Next

                    ' Join the items with commas (or any separator you prefer)
                    OutputList.Add(String.Join("; ", items))

                Else
                    OutputList.Add(propertyValue.ToString())
                End If

            Else
                OutputList.Add("NotSet")
            End If

        Next

        Return String.Join(vbTab, OutputList)

    End Function


End Class



<AttributeUsage(AttributeTargets.Property)>
Public Class SkipExportAttribute
    Inherits Attribute
End Class