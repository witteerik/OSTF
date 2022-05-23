''This software is available under the following license:
''MIT/X11 License
''
''Copyright (c) 2017 Erik Witte
''
''Permission is hereby granted, free of charge, to any person obtaining a copy
''of this software and associated documentation files (the ''Software''), to deal
''in the Software without restriction, including without limitation the rights
''to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
''copies of the Software, and to permit persons to whom the Software is
''furnished to do so, subject to the following conditions:
''
''The above copyright notice and this permission notice shall be included in all
''copies or substantial portions of the Software.
''
''THE SOFTWARE IS PROVIDED ''AS IS'', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
''IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
''FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
''AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
''LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
''OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
''SOFTWARE.

'Imports System.Windows.Forms
'Imports System.Drawing


'Namespace Audio

'    Namespace Graphics

'        Public Class SoundEditor
'            Inherits SplitContainer

'            'Sound panels, etc

'            Friend WithEvents SoundContainerPanel As New TableLayoutPanel
'            Friend WithEvents SoundBackgroundArea As New System.Windows.Forms.PictureBox
'            Friend WithEvents SpectrogramArea As New System.Windows.Forms.PictureBox
'            Friend WithEvents WaveArea As New System.Windows.Forms.PictureBox
'            Friend WithEvents TimeArea As New System.Windows.Forms.PictureBox

'            Private WithEvents SoundScrollBar As New HScrollBar
'            Friend WithEvents SegmentationItemsPanel As New FlowLayoutPanel

'            'Other panels, etc
'            Private ChangeWordButtonTexts As New List(Of String) From {"Previous Word", "Next Word"} ' Button text
'            Private WithEvents SoundDisplayContextMenu As New ContextMenuStrip

'            'Panel settings
'            Private DisplayTypeCount As Integer = 1

'            'Variables for which part of the sound that should be displayed and selected/highlighted
'            Private TimeUnit As TimeUnits = TimeUnits.seconds

'            Private SelectionStart_Sample As Integer
'            Private SelectionLength_Sample As Integer
'            Private SelectionStart_Pixel As Single
'            Private SelectionLength_Pixel As Single
'            Public Property DisplayStart_Sample As Integer
'            Public Property DisplayLength_Samples As Integer

'            Public Function GetSelectionStartSample() As Integer
'                Return SelectionStart_Sample
'            End Function

'            Public Function GetselectionLength() As Integer
'                Return SelectionLength_Sample
'            End Function

'            'Graphic settings
'            Private GraphicUpdateFrequency As Integer = 10

'            'Sound data
'            Private CurrentSound As Sound
'            Private SoundBackUp As Sound
'            Private FS_pos As Double
'            Private CurrentChannel As Integer
'            Private SampleToPixelScale As Single
'            Private WavePointsArray(1, 0) As Single 'used for long sounds, holding the max and min values of for a section of the wave file, in order to draw a vertical line, one pixel wide, between them, repressenting the wave form.
'            Private WaveLinePointArray() As System.Drawing.PointF = {} 'used for short sounds
'            Private NormalizedWavePointsArray(1, 0) As Single 'used for long sounds
'            Private NormalizedWaveLinePointArray() As System.Drawing.PointF = {} 'used for short sounds
'            Private SelectionCopy() As Single

'            'Wave settings
'            Private DrawEverySampleLimitFactor As Integer = 10 '    2000  'TODO: One of the update functions is not synchonized!!! 'This is a factor that sets how many more samples than pixels there should be on the screen in order to swap wave drawing technique
'            Private DrawNormalizedWave As Boolean = False 'This determines if the normalized wave sound be drawn in the background

'            'Spectrogram data
'            Private ShowSpectrogram As Boolean
'            Private SpectrogramFormat As Formats.SpectrogramFormat
'            Private SpectrogramWindowDistance As Integer
'            Private SpectrogramDisplayDataArray As SpectrogramDisplayData()
'            Private SpectrogramLightFactor As Single = 1
'            Friend WithEvents SpectrogramLightFactorScrollBar As New VScrollBar

'            'Variables for assigning phoneme data
'            Private UseItemSegmentation As Boolean
'            'Dim currentSmaData As Sound.SpeechMaterialAnnotation ' List(Of Sound.ptwfData.SmaWordData)
'            Private CurrentSentenceIndex As Integer = 0 'This holds the index of the sentence in each sound.
'            Private CurrentWordIndex As Integer = 0
'            Private CurrentPhonemeIndex As Integer
'            Private PaddingTime As Single        'padding time should be in seconds
'            Private SetSegmentationToZeroCrossings As Boolean

'            'PaSoundPLayer
'            Private SoundPlayer As PortAudioVB.SoundPlayer

'            'Buttons
'            Public ShowDetectBoundariesButton As Boolean
'            Public ShowUpdateSegmentationButton As Boolean
'            Public ShowFadePaddingButton As Boolean

'            Public Sub New()

'                Me.New(False)

'            End Sub


'            'Setting things up
'            Public Sub New(Optional ByVal UseItemSegmentation As Boolean = False, Optional ByVal ShowSpectrogram As Boolean = False,
'                    Optional ByRef SpectrogramFormat As Formats.SpectrogramFormat = Nothing, Optional ByVal PaddingTime As Single = 0,
'                    Optional ByVal DrawNormalizedWave As Boolean = False, Optional ByRef SoundPlayer As PortAudioVB.SoundPlayer = Nothing,
'                    Optional ByVal SetSegmentationToZeroCrossings As Boolean = True, Optional ByVal ShowDetectBoundariesButton As Boolean = True,
'                    Optional ByVal ShowUpdateSegmentationButton As Boolean = True, Optional ByVal ShowFadePaddingButton As Boolean = True)

'                Me.UseItemSegmentation = UseItemSegmentation
'                Me.ShowSpectrogram = ShowSpectrogram
'                Me.SpectrogramFormat = SpectrogramFormat
'                Me.PaddingTime = PaddingTime
'                Me.DrawNormalizedWave = DrawNormalizedWave
'                Me.SoundPlayer = SoundPlayer
'                Me.SetSegmentationToZeroCrossings = SetSegmentationToZeroCrossings
'                Me.ShowDetectBoundariesButton = ShowDetectBoundariesButton
'                Me.ShowUpdateSegmentationButton = ShowUpdateSegmentationButton
'                Me.ShowFadePaddingButton = ShowFadePaddingButton


'                'Setting some default sound data
'                CurrentSound = Audio.Sound.GetTestSound()

'                'Creating a temporary setting
'                If Me.ShowSpectrogram = True Then

'                    'Creating a default spectrogram format if needed
'                    If SpectrogramFormat Is Nothing Then
'                        SpectrogramFormat = New Formats.SpectrogramFormat(,,,,,,,,, True)
'                    End If

'                    SpectrogramWindowDistance = SpectrogramFormat.SpectrogramFftFormat.AnalysisWindowSize - SpectrogramFormat.SpectrogramFftFormat.OverlapSize

'                End If


'                'Setting up controls
'                Me.Orientation = Orientation.Horizontal
'                'Me.IsSplitterFixed = True

'                Me.Panel1.Controls.Add(SoundContainerPanel)
'                SoundContainerPanel.Dock = DockStyle.Fill
'                SoundContainerPanel.GrowStyle = Windows.Forms.TableLayoutPanelGrowStyle.FixedSize

'                If UseItemSegmentation = True Then

'                    SoundContainerPanel.ColumnCount = 2

'                    SoundContainerPanel.ColumnStyles.Clear()
'                    SoundContainerPanel.ColumnStyles.Add(New Windows.Forms.ColumnStyle(Windows.Forms.SizeType.Percent, 100))
'                    SoundContainerPanel.ColumnStyles.Add(New Windows.Forms.ColumnStyle(Windows.Forms.SizeType.Absolute, 10))

'                    SoundContainerPanel.RowCount = 4
'                    SoundContainerPanel.RowStyles.Clear()
'                    SoundContainerPanel.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Percent, 50))
'                    SoundContainerPanel.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Percent, 50))
'                    SoundContainerPanel.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Absolute, 20))
'                    SoundContainerPanel.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Absolute, 10))

'                Else

'                    SoundContainerPanel.ColumnCount = 1

'                    SoundContainerPanel.ColumnStyles.Clear()
'                    SoundContainerPanel.ColumnStyles.Add(New Windows.Forms.ColumnStyle(Windows.Forms.SizeType.Percent, 100))

'                    SoundContainerPanel.RowCount = 3
'                    SoundContainerPanel.RowStyles.Clear()
'                    SoundContainerPanel.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Percent, 50))
'                    SoundContainerPanel.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Absolute, 20))
'                    SoundContainerPanel.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Absolute, 10))

'                End If

'                'Adding sound display controls and time scale
'                Dim AddRowIndex As Integer = 0
'                If UseItemSegmentation = True Then

'                    'Adds the spectrogram at the first column
'                    SoundContainerPanel.Controls.Add(SpectrogramArea, 0, AddRowIndex)
'                    SpectrogramArea.Dock = DockStyle.Fill
'                    AddRowIndex += 1

'                    'And the scroll bar in the second column
'                    SoundContainerPanel.Controls.Add(SpectrogramLightFactorScrollBar, 1, AddRowIndex)
'                    SpectrogramLightFactorScrollBar.Dock = DockStyle.Fill
'                    AddRowIndex += 1

'                    'Setting the value of the spectrogram light bar
'                    SpectrogramLightFactorScrollBar.Minimum = 0
'                    SpectrogramLightFactorScrollBar.Maximum = 100
'                    SpectrogramLightFactorScrollBar.Value = SpectrogramLightFactorScrollBar.Maximum - 20 - SpectrogramLightFactor

'                End If

'                SoundContainerPanel.Controls.Add(WaveArea, 0, AddRowIndex)
'                AddRowIndex += 1
'                SoundContainerPanel.Controls.Add(TimeArea, 0, AddRowIndex)
'                AddRowIndex += 1
'                SoundContainerPanel.Controls.Add(SoundScrollBar, 0, AddRowIndex)
'                AddRowIndex += 1

'                'Docks controls
'                WaveArea.Dock = DockStyle.Fill
'                TimeArea.Dock = DockStyle.Fill
'                SoundScrollBar.Dock = DockStyle.Fill

'                'Expands controls to both table columns
'                'SoundContainerPanel.SetColumnSpan(WaveArea, 2)
'                'SoundContainerPanel.SetColumnSpan(TimeArea, 2)
'                'SoundContainerPanel.SetColumnSpan(SoundScrollBar, 2)

'                'Adding borders to the sound boxes
'                TimeArea.BorderStyle = BorderStyle.FixedSingle
'                WaveArea.BorderStyle = BorderStyle.FixedSingle
'                SpectrogramArea.BorderStyle = BorderStyle.FixedSingle
'                SoundBackgroundArea.BorderStyle = BorderStyle.Fixed3D


'                'Loading phoneme data
'                If Me.UseItemSegmentation = True Then
'                    Me.Panel2Collapsed = False
'                    Me.Panel2MinSize = 30
'                    SegmentationItemsPanel.FlowDirection = FlowDirection.LeftToRight
'                    SegmentationItemsPanel.Dock = DockStyle.Fill
'                    SegmentationItemsPanel.AutoScroll = True
'                Else
'                    Me.Panel2Collapsed = True
'                    'Me.Panel2MinSize = 0
'                    'Me.SplitterDistance = Me.Height
'                End If

'                'Creating the context menu
'                CreateContextMenu()
'                UpdateLayout()

'            End Sub

'            Private Sub CreateContextMenu()

'                Dim menuItemNameList As New List(Of String) From {"Play", "PlayAll", "StopSound", "ZoomOut", "ZoomIn", "ZoomToSelection", "ZoomFull", "SmoothFadeIn", "SmoothFadeOut", "LinearFadeIn", "LinearFadeOut", "SilenceSelection", "SilenceSelectionZeroCross", "Copy", "Cut", "Paste", "Delete", "Crop", "UndoAll"}
'                Dim menuItemTextList As New List(Of String) From {"Play", "Play all", "Stop", "Zoom out", "Zoom in", "Zoom to selection", "Zoom full", "Fade in selection (smooth)", "Fade out selection (smooth)", "Fade in selection (linear)", "Fade out selection (linear)", "Silence selection", "Silence selection (search zero crossings)", "Copy", "Cut", "Paste", "Delete", "Crop", "Undo all"}

'                For item = 0 To menuItemNameList.Count - 1
'                    Dim menuItem As New ToolStripMenuItem
'                    menuItem.Name = menuItemNameList(item)
'                    menuItem.Text = menuItemTextList(item)
'                    SoundDisplayContextMenu.Items.Add(menuItem)
'                Next

'                'AddHandler soundDisplayContextMenu.ItemClicked, AddressOf menuItem_CLick

'            End Sub


'            'Setting things up
'            Public Sub New_OLD(Optional ByVal UseItemSegmentation As Boolean = False, Optional ByVal DisplaySpectrogram As Boolean = False,
'                    Optional ByRef SpectrogramFormat As Formats.SpectrogramFormat = Nothing, Optional ByVal PaddingTime As Single = 0,
'                    Optional ByVal DrawNormalizedWave As Boolean = False, Optional ByRef SoundPlayer As PortAudioVB.SoundPlayer = Nothing,
'                    Optional ByVal SetSegmentationToZeroCrossings As Boolean = True, Optional ByVal ShowDetectBoundariesButton As Boolean = True,
'                    Optional ByVal ShowUpdateSegmentationButton As Boolean = True, Optional ByVal ShowFadePaddingButton As Boolean = True)


'                'Setting some default sound data
'                CurrentSound = Audio.Sound.GetTestSound()


'                Me.ShowDetectBoundariesButton = ShowDetectBoundariesButton
'                Me.ShowUpdateSegmentationButton = ShowUpdateSegmentationButton
'                Me.ShowFadePaddingButton = ShowFadePaddingButton
'                Me.SetSegmentationToZeroCrossings = SetSegmentationToZeroCrossings
'                Me.SoundPlayer = SoundPlayer
'                Me.DrawNormalizedWave = DrawNormalizedWave


'                'Reading input data or creating default data
'                Me.UseItemSegmentation = UseItemSegmentation

'                Me.PaddingTime = PaddingTime
'                SoundContainerPanel.Dock = DockStyle.Fill
'                Me.Orientation = Orientation.Horizontal
'                Me.Panel1.Controls.Add(SoundContainerPanel)
'                Me.IsSplitterFixed = True
'                If Me.UseItemSegmentation = True Then
'                    Me.Panel2.Controls.Add(SegmentationItemsPanel)
'                    SegmentationItemsPanel.Height = 50
'                    Me.SplitterDistance = Me.Height - SegmentationItemsPanel.Height
'                Else
'                    Me.SplitterDistance = Me.Height
'                End If

'                ShowSpectrogram = DisplaySpectrogram


'                'Setting realtions between panels and pictureboxes
'                SoundBackgroundArea.Dock = DockStyle.Top
'                SoundContainerPanel.Controls.Add(SoundBackgroundArea)
'                If ShowSpectrogram = True Then

'                    SoundBackgroundArea.Controls.Add(SpectrogramArea)
'                    SpectrogramArea.BackColor = Color.Transparent
'                    SpectrogramArea.BringToFront()
'                    SpectrogramArea.Controls.Add(SpectrogramLightFactorScrollBar)
'                    SpectrogramLightFactorScrollBar.Dock = DockStyle.Right

'                    'Setting the value of the spectrogram light bar
'                    SpectrogramLightFactorScrollBar.Minimum = 0
'                    SpectrogramLightFactorScrollBar.Maximum = 100
'                    SpectrogramLightFactorScrollBar.Value = SpectrogramLightFactorScrollBar.Maximum - 20 - SpectrogramLightFactor
'                Else

'                End If
'                SoundBackgroundArea.Controls.Add(WaveArea)
'                WaveArea.BackColor = Color.Transparent
'                WaveArea.BringToFront()
'                SoundBackgroundArea.Controls.Add(TimeArea)
'                TimeArea.BackColor = Color.Transparent
'                TimeArea.BringToFront()
'                SoundContainerPanel.Controls.Add(SoundScrollBar)

'                'Adding borders to the sound boxes
'                TimeArea.BorderStyle = BorderStyle.FixedSingle
'                WaveArea.BorderStyle = BorderStyle.FixedSingle
'                SpectrogramArea.BorderStyle = BorderStyle.FixedSingle
'                SoundBackgroundArea.BorderStyle = BorderStyle.Fixed3D

'                'calculating spectrogram data
'                'Creating a temporary setting
'                If ShowSpectrogram = True Then
'                    DisplayTypeCount += 1

'                    'Creating a default spectrogram format if needed
'                    If SpectrogramFormat Is Nothing Then
'                        Me.SpectrogramFormat = New Formats.SpectrogramFormat(,,,,,,,,, True)
'                    Else
'                        Me.SpectrogramFormat = SpectrogramFormat
'                    End If

'                    SpectrogramWindowDistance = Me.SpectrogramFormat.SpectrogramFftFormat.AnalysisWindowSize - Me.SpectrogramFormat.SpectrogramFftFormat.OverlapSize

'                End If

'                'Showing the temporary sound
'                ShowSound(CurrentSound)

'                'Loading phoneme data
'                If Me.UseItemSegmentation = True Then
'                    SegmentationItemsPanel.FlowDirection = FlowDirection.LeftToRight
'                    SegmentationItemsPanel.Dock = DockStyle.Bottom
'                    'phonemesContainerPanel.AutoSize = True
'                    'phonemesContainerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink
'                    SegmentationItemsPanel.AutoScroll = True
'                    Me.Panel2MinSize = 30
'                    LoadAndModifyPhonemeLevelData()
'                Else
'                    Me.Panel2MinSize = 0
'                    Me.SplitterDistance = Me.Height
'                End If

'                'Adding event handler for spectrogram. Paint events for other drawing areas are declared by Handles statments on the appropriate subs
'                If Me.ShowSpectrogram = True Then
'                    AddHandler SpectrogramArea.Paint, AddressOf Me.DrawSpectrogram
'                End If

'                'Creating the context menu
'                CreateContextMenu()
'                UpdateLayout()

'                'Adding handlers for the wave and spectrogram areas
'                AddHandler WaveArea.MouseDown, AddressOf Me.Container_MouseDown
'                If Me.ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseDown, AddressOf Me.Container_MouseDown


'            End Sub

'            Public Sub ShowSound(ByRef InputSound As Sound, Optional ByVal StartSample As Integer = 0, Optional ByVal LengthInSamples As Integer? = Nothing, Optional ByVal ViewChannel As Integer = 1)

'                'Setting channel (the viewer currently only supports display of one channel at a time)
'                CurrentChannel = ViewChannel ' This should be changed so that the container can display stereo channels

'                CurrentSound = InputSound

'                'Creatig  back-up copy of the input sound
'                SoundBackUp = CurrentSound.CreateCopy

'                'Setting full scale
'                FS_pos = CurrentSound.WaveFormat.PositiveFullScale

'                'Setting display section
'                If LengthInSamples.HasValue = False Then LengthInSamples = CurrentSound.WaveData.SampleData(CurrentChannel).Length
'                If LengthInSamples < 2 Then LengthInSamples = 2
'                DisplayStart_Sample = StartSample
'                DisplayLength_Samples = LengthInSamples

'                'Calculating FFT
'                If ShowSpectrogram = True Then
'                    CurrentSound.FFT = New FftData(CurrentSound.WaveFormat, Me.SpectrogramFormat.SpectrogramFftFormat)
'                    CurrentSound.FFT.CalculateSpectrogramData(CurrentSound, Me.SpectrogramFormat, CurrentChannel)
'                End If

'                If Me.UseItemSegmentation = True Then
'                    LoadAndModifyPhonemeLevelData()
'                End If


'                UpdateLayout()

'            End Sub

'            Private Sub LoadAndModifyPhonemeLevelData()

'                'This step is skipped. Instead word end markers are only added below if they don't already exist. Removing any remaining word end marker (so that it would not contain two word end strings, if the word is resegmented)
'                'sound.SMA.RemoveWordEndString()

'                'This step is also removed (2019-04-04), as a phonetic transcription should always exist if phonetic segmentation is activated.
'                'Adds "Word start" to the phoneme list if no phonemes exist. Doing this enables word level segmentation, since "Word end is added later in Graphics"
'                'sound.SMA.AddWordStartString()

'                'Adding word end string so that the end of the word can be located. However, if one is already added, no new word end string is added
'                CurrentSound.SMA.AddWordEndString()

'                'currentSentenceData = sound.SMA.ChannelData(currentChannel)

'                AddButtons()

'            End Sub

'            Private Sub AddButtons()

'                If CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex).Count > 0 Then

'                    For n = 0 To 1
'                        Dim ChangeWordButton As New Button
'                        With ChangeWordButton
'                            .Text = ChangeWordButtonTexts(n)
'                            .Name = ChangeWordButtonTexts(n).Replace(" ", "") 'storing the identity as an index reference to the changeWordButtonTexts list 
'                            .TextAlign = ContentAlignment.MiddleCenter
'                            .AutoSize = True
'                            .AutoSizeMode = AutoSizeMode.GrowAndShrink
'                            '.BackColor = Color.LightGray
'                            '.Font = New Font(New Font.Size(20), FontStyle.Bold)
'                        End With

'                        'Adding eventhandler
'                        AddHandler ChangeWordButton.Click, AddressOf ChangeWordButtonClick

'                        'Adding the control
'                        SegmentationItemsPanel.Controls.Add(ChangeWordButton)

'                    Next

'                End If


'                For index = 0 To CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex).Count - 1
'                    Dim phonemeLabelButton As New Label

'                    With phonemeLabelButton
'                        .Text = "[ " & CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(index).PhoneticForm & " ]"
'                        .Name = index.ToString 'storing the identity of the phoneme as an index reference to the phoneme list 
'                        .BorderStyle = BorderStyle.Fixed3D
'                        .TextAlign = ContentAlignment.MiddleCenter
'                        .BackColor = Color.LightGray
'                        .AutoSize = True
'                        .Font = New Font("Times New Roman", 14.0F, FontStyle.Regular) 'TODO: It would be good to be able to change this font family, and size
'                    End With

'                    'Adding eventhandler
'                    AddHandler phonemeLabelButton.Click, AddressOf ItemLabelButtonClick

'                    'Adding margin
'                    phonemeLabelButton.Margin = New Padding(4)

'                    'Adding the control
'                    SegmentationItemsPanel.Controls.Add(phonemeLabelButton)
'                Next


'                'Adding a Button for automatic word boundary detection
'                If ShowDetectBoundariesButton = True Then
'                    Dim detectBoundariesButton As New Button

'                    With detectBoundariesButton
'                        .Text = "Detect boundaries"
'                        .Name = "detectBoundariesButton"
'                        .TextAlign = ContentAlignment.MiddleCenter
'                        .BackColor = Color.LightGray
'                        .AutoSize = True
'                        '.Font = New Font(.Font.Name, 13, FontStyle.Regular, .Font.Unit)
'                        '.Font = New Font(New Font.Size(20), FontStyle.Bold)
'                    End With

'                    'Adding eventhandler
'                    AddHandler detectBoundariesButton.Click, AddressOf DetectBoundariesButtonClick

'                    'Adding the control
'                    SegmentationItemsPanel.Controls.Add(detectBoundariesButton)
'                End If



'                'Adding UpdateSegmentationButton button
'                If ShowUpdateSegmentationButton = True Then
'                    Dim UpdateSegmentationButton As New Button

'                    With UpdateSegmentationButton
'                        .Text = "Update segmentation"
'                        .Name = "UpdateSegmentation"
'                        .TextAlign = ContentAlignment.MiddleCenter
'                        .BackColor = Color.LightGray
'                        .AutoSize = True
'                        '.Font = New Font(.Font.Name, 13, FontStyle.Regular, .Font.Unit)
'                        '.Font = New Font(New Font.Size(20), FontStyle.Bold)
'                    End With

'                    'Adding eventhandler
'                    AddHandler UpdateSegmentationButton.Click, AddressOf UpdateSegmentationButtonClick

'                    'Adding the control
'                    SegmentationItemsPanel.Controls.Add(UpdateSegmentationButton)
'                End If


'                'Adding a fade padding Button
'                If ShowFadePaddingButton = True Then

'                    Dim FadePaddingButton As New Button

'                    With FadePaddingButton
'                        .Text = "Fade padding"
'                        .Name = "FadePadding"
'                        .TextAlign = ContentAlignment.MiddleCenter
'                        .BackColor = Color.LightGray
'                        .AutoSize = True
'                        '.Font = New Font(.Font.Name, 13, FontStyle.Regular, .Font.Unit)
'                        '.Font = New Font(New Font.Size(20), FontStyle.Bold)
'                    End With

'                    'Adding eventhandler
'                    AddHandler FadePaddingButton.Click, AddressOf FadePaddingButtonClick

'                    'Adding the control
'                    SegmentationItemsPanel.Controls.Add(FadePaddingButton)
'                End If


'            End Sub

'            'Resetting data
'            Private Sub ResetCurrentWordLevelSegmentationData()

'                If UseItemSegmentation = True Then
'                    For phoneme = 0 To CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex).Count - 1
'                        CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(phoneme).StartSample = -1
'                        CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(phoneme).Length = -1
'                    Next
'                End If

'                InitialSegmentationIsDone = False

'            End Sub
'            Private Sub RetriveSoundBackUp()

'                'Undoes all modifications to the input sound and restores the input sound array
'                CurrentSound = SoundBackUp.CreateCopy

'                If ShowSpectrogram = True Then UpdateSpectrogramData()

'                InitialSegmentationIsDone = False

'                UpdateLayout()

'            End Sub

'            Private Sub UpdateSpectrogramData()

'                'Updating fft data
'                CurrentSound.FFT = New FftData(CurrentSound.WaveFormat, SpectrogramFormat.SpectrogramFftFormat)
'                CurrentSound.FFT.CalculateSpectrogramData(CurrentSound, SpectrogramFormat, CurrentChannel)

'            End Sub

'            'Calculating sound visualization data (and container sizes)
'            Private Sub UpdateSampleTimeScale()

'                'Calculates how many samples each pixel repressents
'                SampleToPixelScale = (DisplayLength_Samples - 1) / SoundBackgroundArea.Width

'            End Sub


'            Private Sub UpdateLayout()

'                'This sub recalculates all sound display data, and container positions and sizes

'                Try

'                    'Setting the number of samples to display
'                    If DisplayStart_Sample < 0 Then DisplayStart_Sample = 0
'                    If DisplayStart_Sample > CurrentSound.WaveData.SampleData(CurrentChannel).Length - 1 Then DisplayStart_Sample = CurrentSound.WaveData.SampleData(CurrentChannel).Length - 1
'                    If DisplayStart_Sample + DisplayLength_Samples > CurrentSound.WaveData.SampleData(CurrentChannel).Length Then DisplayLength_Samples = CurrentSound.WaveData.SampleData(CurrentChannel).Length - DisplayStart_Sample

'                    'Sets the scroll bar scale
'                    SoundScrollBar.Minimum = 0
'                    SoundScrollBar.Maximum = CurrentSound.WaveData.SampleData(CurrentChannel).Length - DisplayLength_Samples
'                    SoundScrollBar.Value = DisplayStart_Sample


'                    Dim drawAreaHeight = SoundContainerPanel.Height - SoundScrollBar.Height
'                    'If drawAreaHeight < buttonPanel.Height - hScBar.Height Then instanceContainerPanel.Height = drawAreaHeight + buttonPanel.Height + hScBar.Height
'                    SoundBackgroundArea.Height = drawAreaHeight

'                    SoundBackgroundArea.Top = 0
'                    SoundScrollBar.Top = SoundBackgroundArea.Bottom
'                    'buttonPanel.Top = soundScrollBar.Bottom


'                    'Updating size and location of the controls inside soundBackgroundArea
'                    TimeArea.Height = 20
'                    TimeArea.Width = SoundBackgroundArea.Width
'                    Select Case DisplayTypeCount
'                        Case 1
'                            WaveArea.Top = 0
'                            WaveArea.Height = SoundBackgroundArea.Height - TimeArea.Height
'                            WaveArea.Width = SoundBackgroundArea.Width
'                            TimeArea.Top = WaveArea.Bottom
'                        Case 2
'                            WaveArea.Top = 0
'                            WaveArea.Width = SoundBackgroundArea.Width
'                            WaveArea.Height = (SoundBackgroundArea.Height - TimeArea.Height) / 2

'                            SpectrogramArea.Width = SoundBackgroundArea.Width
'                            SpectrogramArea.Height = (SoundBackgroundArea.Height - TimeArea.Height) / 2
'                            SpectrogramArea.Top = WaveArea.Height
'                            TimeArea.Top = SpectrogramArea.Bottom

'                        Case 3
'                            Throw New NotImplementedException

'                    End Select

'                    'Updates the scale according to the current size of the sound display
'                    UpdateSampleTimeScale()
'                    'Updating Y-scale
'                    Dim YscaleToPixel_Wave As Single
'                    YscaleToPixel_Wave = (WaveArea.Height / 2) / FS_pos


'                    'Updating wave data - Chosing method to update depending on the size of the sound to display
'                    Select Case DisplayLength_Samples
'                        Case Is > WaveArea.Width * DrawEverySampleLimitFactor
'                            'timeArea.BackColor = Color.Red
'                            ReDim WavePointsArray(1, WaveArea.Width)

'                            Dim sectionMax As Double
'                            Dim sectionMin As Double

'                            For CurrentXpixel As Single = 0 To WaveArea.Width - 1 Step 1

'                                'find section max and min
'                                sectionMax = -FS_pos
'                                sectionMin = FS_pos
'                                For sample = CInt((CurrentXpixel * SampleToPixelScale) + DisplayStart_Sample) To CInt((CurrentXpixel + 1) * SampleToPixelScale + DisplayStart_Sample - 1)
'                                    If CurrentSound.WaveData.SampleData(CurrentChannel)(sample) < sectionMin Then sectionMin = CurrentSound.WaveData.SampleData(CurrentChannel)(sample)
'                                    If CurrentSound.WaveData.SampleData(CurrentChannel)(sample) > sectionMax Then sectionMax = CurrentSound.WaveData.SampleData(CurrentChannel)(sample)
'                                Next

'                                WavePointsArray(0, CurrentXpixel) = ((WaveArea.Height) / 2) - (sectionMax * YscaleToPixel_Wave)
'                                WavePointsArray(1, CurrentXpixel) = ((WaveArea.Height) / 2) - (sectionMin * YscaleToPixel_Wave)
'                            Next

'                        Case Else
'                            'timeArea.BackColor = Color.Cyan
'                            ReDim WaveLinePointArray(DisplayLength_Samples - 1)
'                            Dim currentPoint As New System.Drawing.PointF(0, 0)
'                            For point = 0 To WaveLinePointArray.Length - 1

'                                currentPoint.X = point / SampleToPixelScale
'                                currentPoint.Y = ((WaveArea.Height) / 2) - (CurrentSound.WaveData.SampleData(CurrentChannel)(point + DisplayStart_Sample) * YscaleToPixel_Wave)
'                                WaveLinePointArray(point) = currentPoint

'                            Next
'                    End Select

'                    If DrawNormalizedWave = True Then

'                        'TODO: find out what the max value is
'                        Dim sectionAbsMaxValue As Single = DSP.MeasureSectionLevel(CurrentSound, CurrentChannel, DisplayStart_Sample, DisplayLength_Samples, SoundDataUnit.linear,
'                                                  SoundMeasurementType.AbsolutePeakAmplitude)

'                        'Updating the Creating a new scale
'                        Dim normalizedYscaleToPixel_Wave = (WaveArea.Height / 2) / sectionAbsMaxValue

'                        'Create a normalised version of the wavedata above
'                        'Updating wave data - Chosing method to update depending on the size of the sound to display
'                        Select Case DisplayLength_Samples
'                            Case Is > WaveArea.Width * DrawEverySampleLimitFactor
'                                'timeArea.BackColor = Color.Red
'                                ReDim NormalizedWavePointsArray(1, WaveArea.Width - 1)
'                                Dim sectionMax As Double
'                                Dim sectionMin As Double
'                                For CurrentXpixel = 0 To NormalizedWavePointsArray.GetUpperBound(1) - 2

'                                    'find section max and min
'                                    sectionMax = -FS_pos
'                                    sectionMin = FS_pos
'                                    For sample = CInt(CurrentXpixel * SampleToPixelScale + DisplayStart_Sample) To CInt((CurrentXpixel + 1) * SampleToPixelScale + DisplayStart_Sample - 1)
'                                        If CurrentSound.WaveData.SampleData(CurrentChannel)(sample) < sectionMin Then sectionMin = CurrentSound.WaveData.SampleData(CurrentChannel)(sample)
'                                        If CurrentSound.WaveData.SampleData(CurrentChannel)(sample) > sectionMax Then sectionMax = CurrentSound.WaveData.SampleData(CurrentChannel)(sample)
'                                    Next

'                                    NormalizedWavePointsArray(0, CurrentXpixel) = ((WaveArea.Height) / 2) - (sectionMax * normalizedYscaleToPixel_Wave)
'                                    NormalizedWavePointsArray(1, CurrentXpixel) = ((WaveArea.Height) / 2) - (sectionMin * normalizedYscaleToPixel_Wave)
'                                Next

'                                'Adds the last bit
'                                'find section max and min
'                                'sectionMax = -FS_pos
'                                'sectionMin = FS_pos
'                                'Dim LastPoint As Integer = pointsArray.GetUpperBound(1) - 2
'                                'For sample = LastPoint * Int(XscaleToPixel) + displayStartSample To displayStartSample + displayLengthSamples - 1
'                                'If sound(sample) < sectionMin Then sectionMin = sound(sample)
'                                'If sound(sample) > sectionMax Then sectionMax = sound(sample)
'                                'Next
'                                'pointsArray(0, LastPoint) = container.Bottom - (sectionMax * YscaleToPixel + (container.Bottom / 2))
'                                'pointsArray(1, LastPoint) = container.Bottom - (sectionMin * YscaleToPixel + (container.Bottom / 2))


'                            Case Else
'                                'timeArea.BackColor = Color.Cyan
'                                ReDim NormalizedWaveLinePointArray(DisplayLength_Samples - 1)
'                                Dim currentPoint As New System.Drawing.PointF(0, 0)
'                                For point = 0 To NormalizedWaveLinePointArray.Length - 1

'                                    currentPoint.X = point / SampleToPixelScale
'                                    currentPoint.Y = ((WaveArea.Height) / 2) - (CurrentSound.WaveData.SampleData(CurrentChannel)(point + DisplayStart_Sample) * normalizedYscaleToPixel_Wave)
'                                    NormalizedWaveLinePointArray(point) = currentPoint
'                                Next

'                        End Select


'                    End If


'                    If ShowSpectrogram = True Then
'                        ReCalculateSpectrogramData()
'                    End If

'                Catch ex As Exception
'                    MsgBox(ex.ToString)
'                End Try

'                InvalidateGraphics()

'                'RefreshGraphicElements()

'            End Sub

'            Private Sub ReCalculateSpectrogramData()

'                Try

'                    'Updating spectrogram data

'                    Dim YscaleToPixel_Spectrogram As Single
'                    YscaleToPixel_Spectrogram = SpectrogramArea.Height / CurrentSound.FFT.SpectrogramData(CurrentChannel, 0).WindowData.Length

'                    'Localizing the position of the first spectrogram window to display
'                    Dim displayStartColumn As Integer = Utils.Rounding(DisplayStart_Sample / SpectrogramWindowDistance, Utils.roundingMethods.alwaysDown)
'                    Dim samplesOfFirstWindowOutsideDisplay As Integer = (displayStartColumn * SpectrogramWindowDistance) - DisplayStart_Sample
'                    Dim pixelsOfFirstWindowOutsideDisplay As Single = (samplesOfFirstWindowOutsideDisplay / SampleToPixelScale)

'                    'Localizing the position of the last spectrogram window to display
'                    Dim displayLastColumn As Integer = Utils.Rounding((DisplayStart_Sample + DisplayLength_Samples) / SpectrogramWindowDistance, Utils.roundingMethods.alwaysDown)

'                    'Calculating number of windows
'                    Dim displayColumnCount As Integer = displayLastColumn - displayStartColumn

'                    'Selecting update method depending on the width of the presentation panel
'                    Select Case displayColumnCount
'                        Case Is <= SpectrogramArea.Width

'                            'testForm.startvalueLabel.Text = 0
'                            'testForm.startvalueLabel.Refresh()

'                            'Updating spectrogram X-scale
'                            Dim columnsSizeInPixels As Single = SpectrogramWindowDistance / SampleToPixelScale
'                            Dim analysisWindowLengthInPixels As Single = SpectrogramWindowDistance / SampleToPixelScale

'                            Dim newLowResolutionSpectrogramArea(displayColumnCount * CurrentSound.FFT.SpectrogramData(CurrentChannel, 0).WindowData.Length - 1) As SpectrogramDisplayData

'                            'Creating display data
'                            Dim drawingSurface As Integer = 0
'                            For columnNumber = displayStartColumn To displayStartColumn + displayColumnCount - 1
'                                For coeffNumber = 0 To CurrentSound.FFT.SpectrogramData(CurrentChannel, 0).WindowData.Length - 1

'                                    Dim newBrushGradient As Integer '= sound.FFT.SpectrogramData(currentChannel, columnNumber)(coeffNumber) * spectrogramLightFactor
'                                    Try
'                                        newBrushGradient = CurrentSound.FFT.SpectrogramData(CurrentChannel, columnNumber).WindowData(coeffNumber) * SpectrogramLightFactor
'                                    Catch ex As Exception
'                                        MsgBox(ex.ToString)
'                                    End Try
'                                    If newBrushGradient > 255 Then newBrushGradient = 255
'                                    Dim newBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(newBrushGradient, Drawing.Color.Black))
'                                    'newLowResolutionSpectrogramArea(drawingPixel) = New spectrogramDisplayData(newBrush, xPixel, YPixelCount - yPixel, 1, 1)

'                                    newLowResolutionSpectrogramArea(drawingSurface) = New SpectrogramDisplayData(newBrush, ((columnNumber - displayStartColumn) * columnsSizeInPixels + pixelsOfFirstWindowOutsideDisplay),
'                            (SpectrogramArea.Height - coeffNumber * YscaleToPixel_Spectrogram),
'                            columnsSizeInPixels, YscaleToPixel_Spectrogram)

'                                    drawingSurface += 1
'                                Next
'                            Next

'                            SpectrogramDisplayDataArray = newLowResolutionSpectrogramArea


'                        Case Is > SpectrogramArea.Width

'                            'testForm.startvalueLabel.Text = 1
'                            'testForm.startvalueLabel.Refresh()

'                            Dim XPixelCount As Integer = SpectrogramArea.Width
'                            Dim YPixelCount As Single = SpectrogramArea.Height

'                            Dim columnScaleToPixel As Single = displayColumnCount / XPixelCount
'                            Dim binScaleToPixel As Single = CurrentSound.FFT.SpectrogramData(CurrentChannel, 0).WindowData.Length / YPixelCount

'                            Dim newLowResolutionSpectrogramArea(XPixelCount * YPixelCount - 1) As SpectrogramDisplayData

'                            Dim drawingPixel As Integer = 0
'                            For xPixel = 0 To XPixelCount - 1
'                                For yPixel = 0 To YPixelCount - 1

'                                    Try
'                                        Dim newBrushGradient As Integer = CurrentSound.FFT.SpectrogramData(CurrentChannel, Math.Round(displayStartColumn + xPixel * columnScaleToPixel)).WindowData(Math.Round(yPixel * binScaleToPixel)) * SpectrogramLightFactor
'                                        If newBrushGradient > 255 Then newBrushGradient = 255
'                                        Dim newBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(newBrushGradient, Drawing.Color.Black))
'                                        newLowResolutionSpectrogramArea(drawingPixel) = New SpectrogramDisplayData(newBrush, xPixel, YPixelCount - yPixel, 1, 1)
'                                        drawingPixel += 1
'                                    Catch ex As Exception
'                                        MsgBox(ex.ToString)
'                                    End Try

'                                Next
'                            Next

'                            SpectrogramDisplayDataArray = newLowResolutionSpectrogramArea

'                    End Select

'                Catch ex As Exception
'                    MsgBox(ex.ToString)
'                End Try


'            End Sub

'            'Drawing graphic elements
'            Private Sub DrawWave(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles WaveArea.Paint

'                Try

'                    ' Create a local version of the graphics object for the PictureBox.
'                    Dim g As System.Drawing.Graphics = e.Graphics

'                    'Drawing normalized wave data
'                    If DrawNormalizedWave = True Then

'                        ' Create pen.
'                        Dim grayPen As New System.Drawing.Pen(System.Drawing.Color.Gray, 1)

'                        Select Case DisplayLength_Samples
'                            Case Is > WaveArea.Width * DrawEverySampleLimitFactor
'                                For points = 0 To NormalizedWavePointsArray.GetUpperBound(1) - 1
'                                    g.DrawLine(grayPen, points, NormalizedWavePointsArray(0, points), points, NormalizedWavePointsArray(1, points))
'                                Next
'                            Case Else
'                                g.DrawLines(grayPen, NormalizedWaveLinePointArray)

'                        End Select

'                    End If


'                    ' Create pen.
'                    Dim blackPen As New System.Drawing.Pen(System.Drawing.Color.Black, 1)

'                    'Draws the wave display type
'                    'Draws 0 - line in wave area
'                    ' Create points that define 0 line.
'                    Dim point1 As New System.Drawing.Point(0, WaveArea.Height / 2)
'                    Dim point2 As New System.Drawing.Point(WaveArea.Width, WaveArea.Height / 2)
'                    g.DrawLine(blackPen, point1, point2)


'                    'Draws wave form
'                    Select Case DisplayLength_Samples
'                        Case Is > WaveArea.Width * DrawEverySampleLimitFactor
'                            For points = 0 To WavePointsArray.GetUpperBound(1) - 1
'                                g.DrawLine(blackPen, points, WavePointsArray(0, points), points, WavePointsArray(1, points))
'                            Next
'                        Case Else
'                            g.DrawLines(blackPen, WaveLinePointArray)

'                    End Select


'                Catch ex As Exception
'                    MsgBox(ex.ToString)
'                End Try

'            End Sub
'            Private Sub DrawSpectrogram(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles SpectrogramArea.Paint

'                Try

'                    ' Create a local version of the graphics object for the PictureBox.
'                    Dim g As System.Drawing.Graphics = e.Graphics

'                    'Draws spectrogram
'                    For n = 0 To SpectrogramDisplayDataArray.Count - 1
'                        g.FillRectangle(SpectrogramDisplayDataArray(n).BrushColor, SpectrogramDisplayDataArray(n).Area)
'                    Next


'                    'Draws frequencies on the spectrogram area

'                    'Dim blackPen As New System.Drawing.Pen(System.Drawing.Color.Black, 1)
'                    Dim currentPen As New System.Drawing.Pen(System.Drawing.Color.Red, 1)

'                    Dim valuesToWrite As Integer = Utils.Rounding(SpectrogramFormat.SpectrogramCutFrequency / 1000, Utils.roundingMethods.alwaysUp)

'                    For n = 0 To valuesToWrite - 1

'                        'Drawing frequency numbers
'                        g.DrawString((n * 1000).ToString, New Font("Arial", 7), Brushes.Blue, New PointF(10, SpectrogramArea.Height - n * (SpectrogramArea.Height / (SpectrogramFormat.SpectrogramCutFrequency / 1000)) - 5))

'                        'Drawing lines
'                        Dim y As Single = SpectrogramArea.Height - n * (SpectrogramArea.Height / (SpectrogramFormat.SpectrogramCutFrequency / 1000))
'                        g.DrawLine(currentPen, 0, y, 7, y)

'                    Next

'                    'Drawing unit
'                    g.DrawString("(Hz)", New Font("Arial", 7), Brushes.Blue, New PointF(0, 0))

'                    'Drawing the spectrogram settings on the spectrogram area
'                    Dim drawSpetrogramSettings As Boolean = True
'                    If drawSpetrogramSettings = True Then

'                        g.DrawString("Frequency resolution (FFT size in samples): " & SpectrogramFormat.SpectrogramFftFormat.FftWindowSize.ToString, New Font("Arial", 7), Brushes.Blue, New PointF(40, 4))
'                        g.DrawString("Filter FFT size (samples): " & SpectrogramFormat.SpectrogramPreFirFilterFftFormat.FftWindowSize.ToString, New Font("Arial", 7), Brushes.Blue, New PointF(40, 14))
'                        g.DrawString("Filter kernel creation FFT size (samples): " & SpectrogramFormat.SpectrogramPreFilterKernelFftFormat.FftWindowSize.ToString, New Font("Arial", 7), Brushes.Blue, New PointF(40, 24))

'                    End If

'                Catch ex As Exception
'                    MsgBox(ex.ToString)
'                End Try

'            End Sub
'            Private Sub DrawTime(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles TimeArea.Paint

'                'Empties the container before it is redrawn
'                'timeArea.Invalidate()

'                ' Create a local version of the graphics object for the PictureBox.
'                Dim g As System.Drawing.Graphics = e.Graphics
'                Dim blackPen As New System.Drawing.Pen(System.Drawing.Color.Black, 1)
'                Dim currentPen As New System.Drawing.Pen(System.Drawing.Color.Red, 2)

'                Select Case TimeUnit
'                    Case TimeUnits.samples

'                        Dim valuesToWrite As Integer = 10
'                        For n = 0 To valuesToWrite - 1

'                            'Drawing sample numbers
'                            g.DrawString(DisplayStart_Sample + n * (DisplayLength_Samples / valuesToWrite), New Font("Arial", 7), Brushes.Blue, New PointF(n * (TimeArea.Width / valuesToWrite), 2))

'                            'Drawing lines
'                            Dim x As Single = n * (TimeArea.Width / valuesToWrite)
'                            g.DrawLine(currentPen, x, 0, x, TimeArea.Height)

'                        Next

'                        'Drawing unit
'                        g.DrawString("(Samples)", New Font("Arial", 7), Brushes.Blue, New PointF(TimeArea.Width - 50, 2))

'                    Case TimeUnits.seconds

'                        Dim valuesToWrite As Integer = 10
'                        For n = 0 To valuesToWrite - 1

'                            'Drawing sample numbers
'                            g.DrawString(Math.Round((DisplayStart_Sample + n * (DisplayLength_Samples / valuesToWrite)) / CurrentSound.WaveFormat.SampleRate, 3), New Font("Arial", 7), Brushes.Blue, New PointF(n * (TimeArea.Width / valuesToWrite) + 1, 2))

'                            'Drawing lines
'                            Dim x As Single = n * (TimeArea.Width / valuesToWrite)
'                            g.DrawLine(currentPen, x, 0, x, TimeArea.Height)

'                        Next

'                        'Drawing unit
'                        g.DrawString("(s)", New Font("Arial", 7), Brushes.Blue, New PointF(TimeArea.Width - 25, 2))

'                    Case Else
'                        Throw New NotSupportedException
'                End Select


'            End Sub
'            Private Sub DrawSelection(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles SoundBackgroundArea.Paint

'                Try

'                    'Empties the container before it is redrawn
'                    'soundBackgroundArea.Invalidate()

'                    ' Create a local version of the graphics object for the PictureBox.
'                    Dim g As System.Drawing.Graphics = e.Graphics

'                    ' Create pen.
'                    Dim blackPen As New System.Drawing.Pen(System.Drawing.Color.Black, 1)

'                    'Drawing phoneme boundary lines and labels
'                    If UseItemSegmentation = True Then
'                        If CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex).Count > 0 Then

'                            Dim currentPen As New System.Drawing.Pen(System.Drawing.Color.Red, 2)
'                            'Dim TempPen As New Pen(Color.FromArgb(0, 91, 192), 5)


'                            For phoneme = 0 To CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex).Count - 1
'                                'Drawing line

'                                Dim phonemeStartPixel As Single = (CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(phoneme).StartSample - DisplayStart_Sample) / SampleToPixelScale
'                                If Not (CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(phoneme).StartSample) < 0 Then ' Is used to "hide" the lines and phoneme strings if they are not set. Also means that they cannot be displayed if they are set to 0. 
'                                    g.DrawLine(currentPen, phonemeStartPixel, SoundBackgroundArea.Top, phonemeStartPixel, SoundBackgroundArea.Height)

'                                    'Adding phoneme string
'                                    If ShowSpectrogram = True Then
'                                        'Putting the phoneme string in the middle of the background panel
'                                        g.DrawString(CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(phoneme).PhoneticForm,
'                                              New Font("Arial", 20), Brushes.Blue, New PointF(phonemeStartPixel, SoundBackgroundArea.Height / 2 - 14))
'                                    Else
'                                        'Putting the phoneme string in the bottom of the background panel, above the time scale
'                                        'Dim TempPhonemeColorBrush As New SolidBrush(Color.FromArgb(0, 91, 192))

'                                        g.DrawString(CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(phoneme).PhoneticForm,
'                                              New Font("Arial", 20), Brushes.Black, New PointF(phonemeStartPixel, SoundBackgroundArea.Height - 55))

'                                        'g.DrawString(sound.SMA.ChannelData(currentChannel)(sentence)(currentWordIndex).PhoneData(phoneme).Phoneme,
'                                        '         New Font("Doulos SIL", 40), Brushes.Black, New PointF(phonemeStartPixel, soundBackgroundArea.Height - 105))
'                                    End If

'                                End If
'                            Next
'                        End If
'                    End If


'                    'drawing selection (it also draws a selection of length 0 (which is displayed as a single line) with start index 0, if no selection is made)
'                    Dim greenBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(128, Drawing.Color.Gray))
'                    'Dim myBrush As New System.Drawing.Drawing2D.LinearGradientBrush(selection, Drawing.Color.Red, Drawing.Color.Transparent,
'                    'System.Drawing.Drawing2D.LinearGradientMode.Vertical)
'                    'Dim aHatchBrush As New System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.SmallGrid,
'                    'Drawing.Color.Red, Drawing.Color.Transparent)

'                    'Converting selection data pixels
'                    If SelectionLength_Sample > 0 Then

'                        'UpdateScale()
'                        Dim selectedPeriodStartPixel As Single = (SelectionStart_Sample - DisplayStart_Sample) / SampleToPixelScale
'                        Dim selectedPeriodLengthPixel As Single = SelectionLength_Sample / SampleToPixelScale

'                        'Draws a shaded area
'                        g.FillRectangle(greenBrush, selectedPeriodStartPixel, SoundBackgroundArea.Top, selectedPeriodLengthPixel, SoundBackgroundArea.Height)

'                        'Draw lines 
'                        g.DrawLine(blackPen, selectedPeriodStartPixel, SoundBackgroundArea.Top, selectedPeriodStartPixel, SoundBackgroundArea.Height)
'                        g.DrawLine(blackPen, selectedPeriodStartPixel + selectedPeriodLengthPixel, SoundBackgroundArea.Top, selectedPeriodStartPixel + selectedPeriodLengthPixel, SoundBackgroundArea.Height)

'                        'Drawing selection times
'                        'Start time
'                        Select Case TimeUnit
'                            Case TimeUnits.samples
'                                g.DrawString(SelectionStart_Sample, New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel, 10))
'                                g.DrawString(SelectionStart_Sample + SelectionLength_Sample, New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel + selectedPeriodLengthPixel + 1, 15))

'                                'Also drawing selection length
'                                g.DrawString("(" & SelectionLength_Sample & ")", New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel + selectedPeriodLengthPixel + 1, 30))

'                            Case TimeUnits.seconds
'                                g.DrawString(Math.Round(SelectionStart_Sample / CurrentSound.WaveFormat.SampleRate, 3), New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel, 10))
'                                g.DrawString(Math.Round((SelectionStart_Sample + SelectionLength_Sample) / CurrentSound.WaveFormat.SampleRate, 3), New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel + selectedPeriodLengthPixel + 1, 20))

'                                'Also drawing selection length
'                                g.DrawString("(" & Math.Round(SelectionLength_Sample / CurrentSound.WaveFormat.SampleRate, 3) & ")", New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel + selectedPeriodLengthPixel + 1, 40))

'                            Case Else
'                                Throw New NotSupportedException
'                        End Select

'                    Else
'                        'Drawing cursor
'                        If SampleToPixelScale > 0 Then
'                            Dim selectedPeriodStartPixel As Single = (SelectionStart_Sample - DisplayStart_Sample) / SampleToPixelScale
'                            g.DrawLine(blackPen, selectedPeriodStartPixel, SoundBackgroundArea.Top, selectedPeriodStartPixel, SoundBackgroundArea.Height)
'                        End If
'                    End If

'                Catch ex As Exception
'                    MsgBox(ex.ToString)
'                End Try

'            End Sub

'            Private Sub InvalidateGraphics()

'                'Invalidates each control of the sound area so that they are redrawn on next draw event
'                Dim mySoundDisplayRectangle As New Rectangle(Me.Panel1.Left, Me.Panel1.Top,
'                                      Me.Panel1.Width, Me.Panel1.Height)
'                Me.Panel1.Invalidate(mySoundDisplayRectangle, True)

'            End Sub

'            'Taking care of user input
'            '   - Things that concern the sound drawing area
'            Private Sub Container_MouseDown(sender As System.Object, e As MouseEventArgs) Handles WaveArea.MouseDown, SpectrogramArea.MouseDown

'                If e.Button = MouseButtons.Left Then
'                    RemoveHandler WaveArea.MouseDown, AddressOf Me.Container_MouseDown
'                    If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseDown, AddressOf Me.Container_MouseDown

'                    SelectionStart_Pixel = e.X
'                    SelectionLength_Pixel = 0

'                    InvalidateGraphics()

'                    AddHandler WaveArea.MouseMove, AddressOf Me.Container_MouseMove
'                    AddHandler WaveArea.MouseUp, AddressOf Me.Container_MouseUp
'                    If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseMove, AddressOf Me.Container_MouseMove
'                    If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseUp, AddressOf Me.Container_MouseUp

'                End If


'                If e.Button = MouseButtons.Right Then

'                    'Displaying the context menu
'                    SoundDisplayContextMenu.Location = e.Location 'Cursor.Position
'                    SoundDisplayContextMenu.Show()

'                End If

'            End Sub

'            Private Sub Container_MouseMove(sender As System.Object, e As MouseEventArgs)
'                CalculateSelection(e)
'                InvalidateGraphics()
'            End Sub
'            Private Sub Container_MouseLeave() Handles WaveArea.MouseLeave, SpectrogramArea.MouseLeave

'                'Activates the mouse down handlers of the wave and spectrogram areas
'                AddHandler WaveArea.MouseDown, AddressOf Me.Container_MouseDown
'                If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseDown, AddressOf Me.Container_MouseDown
'                If UseItemSegmentation = True Then
'                    'TODO: Here phoneme setting should be cancelled if started:
'                    'switch back to normal event handler
'                    'switch back the backcolor of the phoneme lables
'                End If

'            End Sub
'            Private Sub Container_MouseUp(sender As System.Object, e As MouseEventArgs)

'                'This sub removes the handlers that create the selection during mousemove, and initializes setting selection end
'                'It also enables a new selection by activating the MouseDown event of wave and spectrogram areas
'                If e.Button = MouseButtons.Left Then

'                    RemoveHandler WaveArea.MouseMove, AddressOf Me.Container_MouseMove
'                    RemoveHandler WaveArea.MouseUp, AddressOf Me.Container_MouseUp
'                    If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseMove, AddressOf Me.Container_MouseMove
'                    If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseUp, AddressOf Me.Container_MouseUp

'                    CalculateSelection(e)

'                    InvalidateGraphics()

'                    AddHandler WaveArea.MouseDown, AddressOf Me.Container_MouseDown
'                    If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseDown, AddressOf Me.Container_MouseDown

'                End If

'            End Sub
'            Private Sub CalculateSelection(e As MouseEventArgs)

'                'This sub calculates start and end of the selected region and stores it in selectionStartSample and selectionLengthSample
'                '(A recalculation of selection start position is needed because a selection can be done in both directions on the screen
'                Dim highlightEndPixel As Single = e.X

'                'Makes sure that selection does not extend outside the window
'                If highlightEndPixel >= SoundBackgroundArea.Width Then highlightEndPixel = SoundBackgroundArea.Width - 1
'                If highlightEndPixel < 0 Then highlightEndPixel = 0

'                SelectionLength_Pixel = highlightEndPixel - SelectionStart_Pixel

'                'Makes sure than selection is in the right direction
'                Dim tempSelectedStartPixel As Single
'                Dim tempSelectedLengthPixel As Single

'                If SelectionLength_Pixel >= 0 Then
'                    tempSelectedStartPixel = SelectionStart_Pixel
'                    tempSelectedLengthPixel = SelectionLength_Pixel
'                Else
'                    tempSelectedStartPixel = highlightEndPixel
'                    tempSelectedLengthPixel = -SelectionLength_Pixel
'                End If

'                SelectionStart_Sample = DisplayStart_Sample + tempSelectedStartPixel * SampleToPixelScale
'                SelectionLength_Sample = tempSelectedLengthPixel * SampleToPixelScale

'            End Sub
'            Private Sub WaveScroll(sender As Object, ByVal e As ScrollEventArgs) Handles SoundScrollBar.Scroll
'                'This sub initializes a soundScrollBarScroll
'                SoundScrollBarScroll(sender, e.NewValue)
'            End Sub

'            Private RecalculateDisplayAreaDueTosoundScrollBar_Enabled As Boolean = True
'            Public Sub SoundScrollBarScroll(sender As Object, ByVal ScrollTo As Integer)

'                'This sub performs a scroll of the sound, in rate of update GraphicUpdateFrequency if the sender is the soundScrollBarScroll
'                'and instantaneoulsy if the sender is anything else (I.E. the sub may be call externally)
'                DisplayStart_Sample = ScrollTo

'                If TypeOf sender Is HScrollBar Then
'                    Dim myScrollBar As HScrollBar = sender
'                    If myScrollBar Is SoundScrollBar Then
'                        'The following code limits the rate of recalculation of the display area to GraphicUpdateFrequency times per second
'                        'Compares a time rounded to hundreds of a second, to the updatefrequency converted to update time in hundreds of a second
'                        If Math.Round(DateTime.Now.Millisecond / 10) Mod ((1 / GraphicUpdateFrequency) * 100) = 0 Then
'                            RecalculateDisplayAreaDueTosoundScrollBar_Enabled = True
'                        End If
'                        'Recalculates the display area only if an update is due
'                        If RecalculateDisplayAreaDueTosoundScrollBar_Enabled = True Then
'                            RecalculateDisplayAreaDueTosoundScrollBar_Enabled = False
'                            UpdateLayout()
'                            Exit Sub
'                        End If
'                    End If
'                End If

'                'If the sub isn't exited (I.E. the sender is not soundScrollBar, the display area is updated)
'                UpdateLayout()

'            End Sub
'            Private Sub SoundScrollBarScrollEnd() Handles SoundScrollBar.MouseUp

'                'This sub makes sure that the sound area is updated when a scroll of the sound is finished (on mouse up)
'                '(this is needed because not all scroll events in soundScrollBarScroll triggers a recalculation of the display area
'                UpdateLayout()
'            End Sub

'            Private Sub Container_MovePhoneme_MouseMove(sender As System.Object, e As MouseEventArgs)

'                'This sub sets the position of the currentPhonemeIndex phoneme.
'                If SetSegmentationToZeroCrossings Then
'                    Dim StartSampl As Integer = DisplayStart_Sample + e.X * SampleToPixelScale
'                    StartSampl = DSP.GetZeroCrossingSample(CurrentSound, 1, StartSampl, DSP.MeasurementsExt.SearchDirections.Closest)
'                    CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(CurrentPhonemeIndex).StartSample = StartSampl
'                Else
'                    CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(CurrentPhonemeIndex).StartSample = DisplayStart_Sample + e.X * SampleToPixelScale
'                End If
'                InvalidateGraphics()

'            End Sub
'            Private Sub Container_PositionPhoneme(sender As System.Object, e As MouseEventArgs)

'                'This sub positions the currentPhonemeIndex phoneme, and switches on event handlers to normal state
'                If SetSegmentationToZeroCrossings Then
'                    Dim StartSampl As Integer = DisplayStart_Sample + e.X * SampleToPixelScale
'                    StartSampl = DSP.GetZeroCrossingSample(CurrentSound, 1, StartSampl, DSP.MeasurementsExt.SearchDirections.Closest)
'                    CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(CurrentPhonemeIndex).StartSample = StartSampl
'                Else
'                    CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(CurrentPhonemeIndex).StartSample = DisplayStart_Sample + e.X * SampleToPixelScale
'                End If

'                'Resets the colors of the buttons
'                For Each item As Control In SegmentationItemsPanel.Controls
'                    item.BackColor = Color.LightGray
'                Next

'                RemoveHandler WaveArea.MouseDown, AddressOf Me.Container_PositionPhoneme
'                RemoveHandler WaveArea.MouseMove, AddressOf Me.Container_MovePhoneme_MouseMove
'                If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseDown, AddressOf Me.Container_PositionPhoneme
'                If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseMove, AddressOf Me.Container_MovePhoneme_MouseMove

'                InvalidateGraphics()

'                AddHandler WaveArea.MouseDown, AddressOf Me.Container_MouseDown
'                If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseDown, AddressOf Me.Container_MouseDown

'            End Sub

'            '   - Other user input
'            Private Sub SetSpectrogramLightFactor(sender As Object, ByVal e As ScrollEventArgs) Handles SpectrogramLightFactorScrollBar.Scroll

'                'This sub calculates a new ligth factor of the spectrogram, by reading the new value from spectrogramLightFactorScroll
'                SpectrogramLightFactor = (2 ^ ((SpectrogramLightFactorScrollBar.Maximum - e.NewValue - 9) / 10)) - 1 'Its wierd why the scroll won't go to maximum value, it only goes to 91 of 100

'            End Sub
'            Private Sub UpdateSpectrogramLight() Handles SpectrogramLightFactorScrollBar.MouseCaptureChanged 'I'd be better if this was MouseUp, but for some reason it is not working, why?

'                'This sub executes recalculation of the spectrogram data after a change of the ligth factor of the spectrogram
'                ReCalculateSpectrogramData()
'                InvalidateGraphics() 'TODO: Acctually only the spectrogram area would need to be invalidated!

'            End Sub
'            Private Sub ContextMenuItem_Click(sender As Object, ByVal e As ToolStripItemClickedEventArgs) Handles SoundDisplayContextMenu.ItemClicked
'                'This sub handles the click on the menu item and directs the code to the appropriate modification/action
'                'After doing so the display area is updated once (actually not all actions (like play) need an update of the display area, this could be fixed for performance reasons)

'                SoundDisplayContextMenu.Hide()
'                'upDatePanel1DisplayData()
'                InvalidateGraphics()

'                Select Case e.ClickedItem.Name
'                    Case "Play"
'                        Play()
'                    Case "PlayAll"
'                        PlayAll()
'                    Case "StopSound"
'                        StopSound()
'                    Case "ZoomOut"
'                        ZoomOut()
'                    Case "ZoomIn"
'                        ZoomIn()
'                    Case "ZoomToSelection"
'                        ZoomToSelection()
'                    Case "ZoomFull"
'                        ZoomFull()
'                    Case "SmoothFadeIn"
'                        FadeIn(DSP.FadeSlopeType.Smooth)
'                    Case "SmoothFadeOut"
'                        FadeOut(DSP.FadeSlopeType.Smooth)
'                    Case "LinearFadeIn"
'                        FadeIn(DSP.FadeSlopeType.Linear)
'                    Case "LinearFadeOut"
'                        FadeOut(DSP.FadeSlopeType.Linear)
'                    Case "SilenceSelection"
'                        SilenceSelection(False)
'                    Case "SilenceSelectionZeroCross"
'                        SilenceSelection(True)
'                    Case "Copy"
'                        GraphicCopy()
'                    Case "Cut"
'                        GraphicCut()
'                    Case "Paste"
'                        GraphicPaste()
'                    Case "Delete"
'                        GraphicDelete()
'                    Case "Crop"
'                        GraphicCrop()
'                    Case "UndoAll"
'                        GraphicUndoAll()
'                End Select

'                UpdateLayout() 'This could be moved to the specific subs to increase performance (not all actions need an update)

'            End Sub


'            Private Sub ReSizeTimer_Tick() Handles Me.SizeChanged

'                Try

'                    'Since the SplitterDistance of myLayoutContainer is changed when myLayoutContainer is resized, it is corrected here
'                    If UseItemSegmentation = True Then
'                        Me.SplitterDistance = Me.Height - SegmentationItemsPanel.Height
'                    Else
'                        Me.SplitterDistance = Me.Height
'                    End If

'                    UpdateLayout()

'                Catch ex As Exception
'                    'Do nothing
'                End Try

'            End Sub

'            '   - variuos button clicks
'            Private Sub ChangeWordButtonClick(sender As Object, ByVal e As MouseEventArgs)

'                'This sub handles clicking on the buttons that change to the next word within the same recording

'                Select Case sender.name
'                    Case ChangeWordButtonTexts(0).Replace(" ", "") ' Previous word

'                        'Check to see if more words exist
'                        If CurrentWordIndex - 1 < 0 Then
'                            MsgBox("You're already displaying the phonemes of the first word in the recording.")
'                            Exit Sub
'                        End If

'                        CurrentWordIndex -= 1

'                        'Removing buttons
'                        For n = 0 To SegmentationItemsPanel.Controls.Count - 1
'                            SegmentationItemsPanel.Controls.RemoveAt(0)
'                        Next

'                        'Adding new buttons
'                        AddButtons()

'                        'Updating the sound display
'                        InvalidateGraphics()

'                    Case ChangeWordButtonTexts(1).Replace(" ", "") ' Next word

'                        'Check to see if more words exist
'                        If CurrentWordIndex + 1 > CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex).Count - 1 Then
'                            MsgBox("There are no more words to segment in the current recording.")
'                            Exit Sub
'                        End If

'                        CurrentWordIndex += 1

'                        'Removing buttons
'                        For n = 0 To SegmentationItemsPanel.Controls.Count - 1
'                            SegmentationItemsPanel.Controls.RemoveAt(0)
'                        Next

'                        'Adding new buttons
'                        AddButtons()

'                        'Updating the sound display
'                        InvalidateGraphics()

'                End Select



'            End Sub
'            Private Sub ItemLabelButtonClick(sender As Object, ByVal e As MouseEventArgs)

'                'This sub handles clicking on the phoneme buttons
'                'A left click will play phoneme
'                'A right click will start the event handlers that is used to position the phoneme on the sound display

'                CurrentPhonemeIndex = sender.name

'                Select Case e.Button
'                    Case MouseButtons.Left
'                        'Playing the phoneme (but not the word end)

'                        If Not CurrentPhonemeIndex = CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex).Count - 1 Then
'                            Dim startSample As Integer = CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(CurrentPhonemeIndex).StartSample
'                            Dim lengthToPlay As Integer = CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(CurrentPhonemeIndex + 1).StartSample - 1 - startSample 'PlaySoundStream plays to the end if length is 0
'                            If lengthToPlay < 0 Then lengthToPlay = 0
'                            'PlayBack.Play.PlaySoundStream(sound, currentSentenceData(currentWordIndex).PhoneData(currentPhonemeIndex).StartSample, lengthToPlay)

'                            If SoundPlayer Is Nothing Then CreateNewPaSoundPLayer()
'                            PlayBack.PlayDuplexSoundStream(SoundPlayer, CurrentSound, CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(CurrentPhonemeIndex).StartSample, lengthToPlay,, , 0, 0)

'                        End If

'                    Case MouseButtons.Right

'                        sender.backcolor = Color.Red

'                        'Turn of other eventhandlers
'                        RemoveHandler WaveArea.MouseDown, AddressOf Me.Container_MouseDown
'                        If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseDown, AddressOf Me.Container_MouseDown

'                        'Turn on mouse move eventhandler
'                        AddHandler WaveArea.MouseDown, AddressOf Me.Container_PositionPhoneme
'                        AddHandler WaveArea.MouseMove, AddressOf Me.Container_MovePhoneme_MouseMove

'                        If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseDown, AddressOf Me.Container_PositionPhoneme
'                        If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseMove, AddressOf Me.Container_MovePhoneme_MouseMove

'                End Select

'            End Sub


'            Public Sub CreateNewPaSoundPLayer()

'                Dim newAudioSettingsDialog As New AudioSettingsDialog(CurrentSound.WaveFormat.SampleRate)
'                Dim DialogResult = newAudioSettingsDialog.ShowDialog()
'                Dim MyAudioApiSettings As AudioApiSettings = Nothing
'                If DialogResult = DialogResult.OK Then
'                    MyAudioApiSettings = newAudioSettingsDialog.CurrentAudioApiSettings
'                Else
'                    MsgBox("Default Setting is being used")
'                    MyAudioApiSettings = newAudioSettingsDialog.CurrentAudioApiSettings
'                End If

'                SoundPlayer = New PortAudioVB.SoundPlayer(True, CurrentSound.WaveFormat, CurrentSound, MyAudioApiSettings, , )

'            End Sub


'            Public Sub Play()

'                If SoundPlayer Is Nothing Then CreateNewPaSoundPLayer()

'                If SelectionLength_Sample = 0 Then

'                    PlayAll()

'                    'PlayBack.PlayDuplexSoundStream(SoundPlayer, sound, Nothing, Nothing)

'                Else
'                    UpdateSampleTimeScale()

'                    PlayBack.PlayDuplexSoundStream(SoundPlayer, CurrentSound, SelectionStart_Sample, SelectionLength_Sample,, , 0, 0)

'                End If

'            End Sub



'            'Responses to menu functions (these can also be called from external code)
'            Public Sub PlayAll()

'                UpdateSampleTimeScale()
'                If SoundPlayer Is Nothing Then CreateNewPaSoundPLayer()
'                PlayBack.PlayDuplexSoundStream(SoundPlayer, CurrentSound, Nothing, Nothing,, , 0, 0)

'            End Sub
'            Public Sub StopSound()

'                SoundPlayer.Stop(0.1)

'                'My.Computer.Stop()
'            End Sub
'            Public Sub ZoomOut()
'                ' making the selection twice the size
'                UpdateSampleTimeScale()

'                DisplayLength_Samples = DisplayLength_Samples * 2
'                DisplayStart_Sample = DisplayStart_Sample - Int(DisplayLength_Samples / 4)

'                'Making sure it doesn't zoom out to much
'                If DisplayStart_Sample < 0 Then DisplayStart_Sample = 0
'                If DisplayStart_Sample > CurrentSound.WaveData.SampleData(CurrentChannel).Length - 1 Then DisplayStart_Sample = CurrentSound.WaveData.SampleData(CurrentChannel).Length - 1
'                If DisplayStart_Sample + DisplayLength_Samples > CurrentSound.WaveData.SampleData(CurrentChannel).Length Then DisplayLength_Samples = CurrentSound.WaveData.SampleData(CurrentChannel).Length - DisplayStart_Sample

'            End Sub
'            Public Sub ZoomIn()
'                ' making the selection half the size
'                UpdateSampleTimeScale()

'                'Changing range
'                DisplayLength_Samples = Int(DisplayLength_Samples / 2)
'                DisplayStart_Sample = DisplayStart_Sample + Int(DisplayLength_Samples / 2)

'                'Making sure length is not shorter that 2 samples
'                If DisplayLength_Samples < 2 Then DisplayLength_Samples = 2

'            End Sub
'            Public Sub ZoomToSelection()
'                UpdateSampleTimeScale()
'                DisplayStart_Sample = SelectionStart_Sample
'                DisplayLength_Samples = SelectionLength_Sample

'                'Making sure length is not shorter that 1 sample
'                If DisplayLength_Samples < 2 Then DisplayLength_Samples = 2

'            End Sub
'            Public Sub ZoomFull()
'                UpdateSampleTimeScale()
'                DisplayStart_Sample = 0
'                DisplayLength_Samples = CurrentSound.WaveData.SampleData(CurrentChannel).Length

'            End Sub

'            ''' <summary>
'            ''' Fades in the selected sound using the indicates fade slope type.
'            ''' </summary>
'            ''' <param name="fadeSlopeType"></param>
'            Private Sub FadeIn(ByVal fadeSlopeType As DSP.FadeSlopeType)

'                If Not SelectionLength_Sample < 1 Then
'                    DSP.Fade(CurrentSound,, 0, CurrentChannel, SelectionStart_Sample, SelectionLength_Sample, fadeSlopeType)

'                    'Recalculates spectrogram data, since the waveform have been changed
'                    If ShowSpectrogram = True Then UpdateSpectrogramData()

'                End If

'            End Sub

'            ''' <summary>
'            ''' Fades out the selected sound using the indicates fade slope type.
'            ''' </summary>
'            ''' <param name="fadeSlopeType"></param>
'            Private Sub FadeOut(ByVal fadeSlopeType As DSP.FadeSlopeType)

'                If Not SelectionLength_Sample < 1 Then
'                    DSP.Fade(CurrentSound, 0, , CurrentChannel, SelectionStart_Sample, SelectionLength_Sample, fadeSlopeType)

'                    'Recalculates spectrogram data, since the waveform have been changed
'                    If ShowSpectrogram = True Then UpdateSpectrogramData()

'                End If

'            End Sub

'            ''' <summary>
'            ''' Silences the selected section of the sound
'            ''' </summary>
'            ''' <param name="AdjustToZeroCrossings">If set to true, the silent section starts and end at the closest zero crossings withing the selected section of the wave form.</param>
'            Private Sub SilenceSelection(ByVal AdjustToZeroCrossings As Boolean)

'                Dim SilenceStartSample As Integer = SelectionStart_Sample
'                Dim SilenceLength As Integer = SelectionLength_Sample

'                If AdjustToZeroCrossings = True Then

'                    Dim SelectionEndSample As Integer = SilenceStartSample + SilenceLength

'                    SilenceStartSample = DSP.GetZeroCrossingSample(CurrentSound, CurrentChannel, SilenceStartSample, DSP.MeasurementsExt.SearchDirections.Later)
'                    SilenceLength = DSP.GetZeroCrossingSample(CurrentSound, CurrentChannel, SilenceLength, DSP.MeasurementsExt.SearchDirections.Earlier)

'                    'Checking that the zero crossing search hasn't caused SilenceStartSample to be equal or higher that SelectionEndSample. If so SilenceLength is set to 0 to stop silencing.
'                    If SilenceStartSample >= SelectionEndSample Then SilenceLength = 0

'                End If

'                'Silencing the section using the fade function, with bith silent start and end
'                If Not SelectionLength_Sample < 1 Then
'                    DSP.Fade(CurrentSound, , , CurrentChannel, SilenceStartSample, SilenceLength, DSP.FadeSlopeType.Linear)

'                    'Recalculates spectrogram data, since the waveform have been changed
'                    If ShowSpectrogram = True Then UpdateSpectrogramData()

'                End If
'            End Sub


'            Private Sub GraphicCopy()

'                If Not SelectionLength_Sample < 1 Then

'                    'Copies the selected sound to a new array
'                    ReDim SelectionCopy(SelectionLength_Sample - 1)
'                    For sample = 0 To SelectionCopy.Length - 1
'                        SelectionCopy(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample + SelectionStart_Sample)
'                    Next

'                End If

'            End Sub
'            Private Sub GraphicCut()

'                If Not SelectionLength_Sample < 1 Then

'                    'Copies the selected sound to a new array
'                    ReDim SelectionCopy(SelectionLength_Sample - 1)
'                    For sample = 0 To SelectionCopy.Length - 1
'                        SelectionCopy(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample + SelectionStart_Sample)
'                    Next

'                    'Cutting the selected sound from sound
'                    'Getting a copy of sound without the selected samples
'                    Dim newArray(CurrentSound.WaveData.SampleData(CurrentChannel).Length - SelectionLength_Sample - 1) As Single
'                    For sample = 0 To SelectionStart_Sample - 1
'                        newArray(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample)
'                    Next
'                    For sample = SelectionStart_Sample To newArray.Length - 1
'                        newArray(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample + SelectionLength_Sample)
'                    Next

'                    CurrentSound.WaveData.SampleData(CurrentChannel) = newArray

'                    'Recalculates spectrogram data, since the waveform have been changed
'                    If ShowSpectrogram = True Then UpdateSpectrogramData()

'                    UpdateLayout()
'                    SelectionLength_Sample = 0
'                    If UseItemSegmentation = True Then ResetCurrentWordLevelSegmentationData()

'                End If

'            End Sub
'            Private Sub GraphicPaste()

'                If SelectionCopy.Length > 0 Then

'                    'Copies the selected sound to a new array

'                    'Pasting the data in slectionCopy starting at selectionStartSample 
'                    'Copying the data prior to selectionStartSample 
'                    Dim newArray(CurrentSound.WaveData.SampleData(CurrentChannel).Length + SelectionCopy.Length - 1) As Single
'                    For sample = 0 To SelectionStart_Sample - 1
'                        newArray(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample)
'                    Next

'                    'Pasting the data from selectionCopy
'                    For sample = SelectionStart_Sample To SelectionStart_Sample + SelectionCopy.Length - 1
'                        newArray(sample) = SelectionCopy(sample - SelectionStart_Sample)
'                    Next

'                    'Copying the data after selectionStartSample 
'                    For sample = SelectionStart_Sample + SelectionCopy.Length To newArray.Length - 1
'                        newArray(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample - SelectionCopy.Length)
'                    Next

'                    CurrentSound.WaveData.SampleData(CurrentChannel) = newArray
'                    'sound = newArray

'                    'Recalculates spectrogram data, since the waveform have been changed
'                    If ShowSpectrogram = True Then UpdateSpectrogramData()

'                    SelectionLength_Sample = 0
'                    If UseItemSegmentation = True Then ResetCurrentWordLevelSegmentationData()

'                End If


'            End Sub
'            Private Sub GraphicDelete()

'                If Not SelectionLength_Sample < 1 Then

'                    'Deleting the selected sound from sound
'                    'Getting a copy of sound without the selected samples
'                    Dim newArray(CurrentSound.WaveData.SampleData(CurrentChannel).Length - SelectionLength_Sample - 1) As Single
'                    For sample = 0 To SelectionStart_Sample - 1
'                        newArray(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample)
'                    Next
'                    For sample = SelectionStart_Sample To newArray.Length - 1
'                        newArray(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample + SelectionLength_Sample)
'                    Next

'                    CurrentSound.WaveData.SampleData(CurrentChannel) = newArray
'                    'sound = newArray

'                    'Recalculates spectrogram data, since the waveform have been changed
'                    If ShowSpectrogram = True Then UpdateSpectrogramData()

'                    SelectionLength_Sample = 0
'                    If UseItemSegmentation = True Then ResetCurrentWordLevelSegmentationData()

'                End If

'            End Sub
'            Private Sub GraphicCrop()

'                If Not SelectionLength_Sample < 1 Then

'                    'Copies the selected sound to a new array
'                    ReDim SelectionCopy(SelectionLength_Sample - 1)
'                    For sample = 0 To SelectionCopy.Length - 1
'                        SelectionCopy(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample + SelectionStart_Sample)
'                    Next

'                    'Replacing the old sound with selectionCopy

'                    CurrentSound.WaveData.SampleData(CurrentChannel) = SelectionCopy
'                    'sound = selectionCopy

'                    'Recalculates spectrogram data, since the waveform have been changed
'                    If ShowSpectrogram = True Then UpdateSpectrogramData()

'                    SelectionLength_Sample = 0
'                    If UseItemSegmentation = True Then ResetCurrentWordLevelSegmentationData()

'                End If

'            End Sub
'            Private Sub GraphicUndoAll()
'                RetriveSoundBackUp()
'                'If UseItemSegmentation = True Then resetCurrentWordLevelSegmentationData() ' I don't think this is needed here. Since the original word is retrieved, it's probably ok to also keep the SMA data in it as it is.
'                SelectionLength_Sample = 0
'                ZoomFull()
'            End Sub

'            'Other available public functions, not primarily for internal use
'            Public Sub ZoomTo(ByVal startSample As Integer, ByVal length As Integer)

'                'displaying the waveform for the specified sample interval
'                UpdateSampleTimeScale()

'                'Making sure length is not shorter that 2 samples
'                If length < 2 Then length = 2

'                'Making sure it doesn't zoom out to much
'                If startSample < 0 Then startSample = 0
'                If startSample > CurrentSound.WaveData.SampleData(CurrentChannel).Length - 1 Then startSample = CurrentSound.WaveData.SampleData(CurrentChannel).Length - 1
'                If startSample + length > CurrentSound.WaveData.SampleData(CurrentChannel).Length Then length = CurrentSound.WaveData.SampleData(CurrentChannel).Length - startSample

'                DisplayLength_Samples = startSample
'                DisplayStart_Sample = length

'                'Updating the value of the sound scroll bar
'                SoundScrollBar.Value = DisplayStart_Sample

'                UpdateLayout()
'            End Sub

'            'Performing automatic speech boundary detection
'            Private Sub DetectBoundariesButtonClick()

'                CurrentSound.SMA.DetectSpeechBoundaries(CurrentSound, LongestSilentSegment, SilenceDefinition, TemporalIntegrationDuration,
'                                              DetailedTemporalIntegrationDuration, DetailedSilenceCriteria)

'                UpdateLayout()

'            End Sub


'            'Storing data
'            Private Sub UpdateSegmentationButtonClick(sender As Object, ByVal e As MouseEventArgs)
'                UpdateSegmentation()
'            End Sub

'            Private Sub FadePaddingButtonClick(sender As Object, ByVal e As MouseEventArgs)
'                FadePadding()
'                UpdateLayout()
'            End Sub


'            ''' <summary>
'            ''' Checks that the order of phonemes is correct and returns False if the order is wrong.
'            ''' If the order is correct phoneme lengths are calculated and stored in currentSentenceData.
'            ''' </summary>
'            ''' <returns></returns>
'            Public Function CheckPhonemeOrderAndCalculatePhonemeLengths(Optional ByVal StoreWordSegmentationFromPhonemes As Boolean = False)

'                'Checking the order of phonemes

'                For c As Integer = 1 To CurrentSound.SMA.ChannelCount
'                    For sentence As Integer = 0 To CurrentSound.SMA.ChannelData(c).Count - 1


'                        For word = 0 To CurrentSound.SMA.ChannelData(c)(sentence).Count - 1
'                            For testedPhonemeIndex = 0 To CurrentSound.SMA.ChannelData(c)(sentence)(word).Count - 1

'                                'Updates the word bounaries
'                                If StoreWordSegmentationFromPhonemes = True Then

'                                    If testedPhonemeIndex = 0 Then
'                                        CurrentSound.SMA.ChannelData(c)(sentence)(word).StartSample = CurrentSound.SMA.ChannelData(c)(sentence)(word)(0).StartSample
'                                    End If

'                                    If testedPhonemeIndex = CurrentSound.SMA.ChannelData(c)(sentence)(word).Count - 1 Then
'                                        CurrentSound.SMA.ChannelData(c)(sentence)(word).Length =
'                                    CurrentSound.SMA.ChannelData(c)(sentence)(word)(CurrentSound.SMA.ChannelData(c)(sentence)(word).Count - 1).StartSample -
'                                   CurrentSound.SMA.ChannelData(c)(sentence)(word)(0).StartSample + 1
'                                    End If
'                                End If

'                                'Compared to the previous phoneme
'                                If Not testedPhonemeIndex = 0 Then
'                                    If CurrentSound.SMA.ChannelData(c)(sentence)(word)(testedPhonemeIndex).StartSample < CurrentSound.SMA.ChannelData(c)(sentence)(word)(testedPhonemeIndex - 1).StartSample Then
'                                        'currentSentenceData(currentWordIndex).PhoneData(testedPhonemeIndex).StartSample = -1
'                                        MsgBox("Wrong order of phonemes in the word " & CurrentSound.SMA.ChannelData(c)(sentence)(word).OrthographicForm & ": " & vbCr & vbCr &
'                                       "The phoneme [" & CurrentSound.SMA.ChannelData(c)(sentence)(word)(testedPhonemeIndex).PhoneticForm & "] must be placed after [" & CurrentSound.SMA.ChannelData(c)(sentence)(word)(testedPhonemeIndex - 1).PhoneticForm & "]!",, "Place phonemes in currect order!")
'                                        Return False
'                                    End If
'                                End If

'                                'Compared to the following phoneme
'                                If Not testedPhonemeIndex = CurrentSound.SMA.ChannelData(c)(sentence)(word).Count - 1 Then
'                                    If Not CurrentSound.SMA.ChannelData(c)(sentence)(word)(testedPhonemeIndex + 1).StartSample = -1 Then
'                                        If CurrentSound.SMA.ChannelData(c)(sentence)(word)(testedPhonemeIndex).StartSample > CurrentSound.SMA.ChannelData(c)(sentence)(word)(testedPhonemeIndex + 1).StartSample Then
'                                            'currentSentenceData(currentWordIndex).PhoneData(testedPhonemeIndex).StartSample = -1
'                                            MsgBox("Wrong order of phonemes in the word " & CurrentSound.SMA.ChannelData(c)(sentence)(word).OrthographicForm & ": " & vbCr & vbCr &
'                                           "The phoneme [" & CurrentSound.SMA.ChannelData(c)(sentence)(word)(testedPhonemeIndex + 1).PhoneticForm & "] must be placed after [" & CurrentSound.SMA.ChannelData(c)(sentence)(word)(testedPhonemeIndex).PhoneticForm & "]!",, "Place phonemes in currect order!")
'                                            Return False
'                                        End If
'                                    End If
'                                End If
'                            Next


'                            'Calculating phoneme lengths
'                            For phoneme = 0 To CurrentSound.SMA.ChannelData(c)(sentence)(word).Count - 2
'                                Dim phonemeLength As Integer = CurrentSound.SMA.ChannelData(c)(sentence)(word)(phoneme + 1).StartSample - CurrentSound.SMA.ChannelData(c)(sentence)(word)(phoneme).StartSample
'                                If phonemeLength < 0 Then phonemeLength = 0
'                                CurrentSound.SMA.ChannelData(c)(sentence)(word)(phoneme).Length = phonemeLength
'                            Next
'                        Next
'                    Next
'                Next


'                Return True

'                'To check values only
'                'Dim table As New DataTable
'                'table.Columns.Add("phoneme", GetType(String))
'                'table.Columns.Add("startSample", GetType(String))
'                'table.Columns.Add("length", GetType(String))

'                'For row = 0 To currentChannelData(currentWordIndex).PhonemeLevelDataList.Count - 1
'                'table.Rows.Add(currentChannelData(currentWordIndex).PhonemeLevelDataList(row).phoneme, currentChannelData(currentWordIndex).PhonemeLevelDataList(row).startSample, currentChannelData(currentWordIndex).PhonemeLevelDataList(row).length)
'                'Next

'                'displayTable.DataGridView1.DataSource = table

'            End Function

'            Dim InitialSegmentationIsDone As Boolean = False
'            Public Sub UpdateSegmentation(Optional ByVal DoZoomFull As Boolean = True)

'                Try

'                    If Not CheckPhonemeOrderAndCalculatePhonemeLengths() = True Then Exit Sub

'                    CurrentSound.SMA.UpdateSegmentation(CurrentSound, PaddingTime, CurrentChannel)

'                    'Updating spectrogram data
'                    If ShowSpectrogram = True Then
'                        'Updating fft data
'                        CurrentSound.FFT = New FftData(CurrentSound.WaveFormat, SpectrogramFormat.SpectrogramFftFormat)
'                        CurrentSound.FFT.CalculateSpectrogramData(CurrentSound, SpectrogramFormat, CurrentChannel)
'                    End If

'                    InitialSegmentationIsDone = True

'                    If DoZoomFull = True Then ZoomFull()
'                    SelectionLength_Sample = 0
'                    UpdateLayout()

'                Catch ex As Exception
'                    MsgBox(ex.ToString)
'                End Try

'            End Sub

'            ''' <summary>
'            ''' Fades the padded sections
'            ''' </summary>
'            Private Sub FadePadding()

'                If InitialSegmentationIsDone = True And CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex).StartSample <> -1 And CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex).Length <> 0 Then
'                    CurrentSound.SMA.FadePaddingSection(CurrentSound, CurrentChannel)
'                Else
'                    MsgBox("Unable to fade padding section due to incomplete boundary segmentation.")
'                End If

'            End Sub

'            Private Class SpectrogramDisplayData

'                Public Property Area As RectangleF
'                Public Property BrushColor As Brush

'                Public Sub New(setBrushColor As Brush, X As Single, Y As Single, width As Single, height As Single)

'                    If X = Nothing Then X = 0
'                    If Y = Nothing Then Y = 0
'                    If width = Nothing Then width = 0
'                    If height = Nothing Then height = 0

'                    Area = New RectangleF(X, Y, width, height)
'                    BrushColor = setBrushColor

'                End Sub

'            End Class

'        End Class




'        Public Class SoundLevelMeter
'            Inherits PictureBox

'            Public Property minLevel As Single = -100
'            Public Property maxLevel As Single = +12
'            Public Property FullScaleLevel As Single = 0
'            Public Property WarningLevel As Single = -4
'            Private Property currentLevel As Single = minLevel
'            Private Property memoryPeakLevel As Single
'            Private currentLevelHeightInPixels As Single
'            Private FullScaleLevelHeightInPixels As Single
'            Private WarningLevelHeightInPixels As Single
'            Private memoryPeakLevelHeightInPixels As Single
'            Private maxMemory As New List(Of Single)
'            Public Property Activated As Boolean = False

'            Public Sub New()

'                MyBase.New

'                Me.BackColor = Color.White

'                setPeakLevelMemoryItemCount()

'            End Sub

'            Public Sub setPeakLevelMemoryItemCount(Optional ItemCount As Integer = 10)

'                For memoryItems = 0 To ItemCount - 1
'                    maxMemory.Add(minLevel)
'                Next

'            End Sub

'            Public Sub UpdateLevel(newLevel As Single)

'                currentLevel = newLevel

'                maxMemory.Add(currentLevel)
'                maxMemory.RemoveAt(0)

'                'Dim myRectangle As New RectangleF(Left, Top, Width, Height)
'                'Dim myRegion As New Region(myRectangle)
'                'Me.Invalidate(myRegion, True)
'                Invalidate()

'            End Sub

'            Public Sub Activate()
'                AddHandler Me.Paint, AddressOf drawMeter
'                Activated = True
'            End Sub

'            Public Sub Inactivate()
'                currentLevel = minLevel
'                For n = 0 To maxMemory.Count - 1
'                    maxMemory(n) = minLevel
'                Next
'                Refresh()

'                RemoveHandler Me.Paint, AddressOf drawMeter
'                Activated = False
'            End Sub

'            Private Sub drawMeter(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)

'                Try

'                    ' Create a local version of the graphics object for the PictureBox.
'                    Dim g As System.Drawing.Graphics = e.Graphics

'                    Dim dBRange As Single = Math.Abs(maxLevel - minLevel)


'                    'Converting to pixels
'                    currentLevelHeightInPixels = (Height / dBRange) * (currentLevel - minLevel)
'                    FullScaleLevelHeightInPixels = (Height / dBRange) * (FullScaleLevel - minLevel)
'                    WarningLevelHeightInPixels = (Height / dBRange) * (WarningLevel - minLevel)
'                    memoryPeakLevelHeightInPixels = (Height / dBRange) * (maxMemory.Max - minLevel)

'                    Dim greyBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(128, Drawing.SystemColors.Control))
'                    Dim greenBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(128, Drawing.Color.Green))
'                    Dim redBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(128, Drawing.Color.Red))
'                    Dim yellowBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(128, Drawing.Color.Yellow))

'                    Dim halfWidth As Single = Me.Width / 2
'                    Dim dBLineStartX As Single = (Me.Width / 8) * 3
'                    Dim dBLineEndX As Single = (Me.Width / 8) * 5
'                    Dim dBStringYCorrection As Single = 5 '8 / 2 ' (Height / Rounding(Math.Abs(dBRange) / 6, AudioEnumerators.roundingMethods.alwaysUp))  '(which is Height / total number of dBs to display)/2


'                    'Drawing backcolor rectangle on the values half in system control color
'                    g.FillRectangle(greyBrush, 0, 0, halfWidth, Height)

'                    'Draws level rectangles
'                    Select Case currentLevel
'                        Case <= WarningLevel
'                            g.FillRectangle(greenBrush, halfWidth, Height - currentLevelHeightInPixels, halfWidth, currentLevelHeightInPixels)

'                        Case > WarningLevel
'                            If currentLevel < FullScaleLevel Then
'                                g.FillRectangle(greenBrush, halfWidth, Height - WarningLevelHeightInPixels, halfWidth, WarningLevelHeightInPixels)
'                                g.FillRectangle(yellowBrush, halfWidth, Height - WarningLevelHeightInPixels - (currentLevelHeightInPixels - WarningLevelHeightInPixels), halfWidth, currentLevelHeightInPixels - WarningLevelHeightInPixels)

'                            Else
'                                g.FillRectangle(greenBrush, halfWidth, Height - WarningLevelHeightInPixels, halfWidth, WarningLevelHeightInPixels)
'                                g.FillRectangle(yellowBrush, halfWidth, Height - FullScaleLevelHeightInPixels, halfWidth, FullScaleLevelHeightInPixels - WarningLevelHeightInPixels)
'                                g.FillRectangle(redBrush, halfWidth, Height - FullScaleLevelHeightInPixels - (currentLevelHeightInPixels - FullScaleLevelHeightInPixels), halfWidth, currentLevelHeightInPixels - FullScaleLevelHeightInPixels)

'                            End If
'                    End Select

'                    Dim blackPen As New System.Drawing.Pen(System.Drawing.Color.Black, 1)
'                    Dim grayPen As New System.Drawing.Pen(System.Drawing.Color.Gray, 1)
'                    Dim redPen As New System.Drawing.Pen(System.Drawing.Color.Red, 1)


'                    'Drawing zero line
'                    g.DrawString((0).ToString, New Font("Arial", 7), Brushes.Blue, New PointF(0, Height - FullScaleLevelHeightInPixels - dBStringYCorrection))
'                    Dim zy As Single = Height - FullScaleLevelHeightInPixels
'                    g.DrawLine(blackPen, dBLineStartX, zy, Width, zy)

'                    'Values from zero and above 
'                    Dim valuesFromZeroAndUP As Integer = Utils.Rounding(Math.Abs(maxLevel) / 6, Utils.roundingMethods.alwaysUp)
'                    For n = 1 To valuesFromZeroAndUP

'                        'Drawing frequency numbers
'                        g.DrawString((n * 6).ToString, New Font("Arial", 7), Brushes.Blue, New PointF(0, Height - FullScaleLevelHeightInPixels - 6 * n * (Height / dBRange) - dBStringYCorrection))

'                        'Drawing lines
'                        Dim y As Single = Height - FullScaleLevelHeightInPixels - 6 * n * (Height / dBRange)
'                        g.DrawLine(blackPen, dBLineStartX, y, dBLineEndX, y)
'                    Next


'                    'Values below zero 
'                    Dim valuesBelowZero As Integer = Utils.Rounding(Math.Abs(minLevel) / 6, Utils.roundingMethods.alwaysUp)
'                    For n = 0 To valuesBelowZero - 1

'                        'Drawing frequency numbers
'                        g.DrawString((n * -6).ToString, New Font("Arial", 7), Brushes.Blue, New PointF(0, Height - FullScaleLevelHeightInPixels + 6 * n * (Height / dBRange) - dBStringYCorrection))

'                        'Drawing lines
'                        Dim y As Single = Height - FullScaleLevelHeightInPixels + 6 * n * (Height / dBRange)
'                        g.DrawLine(blackPen, dBLineStartX, y, dBLineEndX, y)

'                    Next

'                    'Drawing memoryPeakLevel
'                    Dim my As Single = Height - memoryPeakLevelHeightInPixels
'                    g.DrawLine(redPen, halfWidth, my, Width, my)

'                    'Drawing separator line
'                    g.DrawLine(grayPen, halfWidth + 1, 0, halfWidth + 1, Height)
'                    g.DrawLine(blackPen, halfWidth, 0, halfWidth, Height)

'                Catch ex As Exception
'                    MsgBox(ex.ToString)
'                End Try

'            End Sub


'        End Class




'    End Namespace

'End Namespace