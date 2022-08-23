Namespace SipTest

    Public Interface IModuleBackend

        Sub InitializeModuleBackendComponents()

        ''' <summary>
        ''' A method that should search for a patient (in a database or equivalent), load the patient into the CurrentPatient property, lock the Gui SSNumber input box, and update available patient data in the Gui.
        ''' </summary>
        ''' <param name="SocialSecurityNumber"></param>
        Sub SearchPatient(ByVal SocialSecurityNumber As String)

        ''' <summary>
        ''' A method that should set the ModuleBackend.SelectedAudiogramData to SelectedAudiogramData and trigger a recalculation chain from the AudiogramData start point, including calls to update the relevant data presented in the Gui.
        ''' </summary>
        ''' <param name="SelectedAudiogramData"></param>
        Sub SelectAudiogram(ByRef SelectedAudiogramData As AudiogramData)

        ''' <summary>
        ''' A method that should set the ModuleBackend.SelectedReferenceLevel to SelectedReferenceLevel and trigger a recalculation chain from the ReferenceLevel start point, including calls to update the relevant data presented in the Gui.
        ''' </summary>
        ''' <param name="SelectedReferenceLevel"></param>
        Sub SelectReferenceLevel(ByVal SelectedReferenceLevel As Double)

        ''' <summary>
        ''' A method that should set the ModuleBackend.SelectedHearingAidGainType to SelectedHearingAidGainType and trigger a recalculation chain from the HearingAidGain start point, including calls to update the relevant data presented in the Gui.
        ''' </summary>
        ''' <param name="SelectedHearingAidGainType"></param>
        Sub SelectHearingAidGainType(ByVal SelectedHearingAidGainType As String)

        ''' <summary>
        ''' A method that should set the ModuleBackend.SelectedPreset to SelectedPreset and trigger a recalculation chain from the TestPreset start point, including calls to update the relevant data presented in the Gui.
        ''' </summary>
        ''' <param name="SelectedPreset"></param>
        Sub SelectPreset(ByVal SelectedPreset As SipTest.SipTestPresets)

        ''' <summary>
        ''' A method that should set the ModuleBackend.SelectedVoice to SelectedVoice and trigger a recalculation chain from the TestVoice start point, including calls to update the relevant data presented in the Gui.
        ''' </summary>
        ''' <param name="SelectedVoice"></param>
        Sub SelectVoice(ByVal SelectedVoice As String)

        ''' <summary>
        ''' A method that should set the ModuleBackend.SelectedTestLengthFactor to SelectedTestLength divided by the number of items in the selected preset and trigger a 
        ''' recalculation chain from the TestLength start point, including calls to update the relevant data presented in the Gui.
        ''' </summary>
        ''' <param name="SelectedTestLength"></param>
        Sub SelectTestLength(ByVal SelectedTestLength As Integer)

        ''' <summary>
        ''' A method that should set the ModuleBackend.SelectedPNR to SelectedPNR and trigger a recalculation chain from the PNR start point, including calls to update the relevant data presented in the Gui.
        ''' </summary>
        ''' <param name="SelectedPNR"></param>
        Sub SelectPNR(ByVal SelectedPNR As Double)


        ''' <summary>
        ''' A method that should handle the event triggered when the user clicks the start button (which is also the pause button)
        ''' </summary>
        Sub StartButtonPressed()

        ''' <summary>
        ''' A method that should handle the event triggered when the user clicks the stop button
        ''' </summary>
        Sub StopButtonPressed()


        ''' <summary>
        ''' A method that should perform a statistical analysis of the score difference between the SiP-testet refered to in ComparedMeasurementGuiDescriptions 
        ''' by their GuiDescription strings and send the result back to the graphical user interface using ISipGui.UpdateSignificanceTestResult.
        ''' </summary>
        ''' <param name="ComparedMeasurementGuiDescriptions"></param>
        Sub CompareTwoSipTestScores(ByRef ComparedMeasurementGuiDescriptions As List(Of String))

        ''' <summary>
        ''' A method that should search for all available bluetooth devices and display the results in the Gui using ISipGui.PopulateBluetoothDevicesList
        ''' </summary>
        Sub SearchForBluetoothDevices()

        ''' <summary>
        ''' A method that should initiate a bluetooth connection to the bluetouth device with indicated SelectedBluetoothDeviceDescription, 
        ''' and if the bluetooth device is successfully connected disable Gui controls for bluetooth connection using 
        ''' ISipGui.DisableBluetoothDevicesList, ISipGui.DisableBluetoothSearchButton and ISipGui.DisableBluetoothDeviceSelectionButton.
        ''' </summary>
        ''' <param name="SelectedBluetoothDeviceDescription"></param>
        Sub SelectBluetoothDevice(ByVal SelectedBluetoothDeviceDescription As String)

        ''' <summary>
        ''' A method that should search for all available sound devices and display the results in the Gui using ISipGui.PopulateSoundDevicesList
        ''' </summary>
        Sub SearchForSoundDevices()

        ''' <summary>
        ''' A method that should initiate a connection to the sound device with the indicated SelectedSoundDeviceDescription, 
        ''' and if the bluetooth device is successfully connected disable Gui controls for bluetooth connection using 
        ''' ISipGui.DisableBluetoothDevicesList, ISipGui.DisableBluetoothSearchButton and ISipGui.DisableBluetoothDeviceSelectionButton.
        ''' </summary>
        ''' <param name="SelectedSoundDeviceDescription"></param>
        Sub SelectSoundDevice(ByVal SelectedSoundDeviceDescription As String)

        ''' <summary>
        ''' A method that should handle the event triggered when the user clicks the save button
        ''' </summary>
        Sub SaveFileButtonPressed()

        ''' <summary>
        ''' A method that should handle the event triggered when the user clicks the open button
        ''' </summary>
        Sub OpenFileButtonPressed()

        ''' <summary>
        ''' A method that should handle the event triggered when the user clicks the export button
        ''' </summary>
        Sub ExportDataButtonPressed()


    End Interface


End Namespace