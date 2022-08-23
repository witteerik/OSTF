
Namespace SipTest


    Public Class TestSession

        ''' <summary>
        ''' Stores the test units presented in the test session
        ''' </summary>
        ''' <returns></returns>
        Public Property TestUnits As New List(Of SiPTestUnit)

        ''' <summary>
        ''' Stores references to SiP-test trials in the order that they were presented.
        ''' </summary>
        ''' <returns></returns>
        Public Property TestTrialHistory As New List(Of SipTrial)

        ''' <summary>
        ''' Holds settings that determine how the test should enfold.
        ''' </summary>
        ''' <returns></returns>
        Public Property TestProcedure As TestProcedure



        Public Property SelectedAudiogramData As AudiogramData = Nothing
        Public Property HearingAidGain As HearingAidGainData = Nothing
        Public Property ReferenceLevel As Nullable(Of Double) = Nothing
        Public Property HearingAidGainType As Nullable(Of HearingAidGainData.GainTypes)

        'Public Property SipTestPresetName As Nullable(Of SipTestPresets) = Nothing

        'Public Enum SipTestPresets
        '    Måttlig_A
        '    Måttlig_B_Fallande
        '    Grav_A
        '    Grav_B_Fallande
        'End Enum


    End Class


    Public Class SiPTestUnit

        Public Property ParentTestSession As TestSession

        Public Property SpeechMaterialComponents As New List(Of SpeechMaterialComponent)

        Public Property TestTrialHistory As New List(Of SipTrial)


        Public Sub New(ByRef ParentTestSession As TestSession)
            Me.ParentTestSession = ParentTestSession
        End Sub

        Public Function GetNextTrial() As SipTrial

            Dim TestProcedure = ParentTestSession.TestProcedure

            Dim SelectedSmcIndex As Integer = 0
            Dim SelectedSpeechMaterialComponent = SpeechMaterialComponents(SelectedSmcIndex)

            Dim SelectedMediaSet As MediaSet = Nothing

            Return New SipTrial(Me, SelectedSpeechMaterialComponent, SelectedMediaSet)

        End Function

    End Class


    Public Class SipTrial

        ''' <summary>
        ''' Holds a reference to the test unit to which the current test trial belongs
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ParentTestUnit As SiPTestUnit

        ''' <summary>
        ''' Holds a reference to the SpeechMaterialComponent presented in the trial.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SpeechMaterialComponent As SpeechMaterialComponent

        ''' <summary>
        ''' Holds a reference to the MediaSet used in the trial.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MediaSet As MediaSet

        Public Property TrialResult As TestTrialResults

        Public Property TestTrialResponse As String = ""

        'Sound Levels


        Public Sub New(ByRef ParentTestUnit As SiPTestUnit,
                       ByRef SpeechMaterialComponent As SpeechMaterialComponent,
                       ByRef MediaSet As MediaSet)

            Me.ParentTestUnit = ParentTestUnit
            Me.SpeechMaterialComponent = SpeechMaterialComponent

        End Sub


        Private _PredictedSuccessProbability As Double

        Public Function GetPredictedSuccessProbability() As Double

            'Getting PDL
            Dim PDL As Double = Me.PhonemeDiscriminabilityLevel

            'Getting BPTAH
            Dim BPTAH As Double = Me.ParentTestUnit.ParentTestSession.SelectedAudiogramData.BPTAH

            'Calculating centred and scaled values for PDL and PBTAH
            Dim PDL_gmc_div = (PDL - 3.917) / 50
            Dim BPTAH_gmc_div = (BPTAH - 39.82) / 100

            'Calculating model eta
            Dim Eta = 0.019 +
                9.996 * PDL_gmc_div +
                -1.758 * BPTAH_gmc_div +
                -5.34 * PDL_gmc_div * BPTAH_gmc_div

            'Calculating estimated success probability
            Dim Output As Double = 1 / 3 + (2 / 3) * (1 / (1 + Math.Exp(-Eta)))

            'Stores the result in PredictedSuccessProbability
            _PredictedSuccessProbability = Output

            Return Output

        End Function

        Public Sub UpdateEstimatedSuccessProbability()


        End Sub

        Private _PhonemeDiscriminabilityLevel As Double
        Private UpdatePdlOnNextCall As Boolean = True
        Public ReadOnly Property PhonemeDiscriminabilityLevel(Optional ByVal PhonemeSpectrumLevelsVariableNamePrefix As String = "SLs",
                                                              Optional ByVal MaskerSpectrumLevelsVariableNamePrefix As String = "SLm") As Double
            Get
                If UpdatePdlOnNextCall = True Then

                    'N.B. Variable name abbreviations used:
                    ' RLxs = ReferenceContrastingPhonemesLevel (dB FS)
                    ' SLs = Phoneme Spectrum Levels (PSL, dB SPL)
                    ' SLm = Environment / Masker Spectrum Levels (ESL, dB SPL)


                    'Using thresholds and gain data from the side with the best aided thresholds (selecting side separately for each critical band)
                    Dim Thresholds(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1) As Double
                    Dim Gain(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1) As Double

                    For i = 0 To Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1
                        'TODO: should we allow for the lack of gain data here, or should we always use a gain of zero when no hearing aid is used?
                        Dim AidedThreshold_Left As Double = Me.ParentTestUnit.ParentTestSession.SelectedAudiogramData.Cb_Left_AC(i) - Me.ParentTestUnit.ParentTestSession.HearingAidGain.LeftSideGain(i)
                        Dim AidedThreshold_Right As Double = Me.ParentTestUnit.ParentTestSession.SelectedAudiogramData.Cb_Right_AC(i) - Me.ParentTestUnit.ParentTestSession.HearingAidGain.RightSideGain(i)

                        If AidedThreshold_Left < AidedThreshold_Right Then
                            Thresholds(i) = Me.ParentTestUnit.ParentTestSession.SelectedAudiogramData.Cb_Left_AC(i)
                            Gain(i) = Me.ParentTestUnit.ParentTestSession.HearingAidGain.LeftSideGain(i)
                        Else
                            Thresholds(i) = Me.ParentTestUnit.ParentTestSession.SelectedAudiogramData.Cb_Right_AC(i)
                            Gain(i) = Me.ParentTestUnit.ParentTestSession.HearingAidGain.RightSideGain(i)
                        End If
                    Next

                    'Getting spectral levels
                    Dim TestPhonemeSpectralLevels(20) As Double
                    Dim IncorrectResponse1TestPhonemeSpectralLevels(20) As Double
                    Dim IncorrectResponse2TestPhonemeSpectralLevels(20) As Double
                    Dim MaskerSpectralLevels(20) As Double
                    Dim Siblings = Me.SpeechMaterialComponent.GetSiblingsExcludingSelf

                    For i = 0 To Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1
                        Dim VariableNameSuffix = Math.Round(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies(i)).ToString("00000")
                        Dim SLsName As String = PhonemeSpectrumLevelsVariableNamePrefix & "_" & VariableNameSuffix
                        Dim SLmName As String = MaskerSpectrumLevelsVariableNamePrefix & "_" & VariableNameSuffix

                        TestPhonemeSpectralLevels(i) = Me.SpeechMaterialComponent.GetNumericMediaSetVariableValue(MediaSet, SLsName)
                        IncorrectResponse1TestPhonemeSpectralLevels(i) = Siblings(0).GetNumericMediaSetVariableValue(MediaSet, SLsName)
                        IncorrectResponse2TestPhonemeSpectralLevels(i) = Siblings(0).GetNumericMediaSetVariableValue(MediaSet, SLsName)
                        MaskerSpectralLevels(i) = Me.SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).GetNumericMediaSetVariableValue(MediaSet, SLmName)
                    Next


                    'Calculating SDRs
                    Dim SDRt = PDL.CalculateSDR(TestPhonemeSpectralLevels, MaskerSpectralLevels, Thresholds, Gain, True, True)
                    Dim SDRc1 = PDL.CalculateSDR(IncorrectResponse1TestPhonemeSpectralLevels, MaskerSpectralLevels, Thresholds, Gain, True, True)
                    Dim SDRc2 = PDL.CalculateSDR(IncorrectResponse2TestPhonemeSpectralLevels, MaskerSpectralLevels, Thresholds, Gain, True, True)

                    'Calculating PDL
                    _PhonemeDiscriminabilityLevel = PDL.CalculatePDL(SDRt, SDRc1, SDRc2)

                    UpdatePdlOnNextCall = False
                End If
                Return _PhonemeDiscriminabilityLevel
            End Get
        End Property

    End Class

    Public Enum TestTrialResults
        Correct
        Incorrect
        Missing
    End Enum

    Public Enum AdaptiveTypes
        SimpleUpDown
        Fixed
    End Enum

    Public Class TestProcedure

        Property AdaptiveType As AdaptiveTypes

        Public Sub New(ByVal AdaptiveType As AdaptiveTypes)
            Me.AdaptiveType = AdaptiveType
        End Sub

    End Class


End Namespace