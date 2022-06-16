Public Class LoadOstaTestSituationsControl

    Public Property SelectedTestSpecification As TestSpecification = Nothing

    Public Property SelectedTestSituation As MediaSet = Nothing

    Public Event TestSituationSelected()

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub SearchForTests_Button_Click(sender As Object, e As EventArgs) Handles SearchForTests_Button.Click
        LoadTestSpecifications()
    End Sub

    Private Sub LoadTestSpecifications()

        If SelectedTestSpecification Is Nothing Then
            MsgBox("No test specification / speech material loaded!")
            Exit Sub
        End If

        SelectedTestSpecification.LoadAvailableTestSituationSpecifications()

        TestSelection_ComboBox.Items.Clear()

        If OstfSettings.AvailableTests.Count > 0 Then
            For Each AvailableTest In OstfSettings.AvailableTests
                TestSelection_ComboBox.Items.Add(AvailableTest)
            Next
            SelectTest_Button.Enabled = True
            TestSelection_ComboBox.Enabled = True
        Else
            MsgBox("No tests specifications are available", MsgBoxStyle.Information, "Loading test specifications")
            SelectedTestSituation = Nothing
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
                SelectedTestSituation = TestSelection_ComboBox.SelectedItem
            Else
                SelectedTestSituation = Nothing
                MsgBox("Select a test!", MsgBoxStyle.Information, "Selecting test specification")
            End If

        Else
            MsgBox("No tests specifications are available", MsgBoxStyle.Information, "Selecting test specification")
            SelectedTestSituation = Nothing
            SelectTest_Button.Enabled = False
            TestSelection_ComboBox.Enabled = False
        End If

        'Trigger the TestSituationSelected event
        RaiseEvent TestSituationSelected()

    End Sub

End Class
