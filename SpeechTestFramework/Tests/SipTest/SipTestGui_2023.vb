Imports SpeechTestFramework.SipTest
Imports SpeechTestFramework.WinFormControls
Imports System.Windows.Forms
Imports System.Drawing

Public Class SipTestGui_2023

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
    Private AvailableTargetAzimuths As New List(Of Double) From {-90, -60, -30, 0, 30, 60, 90}
    Private AvailableMaskerAzimuths As New List(Of Double) From {-150, -120, -90, -60, -30, 0, 30, 60, 90, 120, 150, 180}
    Private AvailableBackgroundAzimuths As New List(Of Double) From {-150, -120, -90, -60, -30, 0, 30, 60, 90, 120, 150, 180}
    Private AvailableLengthReduplications As New List(Of Integer) From {1, 2, 3, 4}
    Private AvailablePNRs As New List(Of Double) From {-15, -12, -10, -8, -6, -4, -2, 0, 2, 4, 6, 8, 10, 12, 15}
    Private BmldTargetModes As New List(Of String) From {"R", "L", "0", "π"} ' For Right, Left, Zero, Pi,
    Private BmldNoiseModes As New List(Of String) From {"R", "L", "0", "π", "U"} ' For Right, Left, Zero, Pi, Uncorrelated (i.e. different noises binaurally)
    Private AvailableSimultaneousNoisesCount As New List(Of Integer) From {1, 2, 3, 4, 5}
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
        ReferenceLevelComboBox.SelectedIndex = DefaultReferenceLevelIndex

        'Adding a default noise count 
        For Each NoiseCount In AvailableSimultaneousNoisesCount
            SimultaneousNoisesCount_ComboBox.Items.Add(NoiseCount)
        Next
        SimultaneousNoisesCount_ComboBox.SelectedIndex = SimultaneousNoisesCount_ComboBox.Items.Count - 1

        'Adding available preset names
        AvailablePresetsNames = SpeechMaterial.Presets.Keys.ToList
        PresetComboBox.Items.Clear()
        For Each Preset In AvailablePresetsNames
            PresetComboBox.Items.Add(Preset)
        Next
        'We don't select a default here...?? 

        'Adding available test situations
        For Each TestSituation In AvailableMediaSets
            Situations_FlowLayoutPanel.Controls.Add(New CheckBox With {.Text = TestSituation.MediaSetName})
        Next

        'Adding available test length reduplications
        TestLengthComboBox.Items.Clear()
        For Each TestLength In AvailableLengthReduplications
            TestLengthComboBox.Items.Add(TestLength)
        Next
        'We don't select a default here...?? 

        'Adding possible PNR values
        For Each PNR In AvailablePNRs
            PNRs_FlowLayoutPanel.Controls.Add(New CheckBox With {.Text = PNR})
        Next

        'Adding sound source azimuth values - target
        For Each Azimuth In AvailableTargetAzimuths
            SpeechAzimuth_FlowLayoutPanel.Controls.Add(New CheckBox With {.Text = Azimuth})
        Next

        'Adding sound source azimuth values - masker
        For Each Azimuth In AvailableMaskerAzimuths
            MaskerAzimuth_FlowLayoutPanel.Controls.Add(New CheckBox With {.Text = Azimuth})
        Next

        'Adding sound source azimuth values - background
        For Each Azimuth In AvailableBackgroundAzimuths
            BackgroundAzimuth_FlowLayoutPanel.Controls.Add(New CheckBox With {.Text = Azimuth})
        Next

        'Adding Target and Noise modes for BMLD
        For Each Value In BmldTargetModes
            BmldSignalMode_ComboBox.Items.Add(Value)
        Next

        For Each Value In BmldNoiseModes
            BmldNoiseMode_ComboBox.Items.Add(Value)
        Next

        SetLanguageStrings(GuiLanguage)

        StartSoundPlayer()

        'Adding values in Testparadigm_ComboBox
        Testparadigm_ComboBox.Items.Add(Testparadigm.FlexibleLocations)
        Testparadigm_ComboBox.Items.Add(Testparadigm.Quick)
        Testparadigm_ComboBox.Items.Add(Testparadigm.Slow)
        Testparadigm_ComboBox.Items.Add(Testparadigm.Directional2)
        Testparadigm_ComboBox.Items.Add(Testparadigm.Directional3)
        Testparadigm_ComboBox.Items.Add(Testparadigm.Directional5)
        Testparadigm_ComboBox.SelectedIndex = 0

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

        SelectedTransducer = Transducer_ComboBox.SelectedItem

        If SelectedTransducer.CanPlay = True Then
            '(At this stage the sound player will be started, if not already done.)
            OstfBase.SoundPlayer.ChangePlayerSettings(SelectedTransducer.ParentAudioApiSettings,,, , 0.4, SelectedTransducer.Mixer,, True, True)

            'Adding available DirectionalSimulationSets
            Dim TempWaveformat = SpeechMaterial.GetWavefileFormat(AvailableMediaSets(0))
            Dim AvailableSets = SelectedTransducer.GetAvailableDirectionalSimulationSets(TempWaveformat.SampleRate)
            For Each Item In AvailableSets
                DirectionalSimulationSet_ComboBox.Items.Add(Item)
            Next
            If DirectionalSimulationSet_ComboBox.Items.Count > 1 Then
                DirectionalSimulationSet_ComboBox.SelectedIndex = 0
            End If

        Else
            MsgBox("Unable to start the player using the selected transducer (probably the selected output device doesn't have enough output channels?)!", MsgBoxStyle.Exclamation, "Sound player failure")
        End If

        If CurrentParticipantID <> "" Then
            Test_TableLayoutPanel.Enabled = True
        End If

    End Sub

    Private Sub DirectionalSimulationSet_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DirectionalSimulationSet_ComboBox.SelectedIndexChanged

        Dim SelectedItem = DirectionalSimulationSet_ComboBox.SelectedItem
        If SelectedItem IsNot Nothing Then
            Dim TempWaveformat = SpeechMaterial.GetWavefileFormat(AvailableMediaSets(0))
            If SelectedTransducer.TrySetSelectedDirectionalSimulationSet(SelectedItem, TempWaveformat.SampleRate) = False Then
                'Well this shold not happen...
                SelectedTransducer.ClearSelectedDirectionalSimulationSet()
            End If
        Else
            SelectedTransducer.ClearSelectedDirectionalSimulationSet()
        End If

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


    Private Sub SimultaneousNoisesCount_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles SimultaneousNoisesCount_ComboBox.SelectedIndexChanged

        'Stores the number of simultaneous maskers
        If SimultaneousNoisesCount_ComboBox.SelectedItem IsNot Nothing Then
            NumberOfSimultaneousMaskers = SimultaneousNoisesCount_ComboBox.SelectedItem
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


    Private Sub TryCreateSipTestMeasurement()

        Try

            'Resetting the test description box
            TestDescriptionTextBox.Text = ""

            'Resetting the planned trial test length text
            PlannedTestLength_TextBox.Text = ""

            If CurrentParticipantID Is Nothing Then Exit Sub
            If SelectedReferenceLevel.HasValue = False Then Exit Sub
            If SelectedPresetName = "" Then Exit Sub
            If SelectedLengthReduplications.HasValue = False Then Exit Sub
            If NumberOfSimultaneousMaskers.HasValue = False Then Exit Sub

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
            If SelectedMediaSets.Count = 0 Then Exit Sub

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
            If SelectedPNRs.Count = 0 Then Exit Sub

            'Prepares the operation progress bar
            Test_TableLayoutPanel.Enabled = False

            'Creates a new test and updates the psychometric function diagram
            CurrentSipTestMeasurement = New SipMeasurement(CurrentParticipantID, SpeechMaterial.ParentTestSpecification)
            CurrentSipTestMeasurement.TestProcedure.LengthReduplications = SelectedLengthReduplications
            CurrentSipTestMeasurement.TestProcedure.TestParadigm = SelectedTestparadigm

            If SelectedTestparadigm = Testparadigm.FlexibleLocations Then

                Dim LocalSpeechLocations As New List(Of Double)
                For Each Control In SpeechAzimuth_FlowLayoutPanel.Controls
                    Dim CurrentControl As CheckBox = TryCast(Control, CheckBox)
                    If CurrentControl IsNot Nothing Then
                        If CurrentControl.Checked = True Then
                            LocalSpeechLocations.Add(CurrentControl.Text)
                        End If
                    End If
                Next
                CurrentSipTestMeasurement.TestProcedure.SetTargetStimulusLocations(SelectedTestparadigm, LocalSpeechLocations)

                Dim LocalMaskerLocations As New List(Of Double)
                For Each Control In MaskerAzimuth_FlowLayoutPanel.Controls
                    Dim CurrentControl As CheckBox = TryCast(Control, CheckBox)
                    If CurrentControl IsNot Nothing Then
                        If CurrentControl.Checked = True Then
                            LocalMaskerLocations.Add(CurrentControl.Text)
                        End If
                    End If
                Next
                CurrentSipTestMeasurement.TestProcedure.SetMaskerLocations(SelectedTestparadigm, LocalMaskerLocations)

                Dim LocalBackgroundLocations As New List(Of Double)
                For Each Control In BackgroundAzimuth_FlowLayoutPanel.Controls
                    Dim CurrentControl As CheckBox = TryCast(Control, CheckBox)
                    If CurrentControl IsNot Nothing Then
                        If CurrentControl.Checked = True Then
                            LocalBackgroundLocations.Add(CurrentControl.Text)
                        End If
                    End If
                Next
                CurrentSipTestMeasurement.TestProcedure.SetBackgroundLocations(SelectedTestparadigm, LocalBackgroundLocations)

            End If



            'Setting up test trials to run
            PlanTestTrials(CurrentSipTestMeasurement, SelectedReferenceLevel, SelectedPresetName, SelectedMediaSets, SelectedPNRs, NumberOfSimultaneousMaskers, RandomSeed_IntegerParsingTextBox.Value)

            Test_TableLayoutPanel.Enabled = True

            'Displayes the planned test length
            PlannedTestLength_TextBox.Text = CurrentSipTestMeasurement.PlannedTrials.Count + CurrentSipTestMeasurement.ObservedTrials.Count

            'TODO: Calling GetTargetAzimuths only to ensure that the Actual Azimuths needed for presentation in the TestTrialTable exist. This should probably be done in some other way... (Only applies to the Directional3 and Directional5 Testparadigms)
            Select Case SelectedTestparadigm
                Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
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


    Private Shared Sub PlanTestTrials(ByRef SipTestMeasurement As SipMeasurement, ByVal ReferenceLevel As Double, ByVal PresetName As String,
                                      ByVal SelectedMediaSets As List(Of MediaSet), ByVal SelectedPNRs As List(Of Double), ByVal NumberOfSimultaneousMaskers As Integer,
                                      Optional ByVal RandomSeed As Integer? = Nothing)

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

                            For c = 0 To NewTestUnit.SpeechMaterialComponents.Count - 1
                                Dim NewTrial As New SipTrial(NewTestUnit, NewTestUnit.SpeechMaterialComponents(c), MediaSet, TargetLocation, CurrentMaskerLocations.ToArray, BackgroundLocations, NewTestUnit.ParentMeasurement.Randomizer)
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
                If PcParticipantForm Is Nothing Then
                    Select Case SelectedTestparadigm
                        Case Testparadigm.Quick, Testparadigm.Slow
                            PcParticipantForm = New PcTesteeForm(PcTesteeForm.TaskType.ForcedChoice)

                        Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
                            PcParticipantForm = New PcTesteeForm(PcTesteeForm.TaskType.ForcedChoiceDirection, NeededTargetAzimuths)
                    End Select
                End If


                Select Case SelectedTestparadigm
                    Case Testparadigm.Quick, Testparadigm.Slow, Testparadigm.FlexibleLocations
                        If PcParticipantForm.CurrentTaskType <> PcTesteeForm.TaskType.ForcedChoice Then
                            PcParticipantForm.UpdateType(PcTesteeForm.TaskType.ForcedChoice)
                        End If

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
                        'This will be an other type, not yet implemented
                        Throw New NotImplementedException

                End Select

                ParticipantControl = PcParticipantForm.ParticipantControl

                'Shows the ParticipantForm
                PcParticipantForm.Show()

            Case ScreenType.Bluetooth

                Select Case SelectedTestparadigm
                    Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
                        ShowMessageBox("Bluetooth screen is not yet implemented for the Directional3 and Directional5 test paradigms. Use the PC screen instead.", "SiP-test")
                        Exit Sub
                End Select

                ParticipantControl = MyBtTesteeControl
                MyBtTesteeControl.StartNewTestSession()

        End Select

        Start_AudioButton.Enabled = True

    End Sub

    Private Sub TryStartTest(sender As Object, e As EventArgs) Handles ParticipantControl.StartedByTestee, Start_AudioButton.Click

        If TestIsStarted = False Then

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
            ItemList.Add(New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSceneItem(Background1, 1, SelectedMediaSet.BackgroundNonspeechRealisticLevel, LevelGroup, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = -30}, 0,,,, FadeSpecs_Background))
            ItemList.Add(New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSceneItem(Background2, 1, SelectedMediaSet.BackgroundNonspeechRealisticLevel, LevelGroup, New SpeechTestFramework.Audio.PortAudioVB.DuplexMixer.SoundSourceLocation With {.HorizontalAzimuth = 30}, 0,,,, FadeSpecs_Background))
            LevelGroup += 1

            MixStopWatch.Stop()
            If LogToConsole = True Then Console.WriteLine("Prepared sounds in " & MixStopWatch.ElapsedMilliseconds & " ms.")
            MixStopWatch.Restart()

            'Creating the mix by calling CreateSoundScene of the current Mixer
            Dim MixedInitialSound As Audio.Sound = SelectedTransducer.Mixer.CreateSoundScene(ItemList)

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
            Case Testparadigm.Directional2, Testparadigm.Directional3, Testparadigm.Directional5
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

        'Moves the trials to from planned to observed trials so thatr it doesn't get presented again
        CurrentSipTestMeasurement.MoveTrialToHistory(CurrentSipTrial)

        'Updates the progress bar
        If ShowProgressIndication = True Then
            ParticipantControl.UpdateTestFormProgressbar(CurrentSipTestMeasurement.ObservedTrials.Count, CurrentSipTestMeasurement.ObservedTrials.Count + CurrentSipTestMeasurement.PlannedTrials.Count)
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

            If MyBtTesteeControl.Initialize(Bt_UUID, Bt_PIN, GuiLanguage, "SiP-tablet") = False Then
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

        End If

    End Sub

    Private Sub StopTest(sender As Object, e As EventArgs) Handles Stop_AudioButton.Click

    End Sub

End Class