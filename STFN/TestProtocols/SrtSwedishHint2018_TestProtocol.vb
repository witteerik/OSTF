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


    Private LargerAdaptiveStepSize As Double = 2

    Private NextAdaptiveLevel As Double = 0

    Public ReadOnly Property TotalTrialCount As Integer
        Get
            If IsInPretestMode = True Then
                Return 10
            Else
                Return 20
            End If
        End Get
    End Property


    Private FinalAdaptiveThreshold As Double? = Nothing

    Public Overrides Function InitializeProtocol(ByRef InitialTaskInstruction As NextTaskInstruction) As Boolean

        NextAdaptiveLevel = InitialTaskInstruction.AdaptiveValue

        Return True
    End Function


    Public Overrides Function NewResponse(ByRef TrialHistory As TrialHistory) As NextTaskInstruction

        If TrialHistory.Count = 0 Then
            'This is the start of the test, returns the initial settings
            Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel,
                .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}
        End If

        Dim ProportionTasksCorrect = TrialHistory.Last.GetProportionTasksCorrect

        'Determines adaptive change
        Select Case ProportionTasksCorrect
            Case 0.5
                'This only happens when there are multiple tasks
                'Leaving the level onchanged and returns

            Case > 0.5
                'Decreasing the level (making the test more difficult)
                NextAdaptiveLevel -= LargerAdaptiveStepSize

            Case Else
                'Increasing the level (making the test more easy)
                NextAdaptiveLevel += LargerAdaptiveStepSize

        End Select

        'Checking if test is complete (presenting max number of trials)
        If TrialHistory.Count >= TotalTrialCount Then

            'Exits the test
            Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .Decision = SpeechTest.SpeechTestReplies.TestIsCompleted}

        End If

        'Continues the test
        Return New NextTaskInstruction With {.AdaptiveValue = NextAdaptiveLevel, .Decision = SpeechTest.SpeechTestReplies.GotoNextTrial}

    End Function

    Public Overrides Sub FinalizeProtocol(ByRef TrialHistory As TrialHistory)

        If IsInPretestMode = True Then
            'Storing the last value as threshold
            FinalAdaptiveThreshold = DirectCast(TrialHistory.Last, SrtTrial).AdaptiveValue

        Else
            'Calculating threshold
            Dim LevelList As New List(Of Double)
            For i As Integer = 4 To 19
                LevelList.Add(DirectCast(TrialHistory(i), SrtTrial).AdaptiveValue)
            Next

            'And adding the last non-presented trial level
            LevelList.Add(NextAdaptiveLevel)

            'Getting the average
            If LevelList.Count > 0 Then
                FinalAdaptiveThreshold = LevelList.Average
            Else
                FinalAdaptiveThreshold = Double.NaN
            End If
        End If

    End Sub

    Public Overrides Function GetFinalResult() As Double?

        Return FinalAdaptiveThreshold

    End Function


End Class