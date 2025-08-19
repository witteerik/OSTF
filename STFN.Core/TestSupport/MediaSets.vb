'A class that can store MediaSets
Imports STFN.Core.Audio
Imports System.IO
Imports STFN.Core.Audio.DSP
Imports STFN.Core.Audio.Sound.SpeechMaterialAnnotation
Imports STFN.Core.Audio.Formats
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
        If FilePath = "" Then FilePath = Utils.GeneralIO.GetOpenFilePath(,, {".txt"}, "Please open a media set specification .txt file.")
        If FilePath = "" Then
            MsgBox("No file selected!")
            Return Nothing
        End If

        'Creates a new random that will be references in all speech material components
        Dim rnd As New Random

        Dim Output As New MediaSet

        Output.ParentTestSpecification = ParentTestSpecification

        Try


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

        Catch ex As Exception
            Throw New Exception("Unable to sucessfully load the file " & FilePath)
        End Try

        'Normalizing paths read from file
        Output.BackgroundNonspeechParentFolder = Utils.GeneralIO.NormalizeCrossPlatformPath(Output.BackgroundNonspeechParentFolder)
        Output.BackgroundSpeechParentFolder = Utils.GeneralIO.NormalizeCrossPlatformPath(Output.BackgroundSpeechParentFolder)
        Output.CustomVariablesFolder = Utils.GeneralIO.NormalizeCrossPlatformPath(Output.CustomVariablesFolder)
        Output.LombardNoisePath = Utils.GeneralIO.NormalizeCrossPlatformPath(Output.LombardNoisePath)
        Output.MaskerParentFolder = Utils.GeneralIO.NormalizeCrossPlatformPath(Output.MaskerParentFolder)
        Output.ContralateralMaskerParentFolder = Utils.GeneralIO.NormalizeCrossPlatformPath(Output.ContralateralMaskerParentFolder)
        Output.CalibrationSignalParentFolder = Utils.GeneralIO.NormalizeCrossPlatformPath(Output.CalibrationSignalParentFolder)
        Output.MasterPrototypeRecordingPath = Utils.GeneralIO.NormalizeCrossPlatformPath(Output.MasterPrototypeRecordingPath)
        Output.MediaParentFolder = Utils.GeneralIO.NormalizeCrossPlatformPath(Output.MediaParentFolder)
        Output.PrototypeMediaParentFolder = Utils.GeneralIO.NormalizeCrossPlatformPath(Output.PrototypeMediaParentFolder)

        'Also loading custom variables
        If Output IsNot Nothing Then
            Output.LoadCustomVariables()
        End If

        Return Output

    End Function


    Public Overrides Function ToString() As String
        Return Me.MediaSetName
    End Function


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

