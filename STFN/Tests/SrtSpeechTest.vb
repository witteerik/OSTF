Imports MathNet.Numerics

Public Class SrtSpeechTest
    Inherits SpeechTest

    ''' <summary>
    ''' This collection contains MaximumNumberOfTestWords which can be used troughout the test, in sequential order.
    ''' </summary>
    Private PlannedTestWords As List(Of SpeechMaterialComponent)

    Private MaximumNumberOfTestWords As Integer = 200



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
        FixedResponseAlternativeCount = 4
        StartLevel = 40
        RandomizeWordsWithinLists = True
        SelectTestProtocol(AvailableTestProtocols(0))

    End Sub

    Public Overrides Function InitializeCurrentTest() As Boolean

        ObservedTrials = New TrialHistory

        CreatePlannedWordsList()

        SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = StartLevel, .TestStage = 0})

        Return True

    End Function

    Private Function CreatePlannedWordsList() As Boolean

        'Adding NumberOfWordsToAdd words, starting from the start list (excluding practise items), and re-using lists if needed 
        Dim TempAvailableLists As New List(Of SpeechMaterialComponent)
        Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, True, False)

        Dim ListCount As Integer = AllLists.Count
        Dim TotalWordCount As Integer = 0
        For Each List In AllLists
            TotalWordCount += List.ChildComponents.Count ' N.B. We get sentence components here, but in spondee materials, each sentence only contains one word.
        Next

        'Calculating the number of loops around the material that is needed to get NumberOfWordsToAdd words, and adding one loop to compensate for not starting the adding of words at the first list
        Dim LoopsNeeded As Integer = Math.Ceiling(TotalWordCount / MaximumNumberOfTestWords) + 1
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
        'Collecting the lists to use, starting with the stat list
        Dim ListsToUse As New List(Of SpeechMaterialComponent)
        If SelectedStartListIndex > -1 Then
            For i = SelectedStartListIndex To TempAvailableLists.Count - 1
                ListsToUse.Add(TempAvailableLists(i))
            Next
        Else
            'This should not happen unless there are no lists loaded!
            Messager.MsgBox("Unable to add test words, probably since the selected speech material only contains " & TotalWordCount & " words!",, "An error occurred!")
            Return False
        End If

        'Adding all planned test words, and stopping after NumberOfWordsToAdd have been added
        PlannedTestWords = New List(Of SpeechMaterialComponent)
        Dim TargetNumberOfWordsReached As Boolean = False
        For Each List In ListsToUse
            Dim CurrentWords = List.GetChildren()

            If RandomizeWordsWithinLists = False Then
                For Each Word In CurrentWords
                    PlannedTestWords.Add(Word)
                    'Checking if enough words have been added
                    If PlannedTestWords.Count = MaximumNumberOfTestWords Then
                        TargetNumberOfWordsReached = True
                        Exit For
                    End If
                Next
            Else
                'Randomizing order
                Dim RandomizedOrder = Utils.SampleWithoutReplacement(CurrentWords.Count, 0, CurrentWords.Count, Randomizer)
                For Each RandomIndex In RandomizedOrder
                    PlannedTestWords.Add(CurrentWords(RandomIndex))
                    'Checking if enough words have been added
                    If PlannedTestWords.Count = MaximumNumberOfTestWords Then
                        TargetNumberOfWordsReached = True
                        Exit For
                    End If
                Next
            End If

            If TargetNumberOfWordsReached = True Then
                'Breaking out of the outer loop if we have enough words
                Exit For
            End If

        Next

        'Checing that we really have NumberOfWordsToAdd words
        If MaximumNumberOfTestWords <> PlannedTestWords.Count Then
            Messager.MsgBox("The wrong number of test items were added. It should have been " & MaximumNumberOfTestWords & " but instead " & PlannedTestWords.Count & " items were added!",, "An error occurred!")
            Return False
        End If

        Return True

    End Function


    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

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
        'Getting next test word
        Dim NextTestWord = PlannedTestWords(ObservedTrials.Count)

        'Creating a new test trial
        CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
            .SpeechLevel = NextTaskInstruction.AdaptiveValue,
            .TestStage = NextTaskInstruction.TestStage,
            .Tasks = 1}

        If IsFreeRecall Then
            CurrentTestTrial.ResponseAlternativeSpellings = New List(Of List(Of String)) From {New List(Of String) From {"Rätt", "Fel"}}
        Else
            'Adding the current word pselling as a response alternative
            Dim ResponseAlternatives As New List(Of String) From {CurrentTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling")}

            'Picking random response alternatives from all available test words
            Dim AllContrastingWords = NextTestWord.GetAllRelativesAtLevelExludingSelf(SpeechMaterialComponent.LinguisticLevels.Sentence, True, False)
            Dim RandomIndices = Utils.SampleWithoutReplacement(Math.Max(0, FixedResponseAlternativeCount - 1), 0, AllContrastingWords.Count, Randomizer)
            For Each RandomIndex In RandomIndices
                ResponseAlternatives.Add(AllContrastingWords(RandomIndex).GetCategoricalVariableValue("Spelling"))
            Next

            'Shuffling the order of response alternatives
            Dim ShuffledResponseAlternatives = Utils.Shuffle(ResponseAlternatives, Randomizer).ToList
            CurrentTestTrial.ResponseAlternativeSpellings = New List(Of List(Of String)) From {ShuffledResponseAlternatives}
        End If


        'Mixing trial sound
        MixNextTrialSound()

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 500, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 501, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 5500, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

        Return SpeechTestReplies.GotoNextTrial

    End Function



    Private Sub MixNextTrialSound()

        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)

        'Setting level
        Audio.DSP.MeasureAndAdjustSectionLevel(TestWordSound, Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel))

        'Copying to stereo and storing in CurrentTestTrial.Sound 
        CurrentTestTrial.Sound = TestWordSound.ConvertMonoToMultiChannel(2, True)

    End Sub


    Public Overrides Function SaveResults(TestResults As TestResults) As Boolean

        'Not finished!

        Return True
    End Function


    Public Overrides Function GetResults() As TestResults
        Return SelectedTestProtocol.GetResults(ObservedTrials)
    End Function
End Class

Public Class TestResults

    Public ReadOnly TestResultType As TestResultTypes

    Public Enum TestResultTypes
        SRT
    End Enum

    Public SpeechRecognitionThreshold As Double
    Public TestStageSeries As List(Of String)
    Public SpeechLevelSeries As List(Of Double)
    Public ScoreSeries As List(Of String)

    Public Sub New(ByVal TestResultType As TestResultTypes)
        Me.TestResultType = TestResultType
    End Sub

    Public Function GetFormattedTestResultsSummaryString() As String

        Dim ResultsRowList = New List(Of String)

        Select Case TestResultType
            Case TestResultTypes.SRT
                ResultsRowList.Add("Speech recognition threshold:" & vbCrLf & Math.Round(SpeechRecognitionThreshold) & " dB SPL")
                ResultsRowList.Add("Test stage:" & vbCrLf & String.Join(vbTab, TestStageSeries))
                ResultsRowList.Add("Speech level:" & vbCrLf & String.Join(vbTab, SpeechLevelSeries))
                ResultsRowList.Add("Trial score:" & vbCrLf & String.Join(vbTab, ScoreSeries))

            Case Else


        End Select

        Return String.Join(vbCrLf, ResultsRowList)

    End Function

End Class



