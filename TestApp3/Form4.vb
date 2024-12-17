Imports System.Diagnostics.Eventing
Imports System.Runtime.InteropServices
Imports SpeechTestFramework
Imports SpeechTestFramework.OstfBase
Imports SpeechTestFramework.SipTest
Imports SpeechTestFramework.Audio.SoundScene
Imports System
Imports System.Numerics
Imports MathNet.Numerics.LinearAlgebra
Public Class Form4

    Dim SoundPlayer As SpeechTestFramework.Audio.PortAudioVB.PortAudioBasedSoundPlayer

    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Initializing the OSTF 
        SpeechTestFramework.InitializeOSTF(Platforms.WinUI)

    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'ResponseGuiItemTable1.AdjustControls()


        Dim test As Integer = 0
        Select Case test
            Case -1

                SpeechTestFramework.OstfBase.LoadAvailableSpeechMaterialSpecifications()
                Dim SelectedTestIndex As Integer = 1
                SpeechTestFramework.OstfBase.AvailableSpeechMaterials(SelectedTestIndex).LoadSpeechMaterialComponentsFile()
                Dim MySpeechMaterial = SpeechTestFramework.OstfBase.AvailableSpeechMaterials(SelectedTestIndex).SpeechMaterial
                SpeechTestFramework.OstfBase.AvailableSpeechMaterials(SelectedTestIndex).LoadAvailableMediaSetSpecifications()
                Dim MyMediaSet = SpeechTestFramework.OstfBase.AvailableSpeechMaterials(SelectedTestIndex).MediaSets(0)

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

                SpeechTestFramework.OstfBase.LoadAvailableSpeechMaterialSpecifications()

                Dim SelectedTestIndex As Integer = 0

                SpeechTestFramework.OstfBase.AvailableSpeechMaterials(SelectedTestIndex).LoadSpeechMaterialComponentsFile()

                Dim CompleteSpeechMaterial = SpeechTestFramework.OstfBase.AvailableSpeechMaterials(SelectedTestIndex).SpeechMaterial

                'Dim CompleteSpeechMaterial = SpeechTestFramework.SpeechMaterialComponent.LoadSpeechMaterial(SpeechTestFramework.OstfBase.CurrentlySelectedTest.SpeechMaterialComponentsSubFilePath)

                'CompleteSpeechMaterial.WriteSpeechMaterialComponenFile(SpeechTestFramework.Utils.logFilePath & "TestSMC.txt")

                Dim NewMediaSet = New SpeechTestFramework.MediaSet
                'NewMediaSet.SetSipValues(1)
                'NewMediaSet.SetHintDebugValues()

                'NewMediaSet.CopySoundFiles(CompleteSpeechMaterial, IO.Path.Combine(SpeechTestFramework.Utils.logFilePath, "MediaSet2"))

                NewMediaSet.RecordAndEditAudioMediaFiles(SpeechTestFramework.MediaSet.SpeechMaterialRecorderLoadOptions.LoadAllSounds, True, SpeechTestFramework.MediaSet.PrototypeRecordingOptions.None)

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

                SoundPlayer = New SpeechTestFramework.Audio.PortAudioVB.PortAudioBasedSoundPlayer(Nothing)

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

    Private Sub Audiogram1_MouseHover(sender As Object, e As EventArgs)

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

        Dim ItemList = New List(Of SoundSceneItem)

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

        ItemList.Add(New SoundSceneItem(Sound_Background1, 1, 60, 1, New SoundSourceLocation With {.HorizontalAzimuth = -45}, SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech, 0,,,, FadeSpecs_Background, DuckSpecs))
        ItemList.Add(New SoundSceneItem(Sound_Background2, 1, 60, 1, New SoundSourceLocation With {.HorizontalAzimuth = 45}, SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech, 0,,,, FadeSpecs_Background, DuckSpecs))
        ItemList.Add(New SoundSceneItem(Sound_Background3, 1, 60, 1, New SoundSourceLocation With {.HorizontalAzimuth = -135}, SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech, 0,,,, FadeSpecs_Background, DuckSpecs))
        ItemList.Add(New SoundSceneItem(Sound_Background4, 1, 60, 1, New SoundSourceLocation With {.HorizontalAzimuth = 135}, SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech, 0,,,, FadeSpecs_Background, DuckSpecs))
        ItemList.Add(New SoundSceneItem(Sound_TestWord, 1, 70, 2, New SoundSourceLocation With {.HorizontalAzimuth = 15}, SoundSceneItem.SoundSceneItemRoles.Target, 48000 * 2,,,, FadeSpecs_Speech))
        ItemList.Add(New SoundSceneItem(Sound_Masker1, 1, 65, 3, New SoundSourceLocation With {.HorizontalAzimuth = 120}, SoundSceneItem.SoundSceneItemRoles.Masker, 48000,,,, FadeSpecs_Maskers))
        ItemList.Add(New SoundSceneItem(Sound_Masker2, 1, 65, 3, New SoundSourceLocation With {.HorizontalAzimuth = 130}, SoundSceneItem.SoundSceneItemRoles.Masker, 48000,,,, FadeSpecs_Maskers))

        Dim MyMixer = New SpeechTestFramework.Audio.SoundScene.DuplexMixer()
        'MyMixer.SetLinearOutput()
        MyMixer.HardwareOutputChannelSpeakerLocations.Add(1, New SoundSourceLocation With {.HorizontalAzimuth = -30})
        MyMixer.HardwareOutputChannelSpeakerLocations.Add(2, New SoundSourceLocation With {.HorizontalAzimuth = 0})
        MyMixer.HardwareOutputChannelSpeakerLocations.Add(3, New SoundSourceLocation With {.HorizontalAzimuth = 30})

        Dim OutputSound = MyMixer.CreateSoundScene(ItemList, SpeechTestFramework.OstfBase.SoundPropagationTypes.SimulatedSoundField)

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


        SpeechTestFramework.SoundPlayer.ChangePlayerSettings(, OutputSound.WaveFormat.SampleRate, OutputSound.WaveFormat.BitDepth, OutputSound.WaveFormat.Encoding,,, Audio.SoundPlayers.iSoundPlayer.SoundDirections.PlaybackOnly, False, False)
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

    Private Sub TestWordLabel1_Click(sender As Object, e As EventArgs)

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

    Private Sub CreateCalib_Button_Click(sender As Object, e As EventArgs) Handles CreateCalib_Button.Click

        Dim SignalLevel As Double = -25

        Dim InputSound = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\Temp\Calib\S_F1_000.wav")

        Dim CalibSound = SpeechTestFramework.Audio.GenerateSound.CreateFrequencyModulatedSineWave(InputSound.WaveFormat, 1, 1000, 1, 20, 0.125, , 30)


        'Setting the signal level
        SpeechTestFramework.Audio.DSP.MeasureAndAdjustSectionLevel(CalibSound, SignalLevel)

        'Fading in and out
        SpeechTestFramework.Audio.DSP.Fade(CalibSound, Nothing, 0,,, 0.05 * CalibSound.WaveFormat.SampleRate, SpeechTestFramework.Audio.DSP.FadeSlopeType.Smooth)
        SpeechTestFramework.Audio.DSP.Fade(CalibSound, 0, Nothing,, CalibSound.WaveData.ShortestChannelSampleCount - 0.05 * CalibSound.WaveFormat.SampleRate, Nothing, SpeechTestFramework.Audio.DSP.FadeSlopeType.Smooth)

        CalibSound.WriteWaveFile("C:\Temp\Calib\Calibration.wav")

        Dim CalibSoundStereo = New SpeechTestFramework.Audio.Sound(New SpeechTestFramework.Audio.Formats.WaveFormat(CalibSound.WaveFormat.SampleRate, CalibSound.WaveFormat.BitDepth, 2,, CalibSound.WaveFormat.Encoding))
        CalibSoundStereo.WaveData.SampleData(1) = CalibSound.WaveData.SampleData(1)
        CalibSoundStereo.WaveData.SampleData(2) = CalibSound.WaveData.SampleData(1)

        CalibSoundStereo.WriteWaveFile("C:\Temp\Calib\Calibration_Stereo.wav")

    End Sub

    Private Sub Button16_Click_1(sender As Object, e As EventArgs) Handles Button16.Click

        Dim [Step] As Integer = 1

        Select Case [Step]
            Case 1
                'Step 1
                Dim MeasurementSignal = SpeechTestFramework.Audio.GenerateSound.CreateIRMeasurementSignal(New SpeechTestFramework.Audio.Formats.WaveFormat(48000, 32, 1),,,,,, )

                SpeechTestFramework.Audio.AudioIOs.SaveToWaveFile(MeasurementSignal.Item1,,,,,, "MeasurementSignal_B")
                SpeechTestFramework.Audio.AudioIOs.SaveToWaveFile(MeasurementSignal.Item2,,,,,, "ProcessingSignal_B")

            Case 2
                'Step 2
                'Dim OriginalSweep = SpeechTestFramework.Audio.AudioIOs.ReadWaveFile("C:\EriksDokument\TestPhonemeMaskers\TestEnvironments\IR_Kitchen\ProcessingSignal.wav")
                Dim OriginalSweep = SpeechTestFramework.Audio.AudioIOs.ReadWaveFile("C:\EriksDokument\TestPhonemeMaskers\TestEnvironments\IR_Kitchen\ProcessingSignal_B.wav")
                Dim RecordedKitchenSweepSignal = SpeechTestFramework.Audio.AudioIOs.ReadWaveFile("C:\EriksDokument\TestPhonemeMaskers\TestEnvironments\IR_Kitchen\Kitchen_Sweep_Recording.wav")
                RecordedKitchenSweepSignal = SpeechTestFramework.Audio.GenerateSound.PostProcessMeasuredIRSweep(RecordedKitchenSweepSignal,,,,,,,,,,, True, True)
                Dim Impulse = SpeechTestFramework.Audio.DSP.ConvertSineSweepToImpulse(OriginalSweep, RecordedKitchenSweepSignal, , 4, True)
                SpeechTestFramework.Audio.AudioIOs.SaveToWaveFile(Impulse,,,,, "Kitchen_IR")

        End Select


    End Sub

    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        SpeechTestFramework.ImpulseResponseCustomFunctions.MeasureIrGain("C:\EriksDokument\source\repos\OSTF\OSTFMedia\RoomImpulses\ARC_Harcellen_KEMAR\48000Hz\UnspecifiedHeadphones_Long\KEMAR_0_L.wav",
                                                                         "C:\EriksDokument\source\repos\OSTF\OSTFMedia\RoomImpulses\ARC_Harcellen_KEMAR\48000Hz\UnspecifiedHeadphones_Long\Test")
    End Sub

    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click

        Dim Paths = SpeechTestFramework.Utils.GetFilesIncludingAllSubdirectories("C:\SwedishSiBTest\SoundFiles\Stad\TWRB")

        SpeechTestFramework.Utils.SendInfoToLog(String.Join(vbCrLf, Paths), "Subpaths", "C:\Temp")


    End Sub

    Private Sub Button20_Click(sender As Object, e As EventArgs) 'Handles Button20.Click

        Dim InputSound = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSiPTest\Media\Unechoic-Talker1-RVE\TestWordRecordings\L02S03_blund\M_000_000_blund.wav")
        'InputSound.ZeroPad(2.0R, 2.0R)
        InputSound.WriteWaveFile("C:\Temp5\OrigSound.wav")
        Dim IR = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\RoomImpulses\ARC_Harcellen_KEMAR\48000Hz\UnspecifiedHeadphones\KEMAR_60_R.wav")
        IR.RemoveUnparsedWaveChunks()
        IR.WriteWaveFile("C:\Temp5\IR.wav")

        Dim ConvSound = SpeechTestFramework.Audio.DSP.FIRFilter(InputSound, IR, New SpeechTestFramework.Audio.Formats.FftFormat)
        ConvSound.WriteWaveFile("C:\Temp5\ConvSound.wav")

        'Dim RevIR = SpeechTestFramework.Audio.DSP.ReverseSound(IR)
        'RevIR.RemoveUnparsedWaveChunks()
        'RevIR.WriteWaveFile("C:\Temp5\RevIR.wav")

        'ConvSound = SpeechTestFramework.Audio.DSP.ReverseSound(ConvSound)

        'Dim DeConvSound2 = SpeechTestFramework.Audio.DSP.Deconvolution2(ConvSound, IR, New SpeechTestFramework.Audio.Formats.FftFormat)
        'DeConvSound2 = SpeechTestFramework.Audio.DSP.ReverseSound(DeConvSound)
        'DeConvSound2.WriteWaveFile("C:\Temp5\DeConvSound2.wav")

        Dim DeConvSound = SpeechTestFramework.Audio.DSP.Deconvolution(ConvSound, IR)
        'DeConvSound = SpeechTestFramework.Audio.DSP.ReverseSound(DeConvSound)
        DeConvSound.WriteWaveFile("C:\Temp5\DeConvSound.wav")



    End Sub


    Private Sub Button20B_Click(sender As Object, e As EventArgs) 'Handles Button20.Click

        Dim Directions As New List(Of Integer)
        For d As Integer = -180 To 150 Step 30
            Directions.Add(d)
        Next

        Dim InputSound = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSiPTest\Media\Unechoic-Talker1-RVE\TestWordRecordings\L02S03_blund\M_000_000_blund.wav")
        InputSound.WriteWaveFile("C:\Temp5\OriginalSound.wav")

        Dim HpIR = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\Temp5\IR_Hårcellen_48000_HATS_AKGK271_best_L.wav")
        'IR.ZeroPad(1.0R, 1.0R)

        For Each Direction In Directions

            Dim InputSound_L = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\Temp5\BuK_ECEbl_" & Direction & "_L.wav")
            Dim InputSound_R = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\Temp5\BuK_ECEbl_" & Direction & "_R.wav")

            Dim ConvSound_L1 = SpeechTestFramework.Audio.DSP.FIRFilter(InputSound, InputSound_L, New SpeechTestFramework.Audio.Formats.FftFormat, ,,,,, True)
            Dim ConvSound_R1 = SpeechTestFramework.Audio.DSP.FIRFilter(InputSound, InputSound_R, New SpeechTestFramework.Audio.Formats.FftFormat, ,,,,, True)
            Dim OutputSound1 = New SpeechTestFramework.Audio.Sound(New SpeechTestFramework.Audio.Formats.WaveFormat(
                                                              ConvSound_L1.WaveFormat.SampleRate, ConvSound_L1.WaveFormat.BitDepth, 2))
            OutputSound1.WaveData.SampleData(1) = ConvSound_L1.WaveData.SampleData(1)
            OutputSound1.WaveData.SampleData(2) = ConvSound_R1.WaveData.SampleData(1)
            OutputSound1.WriteWaveFile("C:\Temp5\UncorrectedSound_" & Direction & ".wav")

            InputSound_L.ZeroPad(1.0R, 1.0R)
            InputSound_R.ZeroPad(1.0R, 1.0R)

            'InputSound_L.RemoveUnparsedWaveChunks()
            'InputSound_R.RemoveUnparsedWaveChunks()

            'InputSound_L.WriteWaveFile("C:\Temp5\OrigSound_L.wav")
            'InputSound_R.WriteWaveFile("C:\Temp5\OrigSound_R.wav")

            Dim DeConvSound_L = SpeechTestFramework.Audio.DSP.Deconvolution(InputSound_L, HpIR,,, 100, 22000, True)
            Dim DeConvSound_R = SpeechTestFramework.Audio.DSP.Deconvolution(InputSound_R, HpIR,,, 100, 22000, True)

            SpeechTestFramework.Audio.DSP.CropSection(DeConvSound_L, 0.9 * DeConvSound_L.WaveFormat.SampleRate, 1 * DeConvSound_L.WaveFormat.SampleRate)
            SpeechTestFramework.Audio.DSP.CropSection(DeConvSound_R, 0.9 * DeConvSound_R.WaveFormat.SampleRate, 1 * DeConvSound_L.WaveFormat.SampleRate)

            SpeechTestFramework.Audio.DSP.Fade(DeConvSound_L, Nothing, 0,,, 100)
            SpeechTestFramework.Audio.DSP.Fade(DeConvSound_R, Nothing, 0,,, 100)

            DeConvSound_L.WriteWaveFile("C:\Temp5\AKGK271\BuK_ECEbl_" & Direction & "_L_AKG.wav")
            DeConvSound_R.WriteWaveFile("C:\Temp5\AKGK271\BuK_ECEbl_" & Direction & "_R_AKG.wav")


            Dim ConvSound_L2 = SpeechTestFramework.Audio.DSP.FIRFilter(InputSound, DeConvSound_L, New SpeechTestFramework.Audio.Formats.FftFormat, ,,,,, True)
            Dim ConvSound_R2 = SpeechTestFramework.Audio.DSP.FIRFilter(InputSound, DeConvSound_R, New SpeechTestFramework.Audio.Formats.FftFormat, ,,,,, True)

            Dim OutputSound2 = New SpeechTestFramework.Audio.Sound(New SpeechTestFramework.Audio.Formats.WaveFormat(
                                                              ConvSound_L2.WaveFormat.SampleRate, ConvSound_L2.WaveFormat.BitDepth, 2))
            OutputSound2.WaveData.SampleData(1) = ConvSound_L2.WaveData.SampleData(1)
            OutputSound2.WaveData.SampleData(2) = ConvSound_R2.WaveData.SampleData(1)
            OutputSound2.WriteWaveFile("C:\Temp5\CorrectedSound_" & Direction & ".wav")

        Next


    End Sub

    Private Sub Button20C_Click(sender As Object, e As EventArgs) Handles Button20.Click

        Dim InputSound = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSiPTest\Media\Unechoic-Talker1-RVE\TestWordRecordings\L02S03_blund\M_000_000_blund.wav")

        'Dim IrFftFormat As New SpeechTestFramework.Audio.Formats.FftFormat(4096,,, SpeechTestFramework.Audio.WindowingType.Hamming, False)

        'Dim CorrectionKernel = SpeechTestFramework.Audio.GenerateSound.GetImpulseResponseForFrequencyResponseFlattening(InputSound,,,,, IrFftFormat,,, 70, 20, 100, 10000)

        'CorrectionKernel.WriteWaveFile("C:\Temp5\CorrectionKernel.wav")

        'Exit Sub

        Dim FrontalGainsList As New List(Of String)
        Dim AverageGainsList As New List(Of String)
        Dim DS = SpeechTestFramework.DirectionalSimulator
        Dim DSS = DS.GetAllDirectionalSimulationSets
        For Each KVP In DSS
            If KVP.Value.SampleRate <> 48000 Then Continue For
            Dim Results = KVP.Value.CalculateFrontalIrGains(True)
            For Each Result In Results
                FrontalGainsList.Add(Result.Item1 & ": " & Result.Item2)
            Next

            'AverageGainsList.Add(KVP.Key & "_" & KVP.Value.CalculateAverageIrGain(True))

        Next

        SpeechTestFramework.Utils.SendInfoToLog("FrontalGainsList" & vbCrLf & String.Join(vbCrLf, FrontalGainsList), "IrGainList")
        SpeechTestFramework.Utils.SendInfoToLog("AverageGainsList" & vbCrLf & String.Join(vbCrLf, AverageGainsList), "IrGainList")

        MsgBox(String.Join(vbCrLf, FrontalGainsList))

        Exit Sub

        Dim Direction As Integer = 0

        Dim IR = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\Temp5\BuK_ECEbl_" & Direction & "_L.wav")

        Dim Duration As Double = 10

        'Dim InputSound = SpeechTestFramework.Audio.GenerateSound.CreateWhiteNoise(IR.WaveFormat,,, Duration)

        Dim PreLevel As Double = -30

        SpeechTestFramework.Audio.DSP.MeasureAndAdjustSectionLevel(InputSound, PreLevel)

        Dim ConvSound = SpeechTestFramework.Audio.DSP.FIRFilter(InputSound, IR, New SpeechTestFramework.Audio.Formats.FftFormat)

        Dim PostLevel = SpeechTestFramework.Audio.DSP.MeasureSectionLevel(ConvSound, 1, ConvSound.WaveFormat.SampleRate * 1, ConvSound.WaveFormat.SampleRate * (Duration - 2))

        Dim FilterGain = PostLevel - PreLevel

        InputSound.WriteWaveFile("C:\Temp5\Noise.wav")
        ConvSound.WriteWaveFile("C:\Temp5\NoiseFiltered.wav")

        MsgBox("FilterGain: " & FilterGain)

    End Sub

    Private Sub Button21_Click(sender As Object, e As EventArgs) 'Handles Button21.Click


        'Initializing all components
        OstfBase.LoadAvailableSpeechMaterialSpecifications()

        Dim SpeechMaterialName = "Swedish SiP-test"

        Dim SelectedTest As SpeechMaterialSpecification = Nothing
        For Each ts In OstfBase.AvailableSpeechMaterials
            If ts.Name = SpeechMaterialName Then
                SelectedTest = ts
                Exit For
            End If
        Next

        Dim SpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(SelectedTest.GetSpeechMaterialFilePath, SelectedTest.GetTestRootPath)
        SpeechMaterial.ParentTestSpecification = SelectedTest
        SelectedTest.SpeechMaterial = SpeechMaterial

        'Loading media sets
        SpeechMaterial.ParentTestSpecification.LoadAvailableMediaSetSpecifications()
        Dim AvailableMediaSets = SpeechMaterial.ParentTestSpecification.MediaSets
        Dim SelectedMediaSet = AvailableMediaSets(0)

        Dim AllPhonemes = SpeechMaterial.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Phoneme)

        Dim OutputList As New List(Of String)
        For Each Phoneme In AllPhonemes

            If Phoneme.IsContrastingComponent = True Then
                For soundIndex = 0 To SelectedMediaSet.MediaAudioItems - 1
                    Dim SMA_Component = Phoneme.GetCorrespondingSmaComponent(SelectedMediaSet, soundIndex, 1, True)

                    Dim StartSample = SMA_Component(0).StartSample
                    Dim Length = SMA_Component(0).Length
                    OutputList.Add(Phoneme.ParentComponent.ParentComponent.Id & "R" & soundIndex & vbTab & Phoneme.Id & "R" & soundIndex & vbTab & StartSample & vbTab & Length & vbTab &
                                   SMA_Component(0).PhoneticForm & vbTab & Phoneme.ParentComponent.PrimaryStringRepresentation)

                Next
            End If

        Next

        SpeechTestFramework.Utils.SendInfoToLog(String.Join(vbCrLf, OutputList), "ExportedTestPhonemeTimes")

    End Sub


    Private Sub Button21B_Click(sender As Object, e As EventArgs) 'Handles Button21.Click


        'Initializing all components
        OstfBase.LoadAvailableSpeechMaterialSpecifications()

        Dim SpeechMaterialName = "Swedish SiP-test"

        Dim SelectedTest As SpeechMaterialSpecification = Nothing
        For Each ts In OstfBase.AvailableSpeechMaterials
            If ts.Name = SpeechMaterialName Then
                SelectedTest = ts
                Exit For
            End If
        Next

        Dim SpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(SelectedTest.GetSpeechMaterialFilePath, SelectedTest.GetTestRootPath)
        SpeechMaterial.ParentTestSpecification = SelectedTest
        SelectedTest.SpeechMaterial = SpeechMaterial

        'Loading media sets
        SpeechMaterial.ParentTestSpecification.LoadAvailableMediaSetSpecifications()
        Dim AvailableMediaSets = SpeechMaterial.ParentTestSpecification.MediaSets
        Dim SelectedMediaSet = AvailableMediaSets(0)

        'Dim ListSMAObject = SpeechMaterial.GetCorrespondingSmaComponent(SelectedMediaSet, 1, 1, False)

        Dim OutputSound = SpeechMaterial.GetSound(SelectedMediaSet, 1, 1)

        'Dim SoundList As New List(Of Audio.Sound)
        'For Each Sound In ListSMAObject
        '    Dim CurrentSound = Sound.GetSoundFileSection(1, False)
        '    SoundList.Add(CurrentSound)
        '    SoundList.Add(Audio.GenerateSound.CreateSilence(CurrentSound.WaveFormat))
        'Next

        'Dim OutputSound = SpeechTestFramework.Audio.DSP.ConcatenateSounds(SoundList)

        OutputSound.WriteWaveFile(IO.Path.Combine(SpeechTestFramework.Utils.logFilePath, "SiP_test_male_rec1s_B.wav"))

    End Sub


    Public Sub Testlibostfdsp()

        Dim TotL As Integer = 2 ^ 18

        Dim Input1 = SpeechTestFramework.Audio.GenerateSound.CreateWhiteNoise(New Audio.Formats.WaveFormat(48000, 32, 1),,, TotL, Audio.BasicAudioEnums.TimeUnits.samples)

        Dim s1 = Input1.WaveData.SampleData(1)
        Dim x1(s1.Length - 1) As Double
        Dim x2(s1.Length - 1) As Double
        For s = 0 To s1.Length - 1
            x1(s) = s1(s)
            x2(s) = s1(s)
        Next
        Dim y1(x1.Length - 1) As Double
        Dim y2(x2.Length - 1) As Double


        SpeechTestFramework.OstfBase.UseOptimizationLibraries = False

        Dim StopWatch1 As New Stopwatch
        StopWatch1.Start()
        SpeechTestFramework.Audio.DSP.FastFourierTransform(Audio.DSP.TransformationsExt.FftDirections.Forward, x1, y1)
        'Input1.FFT = SpeechTestFramework.Audio.DSP.SpectralAnalysis(Input1, FFT)
        Dim Time1 = StopWatch1.Elapsed

        SpeechTestFramework.OstfBase.UseOptimizationLibraries = True

        Dim StopWatch2 As New Stopwatch
        StopWatch2.Start()
        SpeechTestFramework.Audio.DSP.FastFourierTransform(Audio.DSP.TransformationsExt.FftDirections.Forward, x1, y1)
        'Input2.FFT = SpeechTestFramework.Audio.DSP.SpectralAnalysis(Input2, FFT)
        Dim Time2 = StopWatch2.Elapsed

        MsgBox(Time1.TotalMilliseconds & " " & Time2.TotalMilliseconds)


    End Sub


    Private Sub Testlibostfdsp_Button_Click(sender As Object, e As EventArgs) Handles Button21.Click

        Testlibostfdsp()

    End Sub

    Private Sub Button22_Click(sender As Object, e As EventArgs) Handles Button22.Click

        Dim WorkFolder As String = ""
        Dim WorkFolderSuffix As String = ""
        Dim AddSilence As Boolean = False

        Dim WordListFiles = {"sour", "mouse", "shirt", "turn", "keg", "hush", "thumb", "get", "bean", "wire", "fall", "match", "pike", "lore",
            "late", "dead", "rat", "page", "neat", "burn", "wash", "team", "said", "lid", "sell", "nice", "cab", "lose", "bone", "gin", "nag", "far",
            "jar", "tell", "kill", "reach", "mill", "dip", "shack", "yes", "vine", "pool", "should", "lean", "week", "limb", "tire", "sub", "pass", "sale",
            "lot", "shall", "phone", "chat", "search", "love", "shout", "gap", "rag", "join", "ton", "mob", "name", "goose", "learn", "ripe", "such",
            "vote", "note", "young", "doll", "third", "wheat", "king", "bite", "seize", "raid", "tough", "knock", "walk", "gas", "fail", "lease", "which",
            "youth", "size", "take", "keep", "mode", "long", "pearl", "thought", "beg", "numb", "hash", "luck", "chief", "near", "rot", "shawl"}

        Dim SoundList As New List(Of SpeechTestFramework.Audio.Sound)
        Dim SilentSound As Audio.Sound = Nothing
        For Each Filename In WordListFiles
            Dim CurrentWord = SpeechTestFramework.Audio.Sound.LoadWaveFile(IO.Path.Combine(WorkFolder, WorkFolderSuffix, Filename & ".wav"))
            If AddSilence = True Then If SilentSound Is Nothing Then SilentSound = Audio.GenerateSound.CreateSilence(CurrentWord.WaveFormat, 1, 1)
            If AddSilence = True Then SoundList.Add(SilentSound)
            SoundList.Add(CurrentWord)
        Next
        If AddSilence = True Then SoundList.Add(SilentSound)

        Dim ConcatenatedSound = SpeechTestFramework.Audio.DSP.ConcatenateSounds(SoundList)

        Dim Z_WeightedLevel = SpeechTestFramework.Audio.DSP.MeasureSectionLevel(ConcatenatedSound, 1)

        Dim C_WeightedLevel = SpeechTestFramework.Audio.DSP.MeasureSectionLevel(ConcatenatedSound, 1, ,,,, Audio.BasicAudioEnums.FrequencyWeightings.C)

        ConcatenatedSound.WriteWaveFile(WorkFolder & "NU6.wav")

        Dim WordListFiles_SiP = {”hy”, “hyf”, “hys”, “hyrs”, “arm”, “farm”, “charm”, “larm”, “yr”, “fyr”, “skyr”, “syr”, “å”, “få”, “sjå”, “så”, “all”, “hall”, “pall”, “tall”, “il”, “kil”, “fil”, “sil”, “ur”, “bur”, “dur”, “mur”}

        Dim SiPSoundList As New List(Of SpeechTestFramework.Audio.Sound)
        Dim SilentSound_SiP As Audio.Sound = Nothing
        Dim Check As Boolean = True
        For Each Filename In WordListFiles_SiP
            Dim CurrentWord As Audio.Sound = Nothing
            If Check = False Then
                CurrentWord = SpeechTestFramework.Audio.Sound.LoadWaveFile(IO.Path.Combine(WorkFolder & "SiP-A\Sounds_F", Filename & ".wav"))
            Else
                CurrentWord = SpeechTestFramework.Audio.Sound.LoadWaveFile(IO.Path.Combine(WorkFolder & "SiP-A\Sounds_F_Calib", Filename & ".wav"))
            End If

            Dim SMA = CurrentWord.SMA.ChannelData(1)(0)
            Dim Sound = SMA.GetSoundFileSection(1)
            If AddSilence = True Then If SilentSound_SiP Is Nothing Then SilentSound_SiP = Audio.GenerateSound.CreateSilence(Sound.WaveFormat, 1, 1)
            If AddSilence = True Then SiPSoundList.Add(SilentSound_SiP)
            SiPSoundList.Add(Sound)
        Next
        If AddSilence = True Then SiPSoundList.Add(SilentSound_SiP)

        Dim ConcatenatedSound_SiP = SpeechTestFramework.Audio.DSP.ConcatenateSounds(SiPSoundList)

        Dim Z_WeightedLevel_SiP = SpeechTestFramework.Audio.DSP.MeasureSectionLevel(ConcatenatedSound_SiP, 1)

        Dim C_WeightedLevel_SiP = SpeechTestFramework.Audio.DSP.MeasureSectionLevel(ConcatenatedSound_SiP, 1, ,,,, Audio.BasicAudioEnums.FrequencyWeightings.C)

        Dim C_WeightedDifference = C_WeightedLevel - C_WeightedLevel_SiP

        ConcatenatedSound_SiP.WriteWaveFile(WorkFolder & "SiP.wav")

        For Each Filename In WordListFiles_SiP
            Dim CurrentWord As Audio.Sound = Nothing
            If Check = False Then
                CurrentWord = SpeechTestFramework.Audio.Sound.LoadWaveFile(IO.Path.Combine(WorkFolder & "SiP-A\Sounds_F", Filename & ".wav"))
            Else
                CurrentWord = SpeechTestFramework.Audio.Sound.LoadWaveFile(IO.Path.Combine(WorkFolder & "SiP-A\Sounds_F_Calib", Filename & ".wav"))
            End If
            Audio.DSP.AmplifySection(CurrentWord, C_WeightedDifference)
            If Check = False Then
                CurrentWord.WriteWaveFile(IO.Path.Combine(WorkFolder & "SiP-A\Sounds_F_Calib", Filename & ".wav"))
            Else
                CurrentWord.SMA = Nothing
                CurrentWord.WriteWaveFile(IO.Path.Combine(WorkFolder & "SiP-A\Sounds_F_Calib_NoSMA", Filename & ".wav"))
            End If
        Next

        MsgBox("NU6: Z:" & Z_WeightedLevel & " C: " & C_WeightedLevel & vbCrLf &
               "SiP: Z:" & Z_WeightedLevel_SiP & " C: " & C_WeightedLevel_SiP)

    End Sub

    Private Sub Button23_Click(sender As Object, e As EventArgs) Handles Button23.Click

        Dim WorkFolder As String = ""

        Dim WordListFiles = {”hy”, “hyf”, “hys”, “hyrs”, “arm”, “farm”, “charm”, “larm”, “yr”, “fyr”, “skyr”, “syr”, “å”, “få”, “sjå”, “så”, “all”, “hall”, “pall”, “tall”, “il”, “kil”, “fil”, “sil”, “ur”, “bur”, “dur”, “mur”}

        Dim Prefix = "M_000_000_"

        For Each Filename In WordListFiles
            IO.File.Copy(IO.Path.Combine(WorkFolder, "SiP-A\Speaker 1 - Male voice", Prefix & Filename & ".wav"), IO.Path.Combine(WorkFolder, "SiP-A\Sounds_M", Filename & ".wav"))
        Next


    End Sub

    Private Sub Button24_Click(sender As Object, e As EventArgs) Handles Button24.Click

        'Initializing all components
        OstfBase.LoadAvailableSpeechMaterialSpecifications()

        Dim SpeechMaterialName = "Swedish SiP-test"

        Dim SelectedTest As SpeechMaterialSpecification = Nothing
        For Each ts In OstfBase.AvailableSpeechMaterials
            If ts.Name = SpeechMaterialName Then
                SelectedTest = ts
                Exit For
            End If
        Next

        Dim SpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(SelectedTest.GetSpeechMaterialFilePath, SelectedTest.GetTestRootPath)
        SpeechMaterial.ParentTestSpecification = SelectedTest
        SelectedTest.SpeechMaterial = SpeechMaterial

        'Loading media sets
        SpeechMaterial.ParentTestSpecification.LoadAvailableMediaSetSpecifications()
        Dim AvailableMediaSets = SpeechMaterial.ParentTestSpecification.MediaSets
        Dim SelectedMediaSet = AvailableMediaSets(1)

        Dim TWGs = SpeechMaterial.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.List)

        For Each TWG In TWGs

            'Getting concatenated sounds
            Dim ConcatenatedSound = TWG.GetConcatenatedComponentsSound(SelectedMediaSet, SpeechMaterialComponent.LinguisticLevels.Phoneme, True, 1, False, 0, 0, False, True)

            ConcatenatedSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "TWG_Noises", TWG.PrimaryStringRepresentation & ".wav"))

            Dim TWG_Noise = Audio.GenerateSound.GetSpectrallyModulatedNoiseFromFile(ConcatenatedSound, 1024 * 4,,, True)

            TWG_Noise.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "TWG_Noises", TWG.PrimaryStringRepresentation & "_LTASS.wav"))

        Next


    End Sub


    Private Sub Button25_Click_1(sender As Object, e As EventArgs) Handles Button25.Click

        Dim rnd = New Random

        'Initializing all components
        OstfBase.LoadAvailableSpeechMaterialSpecifications()

        Dim SpeechMaterialName = "Swedish SiP-test"

        Dim SelectedTest As SpeechMaterialSpecification = Nothing
        For Each ts In OstfBase.AvailableSpeechMaterials
            If ts.Name = SpeechMaterialName Then
                SelectedTest = ts
                Exit For
            End If
        Next

        Dim SpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(SelectedTest.GetSpeechMaterialFilePath, SelectedTest.GetTestRootPath)
        SpeechMaterial.ParentTestSpecification = SelectedTest
        SelectedTest.SpeechMaterial = SpeechMaterial

        'Loading media sets
        SpeechMaterial.ParentTestSpecification.LoadAvailableMediaSetSpecifications()
        Dim AvailableMediaSets = SpeechMaterial.ParentTestSpecification.MediaSets


        Dim SiPMeasurement = New SipTest.SipMeasurement("TrialSoundGeneration", SpeechMaterial.ParentTestSpecification)

        'Clearing any trials that may have been planned by a previous call
        SiPMeasurement.ClearTrials()

        SiPMeasurement.TestProcedure.TestParadigm = Testparadigm.Slow

        'Getting the sound source locations
        'Getting the sound source locations
        Dim TargetLocations = SiPMeasurement.TestProcedure.TargetStimulusLocations(SiPMeasurement.TestProcedure.TestParadigm)
        Dim MaskerLocations = SiPMeasurement.TestProcedure.MaskerLocations(SiPMeasurement.TestProcedure.TestParadigm)
        Dim BackgroundLocations = SiPMeasurement.TestProcedure.BackgroundLocations(SiPMeasurement.TestProcedure.TestParadigm)

        Dim AvaliableTransducers = OstfBase.AvaliableTransducers
        Dim SelectedTransducer = AvaliableTransducers(0)

        Dim TestWords = SpeechMaterial.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)

        Dim InterTrialInterval = 1
        Dim ResponseAlternativeDelay = 0.5
        Dim PretestSoundDuration = 5
        Dim MinimumStimulusOnsetTime = 0.3 ' Earlier, when this variable directed the test words instead of maskers its value was 1.5. Having maskers of 3 seconds with the test word centralized, should be approximately the same as 0.3.
        Dim MaximumStimulusOnsetTime = 0.8 ' Earlier, when this variable directed the test words instead of maskers its value was 2
        Dim TrialSoundMaxDuration = 7 ' TODO: Optimize by shortening this time
        Dim UseVisualCue = True
        Dim UseBackgroundSpeech = True
        Dim MaximumResponseTime = 4
        Dim ShowProgressIndication = True

        OstfBase.DirectionalSimulator.TrySetSelectedDirectionalSimulationSet("ARC - Harcellen - HATS - 48kHz", SelectedTransducer, False)

        Dim WordsList As New SortedList(Of Double, List(Of String)) From {
            {-2.0R, New List(Of String)},
            {6.0R, New List(Of String)}}

        Dim PNRSoundList As New SortedList(Of Double, List(Of Audio.Sound)) From {
            {-2.0R, New List(Of Audio.Sound)},
            {6.0R, New List(Of Audio.Sound)}}


        For twi = 0 To TestWords.Count - 1

            Dim TestWord = TestWords(twi)

            'Skipping a proportion of test words randomly
            If rnd.NextDouble > 0.1 Then Continue For

            'Randomizing the media set to use
            Dim SelectedMediaSet = AvailableMediaSets(rnd.Next(0, 2))

            For Each PNR_kvp In PNRSoundList

                Dim NewTrial As New SipTrial(New SiPTestUnit(SiPMeasurement), TestWord, SelectedMediaSet, SoundPropagationTypes.SimulatedSoundField,
                                     TargetLocations, MaskerLocations, BackgroundLocations, rnd)

                NewTrial.SetLevels(SpeechTestFramework.Audio.RaisedVocalEffortLevel, PNR_kvp.Key)

                NewTrial.MixSound(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, rnd, TrialSoundMaxDuration, UseBackgroundSpeech)

                PNR_kvp.Value.Add(NewTrial.TestTrialSound)

                'Cropping off the last second
                Audio.DSP.CropSection(NewTrial.TestTrialSound, 0, TrialSoundMaxDuration * NewTrial.TestTrialSound.WaveFormat.SampleRate)

                NewTrial.TestTrialSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "SipTrialSounds", "TrialSound_" & twi + 1 & "_PNR_" & PNR_kvp.Key & ".wav"))

                WordsList(PNR_kvp.Key).Add(TestWord.GetCategoricalVariableValue("Spelling"))
            Next

        Next

        'Getting concatenated sounds
        For Each PNR_kvp In PNRSoundList

            Dim PRN = PNR_kvp.Key
            Dim SoundList As New List(Of Audio.Sound)
            For Each Sound In PNRSoundList(PRN)
                SoundList.Add(Sound.CreateSoundDataCopy)
            Next

            Dim ConcatenatedSound = Audio.DSP.ConcatenateSounds(SoundList,,,,,, 48000)
            ConcatenatedSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "SipTrialSounds", "Demo_of_SiP_test_trial_sounds_at_" & PRN & ".wav"))

            Utils.SendInfoToLog("Exported test words: " & String.Join(", ", WordsList(PRN)),, IO.Path.Combine(Utils.logFilePath, "SipTrialSounds"))
        Next


    End Sub

    Private Sub Button26_Click(sender As Object, e As EventArgs) Handles Button26.Click

        Dim Input = Audio.Sound.LoadWaveFile("C:\SpeechTestFrameworkLog\SipTrialSounds\Demo_of_SiP_test_trial_sounds_at_-2_testwords_tugg-tung-sil-syr-sarg-rött-tuss-å.wav")
        Input.SMA = Nothing
        Input.WriteWaveFile("C:\SpeechTestFrameworkLog\SipTrialSounds\Demo_of_SiP_test_trial_sounds_at_-2_testwords_tugg-tung-sil-syr-sarg-rött-tuss-å_NoSMA.wav")

        Dim Input2 = Audio.Sound.LoadWaveFile("C:\SpeechTestFrameworkLog\SipTrialSounds\Demo_of_SiP_test_trial_sounds_at_6_testwords_tugg-tung-sil-syr-sarg-rött-tuss-å.wav")
        Input2.SMA = Nothing
        Input2.WriteWaveFile("C:\SpeechTestFrameworkLog\SipTrialSounds\Demo_of_SiP_test_trial_sounds_at_6_testwords_tugg-tung-sil-syr-sarg-rött-tuss-å_NoSMA.wav")

    End Sub

    Private Sub Button27_Click(sender As Object, e As EventArgs) Handles Button27.Click

        Dim TestSound = Audio.GenerateSound.CreateWhiteNoise(New Audio.Formats.WaveFormat(48000, 32, 1), 1, 0.2, 3000)
        'TestSound.WaveData.SampleData(1) = {1, 1, 1, 1}
        Dim TestSoundCopy1 = TestSound.CreateCopy
        Dim TestSoundCopy2 = TestSound.CreateCopy
        Dim Gain As Double = 50

        Dim Level1 = Audio.DSP.MeasureSectionLevel(TestSound, 1)

        OstfBase.UseOptimizationLibraries = False

        Dim timer1 = New Stopwatch
        Dim timer2 = New Stopwatch

        timer1.Start()

        Audio.DSP.AmplifySection(TestSoundCopy1, Gain)
        Dim t1 = timer1.ElapsedMilliseconds

        Dim Level2 = Audio.DSP.MeasureSectionLevel(TestSoundCopy1, 1)


        OstfBase.UseOptimizationLibraries = True

        timer2.Start()

        Audio.DSP.AmplifySection(TestSoundCopy2, Gain)
        Dim t2 = timer2.ElapsedMilliseconds
        Dim Level3 = Audio.DSP.MeasureSectionLevel(TestSoundCopy2, 1)


        Console.WriteLine(Level2)
        Console.WriteLine(Level3)

        Console.WriteLine(Level2 - Level1)
        Console.WriteLine(Level3 - Level1)

        Dim x = 1


    End Sub

    Private Sub Button28_Click(sender As Object, e As EventArgs) Handles Button28.Click

        'SpeechTestFramework.Utils.ReplaceCharsInFileSystemEntries("C:\EriksDokument\source\repos\OSTF\OSTFMedia_M")

    End Sub

    Private Sub Button29_Click(sender As Object, e As EventArgs) Handles Button29.Click

        'Testing HL simulator
        Dim InputSound = SpeechTestFramework.Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSiPTest\Media\Unechoic-Talker1-RVE\TestWordRecordings\L02S03_blund\M_000_000_blund.wav")

        Dim HLS = New SpeechTestFramework.Audio.HearinglossSimulator_NoiseBased()

        Dim SimulatedSound = HLS.Simulate(InputSound)

        SimulatedSound.WriteWaveFile(System.IO.Path.Combine(Utils.logFilePath, "SimSound1.wav"))

    End Sub

    Private Sub Button30_Click(sender As Object, e As EventArgs) Handles Button30.Click

        Dim TimeSpanList As New List(Of String)
        Dim StopWatch As New Stopwatch
        StopWatch.Start()

        Dim rnd As New Random(42)
        Dim ArrayLength As Integer = 10 ^ 8
        Dim Array1(ArrayLength - 1) As Single
        Dim Array2(ArrayLength - 1) As Single
        For i = 0 To ArrayLength - 1
            Array1(i) = rnd.NextDouble
            Array2(i) = rnd.NextDouble
        Next

        StopWatch.Stop()
        TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
        StopWatch.Reset()
        StopWatch.Start()

        'Performing array zip with indexed loop
        Dim ResultArray(ArrayLength - 1) As Single
        For i = 0 To Array1.Length - 1
            ResultArray(i) = Array1(i) * Array2(i)
        Next

        StopWatch.Stop()
        TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
        StopWatch.Reset()
        StopWatch.Start()

        'Performing array zip with linq
        Dim ResultArray3 = Array1.Zip(Array2, Function(a, b) a * b)
        Dim ArraySum = ResultArray3.Sum

        StopWatch.Stop()
        TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
        StopWatch.Reset()

        'Testing using SIMD

        'Dim f As Single = 1
        'Dim Array4 = Array1.Select(Function(x) x * f)

        MsgBox(String.Join(vbCrLf, TimeSpanList))

    End Sub

    Private Sub Button31_Click(sender As Object, e As EventArgs) Handles Button31.Click

        Dim FR As New List(Of Tuple(Of Single, Single))

        ''Octave bands
        'FR.Add(New Tuple(Of Single, Single)(250, -6))
        'FR.Add(New Tuple(Of Single, Single)(500, -3))
        'FR.Add(New Tuple(Of Single, Single)(1000, 0))
        'FR.Add(New Tuple(Of Single, Single)(2000, -9))
        'FR.Add(New Tuple(Of Single, Single)(4000, -18))
        'FR.Add(New Tuple(Of Single, Single)(8000, -27))

        ''Octave bands - flat spectrum levels
        'FR.Add(New Tuple(Of Single, Single)(125 / 8, 9))
        'FR.Add(New Tuple(Of Single, Single)(125 / 4, 6))
        'FR.Add(New Tuple(Of Single, Single)(125 / 2, 3))
        'FR.Add(New Tuple(Of Single, Single)(125, 0))
        'FR.Add(New Tuple(Of Single, Single)(250, -3))
        'FR.Add(New Tuple(Of Single, Single)(500, -6))
        'FR.Add(New Tuple(Of Single, Single)(1000, -9))
        'FR.Add(New Tuple(Of Single, Single)(2000, -12))
        'FR.Add(New Tuple(Of Single, Single)(4000, -15))
        'FR.Add(New Tuple(Of Single, Single)(8000, -18))

        '''One-third octave bands
        'FR.Add(New Tuple(Of Single, Single)(125 / 4, 7))
        'FR.Add(New Tuple(Of Single, Single)(125 / 2, 4))
        'FR.Add(New Tuple(Of Single, Single)(125, 1))
        'FR.Add(New Tuple(Of Single, Single)(160, 0))
        'FR.Add(New Tuple(Of Single, Single)(200, -1))
        'FR.Add(New Tuple(Of Single, Single)(250, -2))
        'FR.Add(New Tuple(Of Single, Single)(315, -3))
        'FR.Add(New Tuple(Of Single, Single)(400, -4))
        'FR.Add(New Tuple(Of Single, Single)(500, -5))
        'FR.Add(New Tuple(Of Single, Single)(630, -6))
        'FR.Add(New Tuple(Of Single, Single)(800, -7))
        'FR.Add(New Tuple(Of Single, Single)(1000, -8))
        'FR.Add(New Tuple(Of Single, Single)(1250, -13))
        'FR.Add(New Tuple(Of Single, Single)(1600, -18))
        'FR.Add(New Tuple(Of Single, Single)(2000, -23))
        'FR.Add(New Tuple(Of Single, Single)(2500, -28))
        'FR.Add(New Tuple(Of Single, Single)(3150, -33))
        'FR.Add(New Tuple(Of Single, Single)(4000, -38))
        'FR.Add(New Tuple(Of Single, Single)(5000, -43))
        'FR.Add(New Tuple(Of Single, Single)(6300, -48))
        'FR.Add(New Tuple(Of Single, Single)(8000, -53))
        'FR.Add(New Tuple(Of Single, Single)(16000, -68))
        'FR.Add(New Tuple(Of Single, Single)(22000, -75))
        'FR.Add(New Tuple(Of Single, Single)(24000, -75))

        ''Octave bands
        FR.Add(New Tuple(Of Single, Single)(31.25, 0))
        FR.Add(New Tuple(Of Single, Single)(62.5, -3))
        FR.Add(New Tuple(Of Single, Single)(125, -6))
        FR.Add(New Tuple(Of Single, Single)(250, -9))
        FR.Add(New Tuple(Of Single, Single)(500, -12))
        FR.Add(New Tuple(Of Single, Single)(1000, -15))
        FR.Add(New Tuple(Of Single, Single)(2000, -30))
        FR.Add(New Tuple(Of Single, Single)(4000, -57))
        FR.Add(New Tuple(Of Single, Single)(8000, -96))
        FR.Add(New Tuple(Of Single, Single)(16000, -147))



        Dim IR = Audio.GenerateSound.CreateCustumImpulseResponse(FR, Nothing, New Audio.Formats.WaveFormat(48000, 32, 1,,
                                                                                                Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints), New Audio.Formats.FftFormat(2048), 2048)

        'Creates a warble tone at 1 kHz (measurement sound)
        Dim InternalNoiseSound = Audio.GenerateSound.CreateWhiteNoise(IR.WaveFormat, 1, , 10, Audio.BasicAudioEnums.TimeUnits.seconds)

        'Runs convolution with the kernel
        Dim NoiseSound = SpeechTestFramework.Audio.DSP.FIRFilter(InternalNoiseSound, IR, New SpeechTestFramework.Audio.Formats.FftFormat, ,,,,, True)

        NoiseSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "SWN_3rdOB_5.wav"))
        'NoiseSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "SWN_1OB_Flat2.wav"))

    End Sub


    Private Sub Button32_Click(sender As Object, e As EventArgs) Handles Button32.Click


        Dim SoundFile = SpeechTestFramework.Audio.GenerateSound.CreateSilence(New Audio.Formats.WaveFormat(48000, 32, 1), 1, 1)

        'SoundFile = SpeechTestFramework.Audio.GenerateSound.CreateSineWave(New Audio.Formats.WaveFormat(48000, 32, 1), 1)

        Dim SampleArray = SoundFile.WaveData.SampleData(1)

        For i = 0 To SampleArray.Length - 1
            SampleArray(i) = i * (1 / SampleArray.Length)
        Next

        'SoundFile.WaveData.SampleData(1) = SampleArray


        Dim SE = New SpeechTestFramework.Audio.Graphics.SoundEditor(SoundFile)
        SE.Dock = DockStyle.Fill

        Dim NewForm As New Windows.Forms.Form
        NewForm.Controls.Add(SE)
        NewForm.Show()
    End Sub

    Private Sub Button33_Click(sender As Object, e As EventArgs) Handles Button33.Click

        Dim IR = Audio.Sound.LoadWaveFile("C:\Temp\Supplement 2.wav")
        Dim Sound = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSiPTest\Media\Home-Talker1-RVE\BackgroundNonspeech\Background1-WashingMachine_Fan.wav")

        Dim OutputSound = Audio.DSP.FIRFilter(Sound, IR, New Audio.Formats.FftFormat, 1,,,,, True, True)

        Audio.DSP.RemoveDcComponent(OutputSound)

        Audio.DSP.MeasureAndAdjustSectionLevel(OutputSound, -36)

        OutputSound.WriteWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSiPTest\Media\Home-Talker1-RVE\BackgroundNonspeech\Background1-WashingMachine_Fan_HpFiltered.wav")

    End Sub

    Private Sub Button34_Click(sender As Object, e As EventArgs) Handles Button34.Click

        Dim ReadFolder As String = "C:\EriksDokument\EikholtEdit\Output_hd_7Mbps"
        Dim WriteFolder As String = "C:\EriksDokument\EikholtEdit\June24\Output_hd_7Mbps"

        Dim Files = {"Actor_3_L00S23_M1", "101",
"Actor_4_L13S05_M1", "102",
"Actor_1_L16S04_M1", "103",
"Actor_2_L23S03_M1", "104",
"Actor_3_L16S08_M1", "105",
"Actor_4_L10S17_M1", "106",
"Actor_1_L03S14_M1", "107",
"Actor_2_L07S19_M1", "108",
"Actor_3_L07S10_M1", "109",
"Actor_4_L04S05_M1", "110",
"Actor_1_L23S06_M1", "111",
"Actor_2_L12S03_M1", "112",
"Actor_3_L03S08_M1", "113",
"Actor_4_L08S17_M1", "114",
"Actor_1_L04S06_M1", "115",
"Actor_2_L15S07_M1", "116",
"Actor_3_L03S00_M1", "117",
"Actor_4_L16S09_M1", "118",
"Actor_1_L02S10_M1", "119",
"Actor_2_L12S23_M1", "120",
"Actor_3_L12S16_M1", "121",
"Actor_4_L14S17_M1", "122",
"Actor_1_L05S02_M1", "123",
"Actor_2_L03S15_M1", "124",
"Actor_1_L11S14_M1", "201",
"Actor_2_L20S11_M1", "202",
"Actor_3_L22S08_M1", "203",
"Actor_4_L16S01_M1", "204",
"Actor_1_L16S14_M1", "205",
"Actor_2_L08S03_M1", "206",
"Actor_3_L11S20_M1", "207",
"Actor_4_L18S09_M1", "208",
"Actor_3_L09S16_M1", "209",
"Actor_2_L20S07_M1", "210",
"Actor_3_L03S16_M1", "211",
"Actor_4_L10S01_M1", "212",
"Actor_1_L00S21_M1", "213",
"Actor_2_L14S07_M1", "214",
"Actor_3_L08S04_M1", "215",
"Actor_4_L01S01_M1", "216",
"Actor_1_L07S18_M1", "217",
"Actor_2_L22S03_M1", "218",
"Actor_3_L23S20_M1", "219",
"Actor_4_L06S21_M1", "220",
"Actor_1_L18S18_M1", "221",
"Actor_2_L05S19_M1", "222",
"Actor_3_L05S04_M1", "223",
"Actor_4_L13S13_M1", "224",
"Actor_4_L03S09_M1", "301",
"Actor_3_L21S12_M1", "302",
"Actor_2_L22S15_M1", "303",
"Actor_1_L01S18_M1", "304",
"Actor_4_L15S21_M1", "305",
"Actor_3_L05S00_M1", "306",
"Actor_2_L02S19_M1", "307",
"Actor_1_L20S22_M1", "308",
"Actor_4_L07S24_M1", "309",
"Actor_3_L06S20_M1", "310",
"Actor_2_L09S11_M1", "311",
"Actor_1_L23S18_M1", "312",
"Actor_4_L01S09_M1", "313",
"Actor_1_L02S08_M1", "314",
"Actor_2_L01S23_M1", "315",
"Actor_1_L03S10_M1", "316",
"Actor_4_L11S09_M1", "317",
"Actor_3_L20S16_M1", "318",
"Actor_2_L13S07_M1", "319",
"Actor_1_L20S10_M1", "320",
"Actor_4_L10S21_M1", "321",
"Actor_3_L22S04_M1", "322",
"Actor_2_L06S19_M1", "323",
"Actor_1_L14S02_M1", "324",
"Actor_3_L04S00_M1", "401",
"Actor_4_L00S09_M1", "402",
"Actor_1_L09S14_M1", "403",
"Actor_2_L17S03_M1", "404",
"Actor_1_L08S08_M1", "405",
"Actor_4_L19S17_M1", "406",
"Actor_1_L23S02_M1", "407",
"Actor_2_L14S11_M1", "408",
"Actor_3_L15S20_M1", "409",
"Actor_4_L20S17_M1", "410",
"Actor_1_L07S02_M1", "411",
"Actor_2_L06S07_M1", "412",
"Actor_3_L00S19_M1", "413",
"Actor_4_L07S17_M1", "414",
"Actor_1_L12S14_M1", "415",
"Actor_2_L02S11_M1", "416",
"Actor_3_L13S12_M1", "417",
"Actor_4_L13S21_M1", "418",
"Actor_1_L03S02_M1", "419",
"Actor_2_L13S19_M1", "420",
"Actor_3_L12S08_M1", "421",
"Actor_4_L21S01_M1", "422",
"Actor_1_L09S02_M1", "423",
"Actor_2_L22S11_M1", "424",
"Actor_2_L16S11_M1", "501",
"Actor_1_L05S10_M1", "502",
"Actor_4_L23S21_M1", "503",
"Actor_3_L04S24_M1", "504",
"Actor_2_L00S18_M1", "505",
"Actor_3_L08S10_M1", "506",
"Actor_4_L00S17_M1", "507",
"Actor_3_L07S00_M1", "508",
"Actor_2_L17S11_M1", "509",
"Actor_1_L11S10_M1", "510",
"Actor_4_L03S05_M1", "511",
"Actor_3_L11S08_M1", "512",
"Actor_2_L10S07_M1", "513",
"Actor_1_L12S18_M1", "514",
"Actor_4_L12S09_M1", "515",
"Actor_3_L13S00_M1", "516",
"Actor_2_L18S19_M1", "517",
"Actor_1_L19S06_M1", "518",
"Actor_4_L21S09_M1", "519",
"Actor_3_L13S08_M1", "520",
"Actor_2_L03S23_M1", "521",
"Actor_1_L13S18_M1", "522",
"Actor_4_L09S05_M1", "523",
"Actor_3_L03S04_M1", "524",
"Actor_1_L04S14_M1", "601",
"Actor_2_L18S15_M1", "602",
"Actor_3_L04S08_M1", "603",
"Actor_4_L04S13_M1", "604",
"Actor_1_L11S06_M1", "605",
"Actor_2_L20S23_M1", "606",
"Actor_3_L01S12_M1", "607",
"Actor_4_L10S13_M1", "608",
"Actor_1_L02S18_M1", "609",
"Actor_2_L01S03_M1", "610",
"Actor_3_L19S12_M1", "611",
"Actor_4_L14S21_M1", "612",
"Actor_1_L19S02_M1", "613",
"Actor_2_L16S23_M1", "614",
"Actor_3_L01S20_M1", "615",
"Actor_4_L00S05_M1", "616",
"Actor_1_L15S06_M1", "617",
"Actor_2_L18S01_M1", "618",
"Actor_3_L10S24_M1", "619",
"Actor_4_L09S17_M1", "620",
"Actor_1_L00S14_M1", "621",
"Actor_2_L08S19_M1", "622",
"Actor_3_L10S08_M1", "623",
"Actor_4_L01S05_M1", "624",
"Actor_4_L11S05_M1", "701",
"Actor_1_L09S06_M1", "702",
"Actor_2_L01S11_M1", "703",
"Actor_1_L09S10_M1", "704",
"Actor_4_L06S13_M1", "705",
"Actor_3_L20S20_M1", "706",
"Actor_2_L06S11_M1", "707",
"Actor_1_L20S02_M1", "708",
"Actor_4_L17S21_M1", "709",
"Actor_3_L08S24_M1", "710",
"Actor_2_L09S19_M1", "711",
"Actor_1_L20S18_M1", "712",
"Actor_4_L23S13_M1", "713",
"Actor_3_L22S12_M1", "714",
"Actor_2_L13S23_M1", "715",
"Actor_1_L07S14_M1", "716",
"Actor_4_L07S21_M1", "717",
"Actor_3_L03S24_M1", "718",
"Actor_2_L20S15_M1", "719",
"Actor_1_L07S08_M1", "720",
"Actor_4_L15S13_M1", "721",
"Actor_3_L15S24_M1", "722",
"Actor_2_L18S11_M1", "723",
"Actor_1_L01S02_M1", "724",
"Actor_3_L17S08_M1", "801",
"Actor_4_L19S21_M1", "802",
"Actor_1_L09S18_M1", "803",
"Actor_2_L17S23_M1", "804",
"Actor_3_L19S20_M1", "805",
"Actor_4_L09S13_M1", "806",
"Actor_1_L06S14_M1", "807",
"Actor_2_L21S23_M1", "808",
"Actor_3_L19S08_M1", "809",
"Actor_4_L13S09_M1", "810",
"Actor_1_L03S06_M1", "811",
"Actor_2_L13S11_M1", "812",
"Actor_3_L20S24_M1", "813",
"Actor_4_L00S24_M1", "814",
"Actor_1_L10S22_M1", "815",
"Actor_2_L11S11_M1", "816",
"Actor_3_L20S04_M1", "817",
"Actor_4_L22S01_M1", "818",
"Actor_1_L07S22_M1", "819",
"Actor_2_L22S19_M1", "820",
"Actor_3_L17S12_M1", "821",
"Actor_4_L01S21_M1", "822",
"Actor_1_L14S22_M1", "823",
"Actor_2_L21S07_M1", "824",
"Actor_2_L15S11_M1", "901",
"Actor_1_L12S22_M1", "902",
"Actor_4_L14S09_M1", "903",
"Actor_3_L13S24_M1", "904",
"Actor_2_L07S23_M1", "905",
"Actor_1_L11S18_M1", "906",
"Actor_4_L11S17_M1", "907",
"Actor_3_L11S00_M1", "908",
"Actor_2_L22S07_M1", "909",
"Actor_1_L13S22_M1", "910",
"Actor_4_L09S21_M1", "911",
"Actor_3_L17S04_M1", "912",
"Actor_2_L16S19_M1", "913",
"Actor_1_L22S02_M1", "914",
"Actor_4_L16S13_M1", "915",
"Actor_3_L02S04_M1", "916",
"Actor_2_L18S23_M1", "917",
"Actor_1_L20S14_M1", "918",
"Actor_4_L23S17_M1", "919",
"Actor_3_L18S08_M1", "920",
"Actor_2_L00S22_M1", "921",
"Actor_1_L14S14_M1", "922",
"Actor_4_L15S05_M1", "923",
"Actor_3_L08S12_M1", "924",
"Actor_1_L13S02_M1", "1001",
"Actor_2_L19S11_M1", "1002",
"Actor_3_L20S08_M1", "1003",
"Actor_4_L19S01_M1", "1004",
"Actor_1_L10S06_M1", "1005",
"Actor_2_L12S15_M1", "1006",
"Actor_3_L00S15_M1", "1007",
"Actor_4_L22S21_M1", "1008",
"Actor_3_L05S16_M1", "1009",
"Actor_2_L06S15_M1", "1010",
"Actor_3_L21S16_M1", "1011",
"Actor_4_L06S09_M1", "1012",
"Actor_1_L19S22_M1", "1013",
"Actor_2_L02S15_M1", "1014",
"Actor_3_L04S20_M1", "1015",
"Actor_4_L09S01_M1", "1016",
"Actor_1_L15S18_M1", "1017",
"Actor_2_L19S19_M1", "1018",
"Actor_3_L20S00_M1", "1019",
"Actor_4_L03S17_M1", "1020",
"Actor_1_L13S14_M1", "1021",
"Actor_2_L12S19_M1", "1022",
"Actor_3_L06S22_M1", "1023",
"Actor_4_L07S09_M1", "1024",
"Actor_4_L04S09_M1", "1101",
"Actor_3_L00S00_M1", "1102",
"Actor_2_L04S19_M1", "1103",
"Actor_1_L18S22_M1", "1104",
"Actor_4_L17S05_M1", "1105",
"Actor_3_L16S24_M1", "1106",
"Actor_2_L17S07_M1", "1107",
"Actor_3_L05S06_M1", "1108",
"Actor_4_L12S05_M1", "1109",
"Actor_3_L14S16_M1", "1110",
"Actor_2_L21S19_M1", "1111",
"Actor_1_L02S06_M1", "1112",
"Actor_4_L05S21_M1", "1113",
"Actor_3_L10S12_M1", "1114",
"Actor_2_L03S03_M1", "1115",
"Actor_1_L22S22_M1", "1116",
"Actor_4_L15S17_M1", "1117",
"Actor_3_L14S08_M1", "1118",
"Actor_2_L04S03_M1", "1119",
"Actor_1_L01S06_M1", "1120",
"Actor_4_L08S15_M1", "1121",
"Actor_3_L18S16_M1", "1122",
"Actor_2_L05S03_M1", "1123",
"Actor_1_L18S06_M1", "1124",
"Actor_3_L01S24_M1", "1201",
"Actor_4_L05S13_M1", "1202",
"Actor_1_L02S22_M1", "1203",
"Actor_2_L16S03_M1", "1204",
"Actor_3_L15S12_M1", "1205",
"Actor_4_L19S09_M1", "1206",
"Actor_1_L10S18_M1", "1207",
"Actor_2_L18S07_M1", "1208",
"Actor_3_L12S04_M1", "1209",
"Actor_4_L11S01_M1", "1210",
"Actor_1_L15S14_M1", "1211",
"Actor_2_L11S07_M1", "1212",
"Actor_3_L16S16_M1", "1213",
"Actor_4_L15S01_M1", "1214",
"Actor_1_L21S06_M1", "1215",
"Actor_2_L02S07_M1", "1216",
"Actor_3_L23S12_M1", "1217",
"Actor_4_L15S09_M1", "1218",
"Actor_1_L12S06_M1", "1219",
"Actor_2_L07S15_M1", "1220",
"Actor_3_L17S20_M1", "1221",
"Actor_4_L02S05_M1", "1222",
"Actor_1_L21S10_M1", "1223",
"Actor_2_L23S23_M1", "1224",
"Actor_2_L23S11_M1", "1301",
"Actor_1_L12S02_M1", "1302",
"Actor_4_L00S13_M1", "1303",
"Actor_3_L14S04_M1", "1304",
"Actor_2_L07S07_M1", "1305",
"Actor_1_L20S06_M1", "1306",
"Actor_4_L14S01_M1", "1307",
"Actor_3_L03S12_M1", "1308",
"Actor_2_L03S19_M1", "1309",
"Actor_1_L16S10_M1", "1310",
"Actor_4_L01S17_M1", "1311",
"Actor_3_L21S04_M1", "1312",
"Actor_2_L00S07_M1", "1313",
"Actor_1_L16S02_M1", "1314",
"Actor_4_L18S13_M1", "1315",
"Actor_3_L01S04_M1", "1316",
"Actor_2_L20S03_M1", "1317",
"Actor_1_L06S10_M1", "1318",
"Actor_4_L02S01_M1", "1319",
"Actor_3_L02S16_M1", "1320",
"Actor_2_L20S19_M1", "1321",
"Actor_1_L16S06_M1", "1322",
"Actor_4_L22S17_M1", "1323",
"Actor_3_L22S20_M1", "1324",
"Actor_1_L12S10_M1", "1401",
"Actor_2_L21S11_M1", "1402",
"Actor_1_L05S08_M1", "1403",
"Actor_4_L02S09_M1", "1404",
"Actor_1_L11S02_M1", "1405",
"Actor_2_L23S15_M1", "1406",
"Actor_3_L08S00_M1", "1407",
"Actor_4_L00S16_M1", "1408",
"Actor_1_L23S22_M1", "1409",
"Actor_2_L21S03_M1", "1410",
"Actor_3_L21S00_M1", "1411",
"Actor_4_L06S17_M1", "1412",
"Actor_1_L05S18_M1", "1413",
"Actor_2_L04S15_M1", "1414",
"Actor_3_L10S04_M1", "1415",
"Actor_4_L12S17_M1", "1416",
"Actor_1_L14S18_M1", "1417",
"Actor_2_L10S23_M1", "1418",
"Actor_3_L07S16_M1", "1419",
"Actor_4_L21S05_M1", "1420",
"Actor_1_L16S18_M1", "1421",
"Actor_2_L00S11_M1", "1422",
"Actor_3_L21S08_M1", "1423",
"Actor_4_L00S20_M1", "1424",
"Actor_4_L02S13_M1", "1501",
"Actor_3_L16S20_M1", "1502",
"Actor_2_L05S11_M1", "1503",
"Actor_1_L23S14_M1", "1504",
"Actor_4_L11S13_M1", "1505",
"Actor_3_L14S00_M1", "1506",
"Actor_2_L23S19_M1", "1507",
"Actor_1_L17S14_M1", "1508",
"Actor_4_L08S01_M1", "1509",
"Actor_3_L08S20_M1", "1510",
"Actor_2_L10S11_M1", "1511",
"Actor_1_L08S22_M1", "1512",
"Actor_4_L08S21_M1", "1513",
"Actor_3_L19S04_M1", "1514",
"Actor_2_L02S23_M1", "1515",
"Actor_1_L06S18_M1", "1516",
"Actor_4_L20S09_M1", "1517",
"Actor_3_L11S24_M1", "1518",
"Actor_2_L06S23_M1", "1519",
"Actor_1_L17S02_M1", "1520",
"Actor_4_L23S01_M1", "1521",
"Actor_3_L10S00_M1", "1522",
"Actor_2_L22S23_M1", "1523",
"Actor_1_L17S18_M1", "1524",
"Actor_3_L02S00_M1", "1601",
"Actor_4_L02S21_M1", "1602",
"Actor_1_L22S10_M1", "1603",
"Actor_2_L03S11_M1", "1604",
"Actor_3_L09S12_M1", "1605",
"Actor_4_L21S21_M1", "1606",
"Actor_1_L07S06_M1", "1607",
"Actor_2_L09S15_M1", "1608",
"Actor_3_L23S00_M1", "1609",
"Actor_4_L16S17_M1", "1610",
"Actor_1_L08S06_M1", "1611",
"Actor_2_L08S07_M1", "1612",
"Actor_3_L19S16_M1", "1613",
"Actor_4_L10S05_M1", "1614",
"Actor_1_L22S14_M1", "1615",
"Actor_2_L11S15_M1", "1616",
"Actor_3_L17S24_M1", "1617",
"Actor_4_L14S05_M1", "1618",
"Actor_1_L08S18_M1", "1619",
"Actor_2_L01S15_M1", "1620",
"Actor_3_L06S24_M1", "1621",
"Actor_4_L19S05_M1", "1622",
"Actor_1_L22S18_M1", "1623",
"Actor_2_L16S07_M1", "1624",
"Actor_2_L13S03_M1", "1701",
"Actor_1_L04S22_M1", "1702",
"Actor_4_L10S09_M1", "1703",
"Actor_3_L12S20_M1", "1704",
"Actor_2_L09S03_M1", "1705",
"Actor_1_L18S10_M1", "1706",
"Actor_4_L17S17_M1", "1707",
"Actor_1_L13S04_M1", "1708",
"Actor_2_L09S23_M1", "1709",
"Actor_1_L01S14_M1", "1710",
"Actor_4_L20S13_M1", "1711",
"Actor_3_L03S20_M1", "1712",
"Actor_2_L10S03_M1", "1713",
"Actor_1_L10S02_M1", "1714",
"Actor_4_L07S01_M1", "1715",
"Actor_3_L22S00_M1", "1716",
"Actor_2_L07S11_M1", "1717",
"Actor_1_L21S18_M1", "1718",
"Actor_4_L01S08_M1", "1719",
"Actor_3_L17S16_M1", "1720",
"Actor_2_L09S07_M1", "1721",
"Actor_1_L17S06_M1", "1722",
"Actor_4_L22S09_M1", "1723",
"Actor_3_L10S20_M1", "1724",
"Actor_1_L05S14_M1", "1801",
"Actor_2_L11S19_M1", "1802",
"Actor_3_L19S00_M1", "1803",
"Actor_4_L06S01_M1", "1804",
"Actor_1_L06S02_M1", "1805",
"Actor_2_L01S19_M1", "1806",
"Actor_3_L02S24_M1", "1807",
"Actor_4_L14S13_M1", "1808",
"Actor_1_L02S02_M1", "1809",
"Actor_2_L07S03_M1", "1810",
"Actor_3_L06S16_M1", "1811",
"Actor_4_L23S09_M1", "1812",
"Actor_1_L15S02_M1", "1813",
"Actor_2_L14S03_M1", "1814",
"Actor_3_L04S16_M1", "1815",
"Actor_4_L07S13_M1", "1816",
"Actor_1_L06S06_M1", "1817",
"Actor_2_L10S19_M1", "1818",
"Actor_3_L18S12_M1", "1819",
"Actor_4_L18S00_M1", "1820",
"Actor_1_L02S14_M1", "1821",
"Actor_2_L10S15_M1", "1822",
"Actor_3_L22S24_M1", "1823",
"Actor_4_L05S05_M1", "1824",
"Actor_4_L12S01_M1", "1901",
"Actor_3_L21S24_M1", "1902",
"Actor_2_L12S11_M1", "1903",
"Actor_1_L08S14_M1", "1904",
"Actor_4_L21S13_M1", "1905",
"Actor_3_L06S12_M1", "1906",
"Actor_2_L19S03_M1", "1907",
"Actor_1_L11S22_M1", "1908",
"Actor_4_L20S21_M1", "1909",
"Actor_3_L11S04_M1", "1910",
"Actor_2_L14S15_M1", "1911",
"Actor_1_L10S14_M1", "1912",
"Actor_4_L13S01_M1", "1913",
"Actor_3_L01S16_M1", "1914",
"Actor_2_L15S23_M1", "1915",
"Actor_1_L16S22_M1", "1916",
"Actor_4_L17S09_M1", "1917",
"Actor_3_L14S24_M1", "1918",
"Actor_2_L11S23_M1", "1919",
"Actor_1_L19S10_M1", "1920",
"Actor_4_L05S17_M1", "1921",
"Actor_3_L22S16_M1", "1922",
"Actor_2_L12S07_M1", "1923",
"Actor_1_L23S10_M1", "1924",
"Actor_3_L20S12_M1", "2001",
"Actor_4_L03S13_M1", "2002",
"Actor_1_L14S10_M1", "2003",
"Actor_2_L05S07_M1", "2004",
"Actor_3_L02S12_M1", "2005",
"Actor_4_L05S09_M1", "2006",
"Actor_1_L01S22_M1", "2007",
"Actor_2_L05S15_M1", "2008",
"Actor_3_L04S04_M1", "2009",
"Actor_4_L09S09_M1", "2010",
"Actor_1_L14S06_M1", "2011",
"Actor_2_L17S19_M1", "2012",
"Actor_3_L09S00_M1", "2013",
"Actor_4_L19S13_M1", "2014",
"Actor_1_L17S22_M1", "2015",
"Actor_2_L05S23_M1", "2016",
"Actor_3_L18S24_M1", "2017",
"Actor_4_L18S21_M1", "2018",
"Actor_1_L05S22_M1", "2019",
"Actor_2_L15S19_M1", "2020",
"Actor_3_L13S16_M1", "2021",
"Actor_4_L18S03_M1", "2022",
"Actor_1_L21S14_M1", "2023",
"Actor_2_L19S23_M1", "2024",
"Actor_2_L11S03_M1", "2101",
"Actor_1_L21S22_M1", "2102",
"Actor_4_L13S17_M1", "2103",
"Actor_3_L13S20_M1", "2104",
"Actor_2_L02S03_M1", "2105",
"Actor_1_L19S14_M1", "2106",
"Actor_4_L02S17_M1", "2107",
"Actor_3_L08S16_M1", "2108",
"Actor_2_L15S03_M1", "2109",
"Actor_1_L15S10_M1", "2110",
"Actor_4_L03S21_M1", "2111",
"Actor_3_L14S20_M1", "2112",
"Actor_2_L01S07_M1", "2113",
"Actor_1_L03S22_M1", "2114",
"Actor_4_L04S21_M1", "2115",
"Actor_3_L21S20_M1", "2116",
"Actor_2_L17S15_M1", "2117",
"Actor_1_L10S10_M1", "2118",
"Actor_4_L18S05_M1", "2119",
"Actor_3_L16S00_M1", "2120",
"Actor_2_L23S07_M1", "2121",
"Actor_1_L09S22_M1", "2122",
"Actor_4_L03S01_M1", "2123",
"Actor_3_L17S00_M1", "2124",
"Actor_1_L22S06_M1", "2201",
"Actor_2_L08S23_M1", "2202",
"Actor_3_L09S04_M1", "2203",
"Actor_4_L07S05_M1", "2204",
"Actor_1_L03S18_M1", "2205",
"Actor_2_L04S23_M1", "2206",
"Actor_3_L23S04_M1", "2207",
"Actor_4_L17S01_M1", "2208",
"Actor_1_L04S18_M1", "2209",
"Actor_2_L16S15_M1", "2210",
"Actor_3_L23S24_M1", "2211",
"Actor_4_L18S17_M1", "2212",
"Actor_1_L01S10_M1", "2213",
"Actor_2_L04S07_M1", "2214",
"Actor_3_L15S04_M1", "2215",
"Actor_4_L06S05_M1", "2216",
"Actor_1_L04S10_M1", "2217",
"Actor_2_L08S11_M1", "2218",
"Actor_3_L00S12_M1", "2219",
"Actor_4_L16S21_M1", "2220",
"Actor_3_L13S06_M1", "2221",
"Actor_2_L15S15_M1", "2222",
"Actor_3_L09S24_M1", "2223",
"Actor_4_L22S13_M1", "2224",
"Actor_4_L08S05_M1", "2301",
"Actor_3_L18S20_M1", "2302",
"Actor_2_L21S15_M1", "2303",
"Actor_1_L17S10_M1", "2304",
"Actor_4_L20S05_M1", "2305",
"Actor_3_L00S08_M1", "2306",
"Actor_2_L19S07_M1", "2307",
"Actor_1_L15S22_M1", "2308",
"Actor_4_L08S13_M1", "2309",
"Actor_3_L04S12_M1", "2310",
"Actor_2_L13S15_M1", "2311",
"Actor_1_L13S10_M1", "2312",
"Actor_4_L17S13_M1", "2313",
"Actor_3_L09S20_M1", "2314",
"Actor_2_L19S15_M1", "2315",
"Actor_1_L08S02_M1", "2316",
"Actor_4_L04S01_M1", "2317",
"Actor_3_L01S00_M1", "2318",
"Actor_2_L14S23_M1", "2319",
"Actor_1_L21S02_M1", "2320",
"Actor_4_L12S13_M1", "2321",
"Actor_3_L00S04_M1", "2322",
"Actor_2_L04S11_M1", "2323",
"Actor_1_L04S02_M1", "2324"}

        For i = 0 To Files.Length - 1 Step 2

            Dim ReadVideoFile As String = IO.Path.Combine(ReadFolder, "Mixed", Files(i) & ".mp4")
            Dim ReadWaveFile As String = IO.Path.Combine(ReadFolder, Files(i) & ".wav")

            Dim WriteVideoFile As String = IO.Path.Combine(WriteFolder, "Mixed", Files(i + 1) & ".mp4")
            Dim WriteWaveFile As String = IO.Path.Combine(WriteFolder, Files(i + 1) & ".wav")

            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(WriteVideoFile))
            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(WriteWaveFile))

            IO.File.Copy(ReadVideoFile, WriteVideoFile)
            IO.File.Copy(ReadWaveFile, WriteWaveFile)

        Next

        MsgBox("Done!")

    End Sub

    Private Sub Button35_Click(sender As Object, e As EventArgs) Handles Button35.Click

        'Dim WF = Audio.Sound.LoadWaveFile("C:\EriksDokument\Software\Temporary Affinity Suite\Affinity Suite\AmtasSound\such.wav")

        Dim WF = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\AMTEST_(SE)_MixII\Media\Talker2-RVE\AMTEST-sounds-CDq\blek.wav")



    End Sub


    Private Sub ConvertSampleRate_Button_Click(sender As Object, e As EventArgs) Handles FixAmtestFiles_Button.Click

        'Dim InputFolder = "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\AMTEST_(SE)_MixIV\Media\Talker2-RVE\AMTEST-sounds-Step1-Names"
        'Dim OutputFolder = "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\AMTEST_(SE)_MixIV\Media\Talker2-RVE\AMTEST-sounds-Step2-Format"
        'Dim OutputFolder2 = "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\AMTEST_(SE)_MixIV\Media\Talker2-RVE\AMTEST-sounds-Step3-noMetadata"

        'Dim InputFolder = "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSpondees23\Media\Talker1-RVE\AMTEST-sounds-Step1-Names"
        'Dim OutputFolder = "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSpondees23\Media\Talker1-RVE\AMTEST-sounds-Step2-Format"
        'Dim OutputFolder2 = "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSpondees23\Media\Talker1-RVE\AMTEST-sounds-Step3-noMetadata"

        'Seconds verison, with nominal level of -21 dB FS
        'Dim InputFolder = "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSpondees23\Media\Talker1-RVE\AMTEST sounds new\Step2-Selected files"
        'Dim OutputFolder = "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSpondees23\Media\Talker1-RVE\AMTEST sounds new\Step3-Format"
        'Dim OutputFolder2 = "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSpondees23\Media\Talker1-RVE\AMTEST sounds new\Step4-noMetaData"

        Dim InputFolder = "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\AMTEST_(SE)_MixIV\Media\Talker2-RVE\AMTEST sounds new\Step2-Selected files"
        Dim OutputFolder = "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\AMTEST_(SE)_MixIV\Media\Talker2-RVE\AMTEST sounds new\Step3-Format"
        Dim OutputFolder2 = "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\AMTEST_(SE)_MixIV\Media\Talker2-RVE\AMTEST sounds new\Step4-noMetaData"


        SpeechTestFramework.Audio.AudioIOs.SamplerateConversion_DirectBatch(InputFolder, OutputFolder, New SpeechTestFramework.Audio.Formats.WaveFormat(44100, 32, 1, , SpeechTestFramework.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints))

        MsgBox("Finished converting format")

        Dim Files = IO.Directory.GetFiles(OutputFolder)

        For Each File In Files

            Dim LoadedSound = Audio.Sound.LoadWaveFile(File)
            LoadedSound.RemoveUnparsedWaveChunks()
            LoadedSound.SMA = Nothing
            LoadedSound.WriteWaveFile(IO.Path.Combine(OutputFolder2, IO.Path.GetFileName(File)))

        Next

        MsgBox("Finished removing chunks")

    End Sub

    Private Sub CreateAmtestCalibSign_Button_Click(sender As Object, e As EventArgs) Handles CreateAmtestCalibSign_Button.Click

        'Dim CalibrationLevel_dBFS = -25
        Dim CalibrationLevel_dBFS = -21
        Dim TemplateSound = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\AMTEST_(SE)_MixIV\Media\Talker2-RVE\AMTEST sounds new\Step4-noMetaData\bot.wav")
        Dim CalibrationSignalFolder = "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\AMTEST_(SE)_MixIV\Media\Talker2-RVE\AMTEST sounds new"
        Dim CalibrationSignalPath = Utils.GetSaveFilePath(CalibrationSignalFolder, "CalibrationSignal_" & CalibrationLevel_dBFS.ToString().Replace(",", ".") & "dB.wav")

        'Creates a standard calibration signal (frequency modulated sine wave)
        Dim CarrierFrequency As Double = 1000
        Dim ModulationFrequency As Double = 20
        Dim ModulationDepth As Double = 0.125
        Dim Duration As Double = 60

        Dim GeneratedWarble = Audio.GenerateSound.CreateFrequencyModulatedSineWave(TemplateSound.WaveFormat, , CarrierFrequency, 0.5, ModulationFrequency, ModulationDepth,, Duration)

        'Sets its level using Z-weighting even if some other weighting was used for the speech material 
        Audio.DSP.MeasureAndAdjustSectionLevel(GeneratedWarble, CalibrationLevel_dBFS,,,,)

        'Removes the SMA iXML chunk
        GeneratedWarble.SMA = Nothing

        'Stores the calibration signal
        GeneratedWarble.WriteWaveFile(CalibrationSignalPath)

        'Shows and stores information about the calibration signal
        Dim CalibrationSignalDescription = "The calibration signal (frequency modulated sine wave) in " & CalibrationSignalPath & " is frequency modulated around " & CarrierFrequency & " Hz by ±" & (ModulationDepth * 100).ToString & " %, with a modulation frequency of " & ModulationFrequency & " Hz. Samplerate: " & GeneratedWarble.WaveFormat.SampleRate & " Hz, duration: " & Duration & " seconds."
        Utils.SendInfoToLog(CalibrationSignalDescription, "Calibration signal info", IO.Path.GetDirectoryName(CalibrationSignalPath))
        MsgBox(CalibrationSignalDescription, MsgBoxStyle.Information, "Calibration signal info.")


    End Sub

    Private Sub Button36_Click(sender As Object, e As EventArgs) Handles Button36.Click

        Dim AddSpeech As Boolean = True
        'Dim Delays As New List(Of Double) From {0, 0.001, 0.005, 0.01, 0.015, 0.02, 0.025, 0.03, 0.035, 0.04, 0.045, 0.05, 0.055, 0.06, 0.065, 0.07, 0.075, 0.08, 0.1, 0.15, 0.2, 0.25, 0.3}
        Dim Delays As New List(Of Double) From {0, 0.2, 0.25, 0.3}
        Dim OutputDuration As Double = 20

        Dim OutputFolder As String = "C:\EriksDokument\SNODD-NoiseTest\Mixed"

        Dim InitialKernel = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\RoomImpulses\Data\ARC\ARC_Harcellen_KEMAR\48000Hz\UnspecifiedHeadphones\KEMAR_0_L.wav")

        'Dim OriginalNoise = Audio.GenerateSound.CreateWhiteNoise(InitialKernel.WaveFormat, 1, 0.5, 5)
        Dim OriginalNoise = Audio.Sound.LoadWaveFile("C:\EriksDokument\SNODD-NoiseTest\SpeechMaterialWeightedSNR_0.wav")
        Audio.DSP.CropSection(OriginalNoise, 1, OriginalNoise.WaveFormat.SampleRate * OutputDuration)

        'Fades the noise
        Audio.DSP.Fade(OriginalNoise, Nothing, 0, 1, 0, OriginalNoise.WaveFormat.SampleRate * 0.3, Audio.DSP.Transformations.FadeSlopeType.Linear)

        'Audio.DSP.MeasureAndAdjustSectionLevel(OriginalNoise, -25)
        'OriginalNoise.WriteWaveFile(IO.Path.Combine(OutputFolder, "OriginalNoise.wav"))

        Dim MyFftFormat As Audio.Formats.FftFormat = New Audio.Formats.FftFormat

        Dim AzimuthPairs As New List(Of Tuple(Of Integer, Integer))
        AzimuthPairs.Add(New Tuple(Of Integer, Integer)(90, 270))
        AzimuthPairs.Add(New Tuple(Of Integer, Integer)(120, 240))
        AzimuthPairs.Add(New Tuple(Of Integer, Integer)(150, 210))

        Dim FrontKernel_L = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\RoomImpulses\Data\ARC\ARC_Harcellen_KEMAR\48000Hz\UnspecifiedHeadphones\KEMAR_0_L.wav")
        Dim FilteredSound_Front_L = Audio.DSP.FIRFilter(OriginalNoise, FrontKernel_L, MyFftFormat,,,,,, True)
        'FilteredSound_Front_L.WriteWaveFile(IO.Path.Combine(OutputFolder, "FrontNoise-noise_KEMAR_L.wav"))

        Dim FrontKernel_R = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\RoomImpulses\Data\ARC\ARC_Harcellen_KEMAR\48000Hz\UnspecifiedHeadphones\KEMAR_0_R.wav")
        Dim FilteredSound_Front_R = Audio.DSP.FIRFilter(OriginalNoise, FrontKernel_R, MyFftFormat,,,,,, True)
        'FilteredSound_Front_R.WriteWaveFile(IO.Path.Combine(OutputFolder, "FrontNoise-noise_KEMAR_R.wav"))

        Dim BinarualFrontSound As New Audio.Sound(New Audio.Formats.WaveFormat(InitialKernel.WaveFormat.SampleRate, InitialKernel.WaveFormat.BitDepth, 2))
        BinarualFrontSound.WaveData.SampleData(1) = FilteredSound_Front_L.WaveData.SampleData(1)
        BinarualFrontSound.WaveData.SampleData(2) = FilteredSound_Front_R.WaveData.SampleData(1)
        'BinarualFrontSound.WriteWaveFile(IO.Path.Combine(OutputFolder, "BinarualFrontSound_KEMAR.wav"))

        Dim SpeechSignal_L As Audio.Sound = Nothing
        Dim SpeechSignal_R As Audio.Sound = Nothing
        If AddSpeech = True Then
            'Loading speech
            SpeechSignal_L = Audio.Sound.LoadWaveFile("C:\EriksDokument\SNODD-NoiseTest\Talker2_TP.wav")
            SpeechSignal_R = SpeechSignal_L.CreateSoundDataCopy

            'Filterrung speech
            SpeechSignal_L = Audio.DSP.FIRFilter(SpeechSignal_L, FrontKernel_L, MyFftFormat,,,,,, True)
            SpeechSignal_R = Audio.DSP.FIRFilter(SpeechSignal_R, FrontKernel_R, MyFftFormat,,,,,, True)

        End If

        For Each Delay In Delays

            For Each AzimuthPair In AzimuthPairs

                Dim Kernel1_L = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\RoomImpulses\Data\ARC\ARC_Harcellen_KEMAR\48000Hz\UnspecifiedHeadphones\KEMAR_" & AzimuthPair.Item1 & "_L.wav")
                Dim Kernel2_L = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\RoomImpulses\Data\ARC\ARC_Harcellen_KEMAR\48000Hz\UnspecifiedHeadphones\KEMAR_" & AzimuthPair.Item2 & "_L.wav")

                Dim Kernel1_R = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\RoomImpulses\Data\ARC\ARC_Harcellen_KEMAR\48000Hz\UnspecifiedHeadphones\KEMAR_" & AzimuthPair.Item1 & "_R.wav")
                Dim Kernel2_R = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\RoomImpulses\Data\ARC\ARC_Harcellen_KEMAR\48000Hz\UnspecifiedHeadphones\KEMAR_" & AzimuthPair.Item2 & "_R.wav")

                'Applies FIR-filtering
                Dim FilteredSound1_L = Audio.DSP.FIRFilter(OriginalNoise, Kernel1_L, MyFftFormat,,,,,, True)
                Dim FilteredSound2_L = Audio.DSP.FIRFilter(OriginalNoise, Kernel2_L, MyFftFormat,,,,,, True)

                Dim FilteredSound1_R = Audio.DSP.FIRFilter(OriginalNoise, Kernel1_R, MyFftFormat,,,,,, True)
                Dim FilteredSound2_R = Audio.DSP.FIRFilter(OriginalNoise, Kernel2_R, MyFftFormat,,,,,, True)

                'Delaying sound 1 (the right side sound)
                Audio.DSP.ShiftSection(FilteredSound1_L, Delay * Kernel1_L.WaveFormat.SampleRate)
                Audio.DSP.ShiftSection(FilteredSound1_R, Delay * Kernel1_R.WaveFormat.SampleRate)

                Dim CombinedSound_L As Audio.Sound
                Dim CombinedSound_R As Audio.Sound

                If AddSpeech = True Then
                    Dim TrimmedSpeechCopy_L = SpeechSignal_L.CreateSoundDataCopy
                    Audio.DSP.CropSection(TrimmedSpeechCopy_L, 0, FilteredSound1_L.WaveData.SampleData(1).Length)

                    Dim TrimmedSpeechCopy_R = SpeechSignal_R.CreateSoundDataCopy
                    Audio.DSP.CropSection(TrimmedSpeechCopy_R, 0, FilteredSound1_R.WaveData.SampleData(1).Length)

                    CombinedSound_L = Audio.DSP.SuperpositionEqualLengthSounds(New List(Of Audio.Sound) From {FilteredSound1_L, FilteredSound2_L, TrimmedSpeechCopy_L})
                    'CombinedSound_L.WriteWaveFile(IO.Path.Combine(OutputFolder, "SNODD-noise_KEMAR_L_" & AzimuthPair.Item1 & "_" & AzimuthPair.Item2 & "_" & 1000 * Delay & "ms_" & ".wav"))

                    CombinedSound_R = Audio.DSP.SuperpositionEqualLengthSounds(New List(Of Audio.Sound) From {FilteredSound1_R, FilteredSound2_R, TrimmedSpeechCopy_R})
                    'CombinedSound_R.WriteWaveFile(IO.Path.Combine(OutputFolder, "SNODD-noise_KEMAR_R_" & AzimuthPair.Item1 & "_" & AzimuthPair.Item2 & "_" & 1000 * Delay & "ms_" & ".wav"))
                Else
                    CombinedSound_L = Audio.DSP.SuperpositionEqualLengthSounds(New List(Of Audio.Sound) From {FilteredSound1_L, FilteredSound2_L})
                    'CombinedSound_L.WriteWaveFile(IO.Path.Combine(OutputFolder, "SNODD-noise_KEMAR_L_" & AzimuthPair.Item1 & "_" & AzimuthPair.Item2 & "_" & 1000 * Delay & "ms_" & ".wav"))

                    CombinedSound_R = Audio.DSP.SuperpositionEqualLengthSounds(New List(Of Audio.Sound) From {FilteredSound1_R, FilteredSound2_R})
                    'CombinedSound_R.WriteWaveFile(IO.Path.Combine(OutputFolder, "SNODD-noise_KEMAR_R_" & AzimuthPair.Item1 & "_" & AzimuthPair.Item2 & "_" & 1000 * Delay & "ms_" & ".wav"))

                End If

                Dim BinarualSnoddSound As New Audio.Sound(BinarualFrontSound.WaveFormat)
                BinarualSnoddSound.WaveData.SampleData(1) = CombinedSound_L.WaveData.SampleData(1)
                BinarualSnoddSound.WaveData.SampleData(2) = CombinedSound_R.WaveData.SampleData(1)
                BinarualSnoddSound.WriteWaveFile(IO.Path.Combine(OutputFolder, "BinarualSNODD-noise_KEMAR_" & AzimuthPair.Item1 & "_" & AzimuthPair.Item2 & "_" & 1000 * Delay & "ms_" & ".wav"))

            Next
        Next

    End Sub

    Private Sub Button37_Click(sender As Object, e As EventArgs) Handles Button37.Click

        Dim TimeWeighting As Double = 0.125

        Dim SwedishTiBCalibSignal = Audio.Sound.LoadWaveFile("C:\Temp4\CalibrationSignal_TiB_Cropped.wav")

        Dim SpondeesList2 = Audio.Sound.LoadWaveFile("C:\Temp4\SpondeLista2_NoIntroduction.wav")
        Dim SpondeesList3 = Audio.Sound.LoadWaveFile("C:\Temp4\SpondeLista3_NoIntroduction.wav")
        Dim SpondeesList4 = Audio.Sound.LoadWaveFile("C:\Temp4\SpondeLista4_NoIntroduction.wav")

        Dim SpondeeList2WithoutSilence = Audio.Sound.LoadWaveFile("C:\Temp4\SpondeLista2_UtanMellanrum.wav")
        Dim SpondeeList3WithoutSilence = Audio.Sound.LoadWaveFile("C:\Temp4\SpondeLista3_UtanMellanrum.wav")
        Dim SpondeeList4WithoutSilence = Audio.Sound.LoadWaveFile("C:\Temp4\SpondeLista4_UtanMellanrum.wav")

        Dim SwedishTiBCalibSignalLevel = Audio.DSP.MeasureSectionLevel(SwedishTiBCalibSignal, 1)
        Dim List2_SpeechLevelC = Audio.DSP.MeasureSectionLevel(SpondeeList2WithoutSilence, 1,,,,, Audio.BasicAudioEnums.FrequencyWeightings.C)
        Dim List3_SpeechLevelC = Audio.DSP.MeasureSectionLevel(SpondeeList3WithoutSilence, 1,,,,, Audio.BasicAudioEnums.FrequencyWeightings.C)
        Dim List4_SpeechLevelC = Audio.DSP.MeasureSectionLevel(SpondeeList4WithoutSilence, 1,,,,, Audio.BasicAudioEnums.FrequencyWeightings.C)

        Dim List2_SpeechLevelC_TimeWeighted = Audio.DSP.GetLevelOfLoudestWindow(SpondeesList2, 1, TimeWeighting * SpondeesList2.WaveFormat.SampleRate,,,, Audio.BasicAudioEnums.FrequencyWeightings.C)
        Dim List3_SpeechLevelC_TimeWeighted = Audio.DSP.GetLevelOfLoudestWindow(SpondeesList3, 1, TimeWeighting * SpondeesList3.WaveFormat.SampleRate,,,, Audio.BasicAudioEnums.FrequencyWeightings.C)
        Dim List4_SpeechLevelC_TimeWeighted = Audio.DSP.GetLevelOfLoudestWindow(SpondeesList4, 1, TimeWeighting * SpondeesList4.WaveFormat.SampleRate,,,, Audio.BasicAudioEnums.FrequencyWeightings.C)

        Dim List2Peak = Audio.DSP.MeasureSectionLevel(SpondeesList2, 1,,,, Audio.AudioManagement.SoundMeasurementType.AbsolutePeakAmplitude, Audio.BasicAudioEnums.FrequencyWeightings.C)
        Dim List3Peak = Audio.DSP.MeasureSectionLevel(SpondeesList3, 1,,,, Audio.AudioManagement.SoundMeasurementType.AbsolutePeakAmplitude, Audio.BasicAudioEnums.FrequencyWeightings.C)
        Dim List4Peak = Audio.DSP.MeasureSectionLevel(SpondeesList4, 1,,,, Audio.AudioManagement.SoundMeasurementType.AbsolutePeakAmplitude, Audio.BasicAudioEnums.FrequencyWeightings.C)

        MsgBox("List 2 Level = " & Math.Round(List2_SpeechLevelC.Value, 1) & " dB C (" & Math.Round(List2_SpeechLevelC.Value - SwedishTiBCalibSignalLevel.Value, 1) & " dB above calibration level)" & vbCrLf &
            "List 3 Level = " & Math.Round(List3_SpeechLevelC.Value, 1) & " dB C (" & Math.Round(List3_SpeechLevelC.Value - SwedishTiBCalibSignalLevel.Value, 1) & " dB above calibration level)" & vbCrLf &
            "List 4 Level = " & Math.Round(List4_SpeechLevelC.Value, 1) & " dB C (" & Math.Round(List4_SpeechLevelC.Value - SwedishTiBCalibSignalLevel.Value, 1) & " dB above calibration level)" & vbCrLf &
            "List 2 Level (TW: " & 1000 * TimeWeighting & "ms) = " & Math.Round(List2_SpeechLevelC_TimeWeighted, 1) & " dB C (" & Math.Round(List2_SpeechLevelC_TimeWeighted - SwedishTiBCalibSignalLevel.Value, 1) & " dB above calibration level)" & vbCrLf &
            "List 3 Level (TW: " & 1000 * TimeWeighting & "ms) = " & Math.Round(List3_SpeechLevelC_TimeWeighted, 1) & " dB C (" & Math.Round(List3_SpeechLevelC_TimeWeighted - SwedishTiBCalibSignalLevel.Value, 1) & " dB above calibration level)" & vbCrLf &
            "List 4 Level (TW: " & 1000 * TimeWeighting & "ms) = " & Math.Round(List4_SpeechLevelC_TimeWeighted, 1) & " dB C (" & Math.Round(List4_SpeechLevelC_TimeWeighted - SwedishTiBCalibSignalLevel.Value, 1) & " dB above calibration level)" & vbCrLf &
            "List 2 Level (Peak) = " & Math.Round(List2Peak.Value, 1) & " dB C (" & Math.Round(List2Peak.Value - SwedishTiBCalibSignalLevel.Value, 1) & " dB above calibration level)" & vbCrLf &
            "List 3 Level (Peak) = " & Math.Round(List3Peak.Value, 1) & " dB C (" & Math.Round(List3Peak.Value - SwedishTiBCalibSignalLevel.Value, 1) & " dB above calibration level)" & vbCrLf &
            "List 4 Level (Peak) = " & Math.Round(List4Peak.Value, 1) & " dB C (" & Math.Round(List4Peak.Value - SwedishTiBCalibSignalLevel.Value, 1) & " dB above calibration level)" & vbCrLf &
               "SwedishTiBCalibSignalLevel  = " & Math.Round(SwedishTiBCalibSignalLevel.Value, 1) & " dB FS")

    End Sub

    Private Sub Button38_Click(sender As Object, e As EventArgs) Handles Button38.Click

        Utils.CompareFiles("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\AMTEST_(SE)_MixIV - NominalLevel-25dB\Media\Talker2-RVE\ContralateralMaskers\AMTEST_(SE)_MixIV_AMTEST_(SE)_MixIV\SpeechMaterialWeightedSNR_0.wav",
                           "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishTP50\Media\Talker2-RVE\ContralateralMaskers\SwedishTP50_SwedishTP50\SpeechMaterialWeightedSNR_0.wav", Utils.GeneralIO.FileComparisonMethods.CompareWaveFileData, False, True)

        Utils.CompareFiles("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\AMTEST_(SE)_MixIV - NominalLevel-25dB\Media\Talker2-RVE\ContralateralMaskers\AMTEST_(SE)_MixIV_AMTEST_(SE)_MixIV\SpeechMaterialWeightedSNR_0.wav",
                           "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishTP50\Media\Talker2-RVE\ContralateralMaskers\SwedishTP50_SwedishTP50\SpeechMaterialWeightedSNR_0.wav", Utils.GeneralIO.FileComparisonMethods.CompareBytes, False, True)

        Utils.CompareFiles("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\AMTEST_(SE)_MixIV - NominalLevel-25dB\Media\Talker2-RVE\ContralateralMaskers\AMTEST_(SE)_MixIV_AMTEST_(SE)_MixIV\SpeechMaterialWeightedSNR_0.wav",
                           "C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishTP50\Media\Talker2-RVE\ContralateralMaskers\SwedishTP50_SwedishTP50\SpeechMaterialWeightedSNR_0.wav", Utils.GeneralIO.FileComparisonMethods.CompareBits, False, True)

    End Sub

    Private Sub Button39_Click(sender As Object, e As EventArgs) Handles Button39.Click

        'This peice of code was used to change the masker signal from using a nominal level of -25 dBC to -21 dBC
        Dim OutputFolder = IO.Path.Combine(Utils.logFilePath, "Noises-21dB")

        'Dim InputSound = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishTP50\Media\Talker2-RVE\ContralateralMaskers\SwedishTP50_SwedishTP50\SpeechMaterialWeightedSNR_0.wav")
        'Dim InputSound = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishTP50\Media\Talker1-RVE\ContralateralMaskers\SwedishTP50_SwedishTP50\SpeechMaterialWeightedSNR_0.wav")
        'Dim InputSound = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSpondees23\Media\Talker1-RVE\ContralateralMaskers\Swedish_Spondees_23_Swedish_Spondees_23\SpeechMaterialWeightedSNR_0.wav")
        Dim InputSound = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSpondees23\Media\Talker2-RVE\ContralateralMaskers\Swedish_Spondees_23_Swedish_Spondees_23\SpeechMaterialWeightedSNR_0.wav")

        'Dim WindowLevels As New List(Of Double)
        'Dim TimeWeightedLevel = Audio.DSP.GetLevelOfLoudestWindow(InputSound, 1, InputSound.WaveFormat.SampleRate * 0.125,, , , Audio.BasicAudioEnums.FrequencyWeightings.C, True, WindowLevels)
        'Dim AverageWindowLevel = WindowLevels.Average 'This is approximately the same as the measurement below without time weighting

        Dim NoiseLevel = Audio.DSP.MeasureSectionLevel(InputSound, 1,,, ,, Audio.BasicAudioEnums.FrequencyWeightings.C)

        Dim TargetLevel As Double = -21
        Dim NeededGain As Double = TargetLevel - NoiseLevel

        Audio.DSP.AmplifySection(InputSound, NeededGain)

        InputSound.SMA.NominalLevel = TargetLevel
        InputSound.SMA.InferNominalLevelToAllDescendants()
        InputSound.SMA.SetFrequencyWeighting(Audio.BasicAudioEnums.FrequencyWeightings.C, True)
        InputSound.SMA.MeasureSoundLevels()

        InputSound.WriteWaveFile(IO.Path.Combine(OutputFolder, "SpeechMaterialWeightedSNR_0.wav"))

    End Sub

    Private Sub Button40_Click(sender As Object, e As EventArgs) Handles Button40.Click

        'This code was used to determine the differences in relation to the calibration signal that was caused in the Spondee and TP materials when changing from speech level measurments without time weighting to using time weighted (125 ms) speech level definitions.

        Dim FS_SPL_Diff As Double = 100
        Dim NomLev1 As Double = -25 + FS_SPL_Diff
        Dim NomLev2 As Double = -21 + FS_SPL_Diff
        Dim NomLevDiff As Double = NomLev2 - NomLev1

        Dim InputSound_21_SP_T1 = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSpondees23\Media\Talker1-RVE\TestWordRecordings\L00_Lista_1\Sound00.wav")
        Dim InputSound_25_SP_T1 = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSpondees23 - backup - 2024-11-15 - Protocol B4 version\Media\Talker1-RVE\TestWordRecordings\L00_Lista_1\Sound00.wav")
        Dim SpeechLevel_21_SP_T1 = Audio.DSP.MeasureSectionLevel(InputSound_21_SP_T1, 1,,, ,, Audio.BasicAudioEnums.FrequencyWeightings.C) + FS_SPL_Diff
        Dim SpeechLevel_25_SP_T1 = Audio.DSP.MeasureSectionLevel(InputSound_25_SP_T1, 1,,, ,, Audio.BasicAudioEnums.FrequencyWeightings.C) + FS_SPL_Diff
        Dim SpeechLevelDifference_SP_T1 As Double = SpeechLevel_25_SP_T1 - (SpeechLevel_21_SP_T1 - NomLevDiff)

        Dim InputSound_21_SP_T2 = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSpondees23\Media\Talker2-RVE\TestWordRecordings\L00_Lista_1\Sound00.wav")
        Dim InputSound_25_SP_T2 = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishSpondees23 - backup - 2024-11-15 - Protocol B4 version\Media\Talker2-RVE\TestWordRecordings\L00_Lista_1\Sound00.wav")
        Dim SpeechLevel_21_SP_T2 = Audio.DSP.MeasureSectionLevel(InputSound_21_SP_T2, 1,,, ,, Audio.BasicAudioEnums.FrequencyWeightings.C) + FS_SPL_Diff
        Dim SpeechLevel_25_SP_T2 = Audio.DSP.MeasureSectionLevel(InputSound_25_SP_T2, 1,,, ,, Audio.BasicAudioEnums.FrequencyWeightings.C) + FS_SPL_Diff
        Dim SpeechLevelDifference_SP_T2 As Double = SpeechLevel_25_SP_T2 - (SpeechLevel_21_SP_T2 - NomLevDiff)

        Dim InputSound_21_TP_T1 = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishTP50\Media\Talker1-RVE\TestWordRecordings\L00S00_Sentence00\Sound00.wav")
        Dim InputSound_25_TP_T1 = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishTP50 - backup 2024-11-15 - Protocol B2 version\Media\Talker1-RVE\TestWordRecordings\L00S00_Sentence00\Sound00.wav")
        Dim SpeechLevel_21_TP_T1 = Audio.DSP.MeasureSectionLevel(InputSound_21_TP_T1, 1,,, ,, Audio.BasicAudioEnums.FrequencyWeightings.C) + FS_SPL_Diff
        Dim SpeechLevel_25_TP_T1 = Audio.DSP.MeasureSectionLevel(InputSound_25_TP_T1, 1,,, ,, Audio.BasicAudioEnums.FrequencyWeightings.C) + FS_SPL_Diff
        Dim SpeechLevelDifference_TP_T1 As Double = SpeechLevel_25_TP_T1 - (SpeechLevel_21_TP_T1 - NomLevDiff)

        Dim InputSound_21_TP_T2 = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishTP50\Media\Talker2-RVE\TestWordRecordings\L00S00_Sentence00\Sound00.wav")
        Dim InputSound_25_TP_T2 = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishTP50 - backup 2024-11-15 - Protocol B2 version\Media\Talker2-RVE\TestWordRecordings\L00S00_Sentence00\Sound00.wav")
        Dim SpeechLevel_21_TP_T2 = Audio.DSP.MeasureSectionLevel(InputSound_21_TP_T2, 1,,, ,, Audio.BasicAudioEnums.FrequencyWeightings.C) + FS_SPL_Diff
        Dim SpeechLevel_25_TP_T2 = Audio.DSP.MeasureSectionLevel(InputSound_25_TP_T2, 1,,, ,, Audio.BasicAudioEnums.FrequencyWeightings.C) + FS_SPL_Diff
        Dim SpeechLevelDifference_TP_T2 As Double = SpeechLevel_25_TP_T2 - (SpeechLevel_21_TP_T2 - NomLevDiff)

        MsgBox("Difference spondees - Talker1: " & Math.Round(SpeechLevelDifference_SP_T1, 1) & " dBC" & vbCrLf & vbCrLf &
            "Difference spondees - Talker2: " & Math.Round(SpeechLevelDifference_SP_T2, 1) & " dBC" & vbCrLf & vbCrLf &
               "Difference TP  - Talker1: " & Math.Round(SpeechLevelDifference_TP_T1, 1) & " dBC" & vbCrLf & vbCrLf &
               "Difference TP  - Talker2: " & Math.Round(SpeechLevelDifference_TP_T2, 1) & " dBC")

    End Sub

    Private Sub Button42_Click(sender As Object, e As EventArgs) Handles Button42.Click

        'Dim TestFile = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishHINT\Media\Standard_Sentences\TestWordRecordings\L00S00_Sentence00\Sound00.wav")

        Dim InputFile = Audio.Sound.LoadWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishHINT\Media\Standard_Sentences\Maskers\HINT_Swedish_HINT\List #01female only noise.wav")

        InputFile = InputFile.Convert16to32bitSound()

        InputFile.SMA.NominalLevel = -20
        InputFile.SMA.InferNominalLevelToAllDescendants()

        InputFile.SMA.SetFrequencyWeighting(Audio.BasicAudioEnums.FrequencyWeightings.C, True)
        InputFile.SMA.MeasureSoundLevels()

        InputFile.WriteWaveFile("C:\EriksDokument\source\repos\OSTF\OSTFMedia\SpeechMaterials\SwedishHINT\Media\Standard_Sentences\Maskers\HINT_Swedish_HINT\List #01female only noise_withSMA.wav")

    End Sub
End Class
