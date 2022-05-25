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

            <NonSerialized>
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

            <NonSerialized>
            Public ParentSound As Sound

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
                _ChannelData.Add(New SmaComponent(Me, SmaTags.CHANNEL, Nothing))
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


            Private FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z
            Public Function GetFrequencyWeighting() As FrequencyWeightings
                Return FrequencyWeighting
            End Function

            Public Sub SetFrequencyWeighting(ByVal FrequencyWeighting As FrequencyWeightings, ByVal EnforceOnAllDescendents As Boolean)

                'Setting only SMA top level frequency weighting
                FrequencyWeighting = FrequencyWeighting

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
                TimeWeighting = TimeWeighting

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

                'MsgBox("Check the code below for accuracy!!!")

                'Adjusting the StartSample and limiting it to the available range
                StartIndex = Math.Min(Math.Max(StartIndex + Shift, 0), TotalAvailableLength - 1)

                'Limiting Length to the available length after adjustment of startsample
                Dim MaximumPossibleLength = TotalAvailableLength - StartIndex
                Length = Math.Max(0, Math.Min(Length, MaximumPossibleLength))

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
                            ShiftSegmentationData(InitialShift)
                        ElseIf InitialShift < 0 Then
                            DSP.DeleteSection(Sound, 0, -InitialShift)
                            ShiftSegmentationData(InitialShift)
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
                                  Optional CosinePower As Double = 100)

                Throw New NotImplementedException

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

            <Serializable>
            Public Class SmaComponent
                Inherits List(Of SmaComponent)

                Public Property ParentSMA As SpeechMaterialAnnotation

                Public Property ParentComponent As SmaComponent

                Public Property SmaTag As SpeechMaterialAnnotation.SmaTags

                Public Property SegmentationCompleted As Boolean = False

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

                Public Sub New(ByRef ParentSMA As SpeechMaterialAnnotation, ByVal SmaLevel As SmaTags, ByRef ParentComponent As SmaComponent)
                    Me.ParentSMA = ParentSMA
                    Me.ParentComponent = ParentComponent
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
                Public Sub MeasureSoundLevels(ByVal c As Integer, ByRef AttemptedMeasurementCount As Integer, ByRef SuccesfullMeasurementsCount As Integer)

                    Dim ParentSound = ParentSMA.ParentSound

                    'Checks that parent sound has enough channels
                    If ParentSound.WaveFormat.Channels >= c Then

                        'Checks that parent the current channel of the parent sound contains sounds
                        If ParentSound.WaveData.SampleData(c).Length > 0 Then

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
                            Return OrthographicForm & vbCrLf & "[" & PhoneticForm & "]"
                        ElseIf OrthographicForm <> "" Then
                            Return OrthographicForm
                        Else
                            Return "[" & PhoneticForm & "]"
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

                Public Function GetUnbrokenLineOfAncestorsWithoutSiblings() As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

                    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                    If ParentComponent IsNot Nothing Then
                        If ParentComponent.GetNumberOfSiblingsExcludingSelf = 0 Then
                            OutputList.AddRange(ParentComponent)
                            OutputList.AddRange(ParentComponent.GetUnbrokenLineOfAncestorsWithoutSiblings)
                        End If
                    End If

                    Return OutputList

                End Function

                ''' <summary>
                ''' Returns all SmaComponents that locically share the same StartSample value as the current instance of SmaComponent 
                ''' </summary>
                ''' <returns></returns>
                Public Function GetDependentSegmentationsStarts() As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                    OutputList.AddRange(GetDependentSegmentationsStarts(HierarchicalDirections.Upwards))
                    OutputList.AddRange(GetDependentSegmentationsStarts(HierarchicalDirections.Downwards))
                    Return OutputList
                End Function

                Private Function GetDependentSegmentationsStarts(ByVal HierarchicalDirection As HierarchicalDirections) As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

                    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

                    OutputList.Add(Me)

                    'Cascading up or down
                    Select Case HierarchicalDirection
                        Case HierarchicalDirections.Upwards

                            Dim SelfIndex = GetSelfIndex()
                            If SelfIndex.HasValue Then
                                If SelfIndex = 0 Then
                                    OutputList.AddRange(ParentComponent.GetDependentSegmentationsStarts(HierarchicalDirection))
                                End If
                            End If

                        Case HierarchicalDirections.Downwards

                            If Me.Count > 0 Then
                                OutputList.AddRange(Me(0).GetDependentSegmentationsStarts(HierarchicalDirection))
                            End If

                    End Select

                    Return OutputList

                End Function

                ''' <summary>
                ''' Returns all SmaComponents that locically share the same end time as the current instance of SmaComponent 
                ''' </summary>
                ''' <returns></returns>
                Public Function GetDependentSegmentationsEnds() As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                    OutputList.AddRange(GetDependentSegmentationsEnds(HierarchicalDirections.Upwards))
                    OutputList.AddRange(GetDependentSegmentationsEnds(HierarchicalDirections.Downwards))
                    Return OutputList
                End Function

                Public Function GetDependentSegmentationsEnds(ByVal HierarchicalDirection As HierarchicalDirections) As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

                    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

                    OutputList.Add(Me)

                    'Cascading up or down
                    Select Case HierarchicalDirection
                        Case HierarchicalDirections.Upwards

                            Dim SelfIndex = GetSelfIndex()
                            If SelfIndex.HasValue Then
                                Dim SiblingCount = GetSiblings.Count
                                If SelfIndex = SiblingCount - 1 Then
                                    'The current instance is the last of its same level components
                                    OutputList.AddRange(ParentComponent.GetDependentSegmentationsEnds(HierarchicalDirection))
                                End If
                            End If

                        Case HierarchicalDirections.Downwards

                            If Me.Count > 0 Then
                                OutputList.AddRange(Me(Me.Count - 1).GetDependentSegmentationsEnds(HierarchicalDirection))
                            End If

                    End Select

                    Return OutputList

                End Function

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

                    MoveStart(NewStartSample, SoundLength)

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

                    'Storing the new StartSample
                    StartSample = TempStartSample

                    'Adjusting the Length
                    Length = ExclusiveEndSample - StartSample

                End Sub

                ''' <summary>
                ''' Aligns the end of a segmentation set on the current level to the ends of dependent segmentation on all other levels, by adjusting their lengths, and if needed also their start positions, in case lengths gets reduced to zero.
                ''' </summary>
                ''' <param name="NewExclusiveEndSample"></param>
                ''' <param name="HierarchicalDirection"></param>
                Private Sub AlignSegmentationEnds(ByVal NewExclusiveEndSample As Integer, ByVal HierarchicalDirection As HierarchicalDirections)

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

                            'Locks the length of sibling i to the start position (-1) of the sibling i+1
                            Siblings(i).Length = Siblings(i + 1).StartSample - Siblings(i).StartSample
                        Next
                    End If

                End Sub

                ''' <summary>
                ''' Sets the value of the SegmentationCompleted property and optinally cascades that value to all descendant components.
                ''' </summary>
                ''' <param name="Value"></param>
                ''' <param name="CascadeToAllDescendants"></param>
                Public Sub SetSegmentationCompleted(ByVal Value As Boolean, ByVal CascadeToAllDescendants As Boolean)
                    SegmentationCompleted = Value
                    If CascadeToAllDescendants = True Then
                        For Each child In Me
                            child.SetSegmentationCompleted(Value, CascadeToAllDescendants)
                        Next
                    End If
                End Sub

            End Class

        End Class

    End Class



End Namespace
