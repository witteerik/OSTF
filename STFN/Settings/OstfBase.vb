Imports STFN.Audio.AudioManagementExt
Imports STFN.Audio.SoundPlayers

Public Module OstfBase


    'Optimization libraries
    Public Property UseOptimizationLibraries As Boolean = False ' This can be used to determin if C++ libraries should be called, such as the libostfdsp, instead of calling equivalent OSTF functions implemented in the managed (.NET) code.

    ' Log location 


    ' Program location
    Public Property MediaRootDirectory As String = "" '= IO.Path.Combine("C:\", "OSTFMedia") 'Indicates the root path. Other paths given in the project setting files are relative (subpaths) to this path only if they begin with .\ otherwise they are taken as absolute paths.
    Public Property DefaultMediaRootFolderName As String = "OSTFMedia"

    Public Property WasStartedFromVisualStudio As Boolean = False

    Public Property AvailableTestsSubFolder As String = "AvailableSpeechMaterials"
    Public Property CalibrationSignalSubDirectory As String = "CalibrationSignals"
    Public Property AudioSystemSubDirectory As String = "AudioSystem"
    Public Property AudioSystemSettingsFile As String = IO.Path.Combine(AudioSystemSubDirectory, "AudioSystemSpecification.txt")
    Public Property RoomImpulsesSubDirectory As String = "RoomImpulses"
    Public Property AvailableImpulseResponseSetsFile As String = IO.Path.Combine(RoomImpulsesSubDirectory, "AvailableImpulseResponseSets.txt")

    Public Property AvailableTests As New List(Of SpeechMaterialSpecification)

    ''' <summary>
    ''' The SoundPlayer shared between all STFN applications. Each application that uses it, is responsible of initiating it, with the settings required by the specific application. As well as disposing it when the application is closed.
    ''' </summary>
    Public WithEvents SoundPlayer As Audio.SoundPlayers.iSoundPlayer

    Public Function InitializeSoundPlayer(ByRef SoundPlayer As Audio.SoundPlayers.iSoundPlayer) As Boolean
        SoundPlayer = SoundPlayer
        Return SoundPlayerIsInitialized()
    End Function

    Public Function SoundPlayerIsInitialized() As Boolean
        Return _SoundPlayer IsNot Nothing
    End Function


    Private _PortAudioIsInitialized As Boolean = False
    ''' <summary>
    ''' Returns True if the PortAudio library has been successfylly initialized by call the the OstfBase function InitializeOSTF.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property PortAudioIsInitialized As Boolean
        Get
            Return _PortAudioIsInitialized
        End Get
    End Property

    Private OstfIsInitialized As Boolean = False

    ''' <summary>
    ''' This sub needs to be called upon startup of all OSTF applications. (Any subsequent calls to this Sub will be ignored.)
    ''' </summary>
    Public Sub InitializeOSTF(ByVal MediaRootDirectory As String)

        'Exits the sub to avoid multiple calls (which should be avoided, especially to the Audio.PortAudio.Pa_Initialize function).
        If OstfIsInitialized = True Then Exit Sub
        OstfIsInitialized = True

        Try

            OstfBase.MediaRootDirectory = MediaRootDirectory

            'ReadMediaRootDirectory(StartupPath)

            'Checks that the folder exists
            If IO.Directory.Exists(MediaRootDirectory) = False Then

                MsgBox("Unable to locate the media root directory")

                '0:
                '                Dim NewOSTFMediaFolderDialog As New OSTFMediaFolderDialog
                '                NewOSTFMediaFolderDialog.InitialPath = MediaRootDirectory
                '                NewOSTFMediaFolderDialog.Text = "Cannot find the OSTF media folder"
                '                NewOSTFMediaFolderDialog.StartPosition = Windows.Forms.FormStartPosition.CenterScreen
                '                NewOSTFMediaFolderDialog.ShowDialog()
            End If

            'Checks that it seems to be the right (media) folder
            If IO.Directory.Exists(IO.Path.Combine(MediaRootDirectory, AudioSystemSubDirectory)) = False Then
                MsgBox("It seems like you have selected an incorrect OSTF media folder. The OSTF media folder should for example contain the folder " & AudioSystemSubDirectory & vbCrLf &
                                    "Please try again.", MsgBoxStyle.Exclamation, "Unable to find the OSTF media folder!")

                Throw New Exception("Unable to locate the OSTF media folder! Cannot start the application.")

                'Dim MsgResult = MsgBox("It seems like you have selected an incorrect OSTF media folder. The OSTF media folder should for example contain the folder " & AudioSystemSubDirectory & vbCrLf &
                '                    "Please try again.", MsgBoxStyle.OkCancel, "Unable to find the OSTF media folder!")
                'If MsgResult = MsgBoxResult.Ok Then
                '    GoTo 0
                'Else
                '    Throw New Exception("Unable to locate the OSTF media folder! Cannot start the application.")
                'End If
            End If

            'Initializing the sound player
            'InitializeSoundPlayer()

        Catch ex As Exception
            Throw New Exception("The following error occurred when trying to initialize OSTF:" & vbCrLf & vbCrLf & ex.ToString)
        End Try

    End Sub



    Public Sub LoadAvailableTestSpecifications()

        Dim TestSpecificationFolder As String = IO.Path.Combine(MediaRootDirectory, AvailableTestsSubFolder)

        'Getting .txt files in that folder
        Dim ExistingFiles = IO.Directory.GetFiles(TestSpecificationFolder)
        Dim TextFileNames As New List(Of String)
        For Each FullFilePath In ExistingFiles
            If FullFilePath.EndsWith(".txt") Then
                'Adds only the file name
                TextFileNames.Add(IO.Path.GetFileName(FullFilePath))
            End If
        Next

        'Clears any tests previously loaded before adding new tests
        OstfBase.AvailableTests.Clear()

        For Each TextFileName In TextFileNames
            'Ties to use the text file in order to create a new test specification object, and just skipps it if unsuccessful
            Dim NewTestSpecification = SpeechMaterialSpecification.LoadTestSpecificationFile(TextFileName)
            If NewTestSpecification IsNot Nothing Then
                OstfBase.AvailableTests.Add(NewTestSpecification)
            End If
        Next

    End Sub

    Private _AvaliableTransducers As List(Of AudioSystemSpecification) = Nothing
    Public ReadOnly Property AvaliableTransducers As List(Of AudioSystemSpecification)
        Get
            If _AvaliableTransducers Is Nothing Then LoadAudioSystemSpecificationFile()
            Return _AvaliableTransducers
        End Get
    End Property


    Private Sub LoadAudioSystemSpecificationFile()

        Dim AudioSystemSpecificationFilePath = IO.Path.Combine(OstfBase.MediaRootDirectory, OstfBase.AudioSystemSettingsFile)

        'Reads the API settings, and tries to select the API and device if available, otherwise lets the user select a device manually

        ''Getting calibration file descriptions from the text file SignalDescriptions.txt
        Dim LinesRead As Integer = 0
        Dim InputLines() As String = System.IO.File.ReadAllLines(AudioSystemSpecificationFilePath, System.Text.Encoding.UTF8)

        Dim ApiName As String = "MME"
        Dim OutputDeviceName As String = "Högtalare (2- Realtek(R) Audio)"
        Dim OutputDeviceNames As New List(Of String) ' Used for MME multiple device support
        Dim InputDeviceName As String = ""
        Dim InputDeviceNames As New List(Of String) ' Used for MME multiple device support
        Dim BufferSize As Integer = 2048


        For i = 0 To InputLines.Length - 1
            LinesRead += 1
            Dim Line As String = InputLines(i).Trim

            'Skips empty and outcommented lines
            If Line = "" Then Continue For
            If Line.StartsWith("//") Then Continue For

            If Line = "<AudioDevices>" Then
                'No need to do anything?
                Continue For
            End If
            If Line = "<New transducer>" Then Exit For

            If Line.StartsWith("ApiName") Then ApiName = InputFileSupport.GetInputFileValue(Line, True)
            If Line.Replace(" ", "").StartsWith("OutputDevice=") Then OutputDeviceName = InputFileSupport.GetInputFileValue(Line, True)
            If Line.Replace(" ", "").StartsWith("OutputDevices=") Then OutputDeviceNames = InputFileSupport.InputFileListOfStringParsing(Line, False, True)
            If Line.Replace(" ", "").StartsWith("InputDevice=") Then InputDeviceName = InputFileSupport.GetInputFileValue(Line, True)
            If Line.Replace(" ", "").StartsWith("InputDevices=") Then InputDeviceNames = InputFileSupport.InputFileListOfStringParsing(Line, False, True)
            If Line.StartsWith("BufferSize") Then BufferSize = InputFileSupport.InputFileIntegerValueParsing(Line, True, AudioSystemSpecificationFilePath)

        Next

        Dim DeviceLoadSuccess As Boolean = True
        If OutputDeviceName = "" And InputDeviceName = "" And OutputDeviceNames Is Nothing And InputDeviceNames Is Nothing Then
            'No device names have been specified
            DeviceLoadSuccess = False
        End If

        If ApiName <> "MME" Then
            If OutputDeviceNames IsNot Nothing Or InputDeviceNames IsNot Nothing Then
                DeviceLoadSuccess = False
                MsgBox("When specifying multiple sound (input or output) devices in the file " & AudioSystemSpecificationFilePath & ", the sound API must be MME ( not " & ApiName & ")!", MsgBoxStyle.Exclamation, "Sound device specification error!")
            End If
        End If

        If OutputDeviceNames IsNot Nothing And OutputDeviceName <> "" Then
            DeviceLoadSuccess = False
            MsgBox("Either (not both) of single or multiple sound output devices must be specified in the file " & AudioSystemSpecificationFilePath & "!", MsgBoxStyle.Exclamation, "Sound device specification error!")
        End If

        If InputDeviceNames IsNot Nothing And InputDeviceName <> "" Then
            DeviceLoadSuccess = False
            MsgBox("Either (not both) of single or multiple sound input devices must be specified in the file " & AudioSystemSpecificationFilePath & "!", MsgBoxStyle.Exclamation, "Sound device specification error!")
        End If

        'Tries to setup the AudioApiSettings using the loaded data
        Dim AudioApiSettings As New Audio.AudioApiSettings
        If DeviceLoadSuccess = True Then
            If ApiName = "ASIO" Then
                DeviceLoadSuccess = AudioApiSettings.SetAsioSoundDevice(OutputDeviceName, BufferSize)
            Else
                If OutputDeviceNames Is Nothing And InputDeviceNames Is Nothing Then
                    DeviceLoadSuccess = AudioApiSettings.SetNonAsioSoundDevice(ApiName, OutputDeviceName, InputDeviceName, BufferSize)
                Else
                    DeviceLoadSuccess = AudioApiSettings.SetMmeMultipleDevices(InputDeviceNames, OutputDeviceNames, BufferSize)
                End If
            End If
        End If

        If DeviceLoadSuccess = False Then

            If OutputDeviceNames Is Nothing Then OutputDeviceNames = New List(Of String)
            If InputDeviceNames Is Nothing Then InputDeviceNames = New List(Of String)

            MsgBox("The Open Speech Test Framework (OSTF) was unable to load the sound API (" & ApiName & ") and device/s indicated in the file " & AudioSystemSpecificationFilePath & vbCrLf & vbCrLf &
                "Output device: " & OutputDeviceName & vbCrLf &
                "Output devices: " & String.Join(", ", OutputDeviceNames) & vbCrLf &
                "Input device: " & InputDeviceName & vbCrLf &
                "Input devices: " & String.Join(", ", InputDeviceNames) & vbCrLf & vbCrLf &
                "Click OK to manually select audio input/output devices." & vbCrLf & vbCrLf &
                "IMPORTANT: Sound tranducer calibration and/or routing may not be correct when manually selected sound devices are used!", MsgBoxStyle.Exclamation, "OSTF sound device not found!")

            'Using default settings, as their is not yet any GUI for selecting settings such as the NET FrameWork AudioSettingsDialog 

            'Dim NewAudioSettingsDialog As New AudioSettingsDialog()
            'Dim AudioSettingsDialogResult = NewAudioSettingsDialog.ShowDialog()
            'If AudioSettingsDialogResult = Windows.Forms.DialogResult.OK Then
            '    AudioApiSettings = NewAudioSettingsDialog.CurrentAudioApiSettings
            'Else
            '    MsgBox("You pressed cancel. Default sound settings will be used", MsgBoxStyle.Exclamation, "Select sound device!")
            AudioApiSettings.SelectDefaultAudioDevice()
            'End If
        End If

        'Reads the remains of the file
        _AvaliableTransducers = New List(Of AudioSystemSpecification)

        'Backs up one line
        LinesRead = Math.Max(0, LinesRead - 1)

        Dim CurrentTransducer As AudioSystemSpecification = Nothing
        For i = LinesRead To InputLines.Length - 1
            Dim Line As String = InputLines(i).Trim

            'Skips empty and outcommented lines
            If Line = "" Then Continue For
            If Line.StartsWith("//") Then Continue For

            If Line = "<New transducer>" Then
                If CurrentTransducer Is Nothing Then
                    'Creates the first transducer
                    CurrentTransducer = New AudioSystemSpecification(AudioApiSettings)
                Else
                    'Stores the transducer
                    _AvaliableTransducers.Add(CurrentTransducer)
                    'Creates a new one
                    CurrentTransducer = New AudioSystemSpecification(AudioApiSettings)
                End If
            End If

            If Line.StartsWith("Name") Then CurrentTransducer.Name = InputFileSupport.GetInputFileValue(Line, True)
            If Line.StartsWith("LoudspeakerAzimuths") Then CurrentTransducer.LoudspeakerAzimuths = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
            If Line.StartsWith("LoudspeakerElevations") Then CurrentTransducer.LoudspeakerElevations = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
            If Line.StartsWith("LoudspeakerDistances") Then CurrentTransducer.LoudspeakerDistances = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
            If Line.StartsWith("HardwareOutputChannels") Then CurrentTransducer.HardwareOutputChannels = InputFileSupport.InputFileListOfIntegerParsing(Line, True, AudioSystemSpecificationFilePath)
            If Line.StartsWith("CalibrationGain") Then CurrentTransducer.CalibrationGain = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
            If Line.StartsWith("LimiterThreshold") Then CurrentTransducer.LimiterThreshold = InputFileSupport.InputFileDoubleValueParsing(Line, True, AudioSystemSpecificationFilePath)

        Next

        'Stores the last transducer
        If CurrentTransducer IsNot Nothing Then _AvaliableTransducers.Add(CurrentTransducer)

        'Adding a default transducer if none were sucessfully read
        If _AvaliableTransducers.Count = 0 Then _AvaliableTransducers.Add(New AudioSystemSpecification(AudioApiSettings))

        For Each Transducer In _AvaliableTransducers
            Transducer.SetupMixer()
        Next

        'Checking calibration gain values and issues warnings if calibration gain is above 30 dB
        For Each Transducer In _AvaliableTransducers
            For i = 0 To Transducer.CalibrationGain.Count - 1
                If Transducer.CalibrationGain(i) > 30 Then
                    MsgBox("Calibration gain number " & i & " for the audio transducer '" & Transducer.Name & "' exceeds 30 dB. " & vbCrLf & vbCrLf &
                           "Make sure that this is really correct before you continue and be cautios not to inflict personal injuries or damage your equipment if you continue! " & vbCrLf & vbCrLf &
                           "This calibration value is set in the audio system specifications file: " & AudioSystemSpecificationFilePath, MsgBoxStyle.Exclamation, "Warning - High calibration gain value!")
                End If
            Next
        Next

    End Sub



    Public Enum SoundPropagationTypes
        PointSpeakers
        SimulatedSoundField
        Ambisonics
    End Enum

    Public Class AudioSystemSpecification
        Public Property Name As String = "Default"
        Public ReadOnly Property ParentAudioApiSettings As Audio.AudioApiSettings
        Public Property Mixer As STFN.Audio.SoundScene.DuplexMixer
        Public Property LoudspeakerAzimuths As New List(Of Double) From {-90, 90}
        Public Property LoudspeakerElevations As New List(Of Double) From {0, 0}
        Public Property LoudspeakerDistances As New List(Of Double) From {0, 0}
        Public Property HardwareOutputChannels As New List(Of Integer) From {1, 2}
        Public Property CalibrationGain As New List(Of Double) From {0, 0}
        Public Property LimiterThreshold As Double? = Nothing

        Private _CanPlay As Boolean = False
        Public ReadOnly Property CanPlay As Boolean
            Get
                Return _CanPlay
            End Get
        End Property

        Private _CanRecord As Boolean = False
        Public ReadOnly Property CanRecord As Boolean
            Get
                Return _CanRecord
            End Get
        End Property

        Public Sub New(ByRef ParentAudioApiSettings As Audio.AudioApiSettings)
            Me.ParentAudioApiSettings = ParentAudioApiSettings

        End Sub

        Public Sub SetupMixer()
            'Setting up the mixer
            Mixer = New Audio.SoundScene.DuplexMixer(Me)

            CheckCanPlayRecord()

        End Sub
        Public Sub CheckCanPlayRecord()

            'Checks if the transducerss will play/record
            If Me.Mixer.OutputRouting.Keys.Count > 0 And Me.ParentAudioApiSettings.NumberOfOutputChannels.HasValue = True Then
                If Me.Mixer.OutputRouting.Keys.Max <= Me.ParentAudioApiSettings.NumberOfOutputChannels Then
                    Me._CanPlay = True
                End If
            End If
            If Me.Mixer.InputRouting.Keys.Count > 0 And Me.ParentAudioApiSettings.NumberOfInputChannels.HasValue = True Then
                If Me.Mixer.InputRouting.Keys.Max <= Me.ParentAudioApiSettings.NumberOfInputChannels Then
                    Me._CanRecord = True
                End If
            End If

        End Sub

        Public Sub OverrideCanPlay(ByVal Value As Boolean)
            _CanPlay = Value
        End Sub

        Public Overrides Function ToString() As String
            If Name <> "" Then
                Return Name
            Else
                Return MyBase.ToString()
            End If
        End Function

        Public Function GetDescriptionString() As String
            Dim OutputList As New List(Of String)

            OutputList.Add("Name: " & Name)
            OutputList.Add("Loudspeaker azimuths: " & String.Join(", ", LoudspeakerAzimuths))
            OutputList.Add("Hardware output channels: " & String.Join(", ", HardwareOutputChannels))
            OutputList.Add("CalibrationGain: " & String.Join(", ", CalibrationGain))
            If LimiterThreshold.HasValue Then
                OutputList.Add("Limiter threshold: " & LimiterThreshold.ToString)
            Else
                OutputList.Add("Limiter threshold: (none)")
            End If
            'OutputList.Add(vbCrLf & "Sound device info: " & vbCrLf & ParentAudioApiSettings.ToShorterString)

            Return String.Join(vbCrLf, OutputList)
        End Function

    End Class


    Private _DirectionalSimulator As DirectionalSimulation = Nothing

    Public ReadOnly Property DirectionalSimulator As DirectionalSimulation
        Get
            If _DirectionalSimulator Is Nothing Then _DirectionalSimulator = New DirectionalSimulation()
            Return _DirectionalSimulator
        End Get
    End Property

End Module
