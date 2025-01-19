''' <summary>
''' This protocol implements the test procedure described in the Swedish HINT test instructions at https://doi.org/10.17605/OSF.IO/4ZNCK "HINT-LISTOR PÅ SVENSKA–KLINISK ANVÄNDNING" by M. Hällgren (2018-12-14) Linköping University.
''' </summary>
Public Class SrtSwedishHint2018_TestProtocol
    Inherits TestProtocol

    Public Overrides ReadOnly Property Name As String
        Get
            Return "Swedish HINT 2018"
        End Get
    End Property

    Public Overrides ReadOnly Property Information As String
        Get
            Return "This protocol implements the test procedure described in the Swedish HINT test instructions at https://doi.org/10.17605/OSF.IO/4ZNCK 'HINT-LISTOR PÅ SVENSKA–KLINISK ANVÄNDNING' by M. Hällgren (2018-12-14) Linköping University."
        End Get
    End Property

    Public Overrides Function GetPatientInstructions() As String
        Return ""
    End Function

    Public Overrides Function GetSuggestedStartlevel(Optional ReferenceValue As Double? = Nothing) As Double
        Throw New NotImplementedException()
    End Function

    Public Overrides Property StoppingCriterium As StoppingCriteria
        Get
            Return StoppingCriteria.TrialCount
        End Get
        Set(value As StoppingCriteria)
            MsgBox("An attempt was made to change the test protocol stopping criterium. The " & Name & " procedure requires that the stopping criterium is 'TrialCount'. Ignoring the attempt to set the new value.", , "Unsupported stopping criterium.")
        End Set
    End Property


    Private AdaptiveStepSize As Double = 2

    Private NextAdaptiveLevel As Double = 0


    Private FinalThreshold As Double? = Nothing

    Public Overrides Function InitializeProtocol(ByRef InitialTaskInstruction As NextTaskInstruction) As Boolean

        NextAdaptiveLevel = InitialTaskInstruction.AdaptiveValue

        Return True
    End Function

    ''' <summary>
    ''' Returns the number of trials remaining or -1 if this is not possible to determine.
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function TotalTrialCount() As Integer
        If IsInPretestMode = True Then
            Return 10
        Else
            Return 20
        End If
    End Function

    Public Overrides Function NewResponse(ByRef TrialHistory As TestTrialCollection) As NextTaskInstruction

        If TrialHistory.Count = 0 Then
            'This is the start of the test, returns the initial settings
            Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel,
                .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
        End If

        Dim ProportionTasksCorrect = TrialHistory.Last.GetProportionTasksCorrect

        'Determines adaptive change
        If ProportionTasksCorrect = 1 Then
            'Decreasing the level (making the test more difficult)
            NextAdaptiveLevel -= AdaptiveStepSize
        Else
            'Increasing the level (making the test more easy)
            NextAdaptiveLevel += AdaptiveStepSize
        End If

        'Checking if test is complete (presenting max number of trials)
        If TrialHistory.Count >= TotalTrialCount Then

            'Finalizing the protocol
            FinalizeProtocol(TrialHistory)

            'Exits the test
            Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .Decision = SpeechTest.SpeechTestReplies.TestIsCompleted}

        End If

        'Continues the test
        Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}

    End Function


    Private Sub FinalizeProtocol(ByRef TrialHistory As TestTrialCollection)

        If IsInPretestMode = True Then
            'Storing the last value as threshold
            FinalThreshold = TrialHistory.Last.AdaptiveProtocolValue

        Else
            'Calculating threshold
            Dim LevelList As New List(Of Double)
            For i As Integer = 4 To 19
                LevelList.Add(TrialHistory(i).AdaptiveProtocolValue)
            Next

            'And adding the last non-presented trial level
            LevelList.Add(NextAdaptiveLevel)

            'Getting the average
            If LevelList.Count > 0 Then
                FinalThreshold = LevelList.Average
            Else
                FinalThreshold = Double.NaN
            End If
        End If

    End Sub

    Public Overrides Function GetFinalResultType() As String
        Return "SRT"
    End Function

    Public Overrides Function GetFinalResultValue() As Double?

        Return FinalThreshold

    End Function

    Public Overrides Sub AbortAheadOfTime(ByRef TrialHistory As TestTrialCollection)
        'Setting FinalThreshold to NaN, since it's not possible to finalize early
        FinalThreshold = Double.NaN
    End Sub

    Public Overrides Function GetCurrentAdaptiveValue() As Double?
        Return NextAdaptiveLevel
    End Function

End Class