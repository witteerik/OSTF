Public Class WrsTrial
    Inherits TestTrial

    Public SpeechLevel As Double

    Public MaskerLevel As Double

    Public ContralateralMaskerLevel As Double

    Public AdaptiveValue As Double

    Public TestEar As Utils.SidesWithBoth

    Public ReadOnly Property SNR As Double
        Get
            Return SpeechLevel - MaskerLevel
        End Get
    End Property

    Public LinguisticResponse As String = ""

End Class
