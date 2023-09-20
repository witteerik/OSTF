Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows.Forms

Namespace Audio

    Namespace PortAudioVB

        Public Class DuplexMixer

            Private ParentTransducerSpecification As AudioSystemSpecification

            ''' <summary>
            ''' A list of key-value pairs, where the key repressents the hardware output channel and the value repressents the wave file channel from which the output sound should be drawn.
            ''' </summary>
            Public OutputRouting As New SortedList(Of Integer, Integer)
            ''' <summary>
            ''' A list of key-value pairs, where the key repressents the hardware input channel and the value repressents the wave file channel in which the input sound should be stored.
            ''' </summary>
            Public InputRouting As New SortedList(Of Integer, Integer)

            ''' <summary>
            ''' A list of key-value pairs, where the key repressents the hardware output channel and the values repressents the physical location of the loadspeaker connected to that output channel.
            ''' </summary>
            Public HardwareOutputChannelSpeakerLocations As New SortedList(Of Integer, SoundSourceLocation)

            ''' <summary>
            ''' Holds the gain (value) for the hardwave output channel (key).
            ''' </summary>
            Private _CalibrationGain As New SortedList(Of Integer, Double)

            ''' <summary>
            ''' Returns the CalibrationGain (in dB) for the loudspeaker connected to the indicated hardware output channel.
            ''' </summary>
            ''' <param name="HardWareOutputChannel"></param>
            ''' <returns></returns>
            Public ReadOnly Property CalibrationGain(ByVal HardWareOutputChannel As Integer) As Double
                Get
                    If _CalibrationGain.ContainsKey(HardWareOutputChannel) Then
                        Return _CalibrationGain(HardWareOutputChannel)
                    Else
                        MsgBox("Calibration has not been set for hardware output channel " & HardWareOutputChannel & vbCrLf & vbCrLf &
                       "Click OK to use the default calibration gain of " & 0 & " dB!", MsgBoxStyle.Exclamation, "Warning!")
                        Return 0
                    End If
                End Get
            End Property


            ''' <summary>
            ''' Creating a new mixer.
            ''' </summary>
            Public Sub New(Optional ByVal ParentTransducerSpecification As AudioSystemSpecification = Nothing)

                Try

                    'If ParentTransducerSpecification is not initialized, the first available transducer is selected
                    If ParentTransducerSpecification Is Nothing Then ParentTransducerSpecification = OstfBase.AvaliableTransducers(0)

                    Me.ParentTransducerSpecification = ParentTransducerSpecification

                    'Sets up the routing
                    Dim CorrespondingWaveDataChannel As Integer = 1
                    For Each c In ParentTransducerSpecification.HardwareOutputChannels
                        OutputRouting.Add(c, CorrespondingWaveDataChannel)
                        CorrespondingWaveDataChannel += 1
                    Next

                    'Sets soundsource locations
                    For i = 0 To ParentTransducerSpecification.HardwareOutputChannels.Count - 1

                        Me.HardwareOutputChannelSpeakerLocations.Add(ParentTransducerSpecification.HardwareOutputChannels(i),
                                                                 New SoundSourceLocation With {
                                                                 .HorizontalAzimuth = ParentTransducerSpecification.LoudspeakerAzimuths(i),
                                                                 .Elevation = ParentTransducerSpecification.LoudspeakerElevations(i),
                                                                 .Distance = ParentTransducerSpecification.LoudspeakerDistances(i)})
                    Next

                    'Sets calibration
                    For i = 0 To ParentTransducerSpecification.HardwareOutputChannels.Count - 1
                        Me._CalibrationGain.Add(ParentTransducerSpecification.HardwareOutputChannels(i), ParentTransducerSpecification.CalibrationGain(i))
                    Next

                    'Sets linear input as default
                    SetLinearInput()

                Catch ex As Exception
                    MsgBox("An error occurred! The following error message may be relevant: " & vbCrLf & ex.Message & vbCrLf & vbCrLf & "If the above error message is not relevant, make sure you have the correct (and equal) number of values for a) HardwareOutputChannels, b) LoudspeakerAzimuths, c) LoudspeakerElevations, d) LoudspeakerDistances and e) CalibrationGain in the AudioSystemSpecification.txt file!", MsgBoxStyle.Critical, "Error: " & ex.ToString)
                End Try

            End Sub

            Public Sub DirectMonoSoundToOutputChannel(ByRef TargetOutputChannel As Integer)
                If OutputRouting.ContainsKey(TargetOutputChannel) Then OutputRouting(TargetOutputChannel) = 1
            End Sub

            Public Sub DirectMonoSoundToOutputChannels(ByRef TargetOutputChannels() As Integer)
                For Each OutputChannel In TargetOutputChannels
                    If OutputRouting.ContainsKey(OutputChannel) Then OutputRouting(OutputChannel) = 1
                Next
            End Sub

            Public Sub DirectMonoToAllChannels()
                OutputRouting.Clear()

                For c = 1 To ParentTransducerSpecification.ParentAudioApiSettings.NumberOfOutputChannels
                    OutputRouting.Add(c, 1)
                Next

                InputRouting.Clear()

                For c = 1 To ParentTransducerSpecification.ParentAudioApiSettings.NumberOfInputChannels
                    InputRouting.Add(c, 1)
                Next
            End Sub

            Public Sub SetLinearOutput()
                OutputRouting.Clear()
                For c = 1 To ParentTransducerSpecification.ParentAudioApiSettings.NumberOfOutputChannels
                    OutputRouting.Add(c, c)
                Next
            End Sub

            Public Sub SetLinearInput()
                InputRouting.Clear()
                For c = 1 To ParentTransducerSpecification.ParentAudioApiSettings.NumberOfInputChannels
                    InputRouting.Add(c, c)
                Next
            End Sub


#Region "Calibration"

            ''' <summary>
            ''' Call this sub to set the loudspeaker or headphone calibration of the current instance of DuplexMixer.
            ''' </summary>
            ''' <param name="CalibrationGain"></param>
            Public Sub SetCalibrationValues(ByVal CalibrationGain As SortedList(Of Integer, Double))

                'Setting the private field _CalibrationGain. Values are retrieved by the public Readonly Property CalibrationGain 
                Me._CalibrationGain = CalibrationGain

            End Sub


#End Region

            Public Class SoundSceneItem
                Public Sound As Audio.Sound
                Public ReadChannel As Integer
                Public SoundLevel As Double
                Public SoundLevelFormat As Audio.Formats.SoundLevelFormat = Nothing

                ''' <summary>
                ''' An arbitrary grouping value. Here, grouping indicates that the sum of all sound sources with the same LevelGroup value should equal the specified SoundLevel 
                ''' (This means that the SoundLevel cannot differ between SoundSceneItem within the same LevelGroup. The calling code is responsible to make sure that that doesn't happen. And an exception will be thrown if it does.).
                ''' </summary>
                Public LevelGroup As Integer

                Public LevelDefStartSample As Integer?
                Public LevelDefLength As Integer?

                Public SourceLocation As SoundSourceLocation

                Public InsertSample As Integer

                ''' <summary>
                ''' Specifications of fadings. Note that this can also be used to create duckings, by adding a (partial) fade out, attenuation section, and a (partial) fade in.
                ''' </summary>
                Public FadeSpecifications As List(Of Audio.DSP.FadeSpecifications)

                ''' <summary>
                ''' Specifications of ducking periods. A ducking perdio is specified by combining two fade out and fade in events. Multiple duckings may be specified by listing several fade in-fade out events.
                ''' </summary>
                Public DuckingSpecifications As List(Of Audio.DSP.FadeSpecifications)

                Public Sub New(ByRef Sound As Audio.Sound, ByVal ReadChannel As Integer,
                               ByVal SoundLevel As Double, ByVal LevelGroup As Integer,
                               ByRef SourceLocation As SoundSourceLocation,
                               Optional ByVal InsertSample As Integer = 0,
                               Optional ByVal LevelDefStartSample As Integer? = Nothing, Optional ByVal LevelDefLength As Integer? = Nothing,
                               Optional ByRef SoundLevelFormat As Audio.Formats.SoundLevelFormat = Nothing,
                               Optional ByRef FadeSpecifications As List(Of Audio.DSP.FadeSpecifications) = Nothing,
                               Optional ByRef DuckingSpecifications As List(Of Audio.DSP.FadeSpecifications) = Nothing)

                    Me.Sound = Sound
                    Me.ReadChannel = ReadChannel
                    Me.SoundLevel = SoundLevel
                    Me.LevelGroup = LevelGroup
                    Me.SourceLocation = SourceLocation

                    Me.InsertSample = InsertSample

                    Me.LevelDefStartSample = LevelDefStartSample
                    Me.LevelDefLength = LevelDefLength

                    Me.FadeSpecifications = FadeSpecifications
                    Me.DuckingSpecifications = DuckingSpecifications

                    'Setting a defeult average Z-weighted sound level format, if none is supplied.
                    If SoundLevelFormat IsNot Nothing Then
                        Me.SoundLevelFormat = SoundLevelFormat
                    Else
                        Me.SoundLevelFormat = New Formats.SoundLevelFormat(SoundMeasurementTypes.Average_Z_Weighted)
                    End If

                End Sub

            End Class

            Public Class SoundSourceLocation
                ''' <summary>
                ''' The distance to the sound source in meters.
                ''' </summary>
                Public Distance As Double = 1

                ''' <summary>
                ''' The horizontal azimuth of the sound source in degrees, in relation to front (zero degrees), positive values to the right and negative values to the left.
                ''' </summary>
                Public HorizontalAzimuth As Double = 0

                ''' <summary>
                ''' The verical elevation of the sound source in degrees, where zero degrees refers to the horizontal plane, positive values upwards and negative values downwards.
                ''' </summary>
                Public Elevation As Double = 0

                ''' <summary>
                ''' After presentation, this object should hold the actual location of the presented sound source as limited by the available speakers or limitations of the sound field simulator used.
                ''' </summary>
                Public ActualLocation As SoundSourceLocation

            End Class


            Public Function CreateSoundScene(ByRef Input As List(Of SoundSceneItem), ByVal SoundPropagationType As SoundPropagationTypes, Optional ByVal LimiterThreshold As Double? = 100) As Audio.Sound

                Try


                    Dim WaveFormat As Audio.Formats.WaveFormat = Nothing

                    'Copy sounds so that their individual sample data in the selected channel to a new (mono) sound so that the sample data may be changed without changing the original sound,
                    ' and addes them in a new list of SoundSceneItem, which should henceforth be used instead of the Input object.
                    ' However SourceLocation is referenced, as it revieces its ActualLocation value as part of the algorithm
                    ' TODO: SoundLevelFormat, FadeSpecifications, and DuckingSpecifications is still used and not copied! May be allright?!
                    Dim SoundSceneItemList As New List(Of SoundSceneItem)
                    For Each Item In Input
                        'Creates a new NewSoundSceneItem 
                        Dim NewSoundSceneItem = New SoundSceneItem(Item.Sound.CopyChannelToMonoSound(Item.ReadChannel), 1, Item.SoundLevel, Item.LevelGroup,
                                                                Item.SourceLocation,
                                                                Item.InsertSample, Item.LevelDefStartSample, Item.LevelDefLength,
                                                               Item.SoundLevelFormat, Item.FadeSpecifications, Item.DuckingSpecifications)
                        'Adds the NewSoundSceneItem 
                        SoundSceneItemList.Add(NewSoundSceneItem)

                        'Also gets and checks the wave formats for equality
                        If WaveFormat Is Nothing Then
                            WaveFormat = NewSoundSceneItem.Sound.WaveFormat
                        Else
                            If WaveFormat.IsEqual(NewSoundSceneItem.Sound.WaveFormat) = False Then Throw New ArgumentException("Different wave formats detected when mixing sound files.")
                        End If
                    Next


                    ' Setting levels
                    'Creating sound level groups
                    Dim SoundLevelGroups = SetupSoundLevelGroups(SoundSceneItemList)
                    For Each SoundLevelGroup In SoundLevelGroups

                        Dim TargetLevel = SoundLevelGroup.Value.Item1
                        Dim SoundLevelFormat = SoundLevelGroup.Value.Item2
                        Dim GroupMembers = SoundLevelGroup.Value.Item3

                        'Gets the measurement sounds
                        Dim GroupMemberMeasurementSounds As New List(Of Sound)
                        For Each GroupMember In GroupMembers
                            If GroupMember.LevelDefStartSample.HasValue = False And GroupMember.LevelDefLength.HasValue = False Then
                                GroupMemberMeasurementSounds.Add(GroupMember.Sound)
                            Else
                                'Checks that both have value
                                If GroupMember.LevelDefStartSample.HasValue = True And GroupMember.LevelDefLength.HasValue = True Then
                                    GroupMemberMeasurementSounds.Add(GroupMember.Sound.CopySection(GroupMember.ReadChannel, GroupMember.LevelDefStartSample, GroupMember.LevelDefLength))
                                Else
                                    Throw New ArgumentException("Either none or both of the SoundSceneItem parameters LevelDefStartSample and LevelDefLength must have a value.")
                                End If
                            End If
                        Next

                        Dim MasterMeasurementSound As Audio.Sound = Nothing
                        If GroupMemberMeasurementSounds.Count = 1 Then

                            'References the GroupMemberMeasurementSounds(0) into MasterMeasurementSound 
                            MasterMeasurementSound = GroupMemberMeasurementSounds(0)
                        ElseIf GroupMemberMeasurementSounds.Count > 1 Then

                            'TODO Equalize level before superpositioning?

                            'Adds the measurement sounds into MasterMeasurementSound 
                            MasterMeasurementSound = Audio.DSP.SuperpositionSounds(GroupMemberMeasurementSounds)
                        Else
                            Throw New ArgumentException("Missing sound in SoundSceneItem.")
                        End If

                        'Measures the sound levels
                        Dim CurrentLevel As Double
                        If SoundLevelFormat.LoudestSectionMeasurement = False Then
                            CurrentLevel = Audio.DSP.MeasureSectionLevel(MasterMeasurementSound, 1,,,,, SoundLevelFormat.FrequencyWeighting)
                        Else
                            CurrentLevel = Audio.DSP.GetLevelOfLoudestWindow(MasterMeasurementSound, 1, WaveFormat.SampleRate * SoundLevelFormat.TemporalIntegrationDuration,,,, SoundLevelFormat.FrequencyWeighting, True)
                        End If

                        'Calculating needed gain
                        Dim NeededGain = TargetLevel - Standard_dBFS_To_dBSPL(CurrentLevel)

                        'Applying the same gain to all sounds in the group
                        For Each Member In GroupMembers
                            Audio.DSP.AmplifySection(Member.Sound, NeededGain, 1)
                        Next
                    Next

                    ' Applies fading to the sounds
                    For Each Item In SoundSceneItemList
                        If Item.FadeSpecifications IsNot Nothing Then
                            For Each FadeSpecification In Item.FadeSpecifications
                                Audio.DSP.Fade(Item.Sound, FadeSpecification, 1)
                            Next
                        End If
                    Next

                    'Applies ducking
                    For Each Item In SoundSceneItemList
                        If Item.DuckingSpecifications IsNot Nothing Then
                            For i = 0 To Item.DuckingSpecifications.Count - 1 Step 2

                                Dim FadeOutSpecs = Item.DuckingSpecifications(i)
                                Dim FadeInSpecs = Item.DuckingSpecifications(i + 1)
                                Dim MidStartSample As Integer = FadeOutSpecs.StartSample + FadeOutSpecs.SectionLength
                                Dim MidLength As Integer = FadeInSpecs.StartSample - MidStartSample
                                Dim MidFadeSpecs = New Audio.DSP.FadeSpecifications(FadeOutSpecs.EndAttenuation, FadeInSpecs.StartAttenuation,
                                                                                         MidStartSample, MidLength, FadeOutSpecs.SlopeType, FadeOutSpecs.CosinePower, FadeOutSpecs.EqualPower)

                                Audio.DSP.Fade(Item.Sound, FadeOutSpecs, 1)
                                Audio.DSP.Fade(Item.Sound, MidFadeSpecs, 1)
                                Audio.DSP.Fade(Item.Sound, FadeInSpecs, 1)

                            Next
                        End If
                    Next

                    If OutputRouting.Values.Max = 0 Then Throw New Exception("No output channels specified in the DuplexMixer output routing.")
                    Dim OutputSound As Sound = Nothing

                    'Inserting/adding sounds to the output sound
                    'OutputSound.
                    Select Case SoundPropagationType
                        Case SoundPropagationTypes.PointSpeakers

                            'TODO, perhaps SoundPropagationTypes.Headphones should be treated separately from sound field speakers?

                            'Getting the length of the complete mix (This must be done separately depending on the value of TransducerType, as FIR filterring changes the lengths of the sounds!)
                            OutputSound = GetEmptyOutputSound(SoundSceneItemList, WaveFormat)

                            'Adds the item sound into the single channel that is closest to the SourceLocation specified in the item
                            For Each Item In SoundSceneItemList

                                Dim ClosestHardwareOutput = FindClosestHardwareOutput(Item.SourceLocation)
                                Dim CorrespondingChannellInOutputSound As Integer? = OutputRouting(ClosestHardwareOutput)

                                'Inserts the sound into CorrespondingChannellInOutputSound
                                Audio.DSP.InsertSound(Item.Sound, 1, OutputSound, CorrespondingChannellInOutputSound, Item.InsertSample)

                            Next

                        Case SoundPropagationTypes.Ambisonics

                            Throw New NotImplementedException("Ambisonics presentation is not yet supported.")

                        Case SoundPropagationTypes.SimulatedSoundField

                            'Simulating the speaker locations into stereo headphones
                            SimulateSoundSourceLocation(DirectionalSimulator.SelectedDirectionalSimulationSetName, SoundSceneItemList)

                            'Getting the length of the complete mix (This must be done separately depending on the value of TransducerType, as FIR filterring changes the lengths of the sounds!)
                            OutputSound = GetEmptyOutputSound(SoundSceneItemList, WaveFormat)

                            For Each Item In SoundSceneItemList

                                'Inserts the sound into CorrespondingChannellInOutputSound
                                Audio.DSP.InsertSound(Item.Sound, 1, OutputSound, 1, Item.InsertSample)
                                Audio.DSP.InsertSound(Item.Sound, 2, OutputSound, 2, Item.InsertSample)

                            Next

                        Case Else
                            Throw New NotImplementedException("Unknown TransducerType")
                    End Select


                    ' TODO: Simulation of HL/HA


                    'Limiter
                    If LimiterThreshold.HasValue Then
                        'Limiting the total sound level
                        'Checking the sound levels only in channels with output sound
                        Dim ChannelsToCheck As New SortedSet(Of Integer)
                        For Each c In OutputRouting.Values
                            If Not ChannelsToCheck.Contains(c) Then ChannelsToCheck.Add(c)
                        Next

                        For Each c In ChannelsToCheck
                            Dim LimiterResult = Audio.DSP.SoftLimitSection(OutputSound, c, Standard_dBSPL_To_dBFS(LimiterThreshold),,,, FrequencyWeightings.Z, True)

                            If LimiterResult <> "" Then
                                'Limiting occurred, logging the limiter data
                                Utils.SendInfoToLog(
                            "channel " & c &
                            " had it's output level limited to  " & LimiterThreshold & " dB, " & DateTime.Now.ToString & vbCrLf &
                            "Section:" & vbTab & "Startattenuation" & vbTab & "EndAttenuation" & vbCrLf &
                            LimiterResult)
                            End If
                        Next
                    End If


                    'Exporting sound for manual evaluation
                    'Audio.AudioIOs.SaveToWaveFile(OutputSound, IO.Path.Combine(Utils.logFilePath, "Step6_PostLimiter"))

                    Return OutputSound

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function

            Private Function GetEmptyOutputSound(ByRef SoundSceneItemList As List(Of SoundSceneItem), ByRef WaveFormat As Audio.Formats.WaveFormat) As Sound

                'Getting the length of the complete mix (This must be done separately depending on the value of TransducerType, as FIR filterring changes the lengths of the sounds!)
                Dim MixLength As Integer = 0
                For Each Item In SoundSceneItemList
                    Dim CurrentNeededLength As Integer = Item.InsertSample + Item.Sound.WaveData.SampleData(1).Length
                    MixLength = Math.Max(MixLength, CurrentNeededLength)
                Next

                'Creating an OutputSound with the number of channels required by the current output routing Values.
                Dim OutputSound As New Sound(New Formats.WaveFormat(WaveFormat.SampleRate, WaveFormat.BitDepth, OutputRouting.Values.Max, , WaveFormat.Encoding))

                'Adding the needed channels arrays to the output sound (based on the values in OutputRouting (not adding arrays for in-between channels in which no sound data will exist)
                For Each Channel In OutputRouting.Values
                    Dim NewChannelArray(MixLength - 1) As Single
                    OutputSound.WaveData.SampleData(Channel) = NewChannelArray
                Next

                Return OutputSound

            End Function

            Private Function SetupSoundLevelGroups(ByRef SoundSceneItemList As List(Of SoundSceneItem)) As SortedList(Of Integer, Tuple(Of Double, Audio.Formats.SoundLevelFormat, List(Of SoundSceneItem)))

                'Checking the sound level groups, and addes them along with their sound level, sound level formats, and the respective SoundSceneItems, by which they can henceforth be retrieved based on the LevelGroup value of each SoundSceneItem.
                Dim SoundLevelGroups As New SortedList(Of Integer, Tuple(Of Double, Audio.Formats.SoundLevelFormat, List(Of SoundSceneItem)))
                For Each Item In SoundSceneItemList

                    If Not SoundLevelGroups.ContainsKey(Item.LevelGroup) Then
                        'Adding the sound level group, and the item
                        SoundLevelGroups.Add(Item.LevelGroup, New Tuple(Of Double, Formats.SoundLevelFormat, List(Of SoundSceneItem))(Item.SoundLevel, Item.SoundLevelFormat, New List(Of SoundSceneItem) From {Item}))
                    Else
                        'Checking that the SoundLevel and SoundLevelFormat values agree with previously added items
                        If Item.SoundLevel <> SoundLevelGroups(Item.LevelGroup).Item1 Then Throw New ArgumentException("Different sound levels within the same sound level group detected!")
                        If Item.SoundLevelFormat.IsEqual(SoundLevelGroups(Item.LevelGroup).Item2) = False Then Throw New ArgumentException("Different sound level formats within the same sound level group detected!")

                        'Adding the SoundSceneItem
                        SoundLevelGroups(Item.LevelGroup).Item3.Add(Item)
                    End If
                Next

                'Checking intra group consistence of certain values
                For Each Group In SoundLevelGroups
                    CheckGroupLevelConsistency(Group.Value.Item3)
                Next

                Return SoundLevelGroups

            End Function

            Private Sub CheckGroupLevelConsistency(ByRef GroupMembers As List(Of SoundSceneItem))


                If GroupMembers.Count > 1 Then
                    'Checking equality of LevelDefStartSample and LevelDefLength within the SoundLevelGroup
                    Dim NumberOfMembersWith_LevelDefStartSample As Integer = 0
                    Dim NumberOfMembersWith_LevelDefLength As Integer = 0

                    For i = 0 To GroupMembers.Count - 1
                        If GroupMembers(i).LevelDefStartSample.HasValue Then NumberOfMembersWith_LevelDefStartSample += 1
                        If GroupMembers(i).LevelDefLength.HasValue Then NumberOfMembersWith_LevelDefLength += 1
                    Next

                    If NumberOfMembersWith_LevelDefStartSample = GroupMembers.Count Then
                        'Checking that all have the same start value
                        For i = 0 To GroupMembers.Count - 2
                            If GroupMembers(i).LevelDefStartSample <> GroupMembers(i + 1).LevelDefStartSample Then
                                Throw New ArgumentException("All, or no, members in a LevelGroup must specify the same LevelDefStartSample value.")
                            End If
                        Next
                    ElseIf NumberOfMembersWith_LevelDefStartSample = 0 Then
                        'This is ok
                    Else
                        Throw New ArgumentException("All, or no, members in a LevelGroup must specify a LevelDefStartSample value.")
                    End If

                    If NumberOfMembersWith_LevelDefLength = GroupMembers.Count Then
                        'Checking that all have the same length value
                        For i = 0 To GroupMembers.Count - 2
                            If GroupMembers(i).LevelDefLength <> GroupMembers(i + 1).LevelDefLength Then
                                Throw New ArgumentException("All, or no, members in a LevelGroup must specify the same LevelDefStartSample value.")
                            End If
                        Next
                    ElseIf NumberOfMembersWith_LevelDefLength = 0 Then
                        'This is ok
                    Else
                        Throw New ArgumentException("All, or no, members in a LevelGroup must specify a LevelDefStartSample value.")
                    End If
                End If


            End Sub

            ''' <summary>
            ''' Determines based on the location of the available sound feild speakers which is the closest to the indicated SoundSourceLocation (As of now only azimuth is regarded, and values for elevation and distance ignored!). 
            ''' Rounding is made away from 0 (front) and 180 / -180 (back) degrees, towards the sides (-90 and 90 degrees).) 
            ''' </summary>
            ''' <param name="SoundSourceLocation"></param>
            ''' <returns>Returns a Tuple in which the first Item contains the selected output channel and the second item contains the azimuth of the loudspeaker connected to that channel.</returns>
            Public Function FindClosestHardwareOutput(ByVal SoundSourceLocation As SoundSourceLocation) As Integer

                'NB & TODO: this function does not work with loadspeaker Elevation and distance! Any values for these will be ignored!
                If SoundSourceLocation.ActualLocation Is Nothing Then SoundSourceLocation.ActualLocation = New SoundSourceLocation

                Dim Azimuth As Integer = SoundSourceLocation.HorizontalAzimuth

                'Unwraps the Azimuth into the range: -180 < Azimuth <= 180
                Dim UnwrappedAzimuth = Utils.UnwrapAngle(Azimuth)

                Dim DistanceList As New List(Of Tuple(Of Integer, SoundSourceLocation, Double)) ' Channel, SpeakerAzimuth, distance

                For Each kvp In HardwareOutputChannelSpeakerLocations

                    'Calculates and stores the absolute difference between the speaker azimuth and the target azimuth
                    DistanceList.Add(New Tuple(Of Integer, SoundSourceLocation, Double)(kvp.Key, kvp.Value, Math.Abs(UnwrappedAzimuth - kvp.Value.HorizontalAzimuth)))
                Next

                'Gets the minimum distance value
                Dim MinDistance = DistanceList.Min(Function(x) x.Item3)

                'Gets the Items with that (minimum distance) value
                Dim MinDistanceItems = DistanceList.FindAll(Function(x) x.Item3 = MinDistance)

                'Checks the number of items detected
                If MinDistanceItems.Count = 1 Then

                    'If there is only one speaker with the minimum distance, 
                    'Store its horizontal azimuth
                    SoundSourceLocation.ActualLocation.HorizontalAzimuth = MinDistanceItems(0).Item2.HorizontalAzimuth

                    'TODO: when distance and/or elevation is implemented, these need to be set as well
                    'SoundSourceLocation.ActualLocation.Distance = ...
                    'SoundSourceLocation.ActualLocation.Elevation = ...

                    'And returns its index
                    Return MinDistanceItems(0).Item1

                ElseIf MinDistanceItems.Count = 2 Then
                    'If two speakers are at the exact same azimuth distance , the one closest to -90 or 90 degrees is selected based on the side of the UnwrappedAzimuth (however right side takes precensence if the tagert azimuth is zero degrees and no speaker is located there).

                    If UnwrappedAzimuth < 0 Then
                        'Select the one closest to -90 degrees
                        If Math.Abs(MinDistanceItems(0).Item3 - (-90)) < Math.Abs(MinDistanceItems(1).Item3 - (-90)) Then
                            SoundSourceLocation.ActualLocation.HorizontalAzimuth = MinDistanceItems(0).Item2.HorizontalAzimuth

                            'TODO: when distance and/or elevation is implemented, these need to be set as well
                            'SoundSourceLocation.ActualLocation.Distance = ...
                            'SoundSourceLocation.ActualLocation.Elevation = ...

                            Return MinDistanceItems(0).Item1
                        Else
                            SoundSourceLocation.ActualLocation.HorizontalAzimuth = MinDistanceItems(1).Item2.HorizontalAzimuth

                            'TODO: when distance and/or elevation is implemented, these need to be set as well
                            'SoundSourceLocation.ActualLocation.Distance = ...
                            'SoundSourceLocation.ActualLocation.Elevation = ...

                            Return MinDistanceItems(1).Item1
                        End If

                    Else
                        'Select the one closest to 90 degrees
                        'N.B. The greater and smaller than signs are reversed here to get left/right symmetry 
                        If Math.Abs(MinDistanceItems(0).Item3 - 90) > Math.Abs(MinDistanceItems(1).Item3 - 90) Then
                            SoundSourceLocation.ActualLocation.HorizontalAzimuth = MinDistanceItems(1).Item2.HorizontalAzimuth

                            'TODO: when distance and/or elevation is implemented, these need to be set as well
                            'SoundSourceLocation.ActualLocation.Distance = ...
                            'SoundSourceLocation.ActualLocation.Elevation = ...

                            Return MinDistanceItems(1).Item1
                        Else
                            SoundSourceLocation.ActualLocation.HorizontalAzimuth = MinDistanceItems(0).Item2.HorizontalAzimuth

                            'TODO: when distance and/or elevation is implemented, these need to be set as well
                            'SoundSourceLocation.ActualLocation.Distance = ...
                            'SoundSourceLocation.ActualLocation.Elevation = ...

                            Return MinDistanceItems(0).Item1
                        End If
                    End If

                Else
                    'It should be impossible to have more than two speakers at the same azimuth distance
                    Throw New Exception("Oops something has gone wrong... find out what...")
                End If

            End Function

            ''' <summary>
            ''' Re-usable fft format for FIR-filtering
            ''' </summary>
            Private MyFftFormat As Audio.Formats.FftFormat = New Formats.FftFormat

            Private Sub SimulateSoundSourceLocation(ByVal ImpulseReponseSetName As String, ByRef SoundSceneItemList As List(Of SoundSceneItem))

                For Each SoundSceneItem In SoundSceneItemList

                    Try

                        'Copies the sound of the SoundSceneItem to a new two-channel sound for use with headphones
                        Dim NewSound As New Audio.Sound(New Audio.Formats.WaveFormat(
                                                SoundSceneItem.Sound.WaveFormat.SampleRate,
                                                SoundSceneItem.Sound.WaveFormat.BitDepth,
                                                2,, SoundSceneItem.Sound.WaveFormat.Encoding))

                        Dim OriginalSoundLength As Integer = SoundSceneItem.Sound.WaveData.SampleData(1).Length

                        Dim NewChannel1SampleArray(OriginalSoundLength - 1) As Single
                        NewSound.WaveData.SampleData(1) = NewChannel1SampleArray

                        Dim NewChannel2SampleArray(OriginalSoundLength - 1) As Single
                        NewSound.WaveData.SampleData(2) = NewChannel2SampleArray

                        Array.Copy(SoundSceneItem.Sound.WaveData.SampleData(1), NewSound.WaveData.SampleData(1), OriginalSoundLength)
                        Array.Copy(SoundSceneItem.Sound.WaveData.SampleData(1), NewSound.WaveData.SampleData(2), OriginalSoundLength)

                        'Attains a copy of the appropriate directional FIR-filter kernel
                        Dim SelectedSimulationPoint = DirectionalSimulator.GetStereoKernel(ImpulseReponseSetName, SoundSceneItem.SourceLocation.HorizontalAzimuth, SoundSceneItem.SourceLocation.Elevation, SoundSceneItem.SourceLocation.Distance)
                        Dim CurrentKernel = SelectedSimulationPoint.Item2.CreateSoundDataCopy
                        Dim SelectedActualPoint = SelectedSimulationPoint.Item1

                        'Storing the actual values available in the simulator
                        SoundSceneItem.SourceLocation.ActualLocation = New SoundSourceLocation
                        SoundSceneItem.SourceLocation.ActualLocation.HorizontalAzimuth = SelectedActualPoint.GetSphericalAzimuth
                        SoundSceneItem.SourceLocation.ActualLocation.Elevation = SelectedActualPoint.GetSphericalElevation
                        SoundSceneItem.SourceLocation.ActualLocation.Distance = SelectedActualPoint.GetSphericalDistance

                        'Applies gain to the kernel (this is more efficient than applying gain to the whole sound array)
                        'TODO. The following can be utilized to optimize the need for setting level by array looping, when using sound feild simulation
                        'If SoundSceneItem.NeededGain <> 0 Then
                        '    Audio.DSP.AmplifySection(CurrentKernel, SoundSceneItem.NeededGain)
                        'End If

                        'Applies FIR-filtering
                        Dim FilteredSound = Audio.DSP.FIRFilter(NewSound, CurrentKernel, MyFftFormat,,,,,, True)

                        'FilteredSound.WriteWaveFile("C:\SpeechTestFrameworkLog\Test1.wav")

                        'Replacing the original sound
                        SoundSceneItem.Sound = FilteredSound

                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try

                Next

            End Sub



        End Class

    End Namespace

End Namespace
