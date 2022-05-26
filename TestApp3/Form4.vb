Public Class Form4
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'ResponseGuiItemTable1.AdjustControls()


        Dim settings = New SpeechTestFramework.OstfSettings 'TODO: these should be read from text file!

        Dim CompleteSpeechMaterial = SpeechTestFramework.SpeechMaterialComponent.LoadSpeechMaterial(settings.SpeechMaterialComponentsPath)

        Dim NewMediaSet = New SpeechTestFramework.MediaSet
        NewMediaSet.SetSipValues()

        NewMediaSet.RecordAndEditAudioMediaFiles(CompleteSpeechMaterial, SpeechTestFramework.MediaSet.SpeechMaterialRecorderLoadOptions.LoadAllSounds)

        'NewMediaSet.CreateLackingAudioMediaFiles(CompleteSpeechMaterial)


        Exit Sub

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

        Exit Sub


    End Sub


End Class