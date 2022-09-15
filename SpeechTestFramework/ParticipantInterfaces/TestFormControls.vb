Imports System.Windows.Forms
Imports System.Drawing

Public Class TestWordLabel
    Inherits Label

    Public Sub New(ByRef ParentContainer As Object, Optional BackColorRed As Integer = 255, Optional BackColorGreen As Integer = 255, Optional BackColorBlue As Integer = 90, Optional ByVal FontSize As Single = 60)

        Me.Parent = ParentContainer
        Me.Font = New Font("Verdana", FontSize, FontStyle.Regular)
        Me.TextAlign = ContentAlignment.MiddleCenter
        Me.BackColor = Drawing.Color.FromArgb(BackColorRed, BackColorGreen, BackColorBlue)
        Me.AutoSize = False

    End Sub

    Public Sub New(ByRef ParentContainer As Object, ByVal BackColor As Color, Optional ByVal FontSize As Single = 60)

        Me.Parent = ParentContainer
        Me.Font = New Font("Verdana", FontSize, FontStyle.Regular)
        Me.TextAlign = ContentAlignment.MiddleCenter
        Me.BackColor = BackColor
        Me.AutoSize = False

    End Sub


    Private Sub TestWordLabel_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

        Dim ParentContainerBackColor As Color = Drawing.Color.FromArgb(0, 0, 0)
        If Me.Parent IsNot Nothing Then
            ParentContainerBackColor = Parent.BackColor
        End If

        'Drawing round edges
        Dim Radius As Single = 25
        Dim LineWidth As Single = 3
        e.Graphics.DrawArc(New Pen(ParentContainerBackColor, LineWidth), -LineWidth, -LineWidth, Radius, Radius, 180, 90)
        e.Graphics.DrawArc(New Pen(ParentContainerBackColor, LineWidth), ClientRectangle.Width - 1 - Radius + LineWidth, -LineWidth, Radius, Radius, 270, 90)
        e.Graphics.DrawArc(New Pen(ParentContainerBackColor, LineWidth), ClientRectangle.Width - 1 - Radius + LineWidth, ClientRectangle.Height - 1 - Radius + LineWidth, Radius, Radius, 0, 90)
        e.Graphics.DrawArc(New Pen(ParentContainerBackColor, LineWidth), -LineWidth, ClientRectangle.Height - 1 - Radius + LineWidth, Radius, Radius, 90, 90)

    End Sub


End Class


Public Class ProgressBar_Old
    Inherits Panel

    Private WithEvents ProgressPanel As New Panel

    ''' <summary>
    ''' If set, the progress bar will only change within StepCount descrete steps.
    ''' </summary>
    ''' <returns></returns>
    Public Property StepCount As Integer? = Nothing
    Public Property Minimum As Double
    Public Property Maximum As Double

    Private _Value As Double
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="PushBoundaries">If PushBoundaries is True, a new value outside the minimum or maximum will adjust the minimum or maximum values to include the new value.</param>
    ''' <returns></returns>
    Public Property Value(Optional ByVal PushBoundaries As Boolean = True) As Double
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
                Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
                Return Nothing
            End Try

        End Get
        Set(newValue As Double)

            Try

                'Adjusting boundaries
                If newValue > Maximum Then Maximum = newValue
                If newValue < Minimum Then Minimum = newValue

                'setting Value
                _Value = newValue

                'Updating the progress panel
                SetProgressPanelWidth()
            Catch ex As Exception
                Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
            End Try

        End Set
    End Property



    Public Sub New(ByRef ParentContainer As Object, ByVal ProgressColor As Color, ByVal BackColor As Color)

        Dim ParentContainerBackColor As Color = Drawing.Color.FromArgb(255, 255, 90)
        If Me.Parent IsNot Nothing Then
            ParentContainerBackColor = Parent.BackColor
        End If

        Me.Parent = ParentContainer
        Me.BackColor = BackColor
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

    End Sub

    Private Sub ProgressPanel_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles ProgressPanel.Paint

        Dim ParentContainerBackColor As Color = Drawing.Color.FromArgb(0, 0, 0)
        If ProgressPanel.Parent IsNot Nothing Then
            ParentContainerBackColor = Parent.BackColor
        End If

        'Drawing round edges
        Dim Radius As Single = 15
        Dim LineWidth As Single = 2
        e.Graphics.DrawArc(New Pen(ParentContainerBackColor, LineWidth), -LineWidth, -LineWidth, Radius, Radius, 180, 90)
        e.Graphics.DrawArc(New Pen(ParentContainerBackColor, LineWidth), ProgressPanel.ClientRectangle.Width - 1 - Radius + LineWidth, -LineWidth, Radius, Radius, 270, 90)
        e.Graphics.DrawArc(New Pen(ParentContainerBackColor, LineWidth), ProgressPanel.ClientRectangle.Width - 1 - Radius + LineWidth, ProgressPanel.ClientRectangle.Height - 1 - Radius + LineWidth, Radius, Radius, 0, 90)
        e.Graphics.DrawArc(New Pen(ParentContainerBackColor, LineWidth), -LineWidth, ProgressPanel.ClientRectangle.Height - 1 - Radius + LineWidth, Radius, Radius, 90, 90)

    End Sub


End Class