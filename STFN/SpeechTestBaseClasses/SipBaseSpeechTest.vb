﻿Imports STFN.Audio
Imports STFN.SipTest

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

    Public Overrides ReadOnly Property ShowGuiChoice_TargetLevel As Boolean = False

    Public Overrides ReadOnly Property ShowGuiChoice_MaskingLevel As Boolean = False

    Public Overrides ReadOnly Property ShowGuiChoice_BackgroundLevel As Boolean = False

    Public Overrides ReadOnly Property ShowGuiChoice_ContralateralMasking As Boolean = False


    Protected CurrentSipTestMeasurement As SipMeasurement

    Public SelectedSoundPropagationType As SoundPropagationTypes = SoundPropagationTypes.SimulatedSoundField

    Protected SelectedTestparadigm As Testparadigm = Testparadigm.Quick

    Protected MinimumStimulusOnsetTime As Double = 0.3
    Protected MaximumStimulusOnsetTime As Double = 0.8
    Protected TrialSoundMaxDuration As Double = 10
    Protected UseBackgroundSpeech As Boolean = False
    Protected MaximumResponseTime As Double = 4
    Protected PretestSoundDuration As Double = 5
    Protected UseVisualQue As Boolean = False
    Protected ResponseAlternativeDelay As Double = 0.5
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

            'Corrects the trial response, based on the given response
            Dim CorrectWordsList As New List(Of String)

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

            'This is an incoming test trial response
            If CurrentTestTrial IsNot Nothing Then

                'Taking a dump of the SpeechTest
                CurrentTestTrial.SpeechTestPropertyDump = Utils.Logging.ListObjectPropertyValues(Me.GetType, Me)

                CurrentSipTestMeasurement.MoveTrialToHistory(CurrentTestTrial)
            End If

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



    Public MustOverride Overrides Function GetResultStringForGui() As String

    Public Overrides Function GetTestResultsExportString() As String

        Dim ExportStringList As New List(Of String)

        For i = 0 To CurrentSipTestMeasurement.ObservedTrials.Count - 1
            If i = 0 Then
                ExportStringList.Add("TrialIndex" & vbTab & CurrentSipTestMeasurement.ObservedTrials(i).TestResultColumnHeadings & vbTab & CurrentSipTestMeasurement.ObservedTrials.Last.ListedSpeechTestPropertyNames)
            End If
            ExportStringList.Add(i & vbTab & CurrentSipTestMeasurement.ObservedTrials(i).TestResultAsTextRow & vbTab & CurrentSipTestMeasurement.ObservedTrials.Last.ListedSpeechTestPropertyValues)
        Next

        Return String.Join(vbCrLf, ExportStringList)

    End Function


    Public Overrides Function GetTestTrialResultExportString() As String

        If CurrentSipTestMeasurement.ObservedTrials.Count = 0 Then Return ""

        Dim ExportStringList As New List(Of String)

        'Exporting only the current trial (last added to ObservedTrials)
        Dim TestTrialIndex As Integer = CurrentSipTestMeasurement.ObservedTrials.Count - 1

        'Adding column headings on the first row
        If TestTrialIndex = 0 Then
            ExportStringList.Add("TrialIndex" & vbTab & CurrentSipTestMeasurement.ObservedTrials.Last.TestResultColumnHeadings & vbTab & CurrentSipTestMeasurement.ObservedTrials.Last.ListedSpeechTestPropertyNames)
        End If

        'Adding trial data 
        ExportStringList.Add(TestTrialIndex & vbTab & CurrentSipTestMeasurement.ObservedTrials.Last.TestResultAsTextRow & vbTab & CurrentSipTestMeasurement.ObservedTrials.Last.ListedSpeechTestPropertyValues)

        Return String.Join(vbCrLf, ExportStringList)

    End Function



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

    Public Overrides Sub FinalizeTest()
        'This is not used in the SiP-test
    End Sub


End Class