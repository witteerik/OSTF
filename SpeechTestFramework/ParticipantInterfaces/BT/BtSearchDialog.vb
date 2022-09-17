Imports System.Windows.Forms
Imports InTheHand.Net
Imports InTheHand.Net.Sockets

Public Class BtSearchDialog

    Private AvailableBtDevices As IReadOnlyCollection(Of BluetoothDeviceInfo) = Nothing
    Private SelectedBtDevice As BluetoothDeviceInfo = Nothing
    Private Instructions As String = ""

    Public ReadOnly Property SelectedDevice As BluetoothDeviceInfo
        Get
            Return SelectedBtDevice
        End Get
    End Property

    Private Language As Utils.Languages

    Public Sub New(ByVal Language As Utils.Languages, Optional ByVal Instructions As String = "")

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Language = Language
        Me.Instructions = Instructions

    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

        If AvailableBtDevices Is Nothing Then Exit Sub

        If AvailableBtDevices.Count < 1 Then Exit Sub

        If BtDeviceListBox.SelectedItem Is Nothing Then Exit Sub

        Dim SelectedDeviceName As String = BtDeviceListBox.SelectedItem

        For Each d In AvailableBtDevices
            If d.DeviceName = SelectedDeviceName Then
                SelectedBtDevice = d
            End If
        Next

        If SelectedBtDevice Is Nothing Then
            MsgBox("Unable to select the device!")
            Exit Sub
        End If

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click

        SelectedBtDevice = Nothing

        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub BtSearchDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        SetSearchButtonText(False)

        Instructions_RichTextBox.Text = Instructions

        ' Setting other texts
        Select Case Language
            Case Utils.Languages.Swedish
                Me.Text = "Anslut blåtandsenhet"
                AvaliableDevicesLabel.Text = "Tillgängliga blåtandsenheter"
                SelectDeviceLabel.Text = "Välj enhet och klicka OK"
            Case Utils.Languages.English
                Me.Text = "Connect bluetooth device"
                AvaliableDevicesLabel.Text = "Available bluetooth devices"
                SelectDeviceLabel.Text = "Select device and click OK"
            Case Else
                Me.Text = "Connect bluetooth device"
                AvaliableDevicesLabel.Text = "Available bluetooth devices"
                SelectDeviceLabel.Text = "Select device and click OK"
        End Select


        'Hiding the SelectDeviceLabel until an available device has been detected
        SelectDeviceLabel.Visible = False


    End Sub



    Private Sub SearchButton_Click(sender As Object, e As EventArgs) Handles SearchButton.Click
        SearchForDevices()
    End Sub

    Public Sub SearchForDevices()

        SetSearchButtonText(True)

        BtDeviceListBox.Items.Clear()

        Dim client As New BluetoothClient
        AvailableBtDevices = client.DiscoverDevices

        SetSearchButtonText(False)

        For Each d In AvailableBtDevices
            BtDeviceListBox.Items.Add(d.DeviceName)
        Next

        If AvailableBtDevices.Count > 0 Then
            SelectDeviceLabel.Visible = True
        Else
            SelectDeviceLabel.Visible = False
        End If

    End Sub

    Private Sub SetSearchButtonText(ByVal Searching As Boolean)

        If Searching = False Then

            SearchButton.Enabled = True

            Select Case Language
                Case Utils.Languages.Swedish
                    SearchButton.Text = "Sök efter blåtandsenheter"
                Case Utils.Languages.English
                    SearchButton.Text = "Search for bluetooth units"
                Case Else
                    SearchButton.Text = "Search for bluetooth units"
            End Select

        Else

            SearchButton.Enabled = False

            Select Case Language
                Case Utils.Languages.Swedish
                    SearchButton.Text = "Söker, vänligen vänta..."
                Case Utils.Languages.English
                    SearchButton.Text = "Searching, please wait..."
                Case Else
                    SearchButton.Text = "Searching, please wait..."
            End Select
        End If

        SearchButton.Update()

    End Sub

    ''' <summary>
    ''' Selectes the device with the device name DeviceName and returns 0, or a positive integer representing an error if the device could not be and paired.
    ''' </summary>
    ''' <returns></returns>
    Public Function SelectPairAndConnectDevice(ByVal DeviceName As String) As Integer

        If AvailableBtDevices Is Nothing Then Return 1

        If AvailableBtDevices.Count < 1 Then Return 2

        For Each d In AvailableBtDevices
            If d.DeviceName = DeviceName Then
                SelectedBtDevice = d
            End If
        Next

        If SelectedBtDevice Is Nothing Then Return 3

        ''Pairing
        'If SelectedBtDevice.Authenticated = False Then
        '    If InTheHand.Net.Bluetooth.BluetoothSecurity.PairRequest(SelectedBtDevice.DeviceAddress, PIN) = False Then Return 4
        'End If

        ''Connecting
        'BtClient = New BluetoothClient
        'BtClient.Connect(SelectedBtDevice.DeviceAddress, New Guid(UUID))

        'If BtClient.Connected = True Then
        '    BtStream = BtClient.GetStream
        'Else
        '    Return 5
        'End If

        ''Starting read and write threads
        'Dim bluetoothReadThread = New Threading.Thread(AddressOf BtReadThread)
        'bluetoothReadThread.Start()

        'Return 0

    End Function

End Class
