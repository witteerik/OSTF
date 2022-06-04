Public Class CustomVariableSelectControl

    Private _VariableName As String = ""
    Public Property VariableName As String
        Get
            Return _VariableName
        End Get
        Set(value As String)
            _VariableName = value

            'Also sets the text in the VariabelName_CheckBox
            VariabelName_CheckBox.Text = _VariableName
        End Set
    End Property

    Public Function IsSelected() As Boolean
        Return VariabelName_CheckBox.Checked
    End Function


    Private _IsNumericVariable As Boolean = True
    Public Property IsNumericVariable As Boolean
        Get
            Return _IsNumericVariable
        End Get
        Set(value As Boolean)
            _IsNumericVariable = value

            If _IsNumericVariable = True Then
                NumericSummaryMethodsBox.Visible = True
                CategorialSummaryMethodsBox.Visible = False
            Else
                NumericSummaryMethodsBox.Visible = False
                CategorialSummaryMethodsBox.Visible = True
            End If

        End Set
    End Property

    Private Sub CustomVariableSelectControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        For Each c As Windows.Forms.CheckBox In NumericSummaryMethodsBox.Controls
            AddHandler c.CheckedChanged, AddressOf NumericSummaryMethodsBox_CheckedChanged
        Next

        For Each c As Windows.Forms.CheckBox In CategorialSummaryMethodsBox.Controls
            AddHandler c.CheckedChanged, AddressOf CategorialSummaryMethodsBox_CheckedChanged
        Next

    End Sub


    Private Sub VariabelName_CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles VariabelName_CheckBox.CheckedChanged

        'Selecting a default CheckBox
        If VariabelName_CheckBox.Checked = True Then
            If IsNumericVariable = True Then
                'Checking the ArithmeticMean_CheckBox if no other checkbox is selected
                If IsAnyNumericCheckBoxChecked() = False Then ArithmeticMean_CheckBox.Checked = True
            Else
                'Checking the Mode_CheckBox if no other checkbox is selected
                If IsAnyCategoricalCheckBoxChecked() = False Then Mode_CheckBox.Checked = True
            End If
        End If

    End Sub

    Private Function IsAnyNumericCheckBoxChecked() As Boolean
        For Each c As Windows.Forms.CheckBox In NumericSummaryMethodsBox.Controls
            If c.Checked = True Then Return True
        Next
        Return False
    End Function

    Private Function IsAnyCategoricalCheckBoxChecked() As Boolean
        For Each c As Windows.Forms.CheckBox In CategorialSummaryMethodsBox.Controls
            If c.Checked = True Then Return True
        Next
        Return False
    End Function

    Private Sub CategorialSummaryMethodsBox_CheckedChanged()
        'De-selecting the variable, if no checkboxes are checked, and selecting it if at least one is
        VariabelName_CheckBox.Checked = IsAnyCategoricalCheckBoxChecked()
    End Sub

    Private Sub NumericSummaryMethodsBox_CheckedChanged()
        'De-selecting the variable, if no checkboxes are checked, and selecting it if at least one is
        VariabelName_CheckBox.Checked = IsAnyNumericCheckBoxChecked()

    End Sub


End Class
