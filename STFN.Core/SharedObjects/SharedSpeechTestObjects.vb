Public Module SharedSpeechTestObjects

    Public CurrentParticipantID As String = ""

    Public CurrentParticipantAudiogram As STFN.Core.AudiogramData

    Public Const NoTestId As String = "zz9999"

    Public CurrentSpeechTest As SpeechTest

    Public GuiLanguage As Utils.Languages = Utils.EnumCollection.Languages.Swedish

    Public TestResultsRootFolder As String = ""

End Module
