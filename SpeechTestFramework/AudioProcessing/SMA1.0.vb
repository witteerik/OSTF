
'        ''' <summary>
'        ''' A class used to store data related to the segmentation of speech material sound files. All data can be contained and saved in a wave file.
'        ''' </summary>
'        <Serializable>
'        Class SpeechMaterialAnnotation

'            Public Property ParentSound As Sound

'            Public Const CurrentVersion As String = "1.0"
'            ''' <summary>
'            ''' Holds the SMA version of the file that the data was loaded from, or CurrentVersion if the data was not loaded from file.
'            ''' </summary>
'            Public ReadFromVersion As String = CurrentVersion ' Using CurrentVersion as default

'            Public ReadOnly Property ChannelCount As Integer
'                Get
'                    Return _ChannelData.Count
'                End Get
'            End Property

'            Private _ChannelData As New List(Of SmaChannelData)
'            Property ChannelData(ByVal Channel As Integer) As SmaChannelData
'                Get
'                    Return _ChannelData(Channel - 1)
'                End Get
'                Set(value As SmaChannelData)
'                    _ChannelData(Channel - 1) = value
'                End Set
'            End Property

'            Public Sub AddChannelData()
'                _ChannelData.Add(New SmaChannelData(Me))
'            End Sub

'            Public Sub AddChannelData(ByRef NewSmaChannelData As SmaChannelData)
'                _ChannelData.Add(NewSmaChannelData)
'            End Sub

'            ''' <summary>
'            ''' Creates a new instance of SpeechMaterialAnnotation
'            ''' </summary>
'            Public Sub New(Optional ByVal DefaultFrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z,
'                           Optional ByVal DefaultTimeWeighting As Double = 0)
'                SetFrequencyWeighting(DefaultFrequencyWeighting, False)
'                SetTimeWeighting(DefaultTimeWeighting, False)
'            End Sub

'            Private _FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z
'            Public Function GetFrequencyWeighting() As FrequencyWeightings
'                Return _FrequencyWeighting
'            End Function

'            Public Sub SetFrequencyWeighting(ByVal FrequencyWeighting As FrequencyWeightings, ByVal EnforceOnAllDescendents As Boolean)

'                'Setting only SMA top level frequency weighting
'                _FrequencyWeighting = FrequencyWeighting

'                If EnforceOnAllDescendents = True Then

'                    'Enforcing the same frequency weighting on all descendant channels, sentences, words, and phones
'                    For c = 0 To _ChannelData.Count - 1
'                        _ChannelData(c).SetFrequencyWeighting(FrequencyWeighting, False)
'                        For s = 0 To _ChannelData(c).Count - 1
'                            _ChannelData(c)(s).SetFrequencyWeighting(FrequencyWeighting, False)
'                            For w = 0 To _ChannelData(c)(s).Count - 1
'                                _ChannelData(c)(s)(w).SetFrequencyWeighting(FrequencyWeighting, False)
'                                For p = 0 To _ChannelData(c)(s)(w).PhoneData.Count - 1
'                                    _ChannelData(c)(s)(w).PhoneData(p).FrequencyWeighting = FrequencyWeighting
'                                Next
'                            Next
'                        Next
'                    Next
'                End If
'            End Sub

'            Private _TimeWeighting As Double = 0
'            Public Function GetTimeWeighting() As Double
'                Return _TimeWeighting
'            End Function

'            Public Sub SetTimeWeighting(ByVal TimeWeighting As Double, ByVal EnforceOnAllDescendents As Boolean)

'                'Setting only SMA top level Time weighting
'                _TimeWeighting = TimeWeighting

'                If EnforceOnAllDescendents = True Then

'                    'Enforcing the same Time weighting on all descendant channels, sentences, words, and phones
'                    For c = 0 To _ChannelData.Count - 1
'                        _ChannelData(c).SetTimeWeighting(TimeWeighting, False)
'                        For s = 0 To _ChannelData(c).Count - 1
'                            _ChannelData(c)(s).SetTimeWeighting(TimeWeighting, False)
'                            For w = 0 To _ChannelData(c)(s).Count - 1
'                                _ChannelData(c)(s)(w).SetTimeWeighting(TimeWeighting, False)
'                                For p = 0 To _ChannelData(c)(s)(w).PhoneData.Count - 1
'                                    _ChannelData(c)(s)(w).PhoneData(p).TimeWeighting = TimeWeighting
'                                Next
'                            Next
'                        Next
'                    Next
'                End If
'            End Sub

'            Public Shadows Function ToString(ByVal IncludeHeadings As Boolean) As String

'                Dim OutputList As New List(Of String)

'                If IncludeHeadings = True Then

'                    OutputList.Add("SMA")
'                    OutputList.Add("SMA_VERSION:" & vbTab & Sound.SpeechMaterialAnnotation.CurrentVersion) 'I.e. SMA version number

'                    For channel As Integer = 1 To ChannelCount

'                        'Writing channel data
'                        OutputList.Add("CHANNEL")
'                        OutputList.Add("ORTHOGRAPHIC_FORM")
'                        OutputList.Add("PHONETIC_FORM")

'                        OutputList.Add("START_SAMPLE")
'                        OutputList.Add("LENGTH")

'                        If ChannelData(channel).UnWeightedLevel IsNot Nothing Then
'                            OutputList.Add("UNWEIGHTED_LEVEL")
'                        End If

'                        If ChannelData(channel).UnWeightedPeakLevel IsNot Nothing Then
'                            OutputList.Add("UNWEIGHTED_PEAKLEVEL")
'                        End If

'                        If ChannelData(channel).WeightedLevel IsNot Nothing Then
'                            OutputList.Add("WEIGHTED_LEVEL")
'                        End If
'                        OutputList.Add("FREQUENCY_WEIGHTING")
'                        OutputList.Add("TIME_WEIGHTING")

'                        For sentence As Integer = 0 To ChannelData(channel).Count - 1

'                            'Writing sentence data
'                            OutputList.Add("SENTENCE")
'                            OutputList.Add("ORTHOGRAPHIC_FORM")
'                            OutputList.Add("PHONETIC_FORM")
'                            OutputList.Add("START_SAMPLE")
'                            OutputList.Add("LENGTH")
'                            OutputList.Add("UNWEIGHTED_LEVEL")
'                            OutputList.Add("UNWEIGHTED_PEAKLEVEL")
'                            OutputList.Add("WEIGHTED_LEVEL")
'                            OutputList.Add("FREQUENCY_WEIGHTING")
'                            OutputList.Add("TIME_WEIGHTING")
'                            OutputList.Add("INITIAL_PEAK")
'                            OutputList.Add("START_TIME")

'                            For word = 0 To ChannelData(channel)(sentence).Count - 1

'                                'writing word data
'                                OutputList.Add("WORD")
'                                OutputList.Add("ORTHOGRAPHIC_FORM")
'                                OutputList.Add("PHONETIC_FORM")
'                                OutputList.Add("START_SAMPLE")
'                                OutputList.Add("LENGTH")
'                                OutputList.Add("UNWEIGHTED_LEVEL")
'                                OutputList.Add("UNWEIGHTED_PEAKLEVEL")
'                                OutputList.Add("WEIGHTED_LEVEL")
'                                OutputList.Add("FREQUENCY_WEIGHTING")
'                                OutputList.Add("TIME_WEIGHTING")
'                                OutputList.Add("START_TIME")

'                                For phone = 0 To ChannelData(channel)(sentence)(word).PhoneData.Count - 1

'                                    'writing phone data
'                                    OutputList.Add("PHONE")
'                                    OutputList.Add("ORTHOGRAPHIC_FORM")
'                                    OutputList.Add("PHONETIC_FORM")
'                                    OutputList.Add("START_SAMPLE")
'                                    OutputList.Add("LENGTH")
'                                    OutputList.Add("UNWEIGHTED_LEVEL")
'                                    OutputList.Add("UNWEIGHTED_PEAKLEVEL")
'                                    OutputList.Add("WEIGHTED_LEVEL")
'                                    OutputList.Add("FREQUENCY_WEIGHTING")
'                                    OutputList.Add("TIME_WEIGHTING")

'                                    'End of phone
'                                Next
'                                'End of word
'                            Next
'                            'End of sentence
'                        Next
'                        'End of channel
'                    Next
'                End If

'                'Start writing data
'                Dim DefaultNotMeasuredValue As String = "Not measured"
'                If IncludeHeadings = True Then
'                    OutputList.Add(vbCrLf)
'                Else
'                End If

'                For channel As Integer = 1 To ChannelCount

'                    'Writing channel data

'                    OutputList.Add(channel)

'                    OutputList.Add(ChannelData(channel).OrthographicForm.ToString)
'                    OutputList.Add(ChannelData(channel).PhoneticForm.ToString)

'                    OutputList.Add(ChannelData(channel).StartSample)
'                    OutputList.Add(ChannelData(channel).Length)

'                    If ChannelData(channel).UnWeightedLevel IsNot Nothing Then
'                        OutputList.Add(ChannelData(channel).UnWeightedLevel.Value.ToString(InvariantCulture))
'                    Else
'                        OutputList.Add(DefaultNotMeasuredValue)
'                    End If

'                    If ChannelData(channel).UnWeightedPeakLevel IsNot Nothing Then
'                        OutputList.Add(ChannelData(channel).UnWeightedPeakLevel.Value.ToString(InvariantCulture))
'                    Else
'                        OutputList.Add(DefaultNotMeasuredValue)
'                    End If

'                    If ChannelData(channel).WeightedLevel IsNot Nothing Then
'                        OutputList.Add(ChannelData(channel).WeightedLevel.Value.ToString(InvariantCulture))
'                    Else
'                        OutputList.Add(DefaultNotMeasuredValue)
'                    End If
'                    OutputList.Add(ChannelData(channel).GetFrequencyWeighting.ToString)
'                    OutputList.Add(ChannelData(channel).GetTimeWeighting.ToString(InvariantCulture))

'                    For sentence As Integer = 0 To ChannelData(channel).Count - 1

'                        'Writing sentence data
'                        OutputList.Add(sentence)
'                        OutputList.Add(ChannelData(channel)(sentence).OrthographicForm)
'                        OutputList.Add(ChannelData(channel)(sentence).PhoneticForm)
'                        OutputList.Add(ChannelData(channel)(sentence).StartSample)
'                        OutputList.Add(ChannelData(channel)(sentence).Length)
'                        If ChannelData(channel)(sentence).UnWeightedLevel IsNot Nothing Then
'                            OutputList.Add(ChannelData(channel)(sentence).UnWeightedLevel.Value.ToString(InvariantCulture))
'                        Else
'                            OutputList.Add(DefaultNotMeasuredValue)
'                        End If

'                        If ChannelData(channel)(sentence).UnWeightedPeakLevel IsNot Nothing Then
'                            OutputList.Add(ChannelData(channel)(sentence).UnWeightedPeakLevel.Value.ToString(InvariantCulture))
'                        Else
'                            OutputList.Add(DefaultNotMeasuredValue)
'                        End If

'                        If ChannelData(channel)(sentence).WeightedLevel IsNot Nothing Then
'                            OutputList.Add(ChannelData(channel)(sentence).WeightedLevel.Value.ToString(InvariantCulture))
'                        Else
'                            OutputList.Add(DefaultNotMeasuredValue)
'                        End If
'                        OutputList.Add(ChannelData(channel)(sentence).GetFrequencyWeighting.ToString)
'                        OutputList.Add(ChannelData(channel)(sentence).GetTimeWeighting.ToString(InvariantCulture))

'                        OutputList.Add(ChannelData(channel)(sentence).InitialPeak.ToString(InvariantCulture))
'                        OutputList.Add(ChannelData(channel)(sentence).StartTime.ToString(InvariantCulture))

'                        For word = 0 To ChannelData(channel)(sentence).Count - 1

'                            'writing word data
'                            OutputList.Add(word)
'                            OutputList.Add(ChannelData(channel)(sentence)(word).OrthographicForm)
'                            OutputList.Add(ChannelData(channel)(sentence)(word).PhoneticForm)
'                            OutputList.Add(ChannelData(channel)(sentence)(word).StartSample)
'                            OutputList.Add(ChannelData(channel)(sentence)(word).Length)

'                            If ChannelData(channel)(sentence)(word).UnWeightedLevel IsNot Nothing Then
'                                OutputList.Add(ChannelData(channel)(sentence)(word).UnWeightedLevel.Value.ToString(InvariantCulture))
'                            Else
'                                OutputList.Add(DefaultNotMeasuredValue)
'                            End If

'                            If ChannelData(channel)(sentence)(word).UnWeightedPeakLevel IsNot Nothing Then
'                                OutputList.Add(ChannelData(channel)(sentence)(word).UnWeightedPeakLevel.Value.ToString(InvariantCulture))
'                            Else
'                                OutputList.Add(DefaultNotMeasuredValue)
'                            End If

'                            If ChannelData(channel)(sentence)(word).WeightedLevel IsNot Nothing Then
'                                OutputList.Add(ChannelData(channel)(sentence)(word).WeightedLevel.Value.ToString(InvariantCulture))
'                            Else
'                                OutputList.Add(DefaultNotMeasuredValue)
'                            End If
'                            OutputList.Add(ChannelData(channel)(sentence)(word).GetFrequencyWeighting.ToString)
'                            OutputList.Add(ChannelData(channel)(sentence)(word).GetTimeWeighting.ToString(InvariantCulture))

'                            OutputList.Add(ChannelData(channel)(sentence)(word).StartTime.ToString(InvariantCulture))

'                            For phone = 0 To ChannelData(channel)(sentence)(word).PhoneData.Count - 1

'                                'writing phone data
'                                OutputList.Add(phone)
'                                OutputList.Add(ChannelData(channel)(sentence)(word).PhoneData(phone).OrthographicForm)
'                                OutputList.Add(ChannelData(channel)(sentence)(word).PhoneData(phone).PhoneticForm)
'                                OutputList.Add(ChannelData(channel)(sentence)(word).PhoneData(phone).StartSample)
'                                OutputList.Add(ChannelData(channel)(sentence)(word).PhoneData(phone).Length)

'                                If ChannelData(channel)(sentence)(word).PhoneData(phone).UnWeightedLevel IsNot Nothing Then
'                                    OutputList.Add(ChannelData(channel)(sentence)(word).PhoneData(phone).UnWeightedLevel.Value.ToString(InvariantCulture))
'                                Else
'                                    OutputList.Add(DefaultNotMeasuredValue)
'                                End If

'                                If ChannelData(channel)(sentence)(word).PhoneData(phone).UnWeightedPeakLevel IsNot Nothing Then
'                                    OutputList.Add(ChannelData(channel)(sentence)(word).PhoneData(phone).UnWeightedPeakLevel.Value.ToString(InvariantCulture))
'                                Else
'                                    OutputList.Add(DefaultNotMeasuredValue)
'                                End If

'                                If ChannelData(channel)(sentence)(word).PhoneData(phone).WeightedLevel IsNot Nothing Then
'                                    OutputList.Add(ChannelData(channel)(sentence)(word).PhoneData(phone).WeightedLevel.Value.ToString(InvariantCulture))
'                                Else
'                                    OutputList.Add(DefaultNotMeasuredValue)
'                                End If
'                                OutputList.Add(ChannelData(channel)(sentence)(word).PhoneData(phone).FrequencyWeighting.ToString)
'                                OutputList.Add(ChannelData(channel)(sentence)(word).PhoneData(phone).TimeWeighting.ToString(InvariantCulture))

'                                'End of phone
'                            Next
'                            'End of word
'                        Next
'                        'End of sentence
'                    Next
'                    'End of channel
'                Next

'                Return String.Join(vbTab, OutputList)

'            End Function


'            ''' <summary>
'            ''' Converts all instances of a specified phone to a new phone
'            ''' </summary>
'            ''' <param name="CurrentPhone"></param>
'            ''' <param name="NewPhone"></param>
'            Public Sub ConvertPhone(ByVal CurrentPhone As String, ByVal NewPhone As String)
'                For Each Channel In Me._ChannelData
'                    For Each Sentence In Channel
'                        For Each Word In Sentence

'                            Word.PhoneticForm = Word.PhoneticForm.Replace(CurrentPhone, NewPhone)

'                            For Each Phone In Word.PhoneData
'                                Phone.PhoneticForm = Phone.PhoneticForm.Replace(CurrentPhone, NewPhone)
'                            Next
'                        Next
'                    Next
'                Next
'            End Sub


'            ''' <summary>
'            ''' Measures sound levels for each channel, sentence, word and phone of the current SMA object.
'            ''' </summary>
'            ''' <returns>Returns True if all measurements were successful, and False if one or more measurements failed.</returns>
'            Public Function MeasureSoundLevels(Optional ByVal LogMeasurementResults As Boolean = False, Optional ByVal LogFolder As String = "") As Boolean

'                If ParentSound Is Nothing Then
'                    Throw New Exception("The parent sound if the current instance of SpeechMaterialAnnotation cannot be Nothing!")
'                End If

'                Dim SuccesfullMeasurementsCount As Integer = 0
'                Dim AttemptedMeasurementCount As Integer = 0

'                'Measuring each channel 
'                For c As Integer = 1 To ChannelCount

'                    'Measuring on channel level 

'                    'Skips to next if the parent sound does not have enough channels
'                    If ParentSound.WaveFormat.Channels < c Then Continue For

'                    'Skips to next if the current channel of the parent sound does not contain any sound
'                    If ParentSound.WaveData.SampleData(c).Length = 0 Then Continue For

'                    'Measuring UnWeightedLevel
'                    ChannelData(c).UnWeightedLevel = Nothing
'                    ChannelData(c).UnWeightedLevel = DSP.MeasureSectionLevel(ParentSound, c, 0, Nothing, SoundDataUnit.dB)
'                    AttemptedMeasurementCount += 1
'                    If ChannelData(c).UnWeightedLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

'                    'Meaures UnWeightedPeakLevel
'                    ChannelData(c).UnWeightedPeakLevel = Nothing
'                    ChannelData(c).UnWeightedPeakLevel = DSP.MeasureSectionLevel(ParentSound, c, 0, , SoundDataUnit.dB, SoundMeasurementType.AbsolutePeakAmplitude)
'                    AttemptedMeasurementCount += 1
'                    If ChannelData(c).UnWeightedPeakLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

'                    'Measures weighted level
'                    ChannelData(c).WeightedLevel = Nothing
'                    If ChannelData(c).GetTimeWeighting <> 0 Then
'                        ChannelData(c).WeightedLevel = DSP.GetLevelOfLoudestWindow(ParentSound, c,
'                                                                                     ChannelData(c).GetTimeWeighting * ParentSound.WaveFormat.SampleRate,
'                                                                                      0, Nothing, , ChannelData(c).GetFrequencyWeighting, True)
'                    Else
'                        ChannelData(c).WeightedLevel = DSP.MeasureSectionLevel(ParentSound, c, 0, Nothing, SoundDataUnit.dB, SoundMeasurementType.RMS, ChannelData(c).GetFrequencyWeighting)
'                    End If
'                    AttemptedMeasurementCount += 1
'                    If ChannelData(c).WeightedLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1


'                    'Measuring each sentence 
'                    For s = 0 To ChannelData(c).Count - 1

'                        'Measuring on sentence level

'                        'Measuring UnWeightedLevel
'                        ChannelData(c)(s).UnWeightedLevel = Nothing
'                        ChannelData(c)(s).UnWeightedLevel = DSP.MeasureSectionLevel(ParentSound, c, ChannelData(c)(s).StartSample, ChannelData(c)(s).Length, SoundDataUnit.dB)
'                        AttemptedMeasurementCount += 1
'                        If ChannelData(c)(s).UnWeightedLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

'                        'Meaures UnWeightedPeakLevel
'                        ChannelData(c)(s).UnWeightedPeakLevel = Nothing
'                        ChannelData(c)(s).UnWeightedPeakLevel = DSP.MeasureSectionLevel(ParentSound, c, ChannelData(c)(s).StartSample, ChannelData(c)(s).Length, SoundDataUnit.dB, SoundMeasurementType.AbsolutePeakAmplitude)
'                        AttemptedMeasurementCount += 1
'                        If ChannelData(c)(s).UnWeightedPeakLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

'                        'Measures weighted level
'                        ChannelData(c)(s).WeightedLevel = Nothing
'                        If ChannelData(c)(s).GetTimeWeighting <> 0 Then
'                            ChannelData(c)(s).WeightedLevel = DSP.GetLevelOfLoudestWindow(ParentSound, c,
'                                                                                     ChannelData(c)(s).GetTimeWeighting * ParentSound.WaveFormat.SampleRate,
'                                                                                      ChannelData(c)(s).StartSample, ChannelData(c)(s).Length, , ChannelData(c)(s).GetFrequencyWeighting, True)
'                        Else
'                            ChannelData(c)(s).WeightedLevel = DSP.MeasureSectionLevel(ParentSound, c, ChannelData(c)(s).StartSample, ChannelData(c)(s).Length, SoundDataUnit.dB, SoundMeasurementType.RMS, ChannelData(c)(s).GetFrequencyWeighting)
'                        End If
'                        AttemptedMeasurementCount += 1
'                        If ChannelData(c)(s).WeightedLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1


'                        'Measuring each word
'                        For w = 0 To ChannelData(c)(s).Count - 1

'                            'Measuring on word level

'                            'Measuring UnWeightedLevel
'                            ChannelData(c)(s)(w).UnWeightedLevel = Nothing
'                            ChannelData(c)(s)(w).UnWeightedLevel = DSP.MeasureSectionLevel(ParentSound, c, ChannelData(c)(s)(w).StartSample, ChannelData(c)(s)(w).Length, SoundDataUnit.dB)
'                            AttemptedMeasurementCount += 1
'                            If ChannelData(c)(s)(w).UnWeightedLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

'                            'Meaures UnWeightedPeakLevel
'                            ChannelData(c)(s)(w).UnWeightedPeakLevel = Nothing
'                            ChannelData(c)(s)(w).UnWeightedPeakLevel = DSP.MeasureSectionLevel(ParentSound, c, ChannelData(c)(s)(w).StartSample, ChannelData(c)(s)(w).Length, SoundDataUnit.dB, SoundMeasurementType.AbsolutePeakAmplitude)
'                            AttemptedMeasurementCount += 1
'                            If ChannelData(c)(s)(w).UnWeightedPeakLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

'                            'Measures weighted level
'                            ChannelData(c)(s)(w).WeightedLevel = Nothing
'                            If ChannelData(c)(s)(w).GetTimeWeighting <> 0 Then
'                                ChannelData(c)(s)(w).WeightedLevel = DSP.GetLevelOfLoudestWindow(ParentSound, c,
'                                                                                     ChannelData(c)(s)(w).GetTimeWeighting * ParentSound.WaveFormat.SampleRate,
'                                                                                      ChannelData(c)(s)(w).StartSample, ChannelData(c)(s)(w).Length, , ChannelData(c)(s)(w).GetFrequencyWeighting, True)
'                            Else
'                                ChannelData(c)(s)(w).WeightedLevel = DSP.MeasureSectionLevel(ParentSound, c, ChannelData(c)(s)(w).StartSample, ChannelData(c)(s)(w).Length, SoundDataUnit.dB, SoundMeasurementType.RMS, ChannelData(c)(s)(w).GetFrequencyWeighting)
'                            End If
'                            AttemptedMeasurementCount += 1
'                            If ChannelData(c)(s)(w).WeightedLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1


'                            'Measuring each phone
'                            For p = 0 To ChannelData(c)(s)(w).PhoneData.Count - 1

'                                'Measuring on phone level
'                                If ChannelData(c)(s)(w).PhoneData(p).PhoneticForm = WordEndString Then
'                                    ChannelData(c)(s)(w).PhoneData(p).UnWeightedLevel = Nothing
'                                    ChannelData(c)(s)(w).PhoneData(p).UnWeightedPeakLevel = Nothing
'                                    ChannelData(c)(s)(w).PhoneData(p).WeightedLevel = Nothing
'                                    Continue For
'                                End If

'                                'Measuring UnWeightedLevel
'                                ChannelData(c)(s)(w).PhoneData(p).UnWeightedLevel = Nothing
'                                ChannelData(c)(s)(w).PhoneData(p).UnWeightedLevel = DSP.MeasureSectionLevel(ParentSound, c, ChannelData(c)(s)(w).PhoneData(p).StartSample, ChannelData(c)(s)(w).PhoneData(p).Length, SoundDataUnit.dB)
'                                AttemptedMeasurementCount += 1
'                                If ChannelData(c)(s)(w).PhoneData(p).UnWeightedLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

'                                'Meaures UnWeightedPeakLevel
'                                ChannelData(c)(s)(w).PhoneData(p).UnWeightedPeakLevel = Nothing
'                                ChannelData(c)(s)(w).PhoneData(p).UnWeightedPeakLevel = DSP.MeasureSectionLevel(ParentSound, c, ChannelData(c)(s)(w).PhoneData(p).StartSample, ChannelData(c)(s)(w).PhoneData(p).Length, SoundDataUnit.dB, SoundMeasurementType.AbsolutePeakAmplitude)
'                                AttemptedMeasurementCount += 1
'                                If ChannelData(c)(s)(w).PhoneData(p).UnWeightedPeakLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

'                                'Measures weighted level
'                                ChannelData(c)(s)(w).PhoneData(p).WeightedLevel = Nothing
'                                If ChannelData(c)(s)(w).PhoneData(p).TimeWeighting <> 0 Then
'                                    ChannelData(c)(s)(w).PhoneData(p).WeightedLevel = DSP.GetLevelOfLoudestWindow(ParentSound, c,
'                                                                                     ChannelData(c)(s)(w).PhoneData(p).TimeWeighting * ParentSound.WaveFormat.SampleRate,
'                                                                                      ChannelData(c)(s)(w).PhoneData(p).StartSample, ChannelData(c)(s)(w).PhoneData(p).Length, , ChannelData(c)(s)(w).PhoneData(p).FrequencyWeighting, True)
'                                Else
'                                    ChannelData(c)(s)(w).PhoneData(p).WeightedLevel = DSP.MeasureSectionLevel(ParentSound, c, ChannelData(c)(s)(w).PhoneData(p).StartSample, ChannelData(c)(s)(w).PhoneData(p).Length, SoundDataUnit.dB, SoundMeasurementType.RMS, ChannelData(c)(s)(w).PhoneData(p).FrequencyWeighting)
'                                End If
'                                AttemptedMeasurementCount += 1
'                                If ChannelData(c)(s)(w).PhoneData(p).WeightedLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

'                            Next
'                        Next
'                    Next
'                Next


'                'Logging results
'                If LogMeasurementResults = True Then
'                    SendInfoToAudioLog(vbCrLf &
'                                       "FileName" & vbTab & ParentSound.FileName & vbTab &
'                                       "FailedMeasurementCount: " & vbTab & AttemptedMeasurementCount - SuccesfullMeasurementsCount & vbTab &
'                                       ToString(True), "SoundMeasurementLog.txt", LogFolder)
'                End If

'                'Checking if all attempted measurements were succesful
'                If AttemptedMeasurementCount = SuccesfullMeasurementsCount Then
'                    Return True
'                Else
'                    Return False
'                End If

'            End Function


'            ''' <summary>
'            ''' Resets the sound levels for each channel, sentence, word and phone of the current SMA object.
'            ''' </summary>
'            Public Sub ResetSoundLevels()

'                'Resetting each channel 
'                For c As Integer = 1 To ChannelCount

'                    ChannelData(c).UnWeightedLevel = Nothing
'                    ChannelData(c).UnWeightedPeakLevel = Nothing
'                    ChannelData(c).WeightedLevel = Nothing

'                    'Resetting each sentence 
'                    For s = 0 To ChannelData(c).Count - 1

'                        ChannelData(c)(s).UnWeightedLevel = Nothing
'                        ChannelData(c)(s).UnWeightedPeakLevel = Nothing
'                        ChannelData(c)(s).WeightedLevel = Nothing

'                        'Resetting each word
'                        For w = 0 To ChannelData(c)(s).Count - 1

'                            ChannelData(c)(s)(w).UnWeightedLevel = Nothing
'                            ChannelData(c)(s)(w).UnWeightedPeakLevel = Nothing
'                            ChannelData(c)(s)(w).WeightedLevel = Nothing

'                            'Resetting each phone
'                            For p = 0 To ChannelData(c)(s)(w).PhoneData.Count - 1

'                                ChannelData(c)(s)(w).PhoneData(p).UnWeightedLevel = Nothing
'                                ChannelData(c)(s)(w).PhoneData(p).UnWeightedPeakLevel = Nothing
'                                ChannelData(c)(s)(w).PhoneData(p).WeightedLevel = Nothing
'                            Next
'                        Next
'                    Next
'                Next

'            End Sub

'#Region "ExtensionsMethods"


'            ''' <summary>
'            ''' Measures the unalterred (original recording) absolute peak amplitude (linear value) within the word parts of a segmented audio recording. 
'            ''' At least WordStartSample and WordLength of all words must be set prior to calling this function.
'            ''' </summary>
'            ''' <param name="MeasurementSound">The sound to measure.</param>
'            ''' <returns></returns>
'            Public Function SetInitialPeakAmplitudes(ByVal MeasurementSound As Sound) As Boolean

'                Dim failedMeasurements As Integer = 0

'                For channel As Integer = 1 To Me.ChannelCount

'                    For sentence As Integer = 0 To Me.ChannelData(channel).Count - 1

'                        If Me.ChannelData(channel)(sentence).InitialPeak <> -1 Then
'                            'Aborting measurements if the current channel InitialPeak is already set (-1 is the default/unmeasured value)
'                            Continue For
'                        End If

'                        'Measuring sentence sound level
'                        For word = 0 To Me.ChannelData(channel)(sentence).Count - 1

'                            'Getting word start sample and length
'                            Dim WordStartSample As Integer = Me.ChannelData(channel)(sentence)(word).StartSample
'                            Dim Wordlength As Integer = Me.ChannelData(channel)(sentence)(word).Length

'                            'Checking that the word start sample is set (default value is -1)
'                            If WordStartSample < 0 Then
'                                failedMeasurements += 1
'                                Continue For
'                            End If

'                            'Checking that the word length is not 0 (or negative) or that it does not go outside the recorded audio array
'                            Dim soundLength As Integer = MeasurementSound.WaveData.ShortestChannelSampleCount
'                            If Wordlength < 1 Then
'                                failedMeasurements += 1
'                                Continue For
'                            End If
'                            If WordStartSample + Wordlength > soundLength Then
'                                failedMeasurements += 1
'                                Continue For
'                            End If

'                            'Measures UnWeightedPeakLevel of each word using Z-weighting
'                            Dim UnWeightedPeakLevel As Double?
'                            UnWeightedPeakLevel = DSP.MeasureSectionLevel(MeasurementSound, channel, WordStartSample, Wordlength, SoundDataUnit.linear, SoundMeasurementType.AbsolutePeakAmplitude)
'                            If UnWeightedPeakLevel Is Nothing Then
'                                failedMeasurements += 1
'                                Continue For
'                            End If

'                            'Setting InitialPeak to the highest detected so far
'                            Me.ChannelData(channel)(sentence).InitialPeak = Math.Max(Me.ChannelData(channel)(sentence).InitialPeak, CDbl(UnWeightedPeakLevel))
'                        Next
'                    Next
'                Next

'                If failedMeasurements > 0 Then
'                    Return False
'                Else
'                    Return True
'                End If

'            End Function


'            ''' <summary>
'            ''' This sub adds a word end string to the phoneme lists of all words in the current ptwf object. However, if a word end marker does already exists, it is not added.
'            ''' </summary>
'            Public Sub AddWordEndString()
'                'Adding word-end marker
'                For channel As Integer = 1 To Me.ChannelCount
'                    For sentence As Integer = 0 To Me.ChannelData(channel).Count - 1

'                        For word = 0 To Me.ChannelData(channel)(sentence).Count - 1

'                            'Checks if there is already a word end marker
'                            If Me.ChannelData(channel)(sentence)(word).PhoneData(Me.ChannelData(channel)(sentence)(word).PhoneData.Count - 1).PhoneticForm = WordEndString Then
'                                'There is already a word end marker stored (in a previous segmentation). Skips to the next word
'                                Continue For
'                            End If

'                            Me.ChannelData(channel)(sentence)(word).PhoneData.Add(New Sound.SpeechMaterialAnnotation.SmaPhoneData(Me))
'                            Me.ChannelData(channel)(sentence)(word).PhoneData(Me.ChannelData(channel)(sentence)(word).PhoneData.Count - 1).PhoneticForm = WordEndString
'                            'Positions the word end marker, according to the information stored in the previous phoneme, if there is any
'                            If Me.ChannelData(channel)(sentence)(word).PhoneData.Count > 1 Then
'                                Me.ChannelData(channel)(sentence)(word).PhoneData(Me.ChannelData(channel)(sentence)(word).PhoneData.Count - 1).StartSample =
'                                    Me.ChannelData(channel)(sentence)(word).PhoneData(Me.ChannelData(channel)(sentence)(word).PhoneData.Count - 2).StartSample +
'                                    Me.ChannelData(channel)(sentence)(word).PhoneData(Me.ChannelData(channel)(sentence)(word).PhoneData.Count - 2).Length
'                            End If
'                        Next
'                    Next
'                Next

'            End Sub

'            ''' <summary>
'            ''' This sub removes any word end string present in the phoneme lists of any word in the current ptwf object.
'            ''' </summary>
'            Public Sub RemoveWordEndString()
'                'Removing word-end marker
'                For channel As Integer = 1 To Me.ChannelCount
'                    For sentence As Integer = 0 To Me.ChannelData(channel).Count - 1
'                        For word = 0 To Me.ChannelData(channel)(sentence).Count - 1
'                            Dim tempPhonemeLevelDataList As List(Of Sound.SpeechMaterialAnnotation.SmaPhoneData) = Me.ChannelData(channel)(sentence)(word).PhoneData
'                            For phoneme = 0 To tempPhonemeLevelDataList.Count - 1
'                                Dim tempPhonemeLevelData As Sound.SpeechMaterialAnnotation.SmaPhoneData = Me.ChannelData(channel)(sentence)(word).PhoneData(phoneme)
'                                If tempPhonemeLevelData.PhoneticForm = WordEndString Then
'                                    tempPhonemeLevelDataList.RemoveAt(phoneme)
'                                End If
'                            Next
'                        Next
'                    Next
'                Next

'            End Sub


'            ''' <summary>
'            ''' Detects the boundaries (speech start, and speech end) of recorded speech (with no pauses), using the transcriptions stored in the current ptwf object.
'            ''' This method may be used to fascilitate manual segmentation, as it suggests appropriate word/sentence boundary positions.
'            '''The method works by 
'            ''' a) detecting the speech location by assuming that the loudest window in the recording will be found inside the sentence/word.
'            ''' b) detecting word/sentence start by locating the centre of the last window of a silent section of at least LongestSilentSegment milliseconds.
'            ''' c) detecting word/sentence end by locating the centre of the first window of a silent section of at least LongestSilentSegment milliseconds.
'            ''' </summary>
'            ''' <param name="MeasurementSound">The sound that the measurents shold be made upon.</param>
'            ''' <param name="LongestSilentSegment">The longest silent section (in seconds) that is allowed within the speech recording.</param>
'            ''' <param name="SilenceDefinition">The definition of a silent window is set to SilenceDefinition dB lower that the loudest detected window in the recording.</param>
'            Public Sub DetectSpeechBoundaries(ByRef MeasurementSound As Sound,
'                                              Optional ByVal LongestSilentSegment As Double = 0.3,
'                                              Optional ByVal SilenceDefinition As Double = 40,
'                                              Optional ByVal TemporalIntegrationDuration As Double = 0.06,
'                                              Optional ByVal DetailedTemporalIntegrationDuration As Double = 0.006,
'                                              Optional ByVal DetailedSilenceCriteria As Double = 20,
'                                              Optional ByVal SetToZeroCrossings As Boolean = True) ',
'                'Optional ByVal PlaceIntermediatePhonemes As Boolean = False)

'                Try

'                    'Storing the original soundlevel format
'                    Dim OriginalSoundFrequencyWeighting As FrequencyWeightings = MeasurementSound.SMA.GetFrequencyWeighting
'                    Dim OriginalSoundTimeWeighting As Double = MeasurementSound.SMA.GetTimeWeighting

'                    'Creating a temporary sound level format
'                    MeasurementSound.SMA.SetFrequencyWeighting(FrequencyWeightings.Z, True)
'                    MeasurementSound.SMA.SetTimeWeighting(TemporalIntegrationDuration, True)

'                    'Frequency weighting the sound prior to measurement

'                    'Doing high-pass filterring to reduce vibration influences
'                    Dim CurrentFftFormat As Formats.FftFormat = New Formats.FftFormat(1024 * 4,,,, True)
'                    Dim PreFftFilteringKernelSize As Integer = 4000
'                    Dim kernel As Sound = GenerateSound.CreateSpecialTypeImpulseResponse(MeasurementSound.WaveFormat, CurrentFftFormat, PreFftFilteringKernelSize, ,
'                                                                                                     FilterType.LinearAttenuationBelowCF_dBPerOctave,
'                                                                                                     100,,,, 25,, True)
'                    Dim filterredSound As Sound = DSP.TransformationsExt.FIRFilter(MeasurementSound, kernel, CurrentFftFormat,,,,,, True)
'                    Dim FirFilterDelayInSamles As Integer = kernel.WaveData.ShortestChannelSampleCount / 2

'                    'Creating a temporary and extended copy of the filterred sound, with the delay created by the FIR-filtering removed
'                    Dim tempSound As Sound = New Sound(MeasurementSound.WaveFormat)
'                    tempSound.FFT = New FftData(MeasurementSound.WaveFormat, CurrentFftFormat)
'                    For c = 1 To MeasurementSound.WaveFormat.Channels

'                        Dim SoundArray(MeasurementSound.WaveData.SampleData(c).Length + CurrentFftFormat.AnalysisWindowSize - 1) As Single
'                        For s = 0 To filterredSound.WaveData.SampleData(c).Length - FirFilterDelayInSamles - 1
'                            SoundArray(s) = filterredSound.WaveData.SampleData(c)(s + FirFilterDelayInSamles)
'                        Next
'                        tempSound.WaveData.SampleData(c) = SoundArray

'                    Next
'                    tempSound.SMA = MeasurementSound.SMA

'                    'Code to save the original and filterred sound for inspection
'                    'AudioIOs.SaveToWaveFile(MeasurementSound,,,,,, "Original")
'                    'AudioIOs.SaveToWaveFile(filterredSound,,,,,, "Filterred")
'                    'AudioIOs.SaveToWaveFile(tempSound,,,,,, "FilterredAndAdjusted")

'                    If tempSound Is Nothing Then
'                        Throw New Exception("Something went wrong during IIR-filterring")
'                    End If

'                    For c = 1 To Me.ChannelCount

'                        For sentence As Integer = 0 To Me.ChannelData(c).Count - 1
'                            'Looking at channel c

'                            'Detecting the start position, and level, of the loudest 100/4 ms window
'                            Dim LoudestWindowLevel As Double
'                            Dim WholeWindowList As New List(Of Double)
'                            Dim WindowDistance As Integer
'                            Dim CurrentSilenceEndCandidateWindow As Integer
'                            Dim CurrentSilenceLength As Double

'                            'Measuring sentence sound level
'                            LoudestWindowLevel = DSP.MeasureTimeAndFrequencyWeightedSectionLevel_QuarterOverlap(
'                            tempSound, c, 0, tempSound.WaveData.ShortestChannelSampleCount, SoundDataUnit.dB,
'                            WholeWindowList,, WindowDistance)
'                            Dim IndexOfLoudestWindow As Integer = WholeWindowList.LastIndexOf(WholeWindowList.Max)

'                            'Setting the value of silence definition
'                            Dim SilenceLevel As Double = LoudestWindowLevel - SilenceDefinition
'                            Dim SilenceRMS As Double = dBConversion(SilenceLevel, dBConversionDirection.from_dB, tempSound.WaveFormat)

'                            'Looking for the speech-onset window
'                            Dim SpeechStartSample As Integer = 0
'                            CurrentSilenceEndCandidateWindow = 0 'Setting default to the first window
'                            CurrentSilenceLength = 0
'                            For InverseWindowIndex = 0 To IndexOfLoudestWindow - 1
'                                Dim CurrentWindow As Integer = IndexOfLoudestWindow - 1 - InverseWindowIndex

'                                If WholeWindowList(CurrentWindow) > SilenceRMS Then
'                                    'The window is not "silent"
'                                    'Resetting CurrentSilenceLength
'                                    CurrentSilenceLength = 0

'                                    'Resetting CurrentSilenceEndCandidateWindow to the first window
'                                    CurrentSilenceEndCandidateWindow = 0

'                                Else
'                                    'The window is "silent"
'                                    'Updating the CurrentSilenceEndCandidate
'                                    If CurrentWindow > CurrentSilenceEndCandidateWindow Then
'                                        CurrentSilenceEndCandidateWindow = CurrentWindow
'                                    End If

'                                    'Adding the current window length to the silent section
'                                    CurrentSilenceLength += WindowDistance / MeasurementSound.WaveFormat.SampleRate

'                                End If

'                                'Checking if the silent section found is equal to or longer than LongestSilentSegment
'                                If CurrentSilenceLength >= LongestSilentSegment Then

'                                    'The word/sentence start has been found
'                                    SpeechStartSample = WindowDistance * (CurrentSilenceEndCandidateWindow + 1)
'                                    Exit For
'                                End If
'                            Next

'                            'Copying the window that follows after the first detected silant windo to a new sound, which is analysed to detect a more exact boundary
'                            Dim TempSound1 As Sound = New Sound(New Formats.WaveFormat(tempSound.WaveFormat.SampleRate, tempSound.WaveFormat.BitDepth, 1))
'                            Dim TempSoundArray(WindowDistance * 4 - 1) As Single
'                            For sample = 0 To TempSoundArray.Length - 1
'                                TempSoundArray(sample) = tempSound.WaveData.SampleData(c)(sample + SpeechStartSample)
'                            Next
'                            TempSound1.WaveData.SampleData(1) = TempSoundArray
'                            TempSound1.SMA = New Sound.SpeechMaterialAnnotation(FrequencyWeightings.Z, DetailedTemporalIntegrationDuration)

'                            'Adding one one channel and one sentence
'                            TempSound1.SMA.AddChannelData(New Sound.SpeechMaterialAnnotation.SmaChannelData(TempSound1.SMA))
'                            TempSound1.SMA.ChannelData(1).Add(New Sound.SpeechMaterialAnnotation.SmaSentenceData(TempSound1.SMA))

'                            'Looking inside the TempSound1 window to determine a more exact boundary 
'                            Dim InnerWindowList As New List(Of Double)
'                            Dim InnerWindowDistance As Integer
'                            LoudestWindowLevel = DSP.MeasureTimeAndFrequencyWeightedSectionLevel_QuarterOverlap(
'                            TempSound1, 1, 0,, SoundDataUnit.dB,,
'                            InnerWindowList, InnerWindowDistance)
'                            'Detecting the first window that is at least DetailedSilenceCriteria softer than the loudest window
'                            Dim InnerStartSample As Integer = 0
'                            Dim InnerListMaxLevel As Double = dBConversion(InnerWindowList.Max, dBConversionDirection.to_dB, TempSound1.WaveFormat)
'                            Dim InnelListLevelLimit As Double = InnerListMaxLevel - DetailedSilenceCriteria
'                            Dim InnerListRMSLimit As Double = dBConversion(InnelListLevelLimit, dBConversionDirection.from_dB, TempSound1.WaveFormat)

'                            For w = 0 To InnerWindowList.Count - 1
'                                If InnerWindowList(w) >= InnerListRMSLimit And InnerWindowList(w) > 0 Then 'The last argument ignores windows made up of zero-padding
'                                    InnerStartSample = w * InnerWindowDistance
'                                    Exit For
'                                End If
'                            Next

'                            'Adding the detailed samples from the last analysis
'                            SpeechStartSample += InnerStartSample


'                            'Looking for the speech-end window
'                            Dim SpeechEndSample As Integer = tempSound.WaveData.ShortestChannelSampleCount - 1

'                            'Restting CurrentSilenceEndCandidateWindow and CurrentSilenceLength 
'                            CurrentSilenceEndCandidateWindow = WholeWindowList.Count - 1 'Since the three last windows Setting default to the last window
'                            CurrentSilenceLength = 0

'                            For CurrentWindow = IndexOfLoudestWindow + 1 To WholeWindowList.Count - 1

'                                If WholeWindowList(CurrentWindow) > SilenceRMS Then
'                                    'The window is not "silent"
'                                    'Resetting CurrentSilenceLength
'                                    CurrentSilenceLength = 0

'                                    'Resetting CurrentSilenceEndCandidateWindow to the last window
'                                    CurrentSilenceEndCandidateWindow = WholeWindowList.Count - 1

'                                Else
'                                    'The window is "silent"
'                                    'Updating the CurrentSilenceEndCandidate
'                                    If CurrentWindow < CurrentSilenceEndCandidateWindow Then
'                                        CurrentSilenceEndCandidateWindow = CurrentWindow
'                                    End If

'                                    'Adding the current window distance to the silent section
'                                    CurrentSilenceLength += WindowDistance / tempSound.WaveFormat.SampleRate

'                                End If

'                                'Checking if the silent section found is equal to or longer than LongestSilentSegment
'                                If CurrentSilenceLength >= LongestSilentSegment Then

'                                    'The word/sentence end has been found
'                                    SpeechEndSample = WindowDistance * (CurrentSilenceEndCandidateWindow - 1)

'                                    Exit For
'                                End If
'                            Next

'                            'Copying the window prior to the detected silent window to a new sound, and analyses it to detect a more exact boundary
'                            Dim TempSound2 As Sound = New Sound(New Formats.WaveFormat(tempSound.WaveFormat.SampleRate, tempSound.WaveFormat.BitDepth, 1))
'                            Dim TempSoundArray2(WindowDistance * 4 - 1) As Single

'                            'Correcting SpeechEndSample to be at least one window lower than the sound array
'                            If SpeechEndSample > tempSound.WaveData.SampleData(c).Length - TempSoundArray2.Length - 1 Then
'                                SpeechEndSample = tempSound.WaveData.SampleData(c).Length - TempSoundArray2.Length - 1
'                            End If

'                            For sample = 0 To TempSoundArray2.Length - 1
'                                TempSoundArray2(sample) = tempSound.WaveData.SampleData(c)(sample + SpeechEndSample)
'                            Next
'                            TempSound2.WaveData.SampleData(1) = TempSoundArray
'                            TempSound2.SMA = New Sound.SpeechMaterialAnnotation(FrequencyWeightings.Z, DetailedTemporalIntegrationDuration)

'                            'Adding one one channel and one sentence
'                            TempSound2.SMA.AddChannelData(New Sound.SpeechMaterialAnnotation.SmaChannelData(TempSound2.SMA))
'                            TempSound2.SMA.ChannelData(1).Add(New Sound.SpeechMaterialAnnotation.SmaSentenceData(TempSound2.SMA))

'                            'Looking inside the TempSound1 window to determine a more exakt boundary 
'                            InnerWindowList = New List(Of Double)
'                            LoudestWindowLevel = DSP.MeasureTimeAndFrequencyWeightedSectionLevel_QuarterOverlap(
'                            TempSound2, 1, 0, , SoundDataUnit.dB,,
'                            InnerWindowList, InnerWindowDistance)
'                            'Detecting the last window that is at least half as loud as the loudest window (6 dB)
'                            Dim InnerEndSample As Integer = 0
'                            InnerStartSample = 0
'                            InnerListMaxLevel = dBConversion(InnerWindowList.Max, dBConversionDirection.to_dB, TempSound1.WaveFormat)
'                            InnelListLevelLimit = InnerListMaxLevel - DetailedSilenceCriteria
'                            InnerListRMSLimit = dBConversion(InnelListLevelLimit, dBConversionDirection.from_dB, TempSound1.WaveFormat)

'                            For w_inverse = 0 To InnerWindowList.Count - 1
'                                Dim w As Integer = InnerWindowList.Count - 1 - w_inverse
'                                If InnerWindowList(w) >= InnerListRMSLimit And InnerWindowList(w) > 0 Then 'The last argument ignores windows made up of zero-padding
'                                    InnerEndSample = w * InnerWindowDistance
'                                    Exit For
'                                End If
'                            Next


'                            'Adding the detailed samples from the last analysis
'                            SpeechEndSample += InnerEndSample

'                            If SetToZeroCrossings = True Then
'                                SpeechStartSample = DSP.GetZeroCrossingSample(MeasurementSound, c, SpeechStartSample, DSP.MeasurementsExt.SearchDirections.Closest)
'                                SpeechEndSample = DSP.GetZeroCrossingSample(MeasurementSound, c, SpeechEndSample, DSP.MeasurementsExt.SearchDirections.Closest)
'                            End If

'                            'Storing the data start and end samples in the ptwf object
'                            'Storing speech start and end, on both sentence, word (only using the first word, which has index 0) and phoneme level

'                            'Sentence level
'                            Me.ChannelData(c)(sentence).StartSample = SpeechStartSample
'                            Me.ChannelData(c)(sentence).Length = SpeechEndSample - SpeechStartSample - 1

'                            'Word level (first word only (index 0))
'                            Me.ChannelData(c)(sentence)(0).StartSample = SpeechStartSample
'                            Me.ChannelData(c)(sentence)(0).Length = SpeechEndSample - SpeechStartSample - 1

'                            'Phoneme level
'                            If Me.ChannelData(c)(sentence)(0).PhoneData IsNot Nothing Then
'                                'Storing the position of the first phoneme
'                                If Me.ChannelData(c)(sentence)(0).PhoneData.Count > 0 Then
'                                    Me.ChannelData(c)(sentence)(0).PhoneData(0).StartSample = SpeechStartSample
'                                End If
'                                'Storing the position of the last item in the phoneme array (which should be a word-end string)
'                                If Me.ChannelData(c)(sentence)(0).PhoneData.Count > 1 Then
'                                    Me.ChannelData(c)(sentence)(Me.ChannelData(c)(sentence).Count - 1).PhoneData(Me.ChannelData(c)(sentence)(Me.ChannelData(c)(sentence).Count - 1).PhoneData.Count - 1).StartSample = SpeechEndSample
'                                End If
'                            End If

'                        Next
'                    Next

'                    'Restoring the origial sound level format
'                    MeasurementSound.SMA.SetFrequencyWeighting(OriginalSoundFrequencyWeighting, True)
'                    MeasurementSound.SMA.SetTimeWeighting(OriginalSoundTimeWeighting, True)

'                Catch ex As Exception
'                    MsgBox(ex.ToString)
'                End Try

'            End Sub


'            ''' <summary>
'            ''' This sub resets segmentation time data (startSample and Length) stored in the current ptwf object.
'            ''' </summary>
'            Public Sub ResetTemporalData()

'                For channel As Integer = 1 To Me.ChannelCount

'                    For sentence As Integer = 0 To Me.ChannelData(channel).Count - 1

'                        Me.ChannelData(channel)(sentence).StartSample = -1
'                        Me.ChannelData(channel)(sentence).Length = 0

'                        For word = 0 To Me.ChannelData(channel)(sentence).Count - 1
'                            Me.ChannelData(channel)(sentence)(word).StartSample = -1
'                            Me.ChannelData(channel)(sentence)(word).Length = 0

'                            'Dim tempPhonemeLevelDataList As List(Of Sound.ptwfData.SmaPhoneData) = ChannelData(channel)(sentence)(word).PhoneData
'                            For phoneme = 0 To Me.ChannelData(channel)(sentence)(word).PhoneData.Count - 1 'tempPhonemeLevelDataList.Count - 1
'                                'Dim tempPhonemeLevelData As Sound.ptwfData.SmaPhoneData = ChannelData(channel)(sentence)(word).PhoneData(phoneme)
'                                Me.ChannelData(channel)(sentence)(word).PhoneData(phoneme).StartSample = -1
'                                Me.ChannelData(channel)(sentence)(word).PhoneData(phoneme).Length = 0
'                            Next
'                        Next
'                    Next
'                Next

'            End Sub

'            ''' <summary>
'            ''' Moves segmented phoneme boundaries to the nearset zero crossings. If DoFinalizeSegmentation = True, also phoneme lengths and sound levels are calculated, and fade padding applied the segmented sound.
'            ''' </summary>
'            ''' <param name="Sound"></param>
'            ''' <param name="CurrentChannel"></param>
'            ''' <param name="SearchDirection"></param>
'            ''' <param name="DoFinalizeSegmentation"></param>
'            ''' <param name="PaddingTime"></param>
'            Public Sub MoveSegmentationsToClosestZeroCrossings(ByRef Sound As Sound, ByVal CurrentChannel As Integer, Optional ByVal SearchDirection As DSP.SearchDirections = DSP.SearchDirections.Closest,
'                                                               Optional DoFinalizeSegmentation As Boolean = True, Optional ByRef PaddingTime As Single = 0.5)
'                Try

'                    'TODO: This sub currently can only use mono sounds!

'                    'For c = 1 To Sound.WaveFormat.Channels

'                    For sentence As Integer = 0 To Me.ChannelData(CurrentChannel).Count - 1

'                        'Adjusting sentenceStartTime
'                        Me.ChannelData(CurrentChannel)(sentence).StartSample = DSP.GetZeroCrossingSample(Sound, CurrentChannel, Me.ChannelData(CurrentChannel)(sentence).StartSample, SearchDirection)

'                        'TODO: Perhaps ChannelData(CurrentChannel)(sentence).StartTime should also be updated? 
'                        'For now, StartTime is derived from ChannelData(CurrentChannel)(sentence).StartSample (But, wasn't StartTime supposed to be in reference to a e.g. camera?)
'                        Me.ChannelData(CurrentChannel)(sentence).StartTime = Me.ChannelData(CurrentChannel)(sentence).StartSample / Sound.WaveFormat.SampleRate

'                        For word = 0 To Me.ChannelData(CurrentChannel)(sentence).Count - 1

'                            'Adjusting wordStartTime and start sample
'                            Me.ChannelData(CurrentChannel)(sentence)(word).StartTime = DSP.GetZeroCrossingSample(Sound, CurrentChannel, Me.ChannelData(CurrentChannel)(sentence)(word).StartTime, SearchDirection)
'                            Me.ChannelData(CurrentChannel)(sentence)(word).StartSample = DSP.GetZeroCrossingSample(Sound, CurrentChannel, Me.ChannelData(CurrentChannel)(sentence)(word).StartSample, SearchDirection)

'                            For phoneme = 0 To Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData.Count - 1
'                                'Adjusting phoneme start sample
'                                Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData(phoneme).StartSample = DSP.GetZeroCrossingSample(Sound, CurrentChannel, Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData(phoneme).StartSample, SearchDirection)
'                            Next phoneme
'                        Next word

'                        Me.UpdateSegmentation(Sound, PaddingTime, CurrentChannel)

'                        'Next

'                    Next
'                Catch ex As Exception
'                    MsgBox(ex.ToString)
'                End Try

'            End Sub

'            ''' <summary>
'            ''' Updates the segmentation, data of the current sound file. The following steps are taken. N.B. Presently only multiple sentences are supported!
'            ''' a) 
'            ''' </summary>
'            ''' <param name="Sound">The sound that belong to the current SMA object.</param>
'            ''' <param name="PaddingTime">The time between sound start and speech start, as well as between speech end and sound end.</param>
'            ''' <param name="CurrentChannel"></param>
'            ''' <param name="FadeStartAndEnd">If set to True, the Padding section before and after the recorded material is faded in/out.</param>
'            Public Sub UpdateSegmentation(ByRef Sound As Sound, ByRef PaddingTime As Single, ByRef CurrentChannel As Integer,
'                                          Optional FadeStartAndEnd As Boolean = False,
'                                          Optional FadeType As DSP.FadeSlopeType = DSP.FadeSlopeType.PowerCosine_SkewedFromFadeDirection,
'                                          Optional CosinePower As Double = 100)
'                Try

'                    Throw New NotImplementedException("The UpdateSegmentation method needs to be changed as the word end string has ben removed from the SMA segmentation. Instead of containing the word end string within the SMA object, an end string should be added by the current method (or in another appropriate place) on all segmentation levels (channel, sentence, word and phone, and then removed when not needed again.")

'                    'Throws exception if ChannelSpecific is set to true. This should be removed when channel specific segmentation is implemented in the future
'                    If CurrentChannel <> 1 Then Throw New NotImplementedException("At present, only mono sounds are supported by FinalizeSegmentation")

'                    Dim sentence As Integer = 0

'                    'Determining the sentence start sample and length
'                    Dim SentenceStartSample As Integer = Me.ChannelData(1)(sentence)(0).PhoneData(0).StartSample
'                    Dim SentenceEndSample As Integer = Me.ChannelData(1)(sentence)(Me.ChannelData(1)(sentence).Count - 1).PhoneData(Me.ChannelData(1)(sentence)(Me.ChannelData(1)(sentence).Count - 1).PhoneData.Count - 1).StartSample
'                    Dim SentenceLength As Integer = SentenceEndSample - SentenceStartSample

'                    'Calculating the length of the padding section in samples
'                    Dim PaddingSamples As Integer = PaddingTime * Sound.WaveFormat.SampleRate

'                    'Storing the time of the first phoneme in the first word as sentence start time
'                    Me.ChannelData(1)(sentence).StartTime += SentenceStartSample / Sound.WaveFormat.SampleRate

'                    'Finding out how the the ends of the file should be adjusted
'                    Dim temporalAdjustmentOfSoundArray As Integer = 0
'                    Dim indendedChannelArrayLengthInSample As Integer = SentenceLength + (2 * PaddingSamples)

'                    'Adding word start and length data, and adjusting the lengths of each channel separately (according to channel 1 segmentation data)
'                    For c = 1 To Sound.WaveFormat.Channels

'                        'Adding sentence time data (all from channel 1, which should be changed when ChannelSpecific is implemented)
'                        Me.ChannelData(c)(sentence).StartSample = SentenceStartSample
'                        Me.ChannelData(c)(sentence).Length = SentenceLength
'                        Me.ChannelData(c)(sentence).StartTime = SentenceStartSample / Sound.WaveFormat.SampleRate

'                        'Adding word time data
'                        For word = 0 To Me.ChannelData(c)(sentence).Count - 1
'                            Dim SampleOfFirstPhoneme As Integer = Me.ChannelData(c)(sentence)(word).PhoneData(0).StartSample
'                            Dim WordEndSample As Integer = Me.ChannelData(c)(sentence)(word).PhoneData(Me.ChannelData(c)(sentence)(word).PhoneData.Count - 1).StartSample
'                            Dim LengthOfWord As Integer = WordEndSample - SampleOfFirstPhoneme
'                            Me.ChannelData(c)(sentence)(word).StartSample = Me.ChannelData(c)(sentence)(word).PhoneData(0).StartSample
'                            Me.ChannelData(c)(sentence)(word).Length = LengthOfWord
'                            Me.ChannelData(c)(sentence)(word).StartTime = SampleOfFirstPhoneme / Sound.WaveFormat.SampleRate
'                        Next

'                        'Modifying the sound array so that it is equal in length and is synchronized with the newChannelArray
'                        'taking care of the section prior to the first phoneme
'                        Select Case SentenceStartSample
'                            Case > PaddingSamples

'                                'cutting the section of the sound array before padding time
'                                Dim samplesToCut As Integer = SentenceStartSample - PaddingSamples
'                                Dim tempArray(Sound.WaveData.SampleData(c).Length - samplesToCut - 1) As Single
'                                For sample = 0 To tempArray.Length - 1
'                                    tempArray(sample) = Sound.WaveData.SampleData(c)(sample + samplesToCut)
'                                Next
'                                Sound.WaveData.SampleData(c) = tempArray
'                                If c = 1 Then 'TODO: Getting the sentence start and ends segmentation from channel 1 only. This should change when soundEditor is modified to support multi channel sounds
'                                    temporalAdjustmentOfSoundArray = -samplesToCut
'                                End If

'                            Case < PaddingSamples
'                                'extending the section of the sound array before padding time
'                                Dim samplesToAdd As Integer = PaddingSamples - SentenceStartSample
'                                Dim tempArray(Sound.WaveData.SampleData(c).Length + samplesToAdd - 1) As Single
'                                For sample = 0 To Sound.WaveData.SampleData(c).Length - 1
'                                    tempArray(sample + samplesToAdd) = Sound.WaveData.SampleData(c)(sample)
'                                Next
'                                Sound.WaveData.SampleData(c) = tempArray
'                                If c = 1 Then 'TODO: This should actually be the measurement channel, however soundEditor presently only supports mono sounds
'                                    temporalAdjustmentOfSoundArray = samplesToAdd
'                                End If

'                            Case Else
'                                'I.E. equal length, does nothing

'                        End Select

'                        'Extending the sound array to the length of indendedChannelArrayLengthInSample
'                        ReDim Preserve Sound.WaveData.SampleData(c)(indendedChannelArrayLengthInSample - 1)

'                        If FadeStartAndEnd = True Then

'                            'Fading in start
'                            DSP.Fade(Sound, , 0, c, 0, PaddingSamples, FadeType, CosinePower)

'                            'Fading out end
'                            DSP.Fade(Sound, 0, , c, Sound.WaveData.SampleData(c).Length - PaddingSamples - 1,,
'                                           FadeType, CosinePower)
'                        End If

'                    Next c


'                    'Adjusting times stored in the ptwf phoneme data
'                    For c = 1 To Sound.WaveFormat.Channels

'                        'Adjusting sentenceStartTime (this is the reference time, e.g the duration since a video camera was started), and start sample
'                        Me.ChannelData(CurrentChannel)(sentence).StartTime = ((Me.ChannelData(CurrentChannel)(sentence).StartTime * Sound.WaveFormat.SampleRate) + temporalAdjustmentOfSoundArray) / Sound.WaveFormat.SampleRate
'                        Me.ChannelData(CurrentChannel)(sentence).StartSample += temporalAdjustmentOfSoundArray

'                        For word = 0 To Me.ChannelData(c)(sentence).Count - 1

'                            'Adjusting wordStartTime and start sample
'                            Me.ChannelData(CurrentChannel)(sentence)(word).StartTime = ((Me.ChannelData(CurrentChannel)(sentence)(word).StartTime * Sound.WaveFormat.SampleRate) + temporalAdjustmentOfSoundArray) / Sound.WaveFormat.SampleRate

'                            Me.ChannelData(CurrentChannel)(sentence)(word).StartSample += temporalAdjustmentOfSoundArray

'                            For phoneme = 0 To Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData.Count - 1
'                                'Adjusting phoneme start sample (If phoneme position has been set, i.e. has a value other than -1)
'                                If Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData(phoneme).StartSample <> -1 Then
'                                    Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData(phoneme).StartSample += temporalAdjustmentOfSoundArray
'                                End If
'                            Next phoneme
'                        Next word
'                    Next c

'                    'Setting phoneme lengths
'                    For c = 1 To Sound.WaveFormat.Channels
'                        For word = 0 To Me.ChannelData(c)(sentence).Count - 1

'                            ''Setting the length of the last phoneme to 0 if it is the WordEndString
'                            'If Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData(Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData.Count - 1).PhoneticForm = WordEndString Then
'                            '    Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData(Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData.Count - 1).Length = 0
'                            'End If

'                            'Setting the length of the rest of the phonemes
'                            For phoneme = 0 To Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData.Count - 2
'                                'Setting the phoneme length to that of the distance between the current phoneme and the next (If both phoneme positions have been set, i.e. have values other than -1)
'                                If Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData(phoneme).StartSample <> -1 And Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData(phoneme + 1).StartSample <> -1 Then
'                                    Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData(phoneme).Length =
'                                        Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData(phoneme + 1).StartSample - Me.ChannelData(CurrentChannel)(sentence)(word).PhoneData(phoneme).StartSample
'                                End If
'                            Next phoneme
'                        Next word
'                    Next c


'                    'Measures InitialpeakAmplitudes
'                    If Not Me.SetInitialPeakAmplitudes(Sound) = True Then MsgBox("Failed setting initial peak amplitudes.")

'                    'Adjusting sound level
'                    If Me.GetTimeWeighting <> 0 = True Then

'                        If Not DSP.TimeAndFrequencyWeightedNormalization(Sound, CurrentChannel,,,) = True Then 'SoundLevelFormat.TemporalIntegrationDuration, SoundLevelFormat.Frequencyweighting) = True Then
'                            MsgBox("Distorsion occurred during time and frequency weighted normalization.")
'                        End If

'                        'DSP.MeasureAndSetGatedSectionLevel(sound, currentChannel,,,
'                        'SoundLevelFormat.OutputLevel,
'                        'SoundLevelFormat.GatingWindowDuration,
'                        'SoundLevelFormat.GateRelativeThreshold,
'                        'SoundLevelFormat.FractionForCalculatingAbsThreshold,
'                        'SoundLevelFormat.Frequencyweighting)

'                    Else
'                        DSP.MeasureAndAdjustSectionLevel(Sound, -23, CurrentChannel,,, Me.GetFrequencyWeighting)
'                    End If

'                    'Measuring sound levels
'                    If Not Me.MeasureSoundLevels(True) = True Then MsgBox("Failed measuring sound levels.")

'                Catch ex As Exception

'                End Try


'            End Sub

'            ''' <summary>
'            ''' Goes through all segmentation data and detects unset start values and zero lengths. The accumulated number of unset starts, and zero-lengths are returned in the parameters.
'            ''' N.B. Presently, only multiple sentences are supported!
'            ''' </summary>
'            Public Sub ValidateSegmentation(ByRef UnsetStarts As Integer, ByRef ZeroLengths As Integer,
'                                            ByRef SetStarts As Integer, ByRef SetLengths As Integer)

'                Dim sentence As Integer = 0

'                Dim NumberOfMissingPhonemeArrays As Integer = 0

'                'Adjusting times stored in the ptwf phoneme data
'                For c = 1 To Me.ChannelCount

'                    If Me.ChannelData(c)(sentence).StartSample = -1 Then
'                        UnsetStarts += 1
'                    Else
'                        SetStarts += 1
'                    End If
'                    If Me.ChannelData(c)(sentence).Length = 0 Then
'                        ZeroLengths += 1
'                    Else
'                        SetLengths += 1
'                    End If

'                    For word = 0 To Me.ChannelData(c)(sentence).Count - 1

'                        If Me.ChannelData(c)(sentence)(word).StartSample = -1 Then
'                            UnsetStarts += 1
'                        Else
'                            SetStarts += 1
'                        End If
'                        If Me.ChannelData(c)(sentence)(word).Length = 0 Then
'                            ZeroLengths += 1
'                        Else
'                            SetLengths += 1
'                        End If

'                        If Me.ChannelData(c)(sentence)(word).PhoneData IsNot Nothing Then
'                            For phoneme = 0 To Me.ChannelData(c)(sentence)(word).PhoneData.Count - 1
'                                If Me.ChannelData(c)(sentence)(word).PhoneData(phoneme).StartSample = -1 Then
'                                    UnsetStarts += 1
'                                Else
'                                    SetStarts += 1
'                                End If
'                                If Me.ChannelData(c)(sentence)(word).PhoneData(phoneme).Length = 0 Then
'                                    ZeroLengths += 1
'                                Else
'                                    SetLengths += 1
'                                End If
'                            Next phoneme
'                        Else
'                            'Not phoneme array exists
'                            NumberOfMissingPhonemeArrays += 1
'                        End If
'                    Next word
'                Next c

'            End Sub

'            ''' <summary>
'            ''' Shifts all StartSample indices in the current instance of SpeechMaterialAnnotation, and limits the StartSample and Length values to the sample indices available in the parent sounds file
'            ''' </summary>
'            ''' <param name="ShiftInSamples"></param>
'            Public Sub ShiftSegmentationData(ByVal ShiftInSamples As Integer)

'                For c = 1 To Me.ChannelCount
'                    Dim Channel = Me.ChannelData(c)
'                    Dim SoundChannelLength As Integer = ParentSound.WaveData.SampleData(c).Length
'                    LimitStartIndexAndLengthOnShift(ShiftInSamples, SoundChannelLength, Channel.StartSample, Channel.Length)
'                    For Each Sentence In Channel
'                        LimitStartIndexAndLengthOnShift(ShiftInSamples, SoundChannelLength, Sentence.StartSample, Sentence.Length)
'                        For Each Word In Sentence
'                            LimitStartIndexAndLengthOnShift(ShiftInSamples, SoundChannelLength, Word.StartSample, Word.Length)
'                            For Each Phone In Word.PhoneData
'                                LimitStartIndexAndLengthOnShift(ShiftInSamples, SoundChannelLength, Phone.StartSample, Phone.Length)
'                            Next
'                        Next
'                    Next
'                Next

'            End Sub

'            Private Sub LimitStartIndexAndLengthOnShift(ByVal Shift As Integer, ByVal TotalAvailableLength As Integer, ByRef StartIndex As Integer, ByRef Length As Integer)

'                MsgBox("Check the code below for accuracy!!!")

'                'Adjusting the StartSample and limiting it to the available range
'                StartIndex = Math.Min(Math.Max(StartIndex + Shift, 0), TotalAvailableLength - 1)

'                'Limiting Length to the available length after adjustment of startsample
'                Dim MaximumPossibleLength = TotalAvailableLength - StartIndex
'                Length = Math.Max(0, Math.Min(Length, MaximumPossibleLength))

'            End Sub

'            ''' <summary>
'            ''' Fading the padded sections before the word start and after the word end.
'            ''' N.B. Presently, only multiple sentences are supported!
'            ''' </summary>
'            ''' <param name="Sound"></param>
'            ''' <param name="CurrentChannel"></param>
'            ''' <param name="FadeType"></param>
'            ''' <param name="CosinePower"></param>
'            Public Sub FadePaddingSection(ByRef Sound As Sound, ByRef CurrentChannel As Integer,
'                                          Optional FadeType As DSP.FadeSlopeType = DSP.FadeSlopeType.PowerCosine_SkewedFromFadeDirection,
'                                          Optional CosinePower As Double = 100)

'                Dim sentence As Integer = 0

'                For c = 1 To Sound.WaveFormat.Channels

'                    Dim SentenceStartSample As Integer = Sound.SMA.ChannelData(CurrentChannel)(sentence).StartSample
'                    Dim SentenceEndSample As Integer = Sound.SMA.ChannelData(CurrentChannel)(sentence).StartSample + Sound.SMA.ChannelData(CurrentChannel)(sentence).Length


'                    'Fading in start
'                    DSP.Fade(Sound, , 0, c, 0, SentenceStartSample - 1, FadeType, CosinePower)

'                    'Fading out end
'                    DSP.Fade(Sound, 0, , c, SentenceEndSample + 1,,
'                                   FadeType, CosinePower)

'                Next


'            End Sub

'            Public Function CreateCopy() As SpeechMaterialAnnotation

'                'Creating an output object
'                Dim newSmaData As SpeechMaterialAnnotation

'                'Serializing to memorystream
'                Dim serializedMe As New MemoryStream
'                Dim serializer As New BinaryFormatter
'                serializer.Serialize(serializedMe, Me)

'                'Deserializing to new object
'                serializedMe.Position = 0
'                newSmaData = CType(serializer.Deserialize(serializedMe), SpeechMaterialAnnotation)
'                serializedMe.Close()

'                'Returning the new object
'                Return newSmaData
'            End Function



'#Region "MultiWordRecordings"

'            'N.B. These methods were written when the SMA only had support for one sentence per channel. They should all be re-written!

'            ''' <summary>
'            ''' Adjusts the sound level of each ptwf word in the input sound, so that each carrier phrase gets the indicated target level. The level of each test word is adjusted together with its carrier phrase.
'            ''' </summary>
'            ''' <param name="InputSound"></param>
'            Public Sub MultiWord_SoundLevelEqualization(ByRef InputSound As Sound, ByVal SoundChannel As Integer, Optional ByVal TargetLevel As Double = -30, Optional ByVal PaddingDuration As Double = 0.5)

'                Dim sentence As Integer = 0

'                Try

'                    Dim PtwfChannel As Integer = 1 'This should be a parameter if multi channel ptwfs are needed

'                    'Setting a padding interval
'                    Dim PaddingSamples As Double = PaddingDuration * InputSound.WaveFormat.SampleRate

'                    If InputSound IsNot Nothing Then

'                        For WordIndex = 0 To Me.ChannelData(PtwfChannel)(sentence).Count - 1

'                            'Measures the carries phrases
'                            Dim CarrierLevel As Double = DSP.MeasureSectionLevel(InputSound, SoundChannel,
'                                                                                   Me.ChannelData(PtwfChannel)(sentence)(WordIndex).PhoneData(0).StartSample,
'                                                                                   Me.ChannelData(PtwfChannel)(sentence)(WordIndex).PhoneData(0).Length)

'                            'Calculating gain
'                            Dim Gain As Double = TargetLevel - CarrierLevel

'                            'Applying gain to the whole word
'                            DSP.AmplifySection(InputSound, Gain, SoundChannel, Me.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample - PaddingSamples, Me.ChannelData(PtwfChannel)(sentence)(WordIndex).Length + 2 * PaddingSamples)

'                        Next

'                    End If
'                Catch ex As Exception
'                    MsgBox("An error occurred: " & ex.ToString)
'                End Try

'            End Sub

'            Public Sub MultiWord_SetInterStimulusInterval(ByRef InputSound As Sound, Optional ByVal Interval As Double = 3)

'                Dim sentence As Integer = 0

'                Try

'                    Dim PtwfChannel As Integer = 1 'This should be a parameter if multi channel ptwfs are needed

'                    Dim StimulusIntervalInSamples As Double = Interval * InputSound.WaveFormat.SampleRate

'                    If InputSound IsNot Nothing Then

'                        For WordIndex = 0 To Me.ChannelData(PtwfChannel)(sentence).Count - 2

'                            'Getting the interval to the next word
'                            Dim CurrentInterval As Integer =
'                            Me.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample -
'                            (Me.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample + Me.ChannelData(PtwfChannel)(sentence)(WordIndex).Length)

'                            'Calculating adjustment length
'                            Dim AdjustmentLength As Integer = StimulusIntervalInSamples - CurrentInterval

'                            Select Case AdjustmentLength
'                                Case < 0
'                                    'Deleting a segment right between the words
'                                    Dim StartDeleteSample As Integer = Me.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample - (CurrentInterval / 2) - (Math.Abs(AdjustmentLength / 2))
'                                    DSP.DeleteSection(InputSound, StartDeleteSample, Math.Abs(AdjustmentLength))

'                                Case > 0
'                                    'Inserting a segment
'                                    Dim StartInsertSample As Integer = Me.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample - (CurrentInterval / 2) - (Math.Abs(AdjustmentLength / 2))
'                                    DSP.InsertSilentSection(InputSound, StartInsertSample, Math.Abs(AdjustmentLength))

'                            End Select

'                            'Adjusting the ptwf data of all following words
'                            For FollowingWordIndex = WordIndex + 1 To Me.ChannelData(PtwfChannel)(sentence).Count - 1
'                                Me.ChannelData(PtwfChannel)(sentence)(FollowingWordIndex).StartSample += AdjustmentLength
'                                For p = 0 To Me.ChannelData(PtwfChannel)(sentence)(FollowingWordIndex).PhoneData.Count - 1
'                                    Me.ChannelData(PtwfChannel)(sentence)(FollowingWordIndex).PhoneData(p).StartSample += AdjustmentLength
'                                Next
'                            Next

'                        Next

'                    End If
'                Catch ex As Exception
'                    MsgBox("An error occurred: " & ex.ToString)
'                End Try

'            End Sub

'            Public Sub MultiWord_InterStimulusSectionFade(ByRef InputSound As Sound, ByVal SoundChannel As Integer, Optional ByVal FadeTime As Double = 0.5)

'                Dim sentence As Integer = 0

'                Try

'                    Dim PtwfChannel As Integer = 1 'This should be a parameter if multi channel ptwfs are needed

'                    'Setting a stimulus interval
'                    Dim GeneralFadeLength As Double = FadeTime * InputSound.WaveFormat.SampleRate

'                    If InputSound IsNot Nothing Then

'                        'Fades in the first sound
'                        DSP.Fade(InputSound, Nothing, 0, SoundChannel, 0, Me.ChannelData(PtwfChannel)(sentence)(0).StartSample, DSP.FadeSlopeType.PowerCosine_SkewedFromFadeDirection, 20)

'                        'Fades out the last sound
'                        Dim FadeLastStart As Integer = Me.ChannelData(PtwfChannel)(sentence)(Me.ChannelData(PtwfChannel)(sentence).Count - 1).StartSample + Me.ChannelData(PtwfChannel)(sentence)(Me.ChannelData(PtwfChannel)(sentence).Count - 1).Length
'                        DSP.Fade(InputSound, 0, Nothing, SoundChannel, FadeLastStart, , DSP.FadeSlopeType.PowerCosine_SkewedFromFadeDirection, 20)

'                        'Fades all inter-stimulis sections
'                        For WordIndex = 0 To Me.ChannelData(PtwfChannel)(sentence).Count - 2

'                            'Getting the interval to the next word
'                            Dim CurrentInterval As Integer =
'                            Me.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample -
'                            (Me.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample + Me.ChannelData(PtwfChannel)(sentence)(WordIndex).Length)


'                            'Determines a suitable fade length
'                            Dim FadeLength As Integer = Math.Min((CurrentInterval / 2) + 2, GeneralFadeLength)

'                            'Fades out the current word
'                            DSP.Fade(InputSound, 0, Nothing, SoundChannel, Me.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample + Me.ChannelData(PtwfChannel)(sentence)(WordIndex).Length, FadeLength)

'                            'Fades in the next sound
'                            DSP.Fade(InputSound, Nothing, 0, SoundChannel, Me.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample - FadeLength, FadeLength)

'                            'Silencing the section between the fades
'                            Dim SilenceStart As Integer = Me.ChannelData(PtwfChannel)(sentence)(WordIndex).StartSample + Me.ChannelData(PtwfChannel)(sentence)(WordIndex).Length
'                            Dim SilenceLength As Integer = (Me.ChannelData(PtwfChannel)(sentence)(WordIndex + 1).StartSample - FadeLength) - SilenceStart
'                            DSP.SilenceSection(InputSound, SilenceStart, SilenceLength, SoundChannel)

'                        Next

'                    End If
'                Catch ex As Exception
'                    MsgBox("An error occurred: " & ex.ToString)
'                End Try

'            End Sub




'            ''' <summary>
'            ''' 
'            ''' </summary>
'            ''' <param name="InputSound"></param>
'            ''' <param name="SoundChannel"></param>
'            ''' <param name="MeasurementSection"></param>
'            ''' <param name="FrequencyWeighting"></param>
'            ''' <param name="DurationList">Can be used to retreive a list of durations (in seconds) used in the measurement.</param>
'            ''' <param name="LengthList">Can be used to retreive a list of lengths (in samples) used in the measurement.</param>
'            ''' <returns></returns>
'            Public Function MultiWord_MeasureSoundLevelsOfSections(ByRef InputSound As Sound, ByVal SoundChannel As Integer,
'                                                          ByVal MeasurementSection As Sound.SpeechMaterialAnnotation.MeasurementSections,
'                                                      Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z,
'                                                               Optional DurationList As List(Of Double) = Nothing,
'                                                               Optional LengthList As List(Of Integer) = Nothing) As Double

'                Dim sentence As Integer = 0

'                Dim PtwfChannel As Integer = 1 'This should be a parameter if multi channel ptwfs are needed

'                Dim FirstPtwfPhonemeIndexToMeasure As Integer = 0
'                Dim LastPtwfPhonemeIndexToMeasure As Integer = 0
'                Select Case MeasurementSection
'                    Case Sound.SpeechMaterialAnnotation.MeasurementSections.CarrierPhrases
'                        FirstPtwfPhonemeIndexToMeasure = 0
'                        LastPtwfPhonemeIndexToMeasure = 0
'                    Case Sound.SpeechMaterialAnnotation.MeasurementSections.TestWords
'                        FirstPtwfPhonemeIndexToMeasure = 1
'                        LastPtwfPhonemeIndexToMeasure = 1
'                    Case Sound.SpeechMaterialAnnotation.MeasurementSections.CarriersAndTestWords
'                        FirstPtwfPhonemeIndexToMeasure = 0
'                        LastPtwfPhonemeIndexToMeasure = 1
'                    Case Else
'                        MsgBox("An error occurred!")
'                        Return Nothing
'                End Select

'                Try

'                    If InputSound IsNot Nothing Then

'                        Dim MeasurementSound As New Sound(InputSound.WaveFormat)

'                        'Gets the length of the MeasurementSound
'                        Dim TotalLength As Integer = 0
'                        For WordIndex = 1 To Me.ChannelData(PtwfChannel)(sentence).Count - 2
'                            Dim CurrentLength As Integer = (Me.ChannelData(PtwfChannel)(sentence)(WordIndex).PhoneData(LastPtwfPhonemeIndexToMeasure).StartSample +
'                            Me.ChannelData(PtwfChannel)(sentence)(WordIndex).PhoneData(LastPtwfPhonemeIndexToMeasure).Length) -
'                            Me.ChannelData(PtwfChannel)(sentence)(WordIndex).PhoneData(FirstPtwfPhonemeIndexToMeasure).StartSample

'                            'Adding the duration
'                            If DurationList IsNot Nothing Then DurationList.Add(CurrentLength / InputSound.WaveFormat.SampleRate)
'                            If LengthList IsNot Nothing Then LengthList.Add(CurrentLength)

'                            TotalLength += CurrentLength
'                        Next
'                        Dim NewArray(TotalLength - 1) As Single
'                        MeasurementSound.WaveData.SampleData(SoundChannel) = NewArray


'                        'Copies all test word sections to a the MeasurementSound
'                        Dim CurrentReadSample As Integer = 0

'                        For WordIndex = 1 To Me.ChannelData(PtwfChannel)(sentence).Count - 2
'                            For s = Me.ChannelData(PtwfChannel)(sentence)(WordIndex).PhoneData(FirstPtwfPhonemeIndexToMeasure).StartSample To Me.ChannelData(PtwfChannel)(sentence)(WordIndex).PhoneData(LastPtwfPhonemeIndexToMeasure).StartSample + Me.ChannelData(PtwfChannel)(sentence)(WordIndex).PhoneData(LastPtwfPhonemeIndexToMeasure).Length - 1
'                                NewArray(CurrentReadSample) = InputSound.WaveData.SampleData(SoundChannel)(s)
'                                CurrentReadSample += 1
'                            Next
'                        Next

'                        Return DSP.MeasureSectionLevel(MeasurementSound, SoundChannel,,,,, FrequencyWeighting)

'                    Else
'                        Error ("No input sound set!")
'                    End If
'                Catch ex As Exception
'                    Throw New Exception("An error occurred: " & ex.ToString)
'                End Try

'            End Function


'#End Region



'#End Region


'            <Serializable>
'            Partial Public Class SmaChannelData
'                Inherits List(Of SmaSentenceData)

'                Private Property ParentSMA As SpeechMaterialAnnotation

'                Public Property OrthographicForm As String = ""
'                Public Property PhoneticForm As String = ""

'                Public Property StartSample As Integer = 0

'                Private _Length As Integer = 0
'                Public Property Length As Integer
'                    Get
'                        'Updates the length based on the audio channel
'                        UpdateLength()
'                        Return _Length
'                    End Get
'                    Set(value As Integer)
'                        _Length = value
'                    End Set
'                End Property

'                ''' <summary>
'                ''' Updates the length of the audio channel
'                ''' </summary>
'                ''' <returns></returns>
'                Private Function UpdateLength() As Boolean
'                    If ParentSMA.ParentSound IsNot Nothing Then
'                        Length = ParentSMA.ParentSound.WaveData.SampleData(GetParentAudioChannel).Length
'                        Return True
'                    Else
'                        Return False
'                    End If
'                End Function

'                Private Function GetParentAudioChannel() As Integer
'                    If ParentSMA IsNot Nothing Then
'                        Dim ChannelIndex As Integer = 0
'                        For c = 1 To ParentSMA.ChannelCount
'                            If ParentSMA.ChannelData(c) Is Me Then ChannelIndex = c
'                        Next
'                        Return ChannelIndex
'                    Else
'                        Throw New Exception("No parent SMA exists. Cannot retrieve the current SMA channel parent audio channel.")
'                    End If
'                End Function

'                Public Property UnWeightedLevel As Double? = Nothing
'                Public Property UnWeightedPeakLevel As Double? = Nothing

'                Public Property WeightedLevel As Double? = Nothing

'                Private _FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z
'                Public Function GetFrequencyWeighting() As FrequencyWeightings
'                    Return _FrequencyWeighting
'                End Function

'                Public Sub SetFrequencyWeighting(ByVal FrequencyWeighting As FrequencyWeightings, ByVal EnforceOnAllDescendents As Boolean)

'                    'Setting only SMA top level frequency weighting
'                    _FrequencyWeighting = FrequencyWeighting

'                    If EnforceOnAllDescendents = True Then

'                        'Enforcing the same frequency weighting on all descendant sentences, words, and phones
'                        For s = 0 To Me.Count - 1
'                            Me(s).SetFrequencyWeighting(FrequencyWeighting, False)
'                            For w = 0 To Me(s).Count - 1
'                                Me(s)(w).SetFrequencyWeighting(FrequencyWeighting, False)
'                                For p = 0 To Me(s)(w).PhoneData.Count - 1
'                                    Me(s)(w).PhoneData(p).FrequencyWeighting = FrequencyWeighting
'                                Next
'                            Next
'                        Next
'                    End If
'                End Sub

'                Private _TimeWeighting As Double = 0 'A time weighting of 0 indicates "no time weighting", thus the average RMS level is indicated.
'                Public Function GetTimeWeighting() As Double
'                    Return _TimeWeighting
'                End Function

'                Public Sub SetTimeWeighting(ByVal TimeWeighting As Double, ByVal EnforceOnAllDescendents As Boolean)

'                    'Setting only SMA top level Time weighting
'                    _TimeWeighting = TimeWeighting

'                    If EnforceOnAllDescendents = True Then

'                        'Enforcing the same Time weighting on all descendant sentences, words, and phones
'                        For s = 0 To Me.Count - 1
'                            Me(s).SetTimeWeighting(TimeWeighting, False)
'                            For w = 0 To Me(s).Count - 1
'                                Me(s)(w).SetTimeWeighting(TimeWeighting, False)
'                                For p = 0 To Me(s)(w).PhoneData.Count - 1
'                                    Me(s)(w).PhoneData(p).TimeWeighting = TimeWeighting
'                                Next
'                            Next
'                        Next
'                    End If
'                End Sub

'                Public Sub New(ByRef ParentSMA As SpeechMaterialAnnotation)
'                    Me.ParentSMA = ParentSMA
'                    Me.SetFrequencyWeighting(ParentSMA.GetFrequencyWeighting, True)
'                    Me.SetTimeWeighting(ParentSMA.GetTimeWeighting, True)
'                End Sub

'            End Class

'            <Serializable>
'            Partial Public Class SmaSentenceData
'                Inherits List(Of SmaWordData)

'                Private Property ParentSMA As SpeechMaterialAnnotation

'                Public Property OrthographicForm As String = ""
'                Public Property PhoneticForm As String = ""

'                Public Property StartSample As Integer = -1
'                Public Property Length As Integer = 0
'                Public Property UnWeightedLevel As Double? = Nothing
'                Public Property UnWeightedPeakLevel As Double? = Nothing

'                Public Property WeightedLevel As Double? = Nothing

'                Private _FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z
'                Public Function GetFrequencyWeighting() As FrequencyWeightings
'                    Return _FrequencyWeighting
'                End Function

'                Public Sub SetFrequencyWeighting(ByVal FrequencyWeighting As FrequencyWeightings, ByVal EnforceOnAllDescendents As Boolean)

'                    'Setting only SMA top level frequency weighting
'                    _FrequencyWeighting = FrequencyWeighting

'                    If EnforceOnAllDescendents = True Then

'                        'Enforcing the same frequency weighting on all descendant words, and phones
'                        For w = 0 To Me.Count - 1
'                            Me(w).SetFrequencyWeighting(FrequencyWeighting, False)
'                            For p = 0 To Me(w).PhoneData.Count - 1
'                                Me(w).PhoneData(p).FrequencyWeighting = FrequencyWeighting
'                            Next
'                        Next
'                    End If
'                End Sub

'                Private _TimeWeighting As Double = 0 'A time weighting of 0 indicates "no time weighting", thus the average RMS level is indicated.
'                Public Function GetTimeWeighting() As Double
'                    Return _TimeWeighting
'                End Function

'                Public Sub SetTimeWeighting(ByVal TimeWeighting As Double, ByVal EnforceOnAllDescendents As Boolean)

'                    'Setting only SMA top level Time weighting
'                    _TimeWeighting = TimeWeighting

'                    If EnforceOnAllDescendents = True Then

'                        'Enforcing the same Time weighting on all descendant words, and phones
'                        For w = 0 To Me.Count - 1
'                            Me(w).SetTimeWeighting(TimeWeighting, False)
'                            For p = 0 To Me(w).PhoneData.Count - 1
'                                Me(w).PhoneData(p).TimeWeighting = TimeWeighting
'                            Next
'                        Next
'                    End If
'                End Sub

'                ''' <summary>
'                ''' The initial peak amplitude value is the absulote value of the highest detected sample value within the word parts of a segmented audio recording, before any gain has been applied to the audio segment. 
'                ''' Its default value is -1. Before any gain is applied to such a segment the InitialPeak should be measured using the method SetInitialPeakAmplitude. And once it has been measured (i.e. has a value other than -1), it should not be measured again. 
'                ''' </summary>
'                ''' <returns></returns>
'                Public Property InitialPeak As Double = -1
'                Public Property StartTime As Double

'                Public Sub New(ByRef ParentSMA As SpeechMaterialAnnotation)
'                    Me.ParentSMA = ParentSMA
'                    Me.SetFrequencyWeighting(ParentSMA.GetFrequencyWeighting, True)
'                    Me.SetTimeWeighting(ParentSMA.GetTimeWeighting, True)
'                End Sub

'#Region "ExtensionMethods"

'                Private Function CheckStartAndLength() As Boolean

'                    'Checks to see that start sample and length are assigned before sound measurements
'                    If (Me.StartSample < 0 Or Me.Length < 0) Then
'                        'Warnings("WordData cannot measure sound since one Or both of setStartSample Or setLength has Not been assigned a value.")
'                        Return False
'                    Else
'                        Return True
'                    End If

'                End Function

'                ''' <summary>
'                ''' Compares the current peak amplitude with the InitialPeak of the word segments in the current channel of the audio recording 
'                ''' and returns the gain that has been applied to the audio since the initial recording.
'                ''' </summary>
'                ''' <returns>Returns the gain applied since recoring, or Nothing if measurements failed.</returns>
'                Public Function GetCurrentGain(ByVal MeasurementSound As Sound, ByVal MeasurementChannel As Integer,
'                                   Optional ByVal MeasurementSentence As Integer = 0) As Double?


'                    'Getting the current peak amplitude of each word in the MeasurementChannel, and stores them in a list
'                    Dim FailedMeasurements As Integer = 0
'                    Dim CurrentPeakAmplitudes As New List(Of Double)

'                    For word = 0 To MeasurementSound.SMA.ChannelData(MeasurementChannel)(MeasurementSentence).Count - 1

'                        If Me.CheckStartAndLength() = False Then
'                            FailedMeasurements += 1
'                            Continue For
'                        End If

'                        Dim soundLength As Integer = MeasurementSound.WaveData.ShortestChannelSampleCount
'                        If Me.StartSample + Me.Length > soundLength Then
'                            FailedMeasurements += 1
'                            Continue For
'                        End If

'                        'Meaures UnWeightedPeakLevel
'                        Dim CurrentWordPeakLevel As Double? = DSP.MeasureSectionLevel(MeasurementSound, MeasurementChannel, Me.StartSample, Me.Length, SoundDataUnit.dB, SoundMeasurementType.AbsolutePeakAmplitude)

'                        'Noting failed measurment
'                        If CurrentWordPeakLevel Is Nothing Then
'                            FailedMeasurements += 1
'                            Continue For
'                        End If

'                        'Stores the peak level of the current word
'                        CurrentPeakAmplitudes.Add(CurrentWordPeakLevel.Value)

'                    Next

'                    'Getting the highest peak level of the words in MeasurementChannel
'                    Dim CurrentPeakAmplitude = CurrentPeakAmplitudes.Max

'                    'Converting the initial peak amplitude to dB
'                    Dim InitialPeakLevel As Double = dBConversion(MeasurementSound.SMA.ChannelData(MeasurementChannel)(MeasurementSentence).InitialPeak,
'                                                              dBConversionDirection.to_dB, MeasurementSound.WaveFormat)

'                    'Getting the currently applied gain
'                    Dim Gain As Double = CurrentPeakAmplitude - InitialPeakLevel

'                    'Returns Nothing if any measurements failed
'                    If FailedMeasurements > 0 Then Return Nothing

'                    Return Gain

'                End Function

'#End Region

'            End Class


'            ''' <summary>
'            '''Is used to store data for segmentation and location of recorded words and phones. 
'            '''Can also perform sound measurements on the indicated words and phones, as soon as both start sample and length is assigned
'            ''' </summary>
'            <Serializable>
'            Public Class SmaWordData

'                Private Property ParentSMA As SpeechMaterialAnnotation

'                Public Property OrthographicForm As String = ""
'                Public Property PhoneticForm As String = ""

'                Public Property StartSample As Integer = -1
'                Public Property Length As Integer = 0

'                'Sound level properties. vbNull if never set
'                Public Property UnWeightedLevel As Double? = Nothing
'                Public Property UnWeightedPeakLevel As Double? = Nothing

'                Public Property WeightedLevel As Double? = Nothing

'                Private _FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z
'                Public Function GetFrequencyWeighting() As FrequencyWeightings
'                    Return _FrequencyWeighting
'                End Function

'                Public Sub SetFrequencyWeighting(ByVal FrequencyWeighting As FrequencyWeightings, ByVal EnforceOnAllDescendents As Boolean)

'                    'Setting only SMA top level frequency weighting
'                    _FrequencyWeighting = FrequencyWeighting

'                    If EnforceOnAllDescendents = True Then

'                        'Enforcing the same frequency weighting on all descendant phones
'                        For p = 0 To PhoneData.Count - 1
'                            PhoneData(p).FrequencyWeighting = FrequencyWeighting
'                        Next
'                    End If
'                End Sub

'                Private _TimeWeighting As Double = 0 'A time weighting of 0 indicates "no time weighting", thus the average RMS level is indicated.
'                Public Function GetTimeWeighting() As Double
'                    Return _TimeWeighting
'                End Function

'                Public Sub SetTimeWeighting(ByVal TimeWeighting As Double, ByVal EnforceOnAllDescendents As Boolean)

'                    'Setting only SMA top level Time weighting
'                    _TimeWeighting = TimeWeighting

'                    If EnforceOnAllDescendents = True Then

'                        'Enforcing the same Time weighting on all descendant phones
'                        For p = 0 To PhoneData.Count - 1
'                            PhoneData(p).TimeWeighting = TimeWeighting
'                        Next
'                    End If
'                End Sub

'                'Word start time  
'                Public Property StartTime As Double

'                'Phone data
'                Public Property PhoneData As New List(Of SmaPhoneData)

'                Public Sub New(ByRef ParentSMA As SpeechMaterialAnnotation)
'                    Me.ParentSMA = ParentSMA
'                    Me.SetFrequencyWeighting(ParentSMA.GetFrequencyWeighting, True)
'                    Me.SetTimeWeighting(ParentSMA.GetTimeWeighting, True)
'                End Sub

'#Region "ExtensionMethods"

'                ''' <summary>
'                ''' Gets a cropped copy of the specified phoneme section of the InputSound.
'                ''' </summary>
'                '''<param name="PhonemeIndex">The zero-based index of the phoneme that should be exported.</param>
'                ''' <param name="FadedMargins">Specifies a proportion of any adjacent phonemes before and/or after the selected phoneme that should be included in the output sound. The margins are faded using a smooth fade. If left to Nothing, no margins will be included.</param>
'                ''' <param name="FrequencyWeighting">An optional filterring using a specified frequency weighting that can be applied to the output sound.</param>
'                ''' <returns></returns>
'                Public Function GetCroppedPhonemeSound(ByVal InputSound As Sound, ByVal PhonemeIndex As Integer,
'                                                       Optional ByVal FadedMargins As Single? = Nothing,
'                                                       Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.RLB) As Sound


'                    'Checking if FadedMargins is within the allowed range of 0-1
'                    If FadedMargins < 0 Then Throw New ArgumentException("FadedMargins cannot be lower than 0. It should be a proportion between 1 and 0.")
'                    If FadedMargins > 1 Then Throw New ArgumentException("FadedMargins cannot be higher than 1. It should be a proportion between 1 and 0.")

'                    'Copying the sound source sound
'                    Dim OutputSound As New Sound(InputSound.WaveFormat)

'                    Dim StartReadSample As Integer
'                    Dim LastReadSample As Integer
'                    Dim FadeInLength As Integer
'                    Dim FadeOutStartPosition As Integer

'                    If FadedMargins Is Nothing Then
'                        'Setting the read section to the selected phoneme location
'                        StartReadSample = Me.PhoneData(PhonemeIndex).StartSample
'                        LastReadSample = StartReadSample + Me.PhoneData(PhonemeIndex).Length - 1

'                        'Setting no fade
'                        FadeInLength = 0
'                        FadeOutStartPosition = LastReadSample + 1 'I.e. outside the sound array

'                    Else

'                        'Adding margins
'                        'Initial margin
'                        If PhonemeIndex = 0 Then

'                            'No initial margin, since it's the first phoneme
'                            'Setting start read position to phoneme start
'                            StartReadSample = Me.PhoneData(PhonemeIndex).StartSample
'                            'Setting no fade
'                            FadeInLength = 0

'                        Else
'                            'Setting start read position inside the previous phoneme
'                            Dim FadeInMargin As Integer = Int((1 - FadedMargins) * Me.PhoneData(PhonemeIndex - 1).Length)
'                            StartReadSample = Me.PhoneData(PhonemeIndex - 1).StartSample + FadeInMargin
'                            'Setting no fade
'                            FadeInLength = FadeInMargin
'                        End If

'                        'Final margin
'                        'Checking first that the last it is not the word end marker that is specified
'                        If PhonemeIndex = Me.PhoneData.Count - 1 Then Throw New ArgumentException("The last phoneme should be a word end marker and cannot be specified when exporting phoneme sound data.")

'                        If PhonemeIndex = Me.PhoneData.Count - 2 Then '-2 is used instead of -1, since the last "phoneme" is a word end marker, and the real last phoneme comes before that.

'                            'No final margin, since it's the last phoneme
'                            'Setting reading length to the start position + the length of the selected phoneme
'                            LastReadSample = Me.PhoneData(PhonemeIndex).StartSample + Me.PhoneData(PhonemeIndex).Length - 1

'                            'Setting no fade
'                            FadeOutStartPosition = LastReadSample + 1 'I.e. outside the sound array

'                        Else

'                            Dim TargetPhonemeLastSample As Integer = Me.PhoneData(PhonemeIndex).StartSample + Me.PhoneData(PhonemeIndex).Length - 1
'                            LastReadSample = TargetPhonemeLastSample + FadedMargins * Me.PhoneData(PhonemeIndex + 1).Length
'                            FadeOutStartPosition = TargetPhonemeLastSample + 1

'                        End If
'                    End If

'                    'Cropping the selected section
'                    Dim ReadLength As Integer = LastReadSample - StartReadSample + 1

'                    For c = 1 To OutputSound.WaveFormat.Channels
'                        Dim NewSoundArray(ReadLength - 1) As Single
'                        For n = 0 To NewSoundArray.Length - 1
'                            NewSoundArray(n) = InputSound.WaveData.SampleData(c)(n + StartReadSample)
'                        Next
'                        OutputSound.WaveData.SampleData(c) = NewSoundArray
'                    Next

'                    'Fading in and out
'                    If FadeInLength > 0 Then DSP.Fade(OutputSound, Nothing, 0,, 0, FadeInLength, DSP.FadeSlopeType.Smooth)
'                    If FadeOutStartPosition < LastReadSample Then
'                        Dim FadeOutLength As Integer = LastReadSample - FadeOutStartPosition
'                        DSP.Fade(OutputSound, 0, Nothing,, OutputSound.WaveData.ShortestChannelSampleCount - FadeOutLength, FadeOutLength, DSP.FadeSlopeType.Smooth)
'                    End If

'                    'Filterring the output sound
'                    If FrequencyWeighting <> FrequencyWeightings.Z Then
'                        OutputSound = DSP.IIRFilter(OutputSound, FrequencyWeighting)
'                    End If

'                    'Copying the filename to the OutputSound sound
'                    OutputSound.FileName = InputSound.FileName

'                    Return OutputSound

'                End Function


'                Private Function CheckStartAndLength() As Boolean

'                    'Checks to see that start sample and length are assigned before sound measurements
'                    If (Me.StartSample < 0 Or Me.Length < 0) Then
'                        'Warnings("WordData cannot measure sound since one Or both of setStartSample Or setLength has Not been assigned a value.")
'                        Return False
'                    Else
'                        Return True
'                    End If

'                End Function



'#End Region


'            End Class

'            ''' <summary>
'            ''' Is used to store data for segmentation and location of recorded phones. 
'            ''' Can also perform sound measurements On the indicated sound.
'            ''' </summary>
'            <Serializable>
'            Public Class SmaPhoneData

'                Private Property ParentSMA As SpeechMaterialAnnotation

'                Public Property OrthographicForm As String = ""
'                Public Property PhoneticForm As String = ""

'                Public Property StartSample As Integer = -1
'                Public Property Length As Integer = 0

'                'Sound level properties. vbNull if never set
'                Public Property UnWeightedLevel As Double? = Nothing
'                Public Property UnWeightedPeakLevel As Double? = Nothing

'                Public Property FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z
'                Public Property TimeWeighting As Double = 0 'A time weighting of 0 indicates "no time weighting", thus the average RMS level is indicated.
'                Public Property WeightedLevel As Double? = Nothing

'                Public Sub New(ByRef ParentSMA As SpeechMaterialAnnotation)
'                    Me.ParentSMA = ParentSMA
'                    Me.FrequencyWeighting = ParentSMA.GetFrequencyWeighting
'                    Me.TimeWeighting = ParentSMA.GetTimeWeighting
'                End Sub

'#Region "ExtensionMethods"

'                Private Function CheckStartAndLength() As Boolean

'                    'Checks to see that start sample and length are assigned before sound measurements
'                    If (Me.StartSample < 0 Or Me.Length < 0) Then
'                        'Warnings("WordData cannot measure sound since one Or both of setStartSample Or setLength has Not been assigned a value.")
'                        Return False
'                    Else
'                        Return True
'                    End If

'                End Function

'#End Region

'            End Class


'            Public Enum MeasurementSections
'                CarrierPhrases
'                TestWords
'                CarriersAndTestWords
'            End Enum




'        End Class
