Public Class Form4

    Dim SoundPlayer As SpeechTestFramework.Audio.PortAudioVB.OverlappingSoundPlayer

    'Att göra:
    'And calculate Lcp backwards from spectrum levels (Or just store the CPL in every SMA)
    '
    'Create MediaSet GUI

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'ResponseGuiItemTable1.AdjustControls()


        Dim test As Integer = -1
        Select Case test
            Case -1

                SpeechTestFramework.OstfSettings.LoadAvailableTestSpecifications()
                Dim SelectedTestIndex As Integer = 1
                SpeechTestFramework.OstfSettings.AvailableTests(SelectedTestIndex).LoadSpeechMaterialComponentsFile()
                Dim MySpeechMaterial = SpeechTestFramework.OstfSettings.AvailableTests(SelectedTestIndex).SpeechMaterial
                SpeechTestFramework.OstfSettings.AvailableTests(SelectedTestIndex).LoadAvailableMediaSetSpecifications()
                Dim MyMediaSet = SpeechTestFramework.OstfSettings.AvailableTests(SelectedTestIndex).MediaSets(0)

                Dim TestComponent = MySpeechMaterial.GetAllDescenentsAtLevel(SpeechTestFramework.SpeechMaterialComponent.LinguisticLevels.List)(0)

                'Dim TestSmaList = TestComponent.GetCorrespondingSmaComponent(MyMediaSet, 0, 1)

                'Dim TestSound = TestSmaList(0).GetSoundFileSection(1)

                ' TestSound.WriteWaveFile("C:\SpeechTestFrameworkLog\Temp2\Test4.wav")

                Dim ComponentSound = TestComponent.GetSound(MyMediaSet, 0, 1, 60000)

                ComponentSound.WriteWaveFile("C:\SpeechTestFrameworkLog\Temp2\Test6.wav")

            Case 0

                Dim SM_Creator As New SpeechTestFramework.SpeechMaterialCreator

                SM_Creator.Show()

            Case 1

                SpeechTestFramework.OstfSettings.LoadAvailableTestSpecifications()

                Dim SelectedTestIndex As Integer = 0

                SpeechTestFramework.OstfSettings.AvailableTests(SelectedTestIndex).LoadSpeechMaterialComponentsFile()

                Dim CompleteSpeechMaterial = SpeechTestFramework.OstfSettings.AvailableTests(SelectedTestIndex).SpeechMaterial

                'Dim CompleteSpeechMaterial = SpeechTestFramework.SpeechMaterialComponent.LoadSpeechMaterial(SpeechTestFramework.OstfSettings.CurrentlySelectedTest.SpeechMaterialComponentsSubFilePath)

                'CompleteSpeechMaterial.WriteSpeechMaterialComponenFile(SpeechTestFramework.Utils.logFilePath & "TestSMC.txt")

                Dim NewMediaSet = New SpeechTestFramework.MediaSet
                'NewMediaSet.SetSipValues(1)
                'NewMediaSet.SetHintDebugValues()

                'NewMediaSet.CopySoundFiles(CompleteSpeechMaterial, IO.Path.Combine(SpeechTestFramework.Utils.logFilePath, "MediaSet2"))

                NewMediaSet.RecordAndEditAudioMediaFiles(SpeechTestFramework.MediaSet.SpeechMaterialRecorderLoadOptions.LoadAllSounds, SpeechTestFramework.MediaSet.PrototypeRecordingOptions.None)

                'NewMediaSet.CreateLackingAudioMediaFiles(CompleteSpeechMaterial)


            Case 2


                'Dim s = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\OSTA\Tests\SwedishSiPTest\SoundFiles\Unechoic-Talker1-RVE\TestWordRecordings\farm\M_000_001_farm.wav")

                Dim s = SpeechTestFramework.Audio.Sound.LoadWaveFile(IO.Path.Combine(SpeechTestFramework.Utils.logFilePath, "Test1.wav"))

                s.WriteWaveFile(IO.Path.Combine(SpeechTestFramework.Utils.logFilePath, "Test2.wav"))

                Dim X = 1

                Dim Sound1 = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\SpeechTestFrameworkLog\M_000_003_charm.wav")
                Dim Sound2 = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\SpeechTestFrameworkLog\M_000_003_kil.wav")

                SpeechTestFramework.Audio.DSP.AddSoundToEnd(Sound1, Sound2)

                Dim WaveViewer = New SpeechTestFramework.Audio.Graphics.SoundEditor(Sound1)
                WaveViewer.Dock = DockStyle.Fill
                Me.Controls.Add(WaveViewer)


            Case 3


                'Testing sound player
                Dim Sound1 = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\SpeechTestFrameworkLog\M_000_003_charm.wav")

                Dim RecordingWaveFormat As SpeechTestFramework.Audio.Formats.WaveFormat = Sound1.WaveFormat
                Dim newAudioSettingsDialog As SpeechTestFramework.AudioSettingsDialog
                If RecordingWaveFormat IsNot Nothing Then
                    newAudioSettingsDialog = New SpeechTestFramework.AudioSettingsDialog(RecordingWaveFormat.SampleRate)
                Else
                    newAudioSettingsDialog = New SpeechTestFramework.AudioSettingsDialog()
                    RecordingWaveFormat = New SpeechTestFramework.Audio.Formats.WaveFormat(newAudioSettingsDialog.CurrentAudioApiSettings.SampleRate, 32, 1, , SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints)
                End If

                Dim Result = newAudioSettingsDialog.ShowDialog()
                Dim CurrentAudioApiSettings As New SpeechTestFramework.Audio.AudioApiSettings(RecordingWaveFormat.SampleRate)
                If Result = Windows.Forms.DialogResult.OK Then
                    CurrentAudioApiSettings = newAudioSettingsDialog.CurrentAudioApiSettings
                Else
                    'Attempting to set default AudioApiSettings if the user pressed ok
                    CurrentAudioApiSettings.SelectDefaultAudioDevice(RecordingWaveFormat.SampleRate)
                End If

                SoundPlayer = New SpeechTestFramework.Audio.PortAudioVB.OverlappingSoundPlayer(Nothing, SpeechTestFramework.Audio.PortAudioVB.OverlappingSoundPlayer.SoundDirections.Duplex, CurrentAudioApiSettings,,,,,, 0.1,, False)

                'SoundPlayer.Mixer = NewMixer
                SoundPlayer.OpenStream()
                SoundPlayer.SwapOutputSounds(Sound1)


                'SoundPlayer.SwapOutputSounds(Sound1)

                'SoundPlayer.Stop(True)

                'Sleeps during the fade out phase
                'Threading.Thread.Sleep(SoundPlayer.GetOverlapDuration * 1000)

                'SoundPlayer.CloseStream()
                'SoundPlayer.Dispose()

            Case 10
                'Creating audiogram
                Dim MyAudiogram As New SpeechTestFramework.WinFormControls.Audiogram
                MyAudiogram.AudiogramData = New SpeechTestFramework.AudiogramData

                'Adding som custom data
                Dim fs() As Single = {125, 250, 375, 500, 750, 1000, 1500, 2000, 3000, 4000, 6000, 8000}
                Dim NH() As Single = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                Dim N3() As Single = {35, 35, 35, 35, 35, 40, 45, 50, 55, 60, 65, 65}
                Dim N5() As Single = {65, 65, 67.5, 70, 72.5, 75, 80, 80, 80, 80, 80, 80}
                Dim S3() As Single = {30, 30, 30, 35, 47.5, 60, 70, 75, 80, 80, 85, 85}

                Dim lw As Single = 3
                Dim ps As Single = 3

                Dim MyColors As New List(Of Drawing.Color)
                MyColors.Add(Drawing.Color.FromArgb(255, 98, 140, 190))
                MyColors.Add(Drawing.Color.FromArgb(255, 131, 117, 78))
                MyColors.Add(Drawing.Color.FromArgb(255, 208, 51, 44))
                MyColors.Add(Drawing.Color.FromArgb(255, 0, 0, 0))

                Dim Line1 As New SpeechTestFramework.WinFormControls.PlotBase.Line With {.XValues = fs, .YValues = NH, .Color = MyColors(0), .LineWidth = lw}
                Dim Line2 As New SpeechTestFramework.WinFormControls.PlotBase.Line With {.XValues = fs, .YValues = N3, .Color = MyColors(1), .LineWidth = lw}
                Dim Line3 As New SpeechTestFramework.WinFormControls.PlotBase.Line With {.XValues = fs, .YValues = N5, .Color = MyColors(2), .LineWidth = lw}
                Dim Line4 As New SpeechTestFramework.WinFormControls.PlotBase.Line With {.XValues = fs, .YValues = S3, .Color = MyColors(3), .LineWidth = lw}
                MyAudiogram.Lines.Add(Line4)
                MyAudiogram.Lines.Add(Line1)
                MyAudiogram.Lines.Add(Line2)
                MyAudiogram.Lines.Add(Line3)

                Dim Point1 As New SpeechTestFramework.WinFormControls.PlotBase.PointSerie With {.XValues = fs, .YValues = NH, .Color = MyColors(0), .PointSize = ps, .Type = SpeechTestFramework.WinFormControls.PlotBase.PointSerie.PointTypes.FilledCircle}
                Dim Point2 As New SpeechTestFramework.WinFormControls.PlotBase.PointSerie With {.XValues = fs, .YValues = N3, .Color = MyColors(1), .PointSize = ps, .Type = SpeechTestFramework.WinFormControls.PlotBase.PointSerie.PointTypes.FilledCircle}
                Dim Point3 As New SpeechTestFramework.WinFormControls.PlotBase.PointSerie With {.XValues = fs, .YValues = N5, .Color = MyColors(2), .PointSize = ps, .Type = SpeechTestFramework.WinFormControls.PlotBase.PointSerie.PointTypes.FilledCircle}
                Dim Point4 As New SpeechTestFramework.WinFormControls.PlotBase.PointSerie With {.XValues = fs, .YValues = S3, .Color = MyColors(3), .PointSize = ps, .Type = SpeechTestFramework.WinFormControls.PlotBase.PointSerie.PointTypes.FilledCircle}
                MyAudiogram.PointSeries.Add(Point4)
                MyAudiogram.PointSeries.Add(Point1)
                MyAudiogram.PointSeries.Add(Point2)
                MyAudiogram.PointSeries.Add(Point3)

                MyAudiogram.Update()
                MyAudiogram.Dock = DockStyle.Fill

                Dim NewForm = New Windows.Forms.Form
                NewForm.Controls.Add(MyAudiogram)
                NewForm.Show()


        End Select






    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        SoundPlayer.Start(True)

    End Sub


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click


        Dim x = SoundPlayer.GetRecordedSound


        SoundPlayer.SwapOutputSounds(x, False)



        Dim c = 1

    End Sub

    Private Sub LoadFileControl1_LoadFile(FileToLoad As String)

        MsgBox("Now loading file:" & FileToLoad)

    End Sub
End Class