Public Class SpeechTestResponseAlternative
    Public Spelling As String = ""
    Public IsScoredItem As Boolean = True
    Public SoundSourceLocation As Audio.SoundScene.SoundSourceLocation
    Public ParentTestTrial As TestTrial
    Public TrialPresentationIndex As Integer = 0
    Public IsVisible As Boolean = True
End Class
