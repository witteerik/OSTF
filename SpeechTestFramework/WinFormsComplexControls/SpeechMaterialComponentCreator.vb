Public Class SpeechMaterialComponentCreator

    Private rnd As New Random

    Private DefaultSpellingVariableName As String = "Spelling"
    Private DefaultTranscriptionVariableName As String = "Transcription"
    Private DefaultNotFoundTranscriptionString As String = "---"

    Private Sub SpeechMaterialComponentCreator_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.List)
        SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Sentence)
        SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Word)
        SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Phoneme)

    End Sub

    Private Sub CheckInputButton_Click(sender As Object, e As EventArgs) Handles CheckInputButton.Click

        If CheckInput().Item1 = True Then
            'Updates the textbox with the new data
            MsgBox("All checks passed for creating a speech material component file.", MsgBoxStyle.Information, "Checking input data")
            CreateSpeechMaterialComponentFile_Button.Enabled = True
        End If

    End Sub

    Private Function CheckInput() As Tuple(Of Boolean, SpeechMaterialComponent)


        Dim SpeechMaterialLevelDatabaseName As String = "SpeechMaterialLevelDatabase.txt"
        Dim ListLevelDataBaseName As String = "ListLevelVariables.txt"
        Dim SentenceLevelDataBaseName As String = "SentenceLevelVariables.txt"
        Dim WordLevelDataBaseName As String = "WordLevelVariables.txt"
        Dim PhonemeLevelDataBaseName As String = "PhonemeLevelVariables.txt"

        Dim SoundFilesAtLevel As SpeechMaterialComponent.LinguisticLevels
        If [Enum].TryParse(SoundFileLevelComboBox.SelectedItem, SoundFilesAtLevel) = False Then
            MsgBox("Select a value for 'Sound files at linguistic level'.", MsgBoxStyle.Information, "Checking input data")
            Return New Tuple(Of Boolean, SpeechMaterialComponent)(False, Nothing)
        End If

        If NameTextBox.Text = "" Then
            MsgBox("Add a name", MsgBoxStyle.Information, "Checking input data")
            Return New Tuple(Of Boolean, SpeechMaterialComponent)(False, Nothing)
        End If

        Dim Input = EditRichTextBox.Lines

        'The whole input represent a ListCollection
        Dim CurrentListCollectionComponent As New SpeechMaterialComponent(rnd) With {
            .Id = NameTextBox.Text,
            .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.ListCollection,
            .PrimaryStringRepresentation = NameTextBox.Text,
            .CustomVariablesDatabasePath = SpeechMaterialLevelDatabaseName}

        CurrentListCollectionComponent.SetCategoricalWordMetricValue("DbId", CurrentListCollectionComponent.Id)

        Dim CurrentListComponent As SpeechMaterialComponent = Nothing
        Dim CurrentSentenceComponent As SpeechMaterialComponent = Nothing

        'Parsing input lines
        For i = 0 To Input.Length - 1

            Dim CurrentLine = Input(i).Trim

            'Skips empty lines
            If CurrentLine = "" Then Continue For

            If CurrentLine.StartsWith("{") Then

                Dim ListName As String = CurrentLine.Trim().TrimStart("{").TrimEnd("}")

                'A new List
                CurrentListComponent = New SpeechMaterialComponent(rnd) With {
                    .ParentComponent = CurrentListCollectionComponent,
                    .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.List,
                    .CustomVariablesDatabasePath = ListLevelDataBaseName}

                CurrentListCollectionComponent.ChildComponents.Add(CurrentListComponent)

                CurrentListComponent.Id = "L" & (CurrentListCollectionComponent.ChildComponents.Count - 1).ToString("00")
                CurrentListComponent.DbId = CurrentListComponent.Id
                CurrentListComponent.PrimaryStringRepresentation = ListName
                If SoundFilesAtLevel = SpeechMaterialComponent.LinguisticLevels.List Then CurrentListComponent.MediaFolder = CurrentListComponent.Id & "_" & CurrentListComponent.PrimaryStringRepresentation.Replace(" ", "_")
                CurrentListComponent.OrderedChildren = OrderedSentencesCheckBox.Checked

                CurrentListComponent.SetCategoricalWordMetricValue("DbId", CurrentListComponent.DbId)
                CurrentListComponent.SetCategoricalWordMetricValue("ListName", ListName)

            ElseIf CurrentLine.StartsWith("[") Then

                'It should be the phonetic/phonemic transcription of the current sentence
                'Adds the transcriptions
                CurrentSentenceComponent.SetCategoricalWordMetricValue(DefaultTranscriptionVariableName, CurrentLine)

                Dim WordTranscriptions = CurrentLine.Split(",")
                'Checks that the number of transcriptions and spellings agree
                If CurrentSentenceComponent.ChildComponents.Count <> WordTranscriptions.Length Then
                    MsgBox("The number of transcriptions at line " & i + 1 & " do do agree with the number of speelings in the corresponding sentence: " & vbCrLf &
                   CurrentSentenceComponent.GetCategoricalWordMetricValue(DefaultSpellingVariableName) & vbCrLf & CurrentLine, MsgBoxStyle.Information, "Checking input data")
                    Return New Tuple(Of Boolean, SpeechMaterialComponent)(False, Nothing)
                End If

                For w = 0 To WordTranscriptions.Length - 1
                    CurrentSentenceComponent.ChildComponents(w).SetCategoricalWordMetricValue(DefaultTranscriptionVariableName, WordTranscriptions(w).Trim.TrimStart("[").TrimEnd("]".Trim))

                    'Adding phonetic transcription components, taken from the transcription (phonemes need to be separated by black spaces)
                    Dim Phonemes = CurrentSentenceComponent.ChildComponents(w).GetCategoricalWordMetricValue(DefaultTranscriptionVariableName).Split(" ")
                    For p = 0 To Phonemes.Length - 1

                        Dim CurrentPhoneme = Phonemes(p)

                        'Removes non-phoneme characters (TODO: this could instead be set by a list of valid phonetic characters, optinally set by the user?!?)
                        Dim IpaMainStress As String = "ˈ"
                        Dim IpaMainSwedishAccent2 As String = "²"
                        Dim IpaSecondaryStress As String = "ˌ"
                        Dim IpaSyllableBoundary As String = "."
                        Dim ReplacementList As New List(Of String) From {" ", ",", IpaMainStress, IpaSecondaryStress, IpaSyllableBoundary, IpaMainSwedishAccent2}
                        For Each s In ReplacementList
                            CurrentPhoneme = CurrentPhoneme.Replace(s, "")
                        Next
                        If CurrentPhoneme = "" Then Continue For

                        Dim NewPhonemeComponent = New SpeechMaterialComponent(rnd) With {
                            .ParentComponent = CurrentSentenceComponent.ChildComponents(w),
                            .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Phoneme,
                            .CustomVariablesDatabasePath = PhonemeLevelDataBaseName}

                        CurrentSentenceComponent.ChildComponents(w).ChildComponents.Add(NewPhonemeComponent)

                        NewPhonemeComponent.Id = CurrentSentenceComponent.ChildComponents(w).Id & "P" & p.ToString("00")
                        NewPhonemeComponent.DbId = NewPhonemeComponent.Id
                        NewPhonemeComponent.PrimaryStringRepresentation = CurrentPhoneme
                        If SoundFilesAtLevel = SpeechMaterialComponent.LinguisticLevels.Phoneme Then NewPhonemeComponent.MediaFolder = NewPhonemeComponent.Id & "_" & NewPhonemeComponent.PrimaryStringRepresentation.Replace(" ", "_")

                        NewPhonemeComponent.SetCategoricalWordMetricValue("DbId", NewPhonemeComponent.DbId)
                        NewPhonemeComponent.SetCategoricalWordMetricValue(DefaultTranscriptionVariableName, CurrentPhoneme)

                    Next
                Next

            Else

                'It should be sentence
                CurrentSentenceComponent = New SpeechMaterialComponent(rnd) With {
                    .ParentComponent = CurrentListComponent,
                    .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Sentence,
                            .CustomVariablesDatabasePath = SentenceLevelDataBaseName}

                CurrentListComponent.ChildComponents.Add(CurrentSentenceComponent)

                CurrentSentenceComponent.Id = CurrentListComponent.Id & "S" & (CurrentListComponent.ChildComponents.Count - 1).ToString("00")

                CurrentSentenceComponent.DbId = CurrentSentenceComponent.Id
                CurrentSentenceComponent.PrimaryStringRepresentation = "Sentence" & (CurrentListComponent.ChildComponents.Count - 1).ToString("00")
                If SoundFilesAtLevel = SpeechMaterialComponent.LinguisticLevels.Sentence Then CurrentSentenceComponent.MediaFolder = CurrentSentenceComponent.Id & "_" & CurrentSentenceComponent.PrimaryStringRepresentation.Replace(" ", "_")

                CurrentSentenceComponent.SetCategoricalWordMetricValue("DbId", CurrentSentenceComponent.DbId)
                CurrentSentenceComponent.SetCategoricalWordMetricValue(DefaultSpellingVariableName, CurrentLine)

                'Adds the word components
                Dim Words = CurrentLine.Split(" ")
                For w = 0 To Words.Length - 1
                    Dim NewWordComponent = New SpeechMaterialComponent(rnd) With {
                        .ParentComponent = CurrentSentenceComponent,
                        .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Word,
                        .CustomVariablesDatabasePath = WordLevelDataBaseName}

                    CurrentSentenceComponent.ChildComponents.Add(NewWordComponent)

                    NewWordComponent.Id = CurrentSentenceComponent.Id & "W" & w.ToString("00")

                    NewWordComponent.DbId = NewWordComponent.Id
                    NewWordComponent.PrimaryStringRepresentation = Words(w)
                    If SoundFilesAtLevel = SpeechMaterialComponent.LinguisticLevels.Word Then NewWordComponent.MediaFolder = NewWordComponent.Id & "_" & NewWordComponent.PrimaryStringRepresentation.Replace(" ", "_")

                    NewWordComponent.SetCategoricalWordMetricValue("DbId", NewWordComponent.DbId)
                    NewWordComponent.SetCategoricalWordMetricValue(DefaultSpellingVariableName, Words(w))

                Next

            End If

        Next

        Return New Tuple(Of Boolean, SpeechMaterialComponent)(True, CurrentListCollectionComponent)


    End Function


    Public Sub CreateSpeechMaterialComponents() Handles CreateSpeechMaterialComponentFile_Button.Click

        'Re-checking the input and gets the SmaComponent 
        Dim NewSmaComponent = CheckInput()
        If NewSmaComponent.Item1 = False Then
            Exit Sub
        End If

        'TODO:
        'Create sentence level transcription

        '??? Create phoneme/phone components based on the phonetic transcriptions ??? Already done?


        'Saving to file
        Dim FilePath = Utils.GetSaveFilePath(,, {".txt"}, "Save speech material components file")
        NewSmaComponent.Item2.WriteSpeechMaterialComponenFile(FilePath)

    End Sub

    Private Sub TranscriptionLookupButton_Click(sender As Object, e As EventArgs) Handles TranscriptionLookupButton.Click

        'Re-checking the input and gets the SmaComponent 
        Dim NewSmaComponent = CheckInput()
        If NewSmaComponent.Item1 = False Then
            Exit Sub
        End If

        'Gets all spellings used (at the word level) in the speech material
        Dim ExistingSpellingsList As New SortedSet(Of String)
        Dim WordLevelComponents = NewSmaComponent.Item2.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.Word)
        For Each Component In WordLevelComponents
            Dim Spelling = Component.GetCategoricalWordMetricValue(DefaultSpellingVariableName)
            If CaseInvariantLookupCheckBox.Checked = True Then
                Spelling = Spelling.ToLower
            End If
            If ExistingSpellingsList.Contains(Spelling) = False Then ExistingSpellingsList.Add(Spelling)
        Next

        'Tries to add phonetic transcriptions
        'Reads the tab-delimited file

        Dim TranscriptionDatabasePath As String = TranscriptionDatabase_FilePathControl.SelectedPath
        If TranscriptionDatabasePath = "" Then
            MsgBox("Add the trancription database file path.", MsgBoxStyle.Information, "No transcription database path")
            Exit Sub
        End If

        Dim SpellingVariableName As String = SpellingVariableNameTextBox.Text.Trim
        If SpellingVariableName = "" Then
            MsgBox("Add the variable name used for 'Spelling' in the trancription database file.", MsgBoxStyle.Information, "No spelling variable name")
            Exit Sub
        End If

        Dim TranscriptionVariableName As String = TranscriptionVariableNameTextBox.Text.Trim
        If TranscriptionVariableName = "" Then
            MsgBox("Add the variable name used for 'Transcription' in the trancription database file.", MsgBoxStyle.Information, "No transcription variable name")
            Exit Sub
        End If

        Dim SpellingColumnIndex As Integer = -1
        Dim TranscriptionColumnIndex As Integer = -1

        Dim LookupList As New List(Of Tuple(Of String, String))

        'Reads the transcription database file
        Dim InputLines() As String = System.IO.File.ReadAllLines(TranscriptionDatabasePath, System.Text.Encoding.UTF8)

        'First line should be variable names
        Dim FirstLineData() As String = InputLines(0).Split(vbTab)
        For c = 0 To FirstLineData.Length - 1
            If FirstLineData(c).Trim = SpellingVariableName Then SpellingColumnIndex = c
            If FirstLineData(c).Trim = TranscriptionVariableName Then TranscriptionColumnIndex = c
        Next

        If SpellingColumnIndex = -1 Then
            MsgBox("No variable named " & SpellingVariableName & " could be found in the trancription database file.", MsgBoxStyle.Information, "Missing variable")
            Exit Sub
        End If
        If TranscriptionColumnIndex = -1 Then
            MsgBox("No variable named " & TranscriptionVariableName & " could be found in the trancription database file.", MsgBoxStyle.Information, "Missing variable")
            Exit Sub
        End If

        'Reads the remaining lines, and adds data for existing spellings (to save memory and processing time if the database is very large...)
        For i = 1 To InputLines.Length - 1
            If InputLines(i).Trim = "" Then Continue For
            Dim CurrentLineData() As String = InputLines(i).Split(vbTab)
            Dim Spelling = CurrentLineData(SpellingColumnIndex).Trim
            If CaseInvariantLookupCheckBox.Checked = True Then
                Spelling = Spelling.ToLower
            End If
            Dim Transcription = CurrentLineData(TranscriptionColumnIndex).Trim
            If ExistingSpellingsList.Contains(Spelling) Then
                LookupList.Add(New Tuple(Of String, String)(Spelling, Transcription))
            End If
        Next

        'Looks up the transcriptions of all word level components in the LookupList
        For Each Component In WordLevelComponents
            Dim Spelling = Component.GetCategoricalWordMetricValue(DefaultSpellingVariableName)

            If CaseInvariantLookupCheckBox.Checked = True Then
                Spelling = Spelling.ToLower
            End If
            Dim MatchingSpellings = LookupList.FindAll(Function(x) x.Item1 = Spelling)
            Dim PossibleTranscriptions = New List(Of String)
            For Each MatchingSpelling In MatchingSpellings
                PossibleTranscriptions.Add(MatchingSpelling.Item2)
            Next

            'Creating a string which, if there are more than one possible transcription, the user will have to reduce to one manually. The / character denotes multiple transcriptions and will not be allowed when parsing the input again.
            Dim TranscriptionString As String = String.Join(" / ", PossibleTranscriptions)

            If TranscriptionString <> "" Then
                Component.SetCategoricalWordMetricValue(DefaultTranscriptionVariableName, TranscriptionString)
            Else
                'The spelling (and transcription) was not found. Outputting a the DefaultNotFoundTranscriptionString, which should be replaced manually by the user, and should be not be allowed when parsing the input again.
                Component.SetCategoricalWordMetricValue(DefaultTranscriptionVariableName, DefaultNotFoundTranscriptionString)
            End If
        Next

        'If all went well, we print the text back to the user, with the transcriptions interlined
        Dim OutputList As New List(Of String)
        Dim ListsComponents = NewSmaComponent.Item2.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)
        For Each ListComponent In ListsComponents

            OutputList.Add("{" & ListComponent.PrimaryStringRepresentation & "}")

            For Each SentenceComponent In ListComponent.ChildComponents
                OutputList.Add(SentenceComponent.GetCategoricalWordMetricValue(DefaultSpellingVariableName))

                Dim SentenceWordTranscriptions As New List(Of String)
                For Each WordComponent In SentenceComponent.ChildComponents
                    SentenceWordTranscriptions.Add("[ " & WordComponent.GetCategoricalWordMetricValue(DefaultTranscriptionVariableName) & " ]")
                Next

                Dim SentenceTranscription = String.Join(", ", SentenceWordTranscriptions)
                OutputList.Add(SentenceTranscription)

            Next
            'Adding an empty line between lists
            OutputList.Add("")

        Next

        'Displayes the results in the EditRichTextBox
        EditRichTextBox.Lines = OutputList.ToArray

    End Sub


End Class
