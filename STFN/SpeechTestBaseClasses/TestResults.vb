Public Class TestResults_NOTUSE

    Public ExportLines As New List(Of String)

    Public ReadOnly TestResultType As TestResultTypes

    Public Enum TestResultTypes
        SRT
        WRS
        QSiP
        IHPB1
        IHPB3
        IHPB4
        IHPB7
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

    Public FormattedTrialResultsHeadings As String = ""
    Public FormattedTrialResults As List(Of String)

    Public TestResultSummaryLines As List(Of String)

    Public Sub New(ByVal TestResultType As TestResultTypes)
        Me.TestResultType = TestResultType
    End Sub

    Public Function GetFormattedTestResultsSummaryString() As String

        Dim ResultsRowList = New List(Of String)

        Select Case TestResultType
            Case TestResultTypes.SRT
                If Double.IsNaN(SpeechRecognitionThreshold) = False Then
                    ResultsRowList.Add("HTT = " & vbTab & Math.Round(SpeechRecognitionThreshold) & " dB HL")
                Else
                    If SpeechLevelSeries IsNot Nothing Then
                        If SpeechLevelSeries.Count > 0 Then
                            ResultsRowList.Add("Talnivå = " & vbTab & Math.Round(SpeechLevelSeries.Last) & " dB HL")
                        End If
                    End If
                    If ContralateralMaskerLevelSeries IsNot Nothing Then
                        If ContralateralMaskerLevelSeries.Count > 0 Then
                            ResultsRowList.Add("Kontralateral brusnivå = " & vbTab & Math.Round(ContralateralMaskerLevelSeries.Last) & " dB HL")
                        End If
                    End If
                End If

            Case TestResultTypes.WRS

                If Progress IsNot Nothing And ProgressMax IsNot Nothing Then
                    If Progress.Count > 0 And ProgressMax.Count > 0 Then
                        ResultsRowList.Add("Ord " & vbTab & Progress.Last & " av  " & ProgressMax.Last)
                    End If
                End If
                If Double.IsNaN(ProportionCorrect) = False Then ResultsRowList.Add("TP = " & vbTab & Math.Round(Math.Round(100 * ProportionCorrect)) & " % correct")
                If SpeechLevelSeries IsNot Nothing Then
                    If SpeechLevelSeries.Count > 0 Then
                        ResultsRowList.Add("Talnivå = " & vbTab & Math.Round(SpeechLevelSeries.Last) & " dB HL")
                    End If
                End If
                If ContralateralMaskerLevelSeries IsNot Nothing Then
                    If ContralateralMaskerLevelSeries.Count > 0 Then
                        ResultsRowList.Add("Kontralateral brusnivå = " & vbTab & Math.Round(ContralateralMaskerLevelSeries.Last) & " dB HL")
                    End If
                End If

            Case TestResultTypes.QSiP
                If TestResultSummaryLines IsNot Nothing Then ResultsRowList.AddRange(TestResultSummaryLines)

            Case TestResultTypes.IHPB1
                If TestResultSummaryLines IsNot Nothing Then ResultsRowList.AddRange(TestResultSummaryLines)

            Case TestResultTypes.IHPB3
                If TestResultSummaryLines IsNot Nothing Then ResultsRowList.AddRange(TestResultSummaryLines)

            Case TestResultTypes.IHPB4
                If TestResultSummaryLines IsNot Nothing Then ResultsRowList.AddRange(TestResultSummaryLines)

            Case TestResultTypes.IHPB7
                If TestResultSummaryLines IsNot Nothing Then ResultsRowList.AddRange(TestResultSummaryLines)

            Case Else

        End Select


        Return String.Join(vbCrLf, ResultsRowList)

    End Function

    Public Function GetTestResultsExportString() As String

        Return String.Join(vbCrLf, ExportLines)

    End Function


End Class

