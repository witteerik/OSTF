Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows.Forms

Namespace Audio

    Namespace PortAudioVB

        Public Class DuplexMixer
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

            ''' <summary>
            ''' Creating a new mixer.
            ''' </summary>
            ''' <param name="AvailableOutputChannels"></param>
            ''' <param name="AvailableInputChannels"></param>
            Public Sub New(ByVal AvailableOutputChannels As Integer, ByVal AvailableInputChannels As Integer)

                Me.AvailableOutputChannels = AvailableOutputChannels
                Me.AvailableInputChannels = AvailableInputChannels

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
                Public PlayLength As Integer


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
                               ByVal InsertSample As Integer, ByVal PlayLength As Integer,
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
                    Me.PlayLength = PlayLength

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


            Public Function CreateSoundScene(ByRef Input As List(Of SoundSceneItem)) As Audio.Sound

                Dim WaveFormat As Audio.Formats.WaveFormat = Nothing

                'Checking the sound level groups, and addes them along with their sound level, sound level formats, and the respective SoundSceneItems, by which they can henceforth be retrieved based on the LevelGroup value of each SoundSceneItem.
                Dim SoundLevelGroups As New SortedList(Of Integer, Tuple(Of Double, Audio.Formats.SoundLevelFormat, List(Of SoundSceneItem)))
                For Each Item In Input
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

                'Copy sounds so that their individual sample data in the selected channel to a new (mono) sound so that the sample data may be changed without changing the original sound,
                ' and addes them in a new list of SoundSceneItem, which should henceforth be used instead of the Input object.
                Dim SoundSceneItemList As New List(Of SoundSceneItem)
                For Each Item In Input
                    'Creates a new NewSoundSceneItem 
                    Dim NewSoundSceneItem = New SoundSceneItem(Item.Sound.CopyChannelToMonoSound(Item.ReadChannel), 1, Item.SoundLevel, Item.LevelGroup,
                                                                Item.SourceLocation, Item.InsertSample, Item.PlayLength, Item.LevelDefStartSample, Item.LevelDefLength,
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

                'Getting the length of the complete mix
                Dim MixLength As Integer = 0
                For Each Item In SoundSceneItemList
                    Dim CurrentNeededLength As Integer = Item.InsertSample + Item.PlayLength
                    MixLength = Math.Max(MixLength, CurrentNeededLength)
                Next



                ' Applies fading to the sounds
                For Each Item In SoundSceneItemList
                    For Each FadeSpecification In Item.FadeSpecifications
                        Audio.DSP.Fade(Item.Sound, FadeSpecification, 1)
                    Next
                Next

                ' Setting levels
                For Each SoundLevelGroup In SoundLevelGroups

                    Dim TargetLevel = SoundLevelGroup.Value.Item1
                    Dim SoundLevelFormat = SoundLevelGroup.Value.Item2
                    Dim GroupMembers = SoundLevelGroup.Value.Item3
                    Dim Local_LevelDefStartSample As Integer? = Nothing
                    Dim Local_LevelDefLength As Integer? = Nothing

                    If GroupMembers.Count > 1 Then
                        'Checking equality of Local_LevelDefStartSample and Local_LevelDefLength within the SoundLevelGroup
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

                        'Measures the sound levels
                        If GroupMemberMeasurementSounds.Count = 1 Then

                            'Only one sound, simply set it to the correct level
                            Dim CurrentLevel As Double
                            If SoundLevelFormat.LoudestSectionMeasurement = False Then
                                CurrentLevel = Audio.DSP.MeasureSectionLevel(GroupMemberMeasurementSounds(0), 1,,,,, SoundLevelFormat.FrequencyWeighting)
                            Else
                                CurrentLevel = Audio.DSP.GetLevelOfLoudestWindow(GroupMemberMeasurementSounds(0), 1, WaveFormat.SampleRate * SoundLevelFormat.TemporalIntegrationDuration,,,, SoundLevelFormat.FrequencyWeighting, True)
                            End If

                        'Calculating needed gain
                        Dim NeededGain = TargetLevel - Simulated_dBFS_To_dBSPL(CurrentLevel)

                        'Applying gain
                        Audio.DSP.AmplifySection(GroupMembers(0).Sound, NeededGain, 1)


                    ElseIf GroupMemberMeasurementSounds.Count > 1 Then

                        ' Equalizing the sound levels in all sounds before mixing them

                        '(TODO: Here room impluse responses could be utilized to better estimate the combined sound level!)

                        'Mixing the sounds

                        'Measuring their levels

                        'Adjusting their levels to reach the desired combined sound level


                        'Calculating the sound level of all masker channels added
                        Dim AddedLevels_FS As Double = Audio.DSP.MeasureSectionLevel_AddedChannels(TempSound, MixerSettings.BackgroundOutputChannels.ToList,
                                                                                                    StartMeasureSample, MeasurementLength,,, MeasurementModeFrequencyWeighting)


                        Else
                            Throw New ArgumentException("Missing sound in SoundSceneItem.")
                        End If


                    Next

                    'Applies ducking
                    For Each Item In SoundSceneItemList
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
                Next


                'Creating an OutputSound with the number of channels required by the current output routing Values.
                If OutputRouting.Values.Max = 0 Then Throw New Exception("No output channels specified in the DuplexMixer output routing.")
                Dim OutputSound As New Sound(New Formats.WaveFormat(WaveFormat.SampleRate, WaveFormat.BitDepth, OutputRouting.Values.Max, , WaveFormat.Encoding))
                'Adding the needed channels arrays to the output sound (based on the values in OutputRouting (not adding arrays for in-between channels in which no sound data will exist)
                For Each Channel In OutputRouting.Values
                    Dim NewChannelArray(MixLength - 1) As Single
                    OutputSound.WaveData.SampleData(Channel) = NewChannelArray
                Next

                'Inserting/adding sounds to the output sound



                ' Simulation of HL/HA

                ' Mix
                '       Select sound field or headphones
                '             + Calibrate

                'Limiter

                Return OutputSound


            End Function


        End Class

    End Namespace

End Namespace
