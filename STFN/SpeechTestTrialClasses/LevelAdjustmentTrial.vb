Public Class LevelAdjustmentTrial
    Inherits TestTrial

    Public SpeechLevel As Double

    Public MaskerLevel As Double

    Public ContralateralMaskerLevel As Double

    Public AdaptiveValue As Double

    Public TrialStringComment As String = ""
    Public ReadOnly Property SNR As Double
        Get
            Return SpeechLevel - MaskerLevel
        End Get
    End Property

    Public LevelRating As LevelRatings

    Public Enum LevelRatings
        TooLoud
        Good
        TooSoft
    End Enum

    Public Shared Function GetRatingStrings() As List(Of String)

        Select Case GuiLanguage
            Case Utils.Constants.Languages.Swedish
                Return SwedishRatingList
            Case Else
                Return EnglishRatingList
        End Select

    End Function

    Public Function TranslateResponse(ByVal Response As String) As Boolean

        Dim RatingStrings = GetRatingStrings()
        Select Case Response
            Case RatingStrings(0)
                LevelRating = LevelRatings.TooSoft
            Case RatingStrings(1)
                LevelRating = LevelRatings.Good
            Case RatingStrings(2)
                LevelRating = LevelRatings.TooLoud
            Case Else
                Return False
        End Select

        Return True
    End Function

    Public Shared ReadOnly SwedishRatingList As New List(Of String) From {"För svagt", "Lagom starkt", "För starkt"}
    Public Shared ReadOnly EnglishRatingList As New List(Of String) From {"Too soft", "Good volume", "Too loud"}

End Class