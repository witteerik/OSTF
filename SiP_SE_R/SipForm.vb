Public Class SipForm
    Inherits SpeechTestFramework.SipTestGui

    Public Sub New()
        'Loads the Swedish SiP test in research mode
        MyBase.New("Swedish SiP-test", SpeechTestFramework.Utils.Constants.UserTypes.Research, SpeechTestFramework.Utils.Constants.Language.Swedish)

    End Sub

End Class