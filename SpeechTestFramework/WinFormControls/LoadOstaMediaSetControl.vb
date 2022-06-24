Public Class LoadOstaMediaSetControl

    Public Property SelectedTestSpecification As TestSpecification = Nothing

    Public Property SelectedMediaSet As MediaSet = Nothing

    Public Event MediaSetSelected()

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub SearchForMediaSets_Button_Click(sender As Object, e As EventArgs) Handles SearchForMediaSets_Button.Click
        LoadMediaSetSpecifications()
    End Sub

    Private Sub LoadMediaSetSpecifications()

        If SelectedTestSpecification Is Nothing Then
            MsgBox("No test specification loaded!")
            Exit Sub
        End If

        SelectedTestSpecification.LoadAvailableMediaSetSpecifications()

        MediaSetSelection_ComboBox.Items.Clear()

        If SelectedTestSpecification.MediaSets.Count > 0 Then
            For Each MediaSet In SelectedTestSpecification.MediaSets
                MediaSetSelection_ComboBox.Items.Add(MediaSet)
            Next
            SelectMediaSet_Button.Enabled = True
            MediaSetSelection_ComboBox.Enabled = True
        Else
            MsgBox("No media set specifications are available", MsgBoxStyle.Information, "Loading media set specifications")
            SelectedMediaSet = Nothing
            SelectMediaSet_Button.Enabled = False
            MediaSetSelection_ComboBox.Enabled = False
        End If

        'Selecting the first item
        If MediaSetSelection_ComboBox.Items.Count > 0 Then
            MediaSetSelection_ComboBox.SelectedIndex = 0
        End If

    End Sub


    Private Sub SelectMediaSet_Button_Click(sender As Object, e As EventArgs) Handles SelectMediaSet_Button.Click

        If MediaSetSelection_ComboBox.Items.Count > 0 Then

            MediaSetSelection_ComboBox.Enabled = True

            If MediaSetSelection_ComboBox.SelectedItem IsNot Nothing Then
                SelectedMediaSet = MediaSetSelection_ComboBox.SelectedItem
            Else
                SelectedMediaSet = Nothing
                MsgBox("Select a media set!", MsgBoxStyle.Information, "Selecting media set specification")
            End If

        Else
            MsgBox("No media set specifications are available", MsgBoxStyle.Information, "Selecting media set specification")
            SelectedMediaSet = Nothing
            SelectMediaSet_Button.Enabled = False
            MediaSetSelection_ComboBox.Enabled = False
        End If

        'Trigger the MediaSetSelected event
        If SelectedMediaSet IsNot Nothing Then RaiseEvent MediaSetSelected()

    End Sub

End Class
