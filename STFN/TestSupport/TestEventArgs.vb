Public Class SpeechTestInputEventArgs
    Inherits EventArgs

    Public Property CorrectedResponse As CorrectedResponses

    Public Property LinguisticResponse As String

    Public Property DirectionResponse As SourceLocations

    Public Property LinguisticResponses As String()

    Public Property DirectionResponses As SourceLocations

    Public Property TimeResponded As DateTime

End Class

Public Enum CorrectedResponses
    Incorrect = 0
    Correct = 1
    Missing = 99
End Enum

Public Enum SourceLocations
    None
    Left
    Right
End Enum