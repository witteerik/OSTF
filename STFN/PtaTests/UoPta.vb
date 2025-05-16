

Public Class UoPta

    Private Shared FilePathRepresentation As String = "UoPta"
    Public Shared SaveAudiogramDataToFile As Boolean = True
    Public Shared LogTrialData As Boolean = True
    Public ResultSummary As String = ""

#Region "Protocol settings"

    Public Shared PtaTestProtocol As PtaTestProtocols = PtaTestProtocols.SAME96_Screening

    Public Enum PtaTestProtocols
        SAME96
        SAME96_Screening
    End Enum

#End Region

#Region "Evaluation settings"

    Private GoodFitLimit As Double = 10
    Private MediumFitLimit As Double = 20

#End Region


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

    Private TestRetestDifference As Integer? = Nothing

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

        Private IsInitiatingRepeatedThresholdProcedure As Boolean = False 'This variable is used (and temporarily set to True) when the threshold procedure must be re-initiated, according to point 6 in the guidelines
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

            Select Case PtaTestProtocol
                Case PtaTestProtocols.SAME96

                    'Implementing the Hughson-Westlake method according to the Swedish guidelines (2025)
                    If Trials.Count = 0 Then Return False

                    Dim NeedsReinitiation As Boolean = False

                    'Counting the number of level reversals (strictly defined as the number of level ascends and level unchanged steps), excluding 20 dB changes
                    Dim AscendingSeriesCount As Integer = 0
                    Dim ThresholdCandidateTrials As New List(Of PtaTrial)
                    Dim UnreachedThresholCandidateTrials As New List(Of PtaTrial)
                    For i = 1 To Trials.Count - 1

                        'Ignoring 20 dB steps
                        If Math.Abs(Trials(i - 1).ToneLevel - Trials(i).ToneLevel) = 20 Then Continue For

                        'Looking for completed ascending series (or retained level steps, indicating that we're on the min or max presentation level)
                        If Trials(i - 1).ToneLevel <= Trials(i).ToneLevel And Trials(i).Result = PtaResults.TruePositive Then
                            AscendingSeriesCount += 1
                        End If

                        'Collecting threshold candidates trials (i.e. true positives) 
                        'N.B. The first trial is never a threshold candidate, since it is not preceded by an incorrect trial, and need not be included
                        If Trials(i).Result = PtaResults.TruePositive Then
                            ThresholdCandidateTrials.Add(Trials(i))
                        End If

                        'Collecting candidate trials for unreached thresholds at the max presentation level
                        If Trials(i).ToneLevel = MaxPresentationLevel And Trials(i).Result <> PtaResults.TruePositive Then
                            UnreachedThresholCandidateTrials.Add(Trials(i))
                        End If

                    Next


                    'Checking if three out of three, four or five threshold-candidate trials have the same level
                    If AscendingSeriesCount < 6 Then

                        'Looking for reached thresholds
                        Dim ThresholdCandidateTrialLevelList As New SortedList(Of Integer, Integer)
                        For i = 0 To ThresholdCandidateTrials.Count - 1

                            ' Counting the number of threshold candidate trials at each level
                            If ThresholdCandidateTrialLevelList.ContainsKey(ThresholdCandidateTrials(i).ToneLevel) = False Then ThresholdCandidateTrialLevelList.Add(ThresholdCandidateTrials(i).ToneLevel, 0)
                            ThresholdCandidateTrialLevelList(ThresholdCandidateTrials(i).ToneLevel) += 1

                        Next
                        For Each kvp In ThresholdCandidateTrialLevelList
                            If kvp.Value > 2 Then
                                'Threshold is reached, storing the threshold level, status and returns True
                                Threshold = kvp.Key

                                'Noting that the threshold was reached
                                ThresholdStatus = ThresholdStatuses.Reached
                                Return True
                            End If
                        Next

                        'Looking for unreached thresholds
                        Dim UnreachedThresholdCandidateTrialLevelList As New SortedList(Of Integer, Integer)
                        For i = 0 To UnreachedThresholCandidateTrials.Count - 1

                            ' Counting the number of threshold candidate trials at each level
                            If UnreachedThresholdCandidateTrialLevelList.ContainsKey(UnreachedThresholCandidateTrials(i).ToneLevel) = False Then UnreachedThresholdCandidateTrialLevelList.Add(UnreachedThresholCandidateTrials(i).ToneLevel, 0)
                            UnreachedThresholdCandidateTrialLevelList(UnreachedThresholCandidateTrials(i).ToneLevel) += 1

                        Next

                        For Each kvp In UnreachedThresholdCandidateTrialLevelList
                            If kvp.Value > 2 Then
                                'Un Unreached Threshold has been detected, storing the threshold level, status and returns True
                                Threshold = kvp.Key

                                'Noting if the threshold was reached or not
                                ThresholdStatus = ThresholdStatuses.Unreached ' 
                                Return True
                            End If
                        Next

                        'No threshold has been reach. Checking if five ascending series has passed
                        If AscendingSeriesCount = 5 Then
                            'Three of five ascending series have not resulted in an established threshold. Re-initializing the threshold procedure.
                            NeedsReinitiation = True
                        End If

                    Else
                        Throw New Exception("AscendingSeriesCount should never be higher than five")
                    End If


                    If NeedsReinitiation = True Then

                        'The threshold procedure must be re-initiated
                        'Storing the last level where the tone was heard, or 40 dB HL if no tones have yet been heard
                        Dim TempLastTruePositivelevel As Integer? = Nothing
                        For Each Trial In Trials
                            If Trial.Result = PtaResults.TruePositive Then
                                TempLastTruePositivelevel = Trial.ToneLevel
                            End If
                        Next
                        If TempLastTruePositivelevel.HasValue Then
                            'Setting the level to 10 dB above the last heard level
                            LastTruePositiveLevel = TempLastTruePositivelevel.Value + 10
                        Else
                            LastTruePositiveLevel = 40
                        End If

                        'Emptying Trials if repeated threshold procedure must be made
                        Trials.Clear()

                        'Notes that re-initiating is started
                        IsInitiatingRepeatedThresholdProcedure = True

                        'Returns False at this point, as no threshold has been established
                        Return False

                    End If

                Case PtaTestProtocols.SAME96_Screening

                    'TODO: Implement SAME screening level procedure here
                    'Returning False for now
                    Return False

                Case Else
                    Throw New NotImplementedException("Unknown pta protocol")
            End Select


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

            Select Case PtaTestProtocol
                Case PtaTestProtocols.SAME96


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
                        Dim FirstHeardTrialIndex As Integer = 0
                        For i = 0 To Trials.Count - 1
                            If Trials(i).Result = PtaResults.TruePositive Then
                                'Tones have been heard, changing the increase step to 5 dB
                                IncreaseStepSize = 5

                                'Noting the index of the first heard trial
                                FirstHeardTrialIndex = i

                                Exit For
                            End If
                        Next

                        'Checking if any tone has been missed after the first heard trial
                        Dim DecreaseStepSize As Integer = 20 ' Setting the decrease step size to 20 dB
                        For i = FirstHeardTrialIndex To Trials.Count - 1
                            If Trials(i).Result <> PtaResults.TruePositive Then
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


                Case PtaTestProtocols.SAME96_Screening

                    'TODO: Implement SAME screening procedure here


                Case Else
                    Throw New NotImplementedException("Unknown pta protocol")
            End Select

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

    Public Enum BisgaardAudiogramsLimited
        NH
        N1
        N2
        N3
        N4
        N5
        N67
        S1
        S2
        S3
    End Enum

    Public Enum BisgaardAudiogramsLimitedFit
        Good
        Medium
        Poor
    End Enum


    Public Function ApproximateBisgaardType() As String

        Dim ResultList As New List(Of String)
        ResultList.Add("Audiogram type:")

        Dim ColumnWidth1 As Integer = 8
        Dim ColumnWidth2 As Integer = 8
        Dim ColumnWidth3 As Integer = 8
        Dim ColumnWidth4 As Integer = 8

        ResultList.Add("Side".PadRight(ColumnWidth1) &
                       "Type".PadRight(ColumnWidth2) &
                       "Fit".PadRight(ColumnWidth3) &
                       "RMSE".PadRight(ColumnWidth4))

        Dim Sides() As Utils.Sides = {Utils.Sides.Right, Utils.Sides.Left}

        For Each Side In Sides

            Dim SingleSideAudiogram As New SortedList(Of Integer, Integer)

            For Each SubTest In SubTests

                If SubTest.Side <> Side Then Continue For

                If SingleSideAudiogram.ContainsKey(SubTest.Frequency) = False Then
                    'Storing the frequency and threshold
                    SingleSideAudiogram.Add(SubTest.Frequency, SubTest.Threshold)
                Else
                    'Here we have already stored a threhold, this is therefore the retest
                    'This will overwrite a first threshold value with the repeated threshold value, and also store the test retest-value
                    Dim TestThreshold = SingleSideAudiogram(SubTest.Frequency)
                    Dim RetestThreshold = SubTest.Threshold

                    'Stroing the test-retest difference (note that this only happens once in each complete measurment
                    TestRetestDifference = RetestThreshold - TestThreshold

                    'Updating the SingleSideAudiogram with the retest threshold instead of the test threhold
                    SingleSideAudiogram(SubTest.Frequency) = RetestThreshold
                End If
            Next

            'Approximating to a Bissgaard audiogram
            Dim ApproxResult = ApproximateBisgaardType(SingleSideAudiogram)

            ResultList.Add(Side.ToString.PadRight(ColumnWidth1) &
                           ApproxResult.Item1.ToString.PadRight(ColumnWidth2) &
                           ApproxResult.Item2.ToString.PadRight(ColumnWidth3) &
                           Math.Round(ApproxResult.Item3, 1).ToString.PadRight(ColumnWidth4))

        Next

        Return String.Join(vbCrLf, ResultList)

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Audiogram">A SortedList where keys represent 'Frequency' and values represent 'Thresholds' </param>
    ''' <returns></returns>
    Public Function ApproximateBisgaardType(ByVal Audiogram As SortedList(Of Integer, Integer)) As Tuple(Of BisgaardAudiogramsLimited, BisgaardAudiogramsLimitedFit, Double)

        Dim fs() As Integer = {125, 250, 375, 500, 750, 1000, 1500, 2000, 3000, 4000, 6000, 8000}

        'Checking that no invalid frequencies are supplied
        For Each Frequency In Audiogram.Keys
            If fs.Contains(Frequency) = False Then
                Throw New ArgumentException("Non-supported frequency value supplied to function ApproximateBisgaardType!")
            End If
        Next

        Dim TempAudiograms As New SortedList(Of BisgaardAudiogramsLimited, Integer())

        'These audiograms are base on the Bisgaard set, but limited to 80 dB HL, Bisbard N6 and N7 are combined into N67. 
        ' The 8 kHz threshold is set to the same as the 6 kHz threshold
        TempAudiograms.Add(BisgaardAudiogramsLimited.NH, {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0})
        TempAudiograms.Add(BisgaardAudiogramsLimited.N1, {10, 10, 10, 10, 10, 10, 10, 15, 20, 30, 40, 40})
        TempAudiograms.Add(BisgaardAudiogramsLimited.N2, {20, 20, 20, 20, 22.5, 25, 30, 35, 40, 45, 50, 50})
        TempAudiograms.Add(BisgaardAudiogramsLimited.N3, {35, 35, 35, 35, 35, 40, 45, 50, 55, 60, 65, 65})
        TempAudiograms.Add(BisgaardAudiogramsLimited.N4, {55, 55, 55, 55, 55, 55, 60, 65, 70, 75, 80, 80})
        TempAudiograms.Add(BisgaardAudiogramsLimited.N5, {65, 65, 67.5, 70, 72.5, 75, 80, 80, 80, 80, 80, 80})
        TempAudiograms.Add(BisgaardAudiogramsLimited.N67, {75, 75, 77.5, 80, 80, 80, 80, 80, 80, 80, 80, 80})
        TempAudiograms.Add(BisgaardAudiogramsLimited.S1, {10, 10, 10, 10, 10, 10, 10, 15, 30, 55, 70, 70})
        TempAudiograms.Add(BisgaardAudiogramsLimited.S2, {20, 20, 20, 20, 22.5, 25, 35, 55, 75, 80, 80, 80})
        TempAudiograms.Add(BisgaardAudiogramsLimited.S3, {30, 30, 30, 35, 47.5, 60, 70, 75, 80, 80, 80, 80})

        'Selecting the frequenies (and thresholds) to be used
        Dim SelectedFrequencyAudiograms As New SortedList(Of BisgaardAudiogramsLimited, SortedList(Of Integer, Integer))

        'Adding keys (audiogram name)
        For Each TempAudiogram In TempAudiograms
            SelectedFrequencyAudiograms.Add(TempAudiogram.Key, New SortedList(Of Integer, Integer))
        Next

        'Adding values (audiogram thresholds)
        For i = 0 To fs.Length - 1

            'Adding only frequencies present in the Frequencies input argument
            If Audiogram.Keys.Contains(fs(i)) Then

                'Adding all threshold values
                For Each TempAudiogram In TempAudiograms
                    SelectedFrequencyAudiograms(TempAudiogram.Key).Add(fs(i), TempAudiogram.Value(i))
                Next
            End If
        Next

        'Calculating the RMS-error to each audiogram type
        Dim TypeRmseList As New SortedList(Of BisgaardAudiogramsLimited, Double)

        'Getting the input audiogram thresholds
        Dim InputAudiogramThresholds = Audiogram.Values

        For Each SelectedFrequencyAudiogram In SelectedFrequencyAudiograms

            'Getting the Bisgaard prototype audiogram thresholds
            Dim PrototypeAudiogramThresholds = SelectedFrequencyAudiogram.Value.Values

            'Calculating squared error
            Dim SquareList As New List(Of Double)
            For i = 0 To InputAudiogramThresholds.Count - 1
                SquareList.Add((InputAudiogramThresholds(i) - PrototypeAudiogramThresholds(i)) ^ 2)
            Next

            'Calculating RMSE
            Dim RMSE = Math.Sqrt(SquareList.Average)

            'Adding the RMSE
            TypeRmseList.Add(SelectedFrequencyAudiogram.Key, RMSE)

        Next

        'Selecting the audiogram type with the lowest RMSE
        Dim LowestRmseKvp = TypeRmseList.OrderBy(Function(kvp) kvp.Value).First()
        Dim SelectedAudiogramType As BisgaardAudiogramsLimited = LowestRmseKvp.Key
        Dim LowestRmseValue As Double = LowestRmseKvp.Value

        'Determining categorical fit tho the selected audiogram type
        Dim FitCategory As BisgaardAudiogramsLimitedFit
        If LowestRmseValue < GoodFitLimit Then
            FitCategory = BisgaardAudiogramsLimitedFit.Good
        ElseIf LowestRmseValue < MediumFitLimit Then
            FitCategory = BisgaardAudiogramsLimitedFit.Medium
        Else
            FitCategory = BisgaardAudiogramsLimitedFit.Poor
        End If

        Return New Tuple(Of BisgaardAudiogramsLimited, BisgaardAudiogramsLimitedFit, Double)(SelectedAudiogramType, FitCategory, LowestRmseValue)

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
        Dim ColumnWidth1 As Integer = 8
        Dim ColumnWidth2 As Integer = 12
        Dim ColumnWidth3 As Integer = 12
        Dim ColumnWidth4 As Integer = 18

        AudiogramList.Add("Side".PadRight(ColumnWidth1) &
                          "Frequency".PadRight(ColumnWidth2) &
                          "Threshold".PadRight(ColumnWidth3) &
                          "ThresholdStatus".PadRight(ColumnWidth4))

        For Each SubTest In SubTests
            AudiogramList.Add(SubTest.Side.ToString.PadRight(ColumnWidth1) &
                              SubTest.Frequency.ToString.PadRight(ColumnWidth2) &
                              SubTest.Threshold.ToString.PadRight(ColumnWidth3) &
                              SubTest.ThresholdStatus.ToString.PadRight(ColumnWidth4))

        Next

        AudiogramList.Add("")
        AudiogramList.Add(ApproximateBisgaardType())

        AudiogramList.Add("Test-retest difference (1 kHz): " & TestRetestDifference & " dB HL")

        Dim OutputPath = IO.Path.Combine(SharedSpeechTestObjects.TestResultsRootFolder, UoPta.FilePathRepresentation)
        Dim OutputFilename = UoPta.FilePathRepresentation & "_AudiogramData_" & SharedSpeechTestObjects.CurrentParticipantID

        Utils.SendInfoToLog(String.Join(vbCrLf, AudiogramList), OutputFilename, OutputPath, False, True, False, True, True)

        'Also storing the results in ResultSummary 
        ResultSummary = String.Join(vbCrLf, AudiogramList)

    End Sub

End Class