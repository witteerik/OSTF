Public Class FixedLengthWordsInNoise_WithPreTestLevelAdjustment_TestProtocol
    Inherits TestProtocol

    Public Overrides ReadOnly Property Name As String
        Get
            Return "I Hear - Protokoll B2"
        End Get
    End Property

    Public Overrides ReadOnly Property Information As String
        Get
            Return "I Hear - Protokoll B2"
        End Get
    End Property

    Public Overrides Property StoppingCriterium As StoppingCriteria
        Get
            Return StoppingCriteria.TrialCount
        End Get
        Set(value As StoppingCriteria)
            Throw New Exception("The current test protocol does not allow modifying the stopping criterium.")
        End Set
    End Property

    Public Overrides Property IsInPretestMode As Boolean
        Get
            Return False
        End Get
        Set(value As Boolean)
            'Any value set is ignored
        End Set
    End Property


    Public Overrides Function GetPatientInstructions() As String
        Throw New NotImplementedException()
    End Function

    Public Overrides Function GetSuggestedStartlevel(Optional ReferenceValue As Double? = Nothing) As Double
        Throw New NotImplementedException()
    End Function

    Private TestLength As UInteger

    Public Overrides Function InitializeProtocol(ByRef InitialTaskInstruction As NextTaskInstruction) As Boolean

        If InitialTaskInstruction.TestLength > 0 Then
            TestLength = InitialTaskInstruction.TestLength
        Else
            MsgBox("Test length cannot be zero in the currently selected test protocol.")
            Return False
        End If

        Return True

    End Function

    ''' <summary>
    ''' Returns the number of trials remaining or -1 if this is not possible to determine.
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function TotalTrialCount() As Integer
        Return TestLength
    End Function

    Public Overrides Function NewResponse(ByRef TrialHistory As TestTrialCollection) As NextTaskInstruction

        If TrialHistory.Count = 0 Then
            'This is the start of the test, returns the initial settings
            Return New NextTaskInstruction With {.Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
        End If

        If TrialHistory.Count < TestLength Then
            Return New NextTaskInstruction With {.Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
        Else

            'Finalizing the protocol
            FinalizeProtocol(TrialHistory)

            'And marks the protocol as completed
            Return New NextTaskInstruction With {.Decision = SpeechTest.SpeechTestReplies.TestIsCompleted}
        End If

    End Function

    Private FinalWordRecognition As Double? = Nothing

    Public Overrides Function GetFinalResultType() As String
        Return "WRS"
    End Function

    Public Overrides Function GetFinalResultValue() As Double?

        If FinalWordRecognition IsNot Nothing Then
            Return FinalWordRecognition
        Else
            Return Nothing
        End If

    End Function

    Private Sub FinalizeProtocol(ByRef TrialHistory As TestTrialCollection)

        Dim ScoreList As New List(Of Double)
        For Each Trial In TrialHistory
            ScoreList.Add(Trial.GetProportionTasksCorrect)
        Next
        If ScoreList.Count > 0 Then
            FinalWordRecognition = ScoreList.Average
        Else
            FinalWordRecognition = Double.NaN
        End If

    End Sub

    Public Overrides Sub AbortAheadOfTime(ByRef TrialHistory As TestTrialCollection)
        FinalizeProtocol(TrialHistory)
    End Sub

    Public Overrides Function GetCurrentAdaptiveValue() As Double?
        Return Nothing
    End Function

    Public Overrides Sub OverrideCurrentAdaptiveValue(ByVal NewValue As Double)
        'Any value is ignored since the protocol is not adaptive
    End Sub


End Class