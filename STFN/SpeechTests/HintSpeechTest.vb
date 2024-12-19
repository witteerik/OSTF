Imports System.IO
Imports MathNet.Numerics
Imports MathNet.Numerics.Distributions
Imports STFN.TestProtocol
Imports STFN.Audio.SoundScene

Public Class HintSpeechTest
    Inherits SpeechTest
    Public Overrides ReadOnly Property FilePathRepresentation As String
        Get
            Return "HINT"
        End Get
    End Property

    ''' <summary>
    ''' This collection contains MaximumNumberOfTestWords which can be used troughout the test, in sequential order.
    ''' </summary>
    Private PlannedTestSentencess As List(Of SpeechMaterialComponent)

    Private MaximumNumberOfTestSentences As Integer = 40

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
            Return False
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
            Return False
        End Get
    End Property


    Public Overrides ReadOnly Property UseSoundFieldSimulation As Utils.TriState
        Get
            Return Utils.TriState.Optional
        End Get
    End Property



    Public Overrides ReadOnly Property AvailableTestModes As List(Of TestModes)
        Get
            'Return New List(Of TestModes) From {TestModes.AdaptiveSpeech, TestModes.AdaptiveNoise}
            Return New List(Of TestModes) From {TestModes.AdaptiveNoise}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestProtocols As List(Of TestProtocol)
        Get
            Return New List(Of TestProtocol) From {New SrtSwedishHint2018_TestProtocol}
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

#End Region

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
            Return 1
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
            Return Utils.Constants.TriState.True
        End Get
    End Property

    Public Overrides ReadOnly Property UseListOrderRandomization As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseWithinListRandomization As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseAcrossListRandomization As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseFreeRecall As Utils.TriState
        Get
            Return Utils.TriState.True
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
            Return 1
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

    Public Overrides ReadOnly Property MinimumLevel_Targets As Double = 60
    Public Overrides ReadOnly Property MaximumLevel_Targets As Double = 80

    Public Overrides ReadOnly Property MinimumLevel_Maskers As Double = 35
    Public Overrides ReadOnly Property MaximumLevel_Maskers As Double = 85

    Public Overrides ReadOnly Property MinimumLevel_Background As Double = 0
    Public Overrides ReadOnly Property MaximumLevel_Background As Double = 80

    Public Overrides ReadOnly Property MinimumLevel_ContralateralMaskers As Double = 0
    Public Overrides ReadOnly Property MaximumLevel_ContralateralMaskers As Double = 80

    Public Overrides ReadOnly Property AvailableExperimentNumbers As Integer()
        Get
            Return {}
        End Get
    End Property

    Private MaximumSoundDuration As Double = 10
    Private TestWordPresentationTime As Double = 0.5
    Private MaximumResponseTime As Double = 5

    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

    End Sub

    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        ObservedTrials = New TrialHistory

        ' Using a fixed speech level
        TestOptions.SpeechLevel = 65

        If TestOptions.SignalLocations.Count = 0 Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one signal sound source!")
        End If

        If TestOptions.MaskerLocations.Count = 0 And TestOptions.SelectedTestMode = TestModes.AdaptiveNoise Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one masker sound source in tests with adaptive noise!")
        End If

        Dim StartAdaptiveLevel As Double
        If TestOptions.MaskerLocations.Count > 0 Then
            'It's a speech in noise test, using adaptive SNR
            Dim InitialSNR = SignalToNoiseRatio(TestOptions.SpeechLevel, TestOptions.MaskingLevel)
            StartAdaptiveLevel = InitialSNR
        Else
            'It's a speech only test, using adaptive speech level
            StartAdaptiveLevel = TestOptions.SpeechLevel
        End If

        TestOptions.SelectedTestProtocol.IsInPretestMode = TestOptions.IsPractiseTest

        CreatePlannedWordsList()

        TestOptions.SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.TestStage = 0, .AdaptiveValue = StartAdaptiveLevel})

        Return New Tuple(Of Boolean, String)(True, "")

    End Function

    Private Function CreatePlannedWordsList() As Boolean

        'Adding MaximumNumberOfTestSentences words, starting from the start list (excluding practise items), and re-using lists if needed 
        Dim TempAvailableLists As New List(Of SpeechMaterialComponent)
        Dim AllAvailableLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, False, False)

        Dim AllLists As New List(Of SpeechMaterialComponent)
        'Filtering out lists which are or are not pracise lists depending on the selected value in TestOptions
        For Each List In AllAvailableLists
            If List.IsPractiseComponent = TestOptions.SelectedTestProtocol.IsInPretestMode Then
                AllLists.Add(List)
            End If
        Next

        Dim ListCount As Integer = AllLists.Count
        Dim TotalSentenceCount As Integer = 0
        For Each List In AllLists
            TotalSentenceCount += List.ChildComponents.Count
        Next

        'Calculating the number of loops around the material that is needed to get TotalSentenceCount sentences, and adding one loop to compensate for not starting the adding of sentences at the first list
        Dim LoopsNeeded As Integer = Math.Ceiling(TotalSentenceCount / MaximumNumberOfTestSentences) + 1
        'Adding the number of lists needed 
        For i = 1 To LoopsNeeded
            TempAvailableLists.AddRange(AllLists)
        Next
        'Determines the index of the start list
        Dim SelectedStartListIndex As Integer = -1
        If TestOptions.SelectedTestProtocol.IsInPretestMode = False Then
            '...based on the TestOptions.StartList 
            For i = 0 To AllLists.Count - 1
                If AllLists(i).PrimaryStringRepresentation = TestOptions.StartList Then
                    SelectedStartListIndex = i
                    Exit For
                End If
            Next
        Else
            '...randomly from the number of practise lists
            If AllLists.Count = 0 Then
                Messager.MsgBox("Unable to add test sentences, probably since the selected speech material has no dedicated practise lists ",, "An error occurred!")
                Return False
            End If
            SelectedStartListIndex = Randomizer.Next(0, AllLists.Count)
        End If

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
        PlannedTestSentencess = New List(Of SpeechMaterialComponent)
        Dim TargetNumberOfSentencesReached As Boolean = False
        For Each List In ListsToUse
            Dim CurrentWords = List.GetChildren()

            If TestOptions.RandomizeItemsWithinLists = False Then
                For Each Word In CurrentWords
                    PlannedTestSentencess.Add(Word)
                    'Checking if enough sentences have been added
                    If PlannedTestSentencess.Count = MaximumNumberOfTestSentences Then
                        TargetNumberOfSentencesReached = True
                        Exit For
                    End If
                Next
            Else
                'Randomizing order
                Dim RandomizedOrder = Utils.SampleWithoutReplacement(CurrentWords.Count, 0, CurrentWords.Count, Randomizer)
                For Each RandomIndex In RandomizedOrder
                    PlannedTestSentencess.Add(CurrentWords(RandomIndex))
                    'Checking if enough words have been added
                    If PlannedTestSentencess.Count = MaximumNumberOfTestSentences Then
                        TargetNumberOfSentencesReached = True
                        Exit For
                    End If
                Next
            End If

            If TargetNumberOfSentencesReached = True Then
                'Breaking out of the outer loop if we have enough sentences
                Exit For
            End If

        Next

        'Checking that we really have MaximumNumberOfTestSentences words
        If MaximumNumberOfTestSentences <> PlannedTestSentencess.Count Then
            Messager.MsgBox("The wrong number of test items were added. It should have been " & MaximumNumberOfTestSentences & " but instead " & PlannedTestSentencess.Count & " items were added!",, "An error occurred!")
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

                If TestOptions.ScoreOnlyKeyWords = True Then
                    If WordsInSentence(i).IsKeyComponent = False Then
                        'In keyword correction mode, skipping to next if the word is not a keyword
                        Continue For
                    End If
                End If

                'Correcting the word
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

        'Preparing a full test if the practise list is finished
        If TestOptions.SelectedTestProtocol.IsInPretestMode = True Then
            If ProtocolReply.Decision = SpeechTestReplies.TestIsCompleted Then

                'Finalizing the protocol
                TestOptions.SelectedTestProtocol.FinalizeProtocol(ObservedTrials)

                'Showing results in the GUI
                GetResultStringForGui()

                'here we have to manually save the tst trial results, since ObservedTrials are reset before the code returns to the speech test form (from which it calls SaveTestTrialResults)
                SaveTestTrialResults()

                'Initializing a new test protocol for main testing stage and move directly to test mode
                'Setting the start value in the new protocol to the current AdaptiveValue
                Dim NewProtocol As Object = Nothing
                For Each AvailableProtocol In Me.AvailableTestProtocols
                    If AvailableProtocol.GetType = TestOptions.SelectedTestProtocol.GetType Then
                        NewProtocol = AvailableProtocol
                        Exit For
                    End If
                Next
                TestOptions.SelectedTestProtocol = NewProtocol

                'Initializing the new protocol with the adaptive threshold determined in the practise test as the start value
                TestOptions.SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.TestStage = 0, .AdaptiveValue = ProtocolReply.AdaptiveValue})

                'Clearing observed and planned sentences (since these are based on practise lists), and plan new lists based on the intended start list
                ObservedTrials.Clear()
                PlannedTestSentencess.Clear()
                CreatePlannedWordsList()

                ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial

            End If
        End If

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then
            PrepareNextTrial(ProtocolReply)
        End If

        Return ProtocolReply.Decision


    End Function

    Private Sub PrepareNextTrial(ByVal NextTaskInstruction As TestProtocol.NextTaskInstruction)

        'Preparing the next trial
        'Getting next test word
        Dim NextTestWord = PlannedTestSentencess(ObservedTrials.Count)

        'Creating a new test trial
        Select Case TestOptions.SelectedTestMode
            Case TestModes.AdaptiveSpeech

                If TestOptions.MaskerLocations.Count > 0 Then

                    CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
            .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
            .SpeechLevel = TestOptions.MaskingLevel + NextTaskInstruction.AdaptiveValue,
            .MaskerLevel = TestOptions.MaskingLevel,
            .ContralateralMaskerLevel = TestOptions.ContralateralMaskingLevel,
            .TestStage = NextTaskInstruction.TestStage,
            .Tasks = 1}

                Else

                    CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
            .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
            .SpeechLevel = NextTaskInstruction.AdaptiveValue,
            .MaskerLevel = Double.NegativeInfinity,
            .ContralateralMaskerLevel = TestOptions.ContralateralMaskingLevel,
            .TestStage = NextTaskInstruction.TestStage,
            .Tasks = 1}

                End If


            Case TestModes.AdaptiveNoise

                CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
            .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
            .SpeechLevel = TestOptions.SpeechLevel,
            .MaskerLevel = TestOptions.SpeechLevel - NextTaskInstruction.AdaptiveValue,
            .ContralateralMaskerLevel = TestOptions.ContralateralMaskingLevel,
            .TestStage = NextTaskInstruction.TestStage,
            .Tasks = 1}

            Case Else
                Throw New NotImplementedException
        End Select


        CurrentTestTrial.ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))

        Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)
        If TestOptions.IsFreeRecall Then
            If CurrentTestTrial.SpeechMaterialComponent.ChildComponents.Count > 0 Then

                CurrentTestTrial.Tasks = 0
                For Each Child In CurrentTestTrial.SpeechMaterialComponent.ChildComponents()

                    If TestOptions.ScoreOnlyKeyWords = True Then
                        Dim IsKeyComponent = Child.IsKeyComponent
                        ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = Child.GetCategoricalVariableValue("Spelling"), .IsScoredItem = IsKeyComponent})
                        If IsKeyComponent = True Then
                            CurrentTestTrial.Tasks += 1
                        End If
                    Else
                        ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = Child.GetCategoricalVariableValue("Spelling"), .IsScoredItem = True})
                    End If

                Next
            End If

        Else
            Throw New NotImplementedException
        End If

        CurrentTestTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

        'Storing other data into CurrentTestTrial (TODO: this should probably be moved out of this function)
        CurrentTestTrial.MaximumResponseTime = MaximumResponseTime

        'Mixing trial sound
        MixStandardTestTrialSound(UseNominalLevels:=True, MaximumSoundDuration:=MaximumSoundDuration,
                          TargetLevel:=DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel,
                          TargetPresentationTime:=TestWordPresentationTime,
                          MaskerLevel:=DirectCast(CurrentTestTrial, SrtTrial).MaskerLevel,
                          ContralateralMaskerLevel:=DirectCast(CurrentTestTrial, SrtTrial).ContralateralMaskerLevel,
                          ExportSounds:=False)

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        'CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1000, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        'CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1001, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})

        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * TestWordPresentationTime), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * (TestWordPresentationTime + MaximumResponseTime)), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

    End Sub


    Public Overrides Sub FinalizeTest()

        TestOptions.SelectedTestProtocol.FinalizeProtocol(ObservedTrials)

    End Sub

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

        If ObservedTrials.Count = 0 Then Return ""

        Dim ExportStringList As New List(Of String)

        Dim ProtocolThreshold = TestOptions.SelectedTestProtocol.GetFinalResult()

        'Exporting only the current trial (last added to ObservedTrials)
        Dim TestTrialIndex As Integer = ObservedTrials.Count - 1

        'Adding column headings on the first row
        If TestTrialIndex = 0 Then
            ExportStringList.Add("TrialIndex" & vbTab & ObservedTrials.Last.TestResultColumnHeadings & vbTab & "SRT" & vbTab & TestOptions.ExportColumnHeadings)
        End If

        'Adding trial data 
        If ProtocolThreshold.HasValue = False Then
            ExportStringList.Add(TestTrialIndex & vbTab & ObservedTrials.Last.TestResultAsTextRow & vbTab & "SRT not yet established" & vbTab & TestOptions.ExportSettingsAsTextRow)
        Else
            ExportStringList.Add(TestTrialIndex & vbTab & ObservedTrials.Last.TestResultAsTextRow & vbTab & ProtocolThreshold & vbTab & TestOptions.ExportSettingsAsTextRow)
        End If

        Return String.Join(vbCrLf, ExportStringList)

    End Function


    Public Overrides Function GetTestResultsExportString() As String

        'This test exports results only on trial-by-trial basis
        Return ""

    End Function

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)
        Throw New NotImplementedException("Creating pre-test stimuli is not supported in the HINT test.")
    End Function

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        'This is not used in HINT, just ignores any calls
        'Throw New NotImplementedException()
    End Sub


End Class




