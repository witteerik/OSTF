Imports STFN.Core.Audio


Public Class AudioSystemSpecification
    Public Property Name As String = "Default"
    Public ReadOnly Property MediaPlayerType As MediaPlayerTypes

    Public Enum MediaPlayerTypes
        ''' <summary>
        ''' A sound player MediaPlayerType using the Port Audio library
        ''' </summary>
        PaBased
        ''' <summary>
        ''' A sound player MediaPlayerType using Android AudioTrack feature
        ''' </summary>
        AudioTrackBased
        ''' <summary>
        ''' The default media player MediaPlayerType for the specified platform as defined by OSTF.
        ''' </summary>
        [Default]
    End Enum


    Public ReadOnly Property ParentAudioApiSettings As Audio.AudioSettings
    Public Property Mixer As STFN.Core.Audio.SoundScene.DuplexMixer
    Public Property LoudspeakerAzimuths As New List(Of Double) From {-90, 90}
    Public Property LoudspeakerElevations As New List(Of Double) From {0, 0}
    Public Property LoudspeakerDistances As New List(Of Double) From {0, 0}
    Public Property HardwareOutputChannels As New List(Of Integer) From {1, 2}
    Public Property CalibrationGain As New List(Of Double) From {0, 0}

    Protected _HostVolumeOutputLevel As Integer? = Nothing

    ''' <summary>
    ''' Holds calibration gain for a pure tone audiometry transducer (frequency, gain)
    ''' </summary>
    ''' <returns></returns>
    Public Property PtaCalibrationGain As New SortedList(Of Integer, Double)

    Public Property PtaCalibrationGainFrequencies As New List(Of Integer)
    Public Property PtaCalibrationGainValues As New List(Of Double)
    Public Property RETSPL_Speech As Double = 0



    ''' <summary>
    ''' If supported by the sound player used, this value is used to set and maintain the volume of the selected output sound unit (e.g. sound card) while the player is active. Value represent percentages (0-100).
    ''' </summary>
    ''' <returns></returns>
    Public Property HostVolumeOutputLevel As Integer?
        Get
            Return _HostVolumeOutputLevel
        End Get
        Set(value As Integer?)
            If value.HasValue Then
                _HostVolumeOutputLevel = Math.Clamp(value.Value, 0, 100)
            Else
                _HostVolumeOutputLevel = Nothing
            End If
        End Set
    End Property
    Public Property LimiterThreshold As Double? = Nothing

    Private _CanPlay As Boolean = False
    Public ReadOnly Property CanPlay As Boolean
        Get
            Return _CanPlay
        End Get
    End Property

    Private _CanRecord As Boolean = False
    Public ReadOnly Property CanRecord As Boolean
        Get
            Return _CanRecord
        End Get
    End Property

    Public Sub New(ByVal MediaPlayerType As MediaPlayerTypes, Optional ByRef ParentAudioApiSettings As Audio.AudioSettings = Nothing)

        Me.MediaPlayerType = MediaPlayerType

        Select Case MediaPlayerType
            Case MediaPlayerTypes.PaBased, MediaPlayerTypes.AudioTrackBased
                If ParentAudioApiSettings Is Nothing Then
                    Throw New ArgumentException("The argument ParentAudioApiSettings cannot be Nothing when the media player MediaPlayerType is PaBased or AudioTrackBased!")
                Else
                    Me.ParentAudioApiSettings = ParentAudioApiSettings
                End If
            Case MediaPlayerTypes.Default
                Throw New ArgumentException("The argument ParentAudioApiSettings cannot be Default when initiating an AudioSystemSpecification!")
        End Select

    End Sub

    Public Sub SetupMixer()
        'Setting up the mixer
        Mixer = New Audio.SoundScene.DuplexMixer(Me)

        CheckCanPlayRecord()

    End Sub
    Public Sub CheckCanPlayRecord()

        'Checks if the transducerss will play/record
        If Me.Mixer.OutputRouting.Keys.Count > 0 And Me.ParentAudioApiSettings.NumberOfOutputChannels.HasValue = True Then
            If Me.Mixer.OutputRouting.Keys.Max <= Me.ParentAudioApiSettings.NumberOfOutputChannels Then
                Me._CanPlay = True
            End If
        End If
        If Me.Mixer.InputRouting.Keys.Count > 0 And Me.ParentAudioApiSettings.NumberOfInputChannels.HasValue = True Then
            If Me.Mixer.InputRouting.Keys.Max <= Me.ParentAudioApiSettings.NumberOfInputChannels Then
                Me._CanRecord = True
            End If
        End If

    End Sub

    Public Sub OverrideCanPlay(ByVal Value As Boolean)
        _CanPlay = Value
    End Sub

    Public Function NumberOfApiOutputChannels() As Integer

        If Me.ParentAudioApiSettings.NumberOfOutputChannels.HasValue = True Then
            Return Me.ParentAudioApiSettings.NumberOfOutputChannels
        Else
            Return 0
        End If

    End Function

    Public Function NumberOfApiInputChannels() As Integer

        If Me.ParentAudioApiSettings.NumberOfInputChannels.HasValue = True Then
            Return Me.ParentAudioApiSettings.NumberOfInputChannels
        Else
            Return 0
        End If

    End Function

    ''' <summary>
    ''' Checks to see if the transducers connected to HardWareChannelLeft and HardWareChannelRight are headphones, assuming that the first two specified channels are the headphone channels.
    ''' </summary>
    ''' <param name="HardWareChannelLeft"></param>
    ''' <param name="HardWareChannelRight"></param>
    ''' <returns></returns>
    Public Function IsHeadphones() As Boolean

        'Returns false if there is only one output channel
        If HardwareOutputChannels.Count < 2 Then Return False

        'Tries to determine which is left and which is right
        Dim LeftChannelIndex As Integer
        Dim RightChannelIndex As Integer
        If LoudspeakerAzimuths(0) < 0 And LoudspeakerAzimuths(1) > 0 Then
            LeftChannelIndex = 0
            RightChannelIndex = 1
        ElseIf LoudspeakerAzimuths(0) > 0 And LoudspeakerAzimuths(1) < 0 Then
            LeftChannelIndex = 1
            RightChannelIndex = 0
        Else
            'The channel azimuths does not seem to be stereo. Returning False
            Return False
        End If

        'Requres azimuths to be -90 and 90
        If LoudspeakerAzimuths(LeftChannelIndex) <> -90 Then Return False
        If LoudspeakerAzimuths(RightChannelIndex) <> 90 Then Return False

        'Requires distance to be 0
        If LoudspeakerDistances(LeftChannelIndex) <> 0 Then Return False
        If LoudspeakerDistances(RightChannelIndex) <> 0 Then Return False

        'Requires elevation to be 0
        If LoudspeakerElevations(LeftChannelIndex) <> 0 Then Return False
        If LoudspeakerElevations(RightChannelIndex) <> 0 Then Return False

        'All checks passed, it is headphones
        Return True

    End Function

    Public Function GetVisualSoundSourceLocations() As List(Of Audio.SoundScene.VisualSoundSourceLocation)

        'Adding the appropriate sound sources into the selection views, based on the selected transducer
        Dim SignalLocationCandidateList As New List(Of Audio.SoundScene.VisualSoundSourceLocation)

        'Adding real speaker locations as selectable sound source candidates
        For s = 0 To LoudspeakerAzimuths.Count - 1
            SignalLocationCandidateList.Add(New Audio.SoundScene.VisualSoundSourceLocation(New Audio.SoundScene.SoundSourceLocation With {
                .HorizontalAzimuth = LoudspeakerAzimuths(s),
                .Elevation = LoudspeakerElevations(s),
                .Distance = LoudspeakerDistances(s)}))
        Next

        Return SignalLocationCandidateList

    End Function

    Public Function GetSoundSourceLocations() As List(Of Audio.SoundScene.SoundSourceLocation)

        'Adding the appropriate sound sources into the selection views, based on the selected transducer
        Dim SignalLocationCandidateList As New List(Of Audio.SoundScene.SoundSourceLocation)

        'Adding real speaker locations as selectable sound source candidates
        For s = 0 To LoudspeakerAzimuths.Count - 1
            SignalLocationCandidateList.Add(New Audio.SoundScene.SoundSourceLocation With {
                .HorizontalAzimuth = LoudspeakerAzimuths(s),
                .Elevation = LoudspeakerElevations(s),
                .Distance = LoudspeakerDistances(s)})
        Next

        Return SignalLocationCandidateList

    End Function


    Public Overrides Function ToString() As String
        If Name <> "" Then
            Return Name
        Else
            Return MyBase.ToString()
        End If
    End Function

    Public Function GetDescriptionString() As String
        Dim OutputList As New List(Of String)

        OutputList.Add("Name: " & Name)
        OutputList.Add("Loudspeaker azimuths: " & String.Join(", ", LoudspeakerAzimuths))
        OutputList.Add("Hardware output channels: " & String.Join(", ", HardwareOutputChannels))
        OutputList.Add("CalibrationGain: " & String.Join(", ", CalibrationGain))
        If LimiterThreshold.HasValue Then
            OutputList.Add("Limiter threshold: " & LimiterThreshold.ToString)
        Else
            OutputList.Add("Limiter threshold: (none)")
        End If
        'OutputList.Add(vbCrLf & "Sound device info: " & vbCrLf & ParentAudioApiSettings.ToShorterString)

        Return String.Join(vbCrLf, OutputList)
    End Function

End Class

