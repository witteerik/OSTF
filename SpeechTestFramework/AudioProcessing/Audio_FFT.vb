'This software is available under the following license:
'MIT/X11 License
'
'Copyright (c) 2017 Erik Witte
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


Namespace Audio

    <Serializable>
    Public Class FftData

        Private _FrequencyDomainRealData As New List(Of List(Of TimeWindow))
        Private _FrequencyDomainImaginaryData As New List(Of List(Of TimeWindow))
        Private _AmplitudeSpectrum As New List(Of List(Of TimeWindow))
        Private _PhaseSpectrum As New List(Of List(Of TimeWindow))
        Private _PowerSpectrumData As New List(Of List(Of TimeWindow))
        Private _SpectrogramData As New List(Of List(Of TimeWindow))
        Private _BarkSpectrumData As New List(Of BarkSpectrum)
        Public _BarkSpectrumTimeWindowData As New List(Of List(Of TimeWindow))

        ''' <summary>
        ''' An object that can be used to temporarily store data. Non-serialized.
        ''' </summary>
        <NonSerialized>
        Public TemporaryData As New List(Of TimeWindow)

        <Serializable>
        Public Class TimeWindow
            Public WindowData As Double()
            Public Property WindowingType As WindowingType?
            Public Property ZeroPadding As Integer?
            Public TotalPower As Double?

            Public Sub CalculateTotalPower()
                TotalPower = WindowData.Sum
            End Sub

        End Class

        Public Property Name As String
        Public ReadOnly Property WindowCount(ByVal channel As Integer) As Integer
            Get
                CheckChannelValue(channel, _FrequencyDomainRealData.Count)
                Return _FrequencyDomainRealData(channel - 1).Count
            End Get
        End Property

        Public ReadOnly Property ChannelCount As Integer
            Get
                Return _FrequencyDomainRealData.Count
            End Get
        End Property

        Public ReadOnly Property SpectrogramColumnCount(ByVal channel As Integer) As Integer
            Get
                CheckChannelValue(channel, _SpectrogramData.Count)
                Return _SpectrogramData(channel - 1).Count
            End Get
        End Property


        Public ReadOnly Property SpectrogramChannelCount As Integer
            Get
                Return _SpectrogramData.Count
            End Get
        End Property

        Public ReadOnly Property Waveformat As Audio.Formats.WaveFormat
        Public ReadOnly Property FftFormat As Audio.Formats.FftFormat

        Public Property FrequencyDomainRealData(ByVal channel As Integer, ByVal windowNumber As Integer) As TimeWindow
            Get
                CheckChannelValue(channel, _FrequencyDomainRealData.Count)
                If windowNumber > _FrequencyDomainRealData(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_FrequencyDomainRealData, windowNumber - _FrequencyDomainRealData(channel - 1).Count + 1)
                Return _FrequencyDomainRealData(channel - 1)(windowNumber)
            End Get
            Set(value As TimeWindow)
                CheckChannelValue(channel, _FrequencyDomainRealData.Count)
                If windowNumber > _FrequencyDomainRealData(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_FrequencyDomainRealData, windowNumber - _FrequencyDomainRealData(channel - 1).Count + 1)
                _FrequencyDomainRealData(channel - 1)(windowNumber) = value

            End Set
        End Property

        Public Property FrequencyDomainImaginaryData(ByVal channel As Integer, ByVal windowNumber As Integer) As TimeWindow
            Get
                CheckChannelValue(channel, _FrequencyDomainImaginaryData.Count)
                If windowNumber > _FrequencyDomainImaginaryData(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_FrequencyDomainImaginaryData, windowNumber - _FrequencyDomainImaginaryData(channel - 1).Count + 1)
                Return _FrequencyDomainImaginaryData(channel - 1)(windowNumber)
            End Get
            Set(value As TimeWindow)
                CheckChannelValue(channel, _FrequencyDomainImaginaryData.Count)
                If windowNumber > _FrequencyDomainImaginaryData(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_FrequencyDomainImaginaryData, windowNumber - _FrequencyDomainImaginaryData(channel - 1).Count + 1)
                _FrequencyDomainImaginaryData(channel - 1)(windowNumber) = value

            End Set
        End Property

        Public Property BarkSpectrumTimeWindowData(ByVal Channel As Integer, ByVal windowNumber As Integer) As TimeWindow
            Get
                CheckChannelValue(Channel, _BarkSpectrumTimeWindowData.Count)
                'Behövs kontroll av adderingsbehov här?
                Return _BarkSpectrumTimeWindowData(Channel - 1)(windowNumber)
            End Get
            Set(value As TimeWindow)
                checkChannelValue(Channel, _BarkSpectrumTimeWindowData.Count)
                If windowNumber > _BarkSpectrumTimeWindowData(Channel - 1).Count - 1 Then ExtendFrequencyDomainData(_BarkSpectrumTimeWindowData, windowNumber - _BarkSpectrumTimeWindowData(Channel - 1).Count + 1)
                _BarkSpectrumTimeWindowData(Channel - 1)(windowNumber) = value
            End Set
        End Property

        Public Sub AddBarkSpectrumTimeWindowData(ByRef NewTimeWindow As TimeWindow, ByVal Channel As Integer)
            _BarkSpectrumTimeWindowData(Channel - 1).Add(NewTimeWindow)
        End Sub

        Public Property BarkSpectrumData(ByVal Channel As Integer) As BarkSpectrum
            Get
                Return _BarkSpectrumData(Channel - 1)
            End Get
            Set(value As BarkSpectrum)
                _BarkSpectrumData(Channel - 1) = value
            End Set
        End Property

        Private _BinIndexToFrequencyList As List(Of Double)
        Public ReadOnly Property BinIndexToFrequencyList(Optional ByVal RecreateList As Boolean = False, Optional ByVal SkipNegativeFrequencies As Boolean = True) As List(Of Double)
            Get
                If _BinIndexToFrequencyList Is Nothing Or RecreateList = True Then
                    'Creating a new bin index to frequency list
                    _BinIndexToFrequencyList = New List(Of Double)
                    Dim BinCount As Integer
                    If SkipNegativeFrequencies = True Then
                        BinCount = FftFormat.FftWindowSize / 2
                    Else
                        BinCount = FftFormat.FftWindowSize
                    End If
                    For k = 0 To BinCount - 1
                        _BinIndexToFrequencyList.Add(FftBinFrequencyConversion(FftBinFrequencyConversionDirection.BinIndexToFrequency, k, Waveformat.SampleRate, FftFormat.FftWindowSize))
                    Next
                End If

                Return _BinIndexToFrequencyList

            End Get
        End Property

        Public Property AmplitudeSpectrum(ByVal channel As Integer, ByVal windowNumber As Integer) As TimeWindow
            Get
                checkChannelValue(channel, _AmplitudeSpectrum.Count)
                'Behövs kontroll av adderingsbehov här?
                Return _AmplitudeSpectrum(channel - 1)(windowNumber)
            End Get
            Set(value As TimeWindow)
                checkChannelValue(channel, _AmplitudeSpectrum.Count)
                If windowNumber > _AmplitudeSpectrum(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_AmplitudeSpectrum, windowNumber - _AmplitudeSpectrum(channel - 1).Count + 1)
                _AmplitudeSpectrum(channel - 1)(windowNumber) = value

            End Set
        End Property

        Public Property PhaseSpectrum(ByVal channel As Integer, ByVal windowNumber As Integer) As TimeWindow
            Get
                checkChannelValue(channel, _PhaseSpectrum.Count)
                'Behövs kontroll av adderingsbehov här?
                Return _PhaseSpectrum(channel - 1)(windowNumber)
            End Get
            Set(value As TimeWindow)
                checkChannelValue(channel, _PhaseSpectrum.Count)
                If windowNumber > _PhaseSpectrum(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_PhaseSpectrum, windowNumber - _PhaseSpectrum(channel - 1).Count + 1)
                _PhaseSpectrum(channel - 1)(windowNumber) = value

            End Set
        End Property

        Public Property PowerSpectrumData(ByVal channel As Integer, ByVal windowNumber As Integer) As TimeWindow
            Get
                checkChannelValue(channel, _PowerSpectrumData.Count)
                'Behövs kontroll av adderingsbehov här?
                Return _PowerSpectrumData(channel - 1)(windowNumber)
            End Get
            Set(value As TimeWindow)
                checkChannelValue(channel, _PowerSpectrumData.Count)
                If windowNumber > _PowerSpectrumData(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_PowerSpectrumData, windowNumber - _PowerSpectrumData(channel - 1).Count + 1)
                _PowerSpectrumData(channel - 1)(windowNumber) = value

            End Set
        End Property

        Public Property SpectrogramData(ByVal channel As Integer, ByVal windowNumber As Integer) As TimeWindow
            Get
                checkChannelValue(channel, _SpectrogramData.Count)
                'Behövs kontroll av adderingsbehov här?
                Return _SpectrogramData(channel - 1)(windowNumber)
            End Get
            Set(value As TimeWindow)
                checkChannelValue(channel, _SpectrogramData.Count)
                If windowNumber > _SpectrogramData(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_SpectrogramData, windowNumber - _SpectrogramData(channel - 1).Count + 1)
                _SpectrogramData(channel - 1)(windowNumber) = value

            End Set
        End Property

        Public Sub New(ByVal setWaveFormat As Audio.Formats.WaveFormat, ByVal setFftFormat As Audio.Formats.FftFormat, Optional setName As String = "")

            Name = setName
            Waveformat = setWaveFormat
            FftFormat = setFftFormat

            For n = 0 To Waveformat.Channels - 1
                Dim ChannelFftRealData As New List(Of TimeWindow)
                _FrequencyDomainRealData.Add(ChannelFftRealData)
                Dim ChannelFftImaginaryData As New List(Of TimeWindow)
                _FrequencyDomainImaginaryData.Add(ChannelFftImaginaryData)
                Dim ChannelAmplitudeSpectrum As New List(Of TimeWindow)
                _AmplitudeSpectrum.Add(ChannelAmplitudeSpectrum)
                Dim ChannelPhaseSpectrum As New List(Of TimeWindow)
                _PhaseSpectrum.Add(ChannelPhaseSpectrum)
                Dim ChannelPowerSpectrumData As New List(Of TimeWindow)
                _PowerSpectrumData.Add(ChannelPowerSpectrumData)
                Dim ChannelSpectrogramData As New List(Of TimeWindow)
                _SpectrogramData.Add(ChannelSpectrogramData)
                'Adding empty indices to the list
                _BarkSpectrumData.Add(Nothing)
                Dim ChannelBarkSpectrumTimeWindowData As New List(Of TimeWindow)
                _BarkSpectrumTimeWindowData.Add(ChannelBarkSpectrumTimeWindowData)
            Next

        End Sub

        Private Sub ExtendFrequencyDomainData(ByRef dataToExtend As List(Of List(Of TimeWindow)), ByVal windowCountToAdd As Integer)

            For channel = 0 To dataToExtend.Count - 1
                For n = 0 To windowCountToAdd - 1
                    Dim NewTimeWindow As New TimeWindow
                    Dim newDoubleArray(FftFormat.FftWindowSize - 1) As Double
                    NewTimeWindow.WindowData = newDoubleArray
                    dataToExtend(channel).Add(NewTimeWindow)
                Next
            Next

        End Sub

        ''' <summary>
        ''' Calculates the phase spectrum of a sound using the real and imaginary data stored in the current FFTData object.
        ''' </summary>
        Public Sub CalculatePhaseSpectrum()

            'Resetting channel phase data
            For channel = 0 To _PhaseSpectrum.Count - 1
                _PhaseSpectrum(channel).Clear()
            Next

            For channel = 0 To Me._FrequencyDomainRealData.Count - 1
                Dim NewChannelPhaseSpectrum As New List(Of TimeWindow)
                For window = 0 To Me._FrequencyDomainRealData(channel).Count - 1
                    Dim NewTimeWindow As New TimeWindow
                    Dim NewWindowPhaseSpectrum(FftFormat.FftWindowSize - 1) As Double
                    NewTimeWindow.WindowData = NewWindowPhaseSpectrum

                    'Copying window description data from the _FrequencyDomainRealData
                    NewTimeWindow.WindowingType = _FrequencyDomainRealData(channel)(window).WindowingType
                    NewTimeWindow.ZeroPadding = _FrequencyDomainRealData(channel)(window).ZeroPadding

                    'Getting the spectrum array
                    NewTimeWindow.WindowData = GetTimeWindowSpectrum(channel + 1, window, SpectrumTypes.PhaseSpectrum, False, False, False,,)

                    'getPhases(NewWindowPhaseSpectrum, _FrequencyDomainRealData(channel)(window).WindowData, _FrequencyDomainImaginaryData(channel)(window).WindowData)
                    NewChannelPhaseSpectrum.Add(NewTimeWindow)
                Next
                _PhaseSpectrum(channel) = NewChannelPhaseSpectrum
            Next

        End Sub

        '''' <summary>
        '''' A local variable used in CalculateAmplitudeSpectrum
        '''' </summary>
        'Private CurrentChannel As Integer = 0

        ''' <summary>
        ''' Calculates the amplitude spectrum of a sound using the real and imaginary data stored in the current FFTData object.
        ''' </summary>
        ''' <param name="CompensateForEquivalentNoiseBandwidthScaling"></param>
        ''' <param name="CompensateForZeroPaddingScaling"></param>
        ''' <param name="CompensateForTimeWindowingScaling"></param>
        Public Sub CalculateAmplitudeSpectrum(Optional ByVal CompensateForEquivalentNoiseBandwidthScaling As Boolean = True,
                                     Optional ByVal CompensateForZeroPaddingScaling As Boolean = True,
                                     Optional ByVal CompensateForTimeWindowingScaling As Boolean = True)

            Try


                'Resetting channel magnitude data
                For channel = 0 To _AmplitudeSpectrum.Count - 1
                    _AmplitudeSpectrum(channel).Clear()
                Next


                If Utils.GenerallyAllowParallelProcessing = False Then

                    'Declaring some variables that will be re-used between channels and time windows
                    Dim EquivalentNoiseBandwidth As Double? = Nothing
                    Dim InverseTimeWindowingScalingFactor As Double? = Nothing

                    For channel = 0 To Me._FrequencyDomainRealData.Count - 1
                        Dim NewChannelAmplitudeSpectrum As New List(Of TimeWindow)
                        For window = 0 To Me._FrequencyDomainRealData(channel).Count - 1
                            Dim NewTimeWindow As New TimeWindow
                            Dim NewWindowAmplitudeSpectrum(FftFormat.FftWindowSize - 1) As Double
                            NewTimeWindow.WindowData = NewWindowAmplitudeSpectrum

                            'Copying window description data from the _FrequencyDomainRealData
                            NewTimeWindow.WindowingType = _FrequencyDomainRealData(channel)(window).WindowingType
                            NewTimeWindow.ZeroPadding = _FrequencyDomainRealData(channel)(window).ZeroPadding

                            'Getting the spectrum array
                            NewTimeWindow.WindowData = GetTimeWindowSpectrum(channel + 1, window, SpectrumTypes.AmplitudeSpectrum, CompensateForEquivalentNoiseBandwidthScaling,
                                      CompensateForZeroPaddingScaling, CompensateForTimeWindowingScaling,
                                      EquivalentNoiseBandwidth, InverseTimeWindowingScalingFactor)

                            'Adding the spectrum array
                            NewChannelAmplitudeSpectrum.Add(NewTimeWindow)
                        Next
                        _AmplitudeSpectrum(channel) = NewChannelAmplitudeSpectrum
                    Next
                Else

                    'Declaring some variables that will be re-used between channels and time windows
                    Dim EquivalentNoiseBandwidth As Double? = Nothing
                    Dim InverseTimeWindowingScalingFactor As Double? = Nothing
                    Dim CurrentChannel As Integer = 0

                    'Adding the needed (empty) time windows
                    For channel = 0 To Me._FrequencyDomainRealData.Count - 1
                        Dim NewChannelAmplitudeSpectrum As New List(Of TimeWindow)
                        For window = 0 To Me._FrequencyDomainRealData(channel).Count - 1
                            NewChannelAmplitudeSpectrum.Add(Nothing)
                        Next
                        _AmplitudeSpectrum(channel) = NewChannelAmplitudeSpectrum
                    Next

                    For channel = 0 To Me._FrequencyDomainRealData.Count - 1

                        'Setting the private variable CurrentChannel in order to use for the channel value within the parallel sub below
                        CurrentChannel = channel

                        Parallel.For(0, Me._FrequencyDomainRealData(channel).Count, Sub(window)

                                                                                        Dim NewTimeWindow As New TimeWindow
                                                                                        Dim NewWindowAmplitudeSpectrum(FftFormat.FftWindowSize - 1) As Double
                                                                                        NewTimeWindow.WindowData = NewWindowAmplitudeSpectrum

                                                                                        'Copying window description data from the _FrequencyDomainRealData
                                                                                        NewTimeWindow.WindowingType = _FrequencyDomainRealData(CurrentChannel)(window).WindowingType
                                                                                        NewTimeWindow.ZeroPadding = _FrequencyDomainRealData(CurrentChannel)(window).ZeroPadding

                                                                                        'Getting the spectrum array
                                                                                        NewTimeWindow.WindowData = GetTimeWindowSpectrum(CurrentChannel + 1, window,
                                                                                                                                         SpectrumTypes.AmplitudeSpectrum,
                                                                                                                                         CompensateForEquivalentNoiseBandwidthScaling,
                                                                                                          CompensateForZeroPaddingScaling, CompensateForTimeWindowingScaling,
                                                                                                          EquivalentNoiseBandwidth, InverseTimeWindowingScalingFactor)

                                                                                        'Adding the spectrum array
                                                                                        _AmplitudeSpectrum(CurrentChannel)(window) = NewTimeWindow

                                                                                    End Sub)
                    Next

                End If

            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try

        End Sub

        ''' <summary>
        ''' Calculates the power spectrum of a sound using the real and imaginary data stored in the current FFTData object.
        ''' </summary>
        ''' <param name="CompensateForEquivalentNoiseBandwidthScaling"></param>
        ''' <param name="CompensateForZeroPaddingScaling"></param>
        ''' <param name="CompensateForTimeWindowingScaling"></param>
        ''' <param name="ZeroPaddingCompensationLowerLimit">If set, limits the effect of zero padding compensation to a proportion of the fft size (e.g. 0.25 for using fftsize*0.25 as lowest compensation factor.).</param>
        Public Sub CalculatePowerSpectrum(Optional ByVal CompensateForEquivalentNoiseBandwidthScaling As Boolean = True,
                                     Optional ByVal CompensateForZeroPaddingScaling As Boolean = True,
                                     Optional ByVal CompensateForTimeWindowingScaling As Boolean = True,
                                          Optional ByVal ZeroPaddingCompensationLowerLimit As Double? = Nothing)

            'Resetting channel power spectrum data
            For channel = 0 To _PowerSpectrumData.Count - 1
                _PowerSpectrumData(channel).Clear()
            Next


            If Utils.GenerallyAllowParallelProcessing = False Then

                'Declaring some variables that will be re-used between channels and time windows
                Dim EquivalentNoiseBandwidth As Double? = Nothing
                Dim InverseTimeWindowingScalingFactor As Double? = Nothing

                For channel = 0 To Me._FrequencyDomainRealData.Count - 1
                    Dim NewChannelPowerSpectrumData As New List(Of TimeWindow)
                    For window = 0 To Me._FrequencyDomainRealData(channel).Count - 1
                        Dim NewTimeWindow As New TimeWindow
                        Dim NewWindowPowerSpectrumData(FftFormat.FftWindowSize - 1) As Double
                        NewTimeWindow.WindowData = NewWindowPowerSpectrumData

                        'Copying window description data from the _FrequencyDomainRealData
                        NewTimeWindow.WindowingType = _FrequencyDomainRealData(channel)(window).WindowingType
                        NewTimeWindow.ZeroPadding = _FrequencyDomainRealData(channel)(window).ZeroPadding

                        'Getting the spectrum array
                        NewTimeWindow.WindowData = GetTimeWindowSpectrum(channel + 1, window, SpectrumTypes.PowerSpectrum, CompensateForEquivalentNoiseBandwidthScaling,
                                          CompensateForZeroPaddingScaling, CompensateForTimeWindowingScaling,
                                          EquivalentNoiseBandwidth, InverseTimeWindowingScalingFactor, ZeroPaddingCompensationLowerLimit)

                        NewTimeWindow.CalculateTotalPower()

                        'Adding the spectrum array
                        NewChannelPowerSpectrumData.Add(NewTimeWindow)
                    Next
                    _PowerSpectrumData(channel) = NewChannelPowerSpectrumData
                Next
            Else

                'Declaring some variables that will be re-used between channels and time windows
                Dim EquivalentNoiseBandwidth As Double? = Nothing
                Dim InverseTimeWindowingScalingFactor As Double? = Nothing
                Dim CurrentChannel As Integer = 0

                'Adding the needed (empty) time windows
                For channel = 0 To Me._FrequencyDomainRealData.Count - 1
                    Dim NewChannelPowerSpectrumData As New List(Of TimeWindow)
                    For window = 0 To Me._FrequencyDomainRealData(channel).Count - 1
                        NewChannelPowerSpectrumData.Add(Nothing)
                    Next
                    _PowerSpectrumData(channel) = NewChannelPowerSpectrumData
                Next

                For channel = 0 To Me._FrequencyDomainRealData.Count - 1

                    'Setting the private variable CurrentChannel in order to use for the channel value within the parallel sub below
                    CurrentChannel = channel

                    Parallel.For(0, Me._FrequencyDomainRealData(channel).Count, Sub(window)

                                                                                    Dim NewTimeWindow As New TimeWindow
                                                                                    Dim NewWindowPowerSpectrumData(FftFormat.FftWindowSize - 1) As Double
                                                                                    NewTimeWindow.WindowData = NewWindowPowerSpectrumData

                                                                                    'Copying window description data from the _FrequencyDomainRealData
                                                                                    NewTimeWindow.WindowingType = _FrequencyDomainRealData(CurrentChannel)(window).WindowingType
                                                                                    NewTimeWindow.ZeroPadding = _FrequencyDomainRealData(CurrentChannel)(window).ZeroPadding

                                                                                    'Getting the spectrum array
                                                                                    NewTimeWindow.WindowData = GetTimeWindowSpectrum(CurrentChannel + 1, window, SpectrumTypes.PowerSpectrum, CompensateForEquivalentNoiseBandwidthScaling,
                                                                                                          CompensateForZeroPaddingScaling, CompensateForTimeWindowingScaling,
                                                                                                          EquivalentNoiseBandwidth, InverseTimeWindowingScalingFactor)


                                                                                    'Adding the spectrum array
                                                                                    _PowerSpectrumData(CurrentChannel)(window) = NewTimeWindow

                                                                                End Sub)
                Next

            End If

        End Sub

        ''' <summary>
        ''' Gets the average spectrum stored in the Amplitude, Power or Phase spectrum objects.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetAverageSpectrum(ByVal Channel As Integer,
                                       ByVal SpectrumType As SpectrumTypes,
                                       ByVal SoundFormat As Audio.Formats.WaveFormat,
                                       Optional ByVal ConvertTo_dB As Boolean = True,
                                       Optional ByRef TotalLevel As Double = Nothing) As SortedList(Of Double, Double)


            Dim BinSpectrum As New SortedList(Of Integer, Double)

            TotalLevel = 0

            Select Case SpectrumType
                Case SpectrumTypes.PowerSpectrum

                    For k = 0 To PowerSpectrumData(Channel, 0).WindowData.Length / 2 - 1

                        BinSpectrum.Add(k, 0)
                        For TimeWindow = 0 To WindowCount(Channel) - 1
                            BinSpectrum(k) += PowerSpectrumData(Channel, TimeWindow).WindowData(k) * 2 '*2 since negative frequency values are not read
                        Next

                        TotalLevel += BinSpectrum(k)

                        If ConvertTo_dB = True Then
                            BinSpectrum(k) = dBConversion(BinSpectrum(k) / WindowCount(Channel), dBConversionDirection.to_dB,
                                                  SoundFormat, dBTypes.SoundPower)
                        Else
                            BinSpectrum(k) = BinSpectrum(k) / WindowCount(Channel)
                        End If
                    Next

                Case SpectrumTypes.AmplitudeSpectrum

                    For k = 0 To AmplitudeSpectrum(Channel, 0).WindowData.Length / 2 - 1

                        BinSpectrum.Add(k, 0)
                        For TimeWindow = 0 To WindowCount(Channel) - 1
                            'BinList(k) += AmplitudeSpectrum(Channel, TimeWindow).WindowData(k) * 2 '*2 since negative frequency values are not read

                            'Converting spectral magnitudes to power. Summing spectral power. 
                            'SummedBinPowers += 2 * 10 ^ ((20 * Math.Log10((Math.Sqrt(2) * AmplitudeSpectrum(channel, TimeWindow).WindowData(k)) / Math.Sqrt(2))) / 10)
                            'SummedBinPowers += 2 * 10 ^ ((20 * Math.Log10(AmplitudeSpectrum(channel, TimeWindow).WindowData(k))) / 10)
                            'Simplified to:
                            BinSpectrum(k) += 100 ^ (Math.Log10(AmplitudeSpectrum(Channel, TimeWindow).WindowData(k)))

                        Next

                        'Adding to the total level. (And multiplying by two to compensate for only reading the positive frequencies.)
                        TotalLevel += 2 * BinSpectrum(k)

                        'Taking the quare root to convert power spectrum to amplitude spectrum, and divides by WindowCount(Channel) to average the value of the time windows
                        'And also multiplying by two to compensate for only reading the positive frequencies.
                        BinSpectrum(k) = 2 * Math.Sqrt(BinSpectrum(k) / WindowCount(Channel))

                        If ConvertTo_dB = True Then
                            BinSpectrum(k) = dBConversion(BinSpectrum(k), dBConversionDirection.to_dB,
                                                  SoundFormat, dBTypes.SoundPressure)
                        Else
                            BinSpectrum(k) = BinSpectrum(k)
                        End If

                    Next

                Case Else
                    Throw New NotImplementedException

            End Select

            If ConvertTo_dB = True Then TotalLevel = dBConversion(TotalLevel / WindowCount(Channel), dBConversionDirection.to_dB,
                                   SoundFormat, dBTypes.SoundPower)


            'Converting bins to frequencies
            Dim OutputList As New SortedList(Of Double, Double)
            For Each kvp In BinSpectrum
                OutputList.Add(FftBinFrequencyConversion(FftBinFrequencyConversionDirection.BinIndexToFrequency,
                                                     kvp.Key, SoundFormat.SampleRate, Me.FftFormat.FftWindowSize), kvp.Value)
            Next

            Return OutputList

        End Function

        Public Sub ExportPowerSpectrum(ByVal Channel As Integer, Optional ByVal FileName As String = "PowerSpectrum",
                                       Optional ByVal OutputFolder As String = "", Optional SkipNegativeFrequencies As Boolean = True,
                                       Optional ByVal ConvertTo_dB As Boolean = True, Optional ByVal ReferenceIntensity As Double = 1)

            If OutputFolder = Nothing Then OutputFolder = Utils.logFilePath

            Dim OutputList As New List(Of String)

            If SkipNegativeFrequencies = True Then

                'Dim Test As Double = 0

                For k = 0 To PowerSpectrumData(Channel, 0).WindowData.Length / 2 - 1
                    Dim BinList As New List(Of String)
                    For TimeWindow = 0 To WindowCount(Channel) - 1
                        'Test += PowerSpectrumData(Channel, TimeWindow).WindowData(k) * 2

                        If ConvertTo_dB = True Then
                            BinList.Add(10 * Math.Log10(PowerSpectrumData(Channel, TimeWindow).WindowData(k) * 2) / ReferenceIntensity)
                        Else
                            BinList.Add(PowerSpectrumData(Channel, TimeWindow).WindowData(k) * 2) '*2 since negative frequency values are not read
                        End If

                    Next
                    OutputList.Add(String.Join(vbTab, BinList))
                Next

                'MsgBox("Level: " & 10 * Math.Log10(Test / windowCount(Channel)) / ReferenceIntensity)

            Else

                For k = 0 To PowerSpectrumData(Channel, 0).WindowData.Length - 1
                    Dim BinList As New List(Of String)
                    For TimeWindow = 0 To WindowCount(Channel) - 1
                        If ConvertTo_dB = True Then
                            BinList.Add(10 * Math.Log10(PowerSpectrumData(Channel, TimeWindow).WindowData(k)) / ReferenceIntensity)
                        Else
                            BinList.Add(PowerSpectrumData(Channel, TimeWindow).WindowData(k)) '*2 since negative frequency values are not read
                        End If

                    Next
                    OutputList.Add(String.Join(vbTab, BinList))
                Next

            End If

            SendInfoToAudioLog("PowerSpectrum. Fft size: " & FftFormat.FftWindowSize & vbCrLf & String.Join(vbCrLf, OutputList), FileName, OutputFolder)


        End Sub

        Public Function GetDcComponent(ByVal channel As Integer, ByVal windowNumber As Integer,
                                       Optional ByRef OutputType As GetSpectrumLevel_OutputType = GetSpectrumLevel_OutputType.SpectrumLevel_dB) As Double

            Return GetSpectrumLevel(channel, windowNumber, , 0, 0,, OutputType)

        End Function


        Public Enum GetSpectrumLevel_InputType
            FftBinIndex
            FftBinCentreFrequency_Hz
        End Enum

        Public Enum GetSpectrumLevel_OutputType
            SpectrumLevel_dB
            SpectrumPower_Linear
        End Enum

        ''' <summary>
        ''' Calculates the spectrum level/power based on the current frequency domain data.
        ''' </summary>
        ''' <param name="channel"></param>
        ''' <param name="windowNumber"></param>
        ''' <param name="LowerLimit">Lower inclusive fft bin / frequency.</param>
        ''' <param name="UpperLimit">Highest inclusive fft bin / frequency.</param>
        ''' <param name="InputType"></param>
        ''' <param name="OutputType"></param>
        ''' <param name="ActualLowerLimitFrequency">Returns the centre frequency of the actual lowest fft bin used.</param>
        ''' <param name="ActualUpperLimitFrequency">Returns the centre frequency of the actual highest fft bin used.</param>
        ''' <returns></returns>
        Public Function GetSpectrumLevel(ByVal channel As Integer, ByVal windowNumber As Integer,
                                         Optional ByVal SpectrumType As SpectrumTypes = SpectrumTypes.AmplitudeSpectrum,
                                         Optional ByVal LowerLimit As Single? = Nothing,
                                         Optional ByVal UpperLimit As Single? = Nothing,
                                         Optional ByRef InputType As GetSpectrumLevel_InputType = GetSpectrumLevel_InputType.FftBinIndex,
                                         Optional ByRef OutputType As GetSpectrumLevel_OutputType = GetSpectrumLevel_OutputType.SpectrumLevel_dB,
                                         Optional ByRef ActualLowerLimitFrequency As Single? = Nothing,
                                         Optional ByRef ActualUpperLimitFrequency As Single? = Nothing,
                                     Optional ByVal LowerLimitIsInclusive As Boolean = True,
                                     Optional ByVal UpperLimitIsInclusive As Boolean = True) As Double

            Try

                'Converting frequency to fft bins
                If InputType = GetSpectrumLevel_InputType.FftBinCentreFrequency_Hz Then
                    'Setting default UpperInclusiveFrequency
                    If LowerLimit Is Nothing Then LowerLimit = 0
                    If UpperLimit Is Nothing Then UpperLimit = Waveformat.SampleRate / 2 - 1

                    'Converting frequencies to bin values
                    If LowerLimitIsInclusive = True Then
                        LowerLimit = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, LowerLimit,
                                                                         Waveformat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.alwaysDown)
                    Else
                        LowerLimit = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, LowerLimit,
                                                                         Waveformat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.alwaysUp)

                    End If


                    If UpperLimitIsInclusive = True Then

                        UpperLimit = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, UpperLimit,
                                                                         Waveformat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.alwaysUp)

                    Else

                        UpperLimit = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, UpperLimit,
                                                                         Waveformat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.alwaysDown)

                        'Subtracting 1 to get the exclusive limit
                        'UpperLimit -= 1

                        'Limiting to positive values
                        'If UpperLimit < 0 Then UpperLimit = 0
                    End If

                    'Making sure that LowerLimit and UpperLimit are integers
                    LowerLimit = Int(LowerLimit)
                    UpperLimit = Int(UpperLimit)

                End If


                'Checking if spectrum has been calculated
                Select Case SpectrumType
                    Case SpectrumTypes.PowerSpectrum
                        If _PowerSpectrumData(channel - 1).Count = 0 Then
                            'Calculating power data
                            CalculatePowerSpectrum()
                        End If

                    Case SpectrumTypes.AmplitudeSpectrum
                        If _AmplitudeSpectrum(channel - 1).Count = 0 Then
                            'Calculating amplidute data
                            CalculateAmplitudeSpectrum()
                        End If
                    Case Else
                        Throw New NotImplementedException("Spectrum level cannot be calculated from the phase spectrum. Use power spectrum or amplitude spectrum instead.")
                End Select

                'Copying the spectrum values to a SortedList (so they don't get overwritten)
                Dim SpectrumDataLength As Integer = FrequencyDomainRealData(channel, windowNumber).WindowData.Length
                Dim TempSpectrumData As New SortedList(Of Integer, Single)

                'Setting default input values
                If LowerLimit Is Nothing Then LowerLimit = 0
                If UpperLimit Is Nothing Then UpperLimit = SpectrumDataLength / 2

                'Restricting the values of LowerInclusiveBin and UpperInclusiveBin to their valid ranges
                If LowerLimit < 0 Then LowerLimit = 0
                If UpperLimit > SpectrumDataLength / 2 - 1 Then UpperLimit = SpectrumDataLength / 2 - 1


                'Summing spectral power
                Dim SummedBinPowers As Double = 0

                Select Case SpectrumType
                    Case SpectrumTypes.PowerSpectrum
                        For n = LowerLimit To UpperLimit

                            'Summing spectral power. Multiplying by two to compensate for only reading the positive frequencies.
                            SummedBinPowers += PowerSpectrumData(channel, windowNumber).WindowData(n) * 2

                        Next
                    Case SpectrumTypes.AmplitudeSpectrum
                        For n = LowerLimit To UpperLimit

                            'Converting spectral magnitudes to power. Summing spectral power. And multiplying by two to compensate for only reading the positive frequencies.
                            'Some testing code
                            'Dim HANN_CORR As Double = 3.32 + 0.94 - 0.0039
                            'SummedBinPowers += 2 * 10 ^ ((HANN_CORR + 20 * Math.Log10((Math.Sqrt(2) * AmplitudeSpectrum(channel, windowNumber).WindowData(n)) / Math.Sqrt(2))) / 10)

                            'SummedBinPowers += 2 * 10 ^ ((20 * Math.Log10((Math.Sqrt(2) * AmplitudeSpectrum(channel, windowNumber).WindowData(n)) / Math.Sqrt(2))) / 10)
                            'SummedBinPowers += 2 * 10 ^ ((20 * Math.Log10(AmplitudeSpectrum(channel, windowNumber).WindowData(n))) / 10)

                            'Simplified to:
                            SummedBinPowers += 2 * 100 ^ (Math.Log10(AmplitudeSpectrum(channel, windowNumber).WindowData(n)))

                        Next
                End Select

                Dim CurrentBinCount As Integer = UpperLimit - LowerLimit + 1

                'Calculating the actual cut-off band centre frequencies used
                ActualLowerLimitFrequency = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.BinIndexToFrequency, LowerLimit,
                                                                         Waveformat.SampleRate, FftFormat.FftWindowSize)
                ActualUpperLimitFrequency = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.BinIndexToFrequency, UpperLimit,
                                                                         Waveformat.SampleRate, FftFormat.FftWindowSize)


                'Selecting output unit
                Select Case OutputType
                    Case GetSpectrumLevel_OutputType.SpectrumLevel_dB

                        'Converting to dB
                        Select Case SpectrumType
                            Case SpectrumTypes.PowerSpectrum
                                Return 10 * Math.Log10(SummedBinPowers / Waveformat.PositiveFullScale) 'TODO: check if it's right to use PositiveFullScale even for non floats here?
                            Case SpectrumTypes.AmplitudeSpectrum
                                'Throw New System.Exception("The amplitude type calculation is not working properly yet...")

                                Return 10 * Math.Log10(SummedBinPowers / Waveformat.PositiveFullScale) 'TODO: check if it's right to use PositiveFullScale even for non floats here?

                                'Return dBConversion(SummedBinPowers,  dBConversionDirection.to_dB, waveformat)
                            Case Else
                                Throw New NotImplementedException()
                        End Select

                    Case GetSpectrumLevel_OutputType.SpectrumPower_Linear

                        Return SummedBinPowers

                    Case Else
                        Throw New NotImplementedException()
                End Select

            Catch ex As Exception
                MsgBox(ex.ToString)
                Return Nothing
            End Try

        End Function

        <Obsolete>
        Public Function GetSpectrumMagnitudeOfOneBin(ByVal PositiveFrequencyBinIndex As Integer,
                                                 ByVal channel As Integer, ByVal windowNumber As Integer,
                                                 Optional ByRef EquivalentNoiseBandwidth As Double? = Nothing,
                                                 Optional ByRef ZeroPaddingInverseScalingFactor As Double? = Nothing,
                                                 Optional ByRef InverseTimeWindowingScalingFactor As Double? = Nothing) As Double


            'Copying the power spectrum values to a SortedList (so they don't get overwritten)
            Dim PowerSpectrumDataLength As Integer = PowerSpectrumData(channel, windowNumber).WindowData.Length

            'Copying the values needed 
            Dim PositivePowerData As Single = PowerSpectrumData(channel, windowNumber).WindowData(PositiveFrequencyBinIndex)
            Dim NegativePowerData As Single = PowerSpectrumData(channel, windowNumber).WindowData(PowerSpectrumDataLength - PositiveFrequencyBinIndex)

            'Adjusting for Equivalent Noise Bandwidth
            If FftFormat.WindowingType <> WindowingType.Rectangular Then

                If EquivalentNoiseBandwidth Is Nothing Then EquivalentNoiseBandwidth = GetEquivalentNoiseBandwidth(FftFormat.FftWindowSize, FftFormat.WindowingType, FftFormat.Tukey_r)

                'Adjusting the positive side
                PositivePowerData /= EquivalentNoiseBandwidth

                'Adjusting the negative side
                NegativePowerData /= EquivalentNoiseBandwidth
            End If

            'Compensating for the scaling introduced by zero padding
            If FftFormat.WindowingType <> WindowingType.Rectangular Then

                'Dim ZeroPaddingInverseScalingFactor As Double = 1 / (FftFormat.AnalysisWindowSize / FftFormat.FftWindowSize)
                'ZeroPaddingInverseScalingFactor will be the same within the same time window and can be re-used, but must be re-calculated for each window. This can be achieved by setting ZeroPaddingInverseScalingFactor in the calling code for each new time window
                If ZeroPaddingInverseScalingFactor Is Nothing Then ZeroPaddingInverseScalingFactor = 1 / ((FftFormat.FftWindowSize - PowerSpectrumData(channel, windowNumber).ZeroPadding) / FftFormat.FftWindowSize)

                'Adjusting the positive side
                PositivePowerData *= ZeroPaddingInverseScalingFactor

                'Adjusting the negative side
                NegativePowerData *= ZeroPaddingInverseScalingFactor

            End If

            'Compensating for the scaling introduced by the windowing function
            If FftFormat.WindowingType <> WindowingType.Rectangular Then

                If InverseTimeWindowingScalingFactor Is Nothing Then InverseTimeWindowingScalingFactor = GetInverseWindowingScalingFactor(PowerSpectrumDataLength, FftFormat.WindowingType)

                'Adjusting the positive side
                PositivePowerData *= InverseTimeWindowingScalingFactor ^ 2

                'Adjusting the negative side
                NegativePowerData *= InverseTimeWindowingScalingFactor ^ 2

            End If

            'Summing and returning spectral power
            Return PositivePowerData + NegativePowerData

        End Function

        <Obsolete>
        Public Function GetSpectrumPowerOfOneBin(ByVal PositiveFrequencyBinIndex As Integer,
                                                 ByVal channel As Integer, ByVal windowNumber As Integer,
                                                 Optional ByRef EquivalentNoiseBandwidth As Double? = Nothing,
                                                 Optional ByRef ZeroPaddingInverseScalingFactor As Double? = Nothing,
                                                 Optional ByRef InverseTimeWindowingScalingFactor As Double? = Nothing) As Double

            '            ByRef EquivalentNoiseBandwidth As Double?,
            'ByRef ZeroPaddingInverseScalingFactor As Double?,
            'ByRef InverseTimeWindowingScalingFactor As Double?) As Double


            'Copying the power spectrum values to a SortedList (so they don't get overwritten)
            Dim PowerSpectrumDataLength As Integer = PowerSpectrumData(channel, windowNumber).WindowData.Length

            'Copying the values needed 
            Dim PositivePowerData As Single = PowerSpectrumData(channel, windowNumber).WindowData(PositiveFrequencyBinIndex)
            Dim NegativePowerData As Single = PowerSpectrumData(channel, windowNumber).WindowData(PowerSpectrumDataLength - PositiveFrequencyBinIndex)

            'Adjusting for Equivalent Noise Bandwidth
            If FftFormat.WindowingType <> WindowingType.Rectangular Then

                If EquivalentNoiseBandwidth Is Nothing Then EquivalentNoiseBandwidth = GetEquivalentNoiseBandwidth(FftFormat.FftWindowSize, FftFormat.WindowingType, FftFormat.Tukey_r)

                'Adjusting the positive side
                PositivePowerData /= EquivalentNoiseBandwidth

                'Adjusting the negative side
                NegativePowerData /= EquivalentNoiseBandwidth
            End If

            'Compensating for the scaling introduced by zero padding
            If FftFormat.WindowingType <> WindowingType.Rectangular Then

                'Dim ZeroPaddingInverseScalingFactor As Double = 1 / (FftFormat.AnalysisWindowSize / FftFormat.FftWindowSize)
                'ZeroPaddingInverseScalingFactor will be the same within the same time window and can be re-used, but must be re-calculated for each window. This can be achieved by setting ZeroPaddingInverseScalingFactor in the calling code for each new time window
                If ZeroPaddingInverseScalingFactor Is Nothing Then ZeroPaddingInverseScalingFactor = 1 / ((FftFormat.FftWindowSize - PowerSpectrumData(channel, windowNumber).ZeroPadding) / FftFormat.FftWindowSize)

                'Adjusting the positive side
                PositivePowerData *= ZeroPaddingInverseScalingFactor

                'Adjusting the negative side
                NegativePowerData *= ZeroPaddingInverseScalingFactor

            End If

            'Compensating for the scaling introduced by the windowing function
            If FftFormat.WindowingType <> WindowingType.Rectangular Then

                If InverseTimeWindowingScalingFactor Is Nothing Then InverseTimeWindowingScalingFactor = GetInverseWindowingScalingFactor(PowerSpectrumDataLength, FftFormat.WindowingType)

                'Adjusting the positive side
                PositivePowerData *= InverseTimeWindowingScalingFactor ^ 2

                'Adjusting the negative side
                NegativePowerData *= InverseTimeWindowingScalingFactor ^ 2

            End If

            'Summing and returning spectral power
            Return PositivePowerData + NegativePowerData

        End Function

        Public Enum SpectrumTypes
            AmplitudeSpectrum
            PowerSpectrum
            PhaseSpectrum
        End Enum

        Public Function GetBarkFilterSpectrum(ByVal Channel As Integer,
                                              ByVal SpectrumType As FftData.SpectrumTypes,
                                              ByVal BarkFilterOverlapRatio As Double,
                                              ByRef LowestIncludedCentreFrequency As Double,
                                              ByRef HighestIncludedCentreFrequency As Double,
                                              ByRef CentreFrequencies As SortedSet(Of Double),
                                              ByRef TriangularFilters As SortedList(Of Integer, Single()),
                                              ByVal UseSpecificBarkFilterScalingFactors As Boolean,
                                              ByRef BarkBandSpecificInverseFilterScaleFactors As List(Of Double?),
                                              Optional ByVal ConvertTo_dB As Boolean = True,
                                              Optional ByRef NoiseFloorLevels As List(Of Double) = Nothing) As SortedList(Of Integer, Single())

            Try


                'Summing powers into frequency bands
                Dim FilterredBandPowerArray As New SortedList(Of Integer, Single())

                'Looking at one time window at a time
                For w = 0 To WindowCount(Channel) - 1

                    'Adding the band magnitude
                    FilterredBandPowerArray.Add(w, GetTimeWindowBarkFilterSpectrum(Channel, w, SpectrumType, BarkFilterOverlapRatio,
                                                                                          LowestIncludedCentreFrequency, HighestIncludedCentreFrequency,
                                                                                          CentreFrequencies, TriangularFilters,
                                                                                             UseSpecificBarkFilterScalingFactors,
                                                                                   BarkBandSpecificInverseFilterScaleFactors))
                Next

                'Converting to dB
                If ConvertTo_dB = True Then
                    Select Case SpectrumType
                        Case SpectrumTypes.PowerSpectrum
                            For Each Window In FilterredBandPowerArray
                                For n = 0 To Window.Value.Count - 1
                                    FilterredBandPowerArray(Window.Key)(n) = 10 * Math.Log10(FilterredBandPowerArray(Window.Key)(n) / ReferenceSoundIntensityLevel)
                                Next
                            Next

                        Case SpectrumTypes.AmplitudeSpectrum
                            For Each Window In FilterredBandPowerArray
                                For n = 0 To Window.Value.Count - 1
                                    'FilterredBandPowerArray(Window.Key)(n) = Math.Log10(FilterredBandPowerArray(Window.Key)(n) + Single.Epsilon)
                                    FilterredBandPowerArray(Window.Key)(n) = dBConversion(FilterredBandPowerArray(Window.Key)(n), dBConversionDirection.to_dB, Waveformat)
                                Next
                            Next

                            Throw New NotImplementedException("Phon conversion not finished!")

                    End Select
                End If

                Return FilterredBandPowerArray

            Catch ex As Exception
                MsgBox(ex.ToString)
                Return Nothing
            End Try

        End Function


        ''' <summary>
        ''' Filters a set of FFT band powers using a set of Bark filters.
        ''' </summary>
        ''' <param name="SpectrumType">The type of spectrum that is the basis of the Bark filterred spektrum. May be either amplitude or power spectrum.</param>
        ''' <param name="FilterOverlapRatio">The relative degree (allowed range is 0-0.99) of overlap between filters. If left empty, the bark filters will be positioned next to each other so that adjacent filters share cut-off frequencies.</param>
        ''' <param name="LowestIncludedFrequency">Lower included centre frequency.</param>
        ''' <param name="HighestIncludedFrequency">Highest included centre frequency.</param>
        ''' <param name="TriangularFilters">A set of triangular filters that are being created when needed. Each filter length is only created once, and then re-used when needed. In order to avoid creating new filters on each call, the calling code should provide a reference that can be used on repeated calls.</param>
        ''' <returns>Returns a SortedList where keys repressent window number and the values repressent arrays of Bandcount averaged filter levels, in descending order from lowest to highest frequency band.</returns>
        Private Function GetTimeWindowBarkFilterSpectrum(ByVal channel As Integer, ByVal windowNumber As Integer,
                                                   Optional ByVal SpectrumType As SpectrumTypes = SpectrumTypes.AmplitudeSpectrum,
                                                   Optional ByVal FilterOverlapRatio As Double = 0,
                                                   Optional ByVal LowestIncludedFrequency As Double = 80,
                                                   Optional ByVal HighestIncludedFrequency As Double = 8000,
                                                   Optional ByRef CentreFrequencies As SortedSet(Of Double) = Nothing,
                                                   Optional ByRef TriangularFilters As SortedList(Of Integer, Single()) = Nothing,
                                                         Optional ByVal UseSpecificBarkFilterScalingFactors As Boolean = True,
                                                         Optional ByRef BarkBandSpecificInverseFilterScaleFactors As List(Of Double?) = Nothing) As Single()

            Try

                If BarkBandSpecificInverseFilterScaleFactors Is Nothing Then

                    BarkBandSpecificInverseFilterScaleFactors = New List(Of Double?)
                    BarkBandSpecificInverseFilterScaleFactors.Add(Nothing)

                    'Running the function recursively to fill up the BarkBandSpecificInverseFilterScaleFactors
                    GetTimeWindowBarkFilterSpectrum(channel, windowNumber, SpectrumType, FilterOverlapRatio,
                                                   LowestIncludedFrequency, HighestIncludedFrequency, CentreFrequencies, TriangularFilters,
                                                    False, BarkBandSpecificInverseFilterScaleFactors)

                End If

                Dim TempSpectrumDataLength As Integer = 0
                Select Case SpectrumType
                    Case SpectrumTypes.PowerSpectrum
                    Case SpectrumTypes.AmplitudeSpectrum
                End Select

                Select Case SpectrumType
                    Case SpectrumTypes.PowerSpectrum
                        If PowerSpectrumData(channel, windowNumber) Is Nothing Then Throw New Exception("A power spectrum need to be calculated prior to running BarkFilterSpectrum.")

                        'Storing the intended length of the temporary spectrum array
                        TempSpectrumDataLength = PowerSpectrumData(channel, windowNumber).WindowData.Length / 2
                    Case SpectrumTypes.AmplitudeSpectrum
                        If AmplitudeSpectrum(channel, windowNumber) Is Nothing Then Throw New Exception("An amplitude spectrum need to be calculated prior to running BarkFilterSpectrum.")

                        'Storing the intended length of the temporary spectrum array
                        TempSpectrumDataLength = AmplitudeSpectrum(channel, windowNumber).WindowData.Length / 2
                    Case Else
                        Throw New Exception("Unsupported spectrum type.")
                End Select

                'Creating a new TriangularFilters if not done by the calling code
                If TriangularFilters Is Nothing Then TriangularFilters = New SortedList(Of Integer, Single())

                'Creating a list of included filter centre frequencies (only calculated if CentreFrequencies is Nothing)
                If CentreFrequencies Is Nothing Then CentreFrequencies = GetBarkFilterCentreFrequencies(FilterOverlapRatio, LowestIncludedFrequency, HighestIncludedFrequency, True)

                'Getting a temporary array containing the spectrum to be used within each fft bin wide band
                Dim TempBinSpectrum(TempSpectrumDataLength - 1) As Double

                If BarkBandSpecificInverseFilterScaleFactors IsNot Nothing Then 'Using this to check if we're in the filter measurement mode (inner loop)
                    Dim UseParallelProcessing As Boolean = True
                    'If UseParallelProcessing = True Then

                    '    Parallel.For(1, TempBinSpectrum.Length - 1, TempBinSpectrum(, channel, windowNumber, fftFormat,
                    '                 EquivalentNoiseBandwidth, ZeroPaddingInverseScalingFactor, InverseTimeWindowingScalingFactor),)
                    'Else
                    Select Case SpectrumType
                        Case SpectrumTypes.AmplitudeSpectrum

                            For n = 0 To TempBinSpectrum.Length - 1
                                TempBinSpectrum(n) = AmplitudeSpectrum(channel, windowNumber).WindowData(n)
                                'TempBinSpectrum(n) = GetSpectrumMagnitudeOfOneBin(n, channel, windowNumber, fftFormat, EquivalentNoiseBandwidth, ZeroPaddingInverseScalingFactor, InverseTimeWindowingScalingFactor)
                                'TempBinSpectrum(n) = GetSpectrumLevel(channel, windowNumber, fftFormat, SoundFormat, n, n,, GetSpectrumLevel_OutputType.SpectrumPower_Linear)
                            Next

                        Case SpectrumTypes.PowerSpectrum
                            For n = 0 To TempBinSpectrum.Length - 1
                                TempBinSpectrum(n) = PowerSpectrumData(channel, windowNumber).WindowData(n) * 2 'Times 2 compensates for the loss of energy to the negative frequency side (which should be symmetric). Instead of assuming perfect symmetry between the positive and the negative side, the negative side power could be summed with the positive side. A problem though is the 0 frequency component which has only one repressentation in fft index 0.
                                'TempBinSpectrum(n) = GetSpectrumPowerOfOneBin(n, channel, windowNumber, fftFormat, EquivalentNoiseBandwidth, ZeroPaddingInverseScalingFactor, InverseTimeWindowingScalingFactor)
                                'TempBinSpectrum(n) = GetSpectrumLevel(channel, windowNumber, fftFormat, SoundFormat, n, n,, GetSpectrumLevel_OutputType.SpectrumPower_Linear)
                            Next
                    End Select



                    'End If
                End If

                'MsgBox("Probe 1, Spectral RMS level: " & 10 * Math.Log10(TempBinSpectrum.Sum / SoundFormat.PositiveFullScale))

                'Creating an output array (BarkBands)
                Dim BarkBands(CentreFrequencies.Count - 1) As Single

                'Goeing through each bark band of at a time
                For CentreFrequencyIndex = 0 To CentreFrequencies.Count - 1

                    'Determining the range of the current Bark band (with the current centre frequency)
                    Dim CurrentCentreFrequency As Single = CentreFrequencies(CentreFrequencyIndex)
                    Dim CurrentBandWidth As Single = Utils.CenterFrequencyToBarkFilterBandwidth(CurrentCentreFrequency)
                    Dim LowerFrequencyLimit As Single = CurrentCentreFrequency - CurrentBandWidth 'Getting the lowest frequency that add loudness to the current critical band (This frequency is 0.5 bark below the lower cut-off frequency of the current critical band (Based on Zwicker and Fastl(1999), Phsycho-acoustics, p 164ff). (The filter approximates the auditory filter responce to noise, rather than pure tones is used.))
                    Dim HighestFrequencyLimit As Single = CurrentCentreFrequency + CurrentBandWidth 'Getting the highest frequency that add loudness to the current critical band (This frequency is 0.5 bark above the upper cut-off frequency of the current critical band (Based on Zwicker and Fastl(1999), Phsycho-acoustics, p 164ff). (The filter approximates the auditory filter responce to noise, rather than pure tones is used.))
                    Dim LowestIncludedBinIndex As Integer = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                                                                            LowerFrequencyLimit, Waveformat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.alwaysDown)
                    Dim HighestIncludedBinIndex As Integer = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                                                                            HighestFrequencyLimit, Waveformat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.alwaysUp)
                    'Dim CentreBinIndex As Integer = FftBinFrequencyConversion( FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                    '                                                        CurrentCentreFrequency, SoundFormat.SampleRate, fftFormat.FftWindowSize,  roundingMethods.getClosestValue)

                    'Preparing a triangular filter for the current critical band width
                    Dim BandBinCount As Integer = HighestIncludedBinIndex - LowestIncludedBinIndex + 1
                    Dim myFilter() As Single

                    If TriangularFilters.ContainsKey(BandBinCount) Then
                        'Re-using an existing filter array
                        myFilter = TriangularFilters(BandBinCount)
                    Else

                        'Initializing a new filter array with the values 1
                        ReDim myFilter(BandBinCount - 1)
                        For n = 0 To myFilter.Count - 1
                            myFilter(n) = 1
                        Next

                        'Windowing the filter array to create a triangual filter array
                        windowingFunction(myFilter, WindowingType.Triangular)

                        'Adding the filter array for re-use
                        TriangularFilters.Add(BandBinCount, myFilter)

                    End If

                    'Collecting fft bin values within the band
                    Dim CurrentFilterIndex As Integer = 0
                    Dim ActualBandBinCount As Integer = 0
                    For fb = LowestIncludedBinIndex To HighestIncludedBinIndex

                        'Checking that LowestIncludedBinIndex is not below 0, due to always rounding bin index down. If so, it is simply skipped
                        If fb < 0 Then
                            CurrentFilterIndex += 1 'This used to be only just before the next Next statement, but it should be here. However, it only matters when bin indices would get frequency values below zero Hz, and make only small differences to the overall output.
                            Continue For
                        End If

                        'Checking that HighestIncludedBinIndex is not too high, due to always rounding bin index up. If so, it is simply skipped.
                        If fb > FftFormat.FftWindowSize / 2 - 1 Then
                            CurrentFilterIndex += 1 'This used to be only just before the next Next statement, but it should be here. However, it only matters when bin indices would get frequency values below zero Hz, and make only small differences to the overall output.
                            Continue For
                        End If


                        'Filling up BarkBandSpecificInverseFilterScaleFactors when this is not complete in the inner loop (non-overlapping round)
                        If BarkBandSpecificInverseFilterScaleFactors(0) Is Nothing Then
                            For n = 1 To CentreFrequencies.Count - 1
                                BarkBandSpecificInverseFilterScaleFactors.Add(Nothing)
                            Next
                        End If

                        If BarkBandSpecificInverseFilterScaleFactors(CentreFrequencyIndex) Is Nothing Then
                            'Just summing for triangular window scaling factor
                            BarkBands(CentreFrequencyIndex) += myFilter(CurrentFilterIndex)
                        Else
                            BarkBands(CentreFrequencyIndex) += myFilter(CurrentFilterIndex) * TempBinSpectrum(fb)
                        End If

                        ActualBandBinCount += 1
                        CurrentFilterIndex += 1
                    Next

                    'Applying inverse filter gain
                    If BarkBandSpecificInverseFilterScaleFactors(CentreFrequencyIndex) Is Nothing Then

                        'Storing inverse filter gain (only done to the initial array of ones)
                        Dim ScalingFactor As Double = (BarkBands(CentreFrequencyIndex) / ActualBandBinCount) / 0.5
                        Dim Inverse_ScalingFactor As Double = 1 / ScalingFactor
                        If UseSpecificBarkFilterScalingFactors = True Then
                            BarkBandSpecificInverseFilterScaleFactors(CentreFrequencyIndex) = Inverse_ScalingFactor
                        Else
                            'Storing 1 in BarkBandSpecificInverseFilterScaleFactors(CentreFrequencyIndex) to disable specific Bark band filterring scaling compensation.
                            BarkBandSpecificInverseFilterScaleFactors(CentreFrequencyIndex) = 1
                        End If

                    Else

                        BarkBands(CentreFrequencyIndex) *= BarkBandSpecificInverseFilterScaleFactors(CentreFrequencyIndex)
                    End If
                Next

                Return BarkBands

            Catch ex As Exception
                MsgBox(ex.ToString)
                Return Nothing
            End Try

        End Function

        Public Enum LoudnessFunctions
            InExType
            ZwickerType
            Simple
        End Enum

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="InputSound"></param>
        ''' <param name="Channel"></param>
        ''' <param name="CurrentIsoPhonFilter"></param>
        ''' <param name="CurrentAuditoryFilters"></param>
        ''' <param name="CurrentSpreadOfMaskingFilters"></param>
        ''' <param name="FilterOverlapRatio"></param>
        ''' <param name="LowestIncludedFrequency"></param>
        ''' <param name="HighestIncludedFrequency"></param>
        ''' <param name="dbFSToSplDifference"></param>
        ''' <param name="CurrentBandTemplateList"></param>
        ''' <param name="AverageSpectrumIntoFirstIndex">If set to true, the spectral data in all time windows will be averaged into the first index of the current instance of BarkSpectrum.</param>
        ''' <param name="AverageingStartMargin">An initial proportion of the time windows that will be ignored by the averaging. (Important if, for example, the sound has been faded in.)</param>
        ''' <param name="AverageingEndMargin">A final proportion of the time windows that will be ignored by the averaging. (Important if, for example, the sound has been faded out.)</param>
        Public Sub CalculateBarkSpectrum(ByRef InputSound As Audio.Sound, ByVal Channel As Integer, ByRef CurrentIsoPhonFilter As Audio.DSP.IsoPhonFilter,
                                         ByRef CurrentAuditoryFilters As AuditoryFilters, ByRef CurrentSpreadOfMaskingFilters As FftData.SpreadOfMaskingFilters,
                           ByVal FilterOverlapRatio As Double, ByVal LowestIncludedFrequency As Double, ByVal HighestIncludedFrequency As Double,
                           ByVal dbFSToSplDifference As Double, ByRef LoudnessFunction As LoudnessFunctions,
                                         Optional ByRef CurrentBandTemplateList As BarkSpectrum.BandTemplateList = Nothing,
                                         Optional ByRef SoneScalingFactor As Double? = Nothing,
                                         Optional ByVal AverageSpectrumIntoFirstIndex As Boolean = False,
                           Optional ByVal AverageingStartMargin As Double = 0.1, Optional ByVal AverageingEndMargin As Double = 0.1)


            'Doing Sone scaling on the first run
            If SoneScalingFactor Is Nothing Then

                'Sone scaling
                'Running the Bark band analysis on a sine wave of 40 dB SPL equivalent. The output is used to scale the output of the model so that 
                'a 40 dB SPL sine gets a loudness value of 1 Sone. The factor is stored in SoneScalingFactor.

                'Setting a default value of SoneScalingFactor of 1 (which means that the unscaled result will be returned)
                SoneScalingFactor = 1

                'Creating a measurement sound
                Dim SineWave = Audio.GenerateSound.CreateSineWave(InputSound.WaveFormat,, 1000, 0.1,, 1)
                Audio.DSP.MeasureAndAdjustSectionLevel(SineWave, -dbFSToSplDifference + 40)
                SineWave.FileName = "ScalingSine"
                SineWave.FFT = Audio.DSP.SpectralAnalysis(SineWave, InputSound.FFT.FftFormat)

                'Calculating power spectrum
                SineWave.FFT.CalculatePowerSpectrum(True, True, True, 0.25)

                'Running the Bark band analysis
                SineWave.FFT.BarkSpectrumData(Channel) = New BarkSpectrum(SineWave, Channel, CurrentIsoPhonFilter, CurrentAuditoryFilters, CurrentSpreadOfMaskingFilters,
                           FilterOverlapRatio, LowestIncludedFrequency, HighestIncludedFrequency,
                           dbFSToSplDifference, LoudnessFunction, CurrentBandTemplateList, SoneScalingFactor,
                                                                  AverageSpectrumIntoFirstIndex, AverageingStartMargin, AverageingEndMargin)

                'Updating the Sone scaling factor, taking it from half of the available time windows (to avoid any final fading caused by the specral analysis). 
                Dim AveragingList As New List(Of Double)
                For n = 0 To Int(SineWave.FFT.BarkSpectrumData(1).Count / 2) - 1
                    AveragingList.Add(SineWave.FFT.BarkSpectrumData(1)(n).Sum)
                Next
                'Just so it won't crash if there were less than 2 windows
                If AveragingList.Count = 0 Then AveragingList.Add(SineWave.FFT.BarkSpectrumData(1)(0).Sum)

                'Averaging the output
                SoneScalingFactor = AveragingList.Average

            End If


            'Running the sharp analysis
            _BarkSpectrumData(Channel - 1) = New BarkSpectrum(InputSound, Channel, CurrentIsoPhonFilter, CurrentAuditoryFilters, CurrentSpreadOfMaskingFilters,
                           FilterOverlapRatio, LowestIncludedFrequency, HighestIncludedFrequency,
                           dbFSToSplDifference, LoudnessFunction, CurrentBandTemplateList, SoneScalingFactor, AverageSpectrumIntoFirstIndex, AverageingStartMargin, AverageingEndMargin)

        End Sub

        <Serializable>
        Public Class BarkSpectrum
            Inherits SortedList(Of Integer, Single())

            Public ReadOnly MyDetailedList As New SortedList(Of Integer, List(Of BarkBand))

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="InputSound"></param>
            ''' <param name="Channel"></param>
            ''' <param name="CurrentIsoPhonFilter"></param>
            ''' <param name="CurrentAuditoryFilters"></param>
            ''' <param name="CurrentSpreadOfMaskingFilters"></param>
            ''' <param name="FilterOverlapRatio"></param>
            ''' <param name="LowestIncludedFrequency"></param>
            ''' <param name="HighestIncludedFrequency"></param>
            ''' <param name="dbFSToSplDifference"></param>
            ''' <param name="CurrentBandTemplateList"></param>
            ''' <param name="AverageSpectrumIntoFirstIndex">If set to true, the spectral data in all time windows will be averaged into the first index of the current instance of BarkSpectrum.</param>
            ''' <param name="AverageingStartMargin">An initial proportion of the time windows that will be ignored by the averaging. (Important if, for example, the sound has been faded in.)</param>
            ''' <param name="AverageingEndMargin">A final proportion of the time windows that will be ignored by the averaging. (Important if, for example, the sound has been faded out.)</param>
            Public Sub New(ByRef InputSound As Audio.Sound, ByVal Channel As Integer, ByRef CurrentIsoPhonFilter As Audio.DSP.IsoPhonFilter,
                           ByRef CurrentAuditoryFilters As AuditoryFilters, ByRef CurrentSpreadOfMaskingFilters As FftData.SpreadOfMaskingFilters,
                           ByVal FilterOverlapRatio As Double, ByVal LowestIncludedFrequency As Double, ByVal HighestIncludedFrequency As Double,
                           ByVal dbFSToSplDifference As Double, ByRef LoudnessFunction As LoudnessFunctions, ByRef CurrentBandTemplateList As BandTemplateList,
                           Optional ByRef SoneScalingFactor As Double = Nothing,
                           Optional ByVal AverageSpectrumIntoFirstIndex As Boolean = False,
                           Optional ByVal AverageingStartMargin As Double = 0.1, Optional ByVal AverageingEndMargin As Double = 0.1)

                'Creating a BandDescriptionList if not already done
                If CurrentBandTemplateList Is Nothing Then CurrentBandTemplateList = New BandTemplateList(FilterOverlapRatio, LowestIncludedFrequency, HighestIncludedFrequency,
                                            InputSound.WaveFormat, InputSound.FFT.fftFormat)

                'Creating an IsoPhonfilter
                If CurrentIsoPhonFilter Is Nothing Then
                    'Dim FrequenciesToPreCalculate As List(Of Double) = CurrentBandTemplateList.CentreFrequencies.ToList
                    Dim FrequenciesToPreCalculate As List(Of Double) = InputSound.FFT.BinIndexToFrequencyList()
                    CurrentIsoPhonFilter = New Audio.DSP.IsoPhonFilter(FrequenciesToPreCalculate, 1)
                    'SetIsoPhonFilter.ExportSplToPhonData()
                End If

                'Creating an auditory filter instance
                If CurrentAuditoryFilters Is Nothing Then CurrentAuditoryFilters = New AuditoryFilters(CurrentBandTemplateList)

                'Creating a spread of masking filter instance
                If CurrentSpreadOfMaskingFilters Is Nothing Then CurrentSpreadOfMaskingFilters = New SpreadOfMaskingFilters(CurrentBandTemplateList.CentreFrequencies, CurrentBandTemplateList.CentreFrequenciesTo20k)

                'Creating a temporary re-usable matrix containing "columns" of bark filter intensities for upward spread of masking filterring
                'Each index in TempSUMFrequencyArray contains bark band nr n, and the indices within that band contain the contribution of bark band m to bark band n
                Dim TempSUMFrequencyArray(CurrentBandTemplateList.CentreFrequencies.Count - 1)() As Double
                For n = 0 To TempSUMFrequencyArray.Length - 1
                    Dim TempFilterResultArray(CurrentBandTemplateList.CentreFrequencies.Count - 1) As Double

                    'If spread of masking filterring uses levels, each value in this array need to be set to a value lower than the lowest level. If intensities are used, a default value of 0 is good.
                    For i = 0 To TempFilterResultArray.Length - 1
                        'Filling up each array with Double.MinValue, since we can have negative numbers that are the maximum SPL
                        TempFilterResultArray(i) = Double.MinValue
                    Next
                    TempSUMFrequencyArray(n) = TempFilterResultArray
                Next

                'Analysing all time windows
                For TimeWindowIndex = 0 To InputSound.FFT.windowCount(Channel) - 1

                    MyDetailedList.Add(TimeWindowIndex, New List(Of BarkBand))

                    'Creating Bark bands
                    For n = 0 To CurrentBandTemplateList.Count - 1
                        MyDetailedList(TimeWindowIndex).Add(New BarkBand(CurrentBandTemplateList, n,
                                        InputSound.FFT.PowerSpectrumData(Channel, TimeWindowIndex), CurrentIsoPhonFilter, CurrentAuditoryFilters,
                                       InputSound.WaveFormat, InputSound.FFT.fftFormat, dbFSToSplDifference))
                    Next

                    'Applying spread of masking across Bark bands
                    For BarkBandIndex = 0 To MyDetailedList(TimeWindowIndex).Count - 1
                        Dim CurrentFilter As SpreadOfMaskingFilters.SpreadOfMaskingFilter = CurrentSpreadOfMaskingFilters.GetFilter(BarkBandIndex, MyDetailedList(TimeWindowIndex)(BarkBandIndex).IsoPhonFilterredBandLoudnessLevel) 'Should we use filterred IsoPhonFilterredLinearBandPower or non-filterred BandSIL here? Does an inaudible base sound mask higher frequenies?)

                        For FilterBarkBandIndex = CurrentFilter.LowestIndex To CurrentFilter.LowestIndex + CurrentFilter.FilterArray.Length - 1
                            TempSUMFrequencyArray(FilterBarkBandIndex)(BarkBandIndex) = CurrentFilter.FilterArray(FilterBarkBandIndex - CurrentFilter.LowestIndex) * MyDetailedList(TimeWindowIndex)(BarkBandIndex).IsoPhonFilterredBandLoudnessLevel
                        Next
                    Next

                    Dim TimeWindowBarkBandArray(CurrentBandTemplateList.Count - 1) As Single
                    For BarkBandIndex = 0 To CurrentBandTemplateList.Count - 1

                        'Storing the spread of masking data for the current band (i.e. the loudest contribution from any band)
                        MyDetailedList(TimeWindowIndex)(BarkBandIndex).BandLoudnessLevel = TempSUMFrequencyArray(BarkBandIndex).Max

                        'Limiting to audible range
                        If MyDetailedList(TimeWindowIndex)(BarkBandIndex).BandLoudnessLevel < 0 Then MyDetailedList(TimeWindowIndex)(BarkBandIndex).BandLoudnessLevel = 0

                        'Converting to band loudness
                        MyDetailedList(TimeWindowIndex)(BarkBandIndex).ConvertToBandLoudness(LoudnessFunction)

                        'Doing Sone Scaling
                        MyDetailedList(TimeWindowIndex)(BarkBandIndex).BandLoudness /= SoneScalingFactor

                        'Summing the Bark band values to a new array of single
                        TimeWindowBarkBandArray(BarkBandIndex) = MyDetailedList(TimeWindowIndex)(BarkBandIndex).BandLoudness 'Here it's possible to select desired output value

                    Next

                    'Adding the Bark band values to Me
                    Me.Add(TimeWindowIndex, TimeWindowBarkBandArray)

                Next

                If AverageSpectrumIntoFirstIndex = True Then

                    Dim AveragingStartIndex As Integer = Int(AverageingStartMargin * Me.Count)
                    Dim AveragingLength As Integer = Me.Count - AveragingStartIndex - Int(AverageingEndMargin * Me.Count)
                    Dim TimeAverageArray(CurrentBandTemplateList.Count - 1) As Single

                    'Going through each Bark bin
                    For BarkBandIndex = 0 To CurrentBandTemplateList.Count - 1
                        Dim BarkBandList As New List(Of Double)
                        'Going through each time window and stores their average in TimeAverageArray
                        For TimeWindowIndex = AveragingStartIndex To AveragingStartIndex + AveragingLength
                            BarkBandList.Add(Me(TimeWindowIndex)(BarkBandIndex))
                        Next
                        TimeAverageArray(BarkBandIndex) = BarkBandList.Average
                    Next

                    'Clearing Me, and putting the averaged values at index 0
                    Me.Clear()
                    Me.Add(0, TimeAverageArray)

                End If


            End Sub

            <Serializable>
            Public Class BandTemplateList
                Inherits List(Of BandTemplate)

                Public ReadOnly CentreFrequencies As SortedSet(Of Double)
                Public ReadOnly CentreFrequenciesTo20k As SortedSet(Of Double)

                Public Sub New(ByVal FilterOverlapRatio As Double,
                               ByVal LowestIncludedFrequency As Double,
                               ByVal HighestIncludedFrequency As Double,
                               ByRef CurrentWaveFormat As Audio.Formats.WaveFormat,
                               ByRef CurrentFftFormat As Audio.Formats.FftFormat)

                    'Creating a list of included filter centre frequencies
                    CentreFrequencies = GetBarkFilterCentreFrequencies(FilterOverlapRatio, LowestIncludedFrequency, HighestIncludedFrequency, True)

                    'Creating template for Bark bands
                    For n = 0 To CentreFrequencies.Count - 1
                        Me.Add(New BandTemplate(CentreFrequencies(n), CurrentWaveFormat, CurrentFftFormat))
                    Next

                    'Creating a list of included filter centre frequencies all the way to 20 kHz
                    CentreFrequenciesTo20k = GetBarkFilterCentreFrequencies(FilterOverlapRatio, LowestIncludedFrequency, 20000, True)

                End Sub

                <Serializable>
                Public Class BandTemplate

                    Public ReadOnly CentreFrequency As Double
                    Public ReadOnly BandWidth As Double
                    Public ReadOnly LowerFrequencyLimit As Double
                    Public ReadOnly HighestFrequencyLimit As Double
                    Public ReadOnly LowestIncludedFftBinIndex As Integer
                    Public ReadOnly HighestIncludedFftBinIndex As Integer
                    Public ReadOnly CentreFftBinIndex As Integer
                    Public ReadOnly CentreBandBin As Integer
                    'Public ReadOnly BandWidthBinCount As Integer

                    ''' <summary>
                    ''' Contains the frequencies represented by the fft bins LowestIncludedFftBinIndex to HighestIncludedFftBinIndex in the current instance.
                    ''' </summary>
                    Public ReadOnly Frequencies As New List(Of Double)

                    Public Sub New(ByRef CentreFrequency As Double, ByRef CurrentWaveFormat As Audio.Formats.WaveFormat,
                                   ByRef CurrentFftFormat As Audio.Formats.FftFormat)

                        'Determining the range of the current Bark band (with the current centre frequency)
                        Me.CentreFrequency = CentreFrequency
                        Me.BandWidth = Utils.CenterFrequencyToBarkFilterBandwidth(Me.CentreFrequency)
                        Me.LowerFrequencyLimit = CentreFrequency - BandWidth 'Getting the lowest frequency that add loudness to the current critical band (This frequency is 0.5 bark below the lower cut-off frequency of the current critical band (Based on Zwicker and Fastl(1999), Phsycho-acoustics, p 164ff). (The filter approximates the auditory filter responce to noise, rather than pure tones is used.))
                        Me.HighestFrequencyLimit = CentreFrequency + BandWidth 'Getting the highest frequency that add loudness to the current critical band (This frequency is 0.5 bark above the upper cut-off frequency of the current critical band (Based on Zwicker and Fastl(1999), Phsycho-acoustics, p 164ff). (The filter approximates the auditory filter responce to noise, rather than pure tones is used.))
                        Me.LowestIncludedFftBinIndex = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                                                                            LowerFrequencyLimit, CurrentWaveFormat.SampleRate, CurrentFftFormat.FftWindowSize, Utils.roundingMethods.alwaysDown)
                        Me.HighestIncludedFftBinIndex = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                                                                            HighestFrequencyLimit, CurrentWaveFormat.SampleRate, CurrentFftFormat.FftWindowSize, Utils.roundingMethods.alwaysUp)
                        Me.CentreFftBinIndex = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                                                                        CentreFrequency, CurrentWaveFormat.SampleRate, CurrentFftFormat.FftWindowSize, Utils.roundingMethods.getClosestValue)
                        Me.CentreBandBin = CentreFftBinIndex - LowestIncludedFftBinIndex

                        'Limiting the lowest and highest bin indices to the valid range
                        If LowestIncludedFftBinIndex < 0 Then
                            'Setting LowestIncludedFftBinIndex to 0 and adjusting the centre band accordingly
                            CentreBandBin += LowestIncludedFftBinIndex
                            LowestIncludedFftBinIndex = 0
                        End If

                        If HighestIncludedFftBinIndex > CurrentFftFormat.FftWindowSize / 2 - 1 Then
                            HighestIncludedFftBinIndex = CurrentFftFormat.FftWindowSize / 2 - 1
                        End If

                        ''Getting the lowest and higest true Bark band bins (the ones above includes +/- 1 bandwidth around the centre frequency)
                        'Dim LowestBandBin As Integer = FftBinFrequencyConversion( FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                        'CentreFrequency - (0.5 * BandWidth), CurrentWaveFormat.SampleRate, CurrentFftFormat.FftWindowSize,  roundingMethods.alwaysDown)
                        'Dim HighestBandBin As Integer = FftBinFrequencyConversion( FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                        'CentreFrequency + (0.5 * BandWidth), CurrentWaveFormat.SampleRate, CurrentFftFormat.FftWindowSize,  roundingMethods.alwaysUp)

                        'Me.BandWidthBinCount = HighestBandBin - LowestBandBin

                        'Adding the frequencies
                        For BinIndex = LowestIncludedFftBinIndex To HighestIncludedFftBinIndex
                            Frequencies.Add(FftBinFrequencyConversion(FftBinFrequencyConversionDirection.BinIndexToFrequency,
                                                                            BinIndex, CurrentWaveFormat.SampleRate, CurrentFftFormat.FftWindowSize))
                        Next

                    End Sub
                End Class
            End Class

            <Serializable>
            Public Class BarkBand

                Public PowerSpectrum As TimeWindow

                ''' <summary>
                ''' Should hold the raw power spectrum data relevant to the current Bark band.
                ''' </summary>
                Public RawBandPowerSpectrumData As New List(Of Single)
                Public LinearBandPower As Double
                Public BandSIL As Double

                Public IsoPhonFilterredBandPowerSpectrumData() As Single
                Public IsoPhonFilterredLinearBandPower As Double
                Public IsoPhonFilterredBandLoudnessLevel As Double

                Public UsmFilterredBandPower As Double

                Public BandLoudnessLevel As Double
                Public BandLoudness As Double

                Public CentreFrequency As Double
                Public LowestIncludedFftBinIndex As Integer
                Public HighestIncludedFftBinIndex As Integer

                Public MyWaveFormat As Audio.Formats.WaveFormat
                Public MyIsoPhonFilter As Audio.DSP.IsoPhonFilter
                Public MyAuditoryFilters As AuditoryFilters

                Public dbFSToSplDifference As Double

                ''' <summary>
                ''' Contains the frequencies represented by the fft bins LowestIncludedFftBinIndex to HighestIncludedFftBinIndex in the current instance.
                ''' </summary>
                Public ReadOnly Frequencies As List(Of Double)


                ''' <summary>
                ''' 
                ''' </summary>
                ''' <param name="PowerSpectrum"></param>
                ''' <param name="SetIsoPhonFilter"></param>
                ''' <param name="SetAuditoryFilters"></param>
                ''' <param name="SetWaveFormat"></param>
                ''' <param name="SetFftFormat"></param>
                Public Sub New(ByRef MyBarkBandTemplate As BandTemplateList, ByVal TemplateIndex As Integer,
                               ByRef PowerSpectrum As TimeWindow, ByRef SetIsoPhonFilter As Audio.DSP.IsoPhonFilter, ByRef SetAuditoryFilters As AuditoryFilters,
                    ByRef SetWaveFormat As Audio.Formats.WaveFormat, ByRef SetFftFormat As Audio.Formats.FftFormat, ByVal dbFSToSplDifference As Double)


                    'Copying parameters from the band template
                    Me.CentreFrequency = MyBarkBandTemplate(TemplateIndex).CentreFrequency
                    Me.LowestIncludedFftBinIndex = MyBarkBandTemplate(TemplateIndex).LowestIncludedFftBinIndex
                    Me.HighestIncludedFftBinIndex = MyBarkBandTemplate(TemplateIndex).HighestIncludedFftBinIndex
                    Me.Frequencies = MyBarkBandTemplate(TemplateIndex).Frequencies

                    'Referencing other objects
                    Me.PowerSpectrum = PowerSpectrum
                    MyIsoPhonFilter = SetIsoPhonFilter
                    MyAuditoryFilters = SetAuditoryFilters
                    MyWaveFormat = SetWaveFormat
                    Me.dbFSToSplDifference = dbFSToSplDifference

                    'Starting the analysis of the Bark band
                    StartBarkBandAnalysis()

                End Sub

                Private Sub StartBarkBandAnalysis()

                    Me.CopyBandPowerSpectrum()
                    Me.ApplyAuditoryFilter()
                    Me.CalculateBandSoundIntensityLevel()
                    Me.CalculateIsoPhonFilterredBandSpectrum()

                    'Here follows upward spread of masking between bands performed by the calling code.

                End Sub


                '1
                Private Sub CopyBandPowerSpectrum()

                    'Collecting fft bin values within the band
                    For fb = LowestIncludedFftBinIndex To HighestIncludedFftBinIndex

                        'Adding the power data
                        RawBandPowerSpectrumData.Add(PowerSpectrum.WindowData(fb) * 2)
                    Next

                End Sub

                '2. Applying triangular filter to RawBandPowerSpectrumData
                Private Sub ApplyAuditoryFilter(Optional ByVal CompensateForFilterAttenuation As Boolean = True)

                    'Getting the appropriate filter
                    Dim CurrentAuditoryFilter = MyAuditoryFilters(Me.CentreFrequency)

                    'Applying the filter to the RawBandPowerSpectrumData
                    If CompensateForFilterAttenuation = True Then
                        For k = 0 To RawBandPowerSpectrumData.Count - 1
                            RawBandPowerSpectrumData(k) *= CurrentAuditoryFilter.FilterArray(k) * CurrentAuditoryFilter.InverseFilterAttenuation
                        Next
                    Else
                        For k = 0 To RawBandPowerSpectrumData.Count - 1
                            RawBandPowerSpectrumData(k) *= CurrentAuditoryFilter.FilterArray(k)
                        Next
                    End If

                End Sub

                '3. Calculating band intensity, in order to use as filter level when doing Iso-phon filterring later
                Private Sub CalculateBandSoundIntensityLevel()

                    LinearBandPower = Math.Max(Double.Epsilon, RawBandPowerSpectrumData.Sum) 'Double.Epsilon to avoid log10 of 0

                    'Getting the total power of the bark band
                    Dim BandLevelIn_dBFS As Double = 10 * Math.Log10(LinearBandPower / MyWaveFormat.PositiveFullScale)
                    BandSIL = (BandLevelIn_dBFS + dbFSToSplDifference)

                End Sub

                '4 Applying Iso-phon filter to the raw band spectrum
                Public Sub CalculateIsoPhonFilterredBandSpectrum()

                    Dim ResultArray(RawBandPowerSpectrumData.Count - 1) As Single

                    For k = 0 To ResultArray.Length - 1

                        'RawBandPowerSpectrumData to dB FS and stores it in ResultArray
                        ResultArray(k) = 10 * Math.Log10(RawBandPowerSpectrumData(k) / MyWaveFormat.PositiveFullScale)

                        'Looking up the attenuation of each fft bin frequency
                        Dim CurrentAttenuation As Double = MyIsoPhonFilter.GetAttenuation(BandSIL, Frequencies(k))

                        'Applying attenuation
                        ResultArray(k) -= CurrentAttenuation

                        'Converting back to linear scale
                        ResultArray(k) = MyWaveFormat.PositiveFullScale * 10 ^ (ResultArray(k) / 10)

                        'This code could be optimized by converting attenuation to a factor to multiplicate with the intensity data, and thereby skip the dB-conversion

                    Next

                    'Storing the result
                    IsoPhonFilterredBandPowerSpectrumData = ResultArray

                    'Calculating pre-spread-of masking band loudness level
                    IsoPhonFilterredLinearBandPower = Math.Max(Double.Epsilon, IsoPhonFilterredBandPowerSpectrumData.Sum) 'Double.Epsilon to avoid log10 of 0

                    'Converting to band loudness level
                    IsoPhonFilterredBandLoudnessLevel = 10 * Math.Log10(IsoPhonFilterredLinearBandPower / MyWaveFormat.PositiveFullScale) + dbFSToSplDifference

                End Sub


                '5. Spread of masking

                '6. Summing


                'Private Sub CalculateBandSoundIntensityLevel_OLD()

                '    'Summing the bin values withing +/- 0.5 bandwidths from the centre frequency
                '    Dim LowestBandBin As Integer = CentreBandBin - Rounding(BandWidthBinCount / 2,  roundingMethods.alwaysDown)
                '    Dim HighestBandBin As Integer = CentreBandBin + Rounding(BandWidthBinCount / 2,  roundingMethods.alwaysUp)

                '    'Resetting LinearBandPower
                '    LinearBandPower = Double.Epsilon 'Setting it to Epsilon to avoid log10(0) below.

                '    'Summing the power within the Bark band
                '    For k = LowestBandBin To HighestBandBin
                '        'The following two lines could be optimized by pre calculating these limits in the band template
                '        If k < 0 Then Continue For
                '        If k > RawBandPowerSpectrumData.Count - 1 Then Continue For

                '        LinearBandPower += RawBandPowerSpectrumData(k)
                '    Next

                '    'Getting the total power of the bark band
                '    BandLevelIn_dBFS = 10 * Math.Log10(LinearBandPower / MyWaveFormat.PositiveFullScale)
                '    BandSIL = (BandLevelIn_dBFS + dbFSToSplDifference)

                'End Sub


                'Private Sub ApplyAuditoryFilter_OLD(Optional ByVal CompensateForFilterAttenuation As Boolean = False)

                '    'Getting the appropriate filter
                '    Dim CurrentAuditoryFilter = MyAuditoryFilters(Me.CentreFrequency)

                '    'Applying the filter to the FilterredBandPowerSpectrumData
                '    If CompensateForFilterAttenuation = True Then
                '        For k = 0 To FilterredBandPowerSpectrumData.Length - 1
                '            FilterredBandPowerSpectrumData(k) *= CurrentAuditoryFilter.FilterArray(k) * CurrentAuditoryFilter.InverseFilterAttenuation
                '        Next
                '    Else
                '        For k = 0 To FilterredBandPowerSpectrumData.Length - 1
                '            FilterredBandPowerSpectrumData(k) *= CurrentAuditoryFilter.FilterArray(k)
                '        Next
                '    End If

                'End Sub

                'Private Sub SumAndConvertToLoudnessLevel()

                '    'Summing filterred band power
                '    FilterredLinearBandPower = FilterredBandPowerSpectrumData.Sum

                '    'Converting to dB FS
                '    FilterredBandLevelIn_dBFS = 10 * Math.Log10(FilterredLinearBandPower / MyWaveFormat.PositiveFullScale)

                '    'Converting to Phon (dBSIL equivalent at 1kHz)
                '    FilterredBandLoudnessLevel = FilterredBandLevelIn_dBFS + dbFSToSplDifference

                '    'Limiting to audible range
                '    If FilterredBandLoudnessLevel < 0 Then FilterredBandLoudnessLevel = 0


                '    'A test to go directly to loudness level from filterred band intensity. Couldn't solve the hearing threshold properly though...
                '    'Dim Lin_dBEq As Double = 10 ^ -12 * 10 ^ (dbFSToSplDifference / 10)
                '    'Dim Lin_Threshold As Double = (10 ^ -12 * 10 ^ (0 / 10)) * Lin_dBEq
                '    'Dim I As Double = FilterredLinearBandPower * Lin_dBEq
                '    'I = Math.Max(Lin_Threshold, I)
                '    'Dim Test_L As Double = 251.188643150958 * I ^ 0.3 'The constant is calculated to give 40 db SPL a loudness of 1 Sone 
                '    'FilterredBandSone = Test_L
                '    'Exit sub

                'End Sub

                'InEx function Buus & Florentine 2002
                Private L As Double = BandLoudnessLevel
                Private f1 As Double = 1.7058 * 10 ^ -9
                Private f2 As Double = 6.587 * 10 ^ -7
                Private f3 As Double = 9.7515 * 10 ^ -5
                Private f4 As Double = 6.6964 * 10 ^ -3
                Private f5 As Double = 0.2376
                Private f6 As Double = 3.4831
                Private S0 As Double = 1


                Public Sub ConvertToBandLoudness(ByRef LoudnessFunction As LoudnessFunctions)

                    'Converting to Sone
                    Select Case LoudnessFunction
                        Case LoudnessFunctions.InExType
                            'InEx function Buus & Florentine 2002 (see constants above function code)
                            BandLoudness = S0 * 10 ^ (f1 * L ^ 5 - f2 * L ^ 4 + f3 * L ^ 3 - f4 * L ^ 2 + f5 * L - f6)
                        Case LoudnessFunctions.ZwickerType
                            If BandLoudnessLevel < 40 Then
                                BandLoudness = 10 ^ (Math.Log10(BandLoudnessLevel / 40) / 0.35)
                            Else
                                BandLoudness = 2 ^ ((BandLoudnessLevel - 40) / 10)
                            End If

                        Case LoudnessFunctions.Simple
                            BandLoudness = 2 ^ ((BandLoudnessLevel - 40) / 10)

                    End Select

                End Sub

                Public Sub ConvertToBandLoudnessLevel()

                    'Converting to log base 2
                    BandLoudness = 40 + 10 * Utils.getBase_n_Log(BandLoudness, 2)

                End Sub

            End Class
        End Class

        <Serializable>
        Public Class AuditoryFilters
            Inherits SortedList(Of Double, AuditoryFilter) 'Bark band centre frequency, Filter

            Public Sub New(ByRef MyBarkBandTemplateList As BarkSpectrum.BandTemplateList)

                'Populating Me by the needed filters
                For n = 0 To MyBarkBandTemplateList.Count - 1 'FilterCentreFrequencies.Count - 1
                    Me.Add(MyBarkBandTemplateList(n).CentreFrequency, CreateTriangularFilter(MyBarkBandTemplateList(n)))
                Next

            End Sub

            Private Function CreateTriangularFilter(ByRef CurrentBarkBandTemplate As BarkSpectrum.BandTemplateList.BandTemplate) As AuditoryFilter

                Dim OutputFilter As New AuditoryFilter

                Dim NewFilterArray(CurrentBarkBandTemplate.HighestIncludedFftBinIndex - CurrentBarkBandTemplate.LowestIncludedFftBinIndex) As Single

                'Adding values to the lower part of the filter array
                Dim FirstRectangleWidth As Integer = CurrentBarkBandTemplate.CentreBandBin + 1
                Dim InverseFirstRectangleWidth As Double = 1 / FirstRectangleWidth
                For k = 1 To CurrentBarkBandTemplate.CentreBandBin
                    NewFilterArray(k - 1) = k * InverseFirstRectangleWidth
                Next

                'Adding values to the upper part of the filter array
                Dim SecondRectangleWidth As Integer = NewFilterArray.Length - CurrentBarkBandTemplate.CentreBandBin
                Dim InverseSecondRectangleWidth As Double = 1 / SecondRectangleWidth
                For k = CurrentBarkBandTemplate.CentreBandBin + 1 To NewFilterArray.Length
                    NewFilterArray(k - 1) = InverseSecondRectangleWidth * (SecondRectangleWidth - (k - FirstRectangleWidth))
                Next

                'Storing the filter array
                OutputFilter.FilterArray = NewFilterArray

                'Calculating the filter attenuation
                OutputFilter.FilterAttenuation = 2 * OutputFilter.FilterArray.Sum / OutputFilter.FilterArray.Length
                OutputFilter.InverseFilterAttenuation = 1 / OutputFilter.FilterAttenuation

                Return OutputFilter

            End Function

            <Serializable>
            Public Class AuditoryFilter

                Public FilterArray() As Single
                Public FilterAttenuation As Double
                Public InverseFilterAttenuation As Double

            End Class

        End Class

        Public Class SpreadOfMaskingFilters

            Private MyFilters As New SortedList(Of Integer, SortedList(Of Integer, SpreadOfMaskingFilter)) 'Bark band index, Level, Filter
            Private BarkBandCentreFrequencies As SortedSet(Of Double)
            Private CentreFrequenciesTo20k As SortedSet(Of Double)


            Public Sub New(ByRef BarkBandCentreFrequencies As SortedSet(Of Double), ByRef CentreFrequenciesTo20k As SortedSet(Of Double)) ', Optional ByVal LookupLevelInterval As Integer = 5)

                Me.BarkBandCentreFrequencies = BarkBandCentreFrequencies

                'Populating Me by the needed centre frequencies
                For n = 0 To BarkBandCentreFrequencies.Count - 1
                    MyFilters.Add(n, New SortedList(Of Integer, SpreadOfMaskingFilter))
                Next

                'Referencing CentreFrequenciesTo20k
                Me.CentreFrequenciesTo20k = CentreFrequenciesTo20k

            End Sub

            Public Function GetFilter(ByRef CurrentBarkBandIndex As Integer, ByVal BandLevel As Double) As SpreadOfMaskingFilter

                Dim RoundedBandLevel As Integer = Math.Round(BandLevel)

                'Creating the needed filter if it doesn't already exists
                If Not MyFilters(CurrentBarkBandIndex).ContainsKey(RoundedBandLevel) Then
                    CreateNewFilter(CurrentBarkBandIndex, RoundedBandLevel)
                End If

                Return MyFilters(CurrentBarkBandIndex)(RoundedBandLevel)

            End Function

            Private Sub CreateNewFilter(ByRef CurrentBarkBandIndex As Integer, ByVal RoundedBandLevel As Double)

                'Creating a triangular filter independent of level
                Dim NewFilter As New SpreadOfMaskingFilter

                'Setting a defualt spreading factor
                Dim DefaultSpreadingFactor As Double = 1.5

                'Calculating the amount of downward spread
                Dim CurrentCentreFrequency = BarkBandCentreFrequencies(CurrentBarkBandIndex)
                Dim LowestIncludedIndex As Integer = Utils.GetNearestIndex(CurrentCentreFrequency / DefaultSpreadingFactor, BarkBandCentreFrequencies, False)

                'Calculating the amount of upward spread
                Dim DefaultHighestIncludedIndex As Integer = Utils.GetNearestIndex(CurrentCentreFrequency * DefaultSpreadingFactor, CentreFrequenciesTo20k, True)

                'VirtualHighestIncludedIndex is used to estimate the filter attenuation rate, where the highest possible end point is 20 kHz
                Dim RemainingLength As Integer = CentreFrequenciesTo20k.Count - CurrentBarkBandIndex
                Dim VirtualHighestIncludedIndex As Integer = Math.Max(DefaultHighestIncludedIndex, CurrentBarkBandIndex + Int((RemainingLength) * (RoundedBandLevel / 90)))

                'RealHighestIncludedIndex as an end point for filter array iterattion.
                Dim RealHighestIncludedIndex As Integer = Math.Min(BarkBandCentreFrequencies.Count - 1, VirtualHighestIncludedIndex)

                'Creating and storing the filter array
                Dim NewFilterArray(RealHighestIncludedIndex - LowestIncludedIndex) As Double
                NewFilter.FilterArray = NewFilterArray

                Dim FilterArrayCentreIndex As Integer = CurrentBarkBandIndex - LowestIncludedIndex

                'Adding values to the lower part of the filter array
                Dim FirstRectangleWidth As Integer = FilterArrayCentreIndex + 1
                Dim InverseFirstRectangleWidth As Double = 1 / FirstRectangleWidth
                For k = 1 To FilterArrayCentreIndex
                    NewFilterArray(k - 1) = k * InverseFirstRectangleWidth

                    'NewFilterArray(k - 1) = (10 ^ (k * InverseFirstRectangleWidth)) / 10
                Next

                'Adding values to the upper part of the filter array
                Dim SecondRectangleWidth As Integer = VirtualHighestIncludedIndex - LowestIncludedIndex - FilterArrayCentreIndex + 1
                Dim InverseSecondRectangleWidth As Double = 1 / SecondRectangleWidth
                For k = FilterArrayCentreIndex + 1 To NewFilterArray.Length
                    NewFilterArray(k - 1) = (InverseSecondRectangleWidth * (SecondRectangleWidth - (k - FirstRectangleWidth)))

                    'NewFilterArray(k - 1) = (10 ^ (InverseSecondRectangleWidth * (SecondRectangleWidth - (k - FirstRectangleWidth)))) / 10
                Next

                'Calculating the filter start/lowest index
                NewFilter.LowestIndex = LowestIncludedIndex

                MyFilters(CurrentBarkBandIndex).Add(RoundedBandLevel, NewFilter)

            End Sub



            Public Class SpreadOfMaskingFilter
                Public FilterArray() As Double
                ''' <summary>
                ''' Holds the index of the lowest Bark band that should be filterred by the current filter.
                ''' </summary>
                Public LowestIndex As Integer
            End Class

        End Class

        ''' <summary>
        ''' Calculates the spectrum level/power based on the current frequency domain data.
        ''' </summary>
        ''' <param name="channel"></param>
        ''' <param name="windowNumber"></param>
        ''' <param name="LowerInclusiveLimit">Lower inclusive fft bin / frequency.</param>
        ''' <param name="UpperInclusiveLimit">Highest inclusive fft bin / frequency.</param>
        ''' <param name="FftFormat">The FftFormat used to create the fft data.</param>
        ''' <param name="InputType"></param>
        ''' <param name="OutputType"></param>
        ''' <param name="ActualLowerInclusiveFrequency">Returns the centre frequency of the actual lowest fft bin used.</param>
        ''' <param name="ActualUpperInclusiveFrequency">Returns the centre frequency of the actual highest fft bin used.</param>
        ''' <returns></returns>
        <Obsolete>
        Public Function GetSpectrumLevel_OLD(ByVal channel As Integer, ByVal windowNumber As Integer,
                                         ByVal FftFormat As Audio.Formats.FftFormat, ByVal SoundFormat As Audio.Formats.WaveFormat,
                                         Optional ByVal LowerInclusiveLimit As Single? = Nothing,
                                         Optional ByVal UpperInclusiveLimit As Single? = Nothing,
                                         Optional ByRef InputType As GetSpectrumLevel_InputType = GetSpectrumLevel_InputType.FftBinIndex,
                                         Optional ByRef OutputType As GetSpectrumLevel_OutputType = GetSpectrumLevel_OutputType.SpectrumLevel_dB,
                                         Optional ByRef ActualLowerInclusiveFrequency As Single? = Nothing,
                                         Optional ByRef ActualUpperInclusiveFrequency As Single? = Nothing) As Double

            Try

                'Converting frequency to fft bins
                If InputType = GetSpectrumLevel_InputType.FftBinCentreFrequency_Hz Then
                    'Setting default UpperInclusiveFrequency
                    If LowerInclusiveLimit Is Nothing Then LowerInclusiveLimit = 0
                    If UpperInclusiveLimit Is Nothing Then UpperInclusiveLimit = SoundFormat.SampleRate / 2 - 1

                    'Converting frequencies to bin values
                    LowerInclusiveLimit = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, LowerInclusiveLimit,
                                                                         SoundFormat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.alwaysDown)
                    UpperInclusiveLimit = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex, UpperInclusiveLimit,
                                                                         SoundFormat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.alwaysUp)
                End If


                'Checking if power spectrum has been calculated
                If _PowerSpectrumData(channel - 1) Is Nothing Then
                    'Calculating power data
                    CalculatePowerSpectrum()
                End If

                'Copying the power spectrum values (so they don't get overwritten)
                Dim TempPowerSpectrumData(PowerSpectrumData(channel, windowNumber).WindowData.Length - 1) As Single
                For n = 0 To TempPowerSpectrumData.Length - 1
                    TempPowerSpectrumData(n) = PowerSpectrumData(channel, windowNumber).WindowData(n)
                Next

                'Setting default input values
                If LowerInclusiveLimit Is Nothing Then LowerInclusiveLimit = 1
                If UpperInclusiveLimit Is Nothing Then UpperInclusiveLimit = TempPowerSpectrumData.Length / 2

                'Restricting the values of LowerInclusiveBin and UpperInclusiveBin to their valid ranges
                If LowerInclusiveLimit < 1 Then LowerInclusiveLimit = 1
                If UpperInclusiveLimit > TempPowerSpectrumData.Length / 2 - 1 Then UpperInclusiveLimit = TempPowerSpectrumData.Length / 2 - 1

                'Adjusting for Equivalent Noise Bandwidth
                If FftFormat.WindowingType <> WindowingType.Rectangular Then

                    Dim ENB As Double = GetEquivalentNoiseBandwidth(FftFormat.FftWindowSize, FftFormat.WindowingType, FftFormat.Tukey_r)
                    For n = LowerInclusiveLimit To UpperInclusiveLimit

                        'Adjusting the positive side
                        TempPowerSpectrumData(n) /= ENB

                        'Adjusting the negative side
                        TempPowerSpectrumData(TempPowerSpectrumData.Length - n) /= ENB
                    Next
                End If

                'Compensating for the scaling introduced by the windowing function
                If FftFormat.WindowingType <> WindowingType.Rectangular Then

                    Dim ISF As Double = GetInverseWindowingScalingFactor(TempPowerSpectrumData.Length, FftFormat.WindowingType)
                    For n = LowerInclusiveLimit To UpperInclusiveLimit

                        'Adjusting the positive side
                        TempPowerSpectrumData(n) *= ISF ^ 2

                        'Adjusting the negative side
                        TempPowerSpectrumData(TempPowerSpectrumData.Length - n) *= ISF ^ 2
                    Next
                End If

                'Summing spectral power
                Dim SummedBinPowers As Double = 0
                For n = LowerInclusiveLimit To UpperInclusiveLimit

                    'Summing positive and negative frequency values to one side
                    TempPowerSpectrumData(n) += TempPowerSpectrumData(TempPowerSpectrumData.Length - n)
                    SummedBinPowers += (TempPowerSpectrumData(n))

                Next

                'Calculating the actual cut-off band centre frequencies used
                ActualLowerInclusiveFrequency = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.BinIndexToFrequency, LowerInclusiveLimit,
                                                                         SoundFormat.SampleRate, FftFormat.FftWindowSize)
                ActualUpperInclusiveFrequency = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.BinIndexToFrequency, UpperInclusiveLimit,
                                                                         SoundFormat.SampleRate, FftFormat.FftWindowSize)


                'Selecting output unit
                Select Case OutputType
                    Case GetSpectrumLevel_OutputType.SpectrumLevel_dB

                        'Converting to dB
                        Return 10 * Math.Log10(SummedBinPowers / SoundFormat.PositiveFullScale) 'TODO: check if it's right to use PositiveFullScale even for non floats here?

                    Case GetSpectrumLevel_OutputType.SpectrumPower_Linear

                        Return SummedBinPowers

                    Case Else
                        Throw New NotImplementedException()
                End Select

            Catch ex As Exception
                MsgBox(ex.ToString)
                Return Nothing
            End Try

        End Function


        ''' <summary>
        ''' Using information stored in the magnitude (or power) and phase properties to transform the equivalent data to rectangular form (overwriting any data stored in the rectangular form arrays)
        ''' <param name="UsePowerData">If set to true, power data is used instead of magnitude data.</param>
        ''' </summary>
        Public Sub CalculateRectangualForm(Optional ByVal UsePowerData As Boolean = False)

            Try

                If UsePowerData = False Then

                    'Testing that both magnitude and phase data exist
                    For channel = 0 To Waveformat.Channels - 2
                        If _AmplitudeSpectrum(channel).Count = 0 Then
                            MsgBox("Cannot convert fft polar data to rectangular form. No magnitude data exists.")
                            Exit Sub
                        End If
                        If _PhaseSpectrum(channel).Count = 0 Then
                            MsgBox("Cannot convert fft polar data to rectangular form. No phase data exists.")
                            Exit Sub
                        End If
                    Next

                    'Resetting channel real and imaginary data
                    For channel = 0 To _FrequencyDomainRealData.Count - 1
                        _FrequencyDomainRealData(channel).Clear()
                    Next
                    For channel = 0 To _FrequencyDomainImaginaryData.Count - 1
                        _FrequencyDomainImaginaryData(channel).Clear()
                    Next


                    'Calculating rectangular data
                    For channel = 0 To Me._AmplitudeSpectrum.Count - 1
                        Dim NewChannelRealData As New List(Of TimeWindow)
                        Dim NewChannelImaginaryData As New List(Of TimeWindow)
                        For window = 0 To Me._AmplitudeSpectrum(channel).Count - 1
                            Dim NewRealTimeWindow As New TimeWindow
                            Dim NewWindowRealData(FftFormat.FftWindowSize - 1) As Double
                            NewRealTimeWindow.WindowData = NewWindowRealData

                            Dim NewImaginaryTimeWindow As New TimeWindow
                            Dim NewWindowImaginaryData(FftFormat.FftWindowSize - 1) As Double
                            NewImaginaryTimeWindow.WindowData = NewWindowImaginaryData

                            GetRectangualForm_UsingMagnitudeAndPhase(NewWindowRealData, NewWindowImaginaryData, _AmplitudeSpectrum(channel)(window).WindowData, _PhaseSpectrum(channel)(window).WindowData)

                            'Copying the window descriptions from the magnitude data
                            NewRealTimeWindow.WindowingType = _AmplitudeSpectrum(channel)(window).WindowingType
                            NewImaginaryTimeWindow.WindowingType = NewRealTimeWindow.WindowingType

                            NewRealTimeWindow.ZeroPadding = _AmplitudeSpectrum(channel)(window).ZeroPadding
                            NewImaginaryTimeWindow.ZeroPadding = NewRealTimeWindow.ZeroPadding

                            'Adding the new time windows
                            NewChannelRealData.Add(NewRealTimeWindow)
                            NewChannelImaginaryData.Add(NewImaginaryTimeWindow)
                        Next
                        _FrequencyDomainRealData(channel) = NewChannelRealData
                        _FrequencyDomainImaginaryData(channel) = NewChannelImaginaryData
                    Next

                Else

                    'Testing that both power and phase data exist
                    For channel = 0 To Waveformat.Channels - 2
                        If _PowerSpectrumData(channel).Count = 0 Then
                            MsgBox("Cannot convert fft polar data to rectangular form. No power data exists.")
                            Exit Sub
                        End If
                        If _PhaseSpectrum(channel).Count = 0 Then
                            MsgBox("Cannot convert fft polar data to rectangular form. No phase data exists.")
                            Exit Sub
                        End If
                    Next

                    'Resetting channel real and imaginary data
                    For channel = 0 To _FrequencyDomainRealData.Count - 1
                        _FrequencyDomainRealData(channel).Clear()
                    Next
                    For channel = 0 To _FrequencyDomainImaginaryData.Count - 1
                        _FrequencyDomainImaginaryData(channel).Clear()
                    Next


                    'Calculating rectangular data
                    For channel = 0 To Me._PowerSpectrumData.Count - 1
                        Dim NewChannelRealData As New List(Of TimeWindow)
                        Dim NewChannelImaginaryData As New List(Of TimeWindow)
                        For window = 0 To Me._PowerSpectrumData(channel).Count - 1
                            Dim NewRealTimeWindow As New TimeWindow
                            Dim NewWindowRealData(FftFormat.FftWindowSize - 1) As Double
                            NewRealTimeWindow.WindowData = NewWindowRealData

                            Dim NewImaginaryTimeWindow As New TimeWindow
                            Dim NewWindowImaginaryData(FftFormat.FftWindowSize - 1) As Double
                            NewImaginaryTimeWindow.WindowData = NewWindowImaginaryData

                            GetRectangualForm_UsingPowerAndPhase(NewWindowRealData, NewWindowImaginaryData, _PowerSpectrumData(channel)(window).WindowData, _PhaseSpectrum(channel)(window).WindowData)

                            'Copying the window descriptions from the power data
                            NewRealTimeWindow.WindowingType = _PowerSpectrumData(channel)(window).WindowingType
                            NewImaginaryTimeWindow.WindowingType = NewRealTimeWindow.WindowingType

                            NewRealTimeWindow.ZeroPadding = _PowerSpectrumData(channel)(window).ZeroPadding
                            NewImaginaryTimeWindow.ZeroPadding = NewRealTimeWindow.ZeroPadding

                            'Adding the new time windows
                            NewChannelRealData.Add(NewRealTimeWindow)
                            NewChannelImaginaryData.Add(NewImaginaryTimeWindow)
                        Next
                        _FrequencyDomainRealData(channel) = NewChannelRealData
                        _FrequencyDomainImaginaryData(channel) = NewChannelImaginaryData
                    Next


                End If

            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try

        End Sub

        <Obsolete>
        Public Sub GetPhases(ByRef phaseSpectrumArray() As Single, ByRef x() As Single, ByRef y() As Single)

            'Converts to polar form
            For coeffN = 0 To x.Length - 1
                phaseSpectrumArray(coeffN) = Math.Atan2(y(coeffN), x(coeffN))
            Next

        End Sub

        <Obsolete>
        Public Sub GetMagnitudes(ByRef magnitudeSpectrumArray() As Single, ByRef x() As Single, ByRef y() As Single)

            'Converts to polar form
            For coeffN = 0 To x.Length - 1
                magnitudeSpectrumArray(coeffN) = Math.Sqrt(x(coeffN) ^ 2 + y(coeffN) ^ 2)
            Next

        End Sub

        <Obsolete>
        Public Sub GetPowers(ByRef powerSpectrumArray() As Single, ByRef x() As Single, ByRef y() As Single)

            'Converts to polar form
            For coeffN = 0 To x.Length - 1
                powerSpectrumArray(coeffN) = x(coeffN) ^ 2 + y(coeffN) ^ 2
            Next

        End Sub

        ''' <summary>
        ''' Calculates the spectrum of a specified time window in the specified channel.
        ''' </summary>
        ''' <param name="channel"></param>
        ''' <param name="windowNumber"></param>
        ''' <param name="SpectrumType"></param>
        ''' <param name="CompensateForEquivalentNoiseBandwidthScaling"></param>
        ''' <param name="CompensateForZeroPaddingScaling"></param>
        ''' <param name="CompensateForTimeWindowingScaling"></param>
        ''' <param name="EquivalentNoiseBandwidth"></param>
        ''' <param name="InverseTimeWindowingScalingFactor"></param>
        ''' <returns></returns>
        Public Function GetTimeWindowSpectrum(ByVal channel As Integer, ByVal windowNumber As Integer,
                                     Optional ByVal SpectrumType As SpectrumTypes = SpectrumTypes.AmplitudeSpectrum,
                                     Optional ByVal CompensateForEquivalentNoiseBandwidthScaling As Boolean = True,
                                     Optional ByVal CompensateForZeroPaddingScaling As Boolean = True,
                                     Optional ByVal CompensateForTimeWindowingScaling As Boolean = True,
                                     Optional ByRef EquivalentNoiseBandwidth As Double? = Nothing,
                                     Optional ByRef InverseTimeWindowingScalingFactor As Double? = Nothing,
                                              Optional ByVal ZeroPaddingCompensationLowerLimit As Double? = Nothing) As Double()

            'This sub could be optimized by an option to skip calculation of the negative frequency side.

            Dim TempSpectrumArray(FrequencyDomainRealData(channel, windowNumber).WindowData.Length - 1) As Double

            'Converts to polar form
            Select Case SpectrumType
                Case SpectrumTypes.PowerSpectrum, SpectrumTypes.AmplitudeSpectrum
                    For coeffN = 0 To FrequencyDomainRealData(channel, windowNumber).WindowData.Length - 1
                        TempSpectrumArray(coeffN) = _FrequencyDomainRealData(channel - 1)(windowNumber).WindowData(coeffN) ^ 2 + _FrequencyDomainImaginaryData(channel - 1)(windowNumber).WindowData(coeffN) ^ 2
                    Next

                Case SpectrumTypes.PhaseSpectrum
                    For coeffN = 0 To FrequencyDomainRealData(channel, windowNumber).WindowData.Length - 1
                        TempSpectrumArray(coeffN) = Math.Atan2(_FrequencyDomainImaginaryData(channel - 1)(windowNumber).WindowData(coeffN), _FrequencyDomainRealData(channel - 1)(windowNumber).WindowData(coeffN))
                    Next

            End Select


            'Compensations
            'Adjusting for Equivalent Noise Bandwidth
            If CompensateForEquivalentNoiseBandwidthScaling = True Then

                Select Case SpectrumType
                    Case SpectrumTypes.PowerSpectrum, SpectrumTypes.AmplitudeSpectrum  'TODO: check how this affects the amplitude spectrum!

                        'Rectagular windows have a ENB of 1, so rectangular windows are ignored
                        If FrequencyDomainRealData(channel, windowNumber).WindowingType <> WindowingType.Rectangular Then
                            'Getting the EquivalentNoiseBandwidth if not supplied by the calling code
                            If EquivalentNoiseBandwidth Is Nothing Then EquivalentNoiseBandwidth = GetEquivalentNoiseBandwidth(FftFormat.FftWindowSize, FftFormat.WindowingType, FftFormat.Tukey_r)
                            For coeffN = 0 To TempSpectrumArray.Length - 1
                                TempSpectrumArray(coeffN) *= 1 / EquivalentNoiseBandwidth
                            Next
                        End If

                    Case Else
                        'Not applicable to phase spectrum
                End Select

            End If


            'Compensating for the scaling introduced by zero padding
            If CompensateForZeroPaddingScaling = True Then

                Select Case SpectrumType
                    Case SpectrumTypes.PowerSpectrum, SpectrumTypes.AmplitudeSpectrum
                        'Dim ZeroPaddingInverseScalingFactor As Double = 1 / (FftFormat.AnalysisWindowSize / FftFormat.FftWindowSize)
                        'ZeroPaddingInverseScalingFactor will be the same within the same time window, but must be re-calculated for each window.
                        Dim ZeroPaddingInverseScalingFactor As Double
                        If ZeroPaddingCompensationLowerLimit IsNot Nothing Then
                            ZeroPaddingInverseScalingFactor = 1 / Math.Max(ZeroPaddingCompensationLowerLimit.Value * FftFormat.FftWindowSize, CDbl(((FftFormat.FftWindowSize - FrequencyDomainRealData(channel, windowNumber).ZeroPadding) / FftFormat.FftWindowSize)))
                        Else
                            ZeroPaddingInverseScalingFactor = 1 / ((FftFormat.FftWindowSize - FrequencyDomainRealData(channel, windowNumber).ZeroPadding) / FftFormat.FftWindowSize)
                        End If

                        For coeffN = 0 To TempSpectrumArray.Length - 1
                            TempSpectrumArray(coeffN) *= ZeroPaddingInverseScalingFactor
                        Next

                    Case Else
                        'Not applicable to phase spectrum
                End Select


            End If

            'Compensating for the scaling introduced by the windowing function
            If CompensateForTimeWindowingScaling = True Then

                If FftFormat.WindowingType <> WindowingType.Rectangular Then

                    Select Case SpectrumType
                        Case SpectrumTypes.PowerSpectrum, SpectrumTypes.AmplitudeSpectrum
                            If InverseTimeWindowingScalingFactor Is Nothing Then InverseTimeWindowingScalingFactor = GetInverseWindowingScalingFactor(FftFormat.FftWindowSize, FftFormat.WindowingType)
                            For coeffN = 0 To TempSpectrumArray.Length - 1
                                TempSpectrumArray(coeffN) *= InverseTimeWindowingScalingFactor ^ 2
                            Next
                        Case Else
                            'Not applicable to phase spectrum
                    End Select
                End If
            End If

            'Changing Power to Amplitude
            If SpectrumType = SpectrumTypes.AmplitudeSpectrum Then
                For coeffN = 0 To FrequencyDomainRealData(channel, windowNumber).WindowData.Length - 1
                    TempSpectrumArray(coeffN) = Math.Sqrt(TempSpectrumArray(coeffN))
                Next
            End If

            'Returning the current window spectrum array
            Return TempSpectrumArray

        End Function

        Public Sub GetSpectrogramMagnitudes(ByRef magnitudeSpectrumArray() As Single, ByRef x() As Double, ByRef y() As Double,
                                            ByVal normalizingFactor As Double)
            Try

                'Converts to polar form
                For coeffN = 0 To magnitudeSpectrumArray.Length - 1
                    magnitudeSpectrumArray(coeffN) = Math.Sqrt(x(coeffN) ^ 2 + y(coeffN) ^ 2) * normalizingFactor
                Next

            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try

        End Sub


        Public Sub GetRectangualForm_UsingMagnitudeAndPhase(ByRef x() As Double, ByRef y() As Double, ByRef magnitudes() As Double, ByRef phases() As Double)

            If Not ((magnitudes.Length = phases.Length) And (phases.Length = x.Length) And (x.Length = y.Length)) Then
                AudioError("All input arrays to FftData.getRectangularForm need to have the same length.")
                Exit Sub
            End If

            For n = 0 To magnitudes.Length - 1
                x(n) = Math.Cos(phases(n)) * magnitudes(n)
                y(n) = Math.Sin(phases(n)) * magnitudes(n)
            Next

        End Sub

        Public Sub GetRectangualForm_UsingPowerAndPhase(ByRef x() As Double, ByRef y() As Double, ByRef powers() As Double, ByRef phases() As Double)

            If Not ((powers.Length = phases.Length) And (phases.Length = x.Length) And (x.Length = y.Length)) Then
                AudioError("All input arrays to FftData.getRectangularForm need to have the same length.")
                Exit Sub
            End If

            For n = 0 To powers.Length - 1
                x(n) = Math.Cos(phases(n)) * Math.Sqrt(powers(n))
                y(n) = Math.Sin(phases(n)) * Math.Sqrt(powers(n))
            Next

        End Sub

        ''' <summary>
        ''' Creates data to a spectrogram with overlapping windows. The data is then summed in columns with the length of 
        ''' the analysis window length minus the overlap length. The data for these columns stored in the spectrogramData property of
        ''' this class. Before performing the spectrogram FFT, a filter may be applied to the sound data, to approximately equalize the
        ''' frequency amplitude in each frequency region of the analysed speech material.
        ''' </summary>
        ''' <param name="sound"></param>
        ''' <param name="spectrogramFormat"></param>
        ''' <param name="channel"></param>
        ''' <param name="startSample"></param>
        ''' <param name="sectionLength"></param>
        ''' <param name="normailizeSpectrogramData">Normalizes the calculated spectrogram data to the range from 0 - NormalizeTo.</param>
        ''' <param name="NormalizeTo">Determines the upper inclusive limit of the normalized spectrogram data.</param>
        Public Sub CalculateSpectrogramData(ByRef sound As Audio.Sound,
                                             ByRef spectrogramFormat As Audio.Formats.SpectrogramFormat, Optional ByVal channel As Integer = Nothing,
                                             Optional ByVal startSample As Integer = 0, Optional ByVal sectionLength As Integer? = Nothing, 'TODO StartSample and sectionLength is not implemented!
                                            Optional ByVal normailizeSpectrogramData As Boolean = True, Optional ByVal NormalizeTo As Double = 255)

            Try

                If startSample <> 0 Or sectionLength IsNot Nothing Then Throw New NotImplementedException("optinal StartSample and sectionLength is not yet implemented in CalculateSpectrogramData!")


                'Calculating cut-off frequency bin index
                Dim topDisplayFFTBin As Single = FftBinFrequencyConversion(FftBinFrequencyConversionDirection.FrequencyToBinIndex,
                                                                       spectrogramFormat.SpectrogramCutFrequency, sound.WaveFormat.SampleRate, sound.FFT.FftFormat.FftWindowSize,
                                                                       Utils.roundingMethods.getClosestValue)

                Dim OriginalSoundLength As Integer = sound.WaveData.SampleData(channel).Length
                Dim ColumnSize As Integer = FftFormat.AnalysisWindowSize - FftFormat.OverlapSize
                Dim ColumnCount As Integer = Utils.Rounding(OriginalSoundLength / ColumnSize, Utils.roundingMethods.alwaysUp)
                Dim SoundLengthBeforeFFT As Integer = ColumnSize * ColumnCount + 2 * FftFormat.AnalysisWindowSize - ColumnSize

                'Creating a temporary copy of the input sound which will be extended
                Dim tempSound As Audio.Sound

                'If usePreFftFilterring is True, a new sound is created and filterred, and then put into tempSound.
                'If usePreFftFilterring  is False, the input sound is put into tempSound
                If spectrogramFormat.UsePreFftFiltering = True Then

                    'Creating a filterred copy of the input sound
                    Dim kernelFftFormat As Audio.Formats.FftFormat = spectrogramFormat.SpectrogramPreFilterKernelFftFormat
                    Dim filterFftFormat As Audio.Formats.FftFormat = spectrogramFormat.SpectrogramPreFirFilterFftFormat
                    Dim kernel As Audio.Sound = Audio.GenerateSound.CreateSpecialTypeImpulseResponse(Waveformat, filterFftFormat, spectrogramFormat.PreFftFilteringKernelSize, , FilterType.LinearAttenuationBelowCF_dBPerOctave, Waveformat.SampleRate / 2,,,, spectrogramFormat.PreFftFilteringAttenuationRate,, spectrogramFormat.InActivateWarnings)
                    Dim filterredSound As Audio.Sound = Audio.DSP.TransformationsExt.FIRFilter(sound, kernel, filterFftFormat,,,,,, spectrogramFormat.InActivateWarnings)

                    'Creating a temporary and extended copy of the filterred sound, with the delay created by the FIR-filtering removed
                    tempSound = filterredSound.CreateCopy

                    Dim newSoundArray(SoundLengthBeforeFFT - 1) As Single
                    Dim FirFilterDelayInSamles As Integer = spectrogramFormat.PreFftFilteringKernelSize / 2
                    For sample = 0 To OriginalSoundLength - 1
                        newSoundArray(sample + FftFormat.AnalysisWindowSize) = filterredSound.WaveData.SampleData(channel)(sample + FirFilterDelayInSamles)
                    Next
                    tempSound.WaveData.SampleData(channel) = newSoundArray

                Else

                    'Creating a temporary and extended copy of the input sound
                    tempSound = sound.CreateCopy

                    Dim newSoundArray(SoundLengthBeforeFFT - 1) As Single
                    For sample = 0 To OriginalSoundLength - 1
                        newSoundArray(sample + FftFormat.AnalysisWindowSize) = sound.WaveData.SampleData(channel)(sample)
                    Next
                    tempSound.WaveData.SampleData(channel) = newSoundArray

                End If


                'Calculating fft on the temporary sound 
                tempSound.FFT = Audio.DSP.SpectralAnalysis(tempSound, FftFormat, channel)

                'Transforming fft data to polar form and putting it into the local object localSpectrogramData
                Dim localSpectrogramData As New List(Of List(Of Single()))
                Dim normalizingFactor As Single = FftFormat.FftWindowSize / FftFormat.AnalysisWindowSize
                For channel = 0 To tempSound.FFT._FrequencyDomainRealData.Count - 1
                    Dim NewChannelAmplitudeSpectrum As New List(Of Single())
                    For window = 0 To tempSound.FFT._FrequencyDomainRealData(channel).Count - 1
                        Dim NewWindowAmplitudeSpectrum(topDisplayFFTBin - 1) As Single
                        GetSpectrogramMagnitudes(NewWindowAmplitudeSpectrum, tempSound.FFT.FrequencyDomainRealData(channel + 1, window).WindowData, tempSound.FFT.FrequencyDomainImaginaryData(channel + 1, window).WindowData, normalizingFactor)
                        NewChannelAmplitudeSpectrum.Add(NewWindowAmplitudeSpectrum)
                    Next
                    localSpectrogramData.Add(NewChannelAmplitudeSpectrum)
                Next

                'Clearing addedAmplitudeSpectrum
                For n = 0 To _SpectrogramData.Count - 1
                    _SpectrogramData.Clear()
                Next

                For channel = 0 To tempSound.FFT._FrequencyDomainRealData.Count - 1
                    Dim newSpectrogramChannelData As New List(Of TimeWindow)
                    For column = 0 To ColumnCount - 1
                        Dim NewTimeWindow As New TimeWindow
                        Dim newColumn(topDisplayFFTBin - 1) As Double
                        NewTimeWindow.WindowData = newColumn
                        For fftbin = 0 To topDisplayFFTBin - 1
                            newColumn(fftbin) = 0
                        Next
                        newSpectrogramChannelData.Add(NewTimeWindow)
                    Next
                    _SpectrogramData.Add(newSpectrogramChannelData)
                Next

                'Summing up magnitude values for each column area
                Dim ColumnsPerAnalysisWindow As Integer = Utils.Rounding(FftFormat.AnalysisWindowSize / ColumnSize, Utils.roundingMethods.alwaysUp)
                For channel = 0 To _SpectrogramData.Count - 1
                    For column = 0 To _SpectrogramData(channel).Count - 1
                        For subAnalysisWindow = 0 To ColumnsPerAnalysisWindow - 1
                            For fftBin = 0 To topDisplayFFTBin - 1
                                _SpectrogramData(channel)(column).WindowData(fftBin) += localSpectrogramData(channel)(column + ColumnsPerAnalysisWindow - subAnalysisWindow)(fftBin)
                            Next
                        Next
                    Next
                Next

                If normailizeSpectrogramData = True Then
                    Dim maxAmpl As Double = 0

                    For channel = 0 To _SpectrogramData.Count - 1 'This means that data may be erraneously scaled if channel max levels are largely unequal
                        For column = 0 To _SpectrogramData(channel).Count - 1
                            Dim columnMax As Double = _SpectrogramData(channel)(column).WindowData.Max
                            If columnMax > maxAmpl Then maxAmpl = columnMax
                        Next
                    Next

                    Dim scalingFactor As Single = NormalizeTo / maxAmpl

                    'Normalizing the spectrogram data
                    For channel = 0 To _SpectrogramData.Count - 1
                        For column = 0 To _SpectrogramData(channel).Count - 1
                            For dataPoint = 0 To _SpectrogramData(channel)(column).WindowData.Length - 1
                                _SpectrogramData(channel)(column).WindowData(dataPoint) *= scalingFactor
                            Next
                        Next
                    Next
                End If


            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub


    End Class

    '<Serializable>
    'Public Class LocalEnvelopeData

    '    Private _envelopeData As New List(Of Single())

    '    Private ReadOnly Property Waveformat As Audio.Formats.WaveFormat
    '    Private ReadOnly Property EnvelopeFormat As Audio.Formats.EnvelopeFormat

    '    Public Property EnvelopeData(Optional ByVal channel As Integer = 1) As Single()
    '        Get
    '            CheckChannelValue(channel, _envelopeData.Count)
    '            Return _envelopeData(channel - 1)
    '        End Get
    '        Set(value As Single())
    '            CheckChannelValue(channel, _envelopeData.Count)
    '            _envelopeData(channel - 1) = value
    '        End Set
    '    End Property

    '    Public Sub New(ByVal setWaveFormat As Audio.Formats.WaveFormat, ByVal setEnvelopeFormat As Audio.Formats.EnvelopeFormat)

    '        Waveformat = setWaveFormat
    '        EnvelopeFormat = setEnvelopeFormat

    '        For n = 0 To Waveformat.Channels - 1
    '            Dim ChannelSoundData As Single() = {}
    '            _envelopeData.Add(ChannelSoundData)
    '        Next

    '    End Sub

    'End Class


    '<Serializable>
    'Public Class LocalVoicingAnalysisData

    '    Dim pointsOfZeroCrossings As List(Of Integer)
    '    Dim voicingDataTable As DataTable

    'End Class




End Namespace