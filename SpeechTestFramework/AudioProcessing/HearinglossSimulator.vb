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

        Public FilterBank As Audio.DSP.GammatoneFirFilterBank = Nothing

        Public WaveFormat As Formats.WaveFormat

        Public Sub New(ByVal WaveFormat As Formats.WaveFormat, Optional ByVal CentreFrequencies As List(Of Double) = Nothing)

            'Storing the required wave format
            Me.WaveFormat = WaveFormat

            'Setting default centre frequencies
            If CentreFrequencies Is Nothing Then
                'CentreFrequencies = New List(Of Double) From {125, 250, 500, 750, 1000, 1500, 2000, 3000, 4000, 5000, 6000, 7000, 8000}
                CentreFrequencies = Audio.DSP.GammatoneFirFilterBank.CalculateAdjacentCentreFrequencies(125, 8000)
            End If

            'Creating a filterbank
            FilterBank = New Audio.DSP.GammatoneFirFilterBank(WaveFormat, CentreFrequencies)
            'Exporting filter info and kernels
            'FilterBank.ExportKernels(IO.Path.Combine(ExportFolder, "GammatoneFirFilterKernels"))
            'FilterBank.ExportFilterDescription(ExportFolder)


        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SourceSound"></param>
        ''' <param name="RemoveDcComponent">If set to true, the DC component of the input sound will be set to zero prior to spectrum level calculations.</param>
        ''' <returns></returns>
        Public Function SimulateHearingloss(ByRef SourceSound As Sound,
                                            Optional ByVal RemoveDcComponent As Boolean = True,
                                            Optional ByVal KeepInputSoundLength As Boolean = True) As Sound

            'Checks the WaveFormat
            If WaveFormat.IsEqual(SourceSound.WaveFormat, True, True, True, True) = False Then Throw New Exception("Unexpected wave format in the SourceSound. Make sure the same format is used when creating your instance of HearinglossSimulator as when calling SimulateHearingloss!")

            'References the supplied SourceSound into Me.SourceSound
            Me.SourceSound = SourceSound

            'Removing SourceSound DC-component
            If RemoveDcComponent = True Then Audio.DSP.RemoveDcComponent(SourceSound)

            'Filterring the input sound (left and right channels)
            Dim FilteredSoundChannel1 = FilterBank.Filter(SourceSound, 1, KeepInputSoundLength)
            Dim FilteredSoundChannel2 As New List(Of Audio.DSP.GammatoneFirFilterBank.FilteredSound)
            If SourceSound.WaveFormat.Channels > 1 Then FilteredSoundChannel2 = FilterBank.Filter(SourceSound, 2, KeepInputSoundLength)

            'Clearing any previoulsy created list of FrequencyBand
            LeftSideData.Clear()
            RightSideData.Clear()

            'Setting up simulation FrequencyBands
            'Left side
            For BandIndex = 0 To FilteredSoundChannel1.Count - 1
                'Creating a new band
                Dim NewBand As New FrequencyBand(Me)
                NewBand.BandData = FilteredSoundChannel1(BandIndex).Sound
                NewBand.BandWidth = FilteredSoundChannel1(BandIndex).Bandwidth
                NewBand.CenterFrequency = FilteredSoundChannel1(BandIndex).CentreFrequency
                LeftSideData.Add(NewBand)
            Next

            'Left side
            For BandIndex = 0 To FilteredSoundChannel2.Count - 1
                'Creating a new band
                Dim NewBand As New FrequencyBand(Me)
                NewBand.BandData = FilteredSoundChannel2(BandIndex).Sound
                NewBand.BandWidth = FilteredSoundChannel2(BandIndex).Bandwidth
                NewBand.CenterFrequency = FilteredSoundChannel2(BandIndex).CentreFrequency
                RightSideData.Add(NewBand)
            Next


            'Testing to restore the original sound
            Dim LeftChannelArray = LeftSideData(1).BandData.WaveData.SampleData(1)
            For i = 2 To LeftSideData.Count - 1
                Dim CurrentBandData = LeftSideData(i).BandData.WaveData.SampleData(1)
                For s = 0 To CurrentBandData.Length - 1
                    LeftChannelArray(s) += CurrentBandData(s)
                Next
            Next

            Dim RightChannelArray() As Single = Nothing
            If RightSideData.Count > 0 Then
                RightChannelArray = RightSideData(1).BandData.WaveData.SampleData(1)
                For i = 2 To RightSideData.Count - 1
                    Dim CurrentBandData = RightSideData(i).BandData.WaveData.SampleData(1)
                    For s = 0 To CurrentBandData.Length - 1
                        RightChannelArray(s) += CurrentBandData(s)
                    Next
                Next
            End If

            'Dim FilteredSoundLevels = FilterBank.GetFilteredSoundLevels(SourceSound, 1)


            'Creates an OutputSound, with the same format as the input sound
            Dim OutputSound As New Sound(SourceSound.WaveFormat)

            OutputSound.WaveData.SampleData(1) = LeftChannelArray
            If RightSideData.Count > 0 Then
                OutputSound.WaveData.SampleData(1) = RightChannelArray
            End If


            'For i = 0 To BandLevels.Count - 1
            '    'Converting dB FS to dB SPL
            '    Dim BandLevel_SPL As Double = BandLevels(i) + dBSPL_FSdifference

            '    'Calculating spectrum level according to equation 3 in ANSI S3.5-1997 (The SII-standard)
            '    Dim SpectrumLevel As Double = Audio.DSP.BandLevel2SpectrumLevel(BandLevel_SPL, BandWidths(i))
            '    SpectrumLevelList.Add(SpectrumLevel)
            'Next


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
