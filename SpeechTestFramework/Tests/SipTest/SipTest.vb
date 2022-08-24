
Namespace SipTest

    'N.B. Variable name abbreviations used:
    ' RLxs = ReferenceContrastingPhonemesLevel (dB FS)
    ' SLs = Phoneme Spectrum Levels (PSL, dB SPL)
    ' SLm = Environment / Masker Spectrum Levels (ESL, dB SPL)
    ' Lc = Component Level (TestWord_ReferenceSPL, dB SPL), The average sound level of all recordings of the speech material component
    ' Tc = Component temporal duration (in seconds)
    ' V = HasVowelContrast (1 = Yes, 0 = No)

    Public Class TestSession

        Public Property ParentTestSpecification As TestSpecification

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


        Public Property PlannedTrials As New List(Of SipTrial)

        ''' <summary>
        ''' Holds settings that determine how the test should enfold.
        ''' </summary>
        ''' <returns></returns>
        Public Property TestProcedure As New TestProcedure(AdaptiveTypes.Fixed)


        Public Property SelectedAudiogramData As AudiogramData = Nothing
        Public Property HearingAidGain As HearingAidGainData = Nothing
        Public Property ReferenceLevel As Nullable(Of Double) = Nothing
        Public Property HearingAidGainType As Nullable(Of HearingAidGainData.GainTypes)

        Public Property SelectedMediaSetName As String = "" ' If not selected random media sets can be assigned to different trials

        Public Property SelectedPresetName As String = ""

        Public ReadOnly Property Patient As Patient

        Public Property SelectedPnr As Nullable(Of Double) = Nothing

        Friend Randomizer As Random

        Public Sub New(ByRef Patient As Patient, ByRef ParentTestSpecification As TestSpecification, Optional RandomSeed As Integer? = Nothing)

            If RandomSeed.HasValue = True Then
                Randomizer = New Random(RandomSeed)
            Else
                Randomizer = New Random
            End If

            Me.Patient = Patient
            Me.ParentTestSpecification = ParentTestSpecification

            'Setting a default preset (the first in the preset list)
            Me.SelectedPresetName = Me.ParentTestSpecification.SpeechMaterial.Presets.Keys(0)

        End Sub

        Public Sub PlanTestTrials(ByRef AvailableMediaSet As MediaSetLibrary, Optional ByVal RandomSeed As Integer? = Nothing)

            ClearTrials()

            If RandomSeed.HasValue Then Randomizer = New Random(RandomSeed)

            For r = 1 To TestProcedure.LengthReduplications

                For Each PresetComponent In ParentTestSpecification.SpeechMaterial.Presets(SelectedPresetName)

                    Dim NewTestUnit = New SiPTestUnit(Me)

                    Dim TestWords = PresetComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
                    NewTestUnit.SpeechMaterialComponents.AddRange(TestWords)

                    If SelectedMediaSetName <> "" Then
                        'Adding from the selected media set
                        NewTestUnit.PlanTrials(AvailableMediaSet.GetMediaSet(SelectedMediaSetName))
                        TestUnits.Add(NewTestUnit)
                    Else
                        'Adding from random media sets
                        Dim RandomIndex = Randomizer.Next(0, AvailableMediaSet.Count)
                        NewTestUnit.PlanTrials(AvailableMediaSet(RandomIndex))
                        TestUnits.Add(NewTestUnit)
                    End If

                Next
            Next

            For Each Unit In TestUnits
                For Each Trial In Unit.PlannedTrials
                    PlannedTrials.Add(Trial)
                Next
            Next

            If TestProcedure.RandomizeOrder = True Then
                Dim RandomList As New List(Of SipTrial)
                Do Until PlannedTrials.Count = 0
                    Dim RandomIndex As Integer = Randomizer.Next(0, PlannedTrials.Count)
                    RandomList.Add(PlannedTrials(RandomIndex))
                    PlannedTrials.RemoveAt(RandomIndex)
                Loop
                PlannedTrials = RandomList
            End If

        End Sub

        Public Sub ClearTrials()
            TestUnits.Clear()
            PlannedTrials.Clear()
        End Sub

        Public Function CalculateEstimatedPsychometricFunction(Optional ByVal PNRs As List(Of Double) = Nothing) As SortedList(Of Double, Tuple(Of Double, Double, Double))

            If PNRs Is Nothing Then
                PNRs = New List(Of Double) From {-15, -12, -10, -8, -6, -4, -2, 0, 2, 4, 6, 8, 10, 12, 15}
            End If

            'Stores the original PNR
            Dim OriginalPNR As Double? = Me.SelectedPnr

            'Pnr, Estimate, lower critical boundary, upper critical boundary
            Dim Output As New SortedList(Of Double, Tuple(Of Double, Double, Double))

            For Each pnr In PNRs

                Me.SetLevels(Me.ReferenceLevel, pnr)

                Output.Add(pnr, New Tuple(Of Double, Double, Double)(Me.CalculateEstimatedMeanScore(), 0, 0))

            Next


            'Resets the PNR
            If OriginalPNR.HasValue Then
                Me.SetLevels(Me.ReferenceLevel, OriginalPNR)
            End If

            Return Output

        End Function

        ''' <summary>
        ''' Sets all levels in all trials. (Levels should be set prior to mixing the sounds, or prior to success probability estimation.)
        ''' </summary>
        Public Sub SetLevels(ByVal ReferenceLevel As Double, ByVal PNR As Double)

            For Each TestUnit In Me.TestUnits
                For Each TestTrial In TestUnit.PlannedTrials
                    TestTrial.SetLevels(ReferenceLevel, PNR)
                Next
            Next

        End Sub

        Public Property EstimatedMeanScore As Double

        Public Function CalculateEstimatedMeanScore() As Double

            Dim TrialSuccessProbabilityList As New List(Of Double)

            For Each TestUnit In Me.TestUnits
                For Each PlannedTestTrial In TestUnit.PlannedTrials
                    TrialSuccessProbabilityList.Add(PlannedTestTrial.EstimatedSuccessProbability(True))
                Next
            Next

            If TrialSuccessProbabilityList.Count > 0 Then
                Return TrialSuccessProbabilityList.Average()
            Else
                Return -1
            End If

        End Function

#Region "GUI"

        Public Function GetGuiTableData() As GuiTableData

            Dim Output As New GuiTableData

            Dim LastPresentedTrialIndex As Integer = 0
            For i = 0 To PlannedTrials.Count - 1

                Output.TestWords.Add(PlannedTrials(i).SpeechMaterialComponent.PrimaryStringRepresentation)

                If PlannedTrials(i).ObservedResponse IsNot Nothing Then
                    Output.Responses.Add(PlannedTrials(i).ObservedResponse.ObservedResponseSpelling)
                Else
                    Output.Responses.Add("")
                End If

                Output.ResultResponseTypes.Add(PlannedTrials(i).GetResponseType)

                If PlannedTrials(i).GetResponseType <> ResponseType.NotPresented Then
                    LastPresentedTrialIndex = i
                End If

            Next

            Output.SelectionRow = LastPresentedTrialIndex
            Output.FirstRowToDisplayInScrollmode = Math.Max(0, LastPresentedTrialIndex - 7)

            Return Output

        End Function

        Public Class GuiTableData
            Public TestWords As New List(Of String)
            Public Responses As New List(Of String)
            Public ResultResponseTypes As New List(Of SipTest.ResultResponseType)
            Public UpdateRow As Integer? = Nothing
            Public SelectionRow As Integer? = Nothing
            Public FirstRowToDisplayInScrollmode As Integer? = Nothing

        End Class

#End Region


    End Class

    Public Class SipTestResponse

        Public ReadOnly Property CreateDate As DateTime
        Public ReadOnly Property Correct As Boolean
        Public ReadOnly Property ObservedResponseSpelling As String
        Public ReadOnly Property CorrectResponseSpelling As String

        ''' <summary>
        ''' Creates a new instance of a SipTestResponse.
        ''' </summary>
        ''' <param name="ObservedResponseSpelling">The spelling of the observed response. Pass an empty string for missing responses.</param>
        ''' <param name="CorrectResponseSpelling">The spelling of the correct response. </param>
        Public Sub New(ByVal ObservedResponseSpelling As String, ByVal CorrectResponseSpelling As String)
            'Storing the date and time when the instance of this SipTestResult was created
            CreateDate = DateTime.Now
            Me.ObservedResponseSpelling = ObservedResponseSpelling
            Me.CorrectResponseSpelling = CorrectResponseSpelling

            'Setting the value of Correct
            If ObservedResponseSpelling = CorrectResponseSpelling Then
                Correct = True
            End If

        End Sub

        ''' <summary>
        ''' Determines if the observed response is a missing response
        ''' </summary>
        ''' <returns></returns>
        Public Function IsMissingResponse() As Boolean
            If ObservedResponseSpelling = "" Then
                Return True
            Else
                Return False
            End If
        End Function

    End Class



    Public Class SiPTestUnit

        Public Property ParentTestSession As TestSession

        Public Property SpeechMaterialComponents As New List(Of SpeechMaterialComponent)

        Public Property PlannedTrials As New List(Of SipTrial)

        Public Property TestTrialHistory As New List(Of SipTrial)


        Public Sub New(ByRef ParentTestSession As TestSession)
            Me.ParentTestSession = ParentTestSession
        End Sub

        Public Sub PlanTrials(ByRef MediaSet As MediaSet)

            PlannedTrials.Clear()

            Select Case ParentTestSession.TestProcedure.AdaptiveType
                Case AdaptiveTypes.Fixed

                    'For n = 1 To ParentTestSession.TestProcedure.LengthReduplications ' Should this be done here, or at a higher level?
                    For c = 0 To SpeechMaterialComponents.Count - 1
                        Dim NewTrial As New SipTrial(Me, SpeechMaterialComponents(c), MediaSet)
                        PlannedTrials.Add(NewTrial)
                    Next
                    'Next

            End Select

        End Sub

        Public Function GetNextTrial(ByRef rnd As Random) As SipTrial

            Select Case ParentTestSession.TestProcedure.AdaptiveType
                Case AdaptiveTypes.Fixed

                    If PlannedTrials.Count = 0 Then Return Nothing

                    If ParentTestSession.TestProcedure.RandomizeOrder = True Then
                        Dim RandomIndex = rnd.Next(0, PlannedTrials.Count)
                        Dim NextTrial = PlannedTrials(RandomIndex)
                        'Removing the trial from PlannedTrials
                        PlannedTrials.RemoveAt(RandomIndex)
                        Return NextTrial
                    Else
                        Dim NextTrial = PlannedTrials(0)
                        'Removing the trial from PlannedTrials
                        PlannedTrials.RemoveAt(0)
                        Return NextTrial
                    End If

                Case Else

                    Throw New NotImplementedException

            End Select

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

        Public Property ObservedResponse As SipTestResponse

        Public Property TrialResult As ResultResponseType

        Public Property TestTrialResponse As String = ""

        'Sound Levels


        Public Sub New(ByRef ParentTestUnit As SiPTestUnit,
                       ByRef SpeechMaterialComponent As SpeechMaterialComponent,
                       ByRef MediaSet As MediaSet)

            Me.ParentTestUnit = ParentTestUnit
            Me.SpeechMaterialComponent = SpeechMaterialComponent
            Me.MediaSet = MediaSet

            'Setting some levels
            Dim Fs2Spl As Double = Audio.PortAudioVB.DuplexMixer.Simulated_dBFS_dBSPL_Difference

            ReferenceSpeechMaterialLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.ListCollection).GetNumericMediaSetVariableValue(MediaSet, "Lc")
            ReferenceTestWordLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetNumericMediaSetVariableValue(MediaSet, "Lc") 'TestStimulus.TestWord_ReferenceSPL
            ReferenceContrastingPhonemesLevel_SPL = Fs2Spl + SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).GetNumericMediaSetVariableValue(MediaSet, "RLxs")


        End Sub


        Public Property Reference_SPL As Double
        Public Property PNR As Double

        Private _TargetMasking_SPL As Double? = Nothing

        ''' <summary>
        ''' Returns the intended masker sound level at the position of the listeners head, or Nothing if not calculated
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TargetMasking_SPL As Double?
            Get
                Return _TargetMasking_SPL
            End Get
        End Property

        Public Property ContextRegionSpeech_SPL As Double

        Private Property _TestWordLevel As Double? = Nothing
        Public ReadOnly Property TestWordLevel As Double?
            Get
                Return _TestWordLevel
            End Get
        End Property
        Public ReadOnly Property ReferenceSpeechMaterialLevel_SPL As Double
        Public ReadOnly Property ReferenceTestWordLevel_SPL As Double
        Public ReadOnly Property ReferenceContrastingPhonemesLevel_SPL As Double

        Public Property TestWordLevelLimit As Double = 82.3 '82.3 Shouted Speech level i SII standard
        Public Property ContextSpeechLimit As Double = 74.85 '74.85 Loud Speech Level From SII standard

        ''' <summary>
        ''' Sets all levels in the current test trial. Levels should be set prior to mixing the sound.
        ''' </summary>
        Public Sub SetLevels(ByVal ReferenceLevel As Double, ByVal PNR As Double)

            'Setting UpdatePdlOnNextCall to True to signal recalculation of PDL when needed next time (TODO: this could be skipped if the ReferenceLevel and PNR are not changed since last call)
            UpdatePdlOnNextCall = True

            Me.Reference_SPL = ReferenceLevel
            Me.PNR = PNR

            'Calculating the difference between the standard ReferenceSpeechMaterialLevel_SPL (68.34 dB SPL) reference level and the one currently used
            Dim RefLevelDifference As Double = ReferenceLevel - ReferenceSpeechMaterialLevel_SPL

            '0. Gettings some levels
            Dim ContrastingPhonemesLevel_SPL = ReferenceContrastingPhonemesLevel_SPL + RefLevelDifference

            '1. Setting the noise level
            Dim SNR_Type As String = "PNR"
            If SNR_Type = "PNR" Then
                'In this procedure, CurrentSNR represents the sound level difference between the average max level of the contrasting test phonemes, and the masker sound
                'Setting the test word masker to Lcp

                'Setting TargetMasking_SPL to ContrastingPhonemesLevel_SPL
                _TargetMasking_SPL = ContrastingPhonemesLevel_SPL

            ElseIf SNR_Type = "SNR_SpeechMaterial" Then
                'In this procedure, CurrentSNR represents the sound level difference between the average level of the whole speech material, and the masker sound
                'Setting the TargetMasking_SPL to Lsm
                _TargetMasking_SPL = ReferenceSpeechMaterialLevel_SPL + RefLevelDifference
            Else
                Throw New NotImplementedException()
            End If

            '2. Adjusting the speech level to attain the desired PNR

            'Calculating the unlimited target test word level
            Dim TargetTestWord_SPL = ReferenceTestWordLevel_SPL + RefLevelDifference + PNR

            'Calculating the average speech material level equivalent to the current TargetTestWord_SPL
            Dim CurrentAverageSpeechMaterial_SPL = ReferenceSpeechMaterialLevel_SPL + RefLevelDifference + PNR

            'Setting the ContextRegionSpeech_SPL to the CurrentAverageSpeechMaterial_SPL                    
            ContextRegionSpeech_SPL = CurrentAverageSpeechMaterial_SPL

            '3. Limiting test word level
            If TargetTestWord_SPL > TestWordLevelLimit Then

                Dim Difference As Double = TargetTestWord_SPL - TestWordLevelLimit

                'Decreasing all levels set by Difference, to retain the desired test SNR
                _TargetMasking_SPL -= Difference
                TargetTestWord_SPL -= Difference
                ContextRegionSpeech_SPL -= Difference
            End If

            _TestWordLevel = TargetTestWord_SPL

            '4. Limiting the context speech level
            If ContextRegionSpeech_SPL > ContextSpeechLimit Then

                Dim Difference As Double = ContextRegionSpeech_SPL - ContextSpeechLimit

                'Decreasing the ContextRegionForegroundLevel_SPL to ContextSpeechLimit
                ContextRegionSpeech_SPL -= Difference

            End If

        End Sub


        Public Function GetCurrentSpeechGain() As Double
            Dim SpeechGain As Double = TestWordLevel - ReferenceTestWordLevel_SPL ' SpeechMaterialComponent.GetNumericMediaSetVariableValue(MediaSet, "Lc") 'TestStimulus.TestWord_ReferenceSPL
            Return SpeechGain
        End Function

        Public Function GetCurrentMaskerGain() As Double
            Dim CurrentMaskerGain = TargetMasking_SPL - 70 'TODO: In the SiP-test maskers sound files the level is set to -30 dB FS across the whole sounds. A more detailed sound level data could be used instead!
            Return CurrentMaskerGain
        End Function



        Public _EstimatedSuccessProbability As Double? = Nothing

        Public ReadOnly Property EstimatedSuccessProbability(ByVal ReCalculate As Boolean) As Double
            Get
                If _EstimatedSuccessProbability.HasValue = False Or ReCalculate = True Then
                    UpdateEstimatedSuccessProbability()
                End If
                Return _EstimatedSuccessProbability
            End Get
        End Property

        Public Sub UpdateEstimatedSuccessProbability()

            'Select Case Me.ParentTestUnit.ParentTestSession.Prediction.Models.SelectedModel.GetSwedishSipTestA

            'Getting predictors
            Dim PDL As Double = Me.PhonemeDiscriminabilityLevel
            Dim TPD As Double = Me.SpeechMaterialComponent.GetNumericMediaSetVariableValue(MediaSet, "Tc")
            Dim Z As Double = Me.SpeechMaterialComponent.GetNumericVariableValue("Z")
            Dim iPNDP As Double = 1 / Me.SpeechMaterialComponent.GetNumericVariableValue("PNDP")
            Dim PP As Double = Me.SpeechMaterialComponent.GetNumericVariableValue("PP")
            Dim PT As Double = Me.SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).GetNumericVariableValue("V")

            'Calculating centred and scaled values for PDL and PBTAH
            Dim PDL_gmc_div = (PDL - 10.46) / 50
            Dim Z_gmc_div = (Z - 3.7) / 10
            Dim iPNDP_gmc_div = (iPNDP - 20.6) / 50

            'Calculating model eta
            Dim Eta = 0.73 +
                8.22 * PDL_gmc_div +
                5.11 * (TPD - 0.33) +
                4.24 * Z_gmc_div -
                1.44 * iPNDP_gmc_div +
                4.58 * (PP - 0.92) -
                1.1 * (PT - 0.43) -
                7.25 * PDL_gmc_div * Z_gmc_div +
                7.47 * PDL_gmc_div * iPNDP_gmc_div +
                3.32 * PDL_gmc_div * (PT - 0.43)

            'Calculating estimated success probability
            Dim p As Double = 1 / 3 + (2 / 3) * (1 / (1 + Math.Exp(-Eta)))

            'Stores the result in PredictedSuccessProbability
            _EstimatedSuccessProbability = p

        End Sub


        Private _PhonemeDiscriminabilityLevel As Double
        Private UpdatePdlOnNextCall As Boolean = True

        Public ReadOnly Property PhonemeDiscriminabilityLevel(Optional ByVal SpeechSpectrumLevelsVariableNamePrefix As String = "SLs",
                                                              Optional ByVal MaskerSpectrumLevelsVariableNamePrefix As String = "SLm") As Double
            Get
                If UpdatePdlOnNextCall = True Then

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
                    Dim CorrectResponseSpectralLevels(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1) As Double
                    Dim MaskerSpectralLevels(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1) As Double
                    Dim Siblings = Me.SpeechMaterialComponent.GetSiblingsExcludingSelf
                    Dim IncorrectResponsesSpectralLevels As New List(Of Double())
                    For Each Sibling In Siblings
                        Dim IncorrectResponseSpectralLevels(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1) As Double
                        IncorrectResponsesSpectralLevels.Add(IncorrectResponseSpectralLevels)
                    Next

                    'Getting the current gain, compared to the reference test-word and masker levels
                    Dim CurrentSpeechGain As Double = GetCurrentSpeechGain()
                    Dim CurrentMaskerGain As Double = GetCurrentMaskerGain()

                    For i = 0 To Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies.Length - 1
                        Dim VariableNameSuffix = Math.Round(Audio.DSP.PsychoAcoustics.SiiCriticalBands.CentreFrequencies(i)).ToString("00000")
                        Dim SLsName As String = SpeechSpectrumLevelsVariableNamePrefix & "_" & VariableNameSuffix
                        Dim SLmName As String = MaskerSpectrumLevelsVariableNamePrefix & "_" & VariableNameSuffix

                        'Retreiving the reference levels and adjusts them by the CurrentSpeechGain and CurrentMaskerGain
                        CorrectResponseSpectralLevels(i) = Me.SpeechMaterialComponent.GetNumericMediaSetVariableValue(MediaSet, SLsName) + CurrentSpeechGain
                        MaskerSpectralLevels(i) = Me.SpeechMaterialComponent.GetAncestorAtLevel(SpeechMaterialComponent.LinguisticLevels.List).GetNumericMediaSetVariableValue(MediaSet, SLmName) + CurrentMaskerGain
                        For s = 0 To Siblings.Count - 1
                            IncorrectResponsesSpectralLevels(s)(i) = Siblings(s).GetNumericMediaSetVariableValue(MediaSet, SLsName) + CurrentSpeechGain
                        Next
                    Next

                    'Calculating SDRs
                    Dim SDRt = PDL.CalculateSDR(CorrectResponseSpectralLevels, MaskerSpectralLevels, Thresholds, Gain, True, True)
                    Dim SDRcs As New List(Of Double())
                    For s = 0 To Siblings.Count - 1
                        Dim SDRc = PDL.CalculateSDR(IncorrectResponsesSpectralLevels(s), MaskerSpectralLevels, Thresholds, Gain, True, True)
                        SDRcs.Add(SDRc)
                    Next

                    'Calculating PDL
                    _PhonemeDiscriminabilityLevel = PDL.CalculatePDL(SDRt, SDRcs)

                    UpdatePdlOnNextCall = False
                End If
                Return _PhonemeDiscriminabilityLevel
            End Get
        End Property

        ''' <summary>
        ''' Returns the response type of a test trial, including Missing responses and NotPresented when the trial has not yet been presented in a SipTestMeasurement.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetResponseType() As ResponseType

            If ObservedResponse Is Nothing Then Return ResponseType.NotPresented

            If ObservedResponse.Correct = True Then Return ResponseType.Correct

            If ObservedResponse.IsMissingResponse = True Then
                Return ResponseType.Missing
            Else
                Return ResponseType.Incorrect
            End If

        End Function

        ''' <summary>
        ''' Returns the result-response type of a test trial, including NotPresented, but counting missing responses as randomized as either correct or incorrect results.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetResultResponseType() As ResultResponseType

            If ObservedResponse Is Nothing Then Return ResultResponseType.NotPresented

            If ObservedResponse.Correct = True Then
                Return ResultResponseType.Correct
            Else
                Return ResultResponseType.Incorrect
            End If

        End Function


    End Class

    'Public Enum TestTrialResults
    '    Correct
    '    Incorrect
    '    Missing
    'End Enum

    Public Enum ResponseType
        NotPresented
        Correct
        Incorrect
        Missing
    End Enum

    Public Enum ResultResponseType
        NotPresented
        Correct
        Incorrect
    End Enum

    Public Enum SipTestPresets
        Måttlig_A
        Måttlig_B_Fallande
        Grav_A
        Grav_B_Fallande
    End Enum


    Public Enum AdaptiveTypes
        SimpleUpDown
        Fixed
    End Enum

    Public Class TestProcedure

        Public Property AdaptiveType As AdaptiveTypes

        Public Property LengthReduplications As Integer? = 1

        Public Property RandomizeOrder As Boolean = True

        Public Sub New(ByVal AdaptiveType As AdaptiveTypes)
            Me.AdaptiveType = AdaptiveType
        End Sub

    End Class


    ''' <summary>
    ''' A class that holds descriptions of each completed Sip-test measurement used in the Gui.
    ''' </summary>
    Public Class TestHistoryListData
            Public Property CurrentTestSessionData As New List(Of SipTestMeasurementGuiDescription)
            Public Property PreviousTestSessionData As New List(Of SipTestMeasurementGuiDescription)

            Public Class SipTestMeasurementGuiDescription
                Public ReadOnly Property GuiDescription As String
                Public ReadOnly Property TestLength As String
                Public ReadOnly Property PercentCorrect As String
                Public Sub New(ByVal GuiDescription As String, ByVal TestLength As String, ByVal PercentCorrect As String)
                    Me.GuiDescription = GuiDescription
                    Me.TestLength = TestLength
                    Me.PercentCorrect = PercentCorrect
                End Sub
            End Class

        End Class


End Namespace