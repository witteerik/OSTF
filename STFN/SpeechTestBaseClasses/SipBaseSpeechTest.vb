Imports STFN.Audio
Imports STFN.SipTest
Imports STFN.Utils

Public MustInherit Class SipBaseSpeechTest
    Inherits SpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "SipTest"


    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub


    Public Sub ApplyTestSpecificSettings()
        'Add test specific settings common for all SiP tests here

        AvailableFixedResponseAlternativeCounts = New List(Of Integer) From {3}

        'Allows two decimal points for the reference level
        ReferenceLevel_StepSize = 0.01

        ReferenceLevel = 68.34
        MinimumReferenceLevel = 50
        MaximumReferenceLevel = 90

        'MinimumLevel_Targets = 0
        'MaximumLevel_Targets = 80
        'MinimumLevel_Maskers = 0
        'MaximumLevel_Maskers = 80
        'MinimumLevel_Background = 0
        'MaximumLevel_Background = 80
        'MinimumLevel_ContralateralMaskers = 0
        'MaximumLevel_ContralateralMaskers = 80

        SoundOverlapDuration = 0.5

    End Sub

    <ExludeFromPropertyListing>
    Public Overrides ReadOnly Property ShowGuiChoice_TargetSNRLevel As Boolean = True

    <ExludeFromPropertyListing>
    Public Overrides ReadOnly Property ShowGuiChoice_TargetLevel As Boolean = False

    <ExludeFromPropertyListing>
    Public Overrides ReadOnly Property ShowGuiChoice_MaskingLevel As Boolean = False

    <ExludeFromPropertyListing>
    Public Overrides ReadOnly Property ShowGuiChoice_BackgroundLevel As Boolean = False


    <ExludeFromPropertyListing>
    Public Overrides ReadOnly Property TargetSNRTitle As String
        Get
            Select Case GuiLanguage
                Case Utils.Constants.Languages.Swedish
                    Return "PNR (dB)"
                Case Else
                    Return "PNR (dB)"
            End Select
        End Get
    End Property


    Protected CurrentSipTestMeasurement As SipMeasurement

    Public SelectedSoundPropagationType As SoundPropagationTypes = SoundPropagationTypes.SimulatedSoundField

    Protected SelectedTestparadigm As Testparadigm = Testparadigm.Quick

    Protected MinimumStimulusOnsetTime As Double = 0.3
    Protected MaximumStimulusOnsetTime As Double = 0.8
    Protected TrialSoundMaxDuration As Double = 11
    Protected UseBackgroundSpeech As Boolean = False
    Protected MaximumResponseTime As Double = 4
    Protected PretestSoundDuration As Double = 5
    Protected UseVisualQue As Boolean = False
    Protected ResponseAlternativeDelay As Double = 0.5
    Protected ShowTestSide As Boolean = True
    Protected ShowResponseAlternativePositionsTime As Double = 0.1
    Protected DirectionalSimulationSet As String = "ARC - Harcellen - HATS - SiP"

    Public SipTestMode As SiPTestModes = SiPTestModes.Binaural

    Public Enum SiPTestModes
        Directional
        BMLD
        Binaural
    End Enum


    Public MustOverride Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)


    Protected MustOverride Sub PrepareNextTrial(ByVal NextTaskInstruction As TestProtocol.NextTaskInstruction)
    Protected MustOverride Sub InitiateTestByPlayingSound()


    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        If e IsNot Nothing Then

            'This is an incoming test trial response

            'Corrects the trial response, based on the given response

            'Resets the CurrentTestTrial.ScoreList
            'And also storing SiP-test type data
            CurrentTestTrial.ScoreList = New List(Of Integer)
            Select Case e.LinguisticResponses(0)
                Case CurrentTestTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling")
                    CurrentTestTrial.ScoreList.Add(1)
                    DirectCast(CurrentTestTrial, SipTrial).Result = PossibleResults.Correct
                    DirectCast(CurrentTestTrial, SipTrial).IsCorrect = True

                Case ""
                    CurrentTestTrial.ScoreList.Add(0)
                    DirectCast(CurrentTestTrial, SipTrial).Result = PossibleResults.Missing

                    'Randomizing IsCorrect with a 1/3 chance for True
                    Dim ChanceList As New List(Of Boolean) From {True, False, False}
                    Dim RandomIndex As Integer = Randomizer.Next(ChanceList.Count)
                    DirectCast(CurrentTestTrial, SipTrial).IsCorrect = ChanceList(RandomIndex)

                Case Else
                    CurrentTestTrial.ScoreList.Add(0)
                    DirectCast(CurrentTestTrial, SipTrial).Result = PossibleResults.Incorrect
                    DirectCast(CurrentTestTrial, SipTrial).IsCorrect = False

            End Select

            DirectCast(CurrentTestTrial, SipTrial).Response = e.LinguisticResponses(0)

            'Moving to trial history
            CurrentSipTestMeasurement.MoveTrialToHistory(CurrentTestTrial)

            'Taking a dump of the SpeechTest
            CurrentTestTrial.SpeechTestPropertyDump = Utils.Logging.ListObjectPropertyValues(Me.GetType, Me)


        Else
            'Nothing to correct (this should be the start of a new test)
            'Playing initial sound, and premixing trials
            InitiateTestByPlayingSound()

        End If

        'TODO: We must store the responses and response times!!!

        'Calculating the speech level
        'Dim ProtocolReply = SelectedTestProtocol.NewResponse(ObservedTrials)
        Dim ProtocolReply = New TestProtocol.NextTaskInstruction With {.Decision = SpeechTestReplies.GotoNextTrial}

        If CurrentSipTestMeasurement.PlannedTrials.Count = 0 Then
            'Test is completed
            Return SpeechTestReplies.TestIsCompleted
        End If

        'Preparing next trial if needed
        If ProtocolReply.Decision = SpeechTestReplies.GotoNextTrial Then
            PrepareNextTrial(ProtocolReply)
        End If

        Return ProtocolReply.Decision

    End Function


    Public Overrides Function GetObservedTestTrials() As IEnumerable(Of TestTrial)
        Return CurrentSipTestMeasurement.ObservedTrials
    End Function

    Public MustOverride Overrides Function GetSelectedExportVariables() As List(Of String)


    Public MustOverride Overrides Function GetResultStringForGui() As String




    ''' <summary>
    ''' This method can be called by the backend in order to display a message box message to the user.
    ''' </summary>
    ''' <param name="Message"></param>
    Protected Sub ShowMessageBox(Message As String, Optional ByVal Title As String = "")

        If Title = "" Then
            Select Case GuiLanguage
                Case Utils.Constants.Languages.Swedish
                    Title = "SiP-testet"
                Case Else
                    Title = "SiP-test"
            End Select
        End If

        Messager.MsgBox(Message, MsgBoxStyle.Information, Title)

    End Sub

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Sound, String)
        'This is not used in the SiP-test
        Return Nothing
    End Function

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        'This is not used in the SiP-test
    End Sub

    Public Overrides Sub FinalizeTestAheadOfTime()

        If TestProtocol IsNot Nothing Then
            TestProtocol.AbortAheadOfTime(GetObservedTestTrials)
        End If

    End Sub

    Public Overrides Function GetProgress() As ProgressInfo

        If CurrentSipTestMeasurement IsNot Nothing Then

            Dim NewProgressInfo As New ProgressInfo
            NewProgressInfo.Value = GetObservedTestTrials.Count + 1 ' Adds one to signal started trials
            NewProgressInfo.Maximum = GetObservedTestTrials.Count + CurrentSipTestMeasurement.PlannedTrials.Count
            Return NewProgressInfo

        End If

        Return Nothing

    End Function


End Class