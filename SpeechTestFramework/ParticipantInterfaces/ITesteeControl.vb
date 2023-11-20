Public Interface ITesteeControl
    Sub ShowResponseAlternatives(ByVal ResponseAlternatives As List(Of Tuple(Of String, SpeechTestFramework.Audio.SoundScene.SoundSourceLocation)))
    Sub ShowVisualCue()
    Sub HideVisualCue()
    Sub ResponseTimesOut()
    Sub ResetTestItemPanel()
    Sub UpdateTestFormProgressbar(ByVal Value As Integer, ByVal Maximum As Integer, Optional ByVal Minimum As Integer = 0)
    Sub ShowMessage(ByVal Message As String)

    Event StartedByTestee(sender As Object, e As EventArgs)

    Event ResponseGiven(ByVal Response As String)

End Interface

