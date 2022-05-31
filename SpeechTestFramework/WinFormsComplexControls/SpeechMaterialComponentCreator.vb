Public Class SpeechMaterialComponentCreator

    Private rnd As New Random

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

        Dim SpellingVariableName As String = "Spelling"
        Dim TranscriptionVariableName As String = "Transcription"

        Dim SpeechMaterialLevelDatabaseName As String = "SmLevelDatabase.txt"
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
                CurrentSentenceComponent.SetCategoricalWordMetricValue(TranscriptionVariableName, CurrentLine)

                Dim WordTranscriptions = CurrentLine.Split(",")
                'Checks that the number of transcriptions and spellings agree
                If CurrentSentenceComponent.ChildComponents.Count <> WordTranscriptions.Length Then
                    MsgBox("The number of transcriptions at line " & i + 1 & " do do agree with the number of speelings in the corresponding sentence: " & vbCrLf &
                   CurrentSentenceComponent.GetCategoricalWordMetricValue(SpellingVariableName) & vbCrLf & CurrentLine, MsgBoxStyle.Information, "Checking input data")
                    Return New Tuple(Of Boolean, SpeechMaterialComponent)(False, Nothing)
                End If

                For w = 0 To WordTranscriptions.Length - 1
                    CurrentSentenceComponent.ChildComponents(w).SetCategoricalWordMetricValue(TranscriptionVariableName, WordTranscriptions(w).Trim.TrimStart("[").TrimEnd("]".Trim))

                    'Adding phonetic transcription components, taken from the transcription (phonemes need to be separated by black spaces)
                    Dim Phonemes = CurrentSentenceComponent.ChildComponents(w).GetCategoricalWordMetricValue(TranscriptionVariableName).Split(" ")
                    For p = 0 To Phonemes.Length - 1

                        Dim CurrentPhoneme = Phonemes(p)

                        'Removes non-phoneme characters (TODO: this could instead be set by a list of valid phonetic characters, optinally set by the user?!?)
                        Dim IpaMainStress As String = "ˈ"
                        Dim IpaMainSwedishAccent2 As String = "²"
                        Dim IpaSecondaryStress As String = "ˌ"
                        Dim IpaSyllableBoundary As String = "."
                        Dim ReplacementList As New List(Of String) From {" ", ",", "", IpaMainStress, IpaSecondaryStress, IpaSyllableBoundary, IpaMainSwedishAccent2}
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
                        NewPhonemeComponent.SetCategoricalWordMetricValue(TranscriptionVariableName, CurrentPhoneme)

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
                CurrentSentenceComponent.SetCategoricalWordMetricValue(SpellingVariableName, CurrentLine)

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
                    NewWordComponent.SetCategoricalWordMetricValue(SpellingVariableName, Words(w))

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

        'Saving to file
        Dim FilePath = Utils.GetSaveFilePath(,, {".txt"}, "Save speech material components file")
        NewSmaComponent.Item2.WriteSpeechMaterialComponenFile(FilePath)

    End Sub


End Class
