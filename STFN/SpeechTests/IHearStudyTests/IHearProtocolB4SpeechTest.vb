Imports STFN.TestProtocol
Imports Utils

Public Class IHearProtocolB4SpeechTest
    Inherits SpeechTest

    Public Sub New(SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

    End Sub

    Public Overrides ReadOnly Property FilePathRepresentation As String
        Get
            Return "ProtocolB4"
        End Get
    End Property


    ''' <summary>
    ''' This collection contains MaximumNumberOfTestWords which can be used troughout the test, in sequential order.
    ''' </summary>
    Private PlannedTestWords As List(Of SpeechMaterialComponent)

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
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualMediaSetSelection As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualSpeechLevelSelection As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualMaskingLevelSelection As Boolean
        Get
            Return False
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
            Return Nothing
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestProtocols As List(Of TestProtocol)
        Get
            Return Nothing
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableFixedResponseAlternativeCounts As List(Of Integer)
        Get
            Return New List(Of Integer)
        End Get
    End Property

    Public Overrides ReadOnly Property AvailablePhaseAudiometryTypes As List(Of BmldModes)
        Get
            Return Nothing
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
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSoundFieldBackgroundNonSpeechLocations As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSoundFieldBackgroundSpeechLocations As Integer
        Get
            Return 0
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
            Return False
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
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseListOrderRandomization As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UseWithinListRandomization As Utils.TriState
        Get
            Return Utils.Constants.TriState.True
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

    Public Overrides ReadOnly Property UpperLevelLimit_dBSPL As Double
        Get
            Return 100
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

    Private IsInitialized As Boolean = False

    Private IsSecondTest As Boolean = False

    'TestOrder holds rendomized order in which the two tests are run (specifying MediaSet and List index)
    Private TestOrder As Integer

    Dim CacheLastTestListVariableName As String
    Dim StoredTestListIndex As Integer

    Private ContralateralNoise As Audio.Sound = Nothing
    Private SilentSound As Audio.Sound = Nothing

    Private MaximumSoundDuration As Double = 10
    Private TestWordPresentationTime As Double = 0.5
    Private MaximumResponseTime As Double = 5


    Public Overrides Function InitializeCurrentTest() As Boolean

        If IsInitialized = True Then Return True

        'Randomizing the order of media sets
        TestOrder = Utils.SampleWithoutReplacement(1, 0, 2, Randomizer)(0)

        ObservedTrials = New TrialHistory
        CustomizableTestOptions.SelectedTestProtocol = New SrtIso8253_TestProtocol
        CustomizableTestOptions.SelectedTestMode = TestModes.AdaptiveSpeech

        Dim StartAdaptiveLevel As Double = CustomizableTestOptions.SpeechLevel

        Dim AllTestListsNames = AvailableTestListsNames()
        Dim AllMediaSets = AvailableMediasets

        'Creating cache variable names for storing last test list index and voice between sessions
        CacheLastTestListVariableName = FilePathRepresentation & "LastTestList"

        'Selecting test list
        If Utils.AppCache.AppCacheVariableExists(CacheLastTestListVariableName) = True Then

            'Getting the last tested index
            StoredTestListIndex = Utils.AppCache.GetAppCacheIntegerVariableValue(CacheLastTestListVariableName)

        Else
            'Randomizing a new list number if no list has been run previously 
            StoredTestListIndex = Randomizer.Next(0, AllTestListsNames.Count)

            'Unwrapping TestListIndex
            If StoredTestListIndex > AllTestListsNames.Count - 1 Then
                StoredTestListIndex = 0
            End If

        End If

        CreatePlannedWordsList()

        CustomizableTestOptions.SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = StartAdaptiveLevel, .TestStage = 0})

        IsInitialized = True

        Return True

    End Function

    Private Sub InitializeSecondTest()

        'Store the results of the first test
        CustomizableTestOptions.SelectedTestProtocol.FinalizeProtocol(ObservedTrials)
        Dim Results = GetResults()
        SaveTextFormattedResults(Results)

        'Initialize second test
        ObservedTrials = New TrialHistory
        CustomizableTestOptions.SelectedTestProtocol = New SrtIso8253_TestProtocol
        CustomizableTestOptions.SelectedTestMode = TestModes.AdaptiveSpeech

        Dim StartAdaptiveLevel As Double = CustomizableTestOptions.SpeechLevel

        CreatePlannedWordsList()

        CustomizableTestOptions.SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = StartAdaptiveLevel, .TestStage = 0})


    End Sub

    Private Function CreatePlannedWordsList() As Boolean

        'Adding NumberOfWordsToAdd words, starting from the start list (excluding practise items), and re-using lists if needed 
        Dim TempAvailableLists As New List(Of SpeechMaterialComponent)
        Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, True, False)

        Dim CurrentListSMC As SpeechMaterialComponent

        'Determining which combination of MediaSet and test List that should be run. 
        If IsSecondTest = False Then
            'First test
            If TestOrder = 0 Then
                CustomizableTestOptions.SelectedMediaSet = AvailableMediasets(0)
                CurrentListSMC = AllLists(StoredTestListIndex)
            Else
                ' I.e. TestOrder = 1
                CustomizableTestOptions.SelectedMediaSet = AvailableMediasets(1)

                Dim CurrentListIndex As Integer = StoredTestListIndex + 1
                If CurrentListIndex > AllLists.Count - 1 Then
                    CurrentListIndex = 0
                End If
                CurrentListSMC = AllLists(CurrentListIndex)
            End If
        Else
            'Second test
            If TestOrder = 0 Then
                CustomizableTestOptions.SelectedMediaSet = AvailableMediasets(1)

                Dim CurrentListIndex As Integer = StoredTestListIndex + 1
                If CurrentListIndex > AllLists.Count - 1 Then
                    CurrentListIndex = 0
                End If
                CurrentListSMC = AllLists(CurrentListIndex)
            Else
                ' I.e. TestOrder = 1
                CustomizableTestOptions.SelectedMediaSet = AvailableMediasets(0)
                CurrentListSMC = AllLists(StoredTestListIndex)
            End If
        End If

        'Adding all planned test words, and stopping after NumberOfWordsToAdd have been added
        PlannedTestWords = New List(Of SpeechMaterialComponent)
        Dim CurrentWords = CurrentListSMC.GetChildren()

        'Adding in randomized order
        Dim RandomizedOrder = Utils.SampleWithoutReplacement(CurrentWords.Count, 0, CurrentWords.Count, Randomizer)
        For Each RandomIndex In RandomizedOrder
            PlannedTestWords.Add(CurrentWords(RandomIndex))
        Next

        'Getting the contralateral noise from the first SMC
        ContralateralNoise = PlannedTestWords.First.GetContralateralMaskerSound(CustomizableTestOptions.SelectedMediaSet, 0)

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

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then
            PrepareNextTrial(ProtocolReply)
        End If

        'Chacking if first test is finished
        If ProtocolReply.Decision = SpeechTestReplies.TestIsCompleted Then

            If IsSecondTest = False Then

                'It's the first test, initializing the second test
                InitializeSecondTest()

                'And informing the participant
                ProtocolReply.Decision = SpeechTestReplies.PauseTestingWithCustomInformation
                PauseInformation = "Första delen är klar av testet är klart." & vbCrLf & " Klicka OK för att starta den andra och sista delen."

            End If
        End If

        Return ProtocolReply.Decision

    End Function

    Private Sub PrepareNextTrial(ByVal NextTaskInstruction As TestProtocol.NextTaskInstruction)

        'Preparing the next trial
        'Getting next test word
        Dim NextTestWord = PlannedTestWords(ObservedTrials.Count)

        'Creating a new test trial
        CurrentTestTrial = New SrtTrial With {.SpeechMaterialComponent = NextTestWord,
                        .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
                        .SpeechLevel = NextTaskInstruction.AdaptiveValue,
                        .MaskerLevel = Double.NegativeInfinity,
                        .TestStage = NextTaskInstruction.TestStage,
                        .Tasks = 1}

        CurrentTestTrial.ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))

        Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)

        If CurrentTestTrial.SpeechMaterialComponent.ChildComponents.Count > 0 Then
            CurrentTestTrial.Tasks = 0
            For Each Child In CurrentTestTrial.SpeechMaterialComponent.ChildComponents()
                ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = Child.GetCategoricalVariableValue("Spelling"), .IsScoredItem = True})
                CurrentTestTrial.Tasks += 1
            Next
        End If

        CurrentTestTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

        'Mixing trial sound
        MixNextTrialSound()

        'Setting trial events
        CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * TestWordPresentationTime), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * (TestWordPresentationTime + MaximumResponseTime)), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

    End Sub



    Private Sub MixNextTrialSound()

        'Getting the speech signal
        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(CustomizableTestOptions.SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)
        Dim NominalLevel_FS = TestWordSound.SMA.NominalLevel

        'Creating a silent sound (lazy method to get the same length independently of contralateral masking or not)
        Dim SilentSound = Audio.GenerateSound.CreateSilence(ContralateralNoise.WaveFormat, 1, MaximumSoundDuration)

        'Creating contalateral masking noise (with the same length as the masking noise)
        Dim TrialContralateralNoise As Audio.Sound = Nothing
        Dim IntendedNoiseLength As Integer
        If CustomizableTestOptions.UseContralateralMasking = True Then
            Dim TotalSoundLength = ContralateralNoise.WaveData.SampleData(1).Length
            IntendedNoiseLength = ContralateralNoise.WaveFormat.SampleRate * MaximumSoundDuration
            Dim RandomStartReadSample = Randomizer.Next(0, TotalSoundLength - IntendedNoiseLength)
            TrialContralateralNoise = ContralateralNoise.CopySection(1, RandomStartReadSample - 1, IntendedNoiseLength) ' TODO: Here we should check to ensure that the MaskerNoise is long enough
        End If

        'Checking that Nominal levels agree between signal masker and contralateral masker
        If CustomizableTestOptions.UseContralateralMasking = True Then If ContralateralNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and contralateral noise files!")

        'Calculating presentation levels
        Dim TargetSpeechLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, SrtTrial).SpeechLevel)
        Dim NeededSpeechGain = TargetSpeechLevel_FS - NominalLevel_FS

        'Adjusts the sound levels
        Audio.DSP.AmplifySection(TestWordSound, NeededSpeechGain)

        If CustomizableTestOptions.UseContralateralMasking = True Then

            'Setting level, 
            'Very important: The contralateral masking sound file cannot be the same as the ipsilateral masker sound. The level of the contralateral masker sound must be set to agree with the Nominal level (while the ipsilateral masker sound sound have a level that deviates from the nominal level to attain the desired SNR!)
            Dim ContralateralMaskingNominalLevel_FS = ContralateralNoise.SMA.NominalLevel
            Dim TargetContralateralMaskingLevel_FS As Double = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, SrtTrial).ContralateralMaskerLevel)

            'Calculating the needed gain, also adding the EffectiveContralateralMaskingGain specified in the SelectedMediaSet
            Dim NeededContraLateralMaskerGain = TargetContralateralMaskingLevel_FS - ContralateralMaskingNominalLevel_FS + CustomizableTestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain
            Audio.DSP.AmplifySection(TrialContralateralNoise, NeededContraLateralMaskerGain)

        End If

        'Mixing speech and noise
        Dim TestWordInsertionSample As Integer = TestWordSound.WaveFormat.SampleRate * TestWordPresentationTime
        Dim Silence = Audio.GenerateSound.CreateSilence(SilentSound.WaveFormat, 1, TestWordInsertionSample, Audio.BasicAudioEnums.TimeUnits.samples)
        Audio.DSP.InsertSoundAt(TestWordSound, Silence, 0)
        TestWordSound.ZeroPad(IntendedNoiseLength)
        Dim TestSound = Audio.DSP.SuperpositionSounds({TestWordSound, SilentSound}.ToList)

        'Creating an output sound
        CurrentTestTrial.Sound = New Audio.Sound(New Audio.Formats.WaveFormat(TestWordSound.WaveFormat.SampleRate, TestWordSound.WaveFormat.BitDepth, 2,, TestWordSound.WaveFormat.Encoding))

        If CustomizableTestOptions.SignalLocations(0).HorizontalAzimuth < 0 Then
            'Left test ear
            'Adding speech and noise
            CurrentTestTrial.Sound.WaveData.SampleData(1) = TestSound.WaveData.SampleData(1)
            'Adding contralateral masking
            If CustomizableTestOptions.UseContralateralMasking = True Then
                CurrentTestTrial.Sound.WaveData.SampleData(2) = TrialContralateralNoise.WaveData.SampleData(1)
            End If

        Else
            'Right test ear
            'Adding speech and noise
            CurrentTestTrial.Sound.WaveData.SampleData(2) = TestSound.WaveData.SampleData(1)
            'Adding contralateral masking
            If CustomizableTestOptions.UseContralateralMasking = True Then
                CurrentTestTrial.Sound.WaveData.SampleData(1) = TrialContralateralNoise.WaveData.SampleData(1)
            End If
        End If

    End Sub


    Public Overrides Function GetResults() As TestResults

        Dim ProtocolThreshold = CustomizableTestOptions.SelectedTestProtocol.GetFinalResult()

        Dim Output = New TestResults(TestResults.TestResultTypes.SRT)
        If ProtocolThreshold.HasValue Then
            Output.AdaptiveLevelThreshold = ProtocolThreshold
        Else
            'Storing NaN if no threshold was reached
            Output.AdaptiveLevelThreshold = Double.NaN
        End If

        'Calculating the SRT based on the adaptive threshold
        Select Case CustomizableTestOptions.SelectedTestMode
            Case TestModes.AdaptiveSpeech
                Output.SpeechRecognitionThreshold = Output.AdaptiveLevelThreshold
            Case TestModes.AdaptiveNoise
                Output.SpeechRecognitionThreshold = Output.AdaptiveLevelThreshold
            Case Else
                Throw New NotImplementedException
        End Select

        'Storing the AdaptiveLevelSeries
        'Output.AdaptiveLevelSeries = New List(Of Double)
        Output.SpeechLevelSeries = New List(Of Double)
        'Output.MaskerLevelSeries = New List(Of Double)
        Output.ContralateralMaskerLevelSeries = New List(Of Double)
        'Output.SNRLevelSeries = New List(Of Double)
        Output.TestStageSeries = New List(Of String)
        Output.ProportionCorrectSeries = New List(Of String)
        Output.ScoreSeries = New List(Of String)
        For Each Trial As SrtTrial In ObservedTrials
            'Output.AdaptiveLevelSeries.Add(Math.Round(Trial.AdaptiveValue))
            Output.SpeechLevelSeries.Add(Math.Round(Trial.SpeechLevel))
            'Output.MaskerLevelSeries.Add(Math.Round(Trial.MaskerLevel))
            Output.ContralateralMaskerLevelSeries.Add(Math.Round(Trial.ContralateralMaskerLevel))
            'Output.SNRLevelSeries.Add(Math.Round(Trial.SNR))
            Output.TestStageSeries.Add(Trial.TestStage)
            Output.ProportionCorrectSeries.Add(Trial.GetProportionTasksCorrect)
            If Trial.IsCorrect = True Then
                Output.ScoreSeries.Add("Correct")
            Else
                Output.ScoreSeries.Add("Incorrect")
            End If
        Next

        Return Output

    End Function

    Public Overrides Sub FinalizeTest()

        CustomizableTestOptions.SelectedTestProtocol.FinalizeProtocol(ObservedTrials)

        If CurrentParticipantID <> NoTestId Then

            'Saving updated cache data values, only if a real test was completed
            Dim AllTestListsNames = AvailableTestListsNames()

            Dim NextTestListIndex As Integer = StoredTestListIndex

            NextTestListIndex += 1

            'Unwrapping NextTestListIndex 
            If NextTestListIndex > AllTestListsNames.Count - 1 Then
                NextTestListIndex = 0
            End If

            'Storing the test list index to be used in the next test session (only if NoTestId was not used)
            Utils.AppCache.SetAppCacheVariableValue(CacheLastTestListVariableName, NextTestListIndex)

        End If

    End Sub

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)
        Throw New NotImplementedException
    End Function

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        Throw New NotImplementedException()
    End Sub


End Class
