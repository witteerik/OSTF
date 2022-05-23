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

        MediaParentFolder = "Media\Unechoic-Talker1-RVE\TestWordRecordings"
        MaskerParentFolder = "Media\City-Talker1-RVE\TWRB"
        BackgroundNonspeechParentFolder = "Media\City-Talker1-RVE\BackgroundNonspeech"
        BackgroundSpeechParentFolder = "Media\City-Talker1-RVE\BackgroundSpeech"

    End Sub

    ''' <summary>
    ''' Returns the full file paths to all existing and nonexisting sound recordings assumed to exist in the Media folder in Item1, along with the (first, if several) SpeechMaterialComponent using those paths. 
    ''' For lacking recordings a new paths are suggested. Existing file paths are returned in Item2, and nonexisting paths in Item3.
    ''' </summary>
    ''' <param name="SpeechMaterial"></param>
    ''' <returns></returns>
    Public Function GetAllSpeechMaterialComponentAudioPaths(ByVal SpeechMaterial As SpeechMaterialComponent) As Tuple(Of
        List(Of Tuple(Of String, SpeechMaterialComponent)),
        List(Of Tuple(Of String, SpeechMaterialComponent)),
        List(Of Tuple(Of String, SpeechMaterialComponent)))

        Dim CurrentTestRootPath As String = IO.Path.Combine(OstfSettings.RootPath, OstfSettings.CurrentTestSubPath)

        Dim AllComponents = SpeechMaterial.GetAllRelatives

        Dim ExistingFilesList As New List(Of Tuple(Of String, SpeechMaterialComponent))
        Dim LackingFilesList As New List(Of Tuple(Of String, SpeechMaterialComponent))
        Dim AllPaths As New List(Of Tuple(Of String, SpeechMaterialComponent))

        Dim AddedMediaFolders As New List(Of String)

        For Each Component In AllComponents

            'Skipd to next if no media items are expected
            If Component.MediaFolder = "" Then Continue For

            Dim FullMediaFolderPath = IO.Path.Combine(CurrentTestRootPath, MediaParentFolder, Component.MediaFolder)

            'Skipping the media folder if it has already been added (this will happen if several differen component share the same media folder)
            If AddedMediaFolders.Contains(FullMediaFolderPath) Then
                'The needed files will have already been added in a previous loop
                Continue For
            Else
                'Notes the MediaFolder in AddedMediaFolders
                AddedMediaFolders.Add(FullMediaFolderPath)
            End If

            Dim ExistingFileCount As Integer = 0

            'Checks if the media folder exists
            If IO.Directory.Exists(FullMediaFolderPath) = True Then

                'Gets the sound file paths present there 
                Dim FilesPresent = IO.Directory.GetFiles(FullMediaFolderPath)
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
                LackingFilesList.Add(New Tuple(Of String, SpeechMaterialComponent)(Utils.CheckFileNameConflict(IO.Path.Combine(FullMediaFolderPath, Component.MediaFolder & "_" & (n).ToString("000") & ".wav")), Component))
                AllPaths.Add(LackingFilesList.Last)
            Next

        Next

        Return New Tuple(Of List(Of Tuple(Of String, SpeechMaterialComponent)),
            List(Of Tuple(Of String, SpeechMaterialComponent)),
            List(Of Tuple(Of String, SpeechMaterialComponent)))(AllPaths, ExistingFilesList, LackingFilesList)

    End Function

    ''' <summary>
    ''' Checks whether there are missing audio media files, and offers an option to create the files needed. Returns the a tuple containing the number of created files in Item1 and the number files still lacking upon return in Item2.
    ''' </summary>
    ''' <param name="SpeechMaterial"></param>
    ''' <returns>Returns the a tuple containing the number of created files in Item1 and the number files still lacking upon return in Item2.</returns>
    Public Function CreateLackingAudioMediaFiles(ByVal SpeechMaterial As SpeechMaterialComponent, Optional SupressUnnecessaryMessages As Boolean = False) As Tuple(Of Integer, Integer)

        Dim ExpectedAudioPaths = GetAllSpeechMaterialComponentAudioPaths(SpeechMaterial)

        Dim FilesCreated As Integer = 0

        If ExpectedAudioPaths.Item3.Count > 0 Then

            Dim MsgResult = MsgBox(ExpectedAudioPaths.Item3.Count & " audio files are missing from test situation " & TestSituationName & ". Do you want to prepare new wave files for these components?", MsgBoxStyle.YesNo)
            If MsgResult = MsgBoxResult.Yes Then


                For Each item In ExpectedAudioPaths.Item3

                    Dim NewPath = item.Item1
                    Dim Component = item.Item2

                    Dim NewSound = New Audio.Sound(New Audio.Formats.WaveFormat(WaveFileSampleRate, WaveFileBitDepth, 1,, WaveFileEncoding))

                    'Assign SMA values based on all child components!
                    NewSound.SMA = Component.ConvertToSMA()

                    'Adds an empty channel 1 array
                    NewSound.WaveData.SampleData(1) = {}

                    'Also assigning the SMA parent sound
                    NewSound.SMA.ParentSound = NewSound

                    If NewSound.WriteWaveFile(NewPath) = True Then FilesCreated += 1

                Next

                MsgBox("Created " & FilesCreated & " of " & ExpectedAudioPaths.Item3.Count & " new audio files.")

            Else
                MsgBox("No files were created.")
            End If

        Else

            If SupressUnnecessaryMessages = False Then MsgBox("All files needed are already in place!")

        End If

        Return New Tuple(Of Integer, Integer)(FilesCreated, ExpectedAudioPaths.Item3.Count - FilesCreated)

    End Function

    Public Enum SpeechMaterialRecorderLoadOptions
        LoadAllSounds
        LoadOnlyEmptySounds
        LoadOnlySoundsWithoutCompletedSegmentation
    End Enum


    Public Sub RecordAndEditAudioMediaFiles(ByVal SpeechMaterial As SpeechMaterialComponent, ByVal SpeechMaterialRecorderLoadOption As SpeechMaterialRecorderLoadOptions)

        'Checks first that all expected sound files exist
        Dim FilesStillLacking = CreateLackingAudioMediaFiles(SpeechMaterial, True).Item2

        If FilesStillLacking > 0 Then
            MsgBox("All audio files needed were not created. Exiting RecordAudioMediaFiles.")
            Exit Sub
        End If

        'Getting all paths
        Dim AudioPaths = GetAllSpeechMaterialComponentAudioPaths(SpeechMaterial)

        Dim FilesForRecordAndEdit As New List(Of Tuple(Of String, SpeechMaterialComponent))

        'Parsing through all sound files (without storing them in memory) and stores the paths to be included in the recorder GUI
        For Each soundFileTuple In AudioPaths.Item1

            Dim LoadedSound = Audio.Sound.LoadWaveFile(soundFileTuple.Item1)

            'Checks the waveformat of the sound
            CheckSoundFileFormat(LoadedSound, soundFileTuple.Item1)

            If LoadedSound Is Nothing Then
                MsgBox("The sound file " & soundFileTuple.Item1 & " could not be loaded.")
                Continue For
            End If

            'Checking that the format agrees with the expected format of the Media set.
            If CheckSoundFileFormat(LoadedSound, soundFileTuple.Item1) = False Then
                'TODO: Lets this through for now, but it may be good to offer a chioce to update the format???
            End If

            Select Case SpeechMaterialRecorderLoadOption
                Case SpeechMaterialRecorderLoadOptions.LoadAllSounds

                    'Adds all files
                    FilesForRecordAndEdit.Add(soundFileTuple)

                Case SpeechMaterialRecorderLoadOptions.LoadOnlyEmptySounds

                    'Checks if the sound is empty (N.B. Expects only mono sounds here!)
                    If LoadedSound.WaveData.SampleData(1).Length = 0 Then
                        'Adds the sound
                        FilesForRecordAndEdit.Add(soundFileTuple)
                    End If

                Case SpeechMaterialRecorderLoadOptions.LoadOnlySoundsWithoutCompletedSegmentation

                    'Checks if segmentation if completed
                    If LoadedSound.SMA.SegmentationCompleted = False Then
                        'Adds the sound
                        FilesForRecordAndEdit.Add(soundFileTuple)
                    End If

            End Select

        Next

        'Launches the Recorder GUI
        Dim RecordingWaveFormat As New Audio.Formats.WaveFormat(Me.WaveFileSampleRate, Me.WaveFileBitDepth, 1,, Me.WaveFileEncoding)

        Dim RecorderGUI As New SpeechMaterialRecorder(FilesForRecordAndEdit, RecordingWaveFormat)

        RecorderGUI.Show()


    End Sub

    Public Function CheckSoundFileFormat(ByRef Sound As Audio.Sound, Optional ByVal SoundFilePath As String = "") As Boolean

        If SoundFilePath = "" Then SoundFilePath = Sound.FileName

        If Sound.WaveFormat.SampleRate <> Me.WaveFileSampleRate Then
            MsgBox("The sound file " & SoundFilePath & " has a different sample rate than what is expected by the current media set.", MsgBoxStyle.Exclamation, "Warning!")
            Return False
        End If
        If Sound.WaveFormat.BitDepth <> Me.WaveFileBitDepth Then
            MsgBox("The sound file " & SoundFilePath & " has a different bitdepth than what is expected by the current media set.", MsgBoxStyle.Exclamation, "Warning!")
            Return False
        End If
        If Sound.WaveFormat.Encoding <> Me.WaveFileEncoding Then
            MsgBox("The sound file " & SoundFilePath & " has a different encoding what is expected by the current media set.", MsgBoxStyle.Exclamation, "Warning!")
            Return False
        End If

        Return True

    End Function

End Class

