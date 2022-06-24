Public Class LoadOstaTestSpecificationControl

    Public Property SelectedTestSpecification As TestSpecification = Nothing

    Public Event SpeechTestSpecificationSelected()

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Sub New(ByVal AutoLoadTests As Boolean)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        If AutoLoadTests = True Then
            SelectTest_Button.Visible = False
            LoadTestSpecifications()
        End If

    End Sub

    Private Sub SearchForTests_Button_Click(sender As Object, e As EventArgs) Handles SearchForTests_Button.Click
        LoadTestSpecifications()
    End Sub

    Private Sub LoadTestSpecifications()

        OstfSettings.LoadAvailableTestSpecifications()

        TestSelection_ComboBox.Items.Clear()

        If OstfSettings.AvailableTests.Count > 0 Then
            For Each AvailableTest In OstfSettings.AvailableTests
                TestSelection_ComboBox.Items.Add(AvailableTest)
            Next
            SelectTest_Button.Enabled = True
            TestSelection_ComboBox.Enabled = True
        Else
            MsgBox("No tests specifications are available", MsgBoxStyle.Information, "Loading test specifications")
            SelectedTestSpecification = Nothing
            SelectTest_Button.Enabled = False
            TestSelection_ComboBox.Enabled = False
        End If

        'Selecting the first item
        If TestSelection_ComboBox.Items.Count > 0 Then
            TestSelection_ComboBox.SelectedIndex = 0
        End If

    End Sub


    Private Sub SelectTest_Button_Click(sender As Object, e As EventArgs) Handles SelectTest_Button.Click

        If TestSelection_ComboBox.Items.Count > 0 Then

            TestSelection_ComboBox.Enabled = True

            If TestSelection_ComboBox.SelectedItem IsNot Nothing Then
                SelectedTestSpecification = TestSelection_ComboBox.SelectedItem
            Else
                SelectedTestSpecification = Nothing
                MsgBox("Select a test!", MsgBoxStyle.Information, "Selecting test specification")
            End If

        Else
            MsgBox("No tests specifications are available", MsgBoxStyle.Information, "Selecting test specification")
            SelectedTestSpecification = Nothing
            SelectTest_Button.Enabled = False
            TestSelection_ComboBox.Enabled = False
        End If

        'Trigger the SpeechTestSpecificationSelected event
        If SelectedTestSpecification IsNot Nothing Then RaiseEvent SpeechTestSpecificationSelected()

    End Sub
End Class
