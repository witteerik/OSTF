Imports STFN.Core.Audio.SoundScene
Imports STFN.Core.Audio
Imports STFN.Core.Audio.Formats
Imports System.Reflection
Imports System.Runtime.Serialization

Namespace SipTest

    Public Class SipTrial
        Inherits TestTrial

        ''' <summary>
        ''' Holds a reference to the test unit to which the current test trial belongs
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ParentTestUnit As SiPTestUnit

        Public Property SelectedMediaIndex As Integer

        Public Property SelectedMaskerIndices As New List(Of Integer)

        ''' <summary>
        ''' After sound has been mixed, holds the initial margins of the target sounds (used for export purposes)
        ''' </summary>
        Public TargetInitialMargins As New List(Of Integer)

        ''' <summary>
        ''' Holds a reference to the MediaSet used in the trial.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MediaSet As MediaSet


        'The result of a test trial
        Public Property Result As PossibleResults

        ''' <summary>
        ''' Returns the result converted to a binary score (1=correct, 0=wrong or missing)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Score As Integer
            Get
                If Result = PossibleResults.Correct Then
                    Return 1
                Else
                    Return 0
                End If
            End Get
        End Property

        Public Enum PossibleResults
            Correct
            Incorrect
            Missing
        End Enum

        ''' <summary>
        ''' The response time in milliseconds
        ''' </summary>
        ''' <returns></returns>
        Public Property ResponseTime As Integer

        Public Property ResponseMoment As DateTime


        Public Function DetermineResponseAlternativeCount() As Boolean

            If SpeechMaterialComponent Is Nothing Then Return False

            'Gets the number of response contrasting alternatives
            Dim TempCount As Integer = 0
            SpeechMaterialComponent.IsContrastingComponent(, TempCount)
            _ResponseAlternativeCount = TempCount

            Return True
        End Function

        Protected _ResponseAlternativeCount As Integer? = Nothing
        Public Property ResponseAlternativeCount As Integer?
            Get
                Return _ResponseAlternativeCount
            End Get
            Set(value As Integer?)
                _ResponseAlternativeCount = value
            End Set
        End Property


        ''' <summary>
        ''' Indicates whether the trial is a real test trial or not (e.g. a practise trial)
        ''' </summary>
        ''' <returns></returns>
        Public Property IsTestTrial As Boolean = True

        '''' <summary>
        '''' An object that can hold test trial sounds that can be mixed in advance.
        '''' </summary>
        'Public Sound As Audio.Sound = Nothing

        ''' <summary>
        ''' Holds the start sample of the target signal within the test trial sound (without any delay caused by room simulation)
        ''' </summary>
        Public TargetStartSample As Integer


        ''' <summary>
        ''' A list of gain applied to all different SoundSceneItems in the presented TestTrial
        ''' </summary>
        Public GainList As New SortedList(Of SoundSceneItem.SoundSceneItemRoles, List(Of String))

        'Strings that hold information about the (random) sound, and sound section, selections made in MixSound (for export purposes only)
        Public SelectedTargetIndexString As String = ""
        Public SelectedMaskerIndicesString As String = ""
        Public BackgroundStartSamplesString As String = ""
        Public BackgroundSpeechStartSamplesString As String = ""

        Public TestWordStartTime As Double
        Public TestWordCompletedTime As Double

        Public BackgroundNonSpeechDucking As Double

        Public SoundPropagationType As SoundPropagationTypes

        ''' <summary>
        ''' Holds the location(s) of the target(s)
        ''' </summary>
        Public TargetStimulusLocations As SoundSourceLocation()

        ''' <summary>
        ''' Holds the location of the maskers
        ''' </summary>
        Public MaskerLocations As SoundSourceLocation()

        ''' <summary>
        ''' Holds the location of the background sounds
        ''' </summary>
        Public BackgroundLocations As SoundSourceLocation()

        ''' <summary>
        ''' Determines whether the trial is a binaural masking level difference (BMLD) trial.
        ''' </summary>
        Public IsBmldTrial As Boolean

        ''' <summary>
        ''' If IsBmldTrial is true, determines the BMLD signal mode
        ''' </summary>
        Public BmldSignalMode As BmldModes

        ''' <summary>
        ''' If IsBmldTrial is true, determines the BMLD noise mode
        ''' </summary>
        Public BmldNoiseMode As BmldModes

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

            Me.ParentTestUnit = ParentTestUnit
            Me.SpeechMaterialComponent = SpeechMaterialComponent
            Me.MediaSet = MediaSet
            Me.SoundPropagationType = SoundPropagationType
            Me.TargetStimulusLocations = TargetStimulusLocations
            Me.MaskerLocations = MaskerLocations
            Me.BackgroundLocations = BackgroundLocations

            'Setting some levels
            Dim Fs2Spl As Double = DSP.Standard_dBFS_dBSPL_Difference

            'Calculating levels
            ReferenceSpeechMaterialLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.ListCollection).GetNumericMediaSetVariableValue(MediaSet, "Lc")

            If SpeechMaterialComponent.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Sentence Then
                'Getting the specific level for the component
                ReferenceTestWordLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetNumericMediaSetVariableValue(MediaSet, "Lc") 'TestStimulus.TestWord_ReferenceSPL
                ReferenceContrastingPhonemesLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).GetNumericMediaSetVariableValue(MediaSet, "RLxs")

            ElseIf SpeechMaterialComponent.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.List Then

                'Getting the average levels instead (this is used in the adaptive SiP-test version
                Dim Members = SpeechMaterialComponent.GetChildren
                Dim LevelList As New List(Of Double)
                For Each Child In Members
                    LevelList.Add(Fs2Spl + Child.GetNumericMediaSetVariableValue(MediaSet, "Lc")) 'TestStimulus.TestWord_ReferenceSPL
                Next
                ReferenceTestWordLevel_SPL = LevelList.Average

                ReferenceContrastingPhonemesLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetNumericMediaSetVariableValue(MediaSet, "RLxs")

            Else
                Throw New Exception("The speech-material component linguistic level of " & SpeechMaterialComponent.LinguisticLevel.ToString & " is not supported for the SiPTrial class.")
            End If


            SelectedMediaIndex = SipMeasurementRandomizer.Next(0, Me.MediaSet.MediaAudioItems)

        End Sub



        Public Property Reference_SPL As Double
        Public Property PNR As Double

        Protected _TargetMasking_SPL As Double? = Nothing

        ''' <summary>
        ''' Returns the intended masker sound level at the position of the listeners head, or Nothing if not calculated
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TargetMasking_SPL As Double?
            Get
                Return _TargetMasking_SPL
            End Get
        End Property

        Public Property ContextRegionSpeech_SPL As Double

        Protected Property _TestWordLevel As Double? = Nothing
        Public ReadOnly Property TestWordLevel As Double?
            Get
                Return _TestWordLevel
            End Get
        End Property

        Public ReadOnly Property ReferenceSpeechMaterialLevel_SPL As Double
        Public ReadOnly Property ReferenceTestWordLevel_SPL As Double
        Public ReadOnly Property ReferenceContrastingPhonemesLevel_SPL As Double

        Public Property TestWordLevelLimit As Double = 82.3 '82.3 Shouted Speech level i SII standard
        Public Property ContextSpeechLimit As Double = 74.85 '74.85 Loud Speech Level From SII standard

        ''' <summary>
        ''' Sets all levels in the current test trial. Levels should be set prior to mixing the sound.
        ''' </summary>
        ''' <param name="ReferenceLevel"></param>
        ''' <param name="PNR"></param>
        Public Overridable Sub SetLevels(ByVal ReferenceLevel As Double, ByVal PNR As Double)

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

        End Sub

        Public Overridable Sub MixSound(ByRef SelectedTransducer As AudioSystemSpecification,
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

                Dim SoundWaveFormat As Audio.Formats.WaveFormat = Nothing

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

                Dim Maskers As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation))
                For MaskerIndex = 0 To NumberOfMaskers - 1
                    Dim Masker As Audio.Sound = (Me.SpeechMaterialComponent.GetMaskerSound(Me.MediaSet, SelectedMaskerIndices(MaskerIndex)))

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

                    Maskers.Add(New Tuple(Of Audio.Sound, SoundSourceLocation)(Masker, MaskerLocations(MaskerIndex)))
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
                Dim Targets As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation))
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

                    Targets.Add(New Tuple(Of Audio.Sound, SoundSourceLocation)(TestWordSound, Me.TargetStimulusLocations(TargetIndex)))
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
                Dim BackgroundNonSpeech_Sound As Audio.Sound = Me.SpeechMaterialComponent.GetBackgroundNonspeechSound(Me.MediaSet, 0)
                Dim Backgrounds As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation, Integer)) ' N.B. The Item3 holds the start sample of the section taken out of the source sound
                Dim NumberOfBackgrounds As Integer = BackgroundLocations.Length
                For BackgroundIndex = 0 To NumberOfBackgrounds - 1
                    'Getting a background sound and location
                    'TODO: we should make sure the start time for copying the sounds here differ by several seconds (and can be kept exactly the same for BMLD testing) 
                    Dim BackgroundStartSample As Integer = SipMeasurementRandomizer.Next(0, BackgroundNonSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2)
                    Backgrounds.Add(New Tuple(Of Audio.Sound, SoundSourceLocation, Integer)(
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
                Dim BackgroundSpeechSelections As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation, Integer))
                'For now skipping with BMLD
                If UseBackgroundSpeech = True Then
                    Dim BackgroundSpeech_Sound As Audio.Sound = Me.SpeechMaterialComponent.GetBackgroundSpeechSound(Me.MediaSet, 0)
                    For TargetIndex = 0 To NumberOfTargets - 1
                        'TODO: we should make sure the start time for copying the sounds here differ by several seconds (and can be kept exactly the same for BMLD testing) 
                        Dim StartSample As Integer = SipMeasurementRandomizer.Next(0, BackgroundSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2)
                        Dim CurrentBackgroundSpeechSelection = BackgroundSpeech_Sound.CopySection(1, StartSample, TrialSoundLength)
                        BackgroundSpeechSelections.Add(New Tuple(Of Audio.Sound, SoundSourceLocation, Integer)(CurrentBackgroundSpeechSelection, Me.TargetStimulusLocations(TargetIndex), StartSample))
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
                If LogToConsole = True Then Console.WriteLine("Prepared sounds in " & MixStopWatch.ElapsedMilliseconds & " ms.")
                MixStopWatch.Restart()


                'Creating the mix by calling CreateSoundScene of the current Mixer
                Dim MixedTestTrialSound As Audio.Sound = SelectedTransducer.Mixer.CreateSoundScene(ItemList, UseNominalLevels, False, Me.SoundPropagationType, SelectedTransducer.LimiterThreshold)


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

                If LogToConsole = True Then Console.WriteLine("Mixed sound in " & MixStopWatch.ElapsedMilliseconds & " ms.")

                'TODO: Here we can simulate and/or compensate for hearing loss:
                'SimulateHearingLoss,
                'CompensateHearingLoss

                Sound = MixedTestTrialSound

                'Stores the testword start sample
                Me.TargetStartSample = TestWordStartSample

            Catch ex As Exception
                Logging.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
            End Try

        End Sub



        Public Sub PreMixTestTrialSoundOnNewTread(ByRef SelectedTransducer As AudioSystemSpecification,
                                         ByVal MinimumStimulusOnsetTime As Double, ByVal MaximumStimulusOnsetTime As Double,
                                         ByRef SipMeasurementRandomizer As Random, ByVal TrialSoundMaxDuration As Double, ByVal UseBackgroundSpeech As Boolean,
                                         Optional ByVal FixedMaskerIndices As List(Of Integer) = Nothing)

            Dim NewTestTrialSoundMixClass = New TestTrialSoundMixerOnNewThread(Me, SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, FixedMaskerIndices)


        End Sub

        Private Class TestTrialSoundMixerOnNewThread

            Public SipTrial As SipTrial
            Public SelectedTransducer As AudioSystemSpecification
            Public MinimumStimulusOnsetTime As Double
            Public MaximumStimulusOnsetTime As Double
            Public SipMeasurementRandomizer As Random
            Public TrialSoundMaxDuration As Double
            Public UseBackgroundSpeech As Boolean

            Public FixedMaskerIndices As List(Of Integer) = Nothing

            Public Sub New(ByRef SipTrial As SipTrial, ByRef SelectedTransducer As AudioSystemSpecification,
                           ByVal MinimumStimulusOnsetTime As Double, ByVal MaximumStimulusOnsetTime As Double,
                           ByRef SipMeasurementRandomizer As Random, ByVal TrialSoundMaxDuration As Double, ByVal UseBackgroundSpeech As Boolean,
                           Optional ByVal FixedMaskerIndices As List(Of Integer) = Nothing)

                Me.SipTrial = SipTrial
                Me.SelectedTransducer = SelectedTransducer
                Me.MinimumStimulusOnsetTime = MinimumStimulusOnsetTime
                Me.MaximumStimulusOnsetTime = MaximumStimulusOnsetTime
                Me.SipMeasurementRandomizer = SipMeasurementRandomizer
                Me.TrialSoundMaxDuration = TrialSoundMaxDuration
                Me.UseBackgroundSpeech = UseBackgroundSpeech

                Me.FixedMaskerIndices = FixedMaskerIndices

                Dim NewTread As New Threading.Thread(AddressOf DoWork)
                NewTread.IsBackground = True
                NewTread.Start()

            End Sub

            Public Sub DoWork()
                SipTrial.MixSound(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, FixedMaskerIndices)
            End Sub

        End Class



        Public Shared Function CreateExportHeadings() As String

            Dim Headings As New List(Of String)
            Headings.Add("ParticipantID")
            Headings.Add("MeasurementDateTime")
            Headings.Add("Description")
            Headings.Add("TestUnitIndex")
            Headings.Add("TestUnitDescription")
            Headings.Add("SpeechMaterialComponentID")
            Headings.Add("TestWordGroup")
            Headings.Add("MediaSetName")
            Headings.Add("PresentationOrder")
            Headings.Add("ReferenceSpeechMaterialLevel_SPL")
            Headings.Add("ReferenceContrastingPhonemesLevel_SPL")
            Headings.Add("Reference_SPL")
            Headings.Add("PNR")
            Headings.Add("TargetMasking_SPL")
            Headings.Add("TestWordLevelLimit")
            Headings.Add("ContextSpeechLimit")
            Headings.Add("SoundPropagationType")

            Headings.Add("IntendedTargetLocation_Distance")
            Headings.Add("IntendedTargetLocation_HorizontalAzimuth")
            Headings.Add("IntendedTargetLocation_Elevation")
            Headings.Add("PresentedTargetLocation_Distance")
            Headings.Add("PresentedTargetLocation_HorizontalAzimuth")
            Headings.Add("PresentedTargetLocation_Elevation")
            Headings.Add("PresentedTargetLocation_BinauralDelay_Left")
            Headings.Add("PresentedTargetLocation_BinauralDelay_Right")

            Headings.Add("IntendedMaskerLocations_Distance")
            Headings.Add("IntendedMaskerLocations_HorizontalAzimuth")
            Headings.Add("IntendedMaskerLocations_Elevation")
            Headings.Add("PresentedMaskerLocations_Distance")
            Headings.Add("PresentedMaskerLocations_HorizontalAzimuth")
            Headings.Add("PresentedMaskerLocations_Elevation")
            Headings.Add("PresentedMaskerLocation_BinauralDelay_Left")
            Headings.Add("PresentedMaskerLocation_BinauralDelay_Right")

            Headings.Add("IntendedBackgroundLocations_Distance")
            Headings.Add("IntendedBackgroundLocations_HorizontalAzimuth")
            Headings.Add("IntendedBackgroundLocations_Elevation")
            Headings.Add("PresentedBackgroundLocations_Distance")
            Headings.Add("PresentedBackgroundLocations_HorizontalAzimuth")
            Headings.Add("PresentedBackgroundLocations_Elevation")
            Headings.Add("PresentedBackgroundLocation_BinauralDelay_Left")
            Headings.Add("PresentedBackgroundLocation_BinauralDelay_Right")

            Headings.Add("IsBmldTrial")
            Headings.Add("BmldNoiseMode")
            Headings.Add("BmldSignalMode")

            Headings.Add("Response")
            Headings.Add("Result")
            Headings.Add("Score")
            Headings.Add("ResponseTime")
            Headings.Add("ResponseAlternativeCount")
            Headings.Add("IsTestTrial")

            Headings.Add("PrimaryStringRepresentation")

            Headings.Add("Spelling")
            Headings.Add("SpellingAFC")
            Headings.Add("Transcription")
            Headings.Add("TranscriptionAFC")

            Headings.Add("TargetTrial_SelectedTargetSoundIndex")
            Headings.Add("TargetTrial_SelectedMaskerSoundIndices")
            Headings.Add("TargetTrial_BackgroundNonspeechSoundStartSamples")
            Headings.Add("TargetTrial_BackgroundSpeechSoundStartSamples")

            Headings.Add("TargetTrial_BackgroundNonSpeechDucking")

            Headings.Add("TargetTrial_ContextRegionSpeech_SPL")
            Headings.Add("TargetTrial_TestWordLevel")
            Headings.Add("TargetTrial_ReferenceTestWordLevel_SPL")

            Headings.Add("TargetStartSample")
            Headings.Add("TestPhonemeStartSample")
            Headings.Add("TestPhonemelength")

            Headings.Add("TargetTrial_Gains")

            Return String.Join(vbTab, Headings)

        End Function

        Public Overridable Function CreateExportString(Optional ByVal SkipExportOfSoundFiles As Boolean = True) As String

            Dim TrialDataList As New List(Of String)
            TrialDataList.Add(Me.ParentTestUnit.ParentMeasurement.ParticipantID)
            TrialDataList.Add(Me.ParentTestUnit.ParentMeasurement.MeasurementDateTime.ToString(System.Globalization.CultureInfo.InvariantCulture))
            TrialDataList.Add(Me.ParentTestUnit.ParentMeasurement.Description)
            TrialDataList.Add(Me.ParentTestUnit.ParentMeasurement.GetParentTestUnitIndex(Me))
            TrialDataList.Add(Me.ParentTestUnit.Description)
            TrialDataList.Add(Me.SpeechMaterialComponent.Id)
            TrialDataList.Add(Me.SpeechMaterialComponent.ParentComponent.PrimaryStringRepresentation)
            TrialDataList.Add(Me.MediaSet.MediaSetName)
            TrialDataList.Add(Me.PresentationOrder)
            TrialDataList.Add(Me.ReferenceSpeechMaterialLevel_SPL)
            TrialDataList.Add(Me.ReferenceContrastingPhonemesLevel_SPL)
            TrialDataList.Add(Me.Reference_SPL)
            TrialDataList.Add(Me.PNR)
            If TargetMasking_SPL.HasValue = True Then
                TrialDataList.Add(TargetMasking_SPL)
            Else
                TrialDataList.Add("NA")
            End If
            TrialDataList.Add(TestWordLevelLimit)
            TrialDataList.Add(ContextSpeechLimit)

            TrialDataList.Add(Me.SoundPropagationType.ToString)

            If Me.TargetStimulusLocations.Length > 0 Then
                Dim Distances As New List(Of String)
                Dim HorizontalAzimuths As New List(Of String)
                Dim Elevations As New List(Of String)
                Dim ActualDistances As New List(Of String)
                Dim ActualHorizontalAzimuths As New List(Of String)
                Dim ActualElevations As New List(Of String)
                Dim ActualBinauralDelay_Left As New List(Of String)
                Dim ActualBinauralDelay_Right As New List(Of String)
                For i = 0 To Me.TargetStimulusLocations.Length - 1
                    Distances.Add(Me.TargetStimulusLocations(i).Distance)
                    HorizontalAzimuths.Add(Me.TargetStimulusLocations(i).HorizontalAzimuth)
                    Elevations.Add(Me.TargetStimulusLocations(i).Elevation)
                    If Me.TargetStimulusLocations(i).ActualLocation Is Nothing Then Me.TargetStimulusLocations(i).ActualLocation = New SoundSourceLocation
                    ActualDistances.Add(Me.TargetStimulusLocations(i).ActualLocation.Distance)
                    ActualHorizontalAzimuths.Add(Me.TargetStimulusLocations(i).ActualLocation.HorizontalAzimuth)
                    ActualElevations.Add(Me.TargetStimulusLocations(i).ActualLocation.Elevation)
                    ActualBinauralDelay_Left.Add(Me.TargetStimulusLocations(i).ActualLocation.BinauralDelay.LeftDelay)
                    ActualBinauralDelay_Right.Add(Me.TargetStimulusLocations(i).ActualLocation.BinauralDelay.RightDelay)
                Next
                TrialDataList.Add(String.Join(";", Distances))
                TrialDataList.Add(String.Join(";", HorizontalAzimuths))
                TrialDataList.Add(String.Join(";", Elevations))
                TrialDataList.Add(String.Join(";", ActualDistances))
                TrialDataList.Add(String.Join(";", ActualHorizontalAzimuths))
                TrialDataList.Add(String.Join(";", ActualElevations))
                TrialDataList.Add(String.Join(";", ActualBinauralDelay_Left))
                TrialDataList.Add(String.Join(";", ActualBinauralDelay_Right))
            Else
                For n = 1 To 8
                    TrialDataList.Add("")
                Next
            End If

            If Me.MaskerLocations.Length > 0 Then
                Dim Distances As New List(Of String)
                Dim HorizontalAzimuths As New List(Of String)
                Dim Elevations As New List(Of String)
                Dim ActualDistances As New List(Of String)
                Dim ActualHorizontalAzimuths As New List(Of String)
                Dim ActualElevations As New List(Of String)
                Dim ActualBinauralDelay_Left As New List(Of String)
                Dim ActualBinauralDelay_Right As New List(Of String)
                For i = 0 To Me.MaskerLocations.Length - 1
                    Distances.Add(Me.MaskerLocations(i).Distance)
                    HorizontalAzimuths.Add(Me.MaskerLocations(i).HorizontalAzimuth)
                    Elevations.Add(Me.MaskerLocations(i).Elevation)
                    If Me.MaskerLocations(i).ActualLocation Is Nothing Then Me.MaskerLocations(i).ActualLocation = New SoundSourceLocation
                    ActualDistances.Add(Me.MaskerLocations(i).ActualLocation.Distance)
                    ActualHorizontalAzimuths.Add(Me.MaskerLocations(i).ActualLocation.HorizontalAzimuth)
                    ActualElevations.Add(Me.MaskerLocations(i).ActualLocation.Elevation)
                    ActualBinauralDelay_Left.Add(Me.MaskerLocations(i).ActualLocation.BinauralDelay.LeftDelay)
                    ActualBinauralDelay_Right.Add(Me.MaskerLocations(i).ActualLocation.BinauralDelay.RightDelay)
                Next
                TrialDataList.Add(String.Join(";", Distances))
                TrialDataList.Add(String.Join(";", HorizontalAzimuths))
                TrialDataList.Add(String.Join(";", Elevations))
                TrialDataList.Add(String.Join(";", ActualDistances))
                TrialDataList.Add(String.Join(";", ActualHorizontalAzimuths))
                TrialDataList.Add(String.Join(";", ActualElevations))
                TrialDataList.Add(String.Join(";", ActualBinauralDelay_Left))
                TrialDataList.Add(String.Join(";", ActualBinauralDelay_Right))
            Else
                For n = 1 To 8
                    TrialDataList.Add("")
                Next
            End If

            If Me.BackgroundLocations.Length > 0 Then
                Dim Distances As New List(Of String)
                Dim HorizontalAzimuths As New List(Of String)
                Dim Elevations As New List(Of String)
                Dim ActualDistances As New List(Of String)
                Dim ActualHorizontalAzimuths As New List(Of String)
                Dim ActualElevations As New List(Of String)
                Dim ActualBinauralDelay_Left As New List(Of String)
                Dim ActualBinauralDelay_Right As New List(Of String)
                For i = 0 To Me.BackgroundLocations.Length - 1
                    Distances.Add(Me.BackgroundLocations(i).Distance)
                    HorizontalAzimuths.Add(Me.BackgroundLocations(i).HorizontalAzimuth)
                    Elevations.Add(Me.BackgroundLocations(i).Elevation)
                    If Me.BackgroundLocations(i).ActualLocation Is Nothing Then Me.BackgroundLocations(i).ActualLocation = New SoundSourceLocation
                    ActualDistances.Add(Me.BackgroundLocations(i).ActualLocation.Distance)
                    ActualHorizontalAzimuths.Add(Me.BackgroundLocations(i).ActualLocation.HorizontalAzimuth)
                    ActualElevations.Add(Me.BackgroundLocations(i).ActualLocation.Elevation)
                    ActualBinauralDelay_Left.Add(Me.BackgroundLocations(i).ActualLocation.BinauralDelay.LeftDelay)
                    ActualBinauralDelay_Right.Add(Me.BackgroundLocations(i).ActualLocation.BinauralDelay.RightDelay)
                Next
                TrialDataList.Add(String.Join(";", Distances))
                TrialDataList.Add(String.Join(";", HorizontalAzimuths))
                TrialDataList.Add(String.Join(";", Elevations))
                TrialDataList.Add(String.Join(";", ActualDistances))
                TrialDataList.Add(String.Join(";", ActualHorizontalAzimuths))
                TrialDataList.Add(String.Join(";", ActualElevations))
                TrialDataList.Add(String.Join(";", ActualBinauralDelay_Left))
                TrialDataList.Add(String.Join(";", ActualBinauralDelay_Right))
            Else
                For n = 1 To 8
                    TrialDataList.Add("")
                Next
            End If

            TrialDataList.Add(Me.IsBmldTrial)
            If Me.IsBmldTrial = True Then
                TrialDataList.Add(Me.BmldNoiseMode.ToString)
                TrialDataList.Add(Me.BmldSignalMode.ToString)
            Else
                TrialDataList.Add("")
                TrialDataList.Add("")
            End If

            TrialDataList.Add(Me.Response)
            TrialDataList.Add(Me.Result.ToString)
            TrialDataList.Add(Me.Score)
            TrialDataList.Add(Me.ResponseTime.ToString(System.Globalization.CultureInfo.InvariantCulture))
            If ResponseAlternativeCount IsNot Nothing Then
                TrialDataList.Add(Me.ResponseAlternativeCount)
            Else
                TrialDataList.Add(0)
            End If
            TrialDataList.Add(Me.IsTestTrial.ToString)

            TrialDataList.Add(Me.SpeechMaterialComponent.PrimaryStringRepresentation)
            TrialDataList.Add(Me.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling"))
            TrialDataList.Add(Me.SpeechMaterialComponent.GetCategoricalVariableValue("SpellingAFC"))
            TrialDataList.Add(Me.SpeechMaterialComponent.GetCategoricalVariableValue("Transcription"))
            TrialDataList.Add(Me.SpeechMaterialComponent.GetCategoricalVariableValue("TranscriptionAFC"))

            TrialDataList.Add(SelectedTargetIndexString)
            TrialDataList.Add(SelectedMaskerIndicesString)
            TrialDataList.Add(BackgroundStartSamplesString)
            TrialDataList.Add(BackgroundSpeechStartSamplesString)

            TrialDataList.Add(BackgroundNonSpeechDucking)

            TrialDataList.Add(ContextRegionSpeech_SPL)
            If TestWordLevel.HasValue = True Then
                TrialDataList.Add(TestWordLevel)
            Else
                TrialDataList.Add("NA")
            End If
            TrialDataList.Add(ReferenceTestWordLevel_SPL)

            'Target Startsamples
            TrialDataList.Add(TargetStartSample)

            'Test phoneme start sample and length
            If TargetInitialMargins.Count = 0 Then TargetInitialMargins.Add(0) ' Adding an initial margin of zero if for some reason empty
            Dim TP_SaL = GetTestPhonemeStartAndLength(TargetInitialMargins(0)) ' N.B. / TODO: Here initial margins are assumed only for one target. Need to be changed if several targets with different initial marginsa are to be used.
            Dim TestPhonemeStartSample As Integer = TP_SaL.Item1
            Dim TestPhonemelength As Integer = TP_SaL.Item2
            TrialDataList.Add(TestPhonemeStartSample)
            TrialDataList.Add(TestPhonemelength)

            'Gains
            Dim TargetTrialGains As New List(Of String)
            For Each Item In GainList
                TargetTrialGains.Add(Item.Key.ToString & ": " & String.Join(";", Item.Value))
            Next
            TrialDataList.Add(String.Join(" / ", TargetTrialGains))

            Return String.Join(vbTab, TrialDataList)

        End Function

        Public Function GetTestPhonemeStartAndLength(Optional ByVal InitialMargin As Integer = 0) As Tuple(Of Integer, Integer)

            Dim TestPhonemeStartSample As Integer
            Dim TestPhonemeLength As Integer
            Dim ChildPhonemes = Me.SpeechMaterialComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Phoneme)
            For Each ChildPhoneme In ChildPhonemes
                If ChildPhoneme.IsContrastingComponent = True Then
                    Dim TestPhonemeSma = ChildPhoneme.GetCorrespondingSmaComponent(Me.MediaSet, Me.SelectedMediaIndex, 1, True)
                    If TestPhonemeSma.Count > 0 Then
                        TestPhonemeStartSample = TestPhonemeSma(0).StartSample - InitialMargin
                        TestPhonemeLength = TestPhonemeSma(0).Length
                    End If
                    Exit For
                End If
            Next

            Return New Tuple(Of Integer, Integer)(TestPhonemeStartSample, TestPhonemeLength)

        End Function



        Public Overrides Function TestResultColumnHeadings() As String

            Dim OutputList As New List(Of String)
            'OutputList.AddRange(BaseClassTestResultColumnHeadings())

            OutputList.Add(CreateExportHeadings())

            ''Adding property names
            'Dim properties As PropertyInfo() = GetType(SiPTrial).GetProperties()

            '' Iterating through each property
            'For Each [property] As PropertyInfo In properties

            '    ' Getting the name of the property
            '    Dim propertyName As String = [property].Name
            '    OutputList.Add(propertyName)

            'Next

            Return String.Join(vbTab, OutputList)

        End Function

        Public Overrides Function TestResultAsTextRow() As String

            Dim OutputList As New List(Of String)

            'Copies the MediaSetName, as this has not been stored in SiP trials
            If MediaSet IsNot Nothing Then
                MediaSetName = MediaSet.MediaSetName
            Else
                MediaSetName = ""
            End If

            'And the EM term (which is not used int the SiP-test)
            EfficientContralateralMaskingTerm = 0

            'OutputList.AddRange(BaseClassTestResultAsTextRow())

            OutputList.Add(CreateExportString(True))

            'Dim properties As PropertyInfo() = GetType(SrtTrial).GetProperties()

            '' Iterating through each property
            'For Each [property] As PropertyInfo In properties

            '    ' Getting the name of the property
            '    Dim propertyName As String = [property].Name

            '    ' Getting the value of the property for the current instance 
            '    Dim propertyValue As Object = [property].GetValue(Me)

            '    'If TypeOf propertyValue Is String Then
            '    '    Dim stringValue As String = DirectCast(propertyValue, String)
            '    'ElseIf TypeOf propertyValue Is Integer Then
            '    '    Dim intValue As Integer = DirectCast(propertyValue, Integer)
            '    'Else
            '    'End If

            '   If propertyValue IsNot Nothing Then
            '        OutputList.Add(propertyValue.ToString)
            '   Else
            '        OutputList.Add("NotSet")
            '   End If

            'Next

            Return String.Join(vbTab, OutputList)

        End Function

        'Removes all sounds from the trial free up memory
        Public Overridable Sub RemoveSounds()

            'Removes the sound, since it's not going to be used again (and could take up quite some memory if kept...)
            If Sound IsNot Nothing Then Sound = Nothing

        End Sub

    End Class


End Namespace

