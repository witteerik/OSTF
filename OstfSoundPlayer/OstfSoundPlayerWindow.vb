Imports SpeechTestFramework

Public Class OstfSoundPlayerWindow

    Private SelectedTransducer As AudioSystemSpecification = Nothing

    Private AvaliableLevels As New List(Of Integer)
    Private ColumnStyleList As New List(Of ColumnStyle)
    Private AvaliableDurations As New List(Of String) From {"10 seconds", "30 seconds", "1 minute", "2 minutes", "5 minutes", "10 minutes"}
    Private AvaliableDurationsCorrespondingSeconds As New List(Of Integer) From {10, 30, 60, 2 * 60, 5 * 60, 10 * 60}
    Private SelectedDurationSeconds As Integer = 10
    Private SoundPropagationType As SoundPropagationTypes = SoundPropagationTypes.PointSpeakers
    Private MySoundLevelFormat = New Audio.Formats.SoundLevelFormat(Audio.BasicAudioEnums.SoundMeasurementTypes.LoudestSection_Z_Weighted, 0.1)

    Private Rnd As New Random

    Public StopTimer_SpinLock As New Threading.SpinLock

    Private Sub OstfSoundPlayerWindow_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Initializing the OSTF 
        SpeechTestFramework.InitializeOSTF()

        SpeechTestFramework.UseOptimizationLibraries = True

        For level As Integer = 0 To 80 Step 2
            AvaliableLevels.Add(level)
        Next

        Dim LocalAvailableTransducers = OstfBase.AvaliableTransducers
        If LocalAvailableTransducers.Count = 0 Then
            MsgBox("Unable to start the application since no sound transducers could be found!", MsgBoxStyle.Critical, "OSTF Sound Player")
            Close()
            Exit Sub
        End If

        'Adding transducers to the combobox, and selects the first one
        For Each Transducer In LocalAvailableTransducers
            Transducer_ComboBox.Items.Add(Transducer)
        Next
        'If Transducer_ComboBox.Items.Count > 0 Then
        '    Transducer_ComboBox.SelectedIndex = 0
        'End If

        'Adding column styles for the sound source control
        ColumnStyleList.Add(New ColumnStyle(SizeType.Absolute, 340))
        ColumnStyleList.Add(New ColumnStyle(SizeType.Absolute, 80))
        ColumnStyleList.Add(New ColumnStyle(SizeType.Percent, 100))
        ColumnStyleList.Add(New ColumnStyle(SizeType.Absolute, 80))
        ColumnStyleList.Add(New ColumnStyle(SizeType.Absolute, 80))
        ColumnStyleList.Add(New ColumnStyle(SizeType.Absolute, 80))

        For Each DurationString In AvaliableDurations
            Duration_ComboBox.Items.Add(DurationString)
        Next
        If Duration_ComboBox.Items.Count > 0 Then
            Duration_ComboBox.SelectedIndex = 0
        End If

        'Setting backcolors
        Dim TopColorValue As Single = 205
        Toplevel_TableLayoutPanel.BackColor = Drawing.Color.FromArgb(40, CSng(Rnd.Next(10, TopColorValue)), CSng(Rnd.Next(10, TopColorValue)), CSng(Rnd.Next(10, TopColorValue)))
        'SoundSource_FlowLayoutPanel.BackColor = Drawing.Color.FromArgb(CSng(Rnd.Next(10, TopColorValue)), CSng(Rnd.Next(10, TopColorValue)), CSng(Rnd.Next(10, TopColorValue)))
        SoundSource_FlowLayoutPanel.BackColor = Color.WhiteSmoke

    End Sub


    Private Sub Transducer_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Transducer_ComboBox.SelectedIndexChanged

        CurrentPlaySound = Nothing
        SoundSource_FlowLayoutPanel.Controls.Clear()

        SelectedTransducer = Transducer_ComboBox.SelectedItem

        If SelectedTransducer.CanPlay = True Then
            '(At this stage the sound player will be started, if not already done.)
            OstfBase.SoundPlayer.ChangePlayerSettings(SelectedTransducer.ParentAudioApiSettings,,,, 0.4, SelectedTransducer.Mixer, Audio.SoundPlayers.iSoundPlayer.SoundDirections.PlaybackOnly, True, True, True)
        Else
            MsgBox("Unable to start the sound player using the selected audio settings!", MsgBoxStyle.Exclamation, "OSTF sound player")
            SelectedTransducer = Nothing
        End If

        If SelectedTransducer IsNot Nothing Then
            AddSounds_Button.Enabled = True
            SoundSource_FlowLayoutPanel.Enabled = True
        Else
            Transducer_ComboBox.Enabled = True

            Play_AudioButton.Enabled = False
            AddSounds_Button.Enabled = False
            SoundSource_FlowLayoutPanel.Enabled = False
            Duration_ComboBox.Enabled = False
            Duration_ComboBox.Enabled = False
            Stop_AudioButton.Enabled = False
        End If

    End Sub

    Private Sub AddSounds_Button_Click(sender As Object, e As EventArgs) Handles AddSounds_Button.Click

        If SelectedTransducer IsNot Nothing Then
            Dim SoundSourceLocations = SelectedTransducer.GetAvailableSoundSourceLocations()
            If SoundSourceLocations.Count > 0 Then

                Dim SoundFilePaths() As String = Utils.GetOpenFilePaths(,, {".wav"}, "Select a wave file", True)
                For Each SoundFilePath In SoundFilePaths

                    If SoundFilePath = "" Then
                        MsgBox("No sound file was selected!", MsgBoxStyle.Information, "OSTF sound player")
                        Exit Sub
                    End If

                    Dim LoadedSound = Audio.Sound.LoadWaveFile(SoundFilePath)
                    For SoundChannel = 1 To LoadedSound.WaveFormat.Channels

                        'Adding first a headings control, if controls are empty
                        If SoundSource_FlowLayoutPanel.Controls.Count = 0 Then
                            SoundSource_FlowLayoutPanel.Controls.Add(New OstfSoundPlayerSourceHeadings(ColumnStyleList))
                        End If
                        'Adding the control and a event handler to remove it
                        Dim NewSoundControl = New OstfSoundPlayerSourceControl(ColumnStyleList, SoundFilePath, SoundChannel, SoundSourceLocations, AvaliableLevels)
                        AddHandler NewSoundControl.Remove, AddressOf Me.RemoveSound

                        'Setting a random background color on the control
                        NewSoundControl.SetBackColor(Drawing.Color.FromArgb(20, CSng(Rnd.Next(10, 255)), CSng(Rnd.Next(10, 255)), CSng(Rnd.Next(10, 255))))

                        SoundSource_FlowLayoutPanel.Controls.Add(NewSoundControl)
                    Next

                Next

            Else
                MsgBox("No available sound source locations in the selected audio output!", MsgBoxStyle.Information, "OSTF sound player")
            End If

        Else
            AddSounds_Button.Enabled = False
            SoundSource_FlowLayoutPanel.Enabled = False
            MsgBox("No valid audio output was selected!", MsgBoxStyle.Information, "OSTF sound player")
        End If

        OstfSoundPlayerWindow_Resize(Nothing, Nothing)

        If SoundSource_FlowLayoutPanel.Controls.Count > 1 Then
            Duration_ComboBox.Enabled = True
            Play_AudioButton.Enabled = True
        Else
            Duration_ComboBox.Enabled = False
            Play_AudioButton.Enabled = False
        End If

    End Sub

    Private Sub RemoveSound(ByRef SourceControl As OstfSoundPlayerSourceControl)
        SoundSource_FlowLayoutPanel.Controls.Remove(SourceControl)
        SoundSource_FlowLayoutPanel.Invalidate()
    End Sub

    Private Sub OstfSoundPlayerWindow_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        For Each Control As Control In SoundSource_FlowLayoutPanel.Controls
            Control.Width = SoundSource_FlowLayoutPanel.Width - 22
        Next
    End Sub

    Private Sub Duration_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Duration_ComboBox.SelectedIndexChanged
        SelectedDurationSeconds = AvaliableDurationsCorrespondingSeconds(Duration_ComboBox.SelectedIndex)
    End Sub

    Private Sub Play_AudioButton_Click(sender As Object, e As EventArgs) Handles Play_AudioButton.Click

        Play_AudioButton.Enabled = False

        Progress_Label.Visible = True
        Mix_ProgressBar.Visible = True
        Progress_Label.Invalidate()
        Progress_Label.Refresh()
        Mix_ProgressBar.Invalidate()
        Mix_ProgressBar.Refresh()

        MixSounds()

        Progress_Label.Visible = False
        Mix_ProgressBar.Visible = False

        If CurrentPlaySound IsNot Nothing Then

            OstfBase.SoundPlayer.ChangePlayerSettings(SelectedTransducer.ParentAudioApiSettings, CurrentPlaySound.WaveFormat.SampleRate, CurrentPlaySound.WaveFormat.BitDepth,
                                                      CurrentPlaySound.WaveFormat.Encoding, 0.4, SelectedTransducer.Mixer, Audio.SoundPlayers.iSoundPlayer.SoundDirections.PlaybackOnly, True, True)

            OstfBase.SoundPlayer.SwapOutputSounds(CurrentPlaySound)

            AddSounds_Button.Enabled = False
            SoundSource_FlowLayoutPanel.Enabled = False
            Duration_ComboBox.Enabled = False
            Transducer_ComboBox.Enabled = False
            Duration_ComboBox.Enabled = False

            StopTimer.Stop()
            StopTimer.Interval = Math.Max(100, Math.Ceiling(1000 * (Math.Min(SelectedDurationSeconds, CurrentPlaySound.WaveData.LongestChannelSampleCount / CurrentPlaySound.WaveFormat.SampleRate) + 1))) ' Adding one second to have some margin
            StopTimer.Start()

            Stop_AudioButton.Enabled = True

        Else
            MsgBox("Unable to play the current sounds!", MsgBoxStyle.Information, "OSTF sound player")
        End If

    End Sub


    Private WithEvents StopTimer As New Windows.Forms.Timer

    Private Sub Stop_AudioButton_Click() Handles Stop_AudioButton.Click, StopTimer.Tick

        Dim SpinLockTaken As Boolean = False

        'Attempts to enter a spin lock to avoid multiple thread conflicts when saving to the same file
        StopTimer_SpinLock.Enter(SpinLockTaken)

        StopTimer.Stop()

        OstfBase.SoundPlayer.SwapOutputSounds(Nothing)
        'OstfBase.SoundPlayer.FadeOutPlayback()

        Play_AudioButton.Enabled = True
        AddSounds_Button.Enabled = True
        SoundSource_FlowLayoutPanel.Enabled = True
        Play_AudioButton.Enabled = True
        Transducer_ComboBox.Enabled = True
        Duration_ComboBox.Enabled = True

        Stop_AudioButton.Enabled = False

        'Releases any spinlock
        If SpinLockTaken = True Then StopTimer_SpinLock.Exit()

    End Sub

    Private CurrentPlaySound As Audio.Sound = Nothing

    Private Sub MixSounds()

        CurrentPlaySound = Nothing

        Dim AllSoundItems As New List(Of Audio.SoundScene.SoundSceneItem)

        Dim SoundsLoaded As New SortedList(Of String, Audio.Sound)
        Dim LevelGroup As Integer = 0

        For Each SoundSource In SoundSource_FlowLayoutPanel.Controls
            Dim CurrentControl = TryCast(SoundSource, OstfSoundPlayerSourceControl)
            If CurrentControl IsNot Nothing Then

                If SoundsLoaded.ContainsKey(SoundSource.SoundFilePath) = False Then

                    Dim LoadedSound = Audio.Sound.LoadWaveFile(SoundSource.SoundFilePath)

                    'Changing the bit depth to 32 bit
                    If LoadedSound.WaveFormat.BitDepth = 16 Then
                        LoadedSound = LoadedSound.Convert16to32bitSound()
                    ElseIf LoadedSound.WaveFormat.BitDepth = 32 Then
                        'This is ok
                    Else
                        MsgBox("Unable to read the file " & SoundSource.SoundFilePath & ". File format is not supported!", MsgBoxStyle.Information, "OSTF sound player")
                        Exit Sub
                    End If

                    SoundsLoaded.Add(SoundSource.SoundFilePath, LoadedSound)
                End If

                Dim CurrentSound = SoundsLoaded(SoundSource.SoundFilePath).CopyChannelToMonoSound(SoundSource.SoundChannel)

                If SoundSource.ShouldRepeat = True Then
                    'Extend the sound
                    Dim SoundDuration As Double = CurrentSound.WaveData.SampleData(1).Length / CurrentSound.WaveFormat.SampleRate
                    Dim Repetitions As Integer = Math.Ceiling(SelectedDurationSeconds / SoundDuration)
                    Dim RepSoundsList As New List(Of Audio.Sound)
                    For r = 1 To Repetitions
                        RepSoundsList.Add(CurrentSound.CreateSoundDataCopy)
                    Next
                    CurrentSound = Audio.DSP.ConcatenateSounds(RepSoundsList,,,,, False,,,,)
                Else
                    'Zeropadding the sound
                    CurrentSound.ZeroPad(SelectedDurationSeconds)
                End If

                AllSoundItems.Add(New Audio.SoundScene.SoundSceneItem(CurrentSound, 1, SoundSource.GetLevel, LevelGroup, SoundSource.GetSoundSourceLocation, Audio.SoundScene.SoundSceneItem.SoundSceneItemRoles.Target,,,, MySoundLevelFormat,,,))
                LevelGroup += 1

            End If
        Next

        CurrentPlaySound = SelectedTransducer.Mixer.CreateSoundScene(AllSoundItems, Me.SoundPropagationType)


    End Sub

    Private Sub About_Button_Click(sender As Object, e As EventArgs) Handles About_Button.Click
        AboutBox1.Show()
    End Sub

    Private Sub Stop_AudioButton_Click(sender As Object, e As EventArgs) Handles Stop_AudioButton.Click

    End Sub
End Class