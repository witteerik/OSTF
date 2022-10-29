Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows.Forms

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


    Namespace PortAudioVB

        Public Class OverlappingSoundPlayer
            Implements IDisposable

            Public Event MessageFromPlayer(ByVal Message As String)

            Private Mixer As DuplexMixer

            Public Function GetMixer() As DuplexMixer
                Return Mixer
            End Function

            Private SelectedApiInfo As PortAudio.PaHostApiInfo
            Private SelectedInputDeviceInfo As PortAudio.PaDeviceInfo?
            Private SelectedInputDevice As Integer?
            Private SelectedOutputDeviceInfo As PortAudio.PaDeviceInfo?
            Private SelectedOutputDevice As Integer?
            Private SelectedInputAndOutputDeviceInfo As PortAudio.PaDeviceInfo?

            'MME multiple devices support
            Private UseMmeMultipleDevices As Boolean = False 'N.B. It may be possible to use the MME multiple device support for output and at the same time not using it for input (i.e. signle input device). We have however not tested this. The same functionality can be attained by specifying only a single input device in InputDevices (i.e. so that NumberOfWinMmeInputDevices = 1).
            Private PaWinMmeOutputDeviceAndChannelCountArray() As Integer = {}
            Private WinMmeSuggestedOutputLatency As Double
            Private NumberOfWinMmeOutputDevices As Integer
            Private PaWinMmeInputDeviceAndChannelCountArray() As Integer = {}
            Private WinMmeSuggestedInputLatency As Double
            Private NumberOfWinMmeInputDevices As Integer

            Private Stream As IntPtr
            Public FramesPerBuffer As UInteger
            Private PlaybackBuffer As Single() = New Single(511) {}
            Private RecordingBuffer As Single() = New Single(511) {}
            Private SilentBuffer As Single() = New Single(511) {}

            Private CallbackSpinLock As New Threading.SpinLock

            Private paStreamCallback As PortAudio.PaStreamCallbackDelegate = Function(input As IntPtr, output As IntPtr, frameCount As UInteger, ByRef timeInfo As PortAudio.PaStreamCallbackTimeInfo, statusFlags As PortAudio.PaStreamCallbackFlags, userData As IntPtr) As PortAudio.PaStreamCallbackResult

                                                                                 'Sending a buffer tick to the controller
                                                                                 'Temporarily outcommented, until better solutions is fixed: SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.NewBufferTick)

                                                                                 'Declaring a spin lock taken variable
                                                                                 Dim SpinLockTaken As Boolean = False


                                                                                 Try

                                                                                     'Attempts to enter a spin lock to avoid multiple threads calling before compete
                                                                                     CallbackSpinLock.Enter(SpinLockTaken)

                                                                                     'INPUT SOUND

                                                                                     If RecordingIsActive = True Then
                                                                                         'Getting input sound

                                                                                         'This need to be changed for real time recording, the audio need to be converted to Sound format, or maybe it could be fixed after stoppong the recording?
                                                                                         Dim InputBuffer(RecordingBuffer.Length - 1) As Single
                                                                                         Marshal.Copy(input, InputBuffer, 0, FramesPerBuffer * NumberOfInputChannels)
                                                                                         InputBufferHistory.Add(InputBuffer)
                                                                                     End If


                                                                                     'OUTPUT SOUND
                                                                                     If PlaybackIsActive = True Then

                                                                                         'Checking if the current sound should be swapped (if there is a new sound in NewSound)
                                                                                         If NewSound IsNot Nothing Then

                                                                                             'Swapping sound
                                                                                             Select Case CurrentOutputSound
                                                                                                 Case OutputSounds.OutputSoundA, OutputSounds.FadingToA
                                                                                                     OutputSoundB = NewSound
                                                                                                     NewSound = Nothing
                                                                                                     CurrentOutputSound = OutputSounds.FadingToB

                                                                                                     'Setting reading position
                                                                                                     PositionB = 0

                                                                                                 Case OutputSounds.OutputSoundB, OutputSounds.FadingToB
                                                                                                     OutputSoundA = NewSound
                                                                                                     NewSound = Nothing
                                                                                                     CurrentOutputSound = OutputSounds.FadingToA

                                                                                                     'Setting reading position
                                                                                                     PositionA = 0
                                                                                             End Select

                                                                                             'Setting CrossFadeProgress to 0 since a new fade period have begun
                                                                                             CrossFadeProgress = 0

                                                                                         End If

                                                                                         'Checking current positions to see if an EndOfBufferAlert should be sent
                                                                                         Select Case CurrentOutputSound
                                                                                             Case OutputSounds.OutputSoundA, OutputSounds.FadingToA

                                                                                                 'Copying the silent buffer if the end of sound A is reached
                                                                                                 If PositionA = OutputSoundA.Length - ApproachingEndOfBufferAlert_BufferCount Then
                                                                                                     'Temporarily outcommented, until better solutions is fixed: SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.ApproachingEndOfBufferAlert)
                                                                                                 End If

                                                                                             Case OutputSounds.OutputSoundB, OutputSounds.FadingToB

                                                                                                 'Copying the silent buffer if the end of sound A is reached
                                                                                                 If PositionB = OutputSoundB.Length - ApproachingEndOfBufferAlert_BufferCount Then
                                                                                                     'Temporarily outcommented, until better solutions is fixed: SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.ApproachingEndOfBufferAlert)
                                                                                                 End If

                                                                                             Case Else
                                                                                                 Throw New NotImplementedException
                                                                                         End Select

                                                                                         'Console.WriteLine(CrossFadeProgress & " " & FadeArrayLength & " " & CurrentOutputSound.ToString & " " & PositionA & " " & PositionB & " " & CrossFadeProgress & " " & OverlapFadeInArray.Length & " " & OverlapFadeOutArray.Length)


                                                                                         'Copying buffers 
                                                                                         Select Case CurrentOutputSound
                                                                                             Case OutputSounds.OutputSoundA

                                                                                                 'Copying the silent buffer if the end of sound A is reached
                                                                                                 If PositionA >= OutputSoundA.Length Then
                                                                                                     Marshal.Copy(SilentBuffer, 0, output, SilentBuffer.Length)

                                                                                                     'Sending message to the controller
                                                                                                     'Temporarily outcommented, until better solutions is fixed: SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.EndOfSound)

                                                                                                     If StopAtOutputSoundEnd = True Then

                                                                                                         'Returning the callback to port audio
                                                                                                         Return PortAudio.PaStreamCallbackResult.paComplete
                                                                                                     Else
                                                                                                         Return PortAudio.PaStreamCallbackResult.paContinue
                                                                                                     End If
                                                                                                 End If

                                                                                                 PlaybackBuffer = OutputSoundA(PositionA).InterleavedSampleArray

                                                                                                 'Copying the playback buffer to unmanaged memory
                                                                                                 Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 PositionA += 1

                                                                                             Case OutputSounds.OutputSoundB

                                                                                                 'Copying the silent buffer if the end of sound B is reached
                                                                                                 If PositionB >= OutputSoundB.Length Then
                                                                                                     Marshal.Copy(SilentBuffer, 0, output, SilentBuffer.Length)

                                                                                                     'Sending message to the controller
                                                                                                     'Temporarily outcommented, until better solutions is fixed: SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.EndOfSound)

                                                                                                     If StopAtOutputSoundEnd = True Then

                                                                                                         'Returning the callback to port audio
                                                                                                         Return PortAudio.PaStreamCallbackResult.paComplete
                                                                                                     Else
                                                                                                         Return PortAudio.PaStreamCallbackResult.paContinue
                                                                                                     End If
                                                                                                 End If

                                                                                                 PlaybackBuffer = OutputSoundB(PositionB).InterleavedSampleArray

                                                                                                 'Copying the playback buffer to unmanaged memory
                                                                                                 Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 PositionB += 1

                                                                                             Case OutputSounds.FadingToA

                                                                                                 If PositionA < OutputSoundA.Length And PositionB < OutputSoundB.Length Then

                                                                                                     'Mixing sound A and B to the buffer
                                                                                                     For j As Integer = 0 To PlaybackBuffer.Length - 1
                                                                                                         PlaybackBuffer(j) = OutputSoundB(PositionB).InterleavedSampleArray(j) * OverlapFadeOutArray(CrossFadeProgress) + OutputSoundA(PositionA).InterleavedSampleArray(j) * OverlapFadeInArray(CrossFadeProgress)
                                                                                                         CrossFadeProgress += 1
                                                                                                     Next

                                                                                                     'Copying the playback buffer to unmanaged memory
                                                                                                     Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 ElseIf PositionA < OutputSoundA.Length And PositionB >= OutputSoundB.Length Then

                                                                                                     'Copying only sound A to the buffer
                                                                                                     For j As Integer = 0 To PlaybackBuffer.Length - 1
                                                                                                         PlaybackBuffer(j) = OutputSoundA(PositionA).InterleavedSampleArray(j) * OverlapFadeInArray(CrossFadeProgress)
                                                                                                         CrossFadeProgress += 1
                                                                                                     Next

                                                                                                     'Copying the playback buffer to unmanaged memory
                                                                                                     Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 ElseIf PositionA >= OutputSoundA.Length And PositionB < OutputSoundB.Length Then

                                                                                                     'Copying only sound B to the buffer
                                                                                                     For j As Integer = 0 To PlaybackBuffer.Length - 1
                                                                                                         PlaybackBuffer(j) = OutputSoundB(PositionB).InterleavedSampleArray(j) * OverlapFadeOutArray(CrossFadeProgress)
                                                                                                         CrossFadeProgress += 1
                                                                                                     Next

                                                                                                     'Copying the playback buffer to unmanaged memory
                                                                                                     Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 Else
                                                                                                     'End of both sounds: Copying silence
                                                                                                     CrossFadeProgress = FadeArrayLength
                                                                                                     Marshal.Copy(SilentBuffer, 0, output, SilentBuffer.Length)

                                                                                                     'Sending message to the controller
                                                                                                     'Temporarily outcommented, until better solutions is fixed: SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.EndOfSound)

                                                                                                     If StopAtOutputSoundEnd = True Then

                                                                                                         'Returning the callback to port audio
                                                                                                         Return PortAudio.PaStreamCallbackResult.paComplete
                                                                                                     End If
                                                                                                 End If

                                                                                                 PositionA += 1
                                                                                                 PositionB += 1

                                                                                                 'Changing to OutputSounds.OutputSoundA and Resetting the CrossFadeProgress, if fading is completed
                                                                                                 If CrossFadeProgress >= FadeArrayLength - 1 Then
                                                                                                     'Console.WriteLine("FadeEnd: " & CrossFadeProgress & " " & FadeArrayLength & " " & CurrentOutputSound.ToString & " " & PositionA & " " & PositionB & " " & CrossFadeProgress & " " & OverlapFadeInArray.Length & " " & OverlapFadeOutArray.Length)
                                                                                                     CurrentOutputSound = OutputSounds.OutputSoundA
                                                                                                     CrossFadeProgress = 0
                                                                                                 End If

                                                                                             Case OutputSounds.FadingToB

                                                                                                 If PositionA < OutputSoundA.Length And PositionB < OutputSoundB.Length Then

                                                                                                     'Mixing sound A and B to the buffer
                                                                                                     For j As Integer = 0 To PlaybackBuffer.Length - 1
                                                                                                         PlaybackBuffer(j) = OutputSoundB(PositionB).InterleavedSampleArray(j) * OverlapFadeInArray(CrossFadeProgress) + OutputSoundA(PositionA).InterleavedSampleArray(j) * OverlapFadeOutArray(CrossFadeProgress)
                                                                                                         CrossFadeProgress += 1
                                                                                                     Next

                                                                                                     'Copying the playback buffer to unmanaged memory
                                                                                                     Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 ElseIf PositionA < OutputSoundA.Length And PositionB >= OutputSoundB.Length Then

                                                                                                     'Copying only sound A to the buffer
                                                                                                     For j As Integer = 0 To PlaybackBuffer.Length - 1
                                                                                                         PlaybackBuffer(j) = OutputSoundA(PositionA).InterleavedSampleArray(j) * OverlapFadeOutArray(CrossFadeProgress)
                                                                                                         CrossFadeProgress += 1
                                                                                                     Next

                                                                                                     'Copying the playback buffer to unmanaged memory
                                                                                                     Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 ElseIf PositionA >= OutputSoundA.Length And PositionB < OutputSoundB.Length Then

                                                                                                     'Copying only sound B to the buffer
                                                                                                     For j As Integer = 0 To PlaybackBuffer.Length - 1
                                                                                                         PlaybackBuffer(j) = OutputSoundB(PositionB).InterleavedSampleArray(j) * OverlapFadeInArray(CrossFadeProgress)
                                                                                                         CrossFadeProgress += 1
                                                                                                     Next

                                                                                                     'Copying the playback buffer to unmanaged memory
                                                                                                     Marshal.Copy(PlaybackBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels) 'PlaybackBuffer.length?

                                                                                                 Else
                                                                                                     'End of both sounds: Copying silence
                                                                                                     CrossFadeProgress = FadeArrayLength
                                                                                                     Marshal.Copy(SilentBuffer, 0, output, SilentBuffer.Length)

                                                                                                     'Sending message to the controller
                                                                                                     'Temporarily outcommented, until better solutions is fixed: SendMessageToController(PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer.EndOfSound)

                                                                                                     If StopAtOutputSoundEnd = True Then

                                                                                                         'Returning the callback to port audio
                                                                                                         Return PortAudio.PaStreamCallbackResult.paComplete
                                                                                                     End If
                                                                                                 End If

                                                                                                 PositionA += 1
                                                                                                 PositionB += 1

                                                                                                 'Changing to OutputSounds.OutputSoundA and Resetting the CrossFadeProgress, if fading is completed
                                                                                                 If CrossFadeProgress >= FadeArrayLength - 1 Then
                                                                                                     'Console.WriteLine("FadeEnd: " & CrossFadeProgress & " " & FadeArrayLength & " " & CurrentOutputSound.ToString & " " & PositionA & " " & PositionB & " " & CrossFadeProgress & " " & OverlapFadeInArray.Length & " " & OverlapFadeOutArray.Length)
                                                                                                     CurrentOutputSound = OutputSounds.OutputSoundB
                                                                                                     CrossFadeProgress = 0
                                                                                                 End If

                                                                                                 'Case Else 'This is unnecessary as an exception would be thrown already above
                                                                                                 '    Throw New NotImplementedException
                                                                                         End Select
                                                                                     End If


                                                                                     Return PortAudio.PaStreamCallbackResult.paContinue

                                                                                 Catch ex As Exception

                                                                                     'Console.WriteLine("Error: " & CrossFadeProgress & " " & FadeArrayLength & " " & CurrentOutputSound.ToString & " " & PositionA & " " & PositionB & " " & CrossFadeProgress & " " & OverlapFadeInArray.Length & " " & OverlapFadeOutArray.Length)

                                                                                     SendInfoToAudioLog(CurrentOutputSound.ToString & " " & PositionA & " " & PositionB & " " & CrossFadeProgress & " " & OverlapFadeInArray.Length & " " & OverlapFadeOutArray.Length & vbCrLf &
                                                                                               ex.ToString, "ExceptionsDuringTesting")

                                                                                     'Returning silence if an exception occurred
                                                                                     Marshal.Copy(SilentBuffer, 0, output, FramesPerBuffer * NumberOfOutputChannels)
                                                                                     Return PortAudio.PaStreamCallbackResult.paContinue

                                                                                 Finally

                                                                                     'Releases any spinlock
                                                                                     If SpinLockTaken = True Then CallbackSpinLock.Exit()

                                                                                 End Try

                                                                             End Function


            Private SoundDirection As SoundDirections
            Private NumberOfOutputChannels As Integer
            Private NumberOfInputChannels As Integer

            Private SampleRate As Double
            'As the OSTF library stores sound data as Single (i.e. float) arrays, "paFloat32" is the only "PaSampleFormat" that can be played in without conversion. Therefore the player requires a bitdepth of 32 and a IEEE encoding of the data.
            Private Const Required_AudioEncoding As Formats.WaveFormat.WaveFormatEncodings = Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints
            Public Const Required_BitDepth As Integer = 32
            Private Const Required_PaSampleFormat As PortAudio.PaSampleFormat = PortAudio.PaSampleFormat.paFloat32

            Private _IsInitialized As Boolean = False
            Public ReadOnly Property IsInitialized As Boolean
                Get
                    Return _IsInitialized
                End Get
            End Property

            Private OutputSoundA As BufferHolder()
            Private OutputSoundB As BufferHolder()
            Private NewSound As BufferHolder()
            Private SilentSound As BufferHolder()

            Private OverlappingSounds As Boolean = False
            Private EqualPowerCrossFade As Boolean = True
            Public Enum FadeTypes
                Linear
                Smooth
            End Enum

            Private CurrentOutputSound As OutputSounds = OutputSounds.OutputSoundA
            Private Enum OutputSounds
                OutputSoundA
                OutputSoundB
                FadingToB
                FadingToA
            End Enum

            Private ReadOnly Property FadeArrayLength As Integer
                Get
                    Return NumberOfOutputChannels * _OverlapFrameCount
                End Get
            End Property


            Private _OverlapFrameCount As Double
            ''' <summary>
            ''' A value that holds the number of overlapping frames between two sounds. Setting this value automatically creates overlap fade arrays (OverlapFadeInArray and OverlapFadeOutArray). 
            ''' </summary>
            ''' <returns></returns>
            Private Property OverlapFrameCount As Double
                Get
                    Return _OverlapFrameCount
                End Get
                Set(value As Double)
                    Try

                        'Enforcing overlap fade length to be a multiple of FramesPerBuffer
                        _OverlapFrameCount = FramesPerBuffer * Math.Ceiling(value / FramesPerBuffer)

                        Dim FadeArrayLength As Integer = NumberOfOutputChannels * _OverlapFrameCount

                        'Linear fading
                        'fade in array
                        ReDim OverlapFadeInArray(FadeArrayLength - 1)
                        For n = 0 To _OverlapFrameCount - 1
                            For c = 0 To NumberOfOutputChannels - 1
                                OverlapFadeInArray(n * NumberOfOutputChannels + c) = n / (_OverlapFrameCount - 1)
                            Next
                        Next

                        'fade out array
                        ReDim OverlapFadeOutArray(FadeArrayLength - 1)
                        For n = 0 To _OverlapFrameCount - 1
                            For c = 0 To NumberOfOutputChannels - 1
                                OverlapFadeOutArray(n * NumberOfOutputChannels + c) = 1 - (n / (_OverlapFrameCount - 1))
                            Next
                        Next


                        'Adjusting to equal power fades
                        If EqualPowerCrossFade = True Then
                            For n = 0 To FadeArrayLength - 1
                                OverlapFadeInArray(n) = Math.Sqrt(OverlapFadeInArray(n))
                            Next
                            For n = 0 To FadeArrayLength - 1
                                OverlapFadeOutArray(n) = Math.Sqrt(OverlapFadeOutArray(n))
                            Next
                        End If

                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try

                End Set
            End Property

            Private OverlapFadeInArray As Single()
            Private OverlapFadeOutArray As Single()
            Private InputBufferHistory As New List(Of Single())
            Private StopAtOutputSoundEnd As Boolean

            Private PositionA As Integer
            Private PositionB As Integer
            Private CrossFadeProgress As Integer = 0

            Public Shared Property MessagesEnabled As Boolean
            Public Shared Property LoggingEnabled As Boolean
            Public Shared Property LogToFileEnabled As Boolean = False


            Private _IsPlaying As Boolean = False
            Public ReadOnly Property IsPlaying As Boolean
                Get
                    Return _IsPlaying
                End Get
            End Property

            Private _IsStreamOpen As Boolean = False
            Public ReadOnly Property IsStreamOpen As Boolean
                Get
                    Return _IsStreamOpen
                End Get
            End Property

            Private RecordingIsActive As Boolean
            Private PlaybackIsActive As Boolean

            Private _IsClippingInactivated As Boolean = False
            Public ReadOnly Property IsClippingInactivated As Boolean
                Get
                    Return _IsClippingInactivated
                End Get
            End Property

            Private ApproachingEndOfBufferAlert_BufferCount As Integer
            Private IsBufferTickActive As Boolean

            Public Enum SoundDirections
                PlaybackOnly
                RecordingOnly
                Duplex
            End Enum

            Public Sub New(Optional ByVal LoggingEnabled As Boolean = False,
                   Optional ByVal MessagesEnabled As Boolean = False,
                   Optional ByVal StopAtOutputSoundEnd As Boolean = False,
                   Optional ByVal InactivateClipping As Boolean = False,
                   Optional ByVal ApproachingEndOfBufferAlert_BufferCount As Integer = 1,
                   Optional ByVal ActivateBufferTicks As Boolean = False)

                Me.IsBufferTickActive = ActivateBufferTicks
                Me.ApproachingEndOfBufferAlert_BufferCount = ApproachingEndOfBufferAlert_BufferCount
                Me.StopAtOutputSoundEnd = StopAtOutputSoundEnd

                Try

                    'Setting clipping
                    Me._IsClippingInactivated = InactivateClipping

                    'Overriding any value set in InitializationSuccess
                    _IsInitialized = False

                    OverlappingSoundPlayer.LoggingEnabled = LoggingEnabled
                    OverlappingSoundPlayer.MessagesEnabled = MessagesEnabled
                    Log("Initializing...")

                    'Checking if PortAudio has been initialized 
                    If OstfBase.PortAudioIsInitialized = False Then Throw New Exception("The PortAudio library has not been initialized. This should have been done by a call to the function OsftBase.InitializeOSTF.")

                    'Creating a default mixer if none is supplied
                    If Mixer Is Nothing Then
                        Me.Mixer = New DuplexMixer()

                        ' Mixer_DirectMonoToAllChannels()
                        For c = 1 To NumberOfOutputChannels
                            Me.Mixer.OutputRouting.Add(c, 0)
                        Next

                        'Mixer_DirectMonoSoundToOutputChannels(
                        Dim TargetOutputChannels() As Integer = {1, 2}
                        For Each OutputChannel In TargetOutputChannels
                            If Me.Mixer.OutputRouting.ContainsKey(OutputChannel) Then Me.Mixer.OutputRouting(OutputChannel) = 1
                        Next

                        'Me.Mixer.SetLinearInput()
                        For c = 1 To NumberOfInputChannels
                            Me.Mixer.InputRouting.Add(c, 0)
                        Next


                    Else
                        Me.Mixer = Mixer
                    End If

                    _IsInitialized = True

                Catch e As Exception
                    Log(HasPaError("Terminate", PortAudio.Pa_Terminate(), True))
                    Log(e.ToString())
                End Try
            End Sub

            ''' <summary>
            ''' Stops the player and changes the supplied settings.
            ''' </summary>
            ''' <param name="WaveFormat"></param>
            ''' <param name="SoundDirection"></param>
            ''' <param name="AudioApiSettings"></param>
            ''' <param name="OverlapDuration"></param>
            Public Sub ChangePlayerSettings(Optional ByRef AudioApiSettings As AudioApiSettings = Nothing,
                                            Optional ByVal WaveFormat As Audio.Formats.WaveFormat = Nothing,
                                            Optional ByVal OverlapDuration As Double? = Nothing,
                                            Optional ByRef Mixer As DuplexMixer = Nothing,
                                            Optional ByVal SoundDirection As SoundDirections? = Nothing,
                                            Optional ByVal ReOpenStream As Boolean = True,
                                            Optional ByVal ReStartStream As Boolean = True)

                Dim WasStreamOpen As Boolean = False
                Dim WasPlaying As Boolean = False

                If IsInitialized = True Then
                    If IsStreamOpen = True Then
                        If IsPlaying = True Then
                            'Stops playing if it's playing
                            WasPlaying = True
                            StopStream()
                        End If

                        'Closes the stream if it was open
                        WasStreamOpen = True
                        CloseStream()
                    End If
                End If

                'Updating values
                If AudioApiSettings IsNot Nothing Then SetApiAndDevice(AudioApiSettings, True)

                If WaveFormat IsNot Nothing Then Me.SampleRate = WaveFormat.SampleRate

                If WaveFormat IsNot Nothing Then
                    'Checks that the Encoding is the one required 
                    If WaveFormat.Encoding <> OverlappingSoundPlayer.Required_AudioEncoding Then Throw New NotSupportedException("Unsupported sample encoding!")
                End If

                If WaveFormat IsNot Nothing Then
                    'Checks that the bit depth is the one required 
                    If WaveFormat.BitDepth <> OverlappingSoundPlayer.Required_BitDepth Then Throw New NotSupportedException("Unsupported bitdepth!")
                End If

                'Setting OverlapFadeLength (and creating fade arrays)
                If OverlapDuration.HasValue Then SetOverlapDuration(OverlapDuration)

                If Mixer IsNot Nothing Then Me.Mixer = Mixer

                If SoundDirection.HasValue Then
                    Me.SoundDirection = SoundDirection
                    'Setting PlaybackIsActive depending on the SoundDirection
                    'RecordingIsActive is set to False until playing starts
                    Select Case SoundDirection
                        Case SoundDirections.PlaybackOnly
                            PlaybackIsActive = True
                            RecordingIsActive = False
                        Case SoundDirections.RecordingOnly
                            PlaybackIsActive = False
                            RecordingIsActive = False
                        Case SoundDirections.Duplex
                            PlaybackIsActive = True
                            RecordingIsActive = False
                        Case Else
                            Throw New Exception("Invalid sound direction")
                    End Select
                End If


                'Re-opens stream if it was open upon calling this function
                If WasStreamOpen = True Or ReOpenStream = True Then
                    OpenStream()
                End If

                'Starts playing if it was playing upon calling this function
                If WasPlaying = True Or ReStartStream = True Then
                    Start()
                End If

            End Sub

            Private Sub SetApiAndDevice(Optional ByRef AudioApiSettings As AudioApiSettings = Nothing, Optional ByVal SkipLog As Boolean = False)

                'Setting API settings if not already done
                If AudioApiSettings Is Nothing Then
                    Dim newAudioSettingsDialog As New AudioSettingsDialog()
                    Dim DialogResult = newAudioSettingsDialog.ShowDialog()
                    If DialogResult = DialogResult.OK Then
                        AudioApiSettings = newAudioSettingsDialog.CurrentAudioApiSettings
                    Else
                        MsgBox("Did not initialize PaSoundPlayer due to missing audio settings.")
                        Throw New Exception("Did not initialize PaSoundPlayer due to missing audio settings.")
                        Log(HasPaError("Terminate", PortAudio.Pa_Terminate(), True))
                        Exit Sub
                    End If
                End If


                Me.SelectedApiInfo = AudioApiSettings.SelectedApiInfo
                Me.SelectedInputDeviceInfo = AudioApiSettings.SelectedInputDeviceInfo
                Me.SelectedInputDevice = AudioApiSettings.SelectedInputDevice
                Me.SelectedOutputDeviceInfo = AudioApiSettings.SelectedOutputDeviceInfo
                Me.SelectedOutputDevice = AudioApiSettings.SelectedOutputDevice
                Me.SelectedInputAndOutputDeviceInfo = AudioApiSettings.SelectedInputAndOutputDeviceInfo
                Me.FramesPerBuffer = AudioApiSettings.FramesPerBuffer

                Dim Temp_NumberOfInputChannels = AudioApiSettings.NumberOfInputChannels()
                If Temp_NumberOfInputChannels.HasValue Then
                    NumberOfInputChannels = Temp_NumberOfInputChannels
                Else
                    NumberOfInputChannels = 0
                End If

                Dim Temp_NumberOfOutputChannels = AudioApiSettings.NumberOfOutputChannels()
                If Temp_NumberOfOutputChannels.HasValue Then
                    NumberOfOutputChannels = Temp_NumberOfOutputChannels
                Else
                    NumberOfOutputChannels = 0
                End If

                'MME Multiple device support
                Me.UseMmeMultipleDevices = AudioApiSettings.UseMmeMultipleDevices
                Me.PaWinMmeOutputDeviceAndChannelCountArray = AudioApiSettings.PaWinMmeOutputDeviceAndChannelCountArray
                Me.PaWinMmeInputDeviceAndChannelCountArray = AudioApiSettings.PaWinMmeInputDeviceAndChannelCountArray
                Me.WinMmeSuggestedOutputLatency = AudioApiSettings.WinMmeSuggestedOutputLatency
                Me.WinMmeSuggestedInputLatency = AudioApiSettings.WinMmeSuggestedInputLatency
                Me.NumberOfWinMmeOutputDevices = AudioApiSettings.NumberOfWinMmeOutputDevices
                Me.NumberOfWinMmeInputDevices = AudioApiSettings.NumberOfWinMmeInputDevices

                If SkipLog = False Then
                    Log("Selected HostAPI:" & vbLf & Me.SelectedApiInfo.ToString())
                    If Not Me.SelectedInputDeviceInfo Is Nothing Then Log("Selected input device:" & vbLf & Me.SelectedInputDeviceInfo.ToString())
                    If Not Me.SelectedOutputDeviceInfo Is Nothing Then Log("Selected output device:" & vbLf & Me.SelectedOutputDeviceInfo.ToString())
                    If Not Me.SelectedInputAndOutputDeviceInfo Is Nothing Then Log("Selected input and output device:" & vbLf & Me.SelectedInputAndOutputDeviceInfo.ToString())
                End If

            End Sub


            Private Sub SetOverlapDuration(ByVal Duration As Single)
                OverlapFrameCount = SampleRate * Duration
            End Sub

            Public Function GetOverlapDuration() As Single
                Return _OverlapFrameCount / SampleRate
            End Function


            ''' <summary>
            ''' Swaps the current output sound to a new, using crossfading between ths sounds.
            ''' </summary>
            ''' <param name="NewOutputSound"></param>
            ''' <param name="Record">Activates recording if set to True in Duxplex and RecordingOnly modes.</param>
            ''' <returns>Returns True if successful, or False if unsuccessful.</returns>
            Public Function SwapOutputSounds(ByRef NewOutputSound As Sound, Optional ByVal Record As Boolean = False, Optional ByVal AppendRecordedSound As Boolean = False) As Boolean

                'Fading out playback if the new output sound is Nothing (but continues to record sound if recording is active)
                If NewOutputSound Is Nothing Then
                    FadeOutPlayback()
                    'Return False
                End If

                ''Checks that sound is playing
                'If _IsPlaying = False Then
                '    Log("Error: SwapOutputSounds is only effective during active playback.")
                '    Return False
                'End If

                'Checking that the new sound is at least 1 sample long
                If NewOutputSound IsNot Nothing Then

                    'Checking that the sound has the required bitdepth
                    If NewOutputSound.WaveFormat.BitDepth <> OverlappingSoundPlayer.Required_BitDepth Then
                        Throw New ArgumentException("Unsupported bitdepth detected (OverlappingSoundPLayer only supportes a bitdepth of 32!)")
                    End If

                    If NewOutputSound.WaveFormat.Encoding <> OverlappingSoundPlayer.Required_AudioEncoding Then
                        Throw New ArgumentException("Unsupported bitdepth detected (OverlappingSoundPLayer only supportes a bitdepth of 32!)")
                    End If

                    If NewOutputSound.WaveData.LongestChannelSampleCount = 0 Then
                        Log("Error: New sound contains no sample data (SwapOutputSounds).")
                        Return False
                    End If

                    If NewOutputSound.WaveData.HasUnequalNonZeroChannelLength = True Then
                        Log("Error: New sound have non-empty channels that differ in length. This is not allowed in SwapOutputSounds.")
                        Return False
                    End If

                    'Checking that the sample rate is the same, and returns False if not
                    If NewOutputSound.WaveFormat.SampleRate <> Me.SampleRate Then
                        Log("Error: Different formats in SwapOutputSounds.")
                        Return False
                    End If
                End If


                'Activating recording (if not in PlaybackOnly mode)
                If Record = True Then

                    'Resetting recorded sound
                    If AppendRecordedSound = False Then
                        ClearRecordedSound()
                    End If

                    ActivateRecording()
                Else
                    RecordingIsActive = False
                End If

                'Setting NewSound to the NewOutputSound to indicate that the output sound should be swapped by the callback
                'NewSound = CreateBufferHolders(NewOutputSound)
                If NewOutputSound IsNot Nothing Then
                    NewSound = CreateBufferHoldersOnNewThread(NewOutputSound)
                End If

                Return True

            End Function


            Public Function CreateBufferHoldersOnNewThread(ByRef InputSound As Sound, Optional ByVal BuffersOnMainThread As Integer = 10) As BufferHolder()

                Dim BufferCount As Integer = Int(InputSound.WaveData.LongestChannelSampleCount / FramesPerBuffer) + 1

                Dim Output(BufferCount - 1) As BufferHolder

                'Initializing the BufferHolders
                For b = 0 To Output.Length - 1
                    Output(b) = New BufferHolder(NumberOfOutputChannels, FramesPerBuffer)
                Next

                'Creating the BuffersOnMainThread first buffers
                'Limiting the number of main thread buffers if the sound is very short
                If (Output.Length - 1) < BuffersOnMainThread Then
                    BuffersOnMainThread = Math.Max(0, Output.Length - 1)
                End If


                Dim CurrentChannelInterleavedPosition As Integer
                For Each OutputRouting In Mixer.OutputRouting

                    If OutputRouting.Value = 0 Then Continue For

                    If OutputRouting.Value > InputSound.WaveFormat.Channels Then Continue For

                    'Skipping if channel contains no data
                    If InputSound.WaveData.SampleData(OutputRouting.Value).Length = 0 Then Continue For

                    'Calculates the calibration gain
                    Dim CalibrationGainFactor = 10 ^ (Mixer.CalibrationGain(OutputRouting.Key) / 20)

                    CurrentChannelInterleavedPosition = OutputRouting.Key - 1

                    'Going through buffer by buffer
                    For BufferIndex = 0 To BuffersOnMainThread - 1

                        'Setting start sample and time
                        Output(BufferIndex).StartSample = BufferIndex * FramesPerBuffer

                        'Shuffling samples from the input sound to the interleaved array
                        Dim CurrentWriteSampleIndex As Integer = 0
                        For Sample = BufferIndex * FramesPerBuffer To (BufferIndex + 1) * FramesPerBuffer - 1

                            Dim x = CurrentWriteSampleIndex * NumberOfOutputChannels + CurrentChannelInterleavedPosition

                            Output(BufferIndex).InterleavedSampleArray(CurrentWriteSampleIndex * NumberOfOutputChannels + CurrentChannelInterleavedPosition) = InputSound.WaveData.SampleData(OutputRouting.Value)(Sample) * CalibrationGainFactor
                            CurrentWriteSampleIndex += 1
                        Next
                    Next
                Next

                'Fixes the rest of the buffers on a new thread, allowing the new sound to start playing
                Dim ThreadWork As New BufferCreaterOnNewThread(InputSound, Output, BuffersOnMainThread,
                                                               NumberOfOutputChannels, Mixer, FramesPerBuffer)

                Return Output

            End Function

            Private Class BufferCreaterOnNewThread
                Implements IDisposable

                Private InputSound As Sound
                Private Output As BufferHolder()
                Private BuffersOnMainThread As Integer
                Private NumberOfOutputChannels As Integer
                Private Mixer As DuplexMixer
                Private FramesPerBuffer As UInteger

                Public Sub New(ByRef InputSound As Sound, ByRef Output As BufferHolder(), ByVal BuffersOnMainThread As Integer,
                         ByVal NumberOfOutputChannels As Integer, ByRef Mixer As DuplexMixer, ByVal FramesPerBuffer As UInteger)
                    Me.InputSound = InputSound
                    Me.Output = Output
                    Me.BuffersOnMainThread = BuffersOnMainThread
                    Me.NumberOfOutputChannels = NumberOfOutputChannels
                    Me.Mixer = Mixer
                    Me.FramesPerBuffer = FramesPerBuffer

                    'Starting the new worker thread
                    Dim NewThred As New Thread(AddressOf DoWork)
                    NewThred.IsBackground = True
                    NewThred.Start()

                End Sub

                Private Sub DoWork()

                    Dim CurrentChannelInterleavedPosition As Integer
                    For Each OutputRouting In Mixer.OutputRouting

                        If OutputRouting.Value = 0 Then Continue For

                        If OutputRouting.Value > InputSound.WaveFormat.Channels Then Continue For

                        'Skipping if channel contains no data
                        If InputSound.WaveData.SampleData(OutputRouting.Value).Length = 0 Then Continue For

                        'Calculates the calibration gain
                        Dim CalibrationGainFactor = 10 ^ (Mixer.CalibrationGain(OutputRouting.Key) / 20)

                        CurrentChannelInterleavedPosition = OutputRouting.Key - 1

                        'Going through buffer by buffer
                        For BufferIndex = BuffersOnMainThread To Output.Length - 2

                            'Setting start sample 
                            Output(BufferIndex).StartSample = BufferIndex * FramesPerBuffer

                            'Shuffling samples from the input sound to the interleaved array, and also applied the calibration gain for the output hardware channel
                            Dim CurrentWriteSampleIndex As Integer = 0
                            For Sample = BufferIndex * FramesPerBuffer To (BufferIndex + 1) * FramesPerBuffer - 1

                                Output(BufferIndex).InterleavedSampleArray(CurrentWriteSampleIndex * NumberOfOutputChannels + CurrentChannelInterleavedPosition) = InputSound.WaveData.SampleData(OutputRouting.Value)(Sample) * CalibrationGainFactor
                                CurrentWriteSampleIndex += 1
                            Next
                        Next

                        'Reading the last bit
                        'Setting start sample 
                        Output(Output.Length - 1).StartSample = (Output.Length - 1) * FramesPerBuffer

                        'Shuffling samples from the input sound to the interleaved array
                        Dim CurrentWriteSampleIndexB As Integer = 0
                        For Sample = FramesPerBuffer * (Output.Length - 1) To InputSound.WaveData.SampleData(OutputRouting.Value).Length - 1

                            Output(Output.Length - 1).InterleavedSampleArray(CurrentWriteSampleIndexB * NumberOfOutputChannels + CurrentChannelInterleavedPosition) = InputSound.WaveData.SampleData(OutputRouting.Value)(Sample) * CalibrationGainFactor
                            CurrentWriteSampleIndexB += 1
                        Next
                    Next

                    'Disposing Me
                    Me.Dispose()

                End Sub

#Region "IDisposable Support"
                Private disposedValue As Boolean ' To detect redundant calls

                ' IDisposable
                Protected Overridable Sub Dispose(disposing As Boolean)
                    If Not disposedValue Then
                        If disposing Then
                            ' TODO: dispose managed state (managed objects).
                        End If

                        ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                        ' TODO: set large fields to null.
                    End If
                    disposedValue = True
                End Sub

                ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
                'Protected Overrides Sub Finalize()
                '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                '    Dispose(False)
                '    MyBase.Finalize()
                'End Sub

                ' This code added by Visual Basic to correctly implement the disposable pattern.
                Public Sub Dispose() Implements IDisposable.Dispose
                    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                    Dispose(True)
                    ' TODO: uncomment the following line if Finalize() is overridden above.
                    ' GC.SuppressFinalize(Me)
                End Sub
#End Region
            End Class

            ''' <summary>
            ''' Changes the position of the currently playing sound to the start of the buffer containing the indicated time (in seconds). Returns the exact time selected as new start time. 
            ''' </summary>
            Public Function SeekTime(ByVal Time As Single) As Single

                Dim SelectedSample As Integer = SeekSample(Math.Floor(Time * SampleRate))

                Dim SelectedTime As Single = SelectedSample / SampleRate

                Return SelectedTime

            End Function

            ''' <summary>
            ''' Changes the position of the currently playing sound to the start of the buffer containing the indicated sample. Returns the exact sample selected as new start position. 
            ''' </summary>
            Public Function SeekSample(ByVal StartSample As Integer) As Integer

                Dim SelectedSample As Integer

                Select Case CurrentOutputSound
                    Case OutputSounds.OutputSoundA, OutputSounds.FadingToA

                        'Locating the buffer containing the indicated start sample
                        Dim NewStartPosition As Integer = 0
                        For BufferIndex = 0 To OutputSoundA.Length - 1
                            If StartSample < OutputSoundA(BufferIndex).StartSample Then
                                NewStartPosition = BufferIndex
                                Exit For
                            End If
                        Next

                        'Limiting NewStartPosition to positive values, and values lower than the length of OutputSoundA, and then sets the new PositionA
                        PositionA = Math.Min(Math.Max(0, NewStartPosition), OutputSoundA.Length - 1)

                        'Stores the selected start time
                        SelectedSample = OutputSoundA(PositionA).StartSample

                        'Killing any fade process, and sets CurrentOutputSound to OutputSoundA
                        If CurrentOutputSound = OutputSounds.FadingToA Then
                            CurrentOutputSound = OutputSounds.OutputSoundA
                            CrossFadeProgress = 0
                        End If

                    Case OutputSounds.OutputSoundB, OutputSounds.FadingToB

                        'Locating the buffer containing the indicated start sample
                        Dim NewStartPosition As Integer = 0
                        For BufferIndex = 0 To OutputSoundB.Length - 1
                            If StartSample < OutputSoundB(BufferIndex).StartSample Then
                                NewStartPosition = BufferIndex
                                Exit For
                            End If
                        Next

                        'Limiting NewStartPosition to positive values, and values lower than the length of OutputSoundB, and then sets the new PositionB
                        PositionB = Math.Min(Math.Max(0, NewStartPosition), OutputSoundB.Length - 1)

                        'Stores the selected start time
                        SelectedSample = OutputSoundB(PositionB).StartSample

                        'Killing any fade process, and sets CurrentOutputSound to OutputSoundB
                        If CurrentOutputSound = OutputSounds.FadingToB Then
                            CurrentOutputSound = OutputSounds.OutputSoundB
                            CrossFadeProgress = 0
                        End If

                    Case Else
                        Throw New NotImplementedException
                End Select

                Return SelectedSample

            End Function


            Public Sub ClearRecordedSound()
                InputBufferHistory.Clear()
            End Sub

            Public Function OpenStream() As Boolean

                Log("Opening stream...")
                Me.Stream = StreamOpen()
                If Me.Stream = IntPtr.Zero Then
                    Return False
                Else
                    Log("Stream pointer: " & Stream.ToString())
                    Return True
                End If

            End Function




            ''' <summary>
            ''' Starts the sound stream.
            ''' </summary>
            ''' <param name="Record">Activates recording if set to True in Duxplex and RecordingOnly modes.</param>
            ''' <param name="AppendRecordedSound">If set to True, the new recording will be appended any previously recorded sound. If set to False, a new recording will be started.</param>
            Public Sub Start(Optional ByVal Record As Boolean = False, Optional ByVal AppendRecordedSound As Boolean = False)

                'Activating recording (if not in PlaybackOnly mode)
                If Record = True Then

                    'Resetting recorded sound
                    If AppendRecordedSound = False Then
                        ClearRecordedSound()
                    End If

                    ActivateRecording()
                Else
                    RecordingIsActive = False
                End If

                'Setting both sounds to silent sound
                SilentSound = {New BufferHolder(NumberOfOutputChannels, FramesPerBuffer)}
                OutputSoundA = SilentSound
                OutputSoundB = SilentSound

                Log("Starting stream")
                _IsPlaying = Not HasPaError("StartStream", PortAudio.Pa_StartStream(Stream), True)

            End Sub


            ''' <summary>
            ''' Fades out of the output sound (The fade out will occur during OverlapFadeLength +1 samples.
            ''' </summary>
            Public Sub FadeOutPlayback()

                'Doing fade out by swapping to SilentSound
                NewSound = SilentSound

            End Sub

            ''' <summary>
            ''' Activates (but do not start, nor stop, the player) the recording of the input stream (not effective in PlaybackOnly Mode).
            ''' </summary>
            Private Sub ActivateRecording()

                If SoundDirection = SoundDirections.RecordingOnly Or SoundDirection = SoundDirections.Duplex Then
                    RecordingIsActive = True
                Else
                    RecordingIsActive = False
                End If

            End Sub


            ''' <summary>
            ''' Stops the recording of the input stream (but leaves the player running)
            ''' </summary>
            Public Sub StopRecording()

                RecordingIsActive = False

            End Sub


            ''' <summary>
            ''' Stops the playback and/or recording stream
            ''' </summary>
            Public Sub StopStream()

                'Stops recording
                RecordingIsActive = False

                Log("Stopping stream...")

                'Stopping the stream if it is running
                Dim ReturnValue = PortAudio.Pa_IsStreamStopped(Me.Stream)
                If ReturnValue = 0 Then

                    'The stream in active
                    'Calls Pa_StopStream plays all pending buffers before continuing
                    If HasPaError("Pa_StopStream", PortAudio.Pa_StopStream(Me.Stream), True) = False Then
                        _IsPlaying = False
                    Else
                        'We cannot set _IsPlaying here as we don't know what has happened
                        '_IsPlaying = False
                    End If

                ElseIf ReturnValue < 0 Then
                    'An PA error has occurred
                    HasPaError("Pa_IsStreamStopped", ReturnValue, True)
                    'Calls FadeOutPlayback to set the silent sound as output
                    FadeOutPlayback()

                    'We cannot set _IsPlaying here as we don't know what has happened
                    '_IsPlaying = False

                Else
                    'the stream have already been stopped, or not started
                    _IsPlaying = False
                End If

            End Sub


            Public Sub AbortStream() 'Optional ByVal StoreInputSound As Boolean = True)

                _IsPlaying = Not HasPaError("AbortStream", PortAudio.Pa_AbortStream(Stream), True)

                'Stops recording directly
                RecordingIsActive = False

                Log("Aborting stream...")

                'If StoreInputSound = True Then
                'Storing recorded sound
                'StoreRecordedSound()
                'End If

            End Sub



            Public Sub CloseStream()

                'Colsing the stream if not stopped
                StopStream()

                'Cloing the stream
                If HasPaError("CloseStream", PortAudio.Pa_CloseStream(Stream), True) = False Then

                    _IsStreamOpen = False

                    'Resetting the stream
                    Me.Stream = New IntPtr(0)
                End If

            End Sub


            Private Function StreamOpen() As IntPtr

                'Setting buffer length data, and adjusting the length of the buffer arrays
                'Dim HighestChannelCount As Integer = Math.Max(NumberOfOutputChannels, NumberOfInputChannels)

                'Do recording and playback buffers need to be of equal length?

                'Setting/updating the length of the playback buffer
                Log("Creating a new playback buffer length with the length: " & Me.FramesPerBuffer * NumberOfOutputChannels)
                PlaybackBuffer = New Single((Me.FramesPerBuffer * NumberOfOutputChannels) - 1) {}
                SilentBuffer = New Single((Me.FramesPerBuffer * NumberOfOutputChannels) - 1) {}

                'Setting/updating the length of the recording buffer
                Log("Creating a new recording buffer length with the length: " & Me.FramesPerBuffer * NumberOfInputChannels)
                RecordingBuffer = New Single((Me.FramesPerBuffer * NumberOfInputChannels) - 1) {}

                Dim stream As New IntPtr()
                Dim data As New IntPtr(0)

                Dim inputParams As New PortAudio.PaStreamParameters
                Dim InputDeviceNumChanPtr As IntPtr
                Dim WmmeInputStreamInfoPtr As IntPtr

                Dim outputParams As New PortAudio.PaStreamParameters
                Dim OutputDeviceNumChanPtr As IntPtr
                Dim WmmeOutputStreamInfoPtr As IntPtr

                If UseMmeMultipleDevices = False Then

                    If Me.SelectedInputDevice IsNot Nothing Then
                        inputParams.channelCount = NumberOfInputChannels
                        inputParams.device = Me.SelectedInputDevice
                        inputParams.sampleFormat = Required_PaSampleFormat

                        If Me.SelectedInputAndOutputDeviceInfo.HasValue = True Then
                            inputParams.suggestedLatency = Me.SelectedInputAndOutputDeviceInfo.Value.defaultLowInputLatency
                        Else
                            inputParams.suggestedLatency = Me.SelectedInputDeviceInfo.Value.defaultLowInputLatency
                        End If
                    End If

                    If Me.SelectedOutputDevice IsNot Nothing Then
                        outputParams.channelCount = NumberOfOutputChannels
                        outputParams.device = Me.SelectedOutputDevice
                        outputParams.sampleFormat = Required_PaSampleFormat

                        If Me.SelectedInputAndOutputDeviceInfo.HasValue = True Then
                            outputParams.suggestedLatency = Me.SelectedInputAndOutputDeviceInfo.Value.defaultLowOutputLatency
                        Else
                            outputParams.suggestedLatency = Me.SelectedOutputDeviceInfo.Value.defaultLowOutputLatency
                        End If
                    End If

                Else

                    If NumberOfWinMmeInputDevices > 0 Then

                        inputParams.sampleFormat = Required_PaSampleFormat
                        inputParams.device = PortAudio.PaDeviceIndex.paUseHostApiSpecificDeviceSpecification

                        Dim wmmeInputStreamInfo As New PortAudio.PaWinMmeStreamInfo
                        wmmeInputStreamInfo.size = Marshal.SizeOf(wmmeInputStreamInfo)
                        wmmeInputStreamInfo.hostApiType = PortAudio.PaHostApiTypeId.paMME
                        wmmeInputStreamInfo.version = 1
                        wmmeInputStreamInfo.flags = PortAudio.PaWinMmeStreamInfoFlags.paWinMmeUseMultipleDevices

                        'Marshalling PaWinMmeDeviceAndChannelCount
                        InputDeviceNumChanPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(GetType(Integer)) * PaWinMmeInputDeviceAndChannelCountArray.Length)
                        Marshal.Copy(PaWinMmeInputDeviceAndChannelCountArray, 0, InputDeviceNumChanPtr, PaWinMmeInputDeviceAndChannelCountArray.Length)
                        wmmeInputStreamInfo.devices = InputDeviceNumChanPtr
                        wmmeInputStreamInfo.deviceCount = NumberOfWinMmeInputDevices

                        'Marshalling wmmeInputStreamInfoPtr
                        WmmeInputStreamInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(wmmeInputStreamInfo))
                        Marshal.StructureToPtr(wmmeInputStreamInfo, WmmeInputStreamInfoPtr, True)

                        inputParams.hostApiSpecificStreamInfo = WmmeInputStreamInfoPtr
                        inputParams.channelCount = NumberOfInputChannels
                        inputParams.suggestedLatency = WinMmeSuggestedInputLatency

                    End If


                    If NumberOfWinMmeOutputDevices > 0 Then

                        outputParams.sampleFormat = Required_PaSampleFormat
                        outputParams.device = PortAudio.PaDeviceIndex.paUseHostApiSpecificDeviceSpecification

                        Dim wmmeOutputStreamInfo As New PortAudio.PaWinMmeStreamInfo
                        wmmeOutputStreamInfo.size = Marshal.SizeOf(wmmeOutputStreamInfo)
                        wmmeOutputStreamInfo.hostApiType = PortAudio.PaHostApiTypeId.paMME
                        wmmeOutputStreamInfo.version = 1
                        wmmeOutputStreamInfo.flags = PortAudio.PaWinMmeStreamInfoFlags.paWinMmeUseMultipleDevices

                        'Marshalling PaWinMmeDeviceAndChannelCount
                        OutputDeviceNumChanPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(GetType(Integer)) * PaWinMmeOutputDeviceAndChannelCountArray.Length)
                        Marshal.Copy(PaWinMmeOutputDeviceAndChannelCountArray, 0, OutputDeviceNumChanPtr, PaWinMmeOutputDeviceAndChannelCountArray.Length)
                        wmmeOutputStreamInfo.devices = OutputDeviceNumChanPtr
                        wmmeOutputStreamInfo.deviceCount = NumberOfWinMmeOutputDevices

                        'Marshalling wmmeOutputStreamInfoPtr
                        WmmeOutputStreamInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(wmmeOutputStreamInfo))
                        Marshal.StructureToPtr(wmmeOutputStreamInfo, WmmeOutputStreamInfoPtr, True)

                        outputParams.hostApiSpecificStreamInfo = WmmeOutputStreamInfoPtr
                        outputParams.channelCount = NumberOfOutputChannels
                        outputParams.suggestedLatency = WinMmeSuggestedOutputLatency

                    End If

                End If

                Log(inputParams.ToString)
                Log(outputParams.ToString)

                Dim Flag As PortAudio.PaStreamFlags
                If IsClippingInactivated = True Then
                    Flag = PortAudio.PaStreamFlags.paClipOff
                Else
                    Flag = PortAudio.PaStreamFlags.paNoFlag
                End If


                Select Case SoundDirection
                    Case SoundDirections.PlaybackOnly
                        _IsStreamOpen = Not HasPaError("OpenOutputOnlyStream", PortAudio.Pa_OpenStream(stream, New Nullable(Of PortAudio.PaStreamParameters), outputParams,
                                                                       Me.SampleRate, Me.FramesPerBuffer, Flag, Me.paStreamCallback, data), True)

                    Case SoundDirections.RecordingOnly
                        _IsStreamOpen = Not HasPaError("OpenInputOnlyStream", PortAudio.Pa_OpenStream(stream, inputParams, New Nullable(Of PortAudio.PaStreamParameters),
                                                                      Me.SampleRate, Me.FramesPerBuffer, Flag, Me.paStreamCallback, data), True)

                    Case SoundDirections.Duplex
                        _IsStreamOpen = Not HasPaError("OpenDuplexStream", PortAudio.Pa_OpenStream(stream, inputParams, outputParams, Me.SampleRate, Me.FramesPerBuffer, Flag,
                                                                   Me.paStreamCallback, data), True)
                End Select

                If UseMmeMultipleDevices = True Then
                    If NumberOfWinMmeOutputDevices > 0 Then
                        Marshal.FreeCoTaskMem(WmmeOutputStreamInfoPtr)
                        Marshal.FreeCoTaskMem(OutputDeviceNumChanPtr)
                    End If

                    If NumberOfWinMmeInputDevices > 0 Then
                        Marshal.FreeCoTaskMem(WmmeInputStreamInfoPtr)
                        Marshal.FreeCoTaskMem(InputDeviceNumChanPtr)
                    End If

                End If

                Return stream

            End Function


            ''' <summary>
            ''' Stops recording (but playback continues) and gets the sound recorded so far.
            ''' </summary>
            ''' <param name="ClearRecordingBuffer">Set to True to clear the buffer of recorded sound after the sound has been retrieved. Set to False to keep the recorded sound in memory, whereby it at at later time using a repeated call to GetRecordedSound.</param>
            ''' <returns></returns>
            Public Function GetRecordedSound(Optional ByVal ClearRecordingBuffer As Boolean = True) As Sound

                Log("Attemting to get recorded sound")

                'Stopping recording
                StopRecording()

                'Returning nothing if no input sound exists
                If InputBufferHistory Is Nothing Then Return Nothing

                'Creating a wave format for the recorded sound
                Dim RecordingWaveFormat = New Audio.Formats.WaveFormat(SampleRate, Required_BitDepth, NumberOfInputChannels,, Required_AudioEncoding)

                'Creating a new Sound
                Dim RecordedSound As New Sound(RecordingWaveFormat)

                If InputBufferHistory.Count = 0 Then Return RecordedSound

                If InputBufferHistory.Count > 0 Then

                    'Determining output sound length
                    Dim OutputSoundSampleCount As Long = 0
                    For Each Buffer In InputBufferHistory
                        OutputSoundSampleCount += Buffer.Length / RecordingWaveFormat.Channels
                    Next

                    For ch = 0 To RecordingWaveFormat.Channels - 1
                        Dim NewChannelArray(OutputSoundSampleCount - 1) As Single
                        RecordedSound.WaveData.SampleData(ch + 1) = NewChannelArray
                    Next

                    'Sorting the interleaved samples to 
                    Dim CurrentBufferStartSample As Long = 0
                    For Each Buffer In InputBufferHistory
                        Dim CurrentBufferSampleIndex As Long = 0
                        For CurrentDataPoint = 0 To Buffer.Length - 1 Step RecordingWaveFormat.Channels

                            For ch = 0 To RecordingWaveFormat.Channels - 1
                                Try
                                    RecordedSound.WaveData.SampleData(ch + 1)(CurrentBufferStartSample + CurrentBufferSampleIndex) = Buffer(CurrentDataPoint + ch)
                                Catch ex As Exception
                                    Log(HasPaError("Terminate", PortAudio.Pa_Terminate(), True))
                                End Try
                            Next
                            'Increasing sample index
                            CurrentBufferSampleIndex += 1
                        Next
                        CurrentBufferStartSample += CurrentBufferSampleIndex
                    Next

                End If
                Return RecordedSound

                If ClearRecordingBuffer = True Then ClearRecordedSound()

            End Function

            ''' <summary>
            ''' Returns the time delay (in seconds) caused by the call-back buffer size.
            ''' </summary>
            ''' <returns></returns>
            Public Function GetCallBackTime() As Double

                Return FramesPerBuffer / SampleRate

            End Function

            Private Sub Log(logString As String)
                If LoggingEnabled = True Then
                    System.Console.WriteLine("SoundPlayer: " & logString)
                End If
            End Sub

            Private Sub DisplayMessageInBox(Message As String)
                If MessagesEnabled = True Then
                    MsgBox(Message)
                End If
            End Sub

            Private Sub LogToFile(Message As String)
                If LogToFileEnabled = True Then
                    SendInfoToAudioLog(Message)
                End If
            End Sub

            ''' <summary>
            ''' Checks whether the supplied error code contains a PortAudio error. Returns True if an error code exists or False if no error code exists.
            ''' </summary>
            ''' <param name="Action">A string representing the action attempted.</param>
            ''' <param name="ErrorCode"></param>
            ''' <param name="ShowErrorInMsgBox"></param>
            ''' <returns></returns>
            Private Function HasPaError(ByVal Action As String, ErrorCode As PortAudio.PaError, Optional ShowErrorInMsgBox As Boolean = False) As Boolean
                If ErrorCode <> PortAudio.PaError.paNoError Then

                    'An error has occurred
                    Dim MessageA As String = Action & " error: " & PortAudio.Pa_GetErrorText(ErrorCode)

                    Log(MessageA)
                    If ShowErrorInMsgBox = True Then DisplayMessageInBox(MessageA)

                    LogToFile(MessageA)

                    If ErrorCode = PortAudio.PaError.paUnanticipatedHostError Then
                        Dim errorInfo As PortAudio.PaHostErrorInfo = PortAudio.Pa_GetLastHostErrorInfo()
                        Dim MessageB As String = "- Host error API type: " & errorInfo.hostApiType
                        Dim MessageC As String = "- Host error code: " & errorInfo.errorCode
                        Dim MessageD As String = "- Host error text: " & errorInfo.errorText
                        Log(MessageB)
                        Log(MessageC)
                        Log(MessageD)

                        LogToFile(MessageB)
                        LogToFile(MessageC)
                        LogToFile(MessageD)

                        If ShowErrorInMsgBox = True Then DisplayMessageInBox(MessageB & vbCrLf & MessageC & vbCrLf & MessageD)
                    End If

                    Return True
                Else

                    'No error has occurred
                    Log(Action & " OK")
                    LogToFile(Action & " OK")
                    Return False
                End If
            End Function


            'Private Sub SendMessageToController(ByVal Message As PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer,
            '                            Optional ByVal SendOnNewThread As Boolean = True)

            '    If IsBufferTickActive = True Then
            '        If SendOnNewThread = False Then
            '            'Sending message on same thread (Should not be used when messages are sent from within the callback!)
            '            RaiseEvent MessageFromPlayer(Message)

            '        Else
            '            'Sending message on a new thread, allowing the main thread to continue execution
            '            'Dim NewthreadMessageSender As New MessageSenderOnNewThread(Message, MyController)
            '            'TODO: How to fix this? Raiseing event from separate thread?
            '        End If
            '    End If

            '    'Dim NewThread As New Thread(AddressOf SendMessage)

            '    'NewThread.Join()
            '    'Dim 
            '    'ThreadPool.QueueUserWorkItem(,)
            '    'MyController.Handle

            'End Sub

            '            ''' <summary>
            '            ''' A class used to send one ISoundPlayerControl.MessagesFromSoundPlayer message on a new thread.
            '            ''' </summary>
            '            Private Class MessageSenderOnNewThread
            '                Implements IDisposable

            '                Private Message As PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer
            '                Private Controller As PlayBack.ISoundPlayerControl

            '                ''' <summary>
            '                ''' Sends the supplied message to the indicated Controller directly on initiation and then desposes the sending object.
            '                ''' </summary>
            '                ''' <param name="Message"></param>
            '                ''' <param name="Controller"></param>
            '                Public Sub New(ByRef Message As PlayBack.ISoundPlayerControl.MessagesFromSoundPlayer, ByRef Controller As PlayBack.ISoundPlayerControl)
            '                    Me.Message = Message
            '                    Me.Controller = Controller

            '                    'Sending message on a new thread
            '                    Dim NewThred As New Thread(AddressOf SendMessage)
            '                    NewThred.Start()

            '                End Sub

            '                Private Sub SendMessage()

            '                    'Sending the message
            '                    Controller.MessageFromPlayer(Message)

            '                    'Disposing Me
            '                    Me.Dispose()

            '                End Sub

            '#Region "IDisposable Support"
            '                Private disposedValue As Boolean ' To detect redundant calls

            '                ' IDisposable
            '                Protected Overridable Sub Dispose(disposing As Boolean)
            '                    If Not disposedValue Then
            '                        If disposing Then
            '                            ' TODO: dispose managed state (managed objects).
            '                        End If

            '                        ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            '                        ' TODO: set large fields to null.
            '                    End If
            '                    disposedValue = True
            '                End Sub

            '                ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
            '                'Protected Overrides Sub Finalize()
            '                '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            '                '    Dispose(False)
            '                '    MyBase.Finalize()
            '                'End Sub

            '                ' This code added by Visual Basic to correctly implement the disposable pattern.
            '                Public Sub Dispose() Implements IDisposable.Dispose
            '                    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            '                    Dispose(True)
            '                    ' TODO: uncomment the following line if Finalize() is overridden above.
            '                    ' GC.SuppressFinalize(Me)
            '                End Sub
            '#End Region

            '            End Class


#Region "IDisposable Support"
            Private disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not disposedValue Then
                    If disposing Then
                        ' TODO: dispose managed state (managed objects).
                    End If

                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                    Log("Terminating...")
                    HasPaError("Terminate", PortAudio.Pa_Terminate(), True)

                End If
                disposedValue = True
            End Sub

            ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
            Protected Overrides Sub Finalize()
                '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(False)
                MyBase.Finalize()
            End Sub

            ' This code added by Visual Basic to correctly implement the disposable pattern.
            Public Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(True)
                ' TODO: uncomment the following line if Finalize() is overridden above.
                GC.SuppressFinalize(Me)
            End Sub

#End Region


            Public Class BufferHolder
                Public InterleavedSampleArray As Single()
                Public ChannelDataList As List(Of Single())
                Public ChannelCount As Integer
                Public FrameCount As Integer

                ''' <summary>
                ''' Holds the (0-based) index of the first sample in the current BufferHolder
                ''' </summary>
                Public StartSample As Integer

                Public Sub New(ByVal ChannelCount As Integer, ByVal FrameCount As Integer)
                    Me.ChannelCount = ChannelCount
                    Me.FrameCount = FrameCount
                    Dim NewInterleavedBuffer(ChannelCount * FrameCount - 1) As Single
                    InterleavedSampleArray = NewInterleavedBuffer
                End Sub

                Public Sub New(ByVal ChannelCount As Integer, ByVal FrameCount As Integer, ByRef InterleavedSampleArray As Single())
                    Me.ChannelCount = ChannelCount
                    Me.FrameCount = FrameCount
                    Me.InterleavedSampleArray = InterleavedSampleArray
                End Sub

                Public Sub ConvertToChannelData(ByRef DuplexMixer As DuplexMixer)
                    Throw New NotImplementedException
                End Sub

            End Class


        End Class

    End Namespace

End Namespace
