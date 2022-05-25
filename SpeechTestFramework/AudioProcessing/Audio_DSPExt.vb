Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows.Forms


Namespace Audio

    Namespace DSP

        Public Module MeasurementsExt

            '''' <summary>
            '''' Measures the sound level of a section of the input sound.
            '''' </summary>
            '''' <param name="InputSound"></param>
            '''' <param name="channel"></param>
            '''' <param name="startSample"></param>
            '''' <param name="sectionLength"></param>
            '''' <param name="outputUnit"></param>
            '''' <param name="SoundMeasurementType"></param>
            '''' <param name="Frequencyweighting">The frequency Weighting to be applied before the sound measurement.</param>
            '''' <param name="ReturnLinearMeanSquareData">If set to true, the linear mean square of the measured section is returned (I.e. Any values set for outputUnit and SoundMeasurementType are overridden.).</param>
            '''' <param name="LinearSquareData">If ReturnLinearMeanSquareData is set to True, LinearSquareData will contain item1 = linear sum of square, and item2 = length of the measurement section in samples.</param>
            '''' <returns></returns>
            'Public Function MeasureSectionLevel(ByRef InputSound As Sound, ByVal channel As Integer,
            '                                    Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing,
            '                                    Optional ByVal outputUnit As SoundDataUnit = SoundDataUnit.dB,
            '                                    Optional ByVal SoundMeasurementType As SoundMeasurementType = SoundMeasurementType.RMS,
            '                                    Optional ByVal Frequencyweighting As FrequencyWeightings = FrequencyWeightings.Z,
            '                                    Optional ByVal ReturnLinearMeanSquareData As Boolean = False,
            '                                    Optional ByRef LinearSquareData As Tuple(Of Double, Integer) = Nothing) As Double?

            '    Try

            '        CheckAndCorrectSectionLength(InputSound.WaveData.SampleData(channel).Length, startSample, sectionLength)

            '        'Preparing an array to do measurements on
            '        Dim MeasurementArray() As Single

            '        If Frequencyweighting = FrequencyWeightings.Z Then
            '            'Just referencing the input sound array
            '            MeasurementArray = InputSound.WaveData.SampleData(channel)

            '        Else
            '            'Preparing a new sound with only the measurement section, to be filterred
            '            Dim tempSound As New Sound(InputSound.WaveFormat)
            '            ReDim MeasurementArray(sectionLength - 1)
            '            For s = startSample To startSample + sectionLength - 1
            '                MeasurementArray(s - startSample) = InputSound.WaveData.SampleData(channel)(s)
            '            Next
            '            tempSound.WaveData.SampleData(channel) = MeasurementArray

            '            'Filterring for Weighing
            '            tempSound = DSP.IIRFilter(tempSound, Frequencyweighting, channel)
            '            If tempSound Is Nothing Then
            '                Throw New Exception("Something went wrong during IIR-filterring")
            '                Return Nothing 'Aborting and return vb null if something went wrong here
            '            End If

            '            'Referencing the MeasurementArray again (since the reference is broken during the IIR filtering)
            '            MeasurementArray = tempSound.WaveData.SampleData(channel)

            '            'Setting startsample to 0 since all sound before the startsample has been excluded from MeasurementArray
            '            startSample = 0

            '        End If

            '        'Overrides soundmeasurmenttype
            '        If ReturnLinearMeanSquareData = True Then SoundMeasurementType = SoundMeasurementType.RMS

            '        Select Case SoundMeasurementType
            '            Case SoundMeasurementType.RMS

            '                'Calculates RMS value of the section

            '                Dim AccumulativeSoundLevel As Double
            '                For n = startSample To startSample + sectionLength - 1
            '                    AccumulativeSoundLevel = AccumulativeSoundLevel + MeasurementArray(n) ^ 2
            '                Next

            '                'Returns the mean square (MR) if ReturnLinearMeanSquareData is True
            '                If ReturnLinearMeanSquareData = True Then

            '                    'Stores LinearSquareData
            '                    LinearSquareData = New Tuple(Of Double, Integer)(AccumulativeSoundLevel, sectionLength)

            '                    'Returns the ReturnLinearMeanSquareData
            '                    Return AccumulativeSoundLevel / sectionLength
            '                End If

            '                'Calculates RMS
            '                Dim RMS = (AccumulativeSoundLevel / sectionLength) ^ (1 / 2)

            '                Select Case outputUnit
            '                    Case SoundDataUnit.dB
            '                        Dim sectionLevel As Double = dBConversion(RMS, dBConversionDirection.to_dB, InputSound.WaveFormat)
            '                        Return sectionLevel
            '                    Case SoundDataUnit.linear
            '                        Return RMS
            '                End Select

            '            Case SoundMeasurementType.AbsolutePeakAmplitude
            '                'Calculates the absolute max amplitude of the section

            '                Dim peak_pos As Double '= inputArray.Max (Detekterar peakvärdet för hela Arrayen) 
            '                Dim peak_neg As Double '= inputArray.Min

            '                'Detekterar peakvärdet för sectionen
            '                For n = startSample To startSample + sectionLength - 1
            '                    If MeasurementArray(n) > peak_pos Then peak_pos = MeasurementArray(n)
            '                    If MeasurementArray(n) < peak_neg Then peak_neg = MeasurementArray(n)
            '                Next

            '                Dim peak As Double = 0

            '                If peak_pos > -peak_neg Then
            '                    peak = peak_pos
            '                Else
            '                    peak = -peak_neg
            '                End If

            '                Select Case outputUnit
            '                    Case SoundDataUnit.dB
            '                        Dim sectionLevel As Double = dBConversion(peak, dBConversionDirection.to_dB, InputSound.WaveFormat)
            '                        Return sectionLevel
            '                    Case SoundDataUnit.linear
            '                        Return peak
            '                End Select

            '            Case SoundMeasurementType.averageAbsoluteAmplitude

            '                'Calculates the average absolute amplitude of the section
            '                Dim AccumulativeSoundLevel As Double

            '                'MsgBox(inputArray.Length & " " & startSample & " " & sectionLength)

            '                For n = startSample To startSample + sectionLength - 1
            '                    AccumulativeSoundLevel = AccumulativeSoundLevel + Math.Abs(MeasurementArray(n))
            '                Next

            '                Dim averageAbsoluteAmplitude = AccumulativeSoundLevel / sectionLength

            '                Select Case outputUnit
            '                    Case SoundDataUnit.dB
            '                        Dim sectionLevel As Double = dBConversion(averageAbsoluteAmplitude, dBConversionDirection.to_dB, InputSound.WaveFormat)
            '                        Return sectionLevel
            '                    Case SoundDataUnit.linear
            '                        Return averageAbsoluteAmplitude
            '                End Select

            '        End Select


            '    Catch ex As Exception
            '        AudioError(ex.ToString)
            '        Return Nothing
            '    End Try

            'End Function




            ''' <summary>
            ''' Measures the added sound level of the indicated channels in the specified section of the input sound.
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="channels"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength"></param>
            ''' <param name="outputUnit"></param>
            ''' <param name="SoundMeasurementType"></param>
            ''' <param name="Frequencyweighting">The frequency Weighting to be applied before the sound measurement.</param>
            ''' <param name="ReturnLinearMeanSquareData">If set to true, the linear mean square of the measured section is returned (I.e. Any values set for outputUnit and SoundMeasurementType are overridden.).</param>
            ''' <param name="LinearSquareData">If ReturnLinearMeanSquareData is set to True, LinearSquareData will contain item1 = linear sum of square, and item2 = length of the measurement section in samples.</param>
            ''' <returns></returns>
            Public Function MeasureSectionLevel_AddedChannels(ByRef InputSound As Sound, ByVal Channels As List(Of Integer),
                                            Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing,
                                            Optional ByVal OutputUnit As SoundDataUnit = SoundDataUnit.dB,
                                            Optional ByVal SoundMeasurementType As SoundMeasurementType = SoundMeasurementType.RMS,
                                            Optional ByVal Frequencyweighting As FrequencyWeightings = FrequencyWeightings.Z,
                                            Optional ByVal ReturnLinearMeanSquareData As Boolean = False,
                                            Optional ByRef LinearSquareData As Tuple(Of Double, Integer) = Nothing) As Double?

                Try

                    'Checking some arguments
                    If Channels.Count = 0 Then Throw New ArgumentException("Channel count cannot be 0")
                    For Each c In Channels
                        If c < 1 Then Throw New ArgumentException("Channel index cannot be lower than 1")
                    Next

                    'Determining the shortest channel length
                    Dim ShortestLength As Integer = Integer.MaxValue
                    Dim ShortestChannel As Integer = -1
                    For Each c In Channels
                        If InputSound.WaveData.SampleData(c).Length < ShortestLength Then
                            ShortestLength = InputSound.WaveData.SampleData(c).Length
                            ShortestChannel = c
                        End If
                    Next

                    'Using the shortest channel to check for valid start sample and section length
                    CheckAndCorrectSectionLength(ShortestLength, StartSample, SectionLength)

                    'Sending off straight to MeasureSectionLevel if there is only one channel
                    If Channels.Count = 1 Then
                        Return MeasureSectionLevel(InputSound, Channels(0), StartSample, SectionLength, OutputUnit, SoundMeasurementType, Frequencyweighting, ReturnLinearMeanSquareData, LinearSquareData)
                    End If

                    'Creating a new sound with the appropriate channels combined
                    Dim MeasurementSound As New Sound(New Formats.WaveFormat(InputSound.WaveFormat.SampleRate,
                                                                               InputSound.WaveFormat.BitDepth, 1,,
                                                                               InputSound.WaveFormat.Encoding))

                    'Creating a channel array (using the length of the input sound to avoid additions of indexes below)
                    Dim MeasurmentSoundChannelArray(ShortestLength - 1) As Single
                    MeasurementSound.WaveData.SampleData(1) = MeasurmentSoundChannelArray

                    Select Case Channels.Count
                        Case 2
                            'Creating a custum loop for 2 channels to optimize processing
                            Dim InputSoundFirstChannel = InputSound.WaveData.SampleData(Channels(0))
                            Dim InputSoundSecondChannel = InputSound.WaveData.SampleData(Channels(1))

                            'Adding the channel sounds
                            For s = StartSample To StartSample + SectionLength
                                MeasurmentSoundChannelArray(s) = InputSoundFirstChannel(s) + InputSoundSecondChannel(s)
                            Next

                        Case Else

                            'Adding the channel sounds
                            For Each c In Channels
                                For s = StartSample To StartSample + SectionLength
                                    MeasurmentSoundChannelArray(s) += InputSound.WaveData.SampleData(c)(s)
                                Next
                            Next

                    End Select

                    'Sending off to MeasureSectionLevel to measure the combined sound level
                    Return MeasureSectionLevel(MeasurementSound, 1, StartSample, SectionLength, OutputUnit, SoundMeasurementType, Frequencyweighting, ReturnLinearMeanSquareData, LinearSquareData)

                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try

            End Function


            Public Function GetEnvelope(ByRef InputSound As Sound, ByVal Channel As Integer, ByVal MeasurementSectionLength As Integer,
                                    Optional ByVal OverlapLength As Integer = 0,
                                    Optional ByVal outputUnit As SoundDataUnit = SoundDataUnit.dB,
                                    Optional ByVal SoundMeasurementType As SoundMeasurementType = SoundMeasurementType.RMS,
                                    Optional ByVal Frequencyweighting As FrequencyWeightings = FrequencyWeightings.Z,
                                    Optional ByVal SkipLastWindow As Boolean = True) As List(Of Double)


                Dim WindowCount As Integer = Int(InputSound.WaveData.SampleData(Channel).Length / (MeasurementSectionLength - OverlapLength))

                Dim Envelope As New List(Of Double)

                Dim StartSample As Integer = 0

                Dim LastWindowsCorrection As Integer = 1
                If SkipLastWindow = True Then LastWindowsCorrection = 2 'Skipping the last window since it may not be full

                For n = 0 To WindowCount - LastWindowsCorrection

                    Envelope.Add(MeasureSectionLevel(InputSound, Channel, StartSample, MeasurementSectionLength, outputUnit, SoundMeasurementType, Frequencyweighting))
                    StartSample += (MeasurementSectionLength - OverlapLength)

                Next

                Return Envelope

            End Function


            ''' <summary>
            ''' Returns the RMS of the window with the lowest RMS value.
            ''' </summary>
            ''' <param name="InputSound">The sound to measure.</param>
            ''' <param name="WindowSize">The windows size in samples.</param>
            ''' <param name="QuietestWindowStartSample">Upon return, holds the start sample of loudest window.</param>
            ''' <returns></returns>
            Public Function GetLevelOfQuietestWindow(ByRef InputSound As Sound, ByVal Channel As Integer, ByVal WindowSize As Integer,
                                                Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing,
                                                Optional ByRef QuietestWindowStartSample As Integer = 0,
                                                Optional ByVal Frequencyweighting As FrequencyWeightings = FrequencyWeightings.Z,
                                                Optional ByVal ZeroPadToWindowSize As Boolean = False) As Double

                CheckAndCorrectSectionLength(InputSound.WaveData.SampleData(Channel).Length, StartSample, SectionLength)

                'Stores the initial start sample value since this is changed if the sound is filtering below
                Dim InitialStartSample As Integer = StartSample

                'Resetting QuietestWindowStartSample
                QuietestWindowStartSample = 0

                'Copying the section to do measurements on to a new sound
                Dim TempSound As New Sound(New Formats.WaveFormat(InputSound.WaveFormat.SampleRate, InputSound.WaveFormat.BitDepth, 1,, InputSound.WaveFormat.Encoding))
                If Frequencyweighting = FrequencyWeightings.Z And StartSample = 0 And SectionLength = InputSound.WaveData.SampleData(Channel).Length Then
                    'No need to copy, only referening the sound
                    TempSound.WaveData.SampleData(1) = InputSound.WaveData.SampleData(Channel)

                Else
                    'Coying samples
                    Dim MeasurementArray(SectionLength - 1) As Single
                    For s = StartSample To StartSample + SectionLength - 1
                        MeasurementArray(s - StartSample) = InputSound.WaveData.SampleData(Channel)(s)
                    Next
                    TempSound.WaveData.SampleData(1) = MeasurementArray

                End If

                'Filtering the TempSound 
                If Frequencyweighting <> FrequencyWeightings.Z Then
                    TempSound = DSP.IIRFilter(TempSound, Frequencyweighting, Channel)
                    If TempSound Is Nothing Then
                        Throw New Exception("Something went wrong during IIR-filterring")
                        Return Nothing 'Aborting and returning Nothing if something went wrong here
                    End If
                End If


                Dim LowestSumOfSquares As Double = Double.PositiveInfinity

                'If the section to measure is shorter than the WindowSize, MeasureSectionLevel is used directly (with or without zero-padding)
                If WindowSize > TempSound.WaveData.SampleData(1).Length Then
                    If ZeroPadToWindowSize = True Then

                        ''Creating a zero-padded measurement array
                        'Dim ZeroPaddedMeasurementArray(WindowSize - 1) As Single
                        'For s = 0 To TempSound.WaveData.SampleData(1).Length - 1
                        '    ZeroPaddedMeasurementArray(s) = TempSound.WaveData.SampleData(1)(s)
                        'Next
                        ''Referencing the new array in the temporary measurement sound
                        'TempSound.WaveData.SampleData(1) = redim  TempSound.WaveData.SampleData(1) 

                        ReDim Preserve TempSound.WaveData.SampleData(1)(WindowSize - 1)
                    End If

                    'Measures and returns the level of the whole TempSound
                    Return MeasureSectionLevel(TempSound, 1, ,,,, FrequencyWeightings.Z)

                End If

                Dim CurrentSumOfSquares As Double
                Dim LeftToUpdate As Integer = 0

                For s = 0 To TempSound.WaveData.SampleData(1).Length - 1 - WindowSize

                    If LeftToUpdate = 0 Then

                        'Re-calculating a new CurrentSumOfSquares, by iterating all samples in the current window. The reason this is done is to reduce the accumulated effect of rounding caused by adding and subtracting floating point numbers.
                        CurrentSumOfSquares = 0
                        For ws = s To s + WindowSize - 1
                            CurrentSumOfSquares += TempSound.WaveData.SampleData(1)(ws) ^ 2
                        Next

                        LeftToUpdate = WindowSize - 1

                    Else

                        'Subtracting the square of the s - 1 sample
                        CurrentSumOfSquares -= TempSound.WaveData.SampleData(1)(s - 1) ^ 2

                        'Adding the square of the new sample (s + WindowSize - 1)
                        CurrentSumOfSquares += TempSound.WaveData.SampleData(1)(s + WindowSize - 1) ^ 2

                        'Decreasing the samples left to update
                        LeftToUpdate -= 1
                    End If

                    'Updating and storing the quietest window start sample
                    If CurrentSumOfSquares < LowestSumOfSquares Then
                        LowestSumOfSquares = CurrentSumOfSquares
                        QuietestWindowStartSample = s
                    End If

                Next

                'Adding the section length removed prior to the initial start sample to QuietestWindowStartSample
                QuietestWindowStartSample += InitialStartSample

                'Calulating the RMS level in dB
                If LowestSumOfSquares > 0 Then
                    Return dBConversion(Math.Sqrt(LowestSumOfSquares / WindowSize), dBConversionDirection.to_dB, TempSound.WaveFormat)
                Else
                    Return Double.NegativeInfinity
                End If

            End Function

            ''' <summary>
            ''' Measures a gated sound level of a section of a wave file. 
            ''' Method: a) the sound level of all GatingWindowDuration long windows in the section is measured,
            ''' b) the windows are sorted according to decreasing sound levels
            ''' c) An absolute gating level is determined by adding GateRelativeThreshold to the average sound level of the loudest proportion (ProportionForCalculatingAbsThreshold) of windows.
            ''' d) The average sound level of all windows equal to or louder than the absolute gating level is calculated and returned. 
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="channel"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength"></param>
            ''' <param name="outputUnit"></param>
            ''' <param name="GatingWindowDuration"></param>
            ''' <param name="GateRelativeThreshold"></param>
            ''' <param name="ProportionForCalculatingAbsThreshold"></param>
            ''' <param name="FrequencyWeighting">A frequency weighting may be applied before calculating sound levels.</param>
            ''' <returns>Returns the average sound level of measurement windows equal to or louder than the the loudest proportion (ProportionForCalculatingAbsThreshold) of windows + GateRelativeThreshold, or Nothing if measurement failed.</returns>
            Public Function MeasureGatedSectionLevel(ByVal InputSound As Sound, ByVal channel As Integer,
                         Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer = Nothing,
                                        Optional ByVal outputUnit As SoundDataUnit = SoundDataUnit.dB,
                                                   Optional ByVal GatingWindowDuration As Decimal = 0.01,
                                                 Optional ByVal GateRelativeThreshold As Double = -10,
                                                 Optional ByVal ProportionForCalculatingAbsThreshold As Decimal = 0.25,
                                                  Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z) As Double?

                Try

                    Dim AllWindowsList As New List(Of Double)
                    Dim IncludedWindowsList As New List(Of Double)

                    Dim tempSound As Sound
                    If Not FrequencyWeighting = FrequencyWeightings.Z Then
                        'Copying the input sound to a new temporary sound
                        tempSound = InputSound.CreateCopy
                    Else
                        'Referencing the input array since it will not be alterred
                        tempSound = InputSound
                    End If

                    CheckAndCorrectSectionLength(tempSound.WaveData.SampleData(channel).Length, startSample, sectionLength)

                    'Converting Gating window duration to length in samples
                    Dim windowLength As Integer = TimeUnitConversion(GatingWindowDuration, TimeUnitConversionDirection.secondsToSamples, tempSound.WaveFormat.SampleRate)

                    'Filterring for Weighting
                    If Not FrequencyWeighting = FrequencyWeightings.Z Then
                        tempSound = DSP.IIRFilter(tempSound, FrequencyWeighting, channel)
                        If tempSound Is Nothing Then Return Nothing 'Aborting and return Nothingl if something went wrong here
                    End If

                    'Calculating the number of measuring windows (N.B. This ignores the last window, which will be of different length and therefore could not be included in the calculation of average RMS below)
                    Dim windowCount As Integer = Utils.Rounding((sectionLength / windowLength), Utils.roundingMethods.alwaysDown)

                    If windowCount < 1 Then Return Nothing 'Aborting and returning Nothing if no measurement could be made due to too low number of measurement windows

                    'Measuring and storing the RMS level of each measurement window
                    For windowNumber = 0 To windowCount - 1
                        AllWindowsList.Add(MeasureSectionLevel(tempSound, channel, startSample + windowNumber * windowLength, windowLength, SoundDataUnit.linear, SoundMeasurementType.RMS))
                    Next

                    'Calculating the Absolute Threshold Level (using the FractionForCalculatingAbsThreshold loudest windows as reference to the relative threshold)
                    AllWindowsList.Sort() 'Sorting the list of window sound levels
                    Dim sum1 As Double = 0 'Averaging the top FractionForCalculatingAbsThreshold
                    Dim summedValues1 As Integer = 0
                    For window = Int((1 - ProportionForCalculatingAbsThreshold) * AllWindowsList.Count) To AllWindowsList.Count - 1
                        sum1 += AllWindowsList(window)
                        summedValues1 += 1
                    Next
                    Dim absoluteThresholdLevel As Double = dBConversion((sum1 / summedValues1), dBConversionDirection.to_dB, tempSound.WaveFormat) + GateRelativeThreshold
                    Dim linearAbsoluteThreshold As Double = dBConversion((absoluteThresholdLevel), dBConversionDirection.from_dB, tempSound.WaveFormat)

                    'Selecting the windows levels that are equal to or louder than the absolute threshold level (expressed linearly)
                    For window = 0 To AllWindowsList.Count - 1
                        If AllWindowsList(window) >= linearAbsoluteThreshold Then IncludedWindowsList.Add(AllWindowsList(window))
                    Next

                    'Returns if no windows was included
                    If IncludedWindowsList.Count < 1 Then Return Nothing

                    Select Case outputUnit
                        Case SoundDataUnit.dB

                            'Returning the average RMS Level of the selected levels
                            Return dBConversion((IncludedWindowsList.Average()), dBConversionDirection.to_dB, tempSound.WaveFormat)

                        Case SoundDataUnit.linear

                            'Returning the average linear RMS of the selected levels
                            Return IncludedWindowsList.Average()
                        Case Else
                            Return Nothing

                    End Select

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try


            End Function


            ''' <summary>
            ''' Finds and returns the average level of the loudest section (with the duration of TemporalIntegrationDuration) of the sound file.
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="channel"></param>
            ''' <param name="startSample">The sample where the measurement should start.</param>
            ''' <param name="sectionLength">The length in sample where the measurement should be performed (NB Any incomplete final temporalIntegrationDuration long section will not be included in the measurement.)</param>
            ''' <param name="outputUnit">The unit of the output value.</param>
            ''' <param name="WindowDistance">An optional parameter that can be used to attain the number of samples between each measurement window. It can be use to get the start sample 
            '''<param name="Overlap">This parameter adjusts the amount of overlap. The actual overlap length is (1 / Overlap) * TemporalIntegrationDuration. The TemporalIntegrationDuration is set in the SMA.SoundLevelFormat of the current sound.</param>
            ''' <param name="LoudestWindowStartSample">Upon return, contains the start sample of the loudest window, or -1 if no measurement data could be found.</param>
            ''' of the sound RMS values stored in WholeWindowsList (by multiplying the appropriate list index by WindowDistance).
            ''' (The actual window length is 10*WindowDistance)</param>
            ''' <returns>Returns the average level of the loudest section (with the duration of TemporalIntegrationDuration) of the sound file</returns>
            Public Function MeasureTimeAndFrequencyWeightedSectionLevel(ByVal InputSound As Sound, ByVal Channel As Integer,
                                                                    Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing,
                                                                    Optional ByVal OutputUnit As SoundDataUnit = SoundDataUnit.dB,
                                                                    Optional ByRef WindowDistance As Integer = 0,
                                                                    Optional ByRef Overlap As Integer = 10,
                                                                    Optional ByRef LoudestWindowStartSample As Integer = -1,
                                                                    Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.C,
                                                                    Optional ByVal TimeWeighting As Double = 0.1) As Double? ',

                'NB! Prior to 2017-05-31 this function was identical to MeasureTimeAndFrequencyWeightedSectionLevel_QuarterOverlap, which contained some disadvantages, 
                ' in that it measured average RMS of quarters of the TemporalIntegrationDuration (which was set to 100 ms). This improved version measures real RMS over the TemporalIntegrationDuration, and 
                ' allowes the user to set the amount of overlap.

                Try

                    'Resetting LoudestWindowStartSample (needed only if the user has input something else there)
                    LoudestWindowStartSample = -1

                    Dim WholeWindowsList As List(Of Double) = New List(Of Double)
                    Dim SectionWindowsList As List(Of Double) = New List(Of Double)

                    'Limits the section length, if it goes outside the sound array
                    CheckAndCorrectSectionLength(InputSound.WaveData.SampleData(Channel).Length, StartSample, SectionLength)

                    Dim tempSound As Sound

                    If FrequencyWeighting <> FrequencyWeightings.Z Then

                        'Preparing a new sound with only the measurement section, to be filterred
                        tempSound = New Sound(InputSound.WaveFormat)
                        Dim SoundArray(SectionLength - 1) As Single
                        For s = StartSample To StartSample + SectionLength - 1
                            SoundArray(s - StartSample) = InputSound.WaveData.SampleData(Channel)(s)
                        Next
                        tempSound.WaveData.SampleData(Channel) = SoundArray

                        'Filterring for Weighing
                        tempSound = DSP.IIRFilter(tempSound, FrequencyWeighting, Channel)
                        If tempSound Is Nothing Then
                            Throw New Exception("Something went wrong during IIR-filterring")
                            Return Nothing 'Aborting and return Nothing if something went wrong here
                        End If

                        StartSample = 0
                    Else
                        tempSound = InputSound
                    End If


                    'Converting Gating window duration to length in samples. The RMS-level is only measured once. 1/overlap overlapping of windows is made by splitting the full window length in four, and then averaging the measured level over overlap number of windows at a time.
                    Dim SectionWindowLength As Integer = TimeUnitConversion(TimeWeighting, TimeUnitConversionDirection.secondsToSamples, tempSound.WaveFormat.SampleRate) / Overlap
                    WindowDistance = SectionWindowLength 'Storing the WindowDistance used
                    Dim fullWindowLength As Integer = SectionWindowLength * Overlap

                    'Calculating the number of measuring windows (N.B. This count also includes any incomplete window at the end of the section. Any such window is zero-padded below)
                    Dim FullWindowCount As Integer = Utils.Rounding((SectionLength / fullWindowLength), Utils.roundingMethods.alwaysUp)
                    Dim SectionWindowCount As Integer = FullWindowCount * Overlap


                    'Creating a new sound for measurement of each window
                    Dim WindowSound As New Sound(tempSound.WaveFormat)
                    Dim WindowSoundArray(SectionWindowLength - 1) As Single
                    WindowSound.WaveData.SampleData(Channel) = WindowSoundArray

                    'Measuring and storing the RMS level of each 1/overlap section of the full window length
                    For windowNumber = 0 To SectionWindowCount - 1
                        'Copying the measurement window to the new sound
                        Dim CurrentStartSample As Integer = StartSample + windowNumber * SectionWindowLength

                        For s = CurrentStartSample To CurrentStartSample + SectionWindowLength - 1
                            'Checks if zero-padding is needed
                            If s < StartSample + SectionLength Then
                                'Adding the sample value
                                WindowSoundArray(s - CurrentStartSample) = tempSound.WaveData.SampleData(Channel)(s)
                            Else
                                'We're outside the measurement section. Performs zero-padding. (This will actually replace any existing sound outside the section with zeroes! So even if there is sound at the specified location, this will not be measured.)
                                WindowSoundArray(s - CurrentStartSample) = 0
                            End If
                        Next

                        'Measuring the (frequency weighted, since this is already done to the tempsound above) mean-quare-level of the 1/overlap section window
                        SectionWindowsList.Add(MeasureSectionLevel(WindowSound, Channel, , SectionWindowLength, SoundDataUnit.linear,
                                                                    SoundMeasurementType.RMS,
                                                                FrequencyWeightings.Z,
                                                               True)) ' FrequencyWeighting))
                    Next

                    'Averaging the mean-squares of the 1/overlap section windows to whole window values
                    For SectionWindow = 0 To SectionWindowsList.Count - Overlap
                        Dim SectionWindowSum As Double = 0
                        For window = 0 To (Overlap - 1)

                            'Adding data within the sound
                            SectionWindowSum += SectionWindowsList(SectionWindow + window)

                        Next
                        WholeWindowsList.Add(SectionWindowSum / Overlap)
                    Next

                    'Converting mean-squares in the WholeWindowsList to RMS, by taking the root of each value
                    For w = 0 To WholeWindowsList.Count - 1
                        WholeWindowsList(w) = WholeWindowsList(w) ^ (1 / 2)
                    Next

                    'Finding the loudest whole window
                    Dim LoudestWindowLevel As Double = WholeWindowsList.Max()
                    If WholeWindowsList.Count > 0 Then LoudestWindowStartSample = SectionWindowLength * (WholeWindowsList.IndexOf(LoudestWindowLevel))

                    'Returning the in the appropriate output unit
                    Select Case OutputUnit
                        Case SoundDataUnit.dB

                            'Returning the average RMS Level of the selected levels
                            Return dBConversion(LoudestWindowLevel, dBConversionDirection.to_dB, tempSound.WaveFormat)

                        Case SoundDataUnit.linear

                            'Returning the average linear RMS of the selected levels
                            Return LoudestWindowLevel

                        Case Else
                            Throw New NotImplementedException("Unsupported output unit.")
                    End Select

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Throw New Exception
                End Try


            End Function

            ''' <summary>
            ''' Finds and returns the average level of the loudest section (with the duration of TemporalIntegrationDuration) of the sound file.
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="channel"></param>
            ''' <param name="startSample">The sample where the measurement should start.</param>
            ''' <param name="sectionLength">The length in sample where the measurement should be performed (NB Any incomplete final temporalIntegrationDuration long section will not be included in the measurement.)</param>
            ''' <param name="outputUnit">The unit of the output value.</param>
            ''' <param name="WholeWindowsList">An optional parameter that can be used to attain the values of internally created list of RMS-averages (linear scale) for the measurement windows.</param>
            ''' <param name="QuarterWindowsList">An optional parameter that can be used to attain the values of internally created list of RMS-averages (linear scale) for the quarters of the measurement windows.</param>
            ''' <param name="WindowDistance">An optional parameter that can be used to attain the number of samples between each measurement window. It can be use to get the start sample 
            ''' of the sound RMS values stored in WholeWindowsList (by multiplying the appropriate list index by WindowDistance).
            ''' (The actual window length is 4*WindowDistance)</param>
            ''' <returns>Returns the average level of the loudest section (with the duration of TemporalIntegrationDuration) of the sound file</returns>
            Public Function MeasureTimeAndFrequencyWeightedSectionLevel_QuarterOverlap(ByVal InputSound As Sound, ByVal Channel As Integer,
                                                                    Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing,
                                                                    Optional ByVal OutputUnit As SoundDataUnit = SoundDataUnit.dB,
                                                                    Optional ByRef WholeWindowsList As List(Of Double) = Nothing,
                                                                    Optional ByRef QuarterWindowsList As List(Of Double) = Nothing,
                                                                    Optional ByRef WindowDistance As Integer = 0,
                                                                                   Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.C,
                                                                    Optional ByVal TimeWeighting As Double = 0.1) As Double? ',

                Try

                    If QuarterWindowsList Is Nothing Then QuarterWindowsList = New List(Of Double)
                    If WholeWindowsList Is Nothing Then WholeWindowsList = New List(Of Double)

                    'Limits the section length, if it goes outside the sound array
                    CheckAndCorrectSectionLength(InputSound.WaveData.SampleData(Channel).Length, StartSample, SectionLength)

                    Dim tempSound As Sound

                    If FrequencyWeighting <> FrequencyWeightings.Z Then

                        'Preparing a new sound with only the measurement section, to be filterred
                        tempSound = New Sound(InputSound.WaveFormat)
                        Dim SoundArray(SectionLength - 1) As Single
                        For s = StartSample To StartSample + SectionLength - 1
                            SoundArray(s - StartSample) = InputSound.WaveData.SampleData(Channel)(s)
                        Next
                        tempSound.WaveData.SampleData(Channel) = SoundArray

                        'Filterring for Weighing
                        tempSound = DSP.IIRFilter(tempSound, FrequencyWeighting, Channel)
                        If tempSound Is Nothing Then
                            Throw New Exception("Something went wrong during IIR-filterring")
                            Return Nothing 'Aborting and return Nothing if something went wrong here
                        End If

                        StartSample = 0
                    Else
                        tempSound = InputSound
                    End If


                    'Converting Gating window duration to length in samples. The RMS-level is only measured once. 25 % Overlapping of windows is made by splitting the full window length in four, and then averaging the measured level over four windows at a time.
                    Dim quarterWindowLength As Integer = TimeUnitConversion(TimeWeighting, TimeUnitConversionDirection.secondsToSamples, tempSound.WaveFormat.SampleRate) / 4
                    WindowDistance = quarterWindowLength 'Storing the WindowDistance used
                    Dim fullWindowLength As Integer = quarterWindowLength * 4

                    'Calculating the number of measuring windows (N.B. This count also includes any incomplete window at the end of the section. Any such window is zero-padded below)
                    Dim FullWindowCount As Integer = Utils.Rounding((SectionLength / fullWindowLength), Utils.roundingMethods.alwaysUp)
                    Dim QuarterWindowCount As Integer = FullWindowCount * 4


                    'Creating a new sound for measurement of each window
                    Dim WindowSound As New Sound(tempSound.WaveFormat)
                    Dim WindowSoundArray(quarterWindowLength - 1) As Single
                    WindowSound.WaveData.SampleData(Channel) = WindowSoundArray

                    'Measuring and storing the RMS level of each quarter of the full window length
                    For windowNumber = 0 To QuarterWindowCount - 1
                        'Copying the measurement window to the new sound
                        Dim CurrentStartSample As Integer = StartSample + windowNumber * quarterWindowLength

                        For s = CurrentStartSample To CurrentStartSample + quarterWindowLength - 1
                            'Checks if zero-padding is needed
                            If s < StartSample + SectionLength Then
                                'Adding the sample value
                                WindowSoundArray(s - CurrentStartSample) = tempSound.WaveData.SampleData(Channel)(s)
                            Else
                                'We're outside the measurement section. Performs zero-padding. (This will actually replace any existing sound outside the section with zeroes! So even if there is sound at the specified location, this will not be measured.)
                                WindowSoundArray(s - CurrentStartSample) = 0
                            End If
                        Next

                        'Measuring the (frequency weighted) RMS-level of the quarter window
                        QuarterWindowsList.Add(MeasureSectionLevel(WindowSound, Channel, , quarterWindowLength, SoundDataUnit.linear,
                                                                    SoundMeasurementType.RMS, FrequencyWeightings.Z)) ' FrequencyWeighting))
                    Next

                    'Averaging quarter windows to whole window values
                    For quarterWindow = 0 To QuarterWindowsList.Count - 4
                        Dim quarterWindowSum As Double = 0
                        For window = 0 To 3
                            quarterWindowSum += QuarterWindowsList(quarterWindow + window)
                        Next
                        WholeWindowsList.Add(quarterWindowSum / 4)
                    Next

                    'Finding the loudest whole window
                    Dim LoudestWindowLevel As Double = WholeWindowsList.Max()

                    'Returning the in the appropriate output unit
                    Select Case OutputUnit
                        Case SoundDataUnit.dB

                            'Returning the average RMS Level of the selected levels
                            Return dBConversion(LoudestWindowLevel, dBConversionDirection.to_dB, tempSound.WaveFormat)

                        Case SoundDataUnit.linear

                            'Returning the average linear RMS of the selected levels
                            Return LoudestWindowLevel

                        Case Else
                            Throw New NotImplementedException("Unsupported output unit.")
                    End Select

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Throw New Exception
                End Try


            End Function


            ''' <summary>
            ''' Checks the indicated sound for distorsion (defined as samples with a magnitude above full scale), and returns the number of distorted samples (in all channels together). 
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="channel"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength"></param>
            ''' <returns>Returns the number of distorted samples (in all channels together), or vbNull if an error occurred.</returns>
            Public Function CheckForDistorsion(ByRef InputSound As Sound, Optional ByVal channel As Integer? = Nothing,
                         Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing) As Double

                Try

                    Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, channel)

                    Dim distorsion As Double = 0

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        'Main section
                        Dim SoundArray = InputSound.WaveData.SampleData(c)
                        Dim CorrectedStartSample = startSample
                        Dim CorrectedSectionLength = sectionLength
                        Dim PositiveFullScale = InputSound.WaveFormat.PositiveFullScale
                        CheckAndCorrectSectionLength(SoundArray.Length, CorrectedStartSample, CorrectedSectionLength)

                        For sample = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                            If SoundArray(sample) > PositiveFullScale Then distorsion += 1
                        Next
                    Next

                    Return distorsion

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return vbNull
                End Try

            End Function

            Public Enum SearchDirections
                Later
                Earlier
                Closest
            End Enum

            ''' <summary>
            ''' Looks up the closest sample on which a zero crossing occurs. If there is no sample exactly on the zerocrossing, the sample index with the lowest sample value of the two samples closest to the zero crossing is returned.
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="channel"></param>
            ''' <param name="InputSample"></param>
            ''' <param name="SearchDirection">The direction in which the closest zero crossing sample should be detected. If Closest is selected, a search is done both forward and backwards from the InputSample, and the zero crossing clostest to the InputSample is returned.</param>
            ''' <returns></returns>
            Public Function GetZeroCrossingSample(ByRef InputSound As Sound, ByVal Channel As Integer, ByVal InputSample As Integer, Optional SearchDirection As SearchDirections = SearchDirections.Closest) As Integer

                'Checks if the InputSample is outside the sound array, in such cases, 0 or the index of the last sample is returned.
                If InputSample < 0 Then Return 0
                If InputSample > InputSound.WaveData.SampleData(Channel).Length - 1 Then Return InputSound.WaveData.SampleData(Channel).Length - 1

                'Checks if the input sample is zero
                If InputSound.WaveData.SampleData(Channel)(InputSample) = 0 Then Return InputSample

                Select Case SearchDirection
                    Case SearchDirections.Earlier
                        'looking earlier in the sound for a zero crossing
                        Dim CurrentSample As Integer = 0
                        For inverse_Sample = 0 To InputSample - 1
                            CurrentSample = InputSample - 1 - inverse_Sample

                            'Checking if the current sample is zero
                            If InputSound.WaveData.SampleData(Channel)(CurrentSample) = 0 Then Return CurrentSample

                            'Checking if the signs have changed between the current and the following sample
                            If Math.Sign(InputSound.WaveData.SampleData(Channel)(CurrentSample)) <>
                            Math.Sign(InputSound.WaveData.SampleData(Channel)(CurrentSample + 1)) Then

                                'Determining which of the two samples closest to the zero crossing that are closest to zero and returns it.
                                If Math.Abs(InputSound.WaveData.SampleData(Channel)(CurrentSample)) < Math.Abs(InputSound.WaveData.SampleData(Channel)(CurrentSample + 1)) Then
                                    Return CurrentSample
                                Else
                                    Return CurrentSample + 1
                                End If
                            End If
                        Next

                        'Returns 0 if no zero crossing was found
                        Return 0

                    Case SearchDirections.Later
                        'looking later in the sound for a zero crossing
                        For CurrentSample = InputSample + 1 To InputSound.WaveData.SampleData(Channel).Length - 1

                            'Checking if the current sample is zero
                            If InputSound.WaveData.SampleData(Channel)(CurrentSample) = 0 Then Return CurrentSample

                            'Checking if the signs have changed between the current and the previuos sample
                            If Math.Sign(InputSound.WaveData.SampleData(Channel)(CurrentSample - 1)) <>
                        Math.Sign(InputSound.WaveData.SampleData(Channel)(CurrentSample)) Then

                                'Determining which of the two samples closest to the zero crossing that are closest to zero and returns it.
                                If Math.Abs(InputSound.WaveData.SampleData(Channel)(CurrentSample)) < Math.Abs(InputSound.WaveData.SampleData(Channel)(CurrentSample - 1)) Then
                                    Return CurrentSample
                                Else
                                    Return CurrentSample - 1
                                End If
                            End If

                        Next

                        'Returns the last sample index if no zero crossing was found
                        Return InputSound.WaveData.SampleData(Channel).Length - 1

                    Case SearchDirections.Closest

                        Dim EarlierClostestSample As Integer = 0
                        Dim LaterClostestSample As Integer = 0
                        Dim ZeroCrossingDetected As Boolean = False

                        'looking earlier in the sound for a zero crossing
                        Dim CurrentSample1 As Integer = 0
                        For inverse_Sample = 0 To InputSample - 1
                            CurrentSample1 = InputSample - 1 - inverse_Sample

                            'Checking if the current sample is zero
                            If InputSound.WaveData.SampleData(Channel)(CurrentSample1) = 0 Then
                                EarlierClostestSample = CurrentSample1
                                ZeroCrossingDetected = True
                                Exit For
                            End If

                            'Checking if the signs have changed between the current and the following sample
                            If Math.Sign(InputSound.WaveData.SampleData(Channel)(CurrentSample1)) <>
                            Math.Sign(InputSound.WaveData.SampleData(Channel)(CurrentSample1 + 1)) Then

                                'Determining which of the two samples closest to the zero crossing that are closest to zero and returns it.
                                If Math.Abs(InputSound.WaveData.SampleData(Channel)(CurrentSample1)) < Math.Abs(InputSound.WaveData.SampleData(Channel)(CurrentSample1 + 1)) Then
                                    EarlierClostestSample = CurrentSample1
                                Else
                                    EarlierClostestSample = CurrentSample1 + 1
                                End If

                                ZeroCrossingDetected = True
                                Exit For
                            End If
                        Next

                        'looking later in the sound for a zero crossing
                        For CurrentSample2 = InputSample + 1 To InputSound.WaveData.SampleData(Channel).Length - 1

                            'Checking if the current sample is zero
                            If InputSound.WaveData.SampleData(Channel)(CurrentSample2) = 0 Then
                                LaterClostestSample = CurrentSample2
                                ZeroCrossingDetected = True
                                Exit For
                            End If

                            'Checking if the signs have changed between the current and the previuos sample
                            If Math.Sign(InputSound.WaveData.SampleData(Channel)(CurrentSample2 - 1)) <>
                        Math.Sign(InputSound.WaveData.SampleData(Channel)(CurrentSample2)) Then

                                'Determining which of the two samples closest to the zero crossing that are closest to zero and returns it.
                                If Math.Abs(InputSound.WaveData.SampleData(Channel)(CurrentSample2)) < Math.Abs(InputSound.WaveData.SampleData(Channel)(CurrentSample2 - 1)) Then
                                    LaterClostestSample = CurrentSample2
                                Else
                                    LaterClostestSample = CurrentSample2 - 1
                                End If

                                ZeroCrossingDetected = True
                                Exit For
                            End If
                        Next

                        'Returns 0 if no zero crossing was found
                        If ZeroCrossingDetected = False Then Return 0

                        'Looks for the closest zero crossing
                        If (LaterClostestSample - InputSample) < (InputSample - EarlierClostestSample) Then
                            'Returns the sample index of the later clostest zero crossing sample.
                            Return LaterClostestSample
                        Else
                            'Returns the sample index of the earlier zero crossing index. N.B. This is the default if both distances are equal!
                            Return EarlierClostestSample
                        End If

                    Case Else
                        Throw New NotImplementedException
                End Select

            End Function

            Public Enum ReportedAudibilityTypes
                MaximumAudibility
                MostAudibleWindow
                MostAudibleWindows
                AverageAudibility
                SummedAudibility
                LimitedAudibilitySum
            End Enum

            Public Enum AudibilityDistanceTypes
                LevelsAboveNoise
                Indexation 'SII type
            End Enum

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="InputSpeech">The speech signal.</param>
            ''' <param name="InputNoise">The noise signal. Must be at least as long as the speech signal.</param>
            ''' <param name="MeasurementWindowDuration"></param>
            ''' <param name="MeasurementWindowOverlapDuration"></param>
            ''' <param name="BarkFilterOverlapRatio"></param>
            ''' <param name="LowestIncludedCentreFrequency"></param>
            ''' <param name="HighestIncludedCentreFrequency"></param>
            ''' <param name="MatrixOutputFolder"></param>
            ''' <param name="ExportDetails"></param>
            ''' <param name="InactivateFftWarnings"></param>
            ''' <returns></returns>
            Public Function GetSpeechAudibility(ByRef InputSpeech As Sound, ByRef InputNoise As Sound,
                                            Optional ByVal MeasurementWindowDuration As Double = 0.1,
                                            Optional ByVal MeasurementWindowOverlapDuration As Double = 0.095,
                                            Optional ByVal BarkFilterOverlapRatio As Double = 0.9,
                                            Optional ByRef LowestIncludedCentreFrequency As Double = 80,
                                            Optional ByRef HighestIncludedCentreFrequency As Double = 17500,
                                            Optional ByVal MatrixOutputFolder As String = "",
                                            Optional ByVal ExportDetails As Boolean = False,
                                            Optional ByRef InactivateFftWarnings As Boolean = False,
                                            Optional ByRef AudibilityDistanceType As AudibilityDistanceTypes = AudibilityDistanceTypes.LevelsAboveNoise,
                                            Optional ByRef ReportedAudibilityType As ReportedAudibilityTypes = ReportedAudibilityTypes.MostAudibleWindow) As Double

                'Creating copies of the input sounds (to prevent modifying the input sounds)
                Dim Speech As Sound = InputSpeech.CreateCopy
                Dim Noise As Sound = InputNoise.CreateCopy

                'Checking that both sounds are mono
                If Speech.WaveFormat.Channels <> 1 Or Noise.WaveFormat.Channels <> 1 Then Throw New NotImplementedException("Unsupported channel count.")

                'Checking that the sounds have the same format
                If Speech.WaveFormat.BitDepth <> Noise.WaveFormat.BitDepth Or
            Speech.WaveFormat.SampleRate <> Noise.WaveFormat.SampleRate Or
            Speech.WaveFormat.Encoding <> Noise.WaveFormat.Encoding Then Throw New Exception("Different formats in input sounds.")

                If Noise.WaveData.ShortestChannelSampleCount < Speech.WaveData.ShortestChannelSampleCount Then Throw New Exception("Noise is shorter that the speech signal. It must be at least as long as the speech signal.")

                'Setting up FFT formats
                Dim MeasurementWindowLength As Integer = Speech.WaveFormat.SampleRate * MeasurementWindowDuration
                If MeasurementWindowLength Mod 2 = 1 Then MeasurementWindowLength += 1

                Dim MeasurementWindowOverlapLength As Integer = Speech.WaveFormat.SampleRate * MeasurementWindowOverlapDuration
                If MeasurementWindowOverlapLength Mod 2 = 1 Then MeasurementWindowOverlapLength += 1
                Dim SpectralResolution As Integer = 2048 * 2
                Dim AD_FftFormat As New Formats.FftFormat(MeasurementWindowLength, SpectralResolution, MeasurementWindowOverlapLength, WindowingType.Hamming, InactivateFftWarnings)

                'Cutting the noise to be the same lengt as the speech
                If Noise.WaveData.ShortestChannelSampleCount > Speech.WaveData.ShortestChannelSampleCount Then
                    CropSection(Noise, 0, Speech.WaveData.ShortestChannelSampleCount)
                    'MsgBox(Noise.WaveData.ShortestChannelSampleCount & " " & Speech.WaveData.ShortestChannelSampleCount & "Check A: These should be the same. Remove check if working!")
                End If

                'Zeropadding the input sounds, so that they get MeasurementWindowOverlapLength before the sound. After the sound, the spectral analysis function adds zero padding automatically
                Speech.ZeroPad(MeasurementWindowOverlapLength, Nothing, False)
                Noise.ZeroPad(MeasurementWindowOverlapLength, Nothing, False)

                'Just checking again that the lengths are the same (TODO: remove this check when working fine)
                If Noise.WaveData.ShortestChannelSampleCount > Speech.WaveData.ShortestChannelSampleCount Then
                    'CropSection(Noise, Speech.WaveData.ShortestChannelSampleCount, -1)
                    'MsgBox(Noise.WaveData.ShortestChannelSampleCount & " " & Speech.WaveData.ShortestChannelSampleCount & "Check B: These should be the same. Remove check if working!")
                End If

                'Getting frequency domain data
                Speech.FFT = SpectralAnalysis(Speech, AD_FftFormat)
                Noise.FFT = SpectralAnalysis(Noise, AD_FftFormat)

                'Calculating audibility
                Dim Audibility = CalculateAudibility(Speech.FFT, Noise.FFT, BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency,
                                                       Speech.WaveFormat.SampleRate, MatrixOutputFolder, Speech.FileName & "_" & Noise.FileName, ExportDetails,
                                                 Speech.WaveFormat, MeasurementWindowOverlapLength, MeasurementWindowLength, AudibilityDistanceType, ReportedAudibilityType)


                Return Audibility


            End Function

            Private Function CalculateAudibility(ByRef FftData1 As FftData,
                                             ByRef FftData2 As FftData,
                                             ByVal BarkFilterOverlapRatio As Double,
                                             ByRef LowestIncludedCentreFrequency As Double,
                                             ByRef HighestIncludedCentreFrequency As Double,
                                             ByVal SampleRate As Integer,
                                             ByVal MatrixOutputFolder As String,
                                             ByVal FileComparisonID As String,
                                             ByVal ExportDetails As Boolean,
                                             ByVal CurrentWaveFormat As Formats.WaveFormat,
                                             ByVal MeasurementWindowOverlapLength As Integer,
                                             ByVal MeasurementWindowLength As Integer,
                                             ByVal AudibilityDistanceType As AudibilityDistanceTypes,
                                             ByVal ReportedAudibilityType As ReportedAudibilityTypes) As Double

                'Calculating amplitude arrays
                FftData1.CalculateAmplitudeSpectrum()
                FftData2.CalculateAmplitudeSpectrum()

                'Splitting the magnitude values in different bark filters (critical band widths)
                Dim CentreFrequencies As SortedSet(Of Single) = Nothing
                Throw New NotImplementedException("The Bark filters on next lines are commented out since they are not updated.")
                Dim FilterredMagnitudesArray_Speech As SortedList(Of Integer, Single()) '= BarkFilter(FftData1, SampleRate, BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency, CentreFrequencies)
                Dim FilterredMagnitudesArray_Noise As SortedList(Of Integer, Single()) '= BarkFilter(FftData2, SampleRate, BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency, CentreFrequencies)

                If ExportDetails = True And MatrixOutputFolder <> "" Then
                    'Exports centre frequencies instead of magnitudes
                    Dim newDoubleArray(1, CentreFrequencies.Count - 1) As Double
                    For p = 0 To CentreFrequencies.Count - 1
                        newDoubleArray(0, p) = CentreFrequencies(p)
                        newDoubleArray(1, p) = Utils.CenterFrequencyToBarkFilterBandwidth(CentreFrequencies(p))
                    Next
                    Utils.SaveMatrixToFile(newDoubleArray, IO.Path.Combine(MatrixOutputFolder, "CentreFrequencies.txt"))
                End If

                'Transforming the filterred magnitude arrays to decibels (+ Single.Epsilon to avoid -infinity of silent sound sections)
                For Each Window In FilterredMagnitudesArray_Speech
                    For n = 0 To Window.Value.Count - 1
                        FilterredMagnitudesArray_Speech(Window.Key)(n) = dBConversion(FilterredMagnitudesArray_Speech(Window.Key)(n) + Single.Epsilon, dBConversionDirection.to_dB, CurrentWaveFormat)
                        'FilterredMagnitudesArray1(Window.Key)(n) = Math.Log10(FilterredMagnitudesArray1(Window.Key)(n) + Single.Epsilon) 'Single.Epsilon repressents silence/ the smallest possible positive value of Single
                    Next
                Next
                For Each Window In FilterredMagnitudesArray_Noise
                    For n = 0 To Window.Value.Count - 1
                        FilterredMagnitudesArray_Noise(Window.Key)(n) = dBConversion(FilterredMagnitudesArray_Noise(Window.Key)(n) + Single.Epsilon, dBConversionDirection.to_dB, CurrentWaveFormat)
                        'FilterredMagnitudesArray2(Window.Key)(n) = Math.Log10(FilterredMagnitudesArray2(Window.Key)(n) + Single.Epsilon)
                    Next
                Next

                'Exporting data for manual checking/display
                If ExportDetails = True And MatrixOutputFolder <> "" Then
                    Dim newDoubleArray(FilterredMagnitudesArray_Speech.Count - 1, FilterredMagnitudesArray_Speech(0).Length - 1) As Double
                    For p = 0 To FilterredMagnitudesArray_Speech.Count - 1
                        For q = 0 To FilterredMagnitudesArray_Speech(0).Length - 1
                            newDoubleArray(p, q) = FilterredMagnitudesArray_Speech(p)(q)
                        Next
                    Next
                    Utils.SaveMatrixToFile(newDoubleArray, IO.Path.Combine(MatrixOutputFolder, "Spectrum_" & FileComparisonID & "Speech.txt"))

                    Dim newDoubleArray2(FilterredMagnitudesArray_Noise.Count - 1, FilterredMagnitudesArray_Noise(0).Length - 1) As Double
                    For p = 0 To FilterredMagnitudesArray_Noise.Count - 1
                        For q = 0 To FilterredMagnitudesArray_Noise(0).Length - 1
                            newDoubleArray2(p, q) = FilterredMagnitudesArray_Noise(p)(q)
                        Next
                    Next
                    Utils.SaveMatrixToFile(newDoubleArray2, IO.Path.Combine(MatrixOutputFolder, "Spectrum_" & FileComparisonID & "Noise.txt"))

                End If

                'Getting the audibility
                'Looking at each time window at a time
                'Comparing each time window in the speech sound with the equivalent window in the noise sound (actually, averaging the spectrum of the noise may be better)
                Dim TimeWindowAudibilityList As New List(Of Double)
                Dim TimeWindowDetailedLIst As New List(Of Double())
                'Dim DifferenceType As Integer = 0

                Select Case AudibilityDistanceType
                    Case AudibilityDistanceTypes.LevelsAboveNoise 'Determining speech levels above noise
                        For timeWindow = 0 To FilterredMagnitudesArray_Speech.Count - 1

                            'Subtracting the arrays
                            Dim CurrentTimeWindowAudibilityArray(FilterredMagnitudesArray_Speech(timeWindow).Length - 1) As Double
                            For k = 0 To CurrentTimeWindowAudibilityArray.Length - 1

                                Dim CurrentDistance As Double = FilterredMagnitudesArray_Speech(timeWindow)(k) - FilterredMagnitudesArray_Noise(timeWindow)(k)
                                CurrentTimeWindowAudibilityArray(k) = Math.Max(CurrentDistance, 0)
                            Next

                            'Adding the average audability within the specific time window
                            TimeWindowAudibilityList.Add(CurrentTimeWindowAudibilityArray.Average)

                            'Also adding the detailed list (mostly for export purpose)
                            TimeWindowDetailedLIst.Add(CurrentTimeWindowAudibilityArray)

                        Next

                    Case AudibilityDistanceTypes.Indexation 'SII-type indexation

                        Dim BandImportanceFactor As Double = 1 / CentreFrequencies.Count 'Equal band importance, is determined by taking 1 divided by the number of bark filters.

                        For timeWindow = 0 To FilterredMagnitudesArray_Speech.Count - 1

                            'Subtracting the arrays
                            Dim CurrentTimeWindowAudibilityArray(FilterredMagnitudesArray_Speech(timeWindow).Length - 1) As Double
                            For k = 0 To CurrentTimeWindowAudibilityArray.Length - 1

                                'Dim CurrentDistance As Double = FilterredMagnitudesArray_Speech(timeWindow)(k) - FilterredMagnitudesArray_Noise(timeWindow)(k)
                                Dim CurrentDistance As Double = Math.Max(0, Math.Min(1, (FilterredMagnitudesArray_Speech(timeWindow)(k) - FilterredMagnitudesArray_Noise(timeWindow)(k) + 15) / 30))
                                CurrentTimeWindowAudibilityArray(k) = BandImportanceFactor * CurrentDistance '
                            Next

                            'Adding the average audability within the specific time window
                            TimeWindowAudibilityList.Add(CurrentTimeWindowAudibilityArray.Sum)

                            'Also adding the detailed list (mostly for export purpose)
                            TimeWindowDetailedLIst.Add(CurrentTimeWindowAudibilityArray)

                        Next

                    Case Else
                        Throw New NotImplementedException
                End Select

                'Saving matrix to file
                If ExportDetails = True And MatrixOutputFolder <> "" Then
                    Dim ExportAudibilityMatrix(TimeWindowAudibilityList.Count,
                                     FilterredMagnitudesArray_Speech(0).Length - 1) As Double

                    'Adding the data into the ExportAudibilityMatrix
                    For p = 0 To TimeWindowDetailedLIst.Count - 1
                        For q = 0 To TimeWindowDetailedLIst(0).Length - 1
                            ExportAudibilityMatrix(p, q) = TimeWindowDetailedLIst(p)(q)
                        Next
                    Next

                    Utils.SaveMatrixToFile(ExportAudibilityMatrix, IO.Path.Combine(MatrixOutputFolder, FileComparisonID & "_Audability.txt"))
                End If

                'Förslag
                '1 Ta bort/ eller fejda tidsfönstren i zero-pad regionerna, så att dessa inte får repressentera skillnaden. Detta minskar bland annat inverkan från omkringliggande fonem, och risken att en obetydande del av ljudet väljs.
                '2 Lägg in ett tröskelvärde för hörbart ljud i brus. Nu är det på 0 (dB?), men kan justeras till ex -5 eller -10. Test om -5 hörs i bruset eller ej. Hur mycket maskerar bruset, hur mycket är det möjligt att höra ljud under brusnivån?

                'Reporting the loudest/most audible time window
                'Utils.SendInfoToLog(TimeWindowAudibilityList.IndexOf(TimeWindowAudibilityList.Max), FileComparisonID & "_ExportedTimeWindowIndex.txt", MatrixOutputFolder)
                'Return TimeWindowAudibilityList.Max

                'Selecting method to express the audibility
                Dim TimeWindowOverlapRatio As Double = MeasurementWindowOverlapLength / MeasurementWindowLength

                Select Case ReportedAudibilityType
                    Case ReportedAudibilityTypes.SummedAudibility
                        'Calculatig the sum of the audible sound, weighted by time and frequency overlap ratios
                        'Adding the data into the ExportAudibilityMatrix
                        Dim SummedAudibility As Double = 0
                        For p = 0 To TimeWindowDetailedLIst.Count - 1
                            Dim CurrentTimeWindowSum As Double = 0

                            For q = 0 To TimeWindowDetailedLIst(0).Length - 1
                                CurrentTimeWindowSum += TimeWindowDetailedLIst(p)(q) * (1 - BarkFilterOverlapRatio)
                            Next

                            SummedAudibility += CurrentTimeWindowSum * (1 - TimeWindowOverlapRatio)
                        Next

                        Return SummedAudibility

                    Case ReportedAudibilityTypes.MaximumAudibility

                        'Finding the time window and bark filter that has the highest audibility value
                        'Adding the data into the ExportAudibilityMatrix
                        Dim MaximumAudibility As Double = -1
                        Dim LoudestTimeWindowIndex As Integer = -1
                        Dim LoudestBarkFilterIndex As Integer = -1
                        For p = 0 To TimeWindowDetailedLIst.Count - 1
                            For q = 0 To TimeWindowDetailedLIst(0).Length - 1
                                If TimeWindowDetailedLIst(p)(q) > MaximumAudibility Then
                                    MaximumAudibility = TimeWindowDetailedLIst(p)(q)
                                    'Updating the indices of the max value
                                    LoudestTimeWindowIndex = p
                                    LoudestBarkFilterIndex = q
                                End If
                            Next
                        Next

                        'This code can be optimized by taking, even though this does not report the selected index 
                        For p = 0 To TimeWindowDetailedLIst.Count - 1
                            MaximumAudibility = Math.Max(MaximumAudibility, TimeWindowDetailedLIst(p).Max)
                        Next

                        'Saving the index of the loudest window to file
                        SendInfoToAudioLog("Loudest value position: Time window index= " & LoudestTimeWindowIndex & " Bark filter index= " &
                                  LoudestBarkFilterIndex & " (Fc= " & CentreFrequencies(LoudestBarkFilterIndex) & " Hz)", "SelectedValuePosition", MatrixOutputFolder)

                        Return MaximumAudibility

                    Case ReportedAudibilityTypes.MostAudibleWindow
                        'Getting the loudest window

                        'Saving the index of the loudest window to file
                        SendInfoToAudioLog("Loudest window index: " & TimeWindowAudibilityList.IndexOf(TimeWindowAudibilityList.Max), "LoudestWindowIndex", MatrixOutputFolder)

                        Return TimeWindowAudibilityList.Max

                    Case ReportedAudibilityTypes.MostAudibleWindows

                        'Summing the loudest windows together repressenting a maximum of 200 ms of the speech sound
                        Dim SortedWindowsList As New List(Of Double)
                        Dim BandImportanceFactor As Double = 1 / CentreFrequencies.Count 'Equal band importance, is determined by taking 1 divided by the number of bark filters.

                        'Adding the summed audibility of each time window to SortedWindowsList
                        For p = 0 To TimeWindowDetailedLIst.Count - 1
                            Dim CurrentTimeWindowAudibility As Double = 0

                            For q = 0 To TimeWindowDetailedLIst(0).Length - 1
                                CurrentTimeWindowAudibility += TimeWindowDetailedLIst(p)(q) * BandImportanceFactor
                            Next

                            SortedWindowsList.Add(CurrentTimeWindowAudibility)
                        Next


                        'Sorting in ascending order
                        SortedWindowsList.Sort()

                        'Calculating how many windows that repressent 200 ms
                        Dim IntegrationDuration As Double = 0.2
                        Dim TimeStepDuration As Double = (MeasurementWindowLength - MeasurementWindowOverlapLength) / SampleRate
                        Dim MeasurementWindowOverlapDuration As Double = MeasurementWindowOverlapLength / SampleRate
                        If IntegrationDuration <= MeasurementWindowOverlapDuration Then Throw New Exception("IntegrationDuration must be longer than MeasurementWindowOverlapDuration")
                        Dim NumberOfWindowsToInclude As Integer = Math.Ceiling((IntegrationDuration - MeasurementWindowOverlapDuration) / TimeStepDuration)

                        'Summing the audibility of the loudest windows up to a summed duration of 200 ms
                        Dim SummedAudibility As Double = 0
                        If SortedWindowsList.Count > NumberOfWindowsToInclude Then

                            Dim LocalSum As Double = 0
                            For n = SortedWindowsList.Count - NumberOfWindowsToInclude To SortedWindowsList.Count - 1
                                LocalSum += SortedWindowsList(n)
                            Next
                            SummedAudibility = LocalSum * (1 - TimeWindowOverlapRatio)
                        Else
                            SummedAudibility = SortedWindowsList.Sum * (1 - TimeWindowOverlapRatio)
                        End If

                        Return SummedAudibility

                    Case ReportedAudibilityTypes.LimitedAudibilitySum
                        'Eller
                        'Ha korta FFTs 5 ms?, 4 ms overlap, och summera över (max) 200 ms, samt välja (genomsnittliga ljudnivån) i den starkaste max-200-ms-perioden 
                        Throw New NotImplementedException("Unspecified AudibilityMethod")

                    Case ReportedAudibilityTypes.AverageAudibility

                        Throw New NotImplementedException("Unspecified AudibilityMethod")

                    Case Else
                        Throw New NotImplementedException("Unspecified AudibilityMethod")
                End Select
            End Function


            ''' <summary>
            ''' Calculates the filter response of a FIR filter based on the FilterKernel.
            ''' </summary>
            ''' <param name="FilterKernel">The filter kernel.</param>
            ''' <param name="FftFormat"></param>
            ''' <param name="DurationMeasurementArrays">The duration in seconds of the sound arrays that the gain measurements will be based on.</param>
            ''' <returns>Returns a SortedList(Of Integer, SortedList(Of Double, Double)), where the first keys represent audio channels, and the inner list has frequencies as keys and corresponding filter gain as values.</returns>
            Public Function GetFirKernelResponse(ByRef FilterKernel As Sound, ByRef FftFormat As Formats.FftFormat, Optional DurationMeasurementArrays As Double = 10) As SortedList(Of Integer, SortedList(Of Double, Double))

                'Creating an impulse response with the length of DurationMeasurementArrays seconds
                Dim IR As New Sound(FilterKernel.WaveFormat)
                For c = 1 To IR.WaveFormat.Channels
                    Dim ChannelArray(DurationMeasurementArrays * FilterKernel.WaveFormat.SampleRate - 1) As Single
                    ChannelArray((ChannelArray.Length - 1) / 2) = 1
                    IR.WaveData.SampleData = ChannelArray
                Next

                'Creating a FIR filtered version of the IR
                Dim Filtered_IR = IR.CreateCopy
                Filtered_IR = DSP.FIRFilter(Filtered_IR, FilterKernel, New Formats.FftFormat, Nothing,,,,, True, True)

                'Calculating spectra of both
                IR.FFT = SpectralAnalysis(IR, FftFormat)
                IR.FFT.CalculateAmplitudeSpectrum(True, True, True)
                Filtered_IR.FFT = SpectralAnalysis(Filtered_IR, FftFormat)
                Filtered_IR.FFT.CalculateAmplitudeSpectrum(True, True, True)

                'Calculating the difference spectra (filter gain) of each channel
                Dim DifferenceSpectra As New SortedList(Of Integer, SortedList(Of Double, Double))
                For c = 1 To IR.WaveFormat.Channels

                    Dim IR_TotalLevel As Double
                    Dim Filtered_IR_TotalLevel As Double

                    'Calculating average amplitude spectrum for the two sounds
                    Dim IR_Spectrum = IR.FFT.GetAverageSpectrum(c, FftData.SpectrumTypes.AmplitudeSpectrum, IR.WaveFormat, True, IR_TotalLevel)
                    Dim Filtered_IR_Spectrum = Filtered_IR.FFT.GetAverageSpectrum(c, FftData.SpectrumTypes.AmplitudeSpectrum, Filtered_IR.WaveFormat, True, Filtered_IR_TotalLevel)

                    Dim ChannelDifferenceSpectrum As New SortedList(Of Double, Double)
                    For Each f In IR_Spectrum

                        'Calculating the spectral difference
                        Dim CurrentDifference As Double = Filtered_IR_Spectrum(f.Key) - IR_Spectrum(f.Key)

                        'Storing the difference along with the frequency
                        ChannelDifferenceSpectrum.Add(f.Key, CurrentDifference)
                    Next

                    'Adding channel data
                    DifferenceSpectra.Add(c, ChannelDifferenceSpectrum)
                Next

                Return DifferenceSpectra

            End Function

        End Module

        Public Enum SoundDistanceCalculationTypes
            Type1 'Duration distance
            Type2 'Time warped bark spectrum distance
            Type3
            Type4 'Average bark spectrum distance
        End Enum

        ''' <summary>
        ''' The acoustic distance model used to select the Swedish SiB-test phoneme contrasts.
        ''' </summary>
        Public Module AcousticDistance_ModelA

            ''' <summary>
            ''' Returns the duration distance, expressed as the proportions by which the longest sound is longer than the shortest sound.
            ''' </summary>
            ''' <param name="Sound1"></param>
            ''' <param name="Sound2"></param>
            ''' <returns></returns>
            Public Function GetSoundLengthDistance(ByRef Sound1 As Sound, ByRef Sound2 As Sound) As Double

                Dim DurationRatio As Double =
              Math.Max(Sound1.WaveData.ShortestChannelSampleCount, Sound2.WaveData.ShortestChannelSampleCount) /
          Math.Min(Sound1.WaveData.ShortestChannelSampleCount, Sound2.WaveData.ShortestChannelSampleCount)

                If DurationRatio > 0 Then
                    DurationRatio = DurationRatio - 1
                    Return DurationRatio
                Else
                    Return Single.PositiveInfinity
                End If

            End Function


            Public Function GetAcousticDistance_Non_dB(ByRef Sound1 As Sound, ByRef Sound2 As Sound,
                                            Optional ByVal MeasurementWindowDuration As Double = 0.1,
                                            Optional ByVal MeasurementWindowOverlapDuration As Double = 0.095,
                                            Optional ByVal BarkFilterOverlapRatio As Double = 0.9,
                                            Optional ByRef LowestIncludedCentreFrequency As Double = 80,
                                            Optional ByRef HighestIncludedCentreFrequency As Double = 17500,
                                            Optional ByVal MatrixOutputFolder As String = "",
                                            Optional ByVal ExportDetails As Boolean = False,
                                            Optional ByRef InactivateFftWarnings As Boolean = False,
                                            Optional ByRef UseImprovementsAfterSiB As Boolean = True) As Double

                ' N.B. This is the method called when calculating sound distance for the selection of the Swedish SiB-test phoneme contrasts.

                Dim OutputOnlyTemporalDistance As Boolean = False
                If OutputOnlyTemporalDistance Then
                    Dim DurationRatio As Double = Math.Min(Sound1.WaveData.ShortestChannelSampleCount, Sound2.WaveData.ShortestChannelSampleCount) /
                Math.Max(Sound1.WaveData.ShortestChannelSampleCount, Sound2.WaveData.ShortestChannelSampleCount)

                    If DurationRatio > 0 Then
                        DurationRatio = 1 / DurationRatio
                        Return DurationRatio
                    Else
                        Return Single.PositiveInfinity
                    End If

                End If

                'Setting up FFT formats
                Dim MeasurementWindowLength As Integer = Sound1.WaveFormat.SampleRate * MeasurementWindowDuration
                If MeasurementWindowLength Mod 2 = 1 Then MeasurementWindowLength += 1

                Dim MeasurementWindowOverlapLength As Integer = Sound1.WaveFormat.SampleRate * MeasurementWindowOverlapDuration
                If MeasurementWindowOverlapLength Mod 2 = 1 Then MeasurementWindowOverlapLength += 1
                Dim SpectralResolution As Integer = 2048 * 2 * 2
                Dim AD_FftFormat As New Formats.FftFormat(MeasurementWindowLength, SpectralResolution, MeasurementWindowOverlapLength, WindowingType.Hamming, InactivateFftWarnings)

                'Zeropadding the input sounds, so that they get MeasurementWindowOverlapLength before the sound. After the sound, the spectral analysis function adds zero padding automatically
                If Sound1.FFT Is Nothing Then Sound1.ZeroPad(MeasurementWindowOverlapLength, Nothing, False) 'Only zero padding if FFT is nothing, which means that it hasn't been here before
                Sound2.ZeroPad(MeasurementWindowOverlapLength, Nothing, False)

                'Checking that both sounds are mono
                If Sound1.WaveFormat.Channels <> 1 Or Sound2.WaveFormat.Channels <> 1 Then Throw New NotImplementedException("Unsupported channel count.")

                'Checking that the sounds have the same format
                If Sound1.WaveFormat.BitDepth <> Sound2.WaveFormat.BitDepth Or
            Sound1.WaveFormat.SampleRate <> Sound2.WaveFormat.SampleRate Or
            Sound1.WaveFormat.Encoding <> Sound2.WaveFormat.Encoding Then Throw New Exception("Different formats in input sounds.")


                'Getting frequency domain data (Only if it doesn't allready exist)
                If Sound1.FFT Is Nothing Then Sound1.FFT = SpectralAnalysis(Sound1, AD_FftFormat)
                If Sound2.FFT Is Nothing Then Sound2.FFT = SpectralAnalysis(Sound2, AD_FftFormat)

                'Calculating distance
                Dim AcousticDistance = CalculateTimeWarpedAcousticDistance(Sound1.FFT, Sound2.FFT, BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency,
                                                       Sound1.WaveFormat, MatrixOutputFolder, Sound1.FileName & "_" & Sound2.FileName, ExportDetails, UseImprovementsAfterSiB)

                Dim OutputTimeWeightedDistance As Boolean = True

                If OutputTimeWeightedDistance Then
                    Dim DurationRatio As Double = Math.Min(Sound1.WaveData.ShortestChannelSampleCount, Sound2.WaveData.ShortestChannelSampleCount) /
                Math.Max(Sound1.WaveData.ShortestChannelSampleCount, Sound2.WaveData.ShortestChannelSampleCount)

                    Dim TimeWeightedAcousticDistance As Double
                    If DurationRatio > 0 Then
                        TimeWeightedAcousticDistance = AcousticDistance / DurationRatio
                    Else
                        TimeWeightedAcousticDistance = 9999 ' Single.PositiveInfinity
                    End If

                    Return TimeWeightedAcousticDistance
                Else
                    Return AcousticDistance
                End If

            End Function

            Public Sub CalculateBarkSpectrum(ByRef InputSound As Sound,
                                         Optional ByVal BarkFilterOverlapRatio As Double = 0.5,
                                         Optional ByRef LowestIncludedCentreFrequency As Double = 80,
                                         Optional ByRef HighestIncludedCentreFrequency As Double = 17500,
                                         Optional ByRef ReusableCentreFrequencies As SortedSet(Of Single) = Nothing,
                                         Optional ByRef FftFormat As Formats.FftFormat = Nothing)


                'Setting up FFT formats
                If FftFormat Is Nothing Then FftFormat = New Formats.FftFormat(2048,, 1024, WindowingType.Hamming, False)
                Dim MeasurementWindowOverlapLength As Integer = FftFormat.OverlapSize

                'Calculating spectra
                InputSound.FFT = SpectralAnalysis(InputSound, FftFormat)
                InputSound.FFT.CalculatePowerSpectrum(True, True, True, 0.25)

                'Splitting the magnitude values in different bark filters (critical band widths)
                Dim FilterredMagnitudesArray As SortedList(Of Integer, Single()) = BarkFilter(InputSound.FFT, InputSound.WaveFormat.SampleRate, BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency, ReusableCentreFrequencies, True)

                'Referencing the data into the Sound1.FFT.BarkSpectrumTimeWindowData object
                For Each CurrentTimeWindow In FilterredMagnitudesArray
                    Dim NewTimeWindow As New FftData.TimeWindow
                    NewTimeWindow.WindowData = CurrentTimeWindow.Value
                    InputSound.FFT.AddBarkSpectrumTimeWindowData(NewTimeWindow, 1)
                Next


            End Sub

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="StartWindow"></param>
            ''' <param name="AnalysisLength"></param>
            ''' <param name="LowerInclusiveLimit">If set, determines the lowest frequency/fft bin included in the calculation.</param>
            ''' <param name="UpperLimit">If set, determines the highest frequency/fft bin included in the calculation.</param>
            ''' <param name="InputType">Determines whether LowerInclusiveLimit and UpperInclusiveLimit are defined as fft-bin indices or frequencies in Hz</param>
            ''' <returns></returns>
            Public Function CalculateWindowLevels(ByRef InputSound As Sound, Optional ByVal StartWindow As Integer = 0,
                                               Optional ByVal AnalysisLength As Integer? = Nothing,
                                              Optional ByVal LowerInclusiveLimit As Single? = Nothing,
                                         Optional ByVal UpperLimit As Single? = Nothing,
                                         Optional ByRef InputType As FftData.GetSpectrumLevel_InputType = FftData.GetSpectrumLevel_InputType.FftBinIndex,
                                              Optional ByVal LowerLimitIsInclusive As Boolean = True,
                                              Optional ByVal UpperLimitIsInclusive As Boolean = True,
                                              Optional ByRef ActualLowerLimitFrequency As Single? = Nothing,
                                              Optional ByRef ActualUpperLimitFrequency As Single? = Nothing) As Double()

                Dim AvaliableWindowsCount As Integer = InputSound.FFT.WindowCount(1)
                If StartWindow < 0 Then StartWindow = 0
                If AnalysisLength Is Nothing Then AnalysisLength = AvaliableWindowsCount
                If StartWindow + AnalysisLength > AvaliableWindowsCount Then
                    AnalysisLength = AvaliableWindowsCount - StartWindow
                End If
                If AnalysisLength < 1 Then Return Nothing 'Returns Nothing if not enough data exists

                'Calculating window levels
                Dim LevelArray(AnalysisLength - 1) As Double
                For w = StartWindow To AnalysisLength - 1
                    LevelArray(w) = InputSound.FFT.GetSpectrumLevel(1, w, FftData.SpectrumTypes.PowerSpectrum,
                                                                LowerInclusiveLimit, UpperLimit, InputType,
                                                                      FftData.GetSpectrumLevel_OutputType.SpectrumLevel_dB,
                                                                      ActualLowerLimitFrequency,
                                                                      ActualUpperLimitFrequency,
                                                                      LowerLimitIsInclusive,
                                                                      UpperLimitIsInclusive)
                Next

                Return LevelArray

            End Function


            ''' <summary>
            ''' Calculates the average Bark spectrum of a sound file which has been previously analysed using the method CalculateBarkSpectrum.
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <returns></returns>
            Public Function GetAverageBarkSpectrum(ByRef InputSound As Sound, Optional ByVal StartWindow As Integer = 0,
                                               Optional ByVal AnalysisLength As Integer? = Nothing,
                                               Optional ByRef TotalLevel As Double = Double.NegativeInfinity) As FftData.TimeWindow

                Dim BarkBinCount As Integer = InputSound.FFT.BarkSpectrumTimeWindowData(1, 0).WindowData.Length
                Dim AvaliableWindowsCount As Integer = InputSound.FFT.WindowCount(1)
                If StartWindow < 0 Then StartWindow = 0
                If AnalysisLength Is Nothing Then AnalysisLength = AvaliableWindowsCount
                If StartWindow + AnalysisLength > AvaliableWindowsCount Then
                    AnalysisLength = AvaliableWindowsCount - StartWindow
                End If
                If AnalysisLength < 1 Then Return Nothing 'Returns Nothing if not enough data exists

                'Calculating average Bark spectra
                Dim AverageData As New FftData.TimeWindow
                Dim FrequencyArray(BarkBinCount - 1) As Single
                AverageData.WindowData = FrequencyArray

                TotalLevel = 0

                For f = 0 To BarkBinCount - 1
                    For w = StartWindow To StartWindow + AnalysisLength - 1
                        FrequencyArray(f) += InputSound.FFT.BarkSpectrumTimeWindowData(1, w).WindowData(f)
                    Next
                    FrequencyArray(f) /= AnalysisLength
                Next

                'Dividing by the number Of barkbands, and Converting to dB
                For f = 0 To BarkBinCount - 1

                    'Compensating for the BarkFilterOverlapRatio
                    FrequencyArray(f) /= BarkBinCount ' (1 - BarkFilterOverlapRatio) 'N.B. This compensation is only approximate (and will slightly under-estimate the sound distance), as the lowest and highest bark bands have not been overlapped as many times!

                    'Increasing TotalLevel 
                    TotalLevel += FrequencyArray(f)

                    'Taking the root to convert power to amplitude spectrum, multiplying by 2, to compensate for not using negative frequencies
                    FrequencyArray(f) = 2 * Math.Sqrt(FrequencyArray(f))

                    'Converting to dB
                    FrequencyArray(f) = dBConversion(FrequencyArray(f), dBConversionDirection.to_dB, InputSound.WaveFormat, dBTypes.SoundPressure)
                Next

                'Converting the TotalLevel to dB
                'Taking the root to convert power to amplitude spectrum, multiplying by 2, to compensate for not using negative frequencies
                TotalLevel = 2 * Math.Sqrt(TotalLevel)

                'Converting to dB
                TotalLevel = dBConversion(TotalLevel, dBConversionDirection.to_dB, InputSound.WaveFormat, dBTypes.SoundPressure)

                Return AverageData

            End Function

            Public AverageBarkSpectrumDistanceSpinLock1 As New Threading.SpinLock
            Public AverageBarkSpectrumDistanceSpinLock2 As New Threading.SpinLock


            ''' <summary>
            ''' Calculates the average Bark spectrum power difference (in dB) between two sounds.
            ''' </summary>
            ''' <param name="Sound1"></param>
            ''' <param name="Sound2"></param>
            ''' <param name="BarkFilterOverlapRatio"></param>
            ''' <param name="LowestIncludedCentreFrequency"></param>
            ''' <param name="HighestIncludedCentreFrequency"></param>
            ''' <param name="ExportBarkSpectra"></param>
            ''' <param name="LogOutputFolder"></param>
            ''' <param name="FftFormat"></param>
            ''' <param name="Sound1BarkSpectrum">Upon return, will contain the bark spectrum for sound 1.</param>
            ''' <param name="Sound2BarkSpectrum">Upon return, will contain the bark spectrum for sound 2.</param>
            ''' <returns></returns>
            Public Function GetAverageBarkSpectrumDistance(ByRef Sound1 As Sound, ByRef Sound2 As Sound,
                                                       Optional ByVal BarkFilterOverlapRatio As Double = 0.5,
                                                       Optional ByRef LowestIncludedCentreFrequency As Double = 80,
                                                       Optional ByRef HighestIncludedCentreFrequency As Double = 17500,
                                                       Optional ByRef IrrelevantDifferenceThreshold As Single? = Nothing,
                                                       Optional ByRef ReusableCentreFrequencies As SortedSet(Of Single) = Nothing,
                                                       Optional ByVal ExportBarkSpectra As Boolean = False,
                                                       Optional ByVal LogOutputFolder As String = "",
                                                       Optional ByRef FftFormat As Formats.FftFormat = Nothing,
                                                       Optional ByRef Sound1BarkSpectrum As Single() = Nothing,
                                                       Optional ByRef Sound2BarkSpectrum As Single() = Nothing,
                                                       Optional ByVal SkipDistanceCalculation As Boolean = False) As Double

                Dim SpinLock1Taken As Boolean = False
                Dim SpinLock2Taken As Boolean = False

                Try

                    'Checking that the log output folder exists, if it is needed
                    If ExportBarkSpectra = True Then If Not Directory.Exists(LogOutputFolder) Then Directory.CreateDirectory(LogOutputFolder)


                    'Checking that both sounds are mono
                    If Sound1.WaveFormat.Channels <> 1 Or Sound2.WaveFormat.Channels <> 1 Then Throw New NotImplementedException("Unsupported channel count.")

                    'Checking that the sounds have the same format
                    If Sound1.WaveFormat.BitDepth <> Sound2.WaveFormat.BitDepth Or
                    Sound1.WaveFormat.SampleRate <> Sound2.WaveFormat.SampleRate Or
                    Sound1.WaveFormat.Encoding <> Sound2.WaveFormat.Encoding Then Throw New Exception("Different formats in input sounds.")



                    'Setting up FFT formats
                    If FftFormat Is Nothing Then FftFormat = New Formats.FftFormat(2048,, 1024, WindowingType.Hamming, False)
                    Dim MeasurementWindowOverlapLength As Integer = FftFormat.OverlapSize

                    'Getting frequency domain data (Only if it doesn't allready exist)
                    If Sound1.FFT Is Nothing Then

                        'Attempts to enter a spin lock to avoid that different threads try to calculate the FFt of the same sound
                        AverageBarkSpectrumDistanceSpinLock1.Enter(SpinLock1Taken)

                        'Checking again
                        If Sound1.FFT Is Nothing Then

                            'Zeropadding the input sounds, so that they get MeasurementWindowOverlapLength before the sound. After the sound, the spectral analysis function adds zero padding automatically
                            Sound1.ZeroPad(MeasurementWindowOverlapLength, Nothing, False) 'Only zero padding if FFT is nothing, which means that it hasn't been here before

                            'Calculating spectra
                            Sound1.FFT = SpectralAnalysis(Sound1, FftFormat)
                            Sound1.FFT.CalculatePowerSpectrum(True, True, True, 0.25)

                            'Splitting the magnitude values in different bark filters (critical band widths)
                            Dim FilterredMagnitudesArray As SortedList(Of Integer, Single()) = BarkFilter(Sound1.FFT, Sound1.WaveFormat.SampleRate, BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency, ReusableCentreFrequencies, True)

                            'Referencing the data into the Sound1.FFT.BarkSpectrumTimeWindowData object
                            For Each CurrentTimeWindow In FilterredMagnitudesArray
                                Dim NewTimeWindow As New FftData.TimeWindow
                                NewTimeWindow.WindowData = CurrentTimeWindow.Value
                                Sound1.FFT.AddBarkSpectrumTimeWindowData(NewTimeWindow, 1)
                            Next

                            'Calculating average Bark spectra, and stores in the FFT.TemporaryData Object, for re-use in the next analyses
                            Dim AverageData As New FftData.TimeWindow
                            Dim FrequencyArray(Sound1.FFT.BarkSpectrumTimeWindowData(1, 0).WindowData.Length - 1) As Single
                            AverageData.WindowData = FrequencyArray

                            For f = 0 To FrequencyArray.Length - 1
                                For w = 0 To Sound1.FFT.WindowCount(1) - 1
                                    FrequencyArray(f) += Sound1.FFT.BarkSpectrumTimeWindowData(1, w).WindowData(f)
                                Next
                                FrequencyArray(f) /= Sound1.FFT.WindowCount(1)
                            Next

                            'Dividing by the number Of barkbands, and Converting to dB
                            For f = 0 To FrequencyArray.Length - 1

                                'Compensating for the BarkFilterOverlapRatio
                                FrequencyArray(f) /= ReusableCentreFrequencies.Count ' (1 - BarkFilterOverlapRatio) 'N.B. This compensation is only approximate (and will slightly under-estimate the sound distance), as the lowest and highest bark bands have not been overlapped as many times!

                                'NB. NEW line from 2018-12-26!!: 'Taking the root to convert power to amplitude spectrum, and multiplying by 2, to compensate for not using negative frequencies
                                FrequencyArray(f) = 2 * Math.Sqrt(FrequencyArray(f))

                                'Converting to dB
                                FrequencyArray(f) = dBConversion(FrequencyArray(f), dBConversionDirection.to_dB, Sound1.WaveFormat, dBTypes.SoundPressure)
                            Next

                            Sound1.FFT.TemporaryData = New List(Of FftData.TimeWindow)
                            Sound1.FFT.TemporaryData.Add(AverageData)

                            'Exporting Bark spectra for file 1 along with headings
                            If ExportBarkSpectra = True Then

                                'Exporting top section of file, if the file doesn't already exist
                                If Not File.Exists(IO.Path.Combine(LogOutputFolder, "BarkSpectra.txt")) Then

                                    'Exporting the heading
                                    SendInfoToAudioLog("Average Bark spectra:", "BarkSpectra", LogOutputFolder, True, True)
                                    SendInfoToAudioLog("File name" & vbTab & "Distance to " & Sound1.FileName & vbTab & "Sound power levels (dB) / Bark band (Centre frequency in Hz)", "BarkSpectra", LogOutputFolder, True, True)

                                    'Exporting the centre frequencies
                                    SendInfoToAudioLog(vbTab & vbTab & String.Join(vbTab, ReusableCentreFrequencies), "BarkSpectra", LogOutputFolder, True, True)

                                End If

                                'Exporting the sound power data
                                SendInfoToAudioLog(Sound1.FileName & vbTab & vbTab & String.Join(vbTab, Sound1.FFT.TemporaryData(0).WindowData), "BarkSpectra", LogOutputFolder, True, True)

                            End If

                            'Releases the spinlock
                            If SpinLock1Taken = True Then AverageBarkSpectrumDistanceSpinLock1.Exit()

                        End If
                    End If

                    If Sound2.FFT Is Nothing Then

                        'Attempts to enter a spin lock to avoid that different threads try to calculate the FFt of the same sound
                        AverageBarkSpectrumDistanceSpinLock2.Enter(SpinLock2Taken)

                        'Checking again
                        If Sound2.FFT Is Nothing Then

                            'Zeropadding the input sounds, so that they get MeasurementWindowOverlapLength before the sound. After the sound, the spectral analysis function adds zero padding automatically
                            Sound2.ZeroPad(MeasurementWindowOverlapLength, Nothing, False)

                            'Calculating spectra
                            Sound2.FFT = SpectralAnalysis(Sound2, FftFormat)
                            Sound2.FFT.CalculatePowerSpectrum(True, True, True, 0.25)

                            'Splitting the magnitude values in different bark filters (critical band widths)
                            Dim FilterredMagnitudesArray As SortedList(Of Integer, Single()) = BarkFilter(Sound2.FFT, Sound1.WaveFormat.SampleRate, BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency, ReusableCentreFrequencies, True)

                            'Referencing the data into the Sound1.FFT.BarkSpectrumTimeWindowData object
                            For Each CurrentTimeWindow In FilterredMagnitudesArray
                                Dim NewTimeWindow As New FftData.TimeWindow
                                NewTimeWindow.WindowData = CurrentTimeWindow.Value
                                Sound2.FFT.AddBarkSpectrumTimeWindowData(NewTimeWindow, 1)
                            Next

                            'Calculating average Bark spectra, and stores in the FFT.TemporaryData Object, for re-use in the next analyses
                            Dim AverageData As New FftData.TimeWindow
                            Dim FrequencyArray(Sound2.FFT.BarkSpectrumTimeWindowData(1, 0).WindowData.Length - 1) As Single
                            AverageData.WindowData = FrequencyArray

                            For f = 0 To FrequencyArray.Length - 1
                                For w = 0 To Sound2.FFT.WindowCount(1) - 1
                                    FrequencyArray(f) += Sound2.FFT.BarkSpectrumTimeWindowData(1, w).WindowData(f)
                                Next
                                FrequencyArray(f) /= Sound2.FFT.WindowCount(1)
                            Next

                            'Dividing by the number of barkbands, and Converting to dB
                            For f = 0 To FrequencyArray.Length - 1

                                'Compensating for the BarkFilterOverlapRatio
                                FrequencyArray(f) /= ReusableCentreFrequencies.Count ' (1 - BarkFilterOverlapRatio) 'N.B. This compensation is only approximate (and will slightly under-estimate the sound distance), as the lowest and highest bark bands have not been overlapped as many times!

                                'NB. NEW line from 2018-12-26!!: 'Taking the root to convert power to amplitude spectrum, and multiplying by 2, to compensate for not using negative frequencies
                                FrequencyArray(f) = 2 * Math.Sqrt(FrequencyArray(f))

                                'Converting to dB
                                FrequencyArray(f) = dBConversion(FrequencyArray(f), dBConversionDirection.to_dB, Sound2.WaveFormat, dBTypes.SoundPressure)
                            Next

                            Sound2.FFT.TemporaryData = New List(Of FftData.TimeWindow)
                            Sound2.FFT.TemporaryData.Add(AverageData)

                            'Releases the spinlock
                            If SpinLock2Taken = True Then AverageBarkSpectrumDistanceSpinLock2.Exit()

                        End If
                    End If

                    'Calculating the Euclidean distance (unless it is overridden by SkipDistanceCalculation)
                    Dim Distance As Double
                    If SkipDistanceCalculation = False Then
                        If IrrelevantDifferenceThreshold Is Nothing Then
                            Distance = Utils.Calculations.GetEuclideanDistance(Sound1.FFT.TemporaryData(0).WindowData, Sound2.FFT.TemporaryData(0).WindowData)
                        Else
                            Distance = Utils.Calculations.GetEuclideanDistance(Sound1.FFT.TemporaryData(0).WindowData, Sound2.FFT.TemporaryData(0).WindowData, IrrelevantDifferenceThreshold)
                        End If
                    End If


                    'Exporting Bark spectra for File 2, along with the distance to file 1
                    If ExportBarkSpectra = True Then

                        'Exporting the sound power data
                        SendInfoToAudioLog(Sound2.FileName & vbTab & Distance & vbTab & String.Join(vbTab, Sound2.FFT.TemporaryData(0).WindowData), "BarkSpectra", LogOutputFolder, True, True)

                    End If

                    'Referencing Sound1BarkSpectrum and Sound2BarkSpectrum
                    Sound1BarkSpectrum = Sound1.FFT.TemporaryData(0).WindowData
                    Sound2BarkSpectrum = Sound2.FFT.TemporaryData(0).WindowData

                    Return Distance

                Catch ex As Exception

                    'MsgBox(ex.ToString)
                    Return 0

                Finally

                    'Releases any spinlock
                    If AverageBarkSpectrumDistanceSpinLock1.IsHeldByCurrentThread = True Then If SpinLock1Taken = True Then AverageBarkSpectrumDistanceSpinLock1.Exit()
                    If AverageBarkSpectrumDistanceSpinLock2.IsHeldByCurrentThread = True Then If SpinLock2Taken = True Then AverageBarkSpectrumDistanceSpinLock2.Exit()
                End Try

            End Function


            ''' <summary>
            ''' Calculates acoustic distance based on dynamic time warping.  For reference to the principles behind this code, see Jurafsky and Martin (p 333), and Gold, Morgan, Ellis (pp 340).
            ''' </summary>
            ''' <param name="FftData1"></param>
            ''' <param name="FftData2"></param>
            ''' <returns></returns>
            Private Function CalculateTimeWarpedAcousticDistance(ByRef FftData1 As FftData,
                                                             ByRef FftData2 As FftData,
                                                             ByVal BarkFilterOverlapRatio As Double,
                                                             ByRef LowestIncludedCentreFrequency As Double,
                                                             ByRef HighestIncludedCentreFrequency As Double,
                                                             ByVal CurrentWaveFormat As Formats.WaveFormat,
                                                             ByVal MatrixOutputFolder As String,
                                                             ByVal FileComparisonID As String,
                                                             ByVal ExportDetails As Boolean,
                                                             ByVal UseImprovementsAfterSiB As Boolean) As Double



                Dim SampleRate As Integer = CurrentWaveFormat.SampleRate

                Dim LocalRestrictions As Boolean = True
                Dim DiagonalWeight As Double = 2
                Dim HorizontalWeight As Double = 1
                Dim VerticalWeight As Double = 1

                'Calculating magnitude arrays
                If UseImprovementsAfterSiB = True Then
                    FftData1.CalculateAmplitudeSpectrum()
                    FftData2.CalculateAmplitudeSpectrum()
                Else
                    FftData1.CalculateAmplitudeSpectrum(False, False, False)
                    FftData2.CalculateAmplitudeSpectrum(False, False, False)
                End If


                'Splitting the magnitude values in different bark filters (critical band widths)
                Dim CentreFrequencies As SortedSet(Of Single) = Nothing
                Dim FilterredMagnitudesArray1 As SortedList(Of Integer, Single()) = BarkFilter(FftData1, SampleRate, BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency, CentreFrequencies,, UseImprovementsAfterSiB)
                Dim FilterredMagnitudesArray2 As SortedList(Of Integer, Single()) = BarkFilter(FftData2, SampleRate, BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency, CentreFrequencies,, UseImprovementsAfterSiB)


                If ExportDetails = True And MatrixOutputFolder <> "" Then
                    'Exports centre frequencies instead of magnitudes
                    Dim newDoubleArray(1, CentreFrequencies.Count - 1) As Double
                    For p = 0 To CentreFrequencies.Count - 1
                        newDoubleArray(0, p) = CentreFrequencies(p)
                        newDoubleArray(1, p) = Utils.CenterFrequencyToBarkFilterBandwidth(CentreFrequencies(p))
                    Next
                    Utils.SaveMatrixToFile(newDoubleArray, IO.Path.Combine(MatrixOutputFolder, "CentreFrequencies.txt"))
                End If

                If UseImprovementsAfterSiB = True Then

                    'Taking logs +1 of the filterred magnitude arrays
                    For Each Window In FilterredMagnitudesArray1
                        For n = 0 To Window.Value.Count - 1
                            FilterredMagnitudesArray1(Window.Key)(n) = dBConversion(FilterredMagnitudesArray1(Window.Key)(n) + Single.Epsilon, dBConversionDirection.to_dB,
                                                  CurrentWaveFormat, dBTypes.SoundPressure)
                        Next
                    Next
                    For Each Window In FilterredMagnitudesArray2
                        For n = 0 To Window.Value.Count - 1
                            FilterredMagnitudesArray2(Window.Key)(n) = dBConversion(FilterredMagnitudesArray2(Window.Key)(n) + Single.Epsilon, dBConversionDirection.to_dB,
                                                  CurrentWaveFormat, dBTypes.SoundPressure)
                        Next
                    Next

                Else
                    'Taking logs +1 of the filterred magnitude arrays
                    For Each Window In FilterredMagnitudesArray1
                        For n = 0 To Window.Value.Count - 1
                            FilterredMagnitudesArray1(Window.Key)(n) = Math.Log10(FilterredMagnitudesArray1(Window.Key)(n) + Single.Epsilon) 'Single.Epsilon repressents silence/ the smallest possible positive value of Single
                        Next
                    Next
                    For Each Window In FilterredMagnitudesArray2
                        For n = 0 To Window.Value.Count - 1
                            FilterredMagnitudesArray2(Window.Key)(n) = Math.Log10(FilterredMagnitudesArray2(Window.Key)(n) + Single.Epsilon)
                        Next
                    Next
                End If


                If ExportDetails = True And MatrixOutputFolder <> "" Then
                    Dim newDoubleArray(FilterredMagnitudesArray1.Count - 1, FilterredMagnitudesArray1(0).Length - 1) As Double
                    For p = 0 To FilterredMagnitudesArray1.Count - 1
                        For q = 0 To FilterredMagnitudesArray1(0).Length - 1
                            newDoubleArray(p, q) = FilterredMagnitudesArray1(p)(q)
                        Next
                    Next
                    Utils.SaveMatrixToFile(newDoubleArray, IO.Path.Combine(MatrixOutputFolder, "Spectrum_" & FileComparisonID & "A.txt"))

                    Dim newDoubleArray2(FilterredMagnitudesArray2.Count - 1, FilterredMagnitudesArray2(0).Length - 1) As Double
                    For p = 0 To FilterredMagnitudesArray2.Count - 1
                        For q = 0 To FilterredMagnitudesArray2(0).Length - 1
                            newDoubleArray2(p, q) = FilterredMagnitudesArray2(p)(q)
                        Next
                    Next
                    Utils.SaveMatrixToFile(newDoubleArray2, IO.Path.Combine(MatrixOutputFolder, "Spectrum_" & FileComparisonID & "B.txt"))

                End If

                'Setting up a distance matrix containing the distances between every time window in the two input sounds
                Dim ColumnCount As Integer = FilterredMagnitudesArray1.Count
                Dim RowCount As Integer = FilterredMagnitudesArray2.Count
                Dim DistanceMatrix(ColumnCount - 1, RowCount - 1) As Double

                'Measuring distances between time windows. 
                For Column_i = 0 To ColumnCount - 1
                    For Row_j = 0 To RowCount - 1

                        If LocalRestrictions = True Then
                            'Blocking windows which should not be measured, and just putting Double.Maxvalue in them.
                            If IsWithinMeasurementRegion(ColumnCount - 1, Column_i, RowCount - 1, Row_j) = False Then
                                Continue For
                            End If
                        End If

                        DistanceMatrix(Column_i, Row_j) = GetDistanceValue(FilterredMagnitudesArray1, FilterredMagnitudesArray2, Column_i, Row_j)
                    Next
                Next

                'Saving matrix to file
                If ExportDetails = True And MatrixOutputFolder <> "" Then
                    Utils.SaveMatrixToFile(DistanceMatrix, IO.Path.Combine(MatrixOutputFolder, FileComparisonID & "_Distance.txt"))
                End If

                'Doing dynamic time warping into a new table, the first window is only used for initial comparison (as if it repressented sound prior to the first window), whereby the matrix need to be,
                Dim DtwMatrix(ColumnCount - 1, RowCount - 1) As DTWPoint

                'Filling up the DtwMatrix
                For Column_i = 0 To ColumnCount - 1
                    For Row_j = 0 To RowCount - 1
                        DtwMatrix(Column_i, Row_j) = New DTWPoint(Column_i, Row_j, 0)

                        If LocalRestrictions = True Then
                            'Blocking windows are marked by
                            If IsWithinMeasurementRegion(ColumnCount - 1, Column_i, RowCount - 1, Row_j) = False Then
                                DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.BlockedPoint
                            End If
                        End If

                    Next
                Next


                'Calculating symmetric type 0 DTW (For reference, see Sakoe and Chiba 1978, Optimization for spoken word recognition). Adapted with a restriction that warping is only allowed if the previous value wasn't created y a warping
                For Column_i = 0 To ColumnCount - 1
                    For Row_j = 0 To RowCount - 1

                        'Skipping measurements outside the measurement window
                        If DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.BlockedPoint Then
                            Continue For
                        End If

                        'Declaring blockers
                        Dim BlockHorizontalMove As Boolean = False
                        Dim BlockVerticalMove As Boolean = False

                        'Blocking moves from anything to the left of the first column
                        If Column_i = 0 Then
                            BlockHorizontalMove = True
                        End If

                        'Blocking moves from anything above the first row
                        If Row_j = 0 Then
                            BlockVerticalMove = True
                        End If


                        'Activating local restrictions
                        If LocalRestrictions = True Then

                            'Direction restrictions, blocks the horizontal or vertical directions if the horizontally or vertically preceding cell is it self derived from a horizontal or vertical direction
                            If Column_i > 0 Then
                                If DtwMatrix(Column_i - 1, Row_j).TransitionDirection = TransitionDirections.Horizontal Then
                                    BlockHorizontalMove = True
                                End If
                            End If

                            If Row_j > 0 Then
                                If DtwMatrix(Column_i, Row_j - 1).TransitionDirection = TransitionDirections.Vertical Then
                                    BlockVerticalMove = True
                                End If
                            End If
                        End If

                        'Getting the possible transition sums
                        Dim DiagonalPathValue As Double = Double.PositiveInfinity
                        If Column_i > 0 And Row_j > 0 Then
                            If Not DtwMatrix(Column_i - 1, Row_j - 1).TransitionDirection = TransitionDirections.BlockedPoint Then
                                DiagonalPathValue = DtwMatrix(Column_i - 1, Row_j - 1).Value + DiagonalWeight * DistanceMatrix(Column_i, Row_j)
                            End If
                        End If

                        Dim VerticalPathValue As Double = Double.PositiveInfinity
                        If Row_j > 0 Then
                            If Not DtwMatrix(Column_i, Row_j - 1).TransitionDirection = TransitionDirections.BlockedPoint Then
                                VerticalPathValue = DtwMatrix(Column_i, Row_j - 1).Value + VerticalWeight * DistanceMatrix(Column_i, Row_j)
                            End If
                        End If

                        Dim HorizintalPathValue As Double = Double.PositiveInfinity
                        If Column_i > 0 Then
                            If Not DtwMatrix(Column_i - 1, Row_j).TransitionDirection = TransitionDirections.BlockedPoint Then
                                HorizintalPathValue = DtwMatrix(Column_i - 1, Row_j).Value + HorizontalWeight * DistanceMatrix(Column_i, Row_j)
                            End If
                        End If

                        'Selecting the best path
                        If Column_i = 0 And Row_j = 0 Then
                            'The special case of position 0,0
                            'No move at all, just adding the distance value
                            DtwMatrix(Column_i, Row_j).Value = DiagonalWeight * DistanceMatrix(Column_i, Row_j)
                            DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.NoDirection


                        ElseIf (DiagonalPathValue < VerticalPathValue And DiagonalPathValue < HorizintalPathValue) Or
                            (BlockVerticalMove = True And DiagonalPathValue < HorizintalPathValue) Or
                            (BlockHorizontalMove = True And DiagonalPathValue < VerticalPathValue) Or
                            (BlockVerticalMove = True And BlockHorizontalMove = True And DiagonalPathValue < Double.PositiveInfinity) Or
                            (DiagonalPathValue = 0 Or (DiagonalPathValue = VerticalPathValue And DiagonalPathValue = HorizintalPathValue)) Then 'N.B. the preferred path is always diagonal if the DiagonalPathValue = 0, or if all path values are equal (which would mean that we're in a silent window.)

                            'A diagonal move
                            DtwMatrix(Column_i, Row_j).Value = DiagonalPathValue
                            DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.Diagonal

                            'Storing history from the diagonally preceding cell
                            For Each PrecedingItem In DtwMatrix(Column_i - 1, Row_j - 1).History
                                DtwMatrix(Column_i, Row_j).History.Add(PrecedingItem)
                            Next


                        ElseIf BlockVerticalMove = False And
                        (VerticalPathValue < HorizintalPathValue Or (BlockHorizontalMove = True And VerticalPathValue < Double.PositiveInfinity)) Then

                            'A vertical move
                            DtwMatrix(Column_i, Row_j).Value = VerticalPathValue
                            DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.Vertical

                            'Storing history from the vertically preceding cell
                            For Each PrecedingItem In DtwMatrix(Column_i, Row_j - 1).History
                                DtwMatrix(Column_i, Row_j).History.Add(PrecedingItem)
                            Next


                        ElseIf BlockHorizontalMove = False And
                        (HorizintalPathValue < VerticalPathValue Or (BlockVerticalMove = True And HorizintalPathValue < Double.PositiveInfinity)) Then

                            'A horizontal move
                            DtwMatrix(Column_i, Row_j).Value = HorizintalPathValue
                            DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.Horizontal

                            'Storing history from the horizontally preceding cell
                            For Each PrecedingItem In DtwMatrix(Column_i - 1, Row_j).History
                                DtwMatrix(Column_i, Row_j).History.Add(PrecedingItem)
                            Next


                            'Overriding blockings
                        ElseIf DiagonalPathValue = Double.PositiveInfinity And BlockVerticalMove = True And HorizintalPathValue = Double.PositiveInfinity Then

                            'Overriding the vertical block since it is the only available value (this happens when conflicts occur between area and direction blockings)
                            'A vertical move
                            DtwMatrix(Column_i, Row_j).Value = VerticalPathValue
                            DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.Vertical

                            'Storing history from the vertically preceding cell
                            For Each PrecedingItem In DtwMatrix(Column_i, Row_j - 1).History
                                DtwMatrix(Column_i, Row_j).History.Add(PrecedingItem)
                            Next

                        ElseIf (DiagonalPathValue = Double.PositiveInfinity And VerticalPathValue = Double.PositiveInfinity And BlockHorizontalMove = True) Then

                            'Overriding the horizontal block since it is the only available value (this happens when conflicts occur between area and direction blockings)
                            'A horizontal move
                            DtwMatrix(Column_i, Row_j).Value = HorizintalPathValue
                            DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.Horizontal

                            'Storing history from the horizontally preceding cell
                            For Each PrecedingItem In DtwMatrix(Column_i - 1, Row_j).History
                                DtwMatrix(Column_i, Row_j).History.Add(PrecedingItem)
                            Next

                        Else
                            MsgBox("Something is wrong!")

                        End If

                        'Adding the current cell to it's own history
                        DtwMatrix(Column_i, Row_j).History.Add(DtwMatrix(Column_i, Row_j))

                    Next
                Next


                'Exporting the history of the "winner"
                If ExportDetails = True And MatrixOutputFolder <> "" Then
                    Dim HistoryOutputList As New List(Of String)
                    For Each CurrentPTWPoint In DtwMatrix(ColumnCount - 1, RowCount - 1).History
                        HistoryOutputList.Add(CurrentPTWPoint.ColumnIndex & "," & CurrentPTWPoint.RowIndex & vbCrLf)
                    Next
                    Utils.SaveListOfStringToTxtFile(HistoryOutputList, MatrixOutputFolder, "WinnerHistory")
                End If


                'Saving matrix to file
                If ExportDetails = True And MatrixOutputFolder <> "" Then
                    SaveDTWPointMatrixToFile(DtwMatrix, IO.Path.Combine(MatrixOutputFolder, FileComparisonID & "_DTW.txt"))
                    'SaveMatrixToFile(DtwMatrix, IO.Path.Combine(MatrixOutputFolder, FileComparisonID & "_Restrictions.txt"), 1)
                End If

                Dim DoNormalixation As Boolean = True
                If DoNormalixation = True Then
                    'This is the type of normalization of White and Neely, as reported in Sakoe and Chiba 1978, Optimization for spoken word recognition
                    Return (DtwMatrix(ColumnCount - 1, RowCount - 1).Value) / (ColumnCount - 1 + RowCount - 1)
                Else
                    Return (DtwMatrix(ColumnCount - 1, RowCount - 1).Value)
                End If

            End Function

            Private Function IsWithinMeasurementRegion(ByRef Capital_I As Integer, ByRef i As Integer,
                                                   ByRef Capital_J As Integer, ByRef j As Integer) As Boolean

                If IsBelowBlockingAreaBoundary(Capital_I, i, Capital_J, j) = False Then Return False
                If IsRightOfBlockingAreaBoundary(Capital_I, i, Capital_J, j) = False Then Return False

                'If i < (Capital_I * j) / (2 * Capital_J) Then Return False
                'If i > (Capital_I * j) / (2 * Capital_J) + Capital_I / 2 Then Return False
                'If j < (Capital_J * i) / (2 * Capital_I) Then Return False
                'If j > (Capital_J * i) / (2 * Capital_I) + Capital_J / 2 Then Return False

                Return True

            End Function

            Private Function IsBelowBlockingAreaBoundary(ByRef Capital_I As Integer, ByRef i As Integer,
                                                     ByRef Capital_J As Integer, ByRef j As Integer) As Boolean

                'If i > (Capital_I * j) / (2 * Capital_J) + Capital_I / 2 Then Return False
                'If j < (Capital_J * i) / (2 * Capital_I) Then Return False

                If i < Int(((Capital_I * j) / (2 * Capital_J))) Then Return False
                If j > Utils.Rounding(((Capital_J * i) / (2 * Capital_I)) + (Capital_J / 2), Utils.roundingMethods.alwaysUp) Then Return False

                Return True

            End Function

            Private Function IsRightOfBlockingAreaBoundary(ByRef Capital_I As Integer, ByRef i As Integer,
                                                       ByRef Capital_J As Integer, ByRef j As Integer) As Boolean

                If j < Int(((Capital_J * i) / (2 * Capital_I))) Then Return False
                If i > Utils.Rounding(((Capital_I * j) / (2 * Capital_J)) + (Capital_I / 2), Utils.roundingMethods.alwaysUp) Then Return False

                'If i < (Capital_I * j) / (2 * Capital_J) Then Return False
                'If j > (Capital_J * i) / (2 * Capital_I) + Capital_J / 2 Then Return False

                Return True

            End Function

            Private Class DTWPoint
                Public Value As Double
                Public ColumnIndex As Integer
                Public RowIndex As Integer
                Public History As New List(Of DTWPoint)

                Public TransitionDirection As TransitionDirections
                'Public IsVerticalWarp As Boolean = False
                'Public IsHorizontalWarp As Boolean = False

                'Public PreviousVerticalWarpingCells As SortedSet(Of String)
                'Public PreviousHorizontalWarpingCells As SortedSet(Of String)
                Public Sub New(ColumnIndex As Integer, RowIndex As Integer, Value As Double)
                    Me.Value = Value
                    Me.ColumnIndex = ColumnIndex
                    Me.RowIndex = RowIndex
                End Sub
            End Class

            Private Enum TransitionDirections
                Diagonal
                Vertical
                Horizontal
                NoDirection
                BlockedPoint
            End Enum

            Public BarkFilterSpinLock1 As New Threading.SpinLock
            ''' <summary>
            ''' Filters a set of FFT magnitudes a set of Bark filters.
            ''' </summary>
            ''' <param name="FftData"></param>
            ''' <param name="SampleRate"></param>
            ''' <param name="FilterOverlapRatio">The relative degree (allowed range is 0-0.99) of overlap between filters. If left empty, the bark filters will be positioned next to each other so that adjacent filters share cut-off frequencies.</param>
            ''' <param name="LowestIncludedFrequency">Lower included centre frequency.</param>
            ''' <param name="HighestIncludedFrequency">Highest included centre frequency.</param>
            ''' <param name="UsePowerSpectrum">If set to true, the frequency data is read from the power spectrum. If left to false, frequency data is read from the amplitude spectrum.</param>
            ''' <returns>Returns a SortedList where keys repressent window number and the values repressent arrays of Bandcount averaged filter levels, in descending order from lowest to highest frequency band.</returns>
            Private Function BarkFilter(ByRef FftData As FftData,
                                    ByVal SampleRate As Integer,
                                    Optional ByVal FilterOverlapRatio As Double = 0,
                                    Optional ByRef LowestIncludedFrequency As Double = 80,
                                    Optional ByRef HighestIncludedFrequency As Double = 8000,
                                    Optional ByRef CentreFrequencies As SortedSet(Of Single) = Nothing,
                                    Optional ByVal UsePowerSpectrum As Boolean = False,
                                    Optional ByVal UseImprovementsAfterSiB As Boolean = True) As SortedList(Of Integer, Single())

                Dim SpinLockTaken As Boolean = False

                Try

                    'Checking for invalid values of FilterOverlapRatio
                    If FilterOverlapRatio < 0 Then Throw New ArgumentException("Lowest allowed bark filter overlap ratio is 0")
                    If FilterOverlapRatio > 0.99 Then Throw New ArgumentException("Highest allowed bark filter overlap ratio is 0.99")

                    'Creating a list of included filter centre frequencies
                    If CentreFrequencies Is Nothing Then

                        'Adding a extra check with a spinlock to ensure that frequencies are only added by one thread ' Spinlock added 2018-04-14
                        BarkFilterSpinLock1.Enter(SpinLockTaken)

                        'Checking again
                        If CentreFrequencies Is Nothing Then 'Line added 2018-04-13. Speeds up processing by re-using the object. Should makes no difference, as long as CentreFrequencies is not modified externally.
                            CentreFrequencies = New SortedSet(Of Single)
                            CentreFrequencies.Add(LowestIncludedFrequency)
                            Do
                                Dim CurrentCentreFrequency As Single = CentreFrequencies(CentreFrequencies.Count - 1)
                                Dim CurrentBandWidth As Single = Utils.CenterFrequencyToBarkFilterBandwidth(CurrentCentreFrequency)

                                'Calculating the frequency of the next centre frequency by adding the band width of the previous filter to its centre frequency, and adjusting it to the right degree of overlap
                                Dim NextCentreFrequency As Single = CurrentCentreFrequency + CurrentBandWidth - (CurrentBandWidth * FilterOverlapRatio)

                                'Adding the new centre frequency if it is below the HighestIncludedFrequency, or exits the loop if the new centre frequency exceeds the HighestIncludedFrequency
                                If NextCentreFrequency < HighestIncludedFrequency Then
                                    CentreFrequencies.Add(NextCentreFrequency)
                                Else
                                    Exit Do
                                End If
                            Loop

                            'Releases the spinlock after CentreFrequencies have been created
                            If SpinLockTaken = True Then BarkFilterSpinLock1.Exit()

                        End If
                    End If

                    'Summing magnitudes into frequency bands
                    Dim SummedMagnitudesArray As New SortedList(Of Integer, Single())

                    'Looking at one time window at a time
                    For w = 0 To FftData.WindowCount(1) - 1

                        Dim BandMagnitudes(CentreFrequencies.Count - 1) As Single

                        'Collecting bin values for the current band
                        For CentreFrequencyIndex = 0 To CentreFrequencies.Count - 1

                            Dim CurrentCentreFrequency As Single = CentreFrequencies(CentreFrequencyIndex)
                            Dim CurrentBandWidth As Single = Utils.CenterFrequencyToBarkFilterBandwidth(CurrentCentreFrequency)
                            Dim LowerFrequencyLimit As Single = CurrentCentreFrequency - CurrentBandWidth 'Getting the lowest frequency that add loudness to the current critical band (This frequency is 0.5 bark below the lower cut-off frequency of the current critical band (Based on Zwicker and Fastl(1999), Phsycho-acoustics, p 164ff). (The filter approximates the auditory filter responce to noise, rather than pure tones is used.))
                            Dim HighestFrequencyLimit As Single = CurrentCentreFrequency + CurrentBandWidth 'Getting the highest frequency that add loudness to the current critical band (This frequency is 0.5 bark above the upper cut-off frequency of the current critical band (Based on Zwicker and Fastl(1999), Phsycho-acoustics, p 164ff). (The filter approximates the auditory filter responce to noise, rather than pure tones is used.))
                            Dim LowestIncludedBinIndex As Integer = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                                                                        LowerFrequencyLimit, SampleRate, FftData.FftFormat.FftWindowSize, Utils.roundingMethods.alwaysDown)
                            Dim HighestIncludedBinIndex As Integer = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                                                                        HighestFrequencyLimit, SampleRate, FftData.FftFormat.FftWindowSize, Utils.roundingMethods.alwaysUp)
                            Dim CentreBinIndex As Integer = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                                                                        CurrentCentreFrequency, SampleRate, FftData.FftFormat.FftWindowSize, Utils.roundingMethods.getClosestValue)

                            'Creating a triangular filter for the current critical band width
                            Dim BandBinCount As Integer = HighestIncludedBinIndex - LowestIncludedBinIndex
                            Dim myFilter(BandBinCount) As Single
                            Dim CurrentFilterIndex As Integer = 0

                            Dim UseAdaptiveFilter As Boolean = False
                            If UseAdaptiveFilter = True Then

                                'The follwing adaptive filter gets skewed to allways have one value which is 1 near the centre, and allways 0 at the end points, even if the array has an even length
                                For fb = LowestIncludedBinIndex To HighestIncludedBinIndex
                                    If fb < CentreBinIndex Then
                                        myFilter(CurrentFilterIndex) = (fb - LowestIncludedBinIndex) * (1 / (CentreBinIndex - LowestIncludedBinIndex))
                                        CurrentFilterIndex += 1
                                    ElseIf fb > CentreBinIndex Then
                                        myFilter(CurrentFilterIndex) = (HighestIncludedBinIndex - fb) * (1 / (HighestIncludedBinIndex - CentreBinIndex))
                                        CurrentFilterIndex += 1
                                    ElseIf fb = CentreBinIndex Then
                                        myFilter(CurrentFilterIndex) = 1
                                        CurrentFilterIndex += 1
                                    Else
                                        Throw New Exception("Something must be wrong! Check your code!")
                                    End If
                                Next
                            Else
                                For n = 0 To myFilter.Count - 1
                                    myFilter(n) = 1
                                Next

                                'Windowing to creat a triangual filter array
                                WindowingFunction(myFilter, WindowingType.Triangular)

                            End If


                            'Collecting magnitudes within the band
                            CurrentFilterIndex = 0

                            If UsePowerSpectrum = False Then

                                If UseImprovementsAfterSiB = True Then

                                    For fb = LowestIncludedBinIndex To HighestIncludedBinIndex

                                        'Checking that LowestIncludedBinIndex is not below 0
                                        If fb < 0 Then
                                            CurrentFilterIndex += 1
                                            Continue For
                                        End If

                                        'Checking that HighestIncludedBinIndex is not too high, due to always rounding bin index up. If so, it is simply skipped.
                                        If fb > FftData.AmplitudeSpectrum(1, 0).WindowData.Length - 1 Then Continue For

                                        Dim CurrentBandMagnitude = FftData.AmplitudeSpectrum(1, w).WindowData(fb)
                                        CurrentBandMagnitude = 2 * (100 ^ (Math.Log10(CurrentBandMagnitude)))

                                        BandMagnitudes(CentreFrequencyIndex) += myFilter(CurrentFilterIndex) * CurrentBandMagnitude
                                        CurrentFilterIndex += 1
                                    Next

                                    'Summing within the band, and compensating for the frequency overlap
                                    BandMagnitudes(CentreFrequencyIndex) = 2 * Math.Sqrt(BandMagnitudes(CentreFrequencyIndex) / (2 * (1 - FilterOverlapRatio))) ' Multiplied by 2 due to skipping of negative frequencies

                                Else

                                    'This is the original calculation, from the SiB-test study 
                                    For fb = LowestIncludedBinIndex To HighestIncludedBinIndex

                                        'Checking that LowestIncludedBinIndex is not below 0, due to always rounding bin index down. If so, it is simply skipped
                                        If fb < 0 Then
                                            Continue For
                                        End If

                                        'Checking that HighestIncludedBinIndex is not too high, due to always rounding bin index up. If so, it is simply skipped.
                                        If fb > FftData.AmplitudeSpectrum(1, 0).WindowData.Length - 1 Then Continue For

                                        BandMagnitudes(CentreFrequencyIndex) += myFilter(CurrentFilterIndex) * FftData.AmplitudeSpectrum(1, w).WindowData(fb)
                                        CurrentFilterIndex += 1
                                    Next

                                    'Averaging within the band
                                    BandMagnitudes(CentreFrequencyIndex) = BandMagnitudes(CentreFrequencyIndex) / myFilter.Count ' This is a somewhat harsh method of averaging since it doesn't take into account that the lowest and highest bands may be unavailable frequency bands. An alternative could be to divide by the number of actual filter indices used

                                End If

                            Else

                                If UseImprovementsAfterSiB = True Then

                                    For fb = LowestIncludedBinIndex To HighestIncludedBinIndex

                                        'Checking that LowestIncludedBinIndex is not below 0
                                        If fb < 0 Then
                                            CurrentFilterIndex += 1
                                            Continue For
                                        End If

                                        'Checking that HighestIncludedBinIndex is not too high
                                        If fb > FftData.PowerSpectrumData(1, 0).WindowData.Length - 1 Then Continue For

                                        BandMagnitudes(CentreFrequencyIndex) += myFilter(CurrentFilterIndex) * FftData.PowerSpectrumData(1, w).WindowData(fb) * 2 ' Multiplied by 2 due to skipping of negative frequencies
                                        CurrentFilterIndex += 1
                                    Next

                                    'Compensating for the frequency overlap
                                    BandMagnitudes(CentreFrequencyIndex) = BandMagnitudes(CentreFrequencyIndex) / (2 * (1 - FilterOverlapRatio))

                                Else

                                    For fb = LowestIncludedBinIndex To HighestIncludedBinIndex

                                        'Checking that LowestIncludedBinIndex is not below 0
                                        If fb < 0 Then
                                            CurrentFilterIndex += 1 'This line was added 2018-04-12. In the UsePowerSpectrum = False option (which was used when selecting SiB-test phoneme contrasts), the lack of this line means that the centre frequency of the filter gets moved upwards when fb < 0. 
                                            Continue For
                                        End If

                                        'Checking that HighestIncludedBinIndex is not too high
                                        If fb > FftData.PowerSpectrumData(1, 0).WindowData.Length - 1 Then Continue For

                                        BandMagnitudes(CentreFrequencyIndex) += myFilter(CurrentFilterIndex) * FftData.PowerSpectrumData(1, w).WindowData(fb)
                                        CurrentFilterIndex += 1
                                    Next

                                    'Averaging within the band
                                    BandMagnitudes(CentreFrequencyIndex) = BandMagnitudes(CentreFrequencyIndex) / myFilter.Count ' This is a somewhat harsh method of averaging since it doesn't take into account that the lowest and highest bands may be unavailable frequency bands. An alternative could be to divide by the number of actual filter indices used

                                End If

                            End If


                        Next
                        'Adding the band magnitude
                        SummedMagnitudesArray.Add(w, BandMagnitudes)
                    Next

                    Return SummedMagnitudesArray

                Catch ex As Exception
                    SendInfoToAudioLog(ex.ToString)
                    Return Nothing

                Finally

                    'Releases any spinlock
                    If BarkFilterSpinLock1.IsHeldByCurrentThread = True Then If SpinLockTaken = True Then BarkFilterSpinLock1.Exit()
                End Try

            End Function


            Private Sub SaveDTWPointMatrixToFile(ByRef DistanceMatrix(,) As DTWPoint, Optional ByVal FilePath As String = "")

                If FilePath = "" Then FilePath = Utils.GetSaveFilePath()

                Dim SaveFolder As String = Path.GetDirectoryName(FilePath)
                If Not Directory.Exists(SaveFolder) Then Directory.CreateDirectory(SaveFolder)

                Dim writer As New IO.StreamWriter(FilePath)

                For Row_j = 0 To DistanceMatrix.GetUpperBound(1)

                    Dim CurrentRow As String = ""

                    For Column_i = 0 To DistanceMatrix.GetUpperBound(0)

                        CurrentRow &= DistanceMatrix(Column_i, Row_j).Value & vbTab

                    Next

                    writer.WriteLine(CurrentRow)

                Next

                writer.Close()

            End Sub


            Public Enum AcousticDistanceTypes
                LinearCorrelation
                ManhattanDistance
                EuclideanDistance
            End Enum


            Private Function GetDistanceValue(ByRef AmplitudeSpectrum1 As SortedList(Of Integer, Single()), ByRef AmplitudeSpectrum2 As SortedList(Of Integer, Single()),
                                             ByVal ColumnIndex As Integer, ByVal RowIndex As Integer) As Double

                'Calculating the Euclidean distance

                Dim Sum As Double = 0
                For n = 0 To AmplitudeSpectrum1(0).Length - 1
                    Sum += (AmplitudeSpectrum1(ColumnIndex)(n) - AmplitudeSpectrum2(RowIndex)(n)) ^ 2
                Next

                Return Math.Sqrt(Sum)

            End Function

        End Module


        Public Module Loudness_ModelA

            Public Enum ReturnTypes
                Loudness
                LoudnessLevel
            End Enum


            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="Signal"></param>
            ''' <param name="MeasurementWindowDuration"></param>
            ''' <param name="MeasurementWindowOverlapDuration"></param>
            ''' <param name="BarkFilterOverlapRatio"></param>
            ''' <param name="LowestIncludedCentreFrequency"></param>
            ''' <param name="HighestIncludedCentreFrequency"></param>
            ''' <param name="MatrixOutputFolder"></param>
            ''' <param name="ExportDetails"></param>
            ''' <param name="InactivateFftWarnings"></param>
            ''' <param name="CurrentIsoPhonFilter"></param>
            ''' <param name="CurrentAuditoryFilters"></param>
            ''' <param name="CurrentSpreadOfMaskingFilters"></param>
            ''' <param name="dbFSToSplDifference"></param>
            ''' <param name="CurrentBandTemplateList"></param>
            ''' <param name="SoneScalingFactor"></param>
            ''' <returns></returns>
            Public Function GetLoudness(ByRef Signal As Sound,
                                            Optional ByVal MeasurementWindowDuration As Double = 0.1, '0.17,
                                            Optional ByVal MeasurementWindowOverlapDuration As Double = 0.09, '0.16,
                                            Optional ByVal BarkFilterOverlapRatio As Double = 0,
                                            Optional ByRef LowestIncludedCentreFrequency As Double = 80,
                                            Optional ByRef HighestIncludedCentreFrequency As Double = 12700, 'Previously 17500, but limited due to the limited band width of the iso-phon filter
                                            Optional ByVal MatrixOutputFolder As String = "",
                                            Optional ByVal ExportDetails As Boolean = False,
                                            Optional ByRef InactivateFftWarnings As Boolean = False,
                                            Optional ByRef CurrentIsoPhonFilter As IsoPhonFilter = Nothing,
                                            Optional ByRef CurrentAuditoryFilters As FftData.AuditoryFilters = Nothing,
                                            Optional ByRef CurrentSpreadOfMaskingFilters As FftData.SpreadOfMaskingFilters = Nothing,
                                            Optional ByRef CurrentBandTemplateList As FftData.BarkSpectrum.BandTemplateList = Nothing,
                                    Optional ByRef CurrentFftFormat As Formats.FftFormat = Nothing,
                                    Optional ByRef ReturnType As ReturnTypes = ReturnTypes.Loudness,
                                    Optional ByRef dbFSToSplDifference As Double = 88,
                                    Optional ByRef LoudnessFunction As FftData.LoudnessFunctions = FftData.LoudnessFunctions.ZwickerType,
                                    Optional ByRef SoneScalingFactor As Double? = Nothing) As Double()

                Try
                    'Setting up FFT formats
                    If CurrentFftFormat Is Nothing Then

                        Dim MeasurementWindowLength As Integer = Signal.WaveFormat.SampleRate * MeasurementWindowDuration
                        If MeasurementWindowLength Mod 2 = 1 Then MeasurementWindowLength += 1

                        Dim MeasurementWindowOverlapLength As Integer = Signal.WaveFormat.SampleRate * MeasurementWindowOverlapDuration
                        If MeasurementWindowOverlapLength Mod 2 = 1 Then MeasurementWindowOverlapLength += 1
                        Dim SpectralResolution As Integer = 2048 * 2 * 2
                        CurrentFftFormat = New Formats.FftFormat(MeasurementWindowLength, SpectralResolution, MeasurementWindowOverlapLength, WindowingType.Tukey, InactivateFftWarnings)
                    End If

                    'Creating a MatrixOutputFolder if it doesn't exist
                    Directory.CreateDirectory(MatrixOutputFolder)

                    'Only allowing mono sounds
                    If Signal.WaveFormat.Channels <> 1 Then Throw New Exception("The current function only supports mono sounds.")

                    'No initial zero padding! Zero padding is unnecessary here, since no temporal alignment is needed, and a hearing threshold is used. However, the end of the sound will be zero padded by the spectral analysis function.

                    'Modifying the signal
                    'Adding one fft window length of zero padding, to avoid sound final fading effects. This last bit is then removed from the SignalBarkSpectrumArray before the distance calculation (and before the matrix logging).
                    Dim OriginalSignalLengths As New List(Of Double)
                    For c = 1 To Signal.WaveFormat.Channels
                        OriginalSignalLengths.Add(Signal.WaveData.SampleData(c).Length)
                        ReDim Preserve Signal.WaveData.SampleData(c)(Signal.WaveData.SampleData(c).Length + CurrentFftFormat.FftWindowSize)
                    Next

                    'Getting frequency domain data
                    Signal.FFT = Nothing 'Resetting any previously calculated frequency domain data
                    Signal.FFT = SpectralAnalysis(Signal, CurrentFftFormat)

                    'Resetting the signal by removing the zero padding added above
                    For c = 1 To Signal.WaveFormat.Channels
                        ReDim Preserve Signal.WaveData.SampleData(c)(OriginalSignalLengths(c - 1))
                    Next

                    'Calculating power spectrum
                    Signal.FFT.CalculatePowerSpectrum(True, True, True, 0.25)

                    'Getting bark filter spectrum for the signal
                    Signal.FFT.CalculateBarkSpectrum(Signal, 1, CurrentIsoPhonFilter, CurrentAuditoryFilters, CurrentSpreadOfMaskingFilters,
                       BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency,
                       dbFSToSplDifference, LoudnessFunction, CurrentBandTemplateList, SoneScalingFactor)

                    'Referencing the Bark spectra
                    Dim SignalBarkSpectrumArray As SortedList(Of Integer, Single()) = Signal.FFT.BarkSpectrumData(1)

                    'Removing the last time windows repressenting the period of final zero padding above
                    For n = 0 To Int(CurrentFftFormat.FftWindowSize / (CurrentFftFormat.AnalysisWindowSize - CurrentFftFormat.OverlapSize)) - 1
                        SignalBarkSpectrumArray.RemoveAt(SignalBarkSpectrumArray.Count - 1)
                    Next

                    'Exporting stuff
                    If ExportDetails = True And MatrixOutputFolder <> "" Then

                        CurrentIsoPhonFilter.ExportIsoPhonCurves(MatrixOutputFolder)
                        CurrentIsoPhonFilter.ExportInverseIsoPhonCurves(MatrixOutputFolder,, False)
                        CurrentIsoPhonFilter.ExportInverseIsoPhonCurves(MatrixOutputFolder, "InversePhonData_Att", True)
                        CurrentIsoPhonFilter.ExportSplToPhonData(MatrixOutputFolder,, 10)

                        'Creating a matrix of band loudness levels, and exporting it
                        Dim BandLoudnessLevels As New SortedList(Of Integer, Single())
                        For TimeWindow = 0 To SignalBarkSpectrumArray.Count - 1 'Using SignalBarkSpectrumArray.Count since this is already cut at the end to skip the zero padding bit.
                            Dim CurrentBarkBandArray(Signal.FFT.BarkSpectrumData(1).MyDetailedList(TimeWindow).Count - 1) As Single
                            For BarkBand = 0 To Signal.FFT.BarkSpectrumData(1).MyDetailedList(TimeWindow).Count - 1
                                CurrentBarkBandArray(BarkBand) = Signal.FFT.BarkSpectrumData(1).MyDetailedList(TimeWindow)(BarkBand).BandLoudnessLevel
                            Next
                            BandLoudnessLevels.Add(TimeWindow, CurrentBarkBandArray)
                        Next
                        Dim newDoubleArray1(BandLoudnessLevels.Count - 1, BandLoudnessLevels(0).Length - 1) As Double
                        For p = 0 To BandLoudnessLevels.Count - 1
                            For q = 0 To BandLoudnessLevels(0).Length - 1
                                newDoubleArray1(p, q) = BandLoudnessLevels(p)(q)
                            Next
                        Next
                        Utils.SaveMatrixToFile(newDoubleArray1, IO.Path.Combine(MatrixOutputFolder, "BarkBandLoudnessLevels_" & Signal.FileName & ".txt"))

                        Dim newDoubleArray2(SignalBarkSpectrumArray.Count - 1, SignalBarkSpectrumArray(0).Length - 1) As Double
                        For p = 0 To SignalBarkSpectrumArray.Count - 1
                            For q = 0 To SignalBarkSpectrumArray(0).Length - 1
                                newDoubleArray2(p, q) = SignalBarkSpectrumArray(p)(q)
                            Next
                        Next
                        Utils.SaveMatrixToFile(newDoubleArray2, IO.Path.Combine(MatrixOutputFolder, "BarkBandLoudness_" & Signal.FileName & ".txt"))
                    End If


                    Dim TimeWindowLoudnessArray(SignalBarkSpectrumArray.Count - 1) As Double

                    'Summing band loudness, and converting to total loudness level
                    For TimeWindowIndex = 0 To SignalBarkSpectrumArray.Count - 1
                        Dim TotalLoudness As Double = SignalBarkSpectrumArray(TimeWindowIndex).Sum
                        Select Case ReturnType
                            Case ReturnTypes.Loudness

                                TimeWindowLoudnessArray(TimeWindowIndex) = TotalLoudness
                            Case ReturnTypes.LoudnessLevel

                                Select Case LoudnessFunction
                                    Case FftData.LoudnessFunctions.ZwickerType
                                        'Converting to loudness level
                                        If TotalLoudness < 1 Then
                                            TimeWindowLoudnessArray(TimeWindowIndex) = 40 * TotalLoudness ^ 0.35
                                        Else
                                            TimeWindowLoudnessArray(TimeWindowIndex) = 40 + 10 * Utils.getBase_n_Log(TotalLoudness, 2)
                                        End If

                                    Case FftData.LoudnessFunctions.Simple
                                        'Converting to loudness level
                                        TimeWindowLoudnessArray(TimeWindowIndex) = 40 + 10 * Utils.getBase_n_Log(TotalLoudness, 2)

                                    Case FftData.LoudnessFunctions.InExType
                                        MsgBox("Using ZwickerType loudness function. (Haven't been able to invert InExType)")
                                        'Converting to loudness level
                                        If TotalLoudness < 1 Then
                                            TimeWindowLoudnessArray(TimeWindowIndex) = 40 * TotalLoudness ^ 0.35
                                        Else
                                            TimeWindowLoudnessArray(TimeWindowIndex) = 40 + 10 * Utils.getBase_n_Log(TotalLoudness, 2)
                                        End If

                                End Select

                            Case Else
                                Throw New NotImplementedException("Invalid return type.")
                        End Select
                    Next

                    Return TimeWindowLoudnessArray


                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function



            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="Sound1"></param>
            ''' <param name="Sound2"></param>
            ''' <param name="MeasurementWindowDuration"></param>
            ''' <param name="MeasurementWindowOverlapDuration"></param>
            ''' <param name="BarkFilterOverlapRatio"></param>
            ''' <param name="LowestIncludedCentreFrequency"></param>
            ''' <param name="HighestIncludedCentreFrequency"></param>
            ''' <param name="MatrixOutputFolder"></param>
            ''' <param name="ExportDetails"></param>
            ''' <param name="InactivateFftWarnings"></param>
            '''<param name="Sound1Length">A length value of sound1 that can be used to override the actual sound length in calculating the length ratio between the sounds. If left to Nothing, the actual length of the input sound1 will be used.</param>
            '''<param name="Sound2Length">A length value of sound2 that can be used to override the actual sound length in calculating the length ratio between the sounds. If left to Nothing, the actual length of the input sound2 will be used.</param>
            ''' <returns></returns>
            Public Function GetAcousticDistance(ByRef Sound1 As Sound, ByRef Sound2 As Sound,
                                            Optional ByVal MeasurementWindowDuration As Double = 0.1,
                                            Optional ByVal MeasurementWindowOverlapDuration As Double = 0.095,
                                            Optional ByVal BarkFilterOverlapRatio As Double = 0.9,
                                            Optional ByRef LowestIncludedCentreFrequency As Double = 80,
                                            Optional ByRef HighestIncludedCentreFrequency As Double = 12700, 'Previously 17500, but limited due to the limited band width of the iso-phon filter
                                            Optional ByVal MatrixOutputFolder As String = "",
                                            Optional ByVal ExportDetails As Boolean = False,
                                            Optional ByRef InactivateFftWarnings As Boolean = False,
                                            Optional ByVal Sound1Length As Double? = Nothing,
                                            Optional ByVal Sound2Length As Double? = Nothing,
                                            Optional ByRef CurrentIsoPhonFilter As IsoPhonFilter = Nothing,
                                            Optional ByRef CurrentAuditoryFilters As FftData.AuditoryFilters = Nothing,
                                            Optional ByRef CurrentSpreadOfMaskingFilters As FftData.SpreadOfMaskingFilters = Nothing,
                                            Optional ByRef dbFSToSplDifference As Double = 88,
                                            Optional ByRef LoudnessFunction As FftData.LoudnessFunctions = FftData.LoudnessFunctions.ZwickerType,
                                            Optional ByRef CurrentBandTemplateList As FftData.BarkSpectrum.BandTemplateList = Nothing,
                                            Optional ByRef SoneScalingFactor As Double? = Nothing,
                                            Optional ByVal DoDynamicTimeWarping As Boolean = True) As Double

                Try
                    Dim FileComparisonID As String = Sound1.FileName & "_" & Sound2.FileName

                    'Setting up FFT formats
                    Dim MeasurementWindowLength As Integer = Sound1.WaveFormat.SampleRate * MeasurementWindowDuration
                    If MeasurementWindowLength Mod 2 = 1 Then MeasurementWindowLength += 1

                    Dim MeasurementWindowOverlapLength As Integer = Sound1.WaveFormat.SampleRate * MeasurementWindowOverlapDuration
                    If MeasurementWindowOverlapLength Mod 2 = 1 Then MeasurementWindowOverlapLength += 1
                    Dim SpectralResolution As Integer = 2048 * 2 * 2
                    Dim AD_FftFormat As New Formats.FftFormat(MeasurementWindowLength, SpectralResolution, MeasurementWindowOverlapLength, WindowingType.Hamming, InactivateFftWarnings)


                    'Creating a MatrixOutputFolder if it doesn't exist
                    Directory.CreateDirectory(MatrixOutputFolder)

                    Dim OutputOnlyTemporalDistance As Boolean = False
                    If OutputOnlyTemporalDistance Then
                        Dim DurationRatio As Double = Math.Min(Sound1.WaveData.ShortestChannelSampleCount, Sound2.WaveData.ShortestChannelSampleCount) /
                Math.Max(Sound1.WaveData.ShortestChannelSampleCount, Sound2.WaveData.ShortestChannelSampleCount)

                        If DurationRatio > 0 Then
                            DurationRatio = 1 / DurationRatio
                            Return DurationRatio
                        Else
                            Return Single.PositiveInfinity
                        End If

                    End If

                    'Only allowing mono sounds
                    If Sound1.WaveFormat.Channels <> 1 Or Sound2.WaveFormat.Channels <> 1 Then Throw New Exception("The current function only supports mono sounds.")

                    'Checking that the sounds have the same format
                    If Sound1.WaveFormat.BitDepth <> Sound2.WaveFormat.BitDepth Or
            Sound1.WaveFormat.SampleRate <> Sound2.WaveFormat.SampleRate Or
            Sound1.WaveFormat.Encoding <> Sound2.WaveFormat.Encoding Then Throw New Exception("Different formats in input sounds.")


                    'Zeropadding the input sounds, so that they get MeasurementWindowOverlapLength before the sound. After the sound, the spectral analysis function adds zero padding automatically
                    If Sound1.FFT Is Nothing Then Sound1.ZeroPad(MeasurementWindowOverlapLength, Nothing, False) 'Only zero padding if FFT is nothing, which means that it hasn't been here before
                    Sound2.ZeroPad(MeasurementWindowOverlapLength, Nothing, False)


                    'Getting frequency domain data (Only if it doesn't allready exist)
                    If Sound1.FFT Is Nothing Then
                        Sound1.FFT = SpectralAnalysis(Sound1, AD_FftFormat)
                        'Calculating spectra
                        Sound1.FFT.CalculatePowerSpectrum(True, True, True, 0.25)

                        'Getting bark filter spectrum for sound 1
                        'Calculating new Bark spectrum
                        Sound1.FFT.CalculateBarkSpectrum(Sound1, 1, CurrentIsoPhonFilter, CurrentAuditoryFilters, CurrentSpreadOfMaskingFilters,
                       BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency,
                       dbFSToSplDifference, LoudnessFunction, CurrentBandTemplateList, SoneScalingFactor)

                    End If

                    If Sound2.FFT Is Nothing Then
                        Sound2.FFT = SpectralAnalysis(Sound2, AD_FftFormat)
                        'Calculating spectra
                        Sound2.FFT.CalculatePowerSpectrum(True, True, True, 0.25)

                        'Getting bark filter spectrum for sound 2
                        'Calculating new Bark spectrum
                        Sound2.FFT.CalculateBarkSpectrum(Sound2, 1, CurrentIsoPhonFilter, CurrentAuditoryFilters, CurrentSpreadOfMaskingFilters,
                       BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency,
                       dbFSToSplDifference, LoudnessFunction, CurrentBandTemplateList, SoneScalingFactor)

                    End If

                    Throw New NotImplementedException("This need to convert the Sone values to Phons in order to use Eucledian distance!?")

                    'Exporting stuff
                    If ExportDetails = True And MatrixOutputFolder <> "" Then
                        Dim FilterredBandPowerArray1 As SortedList(Of Integer, Single()) = Sound1.FFT.BarkSpectrumData(1)
                        Dim newDoubleArray(FilterredBandPowerArray1.Count - 1, FilterredBandPowerArray1(0).Length - 1) As Double
                        For p = 0 To FilterredBandPowerArray1.Count - 1
                            For q = 0 To FilterredBandPowerArray1(0).Length - 1
                                newDoubleArray(p, q) = FilterredBandPowerArray1(p)(q)
                            Next
                        Next
                        Utils.SaveMatrixToFile(newDoubleArray, IO.Path.Combine(MatrixOutputFolder, "BarkSpectrum_" & FileComparisonID & "A.txt"))

                        Dim FilterredBandPowerArray2 As SortedList(Of Integer, Single()) = Sound2.FFT.BarkSpectrumData(1)
                        Dim newDoubleArray2(FilterredBandPowerArray2.Count - 1, FilterredBandPowerArray2(0).Length - 1) As Double
                        For p = 0 To FilterredBandPowerArray2.Count - 1
                            For q = 0 To FilterredBandPowerArray2(0).Length - 1
                                newDoubleArray2(p, q) = FilterredBandPowerArray2(p)(q)
                            Next
                        Next
                        Utils.SaveMatrixToFile(newDoubleArray2, IO.Path.Combine(MatrixOutputFolder, "BarkSpectrum_" & FileComparisonID & "B.txt"))

                    End If


                    'Calculating distance
                    Dim AcousticDistance As Double
                    If DoDynamicTimeWarping = True Then
                        AcousticDistance = CalculateTimeWarpedAcousticDistance(Sound1, Sound2, MatrixOutputFolder, FileComparisonID, ExportDetails)

                    Else
                        Throw New NotImplementedException

                    End If


                    Dim OutputTimeWeightedDistance As Boolean = True

                    If OutputTimeWeightedDistance Then
                        If Sound1Length Is Nothing Then Sound1Length = CDbl(Sound1.WaveData.ShortestChannelSampleCount)
                        If Sound2Length Is Nothing Then Sound2Length = CDbl(Sound2.WaveData.ShortestChannelSampleCount)

                        Dim DurationRatio As Double = Math.Min(CDbl(Sound1Length), CDbl(Sound2Length)) / Math.Max(CDbl(Sound1Length), CDbl(Sound2Length))

                        Dim TimeWeightedAcousticDistance As Double
                        If DurationRatio > 0 Then
                            TimeWeightedAcousticDistance = AcousticDistance / DurationRatio
                        Else
                            TimeWeightedAcousticDistance = 9999 ' Single.PositiveInfinity
                        End If

                        Return TimeWeightedAcousticDistance
                    Else
                        Return AcousticDistance
                    End If

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function

            ''' <summary>
            ''' Calculates acoustic distance based on dynamic time warping.  For reference to the principles behind this code, see Jurafsky and Martin (p 333), and Gold, Morgan, Ellis (pp 340).
            ''' </summary>
            ''' <returns></returns>
            Private Function CalculateTimeWarpedAcousticDistance(ByRef Sound1 As Sound,
                                                             ByRef Sound2 As Sound,
                                                             ByVal MatrixOutputFolder As String,
                                                             ByVal FileComparisonID As String,
                                                             ByVal ExportDetails As Boolean) As Double

                Try

                    Dim DistanceType As AcousticDistanceTypes = AcousticDistanceTypes.EuclideanDistance
                    Dim LocalRestrictions As Boolean = True
                    Dim DiagonalWeight As Double = 1 ' 1 is used instead of 2 here, since doubling the cost biases the selected path through the distance matrix to deviate away from the centre/diagonal, which in my opinion should be undesirable.
                    Dim HorizontalWeight As Double = 1
                    Dim VerticalWeight As Double = 1


                    'Setting up a distance matrix containing the distances between every time window in the two input sounds
                    Dim FilterredBandPowerArray1 As SortedList(Of Integer, Single()) = Sound1.FFT.BarkSpectrumData(1)
                    Dim FilterredBandPowerArray2 As SortedList(Of Integer, Single()) = Sound2.FFT.BarkSpectrumData(1)
                    Dim ColumnCount As Integer = FilterredBandPowerArray1.Count
                    Dim RowCount As Integer = FilterredBandPowerArray2.Count
                    Dim DistanceMatrix(ColumnCount - 1, RowCount - 1) As Double

                    'Measuring distances between time windows. 
                    For Column_i = 0 To ColumnCount - 1
                        For Row_j = 0 To RowCount - 1

                            If LocalRestrictions = True Then
                                'Blocking windows which should not be measured, and just putting Double.Maxvalue in them.
                                If IsWithinMeasurementRegion(ColumnCount - 1, Column_i, RowCount - 1, Row_j) = False Then
                                    Continue For
                                End If
                            End If

                            'Getting the distance 
                            DistanceMatrix(Column_i, Row_j) = GetDistanceValue(FilterredBandPowerArray1, FilterredBandPowerArray2, Column_i, Row_j, DistanceType)

                        Next
                    Next

                    'Saving matrix to file
                    If ExportDetails = True And MatrixOutputFolder <> "" Then
                        Utils.SaveMatrixToFile(DistanceMatrix, IO.Path.Combine(MatrixOutputFolder, FileComparisonID & "_Distance.txt"))
                    End If

                    'Doing dynamic time warping into a new table, the first window is only used for initial comparison (as if it repressented sound prior to the first window), whereby the matrix need to be,
                    Dim DtwMatrix(ColumnCount - 1, RowCount - 1) As DTWPoint

                    'Filling up the DtwMatrix
                    For Column_i = 0 To ColumnCount - 1
                        For Row_j = 0 To RowCount - 1
                            DtwMatrix(Column_i, Row_j) = New DTWPoint(Column_i, Row_j, 0)

                            If LocalRestrictions = True Then
                                'Blocking windows are marked by
                                If IsWithinMeasurementRegion(ColumnCount - 1, Column_i, RowCount - 1, Row_j) = False Then
                                    DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.BlockedPoint
                                End If
                            End If

                        Next
                    Next


                    'Calculating symmetric type 0 DTW (For reference, see Sakoe and Chiba 1978, Optimization for spoken word recognition). Adapted with a restriction that warping is only allowed if the previous value wasn't created y a warping
                    For Column_i = 0 To ColumnCount - 1
                        For Row_j = 0 To RowCount - 1

                            'Skipping measurements outside the measurement window
                            If DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.BlockedPoint Then
                                Continue For
                            End If

                            'Declaring blockers
                            Dim BlockHorizontalMove As Boolean = False
                            Dim BlockVerticalMove As Boolean = False

                            'Blocking moves from anything to the left of the first column
                            If Column_i = 0 Then
                                BlockHorizontalMove = True
                            End If

                            'Blocking moves from anything above the first row
                            If Row_j = 0 Then
                                BlockVerticalMove = True
                            End If


                            'Activating local restrictions
                            If LocalRestrictions = True Then

                                'Direction restrictions, blocks the horizontal or vertical directions if the horizontally or vertically preceding cell is it self derived from a horizontal or vertical direction
                                If Column_i > 0 Then
                                    If DtwMatrix(Column_i - 1, Row_j).TransitionDirection = TransitionDirections.Horizontal Then
                                        BlockHorizontalMove = True
                                    End If
                                End If

                                If Row_j > 0 Then
                                    If DtwMatrix(Column_i, Row_j - 1).TransitionDirection = TransitionDirections.Vertical Then
                                        BlockVerticalMove = True
                                    End If
                                End If
                            End If

                            'Getting the possible transition sums
                            Dim DiagonalPathValue As Double = Double.PositiveInfinity
                            If Column_i > 0 And Row_j > 0 Then
                                If Not DtwMatrix(Column_i - 1, Row_j - 1).TransitionDirection = TransitionDirections.BlockedPoint Then
                                    DiagonalPathValue = DtwMatrix(Column_i - 1, Row_j - 1).Value + DiagonalWeight * DistanceMatrix(Column_i, Row_j)
                                End If
                            End If

                            Dim VerticalPathValue As Double = Double.PositiveInfinity
                            If Row_j > 0 Then
                                If Not DtwMatrix(Column_i, Row_j - 1).TransitionDirection = TransitionDirections.BlockedPoint Then
                                    VerticalPathValue = DtwMatrix(Column_i, Row_j - 1).Value + VerticalWeight * DistanceMatrix(Column_i, Row_j)
                                End If
                            End If

                            Dim HorizintalPathValue As Double = Double.PositiveInfinity
                            If Column_i > 0 Then
                                If Not DtwMatrix(Column_i - 1, Row_j).TransitionDirection = TransitionDirections.BlockedPoint Then
                                    HorizintalPathValue = DtwMatrix(Column_i - 1, Row_j).Value + HorizontalWeight * DistanceMatrix(Column_i, Row_j)
                                End If
                            End If

                            'Selecting the best path
                            If Column_i = 0 And Row_j = 0 Then
                                'The special case of position 0,0
                                'No move at all, just adding the distance value
                                DtwMatrix(Column_i, Row_j).Value = DiagonalWeight * DistanceMatrix(Column_i, Row_j)
                                DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.NoDirection


                            ElseIf (DiagonalPathValue < VerticalPathValue And DiagonalPathValue < HorizintalPathValue) Or
                                (BlockVerticalMove = True And DiagonalPathValue < HorizintalPathValue) Or
                                (BlockHorizontalMove = True And DiagonalPathValue < VerticalPathValue) Or
                                (BlockVerticalMove = True And BlockHorizontalMove = True And DiagonalPathValue < Double.PositiveInfinity) Or
                                (DiagonalPathValue = 0 Or (DiagonalPathValue = VerticalPathValue And DiagonalPathValue = HorizintalPathValue)) Then 'N.B. the preferred path is always diagonal if the DiagonalPathValue = 0, or if all path values are equal (which would mean that we're in a silent window.)

                                'A diagonal move
                                DtwMatrix(Column_i, Row_j).Value = DiagonalPathValue
                                DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.Diagonal

                                'Storing history from the diagonally preceding cell
                                For Each PrecedingItem In DtwMatrix(Column_i - 1, Row_j - 1).History
                                    DtwMatrix(Column_i, Row_j).History.Add(PrecedingItem)
                                Next


                            ElseIf BlockVerticalMove = False And
                            (VerticalPathValue < HorizintalPathValue Or (BlockHorizontalMove = True And VerticalPathValue < Double.PositiveInfinity)) Then

                                'A vertical move
                                DtwMatrix(Column_i, Row_j).Value = VerticalPathValue
                                DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.Vertical

                                'Storing history from the vertically preceding cell
                                For Each PrecedingItem In DtwMatrix(Column_i, Row_j - 1).History
                                    DtwMatrix(Column_i, Row_j).History.Add(PrecedingItem)
                                Next


                            ElseIf BlockHorizontalMove = False And
                            (HorizintalPathValue < VerticalPathValue Or (BlockVerticalMove = True And HorizintalPathValue < Double.PositiveInfinity)) Then

                                'A horizontal move
                                DtwMatrix(Column_i, Row_j).Value = HorizintalPathValue
                                DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.Horizontal

                                'Storing history from the horizontally preceding cell
                                For Each PrecedingItem In DtwMatrix(Column_i - 1, Row_j).History
                                    DtwMatrix(Column_i, Row_j).History.Add(PrecedingItem)
                                Next


                                'Overriding blockings
                            ElseIf DiagonalPathValue = Double.PositiveInfinity And BlockVerticalMove = True And HorizintalPathValue = Double.PositiveInfinity Then

                                'Overriding the vertical block since it is the only available value (this happens when conflicts occur between area and direction blockings)
                                'A vertical move
                                DtwMatrix(Column_i, Row_j).Value = VerticalPathValue
                                DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.Vertical

                                'Storing history from the vertically preceding cell
                                For Each PrecedingItem In DtwMatrix(Column_i, Row_j - 1).History
                                    DtwMatrix(Column_i, Row_j).History.Add(PrecedingItem)
                                Next

                            ElseIf (DiagonalPathValue = Double.PositiveInfinity And VerticalPathValue = Double.PositiveInfinity And BlockHorizontalMove = True) Then

                                'Overriding the horizontal block since it is the only available value (this happens when conflicts occur between area and direction blockings)
                                'A horizontal move
                                DtwMatrix(Column_i, Row_j).Value = HorizintalPathValue
                                DtwMatrix(Column_i, Row_j).TransitionDirection = TransitionDirections.Horizontal

                                'Storing history from the horizontally preceding cell
                                For Each PrecedingItem In DtwMatrix(Column_i - 1, Row_j).History
                                    DtwMatrix(Column_i, Row_j).History.Add(PrecedingItem)
                                Next

                            Else
                                MsgBox("Something Is wrong!")

                            End If

                            'Adding the current cell to it's own history
                            DtwMatrix(Column_i, Row_j).History.Add(DtwMatrix(Column_i, Row_j))

                        Next
                    Next


                    'Exporting the history of the "winner"
                    Dim HistoryOutputList As New List(Of String)
                    If ExportDetails = True And MatrixOutputFolder <> "" Then
                        For Each CurrentPTWPoint In DtwMatrix(ColumnCount - 1, RowCount - 1).History
                            HistoryOutputList.Add(CurrentPTWPoint.ColumnIndex & ", " & CurrentPTWPoint.RowIndex & vbCrLf)
                        Next
                        Utils.SaveListOfStringToTxtFile(HistoryOutputList, MatrixOutputFolder, FileComparisonID & "_WinnerHistory")
                    End If


                    'Saving matrix to file
                    If ExportDetails = True And MatrixOutputFolder <> "" Then
                        SaveDTWPointMatrixToFile(DtwMatrix, IO.Path.Combine(MatrixOutputFolder, FileComparisonID & "_DTW.txt"))
                        'SaveMatrixToFile(DtwMatrix, IO.Path.Combine(MatrixOutputFolder, FileComparisonID & "_Restrictions.txt"), 1)
                    End If

                    Dim DoNormalixation As Boolean = True
                    If DoNormalixation = True Then

                        'Normalizes the result by the number of steps that 
                        'MsgBox("HistoryOutputList.Count: " & HistoryOutputList.Count & " A+B: " & ColumnCount - 1 + RowCount - 1)
                        'Return (DtwMatrix(ColumnCount - 1, RowCount - 1).Value) / HistoryOutputList.Count

                        'This is the type of normalization of White and Neely, as reported in Sakoe and Chiba 1978, Optimization for spoken word recognition
                        Return (DtwMatrix(ColumnCount - 1, RowCount - 1).Value) / (ColumnCount - 1 + RowCount - 1)
                    Else
                        Return (DtwMatrix(ColumnCount - 1, RowCount - 1).Value)
                    End If

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function

            Private Function IsWithinMeasurementRegion(ByRef Capital_I As Integer, ByRef i As Integer,
                                                   ByRef Capital_J As Integer, ByRef j As Integer) As Boolean

                If IsBelowBlockingAreaBoundary(Capital_I, i, Capital_J, j) = False Then Return False
                If IsRightOfBlockingAreaBoundary(Capital_I, i, Capital_J, j) = False Then Return False

                'If i < (Capital_I * j) / (2 * Capital_J) Then Return False
                'If i > (Capital_I * j) / (2 * Capital_J) + Capital_I / 2 Then Return False
                'If j < (Capital_J * i) / (2 * Capital_I) Then Return False
                'If j > (Capital_J * i) / (2 * Capital_I) + Capital_J / 2 Then Return False

                Return True

            End Function

            Private Function IsBelowBlockingAreaBoundary(ByRef Capital_I As Integer, ByRef i As Integer,
                                                     ByRef Capital_J As Integer, ByRef j As Integer) As Boolean

                'If i > (Capital_I * j) / (2 * Capital_J) + Capital_I / 2 Then Return False
                'If j < (Capital_J * i) / (2 * Capital_I) Then Return False

                If i < Int(((Capital_I * j) / (2 * Capital_J))) Then Return False
                If j > Utils.Rounding(((Capital_J * i) / (2 * Capital_I)) + (Capital_J / 2), Utils.roundingMethods.alwaysUp) Then Return False

                Return True

            End Function

            Private Function IsRightOfBlockingAreaBoundary(ByRef Capital_I As Integer, ByRef i As Integer,
                                                       ByRef Capital_J As Integer, ByRef j As Integer) As Boolean

                If j < Int(((Capital_J * i) / (2 * Capital_I))) Then Return False
                If i > Utils.Rounding(((Capital_I * j) / (2 * Capital_J)) + (Capital_I / 2), Utils.roundingMethods.alwaysUp) Then Return False

                'If i < (Capital_I * j) / (2 * Capital_J) Then Return False
                'If j > (Capital_J * i) / (2 * Capital_I) + Capital_J / 2 Then Return False

                Return True

            End Function

            Private Class DTWPoint
                Public Value As Double
                Public ColumnIndex As Integer
                Public History As New List(Of DTWPoint)
                Public RowIndex As Integer

                Public TransitionDirection As TransitionDirections

                'Public PreviousVerticalWarpingCells As SortedSet(Of String)
                'Public PreviousHorizontalWarpingCells As SortedSet(Of String)
                Public Sub New(ColumnIndex As Integer, RowIndex As Integer, Value As Double)
                    Me.Value = Value
                    Me.ColumnIndex = ColumnIndex
                    Me.RowIndex = RowIndex
                End Sub
            End Class

            Private Enum TransitionDirections
                Diagonal
                Vertical
                Horizontal
                NoDirection
                BlockedPoint
            End Enum


            Private Sub SaveDTWPointMatrixToFile(ByRef DistanceMatrix(,) As DTWPoint, Optional ByVal FilePath As String = "")

                If FilePath = "" Then FilePath = Utils.GetSaveFilePath()

                Dim SaveFolder As String = Path.GetDirectoryName(FilePath)
                If Not Directory.Exists(SaveFolder) Then Directory.CreateDirectory(SaveFolder)

                Dim writer As New IO.StreamWriter(FilePath)

                For Row_j = 0 To DistanceMatrix.GetUpperBound(1)

                    Dim CurrentRow As String = ""

                    For Column_i = 0 To DistanceMatrix.GetUpperBound(0)

                        CurrentRow &= DistanceMatrix(Column_i, Row_j).Value & vbTab

                    Next

                    writer.WriteLine(CurrentRow)

                Next

                writer.Close()

            End Sub

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="Signal"></param>
            ''' <param name="Noise">Should be at least as long as the longest signal.</param>
            ''' <param name="MeasurementWindowDuration"></param>
            ''' <param name="MeasurementWindowOverlapDuration"></param>
            ''' <param name="BarkFilterOverlapRatio"></param>
            ''' <param name="LowestIncludedCentreFrequency"></param>
            ''' <param name="HighestIncludedCentreFrequency"></param>
            ''' <param name="MatrixOutputFolder"></param>
            ''' <param name="ExportDetails"></param>
            ''' <param name="InactivateFftWarnings"></param>
            ''' <param name="CurrentIsoPhonFilter"></param>
            ''' <param name="CurrentAuditoryFilters"></param>
            ''' <param name="CurrentSpreadOfMaskingFilters"></param>
            ''' <param name="dbFSToSplDifference"></param>
            ''' <param name="CurrentBandTemplateList"></param>
            ''' <returns></returns>
            Public Function GetLoudnessOverNoise(ByRef Signal As Sound, ByRef Noise As Sound,
                                            Optional ByVal MeasurementWindowDuration As Double = 0.1,
                                            Optional ByVal MeasurementWindowOverlapDuration As Double = 0.095,
                                            Optional ByVal BarkFilterOverlapRatio As Double = 0.9,
                                            Optional ByRef LowestIncludedCentreFrequency As Double = 80,
                                            Optional ByRef HighestIncludedCentreFrequency As Double = 12700, 'Previously 17500, but limited due to the limited band width of the iso-phon filter
                                            Optional ByVal MatrixOutputFolder As String = "",
                                            Optional ByVal ExportDetails As Boolean = False,
                                            Optional ByRef InactivateFftWarnings As Boolean = False,
                                            Optional ByRef CurrentIsoPhonFilter As IsoPhonFilter = Nothing,
                                            Optional ByRef CurrentAuditoryFilters As FftData.AuditoryFilters = Nothing,
                                            Optional ByRef CurrentSpreadOfMaskingFilters As FftData.SpreadOfMaskingFilters = Nothing,
                                            Optional ByRef CurrentBandTemplateList As FftData.BarkSpectrum.BandTemplateList = Nothing,
                                             Optional ByRef CurrentFftFormat As Formats.FftFormat = Nothing,
                                             Optional ByRef dbFSToSplDifference As Double = 88,
                                             Optional ByRef LoudnessFunction As FftData.LoudnessFunctions = FftData.LoudnessFunctions.ZwickerType,
                                             Optional ByRef SoneScalingFactor As Double? = Nothing) As Double()

                Try
                    Dim FileComparisonID As String = Signal.FileName & "_" & Noise.FileName

                    'Setting up FFT formats
                    If CurrentFftFormat Is Nothing Then
                        Dim MeasurementWindowLength As Integer = Signal.WaveFormat.SampleRate * MeasurementWindowDuration
                        If MeasurementWindowLength Mod 2 = 1 Then MeasurementWindowLength += 1

                        Dim MeasurementWindowOverlapLength As Integer = Signal.WaveFormat.SampleRate * MeasurementWindowOverlapDuration
                        If MeasurementWindowOverlapLength Mod 2 = 1 Then MeasurementWindowOverlapLength += 1
                        Dim SpectralResolution As Integer = 2048 * 2 * 2
                        CurrentFftFormat = New Formats.FftFormat(MeasurementWindowLength, SpectralResolution, MeasurementWindowOverlapLength, WindowingType.Hamming, InactivateFftWarnings)

                    End If


                    'Creating a MatrixOutputFolder if it doesn't exist
                    Directory.CreateDirectory(MatrixOutputFolder)

                    'Only allowing mono sounds
                    If Signal.WaveFormat.Channels <> 1 Or Noise.WaveFormat.Channels <> 1 Then Throw New Exception("The current function only supports mono sounds.")

                    'Stopping if the noise is shorter than the signal
                    If Noise.WaveData.ShortestChannelSampleCount < Signal.WaveData.ShortestChannelSampleCount Then Throw New Exception("The noise sound must be equally long or longer than the signal sound.")

                    'Checking that the sounds have the same format
                    If Signal.WaveFormat.BitDepth <> Noise.WaveFormat.BitDepth Or
            Signal.WaveFormat.SampleRate <> Noise.WaveFormat.SampleRate Or
            Signal.WaveFormat.Encoding <> Noise.WaveFormat.Encoding Then Throw New Exception("Different formats in input sounds.")


                    'No initial zero padding! Zero padding is unnecessary here, since no temporal alignment is needed (as with the sound similarity calculation), and a hearing threshold is used. However, the end of the sound will be zero padded by the spectral analysis function.
                    ''Zeropadding the input sounds, so that they get MeasurementWindowOverlapLength before the sound. After the sound, the spectral analysis function adds zero padding automatically
                    'If Signal.FFT Is Nothing Then Signal.ZeroPad(MeasurementWindowOverlapLength, Nothing, False) 'Only zero padding if FFT is nothing, which means that it hasn't been here before
                    'Noise.ZeroPad(MeasurementWindowOverlapLength, Nothing, False)

                    'Copying the signal to a new sound. Adding one FftWindowSize of length after the signal to avoid fading effects in the signal caused by the spectral analysis. Instead the fading occurs in the last bit of noise, of which the data is removed below.
                    Dim OriginalSignalLength As Integer = Signal.WaveData.SampleData(1).Length
                    Dim SignalAndNoise As New Sound(Signal.WaveFormat)
                    'Copying the signal samples and mixing them with the noise
                    Dim NewMixedSampleArray(OriginalSignalLength + CurrentFftFormat.FftWindowSize) As Single
                    'Adding signal+noise
                    For s = 0 To OriginalSignalLength - 1
                        NewMixedSampleArray(s) = Signal.WaveData.SampleData(1)(s) + Noise.WaveData.SampleData(1)(s)
                    Next
                    'Adding only noise to the last bit
                    For s = OriginalSignalLength To NewMixedSampleArray.Length - 1
                        NewMixedSampleArray(s) = Noise.WaveData.SampleData(1)(s)
                    Next
                    'Adding the new array to the new sound
                    SignalAndNoise.WaveData.SampleData(1) = NewMixedSampleArray

                    'Getting frequency domain data
                    SignalAndNoise.FFT = SpectralAnalysis(SignalAndNoise, CurrentFftFormat)
                    'Calculating power spectrum
                    SignalAndNoise.FFT.CalculatePowerSpectrum(True, True, True, 0.25)

                    'Getting bark filter spectrum for the signal
                    SignalAndNoise.FFT.CalculateBarkSpectrum(SignalAndNoise, 1, CurrentIsoPhonFilter, CurrentAuditoryFilters, CurrentSpreadOfMaskingFilters,
                       BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency,
                       dbFSToSplDifference, LoudnessFunction, CurrentBandTemplateList, SoneScalingFactor)

                    'Referencing the Bark spectra
                    Dim SignalAndNoiseBarkSpectrumArray As SortedList(Of Integer, Single()) = SignalAndNoise.FFT.BarkSpectrumData(1)

                    'Removing the last time windows repressenting the period of final zero padding above
                    For n = 0 To Int(CurrentFftFormat.FftWindowSize / (CurrentFftFormat.AnalysisWindowSize - CurrentFftFormat.OverlapSize)) - 1
                        SignalAndNoiseBarkSpectrumArray.RemoveAt(SignalAndNoiseBarkSpectrumArray.Count - 1)
                    Next

                    'Exporting stuff
                    If ExportDetails = True And MatrixOutputFolder <> "" Then
                        Dim newDoubleArray(SignalAndNoiseBarkSpectrumArray.Count - 1, SignalAndNoiseBarkSpectrumArray(0).Length - 1) As Double
                        For p = 0 To SignalAndNoiseBarkSpectrumArray.Count - 1
                            For q = 0 To SignalAndNoiseBarkSpectrumArray(0).Length - 1
                                newDoubleArray(p, q) = SignalAndNoiseBarkSpectrumArray(p)(q)
                            Next
                        Next
                        Utils.SaveMatrixToFile(newDoubleArray, IO.Path.Combine(MatrixOutputFolder, "BarkSpectrum_" & FileComparisonID & ".txt"))
                    End If


                    'Running the spectral analysis only once for each noise. If a new noise (using the same Sound instance) is used, the calling code must set its FFT instance to Nothing, lest incorrect sprectral data will be used.
                    'Declaring a local variable to hold the noise Bark spectrum
                    Dim NoiseBarkSpectrumArray As SortedList(Of Integer, Single())
                    If Noise.FFT Is Nothing Then
                        Noise.FFT = SpectralAnalysis(Noise, CurrentFftFormat)
                        'Calculating spectra
                        Noise.FFT.CalculatePowerSpectrum(True, True, True, 0.25)

                        'Getting bark filter spectrum for sound 2.
                        Noise.FFT.CalculateBarkSpectrum(Noise, 1, CurrentIsoPhonFilter, CurrentAuditoryFilters, CurrentSpreadOfMaskingFilters,
                       BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency,
                       dbFSToSplDifference, LoudnessFunction, CurrentBandTemplateList, SoneScalingFactor)

                        'Referencing the Bark noise spectrum
                        NoiseBarkSpectrumArray = Noise.FFT.BarkSpectrumData(1)

                        'Exporting stuff. Only done on creation of the noise Bark Spectrum.
                        If ExportDetails = True And MatrixOutputFolder <> "" Then
                            Dim newDoubleArray2(NoiseBarkSpectrumArray.Count - 1, NoiseBarkSpectrumArray(0).Length - 1) As Double
                            For p = 0 To NoiseBarkSpectrumArray.Count - 1
                                For q = 0 To NoiseBarkSpectrumArray(0).Length - 1
                                    newDoubleArray2(p, q) = NoiseBarkSpectrumArray(p)(q)
                                Next
                            Next
                            Utils.SaveMatrixToFile(newDoubleArray2, IO.Path.Combine(MatrixOutputFolder, "BarkSpectrum_" & Noise.FileName & ".txt"))
                        End If

                    Else
                        'Referencing the pre-calculated Bark spectra
                        NoiseBarkSpectrumArray = Noise.FFT.BarkSpectrumData(1)
                    End If


                    'Calculating loudness over noise
                    Dim LoudnessOverNoiseOverTime(SignalAndNoiseBarkSpectrumArray.Count - 1) As Double
                    For TimeWindowIndex = 0 To SignalAndNoiseBarkSpectrumArray.Count - 1

                        Dim TimeWindowDifferenceArray(SignalAndNoiseBarkSpectrumArray(TimeWindowIndex).Count - 1) As Double
                        For BarkBandIndex = 0 To SignalAndNoiseBarkSpectrumArray(TimeWindowIndex).Count - 1
                            Dim CurrentDistanceOverNoise As Double = Math.Max(0, SignalAndNoiseBarkSpectrumArray(TimeWindowIndex)(BarkBandIndex) - NoiseBarkSpectrumArray(TimeWindowIndex)(BarkBandIndex))
                            TimeWindowDifferenceArray(BarkBandIndex) = CurrentDistanceOverNoise
                        Next
                        LoudnessOverNoiseOverTime(TimeWindowIndex) = TimeWindowDifferenceArray.Sum
                    Next

                    Return LoudnessOverNoiseOverTime

                    'Ev. Längre integrationstid och kortare analysfönster, + intregrering i frekvensdomänen?!? Nog svårt!?
                    'NB. KAnske istället köra ett jämnt brus (medel) som läggs på i frekvensdomänen? Summan blir inkorrekt!? Fasskillnader kan göra att intensiteten varierar! Max eller summa?

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function

            Public Enum AcousticDistanceTypes
                LinearCorrelation
                ManhattanDistance
                EuclideanDistance
            End Enum

            Private Function GetDistanceValue(ByRef AmplitudeSpectrum1 As SortedList(Of Integer, Single()), ByRef AmplitudeSpectrum2 As SortedList(Of Integer, Single()),
                                                 ByVal ColumnIndex As Integer, ByVal RowIndex As Integer,
                                              Optional ByVal AcousticDistanceType As AcousticDistanceTypes = AcousticDistanceTypes.EuclideanDistance) As Double

                Select Case AcousticDistanceType
                    Case AcousticDistanceTypes.LinearCorrelation
                        Return GetCorrelationValue(AmplitudeSpectrum1, AmplitudeSpectrum2, ColumnIndex, RowIndex)

                    Case AcousticDistanceTypes.ManhattanDistance

                        Return GetManhattanDistance(AmplitudeSpectrum1, AmplitudeSpectrum2, ColumnIndex, RowIndex)

                    Case AcousticDistanceTypes.EuclideanDistance

                        Return GetEuclideanDistance(AmplitudeSpectrum1, AmplitudeSpectrum2, ColumnIndex, RowIndex)

                    Case Else

                        Throw New NotImplementedException
                End Select

            End Function

            Private Function GetEuclideanDistance(ByRef AmplitudeSpectrum1 As SortedList(Of Integer, Single()), ByRef AmplitudeSpectrum2 As SortedList(Of Integer, Single()),
                                             ByVal ColumnIndex As Integer, ByVal RowIndex As Integer) As Double

                Dim Sum As Double = 0
                For n = 0 To AmplitudeSpectrum1(0).Length - 1
                    Sum += (AmplitudeSpectrum1(ColumnIndex)(n) - AmplitudeSpectrum2(RowIndex)(n)) ^ 2
                Next

                Return Math.Sqrt(Sum)

            End Function

            Private Function GetManhattanDistance(ByRef AmplitudeSpectrum1 As SortedList(Of Integer, Single()), ByRef AmplitudeSpectrum2 As SortedList(Of Integer, Single()),
                                             ByVal ColumnIndex As Integer, ByVal RowIndex As Integer) As Double

                'Subtracting the arrays
                Dim SubtractionArray(AmplitudeSpectrum1(0).Length - 1) As Single
                For k = 0 To SubtractionArray.Length - 1
                    SubtractionArray(k) = Math.Abs(AmplitudeSpectrum1(ColumnIndex)(k) - AmplitudeSpectrum2(RowIndex)(k))
                Next

                Dim TotalSubtractionValue As Double = SubtractionArray.Average
                Return TotalSubtractionValue

            End Function


            Private Function GetCorrelationValue(ByRef AmplitudeSpectrum1 As SortedList(Of Integer, Single()), ByRef AmplitudeSpectrum2 As SortedList(Of Integer, Single()),
                                             ByVal ColumnIndex As Integer, ByVal RowIndex As Integer) As Double

                'Returns 1 if row or column index is 0, since these repressent sound prior to input the sound start position (A silent array could also be used, but just returning 1, is more efficient)
                'Assuming totally different sound prior to the start
                If RowIndex < 0 Or ColumnIndex < 0 Then
                    Return 1

                Else

                    'Comparing magnitude arrays
                    Dim r As Double = Utils.PearsonsCorrelation(AmplitudeSpectrum1(ColumnIndex), AmplitudeSpectrum2(RowIndex))

                    'Scaleing to range 0-1, where 1 is totally different and 0 is identical
                    Return (1 - r) / 2

                End If

            End Function

        End Module


        Public Module TransformationsExt

            Public Function AM(ByRef inputSound As Sound, ByVal carrierFrequency As Single, ByVal deModulation As Boolean, Optional ByVal channel As Integer? = Nothing,
             Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength? As Integer = Nothing) As Sound

                Try

                    Dim AudioOutputConstructor As New AudioOutputConstructor(inputSound.WaveFormat, channel)
                    Dim outputSound As Sound = AudioOutputConstructor.GetNewOutputSound()

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim inputArray() As Single = inputSound.WaveData.SampleData(c)

                        Dim CorrectedStartSample = startSample
                        Dim CorrectedSectionLength = sectionLength
                        CheckAndCorrectSectionLength(inputArray.Length, CorrectedStartSample, CorrectedSectionLength)

                        ReDim outputSound.WaveData.SampleData(c)(inputArray.Length - 1)

                        'Create carrier wave
                        Dim posFS As Double = inputSound.WaveFormat.PositiveFullScale

                        Dim carrierArray(inputArray.Length - 1) As Single
                        Dim level = 0.5
                        Dim freq = carrierFrequency

                        Select Case inputSound.WaveFormat.BitDepth
                            Case 8
                                For n = 0 To carrierArray.Length - 1
                                    carrierArray(n) = (level * (posFS / 2)) * Math.Sin(twopi * (freq / inputSound.WaveFormat.SampleRate) * n) + posFS / 2 ' - _
                                    '(level * Short.MaxValue) * Math.Cos(twopi * (freq / sampleRate) * n)
                                Next

                            Case 16
                                For n = 0 To carrierArray.Length - 1
                                    carrierArray(n) = ((level) * Math.Sin(twopi * (freq / inputSound.WaveFormat.SampleRate) * n)) + 0.6 ' - _
                                    '(level * Short.MaxValue) * Math.Cos(twopi * (freq / sampleRate) * n)
                                Next
                            Case Else
                                Throw New NotImplementedException(inputSound.WaveFormat.BitDepth & " bit depth Is Not yet implemented.")
                        End Select

                        Select Case deModulation
                            Case False
                                'Multiplying
                                For n = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1

                                    'Convert input array values to decimal values between 0-1
                                    outputSound.WaveData.SampleData(c)(n) = (inputArray(n) / (2 * posFS) + 0.5) * carrierArray(n)
                                    'MsgBox(inputArray(n) & " " & (inputArray(n) / (2 * posFS) + 0.5) & " " & carrierArray(n))
                                Next

                            Case True
                                'Dividing
                                For n = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                                    'Reversing
                                    outputSound.WaveData.SampleData(c)(n) = ((inputArray(n) / carrierArray(n)) - 0.5) * (2 * posFS) 'OBS problem division med 0 i sinusvågen, höj till mellan 0,1-1,1?
                                Next

                        End Select


                    Next

                    Return outputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try


            End Function



            'Section on modifying sounds



            ''' <summary>
            ''' Inverses a section of the sound.
            ''' </summary>
            ''' <param name="InputSound">The sound to modify.</param>
            ''' <param name="Channel">The channel to be modified. If left empty, all channels will be modified.</param>
            ''' <param name="StartSample">Start sample of the section to be inversed.</param>
            ''' <param name="SectionLength">Length (in samples) of the section to be inversed.</param>
            Public Sub InverseSection(ByRef InputSound As Sound,
                                  Optional ByVal Channel As Integer? = Nothing,
                                  Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing)

                Try

                    Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, Channel)

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim SoundArray = InputSound.WaveData.SampleData(c)

                        Dim CorrectedStartSample = StartSample
                        Dim CorrectedSectionLength = SectionLength
                        CheckAndCorrectSectionLength(SoundArray.Length, CorrectedStartSample, CorrectedSectionLength)

                        For n = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                            'Inversing the sample value
                            SoundArray(n) = -SoundArray(n)
                        Next
                    Next

                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try

            End Sub

            ''' <summary>
            ''' Shifts a section of the sound without extending the sound.
            ''' </summary>
            ''' <param name="InputSound">The sound to modify.</param>
            ''' <param name="ShiftLength">The lengths to shift</param>
            ''' <param name="Channel">The channel to be modified. If left empty, all channels will be modified.</param>
            ''' <param name="StartSample">Start sample of the section to be inversed.</param>
            ''' <param name="SectionLength">Length (in samples) of the section to be inversed.</param>
            Public Sub ShiftSection(ByRef InputSound As Sound, ByVal ShiftLength As Integer,
                                  Optional ByVal Channel As Integer? = Nothing,
                                  Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing)

                Try

                    Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, Channel)

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim OriginalArraySoundArray = InputSound.WaveData.SampleData(c)


                        Dim CorrectedStartSample = StartSample
                        Dim CorrectedSectionLength = SectionLength
                        CheckAndCorrectSectionLength(OriginalArraySoundArray.Length, CorrectedStartSample, CorrectedSectionLength)

                        Dim ShifterSoundArray(OriginalArraySoundArray.Length - 1) As Single
                        'Coying the non-shifted region
                        For n = 0 To CorrectedStartSample - 1
                            ShifterSoundArray(n) = OriginalArraySoundArray(n)
                        Next
                        For n = CorrectedStartSample + CorrectedSectionLength To OriginalArraySoundArray.Length - 1
                            ShifterSoundArray(n) = OriginalArraySoundArray(n)
                        Next

                        'Coopying the shifted region
                        Dim ShiftWriteSample As Integer
                        For n = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1

                            ShiftWriteSample = n + ShiftLength

                            'Cheching that we're inside the array bounds
                            If ShiftWriteSample < 0 Or ShiftWriteSample >= OriginalArraySoundArray.Length Then Continue For

                            ShifterSoundArray(ShiftWriteSample) = OriginalArraySoundArray(n)
                        Next

                        'Replacing the original array with the shifted one
                        InputSound.WaveData.SampleData(c) = ShifterSoundArray

                    Next

                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try

            End Sub

            ''' <summary>
            ''' Applies the indicated phoneme gain to the input sound, using the segmentation data stored in the indicated SMA object.
            ''' N.B. Presently, multiple sentences are not supported!
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="CurrentPtwfData"></param>
            ''' <param name="PhonemeGain"></param>
            Public Sub ApplyTestPhonemeGain(ByRef InputSound As Sound, ByRef CurrentPtwfData As Sound.SpeechMaterialAnnotation,
                                        ByVal TestPhonemeIndex As Integer,
                                        ByVal PhonemeGain As Double,
                                        ByVal IsFixedLengthGroup As Boolean,
                                        Optional ByVal FixedFadeDuration As Single = 0.01,
                                        Optional ByVal RelativeFadeLength As Single = 0.1,
                                        Optional ByVal ExtraPaddingSamplesBeforeRecordingStart As Integer = 0,
                                        Optional ByVal SpeechChannel As Integer = 1)

                Dim sentence As Integer = 0

                'Noting indices
                Dim TestPhonemeSampleStartIndex As Integer = CurrentPtwfData.ChannelData(1)(sentence)(0)(TestPhonemeIndex).StartSample + ExtraPaddingSamplesBeforeRecordingStart
                Dim TestPhonemeSampleCount As Integer = CurrentPtwfData.ChannelData(1)(sentence)(0)(TestPhonemeIndex).Length


                'Determining the fade lengths
                Dim FadeInLength As Integer = 0
                Dim FadeOutLength As Integer = 0
                If IsFixedLengthGroup = True Then
                    FadeInLength = InputSound.WaveFormat.SampleRate * FixedFadeDuration
                    FadeOutLength = FadeInLength
                Else
                    FadeInLength = TestPhonemeSampleCount * RelativeFadeLength
                    FadeOutLength = FadeInLength
                End If

                'Doing fade in
                If TestPhonemeIndex = 0 Then
                    'If it is the first phoneme, a fade from silence will be made in the padding region
                    DSP.Fade(InputSound, , -PhonemeGain, SpeechChannel,
                               ExtraPaddingSamplesBeforeRecordingStart,
                               TestPhonemeSampleStartIndex)

                    'Setting fade in length to 0
                    FadeInLength = 0

                Else
                    'Doing test phoneme section fade in
                    If FadeInLength > 0 Then
                        DSP.Fade(InputSound, 0, -PhonemeGain, SpeechChannel, TestPhonemeSampleStartIndex, FadeInLength)
                    End If
                End If

                'Doing fade out after the test word, if the test phoneme is the last phoneme (if not fade out is done below)
                If TestPhonemeIndex = CurrentPtwfData.ChannelData(1)(sentence)(0).Count - 2 Then '-2 is used because the last item in PhoneData should be a word end marker.

                    'We're on the last phoneme, fading to silence
                    DSP.Fade(InputSound, -PhonemeGain, , SpeechChannel, TestPhonemeSampleStartIndex + TestPhonemeSampleCount) 'Fade goes to the end of the sound by leaving length out.

                    'Setting fade out length to 0
                    FadeOutLength = 0
                End If


                'Applying test phoneme section gain
                DSP.AmplifySection(InputSound, PhonemeGain, SpeechChannel, TestPhonemeSampleStartIndex + FadeInLength, TestPhonemeSampleCount - FadeInLength - FadeOutLength)

                'Doing fade out in not last phoneme
                If Not TestPhonemeIndex = CurrentPtwfData.ChannelData(1)(sentence)(0).Count - 2 Then '-2 is used because the last item in PhoneData should be a word end marker.
                    'Doing test phoneme section fade out
                    If FadeOutLength > 0 Then
                        'Calculating the fade out start sample and doing the fading
                        Dim FadeOutStartSample As Integer = TestPhonemeSampleStartIndex + TestPhonemeSampleCount - FadeOutLength
                        DSP.Fade(InputSound, -PhonemeGain, 0, SpeechChannel, FadeOutStartSample, FadeOutLength)
                    End If
                End If

            End Sub

            ''' <summary>
            ''' Silences the indicated section of the input sound.
            ''' </summary>
            ''' <param name="inputSound"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength">If left to nothing, the rest of the sound is silenced.</param>
            ''' <param name="Channel">The channel to silence. If left to -1 all channels will be silenced.</param>
            Public Sub SilenceSection(ByRef InputSound As Sound,
                         Optional ByVal StartSample As Integer = 0,
                               Optional ByVal SectionLength As Integer? = Nothing,
                                  Optional ByVal Channel As Integer = -1)

                Try

                    If Channel = -1 Then

                        'Silences the sound in all channels
                        For c = 1 To InputSound.WaveFormat.Channels
                            Dim InputSoundArray() As Single = InputSound.WaveData.SampleData(c)

                            Dim CorrectedStartSample = StartSample
                            Dim CorrectedSectionLength = SectionLength
                            CheckAndCorrectSectionLength(InputSoundArray.Length, CorrectedStartSample, CorrectedSectionLength)

                            For s = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                                InputSoundArray(s) = 0
                            Next
                        Next
                    Else

                        'Silences the sound
                        Dim InputSoundArray() As Single = InputSound.WaveData.SampleData(Channel)

                        Dim CorrectedStartSample = StartSample
                        Dim CorrectedSectionLength = SectionLength
                        CheckAndCorrectSectionLength(InputSoundArray.Length, CorrectedStartSample, CorrectedSectionLength)

                        For s = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                            InputSoundArray(s) = 0
                        Next
                    End If

                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try

            End Sub

            ''' <summary>
            ''' Peak clips the the indicated section of the input sound.
            ''' </summary>
            ''' <param name="inputSound"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength">If left to nothing, the rest of the sound is silenced.</param>
            ''' <param name="Channel">The channel to hard clip. If left to Nothing, all channels will be hard clipped.</param>
            Public Sub PeakClippingSection(ByRef InputSound As Sound,
                                       Optional PositiveClippingValue As Single = 1,
                                       Optional NegativeClippingValue As Single? = Nothing,
                                       Optional ByVal StartSample As Integer = 0,
                                       Optional ByVal SectionLength As Integer? = Nothing,
                                       Optional ByVal Channel As Integer? = Nothing)

                Try

                    'Setting a default negative clipping value, if not supplied by the calling code
                    If NegativeClippingValue Is Nothing Then NegativeClippingValue = -PositiveClippingValue

                    If Channel Is Nothing Then

                        'Clipping the sound in all channels
                        For c = 1 To InputSound.WaveFormat.Channels
                            Dim InputSoundArray() As Single = InputSound.WaveData.SampleData(c)

                            Dim CorrectedStartSample = StartSample
                            Dim CorrectedSectionLength = SectionLength
                            CheckAndCorrectSectionLength(InputSoundArray.Length, CorrectedStartSample, CorrectedSectionLength)

                            For s = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                                InputSoundArray(s) = Math.Min(Math.Max(NegativeClippingValue.Value, InputSoundArray(s)), PositiveClippingValue)
                            Next
                        Next
                    Else

                        'Clipping the sound
                        Dim InputSoundArray() As Single = InputSound.WaveData.SampleData(Channel)

                        Dim CorrectedStartSample = StartSample
                        Dim CorrectedSectionLength = SectionLength
                        CheckAndCorrectSectionLength(InputSoundArray.Length, CorrectedStartSample, CorrectedSectionLength)

                        For s = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                            InputSoundArray(s) = Math.Min(Math.Max(-NegativeClippingValue.Value, InputSoundArray(s)), PositiveClippingValue)
                        Next
                    End If

                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try

            End Sub


            ''' <summary>
            ''' Soft limits the indicated section of the input sound to the specified RMS level (in dBFS).
            ''' </summary>
            ''' <param name="inputSound"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength"></param>
            ''' <param name="WindowDuration">The time in seconds of windows within which to average the RMS level.</param>
            ''' <param name="Channel">The channel to hard limit. If left to Nothing, all channels will be limited.</param>
            ''' <param name="RemoveAddedLength">If set to true, the input sound will keep the exact length, but processing takes a little longer.</param>
            ''' <param name="ReturnReport">If ReturnReport is set to True, the function returns a string containing a report of the applied limiting.</param>
            ''' <returns>If ReturnReport is set to True, the function returns a string containing a report of the applied limiting.</returns>
            Public Function SoftLimitSection(ByRef InputSound As Sound,
                                    ByVal ThresholdLevel As Double,
                                    Optional ByVal StartSample As Integer = 0,
                                    Optional ByVal SectionLength As Integer? = Nothing,
                                    Optional ByVal WindowDuration As Double = 0.2,
                                    Optional ByVal Channel As Integer? = Nothing,
                                    Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z,
                                    Optional ByVal RemoveAddedLength As Boolean = False,
                                    Optional ByVal ReturnReport As Boolean = True,
                                         Optional ByVal slopeType As FadeSlopeType = FadeSlopeType.Linear,
                                         Optional CosinePower As Double = 10,
                                         Optional ByVal EqualPower As Boolean = False) As String

                Dim Report As String = ""

                Try
                    'Declares a list used for reporting of limiter data
                    Dim ReportList As List(Of String) = Nothing
                    If ReturnReport = True Then
                        ReportList = New List(Of String)
                    End If

                    'Calculating the averaging window length in samples
                    Dim WindowLength As Integer = WindowDuration * InputSound.WaveFormat.SampleRate

                    'Stores the original channel 1 length
                    Dim OriginalLength As Integer = InputSound.WaveData.SampleData(1).Length
                    Dim ZeroPadded As Boolean = False

                    'Zeropads the sound to an integer of WindowLength, if the end of file is reached
                    'If StartSample + SectionLength > OriginalLength Then
                    If InputSound.WaveData.SampleData(1).Length Mod WindowLength <> 0 Then
                        Dim IntendedWindowCount As Integer = Math.Ceiling(InputSound.WaveData.SampleData(1).Length / WindowLength)
                        Dim WindowIntegerMultipleLength As Integer = WindowLength * IntendedWindowCount
                        SectionLength = WindowIntegerMultipleLength
                        ZeroPadded = True
                    End If

                    'Performs limiting by calling LimitChannelSection
                    If Channel Is Nothing Then
                        'Limiting the sound in all channels
                        For c = 1 To InputSound.WaveFormat.Channels
                            'Limiting the sound in the specified channel
                            SoftLimitChannelSection(InputSound, ThresholdLevel, StartSample, SectionLength, WindowLength,
                                                c, FrequencyWeighting, ReportList, slopeType, CosinePower, EqualPower)
                        Next
                    Else
                        'Limiting the sound in the specified channel
                        SoftLimitChannelSection(InputSound, ThresholdLevel, StartSample, SectionLength, WindowLength,
                                            Channel, FrequencyWeighting, ReportList, slopeType, CosinePower, EqualPower)
                    End If

                    If RemoveAddedLength = True And ZeroPadded = True Then
                        'Restores the original length (and sections length, if ever changed to ByRef)
                        CropSection(InputSound, 0, OriginalLength)
                        SectionLength = OriginalLength
                    End If

                    'Creating the report
                    If ReturnReport = True Then
                        Report = String.Join(vbCrLf, ReportList)
                    End If

                Catch ex As Exception
                    AudioError(ex.ToString)
                    'Storing the exception info in Report
                    If ReturnReport = True Then
                        Report &= "The following error occured in SoftLimitSection: " & ex.ToString
                    End If
                End Try

                Return Report

            End Function


            ''' <summary>
            ''' Applies limiting to the specified channel, in the specified sound.
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="ThresholdLevel"></param>
            ''' <param name="StartSample"></param>
            ''' <param name="SectionLength">The length of the measurement windows. Must be an even integer.</param>
            ''' <param name="WindowLength"></param>
            ''' <param name="Channel"></param>
            ''' <param name="ReportList">A list of string that can be used to log limiter data.</param>
            Private Sub SoftLimitChannelSection(ByRef InputSound As Sound,
                                            ByVal ThresholdLevel As Double,
                                            ByVal StartSample As Integer,
                                            ByVal SectionLength As Integer,
                                            ByVal WindowLength As Integer,
                                            ByVal Channel As Integer,
                                            ByVal FrequencyWeighting As FrequencyWeightings,
                                            Optional ByRef ReportList As List(Of String) = Nothing,
                                            Optional ByVal slopeType As FadeSlopeType = FadeSlopeType.Linear,
                                            Optional CosinePower As Double = 10,
                                            Optional ByVal EqualPower As Boolean = False)

                Try

                    'Calculating the number of sections
                    Dim SectionCount As Integer = Int(SectionLength / WindowLength)

                    'Measures each section of the sound, and stores the results in SoundLevelList
                    Dim SoundLevelList As New List(Of Double)
                    For SectionIndex = 0 To SectionCount - 1
                        SoundLevelList.Add(MeasureSectionLevel(InputSound, Channel, StartSample + SectionIndex * WindowLength, WindowLength,,, FrequencyWeighting))
                    Next

                    'Going though each section and calculates needed attenuation 
                    Dim AttenuationList As New List(Of Double)
                    For SectionIndex = 0 To SectionCount - 1
                        AttenuationList.Add(Math.Max(0, SoundLevelList(SectionIndex) - ThresholdLevel))
                    Next

                    Dim LastAttenuation As Double = AttenuationList(0)
                    For SectionIndex = 0 To SectionCount - 1

                        'Calculating attenuation
                        Dim StartAttenuation As Double
                        Dim EndAttenuation As Double
                        If SectionIndex = SectionCount - 1 Then
                            'The last window uses only the attentation level of the last index in AttenuationList
                            StartAttenuation = Math.Max(AttenuationList(SectionIndex), AttenuationList(SectionIndex))
                            EndAttenuation = StartAttenuation
                        Else
                            StartAttenuation = LastAttenuation
                            EndAttenuation = Math.Max(AttenuationList(SectionIndex), AttenuationList(SectionIndex + 1))
                            LastAttenuation = EndAttenuation
                        End If

                        'Using Fade to perform attenuation
                        Fade(InputSound, StartAttenuation, EndAttenuation, Channel, StartSample + SectionIndex * WindowLength,
                         WindowLength, slopeType, CosinePower, EqualPower)

                        'Reporting only limited sections
                        If ReportList IsNot Nothing Then
                            If StartAttenuation > 0 Or EndAttenuation > 0 Then
                                ReportList.Add(SectionIndex & vbTab & StartAttenuation & vbTab & EndAttenuation)
                            End If
                        End If
                    Next

                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try

            End Sub



            ''' <summary>
            ''' Crops the sound (removes all sound before StartSample, and after StartSample + Length).
            ''' </summary>
            ''' <param name="inputSound"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength"></param>
            Public Sub CropSection(ByRef InputSound As Sound,
                         Optional ByVal StartSample As Integer = 0,
                               Optional ByVal SectionLength As Integer = -1)

                Try

                    'Main section
                    For c = 1 To InputSound.WaveFormat.Channels

                        Dim InputSoundArray() As Single = InputSound.WaveData.SampleData(c)

                        Dim CorrectedStartSample = StartSample
                        Dim CorrectedSectionLength = SectionLength
                        CheckAndCorrectSectionLength(InputSoundArray.Length, CorrectedStartSample, CorrectedSectionLength)

                        'Creating a new sound array
                        Dim NewSoundArray(CorrectedSectionLength - 1) As Single

                        'Copies the sound
                        For s = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                            NewSoundArray(s - CorrectedStartSample) = InputSoundArray(s)
                        Next

                        'Replacing the sound array with the cropped sound array
                        InputSound.WaveData.SampleData(c) = NewSoundArray

                    Next

                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try

            End Sub

            ''' <summary>
            ''' Deleting a specified section of the sound from sound (in all channels)
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="StartSample"></param>
            ''' <param name="SectionLength"></param>
            Public Sub DeleteSection(ByRef InputSound As Sound,
                         Optional ByVal StartSample As Integer = 0,
                               Optional ByVal SectionLength As Integer = -1)

                Try

                    'Main section
                    For c = 1 To InputSound.WaveFormat.Channels

                        Dim InputSoundArray() As Single = InputSound.WaveData.SampleData(c)

                        Dim CorrectedStartSample = StartSample
                        Dim CorrectedSectionLength = SectionLength
                        CheckAndCorrectSectionLength(InputSound.WaveData.SampleData(c).Length, CorrectedStartSample, CorrectedSectionLength)

                        'Getting a copy of sound without the selected samples
                        Dim newArray(InputSoundArray.Length - CorrectedSectionLength - 1) As Single

                        For sample = 0 To CorrectedStartSample - 1
                            newArray(sample) = InputSoundArray(sample)
                        Next
                        For sample = CorrectedStartSample To newArray.Length - 1
                            newArray(sample) = InputSoundArray(sample + CorrectedSectionLength)
                        Next

                        InputSound.WaveData.SampleData(c) = newArray

                    Next

                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try


            End Sub


            ''' <summary>
            ''' Insert a section of silence, with the specified startsample and length into all channels of the input sound
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="StartSample"></param>
            ''' <param name="SectionLength"></param>
            Public Sub InsertSilentSection(ByRef InputSound As Sound,
                         Optional ByVal StartSample As Integer = 0,
                               Optional ByVal SectionLength As Integer = -1)

                Try

                    'Main section
                    For c = 1 To InputSound.WaveFormat.Channels

                        Dim InputSoundArray() As Single = InputSound.WaveData.SampleData(c)

                        Dim CorrectedStartSample = StartSample
                        Dim CorrectedSectionLength = SectionLength
                        CheckAndCorrectSectionLength(InputSoundArray.Length, CorrectedStartSample, CorrectedSectionLength)

                        'Getting a copy of sound without the selected samples
                        Dim newArray(InputSoundArray.Length + CorrectedSectionLength - 1) As Single
                        For sample = 0 To CorrectedStartSample - 1
                            newArray(sample) = InputSoundArray(sample)
                        Next
                        For sample = CorrectedStartSample To InputSoundArray.Length - 1
                            newArray(sample + CorrectedSectionLength) = InputSoundArray(sample)
                        Next

                        InputSound.WaveData.SampleData(c) = newArray

                    Next

                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try


            End Sub

            ''' <summary>
            ''' Copies a selected section of the sound to a new Sound.
            ''' </summary>
            ''' <param name="inputSound"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength"></param>
            Public Function CopySection(ByRef InputSound As Sound, Optional ByVal StartSample As Integer = 0,
                                    Optional ByVal SectionLength As Integer? = Nothing,
                                    Optional ByVal Channel As Integer? = Nothing) As Sound

                Try

                    If Channel Is Nothing Then

                        Dim Output As New Sound(InputSound.WaveFormat)

                        'Main section
                        For c = 1 To InputSound.WaveFormat.Channels

                            Dim InputSoundArray() As Single = InputSound.WaveData.SampleData(c)

                            Dim CorrectedStartSample = StartSample
                            Dim CorrectedSectionLength = SectionLength
                            CheckAndCorrectSectionLength(InputSoundArray.Length, CorrectedStartSample, CorrectedSectionLength)

                            'Creating a new sound array
                            Dim NewSoundArray(CorrectedSectionLength - 1) As Single

                            'Copies the sound
                            For s = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                                NewSoundArray(s - CorrectedStartSample) = InputSoundArray(s)
                            Next

                            'Replacing the sound array with the cropped sound array
                            Output.WaveData.SampleData(c) = NewSoundArray

                        Next

                        Return Output


                    Else

                        Dim Output As New Sound(New Formats.WaveFormat(InputSound.WaveFormat.SampleRate, InputSound.WaveFormat.BitDepth, 1,, InputSound.WaveFormat.Encoding))

                        'Main section
                        Dim InputSoundArray() As Single = InputSound.WaveData.SampleData(Channel)

                        Dim CorrectedStartSample = StartSample
                        Dim CorrectedSectionLength = SectionLength
                        CheckAndCorrectSectionLength(InputSoundArray.Length, CorrectedStartSample, CorrectedSectionLength)

                        'Creating a new sound array
                        Dim NewSoundArray(CorrectedSectionLength - 1) As Single

                        'Copies the sound
                        For s = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                            NewSoundArray(s - CorrectedStartSample) = InputSoundArray(s)
                        Next

                        'Referencing the new sound array in the output sound
                        Output.WaveData.SampleData(1) = NewSoundArray

                        Return Output

                    End If


                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try

            End Function


            ''' <summary>
            ''' Creates a new sound with all samples time reversed.
            ''' </summary>
            ''' <param name="inputSound"></param>
            Public Function ReverseSound(ByRef InputSound As Sound) As Sound

                Try

                    Dim OutputSound As New Sound(InputSound.WaveFormat)

                    For c = 1 To InputSound.WaveFormat.Channels

                        Dim NewChannelArray(InputSound.WaveData.SampleData(c).Length - 1) As Single

                        'Copying samples
                        Dim InversedSampleIndex As Integer = NewChannelArray.Length - 1
                        For s = 0 To NewChannelArray.Length - 1
                            NewChannelArray(InversedSampleIndex) = InputSound.WaveData.SampleData(c)(s)
                            InversedSampleIndex -= 1
                        Next

                        OutputSound.WaveData.SampleData(c) = NewChannelArray
                    Next

                    Return OutputSound

                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try

            End Function


            ''' <summary>
            ''' Removes all sections of a sound with a RMS level RelativeThresholdLevel dB lower than the loudest section of the sound. The sound level is only measured in the MeasurementChannel. The section deletions are, however, applied to all channels.
            ''' </summary>
            ''' <returns></returns>
            Public Function RemoveSilentSections(ByRef InputSound As Sound, ByVal RelativeThresholdLevel As Double,
                                                     ByRef SectionLength As Integer, ByRef MeasurementChannel As Integer,
                                                    Optional ByVal DefaultFadeDuration As Double = 0.01,
                                             Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z,
                                             Optional ByVal AllowReferenceOfInputSoundOnNoExclusion As Boolean = False) As Sound

                If InputSound.WaveData.SampleData(MeasurementChannel).Length < SectionLength Then
                    If AllowReferenceOfInputSoundOnNoExclusion = True Then
                        Return InputSound
                    Else
                        Return InputSound.CreateSoundDataCopy
                    End If
                End If

                Dim NumberOfSections As Integer = InputSound.WaveData.SampleData(MeasurementChannel).Length / SectionLength

                Dim SectionsToRemove As New List(Of Integer)

                Dim LoudestSectionLevel = GetLevelOfLoudestWindow(InputSound, MeasurementChannel, SectionLength,,,, FrequencyWeighting)
                Dim ExclutionThreshold As Double = LoudestSectionLevel - RelativeThresholdLevel

                For SectionIndex = 0 To NumberOfSections - 1

                    Dim CurrentLevel = MeasureSectionLevel(InputSound, MeasurementChannel, SectionIndex * SectionLength, SectionLength,,, FrequencyWeighting)
                    If CurrentLevel < ExclutionThreshold Then SectionsToRemove.Add(SectionIndex)

                Next

                'Sending back the input sound if no sections should be excluded
                If SectionsToRemove.Count = 0 Then
                    If AllowReferenceOfInputSoundOnNoExclusion = True Then
                        Return InputSound
                    Else
                        Return InputSound.CreateSoundDataCopy
                    End If
                End If

                'Summarizing sections to remove
                Dim JointExclusionSections As New List(Of Tuple(Of Integer, Integer)) 'Start sample and lengths of sections to be removed
                If SectionsToRemove.Count > 0 Then

                    Dim SuperSectionStartIndex As Integer = SectionsToRemove(0)
                    Dim LastRemoveSectionIndex As Integer = SectionsToRemove(0)
                    Dim NumberOfJointSections As Integer = 1
                    For n = 1 To SectionsToRemove.Count - 1

                        Dim IsNewSection As Boolean = False
                        If SectionsToRemove(n) <> LastRemoveSectionIndex + 1 Then IsNewSection = True

                        If IsNewSection Then

                            'Stores the old section
                            JointExclusionSections.Add(New Tuple(Of Integer, Integer)(SuperSectionStartIndex * SectionLength, NumberOfJointSections * SectionLength))

                            SuperSectionStartIndex = SectionsToRemove(n) 'Marking a new section start position
                            NumberOfJointSections = 1 'Noting that this is the first of a new set of joint sections
                        Else

                            'Incresing the number of sections to be joint, without changeing the start position
                            NumberOfJointSections += 1

                            'Storing the last section, even if its not a new section
                            If n = SectionsToRemove.Count - 1 Then JointExclusionSections.Add(New Tuple(Of Integer, Integer)(SuperSectionStartIndex * SectionLength, NumberOfJointSections * SectionLength))

                        End If

                        LastRemoveSectionIndex = SectionsToRemove(n)

                    Next
                Else
                    'Returns the input sound
                    If AllowReferenceOfInputSoundOnNoExclusion = True Then
                        Return InputSound
                    Else
                        Return InputSound.CreateSoundDataCopy
                    End If
                End If


                'Excluding sections, starting from the end of the sound
                Dim OutputSound = InputSound.CreateSoundDataCopy()
                OutputSound.SMA = Nothing 'Removing the SMA object as any such will not be correct anymore
                For n = 0 To JointExclusionSections.Count - 1

                    Dim Inverse_n As Integer = JointExclusionSections.Count - 1 - n

                    'Fading the inner its borders of the section
                    Dim FadeLength As Integer = Math.Min(Int(JointExclusionSections(Inverse_n).Item2 / 4), InputSound.WaveFormat.SampleRate * DefaultFadeDuration)
                    Dim StartFadeOutSample As Integer = JointExclusionSections(Inverse_n).Item1
                    DSP.Fade(OutputSound, 0, Nothing,, StartFadeOutSample, FadeLength, FadeSlopeType.Linear,, True)

                    'Fading the inner its borders of the section
                    Dim StartFadeInSample As Integer = JointExclusionSections(Inverse_n).Item1 + JointExclusionSections(Inverse_n).Item2
                    DSP.Fade(OutputSound, Nothing, 0, , StartFadeInSample, FadeLength, FadeSlopeType.Linear,, True)

                    'Deleting the section, 
                    DSP.DeleteSection(OutputSound, JointExclusionSections(Inverse_n).Item1 + FadeLength, JointExclusionSections(Inverse_n).Item2 - FadeLength * 2)

                Next

                'Returns the output sound
                Return OutputSound

            End Function

            ''' <summary>
            ''' Splits the input sound into different sounds determined by silent sections in the input sound. Silence is removed. Silence is defined as sections with a RMS level RelativeThresholdLevel dB lower than the loudest section of the sound. The sound level is only measured in the MeasurementChannel. The splitting is, however, applied to all channels.
            ''' </summary>
            ''' <returns></returns>
            Public Function SplitSoundOnSilence(ByRef InputSound As Sound, ByVal RelativeThresholdLevel As Double,
                                            ByRef SectionLength As Integer, ByRef MeasurementChannel As Integer,
                                            Optional ByVal DefaultFadeDuration As Double = 0.01,
                                            Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z,
                                            Optional ByVal AllowReferenceOfInputSoundOnNoExclusion As Boolean = False) As List(Of Sound)

                Dim Output As New List(Of Sound)

                If InputSound.WaveData.SampleData(MeasurementChannel).Length < SectionLength Then
                    If AllowReferenceOfInputSoundOnNoExclusion = True Then
                        Output.Add(InputSound)
                    Else
                        Output.Add(InputSound.CreateSoundDataCopy)
                    End If
                    Return Output
                End If

                Dim NumberOfSections As Integer = InputSound.WaveData.SampleData(MeasurementChannel).Length / SectionLength

                Dim SectionsToKeep As New List(Of Integer)

                Dim LoudestSectionLevel = GetLevelOfLoudestWindow(InputSound, MeasurementChannel, SectionLength,,,, FrequencyWeighting)
                Dim ExclutionThreshold As Double = LoudestSectionLevel - RelativeThresholdLevel

                Dim ExcludedSections As Integer = 0
                For SectionIndex = 0 To NumberOfSections - 1

                    Dim CurrentLevel = MeasureSectionLevel(InputSound, MeasurementChannel, SectionIndex * SectionLength, SectionLength,,, FrequencyWeighting)
                    If CurrentLevel >= ExclutionThreshold Then
                        SectionsToKeep.Add(SectionIndex)
                    Else
                        ExcludedSections += 1
                    End If

                Next

                'Sends back the original sound if no sections should be excluded
                If ExcludedSections = 0 Then
                    If AllowReferenceOfInputSoundOnNoExclusion = True Then
                        Output.Add(InputSound)
                    Else
                        Output.Add(InputSound.CreateSoundDataCopy)
                    End If
                    Return Output
                End If

                'Summarizing sections to keep
                Dim JointInclusionSections As New List(Of Tuple(Of Integer, Integer)) 'Start sample and lengths of sections to keep
                If SectionsToKeep.Count < NumberOfSections Then

                    Dim SuperSectionStartIndex As Integer = SectionsToKeep(0)
                    Dim LastKeepSectionIndex As Integer = SectionsToKeep(0)
                    Dim NumberOfJointSections As Integer = 1
                    For n = 1 To SectionsToKeep.Count - 1

                        Dim IsNewSection As Boolean = False
                        If SectionsToKeep(n) <> LastKeepSectionIndex + 1 Then IsNewSection = True

                        If IsNewSection Then

                            'Stores the old section
                            JointInclusionSections.Add(New Tuple(Of Integer, Integer)(SuperSectionStartIndex * SectionLength, NumberOfJointSections * SectionLength))

                            SuperSectionStartIndex = SectionsToKeep(n) 'Marking a new section start position
                            NumberOfJointSections = 1 'Noting that this is the first of a new set of joint sections
                        Else

                            'Incresing the number of sections to be joint, without changing the start position
                            NumberOfJointSections += 1

                            'Storing the last section, even if its not a new section
                            If n = SectionsToKeep.Count - 1 Then JointInclusionSections.Add(New Tuple(Of Integer, Integer)(SuperSectionStartIndex * SectionLength, NumberOfJointSections * SectionLength))

                        End If

                        LastKeepSectionIndex = SectionsToKeep(n)

                    Next
                Else
                    If AllowReferenceOfInputSoundOnNoExclusion = True Then
                        Output.Add(InputSound)
                    Else
                        Output.Add(InputSound.CreateSoundDataCopy)
                    End If
                    Return Output
                End If


                'Copying sections to keep into new sounds
                For n = 0 To JointInclusionSections.Count - 1

                    Dim StartIncludeSample As Integer = JointInclusionSections(n).Item1
                    Dim IncludeLength As Integer = JointInclusionSections(n).Item2

                    'Deleting the section, 
                    Dim NewSound = DSP.CopySection(InputSound, JointInclusionSections(n).Item1, JointInclusionSections(n).Item2)

                    'Fading the new sound/section
                    If DefaultFadeDuration > 0 Then

                        'Fading in the section
                        Dim FadeLength As Integer = Math.Min(Int(NewSound.WaveData.SampleData(1).Length / 4), NewSound.WaveFormat.SampleRate * DefaultFadeDuration)
                        DSP.Fade(NewSound, Nothing, 0, , 0, FadeLength, FadeSlopeType.Linear,, True)

                        'Fading the inner its borders of the section
                        Dim StartFadeOutSample As Integer = JointInclusionSections(n).Item1
                        DSP.Fade(NewSound, 0, Nothing,, NewSound.WaveData.SampleData(1).Length - FadeLength, FadeLength, FadeSlopeType.Linear,, True)

                    End If

                    'Adding the section only if its length is larger than 0 samples
                    If NewSound.WaveData.SampleData(1).Length > 0 Then Output.Add(NewSound)

                Next

                'Returns the output sounds
                Return Output

            End Function

            ''' <summary>
            ''' Calculates the proportion of the input sound which contains silence. Silence is defined as sections with a RMS level RelativeThresholdLevel dB lower than the loudest section of the sound. 
            ''' </summary>
            ''' <returns></returns>
            Public Function GetSilenceRatio(ByRef InputSound As Sound, ByVal RelativeThresholdLevel As Double,
                                        ByRef SectionLength As Integer, ByRef MeasurementChannel As Integer,
                                        Optional ByVal Frequencyweighting As FrequencyWeightings = FrequencyWeightings.Z) As Double

                'Returning 0 if the sound is too short to measure
                If InputSound.WaveData.SampleData(MeasurementChannel).Length < SectionLength Then Return 0

                Dim NumberOfSections As Integer = InputSound.WaveData.SampleData(MeasurementChannel).Length / SectionLength

                Dim LoudestSectionLevel = GetLevelOfLoudestWindow(InputSound, MeasurementChannel, SectionLength,,,, Frequencyweighting)
                Dim SilenceThreshold As Double = LoudestSectionLevel - RelativeThresholdLevel

                Dim SilentSectionCount As Integer = 0
                For SectionIndex = 0 To NumberOfSections - 1

                    Dim CurrentLevel = MeasureSectionLevel(InputSound, MeasurementChannel, SectionIndex * SectionLength, SectionLength,,, Frequencyweighting)
                    If CurrentLevel < SilenceThreshold Then SilentSectionCount += 1

                Next

                Return SilentSectionCount / NumberOfSections

            End Function


            ''' <summary>
            ''' Calculates the variation of sound levels between non-silent sections of the input sound. 
            ''' </summary>
            ''' <returns></returns>
            Public Function GetSoundLevelVariation(ByRef InputSound As Sound, ByRef SectionLength As Integer, ByRef MeasurementChannel As Integer,
                                              Optional ByVal ExcludeSilenctSections As Boolean = False, Optional ByVal RelativeSilenceThresholdLevel As Double = 30,
                                               Optional ByVal Frequencyweighting As FrequencyWeightings = FrequencyWeightings.Z) As Double
                Try


                    'Returning 0 if the sound is too short to measure (there is less than one section and therefore no variation)
                    If InputSound.WaveData.SampleData(MeasurementChannel).Length < SectionLength Then Return 0

                    Dim SoundLevelList As New List(Of Double)
                    Dim NumberOfSections As Integer = InputSound.WaveData.SampleData(MeasurementChannel).Length / SectionLength

                    If ExcludeSilenctSections = False Then

                        For SectionIndex = 0 To NumberOfSections - 1
                            Dim CurrentLevel = MeasureSectionLevel(InputSound, MeasurementChannel, SectionIndex * SectionLength, SectionLength,,, Frequencyweighting)
                            SoundLevelList.Add(CurrentLevel)
                        Next

                    Else
                        Dim LoudestSectionLevel = GetLevelOfLoudestWindow(InputSound, MeasurementChannel, SectionLength,,,, Frequencyweighting)
                        Dim SilenceThreshold As Double = LoudestSectionLevel - RelativeSilenceThresholdLevel

                        For SectionIndex = 0 To NumberOfSections - 1
                            Dim CurrentLevel = MeasureSectionLevel(InputSound, MeasurementChannel, SectionIndex * SectionLength, SectionLength,,, Frequencyweighting)
                            If CurrentLevel > SilenceThreshold Then SoundLevelList.Add(CurrentLevel)
                        Next

                    End If


                    Dim StandardDeviation As Double
                    Dim CV As Double = Utils.CoefficientOfVariation(SoundLevelList,,,,, StandardDeviation)

                    Return StandardDeviation

                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try

            End Function

            Public Function ExtentSoundByCopyAndCrossFade(ByRef OriginalSound As Sound, ByRef FinalLength As Integer, Optional ByVal CrossFadeLength As Integer? = Nothing) As Sound

                Try

                    Dim ExtendedSound As New Sound(OriginalSound.WaveFormat)
                    Dim ExtendedSoundSampleArray(FinalLength - 1) As Single
                    ExtendedSound.WaveData.SampleData(1) = ExtendedSoundSampleArray

                    'By default, the shortest of a half second and a tenth of the original sound is used for crossfade
                    If CrossFadeLength Is Nothing Then CrossFadeLength = Math.Min(OriginalSound.WaveFormat.SampleRate * 0.5, Int(OriginalSound.WaveData.SampleData(1).Length / 10))
                    Dim CentralSectionLength As Integer = OriginalSound.WaveData.SampleData(1).Length - 2 * CrossFadeLength

                    Dim WriteSample As Integer = 0

                    'Copying the first bit
                    For n = 0 To CrossFadeLength - 1
                        ExtendedSoundSampleArray(WriteSample) = OriginalSound.WaveData.SampleData(1)(n)
                        WriteSample += 1
                    Next

                    Do
                        For s = CrossFadeLength To OriginalSound.WaveData.SampleData(1).Length - CrossFadeLength - 1
                            ExtendedSoundSampleArray(WriteSample) = OriginalSound.WaveData.SampleData(1)(s)
                            WriteSample += 1
                            If WriteSample = ExtendedSoundSampleArray.Length Then Exit Do
                        Next

                        Dim CrossFadeProgress As Integer = 0
                        For s = 0 To CrossFadeLength - 1
                            ExtendedSoundSampleArray(WriteSample) =
                            (CrossFadeProgress / CrossFadeLength) * OriginalSound.WaveData.SampleData(1)(s) +
                            (1 - (CrossFadeProgress / CrossFadeLength)) * OriginalSound.WaveData.SampleData(1)(CrossFadeLength + CentralSectionLength + s)
                            WriteSample += 1
                            CrossFadeProgress += 1
                            If WriteSample = ExtendedSoundSampleArray.Length Then Exit Do
                        Next
                    Loop

                    Return ExtendedSound

                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try

            End Function

            Public Sub InsertSound(ByRef SoundToInsert As Sound, ByRef TargetSound As Sound,
                                   ByVal TargetChannel As Integer?,
                               ByVal StartSample As Integer,
                               Optional ByVal FadeInCrossFadeLength As Integer = 0,
                               Optional ByVal FadeOutCrossFadeLength As Integer = 0,
                               Optional ByVal CrossFadeSlopeType As FadeSlopeType = FadeSlopeType.Smooth,
                               Optional ByVal CheckForDifferentSoundFormats As Boolean = True,
                               Optional ByVal EqualPowerFade As Boolean = False,
                               Optional ByVal TargetSoundAttenuation As Double? = Nothing)
                Try

                    MsgBox("This function needs to be properly debugged! Then is overload without channel specification can be removed.")

                    Dim StartChannel As Integer
                    Dim LastChannel As Integer

                    If TargetChannel.HasValue Then

                        StartChannel = TargetChannel
                        LastChannel = TargetChannel

                        'Checks that the sound to insert is mono
                        If SoundToInsert.WaveFormat.Channels <> 1 Then
                            Throw New ArgumentException("When a target channel is specified, the sound to insert can only be mono. Aborting!")
                        End If

                        If CheckForDifferentSoundFormats = True Then
                            'This check allows different channel counts
                            If SoundToInsert.WaveFormat.SampleRate <> TargetSound.WaveFormat.SampleRate Or
                                SoundToInsert.WaveFormat.BitDepth <> TargetSound.WaveFormat.BitDepth Or
                                SoundToInsert.WaveFormat.Encoding <> TargetSound.WaveFormat.Encoding Then
                                Throw New ArgumentException("Different formats in InsertSound sounds. Aborting!")
                            End If
                        End If
                    Else

                        StartChannel = 1
                        LastChannel = TargetSound.WaveFormat.Channels

                        If CheckForDifferentSoundFormats = True Then
                            If SoundToInsert.WaveFormat.SampleRate <> TargetSound.WaveFormat.SampleRate Or
                                SoundToInsert.WaveFormat.BitDepth <> TargetSound.WaveFormat.BitDepth Or
                                SoundToInsert.WaveFormat.Encoding <> TargetSound.WaveFormat.Encoding Or
                                SoundToInsert.WaveFormat.Channels <> TargetSound.WaveFormat.Channels Then
                                Throw New ArgumentException("Different formats in InsertSound sounds. Aborting!")
                            End If
                        End If

                    End If


                    'Dim SoundToInsertLength As Integer = SoundToInsert.WaveData.ShortestChannelSampleCount
                    Dim SoundToInsertLength As Integer = SoundToInsert.WaveData.LongestChannelSampleCount

                    'Fading out Target sound
                    Fade(TargetSound, 0, TargetSoundAttenuation, TargetChannel, StartSample - FadeInCrossFadeLength, FadeInCrossFadeLength, CrossFadeSlopeType,, EqualPowerFade)

                    'Fading in target sound
                    Fade(TargetSound, TargetSoundAttenuation, 0, TargetChannel, StartSample + SoundToInsertLength - FadeInCrossFadeLength - FadeOutCrossFadeLength, FadeOutCrossFadeLength, CrossFadeSlopeType,, EqualPowerFade)

                    'Fading In SoundToInsert 
                    Fade(SoundToInsert, Nothing, 0, , 0, FadeInCrossFadeLength, CrossFadeSlopeType,, EqualPowerFade)

                    'Fading Out SoundToInsert 
                    Fade(SoundToInsert, 0, Nothing, , SoundToInsert.WaveData.LongestChannelSampleCount - FadeOutCrossFadeLength, FadeOutCrossFadeLength, CrossFadeSlopeType,, EqualPowerFade)

                    'Adding in the first fade section
                    Dim LocalSampleIndex As Integer = 0
                    For c = StartChannel To LastChannel

                        Dim SourceSoundArray As Single()
                        If TargetChannel.HasValue Then
                            SourceSoundArray = SoundToInsert.WaveData.SampleData(1)
                        Else
                            SourceSoundArray = SoundToInsert.WaveData.SampleData(c)
                        End If

                        'Skipping to next if the channel is empty
                        If SourceSoundArray.Length = 0 Then Continue For

                        Dim TargetSoundArray = TargetSound.WaveData.SampleData(c)

                        LocalSampleIndex = 0
                        For s = StartSample - FadeInCrossFadeLength To StartSample - 1
                            TargetSoundArray(s) += SourceSoundArray(LocalSampleIndex)
                            LocalSampleIndex += 1
                        Next
                    Next

                    If TargetSoundAttenuation Is Nothing Then

                        'Replacing sound in between fade sections
                        For c = StartChannel To LastChannel

                            Dim SourceSoundArray As Single()
                            If TargetChannel.HasValue Then
                                SourceSoundArray = SoundToInsert.WaveData.SampleData(1)
                            Else
                                SourceSoundArray = SoundToInsert.WaveData.SampleData(c)
                            End If


                            'Skipping to next if the channel is empty
                            If SourceSoundArray.Length = 0 Then Continue For

                            Dim TargetSoundArray = TargetSound.WaveData.SampleData(c)

                            LocalSampleIndex = FadeInCrossFadeLength
                            For s = StartSample To StartSample + SoundToInsertLength - (FadeInCrossFadeLength + FadeOutCrossFadeLength) - 1
                                TargetSoundArray(s) = SourceSoundArray(LocalSampleIndex)
                                LocalSampleIndex += 1
                            Next
                        Next

                    Else

                        'Attenuating the Target sound insert region using the fade function
                        Fade(TargetSound, TargetSoundAttenuation, TargetSoundAttenuation, TargetChannel, StartSample, SoundToInsertLength - (FadeInCrossFadeLength + FadeOutCrossFadeLength), CrossFadeSlopeType,, EqualPowerFade)

                        'Adding sound in between fade sections
                        For c = StartChannel To LastChannel
                            Dim SourceSoundArray As Single()
                            If TargetChannel.HasValue Then
                                SourceSoundArray = SoundToInsert.WaveData.SampleData(1)
                            Else
                                SourceSoundArray = SoundToInsert.WaveData.SampleData(c)
                            End If

                            'Skipping to next if the channel is empty
                            If SourceSoundArray.Length = 0 Then Continue For

                            Dim TargetSoundArray = TargetSound.WaveData.SampleData(c)

                            LocalSampleIndex = FadeInCrossFadeLength
                            For s = StartSample To StartSample + SoundToInsertLength - (FadeInCrossFadeLength + FadeOutCrossFadeLength) - 1
                                TargetSoundArray(s) += SourceSoundArray(LocalSampleIndex)
                                LocalSampleIndex += 1
                            Next
                        Next

                    End If


                    'Adding in the second fade section
                    For c = StartChannel To LastChannel
                        Dim SourceSoundArray As Single()
                        If TargetChannel.HasValue Then
                            SourceSoundArray = SoundToInsert.WaveData.SampleData(1)
                        Else
                            SourceSoundArray = SoundToInsert.WaveData.SampleData(c)
                        End If

                        'Skipping to next if the channel is empty
                        If SourceSoundArray.Length = 0 Then Continue For

                        Dim TargetSoundArray = TargetSound.WaveData.SampleData(c)

                        LocalSampleIndex = SoundToInsertLength - FadeOutCrossFadeLength
                        For s = StartSample + SoundToInsertLength - FadeInCrossFadeLength - FadeOutCrossFadeLength To StartSample + SoundToInsertLength - FadeInCrossFadeLength - 1
                            TargetSoundArray(s) += SourceSoundArray(LocalSampleIndex)
                            LocalSampleIndex += 1
                        Next
                    Next

                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try


            End Sub



            Public Sub InsertSound(ByRef SoundToInsert As Sound, ByRef TargetSound As Sound,
                               ByVal StartSample As Integer,
                               Optional ByVal FadeInCrossFadeLength As Integer = 0,
                               Optional ByVal FadeOutCrossFadeLength As Integer = 0,
                               Optional ByVal CrossFadeSlopeType As FadeSlopeType = FadeSlopeType.Smooth,
                               Optional ByVal CheckForDifferentSoundFormats As Boolean = True,
                               Optional ByVal EqualPowerFade As Boolean = False,
                               Optional ByVal TargetSoundAttenuation As Double? = Nothing)
                Try

                    If CheckForDifferentSoundFormats = True Then
                        If SoundToInsert.WaveFormat.SampleRate <> TargetSound.WaveFormat.SampleRate Or
                                SoundToInsert.WaveFormat.BitDepth <> TargetSound.WaveFormat.BitDepth Or
                                SoundToInsert.WaveFormat.Encoding <> TargetSound.WaveFormat.Encoding Or
                                SoundToInsert.WaveFormat.Channels <> TargetSound.WaveFormat.Channels Then
                            Throw New ArgumentException("Different formats in InsertSound sounds. Aborting!")
                        End If
                    End If

                    'Dim SoundToInsertLength As Integer = SoundToInsert.WaveData.ShortestChannelSampleCount
                    Dim SoundToInsertLength As Integer = SoundToInsert.WaveData.LongestChannelSampleCount

                    'Fading out Target sound
                    Fade(TargetSound, 0, TargetSoundAttenuation,, StartSample - FadeInCrossFadeLength, FadeInCrossFadeLength, CrossFadeSlopeType,, EqualPowerFade)

                    'Fading in target sound
                    Fade(TargetSound, TargetSoundAttenuation, 0,, StartSample + SoundToInsertLength - FadeInCrossFadeLength - FadeOutCrossFadeLength, FadeOutCrossFadeLength, CrossFadeSlopeType,, EqualPowerFade)

                    'Fading In SoundToInsert 
                    Fade(SoundToInsert, Nothing, 0,, 0, FadeInCrossFadeLength, CrossFadeSlopeType,, EqualPowerFade)

                    'Fading Out SoundToInsert 
                    Fade(SoundToInsert, 0, Nothing,, SoundToInsert.WaveData.LongestChannelSampleCount - FadeOutCrossFadeLength, FadeOutCrossFadeLength, CrossFadeSlopeType,, EqualPowerFade)

                    'Adding in the first fade section
                    Dim LocalSampleIndex As Integer = 0
                    For c = 1 To SoundToInsert.WaveFormat.Channels
                        Dim SourceSoundArray = SoundToInsert.WaveData.SampleData(c)

                        'Skipping to next if the channel is empty
                        If SourceSoundArray.Length = 0 Then Continue For

                        Dim TargetSoundArray = TargetSound.WaveData.SampleData(c)

                        LocalSampleIndex = 0
                        For s = StartSample - FadeInCrossFadeLength To StartSample - 1
                            TargetSoundArray(s) += SourceSoundArray(LocalSampleIndex)
                            LocalSampleIndex += 1
                        Next
                    Next

                    If TargetSoundAttenuation Is Nothing Then

                        'Replacing sound in between fade sections
                        For c = 1 To SoundToInsert.WaveFormat.Channels
                            Dim SourceSoundArray = SoundToInsert.WaveData.SampleData(c)

                            'Skipping to next if the channel is empty
                            If SourceSoundArray.Length = 0 Then Continue For

                            Dim TargetSoundArray = TargetSound.WaveData.SampleData(c)

                            LocalSampleIndex = FadeInCrossFadeLength
                            For s = StartSample To StartSample + SoundToInsertLength - (FadeInCrossFadeLength + FadeOutCrossFadeLength) - 1
                                TargetSoundArray(s) = SourceSoundArray(LocalSampleIndex)
                                LocalSampleIndex += 1
                            Next
                        Next

                    Else

                        'Attenuating the Target sound insert region using the fade function
                        Fade(TargetSound, TargetSoundAttenuation, TargetSoundAttenuation,, StartSample, SoundToInsertLength - (FadeInCrossFadeLength + FadeOutCrossFadeLength), CrossFadeSlopeType,, EqualPowerFade)

                        'Adding sound in between fade sections
                        For c = 1 To SoundToInsert.WaveFormat.Channels
                            Dim SourceSoundArray = SoundToInsert.WaveData.SampleData(c)

                            'Skipping to next if the channel is empty
                            If SourceSoundArray.Length = 0 Then Continue For

                            Dim TargetSoundArray = TargetSound.WaveData.SampleData(c)

                            LocalSampleIndex = FadeInCrossFadeLength
                            For s = StartSample To StartSample + SoundToInsertLength - (FadeInCrossFadeLength + FadeOutCrossFadeLength) - 1
                                TargetSoundArray(s) += SourceSoundArray(LocalSampleIndex)
                                LocalSampleIndex += 1
                            Next
                        Next

                    End If


                    'Adding in the second fade section
                    For c = 1 To SoundToInsert.WaveFormat.Channels
                        Dim SourceSoundArray = SoundToInsert.WaveData.SampleData(c)

                        'Skipping to next if the channel is empty
                        If SourceSoundArray.Length = 0 Then Continue For

                        Dim TargetSoundArray = TargetSound.WaveData.SampleData(c)

                        LocalSampleIndex = SoundToInsertLength - FadeOutCrossFadeLength
                        For s = StartSample + SoundToInsertLength - FadeInCrossFadeLength - FadeOutCrossFadeLength To StartSample + SoundToInsertLength - FadeInCrossFadeLength - 1
                            TargetSoundArray(s) += SourceSoundArray(LocalSampleIndex)
                            LocalSampleIndex += 1
                        Next
                    Next

                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try


            End Sub



            ''' <summary>
            ''' Normalizes the absolute maximum amplitude of a section of the full scale value of the current sound format.
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="channel"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength"></param>
            ''' <param name="NormalizeChannelsSeparately">If set to true, the channels will be indivudually normalized. If left to false, the same amout of gain will be appplied to all channels.</param>
            ''' <returns>Returns the number of distorted samples, or vbNull if something went wrong.</returns>
            Public Function MaxAmplitudeNormalizeSection(ByRef InputSound As Sound, Optional ByVal channel As Integer? = Nothing,
                                        Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing,
                                                     Optional NormalizeChannelsSeparately As Boolean = False) As Double


                Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, channel)

                Dim totalDistortedSamples As Double = 0

                'Main section
                Dim AbsoluteMaxAmplitudeBothChannels As Double = 0
                For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                    Dim CorrectedStartSample = startSample
                    Dim CorrectedSectionLength = sectionLength
                    CheckAndCorrectSectionLength(InputSound.WaveData.SampleData(c).Length, CorrectedStartSample, CorrectedSectionLength)

                    'Measures the level of the channel section
                    Dim AbsoluteMaxAmplitude As Double = MeasureSectionLevel(InputSound, c, CorrectedStartSample, CorrectedSectionLength, SoundDataUnit.linear,
                                                                  SoundMeasurementType.AbsolutePeakAmplitude, FrequencyWeightings.Z)


                    If NormalizeChannelsSeparately = True Then

                        'Calculating the needed gain
                        Dim Gain As Double = AbsoluteMaxAmplitude / InputSound.WaveFormat.PositiveFullScale

                        'Amplifies the section
                        totalDistortedSamples += AmplifySection(InputSound, Gain, c, CorrectedStartSample, CorrectedSectionLength, SoundDataUnit.linear)

                    Else
                        If AbsoluteMaxAmplitude > AbsoluteMaxAmplitudeBothChannels Then AbsoluteMaxAmplitudeBothChannels = AbsoluteMaxAmplitude
                    End If

                Next

                If NormalizeChannelsSeparately = False Then

                    'Calculating the needed gain
                    Dim Gain As Double = InputSound.WaveFormat.PositiveFullScale / AbsoluteMaxAmplitudeBothChannels

                    'Amplifies the section
                    totalDistortedSamples += AmplifySection(InputSound, Gain, , startSample, sectionLength, SoundDataUnit.linear)

                End If

                Return totalDistortedSamples

            End Function


            Public Sub EqualizeLevelOverTime(ByRef InputSound As Sound, ByVal Channel As Integer, ByVal MeasurementSectionLength As Integer,
                                    Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z)

                'Overlap length will be 1/4 of the MeasurementSectionLength to agree with the Tukey windowing r of 0.5
                Dim OverlapLength As Integer = MeasurementSectionLength / 4
                Dim WindowCount As Integer = Int(InputSound.WaveData.SampleData(Channel).Length / (MeasurementSectionLength - OverlapLength))

                'Getting the average level of the input sound
                Dim averageLevel As Double = MeasureSectionLevel(InputSound, Channel,,,,, FrequencyWeighting)

                'Setting each time window to the average level (if it is not silent)
                Dim TimeWindowArrays As New List(Of Single())
                Dim MeasurementSound As New Sound(InputSound.WaveFormat)

                Dim StartSample As Integer = 0
                For n = 0 To WindowCount - 1 'Skipping the last window since it may not be full

                    Dim NewTimeWindowArray(MeasurementSectionLength - 1) As Single

                    'Copying samples to it
                    Dim LocalSampleIndex As Integer = 0
                    For s = StartSample To StartSample + MeasurementSectionLength - 1
                        NewTimeWindowArray(LocalSampleIndex) = InputSound.WaveData.SampleData(Channel)(s)
                        LocalSampleIndex += 1
                    Next

                    MeasurementSound.WaveData.SampleData(Channel) = NewTimeWindowArray
                    MeasureAndAdjustSectionLevel(MeasurementSound, averageLevel, Channel,,, FrequencyWeighting)

                    'Windowing the array
                    WindowingFunction(NewTimeWindowArray, WindowingType.Tukey,, 0.5)

                    TimeWindowArrays.Add(NewTimeWindowArray)

                    StartSample += (MeasurementSectionLength - OverlapLength)

                Next

                'Adding and overlapping sound
                Dim OutputArray(InputSound.WaveData.SampleData(Channel).Length - 1) As Single

                StartSample = 0
                For n = 0 To WindowCount - 1 'Skipping the last window since it may not be full

                    'Copying the samples to the output array
                    Dim LocalSampleIndex As Integer = 0
                    For s = StartSample To StartSample + MeasurementSectionLength - 1
                        OutputArray(s) += TimeWindowArrays(n)(LocalSampleIndex)
                        LocalSampleIndex += 1
                    Next

                    'Jumping back the index
                    StartSample += (MeasurementSectionLength - OverlapLength)

                Next


                InputSound.WaveData.SampleData(Channel) = OutputArray

                'Resetting the original overall level
                MeasureAndAdjustSectionLevel(InputSound, averageLevel, Channel, 0, WindowCount * (MeasurementSectionLength - OverlapLength), FrequencyWeighting)

            End Sub


            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="targetLevel"></param>
            ''' <param name="channel"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength"></param>
            ''' <param name="FrequencyWeighting"></param>
            ''' <returns>Returns the number of distorted samples, or vbNull if something went wrong.</returns>
            Public Function MeasureAndAdjustSectionLevel_OLD(ByRef InputSound As Sound, ByVal targetLevel As Decimal, Optional ByVal channel As Integer? = Nothing,
                                        Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing,
                                                 Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z) As Double


                Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, channel)

                Dim totalDistortedSamples As Double = 0

                'Main section
                For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                    Dim inputArray() As Single = InputSound.WaveData.SampleData(c)

                    Dim CorrectedStartSample = startSample
                    Dim CorrectedSectionLength = sectionLength
                    CheckAndCorrectSectionLength(inputArray.Length, CorrectedStartSample, CorrectedSectionLength)

                    'Copying the input sound to a new temporary sound
                    Dim tempSound As Sound = InputSound.CreateCopy

                    'Filterring for Weighing
                    tempSound = DSP.IIRFilter(tempSound, FrequencyWeighting, channel)
                    If tempSound Is Nothing Then Return vbNull
                    Dim measurementArray() As Single = tempSound.WaveData.SampleData(c)

                    Dim AccumulativeSoundLevel As Double ' previously long

                    'Beräknar RMS för sectionen
                    For n = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                        AccumulativeSoundLevel = AccumulativeSoundLevel + measurementArray(n) ^ 2
                    Next
                    Dim RMS = (AccumulativeSoundLevel / CorrectedSectionLength) ^ (1 / 2)
                    Dim sectionRMS As Double = RMS

                    'Beräkna vad TargetdBFS dB är i RMS
                    Dim targetRMS = dBConversion(targetLevel, dBConversionDirection.from_dB, InputSound.WaveFormat)

                    'Beräknar förstärkningsgraden
                    Dim Gain As Double = 1
                    If Not sectionRMS = 0 Then
                        Gain = targetRMS / sectionRMS
                    Else
                        AudioError("Filen som skulle förstärkas till " & targetLevel & " dBFS var helt tyst!", "Error In modifySound.measureAndAdjustSectionRMS")
                    End If

                    'Verkställer förstärkningen i sektionen (och varnar för distorsion)
                    Dim distorsion As Boolean = False
                    Dim distorsionSampleCount As Integer = 0
                    Dim otherError As Boolean = False
                    Dim otherErrorExeption As Exception = Nothing

                    For n = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1 'tidigare användes: mixNumS - 1
                        Try
                            inputArray(n) = (inputArray(n) * Gain)
                        Catch ex As Exception
                            Try
                                If inputArray(n) * Gain > Double.MaxValue Then inputArray(n) = Double.MaxValue
                                If inputArray(n) * Gain < Double.MinValue Then inputArray(n) = Double.MinValue
                                distorsion = True
                                distorsionSampleCount += 1

                            Catch otherErrorExeption
                                otherError = True
                            End Try
                        End Try
                    Next

                    totalDistortedSamples += distorsionSampleCount

                    'If distorsion = True Then MsgBox("Distorsion occurred for " & distorsionSampleCount & " samples in channel " & c & " in MeasureAndAdjustSectionLevel", "Warning!")
                    'If otherError = True Then AudioError("Fel i modifySound.MeasureAndAdjustSectionLevel. Sound index:  " & soundIndex & ", channel: " & c & ". " & vbCr & otherErrorExeption.ToString)

                Next

                Return totalDistortedSamples

            End Function

            ''' <summary>
            ''' Fading the indicated section of the indicated sound using the specified fading type.
            ''' </summary>
            ''' <param name="input">The sound to be modified.</param>
            ''' <param name="startLevel">The indended start level (in dBFS) of the fade section. If left empty, it is set to averageArrayLevel.</param>
            ''' <param name="endLevel">The indended end level (in dBFS) of the fade section. If left empty, it is set to averageArrayLevel.</param>
            ''' <param name="averageArrayLevel">This parameter (in dBFS) is needed in order to determine how much attenuation/gain should be applied to the fade section.
            ''' If averageArrayLevel is not specified by the calling code, Fade performs an RSM measurement on the fade section.</param>
            ''' <param name="channel">The channel to be modified. If left empty all channels will be modified.</param>
            ''' <param name="startSample">The start sample of the section to fade.</param>
            ''' <param name="sectionLength">The length (in samples) of the section to fade.</param>
            ''' <param name="type">Fade type. SilenceToEndLevel fades from silence to the specified endLevel. StartLevelToSilence fades from the specified startLevel to silence.
            ''' Gradual fades from the specified startLevel to the specified endLevel. SilenceWholeSection does just that.</param>
            ''' <param name="slopeType">Specifies the curvature of the fade section. Linear creates a linear fade, and Smooth fades using a cosine function to smoothen out the fade section.</param>
            Public Sub FadeToLevels(ByRef input As Sound, Optional ByVal startLevel As Double? = Nothing, Optional ByVal endLevel As Double? = Nothing,
                        Optional ByVal averageArrayLevel As Double? = Nothing, Optional ByVal channel As Integer? = Nothing,
                Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing,
                Optional ByVal type As FadeType = FadeType.Gradual,
                Optional ByVal slopeType As FadeSlopeType = FadeSlopeType.Smooth)

                'Ska uppdateras med kurvtyp, linjär ska läggas till som val
                'Även "micro255 law" ska läggas till, Equation 22-1 i dspguide.com: y=(ln(1+0.000001*x))/(ln(1+0.000001)) ' Skapar en naturlig fadening genom "compounding"
                'även en alternativ ekvation finns "A Law" (Stämmer detta. Borde nog byta input och output, jämfört med grafen i boken)

                Try

                    Dim AudioOutputConstructor As New AudioOutputConstructor(input.WaveFormat, channel)

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim CorrectedStartSample = startSample
                        Dim CorrectedSectionLength = sectionLength
                        CheckAndCorrectSectionLength(input.WaveData.SampleData(c).Length, CorrectedStartSample, CorrectedSectionLength)

                        'If needed, measuring the average level of the input section if this is not specified by the calling code
                        Select Case type
                            Case FadeType.Gradual, FadeType.StartLevelToSilence, FadeType.SilenceToEndLevel
                                If averageArrayLevel Is Nothing Then averageArrayLevel = DSP.MeasureSectionLevel(input, c, CorrectedStartSample, CorrectedSectionLength)
                                If startLevel Is Nothing Then startLevel = averageArrayLevel
                                If endLevel Is Nothing Then startLevel = averageArrayLevel
                        End Select

                        Dim inputArray() As Single = input.WaveData.SampleData(c)

                        'Main section
                        Dim startFactor As Double
                        Dim endFactor As Double

                        Select Case type
                            Case FadeType.Gradual
                                startFactor = dBConversion(startLevel, dBConversionDirection.from_dB, input.WaveFormat) /
                    dBConversion(averageArrayLevel, dBConversionDirection.from_dB, input.WaveFormat)
                                endFactor = dBConversion(endLevel, dBConversionDirection.from_dB, input.WaveFormat) /
                    dBConversion(averageArrayLevel, dBConversionDirection.from_dB, input.WaveFormat)
                            Case FadeType.FadeOut
                                startFactor = 1
                                endFactor = 0
                            Case FadeType.FadeIn
                                startFactor = 0
                                endFactor = 1
                            Case FadeType.StartLevelToSilence
                                startFactor = dBConversion(startLevel, dBConversionDirection.from_dB, input.WaveFormat) /
                            dBConversion(averageArrayLevel, dBConversionDirection.from_dB, input.WaveFormat)
                                endFactor = 0
                            Case FadeType.SilenceToEndLevel
                                startFactor = 0
                                endFactor = dBConversion(endLevel, dBConversionDirection.from_dB, input.WaveFormat) /
                        dBConversion(averageArrayLevel, dBConversionDirection.from_dB, input.WaveFormat)
                            Case FadeType.SilenceWholeSection
                                startFactor = 0
                                endFactor = 0
                        End Select

                        Dim fadeSampleCount As Integer = 0
                        Dim fadeProgress As Double = 0

                        Dim distorsion As Boolean = False
                        Dim distorsionSampleCount As Integer = 0
                        Dim otherError As Boolean = False
                        Dim otherErrorExeption As Exception = Nothing

                        For currentSample = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1

                            'fadeProgress goes from 0 to 1 during the fade section
                            fadeProgress = fadeSampleCount / (CorrectedSectionLength - 1)

                            'Modifies currentFadeFactor according to a cosine finction, whereby currentModFactor starts on 1 and end at 0
                            Dim currentModFactor As Double
                            Dim currentFadeFactor As Double
                            If slopeType = FadeSlopeType.Smooth Then
                                currentModFactor = ((Math.Cos(twopi * (fadeProgress / 2)) + 1) / 2)
                                currentFadeFactor = startFactor * currentModFactor + endFactor * (1 - currentModFactor)
                            Else
                                currentFadeFactor = startFactor * (1 - fadeProgress) + endFactor * fadeProgress
                            End If


                            'Fading the section
                            If (inputArray(currentSample) * currentFadeFactor) > Single.MaxValue Then
                                inputArray(currentSample) = Single.MaxValue
                                distorsion = True
                                distorsionSampleCount += 1

                            ElseIf (inputArray(currentSample) * currentFadeFactor) < Single.MinValue Then
                                inputArray(currentSample) = Single.MinValue
                                distorsion = True
                                distorsionSampleCount += 1

                            Else
                                inputArray(currentSample) = (inputArray(currentSample) * currentFadeFactor)

                            End If

                            fadeSampleCount += 1

                        Next

                        If distorsion = True Then AudioError("Distorsion occurred for " & distorsionSampleCount & " samples in channel " & c & " in Fade")
                        If otherError = True Then AudioError("Error in Fade, channel: " & c & ". " & vbCr & otherErrorExeption.ToString)

                    Next

                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try


            End Sub

            ''' <summary>
            ''' Creates an interrupted sound from the InputSound. 
            ''' </summary>
            ''' <param name="InputSound">The sound to interrupt.</param>
            ''' <param name="InterruptionDuration">The duration (in seconds) of silent sections.</param>
            ''' <InterruptionAttenuation>The attenuation in the interruption sections, or Nothing to completely silence the sound in the interruption sections.</InterruptionAttenuation>
            ''' <param name="SoundSectionDuration">The duration (in seconds) of the sound sections.</param>
            ''' <param name="FadeInProportion">The proportion of the sound section which is faded in.</param>
            ''' <param name="FadeOutProportion">The proportion of the sound section which is faded out.</param>
            ''' <returns></returns>
            Public Function InterruptSound(ByVal InputSound As Sound,
                                       ByVal InterruptionDuration As Double,
                                       Optional ByVal InterruptionAttenuation As Double? = Nothing,
                                       Optional ByVal SoundSectionDuration As Double? = Nothing,
                                       Optional ByVal FadeInProportion As Double = 0,
                                       Optional ByVal FadeOutProportion As Double = 0,
                                       Optional ByVal FadeSlopeType As FadeSlopeType = FadeSlopeType.Smooth,
                                       Optional ByVal FadeCosinePower As Double = 10,
                                       Optional ByVal FadeEqualPower As Boolean = False) As Sound

                Try

                    Dim OutputSound As Sound = InputSound.CreateCopy

                    Dim SoundSectionLength As Integer = SoundSectionDuration * InputSound.WaveFormat.SampleRate
                    Dim InterruptionLength As Integer = InterruptionDuration * InputSound.WaveFormat.SampleRate

                    Dim FadeInLength As Integer = Math.Max(0, Int(SoundSectionLength * FadeInProportion))
                    Dim FadeOutLength As Integer = Math.Max(0, Int(SoundSectionLength * FadeOutProportion))

                    'Limiting FadeInLength
                    If FadeInLength > SoundSectionLength Then
                        FadeInLength = SoundSectionLength
                    End If

                    'Limiting FadeOutlength
                    If FadeInLength + FadeOutLength > SoundSectionLength Then
                        FadeOutLength = SoundSectionLength - FadeInLength
                    End If

                    'Calculating the length of the unmodified sound section
                    Dim FullSoundLength As Integer = SoundSectionLength - (FadeInLength + FadeOutLength)

                    Dim StartSample As Integer = 0
                    Dim EndSample As Integer = 0
                    Dim SilentSection As Boolean = False
                    Do

                        If SilentSection = False Then

                            'Doing fade in, keep sound, and fade out

                            'Exiting the loop if the end of the sound file is reached
                            If StartSample >= OutputSound.WaveData.ShortestChannelSampleCount - 1 Then Exit Do

                            'Fading in
                            If FadeInLength > 0 Then Fade(OutputSound, InterruptionAttenuation, 0,, StartSample, FadeInLength, FadeSlopeType, FadeCosinePower, FadeEqualPower)

                            'Increasing startsample
                            StartSample += FadeInLength

                            'Increasing startsample by the unmodified sound length
                            StartSample += FullSoundLength

                            'Exiting the loop if the end of the sound file is reached
                            If StartSample >= OutputSound.WaveData.ShortestChannelSampleCount - 1 Then Exit Do

                            'Fading out
                            If FadeOutLength > 0 Then Fade(OutputSound, 0, InterruptionAttenuation,, StartSample, FadeOutLength, FadeSlopeType, FadeCosinePower, FadeEqualPower)

                            'Increasing startsample by the unmodified sound length
                            StartSample += FadeOutLength

                        Else

                            'Silencing section

                            'Exiting the loop if the end of the sound file is reached
                            If StartSample >= OutputSound.WaveData.ShortestChannelSampleCount - 1 Then Exit Do

                            'Using fade to silence the section
                            Fade(OutputSound, InterruptionAttenuation, InterruptionAttenuation,, StartSample, InterruptionLength)

                            'Increasing startsample by the InterruptionLength
                            StartSample += InterruptionLength

                        End If

                        'Swapping value of SilentSection 
                        SilentSection = Not SilentSection

                    Loop

                    Return OutputSound

                Catch ex As Exception
                    MsgBox("The following error occured in InterruptSound:" & vbCrLf & vbCrLf & ex.ToString)
                    Return Nothing
                End Try

            End Function

            Public Sub CreateInterruptedSpeech_Batch(Optional ByVal InputFolder As String = "",
                                       Optional ByVal InterruptionDuration As Double = 0.2,
                                                 Optional ByVal InterruptionAttenuation As Double? = Nothing,
                                       Optional ByVal SoundSectionDuration As Double? = Nothing,
                                       Optional ByVal FadeInProportion As Double = 0,
                                       Optional ByVal FadeOutProportion As Double = 0,
                                       Optional ByVal FadeSlopeType As FadeSlopeType = FadeSlopeType.Smooth,
                                       Optional ByVal FadeCosinePower As Double = 10,
                                       Optional ByVal FadeEqualPower As Boolean = False,
                                                Optional ByVal OutputFolder As String = "",
                                                 Optional ByVal RandomizeSoundOrder As Boolean = False,
                                                 Optional ByVal InterStimulusInterval As Double = 4,
                                                 Optional ByVal FadeSoundEdges As Boolean = True,
                                                 Optional ByVal SoftLimit As Boolean = True,
                                                 Optional ByVal AverageLevel As Double = -23,
                                                 Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.C,
                                                 Optional ByVal CreateCalibrationSignal As Boolean = True,
                                                 Optional ByVal LogInterruptionFunction As Boolean = True)

                Try

                    Dim DistorsionHasOccurred As Boolean = False

                    'Setting input folder
                    If InputFolder = "" Then
                        Dim fbd As New FolderBrowserDialog
                        fbd.Description = "Select the folder containing the input sounds"
                        Dim DialogResult = fbd.ShowDialog()
                        If DialogResult = DialogResult.OK Then
                            InputFolder = fbd.SelectedPath
                        Else
                            MsgBox("No input folder selected!")
                            Exit Sub
                        End If
                    End If

                    'Setting output folder
                    If OutputFolder = "" Then
                        Dim fbd As New FolderBrowserDialog
                        fbd.Description = "Select a location to store the output files"
                        Dim DialogResult = fbd.ShowDialog()
                        If DialogResult = DialogResult.OK Then
                            OutputFolder = fbd.SelectedPath
                        Else
                            MsgBox("No output folder selected!")
                            Exit Sub
                        End If
                    End If

                    'Reading input sounds
                    Dim InputSounds As New Sounds
                    AudioIOs.ReadMultipleWaveFiles(InputSounds,, InputFolder)

                    'Analysing and modifying to a list of tuple
                    Dim SoundsList As New List(Of Tuple(Of Sound, String, String))
                    For Each InputSound In InputSounds

                        'Checking that the sample rate is 48k with filtered level
                        If FrequencyWeighting <> FrequencyWeightings.Z Then
                            If InputSound.WaveFormat.SampleRate <> 48000 Then
                                MsgBox("Only a sample rate of 48 kHz is allowed when using a frequency weighted level. Either convert the input sounds to 48k, or select Z-weighted average level.",, "Frequency weighting problems...")
                                Exit Sub
                            End If
                        End If

                        'Copying only channel 1 data to a new sound
                        Dim NewSound = DSP.CopySection(InputSound,,, 1)

                        'Setting the level to Average level
                        Dim DistortedSamples As Integer = DSP.MeasureAndAdjustSectionLevel(NewSound, AverageLevel,,,, FrequencyWeighting)

                        'Checking if distorsion occurred during amplification
                        If DistortedSamples > 0 Then DistorsionHasOccurred = True

                        'Fading the edges 
                        If FadeSoundEdges = True Then
                            'Fading in
                            DSP.Fade(NewSound, Nothing, 0, 1, 0, 0.01 * NewSound.WaveFormat.SampleRate, FadeSlopeType.Linear)
                            'Fading out
                            DSP.Fade(NewSound, 0, Nothing, 1, NewSound.WaveData.SampleData(1).Length - 0.01 * NewSound.WaveFormat.SampleRate, 0.01 * NewSound.WaveFormat.SampleRate, FadeSlopeType.Linear)
                        End If

                        'Measuring peak level before limiting
                        Dim PreLimitPeakLevel As String = MeasureSectionLevel(NewSound, 1,,,, SoundMeasurementType.AbsolutePeakAmplitude,
                                                            FrequencyWeightings.Z)

                        'Checking the peak evel
                        If PreLimitPeakLevel > 0 Then DistorsionHasOccurred = True

                        Dim PostLimitPeakLevel As String = ""
                        If SoftLimit = True Then
                            'Soft limiting to -3 dB FS
                            DSP.SoftLimitSection(NewSound, -3,,, 0.01, 1, FrequencyWeightings.Z, True, False)

                            'Measuring peak level after limiting
                            PostLimitPeakLevel = MeasureSectionLevel(NewSound, 1,,,, SoundMeasurementType.AbsolutePeakAmplitude,
                                                            FrequencyWeightings.Z)
                        End If

                        'Storing the new sound and log data
                        SoundsList.Add(New Tuple(Of Sound, String, String)(NewSound, InputSound.FileName & vbTab & PreLimitPeakLevel & vbTab & PostLimitPeakLevel, ""))
                    Next

                    'Randomizes order
                    If RandomizeSoundOrder = True Then

                        Dim TempList As New List(Of Tuple(Of Sound, String, String))
                        Dim rnd As New Random

                        Do Until SoundsList.Count = 0
                            Dim RandomIndex As Integer = rnd.Next(0, SoundsList.Count)
                            TempList.Add(SoundsList(RandomIndex))
                            SoundsList.RemoveAt(RandomIndex)
                        Loop
                        SoundsList = TempList

                    End If

                    'Creating two concatenated sounds with silent intervals, one interrupted and one normal
                    Dim AllSections_InterruptedList As New List(Of Sound)
                    Dim AllSections_NormalList As New List(Of Sound)

                    'Declares a variable that keeps track of the start position of each new sound
                    Dim CurrentStartPosition As Long = 0

                    'Adding initial silence
                    AllSections_InterruptedList.Add(GenerateSound.CreateSilence(SoundsList(0).Item1.WaveFormat, 1, InterStimulusInterval))
                    AllSections_NormalList.Add(GenerateSound.CreateSilence(SoundsList(0).Item1.WaveFormat, 1, InterStimulusInterval))

                    'Increasing CurrentStartPosition 
                    CurrentStartPosition += AllSections_InterruptedList(AllSections_InterruptedList.Count - 1).WaveData.SampleData(1).Length

                    For n = 0 To SoundsList.Count - 1

                        'Creating and adding the interrupted sound
                        Dim InterruptedSection = InterruptSound(SoundsList(n).Item1.CreateCopy, InterruptionDuration, InterruptionAttenuation,
                                                        SoundSectionDuration, FadeInProportion, FadeOutProportion,
                                                        FadeSlopeType, FadeCosinePower, FadeEqualPower)
                        AllSections_InterruptedList.Add(InterruptedSection)

                        'Adding the normal sound
                        AllSections_NormalList.Add(SoundsList(n).Item1)

                        'Storing the current start position and sound length in the log data (we need to create a new tuple as the existing is ReadOnly)
                        SoundsList(n) = New Tuple(Of Sound, String, String)(SoundsList(n).Item1, SoundsList(n).Item2,
                                                                    CurrentStartPosition / SoundsList(n).Item1.WaveFormat.SampleRate & vbTab &
                                                                    SoundsList(n).Item1.WaveData.SampleData(1).Length / SoundsList(n).Item1.WaveFormat.SampleRate)

                        'Increasing CurrentStartPosition 
                        CurrentStartPosition += AllSections_InterruptedList(AllSections_InterruptedList.Count - 1).WaveData.SampleData(1).Length

                        'Adding silence
                        AllSections_InterruptedList.Add(GenerateSound.CreateSilence(SoundsList(0).Item1.WaveFormat, 1, InterStimulusInterval))
                        AllSections_NormalList.Add(GenerateSound.CreateSilence(SoundsList(0).Item1.WaveFormat, 1, InterStimulusInterval))

                        'Increasing CurrentStartPosition 
                        CurrentStartPosition += AllSections_InterruptedList(AllSections_InterruptedList.Count - 1).WaveData.SampleData(1).Length

                    Next

                    'Concatenating the sound
                    Dim ConcatenatedSound_InterruptedSound As Sound = ConcatenateSounds(AllSections_InterruptedList)
                    Dim ConcatenatedSound_NormalSound As Sound = ConcatenateSounds(AllSections_NormalList)


                    'Creats a stereo sound
                    Dim OutputSound As New Sound(New Formats.WaveFormat(ConcatenatedSound_InterruptedSound.WaveFormat.SampleRate,
                                                                ConcatenatedSound_InterruptedSound.WaveFormat.BitDepth, 2,,
                                                                ConcatenatedSound_InterruptedSound.WaveFormat.Encoding))

                    'References the normal sound in channel 1 and the interrupted sound in channel 2 and the 
                    OutputSound.WaveData.SampleData(1) = ConcatenatedSound_NormalSound.WaveData.SampleData(1)
                    OutputSound.WaveData.SampleData(2) = ConcatenatedSound_InterruptedSound.WaveData.SampleData(1)

                    'Exporting the sound
                    AudioIOs.SaveToWaveFile(OutputSound, Path.Combine(OutputFolder, "InterruptedSound"))

                    'Exporting log
                    Dim LogList As New List(Of String)
                    LogList.Add("SoundFile" & vbTab & "PreLimitPeakLevel (dbFS)" & vbTab & "PostLimitPeakLevel (dbFS)" & vbTab & "StartTime (s)" & vbTab & "Duration (s)")
                    For Each Sound In SoundsList
                        LogList.Add(Sound.Item2 & vbTab & Sound.Item3)
                    Next
                    SendInfoToAudioLog(String.Join(vbCrLf, LogList), Path.Combine(OutputFolder, "Log"))

                    If CreateCalibrationSignal = True Then

                        'Creating and exporting a calibration file
                        Dim CalibrationSignal = GenerateSound.CreateFrequencyModulatedSineWave(ConcatenatedSound_InterruptedSound.WaveFormat, 1, 1000, 0.5, 20, 0.125,, 15)

                        'Setting the average level of the calibration signal
                        DSP.MeasureAndAdjustSectionLevel(CalibrationSignal, AverageLevel)

                        'Fading in and out the calibration signal
                        DSP.Fade(CalibrationSignal, Nothing, 0, 1, 0, 0.05 * CalibrationSignal.WaveFormat.SampleRate, DSP.FadeSlopeType.Linear)
                        DSP.Fade(CalibrationSignal, 0, Nothing, 1, CalibrationSignal.WaveData.SampleData(1).Length - 1 - 0.05 * CalibrationSignal.WaveFormat.SampleRate, Nothing, DSP.FadeSlopeType.Linear)

                        'Copying the signal to a two channel sound, containing the same signal in both channels
                        Dim CalibrationSound As Sound = New Sound(OutputSound.WaveFormat)
                        CalibrationSound.WaveData.SampleData(1) = CalibrationSignal.WaveData.SampleData(1)
                        CalibrationSound.WaveData.SampleData(2) = CalibrationSignal.WaveData.SampleData(1)

                        'Exporting the calibration sound
                        AudioIOs.SaveToWaveFile(CalibrationSound, Path.Combine(OutputFolder, "CalibrationSignal_" & AverageLevel & "_dBFS"))

                    End If

                    If DistorsionHasOccurred = True Then

                        MsgBox("Some degree of distorsion is probably present in the output sound file!" & vbCrLf &
                           "You may be able to use the file anyway, but you should inspect it closely for undesired distorsion." & vbCrLf &
                           "However, you are recommended to decrease the average output level and try again.",, "Risk for distorsion!")
                    End If

                    If LogInterruptionFunction = True Then

                        'Creating a 1 second sound with all samples at value positive FS
                        Dim InterruptionFunctionSound = GenerateSound.CreateSilence(ConcatenatedSound_InterruptedSound.WaveFormat, 1, 1)

                        For s = 0 To InterruptionFunctionSound.WaveData.SampleData(1).Length - 1
                            InterruptionFunctionSound.WaveData.SampleData(1)(s) = InterruptionFunctionSound.WaveFormat.PositiveFullScale
                        Next

                        'Interrupting the sound, using the same settings as above
                        InterruptionFunctionSound = InterruptSound(InterruptionFunctionSound, InterruptionDuration, InterruptionAttenuation,
                                                        SoundSectionDuration, FadeInProportion, FadeOutProportion,
                                                        FadeSlopeType, FadeCosinePower, FadeEqualPower)

                        Dim InterruptionFuctionLogList As New List(Of String)
                        InterruptionFuctionLogList.Add("Time" & vbTab & "Factor")
                        For n = 0 To InterruptionFunctionSound.WaveData.SampleData(1).Length - 1
                            InterruptionFuctionLogList.Add(n / InterruptionFunctionSound.WaveFormat.SampleRate & vbTab & InterruptionFunctionSound.WaveData.SampleData(1)(n))
                        Next

                        'Exporting the interruption function data
                        SendInfoToAudioLog(String.Join(vbCrLf, InterruptionFuctionLogList), Path.Combine(OutputFolder, "InterruptionFunction"))

                    End If

                    'Displaying a finished message
                    Dim OpenFolderResult = MsgBox("Creation of interrupted sound files completed!" & vbCrLf & vbCrLf &
               "Do you want to view the output files in Windows Explorer?", MsgBoxStyle.YesNo, "Finished!")
                    If OpenFolderResult = MsgBoxResult.Yes Then
                        Process.Start("explorer.exe", OutputFolder)
                    End If

                Catch ex As Exception
                    MsgBox("The following error has occured:" & vbCrLf & vbCrLf & ex.ToString)
                End Try

            End Sub

            ''' <summary>
            ''' Creates word lists from sound files stored in InputFolder.
            ''' </summary>
            ''' <param name="InputFolder"></param>
            ''' <param name="OutputFolder"></param>
            ''' <param name="ItemCount">The (maximum) number of words in each list.</param>
            ''' <param name="RandomizeSoundOrder">Set to true to randomise the order of items/words</param>
            ''' <param name="RandomizeSpeechLevels">Set to true to randomise the level deviations between set of word lists.</param>
            ''' <param name="InterStimulusInterval">The duration of silence to be inserted between each item/word.</param>
            ''' <param name="FadeSoundEdges">Set to True, to avoid impulses at the beginning and end of each particular item/word.</param>
            ''' <param name="SoftLimit">Set to true, to soft limit the output sound to -3 dB FS</param>
            ''' <param name="AverageLevel">The average level of a level deviation of zero.</param>
            ''' <param name="LevelDeviations">Creates a new set of word lists for each value, with the its average level set to the particular value.</param>
            ''' <param name="FrequencyWeighting">The frequency weighting used for sound level measurements</param>
            ''' <param name="CreateCalibrationSignal">Set to true to create a standard speech audiometry calibration signal</param>
            Public Sub CreateWordLists_Batch(Optional ByVal InputFolder As String = "",
                                                   Optional ByVal OutputFolder As String = "",
                                                 Optional ByVal ItemCount As Integer = 50, ' New, should limit the number of items, and create subsequenct lists
                                                 Optional ByVal RandomizeSoundOrder As Boolean = False,
                                                 Optional ByVal RandomizeSpeechLevels As Boolean = False,
                                                 Optional ByVal InterStimulusInterval As Double = 4,
                                                 Optional ByVal FadeSoundEdges As Boolean = True,
                                                 Optional ByVal SoftLimit As Boolean = True,
                                                 Optional ByVal AverageLevel As Double = -23,
                                                 Optional ByVal LevelDeviations As List(Of Double) = Nothing,
                                                 Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.C,
                                                 Optional ByVal CreateCalibrationSignal As Boolean = True,
                                         Optional ByVal IssueTimeWarning As Boolean = False,
                                         Optional ByVal RandomSeed As Integer? = Nothing,
                                         Optional ByVal RandomizeInterStimulusInterval As Boolean = False,
                                         Optional ByVal LowestInterStimulusInterval As Double = 0,
                                         Optional ByVal HighestInterStimulusInterval As Double = 4)

                Try

                    Dim TempInterStimulusInterval As Double = 0
                    LowestInterStimulusInterval = Math.Max(LowestInterStimulusInterval, 0)
                    HighestInterStimulusInterval = Math.Max(LowestInterStimulusInterval, HighestInterStimulusInterval)

                    Dim rnd As Random
                    If RandomSeed IsNot Nothing Then
                        rnd = New Random(RandomSeed)
                    Else
                        rnd = New Random
                    End If

                    'Sets a default value for LevelDeviations 
                    If LevelDeviations Is Nothing Then
                        LevelDeviations = New List(Of Double) From {0}
                    End If

                    Dim DistorsionHasOccurred As Boolean = False

                    'Setting input folder
                    If InputFolder = "" Then
                        Dim fbd As New FolderBrowserDialog
                        fbd.Description = "Select the folder containing the input sounds"
                        Dim DialogResult = fbd.ShowDialog()
                        If DialogResult = DialogResult.OK Then
                            InputFolder = fbd.SelectedPath
                        Else
                            MsgBox("No input folder selected!")
                            Exit Sub
                        End If
                    End If

                    'Setting output folder
                    If OutputFolder = "" Then
                        Dim fbd As New FolderBrowserDialog
                        fbd.Description = "Select a location to store the output files"
                        Dim DialogResult = fbd.ShowDialog()
                        If DialogResult = DialogResult.OK Then
                            OutputFolder = fbd.SelectedPath
                        Else
                            MsgBox("No output folder selected!")
                            Exit Sub
                        End If
                    End If

                    If IssueTimeWarning = True Then
                        MsgBox("Press OK to start creating the word lists. Depending on the number of items to be processed, it might take quite some time to finish...")
                    End If

                    'Reading input sounds
                    Dim InputSounds As New Sounds
                    AudioIOs.ReadMultipleWaveFiles(InputSounds,, InputFolder)

                    'Analysing and modifying to a list of tuple

                    'Setting up a data structure with room for all output data. Reference As SoundsList(n)(i),n=level, i=word
                    Dim SoundsList As New List(Of List(Of Tuple(Of Sound, String)))
                    For d = 0 To LevelDeviations.Count - 1
                        Dim TempLevelSoundList As New List(Of Tuple(Of Sound, String))
                        For i = 0 To InputSounds.Count - 1
                            TempLevelSoundList.Add(New Tuple(Of Sound, String)(Nothing, ""))
                        Next
                        SoundsList.Add(TempLevelSoundList)
                    Next

                    For i = 0 To InputSounds.Count - 1

                        Dim InputSound = InputSounds(i)

                        'Checking that the sample rate is 48k with filtered level
                        If FrequencyWeighting <> FrequencyWeightings.Z Then
                            If InputSound.WaveFormat.SampleRate <> 48000 Then
                                MsgBox("Only a sample rate of 48 kHz is allowed when using a frequency weighted level. Either convert the input sounds to 48k, or select Z-weighted average level.",, "Frequency weighting problems...")
                                Exit Sub
                            End If
                        End If

                        Dim TempSoundList As New List(Of Tuple(Of Sound, String))

                        For d = 0 To LevelDeviations.Count - 1

                            Dim currentLevelDeviation As Double = LevelDeviations(d)

                            'Copying only channel 1 data to a new sound
                            Dim NewSound = DSP.CopySection(InputSound,,, 1)

                            'Setting the level to Average level
                            Dim DistortedSamples As Integer = DSP.MeasureAndAdjustSectionLevel(NewSound, AverageLevel + currentLevelDeviation,,,, FrequencyWeighting)

                            'Checking if distorsion occurred during amplification
                            If DistortedSamples > 0 Then DistorsionHasOccurred = True

                            'Fading the edges 
                            If FadeSoundEdges = True Then
                                'Fading in
                                DSP.Fade(NewSound, Nothing, 0, 1, 0, 0.01 * NewSound.WaveFormat.SampleRate, FadeSlopeType.Linear)
                                'Fading out
                                DSP.Fade(NewSound, 0, Nothing, 1, NewSound.WaveData.SampleData(1).Length - 0.01 * NewSound.WaveFormat.SampleRate, 0.01 * NewSound.WaveFormat.SampleRate, FadeSlopeType.Linear)
                            End If

                            'Measuring peak level before limiting
                            Dim PreLimitPeakLevel As String = MeasureSectionLevel(NewSound, 1,,,, SoundMeasurementType.AbsolutePeakAmplitude,
                                                                FrequencyWeightings.Z)

                            'Checking the peak evel
                            If PreLimitPeakLevel > 0 Then DistorsionHasOccurred = True

                            Dim PostLimitPeakLevel As String = ""
                            If SoftLimit = True Then
                                'Soft limiting to -3 dB FS
                                DSP.SoftLimitSection(NewSound, -3,,, 0.01, 1, FrequencyWeightings.Z, True, False)

                                'Measuring peak level after limiting
                                PostLimitPeakLevel = MeasureSectionLevel(NewSound, 1,,,, SoundMeasurementType.AbsolutePeakAmplitude,
                                                                FrequencyWeightings.Z)
                            End If

                            'Storing the new sound and log data
                            TempSoundList.Add(New Tuple(Of Sound, String)(NewSound, InputSound.FileName & vbTab &
                                                                      i & vbTab &
                                                                      PreLimitPeakLevel & vbTab &
                                                                      PostLimitPeakLevel & vbTab &
                                                                      AverageLevel + currentLevelDeviation & vbTab &
                                                                      currentLevelDeviation))

                        Next

                        If RandomizeSpeechLevels = True Then

                            'Shuffling the order in TempSoundList
                            Dim TempList As New List(Of Tuple(Of Sound, String))
                            Do Until TempSoundList.Count = 0
                                Dim RandomIndex As Integer = rnd.Next(0, TempSoundList.Count)
                                TempList.Add(TempSoundList(RandomIndex))
                                TempSoundList.RemoveAt(RandomIndex)
                            Loop
                            TempSoundList = TempList

                        End If

                        'Storing the sounds in the right place
                        For d = 0 To LevelDeviations.Count - 1
                            SoundsList(d)(i) = TempSoundList(d)
                        Next

                    Next

                    'Randomizes order of sounds (within each list version)
                    If RandomizeSoundOrder = True Then

                        For d = 0 To LevelDeviations.Count - 1

                            Dim TempList As New List(Of Tuple(Of Sound, String))

                            Do Until SoundsList(d).Count = 0
                                Dim RandomIndex As Integer = rnd.Next(0, SoundsList(d).Count)
                                TempList.Add(SoundsList(d)(RandomIndex))
                                SoundsList(d).RemoveAt(RandomIndex)
                            Loop

                            SoundsList(d) = TempList
                        Next

                    End If


                    For d = 0 To LevelDeviations.Count - 1

                        'Calculating the final number of lists
                        Dim ListCount As Integer = Int(SoundsList(d).Count / ItemCount)
                        If SoundsList(d).Count Mod ItemCount = 0 Then
                            ListCount = SoundsList(d).Count / ItemCount
                        Else
                            ListCount = Int(SoundsList(d).Count / ItemCount) + 1
                        End If

                        ' Creating lists with a length of ItemCount
                        For CurrentListIndex = 0 To ListCount - 1

                            'Creating a concatenated sound with silent intervals, for each list 
                            Dim AllSections_NormalList As New List(Of Sound)

                            'Declares a variable that keeps track of the start position of each new sound
                            Dim CurrentStartPosition As Long = 0

                            'Adding initial silence
                            If RandomizeInterStimulusInterval = False Then
                                TempInterStimulusInterval = InterStimulusInterval
                            Else
                                TempInterStimulusInterval = LowestInterStimulusInterval + rnd.NextDouble() * (HighestInterStimulusInterval - LowestInterStimulusInterval)
                            End If
                            AllSections_NormalList.Add(GenerateSound.CreateSilence(SoundsList(d)(0).Item1.WaveFormat, 1, TempInterStimulusInterval))

                            'Increasing CurrentStartPosition 
                            CurrentStartPosition += AllSections_NormalList(AllSections_NormalList.Count - 1).WaveData.SampleData(1).Length

                            'Adding words
                            For i = CurrentListIndex * ItemCount To Math.Min(CurrentListIndex * ItemCount + ItemCount, SoundsList(d).Count) - 1

                                'Adding the item sound
                                AllSections_NormalList.Add(SoundsList(d)(i).Item1)

                                'Storing the current start position and sound length in the log data (we need to create a new tuple as the existing is ReadOnly)
                                SoundsList(d)(i) = New Tuple(Of Sound, String)(SoundsList(d)(i).Item1,
                                                                           d + 1 & vbTab &
                                                                           CurrentListIndex + 1 & vbTab &
                                                                           SoundsList(d)(i).Item2 & vbTab &
                                                                           CurrentStartPosition / SoundsList(d)(i).Item1.WaveFormat.SampleRate & vbTab &
                                                                           SoundsList(d)(i).Item1.WaveData.SampleData(1).Length / SoundsList(d)(i).Item1.WaveFormat.SampleRate)

                                'Increasing CurrentStartPosition 
                                CurrentStartPosition += AllSections_NormalList(AllSections_NormalList.Count - 1).WaveData.SampleData(1).Length

                                'Adding silence
                                If RandomizeInterStimulusInterval = False Then
                                    TempInterStimulusInterval = InterStimulusInterval
                                Else
                                    TempInterStimulusInterval = LowestInterStimulusInterval + rnd.NextDouble() * (HighestInterStimulusInterval - LowestInterStimulusInterval)
                                End If
                                AllSections_NormalList.Add(GenerateSound.CreateSilence(SoundsList(d)(0).Item1.WaveFormat, 1, TempInterStimulusInterval))

                                'Increasing CurrentStartPosition 
                                CurrentStartPosition += AllSections_NormalList(AllSections_NormalList.Count - 1).WaveData.SampleData(1).Length

                            Next

                            'Concatenating the sound
                            Dim ConcatenatedSound_NormalSound As Sound = ConcatenateSounds(AllSections_NormalList)

                            'Exporting the sound, coded 
                            AudioIOs.SaveToWaveFile(ConcatenatedSound_NormalSound, Path.Combine(OutputFolder, "HomogenizationSounds_Mix_" & d + 1 & "_List_" & CurrentListIndex + 1))

                        Next
                    Next


                    'Exporting log
                    Dim LogList As New List(Of String)
                    LogList.Add("Mix version" & vbTab & "List number" & vbTab & "SoundFile" & vbTab & "Original item order" & vbTab &
                            "PreLimitPeakLevel (dbFS)" & vbTab & "PostLimitPeakLevel (dbFS)" & vbTab &
                            "Level (dBFS)" & vbTab & "Level deviation (dB)" & vbTab & "StartTime (s)" & vbTab & "Duration (s)")
                    For Each ListVersion In SoundsList
                        For Each Sound In ListVersion
                            LogList.Add(Sound.Item2 & vbTab)
                        Next
                    Next
                    SendInfoToAudioLog(String.Join(vbCrLf, LogList), Path.Combine(OutputFolder, "Log"))

                    If CreateCalibrationSignal = True Then

                        'Creating and exporting a calibration file
                        Dim CalibrationSignal = GenerateSound.CreateFrequencyModulatedSineWave(SoundsList(0)(0).Item1.WaveFormat, 1, 1000, 0.5, 20, 0.125,, 15)

                        'Setting the average level of the calibration signal
                        DSP.MeasureAndAdjustSectionLevel(CalibrationSignal, AverageLevel)

                        'Fading in and out the calibration signal
                        DSP.Fade(CalibrationSignal, Nothing, 0, 1, 0, 0.05 * CalibrationSignal.WaveFormat.SampleRate, DSP.FadeSlopeType.Linear)
                        DSP.Fade(CalibrationSignal, 0, Nothing, 1, CalibrationSignal.WaveData.SampleData(1).Length - 1 - 0.05 * CalibrationSignal.WaveFormat.SampleRate, Nothing, DSP.FadeSlopeType.Linear)

                        'Exporting the calibration sound
                        AudioIOs.SaveToWaveFile(CalibrationSignal, Path.Combine(OutputFolder, "CalibrationSignal_" & AverageLevel & "_dBFS"))

                    End If

                    If DistorsionHasOccurred = True Then

                        MsgBox("Some degree of distorsion is probably present in some of the output sound files!" & vbCrLf &
                           "You may be able to use the files anyway, but you should inspect them closely for undesired distorsion." & vbCrLf &
                           "However, you are recommended to decrease the average output level and try again.",, "Risk for distorsion!")
                    End If

                    'Displaying a finished message
                    Dim OpenFolderResult = MsgBox("Creation of homogenization list sound files completed!" & vbCrLf & vbCrLf &
               "Do you want to view the output files in Windows Explorer?", MsgBoxStyle.YesNo, "Finished!")
                    If OpenFolderResult = MsgBoxResult.Yes Then
                        Process.Start("explorer.exe", OutputFolder)
                    End If

                Catch ex As Exception
                    MsgBox("The following error has occured:" & vbCrLf & vbCrLf & ex.ToString)
                End Try

            End Sub


            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="channel"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength"></param>
            ''' <param name="OutputLevel"></param>
            ''' <param name="GatingWindowDuration"></param>
            ''' <param name="GateRelativeThreshold"></param>
            ''' <param name="FractionForCalculatingAbsThreshold"></param>
            ''' <param name="FrequencyWeighting"></param>
            ''' <returns>Returns true if level adjustment could be done without distorsion, and false if level adjustment lead to distorsion (or if something else went wrong).</returns>
            Public Function MeasureAndSetGatedSectionLevel(ByRef InputSound As Sound, Optional ByVal channel As Integer? = Nothing,
                         Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing,
                                                   Optional ByVal OutputLevel As Double = -23,
                                                       Optional ByVal GatingWindowDuration As Decimal = 0.01,
                                                 Optional ByVal GateRelativeThreshold As Double = -10,
                                                 Optional ByVal FractionForCalculatingAbsThreshold As Decimal = 0.25,
                                                 Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z) As Boolean

                Try

                    Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, channel)

                    Dim totalDistortedSamples As Double = 0

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim CorrectedStartSample = startSample
                        Dim CorrectedSectionLength = sectionLength
                        CheckAndCorrectSectionLength(InputSound.WaveData.SampleData(c).Length, CorrectedStartSample, CorrectedSectionLength)

                        'Measuring gated sound level
                        Dim GatedLevel As Double? = MeasureGatedSectionLevel(InputSound, c, CorrectedStartSample, CorrectedSectionLength, SoundDataUnit.dB, GatingWindowDuration,
                                                                GateRelativeThreshold, FractionForCalculatingAbsThreshold, FrequencyWeighting)

                        If GatedLevel Is Nothing Then
                            MsgBox("Error")
                            Return False
                        End If

                        'Adjusting section level
                        Dim Gain As Double = OutputLevel - GatedLevel

                        totalDistortedSamples += AmplifySection(InputSound, Gain, c, CorrectedStartSample, CorrectedSectionLength, SoundDataUnit.dB)

                    Next

                    'Prepares the output
                    If totalDistortedSamples > 0 Then
                        Return False
                    Else
                        Return True
                    End If

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return False
                End Try

            End Function


            ''' <summary>
            ''' Normalizes the average level of the loudest (TemporatIntegrationDuration long) period of the specified section of the input file to the specified output level.
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="channel"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength"></param>
            ''' <param name="OutputLevel">The desired normalised output level.</param>
            ''' <returns>Returns true if level adjustment could be done without distorsion, and false if level adjustment lead to distorsion (or if something else went wrong).</returns>
            Public Function TimeAndFrequencyWeightedNormalization(ByRef InputSound As Sound, Optional ByVal channel As Integer? = Nothing,
                         Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing,
                                                   Optional ByVal OutputLevel As Double = -23) As Boolean

                Try

                    Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, channel)

                    Dim totalDistortedSamples As Double = 0

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim CorrectedStartSample = startSample
                        Dim CorrectedSectionLength = sectionLength
                        CheckAndCorrectSectionLength(InputSound.WaveData.SampleData(c).Length, CorrectedStartSample, CorrectedSectionLength)

                        'Measuring the loudest temporalIntegrationDuration long section
                        Dim MaxLevel As Double? = MeasureTimeAndFrequencyWeightedSectionLevel(InputSound, c, CorrectedStartSample, CorrectedSectionLength,
                                                                         SoundDataUnit.dB) ', TemporalIntegrationDuration, FrequencyWeighting)
                        If MaxLevel Is Nothing Then
                            MsgBox("Error")
                            Return False
                        End If

                        'Adjusting section level
                        Dim Gain As Double = OutputLevel - MaxLevel

                        totalDistortedSamples += AmplifySection(InputSound, Gain, c, CorrectedStartSample, CorrectedSectionLength, SoundDataUnit.dB)

                    Next

                    'Prepares the output
                    If totalDistortedSamples > 0 Then
                        Return False
                    Else
                        Return True
                    End If

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return False
                End Try

            End Function



            'Frequency domain conversions

            ''' <summary>
            ''' Caluculated frequency domain data from the time domain data stored in the specified Sound. The frequency domain data may be stored in the Sound properties
            ''' FFT (which should be done by the calling code).
            ''' </summary>
            ''' <param name="sound">The input sound.</param>
            ''' <param name="fftFormat">The format used to create the frequency domain data. N.B. that overlap may be used, as well as windowing. A shorter analysis window than the input FFT size 
            ''' may be used to increase the frequency resolution without lengthening the analysis window.</param>
            ''' <param name="channel">The channel in the input sound to be analysed. If lenft to default, all channels will be analysed.</param>
            ''' <param name="startSample">This parameter can be used if only a section of the sound file should be analysed. If left empty, the sound will be analysed starting from the first sample.</param>
            ''' <param name="sectionLength">This parameter can be used if only a section of the sound file should be analysed. If left empty, the sound will be analysed from the start sample to the last sample.</param>
            ''' <returns>Returns a new instance of FftData with the frequency domain data stored in the properties FrequencyDomainRealData and FrequencyDomainImaginaryData.</returns>
            Public Function SpectralAnalysis(ByRef sound As Sound, ByRef fftFormat As Formats.FftFormat,
                                         Optional ByVal channel As Integer? = Nothing,
                                         Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing) As FftData

                Try


                    'Allowing different channel lengths during processing to avoid unnecessary redims

                    Dim AudioOutputConstructor As New AudioOutputConstructor(sound.WaveFormat, channel)
                    Dim localFftData As New FftData(sound.WaveFormat, fftFormat)
                    Dim windowDistance As Integer = fftFormat.AnalysisWindowSize - fftFormat.OverlapSize

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim inputArray() As Single = sound.WaveData.SampleData(c)

                        If startSample > 0 Or sectionLength IsNot Nothing Then
                            Dim CorrectedStartSample = startSample
                            Dim CorrectedSectionLength = sectionLength
                            CheckAndCorrectSectionLength(inputArray.Length, CorrectedStartSample, CorrectedSectionLength)
                            inputArray = inputArray.ToList.GetRange(CorrectedStartSample, CorrectedSectionLength).ToArray
                        End If

                        Dim originalInputSoundLength As Integer = inputArray.Length


                        'Extends the input array and determines the number of (overlapping) windows  
                        Dim numberOfWindows As Integer
                        numberOfWindows = Utils.Rounding(inputArray.Length / (windowDistance), Utils.roundingMethods.alwaysUp)
                        Dim EndOfSoundZeroPadding As Integer = ExtendSoundArrayToWindowLengthMultiple(inputArray, fftFormat)

                        For windowNumber = 0 To numberOfWindows - 1
                            Dim localREXArray(fftFormat.FftWindowSize - 1) As Single
                            Dim fftIndex As Integer = 0
                            Dim startReadSample As Integer = windowNumber * (windowDistance)
                            For sample = startReadSample To startReadSample + fftFormat.AnalysisWindowSize - 1
                                localREXArray(fftIndex) = inputArray(sample)
                                fftIndex += 1
                            Next
                            'For sample = fftIndex To localREXArray.Length - 1
                            'localREXArray(fftIndex) = 0 'Perhaps this is not needed ?
                            'Next

                            'Windowing of localDftInputArray comes here
                            WindowingFunction(localREXArray, fftFormat.WindowingType, fftFormat.AnalysisWindowSize)

                            'Preparing for FFT
                            'Creating an imaginary time domain signal consisting of zeros, same length as the real signal
                            Dim localIMXArray(fftFormat.FftWindowSize - 1) As Single

                            'Getting the base 2 log for windowSize
                            Dim nPoints = Utils.getBase_n_Log(fftFormat.FftWindowSize, 2)

                            'Caluculating FFT
                            FFT_Bourke(1, nPoints, localREXArray, localIMXArray)

                            'Storing the DFT data
                            localFftData.FrequencyDomainRealData(c, windowNumber).WindowData = localREXArray
                            localFftData.FrequencyDomainImaginaryData(c, windowNumber).WindowData = localIMXArray

                            'Storing the description
                            localFftData.FrequencyDomainRealData(c, windowNumber).WindowingType = fftFormat.WindowingType
                            localFftData.FrequencyDomainImaginaryData(c, windowNumber).WindowingType = localFftData.FrequencyDomainRealData(c, windowNumber).WindowingType

                            Dim CurrentEndOfSoundZeroPadding As Integer = Math.Max(0, startReadSample + fftFormat.AnalysisWindowSize - originalInputSoundLength) 'TODO Check that this really gets the right amount of zero padding caused by the extension of the sound to an integer multiple of the fft-window length!!!
                            localFftData.FrequencyDomainRealData(c, windowNumber).ZeroPadding = fftFormat.FftWindowSize - fftFormat.AnalysisWindowSize + CurrentEndOfSoundZeroPadding
                            localFftData.FrequencyDomainImaginaryData(c, windowNumber).ZeroPadding = localFftData.FrequencyDomainRealData(c, windowNumber).ZeroPadding

                        Next

                        'Restoring original sound length
                        ReDim Preserve inputArray(originalInputSoundLength - 1)

                    Next

                    Return localFftData

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="InputFftData">The frequency domain data.</param>
            ''' <param name="channel">The channel in the input sound to be analysed. If lenft to default, all channels will be analysed.</param>
            ''' <returns>Returns a new instance of FftData with the frequency domain data stored in the properties FrequencyDomainRealData and FrequencyDomainImaginaryData.</returns>
            Public Function SpectralSynthesis(ByRef InputFftData As FftData,
                                         Optional ByVal channel As Integer? = Nothing,
                                          Optional ByVal DoOutputWindowing As Boolean = False,
                                          Optional SetEachWindowToLevel? As Double = Nothing) As Sound
                'Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing) As Sound
                ' <param name="startSample">This parameter can be used if only a section of the sound file should be analysed. If left empty, the sound will be analysed starting from the first sample.</param>
                ' <param name="sectionLength">This parameter can be used if only a section of the sound file should be analysed. If left empty, the sound will be analysed from the start sample to the last sample.</param>

                Try


                    Dim AudioOutputConstructor As New AudioOutputConstructor(InputFftData.Waveformat, channel)
                    Dim localFftData As New FftData(InputFftData.Waveformat, InputFftData.FftFormat)
                    Dim windowDistance As Integer = InputFftData.FftFormat.AnalysisWindowSize - InputFftData.FftFormat.OverlapSize

                    Dim OutputSound As New Sound(New Formats.WaveFormat(InputFftData.Waveformat.SampleRate, InputFftData.Waveformat.BitDepth, AudioOutputConstructor.LastChannelIndex,, InputFftData.Waveformat.Encoding))

                    Dim TempMeasurementSound As Sound = Nothing
                    'If SetEachWindowToLevel IsNot Nothing Then
                    TempMeasurementSound = New Sound(OutputSound.WaveFormat)
                    'End If

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim StartWriteSample As Integer = 0
                        'Dim OutputChannelSoundArray((InputFftData.fftFormat.AnalysisWindowSize) * InputFftData.windowCount(c) - 1) As Single
                        Dim OutputChannelSoundArray((InputFftData.FftFormat.AnalysisWindowSize - InputFftData.FftFormat.OverlapSize) * InputFftData.WindowCount(c) + InputFftData.FftFormat.OverlapSize - 1) As Single

                        For windowNumber = 0 To InputFftData.WindowCount(c) - 1 'Changed 2018-04-21, (previously -2)

                            'Referencing the rectangual spectral domain data
                            Dim localREXWindow As FftData.TimeWindow = InputFftData.FrequencyDomainRealData(c, windowNumber)
                            Dim localIMXWindow As FftData.TimeWindow = InputFftData.FrequencyDomainImaginaryData(c, windowNumber)


                            'Getting the base 2 log for windowSize
                            Dim nPoints = Utils.getBase_n_Log(InputFftData.FftFormat.FftWindowSize, 2)

                            'Caluculating iFFT
                            FFT_Bourke(-1, nPoints, localREXWindow.WindowData, localIMXWindow.WindowData)

                            'Creating a sound array, which will contain the sound output from the iFFT
                            Dim LocalChannelSoundArray(InputFftData.FftFormat.FftWindowSize - 1) As Single
                            'Copying the output of the fft to the sound array
                            For n = 0 To localREXWindow.WindowData.Length - 1
                                LocalChannelSoundArray(n) = localREXWindow.WindowData(n)
                            Next

                            'Windowing using a triangular function, if it's an exact add-overlap method
                            If DoOutputWindowing = True Then WindowingFunction(LocalChannelSoundArray, WindowingType.Hanning, InputFftData.FftFormat.AnalysisWindowSize)
                            'If DoOutputWindowing = True Then windowingFunction(LocalChannelSoundArray, InputFftData.fftFormat.WindowingType, InputFftData.fftFormat.AnalysisWindowSize - localREXWindow.ZeroPadding)

                            'Putting the sound output from the iFFT into the sound output sound array
                            Dim FftOutputSample As Integer = 0
                            For sample = StartWriteSample To StartWriteSample + InputFftData.FftFormat.AnalysisWindowSize - 1
                                OutputChannelSoundArray(sample) += LocalChannelSoundArray(FftOutputSample)
                                FftOutputSample += 1
                                StartWriteSample += 1
                            Next

                            'Reversing the start write sample by the overlap length
                            StartWriteSample -= InputFftData.FftFormat.OverlapSize

                        Next

                        'Adjusting the level in the current window
                        'If SetEachWindowToLevel IsNot Nothing Then
                        '    TempMeasurementSound.WaveData.SampleData(c) = OutputChannelSoundArray
                        '    MeasureAndAdjustSectionLevel(TempMeasurementSound, SetEachWindowToLevel, c)
                        'Else
                        '    TempMeasurementSound.WaveData.SampleData(c) = OutputChannelSoundArray
                        '    SetEachWindowToLevel = MeasureSectionLevel(TempMeasurementSound, c)
                        'End If

                        'Storing the channel sound array
                        OutputSound.WaveData.SampleData(c) = OutputChannelSoundArray

                    Next

                    Return OutputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function


            ''' <summary>
            ''' Caluculated cepstrum of the time domain data stored in the specified Sound. The frequency domain data may be stored in the Sound properties
            ''' FFT (which should be done by the calling code).
            ''' </summary>
            ''' <param name="sound">The input sound.</param>
            ''' <param name="fftFormat">The format used to create the frequency domain data. N.B. that overlap may be used, as well as windowing. A shorter analysis window than the input FFT size 
            ''' may be used to increase the frequency resolution without lengthening the analysis window.</param>
            ''' <param name="channel">The channel in the input sound to be analysed. If lenft to default, all channels will be analysed.</param>
            ''' <param name="startSample">This parameter can be used if only a section of the sound file should be analysed. If left empty, the sound will be analysed starting from the first sample.</param>
            ''' <param name="sectionLength">This parameter can be used if only a section of the sound file should be analysed. If left empty, the sound will be analysed from the start sample to the last sample.</param>
            ''' <returns></returns>
            Public Function CepstralAnalysis(ByRef sound As Sound, ByRef fftFormat As Formats.FftFormat,
                                         Optional ByVal channel As Integer? = Nothing,
                                         Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing) As Sound

                Try

                    'Runs a spectral analysis if not already done
                    If sound.FFT Is Nothing Then sound.FFT = DSP.SpectralAnalysis(sound, fftFormat, channel,
                                                                                        startSample, sectionLength)

                    'Calculating magnitudes
                    sound.FFT.CalculateAmplitudeSpectrum(False, False, False)
                    sound.FFT.CalculatePhaseSpectrum()

                    'Zeroing the phase
                    For c = 1 To sound.FFT.ChannelCount
                        For w = 0 To sound.FFT.WindowCount(c) - 1
                            For n = 0 To sound.FFT.AmplitudeSpectrum(c, w).WindowData.Length - 1
                                sound.FFT.PhaseSpectrum(c, w).WindowData(n) = 0
                            Next
                        Next
                    Next


                    'Taking the log of the magnitudes
                    Throw New NotImplementedException("Should it be log or log10 below? Check!")
                    For c = 1 To sound.FFT.ChannelCount
                        For w = 0 To sound.FFT.WindowCount(c) - 1
                            For n = 0 To sound.FFT.AmplitudeSpectrum(c, w).WindowData.Length - 1
                                sound.FFT.AmplitudeSpectrum(c, w).WindowData(n) = Math.Log(sound.FFT.AmplitudeSpectrum(c, w).WindowData(n))
                            Next
                        Next
                    Next

                    'Overwriting the original rectangulat data using the logarithmized magnitudes
                    sound.FFT.CalculateRectangualForm()


                    'Performing spectral synthesis from the log rectangular data
                    'Storing the cepstral data in the sound array of an Sound
                    Dim Cepstrum As Sound = DSP.SpectralSynthesis(sound.FFT)

                    'Modifications of the cepstrum goes here

                    'Return Cepstrum


                    'Inverting the process
                    'Calculating the fft of the cepstrum
                    Dim TempFftFomat As New Formats.FftFormat(fftFormat.AnalysisWindowSize, fftFormat.FftWindowSize, 0, WindowingType.Rectangular)
                    Cepstrum.FFT = DSP.SpectralAnalysis(Cepstrum, TempFftFomat, channel, startSample, sectionLength)

                    'Calculating magnitudes
                    Cepstrum.FFT.CalculateAmplitudeSpectrum(False, False, False)
                    Cepstrum.FFT.CalculatePhaseSpectrum()

                    'Zeroing the phase
                    For c = 1 To Cepstrum.FFT.ChannelCount
                        For w = 0 To Cepstrum.FFT.WindowCount(c) - 1
                            For n = 0 To Cepstrum.FFT.AmplitudeSpectrum(c, w).WindowData.Length - 1
                                Cepstrum.FFT.PhaseSpectrum(c, w).WindowData(n) = 0
                            Next
                        Next
                    Next

                    'Taking the antilog of the magnitudes
                    For c = 1 To Cepstrum.FFT.ChannelCount
                        For w = 0 To Cepstrum.FFT.WindowCount(c) - 1
                            For n = 0 To Cepstrum.FFT.AmplitudeSpectrum(c, w).WindowData.Length - 1
                                Cepstrum.FFT.AmplitudeSpectrum(c, w).WindowData(n) = Math.E ^ Cepstrum.FFT.AmplitudeSpectrum(c, w).WindowData(n)
                            Next
                        Next
                    Next

                    'Overwriting the cepstral rectangular data using the antilog magnitudes
                    Cepstrum.FFT.CalculateRectangualForm()

                    'Performing spectral synthesis from the antilog rectangular data
                    'Storing the reversed cepstral data in a new Sound
                    Dim OutputSound As Sound = DSP.SpectralSynthesis(Cepstrum.FFT)

                    Return OutputSound


                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function



            Public Function FFT_Bourke(ByVal dir As Integer, ByVal m As Long, ByRef x() As Single, ByRef y() As Single,
                                   Optional ScaleForwardTransform As Boolean = True)

                'Source:
                'Title of the source chapter:
                'DFT
                '(Discrete Fourier Transform)
                'FFT
                '(Fast Fourier Transform)
                'Written by Paul Bourke
                'June 1993

                'http://paulbourke.net/miscellaneous/dft/  (Aquired 2015-08-14)

                'References cited by Bourke:
                'Fast Fourier Transforms
                'Walker, j.S.
                'CRC Press. 1996 

                'Fast Fourier Transforms: Algorithms
                'Elliot, D.F.and Rao, k.R.
                'Academic Press, New York, 1982 

                'Fast Fourier Transforms And Convolution Algorithms
                'Nussbaumer, H.J.
                'Springer, New York, 1982 

                'Digital Signal Processing
                'Oppenheimer, A.V.and Shaffer, R.W.
                'Prentice - Hall, Englewood Cliffs, NJ, 1975 

                'Appendix B. FFT (Fast Fourier Transform), contains the code in C. The following is a translation to VB.NET by Erik Witte (August 2015)

                '
                '   This computes an in-place complex-to-complex FFT 
                '   x and y are the real and imaginary arrays of 2^m points.
                '   dir =  1 gives forward transform
                '   dir = -1 gives reverse transform 

                Dim n, i, i1, j, k, i2, l, l1, l2 As Long
                Dim c1, c2, tx, ty, t1, t2, u1, u2, z As Single

                '/* Calculate the number of points */
                n = 1
                For i = 0 To m - 1
                    n *= 2
                Next i


                '/* Do the bit reversal */
                i2 = n >> 1
                j = 0
                For i = 0 To n - 2
                    If (i < j) Then
                        tx = x(i)
                        ty = y(i)
                        x(i) = x(j)
                        y(i) = y(j)
                        x(j) = tx
                        y(j) = ty
                    End If
                    k = i2

                    While k <= j
                        j -= k
                        k >>= 1
                    End While
                    j += k
                Next i

                '/* Compute the FFT */
                c1 = -1.0
                c2 = 0.0
                l2 = 1
                For l = 0 To m - 1
                    l1 = l2
                    l2 <<= 1
                    u1 = 1.0
                    u2 = 0.0
                    For j = 0 To l1 - 1
                        For i = j To n - 1 Step l2
                            i1 = i + l1
                            t1 = u1 * x(i1) - u2 * y(i1)
                            t2 = u1 * y(i1) + u2 * x(i1)
                            x(i1) = x(i) - t1
                            y(i1) = y(i) - t2
                            x(i) += t1
                            y(i) += t2
                        Next i
                        z = u1 * c1 - u2 * c2
                        u2 = u1 * c2 + u2 * c1
                        u1 = z
                    Next j
                    c2 = Math.Sqrt((1.0 - c1) / 2.0)
                    If dir = 1 Then c2 = -c2
                    c1 = Math.Sqrt((1.0 + c1) / 2.0)
                Next l

                '/* Scaling for forward transform */
                If dir = 1 And ScaleForwardTransform = True Then
                    For i = 0 To n - 1
                        x(i) /= n
                        y(i) /= n
                    Next i
                End If

                Return True
            End Function


            Public Function FFT_Bourke(ByVal dir As Integer, ByVal m As Long, ByRef x() As Double, ByRef y() As Double,
                                   Optional ScaleForwardTransform As Boolean = True)

                'Source:
                'Title of the source chapter:
                'DFT
                '(Discrete Fourier Transform)
                'FFT
                '(Fast Fourier Transform)
                'Written by Paul Bourke
                'June 1993

                'http://paulbourke.net/miscellaneous/dft/  (Aquired 2015-08-14)

                'References cited by Bourke:
                'Fast Fourier Transforms
                'Walker, j.S.
                'CRC Press. 1996 

                'Fast Fourier Transforms: Algorithms
                'Elliot, D.F.and Rao, k.R.
                'Academic Press, New York, 1982 

                'Fast Fourier Transforms And Convolution Algorithms
                'Nussbaumer, H.J.
                'Springer, New York, 1982 

                'Digital Signal Processing
                'Oppenheimer, A.V.and Shaffer, R.W.
                'Prentice - Hall, Englewood Cliffs, NJ, 1975 

                'Appendix B. FFT (Fast Fourier Transform), contains the code in C. The following is a translation to VB.NET by Erik Witte (August 2015)

                '
                '   This computes an in-place complex-to-complex FFT 
                '   x and y are the real and imaginary arrays of 2^m points.
                '   dir =  1 gives forward transform
                '   dir = -1 gives reverse transform 

                Dim n, i, i1, j, k, i2, l, l1, l2 As Long
                Dim c1, c2, tx, ty, t1, t2, u1, u2, z As Double

                '/* Calculate the number of points */
                n = 1
                For i = 0 To m - 1
                    n *= 2
                Next i


                '/* Do the bit reversal */
                i2 = n >> 1
                j = 0
                For i = 0 To n - 2
                    If (i < j) Then
                        tx = x(i)
                        ty = y(i)
                        x(i) = x(j)
                        y(i) = y(j)
                        x(j) = tx
                        y(j) = ty
                    End If
                    k = i2

                    While k <= j
                        j -= k
                        k >>= 1
                    End While
                    j += k
                Next i

                '/* Compute the FFT */
                c1 = -1.0
                c2 = 0.0
                l2 = 1
                For l = 0 To m - 1
                    l1 = l2
                    l2 <<= 1
                    u1 = 1.0
                    u2 = 0.0
                    For j = 0 To l1 - 1
                        For i = j To n - 1 Step l2
                            i1 = i + l1
                            t1 = u1 * x(i1) - u2 * y(i1)
                            t2 = u1 * y(i1) + u2 * x(i1)
                            x(i1) = x(i) - t1
                            y(i1) = y(i) - t2
                            x(i) += t1
                            y(i) += t2
                        Next i
                        z = u1 * c1 - u2 * c2
                        u2 = u1 * c2 + u2 * c1
                        u1 = z
                    Next j
                    c2 = Math.Sqrt((1.0 - c1) / 2.0)
                    If dir = 1 Then c2 = -c2
                    c1 = Math.Sqrt((1.0 + c1) / 2.0)
                Next l

                '/* Scaling for forward transform */
                If dir = 1 And ScaleForwardTransform = True Then
                    For i = 0 To n - 1
                        x(i) /= n
                        y(i) /= n
                    Next i
                End If

                Return True
            End Function


            ''' <summary>
            ''' Setting the average spectral content of the input sound to the average spectral content of the TargetSound
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="TargetSound"></param>
            ''' <returns></returns>
            Public Function SetSpectralContent(ByRef InputSound As Sound, ByRef TargetSound As Sound, Optional ByRef FftFormat As Formats.FftFormat = Nothing) As Sound

                Throw New NotImplementedException("Experimental function!")

                'Creating a default FFT format
                'If FftFormat Is Nothing Then FftFormat = New FftFormat(512,, 256,  WindowingType.Hanning)
                'If FftFormat Is Nothing Then FftFormat = New FftFormat(2048,, 2048 - 8,  WindowingType.Tukey,, 0.5)
                If FftFormat Is Nothing Then FftFormat = New Formats.FftFormat(512, 2048, 0, WindowingType.Rectangular)

                'Analysing TargetSound spektrum (if not already done)
                If TargetSound.FFT Is Nothing Then

                    TargetSound.FFT = SpectralAnalysis(TargetSound, FftFormat)
                    TargetSound.FFT.CalculateAmplitudeSpectrum(False, False, False)

                    'Calculate Average power spectrum of all timw windows and store the result in the first time window (of each channel)
                    For c = 1 To TargetSound.WaveFormat.Channels

                        'Summing coefficient values
                        Dim AverageTargetSoundSpectrum(TargetSound.FFT.AmplitudeSpectrum(c, 0).WindowData.Length - 1) As Double
                        Dim CurrentWindowCount As Integer = TargetSound.FFT.WindowCount(c)
                        For t = 0 To CurrentWindowCount - 1
                            For k = 0 To AverageTargetSoundSpectrum.Length - 1
                                AverageTargetSoundSpectrum(k) += TargetSound.FFT.AmplitudeSpectrum(c, t).WindowData(k)
                            Next
                        Next

                        'Dividing by the number of time windows to get the average spectrum
                        For k = 0 To AverageTargetSoundSpectrum.Length - 1
                            AverageTargetSoundSpectrum(k) /= CurrentWindowCount
                        Next

                        'Storing it in the first time window of the original sound so that it may be re-used from there
                        For k = 0 To AverageTargetSoundSpectrum.Length - 1
                            TargetSound.FFT.AmplitudeSpectrum(c, 0).WindowData(k) = AverageTargetSoundSpectrum(k)
                        Next
                    Next
                End If

                'Exiting by returning Nothing if InputSound is Nothing (Then only the noise is set up for use in later analyses)
                If InputSound Is Nothing Then Return Nothing

                'Referencing the target spectrum (using only the spectrum of channel 1)
                Dim TargetSpectrum() As Single = TargetSound.FFT.AmplitudeSpectrum(1, 0).WindowData

                'Getting the spectrum of the signal
                InputSound.FFT = SpectralAnalysis(InputSound, FftFormat)
                InputSound.FFT.CalculateAmplitudeSpectrum(False, False, False)
                InputSound.FFT.CalculatePhaseSpectrum()

                'Utils.SendInfoToLog(String.Join(vbCrLf, InputSound.FFT.PhaseSpectrum(1, 4).WindowData), "Phase", "D:/")

                ''Amplifying each coefficient so that the target spectrum is reached (unless specral power is 0) 
                'Putting the target spectrum in each window
                For w = 0 To InputSound.FFT.WindowCount(1) - 1
                    'For k = 0 To TargetSpectrum.Length - 1

                    For k = 0 To InputSound.FFT.AmplitudeSpectrum(1, w).WindowData.Length - 1
                        InputSound.FFT.AmplitudeSpectrum(1, w).WindowData(k) = TargetSpectrum(k)
                    Next

                    ''Positive half
                    'For k = 0 To Int(InputSound.FFT.AmplitudeSpectrum(1, w).WindowData.Length * (0.41)) - 1 'Stopping at 0.83 times the fft length
                    '    InputSound.FFT.AmplitudeSpectrum(1, w).WindowData(k) = TargetSpectrum(k)
                    'Next

                    ''Negative half
                    'For k = InputSound.FFT.AmplitudeSpectrum(1, w).WindowData.Length - Int(InputSound.FFT.AmplitudeSpectrum(1, w).WindowData.Length * (0.41)) To InputSound.FFT.AmplitudeSpectrum(1, w).WindowData.Length - 1 'Stopping at 0.83 times the fft length
                    '    InputSound.FFT.AmplitudeSpectrum(1, w).WindowData(k) = TargetSpectrum(k)
                    'Next

                Next

                'Setting phases to 0
                Dim rnd As New Random
                For w = 0 To InputSound.FFT.WindowCount(1) - 1
                    For k = 0 To TargetSpectrum.Length - 1
                        InputSound.FFT.PhaseSpectrum(1, w).WindowData(k) = 0
                        'InputSound.FFT.PhaseSpectrum(1, w).WindowData(k) = 2 * (rnd.NextDouble() - 0.5) * Math.PI
                    Next
                Next


                'Doing spectral synthesis using the modified data
                'Converting power to magnitudes

                Dim SetWindowLevel As Double? = Nothing
                InputSound.FFT.CalculateRectangualForm()
                Dim OutputSound = DSP.SpectralSynthesis(InputSound.FFT, 1, True, SetWindowLevel)

                Return OutputSound

                'GenerateSound.GetImpulseResponseFromFile(InputSound, FftFormat, 2000)



            End Function

            Public Enum ProcessingDomain
                TimeDomain
                FrequencyDomain
            End Enum

            Public Function FIRFilter(ByVal inputSound As Sound, ByVal impulseResponse As Sound,
                                  ByRef fftFormat As Formats.FftFormat, Optional ByVal inputSoundChannel As Integer? = Nothing,
                                  Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing,
                                  Optional ByVal filteringDomain As ProcessingDomain = ProcessingDomain.FrequencyDomain,
                                  Optional ByVal ScaleImpulseResponse As Boolean = False, Optional InActivateWarnings As Boolean = False,
                                  Optional ByVal KeepInputSoundLength As Boolean = False) As Sound

                Try

                    Dim IRChannel As Integer = 1

                    Dim outputSound As New Sound(inputSound.WaveFormat)
                    Dim AudioOutputConstructor As New AudioOutputConstructor(inputSound.WaveFormat, inputSoundChannel)
                    outputSound.FFT = New FftData(inputSound.WaveFormat, fftFormat)
                    Dim FS As Double = inputSound.WaveFormat.PositiveFullScale

                    'Main section
                    Select Case inputSound.WaveFormat.BitDepth
                        Case 16, 32
                            For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                                'Copies the impulse response to a new array of double
                                Dim tempArray() As Single = impulseResponse.WaveData.SampleData(IRChannel)
                                Dim IRArray(tempArray.Length - 1) As Double
                                For n = 0 To IRArray.Length - 1
                                    IRArray(n) = tempArray(n)
                                Next

                                'Scaling impulse response. 
                                If ScaleImpulseResponse = True Then

                                    Dim ImpulseResponseArraySum As Double = IRArray.Sum

                                    If ImpulseResponseArraySum <> 0 Then
                                        For n = 0 To IRArray.Length - 1
                                            IRArray(n) = IRArray(n) / ImpulseResponseArraySum
                                        Next
                                    Else
                                        MsgBox("The impulse response sums to 0!")
                                    End If
                                End If

                                'Referencing the current channel array, and noting its original length
                                Dim tempInputSoundArray() As Single = inputSound.WaveData.SampleData(c)
                                Dim originalInputLength As Integer = tempInputSoundArray.Length

                                Select Case filteringDomain
                                    Case ProcessingDomain.TimeDomain
                                        'Time domain convolution filtering

                                        'Copies the input array to a new array
                                        Dim inputArray(tempInputSoundArray.Length + IRArray.Length - 1) As Single
                                        For n = 0 To tempInputSoundArray.Length - 1
                                            inputArray(n) = tempInputSoundArray(n)
                                        Next
                                        For n = tempInputSoundArray.Length To inputArray.Length - 1
                                            inputArray(n) = 0
                                        Next

                                        'Starting a progress window
                                        Dim myProgress As New ProgressDisplay
                                        myProgress.Initialize(inputArray.Length - 1, 0, "Calculating FIR filter...", IRArray.Length)
                                        myProgress.Show()
                                        Dim ProgressCounter As Integer = 0

                                        ReDim outputSound.WaveData.SampleData(c)(inputArray.Length - 1)

                                        For n = 0 To inputArray.Length - 1
                                            Dim cumulativeValue As Double = 0

                                            'Updating progress
                                            myProgress.UpdateProgress(ProgressCounter)
                                            ProgressCounter += 1

                                            'Doing the convolution
                                            For i = 0 To IRArray.Length - 1
                                                If n - i >= 0 Then
                                                    cumulativeValue += inputArray(n - i) * IRArray(i)
                                                End If
                                            Next
                                            outputSound.WaveData.SampleData(c)(n) = cumulativeValue

                                        Next

                                        'Closing the progress display
                                        myProgress.Close()


                                    Case ProcessingDomain.FrequencyDomain
                                        'Frequency domain convolution (complex multiplication)

                                        'Reference: This frequency domain calculation is based on the overlap-add method as
                                        'described in Bateman, A. & Paterson-Stephens, I. (2002). The DSP Handbook. Algorithms, Applications and Design Techniques.
                                        'chapter 7, pp 451-453.

                                        Dim L As Integer = IRArray.Length

                                        CheckAndAdjustFFTSize(fftFormat.FftWindowSize, (L * 2) + 1, InActivateWarnings)

                                        Dim sliceLength As Integer = fftFormat.FftWindowSize / 2

                                        'Copies the input array to a new array, if needed, also extends the input array to a whole number multiple of the length of the sound data that goes into each dft
                                        Dim intendedOutputLength As Integer = tempInputSoundArray.Length + IRArray.Length
                                        Dim numberOfWindows As Integer = Int(tempInputSoundArray.Length / sliceLength)
                                        If tempInputSoundArray.Length Mod sliceLength > 0 Then numberOfWindows += 1
                                        Dim workingInputLength As Integer = sliceLength * (numberOfWindows + 1) ' This should only be (dftInputSampleCount * numberOfWindows), however that doesn't work for some reason. Check later! For now it's solved by making the analysed array temporarily longer.

                                        Dim inputArray(workingInputLength - 1) As Single
                                        For n = 0 To tempInputSoundArray.Length - 1
                                            inputArray(n) = tempInputSoundArray(n)
                                        Next
                                        For n = tempInputSoundArray.Length To inputArray.Length - 1
                                            inputArray(n) = 0
                                        Next

                                        Dim OutputChannelSampleArray(inputArray.Length - 1) As Single

                                        'Creates dft bins
                                        Dim dftSoundBin_x(fftFormat.FftWindowSize - 1) As Double
                                        Dim dftSoundBin_y(fftFormat.FftWindowSize - 1) As Double
                                        Dim dftIR_Bin_x(fftFormat.FftWindowSize - 1) As Double
                                        Dim dftIR_Bin_y(fftFormat.FftWindowSize - 1) As Double

                                        'Creates the zero-padded IR array
                                        For sample = 0 To L - 1
                                            dftIR_Bin_x(sample) = IRArray(sample)
                                            dftIR_Bin_y(sample) = 0
                                        Next
                                        For sample = L To fftFormat.FftWindowSize - 1
                                            dftIR_Bin_x(sample) = 0
                                            dftIR_Bin_y(sample) = 0
                                        Next

                                        'Calculates forward FFT for the IR (Skipping the forward transform scaling)
                                        FFT_Bourke(1, Utils.getBase_n_Log(fftFormat.FftWindowSize, 2), dftIR_Bin_x, dftIR_Bin_y, False)

                                        'Starts convolution one window at a time
                                        Dim readSample As Integer = 0
                                        Dim writeSample As Integer = 0

                                        For windowNumber = 0 To numberOfWindows - 1 'Step 2

                                            'Creates a zero-padded sound array with the length of the dft windows size ()
                                            For sample = 0 To sliceLength - 1
                                                dftSoundBin_x(sample) = inputArray(readSample)
                                                readSample += 1
                                                dftSoundBin_y(sample) = 0
                                            Next
                                            For sample = sliceLength To fftFormat.FftWindowSize - 1
                                                dftSoundBin_x(sample) = 0
                                                dftSoundBin_y(sample) = 0
                                            Next

                                            'Calculates forward FFT for the current sound window
                                            FFT_Bourke(1, Utils.getBase_n_Log(fftFormat.FftWindowSize, 2), dftSoundBin_x, dftSoundBin_y)

                                            'performs complex multiplications
                                            Dim tempDftSoundBin_x As Double = 0
                                            For n = 0 To fftFormat.FftWindowSize - 1
                                                tempDftSoundBin_x = dftSoundBin_x(n) 'stores this value so that it does not get overwritten in the following line (it needs to be used also two lines below)
                                                dftSoundBin_x(n) = tempDftSoundBin_x * dftIR_Bin_x(n) - dftSoundBin_y(n) * dftIR_Bin_y(n)
                                                dftSoundBin_y(n) = tempDftSoundBin_x * dftIR_Bin_y(n) + dftSoundBin_y(n) * dftIR_Bin_x(n)
                                            Next

                                            'Calculates inverse FFT
                                            FFT_Bourke(-1, Utils.getBase_n_Log(fftFormat.FftWindowSize, 2), dftSoundBin_x, dftSoundBin_y)

                                            'Puts the convoluted sound in the output array
                                            For sample = 0 To fftFormat.FftWindowSize - 1
                                                OutputChannelSampleArray(writeSample) += dftSoundBin_x(sample)
                                                writeSample += 1
                                            Next
                                            writeSample -= sliceLength

                                            'Referencin the sound array in the output sound
                                            outputSound.WaveData.SampleData(c) = OutputChannelSampleArray

                                        Next

                                        If KeepInputSoundLength = True Then

                                            'Correcting the channel length, by copying the section needed
                                            Dim ItitialTrimLength As Integer = impulseResponse.WaveData.SampleData(IRChannel).Length / 2
                                            Dim NewChannelArray(originalInputLength - 1) As Single
                                            For s = 0 To NewChannelArray.Length - 1
                                                NewChannelArray(s) = OutputChannelSampleArray(s + ItitialTrimLength)
                                            Next

                                            outputSound.WaveData.SampleData(c) = NewChannelArray

                                            'Fading the beginning and the end of the channel array
                                            DSP.Fade(outputSound, Nothing, 0, c, 0, Math.Min(NewChannelArray.Length / 10, 100))
                                            DSP.Fade(outputSound, 0, Nothing, c, NewChannelArray.Length - Math.Min(NewChannelArray.Length / 10, 100))

                                        Else

                                            ReDim Preserve outputSound.WaveData.SampleData(c)(intendedOutputLength - 1)

                                        End If

                                End Select

                                'Increasing impulse response channel if the impulse response has more than 1 channel.
                                If impulseResponse.WaveFormat.Channels > 1 Then IRChannel += 1

                            Next

                            Return outputSound

                        Case Else
                            Throw New NotImplementedException(inputSound.WaveFormat.BitDepth & " bit depth is not yet supported.")
                            Return Nothing
                    End Select


                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try


            End Function

            Public Enum ImpulseResponseScalings
                NoScaling
                SumEqualsZero
                SumEqualsOne
            End Enum

            Public Function FIRFilter_NEW(ByVal InputSound As Sound, ByVal ImpulseResponse As Sound,
                                 Optional ByRef FftFormat As Formats.FftFormat = Nothing, Optional ByVal InputSoundChannel As Integer? = Nothing,
                                  Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing,
                                  Optional ByVal FilteringDomain As ProcessingDomain = ProcessingDomain.FrequencyDomain,
                                  Optional ByVal ImpulseResponseScaling As ImpulseResponseScalings = ImpulseResponseScalings.NoScaling,
                                      Optional InActivateWarnings As Boolean = False,
                                  Optional ByVal KeepInputSoundLength As Boolean = False) As Sound

                Try

                    Dim IRChannel As Integer = 1
                    Dim outputSound As New Sound(InputSound.WaveFormat)
                    Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, InputSoundChannel)

                    'Copies the impulse response to a new array of double
                    Dim TempIR As Sound = ImpulseResponse.CreateSoundDataCopy

                    'Scaling impulse response, based on channel 1 data (so that the same scaling is applied to all channels)
                    Select Case ImpulseResponseScaling
                        Case ImpulseResponseScalings.NoScaling
'Doing nothing
                        Case ImpulseResponseScalings.SumEqualsZero
                            Dim ImpulseResponseChannel1ArrayAverage As Double = TempIR.WaveData.SampleData(1).Average
                            For c = 1 To TempIR.WaveFormat.Channels
                                For n = 0 To TempIR.WaveData.SampleData(c).Length - 1
                                    TempIR.WaveData.SampleData(c)(n) -= ImpulseResponseChannel1ArrayAverage
                                Next
                            Next

                        Case ImpulseResponseScalings.SumEqualsOne
                            Dim ImpulseResponseChannel1ArraySum As Double = TempIR.WaveData.SampleData(1).Sum
                            If ImpulseResponseChannel1ArraySum <> 0 Then 'Scaling only if needed
                                For c = 1 To TempIR.WaveFormat.Channels
                                    For n = 0 To TempIR.WaveData.SampleData(c).Length - 1
                                        TempIR.WaveData.SampleData(c)(n) /= ImpulseResponseChannel1ArraySum
                                    Next
                                Next
                            End If
                    End Select

                    If FftFormat Is Nothing Then FftFormat = New Formats.FftFormat

                    'Main section
                    Select Case InputSound.WaveFormat.BitDepth
                        Case 16, 32
                            For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                                'Getting the channel specific IR array (the highest available IR channel)
                                Dim IRArray() As Single
                                If c > TempIR.WaveFormat.Channels Then
                                    IRArray = TempIR.WaveData.SampleData(c)
                                Else
                                    IRArray = TempIR.WaveData.SampleData(TempIR.WaveFormat.Channels)
                                End If

                                'Referencing the current channel array, and noting its original length
                                Dim InputChannelArray() As Single = InputSound.WaveData.SampleData(c)
                                Dim OriginalChannelLength As Integer = InputChannelArray.Length

                                Select Case FilteringDomain
                                    Case ProcessingDomain.TimeDomain

                                        'Time domain convolution filtering

                                        'Copies the input array to a new extended array
                                        Dim ExtendedInputArray(InputChannelArray.Length + IRArray.Length - 2) As Single
                                        For n = 0 To InputChannelArray.Length - 1
                                            ExtendedInputArray(n) = InputChannelArray(n)
                                        Next

                                        'Starting a progress window
                                        Dim myProgress As New ProgressDisplay
                                        myProgress.Initialize(ExtendedInputArray.Length - 1, 0, "Calculating FIR filter...", IRArray.Length)
                                        myProgress.Show()
                                        Dim ProgressCounter As Integer = 0

                                        ReDim outputSound.WaveData.SampleData(c)(ExtendedInputArray.Length - 1)

                                        For n = 0 To ExtendedInputArray.Length - 1
                                            Dim cumulativeValue As Double = 0

                                            'Updating progress
                                            myProgress.UpdateProgress(ProgressCounter)
                                            ProgressCounter += 1

                                            'Doing the convolution
                                            For i = 0 To IRArray.Length - 1
                                                If n - i >= 0 Then
                                                    cumulativeValue += ExtendedInputArray(n - i) * IRArray(i)
                                                End If
                                            Next
                                            outputSound.WaveData.SampleData(c)(n) = cumulativeValue

                                        Next

                                        'Closing the progress display
                                        myProgress.Close()


                                    Case ProcessingDomain.FrequencyDomain
                                        'Frequency domain convolution (complex multiplication)

                                        'Reference: This frequency domain calculation is based on the overlap-add method as
                                        'described in Bateman, A. & Paterson-Stephens, I. (2002). The DSP Handbook. Algorithms, Applications and Design Techniques.
                                        'chapter 7, pp 451-453.

                                        Dim L As Integer = IRArray.Length

                                        'CheckAndAdjustFFTSize(fftFormat.FftWindowSize, (L * 2) + 1, InActivateWarnings)
                                        CheckAndAdjustFFTSize(FftFormat.FftWindowSize, L, InActivateWarnings)

                                        Dim sliceLength As Integer = FftFormat.FftWindowSize / 2

                                        'Copies the input array to a new array, if needed, also extends the input array to a whole number multiple of the length of the sound data that goes into each dft
                                        Dim IntendedOutputLength As Integer = InputChannelArray.Length + IRArray.Length - 1
                                        Dim NumberOfWindows As Integer = 1 + Int(InputChannelArray.Length / sliceLength)
                                        Dim workingInputLength As Integer = sliceLength * (NumberOfWindows + 1)

                                        Dim ExtendedInputArray(workingInputLength - 1) As Single
                                        For n = 0 To InputChannelArray.Length - 1
                                            ExtendedInputArray(n) = InputChannelArray(n)
                                        Next

                                        'Creates an output channel array
                                        Dim OutputChannelSampleArray(ExtendedInputArray.Length - 1) As Single
                                        outputSound.WaveData.SampleData(c) = OutputChannelSampleArray

                                        'Creates dft bins
                                        Dim DftSoundBin_x(FftFormat.FftWindowSize - 1) As Double
                                        Dim DftSoundBin_y(FftFormat.FftWindowSize - 1) As Double
                                        Dim DftIR_Bin_x(FftFormat.FftWindowSize - 1) As Double
                                        Dim DftIR_Bin_y(FftFormat.FftWindowSize - 1) As Double

                                        'Copies the IR samples into the DftIR_Bin_x array
                                        For sample = 0 To L - 1
                                            DftIR_Bin_x(sample) = IRArray(sample)
                                        Next

                                        'Calculates forward FFT for the IR (Skipping the forward transform scaling)
                                        FFT_Bourke(1, Utils.getBase_n_Log(FftFormat.FftWindowSize, 2), DftIR_Bin_x, DftIR_Bin_y, False)

                                        'Starts convolution one time window at a time, using the add-overlap method
                                        Dim readSample As Integer = 0
                                        Dim writeSample As Integer = 0

                                        For windowNumber = 0 To NumberOfWindows - 1

                                            'Copies sound data into DftSoundBin_x, and resetting the other samples to 0
                                            For sample = 0 To sliceLength - 1
                                                DftSoundBin_x(sample) = ExtendedInputArray(readSample)
                                                readSample += 1
                                                DftSoundBin_y(sample) = 0
                                            Next
                                            For sample = sliceLength To FftFormat.FftWindowSize - 1
                                                DftSoundBin_x(sample) = 0
                                                DftSoundBin_y(sample) = 0
                                            Next

                                            'Calculates forward FFT for the current sound window
                                            FFT_Bourke(1, Utils.getBase_n_Log(FftFormat.FftWindowSize, 2), DftSoundBin_x, DftSoundBin_y)

                                            'performs complex multiplications
                                            Dim tempDftSoundBin_x As Single = 0
                                            For n = 0 To FftFormat.FftWindowSize - 1
                                                tempDftSoundBin_x = DftSoundBin_x(n) 'stores this value so that it does not get overwritten in the following line (it needs to be used also two lines below)
                                                DftSoundBin_x(n) = tempDftSoundBin_x * DftIR_Bin_x(n) - DftSoundBin_y(n) * DftIR_Bin_y(n)
                                                DftSoundBin_y(n) = tempDftSoundBin_x * DftIR_Bin_y(n) + DftSoundBin_y(n) * DftIR_Bin_x(n)
                                            Next

                                            'Calculates inverse FFT
                                            FFT_Bourke(-1, Utils.getBase_n_Log(FftFormat.FftWindowSize, 2), DftSoundBin_x, DftSoundBin_y)

                                            'Puts the convoluted sound in the output array
                                            For sample = 0 To FftFormat.FftWindowSize - 1
                                                outputSound.WaveData.SampleData(c)(writeSample) += DftSoundBin_x(sample)
                                                writeSample += 1
                                            Next
                                            writeSample -= sliceLength

                                        Next


                                        If KeepInputSoundLength = True Then

                                            'Correcting the channel length, by copying the section needed
                                            Dim ItitialTrimLength As Integer = ImpulseResponse.WaveData.SampleData(IRChannel).Length / 2
                                            Dim NewChannelArray(OriginalChannelLength - 1) As Single
                                            For s = 0 To NewChannelArray.Length - 1
                                                NewChannelArray(s) = outputSound.WaveData.SampleData(c)(s + ItitialTrimLength)
                                            Next

                                            outputSound.WaveData.SampleData(c) = NewChannelArray

                                            'Fading the beginning and the end of the channel array
                                            DSP.Fade(outputSound, Nothing, 0, c, 0, Math.Min(NewChannelArray.Length / 10, 100))
                                            DSP.Fade(outputSound, 0, Nothing, c, NewChannelArray.Length - Math.Min(NewChannelArray.Length / 10, 100))

                                        Else

                                            ReDim Preserve outputSound.WaveData.SampleData(c)(IntendedOutputLength - 1)

                                        End If

                                End Select
                            Next

                            Return outputSound

                        Case Else
                            Throw New NotImplementedException(InputSound.WaveFormat.BitDepth & " bit depth is not yet supported.")
                            Return Nothing
                    End Select


                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try


            End Function


            Public Function Deconvolution(ByVal InputSound As Sound, ByVal ImpulseResponse As Sound,
                                 Optional ByRef FftFormat As Formats.FftFormat = Nothing, Optional ByVal InputSoundChannel As Integer? = Nothing,
                                  Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing,
                                      Optional ByVal LowerCutoffFrequency As Double? = Nothing, Optional ByVal UpperCutoffFrequency As Double? = Nothing,
                                      Optional InActivateWarnings As Boolean = False, Optional ByVal ShiftTruncateWindowAndScale As Boolean = True,
                                      Optional ByVal OutputLength As Integer = 48000) As Sound

                Try

                    Dim outputSound As New Sound(InputSound.WaveFormat)
                    Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, InputSoundChannel)
                    If FftFormat Is Nothing Then FftFormat = New Formats.FftFormat

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        'Getting the channel specific IR array (the highest available IR channel)
                        Dim ImpulseResponseChannel As Integer
                        If c > ImpulseResponse.WaveFormat.Channels Then
                            ImpulseResponseChannel = c
                        Else
                            ImpulseResponseChannel = ImpulseResponse.WaveFormat.Channels
                        End If

                        'Referencing the current channel array, and noting its original length
                        Dim Sound1ChannelArray() As Single = InputSound.WaveData.SampleData(c)
                        Dim Sound2ChannelArray() As Single = ImpulseResponse.WaveData.SampleData(ImpulseResponseChannel)

                        Dim OriginalChannelLength As Integer = Sound1ChannelArray.Length

                        CheckAndAdjustFFTSize(FftFormat.FftWindowSize, 8 * Math.Max(Sound1ChannelArray.Length, Sound2ChannelArray.Length), InActivateWarnings)


                        'Creats dft bins
                        Dim DftSound1_x_Bin(FftFormat.FftWindowSize - 1) As Double
                        Dim DftSound1_y_Bin(FftFormat.FftWindowSize - 1) As Double
                        Dim DftSound2_x_Bin(FftFormat.FftWindowSize - 1) As Double
                        Dim DftSound2_y_Bin(FftFormat.FftWindowSize - 1) As Double


                        'Copies sound 1 data into DftSound1_x_Bin, putting zero-padding initially
                        Dim Sound1ReadSample As Integer = 0
                        'For sample = 0 To FftFormat.FftWindowSize - InputChannelArray.Length - 1
                        '    DftSoundBin_x(sample) = 0
                        '    DftSoundBin_y(sample) = 0
                        'Next
                        For sample = FftFormat.FftWindowSize - Sound1ChannelArray.Length To FftFormat.FftWindowSize - 1
                            DftSound1_x_Bin(sample) = Sound1ChannelArray(Sound1ReadSample)
                            Sound1ReadSample += 1
                            'DftSoundBin_y(sample) = 0
                        Next

                        'Calculates forward FFT for sound 1
                        FFT_Bourke(1, Utils.getBase_n_Log(FftFormat.FftWindowSize, 2), DftSound1_x_Bin, DftSound1_y_Bin, False)


                        'Copies the Sound 2IR samples into DftSound2_x_Bin
                        'For sample = 0 To Sound2ChannelArray.Length - 1
                        '    DftSound2_x_Bin(sample) = Sound2ChannelArray(sample)
                        'Next
                        Dim Sound2ReadSample As Integer = 0
                        For sample = FftFormat.FftWindowSize - Sound2ChannelArray.Length To FftFormat.FftWindowSize - 1
                            DftSound2_x_Bin(sample) = Sound2ChannelArray(Sound2ReadSample)
                            Sound2ReadSample += 1
                            'DftSoundBin_y(sample) = 0
                        Next


                        'Calculates forward FFT for Sound 2
                        FFT_Bourke(1, Utils.getBase_n_Log(FftFormat.FftWindowSize, 2), DftSound2_x_Bin, DftSound2_y_Bin, True)


                        'Performs complex division
                        Dim StartIndex As Integer = 0
                        Dim EndIndex As Integer = FftFormat.FftWindowSize - 1

                        If LowerCutoffFrequency IsNot Nothing Then
                            StartIndex = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, LowerCutoffFrequency, InputSound.WaveFormat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.alwaysUp)
                        End If

                        If UpperCutoffFrequency IsNot Nothing Then
                            EndIndex = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, UpperCutoffFrequency, InputSound.WaveFormat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.alwaysDown)
                        End If

                        For n = StartIndex To EndIndex

                            'z1=a+bi z2=c+di 
                            'z1/z2 = (ac+bd)/(c^2+d^2) + (bc-ad)/(c^2+d^2)i
                            Dim cm_a As Double = DftSound1_x_Bin(n)
                            Dim cm_b As Double = DftSound1_y_Bin(n)
                            Dim cm_c As Double = DftSound2_x_Bin(n)
                            Dim cm_d As Double = DftSound2_y_Bin(n)

                            DftSound1_x_Bin(n) = (cm_a * cm_c + cm_b * cm_d) / (cm_c ^ 2 + cm_d ^ 2)
                            DftSound1_y_Bin(n) = (cm_b * cm_c - cm_a * cm_d) / (cm_c ^ 2 + cm_d ^ 2)

                        Next

                        'Calculates inverse FFT
                        FFT_Bourke(-1, Utils.getBase_n_Log(FftFormat.FftWindowSize, 2), DftSound1_x_Bin, DftSound1_y_Bin)

                        'Puts the convoluted sound in the output array
                        Dim OutputChannelArray(DftSound1_x_Bin.Length - 1) As Single
                        For s = 0 To DftSound1_x_Bin.Length - 1
                            OutputChannelArray(s) = DftSound1_x_Bin(s)
                        Next


                        If ShiftTruncateWindowAndScale = True Then


                            'Reference which this code is based on:
                            'The Scientist And Engineer's Guide to
                            'Digital Signal Processing
                            'By Steven W. Smith, Ph.D.
                            'http://www.dspguide.com/ch17/1.htm


                            'Shifting + truncating
                            Dim kernelArray(OutputLength - 1) As Single
                            Dim WriteIndex As Integer = 0
                            For n = 0 To OutputLength / 2 - 1
                                kernelArray(WriteIndex) = OutputChannelArray(OutputChannelArray.Length - (OutputLength / 2 - n))
                                WriteIndex += 1
                            Next
                            For n = 0 To OutputLength / 2 - 1
                                kernelArray(WriteIndex) = OutputChannelArray(n)
                                WriteIndex += 1
                            Next

                            'Windowing
                            WindowingFunction(kernelArray, WindowingType.Hamming)

                            'Scaling (to sum=1)
                            Dim Sum As Double = kernelArray.Sum
                            If Sum <> 0 Then
                                For n = 0 To kernelArray.Length - 1
                                    kernelArray(n) /= Sum
                                Next
                            End If

                            outputSound.WaveData.SampleData(c) = kernelArray

                        Else

                            outputSound.WaveData.SampleData(c) = OutputChannelArray

                        End If



                    Next

                    Return outputSound


                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try


            End Function



            Public Function ConvertSineSweepToImpulse(ByVal Sweep1 As Sound, ByVal Sweep2 As Sound,
                                                  Optional ByVal EqualizeSoundLevels As Boolean = True,
                                                  Optional ByRef TrimOutputToLength As Integer? = Nothing,
                                                  Optional ByRef ExportSounds As Boolean = False) As Sound

                'Equalizing sound levels of the input sounds (based on channel 1 data only, changes (i.e. gain) are applied to all channels)
                If EqualizeSoundLevels = True Then
                    Dim Sound1Level As Double = DSP.MeasureSectionLevel(Sweep1, 1)
                    Dim Sound2Level As Double = DSP.MeasureSectionLevel(Sweep2, 1)
                    Dim NeededSound2Gain As Double = Sound1Level - Sound2Level
                    AmplifySection(Sweep2, NeededSound2Gain)
                End If

                'Reversing the Original sweep
                Dim ReversedSweep1 As Sound = DSP.ReverseSound(Sweep1)

                If ExportSounds = True Then
                    AudioIOs.SaveToWaveFile(ReversedSweep1, AudioIOs.SaveSoundFileDialog(, "Sweep1_Reversed"))
                    AudioIOs.SaveToWaveFile(Sweep2, AudioIOs.SaveSoundFileDialog(, "Sweep2"))
                End If

                Dim Output = DSP.FIRFilter_NEW(Sweep2, ReversedSweep1)

                If TrimOutputToLength IsNot Nothing Then

                    'Trimming only if the signal is longer than requested
                    If Output.WaveData.SampleData(1).Length > TrimOutputToLength * Output.WaveFormat.SampleRate Then

                        'Trimming the centre of the signal
                        Output = DSP.CopySection(Output, Output.WaveData.SampleData(1).Length / 2 - ((TrimOutputToLength * Output.WaveFormat.SampleRate) / 2),
                                               TrimOutputToLength * Output.WaveFormat.SampleRate)

                        'Windowing
                        WindowingFunction(Output.WaveData.SampleData(1), WindowingType.Hamming)

                        ''Fading the edges
                        'DSP.Fade(Output, Nothing, 0,, 0, CInt(TrimOutputToLength * Output.WaveFormat.SampleRate / 20), FadeSlopeType.Smooth)
                        'DSP.Fade(Output, 0, Nothing,, Output.WaveData.SampleData(1).Length - CInt(TrimOutputToLength * Output.WaveFormat.SampleRate / 20), CInt(TrimOutputToLength * Output.WaveFormat.SampleRate / 20), FadeSlopeType.Smooth)

                    End If
                End If

                'Normalizing sum to 1
                For c = 1 To Output.WaveFormat.Channels
                    Dim ImpulseResponseArraySum As Double = Output.WaveData.SampleData(c).Sum
                    If ImpulseResponseArraySum <> 0 Then
                        For n = 0 To Output.WaveData.SampleData(c).Length - 1
                            Output.WaveData.SampleData(c)(n) /= ImpulseResponseArraySum
                        Next
                    Else
                        MsgBox("The impulse response derived from the sine sweep sums to 0 and cannot be normalized!")
                    End If
                Next


                Return Output

            End Function

            Public Function Resample_UsingResampAudio(ByVal InputSound As Sound,
                                                  ByVal TargetWaveFormat As Formats.WaveFormat,
                                                  Optional ByVal ResampAudioPath As String = "C:\Gamla D\EriksDokument\AudioProgrammingCode\AFsp_Win\ResampAudio.exe",
                                                  Optional ByVal WorkFolder As String = "",
                                                  Optional ByVal CopyPTWFObjectToOutput As Boolean = False) As Sound

                'This function is using the ResampAudio software to do the sample rate conversion
                'The resampler used is ResampAudio from http://www-mmsp.ece.mcgill.ca/Documents/Downloads/AFsp/index.html version: AFsp-v10r0.tar.gz from 2017-07 
                'See documentation for ResampAudio at http://www-mmsp.ece.mcgill.ca/Documents/Software/Packages/AFsp/audio/html/ResampAudio.html

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

                'Using the general log folder as default working path
                If WorkFolder = "" Then WorkFolder = Utils.logFilePath

                'Returns nothing if ResampAudio.exe cannot be found
                If Not File.Exists(ResampAudioPath) Then
                    MsgBox("The file ResampAudio.exe cannot be found at the following specified location:" & ResampAudioPath)
                    Return Nothing
                End If

                'Exporting the sound to file
                Dim TempSoundOriginalFileName As String = "TemporarySoundOriginal"
                Dim TempSoundResampledFileName As String = "TemporarySoundResampled"
                AudioIOs.SaveToWaveFile(InputSound, Path.Combine(WorkFolder, TempSoundOriginalFileName))

                'Creating resampled file
                Dim ResampSigStartInfo As New ProcessStartInfo()
                ResampSigStartInfo.FileName = ResampAudioPath
                ResampSigStartInfo.Arguments = "-s " & TargetWaveFormat.SampleRate.ToString & " -D " & DFormat & " " & Path.Combine(WorkFolder, TempSoundOriginalFileName) & ".wav " & Path.Combine(WorkFolder, TempSoundResampledFileName) & ".wav"
                ResampSigStartInfo.WorkingDirectory = WorkFolder
                Dim sp = Process.Start(ResampSigStartInfo)
                sp.WaitForExit()
                sp.Close()

                'Reading the resample files from file
                Dim ResampledSignalSound = AudioIOs.ReadWaveFile(Path.Combine(WorkFolder, TempSoundResampledFileName) & ".wav")

                'Deleting the temporary files
                File.Delete(Path.Combine(WorkFolder, TempSoundOriginalFileName) & ".wav")
                File.Delete(Path.Combine(WorkFolder, TempSoundResampledFileName) & ".wav")

                'Copying the SMA object
                If CopyPTWFObjectToOutput = True Then

                    'Copying a reference to the parent sound as this does not get serialized
                    Dim SMAParentSound = InputSound.SMA.ParentSound

                    ResampledSignalSound.SMA = InputSound.SMA.CreateCopy

                    ResampledSignalSound.SMA.ParentSound = SMAParentSound

                    MsgBox("Warning the PTWF temporal data will be incorrect, as sample rate has changed!")
                    'TODO We should write a method for SMA sample rate conversion
                End If

                Return ResampledSignalSound

            End Function


#Region "SpeechMasking"

            Public Function SplitTestSituationSoundsIntoSpeechMaskingSounds(ByRef InputSound As Sound,
                                                                      Optional ByVal UseOnlyChannel1 As Boolean = True,
                                                                      Optional ByVal SplitSilentSections As Boolean = True,
                                                                      Optional ByVal RemoveSilence As Boolean = True,
                                                                      Optional ByVal AddSilenceToSplitSectionLength As Boolean = True,
                                                                      Optional ByVal SplitIntoFixedLengthSections As Boolean = True,
                                                                      Optional ByVal FixedSectionDuration As Double = 3,
                                                                      Optional ByVal FixedSectionOverlapDuration As Double = 2.5,
                                                                      Optional ByVal FixedSectionFadeInDuration As Double = 1,
                                                                      Optional ByVal FixedSectionFadeOutDuration As Double = 1,
                                                                      Optional ByVal UseSpecificTestStimulusRegion As Boolean = True,
                                                                      Optional ByVal TestStimulusRegionStartTime As Double = 1.25,
                                                                      Optional ByVal TestStimulusRegionDuration As Double = 0.5,
                                                                      Optional ByVal MaximumAcceptedSilenceRatio As Double? = 0,
                                                                      Optional ByVal MinimumAcceptedSilenceRatio As Double? = 0,
                                                                      Optional ByVal MaximumAcceptedSoundLevelVariation As Double? = 3,
                                                                      Optional ByVal MinimumAcceptedSoundLevelVariation As Double? = 0,
                                                                      Optional ByVal MaxLevelTemporalIntegrationDuration As Double = 0.025,
                                                                      Optional ByVal MinimumIncludedMaxLevel_dBSPL As Double? = 45,
                                                                      Optional ByVal dBSPL_dBFS_Difference As Double? = Nothing,
                                                                      Optional ByVal MinimumIncludedRelativeMaxLevel As Double? = 30,
                                                                      Optional ByVal AcceptedMaxLevelUpwardDeviationOutsideTestStimulusRegion As Double? = 3,
                                                                      Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z) As List(Of Sound)


                Try

                    Dim TestStimulusRegionLength As Integer = Int(TestStimulusRegionDuration * InputSound.WaveFormat.SampleRate)
                    Dim TestStimulusRegionStartSample As Integer = Int(TestStimulusRegionStartTime * InputSound.WaveFormat.SampleRate)


                    'Section 1. Create sounds by splitting the input sound into sections.
                    Dim SoundList As New List(Of Sound)
                    Dim FileNameIterator As Integer = 0

                    'Separating input sound channels into different mono sounds
                    If InputSound.WaveFormat.Channels > 1 Then
                        For c = 1 To InputSound.WaveFormat.Channels

                            'Skipping if one one channels shoulc be used and we're on a channel number higher than 1
                            If UseOnlyChannel1 = True And c > 1 Then Continue For

                            Dim NewSound As Sound = DSP.CopySection(InputSound, 0, InputSound.WaveData.SampleData(c).Length, c)

                            'Adding the sound
                            SoundList.Add(NewSound)
                        Next
                    Else
                        'Adding the sound
                        SoundList.Add(InputSound)
                    End If

                    'Getting the level of the loudest section in the input files

                    Dim InputSoundMaxLevelList As New List(Of Double)
                    For Each Sound In SoundList
                        InputSoundMaxLevelList.Add(DSP.GetLevelOfLoudestWindow(Sound, 1, Sound.WaveFormat.SampleRate * MaxLevelTemporalIntegrationDuration,,,, FrequencyWeighting))
                    Next
                    Dim InputSoundMaxLevel As Double = InputSoundMaxLevelList.Max

                    'Splitting into different sounds defined by sections of silence in the original sounds, the silence is removed
                    If SplitSilentSections = True Then

                        'Putting all added File2Sounds In a temporary List
                        Dim TempSoundSections As New List(Of Sound)
                        For Each Sound In SoundList
                            TempSoundSections.Add(Sound)
                        Next

                        'Clearing File2Sounds
                        SoundList.Clear()

                        For Each Sound In TempSoundSections

                            'Splitting into new sounds
                            Dim NewSplit As List(Of Sound) = DSP.SplitSoundOnSilence(Sound, 30, 1 * Sound.WaveFormat.SampleRate, 1,, FrequencyWeighting, True)
                            For Each SplitSound In NewSplit
                                If SplitSound IsNot Nothing Then

                                    'Skipping the section if it's shorter than the test-stimulus region (if assigned). (To aviod very short sounds)
                                    If UseSpecificTestStimulusRegion = True Then
                                        If SplitSound.WaveData.SampleData(1).Length < TestStimulusRegionLength Then
                                            Continue For
                                        End If
                                    End If

                                    SoundList.Add(SplitSound)
                                End If
                            Next

                        Next
                    End If

                    'Removing silent sections of sounds. Is only needed if SplitSilentSections = False, as SplitSilentSections removes all silence.
                    If SplitSilentSections = False And RemoveSilence = True Then
                        For Each Sound In SoundList
                            Sound = DSP.RemoveSilentSections(Sound, 30, 0.1 * Sound.WaveFormat.SampleRate, 1,, FrequencyWeighting, True)
                        Next
                    End If

                    'Adding equal amount of silence on both sides of the sound (if the sound is shorter that the intended section length)
                    If AddSilenceToSplitSectionLength = True Then
                        For Each Sound In SoundList
                            Dim SoundLength As Integer = Sound.WaveData.SampleData(1).Length
                            Dim NeededSectionlength As Integer = Int(FixedSectionDuration * Sound.WaveFormat.SampleRate)

                            If SoundLength < NeededSectionlength Then

                                'Extending the sound to SectionDuration by Zeropadding it
                                Dim HalfPaddingLengthAsDouble As Double = (NeededSectionlength - SoundLength) / 2
                                If HalfPaddingLengthAsDouble Mod 2 <> 0 Then
                                    HalfPaddingLengthAsDouble = Int(HalfPaddingLengthAsDouble) + 1
                                End If
                                Dim HalfPaddingLength As Integer = HalfPaddingLengthAsDouble
                                Sound.ZeroPad(HalfPaddingLength, HalfPaddingLength)

                            End If
                        Next
                    End If


                    'Chopping up the current sounds into sections with FixedSectionDuration
                    If SplitIntoFixedLengthSections = True Then

                        'Putting all added File2Sounds In the temporary List again
                        Dim TempSoundSections As New List(Of Sound)
                        For Each Sound In SoundList
                            TempSoundSections.Add(Sound)
                        Next
                        SoundList.Clear()

                        For Each Sound In TempSoundSections

                            'Skipping if the Sound is nothing 
                            If Sound Is Nothing Then Continue For

                            'Or if sound length is 0
                            If Sound.WaveData.SampleData(1).Length = 0 Then Continue For

                            'Calculating lengths etc.
                            Dim FixedSectionlength As Integer = Int(FixedSectionDuration * Sound.WaveFormat.SampleRate)
                            Dim OverlapLength As Integer = Int(FixedSectionOverlapDuration * Sound.WaveFormat.SampleRate)
                            Dim StepSize As Integer = FixedSectionlength - OverlapLength
                            Dim SectionCount As Integer = Int(1 + (Sound.WaveData.SampleData(1).Length - FixedSectionlength) / StepSize)

                            For SectionIndex = 0 To SectionCount - 1

                                Dim StartReadSample As Integer = SectionIndex * StepSize
                                Dim NewSound As Sound = Nothing
                                If SectionCount = 1 And SectionIndex = 0 And Sound.WaveData.SampleData(1).Length = FixedSectionlength Then
                                    'Just referencing the sound if it's the only one there
                                    NewSound = Sound
                                Else
                                    'Creates a copy of the sound
                                    NewSound = DSP.CopySection(Sound, StartReadSample, FixedSectionlength, 1)
                                End If

                                'Skipping if the sound is too short after removing silence, or if it for some reason would be Nothing
                                If NewSound Is Nothing Then Continue For
                                If NewSound.WaveData.SampleData(1).Length < FixedSectionlength Then Continue For


                                Dim CentralizeLoudestSection As Boolean = True
                                Dim RestrictShiftLength As Boolean = True 'Restricts shift length so that we have some margin to test stimulus region
                                If CentralizeLoudestSection = True Then

                                    'Centralizing the loudest section of the sound to the middle of the measurement region.
                                    'If test stimulus region is specified, the sound is only shifted within the region +/- TestStimulusRegionLength
                                    Dim ShiftedSampleArray(FixedSectionlength - 1) As Single

                                    'Getting the sample of the loudest section
                                    Dim LoudestSectionStartSample As Integer
                                    Dim CurrentMaxLevel As Double
                                    If UseSpecificTestStimulusRegion = True Then
                                        CurrentMaxLevel = DSP.GetLevelOfLoudestWindow(NewSound, 1, NewSound.WaveFormat.SampleRate * 0.05, Math.Max(0, TestStimulusRegionStartSample - TestStimulusRegionLength), TestStimulusRegionLength * 3, LoudestSectionStartSample, FrequencyWeighting)
                                    Else
                                        CurrentMaxLevel = DSP.GetLevelOfLoudestWindow(NewSound, 1, NewSound.WaveFormat.SampleRate * 0.05,,, LoudestSectionStartSample, FrequencyWeighting)
                                    End If
                                    Dim LoudestSectionCenterSample As Integer = LoudestSectionStartSample + ((NewSound.WaveFormat.SampleRate * 0.05) / 2)

                                    Dim ReadStartSample As Integer = LoudestSectionCenterSample - (ShiftedSampleArray.Length / 2)
                                    If UseSpecificTestStimulusRegion = True And RestrictShiftLength = True Then
                                        ReadStartSample = Math.Max(ReadStartSample, -TestStimulusRegionLength * 1.5)
                                        ReadStartSample = Math.Min(ReadStartSample, TestStimulusRegionLength * 1.5)
                                    End If

                                    'Fading (this is done here since at this point we know in what direction the sound will be moved)
                                    Dim FadeInIsComplete As Boolean = False
                                    Dim FadeOutIsComplete As Boolean = False
                                    Select Case ReadStartSample
                                        Case < 0
                                            'Fading in (We have to fade in prior to shifting and fade out after shifting)
                                            If FixedSectionFadeInDuration > 0 Then
                                                Dim FadeInLength As Integer = FixedSectionFadeInDuration * NewSound.WaveFormat.SampleRate
                                                DSP.Fade(NewSound, Nothing, 0, 1, 0, FadeInLength, DSP.FadeSlopeType.Linear,, True)
                                            End If
                                            FadeInIsComplete = True

                                        Case > 0
                                            'Fading in (We have to fade out prior to shifting and fade in after shifting)
                                            If FixedSectionFadeOutDuration > 0 Then
                                                Dim FadeOutLength As Integer = FixedSectionFadeOutDuration * NewSound.WaveFormat.SampleRate
                                                Dim StartFadeSample As Integer = NewSound.WaveData.SampleData(1).Length - FadeOutLength
                                                DSP.Fade(NewSound, 0, Nothing, 1, StartFadeSample, FadeOutLength, DSP.FadeSlopeType.Linear,, True)
                                            End If
                                            FadeOutIsComplete = True

                                        Case = 0
                                            'Fading both in and out, since no shifting will take place
                                            If FixedSectionFadeInDuration > 0 Then
                                                Dim FadeInLength As Integer = FixedSectionFadeInDuration * NewSound.WaveFormat.SampleRate
                                                DSP.Fade(NewSound, Nothing, 0, 1, 0, FadeInLength, DSP.FadeSlopeType.Linear,, True)
                                            End If
                                            FadeInIsComplete = True

                                            If FixedSectionFadeOutDuration > 0 Then
                                                Dim FadeOutLength As Integer = FixedSectionFadeOutDuration * NewSound.WaveFormat.SampleRate
                                                Dim StartFadeSample As Integer = NewSound.WaveData.SampleData(1).Length - FadeOutLength
                                                DSP.Fade(NewSound, 0, Nothing, 1, StartFadeSample, FadeOutLength, DSP.FadeSlopeType.Linear,, True)
                                            End If
                                            FadeOutIsComplete = True
                                    End Select

                                    'Performs the shifting
                                    Dim WriteSampleIndex As Integer = -1
                                    For ReadSampleIndex = ReadStartSample To ReadStartSample + ShiftedSampleArray.Length - 1

                                        WriteSampleIndex += 1

                                        If ReadSampleIndex < 0 Then Continue For

                                        If ReadSampleIndex > (ShiftedSampleArray.Length - 1) Then Exit For

                                        'Copying the sample
                                        ShiftedSampleArray(WriteSampleIndex) = NewSound.WaveData.SampleData(1)(ReadSampleIndex)

                                    Next

                                    NewSound.WaveData.SampleData(1) = ShiftedSampleArray

                                    'Fading in
                                    If FadeInIsComplete = False Then
                                        Dim FadeInLength As Integer = FixedSectionFadeInDuration * NewSound.WaveFormat.SampleRate
                                        DSP.Fade(NewSound, Nothing, 0, 1, 0, FadeInLength, DSP.FadeSlopeType.Linear,, True)
                                    End If

                                    If FadeOutIsComplete = False Then
                                        Dim FadeOutLength As Integer = FixedSectionFadeOutDuration * NewSound.WaveFormat.SampleRate
                                        Dim StartFadeSample As Integer = NewSound.WaveData.SampleData(1).Length - FadeOutLength
                                        DSP.Fade(NewSound, 0, Nothing, 1, StartFadeSample, FadeOutLength, DSP.FadeSlopeType.Linear,, True)
                                    End If

                                Else


                                    If FixedSectionFadeOutDuration > 0 Then
                                        Dim FadeOutLength As Integer = FixedSectionFadeOutDuration * NewSound.WaveFormat.SampleRate
                                        Dim StartFadeSample As Integer = NewSound.WaveData.SampleData(1).Length - FadeOutLength
                                        DSP.Fade(NewSound, 0, Nothing, 1, StartFadeSample, FadeOutLength, DSP.FadeSlopeType.Linear,, True)
                                    End If

                                End If

                                'Adding the sound sections
                                SoundList.Add(NewSound)

                            Next
                        Next
                    End If



                    'Section 2. Excluding sounds that do not fullfil the input criteria, and sets the output sound level
                    Dim Output As New List(Of Sound)

                    For Each SoundSection In SoundList

                        'Skipping if the sound is too short (which would only happen if AddSilenceToSplitSectionLength = False)
                        Dim FixedSectionlength As Integer = Int(FixedSectionDuration * SoundSection.WaveFormat.SampleRate)
                        If SoundSection.WaveData.SampleData(1).Length < FixedSectionlength Then Continue For


                        'Declares a measurement sound. 
                        Dim MeasureMentSound As Sound = Nothing 'This is not needed if some functions below could measure levels on specific sections!
                        If UseSpecificTestStimulusRegion = True Then
                            Dim MeasurementRegionStartSample As Integer = TestStimulusRegionStartTime * SoundSection.WaveFormat.SampleRate
                            Dim MeasurementRegionLength As Integer = TestStimulusRegionDuration * SoundSection.WaveFormat.SampleRate
                            MeasureMentSound = DSP.CopySection(SoundSection, MeasurementRegionStartSample, MeasurementRegionLength)

                            'Copies the file name to the measurement sound
                            MeasureMentSound.FileName = SoundSection.FileName

                            'Supplying the SoundSection sound with segmentation data, on sentence level, as well as a the following word level segmentations 
                            '(word1: pre-measurement section, word2: measurement section, word 3: post measurement section)
                            Dim sentence As Integer = 0
                            SoundSection.SMA.ChannelData(1)(sentence).StartSample = 0
                            SoundSection.SMA.ChannelData(1)(sentence).Length = SoundSection.WaveData.SampleData(1).Length
                            SoundSection.SMA.ChannelData(1)(sentence).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(SoundSection.SMA, Sound.SpeechMaterialAnnotation.SmaTags.WORD, SoundSection.SMA.ChannelData(1)(sentence)) With {.StartSample = 0, .Length = Math.Max(0, MeasurementRegionStartSample), .OrthographicForm = "", .PhoneticForm = ""})
                            SoundSection.SMA.ChannelData(1)(sentence).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(SoundSection.SMA, Sound.SpeechMaterialAnnotation.SmaTags.WORD, SoundSection.SMA.ChannelData(1)(sentence)) With {.StartSample = MeasurementRegionStartSample, .Length = MeasurementRegionLength, .OrthographicForm = "", .PhoneticForm = ""})
                            SoundSection.SMA.ChannelData(1)(sentence).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(SoundSection.SMA, Sound.SpeechMaterialAnnotation.SmaTags.WORD, SoundSection.SMA.ChannelData(1)(sentence)) With {.StartSample = MeasurementRegionStartSample + MeasurementRegionLength, .Length = SoundSection.WaveData.SampleData(1).Length - (MeasurementRegionStartSample + MeasurementRegionLength), .OrthographicForm = "", .PhoneticForm = ""})

                        Else
                            MeasureMentSound = SoundSection

                            'Supplying the SoundSection sound with segmentation data, on sentence level, as well as a single word level segmentation 
                            Dim sentence As Integer = 0
                            SoundSection.SMA.ChannelData(1)(sentence).StartSample = 0
                            SoundSection.SMA.ChannelData(1)(sentence).Length = SoundSection.WaveData.SampleData(1).Length
                            SoundSection.SMA.ChannelData(1)(sentence).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(SoundSection.SMA, Sound.SpeechMaterialAnnotation.SmaTags.WORD, SoundSection.SMA.ChannelData(1)(sentence)) With {.StartSample = 0, .Length = SoundSection.WaveData.SampleData(1).Length, .OrthographicForm = "", .PhoneticForm = ""})

                        End If

                        'Excluding sounds with a maximum sound level below MinimumIncludedMaxLevel_dBSPL
                        Dim MeasurementSoundMaxLevel As Double = DSP.GetLevelOfLoudestWindow(MeasureMentSound, 1, SoundSection.WaveFormat.SampleRate * MaxLevelTemporalIntegrationDuration,,,, FrequencyWeighting)
                        If MinimumIncludedMaxLevel_dBSPL IsNot Nothing Then

                            'Using the default calibration stored in Simulated_dBFS_dBSPL_Difference, if no dBSPL_dBFS_Difference is assigned
                            If dBSPL_dBFS_Difference Is Nothing Then dBSPL_dBFS_Difference = FullScaleSoundFieldOutputSoundLevel
                            FullScaleSoundFieldOutputSoundLevel = dBSPL_dBFS_Difference

                            If MeasurementSoundMaxLevel < Convert_dBSPL_To_dBFS(MinimumIncludedMaxLevel_dBSPL, True) Then Continue For
                        End If


                        'Excluding sounds with a maximum sound level MinimumIncludedRelativeMaxLevel below the loudest window in the input file
                        If MinimumIncludedRelativeMaxLevel IsNot Nothing Then
                            If MeasurementSoundMaxLevel < (InputSoundMaxLevel - MinimumIncludedRelativeMaxLevel) Then Continue For
                        End If


                        'Determining the silence ratio
                        If MaximumAcceptedSilenceRatio IsNot Nothing Or MinimumAcceptedSilenceRatio IsNot Nothing Then
                            Dim SilenceRatio As Double = DSP.GetSilenceRatio(MeasureMentSound, 30, 0.1 * MeasureMentSound.WaveFormat.SampleRate, 1, FrequencyWeighting)
                            If MaximumAcceptedSilenceRatio IsNot Nothing Then If SilenceRatio > MaximumAcceptedSilenceRatio Then Continue For
                            If MinimumAcceptedSilenceRatio IsNot Nothing Then If SilenceRatio < MinimumAcceptedSilenceRatio Then Continue For
                        End If

                        'Determining the sound level variation
                        If MaximumAcceptedSoundLevelVariation IsNot Nothing Or MinimumAcceptedSoundLevelVariation IsNot Nothing Then
                            Dim SoundLevelVariation As Double = DSP.GetSoundLevelVariation(MeasureMentSound, Int(MaxLevelTemporalIntegrationDuration * MeasureMentSound.WaveFormat.SampleRate), 1, True, 30, FrequencyWeighting)
                            If MaximumAcceptedSoundLevelVariation IsNot Nothing Then If SoundLevelVariation > MaximumAcceptedSoundLevelVariation Then Continue For
                            If MinimumAcceptedSoundLevelVariation IsNot Nothing Then If SoundLevelVariation < MinimumAcceptedSoundLevelVariation Then Continue For
                        End If


                        'Makes sure that the max level outside the stimulus region does not exceed AcceptedFullSoundMaxLevelUpwardDeviation dB above the max level in the stimulus region
                        If UseSpecificTestStimulusRegion = True Then
                            If AcceptedMaxLevelUpwardDeviationOutsideTestStimulusRegion IsNot Nothing Then
                                Dim SoundSectionMaxLevel As Double = DSP.GetLevelOfLoudestWindow(SoundSection, 1, SoundSection.WaveFormat.SampleRate * MaxLevelTemporalIntegrationDuration,,,, FrequencyWeighting)
                                If SoundSectionMaxLevel > MeasurementSoundMaxLevel + AcceptedMaxLevelUpwardDeviationOutsideTestStimulusRegion Then Continue For
                            End If
                        End If

                        'Assigning file names to the output sounds
                        'Creating a file name based on the FileNameIterator
                        SoundSection.FileName = InputSound.FileName & "_" & FileNameIterator

                        'Increasing the FileNameIterator each time a new output sound section is added
                        FileNameIterator += 1

                        'Adding the sounds to the output
                        Output.Add(SoundSection)

                    Next

                    Return Output

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try


            End Function

#End Region

#Region "SpeechSynthesis"

            Public Class PitchContour
                ''' <summary>
                ''' Holds the fundamental pitch of periods located at specific samples points
                ''' </summary>
                Public InstantaneousPitch As New SortedList(Of Integer, Single)
            End Class

            Public Sub PSOLA(ByRef InputSound As Sound, ByRef PitchContour As PitchContour)


                '

                'Identifies voiced segments



                'PitchContour



            End Sub


            Public Function AutoCorrelation(ByRef InputSignal As Single(),
                                        Optional Normalize As Boolean = False,
                                        Optional CalculateOnlyRightTail As Boolean = True) As Single()


                If CalculateOnlyRightTail = False Then

                    Dim OutputSignal(InputSignal.Length * 2 - 2) As Single

                    Dim MultiplicationStartIndex As Integer = InputSignal.Length - 1
                    For lag = 0 To InputSignal.Length - 1
                        For i = 0 To InputSignal.Length - 1
                            OutputSignal(lag + i) += InputSignal(i) * InputSignal(MultiplicationStartIndex)
                        Next
                        MultiplicationStartIndex -= 1
                    Next

                    If Normalize = True Then
                        For lag = 0 To InputSignal.Length - 1
                            OutputSignal(lag) = OutputSignal(lag) / (lag + 1)
                        Next

                        For n = 0 To InputSignal.Length - 2
                            Dim InverseLagFromEnd As Integer = OutputSignal.Length - 1 - n
                            OutputSignal(InverseLagFromEnd) = OutputSignal(InverseLagFromEnd) / (n + 1)
                        Next

                    End If

                    Return OutputSignal

                Else

                    Dim OutputSignal(InputSignal.Length - 1) As Single

                    Dim MultiplicationStartIndex As Integer = InputSignal.Length - 1
                    For i = 0 To InputSignal.Length - 1

                        For j = 0 To InputSignal.Length - 1 - i
                            OutputSignal(i) += InputSignal(j) * InputSignal(j + i)
                        Next

                    Next

                    If Normalize = True Then
                        Dim InverseLag As Integer = InputSignal.Length
                        For n = 0 To InputSignal.Length - 1
                            OutputSignal(n) /= InverseLag
                            InverseLag -= 1
                        Next
                    End If

                    Return OutputSignal

                End If


            End Function

#End Region

#Region "IsoPhonFilters"


            <Serializable>
            Public Class IsoPhonFilter

                Private SPLToPhonLookupTable As SortedList(Of Double, SortedList(Of Double, Double)) 'Frequency, SPL, Phon
                Private SplAtZeroPhon As SortedList(Of Integer, Double) 'Frequency, SPL

                'TODO: As of now (2017-07-13) the data below is taken from https://www.dsprelated.com/showcode/174.php , and not double checked with ISO 226.
                Dim f As Double() = {20, 25, 31.5, 40, 50, 63, 80, 100, 125, 160, 200, 250, 315,
            400, 500, 630, 800, 1000, 1250, 1600, 2000, 2500, 3150, 4000, 5000, 6300, 8000, 10000, 12500}

                Dim af As Double() = {0.532, 0.506, 0.48, 0.455, 0.432, 0.409, 0.387, 0.367,
            0.349, 0.33, 0.315, 0.301, 0.288, 0.276, 0.267, 0.259, 0.253, 0.25, 0.246,
            0.244, 0.243, 0.243, 0.243, 0.242, 0.242, 0.245, 0.254, 0.271, 0.301}

                Dim Lu As Double() = {-31.6, -27.2, -23, -19.1, -15.9, -13, -10.3, -8.1,
            -6.2, -4.5, -3.1, -2, -1.1, -0.4, 0, 0.3, 0.5, 0, -2.7, -4.1, -1, 1.7,
            2.5, 1.2, -2.1, -7.1, -11.2, -10.7, -3.1}

                Dim Tf As Double() = {78.5, 68.7, 59.5, 51.1, 44, 37.5, 31.5, 26.5, 22.1,
            17.9, 14.4, 11.4, 8.6, 6.2, 4.4, 3, 2.2, 2.4, 3.5, 1.7, -1.3, -4.2, -6,
            -5.4, -1.5, 6, 12.6, 13.9, 12.3}

                Dim LevelResolution As Double
                Dim LevelDecimalPoints As Integer
                Dim Lowest_Level As Double
                Dim Highest_Level As Double

                ''' <summary>
                ''' May be changed to False if also negative frequencies need to be filterred. Default value is True.
                ''' </summary>
                ''' <returns></returns>
                Public Property SkipNegativeFrequencies As Boolean = True

                Public Enum FilterTypes
                    Adaptive
                    A_Filter
                    B_Filter
                    C_Filter
                End Enum



                ''' <summary>
                ''' Creates a new instance of IsoPhonConversion.
                ''' </summary>
                ''' <param name="LookupFrequencies "></param>
                ''' <param name="SetLevelDecimalPoints"></param>
                Public Sub New(ByVal LookupFrequencies As List(Of Double),
                                     Optional ByVal SetLevelDecimalPoints As Integer = 1,
                                     Optional ByVal SetLowest_Level As Double = -100,
                                     Optional ByVal SetHighest_Level As Double = 110)

                    LevelDecimalPoints = SetLevelDecimalPoints
                    LevelResolution = 10 ^ -LevelDecimalPoints
                    Lowest_Level = SetLowest_Level
                    Highest_Level = SetHighest_Level

                    'Creating a list containing the SPL at each frequency at 0 Phon
                    SplAtZeroPhon = New SortedList(Of Integer, Double)
                    For freqIndex = 0 To f.Length - 1
                        'Calculating SPL value for 0 phon
                        SplAtZeroPhon.Add(freqIndex, GetPhonToSpl(0, freqIndex))
                    Next

                    SPLToPhonLookupTable = New SortedList(Of Double, SortedList(Of Double, Double)) 'Frequency, SPL, Phon

                    For Each freq In LookupFrequencies

                        'Adding Frequency key to the table
                        SPLToPhonLookupTable.Add(freq, New SortedList(Of Double, Double))

                        'Getting the lower and higher frequencies for interpolation
                        Dim NearestAvailableFrequencies = Utils.GetNearestIndices(freq, f)

                        Dim LowerFrequencyIndex As Integer? = NearestAvailableFrequencies.NearestLowerIndex
                        Dim HigherFrequencyIndex As Integer? = NearestAvailableFrequencies.NearestHigherIndex

                        'Setting the interpolation frequencies to the closest possible if frequency value is outside the interpolation range
                        If LowerFrequencyIndex Is Nothing Then
                            LowerFrequencyIndex = 0
                            HigherFrequencyIndex = 1
                        ElseIf HigherFrequencyIndex Is Nothing Then
                            LowerFrequencyIndex = f.Length - 2
                            HigherFrequencyIndex = f.Length - 1
                        End If

                        'Checking if interpolation between frequencies is needed
                        If LowerFrequencyIndex = HigherFrequencyIndex Then

                            'No interpolation needed since the current frequency is in the f array
                            For CurrentSPL As Double = Lowest_Level To Highest_Level Step LevelResolution

                                Dim roundedSPL As Double = Math.Round(CurrentSPL, LevelDecimalPoints)

                                'Calculating Phon value
                                SPLToPhonLookupTable(freq).Add(roundedSPL, roundedSPL - GetSPLToPhon(roundedSPL, LowerFrequencyIndex)) 'N.B. LowerFrequencyIndex can be used since it is the same as HigherFrequencyIndex

                            Next

                        Else

                            'Interpolation is needed
                            'Getting the lower and higher Phon values for interpolation
                            Dim LowerPhons As New SortedList(Of Double, Double)
                            Dim HigherPhons As New SortedList(Of Double, Double)

                            For CurrentSPL As Double = Lowest_Level To Highest_Level Step LevelResolution

                                Dim roundedSPL As Double = Math.Round(CurrentSPL, LevelDecimalPoints)

                                'Calculating lower Phon values
                                LowerPhons.Add(roundedSPL, roundedSPL - GetSPLToPhon(roundedSPL, LowerFrequencyIndex))

                                'Calculating higher Phon values
                                HigherPhons.Add(roundedSPL, roundedSPL - GetSPLToPhon(roundedSPL, HigherFrequencyIndex))
                            Next

                            'Interpolating the current frequency data (all SPL To Phons for the current frequency)
                            For CurrentSPL As Double = Lowest_Level To Highest_Level Step LevelResolution

                                Dim roundedSPL As Double = Math.Round(CurrentSPL, LevelDecimalPoints)
                                SPLToPhonLookupTable(freq).Add(roundedSPL, Utils.LinearInterpolation(freq, f(LowerFrequencyIndex), LowerPhons(roundedSPL), f(HigherFrequencyIndex), HigherPhons(roundedSPL), True))
                            Next
                        End If

                    Next


                End Sub

                ''' <summary>
                ''' Returns the Phon value of an input SPL, at the indicated frequency (Based on ISO 226).
                ''' </summary>
                ''' <param name="InputValue"></param>
                ''' <returns></returns>
                Private Function GetEqualLoudness(ByVal InputValue As Double, ByVal Frequency As Double) As Double

                    Dim RoundedSIL As Double = Math.Round(InputValue, LevelDecimalPoints) 'Rounded SPL is rounded to the number of level decimal point used in setting up the SPLToPhonLookupTable

                    'Checking if the value is lower than the minimum generated range
                    If RoundedSIL < Lowest_Level Then
                        RoundedSIL = Lowest_Level 'Setting it to the lowest generated value
                    End If

                    If RoundedSIL > Highest_Level Then RoundedSIL = Highest_Level 'Setting it to the highest generated value

                    'If SPLToPhonLookupTable(RoundedFrequency).ContainsKey(RoundedSIL) Then
                    Return SPLToPhonLookupTable(Frequency)(RoundedSIL)
                    'Else
                    'Throws an exception if no key larger enough has been generated
                    'Throw New Exception("The SPL value was outside the generated range. Please increase the Highest_Level set when setting up the SPLToPhonLookupTable!")
                    'End If

                End Function

                Public Function GetAttenuation(ByVal FilterLevel As Double, ByVal Frequency As Double) As Double

                    Dim RoundedSIL As Double = Math.Round(FilterLevel, LevelDecimalPoints) 'Rounded SPL is rounded to the number of level decimal point used in setting up the SPLToPhonLookupTable

                    'Checking if the value is lower than the minimum generated range
                    If RoundedSIL < Lowest_Level Then
                        RoundedSIL = Lowest_Level 'Setting it to the lowest generated value
                    End If

                    If RoundedSIL > Highest_Level Then
                        RoundedSIL = Highest_Level 'Setting it to the highest generated value
                        Throw New Exception("The SPL value was outside the generated range. Please increase the Highest_Level set when setting up the SPLToPhonLookupTable!")
                    End If
                    Try
                        Return SPLToPhonLookupTable(Frequency)(RoundedSIL)
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try

                End Function



                ''' <summary>
                ''' Returns the Phon value for the specified input SPL, at the frequency specified in f(FrequencyIndex).
                ''' </summary>
                ''' <param name="InputSIL"></param>
                ''' <param name="FrequencyIndex">The specified index in the public frequency array f.</param>
                ''' <returns></returns>
                Private Function GetSPLToPhon(ByVal InputSIL As Double, ByVal FrequencyIndex As Double) As Double

                    'If the function is fed by an SIL value that would fall below the SPL values genererated at zero phon, 
                    'the function returns the sum of zero phon minus the difference between the SIL value for zero phon and the input SIL value.
                    'The reason this procedure is used is that the PhonToSPL equation is not valid for (actually freaks out!) below 0 phons. 

                    Dim ZeroPhonValue As Double = SplAtZeroPhon(FrequencyIndex)
                    If InputSIL < ZeroPhonValue Then

                        Dim Lp_InputSILDifference As Double = ZeroPhonValue - InputSIL

                        'Returning 0 phon minus the Lp_InputSILDifference
                        Return -Lp_InputSILDifference

                    Else
                        Dim Lp As Double = InputSIL
                        Dim AAf As Double = 10 ^ ((af(FrequencyIndex) * (Lp + Lu(FrequencyIndex) - 94)) / 10)
                        Dim Ln As Double = (Math.Log10(((AAf - ((0.4 * 10 ^ (((Tf(FrequencyIndex) + Lu(FrequencyIndex)) / 10) - 9)) ^ af(FrequencyIndex))) / (4.47 * 10 ^ (-3))) + 1.14)) / 0.025

                        Return Ln
                    End If

                End Function

                ''' <summary>
                ''' Returns the SPL value for the specified input Phon value, at the frequency specified in f(FrequencyIndex).
                ''' </summary>
                ''' <param name="InputPhon"></param>
                ''' <param name="FrequencyIndex">The specified index in the public frequency array f.</param>
                ''' <returns></returns>
                Private Function GetPhonToSpl(ByVal InputPhon As Double, ByVal FrequencyIndex As Double) As Double

                    'Calculating Phon value
                    Dim Ln As Double = InputPhon
                    Dim AAf As Double = (4.47 * 10 ^ -3) * (10 ^ (0.025 * Ln) - 1.14) + (0.4 * 10 ^ ((Tf(FrequencyIndex) + Lu(FrequencyIndex)) / 10 - 9)) ^ af(FrequencyIndex)
                    Dim Lp As Double = ((10 / af(FrequencyIndex)) * Math.Log10(AAf)) - Lu(FrequencyIndex) + 94
                    Return Lp

                End Function

                ''' <summary>
                ''' Filters the power spectrum of a Sound using the current Iso-Phon filter.
                ''' </summary>
                ''' <param name="InputSound"></param>
                Public Sub FilterPowerSpectrum(ByRef InputSound As Sound, ByVal dbFSToSplDifference As Double,
                                           Optional FilterType As FilterTypes = FilterTypes.Adaptive)

                    Dim BinCount As Integer = InputSound.FFT.BinIndexToFrequencyList(, SkipNegativeFrequencies).Count

                    For channel = 1 To InputSound.FFT.ChannelCount
                        For TimeWindow = 0 To InputSound.FFT.WindowCount(channel) - 1

                            Dim FilterLevel As Double
                            Select Case FilterType
                                Case FilterTypes.Adaptive
                                    'Getting the total power of the current time window
                                    InputSound.FFT.PowerSpectrumData(channel, TimeWindow).CalculateTotalPower()
                                    Dim TotalPower As Double = InputSound.FFT.PowerSpectrumData(channel, TimeWindow).TotalPower
                                    Dim TotalPowerIn_dBFS As Double = 10 * Math.Log10(TotalPower / InputSound.WaveFormat.PositiveFullScale)
                                    Dim TotalPowerIn_dBSIL As Double = (TotalPowerIn_dBFS + dbFSToSplDifference)
                                    FilterLevel = TotalPowerIn_dBSIL
                                Case FilterTypes.A_Filter
                                    FilterLevel = 40

                                Case FilterTypes.B_Filter
                                    FilterLevel = 70

                                Case FilterTypes.C_Filter
                                    FilterLevel = 100

                            End Select

                            For k = 0 To BinCount - 1

                                'Converting values to dB scale, and shifts the Levels to SIL range
                                Dim CurrentBandValue As Double = InputSound.FFT.PowerSpectrumData(channel, TimeWindow).WindowData(k)
                                Dim ValueIn_dBFS As Double = 10 * Math.Log10(CurrentBandValue / InputSound.WaveFormat.PositiveFullScale)
                                Dim ValueIn_dBSIL As Double = (ValueIn_dBFS + dbFSToSplDifference)

                                'Getting the attenuation for the current SIL / frequency combination
                                Dim CurrentAttenuation As Double = GetAttenuation(FilterLevel, InputSound.FFT.BinIndexToFrequencyList()(k))
                                Dim CurrentPhonValue As Double = ValueIn_dBSIL - CurrentAttenuation

                                'Leaving it in the SIL range and not taking -dbFSToSplDifference

                                'Shifting back to Linear scale (I= Ir * 10^(LI/10))
                                Dim LinearLoudness As Double = ReferenceSoundIntensityLevel * 10 ^ (CurrentPhonValue / 10)

                                'Storing the new value
                                InputSound.FFT.PowerSpectrumData(channel, TimeWindow).WindowData(k) = LinearLoudness

                            Next
                        Next
                    Next

                End Sub


                Public Sub ExportSplToPhonData(Optional ByVal OutputFolder As String = "", Optional ByVal FileName As String = "SplToPhonData",
                                           Optional ByVal ExportLevelStep As Double? = Nothing)

                    If OutputFolder = "" Then OutputFolder = Utils.logFilePath

                    Dim OutputList As New List(Of String)

                    Dim TempLevelStep As Double = LevelResolution
                    If ExportLevelStep IsNot Nothing Then TempLevelStep = ExportLevelStep

                    For InputLevel As Double = Lowest_Level To Highest_Level Step TempLevelStep
                        Dim RoundedSIL As Double = Math.Round(InputLevel, LevelDecimalPoints) 'Rounded SPL is rounded to the number of level decimal point used in setting up the SPLToPhonLookupTable
                        For Each Frequency In SPLToPhonLookupTable
                            OutputList.Add(InputLevel & vbTab & Frequency.Key & vbTab & SPLToPhonLookupTable(Frequency.Key)(RoundedSIL))
                        Next
                    Next

                    SendInfoToAudioLog(vbCrLf & String.Join(vbCrLf, OutputList), FileName, OutputFolder)

                End Sub

                Public Sub ExportIsoPhonCurves(Optional ByVal OutputFolder As String = "", Optional ByVal FileName As String = "IsoPhoneData")

                    If OutputFolder = "" Then OutputFolder = Utils.logFilePath

                    Dim OutputList As New List(Of String)

                    OutputList.Add("Phon" & vbTab & "Frequency" & vbTab & "SPL (dB)")
                    For InputPhon As Double = -100 To 100 Step 10
                        For FrequencyIndex = 0 To f.Length - 1
                            OutputList.Add(InputPhon & vbTab & f(FrequencyIndex) & vbTab & GetPhonToSpl(InputPhon, FrequencyIndex))
                        Next
                    Next

                    SendInfoToAudioLog(vbCrLf & String.Join(vbCrLf, OutputList), FileName, OutputFolder)

                End Sub

                Public Sub ExportInverseIsoPhonCurves(Optional ByVal OutputFolder As String = "",
                                                  Optional ByVal FileName As String = "InverseIsoPhonData",
                                                  Optional ByVal ShowAsAttenuation As Boolean = False)

                    If OutputFolder = "" Then OutputFolder = Utils.logFilePath

                    Dim OutputList As New List(Of String)

                    OutputList.Add("SPL" & vbTab & "Frequency" & vbTab & "Phon")
                    For InputSPL As Double = -100 To 100 Step 10
                        For FrequencyIndex = 0 To f.Length - 1
                            If ShowAsAttenuation = False Then
                                OutputList.Add(InputSPL & vbTab & f(FrequencyIndex) & vbTab & GetSPLToPhon(InputSPL, FrequencyIndex))
                            Else
                                OutputList.Add(InputSPL & vbTab & f(FrequencyIndex) & vbTab & InputSPL - GetSPLToPhon(InputSPL, FrequencyIndex))
                            End If
                        Next
                    Next

                    SendInfoToAudioLog(vbCrLf & String.Join(vbCrLf, OutputList), FileName, OutputFolder)

                End Sub


            End Class

            Public Class IsoPhonFilter_OLD

                Private SPLToPhonLookupTable As SortedList(Of Double, SortedList(Of Double, Double)) 'Frequency, SPL, Phon
                Private SplAtZeroPhon As SortedList(Of Integer, Double) 'Frequency, SPL

                'TODO: As of now (2017-07-13) the data below is taken from https://www.dsprelated.com/showcode/174.php , and not double checked with ISO 226.
                Dim f As Double() = {20, 25, 31.5, 40, 50, 63, 80, 100, 125, 160, 200, 250, 315,
            400, 500, 630, 800, 1000, 1250, 1600, 2000, 2500, 3150, 4000, 5000, 6300, 8000, 10000, 12500}

                Dim af As Double() = {0.532, 0.506, 0.48, 0.455, 0.432, 0.409, 0.387, 0.367,
            0.349, 0.33, 0.315, 0.301, 0.288, 0.276, 0.267, 0.259, 0.253, 0.25, 0.246,
            0.244, 0.243, 0.243, 0.243, 0.242, 0.242, 0.245, 0.254, 0.271, 0.301}

                Dim Lu As Double() = {-31.6, -27.2, -23, -19.1, -15.9, -13, -10.3, -8.1,
            -6.2, -4.5, -3.1, -2, -1.1, -0.4, 0, 0.3, 0.5, 0, -2.7, -4.1, -1, 1.7,
            2.5, 1.2, -2.1, -7.1, -11.2, -10.7, -3.1}

                Dim Tf As Double() = {78.5, 68.7, 59.5, 51.1, 44, 37.5, 31.5, 26.5, 22.1,
            17.9, 14.4, 11.4, 8.6, 6.2, 4.4, 3, 2.2, 2.4, 3.5, 1.7, -1.3, -4.2, -6,
            -5.4, -1.5, 6, 12.6, 13.9, 12.3}

                Dim LevelResolution As Double
                Dim LevelDecimalPoints As Integer
                Dim Lowest_Level As Double
                Dim Highest_Level As Double

                ''' <summary>
                ''' May be changed to False if also negative frequencies need to be filterred. Default value is True.
                ''' </summary>
                ''' <returns></returns>
                Public Property SkipNegativeFrequencies As Boolean = True

                Public Enum FilterTypes
                    IsoPhon
                    A_Filter
                    B_Filter
                    C_Filter
                End Enum



                ''' <summary>
                ''' Creates a new instance of IsoPhonConversion.
                ''' </summary>
                ''' <param name="LookupFrequencies "></param>
                ''' <param name="SetLevelDecimalPoints"></param>
                ''' <param name="LimitToAudibleRange"></param>
                Public Sub New(ByVal LookupFrequencies As List(Of Double),
                            Optional ByVal IsoPhonFilterType As FilterTypes = FilterTypes.IsoPhon,
                                     Optional ByVal SetLevelDecimalPoints As Integer = 1,
                                     Optional ByVal LimitToAudibleRange As Boolean = True,
                                     Optional ByVal SetLowest_Level As Double = -100,
                                     Optional ByVal SetHighest_Level As Double = 110)

                    LevelDecimalPoints = SetLevelDecimalPoints
                    LevelResolution = 10 ^ -LevelDecimalPoints
                    Lowest_Level = SetLowest_Level
                    Highest_Level = SetHighest_Level

                    'Creating a list containing the SPL at each frequency at 0 Phon
                    SplAtZeroPhon = New SortedList(Of Integer, Double)
                    For freqIndex = 0 To f.Length - 1
                        'Calculating SPL value for 0 phon
                        SplAtZeroPhon.Add(freqIndex, GetPhonToSpl(0, freqIndex))
                    Next

                    SPLToPhonLookupTable = New SortedList(Of Double, SortedList(Of Double, Double)) 'Frequency, SPL, Phon

                    For Each freq In LookupFrequencies

                        'Adding Frequency key to the table
                        SPLToPhonLookupTable.Add(freq, New SortedList(Of Double, Double))

                        'Getting the lower and higher frequencies for interpolation
                        Dim NearestAvailableFrequencies = Utils.GetNearestIndices(freq, f)

                        Dim LowerFrequencyIndex As Integer? = NearestAvailableFrequencies.NearestLowerIndex
                        Dim HigherFrequencyIndex As Integer? = NearestAvailableFrequencies.NearestHigherIndex

                        'Setting the interpolation frequencies to the closest possible if frequency value is outside the interpolation range
                        If LowerFrequencyIndex Is Nothing Then
                            LowerFrequencyIndex = 0
                            HigherFrequencyIndex = 1
                        ElseIf HigherFrequencyIndex Is Nothing Then
                            LowerFrequencyIndex = f.Length - 2
                            HigherFrequencyIndex = f.Length - 1
                        End If

                        'Checking if interpolation between frequencies is needed
                        If LowerFrequencyIndex = HigherFrequencyIndex Then

                            'No interpolation needed since the current frequency is in the f array
                            For CurrentSPL As Double = Lowest_Level To Highest_Level Step LevelResolution

                                Dim roundedSPL As Double = Math.Round(CurrentSPL, LevelDecimalPoints)

                                'Calculating Phon value
                                If LimitToAudibleRange = True Then
                                    SPLToPhonLookupTable(freq).Add(roundedSPL, Math.Max(0, GetSPLToPhon(roundedSPL, LowerFrequencyIndex))) 'N.B. LowerFrequencyIndex can be used since it is the same as HigherFrequencyIndex
                                Else
                                    SPLToPhonLookupTable(freq).Add(roundedSPL, GetSPLToPhon(roundedSPL, LowerFrequencyIndex)) 'N.B. LowerFrequencyIndex can be used since it is the same as HigherFrequencyIndex
                                End If

                            Next

                        Else

                            'Interpolation is needed
                            'Getting the lower and higher Phon values for interpolation
                            Dim LowerPhons As New SortedList(Of Double, Double)
                            Dim HigherPhons As New SortedList(Of Double, Double)

                            For CurrentSPL As Double = Lowest_Level To Highest_Level Step LevelResolution

                                Dim roundedSPL As Double = Math.Round(CurrentSPL, LevelDecimalPoints)

                                If LimitToAudibleRange = True Then
                                    'Calculating lower Phon values
                                    LowerPhons.Add(roundedSPL, Math.Max(0, GetSPLToPhon(roundedSPL, LowerFrequencyIndex)))

                                    'Calculating higher Phon values
                                    HigherPhons.Add(roundedSPL, Math.Max(0, GetSPLToPhon(roundedSPL, HigherFrequencyIndex)))

                                Else
                                    'Calculating lower Phon values
                                    LowerPhons.Add(roundedSPL, GetSPLToPhon(roundedSPL, LowerFrequencyIndex))

                                    'Calculating higher Phon values
                                    HigherPhons.Add(roundedSPL, GetSPLToPhon(roundedSPL, HigherFrequencyIndex))
                                End If
                            Next

                            'Interpolating the current frequency data (all SPL To Phons for the current frequency)
                            For CurrentSPL As Double = Lowest_Level To Highest_Level Step LevelResolution

                                Dim roundedSPL As Double = Math.Round(CurrentSPL, LevelDecimalPoints)
                                SPLToPhonLookupTable(freq).Add(roundedSPL, Utils.LinearInterpolation(freq, f(LowerFrequencyIndex), LowerPhons(roundedSPL), f(HigherFrequencyIndex), HigherPhons(roundedSPL), True))
                            Next
                        End If

                    Next


                End Sub

                ''' <summary>
                ''' Returns the Phon value of an input SPL, at the indicated frequency (Based on ISO 226).
                ''' </summary>
                ''' <param name="InputValue"></param>
                ''' <returns></returns>
                Private Function GetEqualLoudness(ByVal InputValue As Double, ByVal Frequency As Double) As Double

                    Dim RoundedSIL As Double = Math.Round(InputValue, LevelDecimalPoints) 'Rounded SPL is rounded to the number of level decimal point used in setting up the SPLToPhonLookupTable

                    'Checking if the value is lower than the minimum generated range
                    If RoundedSIL < Lowest_Level Then
                        RoundedSIL = Lowest_Level 'Setting it to the lowest generated value
                    End If

                    If RoundedSIL > Highest_Level Then RoundedSIL = Highest_Level 'Setting it to the highest generated value

                    'If SPLToPhonLookupTable(RoundedFrequency).ContainsKey(RoundedSIL) Then
                    Return SPLToPhonLookupTable(Frequency)(RoundedSIL)
                    'Else
                    'Throws an exception if no key larger enough has been generated
                    'Throw New Exception("The SPL value was outside the generated range. Please increase the Highest_Level set when setting up the SPLToPhonLookupTable!")
                    'End If

                End Function

                ''' <summary>
                ''' Returns the Phon value for the specified input SPL, at the frequency specified in f(FrequencyIndex).
                ''' </summary>
                ''' <param name="InputSIL"></param>
                ''' <param name="FrequencyIndex">The specified index in the public frequency array f.</param>
                ''' <returns></returns>
                Private Function GetSPLToPhon(ByVal InputSIL As Double, ByVal FrequencyIndex As Double) As Double

                    'If the function is fed by an SIL value that would fall below the SPL values genererated at zero phon, 
                    'the function returns the sum of zero phon minus the difference between the SIL value for zero phon and the input SIL value.
                    'The reason this procedure is used is that the PhonToSPL equation is not valid for (actually freaks out!) below 0 phons. 

                    Dim ZeroPhonValue As Double = SplAtZeroPhon(FrequencyIndex)
                    If InputSIL < ZeroPhonValue Then

                        Dim Lp_InputSILDifference As Double = ZeroPhonValue - InputSIL

                        'Returning 0 phon minus the Lp_InputSILDifference
                        Return -Lp_InputSILDifference

                    Else
                        Dim Lp As Double = InputSIL
                        Dim AAf As Double = 10 ^ ((af(FrequencyIndex) * (Lp + Lu(FrequencyIndex) - 94)) / 10)
                        Dim Ln As Double = (Math.Log10(((AAf - ((0.4 * 10 ^ (((Tf(FrequencyIndex) + Lu(FrequencyIndex)) / 10) - 9)) ^ af(FrequencyIndex))) / (4.47 * 10 ^ (-3))) + 1.14)) / 0.025

                        Return Ln
                    End If

                End Function

                ''' <summary>
                ''' Returns the SPL value for the specified input Phon value, at the frequency specified in f(FrequencyIndex).
                ''' </summary>
                ''' <param name="InputPhon"></param>
                ''' <param name="FrequencyIndex">The specified index in the public frequency array f.</param>
                ''' <returns></returns>
                Private Function GetPhonToSpl(ByVal InputPhon As Double, ByVal FrequencyIndex As Double) As Double

                    'Calculating Phon value
                    Dim Ln As Double = InputPhon
                    Dim AAf As Double = (4.47 * 10 ^ -3) * (10 ^ (0.025 * Ln) - 1.14) + (0.4 * 10 ^ ((Tf(FrequencyIndex) + Lu(FrequencyIndex)) / 10 - 9)) ^ af(FrequencyIndex)
                    Dim Lp As Double = ((10 / af(FrequencyIndex)) * Math.Log10(AAf)) - Lu(FrequencyIndex) + 94
                    Return Lp

                End Function

                ''' <summary>
                ''' Filters the power spectrum of a Sound using the current Iso-Phon filter.
                ''' </summary>
                ''' <param name="InputSound"></param>
                Public Sub FilterPowerSpectrum(ByRef InputSound As Sound, ByVal dbFSToSplDifference As Double)

                    Dim BinCount As Integer = InputSound.FFT.BinIndexToFrequencyList(, SkipNegativeFrequencies).Count

                    ''Averaging for testing reason
                    'For k = 0 To BinCount - 1
                    '    Dim BinList As New List(Of Double)
                    '    For TimeWindow = 15 To InputSound.FFT.windowCount(1) - 15
                    '        BinList.Add(InputSound.FFT.PowerSpectrumData(1, TimeWindow).WindowData(k))
                    '    Next

                    '    'Putting it in the first time widow to evaluate
                    '    InputSound.FFT.PowerSpectrumData(1, 0).WindowData(k) = BinList.Average
                    'Next


                    For channel = 1 To InputSound.FFT.ChannelCount
                        For TimeWindow = 0 To InputSound.FFT.WindowCount(channel) - 1

                            'Getting the total power of the current time window
                            InputSound.FFT.PowerSpectrumData(channel, TimeWindow).CalculateTotalPower()
                            Dim TotalPower As Double = InputSound.FFT.PowerSpectrumData(channel, TimeWindow).TotalPower


                            For k = 0 To BinCount - 1

                                'Converting values to dB scale, and shifts the Levels to SIL range
                                'Multiplying band BinCount to get the appropriate power to simulate an SIL level that can be used in the SplToPhon lookup table
                                Dim CurrentBandValue As Double = InputSound.FFT.PowerSpectrumData(channel, TimeWindow).WindowData(k)
                                Dim ThisBandProportionOfTotalPower As Double = CurrentBandValue / TotalPower
                                Dim LookupBandValue As Double = CurrentBandValue * ThisBandProportionOfTotalPower
                                Dim ValueIn_dBFS As Double = 10 * Math.Log10(LookupBandValue / InputSound.WaveFormat.PositiveFullScale)
                                Dim ValueIn_dBSIL As Double = (ValueIn_dBFS + dbFSToSplDifference)

                                'Tempcode
                                InputSound.FFT.PowerSpectrumData(channel, TimeWindow).WindowData(k) = ValueIn_dBSIL
                                'Dim LinearLoudness_Temp As Double = ReferenceSoundIntensityLevel * 10 ^ (ValueIn_dBSIL / 10)
                                'Dim ActuralBandValue_Temp As Double = LinearLoudness_Temp / BinCount
                                'InputSound.FFT.PowerSpectrumData(channel, TimeWindow).WindowData(k) = ActuralBandValue_Temp
                                Continue For

                                'Getting the loudness of the particular SIL and frequency combination
                                Dim EqualLoudnessValueInPhon As Double = GetEqualLoudness(ValueIn_dBSIL, InputSound.FFT.BinIndexToFrequencyList()(k))

                                'Leaving it in the SIL range and not taking -dbFSToSplDifference

                                'Shifting back to Linear scale (I= Ir * 10^(LI/10))
                                Dim LinearLoudness As Double = ReferenceSoundIntensityLevel * 10 ^ (EqualLoudnessValueInPhon / 10)

                                'Dividing by BinCount to get the actual band level
                                Dim ActualBandValue As Double = LinearLoudness / BinCount

                                'Storing the new value
                                InputSound.FFT.PowerSpectrumData(channel, TimeWindow).WindowData(k) = ActualBandValue

                            Next
                        Next
                    Next

                End Sub


                Public Sub ExportSplToPhonData(Optional ByVal OutputFolder As String = "", Optional ByVal FileName As String = "SplToPhonData")

                    If OutputFolder = "" Then OutputFolder = Utils.logFilePath

                    Dim OutputList As New List(Of String)

                    For InputLevel As Double = Lowest_Level To Highest_Level Step LevelResolution
                        Dim RoundedSIL As Double = Math.Round(InputLevel, LevelDecimalPoints) 'Rounded SPL is rounded to the number of level decimal point used in setting up the SPLToPhonLookupTable
                        For Each Frequency In SPLToPhonLookupTable
                            OutputList.Add(InputLevel & vbTab & Frequency.Key & vbTab & SPLToPhonLookupTable(Frequency.Key)(RoundedSIL))
                        Next
                    Next

                    SendInfoToAudioLog(vbCrLf & String.Join(vbCrLf, OutputList), FileName, OutputFolder)

                End Sub

                Public Sub ExportIsoPhonCurves(Optional ByVal OutputFolder As String = "", Optional ByVal FileName As String = "IsoPhoneData")

                    If OutputFolder = "" Then OutputFolder = Utils.logFilePath

                    Dim OutputList As New List(Of String)

                    OutputList.Add("Phon" & vbTab & "Frequency" & vbTab & "SPL (dB)")
                    For InputPhon As Double = -100 To 100 Step 10
                        For FrequencyIndex = 0 To f.Length - 1
                            OutputList.Add(InputPhon & vbTab & f(FrequencyIndex) & vbTab & GetPhonToSpl(InputPhon, FrequencyIndex))
                        Next
                    Next

                    SendInfoToAudioLog(vbCrLf & String.Join(vbCrLf, OutputList), FileName, OutputFolder)

                End Sub

                Public Sub ExportInverseIsoPhonCurves(Optional ByVal OutputFolder As String = "", Optional ByVal FileName As String = "InverseIsoPhonData")

                    If OutputFolder = "" Then OutputFolder = Utils.logFilePath

                    Dim OutputList As New List(Of String)

                    OutputList.Add("SPL" & vbTab & "Frequency" & vbTab & "Phon")
                    For InputSPL As Double = -100 To 100 Step 10
                        For FrequencyIndex = 0 To f.Length - 1
                            OutputList.Add(InputSPL & vbTab & f(FrequencyIndex) & vbTab & GetSPLToPhon(InputSPL, FrequencyIndex))
                        Next
                    Next

                    SendInfoToAudioLog(vbCrLf & String.Join(vbCrLf, OutputList), FileName, OutputFolder)

                End Sub


            End Class

#End Region


        End Module

        Public Class GammatoneFirFilterBank

            Public ReadOnly FilterKernels As List(Of Sound)
            Public ReadOnly CentreFrequencies As List(Of Double)
            Public ReadOnly Bandwidths As List(Of Double)
            Private _FilterFftFormat As New Formats.FftFormat(4096,,,, True)
            Private Shared FilterCreationFftFormat As New Formats.FftFormat(4096,,,, True)

            Public ReadOnly Property FilterFftFormat As Formats.FftFormat
                Get
                    Return _FilterFftFormat
                End Get
            End Property

            Public Sub New(ByVal WaveFormat As Formats.WaveFormat,
                       ByVal CentreFrequencies As List(Of Double),
                       Optional ByVal Phases As List(Of Double) = Nothing,
                       Optional ByVal FilterOrder As Integer = 4)

                'Checking some arguments
                If CentreFrequencies.Count < 1 Then Throw New ArgumentException("At least one centre frequency must be supplied!")
                If Phases Is Nothing Then
                    Phases = New List(Of Double)
                    For n = 0 To CentreFrequencies.Count - 1
                        Phases.Add(0)
                    Next
                Else
                    If Phases.Count <> CentreFrequencies.Count Then Throw New ArgumentException("The length of Phases must equal the length of CentreFrequencies")
                End If

                'Creating a mono wave format to use
                Dim TempWaveFormat As New Formats.WaveFormat(WaveFormat.SampleRate, WaveFormat.BitDepth, 1,,
                                                               WaveFormat.Encoding)

                'Storing the centre frequencies
                Me.CentreFrequencies = CentreFrequencies

                'Calculates the appropriate band width, as ERB times 1.019 (Cf. https://engineering.purdue.edu/~malcolm/apple/tr35/PattersonsEar.pdf)
                Bandwidths = New List(Of Double)
                For n = 0 To CentreFrequencies.Count - 1
                    Bandwidths.Add(CalculateGammatoneFilterBandWidth(CentreFrequencies(n)))
                Next

                'Creating filter kernels
                FilterKernels = New List(Of Sound)
                For n = 0 To CentreFrequencies.Count - 1
                    FilterKernels.Add(CreateGammatoneImpulseResponse(TempWaveFormat, 1,
                                                                 CentreFrequencies(n),
                                                                 Phases(n), 500,
                                                                 FilterOrder, Bandwidths(n), 0.1, 0.01,,
                                                                 True))
                Next

            End Sub

            Public Class FilteredSound
                Public Property Sound As Sound
                Public Property CentreFrequency As Double
                Public Property Bandwidth As Double
            End Class

            Public Function Filter(ByRef InputSound As Sound,
                               ByRef channel As Integer,
                               Optional ByVal KeepInputSoundLength As Boolean = True) As List(Of FilteredSound)

                Dim Output As New List(Of FilteredSound)
                For n = 0 To FilterKernels.Count - 1

                    Dim CurrentFilteredSound As New FilteredSound
                    CurrentFilteredSound.CentreFrequency = CentreFrequencies(n)
                    CurrentFilteredSound.Bandwidth = Bandwidths(n)
                    CurrentFilteredSound.Sound = DSP.FIRFilter(InputSound, FilterKernels(n),
                                                                 _FilterFftFormat, channel,,,,,,
                                                                 KeepInputSoundLength)
                    Output.Add(CurrentFilteredSound)
                Next

                Return Output

            End Function

            Public Class FilteredSoundLevels
                Inherits FilteredSound
                Public Property SoundLevel As Double
                Public Property Unit As SoundDataUnit
                Public Property Type As SoundMeasurementType
                Public Property FrequencyWeighting As FrequencyWeightings
            End Class

            Public Function GetFilteredSoundLevels(ByRef InputSound As Sound,
                                               ByRef Channel As Integer,
                                               Optional ByVal KeepInputSoundLength As Boolean = True,
                                               Optional ByVal OutputUnit As SoundDataUnit = SoundDataUnit.dB,
                                               Optional ByVal SoundMeasurementType As SoundMeasurementType = SoundMeasurementType.RMS,
                                               Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z) As List(Of FilteredSoundLevels)

                Dim FilteredSound = Filter(InputSound, Channel, KeepInputSoundLength)

                Dim Output As New List(Of FilteredSoundLevels)
                For n = 0 To FilteredSound.Count - 1

                    Dim NewFilteredSoundLevel As New FilteredSoundLevels
                    NewFilteredSoundLevel.CentreFrequency = FilteredSound(n).CentreFrequency
                    NewFilteredSoundLevel.Bandwidth = FilteredSound(n).Bandwidth
                    NewFilteredSoundLevel.Sound = FilteredSound(n).Sound

                    'Measuring and storing band level
                    NewFilteredSoundLevel.SoundLevel = DSP.MeasureSectionLevel(NewFilteredSoundLevel.Sound, Channel,,,
                                              OutputUnit, SoundMeasurementType, FrequencyWeighting)

                    NewFilteredSoundLevel.Unit = OutputUnit
                    NewFilteredSoundLevel.Type = SoundMeasurementType
                    NewFilteredSoundLevel.FrequencyWeighting = FrequencyWeighting

                    Output.Add(NewFilteredSoundLevel)
                Next

                Return Output

            End Function

            ''' <summary>
            ''' Exporting the filter kernels to wave files.
            ''' </summary>
            ''' <param name="OutputFolder"></param>
            Public Sub ExportKernels(Optional ByVal OutputFolder As String = "")

                If OutputFolder = "" Then OutputFolder = Utils.logFilePath

                For n = 0 To FilterKernels.Count - 1
                    FilterKernels(n).WriteWaveFile(IO.Path.Combine(OutputFolder, "GammatoneFirKernel_fc_" & CentreFrequencies(n) & "_bw_" & Bandwidths(n) & ".wav"))
                Next

            End Sub

            ''' <summary>
            ''' Exporting the information about the filter bank.
            ''' </summary>
            ''' <param name="OutputFolder"></param>
            Public Sub ExportFilterDescription(Optional ByVal OutputFolder As String = "")

                If OutputFolder = "" Then OutputFolder = Utils.logFilePath

                Dim OutputList As New List(Of String)
                OutputList.Add("Gammatone FIR filterbank info")
                OutputList.Add("Filter nr." & vbTab & "CentreFrequency" & vbTab & "Bandwidth")
                For n = 0 To FilterKernels.Count - 1
                    OutputList.Add(n & vbTab & CentreFrequencies(n) & vbTab & Bandwidths(n))
                Next

                Utils.SendInfoToLog(String.Join(vbCrLf, OutputList), "GammatoneFilterbankInfo", OutputFolder)

            End Sub


            Public Shared Function CreateGammatoneImpulseResponse(ByRef format As Formats.WaveFormat,
                                                       Optional ByVal Channel As Integer? = Nothing,
                                                       Optional ByVal CentreFrequency As Double = 1000,
                                                       Optional ByVal Phase As Double = 0,
                                                       Optional ByVal Amplitude As Double = 500,
                                                       Optional ByVal FilterOrder As Integer = 4,
                                                       Optional ByVal BandWidth As Double = 132.6,
                                                       Optional ByVal Duration As Double = 0.1,
                                                       Optional ByVal FadeOutDuration As Double = 0.01,
                                                       Optional ByVal DurationTimeUnit As TimeUnits = TimeUnits.seconds,
                                                       Optional ByVal ZeroPhaseKernel As Boolean = True) As Sound
                Try

                    If FilterCreationFftFormat Is Nothing Then
                        FilterCreationFftFormat = New Formats.FftFormat(4096,,,, True)
                    End If

                    If format.Channels > 1 Then Throw New NotImplementedException("Only mono sound format is implemented in CreateGammatoneImpulseResponse.")

                    Dim outputSound As New Sound(format)
                    Dim AudioOutputConstructor As New AudioOutputConstructor(format, Channel)

                    Dim dataLength As Long = 0
                    Select Case DurationTimeUnit
                        Case TimeUnits.seconds
                            dataLength = Duration * format.SampleRate
                        Case TimeUnits.samples
                            dataLength = Duration
                    End Select

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim channelArray(dataLength - 1) As Single

                        Select Case format.BitDepth
                            Case 16, 32, 64 'Actually used for formats that use signed datatypes
                                For s = 0 To channelArray.Length - 1

                                    Dim t As Double = s / format.SampleRate

                                    channelArray(s) = ((Amplitude * t) ^ (FilterOrder - 1)) * (Math.E ^ (-twopi * BandWidth * t)) * Math.Cos(twopi * CentreFrequency * t + Phase)

                                Next
                            Case Else
                                Throw New NotImplementedException(format.BitDepth & " bit depth Is Not yet supported.")

                        End Select

                        outputSound.WaveData.SampleData(c) = channelArray

                    Next

                    'Fading the end
                    If FadeOutDuration > 0 Then
                        Dim FadeStartSample As Integer = Math.Min(outputSound.WaveData.SampleData(1).Length - 1, outputSound.WaveFormat.SampleRate * FadeOutDuration)
                        DSP.Fade(outputSound, 0,,, FadeStartSample)
                    End If

                    'Scaling to zero gain at f by probing with a sinusoid
                    Dim PrePeaklevel As Double = -6
                    Dim ProbeSignal = GenerateSound.CreateSineWave(format,, CentreFrequency, PrePeaklevel, SoundDataUnit.dB, 10)
                    Dim Prelevel = DSP.MeasureSectionLevel(ProbeSignal, 1)

                    'Filtering the probe and measures the level and calculates the filter gain at f
                    Dim FilteredProbeSignal = DSP.FIRFilter(ProbeSignal, outputSound, FilterCreationFftFormat)
                    DSP.CropSection(FilteredProbeSignal, FilterCreationFftFormat.AnalysisWindowSize, FilteredProbeSignal.WaveData.SampleData(1).Length - 4 * FilterCreationFftFormat.AnalysisWindowSize)
                    Dim PostLevel = DSP.MeasureSectionLevel(FilteredProbeSignal, 1)
                    Dim CurrentGain = PostLevel - Prelevel

                    'Applying minus gain to the impulse response
                    DSP.AmplifySection(outputSound, -CurrentGain)

                    'Converting to a centrailzed zero-phase filter kernel 
                    If ZeroPhaseKernel = True Then
                        Dim FftFormat2 = New Formats.FftFormat(4096,,,, True)
                        outputSound = GenerateSound.GetImpulseResponseFromSound(outputSound, FftFormat2,
                                                                    FftFormat2.FftWindowSize,,,, True, True)
                    End If

                    'Repeating one more time for increased accuracy (avoiding rounding errors)

                    Dim ProbeSignal_B = GenerateSound.CreateSineWave(format,, CentreFrequency, PrePeaklevel, SoundDataUnit.dB, 10)
                    Dim PreLevel_B = DSP.MeasureSectionLevel(ProbeSignal_B, 1)

                    'Filtering the probe and measures the level and calculates the filter gain at f
                    Dim FilteredProbeSignal_B = DSP.FIRFilter(ProbeSignal_B, outputSound, FilterCreationFftFormat)
                    DSP.CropSection(FilteredProbeSignal_B, FilterCreationFftFormat.AnalysisWindowSize, FilteredProbeSignal_B.WaveData.SampleData(1).Length - 4 * FilterCreationFftFormat.AnalysisWindowSize)
                    Dim PostLevel_B = DSP.MeasureSectionLevel(FilteredProbeSignal_B, 1)
                    Dim CurrentGain_B = PostLevel_B - PreLevel_B

                    'Applying minus gain to the impulse response
                    DSP.AmplifySection(outputSound, -CurrentGain_B)

                    Return outputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function

            Public Shared Function CalculateAdjacentCentreFrequencies(ByVal LowestFrequency As Double,
                                                                  ByVal HighestFrequency As Double,
                                                                  Optional ByVal Round As Boolean = True,
                                                                  Optional ByVal ForceInclusionOfHighestFrequency As Boolean = False) As List(Of Double)

                Dim Output = New List(Of Double)
                Dim NextCentreFrequency As Double = LowestFrequency

                Dim LastBandwidth As Double
                Do
                    If NextCentreFrequency > HighestFrequency Then
                        Exit Do
                    End If

                    Output.Add(NextCentreFrequency)

                    LastBandwidth = 1.019 * (24.7 * (4.37 * (NextCentreFrequency / 1000) + 1))
                    NextCentreFrequency += LastBandwidth * 1.058195

                Loop

                If ForceInclusionOfHighestFrequency = True Then
                    If Not Output.Contains(HighestFrequency) Then Output.Add(HighestFrequency)
                End If

                If Round = True Then
                    For n = 0 To Output.Count - 1
                        Output(n) = Math.Round(Output(n))
                    Next
                End If

                Return Output

            End Function

            Public Function CalculateGammatoneFilterBandWidth(ByVal CentreFrequency As Double) As Double
                Return 1.019 * (24.7 * (4.37 * (CentreFrequency / 1000) + 1))
            End Function


        End Class

    End Namespace

    Namespace GenerateSound

        Public Module SignalsExt





            ''' <summary>
            ''' Creates sound containing a frequency modulated (or more correctly phase moduated) sine wave.
            ''' </summary>
            ''' <param name="channel"></param>
            ''' <param name="CarrierFrequency"></param>
            ''' <param name="CarrierIntensity"></param>
            ''' <param name="ModulationDepth">Modulating depth as a proportion of the carrier frequency.</param>
            ''' <param name="intensityUnit"></param>
            ''' <returns></returns>
            Public Function CreateFrequencyModulatedSound(ByVal ModulationSound As Sound, Optional ByVal channel As Integer? = Nothing,
                                                         Optional ByVal CarrierFrequency As Double = 1000, Optional ByVal CarrierIntensity As Decimal = 0.5,
                                                         Optional ByVal ModulationDepth As Double = 0.1,
                                                         Optional intensityUnit As SoundDataUnit = SoundDataUnit.unity) As Sound

                'FM modulation 
                '(Source: Sinusoidal Modulation of Sinusoids, JULIUS O. SMITH III. https://ccrma.stanford.edu/~jos/rbeats/Sinusoidal_Frequency_Modulation_FM.html)
                'x(t) = Ac *Cos [wc*t + fc + Am*Sin(wmt +fm)]
                '
                't=time
                'Ac=Amplitude of carrier
                'wc=frequency of carrier
                'fc= phase of carrier
                '
                'Am=Amplitude of modulator
                'wm=frequency of modulator
                'fm= phase of modulator

                'And here's another useful link: http://www.rfcafe.com/references/electrical/frequency-modulation.htm

                Try

                    Dim outputSound = CreateSineWave(ModulationSound.WaveFormat, , CarrierFrequency, CarrierIntensity, intensityUnit, ModulationSound.WaveData.SampleData(1).Length, TimeUnits.samples)

                    'Calculating the maximum frequency deviation
                    Dim MaximumFrequencyDeviation As Double = CarrierFrequency * ModulationDepth

                    'Main section
                    Dim CurrentFrequency As Double = 0
                    For c = 1 To outputSound.WaveFormat.Channels

                        Dim channelArray(ModulationSound.WaveData.SampleData(c).Length - 1) As Single

                        Select Case outputSound.WaveFormat.BitDepth
                            Case 8 'Actually used for formats that use unsigned datatypes
                                For n = 0 To channelArray.Length - 1

                                    channelArray(n) = (CarrierIntensity * (outputSound.WaveFormat.PositiveFullScale / 2)) * Math.Sin(twopi * (CurrentFrequency / outputSound.WaveFormat.SampleRate) * n) + outputSound.WaveFormat.PositiveFullScale / 2 ' - _

                                Next

                            Case 16, 32, 64 'Actually used for formats that use signed datatypes
                                For n = 0 To channelArray.Length - 1

                                    'Intuitively for carrier frequency:
                                    'channelArray(n) = (intensity * format.PositiveFullScale) * Math.Sin("One lap" * "number of laps in a second" * "the current time (in seconds)")
                                    'channelArray(n) = (intensity * format.PositiveFullScale) * Math.Sin(twopi * CurrentFrequency * (n / format.SampleRate)) ' - _


                                    'Applying modulation by varying the phase (Actually I did this by trial and error, I'm not sure why the factor (twopi*Pi*0.1) is needed (+/-0.1 is the modulator range))
                                    channelArray(n) = (CarrierIntensity * outputSound.WaveFormat.PositiveFullScale) * Math.Sin(twopi * CarrierFrequency * (n / outputSound.WaveFormat.SampleRate) - (ModulationDepth * ModulationSound.WaveData.SampleData(c)(n)))


                                Next
                            Case Else
                                Throw New NotImplementedException(outputSound.WaveFormat.BitDepth & " bit depth Is Not yet supported.")

                        End Select

                        outputSound.WaveData.SampleData(c) = channelArray

                    Next

                    Return outputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function


            Public Function CreateTriangularWave(ByRef format As Formats.WaveFormat, Optional ByVal channel As Integer? = Nothing,
                                    Optional ByVal freq As Double = 1000, Optional ByVal level As Decimal = 1, Optional ByVal duration As Double = 1,
                                         Optional durationTimeUnit As TimeUnits = TimeUnits.seconds,
                                             Optional Type As TriangularWaveTypes = TriangularWaveTypes.FullRange)
                Try

                    If level > 1 Then
                        level = 1
                        MsgBox("Level was outside allowed value (-1 through 1)" & vbCr & vbCr & "The level was adjusted To 1", "Waring from createSineWave")
                    End If

                    If level < -1 Then
                        level = -1
                        MsgBox("Level was outside allowed value (-1 through 1)" & vbCr & vbCr & "The level was adjusted To -1", "Waring from createSineWave")
                    End If

                    Dim outputSound As New Sound(format)
                    Dim AudioOutputConstructor As New AudioOutputConstructor(format, channel)

                    Dim posFS As Double = format.PositiveFullScale

                    Dim dataLength As Long = 0
                    Select Case durationTimeUnit
                        Case TimeUnits.seconds
                            dataLength = duration * format.SampleRate
                        Case TimeUnits.samples
                            dataLength = duration
                    End Select

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim channelArray(dataLength - 1) As Single

                        Select Case format.BitDepth

                            Case 16
                                Select Case Type 'N.B. There should acctually be two more options here InversePositiveHalfRange and InverseNegativeHalfRange
                                    Case TriangularWaveTypes.FullRange
                                        For n = 0 To channelArray.Length - 1
                                            channelArray(n) = level * (posFS - (((2 * freq * posFS) / format.SampleRate) * (n Mod (format.SampleRate / freq))))

                                        Next
                                    Case TriangularWaveTypes.InverseFullRange
                                        For n = 0 To channelArray.Length - 1
                                            channelArray(n) = level * (-posFS + (((2 * freq * posFS) / format.SampleRate) * (n Mod (format.SampleRate / freq))))

                                        Next
                                    Case TriangularWaveTypes.PositiveHalfRange
                                        For n = 0 To channelArray.Length - 1
                                            channelArray(n) = level * (posFS - (((freq * posFS) / format.SampleRate) * (n Mod (format.SampleRate / freq))))

                                        Next
                                    Case TriangularWaveTypes.NegativeHalfRange
                                        For n = 0 To channelArray.Length - 1
                                            channelArray(n) = level * (-posFS + (((freq * posFS) / format.SampleRate) * (n Mod (format.SampleRate / freq))))

                                        Next
                                End Select
                            Case Else
                                Throw New NotImplementedException(format.BitDepth & " bit depth Is Not yet supported.")

                        End Select

                        outputSound.WaveData.SampleData(c) = channelArray

                    Next

                    Return outputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function


            ''' <summary>
            ''' Creates a sweep signal to be used for IR extraction. The output Tuple.Item1 is the signal that should be played and recorded, and Item2 is the signal that sound be used for IR extraction.
            ''' </summary>
            ''' <param name="Format"></param>
            ''' <param name="MeasurementDuration"></param>
            ''' <param name="StartFrequency"></param>
            ''' <param name="ArrivalFrequency"></param>
            ''' <param name="SegmentationPulseTime"></param>
            ''' <param name="SignalDelay"></param>
            ''' <returns></returns>
            Public Function CreateIRMeasurementSignal(ByRef Format As Formats.WaveFormat,
                                                  Optional ByVal MeasurementDuration As Double = 30,
                                                  Optional ByVal StartFrequency As Double = 20,
                                                  Optional ByVal ArrivalFrequency As Double = 20000,
                                                  Optional ByVal SegmentationPulseTime As Double = 10,
                                                  Optional ByVal SignalDelay As Double = 10,
                                                  Optional ByVal FlattenProcessingSoundSpectrum As Boolean = True) As Tuple(Of Sound, Sound)

                'Creating a log sine sweep, to be used for the IR extraction
                Dim OriginalSignal As Sound = CreateLogSineSweep(Format,, StartFrequency, ArrivalFrequency,, 0.5,, MeasurementDuration)

                'Creates a padded copy to be used for the actual measurement
                Dim PaddedSignal As Sound = OriginalSignal.CreateCopy

                'Filterring the original signal to get a flat spectrum (equivalent to the post-measurement processing of the measured sound)
                OriginalSignal = PostProcessMeasuredIRSweep(OriginalSignal,, StartFrequency, ArrivalFrequency, Nothing, 0, 0,, 0,, FlattenProcessingSoundSpectrum)

                'Padds the sine sweep with SignalDelay seconds
                PaddedSignal.ZeroPad(CInt(Format.SampleRate * (SegmentationPulseTime + SignalDelay)), CInt(0), False)

                'Adds a dirac pulse (with height 0.9) at time DiracPulseTime seconds
                For c = 1 To PaddedSignal.WaveFormat.Channels
                    PaddedSignal.WaveData.SampleData(c)(Format.SampleRate * SegmentationPulseTime) = 0.9
                Next

                Dim OutputTuple As New Tuple(Of Sound, Sound)(PaddedSignal, OriginalSignal)

                Return OutputTuple

            End Function


            Public Function PostProcessMeasuredIRSweep(ByRef MeasuredSignal As Sound,
                                                    Optional ByVal OriginalSignalDuration As Double = 30,
                                                    Optional ByVal StartFrequency As Double = 20,
                                                    Optional ByVal ArrivalFrequency As Double = 20000,
                                                    Optional ByVal ApproximateSegmentationPulseTime As Double? = 10,
                                                    Optional ByVal SegmentationPulseSearchRegionDuration As Double = 4,
                                                    Optional ByVal SegmentationImpulseDistance As Double = 0.05, 'In meter
                                                    Optional ByVal SpeedOfSound As Double = 343, 'In m/s
                                                    Optional ByVal SignalDelay As Double = 10,
                                                    Optional ByVal SpeakerImpulseResponse As Sound = Nothing,
                                                    Optional ByVal FlattenSpectrum As Boolean = True,
                                                    Optional ByVal ExportSegmentedSignal As Boolean = False,
                                                    Optional ByVal ExportFilteredSignal As Boolean = False) As Sound

                'The signal is only segmented if ApproximateSegmentationPulseTime is not nothing
                Dim SegmentedMeasuredSignal As Sound = Nothing
                If ApproximateSegmentationPulseTime IsNot Nothing Then

                    'Looking for the dirac pulse to auto-segment the input sound
                    Dim StartSearchSample As Integer = Math.Min(MeasuredSignal.WaveData.SampleData(1).Length - 1, Math.Max((ApproximateSegmentationPulseTime.Value - (SegmentationPulseSearchRegionDuration / 2)) * MeasuredSignal.WaveFormat.SampleRate, 0))
                    Dim StopSearchSample As Integer = Math.Min(MeasuredSignal.WaveData.SampleData(1).Length - 1, Math.Max((ApproximateSegmentationPulseTime.Value + (SegmentationPulseSearchRegionDuration / 2)) * MeasuredSignal.WaveFormat.SampleRate, 0))

                    'Locating the pulse by getting the sample with highest absolute amplitude (using channel 1 data, even if other channels exist)
                    Dim MaxValue As Double = 0
                    Dim MaxValueIndex As Integer = 0
                    For s = StartSearchSample To StopSearchSample
                        If Math.Abs(MeasuredSignal.WaveData.SampleData(1)(s)) > MaxValue Then
                            MaxValue = Math.Abs(MeasuredSignal.WaveData.SampleData(1)(s))
                            MaxValueIndex = s
                        End If
                    Next

                    'Calculating the time delay caused by the speaker-microphone distance at the time of segmentation impulse
                    Dim DistanceDelay As Double = SegmentationImpulseDistance / SpeedOfSound
                    Dim DelayLength As Integer = DistanceDelay * MeasuredSignal.WaveFormat.SampleRate

                    'Segmenting out only the IR-signal
                    Dim SignalStartSample As Integer = Math.Max(0, MaxValueIndex + (SignalDelay * MeasuredSignal.WaveFormat.SampleRate) + DelayLength) '? Should we add or subtract the DelayLength?
                    'Dim SignalStopSample As Integer = Math.Min(MeasuredSignal.WaveData.SampleData(1).Length - 1, SignalStartSample + (OriginalSignalDuration + IntendedIRDuration) * MeasuredSignal.WaveFormat.SampleRate)
                    Dim SignalStopSample As Integer = Math.Min(MeasuredSignal.WaveData.SampleData(1).Length - 1, SignalStartSample + (OriginalSignalDuration * MeasuredSignal.WaveFormat.SampleRate))
                    Dim SignalLength As Integer = SignalStopSample - SignalStartSample

                    SegmentedMeasuredSignal = DSP.CopySection(MeasuredSignal, SignalStartSample, SignalLength)

                Else
                    SegmentedMeasuredSignal = MeasuredSignal.CreateCopy

                    ''Adds post pading (equivalent to reverberation after the last frequency components)
                    'SegmentedMeasuredSignal.ZeroPad(CInt(0), CInt(IntendedIRDuration * MeasuredSignal.WaveFormat.SampleRate), False)

                End If

                'De-convoluting the signal using the speaker IR
                'Reversing the signal
                If SpeakerImpulseResponse IsNot Nothing Then
                    'Measuring level pre filter (on channel 1, and then apply the same gain to all channels)
                    Dim PreLevel As Double = DSP.MeasureSectionLevel(SegmentedMeasuredSignal, 1,,,,, FrequencyWeightings.C)
                    Dim DeconvolutedSound = DSP.FIRFilter_NEW(SegmentedMeasuredSignal, SpeakerImpulseResponse,,,,,,,, True)
                    DSP.MeasureAndAdjustSectionLevel(DeconvolutedSound, PreLevel,,,, FrequencyWeightings.C)
                    SegmentedMeasuredSignal = DeconvolutedSound
                End If


                If ExportSegmentedSignal = True Then AudioIOs.SaveToWaveFile(SegmentedMeasuredSignal, AudioIOs.SaveSoundFileDialog(, "SegmentedIRSweep"))

                If FlattenSpectrum = True Then

                    'Filterring the signal to get at flat spectrum
                    'Assumed the signal is created using the formula from Picinali, Lorenzo. "Techniques for the extraction of the impulse response of a linear and time-invariant system." 
                    'x(t) = sin(((TwoPi*StartFreq * T)*(Ln((TwoPi*ArrivalFreq)/(TwoPi*StartFreq)) *(e ^((t/T)* ln((TwoPi*ArrivalFreq)/(TwoPi*StartFreq)))-1))
                    Dim CurrentTime As Double
                    Dim LnExpression As Double = Math.Log(ArrivalFrequency / StartFrequency) 'Simplification of: Math.Log((twopi * ArrivalFrequency) / (twopi * StartFrequency))
                    Dim Factor1 As Double = ((StartFrequency * OriginalSignalDuration) / LnExpression)
                    Dim Exponent1 As Double

                    Dim Normalizer As Double = 1 / Math.Exp(LnExpression / 2)
                    Dim OriginalSignalLength As Integer = OriginalSignalDuration * SegmentedMeasuredSignal.WaveFormat.SampleRate
                    For c = 1 To SegmentedMeasuredSignal.WaveFormat.Channels
                        For s = 0 To OriginalSignalLength - 1

                            CurrentTime = s / SegmentedMeasuredSignal.WaveFormat.SampleRate
                            Exponent1 = (CurrentTime / OriginalSignalDuration) * LnExpression
                            SegmentedMeasuredSignal.WaveData.SampleData(c)(s) *= (Math.Exp(Exponent1 / 2) * Normalizer) 'The flat spectrum level factor in this equation was created (by EW) much by trial and error...

                        Next
                    Next

                    ''Also filters the last bit (only reverberation segment) by the same amount as the highest included frequency (by just replacing s (with the last original signal sample) in the formula above)
                    'For c = 1 To SegmentedMeasuredSignal.WaveFormat.Channels
                    '    For s = OriginalSignalLength To SegmentedMeasuredSignal.WaveData.SampleData(c).Length - 1

                    '        CurrentTime = (OriginalSignalLength - 1) / SegmentedMeasuredSignal.WaveFormat.SampleRate
                    '        Exponent1 = (CurrentTime / OriginalSignalDuration) * LnExpression
                    '        SegmentedMeasuredSignal.WaveData.SampleData(c)(s) *= (Math.Exp(Exponent1 / 2) * Normalizer) 'The flat spectrum level factor in this equation was created (by EW) much by trial and error...

                    '    Next
                    'Next

                    If ExportFilteredSignal = True Then
                        Dim SavePath = AudioIOs.SaveSoundFileDialog(, "FilteredIRSweep")
                        AudioIOs.SaveToWaveFile(SegmentedMeasuredSignal, SavePath)
                    End If

                End If


                Return SegmentedMeasuredSignal

            End Function


            ''' <summary>
            ''' Creates a log sine sweep.
            ''' </summary>
            ''' <param name="format"></param>
            ''' <param name="channel"></param>
            ''' <param name="StartFrequency"></param>
            ''' <param name="ArrivalFrequency"></param>
            ''' <param name="intensity"></param>
            ''' <param name="intensityUnit"></param>
            ''' <param name="TotalDuration"></param>
            ''' <param name="durationTimeUnit"></param>
            ''' <returns></returns>
            Public Function CreateLogSineSweep(ByRef format As Formats.WaveFormat, Optional ByVal channel As Integer? = Nothing,
                                           Optional ByVal StartFrequency As Double = 20, Optional ByVal ArrivalFrequency As Double = 20000,
                                           Optional ByVal FlatSpectrum As Boolean = False,
                                           Optional ByVal intensity As Decimal = 1, Optional intensityUnit As SoundDataUnit = SoundDataUnit.unity,
                                           Optional ByVal TotalDuration As Double = 1, Optional durationTimeUnit As TimeUnits = TimeUnits.seconds) As Sound

                Try

                    Dim outputSound As New Sound(format)
                    Dim AudioOutputConstructor As New AudioOutputConstructor(format, channel)

                    'Converting intensity values
                    Select Case intensityUnit
                        Case SoundDataUnit.unity
                            'no conversion is needed (since the signal generation words in unity scale)

                        Case SoundDataUnit.dB
                            'convert to linear
                            intensity = dBConversion(intensity, dBConversionDirection.from_dB, outputSound.WaveFormat)

                            'convering to unity
                            intensity = intensity / format.PositiveFullScale

                        Case SoundDataUnit.linear
                            'convering to unity
                            intensity = intensity / format.PositiveFullScale

                    End Select

                    Dim dataLength As Long = 0
                    Select Case durationTimeUnit
                        Case TimeUnits.seconds
                            dataLength = TotalDuration * format.SampleRate
                        Case TimeUnits.samples
                            dataLength = TotalDuration
                    End Select

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim channelArray(dataLength - 1) As Single

                        Select Case format.BitDepth
                            Case 16, 32, 64 'Should be used for formats that use signed datatypes

                                'Formula from Picinali, Lorenzo. "Techniques for the extraction of the impulse response of a linear and time-invariant system." 
                                'x(t) = sin(((TwoPi*StartFreq * T)*(Ln((TwoPi*ArrivalFreq)/(TwoPi*StartFreq)) *(e ^((t/T)* ln((TwoPi*ArrivalFreq)/(TwoPi*StartFreq)))-1))
                                Dim CurrentTime As Double
                                Dim LnExpression As Double = Math.Log(ArrivalFrequency / StartFrequency) 'Simplification of: Math.Log((twopi * ArrivalFrequency) / (twopi * StartFrequency))
                                Dim Factor1 As Double = ((StartFrequency * TotalDuration) / LnExpression)
                                Dim LocalIntensity As Double = (intensity * format.PositiveFullScale)
                                Dim Exponent1 As Double

                                If FlatSpectrum = True Then

                                    Dim Normalizer As Double = 1 / Math.Exp(LnExpression / 2)

                                    For s = 0 To channelArray.Length - 1

                                        CurrentTime = s / format.SampleRate
                                        Exponent1 = (CurrentTime / TotalDuration) * LnExpression
                                        channelArray(s) = LocalIntensity * Math.Sin(twopi * Factor1 * (Math.Exp(Exponent1) - 1)) * (Math.Exp(Exponent1 / 2) * Normalizer) 'The flat spectrum level factor in this equation was created (by EW) much by trial and error...

                                    Next

                                Else
                                    For s = 0 To channelArray.Length - 1

                                        CurrentTime = s / format.SampleRate
                                        Exponent1 = (CurrentTime / TotalDuration) * LnExpression
                                        channelArray(s) = LocalIntensity * Math.Sin(twopi * Factor1 * (Math.Exp(Exponent1) - 1))
                                    Next

                                End If

                            Case Else
                                Throw New NotImplementedException(format.BitDepth & " bit depth Is Not yet supported.")

                        End Select

                        outputSound.WaveData.SampleData(c) = channelArray

                    Next

                    Return outputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function




            Public Function CreatePeriodicRandomNoise(ByRef format As Formats.WaveFormat, Optional ByVal channel As Integer? = Nothing,
                                    Optional ByVal PeriodLength As Integer = 1024, Optional ByVal intensity As Decimal = 1,
                                       Optional intensityUnit As SoundDataUnit = SoundDataUnit.unity,
                                       Optional ByVal duration As Double = 1, Optional durationTimeUnit As TimeUnits = TimeUnits.seconds) As Sound
                Try


                    Dim outputSound As New Sound(format)
                    Dim AudioOutputConstructor As New AudioOutputConstructor(format, channel)


                    'Converting intensity values
                    Select Case intensityUnit
                        Case SoundDataUnit.unity
                            'no conversion is needed (since the signal generation words in unity scale)

                        Case SoundDataUnit.dB
                            'convert to linear
                            intensity = dBConversion(intensity, dBConversionDirection.from_dB, outputSound.WaveFormat)

                            'convering to unity
                            intensity = intensity / format.PositiveFullScale

                        Case SoundDataUnit.linear
                            'convering to unity
                            intensity = intensity / format.PositiveFullScale

                    End Select

                    Dim dataLength As Long = 0
                    Select Case durationTimeUnit
                        Case TimeUnits.seconds
                            dataLength = duration * format.SampleRate
                        Case TimeUnits.samples
                            dataLength = duration
                    End Select

                    'Calculates the number of full periods
                    Dim NumberOfPeriods As Integer = Utils.Rounding(dataLength / PeriodLength, Utils.roundingMethods.alwaysUp)

                    'Calculating period lengths to create
                    Dim Frequencies As New List(Of Double)
                    For n = 1 To PeriodLength
                        Frequencies.Add(n)
                    Next

                    'Utils.SendInfoToLog(String.Join(vbCrLf, PeriodLengths))

                    Dim rnd As New Random

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim channelArray(NumberOfPeriods * PeriodLength - 1) As Single

                        Select Case format.BitDepth
                            Case 16, 32, 64 'Actually used for formats that use signed datatypes

                                For Each CurrentPeriodLength In Frequencies

                                    Dim Phase As Integer = rnd.Next(0, PeriodLength)

                                    For n = 0 To channelArray.Length - 1
                                        channelArray(n) += ((intensity * format.PositiveFullScale) / Frequencies.Count) * Math.Sin(twopi * (1 / CurrentPeriodLength) * (n + Phase))
                                    Next


                                    'For n = 0 To channelArray.Length - 1
                                    '    channelArray(n) += (intensity * format.PositiveFullScale) * Math.Sin(twopi * (1 / CurrentPeriodLength) * n)
                                    'Next

                                Next

                            Case Else
                                Throw New NotImplementedException(format.BitDepth & " bit depth Is Not yet supported.")

                        End Select

                        'Shortens the sound to the data output length
                        ReDim Preserve channelArray(dataLength - 1)

                        outputSound.WaveData.SampleData(c) = channelArray

                    Next

                    Return outputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function


            ''' <summary>
            ''' Creates Gaussian White noise. (A noise with a flat spectrum, and a normal distribution of samples.)
            ''' </summary>
            ''' <param name="format"></param>
            ''' <param name="channel"></param>
            ''' <param name="level"></param>
            ''' <param name="duration"></param>
            ''' <param name="durationTimeUnit"></param>
            ''' <param name="StandardDeviationFactor">The factor multiplied to by the wave format full scale to set as standard deviation of the normal sample distrubution.</param>
            ''' <param name="Mean">The mean of the normal sample distribution. Leave at 0 to get a DC componant of 0.</param>
            ''' <returns></returns>
            Public Function CreateWhiteGaussianNoise(ByRef format As Formats.WaveFormat, Optional ByVal channel As Integer? = Nothing,
                                    Optional ByVal level As Double = 1, Optional ByVal duration As Double = 1,
                                         Optional durationTimeUnit As TimeUnits = TimeUnits.seconds,
                                                 Optional ByVal StandardDeviationFactor As Single = 0.1,
                                                 Optional ByVal Mean As Single = 0,
                                                 Optional ByRef RandomSource As Random = Nothing) As Sound

                Try

                    If level > 1 Then
                        level = 1
                        MsgBox("Level was outside allowed value (-1 through 1)" & vbCr & vbCr & "The level was adjusted To 1", "Waring from createSineWave")
                    End If

                    If level < -1 Then
                        level = -1
                        MsgBox("Level was outside allowed value (-1 through 1)" & vbCr & vbCr & "The level was adjusted To -1", "Waring from createSineWave")
                    End If

                    Dim outputSound As New Sound(format)
                    Dim AudioOutputConstructor As New AudioOutputConstructor(format, channel)

                    Dim posFS As Double = format.PositiveFullScale
                    Dim negFS As Double = format.NegativeFullScale

                    Dim dataLength As Long = 0
                    Select Case durationTimeUnit
                        Case TimeUnits.seconds
                            dataLength = duration * format.SampleRate
                        Case TimeUnits.samples
                            dataLength = duration
                    End Select

                    If RandomSource Is Nothing Then
                        Thread.Sleep(20) ' The reason for sleeping the thread is to avoid the same random seed on multiple close calls. (Due to the limited resolution of the system clock.)
                        RandomSource = New Random()
                    End If

                    Dim t As New MathNet.Numerics.Distributions.Normal(0, format.PositiveFullScale * StandardDeviationFactor, RandomSource)

                    'Main section
                    Select Case format.Encoding
                        Case Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints

                            For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                                Dim channelArray(dataLength - 1) As Single

                                Select Case format.BitDepth
                                    Case 32

                                        For n = 0 To channelArray.Length - 1
                                            channelArray(n) = level * t.Sample
                                        Next

                                    Case Else
                                        Throw New NotImplementedException

                                End Select

                                outputSound.WaveData.SampleData(c) = channelArray

                            Next


                        Case Formats.WaveFormat.WaveFormatEncodings.PCM


                            For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                                Dim channelArray(dataLength - 1) As Single

                                Select Case format.BitDepth
                                    Case 16

                                        For n = 0 To channelArray.Length - 1
                                            channelArray(n) = level * t.Sample
                                        Next

                                End Select

                                outputSound.WaveData.SampleData(c) = channelArray

                            Next

                    End Select

                    Return outputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function




            Public Function CreateSpecialTypeImpulseResponse(ByRef waveFormat As Formats.WaveFormat,
                                                      ByRef fftFormat As Formats.FftFormat,
                                                      ByVal kernelSize As Integer,
                                                      Optional ByVal channel As Integer? = Nothing,
                                                         Optional ByVal IRType As FilterType = FilterType.LowPass,
                                                      Optional ByRef CutOffFreq1 As Single = 1000,
                                                      Optional ByRef CutOffFreq2 As Single = 2000,
                                                      Optional ByVal StopBandLevel_dB As Single = -6,
                                                      Optional ByVal passBandLevel_dB As Single = 0,
                                                         Optional ByVal attenuationRate As Single = 6,
            Optional ByVal windowFunction As WindowingType = WindowingType.Hamming,
                                                         Optional InActivateWarnings As Boolean = False) As Sound

                'The cut-off frequencies are modified (depending on the fftSize) by this sub and sent back, in the same variables

                'Reference which parts of this code is based on:
                'The Scientist And Engineer's Guide to
                'Digital Signal Processing
                'By Steven W. Smith, Ph.D.
                'http://www.dspguide.com/ch17/1.htm

                Try

                    Dim outputSound As New Sound(waveFormat)
                    Dim AudioOutputConstructor As New AudioOutputConstructor(waveFormat, channel)

                    Dim posFS As Double = waveFormat.PositiveFullScale

                    outputSound.FFT = New FftData(waveFormat, fftFormat)


                    'Checks that kernel size is not larger than fftSize, increases fftSize is that is the case
                    If kernelSize > fftFormat.FftWindowSize Then
                        CheckAndAdjustFFTSize(fftFormat.FftWindowSize, kernelSize, InActivateWarnings)
                    End If

                    'Converting the stopbandlevel to linear form
                    Dim StopBandMagnitude As Double = dBConversion(StopBandLevel_dB, dBConversionDirection.from_dB, waveFormat)

                    Dim StopBandLevel As Double = StopBandMagnitude / posFS 'OBS STämmer detta verkligen , kan maninte bara använda StopBandMagnitude istället nedan

                    'Converting the passbandlevel to linear form
                    Dim passBandMagnitude As Double = dBConversion(passBandLevel_dB, dBConversionDirection.from_dB, waveFormat)
                    Dim passBandLevel As Double = passBandMagnitude / posFS

                    'Extracting the sample rate
                    Dim SR As Integer = waveFormat.SampleRate

                    'Setting k values equivalant to the cut-off frequencies
                    Dim kForCF1 As Integer = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, CutOffFreq1, SR, fftFormat.FftWindowSize, Utils.roundingMethods.getClosestValue)
                    Dim kForCF2 As Integer = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, CutOffFreq2, SR, fftFormat.FftWindowSize, Utils.roundingMethods.getClosestValue)

                    'Calculating the cutoff frequencies being used, depending on the fftSize
                    CutOffFreq1 = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.BinIndexToFrequency, kForCF1, SR, fftFormat.FftWindowSize)
                    CutOffFreq2 = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.BinIndexToFrequency, kForCF2, SR, fftFormat.FftWindowSize)


                    'Main section
                    Select Case waveFormat.BitDepth
                        Case 16, 32

                            For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                                Dim magnitudeArray(fftFormat.FftWindowSize - 1) As Single
                                Dim phaseArray(fftFormat.FftWindowSize - 1) As Single

                                Select Case IRType
                                    Case FilterType.RandomPhase
                                        'Setting magnitude values
                                        For n = 1 To magnitudeArray.Length - 1
                                            magnitudeArray(n) = 10
                                        Next
                                        'setting the magnitude of special frequencies
                                        magnitudeArray(0) = 0
                                        magnitudeArray(magnitudeArray.Length / 2) = 0


                                        'creating an array with random phases, with the length fftsize
                                        phaseArray(0) = 0
                                        phaseArray(fftFormat.FftWindowSize / 2) = Math.PI 'ska denna vara PI ???, eller 0, eller vad som helst?


                                        Dim rnd As New Random
                                        For q = 1 To phaseArray.Length / 2 - 1
                                            phaseArray(q) = (rnd.Next(-Math.PI * 1000000, Math.PI * 1000000)) / 1000000
                                            'RandomPhaseArray(RandomPhaseArray.Length - q) = -RandomPhaseArray(q) 'denna behövs inte
                                        Next


                                        'Adjusting phase

                                        '1. justerar faserna till intervallet -PI til PI, phase unwrapping. Behövs nog inte här!
                                        For n = 1 To phaseArray.Length / 2 - 1
                                            If phaseArray(n) < -Math.PI Then phaseArray(n) += twopi
                                            If phaseArray(n) > Math.PI Then phaseArray(n) -= twopi
                                        Next

                                        '2. Kopierar fasinformationen till de negativa frekvenserna
                                        For q = 1 To phaseArray.Length / 2 - 1
                                            phaseArray(phaseArray.Length - q) = -phaseArray(q)
                                        Next

                                    Case FilterType.LowPass

                                        'Setting the passband magnitudes
                                        For k = 0 To kForCF1 '- 1
                                            magnitudeArray(k) = passBandLevel
                                        Next

                                        'Fine tuning the cut-off frequency by adjusting the dft bin closest to the intended cut-off frequency 
                                        'magnitudeArray(kForCF1) = interpolateCutOffFrequency(passBandMagnitude, StopBandLevel, CutOffFreq1, kForCF1, fftSize, SR)

                                        'Setting the stopband magnitudes
                                        For k = kForCF1 + 1 To magnitudeArray.Length / 2 - 1
                                            magnitudeArray(k) = StopBandLevel
                                        Next

                                        'Setting the phases to zero
                                        For q = 0 To phaseArray.Length - 1
                                            phaseArray(q) = 0
                                        Next

                                        'setting the magnitude of special frequencies
                                        magnitudeArray(0) = passBandLevel
                                        magnitudeArray(magnitudeArray.Length / 2) = 0

                                    Case FilterType.HighPass

                                        'Setting the magnitudes
                                        For k = 0 To kForCF1 - 1
                                            magnitudeArray(k) = StopBandLevel
                                        Next
                                        For k = kForCF1 To magnitudeArray.Length / 2 - 1
                                            magnitudeArray(k) = passBandLevel
                                        Next

                                        'Setting the phases to zero
                                        For q = 0 To phaseArray.Length - 1
                                            phaseArray(q) = 0
                                        Next

                                        'setting the magnitude of special frequencies
                                        magnitudeArray(0) = 0
                                        magnitudeArray(magnitudeArray.Length / 2) = passBandLevel

                                    Case FilterType.BandPass

                                        'Setting the magnitudes
                                        For k = 0 To kForCF1 - 1
                                            magnitudeArray(k) = StopBandLevel
                                        Next
                                        For k = kForCF1 To kForCF2
                                            magnitudeArray(k) = passBandLevel
                                        Next
                                        For k = kForCF2 + 1 To magnitudeArray.Length / 2 - 1
                                            magnitudeArray(k) = StopBandLevel
                                        Next

                                        'Setting the phases to zero
                                        For q = 0 To phaseArray.Length - 1
                                            phaseArray(q) = 0
                                        Next

                                    Case FilterType.BandStop

                                        'Setting the magnitudes
                                        For k = 0 To kForCF1 - 1
                                            magnitudeArray(k) = passBandLevel
                                        Next
                                        For k = kForCF1 To kForCF2
                                            magnitudeArray(k) = StopBandLevel
                                        Next
                                        For k = kForCF2 + 1 To magnitudeArray.Length / 2 - 1
                                            magnitudeArray(k) = passBandLevel
                                        Next

                                        'Setting the phases to zero
                                        For q = 0 To phaseArray.Length - 1
                                            phaseArray(q) = 0
                                        Next

                                    Case FilterType.LinearAttenuationBelowCF_dBPerOctave

                                        'Setting the magnitudes below cut-off frequency 1
                                        For k = 0 To kForCF1 - 1
                                            Dim frequency As Single = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.BinIndexToFrequency, k, waveFormat.SampleRate, fftFormat.FftWindowSize, Utils.roundingMethods.donNotRound)
                                            Dim attenuationIn_dB As Single = attenuationRate * Utils.getBase_n_Log(frequency, 2) - attenuationRate * Utils.getBase_n_Log(CutOffFreq1, 2)
                                            Dim attenuationFactor As Single = dBConversion(attenuationIn_dB, dBConversionDirection.from_dB, waveFormat) / posFS
                                            magnitudeArray(k) = passBandLevel * attenuationFactor
                                        Next

                                        'Setting the magnitudes above cut-off frequency 1
                                        For k = kForCF1 To magnitudeArray.Length / 2 - 1
                                            magnitudeArray(k) = passBandLevel
                                        Next

                                        'Setting the phases to zero
                                        For q = 0 To phaseArray.Length - 1
                                            phaseArray(q) = 0
                                        Next

                                    Case FilterType.LinearAttenuationAboveCF_dBPerOctave

                                        'Setting the magnitudes below cut-off frequency 1
                                        For k = 0 To kForCF1
                                            magnitudeArray(k) = passBandLevel
                                        Next

                                        'Setting the magnitudes above cut-off frequency 1
                                        For k = kForCF1 + 1 To magnitudeArray.Length / 2 - 1
                                            Dim frequency As Single = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.BinIndexToFrequency, k, waveFormat.SampleRate, fftFormat.FftWindowSize, Utils.roundingMethods.donNotRound)
                                            Dim attenuationIn_dB As Single = attenuationRate * Utils.getBase_n_Log(CutOffFreq1, 2) - attenuationRate * Utils.getBase_n_Log(frequency, 2)
                                            Dim attenuationFactor As Single = dBConversion(attenuationIn_dB, dBConversionDirection.from_dB, waveFormat) / posFS
                                            magnitudeArray(k) = passBandLevel * attenuationFactor

                                        Next

                                        'Setting the phases to zero
                                        For q = 0 To phaseArray.Length - 1
                                            phaseArray(q) = 0
                                        Next


                                End Select

                                'Copies the magnitude information to the negative frequencies
                                For q = 1 To magnitudeArray.Length / 2 - 1
                                    magnitudeArray(magnitudeArray.Length - q) = magnitudeArray(q)
                                Next


                                Dim NewMagnitudeTimeWindow As New FftData.TimeWindow
                                NewMagnitudeTimeWindow.WindowData = magnitudeArray
                                outputSound.FFT.AmplitudeSpectrum(c, 0) = NewMagnitudeTimeWindow

                                Dim NewPhaseTimeWindow As New FftData.TimeWindow
                                NewPhaseTimeWindow.WindowData = phaseArray
                                outputSound.FFT.PhaseSpectrum(c, 0) = NewPhaseTimeWindow

                                'Skapar DFT bins
                                'Dim FFT_X(fftFormat.FftWindowSize - 1) As Single
                                'Dim FFT_Y(fftFormat.FftWindowSize - 1) As Single

                                'Try
                                'SaveNumericArrayToTextFile(phaseArray, standardOutputDirectory & "phaseArray.txt")
                                'Catch ex As Exception
                                'MsgBox(ex.ToString)
                                'End Try

                                'Transforms to rectangular form
                                outputSound.FFT.CalculateRectangualForm()
                                'getRectangualForm(magnitudeArray, phaseArray, FFT_X, FFT_Y)

                                'Performing an inverse dft on the magnitude and phase arrays
                                DSP.TransformationsExt.FFT_Bourke(-1, Utils.getBase_n_Log(fftFormat.FftWindowSize, 2), outputSound.FFT.FrequencyDomainRealData(c, 0).WindowData, outputSound.FFT.FrequencyDomainImaginaryData(c, 0).WindowData)

                                'Shifting + truncating
                                Dim kernelArray(kernelSize - 1) As Single
                                Dim index As Integer = 0
                                For n = 0 To kernelSize / 2 - 1
                                    kernelArray(index) = outputSound.FFT.FrequencyDomainRealData(c, 0).WindowData(fftFormat.FftWindowSize - (kernelSize / 2 - n))
                                    index += 1
                                Next
                                For n = 0 To kernelSize / 2 - 1
                                    kernelArray(index) = outputSound.FFT.FrequencyDomainRealData(c, 0).WindowData(n)
                                    index += 1
                                Next

                                'Scaling the kernel sample values by fft length
                                For n = 0 To kernelArray.Length - 1
                                    kernelArray(n) /= fftFormat.FftWindowSize
                                Next

                                'Windowing
                                WindowingFunction(kernelArray, windowFunction)

                                'wIO.SaveArrayToTextFile(averageMagnitudes, "C:\VB_Test_Folder\array1")
                                'Dim SaveToTextFile As Boolean = False
                                'If SaveToTextFile = True Then SaveNumericArrayToTextFile(kernelArray, "C:\VB_Test_Folder\array2")

                                'Storing sound
                                outputSound.WaveData.SampleData(c) = kernelArray

                            Next

                        Case Else
                            Throw New NotImplementedException(waveFormat.BitDepth & " bit depth Is Not yet supported.")

                    End Select

                    Return outputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function



            ''' <summary>
            ''' Creates an inpulse response based on the supplied FrequencyResponse.
            ''' </summary>
            ''' <param name="FrequencyResponse"></param>
            ''' <param name="PhaseRandomizationDegrees"></param>
            ''' <param name="waveFormat"></param>
            ''' <param name="fftFormat"></param>
            ''' <param name="kernelSize"></param>
            ''' <param name="windowFunction"></param>
            ''' <param name="InActivateWarnings"></param>
            ''' <param name="FrequencyResponseIsLinear">Set to True to specify frequency response in dB, or False to specify linear frequency response.</param>
            ''' <returns></returns>
            Public Function CreateCustumImpulseResponse(ByRef FrequencyResponse As List(Of Tuple(Of Single, Single)),
                                                    ByRef PhaseRandomizationDegrees As List(Of Tuple(Of Single, Single)),
                                                    ByRef waveFormat As Formats.WaveFormat,
                                                    ByRef fftFormat As Formats.FftFormat,
                                                    ByVal kernelSize As Integer,
                                                    Optional ByVal windowFunction As WindowingType = WindowingType.Hamming,
                                                    Optional ByVal InActivateWarnings As Boolean = False,
                                                    Optional ByVal FrequencyResponseIsLinear As Boolean = False) As Sound

                'Reference which parts of this code is based on:
                'The Scientist And Engineer's Guide to
                'Digital Signal Processing
                'By Steven W. Smith, Ph.D.
                'http://www.dspguide.com/ch17/1.htm

                Try

                    If FrequencyResponse Is Nothing Then FrequencyResponse = New List(Of Tuple(Of Single, Single))
                    If PhaseRandomizationDegrees Is Nothing Then PhaseRandomizationDegrees = New List(Of Tuple(Of Single, Single))

                    Dim outputSound As New Sound(New Formats.WaveFormat(waveFormat.SampleRate, waveFormat.BitDepth, 1,, waveFormat.Encoding))

                    Dim posFS As Double = waveFormat.PositiveFullScale
                    outputSound.FFT = New FftData(waveFormat, fftFormat)


                    'Checks that kernel size is not larger than fftSize, increases fftSize is that is the case
                    If kernelSize > fftFormat.FftWindowSize Then
                        CheckAndAdjustFFTSize(fftFormat.FftWindowSize, kernelSize, InActivateWarnings)
                    End If

                    'Noting the current sample rate
                    Dim SR As Integer = waveFormat.SampleRate

                    'Setting k values equivalent to the frequency response centre frequencies
                    Dim FrequencyResponseBinIndices As New SortedList(Of Double, Double)
                    If FrequencyResponse.Count > 0 Then
                        'Adding values for the the lowest bin
                        'FrequencyResponseBinIndices.Add(FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, FrequencyResponse(0).Item1, SR, fftFormat.FftWindowSize, roundingMethods.getClosestValue), FrequencyResponse(0).Item2)
                        ''Adding 0 dB to bin 0, and the first available value to bin 1
                        'FrequencyResponseBinIndices.Add(0, 0)
                        FrequencyResponseBinIndices.Add(1, FrequencyResponse(0).Item2)

                        'Adding intermediate bin indices
                        For Each CentreFrequency In FrequencyResponse
                            Dim Key As Double = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, CentreFrequency.Item1, SR, fftFormat.FftWindowSize, Utils.roundingMethods.getClosestValue)
                            If Not FrequencyResponseBinIndices.ContainsKey(Key) Then FrequencyResponseBinIndices.Add(Key, CentreFrequency.Item2)
                        Next

                        'Adding values for the highest bin
                        Dim LastKey As Double = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, FrequencyResponse(FrequencyResponse.Count - 1).Item1, SR, fftFormat.FftWindowSize, Utils.roundingMethods.getClosestValue)
                        If Not FrequencyResponseBinIndices.ContainsKey(LastKey) Then FrequencyResponseBinIndices.Add(LastKey, FrequencyResponse(FrequencyResponse.Count - 1).Item2)

                    End If

                    ''The code below can be used to get the actual centre frequencies
                    'For Each CentreFrequency In FrequencyResponseBinIndices
                    '    Dim CF = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.BinIndexToFrequency, CentreFrequency, SR, fftFormat.FftWindowSize)
                    'Next

                    'Setting k values equivalent to the phase response 
                    Dim PhaseResponseBinIndices As New SortedList(Of Double, Double)
                    If PhaseRandomizationDegrees.Count > 0 Then
                        'Adding values for the the lowest bin
                        'PhaseResponseBinIndices.Add(FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, PhaseRandomizationDegrees(0).Item1, SR, fftFormat.FftWindowSize, roundingMethods.getClosestValue), PhaseRandomizationDegrees(0).Item2)
                        ''Adding phase 0 to bin 0, and the first available value to bin 1
                        'PhaseResponseBinIndices.Add(0, 0)
                        PhaseResponseBinIndices.Add(1, PhaseRandomizationDegrees(0).Item2)

                        'Adding intermediate bin indices
                        For Each CentreFrequency In PhaseRandomizationDegrees
                            Dim Key As Double = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, CentreFrequency.Item1, SR, fftFormat.FftWindowSize, Utils.roundingMethods.getClosestValue)
                            If Not PhaseResponseBinIndices.ContainsKey(Key) Then PhaseResponseBinIndices.Add(Key, CentreFrequency.Item2)
                        Next

                        'Adding values for the highest bin
                        Dim LastKey As Double = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, PhaseRandomizationDegrees(PhaseRandomizationDegrees.Count - 1).Item1, SR, fftFormat.FftWindowSize, Utils.roundingMethods.getClosestValue)
                        If Not PhaseResponseBinIndices.ContainsKey(LastKey) Then PhaseResponseBinIndices.Add(LastKey, PhaseRandomizationDegrees(PhaseRandomizationDegrees.Count - 1).Item2)
                    End If



                    'Main section
                    Dim c As Integer = 1 ' Sound channel
                    Select Case waveFormat.BitDepth
                        Case 16, 32

                            Dim magnitudeArray(fftFormat.FftWindowSize - 1) As Single
                            Dim phaseArray(fftFormat.FftWindowSize - 1) As Single

                            If FrequencyResponse.Count > 0 Then

                                'Setting magnitudes (including CD and Nyquist)
                                For k = 0 To magnitudeArray.Length / 2

                                    Dim CurrentInterpolatedResponse As Double = Utils.LinearInterpolation_GetY(k, FrequencyResponseBinIndices)

                                    'Converting the frequency responses to linear form
                                    Dim LinearMagnitude As Double
                                    If FrequencyResponseIsLinear = False Then
                                        LinearMagnitude = dBConversion(CurrentInterpolatedResponse, dBConversionDirection.from_dB, waveFormat) / posFS ' OBS ska man verkligen dividera med posFS här? Testa!
                                    Else
                                        LinearMagnitude = CurrentInterpolatedResponse
                                    End If

                                    'Storing the linear magnitude
                                    magnitudeArray(k) = LinearMagnitude
                                Next

                                'Copies the magnitude information to the negative frequencies
                                For q = 1 To magnitudeArray.Length / 2 - 1
                                    magnitudeArray(magnitudeArray.Length - q) = magnitudeArray(q)
                                Next

                            Else

                                'Setting default magnitude values (of no gain)
                                For n = 1 To magnitudeArray.Length - 1
                                    magnitudeArray(n) = 1
                                Next
                                'setting the magnitude of special frequencies
                                magnitudeArray(0) = 1
                                magnitudeArray(magnitudeArray.Length / 2) = 1

                            End If


                            If PhaseRandomizationDegrees.Count > 0 Then

                                'Randomizing phases                    

                                'creating an array with random phases, with the length fftsize
                                phaseArray(0) = 0
                                phaseArray(fftFormat.FftWindowSize / 2) = Math.PI 'ska denna vara PI ???, eller 0, eller vad som helst?

                                Dim rnd As New Random

                                'Setting magnitudes
                                For k = 0 To magnitudeArray.Length / 2 - 1

                                    'Interpolating a current phase radnomization degree
                                    Dim CurrentInterpolatedPhaseShiftDegree As Double = Math.Min(1, Math.Max(0, Utils.LinearInterpolation_GetY(k, PhaseResponseBinIndices)))

                                    'Storing the phase
                                    phaseArray(k) = CurrentInterpolatedPhaseShiftDegree * (rnd.NextDouble - 0.5) * 2 * Math.PI
                                Next

                                'Copies the phase data to the negative frequencies
                                For q = 1 To phaseArray.Length / 2 - 1
                                    phaseArray(phaseArray.Length - q) = -phaseArray(q)
                                Next

                            Else

                                'Setting default phases (to zero)
                                For q = 0 To phaseArray.Length - 1
                                    phaseArray(q) = 0
                                Next

                            End If

                            'Utils.SendInfoToLog(vbCrLf & String.Join(vbCrLf, magnitudeArray), "IR_Magnitudes")
                            'Utils.SendInfoToLog(vbCrLf & String.Join(vbCrLf, phaseArray), "IR_Phases")

                            Dim NewMagnitudeTimeWindow As New FftData.TimeWindow
                            NewMagnitudeTimeWindow.WindowData = magnitudeArray
                            outputSound.FFT.AmplitudeSpectrum(c, 0) = NewMagnitudeTimeWindow

                            Dim NewPhaseTimeWindow As New FftData.TimeWindow
                            NewPhaseTimeWindow.WindowData = phaseArray
                            outputSound.FFT.PhaseSpectrum(c, 0) = NewPhaseTimeWindow

                            'Transforms to rectangular form
                            outputSound.FFT.CalculateRectangualForm()

                            'Copyiong to double arrays, so that FFT can be run on the Double datatype instead of Single
                            Dim X_Re(outputSound.FFT.FrequencyDomainRealData(c, 0).WindowData.Length - 1) As Double
                            Dim X_Im(outputSound.FFT.FrequencyDomainImaginaryData(c, 0).WindowData.Length - 1) As Double

                            For s = 0 To X_Re.Length - 1
                                X_Re(s) = outputSound.FFT.FrequencyDomainRealData(c, 0).WindowData(s)
                            Next
                            For s = 0 To X_Im.Length - 1
                                X_Im(s) = outputSound.FFT.FrequencyDomainImaginaryData(c, 0).WindowData(s)
                            Next

                            'Performing an inverse dft on the magnitude and phase arrays
                            DSP.TransformationsExt.FFT_Bourke(-1, Utils.getBase_n_Log(fftFormat.FftWindowSize, 2), X_Re, X_Im)

                            'Out-commented code for FFT with Single datatype
                            'DSP.Transformations.FFT_Bourke(-1, getBase_n_Log(fftFormat.FftWindowSize, 2), outputSound.FFT.FrequencyDomainRealData(c, 0).WindowData, outputSound.FFT.FrequencyDomainImaginaryData(c, 0).WindowData)

                            'Shifting + truncating
                            Dim kernelArray(kernelSize - 1) As Single
                            Dim index As Integer = 0
                            For n = 0 To kernelSize / 2 - 1
                                kernelArray(index) = X_Re(fftFormat.FftWindowSize - (kernelSize / 2 - n))
                                index += 1
                            Next
                            For n = 0 To kernelSize / 2 - 1
                                kernelArray(index) = X_Re(n)
                                index += 1
                            Next

                            'Out-commented code for FFT with Single datatype
                            'Dim kernelArray(kernelSize - 1) As Single
                            'Dim index As Integer = 0
                            'For n = 0 To kernelSize / 2 - 1
                            '    kernelArray(index) = outputSound.FFT.FrequencyDomainRealData(c, 0).WindowData(fftFormat.FftWindowSize - (kernelSize / 2 - n))
                            '    index += 1
                            'Next
                            'For n = 0 To kernelSize / 2 - 1
                            '    kernelArray(index) = outputSound.FFT.FrequencyDomainRealData(c, 0).WindowData(n)
                            '    index += 1
                            'Next

                            'Scaling the kernel sample values by fft length
                            For n = 0 To kernelArray.Length - 1
                                kernelArray(n) /= fftFormat.FftWindowSize
                            Next

                            'Windowing
                            WindowingFunction(kernelArray, windowFunction)

                            'Storing sound
                            outputSound.WaveData.SampleData(c) = kernelArray

                        Case Else
                            Throw New NotImplementedException(waveFormat.BitDepth & " bit depth Is Not yet supported.")

                    End Select

                    Return outputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function



            Public Function GetImpulseResponseFromSound(ByRef InputSound As Sound,
                                                    ByRef fftFormat As Formats.FftFormat,
                                                    ByVal kernelSize As Integer,
                                                    Optional ByVal channel As Integer? = Nothing,
                                                    Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing,
                                                    Optional NormalizeOutputSound As Boolean = False,
                                                    Optional ByVal SkipFinalWindowing As Boolean = False) As Sound ' Optional normalizingSpectralMagnidutes As Boolean = True

                'Reference which this code is based on:
                'The Scientist And Engineer's Guide to
                'Digital Signal Processing
                'By Steven W. Smith, Ph.D.
                'http://www.dspguide.com/ch17/1.htm

                'Prepares an outout sound
                Dim outputSound As New Sound(InputSound.WaveFormat)
                Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, channel)

                'Checks that kernel size is not larger than fftSize, increases fftSize is that is the case
                If kernelSize > fftFormat.FftWindowSize Then
                    CheckAndAdjustFFTSize(fftFormat.FftWindowSize, kernelSize)
                End If

                'Main section
                'Ser till att windowSize alltid är ett jämnt tal
                If fftFormat.FftWindowSize Mod 2 = 1 Then fftFormat.FftWindowSize += 1

                'Analysing input sound
                'Performs a dft on the input file
                InputSound.FFT = DSP.SpectralAnalysis(InputSound, fftFormat, , startSample, sectionLength)

                'Calculating magnitudes
                InputSound.FFT.CalculateAmplitudeSpectrum(False, False, False)

                For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                    'Calculates average magnitudes
                    Dim averageMagnitudes(fftFormat.FftWindowSize - 1) As Double
                    For n = 0 To fftFormat.FftWindowSize - 1
                        Dim accumulator As Double = 0
                        For windowNumber = 0 To InputSound.FFT.WindowCount(c) - 1
                            accumulator += InputSound.FFT.AmplitudeSpectrum(c, windowNumber).WindowData(n)
                        Next
                        averageMagnitudes(n) = accumulator / fftFormat.FftWindowSize
                    Next

                    'Dim maxAverageMagnitude As Double
                    'If normalizingSpectralMagnidutes = True Then 'Normalising the max amplitude of averageAmplitudeArray to 1
                    'maxAverageMagnitude = (averageMagnitudes.Max)
                    'For n = 0 To averageMagnitudes.Length - 1
                    'If Not maxAverageMagnitude = 0 Then
                    'averageMagnitudes(n) = averageMagnitudes(n) / maxAverageMagnitude
                    'Else
                    'Throw New Exception("GetImpulseResponseFromFile tried to normalise the spectral amplitudes. However the input sound file was silent!")
                    'End If
                    'Next
                    'End If


                    'Since the phase can be set to 0, the real part of the signal is equal to the magnitudes
                    Dim temporaryIMX(fftFormat.FftWindowSize - 1) As Double

                    'Performing an inverse dft on the magnitudes
                    'FFT_Bourke(-1, getBaseTwoLog(fftFormat.FftWindowSize), averageMagnitudes, temporaryIMX)

                    DSP.FFT_Bourke(-1, Utils.Calculations.getBase_n_Log(fftFormat.FftWindowSize, 2), averageMagnitudes, temporaryIMX)

                    'Shifting + truncate
                    Dim kernelArray(kernelSize - 1) As Single
                    Dim index As Integer = 0
                    For n = 0 To kernelSize / 2 - 1
                        kernelArray(index) = averageMagnitudes(fftFormat.FftWindowSize - (kernelSize / 2 - n))
                        index += 1
                    Next
                    For n = 0 To kernelSize / 2 - 1
                        kernelArray(index) = averageMagnitudes(n)
                        index += 1
                    Next

                    ''Scaling the kernel sample values by fft length
                    'For n = 0 To kernelArray.Length - 1
                    '    kernelArray(n) /= fftFormat.FftWindowSize
                    'Next

                    'Windowing
                    If SkipFinalWindowing = False Then
                        WindowingFunction(kernelArray, WindowingType.Hamming)
                    End If

                    'Storing output sound
                    outputSound.WaveData.SampleData(c) = kernelArray

                Next

                'Normalizing
                If NormalizeOutputSound = True Then DSP.MaxAmplitudeNormalizeSection(outputSound)

                'Resetting InputSound.FFT
                InputSound.FFT = Nothing

                Return outputSound

            End Function

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="InputSound1"></param>
            ''' <param name="InputSound2"></param>
            ''' <param name="startSample_s1"></param>
            ''' <param name="sectionLength_s1"></param>
            ''' <param name="startSample_s2"></param>
            ''' <param name="sectionLength_s2"></param>
            ''' <param name="fftFormat"></param>
            ''' <param name="kernelSize"></param>
            ''' <param name="channel"></param>
            ''' <param name="MaxBandGain"></param>
            ''' <param name="MaxBandAttenuation"></param>
            ''' <param name="LowerCutoffFrequency">'If set, gain below the indicated frequency will be 0 dB.</param>
            ''' <param name="UpperCutoffFrequency">'If set, gain above the indicated frequency will be 0 dB.</param>
            ''' <returns></returns>
            Public Function GetImpulseResponseForSpectralSubtraction(ByRef InputSound1 As Sound, ByRef InputSound2 As Sound,
                                                                 Optional ByVal startSample_s1 As Integer = 0, Optional ByVal sectionLength_s1 As Integer? = Nothing,
                                                                 Optional ByVal startSample_s2 As Integer = 0, Optional ByVal sectionLength_s2 As Integer? = Nothing,
                                                                 Optional ByRef fftFormat As Formats.FftFormat = Nothing,
                                                                 Optional ByVal kernelSize As Integer = 4000,
                                                                 Optional ByVal channel As Integer? = Nothing,
                                                                 Optional MaxBandGain As Double? = 10,
                                                                 Optional MaxBandAttenuation As Double? = 10,
                                                                 Optional ByVal LowerCutoffFrequency As Double? = Nothing,
                                                                 Optional ByVal UpperCutoffFrequency As Double? = Nothing) As Sound ' Optional normalizingSpectralMagnidutes As Boolean = True

                'Reference which this code is based on:
                'The Scientist And Engineer's Guide to
                'Digital Signal Processing
                'By Steven W. Smith, Ph.D.
                'http://www.dspguide.com/ch17/1.htm

                'Checks sounds
                'CheckAndCorrectSectionLength (on the lines below) was outcommented on 2022-03-29. Instead the calling code need to be responsible for sending the correct startSample and sectionLength
                'CheckAndCorrectSectionLength(InputSound1.WaveData.ShortestChannelSampleCount, startSample_s1, sectionLength_s1)
                'CheckAndCorrectSectionLength(InputSound2.WaveData.ShortestChannelSampleCount, startSample_s2, sectionLength_s2)


                'Prepares an outout sound
                Dim outputSound As New Sound(InputSound1.WaveFormat)
                Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound1.WaveFormat, channel)

                'Checks that kernel size is not larger than fftSize, increases fftSize is that is the case
                If kernelSize > fftFormat.FftWindowSize Then
                    CheckAndAdjustFFTSize(fftFormat.FftWindowSize, kernelSize)
                End If

                'Main section
                'Ser till att windowSize alltid är ett jämnt tal
                If fftFormat.FftWindowSize Mod 2 = 1 Then fftFormat.FftWindowSize += 1


                'Analysing input sounds
                'Performs a dft on the input file
                InputSound1.FFT = DSP.SpectralAnalysis(InputSound1, fftFormat, , startSample_s1, sectionLength_s1)
                InputSound2.FFT = DSP.SpectralAnalysis(InputSound2, fftFormat, , startSample_s2, sectionLength_s2)

                'Calculating magnitudes
                InputSound1.FFT.CalculateAmplitudeSpectrum(True, True, True)
                InputSound2.FFT.CalculateAmplitudeSpectrum(True, True, True)

                For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                    'Calculates average magnitudes
                    'Sound1
                    Dim BinSpectrum_1(InputSound1.FFT.FftFormat.FftWindowSize - 1) As Single
                    For k = 0 To InputSound1.FFT.AmplitudeSpectrum(c, 0).WindowData.Length - 1

                        For TimeWindow = 0 To InputSound1.FFT.WindowCount(c) - 1

                            'Converting spectral magnitudes to power. Summing spectral power. 
                            'SummedBinPowers += 2 * 10 ^ ((20 * Math.Log10((Math.Sqrt(2) * AmplitudeSpectrum(channel, TimeWindow).WindowData(k)) / Math.Sqrt(2))) / 10)
                            'SummedBinPowers += 2 * 10 ^ ((20 * Math.Log10(AmplitudeSpectrum(channel, TimeWindow).WindowData(k))) / 10)
                            'Simplified to:
                            BinSpectrum_1(k) += 100 ^ (Math.Log10(InputSound1.FFT.AmplitudeSpectrum(c, TimeWindow).WindowData(k)))

                        Next

                        'Taking the quare root to convert power spectrum to amplitude spectrum, and divides by WindowCount(Channel) to average the value of the time windows
                        BinSpectrum_1(k) = Math.Sqrt(BinSpectrum_1(k) / InputSound1.FFT.WindowCount(c))

                        'Converting to dB
                        BinSpectrum_1(k) = dBConversion(BinSpectrum_1(k), dBConversionDirection.to_dB,
                                              InputSound1.WaveFormat, dBTypes.SoundPressure)

                    Next


                    'Sound2 
                    'Calculates average magnitudes
                    'Sound2
                    Dim BinSpectrum_2(InputSound2.FFT.FftFormat.FftWindowSize - 1) As Single
                    For k = 0 To InputSound2.FFT.AmplitudeSpectrum(c, 0).WindowData.Length - 1

                        For TimeWindow = 0 To InputSound2.FFT.WindowCount(c) - 1

                            'Converting spectral magnitudes to power. Summing spectral power. 
                            'SummedBinPowers += 2 * 10 ^ ((20 * Math.Log10((Math.Sqrt(2) * AmplitudeSpectrum(channel, TimeWindow).WindowData(k)) / Math.Sqrt(2))) / 10)
                            'SummedBinPowers += 2 * 10 ^ ((20 * Math.Log10(AmplitudeSpectrum(channel, TimeWindow).WindowData(k))) / 10)
                            'Simplified to:
                            BinSpectrum_2(k) += 100 ^ (Math.Log10(InputSound2.FFT.AmplitudeSpectrum(c, TimeWindow).WindowData(k)))

                        Next

                        'Taking the quare root to convert power spectrum to amplitude spectrum, and divides by WindowCount(Channel) to average the value of the time windows
                        BinSpectrum_2(k) = Math.Sqrt(BinSpectrum_2(k) / InputSound2.FFT.WindowCount(c))

                        'Converting to dB
                        BinSpectrum_2(k) = dBConversion(BinSpectrum_2(k), dBConversionDirection.to_dB,
                                              InputSound2.WaveFormat, dBTypes.SoundPressure)

                    Next

                    'Calculating spectral subtraction
                    Dim SubtractionMagnitudes(fftFormat.FftWindowSize - 1) As Single
                    For n = 0 To fftFormat.FftWindowSize - 1
                        'converting to and from dB, limiting band gain to MaxBandGain
                        'Dim s1Level = dBConversion(averageMagnitudes_s1(n), dBConversionDirection.to_dB, InputSound1.WaveFormat)
                        'Dim s2Level = dBConversion(averageMagnitudes_s2(n), dBConversionDirection.to_dB, InputSound1.WaveFormat)

                        Dim s1Level = BinSpectrum_1(n)
                        Dim s2Level = BinSpectrum_2(n)

                        Dim TargetGain As Double = s1Level - s2Level
                        If MaxBandGain.HasValue Then TargetGain = Math.Min(MaxBandGain.Value, TargetGain)
                        If MaxBandAttenuation.HasValue Then TargetGain = Math.Max(-MaxBandAttenuation.Value, TargetGain)

                        SubtractionMagnitudes(n) = dBConversion(TargetGain, dBConversionDirection.from_dB, InputSound1.WaveFormat)

                    Next

                    'Overriding any values below and above the cut-off frequencies with zero
                    If LowerCutoffFrequency.HasValue Then

                        'Calculating the cut-off bin index
                        Dim LowerCutoffBin As Integer = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                                                                              LowerCutoffFrequency.Value,
                                                                              InputSound1.WaveFormat.SampleRate,
                                                                              fftFormat.FftWindowSize, Utils.roundingMethods.alwaysDown)

                        Dim NoGainValue As Double = dBConversion(0, dBConversionDirection.from_dB, InputSound1.WaveFormat)

                        For PositiveBinIndex = 0 To LowerCutoffBin - 1

                            'Positive frequencies
                            SubtractionMagnitudes(PositiveBinIndex) = NoGainValue

                            'Negative frequencies
                            Dim NegativeBinIndex As Integer = fftFormat.FftWindowSize - 1 - PositiveBinIndex
                            SubtractionMagnitudes(NegativeBinIndex) = NoGainValue
                        Next
                    End If

                    If UpperCutoffFrequency.HasValue Then

                        'Calculating the cut-off bin index
                        Dim UpperCutoffBin As Integer = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                                                                              UpperCutoffFrequency.Value,
                                                                              InputSound1.WaveFormat.SampleRate,
                                                                              fftFormat.FftWindowSize, Utils.roundingMethods.alwaysDown)

                        Dim NoGainValue As Double = dBConversion(0, dBConversionDirection.from_dB, InputSound1.WaveFormat)

                        For PositiveBinIndex = UpperCutoffBin To fftFormat.FftWindowSize / 2 - 1

                            'Positive frequencies
                            SubtractionMagnitudes(PositiveBinIndex) = NoGainValue

                            'Negative frequencies
                            Dim NegativeBinIndex As Integer = fftFormat.FftWindowSize - 1 - PositiveBinIndex
                            SubtractionMagnitudes(NegativeBinIndex) = NoGainValue
                        Next
                    End If

                    'Since the phase can be set to 0, the real part of the signal is equal to the magnitudes
                    Dim temporaryIMX(fftFormat.FftWindowSize - 1) As Single

                    'Performing an inverse dft on the magnitudes
                    DSP.FFT_Bourke(-1, Utils.getBase_n_Log(fftFormat.FftWindowSize, 2), SubtractionMagnitudes, temporaryIMX)

                    'Shifting + truncate
                    Dim kernelArray(kernelSize - 1) As Single
                    Dim index As Integer = 0
                    For n = 0 To kernelSize / 2 - 1
                        kernelArray(index) = SubtractionMagnitudes(fftFormat.FftWindowSize - (kernelSize / 2 - n))
                        index += 1
                    Next
                    For n = 0 To kernelSize / 2 - 1
                        kernelArray(index) = SubtractionMagnitudes(n)
                        index += 1
                    Next

                    'Windowing
                    WindowingFunction(kernelArray, WindowingType.Hamming)

                    'Scaling the kernel sample values by fft length
                    For n = 0 To kernelArray.Length - 1
                        kernelArray(n) /= fftFormat.FftWindowSize
                    Next

                    'Storing output sound
                    outputSound.WaveData.SampleData(c) = kernelArray

                Next

                'Resetting InputSound.FFT
                InputSound1.FFT = Nothing
                InputSound2.FFT = Nothing

                Return outputSound

            End Function


            Public Function GetIrArrayFromSound(ByRef InputSound As Sound,
                                            ByRef StepSize As Integer, ByRef SectionLengths As Integer,
                                            ByRef FftFormat As Formats.FftFormat, ByVal KernelSize As Integer,
                                            Optional ByVal Channel As Integer? = Nothing,
                                            Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing) As Sound()

                Try

                    Dim Output As New List(Of Sound)

                    'CheckAndCorrectSectionLength (on the line below) was outcommented on 2022-03-29. Instead the calling code need to be responsible for sending the correct startSample and sectionLength
                    'CheckAndCorrectSectionLength(InputSound.WaveData.ShortestChannelSampleCount, StartSample, SectionLength)

                    'Checks that kernel size is not larger than fftSize, increases fftSize is that is the case
                    If KernelSize > FftFormat.FftWindowSize Then
                        CheckAndAdjustFFTSize(FftFormat.FftWindowSize, KernelSize)
                    End If

                    'Main section
                    'Makes sure that windowSize is always an even number
                    If FftFormat.FftWindowSize Mod 2 = 1 Then FftFormat.FftWindowSize += 1

                    Dim WindowCount As Integer = Int((SectionLength - SectionLengths) / StepSize) 'Ignoring any final bit that goes outside the sample array

                    Dim StartReadSample As Integer = StartSample
                    For w = 0 To WindowCount - 1

                        'Getting the current section
                        Dim WindowSound = DSP.CopySection(InputSound, StartReadSample, SectionLengths)

                        'Getting a IR based on the current section
                        Dim IR As Sound = GetImpulseResponseFromSound(WindowSound, FftFormat, KernelSize, Channel)

                        If IR IsNot Nothing Then Output.Add(IR)

                        StartReadSample += StepSize

                    Next

                    Return Output.ToArray

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Function

            Public Function CreateIrArrayFilteredNoise(ByRef IrArray() As Sound,
                                                   ByRef StepSize As Integer, ByRef SectionLengths As Integer,
                                                   ByRef FftFormat As Formats.FftFormat,
                                                   Optional ByVal Channel As Integer? = Nothing,
                                                   Optional ByVal FadeSkewCosinePower As Double = 10) As Sound

                If IrArray.Length = 0 Then Return Nothing
                If IrArray(0) Is Nothing Then Return Nothing

                Dim OverlapLength As Integer = SectionLengths - StepSize

                Dim TempWaveFormat As Formats.WaveFormat = IrArray(0).WaveFormat 'Using the wave format of the first IR-sound

                Dim TempSounds As New List(Of Sound)

                Dim RandomSource As New Random

                For w = 0 To IrArray.Length - 1

                    'Creating a noise
                    Dim CurrentNoise As Sound = GenerateSound.CreateWhiteNoise(TempWaveFormat,, 0.1, SectionLengths, TimeUnits.samples, RandomSource)

                    'Filterring the sound
                    CurrentNoise = DSP.FIRFilter(CurrentNoise, IrArray(w), FftFormat, Channel,,,,,, True)

                    'Storing the new sound
                    TempSounds.Add(CurrentNoise)

                Next

                'Concatenating the sounds
                Dim Output = DSP.ConcatenateSounds(TempSounds,,,,,, OverlapLength, True, FadeSkewCosinePower)

                Return Output

            End Function

            Public Function GetTemporoSpectrallyModulatedNoiseFromFile(ByVal InputSound As Sound,
                                                                   Optional ByVal SectionDuration As Double = 0.2,
                                                                   Optional ByVal StepDuration As Double = 0.1,
                                                                   Optional ByVal KernelSize As Integer = 512,
                                                                   Optional ByVal FadeSkewCosinePower As Double = 10,
                                                                   Optional ByRef FftFormat As Formats.FftFormat = Nothing) As Sound

                'Setting up a suitable fft format
                If FftFormat Is Nothing Then FftFormat = New Formats.FftFormat

                Dim StepSize As Integer = InputSound.WaveFormat.SampleRate * StepDuration
                Dim SectionLengths As Integer = InputSound.WaveFormat.SampleRate * SectionDuration
                Dim IrArray = GetIrArrayFromSound(InputSound, StepSize, SectionLengths, FftFormat, KernelSize)
                Dim Output = CreateIrArrayFilteredNoise(IrArray, StepSize, SectionLengths, FftFormat,, FadeSkewCosinePower)

                Return Output

            End Function

            Public Function GetSpectrallyModulatedNoiseFromFile(ByVal InputSound As Sound,
                                                                   Optional ByVal KernelSize As Integer = 512,
                                                            Optional ByVal CrossFadeLength As Integer = 0,
                                                           Optional ByVal ValidateSoundLevel As Boolean = True) As Sound

                'Measuring input sound level (channel 1 only)
                Dim InputLevel As Double
                If ValidateSoundLevel Then InputLevel = DSP.MeasureSectionLevel(InputSound, 1)

                'Setting up a suitable fft format
                Dim FftFormat As New Formats.FftFormat()
                Dim Ir = GetImpulseResponseFromSound(InputSound, FftFormat, KernelSize)

                'Creating a noise
                Dim Output As Sound = GenerateSound.CreateWhiteNoise(InputSound.WaveFormat,, 0.1, InputSound.WaveData.ShortestChannelSampleCount + CrossFadeLength, TimeUnits.samples)

                'Filterring the sound
                Output = DSP.FIRFilter(Output, Ir, FftFormat,,,,,, True, True)


                If ValidateSoundLevel Then DSP.MeasureAndAdjustSectionLevel(Output, InputLevel)

                Return Output

            End Function

            ''' <summary>
            ''' Creates a noise by making multiple overlays of the same inputs sounds.
            ''' </summary>
            ''' <param name="InputSounds"></param>
            ''' <param name="RandomizeSoundIntervals"></param>
            ''' <returns></returns>
            Public Function CreateOverlayNoise(ByRef InputSounds As List(Of Sound),
                                           ByRef NumberOfOverlays As Integer,
                                           ByVal MinimumOutputSoundDuration As Double,
                                           Optional ByVal RandomizeSoundOrder As Boolean = True,
                                           Optional ByVal RandomizeSoundIntervals As Boolean = True) As Sound

                Try

                    If InputSounds.Count = 0 Then Return Nothing
                    If InputSounds(0) Is Nothing Then Return Nothing

                    'Getting the wave format
                    Dim WaveFormat As Formats.WaveFormat = InputSounds(0).WaveFormat

                    Dim Rnd As New Random

                    Dim ConcatenatedSounds As New List(Of Sound)

                    For n = 0 To NumberOfOverlays - 1

                        Dim CurrentAddedDuration As Double = 0

                        'Making a local sound order (randomization) 
                        Dim OriginalOrderSounds As New List(Of Sound)
                        For Each Sound In InputSounds
                            OriginalOrderSounds.Add(Sound)
                        Next

                        Dim SoundsInOrderToUse As New List(Of Sound)
                        If RandomizeSoundOrder = True Then
                            Do Until OriginalOrderSounds.Count = 0
                                Dim RandomIndex As Integer = Rnd.Next(OriginalOrderSounds.Count)
                                SoundsInOrderToUse.Add(OriginalOrderSounds(RandomIndex))
                                OriginalOrderSounds.RemoveAt(RandomIndex)
                            Loop
                        Else
                            SoundsInOrderToUse = OriginalOrderSounds
                        End If


                        Dim SoundsToConcatenate As New List(Of Sound)

                        Dim MaxInterval As Double
                        If RandomizeSoundIntervals = True Then
                            MaxInterval = 1000 'In milliseconds
                        Else
                            MaxInterval = 100 'In milliseconds
                        End If
                        Dim MinInterval As Double = 100 'In milliseconds

                        'Inserting silent sounds between each sound
                        For Each Sound In SoundsInOrderToUse
                            SoundsToConcatenate.Add(Sound)
                            SoundsToConcatenate.Add(GenerateSound.CreateSilence(WaveFormat,, Rnd.Next(MinInterval, MaxInterval + 1) / 1000))

                            'Increasing the added lengths
                            CurrentAddedDuration +=
                        (SoundsToConcatenate(SoundsToConcatenate.Count - 1).WaveData.ShortestChannelSampleCount +
                        SoundsToConcatenate(SoundsToConcatenate.Count - 2).WaveData.ShortestChannelSampleCount) /
                        WaveFormat.SampleRate

                            'Stopping if desired length is aquired
                            If CurrentAddedDuration >= MinimumOutputSoundDuration Then Exit For

                        Next

                        'Concatenating sounds
                        Dim ConcatenatedSound = DSP.ConcatenateSounds(SoundsToConcatenate,,,,,, WaveFormat.SampleRate * (MinInterval / 1000),,, True)
                        ConcatenatedSounds.Add(ConcatenatedSound)

                    Next

                    'Mixing the sounds to a single sound
                    Dim OutputSound As New Sound(WaveFormat)

                    'Getting the minimum sound length
                    Dim MinSoundLength As Integer = Integer.MaxValue
                    For Each Sound In ConcatenatedSounds
                        MinSoundLength = Math.Min(MinSoundLength, Sound.WaveData.ShortestChannelSampleCount)
                    Next

                    'Creating channel arrays
                    For c = 1 To WaveFormat.Channels
                        Dim ChannelSampleArray(MinSoundLength - 1) As Single
                        OutputSound.WaveData.SampleData(c) = ChannelSampleArray
                    Next

                    'Mixing the sounds
                    'Adding all samples, and applying attenuation for equal power
                    Dim NumberOfSounds As Integer = ConcatenatedSounds.Count
                    Dim AttenuationFactor As Double = Math.Sqrt(1 / NumberOfSounds)
                    For c = 1 To WaveFormat.Channels
                        For s = 0 To MinSoundLength - 1
                            For Each Sound In ConcatenatedSounds
                                OutputSound.WaveData.SampleData(c)(s) += Sound.WaveData.SampleData(c)(s) * AttenuationFactor
                            Next
                        Next
                    Next

                    'Returning the output sound
                    Return OutputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function


            ''' <summary>
            ''' Creates a noise by making multiple overlays of the same input sound.
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <returns></returns>
            Public Function CreateOverlayNoise(ByRef InputSound As Sound,
                                           ByRef NumberOfOverlays As Integer,
                                           ByVal OutputSoundDuration As Double) As Sound

                Try

                    If InputSound Is Nothing Then Return Nothing

                    'Getting the wave format
                    Dim WaveFormat As Formats.WaveFormat = InputSound.WaveFormat

                    'Extending the input sound to twice the length of MinimumOutputSoundDuration, by reduplicating the sound
                    Dim NumberOfRepetitionsNeeded As Integer = Math.Ceiling(
                        OutputSoundDuration / ((InputSound.WaveData.ShortestChannelSampleCount / WaveFormat.SampleRate) - 0.3)) + 1 'The margin -0.3 sec is used to compensate for the overlap in ConcatenateSounds

                    Dim ExtendedSoundList As New List(Of Sound)
                    For r = 1 To NumberOfRepetitionsNeeded
                        ExtendedSoundList.Add(InputSound.CreateCopy)
                    Next
                    Dim ExtendedSound = DSP.ConcatenateSounds(ExtendedSoundList,,,, False,, 0.1 * WaveFormat.SampleRate)


                    'Creating an output sound
                    Dim OutputSound As New Sound(WaveFormat)

                    Dim OutputSoundLength As Integer = WaveFormat.SampleRate * OutputSoundDuration

                    'Creating channel arrays
                    For c = 1 To WaveFormat.Channels
                        Dim ChannelSampleArray(OutputSoundLength - 1) As Single
                        OutputSound.WaveData.SampleData(c) = ChannelSampleArray
                    Next

                    'Adding all samples, and applying attenuation for equal power
                    Dim AttenuationFactor As Double = Math.Sqrt(1 / NumberOfOverlays)

                    Dim Rnd As New Random

                    'Copying and adding sound series with randomized start sample
                    For n = 0 To NumberOfOverlays - 1

                        Dim StartSample As Integer = Rnd.Next(0, InputSound.WaveData.ShortestChannelSampleCount)

                        For c = 1 To WaveFormat.Channels
                            For s = 0 To OutputSoundLength - 1
                                OutputSound.WaveData.SampleData(c)(s) += ExtendedSound.WaveData.SampleData(c)(StartSample + s) * AttenuationFactor
                            Next
                        Next
                    Next


                    'Returning the output sound
                    Return OutputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function


        End Module



    End Namespace

    Namespace PlayBack

        Public Interface ISoundPlayerControl
            Sub MessageFromPlayer(ByRef Message As MessagesFromSoundPlayer)

            Enum MessagesFromSoundPlayer
                EndOfSound
                ApproachingEndOfBufferAlert
                NewBufferTick
            End Enum

        End Interface


        Public Module Play

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="Outputsound"></param>
            ''' <param name="startSample"></param>
            ''' <param name="length"></param>
            ''' <param name="Level">If level (in dBFS) is set, a copy of the original sound is created, for which the level is adjusted to the intended level before playing the sound. If left to nothing, the original unmodified sound will be played. (Any value set for NormalizedLevel will override the value set for Level.)</param>
            ''' <param name="NormalizedLevel">If level (in dBFS) is set, a copy of the original sound is created, for which the level is normalized so that the loudest 200 ms of the section is set to Level. If left to nothing, the original unmodified sound will be played.</param>
            Public Sub PlayDuplexSoundStream(ByRef SoundPlayer As PortAudioVB.SoundPlayer,
                                              ByRef Outputsound As Sound,
                                              Optional ByRef startSample As Long = 0,
                                              Optional ByRef length As Long = -1,
                                              Optional Level As Double? = Nothing,
                                              Optional NormalizedLevel As Double? = Nothing,
                                              Optional ByVal OutputSoundFadeInTime As Double = 0.1,
                                              Optional ByVal OutputSoundFadeOutTime As Double = 0.1)
                Try

                    If SoundPlayer Is Nothing Then
                        Throw New ArgumentException("SoundPlayer is not initialized...")
                    End If

                    'Setting the sound player output sound
                    SoundPlayer.SetNewOutputSound(Outputsound)

                    'Makes sure that a sound stream is open
                    If SoundPlayer.IsStreamOpen = True Then SoundPlayer.CloseStream()
                    If SoundPlayer.IsStreamOpen = False Then SoundPlayer.OpenStream()

                    'If no length is specified the file is played to the end
                    If length < 1 Then length = Outputsound.WaveData.SampleData(1).Length - startSample

                    'Setting level
                    If NormalizedLevel IsNot Nothing Or Level IsNot Nothing Then

                        'Copying th output sound
                        Dim OutputSoundCopy As Sound = Outputsound.CreateCopy

                        'Adjusting the sound level of the copy
                        If NormalizedLevel IsNot Nothing Then 'Doing primarily NormalizedLevel adjustment, and only Level adjustment if NormalizedLevel is Nothing.
                            If Not DSP.TimeAndFrequencyWeightedNormalization(OutputSoundCopy,,,, NormalizedLevel) = True Then ', OutputSoundCopy.SMA.SoundLevelFormat.TemporalIntegrationDuration, ) = True Then
                                MsgBox("Distorsion")
                            End If
                        ElseIf Level IsNot Nothing Then
                            DSP.MeasureAndAdjustSectionLevel(OutputSoundCopy, Level, , startSample, length)
                        End If

                        'Exchanging the sound to the copy
                        SoundPlayer.SetNewOutputSound(OutputSoundCopy)

                    End If

                    'Plays the sound
                    SoundPlayer.Start(startSample, length,, OutputSoundFadeInTime, OutputSoundFadeOutTime, True)

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub

        End Module

    End Namespace
    'End Namespace


    Namespace PortAudioVB

        Public Class SoundPlayer
            Implements IDisposable

            Public ReadOnly UseBlockingIO As Boolean

            'Declaration of BLOCKING STUFF
            Private WithEvents BlockingTimer As New System.Windows.Forms.Timer With {.Interval = 1}
            Dim BlockingReadWrite_Active As Boolean = False


            'Declaration of CALLBACK STUFF
            Private callbackBuffer As Single() = New Single(511) {}
            Private SilentBuffer As Single() = New Single(511) {}
            Private paStreamCallback As PortAudio.PaStreamCallbackDelegate = Function(input As IntPtr, output As IntPtr, frameCount As UInteger, ByRef timeInfo As PortAudio.PaStreamCallbackTimeInfo, statusFlags As PortAudio.PaStreamCallbackFlags, userData As IntPtr) As PortAudio.PaStreamCallbackResult

                                                                                 'log("Callback called")
                                                                                 'log("  time: " & timeInfo.currentTime)
                                                                                 'log("  inputBufferAdcTime: " & timeInfo.inputBufferAdcTime)
                                                                                 'log("  outputBufferDacTime:  " & timeInfo.outputBufferDacTime)
                                                                                 'log("  statusFlags: " & statusFlags)
                                                                                 'log("CurrentStartFrameIndex: " & _Position & " FrameCount: " & frameCount & " bufferSize: " & audioApiSettings.DatapointsPerBuffer)

                                                                                 'INPUT SOUND

                                                                                 If DoSoundRecording = True Then
                                                                                     'Getting input sound
                                                                                     Dim InputBuffer(AudioApiSettings.FramesPerBuffer * RecordingSoundFormat.Channels - 1) As Single
                                                                                     Marshal.Copy(input, InputBuffer, 0, AudioApiSettings.FramesPerBuffer * RecordingSoundFormat.Channels)
                                                                                     InputBufferHistory.Add(InputBuffer)
                                                                                 End If


                                                                                 'OUTPUT SOUND
                                                                                 If DoSoundOutput = True Then

                                                                                     If EndOfOutputSoundIsReached = True Then
                                                                                         'Exporting Silence
                                                                                         Marshal.Copy(SilentBuffer, 0, output, AudioApiSettings.FramesPerBuffer * OutputSound.WaveFormat.Channels)
                                                                                         Return PortAudio.PaStreamCallbackResult.paContinue
                                                                                     End If

                                                                                     For j As Integer = 0 To frameCount - 1

                                                                                         'Reading the different channels
                                                                                         For Ch = 0 To OutputSound.WaveFormat.Channels - 1

                                                                                             'Checking if there is any more sound to be played
                                                                                             If _Position + (j * OutputSound.WaveFormat.Channels) + Ch < _OutputSound.WaveData.SampleData(Ch + 1).Length Then ''SpeechAndHearingTools Audio channels indices are 1-based
                                                                                                 callbackBuffer((j * OutputSound.WaveFormat.Channels) + Ch) = _OutputSound.WaveData.SampleData(Ch + 1)(_Position + j)
                                                                                             Else
                                                                                                 'Marks that the end of the output sound has been reached
                                                                                                 EndOfOutputSoundIsReached = True

                                                                                                 'Fills up the buffer with zeroes, if the end of the sound is reached
                                                                                                 callbackBuffer((j * OutputSound.WaveFormat.Channels) + Ch) = 0
                                                                                             End If
                                                                                         Next

                                                                                     Next

                                                                                     'Increasing position
                                                                                     _Position += frameCount

                                                                                     Marshal.Copy(callbackBuffer, 0, output, AudioApiSettings.FramesPerBuffer * OutputSound.WaveFormat.Channels)

                                                                                     If EndOfOutputSoundIsReached = True Then
                                                                                         'log("End of sound is reached at sample position: " & _Position)

                                                                                         If StopAtOutputSoundEnd = True Then

                                                                                             Log("Sound was stopped at end of sound, at sample position: " & _Position)

                                                                                             Return PortAudio.PaStreamCallbackResult.paComplete

                                                                                         End If
                                                                                     End If
                                                                                 End If

                                                                                 Return PortAudio.PaStreamCallbackResult.paContinue
                                                                             End Function

            Private paStreamCallbackFadeEnabled As PortAudio.PaStreamCallbackDelegate = Function(input As IntPtr, output As IntPtr, frameCount As UInteger, ByRef timeInfo As PortAudio.PaStreamCallbackTimeInfo, statusFlags As PortAudio.PaStreamCallbackFlags, userData As IntPtr) As PortAudio.PaStreamCallbackResult

                                                                                            'INPUT SOUND
                                                                                            If DoSoundRecording = True Then
                                                                                                'Getting input sound
                                                                                                Dim InputBuffer(AudioApiSettings.FramesPerBuffer * RecordingSoundFormat.Channels - 1) As Single
                                                                                                Marshal.Copy(input, InputBuffer, 0, AudioApiSettings.FramesPerBuffer * RecordingSoundFormat.Channels)
                                                                                                InputBufferHistory.Add(InputBuffer)
                                                                                            End If

                                                                                            'OUTPUT SOUND
                                                                                            If DoSoundOutput = True Then

                                                                                                If EndOfOutputSoundIsReached = True Then
                                                                                                    'Exporting Silence
                                                                                                    Marshal.Copy(SilentBuffer, 0, output, AudioApiSettings.FramesPerBuffer * OutputSound.WaveFormat.Channels)
                                                                                                    Return PortAudio.PaStreamCallbackResult.paContinue
                                                                                                End If


                                                                                                'Exporting sound data from the OutputSound
                                                                                                For j As Integer = 0 To frameCount - 1

                                                                                                    'Calculating the fading factor for the current sample (fading factor is not channel specific)

                                                                                                    'Setting a default non-fading factor
                                                                                                    OutputSoundFadeFactor = 1

                                                                                                    'Checking if fading in should be done
                                                                                                    If OutputSoundFadeInCurrentSample < OutputSoundFadeInSamples Then

                                                                                                        'Do fade in
                                                                                                        'Calculating current fade in factor, and multiplying the default fade factor by it
                                                                                                        OutputSoundFadeFactor *= OutputSoundFadeInCurrentSample / OutputSoundFadeInSamples
                                                                                                        'Increasing current fade in sample index
                                                                                                        OutputSoundFadeInCurrentSample += 1

                                                                                                    End If

                                                                                                    'Checking if fading out should be done
                                                                                                    If OutputSoundFadeOutSamples > 0 Then

                                                                                                        'Do fade out
                                                                                                        'Calculating current fade out factor, and multiplying the current fade factor by it (the current fade factor could either be 1 or a fade in factor. This allowes both fade in and fade out being active at the same time point)
                                                                                                        If _Position > OutputSoundFadeOutStartSample Then
                                                                                                            OutputSoundFadeFactor *= (OutputSoundStopSample - _Position) / OutputSoundFadeOutSamples

                                                                                                            If _Position > OutputSoundStopSample Then
                                                                                                                OutputSoundFadeFactor = 0
                                                                                                            End If

                                                                                                        End If
                                                                                                    End If


                                                                                                    'Reading the different channels
                                                                                                    For Ch = 0 To OutputSound.WaveFormat.Channels - 1

                                                                                                        'Checking if there is any more sound to be played
                                                                                                        If _Position < OutputSoundStopSample Then
                                                                                                            callbackBuffer((j * OutputSound.WaveFormat.Channels) + Ch) = _OutputSound.WaveData.SampleData(Ch + 1)(_Position)
                                                                                                        Else
                                                                                                            'Marks that the end of the output sound has been reached
                                                                                                            EndOfOutputSoundIsReached = True

                                                                                                            'Fills up the buffer with zeroes, if the end of the sound is reached
                                                                                                            callbackBuffer((j * OutputSound.WaveFormat.Channels) + Ch) = 0
                                                                                                        End If
                                                                                                    Next

                                                                                                    'Increasing position
                                                                                                    _Position += 1

                                                                                                Next


                                                                                                Marshal.Copy(callbackBuffer, 0, output, AudioApiSettings.FramesPerBuffer * OutputSound.WaveFormat.Channels)

                                                                                                If EndOfOutputSoundIsReached = True Then
                                                                                                    If StopAtOutputSoundEnd = True Then

                                                                                                        'log("Sound was stopped at end of sound, at sample position: " & _Position)
                                                                                                        Return PortAudio.PaStreamCallbackResult.paComplete

                                                                                                    End If
                                                                                                End If
                                                                                            End If

                                                                                            Return PortAudio.PaStreamCallbackResult.paContinue
                                                                                        End Function

            Private paStreamCallback_InputOnly As PortAudio.PaStreamCallbackDelegate = Function(input As IntPtr, output As IntPtr, frameCount As UInteger, ByRef timeInfo As PortAudio.PaStreamCallbackTimeInfo, statusFlags As PortAudio.PaStreamCallbackFlags, userData As IntPtr) As PortAudio.PaStreamCallbackResult

                                                                                           'INPUT SOUND
                                                                                           If DoSoundRecording = True Then
                                                                                               'Getting input sound
                                                                                               Dim InputBuffer(AudioApiSettings.FramesPerBuffer * RecordingSoundFormat.Channels - 1) As Single
                                                                                               Marshal.Copy(input, InputBuffer, 0, AudioApiSettings.FramesPerBuffer * RecordingSoundFormat.Channels)
                                                                                               InputBufferHistory.Add(InputBuffer)
                                                                                           End If
                                                                                           Return PortAudio.PaStreamCallbackResult.paContinue
                                                                                       End Function

            Private paStreamCallback_OutputOnly As PortAudio.PaStreamCallbackDelegate = Function(input As IntPtr, output As IntPtr, frameCount As UInteger, ByRef timeInfo As PortAudio.PaStreamCallbackTimeInfo, statusFlags As PortAudio.PaStreamCallbackFlags, userData As IntPtr) As PortAudio.PaStreamCallbackResult

                                                                                            'OUTPUT SOUND
                                                                                            If EndOfOutputSoundIsReached = True Then
                                                                                                'Exporting Silence
                                                                                                Marshal.Copy(SilentBuffer, 0, output, AudioApiSettings.FramesPerBuffer * OutputSound.WaveFormat.Channels)
                                                                                                Return PortAudio.PaStreamCallbackResult.paContinue
                                                                                            End If

                                                                                            For j As Integer = 0 To frameCount - 1

                                                                                                'Reading the different channels
                                                                                                For Ch = 0 To OutputSound.WaveFormat.Channels - 1

                                                                                                    'Checking if there is any more sound to be played
                                                                                                    If _Position + (j * OutputSound.WaveFormat.Channels) + Ch < _OutputSound.WaveData.SampleData(Ch + 1).Length Then ''SpeechAndHearingTools Audio channels indices are 1-based
                                                                                                        callbackBuffer((j * OutputSound.WaveFormat.Channels) + Ch) = _OutputSound.WaveData.SampleData(Ch + 1)(_Position + j)
                                                                                                    Else
                                                                                                        'Marks that the end of the output sound has been reached
                                                                                                        EndOfOutputSoundIsReached = True

                                                                                                        'Fills up the buffer with zeroes, if the end of the sound is reached
                                                                                                        callbackBuffer((j * OutputSound.WaveFormat.Channels) + Ch) = 0
                                                                                                    End If
                                                                                                Next

                                                                                            Next

                                                                                            'Increasing position
                                                                                            _Position += frameCount

                                                                                            Marshal.Copy(callbackBuffer, 0, output, AudioApiSettings.FramesPerBuffer * OutputSound.WaveFormat.Channels)

                                                                                            If EndOfOutputSoundIsReached = True Then
                                                                                                'log("End of sound is reached at sample position: " & _Position)

                                                                                                If StopAtOutputSoundEnd = True Then

                                                                                                    Log("Sound was stopped at end of sound, at sample position: " & _Position)

                                                                                                    Return PortAudio.PaStreamCallbackResult.paComplete

                                                                                                End If
                                                                                            End If

                                                                                            Return PortAudio.PaStreamCallbackResult.paContinue
                                                                                        End Function


            Private paStreamCallback_OutputOnly_FadeEnabled As PortAudio.PaStreamCallbackDelegate = Function(input As IntPtr, output As IntPtr, frameCount As UInteger, ByRef timeInfo As PortAudio.PaStreamCallbackTimeInfo, statusFlags As PortAudio.PaStreamCallbackFlags, userData As IntPtr) As PortAudio.PaStreamCallbackResult

                                                                                                        'OUTPUT SOUND
                                                                                                        If EndOfOutputSoundIsReached = True Then
                                                                                                            'Exporting Silence
                                                                                                            Marshal.Copy(SilentBuffer, 0, output, AudioApiSettings.FramesPerBuffer * OutputSound.WaveFormat.Channels)
                                                                                                            Return PortAudio.PaStreamCallbackResult.paContinue
                                                                                                        End If


                                                                                                        'Exporting sound data from the OutputSound
                                                                                                        For j As Integer = 0 To frameCount - 1

                                                                                                            'Calculating the fading factor for the current sample (fading factor is not channel specific)

                                                                                                            'Setting a default non-fading factor
                                                                                                            OutputSoundFadeFactor = 1

                                                                                                            'Checking if fading in should be done
                                                                                                            If OutputSoundFadeInCurrentSample < OutputSoundFadeInSamples Then

                                                                                                                'Do fade in
                                                                                                                'Calculating current fade in factor, and multiplying the default fade factor by it
                                                                                                                OutputSoundFadeFactor *= OutputSoundFadeInCurrentSample / OutputSoundFadeInSamples
                                                                                                                'Increasing current fade in sample index
                                                                                                                OutputSoundFadeInCurrentSample += 1

                                                                                                            End If

                                                                                                            'Checking if fading out should be done
                                                                                                            If OutputSoundFadeOutSamples > 0 Then

                                                                                                                'Do fade out
                                                                                                                'Calculating current fade out factor, and multiplying the current fade factor by it (the current fade factor could either be 1 or a fade in factor. This allowes both fade in and fade out being active at the same time point)
                                                                                                                If _Position > OutputSoundFadeOutStartSample Then
                                                                                                                    OutputSoundFadeFactor *= (OutputSoundStopSample - _Position) / OutputSoundFadeOutSamples

                                                                                                                    If _Position > OutputSoundStopSample Then
                                                                                                                        OutputSoundFadeFactor = 0
                                                                                                                    End If

                                                                                                                End If
                                                                                                            End If


                                                                                                            'Reading the different channels
                                                                                                            For Ch = 0 To OutputSound.WaveFormat.Channels - 1

                                                                                                                'Checking if there is any more sound to be played
                                                                                                                If _Position < OutputSoundStopSample Then
                                                                                                                    callbackBuffer((j * OutputSound.WaveFormat.Channels) + Ch) = _OutputSound.WaveData.SampleData(Ch + 1)(_Position)
                                                                                                                Else
                                                                                                                    'Marks that the end of the output sound has been reached
                                                                                                                    EndOfOutputSoundIsReached = True

                                                                                                                    'Fills up the buffer with zeroes, if the end of the sound is reached
                                                                                                                    callbackBuffer((j * OutputSound.WaveFormat.Channels) + Ch) = 0
                                                                                                                End If
                                                                                                            Next

                                                                                                            'Increasing position
                                                                                                            _Position += 1

                                                                                                        Next


                                                                                                        Marshal.Copy(callbackBuffer, 0, output, AudioApiSettings.FramesPerBuffer * OutputSound.WaveFormat.Channels)

                                                                                                        If EndOfOutputSoundIsReached = True Then
                                                                                                            If StopAtOutputSoundEnd = True Then

                                                                                                                'log("Sound was stopped at end of sound, at sample position: " & _Position)
                                                                                                                Return PortAudio.PaStreamCallbackResult.paComplete

                                                                                                            End If
                                                                                                        End If

                                                                                                        Return PortAudio.PaStreamCallbackResult.paContinue
                                                                                                    End Function

            Private paStreamCallback_OutputOnly_FadeEnabled_OLD As PortAudio.PaStreamCallbackDelegate = Function(input As IntPtr, output As IntPtr, frameCount As UInteger, ByRef timeInfo As PortAudio.PaStreamCallbackTimeInfo, statusFlags As PortAudio.PaStreamCallbackFlags, userData As IntPtr) As PortAudio.PaStreamCallbackResult

                                                                                                            'OUTPUT SOUND
                                                                                                            If EndOfOutputSoundIsReached = True Then
                                                                                                                'Exporting Silence
                                                                                                                Marshal.Copy(SilentBuffer, 0, output, AudioApiSettings.FramesPerBuffer * OutputSound.WaveFormat.Channels)
                                                                                                                Return PortAudio.PaStreamCallbackResult.paContinue
                                                                                                            End If


                                                                                                            'Exporting sound data from the OutputSound
                                                                                                            For j As Integer = 0 To frameCount - 1

                                                                                                                'Calculating the fading factor for the current sample (fading factor is not channel specific)

                                                                                                                'Setting a default non-fading factor
                                                                                                                OutputSoundFadeFactor = 1

                                                                                                                'Checking if fading in should be done
                                                                                                                If OutputSoundFadeInCurrentSample < OutputSoundFadeInSamples Then

                                                                                                                    'Do fade in
                                                                                                                    'Calculating current fade in factor, and multiplying the default fade factor by it
                                                                                                                    OutputSoundFadeFactor *= OutputSoundFadeInCurrentSample / OutputSoundFadeInSamples
                                                                                                                    'Increasing current fade in sample index
                                                                                                                    OutputSoundFadeInCurrentSample += 1

                                                                                                                End If

                                                                                                                'Checking if fading out should be done
                                                                                                                If OutputSoundFadeOutSamples > 0 Then

                                                                                                                    'Do fade out
                                                                                                                    'Calculating current fade out factor, and multiplying the current fade factor by it (the current fade factor could either be 1 or a fade in factor. This allowes both fade in and fade out being active at the same time point)
                                                                                                                    If _Position > OutputSoundFadeOutStartSample Then
                                                                                                                        OutputSoundFadeFactor *= (OutputSoundStopSample - _Position) / OutputSoundFadeOutSamples

                                                                                                                        If _Position > OutputSoundStopSample Then
                                                                                                                            OutputSoundFadeFactor = 0
                                                                                                                        End If

                                                                                                                    End If
                                                                                                                End If


                                                                                                                'Reading the different channels
                                                                                                                For Ch = 0 To OutputSound.WaveFormat.Channels - 1

                                                                                                                    'Checking if there is any more sound to be played
                                                                                                                    If _Position < OutputSoundStopSample Then
                                                                                                                        callbackBuffer((j * OutputSound.WaveFormat.Channels) + Ch) = _OutputSound.WaveData.SampleData(Ch + 1)(_Position)
                                                                                                                    Else
                                                                                                                        'Marks that the end of the output sound has been reached
                                                                                                                        EndOfOutputSoundIsReached = True

                                                                                                                        'Fills up the buffer with zeroes, if the end of the sound is reached
                                                                                                                        callbackBuffer((j * OutputSound.WaveFormat.Channels) + Ch) = 0
                                                                                                                    End If
                                                                                                                Next

                                                                                                                'Increasing position
                                                                                                                _Position += 1

                                                                                                            Next


                                                                                                            Marshal.Copy(callbackBuffer, 0, output, AudioApiSettings.FramesPerBuffer * OutputSound.WaveFormat.Channels)

                                                                                                            If EndOfOutputSoundIsReached = True Then
                                                                                                                If StopAtOutputSoundEnd = True Then

                                                                                                                    'log("Sound was stopped at end of sound, at sample position: " & _Position)
                                                                                                                    Return PortAudio.PaStreamCallbackResult.paComplete

                                                                                                                End If
                                                                                                            End If

                                                                                                            Return PortAudio.PaStreamCallbackResult.paContinue
                                                                                                        End Function

            'OTHER DECLATATIONS

            Property _Position As Long
            Property Position As Long
                Private Set(value As Long)
                    _Position = value
                End Set
                Get
                    Return _Position
                End Get
            End Property

            Private _IsInitialized As Boolean = False
            Public ReadOnly Property IsInitialized As Boolean
                Get
                    Return _IsInitialized
                End Get
            End Property

            Private _OutputSound As Sound
            Public ReadOnly Property OutputSound As Sound
                Get
                    Return _OutputSound
                End Get
            End Property

            'Private CurrentInputSound As Sound
            'Public ReadOnly Property InputSound As Sound
            'Get
            'Return CurrentInputSound
            'End Get
            'End Property

            Private InputBufferHistory As New List(Of Single())

            Property _RecordingSoundFormat As Formats.WaveFormat
            Property RecordingSoundFormat As Formats.WaveFormat
                Private Set(value As Formats.WaveFormat)
                    _RecordingSoundFormat = value
                End Set
                Get
                    Return _RecordingSoundFormat
                End Get
            End Property

            Public StopAtOutputSoundEnd As Boolean
            Public EndOfOutputSoundIsReached As Boolean = False

            Private _AudioApiSettings As AudioApiSettings
            Property AudioApiSettings As AudioApiSettings
                Private Set(value As AudioApiSettings)
                    _AudioApiSettings = value
                End Set
                Get
                    Return _AudioApiSettings
                End Get
            End Property

            Private stream As IntPtr
            Private disposed As Boolean = False

            Private Shared m_messagesEnabled As Boolean = False
            Public Shared Property MessagesEnabled() As Boolean
                Get
                    Return m_messagesEnabled
                End Get
                Set
                    m_messagesEnabled = Value
                End Set
            End Property

            Private Shared m_loggingEnabled As Boolean = False
            Public Shared Property LoggingEnabled() As Boolean
                Get
                    Return m_loggingEnabled
                End Get
                Set
                    m_loggingEnabled = Value
                End Set
            End Property

            Private _IsPlaying As Boolean = False
            Public ReadOnly Property IsPlaying As Boolean
                Get
                    Return _IsPlaying
                End Get
            End Property

            Private _IsStreamOpen As Boolean = False
            Public ReadOnly Property IsStreamOpen As Boolean
                Get
                    Return _IsStreamOpen
                End Get
            End Property

            Private _HasSoundOutput As Boolean = False
            Public ReadOnly Property HasSoundOutput As Boolean
                Get
                    Return _HasSoundOutput
                End Get
            End Property

            Private _HasSoundInput As Boolean = False
            Public ReadOnly Property HasSoundInput As Boolean
                Get
                    Return _HasSoundInput
                End Get
            End Property

            Private _IsClippingInactivated As Boolean = False
            Public ReadOnly Property IsClippingInactivated As Boolean
                Get
                    Return _IsClippingInactivated
                End Get
            End Property

            'Private _LongestRecordingDuration As Boolean = False
            'Public ReadOnly Property LongestRecordingDuration As Boolean
            'Get
            'Return _LongestRecordingDuration
            'End Get
            'End Property


            Private _AutoAdjustSampleRateToOutputSound As Boolean = False
            Public ReadOnly Property AutoAdjustSampleRateToOutputSound As Boolean
                Get
                    Return _AutoAdjustSampleRateToOutputSound
                End Get
            End Property


            'Fading and play length
            Private OutputSoundFadeInSamples As Integer
            Private OutputSoundFadeInCurrentSample As Integer
            Private OutputSoundFadeFactor As Single = 0

            Private OutputSoundStopSample As Long = 0
            Private OutputSoundFadeOutSamples As Integer = 0
            Private OutputSoundFadeOutStartSample As Long = 0

            Private CloseStreamAfterPlayCompletion As Boolean = False

            Public Property FadeEnabledCallback As Boolean


            Public Sub New(ByVal UseBlockingIO As Boolean,
                       Optional ByVal RecordingSoundFormat As Formats.WaveFormat = Nothing,
                              Optional ByVal OutputSound As Sound = Nothing,
                              Optional ByRef AudioApiSettings As AudioApiSettings = Nothing,
                              Optional ByVal LoggingEnabled As Boolean = False,
                       Optional ByVal MessagesEnabled As Boolean = False,
                        Optional StopAtOutputSoundEnd As Boolean = True,
                              Optional AutoAdjustSampleRateToOutputSound As Boolean = True,
                       Optional InactivateClipping As Boolean = False,
                       Optional FadeEnabledCallback As Boolean = True)

                'Optional LongestRecordingDuration As Integer = 60 * 60)
                '        ''' <param name="LongestRecordingDuration">Sets an upper limit on recording time (seconds) when callback function is used. Higher values will consume more memory.</param>

                Me.FadeEnabledCallback = FadeEnabledCallback

                'Setting _HasSoundOutput and _HasSoundInput
                If RecordingSoundFormat IsNot Nothing Then _HasSoundInput = True
                If OutputSound IsNot Nothing Then _HasSoundOutput = True

                'Aborting if neither of _HasSoundOutput or _HasSoundInput is true
                If _HasSoundOutput = False And _HasSoundInput = False Then
                    Throw New ArgumentException("Both RecordingSoundFormat and OutputSound cannot be NULL.")
                    Exit Sub
                End If


                'Creating default formats/sound
                'Only playing, using the output sound format as recording sound format
                If RecordingSoundFormat Is Nothing Then RecordingSoundFormat = OutputSound.WaveFormat

                'Only recording, using a default very short silent sound as output sound (after that, also silence will be output)
                'Also setting StopAtOutputSoundEnd to false so that recording will not be stopped in advance.
                If OutputSound Is Nothing Then
                    OutputSound = New Sound(RecordingSoundFormat)
                    OutputSound = GenerateSound.CreateSilence(RecordingSoundFormat,, 0.01)
                    StopAtOutputSoundEnd = False
                End If

                'Setting LongestRecordingDuration
                'Me._LongestRecordingDuration = LongestRecordingDuration

                'Setting clipping
                Me._IsClippingInactivated = InactivateClipping

                'Setting sample rate auto adjustment
                Me._AutoAdjustSampleRateToOutputSound = AutoAdjustSampleRateToOutputSound

                'Overriding any value set in InitializationSuccess
                _IsInitialized = False

                SoundPlayer.LoggingEnabled = LoggingEnabled
                SoundPlayer.MessagesEnabled = MessagesEnabled
                Log("Initializing...")

                Try

                    'Initializing PA
                    If ErrorCheck("Initialize", PortAudio.Pa_Initialize(), True) = True Then
                        Me.disposed = True
                        ' if Pa_Initialize() returns an error code, 
                        ' Pa_Terminate() should NOT be called.
                        Throw New Exception("Can't initialize audio")
                    End If

                    Me.UseBlockingIO = UseBlockingIO
                    Me.RecordingSoundFormat = RecordingSoundFormat
                    _OutputSound = OutputSound
                    Me.StopAtOutputSoundEnd = StopAtOutputSoundEnd

                    'Setting API settings if not already done
                    If AudioApiSettings Is Nothing Then
                        'Dim FixedSampleRate As Integer? = Nothing
                        'If OutputSound IsNot Nothing Then FixedSampleRate = OutputSound.WaveFormat.SampleRate
                        Dim newAudioSettingsDialog As New AudioSettingsDialog() 'FixedSampleRate)
                        Dim DialogResult = newAudioSettingsDialog.ShowDialog()
                        If DialogResult = DialogResult.OK Then
                            AudioApiSettings = newAudioSettingsDialog.CurrentAudioApiSettings

                        Else
                            MsgBox("Did not initialize PaSoundPlayer due to missing audio settings.")
                            Throw New Exception("Did not initialize PaSoundPlayer due to missing audio settings.")
                            Log(ErrorCheck("Terminate", PortAudio.Pa_Terminate(), True))
                            Exit Sub
                        End If
                    End If


                    'Setting Me.audioApiSettings
                    Me.AudioApiSettings = AudioApiSettings

                    'Aborting if the output channel count is not supported.
                    Dim TooManyChannels As Boolean = False
                    If Not Me.AudioApiSettings.SelectedInputDeviceInfo Is Nothing Then
                        If RecordingSoundFormat.Channels > Me.AudioApiSettings.SelectedInputDeviceInfo.Value.maxInputChannels Then
                            TooManyChannels = True
                        End If
                    End If

                    If Not Me.AudioApiSettings.SelectedOutputDeviceInfo Is Nothing Then
                        If OutputSound.WaveFormat.Channels > Me.AudioApiSettings.SelectedOutputDeviceInfo.Value.maxOutputChannels Then
                            TooManyChannels = True
                        End If
                    End If

                    If Not Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo Is Nothing Then
                        If RecordingSoundFormat.Channels > Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo.Value.maxInputChannels Then
                            TooManyChannels = True
                        End If
                        If OutputSound.WaveFormat.Channels > Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo.Value.maxOutputChannels Then
                            TooManyChannels = True
                        End If
                    End If

                    If TooManyChannels = True Then
                        Throw New Exception("Not enough avaliable channels on the input or output device. Disposing PaSoundPLayer.")
                        MsgBox("Not enough avaliable channels on the input or output device. Disposing PaSoundPLayer.")
                        Log(ErrorCheck("Terminate", PortAudio.Pa_Terminate(), True))
                        Me.Dispose()
                        Exit Sub
                    End If


                    Log("Selected HostAPI:" & vbLf & Me.AudioApiSettings.SelectedApiInfo.ToString())
                    If Not Me.AudioApiSettings.SelectedInputDeviceInfo Is Nothing Then Log("Selected input device:" & vbLf & Me.AudioApiSettings.SelectedInputDeviceInfo.ToString())
                    If Not Me.AudioApiSettings.SelectedOutputDeviceInfo Is Nothing Then Log("Selected output device:" & vbLf & Me.AudioApiSettings.SelectedOutputDeviceInfo.ToString())
                    If Not Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo Is Nothing Then Log("Selected input and output device:" & vbLf & Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo.ToString())

                    _IsInitialized = True

                Catch e As Exception
                    Log(ErrorCheck("Terminate", PortAudio.Pa_Terminate(), True))
                    Log(e.ToString())
                End Try
            End Sub


            ''' <summary>
            ''' Replaces the current output sound with a new sound. Replacement is only supported if the two sounds have the same format.
            ''' </summary>
            ''' <param name="OutputSound">The new output sound.</param>
            Public Sub SetNewOutputSound(ByVal OutputSound As Sound)

                'Checking that the new sound has the same SampleRate, BitDepth, and Channel count, and encoding as the old one
                'If OutputSound.WaveFormat.SampleRate = Me.OutputSound.WaveFormat.SampleRate And
                'OutputSound.WaveFormat.BitDepth = Me.OutputSound.WaveFormat.BitDepth And
                'OutputSound.WaveFormat.Channels = Me.OutputSound.WaveFormat.Channels And
                'OutputSound.WaveFormat.Encoding = Me.OutputSound.WaveFormat.Encoding Then

                'Stopping any played sound
                If IsPlaying = True Then [Stop]()

                'Setting position to 0
                _Position = 0

                'Replacing output sound
                Me._OutputSound = OutputSound

                _HasSoundOutput = True

                'Else
                'Throw New ArgumentException("The new output sound must have the same format as the replaced sound.")
                'End If

            End Sub


            ''' <summary>
            ''' Allowes playback to jump to the indicated start position (in seconds). (Start time is adjusted to fit into the range of 0 through OutputSound duration)
            ''' It may be used both during playback and paused playback.
            ''' </summary>
            Public Sub SeekTime(ByVal StartTime As Double)

                If OutputSound IsNot Nothing Then
                    Dim StartSample As Long = StartTime * OutputSound.WaveFormat.SampleRate
                    SeekSample(StartSample)
                Else
                    _Position = 0
                End If

            End Sub

            ''' <summary>
            ''' Allowes playback to jump to the indicated sample position. (Startsample is adjusted to fit into the range of 0 through OutputSound.WaveData.ShortestChannelSampleCount)
            ''' It may be used both during playback and paused playback.
            ''' </summary>
            Public Sub SeekSample(ByVal StartSample As Long)

                If OutputSound IsNot Nothing Then
                    If StartSample < OutputSound.WaveData.ShortestChannelSampleCount Then
                        _Position = StartSample
                        If StartSample < 0 Then _Position = 0
                    Else
                        _Position = OutputSound.WaveData.ShortestChannelSampleCount - 1
                    End If
                Else
                    _Position = 0
                End If

            End Sub


            Public Sub ClearRecordedSound()
                InputBufferHistory.Clear()
            End Sub

            Public Sub OpenStream()

                Log("Opening stream...")
                Me.stream = StreamOpen()
                Log("Stream pointer: " & stream.ToString())

            End Sub


            ''' <summary>
            ''' Starts the sound stream.
            ''' </summary>
            '''<param name="StartTime">Start time (in seconds) from the beginning of the sound.</param>
            '''<param name="PlayDuration">The length to play in seconds. If left out, the whole sound is played.</param>
            ''' <param name="AppendRecordedSound">If set to True, the new recording will be appended any previously recorded sound. If set to False, a new recording will be started.</param>
            ''' <param name="OutputSoundFadeInTime">Time in seconds during which the output sound will be faded in. (Only implemented for BlockingIO.)</param>
            '''<param name="OutputSoundFadeOutTime">Can be used to fade out a preset play length, instead of using the fade parameter of the stop method.</param>
            Public Sub StartSeconds(ByRef StartTime As Double,
                         Optional ByRef PlayDuration As Double? = Nothing,
                         Optional ByVal AppendRecordedSound As Boolean = False,
                         Optional ByVal OutputSoundFadeInTime As Double = 0,
                         Optional ByVal OutputSoundFadeOutTime As Double = 0,
                         Optional ByVal CloseStreamAfterPlayCompletion As Boolean = True)

                'Converting times to samples
                Dim StartSample As Long? = Nothing
                StartSample = StartTime * OutputSound.WaveFormat.SampleRate

                Dim PlayLength As Long? = Nothing
                If PlayDuration IsNot Nothing Then
                    PlayLength = PlayDuration * OutputSound.WaveFormat.SampleRate
                End If

                'Calling Start
                Start(StartSample, PlayLength, AppendRecordedSound, OutputSoundFadeInTime, OutputSoundFadeOutTime, CloseStreamAfterPlayCompletion)

                'Converting the actual sample counts used back to Times, so that they can be retrieved by the calling code
                StartTime = StartSample / OutputSound.WaveFormat.SampleRate
                PlayDuration = PlayLength / OutputSound.WaveFormat.SampleRate

            End Sub

            Private DoSoundRecording As Boolean
            Private DoSoundOutput As Boolean

            ''' <summary>
            ''' Starts the sound stream.
            ''' </summary>
            '''<param name="StartSample">Start position (in samples) from the beginning of the sound. If left out, the sound is played from the current position.</param>
            '''<param name="PlayLength">The length to play in samples. If left out, the sound is played to the end of the sound.</param>
            ''' <param name="AppendRecordedSound">If set to True, the new recording will be appended any previously recorded sound. If set to False, a new recording will be started.</param>
            ''' <param name="OutputSoundFadeInTime">Time in seconds during which the output sound will be faded in. (Only implemented for BlockingIO.)</param>
            '''<param name="OutputSoundFadeOutTime">Can be used to fade out a preset play length, instead of using the fade parameter of the stop method.</param>
            '''<param name="CloseStreamAfterPlayCompletion">If set to true, the sound stream is automatically closed when the sound has finished playing.</param>
            '''<param name="BlockingIOUpdateInterval">Sets the interval (in milliseconds) of the timer triggering the BlockingIO events. If left to nothing, the interval will be set to 80 % of the duration of the callback buffer specified in AudioApiSettings.</param>
            Public Sub Start(Optional ByRef StartSample As Long = 0,
                         Optional ByRef PlayLength As Long = -1,
                         Optional ByVal AppendRecordedSound As Boolean = False,
                         Optional ByVal OutputSoundFadeInTime As Double = 0,
                         Optional ByVal OutputSoundFadeOutTime As Double = 0,
                         Optional ByVal CloseStreamAfterPlayCompletion As Boolean = True,
                         Optional ByVal BlockingIOUpdateInterval As Integer? = Nothing)

                'Resetting DoSoundRecording and DoSoundOutput
                DoSoundRecording = HasSoundInput
                DoSoundOutput = HasSoundOutput

                If BlockingIOUpdateInterval IsNot Nothing Then
                    BlockingTimer.Interval = BlockingIOUpdateInterval
                Else
                    BlockingTimer.Interval = (AudioApiSettings.FramesPerBuffer / OutputSound.WaveFormat.SampleRate) * 800 'Setting the interval to 80 % of the buffer duration selected in the AudioApiSettings
                End If

                'Resetting EndOfOutputSoundIsReached
                EndOfOutputSoundIsReached = False

                Me.CloseStreamAfterPlayCompletion = CloseStreamAfterPlayCompletion

                If AppendRecordedSound = False Then
                    ClearRecordedSound()
                End If


                'Setting start position (by adjusting the current position)
                SeekSample(StartSample)
                StartSample = _Position

                'Setting playing length
                If PlayLength < 0 Then

                    'Limiting the play length to fit within the range of 0 throught the length of the output sound
                    'If _Position + PlayLength > OutputSound.WaveData.ShortestChannelSampleCount Then
                    'PlayLength = Math.Max(0, PlayLength = OutputSound.WaveData.ShortestChannelSampleCount - _Position)
                    'End If

                    'Else
                    'Simply playing the remaining sound length, after the current position 
                    PlayLength = OutputSound.WaveData.ShortestChannelSampleCount - _Position
                End If


                'Setting which sample to stop at
                OutputSoundStopSample = _Position + PlayLength

                'Limiting the stop sample to fit within the remaining sound
                If OutputSoundStopSample > OutputSound.WaveData.ShortestChannelSampleCount Then OutputSoundStopSample = OutputSound.WaveData.ShortestChannelSampleCount



                'Settings fade in sample frame count and current fade in position
                OutputSoundFadeInCurrentSample = 0
                OutputSoundFadeInSamples = OutputSoundFadeInTime * OutputSound.WaveFormat.SampleRate
                'Limiting the fade in region to the length of the output sound
                If OutputSoundFadeInSamples > OutputSound.WaveData.ShortestChannelSampleCount Then OutputSoundFadeInSamples = OutputSound.WaveData.ShortestChannelSampleCount


                'Settings fade out sample count
                OutputSoundFadeOutSamples = OutputSoundFadeOutTime * OutputSound.WaveFormat.SampleRate

                'Limiting the fade out sample count to fit within the sound (making sure fade out doesn't start before the first sample)
                If OutputSoundFadeOutSamples < 0 Then OutputSoundFadeOutSamples = 0

                'Updating fadeout time (this could be done in order to get the actual sample time)
                'OutputSoundFadeOutTime = OutputSoundFadeOutSamples / OutputSound.WaveFormat.SampleRate

                'Determining the fade out start point (sample) 
                OutputSoundFadeOutStartSample = OutputSoundStopSample - OutputSoundFadeOutSamples


                'If PortAudio.Pa_IsStreamActive(Me.stream) = 0 Then
                Log("Starting stream")

                If ErrorCheck("StartStream", PortAudio.Pa_StartStream(stream), True) = False Then

                    If UseBlockingIO = True Then
                        BlockingReadWrite_Active = True
                        BlockingTimer.Start()
                    End If

                    _IsPlaying = True

                End If

            End Sub


            ''' <summary>
            ''' Stops the output sound.
            ''' </summary>
            ''' <param name="OutputSoundFadeOutTime">Time in seconds during which the output sound will be faded out before it is stopped. (Only implemented for BlockingIO.)</param>
            ''' <returns>Returns the actual fade out time (in seconds) used.</returns>
            Public Function [Stop](Optional ByVal OutputSoundFadeOutTime As Double = 0) As Double

                'Stops recording directly
                DoSoundRecording = False

                'Calling stop right away if no fade out should be done
                If OutputSoundFadeOutTime = 0 Then
                    StopNow()
                    Return 0
                End If

                Dim CurrentPosition As Long = _Position 'Creating a local variable to ensure that the position value is not alterred between the steps by the readerwriter thread

                'Setting which sample to stop at
                OutputSoundStopSample = CurrentPosition + (OutputSoundFadeOutTime * OutputSound.WaveFormat.SampleRate)

                'Limiting the stop sample to fit within the remaining sound
                If OutputSoundStopSample > OutputSound.WaveData.ShortestChannelSampleCount Then OutputSoundStopSample = OutputSound.WaveData.ShortestChannelSampleCount

                'Setting the fade out start point (sample) to the current position 
                OutputSoundFadeOutStartSample = CurrentPosition

                'Settings fade out sample count
                OutputSoundFadeOutSamples = OutputSoundStopSample - OutputSoundFadeOutStartSample

                Return OutputSoundFadeOutSamples / OutputSound.WaveFormat.SampleRate

            End Function

            Private Sub StopNow()

                'Resetting stop and fade out variables
                OutputSoundStopSample = 0
                OutputSoundFadeOutSamples = 0
                OutputSoundFadeOutStartSample = 0

                If UseBlockingIO = True Then

                    BlockingReadWrite_Active = False
                    BlockingTimer.Stop()

                    _IsPlaying = False

                End If

                Log("Stopping stream...")

                If ErrorCheck("StopStream", PortAudio.Pa_StopStream(stream), True) = False Then
                    _IsPlaying = False
                End If

                'Storing recorded sound
                'StoreRecordedSound()

            End Sub


            Public Sub AbortStream() 'Optional ByVal StoreInputSound As Boolean = True)

                'Stops recording directly
                DoSoundRecording = False


                If UseBlockingIO = True Then

                    BlockingReadWrite_Active = False
                    BlockingTimer.Stop()

                    _IsPlaying = False

                End If

                Log("Aborting stream...")

                If ErrorCheck("AbortStream", PortAudio.Pa_AbortStream(stream), True) = False Then
                    _IsPlaying = False
                End If

                'If StoreInputSound = True Then
                'Storing recorded sound
                'StoreRecordedSound()
                'End If

            End Sub



            Public Sub CloseStream()

                'Stopping the stream if it is running
                If PortAudio.Pa_IsStreamStopped(Me.stream) < 1 Then
                    [Stop]()
                End If

                'Cloing the stream
                If ErrorCheck("CloseStream", PortAudio.Pa_CloseStream(stream), True) = False Then

                    _IsStreamOpen = False

                    'Resetting the stream
                    Me.stream = New IntPtr(0)
                End If

            End Sub


            Private Function StreamOpen() As IntPtr

                'Setting buffer length data, and adjusting the length of the buffer arrays
                Dim HighestChannelCount As Integer = Math.Max(RecordingSoundFormat.Channels, OutputSound.WaveFormat.Channels)

                If Me.UseBlockingIO = False Then
                    'Adjusting the length of the output buffer
                    If callbackBuffer.Length < (Me.AudioApiSettings.FramesPerBuffer * HighestChannelCount) Then
                        Log("Modified the callback buffer length. Old output buffer length: " & callbackBuffer.Length & " New length: " &
                    Me.AudioApiSettings.FramesPerBuffer * HighestChannelCount)
                        callbackBuffer = New Single((Me.AudioApiSettings.FramesPerBuffer * HighestChannelCount) - 1) {}
                        SilentBuffer = New Single((Me.AudioApiSettings.FramesPerBuffer * HighestChannelCount) - 1) {}
                    End If
                End If

                If AutoAdjustSampleRateToOutputSound = True Then
                    If OutputSound IsNot Nothing Then
                        If Me.AudioApiSettings.SampleRate <> OutputSound.WaveFormat.SampleRate Then
                            'Overriding any previously sample rate stored in audioApiSettings if an output sound exists, and it's sample rate differs from that stored in the audioApiSettings (to ensure it will be played correctly).
                            Log("Overriding previously set sample rate. Setting it to the sample rate of the out put sound: " & OutputSound.WaveFormat.SampleRate)
                            Me.AudioApiSettings.SampleRate = OutputSound.WaveFormat.SampleRate
                        End If
                    End If
                End If

                'Checking that the sample rate matches the output sound sample rate
                If AudioApiSettings.SampleRate <> OutputSound.WaveFormat.SampleRate Then
                    MsgBox("Warning, non matching sample rate of output sound! Have you selected a different sample rate that the one in the sound you're playing?" & vbCr &
                       "Press OK to continue anyway.")
                End If

                Dim stream As New IntPtr()
                Dim data As New IntPtr(0)

                Dim inputParams As New PortAudio.PaStreamParameters
                If Me.AudioApiSettings.SelectedInputDevice IsNot Nothing Then

                    inputParams.channelCount = RecordingSoundFormat.Channels
                    inputParams.device = Me.AudioApiSettings.SelectedInputDevice
                    Select Case RecordingSoundFormat.Encoding
                        Case Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints
                            inputParams.sampleFormat = PortAudio.PaSampleFormat.paFloat32
                        Case Formats.WaveFormat.WaveFormatEncodings.PCM
                            inputParams.sampleFormat = PortAudio.PaSampleFormat.paInt16
                        Case Else
                            Throw New NotImplementedException("Wave data encoding " & RecordingSoundFormat.Encoding.ToString & " is presently not supported for recording.")
                    End Select

                    If Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo.HasValue = True Then
                        inputParams.suggestedLatency = Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo.Value.defaultLowInputLatency
                    Else
                        inputParams.suggestedLatency = Me.AudioApiSettings.SelectedInputDeviceInfo.Value.defaultLowInputLatency
                    End If

                End If

                Dim outputParams As New PortAudio.PaStreamParameters
                If Me.AudioApiSettings.SelectedOutputDevice IsNot Nothing Then
                    outputParams.channelCount = OutputSound.WaveFormat.Channels
                    outputParams.device = Me.AudioApiSettings.SelectedOutputDevice
                    Select Case OutputSound.WaveFormat.Encoding
                        Case Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints
                            outputParams.sampleFormat = PortAudio.PaSampleFormat.paFloat32
                        Case Formats.WaveFormat.WaveFormatEncodings.PCM
                            outputParams.sampleFormat = PortAudio.PaSampleFormat.paInt16
                        Case Else
                            Throw New NotImplementedException("Wave data encoding " & OutputSound.WaveFormat.Encoding.ToString & " is presently not supported for output sounds.")
                    End Select
                    If Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo.HasValue = True Then
                        outputParams.suggestedLatency = Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo.Value.defaultLowOutputLatency
                    Else
                        outputParams.suggestedLatency = Me.AudioApiSettings.SelectedOutputDeviceInfo.Value.defaultLowOutputLatency
                    End If
                End If

                Log(inputParams.ToString)
                Log(outputParams.ToString)

                Dim Flag As PortAudio.PaStreamFlags
                If IsClippingInactivated = True Then
                    Flag = PortAudio.PaStreamFlags.paClipOff
                Else
                    Flag = PortAudio.PaStreamFlags.paNoFlag
                End If


                If UseBlockingIO = True Then

                    ErrorCheck("OpenStream", PortAudio.Pa_OpenStream(stream, inputParams, outputParams, Me.AudioApiSettings.SampleRate, Me.AudioApiSettings.FramesPerBuffer, Flag,
                Nothing, data), True)

                Else
                    If FadeEnabledCallback = True Then

                        If _HasSoundInput = True And _HasSoundOutput = True Then
                            ErrorCheck("OpenDuplexStream", PortAudio.Pa_OpenStream(stream, inputParams, outputParams, Me.AudioApiSettings.SampleRate, Me.AudioApiSettings.FramesPerBuffer, Flag,
                Me.paStreamCallbackFadeEnabled, data), True)

                        ElseIf _HasSoundOutput = True Then
                            ErrorCheck("OpenOutputOnlyStream", PortAudio.Pa_OpenStream(stream, New Nullable(Of PortAudio.PaStreamParameters), outputParams, Me.AudioApiSettings.SampleRate, Me.AudioApiSettings.FramesPerBuffer, Flag,
                Me.paStreamCallback_OutputOnly_FadeEnabled, data), True)

                        Else
                            ErrorCheck("OpenInputOnlyStream", PortAudio.Pa_OpenStream(stream, inputParams, New Nullable(Of PortAudio.PaStreamParameters), Me.AudioApiSettings.SampleRate, Me.AudioApiSettings.FramesPerBuffer, Flag,
                Me.paStreamCallback_InputOnly, data), True)

                        End If

                    Else

                        If _HasSoundInput = True And _HasSoundOutput = True Then
                            ErrorCheck("OpenDuplexStream", PortAudio.Pa_OpenStream(stream, inputParams, outputParams, Me.AudioApiSettings.SampleRate, Me.AudioApiSettings.FramesPerBuffer, Flag,
                Me.paStreamCallback, data), True)

                        ElseIf _HasSoundOutput = True Then
                            ErrorCheck("OpenOutputOnlyStream", PortAudio.Pa_OpenStream(stream, New Nullable(Of PortAudio.PaStreamParameters), outputParams, Me.AudioApiSettings.SampleRate, Me.AudioApiSettings.FramesPerBuffer, Flag,
                Me.paStreamCallback_OutputOnly, data), True)

                        Else
                            ErrorCheck("OpenInputOnlyStream", PortAudio.Pa_OpenStream(stream, inputParams, New Nullable(Of PortAudio.PaStreamParameters), Me.AudioApiSettings.SampleRate, Me.AudioApiSettings.FramesPerBuffer, Flag,
                Me.paStreamCallback_InputOnly, data), True)

                        End If
                    End If

                End If

                _IsStreamOpen = True

                Return stream
            End Function


            'Public Structure ObjectHolder
            'Dim o1 As Object
            '<MarshalAs(UnmanagedType.IDispatch)> Public o2 As Object
            'End Structure

            ''' <summary>
            ''' Gets the recorded sound so far.
            ''' </summary>
            ''' <returns></returns>
            Public Function GetRecordedSound() As Sound

                Log("Attemting to get recorded sound")

                'Stopping sound if not already done
                If _IsPlaying = True Then [Stop](0.1)

                'Returning nothing if no input sound exists
                If HasSoundInput = False Then Return Nothing
                If InputBufferHistory Is Nothing Then Return Nothing

                'Creating a new Sound
                Dim RecordedSound As New Sound(RecordingSoundFormat)

                If InputBufferHistory.Count = 0 Then Return RecordedSound

                If InputBufferHistory.Count > 0 Then

                    'Determining output sound length
                    Dim OutputSoundSampleCount As Long = 0
                    For Each Buffer In InputBufferHistory
                        OutputSoundSampleCount += Buffer.Length / RecordingSoundFormat.Channels
                    Next

                    For ch = 0 To RecordingSoundFormat.Channels - 1
                        Dim NewChannelArray(OutputSoundSampleCount - 1) As Single
                        RecordedSound.WaveData.SampleData(ch + 1) = NewChannelArray
                    Next

                    'Sorting the interleaved samples to 
                    Dim CurrentBufferStartSample As Long = 0
                    For Each Buffer In InputBufferHistory
                        Dim CurrentBufferSampleIndex As Long = 0
                        For CurrentDataPoint = 0 To Buffer.Length - 1 Step RecordingSoundFormat.Channels

                            For ch = 0 To RecordingSoundFormat.Channels - 1
                                Try
                                    RecordedSound.WaveData.SampleData(ch + 1)(CurrentBufferStartSample + CurrentBufferSampleIndex) = Buffer(CurrentDataPoint + ch)
                                Catch ex As Exception
                                    Log(ErrorCheck("Terminate", PortAudio.Pa_Terminate(), True))
                                End Try
                            Next
                            'Increasing sample index
                            CurrentBufferSampleIndex += 1
                        Next
                        CurrentBufferStartSample += CurrentBufferSampleIndex
                    Next

                End If
                Return RecordedSound

            End Function

            ''' <summary>
            ''' Returns the time delay (in seconds) caused by the call-back buffer size. (If UseBlockingIO is true, the time delay is only approximative.)
            ''' </summary>
            ''' <returns></returns>
            Public Function GetCallBackTime() As Double

                If UseBlockingIO = False Then
                    Return AudioApiSettings.FramesPerBuffer / OutputSound.WaveFormat.SampleRate
                Else
                    Return BlockingTimer.Interval / 1000
                End If

            End Function

            Private Sub Log(logString As String)
                If m_loggingEnabled = True Then
                    System.Console.WriteLine("PortAudio: " & logString)
                End If
            End Sub

            Private Sub DisplayMessageInBox(Message As String)
                If m_messagesEnabled = True Then
                    MsgBox(Message)
                End If
            End Sub

            Public Shared LogToFileEnabled As Boolean = True
            Private Sub LogToFile(Message As String)
                If LogToFileEnabled = True Then
                    SendInfoToAudioLog(Message)
                End If
            End Sub

            Private Function ErrorCheck(action As String, errorCode As PortAudio.PaError, Optional ShowErrorInMsgBox As Boolean = False) As Boolean
                If errorCode <> PortAudio.PaError.paNoError Then
                    Dim MessageA As String = action & " error: " & PortAudio.Pa_GetErrorText(errorCode)
                    Log(MessageA)
                    If ShowErrorInMsgBox = True Then DisplayMessageInBox(MessageA)

                    LogToFile(MessageA)

                    If errorCode = PortAudio.PaError.paUnanticipatedHostError Then
                        Dim errorInfo As PortAudio.PaHostErrorInfo = PortAudio.Pa_GetLastHostErrorInfo()
                        Dim MessageB As String = "- Host error API type: " & errorInfo.hostApiType
                        Dim MessageC As String = "- Host error code: " & errorInfo.errorCode
                        Dim MessageD As String = "- Host error text: " & errorInfo.errorText
                        Log(MessageB)
                        Log(MessageC)
                        Log(MessageD)

                        LogToFile(MessageB)
                        LogToFile(MessageC)
                        LogToFile(MessageD)

                        If ShowErrorInMsgBox = True Then DisplayMessageInBox(MessageB & vbCrLf & MessageC & vbCrLf & MessageD)
                    End If

                    Return True
                Else
                    Log(action & " OK")
                    LogToFile(action & " OK")
                    Return False
                End If
            End Function

            'BLOCKING FUNCTIONS
            Private Sub BlockingUpdate() Handles BlockingTimer.Tick
                If _HasSoundInput = True And _HasSoundOutput = True Then
                    ReadAndWriteStream()
                ElseIf _HasSoundOutput = True Then
                    WriteStream()
                Else
                    ReadStream()
                End If
            End Sub


            Private Sub ReadAndWriteStream()

                'Do While StopReadWrite = False
                If BlockingReadWrite_Active = True Then

                    'INPUT SOUND
                    If DoSoundRecording = True Then
                        'Getting input sound
                        Dim AvailableReadFrames As Integer = PortAudio.Pa_GetStreamReadAvailable(stream)

                        Dim InputSoundArray(AvailableReadFrames * RecordingSoundFormat.Channels - 1) As Single
                        PortAudio.Pa_ReadStream(stream, InputSoundArray, AvailableReadFrames)
                        InputBufferHistory.Add(InputSoundArray)
                    End If

                    'OUTPUT SOUND
                    If DoSoundOutput = True Then

                        Dim AvailableWriteFrames As Integer = PortAudio.Pa_GetStreamWriteAvailable(stream)
                        Dim OutputSoundArray(AvailableWriteFrames * OutputSound.WaveFormat.Channels - 1) As Single
                        Dim HasReachedPlayLength As Boolean = False

                        'Checking if end of sound is reached
                        If EndOfOutputSoundIsReached = True Then
                            'Exporting Silence
                            PortAudio.Pa_WriteStream(stream, OutputSoundArray, AvailableWriteFrames)
                        Else

                            'Exporting sound data from the OutputSound
                            For j As Integer = 0 To AvailableWriteFrames - 1

                                'Calculating the fading factor for the current sample (fading factor is not channel specific)

                                'Setting a default non-fading factor
                                OutputSoundFadeFactor = 1

                                'Checking if fading in should be done
                                If OutputSoundFadeInCurrentSample < OutputSoundFadeInSamples Then

                                    'Do fade in
                                    'Calculating current fade in factor, and multiplying the default fade factor by it
                                    OutputSoundFadeFactor *= OutputSoundFadeInCurrentSample / OutputSoundFadeInSamples
                                    'Increasing current fade in sample index
                                    OutputSoundFadeInCurrentSample += 1

                                End If

                                'Checking if fading out should be done
                                If OutputSoundFadeOutSamples > 0 Then

                                    'Do fade out
                                    'Calculating current fade out factor, and multiplying the current fade factor by it (the current fade factor could either be 1 or a fade in factor. This allowes both fade in and fade out being active at the same time point)
                                    If _Position > OutputSoundFadeOutStartSample Then
                                        OutputSoundFadeFactor *= (OutputSoundStopSample - _Position) / OutputSoundFadeOutSamples

                                        If _Position > OutputSoundStopSample Then
                                            OutputSoundFadeFactor = 0
                                        End If

                                    End If
                                End If


                                'Reading the different channels
                                For Ch = 0 To OutputSound.WaveFormat.Channels - 1

                                    'Checking if there is any more sound to be played
                                    If _Position < OutputSoundStopSample Then

                                        OutputSoundArray((j * OutputSound.WaveFormat.Channels) + Ch) = OutputSoundFadeFactor * _OutputSound.WaveData.SampleData(Ch + 1)(_Position)

                                    Else
                                        'Marks that the end of the output sound has been reached
                                        EndOfOutputSoundIsReached = True

                                        'Fills up the buffer with zeroes, if the end of the sound is reached
                                        OutputSoundArray((j * OutputSound.WaveFormat.Channels) + Ch) = 0
                                        'log("Doing zero filling. Sample position: " & _Position)

                                    End If

                                Next

                                'Increasing position
                                _Position += 1

                            Next

                            PortAudio.Pa_WriteStream(stream, OutputSoundArray, AvailableWriteFrames)

                            'Stopping sound if the end of sound was reached
                            If EndOfOutputSoundIsReached = True Then
                                If StopAtOutputSoundEnd = True Then
                                    StopNow()
                                    If CloseStreamAfterPlayCompletion = True Then
                                        CloseStream()
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

            End Sub


            Private Sub ReadAndWriteStreamWithLogging()

                'Do While StopReadWrite = False
                If BlockingReadWrite_Active = True Then

                    'INPUT SOUND
                    If DoSoundRecording = True Then
                        'Getting input sound
                        Dim AvailableReadFrames As Integer = PortAudio.Pa_GetStreamReadAvailable(stream)

                        Log("AvailableReadFrames: " & AvailableReadFrames)

                        Dim InputSoundArray(AvailableReadFrames * RecordingSoundFormat.Channels - 1) As Single
                        Log(ErrorCheck("ReadStream", PortAudio.Pa_ReadStream(stream, InputSoundArray, AvailableReadFrames), True))
                        InputBufferHistory.Add(InputSoundArray)
                    End If


                    'OUTPUT SOUND
                    If DoSoundOutput = True Then

                        Dim AvailableWriteFrames As Integer = PortAudio.Pa_GetStreamWriteAvailable(stream)
                        Dim OutputSoundArray(AvailableWriteFrames * OutputSound.WaveFormat.Channels - 1) As Single
                        Dim HasReachedPlayLength As Boolean = False

                        Log("AvailableWriteFrames: " & AvailableWriteFrames)

                        'Checking if end of sound is reached
                        If EndOfOutputSoundIsReached = True Then
                            'Exporting Silence
                            Log(ErrorCheck("WriteSilentStream", PortAudio.Pa_WriteStream(stream, OutputSoundArray, AvailableWriteFrames), True))
                        Else

                            'Exporting sound data from the OutputSound
                            For j As Integer = 0 To AvailableWriteFrames - 1

                                'Calculating the fading factor for the current sample (fading factor is not channel specific)

                                'Setting a default non-fading factor
                                OutputSoundFadeFactor = 1

                                'Checking if fading in should be done
                                If OutputSoundFadeInCurrentSample < OutputSoundFadeInSamples Then

                                    'Do fade in
                                    'Calculating current fade in factor, and multiplying the default fade factor by it
                                    OutputSoundFadeFactor *= OutputSoundFadeInCurrentSample / OutputSoundFadeInSamples
                                    'Increasing current fade in sample index
                                    OutputSoundFadeInCurrentSample += 1

                                End If

                                'Checking if fading out should be done
                                If OutputSoundFadeOutSamples > 0 Then

                                    'Do fade out
                                    'Calculating current fade out factor, and multiplying the current fade factor by it (the current fade factor could either be 1 or a fade in factor. This allowes both fade in and fade out being active at the same time point)
                                    If _Position > OutputSoundFadeOutStartSample Then
                                        OutputSoundFadeFactor *= (OutputSoundStopSample - _Position) / OutputSoundFadeOutSamples

                                        If _Position > OutputSoundStopSample Then
                                            OutputSoundFadeFactor = 0
                                        End If

                                        'log("FadingOut. " &
                                        '    "FadeStartPos: " & OutputSoundFadeOutStartSample &
                                        '    "OutputSoundStopSample" & OutputSoundStopSample &
                                        '"CurrentPos: " & _Position &
                                        '"OutputSoundFadeFactor: " & OutputSoundFadeFactor)

                                    End If
                                End If


                                'Reading the different channels
                                For Ch = 0 To OutputSound.WaveFormat.Channels - 1

                                    'Checking if there is any more sound to be played
                                    If _Position < OutputSoundStopSample Then

                                        OutputSoundArray((j * OutputSound.WaveFormat.Channels) + Ch) = OutputSoundFadeFactor * _OutputSound.WaveData.SampleData(Ch + 1)(_Position)

                                    Else
                                        'Marks that the end of the output sound has been reached
                                        EndOfOutputSoundIsReached = True

                                        'Fills up the buffer with zeroes, if the end of the sound is reached
                                        OutputSoundArray((j * OutputSound.WaveFormat.Channels) + Ch) = 0
                                        'log("Doing zero filling. Sample position: " & _Position)

                                    End If

                                Next

                                'Increasing position
                                _Position += 1

                            Next

                            Log(ErrorCheck("WriteStream", PortAudio.Pa_WriteStream(stream, OutputSoundArray, AvailableWriteFrames), True))

                            'Stopping sound if the end of sound was reached
                            If EndOfOutputSoundIsReached = True Then
                                Log("The end of the sound detected (sample position): " & _Position)

                                If StopAtOutputSoundEnd = True Then

                                    Log("Sound was stopped at sample position: " & _Position)
                                    StopNow()
                                    If CloseStreamAfterPlayCompletion = True Then
                                        Log("Attempting to close the stream after completion of play. Sample position: " & _Position)
                                        CloseStream()
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                'Loop

            End Sub

            Private Sub ReadStream()

                'Do While StopReadWrite = False
                If BlockingReadWrite_Active = True Then
                    If DoSoundRecording = True Then
                        'Getting input sound
                        Dim AvailableReadFrames As Integer = PortAudio.Pa_GetStreamReadAvailable(stream)

                        Dim InputSoundArray(AvailableReadFrames * RecordingSoundFormat.Channels - 1) As Single
                        PortAudio.Pa_ReadStream(stream, InputSoundArray, AvailableReadFrames)
                        InputBufferHistory.Add(InputSoundArray)
                    End If
                End If
            End Sub

            Private Sub WriteStream()

                If BlockingReadWrite_Active = True Then

                    Dim AvailableWriteFrames As Integer = PortAudio.Pa_GetStreamWriteAvailable(stream)
                    Dim OutputSoundArray(AvailableWriteFrames * OutputSound.WaveFormat.Channels - 1) As Single
                    Dim HasReachedPlayLength As Boolean = False

                    'Checking if end of sound is reached
                    If EndOfOutputSoundIsReached = True Then
                        'Exporting Silence
                        PortAudio.Pa_WriteStream(stream, OutputSoundArray, AvailableWriteFrames)
                    Else

                        'Exporting sound data from the OutputSound
                        For j As Integer = 0 To AvailableWriteFrames - 1

                            'Calculating the fading factor for the current sample (fading factor is not channel specific)

                            'Setting a default non-fading factor
                            OutputSoundFadeFactor = 1

                            'Checking if fading in should be done
                            If OutputSoundFadeInCurrentSample < OutputSoundFadeInSamples Then

                                'Do fade in
                                'Calculating current fade in factor, and multiplying the default fade factor by it
                                OutputSoundFadeFactor *= OutputSoundFadeInCurrentSample / OutputSoundFadeInSamples
                                'Increasing current fade in sample index
                                OutputSoundFadeInCurrentSample += 1

                            End If

                            'Checking if fading out should be done
                            If OutputSoundFadeOutSamples > 0 Then

                                'Do fade out
                                'Calculating current fade out factor, and multiplying the current fade factor by it (the current fade factor could either be 1 or a fade in factor. This allowes both fade in and fade out being active at the same time point)
                                If _Position > OutputSoundFadeOutStartSample Then
                                    OutputSoundFadeFactor *= (OutputSoundStopSample - _Position) / OutputSoundFadeOutSamples

                                    If _Position > OutputSoundStopSample Then
                                        OutputSoundFadeFactor = 0
                                    End If

                                End If
                            End If

                            'Reading the different channels
                            For Ch = 0 To OutputSound.WaveFormat.Channels - 1

                                'Checking if there is any more sound to be played
                                If _Position < OutputSoundStopSample Then

                                    OutputSoundArray((j * OutputSound.WaveFormat.Channels) + Ch) = OutputSoundFadeFactor * _OutputSound.WaveData.SampleData(Ch + 1)(_Position)

                                Else
                                    'Marks that the end of the output sound has been reached
                                    EndOfOutputSoundIsReached = True

                                    'Fills up the buffer with zeroes, if the end of the sound is reached
                                    OutputSoundArray((j * OutputSound.WaveFormat.Channels) + Ch) = 0
                                End If
                            Next

                            'Increasing position
                            _Position += 1

                        Next

                        PortAudio.Pa_WriteStream(stream, OutputSoundArray, AvailableWriteFrames)

                        'Stopping sound if the end of sound was reached
                        If EndOfOutputSoundIsReached = True Then
                            If StopAtOutputSoundEnd = True Then
                                StopNow()
                                If CloseStreamAfterPlayCompletion = True Then
                                    CloseStream()
                                End If
                            End If
                        End If
                    End If
                End If

            End Sub



#Region "IDisposable Support"
            Private disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not disposedValue Then
                    If disposing Then
                        ' TODO: dispose managed state (managed objects).
                    End If

                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                    Log("Terminating...")
                    ErrorCheck("Terminate", PortAudio.Pa_Terminate(), True)

                End If
                disposedValue = True
            End Sub

            ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
            Protected Overrides Sub Finalize()
                '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(False)
                MyBase.Finalize()
            End Sub

            ' This code added by Visual Basic to correctly implement the disposable pattern.
            Public Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(True)
                ' TODO: uncomment the following line if Finalize() is overridden above.
                GC.SuppressFinalize(Me)
            End Sub
#End Region

        End Class



        Public Class OverlappingSoundPlayer
            Implements IDisposable

            Private WithEvents MyController As PlayBack.ISoundPlayerControl
            '
            Private CrossFadeProgress As Integer = 0

            'Declaration of CALLBACK STUFF
            Private PlaybackBuffer As Single() = New Single(511) {}
            Private RecordingBuffer As Single() = New Single(511) {}
            Private SilentBuffer As Single() = New Single(511) {}
            Private paStreamCallback As PortAudio.PaStreamCallbackDelegate = Function(input As IntPtr, output As IntPtr, frameCount As UInteger, ByRef timeInfo As PortAudio.PaStreamCallbackTimeInfo, statusFlags As PortAudio.PaStreamCallbackFlags, userData As IntPtr) As PortAudio.PaStreamCallbackResult


                                                                                 'SyncLock PlaybackBuffer

                                                                                 'Sending a buffer tick to the controller
                                                                                 If IsBufferTickActive = True Then
                                                                                     SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.NewBufferTick)
                                                                                 End If

                                                                                 Try

                                                                                     'INPUT SOUND

                                                                                     If RecordingIsActive = True Then
                                                                                         'Getting input sound

                                                                                         'This need to be changed for real time recording, the audio need to be converted to Sound format, or maybe it could be fixed after stoppong the recording?
                                                                                         Dim InputBuffer(RecordingBuffer.Length - 1) As Single
                                                                                         Marshal.Copy(input, InputBuffer, 0, AudioApiSettings.FramesPerBuffer * RecordingSoundFormat.Channels)
                                                                                         InputBufferHistory.Add(InputBuffer)
                                                                                     End If


                                                                                     'OUTPUT SOUND
                                                                                     If PlaybackIsActive = True Then

                                                                                         'Checking if the current sound should be swapped (if there is a new sound in NewSound)
                                                                                         If NewSound IsNot Nothing Then

                                                                                             'Swapping sound
                                                                                             Select Case CurrentOutputSound
                                                                                                 Case OutputSounds.OutputSoundA, OutputSounds.FadingToA
                                                                                                     OutputSoundB = NewSound
                                                                                                     NewSound = Nothing
                                                                                                     CurrentOutputSound = OutputSounds.FadingToB

                                                                                                     'Setting reading position
                                                                                                     PositionB = 0

                                                                                                 Case OutputSounds.OutputSoundB, OutputSounds.FadingToB
                                                                                                     OutputSoundA = NewSound
                                                                                                     NewSound = Nothing
                                                                                                     CurrentOutputSound = OutputSounds.FadingToA

                                                                                                     'Setting reading position
                                                                                                     PositionA = 0
                                                                                             End Select

                                                                                             'Setting CrossFadeProgress to 0 since a new fade period have begun
                                                                                             CrossFadeProgress = 0

                                                                                         End If

                                                                                         'Checking current positions to see if an EndOfBufferAlert should be sent
                                                                                         Select Case CurrentOutputSound
                                                                                             Case OutputSounds.OutputSoundA, OutputSounds.FadingToA

                                                                                                 'Copying the silent buffer if the end of sound A is reached
                                                                                                 If PositionA = OutputSoundA.Length - ApproachingEndOfBufferAlert_BufferCount Then
                                                                                                     SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.ApproachingEndOfBufferAlert)
                                                                                                 End If

                                                                                             Case OutputSounds.OutputSoundB, OutputSounds.FadingToB

                                                                                                 'Copying the silent buffer if the end of sound A is reached
                                                                                                 If PositionB = OutputSoundB.Length - ApproachingEndOfBufferAlert_BufferCount Then
                                                                                                     SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.ApproachingEndOfBufferAlert)
                                                                                                 End If

                                                                                             Case Else
                                                                                                 Throw New NotImplementedException
                                                                                         End Select


                                                                                         'Copying buffers 
                                                                                         Select Case CurrentOutputSound
                                                                                             Case OutputSounds.OutputSoundA

                                                                                                 'Copying the silent buffer if the end of sound A is reached
                                                                                                 If PositionA >= OutputSoundA.Length Then
                                                                                                     Marshal.Copy(SilentBuffer, 0, output, SilentBuffer.Length)

                                                                                                     'Sending message to the controller
                                                                                                     SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.EndOfSound)

                                                                                                     If StopAtOutputSoundEnd = True Then

                                                                                                         'Returning the callback to port audio
                                                                                                         Return PortAudio.PaStreamCallbackResult.paComplete
                                                                                                     Else
                                                                                                         Return PortAudio.PaStreamCallbackResult.paContinue
                                                                                                     End If
                                                                                                 End If

                                                                                                 PlaybackBuffer = OutputSoundA(PositionA).InterleavedSampleArray

                                                                                                 'Copying the playback buffer to unmanaged memory
                                                                                                 Marshal.Copy(PlaybackBuffer, 0, output, AudioApiSettings.FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 PositionA += 1

                                                                                             Case OutputSounds.OutputSoundB

                                                                                                 'Copying the silent buffer if the end of sound B is reached
                                                                                                 If PositionB >= OutputSoundB.Length Then
                                                                                                     Marshal.Copy(SilentBuffer, 0, output, SilentBuffer.Length)

                                                                                                     'Sending message to the controller
                                                                                                     SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.EndOfSound)

                                                                                                     If StopAtOutputSoundEnd = True Then

                                                                                                         'Returning the callback to port audio
                                                                                                         Return PortAudio.PaStreamCallbackResult.paComplete
                                                                                                     Else
                                                                                                         Return PortAudio.PaStreamCallbackResult.paContinue
                                                                                                     End If
                                                                                                 End If

                                                                                                 PlaybackBuffer = OutputSoundB(PositionB).InterleavedSampleArray

                                                                                                 'Copying the playback buffer to unmanaged memory
                                                                                                 Marshal.Copy(PlaybackBuffer, 0, output, AudioApiSettings.FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 PositionB += 1

                                                                                             Case OutputSounds.FadingToA

                                                                                                 If PositionA < OutputSoundA.Length And PositionB < OutputSoundB.Length Then

                                                                                                     'Mixing sound A and B to the buffer
                                                                                                     For j As Integer = 0 To PlaybackBuffer.Length - 1
                                                                                                         PlaybackBuffer(j) = OutputSoundB(PositionB).InterleavedSampleArray(j) * OverlapFadeOutArray(CrossFadeProgress) + OutputSoundA(PositionA).InterleavedSampleArray(j) * OverlapFadeInArray(CrossFadeProgress)
                                                                                                         CrossFadeProgress += 1
                                                                                                     Next

                                                                                                     'Copying the playback buffer to unmanaged memory
                                                                                                     Marshal.Copy(PlaybackBuffer, 0, output, AudioApiSettings.FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 ElseIf PositionA < OutputSoundA.Length And PositionB >= OutputSoundB.Length Then

                                                                                                     'Copying only sound A to the buffer
                                                                                                     For j As Integer = 0 To PlaybackBuffer.Length - 1
                                                                                                         PlaybackBuffer(j) = OutputSoundA(PositionA).InterleavedSampleArray(j) * OverlapFadeInArray(CrossFadeProgress)
                                                                                                         CrossFadeProgress += 1
                                                                                                     Next

                                                                                                     'Copying the playback buffer to unmanaged memory
                                                                                                     Marshal.Copy(PlaybackBuffer, 0, output, AudioApiSettings.FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 ElseIf PositionA >= OutputSoundA.Length And PositionB < OutputSoundB.Length Then

                                                                                                     'Copying only sound B to the buffer
                                                                                                     For j As Integer = 0 To PlaybackBuffer.Length - 1
                                                                                                         PlaybackBuffer(j) = OutputSoundB(PositionB).InterleavedSampleArray(j) * OverlapFadeOutArray(CrossFadeProgress)
                                                                                                         CrossFadeProgress += 1
                                                                                                     Next

                                                                                                     'Copying the playback buffer to unmanaged memory
                                                                                                     Marshal.Copy(PlaybackBuffer, 0, output, AudioApiSettings.FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 Else
                                                                                                     'End of both sounds: Copying silence
                                                                                                     CrossFadeProgress = OverlapFadeLength
                                                                                                     Marshal.Copy(SilentBuffer, 0, output, SilentBuffer.Length)

                                                                                                     'Sending message to the controller
                                                                                                     SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.EndOfSound)

                                                                                                     If StopAtOutputSoundEnd = True Then

                                                                                                         'Returning the callback to port audio
                                                                                                         Return PortAudio.PaStreamCallbackResult.paComplete
                                                                                                     End If
                                                                                                 End If

                                                                                                 PositionA += 1
                                                                                                 PositionB += 1

                                                                                                 'Changing to OutputSounds.OutputSoundA and Resetting the CrossFadeProgress, if fading is completed
                                                                                                 If CrossFadeProgress >= OverlapFadeLength - 1 Then
                                                                                                     CurrentOutputSound = OutputSounds.OutputSoundA
                                                                                                     CrossFadeProgress = 0
                                                                                                 End If

                                                                                             Case OutputSounds.FadingToB

                                                                                                 If PositionA < OutputSoundA.Length And PositionB < OutputSoundB.Length Then

                                                                                                     'Mixing sound A and B to the buffer
                                                                                                     For j As Integer = 0 To PlaybackBuffer.Length - 1
                                                                                                         PlaybackBuffer(j) = OutputSoundB(PositionB).InterleavedSampleArray(j) * OverlapFadeInArray(CrossFadeProgress) + OutputSoundA(PositionA).InterleavedSampleArray(j) * OverlapFadeOutArray(CrossFadeProgress)
                                                                                                         CrossFadeProgress += 1
                                                                                                     Next

                                                                                                     'Copying the playback buffer to unmanaged memory
                                                                                                     Marshal.Copy(PlaybackBuffer, 0, output, AudioApiSettings.FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 ElseIf PositionA < OutputSoundA.Length And PositionB >= OutputSoundB.Length Then

                                                                                                     'Copying only sound A to the buffer
                                                                                                     For j As Integer = 0 To PlaybackBuffer.Length - 1
                                                                                                         PlaybackBuffer(j) = OutputSoundA(PositionA).InterleavedSampleArray(j) * OverlapFadeOutArray(CrossFadeProgress)
                                                                                                         CrossFadeProgress += 1
                                                                                                     Next

                                                                                                     'Copying the playback buffer to unmanaged memory
                                                                                                     Marshal.Copy(PlaybackBuffer, 0, output, AudioApiSettings.FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 ElseIf PositionA >= OutputSoundA.Length And PositionB < OutputSoundB.Length Then

                                                                                                     'Copying only sound B to the buffer
                                                                                                     For j As Integer = 0 To PlaybackBuffer.Length - 1
                                                                                                         PlaybackBuffer(j) = OutputSoundB(PositionB).InterleavedSampleArray(j) * OverlapFadeInArray(CrossFadeProgress)
                                                                                                         CrossFadeProgress += 1
                                                                                                     Next

                                                                                                     'Copying the playback buffer to unmanaged memory
                                                                                                     Marshal.Copy(PlaybackBuffer, 0, output, AudioApiSettings.FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 Else
                                                                                                     'End of both sounds: Copying silence
                                                                                                     CrossFadeProgress = OverlapFadeLength
                                                                                                     Marshal.Copy(SilentBuffer, 0, output, SilentBuffer.Length)

                                                                                                     'Sending message to the controller
                                                                                                     SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.EndOfSound)

                                                                                                     If StopAtOutputSoundEnd = True Then

                                                                                                         'Returning the callback to port audio
                                                                                                         Return PortAudio.PaStreamCallbackResult.paComplete
                                                                                                     End If
                                                                                                 End If

                                                                                                 PositionA += 1
                                                                                                 PositionB += 1

                                                                                                 'Changing to OutputSounds.OutputSoundA and Resetting the CrossFadeProgress, if fading is completed
                                                                                                 If CrossFadeProgress >= OverlapFadeLength - 1 Then
                                                                                                     CurrentOutputSound = OutputSounds.OutputSoundB
                                                                                                     CrossFadeProgress = 0
                                                                                                 End If

                                                                                                 'Case Else 'This is unnecessary as an exception would be thrown already above
                                                                                                 '    Throw New NotImplementedException
                                                                                         End Select
                                                                                     End If

                                                                                     Return PortAudio.PaStreamCallbackResult.paContinue

                                                                                 Catch ex As Exception

                                                                                     'Logging the exception
                                                                                     'Dim CurrentSiBTestStimulusFileName As String = ""
                                                                                     'If CurrentSiBTestData IsNot Nothing Then
                                                                                     '    If CurrentSiBTestData.CurrentTestStimulus IsNot Nothing Then CurrentSiBTestStimulusFileName = CurrentSiBTestData.CurrentTestStimulus.SoundRecordingFileName
                                                                                     'End If

                                                                                     SendInfoToAudioLog(CurrentOutputSound.ToString & " " & PositionA & " " & PositionB & " " & CrossFadeProgress & vbCrLf &
                                                                                                   ex.ToString, "ExceptionsDuringTesting")


                                                                                     'Utils.SendInfoToLog(CurrentOutputSound.ToString & " " & PositionA & " " & PositionB & " " & CrossFadeProgress & vbCrLf &
                                                                                     '              "CurrentSiBTestStimulusFileName:" & CurrentSiBTestStimulusFileName & vbCrLf &
                                                                                     '              ex.ToString, "ExceptionsDuringTesting")

                                                                                     'Select Case CurrentOutputSound
                                                                                     '    Case OutputSounds.FadingToA
                                                                                     '        Utils.SendInfoToLog("Error in FadingToA" & vbCrLf &
                                                                                     '        "CurrentSiBTestStimulusFileName:" & vbTab &
                                                                                     '        "callbackBuffer.length: " & vbTab &
                                                                                     '        "PlayBackSoundFormat.Channels: " & vbTab &
                                                                                     '    "OutputSoundB.WaveData.ShortestChannelSampleCount: " & vbTab &
                                                                                     '    "PositionB: " & vbTab &
                                                                                     '    "OverlapFadeOutArray.length: " & vbTab &
                                                                                     '    "CrossFadeProgress: " & vbTab &
                                                                                     '    "OutputSoundA.WaveData.ShortestChannelSampleCount: " & vbTab &
                                                                                     '    "PositionA: " & vbTab &
                                                                                     '    "OverlapFadeInArray.length: " & vbCrLf &
                                                                                     '    PlaybackBuffer.Length & vbTab &
                                                                                     '    NumberOfOutputChannels & vbTab &
                                                                                     '    OutputSoundB.WaveData.ShortestChannelSampleCount & vbTab &
                                                                                     '    PositionB & vbTab &
                                                                                     '    OverlapFadeOutArray.Length & vbTab &
                                                                                     '    CrossFadeProgress & vbTab &
                                                                                     '    OutputSoundA.WaveData.ShortestChannelSampleCount & vbTab &
                                                                                     '    PositionA & vbTab &
                                                                                     '    OverlapFadeInArray.Length & vbCrLf & vbCrLf & ex.ToString, "ExceptionsDuringTesting")
                                                                                     '       'CurrentSiBTestStimulusFileName & vbTab &

                                                                                     '    Case OutputSounds.FadingToB
                                                                                     '        Utils.SendInfoToLog("Error in FadingToB" & vbCrLf &
                                                                                     '                      "CurrentSiBTestStimulusFileName:" & vbTab &
                                                                                     '                      "callbackBuffer.length: " & vbTab &
                                                                                     '        "PlayBackSoundFormat.Channels: " & vbTab &
                                                                                     '        "OutputSoundB.WaveData.ShortestChannelSampleCount: " & vbTab &
                                                                                     '        "PositionB: " & vbTab &
                                                                                     '        "OverlapFadeOutArray.length: " & vbTab &
                                                                                     '        "CrossFadeProgress: " & vbTab &
                                                                                     '        "OutputSoundA.WaveData.ShortestChannelSampleCount: " & vbTab &
                                                                                     '        "PositionA: " & vbTab &
                                                                                     '        "OverlapFadeInArray.length: " & vbCrLf &
                                                                                     '        PlaybackBuffer.Length & vbTab &
                                                                                     '        NumberOfOutputChannels & vbTab &
                                                                                     '        OutputSoundB.WaveData.ShortestChannelSampleCount & vbTab &
                                                                                     '        PositionB & vbTab &
                                                                                     '        OverlapFadeOutArray.Length & vbTab &
                                                                                     '        CrossFadeProgress & vbTab &
                                                                                     '        OutputSoundA.WaveData.ShortestChannelSampleCount & vbTab &
                                                                                     '        PositionA & vbTab &
                                                                                     '        OverlapFadeInArray.Length & vbCrLf & vbCrLf & ex.ToString, "ExceptionsDuringTesting")
                                                                                     '        'CurrentSiBTestStimulusFileName & vbTab &
                                                                                     '    Case Else
                                                                                     '        Utils.SendInfoToLog("Nonfading Exception in " & CurrentOutputSound.ToString & vbCrLf &
                                                                                     '                      "CurrentSiBTestStimulusFileName:" & vbTab &
                                                                                     '                      "callbackBuffer.length: " & vbTab &
                                                                                     '       "PlayBackSoundFormat.Channels: " & vbTab &
                                                                                     '       "OutputSoundB.WaveData.SampleData.length: " & vbTab &
                                                                                     '       "PositionB: " & vbTab &
                                                                                     '       "OverlapFadeOutArray.length: " & vbTab &
                                                                                     '       "CrossFadeProgress: " & vbTab &
                                                                                     '       "OutputSoundA.WaveData.SampleData.length: " & vbTab &
                                                                                     '       "PositionA: " & vbTab &
                                                                                     '       "OverlapFadeInArray.length: " & vbCrLf &
                                                                                     '       PlaybackBuffer.Length & vbTab &
                                                                                     '       NumberOfOutputChannels & vbTab &
                                                                                     '       OutputSoundB.WaveData.SampleData.Length & vbTab &
                                                                                     '       PositionB & vbTab &
                                                                                     '       OverlapFadeOutArray.Length & vbTab &
                                                                                     '       CrossFadeProgress & vbTab &
                                                                                     '       OutputSoundA.WaveData.SampleData.Length & vbTab &
                                                                                     '       PositionA & vbTab &
                                                                                     '       OverlapFadeInArray.Length & vbCrLf & vbCrLf & ex.ToString, "ExceptionsDuringTesting")
                                                                                     '        'CurrentSiBTestStimulusFileName & vbTab &
                                                                                     'End Select

                                                                                     ''Creating a simple reset of position holders
                                                                                     'Select Case CurrentOutputSound
                                                                                     '    Case OutputSounds.FadingToA
                                                                                     '        PositionA = 0
                                                                                     '        PositionB = 0
                                                                                     '        CrossFadeProgress = 0
                                                                                     '        CurrentOutputSound = OutputSounds.OutputSoundA

                                                                                     '    Case OutputSounds.FadingToB
                                                                                     '        PositionA = 0
                                                                                     '        PositionB = 0
                                                                                     '        CrossFadeProgress = 0
                                                                                     '        CurrentOutputSound = OutputSounds.OutputSoundB

                                                                                     '    Case OutputSounds.OutputSoundA
                                                                                     '        PositionA = InitialPosA + frameCount
                                                                                     '        PositionB = 0
                                                                                     '        CrossFadeProgress = 0
                                                                                     '    Case OutputSounds.OutputSoundB
                                                                                     '        PositionA = 0
                                                                                     '        PositionB = InitialPosB + frameCount
                                                                                     '        CrossFadeProgress = 0
                                                                                     'End Select


                                                                                     ''Setting positions to the next 100% readable sections
                                                                                     ''Select Case CurrentOutputSound
                                                                                     ''    Case OutputSounds.FadingToA
                                                                                     ''        PositionA = InitialPosA + frameCount
                                                                                     ''        '_PositionB = 0 'This should not have to be set to 0 since it is checked above 
                                                                                     ''        CrossFadeProgress = InitialCrossFadeProgress + frameCount
                                                                                     ''    'CurrentOutputSound = OutputSounds.OutputSoundA
                                                                                     ''    Case OutputSounds.FadingToB
                                                                                     ''        '_PositionA = 0 'This should not have to be set to 0 since it is checked above 
                                                                                     ''        PositionB = InitialPosB + frameCount
                                                                                     ''        CrossFadeProgress = InitialCrossFadeProgress + frameCount
                                                                                     ''    'CurrentOutputSound = OutputSounds.OutputSoundB
                                                                                     ''    Case OutputSounds.OutputSoundA
                                                                                     ''        PositionA = InitialPosA + frameCount
                                                                                     ''        PositionB = 0
                                                                                     ''        CrossFadeProgress = 0
                                                                                     ''    Case OutputSounds.OutputSoundB
                                                                                     ''        PositionA = 0
                                                                                     ''        PositionB = InitialPosB + frameCount
                                                                                     ''        CrossFadeProgress = 0
                                                                                     ''End Select

                                                                                     ''Checking if overlap fade is complete
                                                                                     ''If CrossFadeProgress > OverlapFadeLength - 1 Then
                                                                                     ''    'Setting new output sound
                                                                                     ''    Select Case CurrentOutputSound
                                                                                     ''        Case OutputSounds.FadingToB
                                                                                     ''            CurrentOutputSound = OutputSounds.OutputSoundB
                                                                                     ''        Case OutputSounds.FadingToA
                                                                                     ''            CurrentOutputSound = OutputSounds.OutputSoundA
                                                                                     ''    End Select

                                                                                     ''    'Resetting the CrossFadeProgress
                                                                                     ''    CrossFadeProgress = 0

                                                                                     ''    'Correcting read positions
                                                                                     ''    Select Case CurrentOutputSound
                                                                                     ''        Case OutputSounds.OutputSoundA
                                                                                     ''            PositionA = OverlapFadeLength
                                                                                     ''            PositionB = 0

                                                                                     ''        Case OutputSounds.OutputSoundB
                                                                                     ''            PositionB = OverlapFadeLength
                                                                                     ''            PositionA = 0
                                                                                     ''    End Select

                                                                                     ''End If

                                                                                     'Returning silence if an exception occurred
                                                                                     Marshal.Copy(SilentBuffer, 0, output, AudioApiSettings.FramesPerBuffer * NumberOfOutputChannels)
                                                                                     Return PortAudio.PaStreamCallbackResult.paContinue

                                                                                 End Try

                                                                                 'End SyncLock

                                                                             End Function


            'OTHER DECLATATIONS
            Public ReadOnly SoundDirection As SoundDirections
            Public ReadOnly NumberOfOutputChannels As Integer
            Public ReadOnly NumberOfInputChannels As Integer
            Public ReadOnly AudioEncoding As Formats.WaveFormat.WaveFormatEncodings
            Public ReadOnly AudioBitDepth As Integer
            Private ReadOnly PaSampleFormat As PortAudio.PaSampleFormat

            Public Function GetSampleRate() As Integer
                Return AudioApiSettings.SampleRate
            End Function

            Public PositionA As Integer
            Public PositionB As Integer

            Private _IsInitialized As Boolean = False
            Public ReadOnly Property IsInitialized As Boolean
                Get
                    Return _IsInitialized
                End Get
            End Property

            Private OutputSoundA As BufferHolder()
            Private OutputSoundB As BufferHolder()
            Private NewSound As BufferHolder()
            Private SilentSound As BufferHolder()

            Public Class BufferHolder
                Public InterleavedSampleArray As Single()
                Public ChannelDataList As List(Of Single())
                Public ChannelCount As Integer
                Public FrameCount As Integer

                ''' <summary>
                ''' Holds the (0-based) index of the first sample in the current BufferHolder
                ''' </summary>
                Public StartSample As Integer
                ''' <summary>
                ''' Holds the start time (in seconds) of the first sample in the current BufferHolder
                ''' </summary>
                Public StartTime As Single

                Public Sub New(ByVal ChannelCount As Integer, ByVal FrameCount As Integer)
                    Me.ChannelCount = ChannelCount
                    Me.FrameCount = FrameCount
                    Dim NewInterleavedBuffer(ChannelCount * FrameCount - 1) As Single
                    InterleavedSampleArray = NewInterleavedBuffer
                End Sub

                Public Sub New(ByVal ChannelCount As Integer, ByVal FrameCount As Integer, ByRef InterleavedSampleArray As Single())
                    Me.ChannelCount = ChannelCount
                    Me.FrameCount = FrameCount
                    Me.InterleavedSampleArray = InterleavedSampleArray
                End Sub

                Public Sub ConvertToChannelData(ByRef DuplexMixer As DuplexMixer)
                    Throw New NotImplementedException
                End Sub

            End Class

            Public Property Mixer As DuplexMixer
            Public Class DuplexMixer
                ''' <summary>
                ''' A list of key-value pairs, where the key repressents the hardware output channel and the value repressents the wave file channel from which the output sound should be drawn.
                ''' </summary>
                Public OutputRouting As New SortedList(Of Integer, Integer)
                ''' <summary>
                ''' A list of key-value pairs, where the key repressents the hardware input channel and the value repressents the wave file channel in which the input sound should be stored.
                ''' </summary>
                Public InputRouting As New SortedList(Of Integer, Integer)

                Public ReadOnly AvailableOutputChannels As Integer
                Public ReadOnly AvailableInputChannels As Integer

                ''' <summary>
                ''' Creating a new mixer.
                ''' </summary>
                ''' <param name="AvailableOutputChannels"></param>
                ''' <param name="AvailableInputChannels"></param>
                Public Sub New(ByVal AvailableOutputChannels As Integer, ByVal AvailableInputChannels As Integer)

                    Me.AvailableOutputChannels = AvailableOutputChannels
                    Me.AvailableInputChannels = AvailableInputChannels

                    For c = 1 To AvailableOutputChannels
                        OutputRouting.Add(c, 0)
                    Next

                    For c = 1 To AvailableInputChannels
                        InputRouting.Add(c, 0)
                    Next

                End Sub

                Public Sub DirectMonoSoundToOutputChannel(ByRef TargetOutputChannel As Integer)
                    If OutputRouting.ContainsKey(TargetOutputChannel) Then OutputRouting(TargetOutputChannel) = 1
                End Sub

                Public Sub DirectMonoSoundToOutputChannels(ByRef TargetOutputChannels() As Integer)
                    For Each OutputChannel In TargetOutputChannels
                        If OutputRouting.ContainsKey(OutputChannel) Then OutputRouting(OutputChannel) = 1
                    Next
                End Sub

            End Class

            Public OverlappingSounds As Boolean = False
            Public EqualPowerCrossFade As Boolean = True
            Public OverlappingFadeType As FadeTypes = FadeTypes.Linear
            Public Enum FadeTypes
                Linear
                Smooth
            End Enum

            Private CurrentOutputSound As OutputSounds = OutputSounds.OutputSoundA
            Private Enum OutputSounds
                OutputSoundA
                OutputSoundB
                FadingToB
                FadingToA
            End Enum

            Private Sub SetOverlapDuration(ByVal Duration As Single)
                OverlapFadeLength = NumberOfOutputChannels * AudioApiSettings.SampleRate * Duration
            End Sub

            Public Function GetOverlapDuration() As Single
                Return (_OverlapFadeLength / NumberOfOutputChannels) / AudioApiSettings.SampleRate
            End Function


            Private _OverlapFadeLength As Double
            ''' <summary>
            ''' A value that holds the number of overlapping samples between two sounds. Setting this value automatically creates overlap fade arrays (OverlapFadeInArray and OverlapFadeOutArray). 
            ''' </summary>
            ''' <returns></returns>
            Private Property OverlapFadeLength As Double
                Get
                    Return _OverlapFadeLength
                End Get
                Set(value As Double)
                    Try

                        _OverlapFadeLength = Int(value / (NumberOfOutputChannels * AudioApiSettings.FramesPerBuffer)) * (NumberOfOutputChannels * AudioApiSettings.FramesPerBuffer)

                        Dim OverLapFrameCount As Integer = _OverlapFadeLength / NumberOfOutputChannels

                        Select Case OverlappingFadeType
                            Case FadeTypes.Linear
                                'Linear fading
                                'fade in array
                                ReDim OverlapFadeInArray(_OverlapFadeLength - 1)
                                For n = 0 To OverLapFrameCount - 1
                                    For c = 0 To NumberOfOutputChannels - 1
                                        OverlapFadeInArray(n * NumberOfOutputChannels + c) = n / (OverLapFrameCount - 1)
                                    Next
                                Next

                                'fade out array
                                ReDim OverlapFadeOutArray(_OverlapFadeLength - 1)
                                For n = 0 To OverLapFrameCount - 1
                                    For c = 0 To NumberOfOutputChannels - 1
                                        OverlapFadeOutArray(n * NumberOfOutputChannels + c) = 1 - (n / (OverLapFrameCount - 1))
                                    Next
                                Next

                            Case FadeTypes.Smooth

                                'Smooth fading
                                'fade in array
                                ReDim OverlapFadeInArray(_OverlapFadeLength - 1)

                                'fade out array
                                ReDim OverlapFadeOutArray(_OverlapFadeLength - 1)

                                Dim FadeProgress As Single = 0
                                Dim currentModFactor As Single
                                Dim StartFactor As Single = 0
                                Dim endFactor As Single = 1
                                For n = 0 To _OverlapFadeLength - 1
                                    'fadeProgress goes from 0 to 1 during the fade section
                                    FadeProgress = n / (_OverlapFadeLength - 1)

                                    'Modifies currentFadeFactor according to a cosine finction, whereby currentModFactor starts on 1 and end at 0
                                    currentModFactor = ((Math.Cos(twopi * (FadeProgress / 2)) + 1) / 2)
                                    OverlapFadeInArray(n) = StartFactor * currentModFactor + endFactor * (1 - currentModFactor)

                                    'Setting the fade out array values to 1-(fade in array values) to create an exact inverse, which allways adds up to 1 during fading.
                                    OverlapFadeOutArray(n) = 1 - OverlapFadeInArray(n)
                                Next

                        End Select

                        'Adjusting to equal power fades
                        If EqualPowerCrossFade = True Then
                            For n = 0 To OverlapFadeInArray.Length - 1
                                OverlapFadeInArray(n) = Math.Sqrt(OverlapFadeInArray(n))
                            Next
                            For n = 0 To OverlapFadeOutArray.Length - 1
                                OverlapFadeOutArray(n) = Math.Sqrt(OverlapFadeOutArray(n))
                            Next
                        End If

                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try

                End Set
            End Property
            Private OverlapFadeInArray As Single()
            Private OverlapFadeOutArray As Single()


            Private InputBufferHistory As New List(Of Single())

            Property _RecordingSoundFormat As Formats.WaveFormat
            Property RecordingSoundFormat As Formats.WaveFormat
                Private Set(value As Formats.WaveFormat)
                    _RecordingSoundFormat = value
                End Set
                Get
                    Return _RecordingSoundFormat
                End Get
            End Property

            Public StopAtOutputSoundEnd As Boolean
            'Public EndOfOutputSound_A_IsReached As Boolean = False
            'Public EndOfOutputSound_B_IsReached As Boolean = False

            Private _AudioApiSettings As AudioApiSettings
            Property AudioApiSettings As AudioApiSettings
                Private Set(value As AudioApiSettings)
                    _AudioApiSettings = value
                End Set
                Get
                    Return _AudioApiSettings
                End Get
            End Property

            Private stream As IntPtr
            Private disposed As Boolean = False

            Private Shared m_messagesEnabled As Boolean = False
            Public Shared Property MessagesEnabled() As Boolean
                Get
                    Return m_messagesEnabled
                End Get
                Set
                    m_messagesEnabled = Value
                End Set
            End Property

            Private Shared m_loggingEnabled As Boolean = False
            Public Shared Property LoggingEnabled() As Boolean
                Get
                    Return m_loggingEnabled
                End Get
                Set
                    m_loggingEnabled = Value
                End Set
            End Property

            Private _IsPlaying As Boolean = False
            Public ReadOnly Property IsPlaying As Boolean
                Get
                    Return _IsPlaying
                End Get
            End Property

            Private _IsStreamOpen As Boolean = False
            Public ReadOnly Property IsStreamOpen As Boolean
                Get
                    Return _IsStreamOpen
                End Get
            End Property

            Private RecordingIsActive As Boolean
            Private PlaybackIsActive As Boolean

            Private _IsClippingInactivated As Boolean = False
            Public ReadOnly Property IsClippingInactivated As Boolean
                Get
                    Return _IsClippingInactivated
                End Get
            End Property

            Private CloseStreamAfterPlayCompletion As Boolean = False
            Private ApproachingEndOfBufferAlert_BufferCount As Integer
            Private IsBufferTickActive As Boolean

            Public Enum SoundDirections
                PlaybackOnly
                RecordingOnly
                Duplex
            End Enum

            Public Sub New(ByRef SoundPlayerController As PlayBack.ISoundPlayerControl,
                      Optional SoundDirection As SoundDirections = SoundDirections.PlaybackOnly,
                       Optional ByRef AudioApiSettings As AudioApiSettings = Nothing,
                       Optional ByVal AudioEncoding As Formats.WaveFormat.WaveFormatEncodings = Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints,
                       Optional ByVal LoggingEnabled As Boolean = False,
                       Optional ByVal MessagesEnabled As Boolean = False,
                       Optional StopAtOutputSoundEnd As Boolean = False,
                       Optional InactivateClipping As Boolean = False,
                       Optional OverlapDuration As Double = 1,
                       Optional ByVal ApproachingEndOfBufferAlert_BufferCount As Integer = 1,
                       Optional ByVal ActivateBufferTicks As Boolean = False)

                Me.IsBufferTickActive = ActivateBufferTicks
                Me.MyController = SoundPlayerController
                Me.ApproachingEndOfBufferAlert_BufferCount = ApproachingEndOfBufferAlert_BufferCount

                Try
                    Me.SoundDirection = SoundDirection
                    Select Case SoundDirection
                        Case SoundDirections.PlaybackOnly
                            PlaybackIsActive = True
                            RecordingIsActive = False
                        Case SoundDirections.RecordingOnly
                            PlaybackIsActive = False
                            RecordingIsActive = True
                        Case SoundDirections.Duplex
                            PlaybackIsActive = True
                            RecordingIsActive = True
                        Case Else
                            Throw New Exception("Invalid sound direction")
                    End Select

                    Me.AudioEncoding = AudioEncoding
                    Select Case Me.AudioEncoding 'Bit depth is here assumed from encoding...
                        Case Formats.WaveFormat.WaveFormatEncodings.PCM
                            AudioBitDepth = 16
                            PaSampleFormat = PortAudio.PaSampleFormat.paInt16
                        Case Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints
                            AudioBitDepth = 32
                            PaSampleFormat = PortAudio.PaSampleFormat.paFloat32
                        Case Else
                            Throw New Exception("Unsuppported audio encoding.")
                    End Select

                    'Setting clipping
                    Me._IsClippingInactivated = InactivateClipping

                    'Overriding any value set in InitializationSuccess
                    _IsInitialized = False

                    SoundPlayer.LoggingEnabled = LoggingEnabled 'TODO: NB this is most likely a bug. It should be OverlappingSoundPlayer
                    SoundPlayer.MessagesEnabled = MessagesEnabled 'TODO: NB this is most likely a bug. It should be OverlappingSoundPlayer
                    Log("Initializing...")


                    'Initializing PA
                    If ErrorCheck("Initialize", PortAudio.Pa_Initialize(), True) = True Then
                        Me.disposed = True
                        ' if Pa_Initialize() returns an error code, 
                        ' Pa_Terminate() should NOT be called.
                        Throw New Exception("Can't initialize audio")
                    End If

                    'Setting API settings if not already done
                    If AudioApiSettings Is Nothing Then
                        'Dim FixedSampleRate As Integer? = Nothing
                        Dim newAudioSettingsDialog As New AudioSettingsDialog()
                        Dim DialogResult = newAudioSettingsDialog.ShowDialog()
                        If DialogResult = DialogResult.OK Then
                            AudioApiSettings = newAudioSettingsDialog.CurrentAudioApiSettings
                        Else
                            MsgBox("Did not initialize PaSoundPlayer due to missing audio settings.")
                            Throw New Exception("Did not initialize PaSoundPlayer due to missing audio settings.")
                            Log(ErrorCheck("Terminate", PortAudio.Pa_Terminate(), True))
                            Exit Sub
                        End If
                    End If

                    'Setting Me.audioApiSettings
                    Me.AudioApiSettings = AudioApiSettings

                    'Storing the number of input and output channels
                    If Not Me.AudioApiSettings.SelectedInputDeviceInfo Is Nothing Then
                        NumberOfInputChannels = Me.AudioApiSettings.SelectedInputDeviceInfo.Value.maxInputChannels
                    End If
                    If Not Me.AudioApiSettings.SelectedOutputDeviceInfo Is Nothing Then
                        NumberOfOutputChannels = Me.AudioApiSettings.SelectedOutputDeviceInfo.Value.maxOutputChannels
                    End If
                    If Not Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo Is Nothing Then
                        NumberOfInputChannels = Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo.Value.maxInputChannels
                        NumberOfOutputChannels = Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo.Value.maxOutputChannels
                    End If

                    Log("Selected HostAPI:" & vbLf & Me.AudioApiSettings.SelectedApiInfo.ToString())
                    If Not Me.AudioApiSettings.SelectedInputDeviceInfo Is Nothing Then Log("Selected input device:" & vbLf & Me.AudioApiSettings.SelectedInputDeviceInfo.ToString())
                    If Not Me.AudioApiSettings.SelectedOutputDeviceInfo Is Nothing Then Log("Selected output device:" & vbLf & Me.AudioApiSettings.SelectedOutputDeviceInfo.ToString())
                    If Not Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo Is Nothing Then Log("Selected input and output device:" & vbLf & Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo.ToString())

                    'Setting OverlapFadeLength (and creating fade arrays)
                    SetOverlapDuration(OverlapDuration)

                    'Creating a default mixer if none is supplied
                    If Mixer Is Nothing Then
                        Me.Mixer = New DuplexMixer(NumberOfOutputChannels, NumberOfInputChannels)
                        'Me.Mixer.DirectMonoSoundToOutputChannel(1)
                        Me.Mixer.DirectMonoSoundToOutputChannels({1, 2})

                    Else
                        Me.Mixer = Mixer
                    End If

                    _IsInitialized = True

                Catch e As Exception
                    Log(ErrorCheck("Terminate", PortAudio.Pa_Terminate(), True))
                    Log(e.ToString())
                End Try
            End Sub


            ''' <summary>
            ''' Swaps the current output sound to a new, using crossfading between ths sounds.
            ''' </summary>
            ''' <param name="NewOutputSound"></param>
            ''' <returns>Returns True if successful, or False if unsuccessful.</returns>
            Public Function SwapOutputSounds(ByRef NewOutputSound As Sound) As Boolean

                'Calling [Stop] with fade out to if the new output sound is Nothing
                If NewOutputSound Is Nothing Then
                    [Stop](True)
                    Return False
                End If

                'Checking that the new sound is at least 1 sample long
                If NewOutputSound.WaveData.LongestChannelSampleCount = 0 Then
                    Log("Error: New sound is contains no sample data (SwapOutputSounds).")
                    Return False
                End If

                If NewOutputSound.WaveData.HasUnequalNonZeroChannelLength = True Then
                    Log("Error: New sound have non-empty channels that differ in length. This is not allowed in SwapOutputSounds.")
                    Return False
                End If

                'Checks that sound is playing
                If _IsPlaying = False Then
                    Log("Error: SwapOutputSounds is only effective during active playback.")
                    Return False
                End If

                'Checking that the format is the same format, and returns False if not
                If NewOutputSound.WaveFormat.SampleRate <> AudioApiSettings.SampleRate Or
                            NewOutputSound.WaveFormat.BitDepth <> AudioBitDepth Or
                            NewOutputSound.WaveFormat.Encoding <> AudioEncoding Then
                    Log("Error: Different formats in SwapOutputSounds.")
                    Return False
                End If


                'Setting NewSound to the NewOutputSound to indicate that the output sound should be swapped by the callback
                'NewSound = CreateBufferHolders(NewOutputSound)

                NewSound = CreateBufferHoldersOnNewThread(NewOutputSound)


                Return True

            End Function


            Public Function CreateBufferHolders(ByRef InputSound As Sound) As BufferHolder()

                Dim BufferCount As Integer = Int(InputSound.WaveData.LongestChannelSampleCount / AudioApiSettings.FramesPerBuffer) + 1

                Dim Output(BufferCount - 1) As BufferHolder

                'Initializing the BufferHolders
                For b = 0 To Output.Length - 1
                    Output(b) = New BufferHolder(NumberOfOutputChannels, AudioApiSettings.FramesPerBuffer)
                Next

                Dim CurrentChannelInterleavedPosition As Integer
                For Each OutputChannel In Mixer.OutputRouting

                    If OutputChannel.Value = 0 Then Continue For

                    If OutputChannel.Value > InputSound.WaveFormat.Channels Then Continue For

                    'Skipping if channel contains no data
                    If InputSound.WaveData.SampleData(OutputChannel.Value).Length = 0 Then Continue For

                    CurrentChannelInterleavedPosition = OutputChannel.Key - 1

                    'Reading samples
                    For BufferIndex = 0 To Output.Length - 2
                        Dim CurrentWriteSampleIndex As Integer = 0
                        For Sample = BufferIndex * AudioApiSettings.FramesPerBuffer To (BufferIndex + 1) * AudioApiSettings.FramesPerBuffer - 1

                            Output(BufferIndex).InterleavedSampleArray(CurrentWriteSampleIndex * NumberOfOutputChannels + CurrentChannelInterleavedPosition) = InputSound.WaveData.SampleData(OutputChannel.Value)(Sample)
                            CurrentWriteSampleIndex += 1
                        Next
                    Next

                    'Reading the last bit
                    Dim CurrentWriteSampleIndexB As Integer = 0
                    For Sample = AudioApiSettings.FramesPerBuffer * Output.Length - 1 To InputSound.WaveData.SampleData(OutputChannel.Value).Length - 1

                        Output(Output.Length - 1).InterleavedSampleArray(CurrentWriteSampleIndexB * NumberOfOutputChannels + CurrentChannelInterleavedPosition) = InputSound.WaveData.SampleData(OutputChannel.Value)(Sample)
                        CurrentWriteSampleIndexB += 1
                    Next
                Next

                Return Output

            End Function


            Public Function CreateBufferHoldersOnNewThread(ByRef InputSound As Sound, Optional ByVal BuffersOnMainThread As Integer = 10) As BufferHolder()

                Dim BufferCount As Integer = Int(InputSound.WaveData.LongestChannelSampleCount / AudioApiSettings.FramesPerBuffer) + 1

                Dim Output(BufferCount - 1) As BufferHolder

                'Initializing the BufferHolders
                For b = 0 To Output.Length - 1
                    Output(b) = New BufferHolder(NumberOfOutputChannels, AudioApiSettings.FramesPerBuffer)
                Next

                'Creating the BuffersOnMainThread first buffers
                'Limiting the number of main thread buffers if the sound is very short
                If BuffersOnMainThread < Output.Length - 2 Then
                    BuffersOnMainThread = Math.Max(0, Output.Length - 1)
                End If


                Dim CurrentChannelInterleavedPosition As Integer
                For Each OutputChannel In Mixer.OutputRouting

                    If OutputChannel.Value = 0 Then Continue For

                    If OutputChannel.Value > InputSound.WaveFormat.Channels Then Continue For

                    'Skipping if channel contains no data
                    If InputSound.WaveData.SampleData(OutputChannel.Value).Length = 0 Then Continue For

                    CurrentChannelInterleavedPosition = OutputChannel.Key - 1

                    'Going through buffer by buffer
                    For BufferIndex = 0 To BuffersOnMainThread - 1

                        'Setting start sample and time
                        Output(BufferIndex).StartSample = BufferIndex * AudioApiSettings.FramesPerBuffer
                        Output(BufferIndex).StartTime = BufferIndex * AudioApiSettings.FramesPerBuffer / AudioApiSettings.SampleRate

                        'Shuffling samples from the input sound to the interleaved array
                        Dim CurrentWriteSampleIndex As Integer = 0
                        For Sample = BufferIndex * AudioApiSettings.FramesPerBuffer To (BufferIndex + 1) * AudioApiSettings.FramesPerBuffer - 1

                            Output(BufferIndex).InterleavedSampleArray(CurrentWriteSampleIndex * NumberOfOutputChannels + CurrentChannelInterleavedPosition) = InputSound.WaveData.SampleData(OutputChannel.Value)(Sample)
                            CurrentWriteSampleIndex += 1
                        Next
                    Next
                Next

                'Fixes the rest of the buffers on a new thread, allowing the new sound to start playing
                Dim ThreadWork As New BufferCreaterOnNewThread(InputSound, Output, BuffersOnMainThread,
                                                                   NumberOfOutputChannels, Mixer, AudioApiSettings)

                Return Output

            End Function

            Private Class BufferCreaterOnNewThread
                Implements IDisposable

                Dim InputSound As Sound
                Dim Output As BufferHolder()
                Dim BuffersOnMainThread As Integer
                Dim NumberOfOutputChannels As Integer
                Dim Mixer As DuplexMixer
                Dim AudioApiSettings As AudioApiSettings

                Public Sub New(ByRef InputSound As Sound, ByRef Output As BufferHolder(), ByVal BuffersOnMainThread As Integer,
                             ByVal NumberOfOutputChannels As Integer, ByRef Mixer As DuplexMixer, ByRef AudioApiSettings As AudioApiSettings)
                    Me.InputSound = InputSound
                    Me.Output = Output
                    Me.BuffersOnMainThread = BuffersOnMainThread
                    Me.NumberOfOutputChannels = NumberOfOutputChannels
                    Me.Mixer = Mixer
                    Me.AudioApiSettings = AudioApiSettings

                    'Starting the new worker thread
                    Dim NewThred As New Thread(AddressOf DoWork)
                    NewThred.Start()

                End Sub

                Private Sub DoWork()

                    Dim CurrentChannelInterleavedPosition As Integer
                    For Each OutputChannel In Mixer.OutputRouting

                        If OutputChannel.Value = 0 Then Continue For

                        If OutputChannel.Value > InputSound.WaveFormat.Channels Then Continue For

                        CurrentChannelInterleavedPosition = OutputChannel.Key - 1

                        'Going through buffer by buffer
                        For BufferIndex = BuffersOnMainThread To Output.Length - 2

                            'Setting start sample and time
                            Output(BufferIndex).StartSample = BufferIndex * AudioApiSettings.FramesPerBuffer
                            Output(BufferIndex).StartTime = BufferIndex * AudioApiSettings.FramesPerBuffer / AudioApiSettings.SampleRate

                            'Shuffling samples from the input sound to the interleaved array
                            Dim CurrentWriteSampleIndex As Integer = 0
                            For Sample = BufferIndex * AudioApiSettings.FramesPerBuffer To (BufferIndex + 1) * AudioApiSettings.FramesPerBuffer - 1

                                Output(BufferIndex).InterleavedSampleArray(CurrentWriteSampleIndex * NumberOfOutputChannels + CurrentChannelInterleavedPosition) = InputSound.WaveData.SampleData(OutputChannel.Value)(Sample)
                                CurrentWriteSampleIndex += 1
                            Next
                        Next

                        'Reading the last bit
                        'Setting start sample and time
                        Output(Output.Length - 1).StartSample = (Output.Length - 1) * AudioApiSettings.FramesPerBuffer
                        Output(Output.Length - 1).StartTime = (Output.Length - 1) * AudioApiSettings.FramesPerBuffer / AudioApiSettings.SampleRate

                        'Shuffling samples from the input sound to the interleaved array
                        Dim CurrentWriteSampleIndexB As Integer = 0
                        For Sample = AudioApiSettings.FramesPerBuffer * Output.Length - 1 To InputSound.WaveData.SampleData(OutputChannel.Value).Length - 1

                            Output(Output.Length - 1).InterleavedSampleArray(CurrentWriteSampleIndexB * NumberOfOutputChannels + CurrentChannelInterleavedPosition) = InputSound.WaveData.SampleData(OutputChannel.Value)(Sample)
                            CurrentWriteSampleIndexB += 1
                        Next
                    Next

                    'Disposing Me
                    Me.Dispose()

                End Sub

#Region "IDisposable Support"
                Private disposedValue As Boolean ' To detect redundant calls

                ' IDisposable
                Protected Overridable Sub Dispose(disposing As Boolean)
                    If Not disposedValue Then
                        If disposing Then
                            ' TODO: dispose managed state (managed objects).
                        End If

                        ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                        ' TODO: set large fields to null.
                    End If
                    disposedValue = True
                End Sub

                ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
                'Protected Overrides Sub Finalize()
                '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                '    Dispose(False)
                '    MyBase.Finalize()
                'End Sub

                ' This code added by Visual Basic to correctly implement the disposable pattern.
                Public Sub Dispose() Implements IDisposable.Dispose
                    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                    Dispose(True)
                    ' TODO: uncomment the following line if Finalize() is overridden above.
                    ' GC.SuppressFinalize(Me)
                End Sub
#End Region
            End Class

            ''' <summary>
            ''' Changes the position of the currently playing sound to the start of the buffer containing the indicated time (in seconds). Returns the exact time selected as new start time. 
            ''' </summary>
            Public Function SeekTime(ByVal Time As Single) As Single

                Dim SelectedTime As Single

                Select Case CurrentOutputSound
                    Case OutputSounds.OutputSoundA, OutputSounds.FadingToA

                        'Locating the buffer containing the indicated start sample
                        Dim NewStartPosition As Integer = 0
                        For BufferIndex = 0 To OutputSoundA.Length - 1
                            If Time < OutputSoundA(BufferIndex).StartTime Then
                                NewStartPosition = BufferIndex
                                Exit For
                            End If
                        Next

                        'Limiting NewStartPosition to positive values, and values lower than the length of OutputSoundA, and then sets the new PositionA
                        PositionA = Math.Min(Math.Max(0, NewStartPosition), OutputSoundA.Length - 1)

                        'Stores the selected start time
                        SelectedTime = OutputSoundA(PositionA).StartTime

                        'Killing any fade process, and sets CurrentOutputSound to OutputSoundA
                        If CurrentOutputSound = OutputSounds.FadingToA Then
                            CurrentOutputSound = OutputSounds.OutputSoundA
                            CrossFadeProgress = 0
                        End If

                    Case OutputSounds.OutputSoundB, OutputSounds.FadingToB

                        'Locating the buffer containing the indicated start sample
                        Dim NewStartPosition As Integer = 0
                        For BufferIndex = 0 To OutputSoundB.Length - 1
                            If Time < OutputSoundB(BufferIndex).StartTime Then
                                NewStartPosition = BufferIndex
                                Exit For
                            End If
                        Next

                        'Limiting NewStartPosition to positive values, and values lower than the length of OutputSoundB, and then sets the new PositionB
                        PositionB = Math.Min(Math.Max(0, NewStartPosition), OutputSoundB.Length - 1)

                        'Stores the selected start time
                        SelectedTime = OutputSoundB(PositionB).StartTime

                        'Killing any fade process, and sets CurrentOutputSound to OutputSoundB
                        If CurrentOutputSound = OutputSounds.FadingToB Then
                            CurrentOutputSound = OutputSounds.OutputSoundB
                            CrossFadeProgress = 0
                        End If

                    Case Else
                        Throw New NotImplementedException
                End Select

                Return SelectedTime

            End Function

            ''' <summary>
            ''' Changes the position of the currently playing sound to the start of the buffer containing the indicated sample. Returns the exact sample selected as new start position. 
            ''' </summary>
            Public Function SeekSample(ByVal StartSample As Integer) As Integer

                Dim SelectedSample As Integer

                Select Case CurrentOutputSound
                    Case OutputSounds.OutputSoundA, OutputSounds.FadingToA

                        'Locating the buffer containing the indicated start sample
                        Dim NewStartPosition As Integer = 0
                        For BufferIndex = 0 To OutputSoundA.Length - 1
                            If StartSample < OutputSoundA(BufferIndex).StartSample Then
                                NewStartPosition = BufferIndex
                                Exit For
                            End If
                        Next

                        'Limiting NewStartPosition to positive values, and values lower than the length of OutputSoundA, and then sets the new PositionA
                        PositionA = Math.Min(Math.Max(0, NewStartPosition), OutputSoundA.Length - 1)

                        'Stores the selected start time
                        SelectedSample = OutputSoundA(PositionA).StartSample

                        'Killing any fade process, and sets CurrentOutputSound to OutputSoundA
                        If CurrentOutputSound = OutputSounds.FadingToA Then
                            CurrentOutputSound = OutputSounds.OutputSoundA
                            CrossFadeProgress = 0
                        End If

                    Case OutputSounds.OutputSoundB, OutputSounds.FadingToB

                        'Locating the buffer containing the indicated start sample
                        Dim NewStartPosition As Integer = 0
                        For BufferIndex = 0 To OutputSoundB.Length - 1
                            If StartSample < OutputSoundB(BufferIndex).StartSample Then
                                NewStartPosition = BufferIndex
                                Exit For
                            End If
                        Next

                        'Limiting NewStartPosition to positive values, and values lower than the length of OutputSoundB, and then sets the new PositionB
                        PositionB = Math.Min(Math.Max(0, NewStartPosition), OutputSoundB.Length - 1)

                        'Stores the selected start time
                        SelectedSample = OutputSoundB(PositionB).StartSample

                        'Killing any fade process, and sets CurrentOutputSound to OutputSoundB
                        If CurrentOutputSound = OutputSounds.FadingToB Then
                            CurrentOutputSound = OutputSounds.OutputSoundB
                            CrossFadeProgress = 0
                        End If

                    Case Else
                        Throw New NotImplementedException
                End Select

                Return SelectedSample

            End Function


            Public Sub ClearRecordedSound()
                InputBufferHistory.Clear()
            End Sub

            Public Sub OpenStream()

                Log("Opening stream...")
                Me.stream = StreamOpen()
                Log("Stream pointer: " & stream.ToString())

            End Sub




            ''' <summary>
            ''' Starts the sound stream.
            ''' </summary>
            ''' <param name="AppendRecordedSound">If set to True, the new recording will be appended any previously recorded sound. If set to False, a new recording will be started.</param>
            Public Sub Start(Optional ByVal AppendRecordedSound As Boolean = False,
                         Optional ByVal FadeInSound As Boolean = False)

                'Setting both sounds to silent sound
                SilentSound = {New BufferHolder(NumberOfOutputChannels, AudioApiSettings.FramesPerBuffer)}
                OutputSoundA = SilentSound
                OutputSoundB = SilentSound

                If AppendRecordedSound = False Then
                    ClearRecordedSound()
                End If

                Log("Starting stream")
                If ErrorCheck("StartStream", PortAudio.Pa_StartStream(stream), True) = False Then
                    _IsPlaying = True
                End If

            End Sub


            ''' <summary>
            ''' Stops the output sound.
            ''' </summary>
            ''' <param name="FadeOutSound">Set to true if fading out of the sound shold take place. (The fade out will occur during OverlapFadeLength +1 samples.)</param>
            Public Sub [Stop](Optional ByVal FadeOutSound As Boolean = False)

                'Stops recording directly
                RecordingIsActive = False

                'Calling stop right away if no fade out should be done
                If FadeOutSound = False Then
                    StopNow()
                End If

                'Doing fade out by swapping to SilentSound
                NewSound = SilentSound

            End Sub

            Private Sub StopNow()

                Log("Stopping stream...")

                If ErrorCheck("StopStream", PortAudio.Pa_StopStream(stream), True) = False Then
                    _IsPlaying = False
                End If

            End Sub


            Public Sub AbortStream() 'Optional ByVal StoreInputSound As Boolean = True)

                'Stops recording directly
                RecordingIsActive = False


                Log("Aborting stream...")

                If ErrorCheck("AbortStream", PortAudio.Pa_AbortStream(stream), True) = False Then
                    _IsPlaying = False
                End If

                'If StoreInputSound = True Then
                'Storing recorded sound
                'StoreRecordedSound()
                'End If

            End Sub



            Public Sub CloseStream()

                'Stopping the stream if it is running
                If PortAudio.Pa_IsStreamStopped(Me.stream) < 1 Then
                    [Stop]()
                End If

                'Cloing the stream
                If ErrorCheck("CloseStream", PortAudio.Pa_CloseStream(stream), True) = False Then

                    _IsStreamOpen = False

                    'Resetting the stream
                    Me.stream = New IntPtr(0)
                End If

            End Sub


            Private Function StreamOpen() As IntPtr

                'Setting buffer length data, and adjusting the length of the buffer arrays
                'Dim HighestChannelCount As Integer = Math.Max(NumberOfOutputChannels, NumberOfInputChannels)

                'Do recording and playback buffers need to be of equal length?

                'Setting/updating the length of the playback buffer
                Log("Creating a new playback buffer length with the length: " & Me.AudioApiSettings.FramesPerBuffer * NumberOfOutputChannels)
                PlaybackBuffer = New Single((Me.AudioApiSettings.FramesPerBuffer * NumberOfOutputChannels) - 1) {}
                SilentBuffer = New Single((Me.AudioApiSettings.FramesPerBuffer * NumberOfOutputChannels) - 1) {}

                'Setting/updating the length of the recording buffer
                Log("Creating a new recording buffer length with the length: " & Me.AudioApiSettings.FramesPerBuffer * NumberOfInputChannels)
                RecordingBuffer = New Single((Me.AudioApiSettings.FramesPerBuffer * NumberOfInputChannels) - 1) {}

                Dim stream As New IntPtr()
                Dim data As New IntPtr(0)

                Dim inputParams As New PortAudio.PaStreamParameters
                If Me.AudioApiSettings.SelectedInputDevice IsNot Nothing Then
                    inputParams.channelCount = NumberOfInputChannels
                    inputParams.device = Me.AudioApiSettings.SelectedInputDevice
                    inputParams.sampleFormat = PaSampleFormat

                    If Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo.HasValue = True Then
                        inputParams.suggestedLatency = Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo.Value.defaultLowInputLatency
                    Else
                        inputParams.suggestedLatency = Me.AudioApiSettings.SelectedInputDeviceInfo.Value.defaultLowInputLatency
                    End If
                End If

                Dim outputParams As New PortAudio.PaStreamParameters
                If Me.AudioApiSettings.SelectedOutputDevice IsNot Nothing Then
                    outputParams.channelCount = NumberOfOutputChannels
                    outputParams.device = Me.AudioApiSettings.SelectedOutputDevice
                    outputParams.sampleFormat = PaSampleFormat

                    If Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo.HasValue = True Then
                        outputParams.suggestedLatency = Me.AudioApiSettings.SelectedInputAndOutputDeviceInfo.Value.defaultLowOutputLatency
                    Else
                        outputParams.suggestedLatency = Me.AudioApiSettings.SelectedOutputDeviceInfo.Value.defaultLowOutputLatency
                    End If
                End If

                Log(inputParams.ToString)
                Log(outputParams.ToString)

                Dim Flag As PortAudio.PaStreamFlags
                If IsClippingInactivated = True Then
                    Flag = PortAudio.PaStreamFlags.paClipOff
                Else
                    Flag = PortAudio.PaStreamFlags.paNoFlag
                End If

                Select Case SoundDirection
                    Case SoundDirections.PlaybackOnly
                        ErrorCheck("OpenOutputOnlyStream", PortAudio.Pa_OpenStream(stream, New Nullable(Of PortAudio.PaStreamParameters), outputParams,
                                                                           Me.AudioApiSettings.SampleRate, Me.AudioApiSettings.FramesPerBuffer, Flag, Me.paStreamCallback, data), True)

                    Case SoundDirections.RecordingOnly
                        ErrorCheck("OpenInputOnlyStream", PortAudio.Pa_OpenStream(stream, inputParams, New Nullable(Of PortAudio.PaStreamParameters),
                                                                          Me.AudioApiSettings.SampleRate, Me.AudioApiSettings.FramesPerBuffer, Flag, Me.paStreamCallback, data), True)

                    Case SoundDirections.Duplex
                        ErrorCheck("OpenDuplexStream", PortAudio.Pa_OpenStream(stream, inputParams, outputParams, Me.AudioApiSettings.SampleRate, Me.AudioApiSettings.FramesPerBuffer, Flag,
                                                                       Me.paStreamCallback, data), True)
                End Select

                _IsStreamOpen = True

                Return stream
            End Function


            ''' <summary>
            ''' Gets the recorded sound so far.
            ''' </summary>
            ''' <returns></returns>
            Public Function GetRecordedSound() As Sound

                Log("Attemting to get recorded sound")

                'Stopping sound if not already done
                If _IsPlaying = True Then [Stop](0.1)

                'Returning nothing if no input sound exists
                If RecordingIsActive = False Then Return Nothing
                If InputBufferHistory Is Nothing Then Return Nothing

                'Creating a new Sound
                Dim RecordedSound As New Sound(RecordingSoundFormat)

                If InputBufferHistory.Count = 0 Then Return RecordedSound

                If InputBufferHistory.Count > 0 Then

                    'Determining output sound length
                    Dim OutputSoundSampleCount As Long = 0
                    For Each Buffer In InputBufferHistory
                        OutputSoundSampleCount += Buffer.Length / RecordingSoundFormat.Channels
                    Next

                    For ch = 0 To RecordingSoundFormat.Channels - 1
                        Dim NewChannelArray(OutputSoundSampleCount - 1) As Single
                        RecordedSound.WaveData.SampleData(ch + 1) = NewChannelArray
                    Next

                    'Sorting the interleaved samples to 
                    Dim CurrentBufferStartSample As Long = 0
                    For Each Buffer In InputBufferHistory
                        Dim CurrentBufferSampleIndex As Long = 0
                        For CurrentDataPoint = 0 To Buffer.Length - 1 Step RecordingSoundFormat.Channels

                            For ch = 0 To RecordingSoundFormat.Channels - 1
                                Try
                                    RecordedSound.WaveData.SampleData(ch + 1)(CurrentBufferStartSample + CurrentBufferSampleIndex) = Buffer(CurrentDataPoint + ch)
                                Catch ex As Exception
                                    Log(ErrorCheck("Terminate", PortAudio.Pa_Terminate(), True))
                                End Try
                            Next
                            'Increasing sample index
                            CurrentBufferSampleIndex += 1
                        Next
                        CurrentBufferStartSample += CurrentBufferSampleIndex
                    Next

                End If
                Return RecordedSound

            End Function

            ''' <summary>
            ''' Returns the time delay (in seconds) caused by the call-back buffer size.
            ''' </summary>
            ''' <returns></returns>
            Public Function GetCallBackTime() As Double

                Return AudioApiSettings.FramesPerBuffer / AudioApiSettings.SampleRate

            End Function

            Private Sub Log(logString As String)
                If m_loggingEnabled = True Then
                    System.Console.WriteLine("PortAudio: " & logString)
                End If
            End Sub

            Private Sub DisplayMessageInBox(Message As String)
                If m_messagesEnabled = True Then
                    MsgBox(Message)
                End If
            End Sub

            Public Shared LogToFileEnabled As Boolean = False
            Private Sub LogToFile(Message As String)
                If LogToFileEnabled = True Then
                    SendInfoToAudioLog(Message)
                End If
            End Sub

            Private Function ErrorCheck(action As String, errorCode As PortAudio.PaError, Optional ShowErrorInMsgBox As Boolean = False) As Boolean
                If errorCode <> PortAudio.PaError.paNoError Then
                    Dim MessageA As String = action & " error: " & PortAudio.Pa_GetErrorText(errorCode)
                    Log(MessageA)
                    If ShowErrorInMsgBox = True Then DisplayMessageInBox(MessageA)

                    LogToFile(MessageA)

                    If errorCode = PortAudio.PaError.paUnanticipatedHostError Then
                        Dim errorInfo As PortAudio.PaHostErrorInfo = PortAudio.Pa_GetLastHostErrorInfo()
                        Dim MessageB As String = "- Host error API type: " & errorInfo.hostApiType
                        Dim MessageC As String = "- Host error code: " & errorInfo.errorCode
                        Dim MessageD As String = "- Host error text: " & errorInfo.errorText
                        Log(MessageB)
                        Log(MessageC)
                        Log(MessageD)

                        LogToFile(MessageB)
                        LogToFile(MessageC)
                        LogToFile(MessageD)

                        If ShowErrorInMsgBox = True Then DisplayMessageInBox(MessageB & vbCrLf & MessageC & vbCrLf & MessageD)
                    End If

                    Return True
                Else
                    Log(action & " OK")
                    LogToFile(action & " OK")
                    Return False
                End If
            End Function


            Private Sub SendMessageToController(ByVal Message As PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer,
                                            Optional ByVal SendOnNewThread As Boolean = True)

                If SendOnNewThread = False Then
                    'Sending message on same thread (Should not be used when messages are sent from within the callback!)
                    MyController.MessageFromPlayer(Message)

                Else
                    'Sending message on a new thread, allowing the main thread to continue execution
                    Dim NewthreadMessageSender As New MessageSenderOnNewThread(Message, MyController)

                End If

                'Dim NewThread As New Thread(AddressOf SendMessage)

                'NewThread.Join()
                'Dim 
                'ThreadPool.QueueUserWorkItem(,)
                'MyController.Handle

            End Sub

            ''' <summary>
            ''' A class used to send one ISoundPlayerControl.MessagesFromSoundPlayer message on a new thread.
            ''' </summary>
            Private Class MessageSenderOnNewThread
                Implements IDisposable

                Private Message As PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer
                Private Controller As PlayBack.ISoundPlayerControl

                ''' <summary>
                ''' Sends the supplied message to the indicated Controller directly on initiation and then desposes the sending object.
                ''' </summary>
                ''' <param name="Message"></param>
                ''' <param name="Controller"></param>
                Public Sub New(ByRef Message As PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer, ByRef Controller As PlayBack.ISoundPlayerControl)
                    Me.Message = Message
                    Me.Controller = Controller

                    'Sending message on a new thread
                    Dim NewThred As New Thread(AddressOf SendMessage)
                    NewThred.Start()

                End Sub

                Private Sub SendMessage()

                    'Sending the message
                    Controller.MessageFromPlayer(Message)

                    'Disposing Me
                    Me.Dispose()

                End Sub

#Region "IDisposable Support"
                Private disposedValue As Boolean ' To detect redundant calls

                ' IDisposable
                Protected Overridable Sub Dispose(disposing As Boolean)
                    If Not disposedValue Then
                        If disposing Then
                            ' TODO: dispose managed state (managed objects).
                        End If

                        ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                        ' TODO: set large fields to null.
                    End If
                    disposedValue = True
                End Sub

                ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
                'Protected Overrides Sub Finalize()
                '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                '    Dispose(False)
                '    MyBase.Finalize()
                'End Sub

                ' This code added by Visual Basic to correctly implement the disposable pattern.
                Public Sub Dispose() Implements IDisposable.Dispose
                    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                    Dispose(True)
                    ' TODO: uncomment the following line if Finalize() is overridden above.
                    ' GC.SuppressFinalize(Me)
                End Sub
#End Region

            End Class


#Region "IDisposable Support"
            Private disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not disposedValue Then
                    If disposing Then
                        ' TODO: dispose managed state (managed objects).
                    End If

                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                    Log("Terminating...")
                    ErrorCheck("Terminate", PortAudio.Pa_Terminate(), True)

                End If
                disposedValue = True
            End Sub

            ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
            Protected Overrides Sub Finalize()
                '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(False)
                MyBase.Finalize()
            End Sub

            ' This code added by Visual Basic to correctly implement the disposable pattern.
            Public Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(True)
                ' TODO: uncomment the following line if Finalize() is overridden above.
                GC.SuppressFinalize(Me)
            End Sub

#End Region

        End Class


    End Namespace

End Namespace