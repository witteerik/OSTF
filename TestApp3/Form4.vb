Public Class Form4

    Dim SoundPlayer As SpeechTestFramework.Audio.PortAudioVB.OverlappingSoundPlayer

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'ResponseGuiItemTable1.AdjustControls()

        Dim test As Integer = 1
        Select Case test
            Case 0

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


            Case 1

                Dim settings = New SpeechTestFramework.OstfSettings 'TODO: these should be read from text file!

                Dim CompleteSpeechMaterial = SpeechTestFramework.SpeechMaterialComponent.LoadSpeechMaterial(settings.SpeechMaterialComponentsPath)

                'CompleteSpeechMaterial.WriteSpeechMaterialComponenFile(SpeechTestFramework.Utils.logFilePath & "TestSMC.txt")

                Dim NewMediaSet = New SpeechTestFramework.MediaSet
                NewMediaSet.SetSipValues(1)

                'NewMediaSet.CopySoundFiles(CompleteSpeechMaterial, IO.Path.Combine(SpeechTestFramework.Utils.logFilePath, "MediaSet2"))

                NewMediaSet.RecordAndEditAudioMediaFiles(CompleteSpeechMaterial, SpeechTestFramework.MediaSet.SpeechMaterialRecorderLoadOptions.LoadAllSounds, SpeechTestFramework.MediaSet.PrototypeRecordingOptions.None)

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

End Class