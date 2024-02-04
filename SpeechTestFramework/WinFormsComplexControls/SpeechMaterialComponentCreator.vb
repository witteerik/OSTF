Public Class SpeechMaterialComponentCreator

    'N.B. Much of the code in this class could be moved into shared functions of the class SpeechMaterialComponent, and thus be more generally accessible.

    Private rnd As New Random


    'The following two default string values could possibly (however unlikely, as long as the user use IPA characters in the phonetic transcriptions) create problems, and could instead be allowed to be set by the user
    Private DefaultNotFoundTranscriptionString As String = "---"
    Private DefaultAmbigousTranscriptionMarker As String = "/"


    Private Sub SpeechMaterialComponentCreator_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try

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

            'Setting font in the EditRichTextBox
            EditRichTextBox.Font = New Drawing.Font("Arial", 12, Drawing.FontStyle.Regular)

        Catch ex As Exception
            MsgBox("The following error occured: " & vbCrLf & vbCrLf & ex.ToString)
        End Try

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

        'Checking that we have a name of the list collection speech material component
        If NameTextBox.Text = "" Then
            MsgBox("Add a name", MsgBoxStyle.Information, "Checking input data")
            Return New Tuple(Of Boolean, SpeechMaterialComponent)(False, Nothing)
        End If

        'Getting the input text and parses through its rtf underlining specification in order to mark key words (which should be underlined)
        Dim LextLength As Integer = EditRichTextBox.TextLength()

        'Getting the text and its underlining, character by character
        Dim InputTextList As New List(Of Char)
        Dim InputUnderLineList As New List(Of Boolean)
        For n = 0 To LextLength - 1
            EditRichTextBox.Select(n, 1)
            'Notes if the selected char is underlined
            InputUnderLineList.Add(EditRichTextBox.SelectionFont.Underline)
            InputTextList.Add(EditRichTextBox.Text(n))
        Next

        If InputTextList.Count > 0 Then
            'If the text does not start with an initial line break, we insert one, to regularize the text.
            If InputTextList.First = vbCrLf Or InputTextList.First = vbCr Or InputTextList.First = vbLf Then
                'There is already a line break at the start of the text
            Else
                InputTextList.Insert(0, vbCrLf)
                InputUnderLineList.Insert(0, False)
            End If

            'If the text does not end with a final line break, we insert one, to regularize the text.
            If InputTextList.Last = vbCrLf Or InputTextList.Last = vbCr Or InputTextList.Last = vbLf Then
                'There is already a line break at the end of the text
            Else
                InputTextList.Add(vbCrLf)
                InputUnderLineList.Add(False)
            End If
        End If

        'Getting the indices of line breaks
        Dim LineBreakIndices As New SortedSet(Of Integer)
        For n = 0 To InputTextList.Count - 1
            If InputTextList(n) = vbCrLf Or InputTextList(n) = vbCr Or InputTextList(n) = vbLf Then
                LineBreakIndices.Add(n)
            End If
        Next

        'Parsing lines into lists of words and underlining, still character by character
        Dim WordLineChars As New List(Of List(Of Char))
        Dim UnderLinedChars As New List(Of List(Of Boolean))
        Dim WordBreakIndices As New List(Of SortedSet(Of Integer))
        Dim WordBreakCharacter As Char

        For n = 0 To LineBreakIndices.Count - 2
            Dim StartReadIndex = LineBreakIndices(n) + 1
            Dim ReadLength = LineBreakIndices(n + 1) - LineBreakIndices(n) - 1
            Dim CurrentLineChars = InputTextList.GetRange(StartReadIndex, ReadLength)
            Dim CurrentLineUnderlineData = InputUnderLineList.GetRange(StartReadIndex, ReadLength)

            Dim TrimmedLineAsString = String.Concat(CurrentLineChars).Trim
            If TrimmedLineAsString.StartsWith("{") Then
                'It should be a list name, using space as WordBreakCharacter 'TODO, this could be generalized to WhiteSpace
                WordBreakCharacter = " "
            ElseIf TrimmedLineAsString.StartsWith("[") Then
                'It should be phonetic form, using comma as WordBreakCharacter 
                WordBreakCharacter = ","
            Else
                'It should be a sentence, using space as WordBreakCharacter 'TODO, this could be generalized to WhiteSpace
                WordBreakCharacter = " "
            End If

            If CurrentLineChars.Count > 0 Then
                'Padding the beginning of the line with a WordBreakCharacter, if needed, in order to regularize the text
                If CurrentLineChars.First <> WordBreakCharacter Then
                    CurrentLineChars.Insert(0, WordBreakCharacter)
                    CurrentLineUnderlineData.Insert(0, False)
                End If

                'Padding the line end with a WordBreakCharacter, if needed, in order to regularize the text
                If CurrentLineChars.Last <> WordBreakCharacter Then
                    CurrentLineChars.Add(WordBreakCharacter)
                    CurrentLineUnderlineData.Add(False)
                End If
            End If

            'Getting the indices of all word breaks
            Dim CurrentLineWordBreakIndices As New SortedSet(Of Integer)
            For c = 0 To CurrentLineChars.Count - 1
                If CurrentLineChars(c) = WordBreakCharacter Then
                    CurrentLineWordBreakIndices.Add(c)
                End If
            Next

            WordLineChars.Add(CurrentLineChars)
            UnderLinedChars.Add(CurrentLineUnderlineData)
            WordBreakIndices.Add(CurrentLineWordBreakIndices)
        Next

        'Splitting the data into one string arrays (containing one word in eacg string) per line, and a corresponding boolean array per line (containing True/False for the underlining of each word)
        Dim WordLines As New List(Of String())
        Dim UnderLineInfo As New List(Of Boolean())

        For Line = 0 To WordLineChars.Count - 1

            Dim WordBreakIndicesArray = WordBreakIndices(Line).ToArray
            Dim WordLineCharsList = WordLineChars(Line)
            Dim UnderLinedCharsList = UnderLinedChars(Line)

            Dim Words As New List(Of String)
            Dim Underlining As New List(Of Boolean)

            For n = 0 To WordBreakIndicesArray.Length - 2
                Dim StartReadIndex = WordBreakIndicesArray(n) + 1
                Dim ReadLength = WordBreakIndicesArray(n + 1) - WordBreakIndicesArray(n) - 1

                Words.Add(String.Concat(WordLineCharsList.GetRange(StartReadIndex, ReadLength)))

                'Getting the underlining data from the first character in the word, and ignores all other underlining (Underlining should be binary for each word)
                Underlining.Add(UnderLinedCharsList.GetRange(StartReadIndex, ReadLength).First)
            Next

            WordLines.Add(Words.ToArray)
            UnderLineInfo.Add(Underlining.ToArray)
        Next


        'Dim Input = EditRichTextBox.Lines

        'The whole input represent a ListCollection
        Dim CurrentListCollectionComponent As New SpeechMaterialComponent(rnd) With {
            .Id = NameTextBox.Text,
            .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.ListCollection,
            .PrimaryStringRepresentation = NameTextBox.Text}

        CurrentListCollectionComponent.PrimaryStringRepresentation = CurrentListCollectionComponent.Id

        Dim CurrentListComponent As SpeechMaterialComponent = Nothing
        Dim CurrentSentenceComponent As SpeechMaterialComponent = Nothing

        Dim NumberOfSentenceLines As Integer = 0
        Dim NumberOfTranscriptionLines As Integer = 0

        Dim FirstListDetected As Boolean = False

        'Parsing input lines
        'For i = 0 To Input.Length - 1
        For i = 0 To WordLines.Count - 1

            Dim TrimmedLineAsString = String.Concat(WordLines(i)).Trim
            If TrimmedLineAsString.StartsWith("{") Then
                'It should be a list name, and space should be used as WordBreakCharacter
                WordBreakCharacter = " "
            ElseIf TrimmedLineAsString.StartsWith("[") Then
                'It should be phonetic form, and comma should be used as WordBreakCharacter 
                WordBreakCharacter = ","
            Else
                'It should be a sentence, and space should be used as WordBreakCharacter
                WordBreakCharacter = " "
            End If

            Dim CurrentLine = String.Join(WordBreakCharacter, WordLines(i)).Trim

            'Dim CurrentLine = Input(i).Trim


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
                    .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.List}

                CurrentListCollectionComponent.ChildComponents.Add(CurrentListComponent)

                CurrentListComponent.Id = "L" & (CurrentListCollectionComponent.ChildComponents.Count - 1).ToString("00")
                CurrentListComponent.PrimaryStringRepresentation = ListName

                CurrentListComponent.SetCategoricalVariableValue(SpeechMaterialComponent.DefaultListNameVariableName, ListName)

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
                    MsgBox("The number of transcriptions at the following line (line " & i + 1 & ") do do agree with the number of spellings in the corresponding sentence. Have you missed a comma between word transcriptions?" & vbCrLf & vbCrLf &
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
                                    .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Phoneme}

                            CurrentSentenceComponent.ChildComponents(w).ChildComponents.Add(NewPhonemeComponent)

                            NewPhonemeComponent.Id = CurrentSentenceComponent.ChildComponents(w).Id & "P" & p.ToString("00")
                            NewPhonemeComponent.PrimaryStringRepresentation = CurrentPhoneme

                            NewPhonemeComponent.SetCategoricalVariableValue(SpeechMaterialComponent.DefaultTranscriptionVariableName, CurrentPhoneme)

                        Next

                    End If

                Next

            Else

                'It should be sentence
                CurrentSentenceComponent = New SpeechMaterialComponent(rnd) With {
                    .ParentComponent = CurrentListComponent,
                    .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Sentence}

                CurrentListComponent.ChildComponents.Add(CurrentSentenceComponent)

                CurrentSentenceComponent.Id = CurrentListComponent.Id & "S" & (CurrentListComponent.ChildComponents.Count - 1).ToString("00")

                CurrentSentenceComponent.PrimaryStringRepresentation = SpeechMaterialComponent.DefaultSentencePrefix & (CurrentListComponent.ChildComponents.Count - 1).ToString("00")

                CurrentSentenceComponent.SetCategoricalVariableValue(SpeechMaterialComponent.DefaultSpellingVariableName, CurrentLine)

                'Notes a new sentence
                NumberOfSentenceLines += 1

                'Adds the word components
                'Dim Words = CurrentLine.Split(" ")
                Dim Words = WordLines(i)
                Dim AddedWordIndex As Integer = -1
                For w = 0 To Words.Length - 1

                    Dim WordSpelling As String = Words(w)

                    'Trimming off characters
                    WordSpelling = WordSpelling.TrimStart(WordLevelTrimChars.ToArray)
                    WordSpelling = WordSpelling.TrimEnd(WordLevelTrimChars.ToArray)

                    'For Each c In WordLevelTrimChars
                    '    WordSpelling = WordSpelling.TrimStart(c)
                    '    WordSpelling = WordSpelling.TrimEnd(c)
                    '    'WordSpelling = WordSpelling.Replace(c, " ")
                    'Next
                    'Skipping to next if all characters in the spelling were removed
                    If WordSpelling.Trim = "" Then Continue For

                    'Increasing the AddedWordIndex value
                    AddedWordIndex += 1

                    'Creating a new Word level component
                    Dim NewWordComponent = New SpeechMaterialComponent(rnd) With {
                        .ParentComponent = CurrentSentenceComponent,
                        .LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Word}

                    CurrentSentenceComponent.ChildComponents.Add(NewWordComponent)

                    NewWordComponent.Id = CurrentSentenceComponent.Id & "W" & AddedWordIndex.ToString("00")

                    NewWordComponent.PrimaryStringRepresentation = WordSpelling

                    NewWordComponent.SetCategoricalVariableValue(SpeechMaterialComponent.DefaultSpellingVariableName, WordSpelling)

                    Dim IsUnderline As Boolean = UnderLineInfo(i)(w)
                    Dim IsUnderlineAsInteger As Integer
                    'Overrideing the normal VB convertion of Boolean True to -1, and use instead 1 as True, and (0 as False)
                    If IsUnderline = False Then
                        IsUnderlineAsInteger = 0
                    Else
                        IsUnderlineAsInteger = 1
                    End If

                    NewWordComponent.SetNumericVariableValue(SpeechMaterialComponent.DefaultIsKeyComponentVariableName, IsUnderlineAsInteger)

                Next

            End If

        Next

        'Checking that all or no sentences are transcribed
        If NumberOfTranscriptionLines > 0 Then
            If NumberOfSentenceLines <> NumberOfTranscriptionLines Then
                MsgBox("The number of sentence level inputs lines (" & NumberOfSentenceLines & ") do not agree with the number of transcription lines (" & NumberOfTranscriptionLines &
                       "). Have you missed to transcribe some lines, or mistakenly removed the [ characters from the beginning of a transcription line? ", MsgBoxStyle.Information, "Checking input data")
                Return New Tuple(Of Boolean, SpeechMaterialComponent)(False, Nothing)
            End If
        End If

        'Storing setup values (these are only stored at the very top level component!)
        CurrentListCollectionComponent.SequentiallyOrderedLists = SequentialLists_CheckBox.Checked
        CurrentListCollectionComponent.SequentiallyOrderedSentences = SequentialSentences_CheckBox.Checked
        CurrentListCollectionComponent.SequentiallyOrderedWords = SequentialWords_CheckBox.Checked
        CurrentListCollectionComponent.SequentiallyOrderedPhonemes = SequentialPhonemes_CheckBox.Checked

        'Assigning Id as a categorical variable to all components
        CurrentListCollectionComponent.SetIdAsCategoricalCustumVariable(True)

        MarkIncompleteTranscriptionsInRed()

        Return New Tuple(Of Boolean, SpeechMaterialComponent)(True, CurrentListCollectionComponent)

    End Function


    Public Sub CreateSpeechMaterialComponents() Handles CreateSpeechMaterialComponentFile_Button.Click

        'Re-checking the input and gets the Speech material Component 
        Dim NewSMComponent = CheckInput()
        If NewSMComponent.Item1 = False Then
            CreateSpeechMaterialComponentFile_Button.Enabled = False
            TranscriptionLookupButton.Enabled = False
            Exit Sub
        End If

        'Creating and saving a new test specification file
        'Getting a save folder if not supplied by the calling code
        Dim OutputParentFolder As String = ""
        Dim fbd As New Windows.Forms.FolderBrowserDialog
        fbd.Description = "Select a folder in which to save/create the exported directories and files"
        If fbd.ShowDialog() = Windows.Forms.DialogResult.OK Then
            OutputParentFolder = fbd.SelectedPath
        Else
            MsgBox("No output folder selected!", MsgBoxStyle.Exclamation, "Saving speech material files")
            Exit Sub
        End If

        If OutputParentFolder.Trim = "" Then
            MsgBox("No output folder selected!", MsgBoxStyle.Exclamation, "Saving speech material files")
            Exit Sub
        End If

        'Getting the test Id
        Dim TestId As String = NewSMComponent.Item2.Id

        'Creating a test folder folder name based on the test Id (with allowed characters)
        Dim NewTestSpecificationDirectoryNameCharArray() As Char = TestId.Replace(" ", "_").ToCharArray
        Dim NewTestSpecificationDirectoryNameList As New List(Of Char)
        For Each character In NewTestSpecificationDirectoryNameCharArray
            If IO.Path.GetInvalidPathChars.Contains(character) = False Then
                NewTestSpecificationDirectoryNameList.Add(character)
            Else
                NewTestSpecificationDirectoryNameList.Add("_")
            End If
        Next
        Dim NewTestSpecificationDirectoryName As String = String.Concat(NewTestSpecificationDirectoryNameList)

        'Creates the new SpeechMaterialSpecification
        Dim NewTestSpecification As New SpeechMaterialSpecification(TestId, NewTestSpecificationDirectoryName)

        'Noting where to save the test specification
        Dim TestSpecificationFullPath = IO.Path.Combine(OutputParentFolder, SpeechMaterialSpecification.AvailableSpeechMaterialsDirectory, TestId & "_TestSpecificationFile (Put this in the " & OstfBase.AvailableSpeechMaterialsSubFolder & " folder).txt")
        'Saving the test specification file
        NewTestSpecification.WriteTextFile(TestSpecificationFullPath)

        'Saving the speech material files
        NewSMComponent.Item2.WriteSpeechMaterialToFile(NewTestSpecification, OutputParentFolder)


        MsgBox("Your files should now have been created and save to the folder: " & OutputParentFolder, MsgBoxStyle.Information, "Creating files")


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
        EditRichTextBox.Clear()
        Dim CurrentWriteIndex As Integer = 0
        Dim ListsComponents = NewSmaComponent.Item2.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)
        For Each ListComponent In ListsComponents

            Dim ListString As String = "{" & ListComponent.PrimaryStringRepresentation & "}" & vbCr
            EditRichTextBox.AppendText(ListString)
            CurrentWriteIndex += ListString.Length

            For Each SentenceComponent In ListComponent.ChildComponents

                'Dim SentenceSpelling As String = SentenceComponent.GetCategoricalVariableValue(SpeechMaterialComponent.DefaultSpellingVariableName)
                'EditRichTextBox.AppendText(SentenceSpelling)
                'CurrentWriteIndex += SentenceSpelling.Length

                For w = 0 To SentenceComponent.ChildComponents.Count - 1

                    Dim WordComponent = SentenceComponent.ChildComponents(w)

                    Dim WordSpelling = WordComponent.GetCategoricalVariableValue(SpeechMaterialComponent.DefaultSpellingVariableName)

                    EditRichTextBox.AppendText(WordSpelling)
                    CurrentWriteIndex += WordSpelling.Length

                    EditRichTextBox.Select(CurrentWriteIndex - WordSpelling.Length, WordSpelling.Length)
                    If WordComponent.GetNumericVariableValue(SpeechMaterialComponent.DefaultIsKeyComponentVariableName) = True Then
                        EditRichTextBox.SelectionFont = New Drawing.Font(EditRichTextBox.Font, Drawing.FontStyle.Underline)
                    Else
                        EditRichTextBox.SelectionFont = New Drawing.Font(EditRichTextBox.Font, Drawing.FontStyle.Regular)
                    End If

                    If w < SentenceComponent.ChildComponents.Count - 1 Then
                        'Adding a space between words
                        EditRichTextBox.AppendText(" ")
                    Else
                        'Adding a line break
                        EditRichTextBox.AppendText(vbCr)
                    End If

                    CurrentWriteIndex += 1
                    EditRichTextBox.Select(CurrentWriteIndex - 1, 1)
                    EditRichTextBox.SelectionFont = New Drawing.Font(EditRichTextBox.Font, Drawing.FontStyle.Regular)

                Next

                EditRichTextBox.DeselectAll()

                Dim SentenceWordTranscriptions As New List(Of String)
                For Each WordComponent In SentenceComponent.ChildComponents
                    SentenceWordTranscriptions.Add("[ " & WordComponent.GetCategoricalVariableValue(SpeechMaterialComponent.DefaultTranscriptionVariableName) & " ]")
                Next

                Dim SentenceTranscription = String.Join(", ", SentenceWordTranscriptions) & vbCr
                EditRichTextBox.AppendText(SentenceTranscription)
                CurrentWriteIndex += SentenceTranscription.Length

            Next

            'Adding an empty line between lists
            EditRichTextBox.AppendText(vbCr)
            CurrentWriteIndex += 1

        Next

        MarkIncompleteTranscriptionsInRed()

    End Sub

    Private Sub SelectFontToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectFontToolStripMenuItem.Click

        Dim fd As New Windows.Forms.FontDialog
        Dim Res = fd.ShowDialog
        If Res = Windows.Forms.DialogResult.OK Then

            For n = 0 To EditRichTextBox.TextLength - 1

                EditRichTextBox.Select(n, 1)

                Dim PreviousFont = EditRichTextBox.SelectionFont
                EditRichTextBox.SelectionFont = New Drawing.Font(fd.Font, PreviousFont.Style)

            Next

        End If

    End Sub

    Private Sub UnderlineSelectedTextToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UnderlineSelectedTextToolStripMenuItem.Click

        If EditRichTextBox.SelectionLength > 0 Then
            Dim PreviousFont = EditRichTextBox.SelectionFont
            EditRichTextBox.SelectionFont = New Drawing.Font(PreviousFont.FontFamily, PreviousFont.Size, Drawing.FontStyle.Underline)
        End If

    End Sub

    Private Sub DeunderlineSelectedTextToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeunderlineSelectedTextToolStripMenuItem.Click

        If EditRichTextBox.SelectionLength > 0 Then
            Dim PreviousFont = EditRichTextBox.SelectionFont
            EditRichTextBox.SelectionFont = New Drawing.Font(PreviousFont.FontFamily, PreviousFont.Size, Drawing.FontStyle.Regular)
        End If

    End Sub

    Private Sub MarkIncompleteTranscriptionsInRed()

        Dim OriginalBackColor = EditRichTextBox.BackColor

        Dim MarkColor As Drawing.Color = Drawing.Color.FromArgb(248, 108, 108)

        Dim n As Integer = 0
        Do Until n >= EditRichTextBox.TextLength - 1

            EditRichTextBox.Select(n, 3)

            If EditRichTextBox.SelectedText = DefaultNotFoundTranscriptionString Then
                EditRichTextBox.SelectionBackColor = MarkColor
                n += 3
            Else
                EditRichTextBox.SelectionBackColor = OriginalBackColor
                n += 1
            End If
        Loop

        For m = 0 To EditRichTextBox.TextLength - 1

            EditRichTextBox.Select(m, 1)

            If EditRichTextBox.SelectionBackColor = MarkColor Then Continue For

            If EditRichTextBox.SelectedText = DefaultAmbigousTranscriptionMarker Then
                EditRichTextBox.SelectionBackColor = MarkColor
            Else
                EditRichTextBox.SelectionBackColor = OriginalBackColor
            End If
        Next

    End Sub

    Private Sub SaveWorkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveWorkToolStripMenuItem.Click

        Dim SaveDialog As New Windows.Forms.SaveFileDialog
        SaveDialog.Filter = "Rich text files (*.rtf)|*.rtf"
        SaveDialog.Title = "Save current work"
        Dim Result = SaveDialog.ShowDialog()
        If Result = Windows.Forms.DialogResult.OK Then
            EditRichTextBox.SaveFile(SaveDialog.FileName)
        End If

    End Sub

    Private Sub LoadWorkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadWorkToolStripMenuItem.Click

        Dim OpenDialog As New Windows.Forms.OpenFileDialog
        OpenDialog.Filter = "Rich text files (*.rtf)|*.rtf"
        OpenDialog.Title = "Open work (.rtf) file"

        Dim Result = OpenDialog.ShowDialog()
        If Result = Windows.Forms.DialogResult.OK Then
            EditRichTextBox.LoadFile(OpenDialog.FileName)
        End If

    End Sub

End Class
