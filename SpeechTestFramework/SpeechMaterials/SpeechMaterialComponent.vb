Public Class SpeechMaterial
    Inherits SpeechMaterialComponent

    Public Property TestLists As List(Of SpeechMaterialList)
        Get
            Return Me.ChildComponents
        End Get
        Set(value As List(Of SpeechMaterialComponent))
            Me.ChildComponents = value
        End Set
    End Property

    Public Sub New(ByRef rnd As Random)
        MyBase.New(rnd)
    End Sub

End Class

Public Class SpeechMaterialList
    Inherits SpeechMaterialComponent
    Public Property TestSentences As List(Of SpeechMaterialComponent)
        Get
            Return Me.ChildComponents
        End Get
        Set(value As List(Of SpeechMaterialComponent))
            Me.ChildComponents = value
        End Set
    End Property

    Public Sub New(ByRef rnd As Random)
        MyBase.New(rnd)
    End Sub

End Class

Public Class SpeechMaterialSentence
    Inherits SpeechMaterialComponent

    Public Sub New(ByRef rnd As Random)
        MyBase.New(rnd)
    End Sub

End Class

Public Class SpeechMaterialWord
    Inherits SpeechMaterialComponent

    Public Sub New(ByRef rnd As Random)
        MyBase.New(rnd)
    End Sub

End Class

Public Class SpeechMaterialPhoneme
    Inherits SpeechMaterialComponent

    Public Sub New(ByRef rnd As Random)
        MyBase.New(rnd)
    End Sub

End Class

Public Class SpeechMaterialComponent

    Public Property Id As String

    Public Property ParentComponent As SpeechMaterialComponent

    Public Property PrimaryStringRepresentation As String

    Public Property ChildComponents As New List(Of SpeechMaterialComponent)

    'These two should contain the data defines in the LinguisticDatabase associated to the component in the speech material file.
    Private NumericVariables As New SortedList(Of String, Double)
    Private CategoricalVariables As New SortedList(Of String, String)

    Public Property OrderedChildren As Boolean = False

    Public Property TrialCapacity As TrialCapacities

    Public Enum TrialCapacities
        SuperTrial
        Trial
        SubTrial
    End Enum

    Public Property DefinesLevel As DefinesLevelOptions = DefinesLevelOptions.False

    Public Enum DefinesLevelOptions
        [False]
        Self
        SamePlaceCousins
        Children
        Relatives
    End Enum

    Public Property LevelIntegrationTime As Integer? = Nothing

    Public Property DefinesMaskerSpectrum As DefinesLevelOptions = DefinesLevelOptions.False ' This might need another, additional, enumerator!

    Public Property DefinesReferenceLevel As DefinesLevelOptions = DefinesLevelOptions.False ' This might need another, additional, enumerator!

    Public Property LimitsLevel As Boolean = False

    Private MediaFolder As String
    Private MaskerFolder As String
    Private BackgroundNonspeechFolder As String
    Private BackgroundSpeechFolder As String

    Public Property DistractorItems As DistractorItemTypes = DistractorItemTypes.None

    Public Enum DistractorItemTypes
        None
        Siblings
        SamePlaceCousins
        Relatives
    End Enum

    Private Randomizer As Random

    'Shared stuff used to keep media items in memory instead of re-loading on every use
    Public Shared AudioFileLoadMode As MediaFileLoadModes = MediaFileLoadModes.LoadOnFirstUse
    Public Shared ImageFileLoadMode As MediaFileLoadModes = MediaFileLoadModes.LoadOnFirstUse
    Public Shared SoundLibrary As New SortedList(Of String, Audio.Sound)
    Public Shared ImageLibrary As New SortedList(Of String, Drawing.Bitmap)
    Public Enum MediaFileLoadModes
        LoadEveryTime
        LoadOnFirstUse
    End Enum

    Public Sub New(ByRef rnd As Random)
        Me.Randomizer = rnd
    End Sub

    Public Function GetRandomNumber() As Double
        Return Randomizer.NextDouble()
    End Function

    Public Enum MediaTypes
        Audio
        Image
    End Enum

    Public Function GetMaskerPaths(ByVal RootPath As String, ByRef MediaSet As MediaSet, ByVal MediaType As MediaTypes) As String()

        Dim MaskerFolder As String = IO.Path.Combine(RootPath, MediaSet.MediaParentFolder, Me.MediaFolder)

        Return GetAvailableFiles(MaskerFolder, MediaType)

    End Function

    Private Function GetAvailableFiles(ByVal Folder As String, ByVal MediaType As MediaTypes) As String()

        'Getting files in that folder
        Dim AvailableFiles = IO.Directory.GetFiles(Folder)

        Dim IncludedFiles As New List(Of String)

        Dim AllowedFileExtensions As New List(Of String)

        Select Case MediaType
            Case MediaTypes.Audio
                AllowedFileExtensions.Add(".wav")

            Case MediaTypes.Image
                AllowedFileExtensions.Add(".png")

            Case Else
                Throw New Exception("Unknown value for MediaType")
        End Select

        For Each file In AvailableFiles
            For Each ext In AllowedFileExtensions
                If file.EndsWith(ext) Then
                    IncludedFiles.Add(file)
                    'Exits the inner loop, as the files is now added
                    Exit For
                End If
            Next
        Next

        Return AvailableFiles

    End Function

    Public Function GetSound(ByVal Path) As Audio.Sound

        Select Case AudioFileLoadMode
            Case MediaFileLoadModes.LoadEveryTime
                Return Audio.Sound.LoadWaveFile(Path)

            Case MediaFileLoadModes.LoadOnFirstUse

                If SoundLibrary.ContainsKey(Path) Then
                    Return SoundLibrary(Path)
                Else
                    Dim NewSound As Audio.Sound = Audio.Sound.LoadWaveFile(Path)
                    SoundLibrary.Add(Path, NewSound)
                    Return SoundLibrary(Path)
                End If

            Case Else
                Throw New NotImplementedException
        End Select

    End Function

    Public Function GetImage(ByVal Path) As Drawing.Bitmap

        Select Case ImageFileLoadMode
            Case MediaFileLoadModes.LoadEveryTime

                Return Drawing.Bitmap.FromFile(Path)

            Case MediaFileLoadModes.LoadOnFirstUse

                If ImageLibrary.ContainsKey(Path) Then
                    Return ImageLibrary(Path)
                Else
                    Dim NewImage As Drawing.Bitmap = Drawing.Bitmap.FromFile(Path)
                    ImageLibrary.Add(Path, NewImage)
                    Return ImageLibrary(Path)
                End If

            Case Else
                Throw New NotImplementedException
        End Select

    End Function


    ''' <summary>
    ''' Searches first among the variable types and then among the categorical for the indicated VariableName. If found, returns the value. 
    ''' The calling codes need to parse the value as it is returned as an object. If the variable type is known, it is better to use either GetNumericWordMetricValue or GetCategoricalWordMetricValue instead.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <returns></returns>
    Public Function GetVariableValue(ByVal VariableName As String) As Object

        'Looks first among the numeric metrics
        If NumericVariables.Keys.Contains(VariableName) Then
            Return NumericVariables(VariableName)
        End If

        'If not found, looks among the categorical metrics
        If CategoricalVariables.Keys.Contains(VariableName) Then
            Return CategoricalVariables(VariableName)
        End If

        Return Nothing

    End Function

    ''' <summary>
    ''' Searches among the numeric variable types for the indicated VariableName. If found returns word metric value, otherwise returns Nothing.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <returns></returns>
    Public Function GetNumericVariableValue(ByVal VariableName As String) As Double?

        If NumericVariables.Keys.Contains(VariableName) Then
            Return NumericVariables(VariableName)
        End If

        Return Nothing

    End Function

    ''' <summary>
    ''' Searches among the categorical variable types for the indicated VariableName. If found returns the value stored as a string, otherwise an empty string is returned.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <returns></returns>
    Public Function GetCategoricalWordMetricValue(ByVal VariableName As String) As String

        If NumericVariables.Keys.Contains(VariableName) Then
            Return NumericVariables(VariableName)
        End If

        Return ""

    End Function


    Public Function GetChildren() As List(Of SpeechMaterialComponent)
        Return ChildComponents
    End Function

    Public Function GetParent() As List(Of SpeechMaterialComponent)
        If ParentComponent IsNot Nothing Then
            Dim OutputList As New List(Of SpeechMaterialComponent)
            OutputList.Add(ParentComponent)
            Return OutputList
        Else
            Return Nothing
        End If
    End Function

    Public Function GetSiblings() As List(Of SpeechMaterialComponent)
        If ParentComponent IsNot Nothing Then
            Return ParentComponent.GetChildren
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Figures out and returns at what index in the parent component the component itself is stored, or Nothing if there is no parent compoment, or if (for some unexpected reason) unable to establish the index.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSelfIndex() As Integer?

        If ParentComponent Is Nothing Then Return Nothing

        Dim Siblings = GetSiblings()
        For s = 0 To Siblings.Count - 1
            If Siblings(s) Is Me Then Return s
        Next

        Return Nothing

    End Function


    Public Function SamePlaceCousins() As List(Of SpeechMaterialComponent)

        Dim SelfIndex = GetSelfIndex()

        Dim MySiblingCount As Integer = GetSiblings.Count

        If SelfIndex Is Nothing Then Return Nothing

        If ParentComponent Is Nothing Then Return Nothing

        'Checks that the parent has ordered children
        If ParentComponent.OrderedChildren = False Then Throw New Exception("Cannot return same-place cousins from unordered components. (Component id: " & ParentComponent.Id & ")")

        Dim Aunties = ParentComponent.GetSiblingsExcludingSelf

        If Aunties Is Nothing Then Return Nothing

        Dim OutputList As New List(Of SpeechMaterialComponent)

        For Each auntie In Aunties

            'Checks that the aunties have ordered children
            If auntie.OrderedChildren = False Then Throw New Exception("Cannot return same-place cousins from unordered components. (Component id: " & auntie.Id & ")")

            'Checks that the number of sibling copmonents are the same
            Dim AuntiesChildCount As Integer = auntie.ChildComponents.Count
            If MySiblingCount <> AuntiesChildCount Then Throw New Exception("Cannot return same-place cousins from cousin groups that differ in count. (Component ids: " & ParentComponent.Id & " vs. " & auntie.Id & ")")

            'Finally adding the same place cousin
            OutputList.Add(auntie.ChildComponents(SelfIndex))

        Next

        Return OutputList

    End Function

    Public Function GetSiblingsExcludingSelf() As List(Of SpeechMaterialComponent)
        Dim OutputList As New List(Of SpeechMaterialComponent)
        If ParentComponent IsNot Nothing Then
            For Each child In ParentComponent.ChildComponents
                If child IsNot Me Then
                    OutputList.Add(child)
                End If
            Next
        End If
        Return OutputList
    End Function

    ''' <summary>
    ''' Recursively searches for the SpeechMaterialComponent at the top of the heirachy
    ''' </summary>
    ''' <returns></returns>
    Public Function GetToplevelAncestor() As SpeechMaterialComponent
        If ParentComponent IsNot Nothing Then
            Return ParentComponent.GetToplevelAncestor
        Else
            Return Me
        End If
    End Function

    Public Function GetAllRelatives() As List(Of SpeechMaterialComponent)
        'Creates a list
        Dim OutputList As New List(Of SpeechMaterialComponent)

        'Gets the top level ancestor
        Dim TopLevelAncestor = GetToplevelAncestor()

        'Adds the top level ancestor
        OutputList.Add(TopLevelAncestor)

        'Adds all descendants to the top level ancestor
        TopLevelAncestor.AddDescendents(OutputList)

        Return OutputList
    End Function

    Public Function GetAllTrialLevelComponents() As List(Of SpeechMaterialComponent)

        Dim AllRelatives = GetAllRelatives()

        If AllRelatives Is Nothing Then Return Nothing

        Dim OutputList As New List(Of SpeechMaterialComponent)
        For Each Component In AllRelatives
            If Component.TrialCapacity = TrialCapacities.Trial Then
                OutputList.Add(Component)
            End If
        Next

        Return OutputList

    End Function

    ''' <summary>
    ''' Recursively adds all descentents to the DescendentsList
    ''' </summary>
    ''' <param name="DescendentsList"></param>
    Private Sub AddDescendents(ByRef DescendentsList As List(Of SpeechMaterialComponent))

        For Each child In ChildComponents
            DescendentsList.Add(child)
            child.AddDescendents(DescendentsList)
        Next

    End Sub

    Public Function GetAllRelativesExludingSelf() As List(Of SpeechMaterialComponent)

        Dim RelativesList = GetAllRelatives()
        Dim OutputList As New List(Of SpeechMaterialComponent)
        For Each item In RelativesList
            If item IsNot Me Then
                OutputList.Add(item)
            End If
        Next
        Return OutputList

    End Function

    Public Shared Function LoadSpeechMaterial(ByVal FilePath As String, ByVal RootPath As String) As SpeechMaterialComponent


        'Gets a file path from the user if none is supplied
        'If FilePath = "" Then FilePath = Utils.GetOpenFilePath(,, {".txt"}, "Please open a stuctured speech material .txt file.")
        'If FilePath = "" Then
        '    MsgBox("No file selected!")
        '    Return Nothing
        'End If

        'Creates a new random that will be references in all speech material components
        Dim rnd As New Random

        Dim Output As SpeechMaterialComponent = Nothing

        'Parses the input file
        Dim InputLines() As String = System.IO.File.ReadAllLines(InputFileSupport.InputFilePathValueParsing(FilePath, RootPath, False), Text.Encoding.UTF8)

        Dim CustomVariablesDatabases As New SortedList(Of String, CustomVariablesDatabase)

        Dim IdsUsed As New SortedSet(Of String)

        For Each Line In InputLines

            'Skipping blank lines
            If Line.Trim = "" Then Continue For

            'Also skipping commentary only lines 
            If Line.Trim.StartsWith("//") Then Continue For

            Dim SplitRow = Line.Split(vbTab)

            If SplitRow.Length < 17 Then Throw New ArgumentException("Not enough data columns in the file " & FilePath & vbCrLf & "At the line: " & Line)

            Dim NewComponent As New SpeechMaterialComponent(rnd)

            'Adds component data
            Dim index As Integer = 0
            NewComponent.Id = InputFileSupport.GetInputFileValue(SplitRow(index), False)
            index += 1

            'Checking that the Id is not already used (Ids can only be used once throughout all speech component levels!!!)
            If IdsUsed.Contains(NewComponent.Id) Then
                Throw New ArgumentException("Re-used Id (" & NewComponent.Id & ")! Speech material components must only be used once throughout the whole speech material!")
            End If

            ' Reading ParentId (which is used below
            Dim ParentId As String = InputFileSupport.GetInputFileValue(SplitRow(index), False)
            index += 1

            ' PrimaryStringRepresentation
            NewComponent.PrimaryStringRepresentation = InputFileSupport.GetInputFileValue(SplitRow(index), False)
            index += 1

            ' Add custom variables
            Dim CustomVariablesDatabase As String = IO.Path.Combine(RootPath, "CustomVariables", InputFileSupport.InputFilePathValueParsing(SplitRow(index), RootPath, False))
            index += 1
            Dim DbId As String = InputFileSupport.GetInputFileValue(SplitRow(index), False)
            index += 1

            If CustomVariablesDatabases.ContainsKey(CustomVariablesDatabase) = False Then
                'Loading the database
                Dim NewDatabase As New CustomVariablesDatabase
                NewDatabase.LoadTabDelimitedFile(CustomVariablesDatabase)
                CustomVariablesDatabases.Add(CustomVariablesDatabase, NewDatabase)
            End If

            'Adding the variables
            For n = 0 To CustomVariablesDatabases(CustomVariablesDatabase).CustomVariableNames.Count - 1
                Dim VariableName = CustomVariablesDatabases(CustomVariablesDatabase).CustomVariableNames(n)
                If CustomVariablesDatabases(CustomVariablesDatabase).CustomVariableTypes(n) = VariableTypes.Categorical Then
                    NewComponent.CategoricalVariables.Add(VariableName, CustomVariablesDatabases(CustomVariablesDatabase).GetVariableValue(DbId, VariableName))
                ElseIf CustomVariablesDatabases(CustomVariablesDatabase).CustomVariableTypes(n) = VariableTypes.Numeric Then
                    NewComponent.NumericVariables.Add(VariableName, CustomVariablesDatabases(CustomVariablesDatabase).GetVariableValue(DbId, VariableName))
                Else
                    Throw New NotImplementedException("Variable type not implemented!")
                End If
            Next

            'Adds further component data
            Dim OrderedChildren = InputFileSupport.InputFileBooleanValueParsing(SplitRow(index), False, FilePath)
            If OrderedChildren IsNot Nothing Then NewComponent.OrderedChildren = OrderedChildren
            index += 1

            Dim TrialCapacity = InputFileSupport.InputFileEnumValueParsing(SplitRow(index), GetType(TrialCapacities), FilePath, False)
            If TrialCapacity IsNot Nothing Then
                NewComponent.TrialCapacity = TrialCapacity
            Else
                Throw New Exception("Missing value for TrialCapacity detected in the speech material file. A value for TrialCapacity is obligatory for all speech material components. Line: " & vbCrLf & Line)
            End If
            index += 1

            Dim DefinesLevel = InputFileSupport.InputFileEnumValueParsing(SplitRow(index), GetType(DefinesLevelOptions), FilePath, False)
            If DefinesLevel IsNot Nothing Then NewComponent.DefinesLevel = DefinesLevel
            index += 1

            Dim LevelIntegrationTime = InputFileSupport.InputFileIntegerValueParsing(SplitRow(index), False, FilePath)
            If LevelIntegrationTime IsNot Nothing Then NewComponent.LevelIntegrationTime = LevelIntegrationTime
            index += 1

            Dim DefinesMaskerSpectrum = InputFileSupport.InputFileEnumValueParsing(SplitRow(index), GetType(DefinesLevelOptions), FilePath, False)
            If DefinesMaskerSpectrum IsNot Nothing Then NewComponent.DefinesMaskerSpectrum = DefinesMaskerSpectrum
            index += 1

            Dim DefinesReferenceLevel = InputFileSupport.InputFileEnumValueParsing(SplitRow(index), GetType(DefinesLevelOptions), FilePath, False)
            If DefinesReferenceLevel IsNot Nothing Then NewComponent.DefinesReferenceLevel = DefinesReferenceLevel
            index += 1

            Dim LimitsLevel = InputFileSupport.InputFileBooleanValueParsing(SplitRow(index), False, FilePath)
            If LimitsLevel IsNot Nothing Then NewComponent.LimitsLevel = LimitsLevel
            index += 1

            NewComponent.MediaFolder = InputFileSupport.InputFilePathValueParsing(SplitRow(index), RootPath, False)
            index += 1

            NewComponent.MaskerFolder = InputFileSupport.InputFilePathValueParsing(SplitRow(index), RootPath, False)
            index += 1

            NewComponent.BackgroundNonspeechFolder = InputFileSupport.InputFilePathValueParsing(SplitRow(index), RootPath, False)
            index += 1

            NewComponent.BackgroundSpeechFolder = InputFileSupport.InputFilePathValueParsing(SplitRow(index), RootPath, False)
            index += 1

            Dim DistractorItems = InputFileSupport.InputFileEnumValueParsing(SplitRow(index), GetType(DistractorItemTypes), FilePath, False)
            If DistractorItems IsNot Nothing Then NewComponent.DistractorItems = DistractorItems

            'Adds the component
            If Output Is Nothing Then
                Output = NewComponent
            Else
                If Output.AddComponent(NewComponent, ParentId) = False Then
                    Throw New ArgumentException("Failed to add speech material component defined by the following line in the file : " & FilePath & vbCrLf & Line)
                End If
            End If

        Next

        ''Writing the loaded data to UpdatedOutputFilePath if supplied and valid
        'If UpdatedOutputFilePath <> "" Then
        '    Output.WriteSpeechMaterialFile(UpdatedOutputFilePath)
        'End If

        Return Output

    End Function

    Public Function GetComponentById(ByVal Id As String) As SpeechMaterialComponent

        Dim AllComponents = GetAllRelatives()
        For Each Component In AllComponents
            If Component.Id = Id Then Return Component
        Next

        'Returns nothing if the component was not found
        Return Nothing

    End Function

    Public Function AddComponent(ByRef NewComponent As SpeechMaterialComponent, ByVal ParentId As String) As Boolean

        Dim ParentComponent = GetComponentById(ParentId)
        If ParentComponent IsNot Nothing Then
            'Assigning the parent
            NewComponent.ParentComponent = ParentComponent
            'Storing the child
            ParentComponent.ChildComponents.Add(NewComponent)
            Return True
        Else
            Return False
        End If

    End Function

End Class


''' <summary>
''' A class for looking up custom variables for speech material components.
''' </summary>
Public Class CustomVariablesDatabase

    Private CustomVariablesData As New Dictionary(Of String, SortedList(Of String, Object))

    Public CustomVariableNames As New List(Of String)
    Public CustomVariableTypes As New List(Of VariableTypes)

    Public FilePath As String = ""


    Public Function LoadTabDelimitedFile(ByVal FilePath As String) As Boolean

        Try


            ''Gets a file path from the user if none is supplied
            'If FilePath = "" Then FilePath = Utils.GetOpenFilePath(,, {".txt"}, "Please open a tab delimited word metrics .txt file.")
            'If FilePath = "" Then
            '    MsgBox("No file selected!")
            '    Return Nothing
            'End If

            CustomVariablesData.Clear()
            CustomVariableNames.Clear()
            CustomVariableTypes.Clear()

            'Parses the input file
            Dim InputLines() As String = System.IO.File.ReadAllLines(FilePath, Text.Encoding.UTF8)

            'Stores the file path used for loading the word metric data
            Me.FilePath = FilePath

            'First line should be variable names
            Dim FirstLineData() As String = InputLines(0).Split(vbTab)
            For c = 0 To FirstLineData.Length - 1
                If FirstLineData(c).Trim = "" Then
                    'Assuming that there is no data, if the first line is empty!
                    Return False
                End If

                CustomVariableNames.Add(FirstLineData(c).Trim)
            Next

            'Second line should be variable types (N for Numeric or C for Categorical)
            Dim SecondLineData() As String = InputLines(1).Split(vbTab)
            For c = 0 To SecondLineData.Length - 1
                If SecondLineData(c).Trim.ToLower = "n" Then
                    CustomVariableTypes.Add(VariableTypes.Numeric)
                ElseIf SecondLineData(c).Trim.ToLower = "c" Then
                    CustomVariableTypes.Add(VariableTypes.Categorical)
                Else
                    Throw New Exception("The type for the custom variable " & CustomVariableNames(c) & " in the file " & FilePath & " must be either N for numeric or C for categorical.")
                End If
            Next

            'Reading data
            For i = 2 To InputLines.Length - 1

                Dim LineSplit() As String = InputLines(i).Split(vbTab)

                Dim UniqueIdentifier As String = LineSplit(0).Trim

                CustomVariablesData.Add(UniqueIdentifier, New SortedList(Of String, Object))

                For c = 0 To LineSplit.Length - 1

                    Dim ValueString As String = LineSplit(c).Trim
                    If CustomVariableTypes(c) = VariableTypes.Numeric Then

                        'Adding the data as a Double
                        Dim NumericValue As Double
                        If Double.TryParse(ValueString.Replace(",", "."), NumericValue) Then
                            'Adds the variable and its data only if a value has been parsed
                            CustomVariablesData(UniqueIdentifier).Add(CustomVariableNames(c), NumericValue)
                        Else
                            'Throws an error if parsing failed even though the string was not empty
                            If ValueString.Trim <> "" Then
                                Throw New Exception("Unable to parse the string " & NumericValue & " given for the variable " & CustomVariableNames(c) & " in the file: " & FilePath & " as a numeric value.")
                            End If
                        End If

                    Else
                        'Adding the data as a String
                        CustomVariablesData(UniqueIdentifier).Add(CustomVariableNames(c), ValueString)
                    End If

                Next

            Next

            Return True

        Catch ex As Exception
            'TODO What here?
            Return False
        End Try


    End Function

    Public Function GetVariableValue(ByVal UniqueIdentifier As String, ByVal VariableName As String) As Object

        If CustomVariablesData.ContainsKey(UniqueIdentifier) Then
            If CustomVariablesData(UniqueIdentifier).ContainsKey(VariableName) Then
                Return CustomVariablesData(UniqueIdentifier)(VariableName)
            End If
        End If

        'Returns Nothing is the UniqueIdentifier or VariableName was not found
        Return Nothing

    End Function



End Class

Public Class InputFileSupport


#Region "InputFileSupport"

    Public Shared Function GetInputFileValue(ByVal InputData As String, ByVal ContainsVariableName As Boolean) As String

        'Trimming off any comments
        Dim InputLineSplit() As String = InputData.Split({"//"}, StringSplitOptions.None) 'A comment can be added after // in the input file
        Dim DataPrecedingComments As String = InputLineSplit(0).Trim

        If ContainsVariableName = True Then
            Dim VariebleDataSplit() As String = InputLineSplit(0).Split("=")
            If VariebleDataSplit.Length > 1 Then
                Return VariebleDataSplit(1).Trim
            Else
                Return ""
            End If
        Else
            Return DataPrecedingComments
        End If

    End Function

    Public Shared Function InputFileDoubleValueParsing(ByVal InputData As String, ByVal ContainsVariableName As Boolean, ByVal SourceTextFile As String) As Double?

        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        Dim OutputValue As Double? = Nothing

        Dim ValueString As String = TrimmedData.Replace(",", ".")

        If ValueString = "" Then Return OutputValue

        If IsNumeric(ValueString) = True Then
            OutputValue = ValueString
        Else
            Throw New Exception("Non-numeric data ( " & InputData & ") found where numeric data was expected in the file: " & SourceTextFile)
        End If

        Return OutputValue

    End Function

    Public Shared Function InputFileIntegerValueParsing(ByVal InputData As String, ByVal ContainsVariableName As Boolean, ByVal SourceTextFile As String) As Integer?

        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        Dim OutputValue As Integer? = Nothing

        Dim ValueString As String = TrimmedData.Replace(",", ".")
        If IsNumeric(ValueString) = True Then

            Dim DoubleValue As Double = Double.Parse(ValueString)

            If Math.Round(DoubleValue) = DoubleValue Then
                OutputValue = ValueString
            Else
                Throw New Exception("Non-integer data ( " & InputData & ") found where integer data was expected in the file: " & SourceTextFile)
            End If
        End If

        Return OutputValue

    End Function


    Public Shared Function InputFilePathValueParsing(ByVal InputData As String, ByVal RootPath As String, ByVal ContainsVariableName As Boolean) As String

        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        If TrimmedData = "" Then
            Return ""
        Else
            If TrimmedData.StartsWith(".\") Then
                Return IO.Path.Combine(RootPath, TrimmedData)
            Else
                Return TrimmedData
            End If
        End If

    End Function

    ''' <summary>
    ''' Parses the InputLine as a the indicated EnumType. Returns the Integer equivalent of the Enum value or Nothing if no value was given.
    ''' </summary>
    ''' <param name="InputData"></param>
    ''' <param name="EnumType"></param>
    ''' <returns></returns>
    Public Shared Function InputFileEnumValueParsing(ByVal InputData As String, ByVal EnumType As Type, ByVal SourceTextFile As String, ByVal ContainsVariableName As Boolean) As Integer?

        'Checks if the type given in EnumType is an enum
        If EnumType.IsEnum = False Then
            Throw New ArgumentException("The EnumType argument (" & EnumType.Name & ") supplied to InputFileEnumValueParsing is not an Enum.")
        End If

        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        If TrimmedData <> "" Then
            Try
                Return DirectCast([Enum].Parse(EnumType, TrimmedData), Integer)
            Catch ex As Exception
                Throw New Exception("Unable to parse the value " & InputData & " in the " & SourceTextFile & " file as a " & EnumType.Name)
            End Try
        Else
            Return Nothing
        End If

    End Function

    '''' <summary>
    '''' Parses the InputLine as a list of the indicated EnumType. Returns a list of the Integer equivalent of the Enum values or Nothing if no value was given.
    '''' </summary>
    '''' <param name="InputLine"></param>
    '''' <param name="EnumType"></param>
    '''' <returns></returns>
    'Public Shared Function InputFileEnumListValueParsing(ByVal InputLine As String, ByVal EnumType As Type, Optional SourceTextFile As String = "", Optional SourceVariableName As String = "") As List(Of Integer)

    '    Dim ListedStringValues = InputFileListOfStringParsing(InputLine)

    '    If ListedStringValues IsNot Nothing Then

    '        Dim EnumList As New List(Of Integer)

    '        For Each Value In ListedStringValues

    '            Dim NewEnumValue = InputFileEnumValueParsing(Value, EnumType, SourceTextFile, SourceVariableName)

    '            If NewEnumValue IsNot Nothing Then
    '                EnumList.Add(NewEnumValue)
    '            End If
    '        Next

    '        If EnumList.Count > 0 Then
    '            Return EnumList
    '        Else
    '            Return Nothing
    '        End If
    '    Else
    '        Return Nothing
    '    End If

    'End Function


    Public Shared Function InputFileSortedSetOfIntegerValueParsing(ByVal InputData As String, ByVal ContainsVariableName As Boolean, ByVal SourceTextFile As String) As SortedSet(Of Integer)

        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        If TrimmedData = "" Then Return Nothing

        Dim ValueSplit() As String = TrimmedData.Split(",")
        Dim ValueList As New SortedSet(Of Integer)
        For Each Value In ValueSplit

            Dim CastValue = InputFileIntegerValueParsing(Value, False, SourceTextFile)
            If CastValue IsNot Nothing = True Then ValueList.Add(CastValue.Value)

        Next

        If ValueList.Count > 0 Then
            Return ValueList
        Else
            Return Nothing
        End If

    End Function

    Public Shared Function InputFileListOfStringParsing(ByVal InputData As String, ByVal IncludeEmptyStrings As Boolean, ByVal ContainsVariableName As Boolean) As List(Of String)

        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        If TrimmedData = "" Then Return Nothing

        Dim ValueSplit() As String = TrimmedData.Split(",")
        Dim ValueList As New List(Of String)
        For Each Value In ValueSplit
            If IncludeEmptyStrings = True Then
                ValueList.Add(Value)
            Else
                If Value.Trim <> "" = True Then ValueList.Add(Value.Trim)
            End If
        Next

        If ValueList.Count > 0 Then
            Return ValueList
        Else
            Return Nothing
        End If

    End Function


    Public Shared Function InputFileSortedSetOfStringParsing(ByVal InputData As String, ByVal IncludeEmptyStrings As Boolean, ByVal ContainsVariableName As Boolean) As SortedSet(Of String)

        Dim ListedValues = InputFileListOfStringParsing(InputData, IncludeEmptyStrings, ContainsVariableName)
        If ListedValues IsNot Nothing Then
            Dim OutputValue As New SortedSet(Of String)
            For Each value In ListedValues
                OutputValue.Add(value)
            Next
            Return OutputValue
        Else
            Return Nothing
        End If

    End Function


    Public Shared Function InputFileBooleanValueParsing(ByVal InputData As String, ByVal ContainsVariableName As Boolean, ByVal SourceTextFile As String) As Boolean?


        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        If TrimmedData <> "" Then

            Dim OutputValue As Boolean

            If Boolean.TryParse(TrimmedData, OutputValue) = True Then
                Return OutputValue
            Else
                Throw New ArgumentException("Unable to parse the data in the string '" & InputData & "' in the file: " & SourceTextFile & "as a boolean value (True or False).")
            End If
        End If

        Return Nothing

    End Function

    '''' <summary>
    '''' Parses a member-word line in the speech-material type input file
    '''' </summary>
    '''' <param name="InputLine">The line as an unmodified string</param>
    '''' <param name="WordMetricNames">A list containing the names of all word metrics supplied</param>
    '''' <param name="WordMetricTypes">A list containing the types of all categorical word metrics supplied</param>
    '''' <param name="WML">A word metrics library that in which all input word are looked up. Is set to Nothing, no lookup is performed.</param>
    '''' <returns>Returns a tuple. Item1: Id, Item2: Sound files subfolder, Item3: Maskers sounds file subfolder, Item4: Spelling, Item5: Phonetic form, Item6: Numeric word metrics, Item7: Categorical word metrics</returns>
    'Public Shared Function InputFileTestWordValueParsing(ByVal InputLine As String,
    '                                                     ByVal WordMetricNames As List(Of String),
    '                                                     ByVal WordMetricTypes As List(Of VariableTypes),
    '                                                     ByRef WML As CustomVariablesDatabase) As Tuple(Of String, String, String, String, String, SortedList(Of String, Double), SortedList(Of String, String))


    '    Dim Id As String = ""
    '    Dim SoundFileSubFolder As String = ""
    '    Dim MaskerSoundFileSubFolder As String = ""
    '    Dim Spelling As String = ""
    '    Dim PhoneticTranscription As String = ""
    '    Dim NumericWordMetrics As New SortedList(Of String, Double)
    '    Dim CategoricalWordMetrics As New SortedList(Of String, String)

    '    Const PreWordMetricColumns As Integer = 5

    '    'Getting Id, spelling, transcription, and word metrics available
    '    'Trimming off comments
    '    Dim InputLineSplit() As String = InputLine.Split({"//"}, StringSplitOptions.None) 'A comment can be added after // in the input file
    '    Dim ValueString As String = InputLineSplit(0)
    '    'Gets the values
    '    Dim Values As List(Of String) = ValueString.Split(vbTab).ToList

    '    'Chaing that at least PreWordMetricColumns columns are supplied
    '    If Values.Count < PreWordMetricColumns Then
    '        Throw New Exception("Too few columns in the speech material file at the row: " & ValueString)
    '    End If


    '    'Reads the pre-word meric data
    '    For i = 0 To PreWordMetricColumns - 1

    '        Select Case i
    '            Case 0
    '                'Reads Id
    '                Id = Values(i).Trim

    '            Case 1
    '                'Reads the soundfile subfolder
    '                SoundFileSubFolder = Values(i).Trim

    '            Case 2
    '                'Reads the masker soundfile subfolder
    '                MaskerSoundFileSubFolder = Values(i).Trim

    '            Case 3
    '                'Reads Spelling
    '                Spelling = Values(i).Trim
    '                'Spelling = Spelling.Trim 'converts to lower ' This was done prior to 2022-03-30. Why?

    '            Case 4
    '                'Reads Phonetic transcription
    '                PhoneticTranscription = Values(i).Trim
    '                PhoneticTranscription = PhoneticTranscription.TrimStart("[").Trim
    '                PhoneticTranscription = PhoneticTranscription.TrimEnd("]").Trim

    '                'Supplies default values:
    '                'If the Id column contains only a hyphen "-", a default Id on the form Spelling [ PhoneticTranscription ] is created, and if no transcription exists only the spelling is used.
    '                If Id = "-" Then
    '                    If PhoneticTranscription.Trim = "" Then
    '                        Id = Spelling
    '                    Else
    '                        Id = Spelling & " [ " & PhoneticTranscription.Trim & " ]"
    '                    End If
    '                End If

    '        End Select

    '    Next

    '    'Reads word metrics
    '    'If a word metrics library is supplied, this will be used to lookup the requested word metrics, and all metrics supplied in the speech material file will be ignored.
    '    If WML Is Nothing Then

    '        'This part of the code is used when word metric data is to be supplied directly from the speech material file
    '        For i = PreWordMetricColumns To Values.Count - 1

    '            'Checking that the right number of data is given for each word 
    '            Dim RequiredLength As Integer = PreWordMetricColumns + WordMetricNames.Count
    '            If Values.Count < RequiredLength Then
    '                Throw New Exception("Too few columns in the speech material file at the row: " & ValueString)
    '            End If
    '            If Values.Count > RequiredLength Then
    '                Throw New Exception("Too many columns in the speech material file at the row: " & ValueString)
    '            End If

    '            'Determines the type of word metric supplied
    '            If WordMetricTypes(i - PreWordMetricColumns) = VariableTypes.Numeric Then

    '                'Parses the word metric as a Double and throws an error if parsing was not successful.
    '                Dim NumericValue As Double
    '                Dim NumericValueString As String = Values(i).Trim.Replace(",", ".")
    '                If NumericValueString.Trim = "" Then
    '                    Throw New Exception("Missing numeric " & WordMetricNames(i - PreWordMetricColumns) & " word metric value for the word Id " & Id & " in the speech-material file. (As long as the variable name of a numeric word metrics variable is supplied, values are mandatory for all words!")
    '                End If

    '                If Double.TryParse(NumericValueString, NumericValue) = False Then
    '                    Throw New Exception("Unable to parse the string " & NumericValueString & " as a " & WordMetricNames(i - PreWordMetricColumns) & " word-metric value for the word Id " & Id & " in the speech-material file.")
    '                End If

    '                NumericWordMetrics.Add(WordMetricNames(i - PreWordMetricColumns), NumericValue)

    '            ElseIf WordMetricTypes(i - PreWordMetricColumns) = VariableTypes.Categorical Then

    '                CategoricalWordMetrics.Add(WordMetricNames(i - PreWordMetricColumns), Values(i).Trim)

    '            Else
    '                Throw New Exception("The word metric type " & WordMetricTypes(i - PreWordMetricColumns) & " is not valid (It should be Numeric or Categorical). Please check your speech-material file at Id: " & Id)
    '            End If

    '        Next


    '    Else

    '        'This part of the code is used to supply test words with word metric data
    '        For i = 0 To WordMetricNames.Count - 1

    '            Dim LoopupResult As Object = WML.GetWordMetricValue(Id, WordMetricNames(i))
    '            If LoopupResult = Nothing Then
    '                Throw New Exception("Unable to find the Id " & Id & "in the word metrics library: " & WML.FilePath)
    '            End If

    '            'Determines the type of word metric supplied
    '            If WordMetricTypes(i) = VariableTypes.Numeric Then
    '                NumericWordMetrics.Add(WordMetricNames(i), LoopupResult)
    '            ElseIf WordMetricTypes(i) = VariableTypes.Categorical Then
    '                CategoricalWordMetrics.Add(WordMetricNames(i), LoopupResult)
    '            Else
    '                Throw New Exception("The word metric type " & WordMetricTypes(i) & " is not valid (It should be Numeric or Categorical). Please check your speech-material file and your word metrics library file at Id: " & Id)
    '            End If

    '        Next


    '    End If

    '    Return New Tuple(Of String, String, String, String, String, SortedList(Of String, Double), SortedList(Of String, String))(Id, SoundFileSubFolder, MaskerSoundFileSubFolder, Spelling, PhoneticTranscription, NumericWordMetrics, CategoricalWordMetrics)

    'End Function

#End Region



End Class


Public Enum VariableTypes
    Numeric
    Categorical
End Enum
