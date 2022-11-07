Namespace SipTest

    Public Module PDL

        ''' <summary>
        ''' Calculates PDL
        ''' </summary>
        ''' <param name="SDRt">The Speech-to-Disturbance ratio of the correct response alternative.</param>
        ''' <param name="SDRcs">A list of Speech-to-Disturbance ratios of incorrect response alternatives.</param>
        ''' <returns></returns>
        Public Function CalculatePDL(ByVal SDRt As Double(), ByVal SDRcs As List(Of Double()))

            Dim antilogSDRrdiv10 As Double() = GetAntilogSDRdiv10(SDRt)

            Dim InvSumsList As New List(Of Double)
            For Each SDRc In SDRcs

                Dim antilogSDRcdiv10 As Double() = GetAntilogSDRdiv10(SDRc)

                'Calculating abs differences
                Dim AbsDiff As Double() = GetAbsoluteSdrDifferences(antilogSDRrdiv10, antilogSDRcdiv10)

                'Calculating the inverted sum
                Dim InvSum As Double = 1 / AbsDiff.Sum

                'Adding to the inverted sum to InvSumsList
                InvSumsList.Add(InvSum)

            Next

            'Calculating PDL
            Dim PDL As Double = 10 * Math.Log10(InvSumsList.Count / InvSumsList.Sum)

            Return PDL

        End Function

        Private Function GetAntilogSDRdiv10(ByVal Input As Double()) As Double()
            Dim Output(19) As Double
            'Skipping the first index (cf Witte' thesis study IV page 56.
            For i = 0 To 19
                Output(i) = 10 ^ (Input(i + 1) / 10)
            Next
            Return Output
        End Function

        Private Function GetAbsoluteSdrDifferences(ByVal Input1 As Double(), ByVal Input2 As Double())
            Dim Output(19) As Double
            For i = 0 To 19
                Output(i) = Math.Abs(Input1(i) - Input2(i))
            Next
            Return Output
        End Function



        ''' <summary>
        ''' Calculates the Spectral Speech-to-Disturbance Ratio (SDR).
        ''' </summary>
        ''' <param name="E">Speech spectrum level (in each critical band)</param>
        ''' <param name="N">Noise spectrum level (in each critical band)</param>
        ''' <param name="T_">Equivalent hearing threshold level (in each critical band)</param>
        ''' <param name="G">Hearing-aid insertion gain (in each critical band)</param>
        ''' <param name="Binaural"></param>
        ''' <param name="SF30"></param>
        ''' <param name="SRFM"></param>
        ''' <returns></returns>
        Public Function CalculateSDR(ByVal E As Double(), ByVal N As Double(), ByVal T_ As Double(), ByVal G As Double(),
                                 ByVal Binaural As Boolean, ByVal SF30 As Boolean, ByVal SRFM As Double?()) As Double()

            'Cloning the input arrays, so that they don't get modified by the function
            Dim E_Copy As Double() = E.Clone
            Dim N_Copy As Double() = N.Clone
            Dim T__Copy As Double() = T_.Clone
            Dim G_Copy As Double() = G.Clone

            Dim SRFM_Copy As Double?() = SRFM.Clone

            '  Critical band specifications according to table 1 in ANSI S3.5-1997
            '   Centre frequencies
            Dim F As Double() = {150, 250, 350, 450, 570, 700, 840, 1000, 1170, 1370, 1600, 1850,
                 2150, 2500, 2900, 3400, 4000, 4800, 5800, 7000, 8500}

            '  Lower limits
            Dim l As Double() = {100, 200, 300, 400, 510, 630, 770, 920, 1080, 1270, 1480, 1720,
               2000, 2320, 2700, 3150, 3700, 4400, 5300, 6400, 7700}

            '  Upper limits
            Dim h As Double() = {200, 300, 400, 510, 630, 770, 920, 1080, 1270, 1480, 1720, 2000,
                 2320, 2700, 3150, 3700, 4400, 5300, 6400, 7700, 9500}

            '  Bandwidths
            Dim CBW(20) As Double
            For i = 0 To 20
                CBW(i) = h(i) - l(i)
            Next

            '  Reference internal noise spectrum
            Dim X As Double() = {1.5, -3.9, -7.2, -8.9, -10.3, -11.4, -12.0, -12.5, -13.2, -14.0, -15.4,
                  -16.9, -18.8, -21.2, -23.2, -24.9, -25.9, -24.2, -19.0, -11.7, -6.0}

            'Step 2 (equation A3)
            '  # Calculating Spectrum level of equivalent speech
            Dim E_(20) As Double
            For i = 0 To 20
                E_(i) = E_Copy(i) + G_Copy(i)
            Next

            'Step 3
            If SF30 = True Then

                '    #Interpolated values for the spectral attenuation caused by the +/- 30 degree azimuths of the noise speakers. Data taken from the values of azimuthal dependence D (0,v), in decibels, at frequencies between 0.2 And 1.4 kHz for azimuths of 30 And 330 degrees, taken from Table 1 in E. A. G. Shaw, And M. M. Vaillancourt, Transformation of sound-pressure level from the free field to the eardrum presented in numerical form
                '    # ds below are the differences between attenuation at 30 degrees And that of 330 degrees.
                '    fds <- c(200, 250, 300, 320, 400, 500, 600, 630, 700, 800, 900, 1000, 1200, 1250, 1400, 1600, 1800, 2000, 2300, 2500, 2700, 2900, 3000, 3200, 3500, 4000, 4500, 5000, 5500, 6000, 6300, 6500, 7000, 7500, 8000, 8500, 9000, 9500, 10000, 10500, 11000, 11500, 12000)
                '    ds <- c(0, 0, 0, 0, 0.2, 0.2, 0.2, 0.1, -0.2, -0.7, -1, -1.2, -1.1, -1.1, -0.9, -0.6, -0.5, -0.4, -1, -1.1, -1.1, -0.9, -0.8, -0.9, -1.5, -2.3, -1.9, -1.1, -0.6, -0.4, -0.4, -0.5, -0.6, -0.8, -1, -1.6, -2.6, -3.3, -3.6, -3.4, -3.1, -2.3, -2.4)
                '    int_ds <- approx(x = fds, y = ds, xout = Fi, method = "linear", rule = 2)$y
                'The data below are taken straight from the Witte's thesis, and may be rounded. Todo: check the values directly in R!
                Dim int_ds As Double() = {0.000, 0.000, 0.075, 0.2, 0.2, -0.2, -0.82, -1.2, -1.115, -0.94, -0.6, -0.475, -0.7, -1.1, -0.9, -1.3, -2.3, -1.42, -0.48, -0.6, -1.6}

                'Step 4
                '    Subtracting spectral release from masking To the noise
                If SRFM_Copy IsNot Nothing Then
                    If SRFM_Copy.Length = 1 Or SRFM_Copy.Length = 21 Then

                        ' If the the length of SRFM Is 1, that value Is subtracted from all requency bands,
                        ' And if the length Is 21, SRFM should contain the attenuation of each of the 21 critical frequency bands

                        If SRFM_Copy.Length = 1 Then
                            For i = 0 To int_ds.Length - 1
                                int_ds(i) -= SRFM_Copy(0)
                            Next
                        Else
                            For i = 0 To int_ds.Length - 1
                                int_ds(i) -= SRFM_Copy(i)
                            Next
                        End If
                    Else
                        Throw New ArgumentException("SRFM must be a numeric vector of length 1 or 21, or NULL.")
                    End If
                End If

                'Adding the delta HRFT, optionally modified  by the SRFM, to N
                For i = 0 To 20
                    N_Copy(i) = N_Copy(i) + int_ds(i)
                Next
            End If

            'Step 5
            Dim N_(20) As Double
            For i = 0 To 20
                N_(i) = N_Copy(i) + G_Copy(i)
            Next

            'Step 6
            If Binaural = True Then
                For i = 0 To 20
                    T__Copy(i) -= 1.7
                Next
            End If

            'Step 7

            '  # Calculating Spectrum level for self-speech masking
            Dim V(20) As Double
            For i = 0 To 20
                V(i) = E_(i) - 24
            Next

            '  # Calculating the Larger of the Spectrum levels for equivalent noise And self-speech masking
            Dim B(20) As Double
            For i = 0 To 20
                B(i) = Math.Max(N_(i), V(i))
            Next

            '  # Calculating Masking slope per octave of the upward spread of masking
            Dim C(20) As Double
            For i = 0 To 20
                C(i) = -80 + 0.6 * (B(i) + 10 * Math.Log10(CBW(i)))
            Next


            '  # Calculating Spectrum level for equivalent masking
            Dim Z(20) As Double

            Z(0) = B(0)

            For i = 1 To 19 'TODO: Check why this do not go to 20 (or 21 in the R-code) ???
                Dim ExponentTermB As New List(Of Double)
                For j = 0 To i - 1
                    ExponentTermB.Add(10 ^ (0.1 * (B(j) + 3.32 * C(j) * Math.Log10(F(i) / h(j)))))
                Next
                Z(i) = 10 * Math.Log10(10 ^ (0.1 * N_(i)) + ExponentTermB.Sum)
            Next


            '  # Calculating Equivalent internal noise spectrum
            Dim X_(20) As Double

            For i = 0 To 20
                X_(i) = X(i) + T__Copy(i)
            Next


            'Step 8
            Dim D_(20) As Double

            For i = 0 To 20
                D_(i) = 10 * Math.Log10(10 ^ (Z(i) / 10) + 10 ^ (X_(i) / 10))
            Next

            'Step 9
            Dim SDR(20) As Double
            For i = 0 To 20
                SDR(i) = E_(i) - D_(i)
            Next

            Return SDR

        End Function

        Public Function GetMLD(Optional ByVal Frequencies As Double?() = Nothing, Optional ByVal c_factor As Double = 1) As Double?()

            '  Based on formula from Durlach 1963. Equalization and cancellation Theory...

            If Frequencies Is Nothing Then
                Frequencies = {150, 250, 350, 450, 570, 700, 840, 1000, 1170, 1370, 1600, 1850, 2150, 2500, 2900, 3400, 4000, 4800, 5800, 7000, 8500}
            End If

            Dim MLD(Frequencies.Length - 1) As Double?

            Dim se = 0.25
            Dim sd = 0.000105

            For i = 0 To Frequencies.Length - 1
                Dim w = Frequencies(i) * 2 * Math.PI
                Dim z = 2 * Math.Exp(-w ^ 2 * sd ^ 2) / (1 + se ^ 2 - Math.Exp(-w ^ 2 * sd ^ 2))
                MLD(i) = c_factor * 10 * Math.Log10(z + 1)
            Next

            Return MLD

        End Function

        'GetMLD <-  function(frequencies = NULL){
        '  
        '  # Based on formula from Durlach 1963. Equalization and cancellation Theory...
        '  
        '  if (is.null(frequencies)==TRUE) {
        '    frequencies <-  c(150, 250, 350, 450, 570, 700, 840, 1000, 1170, 1370, 1600, 1850, 
        '                      2150, 2500, 2900, 3400, 4000, 4800, 5800, 7000, 8500)
        '  }
        '  
        '  w <- frequencies*2*pi
        '  se <- 0.25
        '  sd <- 0.000105
        '  
        '  z <- 2*exp(-w^2*sd^2)/(1+se^2-exp(-w^2*sd^2))
        '  
        '  MLD <- 10*log10(z+1)
        '  
        '  #plot(frequencies,MLD, ylim = c(0,30))
        '  #lines(frequencies,GetInterpolatedDurlachData())
        '  
        '  return(MLD)
        '  
        '}


        'Translated from the following R-code

        '    #### ~ Phoneme Discriminability Level, PDL ####

        'CalculatePDL <- function(InData, Binaural, InsertionGain = FALSE, SF30, SRFM){

        '  # Getting Spectrum level of measured noise
        '  Ni <- data.frame(InData$ESL_150, InData$ESL_250, InData$ESL_350, InData$ESL_450, InData$ESL_570, 
        '                   InData$ESL_700, InData$ESL_840, InData$ESL_1000, InData$ESL_1170, InData$ESL_1370, 
        '                   InData$ESL_1600, InData$ESL_1850, InData$ESL_2150, InData$ESL_2500, InData$ESL_2900, 
        '                   InData$ESL_3400, InData$ESL_4000, InData$ESL_4800, InData$ESL_5800, InData$ESL_7000, 
        '                   InData$ESL_8500)

        '  # Getting (interpolated) equivalent hearing threshold levels 
        '  Ti_ <- data.frame(InData$BPTA_150, InData$BPTA_250, InData$BPTA_350, InData$BPTA_450, InData$BPTA_570, 
        '                    InData$BPTA_700, InData$BPTA_840, InData$BPTA_1000, InData$BPTA_1170, InData$BPTA_1370, 
        '                    InData$BPTA_1600, InData$BPTA_1850, InData$BPTA_2150, InData$BPTA_2500, InData$BPTA_2900, 
        '                    InData$BPTA_3400, InData$BPTA_4000, InData$BPTA_4800, InData$BPTA_5800, InData$BPTA_7000, 
        '                    InData$BPTA_8500)


        '  # Getting Spectrum level of measured speech
        '  Ei <- data.frame(InData$PSL_150, InData$PSL_250, InData$PSL_350, InData$PSL_450, InData$PSL_570, 
        '                   InData$PSL_700, InData$PSL_840, InData$PSL_1000, InData$PSL_1170, InData$PSL_1370, 
        '                   InData$PSL_1600, InData$PSL_1850, InData$PSL_2150, InData$PSL_2500, InData$PSL_2900, 
        '                   InData$PSL_3400, InData$PSL_4000, InData$PSL_4800, InData$PSL_5800, InData$PSL_7000, 
        '                   InData$PSL_8500)

        '  # If insertionThen gain Is To be used, it must be supplied For Each row And frequency In the input data frame
        '  If (InsertionGain == True) {
        '    Gi <- data.frame(InData$Gi_150, InData$Gi_250, InData$Gi_350, InData$Gi_450, InData$Gi_570, 
        '                     InData$Gi_700, InData$Gi_840, InData$Gi_1000, InData$Gi_1170, InData$Gi_1370, 
        '                     InData$Gi_1600, InData$Gi_1850, InData$Gi_2150, InData$Gi_2500, InData$Gi_2900, 
        '                     InData$Gi_3400, InData$Gi_4000, InData$Gi_4800, InData$Gi_5800, InData$Gi_7000, 
        '                     InData$Gi_8500)
        '  }else{Gi <- NULL}

        '  SDR <- CalculateSDR(Ei = Ei, Ni = Ni, Ti_ = Ti_, Binaural = Binaural, Gi = Gi, SF30, SRFM) 

        '  # Getting Spectrum level of measured speech, contrast 1
        '  Ei_C1 <- data.frame(InData$PSL_CP1_150, InData$PSL_CP1_250, InData$PSL_CP1_350, InData$PSL_CP1_450, InData$PSL_CP1_570, 
        '                      InData$PSL_CP1_700, InData$PSL_CP1_840, InData$PSL_CP1_1000, InData$PSL_CP1_1170, InData$PSL_CP1_1370, 
        '                      InData$PSL_CP1_1600, InData$PSL_CP1_1850, InData$PSL_CP1_2150, InData$PSL_CP1_2500, InData$PSL_CP1_2900, 
        '                      InData$PSL_CP1_3400, InData$PSL_CP1_4000, InData$PSL_CP1_4800, InData$PSL_CP1_5800, InData$PSL_CP1_7000, 
        '                      InData$PSL_CP1_8500)

        '  # Getting Spectrum level of measured speech, contrast 2
        '  Ei_C2 <- data.frame(InData$PSL_CP2_150, InData$PSL_CP2_250, InData$PSL_CP2_350, InData$PSL_CP2_450, InData$PSL_CP2_570, 
        '                      InData$PSL_CP2_700, InData$PSL_CP2_840, InData$PSL_CP2_1000, InData$PSL_CP2_1170, InData$PSL_CP2_1370, 
        '                      InData$PSL_CP2_1600, InData$PSL_CP2_1850, InData$PSL_CP2_2150, InData$PSL_CP2_2500, InData$PSL_CP2_2900, 
        '                      InData$PSL_CP2_3400, InData$PSL_CP2_4000, InData$PSL_CP2_4800, InData$PSL_CP2_5800, InData$PSL_CP2_7000, 
        '                      InData$PSL_CP2_8500)

        '  # Calculating SDR for the hypothecially presented response alternatives
        '  SDR_C1 <- CalculateSDR(Ei = Ei_C1, Ni = Ni, Ti_ = Ti_, Binaural = Binaural, Gi = Gi, SF30, SRFM) 
        '  SDR_C2 <- CalculateSDR(Ei = Ei_C2, Ni = Ni, Ti_ = Ti_, Binaural = Binaural, Gi = Gi, SF30, SRFM) 

        '  # New section from 2020-10-31

        '  D_C1 <- abs(10^(SDR$SDRi/10) - 10^(SDR_C1$SDRi/10))
        '  D_C2 <- abs(10^(SDR$SDRi/10) - 10^(SDR_C2$SDRi/10))

        '  # Summing the audible differences across the spectrum, excluding the lowest band
        '  D_C1_sum <- D_C1[,2] + D_C1[,3] + D_C1[,4] + D_C1[,5] + D_C1[,6] + D_C1[,7] + D_C1[,8] +
        '    D_C1[,9]+ D_C1[,10]+ D_C1[,11]+ D_C1[,12]+ D_C1[,13]+ D_C1[,14]+ D_C1[,15]+ D_C1[,16]+ D_C1[,17] +
        '    D_C1[,18]+ D_C1[,19]+ D_C1[,20]+ D_C1[,21]

        '  D_C2_sum <- D_C2[,2] + D_C2[,3] + D_C2[,4] + D_C2[,5] + D_C2[,6] + D_C2[,7] + D_C2[,8] +
        '    D_C2[,9]+ D_C2[,10]+ D_C2[,11]+ D_C2[,12]+ D_C2[,13]+ D_C2[,14]+ D_C2[,15]+ D_C2[,16]+ D_C2[,17] +
        '    D_C2[,18]+ D_C2[,19]+ D_C2[,20]+ D_C2[,21]

        '  If (any(Is .infinite(D_C1_sum) == True)) {
        '    Stop("Infinite numbers in linear distance to contrast 1")
        '  }  
        '  If (any(Is .infinite(D_C2_sum) == True)) {
        '    Stop("Infinite numbers in linear distance to contrast 2")
        '  }  

        '  # Converting to similarity
        '  S_C1 <- 1/D_C1_sum
        '  S_C2 <- 1/D_C2_sum

        '  #hist(S_C1)
        '  #hist(S_C2)

        '  # Summing the similarity
        '  S_C <- S_C1 + S_C2
        '  #hist(S_C)

        '  # Calculating the PDL (Convert to distance, And multiplying by the number of incorrect reponse alternatives)
        '  PDI <- 10 * log10(2/S_C)
        '  #hist(PDI)

        '  # If SIIThen has been calculated For Each trial (Using the Function GetSII), comparisons between
        '  # the SII And PDL can be made by uncommenting the following lines
        '  #plot(PDI, SII$SII)
        '  #cor.test(PDI, SII$SII)  

        '  # Storing the PDL In the input data  
        '  InData$PDI <- PDI

        '  Return (InData)



        '  # # Calculating the absolute difference in spectral audibility between the presented test word And 
        '  # # each of the competing response alternatives  
        '  # D_C1 <- abs(SDR$SDRi - SDR_C1$SDRi)
        '  # D_C2 <- abs(SDR$SDRi - SDR_C2$SDRi)
        '  # 
        '  # # Summing the audible differences across the spectrum
        '  # # D_C1_sum <- D_C1[,1] + D_C1[,2] + D_C1[,3] + D_C1[,4] + D_C1[,5] + D_C1[,6] + D_C1[,7] + D_C1[,8] +
        '  # #   D_C1[,9]+ D_C1[,10]+ D_C1[,11]+ D_C1[,12]+ D_C1[,13]+ D_C1[,14]+ D_C1[,15]+ D_C1[,16]+ D_C1[,17] +
        '  # #   D_C1[,18]+ D_C1[,19]+ D_C1[,20]+ D_C1[,21]
        '  # # 
        '  # # D_C2_sum <- D_C2[,1] + D_C2[,2] + D_C2[,3] + D_C2[,4] + D_C2[,5] + D_C2[,6] + D_C2[,7] + D_C2[,8] +
        '  # #   D_C2[,9]+ D_C2[,10]+ D_C2[,11]+ D_C2[,12]+ D_C2[,13]+ D_C2[,14]+ D_C2[,15]+ D_C2[,16]+ D_C2[,17] +
        '  # #   D_C2[,18]+ D_C2[,19]+ D_C2[,20]+ D_C2[,21]
        '  # 
        '  # # Summing the audible differences across the spectrum, excluding the lowest band
        '  # D_C1_sum <- D_C1[,2] + D_C1[,3] + D_C1[,4] + D_C1[,5] + D_C1[,6] + D_C1[,7] + D_C1[,8] +
        '  #   D_C1[,9]+ D_C1[,10]+ D_C1[,11]+ D_C1[,12]+ D_C1[,13]+ D_C1[,14]+ D_C1[,15]+ D_C1[,16]+ D_C1[,17] +
        '  #   D_C1[,18]+ D_C1[,19]+ D_C1[,20]+ D_C1[,21]
        '  # 
        '  # D_C2_sum <- D_C2[,2] + D_C2[,3] + D_C2[,4] + D_C2[,5] + D_C2[,6] + D_C2[,7] + D_C2[,8] +
        '  #   D_C2[,9]+ D_C2[,10]+ D_C2[,11]+ D_C2[,12]+ D_C2[,13]+ D_C2[,14]+ D_C2[,15]+ D_C2[,16]+ D_C2[,17] +
        '  #   D_C2[,18]+ D_C2[,19]+ D_C2[,20]+ D_C2[,21]
        '  # 
        '  # # Multiplying by the number of response alternatives
        '  # D_C1_sum <- D_C1_sum *2
        '  # D_C2_sum <- D_C2_sum *2
        '  # 
        '  # #hist(D_C1_sum)
        '  # #hist(D_C2_sum)
        '  # 
        '  # if (any(Is.infinite(D_C1_sum) == TRUE)) {
        '  #   stop("Infinite numbers in linear distance to contrast 1")
        '  # }  
        '  # if (any(Is.infinite(D_C2_sum) == TRUE)) {
        '  #   stop("Infinite numbers in linear distance to contrast 2")
        '  # }  
        '  # 
        '  # # Converting to similarity
        '  # S_C1 <- 1/D_C1_sum
        '  # S_C2 <- 1/D_C2_sum
        '  # 
        '  # #hist(S_C1)
        '  # #hist(S_C2)
        '  # 
        '  # # Adding the similarity
        '  # S_C <- S_C1 + S_C2
        '  # #hist(S_C)
        '  # 
        '  # # Comvert to distance
        '  # PDI <- 1/S_C
        '  # #hist(PDI)
        '  # 
        '  # # If SII has been calculated for each trial (using the function GetSII), comparisons between
        '  # # the SII And PDL can be made by uncommenting the following lines
        '  # #plot(PDI, SII$SII)
        '  # #cor.test(PDI, SII$SII)  
        '  # 
        '  # # Converting to dB scale
        '  # PDI <- 10 * log10(PDI / 10^(-12))
        '  # 
        '  # #hist(PDI)
        '  # 
        '  # # Storing the PDL in the input data  
        '  # InData$PDI <- PDI
        '  # 
        '  # return(InData)

        '}



        'CalculateSDR <-  function(Ei, Ni, Ti_, Gi = NULL, Binaural, SF30, SRFM = NULL){

        '  # Calculates the spectral speech-to-disturbance ratio (on the linear scale)

        '  # Noting the data length
        '  DataLength <- length(Ei[,1])

        '  # Critical band specifications according to table 1 in ANSI S3.5-1997
        '  # Centre frequencies
        '  Fi <-  c(150, 250, 350, 450, 570, 700, 840, 1000, 1170, 1370, 1600, 1850, 
        '           2150, 2500, 2900, 3400, 4000, 4800, 5800, 7000, 8500)

        '  # Lower limits
        '  l <- c(100, 200, 300, 400, 510, 630, 770, 920, 1080, 1270, 1480, 1720, 
        '         2000, 2320, 2700, 3150, 3700, 4400, 5300, 6400, 7700)

        '  # Upper limits
        '  h <- c(200, 300, 400, 510, 630, 770, 920, 1080, 1270, 1480, 1720, 2000, 
        '         2320, 2700, 3150, 3700, 4400, 5300, 6400, 7700, 9500)

        '  # Bandwidths
        '  CBW <- h - l

        '  # Reference internal noise spectrum
        '  Xi <- c(1.5, -3.9, -7.2, -8.9, -10.3, - 11.4, -12.0, -12.5, -13.2, -14.0, -15.4, 
        '          -16.9, -18.8, -21.2, -23.2, -24.9, -25.9, -24.2, -19.0, -11.7, -6.0)


        '  If (Is .null(Gi) == True) {
        '    # Creating a 0 valued Gi data frame (i.e. no amplification)
        '    Gi <- data.frame("i_150" = rep(0,DataLength), "i_250" = rep(0,DataLength), "i_350" = rep(0,DataLength), "i_450" = rep(0,DataLength), "i_570" = rep(0,DataLength), 
        '                     "i_700" = rep(0,DataLength), "i_840" = rep(0,DataLength), "i_1000" = rep(0,DataLength), "i_1170" = rep(0,DataLength), "i_1370" = rep(0,DataLength), 
        '                     "i_1600" = rep(0,DataLength), "i_1850" = rep(0,DataLength), "i_2150" = rep(0,DataLength), "i_2500" = rep(0,DataLength), "i_2900" = rep(0,DataLength), 
        '                     "i_3400" = rep(0,DataLength), "i_4000" = rep(0,DataLength), "i_4800" = rep(0,DataLength), "i_5800" = rep(0,DataLength), "i_7000" = rep(0,DataLength), 
        '                     "i_8500" = rep(0,DataLength))
        '  }

        '  # Calculating Spectrum level of equivalent speech
        '  Ei_ <- Ei + Gi


        '  If (SF30 == True) {

        '    #Interpolating values for the spectral attenuation caused by the +/- 30 degree azimuths of the noise speakers. Data taken from the values of azimuthal dependence D (0,v), in decibels, at frequencies between 0.2 And 1.4 kHz for azimuths of 30 And 330 degrees, taken from Table 1 in E. A. G. Shaw, And M. M. Vaillancourt, Transformation of sound-pressure level from the free field to the eardrum presented in numerical form
        '    # ds below are the differences between attenuation at 30 degrees And that of 330 degrees.
        '    fds <- c(200, 250, 300, 320, 400, 500, 600, 630, 700, 800, 900, 1000, 1200, 1250, 1400, 1600, 1800, 2000, 2300, 2500, 2700, 2900, 3000, 3200, 3500, 4000, 4500, 5000, 5500, 6000, 6300, 6500, 7000, 7500, 8000, 8500, 9000, 9500, 10000, 10500, 11000, 11500, 12000)
        '    ds <- c(0, 0, 0, 0, 0.2, 0.2, 0.2, 0.1, -0.2, -0.7, -1, -1.2, -1.1, -1.1, -0.9, -0.6, -0.5, -0.4, -1, -1.1, -1.1, -0.9, -0.8, -0.9, -1.5, -2.3, -1.9, -1.1, -0.6, -0.4, -0.4, -0.5, -0.6, -0.8, -1, -1.6, -2.6, -3.3, -3.6, -3.4, -3.1, -2.3, -2.4)
        '    int_ds <- approx(x = fds, y = ds, xout = Fi, method = "linear", rule = 2)$y

        '    # Subtracting spectral release from masking To the noise
        '    If (Is .null(SRFM) == False) {
        '      If (length(SRFM) == 1 | length(SRFM) == 21) {
        '        # If theThen length of SRFM Is 1, that value Is subtracted from all requency bands,
        '        # And if the length Is 21, SRFM should contain the attenuation of each of the 21 critical frequency bands
        '        int_ds <- int_ds - SRFM
        '      }else{
        '        Stop("SRFM must be a numeric vector of length 1 or 21, or NULL.")
        '      }
        '    } 

        '    # Adding To the noise data frame
        '    For (i in 1:21) {
        '      Ni[,i] <-  Ni[,i] + int_ds[i]
        '    }
        '  }


        '  # Calculating Spectrum level of equivalent noise
        '  Ni_ <- Ni + Gi  


        '  If (Binaural == True) {
        '    # Binaural loudness summation factor (Cf. ANSI S3.5 1997, page 15, clause 5.2.4)
        '    Ti_ <- Ti_ - 1.7  
        '  }

        '  # Calculating Spectrum level for self-speech masking
        '  Vi <- Ei_ - 24

        '  # Calculating the Larger of the Spectrum levels for equivalent noise And self-speech masking
        '  Bi <- pmax(Ni_,Vi)

        '  # Calculating Masking slope per octave of the upward spread of masking
        '  Ci <-  Bi

        '  Ci[,1] <- -80 + 0.6 * (Ci[,1] + 10 * log10(CBW[1]))
        '  Ci[,2] <- -80 + 0.6 * (Ci[,2] + 10 * log10(CBW[2]))
        '  Ci[,3] <- -80 + 0.6 * (Ci[,3] + 10 * log10(CBW[3]))
        '  Ci[,4] <- -80 + 0.6 * (Ci[,4] + 10 * log10(CBW[4]))
        '  Ci[,5] <- -80 + 0.6 * (Ci[,5] + 10 * log10(CBW[5]))
        '  Ci[,6] <- -80 + 0.6 * (Ci[,6] + 10 * log10(CBW[6]))
        '  Ci[,7] <- -80 + 0.6 * (Ci[,7] + 10 * log10(CBW[7]))
        '  Ci[,8] <- -80 + 0.6 * (Ci[,8] + 10 * log10(CBW[8]))
        '  Ci[,9] <- -80 + 0.6 * (Ci[,9] + 10 * log10(CBW[9]))
        '  Ci[,10] <- -80 + 0.6 * (Ci[,10] + 10 * log10(CBW[10]))
        '  Ci[,11] <- -80 + 0.6 * (Ci[,11] + 10 * log10(CBW[11]))
        '  Ci[,12] <- -80 + 0.6 * (Ci[,12] + 10 * log10(CBW[12]))
        '  Ci[,13] <- -80 + 0.6 * (Ci[,13] + 10 * log10(CBW[13]))
        '  Ci[,14] <- -80 + 0.6 * (Ci[,14] + 10 * log10(CBW[14]))
        '  Ci[,15] <- -80 + 0.6 * (Ci[,15] + 10 * log10(CBW[15]))
        '  Ci[,16] <- -80 + 0.6 * (Ci[,16] + 10 * log10(CBW[16]))
        '  Ci[,17] <- -80 + 0.6 * (Ci[,17] + 10 * log10(CBW[17]))
        '  Ci[,18] <- -80 + 0.6 * (Ci[,18] + 10 * log10(CBW[18]))
        '  Ci[,19] <- -80 + 0.6 * (Ci[,19] + 10 * log10(CBW[19]))
        '  Ci[,20] <- -80 + 0.6 * (Ci[,20] + 10 * log10(CBW[20]))
        '  Ci[,21] <- -80 + 0.6 * (Ci[,21] + 10 * log10(CBW[21]))

        '  # Calculating Spectrum level for equivalent masking
        '  Zi <- Bi

        '  i <- 2
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2])))
        '  )


        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6]))) +
        '                         10 ^ (0.1 * (Bi[,7] + 3.32 * Ci[,7] * log10(Fi[i] / h[7]))) 
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6]))) +
        '                         10 ^ (0.1 * (Bi[,7] + 3.32 * Ci[,7] * log10(Fi[i] / h[7]))) +
        '                         10 ^ (0.1 * (Bi[,8] + 3.32 * Ci[,8] * log10(Fi[i] / h[8]))) 
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6]))) +
        '                         10 ^ (0.1 * (Bi[,7] + 3.32 * Ci[,7] * log10(Fi[i] / h[7]))) +
        '                         10 ^ (0.1 * (Bi[,8] + 3.32 * Ci[,8] * log10(Fi[i] / h[8]))) +
        '                         10 ^ (0.1 * (Bi[,9] + 3.32 * Ci[,9] * log10(Fi[i] / h[9])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6]))) +
        '                         10 ^ (0.1 * (Bi[,7] + 3.32 * Ci[,7] * log10(Fi[i] / h[7]))) +
        '                         10 ^ (0.1 * (Bi[,8] + 3.32 * Ci[,8] * log10(Fi[i] / h[8]))) +
        '                         10 ^ (0.1 * (Bi[,9] + 3.32 * Ci[,9] * log10(Fi[i] / h[9]))) +
        '                         10 ^ (0.1 * (Bi[,10] + 3.32 * Ci[,10] * log10(Fi[i] / h[10])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6]))) +
        '                         10 ^ (0.1 * (Bi[,7] + 3.32 * Ci[,7] * log10(Fi[i] / h[7]))) +
        '                         10 ^ (0.1 * (Bi[,8] + 3.32 * Ci[,8] * log10(Fi[i] / h[8]))) +
        '                         10 ^ (0.1 * (Bi[,9] + 3.32 * Ci[,9] * log10(Fi[i] / h[9]))) +
        '                         10 ^ (0.1 * (Bi[,10] + 3.32 * Ci[,10] * log10(Fi[i] / h[10]))) +
        '                         10 ^ (0.1 * (Bi[,11] + 3.32 * Ci[,11] * log10(Fi[i] / h[11])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6]))) +
        '                         10 ^ (0.1 * (Bi[,7] + 3.32 * Ci[,7] * log10(Fi[i] / h[7]))) +
        '                         10 ^ (0.1 * (Bi[,8] + 3.32 * Ci[,8] * log10(Fi[i] / h[8]))) +
        '                         10 ^ (0.1 * (Bi[,9] + 3.32 * Ci[,9] * log10(Fi[i] / h[9]))) +
        '                         10 ^ (0.1 * (Bi[,10] + 3.32 * Ci[,10] * log10(Fi[i] / h[10]))) +
        '                         10 ^ (0.1 * (Bi[,11] + 3.32 * Ci[,11] * log10(Fi[i] / h[11]))) +
        '                         10 ^ (0.1 * (Bi[,12] + 3.32 * Ci[,12] * log10(Fi[i] / h[12])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6]))) +
        '                         10 ^ (0.1 * (Bi[,7] + 3.32 * Ci[,7] * log10(Fi[i] / h[7]))) +
        '                         10 ^ (0.1 * (Bi[,8] + 3.32 * Ci[,8] * log10(Fi[i] / h[8]))) +
        '                         10 ^ (0.1 * (Bi[,9] + 3.32 * Ci[,9] * log10(Fi[i] / h[9]))) +
        '                         10 ^ (0.1 * (Bi[,10] + 3.32 * Ci[,10] * log10(Fi[i] / h[10]))) +
        '                         10 ^ (0.1 * (Bi[,11] + 3.32 * Ci[,11] * log10(Fi[i] / h[11]))) +
        '                         10 ^ (0.1 * (Bi[,12] + 3.32 * Ci[,12] * log10(Fi[i] / h[12]))) +
        '                         10 ^ (0.1 * (Bi[,13] + 3.32 * Ci[,13] * log10(Fi[i] / h[13])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6]))) +
        '                         10 ^ (0.1 * (Bi[,7] + 3.32 * Ci[,7] * log10(Fi[i] / h[7]))) +
        '                         10 ^ (0.1 * (Bi[,8] + 3.32 * Ci[,8] * log10(Fi[i] / h[8]))) +
        '                         10 ^ (0.1 * (Bi[,9] + 3.32 * Ci[,9] * log10(Fi[i] / h[9]))) +
        '                         10 ^ (0.1 * (Bi[,10] + 3.32 * Ci[,10] * log10(Fi[i] / h[10]))) +
        '                         10 ^ (0.1 * (Bi[,11] + 3.32 * Ci[,11] * log10(Fi[i] / h[11]))) +
        '                         10 ^ (0.1 * (Bi[,12] + 3.32 * Ci[,12] * log10(Fi[i] / h[12]))) +
        '                         10 ^ (0.1 * (Bi[,13] + 3.32 * Ci[,13] * log10(Fi[i] / h[13]))) +
        '                         10 ^ (0.1 * (Bi[,14] + 3.32 * Ci[,14] * log10(Fi[i] / h[14])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6]))) +
        '                         10 ^ (0.1 * (Bi[,7] + 3.32 * Ci[,7] * log10(Fi[i] / h[7]))) +
        '                         10 ^ (0.1 * (Bi[,8] + 3.32 * Ci[,8] * log10(Fi[i] / h[8]))) +
        '                         10 ^ (0.1 * (Bi[,9] + 3.32 * Ci[,9] * log10(Fi[i] / h[9]))) +
        '                         10 ^ (0.1 * (Bi[,10] + 3.32 * Ci[,10] * log10(Fi[i] / h[10]))) +
        '                         10 ^ (0.1 * (Bi[,11] + 3.32 * Ci[,11] * log10(Fi[i] / h[11]))) +
        '                         10 ^ (0.1 * (Bi[,12] + 3.32 * Ci[,12] * log10(Fi[i] / h[12]))) +
        '                         10 ^ (0.1 * (Bi[,13] + 3.32 * Ci[,13] * log10(Fi[i] / h[13]))) +
        '                         10 ^ (0.1 * (Bi[,14] + 3.32 * Ci[,14] * log10(Fi[i] / h[14]))) +
        '                         10 ^ (0.1 * (Bi[,15] + 3.32 * Ci[,15] * log10(Fi[i] / h[15])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6]))) +
        '                         10 ^ (0.1 * (Bi[,7] + 3.32 * Ci[,7] * log10(Fi[i] / h[7]))) +
        '                         10 ^ (0.1 * (Bi[,8] + 3.32 * Ci[,8] * log10(Fi[i] / h[8]))) +
        '                         10 ^ (0.1 * (Bi[,9] + 3.32 * Ci[,9] * log10(Fi[i] / h[9]))) +
        '                         10 ^ (0.1 * (Bi[,10] + 3.32 * Ci[,10] * log10(Fi[i] / h[10]))) +
        '                         10 ^ (0.1 * (Bi[,11] + 3.32 * Ci[,11] * log10(Fi[i] / h[11]))) +
        '                         10 ^ (0.1 * (Bi[,12] + 3.32 * Ci[,12] * log10(Fi[i] / h[12]))) +
        '                         10 ^ (0.1 * (Bi[,13] + 3.32 * Ci[,13] * log10(Fi[i] / h[13]))) +
        '                         10 ^ (0.1 * (Bi[,14] + 3.32 * Ci[,14] * log10(Fi[i] / h[14]))) +
        '                         10 ^ (0.1 * (Bi[,15] + 3.32 * Ci[,15] * log10(Fi[i] / h[15]))) +
        '                         10 ^ (0.1 * (Bi[,16] + 3.32 * Ci[,16] * log10(Fi[i] / h[16])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6]))) +
        '                         10 ^ (0.1 * (Bi[,7] + 3.32 * Ci[,7] * log10(Fi[i] / h[7]))) +
        '                         10 ^ (0.1 * (Bi[,8] + 3.32 * Ci[,8] * log10(Fi[i] / h[8]))) +
        '                         10 ^ (0.1 * (Bi[,9] + 3.32 * Ci[,9] * log10(Fi[i] / h[9]))) +
        '                         10 ^ (0.1 * (Bi[,10] + 3.32 * Ci[,10] * log10(Fi[i] / h[10]))) +
        '                         10 ^ (0.1 * (Bi[,11] + 3.32 * Ci[,11] * log10(Fi[i] / h[11]))) +
        '                         10 ^ (0.1 * (Bi[,12] + 3.32 * Ci[,12] * log10(Fi[i] / h[12]))) +
        '                         10 ^ (0.1 * (Bi[,13] + 3.32 * Ci[,13] * log10(Fi[i] / h[13]))) +
        '                         10 ^ (0.1 * (Bi[,14] + 3.32 * Ci[,14] * log10(Fi[i] / h[14]))) +
        '                         10 ^ (0.1 * (Bi[,15] + 3.32 * Ci[,15] * log10(Fi[i] / h[15]))) +
        '                         10 ^ (0.1 * (Bi[,16] + 3.32 * Ci[,16] * log10(Fi[i] / h[16]))) +
        '                         10 ^ (0.1 * (Bi[,17] + 3.32 * Ci[,17] * log10(Fi[i] / h[17])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6]))) +
        '                         10 ^ (0.1 * (Bi[,7] + 3.32 * Ci[,7] * log10(Fi[i] / h[7]))) +
        '                         10 ^ (0.1 * (Bi[,8] + 3.32 * Ci[,8] * log10(Fi[i] / h[8]))) +
        '                         10 ^ (0.1 * (Bi[,9] + 3.32 * Ci[,9] * log10(Fi[i] / h[9]))) +
        '                         10 ^ (0.1 * (Bi[,10] + 3.32 * Ci[,10] * log10(Fi[i] / h[10]))) +
        '                         10 ^ (0.1 * (Bi[,11] + 3.32 * Ci[,11] * log10(Fi[i] / h[11]))) +
        '                         10 ^ (0.1 * (Bi[,12] + 3.32 * Ci[,12] * log10(Fi[i] / h[12]))) +
        '                         10 ^ (0.1 * (Bi[,13] + 3.32 * Ci[,13] * log10(Fi[i] / h[13]))) +
        '                         10 ^ (0.1 * (Bi[,14] + 3.32 * Ci[,14] * log10(Fi[i] / h[14]))) +
        '                         10 ^ (0.1 * (Bi[,15] + 3.32 * Ci[,15] * log10(Fi[i] / h[15]))) +
        '                         10 ^ (0.1 * (Bi[,16] + 3.32 * Ci[,16] * log10(Fi[i] / h[16]))) +
        '                         10 ^ (0.1 * (Bi[,17] + 3.32 * Ci[,17] * log10(Fi[i] / h[17]))) +
        '                         10 ^ (0.1 * (Bi[,18] + 3.32 * Ci[,18] * log10(Fi[i] / h[18])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6]))) +
        '                         10 ^ (0.1 * (Bi[,7] + 3.32 * Ci[,7] * log10(Fi[i] / h[7]))) +
        '                         10 ^ (0.1 * (Bi[,8] + 3.32 * Ci[,8] * log10(Fi[i] / h[8]))) +
        '                         10 ^ (0.1 * (Bi[,9] + 3.32 * Ci[,9] * log10(Fi[i] / h[9]))) +
        '                         10 ^ (0.1 * (Bi[,10] + 3.32 * Ci[,10] * log10(Fi[i] / h[10]))) +
        '                         10 ^ (0.1 * (Bi[,11] + 3.32 * Ci[,11] * log10(Fi[i] / h[11]))) +
        '                         10 ^ (0.1 * (Bi[,12] + 3.32 * Ci[,12] * log10(Fi[i] / h[12]))) +
        '                         10 ^ (0.1 * (Bi[,13] + 3.32 * Ci[,13] * log10(Fi[i] / h[13]))) +
        '                         10 ^ (0.1 * (Bi[,14] + 3.32 * Ci[,14] * log10(Fi[i] / h[14]))) +
        '                         10 ^ (0.1 * (Bi[,15] + 3.32 * Ci[,15] * log10(Fi[i] / h[15]))) +
        '                         10 ^ (0.1 * (Bi[,16] + 3.32 * Ci[,16] * log10(Fi[i] / h[16]))) +
        '                         10 ^ (0.1 * (Bi[,17] + 3.32 * Ci[,17] * log10(Fi[i] / h[17]))) +
        '                         10 ^ (0.1 * (Bi[,18] + 3.32 * Ci[,18] * log10(Fi[i] / h[18]))) +
        '                         10 ^ (0.1 * (Bi[,19] + 3.32 * Ci[,19] * log10(Fi[i] / h[19])))
        '  )

        '  i <- i + 1
        '  Zi[,i] <- 10 * log10(10 ^ (0.1 * Ni_[,i]) + 
        '                         10 ^ (0.1 * (Bi[,1] + 3.32 * Ci[,1] * log10(Fi[i] / h[1]))) +
        '                         10 ^ (0.1 * (Bi[,2] + 3.32 * Ci[,2] * log10(Fi[i] / h[2]))) +
        '                         10 ^ (0.1 * (Bi[,3] + 3.32 * Ci[,3] * log10(Fi[i] / h[3]))) +
        '                         10 ^ (0.1 * (Bi[,4] + 3.32 * Ci[,4] * log10(Fi[i] / h[4]))) +
        '                         10 ^ (0.1 * (Bi[,5] + 3.32 * Ci[,5] * log10(Fi[i] / h[5]))) +
        '                         10 ^ (0.1 * (Bi[,6] + 3.32 * Ci[,6] * log10(Fi[i] / h[6]))) +
        '                         10 ^ (0.1 * (Bi[,7] + 3.32 * Ci[,7] * log10(Fi[i] / h[7]))) +
        '                         10 ^ (0.1 * (Bi[,8] + 3.32 * Ci[,8] * log10(Fi[i] / h[8]))) +
        '                         10 ^ (0.1 * (Bi[,9] + 3.32 * Ci[,9] * log10(Fi[i] / h[9]))) +
        '                         10 ^ (0.1 * (Bi[,10] + 3.32 * Ci[,10] * log10(Fi[i] / h[10]))) +
        '                         10 ^ (0.1 * (Bi[,11] + 3.32 * Ci[,11] * log10(Fi[i] / h[11]))) +
        '                         10 ^ (0.1 * (Bi[,12] + 3.32 * Ci[,12] * log10(Fi[i] / h[12]))) +
        '                         10 ^ (0.1 * (Bi[,13] + 3.32 * Ci[,13] * log10(Fi[i] / h[13]))) +
        '                         10 ^ (0.1 * (Bi[,14] + 3.32 * Ci[,14] * log10(Fi[i] / h[14]))) +
        '                         10 ^ (0.1 * (Bi[,15] + 3.32 * Ci[,15] * log10(Fi[i] / h[15]))) +
        '                         10 ^ (0.1 * (Bi[,16] + 3.32 * Ci[,16] * log10(Fi[i] / h[16]))) +
        '                         10 ^ (0.1 * (Bi[,17] + 3.32 * Ci[,17] * log10(Fi[i] / h[17]))) +
        '                         10 ^ (0.1 * (Bi[,18] + 3.32 * Ci[,18] * log10(Fi[i] / h[18]))) +
        '                         10 ^ (0.1 * (Bi[,19] + 3.32 * Ci[,19] * log10(Fi[i] / h[19]))) +
        '                         10 ^ (0.1 * (Bi[,20] + 3.32 * Ci[,20] * log10(Fi[i] / h[20])))
        '  )


        '  # Calculating Equivalent internal noise spectrum
        '  Xi_f <- data.frame("i_150" = rep(Xi[1],DataLength), 
        '                     "i_250" = rep(Xi[2],DataLength), 
        '                     "i_350" = rep(Xi[3],DataLength), 
        '                     "i_450" = rep(Xi[4],DataLength), 
        '                     "i_570" = rep(Xi[5],DataLength), 
        '                     "i_700" = rep(Xi[6],DataLength), 
        '                     "i_840" = rep(Xi[7],DataLength), 
        '                     "i_1000" = rep(Xi[8],DataLength), 
        '                     "i_1170" = rep(Xi[9],DataLength), 
        '                     "i_1370" = rep(Xi[10],DataLength), 
        '                     "i_1600" = rep(Xi[11],DataLength), 
        '                     "i_1850" = rep(Xi[12],DataLength), 
        '                     "i_2150" = rep(Xi[13],DataLength), 
        '                     "i_2500" = rep(Xi[14],DataLength), 
        '                     "i_2900" = rep(Xi[15],DataLength), 
        '                     "i_3400" = rep(Xi[16],DataLength), 
        '                     "i_4000" = rep(Xi[17],DataLength), 
        '                     "i_4800" = rep(Xi[18],DataLength), 
        '                     "i_5800" = rep(Xi[19],DataLength), 
        '                     "i_7000" = rep(Xi[20],DataLength), 
        '                     "i_8500" = rep(Xi[21],DataLength))

        '  Xi_ = Xi_f + Ti_

        '  # Calculating the Equivalent disturbance level (Summing on linear scale instead of selecting the max value as in SII)
        '  #Di <- 10 * log10( (10^(-12) * 10^(Zi/10) + 10^(-12) * 10^(Xi_/10)) / 10^(-12)) # Actually 10^(-12) is cancelled out!
        '  Di <- 10 * log10( 10^(Zi/10) + 10^(Xi_/10) )

        '  # Calculating the Speech-to-Disturbance ratio  
        '  SDRi <- Ei_ - Di

        '  #Fixing column names
        '  colnames(SDRi) <- c("i_150", "i_250", "i_350", "i_450", "i_570", "i_700", "i_840", "i_1000", 
        '                       "i_1170", "i_1370", "i_1600", "i_1850", "i_2150", "i_2500", "i_2900", 
        '                       "i_3400", "i_4000", "i_4800", "i_5800", "i_7000", "i_8500")

        '  Output <- list("SDRi" = SDRi)

        '  # # Calculating the Speech-to-Disturbance ratio  
        '  # SDRidB <- Ei_ - Di
        '  # #hist(SDRi$InData.PSL_1000)
        '  # 
        '  ## Converting the Speech-to-Disturbance ratio to linear scale using power conversion
        '  ## (The canverted value can be interpreted as available the sound-power increase due to the speech in each frequency band, as compared to the noise)
        '  ## Conceptually similar to the SII band audibility function, but on a linear scale instead of a dB scale.
        '  #SDRi <- 10^(-12) * 10^((SDRidB)/10)
        '  ##hist(SDRiLin$InData.PSL_1000)
        '  ##min(SDRiLin$InData.PSL_1000)

        '  # #Fixing column names
        '  # colnames(SDRi) <- c("i_150", "i_250", "i_350", "i_450", "i_570", "i_700", "i_840", "i_1000", 
        '  #                     "i_1170", "i_1370", "i_1600", "i_1850", "i_2150", "i_2500", "i_2900", 
        '  #                     "i_3400", "i_4000", "i_4800", "i_5800", "i_7000", "i_8500")
        '  # 
        '  # Output <- list("SDRi" = SDRi)

        '  return(Output)  

        '}

    End Module

End Namespace