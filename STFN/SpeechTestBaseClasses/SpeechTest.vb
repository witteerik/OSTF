Imports STFN.Audio.SoundScene

Public MustInherit Class SpeechTest


#Region "Instructions"

    Public MustOverride ReadOnly Property TesterInstructions As String
    Public MustOverride ReadOnly Property ParticipantInstructions As String

#End Region

#Region "Initialization"

    Public Sub New(ByVal SpeechMaterialName As String)
        Me.SpeechMaterialName = SpeechMaterialName
        LoadSpeechMaterialSpecification(SpeechMaterialName)
    End Sub

#End Region


#Region "SpeechMaterial"

    Public Function ChangeSpeechMaterial(ByVal NewSpeechMaterialName As String) As Boolean

        If LoadSpeechMaterialSpecification(NewSpeechMaterialName) = True Then
            Me.SpeechMaterialName = NewSpeechMaterialName
            Return True
        Else
            Return False
        End If

    End Function

    Private Function LoadSpeechMaterialSpecification(ByVal SpeechMaterialName As String, Optional ByVal EnforceReloading As Boolean = False) As Boolean

        'Selecting the first available speech material if not specified in the calling code.
        If SpeechMaterialName = "" Then

            Messager.MsgBox("No speech material is selected!" & vbCrLf & "Attempting to select the first speech material available.", Messager.MsgBoxStyle.Information, "Speech material not selected!")

            If AvailableSpeechMaterialSpecifications.Count = 0 Then
                Messager.MsgBox("No speech material is available!" & vbCrLf & "Cannot continue.", Messager.MsgBoxStyle.Information, "Missing speech material!")
                Return False
            Else
                SpeechMaterialName = AvailableSpeechMaterialSpecifications(0)
            End If
        End If

        If LoadedSpeechMaterialSpecifications.ContainsKey(SpeechMaterialName) = False Or EnforceReloading = True Then

            'Removes the SpeechMaterial with SpeechMaterialName if already present
            LoadedSpeechMaterialSpecifications.Remove(SpeechMaterialName)

            'Looking for the speech material
            OstfBase.LoadAvailableSpeechMaterialSpecifications()
            For Each Test In OstfBase.AvailableSpeechMaterials
                If Test.Name = SpeechMaterialName Then
                    'Adding it if found
                    LoadedSpeechMaterialSpecifications.Add(SpeechMaterialName, Test)
                    Exit For
                End If
            Next
        End If

        'Returns true if added (or already present) or false if not found
        Return LoadedSpeechMaterialSpecifications.ContainsKey(SpeechMaterialName)

    End Function


    ''' <summary>
    ''' An object shared between all instances of Speechtest that hold every loaded SpeechtestSpecification and 
    ''' Speech Material component to prevent the need for re-loading between tests. 
    ''' (Note that this also means that test specifications and speech material components should not be altered once loaded.
    ''' </summary>
    ''' <returns></returns>
    Private Shared Property LoadedSpeechMaterialSpecifications As New SortedList(Of String, SpeechMaterialSpecification)

    'A shared function to load tests
    Public ReadOnly Property AvailableSpeechMaterialSpecifications() As List(Of String)
        Get
            Dim OutputList As New List(Of String)
            OstfBase.LoadAvailableSpeechMaterialSpecifications()
            For Each test In OstfBase.AvailableSpeechMaterials
                OutputList.Add(test.Name)
            Next
            Return OutputList
        End Get
    End Property

    ''' <summary>
    ''' The SpeechMaterialName of the currently implemented speech material specification
    ''' </summary>
    ''' <returns></returns>
    Public Property SpeechMaterialName As String


    Public Property SpeechMaterialSpecification As SpeechMaterialSpecification
        Get
            If LoadedSpeechMaterialSpecifications.ContainsKey(SpeechMaterialName) Then
                Return LoadedSpeechMaterialSpecifications(SpeechMaterialName)
            Else
                Return Nothing
            End If
        End Get
        Set(value As SpeechMaterialSpecification)
            LoadedSpeechMaterialSpecifications(SpeechMaterialName) = value
        End Set
    End Property

    Public ReadOnly Property SpeechMaterial As SpeechMaterialComponent
        Get
            If SpeechMaterialSpecification Is Nothing Then
                Return Nothing
            Else
                If SpeechMaterialSpecification.SpeechMaterial Is Nothing Then
                    SpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(SpeechMaterialSpecification.GetSpeechMaterialFilePath(), SpeechMaterialSpecification.GetTestRootPath())
                    SpeechMaterialSpecification.SpeechMaterial = SpeechMaterial
                    SpeechMaterial.ParentTestSpecification = SpeechMaterialSpecification
                End If

                If SpeechMaterialSpecification.SpeechMaterial Is Nothing Then
                    Return Nothing
                Else
                    Return SpeechMaterialSpecification.SpeechMaterial
                End If
            End If
        End Get
    End Property

#End Region

#Region "MediaSets"

    Public ReadOnly Property AvailableMediasets() As List(Of MediaSet)
        Get
            SpeechMaterial.ParentTestSpecification.LoadAvailableMediaSetSpecifications()
            Return SpeechMaterial.ParentTestSpecification.MediaSets
        End Get
    End Property


    Public ReadOnly Property AvailablePresets() As List(Of SmcPresets.Preset)
        Get
            Dim Output = New List(Of SmcPresets.Preset)
            For Each Preset In SpeechMaterial.Presets
                Output.Add(Preset)
            Next
            Return Output
        End Get
    End Property

    Public MustOverride ReadOnly Property AvailableExperimentNumbers As Integer()

    Public ReadOnly Property AvailablePractiseListsNames() As List(Of String)
        Get
            Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)
            Dim Output As New List(Of String)
            For Each List In AllLists
                If List.IsPractiseComponent = True Then
                    Output.Add(List.PrimaryStringRepresentation)
                End If
            Next
            Return Output
        End Get
    End Property

    Public ReadOnly Property AvailableTestListsNames() As List(Of String)
        Get
            Dim AllLists = SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)
            Dim Output As New List(Of String)
            For Each List In AllLists
                If List.IsPractiseComponent = False Then
                    Output.Add(List.PrimaryStringRepresentation)
                End If
            Next
            Return Output
        End Get
    End Property

#End Region


#Region "SoundScene"

    ''' <summary>
    ''' Holds the level step size available in the customizable test options instance used in the test
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride ReadOnly Property LevelStepSize As Double

    Public MustOverride ReadOnly Property MaximumSoundFieldSpeechLocations As Integer
    Public MustOverride ReadOnly Property MaximumSoundFieldMaskerLocations As Integer
    Public MustOverride ReadOnly Property MaximumSoundFieldBackgroundNonSpeechLocations As Integer
    Public MustOverride ReadOnly Property MaximumSoundFieldBackgroundSpeechLocations As Integer

    Public MustOverride ReadOnly Property MinimumSoundFieldSpeechLocations As Integer
    Public MustOverride ReadOnly Property MinimumSoundFieldMaskerLocations As Integer
    Public MustOverride ReadOnly Property MinimumSoundFieldBackgroundNonSpeechLocations As Integer
    Public MustOverride ReadOnly Property MinimumSoundFieldBackgroundSpeechLocations As Integer

    Public MustOverride ReadOnly Property HasOptionalPractiseTest As Boolean
    Public MustOverride ReadOnly Property AllowsUseRetsplChoice As Boolean
    Public MustOverride ReadOnly Property AllowsManualPreSetSelection As Boolean
    Public MustOverride ReadOnly Property AllowsManualStartListSelection As Boolean
    Public MustOverride ReadOnly Property AllowsManualMediaSetSelection As Boolean
    Public MustOverride ReadOnly Property AllowsManualReferenceLevelSelection As Boolean
    Public MustOverride ReadOnly Property AllowsManualSpeechLevelSelection As Boolean
    Public MustOverride ReadOnly Property AllowsManualMaskingLevelSelection As Boolean
    Public MustOverride ReadOnly Property AllowsManualBackgroundLevelSelection As Boolean
    Public MustOverride ReadOnly Property SupportsPrelistening As Boolean
    Public MustOverride ReadOnly Property CanHaveTargets As Boolean
    Public MustOverride ReadOnly Property CanHaveMaskers As Boolean
    Public MustOverride ReadOnly Property CanHaveBackgroundNonSpeech As Boolean
    Public MustOverride ReadOnly Property CanHaveBackgroundSpeech As Boolean
    Public MustOverride ReadOnly Property UseSoundFieldSimulation As Utils.TriState

    Public MustOverride ReadOnly Property SupportsManualPausing As Boolean


    Public ReadOnly Property CurrentlySupportedIrSets As List(Of BinauralImpulseReponseSet)
        Get
            Dim Output As New List(Of BinauralImpulseReponseSet)

            If OstfBase.AllowDirectionalSimulation = True Then
                Dim SupportedIrNames As New List(Of String)
                If TestOptions.SelectedMediaSet IsNot Nothing Then
                    SupportedIrNames = OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(TestOptions.SelectedMediaSet.WaveFileSampleRate)
                ElseIf TestOptions.SelectedMediaSets.Count > 0 Then
                    SupportedIrNames = OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(TestOptions.SelectedMediaSets(0).WaveFileSampleRate)
                Else
                    SupportedIrNames = OstfBase.DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(AvailableMediasets(0).WaveFileSampleRate)
                End If

                Dim AvaliableSets = DirectionalSimulator.GetAllDirectionalSimulationSets()
                For Each AvaliableSet In AvaliableSets
                    If SupportedIrNames.Contains(AvaliableSet.Key) Then
                        Output.Add(AvaliableSet.Value)
                    End If
                Next
            End If

            Return Output
        End Get
    End Property

    ''' <summary>
    ''' Returns the set of transducers from OstfBase.AvaliableTransducers expected to work with the currently connected hardware.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property CurrentlySupportedTransducers As List(Of OstfBase.AudioSystemSpecification)
        Get
            Dim Output = New List(Of OstfBase.AudioSystemSpecification)
            Dim AllTransducers = OstfBase.AvaliableTransducers

            'Adding only transducers that can be used with the current sound system.
            For Each Transducer In AllTransducers
                If Transducer.CanPlay() = True Then Output.Add(Transducer)
            Next

            Return Output
        End Get
    End Property

    ''' <summary>
    ''' This sub mixes targets, maskers, background-non-speech, background-speech and contralateral maskers as assigned in TestOptions sound sources. The mixed sound is stored in the CurrentTestTrial.Sound.
    ''' </summary>
    ''' <param name="UseNominalLevels">If True, applied gains are based on the nominal levels stored in the SMA object of each sound. If False, sound levels are re-calculated.</param>
    ''' <param name="MaximumSoundDuration">The intended duration (ins seconds) of the mixed sound.</param>
    ''' <param name="TargetLevel">The combined level of all targets.</param>
    ''' <param name="TargetPresentationTime">The insertion time of the targets</param>
    ''' <param name="MaskerLevel">The combined level of all maskers.</param>
    ''' <param name="MaskerPresentationTime">The insertion time of the maskers</param>
    ''' <param name="BackgroundNonSpeechLevel">The combined level of all background non-speech sources.</param>
    ''' <param name="BackgroundNonSpeechPresentationTime">The insertion time of the background non-speech sounds</param>
    ''' <param name="BackgroundSpeechLevel">The combined level of all background speech sources</param>
    ''' <param name="BackgroundSpeechPresentationTime">The insertion time of the background-speech sounds</param>
    ''' <param name="ContralateralMaskerLevel">The level of the contralateral masker. Should be specified without correction for efficient masking (EM). (EM is added internally in the function.)</param>
    ''' <param name="ContralateralMaskerPresentationTime">The insertion time of the contralateral maskers</param>
    ''' <param name="FadeSpecs_Target">Optional fading specifications for the targets. If not specified, the function supplies default values that will be used.</param>
    ''' <param name="FadeSpecs_Masker">Optional fading specifications for the maskers. If not specified, the function supplies default values that will be used.</param>
    ''' <param name="FadeSpecs_BackgroundNonSpeech">Optional fading specifications for the background non-speech sounds. If not specified, the function supplies default values that will be used.</param>
    ''' <param name="FadeSpecs_BackgroundSpeech">Optional fading specifications for the background-speech sounds. If not specified, the function supplies default values that will be used.</param>
    ''' <param name="FadeSpecs_ContralateralMasker">Optional fading specifications for the contralateral masker. If not specified, the function supplies default values that will be used.</param>
    ''' <param name="ExportSounds">Can be used to debug or analyse presented sounds. Default value is False. Sounds are stored into the current log folder.</param>
    Protected Sub MixStandardTestTrialSound(ByVal UseNominalLevels As Boolean, ByVal MaximumSoundDuration As Double,
                                  ByVal TargetLevel As Double, ByVal TargetPresentationTime As Double,
                                  Optional ByVal MaskerLevel As Nullable(Of Double) = Nothing, Optional ByVal MaskerPresentationTime As Double = 0,
                                  Optional ByVal BackgroundNonSpeechLevel As Nullable(Of Double) = Nothing, Optional ByVal BackgroundNonSpeechPresentationTime As Double = 0,
                                  Optional ByVal BackgroundSpeechLevel As Nullable(Of Double) = Nothing, Optional ByVal BackgroundSpeechPresentationTime As Double = 0,
                                  Optional ByVal ContralateralMaskerLevel As Nullable(Of Double) = Nothing, Optional ByVal ContralateralMaskerPresentationTime As Double = 0,
                                  Optional ByRef FadeSpecs_Target As List(Of STFN.Audio.DSP.Transformations.FadeSpecifications) = Nothing,
                                  Optional ByRef FadeSpecs_Masker As List(Of STFN.Audio.DSP.Transformations.FadeSpecifications) = Nothing,
                                  Optional ByRef FadeSpecs_BackgroundNonSpeech As List(Of STFN.Audio.DSP.Transformations.FadeSpecifications) = Nothing,
                                  Optional ByRef FadeSpecs_BackgroundSpeech As List(Of STFN.Audio.DSP.Transformations.FadeSpecifications) = Nothing,
                                  Optional ByRef FadeSpecs_ContralateralMasker As List(Of STFN.Audio.DSP.Transformations.FadeSpecifications) = Nothing,
                                            Optional ExportSounds As Boolean = False)

        'TODO: This function is not finished, it still need implementation of BackgroundNonSpeech and BackgroundSpeech

        'Calculates the EM corrected Contralateral masker level (the level supplied should not be EM corrected but be as it would appear on an audiometer attenuator
        Dim ContralateralMaskerLevel_EmCorrected As Double = ContralateralMaskerLevel + TestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain

        'Mix the signal using DuxplexMixer CreateSoundScene
        'Sets a List of SoundSceneItem in which to put the sounds to mix
        Dim ItemList = New List(Of SoundSceneItem)
        Dim LevelGroup As Integer = 1 ' The level group value is used to set the added sound level of items sharing the same (arbitrary) LevelGroup value to the indicated sound level. (Thus, the sounds with the same LevelGroup value are measured together.)
        Dim CurrentSampleRate As Integer = -1

        'Determining test ear and stores in the current test trial (This should perhaps be moved outside this function. On the other hand it's good that it's always detemined when sounds are mixed, though all tests need to implement this or call this code)
        Dim CurrentTestEar As Utils.SidesWithBoth = Utils.SidesWithBoth.Both ' Assuming both, and overriding if needed
        If TestOptions.SelectedTransducer.IsHeadphones = True Then
            If TestOptions.UseSimulatedSoundField = False Then
                Dim HasLeftSideTarget As Boolean = False
                Dim HasRightSideTarget As Boolean = False

                For Each SignalLocation In TestOptions.SignalLocations
                    If SignalLocation.HorizontalAzimuth > 0 Then
                        'At least one signal location is to the right
                        HasRightSideTarget = True
                    End If
                    If SignalLocation.HorizontalAzimuth < 0 Then
                        'At least one signal location is to the left
                        HasLeftSideTarget = True
                    End If
                Next

                'Overriding the value Both if signal is only the left or only the right side
                If HasLeftSideTarget = True And HasRightSideTarget = False Then
                    CurrentTestEar = Utils.Constants.SidesWithBoth.Left
                ElseIf HasLeftSideTarget = False And HasRightSideTarget = True Then
                    CurrentTestEar = Utils.Constants.SidesWithBoth.Right
                End If
            End If
        End If
        CurrentTestTrial.TestEar = CurrentTestEar


        ' **TARGET SOUNDS**
        If TestOptions.SignalLocations.Count > 0 Then

            'Getting the target sound (i.e. test words)
            Dim TargetSound = CurrentTestTrial.SpeechMaterialComponent.GetSound(TestOptions.SelectedMediaSet, 0, 1, , , , , False, False, False, , , False)

            'Storing the samplerate
            CurrentSampleRate = TargetSound.WaveFormat.SampleRate

            'Setting the insertion sample of the target
            Dim TargetStartSample As Integer = Math.Floor(TargetPresentationTime * CurrentSampleRate)

            'Setting the TargetStartMeasureSample (i.e. the sample index in the TargetSound, not in the final mix)
            Dim TargetStartMeasureSample As Integer = 0

            'Getting the TargetMeasureLength from the length of the sound files (i.e. everything is measured)
            Dim TargetMeasureLength As Integer = TargetSound.WaveData.SampleData(1).Length

            'Sets up default fading specifications for the target
            If FadeSpecs_Target Is Nothing Then
                FadeSpecs_Target = New List(Of STFN.Audio.DSP.Transformations.FadeSpecifications)
                FadeSpecs_Target.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.002))
                FadeSpecs_Target.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.002))
            End If

            'Combining targets with the selected SignalLocations
            Dim Targets As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation))
            For Each SignalLocation In TestOptions.SignalLocations
                'Re-using the same target in all selected locations
                Targets.Add(New Tuple(Of Audio.Sound, SoundSourceLocation)(TargetSound, SignalLocation))
            Next

            'Adding the targets sources to the ItemList
            For Index = 0 To Targets.Count - 1
                ItemList.Add(New SoundSceneItem(Targets(Index).Item1, 1, TargetLevel, LevelGroup, Targets(Index).Item2, SoundSceneItem.SoundSceneItemRoles.Target, TargetStartSample, TargetStartMeasureSample, TargetMeasureLength,, FadeSpecs_Target))
            Next

            'Incrementing LevelGroup 
            LevelGroup += 1

            'Storing data in the CurrentTestTrial
            CurrentTestTrial.LinguisticSoundStimulusStartTime = TargetPresentationTime
            CurrentTestTrial.LinguisticSoundStimulusDuration = TargetSound.WaveData.SampleData(1).Length / TargetSound.WaveFormat.SampleRate

        End If


        ' **MASKER SOUNDS**
        If TestOptions.MaskerLocations.Count > 0 Then

            'Ensures that MaskerLevel has a value
            If MaskerLevel.HasValue = False Then Throw New ArgumentException("MaskerLevel value cannot be Nothing!")

            'Getting the masker sound
            Dim MaskerSound = CurrentTestTrial.SpeechMaterialComponent.GetMaskerSound(TestOptions.SelectedMediaSet, 0)

            'Storing the samplerate
            If CurrentSampleRate = -1 Then CurrentSampleRate = MaskerSound.WaveFormat.SampleRate

            'Setting the insertion sample of the masker
            Dim MaskerStartSample As Integer = Math.Floor(MaskerPresentationTime * CurrentSampleRate)

            'Setting the MaskerStartMeasureSample (i.e. the sample index in the MaskerSound, not in the final mix)
            Dim MaskerStartMeasureSample As Integer = 0

            'Getting the MaskerMeasureLength from the length of the sound files (i.e. everything is measured)
            Dim MaskerMeasureLength As Integer = MaskerSound.WaveData.SampleData(1).Length

            'Sets up fading specifications for the masker
            If FadeSpecs_Masker Is Nothing Then
                FadeSpecs_Masker = New List(Of STFN.Audio.DSP.Transformations.FadeSpecifications)
                FadeSpecs_Masker.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.002))
                FadeSpecs_Masker.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.002))
            End If

            'Combining maskers with the selected SignalLocations
            Dim Maskers As New List(Of Tuple(Of Audio.Sound, SoundSourceLocation))

            'Randomizing a start sample in the first half of the masker signal, and then picking different sections of the masker sound, two seconds apart for the different locations.
            Dim RandomStartReadIndex As Integer = Randomizer.Next(0, (MaskerSound.WaveData.SampleData(1).Length / 2) - 1)
            Dim InterMaskerStepLength As Integer = CurrentSampleRate * 2
            Dim IndendedMaskerLength As Integer = MaximumSoundDuration * CurrentSampleRate - MaskerStartSample

            'Calculating the needed sound length and checks that the masker sound is long enough
            Dim NeededSoundLength As Integer = RandomStartReadIndex + (TestOptions.MaskerLocations.Count + 1) * InterMaskerStepLength + IndendedMaskerLength + 10
            If MaskerSound.WaveData.SampleData(1).Length < NeededSoundLength Then
                Throw New Exception("The masker sound specified is too short for the intended maximum sound duration and " & TestOptions.MaskerLocations.Count & " sound sources!")
            End If

            'Picking the masker sounds and combining them with their selected locations
            For Index = 0 To TestOptions.MaskerLocations.Count - 1

                Dim StartCopySample As Integer = RandomStartReadIndex + Index * InterMaskerStepLength
                Dim CurrentSourceMaskerSound = Audio.DSP.CopySection(MaskerSound, StartCopySample, IndendedMaskerLength, 1)

                'Copying the SMA object to retain the nominal level (although other time data and other related stuff will be incorrect, if not adjusted for)
                CurrentSourceMaskerSound.SMA = MaskerSound.SMA.CreateCopy(CurrentSourceMaskerSound)

                'Picking the masker sound
                Maskers.Add(New Tuple(Of Audio.Sound, SoundSourceLocation)(CurrentSourceMaskerSound, TestOptions.MaskerLocations(Index)))

            Next

            'Adding the maskers sources to the ItemList
            For Index = 0 To Maskers.Count - 1
                ItemList.Add(New SoundSceneItem(Maskers(Index).Item1, 1, MaskerLevel, LevelGroup, Maskers(Index).Item2, SoundSceneItem.SoundSceneItemRoles.Masker, MaskerStartSample, MaskerStartMeasureSample, MaskerMeasureLength,, FadeSpecs_Masker))
            Next

            'Incrementing LevelGroup 
            LevelGroup += 1

        End If

        'TODO: implement BackgroundNonSpeech and BackgroundSpeech here

        ' **CONTRALATERAL MASKER**
        If TestOptions.UseContralateralMasking = True Then

            'Ensures that ContralateralMaskerLevel has a value
            If ContralateralMaskerLevel.HasValue = False Then Throw New ArgumentException("ContralateralMaskerLevel value cannot be Nothing!")

            'Ensures that head phones are used
            If TestOptions.SelectedTransducer.IsHeadphones = False Then
                Throw New Exception("Contralateral masking cannot be used without headphone presentation.")
            End If

            'Ensures that it's not a simulated sound field
            If TestOptions.UseSimulatedSoundField = True Then
                Throw New Exception("Contralateral masking cannot be used in a simulated sound field!")
            End If

            'Getting the contralateral masker sound 
            Dim FullContralateralMaskerSound = CurrentTestTrial.SpeechMaterialComponent.GetContralateralMaskerSound(TestOptions.SelectedMediaSet, 0)

            'Storing the samplerate
            If CurrentSampleRate = -1 Then CurrentSampleRate = FullContralateralMaskerSound.WaveFormat.SampleRate

            'Setting the insertion sample of the contralateral masker
            Dim ContralateralMaskerStartSample As Integer = Math.Floor(ContralateralMaskerPresentationTime * CurrentSampleRate)

            'Setting the ContralateralMaskerStartMeasureSample (i.e. the sample index in the ContralateralMaskerSound, not in the final mix)
            Dim ContralateralMaskerStartMeasureSample As Integer = 0

            'Picking a random section of the ContralateralMaskerSound, starting in the first half
            Dim RandomStartReadIndex As Integer = Randomizer.Next(0, (FullContralateralMaskerSound.WaveData.SampleData(1).Length / 2) - 1)
            Dim IndendedMaskerLength As Integer = MaximumSoundDuration * CurrentSampleRate - ContralateralMaskerStartSample

            'Calculating the needed sound length and checks that the contralateral masker sound is long enough
            Dim NeededSoundLength As Integer = RandomStartReadIndex + IndendedMaskerLength + 10
            If FullContralateralMaskerSound.WaveData.SampleData(1).Length < NeededSoundLength Then
                Throw New Exception("The contralateral masker sound specified is too short for the intended maximum sound duration!")
            End If

            'Gets a copy of the sound section
            Dim ContralateralMaskerSound = Audio.DSP.CopySection(FullContralateralMaskerSound, RandomStartReadIndex, IndendedMaskerLength, 1)

            'Copying the SMA object to retain the nominal level (although other time data and other related stuff will be incorrect, if not adjusted for)
            ContralateralMaskerSound.SMA = FullContralateralMaskerSound.SMA.CreateCopy(ContralateralMaskerSound)

            'Getting the ContralateralMaskerMeasureLength from the length of the sound files (i.e. everything is measured)
            Dim ContralateralMaskerMeasureLength As Integer = ContralateralMaskerSound.WaveData.SampleData(1).Length

            'Sets up fading specifications for the contralateral masker
            If FadeSpecs_ContralateralMasker Is Nothing Then
                FadeSpecs_ContralateralMasker = New List(Of STFN.Audio.DSP.Transformations.FadeSpecifications)
                FadeSpecs_ContralateralMasker.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 0.002))
                FadeSpecs_ContralateralMasker.Add(New STFN.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.002))
            End If

            'Determining which side to put the contralateral masker
            Dim ContralateralMaskerLocation As New SoundSourceLocation With {.Distance = 0, .Elevation = 0}
            If CurrentTestEar = Utils.Constants.SidesWithBoth.Left Then
                'Putting contralateral masker in right ear
                ContralateralMaskerLocation.HorizontalAzimuth = 90
            ElseIf CurrentTestEar = Utils.Constants.SidesWithBoth.Right Then
                'Putting contralateral masker in left ear
                ContralateralMaskerLocation.HorizontalAzimuth = -90
            Else
                'This shold never happen...
                Throw New Exception("Contralateral noise cannot be used when the target signal is on both sides!")
            End If

            'Adding the contralateral maskers sources to the ItemList
            ItemList.Add(New SoundSceneItem(ContralateralMaskerSound, 1, ContralateralMaskerLevel_EmCorrected, LevelGroup, ContralateralMaskerLocation, SoundSceneItem.SoundSceneItemRoles.ContralateralMasker, ContralateralMaskerStartSample, ContralateralMaskerStartMeasureSample, ContralateralMaskerMeasureLength,, FadeSpecs_ContralateralMasker))

            'Incrementing LevelGroup 
            LevelGroup += 1

        End If


        Dim CurrentSoundPropagationType As SoundPropagationTypes = SoundPropagationTypes.PointSpeakers
        If TestOptions.UseSimulatedSoundField Then
            CurrentSoundPropagationType = SoundPropagationTypes.SimulatedSoundField
            'TODO: This needs to be modified if/when more SoundPropagationTypes are starting to be supported
        End If

        'Creating the mix by calling CreateSoundScene of the current Mixer
        CurrentTestTrial.Sound = TestOptions.SelectedTransducer.Mixer.CreateSoundScene(ItemList, UseNominalLevels, TestOptions.UseRetsplCorrection, CurrentSoundPropagationType, TestOptions.SelectedTransducer.LimiterThreshold, ExportSounds, CurrentTestTrial.Spelling)


        'TODO: Reasonably this method should only store values into the CurrentTestTrial that are derived within this function! Leaving these for now
        CurrentTestTrial.MediaSetName = TestOptions.SelectedMediaSet.MediaSetName
        CurrentTestTrial.UseContralateralNoise = TestOptions.UseContralateralMasking
        CurrentTestTrial.EfficientContralateralMaskingTerm = TestOptions.SelectedMediaSet.EffectiveContralateralMaskingGain

    End Sub



#End Region

    ''' <summary>
    ''' The sound player crossfade overlap to be used between trials, fade-in and fade-out
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Property SoundOverlapDuration As Double

#Region "Test protocol"

    Public Shared Randomizer As Random = New Random

    Public MustOverride ReadOnly Property AvailableTestModes As List(Of TestModes)

    Public Enum TestModes
        ConstantStimuli
        AdaptiveSpeech
        AdaptiveNoise
        AdaptiveDirectionality
        Custom
    End Enum

    Public MustOverride ReadOnly Property AvailableTestProtocols() As List(Of TestProtocol)

    Public MustOverride ReadOnly Property UseKeyWordScoring As Utils.TriState
    Public MustOverride ReadOnly Property UseListOrderRandomization As Utils.TriState
    Public MustOverride ReadOnly Property UseWithinListRandomization As Utils.TriState
    Public MustOverride ReadOnly Property UseAcrossListRandomization As Utils.TriState
    Public MustOverride ReadOnly Property UseFreeRecall As Utils.TriState
    Public MustOverride ReadOnly Property UseDidNotHearAlternative As Utils.TriState
    Public MustOverride ReadOnly Property AvailableFixedResponseAlternativeCounts() As List(Of Integer)
    Public MustOverride ReadOnly Property UseContralateralMasking As Utils.TriState
    Public MustOverride ReadOnly Property AvailablePhaseAudiometryTypes() As List(Of BmldModes)
    Public MustOverride ReadOnly Property UsePhaseAudiometry As Utils.TriState

    ''' <summary>
    ''' If True, speech and noise levels should be interpreted as dB HL. If False, speech and noise levels should be interpreted as dB SPL.
    ''' </summary>
    ''' <returns></returns>
    Public Property UseRetsplCorrection As Boolean

    Public MustOverride ReadOnly Property DefaultReferenceLevel As Double
    Public MustOverride ReadOnly Property DefaultSpeechLevel As Double
    Public MustOverride ReadOnly Property DefaultMaskerLevel As Double
    Public MustOverride ReadOnly Property DefaultBackgroundLevel As Double
    Public MustOverride ReadOnly Property DefaultContralateralMaskerLevel As Double

    Public MustOverride ReadOnly Property MinimumReferenceLevel As Double
    Public MustOverride ReadOnly Property MaximumReferenceLevel As Double

    Public MustOverride ReadOnly Property MinimumLevel_Targets As Double
    Public MustOverride ReadOnly Property MaximumLevel_Targets As Double

    Public MustOverride ReadOnly Property MinimumLevel_Maskers As Double
    Public MustOverride ReadOnly Property MaximumLevel_Maskers As Double

    Public MustOverride ReadOnly Property MinimumLevel_Background As Double
    Public MustOverride ReadOnly Property MaximumLevel_Background As Double

    Public MustOverride ReadOnly Property MinimumLevel_ContralateralMaskers As Double
    Public MustOverride ReadOnly Property MaximumLevel_ContralateralMaskers As Double

#End Region

#Region "RunningTest"

    Public CurrentTestTrial As TestTrial

    ''' <summary>
    ''' This feid can be used to store information that should be shown on screen during pause. 
    ''' </summary>
    Public PauseInformation As String = ""

    Public AbortInformation As String = ""

    Public MustOverride ReadOnly Property HistoricTrialCount As Integer


#End Region

#Region "TestResults"

    Public Shared Function GetAverageScore(ByVal Trials As IEnumerable(Of TestTrial)) As Double?

        Dim ScoreList As New List(Of Integer)
        For Each Trial In Trials
            If Trial.IsCorrect = True Then
                ScoreList.Add(1)
            Else
                ScoreList.Add(0)
            End If
        Next
        If ScoreList.Count > 0 Then
            Return ScoreList.Average
        Else
            Return Nothing
        End If

    End Function

    Public Shared Function GetNumbersOfCorrectTrials(ByVal Trials As IEnumerable(Of TestTrial)) As Double?

        Dim ScoreList As New List(Of Integer)
        For Each Trial In Trials
            If Trial.IsCorrect = True Then
                ScoreList.Add(1)
            Else
                ScoreList.Add(0)
            End If
        Next
        If ScoreList.Count > 0 Then
            Return ScoreList.Sum
        Else
            Return Nothing
        End If

    End Function



#End Region

#Region "Settings"

    Public Property TestOptions As TestOptions


#End Region


#Region "MustOverride members used in derived classes"

    ''' <summary>
    ''' Initializes the current test
    ''' </summary>
    ''' <returns>A tuple in which the boolean value indicates success, and the string is an optional message that may be relayed to the user.</returns>
    Public MustOverride Function InitializeCurrentTest() As Tuple(Of Boolean, String)

    ''' <summary>
    ''' This method must be implemented in the derived class and must return a decision on what steps to take next. If the next step to take involves a new test trial this method is also responsible for referencing the next test trial in the CurrentTestTrial field.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <returns></returns>
    Public MustOverride Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

    Public MustOverride Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)

    Public Enum SpeechTestReplies
        ContinueTrial
        GotoNextTrial
        PauseTestingWithCustomInformation
        TestIsCompleted
        AbortTest
    End Enum

    Public MustOverride Sub FinalizeTest()

    Public MustOverride Function GetResultStringForGui() As String

    Public MustOverride Function GetTestResultsExportString() As String

    Public MustOverride Function GetTestTrialResultExportString() As String

    Public MustOverride ReadOnly Property FilePathRepresentation As String

    Public Function SaveTestTrialResults() As Boolean

        'Skipping saving data if it's the demo ptc ID
        If SharedSpeechTestObjects.CurrentParticipantID.Trim = SharedSpeechTestObjects.NoTestId Then Return True

        If SharedSpeechTestObjects.TestResultsRootFolder = "" Then
            Messager.MsgBox("Unable to save the results to file due to missing test results output folder. This should have been selected first startup of the app!")
            Return False
        End If

        If IO.Directory.Exists(SharedSpeechTestObjects.TestResultsRootFolder) = False Then
            Try
                IO.Directory.CreateDirectory(SharedSpeechTestObjects.TestResultsRootFolder)
            Catch ex As Exception
                Messager.MsgBox("Unable to save the results to the test results output folder (" & SharedSpeechTestObjects.TestResultsRootFolder & "). The path does not exist, and could not be created!")
            End Try
            Return False
        End If

        Dim OutputPath = IO.Path.Combine(SharedSpeechTestObjects.TestResultsRootFolder, Me.FilePathRepresentation)
        Dim OutputFilename = Me.FilePathRepresentation & "_TrialResults_" & SharedSpeechTestObjects.CurrentParticipantID

        Dim TestTrialResultsString = GetTestTrialResultExportString()
        Utils.SendInfoToLog(TestTrialResultsString, OutputFilename, OutputPath, False, True, False, True, True)

        Return True

    End Function

    Public Function SaveTableFormatedTestResults() As Boolean

        'Skipping saving data if it's the demo ptc ID
        If SharedSpeechTestObjects.CurrentParticipantID.Trim = SharedSpeechTestObjects.NoTestId Then Return True

        If SharedSpeechTestObjects.TestResultsRootFolder = "" Then
            Messager.MsgBox("Unable to save the results to file due to missing test results output folder. This should have been selected first startup of the app!")
            Return False
        End If

        If IO.Directory.Exists(SharedSpeechTestObjects.TestResultsRootFolder) = False Then
            Try
                IO.Directory.CreateDirectory(SharedSpeechTestObjects.TestResultsRootFolder)
            Catch ex As Exception
                Messager.MsgBox("Unable to save the results to the test results output folder (" & SharedSpeechTestObjects.TestResultsRootFolder & "). The path does not exist, and could not be created!")
            End Try
            Return False
        End If

        Dim OutputPath = IO.Path.Combine(SharedSpeechTestObjects.TestResultsRootFolder, Me.FilePathRepresentation)
        Dim OutputFilename = Me.FilePathRepresentation & "_Results_" & SharedSpeechTestObjects.CurrentParticipantID

        Dim TestResultsString = GetTestResultsExportString()
        Utils.SendInfoToLog(TestResultsString, OutputFilename, OutputPath, False, True, False, False, True)

        Return True
    End Function


#End Region

#Region "Pretest"

    Public MustOverride Function CreatePreTestStimulus() As Tuple(Of Audio.Sound, String)

#End Region

End Class

