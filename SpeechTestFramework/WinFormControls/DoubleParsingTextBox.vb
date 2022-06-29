Imports System.Windows.Forms
Imports System.Globalization

''' <summary>
''' Gets a Double value from user input text. Red text color indicates invalid value, and default color indicates valid value. 
''' Valid values should be retrieved from the property Value. (The control allows BOTH comma and dot as decimal separator.)
''' </summary>
Public Class DoubleParsingTextBox
    Inherits TextBox

    Private _Value As Double? = Nothing
    Public ReadOnly Property Value As Double?
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

        If Double.TryParse(Me.Text.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, ParsedValue) = True Then
            Me._Value = ParsedValue
            Me.ForeColor = DefaultTextColor
        Else
            Me._Value = Nothing
            Me.ForeColor = Drawing.Color.Red
        End If

        RaiseEvent ValueUpdated()

    End Sub

End Class