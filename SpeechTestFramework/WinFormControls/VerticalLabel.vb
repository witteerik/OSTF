Imports System.Windows.Forms
Imports System.Drawing

Namespace WinFormControls

    <Serializable>
    Public Class VerticalLabel
        Inherits Label

        Public Sub New()

            MyBase.New

            Me.AutoSize = False

        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)

            Dim b = New SolidBrush(Me.ForeColor)

            Dim StrFormat As New StringFormat(StringFormatFlags.DirectionVertical)
            StrFormat.LineAlignment = StringAlignment.Center
            StrFormat.Alignment = StringAlignment.Center

            e.Graphics.DrawString(Me.Text, Me.Font, b, Width / 2, Height / 2, StrFormat)

        End Sub

    End Class

End Namespace
