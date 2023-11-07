Namespace Audio


    Public Class HearinglossSimulator_NoiseBased

        Public ReadOnly Property SimulatedAudiogram As AudiogramData

        Private BestSideCriticalBandFrequencyThresholds(20) As Double

        Public AnalysisWindowLength As Integer = 4096

        Public FirKernelLength As Integer = 4096

        Public BandBank As Audio.DSP.BandBank

        Public Sub New(Optional ByRef SimulatedAudiogram As AudiogramData = Nothing)

            If SimulatedAudiogram Is Nothing Then
                SimulatedAudiogram = New AudiogramData
                SimulatedAudiogram.CreateTypicalAudiogramData(AudiogramData.BisgaardAudiograms.S3)
            End If

            'Creates the SII critical band bank
            BandBank = Audio.DSP.BandBank.GetSiiCriticalRatioBandBank

            Me.SimulatedAudiogram = SimulatedAudiogram

            'Calculating CB values
            SimulatedAudiogram.InterpolateCriticalBandValues()

            ' Storing the best side critical band thresholds
            For i = 0 To 20
                BestSideCriticalBandFrequencyThresholds(i) = Math.Min(SimulatedAudiogram.Cb_Left_AC(i), SimulatedAudiogram.Cb_Right_AC(i))
            Next

        End Sub

        ''' <summary>
        ''' Returns a new sound in which noise simulating the (best side) hearing thresholds is mixed with the original sound in SourceSound. 
        ''' Note that the FS sound level in the SourceSound should be Standard_dBFS_dBSPL_Difference (100) dB lower than the intended real world sound level in dB SPL.
        ''' </summary>
        ''' <param name="SourceSound"></param>
        Public Function Simulate(ByRef SourceSound As Audio.Sound, Optional ByVal SupressWarnings As Boolean = False) As Audio.Sound

            If SupressWarnings = False Then
                If SourceSound.WaveFormat.Channels <> 1 Then
                    MsgBox("Only the first (left) sound channel will be used to simulate hearing loss!")
                End If
            End If

            'Getting the noise kernel
            Dim NoiseKernel = GetNoiseKernel(SourceSound.WaveFormat)

            'Creates a warble tone at 1 kHz (measurement sound)
            Dim InternalNoiseSound = Audio.GenerateSound.CreateWhiteNoise(SourceSound.WaveFormat, 1, , SourceSound.WaveData.SampleData(1).Length, Audio.BasicAudioEnums.TimeUnits.samples)

            'Runs convolution with the kernel
            Dim NoiseSound = SpeechTestFramework.Audio.DSP.FIRFilter(InternalNoiseSound, NoiseKernel, New SpeechTestFramework.Audio.Formats.FftFormat, ,,,,, True)

            'Mixing the sounds
            Dim OutputSound As New Audio.Sound(SourceSound.WaveFormat)
            Dim SourceSoundArray = SourceSound.WaveData.SampleData(1)
            Dim NoiseSoundArray = NoiseSound.WaveData.SampleData(1)
            Dim TargetSoundArray(SourceSoundArray.Length - 1) As Single
            For s = 0 To SourceSoundArray.Length - 1
                TargetSoundArray(s) = SourceSoundArray(s) + NoiseSoundArray(s)
            Next
            OutputSound.WaveData.SampleData(1) = TargetSoundArray

            Return OutputSound
        End Function

        Public Function GetNoiseKernel(ByVal WaveFormat As Formats.WaveFormat) As Audio.Sound

            'Getting the internal noise representing the hearing loss

            Dim SpectrumLevels As New List(Of Tuple(Of Double, Double, Double))
            Dim InternalNoise = CalculateInternalNoiseBandLevels(BestSideCriticalBandFrequencyThresholds, False, SpectrumLevels)

            Dim FrequencyRepsonse As New List(Of Tuple(Of Single, Single))

            For i = 0 To 20
                FrequencyRepsonse.Add(New Tuple(Of Single, Single)(SpectrumLevels(i).Item1, InternalNoise(i)))
            Next

            Dim NoiseKernel = Audio.GenerateSound.CreateCustumImpulseResponse(FrequencyRepsonse, Nothing, WaveFormat, New Formats.FftFormat(,,,, True), FirKernelLength)

            'Calibrating the kernel
            CalibrateKernel(NoiseKernel, SpectrumLevels(7).Item2, True)

            Return NoiseKernel

        End Function

        Public Sub CalibrateKernel(ByRef KernelSound As Audio.Sound, ByVal ReferenceInternalNoiseSpectrumLevelAt1000Hz As Integer, Optional ByVal ExportSoundFiles As Boolean = False)

            'Notes the length of the IR
            Dim IrLength As Integer = KernelSound.WaveData.SampleData(1).Length

            'Creates a warble tone at 1 kHz (measurement sound)
            Dim TestSound = Audio.GenerateSound.CreateWhiteNoise(KernelSound.WaveFormat, 1, , IrLength * 5, Audio.BasicAudioEnums.TimeUnits.samples)

            'Runs convolution with the kernel
            Dim ConvolutedSound = SpeechTestFramework.Audio.DSP.FIRFilter(TestSound, KernelSound, New SpeechTestFramework.Audio.Formats.FftFormat, ,,,,, True)

            ' Now the kernel gain should be adjusted so that it generates a ReferenceInternalNoiseSpectrumLevelAt1000Hz at 1kHz
            Dim PreAdjustmentMeasurementSound = ConvolutedSound.CopySection(1, IrLength, 3 * IrLength)
            Dim PreLevels = SpeechTestFramework.Audio.DSP.CalculateSpectrumLevels(PreAdjustmentMeasurementSound, 1, ,,,, Audio.Standard_dBFS_dBSPL_Difference)

            'Calculates th needed gain
            Dim GainNeeded = ReferenceInternalNoiseSpectrumLevelAt1000Hz - PreLevels(7) - Audio.Standard_dBFS_dBSPL_Difference

            'Applies the needed gain to the kernel
            Audio.DSP.AmplifySection(KernelSound, GainNeeded)

            If ExportSoundFiles = True Then
                KernelSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "HLSim_Kernel_Calib_Kernel.wav"))
                'PreAdjustmentMeasurementSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "HLSim_Kernel_Calib_ConvolutedSound.wav"))
            End If

        End Sub

        ''' <summary>
        ''' Calculates reference internal noise band levels
        ''' </summary>
        ''' <param name="T_">Equivalent hearing threshold level (in each critical band)</param>
        ''' <param name="Binaural"></param>
        ''' <returns></returns>
        Public Function CalculateInternalNoiseBandLevels(ByVal T_ As Double(), ByVal Binaural As Boolean, ByRef SpectrumLevels As List(Of Tuple(Of Double, Double, Double))) As Double()

            'Cloning the input arrays, so that they don't get modified by the function
            Dim T__Copy As Double() = T_.Clone

            '  Critical band specifications according to table 1 in ANSI S3.5-1997
            '   Centre frequencies
            Dim F As Double() = {150, 250, 350, 450, 570, 700, 840, 1000, 1170, 1370, 1600, 1850,
                 2150, 2500, 2900, 3400, 4000, 4800, 5800, 7000, 8500}

            '  Lower limits
            Dim l As Double() = {100, 200, 300, 400, 510, 630, 770, 920, 1080, 1270, 1480, 1720,
               2000, 2320, 2700, 3150, 3700, 4400, 5300, 6400, 7700}

            '  Upper limits
            Dim h As Double() = {200, 300, 400, 510, 630, 770, 920, 1080, 1270, 1480, 1720, 2000,
                 2320, 2700, 3150, 3700, 4400, 5300, 6400, 7700, 9500}

            '  Bandwidths
            Dim CBW(20) As Double
            For i = 0 To 20
                CBW(i) = h(i) - l(i)
            Next

            'Step 6
            If Binaural = True Then
                For i = 0 To 20
                    T__Copy(i) -= 1.7
                Next
            End If

            'Step 7
            '  Reference internal noise spectrum
            Dim X As Double() = {1.5, -3.9, -7.2, -8.9, -10.3, -11.4, -12.0, -12.5, -13.2, -14.0, -15.4,
                  -16.9, -18.8, -21.2, -23.2, -24.9, -25.9, -24.2, -19.0, -11.7, -6.0}

            '  # Calculating Equivalent internal noise spectrum
            Dim X_(20) As Double

            For i = 0 To 20
                X_(i) = X(i) + T__Copy(i)
            Next

            'Storing the centre band frequencies, spectrum levels and band widths SpectrumLevels
            For i = 0 To 20
                SpectrumLevels.Add(New Tuple(Of Double, Double, Double)(F(i), X_(i), CBW(i)))
            Next

            'Converting to band levels
            Dim BandLevels(20) As Double
            For i = 0 To 20
                BandLevels(i) = Audio.DSP.SpectrumLevel2BandLevel(X_(i), CBW(i))
            Next

            Return BandLevels

        End Function



    End Class



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

            'Calculating CB values
            SimulatedAudiogram.InterpolateCriticalBandValues()
            ListenerAudiogram.InterpolateCriticalBandValues()

            ''Converts audiogram values (thresholds, etc) to spectrum levels
            'Dim RefSL = BandBank.GetReferenceSpectrumLevels.Values.ToArray

            ''Converting to spectrum levels
            'For i = 0 To 20
            '    SimulatedAudiogram.Cb_Left_AC(i) += RefSL(i)
            '    SimulatedAudiogram.Cb_Right_AC(i) += RefSL(i)
            '    SimulatedAudiogram.Cb_Left_BC(i) += RefSL(i)
            '    SimulatedAudiogram.Cb_Right_BC(i) += RefSL(i)
            '    SimulatedAudiogram.Cb_Left_UCL(i) += RefSL(i)
            '    SimulatedAudiogram.Cb_Right_UCL(i) += RefSL(i)
            'Next

            'For i = 0 To 20
            '    ListenerAudiogram.Cb_Left_AC(i) += RefSL(i)
            '    ListenerAudiogram.Cb_Right_AC(i) += RefSL(i)
            '    ListenerAudiogram.Cb_Left_BC(i) += RefSL(i)
            '    ListenerAudiogram.Cb_Right_BC(i) += RefSL(i)
            '    ListenerAudiogram.Cb_Left_UCL(i) += RefSL(i)
            '    ListenerAudiogram.Cb_Right_UCL(i) += RefSL(i)
            'Next

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
                Dim RightWindowArray = TimeWindows(w).SoundData.WaveData.SampleData(2)

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

            Public Left_SignalCriticalBandLevels(20) As Double
            Public Left_SimulationBandGains(20) As Single

            Public Right_SignalCriticalBandLevels(20) As Double
            Public Right_SimulationBandGains(20) As Single

            Private LeftEar_FilterKernel As Audio.Sound
            Private RightEar_FilterKernel As Audio.Sound

            Public SoundData As Audio.Sound

            Public Sub CalculateSignalSpectrumLevels(ByRef BandBank As Audio.DSP.BandBank, ByRef FftFormat As Audio.Formats.FftFormat, ByVal dBSPL_FSdifference As Double)

                'And these are only used to be able to export the values used
                Left_SignalCriticalBandLevels = Audio.DSP.CalculateBandLevels(SoundData, 1, BandBank, FftFormat).ToArray
                Right_SignalCriticalBandLevels = Audio.DSP.CalculateBandLevels(SoundData, 2, BandBank, FftFormat).ToArray

                'Converting from dBFS to dBSPL
                For i = 0 To Left_SignalCriticalBandLevels.Length - 1
                    Left_SignalCriticalBandLevels(i) += dBSPL_FSdifference
                Next
                For i = 0 To Right_SignalCriticalBandLevels.Length - 1
                    Right_SignalCriticalBandLevels(i) += dBSPL_FSdifference
                Next

                'SignalSpectrumLevels = Audio.DSP.CalculateSpectrumLevels(SoundData, 1, BandBank, FftFormat, ActualLowerLimitFrequencyList, ActualUpperLimitFrequencyList, dBSPL_FSdifference).ToArray

            End Sub

            Public Sub CalculateBandGains()

                For b = 0 To 20

                    'Left side
                    Dim S_L = Math.Max(Single.MinValue, Left_SignalCriticalBandLevels(b))
                    Dim Ts_L = ParentHearinglossSimulator.SimulatedAudiogram.Cb_Left_AC(b)
                    Dim UCLs_L = ParentHearinglossSimulator.SimulatedAudiogram.Cb_Left_UCL(b)
                    Dim Tn_L = ParentHearinglossSimulator.ListenerAudiogram.Cb_Left_AC(b)
                    Dim UCLn_L = ParentHearinglossSimulator.ListenerAudiogram.Cb_Left_UCL(b)
                    Left_SimulationBandGains(b) = GetGain(S_L, Ts_L, UCLs_L, Tn_L, UCLn_L)


                    'Right side
                    Dim S_R = Math.Max(Single.MinValue, Right_SignalCriticalBandLevels(b))
                    Dim Ts_R = ParentHearinglossSimulator.SimulatedAudiogram.Cb_Right_AC(b)
                    Dim UCLs_R = ParentHearinglossSimulator.SimulatedAudiogram.Cb_Right_UCL(b)
                    Dim Tn_R = ParentHearinglossSimulator.ListenerAudiogram.Cb_Right_AC(b)
                    Dim UCLn_R = ParentHearinglossSimulator.ListenerAudiogram.Cb_Right_UCL(b)
                    Right_SimulationBandGains(b) = GetGain(S_R, Ts_R, UCLs_R, Tn_R, UCLn_R)

                Next

            End Sub

            Private Function GetGain(ByVal S As Double, ByVal Ts As Double,
                ByVal UCLs As Double, ByVal Tn As Double, ByVal UCLn As Double) As Double

                Dim A_L = S - Ts
                Dim B_L = UCLs - Ts

                Dim SimulationLevel_L As Double
                Select Case A_L / B_L
                    Case < 0
                        SimulationLevel_L = Tn - Ts + S
                    Case > 1
                        SimulationLevel_L = UCLn - UCLs + S
                    Case Else
                        SimulationLevel_L = Math.Min(UCLn, Tn + ((S - Ts) / (UCLs - Ts)) * (UCLn - Tn))
                End Select

                Dim Gain As Double = Math.Max(Single.MinValue, SimulationLevel_L - S)

                'Console.WriteLine(vbCrLf & "Simulated audiogram dynamic range: " & Ts & " to " & UCLs & " dB SPL, Input signal level: " & S & vbCrLf &
                '                      "Listener audiogram dynamic range: " & Tn & " to " & UCLn & " dB SPL, Corresponding simulation level: " & SimulationLevel_L & vbCrLf &
                '                      "Gain: " & Gain)

                Return Gain

            End Function

            Public Sub CreateDynamicFilter()

                Dim LeftEarFilter_TargetResponse As New List(Of Tuple(Of Single, Single))
                Dim RightEarFilter_TargetResponse As New List(Of Tuple(Of Single, Single))

                'Extending gain values towards 1 Hz
                LeftEarFilter_TargetResponse.Add(New Tuple(Of Single, Single)(1, Left_SimulationBandGains(0)))
                RightEarFilter_TargetResponse.Add(New Tuple(Of Single, Single)(1, Right_SimulationBandGains(0)))

                'Adding gain
                For b = 0 To 20
                    LeftEarFilter_TargetResponse.Add(New Tuple(Of Single, Single)(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies(b), Left_SimulationBandGains(b)))
                    RightEarFilter_TargetResponse.Add(New Tuple(Of Single, Single)(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies(b), Right_SimulationBandGains(b)))
                Next

                'Extending gain values to the Nyquist frequency (with some margin)
                LeftEarFilter_TargetResponse.Add(New Tuple(Of Single, Single)(Int(SoundData.WaveFormat.SampleRate / 2) - 2, Left_SimulationBandGains(20)))
                RightEarFilter_TargetResponse.Add(New Tuple(Of Single, Single)(Int(SoundData.WaveFormat.SampleRate / 2) - 2, Right_SimulationBandGains(20)))

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
