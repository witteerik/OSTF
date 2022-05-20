
Namespace Audio
    Public Module GlobalAudioData

        ''' <summary>
        ''' Holds the level of Raised vocal effort (in dB SPL), according to SII, ANSI S3.5 1997.
        ''' </summary>
        Public Const RaisedVocalEffortLevel As Double = 68.34


        'Output sound levels
        ''' <summary>
        ''' Holds the sound field output level of a 1 kHz sine wave at (hypothetical) RMS level of 0 dBFS (actually measured using (the calibration dialog box) at -23 dbFS)
        ''' </summary>
        Public FullScaleSoundFieldOutputSoundLevel As Double = 100

        ''' <summary>
        ''' Holds the headphones output level of a 1 kHz sine wave at (hypothetical) RMS level of 0 dBFS 
        ''' </summary>
        Public FullScaleHeadphonesOutputSoundLevel As Double = 100


        ''' <summary>
        ''' Holds the simulated sound field output level of a 1 kHz sine wave at an (hypothetical) RMS level of 0 dBFS 
        ''' </summary>
        Public Simulated_dBFS_dBSPL_Difference As Double = 100

        Public Enum SoundTransducerMode
            SoundField
            HeadPhones
        End Enum

        Public CurrentSoundTransducerMode As Audio.SoundTransducerMode = SoundTransducerMode.SoundField


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="InputSPL"></param>
        ''' <param name="MeasuredCalibrationLevel">If set to false, the a calibration independent scaling of Simulated_dBFS_dBSPL_Difference is used.</param>
        ''' <returns></returns>
        Public Function Convert_dBSPL_To_dBFS(ByVal InputSPL As Double, Optional ByVal MeasuredCalibrationLevel As Boolean = True) As Double

            If MeasuredCalibrationLevel = True Then

                Select Case CurrentSoundTransducerMode
                    Case SoundTransducerMode.SoundField
                        Return InputSPL - FullScaleSoundFieldOutputSoundLevel
                    Case SoundTransducerMode.HeadPhones
                        Return InputSPL - FullScaleHeadphonesOutputSoundLevel
                    Case Else
                        Throw New NotImplementedException
                End Select

            Else

                Select Case CurrentSoundTransducerMode
                    Case SoundTransducerMode.SoundField
                        Return InputSPL - Simulated_dBFS_dBSPL_Difference
                    Case SoundTransducerMode.HeadPhones
                        Return InputSPL - Simulated_dBFS_dBSPL_Difference
                    Case Else
                        Throw New NotImplementedException
                End Select
            End If

        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="InputFS"></param>
        ''' <param name="MeasuredCalibrationLevel">If set to true, the current full scale calibration is applied. If set to false, a calibration independent scaling of Simulated_dBFS_dBSPL_Difference is used.</param>
        ''' <returns></returns>
        Public Function Convert_dBFS_To_dBSPL(ByVal InputFS As Double, ByVal MeasuredCalibrationLevel As Boolean) As Double

            If MeasuredCalibrationLevel = True Then
                Select Case CurrentSoundTransducerMode
                    Case SoundTransducerMode.SoundField
                        Return FullScaleSoundFieldOutputSoundLevel + InputFS
                    Case SoundTransducerMode.HeadPhones
                        Return FullScaleHeadphonesOutputSoundLevel + InputFS
                    Case Else
                        Throw New NotImplementedException
                End Select

            Else
                Select Case CurrentSoundTransducerMode
                    Case SoundTransducerMode.SoundField
                        Return Simulated_dBFS_dBSPL_Difference + InputFS
                    Case SoundTransducerMode.HeadPhones
                        Return Simulated_dBFS_dBSPL_Difference + InputFS
                    Case Else
                        Throw New NotImplementedException
                End Select
            End If

        End Function


        Public ReferenceSoundIntensityLevel As Double = 10 ^ (-12)


        'Phonetic segmentation parameters
        Public LongestSilentSegment As Double = 0.45
        Public SilenceDefinition As Double = 40
        Public TemporalIntegrationDuration As Double = 0.06
        Public DetailedTemporalIntegrationDuration As Double = 0.006
        Public DetailedSilenceCriteria As Double = 20


    End Module

End Namespace
