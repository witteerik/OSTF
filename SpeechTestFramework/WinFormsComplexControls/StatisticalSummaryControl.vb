Public Class StatisticalSummaryControl

    Private LoadedSpeechMaterial As SpeechMaterialComponent


    Private Sub UpdateControlEnabledStatuses()

        If LoadedSpeechMaterial IsNot Nothing Then
            'LoadDatabase_GroupBox.Enabled = True
        Else
            'LoadDatabase_GroupBox.Enabled = False
        End If


    End Sub


    Private Sub AddAndSave_Button_Click(sender As Object, e As EventArgs) Handles AddAndSave_Button.Click

        If LoadedSpeechMaterial Is Nothing Then
            UpdateControlEnabledStatuses()
            Exit Sub
        End If


        Dim AllWordLevelComponents = LoadedSpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.Word)



        'Calculating metrics on all higher levels
        For Each VariableControl As CustomVariableSelectControl In Variables_TableLayoutPanel.Controls

            'Only calculating summary statistics for selected numeric variables
            If VariableControl.IsSelected = True And VariableControl.IsNumericVariable = True Then

                Dim TopLevelControl = LoadedSpeechMaterial.GetToplevelAncestor

                If VariableControl.ArithmeticMean_CheckBox.Checked = True Then
                    TopLevelControl.SummariseNumericVariables(SpeechMaterialComponent.LinguisticLevels.Word, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.SummaryMetricTypes.ArithmeticMean)
                End If

                If VariableControl.SD_CheckBox.Checked = True Then
                    TopLevelControl.SummariseNumericVariables(SpeechMaterialComponent.LinguisticLevels.Word, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.SummaryMetricTypes.StandardDeviation)
                End If

                If VariableControl.Max_CheckBox.Checked = True Then
                    TopLevelControl.SummariseNumericVariables(SpeechMaterialComponent.LinguisticLevels.Word, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.SummaryMetricTypes.Maximum)
                End If

                If VariableControl.Min_CheckBox.Checked = True Then
                    TopLevelControl.SummariseNumericVariables(SpeechMaterialComponent.LinguisticLevels.Word, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.SummaryMetricTypes.Minimum)
                End If

                If VariableControl.Median_CheckBox.Checked = True Then
                    TopLevelControl.SummariseNumericVariables(SpeechMaterialComponent.LinguisticLevels.Word, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.SummaryMetricTypes.Median)
                End If

                If VariableControl.IQR_CheckBox.Checked = True Then
                    TopLevelControl.SummariseNumericVariables(SpeechMaterialComponent.LinguisticLevels.Word, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.SummaryMetricTypes.InterquartileRange)
                End If

                If VariableControl.CV_CheckBox.Checked = True Then
                    TopLevelControl.SummariseNumericVariables(SpeechMaterialComponent.LinguisticLevels.Word, VariableControl.GetUpdatedVariableName, SpeechMaterialComponent.SummaryMetricTypes.CoefficientOfVariation)
                End If

            End If

        Next

        'Saving updated files
        LoadedSpeechMaterial.GetToplevelAncestor.WriteSpeechMaterialComponenFile()

    End Sub


End Class
