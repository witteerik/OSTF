Public Class SpeechTestInputEventArgs
    Inherits EventArgs

    Public Property LinguisticResponse As String

    Public Property LinguisticResponseTime As DateTime

    Public Property DirectionResponse As SourceLocations

    Public Property DirectionResponseTime As DateTime

End Class


Public Enum SourceLocations
    None
    Left
    Right
End Enum