Public Module ResponseViewEvents

    Public Class ResponseViewEvent

        ''' <summary>
        ''' The time (in ms) relative to the onset of the test trial that the event should take place
        ''' </summary>
        Public TickTime As Integer

        Public Type As ResponseViewEventTypes

        Public Enum ResponseViewEventTypes
            PlaySound
            StopSound
            ShowVisualSoundSources
            ShowResponseAlternatives
            ShowVisualCue
            HideVisualCue
            ShowResponseTimesOut
            ShowMessage
            HideAll
        End Enum

        Public Box As Object

    End Class



End Module