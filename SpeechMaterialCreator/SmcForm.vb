Imports SpeechTestFramework.OstfBase

Public Class SmcForm
    Inherits SpeechTestFramework.SpeechMaterialCreator

    Public Sub New()
        'Initializing the OSTF 
        SpeechTestFramework.InitializeOSTF(Platforms.WinUI)
    End Sub

End Class