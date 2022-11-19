Imports System.Windows.Forms

Public Class DirectionalForcedChoiceControl

    Private TargetPoints As New List(Of Drawing.Point) From {New Drawing.Point(100, 100), New Drawing.Point(100, 300)}

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim ResponseControls As New List(Of TestWordLabel)
        For n = 0 To 3
            Dim NewTestWordLabel = New TestWordLabel
            NewTestWordLabel.Text = n
            NewTestWordLabel.AutoSize = False
            NewTestWordLabel.Height = 80
            NewTestWordLabel.Left = 120 * n
            NewTestWordLabel.Top = 150 * n

            AddHandler NewTestWordLabel.MouseDown, AddressOf TestWordLabel_MouseDown
            AddHandler NewTestWordLabel.MouseUp, AddressOf TestWordLabel_MouseUp
            AddHandler NewTestWordLabel.MouseLeave, AddressOf TestWordLabel_MouseLeave

            ResponseControls.Add(NewTestWordLabel)
        Next

        For Each ResponseControl In ResponseControls
            Me.Controls.Add(ResponseControl)
        Next


        For Each TargetPoint In TargetPoints
            Me.Controls.Add(New Windows.Forms.Label With {.Text = "o", .Location = TargetPoint})
        Next

    End Sub


    Private GripPoint As Drawing.Point = Nothing

    Private Sub TestWordLabel_MouseDown(sender As Object, e As MouseEventArgs)

        Dim CastSender As TestWordLabel = DirectCast(sender, TestWordLabel)
        GripPoint = e.Location
        AddHandler CastSender.MouseMove, AddressOf TestWordLabel_MouseMove

    End Sub

    Private Sub TestWordLabel_MouseUp(sender As Object, e As MouseEventArgs)

        Dim CastSender As TestWordLabel = DirectCast(sender, TestWordLabel)
        RemoveHandler CastSender.MouseMove, AddressOf TestWordLabel_MouseMove

    End Sub

    Private Sub TestWordLabel_MouseLeave(sender As Object, e As EventArgs)

        Dim CastSender As TestWordLabel = DirectCast(sender, TestWordLabel)
        RemoveHandler CastSender.MouseMove, AddressOf TestWordLabel_MouseMove

    End Sub


    Private Sub TestWordLabel_MouseMove(sender As Object, e As MouseEventArgs)

        Dim CastSender As TestWordLabel = DirectCast(sender, TestWordLabel)

        'Calculating the new location
        Dim NewLocation = CastSender.Location + e.Location - GripPoint

        'Limiting to parent client area
        NewLocation.X = Math.Max(0, NewLocation.X)
        NewLocation.Y = Math.Max(0, NewLocation.Y)
        NewLocation.X = Math.Min(NewLocation.X, CastSender.Parent.ClientRectangle.Width - CastSender.ClientRectangle.Width)
        NewLocation.Y = Math.Min(NewLocation.Y, CastSender.Parent.ClientRectangle.Height - CastSender.ClientRectangle.Height)

        'Setting the new location
        CastSender.Location = NewLocation

        'Check if the sender is on a target
        CheckIfOnTarget(CastSender)

    End Sub

    Private Sub CheckIfOnTarget(ByRef TestWordLabel As TestWordLabel)

        For Each TargetPoint In TargetPoints

            If TestWordLabel.ClientRectangle.Contains(TargetPoint - TestWordLabel.Location) = True Then

                CenterOnTarget(TestWordLabel, TargetPoint)

                'Shuts down the MouseMove handler
                RemoveHandler TestWordLabel.MouseMove, AddressOf TestWordLabel_MouseMove

                'Sends response
                SendResponse(TestWordLabel, TargetPoint)

                Exit Sub
            End If
        Next

    End Sub


    Private Sub CenterOnTarget(ByRef TestWordLabel As TestWordLabel, ByVal CenterPoint As Drawing.Point)
        TestWordLabel.Location = New Drawing.Point(CenterPoint.X - TestWordLabel.ClientRectangle.Width / 2, CenterPoint.Y - TestWordLabel.ClientRectangle.Height / 2)
    End Sub

    Private Sub SendResponse(ByRef TestWordLabel As TestWordLabel, ByVal SelectedPoint As Drawing.Point)



    End Sub

End Class
