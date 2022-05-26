Imports System.Windows.Forms

''' <summary>
''' A Windows Forms Textbox that automatically adjusts its height when the text is changed.
''' </summary>
Public Class AutoHeightTextBox
    Inherits TextBox

    Public Sub New()
        MyBase.New

    End Sub

    Protected Overrides Sub OnTextChanged(e As EventArgs)
        MyBase.OnTextChanged(e)

        'Rezising
        Dim PreferredSize = Me.GetPreferredSize(Me.Size)
        Me.Height = PreferredSize.Height

    End Sub

    Protected Overrides Sub OnFontChanged(e As EventArgs)
        MyBase.OnFontChanged(e)

        'Rezising
        Dim PreferredSize = Me.GetPreferredSize(Me.Size)
        Me.Height = PreferredSize.Height

    End Sub


End Class