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

        Public Module Measurements


            ''' <summary>
            ''' Measures the sound level of a section of the input sound.
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="channel"></param>
            ''' <param name="StartSample"></param>
            ''' <param name="SectionLength"></param>
            ''' <param name="outputUnit"></param>
            ''' <param name="SoundMeasurementType"></param>
            ''' <param name="Frequencyweighting">The frequency Weighting to be applied before the sound measurement.</param>
            ''' <param name="ReturnLinearMeanSquareData">If set to true, the linear mean square of the measured section is returned (I.e. Any values set for outputUnit and SoundMeasurementType are overridden.).</param>
            ''' <param name="LinearSquareData">If ReturnLinearMeanSquareData is set to True, LinearSquareData will contain item1 = linear sum of square, and item2 = length of the measurement section in samples.</param>
            ''' <returns></returns>
            Public Function MeasureSectionLevel(ByRef InputSound As Sound, ByVal channel As Integer,
                                            Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing,
                                            Optional ByVal outputUnit As SoundDataUnit = SoundDataUnit.dB,
                                            Optional ByVal SoundMeasurementType As SoundMeasurementType = SoundMeasurementType.RMS,
                                            Optional ByVal Frequencyweighting As FrequencyWeightings = FrequencyWeightings.Z,
                                            Optional ByVal ReturnLinearMeanSquareData As Boolean = False,
                                            Optional ByRef LinearSquareData As Tuple(Of Double, Integer) = Nothing) As Double?

                Try

                    CheckAndCorrectSectionLength(InputSound.WaveData.SampleData(channel).Length, StartSample, SectionLength)

                    'Preparing an array to do measurements on
                    Dim MeasurementArray() As Single

                    If Frequencyweighting = FrequencyWeightings.Z Then
                        'Just referencing the input sound array
                        MeasurementArray = InputSound.WaveData.SampleData(channel)

                    Else
                        'Preparing a new sound with only the measurement section, to be filterred
                        Dim tempSound As New Sound(InputSound.WaveFormat)
                        ReDim MeasurementArray(SectionLength - 1)
                        For s = StartSample To StartSample + SectionLength - 1
                            MeasurementArray(s - StartSample) = InputSound.WaveData.SampleData(channel)(s)
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
                        StartSample = 0

                    End If

                    'Overrides soundmeasurmenttype
                    If ReturnLinearMeanSquareData = True Then SoundMeasurementType = SoundMeasurementType.RMS

                    Select Case SoundMeasurementType
                        Case SoundMeasurementType.RMS

                            'Calculates RMS value of the section

                            Dim RMS As Double? = Nothing

                            Dim SumOfSquare As Double = 0

                            SumOfSquare = Utils.CalculateSumOfSquare(MeasurementArray, StartSample, SectionLength)

                            ''Calculating the sum of sqares in libostfdsp
                            'SumOfSquare = LibOstfDsp_VB.calculateFloatSumOfSquare(MeasurementArray, MeasurementArray.Length, StartSample, SectionLength)

                            'Returns the mean square (MR) if ReturnLinearMeanSquareData is True
                            If ReturnLinearMeanSquareData = False Then

                                'Calculates RMS
                                RMS = (SumOfSquare / SectionLength) ^ (1 / 2)

                            Else

                                'Stores LinearSquareData
                                LinearSquareData = New Tuple(Of Double, Integer)(SumOfSquare, SectionLength)

                                'Returns the ReturnLinearMeanSquareData
                                Return SumOfSquare / SectionLength

                            End If


                            Select Case outputUnit
                                Case SoundDataUnit.dB
                                    Dim sectionLevel As Double = dBConversion(RMS, dBConversionDirection.to_dB, InputSound.WaveFormat)
                                    Return sectionLevel
                                Case SoundDataUnit.linear
                                    Return RMS
                            End Select

                        Case SoundMeasurementType.AbsolutePeakAmplitude
                            'Calculates the absolute max amplitude of the section

                            Dim LocalArrayCopy(SectionLength - 1) As Single
                            Array.Copy(MeasurementArray, LocalArrayCopy, SectionLength.Value)

                            Dim peak_pos As Double = LocalArrayCopy.Max
                            Dim peak_neg As Double = LocalArrayCopy.Min

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

                            For n = StartSample To StartSample + SectionLength - 1
                                AccumulativeSoundLevel = AccumulativeSoundLevel + Math.Abs(MeasurementArray(n))
                            Next

                            Dim averageAbsoluteAmplitude = AccumulativeSoundLevel / SectionLength

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


        Public Module Transformations

            Public Enum FftDirections
                Forward
                Backward
            End Enum


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
            ''' Amplifys a section of the sound.
            ''' </summary>
            ''' <param name="InputSound">The sound to modify.</param>
            ''' <param name="Gain">The amount of gain applied to the specified section (gain unit is set to dB or linear with the parameter GainUnit).</param>
            ''' <param name="Channel">The channel to be modified. If left empty, all channels will be modified.</param>
            ''' <param name="StartSample">Start sample of the section to be amplified.</param>
            ''' <param name="SectionLength">Length (in samples) of the section to be amplified.</param>
            ''' <param name="GainUnit">The unit of the gain paramameter (dB or linear)</param>
            Public Sub AmplifySection(ByRef InputSound As Sound, ByVal Gain As Double,
                                       Optional ByVal Channel As Integer? = Nothing,
                                       Optional ByVal StartSample As Integer = 0, Optional ByVal SectionLength As Integer? = Nothing,
                                       Optional ByVal GainUnit As SoundDataUnit = SoundDataUnit.dB)

                Try

                    Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, Channel)

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
                        Dim otherErrorExeption As Exception = Nothing

                        Dim SoundArray = InputSound.WaveData.SampleData(c)

                        Dim CorrectedStartSample = StartSample
                        Dim CorrectedSectionLength = SectionLength
                        CheckAndCorrectSectionLength(SoundArray.Length, CorrectedStartSample, CorrectedSectionLength)

                        If OstfBase.UseOptimizationLibraries = False Then
                            Utils.MultiplyArray(SoundArray, gainFactor, CorrectedStartSample, CorrectedSectionLength)

                        Else
                            ''Calling libostfdsp function multiplyFloatArraySection
                            LibOstfDsp_VB.MultiplyArraySection(SoundArray, gainFactor, CorrectedStartSample, CorrectedSectionLength)

                        End If

                    Next


                Catch ex As Exception
                    AudioError(ex.ToString)
                End Try

            End Sub


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
            ''' <param name="ThrowOnUnequalNominalLevels">If True, checks to ensure that all nominal levels stored in the SMA object of each sound are the same (except if they are Nothing, then they are ignored)</param>
            ''' <returns></returns>
            Public Function ConcatenateSounds2(ByRef InputSounds As List(Of Sound),
                                               Optional ByVal CrossFadeLength As Integer? = Nothing,
                                               Optional ByVal SkewedFade As Boolean = False,
                                               Optional ByVal CosinePower As Double = 10,
                                               Optional ByVal EqualPower As Boolean = True,
                                               Optional ByVal CheckForDifferentSoundFormats As Boolean = True,
                                               Optional ByVal ThrowOnUnequalNominalLevels As Boolean = True) As Sound

                Try

                    'Returning nothing if there are no input sounds
                    If InputSounds.Count = 0 Then Return Nothing

                    'Returning a single input sound directly
                    If InputSounds.Count = 1 Then Return InputSounds(0)

                    Dim DetectedNominalLevel As Double? = Nothing
                    Dim NominalLevelList As New List(Of Double)
                    For Each Sound In InputSounds
                        If Sound.SMA IsNot Nothing Then
                            If Sound.SMA.NominalLevel.HasValue Then
                                NominalLevelList.Add(Sound.SMA.NominalLevel.Value)
                                DetectedNominalLevel = Sound.SMA.NominalLevel.Value
                            End If
                        End If
                    Next
                    If ThrowOnUnequalNominalLevels Then
                        If NominalLevelList.Count > 0 Then
                            For i = 0 To NominalLevelList.Count - 2
                                If NominalLevelList(i) <> NominalLevelList(i + 1) Then Throw New Exception("Unequal nominal levels detected in concatenated sound files! This may lead to unexpected sound levels during playback!")
                            Next
                        End If
                    End If

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
                    Dim TotalOutputLength As Long = 0
                    For Each InputSound In InputSounds
                        'Getting length from channel 1 only
                        If CrossFadeLength Is Nothing Then
                            TotalOutputLength += InputSound.WaveData.SampleData(1).Length
                        Else
                            TotalOutputLength += InputSound.WaveData.SampleData(1).Length - CrossFadeLength
                            'Ensuring that TotalOutputLength (and later in the code below, WriteSampleIndex) is never negative
                            TotalOutputLength = System.Math.Max(0, TotalOutputLength)
                        End If
                    Next

                    'Adjusting the length derived when crossfading, since the last sound is not faded
                    If CrossFadeLength IsNot Nothing Then
                        TotalOutputLength += CrossFadeLength
                    End If

                    'Getting the lowest number of channels
                    Dim LowestNumberOfChannels As Integer = InputSounds(0).WaveFormat.Channels
                    For n = 1 To InputSounds.Count - 1
                        LowestNumberOfChannels = Math.Min(InputSounds(n).WaveFormat.Channels, LowestNumberOfChannels)
                    Next

                    'Creating an output sound format
                    Dim OutputSoundWaveFormat As New Formats.WaveFormat(InputSounds(0).WaveFormat.SampleRate,
                                                                      InputSounds(0).WaveFormat.BitDepth,
                                                                      LowestNumberOfChannels,, InputSounds(0).WaveFormat.Encoding)

                    'Creating an output sound
                    Dim OutputSound As New Sound(OutputSoundWaveFormat)

                    'Concatenating the sounds
                    If CrossFadeLength Is Nothing Then

                        'Creating sample arrays for each channel
                        For c = 1 To OutputSound.WaveFormat.Channels
                            Dim NewChannelArray(TotalOutputLength - 1) As Single
                            OutputSound.WaveData.SampleData(c) = NewChannelArray
                        Next

                        Dim WriteSampleIndex As Long = 0
                        For Each InputSound In InputSounds

                            For c = 1 To OutputSound.WaveFormat.Channels
                                Dim TargetArray = OutputSound.WaveData.SampleData(c)
                                Dim SourceArray = InputSound.WaveData.SampleData(c)
                                Array.Copy(SourceArray, 0, TargetArray, WriteSampleIndex, SourceArray.Length)
                            Next

                            'Increasing WriteSampleIndex after all channels have been copied, based on the channel 1 length (basicaly requires equal channel lengths in each incoming sound)
                            WriteSampleIndex += InputSound.WaveData.SampleData(1).Length

                        Next

                    Else

                        'Fading input sounds
                        For InputSoundIndex = 0 To InputSounds.Count - 1

                            Dim InputSound = InputSounds(InputSoundIndex)

                            'Fading beginnings (not of first sound)
                            If InputSoundIndex > 0 Then
                                Dim CrossInFadeSlopeType As FadeSlopeType = FadeSlopeType.Linear
                                If SkewedFade = True Then CrossInFadeSlopeType = FadeSlopeType.PowerCosine_SkewedInFadeDirection
                                DSP.Fade(InputSound, Nothing, 0,, 0, CrossFadeLength, CrossInFadeSlopeType, CosinePower, EqualPower)
                            End If

                            'Fading ends (not of last sound)
                            If InputSoundIndex < InputSounds.Count - 1 Then
                                Dim CrossOutFadeSlopeType As FadeSlopeType = FadeSlopeType.Linear
                                If SkewedFade = True Then CrossOutFadeSlopeType = FadeSlopeType.PowerCosine_SkewedFromFadeDirection
                                DSP.Fade(InputSound, 0, Nothing, , InputSound.WaveData.SampleData(1).Length - CrossFadeLength, CrossFadeLength, CrossOutFadeSlopeType, CosinePower, EqualPower)
                            End If

                        Next

                        'Copying the input sounds to their intended position in the output sound
                        Dim WriteSampleIndex As Long = 0
                        Dim OutputSoundLayers As New List(Of SortedList(Of Integer, Single()))
                        For i = 0 To InputSounds.Count - 1
                            OutputSoundLayers.Add(New SortedList(Of Integer, Single()))
                            For c = 1 To OutputSound.WaveFormat.Channels
                                'Creating an empty array
                                Dim OutputSoundLayerChannelArray(TotalOutputLength - 1) As Single

                                'Getting the sound in channel c
                                Dim SourceArray = InputSounds(i).WaveData.SampleData(c)

                                'Copying the sound to the OutputSoundLayerChannelArray, from the intended position (WriteSampleIndex)
                                Array.Copy(SourceArray, 0, OutputSoundLayerChannelArray, WriteSampleIndex, SourceArray.Length)

                                'Adding the OutputSoundLayerChannelArray to OutputSoundLayers
                                OutputSoundLayers(i).Add(c, OutputSoundLayerChannelArray)

                            Next

                            'Increasing WriteSampleIndex after all channels have been copied, based on the channel 1 length (basicaly requires equal channel lengths in each incoming sound)
                            WriteSampleIndex += InputSounds(i).WaveData.SampleData(1).Length

                            'Moving the write position backwards
                            WriteSampleIndex -= CrossFadeLength

                            'Ensuring that WriteSampleIndex is non-negative
                            WriteSampleIndex = System.Math.Max(0, WriteSampleIndex)

                        Next

                        'Superpositioning the channel-layer arrays
                        For c = 1 To OutputSound.WaveFormat.Channels
                            Dim OutputSoundChannelArray = OutputSoundLayers(0)(c)
                            For i = 1 To InputSounds.Count - 1

                                Utils.AddTwoArrays(OutputSoundChannelArray, OutputSoundLayers(i)(c))

                            Next
                            OutputSound.WaveData.SampleData(c) = OutputSoundChannelArray
                        Next

                    End If

                    If DetectedNominalLevel.HasValue Then
                        OutputSound.SMA.NominalLevel = DetectedNominalLevel.Value
                    End If

                    Return OutputSound

                Catch ex As Exception
                    AudioError(ex.ToString)
                    Return Nothing
                End Try

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
            ''' <param name="ThrowOnUnequalNominalLevels">If True, checks to ensure that all nominal levels stored in the SMA object of each sound are the same (except if they are Nothing, then they are ignored)</param>
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
                                          Optional ByVal EqualPower As Boolean = True,
                                              Optional ByVal ThrowOnUnequalNominalLevels As Boolean = True) As Sound

                Try

                    Dim DetectedNominalLevel As Double? = Nothing
                    Dim NominalLevelList As New List(Of Double)
                    For Each Sound In InputSounds
                        If Sound.SMA IsNot Nothing Then
                            If Sound.SMA.NominalLevel.HasValue Then
                                NominalLevelList.Add(Sound.SMA.NominalLevel.Value)
                                DetectedNominalLevel = Sound.SMA.NominalLevel.Value
                            End If
                        End If
                    Next
                    If ThrowOnUnequalNominalLevels Then
                        If NominalLevelList.Count > 0 Then
                            For i = 0 To NominalLevelList.Count - 2
                                If NominalLevelList(i) <> NominalLevelList(i + 1) Then Throw New Exception("Unequal nominal levels detected in concatenated sound files! This may lead to unexpected sound levels during playback!")
                            Next
                        End If
                    End If

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
                    If EqualizeSoundLevel = True Then
                        'Setting all input sounds to EqualizationLevel
                        For Each InputSound In SoundsToUse
                            DSP.MeasureAndAdjustSectionLevel(InputSound, EqualizationLevel,,,, EqualizationLevelFrequencyWeighting)
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

                    If DetectedNominalLevel.HasValue Then
                        OutputSound.SMA.NominalLevel = DetectedNominalLevel.Value
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
            Public Function SuperpositionEqualLengthSounds(ByRef InputSounds As List(Of Sound)) As Sound

                If InputSounds.Count = 0 Then Return Nothing

                Dim WaveFormat = InputSounds(0).WaveFormat
                For i = 1 To InputSounds.Count - 1
                    If WaveFormat.IsEqual(InputSounds(i).WaveFormat) = False Then Throw New ArgumentException("All wave formats need to be the same!")
                Next

                Dim OutputSound As New Sound(WaveFormat)

                'Checking length equality of each channel, compared to channel 1 of the first sound
                Dim ChannelLength As Integer = InputSounds(0).WaveData.SampleData(1).Length
                For i = 0 To InputSounds.Count - 1
                    For c = 1 To InputSounds(i).WaveFormat.Channels
                        If InputSounds(i).WaveData.SampleData(c).Length <> ChannelLength Then Throw New ArgumentException("All sounds need to have the same sample array lengths (in corresponding channels)!")
                    Next
                Next

                For Channel = 1 To WaveFormat.Channels

                    Dim NewChannelArray(ChannelLength - 1) As Single
                    'Copies the first sound
                    Array.Copy(InputSounds(0).WaveData.SampleData(Channel), NewChannelArray, NewChannelArray.Length)

                    'Superpositions the remaining sounds
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
            ''' Superpositions the input sounds to a new sound. The wave format (including channel count) are required to be the same in all input sounds. The lengths of the different channels, however, need not be the same.
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

                For c = 1 To WaveFormat.Channels

                    'Getting the sound index and channel length of the longest channel c in all sounds
                    Dim LongestChannelLength As Integer = 0
                    Dim LongestSoundIndex As Integer = 0
                    For i = 0 To InputSounds.Count - 1
                        If InputSounds(i).WaveData.SampleData(c).Length > LongestChannelLength Then
                            LongestChannelLength = InputSounds(i).WaveData.SampleData(c).Length
                            LongestSoundIndex = i
                        End If
                    Next

                    'Creates an array in which to store the superpositioned sound
                    Dim NewChannelArray(LongestChannelLength - 1) As Single
                    'Copies the longest channel 
                    Array.Copy(InputSounds(LongestSoundIndex).WaveData.SampleData(c), NewChannelArray, NewChannelArray.Length)

                    'Superpositions all sounds but the longest (which is already added)
                    For i = 0 To InputSounds.Count - 1

                        'Skipping the longest sound
                        If i = LongestSoundIndex Then Continue For

                        'Referencing the current channel array
                        Dim CurrentChannelArray = InputSounds(i).WaveData.SampleData(c)
                        'Superpositioning sample values until the end of the current channel  in the current sound
                        For s = 0 To CurrentChannelArray.Length - 1
                            NewChannelArray(s) += CurrentChannelArray(s)
                        Next
                    Next

                    OutputSound.WaveData.SampleData(c) = NewChannelArray

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
            Public Sub MeasureAndAdjustSectionLevel(ByRef InputSound As Sound, ByVal targetLevel As Decimal, Optional ByVal channel As Integer? = Nothing,
                                        Optional ByVal startSample As Integer? = Nothing, Optional ByVal sectionLength As Integer? = Nothing,
                                                 Optional ByVal FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z)


                'Setting default start sample and length values
                If startSample Is Nothing Then startSample = 0
                If sectionLength Is Nothing Then sectionLength = InputSound.WaveData.ShortestChannelSampleCount

                Dim AudioOutputConstructor As New AudioOutputConstructor(InputSound.WaveFormat, channel)

                'Main section
                For c = AudioOutputConstructor.FirstChannelIndex To AudioOutputConstructor.LastChannelIndex

                    'Measures the level of the channel section
                    Dim SectionLevel As Double = MeasureSectionLevel(InputSound, c, startSample, sectionLength,,, FrequencyWeighting)

                    'Calculating the needed gain
                    Dim Gain As Double = targetLevel - SectionLevel

                    'Amplifies the section
                    AmplifySection(InputSound, Gain, c, startSample, sectionLength,)

                Next

            End Sub



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
                            Optional ByVal SlopeType As FadeSlopeType = FadeSlopeType.Linear, Optional CosinePower As Double = 10, Optional ByVal EqualPower As Boolean = False)

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

                        Dim IsFadeIn As Boolean = False
                        If startFactor < endFactor Then IsFadeIn = True

                        Dim CorrectedStartSample = startSample
                        Dim CorrectedSectionLength = sectionLength
                        CheckAndCorrectSectionLength(inputArray.Length, CorrectedStartSample, CorrectedSectionLength)

                        If CorrectedSectionLength > 1 Then ' To avoid division by zero some lines below

                            'Modifies currentFadeFactor according to a cosine finction, whereby currentModFactor starts on 1 and end at 0
                            Dim currentModFactor As Double
                            Dim currentFadeFactor As Double
                            Select Case slopeType
                                Case FadeSlopeType.Smooth

                                    If EqualPower = True Then

                                        For currentSample = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1

                                            'fadeProgress goes from 0 to 1 during the fade section
                                            fadeProgress = fadeSampleCount / (CorrectedSectionLength - 1)

                                            currentModFactor = ((Math.Cos(twopi * (fadeProgress / 2)) + 1) / 2)
                                            'currentFadeFactor = Math.Sqrt(startFactor * currentModFactor + endFactor * (1 - currentModFactor))

                                            'Fading the section
                                            inputArray(currentSample) = inputArray(currentSample) * Math.Sqrt(startFactor * currentModFactor + endFactor * (1 - currentModFactor))
                                            'inputArray(currentSample) = (inputArray(currentSample) * currentFadeFactor)
                                            fadeSampleCount += 1
                                        Next

                                    Else

                                        For currentSample = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1

                                            'fadeProgress goes from 0 to 1 during the fade section
                                            fadeProgress = fadeSampleCount / (CorrectedSectionLength - 1)

                                            currentModFactor = ((Math.Cos(twopi * (fadeProgress / 2)) + 1) / 2)
                                            'currentFadeFactor = startFactor * currentModFactor + endFactor * (1 - currentModFactor)

                                            'Fading the section
                                            inputArray(currentSample) = inputArray(currentSample) * (startFactor * currentModFactor + endFactor * (1 - currentModFactor))
                                            'inputArray(currentSample) = (inputArray(currentSample) * currentFadeFactor)
                                            fadeSampleCount += 1

                                        Next
                                    End If


                                Case FadeSlopeType.PowerCosine_SkewedFromFadeDirection

                                    For currentSample = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1

                                        'fadeProgress goes from 0 to 1 during the fade section
                                        fadeProgress = fadeSampleCount / (CorrectedSectionLength - 1)

                                        If IsFadeIn = True Then
                                            currentModFactor = ((Math.Cos(twopi * ((1 - fadeProgress) / 2)) + 1) / 2) ^ CosinePower
                                            currentFadeFactor = (startFactor * (1 - currentModFactor) + endFactor * currentModFactor)
                                        Else
                                            currentModFactor = ((Math.Cos(twopi * (fadeProgress / 2)) + 1) / 2) ^ CosinePower
                                            currentFadeFactor = (startFactor * currentModFactor) + endFactor * (1 - currentModFactor)
                                        End If

                                        If EqualPower = True Then
                                            currentFadeFactor = Math.Sqrt(currentFadeFactor)
                                        End If

                                        'Fading the section
                                        inputArray(currentSample) = (inputArray(currentSample) * currentFadeFactor)
                                        fadeSampleCount += 1
                                    Next

                                Case FadeSlopeType.PowerCosine_SkewedInFadeDirection

                                    For currentSample = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1

                                        'fadeProgress goes from 0 to 1 during the fade section
                                        fadeProgress = fadeSampleCount / (CorrectedSectionLength - 1)

                                        If IsFadeIn = True Then
                                            currentModFactor = ((Math.Cos(twopi * ((fadeProgress) / 2)) + 1) / 2) ^ CosinePower
                                            currentFadeFactor = (startFactor * (currentModFactor) + endFactor * (1 - currentModFactor))
                                        Else
                                            currentModFactor = ((Math.Cos(twopi * ((1 - fadeProgress) / 2)) + 1) / 2) ^ CosinePower
                                            currentFadeFactor = (startFactor * (1 - currentModFactor)) + endFactor * (currentModFactor)
                                        End If

                                        If EqualPower = True Then
                                            currentFadeFactor = Math.Sqrt(currentFadeFactor)
                                        End If

                                        'Fading the section
                                        inputArray(currentSample) = (inputArray(currentSample) * currentFadeFactor)
                                        fadeSampleCount += 1
                                    Next

                                Case Else 'I.e. Linear!

                                    If EqualPower = True Then

                                        For currentSample = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1

                                            'fadeProgress goes from 0 to 1 during the fade section
                                            fadeProgress = fadeSampleCount / (CorrectedSectionLength - 1)
                                            'currentFadeFactor = Math.Sqrt(startFactor * (1 - fadeProgress) + endFactor * fadeProgress)

                                            'Fading the section
                                            inputArray(currentSample) = inputArray(currentSample) * Math.Sqrt(startFactor * (1 - fadeProgress) + endFactor * fadeProgress)
                                            'inputArray(currentSample) = (inputArray(currentSample) * currentFadeFactor)
                                            fadeSampleCount += 1

                                        Next

                                    Else

                                        For currentSample = CorrectedStartSample To CorrectedStartSample + CorrectedSectionLength - 1

                                            'fadeProgress goes from 0 to 1 during the fade section
                                            fadeProgress = fadeSampleCount / (CorrectedSectionLength - 1)
                                            'currentFadeFactor = startFactor * (1 - fadeProgress) + endFactor * fadeProgress

                                            'Fading the section
                                            inputArray(currentSample) = inputArray(currentSample) * (startFactor * (1 - fadeProgress) + endFactor * fadeProgress)
                                            'inputArray(currentSample) = (inputArray(currentSample) * currentFadeFactor)
                                            fadeSampleCount += 1

                                        Next

                                    End If

                            End Select
                        End If
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
                                Dim LinearLoudness As Double = Utils.ReferenceSoundIntensityLevel * 10 ^ (CurrentPhonValue / 10)

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


        End Module


        Public Module AcousticDistance

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
                                       Optional ByVal Phase As Double = 0, Optional ByVal OverrideLevelCheck As Boolean = False) As Sound
                Try




                    Dim outputSound As New Sound(format)
                    Dim AudioOutputConstructor As New AudioOutputConstructor(format, channel)

                    'Checking valid input intensity values
                    If OverrideLevelCheck = False Then
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
                    End If

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
                            DSP.FastFourierTransform(DSP.FftDirections.Backward, X_Re, X_Im)

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
                                DSP.FastFourierTransform(DSP.FftDirections.Backward, outputSound.FFT.FrequencyDomainRealData(c, 0).WindowData, outputSound.FFT.FrequencyDomainImaginaryData(c, 0).WindowData)

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


        End Module



    End Namespace

End Namespace