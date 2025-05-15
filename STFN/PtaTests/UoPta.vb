
Public Class UoPta

    Private Shared FilePathRepresentation As String = "UoPta"
    Public Shared SaveAudiogramDataToFile As Boolean = True
    Public Shared LogTrialData As Boolean = True

#Region "Level settings"

    Public Shared MinPresentationLevel As Integer = 0
    Public Shared MaxPresentationLevel As Integer = 80

#End Region

#Region "Time settings"

    'These settings allows for approximately 2-5 seconds between tones
    ' and tone duration uniformly distributed within the range of 1-2 seconds

    Public Shared MinToneOnsetTime As Double = 0.5
    Public Shared MaxToneOnsetTime As Double = 3.5
    Public Shared MinToneDurationTime As Double = 1
    Public Shared MaxToneDurationTime As Double = 2

    Public Shared PostToneDuration As Double = 1.5 ' Note! This duration cannot be allowed to be shorter than the UpperOffsetResponseTimeLimit
    Public Shared PostTruePositiveResponseDuration As Double = 1 ' Assuming that mean response time is approximately 0.5 s

    ' These settings sets the criteria for valid responses
    Public Shared LowerOnsetResponseTimeLimit As Double = 0.1
    Public Shared UpperOnsetResponseTimeLimit As Double = 1
    Public Shared LowerOffsetResponseTimeLimit As Double = 0.1
    Public Shared UpperOffsetResponseTimeLimit As Double = 1 ' Note! This limit must be higher than PostToneDuration

#End Region

    Private TestStarted As Boolean
    Public Function IsStarted() As Boolean
        Return TestStarted
    End Function

    Private TestIsCompleted As Boolean
    Public Function IsCompleted() As Boolean
        Return TestIsCompleted
    End Function


    Private TestFrequencies As New List(Of Integer) From {1000, 2000, 4000, 6000, 500}

    Public FirstSide As Utils.Sides = Utils.Constants.Sides.Right

    ''' <summary>
    ''' Keeps track of whether the second side (i.e. left or right) has been started.
    ''' </summary>
    Public SecondSideIsStarted As Boolean = False

    Private SubTests As New List(Of PtaSubTest)

    Private Randomizer As New Random

    ''' <summary>
    ''' Runs and stores the threshold procedure for a single side and frequency
    ''' </summary>
    Public Class PtaSubTest
        Public Frequency As Integer
        Public Side As Utils.Sides
        Public Trials As New List(Of PtaTrial)
        Public Threshold As Integer? = Nothing
        Public ThresholdStatus As ThresholdStatuses = ThresholdStatuses.NA
        Public Randomizer As Random
        Public IsReliabilityCheck As Boolean

        Private IsInitiatingRepeatedThresholdProcedure As Boolean = False 'This variable is used (and temporarily set to True) when the threshold procedure must re-initiated, according to point 6 in the guidelines
        Private LastTruePositiveLevel As Integer

        Public PresentedTrials As Integer = 0

        Public Enum ThresholdStatuses
            NA
            Reached
            Unreached
        End Enum

        Public Sub New(ByRef Randomizer As Random)
            Me.Randomizer = Randomizer
        End Sub

        ''' <summary>
        ''' Checks if the threshold corresponding to the current PtaSubTest has been reached
        ''' </summary>
        ''' <returns></returns>
        Public Function CheckIfThresholdIsReached() As Boolean

            'Implementing the Hughson-Westlake method according to the Swedish guidelines (2025)
            If Trials.Count = 0 Then Return False

            Dim NeedsReinitiation As Boolean = False

            'Determining the index of the first trial which was not heard or presented at the minimum presentation level, here called the LevelDescentBreakPointTrial 
            Dim LevelDescentBreakPointTrialIndex As Integer = -1
            For i = 0 To Trials.Count - 1
                'Looking for the first non-heard trial
                If Trials(i).Result <> PtaResults.TruePositive Then
                    LevelDescentBreakPointTrialIndex = i
                    Exit For
                End If
                'And the first trial at the minimum presentation level, regardless if it was heard or not 
                If Trials(i).ToneLevel = MinPresentationLevel Then
                    LevelDescentBreakPointTrialIndex = i
                    Exit For
                End If
            Next

            'Returns False if the level-descent break point has not yet been reached
            If LevelDescentBreakPointTrialIndex = -1 Then Return False

            'Creating a list of threshold-candidate trials (starting from the level-descent break point trial)
            ' Including the heard trials after the first non-heard
            ' Including also non-heard trials at the maximum output level (as these are needed to determine non-reached thresholds)
            Dim ThresholdCandidateTrials As New List(Of PtaTrial)
            For i = LevelDescentBreakPointTrialIndex To Trials.Count - 1
                If Trials(i).Result = PtaResults.TruePositive Then
                    ThresholdCandidateTrials.Add(Trials(i))
                Else
                    If Trials(i).ToneLevel = MaxPresentationLevel Then
                        ThresholdCandidateTrials.Add(Trials(i))
                    End If
                End If
            Next

            ' Returns false if there are less than three threshold-candidate trials
            If ThresholdCandidateTrials.Count < 3 Then Return False

            'Checking if three out of three, four or five threshold-candidate trials have the same level
            If ThresholdCandidateTrials.Count < 6 Then
                Dim TrialLevelList As New SortedList(Of Integer, Integer)
                Dim TrialLevelScoreList As New SortedList(Of Integer, Integer)
                For i = 0 To ThresholdCandidateTrials.Count - 1

                    ' Counting the number of trials at each level
                    If TrialLevelList.ContainsKey(ThresholdCandidateTrials(i).ToneLevel) = False Then TrialLevelList.Add(ThresholdCandidateTrials(i).ToneLevel, 0)
                    TrialLevelList(ThresholdCandidateTrials(i).ToneLevel) += 1

                    'Counting the number of heard trials at each level 
                    If TrialLevelScoreList.ContainsKey(ThresholdCandidateTrials(i).ToneLevel) = False Then TrialLevelScoreList.Add(ThresholdCandidateTrials(i).ToneLevel, 0)
                    If ThresholdCandidateTrials(i).Result = PtaResults.TruePositive Then
                        TrialLevelScoreList(ThresholdCandidateTrials(i).ToneLevel) += 1
                    End If

                Next

                For Each kvp In TrialLevelList
                    If kvp.Value > 2 Then
                        'Threshold is reached, storing the threshold level and returns True
                        Threshold = kvp.Key

                        'Noting if the threshold was reached or not
                        ThresholdStatus = ThresholdStatuses.Reached ' setting Reached as default, and changing it only if unreached

                        ' The rule applied here is that at least three trials must have been heard at the max presentation level for the threshold to be counted as Reached.
                        ' If less than three thresholds at the max presentation level was heard, the threshold is counted as unreached

                        If Threshold = MaxPresentationLevel Then
                            If TrialLevelScoreList(Threshold) < 3 Then
                                ThresholdStatus = ThresholdStatuses.Unreached
                            End If
                        End If

                        Return True
                    End If
                Next

                If ThresholdCandidateTrials.Count = 5 Then
                    'Three of five threshold crossings have not resulted in an established threshold. Re-initializing threshold procedure.
                    NeedsReinitiation = True
                End If

            Else
                Throw New Exception("ThresholdCandidateTrials should never contain more than five trials")
            End If


            If NeedsReinitiation = True Then

                'The threshold procedure must be re-initiated
                'Storing the last level where the tone was heard, or 40 dB HL if no tones have yet been heard
                Dim TempLastTruePositivelevel As Integer? = Nothing
                For Each Trial In Trials
                    If Trial.Result = PtaResults.TruePositive Then
                        If TempLastTruePositivelevel.HasValue = False Then
                            TempLastTruePositivelevel = Trial.ToneLevel
                        Else
                            TempLastTruePositivelevel = Math.Min(Trial.ToneLevel, TempLastTruePositivelevel.Value)
                        End If
                    End If
                Next
                If TempLastTruePositivelevel.HasValue Then
                    LastTruePositiveLevel = TempLastTruePositivelevel.Value
                Else
                    LastTruePositiveLevel = 40
                End If

                'Emptying Trials if repeated threshold procedure must be made
                Trials.Clear()

                'Notes that re-initiating is started
                IsInitiatingRepeatedThresholdProcedure = True

            End If

            'Returns False at this point, as no threshold has been established
            Return False

        End Function

        ''' <summary>
        ''' Prapares the next PtaTrial, or returns nothing if the threshold has been established
        ''' </summary>
        ''' <returns></returns>
        Public Function GetNextTrial() As PtaTrial

            'Returns nothing if threshold is already reached (checking first if any value has been stored in the Threshold property, or else runs the full procedure for determining if the threshold has been reached
            If Threshold IsNot Nothing Then Return Nothing
            If CheckIfThresholdIsReached() = True Then Return Nothing

            'Creates a new trial
            Dim NewTrial As New PtaTrial(Me)

            'Counting trials
            PresentedTrials += 1

            'Setting the level to be presented
            'If it is the first trial, we create a new one with the level 40 dB HL
            If Trials.Count = 0 Then
                If IsInitiatingRepeatedThresholdProcedure = False Then
                    NewTrial.ToneLevel = 40
                Else
                    'Setting the level to the last heard level
                    NewTrial.ToneLevel = LastTruePositiveLevel
                    'Ending re-initiation
                    IsInitiatingRepeatedThresholdProcedure = False

                    'Noting in the trial that this is a re-initializing trial
                    NewTrial.ReinitializingProcedure = True
                End If
            Else

                'Checks if level should be raised or lowered, and by how much

                Dim IncreaseStepSize As Integer = 10 ' Setting the increase step size to 10 dB
                'Checking if any has been heard yet
                For Each Trial In Trials
                    If Trial.Result = PtaResults.TruePositive Then
                        'Tones have been heard, changing the increase step to 5 dB
                        IncreaseStepSize = 5
                        Exit For
                    End If
                Next

                'Checking if any tone has been missed so far
                Dim DecreaseStepSize As Integer = 20 ' Setting the decrease step size to 20 dB
                For Each Trial In Trials
                    If Trial.Result <> PtaResults.TruePositive Then
                        'Tones have been missed, changing the decrease step to 10 dB
                        DecreaseStepSize = 10
                        Exit For
                    End If
                Next

                'Checks if level should be raised or lowered
                If Trials.Last.Result = PtaResults.TruePositive Then
                    NewTrial.ToneLevel = Trials.Last.ToneLevel - DecreaseStepSize
                Else
                    NewTrial.ToneLevel = Trials.Last.ToneLevel + IncreaseStepSize
                End If

                'Limiting the presentation level in the lower end to MinPresentationLevel
                NewTrial.ToneLevel = Math.Max(NewTrial.ToneLevel, MinPresentationLevel)

                'Limiting the presentation level in the upper end to MaxPresentationLevel
                NewTrial.ToneLevel = Math.Min(NewTrial.ToneLevel, MaxPresentationLevel)

            End If

            'Randomizing and setting up times
            NewTrial.SetupTrial(Me.Randomizer)

            'Adding the new trial to the trial list
            Trials.Add(NewTrial)

            Return NewTrial

        End Function

    End Class

    Public Class PtaTrial

        Public ReadOnly ParentSubTest As PtaSubTest

        Public Shared AddTrialExportHeadings As Boolean = True

        ''' <summary>
        ''' The signal level in dB HL
        ''' </summary>
        ''' <returns></returns>
        Public Property ToneLevel As Integer

        Public Property TrialStartTime As DateTime

        Public Property ToneOnsetTime As TimeSpan
        Public Property ToneDuration As TimeSpan
        Public ReadOnly Property ToneOffsetTime As TimeSpan
            Get
                Return ToneOnsetTime + ToneDuration
            End Get
        End Property

        Public LowestValidOnsetResponseTime As TimeSpan
        Public HighestValidOnsetResponseTime As TimeSpan
        Public LowestValidOffsetResponseTime As TimeSpan
        Public HighestValidOffsetResponseTime As TimeSpan

        Private _ResponseOnsetTime As TimeSpan
        Public Property ResponseOnsetTime As TimeSpan
            Get
                Return _ResponseOnsetTime
            End Get
            Set(value As TimeSpan)
                _ResponseOnsetTime = value

                'Setting the onset result
                If _ResponseOnsetTime < LowestValidOnsetResponseTime Then
                    OnsetResult = PtaResults.FalsePositive
                ElseIf _ResponseOnsetTime > HighestValidOnsetResponseTime Then
                    OnsetResult = PtaResults.FalsePositive
                Else
                    OnsetResult = PtaResults.TruePositive
                End If

            End Set
        End Property

        Private _ResponseOffsetTime As TimeSpan
        Public Property ResponseOffsetTime As TimeSpan
            Get
                Return _ResponseOffsetTime
            End Get
            Set(value As TimeSpan)
                _ResponseOffsetTime = value

                'Setting the offset result
                If _ResponseOffsetTime < LowestValidOffsetResponseTime Then
                    OffsetResult = PtaResults.FalsePositive
                ElseIf _ResponseOffsetTime > HighestValidOffsetResponseTime Then
                    OffsetResult = PtaResults.FalsePositive
                Else
                    OffsetResult = PtaResults.TruePositive
                End If

            End Set
        End Property

        Public Property OnsetResult As PtaResults = PtaResults.NoResponse ' Using NoResponse as default until a response is given
        Public Property OffsetResult As PtaResults = PtaResults.NoResponse ' Using NoResponse as default until a response is given

        Public ReadOnly Property Result As PtaResults
            Get
                'Determining the result from the onset and off set results
                If OffsetResult = PtaResults.NoResponse Then Return PtaResults.NoResponse

                If OnsetResult = PtaResults.TruePositive And OffsetResult = PtaResults.TruePositive Then
                    Return PtaResults.TruePositive
                Else
                    Return PtaResults.FalsePositive
                End If

            End Get
        End Property

        ''' <summary>
        ''' Holds the mixed sound to be played, including silence before and after the tone
        ''' </summary>
        Public MixedSound As Audio.Sound

        Public Property ReinitializingProcedure As Boolean = False

        Public Sub New(ByRef ParentSubTest As PtaSubTest)
            Me.ParentSubTest = ParentSubTest
        End Sub

        Public Sub SetupTrial(ByRef rnd As Random)

            'Randomizing tone onset time and duration
            Dim SignalOnsetTimeMs As Double = rnd.Next(1000 * MinToneOnsetTime, 1000 * MaxToneOnsetTime)
            Dim SignalDurationMs As Double = rnd.Next(1000 * MinToneDurationTime, 1000 * MaxToneDurationTime)

            ToneOnsetTime = TimeSpan.FromMilliseconds(SignalOnsetTimeMs)
            ToneDuration = TimeSpan.FromMilliseconds(SignalDurationMs)

            'Calculating the valid response time ranges
            LowestValidOnsetResponseTime = ToneOnsetTime + TimeSpan.FromSeconds(LowerOnsetResponseTimeLimit)
            HighestValidOnsetResponseTime = ToneOnsetTime + TimeSpan.FromSeconds(UpperOnsetResponseTimeLimit)
            LowestValidOffsetResponseTime = ToneOffsetTime + TimeSpan.FromSeconds(LowerOffsetResponseTimeLimit)
            HighestValidOffsetResponseTime = ToneOffsetTime + TimeSpan.FromSeconds(UpperOffsetResponseTimeLimit)

        End Sub

        Public Sub ExportPtaTrialData()

            'Skipping saving data if it's the demo ptc ID
            If SharedSpeechTestObjects.CurrentParticipantID.Trim = SharedSpeechTestObjects.NoTestId Then Exit Sub

            If SharedSpeechTestObjects.TestResultsRootFolder = "" Then
                Messager.MsgBox("Unable to save the results to file due to missing test results output folder. This should have been selected first startup of the app!")
                Exit Sub
            End If

            If IO.Directory.Exists(SharedSpeechTestObjects.TestResultsRootFolder) = False Then
                Try
                    IO.Directory.CreateDirectory(SharedSpeechTestObjects.TestResultsRootFolder)
                Catch ex As Exception
                    Messager.MsgBox("Unable to save the results to the test results output folder (" & SharedSpeechTestObjects.TestResultsRootFolder & "). The path does not exist, and could not be created!")
                End Try
                Exit Sub
            End If

            Dim ExportedLines As New List(Of String)
            If AddTrialExportHeadings = True Then
                PtaTrial.AddTrialExportHeadings = False

                ExportedLines.Add("TrialStartTime" & vbTab &
                              "Side" & vbTab & "Frequency" & vbTab & "SubTestTrial" & vbTab & "ReinitializingProcedure" & vbTab & "ToneLevel" & vbTab &
                              "Result" & vbTab & "OnsetResult" & vbTab & "OffsetResult" & vbTab &
                              "ToneOnsetTime" & vbTab & "ToneOffsetTime" & vbTab & "ToneDuration" & vbTab &
                              "ResponseOnsetTime" & vbTab & "ResponseOffsetTime" & vbTab &
                              "LowestValidOnsetResponseTime" & vbTab & "HighestValidOnsetResponseTime" & vbTab &
                              "LowestValidOffsetResponseTime" & vbTab & "HighestValidOffsetResponseTime")
            End If

            ExportedLines.Add(Me.TrialStartTime.ToLongDateString & vbTab &
                              ParentSubTest.Side.ToString & vbTab & ParentSubTest.Frequency & vbTab & ParentSubTest.PresentedTrials & vbTab & ReinitializingProcedure.ToString & vbTab & Me.ToneLevel & vbTab &
                              Me.Result.ToString & vbTab & Me.OnsetResult.ToString & vbTab & Me.OffsetResult.ToString & vbTab &
                              Me.ToneOnsetTime.TotalSeconds & vbTab & Me.ToneOffsetTime.TotalSeconds & vbTab & Me.ToneDuration.TotalSeconds & vbTab &
                              Me.ResponseOnsetTime.TotalSeconds & vbTab & Me.ResponseOffsetTime.TotalSeconds & vbTab &
                              Me.LowestValidOnsetResponseTime.TotalSeconds & vbTab & Me.HighestValidOnsetResponseTime.TotalSeconds & vbTab &
                              Me.LowestValidOffsetResponseTime.TotalSeconds & vbTab & Me.HighestValidOffsetResponseTime.TotalSeconds)

            Dim OutputPath = IO.Path.Combine(SharedSpeechTestObjects.TestResultsRootFolder, UoPta.FilePathRepresentation)
            Dim OutputFilename = UoPta.FilePathRepresentation & "_PtaTrialData_" & SharedSpeechTestObjects.CurrentParticipantID

            Utils.SendInfoToLog(String.Join(vbCrLf, ExportedLines), OutputFilename, OutputPath, False, True, False, True, True)

        End Sub


    End Class

    Public Enum PtaResults
        TruePositive
        FalsePositive
        NoResponse
    End Enum

    Public Sub New()

        Me.Randomizer = New Random ' N.B. this randomized could be supplied by the calling code to enable exact replication

        'Resets SecondSideStarted (will probably never be necessary)
        SecondSideIsStarted = False

        'Resets PtaTrial.AddTrialExportHeadings so that the first line in each PTA measurment gets the headings
        PtaTrial.AddTrialExportHeadings = True

        AddFirstSideSubTests()

    End Sub

    Public Sub AddFirstSideSubTests()

        'Adding the first side tests

        'Creating subtests
        If FirstSide = Utils.Constants.Sides.Right Then

            'Adding subtests
            For Each Frequency In TestFrequencies
                SubTests.Add(New PtaSubTest(Randomizer) With {.Side = Utils.Constants.Sides.Right, .Frequency = Frequency, .IsReliabilityCheck = False})
            Next

            'Adding the reliability check
            SubTests.Add(New PtaSubTest(Randomizer) With {.Side = Utils.Constants.Sides.Right, .Frequency = 1000, .IsReliabilityCheck = True})

        Else

            'Adding subtests
            For Each Frequency In TestFrequencies
                SubTests.Add(New PtaSubTest(Randomizer) With {.Side = Utils.Constants.Sides.Left, .Frequency = Frequency, .IsReliabilityCheck = False})
            Next

            'Adding the reliability check
            SubTests.Add(New PtaSubTest(Randomizer) With {.Side = Utils.Constants.Sides.Left, .Frequency = 1000, .IsReliabilityCheck = True})

        End If

    End Sub


    Public Sub AddSecondSideSubTests()

        'Adding the second side tests

        'Creating subtests
        If FirstSide = Utils.Constants.Sides.Right Then

            'Adding subtests
            For Each Frequency In TestFrequencies
                SubTests.Add(New PtaSubTest(Randomizer) With {.Side = Utils.Constants.Sides.Left, .Frequency = Frequency, .IsReliabilityCheck = False})
            Next

        Else

            'Adding subtests
            For Each Frequency In TestFrequencies
                SubTests.Add(New PtaSubTest(Randomizer) With {.Side = Utils.Constants.Sides.Right, .Frequency = Frequency, .IsReliabilityCheck = False})
            Next

        End If

        'Notes that also the side has been started
        SecondSideIsStarted = True

    End Sub


    ''' <summary>
    ''' Prepares next PtaTrial. Returns Nothing if the test is completed.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetNextTrial() As PtaTrial

        'Noting that the test is started as soon as this method is called
        TestStarted = True

        'Getting the next non-completed subtest
        Dim CurrentSubTest = GetFirstNonCompleteSubTest()

        If CurrentSubTest Is Nothing Then
            'If CurrentSubTest is Nothing, it means that the first or the second side has been completed

            If SecondSideIsStarted = False Then

                AddSecondSideSubTests()

                'Starting measurments on the second side
                CurrentSubTest = GetFirstNonCompleteSubTest()

            Else

                'Noting that the test is completed
                TestIsCompleted = True

                'All sub tests are completed
                Return Nothing

            End If

        End If

        Return CurrentSubTest.GetNextTrial

    End Function

    ''' <summary>
    ''' Looks through the SubTests list and returns the first sub test for which the threshold has not yet been established
    ''' </summary>
    ''' <returns></returns>
    Public Function GetFirstNonCompleteSubTest() As PtaSubTest
        For Each SubTest In SubTests
            If SubTest.CheckIfThresholdIsReached() = False Then
                Return SubTest
            End If
        Next
        Return Nothing
    End Function

    Public Sub ExportAudiogramData()

        'Skipping saving data if it's the demo ptc ID
        If SharedSpeechTestObjects.CurrentParticipantID.Trim = SharedSpeechTestObjects.NoTestId Then Exit Sub

        If SharedSpeechTestObjects.TestResultsRootFolder = "" Then
            Messager.MsgBox("Unable to save the results to file due to missing test results output folder. This should have been selected first startup of the app!")
            Exit Sub
        End If

        If IO.Directory.Exists(SharedSpeechTestObjects.TestResultsRootFolder) = False Then
            Try
                IO.Directory.CreateDirectory(SharedSpeechTestObjects.TestResultsRootFolder)
            Catch ex As Exception
                Messager.MsgBox("Unable to save the results to the test results output folder (" & SharedSpeechTestObjects.TestResultsRootFolder & "). The path does not exist, and could not be created!")
            End Try
            Exit Sub
        End If

        Dim AudiogramList As New List(Of String)
        AudiogramList.Add("Side" & vbTab & "Frequency" & vbTab & "Threshold" & vbTab & "ThresholdStatus")
        For Each SubTest In SubTests
            AudiogramList.Add(SubTest.Side.ToString & vbTab & SubTest.Frequency & vbTab & SubTest.Threshold & vbTab & SubTest.ThresholdStatus.ToString)
        Next

        Dim OutputPath = IO.Path.Combine(SharedSpeechTestObjects.TestResultsRootFolder, UoPta.FilePathRepresentation)
        Dim OutputFilename = UoPta.FilePathRepresentation & "_AudiogramData_" & SharedSpeechTestObjects.CurrentParticipantID

        Utils.SendInfoToLog(String.Join(vbCrLf, AudiogramList), OutputFilename, OutputPath, False, True, False, True, True)

    End Sub

End Class