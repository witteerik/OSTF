
Imports STFN.Core.Utils
Imports STFN.Core.Audio
Imports MathNet.Numerics.Distributions

Public Class QuickSiP_StaticSpeechTest
    Inherits SpeechAudiometryTest

    Public Sub New(SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub

    Public Shadows Sub ApplyTestSpecificSettings()

        TesterInstructions = "--  Quick SiP  --" & vbCrLf & vbCrLf &
             "För detta test behövs inga inställningar." & vbCrLf & vbCrLf &
             "1. Informera deltagaren om hur testet går till." & vbCrLf &
             "2. Vänd skärmen till deltagaren. Be sedan deltagaren klicka på start för att starta testet."

        ParticipantInstructions = "--  Quick SiP  --" & vbCrLf & vbCrLf &
             "Lyssna efter enstaviga ord som uttalas i en stadsmiljö." & vbCrLf &
             "Välj från svarsalternativen på skärmen det ord du uppfattade. " & vbCrLf &
             "Gissa om du är osäker." & vbCrLf &
             "Efter varje ord har du maximalt 4 sekunder på dig att ange ditt svar." & vbCrLf &
             "Om svarsalternativen blinkar i röd färg har du inte svarat i tid." & vbCrLf &
             "Testet består av totalt 30 ord, som blir svårare och svårare ju längre testet går." & vbCrLf &
             "Starta testet genom att klicka på knappen 'Start'"

        ParticipantInstructionsButtonText = "Deltagarinstruktion"

        ShowGuiChoice_TargetLocations = False
        ShowGuiChoice_PreSet = False
        ShowGuiChoice_StartList = True
        ShowGuiChoice_MediaSet = False
        SupportsPrelistening = False
        AvailableTestModes = New List(Of TestModes) From {TestModes.ConstantStimuli}
        'AvailableTestProtocols = New List(Of TestProtocol) From {New FixedLengthWordsInNoise_WithPreTestLevelAdjustment_TestProtocol}
        AvailableFixedResponseAlternativeCounts = New List(Of Integer) From {3}

        MaximumSoundFieldSpeechLocations = 1
        MaximumSoundFieldMaskerLocations = 0

        MinimumSoundFieldSpeechLocations = 1
        MinimumSoundFieldMaskerLocations = 0

        'ShowGuiChoice_WithinListRandomization = True
        WithinListRandomization = False

        'ShowGuiChoice_FreeRecall = True
        IsFreeRecall = False

        HistoricTrialCount = 0
        SupportsManualPausing = False

        SoundOverlapDuration = 0.5

        GuiResultType = GuiResultTypes.VisualResults

    End Sub

    Private PlannedTestTrials As New TestTrialCollection
    Private ResponseAlternativePresentationTime As Double = 2
    Private MaximumResponseTime As Double = 6.5
    Private TestLength As Integer = 30

    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

        If IsInitialized = True Then Return New Tuple(Of Boolean, String)(True, "")

        'Copying the target location to the masker location
        SelectSameMaskersAsTargetSoundSources()

        'Assigning the first media set
        MediaSet = AvailableMediasets(0)

        CreatePlannedWordsList()

        'TestProtocol.InitializeProtocol(New TestProtocol.NextTaskInstruction With {.TestLength = TestLength})

        IsInitialized = True

        Return New Tuple(Of Boolean, String)(True, "")

    End Function

    Private Function CreatePlannedWordsList() As Boolean

        Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List, True, False)

        'Determines the index of the start list
        Dim SelectedStartListIndex As Integer = -1
        For i = 0 To AllLists.Count - 1
            If AllLists(i).PrimaryStringRepresentation = StartList Then
                SelectedStartListIndex = i
                Exit For
            End If
        Next

        If SelectedStartListIndex = -1 Then
            Messager.MsgBox("Unable to find the selected start list",, "An error occurred!")
            Return False
        End If

        Dim PlannedTestListWords As List(Of SpeechMaterialComponent) = AllLists(SelectedStartListIndex).GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)

        If PlannedTestListWords.Count = 0 Then
            Messager.MsgBox("Unable to find the test words!", MsgBoxStyle.Exclamation, "An error occurred!")
            Return False
        End If

        PlannedTestTrials = New TestTrialCollection

        For i = 0 To PlannedTestListWords.Count - 1

            Dim CurrentSMC = PlannedTestListWords(i)

            Dim NewTestTrial = New TestTrial With {.SpeechMaterialComponent = CurrentSMC,
                .Tasks = 1,
                .ResponseAlternativeSpellings = New List(Of List(Of SpeechTestResponseAlternative))}

            Dim ResponseAlternatives As New List(Of SpeechTestResponseAlternative)

            Dim ResponsAlternativeInputString = NewTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Alternatives")
            Dim ResponsAlternativeSplit = ResponsAlternativeInputString.Split(",")
            For Each Item In ResponsAlternativeSplit
                If Item.Trim <> "" Then
                    ResponseAlternatives.Add(New SpeechTestResponseAlternative With {.Spelling = Item.Trim})
                End If
            Next

            'Shuffling the order of response alternatives
            ResponseAlternatives = Utils.Shuffle(ResponseAlternatives, Randomizer).ToList

            NewTestTrial.ResponseAlternativeSpellings.Add(ResponseAlternatives)

            PlannedTestTrials.Add(NewTestTrial)

        Next

        'Setting TestLength to the number of available words
        TestLength = PlannedTestTrials.Count

        Return True

    End Function


    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        If e IsNot Nothing Then

            'This is an incoming test trial response

            'Corrects the trial response, based on the given response

            'Resets the CurrentTestTrial.ScoreList
            Select Case e.LinguisticResponses(0)
                Case CurrentTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling")
                    CurrentTestTrial.IsCorrect = True

                Case ""
                    'Randomizing IsCorrect with a 1/3 chance for True
                    Dim ChanceList As New List(Of Boolean) From {True, False, False}
                    Dim RandomIndex As Integer = Randomizer.Next(ChanceList.Count)
                    CurrentTestTrial.IsCorrect = ChanceList(RandomIndex)

                Case Else
                    CurrentTestTrial.IsCorrect = False
            End Select

            CurrentTestTrial.Response = e.LinguisticResponses(0)

            'Adding the test trial
            ObservedTrials.Add(CurrentTestTrial)

            'Taking a dump of the SpeechTest
            CurrentTestTrial.SpeechTestPropertyDump = Utils.Logging.ListObjectPropertyValues(Me.GetType, Me)


        Else
            'Nothing to correct (this should be the start of a new test)

        End If

        Dim ProtocolReply = New TestProtocol.NextTaskInstruction With {.Decision = SpeechTestReplies.GotoNextTrial}

        If ObservedTrials.Count = TestLength Then
            'Test is completed
            Return SpeechTestReplies.TestIsCompleted
        End If

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then

            'Preparing the next trial
            'Getting next test word
            CurrentTestTrial = PlannedTestTrials(ObservedTrials.Count)

            CurrentTestTrial.TrialEventList = New List(Of ResponseViewEvent)
            CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})

            If ObservedTrials.Count = 0 Then

                'Mixing in the initial sound in the test sound to get a longer start
                Dim AddedTime As Double = 4

                'Simply high-jacking the LombardNoisePath to load the initial sound, taking the path from the first planned trial
                Dim CurrentTestRootPath As String = PlannedTestTrials(0).SpeechMaterialComponent.ParentTestSpecification.GetTestRootPath
                Dim InitialSoundPath = IO.Path.Combine(CurrentTestRootPath, MediaSet.LombardNoisePath)
                Dim InitialSound = Sound.LoadWaveFile(InitialSoundPath)
                Audio.DSP.CropSection(InitialSound, 0, InitialSound.WaveFormat.SampleRate * AddedTime + 0.5) ' Adding 0.5 seconds to compensate for the halv second crossfading below

                Dim TrialSoundFilePath = CurrentTestTrial.SpeechMaterialComponent.GetSoundPath(MediaSet, 0)
                Dim TrialSound = Sound.LoadWaveFile(TrialSoundFilePath)
                Dim FirstTrialSound = Audio.DSP.ConcatenateSounds({InitialSound, TrialSound}.ToList, ,,,,, InitialSound.WaveFormat.SampleRate / 2)

                CurrentTestTrial.Sound = FirstTrialSound

                CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * (AddedTime + ResponseAlternativePresentationTime)), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
                CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * (AddedTime + MaximumResponseTime)), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})
            Else
                'Loading the premixed sound right from the sound file without any modifcations
                Dim SoundFilePath = CurrentTestTrial.SpeechMaterialComponent.GetSoundPath(MediaSet, 0)
                CurrentTestTrial.Sound = Sound.LoadWaveFile(SoundFilePath)

                CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * ResponseAlternativePresentationTime), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
                CurrentTestTrial.TrialEventList.Add(New ResponseViewEvent With {.TickTime = System.Math.Max(1, 1000 * MaximumResponseTime), .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseTimesOut})
            End If


        End If

        Return ProtocolReply.Decision

    End Function

    Public Overrides Function GetSelectedExportVariables() As List(Of String)
        'Not used
        Return Nothing
    End Function

    Public Overrides Function GetAverageScore(Optional IncludeTrialsFromEnd As Integer? = Nothing) As Double?

        Dim ScoreList As New List(Of Integer)
        For Each ObservedTrial In ObservedTrials
            If ObservedTrial.IsCorrect = True Then
                ScoreList.Add(1)
            Else
                ScoreList.Add(0)
            End If
        Next

        If ScoreList.Count = 0 Then
            Return Nothing
        Else
            Return ScoreList.Average
        End If

    End Function

    Public Overrides Function GetResultStringForGui() As String
        'Not used
        Return ""
    End Function

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Sound, String)
        'Not used
        Return Nothing
    End Function

    Public Overrides Function GetProgress() As ProgressInfo

        If ObservedTrials.Count > 0 Then

            Dim NewProgressInfo As New ProgressInfo
            NewProgressInfo.Value = ObservedTrials.Count + 1 ' Adds one to signal started trials
            NewProgressInfo.Maximum = TestLength
            Return NewProgressInfo

        Else
            Return Nothing
        End If

    End Function

    Public Overrides Function GetTotalTrialCount() As Integer
        Return TestLength
    End Function

    Public Overrides Function GetSubGroupResults() As List(Of Tuple(Of String, Double))
        'Not used
        Return Nothing
    End Function

    Public Overrides Function GetScorePerLevel() As Tuple(Of String, SortedList(Of Double, Double))
        'Not used
        Return Nothing
    End Function


    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        'Not used
    End Sub

    Public Overrides Sub FinalizeTestAheadOfTime()
        'Not used
    End Sub


End Class