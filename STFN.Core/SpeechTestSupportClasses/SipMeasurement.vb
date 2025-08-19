Imports STFN.Core.Audio.SoundScene
Imports STFN.Core.Audio.Formats
Imports System.Reflection
Imports System.Runtime.Serialization

Namespace SipTest

    'N.B. Variable name abbreviations used:
    ' RLxs = ReferenceContrastingPhonemesLevel (dB FS)
    ' SLs = Phoneme Spectrum Levels (PSL, dB SPL)
    ' SLm = Environment / Masker Spectrum Levels (ESL, dB SPL)
    ' Lc = Component Level (TestWord_ReferenceSPL, dB SPL), The average sound level of all recordings of the speech material component
    ' Tc = Component temporal duration (in seconds)
    ' V = HasVowelContrast (1 = Yes, 0 = No)

    Public Class SipMeasurement

        Public Property Description As String = ""

        Public Property ParticipantID As String

        Public Property SelectedAudiogramData As AudiogramData = Nothing

        Public Property HearingAidGain As HearingAidGainData = Nothing

        Public Property ParentTestSpecification As SpeechMaterialSpecification

        Public Property MeasurementDateTime As DateTime


        ''' <summary>
        ''' Stores references to SiP-test trials in the order that they were presented.
        ''' </summary>
        ''' <returns></returns>
        Public Property ObservedTrials As New List(Of SipTrial)

        Public Property PlannedTrials As New List(Of SipTrial)

        ''' <summary>
        ''' Stores the test units presented in the test session
        ''' </summary>
        ''' <returns></returns>
        Public Property TestUnits As List(Of SiPTestUnit)


        ''' <summary>
        ''' Holds settings that determine how the test should enfold.
        ''' </summary>
        ''' <returns></returns>
        Public Property TestProcedure As SiPTestProcedure

        Public TrialResultsExportFolder As String

        Public Randomizer As Random

        Public ExportTrialSoundFiles As Boolean = False


        Public Sub New(ByRef ParticipantID As String, ByRef ParentTestSpecification As SpeechMaterialSpecification, Optional AdaptiveType As SiPTestProcedure.AdaptiveTypes = SiPTestProcedure.AdaptiveTypes.Fixed, Optional ByVal TestParadigm As SiPTestProcedure.SiPTestparadigm = SiPTestProcedure.SiPTestparadigm.Slow, Optional RandomSeed As Integer? = Nothing)

            If RandomSeed.HasValue = True Then
                Randomizer = New Random(RandomSeed)
            Else
                Randomizer = New Random
            End If

            Me.TestProcedure = New SiPTestProcedure(AdaptiveType, TestParadigm)

            Me.ParticipantID = ParticipantID
            Me.ParentTestSpecification = ParentTestSpecification

        End Sub


        ''' <summary>
        ''' Moves the referenced test trial from the PlannedTrials to ObservedTrials objects, both in the parent TestUnit and in the parent SipMeasurement. This way it will not be presented again.
        ''' </summary>
        ''' <param name="TestTrial"></param>
        ''' <param name="RemoveSounds">If True, removes the Sound in the referenced trial.</param>
        Public Sub MoveTrialToHistory(ByRef TestTrial As SipTrial, Optional ByVal RemoveSounds As Boolean = True)

            Dim ParentTestUnit = TestTrial.ParentTestUnit

            'Adding the trial to the history
            ObservedTrials.Add(TestTrial)
            ParentTestUnit.ObservedTrials.Add(TestTrial)

            'Removes the next trial from PlannedTrials
            PlannedTrials.Remove(TestTrial)
            ParentTestUnit.PlannedTrials.Remove(TestTrial)

            'Increments the number of presented trials, based on the number of trials stored in ObservedTrials so far.
            TestTrial.PresentationOrder = ObservedTrials.Count

            If RemoveSounds = True Then
                TestTrial.RemoveSounds()
            End If

        End Sub

        Public Sub ClearTrials()
            TestUnits.Clear()
            PlannedTrials.Clear()
            ObservedTrials.Clear()
        End Sub

        'Looks through all trials and returns True as soon as a trial that requires directional sound field simulation is detected, or false if no trial requires directional sound field simulation.
        Public Function HasSimulatedSoundFieldTrials() As Boolean

            For Each Trial In PlannedTrials
                If Trial.SoundPropagationType = Utils.SoundPropagationTypes.SimulatedSoundField Then Return True
            Next

            For Each Trial In ObservedTrials
                If Trial.SoundPropagationType = Utils.SoundPropagationTypes.SimulatedSoundField Then Return True
            Next

            For Each TestUnit In TestUnits
                For Each Trial In TestUnit.PlannedTrials
                    If Trial.SoundPropagationType = Utils.SoundPropagationTypes.SimulatedSoundField Then Return True
                Next
                For Each Trial In TestUnit.ObservedTrials
                    If Trial.SoundPropagationType = Utils.SoundPropagationTypes.SimulatedSoundField Then Return True
                Next
            Next

            Return False
        End Function



        Public Sub PreMixTestTrialSounds(ByRef SelectedTransducer As AudioSystemSpecification,
                                         ByVal MinimumStimulusOnsetTime As Double, ByVal MaximumStimulusOnsetTime As Double,
                                         ByRef SipMeasurementRandomizer As Random, ByVal TrialSoundMaxDuration As Double, ByVal UseBackgroundSpeech As Boolean,
                                         Optional ByVal StopAfter As Integer? = 10, Optional SameRandomSeed As Integer? = Nothing,
                                         Optional ByVal FixedMaskerIndices As List(Of Integer) = Nothing, Optional ByVal FixedSpeechIndex As Integer? = Nothing)

            Dim MixedCount As Integer = 0
            Dim RemainingPlannedTrials = PlannedTrials.GetRange(0, PlannedTrials.Count)

            If LogToConsole = True Then
                If StopAfter.HasValue Then
                    Console.WriteLine("Starts mixing " & StopAfter.Value & " trial sounds...")
                Else
                    Console.WriteLine("Starts mixing new trial sounds...")
                End If
            End If

            Dim LastListId As String = ""

            For Each Trial In RemainingPlannedTrials

                If SameRandomSeed.HasValue Then

                    If LastListId <> "" Then
                        LastListId = Trial.SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).Id
                    Else
                        If LastListId <> Trial.SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).Id Then
                            'Incrementing SameRandomSeed by 1 to get a new random value for the new list
                            SameRandomSeed += 1
                        End If
                    End If

                    'Forces the same random series for every trial in the same list level (to force the background sound sections in all trials)
                    SipMeasurementRandomizer = New Random(SameRandomSeed)
                End If

                If Trial.Sound Is Nothing Then
                    Trial.MixSound(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, FixedMaskerIndices, FixedSpeechIndex)
                    MixedCount += 1

                    If LogToConsole = True Then
                        If Trial.Sound IsNot Nothing Then
                            Console.WriteLine("Mixed trial sound: " & MixedCount)
                        Else
                            Console.WriteLine("   Failed to mix trial sound: " & MixedCount)
                        End If
                    End If

                    'Stops after mixing StopAfter new sounds (this can be utilized in order not to bulid up too much memory)
                    If StopAfter.HasValue Then
                        If MixedCount >= StopAfter.Value Then Exit For
                    End If
                End If
            Next

        End Sub


        Public Sub PreMixTestTrialSoundsOnNewTread(ByRef SelectedTransducer As AudioSystemSpecification,
                                 ByVal MinimumStimulusOnsetTime As Double, ByVal MaximumStimulusOnsetTime As Double,
                                 ByRef SipMeasurementRandomizer As Random, ByVal TrialSoundMaxDuration As Double, ByVal UseBackgroundSpeech As Boolean,
                                 Optional ByVal StopAfter As Integer? = 10, Optional SameRandomSeed As Integer? = Nothing, Optional ByVal FixedMaskerIndices As List(Of Integer) = Nothing)

            Dim NewTestTrialSoundMixClass = New TestTrialSoundMixerOnNewThread(Me, SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech,
                                                                               StopAfter, SameRandomSeed, FixedMaskerIndices)


        End Sub


        Private Class TestTrialSoundMixerOnNewThread

            Public SipMeasurement As SipMeasurement
            Public SelectedTransducer As AudioSystemSpecification
            Public MinimumStimulusOnsetTime As Double
            Public MaximumStimulusOnsetTime As Double
            Public SipMeasurementRandomizer As Random
            Public TrialSoundMaxDuration As Double
            Public UseBackgroundSpeech As Boolean
            Public StopAfter As Integer? = 10

            Public SameRandomSeed As Integer? = Nothing
            Public FixedMaskerIndices As List(Of Integer) = Nothing

            Public Sub New(ByRef SipMeasurement As SipMeasurement, ByRef SelectedTransducer As AudioSystemSpecification,
                                         ByVal MinimumStimulusOnsetTime As Double, ByVal MaximumStimulusOnsetTime As Double,
                                         ByRef SipMeasurementRandomizer As Random, ByVal TrialSoundMaxDuration As Double, ByVal UseBackgroundSpeech As Boolean,
                                         Optional ByVal StopAfter As Integer? = 10,
                           Optional SameRandomSeed As Integer? = Nothing, Optional ByVal FixedMaskerIndices As List(Of Integer) = Nothing)

                Me.SipMeasurement = SipMeasurement
                Me.SelectedTransducer = SelectedTransducer
                Me.MinimumStimulusOnsetTime = MinimumStimulusOnsetTime
                Me.MaximumStimulusOnsetTime = MaximumStimulusOnsetTime
                Me.SipMeasurementRandomizer = SipMeasurementRandomizer
                Me.TrialSoundMaxDuration = TrialSoundMaxDuration
                Me.UseBackgroundSpeech = UseBackgroundSpeech
                Me.StopAfter = StopAfter

                Me.SameRandomSeed = SameRandomSeed
                Me.FixedMaskerIndices = FixedMaskerIndices

                Dim NewTread As New Threading.Thread(AddressOf DoWork)
                NewTread.IsBackground = True
                NewTread.Start()

            End Sub

            Public Sub DoWork()
                SipMeasurement.PreMixTestTrialSounds(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, StopAfter,
                                                     SameRandomSeed, FixedMaskerIndices)
            End Sub

        End Class


        Public Function GetNextTrial() As SipTrial

            If PlannedTrials.Count = 0 Then
                Return Nothing
            Else

                'Returns the next planed trial
                Return PlannedTrials(0)
            End If

        End Function

        Public Function GetTargetAzimuths() As List(Of Double)

            Dim AvailableTargetDirections As New SortedSet(Of Double)

            For Each PlannedTrial In PlannedTrials
                For i = 0 To PlannedTrial.TargetStimulusLocations.Length - 1

                    AvailableTargetDirections.Add(PlannedTrial.TargetStimulusLocations(i).HorizontalAzimuth)

                    'TODO we must adjust these to the available and speakers in the selected transducer

                    'Adding tha actual azimuth
                    PlannedTrial.TargetStimulusLocations(i).ActualLocation = New SoundSourceLocation
                    PlannedTrial.TargetStimulusLocations(i).ActualLocation.HorizontalAzimuth = PlannedTrial.TargetStimulusLocations(i).HorizontalAzimuth

                Next

            Next


            Return AvailableTargetDirections.ToList

        End Function

        Public Sub SetDefaultExportPath()
            Me.TrialResultsExportFolder = IO.Path.Combine(Logging.LogFileDirectory, ParentTestSpecification.Name.Replace(" ", "_") & "_" & Description.Replace(" ", "_") & "_" & ParticipantID.Replace(" ", "_"))
        End Sub

        ''' <summary>
        ''' Returns the index at which the ParentTestUnit of the Referenced TestTrial is stored within the TestUnits list of the current instance of SiPMeasurement, or -1 if the test unit does not exist, or cannot be found.
        ''' </summary>
        ''' <param name="TestTrial"></param>
        ''' <returns></returns>
        Public Function GetParentTestUnitIndex(ByRef TestTrial As SipTrial) As Integer

            If TestTrial Is Nothing Then Return -1
            If TestTrial.ParentTestUnit Is Nothing Then Return -1

            For i = 0 To TestUnits.Count - 1
                If TestTrial.ParentTestUnit Is TestUnits(i) Then
                    Return i
                End If
            Next

            Return -1
        End Function




    End Class


End Namespace