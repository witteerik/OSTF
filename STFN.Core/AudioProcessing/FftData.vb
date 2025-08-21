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

Imports System.IO
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace Audio

    <Serializable>
    Public Class FftData

        Protected _FrequencyDomainRealData As New List(Of List(Of TimeWindow))
        Protected _FrequencyDomainImaginaryData As New List(Of List(Of TimeWindow))
        Protected _AmplitudeSpectrum As New List(Of List(Of TimeWindow))
        Protected _PhaseSpectrum As New List(Of List(Of TimeWindow))
        Protected _PowerSpectrumData As New List(Of List(Of TimeWindow))

        ''' <summary>
        ''' An object that can be used to temporarily store data. Non-serialized.
        ''' </summary>
        <XmlIgnore>
        Public TemporaryData As New List(Of TimeWindow)

        <Serializable>
        Public Class TimeWindow
            Public WindowData As Double()
            Public Property WindowingType As DSP.WindowingType?
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
                        _BinIndexToFrequencyList.Add(DSP.FftBinFrequencyConversion(DSP.FftBinFrequencyConversionDirection.BinIndexToFrequency, k, Waveformat.SampleRate, FftFormat.FftWindowSize))
                    Next
                End If

                Return _BinIndexToFrequencyList

            End Get
        End Property

        Public Property AmplitudeSpectrum(ByVal channel As Integer, ByVal windowNumber As Integer) As TimeWindow
            Get
                CheckChannelValue(channel, _AmplitudeSpectrum.Count)
                'Behövs kontroll av adderingsbehov här?
                Return _AmplitudeSpectrum(channel - 1)(windowNumber)
            End Get
            Set(value As TimeWindow)
                CheckChannelValue(channel, _AmplitudeSpectrum.Count)
                If windowNumber > _AmplitudeSpectrum(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_AmplitudeSpectrum, windowNumber - _AmplitudeSpectrum(channel - 1).Count + 1)
                _AmplitudeSpectrum(channel - 1)(windowNumber) = value

            End Set
        End Property

        Public Property PhaseSpectrum(ByVal channel As Integer, ByVal windowNumber As Integer) As TimeWindow
            Get
                CheckChannelValue(channel, _PhaseSpectrum.Count)
                'Behövs kontroll av adderingsbehov här?
                Return _PhaseSpectrum(channel - 1)(windowNumber)
            End Get
            Set(value As TimeWindow)
                CheckChannelValue(channel, _PhaseSpectrum.Count)
                If windowNumber > _PhaseSpectrum(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_PhaseSpectrum, windowNumber - _PhaseSpectrum(channel - 1).Count + 1)
                _PhaseSpectrum(channel - 1)(windowNumber) = value

            End Set
        End Property

        Public Property PowerSpectrumData(ByVal channel As Integer, ByVal windowNumber As Integer) As TimeWindow
            Get
                CheckChannelValue(channel, _PowerSpectrumData.Count)
                'Behövs kontroll av adderingsbehov här?
                Return _PowerSpectrumData(channel - 1)(windowNumber)
            End Get
            Set(value As TimeWindow)
                CheckChannelValue(channel, _PowerSpectrumData.Count)
                If windowNumber > _PowerSpectrumData(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_PowerSpectrumData, windowNumber - _PowerSpectrumData(channel - 1).Count + 1)
                _PowerSpectrumData(channel - 1)(windowNumber) = value

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
            Next

        End Sub

        ''' <summary>
        ''' This private sub is intended to be used only when an object of the current class is cloned by Xml serialization, such as with CreateCopy. 
        ''' </summary>
        Private Sub New()

        End Sub

        Protected Sub ExtendFrequencyDomainData(ByRef dataToExtend As List(Of List(Of TimeWindow)), ByVal windowCountToAdd As Integer)

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
                                              Optional ByVal CompensateForTimeWindowingScaling As Boolean = True,
                                              Optional ByVal AllowParallelProcessing As Boolean = True)

            Try


                'Resetting channel magnitude data
                For channel = 0 To _AmplitudeSpectrum.Count - 1
                    _AmplitudeSpectrum(channel).Clear()
                Next


                If AllowParallelProcessing = False Then

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
                                          Optional ByVal ZeroPaddingCompensationLowerLimit As Double? = Nothing,
                                          Optional ByVal AllowParallelProcessing As Boolean = True)

            'Resetting channel power spectrum data
            For channel = 0 To _PowerSpectrumData.Count - 1
                _PowerSpectrumData(channel).Clear()
            Next


            If AllowParallelProcessing = False Then

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
                            BinSpectrum(k) = DSP.dBConversion(BinSpectrum(k) / WindowCount(Channel), DSP.dBConversionDirection.to_dB,
                                                  SoundFormat, DSP.dBTypes.SoundPower)
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
                            BinSpectrum(k) = DSP.dBConversion(BinSpectrum(k), DSP.dBConversionDirection.to_dB,
                                                  SoundFormat, DSP.dBTypes.SoundPressure)
                        Else
                            BinSpectrum(k) = BinSpectrum(k)
                        End If

                    Next

                Case Else
                    Throw New NotImplementedException

            End Select

            If ConvertTo_dB = True Then TotalLevel = DSP.dBConversion(TotalLevel / WindowCount(Channel), DSP.dBConversionDirection.to_dB,
                                   SoundFormat, DSP.dBTypes.SoundPower)


            'Converting bins to frequencies
            Dim OutputList As New SortedList(Of Double, Double)
            For Each kvp In BinSpectrum
                OutputList.Add(DSP.FftBinFrequencyConversion(DSP.FftBinFrequencyConversionDirection.BinIndexToFrequency,
                                                     kvp.Key, SoundFormat.SampleRate, Me.FftFormat.FftWindowSize), kvp.Value)
            Next

            Return OutputList

        End Function

        Public Sub ExportPowerSpectrum(ByVal Channel As Integer, Optional ByVal FileName As String = "PowerSpectrum",
                                       Optional ByVal OutputFolder As String = "", Optional SkipNegativeFrequencies As Boolean = True,
                                       Optional ByVal ConvertTo_dB As Boolean = True, Optional ByVal ReferenceIntensity As Double = 1)

            If OutputFolder = Nothing Then OutputFolder = Logging.LogFileDirectory

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
                        LowerLimit = DSP.FftBinFrequencyConversion(DSP.FftBinFrequencyConversionDirection.FrequencyToBinIndex, LowerLimit,
                                                                         Waveformat.SampleRate, FftFormat.FftWindowSize, DSP.RoundingMethods.AlwaysDown)
                    Else
                        LowerLimit = DSP.FftBinFrequencyConversion(DSP.FftBinFrequencyConversionDirection.FrequencyToBinIndex, LowerLimit,
                                                                         Waveformat.SampleRate, FftFormat.FftWindowSize, DSP.RoundingMethods.AlwaysUp)

                    End If


                    If UpperLimitIsInclusive = True Then

                        UpperLimit = DSP.FftBinFrequencyConversion(DSP.FftBinFrequencyConversionDirection.FrequencyToBinIndex, UpperLimit,
                                                                         Waveformat.SampleRate, FftFormat.FftWindowSize, DSP.RoundingMethods.AlwaysUp)

                    Else

                        UpperLimit = DSP.FftBinFrequencyConversion(DSP.FftBinFrequencyConversionDirection.FrequencyToBinIndex, UpperLimit,
                                                                         Waveformat.SampleRate, FftFormat.FftWindowSize, DSP.RoundingMethods.AlwaysDown)

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
                ActualLowerLimitFrequency = DSP.FftBinFrequencyConversion(DSP.FftBinFrequencyConversionDirection.BinIndexToFrequency, LowerLimit,
                                                                         Waveformat.SampleRate, FftFormat.FftWindowSize)
                ActualUpperLimitFrequency = DSP.FftBinFrequencyConversion(DSP.FftBinFrequencyConversionDirection.BinIndexToFrequency, UpperLimit,
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

        Public Enum SpectrumTypes
            AmplitudeSpectrum
            PowerSpectrum
            PhaseSpectrum
        End Enum

        Public Enum LoudnessFunctions
            InExType
            ZwickerType
            Simple
        End Enum


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
                        If FrequencyDomainRealData(channel, windowNumber).WindowingType <> DSP.WindowingType.Rectangular Then
                            'Getting the EquivalentNoiseBandwidth if not supplied by the calling code
                            If EquivalentNoiseBandwidth Is Nothing Then EquivalentNoiseBandwidth = DSP.GetEquivalentNoiseBandwidth(FftFormat.FftWindowSize, FftFormat.WindowingType, FftFormat.Tukey_r)
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

                If FftFormat.WindowingType <> DSP.WindowingType.Rectangular Then

                    Select Case SpectrumType
                        Case SpectrumTypes.PowerSpectrum, SpectrumTypes.AmplitudeSpectrum
                            If InverseTimeWindowingScalingFactor Is Nothing Then InverseTimeWindowingScalingFactor = DSP.GetInverseWindowingScalingFactor(FftFormat.FftWindowSize, FftFormat.WindowingType)
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







        ''' <summary>
        ''' Creates a new FftData which is a deep copy of the original, by using serialization.
        ''' </summary>
        ''' <returns></returns>
        Public Function CreateCopy() As FftData

            'Creating an output object
            Dim NewObject As FftData

            'Serializing to memorystream
            Dim serializedMe As New MemoryStream
            Dim serializer As New XmlSerializer(GetType(FftData))
            serializer.Serialize(serializedMe, Me)

            'Deserializing to new object
            serializedMe.Position = 0
            NewObject = CType(serializer.Deserialize(serializedMe), FftData)
            serializedMe.Close()

            'Returning the new object
            Return NewObject
        End Function


    End Class


End Namespace