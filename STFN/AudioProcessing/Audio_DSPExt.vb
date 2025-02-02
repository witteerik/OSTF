﻿Imports System.IO
Imports System.Threading

Namespace Audio

    Namespace DSP

        Public Module MeasurementsExt

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
        Public Module AcousticDistance

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

                ' N.B. This is the method called when calculating sound distance for the selection of the Swedish SiP-test phoneme contrasts.

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
                Dim FilterredMagnitudesArray As SortedList(Of Integer, Double()) = BarkFilter(InputSound.FFT, InputSound.WaveFormat.SampleRate, BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency, ReusableCentreFrequencies, True)

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
                Dim FrequencyArray(BarkBinCount - 1) As Double
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
                                                       Optional ByRef Sound1BarkSpectrum As Double() = Nothing,
                                                       Optional ByRef Sound2BarkSpectrum As Double() = Nothing,
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
                            Dim FilterredMagnitudesArray As SortedList(Of Integer, Double()) = BarkFilter(Sound1.FFT, Sound1.WaveFormat.SampleRate, BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency, ReusableCentreFrequencies, True)

                            'Referencing the data into the Sound1.FFT.BarkSpectrumTimeWindowData object
                            For Each CurrentTimeWindow In FilterredMagnitudesArray
                                Dim NewTimeWindow As New FftData.TimeWindow
                                NewTimeWindow.WindowData = CurrentTimeWindow.Value
                                Sound1.FFT.AddBarkSpectrumTimeWindowData(NewTimeWindow, 1)
                            Next

                            'Calculating average Bark spectra, and stores in the FFT.TemporaryData Object, for re-use in the next analyses
                            Dim AverageData As New FftData.TimeWindow
                            Dim FrequencyArray(Sound1.FFT.BarkSpectrumTimeWindowData(1, 0).WindowData.Length - 1) As Double
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
                            Dim FilterredMagnitudesArray As SortedList(Of Integer, Double()) = BarkFilter(Sound2.FFT, Sound1.WaveFormat.SampleRate, BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency, ReusableCentreFrequencies, True)

                            'Referencing the data into the Sound1.FFT.BarkSpectrumTimeWindowData object
                            For Each CurrentTimeWindow In FilterredMagnitudesArray
                                Dim NewTimeWindow As New FftData.TimeWindow
                                NewTimeWindow.WindowData = CurrentTimeWindow.Value
                                Sound2.FFT.AddBarkSpectrumTimeWindowData(NewTimeWindow, 1)
                            Next

                            'Calculating average Bark spectra, and stores in the FFT.TemporaryData Object, for re-use in the next analyses
                            Dim AverageData As New FftData.TimeWindow
                            Dim FrequencyArray(Sound2.FFT.BarkSpectrumTimeWindowData(1, 0).WindowData.Length - 1) As Double
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
                            Distance = Utils.Math.GetEuclideanDistance(Sound1.FFT.TemporaryData(0).WindowData, Sound2.FFT.TemporaryData(0).WindowData)
                        Else
                            Distance = Utils.Math.GetEuclideanDistance(Sound1.FFT.TemporaryData(0).WindowData, Sound2.FFT.TemporaryData(0).WindowData, IrrelevantDifferenceThreshold)
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
                Dim FilterredMagnitudesArray1 As SortedList(Of Integer, Double()) = BarkFilter(FftData1, SampleRate, BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency, CentreFrequencies,, UseImprovementsAfterSiB)
                Dim FilterredMagnitudesArray2 As SortedList(Of Integer, Double()) = BarkFilter(FftData2, SampleRate, BarkFilterOverlapRatio, LowestIncludedCentreFrequency, HighestIncludedCentreFrequency, CentreFrequencies,, UseImprovementsAfterSiB)


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
                                    Optional ByVal UseImprovementsAfterSiB As Boolean = True) As SortedList(Of Integer, Double())

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
                    Dim SummedMagnitudesArray As New SortedList(Of Integer, Double())

                    'Looking at one time window at a time
                    For w = 0 To FftData.WindowCount(1) - 1

                        Dim BandMagnitudes(CentreFrequencies.Count - 1) As Double

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


            Private Function GetDistanceValue(ByRef AmplitudeSpectrum1 As SortedList(Of Integer, Double()), ByRef AmplitudeSpectrum2 As SortedList(Of Integer, Double()),
                                             ByVal ColumnIndex As Integer, ByVal RowIndex As Integer) As Double

                'Calculating the Euclidean distance

                Dim Sum As Double = 0
                For n = 0 To AmplitudeSpectrum1(0).Length - 1
                    Sum += (AmplitudeSpectrum1(ColumnIndex)(n) - AmplitudeSpectrum2(RowIndex)(n)) ^ 2
                Next

                Return Math.Sqrt(Sum)

            End Function

        End Module


        Public Module AcousticDistance2


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

        Public Module Radix2TrigonometricLookup

            Private LookupDictionary As New SortedList(Of FftDirections, SortedList(Of Integer, List(Of Tuple(Of Double, Double))))
            Private ArrayDictionary As New SortedList(Of FftDirections, SortedList(Of Integer, Tuple(Of Double(), Double())))

            Public Function GetRadix2TrigonomerticValues(ByVal Size As Integer, ByRef Direction As FftDirections) As List(Of Tuple(Of Double, Double))

                If LookupDictionary.ContainsKey(Direction) = False Then
                    LookupDictionary.Add(Direction, New SortedList(Of Integer, List(Of Tuple(Of Double, Double))))
                    ArrayDictionary.Add(Direction, New SortedList(Of Integer, Tuple(Of Double(), Double())))
                End If

                If LookupDictionary(Direction).ContainsKey(Size) = True Then
                    'No values need to be calculated
                    Return LookupDictionary(Direction)(Size)
                End If

                Dim TrigonometricValues As New List(Of Tuple(Of Double, Double))

                Dim ExponentSign As Integer
                If Direction = FftDirections.Forward Then
                    ExponentSign = -1
                ElseIf Direction = FftDirections.Backward Then
                    ExponentSign = 1
                Else
                    Throw New ArgumentException("Unknown FFT direction")
                End If

                Dim HalfSize As Integer = Size / 2
                For n = 0 To HalfSize - 1
                    Dim LookupKey As Integer = HalfSize * (n / HalfSize)
                    Dim Exponent As Double = ExponentSign * (n / HalfSize) * Math.PI
                    TrigonometricValues.Add(New Tuple(Of Double, Double)(Math.Cos(Exponent), Math.Sin(Exponent)))
                Next

                'Adds the values
                LookupDictionary(Direction).Add(Size, TrigonometricValues)

                Dim PcCos As New List(Of Double)
                Dim PcSin As New List(Of Double)

                For Each kvp In TrigonometricValues
                    PcCos.Add(kvp.Item1)
                    PcSin.Add(kvp.Item2)
                Next

                ArrayDictionary(Direction).Add(Size, New Tuple(Of Double(), Double())(PcCos.ToArray, PcSin.ToArray))

                'And also returns them
                Return TrigonometricValues

            End Function


            Public Function GetArrays(ByVal Size As Integer, ByRef Direction As FftDirections) As Tuple(Of Double(), Double())

                If ArrayDictionary.ContainsKey(Direction) = False Then
                    GetRadix2TrigonomerticValues(Size, Direction)
                End If

                If ArrayDictionary(Direction).ContainsKey(Size) = False Then
                    GetRadix2TrigonomerticValues(Size, Direction)
                End If

                Return ArrayDictionary(Direction)(Size)

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
            ''' <param name="ReturnReport">If ReturnReport is set to True, the function returns a string containing a report of the applied limiting.</param>
            ''' <returns>If ReturnReport is set to True, the function returns a string containing a report of the applied limiting.</returns>
            Public Function SoftLimitSection(ByRef InputSound As Sound,
                                             ByVal Channel As Integer,
                                             ByVal ThresholdLevel As Double,
                                             Optional ByVal StartSample As Integer = 0,
                                             Optional ByVal SectionLength As Integer? = Nothing,
                                             Optional ByVal WindowDuration As Double = 0.2,
                                             Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z,
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

                    'Performs limiting by calling LimitChannelSection
                    'Limiting the sound in the specified channel
                    SoftLimitChannelSection(InputSound, Channel, ThresholdLevel, StartSample, SectionLength, WindowLength,
                                            FrequencyWeighting, ReportList, slopeType, CosinePower, EqualPower)

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
            ''' <param name="Channel"></param>
            ''' <param name="ThresholdLevel"></param>
            ''' <param name="StartSample"></param>
            ''' <param name="SectionLength">The length of the measurement windows. Must be an even integer.</param>
            ''' <param name="WindowLength"></param>
            ''' <param name="ReportList">A list of string that can be used to log limiter data.</param>
            Private Sub SoftLimitChannelSection(ByRef InputSound As Sound,
                                            ByVal Channel As Integer,
                                            ByVal ThresholdLevel As Double,
                                            ByVal StartSample As Integer,
                                            ByVal SectionLength As Integer?,
                                            ByVal WindowLength As Integer,
                                            ByVal FrequencyWeighting As FrequencyWeightings,
                                            Optional ByRef ReportList As List(Of String) = Nothing,
                                            Optional ByVal SlopeType As FadeSlopeType = FadeSlopeType.Linear,
                                            Optional ByVal CosinePower As Double = 10,
                                            Optional ByVal EqualPower As Boolean = False)

                Try

                    'TODO: This function ignores the last window, if not full!

                    CheckAndCorrectSectionLength(InputSound.WaveData.SampleData(Channel).Length, StartSample, SectionLength)

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
                        Fade(InputSound, StartAttenuation, EndAttenuation, Channel, StartSample + SectionIndex * WindowLength, WindowLength, SlopeType, CosinePower, EqualPower)

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
                               Optional ByVal SectionLength As Integer? = Nothing)

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
                               Optional ByVal SectionLength As Integer? = Nothing)

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
            ''' Directly concatenates Sound2 to the start of Sound1. The same samplerate, bit depth, sample encoding and number of channels are required in Sound1 and Sound2. Within each each input sound, equal channel lengths are also required.
            ''' </summary>
            ''' <param name="Sound1"></param>
            ''' <param name="Sound2"></param>
            Public Sub AddSoundToStart(ByRef Sound1 As Sound, ByRef Sound2 As Sound)

                Try

                    'Checking format equality
                    If Sound1.WaveFormat.IsEqual(Sound2.WaveFormat) = False Then
                        MsgBox("The function AddSoundToEnd requires the same wave sound format between Sound1 and Sound2.")
                    End If

                    'NB. As changing only the CurrentChannel channel would create channels of differing in lengths in multi channel sounds!!! Avoiding this by also extending the other channels if there are any.
                    For c = 1 To Sound1.WaveFormat.Channels
                        Sound1.WaveData.SampleData(c) = Sound2.WaveData.SampleData(c).Concat(Sound1.WaveData.SampleData(c)).ToArray
                    Next

                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try

            End Sub


            ''' <summary>
            ''' Directly concatenates Sound2 to the end of Sound1. The same samplerate, bit depth, sample encoding and number of channels are required in Sound1 and Sound2. Within each each input sound, equal channel lengths are also required.
            ''' </summary>
            ''' <param name="Sound1"></param>
            ''' <param name="Sound2"></param>
            Public Sub AddSoundToEnd(ByRef Sound1 As Sound, ByRef Sound2 As Sound)

                Try

                    'Checking format equality
                    If Sound1.WaveFormat.IsEqual(Sound2.WaveFormat) = False Then
                        MsgBox("The function AddSoundToEnd requires the same wave sound format between Sound1 and Sound2.")
                    End If

                    'NB. As changing only the CurrentChannel channel would create channels of differing in lengths in multi channel sounds!!! Avoiding this by also extending the other channels if there are any.
                    For c = 1 To Sound1.WaveFormat.Channels
                        Sound1.WaveData.SampleData(c) = Sound1.WaveData.SampleData(c).Concat(Sound2.WaveData.SampleData(c)).ToArray
                    Next

                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try

            End Sub

            ''' <summary>
            ''' Inserts Sound2 in Sound1 at the InsertionStart sample position. The same samplerate, bit depth, sample encoding and number of channels are required in Sound1 and Sound2. Within each input sound, equal channel lengths are also required.
            ''' </summary>
            ''' <param name="Sound1"></param>
            ''' <param name="Sound2"></param>
            Public Sub InsertSoundAt(ByRef Sound1 As Sound, ByRef Sound2 As Sound, ByVal InsertionStart As Integer)

                Try
                    'Checking format equality
                    If Sound1.WaveFormat.IsEqual(Sound1.WaveFormat) = False Then
                        MsgBox("The function AddSoundToEnd requires the same wave sound format between Sound1 and Sound2.")
                        Exit Sub
                    End If

                    'NB. As changing only the CurrentChannel channel would create channels of differing in lengths in multi channel sounds!!! Avoiding this by also extending the other channels if there are any.
                    For c = 1 To Sound1.WaveFormat.Channels

                        Dim ChannelData = Sound1.WaveData.SampleData(c)
                        Dim ChannelDataLength = ChannelData.Length

                        'Limits the InsertionStart to the range of Sound1
                        InsertionStart = Math.Max(0, InsertionStart)
                        InsertionStart = Math.Min(ChannelDataLength, InsertionStart)

                        'Splits the first file array
                        Dim FirstBit = Sound1.WaveData.SampleData(c).Take(InsertionStart).ToArray
                        Dim SecondBit = Sound1.WaveData.SampleData(c).ToList.GetRange(InsertionStart, ChannelDataLength - InsertionStart).ToArray

                        Dim NewChannelArray = FirstBit.Concat(Sound2.WaveData.SampleData(c)).Concat(SecondBit).ToArray

                        Sound1.WaveData.SampleData(c) = NewChannelArray
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
                               Optional ByVal SectionLength As Integer? = Nothing)

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

            Public Sub InsertSound(ByRef SoundToInsert As Sound, ByVal SourceChannel As Integer, ByRef TargetSound As Sound, ByVal TargetChannel As Integer, ByVal StartInsertSample As Integer)

                Try

                    'This check allows different channel counts
                    If SoundToInsert.WaveFormat.IsEqual(TargetSound.WaveFormat, False) = False Then Throw New ArgumentException("Different formats in InsertSound sounds. Aborting!")

                    Dim SourceChannelArray = SoundToInsert.WaveData.SampleData(SourceChannel)
                    Dim TargetChannelArray = TargetSound.WaveData.SampleData(TargetChannel)
                    Dim CopyLength As Integer = SourceChannelArray.Length

                    If OstfBase.UseOptimizationLibraries = True Then

                        'Placing the sound to insert in an array with the same length as TargetSound (as same length is required by AddTwoFloatArrays below)
                        Dim ExtendedSourceArray(TargetChannelArray.Length - 1) As Single
                        Array.Copy(SourceChannelArray, 0, ExtendedSourceArray, StartInsertSample, CopyLength)

                        'Dim TestSound As New Audio.Sound(New Formats.WaveFormat(TargetSound.WaveFormat.SampleRate, TargetSound.WaveFormat.BitDepth, 1,, TargetSound.WaveFormat.Encoding))
                        'TestSound.WaveData.SampleData(1) = ExtendedSourceArray
                        'TestSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "AdaptiveSipSounds", "TestSound_Source"))

                        'Dim TestSound2 As New Audio.Sound(New Formats.WaveFormat(TargetSound.WaveFormat.SampleRate, TargetSound.WaveFormat.BitDepth, 1,, TargetSound.WaveFormat.Encoding))
                        'TestSound2.WaveData.SampleData(1) = TargetChannelArray
                        'TestSound2.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "AdaptiveSipSounds", "TestSound_TargetChannelArray"))

                        'Summing the arrays using AddTwoFloatArrays
                        LibOstfDsp_VB.AddTwoFloatArrays(TargetChannelArray, ExtendedSourceArray)

                        'Dim TestSound3 As New Audio.Sound(New Formats.WaveFormat(TargetSound.WaveFormat.SampleRate, TargetSound.WaveFormat.BitDepth, 1,, TargetSound.WaveFormat.Encoding))
                        'TestSound3.WaveData.SampleData(1) = TargetChannelArray
                        'TestSound3.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "AdaptiveSipSounds", "TestSound_AfterMix"))

                    Else

                        Dim RequiredTargetLength As Integer = StartInsertSample + SourceChannelArray.Length
                        If RequiredTargetLength > TargetChannelArray.Length Then
                            CopyLength = TargetChannelArray.Length - StartInsertSample
                        End If

                        For s = 0 To CopyLength - 1
                            TargetChannelArray(StartInsertSample + s) += SourceChannelArray(s)
                        Next

                    End If

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
            Public Sub MaxAmplitudeNormalizeSection(ByRef InputSound As Sound, Optional ByVal channel As Integer? = Nothing,
                                        Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing,
                                                     Optional NormalizeChannelsSeparately As Boolean = False)

                Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, channel)

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
                        AmplifySection(InputSound, Gain, c, CorrectedStartSample, CorrectedSectionLength, SoundDataUnit.linear)

                    Else
                        If AbsoluteMaxAmplitude > AbsoluteMaxAmplitudeBothChannels Then AbsoluteMaxAmplitudeBothChannels = AbsoluteMaxAmplitude
                    End If

                Next

                If NormalizeChannelsSeparately = False Then

                    'Calculating the needed gain
                    Dim Gain As Double = InputSound.WaveFormat.PositiveFullScale / AbsoluteMaxAmplitudeBothChannels

                    'Amplifies the section
                    AmplifySection(InputSound, Gain, , startSample, sectionLength, SoundDataUnit.linear)

                End If

            End Sub


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
            Public Sub MeasureAndSetGatedSectionLevel(ByRef InputSound As Sound, Optional ByVal channel As Integer? = Nothing,
                         Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing,
                                                   Optional ByVal OutputLevel As Double = -23,
                                                       Optional ByVal GatingWindowDuration As Decimal = 0.01,
                                                 Optional ByVal GateRelativeThreshold As Double = -10,
                                                 Optional ByVal FractionForCalculatingAbsThreshold As Decimal = 0.25,
                                                 Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z)

                Try

                    Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, channel)

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
                        End If

                        'Adjusting section level
                        Dim Gain As Double = OutputLevel - GatedLevel

                        AmplifySection(InputSound, Gain, c, CorrectedStartSample, CorrectedSectionLength, SoundDataUnit.dB)

                    Next

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub


            ''' <summary>
            ''' Normalizes the average level of the loudest (TemporatIntegrationDuration long) period of the specified section of the input file to the specified output level.
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="channel"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength"></param>
            ''' <param name="OutputLevel">The desired normalised output level.</param>
            Public Sub TimeAndFrequencyWeightedNormalization(ByRef InputSound As Sound, Optional ByVal channel As Integer? = Nothing,
                         Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing,
                                                   Optional ByVal OutputLevel As Double = -23)

                Try

                    Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, channel)

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
                        End If

                        'Adjusting section level
                        Dim Gain As Double = OutputLevel - MaxLevel

                        AmplifySection(InputSound, Gain, c, CorrectedStartSample, CorrectedSectionLength, SoundDataUnit.dB)

                    Next

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub



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
                            Dim localREXArray(fftFormat.FftWindowSize - 1) As Double
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
                            Dim localIMXArray(fftFormat.FftWindowSize - 1) As Double

                            'Caluculating FFT
                            FastFourierTransform(FftDirections.Forward, localREXArray, localIMXArray)

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

                            'Caluculating iFFT
                            FastFourierTransform(FftDirections.Backward, localREXWindow.WindowData, localIMXWindow.WindowData)

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

                Throw New NotImplementedException("This method is not completed...")

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


            Public Enum FftDirections
                Forward
                Backward
            End Enum

            ''' <summary>
            ''' Calculates the fast fourier transform using the complex Radix-2 FFT algorithm
            ''' </summary>
            ''' <param name="Direction">Transform direction</param>
            ''' <param name="x">Real data array</param>
            ''' <param name="y">Imaginary data array</param>
            ''' <param name="ScaleForwardTransform"></param>
            ''' <param name="Reorder">Set to false to skip sample reordering</param>
            Public Sub FastFourierTransform(ByVal Direction As FftDirections, ByRef x() As Double, ByRef y() As Double,
                                   Optional ScaleForwardTransform As Boolean = True, Optional ByVal Reorder As Boolean = True)


                If OstfBase.UseOptimizationLibraries = False Then
                    FftRadix2(x, y, Direction, ScaleForwardTransform, Reorder)

                Else

                    LibOstfDsp_VB.Fft_complex(x, y, x.Length, Direction, Reorder, ScaleForwardTransform)

                End If

            End Sub


            ''' <summary>
            ''' Complex Radix-2 FFT
            ''' </summary>
            ''' <param name="x">Real data array</param>
            ''' <param name="y">Imaginary data array</param>
            ''' <param name="Direction">Transform direction</param>
            ''' <param name="ScaleForwardTransform"></param>
            ''' <param name="Reorder">Set to false to skip sample reordering</param>
            Public Sub FftRadix2(ByRef x() As Double, ByRef y() As Double, ByRef Direction As FftDirections, Optional ByVal ScaleForwardTransform As Boolean = True, Optional ByVal Reorder As Boolean = True)

                ' This is a modified VB translation of the MIT licensed code in Mathnet Numerics, See https://github.com/mathnet/mathnet-numerics/blob/306fb068d73f3c3d0e90f6f644b55cddfdeb9a0c/src/Numerics/Providers/FourierTransform/ManagedFourierTransformProvider.Radix2.cs

                Dim ExponentSign As Integer
                If Direction = FftDirections.Forward Then
                    ExponentSign = -1
                ElseIf Direction = FftDirections.Backward Then
                    ExponentSign = 1
                Else
                    Throw New ArgumentException("Unknown FFT direction")
                End If

                If Reorder = True Then

                    Dim TempX As Double
                    Dim TempY As Double

                    Dim j As Integer = 0
                    For i = 0 To x.Length - 2

                        If i < j Then
                            TempX = x(i)
                            x(i) = x(j)
                            x(j) = TempX

                            TempY = y(i)
                            y(i) = y(j)
                            y(j) = TempY

                        End If

                        Dim m As Integer = x.Length

                        Do
                            m >>= 1
                            j = j Xor m
                        Loop While (j And m) = 0

                    Next

                End If

                'Defining some temporary variables to avoid definition inside loop
                Dim aiX As Double
                Dim aiY As Double
                Dim Real1 As Double
                Dim Imaginary1 As Double
                Dim Real2 As Double
                Dim Imaginary2 As Double
                Dim TempReal1 As Double

                Dim LevelSize As Integer = 1
                While LevelSize < x.Length

                    Dim StepSize = LevelSize << 1

                    For k = 0 To LevelSize - 1

                        Dim exponent = (ExponentSign * k) * Math.PI / LevelSize
                        Dim wX As Double = Math.Cos(exponent) ' N.B. this step of the algorithm suffers from the inexact floating point numbers returned from the trigonometric functions Cos and Sin
                        Dim wY As Double = Math.Sin(exponent)

                        Dim i As Integer = k
                        While i < x.Length - 1

                            aiX = x(i)
                            aiY = y(i)

                            Real1 = wX
                            Imaginary1 = wY
                            Real2 = x(i + LevelSize)
                            Imaginary2 = y(i + LevelSize)

                            'Complex multiplication
                            TempReal1 = Real1
                            Real1 = TempReal1 * Real2 - Imaginary1 * Imaginary2
                            Imaginary1 = TempReal1 * Imaginary2 + Imaginary1 * Real2

                            x(i) = aiX + Real1
                            y(i) = aiY + Imaginary1

                            x(i + LevelSize) = aiX - Real1
                            y(i + LevelSize) = aiY - Imaginary1

                            i += StepSize

                        End While

                    Next

                    LevelSize *= 2

                End While

                'Scaling
                If Direction = FftDirections.Forward And ScaleForwardTransform = True Then
                    Dim scalingFactor = 1.0 / x.Length
                    For i = 0 To x.Length - 1
                        x(i) *= scalingFactor
                        y(i) *= scalingFactor
                    Next
                End If

            End Sub

            ''' <summary>
            ''' Complex Radix-2 FFT
            ''' </summary>
            ''' <param name="x">Real data array</param>
            ''' <param name="y">Imaginary data array</param>
            ''' <param name="Direction">Transform direction</param>
            ''' <param name="ScaleForwardTransform"></param>
            ''' <param name="Reorder">Set to false to skip sample reordering</param>
            Public Sub FftRadix2_TrigDict(ByRef x() As Double, ByRef y() As Double, ByRef Direction As FftDirections, Optional ByVal ScaleForwardTransform As Boolean = True, Optional ByVal Reorder As Boolean = True)

                ' This is a modified VB translation of the MIT licensed code in Mathnet Numerics, See https://github.com/mathnet/mathnet-numerics/blob/306fb068d73f3c3d0e90f6f644b55cddfdeb9a0c/src/Numerics/Providers/FourierTransform/ManagedFourierTransformProvider.Radix2.cs

                Dim TrigDict = GetRadix2TrigonomerticValues(x.Length, Direction)

                Dim ExponentSign As Integer
                If Direction = FftDirections.Forward Then
                    ExponentSign = -1
                ElseIf Direction = FftDirections.Backward Then
                    ExponentSign = 1
                Else
                    Throw New ArgumentException("Unknown FFT direction")
                End If

                If Reorder = True Then

                    Dim TempX As Double
                    Dim TempY As Double

                    Dim j As Integer = 0
                    For i = 0 To x.Length - 2

                        If i < j Then
                            TempX = x(i)
                            x(i) = x(j)
                            x(j) = TempX

                            TempY = y(i)
                            y(i) = y(j)
                            y(j) = TempY

                        End If

                        Dim m As Integer = x.Length

                        Do
                            m >>= 1
                            j = j Xor m
                        Loop While (j And m) = 0

                    Next

                End If

                'Defining some temporary variables to avoid definition inside loop
                Dim aiX As Double
                Dim aiY As Double
                Dim Real1 As Double
                Dim Imaginary1 As Double
                Dim Real2 As Double
                Dim Imaginary2 As Double
                Dim TempReal1 As Double

                Dim LevelSize As Integer = 1
                While LevelSize < x.Length

                    Dim StepSize = LevelSize << 1

                    For k = 0 To LevelSize - 1

                        Dim exponent = (ExponentSign * k) * Math.PI / LevelSize
                        Dim TrigTuple = DirectCast(TrigDict(exponent), Tuple(Of Double, Double))
                        Dim wX As Double = TrigTuple.Item1 ' N.B. this step of the algorithm suffers from the inexact floating point numbers returned from the trigonometric functions Cos and Sin
                        Dim wY As Double = TrigTuple.Item2

                        Dim i As Integer = k
                        While i < x.Length - 1

                            aiX = x(i)
                            aiY = y(i)

                            Real1 = wX
                            Imaginary1 = wY
                            Real2 = x(i + LevelSize)
                            Imaginary2 = y(i + LevelSize)

                            'Complex multiplication
                            TempReal1 = Real1
                            Real1 = TempReal1 * Real2 - Imaginary1 * Imaginary2
                            Imaginary1 = TempReal1 * Imaginary2 + Imaginary1 * Real2

                            x(i) = aiX + Real1
                            y(i) = aiY + Imaginary1

                            x(i + LevelSize) = aiX - Real1
                            y(i + LevelSize) = aiY - Imaginary1

                            i += StepSize

                        End While

                    Next

                    LevelSize *= 2

                End While

                'Scaling
                If Direction = FftDirections.Forward And ScaleForwardTransform = True Then
                    Dim scalingFactor = 1.0 / x.Length
                    For i = 0 To x.Length - 1
                        x(i) *= scalingFactor
                        y(i) *= scalingFactor
                    Next
                End If

            End Sub



            ''' <summary>
            ''' Complex Radix-2 FFT- Should be slower than FftRadix since it calls ComplexMultiplication, but a bit clearer in its implementation.
            ''' </summary>
            ''' <param name="x">Real data array</param>
            ''' <param name="y">Imaginary data array</param>
            ''' <param name="Direction">Transform direction</param>
            ''' <param name="ScaleForwardTransform"></param>
            ''' <param name="Reorder">Set to false to skip sample reordering</param>
            Public Sub FftRadix2_B(ByRef x() As Double, ByRef y() As Double, ByRef Direction As FftDirections, Optional ByVal ScaleForwardTransform As Boolean = True, Optional ByVal Reorder As Boolean = True)

                ' This is a modified VB translation of the MIT licensed code in Mathnet Numerics, See https://github.com/mathnet/mathnet-numerics/blob/306fb068d73f3c3d0e90f6f644b55cddfdeb9a0c/src/Numerics/Providers/FourierTransform/ManagedFourierTransformProvider.Radix2.cs

                Dim ExponentSign As Integer
                If Direction = FftDirections.Forward Then
                    ExponentSign = -1
                ElseIf Direction = FftDirections.Backward Then
                    ExponentSign = 1
                Else
                    Throw New ArgumentException("Unknown FFT direction")
                End If

                If Reorder = True Then

                    Dim TempX As Double
                    Dim TempY As Double

                    Dim j As Integer = 0
                    For i = 0 To x.Length - 2

                        If i < j Then
                            TempX = x(i)
                            x(i) = x(j)
                            x(j) = TempX

                            TempY = y(i)
                            y(i) = y(j)
                            y(j) = TempY

                        End If

                        Dim m As Integer = x.Length

                        Do
                            m >>= 1
                            j = j Xor m
                        Loop While (j And m) = 0

                    Next

                End If

                Dim LevelSize As Integer = 1
                While LevelSize < x.Length

                    Dim StepSize = LevelSize << 1

                    For k = 0 To LevelSize - 1

                        Dim exponent = (ExponentSign * k) * Math.PI / LevelSize
                        Dim wX As Double = Math.Cos(exponent)
                        Dim wY As Double = Math.Sin(exponent)

                        Dim i As Integer = k
                        While i < x.Length - 1

                            Dim aiX = x(i)
                            Dim aiY = y(i)

                            Dim t = ComplexMultiplication(wX, wY, x(i + LevelSize), y(i + LevelSize))

                            x(i) = aiX + t.Item1
                            y(i) = aiY + t.Item2

                            x(i + LevelSize) = aiX - t.Item1
                            y(i + LevelSize) = aiY - t.Item2

                            i += StepSize

                        End While

                    Next

                    LevelSize *= 2

                End While

                'Scaling
                If Direction = FftDirections.Forward And ScaleForwardTransform = True Then
                    Dim scalingFactor = 1.0 / x.Length
                    For i = 0 To x.Length - 1
                        x(i) *= scalingFactor
                        y(i) *= scalingFactor
                    Next
                End If

            End Sub

            Public Function ComplexMultiplication(ByVal Real1 As Double, ByVal Imaginary1 As Double, ByVal Real2 As Double, ByVal Imaginary2 As Double) As Tuple(Of Double, Double)

                Dim TempReal1 = Real1
                Real1 = TempReal1 * Real2 - Imaginary1 * Imaginary2
                Imaginary1 = TempReal1 * Imaginary2 + Imaginary1 * Real2

                Return New Tuple(Of Double, Double)(Real1, Imaginary1)

            End Function


            ''' <summary>
            ''' Complex Radix-2 FFT, performed by copying data to the System.Numerics.Complex type.
            ''' </summary>
            ''' <param name="x">Real data array</param>
            ''' <param name="y">Imaginary data array</param>
            ''' <param name="Direction">Transform direction</param>
            ''' <param name="ScaleForwardTransform"></param>
            ''' <param name="Reorder">Set to false to skip sample reordering</param>
            Public Sub FftRadix2_CT(ByRef x() As Double, ByRef y() As Double, ByRef Direction As FftDirections, Optional ByVal ScaleForwardTransform As Boolean = True, Optional ByVal Reorder As Boolean = True)

                'Copying data to complex type
                Dim ComplexData(x.Length - 1) As System.Numerics.Complex
                For i = 0 To x.Length - 1
                    ComplexData(i) = New Numerics.Complex(x(i), y(i))
                Next

                'Performing fft
                FftRadix2(ComplexData, Direction, ScaleForwardTransform, Reorder)

                'Copying data back
                For i = 0 To x.Length - 1
                    x(i) = ComplexData(i).Real
                    y(i) = ComplexData(i).Imaginary
                Next

            End Sub

            ''' <summary>
            ''' Complex Radix-2 FFT, performed directly on a referenced System.Numerics.Complex type array.
            ''' </summary>
            ''' <param name="data">The complex input array</param>
            ''' <param name="Direction">Transform direction</param>
            ''' <param name="ScaleForwardTransform"></param>
            ''' <param name="Reorder">Set to false to skip sample reordering</param>
            Public Sub FftRadix2(ByRef Data() As Numerics.Complex, ByRef Direction As FftDirections, Optional ByVal ScaleForwardTransform As Boolean = True, Optional ByVal Reorder As Boolean = True)

                ' This is a modified VB translation of the MIT licensed code in Mathnet Numerics, See https://github.com/mathnet/mathnet-numerics/blob/306fb068d73f3c3d0e90f6f644b55cddfdeb9a0c/src/Numerics/Providers/FourierTransform/ManagedFourierTransformProvider.Radix2.cs

                Dim CurrentStep As Integer = 0

                Dim ExponentSign As Integer
                If Direction = FftDirections.Forward Then
                    ExponentSign = -1
                ElseIf Direction = FftDirections.Backward Then
                    ExponentSign = 1
                Else
                    Throw New ArgumentException("Unknown FFT direction")
                End If

                If Reorder = True Then
                    Dim TempSample As Numerics.Complex

                    Dim j As Integer = 0
                    For i = 0 To Data.Length - 2

                        If i < j Then
                            TempSample = Data(i)
                            Data(i) = Data(j)
                            Data(j) = TempSample
                        End If

                        Dim m As Integer = Data.Length

                        Do
                            m >>= 1
                            j = j Xor m
                        Loop While (j And m) = 0

                    Next
                End If

                Dim levelSize As Integer = 1
                While levelSize < Data.Length

                    Dim stepSize = levelSize << 1

                    For k = 0 To levelSize - 1

                        Dim exponent = (ExponentSign * k) * Math.PI / levelSize
                        Dim w = New System.Numerics.Complex(Math.Cos(exponent), Math.Sin(exponent))

                        Dim i As Integer = k
                        While i < Data.Length - 1

                            Dim ai = Data(i)
                            Dim t = w * Data(i + levelSize)
                            Data(i) = ai + t
                            Data(i + levelSize) = ai - t

                            CurrentStep += 1

                            i += stepSize

                        End While

                    Next

                    levelSize *= 2

                End While

                'Scaling
                If Direction = FftDirections.Forward And ScaleForwardTransform = True Then
                    Dim scalingFactor = 1.0 / Data.Length
                    For i = 0 To Data.Length - 1
                        Data(i) *= scalingFactor
                        CurrentStep += 1
                    Next
                End If

            End Sub

            'Private Sub Radix2Reorder(ByRef samples() As Numerics.Complex)

            '    ' This is a VB translation of the MIT licensed code in Mathnet Numerics, See https://github.com/mathnet/mathnet-numerics/blob/306fb068d73f3c3d0e90f6f644b55cddfdeb9a0c/src/Numerics/Providers/FourierTransform/ManagedFourierTransformProvider.Radix2.cs

            '    Dim TempSample As Numerics.Complex

            '    Dim j = 0
            '    For i = 0 To samples.Length - 2

            '        If i < j Then

            '            TempSample = samples(i)
            '            samples(i) = samples(j)
            '            samples(j) = TempSample
            '        End If

            '        Dim m As Integer = samples.Length

            '        Do
            '            m >>= 1
            '            j = j Xor m
            '        Loop While (j And m) = 0

            '    Next

            'End Sub

            'Private Sub Radix2Step(ByRef samples As System.Numerics.Complex(), ByVal exponentSign As Integer, ByVal levelSize As Integer, ByVal k As Integer)

            '    ' This is a VB translation of the MIT licensed code in Mathnet Numerics, See https://github.com/mathnet/mathnet-numerics/blob/306fb068d73f3c3d0e90f6f644b55cddfdeb9a0c/src/Numerics/Providers/FourierTransform/ManagedFourierTransformProvider.Radix2.cs

            '    Dim exponent = (exponentSign * k) * Math.PI / levelSize
            '    Dim w = New System.Numerics.Complex(Math.Cos(exponent), Math.Sin(exponent))

            '    Dim stepSize = levelSize << 1

            '    Dim i As Integer = k
            '    While i < samples.Length - 1

            '        Dim ai = samples(i)
            '        Dim t = w * samples(i + levelSize)
            '        samples(i) = ai + t
            '        samples(i + levelSize) = ai - t

            '        i += stepSize

            '    End While

            'End Sub


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

                    'Calculate Average power spectrum of all time windows and store the result in the first time window (of each channel)
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
                Dim TargetSpectrum() As Double = TargetSound.FFT.AmplitudeSpectrum(1, 0).WindowData

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

                If filteringDomain = ProcessingDomain.TimeDomain Then
                    Throw New ArgumentException("The filteringDomain argument value ProcessingDomain.TimeDomain is no longer supported. Use the function FIRFilterTimeDomain instead.")
                End If

                'Frequency domain convolution (complex multiplication)

                'Reference: This frequency domain calculation is based on the overlap-add method as
                'described in Bateman, A. & Paterson-Stephens, I. (2002). The DSP Handbook. Algorithms, Applications and Design Techniques.
                'chapter 7, pp 451-453.

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
                                Dim TempArray = impulseResponse.WaveData.SampleData(IRChannel)
                                Dim IRArray(TempArray.Length - 1) As Double
                                Utils.CopyToDouble(TempArray, IRArray)

                                'Scaling impulse response. 
                                If ScaleImpulseResponse = True Then

                                    Dim ImpulseResponseArraySum As Double = IRArray.Sum

                                    If ImpulseResponseArraySum <> 0 Then
                                        Utils.MultiplyArray(IRArray, (1 / ImpulseResponseArraySum))

                                        'For n = 0 To IRArray.Length - 1
                                        '    IRArray(n) = IRArray(n) / ImpulseResponseArraySum
                                        'Next
                                    Else
                                        MsgBox("The impulse response sums to 0!")
                                    End If
                                End If

                                'Referencing the current channel array, and noting its original length
                                Dim OriginalInputChannelLength As Integer = inputSound.WaveData.SampleData(c).Length
                                Dim IrArrayLength As Integer = IRArray.Length
                                Dim IntendedOutputLength As Integer = OriginalInputChannelLength + IrArrayLength

                                CheckAndAdjustFFTSize(fftFormat.FftWindowSize, (IrArrayLength * 2) + 1, InActivateWarnings)

                                Dim AddOverlapSliceLength As Integer = fftFormat.FftWindowSize / 2
                                Dim NumberOfTimeWindows As Integer = Int(IntendedOutputLength / AddOverlapSliceLength)

                                'Copies the input array to a new array of Double, if needed, also extends the input array to a whole number multiple of the length of the sound data that goes into each dft
                                If IntendedOutputLength Mod AddOverlapSliceLength > 0 Then NumberOfTimeWindows += 1
                                Dim IntputChannelSoundLength As Integer = AddOverlapSliceLength * (NumberOfTimeWindows + 1)
                                Dim InputSoundChannelArrayDouble(IntputChannelSoundLength - 1) As Double
                                Utils.CopyToDouble(inputSound.WaveData.SampleData(c), InputSoundChannelArrayDouble)

                                'Creates an array in which to store the convoluted sound
                                Dim OutputChannelSampleArray(IntputChannelSoundLength - 1) As Single

                                'Creates dft bins for the IR data
                                Dim dftIR_Bin_x(fftFormat.FftWindowSize - 1) As Double
                                Dim dftIR_Bin_y(fftFormat.FftWindowSize - 1) As Double

                                'Copied the IR data into the IR bins
                                Array.Copy(IRArray, dftIR_Bin_x, IrArrayLength)

                                'Calculates forward FFT for the IR 
                                FastFourierTransform(FftDirections.Forward, dftIR_Bin_x, dftIR_Bin_y, True)

                                'Starts convolution one window at a time
                                Dim readSample As Integer = 0
                                Dim writeSample As Integer = 0

                                For windowNumber = 0 To NumberOfTimeWindows - 1 'Step 2

                                    'Creates a zero-padded sound array with the length of the dft windows size ()

                                    'Creates a new bins for the input sound data
                                    Dim dftSoundBin_x(fftFormat.FftWindowSize - 1) As Double
                                    Dim dftSoundBin_y(fftFormat.FftWindowSize - 1) As Double

                                    'Copies the slice samples into the dft x-bin, and increases readSample by AddOverlapSliceLength
                                    Array.Copy(InputSoundChannelArrayDouble, readSample, dftSoundBin_x, 0, AddOverlapSliceLength)
                                    readSample += AddOverlapSliceLength

                                    'Calculates forward FFT for the current sound window (Skipping the forward transform scaling on x instead of h, for optimization (the results of the complex multiplication should be the same))
                                    FastFourierTransform(FftDirections.Forward, dftSoundBin_x, dftSoundBin_y, False)

                                    'Performs complex multiplications
                                    Utils.Math.ComplexMultiplication(dftSoundBin_x, dftSoundBin_y, dftIR_Bin_x, dftIR_Bin_y)

                                    'Dim tempDftSoundBin_x As Double = 0
                                    'For n = 0 To fftFormat.FftWindowSize - 1
                                    '    tempDftSoundBin_x = dftSoundBin_x(n) 'stores this value so that it does not get overwritten in the following line (it needs to be used also two lines below)
                                    '    dftSoundBin_x(n) = tempDftSoundBin_x * dftIR_Bin_x(n) - dftSoundBin_y(n) * dftIR_Bin_y(n)
                                    '    dftSoundBin_y(n) = tempDftSoundBin_x * dftIR_Bin_y(n) + dftSoundBin_y(n) * dftIR_Bin_x(n)
                                    'Next

                                    'Calculates inverse FFT
                                    FastFourierTransform(FftDirections.Backward, dftSoundBin_x, dftSoundBin_y)

                                    'Puts the convoluted sound in the output array
                                    For sample = 0 To fftFormat.FftWindowSize - 1
                                        OutputChannelSampleArray(writeSample) += dftSoundBin_x(sample)
                                        writeSample += 1
                                    Next
                                    writeSample -= AddOverlapSliceLength

                                    'Referencin the sound array in the output sound
                                    outputSound.WaveData.SampleData(c) = OutputChannelSampleArray

                                Next

                                If KeepInputSoundLength = True Then

                                            'Correcting the channel length, by copying the section needed
                                            Dim ItitialTrimLength As Integer = impulseResponse.WaveData.SampleData(IRChannel).Length / 2
                                            Dim NewChannelArray(OriginalInputChannelLength - 1) As Single
                                            'For s = 0 To NewChannelArray.Length - 1
                                            '    NewChannelArray(s) = OutputChannelSampleArray(s + ItitialTrimLength)
                                            'Next
                                            Array.Copy(OutputChannelSampleArray, ItitialTrimLength, NewChannelArray, 0, NewChannelArray.Length)

                                            outputSound.WaveData.SampleData(c) = NewChannelArray

                                            'Fading the beginning and the end of the channel array
                                            DSP.Fade(outputSound, Nothing, 0, c, 0, Math.Min(NewChannelArray.Length / 10, 100))
                                            DSP.Fade(outputSound, 0, Nothing, c, NewChannelArray.Length - Math.Min(NewChannelArray.Length / 10, 100))

                                        Else

                                            Dim NewChannelArray(IntendedOutputLength - 1) As Single
                                            Array.Copy(outputSound.WaveData.SampleData(c), 0, NewChannelArray, 0, NewChannelArray.Length)
                                            outputSound.WaveData.SampleData(c) = NewChannelArray

                                            'ReDim Preserve outputSound.WaveData.SampleData(c)(IntendedOutputLength - 1)

                                        End If

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

            Public Function FIRFilterTimeDomain(ByVal inputSound As Sound, ByVal impulseResponse As Sound,
                                  Optional ByVal inputSoundChannel As Integer? = Nothing,
                                  Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing,
                                  Optional ByVal ScaleImpulseResponse As Boolean = False, Optional ByVal KeepInputSoundLength As Boolean = False) As Sound

                Try

                    Throw New NotImplementedException("The time domain FIR filtering has not been checked since modified on 2024-04-01")

                    If KeepInputSoundLength = True Then Throw New NotImplementedException("Time domain FIR filtering does not yet support keeping input sound length.")
                    If startSample > 0 Then Throw New NotImplementedException("Time domain FIR filtering does not yet support setting start sample other than 0.")
                    If sectionLength IsNot Nothing Then Throw New NotImplementedException("Time domain FIR filtering does not yet support setting section Length.")

                    Dim IRChannel As Integer = 1

                    Dim outputSound As New Sound(inputSound.WaveFormat)
                    Dim AudioOutputConstructor As New AudioOutputConstructor(inputSound.WaveFormat, inputSoundChannel)

                    'Main section
                    Select Case inputSound.WaveFormat.BitDepth
                        Case 16, 32
                            For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                                'Copies the impulse response to a new array of double
                                Dim TempArray = impulseResponse.WaveData.SampleData(IRChannel)
                                Dim IRArray(TempArray.Length - 1) As Double
                                Utils.CopyToDouble(TempArray, IRArray)

                                'Scaling impulse response. 
                                If ScaleImpulseResponse = True Then
                                    Dim ImpulseResponseArraySum As Double = IRArray.Sum
                                    If ImpulseResponseArraySum <> 0 Then
                                        Utils.MultiplyArray(IRArray, (1 / ImpulseResponseArraySum))
                                    Else
                                        MsgBox("The impulse response sums to 0!")
                                    End If
                                End If

                                'Referencing the current channel array, and noting its original length
                                Dim OriginalInputChannelLength As Integer = inputSound.WaveData.SampleData(c).Length
                                Dim IrArrayLength As Integer = IRArray.Length
                                Dim IntendedOutputLength As Integer = OriginalInputChannelLength + IrArrayLength

                                Dim InputSoundChannelArrayDouble(IntendedOutputLength - 1) As Single
                                Array.Copy(inputSound.WaveData.SampleData(c), 0, InputSoundChannelArrayDouble, 0, OriginalInputChannelLength)

                                'Time domain convolution filtering

                                'Starting a progress window
                                'Dim myProgress As New ProgressDisplay
                                'myProgress.Initialize(inputArray.Length - 1, 0, "Calculating FIR filter...", IRArray.Length)
                                'myProgress.Show()
                                'Dim ProgressCounter As Integer = 0

                                For n = 0 To InputSoundChannelArrayDouble.Length - 1
                                    Dim cumulativeValue As Double = 0

                                    'Updating progress
                                    'myProgress.UpdateProgress(ProgressCounter)
                                    'ProgressCounter += 1

                                    'Doing the convolution
                                    For i = 0 To IrArrayLength - 1
                                        If n - i >= 0 Then
                                            cumulativeValue += InputSoundChannelArrayDouble(n - i) * IRArray(i)
                                        End If
                                    Next
                                    InputSoundChannelArrayDouble(n) = cumulativeValue

                                Next

                                outputSound.WaveData.SampleData(c) = InputSoundChannelArrayDouble

                                'Closing the progress display
                                'myProgress.Close()

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

            Public Function Deconvolution(ByVal InputSound As Sound, ByVal ImpulseResponse As Sound,
                                 Optional ByRef FftFormat As Formats.FftFormat = Nothing, Optional ByVal InputSoundChannel As Integer? = Nothing,
                                      Optional ByVal LowerCutoffFrequency As Double? = Nothing, Optional ByVal UpperCutoffFrequency As Double? = Nothing,
                                      Optional InActivateWarnings As Boolean = False) As Sound

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
                        Dim InputSoundChannelArray() As Single = InputSound.WaveData.SampleData(c)
                        Dim IrChannelArray() As Single = ImpulseResponse.WaveData.SampleData(ImpulseResponseChannel)

                        Dim OriginalChannelLength As Integer = InputSoundChannelArray.Length
                        CheckAndAdjustFFTSize(FftFormat.FftWindowSize, Math.Max(InputSoundChannelArray.Length, IrChannelArray.Length), InActivateWarnings)

                        'Creats dft bins for the IR
                        Dim DftIR_x_Bin(FftFormat.FftWindowSize - 1) As Double
                        Dim DftIR_y_Bin(FftFormat.FftWindowSize - 1) As Double

                        'Copies the IR  samples into DftIR_x_Bin
                        Dim IrReadSample As Integer = 0
                        For sample = 0 To IrChannelArray.Length - 1
                            DftIR_x_Bin(sample) = IrChannelArray(IrReadSample)
                            IrReadSample += 1
                        Next


                        'Calculates forward FFT for the IR
                        FastFourierTransform(FftDirections.Forward, DftIR_x_Bin, DftIR_y_Bin, False)


                        'Creats dft bins for the input sound
                        Dim DftInputSound_x_Bin(FftFormat.FftWindowSize - 1) As Double
                        Dim DftInputSound_y_Bin(FftFormat.FftWindowSize - 1) As Double

                        'Copies input sound data into DftInputSound_x_Bin, putting zero-padding initially
                        Dim InputSoundReadSample As Integer = 0
                        For sample = 0 To InputSoundChannelArray.Length - 1
                            DftInputSound_x_Bin(sample) = InputSoundChannelArray(sample)
                        Next

                        'Calculates forward FFT for the input sound
                        FastFourierTransform(FftDirections.Forward, DftInputSound_x_Bin, DftInputSound_y_Bin, True)

                        'Sets frequency limits
                        Dim NyquistFrequencyIndex As Integer = FftFormat.FftWindowSize / 2

                        Dim PositiveSide_LowerInclusiveCutoffIndex As Integer = 1
                        Dim PositiveSide_UpperInclusiveCutoffIndex As Integer = NyquistFrequencyIndex - 1

                        Dim NegativeSide_UpperInclusiveCutoffIndex As Integer = NyquistFrequencyIndex + 1
                        Dim NegativeSide_LowerInclusiveCutoffIndex As Integer = FftFormat.FftWindowSize - 1

                        If LowerCutoffFrequency IsNot Nothing Then

                            PositiveSide_LowerInclusiveCutoffIndex = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, LowerCutoffFrequency, InputSound.WaveFormat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.alwaysUp)
                            NegativeSide_LowerInclusiveCutoffIndex = FftFormat.FftWindowSize - PositiveSide_LowerInclusiveCutoffIndex

                        End If

                        If UpperCutoffFrequency IsNot Nothing Then

                            PositiveSide_UpperInclusiveCutoffIndex = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, UpperCutoffFrequency, InputSound.WaveFormat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.alwaysDown)
                            NegativeSide_UpperInclusiveCutoffIndex = FftFormat.FftWindowSize - PositiveSide_UpperInclusiveCutoffIndex

                        End If

                        'Performs complex division
                        For n = PositiveSide_LowerInclusiveCutoffIndex To PositiveSide_UpperInclusiveCutoffIndex

                            'z1=a+bi z2=c+di 
                            'z1/z2 = (ac+bd)/(c^2+d^2) + (bc-ad)/(c^2+d^2)i
                            Dim cm_a As Double = DftInputSound_x_Bin(n)
                            Dim cm_b As Double = DftInputSound_y_Bin(n)
                            Dim cm_c As Double = DftIR_x_Bin(n)
                            Dim cm_d As Double = DftIR_y_Bin(n)

                            DftInputSound_x_Bin(n) = (cm_a * cm_c + cm_b * cm_d) / (cm_c ^ 2 + cm_d ^ 2)
                            DftInputSound_y_Bin(n) = (cm_b * cm_c - cm_a * cm_d) / (cm_c ^ 2 + cm_d ^ 2)

                        Next

                        For n = NegativeSide_UpperInclusiveCutoffIndex To NegativeSide_LowerInclusiveCutoffIndex

                            'z1=a+bi z2=c+di 
                            'z1/z2 = (ac+bd)/(c^2+d^2) + (bc-ad)/(c^2+d^2)i
                            Dim cm_a As Double = DftInputSound_x_Bin(n)
                            Dim cm_b As Double = DftInputSound_y_Bin(n)
                            Dim cm_c As Double = DftIR_x_Bin(n)
                            Dim cm_d As Double = DftIR_y_Bin(n)

                            DftInputSound_x_Bin(n) = (cm_a * cm_c + cm_b * cm_d) / (cm_c ^ 2 + cm_d ^ 2)
                            DftInputSound_y_Bin(n) = (cm_b * cm_c - cm_a * cm_d) / (cm_c ^ 2 + cm_d ^ 2)

                        Next


                        'Setting the Zero frequency component to 0
                        DftInputSound_x_Bin(0) = 0
                        DftInputSound_y_Bin(0) = 0

                        ''Looking at  the amplitude and phase responses
                        'Dim TempSound = New Sound(InputSound.WaveFormat)
                        'TempSound.FFT = New FftData(InputSound.WaveFormat, FftFormat)
                        'TempSound.FFT.FrequencyDomainRealData(1, 0) = New FftData.TimeWindow With {.WindowData = DftInputSound_x_Bin, .ZeroPadding = 0, .WindowingType = WindowingType.Blackman}
                        'TempSound.FFT.FrequencyDomainImaginaryData(1, 0) = New FftData.TimeWindow With {.WindowData = DftInputSound_y_Bin, .ZeroPadding = 0, .WindowingType = WindowingType.Blackman}

                        'TempSound.FFT.CalculatePhaseSpectrum()
                        'TempSound.FFT.CalculateAmplitudeSpectrum(False, False, False)

                        'Dim Phases = TempSound.FFT.PhaseSpectrum(1, 0).WindowData
                        'Dim Amplitudes = TempSound.FFT.AmplitudeSpectrum(1, 0).WindowData

                        'Utils.SendInfoToLog(String.Join(vbCrLf, Phases), "Phases")
                        'Utils.SendInfoToLog(String.Join(vbCrLf, Amplitudes), "Amplitudes")


                        'Calculates inverse FFT
                        FastFourierTransform(FftDirections.Backward, DftInputSound_x_Bin, DftInputSound_y_Bin)

                        'Puts the convoluted sound in the output array
                        Dim OutputChannelArray(Math.Min(OriginalChannelLength, DftInputSound_x_Bin.Length) - 1) As Single
                        For s = 0 To OutputChannelArray.Length - 1
                            OutputChannelArray(s) = DftInputSound_x_Bin(s)
                        Next


                        outputSound.WaveData.SampleData(c) = OutputChannelArray

                    Next

                    Return outputSound


                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try


            End Function




            Public Function Deconvolution_OLD(ByVal InputSound As Sound, ByVal ImpulseResponse As Sound,
                                 Optional ByRef FftFormat As Formats.FftFormat = Nothing, Optional ByVal InputSoundChannel As Integer? = Nothing,
                                      Optional ByVal LowerCutoffFrequency As Double? = Nothing, Optional ByVal UpperCutoffFrequency As Double? = Nothing,
                                      Optional InActivateWarnings As Boolean = False) As Sound

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
                        Dim InputSoundChannelArray() As Single = InputSound.WaveData.SampleData(c)
                        Dim IrChannelArray() As Single = ImpulseResponse.WaveData.SampleData(ImpulseResponseChannel)

                        Dim OriginalChannelLength As Integer = InputSoundChannelArray.Length
                        CheckAndAdjustFFTSize(FftFormat.FftWindowSize, Math.Max(InputSoundChannelArray.Length, IrChannelArray.Length), InActivateWarnings)

                        'Creats dft bins for the IR
                        Dim DftIR_x_Bin(FftFormat.FftWindowSize - 1) As Double
                        Dim DftIR_y_Bin(FftFormat.FftWindowSize - 1) As Double

                        'Copies the IR  samples into DftIR_x_Bin
                        Dim IrReadSample As Integer = 0
                        For sample = 0 To IrChannelArray.Length - 1
                            DftIR_x_Bin(sample) = IrChannelArray(IrReadSample)
                            IrReadSample += 1
                        Next


                        'Calculates forward FFT for the IR
                        FastFourierTransform(FftDirections.Forward, DftIR_x_Bin, DftIR_y_Bin, False)


                        'Creats dft bins for the input sound
                        Dim DftInputSound_x_Bin(FftFormat.FftWindowSize - 1) As Double
                        Dim DftInputSound_y_Bin(FftFormat.FftWindowSize - 1) As Double

                        'Copies input sound data into DftInputSound_x_Bin, putting zero-padding initially
                        Dim InputSoundReadSample As Integer = 0
                        For sample = 0 To InputSoundChannelArray.Length - 1
                            DftInputSound_x_Bin(sample) = InputSoundChannelArray(sample)
                        Next

                        'Calculates forward FFT for the input sound
                        FastFourierTransform(FftDirections.Forward, DftInputSound_x_Bin, DftInputSound_y_Bin, True)


                        'Performs complex division

                        For n = 0 To FftFormat.FftWindowSize - 1

                            'z1=a+bi z2=c+di 
                            'z1/z2 = (ac+bd)/(c^2+d^2) + (bc-ad)/(c^2+d^2)i
                            Dim cm_a As Double = DftInputSound_x_Bin(n)
                            Dim cm_b As Double = DftInputSound_y_Bin(n)
                            Dim cm_c As Double = DftIR_x_Bin(n)
                            Dim cm_d As Double = DftIR_y_Bin(n)

                            DftInputSound_x_Bin(n) = (cm_a * cm_c + cm_b * cm_d) / (cm_c ^ 2 + cm_d ^ 2)
                            DftInputSound_y_Bin(n) = (cm_b * cm_c - cm_a * cm_d) / (cm_c ^ 2 + cm_d ^ 2)

                        Next


                        If LowerCutoffFrequency IsNot Nothing Then

                            'Copying the value of the lowest bin to all lower bins

                            Dim LowerInclusiveCutoffIndex As Integer = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, LowerCutoffFrequency, InputSound.WaveFormat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.alwaysUp)

                            LowerInclusiveCutoffIndex -= 1

                            For n = 1 To LowerInclusiveCutoffIndex
                                DftInputSound_x_Bin(n) = DftInputSound_x_Bin(LowerInclusiveCutoffIndex)
                                DftInputSound_y_Bin(n) = DftInputSound_y_Bin(LowerInclusiveCutoffIndex)
                            Next

                            For n = FftFormat.FftWindowSize - LowerInclusiveCutoffIndex To FftFormat.FftWindowSize - 1
                                DftInputSound_x_Bin(n) = DftInputSound_x_Bin(LowerInclusiveCutoffIndex)
                                DftInputSound_y_Bin(n) = DftInputSound_y_Bin(LowerInclusiveCutoffIndex)
                            Next

                        End If


                        If UpperCutoffFrequency IsNot Nothing Then

                            'Copying the value of the lowest bin to all lower bins

                            Dim UpperInclusiveCutoffIndex As Integer = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, UpperCutoffFrequency, InputSound.WaveFormat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.alwaysDown)
                            UpperInclusiveCutoffIndex += 1
                            Dim NyquistFrequencyIndex As Integer = FftFormat.FftWindowSize / 2

                            For n = UpperInclusiveCutoffIndex To NyquistFrequencyIndex - 1
                                DftInputSound_x_Bin(n) = DftInputSound_x_Bin(UpperInclusiveCutoffIndex)
                                DftInputSound_y_Bin(n) = DftInputSound_y_Bin(UpperInclusiveCutoffIndex)
                            Next

                            For n = NyquistFrequencyIndex + 1 To FftFormat.FftWindowSize - UpperInclusiveCutoffIndex
                                DftInputSound_x_Bin(n) = DftInputSound_x_Bin(UpperInclusiveCutoffIndex)
                                DftInputSound_y_Bin(n) = DftInputSound_y_Bin(UpperInclusiveCutoffIndex)
                            Next

                        End If


                        'Setting the Zero frequency component to 0
                        DftInputSound_x_Bin(0) = 0
                        DftInputSound_y_Bin(0) = 0


                        'Calculates inverse FFT
                        FastFourierTransform(FftDirections.Backward, DftInputSound_x_Bin, DftInputSound_y_Bin)

                        'Puts the convoluted sound in the output array
                        Dim OutputChannelArray(Math.Min(OriginalChannelLength, DftInputSound_x_Bin.Length) - 1) As Single
                        For s = 0 To OutputChannelArray.Length - 1
                            OutputChannelArray(s) = DftInputSound_x_Bin(s)
                        Next


                        outputSound.WaveData.SampleData(c) = OutputChannelArray

                    Next

                    Return outputSound


                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try


            End Function



            Public Function DeconvolutionOverlapAdd(ByVal inputSound As Sound, ByVal impulseResponse As Sound,
                                  ByRef fftFormat As Formats.FftFormat, Optional ByVal inputSoundChannel As Integer? = Nothing,
                                           Optional ByVal LowerCutoffFrequency As Double? = Nothing, Optional ByVal UpperCutoffFrequency As Double? = Nothing,
                                  Optional InActivateWarnings As Boolean = False) As Sound

                Throw New NotImplementedException("This function is not working yet!")

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


                                'Referencing the current channel array, and noting its original length
                                Dim tempInputSoundArray() As Single = inputSound.WaveData.SampleData(c)
                                Dim originalInputLength As Integer = tempInputSoundArray.Length

                                'Frequency domain deconvolution (complex division)

                                'Reference: This frequency domain calculation is based on the overlap-add method as
                                'described in Bateman, A. & Paterson-Stephens, I. (2002). The DSP Handbook. Algorithms, Applications and Design Techniques.
                                'chapter 7, pp 451-453.

                                CheckAndAdjustFFTSize(fftFormat.FftWindowSize, (IRArray.Length * 2), InActivateWarnings)

                                Dim sliceLength As Integer = fftFormat.FftWindowSize / 2

                                'Copies the input array to a new array, if needed, also extends the input array to a whole number multiple of the length of the sound data that goes into each dft
                                Dim numberOfWindows As Integer = Int(tempInputSoundArray.Length / sliceLength)
                                If tempInputSoundArray.Length Mod sliceLength > 0 Then numberOfWindows += 1

                                Dim inputArray(tempInputSoundArray.Length - 1) As Single
                                For n = 0 To tempInputSoundArray.Length - 1
                                    inputArray(n) = tempInputSoundArray(n)
                                Next


                                'Creates dft bins
                                Dim dftIR_Bin_x(fftFormat.FftWindowSize - 1) As Double
                                Dim dftIR_Bin_y(fftFormat.FftWindowSize - 1) As Double

                                'Creates the zero-padded IR array
                                For sample = 0 To IRArray.Length - 1
                                    dftIR_Bin_x(sample) = IRArray(sample)
                                Next

                                'Calculates forward FFT for the IR (Skipping the forward transform scaling)
                                FastFourierTransform(FftDirections.Forward, dftIR_Bin_x, dftIR_Bin_y, False)

                                'Starts convolution one window at a time
                                Dim readSample As Integer = 0
                                Dim writeSample As Integer = 0
                                Dim OutputChannelSampleArray(inputArray.Length - 1) As Single

                                For windowNumber = 0 To numberOfWindows - 2 'Step 2

                                    Dim dftSoundBin_x(fftFormat.FftWindowSize - 1) As Double
                                    Dim dftSoundBin_y(fftFormat.FftWindowSize - 1) As Double

                                    'Creates a zero-padded sound array with the length of the dft windows size ()
                                    For sample = 0 To sliceLength - 1
                                        dftSoundBin_x(sample) = inputArray(readSample)
                                        readSample += 1
                                    Next

                                    'Calculates forward FFT for the current sound window
                                    FastFourierTransform(FftDirections.Forward, dftSoundBin_x, dftSoundBin_y, True)

                                    Dim StartIndex As Integer = 0
                                    Dim EndIndex As Integer = fftFormat.FftWindowSize - 1

                                    If LowerCutoffFrequency IsNot Nothing Then
                                        StartIndex = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, LowerCutoffFrequency, inputSound.WaveFormat.SampleRate, fftFormat.FftWindowSize, Utils.roundingMethods.alwaysUp)
                                    End If

                                    If UpperCutoffFrequency IsNot Nothing Then
                                        EndIndex = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, UpperCutoffFrequency, inputSound.WaveFormat.SampleRate, fftFormat.FftWindowSize, Utils.roundingMethods.alwaysDown)
                                    End If

                                    For n = StartIndex To EndIndex

                                        ''performs complex multiplications (to instead do convolution)
                                        'Dim tempDftSoundBin_x As Double = 0
                                        'tempDftSoundBin_x = dftSoundBin_x(n) 'stores this value so that it does not get overwritten in the following line (it needs to be used also two lines below)
                                        'dftSoundBin_x(n) = tempDftSoundBin_x * dftIR_Bin_x(n) - dftSoundBin_y(n) * dftIR_Bin_y(n)
                                        'dftSoundBin_y(n) = tempDftSoundBin_x * dftIR_Bin_y(n) + dftSoundBin_y(n) * dftIR_Bin_x(n)


                                        'Performs complex division
                                        'z1=a+bi z2=c+di 
                                        'z1/z2 = (ac+bd)/(c^2+d^2) + (bc-ad)/(c^2+d^2)i
                                        Dim cm_a As Double = dftSoundBin_x(n) 'DftSound1_x_Bin(n)
                                        Dim cm_b As Double = dftSoundBin_y(n) ' DftSound1_y_Bin(n)
                                        Dim cm_c As Double = dftIR_Bin_x(n) ' DftSound2_x_Bin(n)
                                        Dim cm_d As Double = dftIR_Bin_y(n) ' DftSound2_y_Bin(n)

                                        dftSoundBin_x(n) = (cm_a * cm_c + cm_b * cm_d) / (cm_c ^ 2 + cm_d ^ 2)
                                        dftSoundBin_y(n) = (cm_b * cm_c - cm_a * cm_d) / (cm_c ^ 2 + cm_d ^ 2)

                                    Next


                                    'Calculates inverse FFT
                                    FastFourierTransform(FftDirections.Backward, dftSoundBin_x, dftSoundBin_y)

                                    'Puts the convoluted sound in the output array
                                    For sample = 0 To sliceLength - 1
                                        OutputChannelSampleArray(writeSample) += dftSoundBin_x(sample)
                                        writeSample += 1
                                    Next
                                    'writeSample -= AddOverlapSliceLength

                                Next

                                'Referencin the sound array in the output sound
                                outputSound.WaveData.SampleData(c) = OutputChannelSampleArray


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



            Public Function Resample_UsingResampAudio(ByVal InputSound As Sound,
                                                  ByVal TargetWaveFormat As Formats.WaveFormat,
                                                  Optional ByVal ResampAudioPath As String = "C:\AFsp_Win\ResampAudio.exe",
                                                  Optional ByVal WorkFolder As String = "",
                                                  Optional ByVal CopyPTWFObjectToOutput As Boolean = False, Optional NoExtensiveFormat As Boolean = True) As Sound

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

                Dim FTYPE As String
                If NoExtensiveFormat = True Then
                    FTYPE = "WAVE-NOEX"
                Else
                    FTYPE = "WAVE"
                End If

                Dim ResampSigStartInfo As New ProcessStartInfo()
                ResampSigStartInfo.FileName = ResampAudioPath
                ResampSigStartInfo.Arguments = "-s " & TargetWaveFormat.SampleRate.ToString & " -F " & FTYPE & " -D " & DFormat & " " & Path.Combine(WorkFolder, TempSoundOriginalFileName) & ".wav " & Path.Combine(WorkFolder, TempSoundResampledFileName) & ".wav"
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

                    ResampledSignalSound.SMA = InputSound.SMA.CreateCopy(SMAParentSound)

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

                            'Using the default calibration stored in Standard_dBFS_dBSPL_Difference, if no dBSPL_dBFS_Difference is assigned
                            If dBSPL_dBFS_Difference Is Nothing Then dBSPL_dBFS_Difference = Audio.Standard_dBFS_dBSPL_Difference
                            Dim MinimumIncludedMaxLevel_dBFS As Double = MinimumIncludedMaxLevel_dBSPL - dBSPL_dBFS_Difference

                            If MeasurementSoundMaxLevel < MinimumIncludedMaxLevel_dBFS Then Continue For
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

#End Region


        End Module

        Public Module PsychoAcoustics
            Public Structure SiiCriticalBands
                ''' <summary>
                ''' Critical band centre frequencies according to table 1 in ANSI S3.5-1997
                ''' </summary>
                Public Shared CentreFrequencies As Double() = {150, 250, 350, 450, 570, 700, 840, 1000, 1170, 1370, 1600, 1850, 2150, 2500, 2900, 3400, 4000, 4800, 5800, 7000, 8500}
                Public Shared LowerCutoffFrequencies As Double() = {100, 200, 300, 400, 510, 630, 770, 920, 1080, 1270, 1480, 1720, 2000, 2320, 2700, 3150, 3700, 4400, 5300, 6400, 7700}
                Public Shared UpperCutoffFrequencies As Double() = {200, 300, 400, 510, 630, 770, 920, 1080, 1270, 1480, 1720, 2000, 2320, 2700, 3150, 3700, 4400, 5300, 6400, 7700, 9500}
            End Structure


        End Module


        Public Class BandBank
            Inherits List(Of BandInfo)

            Public Function GetCentreFrequencies() As Double()
                Dim Output As New List(Of Double)
                For Each band In Me
                    Output.Add(band.CentreFrequency)
                Next
                Return Output.ToArray
            End Function

            Public Function GetBandWidths() As Double()
                Dim Output As New List(Of Double)
                For Each band In Me
                    Output.Add(band.Bandwidth)
                Next
                Return Output.ToArray
            End Function


            ''' <summary>
            ''' Calculates and returns the spectrum levels equivalent to band levels of 0 dB SPL. Keys represent frequencies and values represent spectrum levels.
            ''' </summary>
            ''' <returns></returns>
            Public Function GetReferenceSpectrumLevels() As SortedList(Of Double, Double)

                'Getting the band widths
                Dim BandWidths = GetBandWidths()

                Dim CentreFrequencies = GetCentreFrequencies()

                'Creating a list to store the spectrum levels
                Dim SpectrumLevelList As New SortedList(Of Double, Double)

                For i = 0 To BandWidths.Count - 1

                    'Sets band levels to 0 dB SPL
                    Dim BandLevel_SPL As Double = 0

                    'Calculating spectrum level according to equation 3 in ANSI S3.5-1997 (The SII-standard)
                    Dim SpectrumLevel As Double = BandLevel2SpectrumLevel(BandLevel_SPL, BandWidths(i))
                    SpectrumLevelList.Add(CentreFrequencies(i), SpectrumLevel)
                Next

                Return SpectrumLevelList

            End Function

            Public Class BandInfo
                Public CentreFrequency As Double
                Public LowerFrequencyLimit As Double
                Public UpperFrequencyLimit As Double

                Public Sub New(ByVal CentreFrequency As Double, ByVal LowerFrequencyLimit As Double,
                           ByVal UpperFrequencyLimit As Double)

                    Me.CentreFrequency = CentreFrequency
                    Me.LowerFrequencyLimit = LowerFrequencyLimit
                    Me.UpperFrequencyLimit = UpperFrequencyLimit
                End Sub

                Public Function Bandwidth() As Double
                    Return UpperFrequencyLimit - LowerFrequencyLimit
                End Function
            End Class

            Public Shared Function GetSiiCriticalRatioBandBank() As BandBank

                Dim OutputBankBank As New BandBank

                'Adding critical band specifications
                For n = 0 To PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1
                    OutputBankBank.Add(New BandInfo(PsychoAcoustics.SiiCriticalBands.CentreFrequencies(n),
                                                    PsychoAcoustics.SiiCriticalBands.LowerCutoffFrequencies(n),
                                                    PsychoAcoustics.SiiCriticalBands.UpperCutoffFrequencies(n)))
                Next

                ''Adding critical band specifications
                'OutputBankBank.Add(New BandInfo(150, 100, 200))
                'OutputBankBank.Add(New BandInfo(250, 200, 300))
                'OutputBankBank.Add(New BandInfo(350, 300, 400))
                'OutputBankBank.Add(New BandInfo(450, 400, 510))
                'OutputBankBank.Add(New BandInfo(570, 510, 630))
                'OutputBankBank.Add(New BandInfo(700, 630, 770))
                'OutputBankBank.Add(New BandInfo(840, 770, 920))
                'OutputBankBank.Add(New BandInfo(1000, 920, 1080))
                'OutputBankBank.Add(New BandInfo(1170, 1080, 1270))
                'OutputBankBank.Add(New BandInfo(1370, 1270, 1480))
                'OutputBankBank.Add(New BandInfo(1600, 1480, 1720))
                'OutputBankBank.Add(New BandInfo(1850, 1720, 2000))
                'OutputBankBank.Add(New BandInfo(2150, 2000, 2320))
                'OutputBankBank.Add(New BandInfo(2500, 2320, 2700))
                'OutputBankBank.Add(New BandInfo(2900, 2700, 3150))
                'OutputBankBank.Add(New BandInfo(3400, 3150, 3700))
                'OutputBankBank.Add(New BandInfo(4000, 3700, 4400))
                'OutputBankBank.Add(New BandInfo(4800, 4400, 5300))
                'OutputBankBank.Add(New BandInfo(5800, 5300, 6400))
                'OutputBankBank.Add(New BandInfo(7000, 6400, 7700))
                'OutputBankBank.Add(New BandInfo(8500, 7700, 9500))

                Return OutputBankBank

            End Function

        End Class


        Public Class GammatoneFirFilterBank

            Public FilterKernels As List(Of Sound)
            Public CentreFrequencies As List(Of Double)
            Public Bandwidths As List(Of Double)
            Private _FilterFftFormat As New Formats.FftFormat(4096,,,, True)
            Private FilterCreationFftFormat As New Formats.FftFormat(4096,,,, True)
            Private WaveFormat As Formats.WaveFormat = Nothing

            Public ReadOnly Property FilterFftFormat As Formats.FftFormat
                Get
                    Return _FilterFftFormat
                End Get
            End Property

            Public ReadOnly FilterOrder As Integer

            Public Sub New(Optional ByVal FilterOrder As Integer = 4)
                Me.FilterOrder = FilterOrder
            End Sub

            Public Sub SetupCustomCenterFrequencies(ByVal WaveFormat As Formats.WaveFormat,
                       ByVal CentreFrequencies As List(Of Double),
                       Optional ByVal BandOverlapGainCompensation As Double = 0,
                       Optional ByVal Phases As List(Of Double) = Nothing)

                Me.WaveFormat = WaveFormat

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
                                                                 Phases(n), 210,
                                                                 FilterOrder, Bandwidths(n), 0.1, 0.01,,
                                                                 True, BandOverlapGainCompensation))
                Next


            End Sub

            Public Sub SetupAdjacentCentreFrequencies(ByVal WaveFormat As Formats.WaveFormat,
                                                      ByVal LowestFrequency As Double,
                                                      ByVal HighestFrequency As Double,
                                                      Optional ByVal Round As Boolean = True,
                                                      Optional ByVal ForceInclusionOfHighestFrequency As Boolean = False)

                Dim BandOverlapGainCompensation As Double = 10.297 'This constant was derived from measuring on a sine chirp, in order to get the right filter gain /EW

                Dim NewCenterFrequencies = New List(Of Double)
                Dim NextCentreFrequency As Double = LowestFrequency

                Dim LastBandwidth As Double
                Do
                    If NextCentreFrequency > HighestFrequency Then
                        Exit Do
                    End If

                    NewCenterFrequencies.Add(NextCentreFrequency)

                    LastBandwidth = CalculateGammatoneFilterBandWidth(NextCentreFrequency)

                    'NextCentreFrequency += LastBandwidth * 1.058195
                    'NextCentreFrequency += (LastBandwidth * 1.058195) / 2
                    NextCentreFrequency += LastBandwidth / 2

                Loop

                If ForceInclusionOfHighestFrequency = True Then
                    If Not NewCenterFrequencies.Contains(HighestFrequency) Then NewCenterFrequencies.Add(HighestFrequency)
                End If

                If Round = True Then
                    For n = 0 To NewCenterFrequencies.Count - 1
                        NewCenterFrequencies(n) = Math.Round(NewCenterFrequencies(n))
                    Next
                End If

                SetupCustomCenterFrequencies(WaveFormat, NewCenterFrequencies, BandOverlapGainCompensation)

            End Sub

            Public Shared Function CalculateGammatoneFilterBandWidth(ByVal CentreFrequency As Double) As Double
                Return 1.019 * (24.7 * (4.37 * (CentreFrequency / 1000) + 1))
            End Function


            Public Sub SetupAudiogramFrequencies(ByVal WaveFormat As Formats.WaveFormat)

                Dim AudiogramFrequencies = New List(Of Double) From {125, 250, 500, 750, 1000, 1500, 2000, 3000, 4000, 5000, 6000, 7000, 8000}
                Dim BandOverlapGainCompensation As Double = 0
                SetupCustomCenterFrequencies(WaveFormat, AudiogramFrequencies, BandOverlapGainCompensation)

            End Sub

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


            Public Function CreateGammatoneImpulseResponse(ByRef format As Formats.WaveFormat,
                                                       Optional ByVal Channel As Integer? = Nothing,
                                                       Optional ByVal CentreFrequency As Double = 1000,
                                                       Optional ByVal Phase As Double = 0,
                                                       Optional ByVal Amplitude As Double = 210,
                                                       Optional ByVal FilterOrder As Integer = 4,
                                                       Optional ByVal BandWidth As Double = 132.6,
                                                       Optional ByVal Duration As Double = 0.1,
                                                       Optional ByVal FadeOutDuration As Double = 0.01,
                                                       Optional ByVal DurationTimeUnit As TimeUnits = TimeUnits.seconds,
                                                       Optional ByVal ZeroPhaseKernel As Boolean = True,
                                                           Optional ByVal BandOverlapGainCompensation As Double = 0) As Sound
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
                    Dim CurrentGain = PostLevel - Prelevel + BandOverlapGainCompensation

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
                    Dim CurrentGain_B = PostLevel_B - PreLevel_B + BandOverlapGainCompensation

                    'Applying minus gain to the impulse response
                    DSP.AmplifySection(outputSound, -CurrentGain_B)

                    Return outputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function


            Public Class FilteredSound
                Public Property Sound As Sound
                Public Property CentreFrequency As Double
                Public Property Bandwidth As Double
            End Class

            Public Class FilteredSoundLevels
                Inherits FilteredSound
                Public Property SoundLevel As Double
                Public Property Unit As SoundDataUnit
                Public Property Type As SoundMeasurementType
                Public Property FrequencyWeighting As FrequencyWeightings
            End Class



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

                                Dim magnitudeArray(fftFormat.FftWindowSize - 1) As Double
                                Dim phaseArray(fftFormat.FftWindowSize - 1) As Double

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
                                DSP.TransformationsExt.FastFourierTransform(DSP.FftDirections.Backward, outputSound.FFT.FrequencyDomainRealData(c, 0).WindowData, outputSound.FFT.FrequencyDomainImaginaryData(c, 0).WindowData)

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

                            Dim magnitudeArray(fftFormat.FftWindowSize - 1) As Double
                            Dim phaseArray(fftFormat.FftWindowSize - 1) As Double

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
                            DSP.TransformationsExt.FastFourierTransform(DSP.FftDirections.Backward, X_Re, X_Im)

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
                    DSP.FastFourierTransform(DSP.FftDirections.Backward, averageMagnitudes, temporaryIMX)

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
            Public Function GetImpulseResponseForSpectralShaping(ByRef InputSound1 As Sound, ByRef InputSound2 As Sound,
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
                    Dim SubtractionMagnitudes(fftFormat.FftWindowSize - 1) As Double
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
                    Dim temporaryIMX(fftFormat.FftWindowSize - 1) As Double

                    'Performing an inverse dft on the magnitudes
                    DSP.FastFourierTransform(DSP.FftDirections.Backward, SubtractionMagnitudes, temporaryIMX)

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



            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="startSample_s1"></param>
            ''' <param name="sectionLength_s1"></param>
            ''' <param name="startSample_s2"></param>
            ''' <param name="sectionLength_s2"></param>
            ''' <param name="fftFormat"></param>
            ''' <param name="kernelSize"></param>
            ''' <param name="channel"></param>
            ''' <param name="LowerCutoffFrequency">'If set, gain below the indicated frequency will be 0 dB.</param>
            ''' <param name="UpperCutoffFrequency">'If set, gain above the indicated frequency will be 0 dB.</param>
            ''' <returns></returns>
            Public Function GetImpulseResponseForFrequencyResponseFlattening(ByRef InputSound As Sound,
                                                                 Optional ByVal startSample_s1 As Integer = 0, Optional ByVal sectionLength_s1 As Integer? = Nothing,
                                                                 Optional ByVal startSample_s2 As Integer = 0, Optional ByVal sectionLength_s2 As Integer? = Nothing,
                                                                 Optional ByRef fftFormat As Formats.FftFormat = Nothing,
                                                                 Optional ByVal kernelSize As Integer = 4000,
                                                                 Optional ByVal channel As Integer? = Nothing,
                                                                 Optional ByVal MaxBandGain As Double? = 20,
                                                                 Optional ByVal MaxBandAttenuation As Double? = 20,
                                                                 Optional ByVal LowerCutoffFrequency As Double? = Nothing,
                                                                 Optional ByVal UpperCutoffFrequency As Double? = Nothing) As Sound ' Optional normalizingSpectralMagnidutes As Boolean = True

                Throw New NotImplementedException("This function is not yet ready to be used!")

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
                Dim outputSound As New Sound(InputSound.WaveFormat)
                Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, channel)

                'Checks that kernel size is not larger than fftSize, increases fftSize is that is the case
                If kernelSize > fftFormat.FftWindowSize Then
                    CheckAndAdjustFFTSize(fftFormat.FftWindowSize, kernelSize)
                End If

                'Main section
                'Ser till att windowSize alltid är ett jämnt tal
                If fftFormat.FftWindowSize Mod 2 = 1 Then fftFormat.FftWindowSize += 1

                'Creating a flat spectrum sound
                'Dim FlatSpectrumSound = Audio.GenerateSound.CreateWhiteNoise(InputSound.WaveFormat, ,, InputSound.WaveData.LongestChannelSampleCount, TimeUnits.samples)
                Dim FlatSpectrumSound = Audio.GenerateSound.CreateSilence(InputSound.WaveFormat,  , InputSound.WaveData.LongestChannelSampleCount, TimeUnits.samples)
                'Setting the levels of FlatSpectrumSound to be equal to sound 1
                For c = 1 To FlatSpectrumSound.WaveFormat.Channels
                    FlatSpectrumSound.WaveData.SampleData(c)(Math.Floor(FlatSpectrumSound.WaveData.SampleData(c).Length / 2)) = 1
                    DSP.MeasureAndAdjustSectionLevel(FlatSpectrumSound, DSP.MeasureSectionLevel(FlatSpectrumSound, c), c)
                Next

                'Analysing input sounds
                'Performs a dft on the input file
                FlatSpectrumSound.FFT = DSP.SpectralAnalysis(FlatSpectrumSound, fftFormat, , startSample_s1, sectionLength_s1)
                InputSound.FFT = DSP.SpectralAnalysis(InputSound, fftFormat, , startSample_s2, sectionLength_s2)

                'Calculating magnitudes
                FlatSpectrumSound.FFT.CalculateAmplitudeSpectrum(True, True, True)
                InputSound.FFT.CalculateAmplitudeSpectrum(True, True, True)

                For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                    'Calculates average magnitudes
                    'Sound1
                    Dim BinSpectrum_1(FlatSpectrumSound.FFT.FftFormat.FftWindowSize - 1) As Single
                    For k = 0 To FlatSpectrumSound.FFT.AmplitudeSpectrum(c, 0).WindowData.Length - 1

                        For TimeWindow = 0 To FlatSpectrumSound.FFT.WindowCount(c) - 1

                            'Converting spectral magnitudes to power. Summing spectral power. 
                            'SummedBinPowers += 2 * 10 ^ ((20 * Math.Log10((Math.Sqrt(2) * AmplitudeSpectrum(channel, TimeWindow).WindowData(k)) / Math.Sqrt(2))) / 10)
                            'SummedBinPowers += 2 * 10 ^ ((20 * Math.Log10(AmplitudeSpectrum(channel, TimeWindow).WindowData(k))) / 10)
                            'Simplified to:
                            BinSpectrum_1(k) += 100 ^ (Math.Log10(FlatSpectrumSound.FFT.AmplitudeSpectrum(c, TimeWindow).WindowData(k)))

                        Next

                        'Taking the quare root to convert power spectrum to amplitude spectrum, and divides by WindowCount(Channel) to average the value of the time windows
                        BinSpectrum_1(k) = Math.Sqrt(BinSpectrum_1(k) / FlatSpectrumSound.FFT.WindowCount(c))

                        'Converting to dB
                        BinSpectrum_1(k) = dBConversion(BinSpectrum_1(k), dBConversionDirection.to_dB,
                                              FlatSpectrumSound.WaveFormat, dBTypes.SoundPressure)

                    Next


                    'Sound2 
                    'Calculates average magnitudes
                    'Sound2
                    Dim BinSpectrum_2(InputSound.FFT.FftFormat.FftWindowSize - 1) As Single
                    For k = 0 To InputSound.FFT.AmplitudeSpectrum(c, 0).WindowData.Length - 1

                        For TimeWindow = 0 To InputSound.FFT.WindowCount(c) - 1

                            'Converting spectral magnitudes to power. Summing spectral power. 
                            'SummedBinPowers += 2 * 10 ^ ((20 * Math.Log10((Math.Sqrt(2) * AmplitudeSpectrum(channel, TimeWindow).WindowData(k)) / Math.Sqrt(2))) / 10)
                            'SummedBinPowers += 2 * 10 ^ ((20 * Math.Log10(AmplitudeSpectrum(channel, TimeWindow).WindowData(k))) / 10)
                            'Simplified to:
                            BinSpectrum_2(k) += 100 ^ (Math.Log10(InputSound.FFT.AmplitudeSpectrum(c, TimeWindow).WindowData(k)))

                        Next

                        'Taking the quare root to convert power spectrum to amplitude spectrum, and divides by WindowCount(Channel) to average the value of the time windows
                        BinSpectrum_2(k) = Math.Sqrt(BinSpectrum_2(k) / InputSound.FFT.WindowCount(c))

                        'Converting to dB
                        BinSpectrum_2(k) = dBConversion(BinSpectrum_2(k), dBConversionDirection.to_dB,
                                              InputSound.WaveFormat, dBTypes.SoundPressure)

                    Next

                    'Calculating needed gain 
                    Dim SubtractionMagnitudes(fftFormat.FftWindowSize - 1) As Double
                    For n = 0 To fftFormat.FftWindowSize - 1
                        'converting to and from dB, limiting band gain to MaxBandGain
                        'Dim s1Level = dBConversion(averageMagnitudes_s1(n), dBConversionDirection.to_dB, InputSound1.WaveFormat)
                        'Dim s2Level = dBConversion(averageMagnitudes_s2(n), dBConversionDirection.to_dB, InputSound1.WaveFormat)

                        Dim s1Level = BinSpectrum_1(n)
                        Dim s2Level = BinSpectrum_2(n)

                        Dim TargetGain As Double = s1Level - s2Level
                        If MaxBandGain.HasValue Then TargetGain = Math.Min(MaxBandGain.Value, TargetGain)
                        If MaxBandAttenuation.HasValue Then TargetGain = Math.Max(-MaxBandAttenuation.Value, TargetGain)
                        SubtractionMagnitudes(n) = dBConversion(TargetGain, dBConversionDirection.from_dB, FlatSpectrumSound.WaveFormat)

                    Next

                    'Overriding any values below and above the cut-off frequencies with zero
                    If LowerCutoffFrequency.HasValue Then

                        'Calculating the cut-off bin index
                        Dim LowerCutoffBin As Integer = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                                                                              LowerCutoffFrequency.Value,
                                                                              FlatSpectrumSound.WaveFormat.SampleRate,
                                                                              fftFormat.FftWindowSize, Utils.roundingMethods.alwaysDown)

                        Dim NoGainValue As Double = dBConversion(0, dBConversionDirection.from_dB, FlatSpectrumSound.WaveFormat)

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
                                                                              FlatSpectrumSound.WaveFormat.SampleRate,
                                                                              fftFormat.FftWindowSize, Utils.roundingMethods.alwaysDown)

                        Dim NoGainValue As Double = dBConversion(0, dBConversionDirection.from_dB, FlatSpectrumSound.WaveFormat)

                        For PositiveBinIndex = UpperCutoffBin To fftFormat.FftWindowSize / 2 - 1

                            'Positive frequencies
                            SubtractionMagnitudes(PositiveBinIndex) = NoGainValue

                            'Negative frequencies
                            Dim NegativeBinIndex As Integer = fftFormat.FftWindowSize - 1 - PositiveBinIndex
                            SubtractionMagnitudes(NegativeBinIndex) = NoGainValue
                        Next
                    End If

                    'Since the phase can be set to 0, the real part of the signal is equal to the magnitudes
                    Dim temporaryIMX(fftFormat.FftWindowSize - 1) As Double

                    'Performing an inverse dft on the magnitudes
                    DSP.FastFourierTransform(DSP.FftDirections.Backward, SubtractionMagnitudes, temporaryIMX)

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
                FlatSpectrumSound.FFT = Nothing
                InputSound.FFT = Nothing

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
                                                           Optional ByVal ValidateSoundLevel As Boolean = True,
                                                                Optional ByRef InActivateWarnings As Boolean = False) As Sound

                'Measuring input sound level (channel 1 only)
                Dim InputLevel As Double
                If ValidateSoundLevel Then InputLevel = DSP.MeasureSectionLevel(InputSound, 1)

                'Setting up a suitable fft format
                Dim FftFormat As New Formats.FftFormat(KernelSize,,,, InActivateWarnings)
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


End Namespace