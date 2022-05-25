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
    Public Module AudioManagement


        ''' <summary>
        ''' A helper function to determine which channels to modify on a sound.
        ''' </summary>
        Public Class AudioOutputConstructor

            Public Property FirstChannelIndex As Integer
            Public Property LastChannelIndex As Integer
            Private outputSoundFormat As Formats.WaveFormat

            ''' <summary>
            ''' Creates a new instance of AudioOutputConstructor
            ''' </summary>
            ''' <param name="format">The wave format of the source sound.</param>
            ''' <param name="channel">The channel to modify, as indicated by the calling function.</param>
            Public Sub New(ByRef format As Formats.WaveFormat, Optional ByVal channel As Integer? = Nothing)

                If channel = 0 Then Throw New ArgumentException("Channel value cannot be lower than 1.")

                If channel IsNot Nothing Then
                    FirstChannelIndex = channel
                    LastChannelIndex = channel
                Else
                    FirstChannelIndex = 1
                    LastChannelIndex = format.Channels
                End If

                Dim outputChannelCount As Integer = LastChannelIndex - FirstChannelIndex + 1
                outputSoundFormat = New Formats.WaveFormat(format.SampleRate, format.BitDepth, outputChannelCount,, format.Encoding)

            End Sub

            ''' <summary>
            ''' Creates a new sound based on the AudioOutputConstructor settings.
            ''' </summary>
            ''' <returns></returns>
            Public Function GetNewOutputSound() As Sound
                Dim outputSound = New Sound(outputSoundFormat)
                Return outputSound
            End Function

        End Class

        ''' <summary>
        ''' 'This sub checks to see if the values given for startSample and sectionLength is too high or below zero.
        ''' If startSample is below 0, it is set to 0. If it is higher than the length of the array, it is set to the last sampe in the array.
        ''' If sectionlength is below zero, it is set to 0. If it is too long for the array, it changed to fit within the length of the array.
        ''' </summary>
        ''' <param name="inputArrayLength">The length of the input array.</param>
        ''' <param name="startSample">The index of the start sample.</param>
        ''' <param name="sectionLength">The length of the section in samples. If the input value is Nothing, then sectionLength is set to the length of the rest of the sound.</param>
        ''' <returns>Returns True if any of startSample or sectionLength was corrected, and False if no correction was made.</returns>
        Public Function CheckAndCorrectSectionLength(ByVal inputArrayLength As Double, ByRef startSample As Integer, ByRef sectionLength As Integer?) As Boolean

            Dim modified As Integer = 0

            If startSample < 0 Then
                startSample = 0
                modified += 1
            End If

            If startSample > inputArrayLength - 1 Then
                startSample = inputArrayLength - 1
                modified += 1
            End If

            If sectionLength Is Nothing Then
                sectionLength = inputArrayLength
                modified += 1
            End If

            If sectionLength < 0 Then
                sectionLength = 0
                modified += 1
            End If

            If sectionLength > inputArrayLength - startSample Then
                sectionLength = inputArrayLength - startSample
                modified += 1
            End If

            If modified > 0 Then
                Return True
            Else
                Return False
            End If

        End Function



        ''' <summary>
        ''' Returns a value indicating if a audio bit depth for a given encoding is supported by the current audio library.
        ''' </summary>
        ''' <param name="WaveEncoding"></param>
        ''' <param name="BitDepth"></param>
        ''' <returns></returns>
        Public Function CheckIfBitDepthIsSupported(ByVal WaveEncoding As Formats.WaveFormat.WaveFormatEncodings, ByVal BitDepth As Integer) As Boolean
            Select Case WaveEncoding
                Case Formats.WaveFormat.WaveFormatEncodings.PCM
                    Select Case BitDepth
                        Case 16, 32
                            Return True
                        Case Else
                            Return False
                    End Select

                Case Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints
                    Select Case BitDepth
                        Case 32
                            Return True
                        Case Else
                            Return False
                    End Select
                Case Else
                    Return False
            End Select
        End Function

        Public Enum dBTypes
            SoundPressure
            SoundPower
        End Enum

        Public Function dBConversion(ByVal inputValue As Double, ByVal conversionDirection As dBConversionDirection,
                                     ByVal soundFormat As Formats.WaveFormat,
                                     Optional ByVal dBConversionType As dBTypes = dBTypes.SoundPressure) As Double

            Try

                Dim posFS As Double = soundFormat.PositiveFullScale

                Select Case dBConversionType
                    Case dBTypes.SoundPressure
                        Select Case conversionDirection
                            Case dBConversionDirection.to_dB
                                Dim dBFS = 20 * Math.Log10(inputValue / posFS)
                                Return dBFS
                            Case dBConversionDirection.from_dB
                                Dim RMS As Double = posFS * 10 ^ (inputValue / 20)
                                Return RMS
                            Case Else
                                Throw New ArgumentException("Invalid conversionDirection")
                        End Select

                    Case dBTypes.SoundPower
                        Select Case conversionDirection
                            Case dBConversionDirection.to_dB
                                Dim dBFS = 10 * Math.Log10(inputValue / posFS)
                                Return dBFS
                            Case dBConversionDirection.from_dB
                                Dim RMS As Double = posFS * 10 ^ (inputValue / 10)
                                Return RMS
                            Case Else
                                Throw New ArgumentException("Invalid conversionDirection")
                        End Select
                    Case Else
                        Throw New ArgumentException("Invalid dBConversionType")
                End Select

            Catch ex As Exception
                MsgBox(ex.ToString)
                Return Nothing
            End Try

        End Function

        Public Enum SoundMeasurementType
            RMS 'Measuring the RMS value
            AbsolutePeakAmplitude 'Measuring the absolute peak amplidude
            averageAbsoluteAmplitude 'Measuring the average absolute valued amplidude
        End Enum

        Public Enum SoundDataUnit
            dB
            linear
            unity
        End Enum

        Public Enum dBConversionDirection
            to_dB
            from_dB
        End Enum

    End Module

End Namespace
