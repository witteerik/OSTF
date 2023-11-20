Public Class SrtSpeechTest

    Inherits SpeechTest




    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

    End Sub


    Public Overrides Function PrepareNextTrial() As Boolean

        Dim TestWords = SpeechMaterial.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)

        Dim testSound = TestWords(1).GetSound(GetAvailableMediasets(0), 0, 1, , , , , False, False, False, , , False)

        //OstfBase.InitializeSoundPlayer()

        SoundPlayer.SwapOutputSounds(testSound)



    End Function

    Public Overrides Function StartNextTrial() As Boolean
        Throw New NotImplementedException()
    End Function

    Public Overrides Function HandleResponse(sender As Object, e As ResponseGivenEventArgs) As HandleResponseOutcomes
        Throw New NotImplementedException()
    End Function


End Class