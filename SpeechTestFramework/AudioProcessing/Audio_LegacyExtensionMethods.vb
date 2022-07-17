Imports System.IO

Namespace Audio

    Namespace AudioIOs

        Public Module LegacyMethods


            ''' <summary>
            ''' Reads a PTWF sound from file and stores it in a new Sound object.
            ''' </summary>
            ''' <param name="filePath">The file path to the file to read. If left empty a open file dialogue box will appear.</param>
            ''' <param name="startReadTime"></param>
            ''' <param name="stopReadTime"></param>
            ''' <param name="inputTimeFormat"></param>
            ''' <param name="DefaultTimeWeighting">This is the default Time weighting used with the PTWF files, and would probably be used with most files. BUT NOT ALL!</param>
            ''' <returns>Returns a new Sound containing the sound data from the input sound file.</returns>
            Public Function LoadPtwfFile(ByVal filePath As String,
                           Optional ByVal startReadTime As Decimal = 0, Optional ByVal stopReadTime As Decimal = 0,
                           Optional ByVal inputTimeFormat As TimeUnits = TimeUnits.seconds,
                                     Optional ByVal DefaultTimeWeighting As Double = 0.1,
                                     Optional ByVal TrimWordEndStrings As Boolean = True) As Audio.Sound

                Try

                    Dim WordEndString As String = "Word end"

                    Dim fileName As String = ""

                    'Finds out the filename
                    If Not filePath = "" Then fileName = Path.GetFileNameWithoutExtension(filePath)

                    'Creates a variable to hold data chunk size
                    Dim dataSize As UInteger = 0

                    Dim fileStreamRead As FileStream = New FileStream(filePath, FileMode.Open)
                    Dim reader As BinaryReader = New BinaryReader(fileStreamRead, Text.Encoding.UTF8)

                    'Starts reading

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

                    Dim sound As Audio.Sound = Nothing
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

                                sound = New Audio.Sound(New Audio.Formats.WaveFormat(sampleRate, bitDepth, channels,, fmtCode))

                                'Checks to see if the whole of subchunk1 has been read
                                While reader.BaseStream.Position < fmtChunkStartPosition + fmtSize
                                    reader.ReadByte()
                                End While

                                'Noting that the format chunk is read
                                FormatChunkIsRead = True

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

                                    'Storing frequency and time weightings on the both sentence, channel and top SMA level
                                    sound.SMA.SetFrequencyWeighting(soundLevelMeasurementFormat.FrequencyWeighting, False)
                                    sound.SMA.ChannelData(channel).SetFrequencyWeighting(soundLevelMeasurementFormat.FrequencyWeighting, False)
                                    sound.SMA.ChannelData(channel)(sentence).SetFrequencyWeighting(soundLevelMeasurementFormat.FrequencyWeighting, False)

                                    If soundLevelMeasurementFormat.LoudestSectionMeasurement = True Then
                                        sound.SMA.SetTimeWeighting(DefaultTimeWeighting, False)
                                        sound.SMA.ChannelData(channel).SetTimeWeighting(DefaultTimeWeighting, False)
                                        sound.SMA.ChannelData(channel)(sentence).SetTimeWeighting(DefaultTimeWeighting, False)
                                    Else
                                        sound.SMA.SetTimeWeighting(0, False)
                                        sound.SMA.ChannelData(channel).SetTimeWeighting(0, False)
                                        sound.SMA.ChannelData(channel)(sentence).SetTimeWeighting(0, False)
                                    End If

                                    'Higher levels, not previously used
                                    Dim EnforceDataOnHigherLevels As Boolean = False
                                    If EnforceDataOnHigherLevels = True Then
                                        'Adding data described in the new SMA format, for both the top and channel levels
                                        sound.SMA.ChannelData(channel).UnWeightedLevel = sound.SMA.ChannelData(channel)(sentence).UnWeightedLevel
                                        sound.SMA.ChannelData(channel).UnWeightedPeakLevel = sound.SMA.ChannelData(channel)(sentence).UnWeightedPeakLevel
                                        sound.SMA.ChannelData(channel).WeightedLevel = sound.SMA.ChannelData(channel)(sentence).WeightedLevel
                                    End If

                                    'Word level data
                                    For word = 0 To currentWordCount - 1

                                        sound.SMA.ChannelData(channel)(sentence).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(sound.SMA, Sound.SpeechMaterialAnnotation.SmaTags.WORD, sound.SMA.ChannelData(channel)(sentence)))

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
                                            sound.SMA.ChannelData(channel)(sentence)(word).SetTimeWeighting(DefaultTimeWeighting, False)
                                        Else
                                            sound.SMA.ChannelData(channel)(sentence)(word).SetTimeWeighting(0, False)
                                        End If

                                        'Phone level data
                                        For phone = 0 To phoneListLength - 1

                                            Dim NewPhoneLevelData = New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(sound.SMA, Sound.SpeechMaterialAnnotation.SmaTags.PHONE, sound.SMA.ChannelData(channel)(sentence)(word))

                                            Dim phoneticTranscription As String = reader.ReadChars(10)
                                            NewPhoneLevelData.PhoneticForm = phoneticTranscription.Trim(" ")
                                            NewPhoneLevelData.StartSample = reader.ReadInt32
                                            NewPhoneLevelData.Length = reader.ReadInt32

                                            'Changing the previously used default not-measured value (-999999) to Nothing
                                            Dim puwl As Double = reader.ReadDouble
                                            If puwl = DefaultNotMeasuredValue Then
                                                NewPhoneLevelData.UnWeightedLevel = Nothing
                                            Else
                                                NewPhoneLevelData.UnWeightedLevel = puwl
                                            End If

                                            Dim ppl As Double = reader.ReadDouble
                                            If ppl = DefaultNotMeasuredValue Then
                                                NewPhoneLevelData.UnWeightedPeakLevel = Nothing
                                            Else
                                                NewPhoneLevelData.UnWeightedPeakLevel = ppl
                                            End If

                                            Dim pwl As Double = reader.ReadDouble
                                            If pwl = DefaultNotMeasuredValue Then
                                                NewPhoneLevelData.WeightedLevel = Nothing
                                            Else
                                                NewPhoneLevelData.WeightedLevel = pwl
                                            End If

                                            'Adding data described in the new SMA format
                                            NewPhoneLevelData.FrequencyWeighting = soundLevelMeasurementFormat.FrequencyWeighting
                                            If soundLevelMeasurementFormat.LoudestSectionMeasurement = True Then
                                                NewPhoneLevelData.TimeWeighting = DefaultTimeWeighting
                                            Else
                                                NewPhoneLevelData.TimeWeighting = 0
                                            End If

                                            'Adding the data if it is not a word-end string (these are not used in the new SMA stardard)
                                            If NewPhoneLevelData.PhoneticForm = WordEndString And TrimWordEndStrings = True Then
                                                'Not adding the data!
                                            Else
                                                'Adding the data
                                                sound.SMA.ChannelData(channel)(sentence)(word).Add(NewPhoneLevelData)
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

                            Case "iXML"

                                Dim iXMLDataStartReadPosition As Integer = reader.BaseStream.Position

                                'Copying iXML data to a new stream
                                Dim iXMLStream As New MemoryStream
                                For s = 0 To sizeOfNextChunk - 1
                                    iXMLStream.WriteByte(fileStreamRead.ReadByte)
                                Next
                                iXMLStream.Position = 0

                                'Parsing the iXML data
                                Dim iXMLdata = Audio.Sound.ParseiXMLString(iXMLStream)

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

                            Case "data"

                                'Aborting if the format chink has not yet been read
                                If FormatChunkIsRead = False Then
                                    AudioError("The wave file has an unsupported internal structure.")
                                    Return Nothing
                                End If

                                dataChunkFound = True
                                dataSize = sizeOfNextChunk

                            Case Else
                                Dim SizeOfUnknownChunk As UInteger = sizeOfNextChunk

                                'Reads to the end of the chunk but does not save the data
                                'Just ignores (not saving) any unparsed chunks, as no such were ever included in the ptwf files
                                reader.ReadBytes(SizeOfUnknownChunk)

                                'Reading padding byte
                                If SizeOfUnknownChunk Mod 2 = 1 Then
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
                            Case = Audio.Formats.WaveFormat.WaveFormatEncodings.PCM
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
                            Case = Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints
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

                    fileStreamRead.Close()

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

                    'Adding the input file name
                    sound.FileName = fileName

                    Return sound

                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try

            End Function

            Public Sub ConvertPtwfBatchToSma(ByVal UpdateSmaLevels As Boolean,
                                         Optional ByVal DefaultTimeWeighting As Double = 0.1,
                                         Optional ByVal ChangePhoneticNameToOrthographic As Boolean = False,
                                         Optional ByVal InputFolder As String = "",
                                         Optional ByVal OutputFolder As String = "",
                                         Optional ByVal ChangeNameToIterationString As Boolean = False,
                                         Optional ByVal IterationNamePrefix As String = "")

                Try

                    'Choosing input folder
                    If InputFolder = "" Then

                        Dim fbd As New Windows.Forms.FolderBrowserDialog
                        fbd.Description = "Select input folder"
                        If fbd.ShowDialog = Windows.Forms.DialogResult.OK Then
                            InputFolder = fbd.SelectedPath & "\"
                        End If

                    End If
                    If InputFolder = "" Then Exit Sub

                    'Choosing output folder
                    If OutputFolder = "" Then
                        Dim ofbd As New Windows.Forms.FolderBrowserDialog
                        ofbd.Description = "Select output folder"
                        If ofbd.ShowDialog = Windows.Forms.DialogResult.OK Then
                            OutputFolder = ofbd.SelectedPath & "\"
                        End If
                    End If
                    If OutputFolder = "" Then Exit Sub


                    'Collecting all files in fileEntries
                    Dim fileEntries As String() = Directory.GetFiles(InputFolder)

                    'Selecting all .ptwf files
                    Dim ptwfFilePaths As New List(Of String)
                    Dim FilesSaved As Integer = 0
                    For Each fileName In fileEntries

                        If fileName.EndsWith(".ptwf") Then
                            ptwfFilePaths.Add(fileName)
                        Else
                            Continue For
                        End If

                        'Reads the file
                        Dim CurrentSound = LoadPtwfFile(fileName,,,, DefaultTimeWeighting, True)

                        If UpdateSmaLevels = True Then
                            CurrentSound.SMA.ResetSoundLevels()
                            CurrentSound.SMA.MeasureSoundLevels(False)
                        End If

                        If ChangePhoneticNameToOrthographic = True Then
                            Dim Prefix As String = CurrentSound.FileName.Substring(0, 10)
                            Try
                                CurrentSound.FileName = Prefix & CurrentSound.SMA.ChannelData(1)(0)(0).OrthographicForm
                            Catch ex As Exception
                                MsgBox("Failed to change filename of the file: " & CurrentSound.FileName & " to spelling. Copying if with its original filename instead.")
                            End Try
                        End If

                        If ChangeNameToIterationString = True Then
                            FilesSaved += 1
                            CurrentSound.FileName = IterationNamePrefix & FilesSaved.ToString("00")
                        End If

                        ''Changing klas to Klas
                        'If CurrentSound.FileName.Contains("klas") Then
                        '    For c = 1 To CurrentSound.SMA.ChannelCount
                        '        CurrentSound.SMA.ChannelData(c).OrthographicForm = CurrentSound.SMA.ChannelData(c).OrthographicForm.Replace("k", "K")
                        '        For s = 0 To CurrentSound.SMA.ChannelData(c).Count - 1
                        '            CurrentSound.SMA.ChannelData(c)(s).OrthographicForm = CurrentSound.SMA.ChannelData(c)(s).OrthographicForm.Replace("k", "K")
                        '            For w = 0 To CurrentSound.SMA.ChannelData(c)(s).Count - 1
                        '                CurrentSound.SMA.ChannelData(c)(s)(w).OrthographicForm = CurrentSound.SMA.ChannelData(c)(s)(w).OrthographicForm.Replace("k", "K")
                        '            Next
                        '        Next
                        '    Next

                        '    CurrentSound.FileName = CurrentSound.FileName.Replace("k", "K")
                        '    'AudioLog.SendInfoToAudioLog("Updated file: " & CurrentSound.FileName, , "C:\SpeechAndHearingToolsLog")
                        'End If

                        ''Changing set to sätt
                        'If CurrentSound.FileName.Contains("set") Then
                        '    For c = 1 To CurrentSound.SMA.ChannelCount
                        '        CurrentSound.SMA.ChannelData(c).OrthographicForm = CurrentSound.SMA.ChannelData(c).OrthographicForm.Replace("set", "sätt")
                        '        For s = 0 To CurrentSound.SMA.ChannelData(c).Count - 1
                        '            CurrentSound.SMA.ChannelData(c)(s).OrthographicForm = CurrentSound.SMA.ChannelData(c)(s).OrthographicForm.Replace("set", "sätt")
                        '            For w = 0 To CurrentSound.SMA.ChannelData(c)(s).Count - 1
                        '                CurrentSound.SMA.ChannelData(c)(s)(w).OrthographicForm = CurrentSound.SMA.ChannelData(c)(s)(w).OrthographicForm.Replace("set", "sätt")
                        '            Next
                        '        Next
                        '    Next

                        '    CurrentSound.FileName = CurrentSound.FileName.Replace("set", "sätt")
                        '    'AudioLog.SendInfoToAudioLog("Updated file: " & CurrentSound.FileName, , "C:\SpeechAndHearingToolsLog")

                        'End If


                        'Saving to .wav with iXML/SMA
                        Dim OutputPath As String = Path.Combine(OutputFolder, CurrentSound.FileName)

                        CurrentSound.WriteWaveFile(OutputPath)

                    Next

                    MsgBox("Conversion of ptwf files completed successfully!")

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub

        End Module

    End Namespace

End Namespace