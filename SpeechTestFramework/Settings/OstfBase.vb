Public Module OstfBase

    ' Program location
    Public Property RootDirectory As String = IO.Path.Combine("C:\", "OSTF") 'Indicates the root path. Other paths given in the project setting files are relative (subpaths) to this path only if they begin with .\ otherwise they are taken as absolute paths.

    Public Property AvailableTestsSubFolder As String = "AvailableTests"
    Public Property CalibrationSignalSubDirectory As String = "CalibrationSignals"
    Public Property AudioSystemSettingsFile As String = IO.Path.Combine("AudioSystem", "AudioSystemSpecification.txt")
    Public Property RoomImpulsesSubDirectory As String = "RoomImpulses"
    Public Property AvailableTests As New List(Of TestSpecification)

    ''' <summary>
    ''' The SoundPlayer shared between all OSTF applications. Each application that uses it, is responsible of initiating it, with the settings required by the specific application. As well as disposing it when the application is closed.
    ''' </summary>
    Public SoundPlayer As Audio.PortAudioVB.OverlappingSoundPlayer

    Public Sub LoadAvailableTestSpecifications()

        Dim TestSpecificationFolder As String = IO.Path.Combine(RootDirectory, AvailableTestsSubFolder)

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
            Dim NewTestSpecification = TestSpecification.LoadTestSpecificationFile(TextFileName)
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

        Dim AudioSystemSpecificationFilePath = IO.Path.Combine(OstfBase.RootDirectory, OstfBase.AudioSystemSettingsFile)

        'Reads the API settings, and tries to select the API and device if available, otherwise lets the user select a device manually

        ''Getting calibration file descriptions from the text file SignalDescriptions.txt
        Dim LinesRead As Integer = 0
        Dim InputLines() As String = System.IO.File.ReadAllLines(AudioSystemSpecificationFilePath, System.Text.Encoding.UTF8)

        Dim ApiName As String = "MME"
        Dim OutputDeviceName As String = "Högtalare (2- Realtek(R) Audio)"
        Dim InputDeviceName As String = ""
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
            If Line.StartsWith("OutputDevice") Then OutputDeviceName = InputFileSupport.GetInputFileValue(Line, True)
            If Line.StartsWith("InputDevice") Then InputDeviceName = InputFileSupport.GetInputFileValue(Line, True)
            If Line.StartsWith("BufferSize") Then BufferSize = InputFileSupport.InputFileIntegerValueParsing(Line, True, AudioSystemSpecificationFilePath)

            If Line = "<New transducer>" Then Exit For
        Next

        'Tries to setup the AudioApiSettings using the loaded data
        Dim AudioApiSettings As New Audio.AudioApiSettings
        Dim DeviceLoadSuccess As Boolean
        If ApiName = "ASIO" Then
            DeviceLoadSuccess = AudioApiSettings.SetAsioSoundDevice(OutputDeviceName, BufferSize)
        Else
            DeviceLoadSuccess = AudioApiSettings.SetNonAsioSoundDevice(ApiName, OutputDeviceName, InputDeviceName, BufferSize)
        End If

        If DeviceLoadSuccess = False Then
            MsgBox("Unable to load the sound API (" & ApiName & ") and output/input device/s (" & OutputDeviceName & "/" & InputDeviceName & ") indicated in the file " & AudioSystemSpecificationFilePath & vbCrLf &
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
            If Line.StartsWith("Calibration_FsToSpl") Then CurrentTransducer.Calibration_FsToSpl = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
            If Line.StartsWith("LimiterThreshold") Then CurrentTransducer.LimiterThreshold = InputFileSupport.InputFileDoubleValueParsing(Line, True, AudioSystemSpecificationFilePath)

        Next

        'Stores the last transducer
        If CurrentTransducer IsNot Nothing Then _AvaliableTransducers.Add(CurrentTransducer)

        'Adding a default transducer if none were sucessfully read
        If _AvaliableTransducers.Count = 0 Then _AvaliableTransducers.Add(New AudioSystemSpecification(AudioApiSettings))

        For Each Transducer In _AvaliableTransducers
            Transducer.SetupMixer()
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
        Public Property Calibration_FsToSpl As New List(Of Double) From {100, 100}
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
            OutputList.Add("Calibration (dBFs to dBSpl): " & String.Join(", ", Calibration_FsToSpl))
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
