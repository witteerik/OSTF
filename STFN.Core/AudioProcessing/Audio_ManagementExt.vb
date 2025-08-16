

Namespace Audio
    Public Module AudioManagementExt

        Public Sub CheckChannelValue(ByRef channelValue As Integer, ByVal upperBound As Integer)
            If channelValue < 1 Then Throw New Exception("Channel value cannot be lower than 1.")
            If channelValue > upperBound Then Throw New Exception("The referred instance does not have " & channelValue & " channels.")
        End Sub






        ''' <summary>
        ''' Extands the input array so that FFT can be run on all overlapping windows, and returns the number of zero-padding samples
        ''' </summary>
        ''' <param name="soundArray"></param>
        ''' <param name="fftFormat"></param>
        Public Function ExtendSoundArrayToWindowLengthMultiple(ByRef soundArray() As Single, ByVal fftFormat As Audio.Formats.FftFormat) As Integer

            Dim OriginalLength As Integer = soundArray.Length

            Dim windowDistance As Integer = fftFormat.AnalysisWindowSize - fftFormat.OverlapSize

            Dim inputSoundLength As Integer = soundArray.Length
            Dim numberOfWindows As Integer = Utils.Rounding(inputSoundLength / windowDistance, Utils.roundingMethods.alwaysUp)
            ReDim Preserve soundArray(numberOfWindows * windowDistance + fftFormat.AnalysisWindowSize - 1)

            Return soundArray.Length - OriginalLength

        End Function


        ''' <summary>
        ''' Copies all elements of a array of single.
        '''     ''' </summary>
        ''' <param name="inputArray">The source array to be copied.</param>
        ''' <returns>Returns a new array, which is a copy of the input array.</returns>
        Public Function CopyArrayOfSingle(ByRef inputArray() As Single)

            Dim copy(inputArray.Length - 1) As Single
            For index = 0 To inputArray.Length - 1
                copy(index) = inputArray(index)
            Next
            Return copy

        End Function


        Public Function TimeUnitConversion(ByVal InputValue As Object, ByVal ConversionDirection As TimeUnitConversionDirection,
                              ByVal SampleRate As Integer)

            Select Case ConversionDirection
                Case TimeUnitConversionDirection.samplesToSeconds
                    Return InputValue / SampleRate
                Case TimeUnitConversionDirection.secondsToSamples
                    Return Int(InputValue * SampleRate)
                Case Else
                    Throw New NotImplementedException("Incorrecly specified conversion direction. The enumerator ConversionDirection.timeUnitConversionDirection should be used by the calling code.")
            End Select

        End Function


        ''' <summary>
        ''' Converts between fft bin number and frequency.
        ''' </summary>
        ''' <param name="conversionDirection"></param>
        ''' <param name="inputValue"></param>
        ''' <param name="sampleRate"></param>
        ''' <param name="fftSize"></param>
        ''' <param name="roundingMethod"></param>
        ''' <param name="Actualvalue">When conversionDirection is FrequencyToBinIndex, this parameters holds the frequency of the actual selected fft bin.</param>
        ''' <returns></returns>
        Public Function FftBinFrequencyConversion(ByVal conversionDirection As FftBinFrequencyConversionDirection,
                                          ByVal inputValue As Single, ByVal sampleRate As Integer, ByVal fftSize As Integer,
                                          Optional ByVal roundingMethod As Utils.roundingMethods = Utils.roundingMethods.getClosestValue,
                                          Optional ByRef Actualvalue As Single = Nothing) As Single
            Try


                Select Case conversionDirection
                    Case FftBinFrequencyConversionDirection.BinIndexToFrequency

                        Try
                            Dim frequency As Single = (inputValue * sampleRate) / fftSize
                            Return frequency

                        Catch ex As Exception
                            AudioError("Error in FftBinFrequencyConversion" & vbCr & ex.ToString)
                            Return Nothing
                        End Try

                    Case FftBinFrequencyConversionDirection.FrequencyToBinIndex

                        Try
                            Dim binIndex As Single
                            Select Case roundingMethod
                                Case Utils.roundingMethods.getClosestValue
                                    binIndex = Math.Round((inputValue * fftSize) / sampleRate)
                                Case Utils.roundingMethods.alwaysDown
                                    binIndex = Int((inputValue * fftSize) / sampleRate)
                                Case Utils.roundingMethods.donNotRound
                                    binIndex = (inputValue * fftSize) / sampleRate
                                Case Utils.roundingMethods.alwaysUp
                                    binIndex = 1 + Int((inputValue * fftSize) / sampleRate)
                                Case Else
                                    Throw New NotImplementedException("Unsupported rounding method.")
                            End Select

                            'Calculates the actual cut-off frequency value used
                            Dim NewActualvalue As Single = 0
                            FftBinFrequencyConversion(FftBinFrequencyConversionDirection.BinIndexToFrequency, binIndex, sampleRate, fftSize,, NewActualvalue)
                            Actualvalue = NewActualvalue

                            Return binIndex

                        Catch ex As Exception

                            AudioError("Error in FftBinFrequencyConversion" & vbCr & ex.ToString)
                            Return Nothing
                        End Try

                    Case Else
                        Throw New NotImplementedException

                End Select

            Catch ex As Exception
                MsgBox(ex.ToString)
                Return Nothing
            End Try

        End Function

        ''' <summary>
        ''' Returns the centre frequencies of adjacent Bark filters.
        ''' </summary>
        ''' <param name="FilterOverlapRatio">A ratio between 0 and 99 % that the output filters may be overlapped on the frequency axis.</param>
        ''' <param name="LowestIncludedFrequency"></param>
        ''' <param name="HighestIncludedFrequency"></param>
        ''' <returns></returns>
        Public Function GetBarkFilterCentreFrequencies(ByVal FilterOverlapRatio As Double,
                                    ByVal LowestIncludedFrequency As Double,
                                    ByVal HighestIncludedFrequency As Double,
                                               Optional ByVal LogSelectedBands As Boolean = True) As SortedSet(Of Double)

            'Checking for invalid values of FilterOverlapRatio
            If FilterOverlapRatio < 0 Then Throw New ArgumentException("Lowest allowed bark filter overlap ratio Is 0")
            If FilterOverlapRatio > 0.99 Then Throw New ArgumentException("Highest allowed bark filter overlap ratio Is 0.99")


            'Creating a list of included filter centre frequencies
            Dim CentreFrequencies As New SortedSet(Of Double)

            CentreFrequencies.Add(LowestIncludedFrequency)
            Do
                Dim CurrentCentreFrequency As Double = CentreFrequencies(CentreFrequencies.Count - 1)
                Dim CurrentBandWidth As Double = Utils.CenterFrequencyToBarkFilterBandwidth(CurrentCentreFrequency)

                'Calculating the frequency of the next centre frequency by adding the band width of the previous filter to its centre frequency, and adjusting it to the right degree of overlap
                Dim NextCentreFrequency As Double = CurrentCentreFrequency + CurrentBandWidth - (CurrentBandWidth * FilterOverlapRatio)

                'Adding the new centre frequency if it is below the HighestIncludedFrequency, or exits the loop if the new centre frequency exceeds the HighestIncludedFrequency
                If NextCentreFrequency < HighestIncludedFrequency Then
                    CentreFrequencies.Add(NextCentreFrequency)
                Else
                    Exit Do
                End If
            Loop

            If LogSelectedBands = True Then
                'Exports centre frequencies
                Dim ExportList As New List(Of String)
                For p = 0 To CentreFrequencies.Count - 1
                    ExportList.Add(CentreFrequencies(p) & vbTab & Utils.CenterFrequencyToBarkFilterBandwidth(CentreFrequencies(p)))
                Next
                SendInfoToAudioLog(vbCrLf & String.Join(vbCrLf, ExportList), "CentreFrequencies_Count_" & CentreFrequencies.Count)
            End If

            Return CentreFrequencies

        End Function

        ''' <summary>
        ''' Applies a windowing function to the whole input array
        ''' </summary>
        ''' <param name="inputArray">The array to be modified.</param>
        ''' <param name="type">Type of windowing function.</param>
        ''' <param name="analysisWindowSize">The length of the array starting from index 0 that should be multiplied by the windowing function. If left to default (-1) the whole input array is windowed.</param>
        Public Sub WindowingFunction(ByRef inputArray() As Single, ByVal type As WindowingType,
                                 Optional ByVal analysisWindowSize As Integer = -1, Optional Tukey_r As Double = 0.5)

            'Setting analysis window size
            If analysisWindowSize = -1 Then analysisWindowSize = inputArray.Length


            'For reference see: Bateman, A. & Paterson-Stephens, I. (2002). The DSP Handbook. Algorithms, Applications and Design Techniques.
            'chapter 6, p350.(Where also desciptions can be found for Triangular, Kaiser, Blackman, and Tukey windows)

            Select Case type
                Case WindowingType.Hamming
                    For n = 0 To analysisWindowSize - 1
                        inputArray(n) = inputArray(n) * (0.54 - 0.46 * Math.Cos(twopi * n / (analysisWindowSize - 1)))
                    Next
                Case WindowingType.Hanning
                    For n = 0 To analysisWindowSize - 1
                        inputArray(n) = inputArray(n) * (0.5 - 0.5 * Math.Cos(twopi * n / (analysisWindowSize - 1)))
                    Next
                Case WindowingType.Sine 'My own type
                    For n = 0 To analysisWindowSize - 1
                        inputArray(n) = inputArray(n) * (Math.Sin(Math.PI * n / (analysisWindowSize - 1)))
                    Next
                Case WindowingType.Rectangular
                'Does nothing

                Case WindowingType.Triangular
                    For n = 0 To analysisWindowSize - 1
                        inputArray(n) = inputArray(n) * (((2 * Math.Abs(((analysisWindowSize - 1) / 2) - n)) / (analysisWindowSize - 1)) - 1) * -1
                    Next

                Case WindowingType.Blackman 'Ref Lyons (2012) "Understanding digital signal processing", p187.
                    For k = 0 To analysisWindowSize - 1
                        inputArray(k) = inputArray(k) * (0.42 - 0.5 * Math.Cos((twopi * k) / (analysisWindowSize - 1)) + 0.08 * Math.Cos((2 * twopi * k) / (analysisWindowSize - 1)))
                    Next

                Case WindowingType.Tukey

                    'Fading up
                    For k = 0 To Int((Tukey_r / 2) * analysisWindowSize) - 1
                        inputArray(k) *= 0.5 * (1 + Math.Cos(Math.PI * ((2 * k) / (Tukey_r * (analysisWindowSize - 1)) - 1)))
                    Next

                    'No change in the intermediate region
                    'For k = Int((Tukey_r / 2) * analysisWindowSize) To Int(1 - ((Tukey_r / 2) * analysisWindowSize)) - 1
                    '    inputArray(k) *= 1 
                    'Next

                    'Fading down
                    For k = Int((1 - (Tukey_r / 2)) * analysisWindowSize) To analysisWindowSize - 1
                        inputArray(k) *= 0.5 * (1 + Math.Cos(Math.PI * ((2 * k) / (Tukey_r * (analysisWindowSize - 1)) - (2 / Tukey_r) + 1)))
                    Next

            End Select

        End Sub


        ''' <summary>
        ''' Applies a windowing function to the whole input array
        ''' </summary>
        ''' <param name="inputArray">The array to be modified.</param>
        ''' <param name="type">Type of windowing function.</param>
        ''' <param name="analysisWindowSize">The length of the array starting from index 0 that should be multiplied by the windowing function. If left to default (-1) the whole input array is windowed.</param>
        Public Sub WindowingFunction(ByRef inputArray() As Double, ByVal type As WindowingType,
                                 Optional ByVal analysisWindowSize As Integer = -1, Optional Tukey_r As Double = 0.5)

            'Setting analysis window size
            If analysisWindowSize = -1 Then analysisWindowSize = inputArray.Length


            'For reference see: Bateman, A. & Paterson-Stephens, I. (2002). The DSP Handbook. Algorithms, Applications and Design Techniques.
            'chapter 6, p350.(Where also desciptions can be found for Triangular, Kaiser, Blackman, and Tukey windows)

            Select Case type
                Case WindowingType.Hamming
                    For n = 0 To analysisWindowSize - 1
                        inputArray(n) = inputArray(n) * (0.54 - 0.46 * Math.Cos(twopi * n / (analysisWindowSize - 1)))
                    Next
                Case WindowingType.Hanning
                    For n = 0 To analysisWindowSize - 1
                        inputArray(n) = inputArray(n) * (0.5 - 0.5 * Math.Cos(twopi * n / (analysisWindowSize - 1)))
                    Next
                Case WindowingType.Sine 'My own type
                    For n = 0 To analysisWindowSize - 1
                        inputArray(n) = inputArray(n) * (Math.Sin(Math.PI * n / (analysisWindowSize - 1)))
                    Next
                Case WindowingType.Rectangular
                'Does nothing

                Case WindowingType.Triangular
                    For n = 0 To analysisWindowSize - 1
                        inputArray(n) = (((2 * Math.Abs(((analysisWindowSize - 1) / 2) - n)) / (analysisWindowSize - 1)) - 1) * -1
                    Next

                Case WindowingType.Blackman 'Ref Lyons (2012) "Understanding digital signal processing", p187.
                    For k = 0 To analysisWindowSize - 1
                        inputArray(k) = inputArray(k) * (0.42 - 0.5 * Math.Cos((twopi * k) / (analysisWindowSize - 1)) + 0.08 * Math.Cos((2 * twopi * k) / (analysisWindowSize - 1)))
                    Next

                Case WindowingType.Tukey

                    'Fading up
                    For k = 0 To Int((Tukey_r / 2) * analysisWindowSize) - 1
                        inputArray(k) *= 0.5 * (1 + Math.Cos(Math.PI * ((2 * k) / (Tukey_r * (analysisWindowSize - 1)) - 1)))
                    Next

                    'No change in the intermediate region
                    'For k = Int((Tukey_r / 2) * analysisWindowSize) To Int(1 - ((Tukey_r / 2) * analysisWindowSize)) - 1
                    '    inputArray(k) *= 1 
                    'Next

                    'Fading down
                    For k = Int((1 - (Tukey_r / 2)) * analysisWindowSize) To analysisWindowSize - 1
                        inputArray(k) *= 0.5 * (1 + Math.Cos(Math.PI * ((2 * k) / (Tukey_r * (analysisWindowSize - 1)) - (2 / Tukey_r) + 1)))
                    Next

            End Select

        End Sub

        ''' <summary>
        ''' Returns equivalent noise bandwidth data pre-calculated in matlab version R2017a
        ''' </summary>
        ''' <param name="WindowLength"></param>
        ''' <param name="WindowingType"></param>
        ''' <returns></returns>
        Public Function GetEquivalentNoiseBandwidth(ByRef WindowLength As Integer, ByRef WindowingType As WindowingType,
                                                Optional ByRef Tukey_r As Double? = Nothing) As Double

            Select Case WindowingType
                Case WindowingType.Hamming
                    Select Case WindowLength
                        Case 64
                            Return 1.3783
                        Case 128
                            Return 1.3705
                        Case 256
                            Return 1.3667
                        Case 512
                            Return 1.3647
                        Case 1024
                            Return 1.3638
                        Case 2048
                            Return 1.3633
                        Case 4096
                            Return 1.3631
                        Case 8192
                            Return 1.3629
                        Case 16384
                            Return 1.3629
                        Case Else
                            Throw New NotImplementedException

                    End Select
                Case WindowingType.Hanning

                    Select Case WindowLength
                        Case 64
                            Return 1.5238
                        Case 128
                            Return 1.5118
                        Case 256
                            Return 1.5059
                        Case 512
                            Return 1.5029
                        Case 1024
                            Return 1.5015
                        Case 2048
                            Return 1.5007
                        Case 4096
                            Return 1.5004
                        Case 8192
                            Return 1.5002
                        Case 16384
                            Return 1.5001
                        Case Else
                            Throw New NotImplementedException
                    End Select

                Case WindowingType.Tukey

                    If Tukey_r Is Nothing Then Throw New ArgumentException("If Tukey window is used, also a Tukey r is needed.")
                    Select Case Tukey_r
                        Case 0.5

                            Select Case WindowLength
                                Case 128
                                    Return 1.2318
                                Case 256
                                    Return 1.227
                                Case 512
                                    Return 1.2246
                                Case 1024
                                    Return 1.2234
                                Case 2048
                                    Return 1.2228
                                Case 4096
                                    Return 1.2225
                                Case 8192
                                    Return 1.2224
                                Case 16384
                                    Return 1.2223
                                Case Else
                                    Throw New NotImplementedException
                            End Select

                        Case Else
                            Throw New NotImplementedException("Values has not been added for the current Tukey window r.")
                    End Select


                Case Else
                    Throw New NotImplementedException

            End Select


        End Function

        ''' <summary>
        ''' Returns the gain needed to compensate for the scaling applied by windowing an array.
        ''' </summary>
        ''' <param name="WindowLength"></param>
        ''' <param name="WindowingType"></param>
        ''' <returns></returns>
        Public Function GetInverseWindowingScalingFactor(ByRef WindowLength As Integer, ByRef WindowingType As WindowingType)

            If WindowLength < 1 Then Throw New ArgumentException("Window length cannot be lower than 1.")

            'Creating a windowed array with 1 as the unwindowed values
            Dim WindowArray(WindowLength - 1) As Double
            For n = 0 To WindowArray.Length - 1
                WindowArray(n) = 1
            Next
            WindowingFunction(WindowArray, WindowingType)

            'Returning the inverse of the scaling applied by the window
            Return 1 / WindowArray.Average

        End Function


        ''' <summary>
        ''' Performs variuos checks on a chosen fft size. If input fftSize is set to nothing, fft size is set to the lowest valid 
        ''' value equal to or higher than the lower inclusive limit. If fft size is lower than the lower inclusive limit, fft size is set 
        ''' to a value equal to or higher than the lower inclusive limit. Is a non valid fft size (I.E. not a base to integer exponent), the
        ''' fft size is set to a value equal to or higher than the input fft size.
        ''' </summary>
        ''' <param name="fftSize"></param>
        ''' <param name="lowerInclusiveLimit"></param>
        Public Sub CheckAndAdjustFFTSize(ByRef fftSize As Double, ByVal lowerInclusiveLimit As Double, Optional ByRef inActivateWarnings As Boolean = False)

            'limit is an inclusive value, ie fftSize can be set to limit

            'Finding the smallest fftSize => lower inclusive limit
            If fftSize = Nothing Then
                'sets the fftsize
                Dim exponent As Integer = 1
                fftSize = 0
                Do Until fftSize >= lowerInclusiveLimit
                    fftSize = 2 ^ exponent
                    exponent += 1
                Loop
                If inActivateWarnings = False Then
                    MsgBox("No fft size is chosen. Setting the fftSize automatically to: " & fftSize)

                    'Inactivates the warning after the first time it's been shown
                    inActivateWarnings = True
                End If
            Else

                'If the manually set FFT size is too small, it is increased automatically.
                If fftSize < lowerInclusiveLimit Then
                    Dim exponent As Integer = 1
                    fftSize = 0
                    Do Until fftSize >= lowerInclusiveLimit
                        fftSize = 2 ^ exponent
                        exponent += 1
                    Loop
                    If inActivateWarnings = False Then
                        MsgBox("The chosen FFT size is to small. Increasing it to " & fftSize & " points.")

                        'Inactivates the warning after the first time it's been shown
                        inActivateWarnings = True
                    End If
                End If

                'Checks that the set fft windows size value is valid
                Dim validValues As Integer = 0
                For exponent = 0 To 1023
                    If 2 ^ exponent = fftSize Then
                        validValues += 1
                        Exit For
                    End If
                Next
                If Not validValues > 0 Then

                    'Corrects the erraneous fft size to the nearest valid fft size that is larger than the input fft size
                    Dim exponent As Integer = 1
                    Dim newFftSize As Integer
                    newFftSize = 0
                    Do Until newFftSize >= fftSize
                        newFftSize = 2 ^ exponent
                        exponent += 1
                    Loop
                    fftSize = newFftSize
                    If inActivateWarnings = False Then
                        MsgBox("The specified fft size is not valid. Setting it automatically to: " & fftSize)

                        'Inactivates the warning after the first time it's been shown
                        inActivateWarnings = True
                    End If
                End If

            End If

        End Sub




        Public Enum FftBinFrequencyConversionDirection
            FrequencyToBinIndex
            BinIndexToFrequency
        End Enum

        Public Enum WindowingType
            Rectangular
            Hamming
            Hanning
            Sine
            Blackman
            Triangular
            Tukey
        End Enum


        Public Enum TimeUnitConversionDirection
            samplesToSeconds
            secondsToSamples
        End Enum



        Public Enum TriangularWaveTypes
            FullRange
            InverseFullRange
            PositiveHalfRange
            NegativeHalfRange
        End Enum

        Public Enum Transducers
            SoundField_ANSI2004
            TDH_IEC
        End Enum


        Public Enum FilterType
            LowPass
            BandPass
            HighPass
            BandStop
            RandomPhase
            LinearAttenuationBelowCF_dBPerOctave
            LinearAttenuationAboveCF_dBPerOctave
        End Enum


    End Module

End Namespace