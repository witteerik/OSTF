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