Imports System.Windows.Forms
Imports System.Windows.Forms.Control


Public Class SpeechMaterialRecorder

    Private SoundFilesForEditing As New List(Of Tuple(Of String, SpeechMaterialComponent))

    Private CurrentlyLoadedSoundFile As Audio.Sound = Nothing
    Private CurrentSoundFileIndex As Integer = -1

    'SoundPlayers
    Private MyGeneralSoundPlayer As Audio.PortAudioVB.SoundPlayer

    'Sound output settings
    Private CurrentAudioApiSettings As Audio.AudioApiSettings = Nothing


#Region "Recording variables"

    Private RecordingWaveFormat As Audio.Formats.WaveFormat
    Private UseAuditoryPrequeing As Boolean = False
    Private AutoStartRecording As Boolean = False
    Private UseRecordingNoise As Boolean = False
    Private ShowSoundLevelMeter As Boolean = False

#End Region


#Region "Segmentation variables"

    Private CurrentSpectrogramFormat As Audio.Formats.SpectrogramFormat
    Private SetSegmentationToZeroCrossings As Boolean
    Private ShowSpectrogram As Boolean = False
    Private PaddingTime As Single = 0.5
    Private InterSentenceTime As Single = 4
    Private DrawNormalizedWave As Boolean = False

#End Region


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        'Setting up a default RecordingWaveFormat
        SetupAudioIO()

    End Sub

    Public Sub New(ByRef EditItems As List(Of Tuple(Of String, SpeechMaterialComponent)), ByRef RecordingWaveFormat As Audio.Formats.WaveFormat)

        ' This call is required by the designer.
        InitializeComponent()

        'Setting the RecordingWaveFormat
        Me.RecordingWaveFormat = RecordingWaveFormat

        ' Add any initialization after the InitializeComponent() call.
        Me.SoundFilesForEditing = EditItems

        SetupAudioIO()

    End Sub


    Private Sub TestWordRecorder_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        FileName_Label.Text = "No file loaded"
        SoundFilePathStatusLabel.Text = "No file loaded"

        MainTabControl.SelectedIndex = 0

        RecordingTabMainSplitContainer.SplitterDistance = 2 * (Me.Width / 3)

        ' Adding items into the ItemComboBox
        For n = 1 To SoundFilesForEditing.Count
            FileComboBox.Items.Add(n)
        Next

        If SoundFilesForEditing.Count > 0 Then SelectSoundFileIndex(0)

    End Sub

#Region "IO"

    Private Sub SpeechMaterialRecorder_FormClosing(sender As Object, e As Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        'Checks if the user wants to save the sound before closing
        CheckIfSaveSound()
    End Sub

    Private Function LoadSoundFile(ByVal FilePath As String) As Boolean

        Dim Succeeded As Boolean = False

        If IO.File.Exists(FilePath) = True Then
            Dim TempSound = Audio.Sound.LoadWaveFile(FilePath,,,,, True)
            If TempSound IsNot Nothing Then
                CheckIfSaveSound()
                CurrentlyLoadedSoundFile = TempSound
                Succeeded = True
            End If
        End If

        If Succeeded = True Then
            FileName_Label.Text = IO.Path.GetFileName(FilePath)
            SoundFilePathStatusLabel.Text = FilePath
        End If

        Return Succeeded

    End Function


    ''' <summary>
    ''' Stores a user selected sound file path in the SoundFilesForEditing object
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem.Click

        Dim FilePath As String = Utils.GetOpenFilePath("",, {".wav"}, "Open wave file (SMA iXML chunk required)", True)
        If FilePath <> "" Then
            SoundFilesForEditing.Add(New Tuple(Of String, SpeechMaterialComponent)(FilePath, Nothing))
            FileComboBox.Items.Add(SoundFilesForEditing.Count)
            SelectSoundFileIndex(SoundFilesForEditing.Count - 1)
        Else
            MsgBox("Unable to load the sound file from " & FilePath)
        End If

    End Sub

    ''' <summary>
    ''' Checks for changes in the CurrentlyLoadedSoundFile and if changes are found ask the user to save or discard those changes.
    ''' </summary>
    Private Sub CheckIfSaveSound()

        If CurrentlyLoadedSoundFile IsNot Nothing Then
            If CurrentlyLoadedSoundFile.IsChanged = True Then

                Dim Res = MsgBox("The current sound has unsaved changes. Do you want to save the changes? (This will overwrite the old loaded sound file!)", MsgBoxStyle.YesNo, "Save file?")

                If Res = MsgBoxResult.Yes Then
                    If CurrentlyLoadedSoundFile.WriteWaveFile(SoundFilesForEditing(CurrentSoundFileIndex).Item1) = False Then
                        MsgBox("Unable to save the current sound (" & SoundFilesForEditing(CurrentSoundFileIndex).Item1 & ") to file. Unknown reason. Is it open in another application?")
                        Exit Sub
                    End If
                End If
            End If
        End If

    End Sub

    ''' <summary>
    ''' Saves the CurrentlyLoadedSoundFile object to file, overwriting the original file.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SaveWaveFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveWaveFileToolStripMenuItem.Click

        If CurrentlyLoadedSoundFile IsNot Nothing Then
            If CurrentlyLoadedSoundFile.WriteWaveFile(SoundFilesForEditing(CurrentSoundFileIndex).Item1) = False Then
                MsgBox("Unable to save the current sound (" & SoundFilesForEditing(CurrentSoundFileIndex).Item1 & ") to file. Unknown reason. Is it open in another application?")
            End If
        End If

    End Sub

    ''' <summary>
    ''' Saves the CurrentlyLoadedSoundFile object to a user selected location.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SaveWaveFileAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveWaveFileAsToolStripMenuItem.Click

        If CurrentlyLoadedSoundFile IsNot Nothing Then

            Dim FilePath As String = Utils.GetSaveFilePath(,, {".wav"}, "Save wave file as...")
            If FilePath <> "" Then
                If CurrentlyLoadedSoundFile.WriteWaveFile(FilePath) = False Then
                    MsgBox("Unable to save the current sound to " & FilePath & ". Unknown reason. Is the file open in another application?")
                End If
            Else
                MsgBox("No file path was supplied.", MsgBoxStyle.Exclamation, "Save wave file as...")
            End If
        Else
            MsgBox("No sound to save.", MsgBoxStyle.Information, "Save as...")
        End If

    End Sub


#End Region

#Region "Setup audio"

    Private Sub IOSettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IOSettingsToolStripMenuItem.Click
        SetupAudioIO()
    End Sub

    ''' <summary>
    ''' Set new audio settings.
    ''' </summary>
    Public Sub SetupAudioIO()

        Dim newAudioSettingsDialog As AudioSettingsDialog = Nothing

        If RecordingWaveFormat IsNot Nothing Then
            newAudioSettingsDialog = New AudioSettingsDialog(RecordingWaveFormat.SampleRate)
        Else
            newAudioSettingsDialog = New AudioSettingsDialog()
            RecordingWaveFormat = New Audio.Formats.WaveFormat(newAudioSettingsDialog.CurrentAudioApiSettings.SampleRate, 32, 1, , Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints)
        End If

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

    Public Sub SetSpectrogramFormatInDialog()
        Dim SpectrogramSettingsResult As New SpectrogramSettingsDialog
        If SpectrogramSettingsResult.ShowDialog = Windows.Forms.DialogResult.OK Then
            CurrentSpectrogramFormat = SpectrogramSettingsResult.NewSpectrogramFormat
        Else
            CurrentSpectrogramFormat = New Audio.Formats.SpectrogramFormat(, 1024,, 512,, True,,,, True)
        End If
    End Sub

#End Region


#Region "Select currently loaded sound file"

    Public Function MinSoundFileSelectionIndex() As Integer
        If SoundFilesForEditing Is Nothing Then
            Return -1
        Else
            Return 0
        End If
    End Function

    Public Function MaxSoundFileSelectionIndex() As Integer
        If SoundFilesForEditing Is Nothing Then
            Return -1
        Else
            Return SoundFilesForEditing.Count - 1
        End If
    End Function

    Private Sub Top_PreviousFileButton_Click(sender As Object, e As EventArgs) Handles Top_PreviousFileButton.Click
        SelectSoundFileIndex(CurrentSoundFileIndex - 1)
    End Sub

    Private Sub Top_NextFileButton_Click(sender As Object, e As EventArgs) Handles Top_NextFileButton.Click
        SelectSoundFileIndex(CurrentSoundFileIndex + 1)
    End Sub

    Private Sub FileComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles FileComboBox.SelectedIndexChanged
        SelectSoundFileIndex(FileComboBox.SelectedIndex)
    End Sub

    Private Sub SelectSoundFileIndex(ByVal NewIndex As Integer)

        CheckIfSaveSound()

        Select Case NewIndex
            Case < MinSoundFileSelectionIndex()

                MsgBox("Selected sound file index too low (you've already at the first file)")

            Case > MaxSoundFileSelectionIndex()

                MsgBox("Selected sound file index too high (you've already at the last file)")

            Case Else
                CurrentSoundFileIndex = NewIndex

                FileComboBox.SelectedIndex = CurrentSoundFileIndex

                Select Case MainTabControl.SelectedTab.Text
                    Case RecordingTab.Text

                        LoadSoundForRecording()

                    Case SegmentationTab.Text

                        LoadSoundForSegmentation()

                End Select

        End Select

        UpdateProgressBar()

    End Sub

    Private Sub UpdateProgressBar()

        If SoundFilesForEditing IsNot Nothing Then
            ItemProgressBar.Minimum = 0
            ItemProgressBar.Maximum = SoundFilesForEditing.Count
            If CurrentSoundFileIndex >= 0 Then
                ItemProgressBar.Value = CurrentSoundFileIndex
            End If
        End If

    End Sub

#End Region

#Region "Select view"

    Private Sub MainTabControl_Selected(sender As Object, e As Windows.Forms.TabControlEventArgs) Handles MainTabControl.Selected

        CheckIfSaveSound()

        Select Case e.TabPage.Text
            Case RecordingTab.Text
                Me.RecordingSettingsMenu.Visible = True
                Me.SegmentationToolStripMenuItem.Visible = False
                LoadSoundForRecording()

            Case SegmentationTab.Text
                Me.RecordingSettingsMenu.Visible = False
                Me.SegmentationToolStripMenuItem.Visible = True
                LoadSoundForSegmentation()

        End Select

    End Sub

    Public Sub LoadSoundForRecording()

        Try

            If CurrentSoundFileIndex >= 0 Then

                Dim SoundPath = SoundFilesForEditing(CurrentSoundFileIndex).Item1

                If LoadSoundFile(SoundPath) = True Then

                    'Resetting sound display
                    If RecordingTabMainSplitContainer.Panel2.Controls.Count > 0 Then RecordingTabMainSplitContainer.Panel2.Controls.RemoveAt(0)

                    Dim waveDrawer As New Audio.Graphics.SoundEditor(CurrentlyLoadedSoundFile,,,,,,,,,, MyGeneralSoundPlayer)
                    waveDrawer.Dock = Windows.Forms.DockStyle.Fill

                    RecordingTabMainSplitContainer.Panel2.Controls.Add(waveDrawer)

                    ListenButton.Enabled = True

                Else

                    CurrentlyLoadedSoundFile = Nothing

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
            MsgBox("The following exception occurred: " & ex.ToString)
        End Try

    End Sub


    Public Sub LoadSoundForSegmentation()

        Try
            If CurrentSoundFileIndex >= 0 Then

                Dim SoundPath = SoundFilesForEditing(CurrentSoundFileIndex).Item1

                If LoadSoundFile(SoundPath) = True Then

                    'Diplays the sound if there is any
                    If CurrentlyLoadedSoundFile.WaveData.ShortestChannelSampleCount > 0 = True Then

                        'Resetting sound display
                        If SegmentationPanel.Controls.Count > 0 Then SegmentationPanel.Controls.RemoveAt(0)

                        'Dim newSoundPanel As New Windows.Forms.SplitContainer
                        'newSoundPanel.Dock = Windows.Forms.DockStyle.Fill

                        If CurrentSpectrogramFormat Is Nothing Then
                            SetSpectrogramFormatInDialog()
                        End If

                        'SoundEditor
                        Dim TestSound = Audio.Sound.GetTestSound
                        'Dim waveDrawer As New Audio.Graphics.SoundEditor(TestSound,,,, True, True, CurrentSpectrogramFormat, paddingTime, True, MyGeneralSoundPlayer)
                        Dim waveDrawer As New Audio.Graphics.SoundEditor(CurrentlyLoadedSoundFile,,,, True, ShowSpectrogram, CurrentSpectrogramFormat, PaddingTime,
                                                                         InterSentenceTime, DrawNormalizedWave, MyGeneralSoundPlayer, SetSegmentationToZeroCrossings)

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
                    CurrentlyLoadedSoundFile = Nothing
                End If
            Else
                SegmentationPanel.Controls.Clear()
            End If

        Catch ex As Exception
            MsgBox("The following exception occurred: " & ex.ToString)
        End Try

    End Sub

#End Region

#Region "RecordingView"


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

    Private Sub Rec_PreviousItemButton_Click(sender As Object, e As EventArgs) Handles Rec_PreviousItemButton.Click

    End Sub

    Private Sub Rec_NextItemButton_Click(sender As Object, e As EventArgs) Handles Rec_NextItemButton.Click

    End Sub


    Private Sub ListenButton_Click(sender As Object, e As EventArgs) Handles ListenButton.Click


    End Sub

    Private Sub StartRecordingButton_Click(sender As Object, e As EventArgs) Handles StartRecordingButton.Click

    End Sub

    Private Sub StopRecordingButton_Click(sender As Object, e As EventArgs) Handles StopRecordingButton.Click

    End Sub

#Region "Font change"


    Private Sub IncreaseFontSizeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IncreaseFontSizeToolStripMenuItem.Click
        Rec_PresentationLabelFontSizeChange(SizeChangeDirection.Increase)
    End Sub

    Private Sub DecreaseFontSizeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DecreaseFontSizeToolStripMenuItem.Click
        Rec_PresentationLabelFontSizeChange(SizeChangeDirection.Decrease)
    End Sub

    Public Enum SizeChangeDirection
        Increase
        Decrease
    End Enum

    Public Sub Rec_PresentationLabelFontSizeChange(ByVal direction As SizeChangeDirection)

        Select Case direction
            Case SizeChangeDirection.Increase

                Dim currentFontSize As Single = Spelling_AutoHeightTextBox.Font.Size
                Spelling_AutoHeightTextBox.Font = New Drawing.Font(Spelling_AutoHeightTextBox.Font.Name, currentFontSize + 2, Spelling_AutoHeightTextBox.Font.Style, Spelling_AutoHeightTextBox.Font.Unit)
                Transcription_AutoHeightTextBox.Font = New Drawing.Font(Transcription_AutoHeightTextBox.Font.Name, currentFontSize + 2, Transcription_AutoHeightTextBox.Font.Style, Transcription_AutoHeightTextBox.Font.Unit)
            Case SizeChangeDirection.Decrease
                Dim currentFontSize As Single = Spelling_AutoHeightTextBox.Font.Size
                Spelling_AutoHeightTextBox.Font = New Drawing.Font(Spelling_AutoHeightTextBox.Font.Name, currentFontSize - 2, Spelling_AutoHeightTextBox.Font.Style, Spelling_AutoHeightTextBox.Font.Unit)
                Transcription_AutoHeightTextBox.Font = New Drawing.Font(Transcription_AutoHeightTextBox.Font.Name, currentFontSize - 2, Transcription_AutoHeightTextBox.Font.Style, Transcription_AutoHeightTextBox.Font.Unit)
        End Select
    End Sub

    Private Sub SetFontOfSpellingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetFontOfSpellingsToolStripMenuItem.Click
        OrthographicFontChange()
    End Sub
    Public Sub OrthographicFontChange()
        Dim newFont As Drawing.Font = Spelling_AutoHeightTextBox.Font
        Dim fdb As New FontDialog
        If fdb.ShowDialog = DialogResult.OK Then
            newFont = fdb.Font
        End If
        Spelling_AutoHeightTextBox.Font = newFont
    End Sub


    Private Sub SetFontOfPhoneticTranscriptionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetFontOfPhoneticTranscriptionsToolStripMenuItem.Click
        PhoneticFontChange()
    End Sub

    Public Sub PhoneticFontChange()
        Dim newFont As Drawing.Font = Transcription_AutoHeightTextBox.Font
        Dim fdb As New FontDialog
        If fdb.ShowDialog = DialogResult.OK Then
            newFont = fdb.Font
        End If
        Transcription_AutoHeightTextBox.Font = newFont
    End Sub

#End Region


    Private Sub AuditoryPrequeingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AuditoryPrequeingToolStripMenuItem.Click
        UseAuditoryPrequeing = Not UseAuditoryPrequeing
        AuditoryPrequeingToolStripMenuItem.Checked = UseAuditoryPrequeing
        If UseAuditoryPrequeing = True Then
            preQueLabel.Text = "Pre-queing: On"
        Else
            preQueLabel.Text = "Pre-queing: Off"
        End If

    End Sub

    Private Sub StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem.Click
        AutoStartRecording = Not AutoStartRecording
        StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem.Checked = AutoStartRecording

        If AutoStartRecording = True Then
            AutoRecordingStatusLabel.Text = "Auto-recording: On"
        Else
            AutoRecordingStatusLabel.Text = "Auto-recording: Off"
        End If

    End Sub

    Private Sub ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem.Click
        UseRecordingNoise = Not UseRecordingNoise
        ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem.Checked = UseRecordingNoise

        If UseRecordingNoise = True Then
            BackgroundSoundStatusLabel.Text = "Background sound: On"
        Else
            BackgroundSoundStatusLabel.Text = "Background sound: Off"
        End If

    End Sub

    Private Sub ToggleSoundLevelMeteronoffToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToggleSoundLevelMeteronoffToolStripMenuItem.Click
        ShowSoundLevelMeter = Not ShowSoundLevelMeter
        ToggleSoundLevelMeteronoffToolStripMenuItem.Checked = ShowSoundLevelMeter
    End Sub


    Private Sub CalibrateOutputLevelToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CalibrateOutputLevelToolStripMenuItem.Click

    End Sub



#End Region




#Region "SegmentationView"

    Private Sub SpectrogramSettingsToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles SpectrogramSettingsToolStripMenuItem1.Click
        SetSpectrogramFormatInDialog()
    End Sub


    Private Sub MoveSegmentationsToZeroCrossingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoveSegmentationsToZeroCrossingsToolStripMenuItem.Click
        'Toggling value
        SetSegmentationToZeroCrossings = Not SetSegmentationToZeroCrossings
        MoveSegmentationsToZeroCrossingsToolStripMenuItem.Checked = SetSegmentationToZeroCrossings
    End Sub

    Private Sub ShowSpectrogramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowSpectrogramToolStripMenuItem.Click
        'Toggling value
        ShowSpectrogram = Not ShowSpectrogram
        ShowSpectrogramToolStripMenuItem.Checked = ShowSpectrogram
    End Sub

    Private Sub DrawNormalizedWaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DrawNormalizedWaveToolStripMenuItem.Click
        'Toggling value
        DrawNormalizedWave = Not DrawNormalizedWave
        DrawNormalizedWaveToolStripMenuItem.Checked = DrawNormalizedWave

    End Sub

    Private Sub PaddingTimeComboBox_Click(sender As Object, e As EventArgs) Handles PaddingTimeComboBox.Click

        Dim TempValue As Single
        If Single.TryParse(PaddingTimeComboBox.SelectedText, TempValue) = True Then
            PaddingTime = TempValue
        Else
            MsgBox("Unable to set the padding time!", MsgBoxStyle.Information, "Set padding time")
        End If

    End Sub

    Private Sub InterSentenceTimeComboBox_Click(sender As Object, e As EventArgs) Handles InterSentenceTimeComboBox.Click

        Dim TempValue As Single
        If Single.TryParse(InterSentenceTimeComboBox.SelectedText, TempValue) = True Then
            InterSentenceTime = TempValue
        Else
            MsgBox("Unable to set the inter-sentence time!", MsgBoxStyle.Information, "Set inter-sentence time")
        End If

    End Sub




#End Region




    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click
        Me.Close()
    End Sub










    ''Global exception handler, could be placed in the application that imports the library, but does not work direcly in the Library
    'Sub MyGlobalExceptionHandler(sender As Object, args As UnhandledExceptionEventArgs) Handles My.MyApplication.UnhandledException
    '    Dim e As Exception = DirectCast(args.ExceptionObject, Exception)
    '    MsgBox("An unexpected error occurred (see below), and the program must close! Click OK to save any unsaved changes and then close the program." & vbCrLf & vbCrLf & e.ToString)
    '    CheckIfSaveSound()
    '    Me.Close()
    'End Sub

End Class
