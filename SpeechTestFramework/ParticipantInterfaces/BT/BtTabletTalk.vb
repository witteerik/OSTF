
Imports InTheHand.Net
Imports InTheHand.Net.Sockets


Public Interface IBtTestController

    Sub ShowAvaliableDevices(ByVal DeviceNames As List(Of String))

    Sub IncomingBtMessage(ByVal Message As String)
    ReadOnly Property InvokeRequired As Boolean
    Function Invoke(method As [Delegate], ParamArray args() As Object) As Object

End Interface

'Public Interface IBtTabletTalker
'    Sub ParseBtMessage(ByRef Message As String)
'    Property BtTestController As Object

'End Interface

Public Class BtTabletTalker
    Inherits Windows.Forms.Control

    Property BtTestController As IBtTestController

    Property Language As Utils.Languages

    'Property TesteeControl As ITesteeControl

#Region "BtCom"

    Private UUID As String '= "056435e9-cfdd-4fb3-8cc8-9a4eb21c439c" 'Created with https://www.uuidgenerator.net/
    Private PIN As String '= "1234"

    Private AvailableBtDevices As IReadOnlyCollection(Of BluetoothDeviceInfo) = Nothing
    Private SelectedBtDevice As BluetoothDeviceInfo = Nothing
    Private BtClient As BluetoothClient = Nothing
    'Private BtStream As InTheHand.Net.Sockets.NonSocketNetworkStream = Nothing
    Private BtStream As InTheHand.Net.Sockets.NetworkStream = Nothing

    Delegate Sub StringArgReturningVoidDelegate([String] As String)

    Public Sub New(ByRef BtTestController As IBtTestController, UUID As String, PIN As String, ByVal Language As Utils.Languages)

        Me.BtTestController = BtTestController

        Me.UUID = UUID
        Me.PIN = PIN

        Me.Language = Language

    End Sub

    Public Function EstablishBtConnection(ByVal AppName As String) As Boolean

        Dim Instructions As String = ""
        Select Case Language
            Case Utils.Constants.Languages.Swedish
                Instructions = "Du behöver en surfplatta med appen " & AppName & " installerad." & vbCrLf & vbCrLf &
                    "Steg 1. Starta appen " & AppName & " på suftplattan och följ instruktionerna." & vbCrLf &
                    "Steg 2. Klicka sedan på knappen Sök nedan, och välj din enhet när den dyker upp i listan." & vbCrLf &
                    "Steg 3. Klicka till sist på OK nedan, eller Cancel för att avbryta."

            Case Utils.Constants.Languages.English
                Instructions = "You will need a tablet with the appen " & AppName & " installed." & vbCrLf & vbCrLf &
                    "Step 1. Start the app " & AppName & " on your tablet and follow the instructions." & vbCrLf &
                    "Step 2. Then click on the button Search below, and select you unit when it appears in the list." & vbCrLf &
                    "Step 3. Finally click OK, or Cancel to abort."

        End Select

        Dim MyBtSearchDialog = New BtSearchDialog(Language, Instructions)

        MyBtSearchDialog.ShowDialog()

        SelectedBtDevice = MyBtSearchDialog.SelectedDevice

        Dim ErrorCode As Integer = PairAndConnectDevice(SelectedBtDevice)
        If ErrorCode > 0 Then
            MsgBox("Unable to connect to the selected bluetooth device! Error code: " & ErrorCode)
            Return False
        End If

        'Setting language of the tablet interface
        Select Case Language
            Case Utils.Languages.Swedish
                SendBtMessage("L|SE")
            Case Utils.Languages.English
                SendBtMessage("L|EN")
            Case Else
                'Setting to english as default
                SendBtMessage("L|EN")
        End Select

        Return True

    End Function

    Public Function SearchForDevices() As List(Of String)

        Dim client As New BluetoothClient
        AvailableBtDevices = client.DiscoverDevices

        Dim DeviceNameList As New List(Of String)

        For Each d In AvailableBtDevices
            DeviceNameList.Add(d.DeviceName)
        Next

        Return DeviceNameList

    End Function

    ''' <summary>
    ''' Selectes the device with the device name DeviceName and returns 0, or a positive integer representing an error if the device could not be and paired.
    ''' </summary>
    ''' <returns></returns>
    Public Function PairAndConnectDevice(ByRef SelectedBtDevice As BluetoothDeviceInfo) As Integer

        Try

            If SelectedBtDevice Is Nothing Then Return 1

            'Pairing
            If SelectedBtDevice.Authenticated = False Then
                If InTheHand.Net.Bluetooth.BluetoothSecurity.PairRequest(SelectedBtDevice.DeviceAddress, PIN) = False Then Return 2
            End If

            'Connecting
            BtClient = New BluetoothClient
            BtClient.Connect(SelectedBtDevice.DeviceAddress, New Guid(UUID))

            If BtClient.Connected = True Then
                BtStream = BtClient.GetStream
            Else
                Return 3
            End If

            'Starting read and write threads
            Dim bluetoothReadThread = New Threading.Thread(AddressOf BtReadThread)
            bluetoothReadThread.Start()

            Return 0

        Catch ex As Exception
            MsgBox(ex.ToString)
            Return 3
        End Try

    End Function

    Public Sub DisconnectBT()
        Try
            BtClient.Close()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

    Public Function TrySendData() As Boolean

        'TODO This should probably be doen in a finer way!

        Try
            SendBtMessage("ping")
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function



    'Public Sub ConnectBT()

    '    Dim client As New BluetoothClient
    '    Dim devices = client.DiscoverDevices

    '    For Each d In devices
    '        If d.DeviceName = "Xperia L1" Then
    '            SelectedBtDevice = d
    '        End If
    '    Next

    '    If SelectedBtDevice Is Nothing Then
    '        MsgBox("No device selected!")
    '        Exit Sub
    '    End If

    '    'Pairing
    '    If PairDevice() = True Then
    '        'Dim bluetoothClientThread = New Threading.Thread(AddressOf ClientConnectThread)
    '        'bluetoothClientThread.Start()
    '        ClientConnectThread()

    '        'Starting read and write threads
    '        Dim bluetoothReadThread = New Threading.Thread(AddressOf BtReadThread)
    '        bluetoothReadThread.Start()

    '    Else
    '        MsgBox("Pairing failed!")
    '    End If

    'End Sub







    'Private Sub ClientConnectThread()

    '    BtClient = New BluetoothClient
    '    BtClient.Connect(SelectedBtDevice.DeviceAddress, New Guid(UUID))

    '    If BtClient.Connected = True Then

    '        BtStream = BtClient.GetStream
    '        'stream.ReadTimeout = 1000
    '        'Dim input As Byte()
    '        'stream.Read(input)


    '        ''Write
    '        'Dim text As String = "hejsan"
    '        'Dim message As Byte() = System.Text.Encoding.UTF8.GetBytes(text)
    '        'BtStream.Write(message, 0, message.Length)

    '    Else
    '        MsgBox("Failed to connect")
    '    End If


    'End Sub

    'Private Function PairDevice() As Boolean

    '    If SelectedBtDevice.Authenticated = False Then
    '        Dim Result = InTheHand.Net.Bluetooth.BluetoothSecurity.PairRequest(SelectedBtDevice.DeviceAddress, PIN)
    '        Return Result
    '    End If

    '    Return True

    'End Function


    Private Async Sub BtReadThread()

        Dim bytes(1023) As Byte
        While True
            If BtStream IsNot Nothing Then

                Try
                    Dim size As Integer = Await BtStream.ReadAsync(bytes, 0, bytes.Length)
                    Dim stringRead = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length)

                    Dim Message As String = stringRead.Trim.Trim(vbNullChar)

                    If Not Message = "" Then

                        IncomingBtMessage_ThreadSafe(Message)

                        ' Replacing the byte array
                        bytes(1023) = New Byte()

                    End If

                Catch ex As Exception
                    Utils.SendInfoToLog("BT error: " & ex.Message)
                End Try
            End If
        End While

    End Sub


    Private Sub IncomingBtMessage_ThreadSafe(ByVal Message As String)

        If Me.InvokeRequired Then
            Me.Invoke(New StringArgReturningVoidDelegate(AddressOf IncomingBtMessage_UnSafe),
                                    New Object() {Message})
        Else
            Me.IncomingBtMessage_UnSafe(Message)
        End If

    End Sub


    Private Sub IncomingBtMessage_UnSafe(ByVal Message As String)

        If BtTestController IsNot Nothing Then
            If BtTestController.InvokeRequired Then
                BtTestController.Invoke(New StringArgReturningVoidDelegate(AddressOf BtTestController.IncomingBtMessage),
                                        New Object() {Message})
            Else
                BtTestController.IncomingBtMessage(Message)
            End If
        End If

    End Sub




    ''' <summary>
    ''' Sends a message over bluetooth and returns true if success, and false if fail.
    ''' </summary>
    ''' <param name="Message"></param>
    ''' <returns></returns>
    Public Function SendBtMessage(ByVal Message As String) As Boolean

        'MsgBox("Add queing of messages by splitting by some character, eg # or § whatever is not used as stimuli. And remeber to check the max length of a message!")

        If BtStream IsNot Nothing Then

            Try

                'Writing message to stream
                ' We use § as an end-of-message character added to the end of the message (This means that the charecters | and § are forbidden within messages), 
                Dim message_bin As Byte() = System.Text.Encoding.UTF8.GetBytes(Message & "§")
                BtStream.Write(message_bin, 0, message_bin.Length)

                Return True

            Catch ex As Exception
                Return False
            End Try
        End If

        Return False

    End Function


#End Region


    Public Sub ShowTabletMenu()
        SendBtMessage("M")
    End Sub

End Class
