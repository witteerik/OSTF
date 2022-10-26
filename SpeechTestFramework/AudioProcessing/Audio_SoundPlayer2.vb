Imports SpeechTestFramework.Audio
Imports System.Runtime.InteropServices

Namespace Audio


    Namespace PortAudioVB

        Public Class SoundPlayer2
            Implements IDisposable

            Private _IsStreamOpen As Boolean
            Private _IsPlaying As Boolean
            Private _IsInitialized As Boolean


            Private SelectedApiInfo As PortAudio.PaHostApiInfo
            Private SelectedOutputDeviceInfo As PortAudio.PaDeviceInfo
            Private SelectedOutputDevice As Integer

            Private Stream As IntPtr
            Public FramesPerBuffer As UInteger
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
                                                                                             Console.WriteLine("End: " & BufferPosition)

                                                                                         Else

                                                                                             For c = 1 To NumberOfOutputChannels
                                                                                                 PlaybackBuffer = OutputBuffer(c - 1)(BufferPosition)
                                                                                                 Marshal.Copy(PlaybackBuffer, 0, arrayRes(c - 1), PlaybackBuffer.Length)
                                                                                             Next

                                                                                             BufferPosition += 1
                                                                                             Console.WriteLine(BufferPosition)

                                                                                         End If

                                                                                     End If

                                                                                     Return PortAudio.PaStreamCallbackResult.paContinue

                                                                                 Catch ex As Exception

                                                                                     Console.WriteLine(ex.ToString)

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

            Public Sub New(ByRef OutputSound As Audio.Sound,
                       ByRef FramesPerBuffer As Integer,
                       Optional ByVal LoggingEnabled As Boolean = False,
                       Optional ByVal LogToFileEnabled As Boolean = False)

                Try

                    Me.FramesPerBuffer = FramesPerBuffer

                    SampleRate = OutputSound.WaveFormat.SampleRate
                    NumberOfOutputChannels = OutputSound.WaveFormat.Channels

                    OutputBuffer.Clear()
                    For c = 1 To NumberOfOutputChannels
                        OutputBuffer.Add(New List(Of Single()))
                    Next

                    'Creating buffers
                    Dim BufferCount As Integer = Math.Floor(OutputSound.WaveData.LongestChannelSampleCount / FramesPerBuffer)
                    For i = 0 To BufferCount - 1

                        For c = 1 To NumberOfOutputChannels

                            Dim NewBufferArray(FramesPerBuffer - 1) As Single

                            Dim StartReadSample As Integer = i * FramesPerBuffer

                            Array.Copy(OutputSound.WaveData.SampleData(c), StartReadSample, NewBufferArray, 0, FramesPerBuffer)

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

                    'Initializing PA
                    If ErrorCheck("Initialize", PortAudio.Pa_Initialize(), True) = True Then
                        ' if Pa_Initialize() returns an error code, 
                        ' Pa_Terminate() should NOT be called.
                        Throw New Exception("Can't initialize audio")
                    End If

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

                'Initializing PA if not already done (using a call to the method Pa_GetDeviceCount to check if PA is initialized.)
                If PortAudio.Pa_GetDeviceCount = PortAudio.PaError.paNotInitialized Then
                    PortAudio.Pa_Initialize()
                End If

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

            Private wmmeStreamInfoPtr As IntPtr
            Private DevNumChanBuffer As IntPtr

            Private Function StreamOpen() As IntPtr

                SilentBuffer = New Single(Me.FramesPerBuffer - 1) {}

                Dim stream As New IntPtr()
                Dim data As New IntPtr(0)

                Dim outputParams As New PortAudio.PaStreamParameters
                outputParams.sampleFormat = Required_PaSampleFormat
                outputParams.device = PortAudio.PaDeviceIndex.paUseHostApiSpecificDeviceSpecification

                Dim wmmeStreamInfo As New PortAudio.PaWinMmeStreamInfo
                wmmeStreamInfo.size = Marshal.SizeOf(wmmeStreamInfo)
                wmmeStreamInfo.hostApiType = PortAudio.PaHostApiTypeId.paMME
                wmmeStreamInfo.version = 1
                wmmeStreamInfo.flags = PortAudio.PaWinMmeStreamInfoFlags.paWinMmeUseMultipleDevices

                Dim DevNumChanBuffer As IntPtr
                'Dim DeviceArray() As Integer = {3, 2, 5, 2}
                Dim DeviceArray() As Integer = {4, 2, 5, 2}
                Marshal.FreeCoTaskMem(DevNumChanBuffer)
                DevNumChanBuffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(GetType(Integer)) * DeviceArray.Length)
                Marshal.Copy(DeviceArray, 0, DevNumChanBuffer, DeviceArray.Length)
                wmmeStreamInfo.devices = DevNumChanBuffer
                wmmeStreamInfo.deviceCount = 2

                'Printing the selected devices
                Dim SelectedDeviceNames As New List(Of String)
                Dim deviceCount As Integer = PortAudio.Pa_GetDeviceCount()
                'Dim outputDeviceList As New List(Of KeyValuePair(Of Integer, PortAudio.PaDeviceInfo))
                For i As Integer = 0 To deviceCount - 1
                    Dim paDeviceInfo As PortAudio.PaDeviceInfo = PortAudio.Pa_GetDeviceInfo(i)
                    'Dim paHostApi As PortAudio.PaHostApiInfo = PortAudio.Pa_GetHostApiInfo(paDeviceInfo.hostApi)
                    If i = DeviceArray(0) Or i = DeviceArray(2) Then
                        SelectedDeviceNames.Add(paDeviceInfo.name & " (" & SelectedApiInfo.type.ToString() & ")")
                    End If
                Next
                Console.WriteLine("Selected devices: " & vbCrLf & String.Join(vbCrLf, SelectedDeviceNames))


                'Marshalling, move this code to Pa_OpenStream ?
                Marshal.FreeCoTaskMem(wmmeStreamInfoPtr)
                wmmeStreamInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(wmmeStreamInfo))
                Marshal.StructureToPtr(wmmeStreamInfo, wmmeStreamInfoPtr, True)
                outputParams.hostApiSpecificStreamInfo = wmmeStreamInfoPtr
                outputParams.channelCount = 4


                Dim paDeviceInfo3 As PortAudio.PaDeviceInfo = PortAudio.Pa_GetDeviceInfo(3)
                outputParams.suggestedLatency = paDeviceInfo3.defaultLowOutputLatency


                Log(outputParams.ToString)

                Dim Flag As PortAudio.PaStreamFlags = PortAudio.PaStreamFlags.paNoFlag
                'Dim IsClippingInactivated As Boolean = False
                'If IsClippingInactivated = True Then
                '    Flag = PortAudio.PaStreamFlags.paClipOff
                'Else
                '    Flag = PortAudio.PaStreamFlags.paNoFlag
                'End If

                _IsStreamOpen = ErrorCheck("OpenOutputOnlyStream", PortAudio.Pa_OpenStream(stream, New Nullable(Of PortAudio.PaStreamParameters), outputParams,
                                                                   Me.SampleRate, Me.FramesPerBuffer, Flag, Me.paStreamCallback, data), True)

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
                Marshal.FreeCoTaskMem(wmmeStreamInfoPtr)
                Marshal.FreeCoTaskMem(DevNumChanBuffer)

                ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(True)
                ' TODO: uncomment the following line if Finalize() is overridden above.
                GC.SuppressFinalize(Me)
            End Sub

#End Region

        End Class

    End Namespace
End Namespace
