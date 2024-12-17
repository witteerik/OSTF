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
            Return TriState.True
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

    Public Overrides ReadOnly Property MinimumLevel As Double = 35
    Public Overrides ReadOnly Property MaximumLevel As Double = 85

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
        CustomizableTestOptions.SpeechLevel = 65

        If CustomizableTestOptions.SignalLocations.Count = 0 Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one signal sound source!")
        End If

        If CustomizableTestOptions.MaskerLocations.Count = 0 And CustomizableTestOptions.SelectedTestMode = TestModes.AdaptiveNoise Then
            Return New Tuple(Of Boolean, String)(False, "You must select at least one masker sound source in tests with adaptive noise!")
        End If

        Dim StartAdaptiveLevel As Double
        If CustomizableTestOptions.MaskerLocations.Count > 0 Then
            'It's a speech in noise test, using adaptive SNR
            Dim InitialSNR = SignalToNoiseRatio(CustomizableTestOptions.SpeechLevel, CustomizableTestOptions.MaskingLevel)
            StartAdaptiveLevel = InitialSNR
        Else
            'It's a speech only test, using adaptive speech level
            StartAdaptiveLevel = CustomizableTestOptions.SpeechLevel
        End If

        CustomizableTestOptions.SelectedTestProtocol.IsInPretestMode = CustomizableTestOptions.IsPractiseTest

        CreatePlannedWordsList()

        CustomizableTestOptions.SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.TestStage = 0, .AdaptiveValue = StartAdaptiveLevel})

        Return New Tuple(Of Boolean, String)(True, "")

    End Function

    Private Function CreatePlannedWordsList() As Boolean

        'Adding MaximumNumberOfTestSentences words, starting from the start list (excluding practise items), and re-using lists if needed 
        Dim TempAvailableLists As New List(Of SpeechMaterialComponent)
        Dim AllAvailableLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, False, False)

        Dim AllLists As New List(Of SpeechMaterialComponent)
        'Filtering out lists which are or are not pracise lists depending on the selected value in CustomizableTestOptions
        For Each List In AllAvailableLists
            If List.IsPractiseComponent = CustomizableTestOptions.SelectedTestProtocol.IsInPretestMode Then
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
        If CustomizableTestOptions.SelectedTestProtocol.IsInPretestMode = False Then
            '...based on the CustomizableTestOptions.StartList 
            For i = 0 To AllLists.Count - 1
                If AllLists(i).PrimaryStringRepresentation = CustomizableTestOptions.StartList Then
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

            If CustomizableTestOptions.RandomizeItemsWithinLists = False Then
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

                If CustomizableTestOptions.ScoreOnlyKeyWords = True Then
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
        Dim ProtocolReply = CustomizableTestOptions.SelectedTestProtocol.NewResponse(ObservedTrials)

        'Preparing a full test if the practise list is finished
        If CustomizableTestOptions.SelectedTestProtocol.IsInPretestMode = True Then
            If ProtocolReply.Decision = SpeechTestReplies.TestIsCompleted Then

                'Finalizing the protocol
                CustomizableTestOptions.SelectedTestProtocol.FinalizeProtocol(ObservedTrials)

                'Showing results in the GUI
                GetResultStringForGui()

                'Saving results of the practise test
                SaveTableFormatedTestResults()

                'Initializing a new test protocol for main testing stage and move directly to test mode
                'Setting the start value in the new protocol to the current AdaptiveValue
                Dim NewProtocol As Object = Nothing
                For Each AvailableProtocol In Me.AvailableTestProtocols
                    If AvailableProtocol.GetType = CustomizableTestOptions.SelectedTestProtocol.GetType Then
                        NewProtocol = AvailableProtocol
                        Exit For
                    End If
                Next
                CustomizableTestOptions.SelectedTestProtocol = NewProtocol

                'Initializing the new protocol with the adaptive threshold determined in the practise test as the start value
                CustomizableTestOptions.SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.TestStage = 0, .AdaptiveValue = ProtocolReply.AdaptiveValue})

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
        Select Case CustomizableTestOptions.SelectedTestMode
            Case TestModes.AdaptiveSpeech

                If CustomizableTestOptions.MaskerLocations.Count > 0 Then

                    CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
            .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
            .SpeechLevel = CustomizableTestOptions.MaskingLevel + NextTaskInstruction.AdaptiveValue,
            .MaskerLevel = CustomizableTestOptions.MaskingLevel,
            .ContralateralMaskerLevel = CustomizableTestOptions.ContralateralMaskingLevel,
            .TestStage = NextTaskInstruction.TestStage,
            .Tasks = 1}

                Else

                    CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
            .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
            .SpeechLevel = NextTaskInstruction.AdaptiveValue,
            .MaskerLevel = Double.NegativeInfinity,
            .ContralateralMaskerLevel = CustomizableTestOptions.ContralateralMaskingLevel,
            .TestStage = NextTaskInstruction.TestStage,
            .Tasks = 1}

                End If


            Case TestModes.AdaptiveNoise

                CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
            .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
            .SpeechLevel = CustomizableTestOptions.SpeechLevel,
            .MaskerLevel = CustomizableTestOptions.SpeechLevel - NextTaskInstruction.AdaptiveValue,
            .ContralateralMaskerLevel = CustomizableTestOptions.ContralateralMaskingLevel,
            .TestStage = NextTaskInstruction.TestStage,
            .Tasks = 1}

            Case Else
                Throw New NotImplementedException
        End Select


        CurrentTestTrial.ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))

        Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)
        If CustomizableTestOptions.IsFreeRecall Then
            If CurrentTestTrial.SpeechMaterialComponent.ChildComponents.Count > 0 Then

                CurrentTestTrial.Tasks = 0
                For Each Child In CurrentTestTrial.SpeechMaterialComponent.ChildComponents()

                    If CustomizableTestOptions.ScoreOnlyKeyWords = True Then
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

        'Mixing trial sound
        MixNextTrialSound()

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        'CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1000, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        'CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1001, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})

        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 500, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * TestWordPresentationTime), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * (TestWordPresentationTime + MaximumResponseTime)), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

    End Sub



    Private Sub MixNextTrialSound()

        Dim UseNominalLevels As Boolean = True

        Dim TargetLevel_SPL As Double = DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel
        Dim MaskerLevel_SPL As Double = DirectCast(CurrentTestTrial, SrtTrial).MaskerLevel
        Dim ContralateralMaskerLevel_SPL As Double = DirectCast(CurrentTestTrial, SrtTrial).ContralateralMaskerLevel + CustomizableTestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain

        Dim TargetPresentationTime As Double = TestWordPresentationTime
        Dim MaskerPresentationTime As Double = 0
        Dim ContralateralMaskerPresentationTime As Double = MaskerPresentationTime


        'Mix the signal using DuxplexMixer CreateSoundScene
        'Sets a List of SoundSceneItem in which to put the sounds to mix
        Dim ItemList = New List(Of SoundSceneItem)
        Dim LevelGroup As Integer = 1 ' The level group value is used to set the added sound level of items sharing the same (arbitrary) LevelGroup value to the indicated sound level. (Thus, the sounds with the same LevelGroup value are measured together.)
        Dim CurrentSampleRate As Integer = -1

        'Determining test ear and stores in the current test trial (This should perhaps be moved outside this function. On the other hand it's good that it's always detemined when sounds are mixed, though all tests need to implement this or call this code)
        Dim CurrentTestEar As Utils.SidesWithBoth = Utils.SidesWithBoth.Both ' Assuming both, and overriding if needed
        If CustomizableTestOptions.SelectedTransducer.IsHeadphones = True Then
            If CustomizableTestOptions.UseSimulatedSoundField = False Then
                Dim HasLeftSideTarget As Boolean = False
                Dim HasRightSideTarget As Boolean = False

                For Each SignalLocation In CustomizableTestOptions.SignalLocations
                    If SignalLocation.HorizontalAzimuth > 0 Then
                        'At least one signal location is to the right
                        HasRightSideTarget = True
                    End If
                    If SignalLocation.HorizontalAzimuth < 0 Then
                        'At least one signal location is to the left
                        HasLeftSideTarget = True
                    End If
                Next

                'Overriding the value Both if signal is only the left or only the right side
                If HasLeftSideTarget = True And HasRightSideTarget = False Then
                    CurrentTestEar = Utils.Constants.SidesWithBoth.Left
                ElseIf HasLeftSideTarget = False And HasRightSideTarget = True Then
                    CurrentTestEar = Utils.Constants.SidesWithBoth.Right
                End If
            End If
        End If
        CurrentTestTrial.TestEar = CurrentTestEar

        ' **TARGET SOUNDS**
        If CustomizableTestOptions.SignalLocations.Count > 0 Then

            'Getting the target sound (i.e. test words)
            Dim TargetSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(CustomizableTestOptions.SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)

            'Storing the samplerate
            CurrentSampleRate = TargetSound.WaveFormat.SampleRate

            'Setting the insertion sample of the target
            Dim TargetStartSample As Integer = Math.Floor(TargetPresentationTime * CurrentSampleRate)

            'Setting the TargetStartMeasureSample (i.e. the sample index in the TargetSound, not in the final mix)
            Dim TargetStartMeasureSample As Integer = 0

            'Getting the TargetMeasureLength from the length of the sound files (i.e. everything is measured)
            Dim TargetMeasureLength As Integer = TargetSound.WaveData.SampleData(1).Length

            'Sets up fading specifications for the target
            Dim FadeSpecs_Target = New List(Of STFN.Audio.DSP.Transformations.FadeSpecifications)
            FadeSpecs_Target.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.002))
            FadeSpecs_Target.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.002))

            'Combining targets with the selected SignalLocations
            Dim Targets As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation))
            For Each SignalLocation In CustomizableTestOptions.SignalLocations
                'Re-using the same target in all selected locations
                Targets.Add(New Tuple(Of Audio.Sound, SoundSourceLocation)(TargetSound, SignalLocation))
            Next

            'Adding the targets sources to the ItemList
            For Index = 0 To Targets.Count - 1
                ItemList.Add(New SoundSceneItem(Targets(Index).Item1, 1, TargetLevel_SPL, LevelGroup, Targets(Index).Item2, SoundSceneItem.SoundSceneItemRoles.Target, TargetStartSample, TargetStartMeasureSample, TargetMeasureLength,, FadeSpecs_Target))
            Next

            'Incrementing LevelGroup 
            LevelGroup += 1

            'Storing data in the CurrentTestTrial (TODO: this should probably be moved out of this function)
            CurrentTestTrial.LinguisticSoundStimulusStartTime = TargetPresentationTime
            CurrentTestTrial.LinguisticSoundStimulusDuration = TargetSound.WaveData.SampleData(1).Length / TargetSound.WaveFormat.SampleRate

        End If


        ' **MASKER SOUNDS**
        If CustomizableTestOptions.MaskerLocations.Count > 0 Then

            'Getting the masker sound
            Dim MaskerSound = CurrentTestTrial.SpeechMaterialComponent.GetMaskerSound(CustomizableTestOptions.SelectedMediaSet, 0)

            'Storing the samplerate
            If CurrentSampleRate = -1 Then CurrentSampleRate = MaskerSound.WaveFormat.SampleRate

            'Setting the insertion sample of the masker
            Dim MaskerStartSample As Integer = Math.Floor(MaskerPresentationTime * CurrentSampleRate)

            'Setting the MaskerStartMeasureSample (i.e. the sample index in the MaskerSound, not in the final mix)
            Dim MaskerStartMeasureSample As Integer = 0

            'Getting the MaskerMeasureLength from the length of the sound files (i.e. everything is measured)
            Dim MaskerMeasureLength As Integer = MaskerSound.WaveData.SampleData(1).Length

            'Sets up fading specifications for the masker
            Dim FadeSpecs_Masker = New List(Of STFN.Audio.DSP.Transformations.FadeSpecifications)
            FadeSpecs_Masker.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.002))
            FadeSpecs_Masker.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.002))

            'Combining maskers with the selected SignalLocations
            Dim Maskers As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation))

            'Randomizing a start sample in the first half of the masker signal, and then picking different sections of the masker sound, two seconds apart for the different locations.
            Dim RandomStartReadIndex As Integer = Randomizer.Next(0, (MaskerSound.WaveData.SampleData(1).Length / 2) - 1)
            Dim InterMaskerStepLength As Integer = CurrentSampleRate * 2
            Dim IndendedMaskerDuration As Integer = MaximumSoundDuration * CurrentSampleRate

            'Calculating the needed sound length and checks that the masker sound is long enough
            Dim NeededSoundLength As Integer = RandomStartReadIndex + (CustomizableTestOptions.MaskerLocations.Count + 1) * InterMaskerStepLength + IndendedMaskerDuration + 10
            If MaskerSound.WaveData.SampleData(1).Length < NeededSoundLength Then
                Throw New Exception("The masker sound specified is too short for the intended maximum sound duration and " & CustomizableTestOptions.MaskerLocations.Count & " sound sources!")
            End If

            'Picking the masker sounds and combining them with their selected locations
            For Index = 0 To CustomizableTestOptions.MaskerLocations.Count - 1

                Dim StartCopySample As Integer = RandomStartReadIndex + Index * InterMaskerStepLength
                Dim CurrentSourceMaskerSound = Audio.DSP.CopySection(MaskerSound, StartCopySample, IndendedMaskerDuration, 1)

                'Copying the SMA object to retain the nominal level (although other time data and other related stuff will be incorrect, if not adjusted for)
                CurrentSourceMaskerSound.SMA = MaskerSound.SMA.CreateCopy(CurrentSourceMaskerSound)

                'Picking the masker sound
                Maskers.Add(New Tuple(Of Audio.Sound, SoundSourceLocation)(CurrentSourceMaskerSound, CustomizableTestOptions.MaskerLocations(Index)))

            Next

            'Adding the maskers sources to the ItemList
            For Index = 0 To Maskers.Count - 1
                ItemList.Add(New SoundSceneItem(Maskers(Index).Item1, 1, MaskerLevel_SPL, LevelGroup, Maskers(Index).Item2, SoundSceneItem.SoundSceneItemRoles.Masker, MaskerStartSample, MaskerStartMeasureSample, MaskerMeasureLength,, FadeSpecs_Masker))
            Next

            'Incrementing LevelGroup 
            LevelGroup += 1

        End If


        ' **CONTRALATERAL MASKER**
        If CustomizableTestOptions.UseContralateralMasking = True Then

            'Ensures that head phones are used
            If CustomizableTestOptions.SelectedTransducer.IsHeadphones = False Then
                Throw New Exception("Contralateral masking cannot be used without headphone presentation.")
            End If

            'Ensures that it's not a simulated sound field
            If CustomizableTestOptions.UseSimulatedSoundField = True Then
                Throw New Exception("Contralateral masking cannot be used in a simulated sound field!")
            End If

            'Getting the contralateral masker sound 
            Dim FullContralateralMaskerSound = CurrentTestTrial.SpeechMaterialComponent.GetContralateralMaskerSound(CustomizableTestOptions.SelectedMediaSet, 0)

            'Picking a random section of the ContralateralMaskerSound, starting in the first half
            Dim RandomStartReadIndex As Integer = Randomizer.Next(0, (FullContralateralMaskerSound.WaveData.SampleData(1).Length / 2) - 1)
            Dim IndendedMaskerDuration As Integer = MaximumSoundDuration * CurrentSampleRate

            'Calculating the needed sound length and checks that the contralateral masker sound is long enough
            Dim NeededSoundLength As Integer = RandomStartReadIndex + IndendedMaskerDuration + 10
            If FullContralateralMaskerSound.WaveData.SampleData(1).Length < NeededSoundLength Then
                Throw New Exception("The contralateral masker sound specified is too short for the intended maximum sound duration!")
            End If

            'Gets a copy of the sound section
            Dim ContralateralMaskerSound = Audio.DSP.CopySection(FullContralateralMaskerSound, RandomStartReadIndex, IndendedMaskerDuration, 1)

            'Copying the SMA object to retain the nominal level (although other time data and other related stuff will be incorrect, if not adjusted for)
            ContralateralMaskerSound.SMA = FullContralateralMaskerSound.SMA.CreateCopy(ContralateralMaskerSound)

            'Storing the samplerate
            If CurrentSampleRate = -1 Then CurrentSampleRate = ContralateralMaskerSound.WaveFormat.SampleRate

            'Setting the insertion sample of the contralateral masker
            Dim ContralateralMaskerStartSample As Integer = Math.Floor(ContralateralMaskerPresentationTime * CurrentSampleRate)

            'Setting the ContralateralMaskerStartMeasureSample (i.e. the sample index in the ContralateralMaskerSound, not in the final mix)
            Dim ContralateralMaskerStartMeasureSample As Integer = 0

            'Getting the ContralateralMaskerMeasureLength from the length of the sound files (i.e. everything is measured)
            Dim ContralateralMaskerMeasureLength As Integer = ContralateralMaskerSound.WaveData.SampleData(1).Length

            'Sets up fading specifications for the contralateral masker
            Dim FadeSpecs_ContralateralMasker = New List(Of STFN.Audio.DSP.Transformations.FadeSpecifications)
            FadeSpecs_ContralateralMasker.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.002))
            FadeSpecs_ContralateralMasker.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.002))

            'Determining which side to put the contralateral masker
            Dim ContralateralMaskerLocation As New SoundSourceLocation With {.Distance = 0, .Elevation = 0}
            If CurrentTestEar = Utils.Constants.SidesWithBoth.Left Then
                'Putting contralateral masker in right ear
                ContralateralMaskerLocation.HorizontalAzimuth = 90
            ElseIf CurrentTestEar = Utils.Constants.SidesWithBoth.Right Then
                'Putting contralateral masker in left ear
                ContralateralMaskerLocation.HorizontalAzimuth = -90
            Else
                'This shold never happen...
                Throw New Exception("Contralateral noise cannot be used when the target signal is on both sides!")
            End If

            'Adding the contralateral maskers sources to the ItemList
            ItemList.Add(New SoundSceneItem(ContralateralMaskerSound, 1, ContralateralMaskerLevel_SPL, LevelGroup, ContralateralMaskerLocation, SoundSceneItem.SoundSceneItemRoles.ContralateralMasker, ContralateralMaskerStartSample, ContralateralMaskerStartMeasureSample, ContralateralMaskerMeasureLength,, FadeSpecs_ContralateralMasker))

            'Incrementing LevelGroup 
            LevelGroup += 1

        End If


        Dim CurrentSoundPropagationType As SoundPropagationTypes = SoundPropagationTypes.PointSpeakers
        If CustomizableTestOptions.UseSimulatedSoundField Then
            CurrentSoundPropagationType = SoundPropagationTypes.SimulatedSoundField
            'TODO: This needs to be modified if/when more SoundPropagationTypes are starting to be supported
        End If

        'Creating the mix by calling CreateSoundScene of the current Mixer
        CurrentTestTrial.Sound = CustomizableTestOptions.SelectedTransducer.Mixer.CreateSoundScene(ItemList, UseNominalLevels, CustomizableTestOptions.UseRetsplCorrection, CurrentSoundPropagationType)

        'Storing other data into CurrentTestTrial (TODO: this should probably be moved out of this function)
        CurrentTestTrial.MaximumResponseTime = MaximumResponseTime

        'Also stores the mediaset
        CurrentTestTrial.MediaSetName = CustomizableTestOptions.SelectedMediaSet.MediaSetName

        'And the contralateral noise on/off setting
        CurrentTestTrial.UseContralateralNoise = CustomizableTestOptions.UseContralateralMasking

        'And the EM term
        CurrentTestTrial.EfficientContralateralMaskingTerm = CustomizableTestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain

        'DirectCast(CurrentTestTrial, SrtTrial).TestEar = ... TODO. Set this!

        CurrentTestTrial.IsPractiseTrial = CustomizableTestOptions.SelectedTestProtocol.IsInPretestMode

    End Sub

    Public Overrides Sub FinalizeTest()

        CustomizableTestOptions.SelectedTestProtocol.FinalizeProtocol(ObservedTrials)

    End Sub

    Public Overrides Function GetResultStringForGui() As String

        Dim ProtocolThreshold = CustomizableTestOptions.SelectedTestProtocol.GetFinalResult()

        Dim Output As New List(Of String)

        If ProtocolThreshold IsNot Nothing Then
            If CustomizableTestOptions.SelectedTestProtocol.IsInPretestMode = True Then
                ResultSummaryForGUI.Add("Resultat för övningstestet: SNR = " & vbTab & Math.Round(ProtocolThreshold.Value) & " dB")
            Else
                ResultSummaryForGUI.Add("Testresultat: SNR = " & vbTab & Math.Round(ProtocolThreshold.Value) & " dB")
            End If

            Output.AddRange(ResultSummaryForGUI)
        Else
            If CustomizableTestOptions.SelectedTestProtocol.IsInPretestMode = True Then
                Output.Add("Övningstest!")
            End If

            If CurrentTestTrial IsNot Nothing Then
                Output.Add("Mening nummer " & ObservedTrials.Count + 1 & " av " & CustomizableTestOptions.SelectedTestProtocol.TotalTrialCount)
                Output.Add("SNR = " & Math.Round(DirectCast(CurrentTestTrial, SrtTrial).SNR) & " dB HL")
                Output.Add("Talnivå = " & Math.Round(DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel) & " dB HL")
                Output.Add("Brusnivå = " & Math.Round(DirectCast(CurrentTestTrial, SrtTrial).MaskerLevel) & " dB HL")
                If CustomizableTestOptions.UseContralateralMasking = True Then
                    Output.Add("Kontralateral brusnivå = " & Math.Round(DirectCast(CurrentTestTrial, SrtTrial).ContralateralMaskerLevel) & " dB HL")
                End If
            End If
        End If

        Return String.Join(vbCrLf, Output)

    End Function

    Private ResultSummaryForGUI As New List(Of String)

    Public Overrides Function GetExportString() As String

        Dim ExportStringList As New List(Of String)

        Dim ProtocolThreshold = CustomizableTestOptions.SelectedTestProtocol.GetFinalResult()

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

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)
        Throw New NotImplementedException("Creating pre-test stimuli is not supported in the HINT test.")
    End Function

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        'This is not used in HINT, just ignores any calls
        'Throw New NotImplementedException()
    End Sub


End Class




