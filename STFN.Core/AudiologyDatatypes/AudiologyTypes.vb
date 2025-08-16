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

    ''' <summary>
    ''' Calculates the signal level based on the noise level and the signal to noise ratio.
    ''' </summary>
    ''' <param name="SignalLevel"></param>
    ''' <param name="SNR"></param>
    ''' <returns></returns>
    Public Function GetSignalLevel(ByVal NoiseLevel As Double, ByVal SNR As Double)
        Return NoiseLevel + SNR
    End Function

    ''' <summary>
    ''' Calculates the noise level based on the signal level and the signal to noise ratio.
    ''' </summary>
    ''' <param name="NoiseLevel"></param>
    ''' <param name="SNR"></param>
    ''' <returns></returns>
    Public Function GetNoiseLevel(ByVal SignalLevel As Double, ByVal SNR As Double)
        Return SignalLevel - SNR
    End Function


End Module