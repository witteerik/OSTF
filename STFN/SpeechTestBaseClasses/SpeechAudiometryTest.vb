Imports STFN.Audio

Public MustInherit Class SpeechAudiometryTest
    Inherits SpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "SpeechAudiometry"


    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub


    Public Sub ApplyTestSpecificSettings()
        'Add test specific settings here

    End Sub

    Protected ObservedTrials As TrialHistory


    Public MustOverride Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

    Public MustOverride Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

    Public MustOverride Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)

    Public MustOverride Overrides Sub FinalizeTest()

    Public MustOverride Overrides Function GetResultStringForGui() As String

    Public MustOverride Overrides Function GetTestResultsExportString() As String

    '    Public Overrides Function GetTestTrialResultExportString(T As Type, Obj As Object) As String
    Public Overrides Function GetTestTrialResultExportString() As String

        If ObservedTrials.Count = 0 Then Return ""

        Dim ExportStringList As New List(Of String)

        Dim ProtocolThreshold = TestProtocol.GetFinalResult()

        'Exporting only the current trial (last added to ObservedTrials)
        Dim TestTrialIndex As Integer = ObservedTrials.Count - 1

        'Adding column headings on the first row
        If TestTrialIndex = 0 Then
            ExportStringList.Add("TrialIndex" & vbTab & ObservedTrials.Last.TestResultColumnHeadings & vbTab & "SRT" & vbTab) '& ExportColumnHeadings(T))
        End If

        'Adding trial data 
        If ProtocolThreshold.HasValue = False Then
            ExportStringList.Add(TestTrialIndex & vbTab & ObservedTrials.Last.TestResultAsTextRow & vbTab & "SRT not established" & vbTab) '& ExportObjectPropertiesAsTextRow(T, Obj))
        Else
            ExportStringList.Add(TestTrialIndex & vbTab & ObservedTrials.Last.TestResultAsTextRow & vbTab & ProtocolThreshold & vbTab) '& ExportObjectPropertiesAsTextRow(T, Obj))
        End If

        Return String.Join(vbCrLf, ExportStringList)

    End Function


    Public MustOverride Overrides Function CreatePreTestStimulus() As Tuple(Of Sound, String)

End Class



