Imports System.Windows.Forms
Imports System.Drawing

Public Class ResponseGuiItem
    Inherits Control

    Private WithEvents ContentHolder As New Panel

    Private WithEvents ContentLabel As ItemContentLabel

    ''' <summary>
    ''' Holds a string represention the response issued by selecting this control.
    ''' </summary>
    ''' <returns></returns>
    Public Property ResponseString As String

    Public Event ResponseGiven(ByVal ResponseString As String)

    Public ReadOnly Property SubmitsResponse As Boolean

    Public ReadOnly Property Selectable As Boolean
    Public Property SelectedColor As Color
    Public Property NonSelectedColor As Color

    Public ReadOnly Property SelectionFrameThickness As Integer
        Get
            Return ContentHolder.Margin.All
        End Get
    End Property

    Private _Selected As Boolean = False
    Public ReadOnly Property Selected As Boolean
        Get
            Return _Selected
        End Get
    End Property


    Public Sub New()

        Me.New(False, True, Drawing.Color.LightGreen, Drawing.Color.LightGray)

    End Sub

    Public Sub New(ByVal Selectable As Boolean,
                   ByVal SubmitsResponse As Boolean,
                   ByRef SelectedColor As Color,
                   ByRef NonSelectedColor As Color,
                   Optional ByVal SelectionFrameThickness As Integer = 10)

        'Setting Selectable (which is readonly)
        Me.Selectable = Selectable

        'Setting SubmitsResponse (which is readonly)
        Me.SubmitsResponse = SubmitsResponse

        'Setting the 
        If Me.Selectable = True Then
            ContentHolder.Padding = New Padding(SelectionFrameThickness)
            Me.SelectedColor = SelectedColor
            Me.NonSelectedColor = NonSelectedColor

            'Updating selection colors
            SetSelected()

        Else
            ContentHolder.Margin = New Padding(0)
        End If

        'Adding the ContentControl
        ContentHolder.Dock = DockStyle.Fill
        Me.Controls.Add(ContentHolder)

        'Setting the images shown in ContentControl to Zoom layout
        ContentHolder.BackgroundImageLayout = ImageLayout.Zoom

        ContentLabel = New ItemContentLabel(Me)
        ContentHolder.Controls.Add(ContentLabel)
        ContentLabel.Dock = DockStyle.Fill

    End Sub


    Private Sub Me_Click(sender As Object, e As EventArgs) Handles Me.Click, Me.DoubleClick, ContentHolder.Click, ContentHolder.DoubleClick,
        ContentLabel.Click, ContentLabel.DoubleClick

        If Me.Selectable = True Then
            _Selected = Not _Selected
            SetSelected()
        End If

        If Me.SubmitsResponse = True Then
            RaiseEvent ResponseGiven(ResponseString)
        End If

    End Sub

    Private Sub SetSelected()

        If Me.Selectable = True Then
            If _Selected = True Then
                Me.BackColor = SelectedColor
            Else
                Me.BackColor = NonSelectedColor
            End If

            Me.Update()

            If ContentLabel IsNot Nothing Then
                ContentLabel.Invalidate()
            End If
        End If

    End Sub

    Public Function HasText() As Boolean

        If ContentLabel Is Nothing Then Return False
        If ContentLabel.Text = "" Then Return False
        Return True

    End Function

    Public Overrides Property Text As String
        Get
            If ContentLabel IsNot Nothing Then
                Return ContentLabel.Text
            Else
                Return MyBase.Text
            End If
        End Get
        Set(value As String)
            If ContentLabel IsNot Nothing Then
                ContentLabel.Text = value
            Else
                MyBase.Text = value
            End If
        End Set
    End Property

    Public Overrides Property Font As Font
        Get
            If ContentLabel IsNot Nothing Then
                Return ContentLabel.Font
            Else
                Return MyBase.Font
            End If

        End Get
        Set(value As Font)
            If ContentLabel IsNot Nothing Then
                ContentLabel.Font = value
            Else
                MyBase.Font = value
            End If
        End Set
    End Property

    Public Overrides Property BackgroundImage As Image
        Get
            If ContentLabel IsNot Nothing Then
                Return ContentLabel.BackgroundImage
            Else
                Return MyBase.BackgroundImage
            End If
        End Get
        Set(value As Image)
            If ContentLabel IsNot Nothing Then
                ContentLabel.BackgroundImage = value
            Else
                MyBase.BackgroundImage = value
            End If
        End Set
    End Property

End Class

Public Class ItemContentLabel
    Inherits Label

    Public Property PaddedImage As Bitmap = Nothing

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


    Private Sub ItemContentLabel_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

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

