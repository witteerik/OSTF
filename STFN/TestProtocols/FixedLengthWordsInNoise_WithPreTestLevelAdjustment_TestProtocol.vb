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

    Public Overrides Function NewResponse(ByRef TrialHistory As TrialHistory) As NextTaskInstruction

        If TrialHistory.Count = 0 Then
            'This is the start of the test, returns the initial settings
            Return New NextTaskInstruction With {.Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
        End If

        If TrialHistory.Count < TestLength Then
            Return New NextTaskInstruction With {.Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
        Else
            Return New NextTaskInstruction With {.Decision = SpeechTest.SpeechTestReplies.TestIsCompleted}
        End If

    End Function

    Private FinalWordRecognition As Double? = Nothing

    Public Overrides Function GetFinalResult() As Double?

        If FinalWordRecognition IsNot Nothing Then
            Return FinalWordRecognition
        Else
            Return Nothing
        End If

    End Function

    Public Overrides Sub FinalizeProtocol(ByRef TrialHistory As TrialHistory)

        Dim ScoreList As New List(Of Double)
        For Each Trial In TrialHistory
            ScoreList.Add(DirectCast(Trial, WrsTrial).GetProportionTasksCorrect)
        Next
        If ScoreList.Count > 0 Then
            FinalWordRecognition = ScoreList.Average
        Else
            FinalWordRecognition = -1
        End If

    End Sub

End Class