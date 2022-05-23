
Public Class SpeechMaterialRecorder

    Private EditItems As List(Of Tuple(Of String, SpeechMaterialComponent))

    Private CurrentItemHasUnsavedChanged As Boolean = False
    Private CurrentlyLoadedSound As Audio.Sound = Nothing

    Private CurrentItemIndex As Integer = 0
    Public MinSelectionIndex As Integer = -1
    Public MaxSelectionIndex As Integer = -1

    Private RecordingWaveFormat As Audio.Formats.WaveFormat

    'SoundPlayers
    Public MyGeneralSoundPlayer As Audio.PortAudioVB.SoundPlayer

    'Sound output settings
    Public CurrentAudioApiSettings As Audio.AudioApiSettings = Nothing

    'Spectrogram settings
    Public CurrentSpectrogramFormat As Audio.Formats.SpectrogramFormat

    'TODO: This should be a setting somewhere!!!
    Public paddingTime As Double = 0.5

    Public Sub New(ByRef EditItems As List(Of Tuple(Of String, SpeechMaterialComponent)), ByRef RecordingWaveFormat As Audio.Formats.WaveFormat)

        ' This call is required by the designer.
        InitializeComponent()

        'Setting the RecordingWaveFormat
        Me.RecordingWaveFormat = RecordingWaveFormat

        ' Add any initialization after the InitializeComponent() call.
        Me.EditItems = EditItems

        SetupAudioIO()

    End Sub


    Private Sub TestWordRecorder_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        MainTabControl.SelectedIndex = 0

        RecordingTabMainSplitContainer.SplitterDistance = 2 * (Me.Width / 3)

        ' Adding items into the ItemComboBox
        For n = 1 To EditItems.Count
            ItemComboBox.Items.Add(n)
        Next

        ' Setting MinSelectionIndex
        If EditItems.Count > 0 Then
            MinSelectionIndex = 0
            MaxSelectionIndex = EditItems.Count - 1
        Else
            MinSelectionIndex = -1
            MaxSelectionIndex = -1
        End If

        SelectRecoringIndex(0)

    End Sub


    Private Sub MainTabControl_Selected(sender As Object, e As Windows.Forms.TabControlEventArgs) Handles MainTabControl.Selected

        Select Case e.TabPage.Text
            Case RecordingTab.Text
                Me.RecordingSettingsMenu.Visible = True
                Me.SegmentationSettingsMenu.Visible = False
                Me.SegmentationToolStripMenuItem.Visible = False
                DisplayRecordedSound()

            Case SegmentationTab.Text
                Me.RecordingSettingsMenu.Visible = False
                Me.SegmentationSettingsMenu.Visible = True
                Me.SegmentationToolStripMenuItem.Visible = True
                PresentSegmentationItem()

        End Select

    End Sub

    ''' <summary>
    ''' Set new audio settings.
    ''' </summary>
    Public Sub SetupAudioIO()

        Dim newAudioSettingsDialog As New AudioSettingsDialog(RecordingWaveFormat.SampleRate)
        Dim Result = newAudioSettingsDialog.ShowDialog()
        If Result = Windows.Forms.DialogResult.OK Then
            CurrentAudioApiSettings = newAudioSettingsDialog.CurrentAudioApiSettings
        Else
            'Attempting to set default AudioApiSettings if the user pressed ok
            CurrentAudioApiSettings.SelectDefaultAudioDevice(RecordingWaveFormat.SampleRate)
        End If

        If CurrentAudioApiSettings IsNot Nothing Then
            'Creating a new sound player with the updated audio settings
            CreateAudioPlayer()
        Else
            MsgBox("Unable to set audio device! You will not be able to play or record audio.", MsgBoxStyle.Exclamation, "No audio device?")
        End If

    End Sub

    Public Sub CreateAudioPlayer()

        If MyGeneralSoundPlayer IsNot Nothing Then
            MyGeneralSoundPlayer.Dispose()
        End If

        Dim TemporaryOutputSound As Audio.Sound = Audio.GenerateSound.CreateSilence(RecordingWaveFormat,, 1)
        MyGeneralSoundPlayer = New Audio.PortAudioVB.SoundPlayer(False, RecordingWaveFormat, TemporaryOutputSound, CurrentAudioApiSettings,
                                                                         False, False, False, True, True)

    End Sub


    Private Sub UpdateSoundLevelMeter(ByRef NewBuffer As Audio.Sound)

        'NB this should be done on a background thread!

        If RecordingSoundLevelMeter.Activated = True Then

            'Updating meter data
            RecordingSoundLevelMeter.minLevel = -100
            RecordingSoundLevelMeter.maxLevel = 12
            Dim peakLevel As Double = Audio.DSP.MeasureSectionLevel(NewBuffer, 1,,, Audio.AudioManagement.SoundDataUnit.linear, Audio.AudioManagement.SoundMeasurementType.AbsolutePeakAmplitude)
            If peakLevel = 0 Then
                RecordingSoundLevelMeter.UpdateLevel(RecordingSoundLevelMeter.minLevel)
            Else
                RecordingSoundLevelMeter.UpdateLevel(Audio.dBConversion(peakLevel, Audio.AudioManagement.dBConversionDirection.to_dB, RecordingWaveFormat))
            End If

        End If

    End Sub




    Private Sub SelectRecoringIndex(ByVal NewIndex As Integer)

        If CurrentlyLoadedSound IsNot Nothing Then
            If CurrentItemHasUnsavedChanged = True Then

                Dim Res = MsgBox("The current sound has unsaved changes. Do you want to save the changes? (This will overwrite the old loaded sound file!)", MsgBoxStyle.YesNo, "Save file?")

                If Res = MsgBoxResult.Yes Then
                    If CurrentlyLoadedSound.WriteWaveFile(EditItems(CurrentItemIndex).Item1) = False Then
                        MsgBox("Unable to save the current sound (" & EditItems(CurrentItemIndex).Item1 & ") to file. Unknown reason. Is it open in another application?")
                        Exit Sub
                    End If
                End If
            End If
        End If

        Select Case NewIndex
            Case < MinSelectionIndex

                MsgBox("Index too low")

            Case > MaxSelectionIndex

                MsgBox("Index too high")

            Case Else
                CurrentItemIndex = NewIndex

                ItemComboBox.SelectedItem = CurrentItemIndex + 1

                Select Case MainTabControl.SelectedTab.Text
                    Case RecordingTab.Text

                        DisplayRecordedSound()

                    Case SegmentationTab.Text

                        PresentSegmentationItem()

                End Select

        End Select

    End Sub


    Public Sub DisplayRecordedSound()

        Try

            If CurrentItemIndex >= 0 Then

                Dim SoundPath = EditItems(CurrentItemIndex).Item1

                If IO.File.Exists(SoundPath) = True Then

                    CurrentlyLoadedSound = Audio.AudioIOs.LoadWaveFile(SoundPath)

                    'Resetting sound display
                    If RecordingTabMainSplitContainer.Panel2.Controls.Count > 0 Then RecordingTabMainSplitContainer.Panel2.Controls.RemoveAt(0)

                    'Dim soundPanel As New Windows.Forms.SplitContainer
                    'soundPanel.Dock = Windows.Forms.DockStyle.Fill

                    Dim waveDrawer As New Audio.Graphics.SoundEditor(CurrentlyLoadedSound,,,,,,,,, MyGeneralSoundPlayer)
                    waveDrawer.Dock = Windows.Forms.DockStyle.Fill

                    RecordingTabMainSplitContainer.Panel2.Controls.Add(waveDrawer)

                    ListenButton.Enabled = True

                Else

                    CurrentlyLoadedSound = Nothing

                    RecordingTabMainSplitContainer.Controls.Clear()

                    Dim noSoundLabel As New Windows.Forms.Label
                    noSoundLabel.Dock = Windows.Forms.DockStyle.Fill
                    noSoundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                    noSoundLabel.Text = "No sound is yet recorded for this item."
                    RecordingTabMainSplitContainer.Panel2.Controls.Add(noSoundLabel)

                    ListenButton.Enabled = False

                End If

            Else

                RecordingTabMainSplitContainer.Controls.Clear()

            End If

        Catch ex As Exception

            MsgBox("No sound to show!")

        End Try


    End Sub


    Public Sub PresentSegmentationItem()


        Try

            Dim SoundPath = EditItems(CurrentItemIndex).Item1

            If IO.File.Exists(SoundPath) = True Then

                CurrentlyLoadedSound = Audio.AudioIOs.LoadWaveFile(SoundPath)

                'Diplays the sound if there is any
                If CurrentlyLoadedSound.WaveData.ShortestChannelSampleCount > 0 = True Then

                    'Resetting sound display
                    If SegmentationPanel.Controls.Count > 0 Then SegmentationPanel.Controls.RemoveAt(0)

                    'Dim newSoundPanel As New Windows.Forms.SplitContainer
                    'newSoundPanel.Dock = Windows.Forms.DockStyle.Fill

                    If CurrentSpectrogramFormat Is Nothing Then
                        Dim SpectrogramSettingsResult As New SpectrogramSettingsDialog
                        If SpectrogramSettingsResult.ShowDialog = Windows.Forms.DialogResult.OK Then
                            CurrentSpectrogramFormat = SpectrogramSettingsResult.NewSpectrogramFormat
                        Else
                            CurrentSpectrogramFormat = New Audio.Formats.SpectrogramFormat(, 1024,, 512,, True,,,, True)
                        End If
                    End If

                    'SoundEditor
                    Dim TestSound = Audio.Sound.GetTestSound
                    Dim waveDrawer As New Audio.Graphics.SoundEditor(TestSound,,,, True, True, CurrentSpectrogramFormat, paddingTime, True, MyGeneralSoundPlayer)
                    'Dim waveDrawer As New Audio.Graphics.SoundEditor(CurrentlyLoadedSound,,,, True, True, CurrentSpectrogramFormat, paddingTime, True, MyGeneralSoundPlayer)
                    waveDrawer.Dock = Windows.Forms.DockStyle.Fill
                    SegmentationPanel.Controls.Add(waveDrawer)

                Else
                    'Resets the sound display, and adds a message that no sound is recorded
                    If SegmentationPanel.Controls.Count > 0 Then SegmentationPanel.Controls.RemoveAt(0)

                    Dim noSoundLabel As New Windows.Forms.Label
                    noSoundLabel.Dock = Windows.Forms.DockStyle.Fill
                    noSoundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                    noSoundLabel.Text = "No sound is yet recorded for this item."
                    SegmentationPanel.Controls.Add(noSoundLabel)

                End If

            Else
                CurrentlyLoadedSound = Nothing

            End If

        Catch ex As Exception
            MsgBox("The following exception occurred: " & ex.ToString)
        End Try

    End Sub

    Private Sub IOSettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IOSettingsToolStripMenuItem.Click

        SetupAudioIO()

    End Sub

    Private Sub ListenButton_Click(sender As Object, e As EventArgs) Handles ListenButton.Click

        DisplayRecordedSound()

    End Sub

    Private Sub IncreaseFontSizeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IncreaseFontSizeToolStripMenuItem.Click

    End Sub

    Private Sub DecreaseFontSizeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DecreaseFontSizeToolStripMenuItem.Click

    End Sub

    Private Sub SetFontOfSpellingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetFontOfSpellingsToolStripMenuItem.Click

    End Sub

    Private Sub SetFontOfPhoneticTranscriptionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetFontOfPhoneticTranscriptionsToolStripMenuItem.Click

    End Sub

    Private Sub AuditoryPrequeingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AuditoryPrequeingToolStripMenuItem.Click

    End Sub

    Private Sub StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem.Click

    End Sub

    Private Sub ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem.Click

    End Sub

    Private Sub ToggleSoundLevelMeteronoffToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToggleSoundLevelMeteronoffToolStripMenuItem.Click

    End Sub

    Private Sub SpectrogramSettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SpectrogramSettingsToolStripMenuItem.Click

    End Sub

    Private Sub CalibrateOutputLevelToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CalibrateOutputLevelToolStripMenuItem.Click

    End Sub

    Private Sub SelectTransducerTypeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectTransducerTypeToolStripMenuItem.Click

    End Sub

    Private Sub AutoDetectBoundariesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AutoDetectBoundariesToolStripMenuItem.Click

    End Sub

    Private Sub BoundaryDetectionSettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BoundaryDetectionSettingsToolStripMenuItem.Click

    End Sub

    Private Sub UpdateAllSegmentationsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UpdateAllSegmentationsToolStripMenuItem.Click

    End Sub

    Private Sub ValidateAllSegmentationsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ValidateAllSegmentationsToolStripMenuItem.Click

    End Sub

    Private Sub FadeAllPaddingSectionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FadeAllPaddingSectionsToolStripMenuItem.Click

    End Sub

    Private Sub MoveSegmentationsToZeroCrossingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoveSegmentationsToZeroCrossingsToolStripMenuItem.Click

    End Sub

    Private Sub ResetAllSegmentationDataToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ResetAllSegmentationDataToolStripMenuItem.Click

    End Sub


    Private Sub StartRecordingButton_Click(sender As Object, e As EventArgs) Handles StartRecordingButton.Click

    End Sub

    Private Sub StopRecordingButton_Click(sender As Object, e As EventArgs) Handles StopRecordingButton.Click

    End Sub

    Private Sub Top_PreviousItemButton_Click(sender As Object, e As EventArgs) Handles Top_PreviousItemButton.Click, Rec_PreviousItemButton.Click

        SelectRecoringIndex(CurrentItemIndex - 1)

    End Sub

    Private Sub Top_NextItemButton_Click(sender As Object, e As EventArgs) Handles Top_NextItemButton.Click, Rec_NextItemButton.Click

        SelectRecoringIndex(CurrentItemIndex + 1)

    End Sub

    Private Sub ItemComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ItemComboBox.SelectedIndexChanged

    End Sub


End Class
