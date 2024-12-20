Imports STFN.Audio

Public Class SpeechAudiometryTest
    Inherits SpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "SpeechAudiometry"


    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub


    Public Sub ApplyTestSpecificSettings()
        'Add test specific settings here

    End Sub



    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)
        'Throw New NotImplementedException()
        Return New Tuple(Of Boolean, String)(False, "")
    End Function

    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies
        'Throw New NotImplementedException()
        Return New SpeechTestReplies
    End Function

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        'Throw New NotImplementedException()
    End Sub

    Public Overrides Sub FinalizeTest()
        'Throw New NotImplementedException()
    End Sub

    Public Overrides Function GetResultStringForGui() As String
        'Throw New NotImplementedException()
        Return ""
    End Function

    Public Overrides Function GetTestResultsExportString() As String
        'Throw New NotImplementedException()
        Return ""
    End Function

    Public Overrides Function GetTestTrialResultExportString() As String
        'Throw New NotImplementedException()
        Return ""
    End Function

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Sound, String)
        'Throw New NotImplementedException()
        Return New Tuple(Of Sound, String)(New Sound(New Formats.WaveFormat(48000, 32, 1)), "")
    End Function

End Class



