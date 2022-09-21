Imports System.IO

Partial Class SipTestGui

    Private CurrentTestSound As Audio.Sound = Nothing

    Private CurrentSipTrial As SipTest.SipTrial

    Private WithEvents PcParticipantForm As PcTesteeForm

    Private WithEvents ParticipantControl As ITesteeControl

    Private UseVisualQue As Boolean = True
    Private MaximumResponseTime As Double = 4

    Private _TestIsStarted As Boolean = False
    Private Property TestIsStarted As Boolean
        Get
            Return _TestIsStarted
        End Get
        Set(value As Boolean)
            _TestIsStarted = value
        End Set
    End Property

    Private IsPaused As Boolean = False

    ''' <summary>
    ''' The (shortest) time delay (in seconds) between the response and the start of a new trial.
    ''' </summary>
    Private InterTrialInterval As Double = 1 ' TestSetup.CurrentEnvironment.TestSoundMixerSettings.InterTrialInterval

    ''' <summary>
    ''' The time delay (in seconds) between the end of the test word and the visual presenation of the response alternatives.
    ''' </summary>
    Private ResponseAlternativeDelay As Double = 0.5

    Private PretestSoundDuration As Double = 5

    Private MinimumTestWordStartTime As Double = 1.5 '// If test word region background sound Is used, this needs to be longer than FadeInDuration_TestWordRegion, And longer than any of the fade-in region in specific test word masker sounds (If The environment variable UseListSpecificMaskers = True).
    Private MaximumTestWordStartTime As Double = 2


    ''' <summary>
    ''' Holds the time of the presentation of the response alternatives
    ''' </summary>
    Private ResponseAlternativesPresentationTime As DateTime

    Private TestWordAlternatives As List(Of String)

    Private ShowProgressIndication As Boolean = True

    ''' <summary>
    ''' Set to True to simulate test results.
    ''' </summary>
    Private Property SimulationMode As Boolean

    'Timers
    Private WithEvents StartTrialTimer As New Timers.Timer
    Private WithEvents ShowVisualQueTimer As New Timers.Timer
    Private WithEvents HideVisualQueTimer As New Timers.Timer
    Private WithEvents ShowResponseAlternativesTimer As New Timers.Timer
    Private WithEvents MaxResponseTimeTimer As New Timers.Timer

    'Others
    Dim CurrentTrialSoundIsReady As Boolean = False
    Dim CurrentTrialIsLaunched As Boolean = False ' A variable that holds a value indicating if the current trial was started by the StartTrialTimer, or if it should be started directly from prepare sound. (This construction is needed as the sound may not always be created before the next trial should start. If that happens the trial starts as soon as the sound is ready to be played.)
    Dim StartTrialTimerHasTicked As Boolean = False

    Delegate Sub NoArgDelegate()
    Delegate Sub StringArgReturningVoidDelegate([String] As String)


#Region "ActiveTesting"

    'Code structure:
    '
    'Start
    'Select Case word
    '-randomize test word start time
    'prepare sound
    'play sound
    'display visual que
    'display response alternatives
    'recieve response/wait For timeout
    'check if paused/store response/adjust result
    'launch next trial/finish up


    Public Sub StartedByTestee() Handles ParticipantControl.StartedByTestee
        Utils.SendInfoToLog("Test started by testee")
        TestIsStarted = True

        InitiateTest()

    End Sub

    Public Sub StartedByAdministrator()
        Utils.SendInfoToLog("Test started by administrator")
        TestIsStarted = True

        If SimulationMode = False Then
            InitiateTest()
        Else
            'Calling StartTimerTick directly
            StartTrialTimerTick()
        End If

    End Sub



    Private Sub InitiateTest()

        StartTrialTimer.Interval = Math.Max(1, InterTrialInterval * 1000)


        'Removes the start button
        ParticipantControl.ResetTestWordPanel()

        'Cretaing a context sound without any test stimulus, that runs for approx TestSetup.PretestSoundDuration seconds
        Dim TestSound As Audio.Sound = Nothing
        'TestSound = SoundLibrary.CreateSoundStimulus(Nothing, 0, 0,
        '                                             Nothing,
        '                                             Nothing,
        '                                             Nothing,
        '                                             ContextRegionForegroundLevel_SPL,
        '                                             ContextRegionBackgroundLevel_SPL,
        '                                             TestSetup.CurrentFixedMaskerSoundRandomization,
        '                                             False,
        '                                             TestSetup.SimulateHearingLoss,
        '                                             TestSetup.CompensateHearingLoss,
        '                                             TestSessionDescription.PatientDetails.ID & "_" & Me.TestSessionStage.ToString & "_" & Me.TestConditionName,
        '                                             "PreSound", Nothing, Nothing, TestSetup.PretestSoundDuration + MaximumResponseTime) 'Adding four seconds to PretestSoundDuration to allow for preparation of the first test trial 


        'Plays sound
        If SimulationMode = False Then SoundPlayer.SwapOutputSounds(TestSound)

        'Setting the interval to the first test stimulus using NewTrialTimer.Interval (N.B. The NewTrialTimer.Interval value has to be reset at the first tick, as the deafault value is overridden here)
        StartTrialTimer.Interval = Math.Max(1, PretestSoundDuration * 1000)


        'Preparing and launching the next trial
        PrepareAndLaunchTrial_ThreadSafe()

    End Sub


    Private Sub PrepareAndLaunchTrial_ThreadSafe()

        If Me.InvokeRequired = True Then
            Dim d As New NoArgDelegate(AddressOf PrepareAndLaunchTrial_Unsafe)
            Me.Invoke(d)
        Else
            PrepareAndLaunchTrial_Unsafe()
        End If

    End Sub


    Private Sub PrepareAndLaunchTrial_Unsafe()

        'Resetting NextTrialIsReady and CurrentTrialIsStarted  
        CurrentTrialSoundIsReady = False
        CurrentTrialIsLaunched = False
        StartTrialTimerHasTicked = False

        'Pausing test
        If IsPaused = True Then
            Exit Sub
        End If

        'Updates the GUI table
        UpdateTestTrialTable()
        UpdateTestProgress()


        'Starting the timer that will initiate the presentation of the trial, if the sound is is prepared in time.
        StartTrialTimer.Start()

        'Gets the next stimulus
        CurrentSipTrial = CurrentSipTestMeasurement.GetNextTrial()

        'Checks if test is finished
        If CurrentSipTrial Is Nothing Then
            FinalizeTesting()
            Exit Sub
        End If


        If CurrentSipTrial IsNot Nothing Then

            'Preparing alternatives
            TestWordAlternatives = New List(Of String)
            Dim TempList As New List(Of SpeechMaterialComponent)
            CurrentSipTrial.SpeechMaterialComponent.IsContrastingComponent(,, TempList)
            For Each ContrastingComponent In TempList
                TestWordAlternatives.Add(ContrastingComponent.GetCategoricalVariableValue("Spelling"))
            Next

            'Randomizing the order
            Dim AlternativesCount As Integer = TestWordAlternatives.Count
            Dim TempList2 As New List(Of String)
            For n = 0 To AlternativesCount - 1
                Dim RandomIndex As Integer = SipMeasurementRandomizer.Next(0, TestWordAlternatives.Count)
                TempList2.Add(TestWordAlternatives(RandomIndex))
                TestWordAlternatives.RemoveAt(RandomIndex)
            Next
            TestWordAlternatives = TempList2

            'Praparing the sound
            PrepareNewSound()

            'Setting NextTrialIsReady to True to mark that the trial is ready to run
            CurrentTrialSoundIsReady = True

        Else
            'Testing Is completed
            FinalizeTesting()
        End If

    End Sub


    Private Sub StartTrialTimerTick() Handles StartTrialTimer.Elapsed
        StartTrialTimer.Stop()

        'Restoring the value of StartTrialTimer.Interval, as this was temporarily modified to use with the session initial sound
        StartTrialTimer.Interval = Math.Max(1, InterTrialInterval * 1000)

        'Notes that the start time has passed
        StartTrialTimerHasTicked = True

        'Lauches the presentation of the trial, if the sound preparation is finished
        If CurrentTrialSoundIsReady = True Then

            'Launching the trial
            LaunchTrial(CurrentTestSound)
        End If

    End Sub



    Private Sub PrepareNewSound()

        Try

            'Resetting CurrentTestSound
            CurrentTestSound = Nothing

            If CurrentSipTestMeasurement.TestProcedure.AdaptiveType <> SipTest.AdaptiveTypes.Fixed Then
                'Levels only need to be set here, and possibly not even here, in adaptive procedures. Its better if the level is set directly upon selection of the trial...
                'CurrentSipTrial.SetLevels()
            End If


            Dim TestWordStartTime As Double = SipMeasurementRandomizer.Next(MinimumTestWordStartTime, MaximumTestWordStartTime)
            Dim CurrentComponentSound = CurrentSipTrial.SpeechMaterialComponent.GetSound(CurrentSipTrial.MediaSet, 1, 1)
            Dim TestWordCompletedTime As Double = TestWordStartTime + CurrentComponentSound.WaveData.SampleData(1).Length / CurrentComponentSound.WaveFormat.SampleRate

            If SimulationMode = False Then 'We don't need to prepare the test sound in simulation mode

                'Testing just with the unmixed sound
                CurrentTestSound = CurrentComponentSound

                'CurrentTestSound = CurrentSipTrial.CreateSoundStimulus(CurrentSipTrial,
                '                                                            TestWordStartTime,
                '                                                            TestWordCompletedTime,
                '                                                            SimulateHearingLoss,
                '                                                            CompensateHearingLoss)
            End If


            'Setting visual que intervals
            If SimulationMode = False Then
                ShowVisualQueTimer.Interval = Math.Max(1, TestWordStartTime * 1000)
                HideVisualQueTimer.Interval = Math.Max(2, TestWordCompletedTime * 1000)
                ShowResponseAlternativesTimer.Interval = HideVisualQueTimer.Interval + 1000 * ResponseAlternativeDelay 'TestSetup.CurrentEnvironment.TestSoundMixerSettings.ResponseAlternativeDelay * 1000
                MaxResponseTimeTimer.Interval = ShowResponseAlternativesTimer.Interval + 1000 * MaximumResponseTime  ' TestSetup.CurrentEnvironment.TestSoundMixerSettings.MaximumResponseTime * 1000
            End If

            If SimulationMode = False Then

                'Launches the trial if the start timer has ticked, without launching the trial (which happens when the sound preparation was not completed at the tick)
                If StartTrialTimerHasTicked = True Then
                    If CurrentTrialIsLaunched = False Then

                        'Launching the trial
                        LaunchTrial(CurrentTestSound)

                    End If
                End If

            Else
                'Simulating a respons directly without displaying anything on the screen
                'The response is based on the the presented SNR and the hearing level of simulated patient, using a bernoulli trial
                Dim CorrectResponse As String = CurrentSipTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling")

                'Assigning approximate success probabilities depending on SNR and the testee hearing level
                Dim SuccessProbability As Double = CurrentSipTrial.EstimatedSuccessProbability(True)

                Dim BernoulliTrialResult = MathNet.Numerics.Distributions.Bernoulli.Sample(SipMeasurementRandomizer, SuccessProbability)
                Dim SimulatedResponse As String = ""
                If BernoulliTrialResult = 1 Then
                    SimulatedResponse = CorrectResponse
                Else
                    'Selecting an incorrect alternative
                    Dim IncorrectAlternatives = New List(Of String)
                    For Each Spelling In TestWordAlternatives
                        If Spelling = CorrectResponse Then Continue For
                        IncorrectAlternatives.Add(Spelling)
                    Next

                    'Selecting a random incorrect response
                    If IncorrectAlternatives.Count > 0 Then
                        SimulatedResponse = IncorrectAlternatives(SipMeasurementRandomizer.Next(0, IncorrectAlternatives.Count))
                    End If
                End If

                'Calling the response sub
                TestWordResponse_TreadSafe(SimulatedResponse)

            End If

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
        End Try

    End Sub

    Private TrialLaunchSpinLock As New Threading.SpinLock

    Private Sub LaunchTrial(TestSound As Audio.Sound)

        'Declaring a spin lock taken variable
        Dim SpinLockTaken As Boolean = False

        Try

            'Attempts to enter a spin lock to avoid multiple threads calling LaunchTrial before CurrentTrialIsLaunched is updated
            TrialLaunchSpinLock.Enter(SpinLockTaken)

            'Exits the sub if the trial has already been started
            If CurrentTrialIsLaunched = True Then
                Exit Sub
            End If

            'Notes that the current trial has been started
            CurrentTrialIsLaunched = True

            'Removes any controls on the ParticipantControl.TestWordPanel
            ParticipantControl.ResetTestWordPanel()

            'Plays sound
            If SimulationMode = False Then SoundPlayer.SwapOutputSounds(TestSound)

            'Presents the visual que
            If UseVisualQue = True Then
                ShowVisualQueTimer.Start()
                HideVisualQueTimer.Start()
            End If

            'Starts response timers
            ShowResponseAlternativesTimer.Start()
            MaxResponseTimeTimer.Start()

        Finally

            'Releases any spinlock
            If SpinLockTaken = True Then TrialLaunchSpinLock.Exit()
        End Try

    End Sub

    Private Sub ShowVisualQueTimer_Tick() Handles ShowVisualQueTimer.Elapsed
        ShowVisualQueTimer.Stop()
        ParticipantControl.ShowVisualQue()
    End Sub

    Private Sub HideVisualQueTimer_Tick() Handles HideVisualQueTimer.Elapsed
        HideVisualQueTimer.Stop()
        ParticipantControl.HideVisualQue()
    End Sub

    Private Sub ShowResponseAlternativesTimer_Tick() Handles ShowResponseAlternativesTimer.Elapsed
        ShowResponseAlternativesTimer.Stop()

        'Noting the response presentation time, as synchrinized with the presentation of the response alternatives
        ResponseAlternativesPresentationTime = DateTime.Now

        ParticipantControl.ShowResponseAlternatives(TestWordAlternatives)

    End Sub

    Private Sub MaxResponseTimeTimer_Tick() Handles MaxResponseTimeTimer.Elapsed
        MaxResponseTimeTimer.Stop()

        'Calculating the responose time
        Dim ResponseTime As TimeSpan = DateTime.Now - ResponseAlternativesPresentationTime

        'Triggers a time out signal on the testee screen
        ParticipantControl.ResponseTimesOut()

        'Sends an empty result, to signal the lack of response
        StoreResult("", ResponseTime)
    End Sub



    Public Sub TestWordResponse_TreadSafe(ByVal ResponseString As String) Handles ParticipantControl.TestWordResponse

        If Me.InvokeRequired = True Then
            Dim d As New StringArgReturningVoidDelegate(AddressOf TestWordResponse_Unsafe)
            Me.Invoke(d, New Object() {ResponseString})
        Else
            Me.TestWordResponse_Unsafe(ResponseString)
        End If

    End Sub


    Public Sub TestWordResponse_Unsafe(ByVal ResponseString As String)

        'Stopping the max response time timer, as a response has been given in time.
        MaxResponseTimeTimer.Stop()

        'Calculating the responose time
        Dim ResponseTime As TimeSpan

        If SimulationMode = False Then
            ResponseTime = DateTime.Now - ResponseAlternativesPresentationTime
        Else
            'Randomizing a simulated response time
            Dim SimulatedRawRT As Double = MathNet.Numerics.Distributions.Normal.Sample(SipMeasurementRandomizer, 0.5, 0.1)
            Dim ResponseDurationMilliseconds As Double = 1000 * Math.Min(MaximumResponseTime, Math.Max(0.1, 1 / SimulatedRawRT))
            ResponseTime = New TimeSpan(0, 0, 0, Int(ResponseDurationMilliseconds / 1000), Int(ResponseDurationMilliseconds Mod 1000))
        End If

        'Sends the result and response time on
        StoreResult(ResponseString, ResponseTime)

    End Sub

    Private Sub StoreResult(ByVal ResponseString As String, ByVal ResponseTime As TimeSpan)

        'Converting the response time to seconds
        Dim CurrentResponseTime As Integer = ResponseTime.TotalMilliseconds

        CurrentSipTrial.ResponseMoment = DateTime.Now

        'Stores the response time 
        CurrentSipTrial.ResponseTime = CurrentResponseTime

        'Stores the response
        CurrentSipTrial.Response = ResponseString

        'Corrects the response
        Dim CorrectResponse As String = CurrentSipTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling")

        If ResponseString = CorrectResponse Then
            CurrentSipTrial.Result = SipTest.PossibleResults.Correct

        ElseIf ResponseString = "" Then
            CurrentSipTrial.Result = SipTest.PossibleResults.Missing

            If SimulationMode = True Then
                'Overriding the response time and sets it to max response time
                CurrentResponseTime = MaximumResponseTime
            End If

        Else
            'Determining the erraneous word (among the presented alternatives)
            CurrentSipTrial.Result = SipTest.PossibleResults.Incorrect
        End If


        'Getting the screen position of the test word
        Dim TestWordScreenPosition As Integer = -1
        For n = 0 To TestWordAlternatives.Count - 1
            If TestWordAlternatives(n) = CorrectResponse Then TestWordScreenPosition = n
        Next

        'Getting the response screen position
        Dim ResponseScreenPosition As Integer? = Nothing
        For n = 0 To TestWordAlternatives.Count - 1
            If TestWordAlternatives(n) = ResponseString Then ResponseScreenPosition = n
        Next

        'These can be used to store the screen position of the alternatives!
        'TestWordScreenPosition
        'ResponseScreenPosition

        'Dim ResponseAlternativePresentationDelay As Double = ResponseAlternativeDelay
        'Dim TestWordDuration As Double = CurrentSipTrial.SpeechMaterialComponent.GetSound.SoundRecording.SMA.ChannelData(1)(sentence).Length / CurrentSipTrial.SoundRecording.WaveFormat.SampleRate

        'Dim TestPhonemeStartDelayReWordStart As Double =
        '    (CurrentSipTrial.SoundRecording.SMA.ChannelData(1)(sentence)(0).PhoneData(CurrentSipTrial.ParentTestWord.ParentTestWordList.ContrastedPhonemeIndex).StartSample -
        '    CurrentSipTrial.SoundRecording.SMA.ChannelData(1)(sentence)(0).PhoneData(0).StartSample) /
        '    CurrentSipTrial.SoundRecording.WaveFormat.SampleRate

        'Dim TestPhonemeDuration As Double = CurrentSipTrial.SoundRecording.SMA.ChannelData(1)(sentence)(0).PhoneData(CurrentSipTrial.ParentTestWord.ParentTestWordList.ContrastedPhonemeIndex).Length / CurrentSipTrial.SoundRecording.WaveFormat.SampleRate


        'Updates the progress bar
        If ShowProgressIndication = True Then
            ParticipantControl.UpdateTestFormProgressbar(CurrentSipTestMeasurement.ObservedTrials.Count, CurrentSipTestMeasurement.ObservedTrials.Count + CurrentSipTestMeasurement.PlannedTrials.Count)
        End If


        'Starting the next trial
        If SimulationMode = False Then
            PrepareAndLaunchTrial_ThreadSafe()
        Else
            StartTrialTimerTick()
        End If

    End Sub



    Public Sub FinalizeTesting()

        StopAllTimers()

        Try

            ParticipantControl.ResetTestWordPanel()

            ParticipantControl.ShowMessage("Testet är klart!")
            'ParticipantControl.ShowMessage(GUIDictionary.SubTestIsCompleted)

            'Saving results to the log folder
            'If SimulationMode = False Then _TestResults.SaveResultsToFile(Path.Combine(logFilePath, "AutoLoggedResults"), "TestResultLogExport_" & TestSessionDescription.PatientDetails.ID)


            'Summarizes the result
            CurrentSipTestMeasurement.SummarizeTestResults()
            MeasurementHistory.Measurements.Add(CurrentSipTestMeasurement)

            'Display results
            PopulateTestHistoryTables()

            'Export data here?


            'Resets values to prepare for next measurement
            ResetValuesAfterMeasurement()

            'Disposing the sound player
            If SimulationMode = False Then
                SoundPlayer.SwapOutputSounds(Nothing)

                'Sleeps during the fade out phase
                Threading.Thread.Sleep(SoundPlayer.GetOverlapDuration * 1000)

                SoundPlayer.CloseStream()
                SoundPlayer.Dispose()
            End If

            TestIsStarted = False

            If CurrentScreenType = ScreenType.Pc Then
                'Unlocks the cursor 
                UnlockCursor()
            End If

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
        End Try


    End Sub


    Public Sub ResetValuesAfterMeasurement()

        ClearTestNameBox()

        UnlockSettingsPanels()

        MsgBox("Unlock stuff for new test!")

    End Sub


    Public Sub PauseTesting()
        IsPaused = True

        'Stopping response timers
        StopAllTimers()

        'Resets the test word panel
        ParticipantControl.ResetTestWordPanel()

        'Changing to silence
        SoundPlayer.SwapOutputSounds(Nothing)

        'Displays a message to the testee
        ParticipantControl.ShowMessage("Testet är pausat")
        'ParticipantControl.ShowMessage(GUIDictionary.TestingIsPaused)

    End Sub

    Public Sub ResumeTesting()

        If IsPaused = True Then
            IsPaused = False

            ParticipantControl.ResetTestWordPanel()

            PrepareAndLaunchTrial_ThreadSafe()

        End If

    End Sub

    Public Sub StopTesting()
        StopAllTimers()
        FinalizeTesting()
    End Sub

    Public Sub StopAllTimers()
        StartTrialTimer.Stop()
        ShowVisualQueTimer.Stop()
        HideVisualQueTimer.Stop()
        ShowResponseAlternativesTimer.Stop()
        MaxResponseTimeTimer.Stop()
    End Sub

#End Region


End Class
