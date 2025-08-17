Imports STFN.Core.Audio

Public Module ImpulseResponseCustomFunctions

    Public Sub MeasureIrGain(ByVal IrPath As String, ByVal OutputFolder As String)

        Dim TargetLevel As Double = -30

        Dim Ir = Sound.LoadWaveFile(IrPath)

        Dim WhiteNoise = DSP.CreateWhiteNoise(Ir.WaveFormat, 1, 1, 5)

        Dim BandPassIr = DSP.CreateSpecialTypeImpulseResponse(Ir.WaveFormat, New Formats.FftFormat(), 8192, 1, DSP.FilterType.BandPass, 200, 8000, -100, 0, 18)

        'Bandbass-filter noise
        Dim BandPassNoise = DSP.FIRFilter(WhiteNoise, BandPassIr, New Formats.FftFormat,,,,,,, True)
        'Dim BandPassNoise = WhiteNoise

        'Set noise level to TargetLevel
        DSP.MeasureAndAdjustSectionLevel(BandPassNoise, TargetLevel)

        BandPassNoise.WriteWaveFile(IO.Path.Combine(OutputFolder, "BandPassNoise .wav"))

        'Filter Noise with IR
        Dim FilteredNoise = DSP.FIRFilter(BandPassNoise, Ir, New Formats.FftFormat,,,,,,, True)

        FilteredNoise.WriteWaveFile(IO.Path.Combine(OutputFolder, "FilteredBandPassNoise .wav"))

        'Measure resulting levels
        Console.WriteLine(DSP.MeasureSectionLevel(BandPassNoise, 1))
        Console.WriteLine(DSP.MeasureSectionLevel(FilteredNoise, 1))

        'Calculate difference

    End Sub


End Module