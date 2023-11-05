
Namespace Audio

    ''Temporarily outcommented, until better solutions is fixed:
    'Namespace PlayBack
    '    Public Interface ISoundPlayerControl
    '        Sub MessageFromPlayer(ByRef Message As MessagesFromSoundPlayer)
    '        Enum MessagesFromSoundPlayer
    '            EndOfSound
    '            ApproachingEndOfBufferAlert
    '            NewBufferTick
    '        End Enum
    '    End Interface
    'End Namespace


    Namespace SoundPlayers

        Public Interface iSoundPlayer

            Property RaisePlaybackBufferTickEvents As Boolean

            Event MessageFromPlayer(ByVal Message As String)

            Property EqualPowerCrossFade As Boolean

            Enum FadeTypes
                Linear
                Smooth
            End Enum

            ReadOnly Property IsPlaying As Boolean

            ''' <summary>
            ''' Set the crossfade duration (in seconds)
            ''' </summary>
            ''' <param name="Duration"></param>
            Sub SetOverlapDuration(ByVal Duration As Double)

            Function GetOverlapDuration() As Double

            ''' <summary>
            ''' Set to adjust the ovelap level adjustment step size for smoother or faster crossfade (in seconds)
            ''' </summary>
            ''' <param name="Granuality"></param>
            Sub SetOverlapGranuality(ByVal Granuality As Double)

            Function GetOverlapGranuality() As Double

            ''' <summary>
            ''' Swaps the current output sound to a new, using crossfading between ths sounds.
            ''' </summary>
            ''' <param name="NewOutputSound"></param>
            ''' <returns>Returns True if successful, or False if unsuccessful.</returns>
            Function SwapOutputSounds(ByRef NewOutputSound As Sound) As Boolean

            ''' <summary>
            ''' Fades out of the output sound (The fade out will occur during OverlapFadeLength +1 samples.
            ''' </summary>
            Sub FadeOutPlayback()

        End Interface

    End Namespace

End Namespace
