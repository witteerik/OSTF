﻿Imports MathNet.Numerics

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

    Public Overrides ReadOnly Property AllowsUseRetsplChoice As Boolean
        Get
            Return False
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
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property SupportsPrelistening As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property UseSoundFieldSimulation As Utils.TriState
        Get
            Return Utils.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestModes As List(Of TestModes)
        Get
            Return New List(Of TestModes) From {TestModes.AdaptiveSpeech, TestModes.AdaptiveNoise}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestProtocols As List(Of TestProtocol)
        Get
            'Returning new instances of supported protocols
            Return New List(Of TestProtocol) From {
                New HagermanKinnefors1995_TestProtocol,
                New BrandKollmeier2002_TestProtocol}
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
            Return Utils.Constants.TriState.False
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

    Public Overrides ReadOnly Property DefaultReferenceLevel As Double = 65
    Public Overrides ReadOnly Property DefaultSpeechLevel As Double = 65
    Public Overrides ReadOnly Property DefaultMaskerLevel As Double = 65
    Public Overrides ReadOnly Property DefaultBackgroundLevel As Double = 50
    Public Overrides ReadOnly Property DefaultContralateralMaskerLevel As Double = 25

    Public Overrides ReadOnly Property MinimumReferenceLevel As Double = 0
    Public Overrides ReadOnly Property MaximumReferenceLevel As Double = 80

    Public Overrides ReadOnly Property MinimumLevel_Targets As Double = 0
    Public Overrides ReadOnly Property MaximumLevel_Targets As Double = 80

    Public Overrides ReadOnly Property MinimumLevel_Maskers As Double = 0
    Public Overrides ReadOnly Property MaximumLevel_Maskers As Double = 80

    Public Overrides ReadOnly Property MinimumLevel_Background As Double = 0
    Public Overrides ReadOnly Property MaximumLevel_Background As Double = 80

    Public Overrides ReadOnly Property MinimumLevel_ContralateralMaskers As Double = 0
    Public Overrides ReadOnly Property MaximumLevel_ContralateralMaskers As Double = 80


    Public Overrides ReadOnly Property AvailableExperimentNumbers As Integer()
        Get
            Return {}
        End Get
    End Property


    Private MaximumSoundDuration As Double = 21
    Private TestWordPresentationTime As Double = 0.5
    Private MaximumResponseTime As Double = 20.5

#End Region


    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

    End Sub

    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        ObservedTrials = New TrialHistory

        If TestOptions.SignalLocations.Count = 0 Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one signal sound source!")
        End If

        If TestOptions.MaskerLocations.Count = 0 And TestOptions.SelectedTestMode = TestModes.AdaptiveNoise Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one masker sound source in tests with adaptive noise!")
        End If

        Dim StartAdaptiveLevel As Double
        If TestOptions.MaskerLocations.Count > 0 Then
            'It's a speech in noise test, using adaptive SNR
            HasNoise = True
            Dim InitialSNR = SignalToNoiseRatio(TestOptions.SpeechLevel, TestOptions.MaskingLevel)
            StartAdaptiveLevel = InitialSNR
        Else
            'It's a speech only test, using adaptive speech level
            HasNoise = False
            StartAdaptiveLevel = TestOptions.SpeechLevel
        End If

        TestOptions.SelectedTestProtocol.IsInPretestMode = TestOptions.IsPractiseTest

        Dim TestLength As Integer

        Select Case True
            Case TypeOf TestOptions.SelectedTestProtocol Is HagermanKinnefors1995_TestProtocol

                If HasNoise = False Then
                    DirectCast(TestOptions.SelectedTestProtocol, HagermanKinnefors1995_TestProtocol).AdaptiveType = HagermanKinnefors1995_TestProtocol.AdaptiveTypes.ThresholdInSilence
                    TestOptions.SelectedTestMode = TestModes.AdaptiveSpeech
                    TestLength = 20
                Else
                    If TestOptions.IsPractiseTest = True Then
                        DirectCast(TestOptions.SelectedTestProtocol, HagermanKinnefors1995_TestProtocol).AdaptiveType = HagermanKinnefors1995_TestProtocol.AdaptiveTypes.PractiseTestThresholdInNoise
                        TestOptions.SelectedTestMode = TestModes.AdaptiveNoise
                        TestLength = 30
                    Else
                        DirectCast(TestOptions.SelectedTestProtocol, HagermanKinnefors1995_TestProtocol).AdaptiveType = HagermanKinnefors1995_TestProtocol.AdaptiveTypes.ThresholdInNoise
                        TestOptions.SelectedTestMode = TestModes.AdaptiveNoise
                        TestLength = 20
                    End If
                End If

            Case TypeOf TestOptions.SelectedTestProtocol Is BrandKollmeier2002_TestProtocol

                DirectCast(TestOptions.SelectedTestProtocol, HagermanKinnefors1995_TestProtocol).AdaptiveType = HagermanKinnefors1995_TestProtocol.AdaptiveTypes.ThresholdInNoise
                TestOptions.SelectedTestMode = TestModes.AdaptiveNoise
                TestOptions.SpeechLevel = 65
                TestOptions.MaskingLevel = 65
                StartAdaptiveLevel = SignalToNoiseRatio(TestOptions.SpeechLevel, TestOptions.MaskingLevel)
                TestLength = 20

            Case Else

                If HasNoise = True Then
                    'It's a speech in noise test, using adaptive SNR
                    StartAdaptiveLevel = SignalToNoiseRatio(TestOptions.SpeechLevel, TestOptions.MaskingLevel)
                Else
                    'It's a speech only test, using adaptive speech level
                    StartAdaptiveLevel = TestOptions.SpeechLevel
                End If

                TestLength = 20

        End Select

        CreatePlannedWordsSentences()

        TestOptions.SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = StartAdaptiveLevel, .TestStage = 0, .TestLength = TestLength})

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
            If AllLists(i).PrimaryStringRepresentation = TestOptions.StartList Then
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
            If TestOptions.RandomizeItemsWithinLists = False Then
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
        Dim ProtocolReply = TestOptions.SelectedTestProtocol.NewResponse(ObservedTrials)

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
        Select Case TestOptions.SelectedTestMode
            Case TestModes.AdaptiveSpeech

                If HasNoise = True Then

                    CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestSentence,
                        .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
                        .SpeechLevel = TestOptions.MaskingLevel + NextTaskInstruction.AdaptiveValue,
                        .MaskerLevel = TestOptions.MaskingLevel,
                        .ContralateralMaskerLevel = TestOptions.ContralateralMaskingLevel,
                        .TestStage = NextTaskInstruction.TestStage,
                        .Tasks = 5}

                Else

                    CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestSentence,
                        .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
                        .SpeechLevel = NextTaskInstruction.AdaptiveValue,
                        .MaskerLevel = Double.NegativeInfinity,
                        .ContralateralMaskerLevel = TestOptions.ContralateralMaskingLevel,
                        .TestStage = NextTaskInstruction.TestStage,
                        .Tasks = 5}

                End If


            Case TestModes.AdaptiveNoise

                CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestSentence,
                    .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
                    .SpeechLevel = TestOptions.SpeechLevel,
                    .MaskerLevel = TestOptions.SpeechLevel - NextTaskInstruction.AdaptiveValue,
                    .ContralateralMaskerLevel = TestOptions.ContralateralMaskingLevel,
                    .TestStage = NextTaskInstruction.TestStage,
                    .Tasks = 5}

            Case Else
                Throw New NotImplementedException
        End Select


        Dim ResponseAlternativeSpellingsList As New List(Of List(Of String))

        If TestOptions.IsFreeRecall = True Then

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
            If TestOptions.ShowDidNotHearResponseAlternative = True Then
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
        MixStandardTestTrialSound(UseNominalLevels:=True, MaximumSoundDuration:=MaximumSoundDuration,
                          TargetLevel:=DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel,
                          TargetPresentationTime:=TestWordPresentationTime,
                          MaskerLevel:=DirectCast(CurrentTestTrial, SrtTrial).MaskerLevel,
                          ContralateralMaskerLevel:=DirectCast(CurrentTestTrial, SrtTrial).ContralateralMaskerLevel,
                          ExportSounds:=False)

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        'CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 500, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        'CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 501, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        'If TestOptions.IsFreeRecall = False Then CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 20500, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * TestWordPresentationTime), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        If TestOptions.IsFreeRecall = False Then CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * (TestWordPresentationTime + MaximumResponseTime)), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

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

        Dim ProtocolThreshold = TestOptions.SelectedTestProtocol.GetFinalResult()

        Dim Output As New List(Of String)

        If ProtocolThreshold IsNot Nothing Then
            If TestOptions.SelectedTestProtocol.IsInPretestMode = True Then
                ResultSummaryForGUI.Add("Resultat för övningstestet: SNR = " & vbTab & Math.Round(ProtocolThreshold.Value) & " dB")
            Else
                ResultSummaryForGUI.Add("Testresultat: SNR = " & vbTab & Math.Round(ProtocolThreshold.Value) & " dB")
            End If

            Output.AddRange(ResultSummaryForGUI)
        Else
            If TestOptions.SelectedTestProtocol.IsInPretestMode = True Then
                Output.Add("Övningstest!")
            End If

            If CurrentTestTrial IsNot Nothing Then
                Output.Add("Mening nummer " & ObservedTrials.Count + 1 & " av " & TestOptions.SelectedTestProtocol.TotalTrialCount)
                Output.Add("SNR = " & Math.Round(DirectCast(CurrentTestTrial, SrtTrial).SNR) & " dB HL")
                Output.Add("Talnivå = " & Math.Round(DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel) & " dB HL")
                Output.Add("Brusnivå = " & Math.Round(DirectCast(CurrentTestTrial, SrtTrial).MaskerLevel) & " dB HL")
                If TestOptions.UseContralateralMasking = True Then
                    Output.Add("Kontralateral brusnivå = " & Math.Round(DirectCast(CurrentTestTrial, SrtTrial).ContralateralMaskerLevel) & " dB HL")
                End If
            End If
        End If

        Return String.Join(vbCrLf, Output)

    End Function

    Private ResultSummaryForGUI As New List(Of String)

    Public Overrides Function GetTestTrialResultExportString() As String
        Return "Export of trial level test results is not yet implemented"
    End Function

    Public Overrides Function GetTestResultsExportString() As String

        Dim ExportStringList As New List(Of String)

        Dim ProtocolThreshold = TestOptions.SelectedTestProtocol.GetFinalResult()

        'Exporting all trials
        Dim TestTrialIndex As Integer = 0
        For i = 0 To ObservedTrials.Count - 1

            If TestTrialIndex = 0 Then
                ExportStringList.Add("TrialIndex" & vbTab & ObservedTrials(i).TestResultColumnHeadings & vbTab & "SRT")
            End If

            If i = ObservedTrials.Count - 1 Then
                'Exporting SRT on last row, last column, if determined
                If ProtocolThreshold.HasValue Then
                    ExportStringList.Add(i & vbTab & ObservedTrials(i).TestResultAsTextRow & vbTab & ProtocolThreshold)
                Else
                    ExportStringList.Add(i & vbTab & ObservedTrials(i).TestResultAsTextRow & vbTab & "SRT not established")
                End If
            Else
                ExportStringList.Add(i & vbTab & ObservedTrials(i).TestResultAsTextRow)
            End If

            TestTrialIndex += 1
        Next

        Return String.Join(vbCrLf, ExportStringList)

    End Function


    Public Overrides Sub FinalizeTest()

        TestOptions.SelectedTestProtocol.FinalizeProtocol(ObservedTrials)

    End Sub

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)
        Throw New NotImplementedException
    End Function

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        Throw New NotImplementedException()
    End Sub


End Class


