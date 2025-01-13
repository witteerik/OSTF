'A class that can store MediaSets
Imports STFN.Audio
Imports System.IO
Imports STFN.Audio.DSP
Imports STFN.Audio.Sound.SpeechMaterialAnnotation
Imports STFN.Audio.Formats
Imports System.Xml.Serialization
Imports System.Runtime.Serialization

<Serializable>
Public Class MediaSetLibrary
    Inherits List(Of MediaSet)

    Public Function GetNames() As List(Of String)
        Dim Output As New List(Of String)
        For Each MediaSet In Me
            Output.Add(MediaSet.MediaSetName)
        Next
        Return Output
    End Function

    Public Function GetMediaSet(ByVal MediaSetName As String) As MediaSet
        For Each MediaSet In Me
            If MediaSet.MediaSetName = MediaSetName Then Return MediaSet
        Next
        Return Nothing

    End Function

    Public Overrides Function ToString() As String
        Return String.Join("; ", GetNames())
    End Function

End Class

<Serializable>
Public Class MediaSet

    'Public Const DefaultMediaFolderName As String = "Media"
    'Public Const DefaultVariablesSubFolderName As String = "Variables"

    Public ParentTestSpecification As SpeechMaterialSpecification

    ''' <summary>
    ''' Describes the media set used in the test situaton in which a test trial is situated
    ''' </summary>
    ''' <returns></returns>
    Public Property MediaSetName As String = "New media set"

    'Information about the talker in the recordings
    Public Property TalkerName As String = ""
    Public Property TalkerGender As Genders = Genders.NotSet
    Public Property TalkerAge As Integer = -1
    Public Property TalkerDialect As String = ""
    Public Property VoiceType As String = ""


    'The following variables are used to ensure that there is an appropriate number of media files stored in the locations:
    'OstaRootPath + MediaSet.MediaParentFolder + SpeechMaterial.MediaFolder
    'and
    'OstaRootPath + MediaSet.MaskerParentFolder + SpeechMaterial.MaskerFolder
    'As well as to determine the number of recordings to create for a speech test if the inbuilt recording and segmentation tool is used.
    Public Property AudioFileLinguisticLevel As SpeechMaterialComponent.LinguisticLevels = SpeechMaterialComponent.LinguisticLevels.List

    ''' <summary>
    ''' The linguistic level at which masker sound files is to be shared.
    ''' </summary>
    ''' <returns></returns>
    Public Property SharedMaskersLevel As SpeechMaterialComponent.LinguisticLevels = SpeechMaterialComponent.LinguisticLevels.List
    Public Property SharedContralateralMaskersLevel As SpeechMaterialComponent.LinguisticLevels = SpeechMaterialComponent.LinguisticLevels.List

    Public Property MediaAudioItems As Integer = 5
    Public Property MaskerAudioItems As Integer = 5
    Public Property ContralateralMaskerAudioItems As Integer = 0
    Public Property MediaImageItems As Integer = 0
    Public Property MaskerImageItems As Integer = 0

    Public Property CustomVariablesFolder As String = ""

    Public Property MediaParentFolder As String = ""
    Public Property MaskerParentFolder As String = ""
    Public Property ContralateralMaskerParentFolder As String = ""
    Public Property CalibrationSignalParentFolder As String = ""

    Public Property EffectiveContralateralMaskingGain As Double = 0 'Holds the value in dB of the amplification that should be added to the contralateral masking to achieve effective masking

    ''' <summary>
    ''' Should store the approximate sound pressure level (SPL) of the audio recorded in the auditory non-speech background sounds stored in BackgroundNonspeechParentFolder, and should represent an ecologically feasible situation
    ''' </summary>
    Public Property BackgroundNonspeechRealisticLevel As Double = -999 ' Setting a default value of -999 dB SPL
    Public Property BackgroundNonspeechParentFolder As String = ""
    Public Property BackgroundSpeechParentFolder As String = ""

    ''' <summary>
    ''' The folder containing the recordings used as prototype recordings during the recording od the MediaSet
    ''' </summary>
    ''' <returns></returns>
    Public Property PrototypeMediaParentFolder As String = ""
    ''' <summary>
    ''' The path should point to a sound recording used as prototype when recording prototype recordings for a MediaSet
    ''' </summary>
    ''' <returns></returns>
    Public Property MasterPrototypeRecordingPath As String = ""

    Public Property PrototypeRecordingLevel As Double = -999 ' Setting a default value of -999 dBC


    Public Property LombardNoisePath As String = ""
    Public Property LombardNoiseLevel As Double = -999 ' Setting a default value of -999 dBC

    Public Property WaveFileSampleRate As Integer = 48000
    Public Property WaveFileBitDepth As Integer = 32
    Public Property WaveFileEncoding As Audio.Formats.WaveFormat.WaveFormatEncodings = Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints

    Public Enum Genders
        Male
        Female
        NotSet
    End Enum

    'These two should contain the data defined in the TestSituationDatabase associated to the component in the speech material file.
    Public NumericVariables As New SortedList(Of String, SortedList(Of String, Double)) ' SpeechMaterial Id, Variable name, Variable Value
    Public CategoricalVariables As New SortedList(Of String, SortedList(Of String, String)) ' SpeechMaterial Id, Variable name, Variable Value

    ''' <summary>
    ''' Returns the full folder name of the media parent folder
    ''' </summary>
    ''' <returns></returns>
    Public Function GetFullMediaParentFolder() As String
        Dim CurrentTestRootPath As String = ParentTestSpecification.GetTestRootPath
        Return IO.Path.Combine(CurrentTestRootPath, MediaParentFolder)
    End Function


    Public Function GetCustomVariablesDirectory()
        Return IO.Path.Combine(Me.CustomVariablesFolder)
    End Function


    Public Sub LoadCustomVariables()

        Dim ComponentLevels As New List(Of SpeechMaterialComponent.LinguisticLevels) From {0, 1, 2, 3, 4}

        For Each ComponentLevel In ComponentLevels

            Dim FilePath = IO.Path.Combine(ParentTestSpecification.GetTestRootPath, GetCustomVariablesDirectory(), SpeechMaterialComponent.GetDatabaseFileName(ComponentLevel))

            If IO.File.Exists(FilePath) = False Then
                'TODO, this happens if no media set variables exist! Probably we should create these files (empty) when creating the mediaset specification??? Or something like it??
                'MsgBox("Missing custom variable file for the media set " & Me.MediaSetName & ". Expected a file at the location: " & FilePath)
                'For now, simply ignoes missing custom media set variable files (as such may never have been created.
                Continue For
            End If

            ' Adding the custom variables
            If FilePath.Trim <> "" Then
                Dim NewDatabase As New CustomVariablesDatabase
                NewDatabase.LoadTabDelimitedFile(FilePath)

                Dim Components = ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(ComponentLevel)

                For Each Component In Components

                    'Adding the variables
                    For n = 0 To NewDatabase.CustomVariableNames.Count - 1

                        Dim VariableName = NewDatabase.CustomVariableNames(n)
                        If NewDatabase.CustomVariableTypes(n) = VariableTypes.Categorical Then

                            If Me.CategoricalVariables.ContainsKey(Component.Id) = False Then Me.CategoricalVariables.Add(Component.Id, New SortedList(Of String, String))
                            If Me.CategoricalVariables(Component.Id).ContainsKey(VariableName) Then Me.CategoricalVariables(Component.Id).Add(VariableName, "")
                            Me.CategoricalVariables(Component.Id).Add(VariableName, NewDatabase.GetVariableValue(Component.Id, VariableName))

                            'Alternatively the following code could be used
                            'Component.SetCategoricalMediaSetVariableValue(Me, VariableName, NewDatabase.GetVariableValue(Component.Id, VariableName))

                        ElseIf NewDatabase.CustomVariableTypes(n) = VariableTypes.Numeric Then

                            If Me.NumericVariables.ContainsKey(Component.Id) = False Then Me.NumericVariables.Add(Component.Id, New SortedList(Of String, Double))
                            If Me.NumericVariables(Component.Id).ContainsKey(VariableName) Then Me.NumericVariables(Component.Id).Add(VariableName, "")
                            Me.NumericVariables(Component.Id).Add(VariableName, NewDatabase.GetVariableValue(Component.Id, VariableName))

                            'Alternatively the following code could be used
                            Component.SetNumericMediaSetVariableValue(Me, VariableName, NewDatabase.GetVariableValue(Component.Id, VariableName))

                        ElseIf NewDatabase.CustomVariableTypes(n) = VariableTypes.Boolean Then

                            If Me.NumericVariables.ContainsKey(Component.Id) = False Then Me.NumericVariables.Add(Component.Id, New SortedList(Of String, Double))
                            If Me.NumericVariables(Component.Id).ContainsKey(VariableName) Then Me.NumericVariables(Component.Id).Add(VariableName, "")
                            Me.NumericVariables(Component.Id).Add(VariableName, NewDatabase.GetVariableValue(Component.Id, VariableName))

                            'Alternatively the following code could be used
                            Component.SetNumericMediaSetVariableValue(Me, VariableName, NewDatabase.GetVariableValue(Component.Id, VariableName))

                        Else
                            Throw New NotImplementedException("Variable type not implemented!")
                        End If

                    Next
                Next
            End If

        Next


    End Sub

    Public Shared Function LoadMediaSetSpecification(ByRef ParentTestSpecification As SpeechMaterialSpecification, ByVal FilePath As String) As MediaSet

        'Gets a file path from the user if none is supplied
        If FilePath = "" Then FilePath = Utils.GetOpenFilePath(,, {".txt"}, "Please open a media set specification .txt file.")
        If FilePath = "" Then
            MsgBox("No file selected!")
            Return Nothing
        End If

        'Creates a new random that will be references in all speech material components
        Dim rnd As New Random

        Dim Output As New MediaSet

        Output.ParentTestSpecification = ParentTestSpecification

        'Parses the input file
        Dim InputLines() As String = System.IO.File.ReadAllLines(FilePath, Text.Encoding.UTF8)

        For Each Line In InputLines

            'Skipping blank lines
            If Line.Trim = "" Then Continue For

            'Also skipping commentary only lines 
            If Line.Trim.StartsWith("//") Then Continue For

            If Line.StartsWith("MediaSetName") Then
                Output.MediaSetName = InputFileSupport.GetInputFileValue(Line, True)
                Continue For
            End If

            If Line.StartsWith("TalkerName") Then
                Output.TalkerName = InputFileSupport.GetInputFileValue(Line, True)
                Continue For
            End If

            If Line.StartsWith("TalkerGender") Then
                Dim Value = InputFileSupport.InputFileEnumValueParsing(Line, GetType(Genders), FilePath, True)
                If Value.HasValue Then
                    Output.TalkerGender = Value
                Else
                    MsgBox("Failed to read the TalkerGender value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("TalkerAge") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.TalkerAge = Value
                Else
                    MsgBox("Failed to read the TalkerAge value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("TalkerDialect") Then
                Output.TalkerDialect = InputFileSupport.GetInputFileValue(Line, True)
                Continue For
            End If

            If Line.StartsWith("VoiceType") Then
                Output.VoiceType = InputFileSupport.GetInputFileValue(Line, True)
                Continue For
            End If

            If Line.StartsWith("AudioFileLinguisticLevel") Then
                Dim Value = InputFileSupport.InputFileEnumValueParsing(Line, GetType(SpeechMaterialComponent.LinguisticLevels), FilePath, True)
                If Value.HasValue Then
                    Output.AudioFileLinguisticLevel = Value
                Else
                    MsgBox("Failed to read the AudioFileLinguisticLevel value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("SharedMaskersLevel") Then
                Dim Value = InputFileSupport.InputFileEnumValueParsing(Line, GetType(SpeechMaterialComponent.LinguisticLevels), FilePath, True)
                If Value.HasValue Then
                    Output.SharedMaskersLevel = Value
                Else
                    MsgBox("Failed to read the SharedMaskersLevel value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("SharedContralateralMaskersLevel") Then
                Dim Value = InputFileSupport.InputFileEnumValueParsing(Line, GetType(SpeechMaterialComponent.LinguisticLevels), FilePath, True)
                If Value.HasValue Then
                    Output.SharedContralateralMaskersLevel = Value
                Else
                    MsgBox("Failed to read the SharedContralateralMaskersLevel value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("MediaAudioItems") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.MediaAudioItems = Value
                Else
                    MsgBox("Failed to read the MediaAudioItems value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("MaskerAudioItems") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.MaskerAudioItems = Value
                Else
                    MsgBox("Failed to read the MaskerAudioItems value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("ContralateralMaskerAudioItems") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.ContralateralMaskerAudioItems = Value
                Else
                    MsgBox("Failed to read the ContralateralMaskerAudioItems value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("MediaImageItems") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.MediaImageItems = Value
                Else
                    MsgBox("Failed to read the MediaImageItems value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("MaskerImageItems") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.MaskerImageItems = Value
                Else
                    MsgBox("Failed to read the MaskerImageItems value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("CustomVariablesFolder") Then
                Output.CustomVariablesFolder = InputFileSupport.GetInputFileValue(Line, True)
                Continue For
            End If

            If Line.StartsWith("MediaParentFolder") Then
                Output.MediaParentFolder = InputFileSupport.GetInputFileValue(Line, True)
                Continue For
            End If

            If Line.StartsWith("MaskerParentFolder") Then
                Output.MaskerParentFolder = InputFileSupport.GetInputFileValue(Line, True)
                Continue For
            End If

            If Line.StartsWith("ContralateralMaskerParentFolder") Then
                Output.ContralateralMaskerParentFolder = InputFileSupport.GetInputFileValue(Line, True)
                Continue For
            End If

            If Line.StartsWith("CalibrationSignalParentFolder") Then
                Output.CalibrationSignalParentFolder = InputFileSupport.GetInputFileValue(Line, True)
                Continue For
            End If

            If Line.StartsWith("EffectiveContralateralMaskingGain") Then
                Dim Value = InputFileSupport.InputFileDoubleValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.EffectiveContralateralMaskingGain = Value
                Else
                    MsgBox("Failed to read the EffectiveContralateralMaskingGain value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("BackgroundNonspeechParentFolder") Then
                Output.BackgroundNonspeechParentFolder = InputFileSupport.GetInputFileValue(Line, True)
                Continue For
            End If

            If Line.StartsWith("BackgroundNonspeechRealisticLevel") Then
                Dim Value = InputFileSupport.InputFileDoubleValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.BackgroundNonspeechRealisticLevel = Value
                Else
                    MsgBox("Failed to read the BackgroundNonspeechRealisticLevel value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("BackgroundSpeechParentFolder") Then
                Output.BackgroundSpeechParentFolder = InputFileSupport.GetInputFileValue(Line, True)
                Continue For
            End If

            If Line.StartsWith("PrototypeMediaParentFolder") Then
                Output.PrototypeMediaParentFolder = InputFileSupport.GetInputFileValue(Line, True)
                Continue For
            End If

            If Line.StartsWith("MasterPrototypeRecordingPath") Then
                Output.MasterPrototypeRecordingPath = InputFileSupport.GetInputFileValue(Line, True)
                Continue For
            End If

            If Line.StartsWith("PrototypeRecordingLevel") Then
                Dim Value = InputFileSupport.InputFileDoubleValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.PrototypeRecordingLevel = Value
                Else
                    MsgBox("Failed to read the PrototypeRecordingLevel value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("LombardNoisePath") Then
                Output.LombardNoisePath = InputFileSupport.GetInputFileValue(Line, True)
                Continue For
            End If

            If Line.StartsWith("LombardNoiseLevel") Then
                Dim Value = InputFileSupport.InputFileDoubleValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.LombardNoiseLevel = Value
                Else
                    MsgBox("Failed to read the LombardNoiseLevel value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("WaveFileSampleRate") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.WaveFileSampleRate = Value
                Else
                    MsgBox("Failed to read the WaveFileSampleRate value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("WaveFileBitDepth") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.WaveFileBitDepth = Value
                Else
                    MsgBox("Failed to read the WaveFileBitDepth value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            If Line.StartsWith("WaveFileEncoding") Then
                Dim Value = InputFileSupport.InputFileEnumValueParsing(Line, GetType(Audio.Formats.WaveFormat.WaveFormatEncodings), FilePath, True)
                If Value.HasValue Then
                    Output.WaveFileEncoding = Value
                Else
                    MsgBox("Failed to read the WaveFileEncoding value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
                Continue For
            End If

            'If the code arrives here, an unparsed line will have been detected
            MsgBox("Failed to parse the following line value from the file " & FilePath & vbCrLf & vbCrLf & Line, MsgBoxStyle.Exclamation, "Reading media set specification file")
            Return Nothing

        Next

        'Normalizing paths read from file
        Output.BackgroundNonspeechParentFolder = Utils.NormalizeCrossPlatformPath(Output.BackgroundNonspeechParentFolder)
        Output.BackgroundSpeechParentFolder = Utils.NormalizeCrossPlatformPath(Output.BackgroundSpeechParentFolder)
        Output.CustomVariablesFolder = Utils.NormalizeCrossPlatformPath(Output.CustomVariablesFolder)
        Output.LombardNoisePath = Utils.NormalizeCrossPlatformPath(Output.LombardNoisePath)
        Output.MaskerParentFolder = Utils.NormalizeCrossPlatformPath(Output.MaskerParentFolder)
        Output.ContralateralMaskerParentFolder = Utils.NormalizeCrossPlatformPath(Output.ContralateralMaskerParentFolder)
        Output.CalibrationSignalParentFolder = Utils.NormalizeCrossPlatformPath(Output.CalibrationSignalParentFolder)
        Output.MasterPrototypeRecordingPath = Utils.NormalizeCrossPlatformPath(Output.MasterPrototypeRecordingPath)
        Output.MediaParentFolder = Utils.NormalizeCrossPlatformPath(Output.MediaParentFolder)
        Output.PrototypeMediaParentFolder = Utils.NormalizeCrossPlatformPath(Output.PrototypeMediaParentFolder)

        'Also loading custom variables
        If Output IsNot Nothing Then
            Output.LoadCustomVariables()
        End If

        Return Output

    End Function


    'Public Sub SetSipValues(ByVal Voice As Integer)

    '    Select Case Voice
    '        Case 1
    '            MediaSetName = "City-Talker1-RVE"

    '            TalkerName = "JE"
    '            TalkerGender = Genders.Male
    '            TalkerAge = 50
    '            TalkerDialect = "Central Swedish"
    '            VoiceType = "Raised vocal effort"

    '            BackgroundNonspeechRealisticLevel = 55

    '            MediaAudioItems = 5
    '            MaskerAudioItems = 5
    '            MediaImageItems = 0
    '            MaskerImageItems = 0

    '            MediaParentFolder = "Media\Unechoic-Talker1-RVE\TestWordRecordings"
    '            MaskerParentFolder = "Media\City-Talker1-RVE\TWRB"
    '            BackgroundNonspeechParentFolder = "Media\City-Talker1-RVE\BackgroundNonspeech"
    '            BackgroundSpeechParentFolder = "Media\City-Talker1-RVE\BackgroundSpeech"

    '            PrototypeMediaParentFolder = ""

    '            MasterPrototypeRecordingPath = ""
    '        Case 2

    '            MediaSetName = "City-Talker2-RVE"

    '            TalkerName = "EL"
    '            TalkerGender = Genders.Female
    '            TalkerAge = 40
    '            TalkerDialect = "Central Swedish"
    '            VoiceType = "Raised vocal effort"

    '            BackgroundNonspeechRealisticLevel = 55

    '            MediaAudioItems = 5
    '            MaskerAudioItems = 5
    '            MediaImageItems = 0
    '            MaskerImageItems = 0

    '            MediaParentFolder = "Media\Unechoic-Talker2-RVE\TestWordRecordings"
    '            MaskerParentFolder = "Media\City-Talker2-RVE\TWRB"
    '            BackgroundNonspeechParentFolder = "Media\City-Talker2-RVE\BackgroundNonspeech"
    '            BackgroundSpeechParentFolder = "Media\City-Talker2-RVE\BackgroundSpeech"

    '            PrototypeMediaParentFolder = ""

    '            MasterPrototypeRecordingPath = ""
    '    End Select

    'End Sub

    'Public Sub SetHintDebugValues()

    '    MediaSetName = "Unechoic-Talker1-RVE"

    '    TalkerName = "EW"
    '    TalkerGender = Genders.Male
    '    TalkerAge = 42
    '    TalkerDialect = "Western Swedish"
    '    VoiceType = "Raised vocal effort"

    '    BackgroundNonspeechRealisticLevel = 55

    '    MediaAudioItems = 2
    '    MaskerAudioItems = 5
    '    MediaImageItems = 0
    '    MaskerImageItems = 0

    '    MediaParentFolder = "Media\Unechoic-Talker1-RVE\TestWordRecordings"
    '    MaskerParentFolder = ""
    '    BackgroundNonspeechParentFolder = ""
    '    BackgroundSpeechParentFolder = ""

    '    PrototypeMediaParentFolder = ""

    '    MasterPrototypeRecordingPath = ""

    'End Sub


    Public Enum PrototypeRecordingOptions
        MasterPrototypeRecording
        PrototypeRecordings
        None
    End Enum

    ''' <summary>
    ''' Returns the full file paths to all existing and nonexisting sound recordings assumed to exist in the Media folder in Item1, along with the the path to a corresponding sound to be used as prototype recording as Item2 and a reference to the corresponding Component as Item3.
    ''' For lacking recordings a new paths are suggested. Existing file paths are returned in Item2, and nonexisting paths in Item3.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetAllSpeechMaterialComponentAudioPaths(ByVal PrototypeRecordingOption As PrototypeRecordingOptions) As Tuple(Of
        List(Of Tuple(Of String, String, SpeechMaterialComponent)),
        List(Of Tuple(Of String, String, SpeechMaterialComponent)),
        List(Of Tuple(Of String, String, SpeechMaterialComponent)))


        Dim CurrentTestRootPath As String = ParentTestSpecification.GetTestRootPath

        Dim AllComponents = ParentTestSpecification.SpeechMaterial.GetAllRelatives

        Dim ExistingFilesList As New List(Of Tuple(Of String, String, SpeechMaterialComponent))
        Dim LackingFilesList As New List(Of Tuple(Of String, String, SpeechMaterialComponent))
        Dim AllPaths As New List(Of Tuple(Of String, String, SpeechMaterialComponent))

        For Each Component In AllComponents

            'Skips to next if no media items are expected
            If Component.LinguisticLevel <> AudioFileLinguisticLevel Then Continue For

            Dim FullMediaFolderPath = IO.Path.Combine(CurrentTestRootPath, MediaParentFolder, Component.GetMediaFolderName)


            'Selects the appropriate prototype recording depending on the value of PrototypeRecordingOption
            Dim PrototypeRecordingPath As String = ""
            Select Case PrototypeRecordingOption
                Case PrototypeRecordingOptions.MasterPrototypeRecording

                    'Getting the file path
                    PrototypeRecordingPath = IO.Path.Combine(CurrentTestRootPath, MasterPrototypeRecordingPath)

                    'Ensuring that the master prototype recoding exist
                    If IO.File.Exists(PrototypeRecordingPath) = False Then
                        MsgBox("The master prototype recoding cannot be found at: " & PrototypeRecordingPath, MsgBoxStyle.Information, "Lacking master prototype recording")
                        Return Nothing
                    End If

                Case PrototypeRecordingOptions.PrototypeRecordings

                    'Getting the folder
                    PrototypeRecordingPath = IO.Path.Combine(CurrentTestRootPath, PrototypeMediaParentFolder, Component.GetMediaFolderName)

                    'Using the first recording (if more than one exist) as the prototype recording
                    If IO.Directory.Exists(PrototypeRecordingPath) = True Then

                        'Getting the file path
                        'Selecting the sound file paths present there 
                        Dim PrototypeRecordingFound As Boolean = False
                        Dim FilesPresent = IO.Directory.GetFiles(PrototypeRecordingPath)
                        For Each filePath In FilesPresent
                            If filePath.EndsWith(".wav") Then
                                PrototypeRecordingPath = filePath
                                PrototypeRecordingFound = True
                                Exit For
                            End If
                        Next

                        'Ensuring that a prototype recoding was found
                        If PrototypeRecordingFound = False Then
                            MsgBox("No wave file could be found in the prototype recoding folder: " & PrototypeRecordingPath, MsgBoxStyle.Information, "Lacking prototype recording")
                            Return Nothing
                        End If

                    Else
                        MsgBox("The following prototype recording folder cannot be found: " & PrototypeRecordingPath, MsgBoxStyle.Information, "Lacking prototype recording folder")
                        Return Nothing

                    End If

                Case PrototypeRecordingOptions.None
                    'No prototype recoding  is to be used
                    PrototypeRecordingPath = ""
            End Select


            Dim ExistingFileCount As Integer = 0

            'Checks if the media folder exists
            If IO.Directory.Exists(FullMediaFolderPath) = True Then

                'Gets the sound file paths present there 
                Dim FilesPresent = IO.Directory.GetFiles(FullMediaFolderPath)
                For Each filePath In FilesPresent
                    If filePath.EndsWith(".wav") Then
                        ExistingFilesList.Add(New Tuple(Of String, String, SpeechMaterialComponent)(filePath, PrototypeRecordingPath, Component))
                        AllPaths.Add(ExistingFilesList.Last)
                        ExistingFileCount += 1
                    End If
                Next

                'Notes how many files are present
                ExistingFileCount = ExistingFileCount

            End If

            'Creates file paths for files not present
            For n = ExistingFileCount To MediaAudioItems - 1
                'Creating a file name (avoiding file name conflicts)
                LackingFilesList.Add(New Tuple(Of String, String, SpeechMaterialComponent)(Utils.CheckFileNameConflict(IO.Path.Combine(FullMediaFolderPath, Component.GetMediaFolderName & "_" & (n).ToString("000") & ".wav")), PrototypeRecordingPath, Component))
                AllPaths.Add(LackingFilesList.Last)
            Next

        Next

        Return New Tuple(Of List(Of Tuple(Of String, String, SpeechMaterialComponent)),
            List(Of Tuple(Of String, String, SpeechMaterialComponent)),
            List(Of Tuple(Of String, String, SpeechMaterialComponent)))(AllPaths, ExistingFilesList, LackingFilesList)

    End Function

    ''' <summary>
    ''' Checks whether there are missing audio media files, and offers an option to create the files needed. Returns the a tuple containing the number of created files in Item1 and the number files still lacking upon return in Item2.
    ''' </summary>
    ''' <returns>Returns the a tuple containing the number of created files in Item1 and the number files still lacking upon return in Item2.</returns>
    Public Async Function CreateLackingAudioMediaFiles(ByVal PrototypeRecordingOption As PrototypeRecordingOptions,
                                                 Optional SupressUnnecessaryMessages As Boolean = False) As Task(Of Tuple(Of Integer, Integer))


        Dim ExpectedAudioPaths = GetAllSpeechMaterialComponentAudioPaths(PrototypeRecordingOption)

        If ExpectedAudioPaths Is Nothing Then
            Return Nothing
        End If

        Dim FilesCreated As Integer = 0

        If ExpectedAudioPaths.Item3.Count > 0 Then

            Dim MsgResult = Await MsgBoxAcceptQuestion(ExpectedAudioPaths.Item3.Count & " audio files are missing from media set " & MediaSetName & ". Do you want to prepare new wave files for these components?")
            ' TODO: This has not yet been tested. Does the code stop and wait for a response here??? I guess not...
            If MsgResult = True Then

                For Each item In ExpectedAudioPaths.Item3

                    Dim NewPath = item.Item1
                    '(N.B. Prototype recording paths are never directly created, but should instead be created as a separate MediaSet)
                    Dim Component = item.Item3

                    Dim NewSound = New Audio.Sound(New Audio.Formats.WaveFormat(WaveFileSampleRate, WaveFileBitDepth, 1,, WaveFileEncoding))

                    'Assign SMA values based on all child components!
                    NewSound.SMA = Component.ConvertToSMA()

                    'Adds an empty channel 1 array
                    NewSound.WaveData.SampleData(1) = {}

                    'Also assigning the SMA parent sound
                    NewSound.SMA.ParentSound = NewSound

                    If NewSound.WriteWaveFile(NewPath) = True Then FilesCreated += 1

                Next

                MsgBox("Created " & FilesCreated & " of " & ExpectedAudioPaths.Item3.Count & " new audio files.")

            Else
                MsgBox("No files were created.")
            End If

        Else

            If SupressUnnecessaryMessages = False Then MsgBox("All files needed are already in place!")

        End If

        Return New Tuple(Of Integer, Integer)(FilesCreated, ExpectedAudioPaths.Item3.Count - FilesCreated)

    End Function

    Public Enum SpeechMaterialRecorderLoadOptions
        LoadAllSounds
        LoadOnlyEmptySounds
        LoadOnlySoundsWithoutCompletedSegmentation
    End Enum


    Public Sub RecordAndEditAudioMediaFiles(ByVal SpeechMaterialRecorderSoundFileLoadOption As SpeechMaterialRecorderLoadOptions,
                                            ByVal RandomItemOrder As Boolean,
                                            ByVal PrototypeRecordingOption As PrototypeRecordingOptions)

        MsgBox("Recording and editing of media files is not supported in STFN")

    End Sub




    ''' <summary>
    ''' Copies all sound files to a folder structure which is based on the Id of the speech material component.
    ''' </summary>
    Public Sub TemporaryFunction_CopySoundFiles(ByVal OutputFolder As String)

        Dim AllSoundPathTuples = GetAllSpeechMaterialComponentAudioPaths(PrototypeRecordingOptions.None)

        'Creates the OutputFolder 
        If IO.Directory.Exists(OutputFolder) = False Then IO.Directory.CreateDirectory(OutputFolder)

        'Moves sound files
        For Each Item In AllSoundPathTuples.Item1

            Dim InputSoundFilePath = Item.Item1
            Dim SpeechMaterialComponent = Item.Item3

            Dim TargetDirectory As String = IO.Path.Combine(OutputFolder, SpeechMaterialComponent.Id)
            If IO.Directory.Exists(TargetDirectory) = False Then IO.Directory.CreateDirectory(TargetDirectory)

            Dim OutputPath As String = IO.Path.Combine(TargetDirectory, IO.Path.GetFileName(InputSoundFilePath))

            IO.File.Copy(InputSoundFilePath, OutputPath)

        Next

        MsgBox("Finished copying files.")

    End Sub

    Public Sub TemporaryFunction_CopySoundFIles2(ByVal OutputFolder As String)

        Dim CurrentTestRootPath As String = ParentTestSpecification.GetTestRootPath

        Dim AllComponents = ParentTestSpecification.SpeechMaterial.GetAllRelatives

        For Each Component In AllComponents

            'Skips to next if no media items are expected
            If Component.LinguisticLevel <> AudioFileLinguisticLevel Then Continue For

            Dim NewMediaFolderPath = IO.Path.Combine(OutputFolder, MediaParentFolder, Component.GetMediaFolderName)

            Dim OldMediaFolderPath = IO.Path.Combine(CurrentTestRootPath, MediaParentFolder, Component.GetMediaFolderName.Split("_")(0))

            'Creates the OutputFolder 
            If IO.Directory.Exists(NewMediaFolderPath) = False Then IO.Directory.CreateDirectory(NewMediaFolderPath)

            Dim FilesInPlace = IO.Directory.GetFiles(OldMediaFolderPath)

            For Each f In FilesInPlace
                IO.File.Copy(f, IO.Path.Combine(NewMediaFolderPath, IO.Path.GetFileName(f)))
            Next

        Next

        MsgBox("Finished copying files")

    End Sub

    Public Sub TemporaryFunction_CopySoundFIles3(ByVal OutputFolder As String)

        Dim CurrentTestRootPath As String = ParentTestSpecification.GetTestRootPath

        Dim AllComponents = ParentTestSpecification.SpeechMaterial.GetAllRelatives

        For Each Component In AllComponents

            'Skips to next if no media items are expected
            If Component.LinguisticLevel <> AudioFileLinguisticLevel Then Continue For

            Dim PrototypeMediaFolderPath = IO.Path.Combine(OutputFolder, MediaParentFolder, Component.GetMediaFolderName)

            Dim PrototypeSoundPath = IO.Path.Combine("C:\OSTF\Tests\SwedishSiPTest\Media\PreQueTalker1-RVE\TestWordRecordings", "SampleRec_" & Component.GetCategoricalVariableValue("Spelling") & ".wav")

            'Creates the OutputFolder 
            If IO.Directory.Exists(PrototypeMediaFolderPath) = False Then IO.Directory.CreateDirectory(PrototypeMediaFolderPath)

            IO.File.Copy(PrototypeSoundPath, IO.Path.Combine(PrototypeMediaFolderPath, Component.GetMediaFolderName & ".wav"))

        Next

        MsgBox("Finished copying files")

    End Sub

    Public Sub TemporaryFunction_RenameMaskerFolder()

        'Clears previously loaded sounds
        Dim MaskerComponents = Me.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(Me.SharedMaskersLevel)

        For Each MaskerComponent In MaskerComponents

            Dim CurrentTestRootPath As String = ParentTestSpecification.GetTestRootPath

            Dim OldMaskerFolderPath = IO.Path.Combine(CurrentTestRootPath, MaskerParentFolder, MaskerComponent.PrimaryStringRepresentation)
            Dim NewMaskerFolderPath = IO.Path.Combine(CurrentTestRootPath, MaskerParentFolder, MaskerComponent.GetMediaFolderName)

            IO.Directory.Move(OldMaskerFolderPath, NewMaskerFolderPath)

        Next

        MsgBox("Folder renaming is completed.")

    End Sub


    Public Function CreateRecordingWaveFormat() As Audio.Formats.WaveFormat
        Return New Audio.Formats.WaveFormat(Me.WaveFileSampleRate, Me.WaveFileBitDepth, 1,, Me.WaveFileEncoding)
    End Function

    Public Function CheckSoundFileFormat(ByRef Sound As Audio.Sound, Optional ByVal SoundFilePath As String = "") As Boolean

        If SoundFilePath = "" Then SoundFilePath = Sound.FileName

        If Sound.WaveFormat.SampleRate <> Me.WaveFileSampleRate Then
            MsgBox("The sound file " & SoundFilePath & " has a different sample rate than what is expected by the current media set.", MsgBoxStyle.Exclamation, "Warning!")
            Return False
        End If
        If Sound.WaveFormat.BitDepth <> Me.WaveFileBitDepth Then
            MsgBox("The sound file " & SoundFilePath & " has a different bitdepth than what is expected by the current media set.", MsgBoxStyle.Exclamation, "Warning!")
            Return False
        End If
        If Sound.WaveFormat.Encoding <> Me.WaveFileEncoding Then
            MsgBox("The sound file " & SoundFilePath & " has a different encoding what is expected by the current media set.", MsgBoxStyle.Exclamation, "Warning!")
            Return False
        End If

        Return True

    End Function

    Public Overrides Function ToString() As String
        Return Me.MediaSetName
    End Function



#Region "Variation preserving sound level normalization"


    ''' <summary>
    ''' Calculates (SII critical bands) spectrum levels of the sound recordings of concatenated speech material components.
    ''' </summary>
    ''' <param name="ConcatenationLevel">The higher linguistic level (summary level) for which the resulting spectrum levels are calculated.</param>
    ''' <param name="SegmentsLevel">The (lower) linguistic level from which the sections to be concatenaded are taken.</param>
    ''' <param name="OnlyLinguisticallyContrastingSegments">If set to true, only contrasting speech material components (e.g. contrasting phonemes in minimal pairs) will be included in the spectrum level calculations.</param>
    ''' <param name="SoundChannel">The audio / wave file channel in which the speech is recorded (channel 1, for mono sounds).</param>
    ''' <param name="SkipPractiseComponents">If set to true, speech material components marked as practise components will be skipped in the spectrum level calculations.</param>
    ''' <param name="MinimumComponentDuration">An optional minimum duration (in seconds) of each included component. If the recorded sound of a component is shorter, it will be zero-padded to the indicated duration.</param>
    ''' <param name="ComponentCrossFadeDuration">A duration by which the sections for concatenations will be cross-faded prior to spectrum level calculations.</param>
    ''' <param name="FadeConcatenatedSound">If set to true, the concatenated sounds will be slightly faded initially and finally (in order to avoid impulse-like onsets and offsets) prior to spectrum level calculations.</param>
    ''' <param name="RemoveDcComponent">If set to true, the DC component of the concatenated sounds will be set to zero prior to spectrum level calculations.</param>
    Public Sub CalculateConcatenatedComponentSpectrumLevels(ByVal ConcatenationLevel As SpeechMaterialComponent.LinguisticLevels,
                                                   ByVal SegmentsLevel As SpeechMaterialComponent.LinguisticLevels,
                                                   ByVal OnlyLinguisticallyContrastingSegments As Boolean,
                                                   ByVal SoundChannel As Integer,
                                                   ByVal SkipPractiseComponents As Boolean,
                                                            Optional ByVal MinimumComponentDuration As Double = 0,
                                                            Optional ByVal ComponentCrossFadeDuration As Double = 0.001,
                                                            Optional ByVal FadeConcatenatedSound As Boolean = True,
                                                            Optional ByVal RemoveDcComponent As Boolean = True,
                                                            Optional ByVal VariableNamePrefix As String = "SLs")

        'Sets of some objects which are reused between the loops in the code below
        Dim BandBank = Audio.DSP.BandBank.GetSiiCriticalRatioBandBank
        Dim FftFormat As New Audio.Formats.FftFormat(4 * 2048,, 1024, Audio.WindowingType.Hamming, False)
        Dim dBSPL_FSdifference As Double? = Audio.Standard_dBFS_dBSPL_Difference

        Dim WaveFormat As Audio.Formats.WaveFormat = Nothing

        'And these are only used to be able to export the values used
        Dim ActualLowerLimitFrequencyList As List(Of Double) = Nothing
        Dim ActualUpperLimitFrequencyList As List(Of Double) = Nothing

        'Clears previously loaded sounds
        SpeechMaterialComponent.ClearAllLoadedSounds()

        Dim SummaryComponents = Me.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(ConcatenationLevel)

        For Each SummaryComponent In SummaryComponents

            'Getting concatenated sounds
            Dim ConcatenatedSound = SummaryComponent.GetConcatenatedComponentsSound(Me, SegmentsLevel, OnlyLinguisticallyContrastingSegments, SoundChannel, SkipPractiseComponents, MinimumComponentDuration, ComponentCrossFadeDuration,
                                                            FadeConcatenatedSound, RemoveDcComponent)

            'Calculates spectrum levels
            Dim SpectrumLevels = Audio.DSP.CalculateSpectrumLevels(ConcatenatedSound, 1, BandBank, FftFormat, ActualLowerLimitFrequencyList, ActualUpperLimitFrequencyList, dBSPL_FSdifference)

            'Stores the value as a custom media set variable
            For b = 0 To SpectrumLevels.Count - 1

                'Creates a variable name (How on earth is are calling functions going to figure out this name???) Perhaps better to use band 1,2,3... instead of centre frequencies?
                Dim VariableName As String = VariableNamePrefix & "_" & Math.Round(BandBank(b).CentreFrequency).ToString("00000")

                SummaryComponent.SetNumericMediaSetVariableValue(Me, VariableName, SpectrumLevels(b))

            Next
        Next

        ''Finally writes the results to file
        'Dim OutputDirectory = Me.WriteCustomVariables()

        ''Send info about calculation to log (only if WriteCustomVariables returned an output folder)
        'If OutputDirectory <> "" Then
        '    Dim LogList As New List(Of String)
        '    LogList.Add("Method name: " & System.Reflection.MethodInfo.GetCurrentMethod.Name)
        '    LogList.Add("dBSPL to  FS difference used :" & dBSPL_FSdifference.ToString)
        '    LogList.Add("Filter specifications (Critical bands based on the SII standard):")
        '    LogList.Add(String.Join(vbTab, New List(Of String) From {"Band", "CentreFrequency", "LowerFrequencyLimit", "UpperFrequencyLimit", "Bandwidth", "ActualLowerLimitFrequency", "ActualUpperLimitFrequency"}))
        '    For b = 0 To BandBank.Count - 1
        '        LogList.Add(String.Join(vbTab, New List(Of String) From {CDbl((b + 1)), BandBank(b).CentreFrequency, BandBank(b).LowerFrequencyLimit, BandBank(b).UpperFrequencyLimit, BandBank(b).Bandwidth, ActualLowerLimitFrequencyList(b), ActualUpperLimitFrequencyList(b)}))
        '    Next
        '    Utils.SendInfoToLog(String.Join(vbCrLf, LogList), "Log_for_function_" & System.Reflection.MethodInfo.GetCurrentMethod.Name, OutputDirectory, False)
        'End If


    End Sub


    ''' <summary>
    ''' Calculates (SII critical bands) spectrum levels of the sound recordings of maskers ... .
    ''' </summary>
    ''' <param name="SoundChannel">The audio / wave file channel in which the speech is recorded (channel 1, for mono sounds).</param>
    ''' <param name="MaskerCrossFadeDuration">A duration by which the masker sound sections for concatenations will be cross-faded prior to spectrum level calculations.</param>
    ''' <param name="FadeConcatenatedSound">If set to true, the concatenated sounds will be slightly faded initially and finally (in order to avoid impulse-like onsets and offsets) prior to spectrum level calculations.</param>
    ''' <param name="RemoveDcComponent">If set to true, the DC component of the concatenated sounds will be set to zero prior to spectrum level calculations.</param>
    Public Sub CalculateMaskerSpectrumLevels(Optional ByVal SoundChannel As Integer = 1,
                                             Optional ByVal MaskerCrossFadeDuration As Double = 0.001,
                                             Optional ByVal FadeConcatenatedSound As Boolean = True,
                                             Optional ByVal RemoveDcComponent As Boolean = True,
                                             Optional ByVal VariableNamePrefix As String = "SLm",
                                             Optional ByVal AdjustToMaxLevelOfContrastingComponents As Boolean = True,
                                             Optional ByVal ContrastingComponentRefMaxLevel_VariableName As String = "RLxs")

        Dim SmaHighjackedSentenceIndex As Integer = 0

        'Sets of some objects which are reused between the loops in the code below
        Dim BandBank = Audio.DSP.BandBank.GetSiiCriticalRatioBandBank
        Dim FftFormat As New Audio.Formats.FftFormat(4 * 2048,, 1024, Audio.WindowingType.Hamming, False)
        Dim dBSPL_FSdifference As Double? = Audio.Standard_dBFS_dBSPL_Difference

        Dim WaveFormat As Audio.Formats.WaveFormat = Nothing

        'And these are only used to be able to export the values used
        Dim ActualLowerLimitFrequencyList As List(Of Double) = Nothing
        Dim ActualUpperLimitFrequencyList As List(Of Double) = Nothing

        'Clears previously loaded sounds
        SpeechMaterialComponent.ClearAllLoadedSounds()

        Dim SummaryComponents = Me.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(Me.SharedMaskersLevel)

        For Each SummaryComponent In SummaryComponents

            Dim MaskerList As New List(Of Audio.Sound)
            For m = 0 To Me.MaskerAudioItems - 1
                Dim MaskerPath = SummaryComponent.GetMaskerPath(Me, m, False)

                Dim LoadedMasker = Audio.Sound.LoadWaveFile(MaskerPath)

                'Stores the wave format
                If WaveFormat Is Nothing Then WaveFormat = LoadedMasker.WaveFormat

                'Getting the central masking region, encoded as sentence: SmaHighjackedSentenceIndex, word: 1 (i.e. the second word)
                Dim CentralMaskerRegion = LoadedMasker.SMA.ChannelData(SoundChannel)(SmaHighjackedSentenceIndex)(1).GetSoundFileSection(SoundChannel)
                MaskerList.Add(CentralMaskerRegion)

            Next

            If AdjustToMaxLevelOfContrastingComponents = True Then
                'New 2020-12-30
                'Getting the contrasting phonemes level (RLxs)
                'Dim ContrastedPhonemesLevel_FS = SoundLibrary.GetAverageTestPhonemeMaxLevel_FS(TestWordList, SpeakerId)
                Dim ContrastedPhonemesLevel_FS = SummaryComponent.GetNumericMediaSetVariableValue(Me, ContrastingComponentRefMaxLevel_VariableName)
                If ContrastedPhonemesLevel_FS Is Nothing Then
                    MsgBox("Missing value for the numeric media set variable ")
                    Exit Sub
                End If

                'Setting each masker to the ContrastedPhonemesLevel_FS
                For n = 0 To MaskerList.Count - 1
                    Audio.DSP.MeasureAndAdjustSectionLevel(MaskerList(n), ContrastedPhonemesLevel_FS, 1)
                Next
                'End new 2020-12-30
            End If


            'Getting concatenated sounds
            Dim ConcatenatedSound = Audio.DSP.ConcatenateSounds(MaskerList, False,,,,, MaskerCrossFadeDuration * WaveFormat.SampleRate, False, 10, True)

            'Fading very slightly to avoid initial and final impulses
            If FadeConcatenatedSound = True Then
                Audio.DSP.Fade(ConcatenatedSound, Nothing, 0,,, ConcatenatedSound.WaveFormat.SampleRate * 0.01, Audio.DSP.FadeSlopeType.Linear)
                Audio.DSP.Fade(ConcatenatedSound, 0, Nothing,, ConcatenatedSound.WaveData.SampleData(1).Length - ConcatenatedSound.WaveFormat.SampleRate * 0.01,, Audio.DSP.FadeSlopeType.Linear)
            End If

            'Removing DC-component
            If RemoveDcComponent = True Then Audio.DSP.RemoveDcComponent(ConcatenatedSound)

            'Calculates spectrum levels
            Dim SpectrumLevels = Audio.DSP.CalculateSpectrumLevels(ConcatenatedSound, 1, BandBank, FftFormat, ActualLowerLimitFrequencyList, ActualUpperLimitFrequencyList, dBSPL_FSdifference)

            'Stores the value as a custom media set variable
            For b = 0 To SpectrumLevels.Count - 1

                'Creates a variable name (How on earth is are calling functions going to figure out this name???) Perhaps better to use band 1,2,3... instead of centre frequencies?
                Dim VariableName As String = VariableNamePrefix & "_" & Math.Round(BandBank(b).CentreFrequency).ToString("00000")

                SummaryComponent.SetNumericMediaSetVariableValue(Me, VariableName, SpectrumLevels(b))

            Next
        Next

        ''Finally writes the results to file
        'Dim OutputDirectory = Me.WriteCustomVariables()

        ''Send info about calculation to log (only if WriteCustomVariables returned an output folder)
        'If OutputDirectory <> "" Then
        '    Dim LogList As New List(Of String)
        '    LogList.Add("Method name: " & System.Reflection.MethodInfo.GetCurrentMethod.Name)
        '    LogList.Add("dBSPL to  FS difference used :" & dBSPL_FSdifference.ToString)
        '    LogList.Add("Filter specifications (Critical bands based on the SII standard):")
        '    LogList.Add(String.Join(vbTab, New List(Of String) From {"Band", "CentreFrequency", "LowerFrequencyLimit", "UpperFrequencyLimit", "Bandwidth", "ActualLowerLimitFrequency", "ActualUpperLimitFrequency"}))
        '    For b = 0 To BandBank.Count - 1
        '        LogList.Add(String.Join(vbTab, New List(Of String) From {CDbl((b + 1)), BandBank(b).CentreFrequency, BandBank(b).LowerFrequencyLimit, BandBank(b).UpperFrequencyLimit, BandBank(b).Bandwidth, ActualLowerLimitFrequencyList(b), ActualUpperLimitFrequencyList(b)}))
        '    Next
        '    Utils.SendInfoToLog(String.Join(vbCrLf, LogList), "Log_for_function_" & System.Reflection.MethodInfo.GetCurrentMethod.Name, OutputDirectory, False)
        'End If


    End Sub


    Public Sub CalculateAverageMaxLevelOfCousinComponents(ByVal SummaryLevel As SpeechMaterialComponent.LinguisticLevels,
                                                               ByVal ContrastLevel As SpeechMaterialComponent.LinguisticLevels,
                                                               ByVal SoundChannel As Integer,
                                                               ByVal SkipPractiseComponents As Boolean,
                                                               Optional ByVal IntegrationTime As Double = 0.05,
                                                               Optional ByVal FrequencyWeighting As Audio.FrequencyWeightings = Audio.FrequencyWeightings.Z,
                                                               Optional ByVal VariableName As String = "RLxs",
                                                               Optional ByVal OnlyLinguisticContrasts As Boolean = True)


        Dim WaveFormat As Audio.Formats.WaveFormat = Nothing

        'Clears previously loaded sounds
        SpeechMaterialComponent.ClearAllLoadedSounds()

        Dim SummaryComponents = Me.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SummaryLevel)

        For Each SummaryComponent In SummaryComponents

            Dim TargetComponents = SummaryComponent.GetAllDescenentsAtLevel(ContrastLevel)

            'Get the SMA components representing the sound sections of all target components
            Dim CurrentSmaComponentList As New List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)

            For c = 0 To TargetComponents.Count - 1

                If SkipPractiseComponents = True Then
                    If TargetComponents(c).IsPractiseComponent = True Then
                        Continue For
                    End If
                End If

                'Determine if is linguistically contraisting component??
                If OnlyLinguisticContrasts = True Then
                    If TargetComponents(c).IsContrastingComponent = False Then
                        Continue For
                    End If
                End If

                For i = 0 To MediaAudioItems - 1
                    CurrentSmaComponentList.AddRange(TargetComponents(c).GetCorrespondingSmaComponent(Me, i, SoundChannel, True))
                Next

            Next

            'Skipping to next Summary component if no
            If CurrentSmaComponentList.Count = 0 Then Continue For

            'Getting the actual sound sections and measures their max levels
            Dim MaxLevelList As New List(Of Double)
            For Each SmaComponent In CurrentSmaComponentList

                Dim CurrentSoundSection = (SmaComponent.GetSoundFileSection(SoundChannel))

                'Getting the WaveFormat from the first available sound
                If WaveFormat Is Nothing Then WaveFormat = CurrentSoundSection.WaveFormat

                MaxLevelList.Add(Audio.DSP.GetLevelOfLoudestWindow(CurrentSoundSection, 1,
                                                                                        CurrentSoundSection.WaveFormat.SampleRate * IntegrationTime,,,, FrequencyWeighting, True))

            Next


            'Storing the max level
            Dim AverageLevel As Double
            If MaxLevelList.Count > 0 Then

                'Calculating the average level (not average dB, but instead average RMS, as if the sounds were concatenated)

                'Converting to linear RMS
                Dim RMSList As New List(Of Double)
                For Each Level In MaxLevelList
                    RMSList.Add(Audio.dBConversion(Level, Audio.dBConversionDirection.from_dB, WaveFormat))
                Next

                'Inverting to the root by taking the square
                'We get mean squares
                Dim MeanSquareList As New List(Of Double)
                For Each RMS In RMSList
                    MeanSquareList.Add(RMS * RMS)
                Next

                'Calculating the grand mean square of all sounds (this assumes all sections being of equal length, which they are here (i.e. IntegrationTime))
                Dim GrandMeanSquare As Double = MeanSquareList.Average

                'Takning the root to get the garnd RMS value
                Dim GrandRMS As Double = Math.Sqrt(GrandMeanSquare)

                'Converting to dB
                AverageLevel = Audio.dBConversion(GrandRMS, Audio.dBConversionDirection.to_dB, WaveFormat)
            Else
                AverageLevel = Double.NegativeInfinity
            End If

            'Stores the value as a custom media set variable
            SummaryComponent.SetNumericMediaSetVariableValue(Me, VariableName, AverageLevel)

        Next

        ''Finally writes the results to file
        'Me.WriteCustomVariables()

    End Sub



    Public Sub CalculateAverageDurationOfContrastingComponents(ByVal SummaryLevel As SpeechMaterialComponent.LinguisticLevels,
                                                               ByVal ContrastLevel As SpeechMaterialComponent.LinguisticLevels,
                                                               ByVal SoundChannel As Integer,
                                                               Optional ByVal VariableName As String = "Tc")


        'Clears previously loaded sounds
        SpeechMaterialComponent.ClearAllLoadedSounds()

        Dim SummaryComponents = Me.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SummaryLevel)

        For Each SummaryComponent In SummaryComponents

            Dim TargetComponents = SummaryComponent.GetAllDescenentsAtLevel(ContrastLevel)

            'Get the SMA components representing the sound sections of all target components
            Dim CurrentSmaComponentList As New List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)

            For c = 0 To TargetComponents.Count - 1

                'Determine if is contraisting component??
                If TargetComponents(c).IsContrastingComponent = False Then
                    Continue For
                End If

                For i = 0 To MediaAudioItems - 1
                    CurrentSmaComponentList.AddRange(TargetComponents(c).GetCorrespondingSmaComponent(Me, i, SoundChannel, True))
                Next

            Next

            'Skipping to next Summary component if no
            If CurrentSmaComponentList.Count = 0 Then Continue For

            'Getting the actual sound sections and measures their durations
            Dim DurationList As New List(Of Double)
            For Each SmaComponent In CurrentSmaComponentList
                DurationList.Add(SmaComponent.Length / SmaComponent.ParentSMA.ParentSound.WaveFormat.SampleRate)
            Next

            'Storing the average duration
            Dim AverageDuration As Double
            If DurationList.Count > 0 Then
                AverageDuration = DurationList.Average
            Else
                AverageDuration = 0
            End If

            'Stores the value as a custom media set variable
            SummaryComponent.SetNumericMediaSetVariableValue(Me, VariableName, AverageDuration)

        Next

        ''Finally writes the results to file
        'Me.WriteCustomVariables()

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="TargetComponentsLevel"></param>
    ''' <param name="SoundChannel"></param>
    ''' <param name="IntegrationTime"></param>
    ''' <param name="FrequencyWeighting"></param>
    ''' <param name="VariableName"></param>
    ''' <param name="IncludePractiseComponents"></param>
    ''' <param name="UniquePrimaryStringRepresenations">If set to true, only the first occurence of a set of components that have the same PrimaryStringRepresentation will be included. This can be used to include multiple instantiations of the same component only once.</param>
    Public Sub CalculateComponentLevel(ByVal TargetComponentsLevel As SpeechMaterialComponent.LinguisticLevels,
                                                               ByVal SoundChannel As Integer,
                                                               Optional ByVal IntegrationTime As Double = 0,
                                                               Optional ByVal FrequencyWeighting As Audio.FrequencyWeightings = Audio.FrequencyWeightings.Z,
                                                               Optional ByVal VariableName As String = "Lc",
                                       Optional ByVal IncludePractiseComponents As Boolean = False,
                                       Optional ByVal UniquePrimaryStringRepresenations As Boolean = False)


        Dim WaveFormat As Audio.Formats.WaveFormat = Nothing

        'Clears previously loaded sounds
        SpeechMaterialComponent.ClearAllLoadedSounds()

        Dim SummaryComponents = Me.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(TargetComponentsLevel)

        For Each SummaryComponent In SummaryComponents

            'Get the SMA components representing the sound sections of all target components
            Dim CurrentSmaComponentList As New List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)
            For i = 0 To MediaAudioItems - 1
                CurrentSmaComponentList.AddRange(SummaryComponent.GetCorrespondingSmaComponent(Me, i, SoundChannel, IncludePractiseComponents, UniquePrimaryStringRepresenations))
            Next

            'Skipping to next Summary component if no
            If CurrentSmaComponentList.Count = 0 Then Continue For

            'Getting the actual sound sections and measures their levels
            Dim SoundLevelList As New List(Of Tuple(Of Double, Double)) ' First item contain Level, Second item contain SegmentLength (in samples)
            For Each SmaComponent In CurrentSmaComponentList

                Dim CurrentSoundSection = (SmaComponent.GetSoundFileSection(SoundChannel))

                'Getting the WaveFormat from the first available sound
                If WaveFormat Is Nothing Then WaveFormat = CurrentSoundSection.WaveFormat

                Dim SegmentLength As Integer = CurrentSoundSection.WaveData.SampleData(1).Length

                If IntegrationTime = 0 Then
                    SoundLevelList.Add(New Tuple(Of Double, Double)(Audio.DSP.MeasureSectionLevel(CurrentSoundSection, 1, ,,,, FrequencyWeighting), SegmentLength))
                Else
                    SoundLevelList.Add(New Tuple(Of Double, Double)(Audio.DSP.GetLevelOfLoudestWindow(CurrentSoundSection, 1, CurrentSoundSection.WaveFormat.SampleRate * IntegrationTime,,,, FrequencyWeighting, True), SegmentLength))
                End If

            Next

            'Storing the average level
            Dim AverageLevel As Double
            If SoundLevelList.Count > 0 Then

                If IntegrationTime = 0 Then

                    'Calculating the average level (not average dB, but instead average RMS, as if the sounds were concatenated)
                    Dim TotalSumOfSquares As Double = 0
                    'N.B. Total length is repressented as a floating point number, which means that some rounding will occur, and the rounding will depend of the total length of the sounds measured. A float is used in order to avoid numeric overflow of Integer or Long with very long sounds.
                    Dim TotalLength As Double = 0
                    For Each Level In SoundLevelList

                        'Converting to linear RMS
                        Dim SegmentRMS As Double = Audio.dBConversion(Level.Item1, Audio.dBConversionDirection.from_dB, WaveFormat)

                        'Getting the mean square by inverting to the RMS value (by taking the square)
                        Dim SegmentMeanSquare As Double = SegmentRMS ^ 2

                        'Getting the summed Squares by multiplying by the length
                        Dim SegmentSumOfSquares = SegmentMeanSquare * Level.Item2

                        'Incrementing TotalSumOfSquares 
                        TotalSumOfSquares += SegmentSumOfSquares

                        'TotalSumOfSquares total length
                        TotalLength += Level.Item2

                    Next

                    'Getting the mean square for all segments concatenated
                    Dim MeanSquare As Double = TotalSumOfSquares / TotalLength

                    'Takning the root to get the RMS value
                    Dim RMS As Double = Math.Sqrt(MeanSquare)

                    'Converting to dB
                    AverageLevel = Audio.dBConversion(RMS, Audio.dBConversionDirection.to_dB, WaveFormat)

                Else

                    'Calculating the average level (not average dB, but instead average RMS, as if the sounds were concatenated)
                    'We use no section weighting since, all segments have the same length (i.e. IntegrationTime)
                    'Converting to linear RMS
                    Dim RMSList As New List(Of Double)
                    For Each Level In SoundLevelList
                        RMSList.Add(Audio.dBConversion(Level.Item1, Audio.dBConversionDirection.from_dB, WaveFormat))
                    Next

                    'Getting the mean square by inverting to the RMS value (by taking the square)
                    Dim MeanSquareList As New List(Of Double)
                    For Each RMS In RMSList
                        MeanSquareList.Add(RMS * RMS)
                    Next

                    'Calculating the grand mean square of all sounds (this assumes all sections being of equal length, which they are here (i.e. IntegrationTime))
                    Dim GrandMeanSquare As Double = MeanSquareList.Average

                    'Takning the root to get the garnd RMS value
                    Dim GrandRMS As Double = Math.Sqrt(GrandMeanSquare)

                    'Converting to dB
                    AverageLevel = Audio.dBConversion(GrandRMS, Audio.dBConversionDirection.to_dB, WaveFormat)

                End If

            Else
                AverageLevel = Double.NegativeInfinity
            End If

            'Stores the value as a custom media set variable
            SummaryComponent.SetNumericMediaSetVariableValue(Me, VariableName, AverageLevel)

        Next

        ''Finally writes the results to file
        'Me.WriteCustomVariables()

    End Sub

    ''' <summary>
    ''' Sets, the sound level of all components at the TargetComponentsLinguisticLevel. The applied gain will be reported in the custom variable file, with the variable name VariableName.
    ''' </summary>
    ''' <param name="TargetComponentsLinguisticLevel"></param>
    ''' <param name="TargetSoundLevel"></param>
    ''' <param name="SoundChannel"></param>
    ''' <param name="IntegrationTime"></param>
    ''' <param name="FrequencyWeighting"></param>
    ''' <param name="VariableName"></param>
    ''' <param name="IncludePractiseComponents"></param>
    ''' <param name="UniquePrimaryStringRepresenations">If set to true, only the first occurence of a set of components that have the same PrimaryStringRepresentation will be included. This can be used to include multiple instantiations of the same component only once.</param>
    ''' <returns>Returns True if successfull, or otherwise False.</returns>    
    Public Function SetComponentLevel(ByVal TargetComponentsLinguisticLevel As SpeechMaterialComponent.LinguisticLevels,
                                      ByVal TargetSoundLevel As Double,
                                      ByVal SoundChannel As Integer,
                                      Optional ByVal IntegrationTime As Double = 0,
                                      Optional ByVal FrequencyWeighting As Audio.FrequencyWeightings = Audio.FrequencyWeightings.Z,
                                      Optional ByVal VariableName As String = "Lc_Ag",
                                      Optional ByVal IncludePractiseComponents As Boolean = True,
                                      Optional ByVal UniquePrimaryStringRepresenations As Boolean = False,
                                      Optional ByVal AllowIncompleteSegmentations As Boolean = True) As Boolean


        Dim WaveFormat As Audio.Formats.WaveFormat = Nothing

        'Clears previously loaded sounds
        SpeechMaterialComponent.ClearAllLoadedSounds()

        Dim SummaryComponents = Me.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(TargetComponentsLinguisticLevel)

        'Checks for which components segmentation is marked as incomplete for all items at the TargetComponentsLinguisticLevel
        Dim IncompletedSegmentationsIndices As New SortedSet(Of Integer)

        For SCI = 0 To SummaryComponents.Count - 1

            Dim SummaryComponent = SummaryComponents(SCI)

            'Get the SMA components representing the sound sections of all target components
            Dim CurrentSmaComponentList As New List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)
            For i = 0 To MediaAudioItems - 1
                CurrentSmaComponentList.AddRange(SummaryComponent.GetCorrespondingSmaComponent(Me, i, SoundChannel, IncludePractiseComponents, UniquePrimaryStringRepresenations))

                Dim CurrentIndexSound = SummaryComponent.GetSound(Me, i, SoundChannel, ,,,,, True,,,, True)
                If CurrentIndexSound Is Nothing Then

                    If AllowIncompleteSegmentations = False Then
                        MsgBox("Unable to calculate or set component sound levels due to lacking recording (index: " & i & ") of the following speech material component: " & SummaryComponent.PrimaryStringRepresentation, MsgBoxStyle.Exclamation, "Missing recording")
                        Return False
                    Else
                        IncompletedSegmentationsIndices.Add(SCI)
                        Continue For
                    End If
                End If
            Next

            If IncompletedSegmentationsIndices.Contains(SCI) = False Then
                For Each SmaComponent In CurrentSmaComponentList
                    If SmaComponent.SegmentationCompleted = False Then
                        If AllowIncompleteSegmentations = False Then
                            MsgBox("Unable to calculate or set component sound levels due to incomplete Speech Material Annotation (SMA) segmentation on the target linguistic level (" & TargetComponentsLinguisticLevel.ToString & ") in the following sound file: " & vbCrLf & vbCrLf & SmaComponent.SourceFilePath, MsgBoxStyle.Exclamation, "Incomplete SMA segmentation")
                            Return False
                        Else
                            IncompletedSegmentationsIndices.Add(SCI)
                        End If
                    End If
                Next
            End If
        Next


        Dim UniqueSoundSectionIdentifiers As New SortedSet(Of String)

        For SCI = 0 To SummaryComponents.Count - 1

            Dim SummaryComponent = SummaryComponents(SCI)

            'Checking if it was a non-segmented component
            If IncompletedSegmentationsIndices.Contains(SCI) Then

                ''Storing the value as Double NaN, as it can not be measure
                'If TargetSoundLevel Is Nothing Then
                '    SummaryComponent.SetNumericMediaSetVariableValue(Me, VariableName, Double.NaN)
                'Else
                '    SummaryComponent.SetNumericMediaSetVariableValue(Me, VariableName, Double.NaN)
                'End If

                'And skips to next
                Continue For
            End If

            For i = 0 To MediaAudioItems - 1
                Dim CurrentIndexSoundSmaComponents As New List(Of SmaComponent)
                Dim CurrentIndexSound = SummaryComponent.GetSound(Me, i, SoundChannel,,,,,,,,, CurrentIndexSoundSmaComponents, True)

                'Measure sound level
                Dim AverageLevel As Double
                If IntegrationTime = 0 Then
                    AverageLevel = Audio.DSP.MeasureSectionLevel(CurrentIndexSound, SoundChannel,,,,, FrequencyWeighting)
                Else
                    AverageLevel = Audio.DSP.GetLevelOfLoudestWindow(CurrentIndexSound, SoundChannel, Math.Round(CurrentIndexSound.WaveFormat.SampleRate * IntegrationTime * 0.001),,,, FrequencyWeighting, True)
                End If

                'Calculates needed gain to reach TargetSoundLevel
                Dim NeededGain As Double = TargetSoundLevel - AverageLevel

                'Add the same amount of gain to all sma components
                For Each SmaComponent In CurrentIndexSoundSmaComponents

                    'Gets the unique identifier for the SmaComponent
                    'As this will not be correct if several SmaComponents pointing to the same sound file section has been added to the list, as then amplification will be applied more than once to the same sound file section, we here check if the section has already been added
                    Dim Current_UniqueSoundSectionIdentifier = SmaComponent.CreateUniqueSoundSectionIdentifier(SmaComponent.SourceFilePath)

                    'Applies gain only if not already done
                    If UniqueSoundSectionIdentifiers.Contains(Current_UniqueSoundSectionIdentifier) = False Then

                        'Adds the identifier
                        UniqueSoundSectionIdentifiers.Add(Current_UniqueSoundSectionIdentifier)

                        'Applies the gain
                        SmaComponent.ApplyGain(NeededGain)
                    End If
                Next
            Next
        Next

        'And save the sma components back to file
        ParentTestSpecification.SpeechMaterial.SaveAllLoadedSounds(True)

        'If TargetSoundLevel Is Nothing Then
        '        'Stores the AverageLevel value as a custom media set variable
        '        SummaryComponent.SetNumericMediaSetVariableValue(Me, VariableName, AverageLevel)
        '    Else
        '        'Stores the NeededGain value as a custom media set variable
        '        SummaryComponent.SetNumericMediaSetVariableValue(Me, VariableName, NeededGain)
        '    End If

        ''Finally writes the results to file
        'Me.WriteCustomVariables()


        If IncompletedSegmentationsIndices.Count > 0 Then
            MsgBox("Unable to calculate or set the level for " & IncompletedSegmentationsIndices.Count & " out of " & SummaryComponents.Count &
                   " speech material components. Likely due to incomplete / unvalidated segmentation on the " & TargetComponentsLinguisticLevel.ToString & " (linguistic) level.", MsgBoxStyle.Exclamation, "Incomplete processing!")
        End If

        Return True

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="NewMediaSet"></param>
    ''' <param name="SoundChannel"></param>
    ''' <param name="AllowIncompleteSegmentations"></param>
    ''' <returns>Returns True if successfull, or otherwise False.</returns>
    Public Function CopySoundsToNewMediaSet(ByRef NewMediaSet As MediaSet,
                                            ByVal SoundChannel As Integer,
                                            ByVal Padding As Integer,
                                            ByVal InterStimulusIntervalLength As Integer,
                                            ByVal CrossFadeLength As Integer,
                                            ByVal IncludeTestItems As Boolean,
                                            ByVal IncludePractiseItems As Boolean,
                                            ByVal RandomizeOrder As Boolean,
                                            Optional ByVal RandomSeed As Integer? = Nothing,
                                            Optional ByVal AllowIncompleteSegmentations As Boolean = True) As Boolean


        'Clears previously loaded sounds
        SpeechMaterialComponent.ClearAllLoadedSounds()

        Dim SummaryComponents = Me.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(NewMediaSet.AudioFileLinguisticLevel, Not IncludePractiseItems, Not IncludeTestItems)

        'Checks for which components segmentation is marked as incomplete for all items at the TargetComponentsLinguisticLevel
        Dim IncompletedSegmentationsIndices As New SortedSet(Of Integer)

        For SCI = 0 To SummaryComponents.Count - 1

            Dim SummaryComponent = SummaryComponents(SCI)

            'Get the SMA components representing the sound sections of all target components
            Dim CurrentSmaComponentList As New List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)
            For i = 0 To MediaAudioItems - 1

                'Creating the appropriate sound path
                Dim NewFullMediaFolderPath = IO.Path.Combine(ParentTestSpecification.GetTestRootPath, NewMediaSet.MediaParentFolder, SummaryComponent.GetMediaFolderName)
                Dim NewSoundFilePath = IO.Path.Combine(NewFullMediaFolderPath, "Sound" & i.ToString("00"))

                'Getting the sound
                Dim CurrentComponentSound = SummaryComponent.GetSound(Me, i, SoundChannel, CrossFadeLength, Padding, InterStimulusIntervalLength,, True, True, RandomizeOrder, RandomSeed)

                If CurrentComponentSound IsNot Nothing Then
                    'Writes to wave file
                    CurrentComponentSound.WriteWaveFile(NewSoundFilePath)

                Else
                    If AllowIncompleteSegmentations = False Then
                        MsgBox("Unable to create the needed sound files due to incomplete Speech Material Annotation (SMA) segmentation on the target linguistic level (" & NewMediaSet.AudioFileLinguisticLevel.ToString & ") in redoring index " & i & " of the following speech material component: " & vbCrLf & vbCrLf & SummaryComponent.PrimaryStringRepresentation, MsgBoxStyle.Exclamation, "Incomplete SMA segmentation")
                        Return False
                    Else
                        IncompletedSegmentationsIndices.Add(SCI)
                    End If
                End If
            Next
        Next

        If IncompletedSegmentationsIndices.Count > 0 Then
            MsgBox("Unable to create sound files for " & IncompletedSegmentationsIndices.Count & " out of " & SummaryComponents.Count &
                   " speech material components. Likely due to incomplete / unvalidated segmentation on the " & NewMediaSet.AudioFileLinguisticLevel.ToString & " (linguistic) level.", MsgBoxStyle.Exclamation, "Incomplete processing!")
        End If

        Return True

    End Function

    ''' <summary>
    ''' Calculates spectrum levels for all items at TargetComponentsLevel in the current MediaSet, by concatenating all sound sections representing each target level component.
    ''' </summary>
    ''' <param name="TargetComponentsLevel"></param>
    ''' <param name="SoundChannel"></param>
    ''' <param name="OnlyLinguisticallyContrastingSegments"></param>
    ''' <param name="SkipPractiseComponents"></param>
    ''' <param name="MinimumComponentDuration"></param>
    ''' <param name="ComponentCrossFadeDuration"></param>
    ''' <param name="FadeConcatenatedSound"></param>
    ''' <param name="RemoveDcComponent"></param>
    ''' <param name="VariableNamePrefix"></param>
    ''' <param name="dBSPL_FSdifference"></param>
    Public Sub CalculateSpectrumlevels(ByVal TargetComponentsLevel As SpeechMaterialComponent.LinguisticLevels,
                                       ByVal SoundChannel As Integer,
                                       ByVal OnlyLinguisticallyContrastingSegments As Boolean,
                                       ByVal SkipPractiseComponents As Boolean,
                                       Optional ByVal MinimumComponentDuration As Double = 0,
                                       Optional ByVal ComponentCrossFadeDuration As Double = 0.001,
                                       Optional ByVal FadeConcatenatedSound As Boolean = True,
                                       Optional ByVal RemoveDcComponent As Boolean = True,
                                       Optional ByVal VariableNamePrefix As String = "SLs",
                                       Optional ByRef dBSPL_FSdifference As Double? = Nothing)

        Dim WaveFormat As Audio.Formats.WaveFormat = Nothing

        'Clears previously loaded sounds
        SpeechMaterialComponent.ClearAllLoadedSounds()

        Dim TargetComponents = Me.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(TargetComponentsLevel)

        For Each TargetComponent In TargetComponents

            Dim ConcatenatedSound = TargetComponent.GetConcatenatedComponentsSound(Me, TargetComponentsLevel, OnlyLinguisticallyContrastingSegments, SoundChannel, SkipPractiseComponents, MinimumComponentDuration, ComponentCrossFadeDuration,
                                                            FadeConcatenatedSound, RemoveDcComponent, True)

            If ConcatenatedSound Is Nothing Then Continue For

            Dim BandBank = Audio.DSP.BandBank.GetSiiCriticalRatioBandBank
            Dim ReusableFftFormat As Audio.Formats.FftFormat = Nothing

            Dim SpectrumLevels = Audio.DSP.CalculateSpectrumLevels(ConcatenatedSound, SoundChannel, BandBank, ReusableFftFormat,,, dBSPL_FSdifference)

            'Stores the value as a custom media set variable
            'The outcommented lines can be used to get the old SiP-test component IDs
            'Dim OldSipName As String = "X_" & TargetComponent.ParentComponent.PrimaryStringRepresentation.ToLower & " (" & TargetComponent.GetAncestorAtLevel(SpeechMaterial.LinguisticLevels.List).PrimaryStringRepresentation & ")"
            'TargetComponent.SetCategoricalMediaSetVariableValue(Me, "OldSipName", OldSipName)
            'TargetComponent.SetNumericMediaSetVariableValue(Me, "PhonemeIndex", TargetComponent.GetSelfIndex)

            For i = 0 To BandBank.Count - 1
                Dim VariableName As String = VariableNamePrefix & "_" & Math.Round(BandBank(i).CentreFrequency).ToString("00000")
                TargetComponent.SetNumericMediaSetVariableValue(Me, VariableName, SpectrumLevels(i))
            Next

        Next

        ''Finally writes the results to file
        'Me.WriteCustomVariables()

    End Sub


    Public Enum MaskerSourceTypes
        RandomNoise
        SpeechMaterial
        ExternalSoundFilesBestMatch
        ExternalSoundFileShroeder
    End Enum



    Private Class MaskerSoundCategoryData
        Public ReadOnly SoundCategoryName As String
        Public AverageSoundDistance As Double = Double.PositiveInfinity
        Public MaskerSoundList As New List(Of MaskerSoundData)

        Public Sub New(ByVal SoundCategoryName As String)
            Me.SoundCategoryName = SoundCategoryName
        End Sub

        Public Class MaskerSoundData

            Public ReadOnly InputSoundIndex As Integer
            Public ReadOnly MaskerFileStartSample As Integer
            Public ReadOnly MaskerFileLength As Integer
            Public ReadOnly CentralRegionFileStartSample As Integer
            Public ReadOnly CentralRegionLength As Integer
            Public ReadOnly AcousticDistance As Double

            Public Sub New(ByVal InputSoundIndex As Integer, ByVal MaskerFileStartSample As Integer,
                ByVal MaskerFileLength As Integer, ByVal CentralRegionFileStartSample As Integer,
                ByVal CentralRegionLength As Integer, ByVal AcousticDistance As Double)

                Me.InputSoundIndex = InputSoundIndex
                Me.MaskerFileStartSample = MaskerFileStartSample
                Me.MaskerFileLength = MaskerFileLength
                Me.CentralRegionFileStartSample = CentralRegionFileStartSample
                Me.CentralRegionLength = CentralRegionLength
                Me.AcousticDistance = AcousticDistance

            End Sub

        End Class
    End Class


    'Public Sub CalculateCbSpectrumLevels(Optional ByVal BandInfo As Audio.DSP.BandBank = Nothing,
    '                                     Optional FftFormat As Audio.Formats.FftFormat = Nothing,
    '                                     Optional ByVal dBSPL_FSdifference As Double? = Nothing)

    '    'Temporarily sets the load type of sound files
    '    Dim AudioFileLoadMode_StartValue = SpeechMaterial.AudioFileLoadMode
    '    SpeechMaterial.AudioFileLoadMode = SpeechMaterial.MediaFileLoadModes.LoadOnFirstUse

    '    If dBSPL_FSdifference Is Nothing Then dBSPL_FSdifference = Audio.PortAudioVB.DuplexMixer.Standard_dBFS_dBSPL_Difference

    '    If BandInfo Is Nothing Then
    '        'Setting default audiogram frequencies
    '        BandInfo = Audio.DSP.BandBank.GetSiiCriticalRatioBandBank

    '    End If

    '    'Setting up FFT formats
    '    If FftFormat Is Nothing Then FftFormat = New Audio.Formats.FftFormat(4 * 2048,, 1024, Audio.WindowingType.Hamming, False)

    '    Try

    '        'Setting a default export folder
    '        Dim ExportFolder As String = ""
    '        If ExportFolder = "" Then
    '            Dim fbd As New Windows.Forms.FolderBrowserDialog
    '            fbd.Description = "Select folder to store the new sound files"
    '            If fbd.ShowDialog() <> Windows.Forms.DialogResult.OK Then
    '                Exit Try
    '            End If

    '            ExportFolder = fbd.SelectedPath
    '            If ExportFolder = "" Then
    '                Exit Try
    '            End If
    '        End If



    '        Dim BandLevelExportList As New List(Of String)
    '        Dim CentreFrequencies As New List(Of String)
    '        For Each band In BandInfo
    '            CentreFrequencies.Add("PBL_" & band.CentreFrequency)
    '        Next
    '        BandLevelExportList.Add("TW_TWG_V" & vbTab & String.Join(vbTab, CentreFrequencies) & vbTab & "AverageTotalBandLevel" & vbTab & "OverallLevel")

    '        Dim SpectrumLevelExportList As New List(Of String)
    '        CentreFrequencies.Clear()
    '        For Each band In BandInfo
    '            CentreFrequencies.Add("PSL_" & band.CentreFrequency)
    '        Next
    '        SpectrumLevelExportList.Add("TW_TWG_V" & vbTab & String.Join(vbTab, CentreFrequencies) & vbTab & "OverallLevel")

    '        'Getting the concatenated phonemes
    '        Dim MasterConcatPhonemesList As New SortedList(Of String, SortedList(Of Integer, Tuple(Of Audio.Sound, List(Of Audio.Sound))))
    '        For Each TestWordList In TestWordLists
    '            For Each TestWord In TestWordList.MemberWords
    '                'The ConcatPhonemesList contains concatenates test phonemes (as values), for each speaker (indicated by key)
    '                MasterConcatPhonemesList.Add(TestWordList.ListName & "_" & TestWord.Spelling, GetConcatenatedTestPhonemes_Word(TestWord, True, Audio.FrequencyWeightings.Z, False, False, ""))
    '            Next
    '        Next

    '        'Starting a progress window
    '        Dim Progress As Integer = 0
    '        Dim myProgressDisplay As New ProgressDisplay
    '        myProgressDisplay.Initialize(TestWordLists.Count - 1, 0, "Calculating sound levels...")
    '        myProgressDisplay.Show()
    '        Progress = 0


    '        Utils.SendInfoToLog("TW_TWG_V" & vbTab &
    '                  "AverageBandLevel" & vbTab &
    '                  "SpectrumLevel" & vbTab &
    '                  "CentreFrequency" & vbTab &
    '                  "LowerFrequencyLimit (actual)" & vbTab &
    '                  "UpperFrequencyLimit (actual)", "MeasurementLog", ExportFolder, True, True)

    '        Dim FilterInfoIsExported As Boolean = False
    '        Dim FilterExportList As New List(Of String)
    '        FilterExportList.Add("Critical band filter info")
    '        FilterExportList.Add("CentreFrequency" & vbTab & "LowerFrequencyLimit" &
    '                                             vbTab & "ActualLowerLimitFrequency" &
    '                                             vbTab & "UpperFrequencyLimit" &
    '                                             vbTab & "ActualUpperLimitFrequency")

    '        For Each TestWordList In TestWordLists

    '            'Updating progress
    '            myProgressDisplay.UpdateProgress(Progress)
    '            Progress += 1

    '            For Each TestWord In TestWordList.MemberWords

    '                'The ConcatPhonemesList contains concatenates test phonemes (as values), for each speaker (indicated by key)
    '                Dim ConcatPhonemesList = MasterConcatPhonemesList(TestWordList.ListName & "_" & TestWord.Spelling)

    '                For Each Speaker In ConcatPhonemesList

    '                    Dim SpeakerID As Integer = Speaker.Key
    '                    Dim TW_TWG_V As String = SpeakerID & "_" & TestWord.Spelling.ToLower & " (" & TestWordList.ListName & ")"

    '                    Dim ConcatPhonemesSound As Audio.Sound = Speaker.Value.Item1
    '                    Dim SampleRate As Integer = ConcatPhonemesSound.WaveFormat.SampleRate

    '                    'ConcatPhonemesSound = Sound.LoadWaveFile("C:\SpeechAndHearingToolsLog\CB_TP2\testsound.wav") ' Just a sound to check calculations with, with energy at 100-9500 Hz
    '                    'ConcatPhonemesSound = Sound.LoadWaveFile("C:\SpeechAndHearingToolsLog\CB_Data\Measured ISTS\CBG_Stad.ptwf") ' Just a sound to check calculations with, with energy at 100-9500 Hz

    '                    'Calculating spectra
    '                    ConcatPhonemesSound.FFT = Audio.DSP.SpectralAnalysis(ConcatPhonemesSound, FftFormat)
    '                    ConcatPhonemesSound.FFT.CalculatePowerSpectrum(True, True, True, 0.25)

    '                    Dim TempBandLevelList As New List(Of String)
    '                    TempBandLevelList.Add(TW_TWG_V)

    '                    Dim TempSpectrumLevelList As New List(Of String)
    '                    TempSpectrumLevelList.Add(TW_TWG_V)

    '                    Dim TotalPower As Double = 0 'A variable used for checking the correctness of the spectral level calculations (See below)
    '                    For Each band In BandInfo

    '                        Dim ActualLowerLimitFrequency As Double
    '                        Dim ActualUpperLimitFrequency As Double

    '                        Dim WindowLevelArray = Audio.DSP.AcousticDistance.CalculateWindowLevels(ConcatPhonemesSound,,,
    '                                                                      band.LowerFrequencyLimit,
    '                                                                      band.UpperFrequencyLimit,
    '                                                                      Audio.FftData.GetSpectrumLevel_InputType.FftBinCentreFrequency_Hz,
    '                                                                      False, False,
    '                                                                      ActualLowerLimitFrequency,
    '                                                                      ActualUpperLimitFrequency)

    '                        Dim AverageBandLevel_FS As Double = WindowLevelArray.Average

    '                        Dim AverageBandLevel As Double = AverageBandLevel_FS + dBSPL_FSdifference
    '                        TempBandLevelList.Add(AverageBandLevel)

    '                        'Calculating spectrum level according to equation 3 in ANSI S3.5-1997 (The SII-standard)
    '                        Dim SpectrumLevel As Double = AverageBandLevel - 10 * Math.Log10(band.Bandwidth / 1)
    '                        TempSpectrumLevelList.Add(SpectrumLevel)

    '                        'Logging
    '                        Utils.SendInfoToLog(TW_TWG_V & vbTab &
    '                                  AverageBandLevel & vbTab &
    '                                  SpectrumLevel & vbTab &
    '                                  band.CentreFrequency & vbTab &
    '                                  band.LowerFrequencyLimit & "(" & ActualLowerLimitFrequency & ")" & vbTab &
    '                                  band.UpperFrequencyLimit & "(" & ActualUpperLimitFrequency & ")",
    '                                  "MeasurementLog", ExportFolder, True, True)

    '                        'Summing the band power 
    '                        'Converting to linear scale
    '                        Dim BandPower As Double = Audio.dBConversion(AverageBandLevel_FS, Audio.dBConversionDirection.from_dB,
    '                                                           ConcatPhonemesSound.WaveFormat,
    '                                                           Audio.dBTypes.SoundPower)
    '                        'Summing the sound power
    '                        TotalPower += BandPower


    '                        'Storing filter info for export
    '                        If FilterInfoIsExported = False Then
    '                            FilterExportList.Add(band.CentreFrequency & vbTab &
    '                                             band.LowerFrequencyLimit & vbTab &
    '                                             ActualLowerLimitFrequency & vbTab &
    '                                             band.UpperFrequencyLimit & vbTab &
    '                                             ActualUpperLimitFrequency & vbTab)
    '                        End If
    '                    Next

    '                    'Converting summed spectral power back to dB scale 
    '                    Dim AverageTotalSpectrumLevel As Double = Audio.dBConversion(TotalPower, Audio.dBConversionDirection.to_dB,
    '                                                                       ConcatPhonemesSound.WaveFormat,
    '                                                                       Audio.dBTypes.SoundPower) + dBSPL_FSdifference
    '                    TempBandLevelList.Add(AverageTotalSpectrumLevel)

    '                    'Also measuring the average total level directly in the time domain, to ensure accuracy of the spectral calculations (i.e. AverageTotalSpectrumLevel should largely agree with OverallLevel, especially for a signal with the majority of level within 100 - 9500 Hz, all sound outside this band is ignored in AverageTotalSpectrumLevel)
    '                    Dim OverallLevel As Double = Audio.DSP.MeasureSectionLevel(ConcatPhonemesSound, 1) + dBSPL_FSdifference
    '                    TempBandLevelList.Add(OverallLevel)
    '                    TempSpectrumLevelList.Add(OverallLevel)

    '                    BandLevelExportList.Add(String.Join(vbTab, TempBandLevelList))
    '                    SpectrumLevelExportList.Add(String.Join(vbTab, TempSpectrumLevelList))

    '                    If FilterInfoIsExported = False Then
    '                        Utils.SendInfoToLog(String.Join(vbCrLf, FilterExportList), "CriticalBandFilterInfo", ExportFolder)
    '                        FilterInfoIsExported = True
    '                    End If
    '                Next
    '            Next
    '        Next

    '        Utils.SendInfoToLog(String.Join(vbCrLf, BandLevelExportList), "TestPhonemeBandLevels", ExportFolder, True, True)
    '        Utils.SendInfoToLog(String.Join(vbCrLf, SpectrumLevelExportList), "TestPhonemeSpectrumLevels", ExportFolder, True, True)

    '        'Closing the progress display
    '        myProgressDisplay.Close()

    '    Catch ex As Exception
    '        MsgBox("An error occured in MeasureSmaObjectSoundLevels." & vbCrLf & ex.ToString)
    '    End Try

    '    'Resets the load type of sound files to the same type as when the sub was called
    '    SpeechMaterial.AudioFileLoadMode = AudioFileLoadMode_StartValue

    'End Sub

    'Public Function GetConcatenatedTestPhonemes_Word(ByRef TestWord As SpeechMaterialLibrary.TestWord,
    '                                              Optional ByVal FrequencyWeighting As Audio.FrequencyWeightings = Audio.FrequencyWeightings.Z,
    '                                              Optional ByVal ExportConcatenatedTestPhonemeFile As Boolean = False,
    '                                            Optional ExportFolder As String = "") As Tuple(Of Audio.Sound, List(Of Audio.Sound)) ' Concatenated Sound, Sounds prior to concatenation


    '    Dim WaveFormat As Audio.Formats.WaveFormat = Nothing

    '    'Getting a list of sound containing the test phoneme

    '    Dim TestPhonemeSoundList As New List(Of Audio.Sound)

    '    For Each Stimulus In TestWord.TestStimuli

    '        Dim StimulusSound As Audio.Sound = Stimulus.SoundRecording

    '        'Getting the WaveFormat from the first available sound
    '        If WaveFormat Is Nothing Then WaveFormat = StimulusSound.WaveFormat

    '        Dim TestPhonemeStartSample As Integer = StimulusSound.SMA.ChannelData(1)(sentence)(0).PhoneData(TestWord.ParentTestWordList.ContrastedPhonemeIndex).StartSample
    '        Dim TestPhonemeLength As Integer = StimulusSound.SMA.ChannelData(1)(sentence)(0).PhoneData(TestWord.ParentTestWordList.ContrastedPhonemeIndex).Length

    '        Dim TestPhonemeSound = Audio.DSP.CopySection(StimulusSound, TestPhonemeStartSample, TestPhonemeLength)

    '        TestPhonemeSoundList.Add(TestPhonemeSound)

    '    Next

    '    'Concatenating the sounds
    '    Dim CrossPhonemeDuration As Double = 0.001 'Note that this should not be longer than the average test phoneme length of the analysed phonemes
    '    Dim AllPhonemesSound As Audio.Sound = Audio.DSP.ConcatenateSounds(TestPhonemeSoundList,,,,,, CrossPhonemeDuration * WaveFormat.SampleRate, False, 10, True)

    '    'Fading very slightly to avoid initial and final impulses
    '    Audio.DSP.Fade(AllPhonemesSound, Nothing, 0,,, AllPhonemesSound.WaveFormat.SampleRate * 0.01, Audio..DSP.FadeSlopeType.Linear)
    '    Audio.DSP.Fade(AllPhonemesSound, 0, Nothing,, AllPhonemesSound.WaveData.SampleData(1).Length - AllPhonemesSound.WaveFormat.SampleRate * 0.01,, Audio..DSP.FadeSlopeType.Linear)

    '    'Removing DC-component
    '    Audio.DSP.RemoveDcComponent(AllPhonemesSound)

    '    If ExportConcatenatedTestPhonemeFile = True Then

    '        Audio.AudioIOs.SaveToWaveFile(AllPhonemesSound, IO.Path.Combine(ExportFolder, "ConcatTestPhoneme_TW_" & TestWord.ParentTestWordList.ListName & "_" & TestWord.Spelling))

    '    End If

    '    Return New Tuple(Of Audio.Sound, List(Of Audio.Sound))(AllPhonemesSound, TestPhonemeSoundList)

    'End Function



    Private Sub LoadAllSoundFIles(ByVal SoundChannel As Integer, ByVal IncludePracticeComponents As Boolean)

        'Loading sound files (storing them in the shared Speech Material Component sound library
        Dim AllComponentsWithSound = Me.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(Me.AudioFileLinguisticLevel)
        Dim MissingSoundIds As New List(Of String)
        For c = 0 To AllComponentsWithSound.Count - 1

            If IncludePracticeComponents = False Then
                If AllComponentsWithSound(c).IsPractiseComponent = True Then Continue For
            End If

            For i = 0 To MediaAudioItems - 1

                Dim CurrentSmaComponentList = AllComponentsWithSound(c).GetCorrespondingSmaComponent(Me, i, SoundChannel, True)

                If CurrentSmaComponentList.Count = 1 Then
                    If CurrentSmaComponentList(0).GetSoundFileSection(SoundChannel) Is Nothing Then
                        MissingSoundIds.Add(AllComponentsWithSound(c).Id & ("(" & i & ")"))
                    End If
                ElseIf CurrentSmaComponentList.Count > 1 Then
                    MsgBox("Detected inconsistent specifications of AudioFileLinguisticLevel (" & Me.AudioFileLinguisticLevel.ToString & ") for the speech material component " &
                           AllComponentsWithSound(c).Id & " ( " & AllComponentsWithSound(c).PrimaryStringRepresentation & "). Cannot continue loading sounds!")
                    Exit Sub
                Else
                    MissingSoundIds.Add(AllComponentsWithSound(c).Id & ("(" & i & ")"))
                End If
            Next
        Next

        If MissingSoundIds.Count > 0 Then
            MsgBox("No sound (or sound containing SMA components) could be loaded for the following " & MissingSoundIds.Count & " components ids (recording number in parentheses):" & vbCrLf & String.Join(" ", MissingSoundIds))
        End If

    End Sub


    Public Shared Function CreateSpeechFilterKernel(ByVal WaveFormat As Audio.Formats.WaveFormat,
                                                   Optional ExportToFile As Boolean = False,
                                                   Optional LogFolder As String = "") As Audio.Sound

        Try
            Dim KernelFrequencyResponse As New List(Of Tuple(Of Single, Single))
            KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(0, 0))
            KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(75, 0))
            KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(80, 1))
            KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(13000, 1))
            KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(15000, 0))
            KernelFrequencyResponse.Add(New Tuple(Of Single, Single)(24000, 0))

            Dim FilterKernel = Audio.GenerateSound.CreateCustumImpulseResponse(KernelFrequencyResponse, Nothing, WaveFormat, New Audio.Formats.FftFormat, 8000,, True, True)

            If ExportToFile = True Then
                If LogFolder = "" Then
                    Audio.AudioIOs.SaveToWaveFile(FilterKernel,,,,, "GeneralSoundFilterKernel")
                Else
                    Audio.AudioIOs.SaveToWaveFile(FilterKernel, IO.Path.Combine(LogFolder, "GeneralSoundFilterKernel"))
                End If
            End If

            Return FilterKernel

        Catch ex As Exception
            MsgBox(ex.ToString)
            Return Nothing
        End Try

    End Function

    ''Returns the sound level (RMS, dbFS) of all input sounds as if they were concatenated (without any margins between the words)
    Public Function GetSoundLevelOfConcatenatedSounds(ByVal InputSounds As List(Of Audio.Sound),
                                                                  ByVal FrequencyWeighting As Audio.FrequencyWeightings,
                                                      ByVal MeasurementChannel As Integer) As Double

        'Measuring the average sound level in all input sounds (as if they were one long sound)

        'Measuring the mean-square of the of each sound file
        Dim SumOfSquaresList As New List(Of Tuple(Of Double, Integer))

        For Each CurrentSound In InputSounds

            'Measures level of each input sound
            Dim SumOfSquareData As Tuple(Of Double, Integer) = Nothing
            Audio.DSP.MeasureSectionLevel(CurrentSound, MeasurementChannel,,,,, FrequencyWeighting, True, SumOfSquareData)

            'Adds the sum-of-square data
            SumOfSquaresList.Add(SumOfSquareData)

        Next

        'Calculating a weighted average sum of squares. (N.B. If fed with very many, or loud files, this calculation may crash due to overflowing the max values of Long or Double.) 
        Dim SumOfSquares As Double = 0
        Dim TotalLength As Long = 0
        For n = 0 To SumOfSquaresList.Count - 1
            SumOfSquares += SumOfSquaresList(n).Item1
            TotalLength += CULng(SumOfSquaresList(n).Item2)
        Next

        'Calculating mean square
        Dim MeanSquare As Double = SumOfSquares / TotalLength

        'Calculating RMS by taking the root of the MeanSquare
        Dim RMS As Double = MeanSquare ^ (1 / 2)

        'Converting to dB
        Dim RMSLevel As Double = Audio.dBConversion(RMS, Audio.dBConversionDirection.to_dB, InputSounds(0).WaveFormat)

        Return RMSLevel

    End Function

#End Region



    Public Sub SetSpeechLevels(ByVal TargetLevel As Double, ByVal FrequencyWeighting As Audio.FrequencyWeightings, ByVal TemporalIntegration As Double?, ByVal LinguisticLevel As SpeechMaterialComponent.LinguisticLevels)

        If TemporalIntegration.HasValue = False Then TemporalIntegration = 0

        Me.SetComponentLevel(LinguisticLevel, TargetLevel, 1, TemporalIntegration, FrequencyWeighting,,,, True)


    End Sub

    ''' <summary>
    ''' Creates a new MediaSet which is a deep copy of the original, by using serialization.
    ''' </summary>
    ''' <returns></returns>
    Public Function CreateCopy() As MediaSet

        'Creating an output object
        Dim newMediaSet As MediaSet

        'Serializing to memorystream
        Dim serializedMe As New MemoryStream
        Dim serializer As New XmlSerializer(GetType(MediaSet))
        serializer.Serialize(serializedMe, Me)

        'Deserializing to new object
        serializedMe.Position = 0
        newMediaSet = CType(serializer.Deserialize(serializedMe), MediaSet)
        serializedMe.Close()

        'Returning the new object
        Return newMediaSet
    End Function

End Class

