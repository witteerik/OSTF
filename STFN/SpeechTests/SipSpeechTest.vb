Public Class SipSpeechTest

    Inherits SpeechTest


    Public Overrides ReadOnly Property AvailableTestModes As List(Of TestModes)
        Get
            Return New List(Of TestModes) From {TestModes.ConstantStimuli, TestModes.AdaptiveSpeech, TestModes.AdaptiveNoise, TestModes.AdaptiveDirectionality}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestProtocols As List(Of TestProtocol)
        Get
            Return TestProtocols.GetSipProtocols
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableFixedResponseAlternativeCounts As List(Of Integer)
        Get
            Return New List(Of Integer) From {3}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailablePresentationModes As List(Of SoundPropagationTypes)
        Get
            Return New List(Of SoundPropagationTypes) From {SoundPropagationTypes.PointSpeakers, SoundPropagationTypes.SimulatedSoundField}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailablePhaseAudiometryTypes As List(Of BmldModes)
        Get
            Return New List(Of BmldModes) From {BmldModes.RightOnly, BmldModes.LeftOnly, BmldModes.BinauralSamePhase, BmldModes.BinauralPhaseInverted, BmldModes.BinauralUncorrelated}
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSpeechLocations As Integer
        Get
            Return 1
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumMaskerLocations As Integer
        Get
            Return Integer.MaxValue
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumBackgroundNonSpeechLocations As Integer
        Get
            Return Integer.MaxValue
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumBackgroundSpeechLocations As Integer
        Get
            Return Integer.MaxValue
        End Get
    End Property

    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)

    End Sub


    Public Overrides Function InitializeCurrentTest() As Boolean
        'Not yet used in the current speech test
        Return True
    End Function

    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

        Throw New NotImplementedException

    End Function


    Private Function GetNextTrial() As TestTrial

        Dim TestWords = SpeechMaterial.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)

        Dim testSound = TestWords(1).GetSound(AvailableMediasets(0), 0, 1, , , , , False, False, False, , , False)

        Dim Output As TestTrial = New TestTrial
        Output.TrialEventList = New List(Of ResponseViewEvent)
        Output.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 500, .Type = ResponseViewEvent.ResponseViewEventTypes.PlaySound})
        Output.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 2000, .Type = ResponseViewEvent.ResponseViewEventTypes.ShowResponseAlternatives})
        'Output.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1200})
        'Output.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 1500})
        'Output.TrialEventList.Add(New ResponseViewEvent With {.TickTime = 3000})

        Output.Sound = testSound

        'This is the correct player:
        'SoundPlayer.SwapOutputSounds(testSound)

        Return Output

    End Function



    Public Overrides Function SaveResults(TestResults As TestResults) As Boolean
        'Throw New NotImplementedException()
        Return True
    End Function

    Public Overrides Function GetResults() As TestResults
        ' Throw New NotImplementedException()
        Return Nothing
    End Function



End Class