
Namespace SipTest

    <Serializable>
    Public Class SipTestSummary
        Inherits List(Of SummarizedTrial)

        Public ReadOnly Property Description As String
        Public ReadOnly Property AverageScore As Double
            Get
                Return GetAverageScore()
            End Get
        End Property

        Public ReadOnly Property TestLength As Integer
            Get
                Return Me.Count
            End Get
        End Property

        Public Sub New(ByVal Description As String)
            Me.Description = Description
        End Sub

        Public Function PercentCorrect() As String
            Dim LocalAverageScore = AverageScore
            If LocalAverageScore = -1 Then
                Return ""
            Else
                Return Math.Round(100 * LocalAverageScore)
            End If
        End Function

        ''' <summary>
        ''' Returns the average score, counting missing responses as correct every ResponseAlternatives:th time. Returns -1 if no SummarizedTrials exist.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetAverageScore() As Double

            If Me.Count = 0 Then Return -1

            Dim Correct As Integer = 0
            Dim Total As Integer = Me.Count
            For n = 0 To Me.Count - 1
                If Me(n).Result = 1 Then
                    Correct += 1
                ElseIf Me(n).Result = -1 Then
                    If Me(n).ResponseAlternatives > 0 Then
                        If n Mod Me(n).ResponseAlternatives = (Me(n).ResponseAlternatives - 1) Then
                            Correct += 1
                        End If
                    End If
                End If
            Next
            Return Correct / Total
        End Function

        Public Sub CalculateAdjustedSuccessProbabilities()

            Dim LocalAverageScore = GetAverageScore()
            Dim UnadjustedEstimates As Double() = GetEstimatedSuccessProbabilities()
            Dim AdjustedEstimates = SpeechTestFramework.CriticalDifferences.getAdjustedSuccessProbabilities(UnadjustedEstimates, LocalAverageScore, , 1 / 3)
            For n = 0 To Me.Count - 1
                Me(n).AdjustedSuccessProbability = AdjustedEstimates(n)
            Next

        End Sub

        Public Function GetEstimatedSuccessProbabilities() As Double()
            Dim OutputList As New List(Of Double)
            For n = 0 To Me.Count - 1
                OutputList.Add(Me(n).EstimatedSuccessProbability)
            Next
            Return OutputList.ToArray
        End Function

        <Serializable>
        Public Class SummarizedTrial
            Public Property Result As Integer '1=Correct, 0=Incorrect, -1= Missing
            Public Property EstimatedSuccessProbability As Double
            Public Property AdjustedSuccessProbability As Double
            Public Property ResponseAlternatives As Integer

        End Class


    End Class
End Namespace
