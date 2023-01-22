Public Class Form4

    Dim SoundPlayer As SpeechTestFramework.Audio.PortAudioVB.OverlappingSoundPlayer

    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Initializing the OSTF 
        SpeechTestFramework.InitializeOSTF()

        Me.Audiogram2.AudiogramData = New SpeechTestFramework.AudiogramData

    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'ResponseGuiItemTable1.AdjustControls()


        Dim test As Integer = 0
        Select Case test
            Case -1

                SpeechTestFramework.OstfBase.LoadAvailableTestSpecifications()
                Dim SelectedTestIndex As Integer = 1
                SpeechTestFramework.OstfBase.AvailableTests(SelectedTestIndex).LoadSpeechMaterialComponentsFile()
                Dim MySpeechMaterial = SpeechTestFramework.OstfBase.AvailableTests(SelectedTestIndex).SpeechMaterial
                SpeechTestFramework.OstfBase.AvailableTests(SelectedTestIndex).LoadAvailableMediaSetSpecifications()
                Dim MyMediaSet = SpeechTestFramework.OstfBase.AvailableTests(SelectedTestIndex).MediaSets(0)

                Dim TestComponent = MySpeechMaterial.GetAllDescenentsAtLevel(SpeechTestFramework.SpeechMaterialComponent.LinguisticLevels.List)(0)

                'Dim TestSmaList = TestComponent.GetCorrespondingSmaComponent(MyMediaSet, 0, 1)

                'Dim TestSound = TestSmaList(0).GetSoundFileSection(1)

                ' TestSound.WriteWaveFile("C:\SpeechTestFrameworkLog\Temp2\Test4.wav")

                Dim ComponentSound = TestComponent.GetSound(MyMediaSet, 0, 1, 60000)

                ComponentSound.WriteWaveFile("C:\SpeechTestFrameworkLog\Temp2\Test6.wav")

            Case 0

                Dim SM_Creator As New SpeechTestFramework.SpeechMaterialCreator(SpeechTestFramework.Utils.Constants.UserTypes.Research, False)

                SM_Creator.Show()

            Case 1

                SpeechTestFramework.OstfBase.LoadAvailableTestSpecifications()

                Dim SelectedTestIndex As Integer = 0

                SpeechTestFramework.OstfBase.AvailableTests(SelectedTestIndex).LoadSpeechMaterialComponentsFile()

                Dim CompleteSpeechMaterial = SpeechTestFramework.OstfBase.AvailableTests(SelectedTestIndex).SpeechMaterial

                'Dim CompleteSpeechMaterial = SpeechTestFramework.SpeechMaterialComponent.LoadSpeechMaterial(SpeechTestFramework.OstfBase.CurrentlySelectedTest.SpeechMaterialComponentsSubFilePath)

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
                    newAudioSettingsDialog = New SpeechTestFramework.AudioSettingsDialog()
                Else
                    newAudioSettingsDialog = New SpeechTestFramework.AudioSettingsDialog()
                    RecordingWaveFormat = New SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, 1, , SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints)
                End If

                Dim Result = newAudioSettingsDialog.ShowDialog()
                Dim CurrentAudioApiSettings As New SpeechTestFramework.Audio.AudioApiSettings()
                If Result = Windows.Forms.DialogResult.OK Then
                    CurrentAudioApiSettings = newAudioSettingsDialog.CurrentAudioApiSettings
                Else
                    'Attempting to set default AudioApiSettings if the user pressed ok
                    CurrentAudioApiSettings.SelectDefaultAudioDevice(RecordingWaveFormat.SampleRate)
                End If

                SoundPlayer = New SpeechTestFramework.Audio.PortAudioVB.OverlappingSoundPlayer(Nothing)

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

    Private Sub HINTfixButton_Click(sender As Object, e As EventArgs) Handles HINTfixButton.Click
        Dim InputFolder As String = "C:\EriksDokument\n200\Phonemically balanced 10-sentence lists"
        Dim OutputFolder As String = "C:\EriksDokument\n200\HINT_lists_Mono"

        Dim Files = IO.Directory.GetFiles(InputFolder)
        For Each file In Files
            If IO.Path.GetExtension(file) <> ".wav" Then Continue For

            Dim FileName = IO.Path.GetFileName(file)

            If FileName.StartsWith("CalibrationSignal") Then Continue For

            Dim InputFile = SpeechTestFramework.Audio.Sound.LoadWaveFile(file)

            Dim Channel1File = New SpeechTestFramework.Audio.Sound(New SpeechTestFramework.Audio.Formats.WaveFormat(InputFile.WaveFormat.SampleRate, InputFile.WaveFormat.BitDepth, 1,, InputFile.WaveFormat.Encoding))

            Channel1File.WaveData.SampleData(1) = InputFile.WaveData.SampleData(1)

            'Exporting the speech file
            Channel1File.SMA = Nothing
            Channel1File.WriteWaveFile(IO.Path.Combine(OutputFolder, FileName))

        Next

        MsgBox("Finished!")

    End Sub

    Private Sub HINTfixButton2_Click(sender As Object, e As EventArgs) Handles HINTfixButton2.Click

        'Getting the noise files

        Dim InputFolder As String = "C:\EriksDokument\n200\Phonemically balanced 10-sentence lists"
        Dim OutputFolder As String = "C:\EriksDokument\n200\HINT_Noises_Mono"

        Dim Files = IO.Directory.GetFiles(InputFolder)
        For Each file In Files
            If IO.Path.GetExtension(file) <> ".wav" Then Continue For

            Dim FileName = IO.Path.GetFileName(file)

            If FileName.StartsWith("CalibrationSignal") Then Continue For

            Dim InputFile = SpeechTestFramework.Audio.Sound.LoadWaveFile(file)

            Dim Channel1File = New SpeechTestFramework.Audio.Sound(New SpeechTestFramework.Audio.Formats.WaveFormat(InputFile.WaveFormat.SampleRate, InputFile.WaveFormat.BitDepth, 1,, InputFile.WaveFormat.Encoding))

            Channel1File.WaveData.SampleData(1) = InputFile.WaveData.SampleData(2)

            'Exporting the speech file
            Channel1File.SMA = Nothing
            Channel1File.WriteWaveFile(IO.Path.Combine(OutputFolder, FileName))

        Next

        MsgBox("Finished!")

    End Sub

    Private Sub HINTfixButton3_Click(sender As Object, e As EventArgs) Handles HINTfixButton3.Click

        'Getting the noise files

        Dim InputFolder1 As String = "C:\EriksDokument\n200\RecordingsVoiceOnly"
        Dim Files1 = IO.Directory.GetFiles(InputFolder1)

        Dim InputFolder2 As String = "C:\OSTFMedia\SpeechMaterials\SwedishHINT\Media\Standard\RecordingsVoiceOnly"
        Dim Files2 = IO.Directory.GetFiles(InputFolder2, "*", IO.SearchOption.AllDirectories)

        If Files1.Length <> Files2.Length Then
            MsgBox("Unequal number of files! Stopping operation now!")
            Exit Sub
        End If

        For n = 0 To Files1.Length - 1

            Dim InputFile1 = SpeechTestFramework.Audio.Sound.LoadWaveFile(Files1(n))
            Dim InputFile2 = SpeechTestFramework.Audio.Sound.LoadWaveFile(Files2(n))

            'Converting HINT recordings to 32 bit
            InputFile1 = InputFile1.Convert16to32bitSound()

            If InputFile1.WaveFormat.IsEqual(InputFile2.WaveFormat) = False Then
                MsgBox("Wave formats are not equal between " & Files1(n) & " and " & Files2(n) & " ! Stopping operation now!")
                Exit Sub
            End If

            Dim NewOutputPath As String = Files2(n).Replace("RecordingsVoiceOnly", "RecordingsVoiceOnly_New")

            'Copying channel 1 sound data from file 1 to file 2
            InputFile2.WaveData.SampleData(1) = InputFile1.WaveData.SampleData(1)

            'Exporting file 2
            InputFile2.WriteWaveFile(NewOutputPath)

        Next

        MsgBox("Finished!")

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

        Dim MyMixer = New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer()
        'MyMixer.SetLinearOutput()
        MyMixer.HardwareOutputChannelSpeakerLocations.Add(1, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = -30})
        MyMixer.HardwareOutputChannelSpeakerLocations.Add(2, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 0})
        MyMixer.HardwareOutputChannelSpeakerLocations.Add(3, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 30})

        MyMixer.SetupDirectionalSimulator(1, Sound_Background.WaveFormat)

        Dim OutputSound = MyMixer.CreateSoundScene(ItemList)

        OutputSound.WriteWaveFile("C:\Temp\OutputSound_S7.wav")

        Dim x = 1

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click


        Dim CalibrationDialog As New SpeechTestFramework.CalibrationForm(SpeechTestFramework.Utils.Constants.UserTypes.Research, True)
        CalibrationDialog.Show()

    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click

        SpeechTestFramework.Audio.AudioIOs.SamplerateConversion_DirectBatch("C:\OSTF\RoomImpulses\wierstorf2011\44100Hz", "C:\OSTF\RoomImpulses\wierstorf2011\48000Hz", New SpeechTestFramework.Audio.Formats.WaveFormat(44100, 32, 720, , SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints))

        MsgBox("Finished")
    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click

        Dim SondFilePath = SpeechTestFramework.Utils.GetOpenFilePath("")

        Dim SoundFile = SpeechTestFramework.Audio.Sound.LoadWaveFile(SondFilePath)

        SoundFile = SoundFile.Convert16to32bitSound()

        Dim SE = New SpeechTestFramework.Audio.Graphics.SoundEditor(SoundFile)
        SE.Dock = DockStyle.Fill

        Dim NewForm As New Windows.Forms.Form
        NewForm.Controls.Add(SE)
        NewForm.Show()

    End Sub

    Private SoundPlayer2 As SpeechTestFramework.Audio.PortAudioVB.SoundPlayer2

    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click

        Dim NumChan As Integer = 2
        Dim Sine As Boolean = True
        Dim Duraton As Integer = 20

        Dim OutputSound As New SpeechTestFramework.Audio.Sound(New SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, NumChan,, SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints))
        Dim Channel1 As SpeechTestFramework.Audio.Sound
        Dim Channel2 As SpeechTestFramework.Audio.Sound
        Dim Channel3 As SpeechTestFramework.Audio.Sound
        Dim Channel4 As SpeechTestFramework.Audio.Sound

        If Sine = True Then
            Channel1 = SpeechTestFramework.Audio.GenerateSound.CreateSineWave(New SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, 1,, SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints), 1, 250, 0.005,, Duraton)
            Channel2 = SpeechTestFramework.Audio.GenerateSound.CreateSineWave(New SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, 1,, SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints), 1, 400, 0.005,, Duraton)
            Channel3 = SpeechTestFramework.Audio.GenerateSound.CreateSineWave(New SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, 1,, SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints), 1, 700, 0.005,, Duraton)
            Channel4 = SpeechTestFramework.Audio.GenerateSound.CreateSineWave(New SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, 1,, SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints), 1, 900, 0.005,, Duraton)
        Else
            Channel1 = SpeechTestFramework.Audio.GenerateSound.CreateSilence(New SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, 1,, SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints), 1, Duraton)
            Channel2 = SpeechTestFramework.Audio.GenerateSound.CreateSilence(New SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, 1,, SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints), 1, Duraton)
            Channel3 = SpeechTestFramework.Audio.GenerateSound.CreateSilence(New SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, 1,, SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints), 1, Duraton)
            Channel4 = SpeechTestFramework.Audio.GenerateSound.CreateSilence(New SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, 1,, SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints), 1, Duraton)
        End If

        If NumChan > 0 Then OutputSound.WaveData.SampleData(1) = Channel1.WaveData.SampleData(1)
        If NumChan > 1 Then OutputSound.WaveData.SampleData(2) = Channel2.WaveData.SampleData(1)
        If NumChan > 2 Then OutputSound.WaveData.SampleData(3) = Channel3.WaveData.SampleData(1)
        If NumChan > 3 Then OutputSound.WaveData.SampleData(4) = Channel4.WaveData.SampleData(1)

        If Sine = False Then
            For c = 1 To NumChan
                OutputSound.WaveData.SampleData(c)(5 * 48000) = 1
                'OutputSound.WaveData.SampleData(c)(5 * 48000 + 1) = -1
                'OutputSound.WaveData.SampleData(c)(48002) = 1
                'OutputSound.WaveData.SampleData(c)(48003) = -1
            Next
        End If

        'Selects the wave format for use (doing it this way means that the wave format MUST be the same in all available MediaSets)
        Dim Transducers = SpeechTestFramework.AvaliableTransducers
        Dim Transducer = Transducers(3)


        SoundPlayer2 = New SpeechTestFramework.Audio.PortAudioVB.SoundPlayer2(OutputSound, Transducer.ParentAudioApiSettings)
        SoundPlayer2.OpenStream()
        SoundPlayer2.Start()

        Exit Sub


        SpeechTestFramework.SoundPlayer.ChangePlayerSettings(, OutputSound.WaveFormat.SampleRate, OutputSound.WaveFormat.BitDepth, OutputSound.WaveFormat.Encoding,,, SpeechTestFramework.Audio.PortAudioVB.OverlappingSoundPlayer.SoundDirections.PlaybackOnly, False, False)
        SpeechTestFramework.SoundPlayer.ChangePlayerSettings(Transducer.ParentAudioApiSettings, ,,, 0.4, Transducer.Mixer,, True, True)
        SpeechTestFramework.SoundPlayer.SwapOutputSounds(OutputSound)

    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        If SoundPlayer2 IsNot Nothing Then
            SoundPlayer2.StopStream()
            SoundPlayer2.CloseStream()
            SoundPlayer2.Dispose()
        End If

    End Sub

    Private Sub TestWordLabel1_Click(sender As Object, e As EventArgs) Handles TestWordLabel1.Click

        Dim DirForm = New Form
        DirForm.Width = 800
        DirForm.Height = 600
        Dim DirControl = New SpeechTestFramework.DirectionalForcedChoiceControl
        DirForm.Controls.Add(DirControl)
        DirForm.Show()

    End Sub

    Private Sub Button16_Click(sender As Object, e As EventArgs) 'Handles MoveFilesButton.Click

        Dim OutputPaths As New List(Of String) From {"Block05\Pseudo\1002-P-10864.mp4",
"Block05\Real\1003-R-10864.mp4",
"Block07\Real\1004-R-2817.mp4",
"Block07\Pseudo\1005-P-2817.mp4",
"Block09\Real\1008-R-7696.mp4",
"Block09\Pseudo\1009-P-7696.mp4",
"Block02\Pseudo\1010-P-17137.mp4",
"Block02\Real\1011-R-17137.mp4",
"Block01\Real\1012-R-3194.mp4",
"Block01\Pseudo\1013-P-3194.mp4",
"Block03\Real\1020-R-10083.mp4",
"Block03\Pseudo\1021-P-10083.mp4",
"Block04\Pseudo\1022-P-10571.mp4",
"Block04\Real\1023-R-10571.mp4",
"Block06\Real\1024-R-11199.mp4",
"Block06\Pseudo\1025-P-11199.mp4",
"Block03\Pseudo\1026-P-2301.mp4",
"Block03\Real\1027-R-2301.mp4",
"Block09\Pseudo\1030-P-12357.mp4",
"Block09\Real\1031-R-12357.mp4",
"Block01\Real\1032-R-2531.mp4",
"Block01\Pseudo\1033-P-2531.mp4",
"Block06\Pseudo\1034-P-1679.mp4",
"Block06\Real\1035-R-1679.mp4",
"Block06\Real\1036-R-19474.mp4",
"Block06\Pseudo\1037-P-19474.mp4",
"Block08\Pseudo\1038-P-15655.mp4",
"Block08\Real\1039-R-15655.mp4",
"Block06\Pseudo\1042-P-13079.mp4",
"Block06\Real\1043-R-13079.mp4",
"Block09\Real\1044-R-13970.mp4",
"Block09\Pseudo\1045-P-13970.mp4",
"Block010\Pseudo\1046-P-5015.mp4",
"Block010\Real\1047-R-5015.mp4",
"Block010\Real\1048-R-6277.mp4",
"Block010\Pseudo\1049-P-6277.mp4",
"Block03\Real\1052-R-11754.mp4",
"Block03\Pseudo\1053-P-11754.mp4",
"Block03\Pseudo\1054-P-1696.mp4",
"Block03\Real\1055-R-1696.mp4",
"Block02\Real\1056-R-5538.mp4",
"Block02\Pseudo\1057-P-5538.mp4",
"Block06\Pseudo\1058-P-13916.mp4",
"Block06\Real\1059-R-13916.mp4",
"Block09\Pseudo\1062-P-1070.mp4",
"Block09\Real\1063-R-1070.mp4",
"Block04\Real\1064-R-8640.mp4",
"Block04\Pseudo\1065-P-8640.mp4",
"Block05\Pseudo\1066-P-4401.mp4",
"Block05\Real\1067-R-4401.mp4",
"Block02\Real\1068-R-19372.mp4",
"Block02\Pseudo\1069-P-19372.mp4",
"Block03\Pseudo\1070-P-6315.mp4",
"Block03\Real\1071-R-6315.mp4",
"Block09\Real\1072-R-2966.mp4",
"Block09\Pseudo\1073-P-2966.mp4",
"Block05\Real\1076-R-6388.mp4",
"Block05\Pseudo\1077-P-6388.mp4",
"Block07\Pseudo\1078-P-3340.mp4",
"Block07\Real\1079-R-3340.mp4",
"Block06\Real\1080-R-1355.mp4",
"Block06\Pseudo\1081-P-1355.mp4",
"Block05\Pseudo\1082-P-17031.mp4",
"Block05\Real\1083-R-17031.mp4",
"Block02\Real\1084-R-5596.mp4",
"Block02\Pseudo\1085-P-5596.mp4",
"Block03\Pseudo\1090-P-4686.mp4",
"Block03\Real\1091-R-4686.mp4",
"Block05\Real\1092-R-14542.mp4",
"Block05\Pseudo\1093-P-14542.mp4",
"Block010\Pseudo\1094-P-14792.mp4",
"Block010\Real\1095-R-14792.mp4",
"Block01\Real\1096-R-128.mp4",
"Block01\Pseudo\1097-P-128.mp4",
"Block010\Pseudo\1098-P-7466.mp4",
"Block010\Real\1099-R-7466.mp4",
"Block07\Pseudo\1102-P-19835.mp4",
"Block07\Real\1103-R-19835.mp4",
"Block010\Real\1104-R-4072.mp4",
"Block010\Pseudo\1105-P-4072.mp4",
"Block03\Pseudo\1106-P-14125.mp4",
"Block03\Real\1107-R-14125.mp4",
"Block08\Real\1108-R-1092.mp4",
"Block08\Pseudo\1109-P-1092.mp4",
"Block04\Real\1112-R-14594.mp4",
"Block04\Pseudo\1113-P-14594.mp4",
"Block04\Pseudo\1114-P-18933.mp4",
"Block04\Real\1115-R-18933.mp4",
"Block07\Real\1116-R-7940.mp4",
"Block07\Pseudo\1117-P-7940.mp4",
"Block08\Pseudo\1118-P-7502.mp4",
"Block08\Real\1119-R-7502.mp4",
"Block07\Pseudo\1122-P-589.mp4",
"Block07\Real\1123-R-589.mp4",
"Block06\Pseudo\1126-P-2246.mp4",
"Block06\Real\1127-R-2246.mp4",
"Block07\Real\1128-R-832.mp4",
"Block07\Pseudo\1129-P-832.mp4",
"Block01\Pseudo\1130-P-12469.mp4",
"Block01\Real\1131-R-12469.mp4",
"Block03\Real\1132-R-7498.mp4",
"Block03\Pseudo\1133-P-7498.mp4",
"Block02\Pseudo\1134-P-17763.mp4",
"Block02\Real\1135-R-17763.mp4",
"Block01\Real\1136-R-18824.mp4",
"Block01\Pseudo\1137-P-18824.mp4",
"Block010\Pseudo\1138-P-11085.mp4",
"Block010\Real\1139-R-11085.mp4",
"Block07\Pseudo\1142-P-18687.mp4",
"Block07\Real\1143-R-18687.mp4",
"Block010\Real\1144-R-12177.mp4",
"Block010\Pseudo\1145-P-12177.mp4",
"Block07\Pseudo\1146-P-11502.mp4",
"Block07\Real\1147-R-11502.mp4",
"Block08\Pseudo\1150-P-6806.mp4",
"Block08\Real\1151-R-6806.mp4",
"Block01\Pseudo\1154-P-13028.mp4",
"Block01\Real\1155-R-13028.mp4",
"Block08\Real\1156-R-1118.mp4",
"Block08\Pseudo\1157-P-1118.mp4",
"Block06\Real\1160-R-2749.mp4",
"Block06\Pseudo\1161-P-2749.mp4",
"Block02\Pseudo\1166-P-1410.mp4",
"Block02\Real\1167-R-1410.mp4",
"Block05\Real\1168-R-15993.mp4",
"Block05\Pseudo\1169-P-15993.mp4",
"Block03\Pseudo\1170-P-20112.mp4",
"Block03\Real\1171-R-20112.mp4",
"Block04\Real\1172-R-17810.mp4",
"Block04\Pseudo\1173-P-17810.mp4",
"Block08\Pseudo\1174-P-1575.mp4",
"Block08\Real\1175-R-1575.mp4",
"Block01\Real\1176-R-3691.mp4",
"Block01\Pseudo\1177-P-3691.mp4",
"Block02\Real\1180-R-6920.mp4",
"Block02\Pseudo\1181-P-6920.mp4",
"Block02\Pseudo\1182-P-1713.mp4",
"Block02\Real\1183-R-1713.mp4",
"Block05\Real\1184-R-4928.mp4",
"Block05\Pseudo\1185-P-4928.mp4",
"Block05\Pseudo\1186-P-1237.mp4",
"Block05\Real\1187-R-1237.mp4",
"Block04\Real\1188-R-2423.mp4",
"Block04\Pseudo\1189-P-2423.mp4",
"Block05\Pseudo\1190-P-19855.mp4",
"Block05\Real\1191-R-19855.mp4",
"Block01\Real\1192-R-17333.mp4",
"Block01\Pseudo\1193-P-17333.mp4",
"Block04\Pseudo\1194-P-5670.mp4",
"Block04\Real\1195-R-5670.mp4",
"Block04\Real\1196-R-11833.mp4",
"Block04\Pseudo\1197-P-11833.mp4",
"Block07\Real\1200-R-1399.mp4",
"Block07\Pseudo\1201-P-1399.mp4",
"Block01\Pseudo\1202-P-18930.mp4",
"Block01\Real\1203-R-18930.mp4",
"Block09\Real\1204-R-4372.mp4",
"Block09\Pseudo\1205-P-4372.mp4",
"Block08\Pseudo\1206-P-12909.mp4",
"Block08\Real\1207-R-12909.mp4",
"Block06\Real\1208-R-18984.mp4",
"Block06\Pseudo\1209-P-18984.mp4",
"Block08\Real\1212-R-875.mp4",
"Block08\Pseudo\1213-P-875.mp4",
"Block04\Pseudo\1214-P-20055.mp4",
"Block04\Real\1215-R-20055.mp4",
"Block07\Real\1216-R-14276.mp4",
"Block07\Pseudo\1217-P-14276.mp4",
"Block010\Real\1220-R-2025.mp4",
"Block010\Pseudo\1221-P-2025.mp4",
"Block09\Pseudo\1222-P-3104.mp4",
"Block09\Real\1223-R-3104.mp4",
"Block04\Real\1224-R-18752.mp4",
"Block04\Pseudo\1225-P-18752.mp4",
"Block09\Pseudo\1226-P-1401.mp4",
"Block09\Real\1227-R-1401.mp4",
"Block02\Pseudo\1230-P-2073.mp4",
"Block02\Real\1231-R-2073.mp4",
"Block010\Real\1232-R-19991.mp4",
"Block010\Pseudo\1233-P-19991.mp4",
"Block05\Pseudo\1234-P-912.mp4",
"Block05\Real\1235-R-912.mp4",
"Block03\Real\1236-R-2694.mp4",
"Block03\Pseudo\1237-P-2694.mp4",
"Block010\Pseudo\1238-P-1238.mp4",
"Block010\Real\1239-R-1238.mp4",
"Block08\Real\1240-R-14984.mp4",
"Block08\Pseudo\1241-P-14984.mp4",
"Block09\Real\1244-R-11625.mp4",
"Block09\Pseudo\1245-P-11625.mp4",
"Block01\Pseudo\1246-P-3506.mp4",
"Block01\Real\1247-R-3506.mp4",
"Block02\Pseudo\1250-P-18600.mp4",
"Block02\Real\1251-R-18600.mp4",
"Block08\Real\1252-R-9073.mp4",
"Block08\Pseudo\1253-P-9073.mp4",
"Block09\Pseudo\1254-P-1404.mp4",
"Block09\Real\1255-R-1404.mp4",
"Block06\Real\1256-R-677.mp4",
"Block06\Pseudo\1257-P-677.mp4",
"Block03\Real\1260-R-12457.mp4",
"Block03\Pseudo\1261-P-12457.mp4",
"Block08\Real\1264-R-10457.mp4",
"Block08\Pseudo\1265-P-10457.mp4",
"Block07\Pseudo\1266-P-2813.mp4",
"Block07\Real\1267-R-2813.mp4",
"Block09\Real\1268-R-5743.mp4",
"Block09\Pseudo\1269-P-5743.mp4",
"Block06\Pseudo\1270-P-10038.mp4",
"Block06\Real\1271-R-10038.mp4",
"Block03\Real\1276-R-1056.mp4",
"Block03\Pseudo\1277-P-1056.mp4",
"Block04\Real\1280-R-16254.mp4",
"Block04\Pseudo\1281-P-16254.mp4",
"Block09\Pseudo\1282-P-12274.mp4",
"Block09\Real\1283-R-12274.mp4",
"Block09\Real\1284-R-6912.mp4",
"Block09\Pseudo\1285-P-6912.mp4",
"Block02\Pseudo\1286-P-6230.mp4",
"Block02\Real\1287-R-6230.mp4",
"Block01\Real\1288-R-3370.mp4",
"Block01\Pseudo\1289-P-3370.mp4",
"Block08\Pseudo\1290-P-2533.mp4",
"Block08\Real\1291-R-2533.mp4",
"Block06\Real\1296-R-13485.mp4",
"Block06\Pseudo\1297-P-13485.mp4",
"Block01\Pseudo\1298-P-1022.mp4",
"Block01\Real\1299-R-1022.mp4",
"Block06\Real\1300-R-10423.mp4",
"Block06\Pseudo\1301-P-10423.mp4",
"Block03\Pseudo\1302-P-6333.mp4",
"Block03\Real\1303-R-6333.mp4",
"Block05\Real\1304-R-8162.mp4",
"Block05\Pseudo\1305-P-8162.mp4",
"Block06\Pseudo\1306-P-2839.mp4",
"Block06\Real\1307-R-2839.mp4",
"Block05\Real\1308-R-11129.mp4",
"Block05\Pseudo\1309-P-11129.mp4",
"Block04\Real\1312-R-2255.mp4",
"Block04\Pseudo\1313-P-2255.mp4",
"Block010\Pseudo\1314-P-15829.mp4",
"Block010\Real\1315-R-15829.mp4",
"Block03\Real\1316-R-7446.mp4",
"Block03\Pseudo\1317-P-7446.mp4",
"Block01\Pseudo\1318-P-14812.mp4",
"Block01\Real\1319-R-14812.mp4",
"Block03\Pseudo\1322-P-16600.mp4",
"Block03\Real\1323-R-16600.mp4",
"Block08\Real\1324-R-16236.mp4",
"Block08\Pseudo\1325-P-16236.mp4",
"Block010\Pseudo\1326-P-2108.mp4",
"Block010\Real\1327-R-2108.mp4",
"Block010\Pseudo\1330-P-298.mp4",
"Block010\Real\1331-R-298.mp4",
"Block04\Real\1332-R-27.mp4",
"Block04\Pseudo\1333-P-27.mp4",
"Block01\Pseudo\1334-P-17064.mp4",
"Block01\Real\1335-R-17064.mp4",
"Block07\Real\1336-R-3010.mp4",
"Block07\Pseudo\1337-P-3010.mp4",
"Block08\Pseudo\1338-P-5236.mp4",
"Block08\Real\1339-R-5236.mp4",
"Block04\Real\1340-R-10866.mp4",
"Block04\Pseudo\1341-P-10866.mp4",
"Block02\Pseudo\1346-P-6616.mp4",
"Block02\Real\1347-R-6616.mp4",
"Block09\Real\1348-R-2071.mp4",
"Block09\Pseudo\1349-P-2071.mp4",
"Block01\Pseudo\1350-P-6515.mp4",
"Block01\Real\1351-R-6515.mp4",
"Block07\Pseudo\1354-P-7867.mp4",
"Block07\Real\1355-R-7867.mp4",
"Block01\Real\1356-R-7137.mp4",
"Block01\Pseudo\1357-P-7137.mp4",
"Block06\Real\1360-R-14978.mp4",
"Block06\Pseudo\1361-P-14978.mp4",
"Block09\Pseudo\1362-P-13309.mp4",
"Block09\Real\1363-R-13309.mp4",
"Block05\Real\1364-R-7899.mp4",
"Block05\Pseudo\1365-P-7899.mp4",
"Block010\Pseudo\1366-P-12480.mp4",
"Block010\Real\1367-R-12480.mp4",
"Block06\Real\1368-R-11900.mp4",
"Block06\Pseudo\1369-P-11900.mp4",
"Block01\Pseudo\1370-P-5533.mp4",
"Block01\Real\1371-R-5533.mp4",
"Block01\Real\1372-R-207.mp4",
"Block01\Pseudo\1373-P-207.mp4",
"Block09\Real\1376-R-9982.mp4",
"Block09\Pseudo\1377-P-9982.mp4",
"Block06\Pseudo\1378-P-371.mp4",
"Block06\Real\1379-R-371.mp4",
"Block06\Real\1380-R-17381.mp4",
"Block06\Pseudo\1381-P-17381.mp4",
"Block010\Pseudo\1382-P-764.mp4",
"Block010\Real\1383-R-764.mp4",
"Block09\Real\1384-R-1660.mp4",
"Block09\Pseudo\1385-P-1660.mp4",
"Block03\Pseudo\1386-P-187.mp4",
"Block03\Real\1387-R-187.mp4",
"Block09\Real\1392-R-4789.mp4",
"Block09\Pseudo\1393-P-4789.mp4",
"Block02\Real\1396-R-3351.mp4",
"Block02\Pseudo\1397-P-3351.mp4",
"Block02\Real\1404-R-15724.mp4",
"Block02\Pseudo\1405-P-15724.mp4",
"Block010\Pseudo\1406-P-3523.mp4",
"Block010\Real\1407-R-3523.mp4",
"Block04\Real\1408-R-4907.mp4",
"Block04\Pseudo\1409-P-4907.mp4",
"Block05\Pseudo\1410-P-11880.mp4",
"Block05\Real\1411-R-11880.mp4",
"Block07\Real\1412-R-3331.mp4",
"Block07\Pseudo\1413-P-3331.mp4",
"Block010\Pseudo\1414-P-4567.mp4",
"Block010\Real\1415-R-4567.mp4",
"Block08\Real\1416-R-19469.mp4",
"Block08\Pseudo\1417-P-19469.mp4",
"Block08\Pseudo\1418-P-1455.mp4",
"Block08\Real\1419-R-1455.mp4",
"Block02\Pseudo\1422-P-4291.mp4",
"Block02\Real\1423-R-4291.mp4",
"Block08\Real\1424-R-14522.mp4",
"Block08\Pseudo\1425-P-14522.mp4",
"Block01\Pseudo\1426-P-12573.mp4",
"Block01\Real\1427-R-12573.mp4",
"Block05\Real\1428-R-3539.mp4",
"Block05\Pseudo\1429-P-3539.mp4",
"Block04\Pseudo\1434-P-17227.mp4",
"Block04\Real\1435-R-17227.mp4",
"Block04\Real\1436-R-262.mp4",
"Block04\Pseudo\1437-P-262.mp4",
"Block03\Pseudo\1438-P-4118.mp4",
"Block03\Real\1439-R-4118.mp4",
"Block05\Real\1440-R-9143.mp4",
"Block05\Pseudo\1441-P-9143.mp4",
"Block03\Real\1444-R-8364.mp4",
"Block03\Pseudo\1445-P-8364.mp4",
"Block09\Pseudo\1446-P-15782.mp4",
"Block09\Real\1447-R-15782.mp4",
"Block05\Pseudo\1450-P-8230.mp4",
"Block05\Real\1451-R-8230.mp4",
"Block03\Real\1452-R-9989.mp4",
"Block03\Pseudo\1453-P-9989.mp4",
"Block07\Real\1456-R-11827.mp4",
"Block07\Pseudo\1457-P-11827.mp4",
"Block02\Pseudo\1458-P-228.mp4",
"Block02\Real\1459-R-228.mp4",
"Block04\Real\1460-R-4043.mp4",
"Block04\Pseudo\1461-P-4043.mp4",
"Block08\Pseudo\1462-P-247.mp4",
"Block08\Real\1463-R-247.mp4",
"Block07\Real\1464-R-3052.mp4",
"Block07\Pseudo\1465-P-3052.mp4",
"Block010\Pseudo\1466-P-1076.mp4",
"Block010\Real\1467-R-1076.mp4",
"Block07\Real\1468-R-18870.mp4",
"Block07\Pseudo\1469-P-18870.mp4",
"Block08\Pseudo\1470-P-4123.mp4",
"Block08\Real\1471-R-4123.mp4",
"Block06\Real\1472-R-1967.mp4",
"Block06\Pseudo\1473-P-1967.mp4",
"Block05\Pseudo\1474-P-3486.mp4",
"Block05\Real\1475-R-3486.mp4",
"Block04\Real\1476-R-11603.mp4",
"Block04\Pseudo\1477-P-11603.mp4",
"Block09\Pseudo\1478-P-12707.mp4",
"Block09\Real\1479-R-12707.mp4",
"Block07\Real\1480-R-1249.mp4",
"Block07\Pseudo\1481-P-1249.mp4",
"Block05\Pseudo\1482-P-17389.mp4",
"Block05\Real\1483-R-17389.mp4",
"Block04\Real\1484-R-690.mp4",
"Block04\Pseudo\1485-P-690.mp4",
"Block01\Pseudo\1486-P-2041.mp4",
"Block01\Real\1487-R-2041.mp4",
"Block08\Real\1488-R-17022.mp4",
"Block08\Pseudo\1489-P-17022.mp4",
"Block010\Pseudo\1490-P-3.mp4",
"Block010\Real\1491-R-3.mp4",
"Block06\Real\1492-R-10997.mp4",
"Block06\Pseudo\1493-P-10997.mp4",
"Block07\Pseudo\1494-P-13951.mp4",
"Block07\Real\1495-R-13951.mp4",
"Block02\Real\1496-R-6871.mp4",
"Block02\Pseudo\1497-P-6871.mp4",
"Block02\Real\1500-R-51.mp4",
"Block02\Pseudo\1501-P-51.mp4",
"Block03\Pseudo\1502-P-5483.mp4",
"Block03\Real\1503-R-5483.mp4",
"Block05\Real\1504-R-647.mp4",
"Block05\Pseudo\1505-P-647.mp4",
"Block010\Pseudo\1506-P-2616.mp4",
"Block010\Real\1507-R-2616.mp4",
"Block07\Real\1508-R-406.mp4",
"Block07\Pseudo\1509-P-406.mp4",
"Block02\Real\1516-R-18766.mp4",
"Block02\Pseudo\1517-P-18766.mp4",
"Block02\Pseudo\1518-P-14298.mp4",
"Block02\Real\1519-R-14298.mp4",
"Block010\Real\1520-R-19404.mp4",
"Block010\Pseudo\1521-P-19404.mp4",
"Block01\Real\1524-R-5559.mp4",
"Block01\Pseudo\1525-P-5559.mp4",
"Block02\Pseudo\1526-P-14384.mp4",
"Block02\Real\1527-R-14384.mp4",
"Block05\Real\1528-R-3888.mp4",
"Block05\Pseudo\1529-P-3888.mp4",
"Block04\Pseudo\1530-P-1775.mp4",
"Block04\Real\1531-R-1775.mp4",
"Block03\Real\1532-R-6188.mp4",
"Block03\Pseudo\1533-P-6188.mp4",
"Block09\Pseudo\1534-P-6513.mp4",
"Block09\Real\1535-R-6513.mp4",
"Block06\Real\1536-R-12416.mp4",
"Block06\Pseudo\1537-P-12416.mp4",
"Block010\Pseudo\1538-P-9940.mp4",
"Block010\Real\1539-R-9940.mp4",
"Block09\Real\1540-R-464.mp4",
"Block09\Pseudo\1541-P-464.mp4",
"Block01\Pseudo\1542-P-4489.mp4",
"Block01\Real\1543-R-4489.mp4",
"Block02\Real\1552-R-1341.mp4",
"Block02\Pseudo\1553-P-1341.mp4",
"Block04\Pseudo\1554-P-1379.mp4",
"Block04\Real\1555-R-1379.mp4",
"Block09\Real\1556-R-10489.mp4",
"Block09\Pseudo\1557-P-10489.mp4",
"Block07\Real\1560-R-5708.mp4",
"Block07\Pseudo\1561-P-5708.mp4",
"Block06\Pseudo\1562-P-9781.mp4",
"Block06\Real\1563-R-9781.mp4",
"Block08\Real\1564-R-15606.mp4",
"Block08\Pseudo\1565-P-15606.mp4",
"Block010\Real\1568-R-3616.mp4",
"Block010\Pseudo\1569-P-3616.mp4",
"Block03\Pseudo\1570-P-9137.mp4",
"Block03\Real\1571-R-9137.mp4",
"Block09\Real\1572-R-14108.mp4",
"Block09\Pseudo\1573-P-14108.mp4",
"Block05\Real\1576-R-10785.mp4",
"Block05\Pseudo\1577-P-10785.mp4",
"Block03\Pseudo\1578-P-15873.mp4",
"Block03\Real\1579-R-15873.mp4",
"Block08\Real\1580-R-304.mp4",
"Block08\Pseudo\1581-P-304.mp4",
"Block07\Pseudo\1582-P-3565.mp4",
"Block07\Real\1583-R-3565.mp4",
"Block03\Real\1584-R-2118.mp4",
"Block03\Pseudo\1585-P-2118.mp4",
"Block07\Pseudo\1586-P-10528.mp4",
"Block07\Real\1587-R-10528.mp4",
"Block03\Real\1588-R-12261.mp4",
"Block03\Pseudo\1589-P-12261.mp4",
"Block010\Real\1592-R-7104.mp4",
"Block010\Pseudo\1593-P-7104.mp4",
"Block07\Pseudo\1594-P-11153.mp4",
"Block07\Real\1595-R-11153.mp4",
"Block08\Pseudo\1598-P-3627.mp4",
"Block08\Real\1599-R-3627.mp4",
"Block01\Real\1600-R-2039.mp4",
"Block01\Pseudo\1601-P-2039.mp4",
"Block01\Pseudo\1602-P-19283.mp4",
"Block01\Real\1603-R-19283.mp4",
"Block07\Real\1604-R-19152.mp4",
"Block07\Pseudo\1605-P-19152.mp4",
"Block010\Pseudo\1606-P-14479.mp4",
"Block010\Real\1607-R-14479.mp4",
"Block010\Real\1608-R-14235.mp4",
"Block010\Pseudo\1609-P-14235.mp4",
"Block01\Pseudo\1610-P-8986.mp4",
"Block01\Real\1611-R-8986.mp4",
"Block07\Real\1612-R-1502.mp4",
"Block07\Pseudo\1613-P-1502.mp4",
"Block07\Real\1616-R-9101.mp4",
"Block07\Pseudo\1617-P-9101.mp4",
"Block04\Pseudo\1618-P-4910.mp4",
"Block04\Real\1619-R-4910.mp4",
"Block05\Real\1620-R-5323.mp4",
"Block05\Pseudo\1621-P-5323.mp4",
"Block03\Pseudo\1622-P-3004.mp4",
"Block03\Real\1623-R-3004.mp4",
"Block09\Real\1624-R-3017.mp4",
"Block09\Pseudo\1625-P-3017.mp4",
"Block02\Pseudo\1626-P-15234.mp4",
"Block02\Real\1627-R-15234.mp4",
"Block010\Real\1628-R-1077.mp4",
"Block010\Pseudo\1629-P-1077.mp4",
"Block06\Pseudo\1630-P-8820.mp4",
"Block06\Real\1631-R-8820.mp4",
"Block08\Real\1632-R-20095.mp4",
"Block08\Pseudo\1633-P-20095.mp4",
"Block04\Pseudo\1634-P-13888.mp4",
"Block04\Real\1635-R-13888.mp4",
"Block02\Real\1636-R-872.mp4",
"Block02\Pseudo\1637-P-872.mp4",
"Block05\Pseudo\1638-P-4547.mp4",
"Block05\Real\1639-R-4547.mp4",
"Block03\Real\1640-R-6456.mp4",
"Block03\Pseudo\1641-P-6456.mp4",
"Block05\Pseudo\1642-P-2265.mp4",
"Block05\Real\1643-R-2265.mp4",
"Block03\Real\1644-R-15464.mp4",
"Block03\Pseudo\1645-P-15464.mp4",
"Block08\Real\1648-R-16338.mp4",
"Block08\Pseudo\1649-P-16338.mp4",
"Block04\Pseudo\1650-P-11014.mp4",
"Block04\Real\1651-R-11014.mp4",
"Block03\Real\1652-R-3837.mp4",
"Block03\Pseudo\1653-P-3837.mp4",
"Block01\Real\1656-R-2371.mp4",
"Block01\Pseudo\1657-P-2371.mp4",
"Block06\Pseudo\1658-P-10445.mp4",
"Block06\Real\1659-R-10445.mp4",
"Block01\Pseudo\1666-P-8972.mp4",
"Block01\Real\1667-R-8972.mp4",
"Block09\Real\1668-R-10531.mp4",
"Block09\Pseudo\1669-P-10531.mp4",
"Block03\Pseudo\1670-P-4104.mp4",
"Block03\Real\1671-R-4104.mp4",
"Block02\Real\1672-R-8875.mp4",
"Block02\Pseudo\1673-P-8875.mp4",
"Block09\Real\1676-R-14875.mp4",
"Block09\Pseudo\1677-P-14875.mp4",
"Block06\Pseudo\1678-P-13559.mp4",
"Block06\Real\1679-R-13559.mp4",
"Block06\Real\1680-R-3014.mp4",
"Block06\Pseudo\1681-P-3014.mp4",
"Block04\Pseudo\1682-P-20142.mp4",
"Block04\Real\1683-R-20142.mp4",
"Block01\Pseudo\1686-P-7036.mp4",
"Block01\Real\1687-R-7036.mp4",
"Block05\Real\1688-R-4481.mp4",
"Block05\Pseudo\1689-P-4481.mp4",
"Block09\Real\1692-R-1319.mp4",
"Block09\Pseudo\1693-P-1319.mp4",
"Block08\Pseudo\1694-P-2304.mp4",
"Block08\Real\1695-R-2304.mp4",
"Block09\Real\1696-R-13969.mp4",
"Block09\Pseudo\1697-P-13969.mp4",
"Block02\Real\1700-R-59.mp4",
"Block02\Pseudo\1701-P-59.mp4",
"Block04\Pseudo\1702-P-1060.mp4",
"Block04\Real\1703-R-1060.mp4",
"Block010\Real\1704-R-2177.mp4",
"Block010\Pseudo\1705-P-2177.mp4",
"Block04\Pseudo\1706-P-12685.mp4",
"Block04\Real\1707-R-12685.mp4",
"Block08\Real\1708-R-19342.mp4",
"Block08\Pseudo\1709-P-19342.mp4",
"Block08\Pseudo\1710-P-8480.mp4",
"Block08\Real\1711-R-8480.mp4",
"Block06\Real\1712-R-3515.mp4",
"Block06\Pseudo\1713-P-3515.mp4",
"Block08\Pseudo\1714-P-10039.mp4",
"Block08\Real\1715-R-10039.mp4",
"Block01\Real\1716-R-1787.mp4",
"Block01\Pseudo\1717-P-1787.mp4",
"Block05\Real\1720-R-2890.mp4",
"Block05\Pseudo\1721-P-2890.mp4",
"Block07\Real\1724-R-1945.mp4",
"Block07\Pseudo\1725-P-1945.mp4",
"Block05\Pseudo\1726-P-11572.mp4",
"Block05\Real\1727-R-11572.mp4",
"Block06\Real\1728-R-354.mp4",
"Block06\Pseudo\1729-P-354.mp4",
"Block06\Pseudo\1730-P-1191.mp4",
"Block06\Real\1731-R-1191.mp4",
"Block04\Real\1732-R-17085.mp4",
"Block04\Pseudo\1733-P-17085.mp4",
"Block05\Real\1736-R-1212.mp4",
"Block05\Pseudo\1737-P-1212.mp4",
"Block02\Pseudo\1738-P-1683.mp4",
"Block02\Real\1739-R-1683.mp4",
"Block02\Pseudo\1742-P-4830.mp4",
"Block02\Real\1743-R-4830.mp4",
"Block02\Real\1744-R-609.mp4",
"Block02\Pseudo\1745-P-609.mp4",
"Block08\Real\1748-R-19897.mp4",
"Block08\Pseudo\1749-P-19897.mp4",
"Block09\Pseudo\1750-P-796.mp4",
"Block09\Real\1751-R-796.mp4",
"Block01\Pseudo\1754-P-428.mp4",
"Block01\Real\1755-R-428.mp4",
"Block07\Real\1756-R-3284.mp4",
"Block07\Pseudo\1757-P-3284.mp4",
"Block05\Pseudo\1758-P-3954.mp4",
"Block05\Real\1759-R-3954.mp4",
"Block04\Real\1760-R-1409.mp4",
"Block04\Pseudo\1761-P-1409.mp4",
"Block07\Real\1764-R-14576.mp4",
"Block07\Pseudo\1765-P-14576.mp4",
"Block010\Pseudo\1770-P-18541.mp4",
"Block010\Real\1771-R-18541.mp4",
"Block06\Pseudo\1774-P-6263.mp4",
"Block06\Real\1775-R-6263.mp4",
"Block02\Real\1776-R-19280.mp4",
"Block02\Pseudo\1777-P-19280.mp4",
"Block010\Pseudo\1778-P-1722.mp4",
"Block010\Real\1779-R-1722.mp4",
"Block03\Pseudo\1782-P-5445.mp4",
"Block03\Real\1783-R-5445.mp4",
"Block06\Real\1784-R-6523.mp4",
"Block06\Pseudo\1785-P-6523.mp4",
"Block03\Real\1788-R-15732.mp4",
"Block03\Pseudo\1789-P-15732.mp4",
"Block08\Pseudo\1790-P-12020.mp4",
"Block08\Real\1791-R-12020.mp4",
"Block09\Real\1792-R-9136.mp4",
"Block09\Pseudo\1793-P-9136.mp4",
"Block09\Pseudo\1794-P-6078.mp4",
"Block09\Real\1795-R-6078.mp4",
"Block01\Real\1796-R-17355.mp4",
"Block01\Pseudo\1797-P-17355.mp4",
"Block04\Pseudo\1798-P-15778.mp4",
"Block04\Real\1799-R-15778.mp4",
"Block02\Real\1800-R-1684.mp4",
"Block02\Pseudo\1801-P-1684.mp4",
"Block05\Real\1804-R-13755.mp4",
"Block05\Pseudo\1805-P-13755.mp4",
"Block07\Pseudo\1806-P-3513.mp4",
"Block07\Real\1807-R-3513.mp4",
"Block04\Real\1808-R-7464.mp4",
"Block04\Pseudo\1809-P-7464.mp4",
"Block05\Pseudo\1810-P-5328.mp4",
"Block05\Real\1811-R-5328.mp4",
"Block07\Real\1812-R-1459.mp4",
"Block07\Pseudo\1813-P-1459.mp4",
"Block06\Real\1816-R-2.mp4",
"Block06\Pseudo\1817-P-2.mp4",
"Block09\Pseudo\1818-P-17985.mp4",
"Block09\Real\1819-R-17985.mp4",
"Block05\Real\1824-R-7307.mp4",
"Block05\Pseudo\1825-P-7307.mp4",
"Block010\Pseudo\1826-P-6244.mp4",
"Block010\Real\1827-R-6244.mp4",
"Block04\Real\1828-R-7844.mp4",
"Block04\Pseudo\1829-P-7844.mp4",
"Block06\Pseudo\1830-P-507.mp4",
"Block06\Real\1831-R-507.mp4",
"Block09\Real\1832-R-6752.mp4",
"Block09\Pseudo\1833-P-6752.mp4",
"Block06\Pseudo\1834-P-16011.mp4",
"Block06\Real\1835-R-16011.mp4",
"Block09\Pseudo\1838-P-10954.mp4",
"Block09\Real\1839-R-10954.mp4",
"Block01\Real\1840-R-3270.mp4",
"Block01\Pseudo\1841-P-3270.mp4",
"Block05\Pseudo\1842-P-4187.mp4",
"Block05\Real\1843-R-4187.mp4",
"Block04\Real\1844-R-6537.mp4",
"Block04\Pseudo\1845-P-6537.mp4",
"Block04\Pseudo\1846-P-1424.mp4",
"Block04\Real\1847-R-1424.mp4",
"Block09\Real\1848-R-2745.mp4",
"Block09\Pseudo\1849-P-2745.mp4",
"Block02\Pseudo\1850-P-16572.mp4",
"Block02\Real\1851-R-16572.mp4",
"Block07\Pseudo\1854-P-4897.mp4",
"Block07\Real\1855-R-4897.mp4",
"Block08\Real\1856-R-1890.mp4",
"Block08\Pseudo\1857-P-1890.mp4",
"Block06\Pseudo\1858-P-17652.mp4",
"Block06\Real\1859-R-17652.mp4",
"Block07\Real\1860-R-4593.mp4",
"Block07\Pseudo\1861-P-4593.mp4",
"Block06\Pseudo\1862-P-8190.mp4",
"Block06\Real\1863-R-8190.mp4",
"Block04\Real\1864-R-6290.mp4",
"Block04\Pseudo\1865-P-6290.mp4",
"Block03\Pseudo\1866-P-2965.mp4",
"Block03\Real\1867-R-2965.mp4",
"Block07\Real\1868-R-339.mp4",
"Block07\Pseudo\1869-P-339.mp4",
"Block06\Pseudo\1870-P-2420.mp4",
"Block06\Real\1871-R-2420.mp4",
"Block08\Real\1872-R-5121.mp4",
"Block08\Pseudo\1873-P-5121.mp4",
"Block01\Real\1876-R-674.mp4",
"Block01\Pseudo\1877-P-674.mp4",
"Block07\Pseudo\1886-P-11053.mp4",
"Block07\Real\1887-R-11053.mp4",
"Block03\Real\1888-R-3540.mp4",
"Block03\Pseudo\1889-P-3540.mp4",
"Block05\Pseudo\1890-P-17173.mp4",
"Block05\Real\1891-R-17173.mp4",
"Block010\Pseudo\1894-P-1501.mp4",
"Block010\Real\1895-R-1501.mp4",
"Block08\Real\1896-R-18553.mp4",
"Block08\Pseudo\1897-P-18553.mp4",
"Block02\Pseudo\1898-P-80.mp4",
"Block02\Real\1899-R-80.mp4",
"Block03\Real\1900-R-610.mp4",
"Block03\Pseudo\1901-P-610.mp4",
"Block010\Pseudo\1906-P-1651.mp4",
"Block010\Real\1907-R-1651.mp4",
"Block08\Real\1908-R-12110.mp4",
"Block08\Pseudo\1909-P-12110.mp4",
"Block02\Real\1916-R-16446.mp4",
"Block02\Pseudo\1917-P-16446.mp4",
"Block07\Pseudo\1918-P-9214.mp4",
"Block07\Real\1919-R-9214.mp4",
"Block05\Real\1920-R-15968.mp4",
"Block05\Pseudo\1921-P-15968.mp4",
"Block09\Pseudo\1922-P-19878.mp4",
"Block09\Real\1923-R-19878.mp4",
"Block04\Real\1924-R-3060.mp4",
"Block04\Pseudo\1925-P-3060.mp4",
"Block010\Pseudo\1926-P-6593.mp4",
"Block010\Real\1927-R-6593.mp4",
"Block03\Real\1928-R-3327.mp4",
"Block03\Pseudo\1929-P-3327.mp4",
"Block07\Pseudo\1930-P-5887.mp4",
"Block07\Real\1931-R-5887.mp4",
"Block01\Pseudo\1934-P-9330.mp4",
"Block01\Real\1935-R-9330.mp4",
"Block08\Real\1936-R-3442.mp4",
"Block08\Pseudo\1937-P-3442.mp4",
"Block04\Pseudo\1938-P-4066.mp4",
"Block04\Real\1939-R-4066.mp4",
"Block010\Real\1940-R-2941.mp4",
"Block010\Pseudo\1941-P-2941.mp4",
"Block01\Pseudo\1942-P-7615.mp4",
"Block01\Real\1943-R-7615.mp4",
"Block08\Pseudo\1946-P-988.mp4",
"Block08\Real\1947-R-988.mp4",
"Block06\Real\1948-R-2519.mp4",
"Block06\Pseudo\1949-P-2519.mp4",
"Block010\Pseudo\1950-P-6653.mp4",
"Block010\Real\1951-R-6653.mp4",
"Block07\Real\1952-R-20064.mp4",
"Block07\Pseudo\1953-P-20064.mp4",
"Block03\Pseudo\1958-P-2117.mp4",
"Block03\Real\1959-R-2117.mp4",
"Block02\Real\1960-R-5099.mp4",
"Block02\Pseudo\1961-P-5099.mp4",
"Block03\Pseudo\1962-P-17605.mp4",
"Block03\Real\1963-R-17605.mp4",
"Block05\Real\1964-R-456.mp4",
"Block05\Pseudo\1965-P-456.mp4",
"Block01\Pseudo\1966-P-12754.mp4",
"Block01\Real\1967-R-12754.mp4",
"Block01\Real\1968-R-7191.mp4",
"Block01\Pseudo\1969-P-7191.mp4",
"Block010\Real\1972-R-4151.mp4",
"Block010\Pseudo\1973-P-4151.mp4",
"Block010\Pseudo\1974-P-5008.mp4",
"Block010\Real\1975-R-5008.mp4",
"Block08\Pseudo\1978-P-4219.mp4",
"Block08\Real\1979-R-4219.mp4",
"Block08\Real\1980-R-3238.mp4",
"Block08\Pseudo\1981-P-3238.mp4",
"Block03\Pseudo\1982-P-12649.mp4",
"Block03\Real\1983-R-12649.mp4",
"Block02\Real\1984-R-574.mp4",
"Block02\Pseudo\1985-P-574.mp4",
"Block05\Pseudo\1986-P-5504.mp4",
"Block05\Real\1987-R-5504.mp4",
"Block07\Pseudo\1990-P-820.mp4",
"Block07\Real\1991-R-820.mp4",
"Block010\Real\1992-R-19848.mp4",
"Block010\Pseudo\1993-P-19848.mp4",
"Block02\Pseudo\1994-P-3445.mp4",
"Block02\Real\1995-R-3445.mp4",
"Block010\Real\1996-R-1946.mp4",
"Block010\Pseudo\1997-P-1946.mp4",
"Block05\Pseudo\1998-P-13681.mp4",
"Block05\Real\1999-R-13681.mp4",
"Block09\Real\2000-R-6495.mp4",
"Block09\Pseudo\2001-P-6495.mp4",
"Block01\Pseudo\2002-P-14712.mp4",
"Block01\Real\2003-R-14712.mp4",
"Block02\Real\2004-R-17401.mp4",
"Block02\Pseudo\2005-P-17401.mp4",
"Block05\Pseudo\2006-P-10686.mp4",
"Block05\Real\2007-R-10686.mp4",
"Block04\Real\2008-R-242.mp4",
"Block04\Pseudo\2009-P-242.mp4",
"Block02\Pseudo\2010-P-5817.mp4",
"Block02\Real\2011-R-5817.mp4",
"Block09\Real\2012-R-12083.mp4",
"Block09\Pseudo\2013-P-12083.mp4",
"Block03\Pseudo\2014-P-8451.mp4",
"Block03\Real\2015-R-8451.mp4",
"Block06\Real\2016-R-15715.mp4",
"Block06\Pseudo\2017-P-15715.mp4",
"Block02\Pseudo\2018-P-20054.mp4",
"Block02\Real\2019-R-20054.mp4",
"Block01\Pseudo\2022-P-3410.mp4",
"Block01\Real\2023-R-3410.mp4",
"Block09\Real\2024-R-5076.mp4",
"Block09\Pseudo\2025-P-5076.mp4",
"Block06\Pseudo\2026-P-193.mp4",
"Block06\Real\2027-R-193.mp4",
"Block08\Real\2028-R-2433.mp4",
"Block08\Pseudo\2029-P-2433.mp4",
"Block01\Pseudo\2030-P-14671.mp4",
"Block01\Real\2031-R-14671.mp4",
"Block04\Pseudo\2038-P-1598.mp4",
"Block04\Real\2039-R-1598.mp4"}

        Dim rnd As New Random(42)

        Dim Check As Boolean = False
        Dim IncorrectList As New List(Of String)
        For Each FilePath In OutputPaths

            Dim FullInputPath = IO.Path.Combine("C:\VLDT\SharpVideosCropped", IO.Path.GetFileName(FilePath))

            Dim FullOutputPath = IO.Path.Combine("C:\VLDT\Pilot1", FilePath)
            Dim OutputDirectory = IO.Path.GetDirectoryName(FullOutputPath)
            If IO.Directory.Exists(OutputDirectory) = False Then IO.Directory.CreateDirectory(OutputDirectory)

            If Check = True Then
                If IO.File.Exists(FullInputPath) = False Then
                    IncorrectList.Add(FullInputPath)
                End If
            Else
                IO.File.Copy(FullInputPath, FullOutputPath)
            End If


        Next

        If Check = True Then
            Console.WriteLine(String.Join(vbCrLf, IncorrectList))
        End If

        MsgBox("Finished")

    End Sub

    Private Sub Button16_Click3(sender As Object, e As EventArgs) 'Handles MoveFilesButton.Click

        Dim OutputPaths As New List(Of String) From {"PractiseBlock\Real\1164-R-7360.mp4",
"PractiseBlock\Pseudo\1165-P-7360.mp4",
"PractiseBlock\Pseudo\1278-P-224.mp4",
"PractiseBlock\Real\1279-R-224.mp4",
"PractiseBlock\Real\1292-R-8619.mp4",
"PractiseBlock\Pseudo\1293-P-8619.mp4",
"PractiseBlock\Pseudo\1294-P-16512.mp4",
"PractiseBlock\Real\1295-R-16512.mp4",
"PractiseBlock\Pseudo\1522-P-4026.mp4",
"PractiseBlock\Real\1523-R-4026.mp4",
"PractiseBlock\Real\1544-R-10957.mp4",
"PractiseBlock\Pseudo\1545-P-10957.mp4",
"PractiseBlock\Pseudo\1546-P-13169.mp4",
"PractiseBlock\Real\1547-R-13169.mp4",
"PractiseBlock\Pseudo\1802-P-8092.mp4",
"PractiseBlock\Real\1803-R-8092.mp4",
"PractiseBlock\Pseudo\1814-P-16160.mp4",
"PractiseBlock\Real\1815-R-16160.mp4",
"PractiseBlock\Real\1820-R-4417.mp4",
"PractiseBlock\Pseudo\1821-P-4417.mp4"}

        Dim rnd As New Random(42)

        Dim Check As Boolean = False
        Dim IncorrectList As New List(Of String)
        For Each FilePath In OutputPaths

            Dim FullInputPath = IO.Path.Combine("C:\VLDT\SharpVideosCropped", IO.Path.GetFileName(FilePath))

            Dim FullOutputPath = IO.Path.Combine("C:\VLDT\Pilot1", FilePath)
            Dim OutputDirectory = IO.Path.GetDirectoryName(FullOutputPath)
            If IO.Directory.Exists(OutputDirectory) = False Then IO.Directory.CreateDirectory(OutputDirectory)

            If Check = True Then
                If IO.File.Exists(FullInputPath) = False Then
                    IncorrectList.Add(FullInputPath)
                End If
            Else
                IO.File.Copy(FullInputPath, FullOutputPath)
            End If


        Next

        If Check = True Then
            Console.WriteLine(String.Join(vbCrLf, IncorrectList))
        End If

        MsgBox("Finished")

    End Sub

    Private Sub Button16_Click4(sender As Object, e As EventArgs) Handles MoveFilesButton.Click

        Dim OutputPaths As New List(Of String) From {"1269-P-5743.mp4",
"1270-P-10038.mp4",
"1314-P-15829.mp4",
"1325-P-16236.mp4",
"1326-P-2108.mp4",
"1333-P-27.mp4",
"1346-P-6616.mp4",
"1357-P-7137.mp4",
"1369-P-11900.mp4",
"1378-P-371.mp4",
"1417-P-19469.mp4",
"1422-P-4291.mp4",
"1425-P-14522.mp4",
"1469-P-18870.mp4",
"1473-P-1967.mp4",
"1537-P-12416.mp4",
"1553-P-1341.mp4",
"1561-P-5708.mp4",
"1589-P-12261.mp4",
"1598-P-3627.mp4",
"1601-P-2039.mp4",
"1605-P-19152.mp4",
"1610-P-8986.mp4",
"1630-P-8820.mp4",
"1633-P-20095.mp4",
"1649-P-16338.mp4",
"1653-P-3837.mp4",
"1681-P-3014.mp4",
"1689-P-4481.mp4",
"1710-P-8480.mp4",
"1281-P-16254.mp4",
"1290-P-2533.mp4",
"1317-P-7446.mp4",
"1338-P-5236.mp4",
"1377-P-9982.mp4",
"1393-P-4789.mp4",
"1405-P-15724.mp4",
"1426-P-12573.mp4",
"1434-P-17227.mp4",
"1445-P-8364.mp4",
"1458-P-228.mp4",
"1489-P-17022.mp4",
"1493-P-10997.mp4",
"1509-P-406.mp4",
"1517-P-18766.mp4",
"1525-P-5559.mp4",
"1533-P-6188.mp4",
"1557-P-10489.mp4",
"1570-P-9137.mp4",
"1577-P-10785.mp4",
"1609-P-14235.mp4",
"1617-P-9101.mp4",
"1634-P-13888.mp4",
"1678-P-13559.mp4",
"1682-P-20142.mp4",
"1706-P-12685.mp4",
"1717-P-1787.mp4",
"1726-P-11572.mp4",
"1729-P-354.mp4",
"1733-P-17085.mp4"}

        Dim rnd As New Random(42)

        Dim Check As Boolean = False
        Dim IncorrectList As New List(Of String)
        For Each FilePath In OutputPaths

            Dim FullInputPath = IO.Path.Combine("C:\VLDT\SharpVideosCropped", IO.Path.GetFileName(FilePath))

            Dim FullOutputPath = IO.Path.Combine("C:\VLDT\Pilot1\VideosForRating", FilePath)
            Dim OutputDirectory = IO.Path.GetDirectoryName(FullOutputPath)
            If IO.Directory.Exists(OutputDirectory) = False Then IO.Directory.CreateDirectory(OutputDirectory)

            If Check = True Then
                If IO.File.Exists(FullInputPath) = False Then
                    IncorrectList.Add(FullInputPath)
                End If
            Else
                IO.File.Copy(FullInputPath, FullOutputPath)
            End If


        Next

        If Check = True Then
            Console.WriteLine(String.Join(vbCrLf, IncorrectList))
        End If

        MsgBox("Finished")

    End Sub


    Private Sub Button16_Click2(sender As Object, e As EventArgs) 'Handles Button16.Click

        Dim AllSignsList As New List(Of String) From {"1000-R-16294.mp4",
"1001-P-16294.mp4",
"1002-P-10864.mp4",
"1003-R-10864.mp4",
"1004-R-2817.mp4",
"1005-P-2817.mp4",
"1006-P-754.mp4",
"1007-R-754.mp4",
"1008-R-7696.mp4",
"1009-P-7696.mp4",
"1010-P-17137.mp4",
"1011-R-17137.mp4",
"1012-R-3194.mp4",
"1013-P-3194.mp4",
"1014-P-19912.mp4",
"1015-R-19912.mp4",
"1016-R-3044.mp4",
"1017-P-3044.mp4",
"1018-P-8859.mp4",
"1019-R-8859.mp4",
"1020-R-10083.mp4",
"1021-P-10083.mp4",
"1022-P-10571.mp4",
"1023-R-10571.mp4",
"1024-R-11199.mp4",
"1025-P-11199.mp4",
"1026-P-2301.mp4",
"1027-R-2301.mp4",
"1028-R-8179.mp4",
"1029-P-8179.mp4",
"1030-P-12357.mp4",
"1031-R-12357.mp4",
"1032-R-2531.mp4",
"1033-P-2531.mp4",
"1034-P-1679.mp4",
"1035-R-1679.mp4",
"1036-R-19474.mp4",
"1037-P-19474.mp4",
"1038-P-15655.mp4",
"1039-R-15655.mp4",
"1040-R-2266.mp4",
"1041-P-2266.mp4",
"1042-P-13079.mp4",
"1043-R-13079.mp4",
"1044-R-13970.mp4",
"1045-P-13970.mp4",
"1046-P-5015.mp4",
"1047-R-5015.mp4",
"1048-R-6277.mp4",
"1049-P-6277.mp4",
"1050-P-1242.mp4",
"1051-R-1242.mp4",
"1052-R-11754.mp4",
"1053-P-11754.mp4",
"1054-P-1696.mp4",
"1055-R-1696.mp4",
"1056-R-5538.mp4",
"1057-P-5538.mp4",
"1058-P-13916.mp4",
"1059-R-13916.mp4",
"1060-R-16061.mp4",
"1061-P-16061.mp4",
"1062-P-1070.mp4",
"1063-R-1070.mp4",
"1064-R-8640.mp4",
"1065-P-8640.mp4",
"1066-P-4401.mp4",
"1067-R-4401.mp4",
"1068-R-19372.mp4",
"1069-P-19372.mp4",
"1070-P-6315.mp4",
"1071-R-6315.mp4",
"1072-R-2966.mp4",
"1073-P-2966.mp4",
"1074-P-9779.mp4",
"1075-R-9779.mp4",
"1076-R-6388.mp4",
"1077-P-6388.mp4",
"1078-P-3340.mp4",
"1079-R-3340.mp4",
"1080-R-1355.mp4",
"1081-P-1355.mp4",
"1082-P-17031.mp4",
"1083-R-17031.mp4",
"1084-R-5596.mp4",
"1085-P-5596.mp4",
"1086-P-14767.mp4",
"1087-R-14767.mp4",
"1088-R-19611.mp4",
"1089-P-19611.mp4",
"1090-P-4686.mp4",
"1091-R-4686.mp4",
"1092-R-14542.mp4",
"1093-P-14542.mp4",
"1094-P-14792.mp4",
"1095-R-14792.mp4",
"1096-R-128.mp4",
"1097-P-128.mp4",
"1098-P-7466.mp4",
"1099-R-7466.mp4",
"1100-R-5295.mp4",
"1101-P-5295.mp4",
"1102-P-19835.mp4",
"1103-R-19835.mp4",
"1104-R-4072.mp4",
"1105-P-4072.mp4",
"1106-P-14125.mp4",
"1107-R-14125.mp4",
"1108-R-1092.mp4",
"1109-P-1092.mp4",
"1110-P-10564.mp4",
"1111-R-10564.mp4",
"1112-R-14594.mp4",
"1113-P-14594.mp4",
"1114-P-18933.mp4",
"1115-R-18933.mp4",
"1116-R-7940.mp4",
"1117-P-7940.mp4",
"1118-P-7502.mp4",
"1119-R-7502.mp4",
"1120-R-14502.mp4",
"1121-P-14502.mp4",
"1122-P-589.mp4",
"1123-R-589.mp4",
"1124-R-12610.mp4",
"1125-P-12610.mp4",
"1126-P-2246.mp4",
"1127-R-2246.mp4",
"1128-R-832.mp4",
"1129-P-832.mp4",
"1130-P-12469.mp4",
"1131-R-12469.mp4",
"1132-R-7498.mp4",
"1133-P-7498.mp4",
"1134-P-17763.mp4",
"1135-R-17763.mp4",
"1136-R-18824.mp4",
"1137-P-18824.mp4",
"1138-P-11085.mp4",
"1139-R-11085.mp4",
"1140-R-12929.mp4",
"1141-P-12929.mp4",
"1142-P-18687.mp4",
"1143-R-18687.mp4",
"1144-R-12177.mp4",
"1145-P-12177.mp4",
"1146-P-11502.mp4",
"1147-R-11502.mp4",
"1148-R-7716.mp4",
"1149-P-7716.mp4",
"1150-P-6806.mp4",
"1151-R-6806.mp4",
"1152-R-18952.mp4",
"1153-P-18952.mp4",
"1154-P-13028.mp4",
"1155-R-13028.mp4",
"1156-R-1118.mp4",
"1157-P-1118.mp4",
"1158-P-363.mp4",
"1159-R-363.mp4",
"1160-R-2749.mp4",
"1161-P-2749.mp4",
"1162-P-1583.mp4",
"1163-R-1583.mp4",
"1164-R-7360.mp4",
"1165-P-7360.mp4",
"1166-P-1410.mp4",
"1167-R-1410.mp4",
"1168-R-15993.mp4",
"1169-P-15993.mp4",
"1170-P-20112.mp4",
"1171-R-20112.mp4",
"1172-R-17810.mp4",
"1173-P-17810.mp4",
"1174-P-1575.mp4",
"1175-R-1575.mp4",
"1176-R-3691.mp4",
"1177-P-3691.mp4",
"1178-P-2162.mp4",
"1179-R-2162.mp4",
"1180-R-6920.mp4",
"1181-P-6920.mp4",
"1182-P-1713.mp4",
"1183-R-1713.mp4",
"1184-R-4928.mp4",
"1185-P-4928.mp4",
"1186-P-1237.mp4",
"1187-R-1237.mp4",
"1188-R-2423.mp4",
"1189-P-2423.mp4",
"1190-P-19855.mp4",
"1191-R-19855.mp4",
"1192-R-17333.mp4",
"1193-P-17333.mp4",
"1194-P-5670.mp4",
"1195-R-5670.mp4",
"1196-R-11833.mp4",
"1197-P-11833.mp4",
"1198-P-18972.mp4",
"1199-R-18972.mp4",
"1200-R-1399.mp4",
"1201-P-1399.mp4",
"1202-P-18930.mp4",
"1203-R-18930.mp4",
"1204-R-4372.mp4",
"1205-P-4372.mp4",
"1206-P-12909.mp4",
"1207-R-12909.mp4",
"1208-R-18984.mp4",
"1209-P-18984.mp4",
"1210-P-17660.mp4",
"1211-R-17660.mp4",
"1212-R-875.mp4",
"1213-P-875.mp4",
"1214-P-20055.mp4",
"1215-R-20055.mp4",
"1216-R-14276.mp4",
"1217-P-14276.mp4",
"1218-P-17288.mp4",
"1219-R-17288.mp4",
"1220-R-2025.mp4",
"1221-P-2025.mp4",
"1222-P-3104.mp4",
"1223-R-3104.mp4",
"1224-R-18752.mp4",
"1225-P-18752.mp4",
"1226-P-1401.mp4",
"1227-R-1401.mp4",
"1228-R-10684.mp4",
"1229-P-10684.mp4",
"1230-P-2073.mp4",
"1231-R-2073.mp4",
"1232-R-19991.mp4",
"1233-P-19991.mp4",
"1234-P-912.mp4",
"1235-R-912.mp4",
"1236-R-2694.mp4",
"1237-P-2694.mp4",
"1238-P-1238.mp4",
"1239-R-1238.mp4",
"1240-R-14984.mp4",
"1241-P-14984.mp4",
"1242-P-2149.mp4",
"1243-R-2149.mp4",
"1244-R-11625.mp4",
"1245-P-11625.mp4",
"1246-P-3506.mp4",
"1247-R-3506.mp4",
"1248-R-1377.mp4",
"1249-P-1377.mp4",
"1250-P-18600.mp4",
"1251-R-18600.mp4",
"1252-R-9073.mp4",
"1253-P-9073.mp4",
"1254-P-1404.mp4",
"1255-R-1404.mp4",
"1256-R-677.mp4",
"1257-P-677.mp4",
"1258-P-7005.mp4",
"1259-R-7005.mp4",
"1260-R-12457.mp4",
"1261-P-12457.mp4",
"1262-P-2389.mp4",
"1263-R-2389.mp4",
"1264-R-10457.mp4",
"1265-P-10457.mp4",
"1266-P-2813.mp4",
"1267-R-2813.mp4",
"1268-R-5743.mp4",
"1269-P-5743.mp4",
"1270-P-10038.mp4",
"1271-R-10038.mp4",
"1272-R-17488.mp4",
"1273-P-17488.mp4",
"1274-P-6441.mp4",
"1275-R-6441.mp4",
"1276-R-1056.mp4",
"1277-P-1056.mp4",
"1278-P-224.mp4",
"1279-R-224.mp4",
"1280-R-16254.mp4",
"1281-P-16254.mp4",
"1282-P-12274.mp4",
"1283-R-12274.mp4",
"1284-R-6912.mp4",
"1285-P-6912.mp4",
"1286-P-6230.mp4",
"1287-R-6230.mp4",
"1288-R-3370.mp4",
"1289-P-3370.mp4",
"1290-P-2533.mp4",
"1291-R-2533.mp4",
"1292-R-8619.mp4",
"1293-P-8619.mp4",
"1294-P-16512.mp4",
"1295-R-16512.mp4",
"1296-R-13485.mp4",
"1297-P-13485.mp4",
"1298-P-1022.mp4",
"1299-R-1022.mp4",
"1300-R-10423.mp4",
"1301-P-10423.mp4",
"1302-P-6333.mp4",
"1303-R-6333.mp4",
"1304-R-8162.mp4",
"1305-P-8162.mp4",
"1306-P-2839.mp4",
"1307-R-2839.mp4",
"1308-R-11129.mp4",
"1309-P-11129.mp4",
"1310-P-19581.mp4",
"1311-R-19581.mp4",
"1312-R-2255.mp4",
"1313-P-2255.mp4",
"1314-P-15829.mp4",
"1315-R-15829.mp4",
"1316-R-7446.mp4",
"1317-P-7446.mp4",
"1318-P-14812.mp4",
"1319-R-14812.mp4",
"1320-R-17778.mp4",
"1321-P-17778.mp4",
"1322-P-16600.mp4",
"1323-R-16600.mp4",
"1324-R-16236.mp4",
"1325-P-16236.mp4",
"1326-P-2108.mp4",
"1327-R-2108.mp4",
"1328-R-1446.mp4",
"1329-P-1446.mp4",
"1330-P-298.mp4",
"1331-R-298.mp4",
"1332-R-27.mp4",
"1333-P-27.mp4",
"1334-P-17064.mp4",
"1335-R-17064.mp4",
"1336-R-3010.mp4",
"1337-P-3010.mp4",
"1338-P-5236.mp4",
"1339-R-5236.mp4",
"1340-R-10866.mp4",
"1341-P-10866.mp4",
"1342-P-4647.mp4",
"1343-R-4647.mp4",
"1344-R-4740.mp4",
"1345-P-4740.mp4",
"1346-P-6616.mp4",
"1347-R-6616.mp4",
"1348-R-2071.mp4",
"1349-P-2071.mp4",
"1350-P-6515.mp4",
"1351-R-6515.mp4",
"1352-R-4247.mp4",
"1353-P-4247.mp4",
"1354-P-7867.mp4",
"1355-R-7867.mp4",
"1356-R-7137.mp4",
"1357-P-7137.mp4",
"1358-P-5013.mp4",
"1359-R-5013.mp4",
"1360-R-14978.mp4",
"1361-P-14978.mp4",
"1362-P-13309.mp4",
"1363-R-13309.mp4",
"1364-R-7899.mp4",
"1365-P-7899.mp4",
"1366-P-12480.mp4",
"1367-R-12480.mp4",
"1368-R-11900.mp4",
"1369-P-11900.mp4",
"1370-P-5533.mp4",
"1371-R-5533.mp4",
"1372-R-207.mp4",
"1373-P-207.mp4",
"1374-P-15681.mp4",
"1375-R-15681.mp4",
"1376-R-9982.mp4",
"1377-P-9982.mp4",
"1378-P-371.mp4",
"1379-R-371.mp4",
"1380-R-17381.mp4",
"1381-P-17381.mp4",
"1382-P-764.mp4",
"1383-R-764.mp4",
"1384-R-1660.mp4",
"1385-P-1660.mp4",
"1386-P-187.mp4",
"1387-R-187.mp4",
"1388-R-3118.mp4",
"1389-P-3118.mp4",
"1390-P-17441.mp4",
"1391-R-17441.mp4",
"1392-R-4789.mp4",
"1393-P-4789.mp4",
"1394-P-1487.mp4",
"1395-R-1487.mp4",
"1396-R-3351.mp4",
"1397-P-3351.mp4",
"1398-P-3804.mp4",
"1399-R-3804.mp4",
"1400-R-10720.mp4",
"1401-P-10720.mp4",
"1402-P-11898.mp4",
"1403-R-11898.mp4",
"1404-R-15724.mp4",
"1405-P-15724.mp4",
"1406-P-3523.mp4",
"1407-R-3523.mp4",
"1408-R-4907.mp4",
"1409-P-4907.mp4",
"1410-P-11880.mp4",
"1411-R-11880.mp4",
"1412-R-3331.mp4",
"1413-P-3331.mp4",
"1414-P-4567.mp4",
"1415-R-4567.mp4",
"1416-R-19469.mp4",
"1417-P-19469.mp4",
"1418-P-1455.mp4",
"1419-R-1455.mp4",
"1420-R-4764.mp4",
"1421-P-4764.mp4",
"1422-P-4291.mp4",
"1423-R-4291.mp4",
"1424-R-14522.mp4",
"1425-P-14522.mp4",
"1426-P-12573.mp4",
"1427-R-12573.mp4",
"1428-R-3539.mp4",
"1429-P-3539.mp4",
"1430-P-14419.mp4",
"1431-R-14419.mp4",
"1432-R-4496.mp4",
"1433-P-4496.mp4",
"1434-P-17227.mp4",
"1435-R-17227.mp4",
"1436-R-262.mp4",
"1437-P-262.mp4",
"1438-P-4118.mp4",
"1439-R-4118.mp4",
"1440-R-9143.mp4",
"1441-P-9143.mp4",
"1442-P-3239.mp4",
"1443-R-3239.mp4",
"1444-R-8364.mp4",
"1445-P-8364.mp4",
"1446-P-15782.mp4",
"1447-R-15782.mp4",
"1448-R-14422.mp4",
"1449-P-14422.mp4",
"1450-P-8230.mp4",
"1451-R-8230.mp4",
"1452-R-9989.mp4",
"1453-P-9989.mp4",
"1454-P-2680.mp4",
"1455-R-2680.mp4",
"1456-R-11827.mp4",
"1457-P-11827.mp4",
"1458-P-228.mp4",
"1459-R-228.mp4",
"1460-R-4043.mp4",
"1461-P-4043.mp4",
"1462-P-247.mp4",
"1463-R-247.mp4",
"1464-R-3052.mp4",
"1465-P-3052.mp4",
"1466-P-1076.mp4",
"1467-R-1076.mp4",
"1468-R-18870.mp4",
"1469-P-18870.mp4",
"1470-P-4123.mp4",
"1471-R-4123.mp4",
"1472-R-1967.mp4",
"1473-P-1967.mp4",
"1474-P-3486.mp4",
"1475-R-3486.mp4",
"1476-R-11603.mp4",
"1477-P-11603.mp4",
"1478-P-12707.mp4",
"1479-R-12707.mp4",
"1480-R-1249.mp4",
"1481-P-1249.mp4",
"1482-P-17389.mp4",
"1483-R-17389.mp4",
"1484-R-690.mp4",
"1485-P-690.mp4",
"1486-P-2041.mp4",
"1487-R-2041.mp4",
"1488-R-17022.mp4",
"1489-P-17022.mp4",
"1490-P-3.mp4",
"1491-R-3.mp4",
"1492-R-10997.mp4",
"1493-P-10997.mp4",
"1494-P-13951.mp4",
"1495-R-13951.mp4",
"1496-R-6871.mp4",
"1497-P-6871.mp4",
"1498-P-5426.mp4",
"1499-R-5426.mp4",
"1500-R-51.mp4",
"1501-P-51.mp4",
"1502-P-5483.mp4",
"1503-R-5483.mp4",
"1504-R-647.mp4",
"1505-P-647.mp4",
"1506-P-2616.mp4",
"1507-R-2616.mp4",
"1508-R-406.mp4",
"1509-P-406.mp4",
"1510-P-6019.mp4",
"1511-R-6019.mp4",
"1512-R-2277.mp4",
"1513-P-2277.mp4",
"1514-P-12566.mp4",
"1515-R-12566.mp4",
"1516-R-18766.mp4",
"1517-P-18766.mp4",
"1518-P-14298.mp4",
"1519-R-14298.mp4",
"1520-R-19404.mp4",
"1521-P-19404.mp4",
"1522-P-4026.mp4",
"1523-R-4026.mp4",
"1524-R-5559.mp4",
"1525-P-5559.mp4",
"1526-P-14384.mp4",
"1527-R-14384.mp4",
"1528-R-3888.mp4",
"1529-P-3888.mp4",
"1530-P-1775.mp4",
"1531-R-1775.mp4",
"1532-R-6188.mp4",
"1533-P-6188.mp4",
"1534-P-6513.mp4",
"1535-R-6513.mp4",
"1536-R-12416.mp4",
"1537-P-12416.mp4",
"1538-P-9940.mp4",
"1539-R-9940.mp4",
"1540-R-464.mp4",
"1541-P-464.mp4",
"1542-P-4489.mp4",
"1543-R-4489.mp4",
"1544-R-10957.mp4",
"1545-P-10957.mp4",
"1546-P-13169.mp4",
"1547-R-13169.mp4",
"1548-R-1373.mp4",
"1549-P-1373.mp4",
"1550-P-2508.mp4",
"1551-R-2508.mp4",
"1552-R-1341.mp4",
"1553-P-1341.mp4",
"1554-P-1379.mp4",
"1555-R-1379.mp4",
"1556-R-10489.mp4",
"1557-P-10489.mp4",
"1558-P-5016.mp4",
"1559-R-5016.mp4",
"1560-R-5708.mp4",
"1561-P-5708.mp4",
"1562-P-9781.mp4",
"1563-R-9781.mp4",
"1564-R-15606.mp4",
"1565-P-15606.mp4",
"1566-P-5187.mp4",
"1567-R-5187.mp4",
"1568-R-3616.mp4",
"1569-P-3616.mp4",
"1570-P-9137.mp4",
"1571-R-9137.mp4",
"1572-R-14108.mp4",
"1573-P-14108.mp4",
"1574-P-2993.mp4",
"1575-R-2993.mp4",
"1576-R-10785.mp4",
"1577-P-10785.mp4",
"1578-P-15873.mp4",
"1579-R-15873.mp4",
"1580-R-304.mp4",
"1581-P-304.mp4",
"1582-P-3565.mp4",
"1583-R-3565.mp4",
"1584-R-2118.mp4",
"1585-P-2118.mp4",
"1586-P-10528.mp4",
"1587-R-10528.mp4",
"1588-R-12261.mp4",
"1589-P-12261.mp4",
"1590-P-2909.mp4",
"1591-R-2909.mp4",
"1592-R-7104.mp4",
"1593-P-7104.mp4",
"1594-P-11153.mp4",
"1595-R-11153.mp4",
"1596-R-1611.mp4",
"1597-P-1611.mp4",
"1598-P-3627.mp4",
"1599-R-3627.mp4",
"1600-R-2039.mp4",
"1601-P-2039.mp4",
"1602-P-19283.mp4",
"1603-R-19283.mp4",
"1604-R-19152.mp4",
"1605-P-19152.mp4",
"1606-P-14479.mp4",
"1607-R-14479.mp4",
"1608-R-14235.mp4",
"1609-P-14235.mp4",
"1610-P-8986.mp4",
"1611-R-8986.mp4",
"1612-R-1502.mp4",
"1613-P-1502.mp4",
"1614-P-3605.mp4",
"1615-R-3605.mp4",
"1616-R-9101.mp4",
"1617-P-9101.mp4",
"1618-P-4910.mp4",
"1619-R-4910.mp4",
"1620-R-5323.mp4",
"1621-P-5323.mp4",
"1622-P-3004.mp4",
"1623-R-3004.mp4",
"1624-R-3017.mp4",
"1625-P-3017.mp4",
"1626-P-15234.mp4",
"1627-R-15234.mp4",
"1628-R-1077.mp4",
"1629-P-1077.mp4",
"1630-P-8820.mp4",
"1631-R-8820.mp4",
"1632-R-20095.mp4",
"1633-P-20095.mp4",
"1634-P-13888.mp4",
"1635-R-13888.mp4",
"1636-R-872.mp4",
"1637-P-872.mp4",
"1638-P-4547.mp4",
"1639-R-4547.mp4",
"1640-R-6456.mp4",
"1641-P-6456.mp4",
"1642-P-2265.mp4",
"1643-R-2265.mp4",
"1644-R-15464.mp4",
"1645-P-15464.mp4",
"1646-P-18674.mp4",
"1647-R-18674.mp4",
"1648-R-16338.mp4",
"1649-P-16338.mp4",
"1650-P-11014.mp4",
"1651-R-11014.mp4",
"1652-R-3837.mp4",
"1653-P-3837.mp4",
"1654-P-11960.mp4",
"1655-R-11960.mp4",
"1656-R-2371.mp4",
"1657-P-2371.mp4",
"1658-P-10445.mp4",
"1659-R-10445.mp4",
"1660-R-349.mp4",
"1661-P-349.mp4",
"1662-P-17433.mp4",
"1663-R-17433.mp4",
"1664-R-5438.mp4",
"1665-P-5438.mp4",
"1666-P-8972.mp4",
"1667-R-8972.mp4",
"1668-R-10531.mp4",
"1669-P-10531.mp4",
"1670-P-4104.mp4",
"1671-R-4104.mp4",
"1672-R-8875.mp4",
"1673-P-8875.mp4",
"1674-P-18184.mp4",
"1675-R-18184.mp4",
"1676-R-14875.mp4",
"1677-P-14875.mp4",
"1678-P-13559.mp4",
"1679-R-13559.mp4",
"1680-R-3014.mp4",
"1681-P-3014.mp4",
"1682-P-20142.mp4",
"1683-R-20142.mp4",
"1684-R-18751.mp4",
"1685-P-18751.mp4",
"1686-P-7036.mp4",
"1687-R-7036.mp4",
"1688-R-4481.mp4",
"1689-P-4481.mp4",
"1690-P-1527.mp4",
"1691-R-1527.mp4",
"1692-R-1319.mp4",
"1693-P-1319.mp4",
"1694-P-2304.mp4",
"1695-R-2304.mp4",
"1696-R-13969.mp4",
"1697-P-13969.mp4",
"1698-P-6247.mp4",
"1699-R-6247.mp4",
"1700-R-59.mp4",
"1701-P-59.mp4",
"1702-P-1060.mp4",
"1703-R-1060.mp4",
"1704-R-2177.mp4",
"1705-P-2177.mp4",
"1706-P-12685.mp4",
"1707-R-12685.mp4",
"1708-R-19342.mp4",
"1709-P-19342.mp4",
"1710-P-8480.mp4",
"1711-R-8480.mp4",
"1712-R-3515.mp4",
"1713-P-3515.mp4",
"1714-P-10039.mp4",
"1715-R-10039.mp4",
"1716-R-1787.mp4",
"1717-P-1787.mp4",
"1718-P-3946.mp4",
"1719-R-3946.mp4",
"1720-R-2890.mp4",
"1721-P-2890.mp4",
"1722-P-16184.mp4",
"1723-R-16184.mp4",
"1724-R-1945.mp4",
"1725-P-1945.mp4",
"1726-P-11572.mp4",
"1727-R-11572.mp4",
"1728-R-354.mp4",
"1729-P-354.mp4",
"1730-P-1191.mp4",
"1731-R-1191.mp4",
"1732-R-17085.mp4",
"1733-P-17085.mp4",
"1734-P-7087.mp4",
"1735-R-7087.mp4",
"1736-R-1212.mp4",
"1737-P-1212.mp4",
"1738-P-1683.mp4",
"1739-R-1683.mp4",
"1740-R-3051.mp4",
"1741-P-3051.mp4",
"1742-P-4830.mp4",
"1743-R-4830.mp4",
"1744-R-609.mp4",
"1745-P-609.mp4",
"1746-P-18440.mp4",
"1747-R-18440.mp4",
"1748-R-19897.mp4",
"1749-P-19897.mp4",
"1750-P-796.mp4",
"1751-R-796.mp4",
"1752-R-3156.mp4",
"1753-P-3156.mp4",
"1754-P-428.mp4",
"1755-R-428.mp4",
"1756-R-3284.mp4",
"1757-P-3284.mp4",
"1758-P-3954.mp4",
"1759-R-3954.mp4",
"1760-R-1409.mp4",
"1761-P-1409.mp4",
"1762-P-10288.mp4",
"1763-R-10288.mp4",
"1764-R-14576.mp4",
"1765-P-14576.mp4",
"1766-P-1578.mp4",
"1767-R-1578.mp4",
"1768-R-2253.mp4",
"1769-P-2253.mp4",
"1770-P-18541.mp4",
"1771-R-18541.mp4",
"1772-R-11818.mp4",
"1773-P-11818.mp4",
"1774-P-6263.mp4",
"1775-R-6263.mp4",
"1776-R-19280.mp4",
"1777-P-19280.mp4",
"1778-P-1722.mp4",
"1779-R-1722.mp4",
"1780-R-2707.mp4",
"1781-P-2707.mp4",
"1782-P-5445.mp4",
"1783-R-5445.mp4",
"1784-R-6523.mp4",
"1785-P-6523.mp4",
"1786-P-2256.mp4",
"1787-R-2256.mp4",
"1788-R-15732.mp4",
"1789-P-15732.mp4",
"1790-P-12020.mp4",
"1791-R-12020.mp4",
"1792-R-9136.mp4",
"1793-P-9136.mp4",
"1794-P-6078.mp4",
"1795-R-6078.mp4",
"1796-R-17355.mp4",
"1797-P-17355.mp4",
"1798-P-15778.mp4",
"1799-R-15778.mp4",
"1800-R-1684.mp4",
"1801-P-1684.mp4",
"1802-P-8092.mp4",
"1803-R-8092.mp4",
"1804-R-13755.mp4",
"1805-P-13755.mp4",
"1806-P-3513.mp4",
"1807-R-3513.mp4",
"1808-R-7464.mp4",
"1809-P-7464.mp4",
"1810-P-5328.mp4",
"1811-R-5328.mp4",
"1812-R-1459.mp4",
"1813-P-1459.mp4",
"1814-P-16160.mp4",
"1815-R-16160.mp4",
"1816-R-2.mp4",
"1817-P-2.mp4",
"1818-P-17985.mp4",
"1819-R-17985.mp4",
"1820-R-4417.mp4",
"1821-P-4417.mp4",
"1822-P-16524.mp4",
"1823-R-16524.mp4",
"1824-R-7307.mp4",
"1825-P-7307.mp4",
"1826-P-6244.mp4",
"1827-R-6244.mp4",
"1828-R-7844.mp4",
"1829-P-7844.mp4",
"1830-P-507.mp4",
"1831-R-507.mp4",
"1832-R-6752.mp4",
"1833-P-6752.mp4",
"1834-P-16011.mp4",
"1835-R-16011.mp4",
"1836-R-1868.mp4",
"1837-P-1868.mp4",
"1838-P-10954.mp4",
"1839-R-10954.mp4",
"1840-R-3270.mp4",
"1841-P-3270.mp4",
"1842-P-4187.mp4",
"1843-R-4187.mp4",
"1844-R-6537.mp4",
"1845-P-6537.mp4",
"1846-P-1424.mp4",
"1847-R-1424.mp4",
"1848-R-2745.mp4",
"1849-P-2745.mp4",
"1850-P-16572.mp4",
"1851-R-16572.mp4",
"1852-R-1625.mp4",
"1853-P-1625.mp4",
"1854-P-4897.mp4",
"1855-R-4897.mp4",
"1856-R-1890.mp4",
"1857-P-1890.mp4",
"1858-P-17652.mp4",
"1859-R-17652.mp4",
"1860-R-4593.mp4",
"1861-P-4593.mp4",
"1862-P-8190.mp4",
"1863-R-8190.mp4",
"1864-R-6290.mp4",
"1865-P-6290.mp4",
"1866-P-2965.mp4",
"1867-R-2965.mp4",
"1868-R-339.mp4",
"1869-P-339.mp4",
"1870-P-2420.mp4",
"1871-R-2420.mp4",
"1872-R-5121.mp4",
"1873-P-5121.mp4",
"1874-P-15531.mp4",
"1875-R-15531.mp4",
"1876-R-674.mp4",
"1877-P-674.mp4",
"1878-P-11087.mp4",
"1879-R-11087.mp4",
"1880-R-4290.mp4",
"1881-P-4290.mp4",
"1882-P-9787.mp4",
"1883-R-9787.mp4",
"1884-R-1879.mp4",
"1885-P-1879.mp4",
"1886-P-11053.mp4",
"1887-R-11053.mp4",
"1888-R-3540.mp4",
"1889-P-3540.mp4",
"1890-P-17173.mp4",
"1891-R-17173.mp4",
"1892-R-8916.mp4",
"1893-P-8916.mp4",
"1894-P-1501.mp4",
"1895-R-1501.mp4",
"1896-R-18553.mp4",
"1897-P-18553.mp4",
"1898-P-80.mp4",
"1899-R-80.mp4",
"1900-R-610.mp4",
"1901-P-610.mp4",
"1902-P-12595.mp4",
"1903-R-12595.mp4",
"1904-R-1849.mp4",
"1905-P-1849.mp4",
"1906-P-1651.mp4",
"1907-R-1651.mp4",
"1908-R-12110.mp4",
"1909-P-12110.mp4",
"1910-P-4286.mp4",
"1911-R-4286.mp4",
"1912-R-5139.mp4",
"1913-P-5139.mp4",
"1914-P-17661.mp4",
"1915-R-17661.mp4",
"1916-R-16446.mp4",
"1917-P-16446.mp4",
"1918-P-9214.mp4",
"1919-R-9214.mp4",
"1920-R-15968.mp4",
"1921-P-15968.mp4",
"1922-P-19878.mp4",
"1923-R-19878.mp4",
"1924-R-3060.mp4",
"1925-P-3060.mp4",
"1926-P-6593.mp4",
"1927-R-6593.mp4",
"1928-R-3327.mp4",
"1929-P-3327.mp4",
"1930-P-5887.mp4",
"1931-R-5887.mp4",
"1932-R-17728.mp4",
"1933-P-17728.mp4",
"1934-P-9330.mp4",
"1935-R-9330.mp4",
"1936-R-3442.mp4",
"1937-P-3442.mp4",
"1938-P-4066.mp4",
"1939-R-4066.mp4",
"1940-R-2941.mp4",
"1941-P-2941.mp4",
"1942-P-7615.mp4",
"1943-R-7615.mp4",
"1944-R-1634.mp4",
"1945-P-1634.mp4",
"1946-P-988.mp4",
"1947-R-988.mp4",
"1948-R-2519.mp4",
"1949-P-2519.mp4",
"1950-P-6653.mp4",
"1951-R-6653.mp4",
"1952-R-20064.mp4",
"1953-P-20064.mp4",
"1954-P-2185.mp4",
"1955-R-2185.mp4",
"1956-R-1970.mp4",
"1957-P-1970.mp4",
"1958-P-2117.mp4",
"1959-R-2117.mp4",
"1960-R-5099.mp4",
"1961-P-5099.mp4",
"1962-P-17605.mp4",
"1963-R-17605.mp4",
"1964-R-456.mp4",
"1965-P-456.mp4",
"1966-P-12754.mp4",
"1967-R-12754.mp4",
"1968-R-7191.mp4",
"1969-P-7191.mp4",
"1970-P-4034.mp4",
"1971-R-4034.mp4",
"1972-R-4151.mp4",
"1973-P-4151.mp4",
"1974-P-5008.mp4",
"1975-R-5008.mp4",
"1976-R-2615.mp4",
"1977-P-2615.mp4",
"1978-P-4219.mp4",
"1979-R-4219.mp4",
"1980-R-3238.mp4",
"1981-P-3238.mp4",
"1982-P-12649.mp4",
"1983-R-12649.mp4",
"1984-R-574.mp4",
"1985-P-574.mp4",
"1986-P-5504.mp4",
"1987-R-5504.mp4",
"1988-R-2443.mp4",
"1989-P-2443.mp4",
"1990-P-820.mp4",
"1991-R-820.mp4",
"1992-R-19848.mp4",
"1993-P-19848.mp4",
"1994-P-3445.mp4",
"1995-R-3445.mp4",
"1996-R-1946.mp4",
"1997-P-1946.mp4",
"1998-P-13681.mp4",
"1999-R-13681.mp4",
"2000-R-6495.mp4",
"2001-P-6495.mp4",
"2002-P-14712.mp4",
"2003-R-14712.mp4",
"2004-R-17401.mp4",
"2005-P-17401.mp4",
"2006-P-10686.mp4",
"2007-R-10686.mp4",
"2008-R-242.mp4",
"2009-P-242.mp4",
"2010-P-5817.mp4",
"2011-R-5817.mp4",
"2012-R-12083.mp4",
"2013-P-12083.mp4",
"2014-P-8451.mp4",
"2015-R-8451.mp4",
"2016-R-15715.mp4",
"2017-P-15715.mp4",
"2018-P-20054.mp4",
"2019-R-20054.mp4",
"2020-R-1436.mp4",
"2021-P-1436.mp4",
"2022-P-3410.mp4",
"2023-R-3410.mp4",
"2024-R-5076.mp4",
"2025-P-5076.mp4",
"2026-P-193.mp4",
"2027-R-193.mp4",
"2028-R-2433.mp4",
"2029-P-2433.mp4",
"2030-P-14671.mp4",
"2031-R-14671.mp4",
"2032-R-12169.mp4",
"2033-P-12169.mp4",
"2034-P-7683.mp4",
"2035-R-7683.mp4",
"2036-R-4803.mp4",
"2037-P-4803.mp4",
"2038-P-1598.mp4",
"2039-R-1598.mp4"}

        Dim rnd As New Random(42)

        Dim IncorrectList As New List(Of String)

        Dim FilePathList As New List(Of String)
        FilePathList.AddRange(IO.Directory.GetFiles("C:\VLDT\SharpVideos\Actor 1").ToList)
        FilePathList.AddRange(IO.Directory.GetFiles("C:\VLDT\SharpVideos\Actor 2").ToList)
        FilePathList.AddRange(IO.Directory.GetFiles("C:\VLDT\SharpVideos\Actor 3").ToList)
        FilePathList.AddRange(IO.Directory.GetFiles("C:\VLDT\SharpVideos\Actor 4").ToList)

        For Each FilePath In FilePathList
            If AllSignsList.Contains(IO.Path.GetFileName(FilePath)) = False Then
                IncorrectList.Add(IO.Path.GetFileName(FilePath))
            End If
        Next

        Console.WriteLine(String.Join(vbCrLf, IncorrectList))

        MsgBox("Finished")

    End Sub


    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click

        Dim InputFolder = "C:\VLDT\SharpVideos\Actor 4"

        Dim Files = IO.Directory.GetFiles(InputFolder)

        For Each File In Files

            If File.Contains("_") Then
                System.IO.File.Move(File, File.Replace("_", "-"))
            End If

        Next

        MsgBox("Finished")

    End Sub

    Private Sub HLSIM_Button_Click(sender As Object, e As EventArgs) Handles HLSIM_Button.Click

        Dim InputSound = SpeechTestFramework.Audio.AudioIOs.LoadWaveFile("C:\Temp\L00_Lista_1_001.wav")
        'Dim InputSound = SpeechTestFramework.Audio.AudioIOs.LoadWaveFile("C:\Temp\Chirp1.wav")
        'Dim InputSound = SpeechTestFramework.Audio.AudioIOs.LoadWaveFile("C:\Temp\WN_93_70_40.wav")

        'Dim InputSound = SpeechTestFramework.Audio.GenerateSound.CreateWhiteNoise(New SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, 2,, SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints),, 1, 5)
        'Dim InputSound = SpeechTestFramework.Audio.GenerateSound.CreateSineWave(New SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, 2,, SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints),, 1000,,, 5)

        SpeechTestFramework.Audio.DSP.MeasureAndAdjustSectionLevel(InputSound, SpeechTestFramework.Audio.Standard_dBSPL_To_dBFS(65))

        'Dim FftFormat = New SpeechTestFramework.Audio.Formats.FftFormat(4 * 2048,, 1024, SpeechTestFramework.Audio.WindowingType.Hamming, False)
        'Dim TDL = SpeechTestFramework.Audio.DSP.MeasureSectionLevel(InputSound, 1)
        'Dim FDL = SpeechTestFramework.Audio.DSP.CalculateSoundLevelFromFrequencyDomain(InputSound, 1, FftFormat)
        'Dim FDLS = SpeechTestFramework.Audio.DSP.CalculateBandLevels(InputSound, 1)


        InputSound.WriteWaveFile("C:\Temp\WN_85.wav")

        Dim SimulatedAudiogram As New SpeechTestFramework.AudiogramData
        SimulatedAudiogram.CreateTypicalAudiogramData(SpeechTestFramework.AudiogramData.BisgaardAudiograms.N1)
        'SimulatedAudiogram.CreateDebuggingAudiogramData(SpeechTestFramework.AudiogramData.DebuggingAudiograms.PlusFivePerFrequency)

        Dim ListenerAudiogram As New SpeechTestFramework.AudiogramData
        ListenerAudiogram.CreateTypicalAudiogramData(SpeechTestFramework.AudiogramData.BisgaardAudiograms.NH)


        Dim HLSIM = New SpeechTestFramework.Audio.HearinglossSimulator_CB(SimulatedAudiogram, ListenerAudiogram)

        HLSIM.Simulate(InputSound)

        Dim OutputSound = HLSIM.SimulatedSound
        Dim ResponseL = HLSIM.GetAverageResponse(SpeechTestFramework.Utils.Sides.Left)
        Dim ResponseR = HLSIM.GetAverageResponse(SpeechTestFramework.Utils.Sides.Right)

        'OutputSound.WriteWaveFile("C:\Temp\L00_Lista_1_000_HL.wav")
        'OutputSound.WriteWaveFile("C:\Temp\Chirp1_HL.wav")
        'OutputSound.WriteWaveFile("C:\Temp\WN_93_70_40_HL.wav")
        OutputSound.WriteWaveFile("C:\Temp\WN_85_HL.wav")

    End Sub
End Class