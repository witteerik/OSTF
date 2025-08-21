Imports System.Runtime.CompilerServices
Imports STFN.Core
Imports STFN.Core.Audio.Sound.SpeechMaterialAnnotation

'This file contains extension methods for STFN.Core.StereoKernel (for directional simulation)

Partial Public Module Extensions


    <Extension>
    Private Function CalculateZeroPhaseEquivalentKernel(obj As StereoKernel) As StereoKernel

        'Gets the wave format
        Dim MonoWaveFormat = New STFN.Core.Audio.Formats.WaveFormat(obj.BinauralIR.WaveFormat.SampleRate, obj.BinauralIR.WaveFormat.BitDepth, 1,, obj.BinauralIR.WaveFormat.Encoding)

        'Gets a copy of the BinauralIR
        Dim TempIrCopy = obj.BinauralIR.CreateSoundDataCopy

        'Creating a new stereo kernel, with references to some object in the parent kernel
        Dim ZeroPhaseEquivalent = New StereoKernel With {.Name = obj.Name & "_ZPE", .Point = obj.Point, .BinauralIR = New STFN.Core.Audio.Sound(obj.BinauralIR.WaveFormat)}

        For Channel = 1 To 2

            Dim IrChannelCopy As New STFN.Core.Audio.Sound(MonoWaveFormat)

            IrChannelCopy.WaveData.SampleData(1) = TempIrCopy.WaveData.SampleData(Channel)

            Dim KernelSize = IrChannelCopy.WaveData.SampleData(1).Length

            Dim ZeroPhaseIR = DSP.GetImpulseResponseFromSound(IrChannelCopy, New STFN.Core.Audio.Formats.FftFormat(2 ^ 12), KernelSize * 2)

            'Use both to filter a sound and add gain to ZeroPhaseIR based on the resulting level difference

            'Notes the length of the IR
            Dim IrLength As Integer = IrChannelCopy.WaveData.SampleData(1).Length

            'Creates a delta pulse measurement sound
            Dim DeltaPulse_TestSound = DSP.CreateSilence(MonoWaveFormat,, IrLength * 5, STFN.Core.Audio.BasicAudioEnums.TimeUnits.samples)
            DeltaPulse_TestSound.WaveData.SampleData(1)(IrLength * 2) = 1

            'Measures the pre filter level
            Dim PreLevel As Double = DSP.MeasureSectionLevel(DeltaPulse_TestSound, 1, IrLength, 3 * IrLength)

            'Runs convolution
            Dim IR_ConvolutedSound = DSP.FIRFilter(DeltaPulse_TestSound, IrChannelCopy, New STFN.Core.Audio.Formats.FftFormat, ,,,,, True)
            Dim ZP_IR_ConvolutedSound = DSP.FIRFilter(DeltaPulse_TestSound, ZeroPhaseIR, New STFN.Core.Audio.Formats.FftFormat, ,,,,, True)

            'Gets the post-convolution level
            Dim PostLevel_IR = DSP.MeasureSectionLevel(IR_ConvolutedSound, 1, IrLength, 3 * IrLength)
            Dim PostLevel_ZP_IR = DSP.MeasureSectionLevel(ZP_IR_ConvolutedSound, 1, IrLength, 3 * IrLength)

            'Calculates the gain
            Dim IR_FilterGain As Double = PostLevel_IR - PreLevel
            Dim ZP_IR_FilterGain As Double = PostLevel_ZP_IR - PreLevel

            Dim Difference = ZP_IR_FilterGain - IR_FilterGain

            DSP.AmplifySection(ZeroPhaseIR, -Difference)

            'ZeroPhaseIR.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "ZeroPhaseIR.wav"))
            'IrChannelCopy.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "IrChannelCopy.wav"))

            If Channel = 1 Then
                ZeroPhaseEquivalent.BinauralIR.WaveData.SampleData(1) = ZeroPhaseIR.WaveData.SampleData(1)
            Else
                ZeroPhaseEquivalent.BinauralIR.WaveData.SampleData(2) = ZeroPhaseIR.WaveData.SampleData(1)
            End If

        Next

        Return ZeroPhaseEquivalent

    End Function



End Module