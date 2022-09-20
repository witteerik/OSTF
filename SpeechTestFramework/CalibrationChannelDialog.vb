Imports System.Windows.Forms

Public Class CalibrationChannelDialog

    Private NumberOfOutputChannels As Integer
    Public SelectedChannel As Integer = -1

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Public Sub New(ByVal NumberOfOutputChannels As Integer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.NumberOfOutputChannels = NumberOfOutputChannels

    End Sub

    Private Sub CalibrationChannelDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        For c = 1 To NumberOfOutputChannels
            SelectedChannelComboBox.Items.Add(c)
        Next

    End Sub

    Private Sub SelectedChannelComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles SelectedChannelComboBox.SelectedIndexChanged

        SelectedChannel = SelectedChannelComboBox.SelectedItem

        If SelectedChannel > -1 Then
            OK_Button.Enabled = True
        End If

    End Sub
End Class
