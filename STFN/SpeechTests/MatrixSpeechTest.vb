Imports MathNet.Numerics

Public Class MatrixSpeechTest
    Inherits SpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String
        Get
            Return "Matrix"
        End Get
    End Property


    ''' <summary>
    ''' This collection contains PlannedTestSentences which can be used troughout the test, in sequential order.
    ''' </summary>
    Private PlannedTestSentences As List(Of SpeechMaterialComponent)

    Private MaximumNumberOfTestSentences As Integer = 100

    Private HasNoise As Boolean

    Private ObservedTrials As TrialHistory


#Region "Settings"

    Public Overrides ReadOnly Property TesterInstructions As String
        Get
            Return ""
        End Get
    End Property

    Public Overrides ReadOnly Property ParticipantInstructions As String
        Get
            Return ""
        End Get
    End Property

    Public Overrides ReadOnly Property HasOptionalPractiseTest As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualPreSetSelection As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualStartListSelection As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualMediaSetSelection As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualSpeechLevelSelection As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualMaskingLevelSelection As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualBackgroundLevelSelection As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property SupportsPrelistening As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property UseSoundFieldSimulation As Utils.TriState
        Get
            Return TriState.True
        End Get
    End Property



    Public Overrides ReadOnly Property AvailableTestModes As List(Of TestModes)
        Get
            Return New List(Of TestModes) From {TestModes.AdaptiveSpeech, TestModes.AdaptiveNoise}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestProtocols As List(Of TestProtocol)
        Get
            Return TestProtocols.GetSrtProtocols
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableFixedResponseAlternativeCounts As List(Of Integer)
        Get
            Return New List(Of Integer)
        End Get
    End Property

    Public Overrides ReadOnly Property AvailablePhaseAudiometryTypes As List(Of BmldModes)
        Get
            Return New List(Of BmldModes) From {BmldModes.RightOnly, BmldModes.LeftOnly, BmldModes.BinauralSamePhase, BmldModes.BinauralPhaseInverted, BmldModes.BinauralUncorrelated}
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSoundFieldSpeechLocations As Integer
        Get
            Return 1
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSoundFieldMaskerLocations As Integer
        Get
            Return 1000
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSoundFieldBackgroundNonSpeechLocations As Integer
        Get
            Return 1000
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSoundFieldBackgroundSpeechLocations As Integer
        Get
            Return 1000
        End Get
    End Property

    Public Overrides ReadOnly Property MinimumSoundFieldSpeechLocations As Integer
        Get
            Return 1
        End Get
    End Property

    Public Overrides ReadOnly Property MinimumSoundFieldMaskerLocations As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property MinimumSoundFieldBackgroundNonSpeechLocations As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property MinimumSoundFieldBackgroundSpeechLocations As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualReferenceLevelSelection As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanHaveTargets As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanHaveMaskers As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanHaveBackgroundNonSpeech As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanHaveBackgroundSpeech As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property UseKeyWordScoring As Utils.TriState
        Get
            Return Utils.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseListOrderRandomization As Utils.TriState
        Get
            Return Utils.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseWithinListRandomization As Utils.TriState
        Get
            Return Utils.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseAcrossListRandomization As Utils.TriState
        Get
            Return Utils.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseFreeRecall As Utils.TriState
        Get
            Return Utils.TriState.Optional
        End Get
    End Property

    Public Overrides ReadOnly Property UseDidNotHearAlternative As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseContralateralMasking As Utils.TriState
        Get
            Return Utils.Constants.TriState.Optional
        End Get
    End Property

    Public Overrides ReadOnly Property UsePhaseAudiometry As Utils.TriState
        Get
            Return Utils.Constants.TriState.Optional
        End Get
    End Property

    Public Overrides ReadOnly Property LevelStepSize As Double
        Get
            Return 5
        End Get
    End Property

    Public Overrides ReadOnly Property HistoricTrialCount As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property SupportsManualPausing As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides Property SoundOverlapDuration As Double = 0.1

    Public Overrides ReadOnly Property UseRetsplCorrection As Utils.Constants.TriState = Utils.Constants.TriState.False

    Public Overrides ReadOnly Property MinimumLevel As Double = -20
    Public Overrides ReadOnly Property MaximumLevel As Double = 80

#End Region

    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

    End Sub

    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        ObservedTrials = New TrialHistory

        If CustomizableTestOptions.SignalLocations.Count = 0 Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one signal sound source!")
        End If

        If CustomizableTestOptions.MaskerLocations.Count = 0 And CustomizableTestOptions.SelectedTestMode = TestModes.AdaptiveNoise Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one masker sound source in tests with adaptive noise!")
        End If

        Dim StartAdaptiveLevel As Double
        If CustomizableTestOptions.MaskerLocations.Count > 0 Then
            'It's a speech in noise test, using adaptive SNR
            HasNoise = True
            Dim InitialSNR = SignalToNoiseRatio(CustomizableTestOptions.SpeechLevel, CustomizableTestOptions.MaskingLevel)
            StartAdaptiveLevel = InitialSNR
        Else
            'It's a speech only test, using adaptive speech level
            HasNoise = False
            StartAdaptiveLevel = CustomizableTestOptions.SpeechLevel
        End If

        CustomizableTestOptions.SelectedTestProtocol.IsInPretestMode = CustomizableTestOptions.IsPractiseTest

        CreatePlannedWordsSentences()

        CustomizableTestOptions.SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = StartAdaptiveLevel, .TestStage = 0, .TestLength = 20})

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
            If AllLists(i).PrimaryStringRepresentation = CustomizableTestOptions.StartList Then
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
            If CustomizableTestOptions.RandomizeItemsWithinLists = False Then
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
        Dim ProtocolReply = CustomizableTestOptions.SelectedTestProtocol.NewResponse(ObservedTrials)

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
        Select Case CustomizableTestOptions.SelectedTestMode
            Case TestModes.AdaptiveSpeech

                If HasNoise = True Then

                    CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestSentence,
            .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
            .SpeechLevel = CustomizableTestOptions.MaskingLevel + NextTaskInstruction.AdaptiveValue,
            .MaskerLevel = CustomizableTestOptions.MaskingLevel,
            .TestStage = NextTaskInstruction.TestStage,
            .Tasks = 5}

                Else

                    CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestSentence,
            .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
            .SpeechLevel = NextTaskInstruction.AdaptiveValue,
            .MaskerLevel = Double.NegativeInfinity,
            .TestStage = NextTaskInstruction.TestStage,
            .Tasks = 5}

                End If


            Case TestModes.AdaptiveNoise

                CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestSentence,
            .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
            .SpeechLevel = CustomizableTestOptions.SpeechLevel,
            .MaskerLevel = CustomizableTestOptions.SpeechLevel - NextTaskInstruction.AdaptiveValue,
            .TestStage = NextTaskInstruction.TestStage,
            .Tasks = 5}

            Case Else
                Throw New NotImplementedException
        End Select


        Dim ResponseAlternativeSpellingsList As New List(Of List(Of String))

        If CustomizableTestOptions.IsFreeRecall = True Then

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
            If CustomizableTestOptions.ShowDidNotHearResponseAlternative = True Then
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
        MixNextTrialSound()

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 500, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 501, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        If CustomizableTestOptions.IsFreeRecall = False Then CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 20500, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

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

        Dim RETSPL_Correction As Double = 0
        If CustomizableTestOptions.UseRetsplCorrection = True Then
            RETSPL_Correction = CustomizableTestOptions.SelectedTransducer.RETSPL_Speech
        End If

        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(CustomizableTestOptions.SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)

        Dim NominalLevel_FS = TestWordSound.SMA.NominalLevel
        Dim TargetLevel_FS = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel) + RETSPL_Correction
        Dim NeededGain = TargetLevel_FS - NominalLevel_FS

        Audio.DSP.AmplifySection(TestWordSound, NeededGain)

        'Setting level
        If HasNoise = True Then
            Dim Noise = CurrentTestTrial.SpeechMaterialComponent.GetMaskerSound(CustomizableTestOptions.SelectedMediaSet, 0)
            Audio.DSP.MeasureAndAdjustSectionLevel(Noise, Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, SrtTrial).MaskerLevel))

            Dim MixedSound = Audio.DSP.SuperpositionSounds({TestWordSound, Noise}.ToList)

            'Copying to stereo and storing in CurrentTestTrial.Sound 
            CurrentTestTrial.Sound = MixedSound.ConvertMonoToMultiChannel(2, True)
        Else

            'Copying to stereo and storing in CurrentTestTrial.Sound 
            CurrentTestTrial.Sound = TestWordSound.ConvertMonoToMultiChannel(2, True)

        End If

    End Sub

    Public Overrides Function GetResultStringForGui() As String
        ' Throw New NotImplementedException()
    End Function

    Public Overrides Function GetExportString() As String
        ' Throw New NotImplementedException()
    End Function

    'Public Overrides Function GetResults() As TestResults

    '    Dim ProtocolThreshold = CustomizableTestOptions.SelectedTestProtocol.GetFinalResult()

    '    Dim RawResults = New TestResults(TestResults.TestResultTypes.SRT)
    '    If ProtocolThreshold.HasValue Then
    '        RawResults.AdaptiveLevelThreshold = ProtocolThreshold
    '    Else
    '        'Storing NaN if no threshold was reached
    '        RawResults.AdaptiveLevelThreshold = Double.NaN
    '    End If

    '    'Calculating the SRT based on the adaptive threshold
    '    Select Case CustomizableTestOptions.SelectedTestMode
    '        Case TestModes.AdaptiveSpeech
    '            RawResults.SpeechRecognitionThreshold = RawResults.AdaptiveLevelThreshold
    '        Case TestModes.AdaptiveNoise
    '            RawResults.SpeechRecognitionThreshold = RawResults.AdaptiveLevelThreshold
    '        Case Else
    '            Throw New NotImplementedException
    '    End Select

    '    'Storing the AdaptiveLevelSeries
    '    RawResults.AdaptiveLevelSeries = New List(Of Double)
    '    RawResults.SpeechLevelSeries = New List(Of Double)
    '    RawResults.MaskerLevelSeries = New List(Of Double)
    '    RawResults.SNRLevelSeries = New List(Of Double)
    '    RawResults.TestStageSeries = New List(Of String)
    '    RawResults.ProportionCorrectSeries = New List(Of String)
    '    'Trial.IsCorrect  is not used
    '    'RawResults.ScoreSeries = New List(Of String)
    '    For Each Trial As SrtTrial In ObservedTrials
    '        RawResults.AdaptiveLevelSeries.Add(Math.Round(Trial.AdaptiveValue))
    '        RawResults.SpeechLevelSeries.Add(Math.Round(Trial.SpeechLevel))
    '        RawResults.MaskerLevelSeries.Add(Math.Round(Trial.MaskerLevel))
    '        RawResults.SNRLevelSeries.Add(Math.Round(Trial.SNR))
    '        RawResults.TestStageSeries.Add(Trial.TestStage)
    '        RawResults.ProportionCorrectSeries.Add(Trial.GetProportionTasksCorrect)
    '    Next

    '    Return RawResults

    'End Function


    Public Overrides Sub FinalizeTest()

        CustomizableTestOptions.SelectedTestProtocol.FinalizeProtocol(ObservedTrials)

    End Sub

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)
        Throw New NotImplementedException
    End Function

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        Throw New NotImplementedException()
    End Sub


End Class


