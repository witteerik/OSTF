Imports System.Windows.Forms

Public Class AudiogramSymbolDialog

    Public EditEnabled As Boolean
    Public LeftSide As Boolean
    Public AirConduction As Boolean
    Public Masked As Boolean
    Public NotHeard As Boolean
    Public Overheard As Boolean

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

        EditEnabled = EnableEdit_RadioButton.Checked
        LeftSide = LeftSide_RadioButton.Checked
        AirConduction = AirConduction_RadioButton.Checked
        Masked = Masked_CheckBox.Checked
        NotHeard = NotHeard_CheckBox.Checked
        Overheard = Overheard_CheckBox.Checked

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click

        EditEnabled = False

        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub EnableEdit_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles EnableEdit_RadioButton.CheckedChanged

        Side_GroupBox.Enabled = True
        Conduction_GroupBox.Enabled = True
        Misc_GroupBox.Enabled = True

    End Sub

    Private Sub DiableEdit_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles DiableEdit_RadioButton.CheckedChanged

        Side_GroupBox.Enabled = False
        Conduction_GroupBox.Enabled = False
        Misc_GroupBox.Enabled = False

    End Sub

    Private Sub NotHeard_CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles NotHeard_CheckBox.CheckedChanged

        If NotHeard_CheckBox.Checked = True Then Overheard_CheckBox.Checked = False

    End Sub

    Private Sub Overheard_CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles Overheard_CheckBox.CheckedChanged

        If Overheard_CheckBox.Checked = True Then NotHeard_CheckBox.Checked = False

    End Sub
End Class
