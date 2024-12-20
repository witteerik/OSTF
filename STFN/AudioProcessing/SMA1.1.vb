Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Xml
Imports System.Globalization.CultureInfo
Imports STFN.SpeechMaterialComponent
Imports System.Xml.Serialization
Imports System.Runtime.Serialization

Namespace Audio

    Partial Public Class Sound


        ''' <summary>
        ''' A class used to store data related to the segmentation of speech material sound files. All data can be contained and saved in a wave file.
        ''' </summary>
        <Serializable>
        Public Class SpeechMaterialAnnotation

            <XmlIgnore>
            Private ChangeDetector As Utils.ObjectChangeDetector

            Public Sub StoreUnchangedState()
                ChangeDetector = New Utils.ObjectChangeDetector(Me)
                ChangeDetector.SetUnchangedState()
            End Sub

            Public Function IsChanged() As Boolean?
                If ChangeDetector IsNot Nothing Then
                    Return ChangeDetector.IsChanged()
                Else
                    Return Nothing
                End If
            End Function

            <XmlIgnore>
            Public ParentSound As Sound

            Public Const CurrentVersion As String = "1.1"
            ''' <summary>
            ''' Holds the SMA version of the file that the data was loaded from, or CurrentVersion if the data was not loaded from file.
            ''' </summary>
            Public ReadFromVersion As String = CurrentVersion ' Using CurrentVersion as default

            ''' <summary>
            ''' The nominal level describes the speech material level with correction applied, thus not necessarily representing the actual level of the speech material, but should exactly represents the level of the calibration signal intended for use with the material.
            ''' </summary>
            Public Property NominalLevel As Double? = Nothing

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

            Public Sub AddChannelData(Optional ByVal SourceFilePath As String = "")
                _ChannelData.Add(New SmaComponent(Me, SmaTags.CHANNEL, Nothing, SourceFilePath))
            End Sub

            Public Sub AddChannelData(ByRef NewSmaChannelData As SmaComponent)
                _ChannelData.Add(NewSmaChannelData)
            End Sub

            ''' <summary>
            ''' This private sub is intended to be used only when an object of the current class is cloned by Xml serialization, such as with CreateCopy. 
            ''' </summary>
            Private Sub New()

            End Sub

            ''' <summary>
            ''' Creates a new instance of SpeechMaterialAnnotation
            ''' </summary>
            Public Sub New(Optional ByVal DefaultFrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z,
                       Optional ByVal DefaultTimeWeighting As Double = 0)
                SetFrequencyWeighting(DefaultFrequencyWeighting, False)
                SetTimeWeighting(DefaultTimeWeighting, False)
            End Sub

            ''' <summary>
            ''' Enforcing the nominal level set in the current instance of SpeechMaterialAnnotation to all descendant channels, sentences, words, and phones.
            ''' This method should be called when loading SMA from wav files and after the nominal level has been set in the SpeechMaterialAnnotation object.
            ''' </summary>
            Public Sub InferNominalLevelToAllDescendants()
                'N.B. The reason that this is not instead doe in the set method of the NominalLevel property is that when the NominalLevel property value is set, not all descendant SmaComponents are necessarily loaded/attached.
                For Each c In _ChannelData
                    c.InferNominalLevelToAllDescendants(NominalLevel)
                Next
            End Sub

            Private FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z
            Public Function GetFrequencyWeighting() As FrequencyWeightings
                Return FrequencyWeighting
            End Function

            Public Sub SetFrequencyWeighting(ByVal FrequencyWeighting As FrequencyWeightings, ByVal EnforceOnAllDescendents As Boolean)

                'Setting only SMA top level frequency weighting
                Me.FrequencyWeighting = FrequencyWeighting

                If EnforceOnAllDescendents = True Then
                    'Enforcing the same frequency weighting on all descendant channels, sentences, words, and phones
                    For Each c In _ChannelData
                        c.SetFrequencyWeighting(FrequencyWeighting, EnforceOnAllDescendents)
                    Next
                End If
            End Sub

            Private TimeWeighting As Double = 0
            Public Function GetTimeWeighting() As Double
                Return TimeWeighting
            End Function

            Public Sub SetTimeWeighting(ByVal TimeWeighting As Double, ByVal EnforceOnAllDescendents As Boolean)

                'Setting only SMA top level Time weighting
                Me.TimeWeighting = TimeWeighting

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

                For c = 0 To _ChannelData.Count - 1

                    HeadingList.Add("CHANNEL")
                    OutputList.Add(c + 1)

                    _ChannelData(c).ToString(HeadingList, OutputList)
                Next

                If IncludeHeadings = True Then
                    Return String.Join(vbTab, HeadingList) & vbCrLf & String.Join(vbCrLf, OutputList)
                Else
                    Return String.Join(vbTab, OutputList)
                End If

            End Function


            Public Shadows Function GetSentenceSegmentationsString(ByVal IncludeHeadings As Boolean) As String

                Dim HeadingList As New List(Of String)
                Dim OutputList As New List(Of String)

                If IncludeHeadings = True Then
                    HeadingList.Add("Channel")
                    HeadingList.Add("SentenceIndex")
                    HeadingList.Add("OrthographicForm")
                    HeadingList.Add("PhoneticForm")
                    HeadingList.Add("StartTimeSec")
                    HeadingList.Add("DurationSec")
                    HeadingList.Add("StartSample")
                    HeadingList.Add("LengthSamples")
                    HeadingList.Add("SegmentationCompleted")
                End If

                For c = 0 To _ChannelData.Count - 1
                    For s = 0 To _ChannelData(c).Count - 1
                        Dim Sentence = _ChannelData(c)(s)
                        Dim SentenceList As New List(Of String)
                        SentenceList.Add(c)
                        SentenceList.Add(s)
                        SentenceList.Add(Sentence.OrthographicForm)
                        SentenceList.Add(Sentence.PhoneticForm)
                        SentenceList.Add(Sentence.StartTime)
                        SentenceList.Add(Sentence.Length / ParentSound.WaveFormat.SampleRate)
                        SentenceList.Add(Sentence.StartSample)
                        SentenceList.Add(Sentence.Length)
                        SentenceList.Add(Sentence.SegmentationCompleted.ToString)
                        OutputList.Add(String.Join(vbTab, SentenceList))
                    Next
                Next

                If IncludeHeadings = True Then
                    Return String.Join(vbTab, HeadingList) & vbCrLf & String.Join(vbCrLf, OutputList)
                Else
                    Return String.Join(vbCrLf, OutputList)
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
            Public Function MeasureSoundLevels(Optional ByVal IncludeCriticalBandLevels As Boolean = False, Optional ByVal LogMeasurementResults As Boolean = False, Optional ByVal LogFolder As String = "") As Boolean

                If ParentSound Is Nothing Then
                    Throw New Exception("The parent sound if the current instance of SpeechMaterialAnnotation cannot be Nothing!")
                End If

                Dim SuccesfullMeasurementsCount As Integer = 0
                Dim AttemptedMeasurementCount As Integer = 0

                'Measuring each channel 
                For c As Integer = 1 To ChannelCount
                    ChannelData(c).MeasureSoundLevels(IncludeCriticalBandLevels, c, AttemptedMeasurementCount, SuccesfullMeasurementsCount)
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
            ''' <returns>Returns True if all measurements were successful, and False if one or more measurements failed.</returns>
            Public Function SetInitialPeakAmplitudes() As Boolean

                If ParentSound Is Nothing Then
                    Throw New Exception("The parent sound if the current instance of SpeechMaterialAnnotation cannot be Nothing!")
                End If

                Dim SuccesfullMeasurementsCount As Integer = 0
                Dim AttemptedMeasurementCount As Integer = 0

                'Measuring each channel 
                For c As Integer = 1 To ChannelCount
                    ChannelData(c).SetInitialPeakAmplitudes(ParentSound, c, AttemptedMeasurementCount, SuccesfullMeasurementsCount)
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
            ''' This sub resets segmentation time data (startSample and Length) stored in the current SMA object.
            ''' </summary>
            Public Sub ResetTemporalData()

                For Each c In _ChannelData
                    c.ResetTemporalData()
                Next

            End Sub

            ''' <summary>
            ''' Checks the SegmentationCompleted value of all descentant SmaComponents in the indicated channel is True. Otherwise returns False .
            ''' </summary>
            ''' <returns></returns>
            Public Function AllSegmentationsCompleted(ByVal Channel As Integer) As Boolean

                Dim AllComponents = ChannelData(Channel).GetAllDescentantComponents

                If AllComponents Is Nothing Then
                    Return False
                Else
                    For Each c In AllComponents
                        If c.SegmentationCompleted = False Then Return False
                    Next
                End If

                Return True

            End Function


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
            ''' Shifts all StartSample indices in the current instance of SpeechMaterialAnnotation located at or after FirstSampleToShift, and limits the StartSample and Length values to the sample indices available in the parent sound file
            ''' </summary>
            ''' <param name="ShiftInSamples"></param>
            '''   <param name="FirstSampleToShift">Applies shift to all StartSample values after FirstSampleToShift.</param>
            Public Sub ShiftSegmentationData(ByVal ShiftInSamples As Integer, ByVal FirstSampleToShift As Integer, Optional ByVal AllowInfinitePositiveShift As Boolean = False)

                For c = 1 To Me.ChannelCount
                    Dim Channel = Me.ChannelData(c)
                    Dim TotalAvailableLength As Integer
                    If AllowInfinitePositiveShift = True Then
                        TotalAvailableLength = Integer.MaxValue
                    Else
                        TotalAvailableLength = ParentSound.WaveData.SampleData(c).Length
                    End If
                    'Shifting channel is no longer needed as its value are always 0 and the sound length
                    'ApplyShift(ShiftInSamples, SoundChannelLength, Channel.StartSample, Channel.Length, FirstSampleToShift)
                    For Each Sentence In Channel
                        ApplyShift(ShiftInSamples, TotalAvailableLength, Sentence.StartSample, Sentence.Length, FirstSampleToShift)
                        For Each Word In Sentence
                            ApplyShift(ShiftInSamples, TotalAvailableLength, Word.StartSample, Word.Length, FirstSampleToShift)
                            For Each Phone In Word
                                ApplyShift(ShiftInSamples, TotalAvailableLength, Phone.StartSample, Phone.Length, FirstSampleToShift)
                            Next
                        Next
                    Next
                Next

            End Sub


            ''' <summary>
            ''' Shifts all StartSample indices located at or after FirstSampleToShift in the current instance of SpeechMaterialAnnotation, and limits the StartSample and Length values to the sample indices available in the parent sound file
            ''' </summary>
            ''' <param name="Shift"></param>
            ''' <param name="TotalAvailableLength"></param>
            ''' <param name="StartIndex"></param>
            ''' <param name="Length"></param>
            ''' <param name="FirstSampleToShift"></param>
            Private Sub ApplyShift(ByVal Shift As Integer, ByVal TotalAvailableLength As Integer, ByRef StartIndex As Integer, ByRef Length As Integer, ByVal FirstSampleToShift As Integer)

                'MsgBox("Check the code below for accuracy!!!")

                If StartIndex >= FirstSampleToShift Then

                    'Adjusting the StartSample and limiting it to the available range
                    StartIndex = Math.Min(Math.Max(StartIndex + Shift, 0), TotalAvailableLength - 1)

                    'Limiting Length to the available length after adjustment of startsample
                    Dim MaximumPossibleLength = TotalAvailableLength - StartIndex
                    Length = Math.Max(0, Math.Min(Length, MaximumPossibleLength))
                End If

            End Sub

            ''' <summary>
            ''' Fading the padded sections before the first sentnce and after the last sentence.
            ''' </summary>
            ''' <param name="Sound"></param>
            ''' <param name="CurrentChannel"></param>
            ''' <param name="FadeType"></param>
            ''' <param name="CosinePower"></param>
            Public Sub FadePaddingSection(ByRef Sound As Sound, ByRef CurrentChannel As Integer,
                                  Optional FadeType As DSP.FadeSlopeType = DSP.FadeSlopeType.PowerCosine_SkewedFromFadeDirection,
                                  Optional CosinePower As Double = 100)

                If CurrentChannel <= Sound.SMA.ChannelCount Then
                    If CurrentChannel <= Sound.WaveFormat.Channels Then

                        Dim FirstSentenceStartSample As Integer = Sound.SMA.ChannelData(CurrentChannel)(0).StartSample
                        Dim LastSentenceIndex As Integer = Sound.SMA.ChannelData(CurrentChannel).Count - 1
                        Dim LastSentenceEndSample As Integer = Sound.SMA.ChannelData(CurrentChannel)(LastSentenceIndex).StartSample + Sound.SMA.ChannelData(CurrentChannel)(LastSentenceIndex).Length

                        'Fading in start
                        DSP.Fade(Sound, , 0, CurrentChannel, 0, FirstSentenceStartSample - 1, FadeType, CosinePower)

                        'Fading out end
                        DSP.Fade(Sound, 0, , CurrentChannel, LastSentenceEndSample + 1,, FadeType, CosinePower)

                    Else
                        MsgBox("The current sound does not contain the requested channel: " & CurrentChannel)
                    End If
                Else
                    MsgBox("The current SMA object does not contain the requested channel: " & CurrentChannel)
                End If

            End Sub

            ''' <summary>
            ''' This method may be used to facilitate manual segmentation, as it suggests appropriate sentence boundary positions, based on a rough initially segmentation.
            '''The method works by 
            ''' a) detecting the speech location by assuming that the loudest window in the initially segmented sentence section will be found inside the actual sentence.
            ''' b) detecting sentence start by locating the centre of the last window of a silent section of at least LongestSilentSegment milliseconds.
            ''' c) detecting sentence end by locating the centre of the first window of a silent section of at least LongestSilentSegment milliseconds.
            ''' </summary>
            ''' <param name="InitialPadding">Time section in seconds included before the detected start position.</param>
            ''' <param name="FinalPadding">Time section in seconds included after the detected end position.</param>
            ''' <param name="SilenceDefinition">The definition of a silent window is set to SilenceDefinition dB lower that the loudest detected window in the initially segmented sentence section.</param>
            Public Sub DetectSpeechBoundaries(ByVal CurrentChannel As Integer,
                                              Optional ByVal InitialPadding As Double = 0,
                                              Optional ByVal FinalPadding As Double = 0,
                                              Optional ByVal SilenceDefinition As Double = 25,
                                              Optional ByVal SetToZeroCrossings As Boolean = True)

                Try



                    Dim TotalSoundLength = Me.ParentSound.WaveData.SampleData(CurrentChannel).Length

                    'Low-pass filter
                    Dim LpFirFilter_FftFormat = New Formats.FftFormat(1024 * 4,,,, True)
                    Dim ReusedLpFirKernel As Audio.Sound = Audio.GenerateSound.CreateSpecialTypeImpulseResponse(Me.ParentSound.WaveFormat, LpFirFilter_FftFormat, 4000, , FilterType.LinearAttenuationAboveCF_dBPerOctave, 15,,,, 25,, True)

                    'High-pass filterring the sound to reduce vibration influences
                    Dim HpFirFilter_FftFormat = New Formats.FftFormat(1024 * 4,,,, True)
                    Dim ReusedHpFirKernel As Audio.Sound = Audio.GenerateSound.CreateSpecialTypeImpulseResponse(Me.ParentSound.WaveFormat, HpFirFilter_FftFormat, 4000, , FilterType.LinearAttenuationBelowCF_dBPerOctave, 100,,,, 25,, True)

                    For s = 0 To Me.ChannelData(CurrentChannel).Count - 1

                        Dim SentenceSmaComponent = Me.ChannelData(CurrentChannel)(s)

                        Dim SentenceSoundCopy = SentenceSmaComponent.GetSoundFileSection(CurrentChannel)

                        Dim FilterredSound As Audio.Sound = Audio.DSP.TransformationsExt.FIRFilter(SentenceSoundCopy, ReusedHpFirKernel, HpFirFilter_FftFormat,,,,,, True, True)

                        'Detecting the start position, and level, of the loudest 25 ms window
                        Dim WindowSize As Integer = 0.025 * ParentSound.WaveFormat.SampleRate

                        'Measuring sentence sound level
                        Dim LoudestWindowStartSample As Integer
                        Dim WindowLevelList As New List(Of Double)
                        Dim LoudestWindowLevel As Double = DSP.GetLevelOfLoudestWindow(FilterredSound, CurrentChannel, WindowSize,,, LoudestWindowStartSample,,, WindowLevelList)

                        'Smoothing the WindowLevelList (using FIR filter)
                        Dim SmoothNonSound As New Audio.Sound(SentenceSoundCopy.WaveFormat)
                        Dim WindowLevelList_Single As New List(Of Single)
                        For Each Value In WindowLevelList
                            WindowLevelList_Single.Add(Value)
                        Next
                        SmoothNonSound.WaveData.SampleData(1) = WindowLevelList_Single.ToArray

                        'DSP.MaxAmplitudeNormalizeSection(SmoothNonSound, CurrentChannel)

                        'SmoothNonSound.WriteWaveFile("C:\Temp\PreFilt.wav")

                        Dim FilterredLevelArray As Audio.Sound = Audio.DSP.TransformationsExt.FIRFilter(SmoothNonSound, ReusedLpFirKernel, LpFirFilter_FftFormat,,,,,, True, True)

                        'ReusedLpFirKernel.WriteWaveFile("C:\Temp\Kern.wav")


                        'FilterredLevelArray.WriteWaveFile("C:\Temp\Filt.wav")

                        WindowLevelList.Clear()
                        For Each Value In FilterredLevelArray.WaveData.SampleData(1)
                            WindowLevelList.Add(Value)
                        Next

                        'Setting the value of silence definition
                        Dim SilenceLevel As Double = LoudestWindowLevel - SilenceDefinition

                        'Setting default start and end values
                        Dim StartSample As Integer = LoudestWindowStartSample
                        Dim EndSample As Integer = LoudestWindowStartSample

                        'Looking for the first non-silent window
                        Dim IterationStart As Integer = Math.Min(1024, WindowLevelList.Count - 1)
                        For w = IterationStart To LoudestWindowStartSample - 1
                            If WindowLevelList(w) > SilenceLevel Then
                                StartSample = Math.Min(w + Math.Floor(InitialPadding * SentenceSoundCopy.WaveFormat.SampleRate), SentenceSoundCopy.WaveData.SampleData(CurrentChannel).Length - 1)
                                Exit For
                            End If
                        Next

                        'Looking for the last non-silent window
                        Dim IterationStart2 As Integer = Math.Max(0, WindowLevelList.Count - 1 - 1024)
                        For w = IterationStart2 To LoudestWindowStartSample + 1 Step -1
                            If WindowLevelList(w) > SilenceLevel Then
                                EndSample = Math.Min(w + WindowSize + Math.Ceiling(FinalPadding * SentenceSoundCopy.WaveFormat.SampleRate), SentenceSoundCopy.WaveData.SampleData(CurrentChannel).Length - 1)
                                Exit For
                            End If
                        Next


                        If SetToZeroCrossings = True Then
                            'Setting to closets zero-crossings
                            StartSample = DSP.GetZeroCrossingSample(SentenceSoundCopy, CurrentChannel, StartSample, DSP.MeasurementsExt.SearchDirections.Earlier)
                            EndSample = DSP.GetZeroCrossingSample(SentenceSoundCopy, CurrentChannel, EndSample, DSP.MeasurementsExt.SearchDirections.Later)
                        End If

                        'Calculating new start and sentence length
                        Dim SoundFileRelativeStartSample = SentenceSmaComponent.StartSample + StartSample
                        Dim SentenceLength As Integer = Math.Max(0, EndSample - StartSample)

                        'Updating the sentence segmentation (and all dependent levels)
                        SentenceSmaComponent.MoveStart(SoundFileRelativeStartSample, TotalSoundLength)
                        SentenceSmaComponent.AlignSegmentationStartsAcrossLevels(TotalSoundLength)

                        If SentenceSmaComponent.StartSample + SentenceLength > TotalSoundLength Then
                            'Limiting length to stay within the length of the sound file
                            SentenceSmaComponent.Length = TotalSoundLength - SentenceSmaComponent.StartSample
                        Else
                            SentenceSmaComponent.Length = SentenceLength
                        End If
                        SentenceSmaComponent.AlignSegmentationEndsAcrossLevels()

                    Next

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try


            End Sub

            Public Sub ApplyPaddingSection(ByRef Sound As Sound, ByRef CurrentChannel As Integer, ByVal PaddingTime As Single)

                If CurrentChannel <= Sound.SMA.ChannelCount Then
                    If CurrentChannel <= Sound.WaveFormat.Channels Then

                        Dim FirstSentenceStartSample As Integer = Sound.SMA.ChannelData(CurrentChannel)(0).StartSample

                        'Converts PaddingTime to samples
                        Dim PaddingLength As Integer = Math.Floor(PaddingTime * Sound.WaveFormat.SampleRate)

                        'Fixing the end
                        'Determines the needed initial shift
                        Dim InitialShift As Integer = PaddingLength - FirstSentenceStartSample
                        'If InitialShift is positive, a forward shift is needed, thus:
                        ' - a silent sound with the length of InitialShift samples need to be inserted at the beginning of the sound
                        ' - InitialShift need to be added to the start samples of all SMA components 

                        'If InitialShift is negative, a backward shift is needed, thus:
                        ' - InitialShift samples need to cut out from the beginning of the sound
                        ' - InitialShift need to be subtracted from the start samples of all SMA components 

                        ' If InitialShift is zero, nothing need to be changed.
                        If InitialShift > 0 Then
                            DSP.InsertSilentSection(Sound, 0, InitialShift)
                            ShiftSegmentationData(InitialShift, 0)
                        ElseIf InitialShift < 0 Then
                            DSP.DeleteSection(Sound, 0, -InitialShift)
                            ShiftSegmentationData(InitialShift, 0)
                        End If

                        'Fixing the end
                        'Determines the final adjustment of the sound file
                        Dim SoundFileLength As Integer = Sound.WaveData.SampleData(CurrentChannel).Length
                        Dim LastSentenceIndex As Integer = Sound.SMA.ChannelData(CurrentChannel).Count - 1
                        Dim LastSentenceEndSample As Integer = Sound.SMA.ChannelData(CurrentChannel)(LastSentenceIndex).StartSample + Sound.SMA.ChannelData(CurrentChannel)(LastSentenceIndex).Length
                        Dim IntendedSoundLength As Integer = LastSentenceEndSample + PaddingLength
                        Dim FinalAdjustment As Integer = IntendedSoundLength - SoundFileLength
                        'If FinalAdjustment is positive, the sound file nedd to be shortened by FinalAdjustment samples 
                        'If FinalAdjustment is negative, the sound file nedd to be lengthed by FinalAdjustment samples 
                        ' No changes is needed to the SMA object
                        'If FinalAdjustment is zero no changes are needed
                        If FinalAdjustment > 0 Then
                            'NB. As changing only the CurrentChannel channel would create channels of differing in lengths in multi channel sounds!!! Avoiding this by also extending the other channels if there are any.
                            For c = 1 To Sound.WaveFormat.Channels
                                ReDim Preserve Sound.WaveData.SampleData(c)(IntendedSoundLength)
                            Next
                        ElseIf FinalAdjustment < 0 Then
                            DSP.DeleteSection(Sound, SoundFileLength + FinalAdjustment, -FinalAdjustment)
                        End If

                    Else
                        MsgBox("The current sound does not contain the requested channel: " & CurrentChannel)
                    End If
                Else
                    MsgBox("The current SMA object does not contain the requested channel: " & CurrentChannel)
                End If

            End Sub

            Public Sub ApplyInterSentenceInterval(ByVal Interval As Single, ByVal FadeInterval As Boolean, ByRef CurrentChannel As Integer,
                                  Optional FadeType As DSP.FadeSlopeType = DSP.FadeSlopeType.PowerCosine_SkewedFromFadeDirection,
                                  Optional CosinePower As Double = 100, Optional ByVal FadedMarginTime As Single = 0.05)

                Dim FadedMarginLength As Integer = Math.Floor(ParentSound.WaveFormat.SampleRate * FadedMarginTime)
                If FadedMarginLength Mod 2 = 1 Then FadedMarginLength -= 1 'Adjusts FadedMarginLength to an even value
                Dim IntervalLength As Integer = Math.Floor(ParentSound.WaveFormat.SampleRate * Interval)
                If IntervalLength Mod 2 = 1 Then IntervalLength -= 1 'Adjusts IntervalLength to an even value

                'Ensuring that the interval is large enough to room two faded margins
                If 2 * FadedMarginLength > IntervalLength Then
                    'Adjusting the FadedMarginLength to be half of the FadedMarginLength
                    FadedMarginLength = IntervalLength / 2
                End If

                'Creating a silent sound with the length of IntervalLength - 2 * FadedMarginLength to insert between the sentence level segments
                Dim SilenceLength As Integer = IntervalLength - 2 * FadedMarginLength
                Dim SilentSound = Audio.GenerateSound.CreateSilence(ParentSound.WaveFormat, CurrentChannel, SilenceLength, TimeUnits.samples)

                Dim SentenceSoundSections As New List(Of Sound)
                'Getting all sentence sound sections including a margin of FadedMarginTime

                Dim SoundChannelData As List(Of Single) = ParentSound.WaveData.SampleData(CurrentChannel).ToList

                For s = 0 To ChannelData(CurrentChannel).Count - 1

                    Dim sentence = ChannelData(CurrentChannel)(s)

                    Dim StartReadSample = sentence.StartSample - FadedMarginLength
                    Dim InitialAdjustment As Integer = 0
                    If StartReadSample < 0 Then
                        InitialAdjustment = -StartReadSample
                    End If
                    StartReadSample = Math.Max(0, StartReadSample)

                    Dim ReadLength = InitialAdjustment + sentence.Length + (2 * FadedMarginLength)
                    Dim FinalAdjustment As Integer = 0
                    If StartReadSample + ReadLength > SoundChannelData.Count Then
                        FinalAdjustment = Math.Abs((StartReadSample + ReadLength) - SoundChannelData.Count)
                    End If
                    ReadLength -= FinalAdjustment

                    Dim SentenceData = SoundChannelData.GetRange(StartReadSample, ReadLength).ToArray

                    'Adjusting for missing samples
                    If InitialAdjustment > 0 Then
                        Dim InitialAdjustmentArray(InitialAdjustment - 1) As Single
                        SentenceData = InitialAdjustmentArray.Concat(SentenceData).ToArray
                    End If
                    If FinalAdjustment > 0 Then
                        Dim FinalAdjustmentArray(FinalAdjustment - 1) As Single
                        SentenceData = SentenceData.Concat(FinalAdjustmentArray).ToArray
                    End If

                    Dim SentenceSound = New Sound(ParentSound.WaveFormat)
                    SentenceSound.WaveData.SampleData(CurrentChannel) = SentenceData

                    'Fading the margins
                    If FadeInterval = True Then
                        Audio.DSP.Fade(SentenceSound,, 0, CurrentChannel, 0, FadedMarginLength, FadeType, CosinePower)
                        Audio.DSP.Fade(SentenceSound, 0, , CurrentChannel, SentenceData.Length - FadedMarginLength, FadedMarginLength, FadeType, CosinePower)
                    End If

                    'Adding the sentence sound
                    SentenceSoundSections.Add(SentenceSound)

                    'Adding silence between sentnces (not adding after the last sentence
                    If s < ChannelData(CurrentChannel).Count - 1 Then
                        SentenceSoundSections.Add(SilentSound)
                    End If

                Next

                'Concatenates the sounds
                Dim ConcatenatedSound = Audio.DSP.ConcatenateSounds(SentenceSoundSections)

                'Storing the concatenated sound array in Me.ParentSound
                Me.ParentSound.WaveData.SampleData(CurrentChannel) = ConcatenatedSound.WaveData.SampleData(CurrentChannel)

                'Adjusts the SMA StartSample positions
                'Fixing the first shift. The new startsample will be at FadedMarginLength
                Dim AccumulativePostShiftStartSample As Integer = 0
                For s = 0 To Math.Min(0, ChannelData(CurrentChannel).Count - 1)

                    Dim PreShiftStartSample = ChannelData(CurrentChannel)(s).StartSample
                    AccumulativePostShiftStartSample = FadedMarginLength
                    Dim Shift = AccumulativePostShiftStartSample - PreShiftStartSample
                    Me.ShiftSegmentationData(Shift, PreShiftStartSample, True)

                Next

                For s = 1 To ChannelData(CurrentChannel).Count - 1

                    Dim PreShiftStartSample = ChannelData(CurrentChannel)(s).StartSample
                    AccumulativePostShiftStartSample += ChannelData(CurrentChannel)(s - 1).Length + IntervalLength
                    Dim Shift = AccumulativePostShiftStartSample - PreShiftStartSample
                    Me.ShiftSegmentationData(Shift, PreShiftStartSample, True)

                Next

                'Setting the channel length to the new sound data length. 'TODO: feeling intuitively that this should be done elsewhere in a more general way...???
                For s = 0 To ChannelData(CurrentChannel).Count - 1
                    ChannelData(CurrentChannel)(s).AlignSegmentationEndsAcrossLevels()
                Next


            End Sub

            Public Function CreateCopy(ByRef ParentSoundReference As Audio.Sound) As SpeechMaterialAnnotation

                'Creating an output object
                Dim newSmaData As SpeechMaterialAnnotation

                'Serializing to memorystream
                Dim serializedMe As New MemoryStream
                Dim serializer As New XmlSerializer(GetType(SpeechMaterialAnnotation))
                serializer.Serialize(serializedMe, Me)

                'Deserializing to new object
                serializedMe.Position = 0
                newSmaData = CType(serializer.Deserialize(serializedMe), SpeechMaterialAnnotation)
                serializedMe.Close()

                If ParentSoundReference IsNot Nothing Then
                    'Rerefereinging the parent sound
                    newSmaData.ParentSound = ParentSoundReference
                End If

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

            <Serializable>
            Public Class SmaComponent
                Inherits List(Of SmaComponent)

                Public Property ParentSMA As SpeechMaterialAnnotation

                Public Property ParentComponent As SmaComponent

                ''' <summary>
                ''' The nominal level describes the speech material level with correction applied, thus not necessarily representing the actual level of the speech material, but exaclty represent the level of the calibration signal intended for use with the material.
                ''' The value of nominal level is infered from the parent SMA object when read from a wave file, but never written back to wave files. Thus, to permenently alter the value of the nominal level, the nominal level value of the parent SMA object needs to be modified.
                ''' </summary>
                Public Property NominalLevel As Double? = Nothing

                Public Property SmaTag As SpeechMaterialAnnotation.SmaTags

                Private _SegmentationCompleted As Boolean = False

                Public ReadOnly Property SegmentationCompleted As Boolean
                    Get

                        'Always returns True for channel level Sma tags, at these need not be validated as they should always correspond to the start (i.e. 0) and length the wave channel data array
                        If Me.SmaTag = SmaTags.CHANNEL Then
                            _SegmentationCompleted = True
                        End If

                        Return _SegmentationCompleted

                    End Get
                End Property

                Public Property OrthographicForm As String = ""
                Public Property PhoneticForm As String = ""
                Private _StartSample As Integer = -1
                Public Property StartSample As Integer
                    Get
                        If Me.SmaTag = SmaTags.CHANNEL Then
                            'Channels start should always be 0
                            Return 0
                        Else
                            Return _StartSample
                        End If
                    End Get
                    Set(value As Integer)
                        _StartSample = value
                    End Set
                End Property

                Private _Length As Integer = 0
                Public Property Length As Integer
                    Get
                        'Channels should return sound channel length, or 0 if no sound exist
                        Dim LocalLength As Integer = _Length
                        If Me.SmaTag = SmaTags.CHANNEL Then
                            If ParentSMA IsNot Nothing Then
                                If ParentSMA.ParentSound IsNot Nothing Then
                                    Dim SmaChannel As Integer? = 1 '= Me.GetSelfIndex' TODO: Here we should specifiy the appropriate sound channel, but as of now, there is no way to get it. Use channel = 1 instead. This will break down if Sma annotations have multiple channels!
                                    If SmaChannel.HasValue Then
                                        LocalLength = ParentSMA.ParentSound.WaveData.SampleData(SmaChannel).Length
                                    End If
                                End If
                            End If
                        End If
                        Return LocalLength
                    End Get
                    Set(value As Integer)
                        _Length = value
                    End Set
                End Property

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
                ''' The integration times used for each band when calculating the levels given in BandLevels. (Introduced in SMA v 1.1)
                ''' </summary>
                ''' <returns></returns>
                Public Property BandIntegrationTimes As Double() = Nothing

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

                ''' <summary>
                ''' If applicable, holds the file path to the file from which the current instance of SmaComponent was read
                ''' </summary>
                Public SourceFilePath As String = ""

                Public Sub New(ByRef ParentSMA As SpeechMaterialAnnotation, ByVal SmaLevel As SmaTags, ByRef ParentComponent As SmaComponent, Optional ByVal SourceWaveFilePath As String = "")
                    Me.ParentSMA = ParentSMA
                    Me.ParentComponent = ParentComponent
                    Me.SmaTag = SmaLevel
                    Me.FrequencyWeighting = ParentSMA.GetFrequencyWeighting
                    Me.TimeWeighting = ParentSMA.GetTimeWeighting
                    Me.SourceFilePath = SourceWaveFilePath
                End Sub

                Public Sub InferNominalLevelToAllDescendants(ByVal NominalLevel As Double?)

                    'Setting the time weighting
                    Me.NominalLevel = NominalLevel

                    'Enforcing the same nominal level on all descendants
                    For Each child In Me
                        child.InferNominalLevelToAllDescendants(NominalLevel)
                    Next
                End Sub

                Public Function GetFrequencyWeighting() As FrequencyWeightings
                    Return FrequencyWeighting
                End Function

                Public Sub SetFrequencyWeighting(ByVal FrequencyWeighting As FrequencyWeightings, ByVal EnforceOnAllDescendents As Boolean)

                    'Setting the frequency weighting
                    Me.FrequencyWeighting = FrequencyWeighting

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

                    'Setting the time weighting
                    Me.TimeWeighting = TimeWeighting

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


                ''' <summary>
                ''' Returns a comma separated string representing the BandIntegrationTimes
                ''' </summary>
                ''' <returns></returns>
                Public Function GetBandIntegrationTimesString() As String
                    Return GetCommaSeparatedArray(BandIntegrationTimes)
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

                Public Sub SetBandIntegrationTimesFromString(ByVal CommaSeparatedValues As String)
                    BandIntegrationTimes = GetValuesFromCommaSeparatedString(CommaSeparatedValues)
                End Sub


                Private Function GetValuesFromCommaSeparatedString(ByVal CommaSeparatedValues As String) As Double()

                    Dim CDS = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator

                    If CommaSeparatedValues Is Nothing Then Return {}
                    If CommaSeparatedValues.Length = 0 Then Return {}
                    Dim OutputList As New List(Of Double)
                    Dim SplitList() As String = CommaSeparatedValues.Trim.Split(",")
                    For Each v In SplitList

                        Dim value As Double
                        If Double.TryParse(v.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                            OutputList.Add(value)
                        Else
                            Throw New Exception("Unable to parse the following string as a list of double: " & CommaSeparatedValues & " (This error may be cause by corrupt SMA specifications in iXML wave file chunks.)")
                        End If

                    Next
                    Return OutputList.ToArray

                End Function

                ''' <summary>
                ''' Can be used to get the orthographic form, if no orthographic form is set, the orthographic form of the closest child in an unbroken line of single children.
                ''' </summary>
                ''' <returns></returns>
                Public Function FindOrthographicForm() As String

                    If OrthographicForm <> "" Then
                        Return OrthographicForm
                    Else
                        If Me.Count = 1 Then
                            Return Me(0).FindOrthographicForm
                        Else
                            Return ""
                        End If
                    End If

                End Function

                ''' <summary>
                ''' Can be used to get the phonetic form, if no orthographic form is set, the phonetic form of the closest child in an unbroken line of single children.
                ''' </summary>
                ''' <returns></returns>
                Public Function FindPhoneticForm() As String

                    If PhoneticForm <> "" Then
                        Return PhoneticForm
                    Else
                        If Me.Count = 1 Then
                            Return Me(0).FindPhoneticForm
                        Else
                            Return ""
                        End If
                    End If

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

                    'Writing headings
                    HeadingList.Add("SEGMENTATION_COMPLETED")
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

                    'Writing data
                    OutputList.Add(SegmentationCompleted.ToString)
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
                Public Sub MeasureSoundLevels(ByVal IncludeCriticalBandLevels As Boolean, ByVal c As Integer, ByRef AttemptedMeasurementCount As Integer, ByRef SuccesfullMeasurementsCount As Integer,
                                              Optional BandInfo As Audio.DSP.BandBank = Nothing,
                                         Optional FftFormat As Audio.Formats.FftFormat = Nothing)

                    Dim ParentSound = ParentSMA.ParentSound

                    'Checks that parent sound has enough channels
                    If ParentSound.WaveFormat.Channels >= c Then

                        'Checks that parent the current channel of the parent sound contains sounds
                        If ParentSound.WaveData.SampleData(c).Length > 0 Then

                            'Measuring sound levels

                            'Measuring UnWeightedLevel
                            UnWeightedLevel = Nothing
                            UnWeightedLevel = DSP.MeasureSectionLevel(ParentSound, c, StartSample, Length, SoundDataUnit.dB)
                            AttemptedMeasurementCount += 1
                            If UnWeightedLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

                            'Meaures UnWeightedPeakLevel
                            UnWeightedPeakLevel = Nothing
                            UnWeightedPeakLevel = DSP.MeasureSectionLevel(ParentSound, c, StartSample, Length, SoundDataUnit.dB, SoundMeasurementType.AbsolutePeakAmplitude)
                            AttemptedMeasurementCount += 1
                            If UnWeightedPeakLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

                            'Measures weighted level
                            WeightedLevel = Nothing
                            If GetTimeWeighting() <> 0 Then
                                WeightedLevel = DSP.GetLevelOfLoudestWindow(ParentSound, c,
                                                                                     GetTimeWeighting() * ParentSound.WaveFormat.SampleRate,
                                                                                      StartSample, Length, , GetFrequencyWeighting, True)
                            Else
                                WeightedLevel = DSP.MeasureSectionLevel(ParentSound, c, StartSample, Length, SoundDataUnit.dB, SoundMeasurementType.RMS, GetFrequencyWeighting)
                            End If
                            AttemptedMeasurementCount += 1
                            If WeightedLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

                            'Measures critical band levels
                            If IncludeCriticalBandLevels = True Then

                                'Gets the sound section
                                Dim SoundFileSection = Me.GetSoundFileSection(c)

                                'Calculating and storing the band levels
                                Dim TempBandLevelList = Audio.DSP.CalculateBandLevels(SoundFileSection, 1, BandInfo)
                                AttemptedMeasurementCount += 1
                                If TempBandLevelList IsNot Nothing Then
                                    SuccesfullMeasurementsCount += 1
                                    Me.BandLevels = TempBandLevelList.ToArray
                                    Me.CentreFrequencies = BandInfo.GetCentreFrequencies
                                    Me.BandWidths = BandInfo.GetBandWidths
                                Else
                                    Me.BandLevels = {}
                                    Me.CentreFrequencies = {}
                                    Me.BandWidths = {}
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
                        childComponent.MeasureSoundLevels(IncludeCriticalBandLevels, c, AttemptedMeasurementCount, SuccesfullMeasurementsCount, BandInfo, FftFormat)
                    Next

                End Sub

                ''' <summary>
                ''' Calculates the SII spectrum levels based on the band levels stored in BandLevels (N.B. requires precalculation of band levels)
                ''' </summary>
                ''' <returns></returns>
                Public Function GetSpectrumLevels(Optional ByVal dBSPL_FSdifference As Double? = Nothing) As Double()

                    'Setting default dBSPL_FSdifference 
                    If dBSPL_FSdifference Is Nothing Then dBSPL_FSdifference = Audio.Standard_dBFS_dBSPL_Difference

                    Dim SpectrumLevelList As New List(Of Double)
                    If BandLevels.Length = BandWidths.Length Then
                        'Calculating the levels only if the lengths of the level and centre frequency vectors agree
                        For i = 0 To BandLevels.Length - 1

                            'Converting dB FS to dB SPL
                            Dim BandLevel_SPL As Double = BandLevels(i) + dBSPL_FSdifference

                            'Calculating spectrum level according to equation 3 in ANSI S3.5-1997 (The SII-standard)
                            'Dim SpectrumLevel As Double = BandLevel_SPL - 10 * Math.Log10(BandWidths(i) / 1)
                            Dim SpectrumLevel As Double = Audio.DSP.BandLevel2SpectrumLevel(BandLevel_SPL, BandWidths(i))
                            SpectrumLevelList.Add(SpectrumLevel)
                        Next
                    End If

                    Return SpectrumLevelList.ToArray

                End Function


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
                ''' Resets the segmentation time data (startSample and Length) of the component and cascading resets to all lower levels.
                ''' </summary>
                Public Sub ResetTemporalData()

                    StartSample = -1
                    Length = 0

                    For Each childcomponent In Me
                        childcomponent.ResetTemporalData()
                    Next

                End Sub

                Public Function GetStringRepresentation() As String

                    If OrthographicForm = "" And PhoneticForm = "" Then
                        Return ""
                    Else
                        If OrthographicForm <> "" And PhoneticForm <> "" Then
                            If PhoneticForm.StartsWith("[") Then
                                Return OrthographicForm & vbCrLf & PhoneticForm
                            Else
                                'Adds square brackets to phonetic forms that lacks them
                                Return OrthographicForm & vbCrLf & "[" & PhoneticForm & "]"
                            End If
                        ElseIf OrthographicForm <> "" Then
                            Return OrthographicForm
                        Else
                            If PhoneticForm.StartsWith("[") Then
                                Return PhoneticForm
                            Else
                                'Adds square brackets to phonetic forms that lacks them
                                Return "[" & PhoneticForm & "]"
                            End If
                        End If
                    End If

                End Function

                ''' <summary>
                ''' Returns all descendant SmaComponents in the current instance of SmaComponent 
                ''' </summary>
                ''' <returns></returns>
                Public Function GetAllDescentantComponents() As List(Of SmaComponent)

                    Dim AllComponents As New List(Of SmaComponent)

                    For Each child In Me
                        AllComponents.Add(child)
                    Next

                    For Each child In Me
                        AllComponents.AddRange(child.GetAllDescentantComponents())
                    Next

                    Return AllComponents

                End Function

                Public Function GetTargetFromLineOfInitialComponents(ByVal TargetLevel As SpeechMaterialComponent.LinguisticLevels) As SmaComponent

                    If GetCorrespondingSpeechMaterialComponentLinguisticLevel() = TargetLevel Then

                        'Returning Me
                        Return Me

                    Else

                        'Not the right level, tries to call GetTargetFromLineOfInitialComponents on the first child
                        If Me.Count > 0 Then

                            Return Me(0).GetTargetFromLineOfInitialComponents(TargetLevel)

                        Else
                            'There are no child components
                            Return Nothing

                        End If

                    End If

                End Function

                Public Function GetCorrespondingSpeechMaterialComponentLinguisticLevel() As SpeechMaterialComponent.LinguisticLevels

                    Select Case Me.SmaTag
                        Case SmaTags.CHANNEL
                            Return SpeechMaterialComponent.LinguisticLevels.List
                        Case SmaTags.SENTENCE
                            Return SpeechMaterialComponent.LinguisticLevels.Sentence
                        Case SmaTags.WORD
                            Return SpeechMaterialComponent.LinguisticLevels.Word
                        Case SmaTags.PHONE
                            Return SpeechMaterialComponent.LinguisticLevels.Phoneme
                        Case Else
                            Throw New Exception("Unable to convert the SmaTag value " & Me.SmaTag & " to SpeechMaterial.LinguisticLevels.")
                    End Select

                End Function

                Public Function GetClosestAncestorComponent(ByVal RequestedParentComponentType As Sound.SpeechMaterialAnnotation.SmaTags) As SmaComponent

                    If ParentComponent Is Nothing Then Return Nothing

                    If ParentComponent.SmaTag = RequestedParentComponentType Then
                        Return ParentComponent
                    Else
                        Return ParentComponent.GetClosestAncestorComponent(RequestedParentComponentType)
                    End If

                End Function

                Public Function GetSiblingsExcludingSelf() As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                    If ParentComponent IsNot Nothing Then
                        For Each child In ParentComponent
                            If child IsNot Me Then
                                OutputList.Add(child)
                            End If
                        Next
                    End If
                    Return OutputList
                End Function

                Public Function GetNumberOfSiblingsExcludingSelf() As Integer

                    Dim Siblings = GetSiblingsExcludingSelf()
                    If Siblings IsNot Nothing Then
                        Return Siblings.Count
                    Else
                        Return 0
                    End If

                End Function

                'Public Function GetUnbrokenLineOfAncestorsWithoutSiblings() As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

                '    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                '    If ParentComponent IsNot Nothing Then
                '        If ParentComponent.GetNumberOfSiblingsExcludingSelf = 0 Then
                '            OutputList.AddRange(ParentComponent)
                '            OutputList.AddRange(ParentComponent.GetUnbrokenLineOfAncestorsWithoutSiblings)
                '        End If
                '    End If

                '    Return OutputList

                'End Function


                Public Sub GetUnbrokenLineOfAncestorsWithSingleChild(ByRef ResultList As List(Of Sound.SpeechMaterialAnnotation.SmaComponent))

                    If Me.GetNumberOfSiblingsExcludingSelf = 0 Then
                        If Me.ParentComponent IsNot Nothing Then
                            ResultList.Add(Me.ParentComponent)
                            Me.ParentComponent.GetUnbrokenLineOfAncestorsWithSingleChild(ResultList)
                        End If
                    End If

                End Sub


                Public Sub GetUnbrokenLineOfDescendentsWithSingleChild(ByRef ResultList As List(Of Sound.SpeechMaterialAnnotation.SmaComponent))

                    If Me.Count = 1 Then
                        ResultList.Add(Me(0))
                        Me(0).GetUnbrokenLineOfDescendentsWithSingleChild(ResultList)
                    End If

                End Sub

                Public Sub GetUnbrokenLineOfFirstbornDescendents(ByRef ResultList As List(Of Sound.SpeechMaterialAnnotation.SmaComponent))

                    If Me.Count > 0 Then
                        ResultList.Add(Me(0))
                        Me(0).GetUnbrokenLineOfFirstbornDescendents(ResultList)
                    End If

                End Sub

                Public Sub GetUnbrokenLineOfLastbornDescendents(ByRef ResultList As List(Of Sound.SpeechMaterialAnnotation.SmaComponent))

                    If Me.Count > 0 Then
                        ResultList.Add(Me(Me.Count - 1))
                        Me(Me.Count - 1).GetUnbrokenLineOfLastbornDescendents(ResultList)
                    End If

                End Sub

                '''' <summary>
                '''' Returns all SmaComponents that locically share the same StartSample value as the current instance of SmaComponent 
                '''' </summary>
                '''' <returns></returns>
                'Public Function GetDependentSegmentationsStarts() As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                '    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                '    OutputList.AddRange(GetDependentSegmentationsStarts(HierarchicalDirections.Upwards))
                '    OutputList.AddRange(GetDependentSegmentationsStarts(HierarchicalDirections.Downwards))
                '    Return OutputList
                'End Function

                'Private Function GetDependentSegmentationsStarts(ByVal HierarchicalDirection As HierarchicalDirections) As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

                '    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

                '    'Cascading up or down
                '    Select Case HierarchicalDirection
                '        Case HierarchicalDirections.Upwards

                '            If Me.SmaTag = SmaTags.CHANNEL Then Return OutputList

                '            Dim SelfIndex = GetSelfIndex()
                '            If SelfIndex.HasValue Then
                '                If SelfIndex = 0 Then
                '                    OutputList.AddRange(ParentComponent.GetDependentSegmentationsStarts(HierarchicalDirection))
                '                End If
                '            End If

                '        Case HierarchicalDirections.Downwards

                '            If Me.Count > 0 Then
                '                OutputList.AddRange(Me(0).GetDependentSegmentationsStarts(HierarchicalDirection))
                '            End If

                '    End Select

                '    Return OutputList

                'End Function

                '''' <summary>
                '''' Returns all SmaComponents that locically share the same end time as the current instance of SmaComponent 
                '''' </summary>
                '''' <returns></returns>
                'Public Function GetDependentSegmentationsEnds() As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                '    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                '    OutputList.AddRange(GetDependentSegmentationsEnds(HierarchicalDirections.Upwards))
                '    OutputList.AddRange(GetDependentSegmentationsEnds(HierarchicalDirections.Downwards))
                '    Return OutputList
                'End Function

                'Public Function GetDependentSegmentationsEnds(ByVal HierarchicalDirection As HierarchicalDirections) As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

                '    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

                '    'Cascading up or down
                '    Select Case HierarchicalDirection
                '        Case HierarchicalDirections.Upwards

                '            Dim SelfIndex = GetSelfIndex()
                '            If SelfIndex.HasValue Then
                '                Dim SiblingCount = GetSiblings.Count
                '                If SelfIndex = SiblingCount - 1 Then
                '                    'The current instance is the last of its same level components
                '                    OutputList.AddRange(ParentComponent.GetDependentSegmentationsEnds(HierarchicalDirection))
                '                End If
                '            End If

                '        Case HierarchicalDirections.Downwards

                '            If Me.Count > 0 Then
                '                OutputList.AddRange(Me(Me.Count - 1).GetDependentSegmentationsEnds(HierarchicalDirection))
                '            End If

                '    End Select

                '    Return OutputList

                'End Function

                Public Sub AlignSegmentationStartsAcrossLevels(ByVal SoundLength As Integer)

                    'Aligning the start of a segmentation set on the current level to dependent segmentation starts on all other levels, without moving the end of each segmentation (unless it the length would have to be reduced below zero, which is not allowed)
                    'These functions may reduce the length of a segmentation to zero
                    AlignSegmentationStarts(StartSample, SoundLength, HierarchicalDirections.Upwards)
                    AlignSegmentationStarts(StartSample, SoundLength, HierarchicalDirections.Downwards)

                End Sub

                Public Sub AlignSegmentationEndsAcrossLevels()

                    'TODO: I'm not entirely clear as to if the ends or starts should be aligned first....????

                    'Aligning the end of a segmentation set on the current level to the ends of dependent segmentation on all other levels, by adjusting their lengths, and if needed also their start positions, in case lengths gets reduced to zero.
                    Dim ExclusiveEndSample As Integer = StartSample + Length
                    'These functions may move the start of a segmentation to an earlier time
                    AlignSegmentationEnds(ExclusiveEndSample, HierarchicalDirections.Upwards)
                    AlignSegmentationEnds(ExclusiveEndSample, HierarchicalDirections.Downwards)

                End Sub

                Public Enum HierarchicalDirections
                    Upwards
                    Downwards
                End Enum

                ''' <summary>
                '''Aligns the start of a segmentation set on the current level to dependent segmentation starts on all other levels, without moving the end of each segmentation (unless it the length would have to be reduced below zero, which is not allowed)
                ''' </summary>
                ''' <param name="NewStartSample"></param>
                ''' <param name="SoundLength"></param>
                ''' <param name="HierarchicalDirection"></param>
                Private Sub AlignSegmentationStarts(ByVal NewStartSample As Integer, ByVal SoundLength As Integer, ByVal HierarchicalDirection As HierarchicalDirections)

                    ' Moving the start, unless the current level channel
                    If Me.SmaTag <> SmaTags.CHANNEL Then
                        MoveStart(NewStartSample, SoundLength)
                    End If

                    'Cascading up or down
                    Select Case HierarchicalDirection
                        Case HierarchicalDirections.Upwards

                            Dim SelfIndex = GetSelfIndex()
                            If SelfIndex.HasValue Then
                                If SelfIndex = 0 Then ParentComponent.AlignSegmentationStarts(NewStartSample, SoundLength, HierarchicalDirection)
                            End If

                        Case HierarchicalDirections.Downwards

                            If Me.Count > 0 Then
                                Me(0).AlignSegmentationStarts(NewStartSample, SoundLength, HierarchicalDirection)
                            End If

                    End Select

                End Sub

                ''' <summary>
                ''' Moves the start sample of a segmentation without changing the end sample (unless the length would be reduced below zero).
                ''' </summary>
                ''' <param name="NewStartSample"></param>
                Public Sub MoveStart(ByVal NewStartSample As Integer, ByVal SoundLength As Integer)

                    Dim TempStartSample As Integer = Math.Max(0, StartSample) ' Needed since default (onset) StartSample value is -1
                    Dim ExclusiveEndSample As Integer = TempStartSample + Length

                    Dim StartSampleChange As Integer = NewStartSample - TempStartSample
                    'A positive value of StartSampleChange represent a forward shift, and vice versa

                    'Changing the start value
                    TempStartSample += StartSampleChange

                    'Making sure the start value stays withing the sound file
                    TempStartSample = Math.Min(SoundLength - 1, TempStartSample)
                    TempStartSample = Math.Max(0, TempStartSample)

                    'Storing the difference in start sampe
                    Dim DifferenceSamples As Integer = TempStartSample - StartSample

                    'Storing the new StartSample
                    StartSample = TempStartSample

                    'Adjusting the Length
                    Length = ExclusiveEndSample - StartSample

                    'Also adjusts StartTime
                    If ParentSMA.ParentSound IsNot Nothing Then
                        Dim DurationDifference As Double = DifferenceSamples / ParentSMA.ParentSound.WaveFormat.SampleRate
                        StartTime += DurationDifference
                    End If

                End Sub

                ''' <summary>
                ''' Aligns the end of a segmentation set on the current level to the ends of dependent segmentation on all other levels, by adjusting their lengths, and if needed also their start positions, in case lengths gets reduced to zero.
                ''' </summary>
                ''' <param name="NewExclusiveEndSample"></param>
                ''' <param name="HierarchicalDirection"></param>
                Private Sub AlignSegmentationEnds(ByVal NewExclusiveEndSample As Integer, ByVal HierarchicalDirection As HierarchicalDirections)

                    ' Moving the end, unless the current level channel
                    If Me.SmaTag <> SmaTags.CHANNEL Then

                        Dim TempStartSample As Integer = Math.Max(0, StartSample) ' Needed since default (onset) StartSample value is -1
                        Dim MyExclusiveEndSample As Integer = TempStartSample + Length

                        Dim EndSampleChange As Integer = NewExclusiveEndSample - MyExclusiveEndSample
                        'A positive value of EndSampleChange represent a forward shift, and vice versa

                        'Changing the length
                        MyExclusiveEndSample += EndSampleChange

                        Dim NewLengthValue As Integer = (MyExclusiveEndSample - 1) - TempStartSample

                        'Moves the start sample (and sets length to zero) if the new length would have to be reduced below zero.
                        If NewLengthValue < 0 Then
                            TempStartSample += NewLengthValue
                            NewLengthValue = 0
                        End If

                        'Storing the new values
                        StartSample = TempStartSample
                        Length = NewLengthValue

                    End If


                    'Cascading up or down
                    Select Case HierarchicalDirection
                        Case HierarchicalDirections.Upwards

                            Dim SelfIndex = GetSelfIndex()
                            If SelfIndex.HasValue Then
                                Dim SiblingCount = GetSiblings.Count
                                If SelfIndex = SiblingCount - 1 Then
                                    'The current instance is the last of its same level components
                                    ParentComponent.AlignSegmentationEnds(NewExclusiveEndSample, HierarchicalDirection)
                                End If
                            End If

                        Case HierarchicalDirections.Downwards

                            If Me.Count > 0 Then
                                Me(Me.Count - 1).AlignSegmentationEnds(NewExclusiveEndSample, HierarchicalDirection)
                            End If

                    End Select


                End Sub

                ''' <summary>
                ''' Figures out and returns at what index in the parent component the component itself is stored, or Nothing if there is no parent compoment, or if (for some unexpected reason) unable to establish the index.
                ''' </summary>
                ''' <returns></returns>
                Public Function GetSelfIndex() As Integer?

                    If ParentComponent Is Nothing Then Return Nothing

                    Dim Siblings = GetSiblings()
                    For s = 0 To Siblings.Count - 1
                        If Siblings(s) Is Me Then Return s
                    Next

                    Return Nothing

                End Function

                Public Sub GetHierarchicalSelfIndexSerie(ByRef HierarchicalSelfIndexSerie As List(Of String))

                    If Me.ParentComponent Is Nothing Then
                        Exit Sub
                    Else

                        'Calls GetHierarchicalSelfIndexSerie on the parent
                        Me.ParentComponent.GetHierarchicalSelfIndexSerie(HierarchicalSelfIndexSerie)

                    End If

                    Dim LinguisticLevelString As String = ""
                    Select Case Me.SmaTag
                        Case SmaTags.CHANNEL
                            LinguisticLevelString = "C"
                        Case SmaTags.SENTENCE
                            LinguisticLevelString = "S"
                        Case SmaTags.WORD
                            LinguisticLevelString = "W"
                        Case SmaTags.PHONE
                            LinguisticLevelString = "P"
                    End Select

                    HierarchicalSelfIndexSerie.Add(LinguisticLevelString & Me.GetSelfIndex)

                End Sub

                Public Function GetSiblings() As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                    If ParentComponent IsNot Nothing Then
                        Return ParentComponent.ToList
                    Else
                        Return Nothing
                    End If
                End Function

                ''' <summary>
                ''' Infers the lengths of each sibling segment from the startposition of the next sibling (naturally, this does not set the length of the last sibling).
                ''' </summary>
                Public Sub InferSiblingLengths()

                    Dim Siblings = GetSiblings()
                    If Siblings IsNot Nothing Then
                        For i = 0 To Siblings.Count - 2

                            Dim PreviousLength As Integer = Siblings(i).Length

                            'Locks the length of sibling i to the start position (-1) of the sibling i+1
                            Siblings(i).Length = Siblings(i + 1).StartSample - Siblings(i).StartSample

                            If Siblings(i).Length <> PreviousLength Then
                                'Invalidating segmentation if the length was changed
                                Siblings(i).SetSegmentationCompleted(False, True)
                            End If

                        Next
                    End If

                End Sub

                ''' <summary>
                ''' Sets the value of the SegmentationCompleted property and optinally cascades that value to all descendant components.
                ''' </summary>
                ''' <param name="Value"></param>
                ''' <param name="CascadeToAllDescendants"></param>
                ''' <param name="LowestSmaLevel">The linguistic level at which to stop validating (highest is CHANNEL and lowest is PHONE)</param>
                Public Sub SetSegmentationCompleted(ByVal Value As Boolean, Optional ByVal InferToDependentComponents As Boolean = True,
                                                    Optional ByVal CascadeToAllDescendants As Boolean = False,
                                                    Optional ByVal LowestSmaLevel As SmaTags = SmaTags.PHONE,
                                                    Optional ByVal HighestSmaLevel As SmaTags = SmaTags.CHANNEL)

                    'Skipping setting the value if SmaTag is Channel, since channels should always be validated
                    If Me.SmaTag <> SmaTags.CHANNEL Then

                        If Me.SmaTag <= LowestSmaLevel And Me.SmaTag >= HighestSmaLevel Then
                            'Changing the validation value only if the current linguistic level is at or below HighestSmaLevel and at or above LowestSmaLevel
                            _SegmentationCompleted = Value
                        End If
                    End If

                    If CascadeToAllDescendants = True Then
                        For Each child In Me
                            child.SetSegmentationCompleted(Value, False, CascadeToAllDescendants, LowestSmaLevel, HighestSmaLevel)
                        Next
                    End If

                    If InferToDependentComponents = True Then

                        If InferToDependentComponents Then

                            SetValidationValueOfDependentAncestors(Value)
                            SetValidationValueOfDependentDescendents(Value)

                        End If


                        ''Gets all dependent segmentations starts 
                        'Dim DependentSegmentations = Me.GetDependentSegmentationsStarts

                        ''And all dependent segmentations ends
                        'DependentSegmentations.AddRange(Me.GetDependentSegmentationsEnds)

                        'For Each DependentSegmentation In DependentSegmentations
                        '    DependentSegmentation.SetSegmentationCompleted(Value, False, False)
                        'Next
                    End If

                End Sub


                Private Sub SetValidationValueOfDependentAncestors(ByVal ValidationValue As Boolean)

                    'Validates all members of an UnbrokenLineOfAncestorsWithSingleChild 
                    Dim UnbrokenLineOfAncestorsWithSingleChild As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                    Me.GetUnbrokenLineOfAncestorsWithSingleChild(UnbrokenLineOfAncestorsWithSingleChild)
                    For Each SmaComponent In UnbrokenLineOfAncestorsWithSingleChild
                        SmaComponent.SetSegmentationCompleted(ValidationValue, False, False)
                    Next

                    'Sets validation values upwards
                    Dim Siblings = GetSiblings()
                    If Siblings IsNot Nothing Then
                        'Means there is a parent

                        'Exiting if ValidationValue = True either the firstborn or lastborn sibling component is not validated
                        If ValidationValue = True Then
                            If Siblings(0).SegmentationCompleted = False Or Siblings(Siblings.Count - 1).SegmentationCompleted = False Then
                                'Not both of the start and end of the sibling series are validated.
                                Exit Sub
                            End If
                        End If

                        'Setting parent validation value
                        ParentComponent.SetSegmentationCompleted(ValidationValue, False, False)

                        'Validating recursively upwards
                        ParentComponent.SetValidationValueOfDependentAncestors(ValidationValue)

                    End If

                End Sub

                Private Sub SetValidationValueOfDependentDescendents(ByVal ValidationValue As Boolean)

                    If ValidationValue = True Then

                        'Sets validation value of all members of an UnbrokenLineOfDescendentsWithSingleChild
                        Dim UnbrokenLineOfDescendentsWithSingleChild As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                        Me.GetUnbrokenLineOfDescendentsWithSingleChild(UnbrokenLineOfDescendentsWithSingleChild)
                        For Each SmaComponent In UnbrokenLineOfDescendentsWithSingleChild
                            SmaComponent.SetSegmentationCompleted(ValidationValue, False, False)
                        Next

                    Else

                        'Invalidates all descendents
                        Dim AllDescendents = GetAllDescentantComponents()
                        For Each SmaComponent In AllDescendents
                            SmaComponent.SetSegmentationCompleted(ValidationValue, False, False)
                        Next

                    End If

                End Sub

                ''' <summary>
                ''' 
                ''' </summary>
                ''' <param name="CurrentChannel"></param>
                ''' <returns>Returns True if all checks passed, and False if any check did not pass.</returns>
                Private Function CheckSoundDataAvailability(ByVal CurrentChannel As Integer, ByVal SupressWarnings As Boolean) As Boolean

                    'Checks that the needed instances exist
                    If ParentSMA Is Nothing Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: The current instance of SmaComponent does not have a ParentSMA object.")
                        Return False
                    End If
                    If ParentSMA.ParentSound Is Nothing Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: The ParentSMA does not have a ParentSound object.")
                        Return False
                    End If
                    If CurrentChannel > ParentSMA.ParentSound.WaveFormat.Channels Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: The ParentSMA.ParentSound object does not have " & CurrentChannel & " channels!")
                        Return False
                    End If
                    If CurrentChannel > ParentSMA.ChannelCount Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: The ParentSMA object does not have " & CurrentChannel & " channels!")
                        Return False
                    End If

                    'Checks that sound data exists at the indicated sampleindices
                    If StartSample < 0 Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: Start sample cannot be lower than zero!")
                        Return False
                    End If

                    If Length < 0 Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: Length of a section cannot lower than zero!")
                        Return False
                    End If

                    If Length = 0 Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: Length of a section cannot be zero!")
                        Return False
                    End If

                    If StartSample + Length > ParentSMA.ParentSound.WaveData.SampleData(CurrentChannel).Length Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: The requested data is outside the range of the sound parent sound!")
                        Return False
                    End If

                    Return True

                End Function


                ''' <summary>
                ''' Creates new (mono) sound containing the portion of the ParentSound that the current instance of SmaComponent represent, based on the available segmentation data. (The output sound does not contain the SMA object.)
                ''' </summary>
                ''' <param name="InitialMargin">If referenced by the calling code, and holds a negative number, returns the number of samples prior to the first sample. </param>
                ''' <returns></returns>
                Public Function GetSoundFileSection(ByVal CurrentChannel As Integer, Optional ByVal SupressWarnings As Boolean = False, Optional ByRef InitialMargin As Integer = -1) As Audio.Sound

                    If CheckSoundDataAvailability(CurrentChannel, SupressWarnings) = False Then
                        Return Nothing
                    End If

                    'Creates a new sound
                    Dim OutputSound = New Audio.Sound(New Formats.WaveFormat(ParentSMA.ParentSound.WaveFormat.SampleRate, ParentSMA.ParentSound.WaveFormat.BitDepth, 1, , ParentSMA.ParentSound.WaveFormat.Encoding))

                    'Stores the initial margin (i.e., the number of samples prior to the first sample retrieved), only if it has not been set
                    If InitialMargin < 0 Then InitialMargin = StartSample

                    'Copies data
                    OutputSound.WaveData.SampleData(1) = ParentSMA.ParentSound.WaveData.SampleData(CurrentChannel).ToList.GetRange(StartSample, Length).ToArray

                    'Also copies the Nominal level
                    OutputSound.SMA.NominalLevel = NominalLevel
                    OutputSound.SMA.InferNominalLevelToAllDescendants()

                    'Returns the sound
                    Return OutputSound

                End Function

                ''' <summary>
                ''' Checks the SegmentationCompleted value of all sibling SmaComponents is True. Otherwise returns False.
                ''' </summary>
                ''' <returns></returns>
                Public Function AllSiblingSegmentationsCompleted() As Boolean

                    Dim MySiblings = GetSiblings()

                    If MySiblings Is Nothing Then
                        Return False
                    Else
                        For Each c In MySiblings
                            If c.SegmentationCompleted = False Then Return False
                        Next
                    End If

                    Return True

                End Function

                ''' <summary>
                ''' Checks the SegmentationCompleted value of all child SmaComponents is True. Otherwise returns False.
                ''' </summary>
                ''' <returns></returns>
                Public Function AllChildSegmentationsCompleted() As Boolean

                    For Each c In Me
                        If c.SegmentationCompleted = False Then Return False
                    Next

                    Return True

                End Function

                ''' <summary>
                ''' Compares the current peak amplitude with the InitialPeak of the word segments in the current channel of the audio recording 
                ''' and returns the gain that has been applied to the audio since the initial recording.
                ''' </summary>
                ''' <returns>Returns the gain applied since recoring, or Nothing if measurements failed.</returns>
                Public Function GetCurrentGain(ByVal MeasurementSound As Sound, ByVal MeasurementChannel As Integer) As Double?

                    If Me.CheckStartAndLength() = False Then
                        Return Nothing
                    End If

                    Dim soundLength As Integer = MeasurementSound.WaveData.ShortestChannelSampleCount
                    If Me.StartSample + Me.Length > soundLength Then
                        Return Nothing
                    End If

                    'Getting the current peak amplitude
                    'Meaures UnWeightedPeakLevel
                    Dim CurrentPeakAmplitude As Double? = DSP.MeasureSectionLevel(MeasurementSound, MeasurementChannel, Me.StartSample, Me.Length, SoundDataUnit.dB, SoundMeasurementType.AbsolutePeakAmplitude)

                    'Returns nothing if measurement failed 
                    If CurrentPeakAmplitude Is Nothing Then
                        Return Nothing
                    End If

                    'Converting the initial peak amplitude to dB
                    Dim InitialPeakLevel As Double = dBConversion(Me.InitialPeak, dBConversionDirection.to_dB, MeasurementSound.WaveFormat)

                    'Getting the currently applied gain
                    Dim Gain As Double = CurrentPeakAmplitude - InitialPeakLevel

                    Return Gain

                End Function


                ''' <summary>
                ''' 
                ''' </summary>
                ''' <param name="Gain"></param>
                Public Sub ApplyGain(ByVal Gain As Double, Optional ByVal CurrentChannel As Integer = 1, Optional ByVal SupressWarnings As Boolean = False, Optional SoftEdgeGain As Boolean = True, Optional SoftEdgeSamples As Integer = 100)

                    If CheckSoundDataAvailability(CurrentChannel, SupressWarnings) = True Then

                        If SoftEdgeGain = True Then

                            'Calculates the amount of possible fade length, limiting it by SoftEdgeSamples and a tenth of the length of the SmaComponent
                            Dim FadeLength As Integer = Math.Min(SoftEdgeSamples, Math.Floor(Length / 10))
                            If FadeLength = 0 Then
                                'Amplifies the whole section
                                Audio.DSP.AmplifySection(ParentSMA.ParentSound, Gain, CurrentChannel, StartSample, Length)
                            Else

                                'Fades in and out during fade length number of samples
                                Audio.DSP.Fade(ParentSMA.ParentSound, 0, -Gain, CurrentChannel, StartSample, FadeLength)
                                Audio.DSP.AmplifySection(ParentSMA.ParentSound, Gain, CurrentChannel, StartSample + FadeLength, Length - 2 * FadeLength)
                                Audio.DSP.Fade(ParentSMA.ParentSound, 0, -Gain, CurrentChannel, StartSample + Length - FadeLength, FadeLength)

                            End If

                        Else
                            'Applies gain
                            Audio.DSP.AmplifySection(ParentSMA.ParentSound, Gain, CurrentChannel, StartSample, Length)
                        End If

                        ParentSMA.ParentSound.SetIsChangedManually(True)

                    Else
                        MsgBox("Unable to apply gain to the SMA component " & Me.GetStringRepresentation())
                    End If

                End Sub

                ''' <summary>
                ''' Cretates a identifier string that points to the sound section of the sound file containing the audio data
                ''' </summary>
                ''' <param name="SoundFilePathString">As the SmaComponent may not always know the wave file from which is was read, the calling code can insteda supply the path here.</param>
                ''' <returns></returns>
                Public Function CreateUniqueSoundSectionIdentifier(Optional ByVal SoundFilePathString As String = "") As String

                    Dim OutputList As New List(Of String)

                    If SoundFilePathString = "" Then
                        If SourceFilePath <> "" Then
                            OutputList.Add(SourceFilePath)
                        Else
                            'Attempts to find the path from the sound file
                            If ParentSMA Is Nothing Then
                                OutputList.Add("NoParent")
                            Else
                                If ParentSMA.ParentSound Is Nothing Then
                                    OutputList.Add("NoParentSound")
                                Else
                                    OutputList.Add(ParentSMA.ParentSound.FileName)
                                End If
                            End If
                        End If
                    Else
                        'Using the string supplied by the calling code
                        OutputList.Add(SoundFilePathString)
                    End If

                    Dim HierarchicalSelfIndexSerie As New List(Of String)
                    Me.GetHierarchicalSelfIndexSerie(HierarchicalSelfIndexSerie)
                    OutputList.Add(String.Concat(HierarchicalSelfIndexSerie))

                    Return String.Join("_", OutputList)

                End Function


                Public Function ReturnIsolatedSMA() As SpeechMaterialAnnotation

                    Dim SmaCopy = Me.ParentSMA.CreateCopy(Nothing)
                    SmaCopy._ChannelData.Clear()
                    SmaCopy.AddChannelData()

                    Dim ParentChannelCopy As SmaComponent = Nothing

                    Select Case Me.SmaTag
                        Case SmaTags.PHONE

                            Dim SmaPhoneCopy = Me.CreateCopy
                            SmaPhoneCopy.ParentSMA = SmaCopy

                            Dim ParentWordCopy = Me.ParentComponent.CreateCopy
                            ParentWordCopy.ParentSMA = SmaCopy
                            ParentWordCopy.Clear()

                            Dim ParentSentenceCopy = Me.ParentComponent.ParentComponent.CreateCopy
                            ParentSentenceCopy.ParentSMA = SmaCopy
                            ParentSentenceCopy.Clear()

                            ParentChannelCopy = Me.ParentComponent.ParentComponent.ParentComponent.CreateCopy
                            ParentChannelCopy.ParentSMA = SmaCopy
                            ParentChannelCopy.Clear()

                            SmaPhoneCopy.ParentComponent = ParentWordCopy
                            ParentWordCopy.ParentComponent = ParentSentenceCopy
                            ParentSentenceCopy.ParentComponent = ParentChannelCopy

                            ParentWordCopy.Add(SmaPhoneCopy)
                            ParentSentenceCopy.Add(ParentWordCopy)
                            ParentChannelCopy.Add(ParentSentenceCopy)

                        Case SmaTags.WORD

                            Dim ParentWordCopy = Me.CreateCopy
                            ParentWordCopy.ParentSMA = SmaCopy

                            Dim ParentSentenceCopy = Me.ParentComponent.CreateCopy
                            ParentSentenceCopy.ParentSMA = SmaCopy
                            ParentSentenceCopy.Clear()
                            ParentSentenceCopy.Add(ParentWordCopy)

                            ParentChannelCopy = Me.ParentComponent.ParentComponent.CreateCopy
                            ParentChannelCopy.ParentSMA = SmaCopy
                            ParentChannelCopy.Clear()
                            ParentChannelCopy.Add(ParentSentenceCopy)

                            ParentWordCopy.ParentComponent = ParentSentenceCopy
                            ParentSentenceCopy.ParentComponent = ParentChannelCopy


                        Case SmaTags.SENTENCE

                            Dim ParentSentenceCopy = Me.CreateCopy
                            ParentSentenceCopy.ParentSMA = SmaCopy

                            ParentChannelCopy = Me.ParentComponent.CreateCopy
                            ParentChannelCopy.ParentSMA = SmaCopy
                            ParentChannelCopy.Clear()
                            ParentChannelCopy.Add(ParentSentenceCopy)

                            ParentSentenceCopy.ParentComponent = ParentChannelCopy


                        Case SmaTags.CHANNEL

                            ParentChannelCopy = Me.ParentComponent.CreateCopy
                            ParentChannelCopy.ParentSMA = SmaCopy

                    End Select

                    'Connecting to the SMA channel data
                    SmaCopy.ChannelData(1) = ParentChannelCopy

                    Return SmaCopy

                End Function



                ''' <summary>
                ''' Creates a new SmaComponent which is a deep copy of the original, by using serialization.
                ''' </summary>
                ''' <returns></returns>
                Public Function CreateCopy() As SmaComponent

                    'Creating an output object
                    Dim newSmaComponent As SmaComponent

                    'Serializing to memorystream
                    Dim serializedMe As New MemoryStream
                    Dim serializer As New XmlSerializer(GetType(SmaComponent))
                    serializer.Serialize(serializedMe, Me)

                    'Deserializing to new object
                    serializedMe.Position = 0
                    newSmaComponent = CType(serializer.Deserialize(serializedMe), SmaComponent)
                    serializedMe.Close()

                    'Returning the new object
                    Return newSmaComponent
                End Function

                'Moves the start position of all child sma components nSamples without concidering the actual sound boundaries
                Public Sub TimeShift(ByRef nSamples As Integer)

                    StartSample += nSamples

                    For Each ChildComponent In Me
                        ChildComponent.TimeShift(nSamples)
                    Next

                End Sub


            End Class

            Public Function GetSmaComponentByIndexSeries(ByVal IndexSeries As SpeechMaterialComponent.ComponentIndices, ByVal AudioFileLinguisticLevel As SpeechMaterialComponent.LinguisticLevels, ByVal SoundChannel As Integer) As SmaComponent

                'Correcting the indices below AudioFileLinguisticLevel
                'If AudioFileLinguisticLevel >= SpeechMaterial.LinguisticLevels.ListCollection Then IndexSeries.ListCollectionIndex = 0 'There is only list collection per recording
                If AudioFileLinguisticLevel >= SpeechMaterialComponent.LinguisticLevels.List Then IndexSeries.ListIndex = 0 'There is only list per recording
                If AudioFileLinguisticLevel >= SpeechMaterialComponent.LinguisticLevels.Sentence Then IndexSeries.SentenceIndex = 0 'There is only one sentence per recording
                If AudioFileLinguisticLevel >= SpeechMaterialComponent.LinguisticLevels.Word Then IndexSeries.WordIndex = 0 'There is only word per recording
                If AudioFileLinguisticLevel >= SpeechMaterialComponent.LinguisticLevels.Phoneme Then IndexSeries.PhoneIndex = 0 'There is only phoneme per recording

                If SoundChannel > Me.ChannelCount Then
                    Return Nothing
                End If

                'If IndexSeries.ListCollectionIndex <> 0 Then Return Nothing

                If IndexSeries.ListIndex <> 0 Then Return Nothing

                If IndexSeries.SentenceIndex > Me.ChannelData(SoundChannel).Count - 1 Then
                    Return Nothing
                ElseIf IndexSeries.SentenceIndex < 0 Then
                    Return Me.ChannelData(SoundChannel)
                End If

                If IndexSeries.WordIndex > Me.ChannelData(SoundChannel)(IndexSeries.SentenceIndex).Count - 1 Then
                    Return Nothing
                ElseIf IndexSeries.WordIndex < 0 Then
                    Return Me.ChannelData(SoundChannel)(IndexSeries.SentenceIndex)
                End If

                If IndexSeries.PhoneIndex > Me.ChannelData(SoundChannel)(IndexSeries.SentenceIndex)(IndexSeries.WordIndex).Count - 1 Then
                    Return Nothing
                ElseIf IndexSeries.PhoneIndex < 0 Then
                    Return Me.ChannelData(SoundChannel)(IndexSeries.SentenceIndex)(IndexSeries.WordIndex)
                Else
                    Return Me.ChannelData(SoundChannel)(IndexSeries.SentenceIndex)(IndexSeries.WordIndex)(IndexSeries.PhoneIndex)
                End If


            End Function

            'Moves the start position of all child sma components nSamples without concidering the actual sound boundaries
            Public Sub TimeShift(ByRef nSamples As Integer)

                For Channel = 1 To Me.ChannelCount
                    Me.ChannelData(1).TimeShift(nSamples)
                Next

            End Sub

            ''' <summary>
            ''' Enforces the validation value to all SmaComponents in the current SMA object
            ''' </summary>
            ''' <param name="ValidationValue"></param>
            Public Sub EnforceValidationValue(ByVal ValidationValue As Boolean)
                For Channel = 1 To Me.ChannelCount
                    Me.ChannelData(1).SetSegmentationCompleted(ValidationValue, False, True)
                Next
            End Sub

            ''' <summary>
            ''' Enforces the validation value to all SmaComponents in the current SMA object
            ''' </summary>
            ''' <param name="ValidationValue"></param>
            Public Sub EnforceValidationValue(ByVal ValidationValue As Boolean,
                                              Optional ByVal LowestSmaLevel As SmaTags = SmaTags.PHONE,
                                              Optional ByVal HighestSmaLevel As SmaTags = SmaTags.CHANNEL)
                For Channel = 1 To Me.ChannelCount
                    Me.ChannelData(1).SetSegmentationCompleted(ValidationValue, False, True, LowestSmaLevel, HighestSmaLevel)
                Next
            End Sub

        End Class

    End Class



End Namespace
