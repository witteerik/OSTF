Imports System.Windows.Forms
Imports System.Drawing

Public Class DirectionalForcedChoiceControl

    Private TargetPoints As New List(Of Tuple(Of Double, Drawing.Point))
    Private ResponseControls As New List(Of ResponseItem)

    Public Sub New()

        Me.New(New List(Of Double) From {-135, -30, 0, 30, 135})

    End Sub

    Public Sub New(ByVal RadialTargetPoints As List(Of Double))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Me.BackColor = Drawing.Color.FromArgb(40, 40, 40)
        Me.Dock = DockStyle.Fill

        Me.Invalidate()
        Me.Update()

        AddRadialTargetPoints(RadialTargetPoints)

    End Sub

    Private Sub AddRadialTargetPoints(ByVal HorizontalAzimuths As List(Of Double))

        Dim Radius As Double = 0.8 * GetLowestDimensionSize() / 2
        Dim CenterPoint = GetMyCenterPoint()

        For Each HorizontalAzimuth In HorizontalAzimuths
            Dim AngleInRadians = Utils.Math.Degrees2Radians(HorizontalAzimuth)
            Dim TargetX = CenterPoint.X - Radius * Math.Sin(AngleInRadians)
            Dim TargetY = CenterPoint.Y - Radius * Math.Cos(AngleInRadians)
            TargetPoints.Add(New Tuple(Of Double, Drawing.Point)(HorizontalAzimuth, New Drawing.Point(TargetX, TargetY)))
        Next



    End Sub

    Private Sub UpdateRadialPointLocations()

        Dim Radius As Double = 0.8 * GetLowestDimensionSize() / 2
        Dim CenterPoint = GetMyCenterPoint()

        For i = 0 To TargetPoints.Count - 1

            Dim AngleInRadians = Utils.Math.Degrees2Radians(TargetPoints(i).Item1)

            Dim TargetX = CenterPoint.X - Radius * Math.Sin(AngleInRadians)
            Dim TargetY = CenterPoint.Y - Radius * Math.Cos(AngleInRadians)

            TargetPoints(i) = New Tuple(Of Double, Drawing.Point)(TargetPoints(i).Item1, New Drawing.Point(TargetX, TargetY))

        Next

    End Sub

    Public Function GetCurrentItemRectangle() As Drawing.Rectangle

        Dim Corners As New List(Of Drawing.Point)
        Dim CornerAzimuths As New List(Of Double) From {135, 45, 225}

        Dim Radius As Double = 0.6 * GetLowestDimensionSize() / 2
        Dim CenterPoint = GetMyCenterPoint()

        For Each HorizontalAzimuth In CornerAzimuths

            Dim AngleInRadians = Utils.Math.Degrees2Radians(HorizontalAzimuth)

            Dim TargetX = CenterPoint.x - Radius * Math.Sin(AngleInRadians)
            Dim TargetY = CenterPoint.y - Radius * Math.Cos(AngleInRadians)

            Corners.Add(New Drawing.Point(TargetX, TargetY))
        Next

        Return New Drawing.Rectangle(Corners(1).X, Corners(1).Y, Corners(2).X - Corners(0).X, Corners(0).Y - Corners(1).Y)

    End Function

    Public Sub AddItems(ByVal ItemList As List(Of String))

        For i = 0 To ItemList.Count - 1

            Dim NewResponseItem = New ResponseItem()
            NewResponseItem.Text = ItemList(i)
            NewResponseItem.AutoSize = False
            NewResponseItem.BackColor = Drawing.Color.Transparent

            AddHandler NewResponseItem.MouseDown, AddressOf ResponseItem_MouseDown
            AddHandler NewResponseItem.MouseUp, AddressOf ResponseItem_MouseUp
            AddHandler NewResponseItem.MouseLeave, AddressOf ResponseItem_MouseLeave

            ResponseControls.Add(NewResponseItem)
        Next

        For Each ResponseControl In ResponseControls
            Me.Controls.Add(ResponseControl)
        Next

        SetItemLocations()

    End Sub


    Private Sub SetItemLocations()

        Dim ItemRectangle = GetCurrentItemRectangle()
        Dim CurrentCenterPoint = GetMyCenterPoint()

        Dim ItemCount As Integer = ResponseControls.Count

        Dim ItemDistance = ItemRectangle.Height / ItemCount
        Dim ItemHeight = 0.8 * (ItemRectangle.Height / ItemCount)
        Dim ItemWidth = 0.6 * ItemRectangle.Width

        For i = 0 To ResponseControls.Count - 1

            If CheckIfOnTarget(ResponseControls(i)).IsEmpty = False Then Continue For

            ResponseControls(i).Height = ItemHeight
            ResponseControls(i).Width = ItemWidth
            ResponseControls(i).Left = CurrentCenterPoint.X - ResponseControls(i).Width / 2
            ResponseControls(i).Top = ItemRectangle.Top + i * ItemDistance
        Next

        MaximizeFontSize()

    End Sub

    Private GripPoint As Drawing.Point = Nothing

    Private Sub ResponseItem_MouseDown(sender As Object, e As MouseEventArgs)

        Dim CastSender As ResponseItem = DirectCast(sender, ResponseItem)
        GripPoint = e.Location

        'Me.SuspendLayout()
        'For i = 0 To Me.Controls.Count - 1
        '    If Me.Controls(i) Is CastSender Then
        '        Dim CurrentReference = Me.Controls(i)
        '        'Me.Controls.RemoveAt(i)
        '        Me.Controls.SetChildIndex(CurrentReference, 0)
        '        Exit For
        '    End If
        'Next
        'Me.ResumeLayout()

        AddHandler CastSender.MouseMove, AddressOf ResponseItem_MouseMove

    End Sub

    Private Sub ResponseItem_MouseUp(sender As Object, e As MouseEventArgs)

        Dim CastSender As ResponseItem = DirectCast(sender, ResponseItem)
        RemoveHandler CastSender.MouseMove, AddressOf ResponseItem_MouseMove

        SetItemLocations()

    End Sub

    Private Sub ResponseItem_MouseLeave(sender As Object, e As EventArgs)

        Dim CastSender As ResponseItem = DirectCast(sender, ResponseItem)
        RemoveHandler CastSender.MouseMove, AddressOf ResponseItem_MouseMove

        SetItemLocations()

    End Sub


    Private Sub ResponseItem_MouseMove(sender As Object, e As MouseEventArgs)

        Dim CastSender As ResponseItem = DirectCast(sender, ResponseItem)

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
        Dim OverlappedTarget = CheckIfOnTarget(CastSender)
        If OverlappedTarget.IsEmpty = False Then
            LockToTarget(CastSender, OverlappedTarget)
        End If

    End Sub

    Private Function CheckIfOnTarget(ByRef ResponseItem As ResponseItem) As Drawing.Point

        For Each TargetPoint In TargetPoints
            If ResponseItem.ClientRectangle.Contains(TargetPoint.Item2 - ResponseItem.Location) = True Then
                Return TargetPoint.Item2
            End If
        Next

        Return Drawing.Point.Empty

    End Function

    Private Sub LockToTarget(ByRef ResponseItem As ResponseItem, ByVal TargetPoint As Drawing.Point)

        CenterOnTarget(ResponseItem, TargetPoint)

        'Shuts down the MouseMove handler
        RemoveHandler ResponseItem.MouseMove, AddressOf ResponseItem_MouseMove

        'Sends response
        SendResponse(ResponseItem, TargetPoint)

    End Sub


    Private Sub CenterOnTarget(ByRef ResponseItem As ResponseItem, ByVal CenterPoint As Drawing.Point)
        ResponseItem.Location = New Drawing.Point(CenterPoint.X - ResponseItem.ClientRectangle.Width / 2, CenterPoint.Y - ResponseItem.ClientRectangle.Height / 2)
    End Sub

    Private Sub SendResponse(ByRef ResponseItem As ResponseItem, ByVal SelectedPoint As Drawing.Point)



    End Sub

    Private Function GetLowestDimensionSize() As Double
        Return Math.Min(Me.ClientRectangle.Width, Me.ClientRectangle.Height)
    End Function

    Private Function GetMyCenterPoint() As Drawing.Point
        Return New Drawing.Point(Me.ClientRectangle.Width / 2, Me.ClientRectangle.Height / 2)
    End Function

    Private Sub PaintTargets(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

        Dim TargetPen As Drawing.Pen = New Drawing.Pen(Drawing.Color.FromArgb(255, 255, 128))
        Dim TargetBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(255, 255, 128))

        Dim TargetRectangleSideLength = GetLowestDimensionSize() / 12

        Dim TargetRectangleSize As New Drawing.Size(TargetRectangleSideLength, TargetRectangleSideLength)
        Dim TargetRectangleHalfSize As New Drawing.Size(TargetRectangleSize.Width / 2, TargetRectangleSize.Height / 2)

        For Each TargetPoint In TargetPoints

            Dim TargetRectanglePoint = TargetPoint.Item2 - TargetRectangleHalfSize
            Dim TargetRectangle As Drawing.Rectangle = New Drawing.Rectangle(TargetRectanglePoint, TargetRectangleSize)
            e.Graphics.DrawEllipse(TargetPen, TargetRectangle)
            e.Graphics.FillEllipse(TargetBrush, TargetRectangle)

        Next


        ''Draws a red border around the item if its segmentation is not validated.
        'If SegmentationItem.SegmentationCompleted = False Then
        '        e.Graphics.DrawRectangle(RedPen, New Rectangle(Me.ClientRectangle.X + 1, Me.ClientRectangle.Y + 1, Me.ClientRectangle.Width - 2, Me.ClientRectangle.Height - 2))
        '    End If

    End Sub

    Public Sub UpdateOnResize() Handles Me.Resize

        SetItemLocations()
        UpdateRadialPointLocations()
        Me.Invalidate()
        Me.Update()

    End Sub

    Private WordWrapFactor As Double? = 4

    Private Function HasText() As Boolean

        For Each item As ResponseItem In Me.Controls
            If item.Text <> "" Then Return True
        Next

        Return False

    End Function

    Public Sub MaximizeFontSize()

        If HasText() = True Then

            'Maximizing the font size
            Dim Texts As New List(Of String)

            'Getting usable width and height (based on the size of the first control (they should all be the same size)
            Dim UsableControlWidth = Me.Controls(0).Width * 0.9 ' Using 5% as margin
            Dim UsableControlHeight = Me.Controls(0).Height * 0.9

            'Reading texts from the controls
            For Each control As ResponseItem In Me.Controls
                Texts.Add(control.Text)
            Next

            'Word wrapping the text
            If WordWrapFactor.HasValue Then

                Dim SplitTexts As New List(Of String)
                For Each t In Texts

                    Dim CurrentlySplitText = t.Split(" ").ToList

                    Dim WordsPerTextRow = Math.Ceiling(CurrentlySplitText.Count / WordWrapFactor.Value)
                    Dim LineBreakInsertIndices As New List(Of Integer)
                    For i As Integer = WordsPerTextRow To CurrentlySplitText.Count - 1 Step WordsPerTextRow
                        LineBreakInsertIndices.Add(i)
                    Next

                    For i = 0 To LineBreakInsertIndices.Count - 1
                        Dim InverseIndex = LineBreakInsertIndices(LineBreakInsertIndices.Count - 1 - i)
                        CurrentlySplitText.Insert(InverseIndex, vbCrLf)
                    Next

                    SplitTexts.Add(String.Concat(CurrentlySplitText))

                Next

                Texts = SplitTexts

            End If


            Dim MaxFontSize As Single? = 70
            Dim StartFontSize As Single = 6
            Dim CurrentFont = Me.Controls(0).Font
            CurrentFont = New Drawing.Font(CurrentFont.FontFamily, StartFontSize, Drawing.GraphicsUnit.Point)

            Dim BreakOut As Boolean = False
            For s As Single = StartFontSize To MaxFontSize.Value Step 0.5

                For Each textString In Texts

                    Dim textSize = Windows.Forms.TextRenderer.MeasureText(textString, CurrentFont)

                    If textSize.Height > UsableControlHeight Then
                        BreakOut = True
                        Exit For
                    End If
                    If textSize.Width > UsableControlWidth Then
                        BreakOut = True
                        Exit For
                    End If

                Next

                If BreakOut = True Then Exit For

                CurrentFont = New Drawing.Font(CurrentFont.FontFamily, s, Drawing.GraphicsUnit.Point)

            Next

            For Each control As ResponseItem In Me.Controls
                control.Font = CurrentFont
            Next

        End If

    End Sub


    Private Sub DirectionalForcedChoiceControl_Click() Handles Me.Click

        AddItems({"1", "2", "3"}.ToList)

    End Sub
End Class


Public Class ResponseItem
    Inherits Label

    Public Sub New()

        Me.Font = New Font("Verdana", 40, FontStyle.Regular)
        Me.TextAlign = ContentAlignment.MiddleCenter
        Me.ForeColor = Drawing.Color.FromArgb(40, 40, 40)
        Me.AutoSize = False

        Me.SetStyle(ControlStyles.Opaque, False)
        Me.SetStyle(ControlStyles.UserPaint, True)

    End Sub


    Protected Overrides Sub OnPaintBackground(ByVal e As System.Windows.Forms.PaintEventArgs)

        'Drawing background with round edges
        Dim Radius As Single = Height / 8
        Dim DoubleRadius As Single = Radius * 2

        Dim TransparentBrush = New SolidBrush(Drawing.Color.Transparent)
        e.Graphics.FillRectangle(TransparentBrush, ClientRectangle)

        Dim FillBackgroundBrush = New SolidBrush(Drawing.Color.FromArgb(255, 255, 128))

        e.Graphics.FillPie(FillBackgroundBrush, 0, 0, DoubleRadius, DoubleRadius, 0, 360)
        e.Graphics.FillPie(FillBackgroundBrush, 0, Height - DoubleRadius, DoubleRadius, DoubleRadius, 0, 360)
        e.Graphics.FillPie(FillBackgroundBrush, Width - DoubleRadius, 0, DoubleRadius, DoubleRadius, 0, 360)
        e.Graphics.FillPie(FillBackgroundBrush, Width - DoubleRadius, Height - DoubleRadius, DoubleRadius, DoubleRadius, 0, 360)

        e.Graphics.FillRectangle(FillBackgroundBrush, New Rectangle(Radius, 0, ClientRectangle.Width - 2 * Radius, ClientRectangle.Height))
        e.Graphics.FillRectangle(FillBackgroundBrush, New Rectangle(0, Radius, ClientRectangle.Width, ClientRectangle.Height - 2 * Radius))

        'Drawing border
        Dim LineWidth As Single = 8
        Dim BorderPen = New Pen(Me.ForeColor, LineWidth)
        e.Graphics.DrawArc(BorderPen, 0, 0, DoubleRadius, DoubleRadius, 180, 90)
        e.Graphics.DrawArc(BorderPen, 0, Height - DoubleRadius, DoubleRadius, DoubleRadius, 90, 90)
        e.Graphics.DrawArc(BorderPen, Width - DoubleRadius, 0, DoubleRadius, DoubleRadius, 270, 90)
        e.Graphics.DrawArc(BorderPen, Width - DoubleRadius, Height - DoubleRadius, DoubleRadius, DoubleRadius, 0, 90)

        e.Graphics.DrawLine(BorderPen, 0, Radius, 0, ClientRectangle.Height - Radius)
        e.Graphics.DrawLine(BorderPen, ClientRectangle.Width, Radius, ClientRectangle.Width, ClientRectangle.Height - Radius)

        e.Graphics.DrawLine(BorderPen, 0 + Radius, 0, ClientRectangle.Width - Radius, 0)
        e.Graphics.DrawLine(BorderPen, Radius, 0 + ClientRectangle.Height, ClientRectangle.Width - Radius, ClientRectangle.Height)

    End Sub



End Class
