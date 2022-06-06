Public Class SpeechMaterialComponentCreator

    'N.B. Much of the code in this class could be moved into shared functions of the class SpeechMaterialComponent, and thus be more generally accessible.

    Private rnd As New Random


    'The following two default string values could possibly (however unlikely, as long as the user use IPA characters in the phonetic transcriptions) create problems, and could instead be allowed to be set by the user
    Private DefaultNotFoundTranscriptionString As String = "---"
    Private DefaultAmbigousTranscriptionMarker As String = "/"


    Private Sub SpeechMaterialComponentCreator_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.List)
        SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Sentence)
        SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Word)
        SoundFileLevelComboBox.Items.Add(SpeechMaterialComponent.LinguisticLevels.Phoneme)

        'Adding default characters into the word level spelling trim text box
        Dim DefaultWordTrimChars As New List(Of Char) From {".", ",", "?", "!", ";", ":", Chr(34), Chr(39), Chr(60), Chr(62)}
        For UnicodePoint = 8216 To 8223
            DefaultWordTrimChars.Add(ChrW(UnicodePoint))
        Next
        DefaultWordTrimChars.Add(ChrW(8242))
        DefaultWordTrimChars.Add(ChrW(8243))
        DefaultWordTrimChars.Add(ChrW(8245))
        DefaultWordTrimChars.Add(ChrW(8246))
        DefaultWordTrimChars.Add(ChrW(8249))
        DefaultWordTrimChars.Add(ChrW(8250))
        WordTrimChars_TextBox.Text = String.Join(" ", DefaultWordTrimChars)

        'Adding default characters into phone level transcription trim text box
        Dim IpaMainStress As Char = "ˈ"
        Dim IpaMainSwedishAccent2 As Char = "²"
        Dim IpaSecondaryStress As Char = "ˌ"
        Dim IpaSyllableBoundary As Char = "."
        Dim IpaLinkingSymbol As Char = "‿"
        Dim IpaMinorFootGroup As Char = "|"
        Dim IpaMajorIntonationGroup As Char = "‖"
        Dim DefaultPhoneTrimChars As New List(Of Char) From {",", IpaMainStress, IpaSecondaryStress, IpaSyllableBoundary, IpaMainSwedishAccent2, IpaLinkingSymbol, IpaMinorFootGroup, IpaMajorIntonationGroup}
        PhoneTrimChars_TextBox.Text = String.Join(" ", DefaultPhoneTrimChars)

    End Sub

    Private Sub CheckInputButton_Click(sender As Object, e As EventArgs) Handles CheckInputButton.Click

        If CheckInput().Item1 = True Then
            'Updates the textbox with the new data
            MsgBox("All checks passed for creating a speech material component file.", MsgBoxStyle.Information, "Checking input data")
            CreateSpeechMaterialComponentFile_Button.Enabled = True
            TranscriptionLookupButton.Enabled = True
        Else
            CreateSpeechMaterialComponentFile_Button.Enabled = False
            TranscriptionLookupButton.Enabled = False
        End If

    End Sub

    Private Function CheckInput() As Tuple(Of Boolean, SpeechMaterialComponent)

        'Parsing characters to trim off of word level spellings 
        Dim WordTrimCharArray = WordTrimChars_TextBox.Text.Replace(" ", "").ToCharArray
        Dim WordLevelTrimChars As New SortedSet(Of Char)
        For Each c In WordTrimCharArray
            If WordLevelTrimChars.Contains(c) = False Then WordLevelTrimChars.Add(c)
        Next

        'Parsing characters to trim off of phone level transcriptions
        Dim PhoneTrimCharArray = PhoneTrimChars_TextBox.Text.Replace(" ", "").ToCharArray
        Dim PhoneCharsToRemoveList As New SortedSet(Of Char)
        For Each c In PhoneTrimCharArray
            If PhoneCharsToRemoveList.Contains(c) = False Then PhoneCharsToRemoveList.Add(c)
        Next
        'Finally also adding the space character to the phoneme trim list (as this is not added manually)
        PhoneCharsToRemoveList.Add(" ")

        'Parsing the info about which level sound recording should be used
        Dim SoundFilesAtLevel As SpeechMaterialComponent.LinguisticLevels
        If [Enum].TryParse(SoundFileLevelComboBox.SelectedItem, SoundFilesAtLevel) = False Then
            MsgBox("Select a value for 'Sound files at linguistic level'.", MsgBoxStyle.Information, "Checking input data")
            Return New Tuple(Of Boolean, SpeechMaterialComponent)(False, Nothing)
        End If

        'Checking that we have a name of the list collection speech material component
        If NameTextBox.Text = "" Then
            MsgBox("Add a name", MsgBoxStyle.Information, "Checking input data")
            Return New Tuple(Of Boolean, SpeechMaterialComponent)(False, Nothing)
        End If

        'Getting the input lines
        Dim Input = EditRichTextBox.Lines

        'The whole input represent a ListCollection
        Dim CurrentListCollectionComponent As New SpeechMaterialComponent(rnd) With {
            .Id = NameTextBox.Text,
            .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.ListCollection,
            .PrimaryStringRepresentation = NameTextBox.Text,
            .CustomVariablesDatabasePath = SpeechMaterialComponent.SpeechMaterialLevelDatabaseName}

        CurrentListCollectionComponent.DbId = CurrentListCollectionComponent.Id
        CurrentListCollectionComponent.PrimaryStringRepresentation = CurrentListCollectionComponent.Id

        CurrentListCollectionComponent.SetCategoricalVariableValue("DbId", CurrentListCollectionComponent.Id)

        Dim CurrentListComponent As SpeechMaterialComponent = Nothing
        Dim CurrentSentenceComponent As SpeechMaterialComponent = Nothing

        Dim NumberOfSentenceLines As Integer = 0
        Dim NumberOfTranscriptionLines As Integer = 0

        Dim FirstListDetected As Boolean = False

        'Parsing input lines
        For i = 0 To Input.Length - 1

            Dim CurrentLine = Input(i).Trim

            'Skips empty lines
            If CurrentLine = "" Then Continue For

            If FirstListDetected = False Then
                If CurrentLine.Trim.StartsWith("{") = True Then
                    FirstListDetected = True
                Else
                    MsgBox("The first input line must start with a list name within curly brackets, then followed by the test items, for example:" & vbCrLf & vbCrLf &
                           "{Word list 1}" & vbCrLf & "The old man saw a bird" & vbCrLf & "The young girl found a lizard" & vbCrLf & "...", MsgBoxStyle.Information, "Missing initial list name")
                    Return New Tuple(Of Boolean, SpeechMaterialComponent)(False, Nothing)
                End If
            End If


            If CurrentLine.StartsWith("{") Then

                Dim ListName As String = CurrentLine.Trim().TrimStart("{").TrimEnd("}")

                'A new List
                CurrentListComponent = New SpeechMaterialComponent(rnd) With {
                    .ParentComponent = CurrentListCollectionComponent,
                    .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.List,
                    .CustomVariablesDatabasePath = SpeechMaterialComponent.ListLevelDataBaseName}

                CurrentListCollectionComponent.ChildComponents.Add(CurrentListComponent)

                CurrentListComponent.Id = "L" & (CurrentListCollectionComponent.ChildComponents.Count - 1).ToString("00")
                CurrentListComponent.DbId = CurrentListComponent.Id
                CurrentListComponent.PrimaryStringRepresentation = ListName
                If SoundFilesAtLevel = SpeechMaterialComponent.LinguisticLevels.List Then CurrentListComponent.MediaFolder = CurrentListComponent.Id & "_" & CurrentListComponent.PrimaryStringRepresentation.Replace(" ", "_")
                CurrentListComponent.OrderedChildren = OrderedSentencesCheckBox.Checked

                CurrentListComponent.SetCategoricalVariableValue("DbId", CurrentListComponent.DbId)
                CurrentListComponent.SetCategoricalVariableValue("ListName", ListName)

            ElseIf CurrentLine.StartsWith("[") Then

                'It should be the phonetic/phonemic transcription of the current sentence

                'Checking if there are any non-transcribed or ambingously transcribed words before continuing
                If CurrentLine.Contains(DefaultNotFoundTranscriptionString) = True Then
                    MsgBox("The following input line (line " & i + 1 & ") lacks the transcription for at least one word. Transcribe it manually and then try again." & vbCrLf & vbCrLf &
                           CurrentLine, MsgBoxStyle.Information, "Checking input data")
                    Return New Tuple(Of Boolean, SpeechMaterialComponent)(False, Nothing)
                End If
                If CurrentLine.Contains(DefaultAmbigousTranscriptionMarker) = True Then
                    MsgBox("The following input line (line " & i + 1 & ") have more than one transcription (marked by the character " & DefaultAmbigousTranscriptionMarker &
                           " ). Select one transcription by manually removing the others and also remove the character " & DefaultAmbigousTranscriptionMarker & ") and then try again." & vbCrLf & vbCrLf &
                           CurrentLine, MsgBoxStyle.Information, "Checking input data")
                    Return New Tuple(Of Boolean, SpeechMaterialComponent)(False, Nothing)
                End If

                'Adds the transcriptions
                CurrentSentenceComponent.SetCategoricalVariableValue(SpeechMaterialComponent.DefaultTranscriptionVariableName, CurrentLine)

                Dim WordTranscriptions = CurrentLine.Split(",")
                'Checks that the number of transcriptions and spellings agree
                If CurrentSentenceComponent.ChildComponents.Count <> WordTranscriptions.Length Then
                    MsgBox("The number of transcriptions at the following line (line " & i + 1 & ") do do agree with the number of speelings in the corresponding sentence. Have you missed a comma between word transcriptions?" & vbCrLf & vbCrLf &
                   CurrentSentenceComponent.GetCategoricalVariableValue(SpeechMaterialComponent.DefaultSpellingVariableName) & vbCrLf & CurrentLine, MsgBoxStyle.Information, "Checking input data")
                    Return New Tuple(Of Boolean, SpeechMaterialComponent)(False, Nothing)
                End If

                'Notes a new sentence transcription
                NumberOfTranscriptionLines += 1

                For w = 0 To WordTranscriptions.Length - 1
                    CurrentSentenceComponent.ChildComponents(w).SetCategoricalVariableValue(SpeechMaterialComponent.DefaultTranscriptionVariableName, WordTranscriptions(w).Trim.TrimStart("[").TrimEnd("]".Trim))

                    'Adding phonetic transcription components, taken from the transcription (phonemes need to be separated by black spaces)
                    Dim Phonemes = CurrentSentenceComponent.ChildComponents(w).GetCategoricalVariableValue(SpeechMaterialComponent.DefaultTranscriptionVariableName).Split(" ")
                    Dim PhonemesToAdd As New List(Of String)

                    For p = 0 To Phonemes.Length - 1

                        Dim CurrentPhoneme = Phonemes(p)

                        'Removes non-phoneme characters
                        For Each s In PhoneCharsToRemoveList
                            CurrentPhoneme = CurrentPhoneme.Replace(s, "")
                        Next
                        If CurrentPhoneme = "" Then Continue For

                        PhonemesToAdd.Add(CurrentPhoneme)

                    Next

                    If PhonemesToAdd.Count > 0 Then

                        'Adds zero phones (markers of empty word-initial syllable onsets and word-final syllable codas)
                        If AddZeroPhoneme_CheckBox.Checked = True Then
                            'Adds a ZeroPhoneme to word final empty syllable codas
                            If IPA.Vowels.Contains(IPA.RemoveLengthMarkers(PhonemesToAdd(PhonemesToAdd.Count - 1))) Then PhonemesToAdd.Add(IPA.ZeroPhoneme)

                            'Adds a ZeroPhoneme to word initial empty syllable onsets
                            If IPA.Vowels.Contains(IPA.RemoveLengthMarkers(PhonemesToAdd(0))) Then PhonemesToAdd.Insert(0, IPA.ZeroPhoneme)
                        End If

                        'Adds the phoneme level components
                        For p = 0 To PhonemesToAdd.Count - 1

                                Dim CurrentPhoneme As String = PhonemesToAdd(p)

                                Dim NewPhonemeComponent = New SpeechMaterialComponent(rnd) With {
                                    .ParentComponent = CurrentSentenceComponent.ChildComponents(w),
                                    .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Phoneme,
                                    .CustomVariablesDatabasePath = SpeechMaterialComponent.PhonemeLevelDataBaseName}

                                CurrentSentenceComponent.ChildComponents(w).ChildComponents.Add(NewPhonemeComponent)

                                NewPhonemeComponent.Id = CurrentSentenceComponent.ChildComponents(w).Id & "P" & p.ToString("00")
                                NewPhonemeComponent.DbId = NewPhonemeComponent.Id
                                NewPhonemeComponent.PrimaryStringRepresentation = CurrentPhoneme
                                If SoundFilesAtLevel = SpeechMaterialComponent.LinguisticLevels.Phoneme Then NewPhonemeComponent.MediaFolder = NewPhonemeComponent.Id & "_" & NewPhonemeComponent.PrimaryStringRepresentation.Replace(" ", "_")

                                NewPhonemeComponent.SetCategoricalVariableValue("DbId", NewPhonemeComponent.DbId)
                                NewPhonemeComponent.SetCategoricalVariableValue(SpeechMaterialComponent.DefaultTranscriptionVariableName, CurrentPhoneme)

                            Next

                        End If

                Next

            Else

                'It should be sentence
                CurrentSentenceComponent = New SpeechMaterialComponent(rnd) With {
                    .ParentComponent = CurrentListComponent,
                    .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Sentence,
                            .CustomVariablesDatabasePath = SpeechMaterialComponent.SentenceLevelDataBaseName}

                CurrentListComponent.ChildComponents.Add(CurrentSentenceComponent)

                CurrentSentenceComponent.Id = CurrentListComponent.Id & "S" & (CurrentListComponent.ChildComponents.Count - 1).ToString("00")

                CurrentSentenceComponent.DbId = CurrentSentenceComponent.Id
                CurrentSentenceComponent.PrimaryStringRepresentation = "Sentence" & (CurrentListComponent.ChildComponents.Count - 1).ToString("00")
                If SoundFilesAtLevel = SpeechMaterialComponent.LinguisticLevels.Sentence Then CurrentSentenceComponent.MediaFolder = CurrentSentenceComponent.Id & "_" & CurrentSentenceComponent.PrimaryStringRepresentation.Replace(" ", "_")

                CurrentSentenceComponent.SetCategoricalVariableValue("DbId", CurrentSentenceComponent.DbId)
                CurrentSentenceComponent.SetCategoricalVariableValue(SpeechMaterialComponent.DefaultSpellingVariableName, CurrentLine)

                'Notes a new sentence
                NumberOfSentenceLines += 1

                'Adds the word components
                Dim Words = CurrentLine.Split(" ")
                Dim AddedWordIndex As Integer = -1
                For w = 0 To Words.Length - 1

                    Dim WordSpelling As String = Words(w)
                    'Trimming off characters
                    For Each c In WordLevelTrimChars
                        WordSpelling = WordSpelling.Replace(c, " ")
                    Next
                    'Skipping to next if all characters in the spelling were removed
                    If WordSpelling.Trim = "" Then Continue For

                    'Increasing the AddedWordIndex value
                    AddedWordIndex += 1

                    'Creating a new Word level component
                    Dim NewWordComponent = New SpeechMaterialComponent(rnd) With {
                        .ParentComponent = CurrentSentenceComponent,
                        .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Word,
                        .CustomVariablesDatabasePath = SpeechMaterialComponent.WordLevelDataBaseName}

                    CurrentSentenceComponent.ChildComponents.Add(NewWordComponent)

                    NewWordComponent.Id = CurrentSentenceComponent.Id & "W" & AddedWordIndex.ToString("00")

                    NewWordComponent.DbId = NewWordComponent.Id
                    NewWordComponent.PrimaryStringRepresentation = WordSpelling
                    If SoundFilesAtLevel = SpeechMaterialComponent.LinguisticLevels.Word Then NewWordComponent.MediaFolder = NewWordComponent.Id & "_" & NewWordComponent.PrimaryStringRepresentation.Replace(" ", "_")

                    NewWordComponent.SetCategoricalVariableValue("DbId", NewWordComponent.DbId)
                    NewWordComponent.SetCategoricalVariableValue(SpeechMaterialComponent.DefaultSpellingVariableName, WordSpelling)

                Next

            End If

        Next

        'Checking that all or no sentnces are transcribed
        If NumberOfTranscriptionLines > 0 Then
            If NumberOfSentenceLines <> NumberOfTranscriptionLines Then
                MsgBox("The number of sentence level inputs lines (" & NumberOfSentenceLines & ") do not agree with the number of transcription lines (" & NumberOfTranscriptionLines &
                       "). Have you missed to transcribe some lines, or mistakenly removed the [ characters from the beginning of a transcription line? ", MsgBoxStyle.Information, "Checking input data")
                Return New Tuple(Of Boolean, SpeechMaterialComponent)(False, Nothing)
            End If
        End If

        Return New Tuple(Of Boolean, SpeechMaterialComponent)(True, CurrentListCollectionComponent)


    End Function


    Public Sub CreateSpeechMaterialComponents() Handles CreateSpeechMaterialComponentFile_Button.Click

        'Re-checking the input and gets the SmaComponent 
        Dim NewSmaComponent = CheckInput()
        If NewSmaComponent.Item1 = False Then
            CreateSpeechMaterialComponentFile_Button.Enabled = False
            TranscriptionLookupButton.Enabled = False
            Exit Sub
        End If

        'Saving to file
        Dim FilePath = Utils.GetSaveFilePath(,, {".txt"}, "Save speech material components file")
        NewSmaComponent.Item2.WriteSpeechMaterialComponenFile(FilePath)

        'Also creating a  saving a new test specification file
        MsgBox("Debug the following lines, they've not been tested!")
        Dim NewTestSpecification As New TestSpecification(NewSmaComponent.Item2.Id, "", "")
        Dim TsFilePath = IO.Path.Combine(IO.Path.GetDirectoryName(FilePath), NewSmaComponent.Item2.Id & "_TestSpecificationFile(Put this in the " & OstfSettings.TestSpecificationSubFolder & " folder).txt")
        NewTestSpecification.WriteTextFile()

    End Sub

    Private Sub TranscriptionLookupButton_Click(sender As Object, e As EventArgs) Handles TranscriptionLookupButton.Click

        'Checks if transcriptions already exist
        If EditRichTextBox.Text.Contains("[") Then

            Dim Result = MsgBox("It looks like you have already looked up some transcriptions. In order to avoid errors, all lines containing the character [ will be removed from the input before attempting to lookup transcriptions. Press Ok to proceed or Cancel to modify the input manually.", MsgBoxStyle.OkCancel, "Found existing transription markers")
            If Result = MsgBoxResult.Ok Then

                Dim LinesToKeep As New List(Of String)
                Dim CurrentInputLines = EditRichTextBox.Lines
                For Each line In CurrentInputLines
                    If line.Contains("[") = False Then LinesToKeep.Add(line)
                Next
                EditRichTextBox.Lines = LinesToKeep.ToArray
                EditRichTextBox.Update()
            Else
                Exit Sub
            End If
        End If


        'Re-checking the input and gets the SmaComponent 
        Dim NewSmaComponent = CheckInput()
        If NewSmaComponent.Item1 = False Then
            CreateSpeechMaterialComponentFile_Button.Enabled = False
            TranscriptionLookupButton.Enabled = False
            Exit Sub
        End If

        'Gets all spellings used (at the word level) in the speech material
        Dim ExistingSpellingsList As New SortedSet(Of String)
        Dim WordLevelComponents = NewSmaComponent.Item2.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.Word)
        For Each Component In WordLevelComponents
            Dim Spelling = Component.GetCategoricalVariableValue(SpeechMaterialComponent.DefaultSpellingVariableName)
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
            Dim Spelling = Component.GetCategoricalVariableValue(SpeechMaterialComponent.DefaultSpellingVariableName)

            If CaseInvariantLookupCheckBox.Checked = True Then
                Spelling = Spelling.ToLower
            End If
            Dim MatchingSpellings = LookupList.FindAll(Function(x) x.Item1 = Spelling)
            Dim PossibleTranscriptions = New List(Of String)
            For Each MatchingSpelling In MatchingSpellings
                PossibleTranscriptions.Add(MatchingSpelling.Item2)
            Next

            'Creating a string which, if there are more than one possible transcription, the user will have to reduce to one manually. The / character denotes multiple transcriptions and will not be allowed when parsing the input again.
            Dim TranscriptionString As String = String.Join(" " & DefaultAmbigousTranscriptionMarker & " ", PossibleTranscriptions)

            If TranscriptionString <> "" Then
                Component.SetCategoricalVariableValue(SpeechMaterialComponent.DefaultTranscriptionVariableName, TranscriptionString)
            Else
                'The spelling (and transcription) was not found. Outputting a the DefaultNotFoundTranscriptionString, which should be replaced manually by the user, and should be not be allowed when parsing the input again.
                Component.SetCategoricalVariableValue(SpeechMaterialComponent.DefaultTranscriptionVariableName, DefaultNotFoundTranscriptionString)
            End If
        Next

        'If all went well, we print the text back to the user, with the transcriptions interlined
        Dim OutputList As New List(Of String)
        Dim ListsComponents = NewSmaComponent.Item2.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)
        For Each ListComponent In ListsComponents

            OutputList.Add("{" & ListComponent.PrimaryStringRepresentation & "}")

            For Each SentenceComponent In ListComponent.ChildComponents
                OutputList.Add(SentenceComponent.GetCategoricalVariableValue(SpeechMaterialComponent.DefaultSpellingVariableName))

                Dim SentenceWordTranscriptions As New List(Of String)
                For Each WordComponent In SentenceComponent.ChildComponents
                    SentenceWordTranscriptions.Add("[ " & WordComponent.GetCategoricalVariableValue(SpeechMaterialComponent.DefaultTranscriptionVariableName) & " ]")
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

    Private Sub MenuStrip1_ItemClicked(sender As Object, e As Windows.Forms.ToolStripItemClickedEventArgs) Handles MenuStrip1.ItemClicked

        Dim fd As New Windows.Forms.FontDialog
        Dim Res = fd.ShowDialog
        If Res = Windows.Forms.DialogResult.OK Then
            EditRichTextBox.Font = fd.Font
        End If

    End Sub
End Class
