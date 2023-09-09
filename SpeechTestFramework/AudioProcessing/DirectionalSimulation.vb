

Imports SpeechTestFramework.Audio.PortAudioVB.DuplexMixer

Public Class DirectionalSimulation

    Private UnsortedKernels As Audio.Sound
    Private StereoKernels As SortedList(Of Integer, Audio.Sound)

    Public Sub New(ByVal KernelPath As String, ByVal IrDatabase As SupportedIrDatabases)

        LoadDirectionalKernels(KernelPath, IrDatabase)

    End Sub


    Private Sub LoadDirectionalKernels(ByVal KernelPath As String, ByVal IrDatabase As SupportedIrDatabases)

        Select Case IrDatabase
            Case SupportedIrDatabases.wierstorf2011

                Try

                    'Loading directional filter kernels (These are 16 bit, 44100 Hz. Converting them to IEEE 32 bit, 44100 Hz)        
                    UnsortedKernels = Audio.Sound.LoadWaveFile(KernelPath)

                    'Converting to IEEE 32-bit
                    Dim ConvertedSound As Audio.Sound = New Audio.Sound(New Audio.Formats.WaveFormat(UnsortedKernels.WaveFormat.SampleRate, 32, UnsortedKernels.WaveFormat.Channels))

                    'Copying data
                    Dim Orig_FS As Integer = UnsortedKernels.WaveFormat.PositiveFullScale
                    For c = 1 To ConvertedSound.WaveFormat.Channels
                        Dim SourceArray = UnsortedKernels.WaveData.SampleData(c)
                        Dim ChannelArray(SourceArray.Length - 1) As Single

                        For s = 0 To SourceArray.Length - 1
                            ChannelArray(s) = SourceArray(s) / Orig_FS
                        Next

                        ConvertedSound.WaveData.SampleData(c) = ChannelArray
                    Next

                    'Replacing Kernels with the ConvertedSound
                    UnsortedKernels = ConvertedSound

                    'Sorting the kernels into a lookup
                    StereoKernels = New SortedList(Of Integer, Audio.Sound)

                    For n = 0 To 360 - 1

                        Dim Angle As Integer = n

                        If Angle > 180 Then Angle = Angle - 360

                        Dim RightChannelIndex As Integer = 1 + n * 2

                        Dim NewSound As New Audio.Sound(GetStereoKernelFormat(UnsortedKernels.WaveFormat))
                        NewSound.WaveData.SampleData(1) = UnsortedKernels.WaveData.SampleData(RightChannelIndex + 1)
                        NewSound.WaveData.SampleData(2) = UnsortedKernels.WaveData.SampleData(RightChannelIndex)

                        StereoKernels.Add(Angle, NewSound)

                    Next

                    'Adding also -180 degrees (by referencing 180 degrees)
                    StereoKernels.Add(-180, StereoKernels(180))

                Catch ex As Exception
                    MsgBox("Directional kernel sound conversions failed! " & vbCrLf & ex.ToString)
                End Try

            Case SupportedIrDatabases.ARC_Harcellen_KEMAR

                'Loading directional filter kernels from wave files        
                StereoKernels = New SortedList(Of Integer, Audio.Sound)

                For n = 0 To 360 - 1 Step 30

                    Dim Angle As Integer = n

                    If Angle > 180 Then Angle = Angle - 360

                    Dim LeftKernelFilePath = IO.Path.Combine(KernelPath, "KEMAR_" & n.ToString & "_L.wav")
                    Dim RightKernelFilePath = IO.Path.Combine(KernelPath, "KEMAR_" & n.ToString & "_R.wav")

                    If IO.File.Exists(LeftKernelFilePath) = False Then
                        Throw New Exception("Missing impulse response file for directional simulation:" & LeftKernelFilePath)
                    End If
                    If IO.File.Exists(RightKernelFilePath) = False Then
                        Throw New Exception("Missing impulse response file for directional simulation:" & RightKernelFilePath)
                    End If

                    Dim LeftKernel = Audio.Sound.LoadWaveFile(LeftKernelFilePath)
                    Dim RightKernel = Audio.Sound.LoadWaveFile(RightKernelFilePath)

                    Dim NewSound As New Audio.Sound(GetStereoKernelFormat(LeftKernel.WaveFormat))
                    NewSound.WaveData.SampleData(1) = LeftKernel.WaveData.SampleData(1)
                    NewSound.WaveData.SampleData(2) = RightKernel.WaveData.SampleData(1)

                    StereoKernels.Add(Angle, NewSound)

                Next

                'Adding also -180 degrees (by referencing 180 degrees)
                StereoKernels.Add(-180, StereoKernels(180))

        End Select

    End Sub

    Public Function GetStereoKernel(ByVal Angle As Integer) As Audio.Sound

        'Retrieves the appropriate kernels
        If StereoKernels.ContainsKey(Angle) Then
            Return StereoKernels(Angle)
        Else

            'TODO: here we could round to the closest available direction, and store the value used in a referenced argument.

            Throw New Exception("The angle " & Angle & " degrees cannot be found in the directional simulator!")
        End If


    End Function

    Public Function GetStereoKernelFormat(ByRef ModelFormat As Audio.Formats.WaveFormat) As Audio.Formats.WaveFormat

        Return New Audio.Formats.WaveFormat(ModelFormat.SampleRate,
                                            ModelFormat.BitDepth, 2,,
                                            ModelFormat.Encoding)

    End Function

End Class