Public Class LoadOstaTestSituationsControl

    Public Property SelectedTestSpecification As TestSpecification = Nothing

    Public Property SelectedTestSituation As MediaSet = Nothing

    Public Event TestSituationSelected()

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub SearchForTests_Button_Click(sender As Object, e As EventArgs) Handles SearchForTestSituations_Button.Click
        LoadTestSituationSpecifications()
    End Sub

    Private Sub LoadTestSituationSpecifications()

        If SelectedTestSpecification Is Nothing Then
            MsgBox("No test specification loaded!")
            Exit Sub
        End If

        SelectedTestSpecification.LoadAvailableTestSituationSpecifications()

        TestSituationSelection_ComboBox.Items.Clear()

        If SelectedTestSpecification.TestSituations.Count > 0 Then
            For Each TestSituation In SelectedTestSpecification.TestSituations
                TestSituationSelection_ComboBox.Items.Add(TestSituation)
            Next
            SelectTestSituation_Button.Enabled = True
            TestSituationSelection_ComboBox.Enabled = True
        Else
            MsgBox("No test situation specifications are available", MsgBoxStyle.Information, "Loading test situation specifications")
            SelectedTestSituation = Nothing
            SelectTestSituation_Button.Enabled = False
            TestSituationSelection_ComboBox.Enabled = False
        End If

        'Selecting the first item
        If TestSituationSelection_ComboBox.Items.Count > 0 Then
            TestSituationSelection_ComboBox.SelectedIndex = 0
        End If

    End Sub


    Private Sub SelectTestSituation_Button_Click(sender As Object, e As EventArgs) Handles SelectTestSituation_Button.Click

        If TestSituationSelection_ComboBox.Items.Count > 0 Then

            TestSituationSelection_ComboBox.Enabled = True

            If TestSituationSelection_ComboBox.SelectedItem IsNot Nothing Then
                SelectedTestSituation = TestSituationSelection_ComboBox.SelectedItem
            Else
                SelectedTestSituation = Nothing
                MsgBox("Select a test situation!", MsgBoxStyle.Information, "Selecting test situation specification")
            End If

        Else
            MsgBox("No test situation specifications are available", MsgBoxStyle.Information, "Selecting test situation specification")
            SelectedTestSituation = Nothing
            SelectTestSituation_Button.Enabled = False
            TestSituationSelection_ComboBox.Enabled = False
        End If

        'Trigger the TestSituationSelected event
        RaiseEvent TestSituationSelected()

    End Sub

End Class
