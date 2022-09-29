Public Class CalibrationForm
    Inherits SpeechTestFramework.CalibrationForm

    Public Sub New()
        'Loads the OSTA calibration form in research mode
        MyBase.New(SpeechTestFramework.Utils.Constants.UserTypes.Research, True)

        'Initializing the OSTF 
        SpeechTestFramework.InitializeOSTF(Application.StartupPath)

    End Sub

End Class