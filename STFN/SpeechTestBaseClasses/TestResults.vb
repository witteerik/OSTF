Public Class TestResults

    Public ReadOnly TestResultType As TestResultTypes

    Public Enum TestResultTypes
        SRT
    End Enum

    Public SpeechRecognitionThreshold As Double
    Public TestStageSeries As List(Of String)
    Public SpeechLevelSeries As List(Of Double)
    Public ScoreSeries As List(Of String)

    Public Sub New(ByVal TestResultType As TestResultTypes)
        Me.TestResultType = TestResultType
    End Sub

    Public Function GetFormattedTestResultsSummaryString() As String

        Dim ResultsRowList = New List(Of String)

        Select Case TestResultType
            Case TestResultTypes.SRT
                ResultsRowList.Add("Speech recognition threshold:" & vbCrLf & Math.Round(SpeechRecognitionThreshold) & " dB SPL")
                ResultsRowList.Add("Test stage:" & vbCrLf & String.Join(vbTab, TestStageSeries))
                ResultsRowList.Add("Speech level:" & vbCrLf & String.Join(vbTab, SpeechLevelSeries))
                ResultsRowList.Add("Trial score:" & vbCrLf & String.Join(vbTab, ScoreSeries))

            Case Else


        End Select

        Return String.Join(vbCrLf, ResultsRowList)

    End Function

End Class

