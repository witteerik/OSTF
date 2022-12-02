Imports System.Windows.Forms

Public Class GainDiagramSettingsDialog

    Public EditEnabled As Boolean
    Public LeftSide As Boolean

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

        EditEnabled = GainEditControl.EditEnabled
        LeftSide = GainEditControl.LeftSide

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click

        EditEnabled = False

        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

End Class
