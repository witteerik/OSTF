Public Class CustomVariableSelectionControl


    Private _OriginalVariableName As String = ""

    Public Property OriginalVariableName As String
        Get
            Return _OriginalVariableName
        End Get
        Set(value As String)
            _OriginalVariableName = value

            'Also sets the text in the OriginalVariabelName_CheckBox
            OriginalVariabelName_CheckBox.Text = _OriginalVariableName
        End Set
    End Property

    Public Function IsSelected() As Boolean
        Return OriginalVariabelName_CheckBox.Checked
    End Function

    Public Property NewVariableName As String = ""

    Public Function GetUpdatedVariableName() As String

        If NewVariableName <> "" Then
            Return NewVariableName
        Else
            Return OriginalVariableName
        End If

    End Function

    Public IsNumericVariable As Boolean

    Private Sub RenameTo_TextBox_TextChanged(sender As Object, e As EventArgs) Handles RenameTo_TextBox.TextChanged

        Dim NewNameIsValid As Boolean = True

        If RenameTo_TextBox.Text.Trim = "" Then NewNameIsValid = False
        If ValidateNewVariableName(RenameTo_TextBox.Text) = False Then NewNameIsValid = False

        If NewNameIsValid = True Then
            NewVariableName = RenameTo_TextBox.Text

            If Me.Parent IsNot Nothing Then
                RenameTo_TextBox.ForeColor = Me.Parent.ForeColor
            Else
                RenameTo_TextBox.ForeColor = Drawing.Color.Black
            End If
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
