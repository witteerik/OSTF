Public Class TestTrial

    Public SpeechMaterialComponent As SpeechMaterialComponent

    ''' <summary>
    ''' A list specifying what is to happen at different timepoints starting from the launch of the test trial
    ''' </summary>
    Public TrialEventList As List(Of ResponseViewEvents.ResponseViewEvent)

    Public Sound As Audio.Sound

    ''' <summary>
    ''' Score equals 1 if correct and 0 if incorrect or missing.
    ''' </summary>
    Public Score As Integer

    Public ResponseAlternativeSpellings As List(Of String)

End Class

Public Class SrtTrial
    Inherits TestTrial

    Public Property AdaptiveStage As SrtSpeechTest.TestStage

    Public SpeechLevel As Double

End Class