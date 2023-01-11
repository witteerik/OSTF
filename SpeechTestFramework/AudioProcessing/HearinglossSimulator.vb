Namespace Audio

    Public Class HearinglossSimulator_CB

        Public ReadOnly Property ListenerAudiogram As AudiogramData
        Public ReadOnly Property SimulatedAudiogram As AudiogramData

        Public ReadOnly Property AnalysisWindowDuration As Double
            Get
                If SourceSound Is Nothing Then
                    Return -1
                Else
                    Return AnalysisWindowLength / SourceSound.WaveFormat.SampleRate
                End If
            End Get
        End Property

        Public AnalysisWindowLength As Integer = 4096

        Public FirKernelLength As Integer = 4096

        Public BandBank As Audio.DSP.BandBank

        Public Property SourceSound As Sound
        Public Property SimulatedSound As Sound

        Public TimeWindows As New List(Of HearinglossSimulatorTimeWindow)

        Public Sub New(ByRef SimulatedAudiogram As AudiogramData, Optional ByRef ListenerAudiogram As AudiogramData = Nothing)

            If ListenerAudiogram Is Nothing Then
                ListenerAudiogram = New AudiogramData
                ListenerAudiogram.CreateTypicalAudiogramData(AudiogramData.BisgaardAudiograms.NH)
            End If

            'Ensures that the audiograms have UCL data
            ListenerAudiogram.FillUpUCLs
            SimulatedAudiogram.FillUpUCLs

            'Creates the SII critical band bank
            BandBank = Audio.DSP.BandBank.GetSiiCriticalRatioBandBank

            Me.SimulatedAudiogram = SimulatedAudiogram
            Me.ListenerAudiogram = ListenerAudiogram

            'Converting db HLs to dB SPLs
            SimulatedAudiogram.ConvertToSplAudiogram()
            ListenerAudiogram.ConvertToSplAudiogram()

            'Converts audiogram values (thresholds, etc) to spectrum levels
            Dim RefSL = BandBank.GetReferenceSpectrumLevels.Values.ToArray

            'Calculating CB values
            SimulatedAudiogram.InterpolateCriticalBandValues()
            ListenerAudiogram.InterpolateCriticalBandValues()

            'Converting to spectrum levels
            For i = 0 To 20
                SimulatedAudiogram.Cb_Left_AC(i) += RefSL(i)
                SimulatedAudiogram.Cb_Right_AC(i) += RefSL(i)
                SimulatedAudiogram.Cb_Left_BC(i) += RefSL(i)
                SimulatedAudiogram.Cb_Right_BC(i) += RefSL(i)
                SimulatedAudiogram.Cb_Left_UCL(i) += RefSL(i)
                SimulatedAudiogram.Cb_Right_UCL(i) += RefSL(i)
            Next

            For i = 0 To 20
                ListenerAudiogram.Cb_Left_AC(i) += RefSL(i)
                ListenerAudiogram.Cb_Right_AC(i) += RefSL(i)
                ListenerAudiogram.Cb_Left_BC(i) += RefSL(i)
                ListenerAudiogram.Cb_Right_BC(i) += RefSL(i)
                ListenerAudiogram.Cb_Left_UCL(i) += RefSL(i)
                ListenerAudiogram.Cb_Right_UCL(i) += RefSL(i)
            Next


            'Dim x = 1
            'Converting CB levels relative to internal noise spectrum levels
            'SimulatedAudiogram.CompensateCbLevelsForInternalNoiseSpectrumLevels(True)
            'ListenerAudiogram.CompensateCbLevelsForInternalNoiseSpectrumLevels(True)

        End Sub

        Public Sub Simulate(ByRef SourceSound As Audio.Sound)

            TimeWindows.Clear()

            'Enforces stereo input (TODO: This should be done without chancing the SourceSound
            If SourceSound.WaveFormat.Channels = 1 Then SourceSound = SourceSound.ConvertMonoToMultiChannel(2, True)
            If SourceSound.WaveFormat.Channels > 2 Then
                Dim TempSourceSound = New Audio.Sound(New Formats.WaveFormat(SourceSound.WaveFormat.SampleRate, SourceSound.WaveFormat.BitDepth, 2,, SourceSound.WaveFormat.Encoding))
                TempSourceSound.WaveData.SampleData(1) = SourceSound.WaveData.SampleData(1)
                TempSourceSound.WaveData.SampleData(2) = SourceSound.WaveData.SampleData(2)
                SourceSound = TempSourceSound
            End If

            Me.SourceSound = SourceSound

            'Copying sound sections to TimeWindows (skipping the last non-full window)
            For StartSample As Integer = 0 To Me.SourceSound.WaveData.ShortestChannelSampleCount - AnalysisWindowLength - 1 Step (AnalysisWindowLength / 2)

                Dim WindowData = New Sound(SourceSound.WaveFormat)
                For c = 1 To WindowData.WaveFormat.Channels
                    Dim NewChannelArray(AnalysisWindowLength - 1) As Single
                    Array.Copy(Me.SourceSound.WaveData.SampleData(c), StartSample, NewChannelArray, 0, AnalysisWindowLength)
                    WindowData.WaveData.SampleData(c) = NewChannelArray
                Next

                Dim NewTimeWindow As New HearinglossSimulatorTimeWindow(Me)
                NewTimeWindow.SoundData = WindowData
                NewTimeWindow.StartSample = StartSample

                TimeWindows.Add(NewTimeWindow)
            Next


            'Sets of some objects which are reused between the loops in the code below
            Dim FftFormat As New Audio.Formats.FftFormat(4 * 2048,, 1024, Audio.WindowingType.Hamming, False)
            Dim dBSPL_FSdifference As Double? = Audio.Standard_dBFS_dBSPL_Difference

            For Each TimeWindow In TimeWindows

                TimeWindow.CalculateSignalSpectrumLevels(BandBank, FftFormat, dBSPL_FSdifference)

                TimeWindow.CalculateBandGains()

                TimeWindow.CreateDynamicFilter()

                TimeWindow.Filter()

                TimeWindow.Window()

            Next

            'Overlapping windows
            Dim TotalLength As Integer = TimeWindows.Last.StartSample + AnalysisWindowLength
            Dim LeftSimulatedSoundArray(TotalLength - 1) As Single
            Dim RightSimulatedSoundArray(TotalLength - 1) As Single

            For w = 0 To TimeWindows.Count - 1

                Dim StartSample = TimeWindows(w).StartSample
                Dim LeftWindowArray = TimeWindows(w).SoundData.WaveData.SampleData(1)
                Dim RightWindowArray = TimeWindows(w).SoundData.WaveData.SampleData(1)

                For s = 0 To TimeWindows(w).SoundData.WaveData.ShortestChannelSampleCount - 1
                    LeftSimulatedSoundArray(StartSample + s) += LeftWindowArray(s)
                    RightSimulatedSoundArray(StartSample + s) += RightWindowArray(s)
                Next
            Next

            'Storing into the SimulatedSound 
            SimulatedSound = New Sound(SourceSound.WaveFormat)
            SimulatedSound.WaveData.SampleData(1) = LeftSimulatedSoundArray
            SimulatedSound.WaveData.SampleData(2) = RightSimulatedSoundArray

        End Sub

        Public Function GetAverageResponse(ByVal Side As Utils.Sides) As SortedList(Of Double, Double)

            Dim BandGains As New SortedList(Of Double, List(Of Double))
            For b = 0 To Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1
                BandGains.Add(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies(b), New List(Of Double))
            Next

            Select Case Side
                Case Utils.Constants.Sides.Left
                    For Each TimeWindow In TimeWindows
                        For b = 0 To Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1
                            BandGains(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies(b)).Add(TimeWindow.Left_SimulationBandGains(b))
                        Next
                    Next
                Case Utils.Constants.Sides.Right
                    For Each TimeWindow In TimeWindows
                        For b = 0 To Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1
                            BandGains(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies(b)).Add(TimeWindow.Right_SimulationBandGains(b))
                        Next
                    Next
                Case Else
                    Throw New ArgumentException("Unknown value for Side")
            End Select

            Dim AverageResponse As New SortedList(Of Double, Double)
            For Each Kvp In BandGains
                AverageResponse.Add(Kvp.Key, Kvp.Value.Average)
            Next

            Return AverageResponse

        End Function


        Public Class HearinglossSimulatorTimeWindow

            Public ParentHearinglossSimulator As HearinglossSimulator_CB

            Public Sub New(ByRef ParentHearinglossSimulator As HearinglossSimulator_CB)
                Me.ParentHearinglossSimulator = ParentHearinglossSimulator
            End Sub

            Public StartSample As Integer

            Public SignalSpectrumLevels(20) As Double
            Public Left_SimulationTargetSpectrumLevels(20) As Double
            Public Left_SimulationBandGains(20) As Single

            Public Right_SimulationTargetSpectrumLevels(20) As Double
            Public Right_SimulationBandGains(20) As Single

            Private LeftEar_FilterKernel As Audio.Sound
            Private RightEar_FilterKernel As Audio.Sound

            Public SoundData As Audio.Sound

            Public Sub CalculateSignalSpectrumLevels(ByRef BandBank As Audio.DSP.BandBank, ByRef FftFormat As Audio.Formats.FftFormat, ByVal dBSPL_FSdifference As Double)

                'And these are only used to be able to export the values used
                Dim ActualLowerLimitFrequencyList As List(Of Double) = Nothing
                Dim ActualUpperLimitFrequencyList As List(Of Double) = Nothing
                SignalSpectrumLevels = Audio.DSP.CalculateSpectrumLevels(SoundData, 1, BandBank, FftFormat, ActualLowerLimitFrequencyList, ActualUpperLimitFrequencyList, dBSPL_FSdifference).ToArray

            End Sub

            Public Sub CalculateBandGains()

                For b = 0 To 20

                    Dim S = Math.Max(Single.MinValue, SignalSpectrumLevels(b))

                    'Left side
                    Dim Tn_L = ParentHearinglossSimulator.ListenerAudiogram.Cb_Left_AC(b)
                    Dim UCLn_L = ParentHearinglossSimulator.ListenerAudiogram.Cb_Left_UCL(b)
                    Dim Tsim_L = ParentHearinglossSimulator.SimulatedAudiogram.Cb_Left_AC(b)
                    Dim UCLsim_L = ParentHearinglossSimulator.SimulatedAudiogram.Cb_Left_UCL(b)

                    Dim SimulationLevel_L = Math.Min(UCLn_L, Tn_L + (S - Tsim_L) / (UCLsim_L - Tsim_L) * (UCLn_L - Tn_L))
                    Left_SimulationTargetSpectrumLevels(b) = SimulationLevel_L 'Uncessesary to store

                    Left_SimulationBandGains(b) = Math.Max(Single.MinValue, SimulationLevel_L - S)

                    'Right side
                    Dim Tn_R = ParentHearinglossSimulator.ListenerAudiogram.Cb_Right_AC(b)
                    Dim UCLn_R = ParentHearinglossSimulator.ListenerAudiogram.Cb_Right_UCL(b)
                    Dim Tsim_R = ParentHearinglossSimulator.SimulatedAudiogram.Cb_Right_AC(b)
                    Dim UCLsim_R = ParentHearinglossSimulator.SimulatedAudiogram.Cb_Right_UCL(b)

                    Dim SimulationLevel_R = Math.Min(UCLn_R, Tn_R + (S - Tsim_R) / (UCLsim_R - Tsim_R) * (UCLn_R - Tn_R))
                    Right_SimulationTargetSpectrumLevels(b) = SimulationLevel_R 'Uncessesary to store

                    Right_SimulationBandGains(b) = Math.Max(Single.MinValue, SimulationLevel_R - S)

                Next

            End Sub

            Public Sub CreateDynamicFilter()

                Dim LeftEarFilter_TargetResponse As New List(Of Tuple(Of Single, Single))
                Dim RightEarFilter_TargetResponse As New List(Of Tuple(Of Single, Single))

                'Extending gain values towards 1 Hz
                LeftEarFilter_TargetResponse.Add(New Tuple(Of Single, Single)(1, Left_SimulationBandGains(0)))
                RightEarFilter_TargetResponse.Add(New Tuple(Of Single, Single)(1, Left_SimulationBandGains(0)))

                'Adding gain
                For b = 0 To 20
                    LeftEarFilter_TargetResponse.Add(New Tuple(Of Single, Single)(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies(b), Left_SimulationBandGains(b)))
                    RightEarFilter_TargetResponse.Add(New Tuple(Of Single, Single)(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies(b), Right_SimulationBandGains(b)))
                Next

                'Extending gain values to the Nyquist frequency (with some margin)
                LeftEarFilter_TargetResponse.Add(New Tuple(Of Single, Single)(Int(SoundData.WaveFormat.SampleRate / 2) - 2, Left_SimulationBandGains(20)))
                RightEarFilter_TargetResponse.Add(New Tuple(Of Single, Single)(Int(SoundData.WaveFormat.SampleRate / 2) - 2, Left_SimulationBandGains(20)))

                LeftEar_FilterKernel = Audio.GenerateSound.CreateCustumImpulseResponse(LeftEarFilter_TargetResponse, Nothing, SoundData.WaveFormat, New Formats.FftFormat(), ParentHearinglossSimulator.FirKernelLength,, True)
                RightEar_FilterKernel = Audio.GenerateSound.CreateCustumImpulseResponse(RightEarFilter_TargetResponse, Nothing, SoundData.WaveFormat, New Formats.FftFormat(), ParentHearinglossSimulator.FirKernelLength,, True)

            End Sub

            Public Sub Filter()

                'Adjusting the amplitude response of both sides
                Dim LeftSound = SoundData.CopySection(1, 0, SoundData.WaveData.SampleData(1).Length)
                Dim RightSound = SoundData.CopySection(2, 0, SoundData.WaveData.SampleData(2).Length)
                LeftSound = Audio.DSP.FIRFilter(LeftSound, LeftEar_FilterKernel, New Formats.FftFormat(), ,,,, False, True, True)
                RightSound = Audio.DSP.FIRFilter(RightSound, RightEar_FilterKernel, New Formats.FftFormat(), ,,,, False, True, True)

                SoundData.WaveData.SampleData(1) = LeftSound.WaveData.SampleData(1)
                SoundData.WaveData.SampleData(2) = RightSound.WaveData.SampleData(1)

            End Sub

            Public Sub Window()

                For c = 1 To SoundData.WaveFormat.Channels
                    Dim SoundArray = SoundData.WaveData.SampleData(c)
                    Audio.WindowingFunction(SoundArray, WindowingType.Hanning)
                Next

            End Sub

        End Class

    End Class


    Public Class HearinglossSimulator_GTF

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

        Public Sub New(ByVal WaveFormat As Formats.WaveFormat)

            'Storing the required wave format
            Me.WaveFormat = WaveFormat

            'Creating a filterbank
            FilterBank = New Audio.DSP.GammatoneFirFilterBank()
            'FilterBank.SetupAdjacentCentreFrequencies(WaveFormat, 125, 8000)
            FilterBank.SetupAudiogramFrequencies(WaveFormat)

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
                LeftSideData(i).BandData.WriteWaveFile("C:\Temp\B\B" & LeftSideData(i).CenterFrequency.ToString("00000") & "_" & LeftSideData(i).BandWidth.ToString("00000") & ".wav")
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

            Public Property ParentSimulator As HearinglossSimulator_GTF

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

            Public Sub New(ByRef ParentSimulator As HearinglossSimulator_GTF)
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
