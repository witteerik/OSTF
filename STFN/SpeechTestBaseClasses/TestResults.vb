Public Class TestResults

    Public ReadOnly TestResultType As TestResultTypes

    Public Enum TestResultTypes
        SRT
    End Enum

    Public SpeechRecognitionThreshold As Double
    Public AdaptiveLevelThreshold As Double
    Public TestStageSeries As List(Of String)
    Public AdaptiveLevelSeries As List(Of Double)
    Public SpeechLevelSeries As List(Of Double)
    Public MaskerLevelSeries As List(Of Double)
    Public SNRLevelSeries As List(Of Double)
    Public ScoreSeries As List(Of String)

    Public Sub New(ByVal TestResultType As TestResultTypes)
        Me.TestResultType = TestResultType
    End Sub

    Public Function GetFormattedTestResultsSummaryString() As String

        Dim ResultsRowList = New List(Of String)

        Select Case TestResultType
            Case TestResultTypes.SRT
                ResultsRowList.Add("Speech recognition threshold:" & vbCrLf & Math.Round(SpeechRecognitionThreshold) & " dB SPL")
                ResultsRowList.Add("Adaptive recognition threshold:" & vbCrLf & Math.Round(AdaptiveLevelThreshold) & " dB SPL")
                ResultsRowList.Add("Test stage:" & vbCrLf & String.Join(vbTab, TestStageSeries))
                ResultsRowList.Add("Adaptive levels:" & vbCrLf & String.Join(vbTab, AdaptiveLevelSeries))
                ResultsRowList.Add("Speech levels:" & vbCrLf & String.Join(vbTab, SpeechLevelSeries))
                ResultsRowList.Add("Masker levels:" & vbCrLf & String.Join(vbTab, MaskerLevelSeries))
                ResultsRowList.Add("SNR levels:" & vbCrLf & String.Join(vbTab, SNRLevelSeries))
                ResultsRowList.Add("Trial score:" & vbCrLf & String.Join(vbTab, ScoreSeries))

            Case Else


        End Select

        Return String.Join(vbCrLf, ResultsRowList)

    End Function

End Class

