Imports System.Windows.Forms

Public Class AudiogramSymbolDialog

    Public EditEnabled As Boolean
    Public LeftSide As Boolean
    Public AirConduction As Boolean
    Public Masked As Boolean
    Public NotHeard As Boolean
    Public Overheard As Boolean

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

        EditEnabled = AudiogramEditControl.EditEnabled
        LeftSide = AudiogramEditControl.LeftSide
        AirConduction = AudiogramEditControl.AirConduction
        Masked = AudiogramEditControl.Masked
        NotHeard = AudiogramEditControl.NotHeard
        Overheard = AudiogramEditControl.Overheard

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click

        EditEnabled = False

        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Public Function GetPointType()
        Return AudiogramEditControl.GetPointType
    End Function

End Class
