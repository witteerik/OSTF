Public Class Form4

    Dim SoundPlayer As SpeechTestFramework.Audio.PortAudioVB.OverlappingSoundPlayer

    'Att göra:
    'And calculate Lcp backwards from spectrum levels (Or just store the CPL in every SMA)
    '
    'Create MediaSet GUI

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'ResponseGuiItemTable1.AdjustControls()


        Dim test As Integer = 0
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

    Private Sub SiP_Button_Click(sender As Object, e As EventArgs) Handles SiP_Button.Click

        Dim SipTest = New SpeechTestFramework.SipTestGui
        SipTest.Show()

    End Sub

    Private Sub Audiogram1_MouseClick(sender As Object, e As MouseEventArgs)

    End Sub

    Private Sub Audiogram1_MouseHover(sender As Object, e As EventArgs) Handles Audiogram1.MouseHover

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

        'Just high-jacking this app to coy files for the SSL-project
        Dim Folders = IO.Directory.EnumerateDirectories("C:\EriksDokument\SslVideosInSubfolders")
        Dim OutputParentDir As String = "C:\VLDT\PilotExperiment1"

        Dim f As Integer = 0

        For b = 1 To 4

            Dim BlockFolder = IO.Path.Combine(OutputParentDir, "Block" & b.ToString("00"))

            For i = 0 To 49

                Dim ReadFolder = IO.Path.Combine(Folders(f))
                Dim ReadFile = IO.Directory.GetFiles(ReadFolder)(0)
                Dim WriteFile = IO.Path.Combine(BlockFolder, "Real", IO.Path.GetFileName(ReadFile))
                IO.File.Copy(ReadFile, WriteFile)
                f += 1

            Next

            For i = 0 To 49

                Dim ReadFolder = IO.Path.Combine(Folders(f))
                Dim ReadFile = IO.Directory.GetFiles(ReadFolder)(0)
                Dim WriteFile = IO.Path.Combine(BlockFolder, "Pseudo", IO.Path.GetFileName(ReadFile))
                IO.File.Copy(ReadFile, WriteFile)
                f += 1

                f += 1
            Next
        Next

        MsgBox("Finished copying files")

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click

        Dim X As Double() = {0.41, 0.42, 0.5, 0.44, 0.39, 0.4, 0.6, 0.8, 0.9, 0.99}
        Dim Floor As Double() = {1 / 3}

        Dim Y = SpeechTestFramework.CriticalDifferences.AdjustSuccessProbabilities(X, 0.9999999999, Floor)

        Dim Z = X.Average
        Dim z2 = Y.Average
        Dim s = 1

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click

        SpeechTestFramework.Audio.AudioIOs.RemoveWaveChunksBatch("C:\EriksDokument\SSHR\STA_rip1")

        MsgBox(SpeechTestFramework.Utils.CompareBatchOfFiles("C:\EriksDokument\SSHR\STA_rip1", "C:\EriksDokument\SSHR\STA_rip1 - kopia", SpeechTestFramework.Utils.GeneralIO.FileComparisonMethods.CompareWaveFileData))

        'MsgBox(SpeechTestFramework.Utils.CompareBatchOfFiles("C:\EriksDokument\SSHR\STA_rip1", "C:\EriksDokument\SSHR\STA_rip2", SpeechTestFramework.Utils.GeneralIO.FileComparisonMethods.CompareWaveFileData))

    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click

        'This function was used to reassemble the parts (1 and 2) of the Hagerman lists from the CD 'Svensk talaudiometri"

        Dim InputFolder As String = "C:\EriksDokument\SSHR\Hagerman(fromSTA)"
        Dim OutputFolder As String = "C:\EriksDokument\SSHR\HagermanWholeLists"

        Dim Track As Integer = 22

        For List As Integer = 2 To 11

            Dim File1 As String
            Dim File2 As String
            If List = 8 Then
                File1 = "PractiseList_part1(20).wav"
                File2 = "PractiseList_part2(21).wav"
            Else
                File1 = "List" & List.ToString("00") & "_part1(" & Track & ").wav"
                Track += 1
                File2 = "List" & List.ToString("00") & "_part2(" & Track & ").wav"
                Track += 1
            End If

            Dim Sound1 = SpeechTestFramework.Audio.Sound.LoadWaveFile(IO.Path.Combine(InputFolder, File1))
            Dim Sound2 = SpeechTestFramework.Audio.Sound.LoadWaveFile(IO.Path.Combine(InputFolder, File2))

            Dim Sound3 = SpeechTestFramework.Audio.DSP.ConcatenateSounds(New List(Of SpeechTestFramework.Audio.Sound) From {Sound1, Sound2})

            'Removes the SMA object
            Sound3.SMA = Nothing

            MsgBox("List " & List & vbCrLf &
                   "Channel 1 (juxtaposed samples): " & Sound1.WaveData.SampleData(1)(Sound1.WaveData.SampleData(1).Length - 1) & " - " & Sound2.WaveData.SampleData(1)(0) & vbCrLf &
                   "Channel 2 (juxtaposed samples): " & Sound1.WaveData.SampleData(2)(Sound1.WaveData.SampleData(2).Length - 1) & " - " & Sound2.WaveData.SampleData(2)(0) & vbCrLf &
                   "Channel 1, Post-Pre-length difference: " & Sound3.WaveData.SampleData(1).Length - (Sound1.WaveData.SampleData(1).Length + Sound2.WaveData.SampleData(1).Length) & vbCrLf &
                   "Channel 2, Post-Pre-length difference:: " & Sound3.WaveData.SampleData(2).Length - (Sound1.WaveData.SampleData(2).Length + Sound2.WaveData.SampleData(2).Length))


            If List = 8 Then
                Sound3.WriteWaveFile(IO.Path.Combine(OutputFolder, "PractiseList.wav"))
            Else
                Sound3.WriteWaveFile(IO.Path.Combine(OutputFolder, "List" & List.ToString("00") & ".wav"))
            End If


        Next



    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click

        Dim InputFolder As String = "C:\EriksDokument\SSHR\HagermanWholeLists"
        Dim OutputFolder As String = "C:\EriksDokument\SSHR\ChannelSplit"

        Dim LongestNoise As SpeechTestFramework.Audio.Sound = Nothing

        Dim Files = IO.Directory.GetFiles(InputFolder)
        For Each file In Files
            If IO.Path.GetExtension(file) <> ".wav" Then Continue For

            Dim FileName = IO.Path.GetFileName(file)

            If FileName.StartsWith("CalibrationTrack") Then Continue For

            Dim InputFile = SpeechTestFramework.Audio.Sound.LoadWaveFile(file)

            Dim SpeechFile = New SpeechTestFramework.Audio.Sound(New SpeechTestFramework.Audio.Formats.WaveFormat(InputFile.WaveFormat.SampleRate, InputFile.WaveFormat.BitDepth, 1,, InputFile.WaveFormat.Encoding))

            SpeechFile.WaveData.SampleData(1) = InputFile.WaveData.SampleData(1)

            'Exporting the speech file
            SpeechFile.SMA = Nothing
            SpeechFile.WriteWaveFile(IO.Path.Combine(OutputFolder, FileName))

            If LongestNoise Is Nothing Then LongestNoise = New SpeechTestFramework.Audio.Sound(SpeechFile.WaveFormat)

            If InputFile.WaveData.SampleData(2).Length > LongestNoise.WaveData.SampleData(1).Length Then
                LongestNoise = New SpeechTestFramework.Audio.Sound(SpeechFile.WaveFormat)
                LongestNoise.WaveData.SampleData(1) = InputFile.WaveData.SampleData(2)
                LongestNoise.FileName = InputFile.FileName
            End If

        Next

        'Exportin the longest noise
        LongestNoise.SMA = Nothing
        LongestNoise.WriteWaveFile(IO.Path.Combine(OutputFolder, "Noise_" & LongestNoise.FileName))


    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Dim InputFolder As String = "C:\EriksDokument\SSHR\HagermanWholeLists"
        Dim OutputFolder As String = "C:\EriksDokument\SSHR\ChannelSplit"

        Dim LongestNoise As SpeechTestFramework.Audio.Sound = Nothing

        Dim Files = IO.Directory.GetFiles(InputFolder)
        For Each file In Files
            If IO.Path.GetExtension(file) <> ".wav" Then Continue For

            Dim FileName = IO.Path.GetFileName(file)

            If Not FileName.StartsWith("CalibrationTrack") Then Continue For

            Dim InputFile = SpeechTestFramework.Audio.Sound.LoadWaveFile(file)

            Dim Channel1File = New SpeechTestFramework.Audio.Sound(New SpeechTestFramework.Audio.Formats.WaveFormat(InputFile.WaveFormat.SampleRate, InputFile.WaveFormat.BitDepth, 1,, InputFile.WaveFormat.Encoding))

            Channel1File.WaveData.SampleData(1) = InputFile.WaveData.SampleData(2)

            'Exporting the speech file
            Channel1File.SMA = Nothing
            Channel1File.WriteWaveFile(IO.Path.Combine(OutputFolder, FileName))

        Next


    End Sub


    Private Sub Button10_Click2(sender As Object, e As EventArgs) Handles Button10.Click

        Dim TestSound = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\OSTF\RoomImpulses\wierstorf2011\48000Hz\QU_KEMAR_anechoic_1.0m.wav")


        'MsgBox(SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.UnwrapAngle(-30))
        'MsgBox(SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.UnwrapAngle(30))
        'MsgBox(SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.UnwrapAngle(400))

        Dim Sound_Background = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\SwedishSiPTest\SoundFiles\Stad\CBG_Stad.wav")
        Dim Sound_TestWord = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\SwedishSiPTest\SoundFiles\Tyst\TestWordSounds\F_001_000_sätt.wav")
        Dim Sound_Masker1 = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\SwedishSiPTest\SoundFiles\Stad\TWRB\satt_sätt_sött_1\Masker_01.wav")
        Dim Sound_Masker2 = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\SwedishSiPTest\SoundFiles\Stad\TWRB\satt_sätt_sött_1\Masker_02.wav")

        Dim TargetLength As Integer = 10 * 48000

        Dim Randomizer = New Random

        Dim Sound_Background1 = Sound_Background.CopySection(1, Randomizer.Next(0, Sound_Background.WaveData.SampleData(1).Length - TargetLength - 2), TargetLength)
        Dim Sound_Background2 = Sound_Background.CopySection(1, Randomizer.Next(0, Sound_Background.WaveData.SampleData(1).Length - TargetLength - 2), TargetLength)
        Dim Sound_Background3 = Sound_Background.CopySection(1, Randomizer.Next(0, Sound_Background.WaveData.SampleData(1).Length - TargetLength - 2), TargetLength)
        Dim Sound_Background4 = Sound_Background.CopySection(1, Randomizer.Next(0, Sound_Background.WaveData.SampleData(1).Length - TargetLength - 2), TargetLength)

        Dim ItemList = New List(Of SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSceneItem)

        Dim FadeSpecs_Background = New List(Of SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications)
        FadeSpecs_Background.Add(New SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, 10000))
        FadeSpecs_Background.Add(New SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -10000))

        Dim FadeSpecs_Speech = New List(Of SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications)
        FadeSpecs_Speech.Add(New SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, 100))
        FadeSpecs_Speech.Add(New SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -100))

        Dim FadeSpecs_Maskers = New List(Of SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications)
        FadeSpecs_Maskers.Add(New SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, 48000))
        FadeSpecs_Maskers.Add(New SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -48000))

        Dim DuckSpecs = New List(Of SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications)
        DuckSpecs.Add(New SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications(0, -3, 48000, 48000))
        DuckSpecs.Add(New SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications(-3, 0, 3 * 48000, 48000))

        ItemList.Add(New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSceneItem(Sound_Background1, 1, 60, 1, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = -45}, 0,,,, FadeSpecs_Background, DuckSpecs))
        ItemList.Add(New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSceneItem(Sound_Background2, 1, 60, 1, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 45}, 0,,,, FadeSpecs_Background, DuckSpecs))
        ItemList.Add(New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSceneItem(Sound_Background3, 1, 60, 1, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = -135}, 0,,,, FadeSpecs_Background, DuckSpecs))
        ItemList.Add(New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSceneItem(Sound_Background4, 1, 60, 1, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 135}, 0,,,, FadeSpecs_Background, DuckSpecs))
        ItemList.Add(New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSceneItem(Sound_TestWord, 1, 70, 2, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 15}, 48000 * 2,,,, FadeSpecs_Speech))
        ItemList.Add(New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSceneItem(Sound_Masker1, 1, 65, 3, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 120}, 48000,,,, FadeSpecs_Maskers))
        ItemList.Add(New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSceneItem(Sound_Masker2, 1, 65, 3, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 130}, 48000,,,, FadeSpecs_Maskers))

        Dim MyMixer = New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer(2, 0)
        MyMixer.SetLinearOutput()
        MyMixer.HardwareOutputChannelSpeakerLocations.Add(1, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = -30})
        MyMixer.HardwareOutputChannelSpeakerLocations.Add(2, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 0})
        MyMixer.HardwareOutputChannelSpeakerLocations.Add(3, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 30})

        MyMixer.TransducerType = SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.TransducerTypes.SimulatedSoundField
        MyMixer.SetupDirectionalSimulator(SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.TransducerNames.Unspecified, 1, Sound_Background.WaveFormat)

        Dim OutputSound = MyMixer.CreateSoundScene(ItemList)

        OutputSound.WriteWaveFile("C:\Temp\OutputSound_S7.wav")

        Dim x = 1

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click

        Dim MyMixer = New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer(2, 0, SpeechTestFramework.OstfSettings.AvaliableTransducers(0))
        MyMixer.SetLinearOutput()
        MyMixer.HardwareOutputChannelSpeakerLocations.Add(1, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = -30})
        MyMixer.HardwareOutputChannelSpeakerLocations.Add(2, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 0})
        MyMixer.HardwareOutputChannelSpeakerLocations.Add(3, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 30})


        Dim CalibrationDialog As New SpeechTestFramework.CalibrationForm(MyMixer)
        CalibrationDialog.Show()

    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click

        SpeechTestFramework.Audio.AudioIOs.SamplerateConversion_DirectBatch("C:\OSTF\RoomImpulses\wierstorf2011\44100Hz", "C:\OSTF\RoomImpulses\wierstorf2011\48000Hz", New SpeechTestFramework.Audio.Formats.WaveFormat(44100, 32, 720, , SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints))

        MsgBox("Finished")
    End Sub
End Class