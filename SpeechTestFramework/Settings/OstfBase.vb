Imports SpeechTestFramework.Audio.AudioManagementExt

Public Module OstfBase

    Public Enum Platforms ' These reflects the platform names currently specified in NET MAUI. Not all will work with OSTF.
        iOS
        WinUI
        UWP
        Tizen
        tvOS
        MacCatalyst
        macOS
        watchOS
        Unknown
        Android
    End Enum

    Public Property CurrentPlatForm As Platforms

    Public Enum MediaPlayerTypes
        ''' <summary>
        ''' A sound player type using the Port Audio library
        ''' </summary>
        PaBased
        ''' <summary>
        ''' A media player type using the MAUI Community Toolkit Media Element library
        ''' </summary>
        MctBased
        ''' <summary>
        ''' The default media player type for the specified platform as defined by OSTF.
        ''' </summary>
        [Default]
    End Enum

    Public CurrentMediaPlayerType As MediaPlayerTypes

    'Optimization libraries
    Public Property UseOptimizationLibraries As Boolean = True ' This can be used to determin if C++ libraries should be called, such as the libostfdsp, instead of calling equivalent OSTF functions implemented in the managed (.NET) code.

    ' Log location 


    ' Program location
    Public Property MediaRootDirectory As String = "" '= IO.Path.Combine("C:\", "OSTFMedia") 'Indicates the root path. Other paths given in the project setting files are relative (subpaths) to this path only if they begin with .\ otherwise they are taken as absolute paths.
    Public Property DefaultMediaRootFolderName As String = "OSTFMedia"

    Public Property WasStartedFromVisualStudio As Boolean = False

    Public Property AvailableSpeechMaterialsSubFolder As String = "AvailableSpeechMaterials"
    Public Property CalibrationSignalSubDirectory As String = "CalibrationSignals"
    Public Property AudioSystemSubDirectory As String = "AudioSystem"
    Public Property AudioSystemSettingsFile As String = IO.Path.Combine(AudioSystemSubDirectory, "AudioSystemSpecification.txt")
    Public Property RoomImpulsesSubDirectory As String = "RoomImpulses"
    Public Property AvailableImpulseResponseSetsFile As String = IO.Path.Combine(RoomImpulsesSubDirectory, "AvailableImpulseResponseSets.txt")

    Public Property AvailableSpeechMaterials As New List(Of SpeechMaterialSpecification)

    ''' <summary>
    ''' The SoundPlayer shared between all OSTF applications. Each application that uses it, is responsible of initiating it, with the settings required by the specific application. As well as disposing it when the application is closed.
    ''' </summary>
    Public WithEvents SoundPlayer As Audio.SoundPlayers.iSoundPlayer

    Public Function InitializeSoundPlayer() As Boolean
        SoundPlayer = New Audio.PortAudioVB.PortAudioBasedSoundPlayer(False, False, False, False)
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
    ''' <param name="PlatForm"></param>
    ''' <param name="MediaRootDirectory"></param>
    ''' <param name="MediaPlayerType"></param>
    Public Sub InitializeOSTF(ByVal PlatForm As Platforms, Optional ByVal MediaRootDirectory As String = "", Optional ByVal MediaPlayerType As MediaPlayerTypes = MediaPlayerTypes.Default)

        'Exits the sub to avoid multiple calls (which should be avoided, especially to the Audio.PortAudio.Pa_Initialize function).
        If OstfIsInitialized = True Then Exit Sub
        OstfIsInitialized = True

        'Storing the platform instruction and the MediaPlayerType specified by the calling code 
        CurrentPlatForm = PlatForm
        CurrentMediaPlayerType = MediaPlayerType

        Try

            'Determining which media player type to use
            Select Case CurrentMediaPlayerType
                Case MediaPlayerTypes.Default

                    'Selecting default media player type depending on the current platform
                    Select Case CurrentPlatForm
                        Case Platforms.WinUI, Platforms.UWP
                            'Selects Port Audio based sound player as media player type
                            CurrentMediaPlayerType = MediaPlayerTypes.PaBased
                        Case Platforms.Unknown
                            Throw New Exception("Unable to initialize media player for " & CurrentPlatForm.ToString & " platform type! The application may not work as intended!")
                        Case Else
                            'Selects MAUI community toolkit media element as media player type
                            CurrentMediaPlayerType = MediaPlayerTypes.MctBased
                    End Select

                Case MediaPlayerTypes.PaBased

                    'Checks that the current platform supports type PaBased
                    Select Case CurrentPlatForm
                        Case Platforms.WinUI, Platforms.UWP
                            'WinUI and UWP should work with the current Port Audio based sound player
                        Case Else
                            'Everything else will currently not work
                            Throw New Exception("Unable to initialize media player for " & CurrentPlatForm.ToString & " platform type! The application may not work as intended!")
                    End Select

                Case MediaPlayerTypes.MctBased

                    'MctBased should work on all platforms except possibly "Unknown" ??. Thowing an exception for this,. TODO: Check what happens in the MctBased media player if platform is unknown (hmmm, difficult to check since it would have to be run from an "unknown" platform)
                    Select Case CurrentPlatForm
                        Case Platforms.Unknown
                            Throw New Exception("Unable to initialize media player for " & CurrentPlatForm.ToString & " platform type! The application may not work as intended!")
                    End Select

            End Select

            'If the code runs to this point, a platform along with a supported MediaPlayerType will have been selected.


            'Initiating PortAudio if PaBased MediaPlayerType is used
            If CurrentMediaPlayerType = MediaPlayerTypes.PaBased Then
                'Initializing the port audio library
                If Audio.PortAudio.Pa_GetDeviceCount = Audio.PortAudio.PaError.paNotInitialized Then
                    Dim Pa_Initialize_ReturnValue = Audio.PortAudio.Pa_Initialize
                    If Pa_Initialize_ReturnValue = Audio.PortAudio.PaError.paNoError Then
                        _PortAudioIsInitialized = True
                    Else
                        Throw New Exception("Unable to initialize PortAudio library for audio processing." & vbCrLf & vbCrLf &
                                        "The following error occurred: " & vbCrLf & vbCrLf &
                                        Audio.PortAudio.Pa_GetErrorText(Pa_Initialize_ReturnValue))
                        ' if Pa_Initialize() returns an error code, 
                        ' Pa_Terminate() should NOT be called.
                    End If
                Else
                    'If the we end up here, PortAudio will have already bee initialized, which it should not. (Then there will be missing calls to Pa_Terminate.)
                End If
            End If

            If MediaRootDirectory = "" Then
                Select Case CurrentPlatForm
                    Case Platforms.WinUI, Platforms.UWP
                        ReadMediaRootDirectory()
                    Case Else
                        Throw New ArgumentException("Un non-windows platforms, the MediaFolder argument to InitializeOSTF cannot be left empty!")
                End Select
            Else
                OstfBase.MediaRootDirectory = MediaRootDirectory
            End If

            'Checks that the folder exists
            If IO.Directory.Exists(OstfBase.MediaRootDirectory) = False Then
0:
                Dim NewOSTFMediaFolderDialog As New OSTFMediaFolderDialog
                NewOSTFMediaFolderDialog.InitialPath = OstfBase.MediaRootDirectory
                NewOSTFMediaFolderDialog.Text = "Cannot find the OSTF media folder"
                NewOSTFMediaFolderDialog.StartPosition = Windows.Forms.FormStartPosition.CenterScreen
                NewOSTFMediaFolderDialog.ShowDialog()
            End If

            'Checks that it seems to be the right (media) folder
            If IO.Directory.Exists(IO.Path.Combine(OstfBase.MediaRootDirectory, AudioSystemSubDirectory)) = False Then
                Dim MsgResult = MsgBox("It seems like you have selected an incorrect OSTF media folder. The OSTF media folder should for example contain the folder " & AudioSystemSubDirectory & vbCrLf &
                                    "Please try again.", MsgBoxStyle.OkCancel, "Unable to find the OSTF media folder!")
                If MsgResult = MsgBoxResult.Ok Then
                    GoTo 0
                Else
                    Throw New Exception("Unable to locate the OSTF media folder! Cannot start the application.")
                End If
            End If

            'Initializing the sound player
            InitializeSoundPlayer()

        Catch ex As Exception
            Throw New Exception("The following error occurred when trying to initialize OSTF:" & vbCrLf & vbCrLf & ex.ToString)
        End Try

    End Sub

    Public Sub ReadMediaRootDirectory()

        Dim StartupPath As String = Windows.Forms.Application.StartupPath

        Dim local_settings_FilePath As String = IO.Path.Combine(StartupPath, "local_settings.txt")
        Dim local_settings_Input = IO.File.ReadAllLines(local_settings_FilePath)

        For Each item In local_settings_Input
            If item.Trim = "" Then Continue For
            If item.Trim.StartsWith("//") Then Continue For

            Dim SplitItem = item.Trim.Split("=")
            If SplitItem.Length < 2 Then Continue For

            If SplitItem(0).Trim.ToLower.StartsWith("MediaRootDirectory".ToLower) Then MediaRootDirectory = SplitItem(1).Trim

        Next

        'Determines if the application was started from visual stuido (or possibly manually from the visual stuido debug/release paths)
        If StartupPath.ToLower.Trim.EndsWith("\bin\debug") Or StartupPath.ToLower.Trim.EndsWith("\bin\release") Then
            'The software is likely started from within Visual studio (or manually started from the debug/release folders created by visual studio),
            ' Assuming the OSTF media folder to be located directly in parent folder of the project folder
            WasStartedFromVisualStudio = True
        Else
            'The application is likely not started from within visual studio, assuming the OSTF media folder to be located directly in the start-up folder
            WasStartedFromVisualStudio = False
        End If

        If MediaRootDirectory.Trim = "" Then
            If WasStartedFromVisualStudio = True Then
                ' Assuming the OSTF media folder to be located directly in parent folder of the project folder
                Dim StartupPathSplit = StartupPath.Split(IO.Path.DirectorySeparatorChar).ToList
                Dim ModifiedStartupPath = StartupPathSplit.GetRange(0, StartupPathSplit.Count - 3).ToArray
                MediaRootDirectory = IO.Path.Combine(String.Join(IO.Path.DirectorySeparatorChar, ModifiedStartupPath), DefaultMediaRootFolderName)
            Else
                'The application is likely not started from within visual studio, assuming the OSTF media folder to be located directly in the start-up folder
                MediaRootDirectory = IO.Path.Combine(Windows.Forms.Application.StartupPath, DefaultMediaRootFolderName)
            End If
        End If

    End Sub

    Public Sub StoreMediaRootDirectory(ByVal MediaRootDirectory As String)

        'Storing the MediaRootDirectory to the file local_settings file, for now just overwriting the file since the MediaRootDirectory is its only content
        Dim local_settings_FilePath As String = IO.Path.Combine(Windows.Forms.Application.StartupPath, "local_settings.txt")
        Try
            IO.File.WriteAllText(local_settings_FilePath, "MediaRootDirectory = " & MediaRootDirectory)
        Catch ex As Exception
            MsgBox("Unable to write to the file " & local_settings_FilePath & vbCrLf & vbCrLf & " If it is open in another application, please close it and try again.", MsgBoxStyle.Critical, "An error occurred when attempting to store the media folder path...")
        End Try

    End Sub

    ''' <summary>
    ''' This sub needs to be called when closing the last OSTF application.
    ''' </summary>
    Public Sub TerminateOSTF()

        'Disposing the sound player. 
        If SoundPlayerIsInitialized() = True Then SoundPlayer.Dispose()

        If OstfBase.CurrentMediaPlayerType = MediaPlayerTypes.PaBased Then
            'Terminating Port Audio
            If PortAudioIsInitialized Then Audio.PortAudio.Pa_Terminate()
        End If

    End Sub

    Public Sub LoadAvailableSpeechMaterialSpecifications()

        Dim TestSpecificationFolder As String = IO.Path.Combine(MediaRootDirectory, AvailableSpeechMaterialsSubFolder)

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
        OstfBase.AvailableSpeechMaterials.Clear()

        For Each TextFileName In TextFileNames
            'Ties to use the text file in order to create a new test specification object, and just skipps it if unsuccessful
            Dim NewTestSpecification = SpeechMaterialSpecification.LoadSpecificationFile(TextFileName)
            If NewTestSpecification IsNot Nothing Then
                OstfBase.AvailableSpeechMaterials.Add(NewTestSpecification)
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

        'Reads first all lines and sort them into a player-type specific dictionary
        Dim PlayerTypeDictionary As New SortedList(Of MediaPlayerTypes, String())
        Dim CurrentPlayerType As MediaPlayerTypes? = Nothing

        ''Getting calibration file descriptions from the text file
        Dim InputLines() As String = System.IO.File.ReadAllLines(AudioSystemSpecificationFilePath, System.Text.Encoding.UTF8)
        Dim CurrentSoundPlayerList As List(Of String) = Nothing
        For i = 0 To InputLines.Length - 1
            Dim Line As String = InputLines(i).Trim
            If Line.StartsWith("<New media player>") Then
                If CurrentSoundPlayerList IsNot Nothing Then
                    'Storing the loaded sound player data in PlayerTypeDictionary
                    If PlayerTypeDictionary.ContainsKey(CurrentPlayerType) Then
                        Throw New Exception("In the file " & AudioSystemSpecificationFilePath & " each MediaPlayerType (PaBased, MctBased, etc.) can only be specified once. It seems as the type " & CurrentPlayerType.ToString & " occurres multiple times.")
                    End If
                    PlayerTypeDictionary.Add(CurrentPlayerType, CurrentSoundPlayerList.ToArray)
                End If
                'Creating a new CurrentSoundPlayerList 
                CurrentSoundPlayerList = New List(Of String)
            ElseIf Line.StartsWith("MediaPlayerType") Then
                CurrentPlayerType = InputFileSupport.InputFileEnumValueParsing(Line, GetType(MediaPlayerTypes), AudioSystemSpecificationFilePath, True)
            ElseIf Line.Trim = "" Then
                'Just ignores empty lines
            Else
                If CurrentSoundPlayerList IsNot Nothing Then
                    CurrentSoundPlayerList.Add(Line)
                Else
                    Throw New Exception("The file " & AudioSystemSpecificationFilePath & " must begin with by specfiying a <New media type> line followed by a MediaPlayerType line (e.g. MediaPlayerType = PaBased)")
                End If
            End If
        Next

        If CurrentSoundPlayerList IsNot Nothing Then
            'Also storing the last loaded sound player data in PlayerTypeDictionary
            PlayerTypeDictionary.Add(CurrentPlayerType, CurrentSoundPlayerList.ToArray)
        End If

        Dim PlayerWasLoaded As Boolean = False

        'Looking for and loading the settings for the first player of the intended type
        For Each PlayerType In PlayerTypeDictionary

            'Skipping the player type if it is not the CurrentMediaPlayerType 
            If PlayerType.Key <> OstfBase.CurrentMediaPlayerType Then Continue For

            Try

                Dim MediaPlayerInputLines = PlayerType.Value

                'Reads the API settings, and tries to select the API and device if available, otherwise lets the user select a device manually

                Dim ApiName As String = ""
                Dim OutputDeviceName As String = ""
                Dim OutputDeviceNames As New List(Of String) ' Used for MME multiple device support
                Dim InputDeviceName As String = ""
                Dim InputDeviceNames As New List(Of String) ' Used for MME multiple device support
                Dim BufferSize As Integer = 2048

                Dim LinesRead As Integer = 0
                For i = 0 To MediaPlayerInputLines.Length - 1
                    LinesRead += 1
                    Dim Line As String = MediaPlayerInputLines(i).Trim

                    'Skips empty and outcommented lines
                    If Line = "" Then Continue For
                    If Line.StartsWith("//") Then Continue For

                    If Line.StartsWith("<AudioDevices>") Then
                        'No need to do anything?
                        Continue For
                    End If
                    If Line.StartsWith("<New transducer>") Then Exit For

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
                For i = LinesRead To MediaPlayerInputLines.Length - 1
                    Dim Line As String = MediaPlayerInputLines(i).Trim

                    'Skips empty and outcommented lines
                    If Line = "" Then Continue For
                    If Line.StartsWith("//") Then Continue For

                    If Line.StartsWith("<New transducer>") Then
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

                PlayerWasLoaded = True

                'Exits the loop after the first type successfully read
                Exit For

            Catch ex As Exception
                MsgBox("An error occurred trying to parse the file: " & AudioSystemSpecificationFilePath & vbCrLf & " The application may not work as intended!")
            End Try

        Next

        If PlayerWasLoaded = False Then
            MsgBox("Unable to load any media player with the type " & OstfBase.CurrentMediaPlayerType.ToString & vbCrLf & " The application may not work as intended!")
        End If

    End Sub



    Public Enum SoundPropagationTypes
        PointSpeakers
        SimulatedSoundField
        Ambisonics
    End Enum

    Public Class AudioSystemSpecification
        Public Property Name As String = "Default"
        Public ReadOnly Property MediaPlayerType As OstfBase.MediaPlayerTypes
        Public ReadOnly Property ParentAudioApiSettings As Audio.AudioApiSettings
        Public Property Mixer As Audio.SoundScene.DuplexMixer
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

            'This is the only available for .NET framework, and is therefore hard coded here
            MediaPlayerType = MediaPlayerTypes.PaBased

        End Sub

        Public Sub SetupMixer()
            'Setting up the mixer
            Mixer = New Audio.SoundScene.DuplexMixer(Me)

            CheckCanPlayRecord()

        End Sub
        Public Sub CheckCanPlayRecord()

            If MediaPlayerType = MediaPlayerTypes.PaBased Then
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
            Else
                'Always using True for MediaPlayerTypes.MctBased
                Me._CanPlay = True
                Me._CanRecord = True
            End If

        End Sub

        Public Overrides Function ToString() As String
            If Name <> "" Then
                Return Name
            Else
                Return MyBase.ToString()
            End If
        End Function

        Public Function GetAvailableSoundSourceLocations() As List(Of Audio.SoundScene.SoundSourceLocation)

            Dim Output As New List(Of Audio.SoundScene.SoundSourceLocation)

            For i = 0 To LoudspeakerAzimuths.Count - 1
                Output.Add(New Audio.SoundScene.SoundSourceLocation With {
                           .HorizontalAzimuth = LoudspeakerAzimuths(i),
                           .Elevation = LoudspeakerElevations(i),
                           .Distance = LoudspeakerDistances(i)})
            Next

            Return Output

        End Function


        Public Function NumberOfApiOutputChannels() As Integer

            If MediaPlayerType = MediaPlayerTypes.PaBased Then
                If Me.ParentAudioApiSettings.NumberOfOutputChannels.HasValue = True Then
                    Return Me.ParentAudioApiSettings.NumberOfOutputChannels
                Else
                    Return 0
                End If
            Else
                'Always using two for MediaPlayerTypes.MctBased
                Return 2
            End If

        End Function

        Public Function NumberOfApiInputChannels() As Integer

            If MediaPlayerType = MediaPlayerTypes.PaBased Then
                If Me.ParentAudioApiSettings.NumberOfInputChannels.HasValue = True Then
                    Return Me.ParentAudioApiSettings.NumberOfInputChannels
                Else
                    Return 0
                End If
            Else
                'Always using two for MediaPlayerTypes.MctBased
                Return 2
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
            OutputList.Add(vbCrLf & "Sound device info: " & vbCrLf & ParentAudioApiSettings.ToShorterString)

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
