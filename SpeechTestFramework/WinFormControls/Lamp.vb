Imports System.Windows.Forms
Imports System.Drawing

Public Class Lamp
    Inherits Control

    Private ColorOn As Brush = Brushes.LightGreen
    Private ColorOnBorderColor As Pen = New Pen(Color.Lime)
    Private ColorOff As Brush = Brushes.Red
    Private ColorOffBorderColor As Pen = New Pen(Color.Pink)
    Private ColorDisabled As Brush = Brushes.LightGray
    Private ColorDisabledBorderColor As Pen = New Pen(Color.FromArgb(219, 219, 219))


    Private _State As States = States.Off

    Public Property State As States
        Get
            Return _State
        End Get
        Set(value As States)
            _State = value
            Me.Invalidate()
            Me.Update()
        End Set
    End Property

    Public Enum States
        [On]
        Off
        Disabled
    End Enum

    Private _Shape As Shapes = Shapes.Circle

    Public Property Shape As Shapes
        Get
            Return _Shape
        End Get
        Set(value As Shapes)
            _Shape = value
            Me.Invalidate()
            Me.Update()
        End Set
    End Property

    Public Enum Shapes
        Square
        Rectangle
        Circle
        Ellipse
    End Enum

    Private _ShapeSize As Single = 0.8
    Public Property ShapeSize As Single
        Get
            Return _ShapeSize
        End Get
        Set(value As Single)
            _ShapeSize = value
            Me.Invalidate()
            Me.Update()
        End Set
    End Property


    Public Sub New()

        MyBase.New

        Me.AutoSize = False

    End Sub

    Private Shadows Sub EnabledChanged() Handles MyBase.EnabledChanged
        If Me.Enabled = False Then Me.State = States.Disabled
    End Sub

    Public Sub SetCustomColors(ByVal ColorOn As Brush, ByVal ColorOnBorderColor As Pen, ByVal ColorOff As Brush, ByVal ColorOffBorderColor As Pen, ByVal ColorDisabled As Brush, ByVal ColorDisabledBorderColor As Pen)
        Me.ColorOn = ColorOn
        Me.ColorOnBorderColor = ColorOnBorderColor
        Me.ColorOff = ColorOff
        Me.ColorOffBorderColor = ColorOffBorderColor
        Me.ColorDisabled = ColorDisabled
        Me.ColorDisabledBorderColor = ColorDisabledBorderColor
        Me.Invalidate()
        Me.Update()
    End Sub

    Private Sub Me_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

        Dim ParentContainerBackColor As Color = Drawing.Color.FromArgb(0, 0, 0)
        If Me.Parent IsNot Nothing Then
            ParentContainerBackColor = Parent.BackColor
        End If

        Dim ShapeHeight As Single = _ShapeSize * ClientRectangle.Height
        Dim ShapeWidth As Single
        Select Case Shape
            Case Shapes.Rectangle, Shapes.Ellipse
                ShapeWidth = _ShapeSize * ClientRectangle.Width

            Case Shapes.Square, Shapes.Circle
                ShapeWidth = ShapeHeight

        End Select

        Dim ShapeTop As Single = ClientRectangle.Height / 2 - ShapeHeight / 2
        Dim ShapeLeft As Single = ClientRectangle.Width / 2 - ShapeWidth / 2

        Dim ShapeRectangle As New Rectangle(ShapeLeft, ShapeTop, ShapeWidth, ShapeHeight)

        Dim FillColor As Brush = ColorDisabled
        Dim BorderColor As Pen = New Pen(ColorDisabledBorderColor.Color, 2 * _ShapeSize)

        Select Case _State
            Case States.On
                FillColor = ColorOn
                BorderColor = ColorOnBorderColor
            Case States.Off
                FillColor = ColorOff
                BorderColor = ColorOffBorderColor
            Case States.Disabled
                FillColor = ColorDisabled
                BorderColor = ColorDisabledBorderColor
        End Select

        Select Case _Shape
            Case Shapes.Rectangle, Shapes.Square
                e.Graphics.FillRectangle(FillColor, ShapeRectangle)
                e.Graphics.DrawRectangle(BorderColor, ShapeRectangle)
            Case Shapes.Ellipse, Shapes.Circle
                e.Graphics.FillEllipse(FillColor, ShapeRectangle)
                e.Graphics.DrawEllipse(BorderColor, ShapeRectangle)
        End Select

    End Sub

End Class

