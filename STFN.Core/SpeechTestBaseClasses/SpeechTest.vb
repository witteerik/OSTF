Imports STFN.Core.Audio.SoundScene
Imports System.ComponentModel
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices.JavaScript.JSType
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

    Protected IsInitialized As Boolean = False

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
    <ExludeFromPropertyListing>
    Private Shared Property LoadedSpeechMaterialSpecifications As New SortedList(Of String, SpeechMaterialSpecification)

    'A shared function to load tests
    <ExludeFromPropertyListing>
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

    <ExludeFromPropertyListing>
    Public ReadOnly Property AvailableMediasets() As List(Of MediaSet)
        Get
            SpeechMaterial.ParentTestSpecification.LoadAvailableMediaSetSpecifications()
            Return SpeechMaterial.ParentTestSpecification.MediaSets
        End Get
    End Property

    <ExludeFromPropertyListing>
    Public ReadOnly Property AvailablePresets() As List(Of SmcPresets.Preset)
        Get
            Dim Output = New List(Of SmcPresets.Preset)
            For Each AvailablePreset In SpeechMaterial.Presets
                Output.Add(AvailablePreset)
            Next
            Return Output
        End Get
    End Property

    <ExludeFromPropertyListing>
    Public Property AvailableExperimentNumbers As Integer() = {}

    <ExludeFromPropertyListing>
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

    <ExludeFromPropertyListing>
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

    <ExludeFromPropertyListing>
    Public ReadOnly Property CurrentlySupportedIrSets As List(Of BinauralImpulseReponseSet)
        Get
            Dim Output As New List(Of BinauralImpulseReponseSet)

            If OstfBase.AllowDirectionalSimulation = True Then
                Dim SupportedIrNames As List(Of String)
                If MediaSet IsNot Nothing Then
                    SupportedIrNames = OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(MediaSet.WaveFileSampleRate)
                ElseIf MediaSets.Count > 0 Then
                    SupportedIrNames = OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(MediaSets(0).WaveFileSampleRate)
                Else
                    SupportedIrNames = OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(AvailableMediasets(0).WaveFileSampleRate)
                End If

                Dim AvaliableSets = OstfBase.DirectionalSimulator.GetAllDirectionalSimulationSets()
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
    <ExludeFromPropertyListing>
    Public ReadOnly Property CurrentlySupportedTransducers As List(Of AudioSystemSpecification)
        Get
            Dim Output = New List(Of AudioSystemSpecification)
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


    Private _TesterInstructionsButtonText As String = "Instruktioner för inställningar"
    <ExludeFromPropertyListing>
    Public Property TesterInstructionsButtonText As String
        Get
            Return _TesterInstructionsButtonText
        End Get
        Set(value As String)
            _TesterInstructionsButtonText = value
            OnPropertyChanged()
        End Set
    End Property


    Private _ParticipantInstructionsButtonText As String = "Patientinstruktioner"
    <ExludeFromPropertyListing>
    Public Property ParticipantInstructionsButtonText As String
        Get
            Return _ParticipantInstructionsButtonText
        End Get
        Set(value As String)
            _ParticipantInstructionsButtonText = value
            OnPropertyChanged()
        End Set
    End Property


    Private _IsPractiseTestTitle As String = "Övningstest"
    <ExludeFromPropertyListing>
    Public Property IsPractiseTestTitle As String
        Get
            Return _IsPractiseTestTitle
        End Get
        Set(value As String)
            _IsPractiseTestTitle = value
            OnPropertyChanged()
        End Set
    End Property



    Private _SelectedPresetTitle As String = "Förval"
    <ExludeFromPropertyListing>
    Public Property SelectedPresetTitle As String
        Get
            Return _SelectedPresetTitle
        End Get
        Set(value As String)
            _SelectedPresetTitle = value
            OnPropertyChanged()
        End Set
    End Property



    Private _ExperimentNumberTitle As String = "Experiment nr"

    <ExludeFromPropertyListing>
    Public Property ExperimentNumberTitle As String
        Get
            Return _ExperimentNumberTitle
        End Get
        Set(value As String)
            _ExperimentNumberTitle = value
            OnPropertyChanged()
        End Set
    End Property



    Private _StartListTitle As String = "StartLista"

    <ExludeFromPropertyListing>
    Public Property StartListTitle As String
        Get
            Return _StartListTitle
        End Get
        Set(value As String)
            _StartListTitle = value
            OnPropertyChanged()
        End Set
    End Property


    Private _SelectedMediaSetTitle As String = "Mediaset"

    <ExludeFromPropertyListing>
    Public Property SelectedMediaSetTitle As String
        Get
            Return _SelectedMediaSetTitle
        End Get
        Set(value As String)
            _SelectedMediaSetTitle = value
            OnPropertyChanged()
        End Set
    End Property



    Private _SelectedMediaSetsTitle As String = "Mediaset"
    <ExludeFromPropertyListing>
    Public Property SelectedMediaSetsTitle As String
        Get
            Return _SelectedMediaSetsTitle
        End Get
        Set(value As String)
            _SelectedMediaSetsTitle = value
            OnPropertyChanged()
        End Set
    End Property


    Private _ReferenceLevelTitle As String = ""
    <ExludeFromPropertyListing>
    Public Property ReferenceLevelTitle As String
        Get
            Return _ReferenceLevelTitle
        End Get
        Set(value As String)

            'Only updates if the new value differs
            If _ReferenceLevelTitle <> value Then
                _ReferenceLevelTitle = value
                OnPropertyChanged()
            End If

        End Set
    End Property



    Private _TargetLevelTitle As String = ""
    <ExludeFromPropertyListing>
    Public Property TargetLevelTitle As String
        Get
            Return _TargetLevelTitle
        End Get
        Set(value As String)
            'Only updates if the new value differs
            If _TargetLevelTitle <> value Then
                _TargetLevelTitle = value
                OnPropertyChanged()
            End If

        End Set
    End Property



    Private _MaskingLevelTitle As String = ""
    <ExludeFromPropertyListing>
    Public Property MaskingLevelTitle As String
        Get
            Return _MaskingLevelTitle
        End Get
        Set(value As String)

            'Only updates if the new value differs
            If _MaskingLevelTitle <> value Then
                _MaskingLevelTitle = value
                OnPropertyChanged()
            End If

        End Set
    End Property



    Private _BackgroundLevelTitle As String = ""
    <ExludeFromPropertyListing>
    Public Property BackgroundLevelTitle As String
        Get
            Return _BackgroundLevelTitle
        End Get
        Set(value As String)

            'Only updates if the new value differs
            If _BackgroundLevelTitle <> value Then
                _BackgroundLevelTitle = value
                OnPropertyChanged()
            End If

        End Set
    End Property



    Private _ContralateralMaskingLevelTitle As String = ""
    <ExludeFromPropertyListing>
    Public Property ContralateralMaskingLevelTitle As String
        Get
            Return _ContralateralMaskingLevelTitle
        End Get
        Set(value As String)

            'Only updates if the new value differs
            If _ContralateralMaskingLevelTitle <> value Then
                _ContralateralMaskingLevelTitle = value
                OnPropertyChanged()
            End If

        End Set
    End Property



    Private _TargetSNRTitle As String = "SNR (dB)"
    <ExludeFromPropertyListing>
    Public Overridable Property TargetSNRTitle As String
        Get
            Return _TargetSNRTitle
        End Get
        Set(value As String)
            _TargetSNRTitle = value
            OnPropertyChanged()
        End Set
    End Property



    Private _SelectedTestModeTitle As String = "Testläge"

    <ExludeFromPropertyListing>
    Public Property SelectedTestModeTitle As String
        Get
            Return _SelectedTestModeTitle
        End Get
        Set(value As String)
            _SelectedTestModeTitle = value
            OnPropertyChanged()
        End Set
    End Property



    Private _SelectedTestProtocolTitle As String = "Testprotokoll"
    <ExludeFromPropertyListing>
    Public Property SelectedTestProtocolTitle As String
        Get
            Return _SelectedTestProtocolTitle
        End Get
        Set(value As String)
            _SelectedTestProtocolTitle = value
            OnPropertyChanged()
        End Set
    End Property



    Private _KeyWordScoringTitle As String = "Rätta på nyckelord"

    <ExludeFromPropertyListing>
    Public Property KeyWordScoringTitle As String
        Get
            Return _KeyWordScoringTitle
        End Get
        Set(value As String)
            _KeyWordScoringTitle = value
            OnPropertyChanged()
        End Set
    End Property



    Private _ListOrderRandomizationTitle As String = "Slumpa listordning"
    <ExludeFromPropertyListing>
    Public Property ListOrderRandomizationTitle As String
        Get
            Return _ListOrderRandomizationTitle
        End Get
        Set(value As String)
            _ListOrderRandomizationTitle = value
            OnPropertyChanged()
        End Set
    End Property


    Private _WithinListRandomizationTitle As String = "Slumpa inom listor"

    <ExludeFromPropertyListing>
    Public Property WithinListRandomizationTitle As String
        Get
            Return _WithinListRandomizationTitle
        End Get
        Set(value As String)
            _WithinListRandomizationTitle = value
            OnPropertyChanged()
        End Set
    End Property



    Private _AcrossListsRandomizationTitle As String = "Slumpa mellan listor"

    <ExludeFromPropertyListing>
    Public Property AcrossListsRandomizationTitle As String
        Get
            Return _AcrossListsRandomizationTitle
        End Get
        Set(value As String)
            _AcrossListsRandomizationTitle = value
            OnPropertyChanged()
        End Set
    End Property


    'TODO: Change all remaining GUI titles so that they call OnPropertyChanged...

    <ExludeFromPropertyListing>
    Public Property IsFreeRecallTitle As String = "Fri rapportering"

    <ExludeFromPropertyListing>
    Public Property IncludeDidNotHearResponseAlternativeTitle As String = "Visa ? som alternativ"

    <ExludeFromPropertyListing>
    Public Property FixedResponseAlternativeCountTitle As String = "Antal svarsalternativ"

    <ExludeFromPropertyListing>
    Public Property TransducerTitle As String = "Ljudgivare"

    <ExludeFromPropertyListing>
    Public Property LevelsAreIn_dBHLTitle As String = "Ange nivåer i dB HL"

    <ExludeFromPropertyListing>
    Public Property SimulatedSoundFieldTitle As String = "Simulera ljudfält"

    <ExludeFromPropertyListing>
    Public Property IrSetTitle As String = "HRIR"

    <ExludeFromPropertyListing>
    Public Property TargetLocationsTitle As String = "Placering av talkälla/or"

    <ExludeFromPropertyListing>
    Public Property MaskerLocationsTitle As String = "Placering av maskeringsljud"

    <ExludeFromPropertyListing>
    Public Property BackgroundNonSpeechLocationsTitle As String = "Placering av bakgrundsljud"

    <ExludeFromPropertyListing>
    Public Property BackgroundSpeechLocationsTitle As String = "Placering av bakgrundstal"

    <ExludeFromPropertyListing>
    Public Property ContralateralMaskingTitle As String = "Kontralateral maskering"

    <ExludeFromPropertyListing>
    Public Property LockContralateralMaskingTitle As String = "Koppla till talnivå"

    <ExludeFromPropertyListing>
    Public Property PhaseAudiometryTitle As String = "Fasaudiometri"

    <ExludeFromPropertyListing>
    Public Property PhaseAudiometryTypeTitle As String = "Fasaudiometrityp"


    <ExludeFromPropertyListing>
    Public Property PreListenTitle As String = "Provlyssna"

    <ExludeFromPropertyListing>
    Public Property PreListenPlayButtonTitle As String = "Spela nästa"

    <ExludeFromPropertyListing>
    Public Property PreListenStopButtonTitle As String = "Stop"

    <ExludeFromPropertyListing>
    Public Property PreListenLouderButtonTitle As String = "Öka nivån"

    <ExludeFromPropertyListing>
    Public Property PreListenSofterButtonTitle As String = "Minska nivån"

    <ExludeFromPropertyListing>
    Public Property CalibrationCheckTitle As String = "Calibration check (play calibration signal mixed by the selected test)"

    <ExludeFromPropertyListing>
    Public Property CalibrationCheckPlayButtonTitle As String = "Play"

    <ExludeFromPropertyListing>
    Public Property CalibrationCheckStopButtonTitle As String = "Stop"

    Public Overridable Function GetTestCompletedGuiMessage() As String

        Select Case SharedSpeechTestObjects.GuiLanguage
            Case Utils.EnumCollection.Languages.Swedish
                Return "Testet är klart!"
            Case Else
                Return "The test is finished!"
        End Select

    End Function

#End Region


#Region "GuiSettings"

    ''' <summary>
    ''' Holds the minimum reference level step size
    ''' </summary>
    ''' <returns></returns>
    Public Property ReferenceLevel_StepSize As Double = 1

    ''' <summary>
    ''' Holds the minimum target level step size 
    ''' </summary>
    ''' <returns></returns>
    Public Property TargetLevel_StepSize As Double = 1

    ''' <summary>
    ''' Holds the minimum masker level step size 
    ''' </summary>
    ''' <returns></returns>
    Public Property MaskingLevel_StepSize As Double = 1

    ''' <summary>
    ''' Holds the minimum background sounds level step size 
    ''' </summary>
    ''' <returns></returns>
    Public Property BackgroundLevel_StepSize As Double = 1

    ''' <summary>
    ''' Holds the minimum contralateral masker level step size 
    ''' </summary>
    ''' <returns></returns>
    Public Property ContralateralMaskingLevel_StepSize As Double = 1

    ''' <summary>
    ''' Holds the minimum target SNR level step size
    ''' </summary>
    ''' <returns></returns>
    Public Property TargetSNR_StepSize As Double = 1


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

    Public Overridable ReadOnly Property ShowGuiChoice_TargetSNRLevel As Boolean = False

    <ExludeFromPropertyListing>
    Public Overridable ReadOnly Property ShowGuiChoice_TargetLevel As Boolean
        Get
            If CanHaveTargets() = True Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    <ExludeFromPropertyListing>
    Public Overridable ReadOnly Property ShowGuiChoice_MaskingLevel As Boolean
        Get
            If CanHaveMaskers() = True Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    <ExludeFromPropertyListing>
    Public Overridable ReadOnly Property ShowGuiChoice_BackgroundLevel As Boolean
        Get
            If CanHaveBackgroundNonSpeech() = True Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    <ExludeFromPropertyListing>
    Public Overridable ReadOnly Property ShowGuiChoice_ContralateralMaskingLevel As Boolean
        Get
            Return CanHaveContralateralMasking()
        End Get
    End Property

    <ExludeFromPropertyListing>
    Public Overridable ReadOnly Property ShowGuiChoice_ContralateralMasking As Boolean
        Get
            Return CanHaveContralateralMasking()
        End Get
    End Property

    <ExludeFromPropertyListing>
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

    <ExludeFromPropertyListing>
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

    <ExludeFromPropertyListing>
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

    <ExludeFromPropertyListing>
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


    <ExludeFromPropertyListing>
    Public Property ShowGuiChoice_KeyWordScoring As Boolean = False

    <ExludeFromPropertyListing>
    Public Property ShowGuiChoice_ListOrderRandomization As Boolean = False

    <ExludeFromPropertyListing>
    Public Property ShowGuiChoice_WithinListRandomization As Boolean = False

    <ExludeFromPropertyListing>
    Public Property ShowGuiChoice_AcrossListRandomization As Boolean = False

    <ExludeFromPropertyListing>
    Public Property ShowGuiChoice_FreeRecall As Boolean = False

    <ExludeFromPropertyListing>
    Public Property ShowGuiChoice_DidNotHearAlternative As Boolean = False

    <ExludeFromPropertyListing>
    Public Property ShowGuiChoice_PhaseAudiometry As Boolean = False


#End Region

#Region "GUI lock / unlock"

    Private _ListSelectionControlIsEnabled As Boolean = True
    <ExludeFromPropertyListing>
    Public Property ListSelectionControlIsEnabled() As Boolean
        Get
            Return _ListSelectionControlIsEnabled
        End Get
        Set(value As Boolean)
            _ListSelectionControlIsEnabled = value
            OnPropertyChanged()
        End Set
    End Property



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
    <ExludeFromPropertyListing>
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
    <ExludeFromPropertyListing>
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
    <ExludeFromPropertyListing>
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
    <ExludeFromPropertyListing>
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
    Private _Preset As SmcPresets.Preset = Nothing

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

    Private _MinimumLevel_TargetSNR As Double = -30
    Public Property MinimumLevel_TargetSNR As Double
        Get
            Return _MinimumLevel_TargetSNR
        End Get
        Set(value As Double)
            _MinimumLevel_TargetSNR = value
            OnPropertyChanged()
        End Set
    End Property

    Private _MaximumLevel_TargetSNR As Double = 30
    Public Property MaximumLevel_TargetSNR As Double
        Get
            Return _MaximumLevel_TargetSNR
        End Get
        Set(value As Double)
            _MaximumLevel_TargetSNR = value
            OnPropertyChanged()
        End Set
    End Property

    Public Property ReferenceLevel As Double
        Get
            Return Math.Clamp(_UnlimitedUnderlying_ReferenceLevel, MinimumReferenceLevel, MaximumReferenceLevel)
        End Get
        Set(value As Double)
            _UnlimitedUnderlying_ReferenceLevel = Math.Round(value / ReferenceLevel_StepSize) * ReferenceLevel_StepSize
            OnPropertyChanged()
        End Set
    End Property
    Private _UnlimitedUnderlying_ReferenceLevel As Double = 68.34


    Public Property TargetLevel As Double
        Get
            Return Math.Clamp(_UnlimitedUnderlying_TargetLevel, MinimumLevel_Targets, MaximumLevel_Targets)
        End Get
        Set(value As Double)

            'Dim NewValue As Double = value
            Dim OldValue As Double = _UnlimitedUnderlying_TargetLevel

            _UnlimitedUnderlying_TargetLevel = Math.Round(value / TargetLevel_StepSize) * TargetLevel_StepSize

            Dim NewValue As Double = _UnlimitedUnderlying_TargetLevel

            If LockContralateralMaskingLevelToSpeechLevel = True Then

                'Adjusting the contralateral masking by the New level difference
                Dim differenceValue As Double = NewValue - OldValue
                _UnlimitedUnderlying_ContralateralMaskingLevel += differenceValue

                'Trigger OnPropertyChanged() for ContralateralMasker
                OnPropertyChanged("ContralateralMaskingLevel")

            End If

            OnPropertyChanged()
        End Set
    End Property
    Private _UnlimitedUnderlying_TargetLevel As Double = 0

    'Returns the TargetLevel - MaskingLevel signal-to-noise ratio
    Public ReadOnly Property CurrentSNR As Double
        Get
            Return TargetLevel - MaskingLevel
        End Get
    End Property

    Public Property MaskingLevel As Double
        Get
            Return Math.Clamp(_UnlimitedUnderlying_MaskingLevel, MinimumLevel_Maskers, MaximumLevel_Maskers)
        End Get
        Set(value As Double)
            _UnlimitedUnderlying_MaskingLevel = Math.Round(value / MaskingLevel_StepSize) * MaskingLevel_StepSize
            OnPropertyChanged()
        End Set
    End Property
    Private _UnlimitedUnderlying_MaskingLevel As Double = 0


    Public Property BackgroundLevel As Double
        Get
            Return Math.Clamp(_UnlimitedUnderlying_BackgroundLevel, MinimumLevel_Background, MaximumLevel_Background)
        End Get
        Set(value As Double)
            _UnlimitedUnderlying_BackgroundLevel = Math.Round(value / BackgroundLevel_StepSize) * BackgroundLevel_StepSize
            OnPropertyChanged()
        End Set
    End Property
    Private _UnlimitedUnderlying_BackgroundLevel As Double = 0


    Public Property ContralateralMaskingLevel As Double
        Get
            Return Math.Clamp(_UnlimitedUnderlying_ContralateralMaskingLevel, MinimumLevel_ContralateralMaskers, MaximumLevel_ContralateralMaskers)
        End Get
        Set(value As Double)
            _UnlimitedUnderlying_ContralateralMaskingLevel = Math.Round(value / ContralateralMaskingLevel_StepSize) * ContralateralMaskingLevel_StepSize
            OnPropertyChanged()
        End Set
    End Property
    Private _UnlimitedUnderlying_ContralateralMaskingLevel As Double = 0


    Private _LockContralateralMaskingLevelToSpeechLevel As Boolean = False
    Public Property LockContralateralMaskingLevelToSpeechLevel As Boolean
        Get
            Return _LockContralateralMaskingLevelToSpeechLevel
        End Get
        Set(value As Boolean)
            _LockContralateralMaskingLevelToSpeechLevel = value
            OnPropertyChanged()
        End Set
    End Property

    ''' <summary>
    ''' The desired SNR value to use in a test. The value can be set from the GUI, or internally in the test. This value should never directly adjust target and masker levels, be instead these levels should be set internally in each test.
    ''' </summary>
    ''' <returns></returns>
    Public Property TargetSNR As Double
        Get
            Return Math.Clamp(_UnlimitedUnderlying_TargetSNR, MinimumLevel_TargetSNR, MaximumLevel_TargetSNR)
        End Get
        Set(value As Double)
            _UnlimitedUnderlying_TargetSNR = Math.Round(value / TargetSNR_StepSize) * TargetSNR_StepSize
            OnPropertyChanged()
        End Set
    End Property
    Private _UnlimitedUnderlying_TargetSNR As Double = 0


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


    ''' <summary>
    ''' An event that should be listened to by the object that holds the Transducer acctually used for the playback.
    ''' </summary>
    Public Event TransducerChanged As EventHandler

    Public Property Transducer As AudioSystemSpecification
        Get
            Return _Transducer
        End Get
        Set(value As AudioSystemSpecification)
            _Transducer = value

            'Inactivates the use of simulated sound field is the transducer is not headphones
            If _Transducer.IsHeadphones() = False Then SimulatedSoundField = False

            'Inactivates contralateral masking if not supported
            If Transducer.IsHeadphones = False Then ContralateralMasking = False
            If Transducer.IsHeadphones = True And CurrentSpeechTest.SimulatedSoundField = True Then ContralateralMasking = False
            If Transducer.IsHeadphones = True And CurrentSpeechTest.SimulatedSoundField = False And PhaseAudiometry = True Then ContralateralMasking = False

            OnPropertyChanged()

            RaiseEvent TransducerChanged(Me, New EventArgs)

            'Updating titles
            UpdateGUI_dB_Titles()

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

            'Only updates if the new value differs
            If _LevelsAreIn_dBHL <> value Then

                _LevelsAreIn_dBHL = value

                If value = True And SimulatedSoundField = True Then
                    'Inactivates sound field simulation if dB HL values should be used
                    SimulatedSoundField = False
                End If

                'Updating titles
                UpdateGUI_dB_Titles()

                OnPropertyChanged()

            End If

        End Set
    End Property
    Private _LevelsAreIn_dBHL As Boolean = False

    Private Sub UpdateGUI_dB_Titles()

        'Updating titles
        Select Case GuiLanguage
            Case Utils.EnumCollection.Languages.Swedish
                ReferenceLevelTitle = "Referensnivå (" & dBString() & ")"
                TargetLevelTitle = "Talnivå (" & dBString() & ")"
                MaskingLevelTitle = "Maskeringsnivå (" & dBString() & ")"
                BackgroundLevelTitle = "Bakgrundsnivå (" & dBString() & ")"
                ContralateralMaskingLevelTitle = "Kontralat. maskeringsnivå (" & dBString() & ")"
            Case Else
                ReferenceLevelTitle = "Reference level (" & dBString() & ")"
                TargetLevelTitle = "Speech level (" & dBString() & ")"
                MaskingLevelTitle = "Masking level (" & dBString() & ")"
                BackgroundLevelTitle = "Background level (" & dBString() & ")"
                ContralateralMaskingLevelTitle = "Contralat. masking level (" & dBString() & ")"
        End Select

    End Sub

    ''' <summary>
    ''' Returns the strings dB HL or dB SPL depending on the value of the property LevelsAreIn_dBHL
    ''' </summary>
    ''' <returns></returns>
    Public Function dBString() As String
        If _LevelsAreIn_dBHL = False Then
            Return "dB SPL"
        Else
            Return "dB HL"
        End If
    End Function

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
    Public ReadOnly Property PresentationMode As Utils.SoundPropagationTypes
        Get
            If SimulatedSoundField = False Then
                Return Utils.SoundPropagationTypes.PointSpeakers
            Else
                Return Utils.SoundPropagationTypes.SimulatedSoundField
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
    ''' Selecting the same SoundSourceLocations in MaskerLocations as selected in TargetLocations
    ''' </summary>
    Protected Sub SelectSameMaskersAsTargetSoundSources()
        For Each TagetLocationCandidate In _SignalLocationCandidates
            For Each MaskerLocationCandidate In _MaskerLocationCandidates
                If TagetLocationCandidate.IsSameLocation(MaskerLocationCandidate) Then
                    MaskerLocationCandidate.Selected = TagetLocationCandidate.Selected
                End If
            Next
        Next
        OnPropertyChanged()
    End Sub


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

    ''' <summary>
    ''' Determines based on the selected MediaSet, Transducer Soundfield simulation and phase audiometry settings if vaontalateral masking can be used
    ''' </summary>
    ''' <returns></returns>
    Public Function CanHaveContralateralMasking() As Boolean
        If SpeechMaterial IsNot Nothing Then
            If MediaSet IsNot Nothing Then
                If MediaSet.ContralateralMaskerAudioItems > 0 Then
                    If Transducer IsNot Nothing Then
                        If Transducer.IsHeadphones = True Then
                            If CurrentSpeechTest.SimulatedSoundField = False Then
                                If CurrentSpeechTest.PhaseAudiometry = False Then
                                    Return True
                                End If
                            End If
                        End If
                    End If
                End If
            Else
                If AvailableMediasets.Count > 0 Then
                    If AvailableMediasets(0).ContralateralMaskerAudioItems > 0 Then
                        If Transducer IsNot Nothing Then
                            If Transducer.IsHeadphones = True Then
                                If CurrentSpeechTest.SimulatedSoundField = False Then
                                    If CurrentSpeechTest.PhaseAudiometry = False Then
                                        Return True
                                    End If
                                End If
                            End If
                        End If
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
                                  ByVal TargetPresentationTime As Double,
                                  Optional ByVal MaskerPresentationTime As Double = 0,
                                  Optional ByVal BackgroundNonSpeechPresentationTime As Double = 0,
                                  Optional ByVal BackgroundSpeechPresentationTime As Double = 0,
                                  Optional ByVal ContralateralMaskerPresentationTime As Double = 0,
                                  Optional ByRef FadeSpecs_Target As List(Of DSP.FadeSpecifications) = Nothing,
                                  Optional ByRef FadeSpecs_Masker As List(Of DSP.FadeSpecifications) = Nothing,
                                  Optional ByRef FadeSpecs_BackgroundNonSpeech As List(Of DSP.FadeSpecifications) = Nothing,
                                  Optional ByRef FadeSpecs_BackgroundSpeech As List(Of DSP.FadeSpecifications) = Nothing,
                                  Optional ByRef FadeSpecs_ContralateralMasker As List(Of DSP.FadeSpecifications) = Nothing,
                                            Optional ExportSounds As Boolean = False)

        'TODO: This function is not finished, it still need implementation of BackgroundNonSpeech and BackgroundSpeech


        'Mix the signal using DuxplexMixer CreateSoundScene
        'Sets a List of SoundSceneItem in which to put the sounds to mix
        Dim ItemList = New List(Of Audio.SoundScene.SoundSceneItem)
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
                    CurrentTestEar = Utils.EnumCollection.SidesWithBoth.Left
                ElseIf HasLeftSideTarget = False And HasRightSideTarget = True Then
                    CurrentTestEar = Utils.EnumCollection.SidesWithBoth.Right
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
                FadeSpecs_Target = New List(Of DSP.FadeSpecifications)
                FadeSpecs_Target.Add(New DSP.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.002))
                FadeSpecs_Target.Add(New DSP.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.002))
            End If

            'Combining targets with the selected SignalLocations
            Dim Targets As New List(Of Tuple(Of Audio.Sound, Audio.SoundScene.SoundSourceLocation))
            For Each SignalLocation In SignalLocations
                'Re-using the same target in all selected locations
                Targets.Add(New Tuple(Of Audio.Sound, Audio.SoundScene.SoundSourceLocation)(TargetSound, SignalLocation))
            Next

            'Adding the targets sources to the ItemList
            For Index = 0 To Targets.Count - 1
                ItemList.Add(New Audio.SoundScene.SoundSceneItem(Targets(Index).Item1, 1, TargetLevel, LevelGroup, Targets(Index).Item2, Audio.SoundScene.SoundSceneItem.SoundSceneItemRoles.Target, TargetStartSample, TargetStartMeasureSample, TargetMeasureLength,, FadeSpecs_Target))
            Next

            'Incrementing LevelGroup 
            LevelGroup += 1

            'Storing data in the CurrentTestTrial
            CurrentTestTrial.LinguisticSoundStimulusStartTime = TargetPresentationTime
            CurrentTestTrial.LinguisticSoundStimulusDuration = TargetSound.WaveData.SampleData(1).Length / TargetSound.WaveFormat.SampleRate

        End If


        ' **MASKER SOUNDS**
        If MaskerLocations.Count > 0 Then

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
                FadeSpecs_Masker = New List(Of DSP.FadeSpecifications)
                FadeSpecs_Masker.Add(New DSP.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.002))
                FadeSpecs_Masker.Add(New DSP.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.002))
            End If

            'Combining maskers with the selected SignalLocations
            Dim Maskers As New List(Of Tuple(Of Audio.Sound, Audio.SoundScene.SoundSourceLocation))

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
                Dim CurrentSourceMaskerSound = DSP.CopySection(MaskerSound, StartCopySample, IndendedMaskerLength, 1)

                'Copying the SMA object to retain the nominal level (although other time data and other related stuff will be incorrect, if not adjusted for)
                CurrentSourceMaskerSound.SMA = MaskerSound.SMA.CreateCopy(CurrentSourceMaskerSound)

                'Picking the masker sound
                Maskers.Add(New Tuple(Of Audio.Sound, Audio.SoundScene.SoundSourceLocation)(CurrentSourceMaskerSound, MaskerLocations(Index)))

            Next

            'Adding the maskers sources to the ItemList
            For Index = 0 To Maskers.Count - 1
                ItemList.Add(New Audio.SoundScene.SoundSceneItem(Maskers(Index).Item1, 1, MaskingLevel, LevelGroup, Maskers(Index).Item2, Audio.SoundScene.SoundSceneItem.SoundSceneItemRoles.Masker, MaskerStartSample, MaskerStartMeasureSample, MaskerMeasureLength,, FadeSpecs_Masker))
            Next

            'Incrementing LevelGroup 
            LevelGroup += 1

        End If

        'TODO: implement BackgroundNonSpeech and BackgroundSpeech here

        ' **CONTRALATERAL MASKER**
        If ContralateralMasking = True Then

            'Calculates the EM corrected Contralateral masker level (the level supplied should not be EM corrected but be as it would appear on an audiometer attenuator
            Dim ContralateralMaskerLevel_EmCorrected As Double = ContralateralMaskingLevel + MediaSet.EffectiveContralateralMaskingGain

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
            Dim ContralateralMaskerSound = DSP.CopySection(FullContralateralMaskerSound, RandomStartReadIndex, IndendedMaskerLength, 1)

            'Copying the SMA object to retain the nominal level (although other time data and other related stuff will be incorrect, if not adjusted for)
            ContralateralMaskerSound.SMA = FullContralateralMaskerSound.SMA.CreateCopy(ContralateralMaskerSound)

            'Getting the ContralateralMaskerMeasureLength from the length of the sound files (i.e. everything is measured)
            Dim ContralateralMaskerMeasureLength As Integer = ContralateralMaskerSound.WaveData.SampleData(1).Length

            'Sets up fading specifications for the contralateral masker
            If FadeSpecs_ContralateralMasker Is Nothing Then
                FadeSpecs_ContralateralMasker = New List(Of DSP.FadeSpecifications)
                FadeSpecs_ContralateralMasker.Add(New DSP.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.002))
                FadeSpecs_ContralateralMasker.Add(New DSP.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.002))
            End If

            'Determining which side to put the contralateral masker
            Dim ContralateralMaskerLocation As New Audio.SoundScene.SoundSourceLocation With {.Distance = 0, .Elevation = 0}
            If CurrentTestEar = Utils.EnumCollection.SidesWithBoth.Left Then
                'Putting contralateral masker in right ear
                ContralateralMaskerLocation.HorizontalAzimuth = 90
            ElseIf CurrentTestEar = Utils.EnumCollection.SidesWithBoth.Right Then
                'Putting contralateral masker in left ear
                ContralateralMaskerLocation.HorizontalAzimuth = -90
            Else
                'This shold never happen...
                Throw New Exception("Contralateral noise cannot be used when the target signal is on both sides!")
            End If

            'Adding the contralateral maskers sources to the ItemList
            ItemList.Add(New Audio.SoundScene.SoundSceneItem(ContralateralMaskerSound, 1, ContralateralMaskerLevel_EmCorrected, LevelGroup, ContralateralMaskerLocation, Audio.SoundScene.SoundSceneItem.SoundSceneItemRoles.ContralateralMasker, ContralateralMaskerStartSample, ContralateralMaskerStartMeasureSample, ContralateralMaskerMeasureLength,, FadeSpecs_ContralateralMasker))

            'Incrementing LevelGroup 
            LevelGroup += 1

        End If


        Dim CurrentSoundPropagationType As Utils.SoundPropagationTypes = Utils.SoundPropagationTypes.PointSpeakers
        If SimulatedSoundField Then
            CurrentSoundPropagationType = Utils.SoundPropagationTypes.SimulatedSoundField
            'TODO: This needs to be modified if/when more SoundPropagationTypes are starting to be supported
        End If

        'Creating the mix by calling CreateSoundScene of the current Mixer
        CurrentTestTrial.Sound = Transducer.Mixer.CreateSoundScene(ItemList, UseNominalLevels, LevelsAreIn_dBHL, CurrentSoundPropagationType, Transducer.LimiterThreshold, ExportSounds, CurrentTestTrial.Spelling)

        'TODO: Reasonably this method should only store values into the CurrentTestTrial that are derived within this function! Leaving these for now
        CurrentTestTrial.MediaSetName = MediaSet.MediaSetName
        CurrentTestTrial.EfficientContralateralMaskingTerm = MediaSet.EffectiveContralateralMaskingGain

    End Sub

    ''' <summary>
    ''' This method should return a calibration sound mixed to the currently set level (Target level in most tests, ReferenceLevel in SiP-tests), presented from the first indicated target sound source.
    ''' This sound should be returned in Item1. Item2 should contain a description of the calibration signal to display in the GUI.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function CreateCalibrationCheckSignal() As Tuple(Of Audio.Sound, String)

    Public Enum CalibrationCheckLevelTypes
        TargetLevel
        ReferenceLevel
    End Enum

    ''' <summary>
    ''' This sub uses similar code as MixStandardTestTrialSound to mix the calibration sound for the current speech material and media set, in the same way as a target sound in MixStandardTestTrialSound, to the sound level (in dB HL or SPL as determined by the value selected in the current SpeechTest) of the targets in the location of the first specified target.
    ''' </summary>
    ''' <param name="UseNominalLevels">If True, applied gains are based on the nominal levels stored in the SMA object of each sound. If False, sound levels are re-calculated.</param>
    ''' <param name="ExportSounds">Can be used to debug or analyse presented sounds. Default value is False. Sounds are stored into the current log folder.</param>
    Protected Function MixStandardCalibrationSound(ByVal UseNominalLevels As Boolean, ByVal LevelType As CalibrationCheckLevelTypes, Optional ExportSounds As Boolean = False) As Tuple(Of Audio.Sound, String)

        'Note that this sub should mix the calibration sound to the same level and location as single target in MixStandardTestTrialSound. It can be used to check that the calibration value and attenuator setting is correct, to ensure correct output levels in the transducer.

        'Mix the signal using DuxplexMixer CreateSoundScene
        'Sets a List of SoundSceneItem in which to put the sounds to mix
        Dim ItemList = New List(Of SoundSceneItem)
        Dim LevelGroup As Integer = 1 ' N.B. There is only one level group here, since there is only one calibration signal. The level group value is used to set the added sound level of items sharing the same (arbitrary) LevelGroup value to the indicated sound level. (Thus, the sounds with the same LevelGroup value are measured together.)
        Dim CurrentSampleRate As Integer = -1

        Dim Description As String = ""

        ' **TARGET SOUNDS**
        If SignalLocations.Count > 0 Then

            'Getting the target sound (i.e. test words)
            Dim TargetSound = SpeechMaterial.GetCalibrationSignalSound(MediaSet, 0) ' Here we get the calibration signal instead of a test word. We use Index = 0. But this could well be updated to allow selection of specific calibration files.
            'Dim TargetSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(MediaSet, 0, 1, , , , , False, False, False, , , False)

            Description &= "File " & "'" & System.IO.Path.GetFileName(TargetSound.SourcePath) & "'"

            If TargetSound Is Nothing Then
                'Returns Nothing if there is no calibration signal
                Return Nothing
            End If

            'Storing the samplerate
            CurrentSampleRate = TargetSound.WaveFormat.SampleRate

            'Setting the insertion sample of the target
            Dim TargetStartSample As Integer = CurrentSampleRate * 1

            'Setting the TargetStartMeasureSample (i.e. the sample index in the TargetSound, not in the final mix)
            Dim TargetStartMeasureSample As Integer = 0

            'Getting the TargetMeasureLength from the length of the sound files (i.e. everything is measured)
            Dim TargetMeasureLength As Integer = TargetSound.WaveData.SampleData(1).Length - TargetStartSample

            'Sets up default fading specifications for the target
            Dim FadeSpecs_Target = New List(Of DSP.FadeSpecifications)
            FadeSpecs_Target.Add(New DSP.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.5))
            FadeSpecs_Target.Add(New DSP.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.5))

            'Combining targets with the first selected SignalLocation
            Dim Targets As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation))
            Targets.Add(New Tuple(Of Audio.Sound, SoundSourceLocation)(TargetSound, SignalLocations(0)))

            'Adding the targets sources to the ItemList
            For Index = 0 To Targets.Count - 1

                Select Case LevelType
                    Case CalibrationCheckLevelTypes.TargetLevel
                        ItemList.Add(New SoundSceneItem(Targets(Index).Item1, 1, TargetLevel, LevelGroup, Targets(Index).Item2, SoundSceneItem.SoundSceneItemRoles.Target, TargetStartSample, TargetStartMeasureSample, TargetMeasureLength,, FadeSpecs_Target))

                        Description &= " played at a speech level of " & TargetLevel

                    Case CalibrationCheckLevelTypes.ReferenceLevel
                        ItemList.Add(New SoundSceneItem(Targets(Index).Item1, 1, ReferenceLevel, LevelGroup, Targets(Index).Item2, SoundSceneItem.SoundSceneItemRoles.Target, TargetStartSample, TargetStartMeasureSample, TargetMeasureLength,, FadeSpecs_Target))

                        Description &= " played at a reference level of " & ReferenceLevel

                    Case Else
                        Throw New ArgumentException("Unknown LevelType")
                End Select

            Next

        Else

            'Returns Nothing if there is no target locations
            Return Nothing
        End If

        Dim CurrentSoundPropagationType As Utils.SoundPropagationTypes = Utils.SoundPropagationTypes.PointSpeakers
        If SimulatedSoundField Then
            CurrentSoundPropagationType = Utils.SoundPropagationTypes.SimulatedSoundField
            'TODO: This needs to be modified if/when more SoundPropagationTypes are starting to be supported
        End If

        'Creating the mix by calling CreateSoundScene of the current Mixer
        Dim OutputSound = Transducer.Mixer.CreateSoundScene(ItemList, UseNominalLevels, LevelsAreIn_dBHL, CurrentSoundPropagationType, Transducer.LimiterThreshold, ExportSounds, "Calibration")

        If LevelsAreIn_dBHL = True Then
            Description &= " dB HL"
        Else
            Description &= " dB SPL"
        End If

        Return New Tuple(Of Audio.Sound, String)(OutputSound, Description)

    End Function






#End Region



#Region "Test protocol"

    Private _TesterInstructions As String = ""

    Public Property TesterInstructions As String
        Get
            Return _TesterInstructions
        End Get
        Set(value As String)
            _TesterInstructions = value
            OnPropertyChanged()
        End Set
    End Property

    Private _ParticipantInstructions As String = ""

    Public Property ParticipantInstructions As String
        Get
            Return _ParticipantInstructions
        End Get
        Set(value As String)
            _ParticipantInstructions = value
            OnPropertyChanged()
        End Set
    End Property


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


    <ExludeFromPropertyListing>
    Public Property SupportsPrelistening As Boolean = False


    <ExludeFromPropertyListing>
    Public Property AvailableTestModes As List(Of TestModes) = New List(Of TestModes)

    Public Enum TestModes
        ConstantStimuli
        AdaptiveSpeech
        AdaptiveNoise
        AdaptiveDirectionality
        Custom
    End Enum

    <ExludeFromPropertyListing>
    Public Property AvailableTestProtocols As List(Of TestProtocol) = New List(Of TestProtocol)

    <ExludeFromPropertyListing>
    Public Property AllTestProtocols As List(Of TestProtocol) = New List(Of TestProtocol) From {
                New SrtSwedishHint2018_TestProtocol,
                New BrandKollmeier2002_TestProtocol,
                New FixedLengthWordsInNoise_WithPreTestLevelAdjustment_TestProtocol,
                New HagermanKinnefors1995_TestProtocol,
                New SrtIso8253_TestProtocol,
                New SrtSwedishHint2018_TestProtocol}

    'TODO: To be moved to STFN extension?
    'New SrtAsha1979_TestProtocol,
    'New SrtChaiklinFontDixon1967_TestProtocol,
    'New SrtChaiklinVentry1964_TestProtocol,


    <ExludeFromPropertyListing>
    Public Property AvailableFixedResponseAlternativeCounts As List(Of Integer) = New List(Of Integer)

    <ExludeFromPropertyListing>
    Public Property AvailablePhaseAudiometryTypes As List(Of BmldModes) = New List(Of BmldModes)

    ''' <summary>
    ''' Returns a string that descripbes the data returned by finalTestProtocolResultValue. If TestProtocol object is not used, the string TestProtocolNotUsed is returned.
    ''' This function is utilized to export the type of the final test protocol value, by storing it in the TestTrial.SpeechTestPropertyDump
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property TestProtocolResultType As String
        Get
            If TestProtocol IsNot Nothing Then

                Return TestProtocol.GetFinalResultType
            Else
                Return "TestProtocolNotUsed"
            End If
        End Get
    End Property

    ''' <summary>
    ''' Returns the final result of the current test protocol, if established. If not established, or if a TestProtocol object is not used, Double.NaN is returned.
    ''' This function is utilized to export the final test protocol value, by storing it in the TestTrial.SpeechTestPropertyDump
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property TestProtocolResultValue As Double
        Get
            If TestProtocol IsNot Nothing Then
                Dim FinalResultValue = TestProtocol.GetFinalResultValue

                If FinalResultValue IsNot Nothing Then
                    Return FinalResultValue
                Else
                    Return Double.NaN
                End If
            Else
                Return Double.NaN
            End If
        End Get
    End Property


#End Region

#Region "RunningTest"

    Public CurrentTestTrial As TestTrial

    ''' <summary>
    ''' This feid can be used to store information that should be shown on screen during pause. 
    ''' </summary>
    Public PauseInformation As String = ""

    Public AbortInformation As String = ""

    Public Property HistoricTrialCount As Integer = 0

    ''' <summary>
    ''' Should deliver the current progress of the test, or Nothing if progress indication is not supported.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function GetProgress() As ProgressInfo


#End Region

#Region "TestResults"

    Public Enum GuiResultTypes
        StringResults
        VisualResults
    End Enum

    Public GuiResultType As GuiResultTypes = GuiResultTypes.StringResults

    ''' <summary>
    ''' Should return the average proportion correct in the observed test trials, of Nothing if no test trials have been presented
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Function GetAverageScore(Optional IncludeTrialsFromEnd As Integer? = Nothing) As Double?

        'Getting all trials
        Dim ObservedTrials = GetObservedTestTrials().ToList

        'Returning Nothing if no trials have been observed
        If ObservedTrials.Count = 0 Then Return Nothing

        'Creating a list to store observed trials to include
        Dim TrialsToInclude As New List(Of TestTrial)

        If IncludeTrialsFromEnd.HasValue Then

            'Getting the IncludeTrialsFromEnd last test trials, or all if ObservedTrials is shorter than IncludeTrialsFromEnd
            If ObservedTrials.Count < IncludeTrialsFromEnd + 1 Then
                TrialsToInclude.AddRange(ObservedTrials)
            Else
                TrialsToInclude.AddRange(ObservedTrials.GetRange(ObservedTrials.Count - IncludeTrialsFromEnd, IncludeTrialsFromEnd))
            End If

        Else
            'Getting all trials
            TrialsToInclude.AddRange(ObservedTrials)
        End If

        'Calculating average score
        Dim ScoreList As New List(Of Integer)
        For Each Trial In TrialsToInclude
            ScoreList.AddRange(Trial.ScoreList)
        Next

        Return ScoreList.Average

    End Function

    ''' <summary>
    ''' Should return the total number of (observed + planned) trials in a test, or -1 if not possible to determine
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function GetTotalTrialCount() As Integer

    ''' <summary>
    ''' Should return a list of sub-test results to display in a test-result GUI. Item1s are the group title, and Item2s are the corresponding scores. Should return Nothing if sub-group scores are not used.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function GetSubGroupResults() As List(Of Tuple(Of String, Double))

    ''' <summary>
    ''' This method should return data suitable for presenting in a x/y plot. Item1 gives the X-axis label indicating type of level. 
    ''' In Item2, keys are presentation levels or SNRs, and values are scores as proportion (0-1).
    ''' Should be Nothing if not used by the current test.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function GetScorePerLevel() As Tuple(Of String, SortedList(Of Double, Double))


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

    Public MustOverride Sub FinalizeTestAheadOfTime()

    Public MustOverride Function GetResultStringForGui() As String


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
        Logging.SendInfoToLog(TestTrialResultsString, OutputFilename, OutputPath, False, True, False, True, True)

        Dim SelectedVariables = GetSelectedExportVariables()
        If SelectedVariables IsNot Nothing Then

            Dim OutputFilename_SelectedVariables = Me.FilePathRepresentation & "_TrialResults_SelectedVariables_" & SharedSpeechTestObjects.CurrentParticipantID

            Dim TestTrialResultsString_SelectedVariables = GetTestTrialResultExportString(SelectedVariables)
            Logging.SendInfoToLog(TestTrialResultsString_SelectedVariables, OutputFilename_SelectedVariables, OutputPath, False, True, False, True, True)

        End If

        Return True

    End Function

    Public Function GetTestResultScreenDumpExportPath() As String

        Dim LanguageBit As String
        Select Case GuiLanguage
            Case Utils.EnumCollection.Languages.Swedish
                LanguageBit = "Sparade skärmdumpar"
            Case Else
                LanguageBit = "SavedScreenShot"
        End Select

        Dim OutputFolder = IO.Path.Combine(SharedSpeechTestObjects.TestResultsRootFolder, LanguageBit, Me.FilePathRepresentation)
        Dim OutputFilename = Me.FilePathRepresentation & "_" & SharedSpeechTestObjects.CurrentParticipantID & "_" & Logging.CreateDateTimeStringForFileNames & ".png"
        Dim OutputPath = System.IO.Path.Combine(OutputFolder, OutputFilename)
        Return OutputPath

    End Function

    'Public Function SaveTableFormatedTestResults() As Boolean

    '    'Skipping saving data if it's the demo ptc ID
    '    If SharedSpeechTestObjects.CurrentParticipantID.Trim = SharedSpeechTestObjects.NoTestId Then Return True

    '    If SharedSpeechTestObjects.TestResultsRootFolder = "" Then
    '        Messager.MsgBox("Unable to save the results to file due to missing test results output folder. This should have been selected first startup of the app!")
    '        Return False
    '    End If

    '    If IO.Directory.Exists(SharedSpeechTestObjects.TestResultsRootFolder) = False Then
    '        Try
    '            IO.Directory.CreateDirectory(SharedSpeechTestObjects.TestResultsRootFolder)
    '        Catch ex As Exception
    '            Messager.MsgBox("Unable to save the results to the test results output folder (" & SharedSpeechTestObjects.TestResultsRootFolder & "). The path does not exist, and could not be created!")
    '        End Try
    '        Return False
    '    End If

    '    Dim OutputPath = IO.Path.Combine(SharedSpeechTestObjects.TestResultsRootFolder, Me.FilePathRepresentation)
    '    Dim OutputFilename = Me.FilePathRepresentation & "_Results_" & SharedSpeechTestObjects.CurrentParticipantID

    '    Dim TestResultsString = GetTestResultsExportString()
    '    Utils.SendInfoToLog(TestResultsString, OutputFilename, OutputPath, False, True, False, False, True)

    '    Dim SelectedVariables = GetSelectedExportVariables()
    '    If SelectedVariables IsNot Nothing Then

    '        Dim OutputFilename_SelectedVariables = Me.FilePathRepresentation & "_Results_SelectedVariables_" & SharedSpeechTestObjects.CurrentParticipantID

    '        Dim TestResultsString_SelectedVariables = GetTestResultsExportString(SelectedVariables)
    '        Utils.SendInfoToLog(TestResultsString_SelectedVariables, OutputFilename_SelectedVariables, OutputPath, False, True, False, False, True)

    '    End If

    '    Return True
    'End Function

    Public MustOverride Function GetObservedTestTrials() As IEnumerable(Of TestTrial)

    'Public Function GetTestResultsExportString(Optional ByVal SelectedVariables As List(Of String) = Nothing) As String

    '    Dim ExportStringList As New List(Of String)

    '    Dim LocalObservedTrials = GetObservedTestTrials()

    '    For i = 0 To LocalObservedTrials.Count - 1
    '        If i = 0 Then
    '            ExportStringList.Add("TrialIndex" & vbTab & LocalObservedTrials(i).TestResultColumnHeadings & vbTab & LocalObservedTrials.Last.ListedSpeechTestPropertyNames(SelectedVariables))
    '        End If
    '        ExportStringList.Add(i & vbTab & LocalObservedTrials(i).TestResultAsTextRow & vbTab & LocalObservedTrials.Last.ListedSpeechTestPropertyValues(SelectedVariables))
    '    Next

    '    Return String.Join(vbCrLf, ExportStringList)

    'End Function


    Public Function GetTestTrialResultExportString(Optional ByVal SelectedVariables As List(Of String) = Nothing) As String

        Dim LocalObservedTrials = GetObservedTestTrials()

        If LocalObservedTrials.Count = 0 Then Return ""

        Dim ExportStringList As New List(Of String)

        'Exporting only the current trial (last added to ObservedTrials)
        Dim TestTrialIndex As Integer = LocalObservedTrials.Count - 1

        If TestTrialIndex = 0 Then

            'Adding column headings on the first row
            Dim DataSetHeadingsColumnList As New List(Of String)
            DataSetHeadingsColumnList.Add("TrialIndex")
            DataSetHeadingsColumnList.Add(LocalObservedTrials(TestTrialIndex).TestResultColumnHeadings)
            For st = 0 To LocalObservedTrials(TestTrialIndex).SubTrials.Count - 1
                DataSetHeadingsColumnList.Add("SubTrial_" & st + 1 & "_" & LocalObservedTrials(TestTrialIndex).SubTrials(st).ListedSpeechTestPropertyNames(SelectedVariables))
            Next
            DataSetHeadingsColumnList.Add(LocalObservedTrials(TestTrialIndex).ListedSpeechTestPropertyNames(SelectedVariables))

            'ExportStringList.Add("TrialIndex" & vbTab & LocalObservedTrials(TestTrialIndex).TestResultColumnHeadings & vbTab & LocalObservedTrials(TestTrialIndex).ListedSpeechTestPropertyNames(SelectedVariables))
            ExportStringList.Add(String.Join(vbTab, DataSetHeadingsColumnList))

        End If

        'Adding trial data 
        Dim DataSetColumnList As New List(Of String)
        DataSetColumnList.Add(TestTrialIndex)
        DataSetColumnList.Add(LocalObservedTrials(TestTrialIndex).TestResultAsTextRow)
        For st = 0 To LocalObservedTrials(TestTrialIndex).SubTrials.Count - 1
            DataSetColumnList.Add(LocalObservedTrials(TestTrialIndex).SubTrials(st).ListedSpeechTestPropertyValues(SelectedVariables))
        Next
        DataSetColumnList.Add(LocalObservedTrials(TestTrialIndex).ListedSpeechTestPropertyValues(SelectedVariables))

        'ExportStringList.Add(TestTrialIndex & vbTab & LocalObservedTrials(TestTrialIndex).TestResultAsTextRow & vbTab & LocalObservedTrials(TestTrialIndex).ListedSpeechTestPropertyValues(SelectedVariables))
        ExportStringList.Add(String.Join(vbTab, DataSetColumnList))

        Return String.Join(vbCrLf, ExportStringList)

    End Function

    Public MustOverride Function GetSelectedExportVariables() As List(Of String)


#End Region

#Region "Pretest"

    Public MustOverride Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)

#End Region




End Class



