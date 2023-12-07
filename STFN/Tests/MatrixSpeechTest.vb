Imports MathNet.Numerics

Public Class MatrixSpeechTest
    Inherits SpeechTest

    ''' <summary>
    ''' This collection contains PlannedTestSentences which can be used troughout the test, in sequential order.
    ''' </summary>
    Private PlannedTestSentences As List(Of SpeechMaterialComponent)

    Private MaximumNumberOfTestSentences As Integer = 200



    Private ObservedTrials As TrialHistory


#Region "Settings"

    Public SelectedMediaSet As MediaSet

    Public StartList As String = ""

    Public StartLevel As Double


#End Region

    Public Sub New(ByVal SpeechMaterialName As String, ByVal AvailableTestProtocols As TestProtocols)
        MyBase.New(SpeechMaterialName, AvailableTestProtocols)

        'Some initial settings which should be overridden by the settings editor
        SelectedMediaSet = GetAvailableMediasets(0)
        StartList = "Lista 3"
        StartLevel = 70
        RandomizeWordsWithinLists = False
        SelectTestProtocol(AvailableTestProtocols(0))

        IsFreeRecall = False

    End Sub

    Public Overrides Function InitializeCurrentTest() As Boolean

        ObservedTrials = New TrialHistory

        CreatePlannedWordsSentences()

        SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = StartLevel, .TestStage = 0})

        Return True

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
            If RandomizeWordsWithinLists = False Then
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


        If e IsNot Nothing Then

            'This is an incoming test trial response

            'Chcking if it's a missing response
            If e.LinguisticResponse = "" Then

                'Adds the remaining blank/missing reponses, forcing the test to move on
                Do Until CurrentTestTrial.ScoreList.Count >= CurrentTestTrial.Tasks
                    CurrentTestTrial.ScoreList.Add(0)
                Loop

            Else

                'Corrects the trial response, based on the given response
                Dim WordsInSentence = CurrentTestTrial.SpeechMaterialComponent.ChildComponents()
                Dim CorrectWordsList As New List(Of String)
                If e.LinguisticResponse = WordsInSentence(CurrentTestTrial.ScoreList.Count).GetCategoricalVariableValue("Spelling") Then
                    CurrentTestTrial.ScoreList.Add(1)
                Else
                    CurrentTestTrial.ScoreList.Add(0)
                End If

            End If

            'Checks if the trial is finished
            If CurrentTestTrial.ScoreList.Count < CurrentTestTrial.Tasks Then
                'Returns to continue the trial
                Return SpeechTestReplies.ContinueTrial
            End If

            'Adding the test trial
            ObservedTrials.Add(CurrentTestTrial)

        Else
            'Nothing to correct (this should be the start of a new test)
        End If

        'TODO: We must store the responses and response times!!!


        'Calculating the speech level
        Dim ProtocolReply = SelectedTestProtocol.NewResponse(ObservedTrials)

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
        CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestSentence,
            .SpeechLevel = NextTaskInstruction.AdaptiveValue,
            .TestStage = NextTaskInstruction.TestStage,
            .Tasks = 5}

        If IsFreeRecall = True Or IsFreeRecall = False Then
            'Think the same code applies in both situations?

            Dim AllSentencesInList = NextTestSentence.GetSiblings()
            Dim ReponseAlternativeList As New List(Of List(Of String))

            For s = 0 To AllSentencesInList.Count - 1
                Dim WordsInSentence = AllSentencesInList(s).ChildComponents()
                Dim WordSpellings = New List(Of String)
                For w = 0 To WordsInSentence.Count - 1
                    WordSpellings.Add(WordsInSentence(w).GetCategoricalVariableValue("Spelling"))
                Next
                ReponseAlternativeList.Add(WordSpellings)
            Next

            'Transposing the matrix
            Dim TransposedMatrix = TransposeMatrix(ReponseAlternativeList)

            'Sorting the matrix alphabetically
            For Each Item In TransposedMatrix
                Item.Sort()
            Next

            'Transposing back after sorting
            Dim ReTransposedMatrix = TransposeMatrix(TransposedMatrix)

            'Add other buttons needed

            'Adding the list
            CurrentTestTrial.ResponseAlternativeSpellings = ReTransposedMatrix
        End If

        'Mixing trial sound
        MixNextTrialSound()

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 500, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 501, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 14500, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

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


    Private Sub MixNextTrialSound()

        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)

        'Setting level
        Audio.DSP.MeasureAndAdjustSectionLevel(TestWordSound, Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel))

        'Padding the sound by one second, and converting it to sterao
        Dim PaddedSound = TestWordSound.ZeroPad(1.0R, Nothing, False).ConvertMonoToMultiChannel(2, True)

        'Copying to stereo and storing in CurrentTestTrial.Sound 
        CurrentTestTrial.Sound = PaddedSound

    End Sub


    Public Overrides Function SaveResults(TestResults As TestResults) As Boolean

        'Not finished!

        Return True
    End Function


    Public Overrides Function GetResults() As TestResults
        Return SelectedTestProtocol.GetResults(ObservedTrials)
    End Function
End Class


