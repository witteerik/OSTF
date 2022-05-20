'A class that can store MediaSets
Public Class MediaSetLibrary
    Inherits SortedList(Of String, MediaSet)

End Class

Public Class MediaSet

    ''' <summary>
    ''' Describes the test situaton in which the trial is situated
    ''' </summary>
    ''' <returns></returns>
    Public Property TestSituationName As String

    'Information about the talker in the recordings
    Public TalkerName As String
    Public TalkerGender As Genders
    Public TalkerAge As Integer
    Public TalkerDialect As String
    Public VoiceType As String

    'Information about the background nonspeech media
    ''' <summary>
    ''' Should store the approximate sound pressure level (SPL) of the audio recorded in the auditory non-speech background sounds stored in BackgroundNonspeechParentFolder, and should represent an ecologically feasible situation
    ''' </summary>
    Public BackgroundNonspeechRealisticLevel As Double

    'The following 6 variables are used to ensure that there is an appropriate number of media files stored in the locations:
    'OstaRootPath + MediaSet.MediaParentFolder + SpeechMaterialComponent.MediaFolder
    'and
    'OstaRootPath + MediaSet.MaskerParentFolder + SpeechMaterialComponent.MaskerFolder
    'As well as to determine the number of recordings to create for a speech test if the inbuilt recording and segmentation tool is used.
    Public MediaAudioItems As Integer = 5
    Public MaskerAudioItems As Integer = 5
    Public MediaImageItems As Integer = 0
    Public MaskerImageItems As Integer = 0
    Public MediaVideoItems As Integer = 0
    Public MaskerVideoItems As Integer = 0

    Public Property MediaParentFolder As String
    Public Property MaskerParentFolder As String
    Public Property BackgroundNonspeechParentFolder As String
    Public Property BackgroundSpeechParentFolder As String


    Public Enum Genders
        Male
        Female
        NotSet
    End Enum

    Public Sub SetSipValues()

        TestSituationName = "City-Talker1-RVE"

        TalkerName = "JE"
        TalkerGender = Genders.Male
        TalkerAge = 50
        TalkerDialect = "Central Swedish"
        VoiceType = "Raised vocal effort"

        BackgroundNonspeechRealisticLevel = 55

        MediaAudioItems = 5
        MaskerAudioItems = 5
        MediaImageItems = 0
        MaskerImageItems = 0
        MediaVideoItems = 0
        MaskerVideoItems = 0

        MediaParentFolder = "Unechoic-Talker1-RVE\TestWordRecordings"
        MaskerParentFolder = "City-Talker1-RVE\TWRB"
        BackgroundNonspeechParentFolder = "City-Talker1-RVE\BackgroundNonspeech"
        BackgroundSpeechParentFolder = "City-Talker1-RVE\BackgroundSpeech"


    End Sub


End Class

