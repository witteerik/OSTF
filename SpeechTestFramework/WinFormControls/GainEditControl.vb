Public Class GainEditControl
    Public EditEnabled As Boolean
    Public LeftSide As Boolean

    Public Event SettingsUpdated()

    Private Sub EnableEdit_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles EnableEdit_RadioButton.CheckedChanged

        Side_GroupBox.Enabled = True

        EditEnabled = True

        RaiseEvent SettingsUpdated()

    End Sub

    Private Sub DiableEdit_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles DiableEdit_RadioButton.CheckedChanged

        Side_GroupBox.Enabled = False

        EditEnabled = False

        RaiseEvent SettingsUpdated()

    End Sub

    Private Sub LeftSide_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles LeftSide_RadioButton.CheckedChanged
        LeftSide = True

        RaiseEvent SettingsUpdated()

    End Sub

    Private Sub RightSide_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles RightSide_RadioButton.CheckedChanged
        LeftSide = False

        RaiseEvent SettingsUpdated()

    End Sub


End Class
