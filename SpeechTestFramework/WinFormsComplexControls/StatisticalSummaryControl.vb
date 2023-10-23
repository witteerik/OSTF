Public Class StatisticalSummaryControl

    Private LoadedSpeechMaterial As SpeechMaterialComponent

    Private Sub StatisticalSummaryControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try

            SourceLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Phoneme)
            SourceLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Word)
            SourceLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Sentence)
            SourceLevel_ComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.List)

            'Selecting a default level
            SourceLevel_ComboBox.SelectedItem = SpeechMaterialComponent.LinguisticLevels.Word

            UpdateControlEnabledStatuses()

        Catch ex As Exception
            MsgBox("The following error occured: " & vbCrLf & vbCrLf & ex.ToString)
        End Try

    End Sub

    Private Sub UpdateControlEnabledStatuses()

        If LoadedSpeechMaterial IsNot Nothing Then
            LevelSelection_GroupBox.Enabled = True

            If Variables_TableLayoutPanel.Controls.Count > 0 Then
                Calculate_Button.Enabled = True
                SaveButton.Enabled = True
            Else
                Calculate_Button.Enabled = False
                SaveButton.Enabled = False
            End If

        Else
            LevelSelection_GroupBox.Enabled = False
            Calculate_Button.Enabled = False
            SaveButton.Enabled = False
        End If

    End Sub


    Private Sub LoadOstaTestSpecificationControl1_SpeechTestSpecificationSelected() Handles LoadOstaTestSpecificationControl1.SpeechTestSpecificationSelected

        Try

            LoadOstaTestSpecificationControl1.SelectedTestSpecification.LoadSpeechMaterialComponentsFile()

            LoadedSpeechMaterial = LoadOstaTestSpecificationControl1.SelectedTestSpecification.SpeechMaterial

            If LoadedSpeechMaterial IsNot Nothing Then
                LoadedSpeechMaterialName_TextBox.Text = LoadedSpeechMaterial.PrimaryStringRepresentation
            Else
                LoadedSpeechMaterialName_TextBox.Text = "No speech material loaded"
                MsgBox("Unable to load the speech material file.", MsgBoxStyle.Information, "File reading error")
            End If
        Catch ex As Exception
            MsgBox("The following error occured: " & vbCrLf & vbCrLf & ex.ToString)
        End Try

        UpdateControlEnabledStatuses()

    End Sub


    Private Sub ViewVariables_Button_Click(sender As Object, e As EventArgs) Handles ViewVariables_Button.Click
        ViewSourceLevelVariables()

        UpdateControlEnabledStatuses()

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


        Variables_TableLayoutPanel.SuspendLayout()

        Dim rnd As New Random(CInt(SourceLevel))

        For Each CustomVariable In AllCustomVariables

            Dim NewVariableControl As New SummaryStatisticsSelectionControl

            'Setting the variable name in the variable selection control
            Dim VariableName = CustomVariable.Key
            NewVariableControl.VariableName = VariableName

            'Determining if the variable is numeric or not, and setting the corresponding variable selection control value
            Dim IsNumericVariable As Boolean = CustomVariable.Value
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

        Variables_TableLayoutPanel.SuspendLayout()

        'Fixes the row and column styles
        Variables_TableLayoutPanel.ColumnStyles.Clear()
        Variables_TableLayoutPanel.ColumnStyles.Add(New Windows.Forms.ColumnStyle(Windows.Forms.SizeType.Percent, 100))

        Variables_TableLayoutPanel.RowStyles.Clear()
        For i = 0 To Variables_TableLayoutPanel.Controls.Count - 1
            Variables_TableLayoutPanel.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Absolute, 100))
        Next

        Variables_TableLayoutPanel.ResumeLayout()


    End Sub

    Private Sub Calculate_Button_Click(sender As Object, e As EventArgs) Handles Calculate_Button.Click

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
        For Each VariableControl As SummaryStatisticsSelectionControl In Variables_TableLayoutPanel.Controls

            'Only calculating summary statistics for selected numeric variables
            If VariableControl.IsSelected = True Then

                Dim TopLevelControl = LoadedSpeechMaterial.GetToplevelAncestor

                If VariableControl.IsNumericVariable Then

                    If VariableControl.ArithmeticMean_CheckBox.Checked = True Then
                        TopLevelControl.SummariseNumericVariables(SourceLevel, VariableControl.VariableName, SpeechMaterialComponent.NumericSummaryMetricTypes.ArithmeticMean)
                    End If

                    If VariableControl.SD_CheckBox.Checked = True Then
                        TopLevelControl.SummariseNumericVariables(SourceLevel, VariableControl.VariableName, SpeechMaterialComponent.NumericSummaryMetricTypes.StandardDeviation)
                    End If

                    If VariableControl.Max_CheckBox.Checked = True Then
                        TopLevelControl.SummariseNumericVariables(SourceLevel, VariableControl.VariableName, SpeechMaterialComponent.NumericSummaryMetricTypes.Maximum)
                    End If

                    If VariableControl.Min_CheckBox.Checked = True Then
                        TopLevelControl.SummariseNumericVariables(SourceLevel, VariableControl.VariableName, SpeechMaterialComponent.NumericSummaryMetricTypes.Minimum)
                    End If

                    If VariableControl.Median_CheckBox.Checked = True Then
                        TopLevelControl.SummariseNumericVariables(SourceLevel, VariableControl.VariableName, SpeechMaterialComponent.NumericSummaryMetricTypes.Median)
                    End If

                    If VariableControl.IQR_CheckBox.Checked = True Then
                        TopLevelControl.SummariseNumericVariables(SourceLevel, VariableControl.VariableName, SpeechMaterialComponent.NumericSummaryMetricTypes.InterquartileRange)
                    End If

                    If VariableControl.CV_CheckBox.Checked = True Then
                        TopLevelControl.SummariseNumericVariables(SourceLevel, VariableControl.VariableName, SpeechMaterialComponent.NumericSummaryMetricTypes.CoefficientOfVariation)
                    End If

                Else

                    If VariableControl.Mode_CheckBox.Checked = True Then
                        TopLevelControl.SummariseCategoricalVariables(SourceLevel, VariableControl.VariableName, SpeechMaterialComponent.CategoricalSummaryMetricTypes.Mode)
                    End If

                    If VariableControl.Distribution_CheckBox.Checked = True Then
                        TopLevelControl.SummariseCategoricalVariables(SourceLevel, VariableControl.VariableName, SpeechMaterialComponent.CategoricalSummaryMetricTypes.Distribution)
                    End If

                End If
            End If

        Next

        MsgBox("Finished calculating summary statistics", MsgBoxStyle.Information, "Run calculations")

    End Sub

    Private Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click

        If LoadedSpeechMaterial Is Nothing Then
            UpdateControlEnabledStatuses()
            MsgBox("No speech material components loaded.", MsgBoxStyle.Information, "Saving speech material components")
            Exit Sub
        End If

        'Saving updated files
        'Ask if overwrite or save to new location
        Dim res = MsgBox("Do you want to overwrite any existing files? Select NO to save the new files to a new location?", MsgBoxStyle.YesNo, "Overwrite existing files?")
        If res = MsgBoxResult.Yes Then

            'Saving updated files
            LoadedSpeechMaterial.GetToplevelAncestor.WriteSpeechMaterialToFile(LoadedSpeechMaterial.ParentTestSpecification, LoadedSpeechMaterial.ParentTestSpecification.GetTestRootPath)

            MsgBox("Your speech material file and corresponding custom variable files should now have been saved to " & vbCrLf & LoadedSpeechMaterial.ParentTestSpecification.GetSpeechMaterialFolder & vbCrLf & vbCrLf & "Click OK to continue.",
                   MsgBoxStyle.Information, "Files saved")

        Else


            'Saving updated files
            LoadedSpeechMaterial.GetToplevelAncestor.WriteSpeechMaterialToFile(LoadedSpeechMaterial.ParentTestSpecification)
            MsgBox("Your speech material file and corresponding custom variable files should now have been saved to the selected folder." & vbCrLf & vbCrLf & "Click OK to continue.", MsgBoxStyle.Information, "Files saved")

        End If


    End Sub

End Class
