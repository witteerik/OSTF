Imports System.Runtime.CompilerServices
Imports STFN.Core


'This file contains extension methods for STFN.Core.MediaSet

Partial Public Module Extensions



    ''' <summary>
    ''' Checks whether there are missing audio media files, and offers an option to create the files needed. Returns the a tuple containing the number of created files in Item1 and the number files still lacking upon return in Item2.
    ''' </summary>
    ''' <returns>Returns the a tuple containing the number of created files in Item1 and the number files still lacking upon return in Item2.</returns>
    <Extension>
    Public Async Function CreateLackingAudioMediaFiles(obj As STFN.Core.MediaSet, ByVal PrototypeRecordingOption As PrototypeRecordingOptions,
                                                 Optional SupressUnnecessaryMessages As Boolean = False) As Task(Of Tuple(Of Integer, Integer))


        Dim ExpectedAudioPaths = obj.GetAllSpeechMaterialComponentAudioPaths(PrototypeRecordingOption)

        If ExpectedAudioPaths Is Nothing Then
            Return Nothing
        End If

        Dim FilesCreated As Integer = 0

        If ExpectedAudioPaths.Item3.Count > 0 Then

            Dim MsgResult = Await Messager.MsgBoxAcceptQuestion(ExpectedAudioPaths.Item3.Count & " audio files are missing from media set " & obj.MediaSetName & ". Do you want to prepare new wave files for these components?")
            ' TODO: This has not yet been tested. Does the code stop and wait for a response here??? I guess not...
            If MsgResult = True Then

                For Each item In ExpectedAudioPaths.Item3

                    Dim NewPath = item.Item1
                    '(N.B. Prototype recording paths are never directly created, but should instead be created as a separate MediaSet)
                    Dim Component = item.Item3

                    Dim NewSound = New STFN.Core.Audio.Sound(New STFN.Core.Audio.Formats.WaveFormat(obj.WaveFileSampleRate, obj.WaveFileBitDepth, 1,, obj.WaveFileEncoding))

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

    <Extension>
    Public Sub RecordAndEditAudioMediaFiles(obj As STFN.Core.MediaSet, ByVal SpeechMaterialRecorderSoundFileLoadOption As SpeechMaterialRecorderLoadOptions,
                                            ByVal RandomItemOrder As Boolean,
                                            ByVal PrototypeRecordingOption As PrototypeRecordingOptions)

        MsgBox("Recording and editing of media files is not supported in STFN")

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
    <Extension>
    Public Function GetAllSpeechMaterialComponentAudioPaths(obj As STFN.Core.MediaSet, ByVal PrototypeRecordingOption As PrototypeRecordingOptions) As Tuple(Of
        List(Of Tuple(Of String, String, SpeechMaterialComponent)),
        List(Of Tuple(Of String, String, SpeechMaterialComponent)),
        List(Of Tuple(Of String, String, SpeechMaterialComponent)))


        Dim CurrentTestRootPath As String = obj.ParentTestSpecification.GetTestRootPath

        Dim AllComponents = obj.ParentTestSpecification.SpeechMaterial.GetAllRelatives

        Dim ExistingFilesList As New List(Of Tuple(Of String, String, SpeechMaterialComponent))
        Dim LackingFilesList As New List(Of Tuple(Of String, String, SpeechMaterialComponent))
        Dim AllPaths As New List(Of Tuple(Of String, String, SpeechMaterialComponent))

        For Each Component In AllComponents

            'Skips to next if no media items are expected
            If Component.LinguisticLevel <> obj.AudioFileLinguisticLevel Then Continue For

            Dim FullMediaFolderPath = IO.Path.Combine(CurrentTestRootPath, obj.MediaParentFolder, Component.GetMediaFolderName)


            'Selects the appropriate prototype recording depending on the value of PrototypeRecordingOption
            Dim PrototypeRecordingPath As String = ""
            Select Case PrototypeRecordingOption
                Case PrototypeRecordingOptions.MasterPrototypeRecording

                    'Getting the file path
                    PrototypeRecordingPath = IO.Path.Combine(CurrentTestRootPath, obj.MasterPrototypeRecordingPath)

                    'Ensuring that the master prototype recoding exist
                    If IO.File.Exists(PrototypeRecordingPath) = False Then
                        MsgBox("The master prototype recoding cannot be found at: " & PrototypeRecordingPath, MsgBoxStyle.Information, "Lacking master prototype recording")
                        Return Nothing
                    End If

                Case PrototypeRecordingOptions.PrototypeRecordings

                    'Getting the folder
                    PrototypeRecordingPath = IO.Path.Combine(CurrentTestRootPath, obj.PrototypeMediaParentFolder, Component.GetMediaFolderName)

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
                        MsgBox("The following prototype recording folder cannot be found: " & PrototypeRecordingPath, MsgBoxStyle.Information, "Lacking prototype recording folder")
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
            For n = ExistingFileCount To obj.MediaAudioItems - 1
                'Creating a file name (avoiding file name conflicts)
                LackingFilesList.Add(New Tuple(Of String, String, SpeechMaterialComponent)(Utils.GeneralIO.CheckFileNameConflict(IO.Path.Combine(FullMediaFolderPath, Component.GetMediaFolderName & "_" & (n).ToString("000") & ".wav")), PrototypeRecordingPath, Component))
                AllPaths.Add(LackingFilesList.Last)
            Next

        Next

        Return New Tuple(Of List(Of Tuple(Of String, String, SpeechMaterialComponent)),
            List(Of Tuple(Of String, String, SpeechMaterialComponent)),
            List(Of Tuple(Of String, String, SpeechMaterialComponent)))(AllPaths, ExistingFilesList, LackingFilesList)

    End Function






    ''' <summary>
    ''' Copies all sound files to a folder structure which is based on the Id of the speech material component.
    ''' </summary>
    <Extension>
    Public Sub TemporaryFunction_CopySoundFiles(obj As STFN.Core.MediaSet, ByVal OutputFolder As String)

        Dim AllSoundPathTuples = obj.GetAllSpeechMaterialComponentAudioPaths(PrototypeRecordingOptions.None)

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


    <Extension>
    Public Sub TemporaryFunction_CopySoundFIles2(obj As STFN.Core.MediaSet, ByVal OutputFolder As String)

        Dim CurrentTestRootPath As String = obj.ParentTestSpecification.GetTestRootPath

        Dim AllComponents = obj.ParentTestSpecification.SpeechMaterial.GetAllRelatives

        For Each Component In AllComponents

            'Skips to next if no media items are expected
            If Component.LinguisticLevel <> obj.AudioFileLinguisticLevel Then Continue For

            Dim NewMediaFolderPath = IO.Path.Combine(OutputFolder, obj.MediaParentFolder, Component.GetMediaFolderName)

            Dim OldMediaFolderPath = IO.Path.Combine(CurrentTestRootPath, obj.MediaParentFolder, Component.GetMediaFolderName.Split("_")(0))

            'Creates the OutputFolder 
            If IO.Directory.Exists(NewMediaFolderPath) = False Then IO.Directory.CreateDirectory(NewMediaFolderPath)

            Dim FilesInPlace = IO.Directory.GetFiles(OldMediaFolderPath)

            For Each f In FilesInPlace
                IO.File.Copy(f, IO.Path.Combine(NewMediaFolderPath, IO.Path.GetFileName(f)))
            Next

        Next

        MsgBox("Finished copying files")

    End Sub


    <Extension>
    Public Sub TemporaryFunction_CopySoundFIles3(obj As STFN.Core.MediaSet, ByVal OutputFolder As String)

        Dim CurrentTestRootPath As String = obj.ParentTestSpecification.GetTestRootPath

        Dim AllComponents = obj.ParentTestSpecification.SpeechMaterial.GetAllRelatives

        For Each Component In AllComponents

            'Skips to next if no media items are expected
            If Component.LinguisticLevel <> obj.AudioFileLinguisticLevel Then Continue For

            Dim PrototypeMediaFolderPath = IO.Path.Combine(OutputFolder, obj.MediaParentFolder, Component.GetMediaFolderName)

            Dim PrototypeSoundPath = IO.Path.Combine("C:\OSTF\Tests\SwedishSiPTest\Media\PreQueTalker1-RVE\TestWordRecordings", "SampleRec_" & Component.GetCategoricalVariableValue("Spelling") & ".wav")

            'Creates the OutputFolder 
            If IO.Directory.Exists(PrototypeMediaFolderPath) = False Then IO.Directory.CreateDirectory(PrototypeMediaFolderPath)

            IO.File.Copy(PrototypeSoundPath, IO.Path.Combine(PrototypeMediaFolderPath, Component.GetMediaFolderName & ".wav"))

        Next

        MsgBox("Finished copying files")

    End Sub

    <Extension>
    Public Sub TemporaryFunction_RenameMaskerFolder(obj As STFN.Core.MediaSet, )

        'Clears previously loaded sounds
        Dim MaskerComponents = obj.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(Me.SharedMaskersLevel)

        For Each MaskerComponent In MaskerComponents

            Dim CurrentTestRootPath As String = obj.ParentTestSpecification.GetTestRootPath

            Dim OldMaskerFolderPath = IO.Path.Combine(CurrentTestRootPath, obj.MaskerParentFolder, MaskerComponent.PrimaryStringRepresentation)
            Dim NewMaskerFolderPath = IO.Path.Combine(CurrentTestRootPath, obj.MaskerParentFolder, MaskerComponent.GetMediaFolderName)

            IO.Directory.Move(OldMaskerFolderPath, NewMaskerFolderPath)

        Next

        MsgBox("Folder renaming is completed.")

    End Sub


End Module

