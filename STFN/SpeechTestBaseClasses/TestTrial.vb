Public Class TrialHistory
    Inherits List(Of TestTrial)

    Public Sub Shuffle(Randomizer As Random)
        Dim SampleOrder = Utils.SampleWithoutReplacement(Me.Count, 0, Me.Count, Randomizer)
        Dim TempList As New List(Of TestTrial)
        For Each RandomIndex In SampleOrder
            TempList.Add(Me(RandomIndex))
        Next
        Me.Clear()
        Me.AddRange(TempList)
    End Sub

End Class

Public Class TestTrial

    ''' <summary>
    ''' An unsigned integer value which can be used to store the test block to which the current trial belongs. While test stages should primarily differ in the applied protocol rules, test blocks should primarily differ in tested content.
    ''' </summary>
    Public TestBlock As UInteger

    ''' <summary>
    ''' An unsigned integer value which can be used to store the test stage to which the current trial belongs. While test blocks should primarily differ in tested content, test stages should primarily differ in the protocol rules applied.
    ''' </summary>
    Public TestStage As UInteger

    Public SpeechMaterialComponent As SpeechMaterialComponent

    ''' <summary>
    ''' A list specifying what is to happen at different timepoints starting from the launch of the test trial
    ''' </summary>
    Public TrialEventList As List(Of ResponseViewEvents.ResponseViewEvent)

    Public Sound As Audio.Sound

    ''' <summary>
    ''' Indicates the number of correctly responded tasks.
    ''' </summary>
    Public ScoreList As New List(Of Integer)

    ''' <summary>
    ''' Indicates if the trial as a whole was correct or not.
    ''' </summary>
    ''' <returns></returns>
    Public Property IsCorrect As Boolean


    ''' <summary>
    ''' Indicate the number of presented tasks.
    ''' </summary>
    Public Tasks As UInteger

    Public Function GetProportionTasksCorrect() As Decimal
        If Tasks > 0 And ScoreList.Count > 0 Then
            Return ScoreList.Sum / Tasks
        Else
            Return 0
        End If
    End Function

    ''' <summary>
    ''' A matrix holding response alternatives in lists. While a test item with a single set of response alternatives (one dimension) should only use one list, while matrix tests should use several lists.
    ''' </summary>
    Public ResponseAlternativeSpellings As List(Of List(Of SpeechTestResponseAlternative))

    Public TimedEventsList As New List(Of Tuple(Of TimedTrialEvents, DateTime))

    Public Enum TimedTrialEvents
        TrialStarted 'Registered in SpeechTestView
        SoundStartedPlay 'Registered in SpeechTestView
        LinguisticSoundStarted 'Registered in SpeechTestView, but requires LinguisticSoundStimulusStartTime to be set by each test
        LinguisticSoundEnded 'Registered in SpeechTestView, but requires LinguisticSoundStimulusStartTime and LinguisticSoundStimulusDuration to be set by each test
        VisualSoundSourcesShown 'Registered in SpeechTestView
        VisualQueShown 'Registered in SpeechTestView
        VisualQueHidden 'Registered in SpeechTestView
        ResponseAlternativesShown 'Registered in SpeechTestView
        ParticipantResponded 'Registered in SpeechTestView
        TestAdministratorCorrectedResponse 'Registered in SpeechTestView, on call from the response view for free recall
        TestAdministratorPressedNextTrial 'Registered in SpeechTestView
        'TestAdministratorUpdatedPreviuosResponse 'Registered in SpeechTestView
        ResponseTimeWasOut 'Registered in SpeechTestView
        SoundStopped 'Registered in SpeechTestView, This will only happen on pause, or stop, etc and not every trial.
        MessageShown 'Registered in SpeechTestView
        PauseMessageShown 'Registered in SpeechTestView
    End Enum

    ''' <summary>
    ''' Should hold the start time of the linguistic test stimulus, related to the start of the trial sound, in milliseconds
    ''' </summary>
    ''' <returns></returns>
    Public Property LinguisticSoundStimulusStartTime As Double

    ''' <summary>
    ''' Should hold the duration of the linguistic test stimulus, in milliseconds
    ''' </summary>
    ''' <returns></returns>
    Public Property LinguisticSoundStimulusDuration As Double

    Public Function GetTimedEventsString() As String

        Dim Output As New List(Of String)

        'Getting the trial start time
        Dim TrialStartTime As DateTime = Nothing
        For Each TimedEvent In TimedEventsList
            If TimedEvent.Item1 = TimedTrialEvents.TrialStarted Then
                TrialStartTime = TimedEvent.Item2

                Output.Add(TimedEvent.Item1.ToString & ": " & TimedEvent.Item2.ToString())

                Exit For
            End If
        Next

        For Each TimedEvent In TimedEventsList
            If TimedEvent.Item1 = TimedTrialEvents.TrialStarted Then Continue For

            'Calculating the time span relative to trial start
            Dim CurrentTimeSpan = TimedEvent.Item2 - TrialStartTime
            Output.Add(TimedEvent.Item1.ToString & ": " & CurrentTimeSpan.TotalMilliseconds)
        Next

        Return String.Join("|", Output)

    End Function

End Class


