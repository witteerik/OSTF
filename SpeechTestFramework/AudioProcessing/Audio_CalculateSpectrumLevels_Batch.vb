
Namespace Audio.DSP

    Public Module AudioBatchFunctions

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SoundChannel">The sound file channel to analyze</param>
        Public Sub CalculateSpectrumLevelsOfSoundFiles(Optional ByVal SoundChannel As Integer = 1)

            Dim SoundFolder As String = ""

            Dim fbd As New Windows.Forms.FolderBrowserDialog
            fbd.Description = "Select a folder containing wave files directly in the folder or in a set of subfolders"
            If fbd.ShowDialog() = Windows.Forms.DialogResult.OK Then
                SoundFolder = fbd.SelectedPath
            Else
                MsgBox("No folder selected. Quitting!", MsgBoxStyle.Exclamation, "Folder not found")
                Exit Sub
            End If

            If System.IO.Directory.Exists(SoundFolder) = False Then
                MsgBox("The specified folder could not be found. Quitting!", MsgBoxStyle.Exclamation, "Folder not found")
                Exit Sub
            End If

            'Getting all files including all subdirectories
            Dim Files = Utils.GeneralIO.GetFilesIncludingAllSubdirectories(SoundFolder)
            Dim SoundFiles As New List(Of String)

            For Each File In Files
                If IO.Path.GetExtension(File) = ".wav" Then
                    SoundFiles.Add(File)
                End If
            Next

            Dim BandBank = Audio.DSP.BandBank.GetSiiCriticalRatioBandBank
            Dim ReusableFftFormat As Audio.Formats.FftFormat = Nothing

            Dim MeasurementResults As New List(Of String)
            Dim UnreadableSounds As New List(Of String)

            'Adding headings
            Dim Headings As New List(Of String)
            Headings.Add("File")
            For i = 0 To BandBank.Count - 1
                Headings.Add(Math.Round(BandBank(i).CentreFrequency).ToString("00000"))
            Next
            MeasurementResults.Add(String.Join(vbTab, Headings))

            'Looping through sound files
            For Each SoundFilePath In SoundFiles

                'Creating a variable to hold results
                Dim SoundFileSpectrumLevels As List(Of Double) = Nothing

                Dim CurrentSoundFile = Audio.Sound.LoadWaveFile(SoundFilePath)
                If CurrentSoundFile IsNot Nothing Then
                    'Calculating spectrum levels
                    SoundFileSpectrumLevels = Audio.DSP.CalculateSpectrumLevels(CurrentSoundFile, SoundChannel, BandBank, ReusableFftFormat,,, Audio.Standard_dBFS_dBSPL_Difference)
                Else
                    'Adding NaNs for sounds not readable
                    UnreadableSounds.Add(SoundFilePath)
                    SoundFileSpectrumLevels = New List(Of Double)
                    For i = 0 To BandBank.Count - 1
                        SoundFileSpectrumLevels.Add(Double.NaN)
                    Next
                End If

                'Adding the filepath and the resulting spectrum levels
                MeasurementResults.Add(SoundFilePath & vbTab & String.Join(vbTab, SoundFileSpectrumLevels))

            Next

            'Reporting unreadable sounds
            If UnreadableSounds.Count > 0 Then
                MsgBox("The following " & UnreadableSounds.Count & " sound files could not be read:" & vbCrLf & String.Join(vbCrLf, UnreadableSounds))
                Utils.SendInfoToLog("Unreadable sound files when calculating spectum levels:" & vbCrLf & String.Join(vbCrLf, UnreadableSounds), "UnreadableSoundFiles_BatchSpectrumLevels")
            Else
                MsgBox("All " & SoundFiles.Count & " sounds were sucessfully measured. Please click next to save the measurements results to file.")
            End If

            'Getting the output file
            Dim OutputFile = Utils.GeneralIO.GetSaveFilePath(SoundFolder, "SpectrumLevels", {".txt"}, "Save spectrum levels to file")

            If OutputFile <> "" Then
                'Saving to file
                Utils.SendInfoToLog(String.Join(vbCrLf, MeasurementResults), IO.Path.GetFileNameWithoutExtension(OutputFile), IO.Path.GetDirectoryName(OutputFile), True, True, True)
                MsgBox("Measurement results were saved to the file: " & OutputFile, MsgBoxStyle.Information, "Finished")
            Else
                MsgBox("No file selected. Quitting")
            End If

        End Sub

    End Module



End Namespace
