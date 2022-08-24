Imports System.Windows.Forms
Imports System.Drawing
Imports System.ComponentModel


Namespace WinFormControls


    <Serializable>
    Public Class PlotBase
        Inherits PictureBox


        <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
        Public Property Areas As New List(Of Area)

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
        Public Property Lines As New List(Of Line)

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
        Public Property PointSeries As New List(Of PointSerie)



        Public Property PlotAreaRelativeMarginLeft As Single = 0.1
        Public Property PlotAreaRelativeMarginRight As Single = 0.1
        Public Property PlotAreaRelativeMarginTop As Single = 0.1
        Public Property PlotAreaRelativeMarginBottom As Single = 0.1

        Public Property XlimMin As Single = 0
        Public Property XlimMax As Single = 10

        Public Property Xlog As Boolean = False
        Public Property XlogBase As Single = 2

        Public Property YlimMin As Single = 0
        Public Property YlimMax As Single = 10

        Public Property Yreversed As Boolean = False
        Public Property Ylog As Boolean = False
        Public Property YlogBase As Single = 10

        Public Property PlotAreaBorderColor As Color = Color.DarkGray
        Public Property PlotAreaBorder As Boolean = True
        Public Property GridLineColor As Color = Color.Gray
        Public Property DashedGridLineColor As Color = Color.Gray
        Public Property XaxisGridLinePositions As New List(Of Single)
        Public Property XaxisDashedGridLinePositions As New List(Of Single)
        Public Property XaxisDrawTop As Boolean = False
        Public Property XaxisDrawBottom As Boolean = True
        Public Property XaxisTickPositions As New List(Of Single)
        Public Property XaxisTickHeight As Single = 2
        Public Property XaxisTextPositions As New List(Of Single)
        Public Property XaxisTextValues As String()
        Public Property XaxisTextSize As Single = 1
        Public Property XAxisTextBrush As Brush = Brushes.Black

        Public Property YaxisGridLinePositions As New List(Of Single)
        Public Property YaxisDashedGridLinePositions As New List(Of Single)
        Public Property YaxisDrawLeft As Boolean = True
        Public Property YaxisDrawRight As Boolean = False
        Public Property YaxisTickPositions As New List(Of Single)
        Public Property YaxisTickWidth As Single = 2
        Public Property YaxisTextPositions As New List(Of Single)
        Public Property YaxisTextValues As String()
        Public Property YaxisTextSize As Single = 1
        Public Property YaxisTextBrush As Brush = Brushes.Black

        Private MidCentreStringFormat As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}


        Public ReadOnly Property GetProportionOfPlotHeight(Optional ByVal Proportion As Single = 0.01) As Single
            Get
                Return PlotAreaHeight() * Proportion
            End Get
        End Property

        Public ReadOnly Property ProportionOfPlotWidth(Optional ByVal Proportion As Single = 0.01) As Single
            Get
                Return PlotAreaWidth() * Proportion
            End Get
        End Property

        Public ReadOnly Property XValueToCoordinate(ByVal x As Single) As Single
            Get
                Dim Output As Single

                If Xlog = False Then
                    Output = PlotAreaLeft() + ((x - XlimMin) / Xrange()) * PlotAreaWidth()
                Else
                    'Overriding Xmin to avoid log of non-positive values
                    Dim LimXmin As Single = getBase_n_Log(Math.Max(Single.Epsilon, XlimMin), XlogBase)
                    Dim LimXmax As Single = getBase_n_Log(Math.Max(Single.Epsilon, XlimMax), XlogBase)
                    Dim LimXrange As Single = LimXmax - LimXmin

                    Output = PlotAreaLeft() + ((getBase_n_Log(x, XlogBase) - LimXmin) / LimXrange) * PlotAreaWidth()

                End If

                Return Output
            End Get
        End Property

        Public ReadOnly Property YValueToCoordinate(ByVal y As Single) As Single
            Get
                Dim Output As Single

                If Yreversed = True Then

                    If Ylog = False Then
                        Output = PlotAreaTop() + ((y - YlimMin) / Yrange()) * PlotAreaHeight()
                    Else
                        'Overriding Ymin to avoid log of non-positive values
                        Dim LimYmin As Single = getBase_n_Log(Math.Max(Single.Epsilon, YlimMin), YlogBase)
                        Dim LimYmax As Single = getBase_n_Log(Math.Max(Single.Epsilon, YlimMax), YlogBase)
                        Dim LimYrange As Single = LimYmax - LimYmin

                        Output = PlotAreaTop() + ((getBase_n_Log(y, YlogBase) - LimYmin) / LimYrange) * PlotAreaHeight()

                    End If

                Else

                    If Ylog = False Then
                        Output = PlotAreaBottom() - ((y - YlimMin) / Yrange()) * PlotAreaHeight()
                    Else
                        'Overriding Ymin to avoid log of non-positive values
                        Dim LimYmin As Single = getBase_n_Log(Math.Max(Single.Epsilon, YlimMin), YlogBase)
                        Dim LimYmax As Single = getBase_n_Log(Math.Max(Single.Epsilon, YlimMax), YlogBase)
                        Dim LimYrange As Single = LimYmax - LimYmin

                        Output = PlotAreaBottom() - ((getBase_n_Log(y, YlogBase) - LimYmin) / LimYrange) * PlotAreaHeight()

                    End If

                End If

                Return Output

            End Get
        End Property


        Public Sub New()

            MyBase.New

            'Setting the default background color of the control
            Me.BackColor = SystemColors.Window

            'AddSomeLinesAndPoints()


        End Sub

        Sub Me_Rezised() Handles Me.Resize
            Invalidate()
        End Sub

        Private Sub AddSomeLinesAndPoints()

            'Adding some PointSeries
            PointSeries.Add(New PointSerie With {.Color = Color.Red, .PointSize = 2, .Type = PointSerie.PointTypes.Cross, .XValues = {1, 2, 3, 4}, .YValues = {4, 4, 4, 1}})
            PointSeries.Add(New PointSerie With {.Color = Color.Green, .PointSize = 2, .Type = PointSerie.PointTypes.FilledCircle, .XValues = {1, 2, 3, 4}, .YValues = {1, 1, 1, 2}})
            PointSeries.Add(New PointSerie With {.Color = Color.Green, .PointSize = 2, .Type = PointSerie.PointTypes.FilledRectangle, .XValues = {1, 2, 3, 4}, .YValues = {2, 2, 2, 2}})
            PointSeries.Add(New PointSerie With {.Color = Color.Green, .PointSize = 2, .Type = PointSerie.PointTypes.OpenCircle, .XValues = {1, 2, 3, 4}, .YValues = {5, 5, 5, 5}})
            PointSeries.Add(New PointSerie With {.Color = Color.Green, .PointSize = 2, .Type = PointSerie.PointTypes.Rectangle, .XValues = {1, 2, 3, 9}, .YValues = {9, 9, 9, 9}})

            Areas.Add(New Area With {.Color = Color.Yellow, .XValues = {1, 2, 3, 4, 5, 6}, .YValuesUpper = {0.42, 0.83, 0.44, 0.43, 0.42, 0.41}, .YValuesLower = {0.42, 0.57, 0.51, 0.41, 0.41, 0.4}})
            Areas.Add(New Area With {.Color = Color.Blue, .XValues = {1, 2, 3, 4, 5, 6}, .YValuesUpper = {0.32, 0.53, 0.64, 0.63, 0.62, 0.61}, .YValuesLower = {0.42, 0.57, 0.51, 0.71, 0.71, 0.7}})

            Lines.Add(New Line With {.Color = Color.Green, .Dashed = True, .DashPattern = {2, 3}, .LineWidth = 1, .XValues = {1, 2, 3, 4}, .YValues = {4, 4, 4, 1}})
            Lines.Add(New Line With {.Color = Color.Green, .Dashed = False, .DashPattern = {2, 3}, .LineWidth = 2, .XValues = {1, 2, 3, 9}, .YValues = {9, 9, 9, 9}})

        End Sub

        Public Function PlotAreaMarginLeft() As Single
            Return PlotAreaRelativeMarginLeft * ClientRectangle.Width
        End Function
        Public Function PlotAreaMarginRight() As Single
            Return PlotAreaRelativeMarginRight * ClientRectangle.Width
        End Function

        Public Function PlotAreaMarginTop() As Single
            Return PlotAreaRelativeMarginTop * ClientRectangle.Height
        End Function

        Public Function PlotAreaMarginBottom() As Single
            Return PlotAreaRelativeMarginBottom * ClientRectangle.Height
        End Function

        Public Function PlotAreaLeft() As Single
            Return PlotAreaMarginLeft()
        End Function

        Public Function PlotAreaRight() As Single
            Return ClientRectangle.Width - PlotAreaMarginRight()
        End Function

        Public Function PlotAreaBottom() As Single
            Return ClientRectangle.Height - PlotAreaMarginBottom()
        End Function

        Public Function PlotAreaTop() As Single
            Return PlotAreaMarginTop()
        End Function

        Public Function PlotAreaWidth() As Single
            Return ClientRectangle.Width - PlotAreaMarginLeft() - PlotAreaMarginRight()
        End Function

        Public Function PlotAreaHeight() As Single
            Return ClientRectangle.Height - PlotAreaMarginTop() - PlotAreaMarginBottom()
        End Function

        Public Function PlotAreaRectangle() As Rectangle
            Return New Rectangle(PlotAreaLeft, PlotAreaTop, PlotAreaWidth, PlotAreaHeight)
        End Function

        Private Function Xrange() As Single
            Return XlimMax - XlimMin
        End Function
        Private Function Yrange() As Single
            Return YlimMax - YlimMin
        End Function



        Private Function getBase_n_Log(ByVal value As Single, Optional ByVal n As Single = 2) As Single

            Return Math.Log10(value) / Math.Log10(n)

        End Function


        Private Sub DrawPlotAreaBorder(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint
            If PlotAreaBorder = True Then
                e.Graphics.DrawRectangle(New Pen(PlotAreaBorderColor, GetProportionOfPlotHeight(0.005)), PlotAreaRectangle)
            End If
        End Sub

        Private Sub DrawVerticalGridLines(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint

            Dim LineWidth = ProportionOfPlotWidth(0.003)

            For Each l In XaxisGridLinePositions
                Dim x As Single = XValueToCoordinate(l)
                If x >= PlotAreaLeft() And x <= PlotAreaRight() Then
                    e.Graphics.DrawLine(New Pen(GridLineColor, LineWidth), x, PlotAreaBottom, x, PlotAreaTop)
                End If
            Next

            If XaxisDashedGridLinePositions.Count > 0 Then

                Dim DashedPen = New Pen(DashedGridLineColor, LineWidth)
                DashedPen.DashPattern = {LineWidth * 3, LineWidth * 2}

                For Each l In XaxisDashedGridLinePositions
                    Dim x As Single = XValueToCoordinate(l)
                    If x >= PlotAreaLeft() And x <= PlotAreaRight() Then
                        e.Graphics.DrawLine(DashedPen, x, PlotAreaBottom, x, PlotAreaTop)
                    End If
                Next

            End If

        End Sub

        Private Sub DrawHorizontalGridLines(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint

            Dim LineWidth = GetProportionOfPlotHeight(0.003)

            For Each l In YaxisGridLinePositions
                Dim y As Single = YValueToCoordinate(l)
                If y >= PlotAreaTop() And y <= PlotAreaBottom() Then
                    e.Graphics.DrawLine(New Pen(GridLineColor, LineWidth), PlotAreaLeft, y, PlotAreaRight, y)
                End If
            Next

            If YaxisDashedGridLinePositions.Count > 0 Then

                Dim DashedPen = New Pen(DashedGridLineColor, LineWidth)
                DashedPen.DashPattern = {LineWidth * 3, LineWidth * 2}

                For Each l In YaxisDashedGridLinePositions
                    Dim y As Single = YValueToCoordinate(l)
                    If y >= PlotAreaTop() And y <= PlotAreaBottom() Then
                        e.Graphics.DrawLine(DashedPen, PlotAreaLeft, y, PlotAreaRight, y)
                    End If
                Next
            End If

        End Sub

        Private Sub DrawXaxisTicks(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint

            Dim ProportionOfPlotWidth = Me.ProportionOfPlotWidth(0.003)
            Dim TickExtension = XaxisTickHeight * ProportionOfPlotWidth

            If XaxisDrawTop = True Then
                For Each l In XaxisTickPositions
                    Dim x As Single = XValueToCoordinate(l)
                    If x >= PlotAreaLeft() And x <= PlotAreaRight() Then

                        e.Graphics.DrawLine(New Pen(GridLineColor, ProportionOfPlotWidth), x, PlotAreaTop() - TickExtension, x, PlotAreaTop() + TickExtension)
                    End If

                Next
            End If

            If XaxisDrawBottom = True Then
                For Each l In XaxisTickPositions
                    Dim x As Single = XValueToCoordinate(l)
                    If x >= PlotAreaLeft() And x <= PlotAreaRight() Then
                        e.Graphics.DrawLine(New Pen(GridLineColor, ProportionOfPlotWidth), x, PlotAreaBottom() - TickExtension, x, PlotAreaBottom() + TickExtension)
                    End If
                Next
            End If

        End Sub

        Private Sub DrawYaxisTicks(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint

            Dim ProportionOfPlotHeight = GetProportionOfPlotHeight(0.003)
            Dim TickExtension = YaxisTickWidth * ProportionOfPlotHeight

            If YaxisDrawLeft = True Then
                For Each l In YaxisTickPositions
                    Dim y As Single = YValueToCoordinate(l)
                    If y >= PlotAreaTop() And y <= PlotAreaBottom() Then
                        e.Graphics.DrawLine(New Pen(GridLineColor, ProportionOfPlotHeight), PlotAreaLeft() - TickExtension, y, PlotAreaLeft() + TickExtension, y)
                    End If
                Next
            End If

            If YaxisDrawRight = True Then
                For Each l In YaxisTickPositions
                    Dim y As Single = YValueToCoordinate(l)
                    If y >= PlotAreaTop() And y <= PlotAreaBottom() Then
                        e.Graphics.DrawLine(New Pen(GridLineColor, ProportionOfPlotHeight), PlotAreaRight() - TickExtension, y, PlotAreaRight() + TickExtension, y)
                    End If
                Next
            End If


        End Sub

        Private Sub DrawXaxisText(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint

            'Exits if XaxisTextValues is nothint
            If XaxisTextValues Is Nothing Then Exit Sub

            Dim DrawCount As Integer = Math.Min(XaxisTextPositions.Count, XaxisTextValues.Length)
            Dim XAxisTextFont = New Drawing.Font(Me.Parent.Font.FontFamily, CSng(GetProportionOfPlotHeight() * XaxisTextSize), FontStyle.Regular, GraphicsUnit.Pixel)

            If XaxisDrawTop = True Then
                Dim y As Single = PlotAreaMarginTop() / 2
                For n = 0 To DrawCount - 1
                    Dim x As Single = XValueToCoordinate(XaxisTextPositions(n))
                    e.Graphics.DrawString(XaxisTextValues(n), XAxisTextFont, XAxisTextBrush, x, y, MidCentreStringFormat)
                Next
            End If

            If XaxisDrawBottom = True Then
                Dim y As Single = Me.ClientRectangle.Bottom - PlotAreaMarginBottom() / 2
                For n = 0 To DrawCount - 1
                    Dim x As Single = XValueToCoordinate(XaxisTextPositions(n))
                    e.Graphics.DrawString(XaxisTextValues(n), XAxisTextFont, XAxisTextBrush, x, y, MidCentreStringFormat)
                Next
            End If

        End Sub

        Private Sub DrawYaxisText(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint

            'Exits if YaxisTextValues is nothint
            If YaxisTextValues Is Nothing Then Exit Sub

            Dim DrawCount As Integer = Math.Min(YaxisTextPositions.Count, YaxisTextValues.Length)
            Dim YAxisTextFont = New Drawing.Font(Me.Parent.Font.FontFamily, CSng(GetProportionOfPlotHeight() * YaxisTextSize), FontStyle.Regular, GraphicsUnit.Pixel)

            If YaxisDrawLeft = True Then
                Dim x As Single = PlotAreaMarginLeft() / 2
                For n = 0 To DrawCount - 1
                    Dim y As Single = YValueToCoordinate(YaxisTextPositions(n))
                    e.Graphics.DrawString(YaxisTextValues(n), YAxisTextFont, YaxisTextBrush, x, y, MidCentreStringFormat)
                Next
            End If

            If YaxisDrawRight = True Then
                Dim x As Single = Me.ClientRectangle.Right - PlotAreaMarginRight() / 2
                For n = 0 To DrawCount - 1
                    Dim y As Single = YValueToCoordinate(YaxisTextPositions(n))
                    e.Graphics.DrawString(YaxisTextValues(n), YAxisTextFont, YaxisTextBrush, x, y, MidCentreStringFormat)
                Next
            End If

        End Sub

        Private Sub DrawAreas(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint

            For Each Area In Areas

                Dim CurrentBrush = New SolidBrush(Color.FromArgb(Area.Alpha, Area.Color.R, Area.Color.G, Area.Color.B))

                Dim DrawLength As Integer = Math.Min(Area.XValues.Length, Math.Min(Area.YValuesUpper.Length, Area.YValuesUpper.Length))

                Dim NewPoints As New List(Of Point)

                'Creating a path around the area
                'Adding the upper limit
                For n = 0 To DrawLength - 1
                    If Single.IsNaN(Area.YValuesUpper(n)) = True Then Continue For
                    NewPoints.Add(New Point(XValueToCoordinate(Area.XValues(n)), YValueToCoordinate(Area.YValuesUpper(n))))
                Next

                'Adding the lower limit in descending x-axis order
                For n = DrawLength - 1 To 0 Step -1
                    If Single.IsNaN(Area.YValuesLower(n)) = True Then Continue For
                    NewPoints.Add(New Point(XValueToCoordinate(Area.XValues(n)), YValueToCoordinate(Area.YValuesLower(n))))
                Next

                'Creating the path
                Dim NewPath As New Drawing2D.GraphicsPath()
                NewPath.StartFigure()
                NewPath.AddLines(NewPoints.ToArray)

                'Drawing the area
                For n = 0 To DrawLength - 2
                    e.Graphics.FillPath(CurrentBrush, NewPath)
                Next
            Next

        End Sub

        Private Sub DrawPointSeries(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint

            For Each PS In PointSeries

                Dim CurrentSize = PS.PointSize * (GetProportionOfPlotHeight(0.005) + ProportionOfPlotWidth(0.005))
                Dim HalfCurrentSize = CurrentSize / 2
                Dim CurrentPen = New Pen(PS.Color, CurrentSize)
                Dim CurrentBrush = New SolidBrush(PS.Color)
                Dim DrawLength As Integer = Math.Min(PS.XValues.Length, PS.YValues.Length)

                If Yreversed = False Then

                    Select Case PS.Type
                        Case PointSerie.PointTypes.Cross
                            For n = 0 To DrawLength - 1
                                If Single.IsNaN(PS.YValues(n)) = True Then Continue For
                                Dim CurrentX = XValueToCoordinate(PS.XValues(n))
                                Dim CurrentY = YValueToCoordinate(PS.YValues(n))
                                e.Graphics.DrawLine(CurrentPen, CurrentX - HalfCurrentSize, CurrentY - HalfCurrentSize, CurrentX + HalfCurrentSize, CurrentY + HalfCurrentSize)
                                e.Graphics.DrawLine(CurrentPen, CurrentX - HalfCurrentSize, CurrentY + HalfCurrentSize, CurrentX + HalfCurrentSize, CurrentY - HalfCurrentSize)
                            Next
                        Case PointSerie.PointTypes.FilledCircle
                            For n = 0 To DrawLength - 1
                                If Single.IsNaN(PS.YValues(n)) = True Then Continue For
                                e.Graphics.FillEllipse(CurrentBrush, XValueToCoordinate(PS.XValues(n)) - HalfCurrentSize, YValueToCoordinate(PS.YValues(n)) + HalfCurrentSize, CurrentSize, CurrentSize)
                            Next
                        Case PointSerie.PointTypes.OpenCircle
                            For n = 0 To DrawLength - 1
                                If Single.IsNaN(PS.YValues(n)) = True Then Continue For
                                e.Graphics.DrawArc(CurrentPen, XValueToCoordinate(PS.XValues(n)) - HalfCurrentSize, YValueToCoordinate(PS.YValues(n)) + HalfCurrentSize, CurrentSize, CurrentSize, 0, 360)
                            Next
                        Case PointSerie.PointTypes.Rectangle
                            For n = 0 To DrawLength - 1
                                If Single.IsNaN(PS.YValues(n)) = True Then Continue For
                                e.Graphics.DrawRectangle(CurrentPen, XValueToCoordinate(PS.XValues(n)) - HalfCurrentSize, YValueToCoordinate(PS.YValues(n)) + HalfCurrentSize, CurrentSize, CurrentSize)
                            Next
                        Case PointSerie.PointTypes.FilledRectangle
                            For n = 0 To DrawLength - 1
                                If Single.IsNaN(PS.YValues(n)) = True Then Continue For
                                e.Graphics.FillRectangle(CurrentBrush, XValueToCoordinate(PS.XValues(n)) - HalfCurrentSize, YValueToCoordinate(PS.YValues(n)) + HalfCurrentSize, CurrentSize, CurrentSize)
                            Next

                    End Select

                Else

                    Select Case PS.Type
                        Case PointSerie.PointTypes.Cross
                            For n = 0 To DrawLength - 1
                                If Single.IsNaN(PS.YValues(n)) = True Then Continue For
                                Dim CurrentX = XValueToCoordinate(PS.XValues(n))
                                Dim CurrentY = YValueToCoordinate(PS.YValues(n))
                                e.Graphics.DrawLine(CurrentPen, CurrentX - HalfCurrentSize, CurrentY + HalfCurrentSize, CurrentX + HalfCurrentSize, CurrentY - HalfCurrentSize)
                                e.Graphics.DrawLine(CurrentPen, CurrentX - HalfCurrentSize, CurrentY - HalfCurrentSize, CurrentX + HalfCurrentSize, CurrentY + HalfCurrentSize)
                            Next
                        Case PointSerie.PointTypes.FilledCircle
                            For n = 0 To DrawLength - 1
                                If Single.IsNaN(PS.YValues(n)) = True Then Continue For
                                e.Graphics.FillEllipse(CurrentBrush, XValueToCoordinate(PS.XValues(n)) - HalfCurrentSize, YValueToCoordinate(PS.YValues(n)) - HalfCurrentSize, CurrentSize, CurrentSize)
                            Next
                        Case PointSerie.PointTypes.OpenCircle
                            For n = 0 To DrawLength - 1
                                If Single.IsNaN(PS.YValues(n)) = True Then Continue For
                                e.Graphics.DrawArc(CurrentPen, XValueToCoordinate(PS.XValues(n)) - HalfCurrentSize, YValueToCoordinate(PS.YValues(n)) - HalfCurrentSize, CurrentSize, CurrentSize, 0, 360)
                            Next
                        Case PointSerie.PointTypes.Rectangle
                            For n = 0 To DrawLength - 1
                                If Single.IsNaN(PS.YValues(n)) = True Then Continue For
                                e.Graphics.DrawRectangle(CurrentPen, XValueToCoordinate(PS.XValues(n)) - HalfCurrentSize, YValueToCoordinate(PS.YValues(n)) - HalfCurrentSize, CurrentSize, CurrentSize)
                            Next
                        Case PointSerie.PointTypes.FilledRectangle
                            For n = 0 To DrawLength - 1
                                If Single.IsNaN(PS.YValues(n)) = True Then Continue For
                                e.Graphics.FillRectangle(CurrentBrush, XValueToCoordinate(PS.XValues(n)) - HalfCurrentSize, YValueToCoordinate(PS.YValues(n)) - HalfCurrentSize, CurrentSize, CurrentSize)
                            Next
                    End Select
                End If

            Next

        End Sub

        Private Sub DrawLines(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint

            For Each Line In Lines

                Dim CurrentSize = Line.LineWidth * (GetProportionOfPlotHeight(0.001) + ProportionOfPlotWidth(0.001))
                Dim CurrentPen = New Pen(Line.Color, CurrentSize)
                Dim DrawLength As Integer = Math.Min(Line.XValues.Length, Line.YValues.Length)
                If Line.Dashed = True Then
                    Dim TempDashPattern As New List(Of Single)
                    If Line.DashPattern IsNot Nothing Then
                        For Each p In Line.DashPattern
                            TempDashPattern.Add(p * CurrentSize)
                        Next
                        CurrentPen.DashPattern = TempDashPattern.ToArray
                    End If
                End If

                For n = 0 To DrawLength - 2
                    If Single.IsNaN(Line.YValues(n)) = True Then Continue For
                    e.Graphics.DrawLine(CurrentPen, XValueToCoordinate(Line.XValues(n)), YValueToCoordinate(Line.YValues(n)),
                                    XValueToCoordinate(Line.XValues(n + 1)), YValueToCoordinate(Line.YValues(n + 1)))
                Next
            Next

        End Sub


        <Serializable>
        Public Class Area
            Public Property YValuesUpper As Single()
            Public Property YValuesLower As Single()
            Public Property XValues As Single()
            Public Property Color As Color = Color.Yellow
            Public Property Alpha As Byte = 40

        End Class

        <Serializable>
        Public Class Line
            Public Property YValues As Single()
            Public Property XValues As Single()
            Public Property LineWidth As Single = 1
            Public Property Color As Color = Color.Black
            Public Property Dashed As Boolean = False
            Public Property DashPattern As Single() = {3, 2}
        End Class

        <Serializable>
        Public Class PointSerie
            Public Property YValues As Single()
            Public Property XValues As Single()
            Public Property Color As Color = Color.Black
            Public Property Type As PointTypes = PointTypes.FilledCircle
            Public Property PointSize As Single = 2
            Public Enum PointTypes
                OpenCircle
                FilledCircle
                Cross
                Rectangle
                FilledRectangle
            End Enum
        End Class

    End Class


End Namespace
