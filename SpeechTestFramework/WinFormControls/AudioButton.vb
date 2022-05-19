Imports System.Windows.Forms
Imports System.Drawing
Imports System.ComponentModel

Namespace WinFormControls

    <Serializable>
    Public Class AudioButton
        Inherits Button

        Private _ViewMode As ViewModes = ViewModes.Play

        Public Property ViewMode As ViewModes
            Get
                Return _ViewMode
            End Get
            Set(value As ViewModes)
                _ViewMode = value
                Me.Invalidate()
            End Set
        End Property

        Public Enum ViewModes
            Play
            Pause
            [Stop]
            Rec
        End Enum

        Private _Text As String = ""

        ''' <summary>
        ''' The Text property of the base class (Button) is overidden in this class, and always sets the text to an empty string, since no text should be displayed on the control.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Property Text As String
            Get
                Return _Text
            End Get
            Set(value As String)
                value = ""
                _Text = value
            End Set
        End Property

        'Below is an alternate way to hide the text on the control...
        '<DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        'Public Shadows Property Text As String = ""

        Public Sub New()

            MyBase.New

        End Sub


        Private Sub DrawSymbol(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint

            Dim MidX = ClientRectangle.Width / 2
            Dim MidY = ClientRectangle.Height / 2
            Dim PaintQuadrantSide As Single = Math.Min(MidX * 1.25, MidY * 1.25)
            Dim PaintQuadrantX As Single = MidX - (PaintQuadrantSide / 2)
            Dim PaintQuadrantY As Single = MidY - (PaintQuadrantSide / 2)

            Select Case ViewMode
                Case ViewModes.Play

                    Dim PlayBrush As SolidBrush
                    Dim PlayPen As Pen
                    If Enabled = True Then
                        PlayBrush = New SolidBrush(Color.Green)
                        PlayPen = New Pen(Color.DarkGreen)
                    Else
                        PlayBrush = New SolidBrush(Color.LightGray)
                        PlayPen = New Pen(Color.Gray)
                    End If

                    Dim Point1 As New PointF(PaintQuadrantX, PaintQuadrantY)
                    Dim Point2 As New PointF(PaintQuadrantX, PaintQuadrantY + PaintQuadrantSide)
                    Dim Point3 As New PointF(PaintQuadrantX + PaintQuadrantSide, PaintQuadrantY + PaintQuadrantSide / 2)
                    Dim Points As PointF() = {Point1, Point2, Point3}

                    e.Graphics.FillPolygon(PlayBrush, Points)
                    e.Graphics.DrawPolygon(PlayPen, Points)

                Case ViewModes.Pause

                    Dim PauseBrush As SolidBrush
                    Dim PausePen As Pen
                    If Enabled = True Then
                        PauseBrush = New SolidBrush(Color.Blue)
                        PausePen = New Pen(Color.DarkBlue)
                    Else
                        PauseBrush = New SolidBrush(Color.LightGray)
                        PausePen = New Pen(Color.Gray)
                    End If

                    e.Graphics.FillRectangle(PauseBrush, PaintQuadrantX, PaintQuadrantY, PaintQuadrantSide / 3, PaintQuadrantSide)
                    e.Graphics.DrawRectangle(PausePen, PaintQuadrantX, PaintQuadrantY, PaintQuadrantSide / 3, PaintQuadrantSide)

                    e.Graphics.FillRectangle(PauseBrush, CSng(PaintQuadrantX + PaintQuadrantSide * (2 / 3)), PaintQuadrantY, PaintQuadrantSide / 3, PaintQuadrantSide)
                    e.Graphics.DrawRectangle(PausePen, CSng(PaintQuadrantX + PaintQuadrantSide * (2 / 3)), PaintQuadrantY, PaintQuadrantSide / 3, PaintQuadrantSide)


                Case ViewModes.Rec

                    Dim RecBrush As SolidBrush
                    Dim RecPen As Pen
                    If Enabled = True Then
                        RecBrush = New SolidBrush(Color.Red)
                        RecPen = New Pen(Color.DarkRed)
                    Else
                        RecBrush = New SolidBrush(Color.LightGray)
                        RecPen = New Pen(Color.Gray)
                    End If

                    e.Graphics.FillEllipse(RecBrush, PaintQuadrantX, PaintQuadrantY, PaintQuadrantSide, PaintQuadrantSide)
                    e.Graphics.DrawEllipse(RecPen, PaintQuadrantX, PaintQuadrantY, PaintQuadrantSide, PaintQuadrantSide)

                Case ViewModes.Stop

                    Dim StopBrush As SolidBrush
                    Dim StopPen As Pen
                    If Enabled = True Then
                        StopBrush = New SolidBrush(Color.Red)
                        StopPen = New Pen(Color.DarkRed)
                    Else
                        StopBrush = New SolidBrush(Color.LightGray)
                        StopPen = New Pen(Color.Gray)
                    End If

                    e.Graphics.FillRectangle(StopBrush, PaintQuadrantX, PaintQuadrantY, PaintQuadrantSide, PaintQuadrantSide)
                    e.Graphics.DrawRectangle(StopPen, PaintQuadrantX, PaintQuadrantY, PaintQuadrantSide, PaintQuadrantSide)

            End Select

        End Sub

        Private Sub AudioButton_Validated(sender As Object, e As EventArgs) Handles Me.Validated

            Me.Text = ""

        End Sub
    End Class

End Namespace