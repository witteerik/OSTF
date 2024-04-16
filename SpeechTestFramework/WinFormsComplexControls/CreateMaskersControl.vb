Imports SpeechTestFramework.CreateMaskersControl
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
        NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.SpeechMaterialWeighted)
        NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.RandomSuperposition)
        NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.StandardSpeechWeighted)
        NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.White)
        NoiseType_ComboBox.SelectedIndex = 0

        NoiseFrequencyWheighting_ComboBox.Items.Add(Audio.FrequencyWeightings.C)
        NoiseFrequencyWheighting_ComboBox.Items.Add(Audio.FrequencyWeightings.Z)
        NoiseFrequencyWheighting_ComboBox.Items.Add(Audio.FrequencyWeightings.K)
        NoiseFrequencyWheighting_ComboBox.Items.Add(Audio.FrequencyWeightings.RLB)
        NoiseFrequencyWheighting_ComboBox.SelectedIndex = 0

        SpeechMatchFilterType_ComboBox.Items.Add(SpeechMatchFilterTypes.OctaveBands)
        SpeechMatchFilterType_ComboBox.Items.Add(SpeechMatchFilterTypes.HalfOctaveBands)
        SpeechMatchFilterType_ComboBox.Items.Add(SpeechMatchFilterTypes.ThirdOctaveBands)
        SpeechMatchFilterType_ComboBox.Items.Add(SpeechMatchFilterTypes.SiiCriticalBands)
        SpeechMatchFilterType_ComboBox.SelectedIndex = 0

    End Sub


    Private Sub NoiseType_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles NoiseType_ComboBox.SelectedIndexChanged

        If NoiseType_ComboBox.SelectedItem = GenerateSnrRangeStimuli_NoiseTypes.RandomSuperposition Then
            NumberOfOverlays_IntegerParsingTextBox.Enabled = True
        Else
            NumberOfOverlays_IntegerParsingTextBox.Enabled = False
        End If

        If NoiseType_ComboBox.SelectedItem = GenerateSnrRangeStimuli_NoiseTypes.SpeechMaterialWeighted Then
            SpeechMatchFilterType_ComboBox.Enabled = True
            Interpolation_CheckBox.Enabled = True
            EnforceMonotonicAbove1kHz_CheckBox1.Enabled = True
        Else
            SpeechMatchFilterType_ComboBox.Enabled = False
            Interpolation_CheckBox.Enabled = False
            EnforceMonotonicAbove1kHz_CheckBox1.Enabled = True
        End If

    End Sub

    Public Enum GenerateSnrRangeStimuli_NoiseTypes
        White
        RandomSuperposition
        SpeechMaterialWeighted
        StandardSpeechWeighted
    End Enum

    Public Enum SpeechMatchFilterTypes
        SiiCriticalBands
        OctaveBands
        HalfOctaveBands
        ThirdOctaveBands
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

        Dim SpeechMatchFilterType As SpeechMatchFilterTypes = SpeechMatchFilterType_ComboBox.SelectedItem

        Dim UseInterpolate As Boolean = Interpolation_CheckBox.Checked
        Dim EnforceMonotonicAbove1k As Boolean = EnforceMonotonicAbove1kHz_CheckBox1.Checked

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

                'Creating a band filter
                Dim BandBank As Audio.DSP.BandBank = Nothing

                Select Case SpeechMatchFilterType
                    Case SpeechMatchFilterTypes.SiiCriticalBands
                        BandBank = Audio.DSP.BandBank.GetSiiCriticalRatioBandBank
                    Case SpeechMatchFilterTypes.OctaveBands
                        BandBank = Audio.DSP.BandBank.GetOctaveBandBank(62.5, 9)
                    Case SpeechMatchFilterTypes.HalfOctaveBands
                        BandBank = Audio.DSP.BandBank.GetHalfOctaveBands(62.5, 17)
                    Case SpeechMatchFilterTypes.ThirdOctaveBands
                        BandBank = Audio.DSP.BandBank.GetThirdOctaveBandBank(62.5, 25)
                    Case Else
                        Throw New NotImplementedException("Unknown filter type" & SpeechMatchFilterType.ToString)
                End Select

                'Calculating spectrum levels 
                Dim FftFormat = New Audio.Formats.FftFormat(4096,, 2048, Audio.WindowingType.Hanning, True)
                Dim BandLevels = Audio.DSP.CalculateSpectrumLevels(AllSoundsConcatenated, 1, BandBank, FftFormat)
                Dim BandCentreFrequencies = BandBank.GetCentreFrequencies.ToList

                If EnforceMonotonicAbove1k = True Then
                    For i = 1 To BandLevels.Count - 1
                        Dim InverseIndex As Integer = BandLevels.Count - 1 - i

                        'Exits if we've come below 1 kHz
                        If BandCentreFrequencies(InverseIndex) < 1000 Then Exit For

                        'Limits the current (lower frequency) band level to the level of the next higher frequency band level.
                        BandLevels(InverseIndex) = Math.Max(BandLevels(InverseIndex), BandLevels(InverseIndex + 1))

                    Next
                End If

                Dim TargetFrequencyResponse As New List(Of Tuple(Of Single, Single))

                'Adding a first point manually at the lowest available frequency
                TargetFrequencyResponse.Add(New Tuple(Of Single, Single)(1, BandLevels.First - 50))

                'Creating a FIR filter kernel with the band levels
                If UseInterpolate = True Then

                    BandCentreFrequencies.Add(BandCentreFrequencies.Last * Math.Pow(2, 1 / 6)) 'Adding a point at a sixth octave above the highest fc, linearly interpolated from the levels of the two last values
                    BandLevels.Add(BandLevels(BandLevels.Count - 1) + (BandLevels(BandLevels.Count - 1) - BandLevels(BandLevels.Count - 2)))

                    'Log2Frequencies
                    Dim Log2Frequencies As New List(Of Double)
                    For Each Frequency In BandCentreFrequencies
                        Log2Frequencies.Add(Utils.getBase_n_Log(Frequency, 2))
                    Next

                    'Creating an interpolator
                    Dim Interpolator = MathNet.Numerics.Interpolate.Polynomial(Log2Frequencies, BandLevels)

                    'Getting all FFT bin frequencies (half spectrum, excluding zero and Nyquist frequencies)
                    Dim InterpolationFrequencies(FftFormat.FftWindowSize / 2 - 2) As Double
                    For k = 1 To InterpolationFrequencies.Length
                        InterpolationFrequencies(k - 1) = Audio.AudioManagementExt.FftBinFrequencyConversion(Audio.AudioManagementExt.FftBinFrequencyConversionDirection.BinIndexToFrequency, k, WaveFormat.SampleRate, FftFormat.FftWindowSize, Utils.roundingMethods.getClosestValue)
                    Next

                    For i = 0 To InterpolationFrequencies.Length - 1
                        'Skipping extrapolation
                        If InterpolationFrequencies(i) < BandCentreFrequencies.First Then Continue For
                        If InterpolationFrequencies(i) > BandCentreFrequencies(BandCentreFrequencies.Count - 2) Then Continue For
                        'Adding interpolated values
                        TargetFrequencyResponse.Add(New Tuple(Of Single, Single)(InterpolationFrequencies(i), Interpolator.Interpolate(Utils.getBase_n_Log(InterpolationFrequencies(i), 2))))
                    Next

                    If EnforceMonotonicAbove1k = True Then
                        Dim TempTargetFrequencyResponse As New List(Of Tuple(Of Single, Single))
                        Dim LastLimitValue As Double? = Nothing
                        For i = 1 To TargetFrequencyResponse.Count - 1
                            Dim InverseIndex As Integer = TargetFrequencyResponse.Count - 1 - i

                            If TargetFrequencyResponse(InverseIndex).Item1 < 1000 Then

                                'Stored the original value
                                TempTargetFrequencyResponse.Insert(0, New Tuple(Of Single, Single)(
                                                                   TargetFrequencyResponse(InverseIndex).Item1,
                                                                   TargetFrequencyResponse(InverseIndex).Item2)
                                                                   )

                            Else

                                'Limits (again after interpolation) the current (lower frequency) band level to the level of the next higher frequency band level.
                                If LastLimitValue.HasValue = False Then LastLimitValue = TargetFrequencyResponse(InverseIndex + 1).Item2
                                LastLimitValue = Math.Max(TargetFrequencyResponse(InverseIndex).Item2, LastLimitValue.Value)
                                TempTargetFrequencyResponse.Insert(0, New Tuple(Of Single, Single)(TargetFrequencyResponse(InverseIndex).Item1, LastLimitValue))

                            End If

                        Next
                        'Adding the last item
                        'Limits (again after interpolation) the current (lower frequency) band level to the level of the next higher frequency band level.
                        TempTargetFrequencyResponse.Add(New Tuple(Of Single, Single)(TargetFrequencyResponse.Last.Item1, TargetFrequencyResponse.Last.Item2))

                        'Swapping references to get the modified data
                        TargetFrequencyResponse = TempTargetFrequencyResponse

                    End If

                Else

                    'Adding frequency bins
                    For i = 0 To BandLevels.Count - 1
                        TargetFrequencyResponse.Add(New Tuple(Of Single, Single)(BandCentreFrequencies(i), BandLevels(i)))
                    Next

                End If

                'Adding a last point manually at the highest available frequency
                TargetFrequencyResponse.Add(New Tuple(Of Single, Single)((WaveFormat.SampleRate / 2), BandLevels.Last - 30))


                Dim IR = Audio.GenerateSound.CreateCustumImpulseResponse(TargetFrequencyResponse, Nothing, WaveFormat, New Audio.Formats.FftFormat(4096), 4096)

                'Creates a white noise
                Dim InternalNoiseSound = Audio.GenerateSound.CreateWhiteNoise(IR.WaveFormat, 1, , TotalNoiseLength, Audio.BasicAudioEnums.TimeUnits.samples)

                'Runs convolution with the kernel
                NoiseSound = SpeechTestFramework.Audio.DSP.FIRFilter(InternalNoiseSound, IR, New SpeechTestFramework.Audio.Formats.FftFormat, ,,,,, True)


                'Case GenerateSnrRangeStimuli_NoiseTypes.ThresholdSimulating

                '    'Creating TSN
                '    NoiseSound = CreateThresholdSimulatingNoise(WaveFormat, IntendedPresentationLevel, Math.Ceiling(TotalNoiseLength))

            Case GenerateSnrRangeStimuli_NoiseTypes.StandardSpeechWeighted
                Throw New NotImplementedException()
            Case Else
                Throw New NotImplementedException("Unknown noise type!")
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
