Imports System.Windows.Forms
Imports System.Globalization

''' <summary>
''' Gets an integer value from user input text. Red text color indicates invalid value, and default color indicates valid value. 
''' Valid values should be retrieved from the property Value.
''' </summary>
Public Class IntegerParsingTextBox
    Inherits TextBox

    Private _Value As Integer? = Nothing
    Public ReadOnly Property Value As Integer?
        Get
            Return _Value
        End Get
    End Property

    Private DefaultTextColor As Drawing.Color

    Public Event ValueUpdated()

    Public Sub New()

        'Stores the default color
        DefaultTextColor = Me.ForeColor

    End Sub

    Protected Overrides Sub OnTextChanged(e As EventArgs)
        MyBase.OnTextChanged(e)

        'Tries to parse the text as a valid Double
        Dim ParsedValue As Double

        If Double.TryParse(Me.Text.Replace(",", "."), NumberStyles.Integer, CultureInfo.InvariantCulture, ParsedValue) = True Then
            Me._Value = ParsedValue
            Me.ForeColor = DefaultTextColor
        Else
            Me._Value = Nothing
            Me.ForeColor = Drawing.Color.Red
        End If

        RaiseEvent ValueUpdated()

    End Sub

End Class