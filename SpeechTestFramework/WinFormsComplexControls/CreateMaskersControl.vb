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
        NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.White)
        NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.RandomSuperposition)
        NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.SpeechMaterialWeighted)
        NoiseType_ComboBox.Items.Add(GenerateSnrRangeStimuli_NoiseTypes.StandardSpeechWeighted)
        NoiseType_ComboBox.SelectedIndex = 2

        NoiseFrequencyWheighting_ComboBox.Items.Add(Audio.FrequencyWeightings.Z)
        NoiseFrequencyWheighting_ComboBox.Items.Add(Audio.FrequencyWeightings.C)
        NoiseFrequencyWheighting_ComboBox.Items.Add(Audio.FrequencyWeightings.K)
        NoiseFrequencyWheighting_ComboBox.Items.Add(Audio.FrequencyWeightings.RLB)
        NoiseFrequencyWheighting_ComboBox.SelectedIndex = 1

        SpeechMatchFilterType_ComboBox.Items.Add(SpeechMatchFilterTypes.OctaveBands)
        SpeechMatchFilterType_ComboBox.Items.Add(SpeechMatchFilterTypes.HalfOctaveBands)
        SpeechMatchFilterType_ComboBox.Items.Add(SpeechMatchFilterTypes.ThirdOctaveBands)
        SpeechMatchFilterType_ComboBox.Items.Add(SpeechMatchFilterTypes.SiiCriticalBands)
        SpeechMatchFilterType_ComboBox.SelectedIndex = 1

    End Sub


    Private Sub NoiseType_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles NoiseType_ComboBox.SelectedIndexChanged

        If NoiseType_ComboBox.SelectedItem = GenerateSnrRangeStimuli_NoiseTypes.RandomSuperposition Then
            NumberOfOverlays_IntegerParsingTextBox.Enabled = True
        Else
            NumberOfOverlays_IntegerParsingTextBox.Enabled = False
        End If

        If NoiseType_ComboBox.SelectedItem = GenerateSnrRangeStimuli_NoiseTypes.SpeechMaterialWeighted Then
            SpeechMatchFilterType_ComboBox.Enabled = True
            Smoothen_IntegerParsingTextBox.Enabled = True
            RowingAverageLength_IntegerParsingTextBox.Enabled = True
        Else
            SpeechMatchFilterType_ComboBox.Enabled = False
            Smoothen_IntegerParsingTextBox.Enabled = False
            RowingAverageLength_IntegerParsingTextBox.Enabled = False
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
        Dim SmoothCount As Integer = 0
        If Smoothen_IntegerParsingTextBox.Value.HasValue Then
            SmoothCount = Smoothen_IntegerParsingTextBox.Value
            If SmoothCount < 0 Then
                MsgBox("Smoothening of noise spectrum must be a non-negative integer value!")
                Exit Sub
            End If
        End If

        Dim RowingAverageLength As Integer = 2 ' Using 2 as default
        If RowingAverageLength_IntegerParsingTextBox.Value.HasValue Then
            RowingAverageLength = RowingAverageLength_IntegerParsingTextBox.Value
            If RowingAverageLength < 0 Then
                MsgBox("Rowing average length must be a non-negative integer value!")
                Exit Sub
            End If
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

                'Creating a band filter
                Dim BandBank As Audio.DSP.BandBank = Nothing

                Select Case SpeechMatchFilterType
                    Case SpeechMatchFilterTypes.SiiCriticalBands
                        BandBank = Audio.DSP.BandBank.GetSiiCriticalRatioBandBank
                    Case SpeechMatchFilterTypes.OctaveBands
                        BandBank = Audio.DSP.BandBank.GetOctaveBandBank(62.5, 8)
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

                'Applies a smoothening function to the band levels
                If SmoothCount > 0 Then

                    Dim AveragedBandLevels As New List(Of Double)
                    For i = 0 To BandLevels.Count - 1
                        Dim AverageList As New List(Of Double)
                        For f = 0 To RowingAverageLength - 1
                            If f + i < BandLevels.Count Then
                                AverageList.Add(BandLevels(i + f))
                            End If
                        Next
                        If AverageList.Count > 0 Then AveragedBandLevels.Add(AverageList.Average)
                    Next

                    For r = 0 To SmoothCount * 2
                        Dim ReversedInterpolatedBandLevels As New List(Of Double)
                        AveragedBandLevels.Reverse()
                        For i = 0 To AveragedBandLevels.Count - 1
                            Dim AverageList As New List(Of Double)
                            For f = 0 To RowingAverageLength - 1
                                If f + i < AveragedBandLevels.Count Then
                                    AverageList.Add(AveragedBandLevels(i + f))
                                End If
                            Next
                            If AverageList.Count > 0 Then ReversedInterpolatedBandLevels.Add(AverageList.Average)
                        Next
                        AveragedBandLevels = ReversedInterpolatedBandLevels
                    Next
                    AveragedBandLevels.Reverse()
                    BandLevels = AveragedBandLevels
                End If

                'Creating a FIR filter kernel with the band levels
                Dim FR As New List(Of Tuple(Of Single, Single))
                Dim Fc = BandBank.GetCentreFrequencies.ToList
                For i = 0 To BandLevels.Count - 1
                    FR.Add(New Tuple(Of Single, Single)(Fc(i), BandLevels(i)))
                Next
                'Adding a last point manually at the highest available frequency
                FR.Add(New Tuple(Of Single, Single)((WaveFormat.SampleRate / 2) - 1, BandLevels.Last - 30))

                Dim IR = Audio.GenerateSound.CreateCustumImpulseResponse(FR, Nothing, WaveFormat, New Audio.Formats.FftFormat(4096), 4096)

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
