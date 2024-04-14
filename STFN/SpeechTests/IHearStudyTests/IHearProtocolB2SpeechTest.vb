Imports System.IO
Imports MathNet.Numerics
Imports STFN.TestProtocol
Imports STFN.Utils

Public Class IHearProtocolB2SpeechTest
    Inherits SpeechTest

    Public Sub New(SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

    End Sub

    Public Overrides ReadOnly Property FilePathRepresentation As String
        Get
            Return "ProtocolB2"
        End Get
    End Property


#Region "Settings"
    Public Overrides ReadOnly Property HasOptionalPractiseTest As Boolean
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

    Public Overrides ReadOnly Property UseSoundFieldSimulation As Utils.TriState
        Get
            Return TriState.False
        End Get
    End Property



    Public Overrides ReadOnly Property AvailableTestModes As List(Of TestModes)
        Get
            Return New List(Of TestModes) From {TestModes.ConstantStimuli}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestProtocols As List(Of TestProtocol)
        Get
            Return New List(Of TestProtocol) From {New FixedLengthWordsInNoise_WithPreTestLevelAdjustment_TestProtocol}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableFixedResponseAlternativeCounts As List(Of Integer)
        Get
            Return New List(Of Integer)
        End Get
    End Property

    Public Overrides ReadOnly Property AvailablePhaseAudiometryTypes As List(Of BmldModes)
        Get
            Return New List(Of BmldModes)
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
            Return 1
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



    Dim LevelAdjustmentStageListIndex As Integer
    Dim TestStageListIndex As Integer
    Dim SelectedMediaSetIndex As Integer

    Dim CacheLastAdjustmentStageListVariableName As String
    Dim CacheLastTestListVariableName As String
    Dim CacheLastMediaSetVariableName As String

    Private PlannedLevelAdjustmentWords As List(Of SpeechMaterialComponent) = Nothing
    Private PlannedTestListWords As List(Of SpeechMaterialComponent) = Nothing

    Private ObservedPreTestTrials As New TrialHistory
    Private ObservedTestTrials As New TrialHistory

    Private MaskerNoise As Audio.Sound = Nothing
    Private ContralateralNoise As Audio.Sound = Nothing

    Private TestWordPresentationTime As Double = 0.5

    Private TestLength As Integer = 50

    Private MaximumSoundDuration As Double = 10


    Public Overrides Function InitializeCurrentTest() As Boolean

        Dim AllTestListsNames = AvailableTestListsNames()
        Dim AllMediaSets = AvailableMediasets

        If AllTestListsNames.Count = 0 Then
            MsgBox("No test lists exist in the currently selected speech material!", MsgBoxStyle.Exclamation, "Missing speech material!")
            Return False
        End If

        'Creating cache variable names for storing last test list index and voice between sessions
        CacheLastAdjustmentStageListVariableName = FilePathRepresentation & "LastASList"
        CacheLastTestListVariableName = FilePathRepresentation & "LastTestList"
        CacheLastMediaSetVariableName = FilePathRepresentation & "LastMediaSet"

        ' Getting the last used voice
        If AppCache.AppCacheVariableExists(CacheLastMediaSetVariableName) = False Then
            'Randomizing a new list number if no list has been run previously 
            SelectedMediaSetIndex = Randomizer.Next(0, AllMediaSets.Count)
        Else
            'Getting the last tested media set
            SelectedMediaSetIndex = AppCache.GetAppCacheIntegerVariableValue(CacheLastMediaSetVariableName)
        End If

        'Increasing the media set index and unwrapping it to the number of available media sets.
        SelectedMediaSetIndex += 1
        If SelectedMediaSetIndex > AllMediaSets.Count - 1 Then
            SelectedMediaSetIndex = 0
        End If

        'Selecting test list
        If AppCache.AppCacheVariableExists(CacheLastAdjustmentStageListVariableName) And AppCache.AppCacheVariableExists(CacheLastTestListVariableName) = True Then

            'Getting the last tested index
            LevelAdjustmentStageListIndex = AppCache.GetAppCacheIntegerVariableValue(CacheLastAdjustmentStageListVariableName)
            TestStageListIndex = AppCache.GetAppCacheIntegerVariableValue(CacheLastTestListVariableName)

        Else
            'Randomizing a new list number if no list has been run previously 
            LevelAdjustmentStageListIndex = Randomizer.Next(0, AllTestListsNames.Count)

            'Storing the TestStageListIndex as the next list
            TestStageListIndex = LevelAdjustmentStageListIndex + 1

            'Unwrapping TestStageListIndex
            If TestStageListIndex > AllTestListsNames.Count - 1 Then
                TestStageListIndex = 0
            End If

        End If

        CustomizableTestOptions.SelectedMediaSet = AvailableMediasets(SelectedMediaSetIndex)

        CreatePlannedWordsList()

        'Getting the masker noise only once (this should be a long section of noise with its using nominal level set
        If MaskerNoise Is Nothing Then MaskerNoise = PlannedTestListWords(0).GetMaskerSound(CustomizableTestOptions.SelectedMediaSet, 0)
        If CustomizableTestOptions.UseContralateralMasking = True Then
            'TODO: Here we should use speech weighted standard noise??? Or the speech material noise??
            ContralateralNoise = PlannedTestListWords(0).GetContralateralMaskerSound(CustomizableTestOptions.SelectedMediaSet, 0)
        End If

        CustomizableTestOptions.SelectedTestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.AdaptiveValue = CustomizableTestOptions.SpeechLevel, .TestStage = 0, .TestLength = TestLength})

        Return True

    End Function

    Private Function CreatePlannedWordsList() As Boolean

        Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, True, False)
        Dim AllTestListsNames = AvailableTestListsNames()

        Dim LevelAdjustmentListName = AllTestListsNames(LevelAdjustmentStageListIndex)
        Dim TestListName = AllTestListsNames(TestStageListIndex)

        For Each List In AllLists
            If List.PrimaryStringRepresentation = LevelAdjustmentListName Then PlannedLevelAdjustmentWords = List.ChildComponents
            If List.PrimaryStringRepresentation = TestListName Then PlannedTestListWords = List.ChildComponents
        Next

        'Checking that the lists are not empty
        If PlannedLevelAdjustmentWords Is Nothing Or PlannedTestListWords Is Nothing Then
            Messager.MsgBox("Unable to find the test word lists!", MsgBoxStyle.Exclamation, "An error occurred!")
            Return False
        End If

        If PlannedLevelAdjustmentWords.Count = 0 Or PlannedTestListWords.Count = 0 Then
            Messager.MsgBox("Unable to find the test words!", MsgBoxStyle.Exclamation, "An error occurred!")
            Return False
        End If

        Return True

    End Function

    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        Dim ProtocolReply As NextTaskInstruction = Nothing

        If e IsNot Nothing Then

            If CustomizableTestOptions.SelectedTestProtocol.IsInPretestMode Then

                'This is an incoming pretest, level adjustment trial trial response

                'Translating the response (Always using the first response, as there should never be more than one)
                LevelAdjustmentTrial.GetRatingStrings()
                Dim Response As String = ""
                For Each ResponseString In e.LinguisticResponses
                    If ResponseString <> "" Then
                        Response = ResponseString
                        Exit For
                    End If
                Next
                If DirectCast(CurrentTestTrial, LevelAdjustmentTrial).TranslateResponse(Response) = False Then
                    Throw New Exception("Unable to interpret sound level rating response!")
                End If

                'Adding the test trial
                ObservedPreTestTrials.Add(CurrentTestTrial)

                'Calculating the speech level
                ProtocolReply = CustomizableTestOptions.SelectedTestProtocol.NewResponse(ObservedPreTestTrials)

            Else

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
                ObservedTestTrials.Add(CurrentTestTrial)

                'TODO: We must store the responses and response times!!!

                'Calculating the speech level
                ProtocolReply = CustomizableTestOptions.SelectedTestProtocol.NewResponse(ObservedTestTrials)

            End If

        Else
            'Nothing to correct (this should be the start of a new test)
            ProtocolReply = CustomizableTestOptions.SelectedTestProtocol.NewResponse(New TrialHistory)
        End If

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then
            PrepareNextTrial(ProtocolReply)
        End If

        Return ProtocolReply.Decision

    End Function

    Dim LastSpeechLevel As Double? = Nothing

    Private Sub PrepareNextTrial(ByVal NextTaskInstruction As TestProtocol.NextTaskInstruction)

        Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)

        If LastSpeechLevel.HasValue Then
            Dim SpeechLevelChangeSinseLastTrial As Double = NextTaskInstruction.AdaptiveValue - LastSpeechLevel

            'Adjusting the ContralateralMaskingLevel accordingly. TODO: We must check that the level doesn't get too loud!
            CustomizableTestOptions.ContralateralMaskingLevel += SpeechLevelChangeSinseLastTrial

        End If


        If CustomizableTestOptions.SelectedTestProtocol.IsInPretestMode Then

            'Preparing the next pretest trial
            Dim NextTestWord = PlannedLevelAdjustmentWords(ObservedPreTestTrials.Count)

            'Creating a new pretest trial
            CurrentTestTrial = New LevelAdjustmentTrial With {.SpeechMaterialComponent = NextTestWord,
                .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
                .SpeechLevel = NextTaskInstruction.AdaptiveValue,
                .ContralateralMaskerLevel = CustomizableTestOptions.ContralateralMaskingLevel,
                .TestStage = NextTaskInstruction.TestStage,
                .Tasks = 1,
                .ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))}

            Dim RatingStrings = LevelAdjustmentTrial.GetRatingStrings
            For Each RatingString In RatingStrings
                ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = RatingString, .IsScoredItem = True})
            Next
            CurrentTestTrial.Tasks = 1

            'Storing LastSpeechLevel
            LastSpeechLevel = DirectCast(CurrentTestTrial, LevelAdjustmentTrial).SpeechLevel

            'Setting trial events
            CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 501, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
            'CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 5500, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

        Else

            'Preparing the next trial
            'Getting next test word
            Dim NextTestWord = PlannedTestListWords(ObservedTestTrials.Count)

            'Creating a new test trial
            CurrentTestTrial = New WrsTrial With {.SpeechMaterialComponent = NextTestWord,
                .AdaptiveValue = NextTaskInstruction.AdaptiveValue,
                .SpeechLevel = NextTaskInstruction.AdaptiveValue,
                .ContralateralMaskerLevel = CustomizableTestOptions.ContralateralMaskingLevel,
                .TestStage = NextTaskInstruction.TestStage,
                .Tasks = 1,
                .ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))}

            If CurrentTestTrial.SpeechMaterialComponent.ChildComponents.Count > 0 Then
                For Each Child In CurrentTestTrial.SpeechMaterialComponent.ChildComponents()
                    If CustomizableTestOptions.ScoreOnlyKeyWords = True Then
                        ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = Child.GetCategoricalVariableValue("Spelling"), .IsScoredItem = Child.IsKeyComponent})
                    Else
                        ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = Child.GetCategoricalVariableValue("Spelling"), .IsScoredItem = True})
                    End If
                Next
            End If

            'Storing LastSpeechLevel
            LastSpeechLevel = DirectCast(CurrentTestTrial, WrsTrial).SpeechLevel

            'Setting trial events
            CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 501, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 5500, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})

        End If

        CurrentTestTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

        'Mixing trial sound
        MixNextTrialSound()

    End Sub

    Private Sub MixNextTrialSound()

        'Getting the speech signal
        Dim TestWordSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(CustomizableTestOptions.SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)
        Dim NominalLevel_FS = TestWordSound.SMA.NominalLevel

        'Getting a random section of the noise
        Dim TotalNoiseLength As Integer = MaskerNoise.WaveData.SampleData(1).Length
        Dim IntendedNoiseLength As Integer = MaskerNoise.WaveFormat.SampleRate * MaximumSoundDuration
        Dim RandomStartReadSample As Integer = Randomizer.Next(0, TotalNoiseLength - IntendedNoiseLength)
        Dim TrialNoise = MaskerNoise.CopySection(1, RandomStartReadSample - 1, IntendedNoiseLength) ' TODO: Here we should check to ensure that the MaskerNoise is long enough

        'Creating contalateral masking noise (with the same length as the masking noise)
        Dim TrialContralateralNoise As Audio.Sound = Nothing
        If CustomizableTestOptions.UseContralateralMasking = True Then
            TotalNoiseLength = ContralateralNoise.WaveData.SampleData(1).Length
            IntendedNoiseLength = ContralateralNoise.WaveFormat.SampleRate * MaximumSoundDuration
            RandomStartReadSample = Randomizer.Next(0, TotalNoiseLength - IntendedNoiseLength)
            TrialContralateralNoise = ContralateralNoise.CopySection(1, RandomStartReadSample - 1, IntendedNoiseLength) ' TODO: Here we should check to ensure that the MaskerNoise is long enough
        End If

        'Checking that Nominal levels agree between signal masker and contralateral masker
        If MaskerNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and noise files!")
        If CustomizableTestOptions.UseContralateralMasking = True Then If ContralateralNoise.SMA.NominalLevel <> NominalLevel_FS Then Throw New Exception("Nominal level is required to be the same between speech and contralateral noise files!")

        'Calculating presentation levels
        Dim TargetSpeechLevel_FS As Double
        If CustomizableTestOptions.SelectedTestProtocol.IsInPretestMode = True Then
            TargetSpeechLevel_FS = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, LevelAdjustmentTrial).SpeechLevel)
        Else
            TargetSpeechLevel_FS = Audio.Standard_dBSPL_To_dBFS(DirectCast(CurrentTestTrial, WrsTrial).SpeechLevel)
        End If
        Dim NeededSpeechGain = TargetSpeechLevel_FS - NominalLevel_FS

        'Adjusts the sound levels
        Audio.DSP.AmplifySection(TestWordSound, NeededSpeechGain)
        'Applying the same gain to the masker. Very important: This requires that the masker is preadjusted to create the intended SNR together with the speech recordings, and have the same nominal level! (I.e. If speech and sound files would be mixed without any adjustment, they would get their desired SNR.)
        Audio.DSP.AmplifySection(TrialNoise, NeededSpeechGain)

        If CustomizableTestOptions.UseContralateralMasking = True Then

            'Setting level, 
            'Very important: The contralateral masking sound file cannot be the same as the ipsilateral masker sound. The level of the contralateral masker sound must be set to agree with the Nominal level (while the ipsilateral masker sound sound have a level that deviates from the nominal level to attain the desired SNR!)
            Dim ContralateralMaskingNominalLevel_FS = ContralateralNoise.SMA.NominalLevel
            Dim TargetContralateralMaskingLevel_FS = Audio.Standard_dBSPL_To_dBFS(CustomizableTestOptions.ContralateralMaskingLevel)

            'Calculating the needed gain, also adding the EffectiveContralateralMaskingGain specified in the SelectedMediaSet
            Dim NeededContraLateralMaskerGain = TargetContralateralMaskingLevel_FS - ContralateralMaskingNominalLevel_FS + CustomizableTestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain
            Audio.DSP.AmplifySection(TrialContralateralNoise, NeededContraLateralMaskerGain)

        End If

        'Mixing speech and noise
        Dim TestWordInsertionSample As Integer = TestWordSound.WaveFormat.SampleRate * TestWordPresentationTime
        Dim Silence = Audio.GenerateSound.CreateSilence(TrialNoise.WaveFormat, 1, TestWordInsertionSample, Audio.BasicAudioEnums.TimeUnits.samples)
        Audio.DSP.InsertSoundAt(TestWordSound, Silence, 0)
        TestWordSound.ZeroPad(IntendedNoiseLength)
        Dim TestSound = Audio.DSP.SuperpositionSounds({TestWordSound, TrialNoise}.ToList)

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

        Dim Output = New TestResults(TestResults.TestResultTypes.WRS)

        Dim ProtocolFInalResult = CustomizableTestOptions.SelectedTestProtocol.GetFinalResult()

        If ProtocolFInalResult.HasValue Then
            Output.ProportionCorrect = ProtocolFInalResult
        Else
            'Storing NaN if no final result exists
            Output.ProportionCorrect = Double.NaN
        End If

        'Storing the AdaptiveLevelSeries
        Output.AdaptiveLevelSeries = New List(Of Double)
        Output.SpeechLevelSeries = New List(Of Double)
        Output.MaskerLevelSeries = New List(Of Double)
        Output.ContralateralMaskerLevelSeries = New List(Of Double)
        Output.SNRLevelSeries = New List(Of Double)
        Output.TestStageSeries = New List(Of String)
        Output.ProportionCorrectSeries = New List(Of String)
        Output.ScoreSeries = New List(Of String)

        If CustomizableTestOptions.SelectedTestProtocol.IsInPretestMode Then
            For Each Trial As LevelAdjustmentTrial In ObservedPreTestTrials
                Output.AdaptiveLevelSeries.Add(System.Math.Round(Trial.AdaptiveValue))
                Output.SpeechLevelSeries.Add(System.Math.Round(Trial.SpeechLevel))
                Output.MaskerLevelSeries.Add(System.Math.Round(Trial.MaskerLevel))
                Output.ContralateralMaskerLevelSeries.Add(System.Math.Round(Trial.ContralateralMaskerLevel))
                Output.SNRLevelSeries.Add(System.Math.Round(Trial.SNR))
                Output.TestStageSeries.Add(Trial.TestStage)
                Output.ProportionCorrectSeries.Add(Trial.GetProportionTasksCorrect)
                If Trial.IsCorrect = True Then
                    Output.ScoreSeries.Add("Correct")
                Else
                    Output.ScoreSeries.Add("Incorrect")
                End If
            Next
        Else
            For Each Trial As WrsTrial In ObservedTestTrials
                Output.AdaptiveLevelSeries.Add(System.Math.Round(Trial.AdaptiveValue))
                Output.SpeechLevelSeries.Add(System.Math.Round(Trial.SpeechLevel))
                Output.MaskerLevelSeries.Add(System.Math.Round(Trial.MaskerLevel))
                Output.ContralateralMaskerLevelSeries.Add(System.Math.Round(Trial.ContralateralMaskerLevel))
                Output.SNRLevelSeries.Add(System.Math.Round(Trial.SNR))
                Output.TestStageSeries.Add(Trial.TestStage)
                Output.ProportionCorrectSeries.Add(Trial.GetProportionTasksCorrect)
                If Trial.IsCorrect = True Then
                    Output.ScoreSeries.Add("Correct")
                Else
                    Output.ScoreSeries.Add("Incorrect")
                End If
            Next
        End If

        Return Output

    End Function


    Public Overrides Sub FinalizeTest()

        CustomizableTestOptions.SelectedTestProtocol.FinalizeProtocol(ObservedTestTrials)

        If CurrentParticipantID <> NoTestId Then

            'Saving updated cache data values, only if a real test was completed
            Dim AllTestListsNames = AvailableTestListsNames()

            Dim NextTestListIndex As Integer = TestStageListIndex
            Dim NextAdjustmentStageListIndex As Integer = LevelAdjustmentStageListIndex

            If SelectedMediaSetIndex >= AvailableMediasets.Count - 1 Then
                'Increasing list index for the next test session, after the last media set has been tested (each list is run once with each media set before next list is started)
                NextTestListIndex += 1
                NextAdjustmentStageListIndex += 1

                'Unwrapping these
                If NextTestListIndex > AllTestListsNames.Count - 1 Then
                    NextTestListIndex = 0
                End If

                If NextAdjustmentStageListIndex > AllTestListsNames.Count - 1 Then
                    NextAdjustmentStageListIndex = 0
                End If

                'Storing the test list index and media set to be used in the next test session (only if NoTestId was not used)
                AppCache.SetAppCacheVariableValue(CacheLastAdjustmentStageListVariableName, NextAdjustmentStageListIndex)
                AppCache.SetAppCacheVariableValue(CacheLastTestListVariableName, NextTestListIndex)
                AppCache.SetAppCacheVariableValue(CacheLastMediaSetVariableName, SelectedMediaSetIndex)

            End If

        End If

    End Sub


End Class