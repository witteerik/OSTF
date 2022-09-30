'A class that can store MediaSets
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

End Class

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
    'OstaRootPath + MediaSet.MediaParentFolder + SpeechMaterialComponent.MediaFolder
    'and
    'OstaRootPath + MediaSet.MaskerParentFolder + SpeechMaterialComponent.MaskerFolder
    'As well as to determine the number of recordings to create for a speech test if the inbuilt recording and segmentation tool is used.
    Public Property AudioFileLinguisticLevel As SpeechMaterialComponent.LinguisticLevels = SpeechMaterialComponent.LinguisticLevels.List

    ''' <summary>
    ''' The linguistic level at which masker sound files is to be shared.
    ''' </summary>
    ''' <returns></returns>
    Public Property SharedMaskersLevel As SpeechMaterialComponent.LinguisticLevels = SpeechMaterialComponent.LinguisticLevels.List

    Public Property MediaAudioItems As Integer = 5
    Public Property MaskerAudioItems As Integer = 5
    Public Property MediaImageItems As Integer = 0
    Public Property MaskerImageItems As Integer = 0

    Public Property CustomVariablesFolder As String = ""

    Public Property MediaParentFolder As String = ""
    Public Property MaskerParentFolder As String = ""

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
    Public NumericVariables As New SortedList(Of String, SortedList(Of String, Double)) ' SpeechMaterialComponent Id, Variable name, Variable Value
    Public CategoricalVariables As New SortedList(Of String, SortedList(Of String, String)) ' SpeechMaterialComponent Id, Variable name, Variable Value


    Public Sub WriteToFile()

        Dim OutputList As New List(Of String)

        Dim DefaultOutputDirectory As String = ParentTestSpecification.GetAvailableTestSituationsDirectory()

        'Creates the diurectory if it doesn't exist
        If IO.Directory.Exists(DefaultOutputDirectory) = False Then IO.Directory.CreateDirectory(DefaultOutputDirectory)

        Dim OutputPath As String = Utils.GetSaveFilePath(ParentTestSpecification.GetAvailableTestSituationsDirectory(), "NewMediaSet", {".txt"}, "Supply a media set specification file name")

        If OutputPath = "" Then
            MsgBox("No filename supplied.", MsgBoxStyle.Information, "Saving media set specification")
            Exit Sub
        End If

        OutputList.Add("// This is an OSTA media set specification file")

        OutputList.Add("MediaSetName = " & MediaSetName)
        OutputList.Add("TalkerName = " & TalkerName)
        OutputList.Add("TalkerGender = " & TalkerGender.ToString)
        OutputList.Add("TalkerAge = " & TalkerAge)
        OutputList.Add("TalkerDialect = " & TalkerDialect)
        OutputList.Add("VoiceType = " & VoiceType)
        OutputList.Add("AudioFileLinguisticLevel = " & AudioFileLinguisticLevel.ToString)
        OutputList.Add("SharedMaskersLevel = " & SharedMaskersLevel.ToString)
        OutputList.Add("MediaAudioItems = " & MediaAudioItems)
        OutputList.Add("MaskerAudioItems = " & MaskerAudioItems)
        OutputList.Add("MediaImageItems = " & MediaImageItems)
        OutputList.Add("MaskerImageItems = " & MaskerImageItems)
        OutputList.Add("CustomVariablesFolder = " & CustomVariablesFolder)
        OutputList.Add("MediaParentFolder = " & MediaParentFolder)
        OutputList.Add("MaskerParentFolder = " & MaskerParentFolder)
        OutputList.Add("BackgroundNonspeechParentFolder = " & BackgroundNonspeechParentFolder)
        OutputList.Add("BackgroundSpeechParentFolder = " & BackgroundSpeechParentFolder)
        OutputList.Add("PrototypeMediaParentFolder = " & PrototypeMediaParentFolder)
        OutputList.Add("MasterPrototypeRecordingPath = " & MasterPrototypeRecordingPath)
        OutputList.Add("PrototypeRecordingLevel = " & PrototypeRecordingLevel)
        OutputList.Add("LombardNoisePath = " & LombardNoisePath)
        OutputList.Add("LombardNoiseLevel = " & LombardNoiseLevel)
        OutputList.Add("WaveFileSampleRate = " & WaveFileSampleRate)
        OutputList.Add("WaveFileBitDepth = " & WaveFileBitDepth)
        OutputList.Add("WaveFileEncoding = " & WaveFileEncoding.ToString)

        Utils.SendInfoToLog(String.Join(vbCrLf, OutputList), IO.Path.GetFileNameWithoutExtension(OutputPath), IO.Path.GetDirectoryName(OutputPath), True, True, True)

        WriteCustomVariables()

    End Sub

    Public Function GetCustomVariablesDirectory()
        Return IO.Path.Combine(Me.CustomVariablesFolder)
    End Function


    ''' <summary>
    ''' Writes custom variable files for the current media set, and returns the output directory.
    ''' </summary>
    Public Function WriteCustomVariables() As String

        Dim OutputDirectory As String = ""


        'Ask if overwrite or save to new location
        Dim res = MsgBox("Do you want to overwrite the existing files? Select NO to save the new files to a new location?", MsgBoxStyle.YesNo, "Overwrite existing files?")
        If res = MsgBoxResult.Yes Then

            OutputDirectory = IO.Path.Combine(ParentTestSpecification.GetTestRootPath, GetCustomVariablesDirectory())

        Else

            Dim fbd As New Windows.Forms.FolderBrowserDialog
            fbd.Description = "Select a folder in which to save the output files"
            If fbd.ShowDialog() = Windows.Forms.DialogResult.OK Then
                OutputDirectory = fbd.SelectedPath
            Else
                MsgBox("No output folder selected.", MsgBoxStyle.Exclamation, "Saving custom media set variables to file")
                Return ""
            End If

            If OutputDirectory.Trim = "" Then
                MsgBox("No output folder selected.", MsgBoxStyle.Exclamation, "Saving custom media set variables to file")
                Return ""
            End If

        End If

        Dim ComponentLevels As New List(Of SpeechMaterialComponent.LinguisticLevels) From {0, 1, 2, 3, 4}

        For Each ComponentLevel In ComponentLevels

            Dim Components = ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(ComponentLevel)

            Dim CategoricalHeadings As New SortedSet(Of String)
            Dim AddId As Boolean = False
            For Each Component In Components
                Dim Headings = Component.GetCategoricalMediaSetVariableNames(Me)
                'Adds the Id as a categorical variable only if it has not previously been added
                If Headings.Count = 0 Then
                    Headings.Add("Id")
                    AddId = True
                End If
                For Each Heading In Headings
                    CategoricalHeadings.Add(Heading)
                Next
            Next

            'Adds the Id as a custom categorical variable if not already present
            If AddId = True Then
                For Each Component In Components
                    Component.SetCategoricalMediaSetVariableValue(Me, "Id", Component.Id)
                Next
            End If

            Dim NumericHeadings As New SortedSet(Of String)
            For Each Component In Components
                Dim Headings = Component.GetNumericMediaSetVariableNames(Me)
                For Each Heading In Headings
                    NumericHeadings.Add(Heading)
                Next
            Next

            Dim OutputList As New List(Of String)

            'Adding headings and types
            If NumericHeadings.Count > 0 Then
                'Headings
                OutputList.Add(String.Join(vbTab, CategoricalHeadings) & vbTab & String.Join(vbTab, NumericHeadings))
                'Variable types
                OutputList.Add(String.Join(vbTab, Utils.Repeat("C", CategoricalHeadings.Count)) & vbTab & String.Join(vbTab, Utils.Repeat("N", NumericHeadings.Count)))
            Else
                'Only categorical headings (which should never be empty, as the Id is always added!)
                'Headings
                OutputList.Add(String.Join(vbTab, CategoricalHeadings))
                'Variable types
                OutputList.Add(String.Join(vbTab, Utils.Repeat("C", CategoricalHeadings.Count)))
            End If

            'Data lines
            For Each Component In Components
                Dim DataLine As New List(Of String)
                For Each CategoricalHeading In CategoricalHeadings
                    DataLine.Add(Component.GetCategoricalMediaSetVariableValue(Me, CategoricalHeading))
                Next
                For Each NumericHeading In NumericHeadings
                    Dim Value As Double? = Component.GetNumericMediaSetVariableValue(Me, NumericHeading)
                    If Value IsNot Nothing Then
                        DataLine.Add(Value)
                    Else
                        DataLine.Add("")
                    End If
                Next
                OutputList.Add(String.Join(vbTab, DataLine))
            Next

            'Saving to file
            Dim OutputPath As String = IO.Path.Combine(OutputDirectory, SpeechMaterialComponent.GetDatabaseFileName(ComponentLevel))

            'Creates the diurectory if it doesn't exist
            If IO.Directory.Exists(OutputDirectory) = False Then IO.Directory.CreateDirectory(OutputDirectory)

            Utils.SendInfoToLog(String.Join(vbCrLf, OutputList), IO.Path.GetFileNameWithoutExtension(OutputPath), IO.Path.GetDirectoryName(OutputPath), True, True, True)

        Next

        MsgBox("Finished saving the custom media set variables to: " & OutputDirectory, MsgBoxStyle.Information, "Saving custom media set variables to file")

        Return OutputDirectory

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

            If Line.StartsWith("MediaSetName") Then Output.MediaSetName = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("TalkerName") Then Output.TalkerName = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("TalkerGender") Then
                Dim Value = InputFileSupport.InputFileEnumValueParsing(Line, GetType(Genders), FilePath, True)
                If Value.HasValue Then
                    Output.TalkerGender = Value
                Else
                    MsgBox("Failed to read the TalkerGender value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("TalkerAge") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.TalkerAge = Value
                Else
                    MsgBox("Failed to read the TalkerAge value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("TalkerDialect") Then Output.TalkerDialect = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("VoiceType") Then Output.VoiceType = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("AudioFileLinguisticLevel") Then
                Dim Value = InputFileSupport.InputFileEnumValueParsing(Line, GetType(SpeechMaterialComponent.LinguisticLevels), FilePath, True)
                If Value.HasValue Then
                    Output.AudioFileLinguisticLevel = Value
                Else
                    MsgBox("Failed to read the AudioFileLinguisticLevel value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("SharedMaskersLevel") Then
                Dim Value = InputFileSupport.InputFileEnumValueParsing(Line, GetType(SpeechMaterialComponent.LinguisticLevels), FilePath, True)
                If Value.HasValue Then
                    Output.SharedMaskersLevel = Value
                Else
                    MsgBox("Failed to read the SharedMaskersLevel value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("MediaAudioItems") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.MediaAudioItems = Value
                Else
                    MsgBox("Failed to read the MediaAudioItems value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("MaskerAudioItems") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.MaskerAudioItems = Value
                Else
                    MsgBox("Failed to read the MaskerAudioItems value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("MediaImageItems") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.MediaImageItems = Value
                Else
                    MsgBox("Failed to read the MediaImageItems value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("MaskerImageItems") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.MaskerImageItems = Value
                Else
                    MsgBox("Failed to read the MaskerImageItems value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("CustomVariablesFolder") Then Output.CustomVariablesFolder = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("MediaParentFolder") Then Output.MediaParentFolder = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("MaskerParentFolder") Then Output.MaskerParentFolder = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("BackgroundNonspeechParentFolder") Then Output.BackgroundNonspeechParentFolder = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("BackgroundNonspeechRealisticLevel") Then
                Dim Value = InputFileSupport.InputFileDoubleValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.BackgroundNonspeechRealisticLevel = Value
                Else
                    MsgBox("Failed to read the BackgroundNonspeechRealisticLevel value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("BackgroundSpeechParentFolder") Then Output.BackgroundSpeechParentFolder = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("PrototypeMediaParentFolder") Then Output.PrototypeMediaParentFolder = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("MasterPrototypeRecordingPath") Then Output.MasterPrototypeRecordingPath = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("PrototypeRecordingLevel") Then
                Dim Value = InputFileSupport.InputFileDoubleValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.PrototypeRecordingLevel = Value
                Else
                    MsgBox("Failed to read the PrototypeRecordingLevel value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("LombardNoisePath") Then Output.LombardNoisePath = InputFileSupport.GetInputFileValue(Line, True)

            If Line.StartsWith("LombardNoiseLevel") Then
                Dim Value = InputFileSupport.InputFileDoubleValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.LombardNoiseLevel = Value
                Else
                    MsgBox("Failed to read the LombardNoiseLevel value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("WaveFileSampleRate") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.WaveFileSampleRate = Value
                Else
                    MsgBox("Failed to read the WaveFileSampleRate value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("WaveFileBitDepth") Then
                Dim Value = InputFileSupport.InputFileIntegerValueParsing(Line, True, FilePath)
                If Value.HasValue Then
                    Output.WaveFileBitDepth = Value
                Else
                    MsgBox("Failed to read the WaveFileBitDepth value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

            If Line.StartsWith("WaveFileEncoding") Then
                Dim Value = InputFileSupport.InputFileEnumValueParsing(Line, GetType(Audio.Formats.WaveFormat.WaveFormatEncodings), FilePath, True)
                If Value.HasValue Then
                    Output.WaveFileEncoding = Value
                Else
                    MsgBox("Failed to read the WaveFileEncoding value from the file " & FilePath, MsgBoxStyle.Exclamation, "Reading media set specification file")
                    Return Nothing
                End If
            End If

        Next

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
    Public Function CreateLackingAudioMediaFiles(ByVal PrototypeRecordingOption As PrototypeRecordingOptions,
                                                 Optional SupressUnnecessaryMessages As Boolean = False) As Tuple(Of Integer, Integer)

        Dim ExpectedAudioPaths = GetAllSpeechMaterialComponentAudioPaths(PrototypeRecordingOption)

        Dim FilesCreated As Integer = 0

        If ExpectedAudioPaths.Item3.Count > 0 Then

            Dim MsgResult = MsgBox(ExpectedAudioPaths.Item3.Count & " audio files are missing from media set " & MediaSetName & ". Do you want to prepare new wave files for these components?", MsgBoxStyle.YesNo)
            If MsgResult = MsgBoxResult.Yes Then


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
                                            ByVal PrototypeRecordingOption As PrototypeRecordingOptions,
                                            Optional ByRef RandomItemOrder As Boolean = True)

        'Checks first that all expected sound files exist
        Dim FilesStillLacking = CreateLackingAudioMediaFiles(PrototypeRecordingOption, True).Item2

        If FilesStillLacking > 0 Then
            MsgBox("All audio files needed were not created. Exiting RecordAudioMediaFiles.")
            Exit Sub
        End If

        'Getting all paths
        Dim AudioPaths = GetAllSpeechMaterialComponentAudioPaths(PrototypeRecordingOption)

        Dim FilesForRecordAndEdit As New List(Of Tuple(Of String, String))

        'Parsing through all sound files (without storing them in memory) and stores the paths to be included in the recorder GUI
        For Each soundFileTuple In AudioPaths.Item1

            Dim LoadedSound = Audio.Sound.LoadWaveFile(soundFileTuple.Item1)

            'Checks the waveformat of the sound
            CheckSoundFileFormat(LoadedSound, soundFileTuple.Item1)

            If LoadedSound Is Nothing Then
                MsgBox("The sound file " & soundFileTuple.Item1 & " could not be loaded.")
                Continue For
            End If

            'Checking that the format agrees with the expected format of the Media set.
            If CheckSoundFileFormat(LoadedSound, soundFileTuple.Item1) = False Then
                'TODO: Lets this through for now, but it may be good to offer a chioce to update the format???
            End If

            Select Case SpeechMaterialRecorderSoundFileLoadOption
                Case SpeechMaterialRecorderLoadOptions.LoadAllSounds

                    'Adds all files
                    FilesForRecordAndEdit.Add(New Tuple(Of String, String)(soundFileTuple.Item1, soundFileTuple.Item2))

                Case SpeechMaterialRecorderLoadOptions.LoadOnlyEmptySounds

                    'Checks if the sound is empty (N.B. Expects only mono sounds here!)
                    If LoadedSound.WaveData.SampleData(1).Length = 0 Then
                        'Adds the sound
                        FilesForRecordAndEdit.Add(New Tuple(Of String, String)(soundFileTuple.Item1, soundFileTuple.Item2))
                    End If

                Case SpeechMaterialRecorderLoadOptions.LoadOnlySoundsWithoutCompletedSegmentation

                    'Checks if segmentation if completed
                    If LoadedSound.SMA.SegmentationCompleted = False Then
                        'Adds the sound
                        FilesForRecordAndEdit.Add(New Tuple(Of String, String)(soundFileTuple.Item1, soundFileTuple.Item2))
                    End If

            End Select

        Next

        'Launches the Recorder GUI

        Dim RecorderGUI As New SpeechMaterialRecorder(Me, FilesForRecordAndEdit, RandomItemOrder)

        RecorderGUI.Show()


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

    'Sound levels
    Public Sub SetNaturalLevels(ByVal TargetLevel As Double, ByVal FrequencyWeighting As Audio.FrequencyWeightings, ByVal TemporalIntegration As Double?)

        'N.B. Should TargetLevel come in as FS or as a dB_FS to SPL corrected value???
        'Audio.Convert_dBFS_To_dBSPL()

        Dim fbd As New Windows.Forms.FolderBrowserDialog
        fbd.Description = "Select folder to store the new sound files"
        If fbd.ShowDialog() <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If

        Dim ExportFolder = fbd.SelectedPath
        If ExportFolder = "" Then Exit Sub

        If TemporalIntegration.HasValue = False Then TemporalIntegration = 0


        CreateNaturalLevelSounds(TargetLevel, FrequencyWeighting, TemporalIntegration, ExportFolder)

    End Sub

#Region "Natural levels"


    ''' <summary>
    ''' Adjusts the average level of all AudioFileLinguisticLevel SMA components to AverageTestWordOutputlevel, while at the same time retaining some of the natural level variations between SMA components referring to different speech material components.
    ''' </summary>
    ''' <param name="AverageTestWordOutputlevel"></param>
    ''' <param name="FrequencyWeighting"></param>
    ''' <param name="ExportFolder"></param>
    Public Sub CreateNaturalLevelSounds(Optional ByVal AverageTestWordOutputlevel As Double = 68.34,
                                        Optional ByVal FrequencyWeighting As Audio.FrequencyWeightings = Audio.FrequencyWeightings.Z,
                                        Optional ByVal TemporalIntegration As Decimal = 0,
                                        Optional ByVal ExportFolder As String = "",
                                        Optional ByVal SpeechFilterSounds As Boolean = True,
                                        Optional ByVal SoundChannel As Integer = 1)


        'Temporarily sets the load type of sound files
        Dim AudioFileLoadMode_StartValue = SpeechMaterialComponent.AudioFileLoadMode
        SpeechMaterialComponent.AudioFileLoadMode = SpeechMaterialComponent.MediaFileLoadModes.LoadOnFirstUse

        Try

            'This sub does the following:
            ' -Resets the sound levels of all recordings to the initial sound level

            'Clears previously loaded sounds
            ParentTestSpecification.SpeechMaterial.ClearAllLoadedSounds()

            'Setting a default export folder
            If ExportFolder = "" Then ExportFolder = Utils.logFilePath

            'Creating a structure to hold sound files
            Dim TempSoundLib As New SortedList(Of String, Tuple(Of Boolean, List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent))) 'SpeechMaterialComponent ID, IsPractiseComponent, SmaComponents

            'Resetting the sound

            'Getting the recording components to adjust
            Dim RecordingComponents = Me.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(Me.AudioFileLinguisticLevel)
            For c = 0 To RecordingComponents.Count - 1
                For i = 0 To MediaAudioItems - 1

                    If TempSoundLib.ContainsKey(RecordingComponents(c).Id) = False Then
                        TempSoundLib.Add(RecordingComponents(c).Id, New Tuple(Of Boolean, List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent))(RecordingComponents(c).IsPractiseComponent, New List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)))
                    End If
                    TempSoundLib(RecordingComponents(c).Id).Item2.Add(RecordingComponents(c).GetCorrespondingSmaComponent(Me, i, SoundChannel))

                Next
            Next


            'Resetting all recording level components to their original sound level, (as well as getting the current wave format)
            Dim CurrentWaveFormat As Audio.Formats.WaveFormat = Nothing
            For Each ComponentId In TempSoundLib
                For Each SmaComponent In ComponentId.Value.Item2

                    If CurrentWaveFormat Is Nothing Then CurrentWaveFormat = SmaComponent.ParentSMA.ParentSound.WaveFormat

                    Dim AppliedGain = SmaComponent.GetCurrentGain(SmaComponent.ParentSMA.ParentSound, 1)

                    If AppliedGain IsNot Nothing Then

                        'Reverting to the original level by amplifying by minus AppliedGain
                        Audio.DSP.AmplifySection(SmaComponent.ParentSMA.ParentSound, -AppliedGain, 1, SmaComponent.StartSample, SmaComponent.Length, Audio.AudioManagement.SoundDataUnit.dB)
                    Else
                        Throw New Exception("Unexpected error in SmaComponent.GetCurrentGain. Are all segmenations in the SMA components properly validated?")
                    End If
                Next
            Next


            'Filters all sounds using the filter kernel produced by CreateSpeechFilterKernel
            Dim LoadedSounds = ParentTestSpecification.SpeechMaterial.GetAllLoadedSounds()

            If SpeechFilterSounds = True Then
                Dim SpeechFilterKernel = CreateSpeechFilterKernel(CurrentWaveFormat,)
                For Each RecordingKpv In LoadedSounds
                    'Copying the SMA object and file name
                    Dim Recording = RecordingKpv.Value
                    Dim LocalSmaRef = Recording.SMA
                    Dim LocalFilename = RecordingKpv.Value.FileName

                    'Filtering
                    Recording = Audio.DSP.FIRFilter(Recording, SpeechFilterKernel, New Audio.Formats.FftFormat, SoundChannel,,,,, True, True)

                    'Re-referencing the SMA object
                    Recording.SMA = LocalSmaRef

                    'And copying the file name
                    Recording.FileName = LocalFilename
                Next
            End If
            '
            For Each SmcID In TempSoundLib

                'Calculating average sound levels for all recordings of the same speech material component (but trimming the lowest and highest values)
                Dim SmcAverageLevelList As New List(Of Double)

                For r = 0 To SmcID.Value.Item2.Count - 1

                    Dim SmaComponent = SmcID.Value.Item2(r)
                    Dim Recording = SmaComponent.ParentSMA.ParentSound

                    If TemporalIntegration = 0 Then

                        SmcAverageLevelList.Add(Audio.DSP.MeasureSectionLevel(Recording, SoundChannel,
                                                                          SmaComponent.StartSample,
                                                                          SmaComponent.Length,
                                                                            Audio.AudioManagement.SoundDataUnit.dB,
                                                                            Audio.SoundMeasurementType.RMS,
                                                                                    FrequencyWeighting))

                    Else

                        SmcAverageLevelList.Add(Audio.DSP.GetLevelOfLoudestWindow(Recording, SoundChannel,
                                                                          TemporalIntegration * Recording.WaveFormat.SampleRate, SmaComponent.StartSample,
                                                                          SmaComponent.Length,, FrequencyWeighting))

                    End If

                Next

                'Trimming the lowest and highest values, and calculating average of the remaining values (trimming is skipped if there is less than 3 recorings of a speech material component)
                Dim TrimmedAverageList As New List(Of Double)
                For n = 0 To SmcAverageLevelList.Count - 1
                    TrimmedAverageList.Add(SmcAverageLevelList(n))
                Next
                If TrimmedAverageList.Count > 2 Then
                    TrimmedAverageList.Sort()
                    TrimmedAverageList.RemoveAt(0)
                    TrimmedAverageList.RemoveAt(TrimmedAverageList.Count - 1)
                End If
                Dim SmcAverageLevel As Double = TrimmedAverageList.Average


                'Setting all speech material component recordings to the average sound level
                For r = 0 To SmcID.Value.Item2.Count - 1
                    Dim SmaComponent = SmcID.Value.Item2(r)
                    Dim Recording = SmaComponent.ParentSMA.ParentSound

                    'Getting the current sound level from the averaging list measured above (instead of calculating it again)
                    Dim CurrentSoundLevel As Double = SmcAverageLevelList(r)

                    'CurrentSoundLevel + NeededGain = SpeakerAverageLevel
                    Dim NeededGain As Double = SmcAverageLevel - CurrentSoundLevel

                    'Applying gain to set all recordings of the same word by the same speaker to the same average level
                    Audio.DSP.AmplifySection(Recording, NeededGain)

                Next
            Next

            'Applying (the same amount of) gain to all components so that the average unweighted sound level of all recording level component recordings is the AverageTestWordOutputlevel.
            'Getting the current sound level of all component recordings as if they were concatenated

            Dim SharpTestingSoundList As New List(Of Audio.Sound)

            For Each SmcID In TempSoundLib

                'Excluding practise components from the overall sound level adjustment.
                If SmcID.Value.Item1 = True Then Continue For

                For r = 0 To SmcID.Value.Item2.Count - 1

                    Dim SmaComponent = SmcID.Value.Item2(r)

                    'Adds a copy of the sound recording
                    SharpTestingSoundList.Add(SmaComponent.GetSoundFileSection(SoundChannel))

                Next
            Next

            'Measuring the level of all test words concatenated
            Dim ConcatenatedSoundLevel As Double = GetSoundLevelOfConcatenatedSounds(SharpTestingSoundList, FrequencyWeighting, SoundChannel)

            'Adjusting each recording with the same gain to attain the correct average sound level
            Dim NeededGainForOutputLevel = Audio.Standard_dBSPL_To_dBFS(AverageTestWordOutputlevel) - ConcatenatedSoundLevel

            Utils.SendInfoToLog("Applying gain to test word recordings: " & NeededGainForOutputLevel & " dB",, ExportFolder)

            For Each SmcID In TempSoundLib
                For r = 0 To SmcID.Value.Item2.Count - 1

                    Dim SmaComponent = SmcID.Value.Item2(r)
                    Dim Recording = SmaComponent.ParentSMA.ParentSound

                    Audio.DSP.AmplifySection(Recording, NeededGainForOutputLevel, SoundChannel, SmaComponent.StartSample, SmaComponent.Length)

                Next
            Next


            'Verifying that the test words by the current speaker have the desired average sound level
            'Getting the current sound level of all test word recordings as if they were concatenated
            Dim LevelVerificationList As New List(Of Audio.Sound)

            For Each SmcID In TempSoundLib

                'Excluding practise components from the overall sound level adjustment.
                If SmcID.Value.Item1 = True Then Continue For

                For r = 0 To SmcID.Value.Item2.Count - 1

                    Dim SmaComponent = SmcID.Value.Item2(r)

                    'Adds a copy of the sound recording
                    LevelVerificationList.Add(SmaComponent.GetSoundFileSection(SoundChannel))

                Next
            Next

            'Measuring the level of all components concatenated
            Dim VerificationLevel As Double = GetSoundLevelOfConcatenatedSounds(LevelVerificationList, FrequencyWeighting, SoundChannel)
            Utils.SendInfoToLog("Verification: Level of recordings: " & VerificationLevel & " dB",, ExportFolder)

            'Exporting all adjusted sounds to ExportFolder
            For Each RecordingKpv In LoadedSounds
                Dim Recording = RecordingKpv.Value
                Dim LoadFilePath = RecordingKpv.Key
                Dim ExportSubPath = LoadFilePath.Replace(IO.Path.Combine(ParentTestSpecification.GetTestRootPath, MediaParentFolder), "")
                Dim SaveFilePath = IO.Path.Combine(ExportFolder, ExportSubPath.Trim(IO.Path.DirectorySeparatorChar))
                Audio.AudioIOs.SaveToWaveFile(Recording, SaveFilePath)
            Next

        Catch ex As Exception
            MsgBox("An error occured in CreateNaturalLevels." & vbCrLf & ex.ToString)
        End Try

        'Resets the load type of sound files to the same type as when the sub was called
        SpeechMaterialComponent.AudioFileLoadMode = AudioFileLoadMode_StartValue

    End Sub


    Public Sub MeasureSmaObjectSoundLevels(ByVal IncludeCriticalBandLevels As Boolean,
                                           ByVal FrequencyWeighting As Audio.FrequencyWeightings,
                                           ByVal TemporalIntegrationDuration As Decimal,
                                           Optional ByVal ExportFolder As String = "",
                                           Optional ByVal SoundChannel As Integer = 1)

        'Temporarily sets the load type of sound files
        Dim AudioFileLoadMode_StartValue = SpeechMaterialComponent.AudioFileLoadMode
        SpeechMaterialComponent.AudioFileLoadMode = SpeechMaterialComponent.MediaFileLoadModes.LoadOnFirstUse

        Try

            'Setting a default export folder
            If ExportFolder = "" Then
                Dim fbd As New Windows.Forms.FolderBrowserDialog
                fbd.Description = "Select folder to store the new sound files"
                If fbd.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                    Exit Try
                End If

                ExportFolder = fbd.SelectedPath
                If ExportFolder = "" Then
                    Exit Try
                End If
            End If

            'Clears previously loaded sounds
            ParentTestSpecification.SpeechMaterial.ClearAllLoadedSounds()

            '(Re-) Loads sound files
            LoadAllSoundFIles(SoundChannel, True)

            'Getting the loaded sounds
            Dim LoadedSounds = ParentTestSpecification.SpeechMaterial.GetAllLoadedSounds()

            'Measuring all other levels that should be stored in the SMA objects
            For Each RecordingKpv In LoadedSounds
                'Copying the SMA object and file name
                Dim Recording = RecordingKpv.Value

                'Adding the new sound level format to all test word recordings, sample recordings masker sounds, as well as to the current SiBTestdata
                Recording.SMA.SetFrequencyWeighting(FrequencyWeighting, True)
                Recording.SMA.SetTimeWeighting(TemporalIntegrationDuration, True)

                'Measures sound levels
                Recording.SMA.MeasureSoundLevels(IncludeCriticalBandLevels, True, ExportFolder)

            Next

            'Exporting all adjusted sounds to ExportFolder
            For Each RecordingKpv In LoadedSounds
                Dim Recording = RecordingKpv.Value
                Dim LoadFilePath = RecordingKpv.Key
                Dim ExportSubPath = LoadFilePath.Replace(IO.Path.Combine(ParentTestSpecification.GetTestRootPath, MediaParentFolder), "")
                Dim SaveFilePath = IO.Path.Combine(ExportFolder, ExportSubPath.Trim(IO.Path.DirectorySeparatorChar))
                Audio.AudioIOs.SaveToWaveFile(Recording, SaveFilePath)
            Next

        Catch ex As Exception
            MsgBox("An error occured in MeasureSmaObjectSoundLevels." & vbCrLf & ex.ToString)
        End Try

        'Resets the load type of sound files to the same type as when the sub was called
        SpeechMaterialComponent.AudioFileLoadMode = AudioFileLoadMode_StartValue

    End Sub


    ''' <summary>
    ''' Calculates (SII critical bands) spectrum levels of the sound recordings of concatenated speech material components.
    ''' </summary>
    ''' <param name="ConcatenationLevel">The higher linguistic level (summary level) for which the resulting spectrum levels are calculated.</param>
    ''' <param name="SegmentsLevel">The (lower) linguistic level from which the sections to be concatenaded are taken.</param>
    ''' <param name="OnlyContrastingSegments">If set to true, only contrasting speech material components (e.g. contrasting phonemes in minimal pairs) will be included in the spectrum level calculations.</param>
    ''' <param name="SoundChannel">The audio / wave file channel in which the speech is recorded (channel 1, for mono sounds).</param>
    ''' <param name="SkipPractiseComponents">If set to true, speech material components marksed as practise components will be skipped in the spectrum level calculations.</param>
    ''' <param name="MinimumComponentDuration">An optional minimum duration (in seconds) of each included component. If the recorded sound of a component is shorter, it will be zero-padded to the indicated duration.</param>
    ''' <param name="ComponentCrossFadeDuration">A duration by which the sections for concatenations will be cross-faded prior to spectrum level calculations.</param>
    ''' <param name="FadeConcatenatedSound">If set to true, the concatenated sounds will be slightly faded initially and finally (in order to avoid impulse-like onsets and offsets) prior to spectrum level calculations.</param>
    ''' <param name="RemoveDcComponent">If set to true, the DC component of the concatenated sounds will be set to zero prior to spectrum level calculations.</param>
    Public Sub CalculateConcatenatedComponentSpectrumLevels(ByVal ConcatenationLevel As SpeechMaterialComponent.LinguisticLevels,
                                                   ByVal SegmentsLevel As SpeechMaterialComponent.LinguisticLevels,
                                                   ByVal OnlyContrastingSegments As Boolean,
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
        ParentTestSpecification.SpeechMaterial.ClearAllLoadedSounds()

        Dim SummaryComponents = Me.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(ConcatenationLevel)

        For Each SummaryComponent In SummaryComponents

            'Getting concatenated sounds
            Dim ConcatenatedSound = SummaryComponent.GetConcatenatedComponentsSound(Me, SegmentsLevel, OnlyContrastingSegments, SoundChannel, SkipPractiseComponents, MinimumComponentDuration, ComponentCrossFadeDuration,
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

        'Finally writes the results to file
        Dim OutputDirectory = Me.WriteCustomVariables()

        'Send info about calculation to log (only if WriteCustomVariables returned an output folder)
        If OutputDirectory <> "" Then
            Dim LogList As New List(Of String)
            LogList.Add("Method name: " & System.Reflection.MethodInfo.GetCurrentMethod.Name)
            LogList.Add("dBSPL to  FS difference used :" & dBSPL_FSdifference.ToString)
            LogList.Add("Filter specifications (Critical bands based on the SII standard):")
            LogList.Add(String.Join(vbTab, New List(Of String) From {"Band", "CentreFrequency", "LowerFrequencyLimit", "UpperFrequencyLimit", "Bandwidth", "ActualLowerLimitFrequency", "ActualUpperLimitFrequency"}))
            For b = 0 To BandBank.Count - 1
                LogList.Add(String.Join(vbTab, New List(Of String) From {CDbl((b + 1)), BandBank(b).CentreFrequency, BandBank(b).LowerFrequencyLimit, BandBank(b).UpperFrequencyLimit, BandBank(b).Bandwidth, ActualLowerLimitFrequencyList(b), ActualUpperLimitFrequencyList(b)}))
            Next
            Utils.SendInfoToLog(String.Join(vbCrLf, LogList), "Log_for_function_" & System.Reflection.MethodInfo.GetCurrentMethod.Name, OutputDirectory, False)
        End If


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
        ParentTestSpecification.SpeechMaterial.ClearAllLoadedSounds()

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

        'Finally writes the results to file
        Dim OutputDirectory = Me.WriteCustomVariables()

        'Send info about calculation to log (only if WriteCustomVariables returned an output folder)
        If OutputDirectory <> "" Then
            Dim LogList As New List(Of String)
            LogList.Add("Method name: " & System.Reflection.MethodInfo.GetCurrentMethod.Name)
            LogList.Add("dBSPL to  FS difference used :" & dBSPL_FSdifference.ToString)
            LogList.Add("Filter specifications (Critical bands based on the SII standard):")
            LogList.Add(String.Join(vbTab, New List(Of String) From {"Band", "CentreFrequency", "LowerFrequencyLimit", "UpperFrequencyLimit", "Bandwidth", "ActualLowerLimitFrequency", "ActualUpperLimitFrequency"}))
            For b = 0 To BandBank.Count - 1
                LogList.Add(String.Join(vbTab, New List(Of String) From {CDbl((b + 1)), BandBank(b).CentreFrequency, BandBank(b).LowerFrequencyLimit, BandBank(b).UpperFrequencyLimit, BandBank(b).Bandwidth, ActualLowerLimitFrequencyList(b), ActualUpperLimitFrequencyList(b)}))
            Next
            Utils.SendInfoToLog(String.Join(vbCrLf, LogList), "Log_for_function_" & System.Reflection.MethodInfo.GetCurrentMethod.Name, OutputDirectory, False)
        End If


    End Sub


    Public Sub CalculateAverageMaxLevelOfContrastingComponents(ByVal SummaryLevel As SpeechMaterialComponent.LinguisticLevels,
                                                               ByVal ContrastLevel As SpeechMaterialComponent.LinguisticLevels,
                                                               ByVal SoundChannel As Integer,
                                                               ByVal SkipPractiseComponents As Boolean,
                                                               Optional ByVal IntegrationTime As Double = 0.05,
                                                               Optional ByVal FrequencyWeighting As Audio.FrequencyWeightings = Audio.FrequencyWeightings.Z,
                                                               Optional ByVal VariableName As String = "RLxs")


        Dim WaveFormat As Audio.Formats.WaveFormat = Nothing

        'Clears previously loaded sounds
        ParentTestSpecification.SpeechMaterial.ClearAllLoadedSounds()

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

                'Determine if is contraisting component??
                If TargetComponents(c).IsContrastingComponent = False Then
                    Continue For
                End If

                For i = 0 To MediaAudioItems - 1
                    CurrentSmaComponentList.AddRange(TargetComponents(c).GetCorrespondingSmaComponent(Me, i, SoundChannel))
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

        'Finally writes the results to file
        Me.WriteCustomVariables()

    End Sub


    Public Sub CalculateAverageDurationOfContrastingComponents(ByVal SummaryLevel As SpeechMaterialComponent.LinguisticLevels,
                                                               ByVal ContrastLevel As SpeechMaterialComponent.LinguisticLevels,
                                                               ByVal SoundChannel As Integer,
                                                               Optional ByVal VariableName As String = "Tc")


        'Clears previously loaded sounds
        ParentTestSpecification.SpeechMaterial.ClearAllLoadedSounds()

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
                    CurrentSmaComponentList.AddRange(TargetComponents(c).GetCorrespondingSmaComponent(Me, i, SoundChannel))
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

        'Finally writes the results to file
        Me.WriteCustomVariables()

    End Sub


    Public Sub CalculateAverageComponentLevel(ByVal TargetComponentsLevel As SpeechMaterialComponent.LinguisticLevels,
                                                               ByVal SoundChannel As Integer,
                                                               Optional ByVal IntegrationTime As Double = 0,
                                                               Optional ByVal FrequencyWeighting As Audio.FrequencyWeightings = Audio.FrequencyWeightings.Z,
                                                               Optional ByVal VariableName As String = "Lc",
                                              Optional ByVal AverageDecibelValues As Boolean = False)


        Dim WaveFormat As Audio.Formats.WaveFormat = Nothing

        'Clears previously loaded sounds
        ParentTestSpecification.SpeechMaterial.ClearAllLoadedSounds()

        Dim SummaryComponents = Me.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(TargetComponentsLevel)

        For Each SummaryComponent In SummaryComponents

            'Get the SMA components representing the sound sections of all target components
            Dim CurrentSmaComponentList As New List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)
            For i = 0 To MediaAudioItems - 1
                CurrentSmaComponentList.AddRange(SummaryComponent.GetCorrespondingSmaComponent(Me, i, SoundChannel))
            Next

            'Skipping to next Summary component if no
            If CurrentSmaComponentList.Count = 0 Then Continue For

            'Getting the actual sound sections and measures their levels
            Dim SoundLevelList As New List(Of Double)
            For Each SmaComponent In CurrentSmaComponentList

                Dim CurrentSoundSection = (SmaComponent.GetSoundFileSection(SoundChannel))

                'Getting the WaveFormat from the first available sound
                If WaveFormat Is Nothing Then WaveFormat = CurrentSoundSection.WaveFormat

                If IntegrationTime = 0 Then
                    SoundLevelList.Add(Audio.DSP.MeasureSectionLevel(CurrentSoundSection, 1, ,,,, FrequencyWeighting))
                Else
                    SoundLevelList.Add(Audio.DSP.GetLevelOfLoudestWindow(CurrentSoundSection, 1, CurrentSoundSection.WaveFormat.SampleRate * IntegrationTime,,,, FrequencyWeighting, True))
                End If

            Next

            'Storing the average level
            Dim AverageLevel As Double
            If SoundLevelList.Count > 0 Then

                If AverageDecibelValues = True Then
                    'Averaging the decibel values
                    AverageLevel = SoundLevelList.Average

                Else

                    'Calculating the average level (not average dB, but instead average RMS, as if the sounds were concatenated)
                    'Converting to linear RMS
                    Dim RMSList As New List(Of Double)
                    For Each Level In SoundLevelList
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
                End If

            Else
                AverageLevel = Double.NegativeInfinity
            End If

            'Stores the value as a custom media set variable
            SummaryComponent.SetNumericMediaSetVariableValue(Me, VariableName, AverageLevel)

        Next

        'Finally writes the results to file
        Me.WriteCustomVariables()

    End Sub


    Public Enum MaskerSourceTypes
        RandomNoise
        SpeechMaterial
        ExternalSoundFilesBestMatch
        ExternalSoundFileShroeder
    End Enum



    ''' <summary>
    ''' Creates masker sound files for a new testing situation.
    ''' </summary>
    ''' <param name="SegmentsLevel">The linguistic level from which segments that form the spectrum templates for the new masker sounds are taken.</param>
    ''' <param name="MaskerCoverageLevel">The linguistic level on which the masker sounds should be used. This is only used to set the duration of the central / masking region of the output sound files. 
    ''' If left to Nothing the parameter MaskerSoundDuration, instead, determines the duration of the central / masking regions of the output sound files.</param>
    ''' <param name="OnlyContrastingSegments">If set to False, all constituent segments within each Summary level components are used to establish spectrum templates. 
    ''' If set to True, only phonetically / phonemically contrasting components are used. Setting it to True, requires that the speech material is structured to contain comparable constituents.</param>
    ''' <param name="SoundChannel">Use 1 for mono sounds. (Stereo sounds will most probably not work yet...)</param>
    ''' <param name="SkipPractiseComponents">Excludes segments that belong to components marked as practise components from beling included in the segment concatenation.</param>
    ''' <param name="MinimumSegmentDuration">Can be used to zero-pad each segment at the SegmentsLevel to MinimumSegmentDuration. If set to zero, or if a segment is longer than MinimumSegmentDuration, no zero-padding is performed.</param>
    ''' <param name="ComponentCrossFadeDuration">The cross-fade duration (in seconds) used when concatenating segments. This can be set to a very small value (such as 0.01), by which sound sharp impulses can be avoided. But extensive cross-fades will impact negatively on the accuracy of the spectrum templates.</param>
    ''' <param name="FadeConcatenatedSound">Set to true to very slightly fade in and fade out the sound containing the concatenated segments, in order to avoid sharp sound 'edges'.</param>
    ''' <param name="RemoveSegmentDcComponents">Removes the DC component of each included segment (prior to concatenation).</param>
    ''' <param name="MaskerSoundDuration">The fixed duration of resulting masker sounds (in seconds)</param>
    ''' <param name="CoarticulationMargin">A time period which will be added on each side of the longest summary level component, in order to set the duration of the central masker region</param>
    ''' <param name="OutputSoundCount">The number of masker sounds created for each summary level component</param>
    ''' <param name="ExtraSoundCount">The number of extra masker sounds that will be created and exported (to the log folder) for each summary level component. These can be used if some of the ordinary output sounds need to be replaced</param>
    ''' <param name="OutputLevel_FS">The average (unweighted) RMS sound level (in dB FS) in each each resulting masker sound.</param>
    Public Sub CreateNewTestSituationMaskers(ByVal MaskerSourceType As MaskerSourceTypes,
                                             Optional ByVal SegmentsLevel As SpeechMaterialComponent.LinguisticLevels = SpeechMaterialComponent.LinguisticLevels.Phoneme,
                                             Optional ByVal MaskerCoverageLevel As Nullable(Of SpeechMaterialComponent.LinguisticLevels) = Nothing,
                                             Optional ByVal OnlyContrastingSegments As Boolean = True,
                                             Optional ByVal SoundChannel As Integer = 1,
                                             Optional ByVal SkipPractiseComponents As Boolean = False,
                                             Optional ByVal MinimumSegmentDuration As Double = 0,
                                             Optional ByVal ComponentCrossFadeDuration As Double = 0.001,
                                             Optional ByVal FadeConcatenatedSound As Boolean = True,
                                             Optional ByVal RemoveSegmentDcComponents As Boolean = True,
                                             Optional ByVal MaskerSoundDuration As Double = 3,
                                             Optional ByVal CoarticulationMargin As Double = 0,
                                             Optional ByVal OutputSoundCount As Integer = 5,
                                             Optional ByVal ExtraSoundCount As Integer = 10,
                                             Optional ByVal OutputLevel_FS As Double = -30)

        Dim SmaHighjackedSentenceIndex As Integer = 0

        If SegmentsLevel < Me.SharedMaskersLevel Then
            MsgBox("Function " & System.Reflection.MethodInfo.GetCurrentMethod.Name & " says: SummaryLevel must be a linguistically more detailed level than SegmentsLevel.")
            Exit Sub
        End If

        Dim myProgressDisplay As New ProgressDisplay

        Dim SoundLevelFormat As New Audio.Formats.SoundLevelFormat(Audio.SoundMeasurementTypes.Average_C_Weighted)

        'TODO: Mayby we should simply write different functions for the different MaskerSourceTypes instead of implememting everyting here???!!!
        Select Case MaskerSourceType
            Case MaskerSourceTypes.ExternalSoundFilesBestMatch
                'This is the type implemented in this function
            Case MaskerSourceTypes.SpeechMaterial
                Throw New NotImplementedException
            Case MaskerSourceTypes.RandomNoise
                Throw New NotImplementedException
            Case MaskerSourceTypes.ExternalSoundFileShroeder
                Throw New NotImplementedException
        End Select

        'Settings options that are fixed but could possibly be made available to the calling code
        Dim PerformInitialCorrelation As Boolean = True
        Dim FilterInputSounds As Boolean = True
        Dim RemoveInputSoundDcComponent As Boolean = True
        Dim CompareSoundCategories As Boolean = True ' Sound categories are defined by the initial letter characters in each input files name. Thus, if CompareSoundCategories is True, input sound files cannot start by numeric characters.
        Dim DistributeSoundCategoriesBetweenSummaryComponents As Boolean = True
        Dim SoundCategoriesToCompareCount As Integer = 3
        If CompareSoundCategories = False Then DistributeSoundCategoriesBetweenSummaryComponents = False 'Overriding DistributeTypesBetweenGroups, since it is not meaningful if sound categories are not used.

        'Sound similarity settings
        Dim ReusableCentreFrequencies As SortedSet(Of Single) = Nothing
        Dim FFT_AnalysisWindowLength As Integer = 2048
        Dim FFT_OverlapLength As Integer = 1024
        Dim FftFormat As New Audio.Formats.FftFormat(FFT_AnalysisWindowLength,, FFT_OverlapLength, Audio.WindowingType.Hamming, False)
        Dim BarkFilterOverlapRatio As Double = 0.5
        Dim LowestIncludedCentreFrequency As Double = 80
        Dim HighestIncludedCentreFrequency As Double = 15000
        Dim IrrelevantDifferenceThreshold As Single? = Nothing '60
        Dim MaxMaskerSpectralGain As Single? = 30
        Dim MaxMaskerSpectralAttenuation As Single? = 40
        Dim MaxSoundLevelRange As Single? = 6
        Dim MinimumIncludedRelativeMaxLevel As Double? = 40
        Dim MaximumAcceptedSoundLevelVariation As Double? = 3

        'Fine tuning settings
        Dim UseSpectralUseFineTuning As Boolean = True
        Dim MaxBandGain As Double? = 3
        Dim MaxBandAttenuation As Double? = 3
        Dim SpectralShapingLowerCutOff As Double? = Nothing
        Dim SpectralShapingUpperCutOff As Double? = Nothing
        Dim FirKernelLength As Integer = 4000
        Dim IrFftFormat As New Audio.Formats.FftFormat(4096,,, Audio.WindowingType.Hamming, False)
        Dim FirFftFormat As New Audio.Formats.FftFormat(4096,,, Audio.WindowingType.Hamming, False)

        'Fade and limiter settings
        Dim RelativeMaxLevelAllowed As Double = 4

        'Output spectra settings
        Dim HighRes_OutputSpectra_FftFormat As New Audio.Formats.FftFormat(4096,,, Audio.WindowingType.Hamming)
        Dim LowRes_OutputSpectra_FftFormat As New Audio.Formats.FftFormat(1024,,, Audio.WindowingType.Hamming)

        'Getting the input sounds
        'Asking the user for an input folder
        Dim fbd As New Windows.Forms.FolderBrowserDialog
        fbd.Description = "Select a folder containing the environment sounds!"
        Dim fbdResult = fbd.ShowDialog
        'Dim InputFolder As String = "C:\EriksDokument\SiBSoundFiles\TestPhonemeMaskers\Gloaguen_Sounds\SelectedEvents" 'This is the folder used for the SiB-test city sounds
        Dim InputFolder As String = ""
        If fbdResult = Windows.Forms.DialogResult.OK Then
            InputFolder = fbd.SelectedPath
        Else
            MsgBox("No folder environment sound folder selected. Press enter to exit!")
            Exit Sub
        End If

        'Setting an export folder
        'Asking the user for an input folder
        Dim output_fbd As New Windows.Forms.FolderBrowserDialog
        output_fbd.Description = "Select an output folder!"
        Dim output_fbdResult = output_fbd.ShowDialog
        Dim ExportFolder As String = ""
        If output_fbdResult = Windows.Forms.DialogResult.OK Then
            ExportFolder = output_fbd.SelectedPath
        Else
            MsgBox("No output folder selected. Using defualt log path!")
            ExportFolder = Utils.logFilePath
        End If

        'Adding a sub-directory to the ExportFolder base on the MaskerParentFolder, if specified
        If Me.MaskerParentFolder.Trim.Length > 0 Then
            Dim MaskerParentFolderSplit = Me.MaskerParentFolder.Split(IO.Path.DirectorySeparatorChar)
            Dim MaskerParentFolderEnd As String = MaskerParentFolderSplit(MaskerParentFolderSplit.Length - 1) '
            ExportFolder = IO.Path.Combine(ExportFolder, MaskerParentFolderEnd)
        End If

        ' Section I - masker sounds
        Dim InputSounds As New List(Of Audio.Sound)
        Audio.AudioIOs.ReadMultipleWaveFiles(InputSounds,, InputFolder)

        'Replacing the input sounds by mono-sounds by skipping any channel about channel 1
        'And zero padding short sounds to at least MaskerSoundDuration
        Dim TempInputSounds As New List(Of Audio.Sound)
        For Each InputSound In InputSounds
            Dim SoundCopy = Audio.DSP.CopySection(InputSound,,, 1)
            SoundCopy.FileName = InputSound.FileName
            SoundCopy.ZeroPad(MaskerSoundDuration, False)
            TempInputSounds.Add(SoundCopy)
        Next
        InputSounds = TempInputSounds

        Dim Progress As Integer = 0

        If PerformInitialCorrelation = True Then

            'Starting a progress window
            myProgressDisplay = New ProgressDisplay
            myProgressDisplay.Initialize(InputSounds.Count, 0, "Performing initial section correlations...")
            myProgressDisplay.Show()
            Progress = 0

            Dim AddedSounds As New List(Of Audio.Sound)
            Dim InitialSectionArrays As New SortedList(Of String, Single())
            Dim ExcludedSoundFileNames As New List(Of String)

            For Each InputSound In InputSounds
                'Updating progress
                myProgressDisplay.UpdateProgress(Progress)
                Progress += 1

                'Copying the initial 400 ms of each file 
                Dim CurrentInputSoundInitialSection(Int(0.4 * InputSound.WaveFormat.SampleRate - 1)) As Single
                Dim SourceArray = InputSound.WaveData.SampleData(1)
                For s = 0 To Math.Min(SourceArray.Length, CurrentInputSoundInitialSection.Length) - 1
                    CurrentInputSoundInitialSection(s) = SourceArray(s)
                Next

                'Correlating the sound with all sounds previously added
                Dim ExcludeSound As Boolean = False
                Dim SoundThatCausedExclusion As String = ""
                Dim ExclusionCorrelation As Double = 0
                For Each AddedArray In InitialSectionArrays

                    Dim r = Utils.Math.PearsonsCorrelation(CurrentInputSoundInitialSection, AddedArray.Value)

                    If Math.Abs(r) > 0.8 Then
                        ExcludeSound = True
                        SoundThatCausedExclusion = AddedArray.Key
                        ExclusionCorrelation = r
                        Exit For
                    End If
                Next

                If ExcludeSound = False Then
                    'Adding the correlation array
                    InitialSectionArrays.Add(InputSound.FileName, CurrentInputSoundInitialSection)

                    'Adding the input sound
                    AddedSounds.Add(InputSound)
                Else

                    'Storing the filename of excluded sounds
                    ExcludedSoundFileNames.Add(InputSound.FileName & vbTab & SoundThatCausedExclusion & vbTab & ExclusionCorrelation)
                End If

            Next
            'Closing the progress display
            myProgressDisplay.Close()

            'Logging excluded file names
            Utils.SendInfoToLog("Names of sound files excluded due to high initial correlation with added sounds:" & vbCrLf &
                          "Excluded sound" & vbTab & "Correlated with" & vbTab & "Pearsons r" & vbCrLf &
                          String.Join(vbCrLf, ExcludedSoundFileNames), "ExcludedSoundFileNames", IO.Path.Combine(ExportFolder, "Log"),, True)

            InputSounds = AddedSounds
        End If

        'Filterring the input sounds
        If FilterInputSounds = True Or RemoveInputSoundDcComponent = True Then

            'Creating a low pass FIR filter kernel
            Dim InputSoundFilterKernel As Audio.Sound = Nothing
            If FilterInputSounds = True Then
                InputSoundFilterKernel = CreateSpeechFilterKernel(InputSounds(0).WaveFormat, True, IO.Path.Combine(ExportFolder, "Log")) 'Using the wave format of the first sound, requires all sounds to have the same format!
            End If

            'Starting a progress window
            myProgressDisplay = New ProgressDisplay
            myProgressDisplay.Initialize(InputSounds.Count, 0, "Filtering input sounds...")
            myProgressDisplay.Show()

            Progress = 0
            For s = 0 To InputSounds.Count - 1

                'Updating progress
                myProgressDisplay.UpdateProgress(Progress)

                'Storing the file name
                Dim CurrentFileName = InputSounds(s).FileName

                'Low-pass filtering using a FIR filter
                If FilterInputSounds = True Then
                    InputSounds(s) = Audio.DSP.FIRFilter(InputSounds(s), InputSoundFilterKernel, New Audio.Formats.FftFormat, 1,,,,, True, True)
                End If

                'Removing CD component
                If RemoveInputSoundDcComponent = True Then
                    Audio.DSP.RemoveDcComponent(InputSounds(s))
                End If

                'Restoring the file name
                InputSounds(s).FileName = CurrentFileName

                Progress += 1
            Next
            'Closing the progress display
            myProgressDisplay.Close()

        End If


        'Analying bark spectra of the input sounds
        'Starting a progress window
        myProgressDisplay = New ProgressDisplay
        myProgressDisplay.Initialize(InputSounds.Count, 0, "Analysing Bark spectra...")
        myProgressDisplay.Show()

        Progress = 0
        For Each InputSound In InputSounds
            'Updating progress
            myProgressDisplay.UpdateProgress(Progress)
            Audio.DSP.CalculateBarkSpectrum(InputSound, BarkFilterOverlapRatio,
                                                                    LowestIncludedCentreFrequency, HighestIncludedCentreFrequency,
                                                                    ReusableCentreFrequencies, FftFormat)
            Progress += 1
        Next
        'Closing the progress display
        myProgressDisplay.Close()


        ' Section II - speech recordings
        Dim SummaryLevelComponents = Me.ParentTestSpecification.SpeechMaterial.GetAllDescenentsAtLevel(Me.SharedMaskersLevel, True)

        'Getting a concatenation of the sound segments to match (e.g test phonemes in a minimal variation group)
        Dim MasterConcatList As New List(Of Tuple(Of SpeechMaterialComponent, Audio.Sound))

        For Each SummaryComponent In SummaryLevelComponents
            'Getting the concatenated sounds
            Dim ConcatenatedSound = SummaryComponent.GetConcatenatedComponentsSound(Me, SegmentsLevel, OnlyContrastingSegments, SoundChannel, SkipPractiseComponents, MinimumSegmentDuration, ComponentCrossFadeDuration,
                                                            FadeConcatenatedSound, RemoveSegmentDcComponents)
            'Storing it along with its speech material component
            MasterConcatList.Add(New Tuple(Of SpeechMaterialComponent, Audio.Sound)(SummaryComponent, ConcatenatedSound))
        Next

        'Starting a progress window
        myProgressDisplay = New ProgressDisplay
        myProgressDisplay.Initialize(SummaryLevelComponents.Count, 0, "Comparing sound files...")
        myProgressDisplay.Show()
        Progress = 0

        'Comparing sound files, and selecting the most suiting for each summary component. The masker sounds are not stored, but only their locations in the input masker sounds
        Dim MasterSoundData As New SortedList(Of String, Tuple(Of SpeechMaterialComponent, Audio.Sound, List(Of MaskerSoundCategoryData)))

        For Each SummaryComponentTuple In MasterConcatList

            'Updating progress
            myProgressDisplay.UpdateProgress(Progress)

            Dim SummaryComponent = SummaryComponentTuple.Item1
            Dim ConcatSoundsSound As Audio.Sound = SummaryComponentTuple.Item2

            'Getting the longest component to mask
            Dim MaskingRegionDuration As Double
            If MaskerCoverageLevel IsNot Nothing Then

                Dim MaskerCoverageComponents = SummaryComponent.GetAllDescenentsAtLevel(MaskerCoverageLevel)
                Dim LongestMaskerCoverageDuration As Double = 0
                For Each MCC In MaskerCoverageComponents
                    For i = 0 To Me.MediaAudioItems - 1
                        Dim MccSound = MCC.GetSound(Me, i, SoundChannel)
                        Dim MccLength = MccSound.WaveData.SampleData(SoundChannel).Length
                        Dim MccDuration As Double = MccLength / MccSound.WaveFormat.SampleRate
                        LongestMaskerCoverageDuration = Math.Max(LongestMaskerCoverageDuration, MccDuration)
                    Next
                Next

                MaskingRegionDuration = Math.Min(MaskerSoundDuration, LongestMaskerCoverageDuration + 2 * CoarticulationMargin) 'Setting MaskingLength, the central region of the sound
                Utils.SendInfoToLog("SummaryComponent: " & vbTab & SummaryComponent.Id & " (" & SummaryComponent.PrimaryStringRepresentation & ")" & vbTab &
                              "LongestMemberDuration: " & vbTab & LongestMaskerCoverageDuration & vbTab &
                              "MaskingRegionDuration: " & vbTab & MaskingRegionDuration,
                              "MaskerTimes", IO.Path.Combine(ExportFolder, "Log"), True, True)

            Else
                'Setting MaskingRegionDuration (almost) to MaskerSoundDuration which means that basically the whole sound will be the masking / central region.
                'The few samples on each side is just there so that the code will work, as these regions (the FadeInRegion and the FadeOutRegion) are expected at other places in the code.
                MaskingRegionDuration = Math.Max(0, MaskerSoundDuration - 0.001)

            End If


            Dim SoundCategoryList As New SortedList(Of String, MaskerSoundCategoryData)

            'Calculating the bark spectrum of the ConcatSoundsSound
            Audio.DSP.CalculateBarkSpectrum(ConcatSoundsSound, BarkFilterOverlapRatio,
                                                                    LowestIncludedCentreFrequency, HighestIncludedCentreFrequency,
                                                                    ReusableCentreFrequencies, FftFormat)

            'Calculating the average Bark spectrum of the concatenated sounds
            Dim AverageSpectralLevel_ConcatComponents As Double
            Dim ConcatAverageBarkSpectrum = Audio.DSP.GetAverageBarkSpectrum(ConcatSoundsSound,,, AverageSpectralLevel_ConcatComponents)

            'Getting the average Bark spectrum section by section in the InputSounds
            For InputSoundIndex = 0 To InputSounds.Count - 1

                Dim InputSound = InputSounds(InputSoundIndex)

                'Creating a SoundCategory  name
                Dim SoundCategory As String = ""
                If CompareSoundCategories = True Then

                    'Reading input file letters until the first numeric character is detected
                    Dim LastnonNumericIndex As Integer = -1
                    For CharPos = 0 To InputSound.FileName.Length - 1
                        If IsNumeric(InputSound.FileName(CharPos)) = False Then
                            LastnonNumericIndex = CharPos
                        Else
                            'Numeric detected, exits loop
                            Exit For
                        End If
                    Next

                    If LastnonNumericIndex > -1 Then
                        'Storing the SoundCategory as the initial non numeric letters
                        SoundCategory = InputSound.FileName.Substring(0, LastnonNumericIndex + 1)
                    Else
                        'Overriding any empty SoundCategory assignment, which occurs with filenames that start on a numeral
                        If SoundCategory = "" Then SoundCategory = "NumericFileName"
                    End If
                Else
                    SoundCategory = "Default"
                End If

                'Calculating the max window level
                Dim MaskerSoundWindowLevels = Audio.DSP.AcousticDistance.CalculateWindowLevels(InputSound)
                Dim MaskerSoundWindowMaxLevel = MaskerSoundWindowLevels.Max

                Dim StepLength As Integer = FFT_AnalysisWindowLength - FFT_OverlapLength
                Dim MaskingLength As Integer = MaskingRegionDuration * InputSound.WaveFormat.SampleRate
                Dim MaskerRegionLengthInWindows As Integer = Math.Ceiling((MaskingLength - FFT_AnalysisWindowLength) / StepLength)
                Dim AvailableWindowsCount As Integer = Math.Max(1, Math.Floor(InputSound.WaveData.SampleData(1).Length - FFT_AnalysisWindowLength) / StepLength)

                For w = 0 To AvailableWindowsCount - MaskerRegionLengthInWindows - 1 Step MaskerRegionLengthInWindows

                    'Comparing section w of the input file, with the concatenated components Bark spectrum

                    'Getting the current window levels
                    Dim MaskerWindowLevels(MaskerRegionLengthInWindows - 1) As Double
                    For c = 0 To MaskerWindowLevels.Length - 1
                        MaskerWindowLevels(c) = MaskerSoundWindowLevels(c + w)
                    Next

                    'Calculating the current comparison Bark spectrum
                    Dim AverageSpectralLevel_Masker As Double
                    Dim CurrentMaskerBarkSpectrum = Audio.DSP.AcousticDistance.GetAverageBarkSpectrum(InputSound, w, MaskerRegionLengthInWindows, AverageSpectralLevel_Masker)

                    'Evaluating and modifying the current CurrentComparisonBarkSpectrum
                    If MaxSoundLevelRange.HasValue Then
                        Dim SoundLevelRange = MaskerWindowLevels.Max - MaskerWindowLevels.Min
                        If SoundLevelRange > MaxSoundLevelRange Then
                            Continue For
                        End If
                    End If

                    If MaximumAcceptedSoundLevelVariation.HasValue Then
                        'Calculating SD of the window levels
                        Dim SoundLevelVariation As Double
                        Utils.Math.CoefficientOfVariation(MaskerWindowLevels.ToList,,,,, SoundLevelVariation)
                        If SoundLevelVariation > MaximumAcceptedSoundLevelVariation Then
                            Continue For
                        End If
                    End If

                    If MinimumIncludedRelativeMaxLevel.HasValue Then
                        'Comparing the current max level to the max level of the whole sound
                        If MaskerWindowLevels.Max < (MaskerSoundWindowMaxLevel - MinimumIncludedRelativeMaxLevel) Then
                            Continue For
                        End If
                    End If

                    'Equalizing the average spectral levels, by changing CurrentComparisonBarkSpectrum
                    Dim CurrentGain = AverageSpectralLevel_ConcatComponents - AverageSpectralLevel_Masker

                    'Limiting the gain
                    If MaxMaskerSpectralGain.HasValue = True Then
                        CurrentGain = Math.Min(MaxMaskerSpectralGain.Value, CurrentGain)
                    End If
                    If MaxMaskerSpectralAttenuation.HasValue = True Then
                        CurrentGain = Math.Max(-MaxMaskerSpectralAttenuation.Value, CurrentGain)
                    End If

                    'Applying gain
                    If CurrentGain <> 0F Then
                        For i = 0 To CurrentMaskerBarkSpectrum.WindowData.Length - 1
                            CurrentMaskerBarkSpectrum.WindowData(i) += CurrentGain
                        Next
                    End If

                    'Calculating distance
                    Dim AcousticDistance As Double
                    If IrrelevantDifferenceThreshold Is Nothing Then
                        AcousticDistance = Utils.Math.GetEuclideanDistance(ConcatAverageBarkSpectrum.WindowData, CurrentMaskerBarkSpectrum.WindowData)
                    Else
                        AcousticDistance = Utils.Math.GetEuclideanDistance(ConcatAverageBarkSpectrum.WindowData, CurrentMaskerBarkSpectrum.WindowData, IrrelevantDifferenceThreshold)
                    End If

                    'Skipping if acoustic distance is 0
                    If AcousticDistance = 0 Then Continue For

                    'Calculating input sound times
                    Dim InputSound_CentralRegionStartSample As Integer = w * (FFT_AnalysisWindowLength - FFT_OverlapLength)
                    Dim InputSound_CentralRegionLength As Integer = MaskingRegionDuration * InputSound.WaveFormat.SampleRate
                    Dim InputSoundLength As Integer = MaskerSoundDuration * InputSound.WaveFormat.SampleRate
                    Dim InputSoundStartSample As Integer = InputSound_CentralRegionStartSample - ((InputSoundLength - InputSound_CentralRegionLength) / 2)

                    'Checking that all margins are inside the input file
                    If InputSound_CentralRegionStartSample < 0 Then Continue For
                    If InputSound_CentralRegionLength > InputSoundLength Then Continue For
                    If InputSoundStartSample < 0 Then Continue For
                    If InputSound_CentralRegionStartSample - InputSoundStartSample < 0 Then Continue For
                    If InputSoundStartSample + InputSoundLength > InputSound.WaveData.SampleData(1).Length Then
                        Continue For
                    End If

                    'Adding the sound category to DistanceList
                    If Not SoundCategoryList.ContainsKey(SoundCategory) Then SoundCategoryList.Add(SoundCategory, New MaskerSoundCategoryData(SoundCategory))

                    'Adding the distance to DistanceList, along with data on where the sound was collected
                    SoundCategoryList(SoundCategory).MaskerSoundList.Add(
                            New MaskerSoundCategoryData.MaskerSoundData(InputSoundIndex, InputSoundStartSample, InputSoundLength,
                                                                        InputSound_CentralRegionStartSample - InputSoundStartSample,
                                                                        InputSound_CentralRegionLength, AcousticDistance))
                Next
            Next


            'Sorting each list in the DistanceList according to distance, and calculating the average distance in each category
            For Each SoundCategory In SoundCategoryList

                Dim Query1 = SoundCategory.Value.MaskerSoundList.OrderBy(Function(Distance) Distance.AcousticDistance)
                Dim mySortedList As New List(Of MaskerSoundCategoryData.MaskerSoundData)

                'Adding in sorted order
                For Each CurrentList In Query1
                    mySortedList.Add(CurrentList)
                Next

                'Selecting the best sounds
                SoundCategory.Value.MaskerSoundList.Clear()
                Dim SummedDistance As Double = 0
                Dim AddedSoundsCount As Integer = 0
                For n = 0 To Math.Min(mySortedList.Count, OutputSoundCount + ExtraSoundCount) - 1
                    SoundCategory.Value.MaskerSoundList.Add(mySortedList(n))

                    'Calculating average only for the original output sounds
                    If n < OutputSoundCount Then
                        SummedDistance += mySortedList(n).AcousticDistance
                        AddedSoundsCount += 1
                    End If
                Next

                'Adding the average distance of the selected sounds
                If AddedSoundsCount > 0 Then
                    SoundCategory.Value.AverageSoundDistance = SummedDistance / AddedSoundsCount
                End If

            Next

            'Selecting the most appropriate sound category, beginning by sorting the list according to rising average sound distance
            Dim SortedSoundCategoryList As New List(Of MaskerSoundCategoryData)
            Dim Query2 = SoundCategoryList.OrderBy(Function(Distance) Distance.Value.AverageSoundDistance)

            'Adding in sorted order
            Dim CurrentlyAdded As Integer = 0
            For Each CurrentList In Query2

                'Including only if there are enough sounds avaliable
                If CurrentList.Value.MaskerSoundList.Count >= OutputSoundCount Then
                    SortedSoundCategoryList.Add(CurrentList.Value)
                    CurrentlyAdded += 1
                    If CurrentlyAdded = SoundCategoriesToCompareCount Then Exit For
                End If
            Next

            'Adding the data to MasterSoundData
            MasterSoundData.Add(SummaryComponent.Id, New Tuple(Of SpeechMaterialComponent, Audio.Sound, List(Of MaskerSoundCategoryData))(SummaryComponent, ConcatSoundsSound, SortedSoundCategoryList))

            Progress += 1
        Next
        'Closing the progress display
        myProgressDisplay.Close()


        'Distibuting sound categories between SummaryComponents (only if there is more than one SummaryComponent to distribute categories to.
        If MasterSoundData.Count > 1 And DistributeSoundCategoriesBetweenSummaryComponents = True Then

            'Selecting the most equal distribution of sound categories between the SummaryComponents, whithin each voice
            Dim MyRandom As New Random(42)

            Dim BestRandomization As New List(Of Tuple(Of String, SpeechMaterialComponent, Audio.Sound, MaskerSoundCategoryData)) ' Id, SmComponent, concatenated sounds
            Dim BestRandomizationMaxDev As Double = Double.MaxValue

            'Counting the number of possible sound categories, and their number of occurences
            Dim PossibleCategories As New SortedSet(Of String)

            For Each SummaryComponentKvp In MasterSoundData
                For i = 0 To SummaryComponentKvp.Value.Item3.Count - 1
                    If PossibleCategories.Contains(SummaryComponentKvp.Value.Item3(i).SoundCategoryName) = False Then
                        PossibleCategories.Add(SummaryComponentKvp.Value.Item3(i).SoundCategoryName)
                    End If
                Next
            Next

            Dim PossibleCategoryCount As Integer = PossibleCategories.Count

            Dim ItarationCount As Integer = 1000000

            'Starting a progress window
            myProgressDisplay = New ProgressDisplay
            myProgressDisplay.Initialize(ItarationCount, 0, "Distributing sound categories between summary components...")
            myProgressDisplay.Show()

            For n = 1 To ItarationCount

                'Updating progress (on every 1000th loop
                If n Mod 1000 = 0 Then myProgressDisplay.UpdateProgress(n)

                Dim RandomizationList As New List(Of Tuple(Of String, SpeechMaterialComponent, Audio.Sound, MaskerSoundCategoryData))

                For Each SummaryComponentKvp In MasterSoundData

                    'Dim getting a random integer, to select a random sound category
                    Dim r = MyRandom.Next(0, SummaryComponentKvp.Value.Item3.Count)

                    'Selecting a random sound category
                    RandomizationList.Add(New Tuple(Of String, SpeechMaterialComponent, Audio.Sound, MaskerSoundCategoryData)(SummaryComponentKvp.Key, SummaryComponentKvp.Value.Item1, SummaryComponentKvp.Value.Item2, SummaryComponentKvp.Value.Item3(r)))

                Next

                'Counting how many times each extant sound category has been included
                Dim CountList As New SortedList(Of String, Integer)
                For Each TestList In RandomizationList
                    If CountList.ContainsKey(TestList.Item4.SoundCategoryName) = False Then
                        CountList.Add(TestList.Item4.SoundCategoryName, 0)
                    End If
                    CountList(TestList.Item4.SoundCategoryName) += 1
                Next

                'Checking that all possible categories are included, and skips to next randomization if some catagories were skipped
                If CountList.Keys.Count < PossibleCategoryCount Then
                    Continue For
                End If

                'Getting the Average inclusion count per category
                Dim Av As Double = CountList.Values.Average

                'Getting the max deviation from the average
                Dim LocalMaxDev As Double = 0
                For Each kvp In CountList
                    LocalMaxDev = Math.Max(LocalMaxDev, Math.Abs(Av - kvp.Value))
                Next

                'Comparing with earlier randomizations
                If LocalMaxDev < BestRandomizationMaxDev Then
                    BestRandomizationMaxDev = LocalMaxDev
                    BestRandomization = RandomizationList
                End If
            Next

            'Storing the best randomization
            MasterSoundData.Clear()
            For Each SummaryComponentKvp In BestRandomization
                MasterSoundData.Add(SummaryComponentKvp.Item1, New Tuple(Of SpeechMaterialComponent, Audio.Sound, List(Of MaskerSoundCategoryData))(
                                    SummaryComponentKvp.Item2, SummaryComponentKvp.Item3, New List(Of MaskerSoundCategoryData) From {SummaryComponentKvp.Item4}))
            Next

            'Logging the best randomization
            Dim LogGroupCount As Boolean = True
            If LogGroupCount = True Then

                'Counting the sound types
                Dim CountList As New SortedList(Of String, Integer)
                For Each SummaryComponentKvp In BestRandomization
                    If CountList.ContainsKey(SummaryComponentKvp.Item4.SoundCategoryName) = False Then
                        CountList.Add(SummaryComponentKvp.Item4.SoundCategoryName, 0)
                    End If
                    CountList(SummaryComponentKvp.Item4.SoundCategoryName) += 1
                Next

                Dim LogList As New List(Of String)
                For Each Group In CountList
                    LogList.Add(Group.Key & vbTab & Group.Value)
                Next

                Utils.SendInfoToLog(String.Join(vbCrLf, LogList) & vbCrLf & vbCrLf &
                           "PossibleCategories (" & PossibleCategories.Count & "):" & vbCrLf &
                           String.Join(vbCrLf, PossibleCategories) & vbCrLf,
                              "SelectedSoundCategoriesCount", IO.Path.Combine(ExportFolder, "Log"), True, True)

            End If

            'Closing the progress display
            myProgressDisplay.Close()
        End If


        'Starting a progress window
        myProgressDisplay = New ProgressDisplay
        myProgressDisplay.Initialize(MasterSoundData.Count * SummaryLevelComponents.Count, 0, "Creating masker sounds...")
        myProgressDisplay.Show()
        Progress = 0

        'Creating a list to store all ordinary sounds file paths, to use for creating a concatenated log-sound containing all exported sound
        Dim SoundExportList As New List(Of String)
        Dim ExtraSoundExportList As New List(Of String)
        Dim HighRes_SoundSpectra As New List(Of Tuple(Of String, SortedList(Of Double, Double)))
        Dim HighRes_ExtraSoundSpectra As New List(Of Tuple(Of String, SortedList(Of Double, Double)))

        Dim LowRes_SoundSpectra As New List(Of Tuple(Of String, SortedList(Of Double, Double)))
        Dim LowRes_ExtraSoundSpectra As New List(Of Tuple(Of String, SortedList(Of Double, Double)))

        'Creating the masker sound files

        'Updating progress
        myProgressDisplay.UpdateProgress(Progress)

        For Each SummaryComponentKvp In MasterSoundData

            Dim SummaryComponentString As String = SummaryComponentKvp.Key & "_" & SummaryComponentKvp.Value.Item1.GetMediaFolderName

            If SummaryComponentKvp.Value.Item3.Count = 0 Then
                MsgBox("Unable to create noise for speech material component: " & SummaryComponentString & vbCrLf &
                       "The reason may be that there are too few input sounds, or that they do not fulfill the quality criteria specified in the function " & System.Reflection.MethodInfo.GetCurrentMethod.Name & vbCrLf &
                       "Cannot continue!")
                Exit Sub
            End If

            Dim SoundCategoryData = SummaryComponentKvp.Value.Item3(0) 'Always using the first index, as this should always contain the data on sounds from the most suitable soundcategory

            'Creates a copy of the current concatenated sounds to be used to calculate a low resolution spectrum only.
            Dim ConcatSounds_LowRes = SummaryComponentKvp.Value.Item2.CreateSoundDataCopy()

            'Export the spectra of the test concatenated sounds - High resolution 
            Dim ConcatSounds = SummaryComponentKvp.Value.Item2
            ConcatSounds.FFT = Audio.DSP.SpectralAnalysis(ConcatSounds, HighRes_OutputSpectra_FftFormat, 1,,)
            ConcatSounds.FFT.CalculateAmplitudeSpectrum()
            Dim ConcatSpectrum = ConcatSounds.FFT.GetAverageSpectrum(1, Audio.FftData.SpectrumTypes.AmplitudeSpectrum,
                                                  ConcatSounds.WaveFormat, True)
            HighRes_SoundSpectra.Add(New Tuple(Of String, SortedList(Of Double, Double))(SummaryComponentString & vbTab & "ConcatSpectrum", ConcatSpectrum))
            HighRes_ExtraSoundSpectra.Add(New Tuple(Of String, SortedList(Of Double, Double))(SummaryComponentString & vbTab & "ConcatSpectrum", ConcatSpectrum))

            'Export the spectra of the test concatenated sounds - Low resolution 
            ConcatSounds_LowRes.FFT = Audio.DSP.SpectralAnalysis(ConcatSounds_LowRes, LowRes_OutputSpectra_FftFormat, 1,,)
            ConcatSounds_LowRes.FFT.CalculateAmplitudeSpectrum()
            Dim ConcatSpectrum_LowRes = ConcatSounds_LowRes.FFT.GetAverageSpectrum(1, Audio.FftData.SpectrumTypes.AmplitudeSpectrum,
                                                  ConcatSounds_LowRes.WaveFormat, True)
            LowRes_SoundSpectra.Add(New Tuple(Of String, SortedList(Of Double, Double))(SummaryComponentString & vbTab & "ConcatSpectrum_LowRes", ConcatSpectrum_LowRes))
            LowRes_ExtraSoundSpectra.Add(New Tuple(Of String, SortedList(Of Double, Double))(SummaryComponentString & vbTab & "ConcatSpectrum_LowRes", ConcatSpectrum_LowRes))

            'Measuring the RMS-level of the concatenated sounds
            Dim ConcatSoundLevel As Double = Audio.DSP.MeasureSectionLevel(ConcatSounds, 1,,,,, Audio.FrequencyWeightings.Z)

            'Copying the file sections into sounds
            For MaskerSoundIndex = 0 To SoundCategoryData.MaskerSoundList.Count - 1

                'Referencing the current list item
                Dim MaskerData = SoundCategoryData.MaskerSoundList(MaskerSoundIndex)

                Dim MaskerSound = Audio.DSP.CopySection(InputSounds(MaskerData.InputSoundIndex),
                                                        MaskerData.MaskerFileStartSample,
                                                        MaskerData.MaskerFileLength, 1)

                'Assigning SMA data (creating a new set of SMA components
                'And supplying the SoundSection sound with segmentation data, on sentence level, as well as a the following word level segmentations 
                '(word1: pre-measurement section, word2: measurement section, word 3: post measurement section)
                Dim CentralRegionStartSample As Integer = MaskerData.CentralRegionFileStartSample
                Dim CentralRegionLength As Integer = MaskerData.CentralRegionLength

                MaskerSound.SMA = New Audio.Sound.SpeechMaterialAnnotation
                MaskerSound.SMA.ParentSound = MaskerSound
                'Adding channel level data
                MaskerSound.SMA.AddChannelData(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(MaskerSound.SMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.CHANNEL, Nothing))

                'Adding sentence level data
                MaskerSound.SMA.ChannelData(1).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(MaskerSound.SMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE, MaskerSound.SMA.ChannelData(1)))
                MaskerSound.SMA.ChannelData(1).StartSample = 0
                MaskerSound.SMA.ChannelData(1).Length = MaskerSound.WaveData.SampleData(1).Length

                MaskerSound.SMA.ChannelData(1)(SmaHighjackedSentenceIndex).StartSample = 0
                MaskerSound.SMA.ChannelData(1)(SmaHighjackedSentenceIndex).Length = MaskerSound.WaveData.SampleData(1).Length

                'Adding word level data
                MaskerSound.SMA.ChannelData(1)(SmaHighjackedSentenceIndex).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(MaskerSound.SMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD,
                                                                                                                                     MaskerSound.SMA.ChannelData(1)(SmaHighjackedSentenceIndex)) With {
                                                                                                                                     .StartSample = 0, .Length = Math.Max(0, CentralRegionStartSample), .OrthographicForm = "FadeInRegion", .PhoneticForm = ""})

                MaskerSound.SMA.ChannelData(1)(SmaHighjackedSentenceIndex).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(MaskerSound.SMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD,
                                                                                                                                     MaskerSound.SMA.ChannelData(1)(SmaHighjackedSentenceIndex)) With {
                                                                                                                                     .StartSample = CentralRegionStartSample, .Length = CentralRegionLength, .OrthographicForm = "MaskerRegion", .PhoneticForm = ""})

                MaskerSound.SMA.ChannelData(1)(SmaHighjackedSentenceIndex).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(MaskerSound.SMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD,
                                                                                                                                     MaskerSound.SMA.ChannelData(1)(SmaHighjackedSentenceIndex)) With {
                                                                                                                                     .StartSample = CentralRegionStartSample + CentralRegionLength, .Length = MaskerSound.WaveData.SampleData(1).Length - (CentralRegionStartSample + CentralRegionLength), .OrthographicForm = "FadeOutRegion", .PhoneticForm = ""})


                If UseSpectralUseFineTuning = True Then

                    Dim SpectralSubtractionKernel As Audio.Sound = Nothing

                    'Creating a central region sound, or just using MaskerSound if no CentralRegionStartTime and CentralRegionDuration has been set
                    Dim CentralRegionSound As Audio.Sound = MaskerSound.CreateSoundDataCopy

                    'Cropping the central region
                    Audio.DSP.CropSection(CentralRegionSound, CentralRegionStartSample, CentralRegionLength)

                    'Prior to spectral subtraction, the average RMS in the masker center region is equalized to that of the concatenated sounds
                    Audio.DSP.MeasureAndAdjustSectionLevel(CentralRegionSound, ConcatSoundLevel, 1,,, Audio.FrequencyWeightings.Z)

                    'Using pure fft data to create an spectral shaping filter
                    SpectralSubtractionKernel = Audio.GenerateSound.GetImpulseResponseForSpectralShaping(SummaryComponentKvp.Value.Item2, CentralRegionSound,,,,,
                                                                                                             IrFftFormat, FirKernelLength, 1, MaxBandGain, MaxBandAttenuation, SpectralShapingLowerCutOff, SpectralShapingUpperCutOff)

                    'Filtering the (whole) masker sound
                    Dim FilteredMasker As Audio.Sound = Audio.DSP.FIRFilter(MaskerSound, SpectralSubtractionKernel, FirFftFormat,,,,,, True, True)

                    'Referencing the SMA object
                    FilteredMasker.SMA = MaskerSound.SMA

                    'Re-referencing the FilteredMasker as MaskerSound 
                    MaskerSound = FilteredMasker

                End If

                'Limiting the level in the whole masker sound to the average level of the central region + RelativeMaxLevelAllowed
                Dim CentralRegionLevel = Audio.DSP.MeasureSectionLevel(MaskerSound, 1, CentralRegionStartSample, CentralRegionLength)
                Audio.DSP.SoftLimitSection(MaskerSound, 1, CentralRegionLevel + RelativeMaxLevelAllowed, , ,
                                           0.4, Audio.FrequencyWeightings.Z, False,
                                           Audio.DSP.FadeSlopeType.Smooth)



                'Setting the output level
                Audio.DSP.MeasureAndAdjustSectionLevel(MaskerSound, OutputLevel_FS,,,, Audio.FrequencyWeightings.Z)

                'Assigns sound level format
                MaskerSound.SMA.SetFrequencyWeighting(SoundLevelFormat.FrequencyWeighting, True)
                MaskerSound.SMA.SetTimeWeighting(SoundLevelFormat.TemporalIntegrationDuration, True)

                'Re-measuring the SMA data
                MaskerSound.SMA.MeasureSoundLevels(False, True)

                'Exporting the sound
                Dim ExportPath As String
                Dim ExportFileName As String = Math.Round(MaskerData.AcousticDistance) & "_" & InputSounds(MaskerData.InputSoundIndex).FileName & "_" & MaskerSoundIndex
                If MaskerSoundIndex < OutputSoundCount Then
                    'Ordinary output sounds
                    ExportPath = IO.Path.Combine(ExportFolder, SummaryComponentString, ExportFileName)

                    'Storing the export path
                    SoundExportList.Add(ExportPath & ".wav")

                Else
                    'Extra output sounds
                    ExportPath = IO.Path.Combine(ExportFolder, "Log", "ExtraSounds", SummaryComponentString, ExportFileName)

                    'Storing the export path
                    ExtraSoundExportList.Add(ExportPath & ".wav")
                End If

                'Saving to wave file
                Audio.AudioIOs.SaveToWaveFile(MaskerSound, ExportPath)

                'Analysing the masker region spectra of the sound files
                'Copying the central region to a new sound
                Dim MaskerRegionSound = Audio.DSP.CopySection(MaskerSound, CentralRegionStartSample, CentralRegionLength, 1)

                ''Exptending the MaskerRegionSound to the length of the concatenades sounds
                Dim ExtendTimes As Integer = Math.Ceiling(ConcatSounds.WaveData.SampleData(1).Length / MaskerRegionSound.WaveData.SampleData(1).Length)
                Dim ExtendList As New List(Of Audio.Sound)
                For n = 0 To ExtendTimes - 1
                    ExtendList.Add(MaskerRegionSound.CreateCopy)
                Next
                Dim CrossSoundDuration As Double = 0.01
                MaskerRegionSound = Audio.DSP.ConcatenateSounds(ExtendList,,,,,, CrossSoundDuration * MaskerRegionSound.WaveFormat.SampleRate, False, 10, True)

                'Fading very slightly to avoid initial and final impulses
                Audio.DSP.Fade(MaskerRegionSound, Nothing, 0,,, MaskerRegionSound.WaveFormat.SampleRate * 0.01, Audio.DSP.FadeSlopeType.Linear)
                Audio.DSP.Fade(MaskerRegionSound, 0, Nothing,, MaskerRegionSound.WaveData.SampleData(1).Length - MaskerRegionSound.WaveFormat.SampleRate * 0.01,, Audio.DSP.FadeSlopeType.Linear)

                'Removing CD-component
                Audio.DSP.RemoveDcComponent(MaskerRegionSound)

                'Setting the RMS level to the RMS level of the concatenated sound
                Audio.DSP.MeasureAndAdjustSectionLevel(MaskerRegionSound, ConcatSoundLevel, 1,,, Audio.FrequencyWeightings.Z)

                'Creating a copy which will only be used to calculate a low resolution spectrum
                Dim MaskerRegionSound_LowResCopy = MaskerRegionSound.CreateCopy

                'Calculating spectrum - high resolution
                MaskerRegionSound.FFT = Audio.DSP.SpectralAnalysis(MaskerRegionSound, HighRes_OutputSpectra_FftFormat, 1,,)
                MaskerRegionSound.FFT.CalculateAmplitudeSpectrum()
                Dim FilteredMaskerSpectrum_HighRes = MaskerRegionSound.FFT.GetAverageSpectrum(1, Audio.FftData.SpectrumTypes.AmplitudeSpectrum, MaskerRegionSound.WaveFormat, True)
                If MaskerSoundIndex < OutputSoundCount Then
                    'Ordinary output sounds
                    HighRes_SoundSpectra.Add(New Tuple(Of String, SortedList(Of Double, Double))(SummaryComponentString & vbTab & ExportFileName, FilteredMaskerSpectrum_HighRes))
                Else
                    'Extra sounds
                    HighRes_ExtraSoundSpectra.Add(New Tuple(Of String, SortedList(Of Double, Double))(SummaryComponentString & vbTab & ExportFileName, FilteredMaskerSpectrum_HighRes))
                End If

                'Calculating spectrum - Low resolution
                MaskerRegionSound_LowResCopy.FFT = Audio.DSP.SpectralAnalysis(MaskerRegionSound_LowResCopy, LowRes_OutputSpectra_FftFormat, 1,,)
                MaskerRegionSound_LowResCopy.FFT.CalculateAmplitudeSpectrum()
                Dim FilteredMaskerSpectrum_LowRes = MaskerRegionSound_LowResCopy.FFT.GetAverageSpectrum(1, Audio.FftData.SpectrumTypes.AmplitudeSpectrum, MaskerRegionSound_LowResCopy.WaveFormat, True)
                If MaskerSoundIndex < OutputSoundCount Then
                    'Ordinary output sounds
                    LowRes_SoundSpectra.Add(New Tuple(Of String, SortedList(Of Double, Double))(SummaryComponentString & vbTab & ExportFileName, FilteredMaskerSpectrum_LowRes))
                Else
                    'Extra sounds
                    LowRes_ExtraSoundSpectra.Add(New Tuple(Of String, SortedList(Of Double, Double))(SummaryComponentString & vbTab & ExportFileName, FilteredMaskerSpectrum_LowRes))
                End If


            Next
            Progress += 1
            myProgressDisplay.UpdateProgress(Progress)
        Next

        'Closing the progress display
        myProgressDisplay.Close()


        'Starting a progress window
        myProgressDisplay = New ProgressDisplay
        myProgressDisplay.Initialize(1, 0, "Analysing and exporting masker sound spectra, please wait...")
        myProgressDisplay.Show()
        Progress = 0
        myProgressDisplay.UpdateProgress(Progress)

        'Logging the spectra
        Dim HighRes_SpectrumOutputList As New List(Of String)
        Dim HighRes_ExtraSpectrumOutputList As New List(Of String)
        Dim LowRes_SpectrumOutputList As New List(Of String)
        Dim LowRes_ExtraSpectrumOutputList As New List(Of String)

        Dim HighRes_HeadingList As New List(Of String) From {"SummaryComponent", "File"}
        For Each Frequency In HighRes_SoundSpectra(0).Item2.Keys
            HighRes_HeadingList.Add(Math.Round(Frequency))
        Next

        Dim LowRes_HeadingList As New List(Of String) From {"SummaryComponent", "File"}
        For Each Frequency In LowRes_SoundSpectra(0).Item2.Keys
            LowRes_HeadingList.Add(Math.Round(Frequency))
        Next

        HighRes_SpectrumOutputList.Add(String.Join(vbTab, HighRes_HeadingList))
        HighRes_ExtraSpectrumOutputList.Add(String.Join(vbTab, HighRes_HeadingList))
        LowRes_SpectrumOutputList.Add(String.Join(vbTab, LowRes_HeadingList))
        LowRes_ExtraSpectrumOutputList.Add(String.Join(vbTab, LowRes_HeadingList))

        For n = 0 To HighRes_SoundSpectra.Count - 1
            Dim DataList As New List(Of String)
            DataList.Add(HighRes_SoundSpectra(n).Item1)
            For Each Amplitude In HighRes_SoundSpectra(n).Item2.Values
                DataList.Add(Math.Round(Amplitude, 3))
            Next
            HighRes_SpectrumOutputList.Add(String.Join(vbTab, DataList))
        Next

        For n = 0 To HighRes_ExtraSoundSpectra.Count - 1
            Dim DataList As New List(Of String)
            DataList.Add(HighRes_ExtraSoundSpectra(n).Item1)
            For Each Amplitude In HighRes_ExtraSoundSpectra(n).Item2.Values
                DataList.Add(Math.Round(Amplitude, 3))
            Next
            HighRes_ExtraSpectrumOutputList.Add(String.Join(vbTab, DataList))
        Next

        For n = 0 To LowRes_SoundSpectra.Count - 1
            Dim DataList As New List(Of String)
            DataList.Add(LowRes_SoundSpectra(n).Item1)
            For Each Amplitude In LowRes_SoundSpectra(n).Item2.Values
                DataList.Add(Math.Round(Amplitude, 3))
            Next
            LowRes_SpectrumOutputList.Add(String.Join(vbTab, DataList))
        Next

        For n = 0 To LowRes_ExtraSoundSpectra.Count - 1
            Dim DataList As New List(Of String)
            DataList.Add(LowRes_ExtraSoundSpectra(n).Item1)
            For Each Amplitude In LowRes_ExtraSoundSpectra(n).Item2.Values
                DataList.Add(Math.Round(Amplitude, 3))
            Next
            LowRes_ExtraSpectrumOutputList.Add(String.Join(vbTab, DataList))
        Next


        Utils.SendInfoToLog("Spectra of masker sounds:" & vbCrLf & "FFT-length:" & vbTab & HighRes_OutputSpectra_FftFormat.FftWindowSize & vbCrLf &
                      String.Join(vbCrLf, HighRes_SpectrumOutputList), IO.Path.Combine(ExportFolder, "Log", "SoundSpectra_HighResolution"))

        Utils.SendInfoToLog("Spectra of extra masker sounds:" & vbCrLf & "FFT-length:" & vbTab & HighRes_OutputSpectra_FftFormat.FftWindowSize & vbCrLf &
                      String.Join(vbCrLf, HighRes_ExtraSpectrumOutputList), IO.Path.Combine(ExportFolder, "Log", "ExtraSoundSpectra_HighResolution"))

        Utils.SendInfoToLog("Spectra of masker sounds:" & vbCrLf & "FFT-length:" & vbTab & LowRes_OutputSpectra_FftFormat.FftWindowSize & vbCrLf &
                      String.Join(vbCrLf, LowRes_SpectrumOutputList), IO.Path.Combine(ExportFolder, "Log", "SoundSpectra_LowResolution"))

        Utils.SendInfoToLog("Spectra of extra masker sounds:" & vbCrLf & "FFT-length:" & vbTab & LowRes_OutputSpectra_FftFormat.FftWindowSize & vbCrLf &
                      String.Join(vbCrLf, LowRes_ExtraSpectrumOutputList), IO.Path.Combine(ExportFolder, "Log", "ExtraSoundSpectra_LowResolution"))


        'Creating concatenated sounds and saving to log folder
        'Loading all exported sounds
        Dim ConcatInputList As New List(Of Audio.Sound)
        For Each FilePath In SoundExportList
            ConcatInputList.Add(Audio.AudioIOs.ReadWaveFile(FilePath))
        Next
        Dim ConcatSound = Audio.DSP.ConcatenateSounds(ConcatInputList)
        Audio.AudioIOs.SaveToWaveFile(ConcatSound, IO.Path.Combine(ExportFolder, "Log", "AllMaskersConcat"))

        'Sending the SoundExportList to log
        Utils.SendInfoToLog(vbCrLf & String.Join(vbCrLf, SoundExportList), IO.Path.Combine(ExportFolder, "Log", "MaskerExportList"),,, True)
        Utils.SendInfoToLog(vbCrLf & String.Join(vbCrLf, ExtraSoundExportList), IO.Path.Combine(ExportFolder, "Log", "ExtraMaskerExportList"),,, True)

        'Closing the progress display
        myProgressDisplay.Close()

        MsgBox("Finished creating masker sounds!")

    End Sub



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
    '    Dim AudioFileLoadMode_StartValue = SpeechMaterialComponent.AudioFileLoadMode
    '    SpeechMaterialComponent.AudioFileLoadMode = SpeechMaterialComponent.MediaFileLoadModes.LoadOnFirstUse

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
    '    SpeechMaterialComponent.AudioFileLoadMode = AudioFileLoadMode_StartValue

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

                Dim CurrentSmaComponentList = AllComponentsWithSound(c).GetCorrespondingSmaComponent(Me, i, SoundChannel)

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



    Public Sub SetSpeechLevels(ByVal TargetLevel As Double, ByVal FrequencyWeighting As Audio.FrequencyWeightings, ByVal TemporalIntegration As Double?)


    End Sub



End Class

