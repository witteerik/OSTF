Public Class TrialHistory
    Inherits List(Of TestTrial)



End Class

Public Class TestTrial

    ''' <summary>
    ''' An unsigned integer value which can be used to store the test block to which the current trial belongs. While test stages should primarily differ in the applied protocol rules, test blocks should primarily differ in tested content.
    ''' </summary>
    Public TestBlock As UInteger

    ''' <summary>
    ''' An unsigned integer value which can be used to store the test stage to which the current trial belongs. While test blocks should primarily differ in tested content, test stages should primarily differ in the protocol rules applied.
    ''' </summary>
    Public TestStage As UInteger

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

    Public SpeechLevel As Double

End Class