Imports System.Windows.Forms
Imports System.Drawing

Public Class ProgressBar
    Inherits Panel

    Private WithEvents ProgressPanel As New Panel

    Public Overrides Property BackColor As Color = Color.Black

    Public Property ProgressColor As Color = Color.Yellow

    ''' <summary>
    ''' If set, the progress bar will only change within StepCount descrete steps.
    ''' </summary>
    ''' <returns></returns>
    Public Property StepCount As Integer? = Nothing
    Public Property Minimum As Double
    Public Property Maximum As Double

    Private _Value As Double

    Public Property Value As Double
        Get

            Try

                If StepCount Is Nothing Then
                    Return _Value
                Else
                    Dim StepSize As Integer = (Maximum - Minimum) / StepCount
                    Dim CurrentStep As Integer = 0
                    If StepSize > 0 Then
                        CurrentStep = Int(_Value / StepSize)
                    End If
                    Return CurrentStep * StepSize
                End If

            Catch ex As Exception
                'SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
                Return Nothing
            End Try

        End Get
        Set(value As Double)

            Try

                'Adjusting boundaries
                If value > Maximum Then Maximum = value
                If value < Minimum Then Minimum = value

                'setting Value
                _Value = value

                'Updating the progress panel
                SetProgressPanelWidth()
            Catch ex As Exception
                'SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
            End Try

        End Set

    End Property


    Public Sub New()

        Height = 15

        Me.ProgressPanel.BackColor = ProgressColor

        Me.Controls.Add(ProgressPanel)

        SetSize()

    End Sub

    Public Sub New(ByVal BackColor As Color, ByVal ProgressColor As Color)

        Me.BackColor = BackColor
        Me.ProgressPanel.BackColor = ProgressColor

        Height = 15

        Me.Controls.Add(ProgressPanel)

        SetSize()

    End Sub

    Public Sub New(ByRef ParentContainer As Control, ByVal ProgressColor As Color)

        Me.Parent = ParentContainer

        If Me.Parent Is Nothing Then
            Me.BackColor = Drawing.Color.FromArgb(255, 255, 90)
        Else
            Me.BackColor = Parent.BackColor
        End If

        Me.ProgressPanel.BackColor = ProgressColor

        Height = 15

        Me.Controls.Add(ProgressPanel)

        SetSize()

    End Sub


    Private Function GetPadding() As Single
        Return Me.Height / 8
    End Function

    Private Function GetProgressRatio() As Single
        Dim ProgressRatio As Single = 0
        Dim Range As Single = Maximum - Minimum
        If Range > 0 Then
            ProgressRatio = Math.Min(Math.Max(0, Value / Range), 1)
        End If
        Return ProgressRatio
    End Function

    Private Sub SetSize() Handles Me.SizeChanged

        'Setting the location and size of the progres panel
        Dim Padding As Single = GetPadding()

        ProgressPanel.Height = Me.Height - 2 * Padding
        ProgressPanel.Top = Padding
        ProgressPanel.Left = Padding

        SetProgressPanelWidth()

    End Sub

    ''' <summary>
    ''' Updates the width of the progress panel, using the Value property to display current progress.
    ''' </summary>
    Private Sub SetProgressPanelWidth()

        Dim ProgressWidth As Integer = Math.Min(Math.Max(0, GetProgressRatio() * (Me.Width - (2 * GetPadding()))), Me.Width)

        ProgressPanel.Width = ProgressWidth

        ProgressPanel.Invalidate()
        ProgressPanel.Update()

    End Sub


    Private Sub ProgressPanel_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles ProgressPanel.Paint

        'Drawing round edges
        Dim Radius As Single = 15
        Dim LineWidth As Single = 2
        e.Graphics.DrawArc(New Pen(BackColor, LineWidth), -LineWidth, -LineWidth, Radius, Radius, 180, 90)
        e.Graphics.DrawArc(New Pen(BackColor, LineWidth), ProgressPanel.ClientRectangle.Width - 1 - Radius + LineWidth, -LineWidth, Radius, Radius, 270, 90)
        e.Graphics.DrawArc(New Pen(BackColor, LineWidth), ProgressPanel.ClientRectangle.Width - 1 - Radius + LineWidth, ProgressPanel.ClientRectangle.Height - 1 - Radius + LineWidth, Radius, Radius, 0, 90)
        e.Graphics.DrawArc(New Pen(BackColor, LineWidth), -LineWidth, ProgressPanel.ClientRectangle.Height - 1 - Radius + LineWidth, Radius, Radius, 90, 90)

    End Sub


End Class
