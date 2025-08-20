Imports STFN.Core
Imports STFN.Core.Utils
Imports STFN.Core.Audio.SoundScene
Imports STFN.Core.Audio


Namespace SipTest

    Public Class SipTrial
        Inherits STFN.Core.SipTest.SipTrial

        ''' <summary>
        ''' A list that can hold non-presented (hypothical) test trials containing the contrasting alternatives.
        ''' </summary>
        Public PseudoTrials As New List(Of SipTrial)

        ''' <summary>
        ''' A list of Tuples that can hold various sound mixes used in the test trial, to be exported to sound file later. Each Sound should be come with a descriptive string (that can be used in the output file name).)
        ''' </summary>
        Public TrialSoundsToExport As New List(Of Tuple(Of String, STFN.Core.Audio.Sound))

        Public Property AdjustedSuccessProbability As Double


        ''' <summary>
        ''' Creates a new SiP-test trial in directional mode
        ''' </summary>
        ''' <param name="ParentTestUnit"></param>
        ''' <param name="SpeechMaterialComponent"></param>
        ''' <param name="MediaSet"></param>
        ''' <param name="TargetStimulusLocations"></param>
        ''' <param name="MaskerLocations"></param>
        ''' <param name="BackgroundLocations"></param>
        ''' <param name="SipMeasurementRandomizer"></param>
        Public Sub New(ByRef ParentTestUnit As SiPTestUnit,
                       ByRef SpeechMaterialComponent As SpeechMaterialComponent,
                       ByRef MediaSet As MediaSet,
                       ByRef SoundPropagationType As SoundPropagationTypes,
                       ByVal TargetStimulusLocations As SoundSourceLocation(),
                       ByVal MaskerLocations As SoundSourceLocation(),
                       ByVal BackgroundLocations As SoundSourceLocation(),
                       ByRef SipMeasurementRandomizer As Random)

            MyBase.New(ParentTestUnit, SpeechMaterialComponent, MediaSet, SoundPropagationType, TargetStimulusLocations, MaskerLocations, BackgroundLocations, SipMeasurementRandomizer)

        End Sub


        ''' <summary>
        ''' Creates a new SiP-test trial in BMLD mode
        ''' </summary>
        ''' <param name="ParentTestUnit"></param>
        ''' <param name="SpeechMaterialComponent"></param>
        ''' <param name="MediaSet"></param>
        ''' <param name="SipMeasurementRandomizer"></param>
        Public Sub New(ByRef ParentTestUnit As SiPTestUnit,
                       ByRef SpeechMaterialComponent As SpeechMaterialComponent,
                       ByRef MediaSet As MediaSet,
                       ByVal SoundPropagationType As SoundPropagationTypes,
                       ByRef SignalMode As BmldModes,
                       ByRef NoiseMode As BmldModes,
                       ByRef SipMeasurementRandomizer As Random)

            MyBase.New(ParentTestUnit, SpeechMaterialComponent, MediaSet, SoundPropagationType, Nothing, Nothing, Nothing, SipMeasurementRandomizer)

            'Storing signal and noise modes
            Me.BmldSignalMode = SignalMode
            Me.BmldNoiseMode = NoiseMode

            'Indicating that it's a BMLD test trial
            IsBmldTrial = True

            'Setting up signal and masker locations
            Select Case BmldSignalMode
                Case BmldModes.BinauralSamePhase, BmldModes.BinauralPhaseInverted
                    TargetStimulusLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0},
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0}}

                Case BmldModes.LeftOnly
                    TargetStimulusLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0}}

                Case BmldModes.RightOnly
                    TargetStimulusLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0}}

            End Select

            Select Case BmldNoiseMode
                Case BmldModes.BinauralSamePhase, BmldModes.BinauralPhaseInverted, BmldModes.BinauralUncorrelated
                    MaskerLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0},
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0}}

                    BackgroundLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0},
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0},
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0},
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0}}

                Case BmldModes.LeftOnly
                    MaskerLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0}}

                    BackgroundLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0},
                        New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1, .Elevation = 0}}

                Case BmldModes.RightOnly
                    MaskerLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0}}

                    BackgroundLocations = {
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0},
                        New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1, .Elevation = 0}}

            End Select



        End Sub


        ''' <summary>
        ''' Sets all levels in the current test trial. Levels should be set prior to mixing the sound.
        ''' </summary>
        ''' <param name="ReferenceLevel"></param>
        ''' <param name="PNR"></param>
        Public Overrides Sub SetLevels(ByVal ReferenceLevel As Double, ByVal PNR As Double)

            'Setting the levels

            Me.Reference_SPL = ReferenceLevel
            Me.PNR = PNR

            'Calculating the difference between the standard ReferenceSpeechMaterialLevel_SPL (68.34 dB SPL) reference level and the one currently used
            Dim RefLevelDifference As Double = ReferenceLevel - ReferenceSpeechMaterialLevel_SPL

            '0. Gettings some levels
            '1.And setting the noise level
            Dim SNR_Type As String = "PNR"
            If SNR_Type = "PNR" Then
                'In this procedure, CurrentSNR represents the sound level difference between the average max level of the contrasting test phonemes, and the masker sound
                'Setting the test word masker to Lcp

                'Setting TargetMasking_SPL to ContrastingPhonemesLevel_SPL
                _TargetMasking_SPL = ReferenceContrastingPhonemesLevel_SPL + RefLevelDifference

            ElseIf SNR_Type = "SNR_SpeechMaterial" Then
                'In this procedure, CurrentSNR represents the sound level difference between the average level of the whole speech material, and the masker sound
                'Setting the TargetMasking_SPL to Lsm
                _TargetMasking_SPL = ReferenceSpeechMaterialLevel_SPL + RefLevelDifference
            Else
                Throw New NotImplementedException()
            End If

            '2. Adjusting the speech level to attain the desired PNR

            'Calculating the unlimited target test word level
            Dim TargetTestWord_SPL = ReferenceTestWordLevel_SPL + RefLevelDifference + PNR

            'Calculating the average speech material level equivalent to the current TargetTestWord_SPL
            Dim CurrentAverageSpeechMaterial_SPL = ReferenceSpeechMaterialLevel_SPL + RefLevelDifference + PNR

            'Setting the ContextRegionSpeech_SPL to the CurrentAverageSpeechMaterial_SPL                    
            ContextRegionSpeech_SPL = CurrentAverageSpeechMaterial_SPL

            '3. Limiting test word level
            If TargetTestWord_SPL > TestWordLevelLimit Then

                Dim Difference As Double = TargetTestWord_SPL - TestWordLevelLimit

                'Decreasing all levels set by Difference, to retain the desired test AdaptiveValue
                _TargetMasking_SPL -= Difference
                TargetTestWord_SPL -= Difference
                ContextRegionSpeech_SPL -= Difference
            End If

            _TestWordLevel = TargetTestWord_SPL

            '4. Limiting the context speech level
            If ContextRegionSpeech_SPL > ContextSpeechLimit Then

                Dim Difference As Double = ContextRegionSpeech_SPL - ContextSpeechLimit

                'Decreasing the ContextRegionForegroundLevel_SPL to ContextSpeechLimit
                ContextRegionSpeech_SPL -= Difference

            End If

            If PseudoTrials IsNot Nothing Then
                For Each NonpresentedAlternativeTrial In PseudoTrials
                    NonpresentedAlternativeTrial.SetLevels(ReferenceLevel, PNR)
                Next
            End If


        End Sub

        Public Overrides Sub MixSound(ByRef SelectedTransducer As AudioSystemSpecification,
                             ByVal MinimumStimulusOnsetTime As Double, ByVal MaximumStimulusOnsetTime As Double,
                             ByRef SipMeasurementRandomizer As Random, ByVal TrialSoundMaxDuration As Double, ByVal UseBackgroundSpeech As Boolean,
                             Optional ByVal FixedMaskerIndices As List(Of Integer) = Nothing, Optional ByVal FixedSpeechIndex As Integer? = Nothing,
                             Optional ByVal SkipNoiseFadeIn As Boolean = False, Optional ByVal SkipNoiseFadeOut As Boolean = False,
                             Optional ByVal UseNominalLevels As Boolean = False)

            'If Me.IsBmldTrial = True Then
            '    FixedMaskerIndices = New List(Of Integer) From {1, 2}
            'Else
            '    FixedMaskerIndices = New List(Of Integer) From {1}
            'End If
            'FixedSpeechIndex = 1

            Try

                'Setting up the SiP-trial sound mix
                Dim MixStopWatch As New Stopwatch
                MixStopWatch.Start()

                'Sets a List of SoundSceneItem in which to put the sounds to mix
                Dim ItemList = New List(Of SoundSceneItem)

                Dim SoundWaveFormat As STFN.Core.Audio.Formats.WaveFormat = Nothing

                'Getting maskers
                SelectedMaskerIndices.Clear()

                Dim NumberOfMaskers As Integer = MaskerLocations.Length

                If FixedMaskerIndices Is Nothing Then
                    SelectedMaskerIndices.AddRange(DSP.SampleWithoutReplacement(NumberOfMaskers, 0, Me.MediaSet.MaskerAudioItems, SipMeasurementRandomizer))
                Else
                    If FixedMaskerIndices.Count <> NumberOfMaskers Then Throw New ArgumentException("FixedMaskerIndices must be either Nothing or contain " & NumberOfMaskers & " integers.")
                    SelectedMaskerIndices.AddRange(FixedMaskerIndices)
                End If

                Dim CurrentSampleRate As Integer
                Dim MaskersLength As Integer
                'Getting the length of the fade-in region before the centralized section (assuming all masker in the test word group to have the same specifications)
                Dim MaskerFadeInLength As Integer
                Dim MaskerFadeOutLength As Integer
                Dim MaskerCentralizedSectionLength As Integer

                Dim Maskers As New List(Of Tuple(Of STFN.Core.Audio.Sound, SoundSourceLocation))
                For MaskerIndex = 0 To NumberOfMaskers - 1
                    Dim Masker As STFN.Core.Audio.Sound = (Me.SpeechMaterialComponent.GetMaskerSound(Me.MediaSet, SelectedMaskerIndices(MaskerIndex)))

                    If MaskerIndex = 0 Then
                        'Stores the sample rate and the wave format
                        CurrentSampleRate = Masker.WaveFormat.SampleRate
                        SoundWaveFormat = Masker.WaveFormat

                        'Getting the lengths of the maskers (Assuming same lengths of all maskers)
                        MaskersLength = Masker.WaveData.SampleData(1).Length

                        'Getting the length of the fade-in region before the centralized section (assuming all masker in the test word group to have the same specifications)
                        MaskerFadeInLength = Masker.SMA.ChannelData(1)(0)(0).Length ' Should be stored as the length of the first word in the first sentence
                        MaskerFadeOutLength = Masker.SMA.ChannelData(1)(0)(2).Length ' Should be stored as the length of the third (and last) word in the first sentence
                        MaskerCentralizedSectionLength = MaskersLength - MaskerFadeInLength - MaskerFadeOutLength

                    End If

                    Maskers.Add(New Tuple(Of STFN.Core.Audio.Sound, SoundSourceLocation)(Masker, MaskerLocations(MaskerIndex)))
                Next

                'Duplicating the maskers for BMLD
                If IsBmldTrial = True Then
                    'Copying masker 1 to masker 2, making them identical
                    Array.Copy(Maskers(0).Item1.WaveData.SampleData(1), Maskers(1).Item1.WaveData.SampleData(1), Maskers(0).Item1.WaveData.SampleData(1).Length)
                End If

                'Randomizing a masker start time, and stores it in TestWordStartTime 

                Dim MaskerStartTime As Double
                If MinimumStimulusOnsetTime = MaximumStimulusOnsetTime Then
                    MaskerStartTime = MinimumStimulusOnsetTime
                Else
                    MaskerStartTime = SipMeasurementRandomizer.Next(Math.Round(MinimumStimulusOnsetTime * 1000), Math.Round(MaximumStimulusOnsetTime * 1000)) / 1000
                End If
                Dim MaskersStartSample As Integer = MaskerStartTime * CurrentSampleRate

                'Calculating a sample region in by which the sound level of the maskers should be defined (Called Centralized Region in Witte's thesis)
                Dim MaskersStartMeasureSample As Integer = Math.Max(0, MaskerFadeInLength - 1)
                Dim MaskersStartMeasureLength As Integer = MaskerCentralizedSectionLength

                'Selects a recording index, and gets the corresponding sound
                'Updating SelectedMediaIndex if FixedSpeechIndex is set. SelectedMediaIndex is otherwise set randomly when the trial is created
                If FixedSpeechIndex.HasValue Then SelectedMediaIndex = FixedSpeechIndex
                Dim Targets As New List(Of Tuple(Of STFN.Core.Audio.Sound, SoundSourceLocation))
                Dim NumberOfTargets As Integer = Me.TargetStimulusLocations.Length
                Dim TestWordLength As Integer
                TargetInitialMargins = New List(Of Integer)
                For TargetIndex = 0 To NumberOfTargets - 1
                    'TODO: Here the same signal is taken for all locations. If different signals are needed in different locations, this should be modified
                    Dim InitialMargin As Integer = 0
                    Dim TestWordSound = Me.SpeechMaterialComponent.GetSound(Me.MediaSet, SelectedMediaIndex, 1, , ,, InitialMargin)

                    'Storing the initial margin
                    Me.TargetInitialMargins.Add(InitialMargin)

                    If TargetIndex = 0 Then
                        'Stores the length of the test word sound
                        TestWordLength = TestWordSound.WaveData.SampleData(1).Length
                    End If

                    Targets.Add(New Tuple(Of STFN.Core.Audio.Sound, SoundSourceLocation)(TestWordSound, Me.TargetStimulusLocations(TargetIndex)))
                Next

                'Not actually duplicating the targets for BMLD
                If IsBmldTrial = True Then
                    'The same signal will already be in all Targets indices (see above)
                    ''If changed the following code could be used
                    ''Copying signal 1 to signal 2, making them identical
                    'Array.Copy(Targets(0).Item1.WaveData.SampleData(1), Targets(1).Item1.WaveData.SampleData(1), Targets(0).Item1.WaveData.SampleData(1).Length)
                End If

                'Calculating test word start sample, syncronized with the centre of the maskers
                Dim TestWordStartSample As Integer = MaskersStartSample + MaskersLength / 2 - TestWordLength / 2

                'Calculating test word start time
                TestWordStartTime = TestWordStartSample / CurrentSampleRate

                'Calculates the offset of the test word sound
                Dim TestWordCompletedSample As Integer = TestWordStartSample + TestWordLength

                'Calculates and stores the TestWordCompletedTime 
                TestWordCompletedTime = TestWordCompletedSample / CurrentSampleRate

                'Sets a total trial sound length
                Dim TrialSoundLength As Integer = TrialSoundMaxDuration * CurrentSampleRate

                'Getting a background non-speech sound, and copies random sections of it into two sounds
                Dim BackgroundNonSpeech_Sound As STFN.Core.Audio.Sound = Me.SpeechMaterialComponent.GetBackgroundNonspeechSound(Me.MediaSet, 0)
                Dim Backgrounds As New List(Of Tuple(Of STFN.Core.Audio.Sound, SoundSourceLocation, Integer)) ' N.B. The Item3 holds the start sample of the section taken out of the source sound
                Dim NumberOfBackgrounds As Integer = BackgroundLocations.Length
                For BackgroundIndex = 0 To NumberOfBackgrounds - 1
                    'Getting a background sound and location
                    'TODO: we should make sure the start time for copying the sounds here differ by several seconds (and can be kept exactly the same for BMLD testing) 
                    Dim BackgroundStartSample As Integer = SipMeasurementRandomizer.Next(0, BackgroundNonSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2)
                    Backgrounds.Add(New Tuple(Of STFN.Core.Audio.Sound, SoundSourceLocation, Integer)(
                            BackgroundNonSpeech_Sound.CopySection(1, BackgroundStartSample, TrialSoundLength),
                            BackgroundLocations(BackgroundIndex), BackgroundStartSample))
                Next

                'Superpositioning the backgrounds for BMLD
                If IsBmldTrial = True Then

                    Dim TempBackgroundSoundList As New List(Of Sound)
                    For Each Background In Backgrounds
                        TempBackgroundSoundList.Add(Background.Item1)
                    Next
                    Dim SuperPositionedBackgroundSounds = DSP.SuperpositionEqualLengthSounds(TempBackgroundSoundList)

                    'Replacing the individual background sounds with the superpositioned on, to get the same sound mix in all background sources
                    For bi = 0 To Backgrounds.Count - 1
                        'Copying background 1 to background 2, making them identical
                        Array.Copy(SuperPositionedBackgroundSounds.WaveData.SampleData(1), Backgrounds(bi).Item1.WaveData.SampleData(1), SuperPositionedBackgroundSounds.WaveData.SampleData(1).Length)
                    Next

                End If

                'Getting a background speech sound, if needed, and copies a random section of it into a single sound. The (random) start sample is stored in Item3, for export to sound files.
                Dim BackgroundSpeechSelections As New List(Of Tuple(Of STFN.Core.Audio.Sound, SoundSourceLocation, Integer))
                'For now skipping with BMLD
                If UseBackgroundSpeech = True Then
                    Dim BackgroundSpeech_Sound As STFN.Core.Audio.Sound = Me.SpeechMaterialComponent.GetBackgroundSpeechSound(Me.MediaSet, 0)
                    For TargetIndex = 0 To NumberOfTargets - 1
                        'TODO: we should make sure the start time for copying the sounds here differ by several seconds (and can be kept exactly the same for BMLD testing) 
                        Dim StartSample As Integer = SipMeasurementRandomizer.Next(0, BackgroundSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2)
                        Dim CurrentBackgroundSpeechSelection = BackgroundSpeech_Sound.CopySection(1, StartSample, TrialSoundLength)
                        BackgroundSpeechSelections.Add(New Tuple(Of STFN.Core.Audio.Sound, SoundSourceLocation, Integer)(CurrentBackgroundSpeechSelection, Me.TargetStimulusLocations(TargetIndex), StartSample))
                    Next

                    'Duplicating the BackgroundSpeechSelections for BMLD
                    If IsBmldTrial = True Then
                        'Copying signal 1 to signal 2, making them identical
                        Array.Copy(BackgroundSpeechSelections(0).Item1.WaveData.SampleData(1), BackgroundSpeechSelections(1).Item1.WaveData.SampleData(1), BackgroundSpeechSelections(0).Item1.WaveData.SampleData(1).Length)
                    End If
                End If

                'Sets up fading specifications for the test word
                Dim FadeSpecs_TestWord = New List(Of DSP.FadeSpecifications)
                FadeSpecs_TestWord.Add(New DSP.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.002))
                FadeSpecs_TestWord.Add(New DSP.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.002))

                'Sets up fading specifications for the maskers
                Dim FadeSpecs_Maskers = New List(Of DSP.FadeSpecifications)
                If SkipNoiseFadeIn = False Then
                    FadeSpecs_Maskers.Add(New DSP.FadeSpecifications(Nothing, 0, 0, MaskerFadeInLength))
                End If
                If SkipNoiseFadeOut = False Then
                    FadeSpecs_Maskers.Add(New DSP.FadeSpecifications(0, Nothing, -MaskerFadeOutLength))
                End If

                'Sets up fading specifications for the background signals
                Dim FadeSpecs_Background = New List(Of DSP.FadeSpecifications)
                If SkipNoiseFadeIn = False Then
                    FadeSpecs_Background.Add(New DSP.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.01))
                End If
                If SkipNoiseFadeOut = False Then
                    FadeSpecs_Background.Add(New DSP.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.01))
                End If

                'Sets up ducking specifications for the background (non-speech) signals
                Dim DuckSpecs_BackgroundNonSpeech = New List(Of DSP.FadeSpecifications)
                'BackgroundNonSpeechDucking = Math.Max(0, Me.MediaSet.BackgroundNonspeechRealisticLevel - Math.Min(Me.TargetMasking_SPL.Value - 3, Me.MediaSet.BackgroundNonspeechRealisticLevel))
                BackgroundNonSpeechDucking = Math.Max(0, Me.MediaSet.BackgroundNonspeechRealisticLevel - Math.Min(Me.TargetMasking_SPL.Value - 6, Me.MediaSet.BackgroundNonspeechRealisticLevel)) ' Ducking 6 dB instead of 3
                'Dim BackgroundStartDuckSample As Integer = Math.Max(0, TestWordStartSample - CurrentSampleRate * 0.5)
                Dim BackgroundStartDuckSample As Integer = Math.Max(0, TestWordStartSample - CurrentSampleRate * 1) ' Extending ducking fade to 1 sec before and after test word, to get a smoother fade
                Dim BackgroundDuckFade1StageLength As Integer = TestWordStartSample - BackgroundStartDuckSample
                DuckSpecs_BackgroundNonSpeech.Add(New DSP.FadeSpecifications(0, BackgroundNonSpeechDucking, BackgroundStartDuckSample, BackgroundDuckFade1StageLength))
                'Dim BackgroundDuckFade2StageLength As Integer = Math.Min(CurrentSampleRate * 0.5, (TrialSoundLength - TestWordCompletedSample - 2))
                Dim BackgroundDuckFade2StageLength As Integer = Math.Min(CurrentSampleRate * 1, (TrialSoundLength - TestWordCompletedSample - 2))
                DuckSpecs_BackgroundNonSpeech.Add(New DSP.FadeSpecifications(BackgroundNonSpeechDucking, 0, TestWordCompletedSample, BackgroundDuckFade2StageLength))

                'Sets up ducking specifications for the background (speech) signals
                Dim DuckSpecs_BackgroundSpeech = New List(Of DSP.FadeSpecifications)
                Dim BGS_DuckFadeOutStart As Integer = Math.Max(0, TestWordStartSample - CurrentSampleRate * 1)
                Dim BGS_DuckFadeOutlength As Integer = Math.Max(0, TestWordStartSample - BGS_DuckFadeOutStart - CurrentSampleRate * 0.5)
                DuckSpecs_BackgroundSpeech.Add(New DSP.FadeSpecifications(0, Nothing, BGS_DuckFadeOutStart, BGS_DuckFadeOutlength))
                Dim BGS_DuckFadeInStart As Integer = Math.Min(TestWordCompletedSample + CurrentSampleRate * 0.5, TrialSoundLength)
                Dim BGS_DuckFadeInLength As Integer = Math.Min(CurrentSampleRate * 0.5, (TrialSoundLength - BGS_DuckFadeInStart - 2))
                DuckSpecs_BackgroundSpeech.Add(New DSP.FadeSpecifications(Nothing, 0, BGS_DuckFadeInStart, BGS_DuckFadeInLength))

                'Adds the test word signal, with fade and location specifications
                Dim LevelGroup As Integer = 1 ' The level group value is used to set the added sound level of items sharing the same (arbitrary) LevelGroup value to the indicated sound level. (Thus, the sounds with the same LevelGroup value are measured together.)
                For TargetIndex = 0 To Targets.Count - 1
                    ItemList.Add(New SoundSceneItem(Targets(TargetIndex).Item1, 1, Me.TestWordLevel, LevelGroup, Targets(TargetIndex).Item2, SoundSceneItem.SoundSceneItemRoles.Target, TestWordStartSample,,,, FadeSpecs_TestWord))
                    'Incrementing LevelGroup if ears should be measured separately as in BMLD. (But leaving the last item since it is incremented below)
                    If IsBmldTrial = True And TargetIndex < Targets.Count - 1 Then LevelGroup += 1
                Next
                LevelGroup += 1

                Dim ForceSkipMaskers As Boolean = False
                If ForceSkipMaskers = False Then
                    'Adds the Maskers, with fade and location specifications
                    For MaskerIndex = 0 To Maskers.Count - 1
                        ItemList.Add(New SoundSceneItem(Maskers(MaskerIndex).Item1, 1, Me.TargetMasking_SPL, LevelGroup, Maskers(MaskerIndex).Item2, SoundSceneItem.SoundSceneItemRoles.Masker, MaskersStartSample, MaskersStartMeasureSample, MaskersStartMeasureLength,, FadeSpecs_Maskers))
                        'Incrementing LevelGroup if ears should be measured separately as in BMLD. (But leaving the last item since it is incremented below)
                        If IsBmldTrial = True And MaskerIndex < Maskers.Count - 1 Then LevelGroup += 1
                    Next
                    LevelGroup += 1
                End If

                'Adds the background (non-speech) signals, with fade, duck and location specifications
                For BackgroundIndex = 0 To Backgrounds.Count - 1
                    ItemList.Add(New SoundSceneItem(Backgrounds(BackgroundIndex).Item1, 1, Me.MediaSet.BackgroundNonspeechRealisticLevel, LevelGroup, Backgrounds(BackgroundIndex).Item2, SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech, 0,,,, FadeSpecs_Background, DuckSpecs_BackgroundNonSpeech))
                    'Incrementing LevelGroup if ears should be measured separately as in BMLD. (But leaving the last item since it is incremented below)
                    If IsBmldTrial = True Then
                        If BackgroundIndex < Backgrounds.Count - 2 Then
                            'Incrementing LevelGroup if we have a new location, based on the azimuth only (i.e. the other ear.) TODO: This is a bad solution, since backgrounds necessarily have to be ordered from one ear to the other, and never mixed!
                            If Backgrounds(BackgroundIndex).Item2.HorizontalAzimuth <> Backgrounds(BackgroundIndex + 1).Item2.HorizontalAzimuth Then
                                LevelGroup += 1
                            End If
                        End If
                    End If
                Next
                LevelGroup += 1

                'Adds the background (speech) signal, with fade, duck and location specifications
                If UseBackgroundSpeech = True Then
                    For TargetIndex = 0 To BackgroundSpeechSelections.Count - 1
                        'TODO: Here LevelGroup needs to be incremented if ears are measured separately as in BMLD! Possibly also on other similar places, as with the noise...
                        ItemList.Add(New SoundSceneItem(BackgroundSpeechSelections(TargetIndex).Item1, 1, Me.ContextRegionSpeech_SPL, LevelGroup, BackgroundSpeechSelections(TargetIndex).Item2, SoundSceneItem.SoundSceneItemRoles.BackgroundSpeech, 0,,,, FadeSpecs_Background, DuckSpecs_BackgroundSpeech))
                        'Incrementing LevelGroup if ears should be measured separately as in BMLD. (But leaving the last item since it is incremented below)
                        If IsBmldTrial = True And TargetIndex < BackgroundSpeechSelections.Count - 1 Then LevelGroup += 1
                    Next
                    LevelGroup += 1
                End If


                MixStopWatch.Stop()
                If STFN.Core.SipTest.LogToConsole = True Then Console.WriteLine("Prepared sounds in " & MixStopWatch.ElapsedMilliseconds & " ms.")
                MixStopWatch.Restart()

                Dim MixedTestTrialSound As STFN.Core.Audio.Sound

                If ParentTestUnit.ParentMeasurement.ExportTrialSoundFiles = True Or Me.IsBmldTrial = True Then

                    'Creating the mix directly by calling CreateSoundScene of the current Mixer and exporting the sound for comparison with the separately exported sounds below
                    Dim ExportAllItemTypes As Boolean = False
                    If ExportAllItemTypes = True Then
                        If Me.IsBmldTrial = True Then Throw New NotImplementedException("Exporting the final mix directly is not supported with BMLD trials.")
                        Dim TempMixedTestTrialSound = SelectedTransducer.Mixer.CreateSoundScene(ItemList, UseNominalLevels, False, Me.SoundPropagationType, SelectedTransducer.LimiterThreshold)
                        TrialSoundsToExport.Add(New Tuple(Of String, STFN.Core.Audio.Sound)("OriginalMix", TempMixedTestTrialSound))
                    End If

                    'Creating separate sound files for target, maskers, and the final mix
                    Dim Target_Items As New List(Of SoundSceneItem)
                    Dim Masker_Items As New List(Of SoundSceneItem)
                    Dim BackgroundNonspeech_Items As New List(Of SoundSceneItem)
                    Dim BackgroundSpeech_Items As New List(Of SoundSceneItem)
                    Dim Nontarget_Items As New List(Of SoundSceneItem)

                    For Each Item In ItemList

                        If Item.Role = SoundSceneItem.SoundSceneItemRoles.Target Then
                            'Collecting the target/s
                            Target_Items.Add(Item)
                        Else
                            'Collecting the non targets
                            Nontarget_Items.Add(Item)
                        End If

                        If ExportAllItemTypes = True Then
                            Select Case Item.Role
                                Case SoundSceneItem.SoundSceneItemRoles.Masker
                                    'Collecting the maskers/s
                                    Masker_Items.Add(Item)
                                Case SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech
                                    'Collecting the BackgroundNonspeech item/s
                                    BackgroundNonspeech_Items.Add(Item)
                                Case SoundSceneItem.SoundSceneItemRoles.BackgroundSpeech
                                    'Collecting the BackgroundSpeech item/s
                                    BackgroundSpeech_Items.Add(Item)
                            End Select
                        End If

                    Next

                    'Creating the target mix by calling CreateSoundScene of the current Mixer
                    Dim TargetSound = SelectedTransducer.Mixer.CreateSoundScene(Target_Items, UseNominalLevels, False, Me.SoundPropagationType, SelectedTransducer.LimiterThreshold)

                    Dim MaskerItemsSound As STFN.Core.Audio.Sound = Nothing
                    If Masker_Items.Count > 0 Then
                        MaskerItemsSound = SelectedTransducer.Mixer.CreateSoundScene(Masker_Items, UseNominalLevels, False, Me.SoundPropagationType, SelectedTransducer.LimiterThreshold)
                    End If

                    Dim BackgroundNonspeechItemsSound As STFN.Core.Audio.Sound = Nothing
                    If BackgroundNonspeech_Items.Count > 0 Then
                        BackgroundNonspeechItemsSound = SelectedTransducer.Mixer.CreateSoundScene(BackgroundNonspeech_Items, UseNominalLevels, False, Me.SoundPropagationType, SelectedTransducer.LimiterThreshold)
                    End If

                    Dim BackgroundSpeechItemsSound As STFN.Core.Audio.Sound = Nothing
                    If BackgroundSpeech_Items.Count > 0 Then
                        BackgroundSpeechItemsSound = SelectedTransducer.Mixer.CreateSoundScene(BackgroundSpeech_Items, UseNominalLevels, False, Me.SoundPropagationType, SelectedTransducer.LimiterThreshold)
                    End If

                    'Creating the non-target mix by calling CreateSoundScene of the current Mixer
                    Dim NontargetSound = SelectedTransducer.Mixer.CreateSoundScene(Nontarget_Items, UseNominalLevels, False, Me.SoundPropagationType, SelectedTransducer.LimiterThreshold)

                    'If, BMLD trial, applying frontal left ear only room simulation,phase inverting the appropriate sounds
                    If Me.IsBmldTrial = True Then

                        'Attains a copy of the appropriate directional FIR-filter kernel
                        Dim FrontalTwoLeftEarsKernel = OstfBase.DirectionalSimulator.GetStereoKernel(OstfBase.DirectionalSimulator.SelectedDirectionalSimulationSetName, 0, 0, 3).TwoLeftEarsKernel
                        'Dim SelectedSimulationKernel = DirectionalSimulator.GetStereoKernel(ImpulseReponseSetName, SoundSceneItem.SourceLocation.HorizontalAzimuth, SoundSceneItem.SourceLocation.Elevation, SoundSceneItem.SourceLocation.Distance)
                        Dim CurrentKernel = FrontalTwoLeftEarsKernel.BinauralIR.CreateSoundDataCopy
                        Dim SelectedActualPoint = FrontalTwoLeftEarsKernel.Point
                        Dim SelectedActualBinauralDelay = FrontalTwoLeftEarsKernel.BinauralDelay

                        'Applies FIR-filtering
                        Dim MyFftFormat As STFN.Core.Audio.Formats.FftFormat = New Formats.FftFormat
                        TargetSound = DSP.FIRFilter(TargetSound, CurrentKernel, MyFftFormat,,,,,, True)
                        If MaskerItemsSound IsNot Nothing Then MaskerItemsSound = DSP.FIRFilter(MaskerItemsSound, CurrentKernel, MyFftFormat,,,,,, True)
                        If BackgroundNonspeechItemsSound IsNot Nothing Then BackgroundNonspeechItemsSound = DSP.FIRFilter(BackgroundNonspeechItemsSound, CurrentKernel, MyFftFormat,,,,,, True)
                        If BackgroundSpeechItemsSound IsNot Nothing Then BackgroundSpeechItemsSound = DSP.FIRFilter(BackgroundSpeechItemsSound, CurrentKernel, MyFftFormat,,,,,, True)
                        NontargetSound = DSP.FIRFilter(NontargetSound, CurrentKernel, MyFftFormat,,,,,, True)

                        'Storing the actual location values used in the simulation
                        For locationIndex = 0 To Me.TargetStimulusLocations.Length - 1
                            TargetStimulusLocations(locationIndex).ActualLocation = New SoundSourceLocation
                            TargetStimulusLocations(locationIndex).ActualLocation.HorizontalAzimuth = SelectedActualPoint.GetSphericalAzimuth
                            TargetStimulusLocations(locationIndex).ActualLocation.Elevation = SelectedActualPoint.GetSphericalElevation
                            TargetStimulusLocations(locationIndex).ActualLocation.Distance = SelectedActualPoint.GetSphericalDistance
                            TargetStimulusLocations(locationIndex).ActualLocation.BinauralDelay = FrontalTwoLeftEarsKernel.BinauralDelay
                        Next
                        For locationIndex = 0 To Me.MaskerLocations.Length - 1
                            MaskerLocations(locationIndex).ActualLocation = New SoundSourceLocation
                            MaskerLocations(locationIndex).ActualLocation.HorizontalAzimuth = SelectedActualPoint.GetSphericalAzimuth
                            MaskerLocations(locationIndex).ActualLocation.Elevation = SelectedActualPoint.GetSphericalElevation
                            MaskerLocations(locationIndex).ActualLocation.Distance = SelectedActualPoint.GetSphericalDistance
                            MaskerLocations(locationIndex).ActualLocation.BinauralDelay = FrontalTwoLeftEarsKernel.BinauralDelay
                        Next
                        For locationIndex = 0 To Me.BackgroundLocations.Length - 1
                            BackgroundLocations(locationIndex).ActualLocation = New SoundSourceLocation
                            BackgroundLocations(locationIndex).ActualLocation.HorizontalAzimuth = SelectedActualPoint.GetSphericalAzimuth
                            BackgroundLocations(locationIndex).ActualLocation.Elevation = SelectedActualPoint.GetSphericalElevation
                            BackgroundLocations(locationIndex).ActualLocation.Distance = SelectedActualPoint.GetSphericalDistance
                            BackgroundLocations(locationIndex).ActualLocation.BinauralDelay = FrontalTwoLeftEarsKernel.BinauralDelay
                        Next

                        'Phase inverting if needed
                        If Me.BmldSignalMode = BmldModes.BinauralPhaseInverted Then
                            'Phase inverting signal, by inverting the right channel
                            TargetSound.InvertChannel(2)

                            'Inverting also the other sounds if they exist (for export)
                            If BackgroundSpeechItemsSound IsNot Nothing Then BackgroundSpeechItemsSound.InvertChannel(2)

                        End If

                        If Me.BmldNoiseMode = BmldModes.BinauralPhaseInverted Then
                            'Phase inverting noise, by inverting the right channel
                            NontargetSound.InvertChannel(2)

                            'Inverting also the other sounds if they exist (for export)
                            If MaskerItemsSound IsNot Nothing Then MaskerItemsSound.InvertChannel(2)
                            If BackgroundNonspeechItemsSound IsNot Nothing Then BackgroundNonspeechItemsSound.InvertChannel(2)

                        End If
                    End If

                    'Creating the combined mix
                    MixedTestTrialSound = DSP.SuperpositionSounds({TargetSound, NontargetSound}.ToList)

                    'Stores the sounds in TrialSoundsToExport, to be exported later.

                    'Add target and masker sound file indices, as well as background start samples (i.e. the first samples at which each file was read from the source background sould file)
                    SelectedTargetIndexString = SelectedMediaIndex
                    SelectedMaskerIndicesString = String.Join("_", SelectedMaskerIndices)
                    BackgroundStartSamplesString = ""
                    Dim BackgroundStartSamplesList As New List(Of Integer)
                    For Each Background In Backgrounds
                        BackgroundStartSamplesList.Add(Background.Item3)
                    Next
                    BackgroundStartSamplesString = String.Join("_", BackgroundStartSamplesList)

                    BackgroundSpeechStartSamplesString = ""
                    Dim BackgroundSpeechSectionSampleList As New List(Of Integer)
                    For Each BackgroundSpeechSection In BackgroundSpeechSelections
                        BackgroundSpeechSectionSampleList.Add(BackgroundSpeechSection.Item3)
                    Next
                    If BackgroundSpeechSectionSampleList.Count > 0 Then
                        BackgroundSpeechStartSamplesString = String.Join("_", BackgroundSpeechSectionSampleList)
                    End If

                    'Storing the sounds
                    TrialSoundsToExport.Add(New Tuple(Of String, STFN.Core.Audio.Sound)("TargetSound", TargetSound))
                    If MaskerItemsSound IsNot Nothing Then TrialSoundsToExport.Add(New Tuple(Of String, STFN.Core.Audio.Sound)("MaskersSound", MaskerItemsSound))
                    If BackgroundNonspeechItemsSound IsNot Nothing Then TrialSoundsToExport.Add(New Tuple(Of String, STFN.Core.Audio.Sound)("BackgroundNonspeechSound", BackgroundNonspeechItemsSound))
                    If BackgroundSpeechItemsSound IsNot Nothing Then TrialSoundsToExport.Add(New Tuple(Of String, STFN.Core.Audio.Sound)("BackgroundSpeechSound", BackgroundSpeechItemsSound))
                    TrialSoundsToExport.Add(New Tuple(Of String, STFN.Core.Audio.Sound)("NontargetSound", NontargetSound))
                    TrialSoundsToExport.Add(New Tuple(Of String, STFN.Core.Audio.Sound)("MixedTestTrialSound", MixedTestTrialSound))

                    'Empties the TrialSoundsToExport if no trial sounds should be exported
                    If ParentTestUnit.ParentMeasurement.ExportTrialSoundFiles = False Then TrialSoundsToExport.Clear()

                Else

                    'Creating the mix by calling CreateSoundScene of the current Mixer
                    MixedTestTrialSound = SelectedTransducer.Mixer.CreateSoundScene(ItemList, UseNominalLevels, False, Me.SoundPropagationType, SelectedTransducer.LimiterThreshold)
                End If

                'Getting the applied gain for all items
                GainList.Clear()
                GainList.Add(SoundSceneItem.SoundSceneItemRoles.Target, New List(Of String))
                GainList.Add(SoundSceneItem.SoundSceneItemRoles.Masker, New List(Of String))
                GainList.Add(SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech, New List(Of String))
                GainList.Add(SoundSceneItem.SoundSceneItemRoles.BackgroundSpeech, New List(Of String))
                For Each Item In ItemList
                    If Item.AppliedGain IsNot Nothing Then
                        If Item.AppliedGain.Value.HasValue Then
                            GainList(Item.Role).Add(Math.Round(Item.AppliedGain.Value.Value, 1))
                        Else
                            GainList(Item.Role).Add("NA")
                        End If
                    End If
                Next

                If STFN.Core.SipTest.LogToConsole = True Then Console.WriteLine("Mixed sound in " & MixStopWatch.ElapsedMilliseconds & " ms.")

                'TODO: Here we can simulate and/or compensate for hearing loss:
                'SimulateHearingLoss,
                'CompensateHearingLoss

                Sound = MixedTestTrialSound

                'Mixing non-presented trials as well
                If PseudoTrials IsNot Nothing Then
                    For Each NonpresentedAlternativeTrial In PseudoTrials
                        NonpresentedAlternativeTrial.MixSound(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, FixedMaskerIndices, FixedSpeechIndex)
                    Next
                End If

                'Stores the testword start sample
                Me.TargetStartSample = TestWordStartSample

            Catch ex As Exception
                Logging.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
            End Try

        End Sub




        Public Sub ExportTrialResult(ByVal IncludeHeadings As Boolean, ByVal SkipExportOfSoundFiles As Boolean)

            Dim FilePath = IO.Path.Combine(Me.ParentTestUnit.ParentMeasurement.TrialResultsExportFolder, "TestResults")

            Dim OutputLines As New List(Of String)

            If IncludeHeadings = True Then
                OutputLines.Add(SiPTestMeasurementHistory.NewMeasurementMarker)
                OutputLines.Add(SipTrial.CreateExportHeadings())
            End If

            Dim TrialExportString = Me.CreateExportString(SkipExportOfSoundFiles)

            OutputLines.Add(TrialExportString)

            Logging.SendInfoToLog(String.Join(vbCrLf, OutputLines), IO.Path.GetFileNameWithoutExtension(FilePath), IO.Path.GetDirectoryName(FilePath), True, True)

        End Sub




        Private _EstimatedSuccessProbability As Double? = Nothing

        Public ReadOnly Property EstimatedSuccessProbability(ByVal ReCalculate As Boolean) As Double
            Get
                If _EstimatedSuccessProbability.HasValue = False Or ReCalculate = True Then
                    UpdateEstimatedSuccessProbability()
                End If
                Return _EstimatedSuccessProbability
            End Get
        End Property

        Public Sub OverideEstimatedSuccessProbabilityValue(ByVal Newvalue As Double)
            _EstimatedSuccessProbability = Newvalue
        End Sub

        Public Sub UpdateEstimatedSuccessProbability()

            'Select Case Me.ParentTestUnit.ParentMeasurement.Prediction.Models.SelectedModel.GetSwedishSipTestA

            'Getting predictors
            Dim PDL As Double = Me.PhonemeDiscriminabilityLevel

            'TODO! Decide whether exact duration of the presented phoneme should be used (slower, but as in Witte,2021) or if average test phoneme duration (for the five exemplar recordings of each word) should be used (a little faster)
            Dim DurationList = Me.SpeechMaterialComponent.GetDurationOfContrastingComponents(Me.MediaSet, SpeechMaterialComponent.LinguisticLevels.Phoneme, Me.SelectedMediaIndex, 1)
            Dim TPD As Double = DurationList(0)
            'Dim TPD As Double = Me.SpeechMaterial.GetNumericMediaSetVariableValue(MediaSet, "Tc")

            Dim Z As Double = Me.SpeechMaterialComponent.GetNumericVariableValue("Z")
            Dim iPNDP As Double = 1 / Me.SpeechMaterialComponent.GetNumericVariableValue("PNDP")
            Dim PP As Double = Me.SpeechMaterialComponent.GetNumericVariableValue("PP")
            Dim PT As Double = Me.SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).GetNumericVariableValue("V")

            'Calculating centred and scaled values for PDL and PBTAH
            Dim PDL_gmc_div = (PDL - 10.46) / 50
            Dim Z_gmc_div = (Z - 3.7) / 10
            Dim iPNDP_gmc_div = (iPNDP - 20.6) / 50

            'Calculating model eta
            Dim Eta = 0.73 +
                8.22 * PDL_gmc_div +
                5.11 * (TPD - 0.33) +
                4.24 * Z_gmc_div -
                1.44 * iPNDP_gmc_div +
                4.58 * (PP - 0.92) -
                1.1 * (PT - 0.43) -
                7.25 * PDL_gmc_div * Z_gmc_div +
                7.47 * PDL_gmc_div * iPNDP_gmc_div +
                3.32 * PDL_gmc_div * (PT - 0.43)

            'Calculating estimated success probability
            Dim p As Double = 1 / 3 + (2 / 3) * (1 / (1 + Math.Exp(-Eta)))

            If STFN.Core.SipTest.LogToConsole = True Then
                Dim LogList As New List(Of String)
                LogList.Add("PrimaryStringRepresentation:" & vbTab & Me.SpeechMaterialComponent.PrimaryStringRepresentation)
                LogList.Add("Reference_SPL:" & vbTab & Reference_SPL)
                LogList.Add("PNR:" & vbTab & PNR)
                LogList.Add("PDL:" & vbTab & PDL)
                LogList.Add("TPD:" & vbTab & TPD)
                LogList.Add("Z:" & vbTab & Z)
                LogList.Add("iPNDP:" & vbTab & iPNDP)
                LogList.Add("PP:" & vbTab & PP)
                LogList.Add("PT:" & vbTab & PT)
                LogList.Add("PDL_gmc_div:" & vbTab & PDL_gmc_div)
                LogList.Add("Z_gmc_div:" & vbTab & Z_gmc_div)
                LogList.Add("iPNDP_gmc_div:" & vbTab & iPNDP_gmc_div)
                LogList.Add("Eta:" & vbTab & Eta)
                LogList.Add("p:" & vbTab & p)

                Console.WriteLine(String.Join(vbCrLf, LogList))
            End If

            'Stores the result in PredictedSuccessProbability
            _EstimatedSuccessProbability = p

        End Sub

        Private _PhonemeDiscriminabilityLevel As Double? = Nothing

        Public ReadOnly Property PhonemeDiscriminabilityLevel(Optional ByVal ReCalculate As Boolean = True,
                                                              Optional ByVal SpeechSpectrumLevelsVariableNamePrefix As String = "SLs",
                                                              Optional ByVal MaskerSpectrumLevelsVariableNamePrefix As String = "SLm") As Double
            Get
                If _PhonemeDiscriminabilityLevel.HasValue = False Or ReCalculate = True Then
                    UpdatePhonemeDiscriminabilityLevel(SpeechSpectrumLevelsVariableNamePrefix, MaskerSpectrumLevelsVariableNamePrefix)
                End If
                Return _PhonemeDiscriminabilityLevel
            End Get
        End Property

        Public Sub SetPhonemeDiscriminabilityLevelExternally(ByVal Value As Double)
            _PhonemeDiscriminabilityLevel = Value
        End Sub

        Public Sub UpdatePhonemeDiscriminabilityLevel(Optional ByVal SpeechSpectrumLevelsVariableNamePrefix As String = "SLs",
                                                              Optional ByVal MaskerSpectrumLevelsVariableNamePrefix As String = "SLm")

            'Using thresholds and gain data from the side with the best aided thresholds (selecting side separately for each critical band)
            Dim Thresholds(DSP.SiiCriticalBands.CentreFrequencies.Length - 1) As Double
            Dim Gain(DSP.SiiCriticalBands.CentreFrequencies.Length - 1) As Double

            For i = 0 To DSP.SiiCriticalBands.CentreFrequencies.Length - 1
                'TODO: should we allow for the lack of gain data here, or should we always use a gain of zero when no hearing aid is used?
                Dim AidedThreshold_Left As Double = DirectCast(Me.ParentTestUnit.ParentMeasurement, SipMeasurement).SelectedAudiogramData.Cb_Left_AC(i) - DirectCast(Me.ParentTestUnit.ParentMeasurement, SipMeasurement).HearingAidGain.LeftSideGain(i).Gain
                Dim AidedThreshold_Right As Double = DirectCast(Me.ParentTestUnit.ParentMeasurement, SipMeasurement).SelectedAudiogramData.Cb_Right_AC(i) - DirectCast(Me.ParentTestUnit.ParentMeasurement, SipMeasurement).HearingAidGain.RightSideGain(i).Gain

                If AidedThreshold_Left < AidedThreshold_Right Then
                    Thresholds(i) = DirectCast(Me.ParentTestUnit.ParentMeasurement, SipMeasurement).SelectedAudiogramData.Cb_Left_AC(i)
                    Gain(i) = DirectCast(Me.ParentTestUnit.ParentMeasurement, SipMeasurement).HearingAidGain.LeftSideGain(i).Gain
                Else
                    Thresholds(i) = DirectCast(Me.ParentTestUnit.ParentMeasurement, SipMeasurement).SelectedAudiogramData.Cb_Right_AC(i)
                    Gain(i) = DirectCast(Me.ParentTestUnit.ParentMeasurement, SipMeasurement).HearingAidGain.RightSideGain(i).Gain
                End If
            Next

            'Getting spectral levels
            Dim CorrectResponseSpectralLevels(DSP.SiiCriticalBands.CentreFrequencies.Length - 1) As Double
            Dim MaskerSpectralLevels(DSP.SiiCriticalBands.CentreFrequencies.Length - 1) As Double
            Dim Siblings = Me.SpeechMaterialComponent.GetSiblingsExcludingSelf
            Dim IncorrectResponsesSpectralLevels As New List(Of Double())
            For Each Sibling In Siblings
                Dim IncorrectResponseSpectralLevels(DSP.SiiCriticalBands.CentreFrequencies.Length - 1) As Double
                IncorrectResponsesSpectralLevels.Add(IncorrectResponseSpectralLevels)
            Next

            'Calculating the difference between the standard ReferenceSpeechMaterialLevel_SPL (68.34 dB SPL) reference level and the one currently used
            Dim RefLevelDifference As Double = Me.Reference_SPL - ReferenceSpeechMaterialLevel_SPL

            'Getting the current gain, compared to the reference test-word and masker levels
            Dim CurrentSpeechGain As Double = TestWordLevel - ReferenceTestWordLevel_SPL + RefLevelDifference ' SpeechMaterial.GetNumericMediaSetVariableValue(MediaSet, "Lc") 'TestStimulus.TestWord_ReferenceSPL
            If STFN.Core.SipTest.LogToConsole = True Then Console.WriteLine(Me.SpeechMaterialComponent.PrimaryStringRepresentation & " PNR: " & Me.PNR & " SpeechGain : " & CurrentSpeechGain)

            Dim CurrentMaskerGain As Double = TargetMasking_SPL - ReferenceContrastingPhonemesLevel_SPL + RefLevelDifference 'Audio.Standard_dBFS_To_dBSPL(Common.SipTestReferenceMaskerLevel_FS) 'TODO: In the SiP-test maskers sound files the level is set to -30 dB FS across the whole sounds. A more detailed sound level data could be used instead!
            If STFN.Core.SipTest.LogToConsole = True Then Console.WriteLine(Me.SpeechMaterialComponent.PrimaryStringRepresentation & " PNR: " & Me.PNR & " CurrentMaskerGain: " & CurrentMaskerGain)

            For i = 0 To DSP.SiiCriticalBands.CentreFrequencies.Length - 1
                Dim VariableNameSuffix = Math.Round(DSP.SiiCriticalBands.CentreFrequencies(i)).ToString("00000")
                Dim SLsName As String = SpeechSpectrumLevelsVariableNamePrefix & "_" & VariableNameSuffix
                Dim SLmName As String = MaskerSpectrumLevelsVariableNamePrefix & "_" & VariableNameSuffix

                'Retreiving the reference levels and adjusts them by the CurrentSpeechGain and CurrentMaskerGain
                CorrectResponseSpectralLevels(i) = Me.SpeechMaterialComponent.GetNumericMediaSetVariableValue(MediaSet, SLsName) + CurrentSpeechGain
                MaskerSpectralLevels(i) = Me.SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).GetNumericMediaSetVariableValue(MediaSet, SLmName) + CurrentMaskerGain
                For s = 0 To Siblings.Count - 1
                    IncorrectResponsesSpectralLevels(s)(i) = Siblings(s).GetNumericMediaSetVariableValue(MediaSet, SLsName) + CurrentSpeechGain
                Next
            Next

            Dim SRFM As Double?() = GetMLD(Nothing, 1.01) ' Cf Witte's Thesis for the value of c_factor

            'N.B. SRFM and SF 30 need to change if presented in other speaker azimuths!

            'Calculating SDRs
            Dim SDRt = PDL.CalculateSDR(CorrectResponseSpectralLevels, MaskerSpectralLevels, Thresholds, Gain, True, True, SRFM)
            Dim SDRcs As New List(Of Double())
            For s = 0 To Siblings.Count - 1
                Dim SDRc = PDL.CalculateSDR(IncorrectResponsesSpectralLevels(s), MaskerSpectralLevels, Thresholds, Gain, True, True, SRFM)
                SDRcs.Add(SDRc)
            Next

            If STFN.Core.SipTest.LogToConsole = True Then
                Dim LogList As New List(Of String)
                LogList.Add("PrimaryStringRepresentation:" & vbTab & Me.SpeechMaterialComponent.PrimaryStringRepresentation)
                LogList.Add("RefLevelDifference:" & vbTab & RefLevelDifference)
                LogList.Add("CurrentSpeechGain:" & vbTab & CurrentSpeechGain)
                LogList.Add("CurrentMaskerGain:" & vbTab & CurrentMaskerGain)

                LogList.Add("CentreFrequencies:" & vbTab & String.Join("; ", DSP.SiiCriticalBands.CentreFrequencies))
                LogList.Add("Thresholds:" & vbTab & String.Join("; ", Thresholds))
                LogList.Add("Gain:" & vbTab & String.Join("; ", Gain))

                LogList.Add("CorrectResponseSpectralLevels:" & vbTab & String.Join("; ", CorrectResponseSpectralLevels))
                For s = 0 To Siblings.Count - 1
                    LogList.Add("IncorrectResponsesSpectralLevels_" & s & ":" & vbTab & String.Join("; ", IncorrectResponsesSpectralLevels(s)))
                Next
                LogList.Add("MaskerSpectralLevels:" & vbTab & String.Join("; ", MaskerSpectralLevels))
                LogList.Add("SRFM:" & vbTab & String.Join("; ", SRFM))

                LogList.Add("SDRt:" & vbTab & String.Join("; ", SDRt))
                For s = 0 To Siblings.Count - 1
                    LogList.Add("SDRcs_" & s & ":" & vbTab & String.Join("; ", SDRcs(s)))
                Next

                Console.WriteLine(String.Join(vbCrLf, LogList))
            End If

            'Calculating PDL
            _PhonemeDiscriminabilityLevel = PDL.CalculatePDL(SDRt, SDRcs)

        End Sub


        ''' <summary>
        ''' This function adds both the base class export headings and enables addition of more export headings
        ''' </summary>
        ''' <returns></returns>
        Public Shared Shadows Function CreateExportHeadings() As String

            Dim Headings As New List(Of String)

            'The outcommented lines below are exported from the base class

            'Headings.Add("ParticipantID")
            'Headings.Add("MeasurementDateTime")
            'Headings.Add("Description")
            'Headings.Add("TestUnitIndex")
            'Headings.Add("TestUnitDescription")
            'Headings.Add("SpeechMaterialComponentID")
            'Headings.Add("TestWordGroup")
            'Headings.Add("MediaSetName")
            'Headings.Add("PresentationOrder")
            'Headings.Add("ReferenceSpeechMaterialLevel_SPL")
            'Headings.Add("ReferenceContrastingPhonemesLevel_SPL")
            'Headings.Add("Reference_SPL")
            'Headings.Add("PNR")
            'Headings.Add("TargetMasking_SPL")
            'Headings.Add("TestWordLevelLimit")
            'Headings.Add("ContextSpeechLimit")
            Headings.Add("EstimatedSuccessProbability")
            Headings.Add("AdjustedSuccessProbability")
            'Headings.Add("SoundPropagationType")

            'Headings.Add("IntendedTargetLocation_Distance")
            'Headings.Add("IntendedTargetLocation_HorizontalAzimuth")
            'Headings.Add("IntendedTargetLocation_Elevation")
            'Headings.Add("PresentedTargetLocation_Distance")
            'Headings.Add("PresentedTargetLocation_HorizontalAzimuth")
            'Headings.Add("PresentedTargetLocation_Elevation")
            'Headings.Add("PresentedTargetLocation_BinauralDelay_Left")
            'Headings.Add("PresentedTargetLocation_BinauralDelay_Right")

            'Headings.Add("IntendedMaskerLocations_Distance")
            'Headings.Add("IntendedMaskerLocations_HorizontalAzimuth")
            'Headings.Add("IntendedMaskerLocations_Elevation")
            'Headings.Add("PresentedMaskerLocations_Distance")
            'Headings.Add("PresentedMaskerLocations_HorizontalAzimuth")
            'Headings.Add("PresentedMaskerLocations_Elevation")
            'Headings.Add("PresentedMaskerLocation_BinauralDelay_Left")
            'Headings.Add("PresentedMaskerLocation_BinauralDelay_Right")

            'Headings.Add("IntendedBackgroundLocations_Distance")
            'Headings.Add("IntendedBackgroundLocations_HorizontalAzimuth")
            'Headings.Add("IntendedBackgroundLocations_Elevation")
            'Headings.Add("PresentedBackgroundLocations_Distance")
            'Headings.Add("PresentedBackgroundLocations_HorizontalAzimuth")
            'Headings.Add("PresentedBackgroundLocations_Elevation")
            'Headings.Add("PresentedBackgroundLocation_BinauralDelay_Left")
            'Headings.Add("PresentedBackgroundLocation_BinauralDelay_Right")

            'Headings.Add("IsBmldTrial")
            'Headings.Add("BmldNoiseMode")
            'Headings.Add("BmldSignalMode")

            'Headings.Add("Response")
            'Headings.Add("Result")
            'Headings.Add("Score")
            'Headings.Add("ResponseTime")
            'Headings.Add("ResponseAlternativeCount")
            'Headings.Add("IsTestTrial")
            Headings.Add("PhonemeDiscriminabilityLevel")

            'Headings.Add("PrimaryStringRepresentation")

            'Headings.Add("Spelling")
            'Headings.Add("SpellingAFC")
            'Headings.Add("Transcription")
            'Headings.Add("TranscriptionAFC")
            Headings.Add("PseudoTrialIds")
            Headings.Add("PseudoTrialSpellings")

            Headings.Add("TargetTrial_ExportedTrialSoundFiles")
            Headings.Add("PseudoTrials_ExportedTrialSoundFiles")

            'Headings.Add("TargetTrial_SelectedTargetSoundIndex")
            'Headings.Add("TargetTrial_SelectedMaskerSoundIndices")
            'Headings.Add("TargetTrial_BackgroundNonspeechSoundStartSamples")
            'Headings.Add("TargetTrial_BackgroundSpeechSoundStartSamples")

            Headings.Add("PseudoTrials_SelectedTargetSoundIndex")
            Headings.Add("PseudoTrials_SelectedMaskerSoundIndices")
            Headings.Add("PseudoTrials_BackgroundNonspeechSoundStartSamples")
            Headings.Add("PseudoTrials_BackgroundSpeechSoundStartSamples")

            'Headings.Add("TargetTrial_BackgroundNonSpeechDucking")
            Headings.Add("PseudoTrials_BackgroundNonSpeechDucking")

            'Headings.Add("TargetTrial_ContextRegionSpeech_SPL")
            'Headings.Add("TargetTrial_TestWordLevel")
            'Headings.Add("TargetTrial_ReferenceTestWordLevel_SPL")

            Headings.Add("PseudoTrials_ContextRegionSpeech_SPL")
            Headings.Add("PseudoTrials_TestWordLevel")
            Headings.Add("PseudoTrials_ReferenceTestWordLevel_SPL")

            'Headings.Add("TargetStartSample")
            Headings.Add("PseudoTrials_TargetStartSample")
            'Headings.Add("TestPhonemeStartSample")
            'Headings.Add("TestPhonemelength")
            Headings.Add("PseudoTrials_TP_StartSamples")
            Headings.Add("PseudoTrials_TP_Length")

            'Headings.Add("TargetTrial_Gains")
            Headings.Add("PseudoTrials_Gains")

            Return String.Join(vbTab, Headings)

        End Function

        ''' <summary>
        ''' This function adds both the base class export data and enables addition of more export data
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function CreateExportString(Optional ByVal SkipExportOfSoundFiles As Boolean = True) As String

            Dim TrialDataList As New List(Of String)

            MyBase.CreateExportString(SkipExportOfSoundFiles)

            'The outcommented lines below are exported from the base class

            'TrialDataList.Add(Me.ParentTestUnit.ParentMeasurement.ParticipantID)
            'TrialDataList.Add(Me.ParentTestUnit.ParentMeasurement.MeasurementDateTime.ToString(System.Globalization.CultureInfo.InvariantCulture))
            'TrialDataList.Add(Me.ParentTestUnit.ParentMeasurement.Description)
            'TrialDataList.Add(Me.ParentTestUnit.ParentMeasurement.GetParentTestUnitIndex(Me))
            'TrialDataList.Add(Me.ParentTestUnit.Description)
            'TrialDataList.Add(Me.SpeechMaterialComponent.Id)
            'TrialDataList.Add(Me.SpeechMaterialComponent.ParentComponent.PrimaryStringRepresentation)
            'TrialDataList.Add(Me.MediaSet.MediaSetName)
            'TrialDataList.Add(Me.PresentationOrder)
            'TrialDataList.Add(Me.ReferenceSpeechMaterialLevel_SPL)
            'TrialDataList.Add(Me.ReferenceContrastingPhonemesLevel_SPL)
            'TrialDataList.Add(Me.Reference_SPL)
            'TrialDataList.Add(Me.PNR)
            'If TargetMasking_SPL.HasValue = True Then
            '    TrialDataList.Add(TargetMasking_SPL)
            'Else
            '    TrialDataList.Add("NA")
            'End If
            'TrialDataList.Add(TestWordLevelLimit)
            'TrialDataList.Add(ContextSpeechLimit)

            If DirectCast(Me.ParentTestUnit.ParentMeasurement, SipMeasurement).SelectedAudiogramData IsNot Nothing Then
                TrialDataList.Add(Me.EstimatedSuccessProbability(False))
                TrialDataList.Add(Me.AdjustedSuccessProbability)
            Else
                TrialDataList.Add("No audiogram stored - cannot calculate")
                TrialDataList.Add("No audiogram stored - cannot calculate")
            End If
            'TrialDataList.Add(Me.SoundPropagationType.ToString)

            'If Me.TargetStimulusLocations.Length > 0 Then
            '    Dim Distances As New List(Of String)
            '    Dim HorizontalAzimuths As New List(Of String)
            '    Dim Elevations As New List(Of String)
            '    Dim ActualDistances As New List(Of String)
            '    Dim ActualHorizontalAzimuths As New List(Of String)
            '    Dim ActualElevations As New List(Of String)
            '    Dim ActualBinauralDelay_Left As New List(Of String)
            '    Dim ActualBinauralDelay_Right As New List(Of String)
            '    For i = 0 To Me.TargetStimulusLocations.Length - 1
            '        Distances.Add(Me.TargetStimulusLocations(i).Distance)
            '        HorizontalAzimuths.Add(Me.TargetStimulusLocations(i).HorizontalAzimuth)
            '        Elevations.Add(Me.TargetStimulusLocations(i).Elevation)
            '        If Me.TargetStimulusLocations(i).ActualLocation Is Nothing Then Me.TargetStimulusLocations(i).ActualLocation = New SoundSourceLocation
            '        ActualDistances.Add(Me.TargetStimulusLocations(i).ActualLocation.Distance)
            '        ActualHorizontalAzimuths.Add(Me.TargetStimulusLocations(i).ActualLocation.HorizontalAzimuth)
            '        ActualElevations.Add(Me.TargetStimulusLocations(i).ActualLocation.Elevation)
            '        ActualBinauralDelay_Left.Add(Me.TargetStimulusLocations(i).ActualLocation.BinauralDelay.LeftDelay)
            '        ActualBinauralDelay_Right.Add(Me.TargetStimulusLocations(i).ActualLocation.BinauralDelay.RightDelay)
            '    Next
            '    TrialDataList.Add(String.Join(";", Distances))
            '    TrialDataList.Add(String.Join(";", HorizontalAzimuths))
            '    TrialDataList.Add(String.Join(";", Elevations))
            '    TrialDataList.Add(String.Join(";", ActualDistances))
            '    TrialDataList.Add(String.Join(";", ActualHorizontalAzimuths))
            '    TrialDataList.Add(String.Join(";", ActualElevations))
            '    TrialDataList.Add(String.Join(";", ActualBinauralDelay_Left))
            '    TrialDataList.Add(String.Join(";", ActualBinauralDelay_Right))
            'Else
            '    For n = 1 To 8
            '        TrialDataList.Add("")
            '    Next
            'End If

            'If Me.MaskerLocations.Length > 0 Then
            '    Dim Distances As New List(Of String)
            '    Dim HorizontalAzimuths As New List(Of String)
            '    Dim Elevations As New List(Of String)
            '    Dim ActualDistances As New List(Of String)
            '    Dim ActualHorizontalAzimuths As New List(Of String)
            '    Dim ActualElevations As New List(Of String)
            '    Dim ActualBinauralDelay_Left As New List(Of String)
            '    Dim ActualBinauralDelay_Right As New List(Of String)
            '    For i = 0 To Me.MaskerLocations.Length - 1
            '        Distances.Add(Me.MaskerLocations(i).Distance)
            '        HorizontalAzimuths.Add(Me.MaskerLocations(i).HorizontalAzimuth)
            '        Elevations.Add(Me.MaskerLocations(i).Elevation)
            '        If Me.MaskerLocations(i).ActualLocation Is Nothing Then Me.MaskerLocations(i).ActualLocation = New SoundSourceLocation
            '        ActualDistances.Add(Me.MaskerLocations(i).ActualLocation.Distance)
            '        ActualHorizontalAzimuths.Add(Me.MaskerLocations(i).ActualLocation.HorizontalAzimuth)
            '        ActualElevations.Add(Me.MaskerLocations(i).ActualLocation.Elevation)
            '        ActualBinauralDelay_Left.Add(Me.MaskerLocations(i).ActualLocation.BinauralDelay.LeftDelay)
            '        ActualBinauralDelay_Right.Add(Me.MaskerLocations(i).ActualLocation.BinauralDelay.RightDelay)
            '    Next
            '    TrialDataList.Add(String.Join(";", Distances))
            '    TrialDataList.Add(String.Join(";", HorizontalAzimuths))
            '    TrialDataList.Add(String.Join(";", Elevations))
            '    TrialDataList.Add(String.Join(";", ActualDistances))
            '    TrialDataList.Add(String.Join(";", ActualHorizontalAzimuths))
            '    TrialDataList.Add(String.Join(";", ActualElevations))
            '    TrialDataList.Add(String.Join(";", ActualBinauralDelay_Left))
            '    TrialDataList.Add(String.Join(";", ActualBinauralDelay_Right))
            'Else
            '    For n = 1 To 8
            '        TrialDataList.Add("")
            '    Next
            'End If

            'If Me.BackgroundLocations.Length > 0 Then
            '    Dim Distances As New List(Of String)
            '    Dim HorizontalAzimuths As New List(Of String)
            '    Dim Elevations As New List(Of String)
            '    Dim ActualDistances As New List(Of String)
            '    Dim ActualHorizontalAzimuths As New List(Of String)
            '    Dim ActualElevations As New List(Of String)
            '    Dim ActualBinauralDelay_Left As New List(Of String)
            '    Dim ActualBinauralDelay_Right As New List(Of String)
            '    For i = 0 To Me.BackgroundLocations.Length - 1
            '        Distances.Add(Me.BackgroundLocations(i).Distance)
            '        HorizontalAzimuths.Add(Me.BackgroundLocations(i).HorizontalAzimuth)
            '        Elevations.Add(Me.BackgroundLocations(i).Elevation)
            '        If Me.BackgroundLocations(i).ActualLocation Is Nothing Then Me.BackgroundLocations(i).ActualLocation = New SoundSourceLocation
            '        ActualDistances.Add(Me.BackgroundLocations(i).ActualLocation.Distance)
            '        ActualHorizontalAzimuths.Add(Me.BackgroundLocations(i).ActualLocation.HorizontalAzimuth)
            '        ActualElevations.Add(Me.BackgroundLocations(i).ActualLocation.Elevation)
            '        ActualBinauralDelay_Left.Add(Me.BackgroundLocations(i).ActualLocation.BinauralDelay.LeftDelay)
            '        ActualBinauralDelay_Right.Add(Me.BackgroundLocations(i).ActualLocation.BinauralDelay.RightDelay)
            '    Next
            '    TrialDataList.Add(String.Join(";", Distances))
            '    TrialDataList.Add(String.Join(";", HorizontalAzimuths))
            '    TrialDataList.Add(String.Join(";", Elevations))
            '    TrialDataList.Add(String.Join(";", ActualDistances))
            '    TrialDataList.Add(String.Join(";", ActualHorizontalAzimuths))
            '    TrialDataList.Add(String.Join(";", ActualElevations))
            '    TrialDataList.Add(String.Join(";", ActualBinauralDelay_Left))
            '    TrialDataList.Add(String.Join(";", ActualBinauralDelay_Right))
            'Else
            '    For n = 1 To 8
            '        TrialDataList.Add("")
            '    Next
            'End If

            'TrialDataList.Add(Me.IsBmldTrial)
            'If Me.IsBmldTrial = True Then
            '    TrialDataList.Add(Me.BmldNoiseMode.ToString)
            '    TrialDataList.Add(Me.BmldSignalMode.ToString)
            'Else
            '    TrialDataList.Add("")
            '    TrialDataList.Add("")
            'End If

            'TrialDataList.Add(Me.Response)
            'TrialDataList.Add(Me.Result.ToString)
            'TrialDataList.Add(Me.Score)
            'TrialDataList.Add(Me.ResponseTime.ToString(System.Globalization.CultureInfo.InvariantCulture))
            'If ResponseAlternativeCount IsNot Nothing Then
            '    TrialDataList.Add(Me.ResponseAlternativeCount)
            'Else
            '    TrialDataList.Add(0)
            'End If
            'TrialDataList.Add(Me.IsTestTrial.ToString)
            If DirectCast(Me.ParentTestUnit.ParentMeasurement, SipMeasurement).SelectedAudiogramData IsNot Nothing Then
                TrialDataList.Add(Me.PhonemeDiscriminabilityLevel(False))
            Else
                TrialDataList.Add("No audiogram stored")
            End If

            'TrialDataList.Add(Me.SpeechMaterialComponent.PrimaryStringRepresentation)
            'TrialDataList.Add(Me.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"))
            'TrialDataList.Add(Me.SpeechMaterialComponent.GetCategoricalVariableValue("SpellingAFC"))
            'TrialDataList.Add(Me.SpeechMaterialComponent.GetCategoricalVariableValue("Transcription"))
            'TrialDataList.Add(Me.SpeechMaterialComponent.GetCategoricalVariableValue("TranscriptionAFC"))

            Dim PseudoTrialIds As New List(Of String)
            Dim PseudoTrialSpellings As New List(Of String)
            For Each PseudoTrial In PseudoTrials
                PseudoTrialIds.Add(PseudoTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"))
                PseudoTrialSpellings.Add(PseudoTrial.SpeechMaterialComponent.Id)
            Next
            TrialDataList.Add(String.Join("; ", PseudoTrialIds))
            TrialDataList.Add(String.Join("; ", PseudoTrialSpellings))

            'Adding export of sound files,
            Dim ExportedSoundFilesList As New List(Of String)
            If SkipExportOfSoundFiles = False Then
                For i = 0 To Me.TrialSoundsToExport.Count - 1
                    Dim ExportSound = Me.TrialSoundsToExport(i).Item2
                    Dim FileName = IO.Path.Combine(Me.ParentTestUnit.ParentMeasurement.TrialResultsExportFolder, "TrialSoundFiles", "Trial_" & Me.PresentationOrder & "_" & Me.TrialSoundsToExport(i).Item1 & "_" & Me.SpeechMaterialComponent.Id & ".wav")
                    ExportSound.WriteWaveFile(FileName)
                    ExportedSoundFilesList.Add(FileName)
                Next
            End If
            TrialDataList.Add(String.Join(";", ExportedSoundFilesList))

            Dim ExportedPseudoTrialSoundFilesList As New List(Of String)
            If SkipExportOfSoundFiles = False Then
                If PseudoTrials IsNot Nothing Then
                    For Each PseudoTrial In PseudoTrials
                        For i = 0 To PseudoTrial.TrialSoundsToExport.Count - 1
                            Dim ExportSound = PseudoTrial.TrialSoundsToExport(i).Item2
                            Dim FileName = IO.Path.Combine(Me.ParentTestUnit.ParentMeasurement.TrialResultsExportFolder, "TrialSoundFiles", "Trial_" & Me.PresentationOrder & "_Pseudo_" & PseudoTrial.TrialSoundsToExport(i).Item1 & "_" & PseudoTrial.SpeechMaterialComponent.Id & ".wav")
                            ExportSound.WriteWaveFile(FileName)
                            ExportedPseudoTrialSoundFilesList.Add(FileName)
                        Next
                    Next
                End If
            End If
            TrialDataList.Add(String.Join(";", ExportedPseudoTrialSoundFilesList))

            'TrialDataList.Add(SelectedTargetIndexString)
            'TrialDataList.Add(SelectedMaskerIndicesString)
            'TrialDataList.Add(BackgroundStartSamplesString)
            'TrialDataList.Add(BackgroundSpeechStartSamplesString)

            Dim PseudoTrial_SelectedTargetIndexStringList As New List(Of String)
            Dim PseudoTrial_SelectedMaskerIndicesStringList As New List(Of String)
            Dim PseudoTrial_BackgroundStartSamplesStringList As New List(Of String)
            Dim PseudoTrial_BackgroundSpeechStartSamplesStringList As New List(Of String)
            For Each PseduTrial In PseudoTrials
                PseudoTrial_SelectedTargetIndexStringList.Add(PseduTrial.SelectedTargetIndexString)
                PseudoTrial_SelectedMaskerIndicesStringList.Add(PseduTrial.SelectedMaskerIndicesString)
                PseudoTrial_BackgroundStartSamplesStringList.Add(PseduTrial.BackgroundStartSamplesString)
                PseudoTrial_BackgroundSpeechStartSamplesStringList.Add(PseduTrial.BackgroundSpeechStartSamplesString)
            Next
            TrialDataList.Add(String.Join(";", PseudoTrial_SelectedTargetIndexStringList))
            TrialDataList.Add(String.Join(";", PseudoTrial_SelectedMaskerIndicesStringList))
            TrialDataList.Add(String.Join(";", PseudoTrial_BackgroundStartSamplesStringList))
            TrialDataList.Add(String.Join(";", PseudoTrial_BackgroundSpeechStartSamplesStringList))

            'TrialDataList.Add(BackgroundNonSpeechDucking)
            Dim PseudoTrial_BackgroundNonSpeechDuckingList As New List(Of String)
            For Each PseduTrial In PseudoTrials
                PseudoTrial_BackgroundNonSpeechDuckingList.Add(PseduTrial.BackgroundNonSpeechDucking)
            Next
            TrialDataList.Add(String.Join(";", PseudoTrial_BackgroundNonSpeechDuckingList))

            'TrialDataList.Add(ContextRegionSpeech_SPL)
            'If TestWordLevel.HasValue = True Then
            '    TrialDataList.Add(TestWordLevel)
            'Else
            '    TrialDataList.Add("NA")
            'End If
            'TrialDataList.Add(ReferenceTestWordLevel_SPL)

            Dim ContextRegionSpeech_SPL_List As New List(Of String)
            Dim TestWordLevel_List As New List(Of String)
            Dim ReferenceTestWordLevel_SPL_List As New List(Of String)
            For Each PseduTrial In PseudoTrials
                ContextRegionSpeech_SPL_List.Add(PseduTrial.ContextRegionSpeech_SPL)
                If PseduTrial.TestWordLevel.HasValue = True Then
                    TestWordLevel_List.Add(PseduTrial.TestWordLevel)
                Else
                    TestWordLevel_List.Add("NA")
                End If
                ReferenceTestWordLevel_SPL_List.Add(PseduTrial.ReferenceTestWordLevel_SPL)
            Next
            TrialDataList.Add(String.Join(";", ContextRegionSpeech_SPL_List))
            TrialDataList.Add(String.Join(";", TestWordLevel_List))
            TrialDataList.Add(String.Join(";", ReferenceTestWordLevel_SPL_List))

            'Target Startsamples
            'TrialDataList.Add(TargetStartSample)
            Dim PseudoTrials_TargetStartSample As New List(Of String)
            For Each PseduTrial In PseudoTrials
                PseudoTrials_TargetStartSample.Add(PseduTrial.TargetStartSample)
            Next
            TrialDataList.Add(String.Join(";", PseudoTrials_TargetStartSample))

            'Test phoneme start sample and length
            'If TargetInitialMargins.Count = 0 Then TargetInitialMargins.Add(0) ' Adding an initial margin of zero if for some reason empty
            'Dim TP_SaL = GetTestPhonemeStartAndLength(TargetInitialMargins(0)) ' N.B. / TODO: Here initial margins are assumed only for one target. Need to be changed if several targets with different initial marginsa are to be used.
            'Dim TestPhonemeStartSample As Integer = TP_SaL.Item1
            'Dim TestPhonemelength As Integer = TP_SaL.Item2
            'TrialDataList.Add(TestPhonemeStartSample)
            'TrialDataList.Add(TestPhonemelength)

            Dim PseudoTrials_TP_StartSamples As New List(Of String)
            Dim PseudoTrials_TP_Length As New List(Of String)
            For pseudoTrialIndex = 0 To PseudoTrials.Count - 1
                If PseudoTrials(pseudoTrialIndex).TargetInitialMargins.Count = 0 Then PseudoTrials(pseudoTrialIndex).TargetInitialMargins.Add(0) ' Adding an initial margin of zero if for some reason empty
                Dim PS_TP_SaL = PseudoTrials(pseudoTrialIndex).GetTestPhonemeStartAndLength(PseudoTrials(pseudoTrialIndex).TargetInitialMargins(0)) ' N.B. / TODO: Here initial margins are assumed only for one target. Need to be changed if several targets with different initial marginsa are to be used.
                PseudoTrials_TP_StartSamples.Add(PS_TP_SaL.Item1)
                PseudoTrials_TP_Length.Add(PS_TP_SaL.Item2)
            Next
            TrialDataList.Add(String.Join(";", PseudoTrials_TP_StartSamples))
            TrialDataList.Add(String.Join(";", PseudoTrials_TP_Length))

            'Gains
            'Dim TargetTrialGains As New List(Of String)
            'For Each Item In GainList
            '    TargetTrialGains.Add(Item.Key.ToString & ": " & String.Join(";", Item.Value))
            'Next
            'TrialDataList.Add(String.Join(" / ", TargetTrialGains))

            Dim PseudoTrialsGains As New List(Of String)
            For pseudoTrialIndex = 0 To PseudoTrials.Count - 1
                Dim PseudoTrialGains As New List(Of String)
                For Each Item In PseudoTrials(pseudoTrialIndex).GainList
                    PseudoTrialGains.Add(Item.Key.ToString & ": " & String.Join(";", Item.Value))
                Next
                PseudoTrialsGains.Add(String.Join(" / ", PseudoTrialGains))
            Next
            TrialDataList.Add(String.Join(" | ", PseudoTrialsGains))

            Return String.Join(vbTab, TrialDataList)

        End Function


        'Removes all sounds from the trial free up memory
        Public Overrides Sub RemoveSounds()

            'Removes the sound, since it's not going to be used again (and could take up quite some memory if kept...)
            If Sound IsNot Nothing Then Sound = Nothing

            If TrialSoundsToExport IsNot Nothing Then
                For i = 0 To TrialSoundsToExport.Count - 1
                    TrialSoundsToExport(i) = New Tuple(Of String, STFN.Core.Audio.Sound)(TrialSoundsToExport(i).Item1, Nothing)
                Next
            End If

            'And also the pseudo trial sounds if any
            If PseudoTrials IsNot Nothing Then
                For Each PseudoTrial In PseudoTrials
                    If PseudoTrial.Sound IsNot Nothing Then PseudoTrial.Sound = Nothing

                    If PseudoTrial.TrialSoundsToExport IsNot Nothing Then
                        For i = 0 To PseudoTrial.TrialSoundsToExport.Count - 1
                            PseudoTrial.TrialSoundsToExport(i) = New Tuple(Of String, STFN.Core.Audio.Sound)(PseudoTrial.TrialSoundsToExport(i).Item1, Nothing)
                        Next
                    End If
                Next
            End If

        End Sub


    End Class



End Namespace
