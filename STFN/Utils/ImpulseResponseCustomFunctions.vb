
Public Module ImpulseResponseCustomFunctions

    Public Sub MeasureIrGain(ByVal IrPath As String, ByVal OutputFolder As String)

        Dim TargetLevel As Double = -30

        Dim Ir = Audio.Sound.LoadWaveFile(IrPath)

        Dim WhiteNoise = Audio.GenerateSound.CreateWhiteNoise(Ir.WaveFormat, 1, 1, 5)

        Dim BandPassIr = Audio.GenerateSound.CreateSpecialTypeImpulseResponse(Ir.WaveFormat, New Audio.Formats.FftFormat(), 8192, 1, Audio.FilterType.BandPass, 200, 8000, -100, 0, 18)

        'Bandbass-filter noise
        Dim BandPassNoise = Audio.DSP.FIRFilter(WhiteNoise, BandPassIr, New Audio.Formats.FftFormat,,,,,,, True)
        'Dim BandPassNoise = WhiteNoise

        'Set noise level to TargetLevel
        Audio.DSP.MeasureAndAdjustSectionLevel(BandPassNoise, TargetLevel)

        BandPassNoise.WriteWaveFile(IO.Path.Combine(OutputFolder, "BandPassNoise .wav"))

        'Filter Noise with IR
        Dim FilteredNoise = Audio.DSP.FIRFilter(BandPassNoise, Ir, New Audio.Formats.FftFormat,,,,,,, True)

        FilteredNoise.WriteWaveFile(IO.Path.Combine(OutputFolder, "FilteredBandPassNoise .wav"))

        'Measure resulting levels
        Console.WriteLine(Audio.DSP.MeasureSectionLevel(BandPassNoise, 1))
        Console.WriteLine(Audio.DSP.MeasureSectionLevel(FilteredNoise, 1))

        'Calculate difference

    End Sub


End Module