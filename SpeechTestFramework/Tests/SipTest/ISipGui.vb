Namespace SipTest

    Public Interface ISipGui

        ' Methods that will be called from the Sip-test ModuleBackend. These all need to be implemented in the graphical interface of the SiP-test.

        Property SipTestBackend As SipTest.IModuleBackend

        ''' <summary>
        ''' Should display the social-security number and name of the selected patient on the GUI and lock/hide the search-for-patient button (or equivalent).
        ''' </summary>
        ''' <param name="SocialSecurityNumber"></param>
        ''' <param name="FirstName"></param>
        ''' <param name="LastName"></param>
        Sub LockPatientDetails(ByVal SocialSecurityNumber As String, ByVal FirstName As String, ByVal LastName As String)

        ''' <summary>
        ''' Should populate a list displaying available audiograms (preferably sorted in date-time-order) to the user with the audiograms referenced in Audiograms, and optionally preselect the Audiogram stored at index SelectedIndex.
        ''' </summary>
        ''' <param name="Audiograms"></param>
        ''' <param name="SelectedIndex">A value of -1 should not select any audiogram.</param>
        Sub PopulateAudiogramList(ByRef Audiograms As List(Of AudiogramData), Optional ByVal SelectedIndex As Integer = -1)

        ''' <summary>
        ''' Should show a visual display of the audiogram data stored in Audiogram to the user.
        ''' </summary>
        ''' <param name="AudiogramData"></param>
        Sub DisplaySelectedAudiogram(ByRef AudiogramData As AudiogramData)

        ''' <summary>
        ''' Should populate a list displaying the available reference levels in AvailableReferenceLevels, and preselect the reference level stored at index SelectedIndex. 
        ''' </summary>
        ''' <param name="AvailableReferenceLevels"></param>
        ''' <param name="SelectedIndex"></param>
        Sub PopulateReferenceLevelList(ByVal AvailableReferenceLevels As List(Of Double), ByVal SelectedIndex As Integer)

        ''' <summary>
        ''' Should populate a list displaying the available hearing aid gain types (E.g No gain, Measured Gain, Fig6, etc.) in AvailableReferenceLevels, and preselect the gain type stored at index SelectedIndex. 
        ''' </summary>
        ''' <param name="AvailableGainTypes"></param>
        ''' <param name="SelectedIndex"></param>
        Sub PopulateHearingAidGainTypeList(ByVal AvailableGainTypes As List(Of HearingAidGainData.GainTypes), ByVal SelectedIndex As Integer)

        ''' <summary>
        ''' Should show a visual display the measured or target hearing-aid gain (or no gain) data stored in HearingAidGainData to the user.
        ''' </summary>
        ''' <param name="HearingAidGain"></param>
        Sub DisplayHearingAidGain(ByRef HearingAidGain As HearingAidGainData)

        ''' <summary>
        ''' Should populate a list displaying the available SiP-test presets in AvailablePresets, and preselect the preset type stored at index SelectedIndex. 
        ''' </summary>
        ''' <param name="AvailablePresets"></param>
        ''' <param name="SelectedIndex"></param>
        Sub PopulatePresetList(ByVal AvailablePresets As List(Of String), ByVal SelectedIndex As Integer)

        ''' <summary>
        ''' Should populate a list displaying the available SiP-test test situations in AvailableSituations, and preselect the test situation stored at index SelectedIndex. 
        ''' </summary>
        ''' <param name="AvailableSituations"></param>
        ''' <param name="SelectedIndex"></param>
        Sub PopulateTestSituationList(ByVal AvailableSituations As List(Of String), ByVal SelectedIndex As Integer?)

        ''' <summary>
        ''' Should populate a list displaying the available SiP-test lengths (in number of trials) AvailableTestLengths, and preselect the test length stored at index SelectedIndex. 
        ''' </summary>
        ''' <param name="AvailableTestLengths"></param>
        ''' <param name="SelectedIndex"></param>
        Sub PopulateTestLengthList(ByVal AvailableTestLengths As List(Of Integer), ByVal SelectedIndex As Integer)

        ''' <summary>
        ''' Should show a visual display of a psychometric curve showing the predicted score for different PNRs, along with the critical interval of each predicted score.
        ''' </summary>
        Sub DisplayPredictedPsychometricCurve(ByVal PNRs() As Single, ByVal PredictedScores() As Single, ByVal LowerCiLimits() As Single, ByVal UpperCiLimits() As Single)

        ''' <summary>
        ''' Should populate a list displaying the available PNR-values in AvailablePNRs, and preselect the PNR-value stored at index SelectedIndex. 
        ''' </summary>
        ''' <param name="AvailablePNRs"></param>
        ''' <param name="SelectedIndex"></param>
        Sub PopulatePnrList(ByVal AvailablePNRs As List(Of Double), ByVal SelectedIndex As Integer)

        ''' <summary>
        ''' Should clear any text in the test-name box
        ''' </summary>
        Sub ClearTestNameBox()

        ''' <summary>
        ''' Should disable editing of the test-name box
        ''' </summary>
        Sub LockTestNameBox()

        ''' <summary>
        ''' Should enable editing of the test-name box
        ''' </summary>
        Sub UnlockTestNameBox()

        ''' <summary>
        ''' Should enable the play button (and activate its enabled layout)
        ''' </summary>
        Sub SubEnablePlayButton()

        ''' <summary>
        ''' Should disable the play button (and activate its disabled layout)
        ''' </summary>
        Sub SubDisableStopButton()

        ''' <summary>
        ''' Should enable the stop button (and activate its enabled layout)
        ''' </summary>
        Sub SubEnableStopButton()

        ''' <summary>
        ''' Should disable the stop button (and activate its disabled layout)
        ''' </summary>
        Sub SubDisablePlayButton()

        ''' <summary>
        ''' Toggles the graphical layout of the play button between play and pause (current enabled/disabled status should be left unchanged).
        ''' </summary>
        ''' <param name="PlayMode">If set to true, the play button should show a play-layout, and if false it should show a pause-layout.</param>
        Sub TogglePlayButton(ByVal PlayMode As Boolean)

        ''' <summary>
        ''' Should update any control showing the progress of an active test.
        ''' </summary>
        ''' <param name="Max">The total number of test trials.</param>
        ''' <param name="Progress">The number of presented trials.</param>
        ''' <param name="Correct">The number of presented trials with a correct response.</param>
        ''' <param name="ProportionCorrect">A formatted string giving the percentage correct test trials.</param>
        Sub UpdateTestProgress(ByVal Max As Integer, ByVal Progress As Integer, ByVal Correct As Integer, ByVal ProportionCorrect As String)

        ''' <summary>
        ''' Should update a table or list containing all words in a SiP-test measurement, the speelling of the observed response, and the response type.
        ''' </summary>
        ''' <param name="TestWords">The spelling of all words in a SiP-test measurement.</param>
        ''' <param name="Responses">The spelling of the observed responses in a SiP-test measurement. (Should be the same length as TestWords, but contain empty strings for trials not yet presented.))</param>
        ''' <param name="ResultResponseTypes">The result of the observed responses including randomized results for missing responses. (Should be the same length as TestWords.)</param>
        ''' <param name="UpdateRow">If only a single trial should be updated (as in an active testing), UpdateRow can indicate which row is to be updated. The GUI can always ignore this argument and instead update the whole table on every call.</param>
        ''' <param name="SelectionRow">The index of the selected row. Can be used to signal which test word is currently presented. If set to Nothing, no row should be selected.</param>
        ''' <param name="FirstRowToDisplayInScrollmode">In scroll mode (when there are more test words than can fit into the visible part of the table) should indicate the first visible row.)</param>
        Sub UpdateTestTrialTable(ByVal TestWords() As String, ByVal Responses() As String, ByVal ResultResponseTypes() As SipTest.ResultResponseType,
                                 Optional ByVal UpdateRow As Integer? = Nothing, Optional SelectionRow As Integer? = Nothing, Optional FirstRowToDisplayInScrollmode As Integer? = Nothing)

        ''' <summary>
        ''' Should populate tables presenting the results of the SiP-test measurements in the current and previous sessions respectively. Each graphical object should be able to initialte an event that trigger a significance test between two selected tests by calling the function .
        ''' </summary>
        ''' <param name="TestHistoryListData"></param>
        Sub PopulateTestHistoryTables(ByRef TestHistoryListData As TestHistoryListData)

        ''' <summary>
        ''' Should update a significance-test result box with the current result string.
        ''' </summary>
        ''' <param name="Result"></param>
        Sub UpdateSignificanceTestResult(ByVal Result As String)

        ''' <summary>
        ''' Should update a status indicator for the bluetooth connection to the SiP-tablet, for example "green" if connection is working fine and "red" if some thing is wrong
        ''' </summary>
        ''' <param name="ConnectionOk"></param>
        Sub UpdateBluetoothConnectionStatusIndicator(ByVal ConnectionOk As Boolean)

        ''' <summary>
        ''' Should populate a list displaying bluetooth devices. 
        ''' </summary>
        ''' <param name="DeviceDescriptions"></param>
        Sub PopulateBluetoothDevicesList(ByVal DeviceDescriptions As List(Of String))

        ''' <summary>
        ''' Should enable selection of items in the BlueToothDevicesList
        ''' </summary>
        Sub EnableBluetoothDevicesList()

        ''' <summary>
        ''' Should disable selection of items in the BlueToothDevicesList
        ''' </summary>
        Sub DisableBluetoothDevicesList()

        ''' <summary>
        ''' Should enable a button that initiate a search for bluetooth devices.
        ''' </summary>
        Sub EnableBluetoothSearchButton()

        ''' <summary>
        ''' Should disable a button that initiate a search for bluetooth devices.
        ''' </summary>
        Sub DisableBluetoothSearchButton()

        ''' <summary>
        ''' Should enable a button used to initiate a connection to the bluetooth device selected in the BlueToothDevicesList
        ''' </summary>
        Sub EnableBluetoothDeviceSelectionButton()

        ''' <summary>
        ''' Should disable a button used to initiate a connection to the bluetooth device selected in the BlueToothDevicesList
        ''' </summary>
        Sub DisableBluetoothDeviceSelectionButton()

        ''' <summary>
        ''' Should update a status indicator for the SoundDevice connection, for example "green" if the connection is working fine and "red" if some thing is wrong
        ''' </summary>
        ''' <param name="ConnectionOk"></param>
        Sub UpdateSoundDeviceConnectionStatusIndicator(ByVal ConnectionOk As Boolean)

        ''' <summary>
        ''' Should populate a list displaying sound devices. 
        ''' </summary>
        ''' <param name="DeviceDescriptions"></param>
        Sub PopulateSoundDevicesList(ByVal DeviceDescriptions As List(Of String))

        ''' <summary>
        ''' Should enable selection of items in the SoundDevicesList
        ''' </summary>
        Sub EnableSoundDevicesList()

        ''' <summary>
        ''' Should disable selection of items in the SoundDevicesList
        ''' </summary>
        Sub DisableSoundDevicesList()

        ''' <summary>
        ''' Should enable a button that initiate a search for sound devices.
        ''' </summary>
        Sub EnableSoundDeviceSearchButton()

        ''' <summary>
        ''' Should disable a button that initiate a search for sound devices.
        ''' </summary>
        Sub DisableSoundDeviceSearchButton()

        ''' <summary>
        ''' Should enable a button used to initiate a connection to the SoundDevice selected in the SoundDevicesList
        ''' </summary>
        Sub EnableSoundDeviceSelectionButton()

        ''' <summary>
        ''' Should disable a button used to initiate a connection to the SoundDevice selected in the SoundDevicesList
        ''' </summary>
        Sub DisableSoundDeviceSelectionButton()

        ''' <summary>
        ''' Should enable a "Save"-button that initiates a process that saves the CurrentPatient Patient object to file using serialization.
        ''' </summary>
        Sub EnableSaveButton()

        ''' <summary>
        ''' Should disable a "Save"-button that initiates a process that saves the CurrentPatient Patient object to file using serialization.
        ''' </summary>
        Sub DisableSaveButton()

        ''' <summary>
        ''' Should enable an "Open"-button that initiates a process that opens a file containing a serialized SipTest.Patient into the CurrentPatient property of the ModuleBackend.
        ''' </summary>
        Sub EnableOpenButton()

        ''' <summary>
        ''' Should disable an "Open"-button that initiates a process that opens a file containing a serialized SipTest.Patient into the CurrentPatient property of the ModuleBackend.
        ''' </summary>
        Sub DisableOpenButton()

        ''' <summary>
        ''' Should enable an "Export"-button that initiates a process that exports the CurrentPatient Patient object to a standard file format.
        ''' </summary>
        Sub EnableExportButton()

        ''' <summary>
        ''' Should disable a "Export"-button that initiates a process that exports the CurrentPatient Patient object to a standard file format.
        ''' </summary>
        Sub DisableExportButton()

        ''' <summary>
        ''' Should open a dialog that asks the user for a file to open. Should return a full path, or an empty string if the user wants to cancel.
        ''' </summary>
        ''' <param name="Title"></param>
        ''' <param name="Extension"></param>
        ''' <returns></returns>
        Function GetOpenFileDialogResult(ByVal Title As String, Optional Extension As String = "")

        ''' <summary>
        ''' Should open a dialog that asks the for a name and location to save or export a file. Should return a full path, or an empty string if the user wants to cancel.
        ''' </summary>
        ''' <param name="Title"></param>
        ''' <param name="Extension"></param>
        ''' <returns></returns>
        Function GetSaveFileDialogResult(ByVal Title As String, Optional Extension As String = "") As String


        ''' <summary>
        ''' Should show a message to the user in a message box that temporarily locks the GUI until the user has clicked ok.
        ''' </summary>
        ''' <param name="Message"></param>
        ''' <param name="Title"></param>
        Sub ShowMessageBox(ByVal Message As String, Optional ByVal Title As String = "SiP-testet")

        ''' <summary>
        ''' Should show a message with a yes/no question to the user in a message box that temporarily locks the GUI until the user has responded. The response "No" should return False, and "Yes" should return True.
        ''' </summary>
        ''' <param name="Question"></param>
        ''' <param name="Title"></param>
        ''' <returns></returns>
        Function ShowYesNoMessageBox(ByVal Question As String, Optional ByVal Title As String = "SiP-testet") As Boolean


    End Interface


End Namespace