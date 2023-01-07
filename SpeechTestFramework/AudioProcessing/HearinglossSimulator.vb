Namespace Audio


    Public Class HearinglossSimulator

        Public Property ListenerAudiogram As AudiogramData

        Public Property SimulatedAudiogram As AudiogramData

        Public AnalysisWindowDuration As Double
        Public AnalysisWindowLength As Double
        Public WindowingFunction() As Single

        Public RightSideData As New List(Of FrequencyBand)
        Public LeftSideData As New List(Of FrequencyBand)

        Public Property SourceSound As Sound
        Public Property SimulatedSound As Sound

        Public Function SimulateHearingloss(ByRef SourceSound As Sound) As Sound

            'References the supplied SourceSound into Me.SourceSound
            Me.SourceSound = SourceSound

            'Creates an OutputSound, with the same format as the input sound
            Dim OutputSound As New Sound(SourceSound.WaveFormat)

            'Simulation comes here



            'Stores the OutputSound in SimulatedSound
            SimulatedSound = OutputSound
            'Returns the SimulatedSound
            Return OutputSound

        End Function


        Public Class FrequencyBand

            Public Property ParentSimulator As HearinglossSimulator

            Public CenterFrequency As Double
            Public BandWidth As Double
            'Public LowerCutoffFrequency As Double
            'Public UpperCutoffFrequency As Double

            Public ListenerPureToneThreshold As Double
            Public ListenerPureToneUCL As Double
            Public SimulatedPureToneThreshold As Double
            Public SimulatedPureToneUCL As Double

            Public WindowLag As Integer

            Public BandData As Sound
            Public BandNoise As Sound
            Public TimeFrequencyWindows As New List(Of TimeFrequencyWindow)

            Public Sub New(ByRef ParentSimulator As HearinglossSimulator)
                Me.ParentSimulator = ParentSimulator
            End Sub

        End Class

        Public Class TimeFrequencyWindow
            Public Property ParentBand As FrequencyBand
            Public WindowStartSample As Integer

            Public InputSpectrumLevel As Double
            Public SimulatedSpectrumLevel As Double
            Public ProportionSignal As Double
            Public ProportionNoise As Double

            Public WindowData As Sound

            Public Sub New(ByRef ParentBand As FrequencyBand)
                Me.ParentBand = ParentBand
            End Sub

            Public Function GetWindowLength() As Integer
                Return ParentBand.ParentSimulator.AnalysisWindowLength
            End Function

            Public Function GetWindowingFunction() As Single()
                Return ParentBand.ParentSimulator.WindowingFunction
            End Function

            Public Function GetNoiseCopy() As Sound
                Return ParentBand.BandNoise.CreateSoundDataCopy
            End Function

            Public Function MixSignalAndNoise()

                Throw New NotImplementedException

            End Function

        End Class


    End Class



End Namespace
