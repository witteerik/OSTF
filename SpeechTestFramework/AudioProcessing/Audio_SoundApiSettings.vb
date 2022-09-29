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

    Public Class AudioApiSettings

        'Public HostApi As Integer 'Probably not needed?
        Public SelectedApiInfo As PortAudio.PaHostApiInfo
        Public SelectedInputDeviceInfo As PortAudio.PaDeviceInfo?
        Public SelectedInputDevice As Integer?
        Public SelectedOutputDeviceInfo As PortAudio.PaDeviceInfo?
        Public SelectedOutputDevice As Integer?
        Public SelectedInputAndOutputDeviceInfo As PortAudio.PaDeviceInfo?
        'Public SelectedInputAndOutputDevice As Integer?

        Public FramesPerBuffer As UInteger

        Public Overrides Function ToString() As String
            Dim OutputString As String = "Selected sound API settings:"

            OutputString &= "Selected HostAPI:" & vbLf & SelectedApiInfo.ToString() & vbCrLf
            If Not SelectedInputDeviceInfo Is Nothing Then OutputString &= "Selected input device:" & vbLf & SelectedInputDeviceInfo.ToString() & vbCrLf
            If Not SelectedOutputDeviceInfo Is Nothing Then OutputString &= "Selected output device:" & vbLf & SelectedOutputDeviceInfo.ToString() & vbCrLf
            If Not SelectedInputAndOutputDeviceInfo Is Nothing Then OutputString &= "Selected input and output device:" & vbLf & SelectedInputAndOutputDeviceInfo.ToString() & vbCrLf
            OutputString &= "FramesPerBuffer: " & FramesPerBuffer

            Return OutputString
        End Function

        Public Function ToShorterString() As String
            Dim OutputString As String = "Selected sound API settings:" & vbCrLf

            OutputString &= SelectedApiInfo.ToShorterString & vbCrLf & vbCrLf
            If Not SelectedInputDeviceInfo Is Nothing Then OutputString &= "Selected input device:" & vbLf & SelectedInputDeviceInfo.Value.ToShorterString() & vbCrLf & vbCrLf
            If Not SelectedOutputDeviceInfo Is Nothing Then OutputString &= "Selected output device:" & vbLf & SelectedOutputDeviceInfo.Value.ToShorterString() & vbCrLf & vbCrLf
            If Not SelectedInputAndOutputDeviceInfo Is Nothing Then OutputString &= "Selected input and output device:" & vbLf & SelectedInputAndOutputDeviceInfo.Value.ToShorterString() & vbCrLf & vbCrLf
            OutputString &= "Frames per buffer: " & FramesPerBuffer

            Return OutputString
        End Function


        'Returns the number of input channels on the selected device, or Nothing if no input device has been selected
        Public Function NumberOfInputChannels() As Integer?

            If SelectedInputDeviceInfo.HasValue = True Then Return SelectedInputDeviceInfo.Value.maxInputChannels
            If SelectedInputAndOutputDeviceInfo.HasValue = True Then Return SelectedInputAndOutputDeviceInfo.Value.maxInputChannels

        End Function

        'Returns the number of output channels on the selected device, or Nothing if no output device has been selected
        Public Function NumberOfOutputChannels() As Integer?

            If SelectedOutputDeviceInfo.HasValue = True Then Return SelectedOutputDeviceInfo.Value.maxOutputChannels
            If SelectedInputAndOutputDeviceInfo.HasValue = True Then Return SelectedInputAndOutputDeviceInfo.Value.maxOutputChannels

        End Function

        Public Function SetAsioSoundDevice(ByVal DeviceName As String, Optional ByVal BufferSize As Integer = 256) As Boolean

            'Initializing PA if not already done (using a call to the method Pa_GetDeviceCount to check if PA is initialized.)
            If PortAudio.Pa_GetDeviceCount = PortAudio.PaError.paNotInitialized Then
                PortAudio.Pa_Initialize()
            End If

            'Selects the ASIO host type
            Dim hostApiCount As Integer = PortAudio.Pa_GetHostApiCount()
            For i As Integer = 0 To hostApiCount - 1
                Dim hostApiInfo As PortAudio.PaHostApiInfo = PortAudio.Pa_GetHostApiInfo(i)
                If hostApiInfo.type = PortAudio.PaHostApiTypeId.paASIO Then
                    SelectedApiInfo = hostApiInfo
                    Exit For
                End If
            Next

            Dim deviceCount As Integer = PortAudio.Pa_GetDeviceCount()
            Dim inputDeviceList As New List(Of KeyValuePair(Of Integer, PortAudio.PaDeviceInfo))

            For i As Integer = 0 To deviceCount - 1
                Dim paDeviceInfo As PortAudio.PaDeviceInfo = PortAudio.Pa_GetDeviceInfo(i)
                Dim paHostApi As PortAudio.PaHostApiInfo = PortAudio.Pa_GetHostApiInfo(paDeviceInfo.hostApi)
                If paHostApi.type = SelectedApiInfo.type Then
                    inputDeviceList.Add(New KeyValuePair(Of Integer, PortAudio.PaDeviceInfo)(i, paDeviceInfo))
                End If
            Next

            'Selects the device with the indicated name
            For Each CurrentItem In inputDeviceList

                If CurrentItem.Value.name = DeviceName Then

                    SelectedInputAndOutputDeviceInfo = CurrentItem.Value

                    SelectedInputDevice = CurrentItem.Key
                    SelectedOutputDevice = CurrentItem.Key

                    SelectedInputDeviceInfo = Nothing
                    SelectedOutputDeviceInfo = Nothing
                    Exit For
                End If
            Next

            'Setting buffer size
            SetBufferSize(BufferSize)

            If SelectedInputAndOutputDeviceInfo IsNot Nothing Then
                Return True
            Else
                Return False
            End If

        End Function


        ''' <summary>
        ''' Selects the indicated API and sound device/devices.
        ''' </summary>
        ''' <returns>Returns True upon succes and Flase if the intended devices could not be set.</returns>
        Public Function SetNonAsioSoundDevice(ByVal ApiName As String, ByVal OutputDeviceName As String, Optional ByVal InputDeviceName As String = "", Optional ByVal BufferSize As Integer = 256) As Boolean

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

            'Selecting devices
            'Output device
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
                        SelectedInputAndOutputDeviceInfo = Nothing
                    Else
                        'Returns false if the intended output device could not be found
                        Return False
                    End If

            End Select

            'Input device
            If InputDeviceName = "" Then

                ' No input device should be selected

                SelectedInputDeviceInfo = Nothing
                SelectedInputDevice = Nothing
                SelectedInputAndOutputDeviceInfo = Nothing

            Else

                Dim InputDeviceList As New List(Of KeyValuePair(Of Integer, PortAudio.PaDeviceInfo))

                For i As Integer = 0 To deviceCount - 1
                    Dim paDeviceInfo As PortAudio.PaDeviceInfo = PortAudio.Pa_GetDeviceInfo(i)
                    Dim paHostApi As PortAudio.PaHostApiInfo = PortAudio.Pa_GetHostApiInfo(paDeviceInfo.hostApi)
                    If paHostApi.type = SelectedApiInfo.type Then
                        If paDeviceInfo.maxInputChannels > 0 Then
                            If paDeviceInfo.name = InputDeviceName Then
                                InputDeviceList.Add(New KeyValuePair(Of Integer, PortAudio.PaDeviceInfo)(i, paDeviceInfo))
                                Exit For
                            End If
                        End If
                    End If
                Next

                Select Case SelectedApiInfo.type
                    Case PortAudio.PaHostApiTypeId.paMME, PortAudio.PaHostApiTypeId.paDirectSound, PortAudio.PaHostApiTypeId.paWASAPI

                        If InputDeviceList.Count > 0 Then
                            SelectedInputDeviceInfo = InputDeviceList(0).Value
                            SelectedInputDevice = InputDeviceList(0).Key
                            SelectedInputAndOutputDeviceInfo = Nothing
                        Else
                            'Returns false if the intended input device could not be found
                            Return False
                        End If
                End Select
            End If

            'Setting buffer size
            SetBufferSize(BufferSize)

            Return True

        End Function


        ''' <summary>
        ''' Selects the first available (non asio) audio device for use.
        ''' </summary>
        ''' <returns>Returns True if at least an input or an output device was set. Returns false if neither an input or an output device could be set.</returns>
        Public Function SelectFirstAvailableDevice(Optional ByVal BufferSize As Integer = 256) As Boolean

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
                        DriverTypeList.Add(CurrentHostApiInfo)
                End Select
            Next

            'Selecting the first available one or returns False if none are available
            If DriverTypeList.Count > 0 Then
                SelectedApiInfo = DriverTypeList(0)
            Else
                Return False
            End If


            'Selecting devices
            'Output device
            Dim deviceCount As Integer = PortAudio.Pa_GetDeviceCount()
            Dim outputDeviceList As New List(Of KeyValuePair(Of Integer, PortAudio.PaDeviceInfo))

            For i As Integer = 0 To deviceCount - 1
                Dim paDeviceInfo As PortAudio.PaDeviceInfo = PortAudio.Pa_GetDeviceInfo(i)
                Dim paHostApi As PortAudio.PaHostApiInfo = PortAudio.Pa_GetHostApiInfo(paDeviceInfo.hostApi)
                If paHostApi.type = SelectedApiInfo.type Then
                    If paDeviceInfo.maxOutputChannels > 0 Then
                        outputDeviceList.Add(New KeyValuePair(Of Integer, PortAudio.PaDeviceInfo)(i, paDeviceInfo))
                    End If
                End If
            Next

            Dim OutputDeviceIsSet As Boolean = False
            Select Case SelectedApiInfo.type
                Case PortAudio.PaHostApiTypeId.paMME, PortAudio.PaHostApiTypeId.paDirectSound, PortAudio.PaHostApiTypeId.paWASAPI

                    If outputDeviceList.Count > 0 Then
                        SelectedOutputDeviceInfo = outputDeviceList(0).Value
                        SelectedOutputDevice = outputDeviceList(0).Key
                        SelectedInputAndOutputDeviceInfo = Nothing
                        OutputDeviceIsSet = True
                    Else
                        MsgBox("No output sound device is available!")
                    End If

            End Select

            'Input device
            Dim InputDeviceList As New List(Of KeyValuePair(Of Integer, PortAudio.PaDeviceInfo))

            For i As Integer = 0 To deviceCount - 1
                Dim paDeviceInfo As PortAudio.PaDeviceInfo = PortAudio.Pa_GetDeviceInfo(i)
                Dim paHostApi As PortAudio.PaHostApiInfo = PortAudio.Pa_GetHostApiInfo(paDeviceInfo.hostApi)
                If paHostApi.type = SelectedApiInfo.type Then
                    If paDeviceInfo.maxInputChannels > 0 Then
                        InputDeviceList.Add(New KeyValuePair(Of Integer, PortAudio.PaDeviceInfo)(i, paDeviceInfo))
                    End If
                End If
            Next

            Dim InputDeviceIsSet As Boolean = False
            Select Case SelectedApiInfo.type
                Case PortAudio.PaHostApiTypeId.paMME, PortAudio.PaHostApiTypeId.paDirectSound, PortAudio.PaHostApiTypeId.paWASAPI

                    If InputDeviceList.Count > 0 Then
                        SelectedInputDeviceInfo = InputDeviceList(0).Value
                        SelectedInputDevice = InputDeviceList(0).Key
                        SelectedInputAndOutputDeviceInfo = Nothing
                        InputDeviceIsSet = True
                    Else
                        MsgBox("No input sound device is available!")
                    End If

            End Select

            'Checking that we have at least one input or one output device
            If InputDeviceIsSet = False And OutputDeviceIsSet = False Then Return False

            'Setting buffer size
            SetBufferSize(BufferSize)

            Return True

        End Function



        ''' <summary>
        ''' Selects the default (non asio) audio device for use. (Only MME, DirectSound and WASAPI is allowed, all others are blocked.)
        ''' </summary>
        ''' <returns>Returns True if at least an input or an output device was set. Returns false if neither an input or an output device could be set.</returns>
        Public Function SelectDefaultAudioDevice(Optional ByVal SampleRate As Integer = 48000,
                                            Optional ByVal BufferSize As Integer = 256) As Boolean

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
                DriverTypeList.Add(CurrentHostApiInfo)
            Next

            'Selecting the default host API or returns False it's not available
            Dim DefaultHostApiIndex As Integer = PortAudio.Pa_GetDefaultHostApi()
            If DriverTypeList.Count > 0 Then
                SelectedApiInfo = DriverTypeList(DefaultHostApiIndex)
            Else
                Return False
            End If

            'Overriding the choice if its not a common API type and returns false.
            Select Case SelectedApiInfo.type
                Case PortAudio.PaHostApiTypeId.paDirectSound, PortAudio.PaHostApiTypeId.paMME, PortAudio.PaHostApiTypeId.paWASAPI
                    'OK
                Case Else
                    SelectedApiInfo = Nothing
                    SelectedInputDeviceInfo = Nothing
                    SelectedInputDevice = Nothing
                    SelectedOutputDeviceInfo = Nothing
                    SelectedOutputDevice = Nothing
                    SelectedInputAndOutputDeviceInfo = Nothing
                    MsgBox("Sound API host of type " & SelectedApiInfo.type.ToString & " is not supported.")
                    Return False
            End Select


            'Selecting devices
            Dim DeviceCount As Integer = PortAudio.Pa_GetDeviceCount()
            Dim DeviceList As New List(Of KeyValuePair(Of Integer, PortAudio.PaDeviceInfo))

            For i As Integer = 0 To DeviceCount - 1
                Dim paDeviceInfo As PortAudio.PaDeviceInfo = PortAudio.Pa_GetDeviceInfo(i)
                Dim paHostApi As PortAudio.PaHostApiInfo = PortAudio.Pa_GetHostApiInfo(paDeviceInfo.hostApi)
                DeviceList.Add(New KeyValuePair(Of Integer, PortAudio.PaDeviceInfo)(i, paDeviceInfo))
            Next

            'Getting the default devices
            Dim DefaultOutputDevice As Integer
            Dim OutputDeviceIsSet As Boolean = False
            Try
                DefaultOutputDevice = PortAudio.Pa_GetDefaultOutputDevice
                OutputDeviceIsSet = True
            Catch ex As Exception
                MsgBox("Cannot read the default output sound device. Do you have one?")
            End Try

            Dim DefaultInputDevice As Integer
            Dim InputDeviceIsSet As Boolean = False
            Try
                DefaultInputDevice = PortAudio.Pa_GetDefaultInputDevice
                InputDeviceIsSet = True
            Catch ex As Exception
                MsgBox("Cannot read the default input sound device. Do you have one?")
            End Try

            If DeviceList.Count > 0 Then
                'Setting output device
                SelectedOutputDeviceInfo = DeviceList(DefaultOutputDevice).Value
                SelectedOutputDevice = DeviceList(DefaultOutputDevice).Key
                SelectedInputAndOutputDeviceInfo = Nothing

                'Setting input device
                SelectedInputDeviceInfo = DeviceList(DefaultInputDevice).Value
                SelectedInputDevice = DeviceList(DefaultInputDevice).Key
                SelectedInputAndOutputDeviceInfo = Nothing

            Else
                Return False
            End If

            'Setting buffer size
            SetBufferSize(BufferSize)

            Return True

        End Function

        Private Sub SetBufferSize(ByVal BufferSize As Integer)

            'Setting buffer size
            Dim ValidBufferSizes As New List(Of Integer)
            Dim NewBufferSize As Integer = 128
            While NewBufferSize < 100000
                ValidBufferSizes.Add(NewBufferSize)
                NewBufferSize *= 2
            End While

            If ValidBufferSizes.Contains(BufferSize) Then
                FramesPerBuffer = BufferSize
            Else
                'If the user supplied an invalid bufer size, the buffer size is rounded up to the next valid value. 
                For n = 0 To ValidBufferSizes.Count - 1
                    If n = 0 Then
                        If BufferSize < ValidBufferSizes(n) Then FramesPerBuffer = ValidBufferSizes(n)
                    End If

                    If n > 0 And n < ValidBufferSizes.Count - 1 Then
                        If BufferSize > ValidBufferSizes(n) And BufferSize < ValidBufferSizes(n + 1) Then FramesPerBuffer = ValidBufferSizes(n + 1)
                    End If

                    If n = ValidBufferSizes.Count - 1 Then
                        If BufferSize > ValidBufferSizes(n) Then FramesPerBuffer = ValidBufferSizes(n)
                    End If
                Next

            End If

        End Sub

        Public Shared Function GetAllAvailableDevices() As String

            Try

                Dim OutputList As New List(Of String)

                Dim DefaultHostApiName As String = ""
                Dim DefaultHostApiNumber As Integer = Audio.PortAudio.Pa_GetDefaultHostApi()
                Dim HostApiCount As Integer = Audio.PortAudio.Pa_GetHostApiCount()
                For HostIndex As Integer = 0 To HostApiCount - 1
                    Dim HostApiInfo As Audio.PortAudio.PaHostApiInfo = Audio.PortAudio.Pa_GetHostApiInfo(HostIndex)
                    If HostIndex = DefaultHostApiNumber Then DefaultHostApiName = HostApiInfo.name
                Next

                Dim SupportedDeviceCount As Integer = 0
                Dim DeviceCount As Integer = Audio.PortAudio.Pa_GetDeviceCount()
                OutputList.Add("Total audio device count: " & DeviceCount)
                OutputList.Add("Total Host API count: " & HostApiCount & vbCrLf)

                For DeviceIndex As Integer = 0 To DeviceCount - 1

                    Dim PaDeviceInfo As Audio.PortAudio.PaDeviceInfo = Audio.PortAudio.Pa_GetDeviceInfo(DeviceIndex)
                    Dim HostApiInfo As Audio.PortAudio.PaHostApiInfo = Audio.PortAudio.Pa_GetHostApiInfo(PaDeviceInfo.hostApi)

                    'Only adding supported host API types
                    Select Case HostApiInfo.type
                        Case Audio.PortAudio.PaHostApiTypeId.paMME, Audio.PortAudio.PaHostApiTypeId.paDirectSound, Audio.PortAudio.PaHostApiTypeId.paWASAPI, Audio.PortAudio.PaHostApiTypeId.paWDMKS, Audio.PortAudio.PaHostApiTypeId.paASIO

                            SupportedDeviceCount += 1

                            OutputList.Add("Device name: " & PaDeviceInfo.name)
                            OutputList.Add("Device number: " & DeviceIndex)

                            OutputList.Add("Host API name: " & HostApiInfo.name)
                            If HostApiInfo.name = DefaultHostApiName Then
                                OutputList.Add("   (Default host API)")
                            Else
                                OutputList.Add("   (Not default host API)")
                            End If

                            OutputList.Add("Input channels: " & PaDeviceInfo.maxInputChannels)
                            OutputList.Add("Output channels: " & PaDeviceInfo.maxOutputChannels)

                            If DeviceIndex = HostApiInfo.defaultInputDevice Then
                                OutputList.Add("   (Default input device)")
                            Else
                                OutputList.Add("   (Not default input device)")
                            End If

                            If DeviceIndex = HostApiInfo.defaultOutputDevice Then
                                OutputList.Add("   (Default output device)" & vbCrLf)
                            Else
                                OutputList.Add("   (Not default output device)" & vbCrLf)
                            End If

                    End Select

                Next

                OutputList.Add("Supported audio device count: " & SupportedDeviceCount)

                Return String.Join(vbCrLf, OutputList)

            Catch ex As Exception
                Return "Unable to get sound device info. THe following error occurred: " & vbCrLf & ex.ToString
            End Try

        End Function

    End Class



End Namespace
