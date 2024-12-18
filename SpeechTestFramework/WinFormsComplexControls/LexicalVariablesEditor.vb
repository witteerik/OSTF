﻿Public Class LexicalVariablesEditor

    ' Looks up word level (lexical) variables from a user specified tab delimited text file and adds that data to the word level custom variables file that belong to the loaded speech material.

    Private LoadedSpeechMaterial As SpeechMaterialComponent
    Private LoadedLexicalDatabase As CustomVariablesDatabase

    Private LookupMatchBy As CustomVariablesDatabase.LookupMathOptions



    Private Sub LexicalVariablesEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try

            MatchBoth_RadioButton.Checked = True

            SpellingVariableNameTextBox.Text = SpeechMaterialComponent.DefaultSpellingVariableName
            TranscriptionVariableNameTextBox.Text = SpeechMaterialComponent.DefaultTranscriptionVariableName

            UpdateControlEnabledStatuses()

        Catch ex As Exception
            MsgBox("The following error occured: " & vbCrLf & vbCrLf & ex.ToString)
        End Try

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

    'Private Sub LoadSpeechMaterial_LoadFileControl_LoadFile(FileToLoad As String) Handles LoadSpeechMaterial_LoadFileControl.LoadFile
    '    Try

    '        LoadedSpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(FileToLoad, "")

    '        If LoadedSpeechMaterial IsNot Nothing Then
    '            LoadedSpeechMaterialName_TextBox.Text = LoadedSpeechMaterial.PrimaryStringRepresentation
    '        Else
    '            LoadedSpeechMaterialName_TextBox.Text = "No speech material loaded"
    '            MsgBox("Unable to load the speech material file.", MsgBoxStyle.Information, "File reading error")
    '        End If
    '    Catch ex As Exception
    '        MsgBox("The following error occured: " & vbCrLf & vbCrLf & ex.ToString)
    '    End Try

    '    UpdateControlEnabledStatuses()

    'End Sub

    Private Sub LoadDatabase_LoadFileControl_LoadFile(FileToLoad As String) Handles LoadDatabase_LoadFileControl.LoadFile

        Try

            Select Case LookupMatchBy
                Case CustomVariablesDatabase.LookupMathOptions.MatchBySpellingAndTranscription
                    If SpellingVariableNameTextBox.Text.Trim = "" Then
                        MsgBox("Add a spelling variable name", MsgBoxStyle.Information, "Checking input")
                        Exit Sub
                    End If
                    If TranscriptionVariableNameTextBox.Text.Trim = "" Then
                        MsgBox("Add a transcription variable name", MsgBoxStyle.Information, "Checking input")
                        Exit Sub
                    End If

                Case CustomVariablesDatabase.LookupMathOptions.MatchBySpelling
                    If SpellingVariableNameTextBox.Text.Trim = "" Then
                        MsgBox("Add a spelling variable name", MsgBoxStyle.Information, "Checking input")
                        Exit Sub
                    End If

                Case CustomVariablesDatabase.LookupMathOptions.MatchByTranscription
                    If TranscriptionVariableNameTextBox.Text.Trim = "" Then
                        MsgBox("Add a transcription variable name", MsgBoxStyle.Information, "Checking input")
                        Exit Sub
                    End If
            End Select

            Dim CaseInsensitiveSpellings As Boolean = CaseInvariantLookupCheckBox.Checked
            Dim IgnoreZeroPhonemes As Boolean = IgnoreZeroPhonemesCheckBox.Checked
            Dim AllWordLevelComponents = LoadedSpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.Word)
            Dim SpeechMaterialLookupKeys As New SortedList(Of String, String)

            For Each WordComponent In AllWordLevelComponents

                Dim Spelling As String = WordComponent.GetCategoricalVariableValue(SpeechMaterialComponent.DefaultSpellingVariableName).Trim
                If CaseInsensitiveSpellings = True Then
                    Spelling = Spelling.ToLower
                End If

                Dim OriginalUniqueIdentifier As String = ""
                Dim PotentiallyModifiedUniqueIdentifier As String = ""
                Select Case LookupMatchBy
                    Case CustomVariablesDatabase.LookupMathOptions.MatchBySpellingAndTranscription
                        Dim OriginalTranscription As String = WordComponent.GetCategoricalVariableValue(SpeechMaterialComponent.DefaultTranscriptionVariableName).Trim
                        Dim Transcription As String = OriginalTranscription
                        If IgnoreZeroPhonemes = True Then
                            Transcription = Transcription.Replace(IPA.ZeroPhoneme, "")
                            Transcription = Transcription.Trim
                            Transcription = Transcription.Replace("  ", " ")
                            Transcription = Transcription.Trim
                        End If
                        PotentiallyModifiedUniqueIdentifier = Spelling & vbTab & Transcription
                        OriginalUniqueIdentifier = Spelling & vbTab & OriginalTranscription
                    Case CustomVariablesDatabase.LookupMathOptions.MatchBySpelling
                        PotentiallyModifiedUniqueIdentifier = Spelling
                        OriginalUniqueIdentifier = Spelling
                    Case CustomVariablesDatabase.LookupMathOptions.MatchByTranscription
                        Dim OriginalTranscription As String = WordComponent.GetCategoricalVariableValue(SpeechMaterialComponent.DefaultTranscriptionVariableName).Trim
                        Dim Transcription As String = OriginalTranscription
                        If IgnoreZeroPhonemes = True Then
                            Transcription = Transcription.Replace(IPA.ZeroPhoneme, "")
                            Transcription = Transcription.Trim
                            Transcription = Transcription.Replace("  ", " ")
                            Transcription = Transcription.Trim
                        End If
                        PotentiallyModifiedUniqueIdentifier = Transcription
                        OriginalUniqueIdentifier = OriginalTranscription
                End Select

                If SpeechMaterialLookupKeys.ContainsKey(PotentiallyModifiedUniqueIdentifier) = False Then SpeechMaterialLookupKeys.Add(PotentiallyModifiedUniqueIdentifier, OriginalUniqueIdentifier)
            Next


            LoadedLexicalDatabase = New CustomVariablesDatabase With {.CaseInvariantSpellings = CaseInsensitiveSpellings}
            LoadedLexicalDatabase.LoadTabDelimitedFile(FileToLoad, LookupMatchBy, SpellingVariableNameTextBox.Text.Trim, TranscriptionVariableNameTextBox.Text.Trim, SpeechMaterialLookupKeys)

            'Checking that all needed words were found in the database
            Dim WordsNotFound As New List(Of String)
            For Each LookupKey In SpeechMaterialLookupKeys
                If LoadedLexicalDatabase.UniqueIdentifierIsPresent(LookupKey.Value) = False Then
                    WordsNotFound.Add(LookupKey.Value)
                End If
            Next

            If WordsNotFound.Count > 0 Then

                LoadedLexicalDatabase = Nothing
                MsgBox(WordsNotFound.Count & " words could not be found in the selected lexical database. These need to be added before continuing. Click OK to show the missing words in a separate window.", MsgBoxStyle.Information, "Missing words in database")
                Dim NewForm As New Windows.Forms.Form With {.Padding = New Windows.Forms.Padding(4)}
                Dim NewFormTextBox As New Windows.Forms.RichTextBox With {.Dock = Windows.Forms.DockStyle.Fill, .Lines = WordsNotFound.ToArray}
                NewForm.Controls.Add(NewFormTextBox)
                NewForm.Show()

            Else
                'Adds variables
                AddVariablesToGUI()
            End If

        Catch ex As Exception
            MsgBox("The following error occured: " & vbCrLf & vbCrLf & ex.ToString)
        End Try

        UpdateControlEnabledStatuses()

    End Sub

    Private Sub UpdateControlEnabledStatuses()

        If LoadedSpeechMaterial IsNot Nothing Then
            LoadDatabase_GroupBox.Enabled = True
        Else
            LoadDatabase_GroupBox.Enabled = False
        End If

        If LoadedSpeechMaterial IsNot Nothing And LoadedLexicalDatabase IsNot Nothing Then
            ProcessingGroupBox.Enabled = True
        Else
            ProcessingGroupBox.Enabled = False
        End If

    End Sub

    Private Sub MatchBoth_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles MatchBoth_RadioButton.CheckedChanged
        LookupMatchBy = CustomVariablesDatabase.LookupMathOptions.MatchBySpellingAndTranscription
    End Sub

    Private Sub MatchBySpellingOnly_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles MatchBySpellingOnly_RadioButton.CheckedChanged
        LookupMatchBy = CustomVariablesDatabase.LookupMathOptions.MatchBySpelling
    End Sub

    Private Sub MatchByTranscriptionOnly_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles MatchByTranscriptionOnly_RadioButton.CheckedChanged
        LookupMatchBy = CustomVariablesDatabase.LookupMathOptions.MatchByTranscription
    End Sub

    Private Function AddVariablesToGUI() As Boolean

        Variables_TableLayoutPanel.Controls.Clear()
        Variables_TableLayoutPanel.RowCount = 1
        Variables_TableLayoutPanel.ColumnCount = 1

        If LoadedSpeechMaterial Is Nothing Or LoadedLexicalDatabase Is Nothing Then
            Return False
        End If

        Dim rnd As New Random(30)

        Variables_TableLayoutPanel.SuspendLayout()

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

        Variables_TableLayoutPanel.SuspendLayout()

        'Fixes the row and column styles
        Variables_TableLayoutPanel.ColumnStyles.Clear()
        Variables_TableLayoutPanel.ColumnStyles.Add(New Windows.Forms.ColumnStyle(Windows.Forms.SizeType.Percent, 100))

        Variables_TableLayoutPanel.RowStyles.Clear()
        For i = 0 To Variables_TableLayoutPanel.Controls.Count - 1
            Variables_TableLayoutPanel.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Absolute, 36))
        Next

        Variables_TableLayoutPanel.ResumeLayout()

    End Sub

    Private Sub AddAndSave_Button_Click(sender As Object, e As EventArgs) Handles AddAndSave_Button.Click

        If LoadedSpeechMaterial Is Nothing Or LoadedLexicalDatabase Is Nothing Then
            UpdateControlEnabledStatuses()
            Exit Sub
        End If


        Dim AllWordLevelComponents = LoadedSpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.Word)

        For Each WordComponent In AllWordLevelComponents

            Dim Spelling As String = WordComponent.GetCategoricalVariableValue(SpeechMaterialComponent.DefaultSpellingVariableName).Trim
            If LoadedLexicalDatabase.CaseInvariantSpellings = True Then
                Spelling = Spelling.ToLower
            End If

            Dim UniqueIdentifier As String = ""
            Select Case LookupMatchBy
                Case CustomVariablesDatabase.LookupMathOptions.MatchBySpellingAndTranscription
                    Dim Transcription As String = WordComponent.GetCategoricalVariableValue(SpeechMaterialComponent.DefaultTranscriptionVariableName).Trim
                    UniqueIdentifier = Spelling & vbTab & Transcription
                Case CustomVariablesDatabase.LookupMathOptions.MatchBySpelling
                    UniqueIdentifier = Spelling
                Case CustomVariablesDatabase.LookupMathOptions.MatchByTranscription
                    Dim Transcription As String = WordComponent.GetCategoricalVariableValue(SpeechMaterialComponent.DefaultTranscriptionVariableName).Trim
                    UniqueIdentifier = Transcription
            End Select

            'Getting selected variables
            For Each VariableControl As CustomVariableSelectionControl In Variables_TableLayoutPanel.Controls

                'Only including selected variables
                If VariableControl.IsSelected = True Then

                    'Looing up the variable and value for the current word
                    Dim CurrentOriginalVariableName As String = VariableControl.OriginalVariableName
                    Dim CurrentVariableValue As Object = LoadedLexicalDatabase.GetVariableValue(UniqueIdentifier, CurrentOriginalVariableName)

                    'Adding the variable and variable value
                    'Using the updated name (which may be the original name or a different name set manually by the user)
                    Dim CurrentUpdatedVariableName As String = VariableControl.GetUpdatedVariableName

                    'Selects variable type, and saves the variable value
                    If VariableControl.IsNumericVariable = True Then
                        WordComponent.SetNumericVariableValue(CurrentUpdatedVariableName, CurrentVariableValue)
                    Else
                        WordComponent.SetCategoricalVariableValue(CurrentUpdatedVariableName, CurrentVariableValue)
                    End If
                End If
            Next
        Next

        'Calculates logically derived variables
        If HasVowelContrast_CheckBox.Checked = True Then
            Try
                'TODO, we could add an option for the user to specify transcription variable name below
                LoadedSpeechMaterial.CreateVariable_HasVowelContrast(SpeechMaterialComponent.LinguisticLevels.List,, "HasVowelContrast")
            Catch ex As Exception
                MsgBox("Unable to calculate variable HasVowelContrast.")
            End Try
        End If


        If ContrastedPhonemeIndex_CheckBox.Checked = True Then
            Try
                'TODO, we could add an option for the user to specify transcription variable name below
                LoadedSpeechMaterial.CreateVariable_ContrastedPhonemeIndex(SpeechMaterialComponent.LinguisticLevels.List,, "ContrastedPhonemeIndex")
            Catch ex As Exception
                MsgBox("Unable to calculate variable ContrastedPhonemeIndex.")
            End Try
        End If


        'Ask if overwrite or save to new location
        Dim res = MsgBox("Do you want to overwrite any existing files? Select NO to save the new files to a new location?", MsgBoxStyle.YesNo, "Overwrite existing files?")
        If res = MsgBoxResult.Yes Then

            'Saving updated files
            LoadedSpeechMaterial.GetToplevelAncestor.WriteSpeechMaterialToFile(LoadedSpeechMaterial.ParentTestSpecification, LoadedSpeechMaterial.ParentTestSpecification.GetTestRootPath)

            MsgBox("Your speech material file and corresponding custom variable files should now have been saved to " & LoadedSpeechMaterial.ParentTestSpecification.GetSpeechMaterialFolder & vbCrLf & "Click OK to continue.",
                   MsgBoxStyle.Information, "Files saved")

        Else


            'Saving updated files
            LoadedSpeechMaterial.GetToplevelAncestor.WriteSpeechMaterialToFile(LoadedSpeechMaterial.ParentTestSpecification)

            MsgBox("Your speech material file and corresponding custom variable files should now have been saved to the selected folder. Click OK to continue.", MsgBoxStyle.Information, "Files saved")

        End If


    End Sub

End Class
