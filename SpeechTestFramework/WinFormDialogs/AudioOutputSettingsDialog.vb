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


Public Class AudioOutputSettingsDialog

    Public CurrentAudioApiSettings As Audio.AudioApiSettings
    Public DefaultDriverName As String

    Public Sub New(Optional FixedSampleRate As Integer? = Nothing, Optional ByVal DefaultDriverName As String = "")

        ' This call is required by the designer.
        InitializeComponent()

        'Initializing PA if not already done (using a call to the method Pa_GetDeviceCount to check if PA is initialized.)
        If Audio.PortAudio.Pa_GetDeviceCount = Audio.PortAudio.PaError.paNotInitialized Then
            Audio.PortAudio.Pa_Initialize()

        End If

        CurrentAudioApiSettings = New Audio.AudioApiSettings(FixedSampleRate)
        Me.DefaultDriverName = DefaultDriverName

    End Sub

    Private Sub AudioSettingsDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        PopulateDriverTypeList()
        PopulateDeviceLists()
        PopulateSampleRateList()
        PopulateBufferSizeList()

    End Sub

    Private Sub PopulateDriverTypeList()

        driverTypeComboBox.Items.Clear()
        Dim hostApiCount As Integer = Audio.PortAudio.Pa_GetHostApiCount()
        Dim DefaultHostApiListIndex As Integer = 0
        Dim DefaultDriverListIndex As Integer? = Nothing
        For i As Integer = 0 To hostApiCount - 1
            Dim hostApiInfo As Audio.PortAudio.PaHostApiInfo = Audio.PortAudio.Pa_GetHostApiInfo(i)
            If hostApiInfo.type <> Audio.PortAudio.PaHostApiTypeId.paInDevelopment Then
                driverTypeComboBox.Items.Add(hostApiInfo.name)

                'Storing the index of the system default to be used to pre-select a driver in the list
                If i = Audio.PortAudio.Pa_GetDefaultHostApi() Then DefaultHostApiListIndex = i

                'Storing the index of the default selection (if any) to be used to pre-select a driver in the list
                If hostApiInfo.name = DefaultDriverName Then DefaultDriverListIndex = i

            End If
        Next
        If hostApiCount > 0 Then
            If DefaultDriverListIndex.HasValue Then
                'Using primarily the driver selected by the calling code as default
                driverTypeComboBox.SelectedIndex = DefaultDriverListIndex
            Else
                'If no such driver existed, the system default is selected
                driverTypeComboBox.SelectedIndex = DefaultHostApiListIndex
            End If
        End If

    End Sub


    Private Sub PopulateSampleRateList()

        Dim ListOfSampleRates As New List(Of Integer)
        ListOfSampleRates.Add(192000)
        ListOfSampleRates.Add(176400)
        ListOfSampleRates.Add(96000)
        ListOfSampleRates.Add(88200)
        ListOfSampleRates.Add(48000)
        ListOfSampleRates.Add(44100)
        ListOfSampleRates.Add(38400)
        ListOfSampleRates.Add(37800)
        ListOfSampleRates.Add(32000)
        ListOfSampleRates.Add(24000)
        ListOfSampleRates.Add(22050)
        ListOfSampleRates.Add(19200)
        ListOfSampleRates.Add(18900)
        ListOfSampleRates.Add(16000)
        ListOfSampleRates.Add(12000)
        ListOfSampleRates.Add(11025)
        ListOfSampleRates.Add(9600)
        ListOfSampleRates.Add(8000)

        sampleRateComboBox.Items.Clear()

        For Each SR In ListOfSampleRates
            sampleRateComboBox.Items.Add(SR)
        Next

        'Settings default sample rate
        'Disabling selection of new sample rate if the sample rate is fixed
        If CurrentAudioApiSettings.IsFixedSampleRate = True Then
            If Not ListOfSampleRates.Contains(CurrentAudioApiSettings.FixedSampleRate) Then
                ListOfSampleRates.Add(CurrentAudioApiSettings.FixedSampleRate)
                sampleRateComboBox.Items.Add(CurrentAudioApiSettings.FixedSampleRate)
            End If
            sampleRateComboBox.SelectedItem = CurrentAudioApiSettings.FixedSampleRate
            sampleRateComboBox.Enabled = False
        Else
            sampleRateComboBox.SelectedIndex = 4
            sampleRateComboBox.Enabled = True
        End If

        CurrentAudioApiSettings.SampleRate = sampleRateComboBox.SelectedItem

    End Sub


    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub DriverTypeComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles driverTypeComboBox.SelectedIndexChanged

        'Getting the current driver type

        Dim hostApiCount As Integer = Audio.PortAudio.Pa_GetHostApiCount()
        For i As Integer = 0 To hostApiCount - 1
            Dim hostApiInfo As Audio.PortAudio.PaHostApiInfo = Audio.PortAudio.Pa_GetHostApiInfo(i)
            If hostApiInfo.type <> Audio.PortAudio.PaHostApiTypeId.paInDevelopment Then
                If i = driverTypeComboBox.SelectedIndex Then
                    CurrentAudioApiSettings.SelectedApiInfo = hostApiInfo
                End If
            End If
        Next

        'Setting text of settingsGroupBox
        If hostApiCount > 0 Then
            settingsGroupBox.Text = CurrentAudioApiSettings.SelectedApiInfo.name & " settings"
            PopulateDeviceLists()
            PopulateBufferSizeList()
        Else
            'MsgBox("hostApiCount: " & hostApiCount & " Cannot proceed.")
        End If

    End Sub

    Private Sub PopulateDeviceLists()

        'Clearing previously added devices
        outputDeviceComboBox.Items.Clear()


        Select Case CurrentAudioApiSettings.SelectedApiInfo.type
            Case Audio.PortAudio.PaHostApiTypeId.paMME, Audio.PortAudio.PaHostApiTypeId.paDirectSound, Audio.PortAudio.PaHostApiTypeId.paWASAPI, Audio.PortAudio.PaHostApiTypeId.paWDMKS

                'Modifying layout
                outputDeviceLabel.Visible = True
                outputDeviceComboBox.Visible = True
                deviceSettingsButton.Visible = False

                'Adding all devices of the selected type
                Dim deviceCount As Integer = Audio.PortAudio.Pa_GetDeviceCount()
                Dim selectedHostApiDeviceCount As Integer = 0
                Dim DefaultInputDeviceIndex As Integer = 0
                Dim DefaultOutputDeviceIndex As Integer = 0
                For i As Integer = 0 To deviceCount - 1
                    Dim paDeviceInfo As Audio.PortAudio.PaDeviceInfo = Audio.PortAudio.Pa_GetDeviceInfo(i)
                    Dim paHostApi As Audio.PortAudio.PaHostApiInfo = Audio.PortAudio.Pa_GetHostApiInfo(paDeviceInfo.hostApi)
                    If paHostApi.type = CurrentAudioApiSettings.SelectedApiInfo.type Then

                        'Adding output devices
                        If paDeviceInfo.maxOutputChannels > 0 Then
                            outputDeviceComboBox.Items.Add(paDeviceInfo.name)

                            'Getting the list index of the default device
                            If i = paHostApi.defaultOutputDevice Then DefaultOutputDeviceIndex = outputDeviceComboBox.Items.Count - 1
                        End If

                    End If
                Next

                'Setting the default device as selected
                If outputDeviceComboBox.Items.Count > 0 Then
                    outputDeviceComboBox.SelectedIndex = DefaultOutputDeviceIndex
                End If

            Case Audio.PortAudio.PaHostApiTypeId.paASIO

                deviceSettingsButton.Visible = True

                'Adding all output devices of the selected type
                Dim deviceCount As Integer = Audio.PortAudio.Pa_GetDeviceCount()
                Dim selectedHostApiDeviceCount As Integer = 0
                Dim DefaultDeviceIndex As Integer = 0
                For i As Integer = 0 To deviceCount - 1
                    Dim paDeviceInfo As Audio.PortAudio.PaDeviceInfo = Audio.PortAudio.Pa_GetDeviceInfo(i)
                    Dim paHostApi As Audio.PortAudio.PaHostApiInfo = Audio.PortAudio.Pa_GetHostApiInfo(paDeviceInfo.hostApi)

                    'MsgBox(paHostApi.type.ToString & " " & paDeviceInfo.name.ToString)

                    If paHostApi.type = CurrentAudioApiSettings.SelectedApiInfo.type Then

                        'Adding devices
                        outputDeviceComboBox.Items.Add(paDeviceInfo.name)
                        'Getting the list index of the default device
                        If i = paHostApi.defaultInputDevice Then DefaultDeviceIndex = outputDeviceComboBox.Items.Count - 1

                    End If
                Next

                'Setting the default device as selected
                If outputDeviceComboBox.Items.Count > 0 Then
                    outputDeviceComboBox.SelectedIndex = DefaultDeviceIndex
                End If


            Case Else
                outputDeviceComboBox.Visible = True
                MsgBox(CurrentAudioApiSettings.SelectedApiInfo.type.ToString & " is not yet implemented." & vbCrLf &
                       "Please select another driver type.")
        End Select

        'Identifying the default set devices
        IdentifyOutputDevice()

    End Sub

    Private Sub PopulateBufferSizeList()

        bufferSizeComboBox.Items.Clear()
        Dim bufferSize As Integer = 32
        While bufferSize < 100000
            bufferSizeComboBox.Items.Add(bufferSize)
            bufferSize *= 2
        End While

        Select Case CurrentAudioApiSettings.SelectedApiInfo.type
            Case Audio.PortAudio.PaHostApiTypeId.paMME
                bufferSizeComboBox.SelectedIndex = 6

            Case Audio.PortAudio.PaHostApiTypeId.paDirectSound
                bufferSizeComboBox.SelectedIndex = 6

            Case Audio.PortAudio.PaHostApiTypeId.paASIO, Audio.PortAudio.PaHostApiTypeId.paALSA
                bufferSizeComboBox.SelectedIndex = 6

            Case Audio.PortAudio.PaHostApiTypeId.paWASAPI
                bufferSizeComboBox.SelectedIndex = 6

            Case Audio.PortAudio.PaHostApiTypeId.paWDMKS
                bufferSizeComboBox.SelectedIndex = 6

            Case Else
                bufferSizeComboBox.SelectedIndex = 6
        End Select

        IdentifyBufferSize()

    End Sub

    Private Sub InputDeviceComboBox_SelectedIndexChanged(sender As Object, e As EventArgs)
        IdentifyInputDevice()
    End Sub

    Private Sub IdentifyInputDevice()

        Dim deviceCount As Integer = Audio.PortAudio.Pa_GetDeviceCount()

        Dim inputDeviceList As New List(Of KeyValuePair(Of Integer, Audio.PortAudio.PaDeviceInfo))

        For i As Integer = 0 To deviceCount - 1
            Dim paDeviceInfo As Audio.PortAudio.PaDeviceInfo = Audio.PortAudio.Pa_GetDeviceInfo(i)
            Dim paHostApi As Audio.PortAudio.PaHostApiInfo = Audio.PortAudio.Pa_GetHostApiInfo(paDeviceInfo.hostApi)
            If paHostApi.type = CurrentAudioApiSettings.SelectedApiInfo.type Then
                If paDeviceInfo.maxInputChannels > 0 Then
                    inputDeviceList.Add(New KeyValuePair(Of Integer, Audio.PortAudio.PaDeviceInfo)(i, paDeviceInfo))
                End If
            End If
        Next

        Select Case CurrentAudioApiSettings.SelectedApiInfo.type
            Case Audio.PortAudio.PaHostApiTypeId.paMME, Audio.PortAudio.PaHostApiTypeId.paDirectSound, Audio.PortAudio.PaHostApiTypeId.paWASAPI, Audio.PortAudio.PaHostApiTypeId.paWDMKS

                CurrentAudioApiSettings.SelectedInputDeviceInfo = Nothing
                CurrentAudioApiSettings.SelectedInputDevice = Nothing

                CurrentAudioApiSettings.SelectedInputAndOutputDeviceInfo = Nothing

            Case Audio.PortAudio.PaHostApiTypeId.paASIO

                CurrentAudioApiSettings.SelectedInputAndOutputDeviceInfo = inputDeviceList(outputDeviceComboBox.SelectedIndex).Value

                CurrentAudioApiSettings.SelectedInputDevice = inputDeviceList(outputDeviceComboBox.SelectedIndex).Key
                CurrentAudioApiSettings.SelectedOutputDevice = inputDeviceList(outputDeviceComboBox.SelectedIndex).Key

                CurrentAudioApiSettings.SelectedInputDeviceInfo = Nothing
                CurrentAudioApiSettings.SelectedOutputDeviceInfo = Nothing

                'Displaying available channels
                OutputChannelCountLabel.Text = "Output: " & CurrentAudioApiSettings.SelectedInputAndOutputDeviceInfo.Value.maxOutputChannels

            Case Else
                outputDeviceComboBox.Visible = True
                MsgBox(CurrentAudioApiSettings.SelectedApiInfo.type.ToString & " is not yet implemented." & vbCrLf &
                       "Please select another driver type.")
                Exit Sub
        End Select

    End Sub

    Private Sub OutputDeviceComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles outputDeviceComboBox.SelectedIndexChanged
        IdentifyOutputDevice()
    End Sub

    Private Sub IdentifyOutputDevice()

        Dim deviceCount As Integer = Audio.PortAudio.Pa_GetDeviceCount()

        Dim outputDeviceList As New List(Of KeyValuePair(Of Integer, Audio.PortAudio.PaDeviceInfo))

        For i As Integer = 0 To deviceCount - 1
            Dim paDeviceInfo As Audio.PortAudio.PaDeviceInfo = Audio.PortAudio.Pa_GetDeviceInfo(i)
            Dim paHostApi As Audio.PortAudio.PaHostApiInfo = Audio.PortAudio.Pa_GetHostApiInfo(paDeviceInfo.hostApi)
            If paHostApi.type = CurrentAudioApiSettings.SelectedApiInfo.type Then
                If paDeviceInfo.maxOutputChannels > 0 Then
                    outputDeviceList.Add(New KeyValuePair(Of Integer, Audio.PortAudio.PaDeviceInfo)(i, paDeviceInfo))
                End If
            End If
        Next

        Select Case CurrentAudioApiSettings.SelectedApiInfo.type
            Case Audio.PortAudio.PaHostApiTypeId.paMME, Audio.PortAudio.PaHostApiTypeId.paDirectSound, Audio.PortAudio.PaHostApiTypeId.paWASAPI, Audio.PortAudio.PaHostApiTypeId.paWDMKS

                CurrentAudioApiSettings.SelectedOutputDeviceInfo = outputDeviceList(outputDeviceComboBox.SelectedIndex).Value
                CurrentAudioApiSettings.SelectedOutputDevice = outputDeviceList(outputDeviceComboBox.SelectedIndex).Key

                CurrentAudioApiSettings.SelectedInputAndOutputDeviceInfo = Nothing

                'Displaying available channels
                OutputChannelCountLabel.Text = "Output: " & CurrentAudioApiSettings.SelectedOutputDeviceInfo.Value.maxOutputChannels

            Case Audio.PortAudio.PaHostApiTypeId.paASIO
                'Nothing is done here since ASIO devices are selected using the inputDeviceComboBox

            Case Else
                outputDeviceComboBox.Visible = True
                MsgBox(CurrentAudioApiSettings.SelectedApiInfo.type.ToString & " is not yet implemented." & vbCrLf &
                       "Please select another driver type.")

                CurrentAudioApiSettings.SelectedOutputDeviceInfo = Nothing
                CurrentAudioApiSettings.SelectedOutputDevice = Nothing
                CurrentAudioApiSettings.SelectedInputAndOutputDeviceInfo = Nothing

                Exit Sub

        End Select

    End Sub

    Private Sub BufferSizeComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles bufferSizeComboBox.SelectedIndexChanged
        IdentifyBufferSize()
    End Sub

    Private Sub IdentifyBufferSize()

        CurrentAudioApiSettings.FramesPerBuffer = bufferSizeComboBox.SelectedItem
        DisplayLatency()

    End Sub

    Private Sub SampleRateComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles sampleRateComboBox.SelectedIndexChanged

        CurrentAudioApiSettings.SampleRate = sampleRateComboBox.SelectedItem

        DisplayLatency()

    End Sub


    Private Sub DisplayLatency()

        Dim sampleRateComboBoxItem As Object = sampleRateComboBox.SelectedItem
        Dim bufferSizeComboBoxItem As Object = bufferSizeComboBox.SelectedItem
        If sampleRateComboBoxItem IsNot Nothing AndAlso bufferSizeComboBoxItem IsNot Nothing Then
            Dim sampleRate As Integer = CInt(sampleRateComboBoxItem)
            Dim bufferSize As Integer = CInt(bufferSizeComboBoxItem)
            Me.latencyLabel.Text = "Minimum callback latency: " & (bufferSize * 1000 / sampleRate) & " ms"
        End If

    End Sub

    Private Sub DeviceSettingsButton_Click(sender As Object, e As EventArgs) Handles deviceSettingsButton.Click

        Select Case CurrentAudioApiSettings.SelectedApiInfo.type
            Case Audio.PortAudio.PaHostApiTypeId.paASIO
                Dim SelectedAsioDeviceIndex As Integer = 0
                If CurrentAudioApiSettings.SelectedInputDevice IsNot Nothing Then
                    SelectedAsioDeviceIndex = CInt(CurrentAudioApiSettings.SelectedInputDevice)
                    Audio.PortAudio.PaAsio_ShowControlPanel(SelectedAsioDeviceIndex, New IntPtr(Me.Handle.ToInt64()))
                End If
        End Select

    End Sub


End Class
