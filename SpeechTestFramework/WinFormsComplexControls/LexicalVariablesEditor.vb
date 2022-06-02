Public Class LexicalVariablesEditor

    ' Looks up word level (lexical) variables from a user specified tab delimited text file and adds that data to the word level custom variables file that belong to the loaded speech material.
    ' Optinally also calculates variables on other levels ...

    Private LoadedSpeechMaterial As SpeechMaterialComponent
    Private LoadedLexicalDatabase As CustomVariablesDatabase

    Private LookupMatchBy As CustomVariablesDatabase.LookupMathOptions



    Private Sub LexicalVariablesEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        MatchBoth_RadioButton.Checked = True

        SpellingVariableNameTextBox.Text = SpeechMaterialComponent.DefaultSpellingVariableName
        TranscriptionVariableNameTextBox.Text = SpeechMaterialComponent.DefaultTranscriptionVariableName

        UpdateControlEnabledStatuses()

    End Sub


    Private Sub LoadSpeechMaterial_LoadFileControl_LoadFile(FileToLoad As String) Handles LoadSpeechMaterial_LoadFileControl.LoadFile
        Try

            LoadedSpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(FileToLoad)

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
            Dim AllWordLevelComponents = LoadedSpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.Word)
            Dim SpeechMaterialLookupKeys As New SortedSet(Of String)

            For Each WordComponent In AllWordLevelComponents

                Dim Spelling As String = WordComponent.GetCategoricalWordMetricValue(SpeechMaterialComponent.DefaultSpellingVariableName).Trim
                If CaseInsensitiveSpellings = True Then
                    Spelling = Spelling.ToLower
                End If

                Dim UniqueIdentifier As String = ""
                Select Case LookupMatchBy
                    Case CustomVariablesDatabase.LookupMathOptions.MatchBySpellingAndTranscription
                        Dim Transcription As String = WordComponent.GetCategoricalWordMetricValue(SpeechMaterialComponent.DefaultTranscriptionVariableName).Trim
                        UniqueIdentifier = Spelling & vbTab & Transcription
                    Case CustomVariablesDatabase.LookupMathOptions.MatchBySpelling
                        UniqueIdentifier = Spelling
                    Case CustomVariablesDatabase.LookupMathOptions.MatchByTranscription
                        Dim Transcription As String = WordComponent.GetCategoricalWordMetricValue(SpeechMaterialComponent.DefaultTranscriptionVariableName).Trim
                        UniqueIdentifier = Transcription
                End Select

                If SpeechMaterialLookupKeys.Contains(UniqueIdentifier) = False Then SpeechMaterialLookupKeys.Add(UniqueIdentifier)
            Next


            LoadedLexicalDatabase = New CustomVariablesDatabase
            LoadedLexicalDatabase.LoadTabDelimitedFile(FileToLoad, LookupMatchBy, SpellingVariableNameTextBox.Text.Trim, TranscriptionVariableNameTextBox.Text.Trim, CaseInsensitiveSpellings, SpeechMaterialLookupKeys)

            'Checking that all needed words were found in the database
            Dim WordsNotFound As New List(Of String)
            For Each LookupKey In SpeechMaterialLookupKeys
                If LoadedLexicalDatabase.UniqueIdentifierIsPresent(LookupKey) = False Then
                    WordsNotFound.Add(LookupKey)
                End If
            Next

            If WordsNotFound.Count > 0 Then

                LoadedLexicalDatabase = Nothing
                MsgBox(WordsNotFound.Count & " words could not be found in the selected lexical database. These need to be added before continuing. Click OK to show the missing words in a separate window.", MsgBoxStyle.Information, "Missing words in database")
                Dim NewForm As New Windows.Forms.Form With {.Padding = New Windows.Forms.Padding(4)}
                Dim NewFormTextBox As New Windows.Forms.RichTextBox With {.Dock = Windows.Forms.DockStyle.Fill, .Lines = WordsNotFound.ToArray}
                NewForm.Controls.Add(NewFormTextBox)
                NewForm.Show()

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


    Private Sub AddAndSave_Button_Click(sender As Object, e As EventArgs) Handles AddAndSave_Button.Click



    End Sub

End Class
