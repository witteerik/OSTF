Public Class TestResults

    Public ReadOnly TestResultType As TestResultTypes

    Public Enum TestResultTypes
        SRT
        WRS
    End Enum

    Public SpeechRecognitionThreshold As Double
    Public AdaptiveLevelThreshold As Double
    Public ProportionCorrect As Double

    Public TestStageSeries As List(Of String)
    Public AdaptiveLevelSeries As List(Of Double)
    Public SpeechLevelSeries As List(Of Double)
    Public MaskerLevelSeries As List(Of Double)
    Public ContralateralMaskerLevelSeries As List(Of Double)
    Public SNRLevelSeries As List(Of Double)
    Public ScoreSeries As List(Of String)
    Public ProportionCorrectSeries As List(Of String)

    Public Sub New(ByVal TestResultType As TestResultTypes)
        Me.TestResultType = TestResultType
    End Sub

    Public Function GetFormattedTestResultsSummaryString() As String

        Dim ResultsRowList = New List(Of String)

        Select Case TestResultType
            Case TestResultTypes.SRT
                ResultsRowList.Add("Speech recognition threshold: " & vbCrLf & Math.Round(SpeechRecognitionThreshold) & " dB SPL")
                ResultsRowList.Add("Adaptive recognition threshold: " & vbCrLf & Math.Round(AdaptiveLevelThreshold) & " dB SPL")

            Case TestResultTypes.WRS
                ResultsRowList.Add("Word recognition score: " & vbCrLf & Math.Round(Math.Round(100 * ProportionCorrect)) & " dB SPL")
                If SpeechLevelSeries.Count > 0 Then ResultsRowList.Add("Speech level: " & vbCrLf & Math.Round(SpeechLevelSeries.Last) & " dB SPL")
                If SNRLevelSeries.Count > 0 Then ResultsRowList.Add("Test SNR: " & vbCrLf & Math.Round(SNRLevelSeries.Last) & " dB SPL")
                If MaskerLevelSeries.Count > 0 Then ResultsRowList.Add("Masking noise level: " & vbCrLf & Math.Round(MaskerLevelSeries.Last) & " dB SPL")
                If ContralateralMaskerLevelSeries.Count > 0 Then ResultsRowList.Add("Contralateral masking level: " & vbCrLf & Math.Round(ContralateralMaskerLevelSeries.Last) & " dB SPL")

            Case Else

        End Select

        If TestStageSeries IsNot Nothing Then ResultsRowList.Add("Test stage:" & vbCrLf & String.Join(vbTab, TestStageSeries))
        If AdaptiveLevelSeries IsNot Nothing Then ResultsRowList.Add("Adaptive levels:" & vbCrLf & String.Join(vbTab, AdaptiveLevelSeries))
        If SpeechLevelSeries IsNot Nothing Then ResultsRowList.Add("Speech levels:" & vbCrLf & String.Join(vbTab, SpeechLevelSeries))
        If MaskerLevelSeries IsNot Nothing Then ResultsRowList.Add("Masker levels:" & vbCrLf & String.Join(vbTab, MaskerLevelSeries))
        If ContralateralMaskerLevelSeries IsNot Nothing Then ResultsRowList.Add("Masker levels:" & vbCrLf & String.Join(vbTab, ContralateralMaskerLevelSeries))
        If SNRLevelSeries IsNot Nothing Then ResultsRowList.Add("SNR levels:" & vbCrLf & String.Join(vbTab, SNRLevelSeries))
        If ScoreSeries IsNot Nothing Then ResultsRowList.Add("Trial score:" & vbCrLf & String.Join(vbTab, ScoreSeries))
        If ProportionCorrectSeries IsNot Nothing Then ResultsRowList.Add("Proportion correct:" & vbCrLf & String.Join(vbTab, ProportionCorrectSeries))

        Return String.Join(vbCrLf, ResultsRowList)

    End Function

End Class

