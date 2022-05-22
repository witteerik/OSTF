Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Xml
Imports System.Globalization.CultureInfo

Namespace Audio

    Partial Public Class Sound


        ''' <summary>
        ''' A class used to store data related to the segmentation of speech material sound files. All data can be contained and saved in a wave file.
        ''' </summary>
        <Serializable>
        Public Class SpeechMaterialAnnotation

            Public Property ParentSound As Sound

            Public Const CurrentVersion As String = "1.1"
            ''' <summary>
            ''' Holds the SMA version of the file that the data was loaded from, or CurrentVersion if the data was not loaded from file.
            ''' </summary>
            Public ReadFromVersion As String = CurrentVersion ' Using CurrentVersion as default

            Public Property SegmentationCompleted As Boolean = True

            Public ReadOnly Property ChannelCount As Integer
                Get
                    Return _ChannelData.Count
                End Get
            End Property

            Private _ChannelData As New List(Of SmaComponent)
            Property ChannelData(ByVal Channel As Integer) As SmaComponent
                Get
                    Return _ChannelData(Channel - 1)
                End Get
                Set(value As SmaComponent)
                    _ChannelData(Channel - 1) = value
                End Set
            End Property

            Public Sub AddChannelData()
                _ChannelData.Add(New SmaComponent(Me, SmaTags.CHANNEL))
            End Sub

            Public Sub AddChannelData(ByRef NewSmaChannelData As SmaComponent)
                _ChannelData.Add(NewSmaChannelData)
            End Sub


            ''' <summary>
            ''' Creates a new instance of SpeechMaterialAnnotation
            ''' </summary>
            Public Sub New(Optional ByVal DefaultFrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z,
                       Optional ByVal DefaultTimeWeighting As Double = 0)
                SetFrequencyWeighting(DefaultFrequencyWeighting, False)
                SetTimeWeighting(DefaultTimeWeighting, False)
            End Sub


            Private _FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z
            Public Function GetFrequencyWeighting() As FrequencyWeightings
                Return _FrequencyWeighting
            End Function

            Public Sub SetFrequencyWeighting(ByVal FrequencyWeighting As FrequencyWeightings, ByVal EnforceOnAllDescendents As Boolean)

                'Setting only SMA top level frequency weighting
                _FrequencyWeighting = FrequencyWeighting

                If EnforceOnAllDescendents = True Then
                    'Enforcing the same frequency weighting on all descendant channels, sentences, words, and phones
                    For Each c In _ChannelData
                        c.SetFrequencyWeighting(FrequencyWeighting, EnforceOnAllDescendents)
                    Next
                End If
            End Sub

            Private _TimeWeighting As Double = 0
            Public Function GetTimeWeighting() As Double
                Return _TimeWeighting
            End Function

            Public Sub SetTimeWeighting(ByVal TimeWeighting As Double, ByVal EnforceOnAllDescendents As Boolean)

                'Setting only SMA top level Time weighting
                _TimeWeighting = TimeWeighting

                If EnforceOnAllDescendents = True Then
                    'Enforcing the same Time weighting on all descendant channels, sentences, words, and phones
                    For Each c In _ChannelData
                        c.SetTimeWeighting(TimeWeighting, EnforceOnAllDescendents)
                    Next
                End If
            End Sub


            Public Shadows Function ToString(ByVal IncludeHeadings As Boolean) As String

                Dim HeadingList As New List(Of String)
                Dim OutputList As New List(Of String)

                If IncludeHeadings = True Then
                    HeadingList.Add("SMA")
                    HeadingList.Add("SMA_VERSION")
                    OutputList.Add("")
                    OutputList.Add(Sound.SpeechMaterialAnnotation.CurrentVersion) 'I.e. SMA version number
                End If

                For c = 0 To _ChannelData.Count

                    HeadingList.Add("CHANNEL")
                    OutputList.Add(c + 1)

                    _ChannelData(c).ToString(HeadingList, OutputList)
                Next

                If IncludeHeadings = True Then
                    Return String.Join(vbTab, HeadingList) & vbCrLf &
                        String.Join(vbTab, OutputList)
                Else
                    Return String.Join(vbTab, OutputList)
                End If



            End Function


            ''' <summary>
            ''' Converts all instances of a specified phone to a new phone
            ''' </summary>
            ''' <param name="CurrentPhone"></param>
            ''' <param name="NewPhone"></param>
            Public Sub ConvertPhone(ByVal CurrentPhone As String, ByVal NewPhone As String)

                For Each c In _ChannelData
                    c.ConvertPhone(CurrentPhone, NewPhone)
                Next

            End Sub



            ''' <summary>
            ''' Measures sound levels for each channel, sentence, word and phone of the current SMA object.
            ''' </summary>
            ''' <returns>Returns True if all measurements were successful, and False if one or more measurements failed.</returns>
            Public Function MeasureSoundLevels(Optional ByVal LogMeasurementResults As Boolean = False, Optional ByVal LogFolder As String = "") As Boolean

                If ParentSound Is Nothing Then
                    Throw New Exception("The parent sound if the current instance of SpeechMaterialAnnotation cannot be Nothing!")
                End If

                Dim SuccesfullMeasurementsCount As Integer = 0
                Dim AttemptedMeasurementCount As Integer = 0

                'Measuring each channel 
                For c As Integer = 1 To ChannelCount
                    ChannelData(c).MeasureSoundLevels(c, AttemptedMeasurementCount, SuccesfullMeasurementsCount)
                Next

                'Logging results
                If LogMeasurementResults = True Then
                    SendInfoToAudioLog(vbCrLf &
                                   "FileName" & vbTab & ParentSound.FileName & vbTab &
                                   "FailedMeasurementCount: " & vbTab & AttemptedMeasurementCount - SuccesfullMeasurementsCount & vbTab &
                                   ToString(True), "SoundMeasurementLog.txt", LogFolder)
                End If

                'Checking if all attempted measurements were succesful
                If AttemptedMeasurementCount = SuccesfullMeasurementsCount Then
                    Return True
                Else
                    Return False
                End If

            End Function


            ''' <summary>
            ''' Measures the unalterred (original recording) absolute peak amplitude (linear value) within the word parts of a segmented audio recording. 
            ''' At least WordStartSample and WordLength of all words must be set prior to calling this function.
            ''' </summary>
            ''' <param name="MeasurementSound">The sound to measure.</param>
            ''' <returns>Returns True if all measurements were successful, and False if one or more measurements failed.</returns>
            Public Function SetInitialPeakAmplitudes(ByVal MeasurementSound As Sound) As Boolean

                If MeasurementSound Is Nothing Then
                    Throw New Exception("The parent sound if the current instance of SpeechMaterialAnnotation cannot be Nothing!")
                End If

                Dim SuccesfullMeasurementsCount As Integer = 0
                Dim AttemptedMeasurementCount As Integer = 0

                'Measuring each channel 
                For c As Integer = 1 To ChannelCount
                    ChannelData(c).SetInitialPeakAmplitudes(MeasurementSound, c, AttemptedMeasurementCount, SuccesfullMeasurementsCount)
                Next

                'Logging results
                SendInfoToAudioLog(vbCrLf &
                               "FileName" & vbTab & ParentSound.FileName & vbTab &
                               "FailedMeasurementCount: " & vbTab & AttemptedMeasurementCount - SuccesfullMeasurementsCount & vbTab &
                               ToString(True), "InitialPeakMeasurementLog.txt")

                'Checking if all attempted measurements were succesful
                If AttemptedMeasurementCount = SuccesfullMeasurementsCount Then
                    Return True
                Else
                    Return False
                End If

            End Function


            ''' <summary>
            ''' Resets the sound levels for each channel, sentence, word and phone of the current SMA object.
            ''' </summary>
            Public Sub ResetSoundLevels()

                For Each c In _ChannelData
                    c.ResetSoundLevels()
                Next

            End Sub


            ''' <summary>
            ''' This sub adds a word end string to the the phonetic and orthographic forms of each level of the SMA object. However, if a word end marker does already exists, it is not added.
            ''' </summary>
            Public Sub AddWordEndString()

                For Each c In _ChannelData
                    c.AddWordEndString()
                Next

            End Sub

            ''' <summary>
            ''' This sub removes word end component based defined by the presence of word end strings in the phonetic or orthographic forms throughout current SMA object.
            ''' </summary>
            Public Sub RemoveWordEndString()

                For Each c In _ChannelData
                    c.RemoveWordEndString()
                Next

            End Sub

            ''' <summary>
            ''' This sub resets segmentation time data (startSample and Length) stored in the current SMA object.
            ''' </summary>
            Public Sub ResetTemporalData()

                For Each c In _ChannelData
                    c.ResetTemporalData()
                Next

            End Sub



            ''' <summary>
            ''' Detects the boundaries (speech start, and speech end) of recorded speech (with no pauses), using the transcriptions stored in the current SMA object.
            ''' This method may be used to fascilitate manual segmentation, as it suggests appropriate word/sentence boundary positions.
            '''The method works by 
            ''' a) detecting the speech location by assuming that the loudest window in the recording will be found inside the sentence/word.
            ''' b) detecting word/sentence start by locating the centre of the last window of a silent section of at least LongestSilentSegment milliseconds.
            ''' c) detecting word/sentence end by locating the centre of the first window of a silent section of at least LongestSilentSegment milliseconds.
            ''' </summary>
            ''' <param name="MeasurementSound">The sound that the measurents should be made upon.</param>
            ''' <param name="LongestSilentSegment">The longest silent section (in seconds) that is allowed within the speech recording.</param>
            ''' <param name="SilenceDefinition">The definition of a silent window is set to SilenceDefinition dB lower that the loudest detected window in the recording.</param>
            Public Sub DetectSpeechBoundaries(ByRef MeasurementSound As Sound,
                                          Optional ByVal LongestSilentSegment As Double = 0.3,
                                          Optional ByVal SilenceDefinition As Double = 40,
                                          Optional ByVal TemporalIntegrationDuration As Double = 0.06,
                                          Optional ByVal DetailedTemporalIntegrationDuration As Double = 0.006,
                                          Optional ByVal DetailedSilenceCriteria As Double = 20,
                                          Optional ByVal SetToZeroCrossings As Boolean = True) ',
                'Optional ByVal PlaceIntermediatePhonemes As Boolean = False)

                Try

                    'Storing the original soundlevel format
                    Dim OriginalSoundFrequencyWeighting As FrequencyWeightings = MeasurementSound.SMA.GetFrequencyWeighting
                    Dim OriginalSoundTimeWeighting As Double = MeasurementSound.SMA.GetTimeWeighting

                    'Creating a temporary sound level format
                    MeasurementSound.SMA.SetFrequencyWeighting(FrequencyWeightings.Z, True)
                    MeasurementSound.SMA.SetTimeWeighting(TemporalIntegrationDuration, True)

                    'Frequency weighting the sound prior to measurement

                    'Doing high-pass filterring to reduce vibration influences
                    Dim CurrentFftFormat As Formats.FftFormat = New Formats.FftFormat(1024 * 4,,,, True)
                    Dim PreFftFilteringKernelSize As Integer = 4000
                    Dim kernel As Sound = GenerateSound.CreateSpecialTypeImpulseResponse(MeasurementSound.WaveFormat, CurrentFftFormat, PreFftFilteringKernelSize, ,
                                                                                                 FilterType.LinearAttenuationBelowCF_dBPerOctave,
                                                                                                 100,,,, 25,, True)
                    Dim filterredSound As Sound = DSP.TransformationsExt.FIRFilter(MeasurementSound, kernel, CurrentFftFormat,,,,,, True)
                    Dim FirFilterDelayInSamles As Integer = kernel.WaveData.ShortestChannelSampleCount / 2

                    'Creating a temporary and extended copy of the filterred sound, with the delay created by the FIR-filtering removed
                    Dim tempSound As Sound = New Sound(MeasurementSound.WaveFormat)
                    tempSound.FFT = New FftData(MeasurementSound.WaveFormat, CurrentFftFormat)
                    For c = 1 To MeasurementSound.WaveFormat.Channels

                        Dim SoundArray(MeasurementSound.WaveData.SampleData(c).Length + CurrentFftFormat.AnalysisWindowSize - 1) As Single
                        For s = 0 To filterredSound.WaveData.SampleData(c).Length - FirFilterDelayInSamles - 1
                            SoundArray(s) = filterredSound.WaveData.SampleData(c)(s + FirFilterDelayInSamles)
                        Next
                        tempSound.WaveData.SampleData(c) = SoundArray

                    Next
                    tempSound.SMA = MeasurementSound.SMA

                    'Code to save the original and filterred sound for inspection
                    'AudioIOs.SaveToWaveFile(MeasurementSound,,,,,, "Original")
                    'AudioIOs.SaveToWaveFile(filterredSound,,,,,, "Filterred")
                    'AudioIOs.SaveToWaveFile(tempSound,,,,,, "FilterredAndAdjusted")

                    If tempSound Is Nothing Then
                        Throw New Exception("Something went wrong during IIR-filterring")
                    End If

                    For c = 1 To Me.ChannelCount

                        For sentence As Integer = 0 To Me.ChannelData(c).Count - 1
                            'Looking at channel c

                            'Detecting the start position, and level, of the loudest 100/4 ms window
                            Dim LoudestWindowLevel As Double
                            Dim WholeWindowList As New List(Of Double)
                            Dim WindowDistance As Integer
                            Dim CurrentSilenceEndCandidateWindow As Integer
                            Dim CurrentSilenceLength As Double

                            'Measuring sentence sound level
                            LoudestWindowLevel = DSP.MeasureTimeAndFrequencyWeightedSectionLevel_QuarterOverlap(
                        tempSound, c, 0, tempSound.WaveData.ShortestChannelSampleCount, SoundDataUnit.dB,
                        WholeWindowList,, WindowDistance)
                            Dim IndexOfLoudestWindow As Integer = WholeWindowList.LastIndexOf(WholeWindowList.Max)

                            'Setting the value of silence definition
                            Dim SilenceLevel As Double = LoudestWindowLevel - SilenceDefinition
                            Dim SilenceRMS As Double = dBConversion(SilenceLevel, dBConversionDirection.from_dB, tempSound.WaveFormat)

                            'Looking for the speech-onset window
                            Dim SpeechStartSample As Integer = 0
                            CurrentSilenceEndCandidateWindow = 0 'Setting default to the first window
                            CurrentSilenceLength = 0
                            For InverseWindowIndex = 0 To IndexOfLoudestWindow - 1
                                Dim CurrentWindow As Integer = IndexOfLoudestWindow - 1 - InverseWindowIndex

                                If WholeWindowList(CurrentWindow) > SilenceRMS Then
                                    'The window is not "silent"
                                    'Resetting CurrentSilenceLength
                                    CurrentSilenceLength = 0

                                    'Resetting CurrentSilenceEndCandidateWindow to the first window
                                    CurrentSilenceEndCandidateWindow = 0

                                Else
                                    'The window is "silent"
                                    'Updating the CurrentSilenceEndCandidate
                                    If CurrentWindow > CurrentSilenceEndCandidateWindow Then
                                        CurrentSilenceEndCandidateWindow = CurrentWindow
                                    End If

                                    'Adding the current window length to the silent section
                                    CurrentSilenceLength += WindowDistance / MeasurementSound.WaveFormat.SampleRate

                                End If

                                'Checking if the silent section found is equal to or longer than LongestSilentSegment
                                If CurrentSilenceLength >= LongestSilentSegment Then

                                    'The word/sentence start has been found
                                    SpeechStartSample = WindowDistance * (CurrentSilenceEndCandidateWindow + 1)
                                    Exit For
                                End If
                            Next

                            'Copying the window that follows after the first detected silant windo to a new sound, which is analysed to detect a more exact boundary
                            Dim TempSound1 As Sound = New Sound(New Formats.WaveFormat(tempSound.WaveFormat.SampleRate, tempSound.WaveFormat.BitDepth, 1))
                            Dim TempSoundArray(WindowDistance * 4 - 1) As Single
                            For sample = 0 To TempSoundArray.Length - 1
                                TempSoundArray(sample) = tempSound.WaveData.SampleData(c)(sample + SpeechStartSample)
                            Next
                            TempSound1.WaveData.SampleData(1) = TempSoundArray
                            TempSound1.SMA = New Sound.SpeechMaterialAnnotation(FrequencyWeightings.Z, DetailedTemporalIntegrationDuration)

                            'Adding one one channel and one sentence
                            TempSound1.SMA.AddChannelData(New Sound.SpeechMaterialAnnotation.SmaComponent(TempSound1.SMA, SmaTags.CHANNEL))
                            TempSound1.SMA.ChannelData(1).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(TempSound1.SMA, SmaTags.SENTENCE))

                            'Looking inside the TempSound1 window to determine a more exact boundary 
                            Dim InnerWindowList As New List(Of Double)
                            Dim InnerWindowDistance As Integer
                            LoudestWindowLevel = DSP.MeasureTimeAndFrequencyWeightedSectionLevel_QuarterOverlap(
                        TempSound1, 1, 0,, SoundDataUnit.dB,,
                        InnerWindowList, InnerWindowDistance)
                            'Detecting the first window that is at least DetailedSilenceCriteria softer than the loudest window
                            Dim InnerStartSample As Integer = 0
                            Dim InnerListMaxLevel As Double = dBConversion(InnerWindowList.Max, dBConversionDirection.to_dB, TempSound1.WaveFormat)
                            Dim InnelListLevelLimit As Double = InnerListMaxLevel - DetailedSilenceCriteria
                            Dim InnerListRMSLimit As Double = dBConversion(InnelListLevelLimit, dBConversionDirection.from_dB, TempSound1.WaveFormat)

                            For w = 0 To InnerWindowList.Count - 1
                                If InnerWindowList(w) >= InnerListRMSLimit And InnerWindowList(w) > 0 Then 'The last argument ignores windows made up of zero-padding
                                    InnerStartSample = w * InnerWindowDistance
                                    Exit For
                                End If
                            Next

                            'Adding the detailed samples from the last analysis
                            SpeechStartSample += InnerStartSample


                            'Looking for the speech-end window
                            Dim SpeechEndSample As Integer = tempSound.WaveData.ShortestChannelSampleCount - 1

                            'Restting CurrentSilenceEndCandidateWindow and CurrentSilenceLength 
                            CurrentSilenceEndCandidateWindow = WholeWindowList.Count - 1 'Since the three last windows Setting default to the last window
                            CurrentSilenceLength = 0

                            For CurrentWindow = IndexOfLoudestWindow + 1 To WholeWindowList.Count - 1

                                If WholeWindowList(CurrentWindow) > SilenceRMS Then
                                    'The window is not "silent"
                                    'Resetting CurrentSilenceLength
                                    CurrentSilenceLength = 0

                                    'Resetting CurrentSilenceEndCandidateWindow to the last window
                                    CurrentSilenceEndCandidateWindow = WholeWindowList.Count - 1

                                Else
                                    'The window is "silent"
                                    'Updating the CurrentSilenceEndCandidate
                                    If CurrentWindow < CurrentSilenceEndCandidateWindow Then
                                        CurrentSilenceEndCandidateWindow = CurrentWindow
                                    End If

                                    'Adding the current window distance to the silent section
                                    CurrentSilenceLength += WindowDistance / tempSound.WaveFormat.SampleRate

                                End If

                                'Checking if the silent section found is equal to or longer than LongestSilentSegment
                                If CurrentSilenceLength >= LongestSilentSegment Then

                                    'The word/sentence end has been found
                                    SpeechEndSample = WindowDistance * (CurrentSilenceEndCandidateWindow - 1)

                                    Exit For
                                End If
                            Next

                            'Copying the window prior to the detected silent window to a new sound, and analyses it to detect a more exact boundary
                            Dim TempSound2 As Sound = New Sound(New Formats.WaveFormat(tempSound.WaveFormat.SampleRate, tempSound.WaveFormat.BitDepth, 1))
                            Dim TempSoundArray2(WindowDistance * 4 - 1) As Single

                            'Correcting SpeechEndSample to be at least one window lower than the sound array
                            If SpeechEndSample > tempSound.WaveData.SampleData(c).Length - TempSoundArray2.Length - 1 Then
                                SpeechEndSample = tempSound.WaveData.SampleData(c).Length - TempSoundArray2.Length - 1
                            End If

                            For sample = 0 To TempSoundArray2.Length - 1
                                TempSoundArray2(sample) = tempSound.WaveData.SampleData(c)(sample + SpeechEndSample)
                            Next
                            TempSound2.WaveData.SampleData(1) = TempSoundArray
                            TempSound2.SMA = New Sound.SpeechMaterialAnnotation(FrequencyWeightings.Z, DetailedTemporalIntegrationDuration)

                            'Adding one one channel and one sentence
                            TempSound2.SMA.AddChannelData(New Sound.SpeechMaterialAnnotation.SmaComponent(TempSound2.SMA, SmaTags.CHANNEL))
                            TempSound2.SMA.ChannelData(1).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(TempSound2.SMA, SmaTags.SENTENCE))

                            'Looking inside the TempSound1 window to determine a more exakt boundary 
                            InnerWindowList = New List(Of Double)
                            LoudestWindowLevel = DSP.MeasureTimeAndFrequencyWeightedSectionLevel_QuarterOverlap(
                        TempSound2, 1, 0, , SoundDataUnit.dB,,
                        InnerWindowList, InnerWindowDistance)
                            'Detecting the last window that is at least half as loud as the loudest window (6 dB)
                            Dim InnerEndSample As Integer = 0
                            InnerStartSample = 0
                            InnerListMaxLevel = dBConversion(InnerWindowList.Max, dBConversionDirection.to_dB, TempSound1.WaveFormat)
                            InnelListLevelLimit = InnerListMaxLevel - DetailedSilenceCriteria
                            InnerListRMSLimit = dBConversion(InnelListLevelLimit, dBConversionDirection.from_dB, TempSound1.WaveFormat)

                            For w_inverse = 0 To InnerWindowList.Count - 1
                                Dim w As Integer = InnerWindowList.Count - 1 - w_inverse
                                If InnerWindowList(w) >= InnerListRMSLimit And InnerWindowList(w) > 0 Then 'The last argument ignores windows made up of zero-padding
                                    InnerEndSample = w * InnerWindowDistance
                                    Exit For
                                End If
                            Next


                            'Adding the detailed samples from the last analysis
                            SpeechEndSample += InnerEndSample

                            If SetToZeroCrossings = True Then
                                SpeechStartSample = DSP.GetZeroCrossingSample(MeasurementSound, c, SpeechStartSample, DSP.MeasurementsExt.SearchDirections.Closest)
                                SpeechEndSample = DSP.GetZeroCrossingSample(MeasurementSound, c, SpeechEndSample, DSP.MeasurementsExt.SearchDirections.Closest)
                            End If

                            'Storing the data start and end samples in the ptwf object
                            'Storing speech start and end, on both sentence, word (only using the first word, which has index 0) and phoneme level

                            'Sentence level
                            Me.ChannelData(c)(sentence).StartSample = SpeechStartSample
                            Me.ChannelData(c)(sentence).Length = SpeechEndSample - SpeechStartSample - 1

                            'Word level (first word only (index 0))
                            Me.ChannelData(c)(sentence)(0).StartSample = SpeechStartSample
                            Me.ChannelData(c)(sentence)(0).Length = SpeechEndSample - SpeechStartSample - 1

                            'Phoneme level
                            If Me.ChannelData(c)(sentence)(0) IsNot Nothing Then
                                'Storing the position of the first phoneme
                                If Me.ChannelData(c)(sentence)(0).Count > 0 Then
                                    Me.ChannelData(c)(sentence)(0)(0).StartSample = SpeechStartSample
                                End If
                                'Storing the position of the last item in the phoneme array (which should be a word-end string)
                                If Me.ChannelData(c)(sentence)(0).Count > 1 Then
                                    Me.ChannelData(c)(sentence)(Me.ChannelData(c)(sentence).Count - 1)(Me.ChannelData(c)(sentence)(Me.ChannelData(c)(sentence).Count - 1).Count - 1).StartSample = SpeechEndSample
                                End If
                            End If

                        Next
                    Next

                    'Restoring the origial sound level format
                    MeasurementSound.SMA.SetFrequencyWeighting(OriginalSoundFrequencyWeighting, True)
                    MeasurementSound.SMA.SetTimeWeighting(OriginalSoundTimeWeighting, True)

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub

            ''' <summary>
            ''' Moves segmented phoneme boundaries to the nearset zero crossings. If DoFinalizeSegmentation = True, also phoneme lengths and sound levels are calculated, and fade padding applied the segmented sound.
            ''' </summary>
            ''' <param name="Sound"></param>
            ''' <param name="CurrentChannel"></param>
            ''' <param name="SearchDirection"></param>
            ''' <param name="DoFinalizeSegmentation"></param>
            ''' <param name="PaddingTime"></param>
            Public Sub MoveSegmentationsToClosestZeroCrossings(ByRef Sound As Sound, ByVal CurrentChannel As Integer, Optional ByVal SearchDirection As DSP.SearchDirections = DSP.SearchDirections.Closest,
                                                       Optional DoFinalizeSegmentation As Boolean = True, Optional ByRef PaddingTime As Single = 0.5)
                Try

                    'TODO: This sub currently can only use mono sounds!

                    'For c = 1 To Sound.WaveFormat.Channels

                    For sentence As Integer = 0 To Me.ChannelData(CurrentChannel).Count - 1

                        'Adjusting sentenceStartTime
                        Me.ChannelData(CurrentChannel)(sentence).StartSample = DSP.GetZeroCrossingSample(Sound, CurrentChannel, Me.ChannelData(CurrentChannel)(sentence).StartSample, SearchDirection)

                        'TODO: Perhaps ChannelData(CurrentChannel)(sentence).StartTime should also be updated? 
                        'For now, StartTime is derived from ChannelData(CurrentChannel)(sentence).StartSample (But, wasn't StartTime supposed to be in reference to a e.g. camera?)
                        Me.ChannelData(CurrentChannel)(sentence).StartTime = Me.ChannelData(CurrentChannel)(sentence).StartSample / Sound.WaveFormat.SampleRate

                        For word = 0 To Me.ChannelData(CurrentChannel)(sentence).Count - 1

                            'Adjusting wordStartTime and start sample
                            Me.ChannelData(CurrentChannel)(sentence)(word).StartTime = DSP.GetZeroCrossingSample(Sound, CurrentChannel, Me.ChannelData(CurrentChannel)(sentence)(word).StartTime, SearchDirection)
                            Me.ChannelData(CurrentChannel)(sentence)(word).StartSample = DSP.GetZeroCrossingSample(Sound, CurrentChannel, Me.ChannelData(CurrentChannel)(sentence)(word).StartSample, SearchDirection)

                            For phoneme = 0 To Me.ChannelData(CurrentChannel)(sentence)(word).Count - 1
                                'Adjusting phoneme start sample
                                Me.ChannelData(CurrentChannel)(sentence)(word)(phoneme).StartSample = DSP.GetZeroCrossingSample(Sound, CurrentChannel, Me.ChannelData(CurrentChannel)(sentence)(word)(phoneme).StartSample, SearchDirection)
                            Next phoneme
                        Next word

                        Me.UpdateSegmentation(Sound, PaddingTime, CurrentChannel)

                        'Next

                    Next
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub

            ''' <summary>
            ''' Updates the segmentation, data of the current sound file. The following steps are taken. N.B. Presently only multiple sentences are supported!
            ''' a) 
            ''' </summary>
            ''' <param name="Sound">The sound that belong to the current SMA object.</param>
            ''' <param name="PaddingTime">The time between sound start and speech start, as well as between speech end and sound end.</param>
            ''' <param name="CurrentChannel"></param>
            ''' <param name="FadeStartAndEnd">If set to True, the Padding section before and after the recorded material is faded in/out.</param>
            Public Sub UpdateSegmentation(ByRef Sound As Sound, ByRef PaddingTime As Single, ByRef CurrentChannel As Integer,
                                  Optional FadeStartAndEnd As Boolean = False,
                                  Optional FadeType As DSP.FadeSlopeType = DSP.FadeSlopeType.PowerCosine_SkewedFromFadeDirection,
                                  Optional CosinePower As Double = 100)
                Try

                    'Throw New NotImplementedException("The UpdateSegmentation method needs to be changed as the word end string has ben removed from the SMA segmentation. Instead of containing the word end string within the SMA object, an end string should be added by the current method (or in another appropriate place) on all segmentation levels (channel, sentence, word and phone, and then removed when not needed again.")

                    'Adding a word and string
                    AddWordEndString()

                    'Throws exception if ChannelSpecific is set to true. This should be removed when channel specific segmentation is implemented in the future
                    If CurrentChannel <> 1 Then Throw New NotImplementedException("At present, only mono sounds are supported by FinalizeSegmentation")

                    Dim sentence As Integer = 0

                    MsgBox("Time to add support for multiple sentences??? Only one sentence per sound file is supported by UpdateSegmentation")

                    'Determining the sentence start sample and length
                    Dim SentenceStartSample As Integer = Me.ChannelData(1)(sentence)(0)(0).StartSample
                    Dim SentenceEndSample As Integer = Me.ChannelData(1)(sentence)(Me.ChannelData(1)(sentence).Count - 1)(Me.ChannelData(1)(sentence)(Me.ChannelData(1)(sentence).Count - 1).Count - 1).StartSample
                    Dim SentenceLength As Integer = SentenceEndSample - SentenceStartSample

                    'Calculating the length of the padding section in samples
                    Dim PaddingSamples As Integer = PaddingTime * Sound.WaveFormat.SampleRate

                    'Storing the time of the first phoneme in the first word as sentence start time
                    Me.ChannelData(1)(sentence).StartTime += SentenceStartSample / Sound.WaveFormat.SampleRate

                    'Finding out how the the ends of the file should be adjusted
                    Dim temporalAdjustmentOfSoundArray As Integer = 0
                    Dim indendedChannelArrayLengthInSample As Integer = SentenceLength + (2 * PaddingSamples)

                    'Adding word start and length data, and adjusting the lengths of each channel separately (according to channel 1 segmentation data)
                    For c = 1 To Sound.WaveFormat.Channels

                        'Adding sentence time data (all from channel 1, which should be changed when ChannelSpecific is implemented)
                        Me.ChannelData(c)(sentence).StartSample = SentenceStartSample
                        Me.ChannelData(c)(sentence).Length = SentenceLength
                        Me.ChannelData(c)(sentence).StartTime = SentenceStartSample / Sound.WaveFormat.SampleRate

                        'Adding word time data
                        For word = 0 To Me.ChannelData(c)(sentence).Count - 1
                            Dim SampleOfFirstPhoneme As Integer = Me.ChannelData(c)(sentence)(word)(0).StartSample
                            Dim WordEndSample As Integer = Me.ChannelData(c)(sentence)(word)(Me.ChannelData(c)(sentence)(word).Count - 1).StartSample
                            Dim LengthOfWord As Integer = WordEndSample - SampleOfFirstPhoneme
                            Me.ChannelData(c)(sentence)(word).StartSample = Me.ChannelData(c)(sentence)(word)(0).StartSample
                            Me.ChannelData(c)(sentence)(word).Length = LengthOfWord
                            Me.ChannelData(c)(sentence)(word).StartTime = SampleOfFirstPhoneme / Sound.WaveFormat.SampleRate
                        Next

                        'Modifying the sound array so that it is equal in length and is synchronized with the newChannelArray
                        'taking care of the section prior to the first phoneme
                        Select Case SentenceStartSample
                            Case > PaddingSamples

                                'cutting the section of the sound array before padding time
                                Dim samplesToCut As Integer = SentenceStartSample - PaddingSamples
                                Dim tempArray(Sound.WaveData.SampleData(c).Length - samplesToCut - 1) As Single
                                For sample = 0 To tempArray.Length - 1
                                    tempArray(sample) = Sound.WaveData.SampleData(c)(sample + samplesToCut)
                                Next
                                Sound.WaveData.SampleData(c) = tempArray
                                If c = 1 Then 'TODO: Getting the sentence start and ends segmentation from channel 1 only. This should change when soundEditor is modified to support multi channel sounds
                                    temporalAdjustmentOfSoundArray = -samplesToCut
                                End If

                            Case < PaddingSamples
                                'extending the section of the sound array before padding time
                                Dim samplesToAdd As Integer = PaddingSamples - SentenceStartSample
                                Dim tempArray(Sound.WaveData.SampleData(c).Length + samplesToAdd - 1) As Single
                                For sample = 0 To Sound.WaveData.SampleData(c).Length - 1
                                    tempArray(sample + samplesToAdd) = Sound.WaveData.SampleData(c)(sample)
                                Next
                                Sound.WaveData.SampleData(c) = tempArray
                                If c = 1 Then 'TODO: This should actually be the measurement channel, however soundEditor presently only supports mono sounds
                                    temporalAdjustmentOfSoundArray = samplesToAdd
                                End If

                            Case Else
                                'I.E. equal length, does nothing

                        End Select

                        'Extending the sound array to the length of indendedChannelArrayLengthInSample
                        ReDim Preserve Sound.WaveData.SampleData(c)(indendedChannelArrayLengthInSample - 1)

                        If FadeStartAndEnd = True Then

                            'Fading in start
                            DSP.Fade(Sound, , 0, c, 0, PaddingSamples, FadeType, CosinePower)

                            'Fading out end
                            DSP.Fade(Sound, 0, , c, Sound.WaveData.SampleData(c).Length - PaddingSamples - 1,,
                                   FadeType, CosinePower)
                        End If

                    Next c


                    'Adjusting times stored in the SMA phoneme data
                    For c = 1 To Sound.WaveFormat.Channels

                        'Adjusting sentenceStartTime (this is the reference time, e.g the duration since a video camera was started), and start sample
                        Me.ChannelData(CurrentChannel)(sentence).StartTime = ((Me.ChannelData(CurrentChannel)(sentence).StartTime * Sound.WaveFormat.SampleRate) + temporalAdjustmentOfSoundArray) / Sound.WaveFormat.SampleRate
                        Me.ChannelData(CurrentChannel)(sentence).StartSample += temporalAdjustmentOfSoundArray

                        For word = 0 To Me.ChannelData(c)(sentence).Count - 1

                            'Adjusting wordStartTime and start sample
                            Me.ChannelData(CurrentChannel)(sentence)(word).StartTime = ((Me.ChannelData(CurrentChannel)(sentence)(word).StartTime * Sound.WaveFormat.SampleRate) + temporalAdjustmentOfSoundArray) / Sound.WaveFormat.SampleRate

                            Me.ChannelData(CurrentChannel)(sentence)(word).StartSample += temporalAdjustmentOfSoundArray

                            For phoneme = 0 To Me.ChannelData(CurrentChannel)(sentence)(word).Count - 1
                                'Adjusting phoneme start sample (If phoneme position has been set, i.e. has a value other than -1)
                                If Me.ChannelData(CurrentChannel)(sentence)(word)(phoneme).StartSample <> -1 Then
                                    Me.ChannelData(CurrentChannel)(sentence)(word)(phoneme).StartSample += temporalAdjustmentOfSoundArray
                                End If
                            Next phoneme
                        Next word
                    Next c

                    'Setting phoneme lengths
                    For c = 1 To Sound.WaveFormat.Channels
                        For word = 0 To Me.ChannelData(c)(sentence).Count - 1

                            ''Setting the length of the last phoneme to 0 if it is the WordEndString
                            'If Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData(Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData.Count - 1).PhoneticForm = WordEndString Then
                            '    Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData(Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData.Count - 1).Length = 0
                            'End If

                            'Setting the length of the rest of the phonemes
                            For phoneme = 0 To Me.ChannelData(CurrentChannel)(sentence)(word).Count - 2
                                'Setting the phoneme length to that of the distance between the current phoneme and the next (If both phoneme positions have been set, i.e. have values other than -1)
                                If Me.ChannelData(CurrentChannel)(sentence)(word)(phoneme).StartSample <> -1 And Me.ChannelData(CurrentChannel)(sentence)(word)(phoneme + 1).StartSample <> -1 Then
                                    Me.ChannelData(CurrentChannel)(sentence)(word)(phoneme).Length =
                                Me.ChannelData(CurrentChannel)(sentence)(word)(phoneme + 1).StartSample - Me.ChannelData(CurrentChannel)(sentence)(word)(phoneme).StartSample
                                End If
                            Next phoneme
                        Next word
                    Next c


                    'Measures InitialpeakAmplitudes
                    If Not Me.SetInitialPeakAmplitudes(Sound) = True Then MsgBox("Failed setting initial peak amplitudes.")

                    'Adjusting sound level
                    If Me.GetTimeWeighting <> 0 = True Then

                        If Not DSP.TimeAndFrequencyWeightedNormalization(Sound, CurrentChannel,,,) = True Then 'SoundLevelFormat.TemporalIntegrationDuration, SoundLevelFormat.Frequencyweighting) = True Then
                            MsgBox("Distorsion occurred during time and frequency weighted normalization.")
                        End If

                        'DSP.MeasureAndSetGatedSectionLevel(sound, currentChannel,,,
                        'SoundLevelFormat.OutputLevel,
                        'SoundLevelFormat.GatingWindowDuration,
                        'SoundLevelFormat.GateRelativeThreshold,
                        'SoundLevelFormat.FractionForCalculatingAbsThreshold,
                        'SoundLevelFormat.Frequencyweighting)

                    Else
                        DSP.MeasureAndAdjustSectionLevel(Sound, -23, CurrentChannel,,, Me.GetFrequencyWeighting)
                    End If

                    'Measuring sound levels
                    If Not Me.MeasureSoundLevels(True) = True Then MsgBox("Failed measuring sound levels.")

                    'Remoiving the previously added word and string
                    RemoveWordEndString()

                Catch ex As Exception

                    MsgBox(ex.ToString)

                End Try

            End Sub

            ''' <summary>
            ''' Goes through all segmentation data and detects unset start values and zero lengths. The accumulated number of unset starts, and zero-lengths are returned in the parameters.
            ''' N.B. Presently, only multiple sentences are supported!
            ''' </summary>
            Public Sub ValidateSegmentation(ByRef UnsetStarts As Integer, ByRef ZeroLengths As Integer,
                                    ByRef SetStarts As Integer, ByRef SetLengths As Integer)

                Dim sentence As Integer = 0

                MsgBox("Time to add support for multiple sentences??? Only one sentence per sound file is supported by ValidateSegmentation")

                Dim NumberOfMissingPhonemeArrays As Integer = 0

                'Adjusting times stored in the ptwf phoneme data
                For c = 1 To Me.ChannelCount

                    If Me.ChannelData(c)(sentence).StartSample = -1 Then
                        UnsetStarts += 1
                    Else
                        SetStarts += 1
                    End If
                    If Me.ChannelData(c)(sentence).Length = 0 Then
                        ZeroLengths += 1
                    Else
                        SetLengths += 1
                    End If

                    For word = 0 To Me.ChannelData(c)(sentence).Count - 1

                        If Me.ChannelData(c)(sentence)(word).StartSample = -1 Then
                            UnsetStarts += 1
                        Else
                            SetStarts += 1
                        End If
                        If Me.ChannelData(c)(sentence)(word).Length = 0 Then
                            ZeroLengths += 1
                        Else
                            SetLengths += 1
                        End If

                        If Me.ChannelData(c)(sentence)(word) IsNot Nothing Then
                            For phoneme = 0 To Me.ChannelData(c)(sentence)(word).Count - 1
                                If Me.ChannelData(c)(sentence)(word)(phoneme).StartSample = -1 Then
                                    UnsetStarts += 1
                                Else
                                    SetStarts += 1
                                End If
                                If Me.ChannelData(c)(sentence)(word)(phoneme).Length = 0 Then
                                    ZeroLengths += 1
                                Else
                                    SetLengths += 1
                                End If
                            Next phoneme
                        Else
                            'Not phoneme array exists
                            NumberOfMissingPhonemeArrays += 1
                        End If
                    Next word
                Next c

            End Sub

            ''' <summary>
            ''' Shifts all StartSample indices in the current instance of SpeechMaterialAnnotation, and limits the StartSample and Length values to the sample indices available in the parent sounds file
            ''' </summary>
            ''' <param name="ShiftInSamples"></param>
            Public Sub ShiftSegmentationData(ByVal ShiftInSamples As Integer)

                For c = 1 To Me.ChannelCount
                    Dim Channel = Me.ChannelData(c)
                    Dim SoundChannelLength As Integer = ParentSound.WaveData.SampleData(c).Length
                    LimitStartIndexAndLengthOnShift(ShiftInSamples, SoundChannelLength, Channel.StartSample, Channel.Length)
                    For Each Sentence In Channel
                        LimitStartIndexAndLengthOnShift(ShiftInSamples, SoundChannelLength, Sentence.StartSample, Sentence.Length)
                        For Each Word In Sentence
                            LimitStartIndexAndLengthOnShift(ShiftInSamples, SoundChannelLength, Word.StartSample, Word.Length)
                            For Each Phone In Word
                                LimitStartIndexAndLengthOnShift(ShiftInSamples, SoundChannelLength, Phone.StartSample, Phone.Length)
                            Next
                        Next
                    Next
                Next

            End Sub

            Private Sub LimitStartIndexAndLengthOnShift(ByVal Shift As Integer, ByVal TotalAvailableLength As Integer, ByRef StartIndex As Integer, ByRef Length As Integer)

                MsgBox("Check the code below for accuracy!!!")

                'Adjusting the StartSample and limiting it to the available range
                StartIndex = Math.Min(Math.Max(StartIndex + Shift, 0), TotalAvailableLength - 1)

                'Limiting Length to the available length after adjustment of startsample
                Dim MaximumPossibleLength = TotalAvailableLength - StartIndex
                Length = Math.Max(0, Math.Min(Length, MaximumPossibleLength))

            End Sub

            ''' <summary>
            ''' Fading the padded sections before the word start and after the word end.
            ''' N.B. Presently, only multiple sentences are supported!
            ''' </summary>
            ''' <param name="Sound"></param>
            ''' <param name="CurrentChannel"></param>
            ''' <param name="FadeType"></param>
            ''' <param name="CosinePower"></param>
            Public Sub FadePaddingSection(ByRef Sound As Sound, ByRef CurrentChannel As Integer,
                                  Optional FadeType As DSP.FadeSlopeType = DSP.FadeSlopeType.PowerCosine_SkewedFromFadeDirection,
                                  Optional CosinePower As Double = 100)

                Dim sentence As Integer = 0

                For c = 1 To Sound.WaveFormat.Channels

                    Dim SentenceStartSample As Integer = Sound.SMA.ChannelData(CurrentChannel)(sentence).StartSample
                    Dim SentenceEndSample As Integer = Sound.SMA.ChannelData(CurrentChannel)(sentence).StartSample + Sound.SMA.ChannelData(CurrentChannel)(sentence).Length


                    'Fading in start
                    DSP.Fade(Sound, , 0, c, 0, SentenceStartSample - 1, FadeType, CosinePower)

                    'Fading out end
                    DSP.Fade(Sound, 0, , c, SentenceEndSample + 1,,
                           FadeType, CosinePower)

                Next


            End Sub

            Public Function CreateCopy() As SpeechMaterialAnnotation

                'Creating an output object
                Dim newSmaData As SpeechMaterialAnnotation

                'Serializing to memorystream
                Dim serializedMe As New MemoryStream
                Dim serializer As New BinaryFormatter
                serializer.Serialize(serializedMe, Me)

                'Deserializing to new object
                serializedMe.Position = 0
                newSmaData = CType(serializer.Deserialize(serializedMe), SpeechMaterialAnnotation)
                serializedMe.Close()

                'Returning the new object
                Return newSmaData
            End Function

            Public Enum SmaTags
                CHANNEL
                SENTENCE
                WORD
                PHONE
            End Enum

#Region "MultiWordRecordings"

            'N.B. These methods were written when the SMA only had support for one sentence per channel. They should all be re-written!

            ''' <summary>
            ''' Adjusts the sound level of each ptwf word in the input sound, so that each carrier phrase gets the indicated target level. The level of each test word is adjusted together with its carrier phrase.
            ''' </summary>
            ''' <param name="InputSound"></param>
            Public Sub MultiWord_SoundLevelEqualization(ByRef InputSound As Sound, ByVal SoundChannel As Integer, Optional ByVal TargetLevel As Double = -30, Optional ByVal PaddingDuration As Double = 0.5)

                Dim sentence As Integer = 0

                Try

                    Dim PtwfChannel As Integer = 1 'This should be a parameter if multi channel ptwfs are needed

                    'Setting a padding interval
                    Dim PaddingSamples As Double = PaddingDuration * InputSound.WaveFormat.SampleRate

                    If InputSound IsNot Nothing Then

                        For WordIndex = 0 To Me.ChannelData(PtwfChannel)(sentence).Count - 1

                            'Measures the carries phrases
                            Dim CarrierLevel As Double = DSP.MeasureSectionLevel(InputSound, SoundChannel,
                                                                           Me.ChannelData(PtwfChannel)(sentence)(WordIndex)(0).StartSample,
                                                                           Me.ChannelData(PtwfChannel)(sentence)(WordIndex)(0).Length)

                            'Calculating gain
                            Dim Gain As Double = TargetLevel - CarrierLevel

                            'Applying gain to the whole word
                            DSP.AmplifySection(InputSound, Gain, SoundChannel, Me.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample - PaddingSamples, Me.ChannelData(PtwfChannel)(sentence)(WordIndex).Length + 2 * PaddingSamples)

                        Next

                    End If
                Catch ex As Exception
                    MsgBox("An error occurred: " & ex.ToString)
                End Try

            End Sub

            Public Sub MultiWord_SetInterStimulusInterval(ByRef InputSound As Sound, Optional ByVal Interval As Double = 3)

                Dim sentence As Integer = 0

                Try

                    Dim PtwfChannel As Integer = 1 'This should be a parameter if multi channel ptwfs are needed

                    Dim StimulusIntervalInSamples As Double = Interval * InputSound.WaveFormat.SampleRate

                    If InputSound IsNot Nothing Then

                        For WordIndex = 0 To Me.ChannelData(PtwfChannel)(sentence).Count - 2

                            'Getting the interval to the next word
                            Dim CurrentInterval As Integer =
                    Me.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample -
                    (Me.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample + Me.ChannelData(PtwfChannel)(sentence)(WordIndex).Length)

                            'Calculating adjustment length
                            Dim AdjustmentLength As Integer = StimulusIntervalInSamples - CurrentInterval

                            Select Case AdjustmentLength
                                Case < 0
                                    'Deleting a segment right between the words
                                    Dim StartDeleteSample As Integer = Me.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample - (CurrentInterval / 2) - (Math.Abs(AdjustmentLength / 2))
                                    DSP.DeleteSection(InputSound, StartDeleteSample, Math.Abs(AdjustmentLength))

                                Case > 0
                                    'Inserting a segment
                                    Dim StartInsertSample As Integer = Me.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample - (CurrentInterval / 2) - (Math.Abs(AdjustmentLength / 2))
                                    DSP.InsertSilentSection(InputSound, StartInsertSample, Math.Abs(AdjustmentLength))

                            End Select

                            'Adjusting the ptwf data of all following words
                            For FollowingWordIndex = WordIndex + 1 To Me.ChannelData(PtwfChannel)(sentence).Count - 1
                                Me.ChannelData(PtwfChannel)(sentence)(FollowingWordIndex).StartSample += AdjustmentLength
                                For p = 0 To Me.ChannelData(PtwfChannel)(sentence)(FollowingWordIndex).Count - 1
                                    Me.ChannelData(PtwfChannel)(sentence)(FollowingWordIndex)(p).StartSample += AdjustmentLength
                                Next
                            Next

                        Next

                    End If
                Catch ex As Exception
                    MsgBox("An error occurred: " & ex.ToString)
                End Try

            End Sub

            Public Sub MultiWord_InterStimulusSectionFade(ByRef InputSound As Sound, ByVal SoundChannel As Integer, Optional ByVal FadeTime As Double = 0.5)

                Dim sentence As Integer = 0

                Try

                    Dim PtwfChannel As Integer = 1 'This should be a parameter if multi channel ptwfs are needed

                    'Setting a stimulus interval
                    Dim GeneralFadeLength As Double = FadeTime * InputSound.WaveFormat.SampleRate

                    If InputSound IsNot Nothing Then

                        'Fades in the first sound
                        DSP.Fade(InputSound, Nothing, 0, SoundChannel, 0, Me.ChannelData(PtwfChannel)(sentence)(0).StartSample, DSP.FadeSlopeType.PowerCosine_SkewedFromFadeDirection, 20)

                        'Fades out the last sound
                        Dim FadeLastStart As Integer = Me.ChannelData(PtwfChannel)(sentence)(Me.ChannelData(PtwfChannel)(sentence).Count - 1).StartSample + Me.ChannelData(PtwfChannel)(sentence)(Me.ChannelData(PtwfChannel)(sentence).Count - 1).Length
                        DSP.Fade(InputSound, 0, Nothing, SoundChannel, FadeLastStart, , DSP.FadeSlopeType.PowerCosine_SkewedFromFadeDirection, 20)

                        'Fades all inter-stimulis sections
                        For WordIndex = 0 To Me.ChannelData(PtwfChannel)(sentence).Count - 2

                            'Getting the interval to the next word
                            Dim CurrentInterval As Integer =
                    Me.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample -
                    (Me.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample + Me.ChannelData(PtwfChannel)(sentence)(WordIndex).Length)


                            'Determines a suitable fade length
                            Dim FadeLength As Integer = Math.Min((CurrentInterval / 2) + 2, GeneralFadeLength)

                            'Fades out the current word
                            DSP.Fade(InputSound, 0, Nothing, SoundChannel, Me.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample + Me.ChannelData(PtwfChannel)(sentence)(WordIndex).Length, FadeLength)

                            'Fades in the next sound
                            DSP.Fade(InputSound, Nothing, 0, SoundChannel, Me.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample - FadeLength, FadeLength)

                            'Silencing the section between the fades
                            Dim SilenceStart As Integer = Me.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample + Me.ChannelData(PtwfChannel)(sentence)(WordIndex).Length
                            Dim SilenceLength As Integer = (Me.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample - FadeLength) - SilenceStart
                            DSP.SilenceSection(InputSound, SilenceStart, SilenceLength, SoundChannel)

                        Next

                    End If
                Catch ex As Exception
                    MsgBox("An error occurred: " & ex.ToString)
                End Try

            End Sub


            Public Enum MeasurementSections
                CarrierPhrases
                TestWords
                CarriersAndTestWords
            End Enum


            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="SoundChannel"></param>
            ''' <param name="MeasurementSection"></param>
            ''' <param name="FrequencyWeighting"></param>
            ''' <param name="DurationList">Can be used to retreive a list of durations (in seconds) used in the measurement.</param>
            ''' <param name="LengthList">Can be used to retreive a list of lengths (in samples) used in the measurement.</param>
            ''' <returns></returns>
            Public Function MultiWord_MeasureSoundLevelsOfSections(ByRef InputSound As Sound, ByVal SoundChannel As Integer,
                                                  ByVal MeasurementSection As Sound.SpeechMaterialAnnotation.MeasurementSections,
                                              Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z,
                                                       Optional DurationList As List(Of Double) = Nothing,
                                                       Optional LengthList As List(Of Integer) = Nothing) As Double

                Dim sentence As Integer = 0

                Dim PtwfChannel As Integer = 1 'This should be a parameter if multi channel ptwfs are needed

                Dim FirstPtwfPhonemeIndexToMeasure As Integer = 0
                Dim LastPtwfPhonemeIndexToMeasure As Integer = 0
                Select Case MeasurementSection
                    Case Sound.SpeechMaterialAnnotation.MeasurementSections.CarrierPhrases
                        FirstPtwfPhonemeIndexToMeasure = 0
                        LastPtwfPhonemeIndexToMeasure = 0
                    Case Sound.SpeechMaterialAnnotation.MeasurementSections.TestWords
                        FirstPtwfPhonemeIndexToMeasure = 1
                        LastPtwfPhonemeIndexToMeasure = 1
                    Case Sound.SpeechMaterialAnnotation.MeasurementSections.CarriersAndTestWords
                        FirstPtwfPhonemeIndexToMeasure = 0
                        LastPtwfPhonemeIndexToMeasure = 1
                    Case Else
                        MsgBox("An error occurred!")
                        Return Nothing
                End Select

                Try

                    If InputSound IsNot Nothing Then

                        Dim MeasurementSound As New Sound(InputSound.WaveFormat)

                        'Gets the length of the MeasurementSound
                        Dim TotalLength As Integer = 0
                        For WordIndex = 1 To Me.ChannelData(PtwfChannel)(sentence).Count - 2
                            Dim CurrentLength As Integer = (Me.ChannelData(PtwfChannel)(sentence)(WordIndex)(LastPtwfPhonemeIndexToMeasure).StartSample +
                    Me.ChannelData(PtwfChannel)(sentence)(WordIndex)(LastPtwfPhonemeIndexToMeasure).Length) -
                    Me.ChannelData(PtwfChannel)(sentence)(WordIndex)(FirstPtwfPhonemeIndexToMeasure).StartSample

                            'Adding the duration
                            If DurationList IsNot Nothing Then DurationList.Add(CurrentLength / InputSound.WaveFormat.SampleRate)
                            If LengthList IsNot Nothing Then LengthList.Add(CurrentLength)

                            TotalLength += CurrentLength
                        Next
                        Dim NewArray(TotalLength - 1) As Single
                        MeasurementSound.WaveData.SampleData(SoundChannel) = NewArray


                        'Copies all test word sections to a the MeasurementSound
                        Dim CurrentReadSample As Integer = 0

                        For WordIndex = 1 To Me.ChannelData(PtwfChannel)(sentence).Count - 2
                            For s = Me.ChannelData(PtwfChannel)(sentence)(WordIndex)(FirstPtwfPhonemeIndexToMeasure).StartSample To Me.ChannelData(PtwfChannel)(sentence)(WordIndex)(LastPtwfPhonemeIndexToMeasure).StartSample + Me.ChannelData(PtwfChannel)(sentence)(WordIndex)(LastPtwfPhonemeIndexToMeasure).Length - 1
                                NewArray(CurrentReadSample) = InputSound.WaveData.SampleData(SoundChannel)(s)
                                CurrentReadSample += 1
                            Next
                        Next

                        Return DSP.MeasureSectionLevel(MeasurementSound, SoundChannel,,,,, FrequencyWeighting)

                    Else
                        Error ("No input sound set!")
                    End If
                Catch ex As Exception
                    Throw New Exception("An error occurred: " & ex.ToString)
                End Try

            End Function


#End Region


            Public Class SmaComponent
                Inherits List(Of SmaComponent)

                Public Property ParentSMA As SpeechMaterialAnnotation

                Public Property SmaTag As SpeechMaterialAnnotation.SmaTags

                Public Property OrthographicForm As String = ""
                Public Property PhoneticForm As String = ""

                Public Property StartSample As Integer = -1
                Public Property Length As Integer = 0

                'Sound level properties. Nothing if never set
                Public Property UnWeightedLevel As Double? = Nothing
                Public Property UnWeightedPeakLevel As Double? = Nothing

                Public Property FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z

                Public Property TimeWeighting As Double = 0 'A time weighting of 0 indicates "no time weighting", thus the average RMS level is indicated.

                Public Property WeightedLevel As Double? = Nothing

                ''' <summary>
                ''' The level of a set of frequency bands defined in BandCentreFrequencies and BandWidths. (Introduced in SMA v 1.1)
                ''' </summary>
                ''' <returns></returns>
                Public Property BandLevels As Double() = Nothing
                ''' <summary>
                ''' The centre frequencies of the bands for which the level is given in BandLevels. (Introduced in SMA v 1.1)
                ''' </summary>
                ''' <returns></returns>
                Public Property CentreFrequencies As Double() = Nothing
                ''' <summary>
                ''' The band widths of the bands for which the level is given in BandLevels. (Introduced in SMA v 1.1)
                ''' </summary>
                ''' <returns></returns>
                Public Property BandWidths As Double() = Nothing


                ''' <summary>
                ''' Indicates the initial absolute peak amplitude, i.e. the absolute value of the most negative or the positive sample value, of the segment. The initial peak value should only be stored once for each segment, 
                ''' directly after segment segmentation, but prior to any gain modification. As such, the initial peak value may be utilized to calculate any gain changes applied to the sound section associated with a specific segment since the initial recording.
                ''' Its default value is -1. Before any gain is applied to such a segment the InitialPeak should be measured using the method SetInitialPeakAmplitude. And once it has been measured (i.e. has a value other than -1), it should not be measured again. 
                ''' In version 1.0, this value was only allowed for sentence tags, since verion 1.1, it is allowed for every type of segment.
                ''' </summary>
                ''' <returns></returns>
                Public Property InitialPeak As Double = -1

                ''' <summary>
                ''' Time in seconds relative to a user defined point in time.
                ''' In version 1.0, this value was only allowed for sentence tags, since verion 1.1, it is allowed for every type of segment.
                ''' </summary>
                ''' <returns></returns>
                Public Property StartTime As Double


                Private Shared DefaultNotMeasuredValue As String = "Not measured"

                Public Sub New(ByRef ParentSMA As SpeechMaterialAnnotation, ByVal SmaLevel As SmaTags)
                    Me.ParentSMA = ParentSMA
                    Me.SmaTag = SmaLevel
                    Me.FrequencyWeighting = ParentSMA.GetFrequencyWeighting
                    Me.TimeWeighting = ParentSMA.GetTimeWeighting
                End Sub

                Public Function GetFrequencyWeighting() As FrequencyWeightings
                    Return FrequencyWeighting
                End Function

                Public Sub SetFrequencyWeighting(ByVal FrequencyWeighting As FrequencyWeightings, ByVal EnforceOnAllDescendents As Boolean)

                    'Setting only SMA top level frequency weighting
                    FrequencyWeighting = FrequencyWeighting

                    If EnforceOnAllDescendents = True Then
                        'Enforcing the same frequency weighting on all descendant phones
                        For Each child In Me
                            child.SetFrequencyWeighting(FrequencyWeighting, EnforceOnAllDescendents)
                        Next
                    End If
                End Sub

                Public Function GetTimeWeighting() As Double
                    Return TimeWeighting
                End Function

                Public Sub SetTimeWeighting(ByVal TimeWeighting As Double, ByVal EnforceOnAllDescendents As Boolean)

                    'Setting only SMA top level Time weighting
                    TimeWeighting = TimeWeighting

                    If EnforceOnAllDescendents = True Then

                        'Enforcing the same Time weighting on all descendant phones
                        For Each child In Me
                            child.SetTimeWeighting(TimeWeighting, EnforceOnAllDescendents)
                        Next
                    End If
                End Sub

                ''' <summary>
                ''' Returns a comma separated string representing the BandLevels
                ''' </summary>
                ''' <returns></returns>
                Public Function GetBandLevelsString() As String
                    Return GetCommaSeparatedArray(BandLevels)
                End Function

                ''' <summary>
                ''' Returns a comma separated string representing the CentreFrequencies
                ''' </summary>
                ''' <returns></returns>
                Public Function GetCentreFrequenciesString() As String
                    Return GetCommaSeparatedArray(CentreFrequencies)
                End Function

                ''' <summary>
                ''' Returns a comma separated string representing the BandWidths
                ''' </summary>
                ''' <returns></returns>
                Public Function GetBandWidthsString() As String
                    Return GetCommaSeparatedArray(BandWidths)
                End Function

                Private Function GetCommaSeparatedArray(ByRef InputArray() As Double) As String

                    If InputArray Is Nothing Then Return ""
                    If InputArray.Count = 0 Then Return ""
                    Dim OutputList As New List(Of String)
                    For Each v In InputArray
                        OutputList.Add(v.ToString(InvariantCulture))
                    Next
                    Return String.Join(",", OutputList)

                End Function

                Public Sub SetBandLevelsFromString(ByVal CommaSeparatedValues As String)
                    BandLevels = GetValuesFromCommaSeparatedString(CommaSeparatedValues)
                End Sub

                Public Sub SetCentreFrequenciesFromString(ByVal CommaSeparatedValues As String)
                    CentreFrequencies = GetValuesFromCommaSeparatedString(CommaSeparatedValues)
                End Sub

                Public Sub SetBandWidthsFromString(ByVal CommaSeparatedValues As String)
                    BandWidths = GetValuesFromCommaSeparatedString(CommaSeparatedValues)
                End Sub

                Private Function GetValuesFromCommaSeparatedString(ByVal CommaSeparatedValues As String) As Double()

                    If CommaSeparatedValues Is Nothing Then Return {}
                    If CommaSeparatedValues.Count = 0 Then Return {}
                    Dim OutputList As New List(Of Double)
                    Dim SplitList() As String = CommaSeparatedValues.Trim.Split(",")
                    For Each v In SplitList
                        OutputList.Add(CDbl(v.Trim))
                    Next
                    Return OutputList.ToArray

                End Function

                ''' <summary>
                ''' Converts all instances of a specified phone to a new phone
                ''' </summary>
                ''' <param name="CurrentPhone"></param>
                ''' <param name="NewPhone"></param>
                Public Sub ConvertPhone(ByVal CurrentPhone As String, ByVal NewPhone As String)
                    For Each childComponent In Me

                        'Replacing the phone
                        childComponent.PhoneticForm = childComponent.PhoneticForm.Replace(CurrentPhone, NewPhone)

                        'Cascading to all lower levels
                        childComponent.ConvertPhone(CurrentPhone, NewPhone)

                    Next

                End Sub



                Public Shadows Sub ToString(ByRef HeadingList As List(Of String), ByRef OutputList As List(Of String))

                    'Writing sentence data
                    HeadingList.Add("ORTHOGRAPHIC_FORM")
                    HeadingList.Add("PHONETIC_FORM")
                    HeadingList.Add("START_SAMPLE")
                    HeadingList.Add("LENGTH")
                    HeadingList.Add("UNWEIGHTED_LEVEL")
                    HeadingList.Add("UNWEIGHTED_PEAKLEVEL")
                    HeadingList.Add("WEIGHTED_LEVEL")
                    HeadingList.Add("FREQUENCY_WEIGHTING")
                    HeadingList.Add("TIME_WEIGHTING")
                    HeadingList.Add("INITIAL_PEAK")
                    HeadingList.Add("START_TIME")

                    'Writing sentence data
                    OutputList.Add(OrthographicForm)
                    OutputList.Add(PhoneticForm)
                    OutputList.Add(StartSample)
                    OutputList.Add(Length)
                    If UnWeightedLevel IsNot Nothing Then
                        OutputList.Add(UnWeightedLevel.Value.ToString(InvariantCulture))
                    Else
                        OutputList.Add(DefaultNotMeasuredValue)
                    End If

                    If UnWeightedPeakLevel IsNot Nothing Then
                        OutputList.Add(UnWeightedPeakLevel.Value.ToString(InvariantCulture))
                    Else
                        OutputList.Add(DefaultNotMeasuredValue)
                    End If

                    If WeightedLevel IsNot Nothing Then
                        OutputList.Add(WeightedLevel.Value.ToString(InvariantCulture))
                    Else
                        OutputList.Add(DefaultNotMeasuredValue)
                    End If
                    OutputList.Add(GetFrequencyWeighting.ToString)
                    OutputList.Add(GetTimeWeighting.ToString(InvariantCulture))

                    OutputList.Add(InitialPeak.ToString(InvariantCulture))
                    OutputList.Add(StartTime.ToString(InvariantCulture))

                    For n = 0 To Me.Count - 1

                        HeadingList.Add(Me(n).SmaTag.ToString)
                        OutputList.Add(n)

                        'Cascading to lower levels
                        Me(n).ToString(HeadingList, OutputList)

                    Next

                End Sub

                Private Function CheckStartAndLength() As Boolean

                    'Checks to see that start sample and length are assigned before sound measurements
                    If (Me.StartSample < 0 Or Me.Length < 0) Then
                        'Warnings("Cannot measure sound since one or both of StartSample or Length has not been assigned a value.")
                        Return False
                    Else
                        Return True
                    End If

                End Function


                ''' <summary>
                ''' Measures sound levels for each channel, sentence, word and phone of the current SMA object.
                ''' </summary>
                ''' <param name="c">The channel index (1-based)</param>
                ''' <param name="AttemptedMeasurementCount">The number of attempted measurements</param>
                ''' <param name="SuccesfullMeasurementsCount">The number of successful measurements</param>
                Public Sub MeasureSoundLevels(ByVal c As Integer, ByRef AttemptedMeasurementCount As Integer, ByRef SuccesfullMeasurementsCount As Integer)

                    Dim ParentSound = ParentSMA.ParentSound

                    'Checks that parent sound has enough channels
                    If ParentSound.WaveFormat.Channels >= c Then

                        'Checks that parent the current channel of the parent sound contains sounds
                        If ParentSound.WaveData.SampleData(c).Length > 0 Then

                            ' Naturally, do not attempt to measure the segment if it is marked as a word end.
                            If PhoneticForm = WordEndString Or OrthographicForm = WordEndString Then

                                ' It's marked as a word and, setting sound levels to Nothing
                                UnWeightedLevel = Nothing
                                UnWeightedPeakLevel = Nothing
                                WeightedLevel = Nothing

                            Else

                                'Measuring sound levels

                                'Measuring UnWeightedLevel
                                UnWeightedLevel = Nothing
                                UnWeightedLevel = DSP.MeasureSectionLevel(ParentSound, c, 0, Nothing, SoundDataUnit.dB)
                                AttemptedMeasurementCount += 1
                                If UnWeightedLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

                                'Meaures UnWeightedPeakLevel
                                UnWeightedPeakLevel = Nothing
                                UnWeightedPeakLevel = DSP.MeasureSectionLevel(ParentSound, c, 0, , SoundDataUnit.dB, SoundMeasurementType.AbsolutePeakAmplitude)
                                AttemptedMeasurementCount += 1
                                If UnWeightedPeakLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

                                'Measures weighted level
                                WeightedLevel = Nothing
                                If GetTimeWeighting() <> 0 Then
                                    WeightedLevel = DSP.GetLevelOfLoudestWindow(ParentSound, c,
                                                                                     GetTimeWeighting() * ParentSound.WaveFormat.SampleRate,
                                                                                      0, Nothing, , GetFrequencyWeighting, True)
                                Else
                                    WeightedLevel = DSP.MeasureSectionLevel(ParentSound, c, 0, Nothing, SoundDataUnit.dB, SoundMeasurementType.RMS, GetFrequencyWeighting)
                                End If
                                AttemptedMeasurementCount += 1
                                If WeightedLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

                            End If

                        Else
                            'Notes a missing measurement
                            AttemptedMeasurementCount += 1
                        End If
                    Else
                        'Notes a missing measurement
                        AttemptedMeasurementCount += 1
                    End If

                    'Cascading to lower levels
                    For Each childComponent In Me
                        childComponent.MeasureSoundLevels(c, AttemptedMeasurementCount, SuccesfullMeasurementsCount)
                    Next

                End Sub



                ''' <summary>
                ''' Measures the unalterred (original recording) absolute peak amplitude (linear value) within the word parts of a segmented audio recording. 
                ''' At least WordStartSample and WordLength of all words must be set prior to calling this function.
                ''' </summary>
                ''' <param name="MeasurementSound">The sound to measure.</param>
                ''' <param name="c">The channel index (1-based)</param>
                ''' <param name="AttemptedMeasurementCount">The number of attempted measurements</param>
                ''' <param name="SuccesfullMeasurementsCount">The number of successful measurements</param>
                Public Sub SetInitialPeakAmplitudes(ByVal MeasurementSound As Sound, ByVal c As Integer, ByRef AttemptedMeasurementCount As Integer, ByRef SuccesfullMeasurementsCount As Integer)

                    'Checks that parent sound has enough channels
                    If MeasurementSound.WaveFormat.Channels >= c Then

                        'Checks that parent the current channel of the parent sound contains sounds
                        If MeasurementSound.WaveData.SampleData(c).Length > 0 Then

                            ' Naturally, do not attempt to measure the segment if it is marked as a word end.
                            If PhoneticForm = WordEndString Or OrthographicForm = WordEndString Then

                                ' It's marked as a word and, setting sound levels to Nothing
                                UnWeightedLevel = Nothing
                                UnWeightedPeakLevel = Nothing
                                WeightedLevel = Nothing

                            Else

                                If InitialPeak <> -1 Then
                                    'Aborting measurements if the current channel InitialPeak is already set (-1 is the default/unmeasured value)
                                    'Measures UnWeightedPeakLevel of each word using Z-weighting
                                    Dim UnWeightedPeakLevel As Double?
                                    UnWeightedPeakLevel = DSP.MeasureSectionLevel(MeasurementSound, c, StartSample, Length, SoundDataUnit.linear, SoundMeasurementType.AbsolutePeakAmplitude)
                                    AttemptedMeasurementCount += 1
                                    If UnWeightedPeakLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

                                    'Setting InitialPeak to the highest detected so far
                                    InitialPeak = Math.Max(InitialPeak, CDbl(UnWeightedPeakLevel))
                                End If

                            End If

                        Else
                            'Notes a missing measurement
                            AttemptedMeasurementCount += 1
                        End If
                    Else
                        'Notes a missing measurement
                        AttemptedMeasurementCount += 1
                    End If

                    'Cascading to lower levels
                    For Each childComponent In Me
                        childComponent.SetInitialPeakAmplitudes(MeasurementSound, c, AttemptedMeasurementCount, SuccesfullMeasurementsCount)
                    Next

                End Sub



                ''' <summary>
                ''' Resets the sound levels of the component and cascading resets to all lower levels.
                ''' </summary>
                Public Sub ResetSoundLevels()

                    UnWeightedLevel = Nothing
                    UnWeightedPeakLevel = Nothing
                    WeightedLevel = Nothing

                    For Each childcomponent In Me
                        childcomponent.ResetSoundLevels()
                    Next

                End Sub


                ''' <summary>
                ''' This sub adds a word end string to the the phonetic and orthographic forms of all descentands of the last stored child component. However, if a word end marker does already exists, it is not added.
                ''' </summary>
                Public Sub AddWordEndString()

                    'Adding word-end marker

                    'Checks if there is already a word end marker
                    If Me(Me.Count - 1).PhoneticForm = WordEndString Then
                        'There is already a word end marker stored (in a previous segmentation).
                    Else
                        Me.Add(New SmaComponent(Me.ParentSMA, Me.SmaTag + 1) With {.PhoneticForm = WordEndString, .OrthographicForm = WordEndString})

                        'Positions the word end marker, according to the information stored in the previous component, if there is any
                        If Me.Count > 1 Then
                            Me(Me.Count - 1).StartSample = Me(Me.Count - 2).StartSample + Me(Me.Count - 2).Length
                        End If

                    End If

                    'Cascading to lower levels
                    Me(Me.Count - 1).AddWordEndString()

                End Sub

                ''' <summary>
                ''' This sub removes word end component based defined by the presence of word end strings in the phonetic or orthographic forms in all descending final components.
                ''' </summary>
                Public Sub RemoveWordEndString()

                    'Removing from the lowerst level first
                    If Me.Count > 0 Then
                        Me(Me.Count - 1).RemoveWordEndString()
                    End If

                    'Removing based on PhoneticForm 
                    If Me.Count > 0 Then
                        If Me(Me.Count - 1).PhoneticForm = WordEndString Then
                            Me.RemoveAt(Me.Count - 1)
                        End If
                    End If

                    'Removing based on OrthographicForm 
                    If Me.Count > 0 Then
                        If Me(Me.Count - 1).OrthographicForm = WordEndString Then
                            Me.RemoveAt(Me.Count - 1)
                        End If
                    End If

                End Sub

                ''' <summary>
                ''' Resets the segmentation time data (startSample and Length) of the component and cascading resets to all lower levels.
                ''' </summary>
                Public Sub ResetTemporalData()

                    StartSample = -1
                    Length = 0

                    For Each childcomponent In Me
                        childcomponent.ResetTemporalData()
                    Next

                End Sub

            End Class

        End Class

    End Class



End Namespace
