Public Class CustomizableTestOptions

    ''' <summary>
    ''' Should indicate whether the test is a practise test or not (Obligatory).
    ''' </summary>
    ''' <returns></returns>
    Public Property IsPractiseTest As Boolean

    ''' <summary>
    ''' If specified, should indicate which test lists to include in the test (Optional).
    ''' </summary>
    ''' <returns></returns>
    Public Property SelectedPreset As List(Of String)


    ''' <summary>
    ''' If specified, should contain the name of the list to present first (Optional).
    ''' </summary>
    ''' <returns></returns>
    Public Property StartList As String

    ''' <summary>
    ''' Should contain the media sets to be included in the test (Obligatory). (Note that this determines the type of signal, masking and background sounds used, and should be used for instance to select type of masking sounds (i.e. babble noise, SWN, etc., which then need to be implemented as separate MediaSets for each SpeechMaterial)
    ''' </summary>
    ''' <returns></returns>
    Public Property SelectedMediaSets As MediaSetLibrary

    Public Property ReferenceLevel As Double
    Public Property SpeechLevel As Double
    Public Property MaskingLevel As Double
    Public Property BackgroundLevel As Double

    ''TODO: Should we have a customizable limit here, or should we use the "LimiterThreshold" specified for each transducer in the AudioSystemSpecification.txt file?
    '''' <summary>
    '''' This 
    '''' </summary>
    '''' <returns></returns>
    'Public Property SaveLevelLimit As Double = 100

    ''' <summary>
    ''' The TestMode property is used to determine which type of test protocol that should be used
    ''' </summary>
    ''' <returns></returns>
    Public Property TestMode As TestModes

    Public Enum TestModes
        FixedLevels
        AdaptiveSpeech
        AdaptiveNoise
    End Enum

    Public Property SelectedTestProtocol As TestProtocol

    Public Property ScoreOnlyKeyWords As Boolean

    Public Property SelectedPresentationMode As SoundPropagationTypes

    ''' <summary>
    ''' Should specify the locations from which the signals should come
    ''' </summary>
    ''' <returns></returns>
    Public Property SignalLocations As List(Of Audio.SoundScene.SoundSourceLocation)

    ''' <summary>
    ''' Should specify the locations from which the maskers should come
    ''' </summary>
    ''' <returns></returns>
    Public Property MaskerLocations As List(Of Audio.SoundScene.SoundSourceLocation)

    ''' <summary>
    ''' Should specify the locations from which the background sounds should come
    ''' </summary>
    ''' <returns></returns>
    Public Property BackgroundLocations As List(Of Audio.SoundScene.SoundSourceLocation)

    ''' <summary>
    ''' This is intended as a shortcut which must override the MaskerLocations property, but can only be specified if SelectedPresentationMode is PointSpeakers at -90 and 90 degrees with distance of 0 (i.e. headphones), and where the signal i set to come from only one side.
    ''' </summary>
    ''' <returns></returns>
    Public Property UseContralateralMasking As Boolean

    ''' <summary>
    ''' This is intended as a shortcut which must override all of the SignalLocations, MaskerLocations and BackgroundLocations properties. It can only be specified if SelectedPresentationMode is either SimulatedSoundField with locations along the mid-sagittal plane, or PointSpeakers at -90 and 90 degrees with distance of 0 (i.e. headphones), and where the signal i set to come from only one side.
    ''' </summary>
    ''' <returns></returns>
    Public Property UsePhaseAudiometry As Boolean

    ''' <summary>
    ''' Determines which type of phase audiometry to use, if UsePhaseAudiometry is True.
    ''' </summary>
    ''' <returns></returns>
    Public Property SelectedPhaseAudiometryType As BmldModes


End Class
