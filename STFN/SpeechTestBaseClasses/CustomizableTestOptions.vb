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
    Private _StartList As String = ""
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
            _ReferenceLevel = value
            OnPropertyChanged()
        End Set
    End Property
    Private _ReferenceLevel As Double
    Public Property ReferenceLevelTitle As String = "Referensnivå"

    Public Property SpeechLevel As Double
        Get
            Return _SpeechLevel
        End Get
        Set(value As Double)
            _SpeechLevel = value
            OnPropertyChanged()
        End Set
    End Property
    Private _SpeechLevel As Double
    Public Property SpeechLevelTitle As String = "Talnivå"

    Public Property MaskingLevel As Double
        Get
            Return _MaskingLevel
        End Get
        Set(value As Double)
            _MaskingLevel = value
            OnPropertyChanged()
        End Set
    End Property
    Private _MaskingLevel As Double
    Public Property MaskingLevelTitle As String = "Maskeringsnivå"


    Public Property BackgroundLevel As Double
        Get
            Return _BackgroundLevel
        End Get
        Set(value As Double)
            _BackgroundLevel = value
            OnPropertyChanged()
        End Set
    End Property
    Private _BackgroundLevel As Double
    Public Property BackgroundLevelTitle As String = "Bakgrundsnivå"

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
    Private _ScoreOnlyKeyWords As Boolean
    Public Property ScoreOnlyKeyWordsTitle As String = "Rätta på nyckelord"

    Public Property RandomizeItemsListOrder As Boolean
        Get
            Return _RandomizeItemsListOrder
        End Get
        Set(value As Boolean)
            _RandomizeItemsListOrder = value
            OnPropertyChanged()
        End Set
    End Property
    Private _RandomizeItemsListOrder As Boolean
    Public Property RandomizeItemsListOrderTitle As String = "Slumpa listordning"

    Public Property RandomizeItemsWithinLists As Boolean
        Get
            Return _RandomizeItemsWithinLists
        End Get
        Set(value As Boolean)
            _RandomizeItemsWithinLists = value
            OnPropertyChanged()
        End Set
    End Property
    Private _RandomizeItemsWithinLists As Boolean
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
    Private _RandomizeItemsAcrossLists As Boolean
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
    Private _IsFreeRecall As Boolean
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
    Private _ShowDidNotHearResponseAlternative As Boolean
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

    Public Property SelectedPresentationMode As SoundPropagationTypes
        Get
            Return _SelectedPresentationMode
        End Get
        Set(value As SoundPropagationTypes)
            _SelectedPresentationMode = value
            OnPropertyChanged()
        End Set
    End Property
    Private _SelectedPresentationMode As SoundPropagationTypes
    Public Property SelectedPresentationModeTitle As String = "Ljudutbredningsläge"


    ''' <summary>
    ''' Should specify the locations from which the signals should come
    ''' </summary>
    ''' <returns></returns>
    Public Property SignalLocations As List(Of Audio.SoundScene.SoundSourceLocation)
        Get
            Return _SignalLocations
        End Get
        Set(value As List(Of Audio.SoundScene.SoundSourceLocation))
            _SignalLocations = value
            OnPropertyChanged()
        End Set
    End Property
    Private _SignalLocations As List(Of Audio.SoundScene.SoundSourceLocation)
    Public Property SignalLocationsTitle As String = "Placering av talkälla/or"


    ''' <summary>
    ''' Should specify the locations from which the maskers should come
    ''' </summary>
    ''' <returns></returns>
    Public Property MaskerLocations As List(Of Audio.SoundScene.SoundSourceLocation)
        Get
            Return _MaskerLocations
        End Get
        Set(value As List(Of Audio.SoundScene.SoundSourceLocation))
            _MaskerLocations = value
            OnPropertyChanged()
        End Set
    End Property
    Private _MaskerLocations As List(Of Audio.SoundScene.SoundSourceLocation)
    Public Property MaskerLocationsTitle As String = "Placering av maskeringsljud"


    ''' <summary>
    ''' Should specify the locations from which the background sounds should come
    ''' </summary>
    ''' <returns></returns>
    Public Property BackgroundLocations As List(Of Audio.SoundScene.SoundSourceLocation)
        Get
            Return _BackgroundLocations
        End Get
        Set(value As List(Of Audio.SoundScene.SoundSourceLocation))
            _BackgroundLocations = value
            OnPropertyChanged()
        End Set
    End Property
    Private _BackgroundLocations As List(Of Audio.SoundScene.SoundSourceLocation)
    Public Property BackgroundLocationsTitle As String = "Placering av bakgrundsljud"


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
    Private _UseContralateralMasking As Boolean
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
    Private _UsePhaseAudiometry As Boolean
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
