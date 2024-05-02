Imports System.Reflection

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

Public MustInherit Class TestTrial

    ''' <summary>
    ''' An integer value which can be used to store the test block to which the current trial belongs. While test stages should primarily differ in the applied protocol rules, test blocks should primarily differ in tested content.
    ''' </summary>
    Public Property TestBlock As Integer

    ''' <summary>
    ''' An integer value which can be used to store the test stage to which the current trial belongs. While test blocks should primarily differ in tested content, test stages should primarily differ in the protocol rules applied.
    ''' </summary>
    Public Property TestStage As Integer

    Public Property SpeechMaterialComponent As SpeechMaterialComponent

    Public ReadOnly Property Spelling As String
        Get
            If SpeechMaterialComponent IsNot Nothing Then
                Return SpeechMaterialComponent.GetCategoricalVariableValue("Spelling")
            Else
                Return ""
            End If
        End Get
    End Property

    ''' <summary>
    ''' A list specifying what is to happen at different timepoints starting from the launch of the test trial
    ''' </summary>
    Public TrialEventList As List(Of ResponseViewEvents.ResponseViewEvent)

    Public Sound As Audio.Sound

    Public Property MediaSetName As String

    Public Property UseContralateralNoise As Boolean

    Public Property EfficientContralateralMaskingTerm As Double

    ''' <summary>
    ''' Indicates the number of correctly responded tasks.
    ''' </summary>
    Public ScoreList As New List(Of Integer)

    Public ReadOnly Property ScoreListString As String
        Get
            If ScoreList IsNot Nothing Then
                Return String.Join(", ", ScoreList)
            Else
                Return ""
            End If
        End Get
    End Property


    ''' <summary>
    ''' Indicates if the trial as a whole was correct or not.
    ''' </summary>
    ''' <returns></returns>
    Public Property IsCorrect As Boolean

    ''' <summary>
    ''' Indicate the number of presented tasks.
    ''' </summary>
    Public Property Tasks As UInteger

    Public ReadOnly Property GetProportionTasksCorrect() As Decimal
        Get
            If Tasks > 0 And ScoreList.Count > 0 Then
                Return ScoreList.Sum / Tasks
            Else
                Return 0
            End If
        End Get
    End Property

    ''' <summary>
    ''' A matrix holding response alternatives in lists. While a test item with a single set of response alternatives (one dimension) should only use one list, while matrix tests should use several lists.
    ''' </summary>
    Public ResponseAlternativeSpellings As List(Of List(Of SpeechTestResponseAlternative))

    Public ReadOnly Property ExportedResponseAlternativeSpellings As String
        Get

            Dim OutputList As New List(Of String)
            If ResponseAlternativeSpellings IsNot Nothing Then
                For Each RAL In ResponseAlternativeSpellings
                    If RAL IsNot Nothing Then
                        For Each RA In RAL
                            If RA IsNot Nothing Then
                                If RA.IsVisible = True Then
                                    OutputList.Add(RA.Spelling)
                                End If
                            End If
                        Next
                    End If
                Next
            End If

            If OutputList.Count > 0 Then
                Return String.Join(", ", OutputList)
            Else
                Return ""
            End If

        End Get
    End Property

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

    Public ReadOnly Property GetTimedEventsString() As String
        Get

            Dim Output As New List(Of String)

            If TimedEventsList IsNot Nothing Then

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

                If Output.Count > 0 Then
                    Return String.Join("|", Output)
                Else
                    Return ""
                End If
            Else
                Return ""
            End If

        End Get
    End Property

    Public MustOverride Function TestResultColumnHeadings() As String

    Public MustOverride Function TestResultAsTextRow() As String

    Protected Shared Function BaseClassTestResultColumnHeadings() As List(Of String)

        Dim OutputList As New List(Of String)
        Dim properties As PropertyInfo() = GetType(TestTrial).GetProperties()

        ' Iterating through each property
        For Each [property] As PropertyInfo In properties

            ' Getting the name of the property
            Dim propertyName As String = [property].Name
            OutputList.Add(propertyName)

        Next

        Return OutputList

    End Function

    Protected Function BaseClassTestResultAsTextRow() As List(Of String)

        Dim OutputList As New List(Of String)
        Dim properties As PropertyInfo() = GetType(TestTrial).GetProperties()

        ' Iterating through each property
        For Each [property] As PropertyInfo In properties

            ' Getting the name of the property
            Dim propertyName As String = [property].Name

            ' Getting the value of the property for the current instance 
            Dim propertyValue As Object = [property].GetValue(Me)

            'If TypeOf propertyValue Is String Then
            '    Dim stringValue As String = DirectCast(propertyValue, String)
            'ElseIf TypeOf propertyValue Is Integer Then
            '    Dim intValue As Integer = DirectCast(propertyValue, Integer)
            'Else
            'End If

            If propertyValue IsNot Nothing Then
                OutputList.Add(propertyValue.ToString)
            Else
                OutputList.Add("NotSet")
            End If

        Next

        Return OutputList

    End Function

End Class


