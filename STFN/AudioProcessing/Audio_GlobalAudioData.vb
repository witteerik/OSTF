
Namespace Audio
    Public Module GlobalAudioData

        ''' <summary>
        ''' Holds the level of Raised vocal effort (in dB SPL), according to SII, ANSI S3.5 1997.
        ''' </summary>
        Public Const RaisedVocalEffortLevel As Double = 68.34

        Public Enum SoundTransducerModes
            SoundField
            HeadPhones
        End Enum

        Public CurrentSoundTransducerMode As Audio.SoundTransducerModes = SoundTransducerModes.SoundField


    End Module

End Namespace
