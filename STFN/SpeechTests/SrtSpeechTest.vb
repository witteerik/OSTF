Imports MathNet.Numerics

Public Class SrtSpeechTest
    Inherits SpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "SRT"

    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub

    Public Sub ApplyTestSpecificSettings()

        TesterInstructions = ""
        ParticipantInstructions = ""
        ShowGuiChoice_PractiseTest = True
        ShowGuiChoice_dBHL = False
        ShowGuiChoice_PreSet = False
        ShowGuiChoice_StartList = True
        ShowGuiChoice_MediaSet = True
        SupportsPrelistening = True
        ShowGuiChoice_SoundFieldSimulation = True
        AvailableTestModes = New List(Of TestModes) From {TestModes.AdaptiveSpeech}

        AvailableTestProtocols = New List(Of TestProtocol) From {
            New SrtIso8253_TestProtocol,
            New HagermanKinnefors1995_TestProtocol,
            New BrandKollmeier2002_TestProtocol,
            New SrtChaiklinVentry1964_TestProtocol,
            New SrtChaiklinFontDixon1967_TestProtocol,
            New SrtMargolis2021_TestProtocol}

        AvailableFixedResponseAlternativeCounts = New List(Of Integer) From {2, 3, 4, 5, 6, 7, 8, 10, 15, 20}

        AvailablePhaseAudiometryTypes = New List(Of BmldModes) From {BmldModes.RightOnly, BmldModes.LeftOnly, BmldModes.BinauralSamePhase, BmldModes.BinauralPhaseInverted, BmldModes.BinauralUncorrelated}

        MaximumSoundFieldSpeechLocations = 1
        MaximumSoundFieldMaskerLocations = 0
        MaximumSoundFieldBackgroundNonSpeechLocations = 0
        MaximumSoundFieldBackgroundSpeechLocations = 0
        MinimumSoundFieldSpeechLocations = 1
        MinimumSoundFieldMaskerLocations = 0
        MinimumSoundFieldBackgroundNonSpeechLocations = 0
        MinimumSoundFieldBackgroundSpeechLocations = 0
        ShowGuiChoice_ReferenceLevel = False
        ShowGuiChoice_KeyWordScoring = False
        ShowGuiChoice_ListOrderRandomization = True
        ShowGuiChoice_WithinListRandomization = True
        ShowGuiChoice_AcrossListRandomization = False
        ShowGuiChoice_FreeRecall = True
        ShowGuiChoice_DidNotHearAlternative = True
        PhaseAudiometry = False
        TargetLevel_StepSize = 5
        HistoricTrialCount = 0
        SupportsManualPausing = False
        ReferenceLevel = 65
        TargetLevel = 65
        MaskingLevel = 65
        BackgroundLevel = 50
        ContralateralMaskingLevel = 25
        MinimumReferenceLevel = -20
        MaximumReferenceLevel = 80
        MinimumLevel_Targets = -20
        MaximumLevel_Targets = 80
        MinimumLevel_Maskers = -20
        MaximumLevel_Maskers = 80
        MinimumLevel_Background = -20
        MaximumLevel_Background = 80
        MinimumLevel_ContralateralMaskers = -20
        MaximumLevel_ContralateralMaskers = 80
        AvailableExperimentNumbers = {}

        SoundOverlapDuration = 0.1

        ShowGuiChoice_TargetLocations = True
        ShowGuiChoice_MaskerLocations = False
        ShowGuiChoice_BackgroundNonSpeechLocations = False
        ShowGuiChoice_BackgroundSpeechLocations = False


    End Sub


    Public Overrides ReadOnly Property ShowGuiChoice_TargetLevel As Boolean = True
    Public Overrides ReadOnly Property ShowGuiChoice_MaskingLevel As Boolean = True
    Public Overrides ReadOnly Property ShowGuiChoice_BackgroundLevel As Boolean = True

    Public Overrides ReadOnly Property ShowGuiChoice_ContralateralMasking As Boolean = False




    ''' <summary>
    ''' This collection contains MaximumNumberOfTestWords which can be used troughout the test, in sequential order.
    ''' </summary>
    Private PlannedTestWords As List(Of SpeechMaterialComponent)

    Private MaximumNumberOfTestWords As Integer = 200

    Private HasNoise As Boolean

    Private ObservedTrials As TrialHistory

    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        ObservedTrials = New TrialHistory

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
            Dim InitialSNR = SignalToNoiseRatio(TargetLevel, MaskingLevel)
            StartAdaptiveLevel = InitialSNR
        Else
            'It's a speech only test, using adaptive speech level
            HasNoise = False
            StartAdaptiveLevel = TargetLevel
        End If

        TestProtocol.IsInPretestMode = IsPractiseTest

        CreatePlannedWordsList()

        TestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = StartAdaptiveLevel, .TestStage = 0})

        Return New Tuple(Of Boolean, String)(True, "")

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

            If WithinListRandomization = False Then
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
        Dim ProtocolReply = TestProtocol.NewResponse(ObservedTrials)

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then
            PrepareNextTrial(ProtocolReply)

            'Here we abort the test if any of the levels had to be adjusted above MaximumLevel dB HL
            If DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel > MaximumLevel_Targets Or
                    DirectCast(CurrentTestTrial, SrtTrial).MaskerLevel > MaximumLevel_Maskers Or
                DirectCast(CurrentTestTrial, SrtTrial).ContralateralMaskerLevel > MaximumLevel_ContralateralMaskers Then

                'And informing the participant
                ProtocolReply.Decision = SpeechTestReplies.AbortTest
                Select Case GuiLanguage
                    Case Utils.Constants.Languages.Swedish
                        AbortInformation = "Testet har avbrutits på grund av höga ljudnivåer."
                    Case Else
                        AbortInformation = "The test hade to be aborted due to high sound levels."
                End Select

            End If

        End If

        Return ProtocolReply.Decision

    End Function

    Private Sub PrepareNextTrial(ByVal NextTaskInstruction As TestProtocol.NextTaskInstruction)

        'Preparing the next trial
        'Getting next test word
        Dim NextTestWord = PlannedTestWords(ObservedTrials.Count)

        'Creating a new test trial
        Select Case TestMode
            Case TestModes.AdaptiveSpeech

                If HasNoise = True Then

                    CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
                        .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
                        .SpeechLevel = MaskingLevel + NextTaskInstruction.AdaptiveValue,
                        .MaskerLevel = MaskingLevel,
                        .TestStage = NextTaskInstruction.TestStage,
                        .Tasks = 1}

                Else

                    CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
                        .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
                        .SpeechLevel = NextTaskInstruction.AdaptiveValue,
                        .MaskerLevel = Double.NegativeInfinity,
                        .TestStage = NextTaskInstruction.TestStage,
                        .Tasks = 1}

                End If


            Case TestModes.AdaptiveNoise

                CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
                    .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
                    .SpeechLevel = TargetLevel,
                    .MaskerLevel = TargetLevel - NextTaskInstruction.AdaptiveValue,
                    .TestStage = NextTaskInstruction.TestStage,
                    .Tasks = 1}

            Case Else
                Throw New NotImplementedException
        End Select


        CurrentTestTrial.ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))

        Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)
        If IsFreeRecall Then
            If CurrentTestTrial.SpeechMaterialComponent.ChildComponents.Count > 0 Then

                CurrentTestTrial.Tasks = 0
                For Each Child In CurrentTestTrial.SpeechMaterialComponent.ChildComponents()

                    If KeyWordScoring = True Then
                        ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = Child.GetCategoricalVariableValue("Spelling"), .IsScoredItem = Child.IsKeyComponent})
                    Else
                        ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = Child.GetCategoricalVariableValue("Spelling"), .IsScoredItem = True})
                    End If

                    CurrentTestTrial.Tasks += 1
                Next

            End If

        Else
            'Adding the current word spelling as a response alternative

            ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = CurrentTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"), .IsScoredItem = CurrentTestTrial.SpeechMaterialComponent.IsKeyComponent})
            CurrentTestTrial.Tasks = 1

            'Picking random response alternatives from all available test words
            Dim AllContrastingWords = NextTestWord.GetAllRelativesAtLevelExludingSelf(SpeechMaterialComponent.LinguisticLevels.Sentence, True, False)
            Dim RandomIndices = Utils.SampleWithoutReplacement(Math.Max(0, FixedResponseAlternativeCount - 1), 0, AllContrastingWords.Count, Randomizer)
            For Each RandomIndex In RandomIndices
                ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = AllContrastingWords(RandomIndex).GetCategoricalVariableValue("Spelling"), .IsScoredItem = AllContrastingWords(RandomIndex).IsKeyComponent})
            Next

            'Shuffling the order of response alternatives
            ResponseAlternatives = Utils.Shuffle(ResponseAlternatives, Randomizer).ToList
        End If

        CurrentTestTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

        'Mixing trial sound
        MixNextTrialSound()

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 500, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 501, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        If IsFreeRecall = False Then CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 5500, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

    End Sub



    Private Sub MixNextTrialSound()

        Dim RETSPL_Correction As Double = 0
        If LevelsAreIn_dBHL = True Then
            RETSPL_Correction = Transducer.RETSPL_Speech
        End If


        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(MediaSet, 0, 1, , , , , False, False, False, , , False)

        Dim NominalLevel_FS = TestWordSound.SMA.NominalLevel
        Dim TargetLevel_FS = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel) + RETSPL_Correction
        Dim NeededGain = TargetLevel_FS - NominalLevel_FS

        Audio.DSP.AmplifySection(TestWordSound, NeededGain)

        'Setting level
        If HasNoise = True Then
            Dim Noise = CurrentTestTrial.SpeechMaterialComponent.GetMaskerSound(MediaSet, 0)
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
        Throw New NotImplementedException()
    End Function

    Public Overrides Function GetTestTrialResultExportString() As String
        Return "Export of trial level test results is not yet implemented"
    End Function

    Public Overrides Function GetTestResultsExportString() As String
        Throw New NotImplementedException()
    End Function

    'Public Overrides Function GetResults() As TestResults

    '    Dim ProtocolThreshold = SelectedTestProtocol.GetFinalResult()

    '    Dim Output = New TestResults(TestResults.TestResultTypes.SRT)
    '    If ProtocolThreshold.HasValue Then
    '        Output.AdaptiveLevelThreshold = ProtocolThreshold
    '    Else
    '        'Storing NaN if no threshold was reached
    '        Output.AdaptiveLevelThreshold = Double.NaN
    '    End If

    '    'Calculating the SRT based on the adaptive threshold
    '    Select Case SelectedTestMode
    '        Case TestModes.AdaptiveSpeech
    '            Output.SpeechRecognitionThreshold = Output.AdaptiveLevelThreshold
    '        Case TestModes.AdaptiveNoise
    '            Output.SpeechRecognitionThreshold = Output.AdaptiveLevelThreshold
    '        Case Else
    '            Throw New NotImplementedException
    '    End Select

    '    'Storing the AdaptiveLevelSeries
    '    Output.AdaptiveLevelSeries = New List(Of Double)
    '    Output.SpeechLevelSeries = New List(Of Double)
    '    Output.MaskerLevelSeries = New List(Of Double)
    '    Output.ContralateralMaskerLevelSeries = New List(Of Double)
    '    Output.SNRLevelSeries = New List(Of Double)
    '    Output.TestStageSeries = New List(Of String)
    '    Output.ProportionCorrectSeries = New List(Of String)
    '    Output.ScoreSeries = New List(Of String)
    '    For Each Trial As SrtTrial In ObservedTrials
    '        Output.AdaptiveLevelSeries.Add(Math.Round(Trial.AdaptiveValue))
    '        Output.SpeechLevelSeries.Add(Math.Round(Trial.SpeechLevel))
    '        Output.MaskerLevelSeries.Add(Math.Round(Trial.MaskerLevel))
    '        Output.ContralateralMaskerLevelSeries.Add(Math.Round(Trial.ContralateralMaskerLevel))
    '        Output.SNRLevelSeries.Add(Math.Round(Trial.SNR))
    '        Output.TestStageSeries.Add(Trial.TestStage)
    '        Output.ProportionCorrectSeries.Add(Trial.GetProportionTasksCorrect)
    '        If Trial.IsCorrect = True Then
    '            Output.ScoreSeries.Add("Correct")
    '        Else
    '            Output.ScoreSeries.Add("Incorrect")
    '        End If
    '    Next

    '    Return Output

    'End Function

    Public Overrides Sub FinalizeTest()

        TestProtocol.FinalizeProtocol(ObservedTrials)

    End Sub

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)
        Throw New NotImplementedException
    End Function

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        Throw New NotImplementedException()
    End Sub

End Class




