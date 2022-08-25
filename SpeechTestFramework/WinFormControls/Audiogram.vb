Imports System.Windows.Forms
Imports System.Drawing

Namespace WinFormControls

    <Serializable>
    Public Class Audiogram
        Inherits PlotBase

        Public Property HideAudiogramLines As Boolean = False

        Private _AudiogramData As AudiogramData
        Public Property AudiogramData As AudiogramData
            Get
                Return _AudiogramData
            End Get
            Set(value As AudiogramData)
                _AudiogramData = value
                Invalidate()
                Update()
            End Set
        End Property

        Private Enum Sides
            Left
            Right
        End Enum

        Private Enum ConductionTypes
            AC
            BC
        End Enum

        Public Sub New()

            MyBase.New

            'Setting up audiogram properties
            PlotAreaRelativeMarginLeft = 0.1
            PlotAreaRelativeMarginRight = 0.1
            PlotAreaRelativeMarginTop = 0.1
            PlotAreaRelativeMarginBottom = 0.05

            XlimMin = 125
            XlimMax = 8000

            Xlog = True
            XlogBase = 2

            YlimMin = -10
            YlimMax = 110
            Yreversed = True
            Ylog = False
            YlogBase = 10

            PlotAreaBorderColor = Color.DarkGray
            PlotAreaBorder = True
            GridLineColor = Color.Gray

            XaxisGridLinePositions = New List(Of Single) From {125, 250, 500, 1000, 2000, 4000}
            XaxisDashedGridLinePositions = New List(Of Single) From {750, 1500, 3000, 6000}
            XaxisDrawTop = True
            XaxisDrawBottom = False
            XaxisTickPositions = New List(Of Single)
            XaxisTickHeight = 2
            XaxisTextPositions = New List(Of Single) From {125, 250, 500, 1000, 2000, 4000, 8000}
            XaxisTextValues = {"125", "250", "500", "1k", "2k", "4k", "8k"}
            XaxisTextSize = 8
            XAxisTextBrush = Brushes.Black

            YaxisGridLinePositions = New List(Of Single) From {0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100}
            YaxisDashedGridLinePositions = New List(Of Single) From {-5, 5, 15, 25, 35, 45, 55, 65, 75, 85, 95, 105}
            YaxisDrawLeft = True
            YaxisDrawRight = True
            YaxisTickPositions = New List(Of Single)
            YaxisTickWidth = 2
            YaxisTextPositions = New List(Of Single) From {0, 20, 40, 60, 80, 100}
            YaxisTextValues = {"0", "20", "40", "60", "80", "100"}
            YaxisTextSize = 8
            YaxisTextBrush = Brushes.Black


            '_AudiogramData = New AudiogramData
            '_AudiogramData.CreateRandomAudiogramData()

        End Sub

        Private Sub DrawAcPoints(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint

            If _AudiogramData Is Nothing Then Exit Sub

            Dim LineWidth = ProportionOfPlotWidth(0.003)
            Dim BluePen = New Pen(Color.Blue, LineWidth)
            Dim RedPen = New Pen(Color.Red, LineWidth)

            For Each TP In _AudiogramData.AC_Left
                DrawAudiogramPoint(TP, Sides.Left, ConductionTypes.AC, False, BluePen, RedPen, e)
            Next

            For Each TP In _AudiogramData.AC_Right
                DrawAudiogramPoint(TP, Sides.Right, ConductionTypes.AC, False, BluePen, RedPen, e)
            Next

            For Each TP In _AudiogramData.BC_Left
                DrawAudiogramPoint(TP, Sides.Left, ConductionTypes.BC, False, BluePen, RedPen, e)
            Next

            For Each TP In _AudiogramData.BC_Right
                DrawAudiogramPoint(TP, Sides.Right, ConductionTypes.BC, False, BluePen, RedPen, e)
            Next

            For Each TP In _AudiogramData.AC_Left_Masked
                DrawAudiogramPoint(TP, Sides.Left, ConductionTypes.AC, True, BluePen, RedPen, e)
            Next

            For Each TP In _AudiogramData.AC_Right_Masked
                DrawAudiogramPoint(TP, Sides.Right, ConductionTypes.AC, True, BluePen, RedPen, e)
            Next

            For Each TP In _AudiogramData.BC_Left_Masked
                DrawAudiogramPoint(TP, Sides.Left, ConductionTypes.BC, True, BluePen, RedPen, e)
            Next

            For Each TP In _AudiogramData.BC_Right_Masked
                DrawAudiogramPoint(TP, Sides.Right, ConductionTypes.BC, True, BluePen, RedPen, e)
            Next

            For Each TP In _AudiogramData.UCL_Left
                DrawAudiogramPoint(TP, Sides.Left, ConductionTypes.AC, False, BluePen, RedPen, e, True)
            Next

            For Each TP In _AudiogramData.UCL_Right
                DrawAudiogramPoint(TP, Sides.Right, ConductionTypes.AC, False, BluePen, RedPen, e, True)
            Next

        End Sub

        Private Sub DrawAudiogramPoint(ByRef TonePoint As AudiogramData.TonePoint, ByVal Side As Sides, ByVal ConductionType As ConductionTypes,
                                   ByVal Masked As Boolean, ByRef BluePen As Pen, ByRef RedPen As Pen, ByVal e As PaintEventArgs,
                                   Optional ByVal IsUCL As Boolean = False)

            Dim Radius As Single = ProportionOfPlotWidth(0.04)
            Dim BC_X_Shift As Single = ProportionOfPlotWidth(0.02)
            Dim Overheard_Shift As Single = ProportionOfPlotWidth(0.1)
            Dim ScaleFactor As Single = ProportionOfPlotWidth(0.1)

            Dim x = XValueToCoordinate(TonePoint.StimulusFrequency)
            Dim y = YValueToCoordinate(TonePoint.StimulusLevel)

            If IsUCL = False Then

                If Masked = False Then
                    'Unmasked values

                    Select Case ConductionType
                        Case ConductionTypes.AC

                            If Side = Sides.Left Then

                                'Left AC
                                Dim Point1 = New PointF(x - CSng(Radius * 0.5), y + CSng(Radius * 0.5))
                                Dim Point2 = New PointF(x + CSng(Radius * 0.5), y + CSng(Radius * 0.5))
                                Dim Point3 = New PointF(x + CSng(Radius * 0.5), y - CSng(Radius * 0.5))
                                Dim Point4 = New PointF(x - CSng(Radius * 0.5), y - CSng(Radius * 0.5))

                                e.Graphics.DrawLine(BluePen, Point1, Point3)
                                e.Graphics.DrawLine(BluePen, Point2, Point4)

                                'Adding the arrow for no response
                                If TonePoint.NoResponse = True Then
                                    DrawNotHeardArrow(Point2, e, Sides.Left, BluePen, RedPen, ScaleFactor)
                                End If

                                'Adding the overheard marking
                                If TonePoint.Overheard = True Then

                                    Dim OverHeardX = x + Overheard_Shift

                                    Dim Point1o = New PointF(OverHeardX - CSng(Radius * 0.4), y + CSng(Radius * 0.4))
                                    Dim Point2o = New PointF(OverHeardX + CSng(Radius * 0.4), y + CSng(Radius * 0.4))
                                    Dim Point3o = New PointF(OverHeardX + CSng(Radius * 0.4), y - CSng(Radius * 0.4))
                                    Dim Point4o = New PointF(OverHeardX - CSng(Radius * 0.4), y - CSng(Radius * 0.4))

                                    e.Graphics.DrawLine(RedPen, Point1o, Point3o)
                                    e.Graphics.DrawLine(RedPen, Point2o, Point4o)

                                End If

                            Else

                                'Right AC
                                e.Graphics.DrawArc(RedPen, x - CSng(Radius * 0.5), y - CSng(Radius * 0.5), Radius, Radius, 0, 360)

                                'Adding the arrow for no responce
                                If TonePoint.NoResponse = True Then
                                    Dim StartPoint As New PointF(x - CSng(Radius * 0.4), y + CSng(Radius * 0.4))
                                    DrawNotHeardArrow(StartPoint, e, Sides.Right, BluePen, RedPen, ScaleFactor)
                                End If

                                'Adding the overheard marking
                                If TonePoint.Overheard = True Then
                                    e.Graphics.DrawArc(BluePen, x - CSng(Radius * 0.35), y - CSng(Radius * 0.35), CSng(Radius * 0.7), CSng(Radius * 0.7), 0, 360)
                                End If

                            End If

                        Case ConductionTypes.BC

                            If Side = Sides.Left Then

                                'Left BC
                                Dim Point1 = New PointF(x + BC_X_Shift, y - CSng(Radius * 0.5))
                                Dim Point2 = New PointF(x + BC_X_Shift + CSng(Radius * 0.5), y)
                                Dim Point3 = New PointF(x + BC_X_Shift, y + CSng(Radius * 0.5))

                                e.Graphics.DrawLine(BluePen, Point1, Point2)
                                e.Graphics.DrawLine(BluePen, Point2, Point3)

                                'Adding the arrow for no responce
                                If TonePoint.NoResponse = True Then
                                    DrawNotHeardArrow(Point2, e, Sides.Left, BluePen, RedPen, ScaleFactor)
                                End If

                                'Adding the overheard marking
                                If TonePoint.Overheard = True Then
                                    'TODO: To be completed ...
                                    Dim OverHeardX = x - Overheard_Shift / 2.5

                                    Dim Point1o = New PointF(OverHeardX + BC_X_Shift, y - CSng(Radius * 0.35))
                                    Dim Point2o = New PointF(OverHeardX + BC_X_Shift + CSng(Radius * 0.35), y)
                                    Dim Point3o = New PointF(OverHeardX + BC_X_Shift, y + CSng(Radius * 0.35))

                                    e.Graphics.DrawLine(RedPen, Point1o, Point2o)
                                    e.Graphics.DrawLine(RedPen, Point2o, Point3o)

                                End If

                            Else

                                'Right BC
                                Dim Point1 = New PointF(x - BC_X_Shift, y - CSng(Radius * 0.5))
                                Dim Point2 = New PointF(x - BC_X_Shift - CSng(Radius * 0.5), y)
                                Dim Point3 = New PointF(x - BC_X_Shift, y + CSng(Radius * 0.5))

                                e.Graphics.DrawLine(RedPen, Point1, Point2)
                                e.Graphics.DrawLine(RedPen, Point2, Point3)

                                'Adding the arrow for no responce
                                If TonePoint.NoResponse = True Then
                                    DrawNotHeardArrow(Point2, e, Sides.Right, BluePen, RedPen, ScaleFactor)
                                End If

                                'Adding the overheard marking
                                If TonePoint.Overheard = True Then
                                    'TODO: To be completed ...
                                    Dim OverHeardX = x + Overheard_Shift / 2.5

                                    Dim Point1o = New PointF(OverHeardX - BC_X_Shift, y - CSng(Radius * 0.35))
                                    Dim Point2o = New PointF(OverHeardX - BC_X_Shift - CSng(Radius * 0.35), y)
                                    Dim Point3o = New PointF(OverHeardX - BC_X_Shift, y + CSng(Radius * 0.35))

                                    e.Graphics.DrawLine(BluePen, Point1o, Point2o)
                                    e.Graphics.DrawLine(BluePen, Point2o, Point3o)
                                End If

                            End If

                        Case Else

                            Throw New NotImplementedException("Unknown ConductionType")

                    End Select

                Else
                    'Masked values

                    Select Case ConductionType
                        Case ConductionTypes.AC

                            If Side = Sides.Left Then

                                Dim Point1 = New PointF(x - CSng(Radius * 0.5), y + CSng(Radius * 0.5))
                                Dim Point2 = New PointF(x + CSng(Radius * 0.5), y + CSng(Radius * 0.5))
                                Dim Point3 = New PointF(x + CSng(Radius * 0.5), y - CSng(Radius * 0.5))
                                Dim Point4 = New PointF(x - CSng(Radius * 0.5), y - CSng(Radius * 0.5))

                                e.Graphics.DrawPolygon(BluePen, {Point1, Point2, Point3, Point4})

                                'Adding the arrow for no responce
                                If TonePoint.NoResponse = True Then
                                    DrawNotHeardArrow(Point2, e, Sides.Left, BluePen, RedPen, ScaleFactor)
                                End If

                                'Adding the overheard marking
                                If TonePoint.Overheard = True Then
                                    'TODO: To be completed ...
                                End If


                            Else

                                Dim Point1 = New PointF(x - CSng(Radius * 0.5), y + CSng(Radius * 0.5))
                                Dim Point2 = New PointF(x + CSng(Radius * 0.5), y + CSng(Radius * 0.5))
                                Dim Point3 = New PointF(x, y - CSng(Radius * 0.5))

                                e.Graphics.DrawPolygon(RedPen, {Point1, Point2, Point3})

                                'Adding the arrow for no responce
                                If TonePoint.NoResponse = True Then
                                    DrawNotHeardArrow(Point1, e, Sides.Right, BluePen, RedPen, ScaleFactor)
                                End If

                                'Adding the overheard marking
                                If TonePoint.Overheard = True Then
                                    'TODO: To be completed ...
                                End If


                            End If
                        Case ConductionTypes.BC

                            If Side = Sides.Left Then

                                Dim Point1 = New PointF(x + BC_X_Shift, y + CSng(Radius * 0.5))
                                Dim Point2 = New PointF(x + BC_X_Shift + CSng(Radius * 0.5), y + CSng(Radius * 0.5))
                                Dim Point3 = New PointF(x + BC_X_Shift + CSng(Radius * 0.5), y - CSng(Radius * 0.5))
                                Dim Point4 = New PointF(x + BC_X_Shift, y - CSng(Radius * 0.5))

                                e.Graphics.DrawLine(BluePen, Point1, Point2)
                                e.Graphics.DrawLine(BluePen, Point2, Point3)
                                e.Graphics.DrawLine(BluePen, Point3, Point4)

                                'Adding the arrow for no responce
                                If TonePoint.NoResponse = True Then
                                    DrawNotHeardArrow(Point2, e, Sides.Left, BluePen, RedPen, ScaleFactor)
                                End If

                                'Adding the overheard marking
                                If TonePoint.Overheard = True Then
                                    'TODO: To be completed ...
                                End If

                            Else

                                Dim Point1 = New PointF(x - BC_X_Shift, y + CSng(Radius * 0.5))
                                Dim Point2 = New PointF(x - BC_X_Shift - CSng(Radius * 0.5), y + CSng(Radius * 0.5))
                                Dim Point3 = New PointF(x - BC_X_Shift - CSng(Radius * 0.5), y - CSng(Radius * 0.5))
                                Dim Point4 = New PointF(x - BC_X_Shift, y - CSng(Radius * 0.5))

                                e.Graphics.DrawLine(RedPen, Point1, Point2)
                                e.Graphics.DrawLine(RedPen, Point2, Point3)
                                e.Graphics.DrawLine(RedPen, Point3, Point4)

                                'Adding the arrow for no responce
                                If TonePoint.NoResponse = True Then
                                    DrawNotHeardArrow(Point2, e, Sides.Right, BluePen, RedPen, ScaleFactor)
                                End If

                                'Adding the overheard marking
                                If TonePoint.Overheard = True Then
                                    'TODO: To be completed ...
                                End If

                            End If

                        Case Else

                            Throw New NotImplementedException("Unknown ConductionType")

                    End Select

                End If

            Else

                If Side = Sides.Left Then

                    Dim Point1 = New PointF(x - CSng(Radius * 0.5), y - CSng(Radius * 0.5))
                    Dim Point2 = New PointF(x, y + CSng(Radius * 0.5))
                    Dim Point3 = New PointF(x + CSng(Radius * 0.5), y - CSng(Radius * 0.5))

                    e.Graphics.DrawLine(BluePen, Point1, Point2)
                    e.Graphics.DrawLine(BluePen, Point2, Point3)

                    'Adding the arrow for no responce
                    If TonePoint.NoResponse = True Then
                        DrawNotHeardArrow(Point2, e, Sides.Left, BluePen, RedPen, ScaleFactor)
                    End If

                    'Adding the overheard marking
                    If TonePoint.Overheard = True Then
                        'TODO: To be completed ...
                    End If

                Else

                    Dim Point1 = New PointF(x - CSng(Radius * 0.5), y + CSng(Radius * 0.5))
                    Dim Point2 = New PointF(x, y - CSng(Radius * 0.5))
                    Dim Point3 = New PointF(x + CSng(Radius * 0.5), y + CSng(Radius * 0.5))

                    e.Graphics.DrawLine(RedPen, Point1, Point2)
                    e.Graphics.DrawLine(RedPen, Point2, Point3)

                    'Adding the arrow for no responce
                    If TonePoint.NoResponse = True Then
                        DrawNotHeardArrow(Point1, e, Sides.Right, BluePen, RedPen, ScaleFactor)
                    End If

                    'Adding the overheard marking
                    If TonePoint.Overheard = True Then
                        'TODO: To be completed ...
                    End If

                End If

            End If


        End Sub

        Private Class AudiogramPoint
            Public Location As PointF
            Public HasValue As Boolean = False
            Public Overheard As Boolean = False
            Public NoResponse As Boolean = False
        End Class

        Private Sub DrawAudiogramLines(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint

            If _AudiogramData Is Nothing Then Exit Sub

            Dim LineWidth = ProportionOfPlotWidth(0.004)
            Dim BluePen = New Pen(Color.Blue, LineWidth)
            Dim RedPen = New Pen(Color.Red, LineWidth)

            Dim BlueDashedPen = New Pen(Color.Blue, LineWidth)
            BlueDashedPen.DashPattern = {LineWidth * 3, LineWidth * 2}
            Dim RedDashedPen = New Pen(Color.Red, LineWidth)
            RedDashedPen.DashPattern = {LineWidth * 3, LineWidth * 2}

            Dim ColumnCount As Integer = 11 'Hard coded for normal audiogram without HF region
            Dim AC_Left_Pts(ColumnCount - 1) As AudiogramPoint
            Dim AC_Right_Pts(ColumnCount - 1) As AudiogramPoint
            Dim AC_Left_Masked_Pts(ColumnCount - 1) As AudiogramPoint
            Dim AC_Right_Masked_Pts(ColumnCount - 1) As AudiogramPoint
            Dim BC_Left_Pts(ColumnCount - 1) As AudiogramPoint
            Dim BC_Right_Pts(ColumnCount - 1) As AudiogramPoint
            Dim BC_Left_Masked_Pts(ColumnCount - 1) As AudiogramPoint
            Dim BC_Right_Masked_Pts(ColumnCount - 1) As AudiogramPoint
            Dim UCL_Left_Pts(ColumnCount - 1) As AudiogramPoint
            Dim UCL_Right_Pts(ColumnCount - 1) As AudiogramPoint

            'Copying values from the audiogram data
            'Translates the input audiogram data to the standard used in the SHT.Audiogram class.

            Dim fs() As Integer = {125, 250, 500, 750, 1000, 1500, 2000, 3000, 4000, 6000, 8000}
            Dim TranslationDictionary As New Dictionary(Of Integer, Integer)
            For n = 0 To fs.Length - 1
                TranslationDictionary.Add(fs(n), n)
            Next

            For n = 0 To ColumnCount - 1
                AC_Left_Pts(n) = New AudiogramPoint
                AC_Right_Pts(n) = New AudiogramPoint
                BC_Left_Pts(n) = New AudiogramPoint
                BC_Right_Pts(n) = New AudiogramPoint
                AC_Left_Masked_Pts(n) = New AudiogramPoint
                AC_Right_Masked_Pts(n) = New AudiogramPoint
                BC_Left_Masked_Pts(n) = New AudiogramPoint
                BC_Right_Masked_Pts(n) = New AudiogramPoint
                UCL_Left_Pts(n) = New AudiogramPoint
                UCL_Right_Pts(n) = New AudiogramPoint
            Next

            For Each TonePoint In _AudiogramData.AC_Left
                Dim i As Integer = TranslationDictionary(TonePoint.StimulusFrequency)
                AC_Left_Pts(i).Location = New PointF(XValueToCoordinate(TonePoint.StimulusFrequency), YValueToCoordinate(TonePoint.StimulusLevel))
                AC_Left_Pts(i).NoResponse = TonePoint.NoResponse
                AC_Left_Pts(i).Overheard = TonePoint.Overheard
                AC_Left_Pts(i).HasValue = True
            Next


            For Each TonePoint In _AudiogramData.AC_Right
                Dim i As Integer = TranslationDictionary(TonePoint.StimulusFrequency)
                AC_Right_Pts(i).Location = New PointF(XValueToCoordinate(TonePoint.StimulusFrequency), YValueToCoordinate(TonePoint.StimulusLevel))
                AC_Right_Pts(i).NoResponse = TonePoint.NoResponse
                AC_Right_Pts(i).Overheard = TonePoint.Overheard
                AC_Right_Pts(i).HasValue = True
            Next

            For Each TonePoint In _AudiogramData.BC_Left
                Dim i As Integer = TranslationDictionary(TonePoint.StimulusFrequency)
                BC_Left_Pts(i).Location = New PointF(XValueToCoordinate(TonePoint.StimulusFrequency), YValueToCoordinate(TonePoint.StimulusLevel))
                BC_Left_Pts(i).NoResponse = TonePoint.NoResponse
                BC_Left_Pts(i).Overheard = TonePoint.Overheard
                BC_Left_Pts(i).HasValue = True
            Next


            For Each TonePoint In _AudiogramData.BC_Right
                Dim i As Integer = TranslationDictionary(TonePoint.StimulusFrequency)
                BC_Right_Pts(i).Location = New PointF(XValueToCoordinate(TonePoint.StimulusFrequency), YValueToCoordinate(TonePoint.StimulusLevel))
                BC_Right_Pts(i).NoResponse = TonePoint.NoResponse
                BC_Right_Pts(i).Overheard = TonePoint.Overheard
                BC_Right_Pts(i).HasValue = True
            Next

            For Each TonePoint In _AudiogramData.AC_Left_Masked
                Dim i As Integer = TranslationDictionary(TonePoint.StimulusFrequency)
                AC_Left_Masked_Pts(i).Location = New PointF(XValueToCoordinate(TonePoint.StimulusFrequency), YValueToCoordinate(TonePoint.StimulusLevel))
                AC_Left_Masked_Pts(i).NoResponse = TonePoint.NoResponse
                AC_Left_Masked_Pts(i).Overheard = TonePoint.Overheard
                AC_Left_Masked_Pts(i).HasValue = True
            Next


            For Each TonePoint In _AudiogramData.AC_Right_Masked
                Dim i As Integer = TranslationDictionary(TonePoint.StimulusFrequency)
                AC_Right_Masked_Pts(i).Location = New PointF(XValueToCoordinate(TonePoint.StimulusFrequency), YValueToCoordinate(TonePoint.StimulusLevel))
                AC_Right_Masked_Pts(i).NoResponse = TonePoint.NoResponse
                AC_Right_Masked_Pts(i).Overheard = TonePoint.Overheard
                AC_Right_Masked_Pts(i).HasValue = True
            Next

            For Each TonePoint In _AudiogramData.BC_Left_Masked
                Dim i As Integer = TranslationDictionary(TonePoint.StimulusFrequency)
                BC_Left_Masked_Pts(i).Location = New PointF(XValueToCoordinate(TonePoint.StimulusFrequency), YValueToCoordinate(TonePoint.StimulusLevel))
                BC_Left_Masked_Pts(i).NoResponse = TonePoint.NoResponse
                BC_Left_Masked_Pts(i).Overheard = TonePoint.Overheard
                BC_Left_Masked_Pts(i).HasValue = True
            Next


            For Each TonePoint In _AudiogramData.BC_Right_Masked
                Dim i As Integer = TranslationDictionary(TonePoint.StimulusFrequency)
                BC_Right_Masked_Pts(i).Location = New PointF(XValueToCoordinate(TonePoint.StimulusFrequency), YValueToCoordinate(TonePoint.StimulusLevel))
                BC_Right_Masked_Pts(i).NoResponse = TonePoint.NoResponse
                BC_Right_Masked_Pts(i).Overheard = TonePoint.Overheard
                BC_Right_Masked_Pts(i).HasValue = True
            Next

            For Each TonePoint In _AudiogramData.UCL_Left
                Dim i As Integer = TranslationDictionary(TonePoint.StimulusFrequency)
                UCL_Left_Pts(i).Location = New PointF(XValueToCoordinate(TonePoint.StimulusFrequency), YValueToCoordinate(TonePoint.StimulusLevel))
                UCL_Left_Pts(i).NoResponse = TonePoint.NoResponse
                UCL_Left_Pts(i).Overheard = TonePoint.Overheard
                UCL_Left_Pts(i).HasValue = True
            Next

            For Each TonePoint In _AudiogramData.UCL_Right
                Dim i As Integer = TranslationDictionary(TonePoint.StimulusFrequency)
                UCL_Right_Pts(i).Location = New PointF(XValueToCoordinate(TonePoint.StimulusFrequency), YValueToCoordinate(TonePoint.StimulusLevel))
                UCL_Right_Pts(i).NoResponse = TonePoint.NoResponse
                UCL_Right_Pts(i).Overheard = TonePoint.Overheard
                UCL_Right_Pts(i).HasValue = True
            Next

            Dim FreqIndex750Hz As Integer = 3
            Dim FreqIndex1500Hz As Integer = 5

            'Drawing lines
            'Drawing AC left
            For n = 0 To ColumnCount - 2

                'Checking if we should skip 750 Hz or 1500 Hz
                If (n = (FreqIndex750Hz - 1) Or n = (FreqIndex1500Hz - 1)) And (AC_Left_Pts(n + 1).HasValue = False) Then

                    'Skipping 750 or 1500 and drawing line to next
                    DrawAudiogramLine(AC_Left_Pts(n), AC_Left_Pts(n + 2), e, BluePen, Sides.Left, AC_Left_Masked_Pts(n), AC_Left_Masked_Pts(n + 2))
                    'Increasing n to jump to next 
                    n += 1
                Else
                    'Drawing normal left AC line
                    DrawAudiogramLine(AC_Left_Pts(n), AC_Left_Pts(n + 1), e, BluePen, Sides.Left, AC_Left_Masked_Pts(n), AC_Left_Masked_Pts(n + 1))
                End If

            Next


            'Drawing right AC line
            For n = 0 To ColumnCount - 2

                'Checking if we should skip 750 Hz or 1500 Hz
                If (n = (FreqIndex750Hz - 1) Or n = (FreqIndex1500Hz - 1)) And (AC_Right_Pts(n + 1).HasValue = False) Then

                    'Skipping 750 or 1500 and drawing line to next
                    DrawAudiogramLine(AC_Right_Pts(n), AC_Right_Pts(n + 2), e, RedPen, Sides.Right, AC_Right_Masked_Pts(n), AC_Right_Masked_Pts(n + 2))
                    'Increasing n to jump to next 
                    n += 1
                Else
                    'Drawing normal left AC line
                    DrawAudiogramLine(AC_Right_Pts(n), AC_Right_Pts(n + 1), e, RedPen, Sides.Right, AC_Right_Masked_Pts(n), AC_Right_Masked_Pts(n + 1))
                End If

            Next


            'Drawing left BC line
            For n = 0 To ColumnCount - 2

                'Checking if we should skip 750 Hz or 1500 Hz
                If (n = (FreqIndex750Hz - 1) Or n = (FreqIndex1500Hz - 1)) And (BC_Left_Pts(n + 1).HasValue = False) Then
                    DrawAudiogramLine(BC_Left_Pts(n), BC_Left_Pts(n + 2), e, BlueDashedPen, Sides.Left, BC_Left_Masked_Pts(n), BC_Left_Masked_Pts(n + 2))
                    'Increasing n to jump to next 
                    n += 1
                Else
                    DrawAudiogramLine(BC_Left_Pts(n), BC_Left_Pts(n + 1), e, BlueDashedPen, Sides.Left, BC_Left_Masked_Pts(n), BC_Left_Masked_Pts(n + 1))
                End If

            Next

            'Drawing right BC line
            For n = 0 To ColumnCount - 2

                'Checking if we should skip 750 Hz or 1500 Hz
                If (n = (FreqIndex750Hz - 1) Or n = (FreqIndex1500Hz - 1)) And (BC_Right_Pts(n + 1).HasValue = False) Then
                    DrawAudiogramLine(BC_Right_Pts(n), BC_Right_Pts(n + 2), e, RedDashedPen, Sides.Right, BC_Right_Masked_Pts(n), BC_Right_Masked_Pts(n + 2))
                    'Increasing n to jump to next 
                    n += 1
                Else
                    DrawAudiogramLine(BC_Right_Pts(n), BC_Right_Pts(n + 1), e, RedDashedPen, Sides.Right, BC_Right_Masked_Pts(n), BC_Right_Masked_Pts(n + 1))
                End If

            Next

            ''Drawing lines
            ''Drawing left ULC line
            Dim TempEmptyAudiogramPoint As New AudiogramPoint With {.HasValue = False}
            For n = 0 To ColumnCount - 2
                'Checking if we should skip 750 Hz or 1500 Hz
                If (n = (FreqIndex750Hz - 1) Or n = (FreqIndex1500Hz - 1)) And (UCL_Left_Pts(n + 1).HasValue = False) Then

                    'Skipping 750 or 1500 and drawing line to next
                    DrawAudiogramLine(UCL_Left_Pts(n), UCL_Left_Pts(n + 2), e, BluePen, Sides.Left, TempEmptyAudiogramPoint, TempEmptyAudiogramPoint)
                    'Increasing n to jump to next 
                    n += 1
                Else
                    'Drawing normal left AC line
                    DrawAudiogramLine(UCL_Left_Pts(n), UCL_Left_Pts(n + 1), e, BluePen, Sides.Left, TempEmptyAudiogramPoint, TempEmptyAudiogramPoint)
                End If
            Next

            'Drawing right ULC line
            For n = 0 To ColumnCount - 2
                'Checking if we should skip 750 Hz or 1500 Hz
                If (n = (FreqIndex750Hz - 1) Or n = (FreqIndex1500Hz - 1)) And (UCL_Right_Pts(n + 1).HasValue = False) Then

                    'Skipping 750 or 1500 and drawing line to next
                    DrawAudiogramLine(UCL_Right_Pts(n), UCL_Right_Pts(n + 2), e, RedPen, Sides.Right, TempEmptyAudiogramPoint, TempEmptyAudiogramPoint)
                    'Increasing n to jump to next 
                    n += 1
                Else
                    'Drawing normal left AC line
                    DrawAudiogramLine(UCL_Right_Pts(n), UCL_Right_Pts(n + 1), e, RedPen, Sides.Right, TempEmptyAudiogramPoint, TempEmptyAudiogramPoint)
                End If
            Next

        End Sub

        'Draws line 
        Private Sub DrawAudiogramLine(ByRef UnmaskedFirstPoint As AudiogramPoint, ByRef UnmaskedSecondPoint As AudiogramPoint,
                         ByRef e As PaintEventArgs, ByRef CurrentPen As Pen, ByVal Side As Sides,
                          ByRef MaskedFirstPoint As AudiogramPoint, ByRef MaskedSecondPoint As AudiogramPoint)

            Dim Point1 As New PointF
            Dim Point2 As New PointF

            Dim DrawPoint1 As Boolean = False 'Variable that can be changed to False to prevent drawing the line
            Dim DrawPoint2 As Boolean = False 'Variable that can be changed to False to prevent drawing the line

            'Using the unmasked value of point 1
            If UnmaskedFirstPoint.HasValue = True Then
                Point1 = UnmaskedFirstPoint.Location
                DrawPoint1 = True

                'Sets Draw to false if it's a no respone, or overheard response
                If UnmaskedFirstPoint.NoResponse = True Or UnmaskedFirstPoint.Overheard = True Then
                    DrawPoint1 = False
                End If
            End If

            'Overriding the unmasked value of point 1 with a masked value if such exists
            If MaskedFirstPoint.HasValue = True Then
                Point1 = MaskedFirstPoint.Location
                DrawPoint1 = True

                'Sets Draw to false if it's a no respone, or overheard response
                If MaskedFirstPoint.NoResponse = True Or MaskedFirstPoint.Overheard = True Then
                    DrawPoint1 = False
                End If
            End If

            'Using the unmasked value of point 2
            If UnmaskedSecondPoint.HasValue = True Then
                Point2 = UnmaskedSecondPoint.Location
                DrawPoint2 = True

                'Sets Draw to false if it's a no respone, or overheard response
                If UnmaskedSecondPoint.NoResponse = True Or UnmaskedSecondPoint.Overheard = True Then
                    DrawPoint2 = False
                End If
            End If

            'Overriding the unmasked of point 2 value with a masked value if such exists
            If MaskedSecondPoint.HasValue = True Then
                Point2 = MaskedSecondPoint.Location
                DrawPoint2 = True

                'Sets Draw to false if it's a no respone, or overheard response
                If MaskedSecondPoint.NoResponse = True Or MaskedSecondPoint.Overheard = True Then
                    DrawPoint2 = False
                End If
            End If

            If DrawPoint1 = True And DrawPoint2 = True Then
                Dim Y_Adjustment As Single = GetProportionOfPlotHeight(0.01)
                'Draws the lines if HideAudiogramLines is not True
                If HideAudiogramLines = False Then
                    Select Case Side
                        Case Sides.Left
                            'Drawing the left side line just above the centre height
                            e.Graphics.DrawLine(CurrentPen, Point1.X, Point1.Y - Y_Adjustment, Point2.X, Point2.Y - Y_Adjustment)
                        Case Sides.Right
                            'Drawing the left side line just below the centre height
                            e.Graphics.DrawLine(CurrentPen, Point1.X, Point1.Y + Y_Adjustment, Point2.X, Point2.Y + Y_Adjustment)
                    End Select
                End If
            End If

        End Sub

        Private Sub DrawNotHeardArrow(ByRef StartPoint As PointF,
                                  ByRef e As PaintEventArgs,
                                  ByVal Side As Sides,
                                  ByRef BluePen As Pen, ByRef RedPen As Pen,
                                  ByVal ScaleFactor As Single)

            Select Case Side
                Case Sides.Left
                    Dim PointU1 = New PointF(StartPoint.X + CSng(ScaleFactor * 0.5), StartPoint.Y + CSng(ScaleFactor * 0.5))
                    Dim PointU2 = New PointF(PointU1.X, PointU1.Y - CSng(ScaleFactor * 0.4))
                    Dim PointU3 = New PointF(PointU1.X - CSng(ScaleFactor * 0.4), PointU1.Y)

                    e.Graphics.DrawLine(BluePen, StartPoint, PointU1)
                    e.Graphics.DrawLine(BluePen, PointU2, PointU1)
                    e.Graphics.DrawLine(BluePen, PointU3, PointU1)

            'NB. An arrow could also be drawn using Drawing2D.LineCap
            'Dim BlueArrowPen As New Pen(Blue, LineWidth)
            'BlueArrowPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor
            'e.Graphics.DrawLine(BlueArrowPen, StartPoint, PointU1)

                Case Sides.Right
                    Dim PointU1 = New PointF(StartPoint.X - CSng(ScaleFactor * 0.5), StartPoint.Y + CSng(ScaleFactor * 0.5))
                    Dim PointU2 = New PointF(PointU1.X, PointU1.Y - CSng(ScaleFactor * 0.4))
                    Dim PointU3 = New PointF(PointU1.X + CSng(ScaleFactor * 0.4), PointU1.Y)

                    e.Graphics.DrawLine(RedPen, StartPoint, PointU1)
                    e.Graphics.DrawLine(RedPen, PointU2, PointU1)
                    e.Graphics.DrawLine(RedPen, PointU3, PointU1)

            End Select

        End Sub

        Private AudiogramToolTip As New Windows.Forms.ToolTip() With {.AutoPopDelay = 50000, .InitialDelay = 10, .ReshowDelay = 10, .ShowAlways = True}

        Private Sub Audiogram_MouseMove(sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove

            ' Rounding to valid audiogram frequency and stimulus level
            Dim Frequency = Utils.RoundToAudiogramFrequency(CoordinateToXValue(e.X))
            Dim StimulusLevel = Utils.RoundToAudiogramLevel(CoordinateToYValue(e.Y))

            AudiogramToolTip.SetToolTip(Me, Frequency & " Hz: " & StimulusLevel & " dB")

        End Sub

#Region "Audiogram editing"


        Private Enum WriteModes
            Write
            Remove
        End Enum

        Private Enum PointTypes
            RightAir
            RightBone
            RightMaskedAir
            RightMaskedBone
            LeftAir
            LeftBone
            LeftMaskedAir
            LeftMaskedBone
        End Enum

        Private EnableEditing As Boolean
        Private CurrentPointType As PointTypes
        Private CurrentOverheard As Boolean
        Private CurrentWriteNotHeard As Boolean
        Private MyAudiogramSymbolDialog = New AudiogramSymbolDialog

        Private Sub Audiogram_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick

            Select Case e.Button
                Case MouseButtons.Left

                    If EnableEditing = False Then Exit Sub

                    ' Rounding to valid audiogram frequency and stimulus level
                    Dim Frequency = Utils.RoundToAudiogramFrequency(CoordinateToXValue(e.X))
                    Dim StimulusLevel = Utils.RoundToAudiogramLevel(CoordinateToYValue(e.Y))

                    'Setting the value
                    EditAudiogramValue(CurrentPointType, Frequency, StimulusLevel, CurrentOverheard, CurrentWriteNotHeard)

                    Me.Invalidate()
                    Me.Update()

                Case MouseButtons.Right

                    MyAudiogramSymbolDialog.Location = e.Location
                    Dim DialogResult = MyAudiogramSymbolDialog.ShowDialog()
                    If DialogResult = DialogResult.OK Then

                        EnableEditing = MyAudiogramSymbolDialog.EditEnabled
                        CurrentOverheard = MyAudiogramSymbolDialog.Overheard
                        CurrentWriteNotHeard = MyAudiogramSymbolDialog.NotHeard

                        If MyAudiogramSymbolDialog.Masked = False Then
                            'Unmasked
                            If MyAudiogramSymbolDialog.AirConduction = True Then
                                'Air
                                If MyAudiogramSymbolDialog.LeftSide = True Then
                                    'Left side
                                    CurrentPointType = PointTypes.LeftAir
                                Else
                                    'Right side
                                    CurrentPointType = PointTypes.RightAir
                                End If
                            Else
                                'Bone
                                If MyAudiogramSymbolDialog.LeftSide = True Then
                                    'Left side
                                    CurrentPointType = PointTypes.LeftBone
                                Else
                                    'Right side
                                    CurrentPointType = PointTypes.RightBone
                                End If
                            End If
                        Else
                            'Masked
                            If MyAudiogramSymbolDialog.AirConduction = True Then
                                'Air
                                If MyAudiogramSymbolDialog.LeftSide = True Then
                                    'Left side
                                    CurrentPointType = PointTypes.LeftMaskedAir
                                Else
                                    'Right side
                                    CurrentPointType = PointTypes.RightMaskedAir
                                End If
                            Else
                                'Bone
                                If MyAudiogramSymbolDialog.LeftSide = True Then
                                    'Left side
                                    CurrentPointType = PointTypes.LeftMaskedBone
                                Else
                                    'Right side
                                    CurrentPointType = PointTypes.RightMaskedBone
                                End If
                            End If
                        End If
                    Else
                        EnableEditing = False
                    End If

            End Select

        End Sub

        Private Sub EditAudiogramValue(ByVal PointType As PointTypes, ByVal Frequency As Integer, ByVal StimulusLevel As Integer, Optional ByVal Overheard As Boolean = False, Optional ByVal NoResponse As Boolean = False)

            'Creating a new instance of audiogram data if none exists
            If _AudiogramData Is Nothing Then _AudiogramData = New AudiogramData

            'Finding the right array to change
            Dim ModArray As New List(Of AudiogramData.TonePoint)
            Select Case PointType
                Case PointTypes.RightAir
                    ModArray = _AudiogramData.AC_Right
                Case PointTypes.RightBone
                    ModArray = _AudiogramData.BC_Right
                Case PointTypes.RightMaskedAir
                    ModArray = _AudiogramData.AC_Right_Masked
                Case PointTypes.RightMaskedBone
                    ModArray = _AudiogramData.BC_Right_Masked
                Case PointTypes.LeftAir
                    ModArray = _AudiogramData.AC_Left
                Case PointTypes.LeftBone
                    ModArray = _AudiogramData.BC_Left
                Case PointTypes.LeftMaskedAir
                    ModArray = _AudiogramData.AC_Left_Masked
                Case PointTypes.LeftMaskedBone
                    ModArray = _AudiogramData.BC_Left_Masked
            End Select

            'Putting points into a SortedList to finding the right frequency
            Dim ModTonePoint As AudiogramData.TonePoint
            Dim FrequencyList As New SortedList(Of Integer, AudiogramData.TonePoint)
            For Each tp In ModArray
                FrequencyList.Add(tp.StimulusFrequency, tp)
            Next

            'Modifying the value
            If FrequencyList.ContainsKey(Frequency) = False Then
                ModTonePoint = New AudiogramData.TonePoint With {.StimulusFrequency = Frequency, .Overheard = Overheard, .NoResponse = NoResponse}
                FrequencyList.Add(ModTonePoint.StimulusFrequency, ModTonePoint)
                FrequencyList(Frequency).StimulusLevel = StimulusLevel
            Else
                'Contains the frequency. If the new StimulusLevel value is the same as the old (the user clicked an existing point), the point is removed, otherwise it's changed
                If FrequencyList(Frequency).StimulusLevel <> StimulusLevel Then
                    FrequencyList(Frequency).StimulusLevel = StimulusLevel
                Else
                    FrequencyList.Remove(Frequency)
                End If
            End If

            'Storing the value (overwriting the ModArray, as the ModPoint may have been inserted or removed, via FrequencyList)
            ModArray.Clear()
            For Each Point In FrequencyList.Values
                ModArray.Add(Point)
            Next

        End Sub

#End Region


    End Class


End Namespace
