Imports System.Windows.Forms
Imports System.Windows.Forms.Control


Public Class SpeechMaterialRecorder

    Private MediaSet As MediaSet = Nothing

    ''' <summary>
    ''' Contains paths to the sound files that should be recorded as Item1, and optionally a path to a prototype recording
    ''' </summary>
    Private SoundFilesForEditing As New List(Of Tuple(Of String, String))

    'Used only in the Recorder

    ''' <summary>
    ''' Contains a list of Tuples where Item1 indicates the original SMA object index of the sentence and the second Item holds the recorded sentnce sound, or Nothing if no sound has yet been recorded for the sentnce.
    ''' </summary>
    Private CurrentSentencesForRecording As New List(Of Tuple(Of Integer, Audio.Sound))
    Private CurrentSentenceIndex As Integer

    ''' <summary>
    ''' The sound channel on which to record. Could be set in the menu!
    ''' </summary>
    Private RecordingChannel As Integer = 1

    Private RandomItemOrder As Boolean = True

    Private CurrentlyLoadedSoundFile As Audio.Sound = Nothing
    Private CurrentSoundFileIndex As Integer = -1

    Private CurrentlyLoadedPrototypeRecordingSoundFile As Audio.Sound = Nothing


    Private BackgroundSound As Audio.Sound = Nothing

    'Sound player
    Private SoundPlayer As Audio.PortAudioVB.OverlappingSoundPlayer

    'Sound audio settings
    Private CurrentAudioApiSettings As Audio.AudioApiSettings = Nothing

    Private _CurrentSoundTransducerMode As Audio.SoundTransducerModes = Audio.GlobalAudioData.SoundTransducerModes.SoundField
    Private Property CurrentSoundTransducerMode As Audio.SoundTransducerModes
        Get
            Return _CurrentSoundTransducerMode
        End Get
        Set(value As Audio.SoundTransducerModes)
            _CurrentSoundTransducerMode = value
            If _CurrentSoundTransducerMode = Audio.GlobalAudioData.SoundTransducerModes.HeadPhones Then
                HeadphonesToolStripMenuItem.Checked = True
                SoundFieldToolStripMenuItem.Checked = False
            Else
                HeadphonesToolStripMenuItem.Checked = False
                SoundFieldToolStripMenuItem.Checked = True
            End If
        End Set
    End Property

    Private PresentationSound_SoundLevelFormat As Audio.Formats.SoundLevelFormat = New Audio.Formats.SoundLevelFormat(Audio.BasicAudioEnums.SoundMeasurementTypes.LoudestSection_C_Weighted, 0.05)
    Private BackgroundSound_SoundLevelFormat As Audio.Formats.SoundLevelFormat = New Audio.Formats.SoundLevelFormat(Audio.BasicAudioEnums.SoundMeasurementTypes.LoudestSection_C_Weighted, 0.05)

#Region "Recording variables"

    Private _PresentationLevel As Double  ' This level is in dB SPL
    Private Property PresentationLevel As Double
        Get
            Return _PresentationLevel
        End Get
        Set(value As Double)

            _PresentationLevel = value

            'Noting the selected level in the combobox
            For Each Item In PresentationLevel_ToolStripComboBox.Items
                If Item = _PresentationLevel Then
                    PresentationLevel_ToolStripComboBox.SelectedItem = Item
                End If
            Next

            'And displays the level in the status field
            PresentationLevelToolStripStatusLabel.Text = "Presentation level: " & _PresentationLevel & " dB SPL"

        End Set
    End Property

    Private ReMeasureBackgroundSoundLevel As Boolean = True

    Private _BackgroundSoundLevel As Double
    Private Property BackgroundSoundLevel As Double
        Get
            Return _BackgroundSoundLevel
        End Get
        Set(value As Double)

            _BackgroundSoundLevel = value

            'Noting a change in background sound level enforcing recalculation of background sound level on next presentation
            ReMeasureBackgroundSoundLevel = True

            'Noting the selected level in the combobox
            For Each Item In BackgroundSoundLevel_ToolStripComboBox.Items
                If Item = _BackgroundSoundLevel Then
                    BackgroundSoundLevel_ToolStripComboBox.SelectedItem = Item
                End If
            Next

            'And displays the level in the status field 
            BackgroundLevel_ToolStripStatusLabel.Text = "Background level: " & _BackgroundSoundLevel & " dB SPL"

        End Set
    End Property


    Private RecordingWaveFormat As Audio.Formats.WaveFormat

    Private _UseAuditoryPrequeing As Boolean = False
    Private Property UseAuditoryPrequeing As Boolean
        Get
            Return _UseAuditoryPrequeing
        End Get
        Set(value As Boolean)
            _UseAuditoryPrequeing = value
            AuditoryPrequeingToolStripMenuItem.Checked = _UseAuditoryPrequeing
            If _UseAuditoryPrequeing = True Then
                preQueLabel.Text = "Pre-queing: On"
            Else
                preQueLabel.Text = "Pre-queing: Off"
            End If
        End Set
    End Property

    Private _AutoStartRecording As Boolean = False
    Private Property AutoStartRecording As Boolean
        Get
            Return _AutoStartRecording
        End Get
        Set(value As Boolean)
            _AutoStartRecording = value
            StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem.Checked = _AutoStartRecording
            If _AutoStartRecording = True Then
                AutoRecordingStatusLabel.Text = "Auto-recording: On"
            Else
                AutoRecordingStatusLabel.Text = "Auto-recording: Off"
            End If
        End Set
    End Property

    Private _UseRecordingNoise As Boolean = False
    Private Property UseRecordingNoise As Boolean
        Get
            Return _UseRecordingNoise
        End Get
        Set(value As Boolean)
            _UseRecordingNoise = value
            ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem.Checked = _UseRecordingNoise
            If _UseRecordingNoise = True Then
                BackgroundSoundStatusLabel.Text = "Background sound: On"
            Else
                BackgroundSoundStatusLabel.Text = "Background sound: Off"
            End If
        End Set
    End Property

    Private _ShowSoundLevelMeter As Boolean = False
    Private Property ShowSoundLevelMeter As Boolean
        Get
            Return _ShowSoundLevelMeter
        End Get
        Set(value As Boolean)
            _ShowSoundLevelMeter = value
            ToggleSoundLevelMeteronoffToolStripMenuItem.Checked = _ShowSoundLevelMeter
        End Set
    End Property

#End Region


#Region "Segmentation variables"

    Private CurrentSpectrogramFormat As Audio.Formats.SpectrogramFormat
    Private SetSegmentationToZeroCrossings As Boolean
    Private ShowSpectrogram As Boolean = False

    Private _PaddingTime As Single = 0.5
    Private Property PaddingTime As Single
        Get
            Return _PaddingTime
        End Get
        Set(value As Single)

            _PaddingTime = value

            'Noting the selected level in the combobox
            For Each Item In PaddingTimeComboBox.Items
                If Item = _PaddingTime Then
                    PaddingTimeComboBox.SelectedItem = Item
                End If
            Next

        End Set
    End Property


    Private _InterSentenceTime As Single = 4
    Private Property InterSentenceTime As Single
        Get
            Return _InterSentenceTime
        End Get
        Set(value As Single)

            _InterSentenceTime = value

            'Noting the selected level in the combobox
            For Each Item In InterSentenceTimeComboBox.Items
                If Item = _InterSentenceTime Then
                    InterSentenceTimeComboBox.SelectedItem = Item
                End If
            Next

        End Set
    End Property

    Private DrawNormalizedWave As Boolean = False

#End Region


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        'Setting up a default RecordingWaveFormat
        SetupAudioIO()

        SetDefaultValues()

    End Sub

    Public Sub New(ByRef MediaSet As MediaSet,
                   ByRef EditItems As List(Of Tuple(Of String, String)),
                   Optional ByVal RandomItemOrder As Boolean = True)

        ' This call is required by the designer.
        InitializeComponent()

        Me.MediaSet = MediaSet

        Me.RandomItemOrder = RandomItemOrder
        If Me.RandomItemOrder = True Then RandomizeRecordingsOrder()

        'Setting the RecordingWaveFormat
        Me.RecordingWaveFormat = MediaSet.CreateRecordingWaveFormat

        ' Add any initialization after the InitializeComponent() call.
        Me.SoundFilesForEditing = EditItems

        SetupAudioIO()

        SetDefaultValues()

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

    Private Sub SetDefaultValues()

        PresentationLevel = 65
        BackgroundSoundLevel = 65

        CurrentSoundTransducerMode = Audio.GlobalAudioData.SoundTransducerModes.HeadPhones

        Dim AvailableInterSentenceTimes() As Single = {0.1, 0.2, 0.5, 1, 1.5, 2, 2.5, 3, 4, 5, 6, 7, 8, 9, 10, 15}
        For Each value In AvailableInterSentenceTimes
            InterSentenceTimeComboBox.Items.Add(value)
        Next

        Dim AvailablePaddingTimes() As Single = {0.1, 0.2, 0.5, 1, 1.5, 2, 2.5, 3, 4, 5, 6, 7, 8, 9, 10, 15}
        For Each value In AvailablePaddingTimes
            PaddingTimeComboBox.Items.Add(value)
        Next

        Dim AvailablePresentationLevels() As Double = {50, 55, 60, 62, 65, 68, 70, 75, 80, 85}
        For Each value In AvailablePresentationLevels
            PresentationLevel_ToolStripComboBox.Items.Add(value)
        Next

        Dim AvailableBackgroundSoundLevels() As Double = {50, 55, 60, 62, 65, 68, 70, 75, 80, 85}
        For Each value In AvailableBackgroundSoundLevels
            BackgroundSoundLevel_ToolStripComboBox.Items.Add(value)
        Next

        IsRecording = False

    End Sub

#Region "IO"

    Private Sub SpeechMaterialRecorder_FormClosing(sender As Object, e As Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        'Checks if the user wants to save the sound before closing
        CheckIfSaveSound()

        If SoundPlayer IsNot Nothing Then
            SoundPlayer.Dispose()
        End If

    End Sub

    Private Sub RandomizeRecordingsOrder()

        Dim rnd As New Random
        Dim RndList As New List(Of Tuple(Of Double, Tuple(Of String, String)))
        For Each item In Me.SoundFilesForEditing
            RndList.Add(New Tuple(Of Double, Tuple(Of String, String))(rnd.NextDouble, item))
            RndList.Sort(Function(x, y) x.Item1.CompareTo(y.Item1))
        Next
        Me.SoundFilesForEditing.Clear()
        For Each item In RndList
            Me.SoundFilesForEditing.Add(item.Item2)
        Next

    End Sub


    Private Function LoadSoundFile(ByVal FilePath As String, Optional ByVal PrototypeRecordingFilePath As String = "") As Boolean

        If IO.File.Exists(FilePath) = True Then
            Dim TempSound = Audio.Sound.LoadWaveFile(FilePath,,,,, True)
            If TempSound IsNot Nothing Then
                CheckIfSaveSound()
                CurrentlyLoadedSoundFile = TempSound
            Else
                MsgBox("Could not load the file: " & FilePath)
                Return False
            End If
        End If

        If PrototypeRecordingFilePath <> "" Then
            'Loads the prototype recording
            Dim TempSound = Audio.Sound.LoadWaveFile(PrototypeRecordingFilePath,,,,, False)
            If TempSound IsNot Nothing Then
                CurrentlyLoadedPrototypeRecordingSoundFile = TempSound
            Else
                MsgBox("Could not load the prototype recording file: " & PrototypeRecordingFilePath)
                Return False
            End If
        End If

        'Updates the loaded sound file GUI labels
        FileName_Label.Text = IO.Path.GetFileName(FilePath)
        SoundFilePathStatusLabel.Text = FilePath

        'Also loading the sentences for recording
        'Checking that the number of sentences agrees if prototype recording is used ( this could be improved by checking that all SMA levels agree
        If PrototypeRecordingFilePath <> "" Then
            If CurrentlyLoadedSoundFile.SMA.ChannelData(1).Count <> CurrentlyLoadedPrototypeRecordingSoundFile.SMA.ChannelData(1).Count Then
                MsgBox("The number of sentence level SMA components differs between recordings: " & vbCrLf & FilePath & vbCrLf & PrototypeRecordingFilePath)
                Return False
            End If
        End If

        'Original sentence order, Sentence sound
        CurrentSentencesForRecording = New List(Of Tuple(Of Integer, Audio.Sound))
        For s = 0 To CurrentlyLoadedSoundFile.SMA.ChannelData(1).Count - 1
            Dim SentenceSmaComponent = CurrentlyLoadedSoundFile.SMA.ChannelData(1)(s)
            CurrentSentencesForRecording.Add(New Tuple(Of Integer, Audio.Sound)(s, Nothing))
        Next

        'Randomizing the order of sentences
        If RandomItemOrder = True Then
            RandomizeSentenceOrder()
        End If

        Return True

    End Function


    ''' <summary>
    ''' Stores a user selected sound file path in the SoundFilesForEditing object
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadWaveFileSMAIXMLChunkRequiredToolStripMenuItem.Click

        Dim FilePath As String = Utils.GetOpenFilePath("",, {".wav"}, "Open wave file (SMA iXML chunk required)", True)
        If FilePath <> "" Then
            SoundFilesForEditing.Add(New Tuple(Of String, String)(FilePath, Nothing))
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
                    Else
                        'Setting IsChanged manually to Nothing, to inactivate the overriding of IsChanged
                        CurrentlyLoadedSoundFile.SetIsChangedManually(Nothing)
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

        If SoundPlayer IsNot Nothing Then
            SoundPlayer.Dispose()
        End If

        Dim TemporaryOutputSound As Audio.Sound = Audio.GenerateSound.CreateSilence(RecordingWaveFormat,, 1)
        Dim PlayerMode As Audio.PortAudioVB.OverlappingSoundPlayer.SoundDirections

        If CurrentAudioApiSettings.NumberOfInputChannels.HasValue = True And CurrentAudioApiSettings.NumberOfOutputChannels.HasValue = True Then
            If CurrentAudioApiSettings.NumberOfInputChannels = 0 And CurrentAudioApiSettings.NumberOfOutputChannels = 0 Then
                MsgBox("This software requires at least an output device sound to work.")
                Exit Sub
            ElseIf CurrentAudioApiSettings.NumberOfInputChannels = 0 Then
                MsgBox("The selected sound device has no input channels! You will not be able to record any sound!")
                PlayerMode = Audio.PortAudioVB.OverlappingSoundPlayer.SoundDirections.PlaybackOnly
            ElseIf CurrentAudioApiSettings.NumberOfOutputChannels = 0 Then
                MsgBox("The selected sound device has no output channels! You will not be able to hear any sound!")
                PlayerMode = Audio.PortAudioVB.OverlappingSoundPlayer.SoundDirections.RecordingOnly
            Else
                'This is the preferred path! Both input and output channels present!
                PlayerMode = Audio.PortAudioVB.OverlappingSoundPlayer.SoundDirections.Duplex
            End If
        ElseIf CurrentAudioApiSettings.NumberOfInputChannels.HasValue = True Then
            If CurrentAudioApiSettings.NumberOfInputChannels = 0 Then
                MsgBox("The selected sound device has no input channels! You will not be able to record any sound!")
                PlayerMode = Audio.PortAudioVB.OverlappingSoundPlayer.SoundDirections.PlaybackOnly
            End If
        ElseIf CurrentAudioApiSettings.NumberOfOutputChannels.HasValue = True Then
            If CurrentAudioApiSettings.NumberOfOutputChannels = 0 Then
                MsgBox("The selected sound device has no output channels! You will not be able to hear any sound!")
                PlayerMode = Audio.PortAudioVB.OverlappingSoundPlayer.SoundDirections.RecordingOnly
            End If
        Else
            MsgBox("No sound device has been selected. This software requires at least an output device sound to work.")
            Exit Sub
        End If


        SoundPlayer = New Audio.PortAudioVB.OverlappingSoundPlayer(Nothing, PlayerMode, CurrentAudioApiSettings,
                                                                            TemporaryOutputSound.WaveFormat.Encoding, False, False, False, False, 0.1)

        SoundPlayer.OpenStream()
        SoundPlayer.Start()

        'Dim TempPlayer = New Audio.PortAudioVB.SoundPlayer(False, RecordingWaveFormat, TemporaryOutputSound, CurrentAudioApiSettings, False, False, False, True, True)
        'SoundPlayer = New Audio.PortAudioVB.SoundPlayer(False, RecordingWaveFormat, TemporaryOutputSound, CurrentAudioApiSettings,
        '                                                                 False, False, False, True, True)

    End Sub

    Public Sub SetSpectrogramFormatInDialog()
        Dim SpectrogramSettingsResult As New SpectrogramSettingsDialog
        If SpectrogramSettingsResult.ShowDialog = Windows.Forms.DialogResult.OK Then
            CurrentSpectrogramFormat = SpectrogramSettingsResult.NewSpectrogramFormat
        Else
            CurrentSpectrogramFormat = New Audio.Formats.SpectrogramFormat(, 1024,, 512,, True,,,, True)
        End If
    End Sub

    Private Sub HeadphonesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HeadphonesToolStripMenuItem.Click
        CurrentSoundTransducerMode = Audio.GlobalAudioData.SoundTransducerModes.HeadPhones
    End Sub

    Private Sub SoundFieldToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SoundFieldToolStripMenuItem.Click
        CurrentSoundTransducerMode = Audio.GlobalAudioData.SoundTransducerModes.SoundField
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

                    CurrentSentenceIndex = -1
                    JumpToUnrecorded = False
                    ChangeSentenceDirection = IndexChangeDirections.Next

                    'Resetting CurrentSentenceIndex 
                    SelectNextSentenceForRecording()

                Else

                    RecordingTabMainSplitContainer.Panel2.Controls.Clear()
                    MsgBox("Cannot load the sound file: " & SoundPath)

                End If

            Else
                RecordingTabMainSplitContainer.Panel2.Controls.Clear()
            End If

        Catch ex As Exception
            MsgBox("The following exception occurred: " & ex.ToString)
        End Try

    End Sub



    Public Function MinSoundSentenceSelectionIndex() As Integer
        If CurrentSentencesForRecording Is Nothing Then
            Return -1
        Else
            Return 0
        End If
    End Function

    Public Function MaxSoundSentenceSelectionIndex() As Integer
        If CurrentSentencesForRecording Is Nothing Then
            Return -1
        Else
            Return CurrentSentencesForRecording.Count - 1
        End If
    End Function


    ''' <summary>
    ''' Selects the next sentnce for recording based on the values in the variables ChangeSentenceDirection and JumpToUnrecorded. Returns true if selection found an appropriate index or false if no index could be found (e.g. end of list etc.)
    ''' </summary>
    ''' <returns></returns>
    Private Function SelectNextSentenceForRecording() As Boolean

        If CurrentSentencesForRecording Is Nothing Then
            MsgBox("No SMA sentence level objects have been loaded from wave file!")
            Return False
        End If

        If CurrentSentencesForRecording.Count = 0 Then
            MsgBox("No sentence level SMA object exist in the loaded sound file!")
            Return False
        End If

        Dim TempIndex As Integer = CurrentSentenceIndex

        If JumpToUnrecorded = False Then

            'Simply jumps to the next or previous sentence
            Select Case ChangeSentenceDirection
                Case IndexChangeDirections.Next
                    TempIndex += 1
                Case IndexChangeDirections.Previous
                    TempIndex -= 1
            End Select

        Else

            Dim AnyIndexFound As Boolean = False
            Select Case ChangeSentenceDirection
                Case IndexChangeDirections.Next

                    For i = TempIndex To CurrentSentencesForRecording.Count - 1
                        If CurrentSentencesForRecording(i).Item2 Is Nothing Then
                            TempIndex = i
                            AnyIndexFound = True
                            Exit For
                        End If
                    Next

                Case IndexChangeDirections.Previous
                    For i = TempIndex To 0 Step -1
                        If CurrentSentencesForRecording(i).Item2 Is Nothing Then
                            TempIndex = i
                            AnyIndexFound = True
                            Exit For
                        End If
                    Next

            End Select

            If AnyIndexFound = False Then

                'Searches through all sentences
                For i = 0 To CurrentSentencesForRecording.Count - 1
                    If CurrentSentencesForRecording(i).Item2 Is Nothing Then
                        TempIndex = i
                        AnyIndexFound = True
                        Exit For
                    End If
                Next

                If AnyIndexFound = False Then
                    MsgBox("No more unrecorded items/sentences can be found.")
                    Return False
                End If
            End If
        End If


        'Checking TempIndex 
        Select Case TempIndex
            Case < MinSoundSentenceSelectionIndex()

                MsgBox("Selected sentence index too low (you've already at the first sentence)")
                Return False

            Case > MaxSoundSentenceSelectionIndex()

                MsgBox("Selected sentence file index too high (you've already at the last sentence)")
                Return False

            Case Else
                CurrentSentenceIndex = TempIndex
                Return True

        End Select

        If AutoStartRecording = False Then
            ViewSentenceForRecording()
        Else
            If CurrentSentencesForRecording(CurrentSentenceIndex).Item2 IsNot Nothing Then
                'Not starting recording automatically if a recoring already exists
                ViewSentenceForRecording()
            Else
                'Starts the recoring automatically if no recordings already exists
                StartNewRecording()
            End If
        End If

    End Function

    Private Sub ViewSentenceForRecording()

        Dim HasSound As Boolean = False

        If CurrentSentencesForRecording(CurrentSentenceIndex).Item2 IsNot Nothing Then

            If CurrentSentencesForRecording(CurrentSentenceIndex).Item2.WaveData.SampleData(RecordingChannel).Length > 0 Then

                'Resetting sound display
                If RecordingTabMainSplitContainer.Panel2.Controls.Count > 0 Then RecordingTabMainSplitContainer.Panel2.Controls.RemoveAt(0)

                Dim waveDrawer As New Audio.Graphics.SoundEditor(CurrentSentencesForRecording(CurrentSentenceIndex).Item2,,,,,,,,,, SoundPlayer, CurrentAudioApiSettings)
                waveDrawer.Dock = Windows.Forms.DockStyle.Fill

                RecordingTabMainSplitContainer.Panel2.Controls.Add(waveDrawer)

                HasSound = True

            End If
        End If

        If HasSound = False Then
            'Resets the sound display, and adds a message that no sound is recorded
            If RecordingTabMainSplitContainer.Panel2.Controls.Count > 0 Then RecordingTabMainSplitContainer.Panel2.Controls.RemoveAt(0)

            Dim noSoundLabel As New Windows.Forms.Label
            noSoundLabel.Dock = Windows.Forms.DockStyle.Fill
            noSoundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            noSoundLabel.Text = "No sound is yet recorded for this item/sentence."
            RecordingTabMainSplitContainer.Panel2.Controls.Add(noSoundLabel)
        End If

    End Sub

    Private Sub RandomizeSentenceOrder()

        If CurrentSentencesForRecording IsNot Nothing Then
            Dim rnd As New Random
            Dim RndList As New List(Of Tuple(Of Double, Tuple(Of Integer, Audio.Sound)))
            For Each item In CurrentSentencesForRecording
                RndList.Add(New Tuple(Of Double, Tuple(Of Integer, Audio.Sound))(rnd.NextDouble, item))
                RndList.Sort(Function(x, y) x.Item1.CompareTo(y.Item1))
            Next
            CurrentSentencesForRecording.Clear()
            For Each item In RndList
                CurrentSentencesForRecording.Add(item.Item2)
            Next
        End If

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
                        Dim waveDrawer As New Audio.Graphics.SoundEditor(CurrentlyLoadedSoundFile,,,, True, ShowSpectrogram, CurrentSpectrogramFormat, PaddingTime,
                                                                         InterSentenceTime, DrawNormalizedWave, SoundPlayer, CurrentAudioApiSettings, SetSegmentationToZeroCrossings)

                        'Dim waveDrawer As New Audio.Graphics.SoundEditor(CurrentlyLoadedSoundFile,,,, True, ShowSpectrogram, CurrentSpectrogramFormat, PaddingTime,
                        '                                                 InterSentenceTime, DrawNormalizedWave, SoundPlayer, SetSegmentationToZeroCrossings)

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

    Private Sub PresentationLevel_ToolStripComboBox_Click(sender As Object, e As EventArgs) Handles PresentationLevel_ToolStripComboBox.SelectedIndexChanged

        If PresentationLevel_ToolStripComboBox.SelectedItem = Nothing Then Exit Sub

        Dim TempValue As Single
        If Double.TryParse(PresentationLevel_ToolStripComboBox.SelectedItem, TempValue) = True Then
            PresentationLevel = TempValue
        Else
            MsgBox("Unable to set the presentation level!", MsgBoxStyle.Information, "Set presentation level")
        End If

        PresentationLevel_ToolStripComboBox.PerformClick()

    End Sub

    Private Sub BackgroundSoundLevel_ToolStripComboBox_Click(sender As Object, e As EventArgs) Handles BackgroundSoundLevel_ToolStripComboBox.SelectedIndexChanged

        If BackgroundSoundLevel_ToolStripComboBox.SelectedItem = Nothing Then Exit Sub

        Dim TempValue As Single
        If Double.TryParse(BackgroundSoundLevel_ToolStripComboBox.SelectedItem, TempValue) = True Then
            BackgroundSoundLevel = TempValue
        Else
            MsgBox("Unable to set the background sound level!", MsgBoxStyle.Information, "Set background sound level")
        End If

        BackgroundSoundLevel_ToolStripComboBox.PerformClick()

    End Sub

    Private Sub PresentationSoundLevelTypeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PresentationSoundLevelTypeToolStripMenuItem.Click
        PresentationSound_SoundLevelFormat.SelectFormatWithGui()
    End Sub

    Private Sub BackgroundSoundLevelTypeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BackgroundSoundLevelTypeToolStripMenuItem.Click
        BackgroundSound_SoundLevelFormat.SelectFormatWithGui()
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


    Private Sub ListenButton_Click(sender As Object, e As EventArgs) Handles ListenButton.Click

        If CurrentlyLoadedSoundFile IsNot Nothing Then

            'TODO Set level!

            'Audio.Convert_dBSPL_To_dBFS(PresentationLevel)

            SoundPlayer.SwapOutputSounds(CurrentlyLoadedSoundFile)

        End If

    End Sub



#Region "Recording Loop"

    Private delayBeforeStoppingRecording As Integer = 500

    Private Property _IsRecording As Boolean = False
    Private Property IsRecording As Boolean
        Get
            Return _IsRecording
        End Get
        Set(value As Boolean)

            _IsRecording = value

            If IsRecording = False Then

                RecordingLabel.ForeColor = Drawing.Color.Blue
                RecordingLabel.BackColor = Drawing.Color.DarkGray
                RecordingLabel.Text = "Not recording"


                MainTabControl.TabPages(1).Enabled = True
                MenuStrip1.Enabled = True

                StartRecordingButton.Enabled = True
                Rec_NextItemButton.Enabled = True
                Rec_PreviousItemButton.Enabled = True
                Rec_NextNRItemButton.Enabled = True
                Rec_PreviousNRItemButton.Enabled = True
                StopRecordingButton.Enabled = False

                Top_PreviousFileButton.Enabled = True
                Top_NextFileButton.Enabled = True
                FileComboBox.Enabled = True
            Else

                RecordingLabel.ForeColor = Drawing.Color.White
                RecordingLabel.BackColor = Drawing.Color.Red
                RecordingLabel.Text = "Recording"
                ListenButton.Enabled = False


                MainTabControl.TabPages(1).Enabled = False
                MenuStrip1.Enabled = False

                StartRecordingButton.Enabled = False
                Rec_NextItemButton.Enabled = True
                Rec_PreviousItemButton.Enabled = True
                Rec_NextNRItemButton.Enabled = True
                Rec_PreviousNRItemButton.Enabled = True
                StopRecordingButton.Enabled = True

                Top_PreviousFileButton.Enabled = False
                Top_NextFileButton.Enabled = False
                FileComboBox.Enabled = False

            End If

            Me.Refresh()

        End Set
    End Property

    Private Enum RecordingStatus
        NotRecording
        Recording
        FinishedRecording
    End Enum


    Private WithEvents StartRecordingTimer As New Timer
    Private Sub StartNewRecording()

        'Plays the specified sound
        If UseAuditoryPrequeing = True Then

            Dim PrototypeSound As Audio.Sound = CurrentlyLoadedPrototypeRecordingSoundFile.SMA.ChannelData(1)(CurrentSentencesForRecording(CurrentSentenceIndex).Item1).GetSoundFileSection(1)

            'Presenting a mono sound

            'TODO: Set level!
            SoundPlayer.SwapOutputSounds(PrototypeSound)

            StartRecordingTimer.Interval = 50 + (1000 * PrototypeSound.SMA.ChannelData(1)(CurrentSentenceIndex).Length / PrototypeSound.WaveFormat.SampleRate)
            StartRecordingTimer.Start()

        Else

            StartRecordingTimer.Interval = 1
            StartRecordingTimer.Start()
        End If

    End Sub


    Private Sub StartRecording() Handles StartRecordingTimer.Tick

        StartRecordingTimer.Stop()

        Dim rnd As New Random

        'Prepares the background noise
        If UseRecordingNoise = True Then

            If BackgroundSound Is Nothing Then
                Dim BoxResult = MsgBox("Please press OK select a background sound file.", MsgBoxStyle.OkCancel)

                If BoxResult = MsgBoxResult.Ok Then
                    BackgroundSound = Audio.AudioIOs.ReadWaveFile(,,,,,)

                    'Checking that the length is enough
                    Dim BackgroundSoundLength As Integer = BackgroundSound.WaveData.SampleData(1).Length

                    'It should be at least 100 seconds
                    If BackgroundSoundLength / BackgroundSound.WaveFormat.SampleRate < 100 Then
                        MsgBox("The background sound should be at least 100 seconds long!")
                        Exit Sub
                    End If

                    If BackgroundSound.WaveFormat.Channels = 1 Then
                        'Creating a stereo sound with uncorrelated channels by copying from random sections 
                        Dim NewBackgroundSound As New Audio.Sound(New Audio.Formats.WaveFormat(BackgroundSound.WaveFormat.SampleRate, BackgroundSound.WaveFormat.BitDepth, 2, , BackgroundSound.WaveFormat.Encoding))
                        Dim Chl1Rnd = rnd.Next(0, BackgroundSoundLength / 4)
                        Dim Chl2Rnd = rnd.Next((1 * BackgroundSoundLength) / 4, (3 * BackgroundSoundLength) / 4)
                        Dim FinalLength = (BackgroundSoundLength / 4) - 50
                        NewBackgroundSound.WaveData.SampleData(1) = BackgroundSound.WaveData.SampleData(1).ToList.GetRange(Chl1Rnd, FinalLength).ToArray
                        NewBackgroundSound.WaveData.SampleData(2) = BackgroundSound.WaveData.SampleData(1).ToList.GetRange(Chl2Rnd, FinalLength).ToArray
                        BackgroundSound = NewBackgroundSound
                    End If
                Else
                    Exit Sub
                End If
            Else

                If ReMeasureBackgroundSoundLevel = True Then

                    For c = 1 To 2

                        'Setting the background sound level
                        'Measures weighted level
                        Dim PreLevel As Double
                        If BackgroundSound_SoundLevelFormat.LoudestSectionMeasurement = True Then
                            PreLevel = Audio.DSP.GetLevelOfLoudestWindow(BackgroundSound, c,
                                                                                     BackgroundSound_SoundLevelFormat.TemporalIntegrationDuration * BackgroundSound.WaveFormat.SampleRate,
                                                                                      0, Nothing, , BackgroundSound_SoundLevelFormat.FrequencyWeighting, True)
                        Else
                            PreLevel = Audio.DSP.MeasureSectionLevel(BackgroundSound, c, 0, Nothing, Audio.AudioManagement.SoundDataUnit.dB, Audio.AudioManagement.SoundMeasurementType.RMS, BackgroundSound_SoundLevelFormat.FrequencyWeighting)
                        End If

                        'Adjusting the level
                        Audio.DSP.AmplifySection(BackgroundSound, PreLevel - BackgroundSoundLevel, c)
                    Next

                    ReMeasureBackgroundSoundLevel = False
                End If

                'Taking a random section of the sound
                Dim MaskerDuration As Single = 15
                Dim MaskerLength As Integer = MaskerDuration * BackgroundSound.WaveFormat.SampleRate

                Dim Chl1_2Rnd = rnd.Next(0, BackgroundSound.WaveData.SampleData(1).Length - MaskerLength - 50)
                Dim CurrentMasker As New Audio.Sound(BackgroundSound.WaveFormat)
                CurrentMasker.WaveData.SampleData(1) = BackgroundSound.WaveData.SampleData(1).ToList.GetRange(Chl1_2Rnd, MaskerLength).ToArray
                CurrentMasker.WaveData.SampleData(2) = BackgroundSound.WaveData.SampleData(2).ToList.GetRange(Chl1_2Rnd, MaskerLength).ToArray

                'Fading in the masker
                Audio.DSP.Fade(CurrentMasker,, 1,,, 2000)

            End If
        End If

        Try

            IsRecording = True

            'Starts the recording
            If UseRecordingNoise = True Then

                'Starting to record with background sound
                SoundPlayer.SwapOutputSounds(BackgroundSound, True)

            Else
                'Starting to record without background sound
                SoundPlayer.SwapOutputSounds(Nothing, True)

            End If


        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub


    Private Sub StopRecording()

        If IsRecording = True Then

            'Stopping player
            SoundPlayer.FadeOutPlayback()

            IsRecording = False

            'Copying the sound from the recorded sound
            Dim RecordedSound = SoundPlayer.GetRecordedSound(True)
            If RecordedSound IsNot Nothing Then

                Dim RecordedMonoSound = New Audio.Sound(RecordingWaveFormat)
                RecordedMonoSound.WaveData.SampleData(1) = RecordedSound.WaveData.SampleData(RecordingChannel)

                CurrentSentencesForRecording(CurrentSentenceIndex) = New Tuple(Of Integer, Audio.Sound)(CurrentSentencesForRecording(CurrentSentenceIndex).Item1, RecordedMonoSound)

                'Storing sounds recorded in CurrentSentencesForRecording in CurrentlyLoadedSoundFile in the correct order
                Dim SortedSoundsList As New SortedList(Of Integer, Audio.Sound)
                For Each sound In CurrentSentencesForRecording
                    SortedSoundsList.Add(sound.Item1, sound.Item2)
                Next
                Dim SoundsList As List(Of Audio.Sound) = SortedSoundsList.Values.ToList
                Dim CurrentlyRecordedSentences As Audio.Sound = Audio.DSP.ConcatenateSounds(SoundsList)
                CurrentlyLoadedSoundFile = CurrentlyRecordedSentences
                CurrentlyLoadedSoundFile.SetIsChangedManually(True)

            Else
                MsgBox("Unable to retrieve any recorded sound data.")
            End If

        End If

    End Sub



    Private WithEvents NextItemTimer As New Windows.Forms.Timer

    Private Sub StartRecordingButton_Click(sender As Object, e As EventArgs) Handles StartRecordingButton.Click
        If IsRecording = False Then
            StartNewRecording()
        End If
    End Sub

    Private ChangeSentenceDirection As IndexChangeDirections
    Private Enum IndexChangeDirections
        Previous
        [Next]
    End Enum

    Private JumpToUnrecorded As Boolean = False

    Private Sub Rec_NextItemButton_Click(sender As Object, e As EventArgs) Handles Rec_NextItemButton.Click

        ChangeSentenceDirection = IndexChangeDirections.Next
        JumpToUnrecorded = False

        If IsRecording = True Then
            NextItemTimer.Interval = delayBeforeStoppingRecording
            NextItemTimer.Start()
        Else
            SelectNextSentenceForRecording()
        End If

    End Sub

    Private Sub Rec_PreviousItemButton_Click(sender As Object, e As EventArgs) Handles Rec_PreviousItemButton.Click

        ChangeSentenceDirection = IndexChangeDirections.Previous
        JumpToUnrecorded = False

        If IsRecording = True Then
            NextItemTimer.Interval = delayBeforeStoppingRecording
            NextItemTimer.Start()
        Else
            SelectNextSentenceForRecording()
        End If

    End Sub

    Private Sub Rec_NextNRItemButton_Click(sender As Object, e As EventArgs) Handles Rec_NextNRItemButton.Click

        ChangeSentenceDirection = IndexChangeDirections.Next
        JumpToUnrecorded = True

        If IsRecording = True Then
            NextItemTimer.Interval = delayBeforeStoppingRecording
            NextItemTimer.Start()
        Else
            SelectNextSentenceForRecording()
        End If

    End Sub

    Private Sub Rec_PreviousNRItemButton_Click(sender As Object, e As EventArgs) Handles Rec_PreviousNRItemButton.Click

        ChangeSentenceDirection = IndexChangeDirections.Previous
        JumpToUnrecorded = True

        If IsRecording = True Then
            NextItemTimer.Interval = delayBeforeStoppingRecording
            NextItemTimer.Start()
        Else
            SelectNextSentenceForRecording()
        End If

    End Sub

    Private Sub NextItemTimer_Tick() Handles NextItemTimer.Tick

        If IsRecording = True Then
            NextItemTimer.Stop()
            StopRecording()
        End If

        'Select next Item
        SelectNextSentenceForRecording()

    End Sub

    Private Sub StopRecordingButton_Click(sender As Object, e As EventArgs) Handles StopRecordingButton.Click

        If IsRecording = True Then
            StopRecording()

            ViewSentenceForRecording()

        End If

    End Sub


#End Region


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
    End Sub

    Private Sub StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartRecordingAutomaticallyOnNextpreviousToolStripMenuItem.Click
        AutoStartRecording = Not AutoStartRecording
    End Sub

    Private Sub ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToggleBackgroundSoundWhileRecordingonoffToolStripMenuItem.Click
        UseRecordingNoise = Not UseRecordingNoise
    End Sub

    Private Sub ToggleSoundLevelMeteronoffToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToggleSoundLevelMeteronoffToolStripMenuItem.Click
        ShowSoundLevelMeter = Not ShowSoundLevelMeter
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

    Private Sub PaddingTimeComboBox_Click(sender As Object, e As EventArgs) Handles PaddingTimeComboBox.SelectedIndexChanged

        If PaddingTimeComboBox.SelectedItem = Nothing Then Exit Sub

        Dim TempValue As Single
        If Single.TryParse(PaddingTimeComboBox.SelectedItem, TempValue) = True Then
            PaddingTime = TempValue
        Else
            MsgBox("Unable to set the padding time!", MsgBoxStyle.Information, "Set padding time")
        End If

        PaddingTimeComboBox.PerformClick()

    End Sub

    Private Sub InterSentenceTimeComboBox_Click(sender As Object, e As EventArgs) Handles InterSentenceTimeComboBox.SelectedIndexChanged

        If InterSentenceTimeComboBox.SelectedItem = Nothing Then Exit Sub

        Dim TempValue As Single
        If Single.TryParse(InterSentenceTimeComboBox.SelectedItem, TempValue) = True Then
            InterSentenceTime = TempValue
        Else
            MsgBox("Unable to set the inter-sentence time!", MsgBoxStyle.Information, "Set inter-sentence time")
        End If

        InterSentenceTimeComboBox.PerformClick()

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
