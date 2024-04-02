
Namespace Audio
    Namespace SoundPlayers

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


        Public Interface iSoundPlayer

            ReadOnly Property WideFormatSupport As Boolean

            Enum FadeTypes
                Linear
                Smooth
            End Enum

            Enum SoundDirections
                PlaybackOnly
                RecordingOnly
                Duplex
            End Enum

            Event MessageFromPlayer(ByVal Message As String)

            Property RaisePlaybackBufferTickEvents As Boolean

            Property EqualPowerCrossFade As Boolean

            ''' <summary>
            ''' Set the crossfade duration (in seconds)
            ''' </summary>
            ''' <param name="Duration"></param>
            Sub SetOverlapDuration(ByVal Duration As Double)

            Function GetOverlapDuration() As Double

            ''' <summary>
            ''' Set to adjust the number of ovelap level adjustment steps for smoother or faster crossfade
            ''' </summary>
            ''' <param name="Granuality"></param>
            Sub SetOverlapGranuality(ByVal Granuality As Integer)

            Function GetOverlapGranuality() As Integer

            ReadOnly Property IsPlaying As Boolean

            Event StartedSwappingOutputSounds()

            Event FinishedSwappingOutputSounds()

            ''' <summary>
            ''' Swaps the current output sound to a new, using crossfading between ths sounds.
            ''' </summary>
            ''' <param name="NewOutputSound"></param>
            ''' <returns>Returns True if successful, or False if unsuccessful.</returns>
            Function SwapOutputSounds(ByRef NewOutputSound As Sound, Optional ByVal Record As Boolean = False, Optional ByVal AppendRecordedSound As Boolean = False) As Boolean

            ''' <summary>
            ''' Stops recording (but playback continues) and gets the sound recorded so far.
            ''' </summary>
            ''' <param name="ClearRecordingBuffer">Set to True to clear the buffer of recorded sound after the sound has been retrieved. Set to False to keep the recorded sound in memory, whereby it at at later time using a repeated call to GetRecordedSound.</param>
            ''' <returns></returns>
            Function GetRecordedSound(Optional ByVal ClearRecordingBuffer As Boolean = True) As Sound

            ''' <summary>
            ''' Fades out of the output sound (The fade out will occur during OverlapFadeLength +1 samples.
            ''' </summary>
            Sub FadeOutPlayback() ' TODO: Perhaps FadeOutPlayback ought to take the Record and AppendRecordedSound parameters as SwapOutputSounds?

            ''' <summary>
            ''' Can be used to change player settings. Not needed in all sound players, and will be ignored if not needed.
            ''' </summary>
            ''' <param name="AudioApiSettings"></param>
            ''' <param name="SampleRate"></param>
            ''' <param name="BitDepth"></param>
            ''' <param name="Encoding"></param>
            ''' <param name="OverlapDuration"></param>
            ''' <param name="Mixer"></param>
            ''' <param name="SoundDirection"></param>
            ''' <param name="ReOpenStream"></param>
            ''' <param name="ReStartStream"></param>
            ''' <param name="ClippingIsActivated"></param>
            Sub ChangePlayerSettings(Optional ByRef AudioApiSettings As AudioApiSettings = Nothing,
                                Optional ByVal SampleRate As Integer? = Nothing,
                                Optional ByVal BitDepth As Integer? = Nothing,
                                Optional ByVal Encoding As Audio.Formats.WaveFormat.WaveFormatEncodings? = Nothing,
                                Optional ByVal OverlapDuration As Double? = Nothing,
                                Optional ByRef Mixer As Audio.SoundScene.DuplexMixer = Nothing,
                                Optional ByVal SoundDirection As SoundDirections? = Nothing,
                                Optional ByVal ReOpenStream As Boolean = True,
                                Optional ByVal ReStartStream As Boolean = True,
                                Optional ByVal ClippingIsActivated As Boolean? = Nothing)


            ''' <summary>
            ''' Can be called to close the sound stream (and of course also stop playing) in sound players that don't close the stream automatically. All players should stop playing upon this call.
            ''' </summary>
            Sub CloseStream()

            ''' <summary>
            ''' Can be used to Dispose soundplayers that need to be displosed. Will be ignored in sound players that do not have to be disposed.
            ''' </summary>
            Sub Dispose()

        End Interface


    End Namespace
End Namespace
