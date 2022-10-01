Public Class AudiogramEditControl

    Public EditEnabled As Boolean
    Public LeftSide As Boolean
    Public AirConduction As Boolean
    Public Masked As Boolean
    Public NotHeard As Boolean
    Public Overheard As Boolean

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
        Conduction_GroupBox.Enabled = True
        Misc_GroupBox.Enabled = True

        EditEnabled = True

        RaiseEvent SettingsUpdated()

    End Sub

    Private Sub DiableEdit_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles DiableEdit_RadioButton.CheckedChanged

        Side_GroupBox.Enabled = False
        Conduction_GroupBox.Enabled = False
        Misc_GroupBox.Enabled = False

        EditEnabled = False

        RaiseEvent SettingsUpdated()

    End Sub

    Private Sub NotHeard_CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles NotHeard_CheckBox.CheckedChanged

        If NotHeard_CheckBox.Checked = True Then Overheard_CheckBox.Checked = False

        NotHeard = NotHeard_CheckBox.Checked

        RaiseEvent SettingsUpdated()

    End Sub

    Private Sub Overheard_CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles Overheard_CheckBox.CheckedChanged

        If Overheard_CheckBox.Checked = True Then NotHeard_CheckBox.Checked = False

        Overheard = Overheard_CheckBox.Checked

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

    Private Sub AirConduction_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles AirConduction_RadioButton.CheckedChanged
        AirConduction = True

        RaiseEvent SettingsUpdated()

    End Sub

    Private Sub BoneConduction_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles BoneConduction_RadioButton.CheckedChanged
        AirConduction = False

        RaiseEvent SettingsUpdated()

    End Sub

    Private Sub Masked_CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles Masked_CheckBox.CheckedChanged
        Masked = Masked_CheckBox.Checked

        RaiseEvent SettingsUpdated()

    End Sub

    Public Function GetPointType() As WinFormControls.Audiogram.AudiogramPointTypes

        Dim CurrentPointType As WinFormControls.Audiogram.AudiogramPointTypes

        If Masked = False Then
            'Unmasked
            If AirConduction = True Then
                'Air
                If LeftSide = True Then
                    'Left side
                    CurrentPointType = WinFormControls.Audiogram.AudiogramPointTypes.LeftAir
                Else
                    'Right side
                    CurrentPointType = WinFormControls.Audiogram.AudiogramPointTypes.RightAir
                End If
            Else
                'Bone
                If LeftSide = True Then
                    'Left side
                    CurrentPointType = WinFormControls.Audiogram.AudiogramPointTypes.LeftBone
                Else
                    'Right side
                    CurrentPointType = WinFormControls.Audiogram.AudiogramPointTypes.RightBone
                End If
            End If
        Else
            'Masked
            If AirConduction = True Then
                'Air
                If LeftSide = True Then
                    'Left side
                    CurrentPointType = WinFormControls.Audiogram.AudiogramPointTypes.LeftMaskedAir
                Else
                    'Right side
                    CurrentPointType = WinFormControls.Audiogram.AudiogramPointTypes.RightMaskedAir
                End If
            Else
                'Bone
                If LeftSide = True Then
                    'Left side
                    CurrentPointType = WinFormControls.Audiogram.AudiogramPointTypes.LeftMaskedBone
                Else
                    'Right side
                    CurrentPointType = WinFormControls.Audiogram.AudiogramPointTypes.RightMaskedBone
                End If
            End If
        End If

        Return CurrentPointType

    End Function

End Class
