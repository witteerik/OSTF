Public Module AudiologyTypes

    Public Enum BmldModes
        RightOnly
        LeftOnly
        BinauralSamePhase
        BinauralPhaseInverted
        BinauralUncorrelated 'Noise only
    End Enum

    ''' <summary>
    ''' Calculates the signal to noise ratio between the signal level and the noise level.
    ''' </summary>
    ''' <param name="SignalLevel"></param>
    ''' <param name="NoiseLevel"></param>
    ''' <returns></returns>
    Public Function SignalToNoiseRatio(ByVal SignalLevel As Double, ByVal NoiseLevel As Double)
        Return SignalLevel - NoiseLevel
    End Function

End Module