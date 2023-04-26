'This software is available under the following license:
'MIT/X11 License
'
'Copyright (c) 2020 Erik Witte
'
'Permission is hereby granted, free of charge, to any person obtaining a copy
'of this software and associated documentation files (the ''Software''), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'copies of the Software, and to permit persons to whom the Software is
'furnished to do so, subject to the following conditions:
'
'The above copyright notice and this permission notice shall be included in all
'copies or substantial portions of the Software.
'
'THE SOFTWARE IS PROVIDED ''AS IS'', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
'SOFTWARE.

Imports System.Threading

Public Module Constants
    Public Const twopi As Double = 2 * Math.PI 'Or 2 * Math.Acos(-1)

End Module

Namespace Audio

    Namespace DSP



        Public Module Measurements


            ''' <summary>
            ''' Measures the sound level of a section of the input sound.
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="channel"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength"></param>
            ''' <param name="outputUnit"></param>
            ''' <param name="SoundMeasurementType"></param>
            ''' <param name="Frequencyweighting">The frequency Weighting to be applied before the sound measurement.</param>
            ''' <param name="ReturnLinearMeanSquareData">If set to true, the linear mean square of the measured section is returned (I.e. Any values set for outputUnit and SoundMeasurementType are overridden.).</param>
            ''' <param name="LinearSquareData">If ReturnLinearMeanSquareData is set to True, LinearSquareData will contain item1 = linear sum of square, and item2 = length of the measurement section in samples.</param>
            ''' <returns></returns>
            Public Function MeasureSectionLevel(ByRef InputSound As Sound, ByVal channel As Integer,
                                            Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing,
                                            Optional ByVal outputUnit As SoundDataUnit = SoundDataUnit.dB,
                                            Optional ByVal SoundMeasurementType As SoundMeasurementType = SoundMeasurementType.RMS,
                                            Optional ByVal Frequencyweighting As FrequencyWeightings = FrequencyWeightings.Z,
                                            Optional ByVal ReturnLinearMeanSquareData As Boolean = False,
                                            Optional ByRef LinearSquareData As Tuple(Of Double, Integer) = Nothing) As Double?

                Try

                    CheckAndCorrectSectionLength(InputSound.WaveData.SampleData(channel).Length, startSample, sectionLength)

                    'Preparing an array to do measurements on
                    Dim MeasurementArray() As Single

                    If Frequencyweighting = FrequencyWeightings.Z Then
                        'Just referencing the input sound array
                        MeasurementArray = InputSound.WaveData.SampleData(channel)

                    Else
                        'Preparing a new sound with only the measurement section, to be filterred
                        Dim tempSound As New Sound(InputSound.WaveFormat)
                        ReDim MeasurementArray(sectionLength - 1)
                        For s = startSample To startSample + sectionLength - 1
                            MeasurementArray(s - startSample) = InputSound.WaveData.SampleData(channel)(s)
                        Next
                        tempSound.WaveData.SampleData(channel) = MeasurementArray

                        'Filterring for Weighing
                        tempSound = DSP.IIRFilter(tempSound, Frequencyweighting, channel)
                        If tempSound Is Nothing Then
                            Throw New Exception("Something went wrong during IIR-filterring")
                            Return Nothing 'Aborting and return vb null if something went wrong here
                        End If

                        'Referencing the MeasurementArray again (since the reference is broken during the IIR filtering)
                        MeasurementArray = tempSound.WaveData.SampleData(channel)

                        'Setting startsample to 0 since all sound before the startsample has been excluded from MeasurementArray
                        startSample = 0

                    End If

                    'Overrides soundmeasurmenttype
                    If ReturnLinearMeanSquareData = True Then SoundMeasurementType = SoundMeasurementType.RMS

                    Select Case SoundMeasurementType
                        Case SoundMeasurementType.RMS

                            'Calculates RMS value of the section

                            Dim AccumulativeSoundLevel As Double
                            For n = startSample To startSample + sectionLength - 1
                                AccumulativeSoundLevel = AccumulativeSoundLevel + MeasurementArray(n) ^ 2
                            Next

                            'Returns the mean square (MR) if ReturnLinearMeanSquareData is True
                            If ReturnLinearMeanSquareData = True Then

                                'Stores LinearSquareData
                                LinearSquareData = New Tuple(Of Double, Integer)(AccumulativeSoundLevel, sectionLength)

                                'Returns the ReturnLinearMeanSquareData
                                Return AccumulativeSoundLevel / sectionLength
                            End If

                            'Calculates RMS
                            Dim RMS = (AccumulativeSoundLevel / sectionLength) ^ (1 / 2)

                            Select Case outputUnit
                                Case SoundDataUnit.dB
                                    Dim sectionLevel As Double = dBConversion(RMS, dBConversionDirection.to_dB, InputSound.WaveFormat)
                                    Return sectionLevel
                                Case SoundDataUnit.linear
                                    Return RMS
                            End Select

                        Case SoundMeasurementType.AbsolutePeakAmplitude
                            'Calculates the absolute max amplitude of the section

                            Dim peak_pos As Double '= inputArray.Max (Detekterar peakvärdet för hela Arrayen) 
                            Dim peak_neg As Double '= inputArray.Min

                            'Detekterar peakvärdet för sectionen
                            For n = startSample To startSample + sectionLength - 1
                                If MeasurementArray(n) > peak_pos Then peak_pos = MeasurementArray(n)
                                If MeasurementArray(n) < peak_neg Then peak_neg = MeasurementArray(n)
                            Next

                            Dim peak As Double = 0

                            If peak_pos > -peak_neg Then
                                peak = peak_pos
                            Else
                                peak = -peak_neg
                            End If

                            Select Case outputUnit
                                Case SoundDataUnit.dB
                                    Dim sectionLevel As Double = dBConversion(peak, dBConversionDirection.to_dB, InputSound.WaveFormat)
                                    Return sectionLevel
                                Case SoundDataUnit.linear
                                    Return peak
                            End Select

                        Case SoundMeasurementType.averageAbsoluteAmplitude

                            'Calculates the average absolute amplitude of the section
                            Dim AccumulativeSoundLevel As Double

                            'MsgBox(inputArray.Length & " " & startSample & " " & sectionLength)

                            For n = startSample To startSample + sectionLength - 1
                                AccumulativeSoundLevel = AccumulativeSoundLevel + Math.Abs(MeasurementArray(n))
                            Next

                            Dim averageAbsoluteAmplitude = AccumulativeSoundLevel / sectionLength

                            Select Case outputUnit
                                Case SoundDataUnit.dB
                                    Dim sectionLevel As Double = dBConversion(averageAbsoluteAmplitude, dBConversionDirection.to_dB, InputSound.WaveFormat)
                                    Return sectionLevel
                                Case SoundDataUnit.linear
                                    Return averageAbsoluteAmplitude
                            End Select

                    End Select


                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try

            End Function

            ''' <summary>
            ''' Calculates band levels of the specified channel in the input sound. .
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="Channel"></param>
            ''' <param name="BandBank">If left as Nothing a default SII Critical Band bank will be created, used and referenced upon return.</param>
            ''' <param name="FftFormat"></param>
            ''' <param name="ActualLowerLimitFrequencyList ">Upon return, contains the actual lower band limits used.</param>
            ''' <param name="ActualUpperLimitFrequencyList">Upon return, contains the actual upper band limits used.</param>
            ''' <returns></returns>
            Public Function CalculateBandLevels(ByRef InputSound As Sound, ByVal Channel As Integer,
                                                Optional ByRef BandBank As Audio.DSP.BandBank = Nothing,
                                                Optional ByRef FftFormat As Audio.Formats.FftFormat = Nothing,
                                                Optional ByRef ActualLowerLimitFrequencyList As List(Of Double) = Nothing,
                                                Optional ByRef ActualUpperLimitFrequencyList As List(Of Double) = Nothing) As List(Of Double)

                Try

                    'Setting default band frequencies
                    If BandBank Is Nothing Then BandBank = Audio.DSP.BandBank.GetSiiCriticalRatioBandBank

                    'Setting up FFT format
                    If FftFormat Is Nothing Then FftFormat = New Audio.Formats.FftFormat(4 * 2048,, 1024, Audio.WindowingType.Hamming, False)

                    'Creating an output list
                    Dim BandLevelList As New List(Of Double)

                    'Creating a list to hold actual band limits
                    ActualLowerLimitFrequencyList = New List(Of Double)
                    ActualUpperLimitFrequencyList = New List(Of Double)

                    'Calculating spectra
                    InputSound.FFT = Audio.DSP.SpectralAnalysis(InputSound, FftFormat)
                    InputSound.FFT.CalculatePowerSpectrum(True, True, True, 0.25)

                    For Each band In BandBank

                        Dim ActualLowerLimitFrequency As Double
                        Dim ActualUpperLimitFrequency As Double

                        Dim WindowLevelArray = Audio.DSP.AcousticDistance.CalculateWindowLevels(InputSound,,,
                                                                          band.LowerFrequencyLimit,
                                                                          band.UpperFrequencyLimit,
                                                                          Audio.FftData.GetSpectrumLevel_InputType.FftBinCentreFrequency_Hz,
                                                                          False, False,
                                                                          ActualLowerLimitFrequency,
                                                                          ActualUpperLimitFrequency)

                        Dim AverageBandLevel_FS As Double = WindowLevelArray.Average
                        BandLevelList.Add(AverageBandLevel_FS)

                        ActualLowerLimitFrequencyList.Add(ActualLowerLimitFrequency)
                        ActualUpperLimitFrequencyList.Add(ActualUpperLimitFrequency)

                    Next

                    Return BandLevelList

                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try

            End Function



            ''' <summary>
            ''' Calculates the sound level in the input sound using frequency domain calculations.
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="Channel"></param>
            ''' <param name="FftFormat"></param>
            ''' <returns></returns>
            Public Function CalculateSoundLevelFromFrequencyDomain(ByRef InputSound As Sound, ByVal Channel As Integer,
                                                Optional ByRef FftFormat As Audio.Formats.FftFormat = Nothing) As Double

                Try

                    'Setting up FFT format
                    If FftFormat Is Nothing Then FftFormat = New Audio.Formats.FftFormat(4 * 2048,, 1024, Audio.WindowingType.Hamming, False)

                    'Calculating spectra
                    InputSound.FFT = Audio.DSP.SpectralAnalysis(InputSound, FftFormat)
                    InputSound.FFT.CalculatePowerSpectrum(True, True, True, 0.25)

                    Dim ActualLowerLimitFrequency As Double
                    Dim ActualUpperLimitFrequency As Double

                    Dim WindowLevelArray = Audio.DSP.AcousticDistance.CalculateWindowLevels(InputSound,,,
                                                                          ,
                                                                          ,
                                                                          Audio.FftData.GetSpectrumLevel_InputType.FftBinCentreFrequency_Hz,
                                                                          False, False,
                                                                          ActualLowerLimitFrequency,
                                                                          ActualUpperLimitFrequency)

                    Dim AverageBandLevel_FS As Double = WindowLevelArray.Average

                    Return AverageBandLevel_FS

                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try

            End Function



            ''' <summary>
            ''' Calculates spectrum levels
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="Channel"></param>
            ''' <param name="BandBank">If left as Nothing a default SII Critical Band bank will be created, used and referenced upon return.</param>
            ''' <param name="FftFormat"></param>
            ''' <param name="ActualLowerLimitFrequencyList ">Upon return, contains the actual lower band limits used.</param>
            ''' <param name="ActualUpperLimitFrequencyList">Upon return, contains the actual upper band limits used.</param>
            ''' <param name="dBSPL_FSdifference"></param>
            ''' <returns></returns>
            Public Function CalculateSpectrumLevels(ByRef InputSound As Sound, ByVal Channel As Integer,
                                                    Optional ByRef BandBank As Audio.DSP.BandBank = Nothing,
                                                    Optional ByRef FftFormat As Audio.Formats.FftFormat = Nothing,
                                                    Optional ByRef ActualLowerLimitFrequencyList As List(Of Double) = Nothing,
                                                    Optional ByRef ActualUpperLimitFrequencyList As List(Of Double) = Nothing,
                                                    Optional ByRef dBSPL_FSdifference As Double? = Nothing) As List(Of Double)


                'Setting default dBSPL_FSdifference 
                If dBSPL_FSdifference Is Nothing Then dBSPL_FSdifference = Audio.Standard_dBFS_dBSPL_Difference

                'Calculates band levels
                Dim BandLevels = CalculateBandLevels(InputSound, Channel, BandBank, FftFormat, ActualLowerLimitFrequencyList, ActualUpperLimitFrequencyList)

                'Getting the band widths
                Dim BandWidths = BandBank.GetBandWidths

                'Creating a list to store the spectrum levels
                Dim SpectrumLevelList As New List(Of Double)

                For i = 0 To BandLevels.Count - 1

                    'Converting dB FS to dB SPL
                    Dim BandLevel_SPL As Double = BandLevels(i) + dBSPL_FSdifference

                    'Calculating spectrum level according to equation 3 in ANSI S3.5-1997 (The SII-standard)
                    Dim SpectrumLevel As Double = BandLevel2SpectrumLevel(BandLevel_SPL, BandWidths(i))
                    SpectrumLevelList.Add(SpectrumLevel)
                Next

                Return SpectrumLevelList

            End Function


            ''' <summary>
            ''' Calculating spectrum level according to equation 3 in ANSI S3.5-1997 (The SII-standard)
            ''' </summary>
            ''' <param name="BandLevel_SPL"></param>
            ''' <param name="BandWidth"></param>
            ''' <returns></returns>
            Public Function BandLevel2SpectrumLevel(ByVal BandLevel_SPL As Double, ByVal BandWidth As Double) As Double

                'Calculating spectrum level according to equation 3 in ANSI S3.5-1997 (The SII-standard)
                Return BandLevel_SPL - 10 * Math.Log10(BandWidth / 1)

            End Function

            ''' <summary>
            ''' Returns the RMS of the window with the highest RMS value.
            ''' </summary>
            ''' <param name="InputSound">The sound to measure.</param>
            ''' <param name="WindowSize">The windows size in samples.</param>
            ''' <param name="LoudestWindowStartSample">Upon return, holds the start sample of loudest window.</param>
            ''' <returns></returns>
            Public Function GetLevelOfLoudestWindow(ByRef InputSound As Sound, ByVal Channel As Integer, ByVal WindowSize As Integer,
                                                Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing,
                                                Optional ByRef LoudestWindowStartSample As Integer = 0,
                                                Optional ByVal Frequencyweighting As FrequencyWeightings = FrequencyWeightings.Z,
                                                Optional ByVal ZeroPadToWindowSize As Boolean = False,
                                                    Optional ByRef WindowLevels As List(Of Double) = Nothing) As Double


                CheckAndCorrectSectionLength(InputSound.WaveData.SampleData(Channel).Length, StartSample, SectionLength)

                'Stores the initial start sample value since this is changed if the sound is filtering below
                Dim InitialStartSample As Integer = StartSample

                'Resetting LoudestWindowStartSample
                LoudestWindowStartSample = 0

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
                    TempSound = DSP.IIRFilter(TempSound, Frequencyweighting, 1)
                    If TempSound Is Nothing Then
                        Throw New Exception("Something went wrong during IIR-filterring")
                        Return Nothing 'Aborting and returning Nothing if something went wrong here
                    End If
                End If


                Dim HighestSumOfSquares As Double = Double.NegativeInfinity

                'If the section to measure is shorter than the WindowSize, MeasureSectionLevel is used directly (with or without zero-padding)
                If WindowSize > TempSound.WaveData.SampleData(1).Length Then
                    If ZeroPadToWindowSize = True Then

                        'Zero-padding the sample array
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

                    'Stroring the CurrentSumOfSquares in WindowLevels
                    If WindowLevels IsNot Nothing Then
                        WindowLevels.Add(CurrentSumOfSquares)
                    End If

                    'Updating and storing the loudest window start sample
                    If CurrentSumOfSquares > HighestSumOfSquares Then
                        HighestSumOfSquares = CurrentSumOfSquares
                        LoudestWindowStartSample = s
                    End If

                Next

                'Adding the section length removed prior to the initial start sample to LoudestWindowStartSample
                LoudestWindowStartSample += InitialStartSample

                If WindowLevels IsNot Nothing Then
                    'Calulating the RMS level in dB for the WindowLevels 
                    Dim dBList As New List(Of Double)
                    For Each SuMOfSquareValue In WindowLevels
                        If SuMOfSquareValue > 0 Then
                            dBList.Add(dBConversion(Math.Sqrt(SuMOfSquareValue / WindowSize), dBConversionDirection.to_dB, TempSound.WaveFormat))
                        Else
                            dBList.Add(Double.NegativeInfinity)
                        End If
                    Next
                    WindowLevels = dBList
                End If

                'Calulating the RMS level in dB
                If HighestSumOfSquares > 0 Then
                    Return dBConversion(Math.Sqrt(HighestSumOfSquares / WindowSize), dBConversionDirection.to_dB, TempSound.WaveFormat)
                Else
                    Return Double.NegativeInfinity
                End If

            End Function


            ''' <summary>
            ''' Calculating the average RMS sound level in all InputSounds (as if they were one long sound)
            ''' </summary>
            ''' <param name="InputSounds"></param>
            ''' <param name="SoundChannel"></param>
            ''' <param name="FrequencyWeighting"></param>
            ''' <returns></returns>
            Public Function GetSoundLevelOfConcatenatedSoundsRecordings(ByVal InputSounds As List(Of Audio.Sound), ByVal SoundChannel As Integer,
                                                                  ByVal FrequencyWeighting As Audio.FrequencyWeightings) As Double

                Dim SumOfSquaresList As New List(Of Tuple(Of Double, Integer))

                For Each TestWordRecording In InputSounds

                    'Measures the test word region of each sound
                    Dim SumOfSquareData As Tuple(Of Double, Integer) = Nothing
                    Audio.DSP.MeasureSectionLevel(TestWordRecording, SoundChannel, 0, TestWordRecording.WaveData.SampleData(SoundChannel).Length,,, FrequencyWeighting, True, SumOfSquareData)
                    'Adds the sum-of-square data
                    SumOfSquaresList.Add(SumOfSquareData)

                Next

                'Calculating a weighted average sum of squares. 
                Dim SumOfSquares As Double = 0
                Dim TotalLength As Double = 0
                For n = 0 To SumOfSquaresList.Count - 1
                    SumOfSquares += SumOfSquaresList(n).Item1
                    TotalLength += SumOfSquaresList(n).Item2
                Next

                'Calculating mean square
                Dim MeanSquare As Double = SumOfSquares / TotalLength

                'Calculating RMS by taking the root of the mean of the MeanSquare
                Dim RMS As Double = MeanSquare ^ (1 / 2)

                'Converting to dB
                Dim RMSLevel As Double = Audio.dBConversion(RMS, Audio.dBConversionDirection.to_dB, InputSounds(0).WaveFormat)

                Return RMSLevel

            End Function

        End Module

        Public Module Transformations

            ''' <summary>
            ''' Amplifys a section of the sound.
            ''' </summary>
            ''' <param name="InputSound">The sound to modify.</param>
            ''' <param name="Gain">The amount of gain applied to the specified section (gain unit is set to dB or linear with the parameter GainUnit).</param>
            ''' <param name="Channel">The channel to be modified. If left empty, all channels will be modified.</param>
            ''' <param name="StartSample">Start sample of the section to be amplified.</param>
            ''' <param name="SectionLength">Length (in samples) of the section to be amplified.</param>
            ''' <param name="GainUnit">The unit of the gain paramameter (dB or linear)</param>
            ''' <returns>Returns the number samples whose value exceeded the value range of the Single data type (such value are set to Single.maxvalue or .minvalue), or vbNull if something else went wrong.</returns>
            Public Function AmplifySection(ByRef InputSound As Sound, ByVal Gain As Double,
                                       Optional ByVal Channel As Integer? = Nothing,
                                       Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing,
                                       Optional ByVal GainUnit As SoundDataUnit = SoundDataUnit.dB) As Integer

                Try

                    Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, Channel)

                    Dim totalDistortedSamples As Double = 0

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim gainFactor As Double = 0

                        Select Case GainUnit
                            Case SoundDataUnit.dB
                                gainFactor = 10 ^ (Gain / 20)

                            Case SoundDataUnit.linear
                                gainFactor = Gain

                            Case Else
                                Throw New NotImplementedException("Unsupported SoundDataUnit. Use either SoundDataUnit.dB Or .Linear.")
                        End Select


                        'Verkställer förstärkningen i sektionen (och varnar för distorsion)
                        Dim distorsion As Boolean = False
                        Dim distorsionSampleCount As Integer = 0
                        Dim otherErrorExeption As Exception = Nothing

                        Dim SoundArray = InputSound.WaveData.SampleData(c)

                        Dim CorrectedStartSample = StartSample
                        Dim CorrectedSectionLength = SectionLength
                        CheckAndCorrectSectionLength(SoundArray.Length, CorrectedStartSample, CorrectedSectionLength)

                        For n = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                            If SoundArray(n) * gainFactor > Single.MaxValue Then
                                SoundArray(n) = Single.MaxValue
                                distorsion = True
                                distorsionSampleCount += 1

                            ElseIf SoundArray(n) * gainFactor < Single.MinValue Then
                                SoundArray(n) = Single.MinValue
                                distorsion = True
                                distorsionSampleCount += 1

                            Else
                                SoundArray(n) *= gainFactor
                            End If
                        Next

                        If distorsion = True Then
                            AudioError("Distorsion occurred for " & distorsionSampleCount & " samples in AmplifySection", "Warning!")
                            totalDistortedSamples += distorsionSampleCount
                        End If

                    Next

                    Return totalDistortedSamples

                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return vbNull
                End Try

            End Function


            ''' <summary>
            ''' Amplifys a section of the sound.
            ''' </summary>
            ''' <param name="InputSound">The sound to modify.</param>
            ''' <param name="ChannelGain">A List containing the amount of gain applied to the specified section of each channel (The channel indices are zero-based (with gain for channel 1 at index 0, gain for channel 2 at index 1, etc.). The gain unit is set to dB or linear with the parameter GainUnit).</param>
            ''' <param name="StartSample">Start sample of the section to be amplified.</param>
            ''' <param name="SectionLength">Length (in samples) of the section to be amplified.</param>
            ''' <param name="GainUnit">The unit of the gain paramameter (dB or linear)</param>
            Public Sub AmplifySection(ByRef InputSound As Sound, ByVal ChannelGain As List(Of Double),
                                       Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing,
                                       Optional ByVal GainUnit As SoundDataUnit = SoundDataUnit.dB)

                Try

                    Dim InputSoundWaveFormat = InputSound.WaveFormat
                    Dim InputSoundLength As Integer = InputSound.WaveData.ShortestChannelSampleCount

                    'Determining if channels have equal length, or if there are more than 3 channels (as the faster version for eaqual length below is not yet implemented)
                    If InputSound.WaveData.IsEqualChannelLength() = False Or InputSound.WaveFormat.Channels > 3 Then

                        For c = 1 To InputSound.WaveFormat.Channels

                            If ChannelGain.Count <> 1 Then Throw New ArgumentException("ChannelGain must contain one gain value.")

                            'Calculating gain for each channel
                            Dim gainFactor As Double = 0
                            Select Case GainUnit
                                Case SoundDataUnit.dB
                                    gainFactor = 10 ^ (ChannelGain(0) / 20)
                                Case SoundDataUnit.linear
                                    gainFactor = ChannelGain(0)
                                Case Else
                                    Throw New NotImplementedException("Unsupported SoundDataUnit. Use either SoundDataUnit.dB Or .Linear.")
                            End Select

                            'Applies the gain
                            Dim ChannelArray = InputSound.WaveData.SampleData(c)

                            Dim CorrectedStartSample = StartSample
                            Dim CorrectedSectionLength = SectionLength
                            CheckAndCorrectSectionLength(ChannelArray.Length, CorrectedStartSample, CorrectedSectionLength)

                            For n = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                                ChannelArray(n) *= gainFactor
                            Next

                        Next

                    Else

                        Dim CorrectedStartSample = StartSample
                        Dim CorrectedSectionLength = SectionLength
                        CheckAndCorrectSectionLength(InputSound.WaveData.SampleData(1).Length, CorrectedStartSample, CorrectedSectionLength)

                        'Treating three channel sounds
                        Select Case InputSound.WaveFormat.Channels

                            Case 1

                                If ChannelGain.Count <> 1 Then Throw New ArgumentException("ChannelGain must contain one gain value.")

                                'Calculating gain for each channel
                                Dim gainFactor_Channel1 As Double = 0
                                Select Case GainUnit
                                    Case SoundDataUnit.dB
                                        gainFactor_Channel1 = 10 ^ (ChannelGain(0) / 20)
                                    Case SoundDataUnit.linear
                                        gainFactor_Channel1 = ChannelGain(0)
                                    Case Else
                                        Throw New NotImplementedException("Unsupported SoundDataUnit. Use either SoundDataUnit.dB Or .Linear.")
                                End Select


                                'Applies the gain
                                Dim Channel1Array = InputSound.WaveData.SampleData(1)
                                For n = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                                    Channel1Array(n) *= gainFactor_Channel1
                                Next

                            Case 2

                                If ChannelGain.Count <> 2 Then Throw New ArgumentException("ChannelGain must contain two gain values, one for each channel.")

                                'Calculating gain for each channel
                                Dim gainFactor_Channel1 As Double = 0
                                Select Case GainUnit
                                    Case SoundDataUnit.dB
                                        gainFactor_Channel1 = 10 ^ (ChannelGain(0) / 20)
                                    Case SoundDataUnit.linear
                                        gainFactor_Channel1 = ChannelGain(0)
                                    Case Else
                                        Throw New NotImplementedException("Unsupported SoundDataUnit. Use either SoundDataUnit.dB Or .Linear.")
                                End Select

                                Dim gainFactor_Channel2 As Double = 0
                                Select Case GainUnit
                                    Case SoundDataUnit.dB
                                        gainFactor_Channel2 = 10 ^ (ChannelGain(1) / 20)
                                    Case SoundDataUnit.linear
                                        gainFactor_Channel2 = ChannelGain(1)
                                    Case Else
                                        Throw New NotImplementedException("Unsupported SoundDataUnit. Use either SoundDataUnit.dB Or .Linear.")
                                End Select

                                'Applies the gain
                                Dim Channel1Array = InputSound.WaveData.SampleData(1)
                                Dim Channel2Array = InputSound.WaveData.SampleData(2)

                                For n = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                                    Channel1Array(n) *= gainFactor_Channel1
                                    Channel2Array(n) *= gainFactor_Channel2
                                Next

                            Case 3

                                If ChannelGain.Count <> 3 Then Throw New ArgumentException("ChannelGain must contain three gain values, one for each channel.")

                                'Calculating gain for each channel
                                Dim gainFactor_Channel1 As Double = 0
                                Select Case GainUnit
                                    Case SoundDataUnit.dB
                                        gainFactor_Channel1 = 10 ^ (ChannelGain(0) / 20)
                                    Case SoundDataUnit.linear
                                        gainFactor_Channel1 = ChannelGain(0)
                                    Case Else
                                        Throw New NotImplementedException("Unsupported SoundDataUnit. Use either SoundDataUnit.dB Or .Linear.")
                                End Select

                                Dim gainFactor_Channel2 As Double = 0
                                Select Case GainUnit
                                    Case SoundDataUnit.dB
                                        gainFactor_Channel2 = 10 ^ (ChannelGain(1) / 20)
                                    Case SoundDataUnit.linear
                                        gainFactor_Channel2 = ChannelGain(1)
                                    Case Else
                                        Throw New NotImplementedException("Unsupported SoundDataUnit. Use either SoundDataUnit.dB Or .Linear.")
                                End Select

                                Dim gainFactor_Channel3 As Double = 0
                                Select Case GainUnit
                                    Case SoundDataUnit.dB
                                        gainFactor_Channel3 = 10 ^ (ChannelGain(2) / 20)
                                    Case SoundDataUnit.linear
                                        gainFactor_Channel3 = ChannelGain(2)
                                    Case Else
                                        Throw New NotImplementedException("Unsupported SoundDataUnit. Use either SoundDataUnit.dB Or .Linear.")
                                End Select

                                'Applies the gain
                                Dim Channel1Array = InputSound.WaveData.SampleData(1)
                                Dim Channel2Array = InputSound.WaveData.SampleData(2)
                                Dim Channel3Array = InputSound.WaveData.SampleData(3)

                                For n = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1
                                    Channel1Array(n) *= gainFactor_Channel1
                                    Channel2Array(n) *= gainFactor_Channel2
                                    Channel3Array(n) *= gainFactor_Channel3
                                Next

                            Case Else

                                AudioError("Amplify section is not yet implemented for " & InputSoundWaveFormat.Channels & " sounds. Exporting a silent sound!")

                                'Creates an empty sound with the length of the input sound if something went wrong
                                Dim SilentSound = GenerateSound.CreateSilence(InputSoundWaveFormat, , InputSoundLength, TimeUnits.samples)

                                InputSound = SilentSound

                        End Select

                    End If

                Catch ex As Exception
                    Throw New NotImplementedException("AmplifySection failed with the following error message:" & vbCrLf & vbCrLf & ex.ToString)
                End Try

            End Sub


            Public Function IIRFilter(ByVal inputSound As Sound, FrequencyWeighting As FrequencyWeightings,
                                  Optional ByVal channelToFilter As Integer? = Nothing) As Sound

                Select Case FrequencyWeighting
                    Case FrequencyWeightings.Z
                        'Returns input sound straight away if Z-Weighting is chosen
                        Return inputSound

                    Case FrequencyWeightings.K

                        'Since K-Weighting is done with 2 subsequent filters a special code path with 2 filters is made here.

                        'Filter 1
                        Dim ACoefficients() As Double = {1, -1.69065929318241, 0.73248077421585}
                        Dim BCoefficients() As Double = {1.53512485958697, -2.69169618940638, 1.19839281085285}
                        Dim GainIn_dB As Double = 0
                        Dim tempSound As Sound = IIR(inputSound, ACoefficients, BCoefficients, GainIn_dB, channelToFilter)

                        'Filter 2
                        ACoefficients = Nothing
                        BCoefficients = Nothing
                        GainIn_dB = 0
                        If Set_IIR_FrequencyWeightingCoefficients(inputSound.WaveFormat, FrequencyWeightings.RLB, ACoefficients, BCoefficients, GainIn_dB) = False Then
                            Return Nothing
                        End If
                        Dim outputSound As Sound = IIR(tempSound, ACoefficients, BCoefficients, GainIn_dB, channelToFilter)

                        If Not outputSound Is Nothing Then
                            Return outputSound
                        Else
                            Return Nothing
                        End If


                    Case Else

                        'For all other filter types

                        Dim ACoefficients() As Double = Nothing
                        Dim BCoefficients() As Double = Nothing

                        Dim GainIn_dB As Double = 0

                        If Set_IIR_FrequencyWeightingCoefficients(inputSound.WaveFormat, FrequencyWeighting, ACoefficients, BCoefficients, GainIn_dB) = False Then
                            Return Nothing
                        End If

                        Dim outputSound As Sound = IIR(inputSound, ACoefficients, BCoefficients, GainIn_dB, channelToFilter)
                        If Not outputSound Is Nothing Then
                            Return outputSound
                        Else
                            Return Nothing
                        End If


                End Select


            End Function

            Public Function IIRFilter(ByVal inputSound As Sound, ByVal ACoefficients() As Double, ByVal BCoefficients() As Double,
                         Optional ByVal channelToFilter As Integer? = Nothing) As Sound

                'Setting a default value of A0 to 1 if not set
                If ACoefficients(0) = vbNull Then ACoefficients(0) = 1

                Dim outputSound As Sound = IIR(inputSound, ACoefficients, BCoefficients, , channelToFilter)
                If Not outputSound Is Nothing Then
                    Return outputSound
                Else
                    Return Nothing
                End If

            End Function



            Private Function IIR(ByVal inputSound As Sound, ByVal ACoefficients() As Double, ByVal BCoefficients() As Double, Optional ByVal Gain As Double = 0,
                         Optional ByVal channelToFilter As Integer? = Nothing) As Sound

                Try

                    'Built on the standard formula for recursive filters. For Ref se e.g. Kates Digital Hearing Aids (2008), p 34.

                    Select Case inputSound.WaveFormat.BitDepth
                        Case 16, 32
                        Case Else
                            Throw New NotImplementedException("")
                    End Select

                    Dim outputSound As New Sound(inputSound.WaveFormat)
                    Dim AudioOutputConstructor As New AudioOutputConstructor(inputSound.WaveFormat, channelToFilter)
                    Dim FS As Double = inputSound.WaveFormat.PositiveFullScale

                    'Main section

                    Dim Zeroes As Double = 0
                    Dim Poles As Double = 0

                    Dim XMemory As New List(Of Double)
                    For sample = 0 To BCoefficients.Length - 2
                        XMemory.Add(0)
                    Next

                    Dim YMemory As New List(Of Double)
                    For sample = 0 To ACoefficients.Length - 2
                        YMemory.Add(0)
                    Next

                    Dim OutputSampleValue As Double = 0

                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim outputDoubleArray(inputSound.WaveData.SampleData(c).Length - 1) As Double
                        Dim CurrentInputArray = inputSound.WaveData.SampleData(c)

                        For sample = 0 To CurrentInputArray.Length - 1

                            'Summing up zeroes
                            Zeroes = BCoefficients(0) * CurrentInputArray(sample)
                            For index = 1 To BCoefficients.Count - 1
                                Zeroes += BCoefficients(index) * XMemory(index - 1)
                            Next

                            'Summing poles
                            Poles = 0
                            For index = 1 To ACoefficients.Count - 1
                                'Try 'Try-block commented out on 2019-09-03, The block should definitely not be necessary!
                                Poles += ACoefficients(index) * YMemory(index - 1)
                                'Catch ex As Exception
                                'MsgBox(ex.ToString)
                                'End Try
                            Next

                            OutputSampleValue = (1 / ACoefficients(0)) * (Zeroes - Poles)
                            outputDoubleArray(sample) = OutputSampleValue

                            'Adjusting the memories
                            XMemory.Insert(0, CurrentInputArray(sample))
                            YMemory.Insert(0, OutputSampleValue)
                            XMemory.RemoveAt(XMemory.Count - 1)
                            YMemory.RemoveAt(YMemory.Count - 1)

                        Next

                        'Applying gain and Convering to single
                        Dim OutputSingleArray(outputDoubleArray.Length - 1) As Single
                        For sample = 0 To outputDoubleArray.Length - 1
                            OutputSingleArray(sample) = CSng(outputDoubleArray(sample)) '*CSng(Gain) 
                        Next
                        outputSound.WaveData.SampleData(c) = OutputSingleArray

                        'Appyling gain
                        If Not Gain = 0 Then
                            DSP.Transformations.AmplifySection(outputSound, Gain, c,,,)
                        End If

                    Next

                    Return outputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function



            Private Function Set_IIR_FrequencyWeightingCoefficients(ByVal soundFormat As Formats.WaveFormat, ByVal FrequencyWeighting As FrequencyWeightings, ByRef ACoefficients() As Double, ByRef BCoefficients() As Double, Optional ByRef GainIn_dB As Double = 1) As Boolean

                Select Case FrequencyWeighting
                    Case FrequencyWeightings.A

                        'Source: Rimell, A. N., et al. (2015). "Design of digital filters for frequency weightings (A and C) required for risk assessments of workers exposed to noise." Industrial Health 53(1): 21-27.

                        Dim f1 As Double = 20.598997
                        Dim f2 As Double = 107.65265
                        Dim f3 As Double = 737.86223
                        Dim f4 As Double = 12194.217

                        Dim fs As Double = soundFormat.SampleRate

                        'Dim PI As Double = 3.1415926535897932384626433832D
                        'Dim PI As Double = Math.Acos(-1.0)
                        Dim PI As Double = Math.PI

                        'W1 represeents ω1′
                        'W4 represeents ω4′

                        'Dim W1 As Double = 2D * Math.Tan(PI * (f1 / fs))
                        'Dim W2 As Double = 2D * Math.Tan(PI * (f2 / fs))
                        'Dim W3 As Double = 2D * Math.Tan(PI * (f3 / fs))
                        'Dim W4 As Double = 2D * Math.Tan(PI * (f4 / fs))

                        Dim W1 As Double = 2 * Math.Tan(PI * (f1 / fs))
                        Dim W2 As Double = 2 * Math.Tan(PI * (f2 / fs))
                        Dim W3 As Double = 2 * Math.Tan(PI * (f3 / fs))
                        Dim W4 As Double = 2 * Math.Tan(PI * (f4 / fs))


                        ReDim ACoefficients(10)
                        ReDim BCoefficients(10)

                        ACoefficients(0) = 64 + (16 * W2 * W1 * W3) + (4 * W2 * W1 ^ 2 * W3) + (32 * W2 * W1 * W4) + (8 * W2 * W1 ^ 2 * W4) + (32 * W1 * W3 * W4) + (16 * W2 * W3 * W4) + (64 * W1) + (32 * W2) + (32 * W3) + (64 * W4) + (32 * W2 * W1) + (8 * W2 * W1 ^ 2) + (16 * W1 ^ 2) + (16 * W2 * W1 * W3 * W4) + (4 * W2 * W1 ^ 2 * W3 * W4) + (32 * W1 * W3) + (16 * W2 * W3) + (8 * W1 ^ 2 * W3) + (64 * W1 * W4) + (32 * W2 * W4) + (32 * W3 * W4) + (16 * W1 ^ 2 * W4) + (8 * W1 ^ 2 * W3 * W4) + (16 * W4 ^ 2) + (16 * W4 ^ 2 * W1) + (4 * W4 ^ 2 * W1 ^ 2) + (4 * W4 ^ 2 * W1 * W2 * W3) + (W4 ^ 2 * W1 ^ 2 * W2 * W3) + (8 * W4 ^ 2 * W1 * W2) + (2 * W4 ^ 2 * W1 ^ 2 * W2) + (8 * W4 ^ 2 * W2) + (8 * W4 ^ 2 * W3) + (8 * W4 ^ 2 * W1 * W3) + (2 * W4 ^ 2 * W1 ^ 2 * W3) + (4 * W4 ^ 2 * W2 * W3)
                        ACoefficients(1) = -128 + (64 * W2 * W1 * W3) + (24 * W2 * W1 ^ 2 * W3) + (128 * W2 * W1 * W4) + (48 * W2 * W1 ^ 2 * W4) + (128 * W1 * W3 * W4) + (64 * W2 * W3 * W4) + (64 * W2 * W1) + (32 * W2 * W1 ^ 2) + (32 * W1 ^ 2) + (96 * W2 * W1 * W3 * W4) + (32 * W2 * W1 ^ 2 * W3 * W4) + (64 * W1 * W3) + (32 * W2 * W3) + (32 * W1 ^ 2 * W3) + (128 * W1 * W4) + (64 * W2 * W4) + (64 * W3 * W4) + (64 * W1 ^ 2 * W4) + (48 * W1 ^ 2 * W3 * W4) + (32 * W4 ^ 2) + (64 * W4 ^ 2 * W1) + (24 * W4 ^ 2 * W1 ^ 2) + (32 * W4 ^ 2 * W1 * W2 * W3) + (10 * W4 ^ 2 * W1 ^ 2 * W2 * W3) + (48 * W4 ^ 2 * W1 * W2) + (16 * W4 ^ 2 * W1 ^ 2 * W2) + (32 * W4 ^ 2 * W2) + (32 * W4 ^ 2 * W3) + (48 * W4 ^ 2 * W1 * W3) + (16 * W4 ^ 2 * W1 ^ 2 * W3) + (24 * W4 ^ 2 * W2 * W3)
                        ACoefficients(2) = -192 + (48 * W2 * W1 * W3) + (52 * W2 * W1 ^ 2 * W3) + (96 * W2 * W1 * W4) + (104 * W2 * W1 ^ 2 * W4) + (96 * W1 * W3 * W4) + (48 * W2 * W3 * W4) - (320 * W1) - (160 * W2) - (160 * W3) - (320 * W4) - (96 * W2 * W1) + (24 * W2 * W1 ^ 2) - (48 * W1 ^ 2) + (208 * W2 * W1 * W3 * W4) + (108 * W2 * W1 ^ 2 * W3 * W4) - (96 * W1 * W3) - (48 * W2 * W3) + (24 * W1 ^ 2 * W3) - (192 * W1 * W4) - (96 * W2 * W4) - (96 * W3 * W4) + (48 * W1 ^ 2 * W4) + (104 * W1 ^ 2 * W3 * W4) - (48 * W4 ^ 2) + (48 * W4 ^ 2 * W1) + (52 * W4 ^ 2 * W1 ^ 2) + (108 * W4 ^ 2 * W1 * W2 * W3) + (45 * W4 ^ 2 * W1 ^ 2 * W2 * W3) + (104 * W4 ^ 2 * W1 * W2) + (54 * W4 ^ 2 * W1 ^ 2 * W2) + (24 * W4 ^ 2 * W2) + (24 * W4 ^ 2 * W3) + (104 * W4 ^ 2 * W1 * W3) + (54 * W4 ^ 2 * W1 ^ 2 * W3) + (52 * W4 ^ 2 * W2 * W3)
                        ACoefficients(3) = 512 - (128 * W2 * W1 * W3) + (32 * W2 * W1 ^ 2 * W3) - (256 * W2 * W1 * W4) + (64 * W2 * W1 ^ 2 * W4) - (256 * W1 * W3 * W4) - (128 * W2 * W3 * W4) - (256 * W2 * W1) - (64 * W2 * W1 ^ 2) - (128 * W1 ^ 2) + (128 * W2 * W1 * W3 * W4) + (192 * W2 * W1 ^ 2 * W3 * W4) - (256 * W1 * W3) - (128 * W2 * W3) - (64 * W1 ^ 2 * W3) - (512 * W1 * W4) - (256 * W2 * W4) - (256 * W3 * W4) - (128 * W1 ^ 2 * W4) + (64 * W1 ^ 2 * W3 * W4) - (128 * W4 ^ 2) - (128 * W4 ^ 2 * W1) + (32 * W4 ^ 2 * W1 ^ 2) + (192 * W4 ^ 2 * W1 * W2 * W3) + (120 * W4 ^ 2 * W1 ^ 2 * W2 * W3) + (64 * W4 ^ 2 * W1 * W2) + (96 * W4 ^ 2 * W1 ^ 2 * W2) - (64 * W4 ^ 2 * W2) - (64 * W4 ^ 2 * W3) + (64 * W4 ^ 2 * W1 * W3) + (96 * W4 ^ 2 * W1 ^ 2 * W3) + (32 * W4 ^ 2 * W2 * W3)
                        ACoefficients(4) = 128 - (224 * W2 * W1 * W3) - (56 * W2 * W1 ^ 2 * W3) - (448 * W2 * W1 * W4) - (112 * W2 * W1 ^ 2 * W4) - (448 * W1 * W3 * W4) - (224 * W2 * W3 * W4) + (640 * W1) + (320 * W2) + (320 * W3) + (640 * W4) + (64 * W2 * W1) - (112 * W2 * W1 ^ 2) + (32 * W1 ^ 2) - (224 * W2 * W1 * W3 * W4) + (168 * W2 * W1 ^ 2 * W3 * W4) + (64 * W1 * W3) + (32 * W2 * W3) - (112 * W1 ^ 2 * W3) + (128 * W1 * W4) + (64 * W2 * W4) + (64 * W3 * W4) - (224 * W1 ^ 2 * W4) - (112 * W1 ^ 2 * W3 * W4) + (32 * W4 ^ 2) - (224 * W4 ^ 2 * W1) - (56 * W4 ^ 2 * W1 ^ 2) + (168 * W4 ^ 2 * W1 * W2 * W3) + (210 * W4 ^ 2 * W1 ^ 2 * W2 * W3) - (112 * W4 ^ 2 * W1 * W2) + (84 * W4 ^ 2 * W1 ^ 2 * W2) - (112 * W4 ^ 2 * W2) - (112 * W4 ^ 2 * W3) - (112 * W4 ^ 2 * W1 * W3) + (84 * W4 ^ 2 * W1 ^ 2 * W3) - (56 * W4 ^ 2 * W2 * W3)
                        ACoefficients(5) = -(448 * W2 * W1 * W3 * W4) - (224 * W1 ^ 2 * W3 * W4) + (384 * W3 * W4) - (112 * W2 * W1 ^ 2 * W3) - (112 * W4 ^ 2 * W1 ^ 2) + (384 * W1 * W3) - (224 * W4 ^ 2 * W1 * W3) + (192 * W2 * W3) - (224 * W2 * W1 ^ 2 * W4) + (192 * W1 ^ 2) + (252 * W4 ^ 2 * W1 ^ 2 * W2 * W3) + (384 * W2 * W1) - (768) - (224 * W4 ^ 2 * W1 * W2) - (112 * W4 ^ 2 * W2 * W3) + (384 * W2 * W4) + (192 * W4 ^ 2) + (768 * W1 * W4)
                        ACoefficients(6) = 128 + (224 * W2 * W1 * W3) - (56 * W2 * W1 ^ 2 * W3) + (448 * W2 * W1 * W4) - (112 * W2 * W1 ^ 2 * W4) + (448 * W1 * W3 * W4) + (224 * W2 * W3 * W4) - (640 * W1) - (320 * W2) - (320 * W3) - (640 * W4) + (64 * W2 * W1) + (112 * W2 * W1 ^ 2) + (32 * W1 ^ 2) - (224 * W2 * W1 * W3 * W4) - (168 * W2 * W1 ^ 2 * W3 * W4) + (64 * W1 * W3) + (32 * W2 * W3) + (112 * W1 ^ 2 * W3) + (128 * W1 * W4) + (64 * W2 * W4) + (64 * W3 * W4) + (224 * W1 ^ 2 * W4) - (112 * W1 ^ 2 * W3 * W4) + (32 * W4 ^ 2) + (224 * W4 ^ 2 * W1) - (56 * W4 ^ 2 * W1 ^ 2) - (168 * W4 ^ 2 * W1 * W2 * W3) + (210 * W4 ^ 2 * W1 ^ 2 * W2 * W3) - (112 * W4 ^ 2 * W1 * W2) - (84 * W4 ^ 2 * W1 ^ 2 * W2) + (112 * W4 ^ 2 * W2) + (112 * W4 ^ 2 * W3) - (112 * W4 ^ 2 * W1 * W3) - (84 * W4 ^ 2 * W1 ^ 2 * W3) - (56 * W4 ^ 2 * W2 * W3)
                        ACoefficients(7) = 512 + (128 * W2 * W1 * W3) + (32 * W2 * W1 ^ 2 * W3) + (256 * W2 * W1 * W4) + (64 * W2 * W1 ^ 2 * W4) + (256 * W1 * W3 * W4) + (128 * W2 * W3 * W4) - (256 * W2 * W1) + (64 * W2 * W1 ^ 2) - (128 * W1 ^ 2) + (128 * W2 * W1 * W3 * W4) - (192 * W2 * W1 ^ 2 * W3 * W4) - (256 * W1 * W3) - (128 * W2 * W3) + (64 * W1 ^ 2 * W3) - (512 * W1 * W4) - (256 * W2 * W4) - (256 * W3 * W4) + (128 * W1 ^ 2 * W4) + (64 * W1 ^ 2 * W3 * W4) - (128 * W4 ^ 2) + (128 * W4 ^ 2 * W1) + (32 * W4 ^ 2 * W1 ^ 2) - (192 * W4 ^ 2 * W1 * W2 * W3) + (120 * W4 ^ 2 * W1 ^ 2 * W2 * W3) + (64 * W4 ^ 2 * W1 * W2) - (96 * W4 ^ 2 * W1 ^ 2 * W2) + (64 * W4 ^ 2 * W2) + (64 * W4 ^ 2 * W3) + (64 * W4 ^ 2 * W1 * W3) - (96 * W4 ^ 2 * W1 ^ 2 * W3) + (32 * W4 ^ 2 * W2 * W3)
                        ACoefficients(8) = -192 - (48 * W2 * W1 * W3) + (52 * W2 * W1 ^ 2 * W3) - (96 * W2 * W1 * W4) + (104 * W2 * W1 ^ 2 * W4) - (96 * W1 * W3 * W4) - (48 * W2 * W3 * W4) + (320 * W1) + (160 * W2) + (160 * W3) + (320 * W4) - (96 * W2 * W1) - (24 * W2 * W1 ^ 2) - (48 * W1 ^ 2) + (208 * W2 * W1 * W3 * W4) - (108 * W2 * W1 ^ 2 * W3 * W4) - (96 * W1 * W3) - (48 * W2 * W3) - (24 * W1 ^ 2 * W3) - (192 * W1 * W4) - (96 * W2 * W4) - (96 * W3 * W4) - (48 * W1 ^ 2 * W4) + (104 * W1 ^ 2 * W3 * W4) - (48 * W4 ^ 2) - (48 * W4 ^ 2 * W1) + (52 * W4 ^ 2 * W1 ^ 2) - (108 * W4 ^ 2 * W1 * W2 * W3) + (45 * W4 ^ 2 * W1 ^ 2 * W2 * W3) + (104 * W4 ^ 2 * W1 * W2) - (54 * W4 ^ 2 * W1 ^ 2 * W2) - (24 * W4 ^ 2 * W2) - (24 * W4 ^ 2 * W3) + (104 * W4 ^ 2 * W1 * W3) - (54 * W4 ^ 2 * W1 ^ 2 * W3) + (52 * W4 ^ 2 * W2 * W3)
                        ACoefficients(9) = -128 - (64 * W2 * W1 * W3) + (24 * W2 * W1 ^ 2 * W3) - (128 * W2 * W1 * W4) + (48 * W2 * W1 ^ 2 * W4) - (128 * W1 * W3 * W4) - (64 * W2 * W3 * W4) + (64 * W2 * W1) - (32 * W2 * W1 ^ 2) + (32 * W1 ^ 2) + (96 * W2 * W1 * W3 * W4) - (32 * W2 * W1 ^ 2 * W3 * W4) + (64 * W1 * W3) + (32 * W2 * W3) - (32 * W1 ^ 2 * W3) + (128 * W1 * W4) + (64 * W2 * W4) + (64 * W3 * W4) - (64 * W1 ^ 2 * W4) + (48 * W1 ^ 2 * W3 * W4) + (32 * W4 ^ 2) - (64 * W4 ^ 2 * W1) + (24 * W4 ^ 2 * W1 ^ 2) - (32 * W4 ^ 2 * W1 * W2 * W3) + (10 * W4 ^ 2 * W1 ^ 2 * W2 * W3) + (48 * W4 ^ 2 * W1 * W2) - (16 * W4 ^ 2 * W1 ^ 2 * W2) - (32 * W4 ^ 2 * W2) - (32 * W4 ^ 2 * W3) + (48 * W4 ^ 2 * W1 * W3) - (16 * W4 ^ 2 * W1 ^ 2 * W3) + (24 * W4 ^ 2 * W2 * W3)
                        ACoefficients(10) = 64 - (16 * W2 * W1 * W3) + (4 * W2 * W1 ^ 2 * W3) - (32 * W2 * W1 * W4) + (8 * W2 * W1 ^ 2 * W4) - (32 * W1 * W3 * W4) - (16 * W2 * W3 * W4) - (64 * W1) - (32 * W2) - (32 * W3) - (64 * W4) + (32 * W2 * W1) - (8 * W2 * W1 ^ 2) + (16 * W1 ^ 2) + (16 * W2 * W1 * W3 * W4) - (4 * W2 * W1 ^ 2 * W3 * W4) + (32 * W1 * W3) + (16 * W2 * W3) - (8 * W1 ^ 2 * W3) + (64 * W1 * W4) + (32 * W2 * W4) + (32 * W3 * W4) - (16 * W1 ^ 2 * W4) + (8 * W1 ^ 2 * W3 * W4) + (16 * W4 ^ 2) - (16 * W4 ^ 2 * W1) + (4 * W4 ^ 2 * W1 ^ 2) - (4 * W4 ^ 2 * W1 * W2 * W3) + (W4 ^ 2 * W1 ^ 2 * W2 * W3) + (8 * W4 ^ 2 * W1 * W2) - (2 * W4 ^ 2 * W1 ^ 2 * W2) - (8 * W4 ^ 2 * W2) - (8 * W4 ^ 2 * W3) + (8 * W4 ^ 2 * W1 * W3) - (2 * W4 ^ 2 * W1 ^ 2 * W3) + (4 * W4 ^ 2 * W2 * W3)
                        BCoefficients(0) = 16 * W4 ^ 2
                        BCoefficients(1) = 32 * W4 ^ 2
                        BCoefficients(2) = -48 * W4 ^ 2
                        BCoefficients(3) = -128 * W4 ^ 2
                        BCoefficients(4) = 32 * W4 ^ 2
                        BCoefficients(5) = 192 * W4 ^ 2
                        BCoefficients(6) = 32 * W4 ^ 2
                        BCoefficients(7) = -128 * W4 ^ 2
                        BCoefficients(8) = -48 * W4 ^ 2
                        BCoefficients(9) = 32 * W4 ^ 2
                        BCoefficients(10) = 16 * W4 ^ 2
                        'Gain = 10.0 ^ (2.0 / 20.0)
                        GainIn_dB = -2

                        Return True

                    Case FrequencyWeightings.C

                        'Source: Rimell, A. N., et al. (2015). "Design of digital filters for frequency weightings (A and C) required for risk assessments of workers exposed to noise." Industrial Health 53(1): 21-27.

                        Dim f1 As Double = 20.598997
                        Dim f2 As Double = 107.65265
                        Dim f3 As Double = 737.86223
                        Dim f4 As Double = 12194.217

                        Dim fs As Double = soundFormat.SampleRate

                        'Dim PI As double = 3.1415926535897932384626433832D
                        'Dim PI As double = Math.Acos(-1.0)
                        Dim PI As Double = Math.PI

                        'W1 represeents ω1′
                        'W4 represeents ω4′

                        Dim W1 As Double = 2D * Math.Tan(PI * (f1 / fs))
                        Dim W2 As Double = 2D * Math.Tan(PI * (f2 / fs))
                        Dim W3 As Double = 2D * Math.Tan(PI * (f3 / fs))
                        Dim W4 As Double = 2D * Math.Tan(PI * (f4 / fs))



                        Try

                            ReDim ACoefficients(6)
                            ReDim BCoefficients(6)

                            ACoefficients(0) = (16 * W4) + (16 * W1) + (4 * W1 ^ 2) + (16 * W4 * W1) + (4 * W4 * W1 ^ 2) + (W4 ^ 2 * W1 ^ 2) + (4 * W4 ^ 2) + 16 + (4 * W4 ^ 2 * W1)
                            ACoefficients(1) = (8 * W4 ^ 2) - 32 + (6 * W4 ^ 2 * W1 ^ 2) + (32 * W4 * W1) + (16 * W4 * W1 ^ 2) + (8 * W1 ^ 2) + (16 * W4 ^ 2 * W1)
                            ACoefficients(2) = -16 + (20 * W4 * W1 ^ 2) - (48 * W4) - (4 * W4 ^ 2) - (4 * W1 ^ 2) + (15 * W4 ^ 2 * W1 ^ 2) - (48 * W1) + (20 * W4 ^ 2 * W1) - (16 * W4 * W1)
                            ACoefficients(3) = 64 - (16 * W1 ^ 2) - (64 * W4 * W1) + (20 * W4 ^ 2 * W1 ^ 2) - (16 * W4 ^ 2)
                            ACoefficients(4) = -(4 * W1 ^ 2) + (15 * W4 ^ 2 * W1 ^ 2) - (4 * W4 ^ 2) + (48 * W1) - (16 * W4 * W1) - (20 * W4 * W1 ^ 2) - 16 - (20 * W4 ^ 2 * W1) + (48 * W4)
                            ACoefficients(5) = (8 * W4 ^ 2) - (16 * W4 ^ 2 * W1) + (8 * W1 ^ 2) + (32 * W4 * W1) - (16 * W4 * W1 ^ 2) + (6 * W4 ^ 2 * W1 ^ 2) - 32
                            ACoefficients(6) = -(4 * W4 * W1 ^ 2) + (16 * W4 * W1) + (W4 ^ 2 * W1 ^ 2) - (4 * W4 ^ 2 * W1) + 16 - (16 * W1) - (16 * W4) + (4 * W4 ^ 2) + (4 * W1 ^ 2)
                            BCoefficients(0) = 4 * W4 ^ 2
                            BCoefficients(1) = 8 * W4 ^ 2
                            BCoefficients(2) = -4 * W4 ^ 2
                            BCoefficients(3) = -16 * W4 ^ 2
                            BCoefficients(4) = -4 * W4 ^ 2
                            BCoefficients(5) = 8 * W4 ^ 2
                            BCoefficients(6) = 4 * W4 ^ 2
                            'Gain = 10 ^ (-0.062 / 20)
                            GainIn_dB = 0.062

                            'case ITU

                            Return True
                        Catch ex As Exception
                            Return False
                        End Try

                    Case FrequencyWeightings.RLB
                        ReDim ACoefficients(2)
                        ReDim BCoefficients(2)

                        ACoefficients(0) = 1.0
                        ACoefficients(1) = -1.99004745483398
                        ACoefficients(2) = 0.99007225036621
                        BCoefficients(0) = 1.0
                        BCoefficients(1) = -2.0
                        BCoefficients(2) = 1.0

                        Return True

                    Case Else
                        Throw New NotImplementedException("Non implemented frequency weigthing.")

                End Select

            End Function


            ''' <summary>
            ''' Concatenates the input sounds. All input sounds must have the same format. They may however differ in number of channels, in which case the output file will contain the lowest number of channels among the input sounds. Data in channels higher than the lowest channel count will be ignored.
            ''' </summary>
            ''' <param name="InputSounds">A list of Sound to concatenate.</param>
            ''' <param name="EqualizeSoundLevel">If set to true, all sounds will be set to EqualizationLevel before conatenation.</param>
            ''' <param name="EqualizationLevel"></param>
            ''' <param name="EqualizationLevelFrequencyWeighting">The frequency weighting used in the sound level equalization measurement.</param>
            ''' <param name="AllowChangingInputSounds">Set to false to work on copies of the input sounds. This will require more memory use!</param>
            ''' <param name="CrossFadeLength">The length (in sample) of a cross-fade section.</param>
            ''' <returns></returns>
            Public Function ConcatenateSounds(ByRef InputSounds As List(Of Sound),
                                          Optional ByVal EqualizeSoundLevel As Boolean = False,
                                          Optional ByVal EqualizationLevel As Double? = -40,
                                          Optional ByVal EqualizationLevelFrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z,
                                          Optional ByVal AllowChangingInputSounds As Boolean = True,
                                          Optional ByVal CheckForDifferentSoundFormats As Boolean = True,
                                          Optional ByVal CrossFadeLength As Integer? = Nothing,
                                          Optional ByVal SkewedFade As Boolean = False,
                                          Optional ByVal CosinePower As Double = 10,
                                          Optional ByVal EqualPower As Boolean = True) As Sound

                Try

                    'Returning nothing if there are no input sounds
                    If InputSounds.Count = 0 Then Return Nothing

                    If CheckForDifferentSoundFormats = True Then
                        If InputSounds.Count > 1 Then
                            For n = 1 To InputSounds.Count - 1
                                If InputSounds(n).WaveFormat.SampleRate <> InputSounds(n - 1).WaveFormat.SampleRate Or
                                    InputSounds(n).WaveFormat.BitDepth <> InputSounds(n - 1).WaveFormat.BitDepth Or
                                    InputSounds(n).WaveFormat.Encoding <> InputSounds(n - 1).WaveFormat.Encoding Then
                                    Throw New ArgumentException("Different formats in ConcatenateSounds input sounds. Aborting!")
                                End If
                            Next
                        End If
                    End If

                    'Getting the length of the output sound
                    Dim TotalLength As Long = 0
                    For Each InputSound In InputSounds
                        'Getting length from channel 1 only
                        If CrossFadeLength Is Nothing Then
                            TotalLength += InputSound.WaveData.SampleData(1).Length
                        Else
                            TotalLength += InputSound.WaveData.SampleData(1).Length - CrossFadeLength
                        End If
                    Next

                    'Adjusting the length derived when crossfading, since the last sound is not faded
                    If CrossFadeLength IsNot Nothing Then
                        TotalLength += CrossFadeLength
                    End If


                    'Getting the lowest number of channels
                    Dim LowestNumberOfChannels As Integer = InputSounds(0).WaveFormat.Channels
                    For n = 1 To InputSounds.Count - 1
                        LowestNumberOfChannels = Math.Min(InputSounds(n).WaveFormat.Channels, LowestNumberOfChannels)
                    Next

                    'Creates copies of the input sounds if needed
                    Dim SoundsToUse As List(Of Sound)
                    If EqualizeSoundLevel = True And AllowChangingInputSounds = False Then
                        SoundsToUse = New List(Of Sound)
                        For Each InputSound In InputSounds
                            SoundsToUse.Add(InputSound.CreateCopy)
                        Next
                    Else
                        SoundsToUse = InputSounds
                    End If

                    'Setting the sound level of each sound to TargetLevel
                    Dim DistoredSamples As Double = 0
                    If EqualizeSoundLevel = True Then
                        'Setting all input sounds to EqualizationLevel
                        For Each InputSound In SoundsToUse
                            DistoredSamples += DSP.MeasureAndAdjustSectionLevel(InputSound, EqualizationLevel,,,, EqualizationLevelFrequencyWeighting)
                        Next
                    End If


                    'Creating an output sound
                    Dim OutputSoundWaveFormat As New Formats.WaveFormat(SoundsToUse(0).WaveFormat.SampleRate,
                                                                      SoundsToUse(0).WaveFormat.BitDepth,
                                                                      LowestNumberOfChannels,, SoundsToUse(0).WaveFormat.Encoding)
                    Dim OutputSound As New Sound(OutputSoundWaveFormat)
                    'Creating sample arrays for each channel
                    For c = 1 To OutputSound.WaveFormat.Channels
                        Dim NewChannelArray(TotalLength - 1) As Single
                        OutputSound.WaveData.SampleData(c) = NewChannelArray
                    Next

                    'Concatenating the sounds
                    If CrossFadeLength Is Nothing Then

                        Dim WriteSampleIndex As Long = 0
                        For Each InputSound In SoundsToUse
                            For s = 0 To InputSound.WaveData.SampleData(1).Length - 1 'Using channel 1 lengths only
                                For c = 1 To OutputSound.WaveFormat.Channels
                                    OutputSound.WaveData.SampleData(c)(WriteSampleIndex) = InputSound.WaveData.SampleData(c)(s)
                                Next
                                'Increasing WriteSampleIndex after all channels have been copied
                                WriteSampleIndex += 1
                            Next
                        Next
                    Else

                        Dim WriteSampleIndex As Long = 0
                        For InputSoundIndex = 0 To SoundsToUse.Count - 1

                            Dim InputSound = SoundsToUse(InputSoundIndex)

                            'Fading beginnings (not of first sound)
                            If InputSoundIndex > 0 Then
                                Dim CrossInFadeSlopeType As FadeSlopeType = FadeSlopeType.Linear
                                If SkewedFade = True Then CrossInFadeSlopeType = FadeSlopeType.PowerCosine_SkewedInFadeDirection
                                DSP.Fade(InputSound, Nothing, 0,, 0, CrossFadeLength, CrossInFadeSlopeType, CosinePower, EqualPower)
                            End If

                            'Fading ends (not of last sound)
                            If InputSoundIndex < SoundsToUse.Count - 1 Then
                                Dim CrossOutFadeSlopeType As FadeSlopeType = FadeSlopeType.Linear
                                If SkewedFade = True Then CrossOutFadeSlopeType = FadeSlopeType.PowerCosine_SkewedFromFadeDirection
                                DSP.Fade(InputSound, 0, Nothing, , InputSound.WaveData.SampleData(1).Length - CrossFadeLength, CrossFadeLength, CrossOutFadeSlopeType, CosinePower, EqualPower)
                            End If

                            'Adding the overlapping sounds into the output arrays
                            For s = 0 To InputSound.WaveData.SampleData(1).Length - 1 'Using channel 1 lengths only

                                'Skipping to next sample if WriteSampleIndex is below 0. This happens when concatenating sounds that are shorter than the CrossFadeLength
                                If WriteSampleIndex < 0 Then
                                    'Increasing WriteSampleIndex after all channels have been copied
                                    WriteSampleIndex += 1
                                    Continue For
                                End If

                                For c = 1 To OutputSound.WaveFormat.Channels
                                    OutputSound.WaveData.SampleData(c)(WriteSampleIndex) += InputSound.WaveData.SampleData(c)(s)
                                Next
                                'Increasing WriteSampleIndex after all channels have been copied
                                WriteSampleIndex += 1
                            Next

                            'Moving the write position backwards
                            WriteSampleIndex -= CrossFadeLength
                        Next


                    End If

                    Return OutputSound

                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try

            End Function

            ''' <summary>
            ''' Superpositions the input sounds to a new sound. The lengths and wave format (including channel count) are required to be the same in all input sounds.
            ''' </summary>
            ''' <param name="InputSounds"></param>
            ''' <returns></returns>
            Public Function SuperpositionSounds(ByRef InputSounds As List(Of Sound)) As Sound

                If InputSounds.Count = 0 Then Return Nothing

                Dim WaveFormat = InputSounds(0).WaveFormat
                For i = 1 To InputSounds.Count - 1
                    If WaveFormat.IsEqual(InputSounds(i).WaveFormat) = False Then Throw New ArgumentException("All wave formats need to be the same!")
                Next

                Dim OutputSound As New Sound(WaveFormat)

                For Channel = 1 To WaveFormat.Channels

                    'Checking length equality of each channel
                    Dim ChannelLength As Integer = InputSounds(0).WaveData.SampleData(Channel).Length
                    For i = 1 To InputSounds.Count - 1
                        If InputSounds(i).WaveData.SampleData(Channel).Length <> ChannelLength Then Throw New ArgumentException("All sounds need to have the same sample array lengths (in corresponding channels)!")
                    Next

                    Dim NewChannelArray(ChannelLength - 1) As Single
                    'Copies the first channel
                    Array.Copy(InputSounds(0).WaveData.SampleData(Channel), NewChannelArray, NewChannelArray.Length)

                    'Superpositions the remaining channels
                    For i = 1 To InputSounds.Count - 1
                        Dim CurrentChannelArray = InputSounds(i).WaveData.SampleData(Channel)
                        For s = 0 To ChannelLength - 1
                            NewChannelArray(s) += CurrentChannelArray(s)
                        Next
                    Next

                    OutputSound.WaveData.SampleData(Channel) = NewChannelArray

                Next

                Return OutputSound

            End Function

            ''' <summary>
            ''' Sets the sound level of the indicated section of the indicated sound to a target level.
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="targetLevel"></param>
            ''' <param name="channel"></param>
            ''' <param name="startSample"></param>
            ''' <param name="sectionLength"></param>
            ''' <param name="FrequencyWeighting"></param>
            ''' <returns>Returns the number of distorted samples, or vbNull if something went wrong.</returns>
            Public Function MeasureAndAdjustSectionLevel(ByRef InputSound As Sound, ByVal targetLevel As Decimal, Optional ByVal channel As Integer? = Nothing,
                                        Optional ByVal startSample As Integer? = Nothing, Optional ByVal sectionLength As Integer? = Nothing,
                                                 Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z) As Double


                'Setting default start sample and length values
                If startSample Is Nothing Then startSample = 0
                If sectionLength Is Nothing Then sectionLength = InputSound.WaveData.ShortestChannelSampleCount

                Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, channel)

                Dim totalDistortedSamples As Double = 0

                'Main section
                For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                    'Measures the level of the channel section
                    Dim SectionLevel As Double = MeasureSectionLevel(InputSound, c, startSample, sectionLength,,, FrequencyWeighting)

                    'Calculating the needed gain
                    Dim Gain As Double = targetLevel - SectionLevel

                    'Amplifies the section
                    totalDistortedSamples += AmplifySection(InputSound, Gain, c, startSample, sectionLength,)

                Next

                Return totalDistortedSamples

            End Function



            Public Enum FadeType
                Gradual
                FadeOut
                FadeIn
                StartLevelToSilence
                SilenceToEndLevel
                SilenceWholeSection
            End Enum

            Public Enum FadeSlopeType
                Smooth
                Linear
                PowerCosine_SkewedInFadeDirection
                PowerCosine_SkewedFromFadeDirection
            End Enum

            Public Class FadeSpecifications

                Public ReadOnly StartAttenuation As Double?
                Public ReadOnly EndAttenuation As Double?
                Public ReadOnly StartSample As Integer
                Public ReadOnly SectionLength As Integer?
                Public ReadOnly SlopeType As FadeSlopeType
                Public ReadOnly CosinePower As Double
                Public ReadOnly EqualPower As Boolean

                ''' <summary>
                ''' Creates a new instance of FadeSpecifications
                ''' </summary>
                ''' <param name="StartAttenuation">The attenuation (in dB) applied in the start of the fade period. I left empty, fade will start from silence.</param>
                ''' <param name="EndAttenuation">The attenuation (in dB) applied in the end of the fade period. I left empty, the fade period will end in silence.</param>
                ''' <param name="startSample">The start sample of the section to fade.</param>
                ''' <param name="sectionLength">The length (in samples) of the section to fade. If left to Nothing, fading will go to the end of the shortest sound channel.</param>
                ''' <param name="slopeType">Specifies the curvature of the fade section. Linear creates a linear fade, and Smooth fades using a cosine function to smoothen out the fade section.</param>
                Public Sub New(Optional ByVal StartAttenuation As Double? = Nothing, Optional ByVal EndAttenuation As Double? = Nothing,
                            Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing,
                            Optional ByVal SlopeType As FadeSlopeType = FadeSlopeType.Smooth, Optional CosinePower As Double = 10, Optional ByVal EqualPower As Boolean = False)

                    Me.StartAttenuation = StartAttenuation
                    Me.EndAttenuation = EndAttenuation
                    Me.StartSample = StartSample
                    Me.SectionLength = SectionLength
                    Me.SlopeType = SlopeType
                    Me.CosinePower = CosinePower
                    Me.EqualPower = EqualPower

                End Sub

            End Class

            ''' <summary>
            ''' Fading the indicated section of the indicated sound using the specified fading type.
            ''' </summary>
            ''' <param name="InputSound">The sound to be modified.</param>
            ''' <param name="FadeSpecifications"></param>
            ''' <param name="Channel">The channel to be modified. If left empty all channels will be modified.</param>
            Public Sub Fade(ByRef InputSound As Sound, ByVal FadeSpecifications As FadeSpecifications, Optional ByVal Channel As Integer? = Nothing)

                Fade(InputSound, FadeSpecifications.StartAttenuation, FadeSpecifications.EndAttenuation, Channel, FadeSpecifications.StartSample, FadeSpecifications.SectionLength, FadeSpecifications.SlopeType, FadeSpecifications.CosinePower, FadeSpecifications.EqualPower)

            End Sub



            ''' <summary>
            ''' Fading the indicated section of the indicated sound using the specified fading type.
            ''' </summary>
            ''' <param name="input">The sound to be modified.</param>
            ''' <param name="StartAttenuation">The attenuation (in dB) applied in the start of the fade period. I left empty, fade will start from silence.</param>
            ''' <param name="EndAttenuation">The attenuation (in dB) applied in the end of the fade period. I left empty, the fade period will end in silence.</param>
            ''' <param name="channel">The channel to be modified. If left empty all channels will be modified.</param>
            ''' <param name="startSample">The start sample of the section to fade.</param>
            ''' <param name="sectionLength">The length (in samples) of the section to fade. If left to Nothing, fading will go to the end of the shortest sound channel.</param>
            ''' <param name="slopeType">Specifies the curvature of the fade section. Linear creates a linear fade, and Smooth fades using a cosine function to smoothen out the fade section.</param>
            Public Sub Fade(ByRef input As Sound, Optional ByVal StartAttenuation As Double? = Nothing, Optional ByVal EndAttenuation As Double? = Nothing,
                            Optional ByVal channel As Integer? = Nothing,
                            Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing,
                            Optional ByVal slopeType As FadeSlopeType = FadeSlopeType.Smooth,
                            Optional CosinePower As Double = 10,
                        Optional ByVal EqualPower As Boolean = False)

                'Ska uppdateras med kurvtyp, linjär ska läggas till som val
                'Även "micro255 law" ska läggas till, Equation 22-1 i dspguide.com: y=(ln(1+0.000001*x))/(ln(1+0.000001)) ' Skapar en naturlig fadening genom "compounding"
                'även en alternativ ekvation finns "A Law" (Stämmer detta. Borde nog byta input och output, jämfört med grafen i boken)

                Try

                    Dim AudioOutputConstructor As New AudioOutputConstructor(input.WaveFormat, channel)

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim inputArray() As Single = input.WaveData.SampleData(c)
                        'CheckAndCorrectSectionLength(inputArray.Length, startSample, sectionLength)

                        'Main section
                        Dim startFactor As Double
                        Dim endFactor As Double

                        'Setting start factor
                        If StartAttenuation Is Nothing Then
                            startFactor = 0
                        Else
                            startFactor = 10 ^ (-StartAttenuation / 20) 'the minus sign converts attenuation to gain 

                            'Modifying the startFactor to account for the sqrt applied later
                            If EqualPower = True Then
                                startFactor = startFactor ^ 2
                            End If

                        End If

                        'Setting end factor
                        If EndAttenuation Is Nothing Then
                            endFactor = 0
                        Else
                            endFactor = 10 ^ (-EndAttenuation / 20) 'the minus sign converts attenuation to gain

                            'Modifying the startFactor to account for the sqrt applied later
                            If EqualPower = True Then
                                endFactor = endFactor ^ 2
                            End If

                        End If


                        Dim fadeSampleCount As Integer = 0
                        Dim fadeProgress As Double = 0

                        Dim distorsion As Boolean = False
                        Dim distorsionSampleCount As Integer = 0

                        Dim IsFadeIn As Boolean = False
                        If startFactor < endFactor Then IsFadeIn = True

                        Dim CorrectedStartSample = startSample
                        Dim CorrectedSectionLength = sectionLength
                        CheckAndCorrectSectionLength(inputArray.Length, CorrectedStartSample, CorrectedSectionLength)

                        If CorrectedSectionLength > 1 Then ' To avoid division by zero some lines below

                            For currentSample = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1

                                'fadeProgress goes from 0 to 1 during the fade section
                                fadeProgress = fadeSampleCount / (CorrectedSectionLength - 1)

                                'Modifies currentFadeFactor according to a cosine finction, whereby currentModFactor starts on 1 and end at 0
                                Dim currentModFactor As Double
                                Dim currentFadeFactor As Double
                                Select Case slopeType
                                    Case FadeSlopeType.Smooth
                                        currentModFactor = ((Math.Cos(twopi * (fadeProgress / 2)) + 1) / 2)
                                        currentFadeFactor = startFactor * currentModFactor + endFactor * (1 - currentModFactor)

                                    Case FadeSlopeType.PowerCosine_SkewedFromFadeDirection
                                        If IsFadeIn = True Then
                                            currentModFactor = ((Math.Cos(twopi * ((1 - fadeProgress) / 2)) + 1) / 2) ^ CosinePower
                                            currentFadeFactor = (startFactor * (1 - currentModFactor) + endFactor * currentModFactor)
                                        Else
                                            currentModFactor = ((Math.Cos(twopi * (fadeProgress / 2)) + 1) / 2) ^ CosinePower
                                            currentFadeFactor = (startFactor * currentModFactor) + endFactor * (1 - currentModFactor)
                                        End If

                                    Case FadeSlopeType.PowerCosine_SkewedInFadeDirection
                                        If IsFadeIn = True Then
                                            currentModFactor = ((Math.Cos(twopi * ((fadeProgress) / 2)) + 1) / 2) ^ CosinePower
                                            currentFadeFactor = (startFactor * (currentModFactor) + endFactor * (1 - currentModFactor))
                                        Else
                                            currentModFactor = ((Math.Cos(twopi * ((1 - fadeProgress) / 2)) + 1) / 2) ^ CosinePower
                                            currentFadeFactor = (startFactor * (1 - currentModFactor)) + endFactor * (currentModFactor)
                                        End If

                                    Case Else 'I.e. Linear!
                                        currentFadeFactor = startFactor * (1 - fadeProgress) + endFactor * fadeProgress
                                End Select

                                If EqualPower = True Then
                                    currentFadeFactor = Math.Sqrt(currentFadeFactor)
                                End If

                                Try


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

                                Catch ex As Exception
                                    AudioError(ex.ToString)
                                End Try

                                fadeSampleCount += 1

                            Next

                        End If
                        If distorsion = True Then AudioError("Distorsion occurred for " & distorsionSampleCount & " samples in channel " & c & " in Fade")

                    Next

                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try


            End Sub


            ''' <summary>
            ''' Removes the DC component from a sound by subtracting the average sample value from each sample.
            ''' </summary>
            ''' <param name="InputSound">The sound to modify.</param>
            ''' <param name="Channel">The channel to modify, or Nothing to modify all channels.</param>
            Public Sub RemoveDcComponent(ByRef InputSound As Sound, Optional ByVal Channel As Integer? = Nothing)

                Dim FirstChannel As Integer? = Channel
                Dim LastChannel As Integer? = Channel

                If Channel Is Nothing Then
                    FirstChannel = 1
                    LastChannel = InputSound.WaveFormat.Channels
                End If

                'Checking channel values
                If FirstChannel < 0 Then Throw New ArgumentException("Channel cannot be lower than 1.")
                If LastChannel > InputSound.WaveFormat.Channels Then Throw New ArgumentException("Channel cannot be higher than the available channel count.")

                For c = FirstChannel To LastChannel

                    'Referencing the current sample array
                    Dim ChannelArray = InputSound.WaveData.SampleData(c)

                    'Calculating the average sample value
                    Dim AverageSampleValue As Single = ChannelArray.Average

                    'Skipping modification if the average sample value is zero
                    If AverageSampleValue <> 0 Then
                        'Subtracting the average sample value from each sample, to remove the DC component
                        For s = 0 To ChannelArray.Length - 1
                            ChannelArray(s) -= AverageSampleValue
                        Next
                    End If

                Next

            End Sub

        End Module

    End Namespace

    Namespace GenerateSound


        Public Module Signals


            Public Function CreateDeltaPulse(ByRef format As Formats.WaveFormat, Optional ByVal channel As Integer? = Nothing,
                                    Optional ByVal level As Double = 1, Optional ByVal duration As Double = 1,
                                         Optional durationTimeUnit As TimeUnits = TimeUnits.seconds) As Sound

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

                    Dim rnd As New Random

                    'Main section
                    Select Case format.Encoding
                        Case Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints

                            For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                                Dim channelArray(dataLength - 1) As Single

                                Select Case format.BitDepth
                                    Case 32

                                        channelArray(0) = level
                                        For n = 1 To channelArray.Length - 1
                                            channelArray(n) = 0
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
                                        channelArray(0) = level * Short.MaxValue

                                    Case 32
                                        channelArray(0) = level * Integer.MaxValue

                                End Select

                                For n = 1 To channelArray.Length - 1
                                    channelArray(n) = 0
                                Next


                                outputSound.WaveData.SampleData(c) = channelArray

                            Next

                    End Select

                    Return outputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function

            Public Function CreateSilence(ByRef format As Formats.WaveFormat, Optional ByVal channel As Integer? = Nothing,
                                     Optional ByVal duration As Double = 1,
                                         Optional durationTimeUnit As TimeUnits = TimeUnits.seconds) As Sound

                Try

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

                    Dim rnd As New Random

                    'Main section

                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim channelArray(dataLength - 1) As Single
                        For n = 1 To channelArray.Length - 1
                            channelArray(n) = 0
                        Next
                        outputSound.WaveData.SampleData(c) = channelArray

                    Next

                    Return outputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function


            Public Function CreateWhiteNoise(ByRef format As Formats.WaveFormat, Optional ByVal channel As Integer? = Nothing,
                                    Optional ByVal level As Double = 1, Optional ByVal duration As Double = 1,
                                         Optional durationTimeUnit As TimeUnits = TimeUnits.seconds,
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

                    'Main section
                    Select Case format.Encoding
                        Case Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints

                            For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                                Dim channelArray(dataLength - 1) As Single

                                Select Case format.BitDepth
                                    Case 32

                                        For n = 0 To channelArray.Length - 1
                                            channelArray(n) = (level * (2 * (RandomSource.NextDouble() - 0.5)))
                                        Next

                                    Case Else
                                        Throw New NotImplementedException

                                End Select

                                outputSound.WaveData.SampleData(c) = channelArray

                            Next


                        Case Formats.WaveFormat.WaveFormatEncodings.PCM

                            For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                                Dim channelArray(dataLength - 1) As Single

                                For n = 0 To channelArray.Length - 1
                                    channelArray(n) = level * RandomSource.Next(negFS, posFS)
                                Next

                                outputSound.WaveData.SampleData(c) = channelArray

                            Next

                    End Select

                    Return outputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function


            ''' <summary>
            ''' Creates a new sound containing a sine wave.
            ''' </summary>
            ''' <param name="format"></param>
            ''' <param name="channel"></param>
            ''' <param name="freq">Specified in Hz</param>
            ''' <param name="intensity"></param>
            ''' <param name="intensityUnit"></param>
            ''' <param name="duration"></param>
            ''' <param name="durationTimeUnit"></param>
            ''' <param name="Phase">Specified in radians.</param>
            ''' <returns></returns>
            Public Function CreateSineWave(ByRef format As Formats.WaveFormat, Optional ByVal channel As Integer? = Nothing,
                                    Optional ByVal freq As Double = 1000, Optional ByVal intensity As Decimal = 1,
                                       Optional intensityUnit As SoundDataUnit = SoundDataUnit.unity,
                                       Optional ByVal duration As Double = 1, Optional durationTimeUnit As TimeUnits = TimeUnits.seconds,
                                       Optional ByVal Phase As Double = 0) As Sound
                Try




                    Dim outputSound As New Sound(format)
                    Dim AudioOutputConstructor As New AudioOutputConstructor(format, channel)


                    'Checking valid input intensity values
                    Select Case intensityUnit
                        Case SoundDataUnit.unity
                            If intensity > 1 Then
                                intensity = 1
                                MsgBox("Level was outside allowed value (0 through 1)" & vbCr & vbCr & "The level was adjusted to 1",, "CreateSineWave")
                            End If

                            If intensity < 0 Then
                                intensity = 1
                                MsgBox("Level was outside allowed value (0 through 1)" & vbCr & vbCr & "The level was adjusted to 1",, "CreateSineWave")
                            End If
                        Case SoundDataUnit.dB
                            If intensity > 0 Then
                                intensity = 0
                                MsgBox("Level was above allowed max value (0 dBFS)" & vbCr & vbCr & "The level was adjusted To 0 dBFS",, "CreateSineWave")
                            End If
                        Case SoundDataUnit.linear
                            If intensity > format.PositiveFullScale Then
                                intensity = format.PositiveFullScale
                                MsgBox("Level was outside allowed value (0 through " & format.PositiveFullScale & ")" & vbCr & vbCr & "The level was adjusted To " & format.PositiveFullScale,, "Waring from createSineWave")
                            End If

                            If intensity < 0 Then
                                intensity = format.PositiveFullScale
                                MsgBox("Level was outside allowed value (0 through " & format.PositiveFullScale & ")" & vbCr & vbCr & "The level was adjusted To " & format.PositiveFullScale,, "CreateSineWave")
                            End If
                    End Select

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

                    'Main section
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim channelArray(dataLength - 1) As Single

                        Select Case format.BitDepth
                            Case 8 'Actually used for formats that use unsigned datatypes
                                For n = 0 To channelArray.Length - 1
                                    channelArray(n) = (intensity * (format.PositiveFullScale / 2)) * Math.Sin(Phase + twopi * (freq / format.SampleRate) * n) + format.PositiveFullScale / 2 ' - _
                                Next

                            Case 16, 32, 64 'Actually used for formats that use signed datatypes
                                For n = 0 To channelArray.Length - 1
                                    channelArray(n) = (intensity * format.PositiveFullScale) * Math.Sin(Phase + twopi * (freq / format.SampleRate) * n) ' - _
                                Next
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
            ''' Creates sound containing a frequency modulated (or more correctly phase moduated) sine wave.
            ''' </summary>
            ''' <param name="format"></param>
            ''' <param name="channel"></param>
            ''' <param name="CarrierFrequency"></param>
            ''' <param name="intensity"></param>
            ''' <param name="ModulationFrequency"></param>
            ''' <param name="ModulationDepth">Modulating depth as a proportion of the carrier frequency.</param>
            ''' <param name="intensityUnit"></param>
            ''' <param name="duration"></param>
            ''' <param name="durationTimeUnit"></param>
            ''' <returns></returns>
            Public Function CreateFrequencyModulatedSineWave(ByRef format As Formats.WaveFormat, Optional ByVal channel As Integer? = Nothing,
                                                         Optional ByVal CarrierFrequency As Double = 1000, Optional ByVal intensity As Decimal = 1,
                                                         Optional ByVal ModulationFrequency As Double = 10, Optional ByVal ModulationDepth As Double = 0.1,
                                                         Optional intensityUnit As SoundDataUnit = SoundDataUnit.unity,
                                                         Optional ByVal duration As Double = 1, Optional durationTimeUnit As TimeUnits = TimeUnits.seconds) As Sound

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


                    Dim outputSound As New Sound(format)
                    Dim AudioOutputConstructor As New AudioOutputConstructor(format, channel)


                    'Checking valid input intensity values
                    Select Case intensityUnit
                        Case SoundDataUnit.unity
                            If intensity > 1 Then
                                intensity = 1
                                MsgBox("Level was outside allowed value (0 through 1)" & vbCr & vbCr & "The level was adjusted to 1",, "CreateSineWave")
                            End If

                            If intensity < 0 Then
                                intensity = 1
                                MsgBox("Level was outside allowed value (0 through 1)" & vbCr & vbCr & "The level was adjusted to 1",, "CreateSineWave")
                            End If
                        Case SoundDataUnit.dB
                            If intensity > 0 Then
                                intensity = 0
                                MsgBox("Level was above allowed max value (0 dBFS)" & vbCr & vbCr & "The level was adjusted To 0 dBFS",, "CreateSineWave")
                            End If
                        Case SoundDataUnit.linear
                            If intensity > format.PositiveFullScale Then
                                intensity = format.PositiveFullScale
                                MsgBox("Level was outside allowed value (0 through " & format.PositiveFullScale & ")" & vbCr & vbCr & "The level was adjusted To " & format.PositiveFullScale,, "Waring from createSineWave")
                            End If

                            If intensity < 0 Then
                                intensity = format.PositiveFullScale
                                MsgBox("Level was outside allowed value (0 through " & format.PositiveFullScale & ")" & vbCr & vbCr & "The level was adjusted To " & format.PositiveFullScale,, "CreateSineWave")
                            End If
                    End Select

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

                    'Calculating the maximum frequency deviation
                    Dim MaximumFrequencyDeviation As Double = CarrierFrequency * ModulationDepth

                    'Main section
                    Dim CurrentFrequency As Double = 0
                    For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                        Dim channelArray(dataLength - 1) As Single

                        Select Case format.BitDepth
                            Case 8 'Actually used for formats that use unsigned datatypes
                                For n = 0 To channelArray.Length - 1

                                    channelArray(n) = (intensity * (format.PositiveFullScale / 2)) * Math.Sin(twopi * (CurrentFrequency / format.SampleRate) * n) + format.PositiveFullScale / 2 ' - _

                                Next

                            Case 16, 32, 64 'Actually used for formats that use signed datatypes
                                For n = 0 To channelArray.Length - 1

                                    'Intuitively for carrier frequency:
                                    'channelArray(n) = (intensity * format.PositiveFullScale) * Math.Sin("One lap" * "number of laps in a second" * "the current time (in seconds)")
                                    'channelArray(n) = (intensity * format.PositiveFullScale) * Math.Sin(twopi * CurrentFrequency * (n / format.SampleRate)) ' - _


                                    'Applying modulation by varying the phase (Actually I did this by trial and error, I'm not sure why the factor (twopi*Pi*0.1) is needed (+/-0.1 is the modulator range))
                                    'channelArray(n) = (intensity * format.PositiveFullScale) * Math.Sin(twopi * CarrierFrequency * (n / format.SampleRate) - (twopi * Math.PI * ModulationFrequencyDeviation * ModulationWaveForm.WaveData.SampleData(c)(n)))

                                    'x(t) = Ac *Cos [wc*t + fc + Am*Sin(wmt +fm)]
                                    channelArray(n) = (intensity * format.PositiveFullScale) * Math.Cos(twopi * CarrierFrequency * (n / format.SampleRate) + 0 +
                                                                                                    (MaximumFrequencyDeviation / ModulationFrequency) * Math.Sin(twopi * ModulationFrequency * (n / format.SampleRate) + 0))


                                Next
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


        End Module



    End Namespace

End Namespace