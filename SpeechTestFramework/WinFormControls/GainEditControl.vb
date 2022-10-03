Public Class GainEditControl
    Public EditEnabled As Boolean
    Public LeftSide As Boolean

    Public Event SettingsUpdated()

    Private _ShowEditEnabledOptions As Boolean = True
    Public Property ShowEditEnabledOptions As Boolean
        Get
            Return _ShowEditEnabledOptions
        End Get
        Set(value As Boolean)
            _ShowEditEnabledOptions = value
            SetShowEditEnabledPanelVisibility()
        End Set
    End Property

    Public Sub SetShowEditEnabledPanelVisibility()
        If _ShowEditEnabledOptions = True Then
            Me.Height = 200
            EditMode_GroupBox.Visible = True
            Content_TableLayoutPanel.RowStyles(0) = New Windows.Forms.RowStyle(Windows.Forms.SizeType.Absolute, 44)
        Else
            'Enabling the control by selecting the (invisible) EnableEdit_RadioButton
            EnableEdit_RadioButton.Checked = True
            Me.Height = 150
            EditMode_GroupBox.Visible = False
            Content_TableLayoutPanel.RowStyles(0) = New Windows.Forms.RowStyle(Windows.Forms.SizeType.Absolute, 1)
        End If
    End Sub

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
