Public Class EnvironmentCreatorControl

    Private SelectedTestSpecification As TestSpecification

    Private Sub EnvironmentCreatorControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        UpdateControlEnabledStatuses()

    End Sub


    Private Sub LoadOstaTestSpecificationControl1_SpeechTestSpecificationSelected() Handles LoadOstaTestSpecificationControl1.SpeechTestSpecificationSelected

        Try

            LoadOstaTestSpecificationControl1.SelectedTestSpecification.LoadSpeechMaterialComponentsFile()

            SelectedTestSpecification = LoadOstaTestSpecificationControl1.SelectedTestSpecification

            If SelectedTestSpecification IsNot Nothing Then
                LoadedSpeechMaterialName_TextBox.Text = SelectedTestSpecification.SpeechMaterial.PrimaryStringRepresentation
            Else
                LoadedSpeechMaterialName_TextBox.Text = "No speech material loaded"
                MsgBox("Unable to load the speech material file.", MsgBoxStyle.Information, "File reading error")
            End If
        Catch ex As Exception
            MsgBox("The following error occured: " & vbCrLf & vbCrLf & ex.ToString)
        End Try

        NewTestSituationControl1.SetTestSpecification(SelectedTestSpecification)

        UpdateControlEnabledStatuses()

    End Sub

    Private Sub UpdateControlEnabledStatuses()

        If SelectedTestSpecification IsNot Nothing Then
            NewTestSituationControl1.Enabled = True
        Else
            NewTestSituationControl1.Enabled = False
        End If

    End Sub



End Class
