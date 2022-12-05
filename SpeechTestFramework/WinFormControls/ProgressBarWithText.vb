Imports System.Windows.Forms
Imports System.Drawing

Public Class ProgressBarWithText
    Inherits System.Windows.Forms.ProgressBar

    Private BlackBrush As Brush = Brushes.Black
    Private GrayBrush As Brush = Brushes.LightGray

    Private MyStringFormat As StringFormat

    Public Property TextFont As Font

    Private _TextMode As TextModes = TextModes.Progress
    Public Property TextMode As TextModes
        Get
            Return _TextMode
        End Get
        Set(value As TextModes)
            _TextMode = value
            Me.Invalidate()
            Me.Update()
        End Set
    End Property

    Public Enum TextModes
        None
        Progress
        CustomText
    End Enum

    Private _CustomText As String = ""
    Public Property CustomText As String
        Get
            Return _CustomText
        End Get
        Set(value As String)
            _CustomText = value
            Me.Invalidate()
            Me.Update()
        End Set
    End Property

    Public Sub New()

        MyStringFormat = New StringFormat
        MyStringFormat.Alignment = StringAlignment.Center
        MyStringFormat.LineAlignment = StringAlignment.Center

        SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)

        TextFont = New Font(Me.Font, FontStyle.Bold)

    End Sub

    Public Sub DrawText(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

        ProgressBarRenderer.DrawHorizontalBar(e.Graphics, Me.ClientRectangle)

        If Me.Value > 0 Then
            Dim ValueRect = New Rectangle With {.X = Me.ClientRectangle.X, .Y = Me.ClientRectangle.Y, .Height = Me.ClientRectangle.Height, .Width = Me.ClientRectangle.Width * (Me.Value / Me.Maximum)}
            'ProgressBarRenderer.DrawHorizontalChunks(e.Graphics, ValueRect)

            Dim MyBrush As Drawing2D.LinearGradientBrush = New Drawing2D.LinearGradientBrush(ValueRect, Color.LightGreen, Color.Green, Drawing2D.LinearGradientMode.Vertical)
            e.Graphics.FillRectangle(MyBrush, ValueRect)
        End If

        Select Case TextMode
            Case TextModes.None
                'Draws no text
            Case TextModes.Progress
                If Me.Value <= Me.Maximum And Me.Value >= Me.Minimum Then
                    If Me.Enabled = True Then
                        e.Graphics.DrawString(Me.Value & " / " & Me.Maximum, TextFont, BlackBrush, Me.ClientRectangle, MyStringFormat)
                    Else
                        e.Graphics.DrawString(Me.Value & " / " & Me.Maximum, TextFont, GrayBrush, Me.ClientRectangle, MyStringFormat)
                    End If
                End If
            Case TextModes.CustomText
                If Me.Enabled = True Then
                    e.Graphics.DrawString(Me._CustomText, TextFont, BlackBrush, Me.ClientRectangle, MyStringFormat)
                Else
                    e.Graphics.DrawString(Me._CustomText, TextFont, GrayBrush, Me.ClientRectangle, MyStringFormat)
                End If
        End Select

    End Sub

End Class