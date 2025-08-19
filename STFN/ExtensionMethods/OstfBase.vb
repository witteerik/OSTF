Imports System.Runtime.CompilerServices
Imports STFN.Core


'This file contains extension methods for STFN.Core.OstfBase

Partial Public Module Extensions



    'Protected Shared Sub LoadAudioSystemSpecificationFile_OLD( )

    '    Dim AudioSystemSpecificationFilePath = IO.Path.Combine(OstfBase.MediaRootDirectory, OstfBase.AudioSystemSettingsFile)

    '    'Reads the API settings, and tries to select the API and device if available, otherwise lets the user select a device manually

    '    ''Getting calibration file descriptions from the text file SignalDescriptions.txt
    '    Dim LinesRead As Integer = 0
    '    Dim InputLines() As String = System.IO.File.ReadAllLines(AudioSystemSpecificationFilePath, System.Text.Encoding.UTF8)

    '    Dim ApiName As String = "MME"
    '    Dim OutputDeviceName As String = "Högtalare (2- Realtek(R) Audio)"
    '    Dim OutputDeviceNames As New List(Of String) ' Used for MME multiple device support
    '    Dim InputDeviceName As String = ""
    '    Dim InputDeviceNames As New List(Of String) ' Used for MME multiple device support
    '    Dim BufferSize As Integer = 2048


    '    For i = 0 To InputLines.Length - 1
    '        LinesRead += 1
    '        Dim Line As String = InputLines(i).Trim

    '        'Skips empty and outcommented lines
    '        If Line = "" Then Continue For
    '        If Line.StartsWith("//") Then Continue For

    '        If Line = "<AudioDevices>" Then
    '            'No need to do anything?
    '            Continue For
    '        End If
    '        If Line = "<New transducer>" Then Exit For

    '        If Line.StartsWith("ApiName") Then ApiName = InputFileSupport.GetInputFileValue(Line, True)
    '        If Line.Replace(" ", "").StartsWith("OutputDevice=") Then OutputDeviceName = InputFileSupport.GetInputFileValue(Line, True)
    '        If Line.Replace(" ", "").StartsWith("OutputDevices=") Then OutputDeviceNames = InputFileSupport.InputFileListOfStringParsing(Line, False, True)
    '        If Line.Replace(" ", "").StartsWith("InputDevice=") Then InputDeviceName = InputFileSupport.GetInputFileValue(Line, True)
    '        If Line.Replace(" ", "").StartsWith("InputDevices=") Then InputDeviceNames = InputFileSupport.InputFileListOfStringParsing(Line, False, True)
    '        If Line.StartsWith("BufferSize") Then BufferSize = InputFileSupport.InputFileIntegerValueParsing(Line, True, AudioSystemSpecificationFilePath)

    '    Next

    '    Dim DeviceLoadSuccess As Boolean = True
    '    If OutputDeviceName = "" And InputDeviceName = "" And OutputDeviceNames Is Nothing And InputDeviceNames Is Nothing Then
    '        'No device names have been specified
    '        DeviceLoadSuccess = False
    '    End If

    '    If ApiName <> "MME" Then
    '        If OutputDeviceNames IsNot Nothing Or InputDeviceNames IsNot Nothing Then
    '            DeviceLoadSuccess = False
    '            MsgBox("When specifying multiple sound (input or output) devices in the file " & AudioSystemSpecificationFilePath & ", the sound API must be MME ( not " & ApiName & ")!", MsgBoxStyle.Exclamation, "Sound device specification error!")
    '        End If
    '    End If

    '    If OutputDeviceNames IsNot Nothing And OutputDeviceName <> "" Then
    '        DeviceLoadSuccess = False
    '        MsgBox("Either (not both) of single or multiple sound output devices must be specified in the file " & AudioSystemSpecificationFilePath & "!", MsgBoxStyle.Exclamation, "Sound device specification error!")
    '    End If

    '    If InputDeviceNames IsNot Nothing And InputDeviceName <> "" Then
    '        DeviceLoadSuccess = False
    '        MsgBox("Either (not both) of single or multiple sound input devices must be specified in the file " & AudioSystemSpecificationFilePath & "!", MsgBoxStyle.Exclamation, "Sound device specification error!")
    '    End If

    '    'Tries to setup the PortAudioApiSettings using the loaded data
    '    Dim AudioApiSettings As New Audio.PortAudioApiSettings
    '    If DeviceLoadSuccess = True Then
    '        If ApiName = "ASIO" Then
    '            DeviceLoadSuccess = AudioApiSettings.SetAsioSoundDevice(OutputDeviceName, BufferSize)
    '        Else
    '            If OutputDeviceNames Is Nothing And InputDeviceNames Is Nothing Then
    '                DeviceLoadSuccess = AudioApiSettings.SetNonAsioSoundDevice(ApiName, OutputDeviceName, InputDeviceName, BufferSize)
    '            Else
    '                DeviceLoadSuccess = AudioApiSettings.SetMmeMultipleDevices(InputDeviceNames, OutputDeviceNames, BufferSize)
    '            End If
    '        End If
    '    End If

    '    If DeviceLoadSuccess = False Then

    '        If OutputDeviceNames Is Nothing Then OutputDeviceNames = New List(Of String)
    '        If InputDeviceNames Is Nothing Then InputDeviceNames = New List(Of String)

    '        MsgBox("The Open Speech Test Framework (OSTF) was unable to load the sound API (" & ApiName & ") and device/s indicated in the file " & AudioSystemSpecificationFilePath & vbCrLf & vbCrLf &
    '            "Output device: " & OutputDeviceName & vbCrLf &
    '            "Output devices: " & String.Join(", ", OutputDeviceNames) & vbCrLf &
    '            "Input device: " & InputDeviceName & vbCrLf &
    '            "Input devices: " & String.Join(", ", InputDeviceNames) & vbCrLf & vbCrLf &
    '            "Click OK to manually select audio input/output devices." & vbCrLf & vbCrLf &
    '            "IMPORTANT: Sound tranducer calibration and/or routing may not be correct when manually selected sound devices are used!", MsgBoxStyle.Exclamation, "OSTF sound device not found!")

    '        'Using default settings, as their is not yet any GUI for selecting settings such as the NET FrameWork AudioSettingsDialog 

    '        'Dim NewAudioSettingsDialog As New AudioSettingsDialog()
    '        'Dim AudioSettingsDialogResult = NewAudioSettingsDialog.ShowDialog()
    '        'If AudioSettingsDialogResult = Windows.Forms.DialogResult.OK Then
    '        '    PortAudioApiSettings = NewAudioSettingsDialog.CurrentAudioApiSettings
    '        'Else
    '        '    MsgBox("You pressed cancel. Default sound settings will be used", MsgBoxStyle.Exclamation, "Select sound device!")
    '        AudioApiSettings.SelectDefaultAudioDevice()
    '        'End If
    '    End If

    '    'Reads the remains of the file
    '    _AvaliableTransducers = New List(Of AudioSystemSpecification)

    '    'Backs up one line
    '    LinesRead = Math.Max(0, LinesRead - 1)

    '    Dim CurrentTransducer As AudioSystemSpecification = Nothing
    '    For i = LinesRead To InputLines.Length - 1
    '        Dim Line As String = InputLines(i).Trim

    '        'Skips empty and outcommented lines
    '        If Line = "" Then Continue For
    '        If Line.StartsWith("//") Then Continue For

    '        If Line = "<New transducer>" Then
    '            If CurrentTransducer Is Nothing Then
    '                'Creates the first transducer
    '                CurrentTransducer = New AudioSystemSpecification(CurrentMediaPlayerType, AudioApiSettings)
    '            Else
    '                'Stores the transducer
    '                _AvaliableTransducers.Add(CurrentTransducer)
    '                'Creates a new one
    '                CurrentTransducer = New AudioSystemSpecification(CurrentMediaPlayerType, AudioApiSettings)
    '            End If
    '        End If

    '        If Line.StartsWith("Name") Then CurrentTransducer.Name = InputFileSupport.GetInputFileValue(Line, True)
    '        If Line.StartsWith("LoudspeakerAzimuths") Then CurrentTransducer.LoudspeakerAzimuths = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
    '        If Line.StartsWith("LoudspeakerElevations") Then CurrentTransducer.LoudspeakerElevations = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
    '        If Line.StartsWith("LoudspeakerDistances") Then CurrentTransducer.LoudspeakerDistances = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
    '        If Line.StartsWith("HardwareOutputChannels") Then CurrentTransducer.HardwareOutputChannels = InputFileSupport.InputFileListOfIntegerParsing(Line, True, AudioSystemSpecificationFilePath)
    '        If Line.StartsWith("CalibrationGain") Then CurrentTransducer.CalibrationGain = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
    '        If Line.StartsWith("LimiterThreshold") Then CurrentTransducer.LimiterThreshold = InputFileSupport.InputFileDoubleValueParsing(Line, True, AudioSystemSpecificationFilePath)

    '    Next

    '    'Stores the last transducer
    '    If CurrentTransducer IsNot Nothing Then _AvaliableTransducers.Add(CurrentTransducer)

    '    'Adding a default transducer if none were sucessfully read
    '    If _AvaliableTransducers.Count = 0 Then _AvaliableTransducers.Add(New AudioSystemSpecification(CurrentMediaPlayerType, AudioApiSettings))

    '    If OstfBase.CurrentMediaPlayerType <> AudioSystemSpecification.MediaPlayerTypes.AudioTrackBased Then
    '        'This has to be made later in STFM for the AudioTrackBased player
    '        For Each Transducer In _AvaliableTransducers
    '            Transducer.SetupMixer()
    '        Next
    '    End If

    '    'Checking calibration gain values and issues warnings if calibration gain is above 30 dB
    '    For Each Transducer In _AvaliableTransducers
    '        For i = 0 To Transducer.CalibrationGain.Count - 1
    '            If Transducer.CalibrationGain(i) > 30 Then
    '                MsgBox("Calibration gain number " & i & " for the audio transducer '" & Transducer.Name & "' exceeds 30 dB. " & vbCrLf & vbCrLf &
    '                       "Make sure that this is really correct before you continue and be cautios not to inflict personal injuries or damage your equipment if you continue! " & vbCrLf & vbCrLf &
    '                       "This calibration value is set in the audio system specifications file: " & AudioSystemSpecificationFilePath, MsgBoxStyle.Exclamation, "Warning - High calibration gain value!")
    '            End If
    '        Next
    '    Next

    'End Sub





End Module



