Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows.Forms

Namespace Audio

    Namespace PortAudioVB

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

            Public Sub SetLinearInput()
                For c = 1 To AvailableInputChannels
                    InputRouting(c) = c
                Next
            End Sub
            Public Sub SetLinearOutput()
                For c = 1 To AvailableOutputChannels
                    OutputRouting(c) = c
                Next
            End Sub

            Public Sub DirectMonoSoundToOutputChannels(ByRef TargetOutputChannels() As Integer)
                For Each OutputChannel In TargetOutputChannels
                    If OutputRouting.ContainsKey(OutputChannel) Then OutputRouting(OutputChannel) = 1
                Next
            End Sub

#Region "Calibration"

            ''' <summary>
            ''' Holds the simulated sound field output level of a 1 kHz sine wave at an (hypothetical) RMS level of 0 dBFS. 
            ''' </summary>
            Public Const Simulated_dBFS_dBSPL_Difference As Double = 100

            Private _Calibration_FsToSpl As New SortedList(Of Integer, Double)

            ''' <summary>
            ''' Returns the calibration (FsToSpl) value for the indicated loudspeaker.
            ''' </summary>
            ''' <param name="Channel"></param>
            ''' <returns></returns>
            Public ReadOnly Property Calibration_FsToSpl(ByVal Channel As Integer) As Double
                Get
                    If _Calibration_FsToSpl.ContainsKey(Channel) Then
                        Return _Calibration_FsToSpl(Channel)
                    Else
                        MsgBox("Calibration has not been set for output channel " & Channel & vbCrLf & vbCrLf &
                       "Click OK to use the default FsToSpl value of " & Simulated_dBFS_dBSPL_Difference & " dB!", MsgBoxStyle.Exclamation, "Warning!")
                        Return Simulated_dBFS_dBSPL_Difference
                    End If
                End Get
            End Property


            Public Function GetCalibrationGain(ByVal Channel As Integer)

                'Calculates the calibration gain
                Dim CalibrationGain As Double = Simulated_dBFS_dBSPL_Difference - Calibration_FsToSpl(Channel)

                'Returns the calibration gain
                Return CalibrationGain

            End Function

            ''' <summary>
            ''' Call this sub to set the loudspeaker or headphone calibration of the current instance of DuplexMixer.
            ''' </summary>
            ''' <param name="Calibration_FsToSpl"></param>
            Public Sub SetCalibrationValues(ByVal Calibration_FsToSpl As SortedList(Of Integer, Double))

                'Setting the private field _Calibration_FsToSpl. Values are retrieved by the public Readonly Property Calibration_FsToSpl 
                Me._Calibration_FsToSpl = Calibration_FsToSpl

            End Sub

            ''' <summary>
            ''' Converts the sound pressure level given by InputSPL to a value in dB FS using the conversion value given by Simulated_dBFS_dBSPL_Difference
            ''' </summary>
            ''' <param name="InputSPL"></param>
            ''' <returns></returns>
            Public Shared Function Simulated_dBSPL_To_dBFS(ByVal InputSPL As Double) As Double
                Return InputSPL - Simulated_dBFS_dBSPL_Difference
            End Function

            ''' <summary>
            ''' Converts the full scale sound level given by InputFS to a sound pressure level value using the conversion value given by Simulated_dBFS_dBSPL_Difference
            ''' </summary>
            ''' <param name="InputFS"></param>
            ''' <returns></returns>
            Public Shared Function Simulated_dBFS_To_dBSPL(ByVal InputFS As Double) As Double
                Return Simulated_dBFS_dBSPL_Difference + InputFS
            End Function

#End Region

            Public Class SoundSceneItem
                Public Sound As Audio.Sound
                Public ReadChannel As Integer
                Public SoundLevel As Double
                Public SoundLevelFormat As Audio.Formats.SoundLevelFormat
                Public LevelGroup As Integer
                Public SourceLocation As SoundSourceLocation
                ''' <summary>
                ''' Specifications of fadings. Note that this can also be used to create duckings, by adding a (partial) fade out, attenuation section, and a (partial) fade in.
                ''' </summary>
                Public FadeSpecifications As List(Of Audio.DSP.FadeSpecifications)

                ''' <summary>
                ''' Specifications of ducking periods. A ducking perdio is specified by combining two fade out and fade in events. Multiple duckings may be specified by listing several fade in-fade out events.
                ''' </summary>
                Public DuckingSpecifications As List(Of Audio.DSP.FadeSpecifications)

            End Class

            Public Class SoundSourceLocation
                ''' <summary>
                ''' The distance to the sound source in meters.
                ''' </summary>
                Public Distance As Double

                ''' <summary>
                ''' The horizontal azimuth of the sound source in degrees, in relation to front (zero degrees), positive values to the right and negative values to the left.
                ''' </summary>
                Public HorizontalAzimuth As Double

                ''' <summary>
                ''' The verical elevation of the sound source in degrees, where zero degrees refers to the horizontal plane, positive values upwards and negative values downwards.
                ''' </summary>
                Public Elevation As Double

            End Class


            Public Function CreateSoundScene(Input As List(Of SoundSceneItem)) As Audio.Sound

                'Copy sounds


                'Create OutputSound
                Dim OutputSound

                ' fade stuff

                ' Set levels

                '       Nonducking 
                '       Ducking

                ' Ducking?

                ' Simulation of HL/HA

                ' Mix
                '       Select sound field or headphones
                '             + Calibrate

                'Limiter

                Return OutputSound


            End Function


            End Class

    End Namespace

End Namespace
