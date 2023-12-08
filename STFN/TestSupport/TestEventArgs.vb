Public Class SpeechTestInputEventArgs
    Inherits EventArgs

    Public Property LinguisticResponses As New List(Of String)

    Public Property LinguisticResponseTime As DateTime

    Public Property DirectionResponse As SourceLocations

    Public Property DirectionResponseTime As DateTime

End Class


Public Enum SourceLocations
    None
    Left
    Right
End Enum