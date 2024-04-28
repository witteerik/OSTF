'This file contains a portaudio wrapper for .NET. In order to use it, portaudio dll files named portaudio_x86.dll (for 32 bit) or portaudio_x64.dll (for 64 bit)
' should be placed in the directory of your assembly. The wrapper is written for PortAudio version 19.

'This software is available under the following license. Please note, the original contributors below.
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


'The PortAudioVB wrapper derives from PortAudioSharp, created by Riccardo Gerosa et al. 
'For references to the original contributors, see the text inserted below.
'
'  * PortAudioSharp - PortAudio bindings for .NET
'  * Copyright 2006-2011 Riccardo Gerosa and individual contributors as indicated
'  * by the @authors tag. See the copyright.txt in the distribution for a
'  * full listing of individual contributors.
'  *
'  * Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
'  * and associated documentation files (the "Software"), to deal in the Software without restriction, 
'  * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
'  * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
'  * subject to the following conditions:
'  *
'  * The above copyright notice and this permission notice shall be included in all copies or substantial 
'  * portions of the Software.
'  *
'  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
'  * NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
'  * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
'  * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
'  * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
' 
'   Full listing of Authors copied from the distribution file "copyright.txt"
'   @author 
'      Riccardo Gerosa(aka h3r3)
'      email: r.gerosa@gmail.com
'      site: http : //www.postronic.org/h3

'   @author
'      Moreno Carullo
'      email: moreno.carullo@pulc.it
'      site: http : //tweety.pulc.it/
'  

Option Explicit On
Option Infer Off
Option Strict On

Imports System.Runtime.InteropServices

Namespace Audio


    ''' <summary>
    ''' PortAudio v.19 bindings for .NET
    ''' </summary>
    Partial Public Class PortAudio
#Region "**** PORTAUDIO CALLBACKS ****"

        <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
        Public Delegate Function PaStreamCallbackDelegate(input As IntPtr, output As IntPtr, frameCount As UInteger, ByRef timeInfo As PaStreamCallbackTimeInfo, statusFlags As PaStreamCallbackFlags, userData As IntPtr) As PaStreamCallbackResult

        <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
        Public Delegate Sub PaStreamFinishedCallbackDelegate(userData As IntPtr)

#End Region

#Region "**** PORTAUDIO DATA STRUCTURES ****"

        <StructLayout(LayoutKind.Sequential)>
        Public Structure PaDeviceInfo

            Public structVersion As Integer
            <MarshalAs(UnmanagedType.LPStr)>
            Public name As String
            Public hostApi As Integer
            Public maxInputChannels As Integer
            Public maxOutputChannels As Integer
            Public defaultLowInputLatency As Double
            Public defaultLowOutputLatency As Double
            Public defaultHighInputLatency As Double
            Public defaultHighOutputLatency As Double
            Public defaultSampleRate As Double

            Public Overrides Function ToString() As String
                Return (Convert.ToString("[" & Me.[GetType]().Name & "]" & vbLf & "name: ") & name) & vbLf & "hostApi: " & hostApi & vbLf & "maxInputChannels: " & maxInputChannels & vbLf & "maxOutputChannels: " & maxOutputChannels & vbLf & "defaultLowInputLatency: " & defaultLowInputLatency & vbLf & "defaultLowOutputLatency: " & defaultLowOutputLatency & vbLf & "defaultHighInputLatency: " & defaultHighInputLatency & vbLf & "defaultHighOutputLatency: " & defaultHighOutputLatency & vbLf & "defaultSampleRate: " & defaultSampleRate
            End Function

            Public Function ToShorterString() As String
                Return (Convert.ToString("Name: ") & name) & vbLf & "Input channels: " & maxInputChannels & vbLf & "Output channels: " & maxOutputChannels
            End Function

            Public Function GetOutputDeviceName() As String
                Return name
            End Function

        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure PaHostApiInfo

            Public structVersion As Integer
            Public type As PaHostApiTypeId
            <MarshalAs(UnmanagedType.LPStr)>
            Public name As String
            Public deviceCount As Integer
            Public defaultInputDevice As Integer
            Public defaultOutputDevice As Integer

            Public Overrides Function ToString() As String
                Return (Convert.ToString("[" & Me.[GetType]().Name & "]" & vbLf & "structVersion: " & structVersion & vbLf & "type: " & type & vbLf & "name: ") & name) & vbLf & "deviceCount: " & deviceCount & vbLf & "defaultInputDevice: " & defaultInputDevice & vbLf & "defaultOutputDevice: " & defaultOutputDevice
            End Function

            Public Function ToShorterString() As String
                Return (Convert.ToString("Name: ") & name)
            End Function

        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure PaHostErrorInfo

            Public hostApiType As PaHostApiTypeId
            Public errorCode As Integer
            <MarshalAs(UnmanagedType.LPStr)>
            Public errorText As String

            Public Overrides Function ToString() As String
                Return Convert.ToString("[" & Me.[GetType]().Name & "]" & vbLf & "hostApiType: " & hostApiType & vbLf & "errorCode: " & errorCode & vbLf & "errorText: ") & errorText
            End Function

        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure PaStreamCallbackTimeInfo

            Public inputBufferAdcTime As Double
            Public currentTime As Double
            Public outputBufferDacTime As Double

            Public Overrides Function ToString() As String
                Return "[" & Me.[GetType]().Name & "]" & vbLf & "currentTime: " & currentTime & vbLf & "inputBufferAdcTime: " & inputBufferAdcTime & vbLf & "outputBufferDacTime: " & outputBufferDacTime
            End Function
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure PaStreamInfo

            Public structVersion As Integer
            Public inputLatency As Double
            Public outputLatency As Double
            Public sampleRate As Double

            Public Overrides Function ToString() As String
                Return "[" & Me.[GetType]().Name & "]" & vbLf & "structVersion: " & structVersion & vbLf & "inputLatency: " & inputLatency & vbLf & "outputLatency: " & outputLatency & vbLf & "sampleRate: " & sampleRate
            End Function
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure PaStreamParameters

            Public device As Integer
            Public channelCount As Integer
            Public sampleFormat As PaSampleFormat
            Public suggestedLatency As Double
            Public hostApiSpecificStreamInfo As IntPtr

            Public Overrides Function ToString() As String
                Return "[" & Me.[GetType]().Name & "]" & vbLf & "device: " & device & vbLf & "channelCount: " & channelCount & vbLf & "sampleFormat: " & sampleFormat & vbLf & "suggestedLatency: " & suggestedLatency
            End Function
        End Structure


#End Region

#Region "**** PORTAUDIO DEFINES ****"

        Public Enum PaDeviceIndex As Integer
            paNoDevice = -1
            paUseHostApiSpecificDeviceSpecification = -2
        End Enum

        Public Enum PaSampleFormat As UInteger
            paFloat32 = &H1UI
            paInt32 = &H2UI
            paInt24 = &H4UI
            paInt16 = &H8UI
            paInt8 = &H10UI
            paUInt8 = &H20UI
            paCustomFormat = &H10000UI
            paNonInterleaved = &H80000000UI

            'paNonInterleaved = &H80000000UI
        End Enum

        Public Const paFormatIsSupported As Integer = 0
        Public Const paFramesPerBufferUnspecified As Integer = 0

        Public Enum PaStreamFlags As UInteger
            paNoFlag = 0UI
            paClipOff = &H1UI
            paDitherOff = &H2UI
            paNeverDropInput = &H4UI
            paPrimeOutputBuffersUsingStreamCallback = &H8UI
            paPlatformSpecificFlags = &HFFFF0000UI
        End Enum

        Public Enum PaStreamCallbackFlags As UInteger
            paInputUnderflow = &H1
            paInputOverflow = &H2
            paOutputUnderflow = &H4
            paOutputOverflow = &H8
            paPrimingOutput = &H10
        End Enum

#End Region

#Region "**** PORTAUDIO ENUMERATIONS ****"

        Public Enum PaError As Integer
            paNoError = 0
            paNotInitialized = -10000
            paUnanticipatedHostError
            paInvalidChannelCount
            paInvalidSampleRate
            paInvalidDevice
            paInvalidFlag
            paSampleFormatNotSupported
            paBadIODeviceCombination
            paInsufficientMemory
            paBufferTooBig
            paBufferTooSmall
            paNullCallback
            paBadStreamPtr
            paTimedOut
            paInternalError
            paDeviceUnavailable
            paIncompatibleHostApiSpecificStreamInfo
            paStreamIsStopped
            paStreamIsNotStopped
            paInputOverflowed
            paOutputUnderflowed
            paHostApiNotFound
            paInvalidHostApi
            paCanNotReadFromACallbackStream
            paCanNotWriteToACallbackStream
            paCanNotReadFromAnOutputOnlyStream
            paCanNotWriteToAnInputOnlyStream
            paIncompatibleStreamHostApi
            paBadBufferPtr
        End Enum

        Public Enum PaHostApiTypeId As UInteger
            paInDevelopment = 0
            paDirectSound = 1
            paMME = 2
            paASIO = 3
            paSoundManager = 4
            paCoreAudio = 5
            paOSS = 7
            paALSA = 8
            paAL = 9
            paBeOS = 10
            paWDMKS = 11
            paJACK = 12
            paWASAPI = 13
            paAudioScienceHPI = 14
        End Enum

        Public Enum PaStreamCallbackResult As UInteger
            paContinue = 0
            paComplete = 1
            paAbort = 2
        End Enum

#End Region

#Region "**** MME SPECIFIC STUFF ****"


        Public Enum PaWinMmeStreamInfoFlags As UInteger
            ' The following are flags which can be set in
            ' PaWinMmeStreamInfo's flags field.
            '
            paWinMmeUseLowLevelLatencyParameters = &H1 ' (0x01)
            paWinMmeUseMultipleDevices = &H2 ' (0x02)  /* use mme specific multiple device feature */
            paWinMmeUseChannelMask = &H4 ' (0x04)

            'By default, the mme implementation drops the processing thread's priority
            'to THREAD_PRIORITY_NORMAL and sleeps the thread if the CPU load exceeds 100%
            'This flag disables any priority throttling. The processing thread will always
            'run at THREAD_PRIORITY_TIME_CRITICAL.

            paWinMmeDontThrottleOverloadedProcessingThread = &H8 ' (0x08)

            'Flags for non-PCM spdif passthrough.

            paWinMmeWaveFormatDolbyAc3Spdif = &H10 ' (0x10)
            paWinMmeWaveFormatWmaSpdif = &H20 ' (0x20)

        End Enum



        <StructLayout(LayoutKind.Sequential)>
        Public Structure PaWinMmeDeviceAndChannelCount
            '<FieldOffset(0)>
            Public device As PaDeviceIndex
            '<FieldOffset(4)>
            Public channelCount As Integer

            '        [FieldOffset(8)]
            '[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            'Public Byte[] itemname;

        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure PaWinMmeStreamInfo

            Public size As UInteger
            Public hostApiType As PaHostApiTypeId ' paMME
            Public version As UInteger ' 1
            Public flags As UInteger

            'low-level latency setting support
            'These settings control the number and size of host buffers in order
            'to set latency. They will be used instead of the generic parameters
            'to Pa_OpenStream() if flags contains the PaWinMmeUseLowLevelLatencyParameters
            'flag.

            'If PaWinMmeStreamInfo structures with PaWinMmeUseLowLevelLatencyParameters
            'are supplied for both input and output in a full duplex stream, then the
            'input and output framesPerBuffer must be the same, or the larger of the
            'two must be a multiple of the smaller, otherwise a
            'paIncompatibleHostApiSpecificStreamInfo error will be returned from
            'Pa_OpenStream().

            Public framesPerBuffer As UInteger
            Public bufferCount As UInteger

            'multiple devices per direction support
            'If flags contains the PaWinMmeUseMultipleDevices flag,
            'this functionality will be used, otherwise the device parameter to
            'Pa_OpenStream() will be used instead.
            'If devices are specified here, the corresponding device parameter
            'to Pa_OpenStream() should be set to paUseHostApiSpecificDeviceSpecification,
            'otherwise an paInvalidDevice error will result.
            'The total number of channels across all specified devices
            'must agree with the corresponding channelCount parameter to
            'Pa_OpenStream() otherwise a paInvalidChannelCount error will result.

            Public devices As IntPtr ' PaWinMmeDeviceAndChannelCount

            Public deviceCount As UInteger

            'support for WAVEFORMATEXTENSIBLE channel masks. If flags contains
            'paWinMmeUseChannelMask this allows you to specify which speakers
            'to address in a multichannel stream. Constants for channelMask
            'are specified in pa_win_waveformat.h

            Public channelMask As UInteger ' PaWinWaveFormatChannelMask


            'Public Overrides Function ToString() As String
            '    Return (Convert.ToString("[" & Me.[GetType]().Name & "]" & vbLf & "structVersion: " & structVersion & vbLf & "type: " & type & vbLf & "name: ") & name) & vbLf & "deviceCount: " & deviceCount & vbLf & "defaultInputDevice: " & defaultInputDevice & vbLf & "defaultOutputDevice: " & defaultOutputDevice
            'End Function

            'Public Function ToShorterString() As String
            '    Return (Convert.ToString("Name: ") & name)
            'End Function

        End Structure


#End Region


#Region "**** PORTAUDIO FUNCTIONS ****"
        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetVersion", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetVersion_32() As Integer
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetVersion", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetVersion_64() As Integer
        End Function
        Public Shared Function Pa_GetVersion() As Integer
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_GetVersion_32()
            Else
                Return Pa_GetVersion_64()
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetVersionText", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function IntPtr_Pa_GetVersionText_32() As IntPtr
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetVersionText", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function IntPtr_Pa_GetVersionText_64() As IntPtr
        End Function
        Public Shared Function Pa_GetVersionText() As String
            'Checking whether a 32-bit or 64-bit environment is running
            Dim strptr As IntPtr
            If IntPtr.Size = 4 Then
                strptr = IntPtr_Pa_GetVersionText_32()
            Else
                strptr = IntPtr_Pa_GetVersionText_64()
            End If
            Return Marshal.PtrToStringAnsi(strptr)
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetErrorText", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function IntPtr_Pa_GetErrorText_32(errorCode As PaError) As IntPtr
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetErrorText", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function IntPtr_Pa_GetErrorText_64(errorCode As PaError) As IntPtr
        End Function
        Public Shared Function Pa_GetErrorText(errorCode As PaError) As String
            Dim strptr As IntPtr
            If IntPtr.Size = 4 Then
                strptr = IntPtr_Pa_GetErrorText_32(errorCode)
            Else
                strptr = IntPtr_Pa_GetErrorText_64(errorCode)
            End If
            Return Marshal.PtrToStringAnsi(strptr)
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_Initialize", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_Initialize_32() As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_Initialize", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_Initialize_64() As PaError
        End Function
        Public Shared Function Pa_Initialize() As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_Initialize_32()
            Else
                Return Pa_Initialize_64()
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_Terminate", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_Terminate_32() As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_Terminate", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_Terminate_64() As PaError
        End Function
        Public Shared Function Pa_Terminate() As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_Terminate_32()
            Else
                Return Pa_Terminate_64()
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetHostApiCount", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetHostApiCount_32() As Integer
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetHostApiCount", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetHostApiCount_64() As Integer
        End Function
        Public Shared Function Pa_GetHostApiCount() As Integer
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_GetHostApiCount_32()
            Else
                Return Pa_GetHostApiCount_64()
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetDefaultHostApi", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetDefaultHostApi_32() As Integer
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetDefaultHostApi", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetDefaultHostApi_64() As Integer
        End Function
        Public Shared Function Pa_GetDefaultHostApi() As Integer
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_GetDefaultHostApi_32()
            Else
                Return Pa_GetDefaultHostApi_64()
            End If
        End Function


        <DllImport("PortAudio_x86.DLL", EntryPoint:="Pa_GetHostApiInfo", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function IntPtr_Pa_GetHostApiInfo_32(hostApi As Long) As IntPtr ' integer originally
        End Function
        <DllImport("PortAudio_x64.DLL", EntryPoint:="Pa_GetHostApiInfo", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function IntPtr_Pa_GetHostApiInfo_64(hostApi As Long) As IntPtr ' integer originally
        End Function
        Public Shared Function Pa_GetHostApiInfo(hostApi As Int64) As PaHostApiInfo
            Dim structptr As IntPtr
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                structptr = IntPtr_Pa_GetHostApiInfo_32(hostApi)
            Else
                structptr = IntPtr_Pa_GetHostApiInfo_64(hostApi)
            End If
            Return CType(Marshal.PtrToStructure(structptr, GetType(PaHostApiInfo)), PaHostApiInfo)
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_HostApiTypeIdToHostApiIndex", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_HostApiTypeIdToHostApiIndex_32(type As PaHostApiTypeId) As Integer
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_HostApiTypeIdToHostApiIndex", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_HostApiTypeIdToHostApiIndex_64(type As PaHostApiTypeId) As Integer
        End Function
        Public Shared Function Pa_HostApiTypeIdToHostApiIndex(type As PaHostApiTypeId) As Integer
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_HostApiTypeIdToHostApiIndex_32(type)
            Else
                Return Pa_HostApiTypeIdToHostApiIndex_64(type)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_HostApiDeviceIndexToDeviceIndex", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_HostApiDeviceIndexToDeviceIndex_32(hostApi As Integer, hostApiDeviceIndex As Integer) As Integer
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_HostApiDeviceIndexToDeviceIndex", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_HostApiDeviceIndexToDeviceIndex_64(hostApi As Integer, hostApiDeviceIndex As Integer) As Integer
        End Function
        Public Shared Function Pa_HostApiDeviceIndexToDeviceIndex(hostApi As Integer, hostApiDeviceIndex As Integer) As Integer
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_HostApiDeviceIndexToDeviceIndex_32(hostApi, hostApiDeviceIndex)
            Else
                Return Pa_HostApiDeviceIndexToDeviceIndex_64(hostApi, hostApiDeviceIndex)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetLastHostErrorInfo", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function IntPtr_Pa_GetLastHostErrorInfo_32() As IntPtr
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetLastHostErrorInfo", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function IntPtr_Pa_GetLastHostErrorInfo_64() As IntPtr
        End Function
        Public Shared Function Pa_GetLastHostErrorInfo() As PaHostErrorInfo
            Dim structptr As IntPtr
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                structptr = IntPtr_Pa_GetLastHostErrorInfo_32()
            Else
                structptr = IntPtr_Pa_GetLastHostErrorInfo_64()
            End If
            Return CType(Marshal.PtrToStructure(structptr, GetType(PaHostErrorInfo)), PaHostErrorInfo)
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetDeviceCount", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetDeviceCount_32() As Integer
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetDeviceCount", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetDeviceCount_64() As Integer
        End Function
        Public Shared Function Pa_GetDeviceCount() As Integer
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_GetDeviceCount_32()
            Else
                Return Pa_GetDeviceCount_64()
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetDefaultInputDevice", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetDefaultInputDevice_32() As Integer
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetDefaultInputDevice", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetDefaultInputDevice_64() As Integer
        End Function
        Public Shared Function Pa_GetDefaultInputDevice() As Integer
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_GetDefaultInputDevice_32()
            Else
                Return Pa_GetDefaultInputDevice_64()
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetDefaultOutputDevice", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetDefaultOutputDevice_32() As Integer
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetDefaultOutputDevice", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetDefaultOutputDevice_64() As Integer
        End Function
        Public Shared Function Pa_GetDefaultOutputDevice() As Integer
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_GetDefaultOutputDevice_32()
            Else
                Return Pa_GetDefaultOutputDevice_64()
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetDeviceInfo", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function IntPtr_Pa_GetDeviceInfo_32(device As Integer) As IntPtr
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetDeviceInfo", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function IntPtr_Pa_GetDeviceInfo_64(device As Integer) As IntPtr
        End Function
        Public Shared Function Pa_GetDeviceInfo(device As Integer) As PaDeviceInfo
            Dim structptr As IntPtr
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                structptr = IntPtr_Pa_GetDeviceInfo_32(device)
            Else
                structptr = IntPtr_Pa_GetDeviceInfo_64(device)
            End If
            Return CType(Marshal.PtrToStructure(structptr, GetType(PaDeviceInfo)), PaDeviceInfo)
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_IsFormatSupported", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_IsFormatSupported_32(ByRef inputParameters As PaStreamParameters, ByRef outputParameters As PaStreamParameters, sampleRate As Double) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_IsFormatSupported", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_IsFormatSupported_64(ByRef inputParameters As PaStreamParameters, ByRef outputParameters As PaStreamParameters, sampleRate As Double) As PaError
        End Function
        Public Shared Function Pa_IsFormatSupported(ByRef inputParameters As PaStreamParameters, ByRef outputParameters As PaStreamParameters, sampleRate As Double) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_IsFormatSupported_32(inputParameters, outputParameters, sampleRate)
            Else
                Return Pa_IsFormatSupported_64(inputParameters, outputParameters, sampleRate)
            End If
        End Function


        'This fix is modified from source: https://github.com/gp1313/portaudiosharp/commit/25fc512073391eda71b15af3cf4e8097a1a5e5b1 (R.Gerosa committed on Feb 26 2012 )

        ''' <summary>
        ''' Pa_OpenStream for duplex (input and output) stream.
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <param name="inputParameters"></param>
        ''' <param name="outputParameters"></param>
        ''' <param name="sampleRate"></param>
        ''' <param name="framesPerBuffer"></param>
        ''' <param name="streamFlags"></param>
        ''' <param name="streamCallback"></param>
        ''' <param name="userData"></param>
        ''' <returns></returns>
        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_OpenStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_OpenStream_32(ByRef stream As IntPtr, ByRef inputParameters As PaStreamParameters, ByRef outputParameters As PaStreamParameters, sampleRate As Double, framesPerBuffer As UInteger, streamFlags As PaStreamFlags,
        streamCallback As PaStreamCallbackDelegate, userData As IntPtr) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_OpenStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_OpenStream_64(ByRef stream As IntPtr, ByRef inputParameters As PaStreamParameters, ByRef outputParameters As PaStreamParameters, sampleRate As Double, framesPerBuffer As UInteger, streamFlags As PaStreamFlags,
        streamCallback As PaStreamCallbackDelegate, userData As IntPtr) As PaError
        End Function
        Public Shared Function Pa_OpenStream(ByRef stream As IntPtr, ByRef inputParameters As PaStreamParameters, ByRef outputParameters As PaStreamParameters, sampleRate As Double, framesPerBuffer As UInteger, streamFlags As PaStreamFlags,
        streamCallback As PaStreamCallbackDelegate, userData As IntPtr) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_OpenStream_32(stream, inputParameters, outputParameters, sampleRate, framesPerBuffer, streamFlags, streamCallback, userData)
            Else
                Return Pa_OpenStream_64(stream, inputParameters, outputParameters, sampleRate, framesPerBuffer, streamFlags, streamCallback, userData)
            End If
        End Function


        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_OpenStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_OpenStream_32(ByRef stream As IntPtr, inputParameters As IntPtr, outputParameters As IntPtr, sampleRate As Double, framesPerBuffer As UInteger, streamFlags As PaStreamFlags,
        streamCallback As PaStreamCallbackDelegate, userData As IntPtr) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_OpenStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_OpenStream_64(ByRef stream As IntPtr, inputParameters As IntPtr, outputParameters As IntPtr, sampleRate As Double, framesPerBuffer As UInteger, streamFlags As PaStreamFlags,
        streamCallback As PaStreamCallbackDelegate, userData As IntPtr) As PaError
        End Function

        'Public Shared Function Pa_OpenStream(ByRef stream As IntPtr, ByRef inputParameters As System.Nullable(Of PaStreamParameters), ByRef outputParameters As System.Nullable(Of PaStreamParameters), sampleRate As Double, framesPerBuffer As UInteger, streamFlags As PaStreamFlags,
        'streamCallback As PaStreamCallbackDelegate, userData As IntPtr) As PaError
        'Dim inputParametersPtr As IntPtr
        'If inputParameters IsNot Nothing Then
        'inputParametersPtr = Marshal.AllocHGlobal(Marshal.SizeOf(inputParameters.Value))
        'Marshal.StructureToPtr(inputParameters.Value, inputParametersPtr, True)
        'Else
        'inputParametersPtr = IntPtr.Zero
        'End If
        'Dim outputParametersPtr As IntPtr
        'If outputParameters IsNot Nothing Then
        'outputParametersPtr = Marshal.AllocHGlobal(Marshal.SizeOf(outputParameters.Value))
        'Marshal.StructureToPtr(outputParameters.Value, outputParametersPtr, True)
        'Else
        'outputParametersPtr = IntPtr.Zero
        'End If
        'Return Pa_OpenStream(stream, inputParametersPtr, outputParametersPtr, sampleRate, framesPerBuffer, streamFlags,
        'streamCallback, userData)
        'End Function

        ''' <summary>
        ''' Pa_OpenStream for playback only. Set inputParameters to Nothing.
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <param name="inputParameters"></param>
        ''' <param name="outputParameters"></param>
        ''' <param name="sampleRate"></param>
        ''' <param name="framesPerBuffer"></param>
        ''' <param name="streamFlags"></param>
        ''' <param name="streamCallback"></param>
        ''' <param name="userData"></param>
        ''' <returns></returns>
        Public Shared Function Pa_OpenStream(ByRef stream As IntPtr, ByRef inputParameters As System.Nullable(Of PaStreamParameters), ByRef outputParameters As PaStreamParameters, sampleRate As Double, framesPerBuffer As UInteger, streamFlags As PaStreamFlags,
        streamCallback As PaStreamCallbackDelegate, userData As IntPtr) As PaError
            Dim inputParametersPtr As IntPtr
            If inputParameters IsNot Nothing Then
                inputParametersPtr = Marshal.AllocHGlobal(Marshal.SizeOf(inputParameters.Value))
                Marshal.StructureToPtr(inputParameters.Value, inputParametersPtr, True)
            Else
                inputParametersPtr = IntPtr.Zero
            End If
            Dim outputParametersPtr As IntPtr

            outputParametersPtr = Marshal.AllocHGlobal(Marshal.SizeOf(outputParameters))
            Marshal.StructureToPtr(outputParameters, outputParametersPtr, True)

            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_OpenStream_32(stream, inputParametersPtr, outputParametersPtr, sampleRate, framesPerBuffer, streamFlags,
            streamCallback, userData)
            Else
                Return Pa_OpenStream_64(stream, inputParametersPtr, outputParametersPtr, sampleRate, framesPerBuffer, streamFlags,
            streamCallback, userData)
            End If

        End Function


        ''' <summary>
        ''' For recording only. Set outputParameters to Nothing.
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <param name="inputParameters"></param>
        ''' <param name="outputParameters"></param>
        ''' <param name="sampleRate"></param>
        ''' <param name="framesPerBuffer"></param>
        ''' <param name="streamFlags"></param>
        ''' <param name="streamCallback"></param>
        ''' <param name="userData"></param>
        ''' <returns></returns>
        Public Shared Function Pa_OpenStream(ByRef stream As IntPtr, ByRef inputParameters As PaStreamParameters, ByRef outputParameters As System.Nullable(Of PaStreamParameters), sampleRate As Double, framesPerBuffer As UInteger, streamFlags As PaStreamFlags,
        streamCallback As PaStreamCallbackDelegate, userData As IntPtr) As PaError

            Dim inputParametersPtr As IntPtr
            inputParametersPtr = Marshal.AllocHGlobal(Marshal.SizeOf(inputParameters))
            Marshal.StructureToPtr(inputParameters, inputParametersPtr, True)

            Dim outputParametersPtr As IntPtr
            If outputParameters IsNot Nothing Then
                outputParametersPtr = Marshal.AllocHGlobal(Marshal.SizeOf(outputParameters.Value))
                Marshal.StructureToPtr(outputParameters.Value, outputParametersPtr, True)
            Else
                outputParametersPtr = IntPtr.Zero
            End If

            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_OpenStream_32(stream, inputParametersPtr, outputParametersPtr, sampleRate, framesPerBuffer, streamFlags,
            streamCallback, userData)
            Else
                Return Pa_OpenStream_64(stream, inputParametersPtr, outputParametersPtr, sampleRate, framesPerBuffer, streamFlags,
            streamCallback, userData)
            End If

        End Function

        'Endtest



        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_OpenDefaultStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_OpenDefaultStream_32(ByRef stream As IntPtr, numInputChannels As Integer, numOutputChannels As Integer, sampleFormat As UInteger, sampleRate As Double, framesPerBuffer As UInteger,
        streamCallback As PaStreamCallbackDelegate, userData As IntPtr) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_OpenDefaultStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_OpenDefaultStream_64(ByRef stream As IntPtr, numInputChannels As Integer, numOutputChannels As Integer, sampleFormat As UInteger, sampleRate As Double, framesPerBuffer As UInteger,
        streamCallback As PaStreamCallbackDelegate, userData As IntPtr) As PaError
        End Function
        Public Shared Function Pa_OpenDefaultStream(ByRef stream As IntPtr, numInputChannels As Integer, numOutputChannels As Integer, sampleFormat As UInteger, sampleRate As Double, framesPerBuffer As UInteger,
        streamCallback As PaStreamCallbackDelegate, userData As IntPtr) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_OpenDefaultStream_32(stream, numInputChannels, numOutputChannels, sampleFormat, sampleRate, framesPerBuffer, streamCallback, userData)
            Else
                Return Pa_OpenDefaultStream_64(stream, numInputChannels, numOutputChannels, sampleFormat, sampleRate, framesPerBuffer, streamCallback, userData)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_CloseStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_CloseStream_32(stream As IntPtr) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_CloseStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_CloseStream_64(stream As IntPtr) As PaError
        End Function
        Public Shared Function Pa_CloseStream(stream As IntPtr) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_CloseStream_32(stream)
            Else
                Return Pa_CloseStream_64(stream)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_SetStreamFinishedCallback", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_SetStreamFinishedCallback_32(ByRef stream As IntPtr, <MarshalAs(UnmanagedType.FunctionPtr)> streamFinishedCallback As PaStreamFinishedCallbackDelegate) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_SetStreamFinishedCallback", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_SetStreamFinishedCallback_64(ByRef stream As IntPtr, <MarshalAs(UnmanagedType.FunctionPtr)> streamFinishedCallback As PaStreamFinishedCallbackDelegate) As PaError
        End Function
        Public Shared Function Pa_SetStreamFinishedCallback(ByRef stream As IntPtr, streamFinishedCallback As PaStreamFinishedCallbackDelegate) As PaError
            '<MarshalAs(UnmanagedType.FunctionPtr)> streamFinishedCallback As PaStreamFinishedCallbackDelegate is alterred in the second argument
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_SetStreamFinishedCallback_32(stream, streamFinishedCallback)
            Else
                Return Pa_SetStreamFinishedCallback_64(stream, streamFinishedCallback)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_StartStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_StartStream_32(stream As IntPtr) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_StartStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_StartStream_64(stream As IntPtr) As PaError
        End Function
        Public Shared Function Pa_StartStream(stream As IntPtr) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_StartStream_32(stream)
            Else
                Return Pa_StartStream_64(stream)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_StopStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_StopStream_32(stream As IntPtr) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_StopStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_StopStream_64(stream As IntPtr) As PaError
        End Function
        Public Shared Function Pa_StopStream(stream As IntPtr) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_StopStream_32(stream)
            Else
                Return Pa_StopStream_64(stream)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_AbortStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_AbortStream_32(stream As IntPtr) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_AbortStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_AbortStream_64(stream As IntPtr) As PaError
        End Function
        Public Shared Function Pa_AbortStream(stream As IntPtr) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_AbortStream_32(stream)
            Else
                Return Pa_AbortStream_64(stream)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_IsStreamStopped", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_IsStreamStopped_32(stream As IntPtr) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_IsStreamStopped", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_IsStreamStopped_64(stream As IntPtr) As PaError
        End Function
        Public Shared Function Pa_IsStreamStopped(stream As IntPtr) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_IsStreamStopped_32(stream)
            Else
                Return Pa_IsStreamStopped_64(stream)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_IsStreamActive", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_IsStreamActive_32(stream As IntPtr) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_IsStreamActive", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_IsStreamActive_64(stream As IntPtr) As PaError
        End Function
        Public Shared Function Pa_IsStreamActive(stream As IntPtr) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_IsStreamActive_32(stream)
            Else
                Return Pa_IsStreamActive_64(stream)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetStreamInfo", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function IntPtr_Pa_GetStreamInfo_32(stream As IntPtr) As IntPtr
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetStreamInfo", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function IntPtr_Pa_GetStreamInfo_64(stream As IntPtr) As IntPtr
        End Function
        Public Shared Function Pa_GetStreamInfo(stream As IntPtr) As PaStreamInfo
            Dim structptr As IntPtr
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                structptr = IntPtr_Pa_GetStreamInfo_32(stream)
            Else
                structptr = IntPtr_Pa_GetStreamInfo_64(stream)
            End If
            Return CType(Marshal.PtrToStructure(structptr, GetType(PaStreamInfo)), PaStreamInfo)
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetStreamTime", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetStreamTime_32(stream As IntPtr) As Double
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetStreamTime", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetStreamTime_64(stream As IntPtr) As Double
        End Function
        Public Shared Function Pa_GetStreamTime(stream As IntPtr) As Double
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_GetStreamTime_32(stream)
            Else
                Return Pa_GetStreamTime_64(stream)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetStreamCpuLoad", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetStreamCpuLoad_32(stream As IntPtr) As Double
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetStreamCpuLoad", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetStreamCpuLoad_64(stream As IntPtr) As Double
        End Function
        Public Shared Function Pa_GetStreamCpuLoad(stream As IntPtr) As Double
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_GetStreamCpuLoad_32(stream)
            Else
                Return Pa_GetStreamCpuLoad_64(stream)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_ReadStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_ReadStream_32(stream As IntPtr, <Out> buffer As Single(), frames As UInteger) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_ReadStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_ReadStream_64(stream As IntPtr, <Out> buffer As Single(), frames As UInteger) As PaError
        End Function
        Public Shared Function Pa_ReadStream(stream As IntPtr, <Out> buffer As Single(), frames As UInteger) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_ReadStream_32(stream, buffer, frames)
            Else
                Return Pa_ReadStream_64(stream, buffer, frames)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_ReadStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_ReadStream_32(stream As IntPtr, <Out> buffer As Byte(), frames As UInteger) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_ReadStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_ReadStream_64(stream As IntPtr, <Out> buffer As Byte(), frames As UInteger) As PaError
        End Function
        Public Shared Function Pa_ReadStream(stream As IntPtr, <Out> buffer As Byte(), frames As UInteger) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_ReadStream_32(stream, buffer, frames)
            Else
                Return Pa_ReadStream_64(stream, buffer, frames)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_ReadStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_ReadStream_32(stream As IntPtr, <Out> buffer As SByte(), frames As UInteger) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_ReadStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_ReadStream_64(stream As IntPtr, <Out> buffer As SByte(), frames As UInteger) As PaError
        End Function
        Public Shared Function Pa_ReadStream(stream As IntPtr, <Out> buffer As SByte(), frames As UInteger) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_ReadStream_32(stream, buffer, frames)
            Else
                Return Pa_ReadStream_64(stream, buffer, frames)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_ReadStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_ReadStream_32(stream As IntPtr, <Out> buffer As UShort(), frames As UInteger) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_ReadStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_ReadStream_64(stream As IntPtr, <Out> buffer As UShort(), frames As UInteger) As PaError
        End Function
        Public Shared Function Pa_ReadStream(stream As IntPtr, <Out> buffer As UShort(), frames As UInteger) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_ReadStream_32(stream, buffer, frames)
            Else
                Return Pa_ReadStream_64(stream, buffer, frames)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_ReadStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_ReadStream_32(stream As IntPtr, <Out> buffer As Short(), frames As UInteger) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_ReadStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_ReadStream_64(stream As IntPtr, <Out> buffer As Short(), frames As UInteger) As PaError
        End Function
        Public Shared Function Pa_ReadStream(stream As IntPtr, <Out> buffer As Short(), frames As UInteger) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_ReadStream_32(stream, buffer, frames)
            Else
                Return Pa_ReadStream_64(stream, buffer, frames)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_ReadStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_ReadStream_32(stream As IntPtr, <Out> buffer As UInteger(), frames As UInteger) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_ReadStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_ReadStream_64(stream As IntPtr, <Out> buffer As UInteger(), frames As UInteger) As PaError
        End Function
        Public Shared Function Pa_ReadStream(stream As IntPtr, <Out> buffer As UInteger(), frames As UInteger) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_ReadStream_32(stream, buffer, frames)
            Else
                Return Pa_ReadStream_64(stream, buffer, frames)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_ReadStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_ReadStream_32(stream As IntPtr, <Out> buffer As Integer(), frames As UInteger) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_ReadStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_ReadStream_64(stream As IntPtr, <Out> buffer As Integer(), frames As UInteger) As PaError
        End Function
        Public Shared Function Pa_ReadStream(stream As IntPtr, <Out> buffer As Integer(), frames As UInteger) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_ReadStream_32(stream, buffer, frames)
            Else
                Return Pa_ReadStream_64(stream, buffer, frames)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_WriteStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_WriteStream_32(stream As IntPtr, <[In]> buffer As Single(), frames As UInteger) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_WriteStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_WriteStream_64(stream As IntPtr, <[In]> buffer As Single(), frames As UInteger) As PaError
        End Function
        Public Shared Function Pa_WriteStream(stream As IntPtr, <[In]> buffer As Single(), frames As UInteger) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_WriteStream_32(stream, buffer, frames)
            Else
                Return Pa_WriteStream_64(stream, buffer, frames)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_WriteStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_WriteStream_32(stream As IntPtr, <[In]> buffer As Byte(), frames As UInteger) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_WriteStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_WriteStream_64(stream As IntPtr, <[In]> buffer As Byte(), frames As UInteger) As PaError
        End Function
        Public Shared Function Pa_WriteStream(stream As IntPtr, <[In]> buffer As Byte(), frames As UInteger) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_WriteStream_32(stream, buffer, frames)
            Else
                Return Pa_WriteStream_64(stream, buffer, frames)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_WriteStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_WriteStream_32(stream As IntPtr, <[In]> buffer As SByte(), frames As UInteger) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_WriteStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_WriteStream_64(stream As IntPtr, <[In]> buffer As SByte(), frames As UInteger) As PaError
        End Function
        Public Shared Function Pa_WriteStream(stream As IntPtr, <[In]> buffer As SByte(), frames As UInteger) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_WriteStream_32(stream, buffer, frames)
            Else
                Return Pa_WriteStream_64(stream, buffer, frames)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_WriteStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_WriteStream_32(stream As IntPtr, <[In]> buffer As UShort(), frames As UInteger) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_WriteStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_WriteStream_64(stream As IntPtr, <[In]> buffer As UShort(), frames As UInteger) As PaError
        End Function
        Public Shared Function Pa_WriteStream(stream As IntPtr, <[In]> buffer As UShort(), frames As UInteger) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_WriteStream_32(stream, buffer, frames)
            Else
                Return Pa_WriteStream_64(stream, buffer, frames)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_WriteStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_WriteStream_32(stream As IntPtr, <[In]> buffer As Short(), frames As UInteger) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_WriteStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_WriteStream_64(stream As IntPtr, <[In]> buffer As Short(), frames As UInteger) As PaError
        End Function
        Public Shared Function Pa_WriteStream(stream As IntPtr, <[In]> buffer As Short(), frames As UInteger) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_WriteStream_32(stream, buffer, frames)
            Else
                Return Pa_WriteStream_64(stream, buffer, frames)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_WriteStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_WriteStream_32(stream As IntPtr, <[In]> buffer As UInteger(), frames As UInteger) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_WriteStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_WriteStream_64(stream As IntPtr, <[In]> buffer As UInteger(), frames As UInteger) As PaError
        End Function
        Public Shared Function Pa_WriteStream(stream As IntPtr, <[In]> buffer As UInteger(), frames As UInteger) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_WriteStream_32(stream, buffer, frames)
            Else
                Return Pa_WriteStream_64(stream, buffer, frames)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_WriteStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_WriteStream_32(stream As IntPtr, <[In]> buffer As Integer(), frames As UInteger) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_WriteStream", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_WriteStream_64(stream As IntPtr, <[In]> buffer As Integer(), frames As UInteger) As PaError
        End Function
        Public Shared Function Pa_WriteStream(stream As IntPtr, <[In]> buffer As Integer(), frames As UInteger) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_WriteStream_32(stream, buffer, frames)
            Else
                Return Pa_WriteStream_64(stream, buffer, frames)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetStreamReadAvailable", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetStreamReadAvailable_32(stream As IntPtr) As Integer
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetStreamReadAvailable", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetStreamReadAvailable_64(stream As IntPtr) As Integer
        End Function
        Public Shared Function Pa_GetStreamReadAvailable(stream As IntPtr) As Integer
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_GetStreamReadAvailable_32(stream)
            Else
                Return Pa_GetStreamReadAvailable_64(stream)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetStreamWriteAvailable", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetStreamWriteAvailable_32(stream As IntPtr) As Integer
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetStreamWriteAvailable", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetStreamWriteAvailable_64(stream As IntPtr) As Integer
        End Function
        Public Shared Function Pa_GetStreamWriteAvailable(stream As IntPtr) As Integer
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_GetStreamWriteAvailable_32(stream)
            Else
                Return Pa_GetStreamWriteAvailable_64(stream)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_GetSampleSize", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetSampleSize_32(format As PaSampleFormat) As PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_GetSampleSize", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function Pa_GetSampleSize_64(format As PaSampleFormat) As PaError
        End Function
        Public Shared Function Pa_GetSampleSize(format As PaSampleFormat) As PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return Pa_GetSampleSize_32(format)
            Else
                Return Pa_GetSampleSize_64(format)
            End If
        End Function

        <DllImport("PortAudio_x86.dll", EntryPoint:="Pa_Sleep", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Sub Pa_Sleep_32(msec As Integer)
        End Sub
        <DllImport("PortAudio_x64.dll", EntryPoint:="Pa_Sleep", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Sub Pa_Sleep_64(msec As Integer)
        End Sub
        Public Shared Sub Pa_Sleep(msec As Integer)
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Pa_Sleep_32(msec)
            Else
                Pa_Sleep_64(msec)
            End If
        End Sub

#End Region

        ' This is a static class
        Private Sub New()
        End Sub

    End Class



    Partial Public Class PortAudio
#Region "**** PORTAUDIO CALLBACKS ****"

#End Region

#Region "**** PORTAUDIO DATA STRUCTURES ****"

        <StructLayout(LayoutKind.Sequential)>
        Public Structure PaAsioStreamInfo
            Public size As ULong
            '*< sizeof(PaAsioStreamInfo) 
            Public hostApiType As Integer
            '*< paASIO 
            Public version As ULong
            '*< 1 
            Public flags As ULong

            ''' Support for opening only specific channels of an ASIO device.
            ''' If the paAsioUseChannelSelectors flag is set, channelSelectors is a
            ''' pointer to an array of integers specifying the device channels to use.
            ''' When used, the length of the channelSelectors array must match the
            ''' corresponding channelCount parameter to Pa_OpenStream() otherwise a
            ''' crash may result.
            ''' The values in the selectors array must specify channels within the
            ''' range of supported channels for the device or paInvalidChannelCount will
            ''' result.
            Private channelSelectors As IntPtr
        End Structure

#End Region

#Region "**** PORTAUDIO DEFINES ****"

        Public Const paAsioUseChannelSelectors As Integer = &H1

#End Region

#Region "**** PORTAUDIO ENUMERATIONS ****"

#End Region

#Region "**** PORTAUDIO FUNCTIONS ****"

        '		/// <summary> Retrieve legal latency settings for the specificed device, in samples. </summary>
        '		/// <param name="device"> The global index of the device about which the query is being made. </param>
        '		/// <param name="minLatency"> A pointer to the location which will recieve the minimum latency value. </param>
        '		/// <param name="maxLatency"> A pointer to the location which will recieve the maximum latency value. </param>
        '		/// <param name="preferredLatency"> A pointer to the location which will recieve the preferred latency value. </param>
        '		/// <param name="granularity"> A pointer to the location which will recieve the granularity. This value 
        '		/// 	determines which values between minLatency and maxLatency are available. ie the step size,
        '		/// 	if granularity is -1 then available latency settings are powers of two. </param>
        '		/// See ASIOGetBufferSize in the ASIO SDK.
        '		PaError PaAsio_GetAvailableLatencyValues( PaDeviceIndex device, long *minLatency, long *maxLatency, 
        '			long *preferredLatency, long *granularity );

        <DllImport("PortAudio_x86.dll", EntryPoint:="PaAsio_ShowControlPanel", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function PaAsio_ShowControlPanel_32(device As Integer, systemSpecific As IntPtr) As PortAudio.PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="PaAsio_ShowControlPanel", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function PaAsio_ShowControlPanel_64(device As Integer, systemSpecific As IntPtr) As PortAudio.PaError
        End Function
        ''' <summary> Display the ASIO control panel for the specified device. </summary>
        ''' <param name="device"> The global index of the device whose control panel is to be displayed. </param>
        ''' <param name="systemSpecific">On Windows, the calling application's main window handle, 
        ''' 	on Macintosh this value should be zero.</param>
        Public Shared Function PaAsio_ShowControlPanel(device As Integer, systemSpecific As IntPtr) As PortAudio.PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return PaAsio_ShowControlPanel_32(device, systemSpecific)
            Else
                Return PaAsio_ShowControlPanel_64(device, systemSpecific)
            End If
        End Function

        '	 	/// <summary> Retrieve a pointer to a string containing the name of the specified input channel. </summary>
        '	 	/// The string is valid until Pa_Terminate is called.
        '	 	/// The string will be no longer than 32 characters including the null terminator.
        '		PaError PaAsio_GetInputChannelName(PaDeviceIndex device, int channelIndex, const char** channelName );

        '		/// <summary> Retrieve a pointer to a string containing the name of the specified input channel. </summary>
        '		/// The string is valid until Pa_Terminate is called. 
        '		/// The string will be no longer than 32 characters including the null terminator.
        '		PaError PaAsio_GetOutputChannelName( PaDeviceIndex device, int channelIndex, const char** channelName );

        <DllImport("PortAudio_x86.dll", EntryPoint:="PaAsio_SetStreamSampleRate", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function PaAsio_SetStreamSampleRate_32(stream As IntPtr, sampleRate As Double) As PortAudio.PaError
        End Function
        <DllImport("PortAudio_x64.dll", EntryPoint:="PaAsio_SetStreamSampleRate", CallingConvention:=CallingConvention.Cdecl)>
        Private Shared Function PaAsio_SetStreamSampleRate_64(stream As IntPtr, sampleRate As Double) As PortAudio.PaError
        End Function
        ''' <summary> Set the sample rate of an open paASIO stream. </summary>
        ''' <param name="stream"></param> The stream to operate on.
        ''' <param name="sampleRate"></param> The new sample rate. 
        ''' Note that this function may fail if the stream is alredy running and the 
        ''' ASIO driver does not support switching the sample rate of a running stream.
        ''' <returns> paIncompatibleStreamHostApi if stream is not a paASIO stream. </returns>
        Public Shared Function PaAsio_SetStreamSampleRate(stream As IntPtr, sampleRate As Double) As PortAudio.PaError
            'Checking whether a 32-bit or 64-bit environment is running
            If IntPtr.Size = 4 Then
                Return PaAsio_SetStreamSampleRate_32(stream, sampleRate)
            Else
                Return PaAsio_SetStreamSampleRate_64(stream, sampleRate)
            End If
        End Function

#End Region

    End Class


End Namespace
