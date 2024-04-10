Public Class FixedLengthSpeechInNoise_WithPreTestLevelAdjustment_TestProtocol
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
            If CurrentTestStage < 2 Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(value As Boolean)
            'Any value set is ignored
        End Set
    End Property

    Public Overrides Sub FinalizeProtocol(ByRef TrialHistory As TrialHistory)
        Throw New NotImplementedException()
    End Sub

    Public Overrides Function GetPatientInstructions() As String
        Throw New NotImplementedException()
    End Function

    Public Overrides Function GetSuggestedStartlevel(Optional ReferenceValue As Double? = Nothing) As Double
        Throw New NotImplementedException()
    End Function



    Private TestLength As UInteger

    Private CurrentTestStage As UInteger ' 0 and 1= Level adjustment stage, 2 = test stage
    Private AdaptiveStepSize As Double = 5
    Private NextAdaptiveLevel As Double = 65

    Public Overrides Function InitializeProtocol(ByRef InitialTaskInstruction As NextTaskInstruction) As Boolean

        If InitialTaskInstruction.TestLength > 0 Then
            TestLength = InitialTaskInstruction.TestLength
        Else
            MsgBox("Test length cannot be zero in the currently selected test protocol.")
            Return False
        End If

        If InitialTaskInstruction.AdaptiveValue.HasValue Then
            NextAdaptiveLevel = InitialTaskInstruction.AdaptiveValue
        Else
            MsgBox("An initial speech level value has to be set in the currently selected test protocol.")
            Return False
        End If

        CurrentTestStage = 0

        Return True

    End Function

    Public Overrides Function NewResponse(ByRef TrialHistory As TrialHistory) As NextTaskInstruction

        If TrialHistory.Count = 0 Then
            'This is the start of the test, returns the initial settings
            Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .TestStage = CurrentTestStage, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
        End If

        If IsInPretestMode Then
            Select Case DirectCast(TrialHistory.Last, LevelAdjustmentTrial).LevelRating
                Case LevelAdjustmentTrial.LevelRatings.TooSoft

                    NextAdaptiveLevel += AdaptiveStepSize
                    Return New NextTaskInstruction With {.Decision = SpeechTest.SpeechTestReplies.GotoNextTrial, .AdaptiveValue = NextAdaptiveLevel}

                Case LevelAdjustmentTrial.LevelRatings.Good

                    ' The listener has to select 'Good' twice before the testing starts
                    CurrentTestStage += 1
                    Return New NextTaskInstruction With {.Decision = SpeechTest.SpeechTestReplies.GotoNextTrial, .AdaptiveValue = NextAdaptiveLevel}

                Case LevelAdjustmentTrial.LevelRatings.TooLoud

                    NextAdaptiveLevel += AdaptiveStepSize
                    Return New NextTaskInstruction With {.Decision = SpeechTest.SpeechTestReplies.GotoNextTrial, .AdaptiveValue = NextAdaptiveLevel}

                Case Else
                    Throw New Exception("Level rating. This is likely a bug!")
            End Select

        Else

            'Main test stage
            If TrialHistory.Count < TestLength Then
                Return New NextTaskInstruction With {.Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
            Else
                Return New NextTaskInstruction With {.Decision = SpeechTest.SpeechTestReplies.TestIsCompleted}
            End If
        End If

    End Function

    Public Overrides Function GetFinalResult() As Double?
        Throw New NotImplementedException()
    End Function
End Class