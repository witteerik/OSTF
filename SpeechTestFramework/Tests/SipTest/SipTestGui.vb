Imports SpeechTestFramework.SipTest
Imports SpeechTestFramework.WinFormControls
Imports System.Windows.Forms
Imports System.Drawing

Public Class SipTestGui
    Public ReadOnly Property IsStandAlone As Boolean

    ''' <summary>
    ''' Holds the type of layout / functionality. R=Reserach, C=Clinical
    ''' </summary>
    Private ReadOnly UserType As Utils.UserTypes
    Private GuiLanguage As Utils.Languages
    Private SelectedTransducer As AudioSystemSpecification = Nothing
    Private SpeechMaterial As SpeechMaterialComponent

    ''' <summary>
    ''' Holds the name of the speech material to be loaded.
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property SpeechMaterialName As String

    Private WithEvents Audiogram As Audiogram
    Private WithEvents GainDiagram As GainDiagram
    Private ExpectedScoreDiagram As PsychometricFunctionDiagram
    Private AvailableAudiograms As New List(Of AudiogramData)
    Private ReadOnly AvailableReferenceLevels As New List(Of Double) From {68 - 10, 68 - 5, 68, 68 + 5, 68 + 10}
    Private AvailableHaGains As New List(Of HearingAidGainData)
    Private AvailablePresetsNames As List(Of String)
    Private CustomPresetCount As Integer = 1
    Private AvailableMediaSets As MediaSetLibrary
    Private AvailableLengthReduplications As New List(Of Integer) From {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 30, 60}
    Private AvailablePNRs As New List(Of Double) From {-15, -12, -10, -8, -6, -4, -2, 0, 2, 4, 6, 8, 10, 12, 15}
    ''' <summary>
    ''' Holds the (zero-based) index of the default reference level in the AvailableReferenceLevels object
    ''' </summary>
    Private ReadOnly DefaultReferenceLevelIndex As Integer = 2

    Private CurrentParticipantID As String = ""
    Private _SelectedAudiogramData As AudiogramData = Nothing
    Private Property SelectedAudiogramData As AudiogramData
        Get
            Return _SelectedAudiogramData
        End Get
        Set(value As AudiogramData)

            _SelectedAudiogramData = value

            'Enables/disables the audiogram depending on whether _SelectedAudiogramData has a value
            If _SelectedAudiogramData Is Nothing Then
                Audiogram.Enabled = False
                Audiogram.AudiogramData = Nothing
            Else
                Audiogram.AudiogramData = _SelectedAudiogramData
                Audiogram.Enabled = True
            End If

        End Set
    End Property

    Private SelectedReferenceLevel As Double?
    Private SelectedHearingAidGain As HearingAidGainData = Nothing
    Private SelectedPresetName As String = ""
    Private SelectedMediaSet As MediaSet = Nothing
    Private SelectedLengthReduplications As Integer?
    Private SelectedPnr As Double?
    Private SelectedTestDescription As String = ""

    Private SipMeasurementRandomizer As New Random

    Private MeasurementHistory As New MeasurementHistory
    Private TestComparisonHistory As New List(Of String)
    Private CurrentSipTestMeasurement As SipMeasurement
    Private CurrentTestSound As Audio.Sound = Nothing
    Private CurrentSipTrial As SipTest.SipTrial = Nothing


    Private WithEvents PcParticipantForm As PcTesteeForm
    Private WithEvents ParticipantControl As ITesteeControl

    Private Enum ScreenType
        Pc
        Bluetooth
    End Enum

    Private CurrentScreenType As ScreenType
    Private PcResponseMode As Utils.ResponseModes

    Private ReadOnly Bt_UUID As String = "056435e9-cfdd-4fb3-8cc8-9a4eb21c439c" 'Created with https://www.uuidgenerator.net/
    Private ReadOnly Bt_PIN As String = "1234"
    Private MyBtTesteeControl As BtTesteeControl = Nothing

    Private SelectedTestparadigm As Testparadigm = Testparadigm.Slow

    'Test settings /note that these default values are or may be overwritten when changing SelectedTestparadigm through the GUI
    ''' <summary>
    ''' The (shortest) time delay (in seconds) between the response and the start of a new trial.
    ''' </summary>
    Private InterTrialInterval As Double = 1

    ''' <summary>
    ''' The time delay (in seconds) between the end of the test word and the visual presenation of the response alternatives.
    ''' </summary>
    Private ResponseAlternativeDelay As Double = 0.5
    Private PretestSoundDuration As Double = 5
    Private MinimumStimulusOnsetTime As Double ' Used to be called MinimumTestWordStartTime and influence the test word onset time. However as the test word is always shorter that the masker, it now instead influences the masker onset time
    Private MaximumStimulusOnsetTime As Double ' Used to be called MaximumTestWordStartTime and influence the test word onset time. However as the test word is always shorter that the masker, it now instead influences the masker onset time
    Private TrialSoundMaxDuration As Double = 10 ' TODO: Optimize by shortening this time
    Private UseVisualQue As Boolean = True
    Private UseBackgroundSpeech As Boolean = False
    Private MaximumResponseTime As Double = 4
    Private ShowProgressIndication As Boolean = True

    ''' <summary>
    ''' Set to True to simulate test results.
    ''' </summary>
    Private SimulationMode As Boolean = False

    'Variables used during active testing
    Private TestIsStarted As Boolean = False
    Private TestIsPaused As Boolean = False
    ''' <summary>
    ''' Holds the time of the presentation of the response alternatives
    ''' </summary>
    Private ResponseAlternativesPresentationTime As DateTime
    Private TestWordAlternatives As List(Of Tuple(Of String, Audio.PortAudioVB.DuplexMixer.SoundSourceLocation))
    Private CorrectResponse As String = ""
    Private CurrentTrialSoundIsReady As Boolean = False
    Private CurrentTrialIsLaunched As Boolean = False ' A variable that holds a value indicating if the current trial was started by the StartTrialTimer, or if it should be started directly from prepare sound. (This construction is needed as the sound may not always be created before the next trial should start. If that happens the trial starts as soon as the sound is ready to be played.)
    Private StartTrialTimerHasTicked As Boolean = False
    Private TrialLaunchSpinLock As New Threading.SpinLock

    'Timers
    Private WithEvents StartTrialTimer As New Timers.Timer
    Private WithEvents ShowVisualQueTimer As New Timers.Timer
    Private WithEvents HideVisualQueTimer As New Timers.Timer
    Private WithEvents ShowResponseAlternativesTimer As New Timers.Timer
    Private WithEvents MaxResponseTimeTimer As New Timers.Timer

    'Delegate subs
    Delegate Sub NoArgDelegate()
    Delegate Sub StringArgReturningVoidDelegate([String] As String)


    Public Sub New()
        MyClass.New("Swedish SiP-test", Utils.Constants.UserTypes.Research, Utils.Constants.Languages.English, True)
    End Sub

    ''' <summary>
    ''' Creates a new instance of SiPTestGui 
    ''' </summary>
    ''' <param name="SpeechMaterialName"></param>
    ''' <param name="UserType"></param>
    ''' <param name="GuiLanguage"></param>
    ''' <param name="IsStandAlone">Set to true if called from within another OSTF application, and False if run as a standalone OSTF application. </param>
    Public Sub New(ByVal SpeechMaterialName As String, ByVal UserType As Utils.UserTypes, ByVal GuiLanguage As Utils.Languages, ByVal IsStandAlone As Boolean)

        ' This call is required by the designer.
        InitializeComponent()

        Me.IsStandAlone = IsStandAlone

        ' Add any initialization after the InitializeComponent() call.
        Me.SpeechMaterialName = SpeechMaterialName
        Me.UserType = UserType
        Me.GuiLanguage = GuiLanguage

        Dim UserTypeString As String = ""
        Select Case UserType
            Case Utils.Constants.UserTypes.Research
                Select Case GuiLanguage
                    Case Utils.Constants.Languages.Swedish
                        UserTypeString = "Forskningsversion"
                    Case Else
                        UserTypeString = "Research version"
                End Select
            Case Else
                UserTypeString = ""
        End Select

        Me.Text = SpeechMaterialName & " - " & UserTypeString

    End Sub


    Private Sub SipTestGui_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Initializing all components
        OstfBase.LoadAvailableTestSpecifications()

        Dim SelectedTest As SpeechMaterialSpecification = Nothing
        For Each ts In OstfBase.AvailableTests
            If ts.Name = SpeechMaterialName Then
                SelectedTest = ts
                Exit For
            End If
        Next

        If SelectedTest IsNot Nothing Then
            SpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(SelectedTest.GetSpeechMaterialFilePath, SelectedTest.GetTestRootPath)
            SpeechMaterial.ParentTestSpecification = SelectedTest
            SelectedTest.SpeechMaterial = SpeechMaterial
        Else
            MsgBox("Unable to locate or load the speech material '" & SpeechMaterialName & "'. Exiting the program!", MsgBoxStyle.Exclamation, "Speech material not found!")
            Exit Sub
        End If

        'Loading media sets
        SpeechMaterial.ParentTestSpecification.LoadAvailableMediaSetSpecifications()
        AvailableMediaSets = SpeechMaterial.ParentTestSpecification.MediaSets

        'Hiding things that should not be visible from the start
        StatAnalysisLabel.Visible = False

        'Createing diagrams and adding their references to the corresponding private fields
        AudiogramPanel.Controls.Add(New Audiogram)
        Audiogram = AudiogramPanel.Controls(AudiogramPanel.Controls.Count - 1)
        Audiogram.RightClickMode = Audiogram.RightClickActions.ShowAudiogramDialog
        Audiogram.Enabled = False

        GainPanel.Controls.Add(New GainDiagram)
        GainDiagram = GainPanel.Controls(GainPanel.Controls.Count - 1)

        ExpectedScorePanel.Controls.Add(New PsychometricFunctionDiagram)
        ExpectedScoreDiagram = ExpectedScorePanel.Controls(ExpectedScorePanel.Controls.Count - 1)

        'Docks the diagrams in their parent controls
        Audiogram.Dock = DockStyle.Fill
        GainDiagram.Dock = DockStyle.Fill
        ExpectedScoreDiagram.Dock = DockStyle.Fill

        'Setting diagram border style
        Audiogram.BorderStyle = BorderStyle.FixedSingle
        GainDiagram.BorderStyle = BorderStyle.FixedSingle
        ExpectedScoreDiagram.BorderStyle = BorderStyle.FixedSingle

        'Adding available reference levels
        ReferenceLevelComboBox.Items.Clear()
        For Each RefLevel In AvailableReferenceLevels
            ReferenceLevelComboBox.Items.Add(RefLevel)
        Next
        ReferenceLevelComboBox.SelectedIndex = DefaultReferenceLevelIndex

        'Adding a default no-gain hearing aid gain
        Dim NoGainData = HearingAidGainData.CreateNewNoGainData
        NoGainData.Name = "No gain"
        AvailableHaGains.Add(NoGainData)
        For Each HaGain In AvailableHaGains
            HaGainComboBox.Items.Add(HaGain)
        Next
        HaGainComboBox.SelectedIndex = HaGainComboBox.Items.Count - 1

        'Adding available preset names
        AvailablePresetsNames = SpeechMaterial.Presets.Keys.ToList
        PresetComboBox.Items.Clear()
        For Each Preset In AvailablePresetsNames
            PresetComboBox.Items.Add(Preset)
        Next
        'We don't select a default here...?? 

        'Adding available test situations
        TestSituationComboBox.Items.Clear()
        For Each Value In AvailableMediaSets
            TestSituationComboBox.Items.Add(Value)
        Next
        'We don't select a default here...?? 

        'Adding available test length reduplications
        TestLengthComboBox.Items.Clear()
        For Each TestLength In AvailableLengthReduplications
            TestLengthComboBox.Items.Add(TestLength)
        Next
        'We don't select a default here...?? 

        'Adding possible PNR values
        PnrComboBox.Items.Clear()
        For Each Pnr In AvailablePNRs
            PnrComboBox.Items.Add(Pnr)
        Next
        'We don't yet select a default here...?? It could possibly be done automatically at a later stage...

        SetLanguageStrings(GuiLanguage)

        StartSoundPlayer()

        'Adding values in Testparadigm_ComboBox
        Testparadigm_ComboBox.Items.Add(Testparadigm.Quick)
        Testparadigm_ComboBox.Items.Add(Testparadigm.Slow)
        Testparadigm_ComboBox.Items.Add(Testparadigm.Directional3)
        Testparadigm_ComboBox.Items.Add(Testparadigm.Directional5)
        Testparadigm_ComboBox.SelectedIndex = 0

    End Sub

    Private Sub SetLanguageStrings(ByVal Language As Utils.Languages)

        Select Case Language
            Case Utils.Languages.Swedish

                AboutToolStripMenuItem.Text = "Om"
                ParticipantID_Label.Text = "P.Id."
                ParticipantLock_Button.Text = "Lås"
                PcScreen_RadioButton.Text = "PC-skärm"
                PcTouch_CheckBox.Text = "Touch"
                BtScreen_RadioButton.Text = "BT-skärm"
                ConnectBluetoothScreen_Button.Text = "Anslut BT-skärm"
                DisconnectBtScreen_Button.Text = "Koppla från BT-skärm"
                SelectTransducer_Label.Text = "Välj ljudgivare"
                SelectAudiogram_Label.Text = "Välj audiogram"
                TestDescription_Label.Text = "Testbeskrivning:"
                RandomSeed_Label.Text = "Slumptalsfrö (valfritt):"
                CompletedTests_Label.Text = "Genomförda test"
                Audiogram_VerticalLabel.Text = "AUDIOGRAM (dB HL)"
                NewAudiogram_Button.Text = "Skapa nytt"
                AddTypicalAudiograms_Button.Text = "Skapa typiska"
                ReferenceLevel_Label.Text = "Referensnivå (dB)"
                Gain_Label.Text = "Förstärkning"
                Gain_VerticalLabel.Text = "FÖRSTÄRKNING (dB)"
                CreateNewGain_Button.Text = "Skapa nytt"
                AddFig6Gain_Button.Text = "Fig6"
                Preset_Label.Text = "Test"
                Situation_Label.Text = "Situation"
                LengthReduplications_Label.Text = "Repetitioner"
                Testparadigm_Label.Text = "Testläge"
                PsychmetricFunction_VerticalLabel.Text = "FÖRV. RESULTAT (%)"
                PNR_Label.Text = "PNR (dB)"
                CorrectCount_Label.Text = "Antal rätt"
                ProportionCorrect_Label.Text = "Andel rätt"
                StatAnalysisLabel.Text = "Statistisk analys"
                ExportData_Button.Text = "Exportera resultat"
                ImportData_Button.Text = "Importera resultat"
                MostDifficultItems_Button.Text = "Anpassat"
                TestLength_Label.Text = "Testlängd"

            Case Else

                'English is default
                AboutToolStripMenuItem.Text = "About"
                ParticipantID_Label.Text = "P.Id."
                ParticipantLock_Button.Text = "Lock"
                PcScreen_RadioButton.Text = "PC screen"
                PcTouch_CheckBox.Text = "Touch"
                BtScreen_RadioButton.Text = "BT screen"
                ConnectBluetoothScreen_Button.Text = "Connect to BT screen"
                DisconnectBtScreen_Button.Text = "Disconnect BT screen"
                SelectTransducer_Label.Text = "Select sound transducer"
                SelectAudiogram_Label.Text = "Select audiogram"
                TestDescription_Label.Text = "Test description:"
                RandomSeed_Label.Text = "Random seed (optional):"
                CompletedTests_Label.Text = "Completed tests"
                Audiogram_VerticalLabel.Text = "AUDIOGRAM (dB HL)"
                NewAudiogram_Button.Text = "Create new"
                AddTypicalAudiograms_Button.Text = "Add typical"
                ReferenceLevel_Label.Text = "Reference level (dB)"
                Gain_Label.Text = "Gain"
                Gain_VerticalLabel.Text = "GAIN (dB)"
                CreateNewGain_Button.Text = "Create new"
                AddFig6Gain_Button.Text = "Fig6"
                Preset_Label.Text = "Test"
                Situation_Label.Text = "Situation"
                LengthReduplications_Label.Text = "Repetitions"
                Testparadigm_Label.Text = "Test mode"
                PsychmetricFunction_VerticalLabel.Text = "EST. SCORE (%)"
                PNR_Label.Text = "PNR (dB)"
                CorrectCount_Label.Text = "Number correct"
                ProportionCorrect_Label.Text = "Percent correct"
                StatAnalysisLabel.Text = "Statistical analysis"
                ExportData_Button.Text = "Export data"
                ImportData_Button.Text = "Import data"
                MostDifficultItems_Button.Text = "Custom"
                TestLength_Label.Text = "Test length"

        End Select


    End Sub

    Private Sub StartSoundPlayer()

        'Selects the wave format for use (doing it this way means that the wave format MUST be the same in all available MediaSets)
        OstfBase.SoundPlayer.ChangePlayerSettings(, SpeechMaterial.GetWavefileFormat(AvailableMediaSets(0)),,, Audio.PortAudioVB.OverlappingSoundPlayer.SoundDirections.PlaybackOnly, False, False)

        Dim LocalAvailableTransducers = OstfBase.AvaliableTransducers
        If LocalAvailableTransducers.Count = 0 Then
            MsgBox("Unable to start the application since no sound transducers could be found!", MsgBoxStyle.Critical, "SiP-test")
        End If

        'Adding transducers to the combobox, and selects the first one
        For Each Transducer In LocalAvailableTransducers
            Transducer_ComboBox.Items.Add(Transducer)
        Next
        'Transducer_ComboBox.SelectedIndex = 0

    End Sub

    Private Sub Transducer_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Transducer_ComboBox.SelectedIndexChanged

        SelectedTransducer = Transducer_ComboBox.SelectedItem

        If SelectedTransducer.CanPlay = True Then
            '(At this stage the sound player will be started, if not already done.)
            OstfBase.SoundPlayer.ChangePlayerSettings(SelectedTransducer.ParentAudioApiSettings, , 0.4, SelectedTransducer.Mixer,, True, True)
        Else
            MsgBox("Unable to start the player using the selected transducer (probably the selected output device doesn't have enough output channels?)!", MsgBoxStyle.Exclamation, "Sound player failure")
        End If

        If CurrentParticipantID <> "" Then
            Test_TableLayoutPanel.Enabled = True
        End If

    End Sub

    Private Sub KeyDetection(sender As Object, e As KeyEventArgs) Handles Me.KeyUp, PcParticipantForm.KeyUp

        'Use this method to tigger actions by pressing a keyboard key during active testing, when PcScreen is used, and mouse is therefore used by the testee.
        'MsgBox("Key pressed: " & e.KeyData)

    End Sub


    Private Sub LockParticipant(sender As Object, e As EventArgs) Handles ParticipantLock_Button.Click

        'Then look up a patient in a file or database, create a patient from it and reference that patient into the CurrentPatient property

        Dim ParticipantID As String = ParticipantIdTextBox.Text

        'Checking SocialSecurityNumber
        If ParticipantID = "" Then
            ShowMessageBox("Please enter a participant ID", "Invalid participant ID")
            Exit Sub
        End If

        'Locking ID controls in the Gui
        LockParticipanDetails(ParticipantID)

        'Creating a new patient
        CurrentParticipantID = ParticipantID

        Screen_TableLayoutPanel.Enabled = True
        SoundSettings_TableLayoutPanel.Enabled = True

        Transducer_ComboBox.Focus()

    End Sub

    Private Sub LockParticipanDetails(ID As String)

        ParticipantIdTextBox.ReadOnly = True
        ParticipantIdTextBox.Text = ID
        ParticipantLock_Button.Enabled = False

    End Sub


    Private Sub CreateNewAudiogram(sender As Object, e As EventArgs) Handles NewAudiogram_Button.Click

        'Stores the selected audiogram data
        Dim NewAudiogram = New AudiogramData
        NewAudiogram.Name = CurrentParticipantID & "_" & DateTime.Now
        AvailableAudiograms.Add(NewAudiogram)

        UpdateAudiogramList()

    End Sub

    Private Sub AddTypicalAudiograms_Button_Click(sender As Object, e As EventArgs) Handles AddTypicalAudiograms_Button.Click

        Dim AudiogramsToAdd As New List(Of AudiogramData.BisgaardAudiograms) From {
                AudiogramData.BisgaardAudiograms.NH,
                AudiogramData.BisgaardAudiograms.N1,
                AudiogramData.BisgaardAudiograms.N2,
                AudiogramData.BisgaardAudiograms.N3,
                AudiogramData.BisgaardAudiograms.N4,
                AudiogramData.BisgaardAudiograms.N5,
                AudiogramData.BisgaardAudiograms.N6,
                AudiogramData.BisgaardAudiograms.N7,
                AudiogramData.BisgaardAudiograms.S1,
                AudiogramData.BisgaardAudiograms.S2,
                AudiogramData.BisgaardAudiograms.S3}

        For Each AudiogramToAdd In AudiogramsToAdd
            Dim NewAudiogram As New AudiogramData() 'CurrentPatient.GetCurrentSession())
            NewAudiogram.CreateTypicalAudiogramData(AudiogramToAdd)
            AvailableAudiograms.Add(NewAudiogram)
        Next

        UpdateAudiogramList()

        'Preventing the user from adding these again by disabling the button
        AddTypicalAudiograms_Button.Enabled = False

    End Sub

    ''' <summary>
    ''' Updates the audiogram list and selects the audiogram last added 
    ''' </summary>
    Private Sub UpdateAudiogramList()

        AudiogramComboBox.Items.Clear()
        AudiogramComboBox.Items.AddRange(AvailableAudiograms.ToArray)
        If AudiogramComboBox.Items.Count > 0 Then
            AudiogramComboBox.SelectedIndex = AudiogramComboBox.Items.Count - 1
        End If

    End Sub

    Private Sub SelectAudiogram(sender As Object, e As EventArgs) Handles AudiogramComboBox.SelectedIndexChanged

        'Stores the selected audiogram data
        SelectedAudiogramData = AudiogramComboBox.SelectedItem

        'Checks if the audiogram contains data, and stops if not
        If SelectedAudiogramData.ContainsAcData = False Then Exit Sub
        If SelectedAudiogramData.ContainsCbData = False Then SelectedAudiogramData.CalculateCriticalBandValues()

        TryCalculatePsychometricFunction()

    End Sub

    Private Sub AudiogramDataChanged() Handles Audiogram.DataChanged

        'Checks if the audiogram contains data, and stops if not
        If SelectedAudiogramData.ContainsAcData = False Then Exit Sub

        'Updating the critical band values
        If SelectedAudiogramData.CalculateCriticalBandValues() = False Then
            ShowMessageBox("Unable to calculate critical band values. Probably there are not enough data points in the selected audiogram.")
            Exit Sub
        End If

        'Triggers recalculation based on a change in the selected audiogram data
        TryCalculatePsychometricFunction()

    End Sub

    Private Sub SelectReferenceLevel(sender As Object, e As EventArgs) Handles ReferenceLevelComboBox.SelectedIndexChanged

        'Stores the selected reference level
        SelectedReferenceLevel = ReferenceLevelComboBox.SelectedItem

        TryCalculatePsychometricFunction()

    End Sub

    Private Sub CreateNewGain_Button_Click(sender As Object, e As EventArgs) Handles CreateNewGain_Button.Click

        Dim NewGain = HearingAidGainData.CreateNewNoGainData
        NewGain.Name = CurrentParticipantID & "_" & DateTime.Now
        AvailableHaGains.Add(NewGain)
        UpdateGainList()

    End Sub

    Private Sub AddFig6Gain_Button_Click(sender As Object, e As EventArgs) Handles AddFig6Gain_Button.Click

        If SelectedAudiogramData Is Nothing Or SelectedReferenceLevel Is Nothing Then
            MsgBox("Before you can create new Fig6 gain, you must select/supply audiogram data and reference level.", MsgBoxStyle.Information, "Please supply data!")
            Exit Sub
        End If

        Dim NewGain = HearingAidGainData.CreateNewFig6GainData(SelectedAudiogramData, SelectedReferenceLevel)
        If NewGain Is Nothing Then
            ShowMessageBox("Unable to create Fig6 gain. Probably there are not enough data points in the selected audiogram!")
            Exit Sub
        End If
        NewGain.Name = CurrentParticipantID & "_" & "Fig6" & "_R" & SelectedReferenceLevel & "_" & SelectedAudiogramData.Name
        AvailableHaGains.Add(NewGain)
        UpdateGainList()

    End Sub


    ''' <summary>
    ''' Updates the hearing aid gain list and selects the gain data last added 
    ''' </summary>
    Private Sub UpdateGainList()

        HaGainComboBox.Items.Clear()
        HaGainComboBox.Items.AddRange(AvailableHaGains.ToArray)
        If HaGainComboBox.Items.Count > 0 Then
            HaGainComboBox.SelectedIndex = HaGainComboBox.Items.Count - 1
        End If

    End Sub

    Private Sub SelectHearingAidGain(sender As Object, e As EventArgs) Handles HaGainComboBox.SelectedIndexChanged

        'Stores the selected hearing-aid gain type
        SelectedHearingAidGain = HaGainComboBox.SelectedItem

        'Displays it in the Audiogram
        GainDiagram.GainData = SelectedHearingAidGain

        TryCalculatePsychometricFunction()

    End Sub

    Private Sub GainDataChanged() Handles GainDiagram.DataChanged

        'Triggers recalculation based on a change in the selected audiogram data
        TryCalculatePsychometricFunction()

    End Sub

    Private Sub SelectSituation(sender As Object, e As EventArgs) Handles TestSituationComboBox.SelectedIndexChanged

        'Stores the selected preset
        SelectedMediaSet = TestSituationComboBox.SelectedItem

        TryCalculatePsychometricFunction()

    End Sub

    Private Sub SelectPreset(sender As Object, e As EventArgs) Handles PresetComboBox.SelectedIndexChanged

        'Stores the selected preset
        SelectedPresetName = PresetComboBox.SelectedItem

        TryCalculatePsychometricFunction()

    End Sub

    Private Sub MostDifficultItems_Button_Click() Handles MostDifficultItems_Button.Click
        SelectMostDifficultItems()
    End Sub

    Private Sub SelectMostDifficultItems(Optional ByVal GroupCount As Integer = 7)

        If SelectedPnr Is Nothing Then
            ShowMessageBox("Please select a PNR value!", "SiP-test")
            Exit Sub
        End If
        If CurrentParticipantID Is Nothing Then
            ShowMessageBox("Please enter a participant ID!", "SiP-test")
            Exit Sub
        End If
        If SelectedAudiogramData Is Nothing Then
            ShowMessageBox("Please select an audiogram!", "SiP-test")
            Exit Sub
        End If
        If SelectedReferenceLevel.HasValue = False Then
            ShowMessageBox("Please select a reference level!", "SiP-test")
            Exit Sub
        End If
        If SelectedHearingAidGain Is Nothing Then
            ShowMessageBox("Please select hearing aid gain, or no gain!", "SiP-test")
            Exit Sub
        End If
        If SelectedMediaSet Is Nothing Then
            ShowMessageBox("Please select a test situation!", "SiP-test")
            Exit Sub
        End If

        'Creates a new test and updates the psychometric function diagram
        Dim TempSipTestMeasurement = New SipMeasurement(CurrentParticipantID, SpeechMaterial.ParentTestSpecification)
        TempSipTestMeasurement.SelectedAudiogramData = SelectedAudiogramData
        TempSipTestMeasurement.HearingAidGain = SelectedHearingAidGain
        TempSipTestMeasurement.TestProcedure.LengthReduplications = 1

        'Test length was updated, adds test trials to the measurement
        Dim Lists = TempSipTestMeasurement.ParentTestSpecification.SpeechMaterial.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.List)

        Dim CandidateLists As New List(Of Tuple(Of SpeechMaterialComponent, Double)) 'Component, success probabilities
        Dim SuccessProbabilityList As New List(Of Double)

        For Each List In Lists

            Dim TempPreset As New List(Of SpeechMaterialComponent) From {List}

            TempSipTestMeasurement.PlanTestTrials(AvailableMediaSets, TempPreset, SelectedMediaSet.MediaSetName)
            TempSipTestMeasurement.SetLevels(SelectedReferenceLevel, SelectedPnr)
            Dim EstimatedScore = TempSipTestMeasurement.CalculateEstimatedMeanScore

            CandidateLists.Add(New Tuple(Of SpeechMaterialComponent, Double)(List, EstimatedScore.Item1))
            SuccessProbabilityList.Add(EstimatedScore.Item1)

        Next

        'Sorts the list with success probabilities
        SuccessProbabilityList.Sort()

        Dim LastIndex As Integer = Math.Min(SuccessProbabilityList.Count - 1, GroupCount - 1)
        Dim Limit As Double = SuccessProbabilityList(LastIndex)
        Dim SelectedComponents As New List(Of SpeechMaterialComponent)

        For Each Component In CandidateLists
            If Component.Item2 <= Limit Then SelectedComponents.Add(Component.Item1)
            If SelectedComponents.Count = GroupCount Then Exit For
        Next

        'Creating a preset
        Dim NewPresetName As String = "Custom " & CustomPresetCount '"Custom_" & SelectedAudiogramData.Name & "_" & SelectedReferenceLevel & "_" & SelectedPnr
        CustomPresetCount += 1

        SpeechMaterial.Presets.Add(NewPresetName, SelectedComponents)
        AvailablePresetsNames.Add(NewPresetName)
        PresetComboBox.Items.Add(NewPresetName)
        PresetComboBox.SelectedItem = NewPresetName

        Dim SelectedComponentNames As New List(Of String)
        For Each SelectedComponent In SelectedComponents
            SelectedComponentNames.Add(SelectedComponent.PrimaryStringRepresentation)
        Next

        MsgBox("A new preset was created containing the groups: " & vbCrLf & vbCrLf & String.Join(vbCrLf, SelectedComponentNames), MsgBoxStyle.Information, "New custom test preset created")

    End Sub

    Private Sub SelectTestLength(sender As Object, e As EventArgs) Handles TestLengthComboBox.SelectedIndexChanged

        'Stores the selected length
        SelectedLengthReduplications = TestLengthComboBox.SelectedItem

        TryCalculatePsychometricFunction()

    End Sub

    Private Sub TryCalculatePsychometricFunction()

        Try

            'Resetting the test description box
            TestDescriptionTextBox.Text = ""

            'Resetting the planned trial test length text
            PlannedTestLength_TextBox.Text = ""

            If CurrentParticipantID Is Nothing Then Exit Sub
            If SelectedAudiogramData Is Nothing Then Exit Sub
            If SelectedReferenceLevel.HasValue = False Then Exit Sub
            If SelectedHearingAidGain Is Nothing Then Exit Sub
            If SelectedPresetName = "" Then Exit Sub
            If SelectedMediaSet Is Nothing Then Exit Sub
            If SelectedLengthReduplications.HasValue = False Then Exit Sub


            'Creates a new test and updates the psychometric function diagram
            CurrentSipTestMeasurement = New SipMeasurement(CurrentParticipantID, SpeechMaterial.ParentTestSpecification)
            CurrentSipTestMeasurement.SelectedAudiogramData = SelectedAudiogramData
            CurrentSipTestMeasurement.HearingAidGain = SelectedHearingAidGain
            CurrentSipTestMeasurement.TestProcedure.LengthReduplications = SelectedLengthReduplications
            CurrentSipTestMeasurement.TestProcedure.TestParadigm = SelectedTestparadigm

            'Test length was updated, adds test trials to the measurement
            CurrentSipTestMeasurement.PlanTestTrials(AvailableMediaSets, SelectedPresetName, SelectedMediaSet.MediaSetName)

            'Calculates the psychometric function
            Dim PsychoMetricFunction = CurrentSipTestMeasurement.CalculateEstimatedPsychometricFunction(SelectedReferenceLevel)

            Dim PNRs(PsychoMetricFunction.Count - 1) As Single
            Dim PredictedScores(PsychoMetricFunction.Count - 1) As Single
            Dim LowerCriticalBoundary(PsychoMetricFunction.Count - 1) As Single
            Dim UpperCriticalBoundary(PsychoMetricFunction.Count - 1) As Single

            Dim n As Integer = 0
            For Each kvp In PsychoMetricFunction
                PNRs(n) = kvp.Key
                PredictedScores(n) = kvp.Value.Item1
                LowerCriticalBoundary(n) = kvp.Value.Item2
                UpperCriticalBoundary(n) = kvp.Value.Item3
                n += 1
            Next

            'Updates the psychometric function diagram
            DisplayPredictedPsychometricCurve(PNRs, PredictedScores, LowerCriticalBoundary, UpperCriticalBoundary)

            'Displayes the planned test length
            PlannedTestLength_TextBox.Text = CurrentSipTestMeasurement.PlannedTrials.Count + CurrentSipTestMeasurement.ObservedTrials.Count

            'TODO: Calling GetTargetAzimuths only to ensure that the Actual Azimuths needed for presentation in the TestTrialTable exist. This should probably be done in some other way... (Only applies to the Directional3 and Directional5 Testparadigms)
            Select Case SelectedTestparadigm
                Case Testparadigm.Directional3, Testparadigm.Directional5
                    CurrentSipTestMeasurement.GetTargetAzimuths()
            End Select

            UpdateTestTrialTable()
            UpdateTestProgress()

            'Initiates the test
            TestDescriptionTextBox.Focus()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

    Private Sub DisplayPredictedPsychometricCurve(PNRs() As Single, PredictedScores() As Single, LowerCiLimits() As Single, UpperCiLimits() As Single)

        ExpectedScoreDiagram.Lines.Clear()
        ExpectedScoreDiagram.Lines.Add(New PlotBase.Line With {.Color = Color.Black, .Dashed = False, .LineWidth = 3, .XValues = PNRs, .YValues = PredictedScores})

        ExpectedScoreDiagram.Areas.Clear()

        ExpectedScoreDiagram.Areas.Add(New PlotBase.Area With {.Color = Color.Pink, .XValues = PNRs, .YValuesLower = LowerCiLimits, .YValuesUpper = UpperCiLimits})

        ExpectedScoreDiagram.Invalidate()
        ExpectedScoreDiagram.Update()

    End Sub



    Private Sub SelectPNR(sender As Object, e As EventArgs) Handles PnrComboBox.SelectedIndexChanged

        'Stores the selected selected PNR
        SelectedPnr = PnrComboBox.SelectedItem

    End Sub

    Private Sub TestDescriptionTextBox_TextChanged(sender As Object, e As EventArgs) Handles TestDescriptionTextBox.TextChanged

        'Makes sure that the user enters a valid, not already taken, test description
        Dim DescriptionIsOk As Boolean = False

        Dim ExistingDescriptions As New List(Of String)
        For Each Measurement In MeasurementHistory.Measurements
            ExistingDescriptions.Add(Measurement.Description.Trim.ToLower)
        Next

        If ExistingDescriptions.Contains(TestDescriptionTextBox.Text.Trim.ToLower) Then
            TestDescriptionTextBox.ForeColor = Color.Red
            DescriptionIsOk = False
        Else
            TestDescriptionTextBox.ForeColor = Color.Black
            If TestDescriptionTextBox.Text.Trim = "" Then
                DescriptionIsOk = False
            Else
                DescriptionIsOk = True
                SelectedTestDescription = TestDescriptionTextBox.Text.Trim
            End If
        End If

        If DescriptionIsOk = True Then
            TryEnableTestStart()
        Else
            Start_AudioButton.Enabled = False
        End If

    End Sub


    Private Sub TryEnableTestStart()

        If SelectedTransducer.CanPlay = False Then
            'Aborts if the SelectedTransducer cannot be used to play sound
            MsgBox("Unable to play sound using the selected transducer!", MsgBoxStyle.Exclamation, "Sound player error")
            Exit Sub
        End If

        If CurrentSipTestMeasurement Is Nothing Then
            ShowMessageBox("Inget test är laddat.", "SiP-test")
            Exit Sub
        End If

        'Sets the measurement datetime
        CurrentSipTestMeasurement.MeasurementDateTime = DateTime.Now

        'Getting NeededTargetAzimuths for the Directional3 and Directional5 Testparadigms
        Dim NeededTargetAzimuths As List(Of Double) = Nothing
        Select Case SelectedTestparadigm
            Case Testparadigm.Directional3, Testparadigm.Directional5
                NeededTargetAzimuths = CurrentSipTestMeasurement.GetTargetAzimuths()
        End Select

        'Creating a ParticipantForm
        Select Case CurrentScreenType
            Case ScreenType.Pc

                'Creating a new participant form (and ParticipantControl) if none exist
                If PcParticipantForm Is Nothing Then
                    Select Case SelectedTestparadigm
                        Case Testparadigm.Quick, Testparadigm.Slow
                            PcParticipantForm = New PcTesteeForm(PcTesteeForm.TaskType.ForcedChoice)

                        Case Testparadigm.Directional3, Testparadigm.Directional5
                            PcParticipantForm = New PcTesteeForm(PcTesteeForm.TaskType.ForcedChoiceDirection, NeededTargetAzimuths)
                    End Select
                End If


                Select Case SelectedTestparadigm
                    Case Testparadigm.Quick, Testparadigm.Slow
                        If PcParticipantForm.CurrentTaskType <> PcTesteeForm.TaskType.ForcedChoice Then
                            PcParticipantForm.UpdateType(PcTesteeForm.TaskType.ForcedChoice)
                        End If

                    Case Testparadigm.Directional3, Testparadigm.Directional5

                        If PcParticipantForm.CurrentTaskType <> PcTesteeForm.TaskType.ForcedChoiceDirection Then
                            PcParticipantForm.UpdateType(PcTesteeForm.TaskType.ForcedChoiceDirection, NeededTargetAzimuths)

                        ElseIf PcParticipantForm.CurrentTaskType = PcTesteeForm.TaskType.ForcedChoiceDirection Then

                            If String.Join(" ", PcParticipantForm.CurrentTargetDirections) <> String.Join(" ", NeededTargetAzimuths) Then
                                PcParticipantForm.UpdateType(PcTesteeForm.TaskType.ForcedChoiceDirection, NeededTargetAzimuths)
                            Else
                                'No need for update, alreday the right azimuths
                            End If

                        Else
                            'No need for update
                        End If

                    Case Else
                        'This will be an other type, not yet implemented
                        Throw New NotImplementedException

                End Select

                ParticipantControl = PcParticipantForm.ParticipantControl

                'Shows the ParticipantForm
                PcParticipantForm.Show()

            Case ScreenType.Bluetooth

                Select Case SelectedTestparadigm
                    Case Testparadigm.Directional3, Testparadigm.Directional5
                        ShowMessageBox("Bluetooth screen is not yet implemented for the Directional3 and Directional5 test paradigms. Use the PC screen instead.", "SiP-test")
                        Exit Sub
                End Select

                ParticipantControl = MyBtTesteeControl
                MyBtTesteeControl.StartNewTestSession()

        End Select

        Start_AudioButton.Enabled = True

    End Sub

    Private Sub TryStartTest(sender As Object, e As EventArgs) Handles Start_AudioButton.Click, ParticipantControl.StartedByTestee

        If TestIsStarted = False Then


            If SelectedPnr Is Nothing Then
                ShowMessageBox("Please select a PNR value!", "SiP-test")
                Exit Sub
            End If
            If SelectedTestDescription = "" Then
                ShowMessageBox("Please provide a test description (such as 'test 1, with HA')!", "SiP-test")
                Exit Sub
            End If

            'Creates a new randomizer before each test start
            Dim Seed As Integer? = RandomSeed_IntegerParsingTextBox.Value
            If Seed.HasValue Then
                SipMeasurementRandomizer = New Random(Seed)
            Else
                SipMeasurementRandomizer = New Random
            End If


            'Applying the SelectedReferenceLevel, SelectedPnr and the SelectedTestDescription
            CurrentSipTestMeasurement.SetLevels(SelectedReferenceLevel, SelectedPnr)

            CurrentSipTestMeasurement.Description = SelectedTestDescription

            'Things seemed to be in order,
            'Starting the test

            TogglePlayButton(False)
            Stop_AudioButton.Enabled = True

            LockSettingsPanels()

            If CurrentScreenType = ScreenType.Pc Then
                'Locks the cursor to the form
                If PcParticipantForm IsNot Nothing Then
                    PcParticipantForm.LockCursorToForm()
                    PcParticipantForm.SetResponseMode(PcResponseMode)
                End If
            End If

            If sender Is ParticipantControl Then
                Utils.SendInfoToLog("Test started by administrator")
            ElseIf sender Is ParticipantControl Then
                Utils.SendInfoToLog("Test started by testee")
            End If

            TestIsStarted = True

            If SimulationMode = False Then
                InitiateTestByPlayingSound()
            Else
                'Calling StartTimerTick directly
                StartTrialTimerTick()
            End If

        Else
            'Test is started
            If TestIsPaused = True Then
                ResumeTesting()
            Else
                PauseTesting()
            End If
        End If

    End Sub

    Private Sub StopButton_Click() Handles Stop_AudioButton.Click
        If TestIsStarted = True Then
            FinalizeTesting()
        End If
    End Sub

    ''' <summary>
    ''' Toggles the start button.
    ''' </summary>
    ''' <param name="ShowPlay">Set to True to show a play symbol, or False to show a pausue symbol.</param>
    Private Sub TogglePlayButton(ByVal ShowPlay As Boolean)
        If ShowPlay = True Then
            Start_AudioButton.ViewMode = AudioButton.ViewModes.Play
        Else
            Start_AudioButton.ViewMode = AudioButton.ViewModes.Pause
        End If
    End Sub


    Private Sub LockSettingsPanels()
        PcScreen_RadioButton.Enabled = False
        BtScreen_RadioButton.Enabled = False
        TestSettings_TableLayoutPanel.Enabled = False
        TestDescriptionTextBox.ReadOnly = True
        SoundSettings_TableLayoutPanel.Enabled = False
    End Sub

    Private Sub UnlockSettingsPanels()
        PcScreen_RadioButton.Enabled = True
        BtScreen_RadioButton.Enabled = True
        TestSettings_TableLayoutPanel.Enabled = True
        TestDescriptionTextBox.ReadOnly = False
        SoundSettings_TableLayoutPanel.Enabled = True
    End Sub

    Private Sub UnlockCursor()
        'Puts the mouse on the test form
        Cursor.Position = New Point(Me.ClientRectangle.Left + Me.ClientRectangle.Width / 2, Me.ClientRectangle.Height / 2)

        'Limits mouse movements to the form boundaries
        Cursor.Clip = Nothing 'Me.Bounds

    End Sub

    Private Sub InitiateTestByPlayingSound()

        'Premixing the first 10 sounds 
        CurrentSipTestMeasurement.PreMixTestTrialSoundsOnNewTread(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, 10)

        StartTrialTimer.Interval = Math.Max(1, InterTrialInterval * 1000)

        'Removes the start button
        ParticipantControl.ResetTestItemPanel()

        'Cretaing a context sound without any test stimulus, that runs for approx TestSetup.PretestSoundDuration seconds
        Dim TestSound As Audio.Sound = Nothing
        'TestSound = SoundLibrary.CreateSoundStimulus(Nothing, 0, 0,
        '                                             Nothing,
        '                                             Nothing,
        '                                             Nothing,
        '                                             ContextRegionForegroundLevel_SPL,
        '                                             ContextRegionBackgroundLevel_SPL,
        '                                             TestSetup.CurrentFixedMaskerSoundRandomization,
        '                                             False,
        '                                             TestSetup.SimulateHearingLoss,
        '                                             TestSetup.CompensateHearingLoss,
        '                                             TestSessionDescription.PatientDetails.ID & "_" & Me.TestSessionStage.ToString & "_" & Me.TestConditionName,
        '                                             "PreSound", Nothing, Nothing, TestSetup.PretestSoundDuration + MaximumResponseTime) 'Adding four seconds to PretestSoundDuration to allow for preparation of the first test trial 


        'Plays sound
        If SimulationMode = False Then SoundPlayer.SwapOutputSounds(TestSound)

        'Setting the interval to the first test stimulus using NewTrialTimer.Interval (N.B. The NewTrialTimer.Interval value has to be reset at the first tick, as the deafault value is overridden here)
        StartTrialTimer.Interval = Math.Max(1, PretestSoundDuration * 1000)


        'Preparing and launching the next trial
        PrepareAndLaunchTrial_ThreadSafe()

    End Sub


    Private Sub PrepareAndLaunchTrial_ThreadSafe()

        If Me.InvokeRequired = True Then
            Dim d As New NoArgDelegate(AddressOf PrepareAndLaunchTrial_Unsafe)
            Me.Invoke(d)
        Else
            PrepareAndLaunchTrial_Unsafe()
        End If

    End Sub

    Private Sub PrepareAndLaunchTrial_Unsafe()

        'Resetting NextTrialIsReady and CurrentTrialIsStarted  
        CurrentTrialSoundIsReady = False
        CurrentTrialIsLaunched = False
        StartTrialTimerHasTicked = False

        'Pausing test
        If TestIsPaused = True Then
            Exit Sub
        Else
            TogglePlayButton(False)
        End If

        'Updates the GUI table
        UpdateTestTrialTable()
        UpdateTestProgress()

        'Gets the next stimulus
        CurrentSipTrial = CurrentSipTestMeasurement.GetNextTrial()

        'Checks if test is finished
        If CurrentSipTrial Is Nothing Then
            FinalizeTesting()
            Exit Sub
        End If

        'Starting the timer that will initiate the presentation of the trial, if the sound is is prepared in time.
        StartTrialTimer.Start()

        If CurrentSipTrial IsNot Nothing Then

            'Preparing words to be presented on the response screen, and defines the correct response
            PrepareResponseScreenData()

            'Praparing the sound
            PrepareTestTrialSound()

            'Setting NextTrialIsReady to True to mark that the trial is ready to run
            CurrentTrialSoundIsReady = True

        Else
            'Testing Is completed
            FinalizeTesting()
        End If

    End Sub


    Private Sub StartTrialTimerTick() Handles StartTrialTimer.Elapsed
        StartTrialTimer.Stop()

        'Restoring the value of StartTrialTimer.Interval, as this was temporarily modified to use with the session initial sound
        StartTrialTimer.Interval = Math.Max(1, InterTrialInterval * 1000)

        'Notes that the start time has passed
        StartTrialTimerHasTicked = True

        'Lauches the presentation of the trial, if the sound preparation is finished
        If CurrentTrialSoundIsReady = True Then

            'Launching the trial
            LaunchTrial(CurrentTestSound)
        End If

    End Sub

    Private Sub PrepareResponseScreenData()

        'Creates a response string
        Select Case SelectedTestparadigm
            Case Testparadigm.Quick, Testparadigm.Slow
                CorrectResponse = CurrentSipTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling")
            Case Testparadigm.Directional3, Testparadigm.Directional5
                CorrectResponse = CurrentSipTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling") & vbTab & CurrentSipTrial.TargetStimulusLocation.ActualLocation.HorizontalAzimuth
        End Select

        'Collects the response alternatives
        TestWordAlternatives = New List(Of Tuple(Of String, Audio.PortAudioVB.DuplexMixer.SoundSourceLocation))
        Dim TempList As New List(Of SpeechMaterialComponent)
        CurrentSipTrial.SpeechMaterialComponent.IsContrastingComponent(,, TempList)
        For Each ContrastingComponent In TempList
            TestWordAlternatives.Add(New Tuple(Of String, Audio.PortAudioVB.DuplexMixer.SoundSourceLocation)(ContrastingComponent.GetCategoricalVariableValue("Spelling"), CurrentSipTrial.TargetStimulusLocation.ActualLocation))
        Next

        'Randomizing the order
        Dim AlternativesCount As Integer = TestWordAlternatives.Count
        Dim TempList2 As New List(Of Tuple(Of String, Audio.PortAudioVB.DuplexMixer.SoundSourceLocation))
        For n = 0 To AlternativesCount - 1
            Dim RandomIndex As Integer = SipMeasurementRandomizer.Next(0, TestWordAlternatives.Count)
            TempList2.Add(TestWordAlternatives(RandomIndex))
            TestWordAlternatives.RemoveAt(RandomIndex)
        Next
        TestWordAlternatives = TempList2

    End Sub

    Private Sub PrepareTestTrialSound()

        Try

            'Resetting CurrentTestSound
            CurrentTestSound = Nothing

            If CurrentSipTestMeasurement.TestProcedure.AdaptiveType <> SipTest.AdaptiveTypes.Fixed Then
                'Levels only need to be set here, and possibly not even here, in adaptive procedures. Its better if the level is set directly upon selection of the trial...
                'CurrentSipTrial.SetLevels()
            End If


            If SimulationMode = False Then 'We don't actually need to prepare the test sound in simulation mode

                If (CurrentSipTestMeasurement.ObservedTrials.Count + 3) Mod 10 = 0 Then
                    'Premixing the next 10 sounds, starting three trials before the next is needed 
                    CurrentSipTestMeasurement.PreMixTestTrialSoundsOnNewTread(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, 10)
                End If

                'Waiting for the background thread to finish mixing
                Dim WaitPeriods As Integer = 0
                While CurrentSipTrial.TestTrialSound Is Nothing
                    WaitPeriods += 1
                    Threading.Thread.Sleep(100)
                    Console.WriteLine("Waiting for sound to mix: " & WaitPeriods * 100 & " ms")
                End While

                'If CurrentSipTrial.TestTrialSound Is Nothing Then
                '    CurrentSipTrial.MixSound(SelectedTransducer, SelectedTestparadigm, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech)
                'End If

                'References the sound
                CurrentTestSound = CurrentSipTrial.TestTrialSound

                'Setting visual que intervals
                If UseVisualQue = True Then
                    ShowVisualQueTimer.Interval = Math.Max(1, CurrentSipTrial.TestWordStartTime * 1000)
                    HideVisualQueTimer.Interval = Math.Max(2, CurrentSipTrial.TestWordCompletedTime * 1000)
                    ShowResponseAlternativesTimer.Interval = HideVisualQueTimer.Interval + 1000 * ResponseAlternativeDelay 'TestSetup.CurrentEnvironment.TestSoundMixerSettings.ResponseAlternativeDelay * 1000
                    MaxResponseTimeTimer.Interval = Math.Max(1, ShowResponseAlternativesTimer.Interval + 1000 * MaximumResponseTime)  ' TestSetup.CurrentEnvironment.TestSoundMixerSettings.MaximumResponseTime * 1000
                Else
                    ShowResponseAlternativesTimer.Interval = Math.Max(1, CurrentSipTrial.TestWordStartTime * 1000) + 1000 * ResponseAlternativeDelay
                    MaxResponseTimeTimer.Interval = Math.Max(2, CurrentSipTrial.TestWordCompletedTime * 1000) + 1000 * MaximumResponseTime
                End If
            End If

            If SimulationMode = False Then

                'Launches the trial if the start timer has ticked, without launching the trial (which happens when the sound preparation was not completed at the tick)
                If StartTrialTimerHasTicked = True Then
                    If CurrentTrialIsLaunched = False Then

                        'Launching the trial
                        LaunchTrial(CurrentTestSound)

                    End If
                End If

            Else

                Select Case SelectedTestparadigm
                    Case Testparadigm.Directional3, Testparadigm.Directional5
                        Throw New Exception("Simulation is not implemented for Testparadigm Directional3 and Directional5")
                End Select

                'Simulating a respons directly without displaying anything on the screen
                'The response is based on the the presented SNR and the hearing level of simulated patient, using a bernoulli trial
                Dim CorrectResponse As String = CurrentSipTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling")

                'Assigning approximate success probabilities depending on SNR and the testee hearing level
                Dim SuccessProbability As Double = CurrentSipTrial.EstimatedSuccessProbability(True)

                Dim BernoulliTrialResult = MathNet.Numerics.Distributions.Bernoulli.Sample(SipMeasurementRandomizer, SuccessProbability)
                Dim SimulatedResponse As String = ""
                If BernoulliTrialResult = 1 Then
                    SimulatedResponse = CorrectResponse
                Else
                    'Selecting an incorrect alternative
                    Dim IncorrectAlternatives = New List(Of String)
                    For Each Spelling In TestWordAlternatives
                        If Spelling.Item1 = CorrectResponse Then Continue For
                        IncorrectAlternatives.Add(Spelling.Item1)
                    Next

                    'Selecting a random incorrect response
                    If IncorrectAlternatives.Count > 0 Then
                        SimulatedResponse = IncorrectAlternatives(SipMeasurementRandomizer.Next(0, IncorrectAlternatives.Count))
                    End If
                End If

                'Calling the response sub
                TestWordResponse_TreadSafe(SimulatedResponse)

            End If

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
        End Try

    End Sub


    Private Sub LaunchTrial(TestSound As Audio.Sound)

        'Declaring a spin lock taken variable
        Dim SpinLockTaken As Boolean = False

        Try

            'Attempts to enter a spin lock to avoid multiple threads calling LaunchTrial before CurrentTrialIsLaunched is updated
            TrialLaunchSpinLock.Enter(SpinLockTaken)

            'Exits the sub if the trial has already been started
            If CurrentTrialIsLaunched = True Then
                Exit Sub
            End If

            'Notes that the current trial has been started
            CurrentTrialIsLaunched = True

            'Removes any controls on the ParticipantControl.TestWordPanel
            ParticipantControl.ResetTestItemPanel()

            'Plays sound
            If SimulationMode = False Then SoundPlayer.SwapOutputSounds(TestSound)

            'Presents the visual que
            If UseVisualQue = True Then
                ShowVisualQueTimer.Start()
                HideVisualQueTimer.Start()
            End If

            'Starts response timers
            ShowResponseAlternativesTimer.Start()
            MaxResponseTimeTimer.Start()

        Finally

            'Releases any spinlock
            If SpinLockTaken = True Then TrialLaunchSpinLock.Exit()
        End Try

    End Sub

    Private Sub ShowVisualQueTimer_Tick() Handles ShowVisualQueTimer.Elapsed
        ShowVisualQueTimer.Stop()
        ParticipantControl.ShowVisualQue()
    End Sub

    Private Sub HideVisualQueTimer_Tick() Handles HideVisualQueTimer.Elapsed
        HideVisualQueTimer.Stop()
        ParticipantControl.HideVisualQue()
    End Sub

    Private Sub ShowResponseAlternativesTimer_Tick() Handles ShowResponseAlternativesTimer.Elapsed
        ShowResponseAlternativesTimer.Stop()

        'Noting the response presentation time, as synchrinized with the presentation of the response alternatives
        ResponseAlternativesPresentationTime = DateTime.Now

        ParticipantControl.ShowResponseAlternatives(TestWordAlternatives)

    End Sub

    Private Sub MaxResponseTimeTimer_Tick() Handles MaxResponseTimeTimer.Elapsed
        MaxResponseTimeTimer.Stop()

        'Calculating the responose time
        Dim ResponseTime As TimeSpan = DateTime.Now - ResponseAlternativesPresentationTime

        'Triggers a time out signal on the testee screen
        ParticipantControl.ResponseTimesOut()

        'Sends an empty result, to signal the lack of response
        StoreResult("", ResponseTime)
    End Sub


    Private Sub TestWordResponse_TreadSafe(ByVal ResponseString As String) Handles ParticipantControl.ResponseGiven

        If Me.InvokeRequired = True Then
            Dim d As New StringArgReturningVoidDelegate(AddressOf TestWordResponse_Unsafe)
            Me.Invoke(d, New Object() {ResponseString})
        Else
            Me.TestWordResponse_Unsafe(ResponseString)
        End If

    End Sub


    Private Sub TestWordResponse_Unsafe(ByVal ResponseString As String)

        'Stopping the max response time timer, as a response has been given in time.
        MaxResponseTimeTimer.Stop()

        'Calculating the responose time
        Dim ResponseTime As TimeSpan

        If SimulationMode = False Then
            ResponseTime = DateTime.Now - ResponseAlternativesPresentationTime
        Else
            'Randomizing a simulated response time
            Dim SimulatedRawRT As Double = MathNet.Numerics.Distributions.Normal.Sample(SipMeasurementRandomizer, 0.5, 0.1)
            Dim ResponseDurationMilliseconds As Double = 1000 * Math.Min(MaximumResponseTime, Math.Max(0.1, 1 / SimulatedRawRT))
            ResponseTime = New TimeSpan(0, 0, 0, Int(ResponseDurationMilliseconds / 1000), Int(ResponseDurationMilliseconds Mod 1000))
        End If

        'Sends the result and response time on
        StoreResult(ResponseString, ResponseTime)

    End Sub

    Private Sub StoreResult(ByVal ResponseString As String, ByVal ResponseTime As TimeSpan)

        'Converting the response time to seconds
        Dim CurrentResponseTime As Integer = ResponseTime.TotalMilliseconds

        'Stores the ResponseMoment
        CurrentSipTrial.ResponseMoment = DateTime.Now

        'Corrects the response
        If ResponseString = CorrectResponse Then
            CurrentSipTrial.Result = SipTest.PossibleResults.Correct

        ElseIf ResponseString = "" Then
            CurrentSipTrial.Result = SipTest.PossibleResults.Missing

            If SimulationMode = True Then
                'Overriding the response time and sets it to max response time
                CurrentResponseTime = MaximumResponseTime
            End If

        Else
            'Determining the erraneous word (among the presented alternatives)
            CurrentSipTrial.Result = SipTest.PossibleResults.Incorrect
        End If


        'Getting the screen position of the test word
        'And getting the response screen position
        Dim TestWordScreenPosition As Integer = -1
        Dim ResponseScreenPosition As Integer? = Nothing

        Select Case SelectedTestparadigm
            Case Testparadigm.Directional3, Testparadigm.Directional5
                For n = 0 To TestWordAlternatives.Count - 1
                    If TestWordAlternatives(n).Item1 & vbTab & TestWordAlternatives(n).Item2.HorizontalAzimuth = CorrectResponse Then TestWordScreenPosition = n
                Next

                For n = 0 To TestWordAlternatives.Count - 1
                    If TestWordAlternatives(n).Item1 & vbTab & TestWordAlternatives(n).Item2.HorizontalAzimuth = ResponseString Then ResponseScreenPosition = n
                Next

            Case Else
                For n = 0 To TestWordAlternatives.Count - 1
                    If TestWordAlternatives(n).Item1 = CorrectResponse Then TestWordScreenPosition = n
                Next

                For n = 0 To TestWordAlternatives.Count - 1
                    If TestWordAlternatives(n).Item1 = ResponseString Then ResponseScreenPosition = n
                Next
        End Select

        'These can be used to store the screen position of the alternatives!
        'CurrentSipTrial.CorrectScreenPosition = TestWordScreenPosition
        'CurrentSipTrial.ResponseScreenPosition = ResponseScreenPosition

        'Stores the response time 
        CurrentSipTrial.ResponseTime = CurrentResponseTime

        'Stores the response
        CurrentSipTrial.Response = ResponseString

        'Moves the trials to from planned to observed trials so thatr it doesn't get presented again
        CurrentSipTestMeasurement.MoveTrialToHistory(CurrentSipTrial)

        'Updates the progress bar
        If ShowProgressIndication = True Then
            ParticipantControl.UpdateTestFormProgressbar(CurrentSipTestMeasurement.ObservedTrials.Count, CurrentSipTestMeasurement.ObservedTrials.Count + CurrentSipTestMeasurement.PlannedTrials.Count)
        End If

        'Starting the next trial
        If SimulationMode = False Then
            PrepareAndLaunchTrial_ThreadSafe()
        Else
            StartTrialTimerTick()
        End If

    End Sub

    Private Sub FinalizeTesting()

        StopAllTimers()

        Try

            Stop_AudioButton.Enabled = False

            ParticipantControl.ResetTestItemPanel()

            ParticipantControl.ShowMessage("Testet är klart!")
            'ParticipantControl.ShowMessage(GUIDictionary.SubTestIsCompleted)

            'Saving results to the log folder
            If SimulationMode = False Then
                SoundPlayer.SwapOutputSounds(Nothing)

                'Sleeps during the fade out phase
                Threading.Thread.Sleep(SoundPlayer.GetOverlapDuration * 1000)

            End If

            'Summarizes the result
            CurrentSipTestMeasurement.SummarizeTestResults()
            MeasurementHistory.Measurements.Add(CurrentSipTestMeasurement)

            'Display results
            PopulateTestHistoryTables()

            'TODO: Should we auto-export data here, or let the user be responsible for saving the test results?
            'If SimulationMode = False Then MeasurementHistory.SaveToFile(Path.Combine(Utils.logFilePath, "AutoLoggedResults"))
            'If SimulationMode = False Then MeasurementHistory.SaveToFile(Path.Combine(Utils.logFilePath, "AutoLoggedResults"))

            'Resets values to prepare for next measurement
            ResetValuesAfterMeasurement()

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
        End Try

    End Sub

    Private Sub ResetValuesAfterMeasurement()

        TestIsPaused = False
        TestIsStarted = False

        'Unlocks the cursor in Pc mode
        If CurrentScreenType = ScreenType.Pc Then UnlockCursor()

        TestDescriptionTextBox.Text = ""

        UnlockSettingsPanels()

        TogglePlayButton(True)
        Start_AudioButton.Enabled = False

        'Calls TryCalculatePsychometricFunction to create a new instance of SiPmeasuement based on the settings already selected
        TryCalculatePsychometricFunction()

    End Sub

    Private Sub PauseTesting()
        TestIsPaused = True

        'Stopping response timers
        StopAllTimers()

        'Resets the test word panel
        ParticipantControl.ResetTestItemPanel()

        'Changing to silence
        SoundPlayer.SwapOutputSounds(Nothing)

        'Displays a message to the testee
        ParticipantControl.ShowMessage("Testet är pausat")
        'ParticipantControl.ShowMessage(GUIDictionary.TestingIsPaused)

        TogglePlayButton(True)

    End Sub

    Private Sub ResumeTesting()

        If TestIsPaused = True Then
            TestIsPaused = False
            TogglePlayButton(False)
            ParticipantControl.ResetTestItemPanel()
            PrepareAndLaunchTrial_ThreadSafe()
        End If

    End Sub

    Private Sub StopAllTimers()
        StartTrialTimer.Stop()
        ShowVisualQueTimer.Stop()
        HideVisualQueTimer.Stop()
        ShowResponseAlternativesTimer.Stop()
        MaxResponseTimeTimer.Stop()
    End Sub

    Private Sub UpdateTestProgress()

        Dim Max As Integer = CurrentSipTestMeasurement.ObservedTrials.Count + CurrentSipTestMeasurement.PlannedTrials.Count
        Dim Progress As Integer = CurrentSipTestMeasurement.ObservedTrials.Count
        Dim NumberObservedScore = CurrentSipTestMeasurement.GetNumberObservedScore
        Dim ProportionCorrect As String = CurrentSipTestMeasurement.PercentCorrect

        MeasurementProgressBar.Minimum = 0
        MeasurementProgressBar.Maximum = Max
        MeasurementProgressBar.Value = Progress

        If NumberObservedScore IsNot Nothing Then
            CorrectCountTextBox.Text = NumberObservedScore.Item1 & " / " & Progress
        Else
            CorrectCountTextBox.Text = 0 & " / " & Progress
        End If

        If ProportionCorrect <> "" Then
            ProportionCorrectTextBox.Text = ProportionCorrect
        Else
            ProportionCorrectTextBox.Text = ""
        End If
    End Sub

    Private Sub UpdateTestTrialTable()

        Dim GetGuiTableData = CurrentSipTestMeasurement.GetGuiTableData()

        Dim TestWords() As String = GetGuiTableData.TestWords.ToArray
        Dim Responses() As String = GetGuiTableData.Responses.ToArray
        Dim ResultResponseTypes() As SipTest.PossibleResults = GetGuiTableData.ResponseType.ToArray
        Dim UpdateRow As Integer? = GetGuiTableData.UpdateRow
        Dim SelectionRow As Integer? = GetGuiTableData.SelectionRow
        Dim FirstRowToDisplayInScrollmode As Integer? = GetGuiTableData.FirstRowToDisplayInScrollmode


        'Checking input arguments
        If TestWords.Length <> Responses.Length Or TestWords.Length <> ResultResponseTypes.Length Then
            Throw New ArgumentException("TestWords, Responses and ResultResponseTypes must all have the same length!")
        End If

        If UpdateRow.HasValue = True Then
            If UpdateRow < 0 Or UpdateRow >= Responses.Length Then Throw New ArgumentException("UpdateRow must be non-negative integer, less than the length of the number of test-list items!")
        End If

        If SelectionRow.HasValue = True Then
            If SelectionRow < 0 Or SelectionRow >= Responses.Length Then Throw New ArgumentException("SelectionRow must be non-negative integer, less than the length of the number of test-list items!")
        End If

        If FirstRowToDisplayInScrollmode.HasValue = True Then
            If FirstRowToDisplayInScrollmode < 0 Or FirstRowToDisplayInScrollmode >= Responses.Length Then Throw New ArgumentException("FirstRowToDisplayInScrollmode must be non-negative integer, less than the length of the number of test-list items!")
        End If


        'Determines if the whole table can be 
        Dim UpdateOnlySpecificRow As Boolean = False

        If UpdateRow.HasValue = True Then
            If Responses.Length = TestTrialDataGridView.Rows.Count Then
                'Allows updating of only a specific row if 
                '-an UpdateIndex is given
                '-the number of rows in the existing table equals the number of test-list items (taken as the length of TestWords). This could happen if the number of test-list items have changed.
                UpdateOnlySpecificRow = True
            End If
        End If

        If UpdateOnlySpecificRow = True Then

            TestTrialDataGridView.Rows(UpdateRow.Value).Cells(0).Value = TestWords(UpdateRow.Value)
            TestTrialDataGridView.Rows(UpdateRow.Value).Cells(1).Value = Responses(UpdateRow.Value)

            Select Case ResultResponseTypes(UpdateRow.Value)
                Case SipTest.PossibleResults.Correct
                    TestTrialDataGridView.Rows(UpdateRow.Value).Cells(2).Value = My.Resources.CorrectResponseImage

                Case SipTest.PossibleResults.Incorrect
                    TestTrialDataGridView.Rows(UpdateRow.Value).Cells(2).Value = My.Resources.IncorrectResponseImage

                Case Else
                    TestTrialDataGridView.Rows(UpdateRow.Value).Cells(2).Value = My.Resources.IncorrectResponseImage
            End Select

        Else

            'Clearing all rows
            TestTrialDataGridView.Rows.Clear()

            'Creating new rows
            TestTrialDataGridView.Rows.Add(TestWords.Length)

            'Adding data to all rows
            For r = 0 To TestWords.Length - 1

                TestTrialDataGridView.Rows(r).Cells(0).Value = TestWords(r)
                TestTrialDataGridView.Rows(r).Cells(1).Value = Responses(r)

                Select Case ResultResponseTypes(r)
                    Case SipTest.PossibleResults.Correct
                        TestTrialDataGridView.Rows(r).Cells(2).Value = My.Resources.CorrectResponseImage

                    Case SipTest.PossibleResults.Incorrect
                        TestTrialDataGridView.Rows(r).Cells(2).Value = My.Resources.IncorrectResponseImage

                    Case Else
                        TestTrialDataGridView.Rows(r).Cells(2).Value = My.Resources.TrialNotPresentedImage
                End Select


            Next

        End If

        'Sets the selection row
        'Clears any selection first
        TestTrialDataGridView.ClearSelection()
        If SelectionRow.HasValue Then
            'Selects the first column of the SelectionRow
            TestTrialDataGridView.Rows(SelectionRow).Cells(0).Selected = True
        End If

        'Scrolls to the indicated FirstRowToDisplayInScrollmode
        If FirstRowToDisplayInScrollmode.HasValue Then
            TestTrialDataGridView.FirstDisplayedScrollingRowIndex = FirstRowToDisplayInScrollmode
        End If

    End Sub

    Private Sub CurrentSessionResults_DataGridView_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles CurrentSessionResults_DataGridView.CurrentCellDirtyStateChanged

        'This extra event handler is needed since the CellValueChanged event does not always trigger for DataGridViewCheckBoxCells. See https://stackoverflow.com/questions/11843488/how-to-detect-datagridview-checkbox-event-change for this solution
        Dim Result = TryCast(sender.CurrentCell, DataGridViewCheckBoxCell)
        If Result IsNot Nothing Then
            sender.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If

    End Sub

    Private Sub SessionResults_DataGridView_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles CurrentSessionResults_DataGridView.CellValueChanged

        'Exits sub if invalid indices are sent
        If e.RowIndex < 0 Then Exit Sub
        If e.ColumnIndex < 0 Then Exit Sub
        If e.RowIndex > sender.Rows.count - 1 Then Exit Sub
        If e.ColumnIndex > sender.Columns.count - 1 Then Exit Sub

        'Ignores any calls that do not come from the third column (i.e. the check-box column)
        If e.ColumnIndex <> 3 Then Exit Sub

        'Adding/removing the appropriate test GuiDescriptions to/from the TestComparisonHistory.
        If sender.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = True Then

            'Checks to see whether to add the test to the TestComparisonHistory and perform a significance test

            'Updating the testcomparison history with the description string of checked test
            If Not TestComparisonHistory.Contains(CurrentSessionResults_DataGridView.Rows(e.RowIndex).Cells(0).Value) Then
                TestComparisonHistory.Add(CurrentSessionResults_DataGridView.Rows(e.RowIndex).Cells(0).Value)
            End If

            'Removing anything but the two last items in TestComparisonHistory
            If TestComparisonHistory.Count > 2 Then
                TestComparisonHistory.RemoveRange(0, TestComparisonHistory.Count - 2)
            End If

            'Updating the checkboxes in both the CurrentSessionResults_DataGridView and the PreviousSessionsResults_DataGridView based on the last two selected values
            For r = 0 To CurrentSessionResults_DataGridView.Rows.Count - 1
                If TestComparisonHistory.Contains(CurrentSessionResults_DataGridView.Rows(r).Cells(0).Value) Then
                    CurrentSessionResults_DataGridView.Rows(r).Cells(3).Value = True
                Else
                    CurrentSessionResults_DataGridView.Rows(r).Cells(3).Value = False
                End If
            Next

        Else

            'Removes the test from testcomparison if it is there as the test was unchecked (This need to loop because Remove only removes the first occurence...)
            Do Until TestComparisonHistory.Contains(CurrentSessionResults_DataGridView.Rows(e.RowIndex).Cells(0).Value) = False
                TestComparisonHistory.Remove(CurrentSessionResults_DataGridView.Rows(e.RowIndex).Cells(0).Value)
            Loop

        End If

        'Sending a call for statistical analysis to the backend. 
        CompareTwoSipTestScores(TestComparisonHistory)

    End Sub


    Private Sub PopulateTestHistoryTables()

        'Clears all rows in the TestHistoryTables
        CurrentSessionResults_DataGridView.Rows.Clear()

        'Adds rows
        CurrentSessionResults_DataGridView.Rows.Add(MeasurementHistory.Measurements.Count)

        'Adds data
        For r = 0 To MeasurementHistory.Measurements.Count - 1
            CurrentSessionResults_DataGridView.Rows(r).Cells(0).Value = MeasurementHistory.Measurements(r).Description
            CurrentSessionResults_DataGridView.Rows(r).Cells(1).Value = MeasurementHistory.Measurements(r).ObservedTestLength
            CurrentSessionResults_DataGridView.Rows(r).Cells(2).Value = MeasurementHistory.Measurements(r).PercentCorrect
            CurrentSessionResults_DataGridView.Rows(r).Cells(3).Value = False 'Setting selected value to false by default
        Next

    End Sub

    Private Sub UpdateSignificanceTestResult(Result As String)

        SignificanceTestResult_RichTextBox.Text = Result

        'Also showing/hiding the StatAnalysisLabel depending on the information in Result
        If Result = "" Then
            StatAnalysisLabel.Visible = False
        Else
            StatAnalysisLabel.Visible = True
        End If

    End Sub



    ''' <summary>
    ''' Performs a statistical analysis of the score difference between the SiP-testet refered to in ComparedMeasurementGuiDescriptions by their GuiDescription strings
    ''' </summary>
    ''' <param name="ComparedMeasurementGuiDescriptions"></param>
    Private Sub CompareTwoSipTestScores(ByRef ComparedMeasurementGuiDescriptions As List(Of String))

        'Clears the Gui significance test result box if not exaclty two measurements descriptions are recieved. And the exits the sub
        If ComparedMeasurementGuiDescriptions.Count = 2 Then

            Dim MeasurementDescription As New List(Of String)

            Dim MeasurementsToCompare As New List(Of SipMeasurement)
            For Each Measurement In MeasurementHistory.Measurements
                If ComparedMeasurementGuiDescriptions.Contains(Measurement.Description) Then
                    MeasurementsToCompare.Add(Measurement)
                    MeasurementDescription.Add(Measurement.Description)
                End If
            Next

            Dim Result = CriticalDifferences.IsNotSignificantlyDifferent_PBAC(MeasurementsToCompare(0).GetAdjustedSuccessProbabilities, MeasurementsToCompare(1).GetAdjustedSuccessProbabilities, 0.95)

            If Result = False Then
                'Significant
                UpdateSignificanceTestResult("The difference (" & Math.Round(100 * Math.Abs(MeasurementsToCompare(0).GetAverageObservedScore - MeasurementsToCompare(1).GetAverageObservedScore)) & " % points) between " &
                                             MeasurementDescription(0) & " and " & MeasurementDescription(1) & " is statistically significant (p < 0.05).")
            Else
                'Not significant
                UpdateSignificanceTestResult("The difference (" & Math.Round(100 * Math.Abs(MeasurementsToCompare(0).GetAverageObservedScore - MeasurementsToCompare(1).GetAverageObservedScore)) & " % points) between " &
                                             MeasurementDescription(0) & " and " & MeasurementDescription(1) & " is NOT statistically significant (p < 0.05).")
            End If

        Else
            UpdateSignificanceTestResult("")
        End If

    End Sub


    Private Sub SaveFileButtonPressed() Handles ExportData_Button.Click

        If CurrentParticipantID Is Nothing Then
            MsgBox("No participant selected!", MsgBoxStyle.Exclamation, "Exporting measurements")
        End If

        MeasurementHistory.SaveToFile()

    End Sub

    Private Sub OpenFileButtonPressed() Handles ImportData_Button.Click

        If CurrentParticipantID Is Nothing Then
            MsgBox("No participant selected!", MsgBoxStyle.Exclamation, "Importing measurements")
            Exit Sub
        End If

        Dim ImportedMeasurements = MeasurementHistory.LoadMeasurements(SpeechMaterial.ParentTestSpecification,, CurrentParticipantID)

        If ImportedMeasurements Is Nothing Then Exit Sub

        For Each LoadedMeasurement In ImportedMeasurements.Measurements
            MeasurementHistory.Measurements.Insert(0, LoadedMeasurement)
        Next

        'Displaying the loded tests
        PopulateTestHistoryTables()

    End Sub

    ''' <summary>
    ''' This method can be called by the backend in order to display a message box message to the user.
    ''' </summary>
    ''' <param name="Message"></param>
    Private Sub ShowMessageBox(Message As String, Optional ByVal Title As String = "SiP-testet")

        MsgBox(Message, MsgBoxStyle.Information, Title)

    End Sub

    Private Function ShowYesNoMessageBox(Question As String, Optional Title As String = "SiP-testet") As Boolean

        Dim Result = MsgBox(Question, MsgBoxStyle.YesNo, Title)

        If Result = MsgBoxResult.Yes Then
            Return True
        Else
            Return False
        End If

    End Function


#Region "ParticipantScreens"

    Private Sub PcScreen_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles PcScreen_RadioButton.CheckedChanged

        If PcScreen_RadioButton.Checked = True Then

            'Disconnects the BT screen
            DisconnectWirelessScreen()

            'Sets the CurrentScreenType 
            CurrentScreenType = ScreenType.Pc

            'Enables/disables controls
            BtScreen_TableLayoutPanel.Enabled = False
            PcScreen_TableLayoutPanel.Enabled = True

            'Clearing items in the Screen_ComboBox
            PcScreen_ComboBox.Items.Clear()

            'Adding all screens into the Screen_ComboBox
            Dim Screens() As Screen = Screen.AllScreens
            For Each Screen In Screens
                PcScreen_ComboBox.Items.Add(Screen.DeviceName)
            Next

            'Preselects the first screen (which will also create the PC participant form!)
            If PcScreen_ComboBox.Items.Count > 0 Then PcScreen_ComboBox.SelectedIndex = 0

        End If

    End Sub

    '    Private Sub BtScreen_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles BtScreen_RadioButton.CheckedChanged
    Private Sub SetBtScreen() Handles BtScreen_RadioButton.CheckedChanged

        If BtScreen_RadioButton.Checked = True Then

            'Sets the CurrentScreenType 
            CurrentScreenType = ScreenType.Bluetooth

            'Enables/disables controls
            BtScreen_TableLayoutPanel.Enabled = True
            PcScreen_TableLayoutPanel.Enabled = False

            'Clearing items in the Screen_ComboBox
            PcScreen_ComboBox.Items.Clear()

            If PcParticipantForm IsNot Nothing Then
                PcParticipantForm.Close()
                PcParticipantForm.Dispose()
                PcParticipantForm = Nothing
            End If

        End If

    End Sub

    Private Sub PcTouch_CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles PcTouch_CheckBox.CheckedChanged
        If PcTouch_CheckBox.Checked = True Then
            PcResponseMode = Utils.Constants.ResponseModes.TabletTouch
        Else
            PcResponseMode = Utils.Constants.ResponseModes.MouseClick
        End If

        If PcParticipantForm IsNot Nothing Then
            PcParticipantForm.SetResponseMode(PcResponseMode)
        End If

    End Sub

    'Private Sub PcScreen_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles PcScreen_ComboBox.SelectedIndexChanged
    Private Sub SetPcScreen() Handles PcScreen_ComboBox.SelectedIndexChanged

        If CurrentScreenType = ScreenType.Pc Then

            'Creating a new participant form (and ParticipantControl) if none exist
            If PcParticipantForm Is Nothing Then
                PcParticipantForm = New PcTesteeForm(PcTesteeForm.TaskType.ForcedChoice)
                ParticipantControl = PcParticipantForm.ParticipantControl
            End If
            PcParticipantForm.Show()

            'Selects the screen that comes in the iterated order returned by Screen.AllScreens, which repressent the order screens are added into Screen_ComboBox, and also the order they are selectd in ChangeTestFormScreen
            PcParticipantForm.ChangeTestFormScreen(PcScreen_ComboBox.SelectedIndex)

        End If

    End Sub


#Region "BlueToothConnection"

    Private Sub SendBTMessageToolStripMenuItem_Click(sender As Object, e As EventArgs) 'Handles SendBTMessageToolStripMenuItem.Click

        If MyBtTesteeControl IsNot Nothing Then
            MyBtTesteeControl.BtTabletTalker.SendBtMessage("ping")
        End If

    End Sub

    Private Sub ConnectBluetoothScreenButton_Click(sender As Object, e As EventArgs) Handles ConnectBluetoothScreen_Button.Click

        Dim Failed As Boolean = False
        If MyBtTesteeControl Is Nothing Then

            'MyBtTabletTalker = New BtTabletTalker(Me, Bt_UUID, Bt_PIN)
            'If MyBtTabletTalker.EstablishBtConnection() = False Then Failed = True

            'Creating a new BtTesteeControl (This should be reused as long as the connection is open!)
            MyBtTesteeControl = New BtTesteeControl()

            If MyBtTesteeControl.Initialize(Bt_UUID, Bt_PIN, GuiLanguage) = False Then
                Failed = True
            End If

        Else
            If MyBtTesteeControl.BtTabletTalker.TrySendData() = False Then Failed = True
        End If

        If Failed = True Then
            MsgBox("Ingen blåtandsenhet kunde anslutas, vänligen försök igen!")
            MyBtTesteeControl = Nothing
            DisconnectWirelessScreen()
            BtLamp.State = Lamp.States.Disabled
            Exit Sub
        Else
            BtLamp.State = Lamp.States.On
        End If

        ConnectBluetoothScreen_Button.Enabled = False

        'Calling UseBtScreen to set things up
        UseBtScreen()

    End Sub

    Private Sub DisconnectWirelessScreenButton_Click(sender As Object, e As EventArgs) Handles DisconnectBtScreen_Button.Click
        DisconnectWirelessScreen()
    End Sub

    Private Sub DisconnectWirelessScreen()

        If MyBtTesteeControl IsNot Nothing Then
            Try
                MyBtTesteeControl.BtTabletTalker.DisconnectBT()
                MyBtTesteeControl.BtTabletTalker.Dispose()
                MyBtTesteeControl.BtTabletTalker = Nothing
                MyBtTesteeControl = Nothing
            Catch ex As Exception
                MyBtTesteeControl = Nothing
            End Try
        End If

        ConnectBluetoothScreen_Button.Enabled = True

        PcScreen_RadioButton.Checked = True

        BtLamp.State = Lamp.States.Disabled

    End Sub

    Private Sub UseBtScreen()


        Dim Failed As Boolean = False
        Try
            If MyBtTesteeControl.BtTabletTalker.TrySendData = True Then
                CurrentScreenType = ScreenType.Bluetooth

                'Enabling the AvailableTestsComboBox if not already done
                DisconnectBtScreen_Button.Enabled = True

            Else
                Failed = True
            End If
        Catch ex As Exception
            Failed = True
        End Try

        If Failed = True Then
            MsgBox("Anslutningen till blåtandsskärmen har gått förlorad.")

            ' Calls DisconnectWirelessScreen to set correct enabled status of all controls
            DisconnectWirelessScreen()

        End If

    End Sub

    Private Sub SipTestGui_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        'Closing the PcResponseForm
        Try
            If PcParticipantForm IsNot Nothing Then
                PcParticipantForm.Close()
            End If
        Catch ex As Exception
            'Ignores any error
        End Try

        If IsStandAlone = True Then OstfBase.TerminateOSTF()

    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click

        Dim MyAboutBox = New AboutBox_WithLicenseButton
        MyAboutBox.SelectedLicense = LicenseBox.AvailableLicenses.MIT_X11
        MyAboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.PortAudio)
        MyAboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.MathNet)
        MyAboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.InTheHand)
        MyAboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.Wierstorf)
        MyAboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.SwedishSipRecordings)
        MyAboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.Modified_IFFM)
        MyAboutBox.Show()

    End Sub


#End Region

#End Region


#Region "ExperimentalStuff"

    Private Sub SimulateAdaptiveTests()

        Dim Ms As New List(Of Integer) From {1, 2, 3}

        For Each M In Ms


            Dim Simulations As Integer = 500

            Dim Seed As Integer = 42
            Dim Rnd As New Random(Seed)

            Dim TestResults As New List(Of String)
            Dim ProgressDisplay As New ProgressDisplay
            ProgressDisplay.Initialize(Simulations, 0, "Simulating adaptive tests...", 1)
            ProgressDisplay.Show()

            For i = 1 To Simulations

                ProgressDisplay.UpdateProgress(i)

                Dim NewResult = SimulateAdaptiveTest(Rnd, M)

                TestResults.Add(i.ToString() & vbTab & vbTab &
                                    NewResult.Item1.Last.ToString & vbTab & vbTab &
                                    NewResult.Item1.Average.ToString & vbTab & vbTab &
                                    String.Join(vbTab, NewResult.Item1) & vbTab & vbTab &
                                    String.Join(vbTab, NewResult.Item2))

            Next

            Utils.SendInfoToLog(vbCrLf & vbCrLf & "Method: " & M & vbCrLf & String.Join(vbCrLf, TestResults), "SimulationResults")

            ProgressDisplay.Close()

        Next

        MsgBox("Simulation Complete")

    End Sub


    Private Function SimulateAdaptiveTest(ByRef Rnd As Random, ByVal Method As Integer) As Tuple(Of List(Of Double), List(Of String))

        Dim ReferenceLevel As Double = 68
        Dim CurrentPNR As Double = 0
        Dim Elapses As Integer = 0
        Dim PNRList As New List(Of Double)
        Dim ResponseList As New List(Of String)

        Dim ResultList As New List(Of Integer)

        For Each Trial In CurrentSipTestMeasurement.PlannedTrials

            PNRList.Add(CurrentPNR)

            Trial.SetLevels(ReferenceLevel, CurrentPNR)

            Dim p = Trial.EstimatedSuccessProbability(True)

            'Simulating a test trial response by sampling from a bernoulli distribution

            Dim BernoulliTrialResult = MathNet.Numerics.Distributions.Bernoulli.Sample(Rnd, p)
            Dim SimulatedResponse As String = ""

            ResultList.Add(BernoulliTrialResult)

            Dim StepSize As Double
            If Elapses >= 30 Then
                StepSize = 1
            Else
                StepSize = 2
            End If


            Select Case Method
                Case 1
                    If ResultList.Count = 3 Then
                        ChangePNR(CurrentPNR, ResultList, StepSize)
                        ResultList.Clear()
                    End If
                Case 2
                    If ResultList.Count = 6 Then
                        ChangePNR(CurrentPNR, ResultList, StepSize)
                        ResultList.Clear()
                    End If
                Case 3
                    If ResultList.Count = 12 Then
                        ChangePNR(CurrentPNR, ResultList, StepSize)
                        ResultList.Clear()
                    End If
            End Select

            If BernoulliTrialResult = 1 Then
                SimulatedResponse = "Correct"
            Else
                SimulatedResponse = "Incorrect"
            End If

            Elapses += 1

            ResponseList.Add(SimulatedResponse)

        Next

        Return New Tuple(Of List(Of Double), List(Of String))(PNRList, ResponseList)

    End Function

    Private Sub ChangePNR(ByRef CurrentPNR As Double, ResultList As List(Of Integer), ByVal StepSize As Double)
        Select Case ResultList.Average
            Case <= (2 / 3)
                CurrentPNR += StepSize
            Case 1
                CurrentPNR -= StepSize
            Case Else
                'Do not change!
        End Select
    End Sub


    Private Sub TestingSpeed_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Testparadigm_ComboBox.SelectedIndexChanged

        If Testparadigm_ComboBox.SelectedItem IsNot Nothing Then
            SelectedTestparadigm = Testparadigm_ComboBox.SelectedItem
        End If

        Select Case SelectedTestparadigm
            Case Testparadigm.Slow

                InterTrialInterval = 1
                ResponseAlternativeDelay = 0.5
                PretestSoundDuration = 5
                MinimumStimulusOnsetTime = 0.3 ' Earlier, when this variable directed the test words instead of maskers its value was 1.5. Having maskers of 3 seconds with the test word centralized, should be approximately the same as 0.3.
                MaximumStimulusOnsetTime = 0.8 ' Earlier, when this variable directed the test words instead of maskers its value was 2
                TrialSoundMaxDuration = 10 ' TODO: Optimize by shortening this time
                UseVisualQue = True
                UseBackgroundSpeech = True
                MaximumResponseTime = 4
                ShowProgressIndication = True

            Case Testparadigm.Quick

                InterTrialInterval = 0.1
                ResponseAlternativeDelay = 0
                PretestSoundDuration = 2
                MinimumStimulusOnsetTime = 0.02
                MaximumStimulusOnsetTime = 0.1
                TrialSoundMaxDuration = 4 ' TODO: Optimize by shortening this time
                UseVisualQue = False
                UseBackgroundSpeech = False
                MaximumResponseTime = 1
                ShowProgressIndication = True

            Case Testparadigm.Directional3, Testparadigm.Directional5

                InterTrialInterval = 1
                ResponseAlternativeDelay = 0.5
                PretestSoundDuration = 5
                MinimumStimulusOnsetTime = 0.3 ' Earlier, when this variable directed the test words instead of maskers its value was 1.5. Having maskers of 3 seconds with the test word centralized, should be approximately the same as 0.3.
                MaximumStimulusOnsetTime = 0.8 ' Earlier, when this variable directed the test words instead of maskers its value was 2
                TrialSoundMaxDuration = 10 ' TODO: Optimize by shortening this time
                UseVisualQue = False
                UseBackgroundSpeech = False
                MaximumResponseTime = 4
                ShowProgressIndication = True

        End Select

        TryCalculatePsychometricFunction()

    End Sub

    Private Sub StopButton_Click(sender As Object, e As EventArgs) Handles Stop_AudioButton.Click

    End Sub


#End Region


End Class



