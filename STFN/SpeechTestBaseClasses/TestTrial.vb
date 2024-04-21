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

End Class


