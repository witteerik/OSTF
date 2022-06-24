'A class that can store MediaSets
Public Class MediaSetLibrary
    Inherits List(Of MediaSet)

End Class

Public Class MediaSet

    Public Const DefaultMediaFolderName As String = "Media"

    Public ParentTestSpecification As TestSpecification

    ''' <summary>
    ''' Describes the media set used in the test situaton in which a test trial is situated
    ''' </summary>
    ''' <returns></returns>
    Public Property MediaSetName As String = "New media set"

    'Information about the talker in the recordings
    Public Property TalkerName As String = ""
    Public Property TalkerGender As Genders = Genders.NotSet
    Public Property TalkerAge As Integer = -1
    Public Property TalkerDialect As String = ""
    Public Property VoiceType As String = ""


    'The following 6 variables are used to ensure that there is an appropriate number of media files stored in the locations:
    'OstaRootPath + MediaSet.MediaParentFolder + SpeechMaterialComponent.MediaFolder
    'and
    'OstaRootPath + MediaSet.MaskerParentFolder + SpeechMaterialComponent.MaskerFolder
    'As well as to determine the number of recordings to create for a speech test if the inbuilt recording and segmentation tool is used.
    Public Property MediaAudioItems As Integer = 5
    Public Property MaskerAudioItems As Integer = 5
    Public Property MediaImageItems As Integer = 0
    Public Property MaskerImageItems As Integer = 0

    Public Property MediaParentFolder As String = ""
    Public Property MaskerParentFolder As String = ""

    ''' <summary>
    ''' Should store the approximate sound pressure level (SPL) of the audio recorded in the auditory non-speech background sounds stored in BackgroundNonspeechParentFolder, and should represent an ecologically feasible situation
    ''' </summary>
    Public Property BackgroundNonspeechRealisticLevel As Double = -999 ' Setting a default value of -999 dB SPL
    Public Property BackgroundNonspeechParentFolder As String = ""
    Public Property BackgroundSpeechParentFolder As String = ""

    ''' <summary>
    ''' The folder containing the recordings used as prototype recordings during the recording od the MediaSet
    ''' </summary>
    ''' <returns></returns>
    Public Property PrototypeMediaParentFolder As String = ""
    ''' <summary>
    ''' The path should point to a sound recording used as prototype when recording prototype recordings for a MediaSet
    ''' </summary>
    ''' <returns></returns>
    Public Property MasterPrototypeRecordingPath As String = ""

    Public Property PrototypeRecordingLevel As Double = -999 ' Setting a default value of -999 dBC


    Public Property LombardNoisePath As String = ""
    Public Property LombardNoiseLevel As Double = -999 ' Setting a default value of -999 dBC

    Public Property WaveFileSampleRate As Integer = 48000
    Public Property WaveFileBitDepth As Integer = 32
    Public Property WaveFileEncoding As Audio.Formats.WaveFormat.WaveFormatEncodings = Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints

    Public Enum Genders
        Male
        Female
        NotSet
    End Enum

    Public Sub WriteToFile()

        Dim OutputList As New List(Of String)

        Dim DefaultOutputDirectory As String = ParentTestSpecification.GetAvailableTestSituationsDirectory()

        'Creates the diurectory if it doesn't exist
        If IO.Directory.Exists(DefaultOutputDirectory) = False Then IO.Directory.CreateDirectory(DefaultOutputDirectory)

        Dim OutputPath As String = Utils.GetSaveFilePath(ParentTestSpecification.GetAvailableTestSituationsDirectory(), "NewMediaSet", {".txt"}, "Supply a media set specification file name")

        If OutputPath = "" Then
            MsgBox("No filename supplied.", MsgBoxStyle.Information, "Saving media set specification")
            Exit Sub
        End If

        OutputList.Add("// This is an OSTA media set specification file")

        OutputList.Add("MediaSetName = " & MediaSetName)
        OutputList.Add("TalkerName = " & TalkerName)
        OutputList.Add("TalkerGender = " & TalkerGender.ToString)
        OutputList.Add("TalkerAge = " & TalkerAge)
        OutputList.Add("TalkerDialect = " & TalkerDialect)
        OutputList.Add("VoiceType = " & VoiceType)
        OutputList.Add("MediaAudioItems = " & MediaAudioItems)
        OutputList.Add("MaskerAudioItems = " & MaskerAudioItems)
        OutputList.Add("MediaImageItems = " & MediaImageItems)
        OutputList.Add("MaskerImageItems = " & MaskerImageItems)
        OutputList.Add("MediaParentFolder = " & MediaParentFolder)
        OutputList.Add("MaskerParentFolder = " & MaskerParentFolder)
        OutputList.Add("BackgroundNonspeechParentFolder = " & BackgroundNonspeechParentFolder)
        OutputList.Add("BackgroundSpeechParentFolder = " & BackgroundSpeechParentFolder)
        OutputList.Add("PrototypeMediaParentFolder = " & PrototypeMediaParentFolder)
        OutputList.Add("MasterPrototypeRecordingPath = " & MasterPrototypeRecordingPath)
        OutputList.Add("PrototypeRecordingLevel = " & PrototypeRecordingLevel)
        OutputList.Add("LombardNoisePath = " & LombardNoisePath)
        OutputList.Add("LombardNoiseLevel = " & LombardNoiseLevel)
        OutputList.Add("WaveFileSampleRate = " & WaveFileSampleRate)
        OutputList.Add("WaveFileBitDepth = " & WaveFileBitDepth)
        OutputList.Add("WaveFileEncoding = " & WaveFileEncoding.ToString)

        Utils.SendInfoToLog(String.Join(vbCrLf, OutputList), IO.Path.GetFileNameWithoutExtension(OutputPath), IO.Path.GetDirectoryName(OutputPath), True, True, True)

    End Sub

    Public Shared Function LoadMediaSet(ByVal FilePath As String) As MediaSet

        'Gets a file path from the user if none is supplied
        If FilePath = "" Then FilePath = Utils.GetOpenFilePath(,, {".txt"}, "Please open a stuctured media set .txt file.")
        If FilePath = "" Then
            MsgBox("No file selected!")
            Return Nothing
        End If

        'Creates a new random that will be references in all speech material components
        Dim rnd As New Random

        Dim Output As New MediaSet

        'Parses the input file
        Dim InputLines() As String = System.IO.File.ReadAllLines(FilePath, Text.Encoding.UTF8)

        For Each Line In InputLines

            'Skipping blank lines
            If Line.Trim = "" Then Continue For

            'Also skipping commentary only lines 
            If Line.Trim.StartsWith("//") Then Continue For

            If Line.StartsWith("MediaSetName") Then Output.MediaSetName = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("TalkerName") Then Output.TalkerName = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("TalkerGender") Then
                Dim Value = InputFileSupport.InputFileEnumValueParsing(Line, GetType(Genders), FilePath, True)
                If Value.HasValue Then
                    Output.TalkerGender = Value
                Else
                    MsgBox("Failed to read the TalkerGender value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("TalkerAge") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.TalkerAge = Value
                Else
                    MsgBox("Failed to read the TalkerAge value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("TalkerDialect") Then Output.TalkerDialect = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("VoiceType") Then Output.VoiceType = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("MediaAudioItems") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.MediaAudioItems = Value
                Else
                    MsgBox("Failed to read the MediaAudioItems value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("MaskerAudioItems") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.MaskerAudioItems = Value
                Else
                    MsgBox("Failed to read the MaskerAudioItems value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("MediaImageItems") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.MediaImageItems = Value
                Else
                    MsgBox("Failed to read the MediaImageItems value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("MaskerImageItems") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.MaskerImageItems = Value
                Else
                    MsgBox("Failed to read the MaskerImageItems value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("MediaParentFolder") Then Output.MediaParentFolder = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("MaskerParentFolder") Then Output.MaskerParentFolder = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("BackgroundNonspeechParentFolder") Then Output.BackgroundNonspeechParentFolder = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("BackgroundSpeechParentFolder") Then Output.BackgroundSpeechParentFolder = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("PrototypeMediaParentFolder") Then Output.PrototypeMediaParentFolder = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("MasterPrototypeRecordingPath") Then Output.MasterPrototypeRecordingPath = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("PrototypeRecordingLevel") Then
                Dim Value = InputFileSupport.InputFileDoubleValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.PrototypeRecordingLevel = Value
                Else
                    MsgBox("Failed to read the PrototypeRecordingLevel value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("LombardNoisePath") Then Output.LombardNoisePath = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("LombardNoiseLevel") Then
                Dim Value = InputFileSupport.InputFileDoubleValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.LombardNoiseLevel = Value
                Else
                    MsgBox("Failed to read the LombardNoiseLevel value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("WaveFileSampleRate") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.WaveFileSampleRate = Value
                Else
                    MsgBox("Failed to read the WaveFileSampleRate value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("WaveFileBitDepth") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.WaveFileBitDepth = Value
                Else
                    MsgBox("Failed to read the WaveFileBitDepth value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("WaveFileEncoding") Then
                Dim Value = InputFileSupport.InputFileEnumValueParsing(Line, GetType(Audio.Formats.WaveFormat.WaveFormatEncodings), FilePath, True)
                If Value.HasValue Then
                    Output.WaveFileEncoding = Value
                Else
                    MsgBox("Failed to read the WaveFileEncoding value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

        Next

        Return Output

    End Function


    Public Sub SetSipValues(ByVal Voice As Integer)

        Select Case Voice
            Case 1
                MediaSetName = "City-Talker1-RVE"

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

                PrototypeMediaParentFolder = ""

                MasterPrototypeRecordingPath = ""
            Case 2

                MediaSetName = "City-Talker2-RVE"

                TalkerName = "EL"
                TalkerGender = Genders.Female
                TalkerAge = 40
                TalkerDialect = "Central Swedish"
                VoiceType = "Raised vocal effort"

                BackgroundNonspeechRealisticLevel = 55

                MediaAudioItems = 5
                MaskerAudioItems = 5
                MediaImageItems = 0
                MaskerImageItems = 0

                MediaParentFolder = "Media\Unechoic-Talker2-RVE\TestWordRecordings"
                MaskerParentFolder = "Media\City-Talker2-RVE\TWRB"
                BackgroundNonspeechParentFolder = "Media\City-Talker2-RVE\BackgroundNonspeech"
                BackgroundSpeechParentFolder = "Media\City-Talker2-RVE\BackgroundSpeech"

                PrototypeMediaParentFolder = ""

                MasterPrototypeRecordingPath = ""
        End Select

    End Sub

    Public Sub SetHintDebugValues()

        MediaSetName = "Unechoic-Talker1-RVE"

        TalkerName = "EW"
        TalkerGender = Genders.Male
        TalkerAge = 42
        TalkerDialect = "Western Swedish"
        VoiceType = "Raised vocal effort"

        BackgroundNonspeechRealisticLevel = 55

        MediaAudioItems = 2
        MaskerAudioItems = 5
        MediaImageItems = 0
        MaskerImageItems = 0

        MediaParentFolder = "Media\Unechoic-Talker1-RVE\TestWordRecordings"
        MaskerParentFolder = ""
        BackgroundNonspeechParentFolder = ""
        BackgroundSpeechParentFolder = ""

        PrototypeMediaParentFolder = ""

        MasterPrototypeRecordingPath = ""

    End Sub
    Public Enum PrototypeRecordingOptions
        MasterPrototypeRecording
        PrototypeRecordings
        None
    End Enum

    ''' <summary>
    ''' Returns the full file paths to all existing and nonexisting sound recordings assumed to exist in the Media folder in Item1, along with the the path to a corresponding sound to be used as prototype recording as Item2 and a reference to the corresponding Component as Item3.
    ''' For lacking recordings a new paths are suggested. Existing file paths are returned in Item2, and nonexisting paths in Item3.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetAllSpeechMaterialComponentAudioPaths(ByVal PrototypeRecordingOption As PrototypeRecordingOptions) As Tuple(Of
        List(Of Tuple(Of String, String, SpeechMaterialComponent)),
        List(Of Tuple(Of String, String, SpeechMaterialComponent)),
        List(Of Tuple(Of String, String, SpeechMaterialComponent)))


        Dim CurrentTestRootPath As String = ParentTestSpecification.GetTestRootPath

        Dim AllComponents = ParentTestSpecification.SpeechMaterial.GetAllRelatives

        Dim ExistingFilesList As New List(Of Tuple(Of String, String, SpeechMaterialComponent))
        Dim LackingFilesList As New List(Of Tuple(Of String, String, SpeechMaterialComponent))
        Dim AllPaths As New List(Of Tuple(Of String, String, SpeechMaterialComponent))

        For Each Component In AllComponents

            'Skips to next if no media items are expected
            If Component.MediaFolder = "" Then Continue For

            Dim FullMediaFolderPath = IO.Path.Combine(CurrentTestRootPath, MediaParentFolder, Component.MediaFolder)

            'Selects the appropriate prototype recording depending on the value of PrototypeRecordingOption
            Dim PrototypeRecordingPath As String = ""
            Select Case PrototypeRecordingOption
                Case PrototypeRecordingOptions.MasterPrototypeRecording

                    'Getting the file path
                    PrototypeRecordingPath = IO.Path.Combine(CurrentTestRootPath, MasterPrototypeRecordingPath)

                    'Ensuring that the master prototype recoding exist
                    If IO.File.Exists(PrototypeRecordingPath) = False Then
                        MsgBox("The master prototype recoding cannot be found at: " & PrototypeRecordingPath, MsgBoxStyle.Information, "Lacking master prototype recording")
                        Return Nothing
                    End If

                Case PrototypeRecordingOptions.PrototypeRecordings

                    'Getting the folder
                    PrototypeRecordingPath = IO.Path.Combine(CurrentTestRootPath, PrototypeMediaParentFolder, Component.MediaFolder)

                    'Using the first recording (if more than one exist) as the prototype recording
                    If IO.Directory.Exists(PrototypeRecordingPath) = True Then

                        'Getting the file path
                        'Selecting the sound file paths present there 
                        Dim PrototypeRecordingFound As Boolean = False
                        Dim FilesPresent = IO.Directory.GetFiles(PrototypeRecordingPath)
                        For Each filePath In FilesPresent
                            If filePath.EndsWith(".wav") Then
                                PrototypeRecordingPath = filePath
                                PrototypeRecordingFound = True
                                Exit For
                            End If
                        Next

                        'Ensuring that a prototype recoding was found
                        If PrototypeRecordingFound = False Then
                            MsgBox("No wave file could be found in the prototype recoding folder: " & PrototypeRecordingPath, MsgBoxStyle.Information, "Lacking prototype recording")
                            Return Nothing
                        End If

                    Else
                        MsgBox("The following prototype recoding folder cannot be found: " & PrototypeRecordingPath, MsgBoxStyle.Information, "Lacking prototype recording folder")
                        Return Nothing

                    End If

                Case PrototypeRecordingOptions.None
                    'No prototype recoding  is to be used
                    PrototypeRecordingPath = ""
            End Select


            Dim ExistingFileCount As Integer = 0

            'Checks if the media folder exists
            If IO.Directory.Exists(FullMediaFolderPath) = True Then

                'Gets the sound file paths present there 
                Dim FilesPresent = IO.Directory.GetFiles(FullMediaFolderPath)
                For Each filePath In FilesPresent
                    If filePath.EndsWith(".wav") Then
                        ExistingFilesList.Add(New Tuple(Of String, String, SpeechMaterialComponent)(filePath, PrototypeRecordingPath, Component))
                        AllPaths.Add(ExistingFilesList.Last)
                        ExistingFileCount += 1
                    End If
                Next

                'Notes how many files are present
                ExistingFileCount = ExistingFileCount

            End If

            'Creates file paths for files not present
            For n = ExistingFileCount To MediaAudioItems - 1
                'Creating a file name (avoiding file name conflicts)
                LackingFilesList.Add(New Tuple(Of String, String, SpeechMaterialComponent)(Utils.CheckFileNameConflict(IO.Path.Combine(FullMediaFolderPath, Component.MediaFolder & "_" & (n).ToString("000") & ".wav")), PrototypeRecordingPath, Component))
                AllPaths.Add(LackingFilesList.Last)
            Next

        Next

        Return New Tuple(Of List(Of Tuple(Of String, String, SpeechMaterialComponent)),
            List(Of Tuple(Of String, String, SpeechMaterialComponent)),
            List(Of Tuple(Of String, String, SpeechMaterialComponent)))(AllPaths, ExistingFilesList, LackingFilesList)

    End Function

    ''' <summary>
    ''' Checks whether there are missing audio media files, and offers an option to create the files needed. Returns the a tuple containing the number of created files in Item1 and the number files still lacking upon return in Item2.
    ''' </summary>
    ''' <returns>Returns the a tuple containing the number of created files in Item1 and the number files still lacking upon return in Item2.</returns>
    Public Function CreateLackingAudioMediaFiles(ByVal PrototypeRecordingOption As PrototypeRecordingOptions,
                                                 Optional SupressUnnecessaryMessages As Boolean = False) As Tuple(Of Integer, Integer)

        Dim ExpectedAudioPaths = GetAllSpeechMaterialComponentAudioPaths(PrototypeRecordingOption)

        Dim FilesCreated As Integer = 0

        If ExpectedAudioPaths.Item3.Count > 0 Then

            Dim MsgResult = MsgBox(ExpectedAudioPaths.Item3.Count & " audio files are missing from media set " & MediaSetName & ". Do you want to prepare new wave files for these components?", MsgBoxStyle.YesNo)
            If MsgResult = MsgBoxResult.Yes Then


                For Each item In ExpectedAudioPaths.Item3

                    Dim NewPath = item.Item1
                    '(N.B. Prototype recording paths are never directly created, but should instead be created as a separate MediaSet)
                    Dim Component = item.Item3

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


    Public Sub RecordAndEditAudioMediaFiles(ByVal SpeechMaterialRecorderSoundFileLoadOption As SpeechMaterialRecorderLoadOptions,
                                            ByVal PrototypeRecordingOption As PrototypeRecordingOptions,
                                            Optional ByRef RandomItemOrder As Boolean = True)

        'Checks first that all expected sound files exist
        Dim FilesStillLacking = CreateLackingAudioMediaFiles(PrototypeRecordingOption, True).Item2

        If FilesStillLacking > 0 Then
            MsgBox("All audio files needed were not created. Exiting RecordAudioMediaFiles.")
            Exit Sub
        End If

        'Getting all paths
        Dim AudioPaths = GetAllSpeechMaterialComponentAudioPaths(PrototypeRecordingOption)

        Dim FilesForRecordAndEdit As New List(Of Tuple(Of String, String))

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

            Select Case SpeechMaterialRecorderSoundFileLoadOption
                Case SpeechMaterialRecorderLoadOptions.LoadAllSounds

                    'Adds all files
                    FilesForRecordAndEdit.Add(New Tuple(Of String, String)(soundFileTuple.Item1, soundFileTuple.Item2))

                Case SpeechMaterialRecorderLoadOptions.LoadOnlyEmptySounds

                    'Checks if the sound is empty (N.B. Expects only mono sounds here!)
                    If LoadedSound.WaveData.SampleData(1).Length = 0 Then
                        'Adds the sound
                        FilesForRecordAndEdit.Add(New Tuple(Of String, String)(soundFileTuple.Item1, soundFileTuple.Item2))
                    End If

                Case SpeechMaterialRecorderLoadOptions.LoadOnlySoundsWithoutCompletedSegmentation

                    'Checks if segmentation if completed
                    If LoadedSound.SMA.SegmentationCompleted = False Then
                        'Adds the sound
                        FilesForRecordAndEdit.Add(New Tuple(Of String, String)(soundFileTuple.Item1, soundFileTuple.Item2))
                    End If

            End Select

        Next

        'Launches the Recorder GUI

        Dim RecorderGUI As New SpeechMaterialRecorder(Me, FilesForRecordAndEdit, RandomItemOrder)

        RecorderGUI.Show()


    End Sub

    ''' <summary>
    ''' Copies all sound files to a folder structure which is based on the Id of the speech material component.
    ''' </summary>
    Public Sub CopySoundFiles(ByVal OutputFolder As String)

        Dim AllSoundPathTuples = GetAllSpeechMaterialComponentAudioPaths(PrototypeRecordingOptions.None)

        'Creates the OutputFolder 
        If IO.Directory.Exists(OutputFolder) = False Then IO.Directory.CreateDirectory(OutputFolder)

        'Moves sound files
        For Each Item In AllSoundPathTuples.Item1

            Dim InputSoundFilePath = Item.Item1
            Dim SpeechMaterialComponent = Item.Item3

            Dim TargetDirectory As String = IO.Path.Combine(OutputFolder, SpeechMaterialComponent.Id)
            If IO.Directory.Exists(TargetDirectory) = False Then IO.Directory.CreateDirectory(TargetDirectory)

            Dim OutputPath As String = IO.Path.Combine(TargetDirectory, IO.Path.GetFileName(InputSoundFilePath))

            IO.File.Copy(InputSoundFilePath, OutputPath)

        Next

        MsgBox("Finished copying files.")

    End Sub


    Public Function CreateRecordingWaveFormat() As Audio.Formats.WaveFormat
        Return New Audio.Formats.WaveFormat(Me.WaveFileSampleRate, Me.WaveFileBitDepth, 1,, Me.WaveFileEncoding)
    End Function

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

    Public Overrides Function ToString() As String
        Return Me.MediaSetName
    End Function

End Class

