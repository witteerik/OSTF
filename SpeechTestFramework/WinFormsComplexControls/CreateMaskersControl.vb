Imports SpeechTestFramework.MediaSetSetupControl

Public Class CreateMaskersControl

    Private Property SourceMediaSet As MediaSet

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Enabled = False
    End Sub

    ''' <summary>
    ''' Initiating the control by setting its media set to the referenced media set.
    ''' </summary>
    ''' <param name="MediaSet"></param>
    Public Sub InitiateControl(ByRef MediaSet As MediaSet)
        SourceMediaSet = MediaSet
        Me.Enabled = True
        'Checking default radio button
        'RearrangeWithinLists_RadioButton.Checked = True

        'Setting a default list length value based on the current length of the first list
        Dim AllLists = SourceMediaSet.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)
        If AllLists.Count > 0 Then
            'ListLength_IntegerParsingTextBox.Text = AllLists(0).ChildComponents.Count.ToString
        End If
    End Sub

    ''' <summary>
    ''' Clears the control from the loaded media set and disables its graphical interface.
    ''' </summary>
    Public Sub DisableControl()
        Me.Enabled = False
        SourceMediaSet = Nothing
    End Sub

    Private Sub CreateMaskersControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Items for the GenerateSnrRangeStimuli_NoiseType_ComboBox
        NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.White)
        NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.RandomSuperposition)
        NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.SpeechMaterialWeighted)
        NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.StandardSpeechWeighted)
        NoiseType_ComboBox.SelectedIndex = 0

        NoiseFrequencyWheighting_ComboBox.Items.Add(Audio.FrequencyWeightings.Z)
        NoiseFrequencyWheighting_ComboBox.Items.Add(Audio.FrequencyWeightings.C)
        NoiseFrequencyWheighting_ComboBox.Items.Add(Audio.FrequencyWeightings.K)
        NoiseFrequencyWheighting_ComboBox.Items.Add(Audio.FrequencyWeightings.RLB)
        NoiseFrequencyWheighting_ComboBox.SelectedIndex = 0

    End Sub


    Private Sub NoiseType_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles NoiseType_ComboBox.SelectedIndexChanged

        If NoiseType_ComboBox.SelectedItem = GenerateSnrRangeStimuli_NoiseTypes.RandomSuperposition Then
            NumberOfOverlays_IntegerParsingTextBox.Enabled = True
        Else
            NumberOfOverlays_IntegerParsingTextBox.Enabled = False
        End If

    End Sub

    Public Enum GenerateSnrRangeStimuli_NoiseTypes
        White
        RandomSuperposition
        SpeechMaterialWeighted
        StandardSpeechWeighted
    End Enum

    Private Sub CreateNoise_Button_Click(sender As Object, e As EventArgs) Handles CreateNoise_Button.Click

        If TargetSNR_IntegerParsingTextBox.Value Is Nothing Then
            MsgBox("You must supply a value for SNR!")
            Exit Sub
        End If

        If NoiseDuration_DoubleParsingTextBox.Value Is Nothing Then
            MsgBox("You must supply a value for noise duration!")
            Exit Sub
        End If

        If NoiseDuration_DoubleParsingTextBox.Value <= 0 Then
            MsgBox("You must supply a positive value for noise duration!")
            Exit Sub
        End If

        'Getting values
        Dim TargetSNR As Double = TargetSNR_IntegerParsingTextBox.Value
        Dim NoiseDuration As Double = NoiseDuration_DoubleParsingTextBox.Value
        Dim NoiseType As GenerateSnrRangeStimuli_NoiseTypes = NoiseType_ComboBox.SelectedItem

        Dim OverlayCount As Integer = 0
        If NoiseType = GenerateSnrRangeStimuli_NoiseTypes.RandomSuperposition Then
            If NumberOfOverlays_IntegerParsingTextBox.Value Is Nothing Then
                MsgBox("You must supply a value for number of speech overlays!")
                Exit Sub
            Else
                OverlayCount = NumberOfOverlays_IntegerParsingTextBox.Value
            End If
        End If


        Dim NoiseFrequencyWeighting As Audio.FrequencyWeightings = NoiseFrequencyWheighting_ComboBox.SelectedItem

        'Asks the user for an output path.
        Dim OutputFolder As String = SourceMediaSet.GetFullMediaParentFolder
        Dim fbd = New Windows.Forms.FolderBrowserDialog
        fbd.SelectedPath = OutputFolder
        fbd.Description = "Where do you want to save you files?"
        Dim result = fbd.ShowDialog
        If result = Windows.Forms.DialogResult.OK Then
            OutputFolder = fbd.SelectedPath
        Else
            Exit Sub
        End If

        'All arguments ok, now start processing

        'Clears previously loaded sounds
        SpeechMaterialComponent.ClearAllLoadedSounds()

        'Loading All needed sound files
        Dim AllComponents = SourceMediaSet.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SourceMediaSet.AudioFileLinguisticLevel)
        For SoundIndex = 0 To SourceMediaSet.MediaAudioItems - 1
            For Each Component In AllComponents
                Dim CurrentSound = Component.GetSound(SourceMediaSet, SoundIndex, 1)
            Next
        Next

        'Referencing the loaded sounds locally
        Dim LoadedSoundFiles = SpeechMaterialComponent.GetAllLoadedSounds

        'Getting and checking the nominal level
        Dim NominalLevel As Double? = Nothing
        Dim WaveFormat As Audio.Formats.WaveFormat = Nothing
        For Each LoadedSoundFile In LoadedSoundFiles
            If NominalLevel.HasValue = False Then
                If LoadedSoundFile.Value.SMA.NominalLevel.HasValue Then
                    NominalLevel = LoadedSoundFile.Value.SMA.NominalLevel
                Else
                    MsgBox("All speech sounds in the current media set needs to have nominal levels specified in their SMA chunk!")
                    Exit Sub
                End If
            Else
                If NominalLevel <> LoadedSoundFile.Value.SMA.NominalLevel Then
                    MsgBox("Unequal nominal levels (as specified in the corresponding SMA chunk) detected among the speech sounds in the current media set!" & vbCrLf & " First deviation sound file detected is: " & LoadedSoundFile.Key)
                    Exit Sub
                End If
            End If

            If WaveFormat Is Nothing Then
                WaveFormat = LoadedSoundFile.Value.WaveFormat
            End If
        Next

        'Calculating noise length
        Dim TotalNoiseLength As Integer = WaveFormat.SampleRate * NoiseDuration

        'Creating noise of lengt TotalNoiseLength 
        Dim NoiseSound As Audio.Sound = Nothing
        Select Case NoiseType
            Case GenerateSnrRangeStimuli_NoiseTypes.White
                NoiseSound = Audio.GenerateSound.CreateWhiteNoise(WaveFormat, 1, 1, Math.Ceiling(TotalNoiseLength), Audio.BasicAudioEnums.TimeUnits.samples)

            Case GenerateSnrRangeStimuli_NoiseTypes.RandomSuperposition
                Dim AllSoundsConcatenated = Audio.DSP.ConcatenateSounds(LoadedSoundFiles.Values.ToList,,,, False,)
                NoiseSound = Audio.GenerateSound.CreateOverlayNoise(AllSoundsConcatenated, OverlayCount, (TotalNoiseLength / WaveFormat.SampleRate) + 1)

            Case GenerateSnrRangeStimuli_NoiseTypes.SpeechMaterialWeighted
                'Getting all speech sound sections concatenated
                Dim AllSoundsConcatenated = Audio.DSP.ConcatenateSounds(LoadedSoundFiles.Values.ToList,,,, False,)

                'AllSoundsConcatenated = Audio.GenerateSound.CreateOverlayNoise(AllSoundsConcatenated, 1000, (TotalNoiseLength / WaveFormat.SampleRate) + 1)

                'Creating a critical band filter
                Dim BandBank = Audio.DSP.BandBank.GetHalfOctaveBands(62.5, 17)
                'Dim BandBank = Audio.DSP.BandBank.GetThirdOctaveBandBank(62.5, 25)

                Dim FftFormat = New Audio.Formats.FftFormat(4096,, 2048, Audio.WindowingType.Hanning, True)
                Dim BandLevels = Audio.DSP.CalculateSpectrumLevels(AllSoundsConcatenated, 1, BandBank, FftFormat)

                Dim InterpolatedBandLevels As New List(Of Double)
                Dim FlowLength As Integer = 2
                For i = 0 To BandLevels.Count - 1
                    Dim AverageList As New List(Of Double)
                    For f = 0 To FlowLength - 1
                        If f + i < BandLevels.Count Then
                            AverageList.Add(BandLevels(i + f))
                        End If
                    Next
                    If AverageList.Count > 0 Then InterpolatedBandLevels.Add(AverageList.Average)
                Next

                'For r = 0 To 20
                '    Dim ReversedInterpolatedBandLevels As New List(Of Double)

                '    InterpolatedBandLevels.Reverse()

                '    For i = 0 To InterpolatedBandLevels.Count - 1
                '        Dim AverageList As New List(Of Double)
                '        For f = 0 To FlowLength - 1
                '            If f + i < InterpolatedBandLevels.Count Then
                '                AverageList.Add(InterpolatedBandLevels(i + f))
                '            End If
                '        Next
                '        If AverageList.Count > 0 Then ReversedInterpolatedBandLevels.Add(AverageList.Average)
                '    Next

                '    InterpolatedBandLevels = ReversedInterpolatedBandLevels

                'Next

                'InterpolatedBandLevels.Reverse()

                'Creating a FIR filter kernel with the band levels
                Dim FR As New List(Of Tuple(Of Single, Single))
                Dim Fc = BandBank.GetCentreFrequencies.ToList
                'FR.Add(New Tuple(Of Single, Single)(1, InterpolatedBandLevels.First - 50))
                For i = 0 To InterpolatedBandLevels.Count - 1
                    FR.Add(New Tuple(Of Single, Single)(Fc(i), InterpolatedBandLevels(i)))
                Next
                'FR.Add(New Tuple(Of Single, Single)((16000) - 1, InterpolatedBandLevels.Last - 12))
                FR.Add(New Tuple(Of Single, Single)((WaveFormat.SampleRate / 2) - 1, InterpolatedBandLevels.Last - 30))


                Dim IR = Audio.GenerateSound.CreateCustumImpulseResponse(FR, Nothing, WaveFormat, New Audio.Formats.FftFormat(4096), 4096)

                'Creates a white noise
                Dim InternalNoiseSound = Audio.GenerateSound.CreateWhiteNoise(IR.WaveFormat, 1, , TotalNoiseLength, Audio.BasicAudioEnums.TimeUnits.samples)

                'Runs convolution with the kernel
                NoiseSound = SpeechTestFramework.Audio.DSP.FIRFilter(InternalNoiseSound, IR, New SpeechTestFramework.Audio.Formats.FftFormat, ,,,,, True)


                'Case GenerateSnrRangeStimuli_NoiseTypes.SpeechMaterialWeighted
                '    'Getting all speech sound sections concatenated
                '    Dim AllSoundsConcatenated = Audio.DSP.ConcatenateSounds(LoadedSoundFiles.Values.ToList,,,, False,)

                '    'AllSoundsConcatenated = Audio.GenerateSound.CreateOverlayNoise(AllSoundsConcatenated, 1000, (TotalNoiseLength / WaveFormat.SampleRate) + 1)

                '    'Creating a critical band filter
                '    Dim BandBank = Audio.DSP.BandBank.GetSiiCriticalRatioBandBank
                '    Dim FftFormat = New Audio.Formats.FftFormat(4096,, 2048, Audio.WindowingType.Hanning, True)
                '    Dim BandLevels = Audio.DSP.CalculateSpectrumLevels(AllSoundsConcatenated, 1, BandBank, FftFormat)

                '    Dim InterpolatedBandLevels As New List(Of Double)
                '    Dim FlowLength As Integer = 2
                '    For i = 0 To BandLevels.Count - 1
                '        Dim AverageList As New List(Of Double)
                '        For f = 0 To FlowLength - 1
                '            If f + i < BandLevels.Count Then
                '                AverageList.Add(BandLevels(i + f))
                '            End If
                '        Next
                '        If AverageList.Count > 0 Then InterpolatedBandLevels.Add(AverageList.Average)
                '    Next

                '    For r = 0 To 20
                '        Dim ReversedInterpolatedBandLevels As New List(Of Double)

                '        InterpolatedBandLevels.Reverse()

                '        For i = 0 To InterpolatedBandLevels.Count - 1
                '            Dim AverageList As New List(Of Double)
                '            For f = 0 To FlowLength - 1
                '                If f + i < InterpolatedBandLevels.Count Then
                '                    AverageList.Add(InterpolatedBandLevels(i + f))
                '                End If
                '            Next
                '            If AverageList.Count > 0 Then ReversedInterpolatedBandLevels.Add(AverageList.Average)
                '        Next

                '        InterpolatedBandLevels = ReversedInterpolatedBandLevels

                '    Next

                '    InterpolatedBandLevels.Reverse()

                '    'Creating a FIR filter kernel with the band levels
                '    Dim FR As New List(Of Tuple(Of Single, Single))
                '    Dim Fc = BandBank.GetCentreFrequencies.ToList
                '    FR.Add(New Tuple(Of Single, Single)(1, InterpolatedBandLevels.First - 50))
                '    For i = 0 To InterpolatedBandLevels.Count - 1
                '        FR.Add(New Tuple(Of Single, Single)(Fc(i), InterpolatedBandLevels(i)))
                '    Next
                '    FR.Add(New Tuple(Of Single, Single)((16000) - 1, InterpolatedBandLevels.Last - 6))
                '    FR.Add(New Tuple(Of Single, Single)((WaveFormat.SampleRate / 2) - 1, InterpolatedBandLevels.Last - 12))


                '    Dim IR = Audio.GenerateSound.CreateCustumImpulseResponse(FR, Nothing, WaveFormat, New Audio.Formats.FftFormat(4096), 4096)

                '    'Creates a white noise
                '    Dim InternalNoiseSound = Audio.GenerateSound.CreateWhiteNoise(IR.WaveFormat, 1, , TotalNoiseLength, Audio.BasicAudioEnums.TimeUnits.samples)

                '    'Runs convolution with the kernel
                '    NoiseSound = SpeechTestFramework.Audio.DSP.FIRFilter(InternalNoiseSound, IR, New SpeechTestFramework.Audio.Formats.FftFormat, ,,,,, True)


                'Case GenerateSnrRangeStimuli_NoiseTypes.ThresholdSimulating

                '    'Creating TSN
                '    NoiseSound = CreateThresholdSimulatingNoise(WaveFormat, IntendedPresentationLevel, Math.Ceiling(TotalNoiseLength))

        End Select

        'Setting the noise level to the Nominal Level, using indicated frequency weighting
        Audio.DSP.MeasureAndAdjustSectionLevel(NoiseSound, NominalLevel, 1,,, NoiseFrequencyWeighting)

        'Attenuating noise by CurrentSNR to attain the CurrentSNR 
        Audio.DSP.AmplifySection(NoiseSound, -TargetSNR)

        'Setting the nominal level
        NoiseSound.SMA.NominalLevel = NominalLevel
        NoiseSound.SMA.InferNominalLevelToAllDescendants()

        'Generating output file
        Dim CurrentOutputPath = IO.Path.Combine(OutputFolder, NoiseType.ToString & "SNR_" & TargetSNR.ToString)
        NoiseSound.WriteWaveFile(CurrentOutputPath)

        MsgBox("Finished creating masker sound file.")

    End Sub

End Class
