Public Class SipSpeechTest

    Inherits SpeechTest




    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

    End Sub


    Public Overrides Function InitializeCurrentTest() As Boolean
        'Not yet used in the current speech test
        Return True
    End Function

    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        Throw New NotImplementedException

    End Function


    Private Function GetNextTrial() As TestTrial

        Dim TestWords = SpeechMaterial.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)

        Dim testSound = TestWords(1).GetSound(GetAvailableMediasets(0), 0, 1, , , , , False, False, False, , , False)

        Dim Output As TestTrial = New TestTrial
        Output.TrialEventList = New List(Of ResponseViewEvent)
        Output.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 500, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        Output.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 2000, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        'Output.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1200})
        'Output.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1500})
        'Output.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 3000})

        Output.Sound = testSound

        'This is the correct player:
        'SoundPlayer.SwapOutputSounds(testSound)

        Return Output

    End Function



    Public Overrides Function SaveResults() As Boolean
        'Throw New NotImplementedException()
        Return True
    End Function

    Public Overrides Function GetResults() As Object
        ' Throw New NotImplementedException()
        Return Nothing
    End Function



End Class