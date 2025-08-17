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

Imports System.Runtime.Serialization

Namespace Audio

    Namespace Formats

        Public Enum SupportedWaveFormatsEncodings
            PCM = 1
            IeeeFloatingPoints = 3
        End Enum

        <Serializable>
        Public Class WaveFormat

            Public ReadOnly Property SampleRate As UInteger
            Public ReadOnly Property BitDepth As UShort
            Public ReadOnly Property Channels As Short
            Public ReadOnly Property Encoding As WaveFormatEncodings 'This property is acctually redundant since it hold the same numeric value as FmtCode. It is retained for now since it clearly specifies the names of the formats, and FmtCode need to be in UShort

            Public Property Name As String
            Public ReadOnly Property MainChunkID As String = "RIFF"
            Public ReadOnly Property RiffType As String = "WAVE"
            Public ReadOnly Property FmtID As String = "fmt "
            Public ReadOnly Property FmtSize As UInteger = 16
            Public ReadOnly Property FmtCode As UShort
            Public ReadOnly Property FmtAvgBPS As UInteger
            Public ReadOnly Property FmtBlockAlign As UShort
            Public ReadOnly Property DataID As String = "data"
            Public ReadOnly Property PositiveFullScale As Double
            Public ReadOnly Property NegativeFullScale As Double

            Public Enum WaveFormatEncodings
                PCM = 1
                IeeeFloatingPoints = 3
                'WAVE_FORMAT_EXTENSIBLE = 65534
            End Enum

            Public Sub New(ByVal SampleRate As Integer, ByVal BitDepth As Integer, ByVal Channels As Integer,
                       Optional Name As String = "",
                       Optional Encoding As WaveFormatEncodings = WaveFormatEncodings.IeeeFloatingPoints)

                Me.SampleRate = SampleRate
                Me.BitDepth = BitDepth
                Me.Name = Name

                'Setting bit depth, and checks that the bit depth is supported
                If Not CheckIfBitDepthIsSupported(Encoding, Me.BitDepth) = True Then Throw New NotImplementedException("BitDepth " & Me.BitDepth & " Is Not yet implemented.")

                'Setting format code (automatically converting the numeric value of the enumerator to UShort)
                FmtCode = Encoding
                Me.Encoding = Encoding

                'Set full scale
                Select Case Me.Encoding
                    Case WaveFormatEncodings.PCM
                        Select Case Me.BitDepth
                            Case 8  'Byte
                                'Note that 8 bit wave has the numeric range of 0-255,
                                PositiveFullScale = Byte.MaxValue
                                NegativeFullScale = Byte.MinValue + 1
                            Case 16 'Short
                                PositiveFullScale = Short.MaxValue
                                NegativeFullScale = Short.MinValue + 1
                            Case 32 'Integer
                                PositiveFullScale = Integer.MaxValue
                                NegativeFullScale = Integer.MinValue + 1
                            Case Else
                                Throw New NotImplementedException(Me.BitDepth & " bit depth is not yet supported for the PCM wave format.")
                        End Select
                    Case WaveFormatEncodings.IeeeFloatingPoints
                        Select Case Me.BitDepth
                            Case 32 'Single
                                PositiveFullScale = 1
                                NegativeFullScale = -1
                            Case 64 'Double
                                PositiveFullScale = 1
                                NegativeFullScale = -1
                            Case Else
                                Throw New NotImplementedException(Me.BitDepth & " bit depth is not yet supported for Ieee Floating Points format.")
                        End Select
                End Select

                'Setting (and correcting) number of channels
                If Channels < 1 Then Channels = 1
                Me.Channels = Channels

                'Calculating and setting FmtAvgBPS and FmtBlockAlign 
                FmtAvgBPS = Me.SampleRate * Me.Channels * (Me.BitDepth / 8)
                FmtBlockAlign = Me.Channels * (Me.BitDepth / 8)

            End Sub

            ''' <summary>
            ''' Determines whether values are the same between the current WaveFormat and ComparisonFormat
            ''' </summary>
            ''' <param name="ComparisonFormat"></param>
            ''' <returns></returns>
            Public Function IsEqual(ByRef ComparisonFormat As WaveFormat,
                                    Optional ByVal CompareChannels As Boolean = True,
                                    Optional ByVal CompareSampleRate As Boolean = True,
                                    Optional ByVal CompareBitDepth As Boolean = True,
                                    Optional ByVal CompareEncoding As Boolean = True) As Boolean

                If CompareChannels = True Then If ComparisonFormat.Channels <> Me.Channels Then Return False
                If CompareSampleRate = True Then If ComparisonFormat.SampleRate <> Me.SampleRate Then Return False
                If CompareBitDepth = True Then If ComparisonFormat.BitDepth <> Me.BitDepth Then Return False
                If CompareEncoding = True Then If ComparisonFormat.Encoding <> Me.Encoding Then Return False

                Return True

            End Function

            Public Overrides Function ToString() As String

                Dim Output As New List(Of String)

                Output.Add("Samplerate: " & SampleRate.ToString & " Hz")
                Output.Add("Bit depth: " & BitDepth.ToString)
                Output.Add("Channels: " & Channels.ToString)
                Output.Add("Encoding: " & Encoding.ToString)

                Return String.Join(vbCrLf, Output)

            End Function

        End Class


        <Serializable>
        Public Class FftFormat

            Public Property FftWindowSize As Integer
            Public Property AnalysisWindowSize As Integer
            Public Property OverlapSize As Integer
            Public Property WindowingType As DSP.WindowingType = WindowingType.Rectangular
            Public Property Tukey_r As Double

            ''' <summary>
            ''' Creates a new fft format.
            ''' </summary>
            ''' <param name="setAnalysisWindowSize">Determines the length in samples of each part of the wave form that will be analysed in fft.</param>
            ''' <param name="setFftWindowSize">Determines the FFT length (frequency resolution) that will be used when calculating fft (is automatically set to the length of the analysis window if left emtpy).</param>
            ''' <param name="setoverlapSize">Determines the overlap between two analysis windows (in samples.)</param>
            ''' <param name="setWindowing">Determines which windowing function of the analysis window that will be used the before the fft calculation.</param>
            ''' <param name="Tukey_r">The ratio between windowing size and the cosine sections in a Tukey window. Only needed if Tukey windowing is used.</param>
            Public Sub New(Optional ByRef setAnalysisWindowSize As Integer = 1024, Optional ByRef setFftWindowSize As Integer = -1, Optional ByRef setoverlapSize As Integer = 0,
                   Optional setWindowing As DSP.WindowingType = DSP.WindowingType.Rectangular, Optional ByRef InActivateWarnings As Boolean = False,
                   Optional Tukey_r As Double = 0.5)

                'Adjusting setAnalysisWindowSize
                If setAnalysisWindowSize < 0 Then setAnalysisWindowSize = 1024
                If setAnalysisWindowSize Mod 2 = 1 Then setAnalysisWindowSize += 1
                AnalysisWindowSize = setAnalysisWindowSize

                'Adding default value for setFftWindowSize, if left empty
                If setFftWindowSize < 0 Then setFftWindowSize = AnalysisWindowSize

                'Checking fft size
                DSP.CheckAndAdjustFFTSize(setFftWindowSize, AnalysisWindowSize, InActivateWarnings)
                FftWindowSize = setFftWindowSize

                'Checking overlap size
                If Not setoverlapSize < AnalysisWindowSize Then setoverlapSize = AnalysisWindowSize - 1
                OverlapSize = setoverlapSize

                WindowingType = setWindowing
                Me.Tukey_r = Tukey_r
            End Sub


        End Class


        <Serializable>
        Public Class SoundLevelFormat

            Private _SoundMeasurementType As SoundMeasurementTypes
            Public Property SoundMeasurementType As SoundMeasurementTypes
                Get
                    Return _SoundMeasurementType
                End Get
                Set(value As SoundMeasurementTypes)
                    _SoundMeasurementType = value
                    Select Case value
                        Case Audio.SoundMeasurementTypes.LoudestSection_Z_Weighted
                            LoudestSectionMeasurement = True
                            FrequencyWeighting = FrequencyWeightings.Z

                        Case Audio.SoundMeasurementTypes.LoudestSection_C_Weighted
                            LoudestSectionMeasurement = True
                            FrequencyWeighting = FrequencyWeightings.C

                        Case Audio.SoundMeasurementTypes.LoudestSection_RLB_Weighted
                            LoudestSectionMeasurement = True
                            FrequencyWeighting = FrequencyWeightings.RLB

                        Case Audio.SoundMeasurementTypes.LoudestSection_K_Weighted
                            LoudestSectionMeasurement = True
                            FrequencyWeighting = FrequencyWeightings.K

                        Case Audio.SoundMeasurementTypes.Average_C_Weighted
                            LoudestSectionMeasurement = False
                            FrequencyWeighting = FrequencyWeightings.C

                        Case Audio.SoundMeasurementTypes.Average_RLB_Weighted
                            LoudestSectionMeasurement = False
                            FrequencyWeighting = FrequencyWeightings.RLB

                        Case Audio.SoundMeasurementTypes.Average_K_Weighted
                            LoudestSectionMeasurement = False
                            FrequencyWeighting = FrequencyWeightings.K

                        Case Audio.SoundMeasurementTypes.Average_Z_Weighted
                            LoudestSectionMeasurement = False
                            FrequencyWeighting = FrequencyWeightings.Z

                        Case Else
                            Throw New NotImplementedException("Unsupported frequency weighting.")
                    End Select
                End Set
            End Property

            Public Property LoudestSectionMeasurement As Boolean
            ''' <summary>
            ''' The lengths of the sound level measurement sections. (Should appropriately be set to the auditory temporal integration time (appr 0.1-0.2 s))
            ''' </summary>
            ''' <returns></returns>
            Public Property TemporalIntegrationDuration As Decimal
            Public Property FrequencyWeighting As FrequencyWeightings

            ''' <summary>
            ''' Creates a new instance of the SoundLevelFormat class
            ''' </summary>
            ''' <param name="DefaultSoundMeasurementType"></param>
            ''' <param name="DefaultTemporalIntegrationDuration"></param>
            Public Sub New(DefaultSoundMeasurementType As Audio.SoundMeasurementTypes,
                       Optional DefaultTemporalIntegrationDuration As Decimal = 0.05)

                SoundMeasurementType = DefaultSoundMeasurementType
                TemporalIntegrationDuration = DefaultTemporalIntegrationDuration

            End Sub

            ''' <summary>
            ''' Compares the instance of SoundLevelFormat to another instance.
            ''' </summary>
            ''' <param name="SoundLevelFormat"></param>
            ''' <returns>Returns False if the compared instances differ in SoundMeasurementType, and if time weighting is used, also if the temporal integration time differs. Otherwise returns True</returns>
            Public Function IsEqual(ByVal SoundLevelFormat As SoundLevelFormat) As Boolean

                If SoundMeasurementType <> SoundLevelFormat.SoundMeasurementType Then Return False

                If LoudestSectionMeasurement Then
                    If TemporalIntegrationDuration <> SoundLevelFormat.TemporalIntegrationDuration Then Return False
                End If

                Return True

            End Function

            '            ''' <summary>
            '            ''' Displays the parameters of the current SoundLevelFormat, using a GUI, and asks the user to confirm or change it.
            '            ''' </summary>
            '            Public Sub SelectFormatWithGui()

            '                Try

            '                    Dim myDialogBox As New GeneralDialogBox
            '                    myDialogBox.Text = "New sound level format settings"
            '                    myDialogBox.FormBorderStyle = FormBorderStyle.Sizable
            '                    myDialogBox.Height = 450
            '                    myDialogBox.Width = 600

            '                    Dim myPanel As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 7}

            '                    'Sound measurement type
            '                    myPanel.Controls.Add(New Label With {.Text = "Sound measurement type:  ", .AutoSize = True})
            '                    Dim myMeasurementTypeBox As New ListBox
            '                    myMeasurementTypeBox.Dock = DockStyle.Fill
            '                    Dim MeasureMentTypes() As String = [Enum].GetNames(GetType(Audio.SoundMeasurementTypes))
            '                    For Each CurrentItem In MeasureMentTypes
            '                        myMeasurementTypeBox.Items.Add(CurrentItem)
            '                    Next

            '                    'Setting default value as selected
            '                    For n = 0 To MeasureMentTypes.Length - 1
            '                        If myMeasurementTypeBox.Items(n) = Me.SoundMeasurementType.ToString Then myMeasurementTypeBox.SelectedIndex = n
            '                    Next

            '                    myPanel.Controls.Add(myMeasurementTypeBox)


            '                    'The rest
            '                    myPanel.Controls.Add(New Label With {.Text = "[Optional] Temporal integration duration (s): ", .AutoSize = True, .TextAlign = Drawing.ContentAlignment.BottomLeft})
            '                    Dim TemporalIntegrationDurationBox As New TextBox With {.Text = Me.TemporalIntegrationDuration}
            '                    myPanel.Controls.Add(TemporalIntegrationDurationBox)

            '                    myDialogBox.Controls.Add(myPanel)

            '0:
            '                    Dim Result = myDialogBox.ShowDialog()

            '                    'Setting Audio.SoundMeasurementType
            '                    Me.SoundMeasurementType = [Enum].Parse(GetType(Audio.SoundMeasurementTypes), myMeasurementTypeBox.Items(myMeasurementTypeBox.SelectedIndex))

            '                    If IsNumeric(TemporalIntegrationDurationBox.Text) Then
            '                        Me.TemporalIntegrationDuration = TemporalIntegrationDurationBox.Text
            '                    Else
            '                        MsgBox("Incorrect value for temporal integration duration.")
            '                        GoTo 0
            '                    End If


            '                    Dim CheckResult = MsgBox("You've selected the following sound level format:" & vbCr & vbCr &
            '                            "Sound measurement type: " & Me.SoundMeasurementType.ToString & vbCr &
            '                            "Temporal integration duration: " & Me.TemporalIntegrationDuration & " seconds" & vbCr &
            '                            "Is this correct? (Click Yes to accept, or No to go back!)", MsgBoxStyle.YesNo, "Check selected sound level format!")
            '                    If CheckResult = MsgBoxResult.No Then
            '                        GoTo 0
            '                    End If

            '                Catch ex As Exception
            '                    MsgBox(ex.ToString)
            '                End Try

            '            End Sub


        End Class

        <Serializable>
        Public Class SpectrogramFormat

            Public Property SpectrogramFftFormat As FftFormat
            Public Property SpectrogramPreFilterKernelFftFormat As FftFormat
            Public Property SpectrogramPreFirFilterFftFormat As FftFormat

            Public Property SpectrogramCutFrequency As Single
            Public Property UsePreFftFiltering As Boolean
            Public Property PreFftFilteringAttenuationRate As Single
            Public Property PreFftFilteringKernelSize As Integer
            Public Property PreFftFilteringKernelCreationAnalysisWindowSize As Integer
            Public Property PreFftFilteringAnalysisWindowSize As Integer
            Public Property InActivateWarnings As Boolean

            Public Sub New(Optional ByVal setSpectrogramCutFrequency As Single = 8000, Optional spectrogramAnalysisWindowSize As Integer = 1024, Optional spectrogramFftSize As Integer = 1024,
        Optional spectrogramAnalysisWindowOverlapSize As Integer = 512,
                   Optional spectrogramWindowingMethod As DSP.WindowingType = DSP.WindowingType.Hamming,
                   Optional ByVal setUsePreFftFiltering As Boolean = True,
                   Optional ByVal setPreFftFilteringAttenuationRate As Single = 6, Optional ByVal setPreFftFilteringKernelSize As Integer = 2000,
                   Optional ByVal setPreFftFilteringAnalysisWindowSize As Integer = 1024,
                   Optional setInActivateWarnings As Boolean = False)

                InActivateWarnings = setInActivateWarnings
                UsePreFftFiltering = setUsePreFftFiltering
                PreFftFilteringAttenuationRate = setPreFftFilteringAttenuationRate
                PreFftFilteringKernelSize = setPreFftFilteringKernelSize
                PreFftFilteringAnalysisWindowSize = setPreFftFilteringAnalysisWindowSize
                PreFftFilteringKernelCreationAnalysisWindowSize = setPreFftFilteringAnalysisWindowSize

                SpectrogramCutFrequency = setSpectrogramCutFrequency
                SpectrogramFftFormat = New FftFormat(spectrogramAnalysisWindowSize, spectrogramFftSize, spectrogramAnalysisWindowOverlapSize, spectrogramWindowingMethod, InActivateWarnings)
                SpectrogramPreFilterKernelFftFormat = New FftFormat(PreFftFilteringKernelCreationAnalysisWindowSize,,, DSP.WindowingType.Hamming, InActivateWarnings) 'TODO should there be a windowing function specified here?
                SpectrogramPreFirFilterFftFormat = New FftFormat(PreFftFilteringAnalysisWindowSize,,, DSP.WindowingType.Hamming, InActivateWarnings) 'TODO should there be a windowing function specified here?

            End Sub

        End Class

    End Namespace

    Public Module BasicAudioEnums

        Public Enum SoundFileFormats
            wav
            ptwf
        End Enum

        Public Enum TimeUnits
            samples
            seconds
            pixels
        End Enum

        Public Enum FrequencyWeightings
            A
            C
            Z
            RLB
            K
        End Enum

        Public Enum SoundMeasurementTypes
            Average_Z_Weighted = 0
            LoudestSection_Z_Weighted = 100
            Average_A_Weighted = 1
            LoudestSection_A_Weighted = 101
            Average_C_Weighted = 2
            LoudestSection_C_Weighted = 102
            Average_RLB_Weighted = 3
            LoudestSection_RLB_Weighted = 103
            Average_K_Weighted = 4
            LoudestSection_K_Weighted = 104
        End Enum

    End Module

End Namespace