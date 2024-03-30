Imports System.IO



Namespace Audio

    Namespace AudioIOs

        Public Module AudioIOExt

            ''' <summary>
            ''' Launches an open file dialog and asks the user for a sound file.
            ''' </summary>
            ''' <param name="directory"></param>
            ''' <param name="fileName"></param>
            ''' <returns>Returns the path to the indicated sound file.</returns>
            Public Async Function OpenSoundFileDialog(Optional ByVal directory As String = "", Optional ByVal fileName As String = "") As Task(Of String)

                Return Await Messager.OpenSoundFileDialog(directory, fileName)

                'The iOstfGui function should do the following

                '                Dim filePath As String = ""

                'SavingFile:     Dim ofd As New OpenFileDialog
                '                Dim filter As String = "Wave Files (*.wav)|*.wav|Ptwf Files (*.ptwf)|*.ptwf"
                '                ofd.Filter = filter
                '                If Not directory = "" Then ofd.InitialDirectory = directory
                '                If Not fileName = "" Then ofd.FileName = fileName
                '                ofd.Title = "Please select an audio file..."
                '                Dim result As DialogResult = ofd.ShowDialog()
                '                If result = DialogResult.OK Then
                '                    filePath = ofd.FileName
                '                Else
                '                    Dim boxResult As MsgBoxResult = MsgBox("An error occurred choosing file name.", MsgBoxStyle.RetryCancel, "Warning!")
                '                    If boxResult = MsgBoxResult.Retry Then
                '                        GoTo SavingFile
                '                    End If
                '                    If boxResult = MsgBoxResult.Cancel Then
                '                        Return Nothing
                '                    End If
                '                End If

                '                Return filePath

            End Function

            ''' <summary>
            ''' Launches a save file dialog and asks the user for a file path.
            ''' </summary>
            ''' <param name="directory"></param>
            ''' <param name="fileName"></param>
            ''' <param name="fileFormat"></param>
            ''' <returns>The file path to which the sound file should be written.</returns>
            Public Function SaveSoundFileDialog(Optional ByRef directory As String = "", Optional ByRef fileName As String = "",
                                            Optional ByVal fileFormat As SoundFileFormats = SoundFileFormats.wav) As String


                Return Messager.SaveSoundFileDialog(directory, fileName, fileFormat).Result

                'The OstfGui.SaveSoundFileDialog shold do the following:
                '                Dim filePath As String = ""

                'SavingFile:     Dim sfd As New SaveFileDialog
                '                'Saving project file
                '                Dim filter As String = "Wave Files (*.wav)|*.wav|Ptwf Files (*.ptwf)|*.ptwf"
                '                sfd.Filter = filter
                '                If directory <> "" Then
                '                    sfd.InitialDirectory = directory
                '                End If
                '                If fileName <> "" Then
                '                    sfd.FileName = fileName
                '                Else
                '                    'Setting the filename stored in the sound object to default
                '                    sfd.FileName = fileName
                '                End If
                '                Select Case fileFormat
                '                    Case SoundFileFormats.wav
                '                        sfd.FilterIndex = 1
                '                    Case SoundFileFormats.ptwf
                '                        sfd.FilterIndex = 2
                '                End Select

                '                Dim result As DialogResult = sfd.ShowDialog()
                '                If result = DialogResult.OK Then
                '                    filePath = sfd.FileName
                '                    Dim chosenFormat As String = Path.GetExtension(filePath)
                '                    Select Case chosenFormat
                '                        Case ".wav"
                '                            fileFormat = SoundFileFormats.wav
                '                        Case ".ptwf"
                '                            fileFormat = SoundFileFormats.ptwf
                '                        Case Else
                '                            'using wave as default (this would never occur unless the filter above is changed)
                '                            fileFormat = SoundFileFormats.wav
                '                    End Select
                '                Else
                '                    Dim errorSaving As MsgBoxResult = MsgBox("An error occurred choosing file name.", MsgBoxStyle.RetryCancel, "Warning!")
                '                    If errorSaving = MsgBoxResult.Retry Then
                '                        GoTo SavingFile
                '                    End If
                '                    If errorSaving = MsgBoxResult.Cancel Then
                '                        Return Nothing
                '                    End If
                '                End If

                '                Return filePath

            End Function

            ''' <summary>
            ''' Reads a sound (.wav or .ptwf) from file and stores it in a new Sounds object.
            ''' </summary>
            ''' <param name="filePath">The file path to the file to read. If left empty a open file dialogue box will appear.</param>
            ''' <param name="startReadTime"></param>
            ''' <param name="stopReadTime"></param>
            ''' <param name="inputTimeFormat"></param>
            ''' <param name="directory">The initial directory in the open file dialogue box.</param>
            ''' <param name="fileName">The default filenema in the open file dialogue box.</param>
            ''' <returns>Returns a new Sound containing the sound data from the input sound file.</returns>
            Public Function ReadWaveFile(Optional ByVal filePath As String = "", Optional ByVal directory As String = "", Optional ByVal fileName As String = "") As Sound

                If filePath = "" Then
                    filePath = OpenSoundFileDialog(directory, fileName).Result
                End If

                If filePath = "" Then
                    Return Nothing
                End If

                Select Case Path.GetExtension(filePath)
                    Case ".wav"
                        Return Sound.LoadWaveFile(filePath)

                    Case ".ptwf"
                        Return AudioIOs.LegacyMethods.LoadPtwfFile(filePath)

                    Case Else
                        AudioError("The format of the selected file is not supported!")
                        Return Nothing
                End Select


            End Function


            ''' <summary>
            ''' Saves the current instance of Sound to a wave file.
            ''' </summary>
            ''' <param name="sound">The sound to be saved.</param>
            ''' <param name="filePath">The filepath where the file should be saved. If left empty a windows forms save file dialogue box will appaear,
            ''' in which the user may enter file name and storing location.</param>
            ''' <param name="startSample">This parameter enables saving of only a part of the file. StartSample indicates the first sample to be saved.</param>
            ''' <param name="length">This parameter enables saving of only a part of the file. Length indicates the length in samples of the file to be saved.</param>
            ''' <returns>Returns true if save succeded, and Flase if save failed.</returns>
            Public Function SaveToWaveFile(ByRef sound As Sound, Optional ByRef filePath As String = "",
                                   Optional ByVal startSample As Integer = Nothing, Optional ByVal length As Integer? = Nothing,
                                       Optional directory As String = "", Optional fileName As String = "",
                                       Optional CreatePath As Boolean = True) As Boolean


                If filePath = "" Then
                    filePath = SaveSoundFileDialog(directory, fileName, SoundFileFormats.wav)
                End If

                Dim ErrorMessage As String = ""

                Return sound.WriteWaveFile(filePath, startSample, length, CreatePath)

            End Function





            ''' <summary>
            ''' Reads all wavefiles in the input folder, and copies them to the output folder, and splitting files longer than MaxDuration to sections of MaxDuration seconds.
            ''' </summary>
            ''' <param name="InputFolder"></param>
            ''' <param name="OutputFolder"></param>
            ''' <param name="MaxDuration"></param>
            Public Sub SplitLongWaveFiles(ByVal InputFolder As String, ByVal OutputFolder As String, ByVal MaxDuration As Double, Optional ByVal OverlapDuration As Double = 0)

                Try

                    Dim Files As String() = Directory.GetFiles(InputFolder)

                    'Creates an output folder
                    Directory.CreateDirectory(OutputFolder)

                    For Each File In Files

                        Dim Extension As String = Path.GetExtension(File)
                        If Extension = ".ptwf" Or Extension = ".wav" Then

                            Dim InputFile = AudioIOs.ReadWaveFile(File)

                            Dim MaxLength As Integer = InputFile.WaveFormat.SampleRate * MaxDuration
                            Dim OverlapLength As Integer = InputFile.WaveFormat.SampleRate * OverlapDuration
                            Dim StepSize As Integer = MaxLength - OverlapLength
                            Dim FileLength As Integer = InputFile.WaveData.SampleData(1).Length

                            If InputFile.WaveData.SampleData(1).Length > MaxLength Then

                                'Splitting the file
                                Dim SectionCount As Integer = 1 + Utils.Rounding((FileLength - MaxLength) / StepSize, Utils.roundingMethods.alwaysUp)

                                'Copying and saving the sections
                                For n = 0 To SectionCount - 2
                                    Dim NewSound = DSP.CopySection(InputFile, StepSize * n, MaxLength) 'Reads to the end of the file
                                    AudioIOs.SaveToWaveFile(NewSound, Path.Combine(OutputFolder, Path.GetFileNameWithoutExtension(File) & "_" & n & ".wav"))

                                Next

                                'Copying the last section
                                Dim LastSound = DSP.CopySection(InputFile, StepSize * (SectionCount - 1),) 'Reads to the end of the file
                                AudioIOs.SaveToWaveFile(LastSound, Path.Combine(OutputFolder, Path.GetFileNameWithoutExtension(File) & "_" & (SectionCount - 1) & ".wav"))

                            Else

                                'Copies the file
                                FileSystem.FileCopy(File, Path.Combine(OutputFolder, Path.GetFileName(File)))

                            End If
                        End If
                    Next

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub


            '''' <summary>
            '''' Loads all soundfiles in a folder and converts them to ptwf (if not already in ptwf format), and updates their sound level measurements, using the supplied sound level format.
            '''' </summary>
            '''' <param name="UpdateMaskerSoundSections">A custom SiB-test option,</param>
            '''' <param name="Directory"></param>
            'Public Sub UpdateSoundFiles(ByVal Directory As String, IncludeSubdirectories As Boolean,
            '                        Optional ByVal SubDirectoryLevelsToInclude As Integer? = Nothing,
            '                        Optional ByRef TargetLevel_dBFS As Double? = Nothing,
            '                       Optional ByRef TargetLevel_FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z,
            '                       Optional ByRef SoundLevelFormat As Formats.SoundLevelFormat = Nothing,
            '                       Optional ByVal LogMeasurentResults As Boolean = True,
            '                       Optional ByVal CreateBackUps As Boolean = True,
            '                       Optional ByVal UpdateMaskerSoundSections As Boolean = False,
            '                       Optional ByVal MaskerSoundCentralSectionStartTime As Double = 1.25,
            '                       Optional ByVal MaskerSoundCentralSectionDuration As Double = 0.5)

            '    Try
            '        If SoundLevelFormat Is Nothing Then
            '            SoundLevelFormat = New Formats.SoundLevelFormat(SoundMeasurementTypes.Average_C_Weighted)
            '            SoundLevelFormat.SelectFormatWithGui()
            '        End If

            '        Dim FilePaths As String()
            '        If IncludeSubdirectories = False Then
            '            FilePaths = IO.Directory.GetFiles(Directory)
            '        Else
            '            If SubDirectoryLevelsToInclude Is Nothing Then
            '                FilePaths = Utils.GetFilesIncludingAllSubdirectories(Directory)
            '            Else
            '                FilePaths = Utils.GetFilesIncludingAllSubdirectories(Directory, SubDirectoryLevelsToInclude)
            '            End If
            '        End If

            '        For Each FilePath In FilePaths
            '            UpdateSoundFile(FilePath, TargetLevel_dBFS, TargetLevel_FrequencyWeighting, SoundLevelFormat, LogMeasurentResults, CreateBackUps,
            '                        UpdateMaskerSoundSections, MaskerSoundCentralSectionStartTime, MaskerSoundCentralSectionDuration)
            '        Next

            '    Catch ex As Exception
            '        MsgBox(ex.ToString)
            '    End Try

            'End Sub

            Public Sub UpdateSoundFile(ByVal FilePath As String,
                                   Optional ByRef TargetLevel_dBFS As Double? = Nothing,
                                   Optional ByRef TargetLevel_FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z,
                                   Optional ByRef SoundLevelFormat As Formats.SoundLevelFormat = Nothing,
                                   Optional ByVal LogMeasurentResults As Boolean = True,
                                   Optional ByVal CreateBackUps As Boolean = True,
                                   Optional ByVal UpdateMaskerSoundSections As Boolean = False,
                                   Optional ByVal MaskerSoundCentralSectionStartTime As Double = 1.25,
                                   Optional ByVal MaskerSoundCentralSectionDuration As Double = 0.5)
                Try

                    'Creates a backup folder
                    Dim BackupFolder As String = Path.Combine(Path.GetDirectoryName(FilePath), "BackUp")
                    If CreateBackUps = True Then IO.Directory.CreateDirectory(BackupFolder)

                    Dim Extension As String = Path.GetExtension(FilePath)
                    'Ignoring any files which are not .ptwf or .wav
                    If Extension = ".ptwf" Or Extension = ".wav" Then

                        'Creates a backup copy
                        If CreateBackUps = True Then FileSystem.FileCopy(FilePath, Path.Combine(BackupFolder, Path.GetFileName(FilePath)))

                        'Reads the file
                        Dim InputSound As Sound = AudioIOs.ReadWaveFile(FilePath)

                        'Checks that reading was ok
                        If InputSound Is Nothing Then
                            MsgBox("Failed to read the file " & FilePath & vbCrLf & "The program will terminate!")
                            SendInfoToAudioLog("Failed to read the file " & FilePath & vbCrLf & "The program will terminate!")
                            Exit Sub
                        End If

                        'Removes the original file if it was a .wav file
                        IO.File.Delete(FilePath)

                        'Setting the averagelevel to TargetLevel_dBFS
                        If TargetLevel_dBFS.HasValue Then
                            DSP.MeasureAndAdjustSectionLevel(InputSound, TargetLevel_dBFS,,,, TargetLevel_FrequencyWeighting)
                        End If

                        'SMA sentence
                        Dim sentence As Integer = 0

                        'Update masker sound sections as "word" data
                        If UpdateMaskerSoundSections = True Then

                            'Clearing any previous data
                            InputSound.SMA.ChannelData(1)(sentence).Clear()

                            Dim MeasurementRegionStartSample As Integer = MaskerSoundCentralSectionStartTime * InputSound.WaveFormat.SampleRate
                            Dim MeasurementRegionLength As Integer = MaskerSoundCentralSectionDuration * InputSound.WaveFormat.SampleRate

                            'Supplying the SoundSection sound with segmentation data, on sentence level, as well as a the following word level segmentations 
                            '(word1: pre-measurement section, word2: measurement section, word 3: post measurement section)
                            InputSound.SMA.ChannelData(1)(sentence).StartSample = 0
                            InputSound.SMA.ChannelData(1)(sentence).Length = InputSound.WaveData.SampleData(1).Length
                            InputSound.SMA.ChannelData(1)(sentence).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(InputSound.SMA, Sound.SpeechMaterialAnnotation.SmaTags.WORD, InputSound.SMA.ChannelData(1)(sentence)) With {.StartSample = 0, .Length = Math.Max(0, MeasurementRegionStartSample), .OrthographicForm = "", .PhoneticForm = ""})
                            InputSound.SMA.ChannelData(1)(sentence).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(InputSound.SMA, Sound.SpeechMaterialAnnotation.SmaTags.WORD, InputSound.SMA.ChannelData(1)(sentence)) With {.StartSample = MeasurementRegionStartSample, .Length = MeasurementRegionLength, .OrthographicForm = "", .PhoneticForm = ""})
                            InputSound.SMA.ChannelData(1)(sentence).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(InputSound.SMA, Sound.SpeechMaterialAnnotation.SmaTags.WORD, InputSound.SMA.ChannelData(1)(sentence)) With {.StartSample = MeasurementRegionStartSample + MeasurementRegionLength, .Length = InputSound.WaveData.SampleData(1).Length - (MeasurementRegionStartSample + MeasurementRegionLength), .OrthographicForm = "", .PhoneticForm = ""})
                        End If

                        'Sets the SoundLevelFormat
                        InputSound.SMA.SetFrequencyWeighting(SoundLevelFormat.FrequencyWeighting, True)
                        InputSound.SMA.SetTimeWeighting(SoundLevelFormat.TemporalIntegrationDuration, True)

                        'Measures sound levels
                        InputSound.SMA.MeasureSoundLevels(False, LogMeasurentResults)

                        'Saves the file with the original file name, but always in .ptwf format
                        AudioIOs.SaveToWaveFile(InputSound, Path.Combine(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath)))

                    End If

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub


            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="InputFolder"></param>
            ''' <param name="OutputFolder"></param>
            ''' <param name="TargetWaveFormat">Only the wave format fieds SampleRate, BitDepth and Encoding are used, all other are copied from the input files.</param>
            Public Sub SamplerateConversionBatch(ByVal InputFolder As String, ByVal OutputFolder As String, ByVal TargetWaveFormat As Formats.WaveFormat)


                'Collecting al files in fileEntries
                Dim FilePaths As String() = Directory.GetFiles(InputFolder)

                'Creating the output folder
                Directory.CreateDirectory(OutputFolder)

                For Each CurrentPath In FilePaths

                    Dim CurrentFormat As SoundFileFormats
                    Dim CurrentExtension As String = IO.Path.GetExtension(CurrentPath)
                    Select Case CurrentExtension
                        Case ".wav"
                            CurrentFormat = SoundFileFormats.wav
                        Case ".ptwf"
                            CurrentFormat = SoundFileFormats.ptwf
                        Case Else
                            'Skipping the file if it's not a .wav or .ptwf file
                            Continue For
                    End Select

                    'Reading the file
                    Dim InputSound = AudioIOs.ReadWaveFile(CurrentPath)

                    'Resamples the sound
                    Dim ResamplesSound = DSP.Resample_UsingResampAudio(InputSound, TargetWaveFormat)

                    'Saving the sound to the output folder
                    AudioIOs.SaveToWaveFile(ResamplesSound, Path.Combine(OutputFolder, InputSound.FileName & CurrentExtension))

                Next

                MsgBox("Finished batch sample rate conversion.")

            End Sub

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="InputFolder"></param>
            ''' <param name="OutputFolder"></param>
            ''' <param name="TargetWaveFormat">Only the wave format fieds SampleRate, BitDepth and Encoding are used, all other are copied from the input files.</param>
            Public Sub SamplerateConversion_DirectBatch(ByVal InputFolder As String, ByVal OutputFolder As String, ByVal TargetWaveFormat As Formats.WaveFormat,
                                                  Optional ByVal ResampAudioPath As String = "C:\AFsp_Win\ResampAudio.exe", Optional NoExtensiveFormat As Boolean = True)


                'This function is using the ResampAudio software to do the sample rate conversion
                'The resampler used is ResampAudio from http://www-mmsp.ece.mcgill.ca/Documents/Downloads/AFsp/index.html version: AFsp-v10r0.tar.gz from 2017-07 
                'See documentation for ResampAudio at http://www-mmsp.ece.mcgill.ca/Documents/Software/Packages/AFsp/audio/html/ResampAudio.html

                'Returns nothing if ResampAudio.exe cannot be found
                If Not File.Exists(ResampAudioPath) Then
                    MsgBox("The file ResampAudio.exe cannot be found at the following specified location:" & ResampAudioPath)
                    Exit Sub
                End If

                Dim DFormat As String = ""
                Select Case TargetWaveFormat.Encoding
                    Case Formats.WaveFormat.WaveFormatEncodings.PCM
                        Select Case TargetWaveFormat.BitDepth
                            Case 16
                                DFormat = "integer16"
                            Case Else
                                Throw New NotImplementedException("Unsupported audio bit depth")
                        End Select
                    Case Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints
                        Select Case TargetWaveFormat.BitDepth
                            Case 32
                                DFormat = "float32"
                            Case Else
                                Throw New NotImplementedException("Unsupported audio bit depth")
                        End Select
                    Case Else
                        Throw New NotImplementedException("Unsupported audio encoding")
                End Select


                'Collecting all files in fileEntries
                Dim FilePaths As String() = Directory.GetFiles(InputFolder)

                'Creating the output folder
                Directory.CreateDirectory(OutputFolder)

                For Each CurrentPath In FilePaths

                    Dim CurrentFormat As SoundFileFormats
                    Dim CurrentExtension As String = IO.Path.GetExtension(CurrentPath)
                    Select Case CurrentExtension
                        Case ".wav"
                            CurrentFormat = SoundFileFormats.wav
                        Case ".ptwf"
                            CurrentFormat = SoundFileFormats.ptwf
                        Case Else
                            'Skipping the file if it's not a .wav or .ptwf file
                            Continue For
                    End Select

                    Dim CurrentFileName As String = IO.Path.GetFileName(CurrentPath)

                    Dim FTYPE As String
                    If NoExtensiveFormat = True Then
                        FTYPE = "WAVE-NOEX"
                    Else
                        FTYPE = "WAVE"
                    End If

                    'Creating resampled file
                    Dim ResampSigStartInfo As New ProcessStartInfo()
                    ResampSigStartInfo.FileName = ResampAudioPath
                    ResampSigStartInfo.Arguments = "-s " & TargetWaveFormat.SampleRate.ToString & " -F " & FTYPE & " -D " & DFormat & " " & Chr(34) & CurrentPath & Chr(34) & " " & Chr(34) & Path.Combine(OutputFolder, CurrentFileName) & Chr(34)
                    'ResampSigStartInfo.WorkingDirectory = WorkFolder
                    Dim sp = Process.Start(ResampSigStartInfo)
                    sp.WaitForExit()
                    sp.Close()

                Next

                MsgBox("Finished batch sample rate conversion.")

            End Sub


            ''' <summary>
            ''' Reads a wave file stream (.wav or .ptwf) and stores it in a new Sounds object.
            ''' </summary>
            ''' <param name="startReadTime"></param>
            ''' <param name="stopReadTime"></param>
            ''' <param name="inputTimeFormat"></param>
            ''' <returns>Returns a new Sound containing the sound data from the input sound file.</returns>
            Public Function ReadWaveFileStream(ByRef Stream As UnmanagedMemoryStream,
                                           Optional ByVal startReadTime As Decimal = 0, Optional ByVal stopReadTime As Decimal = 0,
                                           Optional ByVal inputTimeFormat As TimeUnits = TimeUnits.seconds,
                                           Optional ByVal DefaultPtwfTimeWeighting As Double = 0.1) As Sound

                Try

                    'Creates a variable to hold data chunk size
                    Dim dataSize As UInteger = 0

                    'Resets stream position
                    Stream.Position = 0

                    Dim reader As BinaryReader = New BinaryReader(Stream, Text.Encoding.UTF8)

                    Dim chunkID As String = reader.ReadChars(4)
                    Dim fileSize As UInteger = reader.ReadUInt32
                    Dim riffType As String = reader.ReadChars(4)
                    'Abort if riffType is not WAVE
                    If Not riffType = "WAVE" Then
                        Throw New Exception("The file is not a wave-file!")
                    End If

                    Dim fmtID As String
                    Dim fmtSize As UInteger
                    Dim fmtCode As UShort
                    Dim channels As UShort
                    Dim sampleRate As UInteger
                    Dim fmtAvgBPS As UInteger
                    Dim fmtBlockAlign As UShort
                    Dim bitDepth As UShort

                    Dim sound As Sound = Nothing
                    Dim FormatChunkIsRead As Boolean = False 'THis variable is used to ensure that the format chunk is read before the ptwf and the data chunks.

                    'Chunks to ignore
                    Dim dataChunkFound As Boolean
                    While dataChunkFound = False

                        Dim IDOfNextChunk As String = reader.ReadChars(4)
                        Dim sizeOfNextChunk As UInteger = reader.ReadUInt32
                        Select Case IDOfNextChunk

                            Case "fmt "

                                Dim fmtChunkStartPosition As Integer = reader.BaseStream.Position

                                ' Reading the format chunk (not all data is stored)
                                fmtID = IDOfNextChunk ' reader.ReadChars(4)
                                fmtSize = sizeOfNextChunk ' reader.ReadUInt32
                                fmtCode = reader.ReadUInt16
                                channels = reader.ReadUInt16
                                sampleRate = reader.ReadUInt32
                                fmtAvgBPS = reader.ReadUInt32
                                fmtBlockAlign = reader.ReadUInt16
                                bitDepth = reader.ReadUInt16

                                sound = New Sound(New Formats.WaveFormat(sampleRate, bitDepth, channels,, fmtCode))

                                'Checks to see if the whole of subchunk1 has been read
                                While reader.BaseStream.Position < fmtChunkStartPosition + fmtSize
                                    reader.ReadByte()
                                End While

                                'Noting that the format chunk is read
                                FormatChunkIsRead = True

                            Case "iXML"

                                Dim iXMLDataStartReadPosition As Integer = reader.BaseStream.Position

                                'Copying iXML data to a new stream
                                Dim iXMLStream As New MemoryStream
                                For s = 0 To sizeOfNextChunk - 1
                                    iXMLStream.WriteByte(Stream.ReadByte)
                                Next
                                iXMLStream.Position = 0

                                'Parsing the iXML data
                                Dim iXMLdata = Sound.ParseiXMLString(iXMLStream)

                                'Storing the data
                                If iXMLdata.Item1 IsNot Nothing Then
                                    sound.SMA = iXMLdata.Item1
                                End If
                                If iXMLdata.Item2 IsNot Nothing Then
                                    sound.iXmlNodes = iXMLdata.Item2
                                End If

                                'Checks if a padding byte needs to be read
                                Dim currentBaseStreamPosition As Integer = reader.BaseStream.Position
                                If Not currentBaseStreamPosition Mod 2 = 0 Then
                                    reader.ReadByte()
                                End If

                            Case "ptwf"

                                Dim DefaultNotMeasuredValue As Double = -999999 ' This is the default value used with the previous PTWF version,for sound level that have not been measured

                                'Aborting if the format chink has not yet been read
                                If FormatChunkIsRead = False Then
                                    AudioError("The wave file has an unsupported internal structure.")
                                    Return Nothing
                                End If

                                Dim ptwfDataStartReadPosition As Integer = reader.BaseStream.Position

                                'read the ptwf chunk
                                Dim ptwfID = IDOfNextChunk
                                Dim ptwfSize = sizeOfNextChunk

                                'Just skips storing the version read, as no more ptwf versions will be created... Thus version 0 indicates that the SMA data was read from a PTWF file.
                                sound.SMA.ReadFromVersion = "0"
                                Dim ReadVersion = reader.ReadUInt32()

                                Dim SegmentationDataEncoding = reader.ReadUInt32
                                'sound.SMA.SegmentationDataEncoding = reader.ReadUInt32

                                Dim SMA_ChannelCount As UInteger = reader.ReadUInt32
                                'sound.SMA.ChannelCount = reader.ReadUInt32

                                Dim soundLevelMeasurementFormat As New Formats.SoundLevelFormat(reader.ReadUInt32) 'Creating a deafault SoundLevelFormat. (Future versions of ptwf could add parameters here) Is this working?
                                'The soundLevelMeasurementFormat is set while reading all levels below

                                Dim TempChannelSpecific As UInteger = reader.ReadUInt32

                                For channel As Integer = 1 To SMA_ChannelCount 'sound.SMA.ChannelCount

                                    'The previous version of SMA used only one sentence per channel. Therefore only sentence index 0 is used here! (Instead of a sentence loop)
                                    'For sentence As Integer = 0 To ChannelData(channel).Count - 1
                                    Dim sentence As Integer = 0

                                    sound.SMA.ChannelData(channel)(sentence).StartSample = reader.ReadInt32
                                    sound.SMA.ChannelData(channel)(sentence).Length = reader.ReadInt32

                                    'Changing the previously used default not-measured value (-999999) to Nothing
                                    Dim suwl As Double = reader.ReadDouble
                                    If suwl = DefaultNotMeasuredValue Then
                                        sound.SMA.ChannelData(channel)(sentence).UnWeightedLevel = Nothing
                                    Else
                                        sound.SMA.ChannelData(channel)(sentence).UnWeightedLevel = suwl
                                    End If

                                    Dim spkl As Double = reader.ReadDouble
                                    If spkl = DefaultNotMeasuredValue Then
                                        sound.SMA.ChannelData(channel)(sentence).UnWeightedPeakLevel = Nothing
                                    Else
                                        sound.SMA.ChannelData(channel)(sentence).UnWeightedPeakLevel = spkl
                                    End If

                                    Dim swl As Double = reader.ReadDouble
                                    If swl = DefaultNotMeasuredValue Then
                                        sound.SMA.ChannelData(channel)(sentence).WeightedLevel = Nothing
                                    Else
                                        sound.SMA.ChannelData(channel)(sentence).WeightedLevel = swl
                                    End If

                                    sound.SMA.ChannelData(channel)(sentence).InitialPeak = reader.ReadDouble
                                    sound.SMA.ChannelData(channel)(sentence).StartTime = reader.ReadDouble
                                    Dim currentWordCount As UInteger = reader.ReadUInt32 'sound.SMA.WordCount = reader.ReadUInt32

                                    'Adding data described in the new SMA format, for both the top, channel and sentence levels
                                    sound.SMA.SetFrequencyWeighting(soundLevelMeasurementFormat.FrequencyWeighting, False)
                                    sound.SMA.ChannelData(channel).SetFrequencyWeighting(soundLevelMeasurementFormat.FrequencyWeighting, False)
                                    sound.SMA.ChannelData(channel)(sentence).SetFrequencyWeighting(soundLevelMeasurementFormat.FrequencyWeighting, False)
                                    If soundLevelMeasurementFormat.LoudestSectionMeasurement = True Then
                                        sound.SMA.SetTimeWeighting(DefaultPtwfTimeWeighting, False)
                                        sound.SMA.ChannelData(channel).SetTimeWeighting(DefaultPtwfTimeWeighting, False)
                                        sound.SMA.ChannelData(channel)(sentence).SetTimeWeighting(DefaultPtwfTimeWeighting, False)
                                    Else
                                        sound.SMA.SetTimeWeighting(0, False)
                                        sound.SMA.ChannelData(channel).SetTimeWeighting(0, False)
                                        sound.SMA.ChannelData(channel)(sentence).SetTimeWeighting(0, False)
                                    End If

                                    'Sound levels were typically not stored for the whole channel but the sentence level is stored here anyway, as there were only one sentnce per channel in the old version
                                    sound.SMA.ChannelData(channel).UnWeightedLevel = sound.SMA.ChannelData(channel)(sentence).UnWeightedLevel
                                    sound.SMA.ChannelData(channel).UnWeightedPeakLevel = sound.SMA.ChannelData(channel)(sentence).UnWeightedPeakLevel
                                    sound.SMA.ChannelData(channel).WeightedLevel = sound.SMA.ChannelData(channel)(sentence).WeightedLevel

                                    'Word level data
                                    For word = 0 To currentWordCount - 1

                                        sound.SMA.ChannelData(channel)(sentence).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(sound.SMA, Sound.SpeechMaterialAnnotation.SmaTags.WORD, sound.SMA.ChannelData(channel)(sentence)))

                                        Dim OrthographicFormLength As Integer = reader.ReadUInt32
                                        sound.SMA.ChannelData(channel)(sentence)(word).OrthographicForm = reader.ReadChars(OrthographicFormLength)
                                        Dim OrthographicFormBLength As Integer = reader.ReadUInt32
                                        sound.SMA.ChannelData(channel)(sentence)(word).PhoneticForm = reader.ReadChars(OrthographicFormBLength)
                                        sound.SMA.ChannelData(channel)(sentence)(word).StartSample = reader.ReadInt32
                                        sound.SMA.ChannelData(channel)(sentence)(word).Length = reader.ReadInt32

                                        'Changing the previously used default not-measured value (-999999) to Nothing
                                        Dim wuwl As Double = reader.ReadDouble
                                        If wuwl = DefaultNotMeasuredValue Then
                                            sound.SMA.ChannelData(channel)(sentence)(word).UnWeightedLevel = Nothing
                                        Else
                                            sound.SMA.ChannelData(channel)(sentence)(word).UnWeightedLevel = wuwl
                                        End If

                                        Dim wpl As Double = reader.ReadDouble
                                        If wpl = DefaultNotMeasuredValue Then
                                            sound.SMA.ChannelData(channel)(sentence)(word).UnWeightedPeakLevel = Nothing
                                        Else
                                            sound.SMA.ChannelData(channel)(sentence)(word).UnWeightedPeakLevel = wpl
                                        End If

                                        Dim wwl As Double = reader.ReadDouble
                                        If wwl = DefaultNotMeasuredValue Then
                                            sound.SMA.ChannelData(channel)(sentence)(word).WeightedLevel = Nothing
                                        Else
                                            sound.SMA.ChannelData(channel)(sentence)(word).WeightedLevel = wwl
                                        End If

                                        sound.SMA.ChannelData(channel)(sentence)(word).StartTime = reader.ReadDouble

                                        Dim phoneCount As Integer = reader.ReadUInt32
                                        Dim phoneListLength As Integer = reader.ReadUInt32

                                        'Adding data described in the new SMA format
                                        sound.SMA.ChannelData(channel)(sentence)(word).SetFrequencyWeighting(soundLevelMeasurementFormat.FrequencyWeighting, False)
                                        If soundLevelMeasurementFormat.LoudestSectionMeasurement = True Then
                                            sound.SMA.ChannelData(channel)(sentence)(word).SetTimeWeighting(DefaultPtwfTimeWeighting, False)
                                        Else
                                            sound.SMA.ChannelData(channel)(sentence)(word).SetTimeWeighting(0, False)
                                        End If

                                        'Phone level data
                                        For phone = 0 To phoneListLength - 1
                                            sound.SMA.ChannelData(channel)(sentence)(word).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(sound.SMA, Sound.SpeechMaterialAnnotation.SmaTags.PHONE, sound.SMA.ChannelData(channel)(sentence)(word)))

                                            Dim phoneticTranscription As String = reader.ReadChars(10)
                                            sound.SMA.ChannelData(channel)(sentence)(word)(phone).PhoneticForm = phoneticTranscription.Trim(" ")
                                            sound.SMA.ChannelData(channel)(sentence)(word)(phone).StartSample = reader.ReadInt32
                                            sound.SMA.ChannelData(channel)(sentence)(word)(phone).Length = reader.ReadInt32

                                            'Changing the previously used default not-measured value (-999999) to Nothing
                                            Dim puwl As Double = reader.ReadDouble
                                            If puwl = DefaultNotMeasuredValue Then
                                                sound.SMA.ChannelData(channel)(sentence)(word)(phone).UnWeightedLevel = Nothing
                                            Else
                                                sound.SMA.ChannelData(channel)(sentence)(word)(phone).UnWeightedLevel = puwl
                                            End If

                                            Dim ppl As Double = reader.ReadDouble
                                            If ppl = DefaultNotMeasuredValue Then
                                                sound.SMA.ChannelData(channel)(sentence)(word)(phone).UnWeightedPeakLevel = Nothing
                                            Else
                                                sound.SMA.ChannelData(channel)(sentence)(word)(phone).UnWeightedPeakLevel = ppl
                                            End If

                                            Dim pwl As Double = reader.ReadDouble
                                            If pwl = DefaultNotMeasuredValue Then
                                                sound.SMA.ChannelData(channel)(sentence)(word)(phone).WeightedLevel = Nothing
                                            Else
                                                sound.SMA.ChannelData(channel)(sentence)(word)(phone).WeightedLevel = pwl
                                            End If

                                            'Adding data described in the new SMA format
                                            sound.SMA.ChannelData(channel)(sentence)(word)(phone).FrequencyWeighting = soundLevelMeasurementFormat.FrequencyWeighting
                                            If soundLevelMeasurementFormat.LoudestSectionMeasurement = True Then
                                                sound.SMA.ChannelData(channel)(sentence)(word)(phone).TimeWeighting = DefaultPtwfTimeWeighting
                                            Else
                                                sound.SMA.ChannelData(channel)(sentence)(word)(phone).TimeWeighting = 0
                                            End If
                                        Next
                                    Next
                                    'Next
                                Next

                                'Make sure that reader has finished reading the chunk, including any zero-padding bytes
                                Dim currentReaderPosition As Integer = reader.BaseStream.Position
                                Dim paddingBytesToRead As Integer = ptwfSize - (currentReaderPosition - ptwfDataStartReadPosition)
                                reader.ReadBytes(paddingBytesToRead)

                            'continue reading and storing phone data

                            Case "data"
                                dataChunkFound = True
                                dataSize = sizeOfNextChunk

                            Case Else
                                Dim SizeOfUnknownChunk As UInteger = sizeOfNextChunk

                                'Reads to the end of the chunk but does not save the data
                                Dim SCUPadding As Boolean
                                If SizeOfUnknownChunk Mod 2 = 1 Then
                                    SCUPadding = True
                                End If

                                'MsgBox(SizeOfUnknownChunk)

                                reader.ReadBytes(SizeOfUnknownChunk)
                                If SCUPadding = True Then
                                    reader.ReadByte()
                                End If
                        End Select

                    End While



                    Dim startReadDataPoint As Integer
                    Dim stopReadDataPoint As Integer

                    Select Case inputTimeFormat
                        Case TimeUnits.seconds
                            startReadDataPoint = startReadTime * sound.WaveFormat.SampleRate * sound.WaveFormat.Channels
                            stopReadDataPoint = stopReadTime * sound.WaveFormat.SampleRate * sound.WaveFormat.Channels

                        Case TimeUnits.samples
                            startReadDataPoint = startReadTime * sound.WaveFormat.Channels
                            stopReadDataPoint = stopReadTime * sound.WaveFormat.Channels

                    End Select

                    Dim soundIndexOfDataPoints As Integer = dataSize / (sound.WaveFormat.BitDepth / 8)

                    If stopReadTime = 0 Then
                        stopReadDataPoint = soundIndexOfDataPoints - 1
                    End If

                    If stopReadDataPoint > soundIndexOfDataPoints Then
                        stopReadDataPoint = soundIndexOfDataPoints - 1
                    End If

                    Dim numberOfDataPointsToRead As Integer = stopReadDataPoint + 1 - startReadDataPoint
                    Dim soundDataArray(numberOfDataPointsToRead - 1) As Double

                    If numberOfDataPointsToRead > 0 Then
                        Select Case sound.WaveFormat.Encoding
                            Case = Formats.WaveFormat.WaveFormatEncodings.PCM
                                Select Case sound.WaveFormat.BitDepth
                                    Case 16
                                        For n = 0 To startReadDataPoint - 1
                                            reader.ReadInt16()
                                        Next
                                        For n = startReadDataPoint To stopReadDataPoint '- 1?
                                            soundDataArray(n - startReadDataPoint) = reader.ReadInt16()
                                            'MsgBox("Reading" & n - startReadDataPoint & " " & soundDataArray(n - startReadDataPoint))
                                        Next
                                    Case Else
                                        Throw New NotImplementedException("Reading of " & sound.WaveFormat.BitDepth & " bits PCM format is not yet supported.")
                                End Select
                            Case = Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints
                                Select Case sound.WaveFormat.BitDepth
                                    Case 32
                                        For n = 0 To startReadDataPoint - 1
                                            reader.ReadSingle()
                                        Next
                                        For n = startReadDataPoint To stopReadDataPoint '- 1?
                                            soundDataArray(n - startReadDataPoint) = reader.ReadSingle()
                                            'MsgBox("Reading" & n - startReadDataPoint & " " & soundDataArray(n - startReadDataPoint))
                                        Next
                                    Case Else
                                        Throw New NotImplementedException("Reading of " & sound.WaveFormat.BitDepth & " bits IEEE floating points format is not yet supported.")
                                End Select
                        End Select

                    Else
                        If numberOfDataPointsToRead < 0 Then Throw New Exception("The number of data points to read was below zero.")
                    End If

                    'Resets stream position
                    Stream.Position = 0

                    'Dim  As Integer = sound.waveFormat.channels
                    If Not numberOfDataPointsToRead Mod channels = 0 Then Throw New Exception("ReadWaveFile detected unequal number of samples between the channels.")
                    Dim numberofDataPointsIneachChannelarray = (numberOfDataPointsToRead / channels)

                    For c = 1 To channels
                        Dim channelData((numberofDataPointsIneachChannelarray) - 1) As Single

                        If numberOfDataPointsToRead > channels Then
                            Dim counter As Integer = 0
                            For n = c - 1 To soundDataArray.Length - 1 Step channels
                                channelData(counter) = soundDataArray(n)
                                'MsgBox("Sorting channel " & c & counter & " " & channelData(counter))
                                counter += 1
                            Next
                        Else
                            If numberOfDataPointsToRead < 0 Then Throw New Exception("The number of data points to read was below zero.")
                        End If

                        sound.WaveData.SampleData(c) = channelData

                    Next


                    Return sound

                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try

            End Function

        End Module

    End Namespace

End Namespace