
Public Class SpeechMaterialRecorder


    'TODO: Large change: The way sounds are stored need to be updated! In addition, all(?) methods need to be implemented

    Private ParentSoundLibrary As SpeechAudiometrySoundLibrary

    Public AllRecordings As List(Of Tuple(Of String, SpeechMaterialLibrary.TestWordRecording))

    Private CurrentRecordingIndex As Integer = 0
    Public MinSelectionIndex As Integer = -1
    Public MaxSelectionIndex As Integer = -1

    Public Property RecordingWaveFormat As Audio.Formats.WaveFormat

    'SoundPlayers
    Public MyGeneralSoundPlayer As Audio.PortAudioVB.SoundPlayer

    'Sound output settings
    Public CurrentAudioApiSettings As Audio.AudioApiSettings

    'Spectrogram settings
    Public CurrentSpectrogramFormat As Audio.Formats.SpectrogramFormat

    'TODO: This should be a setting somewhere!!!
    Public paddingTime As Double = 0.5

    Public Sub New(ByRef ParentSoundLibrary As SpeechAudiometrySoundLibrary)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.ParentSoundLibrary = ParentSoundLibrary

        UpdateAllRecordingsList("Tyst")

        SelectRecoringIndex(0)

    End Sub

    Private Sub TestWordRecorder_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        MainTabControl.SelectedIndex = 0

        RecordingTabMainSplitContainer.SplitterDistance = 2 * (Me.Width / 3)

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

        Dim newAudioSettingsDialog As New AudioSettingsDialog()
        Dim Result = newAudioSettingsDialog.ShowDialog()
        If Result = Windows.Forms.DialogResult.OK Then
            CurrentAudioApiSettings = newAudioSettingsDialog.CurrentAudioApiSettings

            'Updating the recording format with the selected sample rate
            RecordingWaveFormat = New Audio.Formats.WaveFormat(CurrentAudioApiSettings.SampleRate, 32, 1)

            'Creating a new sound player with the updated audio settings
            If MyGeneralSoundPlayer IsNot Nothing Then MyGeneralSoundPlayer.Dispose()

            Dim TemporaryOutputSound As Audio.Sound = Audio.GenerateSound.CreateSilence(RecordingWaveFormat,, 1)

            MyGeneralSoundPlayer = New Audio.PortAudioVB.SoundPlayer(False, RecordingWaveFormat, TemporaryOutputSound, CurrentAudioApiSettings,
                                                                         False, False, False, True, True)

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


    Public Sub UpdateAllRecordingsList(ByVal TestSituationName As String)

        'Creates a list of all sounds
        Dim FilenameList As New SortedSet(Of String)
        Dim TempList As New List(Of Tuple(Of String, SpeechMaterialLibrary.TestWordRecording))

        For Each TestWordList In ParentSoundLibrary.ParentSpeechMaterial.TestWordLists
            For Each TestWord In TestWordList.MemberWords
                For Each TestStimulus In TestWord.Recordings(TestSituationName)

                    Dim FileName As String = TestStimulus.SoundFileNameWithoutExtension

                    'Skips if the sound is already added (from another list)
                    'N.B. This means that even though two different instances of TestWordRecording may have some features that differ, only the features from the first one will be used.
                    If FilenameList.Contains(FileName) Then Continue For

                    'Adds the sound to the list
                    TempList.Add(New Tuple(Of String, SpeechMaterialLibrary.TestWordRecording)(FileName, TestStimulus))

                Next
            Next
        Next

        AllRecordings = TempList

        For n = 1 To AllRecordings.Count
            ItemComboBox.Items.Add(n)
        Next

        If AllRecordings.Count > 0 Then
            MinSelectionIndex = 0
            MaxSelectionIndex = AllRecordings.Count - 1
        Else
            MinSelectionIndex = -1
            MaxSelectionIndex = -1
        End If

    End Sub

    Private Sub SelectRecoringIndex(ByVal NewIndex As Integer)

        Select Case NewIndex
            Case < MinSelectionIndex

                MsgBox("Index too low")

            Case > MaxSelectionIndex

                MsgBox("Index too high")

            Case Else
                CurrentRecordingIndex = NewIndex

                ItemComboBox.SelectedItem = CurrentRecordingIndex + 1

                Select Case MainTabControl.SelectedTab.Text
                    Case RecordingTab.Text

                        DisplayRecordedSound()

                    Case SegmentationTab.Text

                        PresentSegmentationItem()

                End Select

        End Select

    End Sub

    Public Function GetLackingTestWordRecordings(ByVal TestSituationName As String) As SortedList(Of String, SpeechMaterialLibrary.TestWordRecording)


        'Creates a list of all sounds
        Dim RecordingsNeeded As New SortedList(Of String, SpeechMaterialLibrary.TestWordRecording)

        For Each TestWordList In ParentSoundLibrary.ParentSpeechMaterial.TestWordLists
            For Each TestWord In TestWordList.MemberWords
                For Each TestStimulus In TestWord.Recordings(TestSituationName)

                    Dim FileName As String = TestStimulus.SoundFileNameWithoutExtension

                    'Skips if the sound is already added (from another list)
                    'N.B. This means that even though two different instances of TestWordRecording may have some features that differ, only the features from the first one will be used.
                    If RecordingsNeeded.ContainsKey(FileName) Then Continue For

                    'Adds the sound to the list
                    RecordingsNeeded.Add(FileName, TestStimulus)

                Next
            Next
        Next

        'Getting the recordings lacking
        Dim RecordingsLacking As New SortedList(Of String, SpeechMaterialLibrary.TestWordRecording)
        For Each Recording In RecordingsNeeded
            If ParentSoundLibrary.CheckIfTestWordRecordingExists(Recording.Key, TestSituationName) Then
                RecordingsLacking.Add(Recording.Key, Recording.Value)
            End If
        Next

        Return RecordingsLacking

    End Function



    Public Sub DisplayRecordedSound()

        Try

            If CurrentRecordingIndex >= 0 Then

                Dim SoundRecordingFileName = AllRecordings(CurrentRecordingIndex).Item1

                If ParentSoundLibrary.CheckIfTestWordRecordingExists(SoundRecordingFileName, ParentSoundLibrary.CurrentTestSituationName) = True Then

                    Dim SoundPath As String = IO.Path.Combine(ParentSoundLibrary.ParentSpeechMaterial.AvailableTestSituations(ParentSoundLibrary.CurrentTestSituationName).TestWordSoundFolder, SoundRecordingFileName & ".wav")

                    Dim ShowSound = Audio.AudioIOs.LoadWaveFile(SoundPath)

                    'Resetting sound display
                    If RecordingTabMainSplitContainer.Panel2.Controls.Count > 0 Then RecordingTabMainSplitContainer.Panel2.Controls.RemoveAt(0)

                    Dim soundPanel As New Windows.Forms.SplitContainer
                    soundPanel.Dock = Windows.Forms.DockStyle.Fill

                    Dim waveDrawer As New Audio.Graphics.SoundEditor(ShowSound, soundPanel,,,,,,,, MyGeneralSoundPlayer)

                    RecordingTabMainSplitContainer.Panel2.Controls.Add(soundPanel)

                    ListenButton.Enabled = True

                Else

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

            Dim ShowSound = Audio.AudioIOs.LoadWaveFile("C:\SpeechAndHearingToolsLog\F_001_000_hyrs.wav")

            'Dim ShowSound = Audio.GenerateSound.CreateSineWave(New Audio.Formats.WaveFormat(48000, 32, 1),, 2000)

            'Diplays the sound if there is any
            If ShowSound.WaveData.ShortestChannelSampleCount > 0 = True Then

                'Resetting sound display
                If SegmentationPanel.Controls.Count > 0 Then SegmentationPanel.Controls.RemoveAt(0)

                Dim newSoundPanel As New Windows.Forms.SplitContainer
                newSoundPanel.Dock = Windows.Forms.DockStyle.Fill

                If CurrentSpectrogramFormat Is Nothing Then
                    Dim SpectrogramSettingsResult As New SpectrogramSettingsDialog
                    If SpectrogramSettingsResult.ShowDialog = Windows.Forms.DialogResult.OK Then
                        CurrentSpectrogramFormat = SpectrogramSettingsResult.NewSpectrogramFormat
                    Else
                        CurrentSpectrogramFormat = New Audio.Formats.SpectrogramFormat(, 1024,, 512,, True,,,, True)
                    End If
                End If

                'SoundEditor
                Dim waveDrawer As New Audio.Graphics.SoundEditor(ShowSound, newSoundPanel,,, True, True, CurrentSpectrogramFormat, paddingTime, True, MyGeneralSoundPlayer)

                SegmentationPanel.Controls.Add(newSoundPanel)

            Else
                'Resets the sound display, and adds a message that no sound is recorded
                If SegmentationPanel.Controls.Count > 0 Then SegmentationPanel.Controls.RemoveAt(0)

                Dim noSoundLabel As New Windows.Forms.Label
                noSoundLabel.Dock = Windows.Forms.DockStyle.Fill
                noSoundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                noSoundLabel.Text = "No sound is yet recorded for this item."
                SegmentationPanel.Controls.Add(noSoundLabel)

            End If

        Catch ex As Exception
            MsgBox(ex.ToString)
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

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

    End Sub

    Private Sub StartRecordingButton_Click(sender As Object, e As EventArgs) Handles StartRecordingButton.Click

    End Sub

    Private Sub StopRecordingButton_Click(sender As Object, e As EventArgs) Handles StopRecordingButton.Click

    End Sub

    Private Sub Top_PreviousItemButton_Click(sender As Object, e As EventArgs) Handles Top_PreviousItemButton.Click

        SelectRecoringIndex(CurrentRecordingIndex - 1)

    End Sub

    Private Sub Top_NextItemButton_Click(sender As Object, e As EventArgs) Handles Top_NextItemButton.Click

        SelectRecoringIndex(CurrentRecordingIndex + 1)

    End Sub
End Class

#Region "OnlyTemporary_Remove"
Public Class SpeechAudiometrySoundLibrary
    Public Property ParentSpeechMaterial
    Public Property CurrentTestSituationName
    Public Function CheckIfTestWordRecordingExists()
        Throw New NotImplementedException
    End Function
End Class
Public Class SpeechMaterialLibrary
    Public Class TestWordRecording
    End Class
End Class
#End Region