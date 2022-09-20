Public Module OstfBase

    ' Program location
    Public Property RootDirectory As String = IO.Path.Combine("C:\", "OSTF") 'Indicates the root path. Other paths given in the project setting files are relative (subpaths) to this path only if they begin with .\ otherwise they are taken as absolute paths.

    Public Property AvailableTestsSubFolder As String = "AvailableTests"
    Public Property CalibrationSignalSubDirectory As String = "CalibrationSignals"
    Public Property CalibrationSettingsFile As String = IO.Path.Combine("Calibration", "TransducerCalibration.txt")
    Public Property RoomImpulsesSubDirectory As String = "RoomImpulses"
    Public Property AvailableTests As New List(Of TestSpecification)


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

    Private _AvaliableTransducers As List(Of TransducerSpecification) = Nothing
    Public ReadOnly Property AvaliableTransducers As List(Of TransducerSpecification)
        Get
            If _AvaliableTransducers Is Nothing Then LoadTransducerCalibrationFile()
            Return _AvaliableTransducers
        End Get
    End Property

    Private Sub LoadTransducerCalibrationFile()

        _AvaliableTransducers = New List(Of TransducerSpecification)

        Dim CalibrationSettingsFilePath = IO.Path.Combine(OstfBase.RootDirectory, OstfBase.CalibrationSettingsFile)

        ''Getting calibration file descriptions from the text file SignalDescriptions.txt
        'Dim InputLines() As String = System.IO.File.ReadAllLines(CalibrationSettingsFilePath, System.Text.Encoding.UTF8)
        'For Each line In InputLines
        '    If line.Trim = "" Then Continue For
        '    If line.Trim.StartsWith("//") Then Continue For

        '    'TODO Fix this


        '    CalibrationFileDescriptions.Add(IO.Path.GetFileNameWithoutExtension(LineSplit(0).Trim), LineSplit(1).Trim)
        'Next

        _AvaliableTransducers.Add(New TransducerSpecification)

    End Sub


    Public Class AudioApiSpecification

        Public Property Name As String = "Default"
        Public Property API As String = "MME"
        Public Property OutputDevice As String = "Högtalare (2- Realtek(R) Audio)"
        Public Property InputDevice As String = "Högtalare (2- Realtek(R) Audio)"
        Public Property BufferSize As Integer = 2048

        Public Shared Function LoadFromFile(ByRef FilePath As String) As AudioApiSpecification

        End Function

    End Class

    Public Class TransducerSpecification
        Public Property Name As String = "Default"
        Public Property TransducerType As Audio.PortAudioVB.DuplexMixer.TransducerTypes = Audio.PortAudioVB.DuplexMixer.TransducerTypes.SoundField
        Public Property TransducerName As Audio.PortAudioVB.DuplexMixer.TransducerNames = Audio.PortAudioVB.DuplexMixer.TransducerNames.Unspecified
        Public Property SoundSourceAzimuths As New List(Of Double) From {-90, 90}
        Public Property SoundSourceElevations As New List(Of Double) From {0, 0}
        Public Property SoundSourceDistances As New List(Of Double) From {0, 0}
        Public Property OutputChannels As New List(Of Integer) From {1, 2}
        Public Property Calibration_FsToSpl As New List(Of Double) From {100, 100}
        Public Property LimiterThreshold As Double? = Nothing

        Public Overrides Function ToString() As String
            If Name <> "" Then
                Return Name
            Else
                Return MyBase.ToString()
            End If
        End Function

    End Class

End Module
