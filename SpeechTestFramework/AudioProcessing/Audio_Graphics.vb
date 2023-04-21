'This software is available under the following license:
'MIT/X11 License
'
'Copyright (c) 2017 Erik Witte
'
'Permission is hereby granted, free of charge, to any person obtaining a copy
'of this software and associated documentation files (the ''Software''), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'copies of the Software, and to permit persons to whom the Software is
'furnished to do so, subject to the following conditions:
'
'The above copyright notice and this permission notice shall be included in all
'copies or substantial portions of the Software.
'
'THE SOFTWARE IS PROVIDED ''AS IS'', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
'SOFTWARE.

Imports System.Windows.Forms
Imports System.Drawing


Namespace Audio

    Namespace Graphics

        Public Class SoundEditor
            Inherits SplitContainer

            'Sound panels, etc
            Friend WithEvents LeftMainContainer As New SplitContainer

            Friend WithEvents SoundContainerPanel As New Panel
            Friend WithEvents SoundBackgroundArea As New System.Windows.Forms.PictureBox
            Friend WithEvents SpectrogramArea As New System.Windows.Forms.PictureBox
            Friend WithEvents WaveArea As New System.Windows.Forms.PictureBox
            Friend WithEvents TimeArea As New System.Windows.Forms.PictureBox

            Private WithEvents SoundScrollBar As New HScrollBar
            Friend WithEvents SegmentationItemsPanel As New FlowLayoutPanel

            Private RightMainContainer As New TableLayoutPanel
            Private SentenceSelectorPanel As New FlowLayoutPanel
            Private WordSelectorPanel As New FlowLayoutPanel
            Private PhonemeSelectorPanel As New FlowLayoutPanel


            'Other panels, etc
            Private WithEvents SoundDisplayContextMenu As New ContextMenuStrip

            'Panel settings
            Private DisplayTypeCount As Integer = 1

            'Variables for which part of the sound that should be displayed and selected/highlighted
            Private TimeUnit As TimeUnits = TimeUnits.seconds

            Private SelectionStart_Sample As Integer
            Private SelectionLength_Sample As Integer
            Private SelectionStart_Pixel As Single
            Private SelectionLength_Pixel As Single
            Public Property DisplayStart_Sample As Integer
            Public Property DisplayLength_Samples As Integer

            Public Function GetSelectionStartSample() As Integer
                Return SelectionStart_Sample
            End Function

            Public Function GetselectionLength() As Integer
                Return SelectionLength_Sample
            End Function

            'Graphic settings
            Private GraphicUpdateFrequency As Integer = 10

            'Sound data
            Private CurrentSound As Sound
            Private SoundBackUp As Sound
            Private FS_pos As Double
            Private CurrentChannel As Integer
            Private SampleToPixelScale As Single
            Private WavePointsArray(1, 0) As Single 'used for long sounds, holding the max and min values of for a section of the wave file, in order to draw a vertical line, one pixel wide, between them, repressenting the wave form.
            Private WaveLinePointArray() As System.Drawing.PointF = {} 'used for short sounds
            Private NormalizedWavePointsArray(1, 0) As Single 'used for long sounds
            Private NormalizedWaveLinePointArray() As System.Drawing.PointF = {} 'used for short sounds
            Private SelectionCopy() As Single

            'Wave settings
            Private DrawEverySampleLimitFactor As Integer = 10 '    2000  'TODO: One of the update functions is not synchonized!!! 'This is a factor that sets how many more samples than pixels there should be on the screen in order to swap wave drawing technique
            Private DrawNormalizedWave As Boolean = False 'This determines if the normalized wave sound be drawn in the background

            'Spectrogram data
            Private ShowSpectrogram As Boolean
            Private SpectrogramFormat As Formats.SpectrogramFormat
            Private SpectrogramWindowDistance As Integer
            Private SpectrogramDisplayDataArray As SpectrogramDisplayData()
            Private SpectrogramLightFactor As Single = 1
            Friend WithEvents SpectrogramLightFactorScrollBar As New VScrollBar

            'Variables for assigning phoneme data
            Private UseItemSegmentation As Boolean
            'Dim currentSmaData As Sound.SpeechMaterialAnnotation ' List(Of Sound.ptwfData.SmaWordData)
            Private CurrentSentenceIndex As Integer = 0 'This holds the index of the sentence in each sound.
            Private CurrentWordIndex As Integer = 0
            Private CurrentPhonemeIndex As Integer

            Private PaddingTime As Single        'padding time should be in seconds
            Private InterSentenceTime As Single

            Public Function SetPaddingTime(ByVal PaddingSeconds As Single)
                PaddingTime = Math.Max(0, PaddingSeconds)
            End Function

            Public Function SetInterSentenceTime(ByVal InterSentenceTimeSeconds As Single)
                InterSentenceTimeSeconds = Math.Max(0, InterSentenceTimeSeconds)
            End Function

            Private SetSegmentationToZeroCrossings As Boolean

            Private CurrentSegmentationItem As Audio.Sound.SpeechMaterialAnnotation.SmaComponent = Nothing

            Private AllSegmentationComponents As New List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)

            'Buttons
            Public ShowPlaySoundButton As Boolean
            Public ShowInferEndsButton As Boolean
            Public ShowNextUnvalidatedItemButtons As Boolean
            Public ShowValidateSegmentationButton As Boolean
            Public ShowFadePaddingButton As Boolean
            Public ShowFadeIntervalsButton As Boolean

            Private Sub CreateContextMenu()

                Dim menuItemNameList As New List(Of String) From {"Play", "PlayAll", "StopSound", "ZoomOut", "ZoomIn", "ZoomToSelection", "ZoomFull", "SmoothFadeIn", "SmoothFadeOut", "LinearFadeIn", "LinearFadeOut", "SilenceSelection", "SilenceSelectionZeroCross", "Copy", "Cut", "Paste", "Delete", "Crop", "UndoAll", "SetAudioApiSettings"}
                Dim menuItemTextList As New List(Of String) From {"Play", "Play all", "Stop", "Zoom out", "Zoom in", "Zoom to selection", "Zoom full", "Fade in selection (smooth)", "Fade out selection (smooth)", "Fade in selection (linear)", "Fade out selection (linear)", "Silence selection", "Silence selection (search zero crossings)", "Copy", "Cut", "Paste", "Delete", "Crop", "Undo all", "New audio settings"}

                For item = 0 To menuItemNameList.Count - 1
                    Dim menuItem As New ToolStripMenuItem
                    menuItem.Name = menuItemNameList(item)
                    menuItem.Text = menuItemTextList(item)
                    SoundDisplayContextMenu.Items.Add(menuItem)
                Next

                'AddHandler soundDisplayContextMenu.ItemClicked, AddressOf menuItem_CLick

            End Sub


            'Setting things up
            Public Sub New(ByRef InputSound As Sound, Optional ByVal StartSample As Integer = 0, Optional ByVal LengthInSamples As Integer? = Nothing, Optional ByVal ViewChannel As Integer = 1,
                           Optional ByVal UseItemSegmentation As Boolean = False, Optional ByVal ShowSpectrogram As Boolean = False,
                    Optional ByRef SpectrogramFormat As Formats.SpectrogramFormat = Nothing, Optional ByRef PaddingTime As Single = 0.5, Optional ByRef InterSentenceTime As Single = 4,
                    Optional ByRef DrawNormalizedWave As Boolean = False, Optional ByRef SetSegmentationToZeroCrossings As Boolean = True,
                           Optional ByRef ShowPlaySoundButton As Boolean = True, Optional ShowInferLengthsButton As Boolean = True,
                    Optional ByRef ShowNextUnvalidatedItemButtons As Boolean = True, Optional ByRef ShowValidateSegmentationButton As Boolean = True,
                           Optional ByRef ShowFadePaddingButton As Boolean = True, Optional ByRef ShowFadeIntervalsButton As Boolean = True)

                Me.CurrentSound = InputSound
                Me.DisplayStart_Sample = StartSample
                Me.CurrentChannel = ViewChannel 'Setting channel (the viewer currently only supports display of one channel at a time) ' This should be changed so that the container can display stereo channels
                Me.UseItemSegmentation = UseItemSegmentation
                Me.ShowSpectrogram = ShowSpectrogram
                Me.SpectrogramFormat = SpectrogramFormat
                Me.PaddingTime = PaddingTime
                Me.InterSentenceTime = InterSentenceTime
                Me.DrawNormalizedWave = DrawNormalizedWave
                'Me.AudioApiSettings = AudioApiSettings

                'If Me.AudioApiSettings IsNot Nothing Then SetupSoundPlayer()

                Me.SetSegmentationToZeroCrossings = SetSegmentationToZeroCrossings

                Me.ShowPlaySoundButton = ShowPlaySoundButton
                Me.ShowInferEndsButton = ShowInferLengthsButton
                Me.ShowNextUnvalidatedItemButtons = ShowNextUnvalidatedItemButtons
                Me.ShowValidateSegmentationButton = ShowValidateSegmentationButton
                Me.ShowFadePaddingButton = ShowFadePaddingButton
                Me.ShowFadeIntervalsButton = ShowFadeIntervalsButton


                If LengthInSamples.HasValue = False Then LengthInSamples = CurrentSound.WaveData.SampleData(CurrentChannel).Length
                If LengthInSamples < 2 Then LengthInSamples = 2
                Me.DisplayLength_Samples = LengthInSamples

                'Creatig  back-up copy of the input sound
                SoundBackUp = CurrentSound.CreateCopy

                'Setting full scale
                FS_pos = CurrentSound.WaveFormat.PositiveFullScale



                'Setting up layout
                Me.Orientation = Orientation.Vertical
                Me.Panel1.Controls.Add(LeftMainContainer)
                LeftMainContainer.Dock = DockStyle.Fill


                If UseItemSegmentation = True Then

                    Me.SplitterDistance = 3 * Me.Width / 4

                    Me.Panel2.Controls.Add(RightMainContainer)
                    RightMainContainer.Dock = DockStyle.Fill

                    SentenceSelectorPanel.Dock = DockStyle.Fill
                    SentenceSelectorPanel.FlowDirection = FlowDirection.TopDown
                    SentenceSelectorPanel.AutoScroll = True
                    SentenceSelectorPanel.AutoSize = True
                    SentenceSelectorPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink

                    WordSelectorPanel.Dock = DockStyle.Fill
                    WordSelectorPanel.FlowDirection = FlowDirection.LeftToRight
                    WordSelectorPanel.AutoScroll = True
                    WordSelectorPanel.AutoSize = True
                    WordSelectorPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink

                    PhonemeSelectorPanel.Dock = DockStyle.Fill
                    PhonemeSelectorPanel.FlowDirection = FlowDirection.LeftToRight
                    PhonemeSelectorPanel.AutoScroll = True
                    PhonemeSelectorPanel.AutoSize = True
                    PhonemeSelectorPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink

                Else

                    Me.Panel2Collapsed = True
                End If



                SoundContainerPanel.Dock = DockStyle.Fill
                LeftMainContainer.Orientation = Orientation.Horizontal
                LeftMainContainer.Panel1.Controls.Add(SoundContainerPanel)
                LeftMainContainer.IsSplitterFixed = True
                If Me.UseItemSegmentation = True Then
                    LeftMainContainer.Panel2.Controls.Add(SegmentationItemsPanel)
                    SegmentationItemsPanel.Height = 50 '68
                    LeftMainContainer.SplitterDistance = LeftMainContainer.Height - SegmentationItemsPanel.Height
                Else
                    LeftMainContainer.SplitterDistance = LeftMainContainer.Height
                End If



                'Setting realtions between panels and pictureboxes
                SoundBackgroundArea.Dock = DockStyle.Top
                SoundContainerPanel.Controls.Add(SoundBackgroundArea)
                If Me.ShowSpectrogram = True Then

                    SoundBackgroundArea.Controls.Add(SpectrogramArea)
                    SpectrogramArea.BackColor = Color.Transparent
                    SpectrogramArea.BringToFront()
                    SpectrogramArea.Controls.Add(SpectrogramLightFactorScrollBar)
                    SpectrogramLightFactorScrollBar.Dock = DockStyle.Right

                    'Setting the value of the spectrogram light bar
                    SpectrogramLightFactorScrollBar.Minimum = 0
                    SpectrogramLightFactorScrollBar.Maximum = 100
                    SpectrogramLightFactorScrollBar.Value = SpectrogramLightFactorScrollBar.Maximum - 20 - SpectrogramLightFactor
                Else

                End If
                SoundBackgroundArea.Controls.Add(WaveArea)
                WaveArea.BackColor = Color.Transparent
                WaveArea.BringToFront()
                SoundBackgroundArea.Controls.Add(TimeArea)
                TimeArea.BackColor = Color.Transparent
                TimeArea.BringToFront()
                SoundContainerPanel.Controls.Add(SoundScrollBar)
                SoundScrollBar.Dock = DockStyle.Bottom

                'Adding borders to the sound boxes
                TimeArea.BorderStyle = BorderStyle.FixedSingle
                WaveArea.BorderStyle = BorderStyle.FixedSingle
                SpectrogramArea.BorderStyle = BorderStyle.FixedSingle
                SoundBackgroundArea.BorderStyle = BorderStyle.Fixed3D

                'calculating spectrogram data
                'Creating a temporary setting
                If Me.ShowSpectrogram = True Then
                    DisplayTypeCount += 1

                    'Creating a default spectrogram format if needed
                    If Me.SpectrogramFormat Is Nothing Then Me.SpectrogramFormat = New Formats.SpectrogramFormat(,,,,,,,,, True)

                    SpectrogramWindowDistance = Me.SpectrogramFormat.SpectrogramFftFormat.AnalysisWindowSize - Me.SpectrogramFormat.SpectrogramFftFormat.OverlapSize

                End If

                'Calculating FFT
                If Me.ShowSpectrogram = True Then
                    CurrentSound.FFT = New FftData(CurrentSound.WaveFormat, Me.SpectrogramFormat.SpectrogramFftFormat)
                    CurrentSound.FFT.CalculateSpectrogramData(CurrentSound, Me.SpectrogramFormat, CurrentChannel)
                End If

                'Loading phoneme data
                If Me.UseItemSegmentation = True Then
                    SegmentationItemsPanel.FlowDirection = FlowDirection.LeftToRight
                    SegmentationItemsPanel.Dock = DockStyle.Bottom
                    'phonemesContainerPanel.AutoSize = True
                    'phonemesContainerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink
                    SegmentationItemsPanel.AutoScroll = True
                    LeftMainContainer.Panel2MinSize = 30
                    'LoadAndModifyPhonemeLevelData()
                Else
                    LeftMainContainer.Panel2MinSize = 0
                    LeftMainContainer.SplitterDistance = LeftMainContainer.Height
                End If

                'Adding event handler for spectrogram. Paint events for other drawing areas are declared by Handles statments on the appropriate subs
                If Me.ShowSpectrogram = True Then
                    AddHandler SpectrogramArea.Paint, AddressOf Me.DrawSpectrogram
                End If

                If Me.UseItemSegmentation = True Then
                    LoadAndModifyPhonemeLevelData()
                End If

                'Creating the context menu
                CreateContextMenu()
                UpdateLayout()

                'Adding handlers for the wave and spectrogram areas
                AddHandler WaveArea.MouseDown, AddressOf Me.Container_MouseDown
                If Me.ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseDown, AddressOf Me.Container_MouseDown


            End Sub


            Private Sub LoadAndModifyPhonemeLevelData()

                If CurrentSound.SMA.ChannelData(CurrentChannel).Count > 0 Then

                    RightMainContainer.Controls.Clear()

                    'Adding one column
                    RightMainContainer.ColumnCount = 1
                    RightMainContainer.ColumnStyles.Add(New Windows.Forms.ColumnStyle(Windows.Forms.SizeType.Percent, 100))

                    'Adding all three selection containers
                    RightMainContainer.RowCount = 3
                    RightMainContainer.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Percent, 55))
                    RightMainContainer.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Percent, 30))
                    RightMainContainer.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Percent, 15))

                    Dim SentenceGroupBox As New GroupBox With {.Text = "Select sentence", .Dock = DockStyle.Fill}
                    SentenceGroupBox.Controls.Add(SentenceSelectorPanel)
                    RightMainContainer.Controls.Add(SentenceGroupBox)

                    Dim WordGroupBox As New GroupBox With {.Text = "Select word", .Dock = DockStyle.Fill}
                    WordGroupBox.Controls.Add(WordSelectorPanel)
                    RightMainContainer.Controls.Add(WordGroupBox)

                    Dim PhonemeGroupBox As New GroupBox With {.Text = "Select phone", .Dock = DockStyle.Fill}
                    PhonemeGroupBox.Controls.Add(PhonemeSelectorPanel)
                    RightMainContainer.Controls.Add(PhonemeGroupBox)

                    AllSegmentationComponents = CurrentSound.SMA.ChannelData(CurrentChannel).GetAllDescentantComponents

                    AddSentences()

                End If

            End Sub

            Private Sub AddSentences()

                SentenceSelectorPanel.Controls.Clear()

                For sentenceIndex = 0 To CurrentSound.SMA.ChannelData(CurrentChannel).Count - 1
                    Dim SentenceLabel As New SegmentationItemLabel

                    Dim SentenceStringRepresentation = CurrentSound.SMA.ChannelData(CurrentChannel)(sentenceIndex).GetStringRepresentation
                    If SentenceStringRepresentation = "" Then SentenceStringRepresentation = "Sentence " & sentenceIndex + 1

                    With SentenceLabel
                        .Text = SentenceStringRepresentation
                        .Name = sentenceIndex.ToString 'storing the identity of the sentence as an index that can be used to set the CurrentSentenceIndex 
                        .TextAlign = ContentAlignment.MiddleCenter
                        .BackColor = Color.White
                        .AutoSize = True
                        .Padding = New Padding(2)
                        .Font = New Font("Arial", 12.0F, FontStyle.Regular) 'TODO: It would be good to be able to change this font family, and size
                        .SegmentationItem = CurrentSound.SMA.ChannelData(CurrentChannel)(sentenceIndex)
                        .TextAlign = ContentAlignment.TopLeft
                    End With

                    'Adding eventhandler
                    AddHandler SentenceLabel.Click, AddressOf SentenceLabelButtonClick

                    'Adding margin
                    SentenceLabel.Margin = New Padding(2)

                    'Adding the control
                    SentenceSelectorPanel.Controls.Add(SentenceLabel)
                Next

                'If there is only one sentence, selecting it right away
                If CurrentSound.SMA.ChannelData(CurrentChannel).Count = 1 Then
                    If SentenceSelectorPanel.Controls.Count = 1 Then
                        SentenceLabelButtonClick(SentenceSelectorPanel.Controls.Item(0), Nothing)
                    End If
                End If

            End Sub

            Private Sub AddWords()

                WordSelectorPanel.Controls.Clear()

                For wordIndex = 0 To CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex).Count - 1
                    Dim WordLabel As New SegmentationItemLabel

                    With WordLabel
                        .Text = CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(wordIndex).OrthographicForm
                        .Name = wordIndex.ToString 'storing the identity of the sentence as an index that can be used to set the CurrentSentenceIndex 
                        .TextAlign = ContentAlignment.MiddleCenter
                        .BackColor = Color.White
                        .AutoSize = True
                        .Padding = New Padding(2)
                        .Font = New Font("Arial", 14.0F, FontStyle.Regular) 'TODO: It would be good to be able to change this font family, and size
                        .SegmentationItem = CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(wordIndex)
                        .TextAlign = ContentAlignment.TopLeft
                    End With

                    'Adding eventhandler
                    AddHandler WordLabel.Click, AddressOf WordLabelButtonClick

                    'Adding margin
                    WordLabel.Margin = New Padding(2)

                    'Adding the control
                    WordSelectorPanel.Controls.Add(WordLabel)
                Next

                'If there is only one word in the current sentence, selecting it right away. Otherwise, emptying the phoneme box (otherwise the previously shown phonemes will be shown there)
                Dim ClearPhonemes As Boolean = True
                If CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex).Count = 1 Then
                    If WordSelectorPanel.Controls.Count = 1 Then
                        WordLabelButtonClick(WordSelectorPanel.Controls.Item(0), Nothing)
                        ClearPhonemes = False
                    End If
                End If

                If ClearPhonemes = True Then
                    PhonemeSelectorPanel.Controls.Clear()
                End If

            End Sub


            Private Sub AddPhones()

                PhonemeSelectorPanel.Controls.Clear()

                For phoneIndex = 0 To CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex).Count - 1
                    Dim PhoneLabel As New SegmentationItemLabel

                    With PhoneLabel
                        .Text = "[ " & CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(phoneIndex).PhoneticForm & " ]"
                        .Name = phoneIndex.ToString 'storing the identity of the sentence as an index that can be used to set the CurrentSentenceIndex 
                        .TextAlign = ContentAlignment.MiddleCenter
                        .BackColor = Color.White
                        .AutoSize = True
                        .Padding = New Padding(4)
                        .Font = New Font("Arial", 14.0F, FontStyle.Regular) 'TODO: It would be good to be able to change this font family, and size
                        .SegmentationItem = CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(phoneIndex)
                        .TextAlign = ContentAlignment.MiddleCenter
                    End With

                    'Adding eventhandler
                    AddHandler PhoneLabel.Click, AddressOf PhoneLabelButtonClick

                    'Adding margin
                    PhoneLabel.Margin = New Padding(2)

                    'Adding the control
                    PhonemeSelectorPanel.Controls.Add(PhoneLabel)
                Next

            End Sub


            Private Sub SentenceLabelButtonClick(sender As Object, ByVal e As MouseEventArgs)

                'This sub handles clicking on the sentence labels
                CurrentSentenceIndex = sender.name
                    CurrentWordIndex = 0
                    CurrentPhonemeIndex = 0

                    'Resets the colors of the buttons
                    For Each item As Control In SentenceSelectorPanel.Controls
                        item.BackColor = Color.White
                    Next
                    For Each item As Control In WordSelectorPanel.Controls
                        item.BackColor = Color.White
                    Next
                    For Each item As Control In PhonemeSelectorPanel.Controls
                        item.BackColor = Color.White
                    Next

                    sender.BackColor = Color.LightGreen

                    'Setting the current segmentation item
                    CurrentSegmentationItem = CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)

                    AddSegmentationControls()

                    If CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex).Count > 0 Then
                        AddWords()
                    End If

                    InvalidateGraphics()

                'Also plays the item on right click
                If e IsNot Nothing Then
                    If e.Button = MouseButtons.Right Then
                        SegmentationItemPlay()
                    End If
                End If

            End Sub



            Private Sub WordLabelButtonClick(sender As Object, ByVal e As MouseEventArgs)

                'This sub handles clicking on the Word labels
                CurrentWordIndex = sender.name
                CurrentPhonemeIndex = 0

                'Resets the colors of the buttons
                For Each item As Control In WordSelectorPanel.Controls
                    item.BackColor = Color.White
                Next
                For Each item As Control In PhonemeSelectorPanel.Controls
                    item.BackColor = Color.White
                Next

                sender.BackColor = Color.LightGreen

                'Setting the current segmentation item
                CurrentSegmentationItem = CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)

                AddSegmentationControls()

                If CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex).Count > 0 Then
                    AddPhones()
                End If

                InvalidateGraphics()

                'Also plays the item on right click
                If e IsNot Nothing Then
                    If e.Button = MouseButtons.Right Then
                        SegmentationItemPlay()
                    End If
                End If

            End Sub

            Private Sub PhoneLabelButtonClick(sender As Object, ByVal e As MouseEventArgs)

                'This sub handles clicking on the Phone labels
                CurrentPhonemeIndex = sender.name

                'Resets the colors of the buttons
                For Each item As Control In PhonemeSelectorPanel.Controls
                    item.BackColor = Color.White
                Next

                sender.BackColor = Color.LightGreen

                'Setting the current segmentation item
                CurrentSegmentationItem = CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex)(CurrentPhonemeIndex)

                'Adding phone buttons
                AddSegmentationControls()

                If CurrentSound.SMA.ChannelData(CurrentChannel)(CurrentSentenceIndex)(CurrentWordIndex).Count > 0 Then
                End If

                InvalidateGraphics()

                'Also plays the item on right click
                If e IsNot Nothing Then
                    If e.Button = MouseButtons.Right Then
                        SegmentationItemPlay()
                    End If
                End If

            End Sub

            Private Sub ZoomToParent()

                'Zooming to parent component (with margin)
                Dim HasZoomed As Boolean = False
                If CurrentSegmentationItem IsNot Nothing Then
                    Dim ParentComponent = CurrentSegmentationItem.ParentComponent
                    If ParentComponent IsNot Nothing Then
                        'The parent should be either a channel, a sentence or a word (phonemes do not have any child components)
                        'Assuming that the parent component is segmented if its length has been set
                        If ParentComponent.Length > 0 Then

                            'Zooming to parent
                            Dim ZoomMargin = ParentComponent.Length * 0.2
                            Dim ZoomStart = Math.Max(0, ParentComponent.StartSample - ZoomMargin)
                            Dim ZoomLength = ParentComponent.Length + ZoomMargin + (ParentComponent.StartSample - ZoomStart)
                            ZoomTo(ZoomStart, ZoomLength)
                            HasZoomed = True
                        Else
                            'Looks for a grandparent with length set
                            If ParentComponent.ParentComponent IsNot Nothing Then
                                If ParentComponent.ParentComponent.Length > 0 Then

                                    'Zooming to the grand parent
                                    Dim ZoomMargin = ParentComponent.ParentComponent.Length * 0.2
                                    Dim ZoomStart = Math.Max(0, ParentComponent.ParentComponent.StartSample - ZoomMargin)
                                    Dim ZoomLength = ParentComponent.ParentComponent.Length + ZoomMargin + (ParentComponent.ParentComponent.StartSample - ZoomStart)
                                    ZoomTo(ZoomStart, ZoomLength)
                                    HasZoomed = True
                                End If
                            End If
                        End If
                    End If
                End If

                If HasZoomed = False Then
                    'Zooms out to full sound if no other zoom has been made
                    ZoomFull()
                End If

            End Sub

            Private Sub AddSegmentationControls()

                ZoomToParent()

                Dim CurrentFont = Button.DefaultFont ' New Font("Arial", 9.0F, FontStyle.Regular)
                Dim CurrentMargin = New Windows.Forms.Padding(2, 3, 2, 0)
                Dim ButtonHeight As Single = 22
                Dim SmallButtonWidth As Single = 58
                Dim WideButtonWidth As Single = 120
                SegmentationItemsPanel.Padding = New Padding(0)

                SegmentationItemsPanel.Controls.Clear()

                Dim PreviousItemButton As New Button With {.Text = "Previous", .TextAlign = ContentAlignment.MiddleCenter,
                        .AutoSize = False, .Font = CurrentFont, .Width = SmallButtonWidth, .Height = ButtonHeight, .Margin = CurrentMargin}
                AddHandler PreviousItemButton.Click, AddressOf GotoPreviuosItem
                SegmentationItemsPanel.Controls.Add(PreviousItemButton)

                Dim NextItemButton As New Button With {.Text = "Next", .TextAlign = ContentAlignment.MiddleCenter,
                        .AutoSize = False, .Font = CurrentFont, .Width = SmallButtonWidth, .Height = ButtonHeight, .Margin = CurrentMargin}
                AddHandler NextItemButton.Click, AddressOf GotoNextItem
                SegmentationItemsPanel.Controls.Add(NextItemButton)

                Dim SegmentationItemStartButton As New Button With {.Text = "Set start", .TextAlign = ContentAlignment.MiddleCenter,
                    .AutoSize = False, .Font = CurrentFont, .BackColor = Color.FromArgb(50, Color.LightGreen), .Width = WideButtonWidth, .Height = ButtonHeight, .Margin = CurrentMargin}
                AddHandler SegmentationItemStartButton.Click, AddressOf SegmentationItemStartButton_Click
                SegmentationItemsPanel.Controls.Add(SegmentationItemStartButton)

                Dim SegmentationItemEndButton As New Button With {.Text = "Set end", .TextAlign = ContentAlignment.MiddleCenter,
                    .AutoSize = False, .Font = CurrentFont, .BackColor = Color.FromArgb(50, Color.LightCoral), .Width = WideButtonWidth, .Height = ButtonHeight, .Margin = CurrentMargin}
                AddHandler SegmentationItemEndButton.Click, AddressOf SegmentationItemEndButton_Click
                SegmentationItemsPanel.Controls.Add(SegmentationItemEndButton)


                'Adding a play button
                If ShowPlaySoundButton = True Then
                    Dim PlaySectionButton As New WinFormControls.AudioButton With {.Enabled = True, .ViewMode = WinFormControls.AudioButton.ViewModes.Play,
                        .AutoSize = False, .Width = SmallButtonWidth, .Height = ButtonHeight, .Margin = CurrentMargin}
                    AddHandler PlaySectionButton.Click, AddressOf SegmentationItemPlay
                    SegmentationItemsPanel.Controls.Add(PlaySectionButton)
                End If

                If ShowInferEndsButton = True Then
                    Dim SyncEndsButton As New Button With {.Text = "Sync ends", .TextAlign = ContentAlignment.MiddleCenter,
                        .AutoSize = False, .Font = CurrentFont, .Width = WideButtonWidth, .Height = ButtonHeight, .Margin = CurrentMargin}
                    AddHandler SyncEndsButton.Click, AddressOf InferSiblingLengths
                    SegmentationItemsPanel.Controls.Add(SyncEndsButton)
                End If

                If ShowNextUnvalidatedItemButtons = True Then

                    Dim PreviousUnvalidatedItemButton As New Button With {.Text = "Previous unvalidated", .TextAlign = ContentAlignment.MiddleCenter,
                            .AutoSize = False, .Font = CurrentFont, .Width = WideButtonWidth, .Height = ButtonHeight, .Margin = CurrentMargin}
                    AddHandler PreviousUnvalidatedItemButton.Click, AddressOf GotoPreviousUnvalidatedItem
                    SegmentationItemsPanel.Controls.Add(PreviousUnvalidatedItemButton)

                    Dim NextUnvalidatedItemButton As New Button With {.Text = "Next unvalidated", .TextAlign = ContentAlignment.MiddleCenter,
                            .AutoSize = False, .Font = CurrentFont, .Width = WideButtonWidth, .Height = ButtonHeight, .Margin = CurrentMargin}
                    AddHandler NextUnvalidatedItemButton.Click, AddressOf GotoNextUnvalidatedItem
                    SegmentationItemsPanel.Controls.Add(NextUnvalidatedItemButton)

                End If

                If ShowValidateSegmentationButton = True Then
                    Dim ValidateSegmentationButton As New Button With {.Text = "Validate", .TextAlign = ContentAlignment.MiddleCenter,
                        .AutoSize = False, .Font = CurrentFont, .Width = WideButtonWidth, .Height = ButtonHeight, .Margin = CurrentMargin}
                    AddHandler ValidateSegmentationButton.Click, AddressOf ValidateSegmentation
                    SegmentationItemsPanel.Controls.Add(ValidateSegmentationButton)
                End If

                Dim ShowAutoDetectSentencesIntervalsButton As Boolean = True
                If ShowAutoDetectSentencesIntervalsButton = True Then
                    Dim AutoCropButton As New Button With {.Text = "Auto-detect sentence start/end", .TextAlign = ContentAlignment.MiddleCenter,
                        .AutoSize = False, .Font = CurrentFont, .Width = WideButtonWidth, .Height = ButtonHeight, .Margin = CurrentMargin}
                    AddHandler AutoCropButton.Click, AddressOf AutoSetSentenceSegmentation
                    SegmentationItemsPanel.Controls.Add(AutoCropButton)
                End If

                If ShowFadeIntervalsButton = True Then
                    Dim FixIntervalsButton As New Button With {.Text = "Fix intervals", .TextAlign = ContentAlignment.MiddleCenter,
                        .AutoSize = False, .Font = CurrentFont, .Width = WideButtonWidth, .Height = ButtonHeight, .Margin = CurrentMargin}
                    AddHandler FixIntervalsButton.Click, AddressOf FixIntervals
                    SegmentationItemsPanel.Controls.Add(FixIntervalsButton)
                End If

                If ShowFadeIntervalsButton = True Then
                    Dim FixPaddingButton As New Button With {.Text = "Fix padding", .TextAlign = ContentAlignment.MiddleCenter,
                        .AutoSize = False, .Font = CurrentFont, .Width = WideButtonWidth, .Height = ButtonHeight, .Margin = CurrentMargin}
                    AddHandler FixPaddingButton.Click, AddressOf FixPadding
                    SegmentationItemsPanel.Controls.Add(FixPaddingButton)
                End If

                Dim ShowSetInitialPeakAmplitudesButton As Boolean = True
                If ShowSetInitialPeakAmplitudesButton = True Then
                    Dim SetPeakButton As New Button With {.Text = "Store initial peak", .TextAlign = ContentAlignment.MiddleCenter,
                        .AutoSize = False, .Font = CurrentFont, .Width = WideButtonWidth, .Height = ButtonHeight, .Margin = CurrentMargin}
                    AddHandler SetPeakButton.Click, AddressOf SetInitialPeak
                    SegmentationItemsPanel.Controls.Add(SetPeakButton)
                End If


            End Sub

            'Resetting data
            Private Sub ResetCurrentWordLevelSegmentationData()

                CurrentSound.SMA.ChannelData(CurrentChannel).ResetTemporalData()

            End Sub
            Private Sub RetriveSoundBackUp()

                'Undoes all modifications to the input sound and restores the input sound array
                CurrentSound = SoundBackUp.CreateCopy

                If ShowSpectrogram = True Then UpdateSpectrogramData()

                UpdateLayout()

            End Sub

            Private Sub UpdateSpectrogramData()

                'Updating fft data
                CurrentSound.FFT = New FftData(CurrentSound.WaveFormat, SpectrogramFormat.SpectrogramFftFormat)
                CurrentSound.FFT.CalculateSpectrogramData(CurrentSound, SpectrogramFormat, CurrentChannel)

            End Sub

            'Calculating sound visualization data (and container sizes)
            Private Sub UpdateSampleTimeScale()

                'Calculates how many samples each pixel repressents
                SampleToPixelScale = (DisplayLength_Samples - 1) / SoundBackgroundArea.Width

            End Sub


            Private Sub UpdateLayout()

                'This sub recalculates all sound display data, and container positions and sizes

                Try

                    'Setting the number of samples to display
                    If DisplayStart_Sample < 0 Then DisplayStart_Sample = 0
                    If DisplayStart_Sample > CurrentSound.WaveData.SampleData(CurrentChannel).Length - 1 Then DisplayStart_Sample = CurrentSound.WaveData.SampleData(CurrentChannel).Length - 1
                    If DisplayStart_Sample + DisplayLength_Samples > CurrentSound.WaveData.SampleData(CurrentChannel).Length Then DisplayLength_Samples = CurrentSound.WaveData.SampleData(CurrentChannel).Length - DisplayStart_Sample

                    'Sets the scroll bar scale
                    SoundScrollBar.Minimum = 0
                    SoundScrollBar.Maximum = CurrentSound.WaveData.SampleData(CurrentChannel).Length - DisplayLength_Samples
                    SoundScrollBar.Value = DisplayStart_Sample


                    Dim drawAreaHeight = SoundContainerPanel.Height - SoundScrollBar.Height
                    'If drawAreaHeight < buttonPanel.Height - hScBar.Height Then instanceContainerPanel.Height = drawAreaHeight + buttonPanel.Height + hScBar.Height
                    SoundBackgroundArea.Height = drawAreaHeight

                    SoundBackgroundArea.Top = 0
                    SoundScrollBar.Top = SoundBackgroundArea.Bottom
                    'buttonPanel.Top = soundScrollBar.Bottom


                    'Updating size and location of the controls inside soundBackgroundArea
                    TimeArea.Height = 20
                    TimeArea.Width = SoundBackgroundArea.Width
                    Select Case DisplayTypeCount
                        Case 1
                            WaveArea.Top = 0
                            WaveArea.Height = SoundBackgroundArea.Height - TimeArea.Height
                            WaveArea.Width = SoundBackgroundArea.Width
                            TimeArea.Top = WaveArea.Bottom
                        Case 2
                            WaveArea.Top = 0
                            WaveArea.Width = SoundBackgroundArea.Width
                            WaveArea.Height = (SoundBackgroundArea.Height - TimeArea.Height) / 2

                            SpectrogramArea.Width = SoundBackgroundArea.Width
                            SpectrogramArea.Height = (SoundBackgroundArea.Height - TimeArea.Height) / 2
                            SpectrogramArea.Top = WaveArea.Height
                            TimeArea.Top = SpectrogramArea.Bottom

                        Case 3
                            Throw New NotImplementedException

                    End Select

                    'Updates the scale according to the current size of the sound display
                    UpdateSampleTimeScale()
                    'Updating Y-scale
                    Dim YscaleToPixel_Wave As Single
                    YscaleToPixel_Wave = (WaveArea.Height / 2) / FS_pos


                    'Updating wave data - Chosing method to update depending on the size of the sound to display
                    Select Case DisplayLength_Samples
                        Case Is > WaveArea.Width * DrawEverySampleLimitFactor
                            'timeArea.BackColor = Color.Red
                            ReDim WavePointsArray(1, WaveArea.Width)

                            Dim sectionMax As Double
                            Dim sectionMin As Double

                            For CurrentXpixel As Single = 0 To WaveArea.Width - 1 Step 1

                                'find section max and min
                                sectionMax = -FS_pos
                                sectionMin = FS_pos
                                For sample = CInt((CurrentXpixel * SampleToPixelScale) + DisplayStart_Sample) To CInt((CurrentXpixel + 1) * SampleToPixelScale + DisplayStart_Sample - 1)
                                    If CurrentSound.WaveData.SampleData(CurrentChannel)(sample) < sectionMin Then sectionMin = CurrentSound.WaveData.SampleData(CurrentChannel)(sample)
                                    If CurrentSound.WaveData.SampleData(CurrentChannel)(sample) > sectionMax Then sectionMax = CurrentSound.WaveData.SampleData(CurrentChannel)(sample)
                                Next

                                WavePointsArray(0, CurrentXpixel) = ((WaveArea.Height) / 2) - (sectionMax * YscaleToPixel_Wave)
                                WavePointsArray(1, CurrentXpixel) = ((WaveArea.Height) / 2) - (sectionMin * YscaleToPixel_Wave)
                            Next

                        Case Else
                            'timeArea.BackColor = Color.Cyan
                            ReDim WaveLinePointArray(DisplayLength_Samples - 1)
                            Dim currentPoint As New System.Drawing.PointF(0, 0)
                            For point = 0 To WaveLinePointArray.Length - 1

                                currentPoint.X = point / SampleToPixelScale
                                currentPoint.Y = ((WaveArea.Height) / 2) - (CurrentSound.WaveData.SampleData(CurrentChannel)(point + DisplayStart_Sample) * YscaleToPixel_Wave)
                                WaveLinePointArray(point) = currentPoint

                            Next
                    End Select

                    If DrawNormalizedWave = True Then

                        'TODO: find out what the max value is
                        Dim sectionAbsMaxValue As Single = DSP.MeasureSectionLevel(CurrentSound, CurrentChannel, DisplayStart_Sample, DisplayLength_Samples, SoundDataUnit.linear,
                                                  SoundMeasurementType.AbsolutePeakAmplitude)

                        'Updating the Creating a new scale
                        Dim normalizedYscaleToPixel_Wave = (WaveArea.Height / 2) / sectionAbsMaxValue

                        'Create a normalised version of the wavedata above
                        'Updating wave data - Chosing method to update depending on the size of the sound to display
                        Select Case DisplayLength_Samples
                            Case Is > WaveArea.Width * DrawEverySampleLimitFactor
                                'timeArea.BackColor = Color.Red
                                ReDim NormalizedWavePointsArray(1, WaveArea.Width - 1)
                                Dim sectionMax As Double
                                Dim sectionMin As Double
                                For CurrentXpixel = 0 To NormalizedWavePointsArray.GetUpperBound(1) - 2

                                    'find section max and min
                                    sectionMax = -FS_pos
                                    sectionMin = FS_pos
                                    For sample = CInt(CurrentXpixel * SampleToPixelScale + DisplayStart_Sample) To CInt((CurrentXpixel + 1) * SampleToPixelScale + DisplayStart_Sample - 1)
                                        If CurrentSound.WaveData.SampleData(CurrentChannel)(sample) < sectionMin Then sectionMin = CurrentSound.WaveData.SampleData(CurrentChannel)(sample)
                                        If CurrentSound.WaveData.SampleData(CurrentChannel)(sample) > sectionMax Then sectionMax = CurrentSound.WaveData.SampleData(CurrentChannel)(sample)
                                    Next

                                    NormalizedWavePointsArray(0, CurrentXpixel) = ((WaveArea.Height) / 2) - (sectionMax * normalizedYscaleToPixel_Wave)
                                    NormalizedWavePointsArray(1, CurrentXpixel) = ((WaveArea.Height) / 2) - (sectionMin * normalizedYscaleToPixel_Wave)
                                Next

                                'Adds the last bit
                                'find section max and min
                                'sectionMax = -FS_pos
                                'sectionMin = FS_pos
                                'Dim LastPoint As Integer = pointsArray.GetUpperBound(1) - 2
                                'For sample = LastPoint * Int(XscaleToPixel) + displayStartSample To displayStartSample + displayLengthSamples - 1
                                'If sound(sample) < sectionMin Then sectionMin = sound(sample)
                                'If sound(sample) > sectionMax Then sectionMax = sound(sample)
                                'Next
                                'pointsArray(0, LastPoint) = container.Bottom - (sectionMax * YscaleToPixel + (container.Bottom / 2))
                                'pointsArray(1, LastPoint) = container.Bottom - (sectionMin * YscaleToPixel + (container.Bottom / 2))


                            Case Else
                                'timeArea.BackColor = Color.Cyan
                                ReDim NormalizedWaveLinePointArray(DisplayLength_Samples - 1)
                                Dim currentPoint As New System.Drawing.PointF(0, 0)
                                For point = 0 To NormalizedWaveLinePointArray.Length - 1

                                    currentPoint.X = point / SampleToPixelScale
                                    currentPoint.Y = ((WaveArea.Height) / 2) - (CurrentSound.WaveData.SampleData(CurrentChannel)(point + DisplayStart_Sample) * normalizedYscaleToPixel_Wave)
                                    NormalizedWaveLinePointArray(point) = currentPoint
                                Next

                        End Select


                    End If


                    If ShowSpectrogram = True Then
                        ReCalculateSpectrogramData()
                    End If

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

                InvalidateGraphics()

                'RefreshGraphicElements()

            End Sub

            Private Sub ReCalculateSpectrogramData()

                If CurrentSound Is Nothing Then Exit Sub
                If CurrentSound.FFT Is Nothing Then Exit Sub

                Try

                    'Updating spectrogram data

                    Dim YscaleToPixel_Spectrogram As Single
                    YscaleToPixel_Spectrogram = SpectrogramArea.Height / CurrentSound.FFT.SpectrogramData(CurrentChannel, 0).WindowData.Length

                    'Localizing the position of the first spectrogram window to display
                    Dim displayStartColumn As Integer = Utils.Rounding(DisplayStart_Sample / SpectrogramWindowDistance, Utils.roundingMethods.alwaysDown)
                    Dim samplesOfFirstWindowOutsideDisplay As Integer = (displayStartColumn * SpectrogramWindowDistance) - DisplayStart_Sample
                    Dim pixelsOfFirstWindowOutsideDisplay As Single = (samplesOfFirstWindowOutsideDisplay / SampleToPixelScale)

                    'Localizing the position of the last spectrogram window to display
                    Dim displayLastColumn As Integer = Utils.Rounding((DisplayStart_Sample + DisplayLength_Samples) / SpectrogramWindowDistance, Utils.roundingMethods.alwaysDown)

                    'Calculating number of windows
                    Dim displayColumnCount As Integer = displayLastColumn - displayStartColumn

                    'Selecting update method depending on the width of the presentation panel
                    Select Case displayColumnCount
                        Case Is <= SpectrogramArea.Width

                            'Updating spectrogram X-scale
                            Dim columnsSizeInPixels As Single = SpectrogramWindowDistance / SampleToPixelScale
                            Dim analysisWindowLengthInPixels As Single = SpectrogramWindowDistance / SampleToPixelScale

                            Dim newLowResolutionSpectrogramArea(displayColumnCount * CurrentSound.FFT.SpectrogramData(CurrentChannel, 0).WindowData.Length - 1) As SpectrogramDisplayData

                            'Creating display data
                            Dim drawingSurface As Integer = 0
                            For columnNumber = displayStartColumn To displayStartColumn + displayColumnCount - 1
                                For coeffNumber = 0 To CurrentSound.FFT.SpectrogramData(CurrentChannel, 0).WindowData.Length - 1

                                    Dim newBrushGradient As Integer '= sound.FFT.SpectrogramData(currentChannel, columnNumber)(coeffNumber) * spectrogramLightFactor
                                    newBrushGradient = CurrentSound.FFT.SpectrogramData(CurrentChannel, columnNumber).WindowData(coeffNumber) * SpectrogramLightFactor
                                    If newBrushGradient > 255 Then newBrushGradient = 255
                                    Dim newBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(newBrushGradient, Drawing.Color.Black))

                                    newLowResolutionSpectrogramArea(drawingSurface) = New SpectrogramDisplayData(newBrush, ((columnNumber - displayStartColumn) * columnsSizeInPixels + pixelsOfFirstWindowOutsideDisplay),
                            (SpectrogramArea.Height - coeffNumber * YscaleToPixel_Spectrogram),
                            columnsSizeInPixels, YscaleToPixel_Spectrogram)

                                    drawingSurface += 1
                                Next
                            Next

                            SpectrogramDisplayDataArray = newLowResolutionSpectrogramArea


                        Case Is > SpectrogramArea.Width

                            Dim XPixelCount As Integer = SpectrogramArea.ClientRectangle.Width
                            Dim YPixelCount As Single = SpectrogramArea.ClientRectangle.Height

                            Dim columnScaleToPixel As Single = displayColumnCount / XPixelCount
                            Dim binScaleToPixel As Single = CurrentSound.FFT.SpectrogramData(CurrentChannel, 0).WindowData.Length / YPixelCount

                            Dim newLowResolutionSpectrogramArea(XPixelCount * YPixelCount - 1) As SpectrogramDisplayData

                            Dim drawingPixel As Integer = 0
                            For xPixel = 0 To XPixelCount - 1
                                For yPixel = 0 To YPixelCount - 1

                                    Dim WindowIndex = Math.Floor(displayStartColumn + xPixel * columnScaleToPixel)
                                    Dim BinIndex = Math.Floor(yPixel * binScaleToPixel)

                                    Dim newBrushGradient As Integer = CurrentSound.FFT.SpectrogramData(CurrentChannel, WindowIndex).WindowData(BinIndex) * SpectrogramLightFactor
                                    If newBrushGradient > 255 Then newBrushGradient = 255
                                    Dim newBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(newBrushGradient, Drawing.Color.Black))
                                    newLowResolutionSpectrogramArea(drawingPixel) = New SpectrogramDisplayData(newBrush, xPixel, YPixelCount - yPixel, 1, 1)
                                    drawingPixel += 1

                                Next
                            Next

                            SpectrogramDisplayDataArray = newLowResolutionSpectrogramArea

                    End Select

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try


            End Sub

            'Drawing graphic elements
            Private Sub DrawWave(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles WaveArea.Paint

                Try

                    ' Create a local version of the graphics object for the PictureBox.
                    Dim g As System.Drawing.Graphics = e.Graphics

                    'Drawing normalized wave data
                    If DrawNormalizedWave = True Then

                        ' Create pen.
                        Dim grayPen As New System.Drawing.Pen(System.Drawing.Color.Gray, 1)

                        Select Case DisplayLength_Samples
                            Case Is > WaveArea.Width * DrawEverySampleLimitFactor
                                For points = 0 To NormalizedWavePointsArray.GetUpperBound(1) - 1
                                    g.DrawLine(grayPen, points, NormalizedWavePointsArray(0, points), points, NormalizedWavePointsArray(1, points))
                                Next
                            Case Else
                                g.DrawLines(grayPen, NormalizedWaveLinePointArray)

                        End Select

                    End If


                    ' Create pen.
                    Dim blackPen As New System.Drawing.Pen(System.Drawing.Color.Black, 1)

                    'Draws the wave display type
                    'Draws 0 - line in wave area
                    ' Create points that define 0 line.
                    Dim point1 As New System.Drawing.Point(0, WaveArea.Height / 2)
                    Dim point2 As New System.Drawing.Point(WaveArea.Width, WaveArea.Height / 2)
                    g.DrawLine(blackPen, point1, point2)


                    'Draws wave form
                    Select Case DisplayLength_Samples
                        Case Is > WaveArea.Width * DrawEverySampleLimitFactor
                            For points = 0 To WavePointsArray.GetUpperBound(1) - 1
                                g.DrawLine(blackPen, points, WavePointsArray(0, points), points, WavePointsArray(1, points))
                            Next
                        Case Else
                            g.DrawLines(blackPen, WaveLinePointArray)

                    End Select


                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub
            Private Sub DrawSpectrogram(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles SpectrogramArea.Paint

                Try

                    ' Create a local version of the graphics object for the PictureBox.
                    Dim g As System.Drawing.Graphics = e.Graphics

                    'Draws spectrogram
                    For n = 0 To SpectrogramDisplayDataArray.Count - 1
                        g.FillRectangle(SpectrogramDisplayDataArray(n).BrushColor, SpectrogramDisplayDataArray(n).Area)
                    Next


                    'Draws frequencies on the spectrogram area
                    Dim DiagramValuesBrush = Brushes.Black
                    Dim SpectrogramSettingsBrush As New SolidBrush(Color.FromArgb(150, Color.Black))
                    Dim TickPen As New System.Drawing.Pen(System.Drawing.Color.Black, 2)

                    Dim valuesToWrite As Integer = Utils.Rounding(SpectrogramFormat.SpectrogramCutFrequency / 1000, Utils.roundingMethods.alwaysUp)

                    For n = 0 To valuesToWrite - 1

                        'Drawing frequency numbers
                        g.DrawString((n * 1000).ToString, New Font("Arial", 7), DiagramValuesBrush, New PointF(10, SpectrogramArea.Height - n * (SpectrogramArea.Height / (SpectrogramFormat.SpectrogramCutFrequency / 1000)) - 5))

                        'Drawing lines
                        Dim y As Single = SpectrogramArea.Height - n * (SpectrogramArea.Height / (SpectrogramFormat.SpectrogramCutFrequency / 1000))
                        g.DrawLine(TickPen, 0, y, 7, y)

                    Next

                    'Drawing unit
                    g.DrawString("(Hz)", New Font("Arial", 7), DiagramValuesBrush, New PointF(0, 0))

                    'Drawing the spectrogram settings on the spectrogram area
                    Dim drawSpetrogramSettings As Boolean = True
                    If drawSpetrogramSettings = True Then

                        g.DrawString("Frequency resolution (FFT size in samples): " & SpectrogramFormat.SpectrogramFftFormat.FftWindowSize.ToString, New Font("Arial", 7), SpectrogramSettingsBrush, New PointF(40, 4))
                        g.DrawString("Filter FFT size (samples): " & SpectrogramFormat.SpectrogramPreFirFilterFftFormat.FftWindowSize.ToString, New Font("Arial", 7), SpectrogramSettingsBrush, New PointF(40, 14))
                        g.DrawString("Filter kernel creation FFT size (samples): " & SpectrogramFormat.SpectrogramPreFilterKernelFftFormat.FftWindowSize.ToString, New Font("Arial", 7), SpectrogramSettingsBrush, New PointF(40, 24))

                    End If

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub
            Private Sub DrawTime(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles TimeArea.Paint

                'Empties the container before it is redrawn
                'timeArea.Invalidate()

                ' Create a local version of the graphics object for the PictureBox.
                Dim g As System.Drawing.Graphics = e.Graphics
                Dim TickPen As New System.Drawing.Pen(System.Drawing.Color.Black, 2)
                Dim DiagramValuesBrush = Brushes.Black

                Select Case TimeUnit
                    Case TimeUnits.samples

                        Dim valuesToWrite As Integer = 10
                        For n = 0 To valuesToWrite - 1

                            'Drawing sample numbers
                            g.DrawString(DisplayStart_Sample + n * (DisplayLength_Samples / valuesToWrite), New Font("Arial", 7), DiagramValuesBrush, New PointF(n * (TimeArea.Width / valuesToWrite), 2))

                            'Drawing lines
                            Dim x As Single = n * (TimeArea.Width / valuesToWrite)
                            g.DrawLine(TickPen, x, 0, x, TimeArea.Height)

                        Next

                        'Drawing unit
                        g.DrawString("(Samples)", New Font("Arial", 7), DiagramValuesBrush, New PointF(TimeArea.Width - 50, 2))

                    Case TimeUnits.seconds

                        Dim valuesToWrite As Integer = 10
                        For n = 0 To valuesToWrite - 1

                            'Drawing sample numbers
                            g.DrawString(Math.Round((DisplayStart_Sample + n * (DisplayLength_Samples / valuesToWrite)) / CurrentSound.WaveFormat.SampleRate, 3), New Font("Arial", 7), DiagramValuesBrush, New PointF(n * (TimeArea.Width / valuesToWrite) + 1, 2))

                            'Drawing lines
                            Dim x As Single = n * (TimeArea.Width / valuesToWrite)
                            g.DrawLine(TickPen, x, 0, x, TimeArea.Height)

                        Next

                        'Drawing unit
                        g.DrawString("(s)", New Font("Arial", 7), DiagramValuesBrush, New PointF(TimeArea.Width - 25, 2))

                    Case Else
                        Throw New NotSupportedException
                End Select


            End Sub
            Private Sub DrawSelection(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles SoundBackgroundArea.Paint

                Try

                    ' Create a local version of the graphics object for the PictureBox.
                    Dim g As System.Drawing.Graphics = e.Graphics

                    ' Create pen.
                    Dim blackPen As New System.Drawing.Pen(System.Drawing.Color.Black, 1)

                    'Drawing segmentation boundary lines and labels
                    If UseItemSegmentation = True Then

                        'Drawing the (visible) surrounding segments
                        If CurrentSegmentationItem IsNot Nothing Then

                            Dim SegmentationSiblings = CurrentSegmentationItem.GetSiblingsExcludingSelf

                            If SegmentationSiblings IsNot Nothing Then

                                For Each SiblingComponent In SegmentationSiblings

                                    Dim startPen As New System.Drawing.Pen(System.Drawing.Color.Gray, 2)
                                    Dim segmentBrush As New SolidBrush(Color.FromArgb(60, Color.Gray))
                                    Dim segmentTextBrush As New SolidBrush(Color.FromArgb(120, Color.Gray))
                                    Dim endPen As New System.Drawing.Pen(System.Drawing.Color.Gray, 2)

                                    Dim SegmentationStartPixel As Single = (SiblingComponent.StartSample - DisplayStart_Sample) / SampleToPixelScale
                                    Dim SegmentationWidthInPixels As Single = SiblingComponent.Length / SampleToPixelScale

                                    If Not (SiblingComponent.StartSample) < 0 Then ' Is used to "hide" the segmentation lines and strings if they are not set. Also means that they cannot be displayed if they are set to 0. 

                                        'Draws the segmentation area
                                        'As a rectangle
                                        'Dim SegmentationLayoutRectangle As New RectangleF(SegmentationStartPixel, SoundBackgroundArea.Top, SegmentationWidthInPixels, SoundBackgroundArea.Height - TimeArea.Height)
                                        'g.FillRectangle(segmentBrush, SegmentationLayoutRectangle)

                                        'As a triangle
                                        Dim SegmentationLayoutTriangle() As PointF = {
                                            New PointF(SegmentationStartPixel, SoundBackgroundArea.Height - TimeArea.Height),
                                            New PointF(SegmentationStartPixel, SoundBackgroundArea.Top),
                                            New PointF(SegmentationStartPixel + SegmentationWidthInPixels, SoundBackgroundArea.Height - TimeArea.Height)}
                                        g.FillPolygon(segmentBrush, SegmentationLayoutTriangle)

                                        'Draws the segmentation end line first (as it may otherwise be overwrite the start line)
                                        g.DrawLine(endPen, SegmentationStartPixel + SegmentationWidthInPixels, SoundBackgroundArea.Top, SegmentationStartPixel + SegmentationWidthInPixels, SoundBackgroundArea.Height)

                                        'Draws the segmentation start line
                                        g.DrawLine(startPen, SegmentationStartPixel, SoundBackgroundArea.Top, SegmentationStartPixel, SoundBackgroundArea.Height)

                                        'Getting an appropriate string to display
                                        Dim SegmentationStartText As String = ""
                                        Select Case SiblingComponent.SmaTag
                                            Case Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE
                                                SegmentationStartText = SiblingComponent.OrthographicForm
                                            Case Sound.SpeechMaterialAnnotation.SmaTags.PHONE
                                                SegmentationStartText = "[ " & SiblingComponent.PhoneticForm & " ]"
                                            Case Else
                                                SegmentationStartText = SiblingComponent.GetStringRepresentation
                                        End Select
                                        If SegmentationStartText = "" Then SegmentationStartText = "Start"

                                        'Putting the string at the top of the background panel
                                        g.DrawString(SegmentationStartText,
                                              New Font("Arial", 20), segmentTextBrush, New PointF(SegmentationStartPixel, 0))

                                    End If

                                Next
                            End If
                        End If

                        'Drawing the current segmentation item
                        If CurrentSegmentationItem IsNot Nothing Then

                            Dim startPen As New System.Drawing.Pen(Color.FromArgb(150, Color.Green), 2)
                            Dim segmentBrush As New SolidBrush(Color.FromArgb(80, Color.LightGreen))
                            Dim redSegmentBrush As New SolidBrush(Color.FromArgb(80, Color.LightCoral))
                            Dim segmentTextBrush As New SolidBrush(Color.FromArgb(200, Color.Green))
                            Dim endPen As New System.Drawing.Pen(System.Drawing.Color.LightCoral, 2)

                            Dim SegmentationStartPixel As Single = (CurrentSegmentationItem.StartSample - DisplayStart_Sample) / SampleToPixelScale
                            Dim SegmentationWidthInPixels As Single = CurrentSegmentationItem.Length / SampleToPixelScale

                            If Not (CurrentSegmentationItem.StartSample) < 0 Then ' Is used to "hide" the segmentation lines and strings if they are not set. Also means that they cannot be displayed if they are set to 0. 

                                'Draws the segmentation area
                                'As a rectangle
                                'Dim SegmentationLayoutRectangle As New RectangleF(SegmentationStartPixel, SoundBackgroundArea.Top, SegmentationWidthInPixels, SoundBackgroundArea.Height - TimeArea.Height)
                                'g.FillRectangle(segmentBrush, SegmentationLayoutRectangle)

                                'As a triangle
                                Dim SegmentationLayoutTriangle() As PointF = {
                                            New PointF(SegmentationStartPixel, SoundBackgroundArea.Height - TimeArea.Height),
                                            New PointF(SegmentationStartPixel, SoundBackgroundArea.Top),
                                            New PointF(SegmentationStartPixel + SegmentationWidthInPixels, SoundBackgroundArea.Height - TimeArea.Height)}
                                If SegmentationWidthInPixels < 0 Then
                                    g.FillPolygon(redSegmentBrush, SegmentationLayoutTriangle)
                                Else
                                    g.FillPolygon(segmentBrush, SegmentationLayoutTriangle)
                                End If

                                'Draws the segmentation end line first (as it may otherwise be overwrite the start line)
                                g.DrawLine(endPen, SegmentationStartPixel + SegmentationWidthInPixels, SoundBackgroundArea.Top, SegmentationStartPixel + SegmentationWidthInPixels, SoundBackgroundArea.Height)

                                'Draws the segmentation start line
                                g.DrawLine(startPen, SegmentationStartPixel, SoundBackgroundArea.Top, SegmentationStartPixel, SoundBackgroundArea.Height)

                                'Getting an appropriate string to display
                                Dim SegmentationStartText As String = ""
                                Select Case CurrentSegmentationItem.SmaTag
                                    Case Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE
                                        SegmentationStartText = CurrentSegmentationItem.OrthographicForm
                                    Case Sound.SpeechMaterialAnnotation.SmaTags.PHONE
                                        SegmentationStartText = "[ " & CurrentSegmentationItem.PhoneticForm & " ]"
                                    Case Else
                                        SegmentationStartText = CurrentSegmentationItem.GetStringRepresentation
                                End Select

                                If SegmentationStartText = "" Then SegmentationStartText = "Start"

                                'Putting the string at the top of the background panel
                                g.DrawString(SegmentationStartText,
                                              New Font("Arial", 20), segmentTextBrush, New PointF(SegmentationStartPixel, 0))

                            End If


                        End If


                    End If


                    'drawing selection (it also draws a selection of length 0 (which is displayed as a single line) with start index 0, if no selection is made)
                    Dim greenBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(128, Drawing.Color.Gray))
                    'Dim myBrush As New System.Drawing.Drawing2D.LinearGradientBrush(selection, Drawing.Color.Red, Drawing.Color.Transparent,
                    'System.Drawing.Drawing2D.LinearGradientMode.Vertical)
                    'Dim aHatchBrush As New System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.SmallGrid,
                    'Drawing.Color.Red, Drawing.Color.Transparent)

                    'Converting selection data pixels
                    If SelectionLength_Sample > 0 Then

                        'UpdateScale()
                        Dim selectedPeriodStartPixel As Single = (SelectionStart_Sample - DisplayStart_Sample) / SampleToPixelScale
                        Dim selectedPeriodLengthPixel As Single = SelectionLength_Sample / SampleToPixelScale

                        'Draws a shaded area
                        g.FillRectangle(greenBrush, selectedPeriodStartPixel, SoundBackgroundArea.Top, selectedPeriodLengthPixel, SoundBackgroundArea.Height)

                        'Draw lines 
                        g.DrawLine(blackPen, selectedPeriodStartPixel, SoundBackgroundArea.Top, selectedPeriodStartPixel, SoundBackgroundArea.Height)
                        g.DrawLine(blackPen, selectedPeriodStartPixel + selectedPeriodLengthPixel, SoundBackgroundArea.Top, selectedPeriodStartPixel + selectedPeriodLengthPixel, SoundBackgroundArea.Height)

                        'Drawing selection times
                        'Start time
                        Select Case TimeUnit
                            Case TimeUnits.samples
                                g.DrawString(SelectionStart_Sample, New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel, 10))
                                g.DrawString(SelectionStart_Sample + SelectionLength_Sample, New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel + selectedPeriodLengthPixel + 1, 15))

                                'Also drawing selection length
                                g.DrawString("(" & SelectionLength_Sample & ")", New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel + selectedPeriodLengthPixel + 1, 30))

                            Case TimeUnits.seconds
                                g.DrawString(Math.Round(SelectionStart_Sample / CurrentSound.WaveFormat.SampleRate, 3), New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel, 10))
                                g.DrawString(Math.Round((SelectionStart_Sample + SelectionLength_Sample) / CurrentSound.WaveFormat.SampleRate, 3), New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel + selectedPeriodLengthPixel + 1, 20))

                                'Also drawing selection length
                                g.DrawString("(" & Math.Round(SelectionLength_Sample / CurrentSound.WaveFormat.SampleRate, 3) & ")", New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel + selectedPeriodLengthPixel + 1, 40))

                            Case Else
                                Throw New NotSupportedException
                        End Select

                    Else
                        'Drawing cursor
                        If SampleToPixelScale > 0 Then
                            Dim selectedPeriodStartPixel As Single = (SelectionStart_Sample - DisplayStart_Sample) / SampleToPixelScale
                            g.DrawLine(blackPen, selectedPeriodStartPixel, SoundBackgroundArea.Top, selectedPeriodStartPixel, SoundBackgroundArea.Height)
                        End If
                    End If

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub

            Private Sub InvalidateGraphics()

                'Invalidates each control of the sound area so that they are redrawn on next draw event
                Dim mySoundDisplayRectangle As New Rectangle(LeftMainContainer.Panel1.Left, LeftMainContainer.Panel1.Top,
                                      LeftMainContainer.Panel1.Width, LeftMainContainer.Panel1.Height)
                LeftMainContainer.Panel1.Invalidate(mySoundDisplayRectangle, True)

            End Sub

            'Taking care of user input
            '   - Things that concern the sound drawing area
            Private Sub Container_MouseDown(sender As System.Object, e As MouseEventArgs) Handles WaveArea.MouseDown, SpectrogramArea.MouseDown

                If e.Button = MouseButtons.Left Then
                    RemoveHandler WaveArea.MouseDown, AddressOf Me.Container_MouseDown
                    If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseDown, AddressOf Me.Container_MouseDown

                    SelectionStart_Pixel = e.X
                    SelectionLength_Pixel = 0

                    InvalidateGraphics()

                    AddHandler WaveArea.MouseMove, AddressOf Me.Container_MouseMove
                    AddHandler WaveArea.MouseUp, AddressOf Me.Container_MouseUp
                    If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseMove, AddressOf Me.Container_MouseMove
                    If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseUp, AddressOf Me.Container_MouseUp

                End If


                If e.Button = MouseButtons.Right Then

                    Dim CastSender As Control = TryCast(sender, Control)
                    If CastSender IsNot Nothing Then
                        'Displaying the context menu
                        SoundDisplayContextMenu.Location = CastSender.PointToScreen(e.Location)  'Cursor.Position
                    Else
                        'Putting the control top left on the screen if for some reason the sender could not be cast to a Control
                        SoundDisplayContextMenu.Location = New Point(0, 0)
                    End If
                    SoundDisplayContextMenu.Show()

                End If

            End Sub

            Private Sub Container_MouseMove(sender As System.Object, e As MouseEventArgs)
                CalculateSelection(e)
                InvalidateGraphics()
            End Sub
            Private Sub Container_MouseLeave() Handles WaveArea.MouseLeave, SpectrogramArea.MouseLeave

                'Activates the mouse down handlers of the wave and spectrogram areas
                AddHandler WaveArea.MouseDown, AddressOf Me.Container_MouseDown
                If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseDown, AddressOf Me.Container_MouseDown
                If UseItemSegmentation = True Then
                    'TODO: Here phoneme setting should be cancelled if started:
                    'switch back to normal event handler
                    'switch back the backcolor of the phoneme lables
                End If

            End Sub
            Private Sub Container_MouseUp(sender As System.Object, e As MouseEventArgs)

                'This sub removes the handlers that create the selection during mousemove, and initializes setting selection end
                'It also enables a new selection by activating the MouseDown event of wave and spectrogram areas
                If e.Button = MouseButtons.Left Then

                    RemoveHandler WaveArea.MouseMove, AddressOf Me.Container_MouseMove
                    RemoveHandler WaveArea.MouseUp, AddressOf Me.Container_MouseUp
                    If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseMove, AddressOf Me.Container_MouseMove
                    If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseUp, AddressOf Me.Container_MouseUp

                    CalculateSelection(e)

                    InvalidateGraphics()

                    AddHandler WaveArea.MouseDown, AddressOf Me.Container_MouseDown
                    If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseDown, AddressOf Me.Container_MouseDown

                End If

            End Sub
            Private Sub CalculateSelection(e As MouseEventArgs)

                'This sub calculates start and end of the selected region and stores it in selectionStartSample and selectionLengthSample
                '(A recalculation of selection start position is needed because a selection can be done in both directions on the screen
                Dim highlightEndPixel As Single = e.X

                'Makes sure that selection does not extend outside the window
                If highlightEndPixel >= SoundBackgroundArea.Width Then highlightEndPixel = SoundBackgroundArea.Width - 1
                If highlightEndPixel < 0 Then highlightEndPixel = 0

                SelectionLength_Pixel = highlightEndPixel - SelectionStart_Pixel

                'Makes sure than selection is in the right direction
                Dim tempSelectedStartPixel As Single
                Dim tempSelectedLengthPixel As Single

                If SelectionLength_Pixel >= 0 Then
                    tempSelectedStartPixel = SelectionStart_Pixel
                    tempSelectedLengthPixel = SelectionLength_Pixel
                Else
                    tempSelectedStartPixel = highlightEndPixel
                    tempSelectedLengthPixel = -SelectionLength_Pixel
                End If

                SelectionStart_Sample = DisplayStart_Sample + tempSelectedStartPixel * SampleToPixelScale
                SelectionLength_Sample = tempSelectedLengthPixel * SampleToPixelScale

            End Sub
            Private Sub WaveScroll(sender As Object, ByVal e As ScrollEventArgs) Handles SoundScrollBar.Scroll
                'This sub initializes a soundScrollBarScroll
                SoundScrollBarScroll(sender, e.NewValue)
            End Sub

            Private RecalculateDisplayAreaDueTosoundScrollBar_Enabled As Boolean = True
            Public Sub SoundScrollBarScroll(sender As Object, ByVal ScrollTo As Integer)

                'This sub performs a scroll of the sound, in rate of update GraphicUpdateFrequency if the sender is the soundScrollBarScroll
                'and instantaneoulsy if the sender is anything else (I.E. the sub may be call externally)
                DisplayStart_Sample = ScrollTo

                If TypeOf sender Is HScrollBar Then
                    Dim myScrollBar As HScrollBar = sender
                    If myScrollBar Is SoundScrollBar Then
                        'The following code limits the rate of recalculation of the display area to GraphicUpdateFrequency times per second
                        'Compares a time rounded to hundreds of a second, to the updatefrequency converted to update time in hundreds of a second
                        If Math.Round(DateTime.Now.Millisecond / 10) Mod ((1 / GraphicUpdateFrequency) * 100) = 0 Then
                            RecalculateDisplayAreaDueTosoundScrollBar_Enabled = True
                        End If
                        'Recalculates the display area only if an update is due
                        If RecalculateDisplayAreaDueTosoundScrollBar_Enabled = True Then
                            RecalculateDisplayAreaDueTosoundScrollBar_Enabled = False
                            UpdateLayout()
                            Exit Sub
                        End If
                    End If
                End If

                'If the sub isn't exited (I.E. the sender is not soundScrollBar, the display area is updated)
                UpdateLayout()

            End Sub
            Private Sub SoundScrollBarScrollEnd() Handles SoundScrollBar.MouseUp

                'This sub makes sure that the sound area is updated when a scroll of the sound is finished (on mouse up)
                '(this is needed because not all scroll events in soundScrollBarScroll triggers a recalculation of the display area
                UpdateLayout()
            End Sub



            Private Sub Container_PositionSegmentationStart(sender As System.Object, e As MouseEventArgs)

                Container_MoveSegmentationStart(sender, e)

                ResetSegmentationIntemPanelControlColors()

                RemoveHandler WaveArea.MouseDown, AddressOf Me.Container_PositionSegmentationStart
                RemoveHandler WaveArea.MouseMove, AddressOf Me.Container_MoveSegmentationStart
                If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseDown, AddressOf Me.Container_PositionSegmentationStart
                If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseMove, AddressOf Me.Container_MoveSegmentationStart

                InvalidateGraphics()

                AddHandler WaveArea.MouseDown, AddressOf Me.Container_MouseDown
                If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseDown, AddressOf Me.Container_MouseDown

                'Aligning segmentations
                If CurrentSegmentationItem IsNot Nothing And CurrentSound IsNot Nothing Then
                    CurrentSegmentationItem.AlignSegmentationStartsAcrossLevels(CurrentSound.WaveData.SampleData(CurrentChannel).Length)
                End If

                'Setting all dependent segmentations to SegmentationCompleted = False
                Dim DependentSegmentations = CurrentSegmentationItem.GetDependentSegmentationsStarts
                For Each DependentSegmentation In DependentSegmentations
                    DependentSegmentation.SegmentationCompleted = False
                Next

                'Updates the controls in the side segmentation panel, to ensure they show the correct validated status
                UpdateSideSegmentationPanelControls()

            End Sub

            Private Sub Container_PositionSegmentationEnd(sender As System.Object, e As MouseEventArgs)

                Container_MoveSegmentationEnd(sender, e)

                ResetSegmentationIntemPanelControlColors()

                RemoveHandler WaveArea.MouseDown, AddressOf Me.Container_PositionSegmentationEnd
                RemoveHandler WaveArea.MouseMove, AddressOf Me.Container_MoveSegmentationEnd
                If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseDown, AddressOf Me.Container_PositionSegmentationEnd
                If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseMove, AddressOf Me.Container_MoveSegmentationEnd

                InvalidateGraphics()

                AddHandler WaveArea.MouseDown, AddressOf Me.Container_MouseDown
                If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseDown, AddressOf Me.Container_MouseDown

                'Aligning segmentations
                If CurrentSegmentationItem IsNot Nothing Then
                    CurrentSegmentationItem.AlignSegmentationEndsAcrossLevels()
                End If

                'Setting all dependent segmentations to SegmentationCompleted = False
                Dim DependentSegmentations = CurrentSegmentationItem.GetDependentSegmentationsEnds
                For Each DependentSegmentation In DependentSegmentations
                    DependentSegmentation.SegmentationCompleted = False
                Next

                'Updates the controls in the side segmentation panel, to ensure they show the correct validated status
                UpdateSideSegmentationPanelControls()

            End Sub

            Private Sub Container_MoveSegmentationStart(sender As System.Object, e As MouseEventArgs)

                'This sub sets the start position of the selected sentence, word or phone.

                If CurrentSegmentationItem IsNot Nothing Then

                    'Invadates the segmentation, if graphically modified
                    CurrentSegmentationItem.SegmentationCompleted = False

                    If SetSegmentationToZeroCrossings Then
                        Dim StartSample As Integer = DisplayStart_Sample + e.X * SampleToPixelScale
                        StartSample = DSP.GetZeroCrossingSample(CurrentSound, 1, StartSample, DSP.MeasurementsExt.SearchDirections.Closest)
                        CurrentSegmentationItem.MoveStart(StartSample, CurrentSound.WaveData.SampleData(CurrentChannel).Length)
                    Else
                        CurrentSegmentationItem.MoveStart(DisplayStart_Sample + e.X * SampleToPixelScale, CurrentSound.WaveData.SampleData(CurrentChannel).Length)
                    End If

                End If

                InvalidateGraphics()

            End Sub

            Private Sub Container_MoveSegmentationEnd(sender As System.Object, e As MouseEventArgs)

                'This sub sets the end position of the selected sentence, word or phone.
                If CurrentSegmentationItem IsNot Nothing Then

                    'Invadates the segmentation, if graphically modified
                    CurrentSegmentationItem.SegmentationCompleted = False

                    If SetSegmentationToZeroCrossings Then
                        Dim EndSample As Integer = DisplayStart_Sample + e.X * SampleToPixelScale
                        EndSample = DSP.GetZeroCrossingSample(CurrentSound, 1, EndSample, DSP.MeasurementsExt.SearchDirections.Closest)
                        CurrentSegmentationItem.Length = Math.Max(0, EndSample - CurrentSegmentationItem.StartSample)
                    Else
                        CurrentSegmentationItem.Length = Math.Max(0, (DisplayStart_Sample + e.X * SampleToPixelScale) - CurrentSegmentationItem.StartSample)
                    End If

                End If

                InvalidateGraphics()

            End Sub



            Private Sub ResetSegmentationIntemPanelControlColors()

                'Resets the colors of the buttons
                For Each item As Control In SegmentationItemsPanel.Controls
                    If item.Text = "Set start" Then
                        item.BackColor = Color.FromArgb(50, Color.LightGreen)
                    ElseIf item.Text = "Set end" Then
                        item.BackColor = Color.FromArgb(50, Color.LightCoral)
                    Else
                        item.BackColor = Color.LightGray
                    End If
                Next
            End Sub

            '   - Other user input
            Private Sub SetSpectrogramLightFactor(sender As Object, ByVal e As ScrollEventArgs) Handles SpectrogramLightFactorScrollBar.Scroll

                'This sub calculates a new ligth factor of the spectrogram, by reading the new value from spectrogramLightFactorScroll
                SpectrogramLightFactor = (2 ^ ((SpectrogramLightFactorScrollBar.Maximum - e.NewValue - 9) / 10)) - 1 'Its wierd why the scroll won't go to maximum value, it only goes to 91 of 100

            End Sub
            Private Sub UpdateSpectrogramLight() Handles SpectrogramLightFactorScrollBar.MouseCaptureChanged 'I'd be better if this was MouseUp, but for some reason it is not working, why?

                'This sub executes recalculation of the spectrogram data after a change of the ligth factor of the spectrogram
                ReCalculateSpectrogramData()
                InvalidateGraphics() 'TODO: Acctually only the spectrogram area would need to be invalidated!

            End Sub
            Private Sub ContextMenuItem_Click(sender As Object, ByVal e As ToolStripItemClickedEventArgs) Handles SoundDisplayContextMenu.ItemClicked
                'This sub handles the click on the menu item and directs the code to the appropriate modification/action
                'After doing so the display area is updated once (actually not all actions (like play) need an update of the display area, this could be fixed for performance reasons)

                SoundDisplayContextMenu.Hide()
                'upDatePanel1DisplayData()
                InvalidateGraphics()

                Select Case e.ClickedItem.Name
                    Case "Play"
                        Play()
                    Case "PlayAll"
                        PlayAll()
                    Case "StopSound"
                        StopSound()
                    Case "ZoomOut"
                        ZoomOut()
                    Case "ZoomIn"
                        ZoomIn()
                    Case "ZoomToSelection"
                        ZoomToSelection()
                    Case "ZoomFull"
                        ZoomFull()
                    Case "SmoothFadeIn"
                        FadeIn(DSP.FadeSlopeType.Smooth)
                    Case "SmoothFadeOut"
                        FadeOut(DSP.FadeSlopeType.Smooth)
                    Case "LinearFadeIn"
                        FadeIn(DSP.FadeSlopeType.Linear)
                    Case "LinearFadeOut"
                        FadeOut(DSP.FadeSlopeType.Linear)
                    Case "SilenceSelection"
                        SilenceSelection(False)
                    Case "SilenceSelectionZeroCross"
                        SilenceSelection(True)
                    Case "Copy"
                        GraphicCopy()
                    Case "Cut"
                        GraphicCut()
                    Case "Paste"
                        GraphicPaste()
                    Case "Delete"
                        GraphicDelete()
                    Case "Crop"
                        GraphicCrop()
                    Case "UndoAll"
                        GraphicUndoAll()
                End Select

            End Sub


            Private Sub ReSizeTimer_Tick() Handles Me.SizeChanged, Me.SplitterMoved

                Try

                    'Since the SplitterDistance of myLayoutContainer is changed when myLayoutContainer is resized, it is corrected here
                    If UseItemSegmentation = True Then
                        LeftMainContainer.SplitterDistance = LeftMainContainer.Height - SegmentationItemsPanel.Height
                    Else
                        LeftMainContainer.SplitterDistance = LeftMainContainer.Height
                    End If

                    UpdateLayout()

                Catch ex As Exception
                    'Do nothing
                End Try

            End Sub

            '   - variuos button clicks

            Private Sub GotoNextItem()
                GotoNextItem(False, False)
            End Sub

            Private Sub GotoPreviuosItem()
                GotoNextItem(True, False)
            End Sub

            Private Sub GotoNextUnvalidatedItem()
                GotoNextItem(False, True)
            End Sub

            Private Sub GotoPreviousUnvalidatedItem()
                GotoNextItem(True, True)
            End Sub


            Private Sub GotoNextItem(ByVal Backwards As Boolean, ByVal SkipValidated As Boolean)

                If CurrentSegmentationItem IsNot Nothing Then

                    Dim TemporaryComponentCollection As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                    If SkipValidated = False Then
                        For Each component In AllSegmentationComponents
                            'Adds all components
                            TemporaryComponentCollection.Add(component)
                        Next
                    Else
                        For Each component In AllSegmentationComponents
                            'Adds only unvalidated components
                            If component.SegmentationCompleted = False Then TemporaryComponentCollection.Add(component)
                        Next

                        'Exits if all componants are validated
                        If TemporaryComponentCollection.Count = 0 Then
                            MsgBox("All items are validated!")
                            Exit Sub
                        End If
                    End If

                    Dim CurrentSegmentationItemIndex As Integer = -1
                    For i = 0 To TemporaryComponentCollection.Count - 1
                        If TemporaryComponentCollection(i) Is CurrentSegmentationItem Then
                            CurrentSegmentationItemIndex = i
                            Exit For
                        End If
                    Next

                    If CurrentSegmentationItemIndex = -1 Then
                        MsgBox("Could not find the indicated item.")
                        Exit Sub
                    End If

                    If Backwards = False Then
                        'Check to see if more items exist
                        If CurrentSegmentationItemIndex + 1 > TemporaryComponentCollection.Count - 1 Then
                            MsgBox("You're already displaying the last segmentation item.")
                            Exit Sub
                        End If
                        CurrentSegmentationItemIndex += 1

                    Else
                        'Check to see if more items exist
                        If CurrentSegmentationItemIndex - 1 < 0 Then
                            MsgBox("You're already displaying the first segmentation item.")
                            Exit Sub
                        End If
                        CurrentSegmentationItemIndex -= 1
                    End If

                    ViewSegmentationItem(TemporaryComponentCollection(CurrentSegmentationItemIndex))

                End If

            End Sub

            Private Sub UpdateSideSegmentationPanelControls()

                'Updates the layout of the controls in SentenceSelectorPanel, in order to ensure they show correct Validated state
                For Each control As Control In SentenceSelectorPanel.Controls
                    control.Invalidate()
                Next

                For Each control As Control In WordSelectorPanel.Controls
                    control.Invalidate()
                Next

                For Each control As Control In PhonemeSelectorPanel.Controls
                    control.Invalidate()
                Next

            End Sub



            ''' <summary>
            ''' Sets the indicated new SegmentationItem into view.
            ''' </summary>
            ''' <param name="NewSegmentationItem"></param>
            Private Sub ViewSegmentationItem(ByRef NewSegmentationItem As Sound.SpeechMaterialAnnotation.SmaComponent)

                If NewSegmentationItem.SmaTag = Sound.SpeechMaterialAnnotation.SmaTags.CHANNEL Then
                    MsgBox("Cannot select next audio channel! Only single channel segmentation is supported!")
                    Exit Sub
                End If

                'Updates the layout of the controls in SentenceSelectorPanel, in order to ensure they show correct Validated state
                For Each control As Control In SentenceSelectorPanel.Controls
                    control.Invalidate()
                Next

                'If the NewSegmentationItem is a word or a phone, the sentence components is retreived by the GetAncestorComponent function of the NewSegmentationItem
                If NewSegmentationItem.SmaTag = Sound.SpeechMaterialAnnotation.SmaTags.PHONE Or NewSegmentationItem.SmaTag = Sound.SpeechMaterialAnnotation.SmaTags.WORD Then
                    'Selecting the sentence
                    Dim Sentence = NewSegmentationItem.GetClosestAncestorComponent(Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE)
                    If Sentence IsNot Nothing Then
                        For Each control As SegmentationItemLabel In SentenceSelectorPanel.Controls
                            If control.SegmentationItem Is Sentence Then
                                'Emulates a click on the appropriate button
                                SentenceLabelButtonClick(control, Nothing)
                            End If
                        Next
                    End If
                End If

                'If the NewSegmentationItem is a phone, the word components is retreived by the GetAncestorComponent function of the NewSegmentationItem
                If NewSegmentationItem.SmaTag = Sound.SpeechMaterialAnnotation.SmaTags.PHONE Then
                    'Selecting the word
                    Dim Word = NewSegmentationItem.GetClosestAncestorComponent(Sound.SpeechMaterialAnnotation.SmaTags.WORD)
                    If Word IsNot Nothing Then
                        For Each control As SegmentationItemLabel In WordSelectorPanel.Controls
                            If control.SegmentationItem Is Word Then
                                'Emulates a click on the appropriate button
                                WordLabelButtonClick(control, Nothing)
                            End If
                        Next
                    End If
                End If

                ' Finally setting the segmentation item based directly on the NewSegmentationItem
                Select Case NewSegmentationItem.SmaTag
                    Case Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE

                        'We only select the sentence
                        For Each control As SegmentationItemLabel In SentenceSelectorPanel.Controls
                            If control.SegmentationItem Is NewSegmentationItem Then
                                'Emulates a click on the appropriate button
                                SentenceLabelButtonClick(control, Nothing)
                            End If
                        Next

                    Case Sound.SpeechMaterialAnnotation.SmaTags.WORD

                        'Selects the word
                        For Each control As SegmentationItemLabel In WordSelectorPanel.Controls
                            If control.SegmentationItem Is NewSegmentationItem Then
                                'Emulates a click on the appropriate button
                                WordLabelButtonClick(control, Nothing)
                            End If
                        Next

                    Case Sound.SpeechMaterialAnnotation.SmaTags.PHONE

                        'Selects the phoneme
                        For Each control As SegmentationItemLabel In PhonemeSelectorPanel.Controls
                            If control.SegmentationItem Is NewSegmentationItem Then
                                'Emulates a click on the appropriate button
                                PhoneLabelButtonClick(control, Nothing)
                            End If
                        Next

                End Select

            End Sub

            Private Sub SegmentationItemPlay()

                'Playing the segmentation
                If CurrentSegmentationItem IsNot Nothing Then

                    Dim startSample As Integer = CurrentSegmentationItem.StartSample
                    Dim lengthToPlay As Integer = CurrentSegmentationItem.Length
                    If lengthToPlay < 0 Then lengthToPlay = 0

                    'If SoundPlayerIsInitialized() = False Then SetupSoundPlayer()
                    Dim PlaySection = Audio.DSP.CopySection(CurrentSound, startSample, lengthToPlay)
                    SoundPlayer.SwapOutputSounds(PlaySection)

                End If

            End Sub

            Private Sub SegmentationItemStartButton_Click(sender As Object, ByVal e As MouseEventArgs)

                'This sub handles clicking on the segmentation item start button
                'Starts the event handlers that is used to position the start of the segmentation on the sound display

                sender.backcolor = Color.LightGreen

                'Turn of other eventhandlers
                RemoveHandler WaveArea.MouseDown, AddressOf Me.Container_MouseDown
                If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseDown, AddressOf Me.Container_MouseDown

                'Turn on mouse move eventhandler
                AddHandler WaveArea.MouseDown, AddressOf Me.Container_PositionSegmentationStart
                AddHandler WaveArea.MouseMove, AddressOf Me.Container_MoveSegmentationStart

                If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseDown, AddressOf Me.Container_PositionSegmentationStart
                If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseMove, AddressOf Me.Container_MoveSegmentationStart

            End Sub


            Private Sub SegmentationItemEndButton_Click(sender As Object, ByVal e As MouseEventArgs)

                'This sub handles clicking on the segmentation item length button
                'Starts the event handlers that is used to position the length of the segmentation on the sound display

                sender.backcolor = Color.LightCoral

                'Turn of other eventhandlers
                RemoveHandler WaveArea.MouseDown, AddressOf Me.Container_MouseDown
                If ShowSpectrogram = True Then RemoveHandler SpectrogramArea.MouseDown, AddressOf Me.Container_MouseDown

                'Turn on mouse move eventhandler
                AddHandler WaveArea.MouseDown, AddressOf Me.Container_PositionSegmentationEnd
                AddHandler WaveArea.MouseMove, AddressOf Me.Container_MoveSegmentationEnd

                If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseDown, AddressOf Me.Container_PositionSegmentationEnd
                If ShowSpectrogram = True Then AddHandler SpectrogramArea.MouseMove, AddressOf Me.Container_MoveSegmentationEnd

            End Sub


            Public Sub Play()

                'If SoundPlayerIsInitialized() = False Then SetupSoundPlayer()

                If SelectionLength_Sample = 0 Then

                    PlayAll()

                Else
                    UpdateSampleTimeScale()
                    Dim PlaySection = Audio.DSP.CopySection(CurrentSound, SelectionStart_Sample, SelectionLength_Sample)
                    SoundPlayer.SwapOutputSounds(PlaySection)

                End If

            End Sub



            'Responses to menu functions (these can also be called from external code)
            Public Sub PlayAll()

                UpdateSampleTimeScale()
                'If SoundPlayerIsInitialized() = False Then SoundPlayer.SetupSoundPlayer()

                SoundPlayer.SwapOutputSounds(CurrentSound)

            End Sub
            Public Sub StopSound()

                SoundPlayer.FadeOutPlayback()

            End Sub
            Public Sub ZoomOut()
                ' making the selection twice the size
                UpdateSampleTimeScale()

                DisplayLength_Samples = DisplayLength_Samples * 2
                DisplayStart_Sample = DisplayStart_Sample - Int(DisplayLength_Samples / 4)

                'Making sure it doesn't zoom out to much
                If DisplayStart_Sample < 0 Then DisplayStart_Sample = 0
                If DisplayStart_Sample > CurrentSound.WaveData.SampleData(CurrentChannel).Length - 1 Then DisplayStart_Sample = CurrentSound.WaveData.SampleData(CurrentChannel).Length - 1
                If DisplayStart_Sample + DisplayLength_Samples > CurrentSound.WaveData.SampleData(CurrentChannel).Length Then DisplayLength_Samples = CurrentSound.WaveData.SampleData(CurrentChannel).Length - DisplayStart_Sample

                UpdateLayout()

            End Sub
            Public Sub ZoomIn()
                ' making the selection half the size
                UpdateSampleTimeScale()

                'Changing range
                DisplayLength_Samples = Int(DisplayLength_Samples / 2)
                DisplayStart_Sample = DisplayStart_Sample + Int(DisplayLength_Samples / 2)

                'Making sure length is not shorter that 2 samples
                If DisplayLength_Samples < 2 Then DisplayLength_Samples = 2

                UpdateLayout()

            End Sub
            Public Sub ZoomToSelection()
                UpdateSampleTimeScale()
                DisplayStart_Sample = SelectionStart_Sample
                DisplayLength_Samples = SelectionLength_Sample

                'Making sure length is not shorter that 1 sample
                If DisplayLength_Samples < 2 Then DisplayLength_Samples = 2

                UpdateLayout()

            End Sub
            Public Sub ZoomFull()
                UpdateSampleTimeScale()
                DisplayStart_Sample = 0
                DisplayLength_Samples = CurrentSound.WaveData.SampleData(CurrentChannel).Length

                UpdateLayout()

            End Sub

            ''' <summary>
            ''' Fades in the selected sound using the indicates fade slope type.
            ''' </summary>
            ''' <param name="fadeSlopeType"></param>
            Private Sub FadeIn(ByVal fadeSlopeType As DSP.FadeSlopeType)

                If Not SelectionLength_Sample < 1 Then
                    DSP.Fade(CurrentSound,, 0, CurrentChannel, SelectionStart_Sample, SelectionLength_Sample, fadeSlopeType)

                    'Recalculates spectrogram data, since the waveform have been changed
                    If ShowSpectrogram = True Then UpdateSpectrogramData()

                    UpdateLayout()

                End If

            End Sub

            ''' <summary>
            ''' Fades out the selected sound using the indicates fade slope type.
            ''' </summary>
            ''' <param name="fadeSlopeType"></param>
            Private Sub FadeOut(ByVal fadeSlopeType As DSP.FadeSlopeType)

                If Not SelectionLength_Sample < 1 Then
                    DSP.Fade(CurrentSound, 0, , CurrentChannel, SelectionStart_Sample, SelectionLength_Sample, fadeSlopeType)

                    'Recalculates spectrogram data, since the waveform have been changed
                    If ShowSpectrogram = True Then UpdateSpectrogramData()

                    UpdateLayout()

                End If

            End Sub

            ''' <summary>
            ''' Silences the selected section of the sound
            ''' </summary>
            ''' <param name="AdjustToZeroCrossings">If set to true, the silent section starts and end at the closest zero crossings withing the selected section of the wave form.</param>
            Private Sub SilenceSelection(ByVal AdjustToZeroCrossings As Boolean)

                Dim SilenceStartSample As Integer = SelectionStart_Sample
                Dim SilenceLength As Integer = SelectionLength_Sample

                If AdjustToZeroCrossings = True Then

                    Dim SelectionEndSample As Integer = SilenceStartSample + SilenceLength

                    SilenceStartSample = DSP.GetZeroCrossingSample(CurrentSound, CurrentChannel, SilenceStartSample, DSP.MeasurementsExt.SearchDirections.Later)
                    SilenceLength = DSP.GetZeroCrossingSample(CurrentSound, CurrentChannel, SilenceLength, DSP.MeasurementsExt.SearchDirections.Earlier)

                    'Checking that the zero crossing search hasn't caused SilenceStartSample to be equal or higher that SelectionEndSample. If so SilenceLength is set to 0 to stop silencing.
                    If SilenceStartSample >= SelectionEndSample Then SilenceLength = 0

                End If

                'Silencing the section using the fade function, with bith silent start and end
                If Not SelectionLength_Sample < 1 Then
                    DSP.Fade(CurrentSound, , , CurrentChannel, SilenceStartSample, SilenceLength, DSP.FadeSlopeType.Linear)

                    'Recalculates spectrogram data, since the waveform have been changed
                    If ShowSpectrogram = True Then UpdateSpectrogramData()

                End If

                UpdateLayout()

            End Sub


            Private Sub GraphicCopy()

                If Not SelectionLength_Sample < 1 Then

                    'Copies the selected sound to a new array
                    ReDim SelectionCopy(SelectionLength_Sample - 1)
                    For sample = 0 To SelectionCopy.Length - 1
                        SelectionCopy(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample + SelectionStart_Sample)
                    Next

                End If

            End Sub
            Private Sub GraphicCut()

                If Not SelectionLength_Sample < 1 Then

                    'Copies the selected sound to a new array
                    ReDim SelectionCopy(SelectionLength_Sample - 1)
                    For sample = 0 To SelectionCopy.Length - 1
                        SelectionCopy(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample + SelectionStart_Sample)
                    Next

                    'Cutting the selected sound from sound
                    'Getting a copy of sound without the selected samples
                    Dim newArray(CurrentSound.WaveData.SampleData(CurrentChannel).Length - SelectionLength_Sample - 1) As Single
                    For sample = 0 To SelectionStart_Sample - 1
                        newArray(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample)
                    Next
                    For sample = SelectionStart_Sample To newArray.Length - 1
                        newArray(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample + SelectionLength_Sample)
                    Next

                    CurrentSound.WaveData.SampleData(CurrentChannel) = newArray

                    'Recalculates spectrogram data, since the waveform have been changed
                    If ShowSpectrogram = True Then UpdateSpectrogramData()

                    UpdateLayout()
                    SelectionLength_Sample = 0
                    If UseItemSegmentation = True Then ResetCurrentWordLevelSegmentationData()

                End If

                UpdateLayout()

            End Sub
            Private Sub GraphicPaste()

                If SelectionCopy.Length > 0 Then

                    'Copies the selected sound to a new array

                    'Pasting the data in slectionCopy starting at selectionStartSample 
                    'Copying the data prior to selectionStartSample 
                    Dim newArray(CurrentSound.WaveData.SampleData(CurrentChannel).Length + SelectionCopy.Length - 1) As Single
                    For sample = 0 To SelectionStart_Sample - 1
                        newArray(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample)
                    Next

                    'Pasting the data from selectionCopy
                    For sample = SelectionStart_Sample To SelectionStart_Sample + SelectionCopy.Length - 1
                        newArray(sample) = SelectionCopy(sample - SelectionStart_Sample)
                    Next

                    'Copying the data after selectionStartSample 
                    For sample = SelectionStart_Sample + SelectionCopy.Length To newArray.Length - 1
                        newArray(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample - SelectionCopy.Length)
                    Next

                    CurrentSound.WaveData.SampleData(CurrentChannel) = newArray
                    'sound = newArray

                    'Recalculates spectrogram data, since the waveform have been changed
                    If ShowSpectrogram = True Then UpdateSpectrogramData()

                    SelectionLength_Sample = 0
                    If UseItemSegmentation = True Then ResetCurrentWordLevelSegmentationData()

                End If

                UpdateLayout()

            End Sub
            Private Sub GraphicDelete()

                If Not SelectionLength_Sample < 1 Then

                    'Deleting the selected sound from sound
                    'Getting a copy of sound without the selected samples
                    Dim newArray(CurrentSound.WaveData.SampleData(CurrentChannel).Length - SelectionLength_Sample - 1) As Single
                    For sample = 0 To SelectionStart_Sample - 1
                        newArray(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample)
                    Next
                    For sample = SelectionStart_Sample To newArray.Length - 1
                        newArray(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample + SelectionLength_Sample)
                    Next

                    CurrentSound.WaveData.SampleData(CurrentChannel) = newArray
                    'sound = newArray

                    'Recalculates spectrogram data, since the waveform have been changed
                    If ShowSpectrogram = True Then UpdateSpectrogramData()

                    SelectionLength_Sample = 0
                    If UseItemSegmentation = True Then
                        ResetCurrentWordLevelSegmentationData()
                    End If

                End If

                UpdateLayout()

            End Sub
            Private Sub GraphicCrop()

                If Not SelectionLength_Sample < 1 Then

                    'Copies the selected sound to a new array
                    ReDim SelectionCopy(SelectionLength_Sample - 1)
                    For sample = 0 To SelectionCopy.Length - 1
                        SelectionCopy(sample) = CurrentSound.WaveData.SampleData(CurrentChannel)(sample + SelectionStart_Sample)
                    Next

                    'Replacing the old sound with selectionCopy

                    CurrentSound.WaveData.SampleData(CurrentChannel) = SelectionCopy
                    'sound = selectionCopy

                    'Recalculates spectrogram data, since the waveform have been changed
                    If ShowSpectrogram = True Then UpdateSpectrogramData()

                    SelectionLength_Sample = 0
                    If UseItemSegmentation = True Then ResetCurrentWordLevelSegmentationData()

                End If

                UpdateLayout()

            End Sub
            Private Sub GraphicUndoAll()
                RetriveSoundBackUp()
                'If UseItemSegmentation = True Then resetCurrentWordLevelSegmentationData() ' I don't think this is needed here. Since the original word is retrieved, it's probably ok to also keep the SMA data in it as it is.
                SelectionLength_Sample = 0
                ZoomFull()
            End Sub

            'Other available public functions, not primarily for internal use
            Public Sub ZoomTo(ByVal startSample As Integer, ByVal length As Integer)

                'displaying the waveform for the specified sample interval
                UpdateSampleTimeScale()

                'Making sure length is not shorter that 2 samples
                If length < 2 Then length = 2

                'Making sure it doesn't zoom out to much
                If startSample < 0 Then startSample = 0
                If startSample > CurrentSound.WaveData.SampleData(CurrentChannel).Length - 1 Then startSample = CurrentSound.WaveData.SampleData(CurrentChannel).Length - 1
                If startSample + length > CurrentSound.WaveData.SampleData(CurrentChannel).Length Then length = CurrentSound.WaveData.SampleData(CurrentChannel).Length - startSample

                DisplayStart_Sample = startSample
                DisplayLength_Samples = length

                'Updating the value of the sound scroll bar
                'SoundScrollBar.Value = DisplayStart_Sample

                UpdateLayout()

            End Sub


            Private Sub InferSiblingLengths()

                If CurrentSegmentationItem IsNot Nothing Then
                    CurrentSegmentationItem.InferSiblingLengths()
                End If

                UpdateLayout()

            End Sub

            Private Sub AutoSetSentenceSegmentation()

                If CurrentSound IsNot Nothing Then
                    If CurrentSound.SMA IsNot Nothing Then

                        'Moves the sentence start and end positions based on the audio, but only if the segmentations of all sentence level components are validated
                        If CurrentSound.SMA.ChannelData(CurrentChannel).AllChildSegmentationsCompleted = True Then

                            Dim InitialPadding As Double = 0
                            Dim FinalPadding As Double = 0
                            Dim SilenceDefinition As Double = 25
                            Dim SetToZeroCrossings As Boolean = True

                            CurrentSound.SMA.DetectSpeechBoundaries(CurrentChannel, InitialPadding, FinalPadding, SilenceDefinition, SetToZeroCrossings)

                        Else
                            MsgBox("Unable to fix the sentences level auto segmentation due to incomplete initial boundary segmentation. (You need to first set approximate sentence segmentation).")
                            Exit Sub
                        End If
                    End If
                End If

                'Recalculates spectrogram data, since the waveform have been changed
                If ShowSpectrogram = True Then UpdateSpectrogramData()

                ZoomFull()


            End Sub



            Private Sub SetInitialPeak()

                If CurrentSound IsNot Nothing Then
                    If CurrentSound.SMA IsNot Nothing Then

                        'Moves the sentence start and end positions based on the audio, but only if the segmentations of all sentence level components are validated
                        If CurrentSound.SMA.ChannelData(CurrentChannel).AllChildSegmentationsCompleted = True Then

                            CurrentSound.SMA.SetInitialPeakAmplitudes()

                        Else
                            MsgBox("Unable to fix the sentences level auto segmentation due to incomplete initial boundary segmentation. (You need to first set approximate sentence segmentation).")
                            Exit Sub
                        End If
                    End If
                End If

                'Recalculates spectrogram data, since the waveform have been changed
                If ShowSpectrogram = True Then UpdateSpectrogramData()

                ZoomFull()


            End Sub

            Private Sub FixIntervals()

                If CurrentSound IsNot Nothing Then
                    If CurrentSound.SMA IsNot Nothing Then
                        'Checks if the segmentations of all sentence level components are validated
                        If CurrentSound.SMA.ChannelData(CurrentChannel).AllChildSegmentationsCompleted = True Then
                            CurrentSound.SMA.ApplyInterSentenceInterval(InterSentenceTime, True, CurrentChannel)
                        Else
                            MsgBox("Unable to fix the inter-sentence intervals due to incomplete boundary segmentation.")
                            Exit Sub
                        End If
                    End If
                End If

                'Recalculates spectrogram data, since the waveform have been changed
                If ShowSpectrogram = True Then UpdateSpectrogramData()

                ZoomFull()

            End Sub

            Private Sub FixPadding()
                If ApplyPadding(True) = False Then Exit Sub
                FadePadding(False)
                ZoomFull()

            End Sub

            ''' <summary>
            ''' Sets the length of the padding sections (before the first sentence and after the last sentence)
            ''' </summary>
            Private Function ApplyPadding(Optional ByVal SkipRecalculationOfSpectrogramData As Boolean = False) As Boolean

                If CurrentSound IsNot Nothing Then
                    If CurrentSound.SMA IsNot Nothing Then
                        If CurrentSound.SMA.ChannelData(CurrentChannel).AllChildSegmentationsCompleted = True Then
                            CurrentSound.SMA.ApplyPaddingSection(CurrentSound, CurrentChannel, PaddingTime)
                        Else
                            MsgBox("Unable to fade padding section due to incomplete boundary segmentation.")
                            Return False
                        End If
                    End If
                End If

                If SkipRecalculationOfSpectrogramData = False Then
                    'Recalculates spectrogram data, since the waveform have been changed
                    If ShowSpectrogram = True Then UpdateSpectrogramData()
                End If

                UpdateLayout()
                Return True

            End Function

            ''' <summary>
            ''' Fades the padded sections
            ''' </summary>
            Private Function FadePadding(Optional ByVal SkipRecalculationOfSpectrogramData As Boolean = False) As Boolean

                If CurrentSound IsNot Nothing Then
                    If CurrentSound.SMA IsNot Nothing Then
                        If CurrentSound.SMA.ChannelData(CurrentChannel).AllChildSegmentationsCompleted = True Then
                            CurrentSound.SMA.FadePaddingSection(CurrentSound, CurrentChannel)
                        Else
                            MsgBox("Unable to fade padding section due to incomplete boundary segmentation.")
                            Return False
                        End If
                    End If
                End If

                If SkipRecalculationOfSpectrogramData = False Then
                    'Recalculates spectrogram data, since the waveform have been changed
                    If ShowSpectrogram = True Then UpdateSpectrogramData()
                End If

                UpdateLayout()
                Return True

            End Function


            Private Sub ValidateSegmentation()

                If CurrentSegmentationItem IsNot Nothing Then

                    Dim Siblings = CurrentSegmentationItem.GetSiblings
                    If Siblings IsNot Nothing Then

                        If CheckItemStartOrders(Siblings) = False Then Exit Sub

                        If CheckItemLengths(Siblings) = False Then Exit Sub

                        If CheckItemEndOrders(Siblings) = False Then Exit Sub

                        'Validates the segmentation of the siblings and all members of an UnbrokenLineOfAncestorsWithoutSiblings
                        For Each item In Siblings
                            item.SegmentationCompleted = True
                        Next

                        Dim UnbrokenLineOfAncestorsWithoutSiblings = CurrentSegmentationItem.GetUnbrokenLineOfAncestorsWithoutSiblings()
                        For Each Item In UnbrokenLineOfAncestorsWithoutSiblings
                            Item.SegmentationCompleted = True
                        Next

                        'Updates the controls in the side segmentation panel
                        UpdateSideSegmentationPanelControls()

                    End If
                End If

            End Sub

            ''' <summary>
            ''' Ensures that the order of StartSamples of items are correct
            ''' </summary>
            ''' <param name="Items"></param>
            ''' <returns></returns>
            Private Function CheckItemStartOrders(ByRef Items As List(Of Sound.SpeechMaterialAnnotation.SmaComponent))

                For i = 0 To Items.Count - 2
                    If Items(i).StartSample >= Items(i + 1).StartSample Then
                        MsgBox("The start position of item " & Items(i + 1).GetStringRepresentation & " must be later than the start postion of " & Items(i + 1).GetStringRepresentation)
                        Return False
                    End If
                Next

                'All is ok, returns True
                Return True

            End Function

            ''' <summary>
            ''' Ensures that the Length items exceeds 0 
            ''' </summary>
            ''' <param name="Items"></param>
            ''' <returns></returns>
            Private Function CheckItemLengths(ByRef Items As List(Of Sound.SpeechMaterialAnnotation.SmaComponent))

                For i = 0 To Items.Count - 1
                    If Items(i).Length <= 0 Then
                        MsgBox("The length of item " & Items(i).GetStringRepresentation & " cannot be " & Items(i).Length)
                        Return False
                    End If
                Next

                'All is ok, returns True
                Return True

            End Function

            ''' <summary>
            ''' Ensures that the order of item ends are correct
            ''' </summary>
            ''' <param name="Items"></param>
            ''' <returns></returns>
            Private Function CheckItemEndOrders(ByRef Items As List(Of Sound.SpeechMaterialAnnotation.SmaComponent))

                For i = 0 To Items.Count - 2
                    If Items(i).StartSample + Items(i).Length >= Items(i + 1).StartSample + Items(i + 1).Length Then
                        MsgBox("The end position of item " & Items(i + 1).GetStringRepresentation & " must be later than the end postion of " & Items(i + 1).GetStringRepresentation)
                        Return False
                    End If
                Next

                'All is ok, returns True
                Return True

            End Function

            Private Class SpectrogramDisplayData

                Public Property Area As RectangleF
                Public Property BrushColor As Brush

                Public Sub New(setBrushColor As Brush, X As Single, Y As Single, width As Single, height As Single)

                    If X = Nothing Then X = 0
                    If Y = Nothing Then Y = 0
                    If width = Nothing Then width = 0
                    If height = Nothing Then height = 0

                    Area = New RectangleF(X, Y, width, height)
                    BrushColor = setBrushColor

                End Sub

            End Class

        End Class




        Public Class SoundLevelMeter
            Inherits PictureBox

            Public Property minLevel As Single = -100
            Public Property maxLevel As Single = +12
            Public Property FullScaleLevel As Single = 0
            Public Property WarningLevel As Single = -4
            Private Property currentLevel As Single = minLevel
            Private Property memoryPeakLevel As Single
            Private currentLevelHeightInPixels As Single
            Private FullScaleLevelHeightInPixels As Single
            Private WarningLevelHeightInPixels As Single
            Private memoryPeakLevelHeightInPixels As Single
            Private maxMemory As New List(Of Single)
            Public Property Activated As Boolean = False

            Public Sub New()

                MyBase.New

                Me.BackColor = Color.White

                setPeakLevelMemoryItemCount()

            End Sub

            Public Sub setPeakLevelMemoryItemCount(Optional ItemCount As Integer = 10)

                For memoryItems = 0 To ItemCount - 1
                    maxMemory.Add(minLevel)
                Next

            End Sub

            Public Sub UpdateLevel(newLevel As Single)

                currentLevel = newLevel

                maxMemory.Add(currentLevel)
                maxMemory.RemoveAt(0)

                'Dim myRectangle As New RectangleF(Left, Top, Width, Height)
                'Dim myRegion As New Region(myRectangle)
                'Me.Invalidate(myRegion, True)
                Invalidate()

            End Sub

            Public Sub Activate()
                AddHandler Me.Paint, AddressOf drawMeter
                Activated = True
            End Sub

            Public Sub Inactivate()
                currentLevel = minLevel
                For n = 0 To maxMemory.Count - 1
                    maxMemory(n) = minLevel
                Next
                Refresh()

                RemoveHandler Me.Paint, AddressOf drawMeter
                Activated = False
            End Sub

            Private Sub drawMeter(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)

                Try

                    ' Create a local version of the graphics object for the PictureBox.
                    Dim g As System.Drawing.Graphics = e.Graphics

                    Dim dBRange As Single = Math.Abs(maxLevel - minLevel)


                    'Converting to pixels
                    currentLevelHeightInPixels = (Height / dBRange) * (currentLevel - minLevel)
                    FullScaleLevelHeightInPixels = (Height / dBRange) * (FullScaleLevel - minLevel)
                    WarningLevelHeightInPixels = (Height / dBRange) * (WarningLevel - minLevel)
                    memoryPeakLevelHeightInPixels = (Height / dBRange) * (maxMemory.Max - minLevel)

                    Dim greyBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(128, Drawing.SystemColors.Control))
                    Dim greenBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(128, Drawing.Color.Green))
                    Dim redBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(128, Drawing.Color.Red))
                    Dim yellowBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(128, Drawing.Color.Yellow))

                    Dim halfWidth As Single = Me.Width / 2
                    Dim dBLineStartX As Single = (Me.Width / 8) * 3
                    Dim dBLineEndX As Single = (Me.Width / 8) * 5
                    Dim dBStringYCorrection As Single = 5 '8 / 2 ' (Height / Rounding(Math.Abs(dBRange) / 6, AudioEnumerators.roundingMethods.alwaysUp))  '(which is Height / total number of dBs to display)/2


                    'Drawing backcolor rectangle on the values half in system control color
                    g.FillRectangle(greyBrush, 0, 0, halfWidth, Height)

                    'Draws level rectangles
                    Select Case currentLevel
                        Case <= WarningLevel
                            g.FillRectangle(greenBrush, halfWidth, Height - currentLevelHeightInPixels, halfWidth, currentLevelHeightInPixels)

                        Case > WarningLevel
                            If currentLevel < FullScaleLevel Then
                                g.FillRectangle(greenBrush, halfWidth, Height - WarningLevelHeightInPixels, halfWidth, WarningLevelHeightInPixels)
                                g.FillRectangle(yellowBrush, halfWidth, Height - WarningLevelHeightInPixels - (currentLevelHeightInPixels - WarningLevelHeightInPixels), halfWidth, currentLevelHeightInPixels - WarningLevelHeightInPixels)

                            Else
                                g.FillRectangle(greenBrush, halfWidth, Height - WarningLevelHeightInPixels, halfWidth, WarningLevelHeightInPixels)
                                g.FillRectangle(yellowBrush, halfWidth, Height - FullScaleLevelHeightInPixels, halfWidth, FullScaleLevelHeightInPixels - WarningLevelHeightInPixels)
                                g.FillRectangle(redBrush, halfWidth, Height - FullScaleLevelHeightInPixels - (currentLevelHeightInPixels - FullScaleLevelHeightInPixels), halfWidth, currentLevelHeightInPixels - FullScaleLevelHeightInPixels)

                            End If
                    End Select

                    Dim blackPen As New System.Drawing.Pen(System.Drawing.Color.Black, 1)
                    Dim grayPen As New System.Drawing.Pen(System.Drawing.Color.Gray, 1)
                    Dim redPen As New System.Drawing.Pen(System.Drawing.Color.Red, 1)


                    'Drawing zero line
                    g.DrawString((0).ToString, New Font("Arial", 7), Brushes.Blue, New PointF(0, Height - FullScaleLevelHeightInPixels - dBStringYCorrection))
                    Dim zy As Single = Height - FullScaleLevelHeightInPixels
                    g.DrawLine(blackPen, dBLineStartX, zy, Width, zy)

                    'Values from zero and above 
                    Dim valuesFromZeroAndUP As Integer = Utils.Rounding(Math.Abs(maxLevel) / 6, Utils.roundingMethods.alwaysUp)
                    For n = 1 To valuesFromZeroAndUP

                        'Drawing frequency numbers
                        g.DrawString((n * 6).ToString, New Font("Arial", 7), Brushes.Blue, New PointF(0, Height - FullScaleLevelHeightInPixels - 6 * n * (Height / dBRange) - dBStringYCorrection))

                        'Drawing lines
                        Dim y As Single = Height - FullScaleLevelHeightInPixels - 6 * n * (Height / dBRange)
                        g.DrawLine(blackPen, dBLineStartX, y, dBLineEndX, y)
                    Next


                    'Values below zero 
                    Dim valuesBelowZero As Integer = Utils.Rounding(Math.Abs(minLevel) / 6, Utils.roundingMethods.alwaysUp)
                    For n = 0 To valuesBelowZero - 1

                        'Drawing frequency numbers
                        g.DrawString((n * -6).ToString, New Font("Arial", 7), Brushes.Blue, New PointF(0, Height - FullScaleLevelHeightInPixels + 6 * n * (Height / dBRange) - dBStringYCorrection))

                        'Drawing lines
                        Dim y As Single = Height - FullScaleLevelHeightInPixels + 6 * n * (Height / dBRange)
                        g.DrawLine(blackPen, dBLineStartX, y, dBLineEndX, y)

                    Next

                    'Drawing memoryPeakLevel
                    Dim my As Single = Height - memoryPeakLevelHeightInPixels
                    g.DrawLine(redPen, halfWidth, my, Width, my)

                    'Drawing separator line
                    g.DrawLine(grayPen, halfWidth + 1, 0, halfWidth + 1, Height)
                    g.DrawLine(blackPen, halfWidth, 0, halfWidth, Height)

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub


        End Class


        Public Class SegmentationItemLabel
            Inherits Label

            Public Property SegmentationItem As Audio.Sound.SpeechMaterialAnnotation.SmaComponent

            Private RedPen As Pen = New System.Drawing.Pen(System.Drawing.Color.Red, 2)

            Private Sub SegmentationItemLabel_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

                If SegmentationItem IsNot Nothing Then
                    'Draws a red border around the item if its segmentation is not validated.
                    If SegmentationItem.SegmentationCompleted = False Then
                        e.Graphics.DrawRectangle(RedPen, New Rectangle(Me.ClientRectangle.X + 1, Me.ClientRectangle.Y + 1, Me.ClientRectangle.Width - 2, Me.ClientRectangle.Height - 2))
                    End If
                End If

            End Sub


        End Class


    End Namespace

End Namespace