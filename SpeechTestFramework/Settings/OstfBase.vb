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
        '<AudioAPI>
        'API = MME // "MME", "Windows DirectSound", "Windows WASAPI", "ASIO"
        'OutputDevice = Högtalare (2- Realtek(R) Audio) // For example: "Primär ljuddrivrutin", "HÃ¶gtalare (2- Realtek(R) Audio) (2)", (MME:"Microsoft Sound Mapper - Output")
        'InputDevice = 
        'BufferSize = 2048 // Must be powers of 2

        Dim ApiName As String = "MME"
        Dim OutputDeviceName As String = "Högtalare (2- Realtek(R) Audio)"
        Dim InputDevice As String = ""
        Dim BufferSize As Integer = 2048

        Dim AudioApiSettings As New Audio.AudioApiSettings
        Dim DeviceLoadSuccess As Boolean
        If ApiName = "ASIO" Then
            DeviceLoadSuccess = AudioApiSettings.SetAsioSoundDevice(OutputDeviceName, BufferSize)
        Else
            DeviceLoadSuccess = AudioApiSettings.SetNonAsioSoundDevice(ApiName, OutputDeviceName, InputDevice, BufferSize)
        End If

        If DeviceLoadSuccess = False Then
            MsgBox("Unable to load the sound API (" & ApiName & ") and output/input device/s (" & OutputDeviceName & "/" & InputDevice & ") indicated in the file " & AudioSystemSpecificationFilePath & vbCrLf &
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


        _AvaliableTransducers = New List(Of AudioSystemSpecification)


        ''Getting calibration file descriptions from the text file SignalDescriptions.txt
        Dim InputLines() As String = System.IO.File.ReadAllLines(AudioSystemSpecificationFilePath, System.Text.Encoding.UTF8)
        'For Each line In InputLines
        '    If line.Trim = "" Then Continue For
        '    If line.Trim.StartsWith("//") Then Continue For

        '    'TODO Fix read file input


        '    CalibrationFileDescriptions.Add(IO.Path.GetFileNameWithoutExtension(LineSplit(0).Trim), LineSplit(1).Trim)
        'Next

        _AvaliableTransducers.Add(New AudioSystemSpecification(AudioApiSettings))

    End Sub


    Public Class AudioSystemSpecification
        Public Property Name As String = "Default"
        Public ReadOnly Property ParentAudioApiSettings As Audio.AudioApiSettings
        Public Property Mixer As Audio.PortAudioVB.DuplexMixer
        Public Property PresentationType As Audio.PortAudioVB.DuplexMixer.PresentationTypes = Audio.PortAudioVB.DuplexMixer.PresentationTypes.SoundField
        Public Property HeadphonesName As Audio.PortAudioVB.DuplexMixer.HeadphonesName = Audio.PortAudioVB.DuplexMixer.HeadphonesName.Unspecified
        Public Property SoundSourceAzimuths As New List(Of Double) From {-90, 90}
        Public Property SoundSourceElevations As New List(Of Double) From {0, 0}
        Public Property SoundSourceDistances As New List(Of Double) From {0, 0}
        Public Property HardwareOutputChannels As New List(Of Integer) From {1, 2}
        Public Property Calibration_FsToSpl As New List(Of Double) From {100, 100}
        Public Property LimiterThreshold As Double? = Nothing

        Public Sub New(ByRef ParentAudioApiSettings As Audio.AudioApiSettings)
            Me.ParentAudioApiSettings = ParentAudioApiSettings

            'Setting up the mixer
            Mixer = New Audio.PortAudioVB.DuplexMixer(Me)

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
