Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class CustomizableTestOptions
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub OnPropertyChanged(<CallerMemberName> Optional name As String = "")
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    Public Sub New()
        SelectedTest.CustomizableTestOptions = Me
    End Sub

    Public ReadOnly Property SelectedTest As SpeechTest
        Get
            Return SharedSpeechTestObjects.CurrentSpeechTest
        End Get
    End Property

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
    Public Property IsPractiseTestTitle As String = "Övningstest"


    ''' <summary>
    ''' If specified, should indicate the name of the selected pretest (Optional).
    ''' </summary>
    ''' <returns></returns>
    Public Property SelectedPreset As SmcPresets.Preset
        Get
            Return _SelectedPreset
        End Get
        Set(value As SmcPresets.Preset)
            _SelectedPreset = value
            OnPropertyChanged()
        End Set
    End Property
    Private _SelectedPreset As SmcPresets.Preset
    Public Property SelectedPresetTitle As String = "Förval"


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
    Public Property StartListTitle As String = "StartLista"

    ''' <summary>
    ''' Should contain the media sets to be included in the test (Obligatory). (Note that this determines the type of signal, masking and background sounds used, and should be used for instance to select type of masking sounds (i.e. babble noise, SWN, etc., which then need to be implemented as separate MediaSets for each SpeechMaterial)
    ''' </summary>
    ''' <returns></returns>
    Public Property SelectedMediaSet As MediaSet
        Get
            Return _SelectedMediaSet
        End Get
        Set(value As MediaSet)
            _SelectedMediaSet = value
            OnPropertyChanged()
        End Set
    End Property
    Private _SelectedMediaSet As New MediaSet
    Public Property SelectedMediaSetTitle As String = "Mediaset"


    ''' <summary>
    ''' Should contain the media sets to be included in the test (Obligatory). (Note that this determines the type of signal, masking and background sounds used, and should be used for instance to select type of masking sounds (i.e. babble noise, SWN, etc., which then need to be implemented as separate MediaSets for each SpeechMaterial)
    ''' </summary>
    ''' <returns></returns>
    Public Property SelectedMediaSets As MediaSetLibrary
        Get
            Return _SelectedMediaSets
        End Get
        Set(value As MediaSetLibrary)
            _SelectedMediaSets = value
            OnPropertyChanged()
        End Set
    End Property
    Private _SelectedMediaSets As New MediaSetLibrary
    Public Property SelectedMediaSetsTitle As String = "Mediaset"

    Public Property ReferenceLevel As Double
        Get
            Return _ReferenceLevel
        End Get
        Set(value As Double)
            _ReferenceLevel = Math.Round(value)
            OnPropertyChanged()
        End Set
    End Property
    Private _ReferenceLevel As Double = 65
    Public Property ReferenceLevelTitle As String = "Referensnivå (dB)"

    Public Property SpeechLevel As Double
        Get
            Return _SpeechLevel
        End Get
        Set(value As Double)
            _SpeechLevel = Math.Round(value)
            OnPropertyChanged()
        End Set
    End Property
    Private _SpeechLevel As Double = 65
    Public Property SpeechLevelTitle As String = "Talnivå (dB)"

    Public Property MaskingLevel As Double
        Get
            Return _MaskingLevel
        End Get
        Set(value As Double)
            _MaskingLevel = Math.Round(value)
            OnPropertyChanged()
        End Set
    End Property
    Private _MaskingLevel As Double = 65
    Public Property MaskingLevelTitle As String = "Maskeringsnivå (dB)"


    Public Property BackgroundLevel As Double
        Get
            Return _BackgroundLevel
        End Get
        Set(value As Double)
            _BackgroundLevel = Math.Round(value)
            OnPropertyChanged()
        End Set
    End Property
    Private _BackgroundLevel As Double = 55
    Public Property BackgroundLevelTitle As String = "Bakgrundsnivå (dB)"

    ''TODO: Should we have a customizable limit here, or should we use the "LimiterThreshold" specified for each transducer in the AudioSystemSpecification.txt file?
    '''' <summary>
    '''' This 
    '''' </summary>
    '''' <returns></returns>
    'Public Property SaveLevelLimit As Double = 100



    ''' <summary>
    ''' The SelectedTestMode property is used to determine which type of test protocol that should be used
    ''' </summary>
    ''' <returns></returns>
    Public Property SelectedTestMode As SpeechTest.TestModes
        Get
            Return _TestMode
        End Get
        Set(value As SpeechTest.TestModes)
            _TestMode = value
            OnPropertyChanged()
        End Set
    End Property
    Private _TestMode As SpeechTest.TestModes
    Public Property SelectedTestModeTitle As String = "Testläge"

    Public Property SelectedTestProtocol As TestProtocol
        Get
            Return _SelectedTestProtocol
        End Get
        Set(value As TestProtocol)
            _SelectedTestProtocol = value
            OnPropertyChanged()
        End Set
    End Property
    Private _SelectedTestProtocol As TestProtocol
    Public Property SelectedTestProtocolTitle As String = "Testprotokoll"


    Public Property ScoreOnlyKeyWords As Boolean
        Get
            Return _ScoreOnlyKeyWords
        End Get
        Set(value As Boolean)
            _ScoreOnlyKeyWords = value
            OnPropertyChanged()
        End Set
    End Property
    Private _ScoreOnlyKeyWords As Boolean = False
    Public Property ScoreOnlyKeyWordsTitle As String = "Rätta på nyckelord"

    Public Property RandomizeListOrder As Boolean
        Get
            Return _RandomizeListOrder
        End Get
        Set(value As Boolean)
            _RandomizeListOrder = value
            OnPropertyChanged()
        End Set
    End Property
    Private _RandomizeListOrder As Boolean = False
    Public Property RandomizeListOrderTitle As String = "Slumpa listordning"

    Public Property RandomizeItemsWithinLists As Boolean
        Get
            Return _RandomizeItemsWithinLists
        End Get
        Set(value As Boolean)
            _RandomizeItemsWithinLists = value
            OnPropertyChanged()
        End Set
    End Property
    Private _RandomizeItemsWithinLists As Boolean = False
    Public Property RandomizeItemsWithinListsTitle As String = "Slumpa inom listor"

    Public Property RandomizeItemsAcrossLists As Boolean
        Get
            Return _RandomizeItemsAcrossLists
        End Get
        Set(value As Boolean)
            _RandomizeItemsAcrossLists = value
            OnPropertyChanged()
        End Set
    End Property
    Private _RandomizeItemsAcrossLists As Boolean = False
    Public Property RandomizeItemsAcrossListsTitle As String = "Slumpa mellan listor"

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
    Public Property IsFreeRecallTitle As String = "Fri rapportering"

    Public Property ShowDidNotHearResponseAlternative As Boolean
        Get
            Return _ShowDidNotHearResponseAlternative
        End Get
        Set(value As Boolean)
            _ShowDidNotHearResponseAlternative = value
            OnPropertyChanged()
        End Set
    End Property
    Private _ShowDidNotHearResponseAlternative As Boolean = False
    Public Property ShowDidNotHearResponseAlternativeTitle As String = "Visa ? som alternativ"

    Public Property FixedResponseAlternativeCount As Integer
        Get
            Return _FixedResponseAlternativeCount
        End Get
        Set(value As Integer)
            _FixedResponseAlternativeCount = value
            OnPropertyChanged()
        End Set
    End Property
    Private _FixedResponseAlternativeCount As Integer = 4
    Public Property FixedResponseAlternativeCountTitle As String = "Antal svarsalternativ"

    Public Property SelectedTransducer As AudioSystemSpecification
        Get
            Return _SelectedTransducer
        End Get
        Set(value As AudioSystemSpecification)
            _SelectedTransducer = value

            'Inactivates the use of simulated sound field is the transducer is not headphones
            'TODO. Note that this only works if headphones are connected to hardware output 1 and 2. In order to work in other cases, the software somehow needs to know to what hardware the headphones are connected.
            If _SelectedTransducer.IsHeadphones(1, 2) = False Then UseSimulatedSoundField = False
            OnPropertyChanged()
        End Set
    End Property
    Private _SelectedTransducer As AudioSystemSpecification
    Public Property SelectedTransducerTitle As String = "Ljudgivare"

    Public Property UseSimulatedSoundField As Boolean
        Get
            Return _UseSimulatedSoundField
        End Get
        Set(value As Boolean)
            _UseSimulatedSoundField = value
            OnPropertyChanged()
        End Set
    End Property
    Private _UseSimulatedSoundField As Boolean = False
    Public Property UseSimulatedSoundFieldTitle As String = "Simulera ljudfält"

    Public Property SelectedIrSet As BinauralImpulseReponseSet
        Get
            Return _SelectedIrSet
        End Get
        Set(value As BinauralImpulseReponseSet)
            _SelectedIrSet = value
            OnPropertyChanged()
        End Set
    End Property
    Private _SelectedIrSet As BinauralImpulseReponseSet = Nothing
    Public Property SelectedIrSetTitle As String = "HRIR"


    'Returns the selected SelectedPresentationMode indirectly based on the UseSimulatedSoundField property. In the future if more options are supported, this will have to be exposed to the GUI.
    Public ReadOnly Property SelectedPresentationMode As SoundPropagationTypes
        Get
            If UseSimulatedSoundField = False Then
                Return SoundPropagationTypes.PointSpeakers
            Else
                Return SoundPropagationTypes.SimulatedSoundField
            End If
        End Get
    End Property


    ''' <summary>
    ''' Should specify the locations from which the signals should come
    ''' </summary>
    ''' <returns></returns>
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
    Public Property SignalLocationsTitle As String = "Placering av talkälla/or"

    ''' <summary>
    ''' Returns the selected locations from which the signals should come
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
    ''' Should specify the locations from which the Maskers should come
    ''' </summary>
    ''' <returns></returns>
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
    Public Property MaskerLocationsTitle As String = "Placering av maskeringsljud"


    ''' <summary>
    ''' Should specify the locations from which the maskers should come
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
    ''' Should specify the locations from which the background (non-speech) sounds should come
    ''' </summary>
    ''' <returns></returns>
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
    Public Property BackgroundNonSpeechLocationsTitle As String = "Placering av bakgrundsljud"

    ''' <summary>
    ''' Should specify the locations from which the BackgroundNonSpeech non-speech sounds should come
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
    ''' Should specify the locations from which the background speech sounds should come
    ''' </summary>
    ''' <returns></returns>
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
    Public Property BackgroundSpeechLocationsTitle As String = "Placering av bakgrundstal"


    ''' <summary>
    ''' Should specify the locations from which the BackgroundSpeech speech sounds should come
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
    Public Property UseContralateralMasking As Boolean
        Get
            Return _UseContralateralMasking
        End Get
        Set(value As Boolean)
            _UseContralateralMasking = value
            OnPropertyChanged()
        End Set
    End Property
    Private _UseContralateralMasking As Boolean = False
    Public Property UseContralateralMaskingTitle As String = "Kontralateral maskering"

    ''' <summary>
    ''' This is intended as a shortcut which must override all of the SignalLocations, MaskerLocations and BackgroundLocations properties. It can only be specified if SelectedPresentationMode is either SimulatedSoundField with locations along the mid-sagittal plane, or PointSpeakers at -90 and 90 degrees with distance of 0 (i.e. headphones), and where the signal i set to come from only one side.
    ''' </summary>
    ''' <returns></returns>
    Public Property UsePhaseAudiometry As Boolean
        Get
            Return _UsePhaseAudiometry
        End Get
        Set(value As Boolean)
            _UsePhaseAudiometry = value
            OnPropertyChanged()
        End Set
    End Property
    Private _UsePhaseAudiometry As Boolean = False
    Public Property UsePhaseAudiometryTitle As String = "Fasaudiometri"


    ''' <summary>
    ''' Determines which type of phase audiometry to use, if UsePhaseAudiometry is True.
    ''' </summary>
    ''' <returns></returns>
    Public Property SelectedPhaseAudiometryType As BmldModes
        Get
            Return _SelectedPhaseAudiometryType
        End Get
        Set(value As BmldModes)
            _SelectedPhaseAudiometryType = value
            OnPropertyChanged()
        End Set
    End Property
    Private _SelectedPhaseAudiometryType As BmldModes
    Public Property SelectedPhaseAudiometryTypeTitle As String = "Fasaudiometrityp"


End Class
