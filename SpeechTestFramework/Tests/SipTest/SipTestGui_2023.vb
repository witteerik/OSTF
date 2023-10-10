Imports SpeechTestFramework.SipTest
Imports SpeechTestFramework.WinFormControls
Imports System.Windows.Forms
Imports System.Drawing

Public Class SipTestGui_2023

    ''' <summary>
    ''' Set to false to show all available test modes
    ''' </summary>
    Private DisableExtraTestModes As Boolean = True

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

    Private ReadOnly AvailableReferenceLevels As New List(Of Double) From {
        Audio.GlobalAudioData.RaisedVocalEffortLevel - 10,
        Audio.GlobalAudioData.RaisedVocalEffortLevel - 5,
        Audio.GlobalAudioData.RaisedVocalEffortLevel,
        Audio.GlobalAudioData.RaisedVocalEffortLevel + 5,
        Audio.GlobalAudioData.RaisedVocalEffortLevel + 10}

    Private AvailablePresetsNames As List(Of String)
    Private CustomPresetCount As Integer = 1
    Private AvailableMediaSets As MediaSetLibrary

    Public Enum TestModes
        Directional
        BMLD
        Custom1
    End Enum

    Private NoSimulationString As String = "No simulation"
    Public TestMode As TestModes = TestModes.Directional
    Private AvailableTargetAzimuths As New List(Of Double) From {-90, -60, -30, 0, 30, 60, 90}
    Private AvailableMaskerAzimuths As New List(Of Double) From {-150, -120, -90, -60, -30, 0, 30, 60, 90, 120, 150, 180}
    Private AvailableBackgroundAzimuths As New List(Of Double) From {-150, -120, -90, -60, -30, 0, 30, 60, 90, 120, 150, 180}
    'Private AvailableLengthReduplications As New List(Of Integer) From {1, 2, 3, 4}
    Private AvailableLengthReduplications As New List(Of Integer) From {1}
    'Private AvailablePNRs As New List(Of Double) From {-15, -12, -10, -8, -6, -4, -2, 0, 2, 4, 6, 8, 10, 12, 15}
    Private AvailablePNRs As New List(Of Double) From {-20, -16, -10, -3, 4, 11}
    Private BmldTargetModeStrings As New List(Of String) From {"R", "L", "0", "π"} ' For Right, Left, Zero, Pi,
    Private BmldNoiseModeStrings As New List(Of String) From {"R", "L", "0", "π", "U"} ' For Right, Left, Zero, Pi, Uncorrelated (i.e. different noises binaurally)

    Private AvailableSimultaneousNoisesCount As New List(Of Integer) From {1, 2, 3, 4, 5}
    Private ReadOnly DefaultSelectedSimultaneousNoisesCountIndex As Integer = 0
    ''' <summary>
    ''' Holds the (zero-based) index of the default reference level in the AvailableReferenceLevels object
    ''' </summary>
    Private ReadOnly DefaultReferenceLevelIndex As Integer = 2

    Private CurrentParticipantID As String = ""

    Private SelectedReferenceLevel As Double?
    Private SelectedPresetName As String = ""
    Private SelectedLengthReduplications As Integer?
    Private NumberOfSimultaneousMaskers As Integer?
    Private SelectedTestDescription As String = ""

    Private SelectedPNRs As New List(Of Double)
    Private SelectedMediaSets As New List(Of MediaSet)

    Private SipMeasurementRandomizer As New Random

    Private MeasurementHistory As New MeasurementHistory
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
    Delegate Sub ResultDelegate(ResponseString As String, ResponseTime As TimeSpan)


    Public SelectedSoundPropagationType As SoundPropagationTypes = SoundPropagationTypes.PointSpeakers

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


    Private Sub SipTestGui_2023_Load(sender As Object, e As EventArgs) Handles MyBase.Load

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

        'Adding available reference levels
        ReferenceLevelComboBox.Items.Clear()
        For Each RefLevel In AvailableReferenceLevels
            ReferenceLevelComboBox.Items.Add(RefLevel)
        Next
        If DefaultReferenceLevelIndex < ReferenceLevelComboBox.Items.Count Then
            ReferenceLevelComboBox.SelectedIndex = DefaultReferenceLevelIndex
        End If

        'Adding a default noise count 
        For Each NoiseCount In AvailableSimultaneousNoisesCount
            SimultaneousMaskersCount_ComboBox.Items.Add(NoiseCount)
        Next
        If DefaultSelectedSimultaneousNoisesCountIndex < SimultaneousMaskersCount_ComboBox.Items.Count Then
            SimultaneousMaskersCount_ComboBox.SelectedIndex = DefaultSelectedSimultaneousNoisesCountIndex
        End If

        'Adding available preset names
        AvailablePresetsNames = SpeechMaterial.Presets.Keys.ToList
        PresetComboBox.Items.Clear()
        For Each Preset In AvailablePresetsNames
            PresetComboBox.Items.Add(Preset)
        Next

        'Adding available test situations
        For Each TestSituation In AvailableMediaSets
            Dim NewCheckBox = New CheckBox With {.Text = TestSituation.MediaSetName}
            AddHandler NewCheckBox.CheckedChanged, AddressOf TryCreateSipTestMeasurement
            Situations_FlowLayoutPanel.Controls.Add(NewCheckBox)
        Next
        If Situations_FlowLayoutPanel.Controls.Count > 0 Then
            'Checks the first CheckBox
            DirectCast(Situations_FlowLayoutPanel.Controls(0), CheckBox).Checked = True
        End If


        'Adding available test length reduplications
        TestLengthComboBox.Items.Clear()
        For Each TestLength In AvailableLengthReduplications
            TestLengthComboBox.Items.Add(TestLength)
        Next
        If TestLengthComboBox.Items.Count > 0 Then
            TestLengthComboBox.SelectedIndex = 0
        End If

        'Adding possible PNR values
        For Each PNR In AvailablePNRs
            Dim NewCheckBox = New CheckBox With {.Text = PNR, .Checked = True}
            AddHandler NewCheckBox.CheckedChanged, AddressOf TryCreateSipTestMeasurement
            PNRs_FlowLayoutPanel.Controls.Add(NewCheckBox)
        Next

        'Adding sound source azimuth values - target
        For Each Azimuth In AvailableTargetAzimuths
            Dim NewCheckBox = New CheckBox With {.Text = Azimuth}
            AddHandler NewCheckBox.CheckedChanged, AddressOf TryCreateSipTestMeasurement
            SpeechAzimuth_FlowLayoutPanel.Controls.Add(NewCheckBox)
        Next

        'Adding sound source azimuth values - masker
        For Each Azimuth In AvailableMaskerAzimuths
            Dim NewCheckBox = New CheckBox With {.Text = Azimuth}
            AddHandler NewCheckBox.CheckedChanged, AddressOf TryCreateSipTestMeasurement
            MaskerAzimuth_FlowLayoutPanel.Controls.Add(NewCheckBox)
        Next

        'Adding sound source azimuth values - background
        For Each Azimuth In AvailableBackgroundAzimuths
            Dim NewCheckBox = New CheckBox With {.Text = Azimuth}
            AddHandler NewCheckBox.CheckedChanged, AddressOf TryCreateSipTestMeasurement
            BackgroundAzimuth_FlowLayoutPanel.Controls.Add(NewCheckBox)
        Next

        'Adding Target and Noise modes for BMLD
        For Each Value In BmldTargetModeStrings
            BmldSignalMode_ComboBox.Items.Add(Value)
        Next

        For Each Value In BmldNoiseModeStrings
            BmldNoiseMode_ComboBox.Items.Add(Value)
        Next

        SetLanguageStrings(GuiLanguage)

        StartSoundPlayer()

        'Adding values in Testparadigm_ComboBox
        Testparadigm_ComboBox.Items.Add(Testparadigm.FlexibleLocations)
        If DisableExtraTestModes = False Then
            Testparadigm_ComboBox.Items.Add(Testparadigm.Quick)
            Testparadigm_ComboBox.Items.Add(Testparadigm.Slow)
            Testparadigm_ComboBox.Items.Add(Testparadigm.Directional2)
            Testparadigm_ComboBox.Items.Add(Testparadigm.Directional3)
            Testparadigm_ComboBox.Items.Add(Testparadigm.Directional5)
        End If
        Testparadigm_ComboBox.SelectedIndex = 0

        If DisableExtraTestModes = True Then

            'Removing the extra tabs in TestMode_TabControl
            TestMode_TabControl.Controls.Remove(DirectionalModeTabPage)
            TestMode_TabControl.Controls.Remove(BmldModeTabPage)

            'Disabling the extra tabs in TestMode_TabControl
            'DirectionalModeTabPage.Enabled = False
            'BmldModeTabPage.Enabled = False

            'Setting the test mode to Custom1
            TestMode = TestModes.Custom1
        End If

    End Sub


    Private Sub SetLanguageStrings(ByVal Language As Utils.Languages)

        Select Case Language
            Case Utils.Languages.Swedish

                AboutToolStripMenuItem.Text = "Om"
                ParticipantLock_Button.Text = "Lås"
                PcScreen_RadioButton.Text = "PC-skärm"
                PcTouch_CheckBox.Text = "Touch"
                BtScreen_RadioButton.Text = "BT-skärm"
                ConnectBluetoothScreen_Button.Text = "Anslut BT-skärm"
                DisconnectBtScreen_Button.Text = "Koppla från BT-skärm"
                SelectTransducer_Label.Text = "Välj ljudgivare"
                TestDescription_Label.Text = "Testbeskrivning:"
                RandomSeed_Label.Text = "Slumptalsfrö (valfritt):"
                CompletedTests_Label.Text = "Genomförda test"
                ReferenceLevel_Label.Text = "Referensnivå (dB)"
                Preset_Label.Text = "Test"
                Situation_Label.Text = "Situation"
                LengthReduplications_Label.Text = "Repetitioner"
                Testparadigm_Label.Text = "Testläge"
                PNR_Label.Text = "PNR (dB)"
                CorrectCount_Label.Text = "Antal rätt"
                ProportionCorrect_Label.Text = "Andel rätt"
                TestLength_Label.Text = "Testlängd"
                KeyBoardShortcut_Label.Text = "Tangentkommandon att använda vid testkörning"
                Pause_Label.Text = "Pausa test = P"
                Resume_Label.Text = "Återuppta test = R"
                Stop_Label.Text = "Stoppa test = S"

                ScrollToPresented_CheckBox.Text = "Auto-skrolla till testord"
                PauseOnNextNonTrial_CheckBox.Text = "Pausa innan nästa icke-test-ord"

                NoSimulationString = "Ingen simulering"

            Case Else

                'English is default
                AboutToolStripMenuItem.Text = "About"
                ParticipantLock_Button.Text = "Lock"
                PcScreen_RadioButton.Text = "PC screen"
                PcTouch_CheckBox.Text = "Touch"
                BtScreen_RadioButton.Text = "BT screen"
                ConnectBluetoothScreen_Button.Text = "Connect to BT screen"
                DisconnectBtScreen_Button.Text = "Disconnect BT screen"
                SelectTransducer_Label.Text = "Select sound transducer"
                TestDescription_Label.Text = "Test description:"
                RandomSeed_Label.Text = "Random seed (optional):"
                CompletedTests_Label.Text = "Completed tests"
                ReferenceLevel_Label.Text = "Reference level (dB)"
                Preset_Label.Text = "Test"
                Situation_Label.Text = "Situation"
                LengthReduplications_Label.Text = "Repetitions"
                Testparadigm_Label.Text = "Test mode"
                PNR_Label.Text = "PNR (dB)"
                CorrectCount_Label.Text = "Number correct"
                ProportionCorrect_Label.Text = "Percent correct"
                TestLength_Label.Text = "Test length"
                KeyBoardShortcut_Label.Text = "Keyboard commands to use during testing"
                Pause_Label.Text = "Pause test = P"
                Resume_Label.Text = "Resume test = R"
                Stop_Label.Text = "Stop test = S"

                ScrollToPresented_CheckBox.Text = "Auto scroll to test word"
                PauseOnNextNonTrial_CheckBox.Text = "Pause on next non-test trial"

                NoSimulationString = "No simulation"

        End Select


    End Sub

    Private Sub StartSoundPlayer()

        'Selects the wave format for use (doing it this way means that the wave format MUST be the same in all available MediaSets)
        Dim TempWaveformat = SpeechMaterial.GetWavefileFormat(AvailableMediaSets(0))
        OstfBase.SoundPlayer.ChangePlayerSettings(, TempWaveformat.SampleRate, TempWaveformat.BitDepth, TempWaveformat.Encoding,, , Audio.PortAudioVB.OverlappingSoundPlayer.SoundDirections.PlaybackOnly, False, False)

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

        DirectionalSimulationSet_ComboBox.Items.Clear()
        SimulatedDistance_ComboBox.Items.Clear()
        DirectionalSimulationSet_C1_ComboBox.Items.Clear()

        SelectedTransducer = Transducer_ComboBox.SelectedItem

        If SelectedTransducer.CanPlay = True Then
            '(At this stage the sound player will be started, if not already done.)
            OstfBase.SoundPlayer.ChangePlayerSettings(SelectedTransducer.ParentAudioApiSettings,,, , 0.4, SelectedTransducer.Mixer,, True, True)

            'Adding available DirectionalSimulationSets
            Dim TempWaveformat = SpeechMaterial.GetWavefileFormat(AvailableMediaSets(0))
            Dim AvailableSets = DirectionalSimulator.GetAvailableDirectionalSimulationSets(SelectedTransducer, TempWaveformat.SampleRate)
            AvailableSets.Insert(0, NoSimulationString)
            For Each Item In AvailableSets
                DirectionalSimulationSet_ComboBox.Items.Add(Item)
                DirectionalSimulationSet_C1_ComboBox.Items.Add(Item)
            Next
            DirectionalSimulationSet_ComboBox.SelectedIndex = 0
            DirectionalSimulationSet_C1_ComboBox.SelectedIndex = 0

        Else
            MsgBox("Unable to start the player using the selected transducer (probably the selected output device doesn't have enough output channels?)!", MsgBoxStyle.Exclamation, "Sound player failure")
        End If

        If CurrentParticipantID <> "" Then
            Test_TableLayoutPanel.Enabled = True
        End If

        TryCreateSipTestMeasurement()

    End Sub


    Private Sub TestMode_TabControl_Selected(sender As Object, e As TabControlEventArgs) Handles TestMode_TabControl.Selected

        If DisableExtraTestModes = True Then Exit Sub

        If e.TabPage.Name = "BmldModeTabPage" Then
            TestMode = TestModes.BMLD
        ElseIf e.TabPage.Name = "DirectionalModeTabPage" Then
            TestMode = TestModes.Directional
        ElseIf e.TabPage.Name = "CustomMode1_TabPage" Then
            TestMode = TestModes.Custom1
        Else
            Throw New Exception("Unknown tabpage name. This is surely a bug!")
        End If

        TryCreateSipTestMeasurement()

    End Sub


    Private Sub DirectionalSimulationSet_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DirectionalSimulationSet_ComboBox.SelectedIndexChanged

        SimulatedDistance_ComboBox.Items.Clear()
        SimulatedDistance_ComboBox.ResetText()

        Dim SelectedItem = DirectionalSimulationSet_ComboBox.SelectedItem
        If SelectedItem IsNot Nothing Then

            If SelectedItem = NoSimulationString Then
                DirectionalSimulator.ClearSelectedDirectionalSimulationSet()
                SelectedSoundPropagationType = SoundPropagationTypes.PointSpeakers
            Else
                Dim TempWaveformat = SpeechMaterial.GetWavefileFormat(AvailableMediaSets(0))
                SelectedSoundPropagationType = SoundPropagationTypes.SimulatedSoundField
                If DirectionalSimulator.TrySetSelectedDirectionalSimulationSet(SelectedItem, SelectedTransducer, TempWaveformat.SampleRate) = False Then
                    'Well this should not happen...
                    DirectionalSimulator.ClearSelectedDirectionalSimulationSet()
                    SelectedSoundPropagationType = SoundPropagationTypes.PointSpeakers
                End If
            End If

            'Adding available simulation distances
            Dim AvailableDistances = DirectionalSimulator.GetAvailableDirectionalSimulationSetDistances(SelectedItem)
            For Each Distance In AvailableDistances
                SimulatedDistance_ComboBox.Items.Add(Distance)
            Next
            If SimulatedDistance_ComboBox.Items.Count > 0 Then SimulatedDistance_ComboBox.SelectedIndex = 0

        Else
            DirectionalSimulator.ClearSelectedDirectionalSimulationSet()
        End If

        TryCreateSipTestMeasurement()

    End Sub

    Private Sub DirectionalSimulationSet_C1_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DirectionalSimulationSet_C1_ComboBox.SelectedIndexChanged

        Dim SelectedItem = DirectionalSimulationSet_C1_ComboBox.SelectedItem
        If SelectedItem IsNot Nothing Then

            If SelectedItem = NoSimulationString Then
                DirectionalSimulator.ClearSelectedDirectionalSimulationSet()
            Else

                Dim TempWaveformat = SpeechMaterial.GetWavefileFormat(AvailableMediaSets(0))
                If DirectionalSimulator.TrySetSelectedDirectionalSimulationSet(SelectedItem, SelectedTransducer, TempWaveformat.SampleRate) = False Then
                    'Well this shold not happen...
                    DirectionalSimulator.ClearSelectedDirectionalSimulationSet()
                End If

            End If

        Else
            DirectionalSimulator.ClearSelectedDirectionalSimulationSet()
        End If

        TryCreateSipTestMeasurement()

    End Sub

    Private Sub Custom_SNC_TextBox_KeyUp(sender As Object, e As KeyEventArgs) Handles Custom_SNC_TextBox.KeyUp

        If e.KeyValue = Keys.Enter Then
            TryCreateSipTestMeasurement()
        End If

    End Sub

    Private Sub Custom_SNC_TextBox_KeyUp(sender As Object, e As EventArgs) Handles Custom_SNC_TextBox.LostFocus, Custom_SNC_TextBox.KeyUp

        TryCreateSipTestMeasurement()

    End Sub


    Private Sub SimulatedDistance_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles SimulatedDistance_ComboBox.SelectedIndexChanged

        TryCreateSipTestMeasurement()

    End Sub


    Private Sub KeyDetection(sender As Object, e As KeyEventArgs) Handles PcParticipantForm.KeyUp, MyBase.KeyUp

        'Use this method to tigger actions by pressing a keyboard key during active testing, when PcScreen is used, and mouse is therefore used by the testee.
        'MsgBox("Key pressed: " & e.KeyData)

        'Triggering these functions only if a test is started, and then also notes that the event is handled
        If TestIsStarted = True Then

            Select Case e.KeyData
                Case Keys.P
                    'For pause
                    PauseTesting()

                Case Keys.R
                    'For resume
                    ResumeTesting()

                Case Keys.S
                    'For stop
                    StopTest()

            End Select

            e.Handled = True

        End If

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



    Private Sub SelectReferenceLevel(sender As Object, e As EventArgs) Handles ReferenceLevelComboBox.SelectedIndexChanged

        'Stores the selected reference level
        SelectedReferenceLevel = ReferenceLevelComboBox.SelectedItem

        TryCreateSipTestMeasurement()

    End Sub


    Private Sub SelectPreset(sender As Object, e As EventArgs) Handles PresetComboBox.SelectedIndexChanged

        'Stores the selected preset
        SelectedPresetName = PresetComboBox.SelectedItem

        TryCreateSipTestMeasurement()

    End Sub


    Private Sub SelectTestLength(sender As Object, e As EventArgs) Handles TestLengthComboBox.SelectedIndexChanged

        'Stores the selected length
        SelectedLengthReduplications = TestLengthComboBox.SelectedItem

        TryCreateSipTestMeasurement()

    End Sub


    Private Sub SimultaneousNoisesCount_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles SimultaneousMaskersCount_ComboBox.SelectedIndexChanged

        'Stores the number of simultaneous maskers
        If SimultaneousMaskersCount_ComboBox.SelectedItem IsNot Nothing Then
            NumberOfSimultaneousMaskers = SimultaneousMaskersCount_ComboBox.SelectedItem
        Else
            NumberOfSimultaneousMaskers = Nothing
        End If

        TryCreateSipTestMeasurement()

    End Sub


    Private Sub Testparadigm_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Testparadigm_ComboBox.SelectedIndexChanged

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

            Case Testparadigm.Quick, Testparadigm.FlexibleLocations

                InterTrialInterval = 0.1
                ResponseAlternativeDelay = 0.5
                PretestSoundDuration = 2
                MinimumStimulusOnsetTime = 0.02
                MaximumStimulusOnsetTime = 0.1
                TrialSoundMaxDuration = 7 ' TODO: Optimize by shortening this time
                UseVisualQue = True
                UseBackgroundSpeech = False
                MaximumResponseTime = 3
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
                'Temporarily overriding the BackgroundNonspeechRealisticLevel 
                'SelectedMediaSet.BackgroundNonspeechRealisticLevel = 65

            Case Testparadigm.Directional2

                InterTrialInterval = 1
                ResponseAlternativeDelay = 0.5
                PretestSoundDuration = 5
                MinimumStimulusOnsetTime = 0.5 ' Earlier, when this variable directed the test words instead of maskers its value was 1.5. Having maskers of 3 seconds with the test word centralized, should be approximately the same as 0.3.
                MaximumStimulusOnsetTime = 0.5 ' Earlier, when this variable directed the test words instead of maskers its value was 2
                TrialSoundMaxDuration = 5 ' TODO: Optimize by shortening this time
                UseVisualQue = False
                UseBackgroundSpeech = False
                MaximumResponseTime = 4
                ShowProgressIndication = True
                'Temporarily overriding the BackgroundNonspeechRealisticLevel 
                'SelectedMediaSet.BackgroundNonspeechRealisticLevel = 65


        End Select

        TryCreateSipTestMeasurement()

    End Sub

    Private Function CheckRequiredInputSettings() As Boolean

        Try

            'Resetting the test description box
            TestDescriptionTextBox.Text = ""

            'Resetting the planned trial test length text
            PlannedTestLength_TextBox.Text = ""

            If CurrentParticipantID Is Nothing Then Return False
            If SelectedReferenceLevel.HasValue = False Then Return False
            If SelectedPresetName = "" Then Return False
            If SelectedLengthReduplications.HasValue = False Then Return False

            'Getting the selected media sets
            SelectedMediaSets.Clear()
            For Each Control In Situations_FlowLayoutPanel.Controls
                Dim CurrentControl As CheckBox = TryCast(Control, CheckBox)
                If CurrentControl IsNot Nothing Then
                    If CurrentControl.Checked = True Then
                        SelectedMediaSets.Add(AvailableMediaSets.GetMediaSet(CurrentControl.Text))
                    End If
                End If
            Next
            If SelectedMediaSets.Count = 0 Then Return False

            'Getting the selected PNRs
            SelectedPNRs.Clear()
            For Each Control In PNRs_FlowLayoutPanel.Controls
                Dim CurrentControl As CheckBox = TryCast(Control, CheckBox)
                If CurrentControl IsNot Nothing Then
                    If CurrentControl.Checked = True Then
                        SelectedPNRs.Add(CurrentControl.Text)
                    End If
                End If
            Next
            If SelectedPNRs.Count = 0 Then Return False


            If TestMode = TestModes.Directional Then
                If NumberOfSimultaneousMaskers.HasValue = False Then Return False
            End If

        Catch ex As Exception
            ShowMessageBox("An unexpected error occurred.")
            Return False
        End Try

        Return True

    End Function

    Private Sub TryCreateSipTestMeasurement()

        Try

            If CheckRequiredInputSettings() = False Then Exit Sub


            'Creates a new test 
            CurrentSipTestMeasurement = New SipMeasurement(CurrentParticipantID, SpeechMaterial.ParentTestSpecification)
            CurrentSipTestMeasurement.TestProcedure.LengthReduplications = SelectedLengthReduplications
            CurrentSipTestMeasurement.TestProcedure.TestParadigm = SelectedTestparadigm

            'Stores whether to export tets trial sounds
            CurrentSipTestMeasurement.ExportTrialSoundFiles = ExportTrialSounds_CheckBox.Checked

            Select Case TestMode
                Case TestModes.Directional

                    If SelectedTestparadigm = Testparadigm.FlexibleLocations Then

                        If SimulatedDistance_ComboBox.SelectedItem Is Nothing Then Exit Sub

                        Dim LocalSpeechAzimuths As New List(Of Double)
                        Dim LocalSpeechElevations As New List(Of Double)
                        Dim LocalSpeechDistances As New List(Of Double)
                        For Each Control In SpeechAzimuth_FlowLayoutPanel.Controls
                            Dim CurrentControl As CheckBox = TryCast(Control, CheckBox)
                            If CurrentControl IsNot Nothing Then
                                If CurrentControl.Checked = True Then
                                    LocalSpeechAzimuths.Add(CurrentControl.Text)
                                    LocalSpeechElevations.Add(0)
                                    LocalSpeechDistances.Add(SimulatedDistance_ComboBox.SelectedItem)
                                End If
                            End If
                        Next
                        CurrentSipTestMeasurement.TestProcedure.SetTargetStimulusLocations(SelectedTestparadigm, LocalSpeechAzimuths, LocalSpeechElevations, LocalSpeechDistances)

                        Dim LocalMaskerAzimuths As New List(Of Double)
                        Dim LocalMaskerElevations As New List(Of Double)
                        Dim LocalMaskerDistances As New List(Of Double)
                        For Each Control In MaskerAzimuth_FlowLayoutPanel.Controls
                            Dim CurrentControl As CheckBox = TryCast(Control, CheckBox)
                            If CurrentControl IsNot Nothing Then
                                If CurrentControl.Checked = True Then
                                    LocalMaskerAzimuths.Add(CurrentControl.Text)
                                    LocalMaskerElevations.Add(0)
                                    LocalMaskerDistances.Add(SimulatedDistance_ComboBox.SelectedItem)
                                End If
                            End If
                        Next
                        CurrentSipTestMeasurement.TestProcedure.SetMaskerLocations(SelectedTestparadigm, LocalMaskerAzimuths, LocalMaskerElevations, LocalMaskerDistances)

                        Dim LocalBackgroundAzimuths As New List(Of Double)
                        Dim LocalBackgroundElevations As New List(Of Double)
                        Dim LocalBackgroundDistances As New List(Of Double)
                        For Each Control In BackgroundAzimuth_FlowLayoutPanel.Controls
                            Dim CurrentControl As CheckBox = TryCast(Control, CheckBox)
                            If CurrentControl IsNot Nothing Then
                                If CurrentControl.Checked = True Then
                                    LocalBackgroundAzimuths.Add(CurrentControl.Text)
                                    LocalBackgroundElevations.Add(0)
                                    LocalBackgroundDistances.Add(SimulatedDistance_ComboBox.SelectedItem)
                                End If
                            End If
                        Next
                        CurrentSipTestMeasurement.TestProcedure.SetBackgroundLocations(SelectedTestparadigm, LocalBackgroundAzimuths, LocalBackgroundElevations, LocalBackgroundDistances)

                    End If

                    'Checking if enough maskers where selected
                    If NumberOfSimultaneousMaskers > CurrentSipTestMeasurement.TestProcedure.MaskerLocations(SelectedTestparadigm).Count Then
                        MsgBox("Select more masker locations of fewer maskers!", MsgBoxStyle.Information, "Not enough masker locations selected!")
                        Exit Sub
                    End If

                    'Setting up test trials to run
                    PlanDirectionalTestTrials(CurrentSipTestMeasurement, SelectedReferenceLevel, SelectedPresetName, SelectedMediaSets, SelectedPNRs, NumberOfSimultaneousMaskers, SelectedSoundPropagationType, RandomSeed_IntegerParsingTextBox.Value)

                Case TestModes.BMLD

                    'Getting BMLD type

                    If BmldSignalMode_ComboBox.SelectedItem Is Nothing Then Exit Sub
                    If BmldNoiseMode_ComboBox.SelectedItem Is Nothing Then Exit Sub

                    'BmldTargetModeStrings: "R", "L", "0", "π" ' For Right, Left, Zero, Pi,
                    'BmldNoiseModeStrings: "R", "L", "0", "π", "U" ' For Right, Left, Zero, Pi, Uncorrelated (i.e. different noises binaurally)

                    Dim BmldSignalMode As BmldModes
                    Select Case BmldSignalMode_ComboBox.SelectedItem
                        Case "R"
                            BmldSignalMode = BmldModes.RightOnly
                        Case "L"
                            BmldSignalMode = BmldModes.LeftOnly
                        Case "0"
                            BmldSignalMode = BmldModes.BinauralSamePhase
                        Case "π"
                            BmldSignalMode = BmldModes.BinauralPhaseInverted
                        Case Else
                            Throw New NotImplementedException("Unknown BMLD signal mode")
                    End Select

                    Dim BmldNoiseMode As BmldModes
                    Select Case BmldNoiseMode_ComboBox.SelectedItem
                        Case "R"
                            BmldNoiseMode = BmldModes.RightOnly
                        Case "L"
                            BmldNoiseMode = BmldModes.LeftOnly
                        Case "0"
                            BmldNoiseMode = BmldModes.BinauralSamePhase
                        Case "π"
                            BmldNoiseMode = BmldModes.BinauralPhaseInverted
                        Case "U"
                            BmldNoiseMode = BmldModes.BinauralUncorrelated
                        Case Else
                            Throw New NotImplementedException("Unknown BMLD noise mode")
                    End Select

                    PlanBmldTestTrials(CurrentSipTestMeasurement, SelectedReferenceLevel, SelectedPresetName, SelectedMediaSets, SelectedPNRs, BmldSignalMode, BmldNoiseMode, RandomSeed_IntegerParsingTextBox.Value)

                Case TestModes.Custom1

                    'Read input from textbox

                    Dim BlockTypes As New List(Of Tuple(Of Object, Object, String))

                    Dim InputLines = Custom_SNC_TextBox.Lines

                    For LineIndex = 0 To InputLines.Length - 1

                        Dim Line As String = InputLines(LineIndex)

                        If Line.Trim = "" Then Continue For
                        If Line.Trim.StartsWith("//") Then Continue For

                        Dim DataPart = Line.Trim.Split({"//"}, StringSplitOptions.None)(0).Trim
                        Dim DataSplit = DataPart.Split("|")

                        If DataSplit.Length <> 2 Then
                            ShowMessageBox("Invalid signal-noise-characterization on line " & LineIndex + 1 & ": " & Line)
                            Exit Sub
                        End If

                        Dim SignalPart As String = DataSplit(0).Trim
                        Dim NoisePart As String = DataSplit(1).Trim

                        Dim SignalItem As Object
                        Dim NoiseItem As Object

                        'Parsing signal
                        Select Case SignalPart
                            Case "R"
                                SignalItem = BmldModes.RightOnly
                            Case "L"
                                SignalItem = BmldModes.LeftOnly
                            Case "Z"
                                SignalItem = BmldModes.BinauralSamePhase
                            Case "P"
                                SignalItem = BmldModes.BinauralPhaseInverted
                            Case "U"
                                ShowMessageBox("U (uncorrelated) is not a valid characterization of the signal.")
                                Exit Sub
                            Case Else
                                'It should be numeric
                                Dim ParsedValue As Double
                                If Double.TryParse(SignalPart.Replace(",", "."), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, ParsedValue) = False Then
                                    ShowMessageBox("Invalid signal-noise-characterization on line " & LineIndex + 1 & ": " & Line & " Unable to parse the expression " & SignalPart & " as a numeric value.")
                                    Exit Sub
                                End If
                                SignalItem = ParsedValue
                        End Select

                        'Parsing noise
                        Select Case NoisePart
                            Case "R"
                                NoiseItem = BmldModes.RightOnly
                            Case "L"
                                NoiseItem = BmldModes.LeftOnly
                            Case "Z"
                                NoiseItem = BmldModes.BinauralSamePhase
                            Case "P"
                                NoiseItem = BmldModes.BinauralPhaseInverted
                            Case "U"
                                NoiseItem = BmldModes.BinauralUncorrelated
                            Case Else
                                'It should be numeric
                                Dim ParsedValue As Double
                                If Double.TryParse(NoisePart.Replace(",", "."), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, ParsedValue) = False Then
                                    ShowMessageBox("Invalid signal-noise-characterization on line " & LineIndex + 1 & ": " & Line & " Unable to parse the expression " & NoisePart & " as a numeric value.")
                                    Exit Sub
                                End If
                                NoiseItem = ParsedValue

                        End Select

                        If SignalItem.GetType <> NoiseItem.GetType Then
                            ShowMessageBox("Invalid signal-noise-characterization on line " & LineIndex + 1 & ": " & Line & " Directional azimuths cannot be combined with BMLD modes.")
                            Exit Sub
                        End If

                        BlockTypes.Add(New Tuple(Of Object, Object, String)(SignalItem, NoiseItem, DataPart))

                    Next

                    If BlockTypes.Count = 0 Then Exit Sub

                    If PlanCustom1Trials(CurrentSipTestMeasurement, SelectedReferenceLevel, SelectedPresetName, SelectedMediaSets, SelectedPNRs, BlockTypes, 2, RandomSeed_IntegerParsingTextBox.Value) = False Then
                        Exit Sub
                    End If

            End Select

            'Checks to see if a simulation set is required
            If SelectedSoundPropagationType = SoundPropagationTypes.SimulatedSoundField And DirectionalSimulator.SelectedDirectionalSimulationSetName = "" Then
                ShowMessageBox("No directional simulation set selected!")
                Exit Sub
            End If

            If CurrentSipTestMeasurement.HasSimulatedSoundFieldTrials = True And DirectionalSimulator.SelectedDirectionalSimulationSetName = "" Then
                ShowMessageBox("The measurement requires a directional simulation set to be selected!")
                Exit Sub
            End If

            'Displayes the planned test length
            PlannedTestLength_TextBox.Text = CurrentSipTestMeasurement.PlannedTrials.Count + CurrentSipTestMeasurement.ObservedTrials.Count

            'TODO: Calling GetTargetAzimuths only to ensure that the Actual Azimuths needed for presentation in the TestTrialTable exist. This should probably be done in some other way... (Only applies to the Directional3 and Directional5 Testparadigms)
            Select Case SelectedTestparadigm
                Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
                    CurrentSipTestMeasurement.GetTargetAzimuths()
            End Select

            UpdateTestTrialTable(ScrollToPresented_CheckBox.Checked)
            UpdateTestProgress()

            'Initiates the test
            TestDescriptionTextBox.Focus()

        Catch ex As Exception
            ShowMessageBox("The following error occurred: " & ex.ToString)
        End Try

    End Sub


    Private Shared Sub PlanDirectionalTestTrials(ByRef SipTestMeasurement As SipMeasurement, ByVal ReferenceLevel As Double, ByVal PresetName As String,
                                      ByVal SelectedMediaSets As List(Of MediaSet), ByVal SelectedPNRs As List(Of Double), ByVal NumberOfSimultaneousMaskers As Integer,
                                                 ByVal SoundPropagationType As SoundPropagationTypes, Optional ByVal RandomSeed As Integer? = Nothing)

        'Creating a new random if seed is supplied
        If RandomSeed.HasValue Then SipTestMeasurement.Randomizer = New Random(RandomSeed)

        'Getting the preset
        Dim Preset = SipTestMeasurement.ParentTestSpecification.SpeechMaterial.Presets(PresetName)

        'Clearing any trials that may have been planned by a previous call
        SipTestMeasurement.ClearTrials()

        'Getting the sound source locations
        Dim CurrentTargetLocations = SipTestMeasurement.TestProcedure.TargetStimulusLocations(SipTestMeasurement.TestProcedure.TestParadigm)
        Dim MaskerLocations = SipTestMeasurement.TestProcedure.MaskerLocations(SipTestMeasurement.TestProcedure.TestParadigm)
        Dim BackgroundLocations = SipTestMeasurement.TestProcedure.BackgroundLocations(SipTestMeasurement.TestProcedure.TestParadigm)


        For Each PresetComponent In Preset
            For Each MediaSet In SelectedMediaSets
                For Each PNR In SelectedPNRs
                    For Each TargetLocation In CurrentTargetLocations

                        'Drawing two random MaskerLocations
                        Dim CurrentMaskerLocations As New List(Of Audio.PortAudioVB.DuplexMixer.SoundSourceLocation)
                        Dim SelectedMaskerIndices As New List(Of Integer)
                        SelectedMaskerIndices.AddRange(Utils.SampleWithoutReplacement(NumberOfSimultaneousMaskers, 0, MaskerLocations.Length, SipTestMeasurement.Randomizer))
                        For Each RandomIndex In SelectedMaskerIndices
                            CurrentMaskerLocations.Add(MaskerLocations(RandomIndex))
                        Next

                        For Repetition = 1 To SipTestMeasurement.TestProcedure.LengthReduplications

                            Dim NewTestUnit = New SiPTestUnit(SipTestMeasurement)

                            Dim TestWords = PresetComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
                            NewTestUnit.SpeechMaterialComponents.AddRange(TestWords)

                            For c = 0 To TestWords.Count - 1
                                'TODO/NB: The following line uses only a single TargetLocation, even though several could in principle be set
                                Dim NewTrial As New SipTrial(NewTestUnit, TestWords(c), MediaSet, SoundPropagationType, {TargetLocation}, CurrentMaskerLocations.ToArray, BackgroundLocations, NewTestUnit.ParentMeasurement.Randomizer)
                                NewTrial.SetLevels(ReferenceLevel, PNR)
                                NewTestUnit.PlannedTrials.Add(NewTrial)
                            Next

                            'Adding from the selected media set
                            SipTestMeasurement.TestUnits.Add(NewTestUnit)

                        Next
                    Next
                Next
            Next
        Next

        'Adding the trials SipTestMeasurement (from which they can be drawn during testing)
        For Each Unit In SipTestMeasurement.TestUnits
            For Each Trial In Unit.PlannedTrials
                SipTestMeasurement.PlannedTrials.Add(Trial)
            Next
        Next

        'Randomizing the order
        If SipTestMeasurement.TestProcedure.RandomizeOrder = True Then
            Dim RandomList As New List(Of SipTrial)
            Do Until SipTestMeasurement.PlannedTrials.Count = 0
                Dim RandomIndex As Integer = SipTestMeasurement.Randomizer.Next(0, SipTestMeasurement.PlannedTrials.Count)
                RandomList.Add(SipTestMeasurement.PlannedTrials(RandomIndex))
                SipTestMeasurement.PlannedTrials.RemoveAt(RandomIndex)
            Loop
            SipTestMeasurement.PlannedTrials = RandomList
        End If

    End Sub



    Private Shared Sub PlanBmldTestTrials(ByRef SipTestMeasurement As SipMeasurement, ByVal ReferenceLevel As Double, ByVal PresetName As String,
                                      ByVal SelectedMediaSets As List(Of MediaSet), ByVal SelectedPNRs As List(Of Double), ByVal SignalMode As BmldModes, ByVal NoiseMode As BmldModes,
                                      Optional ByVal RandomSeed As Integer? = Nothing)

        'Creating a new random if seed is supplied
        If RandomSeed.HasValue Then SipTestMeasurement.Randomizer = New Random(RandomSeed)

        'Getting the preset
        Dim Preset = SipTestMeasurement.ParentTestSpecification.SpeechMaterial.Presets(PresetName)

        'Clearing any trials that may have been planned by a previous call
        SipTestMeasurement.ClearTrials()


        For Each PresetComponent In Preset
            For Each MediaSet In SelectedMediaSets
                For Each PNR In SelectedPNRs

                    For Repetition = 1 To SipTestMeasurement.TestProcedure.LengthReduplications

                        Dim NewTestUnit = New SiPTestUnit(SipTestMeasurement)

                        Dim TestWords = PresetComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
                        NewTestUnit.SpeechMaterialComponents.AddRange(TestWords)

                        For c = 0 To TestWords.Count - 1
                            Dim NewTrial As New SipTrial(NewTestUnit, TestWords(c), MediaSet, SoundPropagationTypes.PointSpeakers, SignalMode, NoiseMode, NewTestUnit.ParentMeasurement.Randomizer)
                            NewTrial.SetLevels(ReferenceLevel, PNR)
                            NewTestUnit.PlannedTrials.Add(NewTrial)
                        Next

                        'Adding from the selected media set
                        SipTestMeasurement.TestUnits.Add(NewTestUnit)

                    Next
                Next
            Next
        Next

        'Adding the trials SipTestMeasurement (from which they can be drawn during testing)
        For Each Unit In SipTestMeasurement.TestUnits
            For Each Trial In Unit.PlannedTrials
                SipTestMeasurement.PlannedTrials.Add(Trial)
            Next
        Next

        'Randomizing the order
        If SipTestMeasurement.TestProcedure.RandomizeOrder = True Then
            Dim RandomList As New List(Of SipTrial)
            Do Until SipTestMeasurement.PlannedTrials.Count = 0
                Dim RandomIndex As Integer = SipTestMeasurement.Randomizer.Next(0, SipTestMeasurement.PlannedTrials.Count)
                RandomList.Add(SipTestMeasurement.PlannedTrials(RandomIndex))
                SipTestMeasurement.PlannedTrials.RemoveAt(RandomIndex)
            Loop
            SipTestMeasurement.PlannedTrials = RandomList
        End If

    End Sub



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="SipTestMeasurement"></param>
    ''' <param name="ReferenceLevel"></param>
    ''' <param name="PresetName"></param>
    ''' <param name="SelectedMediaSets"></param>
    ''' <param name="SelectedPNRs"></param>
    ''' <param name="BlockTypes">Block types should contain tuples of two objects and one string, where the first object indicate the signal, the second object the noise, and the string is a descriptive block name. The object can be either a double representing an sound source azimuth or a string representing a BMLD mode. </param>
    ''' <param name="RandomSeed"></param>
    Private Shared Function PlanCustom1Trials(ByRef SipTestMeasurement As SipMeasurement, ByVal ReferenceLevel As Double, ByVal PresetName As String,
                                      ByVal SelectedMediaSets As List(Of MediaSet), ByVal SelectedPNRs As List(Of Double), ByVal BlockTypes As List(Of Tuple(Of Object, Object, String)),
                                              ByVal NumberOfIndicatorTrials As Integer, Optional ByVal RandomSeed As Integer? = Nothing) As Boolean

        'Creating a new random if seed is supplied
        If RandomSeed.HasValue Then SipTestMeasurement.Randomizer = New Random(RandomSeed)

        'Getting the preset
        Dim Preset = SipTestMeasurement.ParentTestSpecification.SpeechMaterial.Presets(PresetName)

        'Creating a collection of test words (i.e. NonTestMaterial) that do not occur in any of the test words groups included in the Preset
        Dim NonTestMaterial As New List(Of SpeechMaterialComponent)
        Dim PresetTestWords As New SortedSet(Of String)
        For Each TWG In Preset
            For Each TestWord In TWG.ChildComponents
                PresetTestWords.Add(TestWord.PrimaryStringRepresentation)
            Next
        Next
        Dim WholeMaterial = SipTestMeasurement.ParentTestSpecification.SpeechMaterial.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.List)
        For Each TWG In WholeMaterial
            For Each TestWord In TWG.ChildComponents
                If PresetTestWords.Contains(TestWord.PrimaryStringRepresentation) = False Then
                    NonTestMaterial.Add(TestWord)
                End If
            Next
        Next

        'Clearing any trials that may have been planned by a previous call
        SipTestMeasurement.ClearTrials()


        'Randomizing the order of blocks
        Dim RandomizedBlockTypes As New List(Of Tuple(Of Object, Object, String))
        Dim RandomIndices = Utils.SampleWithoutReplacement(BlockTypes.Count, 0, BlockTypes.Count, SipTestMeasurement.Randomizer)
        For i = 0 To RandomIndices.Length - 1
            RandomizedBlockTypes.Add(BlockTypes(RandomIndices(i)))
        Next
        BlockTypes = RandomizedBlockTypes

        For Each BlockType In BlockTypes

            'Creating and adding a test unit for each block
            Dim NewTestUnit = New SiPTestUnit(SipTestMeasurement, BlockType.Item3)
            SipTestMeasurement.TestUnits.Add(NewTestUnit)

            'Checking that both signal and noise have the same type
            If BlockType.Item1.GetType <> BlockType.Item2.GetType Then
                MsgBox("The following incorrect specification of blocks was detected: " & BlockType.Item1.ToString & " " & BlockType.Item2.ToString, MsgBoxStyle.Exclamation, "SiP-test")
                SipTestMeasurement.ClearTrials()
                Return False
            End If

            'Getting the type of block
            Dim CurrentBlockIsBMLD As Boolean = False
            If TypeOf (BlockType.Item1) Is BmldModes Then
                CurrentBlockIsBMLD = True
            End If

            Dim TargetStimulusLocations() As SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation
            Dim MaskerLocations() As SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation
            Dim BackgroundLocations() As SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation
            Dim SignalMode As BmldModes
            Dim NoiseMode As BmldModes

            If CurrentBlockIsBMLD = False Then

                'Parsing the signal and masker locations
                Dim SignalLocation As Double = BlockType.Item1
                Dim MaskerLocation As Double = BlockType.Item2

                'Directional stimulation
                TargetStimulusLocations = {New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = SignalLocation, .Elevation = 0, .Distance = 1}}
                MaskerLocations = {New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = MaskerLocation, .Elevation = 0, .Distance = 1}}

                'Adding a number of background sounds
                BackgroundLocations = {
                    New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 60, .Elevation = 0, .Distance = 1},
                    New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 150, .Elevation = 0, .Distance = 1},
                    New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = -150, .Elevation = 0, .Distance = 1},
                    New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = -60, .Elevation = 0, .Distance = 1}}

            Else

                'BMLD stimulation, storing the current mode
                SignalMode = BlockType.Item1
                NoiseMode = BlockType.Item2
            End If

            'Creating test trials
            For Each PresetComponent In Preset

                'Adding SMCs to the unit
                Dim TestWords = PresetComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
                NewTestUnit.SpeechMaterialComponents.AddRange(TestWords)

                For c = 0 To TestWords.Count - 1

                    For Each MediaSet In SelectedMediaSets
                        For Each PNR In SelectedPNRs
                            For Repetition = 1 To SipTestMeasurement.TestProcedure.LengthReduplications

                                Dim NewTrial As SipTrial

                                If CurrentBlockIsBMLD = False Then
                                    NewTrial = New SipTrial(NewTestUnit, TestWords(c), MediaSet, SoundPropagationTypes.SimulatedSoundField, TargetStimulusLocations, MaskerLocations, BackgroundLocations, NewTestUnit.ParentMeasurement.Randomizer)

                                    'Adding non-presented trials with contrasting alternatives
                                    NewTrial.PseudoTrials = New List(Of SipTrial)
                                    Dim ContrastingAlternatives = TestWords(c).GetSiblingsExcludingSelf
                                    For Each ContrastingAlternative In ContrastingAlternatives
                                        Dim NonpresentedTrial = New SipTrial(NewTestUnit, ContrastingAlternative, MediaSet, SoundPropagationTypes.SimulatedSoundField, TargetStimulusLocations, MaskerLocations, BackgroundLocations, NewTestUnit.ParentMeasurement.Randomizer)
                                        NonpresentedTrial.IsTestTrial = False
                                        NewTrial.PseudoTrials.Add(NonpresentedTrial)
                                    Next

                                Else
                                    NewTrial = New SipTrial(NewTestUnit, TestWords(c), MediaSet, SoundPropagationTypes.PointSpeakers, SignalMode, NoiseMode, NewTestUnit.ParentMeasurement.Randomizer)

                                    'Adding non-presented trials with contrasting alternatives
                                    NewTrial.PseudoTrials = New List(Of SipTrial)
                                    Dim ContrastingAlternatives = TestWords(c).GetSiblingsExcludingSelf
                                    For Each ContrastingAlternative In ContrastingAlternatives
                                        Dim NonpresentedTrial = New SipTrial(NewTestUnit, ContrastingAlternative, MediaSet, SoundPropagationTypes.PointSpeakers, SignalMode, NoiseMode, NewTestUnit.ParentMeasurement.Randomizer)
                                        NonpresentedTrial.IsTestTrial = False
                                        NewTrial.PseudoTrials.Add(NonpresentedTrial)
                                    Next

                                End If

                                NewTrial.SetLevels(ReferenceLevel, PNR)
                                NewTestUnit.PlannedTrials.Add(NewTrial)
                            Next

                        Next
                    Next
                Next
            Next

            'Randomizing the order of test trials within the block
            If SipTestMeasurement.TestProcedure.RandomizeOrder = True Then
                Dim RandomList As New List(Of SipTrial)
                Do Until NewTestUnit.PlannedTrials.Count = 0
                    Dim RandomIndex As Integer = SipTestMeasurement.Randomizer.Next(0, NewTestUnit.PlannedTrials.Count)
                    RandomList.Add(NewTestUnit.PlannedTrials(RandomIndex))
                    NewTestUnit.PlannedTrials.RemoveAt(RandomIndex)
                Loop
                NewTestUnit.PlannedTrials = RandomList
            End If

            'Inserting indicator trials initially in the block
            'Checking that we have enough nonused SCMs
            If NonTestMaterial.Count < NumberOfIndicatorTrials Then
                MsgBox("There are not enough (" & NonTestMaterial.Count & ") un-used test words to create " & NumberOfIndicatorTrials & " indicator trials.", MsgBoxStyle.Exclamation, "SiP-test")
                Return False
            End If

            'Picking random SMCs for the indicator trials
            Dim IndicatorTrialSMCs As New List(Of SpeechMaterialComponent)
            Dim RandomIndicatorTrialSMCIndices = Utils.SampleWithoutReplacement(NumberOfIndicatorTrials, 0, NonTestMaterial.Count, SipTestMeasurement.Randomizer)
            For i = 0 To RandomIndicatorTrialSMCIndices.Length - 1
                IndicatorTrialSMCs.Add(NonTestMaterial(RandomIndicatorTrialSMCIndices(i)))
            Next

            For Each SMC In IndicatorTrialSMCs

                'Picking one word from each test word group
                Dim RandomMediaSetIndex = Utils.SampleWithoutReplacement(1, 0, SelectedMediaSets.Count, SipTestMeasurement.Randomizer)
                Dim IndicatorTrialPNR = SelectedPNRs.Max

                Dim NewTrial As SipTrial

                'Creating indicator trials
                If CurrentBlockIsBMLD = False Then
                    NewTrial = New SipTrial(NewTestUnit, SMC, SelectedMediaSets(RandomMediaSetIndex(0)), SoundPropagationTypes.SimulatedSoundField, TargetStimulusLocations, MaskerLocations, BackgroundLocations, NewTestUnit.ParentMeasurement.Randomizer)
                Else
                    NewTrial = New SipTrial(NewTestUnit, SMC, SelectedMediaSets(RandomMediaSetIndex(0)), SoundPropagationTypes.PointSpeakers, SignalMode, NoiseMode, NewTestUnit.ParentMeasurement.Randomizer)
                End If

                'Notes that this is not a test trial
                NewTrial.IsTestTrial = False
                NewTrial.SetLevels(ReferenceLevel, IndicatorTrialPNR)
                NewTestUnit.PlannedTrials.Insert(0, NewTrial)

            Next

        Next

        'Adding the trials SipTestMeasurement (from which they can be drawn during testing)
        For Each Unit In SipTestMeasurement.TestUnits
            For Each Trial In Unit.PlannedTrials
                SipTestMeasurement.PlannedTrials.Add(Trial)
            Next
        Next

        Return True

    End Function


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
            MsgBox("Unable To play sound Using the selected transducer!", MsgBoxStyle.Exclamation, "Sound player Error")
            Exit Sub
        End If

        If CurrentSipTestMeasurement Is Nothing Then
            ShowMessageBox("Inget test är laddat.", "SiP-test")
            Exit Sub
        End If

        'Sets the measurement datetime
        CurrentSipTestMeasurement.MeasurementDateTime = DateTime.Now

        'Getting NeededTargetAzimuths for the Directional2, Directional3 and Directional5 Testparadigms
        Dim NeededTargetAzimuths As List(Of Double) = Nothing
        Select Case SelectedTestparadigm
            Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
                NeededTargetAzimuths = CurrentSipTestMeasurement.GetTargetAzimuths()
        End Select

        'Creating a ParticipantForm
        Select Case CurrentScreenType
            Case ScreenType.Pc

                'Creating a new participant form (and ParticipantControl) if none exist
                If CheckPcScreen() = False Then
                    Select Case SelectedTestparadigm
                        Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
                            PcParticipantForm = New PcTesteeForm(PcTesteeForm.TaskType.ForcedChoiceDirection, NeededTargetAzimuths)

                        Case Else
                            PcParticipantForm = New PcTesteeForm(PcTesteeForm.TaskType.ForcedChoice)
                    End Select
                End If


                Select Case SelectedTestparadigm
                    Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5

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

                        If PcParticipantForm.CurrentTaskType <> PcTesteeForm.TaskType.ForcedChoice Then
                            PcParticipantForm.UpdateType(PcTesteeForm.TaskType.ForcedChoice)
                        End If

                End Select

                ParticipantControl = PcParticipantForm.ParticipantControl

                'Shows the ParticipantForm
                PcParticipantForm.Show()

            Case ScreenType.Bluetooth

                Select Case SelectedTestparadigm
                    Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
                        ShowMessageBox("Bluetooth screen Is Not yet implemented For the Directional3 And Directional5 test paradigms. Use the PC screen instead.", "SiP-test")
                        Exit Sub
                End Select

                ParticipantControl = MyBtTesteeControl
                'Select Case GuiLanguage
                '    Case Utils.Constants.Languages.Swedish
                '        ParticipantControl.ShowMessage("Testet börjar strax")
                '    Case Utils.Constants.Languages.English
                '        ParticipantControl.ShowMessage("The test is about to start")
                'End Select


                'MyBtTesteeControl.StartNewTestSession() ' Inactivates the startbutton on the ParticipantControl by commenting out MyBtTesteeControl.StartNewTestSession()

        End Select

        Select Case GuiLanguage
            Case Utils.Constants.Languages.Swedish
                ParticipantControl.ShowMessage("Testet börjar strax")
            Case Utils.Constants.Languages.English
                ParticipantControl.ShowMessage("The test is about to start")
        End Select


        Start_AudioButton.Enabled = True

    End Sub


    Private Sub TryStartTest(sender As Object, e As EventArgs) Handles Start_AudioButton.Click ', ParticipantControl.StartedByTestee ' Started by Testee is commented out, we propbably shouldn't use it.

        If TestIsStarted = False Then

            If SelectedTestDescription = "" Then
                ShowMessageBox("Please provide a test description (such As 'test 1, with HA')!", "SiP-test")
                Exit Sub
            End If

            'Creates a new randomizer before each test start
            Dim Seed As Integer? = RandomSeed_IntegerParsingTextBox.Value
            If Seed.HasValue Then
                SipMeasurementRandomizer = New Random(Seed)
            Else
                SipMeasurementRandomizer = New Random
            End If

            'Storing the test description
            CurrentSipTestMeasurement.Description = SelectedTestDescription

            'Setting the default export path
            CurrentSipTestMeasurement.SetDefaultExportPath()

            'Things seemed to be in order,
            'Starting the test

            If CurrentScreenType = ScreenType.Pc Then
                If CheckPcScreen() = True Then
                    PcParticipantForm.LockCursorToForm()
                    PcParticipantForm.SetResponseMode(PcResponseMode)
                Else
                    ShowMessageBox("Did you close the participant screen? Please try again, without closing it.")
                    TryEnableTestStart()
                    Exit Sub
                End If
            End If

            TogglePlayButton(False)
            Stop_AudioButton.Enabled = True

            LockSettingsPanels()

            If sender Is ParticipantControl Then
                Utils.SendInfoToLog("Test started by administrator")
            ElseIf sender Is ParticipantControl Then
                Utils.SendInfoToLog("Test started by testee")
            End If

            TestIsStarted = True


            InitiateTestByPlayingSound()

        Else
            'Test is started
            If TestIsPaused = True Then
                ResumeTesting()
            Else
                PauseTesting()
            End If
        End If

    End Sub

    Private Function CheckPcScreen() As Boolean

        If CurrentScreenType = ScreenType.Pc Then
            'Locks the cursor to the form
            If PcParticipantForm IsNot Nothing Then
                If PcParticipantForm.IsDisposed = True Then
                    Return False
                End If
            Else
                Return False
            End If
        End If
        Return True
    End Function

    Private Sub StopTest() Handles Stop_AudioButton.Click
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

        UpdateTestProgress()
        'Updates the progress bar
        If ShowProgressIndication = True Then
            ParticipantControl.UpdateTestFormProgressbar(CurrentSipTestMeasurement.ObservedTrials.Count, CurrentSipTestMeasurement.ObservedTrials.Count + CurrentSipTestMeasurement.PlannedTrials.Count)
        End If


        'Removes the start button
        ParticipantControl.ResetTestItemPanel()

        'Cretaing a context sound without any test stimulus, that runs for approx TestSetup.PretestSoundDuration seconds, using audio from the first selected MediaSet
        Dim TestSound As Audio.Sound = CreateInitialSound(SelectedMediaSets(0))

        'Plays sound
        SoundPlayer.SwapOutputSounds(TestSound)

        'Setting the interval to the first test stimulus using NewTrialTimer.Interval (N.B. The NewTrialTimer.Interval value has to be reset at the first tick, as the deafault value is overridden here)
        StartTrialTimer.Interval = Math.Max(1, PretestSoundDuration * 1000)

        'Premixing the first 10 sounds 
        CurrentSipTestMeasurement.PreMixTestTrialSoundsOnNewTread(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, 10)

        'Preparing and launching the next trial
        PrepareAndLaunchTrial_ThreadSafe()

    End Sub


    Public Function CreateInitialSound(ByRef SelectedMediaSet As MediaSet, Optional ByVal Duration As Double? = Nothing) As Audio.Sound

        Try

            'Setting up the SiP-trial sound mix
            Dim MixStopWatch As New Stopwatch
            MixStopWatch.Start()

            'Sets a List of SoundSceneItem in which to put the sounds to mix
            Dim ItemList = New List(Of SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSceneItem)

            Dim SoundWaveFormat As Audio.Formats.WaveFormat = Nothing

            'Getting a background non-speech sound
            Dim BackgroundNonSpeech_Sound As Audio.Sound = SpeechMaterial.GetBackgroundNonspeechSound(SelectedMediaSet, 0)

            'Stores the sample rate and the wave format
            Dim CurrentSampleRate As Integer = BackgroundNonSpeech_Sound.WaveFormat.SampleRate
            SoundWaveFormat = BackgroundNonSpeech_Sound.WaveFormat

            'Sets a total pretest sound length
            Dim TrialSoundLength As Integer
            If Duration.HasValue Then
                TrialSoundLength = Duration * SoundWaveFormat.SampleRate
            Else
                TrialSoundLength = (PretestSoundDuration + 4) * CurrentSampleRate 'Adds 4 seconds to allow for potential delay caused by the mixing time of the first test trial sounds
            End If


            'Copies copies random sections of the background non-speech sound into two sounds
            Dim Background1 = BackgroundNonSpeech_Sound.CopySection(1, SipMeasurementRandomizer.Next(0, BackgroundNonSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2), TrialSoundLength)
            Dim Background2 = BackgroundNonSpeech_Sound.CopySection(1, SipMeasurementRandomizer.Next(0, BackgroundNonSpeech_Sound.WaveData.SampleData(1).Length - TrialSoundLength - 2), TrialSoundLength)

            'Sets up fading specifications for the background signals
            Dim FadeSpecs_Background = New List(Of SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications)
            FadeSpecs_Background.Add(New SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications(Nothing, 0, 0, CurrentSampleRate * 1))
            FadeSpecs_Background.Add(New SpeechTestFramework.Audio.DSP.Transformations.FadeSpecifications(0, Nothing, -CurrentSampleRate * 0.01))

            'Adds the background (non-speech) signals, with fade, duck and location specifications
            Dim LevelGroup As Integer = 1 ' The level group value is used to set the added sound level of items sharing the same (arbitrary) LevelGroup value to the indicated sound level. (Thus, the sounds with the same LevelGroup value are measured together.)
            ItemList.Add(New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSceneItem(Background1, 1, SelectedMediaSet.BackgroundNonspeechRealisticLevel, LevelGroup,
                                                                                              New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = -30},
                                                                                              Audio.PortAudioVB.DuplexMixer.SoundSceneItem.SoundSceneItemRoles.BackgroundSpeech, 0,,,, FadeSpecs_Background))
            ItemList.Add(New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSceneItem(Background2, 1, SelectedMediaSet.BackgroundNonspeechRealisticLevel, LevelGroup,
                                                                                              New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 30},
                                                                                              Audio.PortAudioVB.DuplexMixer.SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech, 0,,,, FadeSpecs_Background))
            LevelGroup += 1

            MixStopWatch.Stop()
            If LogToConsole = True Then Console.WriteLine("Prepared sounds in " & MixStopWatch.ElapsedMilliseconds & " ms.")
            MixStopWatch.Restart()

            'Creating the mix by calling CreateSoundScene of the current Mixer
            Dim MixedInitialSound As Audio.Sound = SelectedTransducer.Mixer.CreateSoundScene(ItemList, SelectedSoundPropagationType)

            If LogToConsole = True Then Console.WriteLine("Mixed sound in " & MixStopWatch.ElapsedMilliseconds & " ms.")

            'TODO: Here we can simulate and/or compensate for hearing loss:
            'SimulateHearingLoss,
            'CompensateHearingLoss

            Return MixedInitialSound

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
            Return Nothing
        End Try

    End Function


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
        UpdateTestTrialTable(ScrollToPresented_CheckBox.Checked)
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
            Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
                'TODO: the following line only uses the first of possible target stimulus locations
                CorrectResponse = CurrentSipTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling") & vbTab & CurrentSipTrial.TargetStimulusLocations(0).ActualLocation.HorizontalAzimuth
            Case Else
                CorrectResponse = CurrentSipTrial.SpeechMaterialComponent.GetCategoricalVariableValue("Spelling")
        End Select

        'Collects the response alternatives
        TestWordAlternatives = New List(Of Tuple(Of String, Audio.PortAudioVB.DuplexMixer.SoundSourceLocation))
        Dim TempList As New List(Of SpeechMaterialComponent)
        CurrentSipTrial.SpeechMaterialComponent.IsContrastingComponent(,, TempList)
        For Each ContrastingComponent In TempList
            'TODO: the following line only uses the first of each possible contrasting response alternative stimulus locations
            TestWordAlternatives.Add(New Tuple(Of String, Audio.PortAudioVB.DuplexMixer.SoundSourceLocation)(ContrastingComponent.GetCategoricalVariableValue("Spelling"), CurrentSipTrial.TargetStimulusLocations(0).ActualLocation))
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


            If (CurrentSipTestMeasurement.ObservedTrials.Count + 3) Mod 10 = 0 Then
                'Premixing the next 10 sounds, starting three trials before the next is needed 
                CurrentSipTestMeasurement.PreMixTestTrialSoundsOnNewTread(SelectedTransducer, MinimumStimulusOnsetTime, MaximumStimulusOnsetTime, SipMeasurementRandomizer, TrialSoundMaxDuration, UseBackgroundSpeech, 10)
            End If

            'Waiting for the background thread to finish mixing
            Dim WaitPeriods As Integer = 0
            While CurrentSipTrial.TestTrialSound Is Nothing
                WaitPeriods += 1
                Threading.Thread.Sleep(100)
                If LogToConsole = True Then Console.WriteLine("Waiting for sound to mix: " & WaitPeriods * 100 & " ms")
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

            'Launches the trial if the start timer has ticked, without launching the trial (which happens when the sound preparation was not completed at the tick)
            If StartTrialTimerHasTicked = True Then
                If CurrentTrialIsLaunched = False Then

                    'Launching the trial
                    LaunchTrial(CurrentTestSound)

                End If
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
            SoundPlayer.SwapOutputSounds(TestSound)

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

        ResponseTime = DateTime.Now - ResponseAlternativesPresentationTime

        'Sends the result and response time on
        StoreResult(ResponseString, ResponseTime)

    End Sub

    Private Sub StoreResult(ByVal ResponseString As String, ByVal ResponseTime As TimeSpan)

        If Me.InvokeRequired = True Then
            Dim d As New ResultDelegate(AddressOf StoreResult_Unsafe)
            Me.Invoke(d, New Object() {ResponseString, ResponseTime})
        Else
            StoreResult_Unsafe(ResponseString, ResponseTime)
        End If

    End Sub

    Private Sub StoreResult_Unsafe(ByVal ResponseString As String, ByVal ResponseTime As TimeSpan)

        'Converting the response time to seconds
        Dim CurrentResponseTime As Integer = ResponseTime.TotalMilliseconds

        'Stores the ResponseMoment
        CurrentSipTrial.ResponseMoment = DateTime.Now

        'Corrects the response
        If ResponseString = CorrectResponse Then
            CurrentSipTrial.Result = SipTest.PossibleResults.Correct

        ElseIf ResponseString = "" Then
            CurrentSipTrial.Result = SipTest.PossibleResults.Missing

        Else
            'Determining the erraneous word (among the presented alternatives)
            CurrentSipTrial.Result = SipTest.PossibleResults.Incorrect
        End If


        'Getting the screen position of the test word
        'And getting the response screen position
        Dim TestWordScreenPosition As Integer = -1
        Dim ResponseScreenPosition As Integer? = Nothing

        Select Case SelectedTestparadigm
            Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
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

        'Moves the trials to from planned to observed trials so that it doesn't get presented again
        CurrentSipTestMeasurement.MoveTrialToHistory(CurrentSipTrial, False)

        'Exports the result
        If CurrentSipTestMeasurement.ObservedTrials.Count = 1 Then
            CurrentSipTrial.ExportTrialResult(True, Not CurrentSipTestMeasurement.ExportTrialSoundFiles)
        Else
            CurrentSipTrial.ExportTrialResult(False, Not CurrentSipTestMeasurement.ExportTrialSoundFiles)
        End If

        'Removes sounds from the current test trial (as its no longer needed)
        CurrentSipTrial.RemoveSounds()

        'Updates the progress bar
        If ShowProgressIndication = True Then
            ParticipantControl.UpdateTestFormProgressbar(CurrentSipTestMeasurement.ObservedTrials.Count, CurrentSipTestMeasurement.ObservedTrials.Count + CurrentSipTestMeasurement.PlannedTrials.Count)
        End If

        'Checking if need to pause
        'Gets the next stimulus
        Dim NextTrial = CurrentSipTestMeasurement.GetNextTrial()

        'Pauses test if admin checked the PauseOnNextNonTrial_CheckBox, and if the trial is not a test trial
        If NextTrial IsNot Nothing Then
            If NextTrial.IsTestTrial = False Then
                If PauseOnNextNonTrial_CheckBox.Checked = True Then

                    'Pauses testing
                    PauseTesting()

                    'Deselects the PauseOnNextNonTrial_CheckBox
                    PauseOnNextNonTrial_CheckBox.Checked = False

                    'Updates the GUI table
                    UpdateTestTrialTable(ScrollToPresented_CheckBox.Checked)
                    UpdateTestProgress()

                    Exit Sub

                End If
            End If
        End If

        'Starting the next trial
        PrepareAndLaunchTrial_ThreadSafe()

    End Sub




    Private Sub FinalizeTesting()

        StopAllTimers()

        Try

            Stop_AudioButton.Enabled = False

            ParticipantControl.ResetTestItemPanel()

            Select Case GuiLanguage
                Case Utils.Constants.Languages.Swedish
                    ParticipantControl.ShowMessage("Testet är klart!")
                Case Utils.Constants.Languages.English
                    ParticipantControl.ShowMessage("The test is completed!")
            End Select

            'ParticipantControl.ShowMessage(GUIDictionary.SubTestIsCompleted)

            'Saving results to the log folder
            SoundPlayer.SwapOutputSounds(Nothing)

            'Sleeps during the fade out phase
            Threading.Thread.Sleep(SoundPlayer.GetOverlapDuration * 3000)

            'Summarizes the result
            'CurrentSipTestMeasurement.SummarizeTestResults()

            MeasurementHistory.Measurements.Add(CurrentSipTestMeasurement)

            'Display results
            PopulateTestHistoryTables()

            'TODO: Should we auto-export data here, or let the user be responsible for saving the test results?
            'MeasurementHistory.SaveToFile(Path.Combine(Utils.logFilePath, "AutoLoggedResults"))
            'MeasurementHistory.SaveToFile(Path.Combine(Utils.logFilePath, "AutoLoggedResults"))

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
        TryCreateSipTestMeasurement()

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
        Select Case GuiLanguage
            Case Utils.Constants.Languages.Swedish
                ParticipantControl.ShowMessage("Testet är pausat!")
            Case Utils.Constants.Languages.English
                ParticipantControl.ShowMessage("Testing is paused!")
        End Select

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

    Private Sub UpdateTestTrialTable(Optional ByVal ScrollToTestWord As Boolean = True)

        Dim GetGuiTableData = CurrentSipTestMeasurement.GetGuiTableData()

        Dim TestWords() As String = GetGuiTableData.TestWords.ToArray
        Dim Responses() As String = GetGuiTableData.Responses.ToArray
        Dim ResultResponseTypes() As SipTest.PossibleResults = GetGuiTableData.ResponseType.ToArray
        Dim UpdateRow As Integer? = GetGuiTableData.UpdateRow
        Dim SelectionRow As Integer? = GetGuiTableData.SelectionRow
        Dim FirstRowToDisplayInScrollmode As Integer? = Nothing

        If ScrollToTestWord = True Then
            FirstRowToDisplayInScrollmode = GetGuiTableData.FirstRowToDisplayInScrollmode
        Else
            FirstRowToDisplayInScrollmode = TestTrialDataGridView.FirstDisplayedScrollingRowIndex
            If FirstRowToDisplayInScrollmode < 0 Then FirstRowToDisplayInScrollmode = Nothing
        End If

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
            If SelectionRow < TestTrialDataGridView.Rows.Count Then
                'Selects the first column of the SelectionRow
                TestTrialDataGridView.Rows(SelectionRow).Cells(0).Selected = True
            End If
        End If

        'Scrolls to the indicated FirstRowToDisplayInScrollmode
        If FirstRowToDisplayInScrollmode.HasValue Then
            TestTrialDataGridView.FirstDisplayedScrollingRowIndex = FirstRowToDisplayInScrollmode
        End If

    End Sub

    Private Sub CurrentSessionResults_DataGridView_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles CurrentSessionResults_DataGridView.CurrentCellDirtyStateChanged

    End Sub

    Private Sub SessionResults_DataGridView_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles CurrentSessionResults_DataGridView.CellValueChanged

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


    Private Sub ExportResultsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportResultsToolStripMenuItem.Click

        If CurrentParticipantID Is Nothing Then
            MsgBox("No participant selected!", MsgBoxStyle.Exclamation, "Exporting measurements")
        End If

        MeasurementHistory.SaveToFile()

        Me.ShowMessageBox("Finished exporting data!")

    End Sub


    ''' <summary>
    ''' This method can be called by the backend in order to display a message box message to the user.
    ''' </summary>
    ''' <param name="Message"></param>
    Private Sub ShowMessageBox(Message As String, Optional ByVal Title As String = "")

        If Title = "" Then
            Select Case GuiLanguage
                Case Utils.Constants.Languages.Swedish
                    Title = "SiP-testet"
                Case Else
                    Title = "SiP-test"
            End Select
        End If

        MsgBox(Message, MsgBoxStyle.Information, Title)

    End Sub

    Private Function ShowYesNoMessageBox(Question As String, Optional Title As String = "") As Boolean

        If Title = "" Then
            Select Case GuiLanguage
                Case Utils.Constants.Languages.Swedish
                    Title = "SiP-testet"
                Case Else
                    Title = "SiP-test"
            End Select
        End If

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

            If CheckPcScreen() = True Then
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

        If CheckPcScreen() = True Then
            PcParticipantForm.SetResponseMode(PcResponseMode)
        End If

    End Sub

    'Private Sub PcScreen_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles PcScreen_ComboBox.SelectedIndexChanged
    Private Sub SetPcScreen() Handles PcScreen_ComboBox.SelectedIndexChanged

        If CurrentScreenType = ScreenType.Pc Then

            'Creating a new participant form (and ParticipantControl) if none exist
            If CheckPcScreen() = False Then
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

            If MyBtTesteeControl.Initialize(Bt_UUID, Bt_PIN, GuiLanguage, "SiP-tablet") = False Then
                Failed = True
            End If

        Else
            If MyBtTesteeControl.BtTabletTalker.TrySendData() = False Then Failed = True
        End If

        If Failed = True Then
            Select Case GuiLanguage
                Case Utils.Constants.Languages.Swedish
                    ShowMessageBox("Ingen blåtandsenhet kunde anslutas, vänligen försök igen!")
                Case Utils.Constants.Languages.English
                    ShowMessageBox("No bluetooth unit could be connected, please try again!")
            End Select

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
            Select Case GuiLanguage
                Case Utils.Constants.Languages.Swedish
                    ShowMessageBox("Anslutningen till blåtandsskärmen har gått förlorad.")
                Case Utils.Constants.Languages.English
                    ShowMessageBox("The connected to the bluetooth screen has been lost.")
            End Select

            ' Calls DisconnectWirelessScreen to set correct enabled status of all controls
            DisconnectWirelessScreen()

        End If

    End Sub

    Private Sub SipTestGui_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        'Closing the PcResponseForm
        Try
            If CheckPcScreen() = True Then
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




    Private Sub AboutToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles BmldSignalMode_ComboBox.SelectedIndexChanged
        SetBmldString()
    End Sub

    Private Sub MaskerAndBackgroundPhase_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles BmldNoiseMode_ComboBox.SelectedIndexChanged
        SetBmldString()
    End Sub

    Private Sub SetBmldString()

        If BmldSignalMode_ComboBox.SelectedItem IsNot Nothing And BmldNoiseMode_ComboBox.SelectedItem IsNot Nothing Then

            BmldMode_RichTextBox.Clear()

            BmldMode_RichTextBox.Text = "S" & BmldSignalMode_ComboBox.SelectedItem & " N" & BmldNoiseMode_ComboBox.SelectedItem

            BmldMode_RichTextBox.Select(0, 1)
            BmldMode_RichTextBox.SelectionFont = New Font("Arial", 12, FontStyle.Italic)

            BmldMode_RichTextBox.Select(1, 1)
            BmldMode_RichTextBox.SelectionCharOffset = -6
            BmldMode_RichTextBox.SelectionFont = New Font("Arial", 7, FontStyle.Italic)

            BmldMode_RichTextBox.Select(3, 1)
            BmldMode_RichTextBox.SelectionFont = New Font("Arial", 12, FontStyle.Italic)

            BmldMode_RichTextBox.Select(4, 1)
            BmldMode_RichTextBox.SelectionCharOffset = -6
            BmldMode_RichTextBox.SelectionFont = New Font("Arial", 7, FontStyle.Italic)

            TryCreateSipTestMeasurement()

        End If

    End Sub

    Private Sub ScrollToPresented_CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles ScrollToPresented_CheckBox.CheckedChanged

        If ScrollToPresented_CheckBox.Checked = True Then
            If CurrentSipTestMeasurement IsNot Nothing Then
                UpdateTestTrialTable(ScrollToPresented_CheckBox.Checked)
            End If
        End If

    End Sub

End Class