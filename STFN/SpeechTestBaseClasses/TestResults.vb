Public Class TestResults

    Public ReadOnly TestResultType As TestResultTypes

    Public Enum TestResultTypes
        SRT
        WRS
    End Enum

    Public SpeechRecognitionThreshold As Double = Double.NaN
    Public AdaptiveLevelThreshold As Double = Double.NaN
    Public ProportionCorrect As Double = Double.NaN

    Public TestStageSeries As List(Of String)
    Public AdaptiveLevelSeries As List(Of Double)
    Public SpeechLevelSeries As List(Of Double)
    Public MaskerLevelSeries As List(Of Double)
    Public ContralateralMaskerLevelSeries As List(Of Double)
    Public SNRLevelSeries As List(Of Double)
    Public ScoreSeries As List(Of String)
    Public ProportionCorrectSeries As List(Of String)
    Public Progress As List(Of Integer)
    Public ProgressMax As List(Of Integer)
    Public TrialStringComment As List(Of String)

    Public Sub New(ByVal TestResultType As TestResultTypes)
        Me.TestResultType = TestResultType
    End Sub

    Public Function GetFormattedTestResultsSummaryString() As String

        Dim ResultsRowList = New List(Of String)

        Select Case TestResultType
            Case TestResultTypes.SRT
                ResultsRowList.Add("Speech recognition threshold: " & vbCrLf & Math.Round(SpeechRecognitionThreshold) & " dB SPL")
                ResultsRowList.Add("Adaptive recognition threshold: " & vbCrLf & Math.Round(AdaptiveLevelThreshold) & " dB SPL")

                If TestStageSeries IsNot Nothing Then ResultsRowList.Add("Test stage:" & vbCrLf & String.Join(vbTab, TestStageSeries))
                If AdaptiveLevelSeries IsNot Nothing Then ResultsRowList.Add("Adaptive levels:" & vbCrLf & String.Join(vbTab, AdaptiveLevelSeries))
                If SpeechLevelSeries IsNot Nothing Then ResultsRowList.Add("Speech levels:" & vbCrLf & String.Join(vbTab, SpeechLevelSeries))
                If MaskerLevelSeries IsNot Nothing Then ResultsRowList.Add("Masker levels:" & vbCrLf & String.Join(vbTab, MaskerLevelSeries))
                If ContralateralMaskerLevelSeries IsNot Nothing Then ResultsRowList.Add("Contralateral masker levels:" & vbCrLf & String.Join(vbTab, ContralateralMaskerLevelSeries))
                If SNRLevelSeries IsNot Nothing Then ResultsRowList.Add("SNR levels:" & vbCrLf & String.Join(vbTab, SNRLevelSeries))
                If ScoreSeries IsNot Nothing Then ResultsRowList.Add("Trial score:" & vbCrLf & String.Join(vbTab, ScoreSeries))
                If ProportionCorrectSeries IsNot Nothing Then ResultsRowList.Add("Proportion correct:" & vbCrLf & String.Join(vbTab, ProportionCorrectSeries))

            Case TestResultTypes.WRS

                If Progress IsNot Nothing And ProgressMax IsNot Nothing Then
                    If Progress.Count > 0 And ProgressMax.Count > 0 Then
                        ResultsRowList.Add("Word: " & Progress.Last & " / " & ProgressMax.Last)
                    End If
                End If

                If TrialStringComment IsNot Nothing Then
                    If TrialStringComment.Count > 0 Then ResultsRowList.Add(TrialStringComment.Last)
                End If

                If Double.IsNaN(ProportionCorrect) = False Then ResultsRowList.Add("Word recognition score: " & Math.Round(Math.Round(100 * ProportionCorrect)) & " % correct")
                If SpeechLevelSeries.Count > 0 Then ResultsRowList.Add("Speech level: " & Math.Round(SpeechLevelSeries.Last) & " dB SPL")
                'If SNRLevelSeries.Count > 0 Then ResultsRowList.Add("Test SNR: " & vbCrLf & Math.Round(SNRLevelSeries.Last) & " dB SPL")
                'If MaskerLevelSeries.Count > 0 Then ResultsRowList.Add("Masking noise level: " & vbCrLf & Math.Round(MaskerLevelSeries.Last) & " dB SPL")
                If ContralateralMaskerLevelSeries.Count > 0 Then ResultsRowList.Add("Contralateral masking level: " & Math.Round(ContralateralMaskerLevelSeries.Last) & " dB SPL")

                'If TestStageSeries IsNot Nothing Then ResultsRowList.Add("Test stage:" & vbCrLf & String.Join(vbTab, TestStageSeries))
                'If AdaptiveLevelSeries IsNot Nothing Then ResultsRowList.Add("Adaptive levels:" & vbCrLf & String.Join(vbTab, AdaptiveLevelSeries))
                'If SpeechLevelSeries IsNot Nothing Then ResultsRowList.Add("Speech levels:" & vbCrLf & String.Join(vbTab, SpeechLevelSeries))
                'If MaskerLevelSeries IsNot Nothing Then ResultsRowList.Add("Masker levels:" & vbCrLf & String.Join(vbTab, MaskerLevelSeries))
                'If ContralateralMaskerLevelSeries IsNot Nothing Then ResultsRowList.Add("Contralateral masker levels:" & vbCrLf & String.Join(vbTab, ContralateralMaskerLevelSeries))
                'If SNRLevelSeries IsNot Nothing Then ResultsRowList.Add("SNR levels:" & vbCrLf & String.Join(vbTab, SNRLevelSeries))
                'If ScoreSeries IsNot Nothing Then ResultsRowList.Add("Trial score:" & vbCrLf & String.Join(vbTab, ScoreSeries))
                'If ProportionCorrectSeries IsNot Nothing Then ResultsRowList.Add("Proportion correct:" & vbCrLf & String.Join(vbTab, ProportionCorrectSeries))

            Case Else

        End Select


        Return String.Join(vbCrLf, ResultsRowList)

    End Function

End Class

