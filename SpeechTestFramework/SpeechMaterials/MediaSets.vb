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

    Public Property MediaParentFolder As String
    Public Property MaskerParentFolder As String
    Public Property BackgroundNonspeechParentFolder As String
    Public Property BackgroundSpeechParentFolder As String

    Public Property WaveFileSampleRate As Integer = 48000
    Public Property WaveFileBitDepth As Integer = 32
    Public Property WaveFileEncoding As Audio.Formats.WaveFormat.WaveFormatEncodings = Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints

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

        MediaParentFolder = "Unechoic-Talker1-RVE\TestWordRecordings"
        MaskerParentFolder = "City-Talker1-RVE\TWRB"
        BackgroundNonspeechParentFolder = "City-Talker1-RVE\BackgroundNonspeech"
        BackgroundSpeechParentFolder = "City-Talker1-RVE\BackgroundSpeech"

    End Sub

    ''' <summary>
    ''' Returns the full file paths to all existing and nonexisting sound recordings assumed to exist in the Media folder in Item1. For lacking recordings a new paths are suggested. Existing file paths are returned in Item2, and nonexisting paths in Item3.
    ''' </summary>
    ''' <param name="SpeechMaterial"></param>
    ''' <returns></returns>
    Public Function GetAllSpeechMaterialComponentAudioPaths(ByVal SpeechMaterial As SpeechMaterialComponent) As Tuple(Of
        List(Of Tuple(Of String, SpeechMaterialComponent)),
        List(Of Tuple(Of String, SpeechMaterialComponent)),
        List(Of Tuple(Of String, SpeechMaterialComponent)))

        Dim AllComponents = SpeechMaterial.GetAllRelatives

        Dim ExistingFilesList As New List(Of Tuple(Of String, SpeechMaterialComponent))
        Dim LackingFilesList As New List(Of Tuple(Of String, SpeechMaterialComponent))
        Dim AllPaths As New List(Of Tuple(Of String, SpeechMaterialComponent))


        For Each Component In AllComponents

            'Skipd to next if no media items are expected
            If Component.MediaFolder = "" Then Continue For

            Dim ExistingFileCount As Integer = 0

            'Checks if the media folder exists
            If IO.Directory.Exists(Component.MediaFolder) = True Then

                'Gets the sound file paths present there 
                Dim FilesPresent = IO.Directory.GetFiles(Component.MediaFolder)
                For Each filePath In FilesPresent
                    If filePath.EndsWith(".wav") Then
                        ExistingFilesList.Add(New Tuple(Of String, SpeechMaterialComponent)(filePath, Component))
                        AllPaths.Add(ExistingFilesList.Last)
                    End If
                Next

                'Notes how many files are present
                ExistingFileCount = ExistingFilesList.Count

            End If

            'Creates file paths for files not present
            For n = ExistingFileCount To MediaAudioItems - 1
                'Creating a file name (avoiding file name conflicts)
                LackingFilesList.Add(New Tuple(Of String, SpeechMaterialComponent)(Utils.CheckFileNameConflict(IO.Path.Combine(Component.MediaFolder, Component.Id & (n).ToString("000") & ".wav")), Component))
                AllPaths.Add(LackingFilesList.Last)
            Next

        Next

        Return New Tuple(Of List(Of Tuple(Of String, SpeechMaterialComponent)),
            List(Of Tuple(Of String, SpeechMaterialComponent)),
            List(Of Tuple(Of String, SpeechMaterialComponent)))(AllPaths, ExistingFilesList, LackingFilesList)

    End Function

    Public Sub CreateLackingAudioPaths(ByVal SpeechMaterial As SpeechMaterialComponent)

        Dim ExpectedAudioPaths = GetAllSpeechMaterialComponentAudioPaths(SpeechMaterial)

        If ExpectedAudioPaths.Item3.Count > 0 Then

            Dim MsgResult = MsgBox(ExpectedAudioPaths.Item3.Count & " audio files are missing from test situation " & TestSituationName & ". Do you want to create new wave files () for these components?", MsgBoxStyle.YesNo)
            If MsgResult = MsgBoxResult.Yes Then

                Dim FilesCreated As Integer = 0

                For Each item In ExpectedAudioPaths.Item3

                    Dim NewPath = item.Item1
                    Dim Component = item.Item2

                    Dim NewSound = New Audio.Sound(New Audio.Formats.WaveFormat(WaveFileSampleRate, WaveFileBitDepth, 1,, WaveFileEncoding))

                    NewSound.SMA = New Audio.Sound.SpeechMaterialAnnotation With {.SegmentationCompleted = False}

                    'Assign SMA values based on all child components!

                    FilesCreated += NewSound.WriteWaveFile(NewPath)

                Next

                MsgBox("Created " & FilesCreated & " of " & ExpectedAudioPaths.Item3.Count & " new audio files.")

            Else
                MsgBox("No files were created.")
            End If

        Else

            MsgBox("All files needed are already in place!")

        End If

    End Sub

End Class

