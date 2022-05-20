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


            'Sound panels, etc
            Friend WithEvents myLayoutContainer As New SplitContainer
            Friend WithEvents soundContainerPanel As New Panel
            Friend WithEvents phonemesContainerPanel As New FlowLayoutPanel
            Friend WithEvents soundBackgroundArea As New System.Windows.Forms.PictureBox
            Friend WithEvents waveArea As New System.Windows.Forms.PictureBox
            Friend WithEvents spectrogramArea As New System.Windows.Forms.PictureBox
            Friend WithEvents timeArea As New System.Windows.Forms.PictureBox

            'Other panels, etc
            'Private buttonPanel As New FlowLayoutPanel
            Private changeWordButtonTexts As New List(Of String) From {"Previous Word", "Next Word"} ' Button text
            Private WithEvents soundScrollBar As New HScrollBar
            'Public testForm As New testForm
            Private WithEvents soundDisplayContextMenu As New ContextMenuStrip

            'Panel settings
            Private displayTypeCount As Integer = 1

            'Variables for which part of the sound that should be displayed and selected/highlighted
            Dim timeUnit As TimeUnits = TimeUnits.seconds

            Private selectionStartSample As Integer
            Private selectionLengthSample As Integer
            Private selectionStartPixel As Single
            Private selectionLengthPixel As Single
            Public Property displayStartSample As Integer
            Public Property displayLengthSamples As Integer

            Public Function GetSelectionStartSample() As Integer
                Return selectionStartSample
            End Function

            Public Function GetselectionLength() As Integer
                Return selectionLengthSample
            End Function

            'Graphic settings
            Private GraphicUpdateFrequency As Integer = 10

            'Sound data
            Private sound As Sound
            Private soundBackUp As Sound
            Private FS_pos As Double
            Private currentChannel As Integer
            Private SampleToPixelScale As Single
            Private wavePointsArray(1, 0) As Single 'used for long sounds, holding the max and min values of for a section of the wave file, in order to draw a vertical line, one pixel wide, between them, repressenting the wave form.
            Private waveLinePointArray() As System.Drawing.PointF 'used for short sounds
            Private normalizedWavePointsArray(1, 0) As Single 'used for long sounds
            Private normalizedWaveLinePointArray() As System.Drawing.PointF 'used for short sounds
            Private selectionCopy() As Single

            'Wave settings
            Private drawEverySampleLimitFactor As Integer = 10 '    2000  'TODO: One of the update functions is not synchonized!!! 'This is a factor that sets how many more samples than pixels there should be on the screen in order to swap wave drawing technique
            Private drawNormalizedWave As Boolean = False 'This determines if the normalized wave sound be drawn in the background

            'Spectrogram data
            Private showSpectrogram As Boolean
            Private spectrogramFormat As Formats.SpectrogramFormat
            Private spectrogramWindowDistance As Integer
            Private spectrogramDisplayDataArray As spectrogramDisplayData()
            Private spectrogramLightFactor As Single = 1
            Friend WithEvents spectrogramLightFactorScrollBar As New VScrollBar

            'Variables for assigning phoneme data
            Dim usePhonemeAssignment As Boolean
            Dim currentPtwfData As Sound.SpeechMaterialAnnotation ' List(Of Sound.ptwfData.SmaWordData)
            Dim currentWord As Integer = 0
            Dim currentPhonemeIndex As Integer
            Dim PaddingTime As Single        'padding time should be in seconds
            Dim SetSegmentationToZeroCrossings As Boolean

            'PaSoundPLayer
            Private CurrentSoundPlayer As PortAudioVB.SoundPlayer

            'Buttons
            Public ShowdetectBoundariesButton As Boolean
            Public ShowUpdateSegmentationButton As Boolean
            Public ShowFadePaddingButton As Boolean

            Private sentence As Integer = 0 'This holds the index of the sentence in each sound. Presently, multiple sentences sounds are not supported!

            'Setting things up
            Sub New(ByRef inputSound As Sound, ByRef containerPanel As SplitContainer,
                        Optional ByVal startSample As Integer = 0, Optional ByVal lengthInSamples As Integer = Nothing,
            Optional ByVal usePhonemeSegmentation As Boolean = False, Optional displaySpectrogram As Boolean = False,
                Optional ByRef setSpectrogramFormat As Formats.SpectrogramFormat = Nothing, Optional ByVal setPaddingTime As Single = 0,
                Optional setDrawNormalizedWave As Boolean = False, Optional ByRef PaSoundPlayer As PortAudioVB.SoundPlayer = Nothing,
                Optional SetSegmentationToZeroCrossings As Boolean = True,
                Optional ShowdetectBoundariesButton As Boolean = True,
                Optional ShowUpdateSegmentationButton As Boolean = True,
                Optional ShowFadePaddingButton As Boolean = True)

                Me.ShowdetectBoundariesButton = ShowdetectBoundariesButton
                Me.ShowUpdateSegmentationButton = ShowUpdateSegmentationButton
                Me.ShowFadePaddingButton = ShowFadePaddingButton

                Me.SetSegmentationToZeroCrossings = SetSegmentationToZeroCrossings
                Me.CurrentSoundPlayer = PaSoundPlayer

                drawNormalizedWave = setDrawNormalizedWave

                'Setting channel to 1 - ' This should be changed so that the panels can display stereo channels!!!
                currentChannel = 1 ' This should be changed so that the container can display stereo channels

                'Reading input data or creating default data
                usePhonemeAssignment = usePhonemeSegmentation

                sound = inputSound
                PaddingTime = setPaddingTime
                soundContainerPanel.Dock = DockStyle.Fill
                myLayoutContainer = containerPanel
                myLayoutContainer.Orientation = Orientation.Horizontal
                myLayoutContainer.Panel1.Controls.Add(soundContainerPanel)
                myLayoutContainer.IsSplitterFixed = True
                If usePhonemeAssignment = True Then
                    myLayoutContainer.Panel2.Controls.Add(phonemesContainerPanel)
                    phonemesContainerPanel.Height = 50
                    myLayoutContainer.SplitterDistance = myLayoutContainer.Height - phonemesContainerPanel.Height
                Else
                    myLayoutContainer.SplitterDistance = myLayoutContainer.Height
                End If

                showSpectrogram = displaySpectrogram

                If lengthInSamples = Nothing Then lengthInSamples = sound.WaveData.SampleData(currentChannel).Length
                If lengthInSamples < 2 Then lengthInSamples = 2
                displayStartSample = startSample
                displayLengthSamples = lengthInSamples

                'Creatig  back-up copy of the input sound
                soundBackUp = sound.CreateCopy

                'Setting full scale
                FS_pos = sound.WaveFormat.PositiveFullScale

                'Setting realtions between panels and pictureboxes
                soundBackgroundArea.Dock = DockStyle.Top
                soundContainerPanel.Controls.Add(soundBackgroundArea)
                If showSpectrogram = True Then

                    soundBackgroundArea.Controls.Add(spectrogramArea)
                    spectrogramArea.BackColor = Color.Transparent
                    spectrogramArea.BringToFront()
                    'soundContainerPanel.Controls.Add(spectrogramLightFactorScrollBar)
                    spectrogramArea.Controls.Add(spectrogramLightFactorScrollBar)
                    spectrogramLightFactorScrollBar.Dock = DockStyle.Right

                    'Setting the value of the spectrogram light bar
                    spectrogramLightFactorScrollBar.Minimum = 0
                    spectrogramLightFactorScrollBar.Maximum = 100
                    spectrogramLightFactorScrollBar.Value = spectrogramLightFactorScrollBar.Maximum - 20 - spectrogramLightFactor
                Else

                End If
                soundBackgroundArea.Controls.Add(waveArea)
                waveArea.BackColor = Color.Transparent
                waveArea.BringToFront()
                soundBackgroundArea.Controls.Add(timeArea)
                timeArea.BackColor = Color.Transparent
                timeArea.BringToFront()
                soundContainerPanel.Controls.Add(soundScrollBar)

                'Adding borders to the sound boxes
                timeArea.BorderStyle = BorderStyle.FixedSingle
                waveArea.BorderStyle = BorderStyle.FixedSingle
                spectrogramArea.BorderStyle = BorderStyle.FixedSingle
                soundBackgroundArea.BorderStyle = BorderStyle.Fixed3D


                'calculating spectrogram data
                'Creating a temporary setting
                If showSpectrogram = True Then
                    displayTypeCount += 1

                    'Creating a default spectrogram format if needed
                    If setSpectrogramFormat Is Nothing Then
                        spectrogramFormat = New Formats.SpectrogramFormat(,,,,,,,,, True)
                    Else
                        spectrogramFormat = setSpectrogramFormat
                    End If

                    spectrogramWindowDistance = spectrogramFormat.SpectrogramFftFormat.AnalysisWindowSize - spectrogramFormat.SpectrogramFftFormat.OverlapSize

                    sound.FFT = New FftData(sound.WaveFormat, spectrogramFormat.SpectrogramFftFormat)
                    sound.FFT.CalculateSpectrogramData(sound, spectrogramFormat, currentChannel)
                End If

                'Loading phoneme data
                If usePhonemeAssignment = True Then
                    phonemesContainerPanel.FlowDirection = FlowDirection.LeftToRight
                    phonemesContainerPanel.Dock = DockStyle.Bottom
                    'phonemesContainerPanel.AutoSize = True
                    'phonemesContainerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink
                    phonemesContainerPanel.AutoScroll = True
                    myLayoutContainer.Panel2MinSize = 30
                    LoadAndModifyPhonemeLevelData()
                Else
                    myLayoutContainer.Panel2MinSize = 0
                    myLayoutContainer.SplitterDistance = myLayoutContainer.Height
                End If

                'Adding event handler for spectrogram. Paint events for other drawing areas are declared by Handles statments on the appropriate subs
                If showSpectrogram = True Then
                    AddHandler spectrogramArea.Paint, AddressOf Me.drawSpectrogram
                End If

                'Creating the context menu
                createContextMenu()
                upDatePanel1DisplayData()

                'Adding handlers for the wave and spectrogram areas
                AddHandler waveArea.MouseDown, AddressOf Me.container_MouseDown
                If showSpectrogram = True Then AddHandler spectrogramArea.MouseDown, AddressOf Me.container_MouseDown

            End Sub
            Private Sub LoadAndModifyPhonemeLevelData()

                'This step is skipped. Instead word end markers are only added below if they don't already exist. Removing any remaining word end marker (so that it would not contain two word end strings, if the word is resegmented)
                'sound.SMA.RemoveWordEndString()

                'This step is also removed (2019-04-04), as a phonetic transcription should always exist if phonetic segmentation is activated.
                'Adds "Word start" to the phoneme list if no phonemes exist. Doing this enables word level segmentation, since "Word end is added later in Graphics"
                'sound.SMA.AddWordStartString()

                'Adding word end string so that the end of the word can be located. However, if one is already added, no new word end string is added
                sound.SMA.AddWordEndString()

                'currentSentenceData = sound.SMA.ChannelData(currentChannel)

                addButtons()

            End Sub

            Private Sub addButtons()

                If sound.SMA.ChannelData(currentChannel)(sentence).Count > 0 Then

                    For n = 0 To 1
                        Dim ChangeWordButton As New Button
                        With ChangeWordButton
                            .Text = changeWordButtonTexts(n)
                            .Name = changeWordButtonTexts(n).Replace(" ", "") 'storing the identity as an index reference to the changeWordButtonTexts list 
                            .TextAlign = ContentAlignment.MiddleCenter
                            .AutoSize = True
                            .AutoSizeMode = AutoSizeMode.GrowAndShrink
                            '.BackColor = Color.LightGray
                            '.Font = New Font(New Font.Size(20), FontStyle.Bold)
                        End With

                        'Adding eventhandler
                        AddHandler ChangeWordButton.Click, AddressOf changeWordButtonClick

                        'Adding the control
                        phonemesContainerPanel.Controls.Add(ChangeWordButton)

                    Next

                End If


                For index = 0 To sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData.Count - 1
                    Dim phonemeLabelButton As New Label

                    With phonemeLabelButton
                        .Text = "[ " & sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(index).PhoneticForm & " ]"
                        .Name = index.ToString 'storing the identity of the phoneme as an index reference to the phoneme list 
                        .BorderStyle = BorderStyle.Fixed3D
                        .TextAlign = ContentAlignment.MiddleCenter
                        .BackColor = Color.LightGray
                        .AutoSize = True
                        .Font = New Font("Times New Roman", 14.0F, FontStyle.Regular) 'TODO: It would be good to be able to change this font family, and size
                    End With

                    'Adding eventhandler
                    AddHandler phonemeLabelButton.Click, AddressOf phonemeLabelButtonClick

                    'Adding margin
                    phonemeLabelButton.Margin = New Padding(4)

                    'Adding the control
                    phonemesContainerPanel.Controls.Add(phonemeLabelButton)
                Next


                'Adding a Button for automatic word boundary detection
                If ShowdetectBoundariesButton = True Then
                    Dim detectBoundariesButton As New Button

                    With detectBoundariesButton
                        .Text = "Detect boundaries"
                        .Name = "detectBoundariesButton"
                        .TextAlign = ContentAlignment.MiddleCenter
                        .BackColor = Color.LightGray
                        .AutoSize = True
                        '.Font = New Font(.Font.Name, 13, FontStyle.Regular, .Font.Unit)
                        '.Font = New Font(New Font.Size(20), FontStyle.Bold)
                    End With

                    'Adding eventhandler
                    AddHandler detectBoundariesButton.Click, AddressOf detectBoundariesButtonClick

                    'Adding the control
                    phonemesContainerPanel.Controls.Add(detectBoundariesButton)
                End If



                'Adding UpdateSegmentationButton button
                If ShowUpdateSegmentationButton = True Then
                    Dim UpdateSegmentationButton As New Button

                    With UpdateSegmentationButton
                        .Text = "Update segmentation"
                        .Name = "UpdateSegmentation"
                        .TextAlign = ContentAlignment.MiddleCenter
                        .BackColor = Color.LightGray
                        .AutoSize = True
                        '.Font = New Font(.Font.Name, 13, FontStyle.Regular, .Font.Unit)
                        '.Font = New Font(New Font.Size(20), FontStyle.Bold)
                    End With

                    'Adding eventhandler
                    AddHandler UpdateSegmentationButton.Click, AddressOf UpdateSegmentationButtonClick

                    'Adding the control
                    phonemesContainerPanel.Controls.Add(UpdateSegmentationButton)
                End If


                'Adding a fade padding Button
                If ShowFadePaddingButton = True Then

                    Dim FadePaddingButton As New Button

                    With FadePaddingButton
                        .Text = "Fade padding"
                        .Name = "FadePadding"
                        .TextAlign = ContentAlignment.MiddleCenter
                        .BackColor = Color.LightGray
                        .AutoSize = True
                        '.Font = New Font(.Font.Name, 13, FontStyle.Regular, .Font.Unit)
                        '.Font = New Font(New Font.Size(20), FontStyle.Bold)
                    End With

                    'Adding eventhandler
                    AddHandler FadePaddingButton.Click, AddressOf FadePaddingButtonClick

                    'Adding the control
                    phonemesContainerPanel.Controls.Add(FadePaddingButton)
                End If


            End Sub
            Private Sub createContextMenu()

                Dim menuItemNameList As New List(Of String) From {"play", "playAll", "stopSound", "zoomOut", "zoomIn", "zoomToSelection", "zoomFull", "smoothFadeIn", "smoothFadeOut", "linearFadeIn", "linearFadeOut", "silenceSelection", "silenceSelectionZeroCross", "copy", "cut", "paste", "delete", "crop", "undoAll"}
                Dim menuItemTextList As New List(Of String) From {"Play", "Play all", "Stop", "Zoom out", "Zoom in", "Zoom to selection", "Zoom full", "Fade in selection (smooth)", "Fade out selection (smooth)", "Fade in selection (linear)", "Fade out selection (linear)", "Silence selection", "Silence selection (search zero crossings)", "Copy", "Cut", "Paste", "Delete", "Crop", "Undo all"}

                For item = 0 To menuItemNameList.Count - 1
                    Dim menuItem As New ToolStripMenuItem
                    menuItem.Name = menuItemNameList(item)
                    menuItem.Text = menuItemTextList(item)
                    soundDisplayContextMenu.Items.Add(menuItem)
                Next

                'AddHandler soundDisplayContextMenu.ItemClicked, AddressOf menuItem_CLick

            End Sub

            'Resetting data
            Private Sub resetCurrentWordLevelSegmentationData()

                If usePhonemeAssignment = True Then
                    For phoneme = 0 To sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData.Count - 1
                        sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(phoneme).StartSample = -1
                        sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(phoneme).Length = -1
                    Next
                End If

                InitialSegmentationIsDone = False

            End Sub
            Private Sub retriveSoundBackUp()

                'Undoes all modifications to the input sound and restores the input sound array
                sound = soundBackUp.CreateCopy

                If showSpectrogram = True Then UpdateSpectrogramData()

                InitialSegmentationIsDone = False

                upDatePanel1DisplayData()

            End Sub

            Private Sub UpdateSpectrogramData()

                'Updating fft data
                sound.FFT = New FftData(sound.WaveFormat, spectrogramFormat.SpectrogramFftFormat)
                sound.FFT.CalculateSpectrogramData(sound, spectrogramFormat, currentChannel)

            End Sub

            'Calculating sound visualization data (and container sizes)
            Private Sub UpdateSampleTimeScale()

                'Calculates how many samples each pixel repressents
                SampleToPixelScale = (displayLengthSamples - 1) / soundBackgroundArea.Width

            End Sub

            Public Sub UpdateLayout()
                upDatePanel1DisplayData()
            End Sub

            Private Sub upDatePanel1DisplayData()

                'This sub recalculates all sound display data, and container positions and sizes

                Try

                    'Setting the number of samples to display
                    If displayStartSample < 0 Then displayStartSample = 0
                    If displayStartSample > sound.WaveData.SampleData(currentChannel).Length - 1 Then displayStartSample = sound.WaveData.SampleData(currentChannel).Length - 1
                    If displayStartSample + displayLengthSamples > sound.WaveData.SampleData(currentChannel).Length Then displayLengthSamples = sound.WaveData.SampleData(currentChannel).Length - displayStartSample

                    'Sets the scroll bar scale
                    soundScrollBar.Minimum = 0
                    soundScrollBar.Maximum = sound.WaveData.SampleData(currentChannel).Length - displayLengthSamples
                    soundScrollBar.Value = displayStartSample

                    'Updating the size of the graphic elements
                    'If usePhonemeAssignment = True Then myLayoutContainer.SplitterDistance = myLayoutContainer.Height - 20

                    soundBackgroundArea.Left = 0
                    soundScrollBar.Left = 0
                    'phonemesContainerPanel.Left = 0

                    soundBackgroundArea.Width = myLayoutContainer.Width
                    soundScrollBar.Width = myLayoutContainer.Width
                    'phonemesContainerPanel.Width = myLayoutContainer.Width

                    Dim drawAreaHeight = soundContainerPanel.Height - soundScrollBar.Height
                    'If drawAreaHeight < buttonPanel.Height - hScBar.Height Then instanceContainerPanel.Height = drawAreaHeight + buttonPanel.Height + hScBar.Height
                    soundBackgroundArea.Height = drawAreaHeight

                    soundBackgroundArea.Top = 0
                    soundScrollBar.Top = soundBackgroundArea.Bottom
                    'buttonPanel.Top = soundScrollBar.Bottom


                    'Updating size and location of the controls inside soundBackgroundArea
                    timeArea.Height = 20
                    timeArea.Width = soundBackgroundArea.Width
                    Select Case displayTypeCount
                        Case 1
                            waveArea.Top = 0
                            waveArea.Height = soundBackgroundArea.Height - timeArea.Height
                            waveArea.Width = soundBackgroundArea.Width
                            timeArea.Top = waveArea.Bottom
                        Case 2
                            waveArea.Top = 0
                            waveArea.Width = soundBackgroundArea.Width
                            waveArea.Height = (soundBackgroundArea.Height - timeArea.Height) / 2

                            spectrogramArea.Width = soundBackgroundArea.Width
                            spectrogramArea.Height = (soundBackgroundArea.Height - timeArea.Height) / 2
                            spectrogramArea.Top = waveArea.Height
                            timeArea.Top = spectrogramArea.Bottom

                        Case 3
                            Throw New NotImplementedException

                    End Select

                    'Updates the scale according to the current size of the sound display
                    UpdateSampleTimeScale()
                    'Updating Y-scale
                    Dim YscaleToPixel_Wave As Single
                    YscaleToPixel_Wave = (waveArea.Height / 2) / FS_pos


                    'Updating wave data - Chosing method to update depending on the size of the sound to display
                    Select Case displayLengthSamples
                        Case Is > waveArea.Width * drawEverySampleLimitFactor
                            'timeArea.BackColor = Color.Red
                            ReDim wavePointsArray(1, waveArea.Width)

                            Dim sectionMax As Double
                            Dim sectionMin As Double

                            For CurrentXpixel As Single = 0 To waveArea.Width - 1 Step 1

                                'find section max and min
                                sectionMax = -FS_pos
                                sectionMin = FS_pos
                                For sample = CInt((CurrentXpixel * SampleToPixelScale) + displayStartSample) To CInt((CurrentXpixel + 1) * SampleToPixelScale + displayStartSample - 1)
                                    If sound.WaveData.SampleData(currentChannel)(sample) < sectionMin Then sectionMin = sound.WaveData.SampleData(currentChannel)(sample)
                                    If sound.WaveData.SampleData(currentChannel)(sample) > sectionMax Then sectionMax = sound.WaveData.SampleData(currentChannel)(sample)
                                Next

                                wavePointsArray(0, CurrentXpixel) = ((waveArea.Height) / 2) - (sectionMax * YscaleToPixel_Wave)
                                wavePointsArray(1, CurrentXpixel) = ((waveArea.Height) / 2) - (sectionMin * YscaleToPixel_Wave)
                            Next

                        Case Else
                            'timeArea.BackColor = Color.Cyan
                            ReDim waveLinePointArray(displayLengthSamples - 1)
                            Dim currentPoint As New System.Drawing.PointF(0, 0)
                            For point = 0 To waveLinePointArray.Length - 1

                                currentPoint.X = point / SampleToPixelScale
                                currentPoint.Y = ((waveArea.Height) / 2) - (sound.WaveData.SampleData(currentChannel)(point + displayStartSample) * YscaleToPixel_Wave)
                                waveLinePointArray(point) = currentPoint

                            Next
                    End Select

                    If drawNormalizedWave = True Then

                        'TODO: find out what the max value is
                        Dim sectionAbsMaxValue As Single = DSP.MeasureSectionLevel(sound, currentChannel, displayStartSample, displayLengthSamples, SoundDataUnit.linear,
                                                  SoundMeasurementType.AbsolutePeakAmplitude)

                        'Updating the Creating a new scale
                        Dim normalizedYscaleToPixel_Wave = (waveArea.Height / 2) / sectionAbsMaxValue

                        'Create a normalised version of the wavedata above
                        'Updating wave data - Chosing method to update depending on the size of the sound to display
                        Select Case displayLengthSamples
                            Case Is > waveArea.Width * drawEverySampleLimitFactor
                                'timeArea.BackColor = Color.Red
                                ReDim normalizedWavePointsArray(1, waveArea.Width - 1)
                                Dim sectionMax As Double
                                Dim sectionMin As Double
                                For CurrentXpixel = 0 To normalizedWavePointsArray.GetUpperBound(1) - 2

                                    'find section max and min
                                    sectionMax = -FS_pos
                                    sectionMin = FS_pos
                                    For sample = CInt(CurrentXpixel * SampleToPixelScale + displayStartSample) To CInt((CurrentXpixel + 1) * SampleToPixelScale + displayStartSample - 1)
                                        If sound.WaveData.SampleData(currentChannel)(sample) < sectionMin Then sectionMin = sound.WaveData.SampleData(currentChannel)(sample)
                                        If sound.WaveData.SampleData(currentChannel)(sample) > sectionMax Then sectionMax = sound.WaveData.SampleData(currentChannel)(sample)
                                    Next

                                    normalizedWavePointsArray(0, CurrentXpixel) = ((waveArea.Height) / 2) - (sectionMax * normalizedYscaleToPixel_Wave)
                                    normalizedWavePointsArray(1, CurrentXpixel) = ((waveArea.Height) / 2) - (sectionMin * normalizedYscaleToPixel_Wave)
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
                                ReDim normalizedWaveLinePointArray(displayLengthSamples - 1)
                                Dim currentPoint As New System.Drawing.PointF(0, 0)
                                For point = 0 To normalizedWaveLinePointArray.Length - 1

                                    currentPoint.X = point / SampleToPixelScale
                                    currentPoint.Y = ((waveArea.Height) / 2) - (sound.WaveData.SampleData(currentChannel)(point + displayStartSample) * normalizedYscaleToPixel_Wave)
                                    normalizedWaveLinePointArray(point) = currentPoint
                                Next

                        End Select


                    End If


                    If showSpectrogram = True Then
                        ReCalculateSpectrogramData()
                    End If

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

                InvalidateGraphics()

                'RefreshGraphicElements()

            End Sub

            Private Sub ReCalculateSpectrogramData()

                Try

                    'Updating spectrogram data

                    Dim YscaleToPixel_Spectrogram As Single
                    YscaleToPixel_Spectrogram = spectrogramArea.Height / sound.FFT.SpectrogramData(currentChannel, 0).WindowData.Length

                    'Localizing the position of the first spectrogram window to display
                    Dim displayStartColumn As Integer = Utils.Rounding(displayStartSample / spectrogramWindowDistance, Utils.roundingMethods.alwaysDown)
                    Dim samplesOfFirstWindowOutsideDisplay As Integer = (displayStartColumn * spectrogramWindowDistance) - displayStartSample
                    Dim pixelsOfFirstWindowOutsideDisplay As Single = (samplesOfFirstWindowOutsideDisplay / SampleToPixelScale)

                    'Localizing the position of the last spectrogram window to display
                    Dim displayLastColumn As Integer = Utils.Rounding((displayStartSample + displayLengthSamples) / spectrogramWindowDistance, Utils.roundingMethods.alwaysDown)

                    'Calculating number of windows
                    Dim displayColumnCount As Integer = displayLastColumn - displayStartColumn

                    'Selecting update method depending on the width of the presentation panel
                    Select Case displayColumnCount
                        Case Is <= spectrogramArea.Width

                            'testForm.startvalueLabel.Text = 0
                            'testForm.startvalueLabel.Refresh()

                            'Updating spectrogram X-scale
                            Dim columnsSizeInPixels As Single = spectrogramWindowDistance / SampleToPixelScale
                            Dim analysisWindowLengthInPixels As Single = spectrogramWindowDistance / SampleToPixelScale

                            Dim newLowResolutionSpectrogramArea(displayColumnCount * sound.FFT.SpectrogramData(currentChannel, 0).WindowData.Length - 1) As spectrogramDisplayData

                            'Creating display data
                            Dim drawingSurface As Integer = 0
                            For columnNumber = displayStartColumn To displayStartColumn + displayColumnCount - 1
                                For coeffNumber = 0 To sound.FFT.SpectrogramData(currentChannel, 0).WindowData.Length - 1

                                    Dim newBrushGradient As Integer '= sound.FFT.SpectrogramData(currentChannel, columnNumber)(coeffNumber) * spectrogramLightFactor
                                    Try
                                        newBrushGradient = sound.FFT.SpectrogramData(currentChannel, columnNumber).WindowData(coeffNumber) * spectrogramLightFactor
                                    Catch ex As Exception
                                        MsgBox(ex.ToString)
                                    End Try
                                    If newBrushGradient > 255 Then newBrushGradient = 255
                                    Dim newBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(newBrushGradient, Drawing.Color.Black))
                                    'newLowResolutionSpectrogramArea(drawingPixel) = New spectrogramDisplayData(newBrush, xPixel, YPixelCount - yPixel, 1, 1)

                                    newLowResolutionSpectrogramArea(drawingSurface) = New spectrogramDisplayData(newBrush, ((columnNumber - displayStartColumn) * columnsSizeInPixels + pixelsOfFirstWindowOutsideDisplay),
                            (spectrogramArea.Height - coeffNumber * YscaleToPixel_Spectrogram),
                            columnsSizeInPixels, YscaleToPixel_Spectrogram)

                                    drawingSurface += 1
                                Next
                            Next

                            spectrogramDisplayDataArray = newLowResolutionSpectrogramArea


                        Case Is > spectrogramArea.Width

                            'testForm.startvalueLabel.Text = 1
                            'testForm.startvalueLabel.Refresh()

                            Dim XPixelCount As Integer = spectrogramArea.Width
                            Dim YPixelCount As Single = spectrogramArea.Height

                            Dim columnScaleToPixel As Single = displayColumnCount / XPixelCount
                            Dim binScaleToPixel As Single = sound.FFT.SpectrogramData(currentChannel, 0).WindowData.Length / YPixelCount

                            Dim newLowResolutionSpectrogramArea(XPixelCount * YPixelCount - 1) As spectrogramDisplayData

                            Dim drawingPixel As Integer = 0
                            For xPixel = 0 To XPixelCount - 1
                                For yPixel = 0 To YPixelCount - 1

                                    Try
                                        Dim newBrushGradient As Integer = sound.FFT.SpectrogramData(currentChannel, Math.Round(displayStartColumn + xPixel * columnScaleToPixel)).WindowData(Math.Round(yPixel * binScaleToPixel)) * spectrogramLightFactor
                                        If newBrushGradient > 255 Then newBrushGradient = 255
                                        Dim newBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(newBrushGradient, Drawing.Color.Black))
                                        newLowResolutionSpectrogramArea(drawingPixel) = New spectrogramDisplayData(newBrush, xPixel, YPixelCount - yPixel, 1, 1)
                                        drawingPixel += 1
                                    Catch ex As Exception
                                        MsgBox(ex.ToString)
                                    End Try

                                Next
                            Next

                            spectrogramDisplayDataArray = newLowResolutionSpectrogramArea

                    End Select

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try


            End Sub

            'Drawing graphic elements
            Private Sub drawWave(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles waveArea.Paint

                Try

                    ' Create a local version of the graphics object for the PictureBox.
                    Dim g As System.Drawing.Graphics = e.Graphics

                    'Drawing normalized wave data
                    If drawNormalizedWave = True Then

                        ' Create pen.
                        Dim grayPen As New System.Drawing.Pen(System.Drawing.Color.Gray, 1)

                        Select Case displayLengthSamples
                            Case Is > waveArea.Width * drawEverySampleLimitFactor
                                For points = 0 To normalizedWavePointsArray.GetUpperBound(1) - 1
                                    g.DrawLine(grayPen, points, normalizedWavePointsArray(0, points), points, normalizedWavePointsArray(1, points))
                                Next
                            Case Else
                                g.DrawLines(grayPen, normalizedWaveLinePointArray)

                        End Select

                    End If


                    ' Create pen.
                    Dim blackPen As New System.Drawing.Pen(System.Drawing.Color.Black, 1)

                    'Draws the wave display type
                    'Draws 0 - line in wave area
                    ' Create points that define 0 line.
                    Dim point1 As New System.Drawing.Point(0, waveArea.Height / 2)
                    Dim point2 As New System.Drawing.Point(waveArea.Width, waveArea.Height / 2)
                    g.DrawLine(blackPen, point1, point2)


                    'Draws wave form
                    Select Case displayLengthSamples
                        Case Is > waveArea.Width * drawEverySampleLimitFactor
                            For points = 0 To wavePointsArray.GetUpperBound(1) - 1
                                g.DrawLine(blackPen, points, wavePointsArray(0, points), points, wavePointsArray(1, points))
                            Next
                        Case Else
                            g.DrawLines(blackPen, waveLinePointArray)

                    End Select


                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub
            Private Sub drawSpectrogram(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)

                Try

                    ' Create a local version of the graphics object for the PictureBox.
                    Dim g As System.Drawing.Graphics = e.Graphics

                    'Draws spectrogram
                    For n = 0 To spectrogramDisplayDataArray.Count - 1
                        g.FillRectangle(spectrogramDisplayDataArray(n).brushColor, spectrogramDisplayDataArray(n).area)
                    Next


                    'Draws frequencies on the spectrogram area

                    'Dim blackPen As New System.Drawing.Pen(System.Drawing.Color.Black, 1)
                    Dim currentPen As New System.Drawing.Pen(System.Drawing.Color.Red, 1)

                    Dim valuesToWrite As Integer = Utils.Rounding(spectrogramFormat.SpectrogramCutFrequency / 1000, Utils.roundingMethods.alwaysUp)

                    For n = 0 To valuesToWrite - 1

                        'Drawing frequency numbers
                        g.DrawString((n * 1000).ToString, New Font("Arial", 7), Brushes.Blue, New PointF(10, spectrogramArea.Height - n * (spectrogramArea.Height / (spectrogramFormat.SpectrogramCutFrequency / 1000)) - 5))

                        'Drawing lines
                        Dim y As Single = spectrogramArea.Height - n * (spectrogramArea.Height / (spectrogramFormat.SpectrogramCutFrequency / 1000))
                        g.DrawLine(currentPen, 0, y, 7, y)

                    Next

                    'Drawing unit
                    g.DrawString("(Hz)", New Font("Arial", 7), Brushes.Blue, New PointF(0, 0))

                    'Drawing the spectrogram settings on the spectrogram area
                    Dim drawSpetrogramSettings As Boolean = True
                    If drawSpetrogramSettings = True Then

                        g.DrawString("Frequency resolution (FFT size in samples): " & spectrogramFormat.SpectrogramFftFormat.FftWindowSize.ToString, New Font("Arial", 7), Brushes.Blue, New PointF(40, 4))
                        g.DrawString("Filter FFT size (samples): " & spectrogramFormat.SpectrogramPreFirFilterFftFormat.FftWindowSize.ToString, New Font("Arial", 7), Brushes.Blue, New PointF(40, 14))
                        g.DrawString("Filter kernel creation FFT size (samples): " & spectrogramFormat.SpectrogramPreFilterKernelFftFormat.FftWindowSize.ToString, New Font("Arial", 7), Brushes.Blue, New PointF(40, 24))

                    End If

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub
            Private Sub drawTime(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles timeArea.Paint

                'Empties the container before it is redrawn
                'timeArea.Invalidate()

                ' Create a local version of the graphics object for the PictureBox.
                Dim g As System.Drawing.Graphics = e.Graphics
                Dim blackPen As New System.Drawing.Pen(System.Drawing.Color.Black, 1)
                Dim currentPen As New System.Drawing.Pen(System.Drawing.Color.Red, 2)

                Select Case timeUnit
                    Case TimeUnits.samples

                        Dim valuesToWrite As Integer = 10
                        For n = 0 To valuesToWrite - 1

                            'Drawing sample numbers
                            g.DrawString(displayStartSample + n * (displayLengthSamples / valuesToWrite), New Font("Arial", 7), Brushes.Blue, New PointF(n * (timeArea.Width / valuesToWrite), 2))

                            'Drawing lines
                            Dim x As Single = n * (timeArea.Width / valuesToWrite)
                            g.DrawLine(currentPen, x, 0, x, timeArea.Height)

                        Next

                        'Drawing unit
                        g.DrawString("(Samples)", New Font("Arial", 7), Brushes.Blue, New PointF(timeArea.Width - 50, 2))

                    Case TimeUnits.seconds

                        Dim valuesToWrite As Integer = 10
                        For n = 0 To valuesToWrite - 1

                            'Drawing sample numbers
                            g.DrawString(Math.Round((displayStartSample + n * (displayLengthSamples / valuesToWrite)) / sound.WaveFormat.SampleRate, 3), New Font("Arial", 7), Brushes.Blue, New PointF(n * (timeArea.Width / valuesToWrite) + 1, 2))

                            'Drawing lines
                            Dim x As Single = n * (timeArea.Width / valuesToWrite)
                            g.DrawLine(currentPen, x, 0, x, timeArea.Height)

                        Next

                        'Drawing unit
                        g.DrawString("(s)", New Font("Arial", 7), Brushes.Blue, New PointF(timeArea.Width - 25, 2))

                    Case Else
                        Throw New NotSupportedException
                End Select


            End Sub
            Private Sub drawSelection(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles soundBackgroundArea.Paint

                Try

                    'Empties the container before it is redrawn
                    'soundBackgroundArea.Invalidate()

                    ' Create a local version of the graphics object for the PictureBox.
                    Dim g As System.Drawing.Graphics = e.Graphics

                    ' Create pen.
                    Dim blackPen As New System.Drawing.Pen(System.Drawing.Color.Black, 1)

                    'Drawing phoneme boundary lines and labels
                    If usePhonemeAssignment = True Then
                        If sound.SMA.ChannelData(currentChannel)(sentence).Count > 0 Then

                            Dim currentPen As New System.Drawing.Pen(System.Drawing.Color.Red, 2)
                            'Dim TempPen As New Pen(Color.FromArgb(0, 91, 192), 5)


                            For phoneme = 0 To sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData.Count - 1
                                'Drawing line

                                Dim phonemeStartPixel As Single = (sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(phoneme).StartSample - displayStartSample) / SampleToPixelScale
                                If Not (sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(phoneme).StartSample) < 0 Then ' Is used to "hide" the lines and phoneme strings if they are not set. Also means that they cannot be displayed if they are set to 0. 
                                    g.DrawLine(currentPen, phonemeStartPixel, soundBackgroundArea.Top, phonemeStartPixel, soundBackgroundArea.Height)

                                    'Adding phoneme string
                                    If showSpectrogram = True Then
                                        'Putting the phoneme string in the middle of the background panel
                                        g.DrawString(sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(phoneme).PhoneticForm,
                                              New Font("Arial", 20), Brushes.Blue, New PointF(phonemeStartPixel, soundBackgroundArea.Height / 2 - 14))
                                    Else
                                        'Putting the phoneme string in the bottom of the background panel, above the time scale
                                        'Dim TempPhonemeColorBrush As New SolidBrush(Color.FromArgb(0, 91, 192))

                                        g.DrawString(sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(phoneme).PhoneticForm,
                                              New Font("Arial", 20), Brushes.Black, New PointF(phonemeStartPixel, soundBackgroundArea.Height - 55))

                                        'g.DrawString(sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(phoneme).Phoneme,
                                        '         New Font("Doulos SIL", 40), Brushes.Black, New PointF(phonemeStartPixel, soundBackgroundArea.Height - 105))
                                    End If

                                End If
                            Next
                        End If
                    End If


                    'drawing selection (it also draws a selection of length 0 (which is displayed as a single line) with start index 0, if no selection is made)
                    Dim greenBrush As New Drawing.SolidBrush(Drawing.Color.FromArgb(128, Drawing.Color.Gray))
                    'Dim myBrush As New System.Drawing.Drawing2D.LinearGradientBrush(selection, Drawing.Color.Red, Drawing.Color.Transparent,
                    'System.Drawing.Drawing2D.LinearGradientMode.Vertical)
                    'Dim aHatchBrush As New System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.SmallGrid,
                    'Drawing.Color.Red, Drawing.Color.Transparent)

                    'Converting selection data pixels
                    If selectionLengthSample > 0 Then

                        'UpdateScale()
                        Dim selectedPeriodStartPixel As Single = (selectionStartSample - displayStartSample) / SampleToPixelScale
                        Dim selectedPeriodLengthPixel As Single = selectionLengthSample / SampleToPixelScale

                        'Draws a shaded area
                        g.FillRectangle(greenBrush, selectedPeriodStartPixel, soundBackgroundArea.Top, selectedPeriodLengthPixel, soundBackgroundArea.Height)

                        'Draw lines 
                        g.DrawLine(blackPen, selectedPeriodStartPixel, soundBackgroundArea.Top, selectedPeriodStartPixel, soundBackgroundArea.Height)
                        g.DrawLine(blackPen, selectedPeriodStartPixel + selectedPeriodLengthPixel, soundBackgroundArea.Top, selectedPeriodStartPixel + selectedPeriodLengthPixel, soundBackgroundArea.Height)

                        'Drawing selection times
                        'Start time
                        Select Case timeUnit
                            Case TimeUnits.samples
                                g.DrawString(selectionStartSample, New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel, 10))
                                g.DrawString(selectionStartSample + selectionLengthSample, New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel + selectedPeriodLengthPixel + 1, 15))

                                'Also drawing selection length
                                g.DrawString("(" & selectionLengthSample & ")", New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel + selectedPeriodLengthPixel + 1, 30))

                            Case TimeUnits.seconds
                                g.DrawString(Math.Round(selectionStartSample / sound.WaveFormat.SampleRate, 3), New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel, 10))
                                g.DrawString(Math.Round((selectionStartSample + selectionLengthSample) / sound.WaveFormat.SampleRate, 3), New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel + selectedPeriodLengthPixel + 1, 20))

                                'Also drawing selection length
                                g.DrawString("(" & Math.Round(selectionLengthSample / sound.WaveFormat.SampleRate, 3) & ")", New Font("Arial", 8), Brushes.Blue, New PointF(selectedPeriodStartPixel + selectedPeriodLengthPixel + 1, 40))

                            Case Else
                                Throw New NotSupportedException
                        End Select

                    Else
                        'Drawing cursor
                        If SampleToPixelScale > 0 Then
                            Dim selectedPeriodStartPixel As Single = (selectionStartSample - displayStartSample) / SampleToPixelScale
                            g.DrawLine(blackPen, selectedPeriodStartPixel, soundBackgroundArea.Top, selectedPeriodStartPixel, soundBackgroundArea.Height)
                        End If
                    End If

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub

            Private Sub InvalidateGraphics()

                'Invalidates each control of the sound area so that they are redrawn on next draw event
                Dim mySoundDisplayRectangle As New Rectangle(myLayoutContainer.Panel1.Left, myLayoutContainer.Panel1.Top,
                                      myLayoutContainer.Panel1.Width, myLayoutContainer.Panel1.Height)
                myLayoutContainer.Panel1.Invalidate(mySoundDisplayRectangle, True)

            End Sub

            'Taking care of user input
            '   - Things that concern the sound drawing area
            Private Sub container_MouseDown(sender As System.Object, e As MouseEventArgs)

                If e.Button = MouseButtons.Left Then
                    RemoveHandler waveArea.MouseDown, AddressOf Me.container_MouseDown
                    If showSpectrogram = True Then RemoveHandler spectrogramArea.MouseDown, AddressOf Me.container_MouseDown

                    selectionStartPixel = e.X
                    selectionLengthPixel = 0

                    InvalidateGraphics()

                    AddHandler waveArea.MouseMove, AddressOf Me.container_MouseMove
                    AddHandler waveArea.MouseUp, AddressOf Me.container_MouseUp
                    If showSpectrogram = True Then AddHandler spectrogramArea.MouseMove, AddressOf Me.container_MouseMove
                    If showSpectrogram = True Then AddHandler spectrogramArea.MouseUp, AddressOf Me.container_MouseUp

                End If


                If e.Button = MouseButtons.Right Then

                    'Displaying the context menu
                    soundDisplayContextMenu.Location = e.Location 'Cursor.Position
                    soundDisplayContextMenu.Show()

                End If

            End Sub

            Private Sub container_MouseMove(sender As System.Object, e As MouseEventArgs)
                CalculateSelection(e)
                InvalidateGraphics()
            End Sub
            Private Sub mouseLeave() Handles waveArea.MouseLeave, spectrogramArea.MouseLeave

                'Activates the mouse down handlers of the wave and spectrogram areas
                AddHandler waveArea.MouseDown, AddressOf Me.container_MouseDown
                If showSpectrogram = True Then AddHandler spectrogramArea.MouseDown, AddressOf Me.container_MouseDown
                If usePhonemeAssignment = True Then
                    'TODO: Here phoneme setting should be cancelled if started:
                    'switch back to normal event handler
                    'switch back the backcolor of the phoneme lables
                End If

            End Sub
            Private Sub container_MouseUp(sender As System.Object, e As MouseEventArgs)

                'This sub removes the handlers that create the selection during mousemove, and initializes setting selection end
                'It also enables a new selection by activating the MouseDown event of wave and spectrogram areas
                If e.Button = MouseButtons.Left Then

                    RemoveHandler waveArea.MouseMove, AddressOf Me.container_MouseMove
                    RemoveHandler waveArea.MouseUp, AddressOf Me.container_MouseUp
                    If showSpectrogram = True Then RemoveHandler spectrogramArea.MouseMove, AddressOf Me.container_MouseMove
                    If showSpectrogram = True Then RemoveHandler spectrogramArea.MouseUp, AddressOf Me.container_MouseUp

                    CalculateSelection(e)

                    InvalidateGraphics()

                    AddHandler waveArea.MouseDown, AddressOf Me.container_MouseDown
                    If showSpectrogram = True Then AddHandler spectrogramArea.MouseDown, AddressOf Me.container_MouseDown

                End If

            End Sub
            Private Sub CalculateSelection(e As MouseEventArgs)

                'This sub calculates start and end of the selected region and stores it in selectionStartSample and selectionLengthSample
                '(A recalculation of selection start position is needed because a selection can be done in both directions on the screen
                Dim highlightEndPixel As Single = e.X

                'Makes sure that selection does not extend outside the window
                If highlightEndPixel >= soundBackgroundArea.Width Then highlightEndPixel = soundBackgroundArea.Width - 1
                If highlightEndPixel < 0 Then highlightEndPixel = 0

                selectionLengthPixel = highlightEndPixel - selectionStartPixel

                'Makes sure than selection is in the right direction
                Dim tempSelectedStartPixel As Single
                Dim tempSelectedLengthPixel As Single

                If selectionLengthPixel >= 0 Then
                    tempSelectedStartPixel = selectionStartPixel
                    tempSelectedLengthPixel = selectionLengthPixel
                Else
                    tempSelectedStartPixel = highlightEndPixel
                    tempSelectedLengthPixel = -selectionLengthPixel
                End If

                selectionStartSample = displayStartSample + tempSelectedStartPixel * SampleToPixelScale
                selectionLengthSample = tempSelectedLengthPixel * SampleToPixelScale

            End Sub
            Private Sub waveScroll(sender As Object, ByVal e As ScrollEventArgs) Handles soundScrollBar.Scroll
                'This sub initializes a soundScrollBarScroll
                soundScrollBarScroll(sender, e.NewValue)
            End Sub
            Private RecalculateDisplayAreaDueTosoundScrollBar_Enabled As Boolean = True
            Public Sub soundScrollBarScroll(sender As Object, ByVal ScrollTo As Integer)

                'This sub performs a scroll of the sound, in rate of update GraphicUpdateFrequency if the sender is the soundScrollBarScroll
                'and instantaneoulsy if the sender is anything else (I.E. the sub may be call externally)
                displayStartSample = ScrollTo

                If TypeOf sender Is HScrollBar Then
                    Dim myScrollBar As HScrollBar = sender
                    If myScrollBar Is soundScrollBar Then
                        'The following code limits the rate of recalculation of the display area to GraphicUpdateFrequency times per second
                        'Compares a time rounded to hundreds of a second, to the updatefrequency converted to update time in hundreds of a second
                        If Math.Round(DateTime.Now.Millisecond / 10) Mod ((1 / GraphicUpdateFrequency) * 100) = 0 Then
                            RecalculateDisplayAreaDueTosoundScrollBar_Enabled = True
                        End If
                        'Recalculates the display area only if an update is due
                        If RecalculateDisplayAreaDueTosoundScrollBar_Enabled = True Then
                            RecalculateDisplayAreaDueTosoundScrollBar_Enabled = False
                            upDatePanel1DisplayData()
                            Exit Sub
                        End If
                    End If
                End If

                'If the sub isn't exited (I.E. the sender is not soundScrollBar, the display area is updated)
                upDatePanel1DisplayData()

            End Sub
            Private Sub soundScrollBarScrollEnd() Handles soundScrollBar.MouseUp

                'This sub makes sure that the sound area is updated when a scroll of the sound is finished (on mouse up)
                '(this is needed because not all scroll events in soundScrollBarScroll triggers a recalculation of the display area
                upDatePanel1DisplayData()
            End Sub

            Private Sub container_MovePhoneme_MouseMove(sender As System.Object, e As MouseEventArgs)

                'This sub sets the position of the currentPhonemeIndex phoneme.
                If SetSegmentationToZeroCrossings Then
                    Dim StartSampl As Integer = displayStartSample + e.X * SampleToPixelScale
                    StartSampl = DSP.GetZeroCrossingSample(sound, 1, StartSampl, DSP.MeasurementsExt.SearchDirections.Closest)
                    sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(currentPhonemeIndex).StartSample = StartSampl
                Else
                    sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(currentPhonemeIndex).StartSample = displayStartSample + e.X * SampleToPixelScale
                End If
                InvalidateGraphics()

            End Sub
            Private Sub container_PositionPhoneme(sender As System.Object, e As MouseEventArgs)

                'This sub positions the currentPhonemeIndex phoneme, and switches on event handlers to normal state
                If SetSegmentationToZeroCrossings Then
                    Dim StartSampl As Integer = displayStartSample + e.X * SampleToPixelScale
                    StartSampl = DSP.GetZeroCrossingSample(sound, 1, StartSampl, DSP.MeasurementsExt.SearchDirections.Closest)
                    sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(currentPhonemeIndex).StartSample = StartSampl
                Else
                    sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(currentPhonemeIndex).StartSample = displayStartSample + e.X * SampleToPixelScale
                End If

                'Resets the colors of the buttons
                For Each item As Control In phonemesContainerPanel.Controls
                    item.BackColor = Color.LightGray
                Next

                RemoveHandler waveArea.MouseDown, AddressOf Me.container_PositionPhoneme
                RemoveHandler waveArea.MouseMove, AddressOf Me.container_MovePhoneme_MouseMove
                If showSpectrogram = True Then RemoveHandler spectrogramArea.MouseDown, AddressOf Me.container_PositionPhoneme
                If showSpectrogram = True Then RemoveHandler spectrogramArea.MouseMove, AddressOf Me.container_MovePhoneme_MouseMove

                InvalidateGraphics()

                AddHandler waveArea.MouseDown, AddressOf Me.container_MouseDown
                If showSpectrogram = True Then AddHandler spectrogramArea.MouseDown, AddressOf Me.container_MouseDown

            End Sub

            '   - Other user input
            Private Sub setSpectrogramLightFactor(sender As Object, ByVal e As ScrollEventArgs) Handles spectrogramLightFactorScrollBar.Scroll

                'This sub calculates a new ligth factor of the spectrogram, by reading the new value from spectrogramLightFactorScroll
                spectrogramLightFactor = (2 ^ ((spectrogramLightFactorScrollBar.Maximum - e.NewValue - 9) / 10)) - 1 'Its wierd why the scroll won't go to maximum value, it only goes to 91 of 100

            End Sub
            Private Sub updateSpectrogramLight() Handles spectrogramLightFactorScrollBar.MouseCaptureChanged 'I'd be better if this was MouseUp, but for some reason it is not working, why?

                'This sub executes recalculation of the spectrogram data after a change of the ligth factor of the spectrogram
                ReCalculateSpectrogramData()
                InvalidateGraphics() 'TODO: Acctually only the spectrogram area would need to be invalidated!

            End Sub
            Private Sub ContextMenuItem_Click(sender As Object, ByVal e As ToolStripItemClickedEventArgs) Handles soundDisplayContextMenu.ItemClicked
                'This sub handles the click on the menu item and directs the code to the appropriate modification/action
                'After doing so the display area is updated once (actually not all actions (like play) need an update of the display area, this could be fixed for performance reasons)

                soundDisplayContextMenu.Hide()
                'upDatePanel1DisplayData()
                InvalidateGraphics()

                Select Case e.ClickedItem.Name
                    Case "play"
                        play()
                    Case "playAll"
                        playAll()
                    Case "stopSound"
                        stopSound()
                    Case "zoomOut"
                        zoomOut()
                    Case "zoomIn"
                        zoomIn()
                    Case "zoomToSelection"
                        zoomToSelection()
                    Case "zoomFull"
                        zoomFull()
                    Case "smoothFadeIn"
                        FadeIn(DSP.FadeSlopeType.Smooth)
                    Case "smoothFadeOut"
                        Fadeout(DSP.FadeSlopeType.Smooth)
                    Case "linearFadeIn"
                        FadeIn(DSP.FadeSlopeType.Linear)
                    Case "linearFadeOut"
                        Fadeout(DSP.FadeSlopeType.Linear)
                    Case "silenceSelection"
                        silenceSelection(False)
                    Case "silenceSelectionZeroCross"
                        silenceSelection(True)
                    Case "copy"
                        graphicCopy()
                    Case "cut"
                        graphicCut()
                    Case "paste"
                        graphicPaste()
                    Case "delete"
                        graphicDelete()
                    Case "crop"
                        graphicCrop()
                    Case "undoAll"
                        graphicUndoAll()
                End Select

                upDatePanel1DisplayData() 'This could be moved to the specific subs to increase performance (not all actions need an update)

            End Sub

            Private WithEvents ReSizeTimer As New Timer
            Private Sub myLayoutContainer_Resized() Handles myLayoutContainer.Resize
                'Resizeing of myLayoutContainer is handled by a timer. On order to avoid recalculation of the display area for
                'every new size. A timer is used to delay resizing GraphicUpdateFrequency times per second.
                'If myLayoutContainer recieves a new resize value before the GraphicUpdateFrequency period, recalculation is delayed again

                ReSizeTimer.Stop()
                ReSizeTimer.Interval = (1 / GraphicUpdateFrequency) * 1000
                ReSizeTimer.Start()
            End Sub
            Private Sub ReSizeTimer_Tick() Handles ReSizeTimer.Tick
                ReSizeTimer.Stop()

                Try

                    'Since the SplitterDistance of myLayoutContainer is changed when myLayoutContainer is resized, it is corrected here
                    If usePhonemeAssignment = True Then
                        myLayoutContainer.SplitterDistance = myLayoutContainer.Height - phonemesContainerPanel.Height
                    Else
                        myLayoutContainer.SplitterDistance = myLayoutContainer.Height
                    End If

                    upDatePanel1DisplayData()

                Catch ex As Exception
                    'Do nothing
                End Try

            End Sub

            '   - variuos button clicks
            Private Sub changeWordButtonClick(sender As Object, ByVal e As MouseEventArgs)

                'This sub handles clicking on the buttons that change to the next word within the same recording

                Select Case sender.name
                    Case changeWordButtonTexts(0).Replace(" ", "") ' Previous word

                        'Check to see if more words exist
                        If currentWord - 1 < 0 Then
                            MsgBox("You're already displaying the phonemes of the first word in the recording.")
                            Exit Sub
                        End If

                        currentWord -= 1

                        'Removing buttons
                        For n = 0 To phonemesContainerPanel.Controls.Count - 1
                            phonemesContainerPanel.Controls.RemoveAt(0)
                        Next

                        'Adding new buttons
                        addButtons()

                        'Updating the sound display
                        InvalidateGraphics()

                    Case changeWordButtonTexts(1).Replace(" ", "") ' Next word

                        'Check to see if more words exist
                        If currentWord + 1 > sound.SMA.ChannelData(currentChannel)(sentence).Count - 1 Then
                            MsgBox("There are no more words to segment in the current recording.")
                            Exit Sub
                        End If

                        currentWord += 1

                        'Removing buttons
                        For n = 0 To phonemesContainerPanel.Controls.Count - 1
                            phonemesContainerPanel.Controls.RemoveAt(0)
                        Next

                        'Adding new buttons
                        addButtons()

                        'Updating the sound display
                        InvalidateGraphics()

                End Select



            End Sub
            Private Sub phonemeLabelButtonClick(sender As Object, ByVal e As MouseEventArgs)

                'This sub handles clicking on the phoneme buttons
                'A left click will play phoneme
                'A right click will start the event handlers that is used to position the phoneme on the sound display

                currentPhonemeIndex = sender.name

                Select Case e.Button
                    Case MouseButtons.Left
                        'Playing the phoneme (but not the word end)

                        If Not currentPhonemeIndex = sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData.Count - 1 Then
                            Dim startSample As Integer = sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(currentPhonemeIndex).StartSample
                            Dim lengthToPlay As Integer = sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(currentPhonemeIndex + 1).StartSample - 1 - startSample 'PlaySoundStream plays to the end if length is 0
                            If lengthToPlay < 0 Then lengthToPlay = 0
                            'PlayBack.Play.PlaySoundStream(sound, currentSentenceData(currentWord).PhoneData(currentPhonemeIndex).StartSample, lengthToPlay)

                            If CurrentSoundPlayer Is Nothing Then CreateNewPaSoundPLayer()
                            PlayBack.PlayDuplexSoundStream(CurrentSoundPlayer, sound, sound.SMA.ChannelData(currentChannel)(sentence)(currentWord).PhoneData(currentPhonemeIndex).StartSample, lengthToPlay,, , 0, 0)

                        End If

                    Case MouseButtons.Right

                        sender.backcolor = Color.Red

                        'Turn of other eventhandlers
                        RemoveHandler waveArea.MouseDown, AddressOf Me.container_MouseDown
                        If showSpectrogram = True Then RemoveHandler spectrogramArea.MouseDown, AddressOf Me.container_MouseDown

                        'Turn on mouse move eventhandler
                        AddHandler waveArea.MouseDown, AddressOf Me.container_PositionPhoneme
                        AddHandler waveArea.MouseMove, AddressOf Me.container_MovePhoneme_MouseMove

                        If showSpectrogram = True Then AddHandler spectrogramArea.MouseDown, AddressOf Me.container_PositionPhoneme
                        If showSpectrogram = True Then AddHandler spectrogramArea.MouseMove, AddressOf Me.container_MovePhoneme_MouseMove

                End Select

            End Sub


            Public Sub CreateNewPaSoundPLayer()

                Dim newAudioSettingsDialog As New AudioSettingsDialog(sound.WaveFormat.SampleRate)
                Dim DialogResult = newAudioSettingsDialog.ShowDialog()
                Dim MyAudioApiSettings As AudioApiSettings = Nothing
                If DialogResult = DialogResult.OK Then
                    MyAudioApiSettings = newAudioSettingsDialog.CurrentAudioApiSettings
                Else
                    MsgBox("Default Setting is being used")
                    MyAudioApiSettings = newAudioSettingsDialog.CurrentAudioApiSettings
                End If

                CurrentSoundPlayer = New PortAudioVB.SoundPlayer(True, sound.WaveFormat, sound, MyAudioApiSettings, , )

            End Sub


            Public Sub play()


                If CurrentSoundPlayer Is Nothing Then CreateNewPaSoundPLayer()

                If selectionLengthSample = 0 Then

                    playAll()

                    'PlayBack.PlayDuplexSoundStream(CurrentSoundPlayer, sound, Nothing, Nothing)

                Else
                    UpdateSampleTimeScale()

                    PlayBack.PlayDuplexSoundStream(CurrentSoundPlayer, sound, selectionStartSample, selectionLengthSample,, , 0, 0)

                End If

            End Sub



            'Responses to menu functions (these can also be called from external code)
            Public Sub playAll()

                UpdateSampleTimeScale()
                If CurrentSoundPlayer Is Nothing Then CreateNewPaSoundPLayer()
                PlayBack.PlayDuplexSoundStream(CurrentSoundPlayer, sound, Nothing, Nothing,, , 0, 0)

            End Sub
            Public Sub stopSound()

                CurrentSoundPlayer.Stop(0.1)

                'My.Computer.Stop()
            End Sub
            Public Sub zoomOut()
                ' making the selection twice the size
                UpdateSampleTimeScale()

                displayLengthSamples = displayLengthSamples * 2
                displayStartSample = displayStartSample - Int(displayLengthSamples / 4)

                'Making sure it doesn't zoom out to much
                If displayStartSample < 0 Then displayStartSample = 0
                If displayStartSample > sound.WaveData.SampleData(currentChannel).Length - 1 Then displayStartSample = sound.WaveData.SampleData(currentChannel).Length - 1
                If displayStartSample + displayLengthSamples > sound.WaveData.SampleData(currentChannel).Length Then displayLengthSamples = sound.WaveData.SampleData(currentChannel).Length - displayStartSample

            End Sub
            Public Sub zoomIn()
                ' making the selection half the size
                UpdateSampleTimeScale()

                'Changing range
                displayLengthSamples = Int(displayLengthSamples / 2)
                displayStartSample = displayStartSample + Int(displayLengthSamples / 2)

                'Making sure length is not shorter that 2 samples
                If displayLengthSamples < 2 Then displayLengthSamples = 2

            End Sub
            Public Sub zoomToSelection()
                UpdateSampleTimeScale()
                displayStartSample = selectionStartSample
                displayLengthSamples = selectionLengthSample

                'Making sure length is not shorter that 1 sample
                If displayLengthSamples < 2 Then displayLengthSamples = 2

            End Sub
            Public Sub zoomFull()
                UpdateSampleTimeScale()
                displayStartSample = 0
                displayLengthSamples = sound.WaveData.SampleData(currentChannel).Length

            End Sub

            ''' <summary>
            ''' Fades in the selected sound using the indicates fade slope type.
            ''' </summary>
            ''' <param name="fadeSlopeType"></param>
            Private Sub FadeIn(ByVal fadeSlopeType As DSP.FadeSlopeType)

                If Not selectionLengthSample < 1 Then
                    DSP.Fade(sound,, 0, currentChannel, selectionStartSample, selectionLengthSample, fadeSlopeType)

                    'Recalculates spectrogram data, since the waveform have been changed
                    If showSpectrogram = True Then UpdateSpectrogramData()

                End If

            End Sub

            ''' <summary>
            ''' Fades out the selected sound using the indicates fade slope type.
            ''' </summary>
            ''' <param name="fadeSlopeType"></param>
            Private Sub Fadeout(ByVal fadeSlopeType As DSP.FadeSlopeType)

                If Not selectionLengthSample < 1 Then
                    DSP.Fade(sound, 0, , currentChannel, selectionStartSample, selectionLengthSample, fadeSlopeType)

                    'Recalculates spectrogram data, since the waveform have been changed
                    If showSpectrogram = True Then UpdateSpectrogramData()

                End If

            End Sub

            ''' <summary>
            ''' Silences the selected section of the sound
            ''' </summary>
            ''' <param name="AdjustToZeroCrossings">If set to true, the silent section starts and end at the closest zero crossings withing the selected section of the wave form.</param>
            Private Sub silenceSelection(ByVal AdjustToZeroCrossings As Boolean)

                Dim SilenceStartSample As Integer = selectionStartSample
                Dim SilenceLength As Integer = selectionLengthSample

                If AdjustToZeroCrossings = True Then

                    Dim SelectionEndSample As Integer = SilenceStartSample + SilenceLength

                    SilenceStartSample = DSP.GetZeroCrossingSample(sound, currentChannel, SilenceStartSample, DSP.MeasurementsExt.SearchDirections.Later)
                    SilenceLength = DSP.GetZeroCrossingSample(sound, currentChannel, SilenceLength, DSP.MeasurementsExt.SearchDirections.Earlier)

                    'Checking that the zero crossing search hasn't caused SilenceStartSample to be equal or higher that SelectionEndSample. If so SilenceLength is set to 0 to stop silencing.
                    If SilenceStartSample >= SelectionEndSample Then SilenceLength = 0

                End If

                'Silencing the section using the fade function, with bith silent start and end
                If Not selectionLengthSample < 1 Then
                    DSP.Fade(sound, , , currentChannel, SilenceStartSample, SilenceLength, DSP.FadeSlopeType.Linear)

                    'Recalculates spectrogram data, since the waveform have been changed
                    If showSpectrogram = True Then UpdateSpectrogramData()

                End If
            End Sub


            Private Sub graphicCopy()

                If Not selectionLengthSample < 1 Then

                    'Copies the selected sound to a new array
                    ReDim selectionCopy(selectionLengthSample - 1)
                    For sample = 0 To selectionCopy.Length - 1
                        selectionCopy(sample) = sound.WaveData.SampleData(currentChannel)(sample + selectionStartSample)
                    Next

                End If

            End Sub
            Private Sub graphicCut()

                If Not selectionLengthSample < 1 Then

                    'Copies the selected sound to a new array
                    ReDim selectionCopy(selectionLengthSample - 1)
                    For sample = 0 To selectionCopy.Length - 1
                        selectionCopy(sample) = sound.WaveData.SampleData(currentChannel)(sample + selectionStartSample)
                    Next

                    'Cutting the selected sound from sound
                    'Getting a copy of sound without the selected samples
                    Dim newArray(sound.WaveData.SampleData(currentChannel).Length - selectionLengthSample - 1) As Single
                    For sample = 0 To selectionStartSample - 1
                        newArray(sample) = sound.WaveData.SampleData(currentChannel)(sample)
                    Next
                    For sample = selectionStartSample To newArray.Length - 1
                        newArray(sample) = sound.WaveData.SampleData(currentChannel)(sample + selectionLengthSample)
                    Next

                    sound.WaveData.SampleData(currentChannel) = newArray

                    'Recalculates spectrogram data, since the waveform have been changed
                    If showSpectrogram = True Then UpdateSpectrogramData()

                    upDatePanel1DisplayData()
                    selectionLengthSample = 0
                    If usePhonemeAssignment = True Then resetCurrentWordLevelSegmentationData()

                End If

            End Sub
            Private Sub graphicPaste()

                If selectionCopy.Length > 0 Then

                    'Copies the selected sound to a new array

                    'Pasting the data in slectionCopy starting at selectionStartSample 
                    'Copying the data prior to selectionStartSample 
                    Dim newArray(sound.WaveData.SampleData(currentChannel).Length + selectionCopy.Length - 1) As Single
                    For sample = 0 To selectionStartSample - 1
                        newArray(sample) = sound.WaveData.SampleData(currentChannel)(sample)
                    Next

                    'Pasting the data from selectionCopy
                    For sample = selectionStartSample To selectionStartSample + selectionCopy.Length - 1
                        newArray(sample) = selectionCopy(sample - selectionStartSample)
                    Next

                    'Copying the data after selectionStartSample 
                    For sample = selectionStartSample + selectionCopy.Length To newArray.Length - 1
                        newArray(sample) = sound.WaveData.SampleData(currentChannel)(sample - selectionCopy.Length)
                    Next

                    sound.WaveData.SampleData(currentChannel) = newArray
                    'sound = newArray

                    'Recalculates spectrogram data, since the waveform have been changed
                    If showSpectrogram = True Then UpdateSpectrogramData()

                    selectionLengthSample = 0
                    If usePhonemeAssignment = True Then resetCurrentWordLevelSegmentationData()

                End If


            End Sub
            Private Sub graphicDelete()

                If Not selectionLengthSample < 1 Then

                    'Deleting the selected sound from sound
                    'Getting a copy of sound without the selected samples
                    Dim newArray(sound.WaveData.SampleData(currentChannel).Length - selectionLengthSample - 1) As Single
                    For sample = 0 To selectionStartSample - 1
                        newArray(sample) = sound.WaveData.SampleData(currentChannel)(sample)
                    Next
                    For sample = selectionStartSample To newArray.Length - 1
                        newArray(sample) = sound.WaveData.SampleData(currentChannel)(sample + selectionLengthSample)
                    Next

                    sound.WaveData.SampleData(currentChannel) = newArray
                    'sound = newArray

                    'Recalculates spectrogram data, since the waveform have been changed
                    If showSpectrogram = True Then UpdateSpectrogramData()

                    selectionLengthSample = 0
                    If usePhonemeAssignment = True Then resetCurrentWordLevelSegmentationData()

                End If

            End Sub
            Private Sub graphicCrop()

                If Not selectionLengthSample < 1 Then

                    'Copies the selected sound to a new array
                    ReDim selectionCopy(selectionLengthSample - 1)
                    For sample = 0 To selectionCopy.Length - 1
                        selectionCopy(sample) = sound.WaveData.SampleData(currentChannel)(sample + selectionStartSample)
                    Next

                    'Replacing the old sound with selectionCopy

                    sound.WaveData.SampleData(currentChannel) = selectionCopy
                    'sound = selectionCopy

                    'Recalculates spectrogram data, since the waveform have been changed
                    If showSpectrogram = True Then UpdateSpectrogramData()

                    selectionLengthSample = 0
                    If usePhonemeAssignment = True Then resetCurrentWordLevelSegmentationData()

                End If

            End Sub
            Private Sub graphicUndoAll()
                retriveSoundBackUp()
                'If usePhonemeAssignment = True Then resetCurrentWordLevelSegmentationData() ' I don't think this is needed here. Since the original word is retrieved, it's probably ok to also keep the SMA data in it as it is.
                selectionLengthSample = 0
                zoomFull()
            End Sub

            'Other available public functions, not primarily for internal use
            Public Sub zoomTo(ByVal startSample As Integer, ByVal length As Integer)

                'displaying the waveform for the specified sample interval
                UpdateSampleTimeScale()

                'Making sure length is not shorter that 2 samples
                If length < 2 Then length = 2

                'Making sure it doesn't zoom out to much
                If startSample < 0 Then startSample = 0
                If startSample > sound.WaveData.SampleData(currentChannel).Length - 1 Then startSample = sound.WaveData.SampleData(currentChannel).Length - 1
                If startSample + length > sound.WaveData.SampleData(currentChannel).Length Then length = sound.WaveData.SampleData(currentChannel).Length - startSample

                displayLengthSamples = startSample
                displayStartSample = length

                'Updating the value of the sound scroll bar
                soundScrollBar.Value = displayStartSample

                upDatePanel1DisplayData()
            End Sub

            'Performing automatic speech boundary detection
            Private Sub detectBoundariesButtonClick()

                sound.SMA.DetectSpeechBoundaries(sound, LongestSilentSegment, SilenceDefinition, TemporalIntegrationDuration,
                                              DetailedTemporalIntegrationDuration, DetailedSilenceCriteria)

                upDatePanel1DisplayData()

            End Sub


            'Storing data
            Private Sub UpdateSegmentationButtonClick(sender As Object, ByVal e As MouseEventArgs)
                UpdateSegmentation()
            End Sub

            Private Sub FadePaddingButtonClick(sender As Object, ByVal e As MouseEventArgs)
                FadePadding()
                upDatePanel1DisplayData()
            End Sub


            ''' <summary>
            ''' Checks that the order of phonemes is correct and returns False if the order is wrong.
            ''' If the order is correct phoneme lengths are calculated and stored in currentSentenceData.
            ''' </summary>
            ''' <returns></returns>
            Public Function CheckPhonemeOrderAndCalculatePhonemeLengths(Optional ByVal StoreWordSegmentationFromPhonemes As Boolean = False)

                'Checking the order of phonemes

                For c As Integer = 1 To sound.SMA.ChannelCount
                    For sentence As Integer = 0 To sound.SMA.ChannelData(c).Count - 1


                        For word = 0 To sound.SMA.ChannelData(c)(sentence).Count - 1
                            For testedPhonemeIndex = 0 To sound.SMA.ChannelData(c)(sentence)(word).PhoneData.Count - 1

                                'Updates the word bounaries
                                If StoreWordSegmentationFromPhonemes = True Then

                                    If testedPhonemeIndex = 0 Then
                                        sound.SMA.ChannelData(c)(sentence)(word).StartSample = sound.SMA.ChannelData(c)(sentence)(word).PhoneData(0).StartSample
                                    End If

                                    If testedPhonemeIndex = sound.SMA.ChannelData(c)(sentence)(word).PhoneData.Count - 1 Then
                                        sound.SMA.ChannelData(c)(sentence)(word).Length =
                                    sound.SMA.ChannelData(c)(sentence)(word).PhoneData(sound.SMA.ChannelData(c)(sentence)(word).PhoneData.Count - 1).StartSample -
                                   sound.SMA.ChannelData(c)(sentence)(word).PhoneData(0).StartSample + 1
                                    End If
                                End If

                                'Compared to the previous phoneme
                                If Not testedPhonemeIndex = 0 Then
                                    If sound.SMA.ChannelData(c)(sentence)(word).PhoneData(testedPhonemeIndex).StartSample < sound.SMA.ChannelData(c)(sentence)(word).PhoneData(testedPhonemeIndex - 1).StartSample Then
                                        'currentSentenceData(currentWord).PhoneData(testedPhonemeIndex).StartSample = -1
                                        MsgBox("Wrong order of phonemes in the word " & sound.SMA.ChannelData(c)(sentence)(word).OrthographicForm & ": " & vbCr & vbCr &
                                       "The phoneme [" & sound.SMA.ChannelData(c)(sentence)(word).PhoneData(testedPhonemeIndex).PhoneticForm & "] must be placed after [" & sound.SMA.ChannelData(c)(sentence)(word).PhoneData(testedPhonemeIndex - 1).PhoneticForm & "]!",, "Place phonemes in currect order!")
                                        Return False
                                    End If
                                End If

                                'Compared to the following phoneme
                                If Not testedPhonemeIndex = sound.SMA.ChannelData(c)(sentence)(word).PhoneData.Count - 1 Then
                                    If Not sound.SMA.ChannelData(c)(sentence)(word).PhoneData(testedPhonemeIndex + 1).StartSample = -1 Then
                                        If sound.SMA.ChannelData(c)(sentence)(word).PhoneData(testedPhonemeIndex).StartSample > sound.SMA.ChannelData(c)(sentence)(word).PhoneData(testedPhonemeIndex + 1).StartSample Then
                                            'currentSentenceData(currentWord).PhoneData(testedPhonemeIndex).StartSample = -1
                                            MsgBox("Wrong order of phonemes in the word " & sound.SMA.ChannelData(c)(sentence)(word).OrthographicForm & ": " & vbCr & vbCr &
                                           "The phoneme [" & sound.SMA.ChannelData(c)(sentence)(word).PhoneData(testedPhonemeIndex + 1).PhoneticForm & "] must be placed after [" & sound.SMA.ChannelData(c)(sentence)(word).PhoneData(testedPhonemeIndex).PhoneticForm & "]!",, "Place phonemes in currect order!")
                                            Return False
                                        End If
                                    End If
                                End If
                            Next


                            'Calculating phoneme lengths
                            For phoneme = 0 To sound.SMA.ChannelData(c)(sentence)(word).PhoneData.Count - 2
                                Dim phonemeLength As Integer = sound.SMA.ChannelData(c)(sentence)(word).PhoneData(phoneme + 1).StartSample - sound.SMA.ChannelData(c)(sentence)(word).PhoneData(phoneme).StartSample
                                If phonemeLength < 0 Then phonemeLength = 0
                                sound.SMA.ChannelData(c)(sentence)(word).PhoneData(phoneme).Length = phonemeLength
                            Next
                        Next
                    Next
                Next


                Return True

                'To check values only
                'Dim table As New DataTable
                'table.Columns.Add("phoneme", GetType(String))
                'table.Columns.Add("startSample", GetType(String))
                'table.Columns.Add("length", GetType(String))

                'For row = 0 To currentChannelData(currentWord).PhonemeLevelDataList.Count - 1
                'table.Rows.Add(currentChannelData(currentWord).PhonemeLevelDataList(row).phoneme, currentChannelData(currentWord).PhonemeLevelDataList(row).startSample, currentChannelData(currentWord).PhonemeLevelDataList(row).length)
                'Next

                'displayTable.DataGridView1.DataSource = table

            End Function

            Dim InitialSegmentationIsDone As Boolean = False
            Public Sub UpdateSegmentation(Optional ByVal DoZoomFull As Boolean = True)

                Try

                    If currentChannel <> 1 Then Throw New NotImplementedException("At present, only mono sounds are supported by UpdateSegmentation")

                    If Not CheckPhonemeOrderAndCalculatePhonemeLengths() = True Then Exit Sub

                    sound.SMA.UpdateSegmentation(sound, PaddingTime, currentChannel)

                    'Updating spectrogram data
                    If showSpectrogram = True Then
                        'Updating fft data
                        sound.FFT = New FftData(sound.WaveFormat, spectrogramFormat.SpectrogramFftFormat)
                        sound.FFT.CalculateSpectrogramData(sound, spectrogramFormat, currentChannel)
                    End If

                    InitialSegmentationIsDone = True

                    If DoZoomFull = True Then zoomFull()
                    selectionLengthSample = 0
                    upDatePanel1DisplayData()

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            End Sub

            ''' <summary>
            ''' Fades the padded sections
            ''' </summary>
            Private Sub FadePadding()

                If InitialSegmentationIsDone = True And sound.SMA.ChannelData(currentChannel)(sentence).StartSample <> -1 And sound.SMA.ChannelData(currentChannel)(sentence).Length <> 0 Then
                    sound.SMA.FadePaddingSection(sound, currentChannel)
                Else
                    MsgBox("Unable to fade padding section due to incomplete boundary segmentation.")
                End If

            End Sub

            Private Class spectrogramDisplayData

                Public Property area As RectangleF
                Public Property brushColor As Brush

                Public Sub New(setBrushColor As Brush, X As Single, Y As Single, width As Single, height As Single)

                    If X = Nothing Then X = 0
                    If Y = Nothing Then Y = 0
                    If width = Nothing Then width = 0
                    If height = Nothing Then height = 0

                    area = New RectangleF(X, Y, width, height)
                    brushColor = setBrushColor

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




    End Namespace

End Namespace