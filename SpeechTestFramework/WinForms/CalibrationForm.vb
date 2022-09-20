Public Class CalibrationForm


    Private ParentMixer As Audio.PortAudioVB.DuplexMixer
    Private NumberOfOutputChannels As Integer
    Public SelectedChannel As Integer = -1
    Private CalibrationFileDescriptions As New SortedList(Of String, String)

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByRef ParentMixer As Audio.PortAudioVB.DuplexMixer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.NumberOfOutputChannels = NumberOfOutputChannels
        Me.ParentMixer = ParentMixer

    End Sub

    Private Sub CalibrationForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Adding signals
        Dim CalibrationFiles = IO.Directory.GetFiles("CalibrationSignals")
        Dim CalibrationSounds As New List(Of Audio.Sound)

        Dim DescriptionFile = IO.Path.Combine("CalibrationSignals", "SignalDescriptions.txt")
        Dim InputLines() As String = System.IO.File.ReadAllLines(DescriptionFile, System.Text.Encoding.UTF8)
        For Each line In InputLines
            If line.Trim = "" Then Continue For
            If line.Trim.StartsWith("//") Then Continue For
            Dim LineSplit = line.Trim.Split("=")
            If LineSplit.Length < 2 Then Continue For
            If LineSplit(0).Trim = "" Then Continue For
            If LineSplit(1).Trim = "" Then Continue For
            'Adds the description (filename, description)
            CalibrationFileDescriptions.Add(IO.Path.GetFileNameWithoutExtension(LineSplit(0).Trim), LineSplit(1).Trim)
        Next

        'Adding sound files
        For Each File In CalibrationFiles
            If IO.Path.GetExtension(File) = ".wav" Then
                Dim NewCalibrationSound = Audio.Sound.LoadWaveFile(File)
                NewCalibrationSound.Description = IO.Path.GetFileNameWithoutExtension(File).Replace("_", " ")
                If NewCalibrationSound IsNot Nothing Then CalibrationSounds.Add(NewCalibrationSound)
            End If
        Next

        'Adding into CalibrationSignal_ComboBox
        For Each CalibrationSound In CalibrationSounds
            CalibrationSignal_ComboBox.Items.Add(CalibrationSound)
        Next
        If CalibrationSignal_ComboBox.Items.Count > 0 Then
            CalibrationSignal_ComboBox.SelectedIndex = 0
        End If

        'Adding levels
        For Level = 60 To 80 Step 5
            CalibrationLevel_ComboBox.Items.Add(Level)
        Next
        CalibrationLevel_ComboBox.SelectedItem = 70

        'Adding channels
        For c = 1 To NumberOfOutputChannels
            SelectedChannel_ComboBox.Items.Add(c)
        Next

    End Sub

    Private Sub SelectedChannelComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles SelectedChannel_ComboBox.SelectedIndexChanged

        SelectedChannel = SelectedChannel_ComboBox.SelectedItem

        If SelectedChannel > -1 Then
            OK_Button.Enabled = True
        End If

    End Sub

    Private Sub CalibrationSignal_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CalibrationSignal_ComboBox.SelectedIndexChanged

        CalibrationSignal_RichTextBox.Text = ""

        If CalibrationSignal_ComboBox.SelectedItem IsNot Nothing Then

            Dim CurrentCalibrationSound As Audio.Sound = CalibrationSignal_ComboBox.SelectedItem
            If CalibrationFileDescriptions.ContainsKey(CurrentCalibrationSound.FileName) Then
                CalibrationSignal_RichTextBox.Text = CalibrationFileDescriptions(CurrentCalibrationSound.FileName) & vbCrLf &
                    CurrentCalibrationSound.WaveFormat.ToString
            Else
                CalibrationSignal_RichTextBox.Text = "Calibration file without custom description." & vbCrLf &
                    CurrentCalibrationSound.WaveFormat.ToString
            End If

        End If

    End Sub

End Class