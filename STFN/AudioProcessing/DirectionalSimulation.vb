

Imports System.Numerics
Imports MathNet.Numerics
Imports STFN.BinauralImpulseReponseSet
Imports STFN.OstfBase

Public Class DirectionalSimulation

    Private BinauralImpulseReponseSets As New SortedList(Of String, BinauralImpulseReponseSet)

    Public Sub New()

        'Getting the path for the available impulse response sets
        Dim AvailableIrSets = IO.File.ReadAllLines(IO.Path.Combine(MediaRootDirectory, OstfBase.AvailableImpulseResponseSetsFile), Text.Encoding.UTF8)

        For i = 0 To AvailableIrSets.Length - 1

            If AvailableIrSets(i).Trim = "" Then Continue For
            If AvailableIrSets(i).Trim.StartsWith("//") Then Continue For
            Dim ImpulseResponseSetSpecificationFile = Utils.NormalizeCrossPlatformPath(IO.Path.Combine(OstfBase.MediaRootDirectory, OstfBase.RoomImpulsesSubDirectory, AvailableIrSets(i)))
            If IO.File.Exists(ImpulseResponseSetSpecificationFile) = False Then Continue For

            Dim NewBinauralImpulseReponseSet = New BinauralImpulseReponseSet(ImpulseResponseSetSpecificationFile)

            If NewBinauralImpulseReponseSet IsNot Nothing Then
                If NewBinauralImpulseReponseSet.Name <> "" Then
                    If BinauralImpulseReponseSets.ContainsKey(NewBinauralImpulseReponseSet.Name) = False Then
                        BinauralImpulseReponseSets.Add(NewBinauralImpulseReponseSet.Name, NewBinauralImpulseReponseSet)
                    Else
                        MsgBox("Detected more than one ImpulseReponseSets with the name: " & NewBinauralImpulseReponseSet.Name & ". This is not allowed. Only the first loaded set will be used.", MsgBoxStyle.Exclamation, "Multiple impulse reponse sets with identical names!")
                    End If
                End If
            End If
        Next

    End Sub


    ''' <summary>
    ''' Searches the impulse response set and returns the binaural impulse response available closest the the indicated azimuth, elevation and distance.
    ''' </summary>
    ''' <param name="Azimuth">The azimuth (phi)</param>
    ''' <param name="Elevation">The elevation (or inclination, theta), relative to the horizontal plane</param>
    ''' <param name="Distance">The distance (or radius, r)</param>
    ''' <returns>Returns a tuple containing the selected 3D-point and the corresponding binaural impulse response.</returns>
    Public Function GetStereoKernel(ByVal ImpulseReponseSetName As String, ByVal Azimuth As Double, ByVal Elevation As Double, ByVal Distance As Double) As StereoKernel

        If BinauralImpulseReponseSets.ContainsKey(ImpulseReponseSetName) = False Then
            Throw New Exception("Unable to locate the ImpulseReponseSet with the name: " & ImpulseReponseSetName)
        End If

        Return BinauralImpulseReponseSets(ImpulseReponseSetName).GetClosestPoint(Azimuth, Elevation, Distance)

    End Function

    Public Function GetAvailableDirectionalSimulationSetNames(ByVal SampleRate As Integer) As List(Of String)
        Dim OutputList As New List(Of String)
        For Each BinauralImpulseReponseSet In BinauralImpulseReponseSets
            If BinauralImpulseReponseSet.Value.SampleRate = SampleRate Then
                OutputList.Add(BinauralImpulseReponseSet.Value.Name)
            End If
        Next
        Return OutputList
    End Function

    Public Function GetAvailableDirectionalSimulationSetNames() As List(Of String)
        Dim OutputList As New List(Of String)
        For Each BinauralImpulseReponseSet In BinauralImpulseReponseSets
            OutputList.Add(BinauralImpulseReponseSet.Value.Name)
        Next
        Return OutputList
    End Function

    Public Function GetAvailableDirectionalSimulationSetDistances(ByVal DirectionalSimulationSetName As String) As SortedSet(Of Double)
        Dim OutputList As New SortedSet(Of Double)
        If BinauralImpulseReponseSets.ContainsKey(DirectionalSimulationSetName) Then
            Return BinauralImpulseReponseSets(DirectionalSimulationSetName).GetAllOccurringDistances
        End If
        Return OutputList
    End Function


    Public Function GetAvailableDirectionalSimulationSets(ByRef SelectedTransducer As AudioSystemSpecification, ByVal SampleRate As Integer) As List(Of String)

        Dim OutputList As New List(Of String)

        'Requires -90 and 90 (i.e. left and right) loudspeakers
        'TODO, other requirements?? such as elevation of 0, distance 0 etc?
        If SelectedTransducer.LoudspeakerAzimuths.Contains(-90) And SelectedTransducer.LoudspeakerAzimuths.Contains(90) Then
            Return DirectionalSimulator.GetAvailableDirectionalSimulationSetNames(SampleRate)
        End If

        Return OutputList

    End Function

    Public Function GetAvailableDirectionalSimulationSets(ByRef SelectedTransducer As AudioSystemSpecification) As List(Of String)

        Dim OutputList As New List(Of String)

        'Requires -90 and 90 (i.e. left and right) loudspeakers
        'TODO, other requirements?? such as elevation of 0, distance 0 etc?
        If SelectedTransducer.LoudspeakerAzimuths.Contains(-90) And SelectedTransducer.LoudspeakerAzimuths.Contains(90) Then
            Return DirectionalSimulator.GetAvailableDirectionalSimulationSetNames()
        End If

        Return OutputList

    End Function

    ''' <summary>
    ''' Attempts to set the SelectedDirectionalSimulationSet to DirectionalSimulationSetName based  on the SelectedTransducer and SampleRate. Returns True if success, and False if not possible.
    ''' </summary>
    ''' <param name="DirectionalSimulationSetName"></param>
    ''' <param name="SampleRate"></param>
    ''' <returns></returns>
    Public Function TrySetSelectedDirectionalSimulationSet(ByVal DirectionalSimulationSetName As String, ByRef SelectedTransducer As AudioSystemSpecification, ByVal SampleRate As Integer) As Boolean

        Dim AvailableSets = GetAvailableDirectionalSimulationSets(SelectedTransducer, SampleRate)
        If AvailableSets.Contains(DirectionalSimulationSetName) Then
            Me._SelectedDirectionalSimulationSetName = DirectionalSimulationSetName
            Return True
        End If
        Return False
    End Function

    ''' <summary>
    ''' Attempts to set the SelectedDirectionalSimulationSet to DirectionalSimulationSetName based  on the SelectedTransducer. Returns True if success, and False if not possible.
    ''' </summary>
    ''' <param name="DirectionalSimulationSetName"></param>
    ''' <returns></returns>
    Public Function TrySetSelectedDirectionalSimulationSet(ByVal DirectionalSimulationSetName As String, ByRef SelectedTransducer As AudioSystemSpecification) As Boolean

        Dim AvailableSets = GetAvailableDirectionalSimulationSets(SelectedTransducer)
        If AvailableSets.Contains(DirectionalSimulationSetName) Then
            Me._SelectedDirectionalSimulationSetName = DirectionalSimulationSetName
            Return True
        End If
        Return False
    End Function

    ''' <summary>
    ''' Clears the SelectedDirectionalSimulationSet.
    ''' </summary>
    Public Sub ClearSelectedDirectionalSimulationSet()
        Me._SelectedDirectionalSimulationSetName = ""
    End Sub


    Private _SelectedDirectionalSimulationSetName As String = ""

    ''' <summary>
    ''' Hold the name of the currently selected directional simulation set
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property SelectedDirectionalSimulationSetName As String
        Get
            Return _SelectedDirectionalSimulationSetName
        End Get
    End Property

    Public Function IsActive() As Boolean
        If SelectedDirectionalSimulationSetName = "" Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Function GetAllDirectionalSimulationSets() As SortedList(Of String, BinauralImpulseReponseSet)

        Return BinauralImpulseReponseSets

    End Function


End Class

Public Class StereoKernel
    Public Name As String
    Public Point As Point3D
    Public BinauralIR As Audio.Sound
    Public BinauralDelay As New BinauralDelay

    Private _ZeroPhaseEquivalent As StereoKernel = Nothing
    Public ReadOnly Property ZeroPhaseEquivalent() As StereoKernel
        Get

            If _ZeroPhaseEquivalent Is Nothing Then
                CalculateZeroPhaseEquivalentKernel()
            End If
            Return _ZeroPhaseEquivalent

        End Get
    End Property

    Private _TwoLeftEarsKernel As StereoKernel = Nothing
    Public ReadOnly Property TwoLeftEarsKernel() As StereoKernel
        Get

            If _TwoLeftEarsKernel Is Nothing Then
                CreateTwoSameEarsKernels()
            End If
            Return _TwoLeftEarsKernel

        End Get
    End Property

    Private _TwoRightEarsKernel As StereoKernel = Nothing
    Public ReadOnly Property TwoRightEarsKernel() As StereoKernel
        Get

            If _TwoRightEarsKernel Is Nothing Then
                CreateTwoSameEarsKernels()
            End If
            Return _TwoRightEarsKernel

        End Get
    End Property

    Private Function GetExportName()
        Return Name.Replace("<", "_").Replace(">", "_").Replace(",", "_").Replace(" ", "_")
    End Function

    Public Sub CreateTwoSameEarsKernels()

        'Creating a new stereo kernel, with references to some object in the parent kernel
        _TwoLeftEarsKernel = New StereoKernel With {.Name = Me.Name & "_2LE", .Point = Me.Point, .BinauralIR = New Audio.Sound(BinauralIR.WaveFormat), .BinauralDelay = New BinauralDelay With {.LeftDelay = Me.BinauralDelay.LeftDelay, .RightDelay = Me.BinauralDelay.LeftDelay}}
        _TwoRightEarsKernel = New StereoKernel With {.Name = Me.Name & "_2RE", .Point = Me.Point, .BinauralIR = New Audio.Sound(BinauralIR.WaveFormat), .BinauralDelay = New BinauralDelay With {.LeftDelay = Me.BinauralDelay.RightDelay, .RightDelay = Me.BinauralDelay.RightDelay}}

        'Creating two copies (not references) of the audio data in the BinauralIR
        Dim TempIrCopy1 = BinauralIR.CreateSoundDataCopy
        Dim TempIrCopy2 = BinauralIR.CreateSoundDataCopy

        'And puts the left side data of both copies into the BinauralIR of the _TwoLeftEarsKernel
        _TwoLeftEarsKernel.BinauralIR.WaveData.SampleData(1) = TempIrCopy1.WaveData.SampleData(1)
        _TwoLeftEarsKernel.BinauralIR.WaveData.SampleData(2) = TempIrCopy2.WaveData.SampleData(1)

        'And puts the right side data of both copies into the BinauralIR of the _TwoRightEarsKernel
        _TwoRightEarsKernel.BinauralIR.WaveData.SampleData(1) = TempIrCopy1.WaveData.SampleData(2)
        _TwoRightEarsKernel.BinauralIR.WaveData.SampleData(2) = TempIrCopy2.WaveData.SampleData(2)

    End Sub

    Private Sub CalculateZeroPhaseEquivalentKernel()

        'Gets the wave format
        Dim MonoWaveFormat = New Audio.Formats.WaveFormat(BinauralIR.WaveFormat.SampleRate, BinauralIR.WaveFormat.BitDepth, 1,, BinauralIR.WaveFormat.Encoding)

        'Gets a copy of the BinauralIR
        Dim TempIrCopy = BinauralIR.CreateSoundDataCopy

        'Creating a new stereo kernel, with references to some object in the parent kernel
        _ZeroPhaseEquivalent = New StereoKernel With {.Name = Me.Name & "_ZPE", .Point = Me.Point, .BinauralIR = New Audio.Sound(BinauralIR.WaveFormat)}

        For Channel = 1 To 2

            Dim IrChannelCopy As New Audio.Sound(MonoWaveFormat)

            IrChannelCopy.WaveData.SampleData(1) = TempIrCopy.WaveData.SampleData(Channel)

            Dim KernelSize = IrChannelCopy.WaveData.SampleData(1).Length

            Dim ZeroPhaseIR = Audio.GenerateSound.GetImpulseResponseFromSound(IrChannelCopy, New Audio.Formats.FftFormat(2 ^ 12), KernelSize * 2)

            'Use both to filter a sound and add gain to ZeroPhaseIR based on the resulting level difference

            'Notes the length of the IR
            Dim IrLength As Integer = IrChannelCopy.WaveData.SampleData(1).Length

            'Creates a delta pulse measurement sound
            Dim DeltaPulse_TestSound = Audio.GenerateSound.CreateSilence(MonoWaveFormat,, IrLength * 5, Audio.BasicAudioEnums.TimeUnits.samples)
            DeltaPulse_TestSound.WaveData.SampleData(1)(IrLength * 2) = 1

            'Measures the pre filter level
            Dim PreLevel As Double = Audio.DSP.MeasureSectionLevel(DeltaPulse_TestSound, 1, IrLength, 3 * IrLength)

            'Runs convolution
            Dim IR_ConvolutedSound = Audio.DSP.FIRFilter(DeltaPulse_TestSound, IrChannelCopy, New Audio.Formats.FftFormat, ,,,,, True)
            Dim ZP_IR_ConvolutedSound = Audio.DSP.FIRFilter(DeltaPulse_TestSound, ZeroPhaseIR, New Audio.Formats.FftFormat, ,,,,, True)

            'Gets the post-convolution level
            Dim PostLevel_IR = Audio.DSP.MeasureSectionLevel(IR_ConvolutedSound, 1, IrLength, 3 * IrLength)
            Dim PostLevel_ZP_IR = Audio.DSP.MeasureSectionLevel(ZP_IR_ConvolutedSound, 1, IrLength, 3 * IrLength)

            'Calculates the gain
            Dim IR_FilterGain As Double = PostLevel_IR - PreLevel
            Dim ZP_IR_FilterGain As Double = PostLevel_ZP_IR - PreLevel

            Dim Difference = ZP_IR_FilterGain - IR_FilterGain

            Audio.DSP.AmplifySection(ZeroPhaseIR, -Difference)

            'ZeroPhaseIR.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "ZeroPhaseIR.wav"))
            'IrChannelCopy.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "IrChannelCopy.wav"))

            If Channel = 1 Then
                _ZeroPhaseEquivalent.BinauralIR.WaveData.SampleData(1) = ZeroPhaseIR.WaveData.SampleData(1)
            Else
                _ZeroPhaseEquivalent.BinauralIR.WaveData.SampleData(2) = ZeroPhaseIR.WaveData.SampleData(1)
            End If

        Next

        _ZeroPhaseEquivalent.CalculateBinauralDelay("", False) ' TODO: To get the right export name, we need a refernce to the parent BinauralSimulationSet here.

    End Sub

    Public Sub CalculateBinauralDelay(ByVal IrSetName As String, Optional ByVal ExportSoundFiles As Boolean = False)

        'Gets the wave format
        Dim MonoWaveFormat = New Audio.Formats.WaveFormat(BinauralIR.WaveFormat.SampleRate, BinauralIR.WaveFormat.BitDepth, 1,, BinauralIR.WaveFormat.Encoding)

        For Channel As Integer = 1 To 2

            'Copies the channel of interest in the IR to a mono sound
            Dim IrSound As New Audio.Sound(MonoWaveFormat)
            IrSound.WaveData.SampleData(1) = BinauralIR.WaveData.SampleData(Channel)

            'Notes the length of the IR
            Dim IrLength As Integer = IrSound.WaveData.SampleData(1).Length

            'Creates a delta pulse measurement sound
            Dim TestSound = Audio.GenerateSound.CreateSilence(MonoWaveFormat,, IrLength * 5, Audio.BasicAudioEnums.TimeUnits.samples)
            Dim DeltaIndex As Integer = IrLength * 2
            TestSound.WaveData.SampleData(1)(DeltaIndex) = 1

            'Runs convolution
            Dim ConvolutedSound = Audio.DSP.FIRFilter(TestSound, IrSound, New Audio.Formats.FftFormat, ,,,,, True)

            'Gets the peak sample of the convoluted delta pulse
            Dim MaxValue = ConvolutedSound.WaveData.SampleData(1).Max()
            Dim MinValue = ConvolutedSound.WaveData.SampleData(1).Min()
            Dim PeakIndex As Integer = 0
            If Math.Abs(MaxValue) > Math.Abs(MinValue) Then
                PeakIndex = ConvolutedSound.WaveData.SampleData(1).ToList.IndexOf(MaxValue)
            Else
                PeakIndex = ConvolutedSound.WaveData.SampleData(1).ToList.IndexOf(MinValue)
            End If

            Dim Delay As Integer = PeakIndex - DeltaIndex

            'Limiting Delay to non-negative values
            Delay = Math.Max(0, Delay)

            'Storing the delay
            If Channel = 1 Then
                BinauralDelay.LeftDelay = Delay
            Else
                BinauralDelay.RightDelay = Delay
            End If

            If ExportSoundFiles = True Then
                TestSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "IrDelays", IrSetName & "_" & GetExportName() & "Channel_" & Channel & "_Delay_" & Delay & "_OriginalSound.wav"))
                ConvolutedSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "IrDelays", IrSetName & "_" & GetExportName() & "Channel_" & Channel & "_Delay_" & Delay & "_ConvolutedSound.wav"))
            End If

        Next

    End Sub


End Class

Public Class BinauralDelay
    Public LeftDelay As Integer
    Public RightDelay As Integer

    Public Function GetMeanDelay() As Integer
        Return Math.Floor((LeftDelay + RightDelay) / 2)
    End Function

End Class

Public Class BinauralImpulseReponseSet

    Public Property Name As String = ""
    Private _StereoKernels As New SortedList(Of String, StereoKernel)
    Private Property StereoKernels As SortedList(Of String, StereoKernel)
        Get
            If _StereoKernels.Count = 0 Then
                LoadDirectionalKernels(ImpulseResponseSetSpecificationFile)
            End If
            Return _StereoKernels
        End Get
        Set(value As SortedList(Of String, StereoKernel))
            _StereoKernels = value
        End Set
    End Property


    Private _SampleRate As Integer = -1
    Public ReadOnly Property SampleRate As Integer
        Get
            If _StereoKernels.Count = 0 Then
                LoadDirectionalKernels(ImpulseResponseSetSpecificationFile)
            End If
            Return _SampleRate
        End Get
    End Property

    Private ImpulseResponseSetSpecificationFile As String

    Private ImpulseResponseFolder As String = ""

    Public Sub New(ByVal ImpulseResponseSetSpecificationFile As String)

        Me.ImpulseResponseSetSpecificationFile = ImpulseResponseSetSpecificationFile
        Me.LoadMetaData(ImpulseResponseSetSpecificationFile)

    End Sub

    Private Sub LoadMetaData(ByVal ImpulseResponseSetSpecificationFile As String)

        'Reading the whole file
        Dim Lines = IO.File.ReadAllLines(ImpulseResponseSetSpecificationFile, Text.Encoding.UTF8)

        Dim ReadSourceLocations As Boolean = False

        'Creating a buffer so that sounds already loaded do not have to be loaded again
        Dim LoadedSoundFiles As New SortedList(Of String, Audio.Sound)

        For Each Line In Lines
            If Line.Trim = "" Then Continue For
            If Line.Trim(vbTab) = "" Then Continue For
            If Line.Trim.StartsWith("//") Then Continue For

            If ReadSourceLocations = False Then
                If Line.Trim.StartsWith("Name") Then Name = InputFileSupport.GetInputFileValue(Line, True)
                If Line.Trim.StartsWith("ImpulseResponseSubFolder") Then ImpulseResponseFolder = Utils.NormalizeCrossPlatformPath(IO.Path.Combine(OstfBase.RoomImpulsesSubDirectory, InputFileSupport.InputFilePathValueParsing(Line, "", True)))
                If Line.Trim.StartsWith("SampleRate") Then _SampleRate = InputFileSupport.InputFileIntegerValueParsing(Line, True, ImpulseResponseSetSpecificationFile)
                If Line.Trim.StartsWith("<AvailableSourceLocations>") Then
                    Exit For
                End If
            End If
        Next

    End Sub


    Private Sub LoadDirectionalKernels(ByVal ImpulseResponseSetSpecificationFile As String)

        'Reading the whole file
        Dim Lines = IO.File.ReadAllLines(ImpulseResponseSetSpecificationFile, Text.Encoding.UTF8)

        Dim ReadSourceLocations As Boolean = False

        'Creating a buffer so that sounds already loaded do not have to be loaded again
        Dim LoadedSoundFiles As New SortedList(Of String, Audio.Sound)

        Dim ExpectedColumnCount As Integer = 6

        Dim LineRead As Integer = 0
        For Each Line In Lines

            LineRead += 1

            If Line.Trim = "" Then Continue For
            If Line.Trim(vbTab) = "" Then Continue For
            If Line.Trim.StartsWith("//") Then Continue For

            If ReadSourceLocations = False Then
                'If Line.Trim.StartsWith("Name") Then Name = InputFileSupport.GetInputFileValue(Line, True)
                'If Line.Trim.StartsWith("ImpulseResponseSubFolder") Then ImpulseResponseFolder = Utils.NormalizeCrossPlatformPath(IO.Path.Combine(OstfBase.RoomImpulsesSubDirectory, InputFileSupport.InputFilePathValueParsing(Line, "", True)))
                'If Line.Trim.StartsWith("SampleRate") Then _SampleRate = InputFileSupport.InputFileIntegerValueParsing(Line, True, ImpulseResponseSetSpecificationFile)
                If Line.Trim.StartsWith("<AvailableSourceLocations>") Then ReadSourceLocations = True

            Else

                'Reading IR sounds
                'Parsing line
                Dim LineSplit = Line.Trim.Trim(vbTab).Split(vbTab)

                If LineSplit.Length < ExpectedColumnCount Then Throw New ArgumentException("The file " & ImpulseResponseSetSpecificationFile & " contains a bad structure at line " & LineRead & "(There should be " & ExpectedColumnCount & " tab delimited columns.")

                Dim SoundFile As String = LineSplit(0).Trim
                Dim ImpulseResponseInputChannel As String = InputFileSupport.InputFileIntegerValueParsing(LineSplit(1).Trim, False, ImpulseResponseSetSpecificationFile)
                Dim Ear As String = LineSplit(2).Trim
                If Ear = "R" Or Ear = "L" Then
                    'OK
                Else
                    Throw New ArgumentException("The file " & ImpulseResponseSetSpecificationFile & " contains a bad structure at line " & LineRead & "(The third column should be either L or R.")
                End If
                Dim Azimuth As Double = InputFileSupport.InputFileDoubleValueParsing(LineSplit(3).Trim, False, ImpulseResponseSetSpecificationFile)
                'Unwraps the azimuth
                Azimuth = Utils.UnwrapAngle(Azimuth)
                Dim Elevation As Double = InputFileSupport.InputFileDoubleValueParsing(LineSplit(4).Trim, False, ImpulseResponseSetSpecificationFile)
                Dim Distance As Double = InputFileSupport.InputFileDoubleValueParsing(LineSplit(5).Trim, False, ImpulseResponseSetSpecificationFile)

                'Loading the sound file if needed
                If LoadedSoundFiles.ContainsKey(SoundFile) = False Then

                    Dim LoadedSound = Audio.Sound.LoadWaveFile(Utils.NormalizeCrossPlatformPath(IO.Path.Combine(MediaRootDirectory, ImpulseResponseFolder, SoundFile)))
                    Select Case LoadedSound.WaveFormat.BitDepth
                        Case 16

                            'Converting to IEEE 32-bit, if needed
                            Dim ConvertedSound = New Audio.Sound(New Audio.Formats.WaveFormat(LoadedSound.WaveFormat.SampleRate, 32, LoadedSound.WaveFormat.Channels))

                            'Copying and scaling data
                            Dim Orig_FS As Integer = LoadedSound.WaveFormat.PositiveFullScale
                            For c = 1 To LoadedSound.WaveFormat.Channels
                                Dim SourceArray = LoadedSound.WaveData.SampleData(c)
                                Dim ChannelArray(SourceArray.Length - 1) As Single
                                For s = 0 To SourceArray.Length - 1
                                    ChannelArray(s) = SourceArray(s) / Orig_FS
                                Next
                                ConvertedSound.WaveData.SampleData(c) = ChannelArray
                            Next
                            LoadedSound = ConvertedSound

                        Case 32
                            'OK
                        Case Else
                            Throw New NotImplementedException("Unsupported bit depth detected when loading impulse responses for directional simulation.")
                    End Select

                    'Adding the sound to LoadedSoundFiles
                    LoadedSoundFiles.Add(SoundFile, LoadedSound)

                End If

                'Creating a 3D point representing the sound source location, sorted and stored in cartesian coordinates
                Dim NewPoint As New Point3D
                NewPoint.SetBySpherical(Azimuth, Elevation, Distance)
                Dim CartesianPoint = NewPoint.GetCartesianLocation

                'References the current input sound
                Dim CurrentInputSound = LoadedSoundFiles(SoundFile)
                Dim PointString = CartesianPoint.ToString("", System.Globalization.CultureInfo.InvariantCulture)

                If _StereoKernels.ContainsKey(PointString) = False Then
                    'Adding a new sound in the appropriate stereo format
                    Dim NewSound As New Audio.Sound(GetStereoKernelFormat(CurrentInputSound.WaveFormat))
                    _StereoKernels.Add(PointString, New StereoKernel With {.Name = PointString, .Point = NewPoint, .BinauralIR = NewSound})
                End If

                'Adding the sound data
                If Ear = "L" Then
                    _StereoKernels(PointString).BinauralIR.WaveData.SampleData(1) = CurrentInputSound.WaveData.SampleData(ImpulseResponseInputChannel)
                Else
                    _StereoKernels(PointString).BinauralIR.WaveData.SampleData(2) = CurrentInputSound.WaveData.SampleData(ImpulseResponseInputChannel)
                End If

            End If

        Next

        'Adjusting calibration gain so that a signal with a C-weighted spectrum gets zero dB filter gain when the IR is used as a kernel in a convolution filter, in the front position (azimuth = 0, elevation = 0).
        'For this the average gain of the left and the right ear IRs (in the front prosition) is used. 
        'Each distance is modified separately, so that simulated level (i.e. the level in the headphones) gets independent of distance.
        Dim CalibrationOffSets = CalculateFrontalIrGains()
        For Each Distance_OffSetValue In CalibrationOffSets

            Dim CurrentDistance = Distance_OffSetValue.Item1
            Dim CurrentCalibrationOffSet = Distance_OffSetValue.Item2

            'N.B. this code will fail if an occurring distance lack a front position!

            For Each Kernel In _StereoKernels
                If Kernel.Value.Point.GetSphericalDistance = CurrentDistance Then

                    'Attenuating by the calibration offset (to get zero dB filter gain for a signal with a C-weighted spectrum)
                    Audio.DSP.AmplifySection(Kernel.Value.BinauralIR, -CurrentCalibrationOffSet)

                End If
            Next
        Next

        'Dim CalibrationOffSetsCheck = CalculateFrontalIrGains()


        'Calculating the BinauralDelay
        For Each Kernel In _StereoKernels
            Kernel.Value.CalculateBinauralDelay(Name)
        Next

        'Also preparing SameEarsKernels
        For Each Kernel In _StereoKernels
            Kernel.Value.CreateTwoSameEarsKernels()
        Next



    End Sub

    ''' <summary>
    ''' Searches the imuplse response set and returns the binaural impulse response available closest the the indicated azimuth, elevation and distance.
    ''' </summary>
    ''' <param name="Azimuth">The azimuth (phi)</param>
    ''' <param name="Elevation">The elevation (or inclination, theta), relative to the horizontal plane</param>
    ''' <param name="Distance">The distance (or radius, r). Must be a non-zero positive value.</param>
    ''' <returns>Returns a tuple containing the selected 3D-point and the corresponding binaural impulse response.</returns>
    Public Function GetClosestPoint(ByVal Azimuth As Double, ByVal Elevation As Double, ByVal Distance As Double) As StereoKernel

        If Distance <= 0 Then
            Throw New ArgumentException("Distance cannot be zero (or lower) in directional simulation.")
        End If

        Dim TargetPoint As New Point3D
        TargetPoint.SetBySpherical(Azimuth, Elevation, Distance)
        Dim CartesianPoint = TargetPoint.GetCartesianLocation

        'Searching for the closest point
        Dim SmallestDistance As Single = Single.MaxValue
        Dim ClosestStereoKernel As StereoKernel = Nothing
        For Each CandidatePoint In StereoKernels

            'Calculating the distance between the target point and the current candidate point
            Dim CurrentDistance = Vector3.Distance(TargetPoint.GetCartesianLocation, CandidatePoint.Value.Point.GetCartesianLocation)

            'Stores the candidate point if closer than all previous
            If CurrentDistance < SmallestDistance Then
                SmallestDistance = CurrentDistance
                ClosestStereoKernel = CandidatePoint.Value
            End If
        Next

        Return ClosestStereoKernel

    End Function

    Public Function GetAllOccurringDistances() As SortedSet(Of Double)

        Dim Output As New SortedSet(Of Double)
        For Each IR In StereoKernels
            Output.Add(IR.Value.Point.GetSphericalDistance())
        Next
        Return Output

    End Function

    Public Class Point3D

        ''' <summary>
        ''' Horizontal azimuth (phi) pointing to the point in sperical coordinates 
        ''' </summary>
        ''' <returns></returns>
        Private Property SphericalAzimuth As Double
        ''' <summary>
        ''' The elevation (or inclination, theta) pointing to the point, in sperical coordinates, relative to the horizontal plane 
        ''' </summary>
        ''' <returns></returns>
        Private Property SphericalElevation As Double
        ''' <summary>
        ''' The distance (or radius, r) to the point from the point of origin
        ''' </summary>
        ''' <returns></returns>
        Private Property SphericalDistance As Double

        ''' <summary>
        ''' The location of the point in cartesian coordinates
        ''' </summary>
        ''' <returns></returns>
        Private Property CartesianLocation As Vector3

        ''' <summary>
        ''' Sets the spherical coordinates of the point and calculates the cartesian coordinates.
        ''' </summary>
        ''' <param name="Azimuth">The azimuth (phi)</param>
        ''' <param name="Elevation">The elevation (or inclination, theta), relative to the horizontal plane</param>
        ''' <param name="Distance">The distance (or radius, r)</param>
        Public Sub SetBySpherical(ByVal Azimuth As Double, ByVal Elevation As Double, ByVal Distance As Double)

            Me.SphericalAzimuth = Azimuth
            Me.SphericalElevation = Elevation
            Me.SphericalDistance = Distance

            Me.CartesianLocation = GetCartesian(Azimuth, Elevation, Distance)

        End Sub

        ''' <summary>
        ''' Sets the cartesian coordinates of the point and calculates the spherical coordinates.
        ''' </summary>
        ''' <param name="x">The back(negative values)-to-front(positive values) dimension</param>
        ''' <param name="y">The left(negative values)-to-right(positive values) dimension</param>
        ''' <param name="z">The down(negative values)-to-up(positive values) dimension</param>
        Public Sub SetByCartesian(ByVal x As Double, ByVal y As Double, ByVal z As Double)

            Me.CartesianLocation = New Vector3(x, y, z)

            Dim SC = GetSpherical(x, y, z)
            Me.SphericalAzimuth = SC.Item1
            Me.SphericalElevation = SC.Item2
            Me.SphericalDistance = SC.Item3

        End Sub

        ''' <summary>
        ''' Return the cartesian coordinates for the corresponding spherical point.
        ''' </summary>
        ''' <param name="phi">The azimuth (phi)</param>
        ''' <param name="theta">The elevation (or inclination, theta), relative to the horizontal plane</param>
        ''' <param name="r">The distance (or radius, r)</param>
        ''' <returns>The cartesian coordinates. x is back-to-front, y is left-to-right, z is down-to-up</returns>
        Public Shared Function GetCartesian(ByVal phi As Double, ByVal theta As Double, ByVal r As Double) As Vector3

            'Shifting theta to be relative to the polar axis
            theta += 90

            'Converting degrees to radians 
            theta = Utils.Math.Degrees2Radians(theta)
            phi = Utils.Math.Degrees2Radians(phi)

            'Cf. https://en.wikipedia.org/wiki/Spherical_coordinate_system#Cartesian_coordinates

            Dim x = r * Math.Sin(theta) * Math.Cos(phi)
            Dim y = r * Math.Sin(theta) * Math.Sin(phi)
            Dim z = r * Math.Cos(theta)

            Return New Vector3(x, y, z)

        End Function

        ''' <summary>
        ''' Return the spherical coordinates for the corresponding cartesian point.
        ''' </summary>
        ''' <param name="x">The back(negative values)-to-front(positive values) dimension</param>
        ''' <param name="y">The left(negative values)-to-right(positive values) dimension</param>
        ''' <param name="z">The down(negative values)-to-up(positive values) dimension</param>
        ''' <returns>Returns the corresponding spherical coordinates phi, theta, r, where phi is the azimuth, theta is the elevation (or inclination) relative to the horizontal plane, and r is the distance (or radius) to the point of origin.</returns>
        Public Shared Function GetSpherical(ByVal x As Double, ByVal y As Double, ByVal z As Double) As Tuple(Of Double, Double, Double)

            'Cf. https://en.wikipedia.org/wiki/Spherical_coordinate_system#Cartesian_coordinates

            Dim r = Math.Sqrt(x ^ 2 + y ^ 2 + x ^ 2)
            Dim theta = Math.Acos(z / r)
            Dim phi = Math.Sign(y) * Math.Acos(x / (x ^ 2 + y ^ 2))

            'Converting radians to degrees
            theta = Utils.Math.Radians2Degrees(theta)
            phi = Utils.Math.Radians2Degrees(phi)

            'Shifting theta to be relative to the horizontal plane instead of the polar axis
            theta -= 90

            Return New Tuple(Of Double, Double, Double)(phi, theta, r)

        End Function


        ''' <summary>
        ''' Gets the horizontal azimuth (phi) pointing to the point in sperical coordinates 
        ''' </summary>
        ''' <returns></returns>
        Public Function GetSphericalAzimuth() As Double
            Return SphericalAzimuth
        End Function

        ''' <summary>
        ''' Gets the elevation (or inclination, theta) pointing to the point, in sperical coordinates, relative to the horizontal plane 
        ''' </summary>
        ''' <returns></returns>
        Public Function GetSphericalElevation() As Double
            Return SphericalElevation
        End Function

        ''' <summary>
        ''' Gets the distance (or radius, r) to the point from the point of origin
        ''' </summary>
        ''' <returns></returns>
        Public Function GetSphericalDistance() As Double
            Return SphericalDistance
        End Function

        ''' <summary>
        ''' Gets the location of the point in cartesian coordinates
        ''' </summary>
        ''' <returns></returns>
        Public Function GetCartesianLocation() As Vector3
            Return CartesianLocation
        End Function

    End Class


    Public Function GetStereoKernelFormat(ByRef ModelFormat As Audio.Formats.WaveFormat) As Audio.Formats.WaveFormat

        Return New Audio.Formats.WaveFormat(ModelFormat.SampleRate,
                                            ModelFormat.BitDepth, 2,,
                                            ModelFormat.Encoding)

    End Function

    Public Function CalculateFrontalIrGains(Optional ByVal ExportSoundFiles As Boolean = False) As List(Of Tuple(Of Double, Double))

        Dim GainList As New List(Of Tuple(Of Double, Double))

        Dim Distances = GetAllOccurringDistances()

        For Each Distance In Distances

            Dim ClosestPointSound = GetClosestPoint(0, 0, Distance).BinauralIR

            Dim LeftValue = CalculateIrGain(Me.Name & "L" & Distance, ClosestPointSound, 1, ExportSoundFiles)
            Dim RightValue = CalculateIrGain(Me.Name & "R" & Distance, ClosestPointSound, 2, ExportSoundFiles)
            Dim Average = {LeftValue, RightValue}.Average

            'Dim LeftValue_B = CalculateIrGain_UsingNoise(Me.Name & "L" & Distance, ClosestPointSound, 1, ExportSoundFiles)
            'Dim RightValue_B = CalculateIrGain_UsingNoise(Me.Name & "R" & Distance, ClosestPointSound, 2, ExportSoundFiles)
            'Dim Average_B = {LeftValue_B, RightValue_B}.Average

            GainList.Add(New Tuple(Of Double, Double)(Distance, Average))

        Next

        Return GainList

    End Function

    Public Shared Function CalculateIrGain(ByVal IrName As String, ByRef IR As Audio.Sound, ByVal Channel As Integer, Optional ByVal ExportSoundFiles As Boolean = False) As Double

        Dim MonoWaveFormat = New Audio.Formats.WaveFormat(IR.WaveFormat.SampleRate, IR.WaveFormat.BitDepth, 1,, IR.WaveFormat.Encoding)

        'Copies the channel of interest in the IR to a mono sound
        Dim IrSound As New Audio.Sound(MonoWaveFormat)
        IrSound.WaveData.SampleData(1) = IR.WaveData.SampleData(Channel)

        'Notes the length of the IR
        Dim IrLength As Integer = IrSound.WaveData.SampleData(1).Length

        'Creates a delta pulse measurement sound
        Dim TestSound = Audio.GenerateSound.CreateSilence(MonoWaveFormat,, IrLength * 5, Audio.BasicAudioEnums.TimeUnits.samples)
        TestSound.WaveData.SampleData(1)(IrLength * 2) = 1

        'Filters it with a C-weighting, to avoid low and high frequency influences
        TestSound = Audio.DSP.IIRFilter(TestSound, Audio.BasicAudioEnums.FrequencyWeightings.C)

        'Measures the pre filter level
        Dim PreLevel As Double = Audio.DSP.MeasureSectionLevel(TestSound, 1, IrLength, 3 * IrLength)

        'Runs convolution
        Dim ConvolutedSound = Audio.DSP.FIRFilter(TestSound, IrSound, New Audio.Formats.FftFormat, ,,,,, True)

        'Gets the post-convolution level
        Dim PostLevel = Audio.DSP.MeasureSectionLevel(ConvolutedSound, 1, IrLength, 3 * IrLength)

        'Calculates the gain
        Dim FilterGain As Double = PostLevel - PreLevel

        If ExportSoundFiles = True Then
            TestSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "IrGains", IrName & "Channel_" & Channel & "OriginalSound.wav"))
            ConvolutedSound.WriteWaveFile(IO.Path.Combine(Utils.logFilePath, "IrGains", IrName & "Channel_" & Channel & "ConvolutedSound.wav"))
        End If

        Return FilterGain

    End Function


End Class