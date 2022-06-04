Public Class StatisticalSummaryControl

    Private LoadedSpeechMaterial As SpeechMaterialComponent

    Private Sub StatisticalSummaryControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        SourceLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Phoneme)
        SourceLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Word)
        SourceLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Sentence)
        SourceLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.List)

        'Selecting a default level
        SourceLevel_ComboBox.SelectedItem = SpeechMaterialComponent.LinguisticLevels.Word

    End Sub

    Private Sub UpdateControlEnabledStatuses()

        If LoadedSpeechMaterial IsNot Nothing Then
            'LoadDatabase_GroupBox.Enabled = True
        Else
            'LoadDatabase_GroupBox.Enabled = False
        End If


    End Sub


    Private Function ViewSourceLevelVariables() As Boolean

        Variables_TableLayoutPanel.Controls.Clear()
        Variables_TableLayoutPanel.RowCount = 1
        Variables_TableLayoutPanel.ColumnCount = 1

        If LoadedSpeechMaterial Is Nothing Then
            UpdateControlEnabledStatuses()
            MsgBox("No speech material components loaded.", MsgBoxStyle.Information, "Viewing variables")
            Return False
        End If


        If SourceLevel_ComboBox.SelectedItem Is Nothing Then
            UpdateControlEnabledStatuses()
            MsgBox("Select a source linguistic level.", MsgBoxStyle.Information, "Calculate statistics")
            Return False
        End If

        Dim SourceLevel As SpeechMaterialComponent.LinguisticLevels = SourceLevel_ComboBox.SelectedItem

        'Getting all variable names and types at the source linguistic level
        Dim AllCustomVariables As SortedList(Of String, Boolean) = LoadedSpeechMaterial.GetCustomVariableNameAndTypes(SourceLevel) ' Variable name, IsNumeric

        '...


        Variables_TableLayoutPanel.SuspendLayout()

        Dim rnd As New Random(30)

        For v = 0 To LoadedLexicalDatabase.CustomVariableNames.Count - 1

            Dim NewVariableControl As New CustomVariableSelectionControl

            'Setting the variable name in the variable selection control
            Dim VariableName = LoadedLexicalDatabase.CustomVariableNames(v)
            NewVariableControl.OriginalVariableName = VariableName

            'Determining if the variable is numeric or not, and setting the corresponding variable selection control value
            Dim IsNumericVariable As Boolean = True
            If LoadedLexicalDatabase.CustomVariableTypes(v) = VariableTypes.Categorical Then
                IsNumericVariable = False
            End If
            NewVariableControl.IsNumericVariable = IsNumericVariable

            'Setting a random background color on the control
            NewVariableControl.BackColor = Drawing.Color.FromArgb(20, CSng(rnd.Next(10, 255)), CSng(rnd.Next(10, 255)), CSng(rnd.Next(10, 255)))

            'Adding the variable selection control
            NewVariableControl.Dock = Windows.Forms.DockStyle.Fill
            Variables_TableLayoutPanel.Controls.Add(NewVariableControl)

        Next

        FormatVariableSelectionControl()

        Variables_TableLayoutPanel.ResumeLayout()

        Return True

    End Function



    Private Sub FormatVariableSelectionControl() Handles Me.Resize

        'Fixes the row and column styles
        Variables_TableLayoutPanel.ColumnStyles.Clear()
        Variables_TableLayoutPanel.ColumnStyles.Add(New Windows.Forms.ColumnStyle(Windows.Forms.SizeType.Percent, 100))

        Variables_TableLayoutPanel.RowStyles.Clear()
        For i = 0 To Variables_TableLayoutPanel.Controls.Count - 1
            Variables_TableLayoutPanel.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Absolute, 36))
        Next

    End Sub

    Private Sub AddAndSave_Button_Click(sender As Object, e As EventArgs) Handles Calculate_Button.Click

        If LoadedSpeechMaterial Is Nothing Then
            UpdateControlEnabledStatuses()
            MsgBox("No speech material components loaded.", MsgBoxStyle.Information, "Calculate statistics")
            Exit Sub
        End If

        If SourceLevel_ComboBox.SelectedItem Is Nothing Then
            UpdateControlEnabledStatuses()
            MsgBox("Select a source linguistic level.", MsgBoxStyle.Information, "Calculate statistics")
            Exit Sub
        End If

        Dim SourceLevel As SpeechMaterialComponent.LinguisticLevels = SourceLevel_ComboBox.SelectedItem


        'Calculating metrics on all higher levels
        For Each VariableControl As CustomVariableSelectControl In Variables_TableLayoutPanel.Controls

            'Only calculating summary statistics for selected numeric variables
            If VariableControl.IsSelected = True And VariableControl.IsNumericVariable = True Then

                Dim TopLevelControl = LoadedSpeechMaterial.GetToplevelAncestor

                If VariableControl.IsNumericVariable Then

                    If VariableControl.ArithmeticMean_CheckBox.Checked = True Then
                        TopLevelControl.SummariseNumericVariables(SourceLevel, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.NumericSummaryMetricTypes.ArithmeticMean)
                    End If

                    If VariableControl.SD_CheckBox.Checked = True Then
                        TopLevelControl.SummariseNumericVariables(SourceLevel, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.NumericSummaryMetricTypes.StandardDeviation)
                    End If

                    If VariableControl.Max_CheckBox.Checked = True Then
                        TopLevelControl.SummariseNumericVariables(SourceLevel, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.NumericSummaryMetricTypes.Maximum)
                    End If

                    If VariableControl.Min_CheckBox.Checked = True Then
                        TopLevelControl.SummariseNumericVariables(SourceLevel, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.NumericSummaryMetricTypes.Minimum)
                    End If

                    If VariableControl.Median_CheckBox.Checked = True Then
                        TopLevelControl.SummariseNumericVariables(SourceLevel, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.NumericSummaryMetricTypes.Median)
                    End If

                    If VariableControl.IQR_CheckBox.Checked = True Then
                        TopLevelControl.SummariseNumericVariables(SourceLevel, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.NumericSummaryMetricTypes.InterquartileRange)
                    End If

                    If VariableControl.CV_CheckBox.Checked = True Then
                        TopLevelControl.SummariseNumericVariables(SourceLevel, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.NumericSummaryMetricTypes.CoefficientOfVariation)
                    End If

                Else

                    If VariableControl.Mode_CheckBox.Checked = True Then
                        TopLevelControl.SummariseCategoricalVariables(SourceLevel, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.CategoricalSummaryMetricTypes.Mode)
                    End If


                End If

            End If

        Next


    End Sub

    Private Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click

        If LoadedSpeechMaterial Is Nothing Then
            UpdateControlEnabledStatuses()
            MsgBox("No speech material components loaded.", MsgBoxStyle.Information, "Saving speech material components")
            Exit Sub
        End If

        'Saving updated files
        LoadedSpeechMaterial.GetToplevelAncestor.WriteSpeechMaterialComponenFile()

    End Sub

End Class
