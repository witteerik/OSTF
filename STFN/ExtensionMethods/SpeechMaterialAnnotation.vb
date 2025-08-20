Imports System.Runtime.CompilerServices
Imports STFN.Core.Audio.Sound
Imports STFN.Core.Audio.Sound.SpeechMaterialAnnotation

'This file contains extension methods for STFN.Core.Audio.Sound.SpeechMaterialAnnotation

Partial Public Module Extensions


    ''' <summary>
    ''' Measures sound levels for each channel, sentence, word and phone of the current SMA object.
    ''' </summary>
    ''' <returns>Returns True if all measurements were successful, and False if one or more measurements failed.</returns>
    <Extension>
    Public Function MeasureSoundLevels(obj As SpeechMaterialAnnotation, Optional ByVal IncludeCriticalBandLevels As Boolean = False, Optional ByVal LogMeasurementResults As Boolean = False, Optional ByVal LogFolder As String = "") As Boolean

        If obj.ParentSound Is Nothing Then
            Throw New Exception("The parent sound if the current instance of SpeechMaterialAnnotation cannot be Nothing!")
        End If

        Dim SuccesfullMeasurementsCount As Integer = 0
        Dim AttemptedMeasurementCount As Integer = 0

        'Measuring each channel 
        For c As Integer = 1 To obj.ChannelCount
            obj.ChannelData(c).MeasureSoundLevels(IncludeCriticalBandLevels, c, AttemptedMeasurementCount, SuccesfullMeasurementsCount)
        Next

        'Logging results
        If LogMeasurementResults = True Then
            STFN.Core.Audio.SendInfoToAudioLog(vbCrLf &
                           "FileName" & vbTab & obj.ParentSound.FileName & vbTab &
                           "FailedMeasurementCount: " & vbTab & AttemptedMeasurementCount - SuccesfullMeasurementsCount & vbTab &
                           obj.ToString(True), "SoundMeasurementLog.txt", LogFolder)
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
    <Extension>
    Public Function SetInitialPeakAmplitudes(obj As SpeechMaterialAnnotation) As Boolean

        If obj.ParentSound Is Nothing Then
            Throw New Exception("The parent sound if the current instance of SpeechMaterialAnnotation cannot be Nothing!")
        End If

        Dim SuccesfullMeasurementsCount As Integer = 0
        Dim AttemptedMeasurementCount As Integer = 0

        'Measuring each channel 
        For c As Integer = 1 To obj.ChannelCount
            obj.ChannelData(c).SetInitialPeakAmplitudes(obj.ParentSound, c, AttemptedMeasurementCount, SuccesfullMeasurementsCount)
        Next

        'Logging results
        STFN.Core.Audio.SendInfoToAudioLog(vbCrLf &
                       "FileName" & vbTab & obj.ParentSound.FileName & vbTab &
                       "FailedMeasurementCount: " & vbTab & AttemptedMeasurementCount - SuccesfullMeasurementsCount & vbTab &
                       obj.ToString(True), "InitialPeakMeasurementLog.txt")

        'Checking if all attempted measurements were succesful
        If AttemptedMeasurementCount = SuccesfullMeasurementsCount Then
            Return True
        Else
            Return False
        End If

    End Function


    ''' <summary>
    ''' Enforces the validation value to all SmaComponents in the current SMA object
    ''' </summary>
    ''' <param name="ValidationValue"></param>
    <Extension>
    Public Sub EnforceValidationValue(obj As SpeechMaterialAnnotation, ByVal ValidationValue As Boolean)
        For Channel = 1 To obj.ChannelCount
            obj.ChannelData(1).SetSegmentationCompleted(ValidationValue, False, True)
        Next
    End Sub

    ''' <summary>
    ''' Enforces the validation value to all SmaComponents in the current SMA object
    ''' </summary>
    ''' <param name="ValidationValue"></param>
    <Extension>
    Public Sub EnforceValidationValue(obj As SpeechMaterialAnnotation, ByVal ValidationValue As Boolean,
                                      Optional ByVal LowestSmaLevel As SmaTags = SmaTags.PHONE,
                                      Optional ByVal HighestSmaLevel As SmaTags = SmaTags.CHANNEL)
        For Channel = 1 To obj.ChannelCount
            obj.ChannelData(1).SetSegmentationCompleted(ValidationValue, False, True, LowestSmaLevel, HighestSmaLevel)
        Next
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
    <Extension>
    Public Sub DetectSpeechBoundaries(obj As SpeechMaterialAnnotation, ByVal CurrentChannel As Integer,
                                              Optional ByVal InitialPadding As Double = 0,
                                              Optional ByVal FinalPadding As Double = 0,
                                              Optional ByVal SilenceDefinition As Double = 25,
                                              Optional ByVal SetToZeroCrossings As Boolean = True)

        Try



            Dim TotalSoundLength = obj.ParentSound.WaveData.SampleData(CurrentChannel).Length

            'Low-pass filter
            Dim LpFirFilter_FftFormat = New STFN.Core.Audio.Formats.FftFormat(1024 * 4,,,, True)
            Dim ReusedLpFirKernel As STFN.Core.Audio.Sound = DSP.CreateSpecialTypeImpulseResponse(obj.ParentSound.WaveFormat, LpFirFilter_FftFormat, 4000, , DSP.FilterType.LinearAttenuationAboveCF_dBPerOctave, 15,,,, 25,, True)

            'High-pass filterring the sound to reduce vibration influences
            Dim HpFirFilter_FftFormat = New STFN.Core.Audio.Formats.FftFormat(1024 * 4,,,, True)
            Dim ReusedHpFirKernel As STFN.Core.Audio.Sound = DSP.CreateSpecialTypeImpulseResponse(obj.ParentSound.WaveFormat, HpFirFilter_FftFormat, 4000, , DSP.FilterType.LinearAttenuationBelowCF_dBPerOctave, 100,,,, 25,, True)

            For s = 0 To obj.ChannelData(CurrentChannel).Count - 1

                Dim SentenceSmaComponent = obj.ChannelData(CurrentChannel)(s)

                Dim SentenceSoundCopy = SentenceSmaComponent.GetSoundFileSection(CurrentChannel)

                Dim FilterredSound As STFN.Core.Audio.Sound = DSP.FIRFilter(SentenceSoundCopy, ReusedHpFirKernel, HpFirFilter_FftFormat,,,,,, True, True)

                'Detecting the start position, and level, of the loudest 25 ms window
                Dim WindowSize As Integer = 0.025 * obj.ParentSound.WaveFormat.SampleRate

                'Measuring sentence sound level
                Dim LoudestWindowStartSample As Integer
                Dim WindowLevelList As New List(Of Double)
                Dim LoudestWindowLevel As Double = DSP.GetLevelOfLoudestWindow(FilterredSound, CurrentChannel, WindowSize,,, LoudestWindowStartSample,,, WindowLevelList)

                'Smoothing the WindowLevelList (using FIR filter)
                Dim SmoothNonSound As New STFN.Core.Audio.Sound(SentenceSoundCopy.WaveFormat)
                Dim WindowLevelList_Single As New List(Of Single)
                For Each Value In WindowLevelList
                    WindowLevelList_Single.Add(Value)
                Next
                SmoothNonSound.WaveData.SampleData(1) = WindowLevelList_Single.ToArray

                'DSP.MaxAmplitudeNormalizeSection(SmoothNonSound, CurrentChannel)

                'SmoothNonSound.WriteWaveFile("C:\Temp\PreFilt.wav")

                Dim FilterredLevelArray As STFN.Core.Audio.Sound = DSP.FIRFilter(SmoothNonSound, ReusedLpFirKernel, LpFirFilter_FftFormat,,,,,, True, True)

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
                    StartSample = DSP.GetZeroCrossingSample(SentenceSoundCopy, CurrentChannel, StartSample, DSP.SearchDirections.Earlier)
                    EndSample = DSP.GetZeroCrossingSample(SentenceSoundCopy, CurrentChannel, EndSample, DSP.SearchDirections.Later)
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



    <Extension>
    Public Sub ApplyPaddingSection(obj As SpeechMaterialAnnotation, ByRef Sound As STFN.Core.Audio.Sound, ByRef CurrentChannel As Integer, ByVal PaddingTime As Single)

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
                    obj.ShiftSegmentationData(InitialShift, 0)
                ElseIf InitialShift < 0 Then
                    DSP.DeleteSection(Sound, 0, -InitialShift)
                    obj.ShiftSegmentationData(InitialShift, 0)
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


    <Extension>
    Public Sub ApplyInterSentenceInterval(obj As SpeechMaterialAnnotation, ByVal Interval As Single, ByVal FadeInterval As Boolean, ByRef CurrentChannel As Integer,
                                  Optional FadeType As DSP.FadeSlopeType = DSP.FadeSlopeType.PowerCosine_SkewedFromFadeDirection,
                                  Optional CosinePower As Double = 100, Optional ByVal FadedMarginTime As Single = 0.05)

        Dim FadedMarginLength As Integer = Math.Floor(obj.ParentSound.WaveFormat.SampleRate * FadedMarginTime)
        If FadedMarginLength Mod 2 = 1 Then FadedMarginLength -= 1 'Adjusts FadedMarginLength to an even value
        Dim IntervalLength As Integer = Math.Floor(obj.ParentSound.WaveFormat.SampleRate * Interval)
        If IntervalLength Mod 2 = 1 Then IntervalLength -= 1 'Adjusts IntervalLength to an even value

        'Ensuring that the interval is large enough to room two faded margins
        If 2 * FadedMarginLength > IntervalLength Then
            'Adjusting the FadedMarginLength to be half of the FadedMarginLength
            FadedMarginLength = IntervalLength / 2
        End If

        'Creating a silent sound with the length of IntervalLength - 2 * FadedMarginLength to insert between the sentence level segments
        Dim SilenceLength As Integer = IntervalLength - 2 * FadedMarginLength
        Dim SilentSound = DSP.CreateSilence(obj.ParentSound.WaveFormat, CurrentChannel, SilenceLength, STFN.Core.Audio.BasicAudioEnums.TimeUnits.samples)

        Dim SentenceSoundSections As New List(Of STFN.Core.Audio.Sound)
        'Getting all sentence sound sections including a margin of FadedMarginTime

        Dim SoundChannelData As List(Of Single) = obj.ParentSound.WaveData.SampleData(CurrentChannel).ToList

        For s = 0 To obj.ChannelData(CurrentChannel).Count - 1

            Dim sentence = obj.ChannelData(CurrentChannel)(s)

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

            Dim SentenceSound = New STFN.Core.Audio.Sound(obj.ParentSound.WaveFormat)
            SentenceSound.WaveData.SampleData(CurrentChannel) = SentenceData

            'Fading the margins
            If FadeInterval = True Then
                DSP.Fade(SentenceSound,, 0, CurrentChannel, 0, FadedMarginLength, FadeType, CosinePower)
                DSP.Fade(SentenceSound, 0, , CurrentChannel, SentenceData.Length - FadedMarginLength, FadedMarginLength, FadeType, CosinePower)
            End If

            'Adding the sentence sound
            SentenceSoundSections.Add(SentenceSound)

            'Adding silence between sentnces (not adding after the last sentence
            If s < obj.ChannelData(CurrentChannel).Count - 1 Then
                SentenceSoundSections.Add(SilentSound)
            End If

        Next

        'Concatenates the sounds
        Dim ConcatenatedSound = DSP.ConcatenateSounds(SentenceSoundSections)

        'Storing the concatenated sound array in Me.ParentSound
        obj.ParentSound.WaveData.SampleData(CurrentChannel) = ConcatenatedSound.WaveData.SampleData(CurrentChannel)

        'Adjusts the SMA StartSample positions
        'Fixing the first shift. The new startsample will be at FadedMarginLength
        Dim AccumulativePostShiftStartSample As Integer = 0
        For s = 0 To Math.Min(0, obj.ChannelData(CurrentChannel).Count - 1)

            Dim PreShiftStartSample = obj.ChannelData(CurrentChannel)(s).StartSample
            AccumulativePostShiftStartSample = FadedMarginLength
            Dim Shift = AccumulativePostShiftStartSample - PreShiftStartSample
            obj.ShiftSegmentationData(Shift, PreShiftStartSample, True)

        Next

        For s = 1 To obj.ChannelData(CurrentChannel).Count - 1

            Dim PreShiftStartSample = obj.ChannelData(CurrentChannel)(s).StartSample
            AccumulativePostShiftStartSample += obj.ChannelData(CurrentChannel)(s - 1).Length + IntervalLength
            Dim Shift = AccumulativePostShiftStartSample - PreShiftStartSample
            obj.ShiftSegmentationData(Shift, PreShiftStartSample, True)

        Next

        'Setting the channel length to the new sound data length. 'TODO: feeling intuitively that this should be done elsewhere in a more general way...???
        For s = 0 To obj.ChannelData(CurrentChannel).Count - 1
            obj.ChannelData(CurrentChannel)(s).AlignSegmentationEndsAcrossLevels()
        Next


    End Sub

    ''' <summary>
    ''' Fading the padded sections before the first sentnce and after the last sentence.
    ''' </summary>
    ''' <param name="Sound"></param>
    ''' <param name="CurrentChannel"></param>
    ''' <param name="FadeType"></param>
    ''' <param name="CosinePower"></param>
    <Extension>
    Public Sub FadePaddingSection(obj As SpeechMaterialAnnotation, ByRef Sound As STFN.Core.Audio.Sound, ByRef CurrentChannel As Integer,
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
    ''' Shifts all StartSample indices in the current instance of SpeechMaterialAnnotation located at or after FirstSampleToShift, and limits the StartSample and Length values to the sample indices available in the parent sound file
    ''' </summary>
    ''' <param name="ShiftInSamples"></param>
    '''   <param name="FirstSampleToShift">Applies shift to all StartSample values after FirstSampleToShift.</param>
    <Extension>
    Public Sub ShiftSegmentationData(obj As SpeechMaterialAnnotation, ByVal ShiftInSamples As Integer, ByVal FirstSampleToShift As Integer, Optional ByVal AllowInfinitePositiveShift As Boolean = False)

        For c = 1 To obj.ChannelCount
            Dim Channel = obj.ChannelData(c)
            Dim TotalAvailableLength As Integer
            If AllowInfinitePositiveShift = True Then
                TotalAvailableLength = Integer.MaxValue
            Else
                TotalAvailableLength = obj.ParentSound.WaveData.SampleData(c).Length
            End If
            'Shifting channel is no longer needed as its value are always 0 and the sound length
            'ApplyShift(ShiftInSamples, SoundChannelLength, Channel.StartSample, Channel.Length, FirstSampleToShift)
            For Each Sentence In Channel
                obj.ApplyShift(ShiftInSamples, TotalAvailableLength, Sentence.StartSample, Sentence.Length, FirstSampleToShift)
                For Each Word In Sentence
                    obj.ApplyShift(ShiftInSamples, TotalAvailableLength, Word.StartSample, Word.Length, FirstSampleToShift)
                    For Each Phone In Word
                        obj.ApplyShift(ShiftInSamples, TotalAvailableLength, Phone.StartSample, Phone.Length, FirstSampleToShift)
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
    <Extension>
    Private Sub ApplyShift(obj As SpeechMaterialAnnotation, ByVal Shift As Integer, ByVal TotalAvailableLength As Integer, ByRef StartIndex As Integer, ByRef Length As Integer, ByVal FirstSampleToShift As Integer)

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
    ''' Goes through all segmentation data and detects unset start values and zero lengths. The accumulated number of unset starts, and zero-lengths are returned in the parameters.
    ''' N.B. Presently, only multiple sentences are supported!
    ''' </summary>
    <Extension>
    Public Sub ValidateSegmentation(obj As SpeechMaterialAnnotation, ByRef UnsetStarts As Integer, ByRef ZeroLengths As Integer,
                                    ByRef SetStarts As Integer, ByRef SetLengths As Integer)

        Dim sentence As Integer = 0

        MsgBox("Time to add support for multiple sentences??? Only one sentence per sound file is supported by ValidateSegmentation")

        Dim NumberOfMissingPhonemeArrays As Integer = 0

        'Adjusting times stored in the ptwf phoneme data
        For c = 1 To obj.ChannelCount

            If obj.ChannelData(c)(sentence).StartSample = -1 Then
                UnsetStarts += 1
            Else
                SetStarts += 1
            End If
            If obj.ChannelData(c)(sentence).Length = 0 Then
                ZeroLengths += 1
            Else
                SetLengths += 1
            End If

            For word = 0 To obj.ChannelData(c)(sentence).Count - 1

                If obj.ChannelData(c)(sentence)(word).StartSample = -1 Then
                    UnsetStarts += 1
                Else
                    SetStarts += 1
                End If
                If obj.ChannelData(c)(sentence)(word).Length = 0 Then
                    ZeroLengths += 1
                Else
                    SetLengths += 1
                End If

                If obj.ChannelData(c)(sentence)(word) IsNot Nothing Then
                    For phoneme = 0 To obj.ChannelData(c)(sentence)(word).Count - 1
                        If obj.ChannelData(c)(sentence)(word)(phoneme).StartSample = -1 Then
                            UnsetStarts += 1
                        Else
                            SetStarts += 1
                        End If
                        If obj.ChannelData(c)(sentence)(word)(phoneme).Length = 0 Then
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
    ''' Checks the SegmentationCompleted value of all descentant SmaComponents in the indicated channel is True. Otherwise returns False .
    ''' </summary>
    ''' <returns></returns>
    <Extension>
    Public Function AllSegmentationsCompleted(obj As SpeechMaterialAnnotation, ByVal Channel As Integer) As Boolean

        Dim AllComponents = obj.ChannelData(Channel).GetAllDescentantComponents

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
    ''' Resets the sound levels for each channel, sentence, word and phone of the current SMA object.
    ''' </summary>
    <Extension>
    Public Sub ResetSoundLevels(obj As SpeechMaterialAnnotation)

        For c = 1 To obj.ChannelCount
            obj.ChannelData(c).ResetSoundLevels()
        Next

        'Earlier, when not extension method:
        'For Each c In _ChannelData
        '    c.ResetSoundLevels()
        'Next

    End Sub


    ''' <summary>
    ''' This sub resets segmentation time data (startSample and Length) stored in the current SMA object.
    ''' </summary>
    <Extension>
    Public Sub ResetTemporalData(obj As SpeechMaterialAnnotation)

        For c = 1 To obj.ChannelCount
            obj.ChannelData(c).ResetTemporalData()
        Next

        'Earlier, when not extension method:
        'For Each c In _ChannelData
        '    c.ResetTemporalData()
        'Next

    End Sub

    ''' <summary>
    ''' Converts all instances of a specified phone to a new phone
    ''' </summary>
    ''' <param name="CurrentPhone"></param>
    ''' <param name="NewPhone"></param>
    <Extension>
    Public Sub ConvertPhone(obj As SpeechMaterialAnnotation, ByVal CurrentPhone As String, ByVal NewPhone As String)

        For c = 1 To obj.ChannelCount
            obj.ChannelData(c).ConvertPhone(CurrentPhone, NewPhone)
        Next

        'Earlier, when not extension method:
        'For Each c In _ChannelData
        '    c.ConvertPhone(CurrentPhone, NewPhone)
        'Next

    End Sub


    <Extension>
    Public Function GetSentenceSegmentationsString(obj As SpeechMaterialAnnotation, ByVal IncludeHeadings As Boolean) As String

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

        For c = 1 To obj.ChannelCount
            For s = 0 To obj.ChannelData(c).Count - 1
                Dim Sentence = obj.ChannelData(c)(s)
                Dim SentenceList As New List(Of String)
                SentenceList.Add(c)
                SentenceList.Add(s)
                SentenceList.Add(Sentence.OrthographicForm)
                SentenceList.Add(Sentence.PhoneticForm)
                SentenceList.Add(Sentence.StartTime)
                SentenceList.Add(Sentence.Length / obj.ParentSound.WaveFormat.SampleRate)
                SentenceList.Add(Sentence.StartSample)
                SentenceList.Add(Sentence.Length)
                SentenceList.Add(Sentence.SegmentationCompleted.ToString)
                OutputList.Add(String.Join(vbTab, SentenceList))
            Next
        Next

        'Earlier, when not extension method:
        'For c = 0 To _ChannelData.Count - 1
        '    For s = 0 To _ChannelData(c).Count - 1
        '        Dim Sentence = _ChannelData(c)(s)
        '        Dim SentenceList As New List(Of String)
        '        SentenceList.Add(c)
        '        SentenceList.Add(s)
        '        SentenceList.Add(Sentence.OrthographicForm)
        '        SentenceList.Add(Sentence.PhoneticForm)
        '        SentenceList.Add(Sentence.StartTime)
        '        SentenceList.Add(Sentence.Length / obj.ParentSound.WaveFormat.SampleRate)
        '        SentenceList.Add(Sentence.StartSample)
        '        SentenceList.Add(Sentence.Length)
        '        SentenceList.Add(Sentence.SegmentationCompleted.ToString)
        '        OutputList.Add(String.Join(vbTab, SentenceList))
        '    Next
        'Next

        If IncludeHeadings = True Then
            Return String.Join(vbTab, HeadingList) & vbCrLf & String.Join(vbCrLf, OutputList)
        Else
            Return String.Join(vbCrLf, OutputList)
        End If

    End Function


#Region "MultiWordRecordings"

    'N.B. These methods were written when the SMA only had support for one sentence per channel. They should all be re-written!

    ''' <summary>
    ''' Adjusts the sound level of each ptwf word in the input sound, so that each carrier phrase gets the indicated target level. The level of each test word is adjusted together with its carrier phrase.
    ''' </summary>
    ''' <param name="InputSound"></param>
    <Extension>
    Public Sub MultiWord_SoundLevelEqualization(obj As SpeechMaterialAnnotation, ByRef InputSound As STFN.Core.Audio.Sound, ByVal SoundChannel As Integer, Optional ByVal TargetLevel As Double = -30, Optional ByVal PaddingDuration As Double = 0.5)

        Dim sentence As Integer = 0

        Try

            Dim PtwfChannel As Integer = 1 'This should be a parameter if multi channel ptwfs are needed

            'Setting a padding interval
            Dim PaddingSamples As Double = PaddingDuration * InputSound.WaveFormat.SampleRate

            If InputSound IsNot Nothing Then

                For WordIndex = 0 To obj.ChannelData(PtwfChannel)(sentence).Count - 1

                    'Measures the carries phrases
                    Dim CarrierLevel As Double = DSP.MeasureSectionLevel(InputSound, SoundChannel,
                                                                           obj.ChannelData(PtwfChannel)(sentence)(WordIndex)(0).StartSample,
                                                                           obj.ChannelData(PtwfChannel)(sentence)(WordIndex)(0).Length)

                    'Calculating gain
                    Dim Gain As Double = TargetLevel - CarrierLevel

                    'Applying gain to the whole word
                    DSP.AmplifySection(InputSound, Gain, SoundChannel, obj.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample - PaddingSamples, obj.ChannelData(PtwfChannel)(sentence)(WordIndex).Length + 2 * PaddingSamples)

                Next

            End If
        Catch ex As Exception
            MsgBox("An error occurred: " & ex.ToString)
        End Try

    End Sub

    <Extension>
    Public Sub MultiWord_SetInterStimulusInterval(obj As SpeechMaterialAnnotation, ByRef InputSound As STFN.Core.Audio.Sound, Optional ByVal Interval As Double = 3)

        Dim sentence As Integer = 0

        Try

            Dim PtwfChannel As Integer = 1 'This should be a parameter if multi channel ptwfs are needed

            Dim StimulusIntervalInSamples As Double = Interval * InputSound.WaveFormat.SampleRate

            If InputSound IsNot Nothing Then

                For WordIndex = 0 To obj.ChannelData(PtwfChannel)(sentence).Count - 2

                    'Getting the interval to the next word
                    Dim CurrentInterval As Integer =
                    obj.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample -
                    (obj.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample + obj.ChannelData(PtwfChannel)(sentence)(WordIndex).Length)

                    'Calculating adjustment length
                    Dim AdjustmentLength As Integer = StimulusIntervalInSamples - CurrentInterval

                    Select Case AdjustmentLength
                        Case < 0
                            'Deleting a segment right between the words
                            Dim StartDeleteSample As Integer = obj.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample - (CurrentInterval / 2) - (Math.Abs(AdjustmentLength / 2))
                            DSP.DeleteSection(InputSound, StartDeleteSample, Math.Abs(AdjustmentLength))

                        Case > 0
                            'Inserting a segment
                            Dim StartInsertSample As Integer = obj.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample - (CurrentInterval / 2) - (Math.Abs(AdjustmentLength / 2))
                            DSP.InsertSilentSection(InputSound, StartInsertSample, Math.Abs(AdjustmentLength))

                    End Select

                    'Adjusting the ptwf data of all following words
                    For FollowingWordIndex = WordIndex + 1 To obj.ChannelData(PtwfChannel)(sentence).Count - 1
                        obj.ChannelData(PtwfChannel)(sentence)(FollowingWordIndex).StartSample += AdjustmentLength
                        For p = 0 To obj.ChannelData(PtwfChannel)(sentence)(FollowingWordIndex).Count - 1
                            obj.ChannelData(PtwfChannel)(sentence)(FollowingWordIndex)(p).StartSample += AdjustmentLength
                        Next
                    Next

                Next

            End If
        Catch ex As Exception
            MsgBox("An error occurred: " & ex.ToString)
        End Try

    End Sub

    <Extension>
    Public Sub MultiWord_InterStimulusSectionFade(obj As SpeechMaterialAnnotation, ByRef InputSound As STFN.Core.Audio.Sound, ByVal SoundChannel As Integer, Optional ByVal FadeTime As Double = 0.5)

        Dim sentence As Integer = 0

        Try

            Dim PtwfChannel As Integer = 1 'This should be a parameter if multi channel ptwfs are needed

            'Setting a stimulus interval
            Dim GeneralFadeLength As Double = FadeTime * InputSound.WaveFormat.SampleRate

            If InputSound IsNot Nothing Then

                'Fades in the first sound
                DSP.Fade(InputSound, Nothing, 0, SoundChannel, 0, obj.ChannelData(PtwfChannel)(sentence)(0).StartSample, DSP.FadeSlopeType.PowerCosine_SkewedFromFadeDirection, 20)

                'Fades out the last sound
                Dim FadeLastStart As Integer = obj.ChannelData(PtwfChannel)(sentence)(obj.ChannelData(PtwfChannel)(sentence).Count - 1).StartSample + obj.ChannelData(PtwfChannel)(sentence)(obj.ChannelData(PtwfChannel)(sentence).Count - 1).Length
                DSP.Fade(InputSound, 0, Nothing, SoundChannel, FadeLastStart, , DSP.FadeSlopeType.PowerCosine_SkewedFromFadeDirection, 20)

                'Fades all inter-stimulis sections
                For WordIndex = 0 To obj.ChannelData(PtwfChannel)(sentence).Count - 2

                    'Getting the interval to the next word
                    Dim CurrentInterval As Integer =
                    obj.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample -
                    (obj.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample + obj.ChannelData(PtwfChannel)(sentence)(WordIndex).Length)


                    'Determines a suitable fade length
                    Dim FadeLength As Integer = Math.Min((CurrentInterval / 2) + 2, GeneralFadeLength)

                    'Fades out the current word
                    DSP.Fade(InputSound, 0, Nothing, SoundChannel, obj.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample + obj.ChannelData(PtwfChannel)(sentence)(WordIndex).Length, FadeLength)

                    'Fades in the next sound
                    DSP.Fade(InputSound, Nothing, 0, SoundChannel, obj.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample - FadeLength, FadeLength)

                    'Silencing the section between the fades
                    Dim SilenceStart As Integer = obj.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample + obj.ChannelData(PtwfChannel)(sentence)(WordIndex).Length
                    Dim SilenceLength As Integer = (obj.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample - FadeLength) - SilenceStart
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
    <Extension>
    Public Function MultiWord_MeasureSoundLevelsOfSections(obj As SpeechMaterialAnnotation, ByRef InputSound As STFN.Core.Audio.Sound, ByVal SoundChannel As Integer,
                                                  ByVal MeasurementSection As MeasurementSections,
                                              Optional ByVal FrequencyWeighting As STFN.Core.Audio.FrequencyWeightings = STFN.Core.Audio.FrequencyWeightings.Z,
                                                       Optional DurationList As List(Of Double) = Nothing,
                                                       Optional LengthList As List(Of Integer) = Nothing) As Double

        Dim sentence As Integer = 0

        Dim PtwfChannel As Integer = 1 'This should be a parameter if multi channel ptwfs are needed

        Dim FirstPtwfPhonemeIndexToMeasure As Integer = 0
        Dim LastPtwfPhonemeIndexToMeasure As Integer = 0
        Select Case MeasurementSection
            Case MeasurementSections.CarrierPhrases
                FirstPtwfPhonemeIndexToMeasure = 0
                LastPtwfPhonemeIndexToMeasure = 0
            Case MeasurementSections.TestWords
                FirstPtwfPhonemeIndexToMeasure = 1
                LastPtwfPhonemeIndexToMeasure = 1
            Case MeasurementSections.CarriersAndTestWords
                FirstPtwfPhonemeIndexToMeasure = 0
                LastPtwfPhonemeIndexToMeasure = 1
            Case Else
                MsgBox("An error occurred!")
                Return Nothing
        End Select

        Try

            If InputSound IsNot Nothing Then

                Dim MeasurementSound As New STFN.Core.Audio.Sound(InputSound.WaveFormat)

                'Gets the length of the MeasurementSound
                Dim TotalLength As Integer = 0
                For WordIndex = 1 To obj.ChannelData(PtwfChannel)(sentence).Count - 2
                    Dim CurrentLength As Integer = (obj.ChannelData(PtwfChannel)(sentence)(WordIndex)(LastPtwfPhonemeIndexToMeasure).StartSample +
                    obj.ChannelData(PtwfChannel)(sentence)(WordIndex)(LastPtwfPhonemeIndexToMeasure).Length) -
                    obj.ChannelData(PtwfChannel)(sentence)(WordIndex)(FirstPtwfPhonemeIndexToMeasure).StartSample

                    'Adding the duration
                    If DurationList IsNot Nothing Then DurationList.Add(CurrentLength / InputSound.WaveFormat.SampleRate)
                    If LengthList IsNot Nothing Then LengthList.Add(CurrentLength)

                    TotalLength += CurrentLength
                Next
                Dim NewArray(TotalLength - 1) As Single
                MeasurementSound.WaveData.SampleData(SoundChannel) = NewArray


                'Copies all test word sections to a the MeasurementSound
                Dim CurrentReadSample As Integer = 0

                For WordIndex = 1 To obj.ChannelData(PtwfChannel)(sentence).Count - 2
                    For s = obj.ChannelData(PtwfChannel)(sentence)(WordIndex)(FirstPtwfPhonemeIndexToMeasure).StartSample To obj.ChannelData(PtwfChannel)(sentence)(WordIndex)(LastPtwfPhonemeIndexToMeasure).StartSample + obj.ChannelData(PtwfChannel)(sentence)(WordIndex)(LastPtwfPhonemeIndexToMeasure).Length - 1
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


End Module
