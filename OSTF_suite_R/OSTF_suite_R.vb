Imports SpeechTestFramework

Public Class OSTF_suite_R

    Private CurrentApplicationForm As Windows.Forms.Form

    Private Sub OSTF_suite_R_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Initializing the OSTF 
        SpeechTestFramework.InitializeOSTF(Application.StartupPath)

        'Setting texts
        Launch_SiP_SE_R_Button.Text = "Swedish SiP-test" & vbCr & "(Research version)"
        Launch_SpeechMaterialCreator_Button.Text = "OSTF" & vbCr & "Speech Material Creator"
        Launch_SoundLevelCalibration_Button.Text = "OSTF" & vbCr & "Sound Level Calibration" & vbCr & "(Research version)"

    End Sub

    Public Function CheckApplicationRunning() As Boolean

        If CurrentApplicationForm Is Nothing Then
            Return False
        Else
            If CurrentApplicationForm.IsDisposed Then
                Return False
            Else
                Return True
            End If
        End If

    End Function

    Private Sub Launch_SiP_SE_R_Button_Click(sender As Object, e As EventArgs) Handles Launch_SiP_SE_R_Button.Click

        If CheckApplicationRunning() = False Then
            Try
                'Loads the Swedish SiP test in research mode
                CurrentApplicationForm = New SpeechTestFramework.SipTestGui("Swedish SiP-test", SpeechTestFramework.Utils.Constants.UserTypes.Research, SpeechTestFramework.Utils.Constants.Languages.English, False)
                CurrentApplicationForm.Show()
            Catch ex As Exception
                MsgBox("The following error occurred: " & ex.ToString, MsgBoxStyle.Critical, "Error!")
            End Try
        Else
            MsgBox("Only one application is allowed to run at the same time! Close the other OSTF application and try again!", MsgBoxStyle.Information, "Another OSTF application is currently running")
        End If

    End Sub

    Private Sub Launch_SpeechMaterialCreator_Button_Click(sender As Object, e As EventArgs) Handles Launch_SpeechMaterialCreator_Button.Click

        If CheckApplicationRunning() = False Then
            Try
                'Loads the Swedish SiP test in research mode
                CurrentApplicationForm = New SpeechTestFramework.SpeechMaterialCreator(Utils.Constants.UserTypes.Research, False)
                CurrentApplicationForm.Show()
            Catch ex As Exception
                MsgBox("The following error occurred: " & ex.ToString, MsgBoxStyle.Critical, "Error!")
            End Try
        Else
            MsgBox("Only one application is allowed to run at the same time! Close the other OSTF application and try again!", MsgBoxStyle.Information, "Another OSTF application is currently running")
        End If

    End Sub

    Private Sub Launch_SoundLevelCalibration_Button_Click(sender As Object, e As EventArgs) Handles Launch_SoundLevelCalibration_Button.Click

        If CheckApplicationRunning() = False Then
            Try
                'Loads the calibration form
                CurrentApplicationForm = New SpeechTestFramework.CalibrationForm(SpeechTestFramework.Utils.Constants.UserTypes.Research, False)
                CurrentApplicationForm.Show()
            Catch ex As Exception
                MsgBox("The following error occurred: " & ex.ToString, MsgBoxStyle.Critical, "Error!")
            End Try
        Else
            MsgBox("Only one application is allowed to run at the same time! Close the other OSTF application and try again!", MsgBoxStyle.Information, "Another OSTF application is currently running")
        End If

    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click

        Dim MyAboutBox = New AboutBox_WithLicenseButton
        MyAboutBox.SelectedLicense = LicenseBox.AvailableLicenses.MIT_X11
        MyAboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.PortAudio)
        MyAboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.MathNet)
        MyAboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.InTheHand)
        MyAboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.Wierstorf)
        MyAboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.SwedishSipRecordings)
        MyAboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.Modified_IFFM)
        MyAboutBox.Show()

    End Sub

    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub OSTF_suite_R_FormClosing(sender As Object, e As Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing

        OstfBase.TerminateOSTF()

    End Sub

End Class