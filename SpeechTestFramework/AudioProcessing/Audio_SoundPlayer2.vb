Imports SpeechTestFramework.Audio
Imports System.Runtime.InteropServices

Namespace Audio


    Namespace PortAudioVB

        Public Class SoundPlayer2
            Implements IDisposable

            'N.B. This sound player is just for debugging and trying out new stuff! Don't use it for regular tasks!

            Private _IsStreamOpen As Boolean
            Private _IsPlaying As Boolean
            Private _IsInitialized As Boolean


            Private SelectedApiInfo As PortAudio.PaHostApiInfo
            Private SelectedOutputDeviceInfo As PortAudio.PaDeviceInfo
            Private SelectedOutputDevice As Integer

            Private Stream As IntPtr
            Private PlaybackBuffer As Single()
            Private SilentBuffer As Single()

            Private StopAtOutputSoundEnd = False

            Private paStreamCallback As PortAudio.PaStreamCallbackDelegate = Function(input As IntPtr, output As IntPtr, frameCount As UInteger, ByRef timeInfo As PortAudio.PaStreamCallbackTimeInfo, statusFlags As PortAudio.PaStreamCallbackFlags, userData As IntPtr) As PortAudio.PaStreamCallbackResult

                                                                                 Try

                                                                                     'OUTPUT SOUND
                                                                                     If _IsPlaying = True Then

                                                                                         'Getting the pointers to the channel arrays
                                                                                         Dim arrayRes As IntPtr() = New IntPtr(Me.NumberOfOutputChannels - 1) {}
                                                                                         Marshal.Copy(output, arrayRes, 0, Me.NumberOfOutputChannels)

                                                                                         'Copying the silent buffer if the end of sound B is reached
                                                                                         If BufferPosition >= OutputBuffer(0).Count Then

                                                                                             For c = 1 To NumberOfOutputChannels
                                                                                                 Marshal.Copy(SilentBuffer, 0, arrayRes(c - 1), SilentBuffer.Length)
                                                                                             Next

                                                                                             If StopAtOutputSoundEnd = True Then
                                                                                                 'Returning the callback to port audio
                                                                                                 Return PortAudio.PaStreamCallbackResult.paComplete
                                                                                             End If

                                                                                             BufferPosition += 1
                                                                                             'Console.WriteLine("End: " & BufferPosition)

                                                                                         Else

                                                                                             For c = 1 To NumberOfOutputChannels
                                                                                                 PlaybackBuffer = OutputBuffer(c - 1)(BufferPosition)
                                                                                                 Marshal.Copy(PlaybackBuffer, 0, arrayRes(c - 1), PlaybackBuffer.Length)
                                                                                             Next

                                                                                             BufferPosition += 1
                                                                                             'Console.WriteLine(BufferPosition)

                                                                                         End If

                                                                                     End If

                                                                                     Return PortAudio.PaStreamCallbackResult.paContinue

                                                                                 Catch ex As Exception

                                                                                     'Console.WriteLine(ex.ToString)

                                                                                     SendInfoToAudioLog(BufferPosition & " " & ex.ToString, "SoundPlayerExceptions")

                                                                                     Return PortAudio.PaStreamCallbackResult.paComplete

                                                                                 End Try

                                                                             End Function


            Private NumberOfOutputChannels As Integer

            Private SampleRate As Double
            'As the OSTF library stores sound data as Single (i.e. float) arrays, "paFloat32" is the only "PaSampleFormat" that can be played in without conversion. Therefore the player requires a bitdepth of 32 and a IEEE encoding of the data.
            'Private Const Required_AudioEncoding As Formats.WaveFormat.WaveFormatEncodings = Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints
            'Public Const Required_BitDepth As Integer = 32
            Private Const Required_PaSampleFormat As PortAudio.PaSampleFormat = PortAudio.PaSampleFormat.paFloat32 Or PortAudio.PaSampleFormat.paNonInterleaved

            Private OutputBuffer As New List(Of List(Of Single()))
            Private BufferPosition As Integer = 0

            Public Shared Property LoggingEnabled As Boolean = False
            Public Shared Property LogToFileEnabled As Boolean = False

            Public AudioApiSettings As AudioApiSettings

            Public Sub New(ByRef OutputSound As Audio.Sound,
                                           ByRef AudioApiSettings As AudioApiSettings,
                       Optional ByVal LoggingEnabled As Boolean = False,
                       Optional ByVal LogToFileEnabled As Boolean = False)

                Try

                    Me.AudioApiSettings = AudioApiSettings

                    Me.AudioApiSettings.FramesPerBuffer = Me.AudioApiSettings.FramesPerBuffer

                    SampleRate = OutputSound.WaveFormat.SampleRate
                    NumberOfOutputChannels = OutputSound.WaveFormat.Channels

                    OutputBuffer.Clear()
                    For c = 1 To NumberOfOutputChannels
                        OutputBuffer.Add(New List(Of Single()))
                    Next

                    'Creating buffers
                    Dim BufferCount As Integer = Math.Floor(OutputSound.WaveData.LongestChannelSampleCount / Me.AudioApiSettings.FramesPerBuffer)
                    For i = 0 To BufferCount - 1

                        For c = 1 To NumberOfOutputChannels

                            Dim NewBufferArray(Me.AudioApiSettings.FramesPerBuffer - 1) As Single

                            Dim StartReadSample As Integer = i * Me.AudioApiSettings.FramesPerBuffer

                            Array.Copy(OutputSound.WaveData.SampleData(c), StartReadSample, NewBufferArray, 0, Me.AudioApiSettings.FramesPerBuffer)

                            OutputBuffer(c - 1).Add(NewBufferArray)

                        Next

                        'skipping the last non-full buffer for now

                    Next

                    BufferPosition = 0


                    'Overriding any value set in InitializationSuccess
                    _IsInitialized = False

                    SoundPlayer2.LoggingEnabled = LoggingEnabled
                    SoundPlayer2.LogToFileEnabled = LogToFileEnabled

                    Log("Initializing...")

                    'Checking if PortAudio has been initialized 
                    If OstfBase.PortAudioIsInitialized = False Then Throw New Exception("The PortAudio library has not been initialized. This should have been done by a call to the function OsftBase.InitializeOSTF.")

                    _IsInitialized = True

                    SetNonAsioSoundDevice()

                Catch e As Exception
                    Log(ErrorCheck("Terminate", PortAudio.Pa_Terminate(), True))
                    Log(e.ToString())
                End Try
            End Sub

            ''' <summary>
            ''' Selects the indicated API and sound device/devices.
            ''' </summary>
            ''' <returns>Returns True upon succes and Flase if the intended devices could not be set.</returns>
            Public Function SetNonAsioSoundDevice() As Boolean

                Dim ApiName As String = "MME"

                'Checking if PortAudio has been initialized 
                If OstfBase.PortAudioIsInitialized = False Then Throw New Exception("The PortAudio library has not been initialized. This should have been done by a call to the function OsftBase.InitializeOSTF.")

                'Setting driver type
                'Getting driver types
                Dim DriverTypeList As New List(Of PortAudio.PaHostApiInfo)
                Dim hostApiCount As Integer = PortAudio.Pa_GetHostApiCount()
                For i As Integer = 0 To hostApiCount - 1
                    Dim CurrentHostApiInfo As PortAudio.PaHostApiInfo = PortAudio.Pa_GetHostApiInfo(i)

                    Select Case CurrentHostApiInfo.type
                    'Adds only the most common types
                        Case PortAudio.PaHostApiTypeId.paDirectSound, PortAudio.PaHostApiTypeId.paMME, PortAudio.PaHostApiTypeId.paWASAPI
                            If CurrentHostApiInfo.name = ApiName Then
                                DriverTypeList.Add(CurrentHostApiInfo)
                                Exit For
                            End If
                    End Select
                Next

                'Selecting the only added driver or returns False if none are available
                If DriverTypeList.Count > 0 Then
                    SelectedApiInfo = DriverTypeList(0)
                Else
                    Return False
                End If

                Return True

                'Selecting devices
                'Output device
                Dim OutputDeviceName As String

                Dim deviceCount As Integer = PortAudio.Pa_GetDeviceCount()
                Dim outputDeviceList As New List(Of KeyValuePair(Of Integer, PortAudio.PaDeviceInfo))

                For i As Integer = 0 To deviceCount - 1
                    Dim paDeviceInfo As PortAudio.PaDeviceInfo = PortAudio.Pa_GetDeviceInfo(i)
                    Dim paHostApi As PortAudio.PaHostApiInfo = PortAudio.Pa_GetHostApiInfo(paDeviceInfo.hostApi)
                    If paHostApi.type = SelectedApiInfo.type Then
                        If paDeviceInfo.maxOutputChannels > 0 Then
                            If paDeviceInfo.name = OutputDeviceName Then
                                outputDeviceList.Add(New KeyValuePair(Of Integer, PortAudio.PaDeviceInfo)(i, paDeviceInfo))
                                Exit For
                            End If
                        End If
                    End If
                Next

                Select Case SelectedApiInfo.type
                    Case PortAudio.PaHostApiTypeId.paMME, PortAudio.PaHostApiTypeId.paDirectSound, PortAudio.PaHostApiTypeId.paWASAPI
                        If outputDeviceList.Count > 0 Then
                            SelectedOutputDeviceInfo = outputDeviceList(0).Value
                            SelectedOutputDevice = outputDeviceList(0).Key
                        Else
                            'Returns false if the intended output device could not be found
                            Return False
                        End If

                End Select

                Return True

            End Function


            Public Function OpenStream() As Boolean

                Log("Opening stream...")
                Me.Stream = StreamOpen()
                If IntPtr.Zero = False Then
                    Log("Stream pointer: " & Stream.ToString())
                    Return True
                Else
                    Return False
                End If

            End Function



            ''' <summary>
            ''' Starts the sound stream.
            ''' </summary>
            Public Sub Start()

                Log("Starting stream")
                If ErrorCheck("StartStream", PortAudio.Pa_StartStream(Stream), True) = False Then
                    _IsPlaying = True
                End If

            End Sub




            ''' <summary>
            ''' Stops the playback and/or recording stream
            ''' </summary>
            Public Sub StopStream()

                Log("Stopping stream...")

                If ErrorCheck("StopStream", PortAudio.Pa_StopStream(Stream), True) = False Then
                    _IsPlaying = False
                End If

            End Sub


            Public Sub AbortStream() 'Optional ByVal StoreInputSound As Boolean = True)

                Log("Aborting stream...")

                If ErrorCheck("AbortStream", PortAudio.Pa_AbortStream(Stream), True) = False Then
                    _IsPlaying = False
                End If

            End Sub



            Public Sub CloseStream()

                'Stopping the stream if it is running
                If PortAudio.Pa_IsStreamStopped(Me.Stream) < 1 Then
                    StopStream()
                End If

                'Cloing the stream
                If ErrorCheck("CloseStream", PortAudio.Pa_CloseStream(Stream), True) = False Then

                    _IsStreamOpen = False

                    'Resetting the stream
                    Me.Stream = New IntPtr(0)
                End If

            End Sub


            Private Function StreamOpen() As IntPtr



                SilentBuffer = New Single(Me.AudioApiSettings.FramesPerBuffer - 1) {}

                Dim stream As New IntPtr()
                Dim data As New IntPtr(0)

                Dim outputParams As New PortAudio.PaStreamParameters


                Dim OutputDeviceNumChanPtr As IntPtr
                Dim WmmeOutputStreamInfoPtr As IntPtr

                If Me.AudioApiSettings.UseMmeMultipleDevices = False Then

                Else

                    If AudioApiSettings.NumberOfWinMmeOutputDevices > 0 Then

                        outputParams.sampleFormat = Required_PaSampleFormat
                        outputParams.device = PortAudio.PaDeviceIndex.paUseHostApiSpecificDeviceSpecification

                        Dim wmmeOutputStreamInfo As New PortAudio.PaWinMmeStreamInfo
                        wmmeOutputStreamInfo.size = Marshal.SizeOf(wmmeOutputStreamInfo)
                        wmmeOutputStreamInfo.hostApiType = PortAudio.PaHostApiTypeId.paMME
                        wmmeOutputStreamInfo.version = 1
                        wmmeOutputStreamInfo.flags = PortAudio.PaWinMmeStreamInfoFlags.paWinMmeUseMultipleDevices

                        'Marshalling PaWinMmeDeviceAndChannelCount
                        OutputDeviceNumChanPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(GetType(Integer)) * AudioApiSettings.PaWinMmeOutputDeviceAndChannelCountArray.Length)
                        Marshal.Copy(AudioApiSettings.PaWinMmeOutputDeviceAndChannelCountArray, 0, OutputDeviceNumChanPtr, AudioApiSettings.PaWinMmeOutputDeviceAndChannelCountArray.Length)
                        wmmeOutputStreamInfo.devices = OutputDeviceNumChanPtr
                        wmmeOutputStreamInfo.deviceCount = AudioApiSettings.NumberOfWinMmeOutputDevices

                        'Marshalling wmmeOutputStreamInfoPtr
                        WmmeOutputStreamInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(wmmeOutputStreamInfo))
                        Marshal.StructureToPtr(wmmeOutputStreamInfo, WmmeOutputStreamInfoPtr, True)

                        outputParams.hostApiSpecificStreamInfo = WmmeOutputStreamInfoPtr
                        outputParams.channelCount = AudioApiSettings.NumberOfWinMmeOutputChannels
                        outputParams.suggestedLatency = AudioApiSettings.WinMmeSuggestedOutputLatency

                    End If

                End If



                Dim Flag As PortAudio.PaStreamFlags
                Dim IsClippingInactivated As Boolean = False
                If IsClippingInactivated = True Then
                    Flag = PortAudio.PaStreamFlags.paClipOff
                Else
                    Flag = PortAudio.PaStreamFlags.paNoFlag
                End If

                Log(outputParams.ToString)


                _IsStreamOpen = Not ErrorCheck("OpenOutputOnlyStream", PortAudio.Pa_OpenStream(stream, New Nullable(Of PortAudio.PaStreamParameters), outputParams,
                                                                   Me.SampleRate, Me.AudioApiSettings.FramesPerBuffer, Flag, Me.paStreamCallback, data), True)


                Dim StreamInfo = PortAudio.Pa_GetStreamInfo(stream)
                Console.WriteLine(StreamInfo.outputLatency)


                If Me.AudioApiSettings.UseMmeMultipleDevices = True Then
                    Marshal.FreeCoTaskMem(WmmeOutputStreamInfoPtr)
                    Marshal.FreeCoTaskMem(OutputDeviceNumChanPtr)
                End If

                Return stream

            End Function



            Private Sub Log(logString As String)
                If LoggingEnabled = True Then
                    System.Console.WriteLine("PortAudio: " & logString)
                End If
            End Sub


            Private Sub LogToFile(Message As String)
                If LogToFileEnabled = True Then
                    SendInfoToAudioLog(Message)
                End If
            End Sub

            Private Function ErrorCheck(action As String, errorCode As PortAudio.PaError, Optional ShowErrorInMsgBox As Boolean = False) As Boolean
                If errorCode <> PortAudio.PaError.paNoError Then
                    Dim MessageA As String = action & " error: " & PortAudio.Pa_GetErrorText(errorCode)

                    Log(MessageA)
                    LogToFile(MessageA)

                    If errorCode = PortAudio.PaError.paUnanticipatedHostError Then
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

                    End If

                    Return True
                Else
                    Log(action & " OK")
                    LogToFile(action & " OK")
                    Return False
                End If
            End Function


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
                    ErrorCheck("Terminate", PortAudio.Pa_Terminate(), True)

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

                'Freeing memory from the last callback
                'Marshal.FreeCoTaskMem(wmmeStreamInfoPtr)
                'Marshal.FreeCoTaskMem(DevNumChanBuffer)

                ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(True)
                ' TODO: uncomment the following line if Finalize() is overridden above.
                GC.SuppressFinalize(Me)
            End Sub

#End Region

        End Class

    End Namespace
End Namespace
