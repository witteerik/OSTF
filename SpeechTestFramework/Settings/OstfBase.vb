Public Module OstfBase

    ' Program location
    Public Property MediaRootDirectory As String = "" '= IO.Path.Combine("C:\", "OSTFMedia") 'Indicates the root path. Other paths given in the project setting files are relative (subpaths) to this path only if they begin with .\ otherwise they are taken as absolute paths.

    Public Property AvailableTestsSubFolder As String = "AvailableSpeechMaterials"
    Public Property CalibrationSignalSubDirectory As String = "CalibrationSignals"
    Public Property AudioSystemSettingsFile As String = IO.Path.Combine("AudioSystem", "AudioSystemSpecification.txt")
    Public Property RoomImpulsesSubDirectory As String = "RoomImpulses"
    Public Property AvailableTests As New List(Of SpeechMaterialSpecification)

    Private _SoundPlayer As Audio.PortAudioVB.OverlappingSoundPlayer

    ''' <summary>
    ''' The SoundPlayer shared between all OSTF applications. Each application that uses it, is responsible of initiating it, with the settings required by the specific application. As well as disposing it when the application is closed.
    ''' </summary>
    Public ReadOnly Property SoundPlayer As Audio.PortAudioVB.OverlappingSoundPlayer
        Get
            If _SoundPlayer Is Nothing Then _SoundPlayer = New Audio.PortAudioVB.OverlappingSoundPlayer(False, False, False, False)
            Return _SoundPlayer
        End Get
    End Property

    Public Function SoundPlayerIsInitialized() As Boolean
        Return _SoundPlayer IsNot Nothing
    End Function

    ''' <summary>
    ''' This sub needs to be called upon startup of all OSTF applications.
    ''' </summary>
    ''' <param name="StartupPath"></param>
    Public Sub InitializeOSTF(ByVal StartupPath As String)

        Try
            Dim local_settings_FilePath As String = IO.Path.Combine(StartupPath, "local_settings.txt")
            Dim local_settings_Input = IO.File.ReadAllLines(local_settings_FilePath)

            For Each item In local_settings_Input
                If item.Trim = "" Then Continue For
                If item.Trim.StartsWith("//") Then Continue For

                Dim SplitItem = item.Trim.Split("=")
                If SplitItem.Length < 2 Then Continue For

                If SplitItem(0).Trim.ToLower.StartsWith("MediaRootDirectory".ToLower) Then MediaRootDirectory = SplitItem(1).Trim

            Next

            If IO.Directory.Exists(MediaRootDirectory) = False Then
                Throw New Exception("Unable to locate the MediaRootDirectory: " & MediaRootDirectory & vbCrLf &
                                    "Make sure that the correct MediaRootDirectory directory is supplied in the file 'local_settings.txt' located at your application startup path.")
            End If

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
            End If

            If Line.StartsWith("ApiName") Then ApiName = InputFileSupport.GetInputFileValue(Line, True)
            If Line.Replace(" ", "").StartsWith("OutputDevice=") Then OutputDeviceName = InputFileSupport.GetInputFileValue(Line, True)
            If Line.Replace(" ", "").StartsWith("OutputDevices=") Then OutputDeviceNames = InputFileSupport.InputFileListOfStringParsing(Line, False, True)
            If Line.Replace(" ", "").StartsWith("InputDevice=") Then InputDeviceName = InputFileSupport.GetInputFileValue(Line, True)
            If Line.Replace(" ", "").StartsWith("InputDevices=") Then InputDeviceNames = InputFileSupport.InputFileListOfStringParsing(Line, False, True)
            If Line.StartsWith("BufferSize") Then BufferSize = InputFileSupport.InputFileIntegerValueParsing(Line, True, AudioSystemSpecificationFilePath)

            If Line = "<New transducer>" Then Exit For
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

            MsgBox("Unable to load the sound API (" & ApiName & ") and device/s indicated in the file " & AudioSystemSpecificationFilePath & vbCrLf & vbCrLf &
                "Output device: " & OutputDeviceName & vbCrLf &
                "Output devices: " & String.Join(", ", OutputDeviceNames) & vbCrLf &
                "Input device: " & InputDeviceName & vbCrLf &
                "Input devices: " & String.Join(", ", InputDeviceNames) & vbCrLf & vbCrLf &
                "Click OK to manually select audio input/output devices." & vbCrLf & vbCrLf &
                "IMPORTANT: Sound tranducer calibration and/or routing may not be correct when manually selected sound devices are used!", MsgBoxStyle.Exclamation, "Sound device not found!")

            Dim NewAudioSettingsDialog As New AudioSettingsDialog()
            Dim AudioSettingsDialogResult = NewAudioSettingsDialog.ShowDialog()
            If AudioSettingsDialogResult = Windows.Forms.DialogResult.OK Then
                AudioApiSettings = NewAudioSettingsDialog.CurrentAudioApiSettings
            Else
                MsgBox("You pressed cancel. Default sound settings will be used", MsgBoxStyle.Exclamation, "Select sound device!")
                AudioApiSettings.SelectDefaultAudioDevice()
            End If
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
            If Line.StartsWith("PresentationType") Then CurrentTransducer.PresentationType = InputFileSupport.InputFileEnumValueParsing(Line, GetType(PresentationTypes), AudioSystemSpecificationFilePath, True)
            If Line.StartsWith("HeadphonesName") Then CurrentTransducer.HeadphonesName = InputFileSupport.InputFileEnumValueParsing(Line, GetType(HeadphonesName), AudioSystemSpecificationFilePath, True)
            If Line.StartsWith("SoundSourceAzimuths") Then CurrentTransducer.SoundSourceAzimuths = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
            If Line.StartsWith("SoundSourceElevations") Then CurrentTransducer.SoundSourceElevations = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
            If Line.StartsWith("SoundSourceDistances") Then CurrentTransducer.SoundSourceDistances = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
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

    Public Enum PresentationTypes
        SoundField
        SimulatedSoundField
        Headphones
        Ambisonics
    End Enum

    Public Enum HeadphonesName
        Unspecified
        AKGK601
        AKGK271MKII
        SennheiserHD25_1
    End Enum

    Public Class AudioSystemSpecification
        Public Property Name As String = "Default"
        Public ReadOnly Property ParentAudioApiSettings As Audio.AudioApiSettings
        Public Property Mixer As Audio.PortAudioVB.DuplexMixer
        Public Property PresentationType As PresentationTypes = PresentationTypes.SoundField
        Public Property HeadphonesName As HeadphonesName = HeadphonesName.Unspecified
        Public Property SoundSourceAzimuths As New List(Of Double) From {-90, 90}
        Public Property SoundSourceElevations As New List(Of Double) From {0, 0}
        Public Property SoundSourceDistances As New List(Of Double) From {0, 0}
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
            Mixer = New Audio.PortAudioVB.DuplexMixer(Me)

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
            OutputList.Add("Presentation type: " & PresentationType.ToString)
            OutputList.Add("Headphones (when used): " & HeadphonesName.ToString)
            OutputList.Add("Sound-source azimuths: " & String.Join(", ", SoundSourceAzimuths))
            OutputList.Add("Hardware output channels: " & String.Join(", ", HardwareOutputChannels))
            OutputList.Add("CalibrationGain: " & String.Join(", ", CalibrationGain))
            If LimiterThreshold.HasValue Then
                OutputList.Add("Limiter threshold: " & LimiterThreshold.ToString)
            Else
                OutputList.Add("Limiter threshold: (none)")
            End If
            OutputList.Add(vbCrLf & "Sound device info: " & vbCrLf & ParentAudioApiSettings.ToShorterString)

            Return String.Join(vbCrLf, OutputList)
        End Function

    End Class

End Module
