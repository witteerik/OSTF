Imports System.Windows.Forms

''' <summary>
''' A TextBox that can be used to input file names, and checks input strings for file name validity. Red text color indicates invalid value, and default color indicates valid value. 
''' Valid filenames should be retrieved from the property SelectedFileName.
''' </summary>
Public Class FileNameTextBox
    Inherits TextBox

    Private _SelectedFileName As String = String.Empty
    Public ReadOnly Property SelectedFileName As String
        Get
            Return _SelectedFileName
        End Get
    End Property

    Private DefaultTextColor As Drawing.Color

    Public Sub New()

        'Stores the default color
        DefaultTextColor = Me.ForeColor

    End Sub

    Protected Overrides Sub OnTextChanged(e As EventArgs)
        MyBase.OnTextChanged(e)


        Dim AllOk As Boolean = True
        Dim TrimmedInput As String = Me.Text.Trim

        Dim FileName As String = IO.Path.GetFileName(TrimmedInput)
        If String.IsNullOrEmpty(FileName) = True Then
            AllOk = False
        End If

        If AllOk = True Then
            'Checks for invalid characters in the file name
            Dim InvalidChars() As Char = IO.Path.GetInvalidFileNameChars
            For Each c In InvalidChars
                If FileName.Contains(c) Then
                    AllOk = False
                    Exit For
                End If
            Next
        End If

        'Finally adjusts the color and selected path values
        If AllOk = True Then
            Me.ForeColor = DefaultTextColor
            Me._SelectedFileName = TrimmedInput
        Else
            Me.ForeColor = Drawing.Color.Red
            Me._SelectedFileName = String.Empty
        End If

    End Sub

End Class