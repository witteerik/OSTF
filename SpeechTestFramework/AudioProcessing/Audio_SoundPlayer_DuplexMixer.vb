Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows.Forms

Namespace Audio

    Namespace PortAudioVB

        Public Class DuplexMixer

            Private TransducerSpecification As TransducerSpecification

            ''' <summary>
            ''' A list of key-value pairs, where the key repressents the hardware output channel and the value repressents the wave file channel from which the output sound should be drawn.
            ''' </summary>
            Public OutputRouting As New SortedList(Of Integer, Integer)
            ''' <summary>
            ''' A list of key-value pairs, where the key repressents the hardware input channel and the value repressents the wave file channel in which the input sound should be stored.
            ''' </summary>
            Public InputRouting As New SortedList(Of Integer, Integer)

            Public ReadOnly AvailableOutputChannels As Integer
            Public ReadOnly AvailableInputChannels As Integer

            ''' <summary>
            ''' A list of key-value pairs, where the key repressents the hardware output channel and the values repressents the physical location of the loadspeaker connected to that output channel.
            ''' </summary>
            Public HardwareOutputChannelSpeakerLocations As New SortedList(Of Integer, SoundSourceLocation)

            Public TransducerType As TransducerTypes = TransducerTypes.SoundField

            Public Enum TransducerTypes
                SoundField
                SimulatedSoundField
                Headphones
                Ambisonics
            End Enum

            ''' <summary>
            ''' Creating a new mixer.
            ''' </summary>
            ''' <param name="AvailableOutputChannels"></param>
            ''' <param name="AvailableInputChannels"></param>
            Public Sub New(ByVal AvailableOutputChannels As Integer, ByVal AvailableInputChannels As Integer, Optional ByVal TransducerSpecification As TransducerSpecification = Nothing)

                Me.AvailableOutputChannels = AvailableOutputChannels
                Me.AvailableInputChannels = AvailableInputChannels

                'Selects the first available transducer
                If TransducerSpecification Is Nothing Then TransducerSpecification = OstfSettings.AvaliableTransducers(0)
                Me.TransducerSpecification = TransducerSpecification

                For c = 1 To AvailableOutputChannels
                    OutputRouting.Add(c, 0)
                Next

                For c = 1 To AvailableInputChannels
                    InputRouting.Add(c, 0)
                Next

            End Sub

            Public Sub DirectMonoSoundToOutputChannel(ByRef TargetOutputChannel As Integer)
                If OutputRouting.ContainsKey(TargetOutputChannel) Then OutputRouting(TargetOutputChannel) = 1
            End Sub

            Public Sub SetLinearInput()
                For c = 1 To AvailableInputChannels
                    InputRouting(c) = c
                Next
            End Sub
            Public Sub SetLinearOutput()
                For c = 1 To AvailableOutputChannels
                    OutputRouting(c) = c
                Next
            End Sub

            Public Sub DirectMonoSoundToOutputChannels(ByRef TargetOutputChannels() As Integer)
                For Each OutputChannel In TargetOutputChannels
                    If OutputRouting.ContainsKey(OutputChannel) Then OutputRouting(OutputChannel) = 1
                Next
            End Sub

#Region "Calibration"

            ''' <summary>
            ''' Holds the simulated sound field output level of a 1 kHz sine wave at an (hypothetical) RMS level of 0 dBFS. 
            ''' </summary>
            Public Const Simulated_dBFS_dBSPL_Difference As Double = 100

            Private _Calibration_FsToSpl As New SortedList(Of Integer, Double)

            ''' <summary>
            ''' Returns the calibration (FsToSpl) value for the indicated loudspeaker.
            ''' </summary>
            ''' <param name="Channel"></param>
            ''' <returns></returns>
            Public ReadOnly Property Calibration_FsToSpl(ByVal Channel As Integer) As Double
                Get
                    If _Calibration_FsToSpl.ContainsKey(Channel) Then
                        Return _Calibration_FsToSpl(Channel)
                    Else
                        MsgBox("Calibration has not been set for output channel " & Channel & vbCrLf & vbCrLf &
                       "Click OK to use the default FsToSpl value of " & Simulated_dBFS_dBSPL_Difference & " dB!", MsgBoxStyle.Exclamation, "Warning!")
                        Return Simulated_dBFS_dBSPL_Difference
                    End If
                End Get
            End Property


            Public Function GetCalibrationGain(ByVal Channel As Integer)

                'Calculates the calibration gain
                Dim CalibrationGain As Double = Simulated_dBFS_dBSPL_Difference - Calibration_FsToSpl(Channel)

                'Returns the calibration gain
                Return CalibrationGain

            End Function

            ''' <summary>
            ''' Call this sub to set the loudspeaker or headphone calibration of the current instance of DuplexMixer.
            ''' </summary>
            ''' <param name="Calibration_FsToSpl"></param>
            Public Sub SetCalibrationValues(ByVal Calibration_FsToSpl As SortedList(Of Integer, Double))

                'Setting the private field _Calibration_FsToSpl. Values are retrieved by the public Readonly Property Calibration_FsToSpl 
                Me._Calibration_FsToSpl = Calibration_FsToSpl

            End Sub

            ''' <summary>
            ''' Converts the sound pressure level given by InputSPL to a value in dB FS using the conversion value given by Simulated_dBFS_dBSPL_Difference
            ''' </summary>
            ''' <param name="InputSPL"></param>
            ''' <returns></returns>
            Public Shared Function Simulated_dBSPL_To_dBFS(ByVal InputSPL As Double) As Double
                Return InputSPL - Simulated_dBFS_dBSPL_Difference
            End Function

            ''' <summary>
            ''' Converts the full scale sound level given by InputFS to a sound pressure level value using the conversion value given by Simulated_dBFS_dBSPL_Difference
            ''' </summary>
            ''' <param name="InputFS"></param>
            ''' <returns></returns>
            Public Shared Function Simulated_dBFS_To_dBSPL(ByVal InputFS As Double) As Double
                Return Simulated_dBFS_dBSPL_Difference + InputFS
            End Function

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
                Public Distance As Double

                ''' <summary>
                ''' The horizontal azimuth of the sound source in degrees, in relation to front (zero degrees), positive values to the right and negative values to the left.
                ''' </summary>
                Public HorizontalAzimuth As Double

                ''' <summary>
                ''' The verical elevation of the sound source in degrees, where zero degrees refers to the horizontal plane, positive values upwards and negative values downwards.
                ''' </summary>
                Public Elevation As Double

            End Class


            Public Function CreateSoundScene(ByRef Input As List(Of SoundSceneItem), Optional ByVal LimiterThreshold As Double? = 100) As Audio.Sound

                Dim WaveFormat As Audio.Formats.WaveFormat = Nothing

                'Copy sounds so that their individual sample data in the selected channel to a new (mono) sound so that the sample data may be changed without changing the original sound,
                ' and addes them in a new list of SoundSceneItem, which should henceforth be used instead of the Input object.
                Dim SoundSceneItemList As New List(Of SoundSceneItem)
                For Each Item In Input
                    'Creates a new NewSoundSceneItem 
                    Dim NewSoundSceneItem = New SoundSceneItem(Item.Sound.CopyChannelToMonoSound(Item.ReadChannel), 1, Item.SoundLevel, Item.LevelGroup,
                                                                Item.SourceLocation, Item.InsertSample, Item.LevelDefStartSample, Item.LevelDefLength,
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
                    Dim NeededGain = TargetLevel - Simulated_dBFS_To_dBSPL(CurrentLevel)

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
                Select Case TransducerType
                    Case TransducerTypes.SoundField

                        'Getting the length of the complete mix (This must be done separately depending on the value of TransducerType, as FIR filterring changes the lengths of the sounds!)
                        OutputSound = GetEmptyOutputSound(SoundSceneItemList, WaveFormat)

                        'Adds the item sound into the single channel that is closest to the SourceLocation specified in the item
                        For Each Item In SoundSceneItemList

                            Dim ClosestHardwareOutput = FindClosestHardwareOutput(Item.SourceLocation)
                            Dim CorrespondingChannellInOutputSound As Integer? = OutputRouting(ClosestHardwareOutput)

                            'Inserts the sound into CorrespondingChannellInOutputSound
                            Audio.DSP.InsertSound(Item.Sound, 1, OutputSound, CorrespondingChannellInOutputSound, Item.InsertSample)

                        Next

                    Case TransducerTypes.Ambisonics

                        Throw New NotImplementedException("Ambisonics presentation is not yet supported.")

                    Case TransducerTypes.SimulatedSoundField

                        'Simulating the speaker locations into stereo headphones
                        SimulateSoundSourceLocation(SoundSceneItemList)

                        'Getting the length of the complete mix (This must be done separately depending on the value of TransducerType, as FIR filterring changes the lengths of the sounds!)
                        OutputSound = GetEmptyOutputSound(SoundSceneItemList, WaveFormat)

                        For Each Item In SoundSceneItemList

                            'Inserts the sound into CorrespondingChannellInOutputSound
                            Audio.DSP.InsertSound(Item.Sound, 1, OutputSound, 1, Item.InsertSample)
                            Audio.DSP.InsertSound(Item.Sound, 2, OutputSound, 2, Item.InsertSample)

                        Next

                    Case TransducerTypes.Ambisonics
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
                        Dim LimiterResult = Audio.DSP.SoftLimitSection(OutputSound, c, Simulated_dBSPL_To_dBFS(LimiterThreshold),,,, FrequencyWeightings.Z, True)

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

                ' Calibrate

                MsgBox("Fix calibration")


                Return OutputSound


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

                Dim Azimuth As Integer = SoundSourceLocation.HorizontalAzimuth

                'Unwraps the Azimuth into the range: -180 < Azimuth <= 180
                Dim UnwrappedAzimuth = UnwrapAngle(Azimuth)

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

                    'If there is only one speaker with the minimum distance, its values are returned
                    Return MinDistanceItems(0).Item1

                ElseIf MinDistanceItems.Count = 2 Then
                    'If two speakers are at the exact same azimuth distance , the one closest to -90 or 90 degrees is selected based on the side of the UnwrappedAzimuth (however right side takes precensence if the tagert azimuth is zero degrees and no speaker is located there).

                    If UnwrappedAzimuth < 0 Then
                        'Select the one closest to -90 degrees
                        If Math.Abs(MinDistanceItems(0).Item3 - (-90)) < Math.Abs(MinDistanceItems(1).Item3 - (-90)) Then
                            Return MinDistanceItems(0).Item1
                        Else
                            Return MinDistanceItems(1).Item1
                        End If

                    Else
                        'Select the one closest to 90 degrees
                        'N.B. The greater and smaller than signs are reversed here to get left/right symmetry 
                        If Math.Abs(MinDistanceItems(0).Item3 - 90) > Math.Abs(MinDistanceItems(1).Item3 - 90) Then
                            Return MinDistanceItems(1).Item1
                        Else
                            Return MinDistanceItems(0).Item1
                        End If
                    End If

                Else
                    'It should be impossible to have more than two speakers at the same azimuth distance
                    Throw New Exception("Oops something has gone wrong... find out what...")
                End If

            End Function

            ''' <summary>
            ''' Unwraps the indicated angle into the range -180 (is lower than) Azimuth (which is equal to or lower than) 180 degrees.
            ''' </summary>
            ''' <param name="Angle">The angle in degrees</param>
            ''' <returns></returns>
            Private Shared Function UnwrapAngle(ByVal Angle As Integer) As Integer

                'Gets the remainder when dividing by 360
                Dim UnwrappedAngle As Integer
                Dim Div = Math.DivRem(Angle, 360, UnwrappedAngle)

                'Sets the Azimuth in the following range: -180 < Azimuth <= 180
                If UnwrappedAngle > 180 Then UnwrappedAngle -= 360

                Return UnwrappedAngle
            End Function


            ''' <summary>
            ''' Re-usable fft format for FIR-filtering
            ''' </summary>
            Private MyFftFormat As Audio.Formats.FftFormat = New Formats.FftFormat

            Public DirectionalSimulator As DirectionalSimulation = Nothing

            Private _TransducerName As TransducerNames

            Public ReadOnly Property TransducerName As TransducerNames
                Get
                    Return _TransducerName
                End Get
            End Property

            Public Enum TransducerNames
                Unspecified
                AKGK601
                AKGK271MKII
                SennheiserHD25_1
            End Enum

            Private _SpeakerDistance As Double
            Public ReadOnly Property SpeakerDistance As Double
                Get
                    Return _SpeakerDistance
                End Get
            End Property

            Public SupportedSpeakerDistances As New SortedList(Of String, List(Of Double)) From {{"wierstorf2011", New List(Of Double) From {0.5, 1.0R, 2.0R, 3.0R}}}

            Public Sub SetupDirectionalSimulator(ByVal TransducerName As TransducerNames, ByVal SpeakerDistance As Double, ByVal WaveFormat As Audio.Formats.WaveFormat)

                Me._TransducerName = TransducerName
                Me._SpeakerDistance = SpeakerDistance

                If Not SupportedSpeakerDistances("wierstorf2011").Contains(SpeakerDistance) Then Throw New NotImplementedException("The selected speaker distance (" & SpeakerDistance & " meters) is not available for sound field simulation.")

                Select Case WaveFormat.BitDepth
                    Case 32
                        'Ok!
                    Case Else
                        Throw New NotImplementedException("Directional simulation is unfortunately not supported for audio file bit dephts of " & WaveFormat.BitDepth)
                End Select

                'Getting the folder
                Dim CurrentIrDatabasePath As String
                Select Case WaveFormat.SampleRate
                    Case 44100
                        CurrentIrDatabasePath = IO.Path.Combine(OstfSettings.RootDirectory, OstfSettings.RoomImpulsesSubDirectory, "wierstorf2011\44100Hz")
                    Case 48000
                        CurrentIrDatabasePath = IO.Path.Combine(OstfSettings.RootDirectory, OstfSettings.RoomImpulsesSubDirectory, "wierstorf2011\48000Hz")
                    Case Else
                        Throw New NotImplementedException("Directional simulation is unfortunately not supported for the samplerate " & WaveFormat.SampleRate)
                End Select

                'Creating a file name
                Dim HeadphoneTypeString As String = ""
                Select Case TransducerName
                    Case TransducerNames.Unspecified
                        HeadphoneTypeString = ""
                    Case TransducerNames.AKGK271MKII
                        HeadphoneTypeString = "AKGK271_"
                    Case TransducerNames.AKGK601
                        HeadphoneTypeString = "AKGK601_"
                    Case TransducerNames.SennheiserHD25_1
                        HeadphoneTypeString = "SennheiserHD25_"
                End Select

                Dim CurrentIrFileName As String = "QU_KEMAR_anechoic_" & HeadphoneTypeString & SpeakerDistance.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) & "m.wav"

                DirectionalSimulator = New DirectionalSimulation(IO.Path.Combine(CurrentIrDatabasePath, CurrentIrFileName))

            End Sub

            Private Sub SimulateSoundSourceLocation(ByRef SoundSceneItemList As List(Of SoundSceneItem))

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
                        Dim CurrentKernel = DirectionalSimulator.GetStereoKernel(SoundSceneItem.SourceLocation.HorizontalAzimuth).CreateSoundDataCopy

                        'Applies gain to the kernel (this is more efficient than applying gain to the whole sound array)
                        'TODO. The following can be utilized to optimize the need for setting level by array looping, when using sound feild simulation
                        'If SoundSceneItem.NeededGain <> 0 Then
                        '    Audio.DSP.AmplifySection(CurrentKernel, SoundSceneItem.NeededGain)
                        'End If

                        'Applies FIR-filtering
                        Dim FilteredSound = Audio.DSP.FIRFilter(NewSound, CurrentKernel, MyFftFormat)

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
