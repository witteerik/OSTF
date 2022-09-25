Public Interface ITesteeControl
    Sub ShowResponseAlternatives(ByVal ResponseAlternatives As List(Of String))
    Sub ShowVisualQue()
    Sub HideVisualQue()
    Sub ResponseTimesOut()
    Sub ResetTestItemPanel()
    Sub UpdateTestFormProgressbar(ByVal Value As Integer, ByVal Maximum As Integer, Optional ByVal Minimum As Integer = 0)
    Sub ShowMessage(ByVal Message As String)

    Event StartedByTestee()
    Event ResponseGiven(ByVal Response As String)

End Interface

