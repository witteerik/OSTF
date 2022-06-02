Public Class CustomVariableSelectionControl

    'Public ParentControl As Windows.Forms.Control
    Public Property OriginalVariableName As String = ""

    Public Property NewVariableName As String = ""

    Public Property DefaultTextColor As Drawing.Color

    'Public Sub New()

    '    'Stores the default color
    '    DefaultTextColor = Me.ForeColor

    'End Sub

    Public Function GetUpdatedVariableName() As String

        If NewVariableName <> "" Then
            Return NewVariableName
        Else
            Return OriginalVariableName
        End If

    End Function

    Private Sub RenameTo_TextBox_TextChanged(sender As Object, e As EventArgs) Handles RenameTo_TextBox.TextChanged

        Dim NewNameIsValid As Boolean = True

        If RenameTo_TextBox.Text.Trim = "" Then NewNameIsValid = False
        If ValidateNewVariableName(RenameTo_TextBox.Text) = False Then NewNameIsValid = False

        If NewNameIsValid = True Then
            NewVariableName = RenameTo_TextBox.Text
            RenameTo_TextBox.ForeColor = DefaultTextColor
        Else
            NewVariableName = ""
            RenameTo_TextBox.ForeColor = System.Drawing.Color.Red
        End If

    End Sub

    ''' <summary>
    ''' Checks that no other sibling control has the same name
    ''' </summary>
    ''' <param name="NewVariableName"></param>
    ''' <returns>Returns true if the new variable name is free to use</returns>
    Private Function ValidateNewVariableName(ByVal NewVariableName As String)

        'Returns true as there is nothing to compare to
        If Parent Is Nothing Then Return True

        Dim SiblingControls As New List(Of CustomVariableSelectionControl)
        For Each c In Parent.Controls

            Dim CastControl = TryCast(c, CustomVariableSelectionControl)
            If CastControl IsNot Nothing Then
                If CastControl.GetUpdatedVariableName = NewVariableName Then Return False
            End If
        Next

        Return True

    End Function

End Class
