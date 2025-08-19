Public Class SpeechTestInputEventArgs
    Inherits EventArgs

    Public Property LinguisticResponses As New List(Of String)

    Public Property LinguisticResponseTime As DateTime

    Public Property DirectionResponseLocations As New List(Of Audio.SoundScene.SoundSourceLocation)

    Public Property DirectionResponseName As String

    Public Property DirectionResponseTime As DateTime

End Class


