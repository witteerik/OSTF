Imports MathNet.Numerics
Imports STFN.TestProtocol

Public Class MatrixSpeechTest
    Inherits SpeechAudiometryTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "Matrix"

    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub

    Public Shadows Sub ApplyTestSpecificSettings()

        TesterInstructions = ""
        ParticipantInstructions = ""

        ShowGuiChoice_PractiseTest = True
        ShowGuiChoice_StartList = True
        ShowGuiChoice_MediaSet = True
        AvailableTestModes = New List(Of TestModes) From {TestModes.AdaptiveNoise}
        'AvailableTestModes = New List(Of TestModes) From {TestModes.AdaptiveSpeech, TestModes.AdaptiveNoise}

        AvailableTestProtocols = New List(Of TestProtocol) From {New HagermanKinnefors1995_TestProtocol}
        'AvailableTestProtocols = New List(Of TestProtocol) From {New HagermanKinnefors1995_TestProtocol, New BrandKollmeier2002_TestProtocol}

        MaximumSoundFieldSpeechLocations = 1
        MaximumSoundFieldMaskerLocations = 1
        MinimumSoundFieldSpeechLocations = 1
        MinimumSoundFieldMaskerLocations = 1

        IsFreeRecall = True
        SupportsManualPausing = True

        TargetLevel = 65
        MaskingLevel = 65
        ContralateralMaskingLevel = 25

        MinimumLevel_Targets = 0
        MaximumLevel_Targets = 90
        MinimumLevel_Maskers = 0
        MaximumLevel_Maskers = 90
        MinimumLevel_ContralateralMaskers = 0
        MaximumLevel_ContralateralMaskers = 90

        SoundOverlapDuration = 0.1

        ShowGuiChoice_TargetLocations = True
        ShowGuiChoice_MaskerLocations = True

    End Sub


    ''' <summary>
    ''' This collection contains PlannedTestSentences which can be used troughout the test, in sequential order.
    ''' </summary>
    Private PlannedTestSentences As List(Of SpeechMaterialComponent)

    Private MaximumNumberOfTestSentences As Integer = 100

    Private HasNoise As Boolean


#Region "Settings"



    Public Overrides ReadOnly Property ShowGuiChoice_TargetLevel As Boolean = True

    Public Overrides ReadOnly Property ShowGuiChoice_MaskingLevel As Boolean = True

    Public Overrides ReadOnly Property ShowGuiChoice_BackgroundLevel As Boolean = False


    Private MaximumSoundDuration As Double = 21
    Private TestWordPresentationTime As Double = 0.5
    Private MaximumResponseTime As Double = 20.5
    Private ResultSummaryForGUI As New List(Of String)


#End Region

    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        If IsInitialized = True Then Return New Tuple(Of Boolean, String)(True, "")

        If SignalLocations.Count = 0 Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one signal sound source!")
        End If

        If MaskerLocations.Count = 0 And TestMode = TestModes.AdaptiveNoise Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one masker sound source in tests with adaptive noise!")
        End If

        Dim StartAdaptiveLevel As Double
        If MaskerLocations.Count > 0 Then
            'It's a speech in noise test, using adaptive SNR
            HasNoise = True
            StartAdaptiveLevel = CurrentSNR
        Else
            'It's a speech only test, using adaptive speech level
            HasNoise = False
            StartAdaptiveLevel = TargetLevel
        End If

        TestProtocol.IsInPretestMode = IsPractiseTest

        Dim TestLength As Integer

        Select Case True
            Case TypeOf TestProtocol Is HagermanKinnefors1995_TestProtocol

                If HasNoise = False Then
                    DirectCast(TestProtocol, HagermanKinnefors1995_TestProtocol).AdaptiveType = HagermanKinnefors1995_TestProtocol.AdaptiveTypes.ThresholdInSilence
                    TestMode = TestModes.AdaptiveSpeech
                    TestLength = 20
                Else

                    DirectCast(TestProtocol, HagermanKinnefors1995_TestProtocol).AdaptiveType = HagermanKinnefors1995_TestProtocol.AdaptiveTypes.ThresholdInNoise
                    TestMode = TestModes.AdaptiveNoise

                    If TestProtocol.IsInPretestMode = True Then
                        TestLength = 30
                    Else
                        TestLength = 20
                    End If

                End If

            Case TypeOf TestProtocol Is BrandKollmeier2002_TestProtocol

                DirectCast(TestProtocol, HagermanKinnefors1995_TestProtocol).AdaptiveType = HagermanKinnefors1995_TestProtocol.AdaptiveTypes.ThresholdInNoise
                TestMode = TestModes.AdaptiveNoise
                TargetLevel = 65
                MaskingLevel = 65
                StartAdaptiveLevel = SignalToNoiseRatio(TargetLevel, MaskingLevel)
                TestLength = 20

            Case Else

                If HasNoise = True Then
                    'It's a speech in noise test, using adaptive SNR
                    StartAdaptiveLevel = SignalToNoiseRatio(TargetLevel, MaskingLevel)
                Else
                    'It's a speech only test, using adaptive speech level
                    StartAdaptiveLevel = TargetLevel
                End If

                TestLength = 20

        End Select

        'Ensuring that contralateral masking level is always used with contralateral masking
        LockContralateralMaskingLevelToSpeechLevel = ContralateralMasking

        CreatePlannedWordsSentences()

        TestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = StartAdaptiveLevel, .TestStage = 0, .TestLength = TestLength})

        IsInitialized = True

        Return New Tuple(Of Boolean, String)(True, "")

    End Function

    Private Function CreatePlannedWordsSentences() As Boolean

        'Adding MaximumNumberOfTestSentences sentences, starting from the start list (excluding practise items), and re-using lists if needed 
        Dim TempAvailableLists As New List(Of SpeechMaterialComponent)
        Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, True, False)

        Dim ListCount As Integer = AllLists.Count
        Dim TotalSentenceCount As Integer = 0
        For Each List In AllLists
            TotalSentenceCount += List.ChildComponents.Count ' N.B. We get sentence components here, but in spondee materials, each sentence only contains one word.
        Next

        'Calculating the number of loops around the material that is needed to get MaximumNumberOfTestSentences sentences, and adding one loop to compensate for not starting the adding of sentences at the first list
        Dim LoopsNeeded As Integer = Math.Ceiling(TotalSentenceCount / MaximumNumberOfTestSentences) + 1
        'Adding the number of lists needed 
        For i = 1 To LoopsNeeded
            TempAvailableLists.AddRange(AllLists)
        Next
        'Determines the index of the start list
        Dim SelectedStartListIndex As Integer = -1
        For i = 0 To AllLists.Count - 1
            If AllLists(i).PrimaryStringRepresentation = StartList Then
                SelectedStartListIndex = i
                Exit For
            End If
        Next
        'Collecting the lists to use, starting with the start list
        Dim ListsToUse As New List(Of SpeechMaterialComponent)
        If SelectedStartListIndex > -1 Then
            For i = SelectedStartListIndex To TempAvailableLists.Count - 1
                ListsToUse.Add(TempAvailableLists(i))
            Next
        Else
            'This should not happen unless there are no lists loaded!
            Messager.MsgBox("Unable to add test sentences, probably since the selected speech material only contains " & TotalSentenceCount & " sentences!",, "An error occurred!")
            Return False
        End If

        'Adding all planned test sentences, and stopping after MaximumNumberOfTestSentences have been added
        PlannedTestSentences = New List(Of SpeechMaterialComponent)
        Dim TargetNumberOfSentencesReached As Boolean = False
        For Each List In ListsToUse
            Dim CurrentSentences = List.GetChildren()

            'Adding sentence in the original order
            If WithinListRandomization = False Then
                For Each Sentence In CurrentSentences
                    PlannedTestSentences.Add(Sentence)
                    'Checking if enough words have been added
                    If PlannedTestSentences.Count = MaximumNumberOfTestSentences Then
                        TargetNumberOfSentencesReached = True
                        Exit For
                    End If
                Next
            Else

                Throw New Exception("This block of code is not finished!")

                'Randomizing words across the sentence in each sentence list

                'Checing to ensure that all sentences have equal number of words (i.e. the list is matrix form)
                Dim WordCount As Integer? = Nothing
                For Each Sentence In CurrentSentences
                    If WordCount.HasValue Then
                        WordCount = Sentence.ChildComponents.Count
                    Else
                        If Sentence.ChildComponents.Count <> WordCount Then
                            MsgBox("An attempt was made to create a matrix test from a speech material which was not in matrix form, unable to proceed.", , "An error occurred!")
                            Throw New Exception("An attempt was made to create a matrix test from a speech material which was not in matrix form.")
                        End If
                    End If
                Next

                'Randomizing across the first set of words, then the second set of words and so on
                For w = 0 To WordCount - 1

                    Dim RandomizedOrder = Utils.SampleWithoutReplacement(CurrentSentences.Count, 0, CurrentSentences.Count, Randomizer)
                    Dim WordsInRandomOrder As New List(Of SpeechMaterialComponent)

                    For Each RandomIndex In RandomizedOrder
                        WordsInRandomOrder.Add(CurrentSentences(RandomIndex).ChildComponents(w))
                    Next

                    'CurrentSentences(i).ChildComponents.Clear()
                    For i = 0 To CurrentSentences.Count - 1
                        'TODO: This may be a bad idea since it probably messes up the originally loaded structure of the SpeechMaterialComponent
                        CurrentSentences(i).ChildComponents.Add(WordsInRandomOrder(i))
                        WordsInRandomOrder(i).ParentComponent = CurrentSentences(i)
                    Next

                Next

                'Checking if enough words have been added
                If PlannedTestSentences.Count = MaximumNumberOfTestSentences Then
                    TargetNumberOfSentencesReached = True
                    Exit For
                End If

            End If

            If TargetNumberOfSentencesReached = True Then
                'Breaking out of the outer loop if we have enough words
                Exit For
            End If

        Next

        'Checing that we really have NumberOfWordsToAdd words
        If MaximumNumberOfTestSentences <> PlannedTestSentences.Count Then
            Messager.MsgBox("The wrong number of test items were added. It should have been " & MaximumNumberOfTestSentences & " but instead " & PlannedTestSentences.Count & " items were added!",, "An error occurred!")
            Return False
        End If

        Return True

    End Function


    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        Dim ProtocolReply As NextTaskInstruction = Nothing

        If e IsNot Nothing Then

            'This is an incoming test trial response

            'Corrects the trial response, based on the given response
            Dim WordsInSentence = CurrentTestTrial.SpeechMaterialComponent.ChildComponents()
            Dim CorrectWordsList As New List(Of String)

            'Resets the CurrentTestTrial.ScoreList
            CurrentTestTrial.ScoreList = New List(Of Integer)
            For i = 0 To e.LinguisticResponses.Count - 1
                If e.LinguisticResponses(i) = WordsInSentence(i).GetCategoricalVariableValue("Spelling") Then
                    CurrentTestTrial.ScoreList.Add(1)
                Else
                    CurrentTestTrial.ScoreList.Add(0)
                End If
            Next

            'Checks if the trial is finished
            If CurrentTestTrial.ScoreList.Count < CurrentTestTrial.Tasks Then
                'Returns to continue the trial
                Return SpeechTestReplies.ContinueTrial
            End If

            'Adding the test trial
            ObservedTrials.Add(CurrentTestTrial)

            'Calculating the speech level
            ProtocolReply = TestProtocol.NewResponse(ObservedTrials)

            'Taking a dump of the SpeechTest before swapping to the new trial, but after the protocol reply so that the protocol results also gets dumped
            CurrentTestTrial.SpeechTestPropertyDump = Utils.Logging.ListObjectPropertyValues(Me.GetType, Me)

        Else
            'Nothing to correct (this should be the start of a new test)

            'Calculating the speech level (of the first trial)
            ProtocolReply = TestProtocol.NewResponse(ObservedTrials)

        End If


        'TODO: We must store the responses and response times!!!



        ' Returning if we should not move to the next trial
        If ProtocolReply.Decision <> SpeechTestReplies.GotoNextTrial Then
            Return ProtocolReply.Decision
        Else
            Return PrepareNextTrial(ProtocolReply)
        End If

    End Function

    Private Function PrepareNextTrial(ByVal NextTaskInstruction As TestProtocol.NextTaskInstruction) As SpeechTestReplies

        'Preparing the next trial
        'Getting next test sentence
        Dim NextTestSentence = PlannedTestSentences(ObservedTrials.Count)

        'Creating a new test trial
        Select Case TestMode
            Case TestModes.AdaptiveSpeech

                If HasNoise = True Then

                    'Adjusting levels
                    TargetLevel = MaskingLevel + NextTaskInstruction.AdaptiveValue
                    'MaskingLevel = MaskingLevel
                    'ContralateralMaskingLevel = ContralateralMaskingLevel

                    CurrentTestTrial = New TestTrial With {.SpeechMaterialComponent = NextTestSentence,
                        .AdaptiveProtocolValue = NextTaskInstruction.AdaptiveValue,
                        .TestStage = NextTaskInstruction.TestStage,
                        .Tasks = 5}

                Else

                    'Adjusting levels
                    TargetLevel = NextTaskInstruction.AdaptiveValue
                    MaskingLevel = Double.NegativeInfinity
                    'ContralateralMaskingLevel = ContralateralMaskingLevel

                    CurrentTestTrial = New TestTrial With {.SpeechMaterialComponent = NextTestSentence,
                        .AdaptiveProtocolValue = NextTaskInstruction.AdaptiveValue,
                        .TestStage = NextTaskInstruction.TestStage,
                        .Tasks = 5}

                End If


            Case TestModes.AdaptiveNoise

                'Adjusting levels
                'TargetLevel = TargetLevel
                MaskingLevel = TargetLevel - NextTaskInstruction.AdaptiveValue
                'ContralateralMaskingLevel = ContralateralMaskingLevel

                CurrentTestTrial = New TestTrial With {.SpeechMaterialComponent = NextTestSentence,
                    .AdaptiveProtocolValue = NextTaskInstruction.AdaptiveValue,
                    .TestStage = NextTaskInstruction.TestStage,
                    .Tasks = 5}

            Case Else
                Throw New NotImplementedException
        End Select


        Dim ResponseAlternativeSpellingsList As New List(Of List(Of String))

        If IsFreeRecall = True Then

            'Adding only the correct words to the GUI
            Dim WordsInSentence = CurrentTestTrial.SpeechMaterialComponent.ChildComponents()
            Dim CorrectWordsList As New List(Of String)
            For Each Word In WordsInSentence
                CorrectWordsList.Add(Word.GetCategoricalVariableValue("Spelling"))
            Next
            ResponseAlternativeSpellingsList.Add(CorrectWordsList)

        Else

            'Adding all words to the GUI
            Dim AllSentencesInList = NextTestSentence.GetSiblings()

            For s = 0 To AllSentencesInList.Count - 1
                Dim WordsInSentence = AllSentencesInList(s).ChildComponents()
                Dim WordSpellings = New List(Of String)
                For w = 0 To WordsInSentence.Count - 1
                    WordSpellings.Add(WordsInSentence(w).GetCategoricalVariableValue("Spelling"))
                Next
                ResponseAlternativeSpellingsList.Add(WordSpellings)
            Next

            'Transposing the matrix
            ResponseAlternativeSpellingsList = TransposeMatrix(ResponseAlternativeSpellingsList)

            'Sorting the matrix alphabetically
            For Each Item In ResponseAlternativeSpellingsList
                Item.Sort()
            Next

            'Transposing back after sorting
            'ReponseAlternativeList = TransposeMatrix(ReponseAlternativeList)

            'Add other buttons needed ?

            'A Did-Not-Hear-Response Alternative ?
            If IncludeDidNotHearResponseAlternative = True Then
                For Each Item In ResponseAlternativeSpellingsList
                    Item.Add("?")
                Next
            End If

        End If

        'Converting to a SpeechTestResponseAlternative instead of strings
        Dim ResponseAlternativeList As New List(Of List(Of SpeechTestResponseAlternative))
        For Each List In ResponseAlternativeSpellingsList
            Dim NewList As New List(Of SpeechTestResponseAlternative)
            For Each ListItem In List
                NewList.Add(New SpeechTestResponseAlternative With {.Spelling = ListItem})
            Next
            ResponseAlternativeList.Add(NewList)
        Next

        'Adding the list
        CurrentTestTrial.ResponseAlternativeSpellings = ResponseAlternativeList

        'Mixing trial sound
        MixStandardTestTrialSound(UseNominalLevels:=True,
                                  MaximumSoundDuration:=MaximumSoundDuration,
                                  TargetPresentationTime:=TestWordPresentationTime,
                                  ExportSounds:=False)

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        'CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 500, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        'CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 501, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        'If IsFreeRecall = False Then CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 20500, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * TestWordPresentationTime), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        If IsFreeRecall = False Then CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * (TestWordPresentationTime + MaximumResponseTime)), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

        Return SpeechTestReplies.GotoNextTrial

    End Function

    Private Function TransposeMatrix(ByVal Matrix As List(Of List(Of String))) As List(Of List(Of String))

        Dim Output As New List(Of List(Of String))

        If Matrix.Count = 0 Then
            Return Output
        Else
            'Adding the second dimension lists
            For Each Column In Matrix(0)
                Output.Add(New List(Of String))
            Next
        End If

        'Transposing the matrix
        For OutputRow = 0 To Matrix.Count - 1
            For OutputColumn = 0 To Output.Count - 1
                Output(OutputColumn).Add(Matrix(OutputRow)(OutputColumn))
            Next
        Next

        Return Output

    End Function




    Public Overrides Function GetResultStringForGui() As String

        Dim ProtocolThreshold = TestProtocol.GetFinalResultValue()

        Dim Output As New List(Of String)

        If ProtocolThreshold IsNot Nothing Then
            If TestProtocol.IsInPretestMode = True Then
                ResultSummaryForGUI.Add("Resultat för övningstestet: SNR = " & vbTab & Math.Round(ProtocolThreshold.Value) & " dB")
            Else
                ResultSummaryForGUI.Add("Testresultat: SNR = " & vbTab & Math.Round(ProtocolThreshold.Value) & " dB")
            End If

            Output.AddRange(ResultSummaryForGUI)
        Else
            If TestProtocol.IsInPretestMode = True Then
                Output.Add("Övningstest!")
            End If

            If CurrentTestTrial IsNot Nothing Then
                Output.Add("Mening nummer " & ObservedTrials.Count + 1 & " av " & TestProtocol.TotalTrialCount)
                Output.Add("SNR = " & Math.Round(CurrentSNR) & " " & dBString())
                Output.Add("Talnivå = " & Math.Round(TargetLevel) & " " & dBString())
                Output.Add("Brusnivå = " & Math.Round(MaskingLevel) & " " & dBString())
                If ContralateralMasking = True Then
                    Output.Add("Kontralateral brusnivå = " & Math.Round(ContralateralMaskingLevel) & " " & dBString())
                End If
            End If
        End If

        Return String.Join(vbCrLf, Output)

    End Function

    ''' <summary>
    ''' This function should list the names of variables included SpeechTestDump of each test trial to be exported in the "selected-variables" export file.
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function GetSelectedExportVariables() As List(Of String)
        Return New List(Of String)
    End Function

    Public Overrides Sub FinalizeTestAheadOfTime()

        TestProtocol.AbortAheadOfTime(ObservedTrials)

    End Sub

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)
        Throw New NotImplementedException
    End Function

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        Throw New NotImplementedException()
    End Sub

End Class


