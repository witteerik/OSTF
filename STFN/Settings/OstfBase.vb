Imports System.Runtime.InteropServices
Imports STFN.Audio
Imports STFN.Audio.AudioManagementExt
Imports STFN.Audio.SoundPlayers

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
        ''' A sound player MediaPlayerType using the Port Audio library
        ''' </summary>
        PaBased
        ''' <summary>
        ''' A sound player MediaPlayerType using Android AudioTrack feature
        ''' </summary>
        AudioTrackBased
        ''' <summary>
        ''' The default media player MediaPlayerType for the specified platform as defined by OSTF.
        ''' </summary>
        [Default]
    End Enum

    Public CurrentMediaPlayerType As MediaPlayerTypes

    'Optimization libraries
    Public Property UseOptimizationLibraries As Boolean = True ' This can be used to determine if C++ libraries should be called, such as the libostfdsp, instead of calling equivalent OSTF functions implemented in the managed (.NET) code.

    'Other basic settings
    Public Property AllowDirectionalSimulation As Boolean = True

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
    ''' The SoundPlayer shared between all STFN applications. Each application that uses it, is responsible of initiating it, with the settings required by the specific application. As well as disposing it when the application is closed.
    ''' </summary>
    Public WithEvents SoundPlayer As Audio.SoundPlayers.iSoundPlayer

    Public Function InitializeSoundPlayer(ByRef SoundPlayer As Audio.SoundPlayers.iSoundPlayer) As Boolean
        OstfBase.SoundPlayer = SoundPlayer
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
    Public Sub InitializeOSTF(ByVal PlatForm As Platforms, ByVal MediaPlayerType As MediaPlayerTypes, Optional ByVal MediaRootDirectory As String = "")

        ''Using this place to debug android optimization library
        'Try

        '    For LL = 14 To 14

        '        Dim Length As Integer = 2 ^ LL
        '        Dim Loops As Integer = 50

        '        Dim x1(Length - 1) As Double
        '        Dim y1(Length - 1) As Double
        '        Dim x2(Length - 1) As Double
        '        Dim y2(Length - 1) As Double
        '        Dim xx(Length - 1) As Double
        '        Dim rnd As New Random(42)

        '        For i = 0 To x1.Length - 1
        '            x1(i) = rnd.NextDouble
        '            x2(i) = x1(i)
        '            xx(i) = x1(i)
        '        Next

        '        Dim Direction = Audio.DSP.TransformationsExt.FftDirections.Backward
        '        Dim TrigDict = Audio.DSP.GetRadix2TrigonomerticValues(Length, Direction)

        '        CurrentPlatForm = Platforms.Android

        '        STFN.Audio.DSP.FftRadix2(x1, y1, Direction)
        '        LibOstfDsp_VB.Fft_complex(x2, y2, x2.Length, Direction)

        '        Dim TimeSpanList As New List(Of String)
        '        Dim StopWatch As New Stopwatch
        '        StopWatch.Start()

        '        For i = 0 To Loops - 1
        '            STFN.Audio.DSP.FftRadix2(x1, y1, Direction)
        '        Next

        '        StopWatch.Stop()
        '        TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
        '        StopWatch.Reset()
        '        StopWatch.Start()

        '        For i = 0 To Loops - 1
        '            LibOstfDsp_VB.Fft_complex(x2, y2, x2.Length, Direction)
        '        Next

        '        StopWatch.Stop()
        '        TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
        '        StopWatch.Reset()

        '        Dim Diffs As Integer = 0
        '        For j = 0 To x1.Length - 1
        '            If (x1(j) = x2(j)) = False Then Diffs += 1
        '        Next

        '        Messager.MsgBox(LL & " " & String.Join(" ", TimeSpanList))

        '    Next

        'Catch ex As Exception
        '    Messager.MsgBox(ex.ToString)
        'End Try

        'Exits the sub to avoid multiple calls (which should be avoided, especially to the Audio.PortAudio.Pa_Initialize function).
        If OstfIsInitialized = True Then Exit Sub
        OstfIsInitialized = True

        'Using optimization libraries if available (currently only on Windows)
        Select Case PlatForm
            Case Platforms.WinUI, Platforms.UWP, Platforms.Android
                UseOptimizationLibraries = True
        End Select

        'Storing the platform instruction and the MediaPlayerType specified by the calling code 
        CurrentPlatForm = PlatForm
        CurrentMediaPlayerType = MediaPlayerType

        Try

            'Determining which media player MediaPlayerType to use
            Select Case CurrentMediaPlayerType
                Case MediaPlayerTypes.Default

                    'Selecting default media player MediaPlayerType depending on the current platform
                    Select Case CurrentPlatForm
                        Case Platforms.WinUI, Platforms.UWP
                            'Selects Port Audio based sound player as media player MediaPlayerType
                            CurrentMediaPlayerType = MediaPlayerTypes.PaBased
                        Case Platforms.Android
                            'Selects MAUI community toolkit media element as media player MediaPlayerType
                            CurrentMediaPlayerType = MediaPlayerTypes.AudioTrackBased
                        Case Platforms.Unknown
                            Throw New Exception("Unable to initialize media player for " & CurrentPlatForm.ToString & " platform MediaPlayerType! The application may not work as intended!")
                        Case Else
                            Throw New Exception("There is no supported OSTF sound player for the " & CurrentPlatForm.ToString & " platform! The application may not work as intended!")
                    End Select

                Case MediaPlayerTypes.PaBased

                    'Checks that the current platform supports MediaPlayerType PaBased
                    Select Case CurrentPlatForm
                        Case Platforms.WinUI, Platforms.UWP
                            'WinUI and UWP should work with the Port Audio based sound player
                        Case Else
                            'Everything else will currently not work
                            Throw New Exception("Unable to initialize media player for " & CurrentPlatForm.ToString & " platform MediaPlayerType! The application may not work as intended!")
                    End Select

                Case MediaPlayerTypes.AudioTrackBased

                    'Checks that the current platform supports the selected MediaPlayerType
                    Select Case CurrentPlatForm
                        Case Platforms.Android
                            'Android should work with the AudioTrack based sound player
                        Case Else
                            'Everything else will currently not work
                            Throw New Exception("Unable to initialize media player for " & CurrentPlatForm.ToString & " platform MediaPlayerType! The application may not work as intended!")
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

            'Sets the MediaRootDirectory 
            OstfBase.MediaRootDirectory = MediaRootDirectory

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
            If OstfBase.CurrentMediaPlayerType = MediaPlayerTypes.PaBased Then
                Dim SoundPlayer As iSoundPlayer = New Audio.PortAudioVB.PortAudioBasedSoundPlayer(False, False, False, False)
                InitializeSoundPlayer(SoundPlayer)
            End If

            'Note that the AudioTrack based player need to be initialized in the STFM library (as it does not exist in STFN)

        Catch ex As Exception
            Throw New Exception("The following error occurred when trying to initialize OSTF:" & vbCrLf & vbCrLf & ex.ToString)
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

        Dim AudioSystemSpecificationFilePath = Utils.NormalizeCrossPlatformPath(IO.Path.Combine(OstfBase.MediaRootDirectory, OstfBase.AudioSystemSettingsFile))

        'Reads first all lines and sort them into a player-MediaPlayerType specific dictionary
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
                        Throw New Exception("In the file " & AudioSystemSpecificationFilePath & " each MediaPlayerType (PaBased, AudioTrackBased, etc.) can only be specified once. It seems as the MediaPlayerType " & CurrentPlayerType.ToString & " occurres multiple times.")
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
                    Throw New Exception("The file " & AudioSystemSpecificationFilePath & " must begin with by specfiying a <New media MediaPlayerType> line followed by a MediaPlayerType line (e.g. MediaPlayerType = PaBased)")
                End If
            End If
        Next

        If CurrentSoundPlayerList IsNot Nothing Then
            'Also storing the last loaded sound player data in PlayerTypeDictionary
            PlayerTypeDictionary.Add(CurrentPlayerType, CurrentSoundPlayerList.ToArray)
        End If

        Dim PlayerWasLoaded As Boolean = False

        'Looking for and loading the settings for the first player of the intended MediaPlayerType
        For Each PlayerType In PlayerTypeDictionary

            'Skipping the player MediaPlayerType if it is not the CurrentMediaPlayerType 
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
                Dim AllowDefaultOutputDevice As Boolean? = Nothing

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

                    If OstfBase.CurrentMediaPlayerType = MediaPlayerTypes.PaBased Then
                        If Line.StartsWith("ApiName") Then ApiName = InputFileSupport.GetInputFileValue(Line, True)
                    End If

                    If Line.Replace(" ", "").StartsWith("OutputDevice=") Then OutputDeviceName = InputFileSupport.GetInputFileValue(Line, True)
                    If Line.Replace(" ", "").StartsWith("OutputDevices=") Then OutputDeviceNames = InputFileSupport.InputFileListOfStringParsing(Line, False, True)
                    If Line.Replace(" ", "").StartsWith("InputDevice=") Then InputDeviceName = InputFileSupport.GetInputFileValue(Line, True)
                    If Line.Replace(" ", "").StartsWith("InputDevices=") Then InputDeviceNames = InputFileSupport.InputFileListOfStringParsing(Line, False, True)
                    If Line.StartsWith("BufferSize") Then BufferSize = InputFileSupport.InputFileIntegerValueParsing(Line, True, AudioSystemSpecificationFilePath)
                    If Line.StartsWith("AllowDefaultOutputDevice") Then AllowDefaultOutputDevice = InputFileSupport.InputFileBooleanValueParsing(Line, True, AudioSystemSpecificationFilePath)

                Next

                Dim AudioSettings As Audio.AudioSettings = Nothing

                If OstfBase.CurrentMediaPlayerType = MediaPlayerTypes.PaBased Then

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

                    'Tries to setup the PortAudioApiSettings using the loaded data
                    AudioSettings = New Audio.PortAudioApiSettings

                    If AllowDefaultOutputDevice Is Nothing Then
                        DeviceLoadSuccess = False
                        MsgBox("The AllowDefaultOutputDevice behaviour must be specified in the file " & AudioSystemSpecificationFilePath & "!" & vbCrLf & vbCrLf &
                               "Use either:" & vbCrLf & "AllowDefaultOutputDevice = True" & vbCrLf & "or" & vbCrLf & "AllowDefaultOutputDevice = False" & vbCrLf & vbCrLf &
                               "Press OK to close the application.", MsgBoxStyle.Exclamation, "Sound device specification error!")
                        Messager.RequestCloseApp()
                    End If

                    AudioSettings.AllowDefaultOutputDevice = AllowDefaultOutputDevice

                    If DeviceLoadSuccess = True Then
                        If ApiName = "ASIO" Then
                            DeviceLoadSuccess = DirectCast(AudioSettings, PortAudioApiSettings).SetAsioSoundDevice(OutputDeviceName, BufferSize)
                        Else
                            If OutputDeviceNames Is Nothing And InputDeviceNames Is Nothing Then
                                DeviceLoadSuccess = DirectCast(AudioSettings, PortAudioApiSettings).SetNonAsioSoundDevice(ApiName, OutputDeviceName, InputDeviceName, BufferSize)
                            Else
                                DeviceLoadSuccess = DirectCast(AudioSettings, PortAudioApiSettings).SetMmeMultipleDevices(InputDeviceNames, OutputDeviceNames, BufferSize)
                            End If
                        End If
                    End If

                    If DeviceLoadSuccess = False Then

                        If OutputDeviceNames Is Nothing Then OutputDeviceNames = New List(Of String)
                        If InputDeviceNames Is Nothing Then InputDeviceNames = New List(Of String)

                        'MsgBox("The Open Speech Test Framework (OSTF) was unable to load the sound API (" & ApiName & ") and device/s indicated in the file " & AudioSystemSpecificationFilePath & vbCrLf & vbCrLf &
                        '    "Output device: " & OutputDeviceName & vbCrLf &
                        '    "Output devices: " & String.Join(", ", OutputDeviceNames) & vbCrLf &
                        '    "Input device: " & InputDeviceName & vbCrLf &
                        '    "Input devices: " & String.Join(", ", InputDeviceNames) & vbCrLf & vbCrLf &
                        '    "Click OK to manually select audio input/output devices." & vbCrLf & vbCrLf &
                        '    "IMPORTANT: Sound tranducer calibration and/or routing may not be correct when manually selected sound devices are used!", MsgBoxStyle.Exclamation, "OSTF sound device not found!")

                        'Using default settings, as there is not yet any GUI for selecting settings such as the .NET Framework AudioSettingsDialog 

                        'MsgBox("The Open Speech Test Framework (OSTF) was unable to load the sound API (" & ApiName & ") and device/s indicated in the file " & AudioSystemSpecificationFilePath & vbCrLf & vbCrLf &
                        '"Output device: " & OutputDeviceName & vbCrLf &
                        '"Output devices: " & String.Join(", ", OutputDeviceNames) & vbCrLf &
                        '"Input device: " & InputDeviceName & vbCrLf &
                        '"Input devices: " & String.Join(", ", InputDeviceNames) & vbCrLf & vbCrLf &
                        '"Click OK to select the default input/output devices." & vbCrLf & vbCrLf &
                        '"IMPORTANT: Sound tranducer calibration and/or routing may not be correct!", MsgBoxStyle.Exclamation, "OSTF sound device not found!")

                        'Dim NewAudioSettingsDialog As New AudioSettingsDialog()
                        'Dim AudioSettingsDialogResult = NewAudioSettingsDialog.ShowDialog()
                        'If AudioSettingsDialogResult = Windows.Forms.DialogResult.OK Then
                        '    PortAudioApiSettings = NewAudioSettingsDialog.CurrentAudioApiSettings
                        'Else
                        '    MsgBox("You pressed cancel. Default sound settings will be used", MsgBoxStyle.Exclamation, "Select sound device!")

                        If AllowDefaultOutputDevice = True Then
                            MsgBox("The Open Speech Test Framework (OSTF) was unable to load the sound API (" & ApiName & ") and device/s indicated in the file " & AudioSystemSpecificationFilePath & vbCrLf & vbCrLf &
                        "Output device: " & OutputDeviceName & vbCrLf &
                        "Output devices: " & String.Join(", ", OutputDeviceNames) & vbCrLf &
                        "Input device: " & InputDeviceName & vbCrLf &
                        "Input devices: " & String.Join(", ", InputDeviceNames) & vbCrLf & vbCrLf &
                        "Click OK to use the default input/output devices." & vbCrLf & vbCrLf &
                        "IMPORTANT: Sound tranducer calibration and/or routing may not be correct!", MsgBoxStyle.Exclamation, "OSTF sound device not found!")

                            DirectCast(AudioSettings, PortAudioApiSettings).SelectDefaultAudioDevice()
                        Else
                            MsgBox("Selecting default device for PaBased sound players has been disabled in the audio system specifications file " & AudioSystemSpecificationFilePath & vbCrLf & vbCrLf & "Press OK to close the application.")
                            Messager.RequestCloseApp()
                        End If
                        'End If
                    End If

                ElseIf OstfBase.CurrentMediaPlayerType = MediaPlayerTypes.AudioTrackBased Then

                    Dim DeviceLoadSuccess As Boolean = True
                    If OutputDeviceName = "" Then
                        'No device names have been specified
                        MsgBox("An output device must be specified for the android AudioTrack based player in the file " & AudioSystemSpecificationFilePath & "!", MsgBoxStyle.Exclamation, "Sound device specification error!")
                        DeviceLoadSuccess = False
                    End If

                    'No input device is required
                    'If InputDeviceName = "" Then
                    '    DeviceLoadSuccess = False
                    'MsgBox("An input device must be specified for the android AudioTrack based player in the file " & AudioSystemSpecificationFilePath & "!", MsgBoxStyle.Exclamation, "Sound device specification error!")
                    'End If

                    'Setting up the player must be done in STFM as there is no access to Android AudioTrack in STFN, however the object holding the AudioSettings must be created here as it needs to be referenced in the Transducers below
                    AudioSettings = New Audio.AndroidAudioTrackPlayerSettings
                    AudioSettings.AllowDefaultOutputDevice = AllowDefaultOutputDevice
                    DirectCast(AudioSettings, AndroidAudioTrackPlayerSettings).SelectedOutputDeviceName = OutputDeviceName
                    DirectCast(AudioSettings, AndroidAudioTrackPlayerSettings).SelectedInputDeviceName = InputDeviceName
                    AudioSettings.FramesPerBuffer = BufferSize

                Else
                    Throw New NotImplementedException("Unknown media player MediaPlayerType specified in the file " & AudioSystemSpecificationFilePath)
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
                            CurrentTransducer = New AudioSystemSpecification(OstfBase.CurrentMediaPlayerType, AudioSettings)
                        Else
                            'Stores the transducer
                            _AvaliableTransducers.Add(CurrentTransducer)
                            'Creates a new one
                            CurrentTransducer = New AudioSystemSpecification(OstfBase.CurrentMediaPlayerType, AudioSettings)
                        End If
                    End If

                    If Line.StartsWith("Name") Then CurrentTransducer.Name = InputFileSupport.GetInputFileValue(Line, True)
                    If Line.StartsWith("LoudspeakerAzimuths") Then CurrentTransducer.LoudspeakerAzimuths = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
                    If Line.StartsWith("LoudspeakerElevations") Then CurrentTransducer.LoudspeakerElevations = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
                    If Line.StartsWith("LoudspeakerDistances") Then CurrentTransducer.LoudspeakerDistances = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
                    If Line.StartsWith("HardwareOutputChannels") Then CurrentTransducer.HardwareOutputChannels = InputFileSupport.InputFileListOfIntegerParsing(Line, True, AudioSystemSpecificationFilePath)
                    If Line.StartsWith("CalibrationGain") Then CurrentTransducer.CalibrationGain = InputFileSupport.InputFileListOfDoubleParsing(Line, True, AudioSystemSpecificationFilePath)
                    If Line.StartsWith("HostVolumeOutputLevel") Then CurrentTransducer.HostVolumeOutputLevel = InputFileSupport.InputFileDoubleValueParsing(Line, True, AudioSystemSpecificationFilePath)
                    If Line.StartsWith("LimiterThreshold") Then CurrentTransducer.LimiterThreshold = InputFileSupport.InputFileDoubleValueParsing(Line, True, AudioSystemSpecificationFilePath)

                Next

                'Stores the last transducer
                If CurrentTransducer IsNot Nothing Then _AvaliableTransducers.Add(CurrentTransducer)

                'Adding a default transducer if none were sucessfully read
                If _AvaliableTransducers.Count = 0 Then _AvaliableTransducers.Add(New AudioSystemSpecification(OstfBase.CurrentMediaPlayerType, AudioSettings))

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

                'Exits the loop after the first MediaPlayerType successfully read
                Exit For

            Catch ex As Exception
                MsgBox("An error occurred trying to parse the file: " & AudioSystemSpecificationFilePath & vbCrLf & " The application may not work as intended!")
            End Try

        Next

        If PlayerWasLoaded = False Then
            MsgBox("Unable to load any media player with the MediaPlayerType " & OstfBase.CurrentMediaPlayerType.ToString & vbCrLf & " The application may not work as intended!")
        End If

    End Sub


    Private Sub LoadAudioSystemSpecificationFile_OLD()

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

        'Tries to setup the PortAudioApiSettings using the loaded data
        Dim AudioApiSettings As New Audio.PortAudioApiSettings
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
            '    PortAudioApiSettings = NewAudioSettingsDialog.CurrentAudioApiSettings
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
                    CurrentTransducer = New AudioSystemSpecification(CurrentMediaPlayerType, AudioApiSettings)
                Else
                    'Stores the transducer
                    _AvaliableTransducers.Add(CurrentTransducer)
                    'Creates a new one
                    CurrentTransducer = New AudioSystemSpecification(CurrentMediaPlayerType, AudioApiSettings)
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
        If _AvaliableTransducers.Count = 0 Then _AvaliableTransducers.Add(New AudioSystemSpecification(CurrentMediaPlayerType, AudioApiSettings))

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
        Public ReadOnly Property MediaPlayerType As OstfBase.MediaPlayerTypes
        'Public ReadOnly Property ParentAudioApiSettings As Audio.PortAudioApiSettings

        Public ReadOnly Property ParentAudioApiSettings As Audio.AudioSettings


        Public Property Mixer As STFN.Audio.SoundScene.DuplexMixer
        Public Property LoudspeakerAzimuths As New List(Of Double) From {-90, 90}
        Public Property LoudspeakerElevations As New List(Of Double) From {0, 0}
        Public Property LoudspeakerDistances As New List(Of Double) From {0, 0}
        Public Property HardwareOutputChannels As New List(Of Integer) From {1, 2}
        Public Property CalibrationGain As New List(Of Double) From {0, 0}

        Private _HostVolumeOutputLevel As Integer? = Nothing
        ''' <summary>
        ''' If possible, this value should be used to set and maintain the volume of the selected output sound unit (e.g. sound card) during speech tests. Value represent percentages (0-100).
        ''' </summary>
        ''' <returns></returns>
        Public Property HostVolumeOutputLevel As Integer?
            Get
                Return _HostVolumeOutputLevel
            End Get
            Set(value As Integer?)
                If value.HasValue Then
                    _HostVolumeOutputLevel = Math.Clamp(value.Value, 0, 100)
                Else
                    _HostVolumeOutputLevel = Nothing
                End If
            End Set
        End Property
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

        Public Sub New(ByVal MediaPlayerType As OstfBase.MediaPlayerTypes, Optional ByRef ParentAudioApiSettings As Audio.AudioSettings = Nothing)

            Me.MediaPlayerType = MediaPlayerType

            Select Case MediaPlayerType
                Case MediaPlayerTypes.PaBased, MediaPlayerTypes.AudioTrackBased
                    If ParentAudioApiSettings Is Nothing Then
                        Throw New ArgumentException("The argument ParentAudioApiSettings cannot be Nothing when the media player MediaPlayerType is PaBased or AudioTrackBased!")
                    Else
                        Me.ParentAudioApiSettings = ParentAudioApiSettings
                    End If
                Case MediaPlayerTypes.Default
                    Throw New ArgumentException("The argument ParentAudioApiSettings cannot be Default when initiating an AudioSystemSpecification!")
            End Select

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
                'Always using True for MediaPlayerTypes.AudioTrackBased
                Me._CanPlay = True
                Me._CanRecord = True
            End If

        End Sub

        Public Sub OverrideCanPlay(ByVal Value As Boolean)
            _CanPlay = Value
        End Sub

        Public Function NumberOfApiOutputChannels() As Integer

            If MediaPlayerType = MediaPlayerTypes.PaBased Then
                If Me.ParentAudioApiSettings.NumberOfOutputChannels.HasValue = True Then
                    Return Me.ParentAudioApiSettings.NumberOfOutputChannels
                Else
                    Return 0
                End If
            Else
                'Always using two for MediaPlayerTypes.AudioTrackBased
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
                'Always using two for MediaPlayerTypes.AudioTrackBased
                Return 2
            End If

        End Function

        ''' <summary>
        ''' Checks to see if the transducers connected to HardWareChannelLeft and HardWareChannelRight are headphones.
        ''' </summary>
        ''' <param name="HardWareChannelLeft"></param>
        ''' <param name="HardWareChannelRight"></param>
        ''' <returns></returns>
        Public Function IsHeadphones(Optional ByVal HardWareChannelLeft As Integer = 1, Optional ByVal HardWareChannelRight As Integer = 2) As Boolean

            'Returns false if both channels are the same
            If HardWareChannelLeft = HardWareChannelRight Then Return False

            'Checks that the specified hardware channels exist in the current instance of AudioSystemSpecification
            Dim LeftChannelIndex = HardwareOutputChannels.IndexOf(HardWareChannelLeft)
            If LeftChannelIndex = -1 Then Return False

            Dim RightChannelIndex = HardwareOutputChannels.IndexOf(HardWareChannelRight)
            If RightChannelIndex = -1 Then Return False

            'Requres azimuths to be -90 and 90
            If LoudspeakerAzimuths(LeftChannelIndex) <> -90 Then Return False
            If LoudspeakerAzimuths(RightChannelIndex) <> 90 Then Return False

            'Requires distance to be 0
            If LoudspeakerDistances(LeftChannelIndex) <> 0 Then Return False
            If LoudspeakerDistances(RightChannelIndex) <> 0 Then Return False

            'Requires elevation to be 0
            If LoudspeakerElevations(LeftChannelIndex) <> 0 Then Return False
            If LoudspeakerElevations(RightChannelIndex) <> 0 Then Return False

            'All checks passed, it is headphones
            Return True

        End Function

        Public Function GetVisualSoundSourceLocations() As List(Of Audio.SoundScene.VisualSoundSourceLocation)

            'Adding the appropriate sound sources into the selection views, based on the selected transducer
            Dim SignalLocationCandidateList As New List(Of Audio.SoundScene.VisualSoundSourceLocation)

            'Adding real speaker locations as selectable sound source candidates
            For s = 0 To LoudspeakerAzimuths.Count - 1
                SignalLocationCandidateList.Add(New Audio.SoundScene.VisualSoundSourceLocation(New Audio.SoundScene.SoundSourceLocation With {
                .HorizontalAzimuth = LoudspeakerAzimuths(s),
                .Elevation = LoudspeakerElevations(s),
                .Distance = LoudspeakerDistances(s)}))
            Next

            Return SignalLocationCandidateList

        End Function

        Public Function GetSoundSourceLocations() As List(Of Audio.SoundScene.SoundSourceLocation)

            'Adding the appropriate sound sources into the selection views, based on the selected transducer
            Dim SignalLocationCandidateList As New List(Of Audio.SoundScene.SoundSourceLocation)

            'Adding real speaker locations as selectable sound source candidates
            For s = 0 To LoudspeakerAzimuths.Count - 1
                SignalLocationCandidateList.Add(New Audio.SoundScene.SoundSourceLocation With {
                .HorizontalAzimuth = LoudspeakerAzimuths(s),
                .Elevation = LoudspeakerElevations(s),
                .Distance = LoudspeakerDistances(s)})
            Next

            Return SignalLocationCandidateList

        End Function


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
            If AllowDirectionalSimulation = True Then
                If _DirectionalSimulator Is Nothing Then _DirectionalSimulator = New DirectionalSimulation()
                Return _DirectionalSimulator
            Else
                Return Nothing
            End If
        End Get
    End Property

End Module
