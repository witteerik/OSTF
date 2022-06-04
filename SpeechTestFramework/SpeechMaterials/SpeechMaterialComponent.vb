Imports System.Globalization

Public Class SpeechMaterialComponent

    Public Property LinguisticLevel As LinguisticLevels

    Public Enum LinguisticLevels
        ListCollection ' Represents a collection of test lists which together forms a speech material. This level cannot have sound recordings.
        List ' Represents a full speech test list. Should always have one or more sentences as child components. This level may have sound recordings.
        Sentence ' Represents a sentence, may have one or more words as child components (in a word list, each sentence will always have only one single word). This level may have sound recordings.
        Word ' Represents a word in a sentence, may have one or more phonemes as child components. This level may have sound recordings.
        Phoneme ' Represents a phoneme in a word. This level may have sound recordings.
    End Enum

    Public Property Id As String

    Public Property ParentComponent As SpeechMaterialComponent

    Public Property PrimaryStringRepresentation As String

    Public Property ChildComponents As New List(Of SpeechMaterialComponent)

    'These two should contain the data defined in the LinguisticDatabase associated to the component in the speech material file.
    Private NumericVariables As New SortedList(Of String, Double)
    Private CategoricalVariables As New SortedList(Of String, String)

    ' This variable is loaded from the speech material file and contains the full path to a custom variables database file for the component. The data is stored within the objects NumericVariables and CategoricalVariables.
    ' The path is stored to be able to write to the same file in order to update the variables.
    Public CustomVariablesDatabasePath As String = ""

    'These two should contain the data defined in the TestSituationDatabase associated to the component in the speech material file.
    Private NumericTestSituationVariables As New SortedList(Of String, SortedList(Of String, Double)) ' Test situation Id, Variable name, Variable Value
    Private CategoricalTestSituationVariables As New SortedList(Of String, SortedList(Of String, String)) ' Test situation Id, Variable name, Variable Value

    ' This variable should contain a subpath to a custom variables database file in the test situation folder in which test situation specific data for the component are stored. 
    ' Once these data are loaded/created, they are stored in the objects NumericTestSituationVariables and CategoricalTestSituationVariables.
    ' Only the filename is saved to and read from the speech material component file
    Private TestSituationDatabaseSubPath As String = ""

    ''' <summary>
    ''' The Id used to refer to the component in the LinguisticDatabase and/or the TestSituationDatabase
    ''' </summary>
    Public DbId As String = ""

    Public Function GetTestSituationVariableValue()
        'This function should somehow returns the requested variable values from the indicated test situation, or even offer an option to create/calculate that data if not present.
        Throw New NotImplementedException
    End Function

    Public Property OrderedChildren As Boolean = False

    Public Property MediaFolder As String
    Public Property MaskerFolder As String
    Public Property BackgroundNonspeechFolder As String
    Public Property BackgroundSpeechFolder As String


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

    'Setting up some default strings
    Public Shared DefaultSpellingVariableName As String = "Spelling"
    Public Shared DefaultTranscriptionVariableName As String = "Transcription"

    'Creating default names for database files
    Public Shared SpeechMaterialLevelDatabaseName As String = "SpeechMaterialLevelDatabase.txt"
    Public Shared ListLevelDataBaseName As String = "ListLevelVariables.txt"
    Public Shared SentenceLevelDataBaseName As String = "SentenceLevelVariables.txt"
    Public Shared WordLevelDataBaseName As String = "WordLevelVariables.txt"
    Public Shared PhonemeLevelDataBaseName As String = "PhonemeLevelVariables.txt"


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
                Throw New Exception("Unknown value For MediaType")
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
    ''' Returns all variable names at any component at the indicated Linguistic level and their type (as a boolean IsNumeric), and checks that no variable name is used as both numeric and categorical.
    ''' </summary>
    ''' <param name="LinguisticLevel"></param>
    ''' <returns></returns>
    Public Function GetCustomVariableNameAndTypes(ByVal LinguisticLevel As SpeechMaterialComponent.LinguisticLevels) As SortedList(Of String, Boolean)

        Dim AllCustomVariables As New SortedList(Of String, Boolean) ' Variable name, IsNumeric
        Dim AllTargetLevelComponents = GetAllRelativesAtLevel(LinguisticLevel)

        For Each Component In AllTargetLevelComponents

            Dim CategoricalVariableNames = Component.GetCategoricalVariableNames
            Dim NumericVariableNames = Component.GetNumericVariableNames

            For Each CatName In CategoricalVariableNames

                'Checking that the variable name is not used as both numeric and categorical.
                If NumericVariableNames.Contains(CatName) Then
                    MsgBox("The variable name " & CatName & " exist as both categorical and numeric at the linguistic level " & LinguisticLevel &
                           " in the speech material component id: " & Component.Id & " ( " & Component.PrimaryStringRepresentation & " ). This is not allowed!", MsgBoxStyle.Information, "Getting custom variable names and types")
                    Return Nothing
                End If

                'Stores the variable name, and the IsNumeric value of False
                If AllCustomVariables.ContainsKey(CatName) = False Then AllCustomVariables.Add(CatName, False)
            Next

            For Each CatName In NumericVariableNames

                'Checking that the variable name is not used as both numeric and categorical.
                If CategoricalVariableNames.Contains(CatName) Then
                    MsgBox("The variable name " & CatName & " exist as both numeric and categorical at the linguistic level " & LinguisticLevel &
                           " in the speech material component id: " & Component.Id & " ( " & Component.PrimaryStringRepresentation & " ). This is not allowed!", MsgBoxStyle.Information, "Getting custom variable names and types")
                    Return Nothing
                End If

                'Stores the variable name, and the IsNumeric value of False
                If AllCustomVariables.ContainsKey(CatName) = False Then AllCustomVariables.Add(CatName, True)
            Next

        Next

        Return AllCustomVariables

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
    Public Function GetCategoricalVariableValue(ByVal VariableName As String) As String

        If CategoricalVariables.Keys.Contains(VariableName) Then
            Return CategoricalVariables(VariableName)
        End If

        Return ""

    End Function

    ''' <summary>
    ''' Returns the names of all categorical custom variabels
    ''' </summary>
    ''' <returns></returns>
    Public Function GetCategoricalVariableNames() As List(Of String)
        Return CategoricalVariables.Keys.ToList
    End Function


    ''' <summary>
    ''' Returns the names of all numeric custom variabels
    ''' </summary>
    ''' <returns></returns>
    Public Function GetNumericVariableNames() As List(Of String)
        Return NumericVariables.Keys.ToList
    End Function


    ''' <summary>
    ''' Adds the indicated Value to the indicated VariableName in the collection of CategoricalVariables. Adds the variable name if not already present.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <param name="Value"></param>
    Public Sub SetNumericWordMetricValue(ByVal VariableName As String, ByVal Value As Double)
        If NumericVariables.Keys.Contains(VariableName) = True Then
            NumericVariables(VariableName) = Value
        Else
            NumericVariables.Add(VariableName, Value)
        End If
    End Sub


    ''' <summary>
    ''' Adds the indicated Value to the indicated VariableName in the collection of CategoricalVariables. Adds the variable name if not already present.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <param name="Value"></param>
    Public Sub SetCategoricalVariableValue(ByVal VariableName As String, ByVal Value As String)
        If CategoricalVariables.Keys.Contains(VariableName) = True Then
            CategoricalVariables(VariableName) = Value
        Else
            CategoricalVariables.Add(VariableName, Value)
        End If
    End Sub

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
        If ParentComponent.OrderedChildren = False Then Throw New Exception("Cannot Return same-place cousins from unordered components. (Component id: " & ParentComponent.Id & ")")

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

    ''' <summary>
    ''' Returns the second cousin components that are stored at the same hierachical index orders.
    ''' </summary>
    ''' <returns></returns>
    Public Function SamePlaceSecondCousins() As List(Of SpeechMaterialComponent)

        Dim SelfIndex = GetSelfIndex()

        If SelfIndex Is Nothing Then Return Nothing

        Dim SiblingCount As Integer = ParentComponent.GetSiblings.Count

        If ParentComponent Is Nothing Then Return Nothing

        Dim ParentSelfIndex = ParentComponent.GetSelfIndex()

        Dim ParentSiblingCount As Integer = ParentComponent.GetSiblings.Count

        If ParentSelfIndex Is Nothing Then Return Nothing

        If ParentComponent.ParentComponent Is Nothing Then Return Nothing

        'Checks that the grand-parent has ordered children
        If ParentComponent.ParentComponent.OrderedChildren = False Then Throw New Exception("Cannot return same-place second cousins from unordered components. (Component id: " & ParentComponent.ParentComponent.Id & ")")

        Dim ParentAunties = ParentComponent.ParentComponent.GetSiblingsExcludingSelf

        If ParentAunties Is Nothing Then Return Nothing

        Dim OutputList As New List(Of SpeechMaterialComponent)

        For Each parentAuntie In ParentAunties

            'Checks that the parent auntie have ordered children
            If parentAuntie.OrderedChildren = False Then Throw New Exception("Cannot return same-place cousins from unordered components. (Component id: " & parentAuntie.Id & ")")

            'Checks that the number of sibling components are the same
            Dim ParentAuntieChildCount As Integer = parentAuntie.ChildComponents.Count
            If ParentSiblingCount <> ParentAuntieChildCount Then Throw New Exception("Cannot return same-place cousins from cousin groups that differ in count. (Component ids: " & ParentComponent.Id & " vs. " & parentAuntie.Id & ")")

            'Getting the same place second cousins
            Dim SamePlaceParentCousin = parentAuntie.ChildComponents(ParentSelfIndex)

            'Checks that the SamePlaceParentCousin have ordered children
            If SamePlaceParentCousin.OrderedChildren = False Then Throw New Exception("Cannot return same-place cousins from unordered components. (Component id: " & SamePlaceParentCousin.Id & ")")

            'Checks that the number of sibling components are the same
            Dim SamePlaceParentCousinChildCount As Integer = SamePlaceParentCousin.ChildComponents.Count
            If SiblingCount <> SamePlaceParentCousinChildCount Then Throw New Exception("Cannot return same-place second cousins from cousin groups that differ in count. (Component ids: " & ParentComponent.Id & " vs. " & SamePlaceParentCousin.Id & ")")

            OutputList.Add(SamePlaceParentCousin.ChildComponents(SelfIndex))

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

    Public Function GetAllRelativesAtLevel(ByVal Level As SpeechMaterialComponent.LinguisticLevels) As List(Of SpeechMaterialComponent)
        'Creates a list
        Dim OutputList As New List(Of SpeechMaterialComponent)
        Dim AllRelatives = GetAllRelatives()
        For Each Component In AllRelatives
            If Component.LinguisticLevel = Level Then OutputList.Add(Component)
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

    Public Shared Function LoadSpeechMaterial(ByVal FilePath As String) As SpeechMaterialComponent

        Dim CurrentTestRootPath As String = IO.Path.Combine(OstfSettings.RootPath, OstfSettings.CurrentTestSubPath)

        'Gets a file path from the user if none is supplied
        If FilePath = "" Then FilePath = Utils.GetOpenFilePath(,, {".txt"}, "Please open a stuctured speech material component .txt file.")
        If FilePath = "" Then
            MsgBox("No file selected!")
            Return Nothing
        End If

        'Creates a new random that will be references in all speech material components
        Dim rnd As New Random

        Dim Output As SpeechMaterialComponent = Nothing

        'Parses the input file
        Dim InputLines() As String = System.IO.File.ReadAllLines(InputFileSupport.InputFilePathValueParsing(FilePath, CurrentTestRootPath, False), Text.Encoding.UTF8)

        Dim CustomVariablesDatabases As New SortedList(Of String, CustomVariablesDatabase)

        Dim IdsUsed As New SortedSet(Of String)

        For Each Line In InputLines

            'Skipping blank lines
            If Line.Trim = "" Then Continue For

            'Also skipping commentary only lines 
            If Line.Trim.StartsWith("//") Then Continue For

            Dim SplitRow = Line.Split(vbTab)

            If SplitRow.Length < 12 Then Throw New ArgumentException("Not enough data columns in the file " & FilePath & vbCrLf & "At the line: " & Line)

            Dim NewComponent As New SpeechMaterialComponent(rnd)

            'Adds component data
            Dim index As Integer = 0

            'Linguistic Level
            Dim LinguisticLevel = InputFileSupport.InputFileEnumValueParsing(SplitRow(index), GetType(LinguisticLevels), FilePath, False)
            If LinguisticLevel IsNot Nothing Then
                NewComponent.LinguisticLevel = LinguisticLevel
            Else
                Throw New Exception("Missing value for LinguisticLevel detected in the speech material file. A value for LinguisticLevel is obligatory for all speech material components. Line: " & vbCrLf & Line & vbCrLf &
                                    "Possible values are:" & vbCrLf & String.Join(" ", [Enum].GetNames(GetType(LinguisticLevels))))
            End If
            index += 1

            NewComponent.Id = InputFileSupport.GetInputFileValue(SplitRow(index), False)
            index += 1

            'Checking that the Id is not already used (Ids can only be used once throughout all speech component levels!!!)
            If IdsUsed.Contains(NewComponent.Id) Then
                Throw New ArgumentException("Re-used Id (" & NewComponent.Id & ")! Speech material components must only be used once throughout the whole speech material!")
            Else
                'Adding the Id to IdsUsed
                IdsUsed.Add(NewComponent.Id)
            End If

            ' Reading ParentId (which is used below
            Dim ParentId As String = InputFileSupport.GetInputFileValue(SplitRow(index), False)
            index += 1

            ' PrimaryStringRepresentation
            NewComponent.PrimaryStringRepresentation = InputFileSupport.GetInputFileValue(SplitRow(index), False)
            index += 1

            ' Getting the custom variables path
            Dim CustomVariablesDatabaseSubPath As String = InputFileSupport.InputFilePathValueParsing(SplitRow(index), CurrentTestRootPath, False)
            Dim CustomVariablesDatabasePath As String = IO.Path.Combine(CurrentTestRootPath, "CustomVariables", CustomVariablesDatabaseSubPath)
            If CustomVariablesDatabaseSubPath.Trim <> "" Then
                NewComponent.CustomVariablesDatabasePath = CustomVariablesDatabasePath
            End If
            index += 1

            ' Adding the test situation database subpath
            Dim TestSituationDatabaseSubPath As String = InputFileSupport.InputFilePathValueParsing(SplitRow(index), CurrentTestRootPath, False)
            NewComponent.TestSituationDatabaseSubPath = TestSituationDatabaseSubPath
            index += 1

            ' Adding the DbId
            Dim DbId As String = InputFileSupport.GetInputFileValue(SplitRow(index), False)
            NewComponent.DbId = DbId
            index += 1

            ' Adding the custom variables
            If CustomVariablesDatabaseSubPath.Trim <> "" Then
                If CustomVariablesDatabases.ContainsKey(CustomVariablesDatabasePath) = False Then
                    'Loading the database
                    Dim NewDatabase As New CustomVariablesDatabase
                    NewDatabase.LoadTabDelimitedFile(CustomVariablesDatabasePath)
                    CustomVariablesDatabases.Add(CustomVariablesDatabasePath, NewDatabase)
                End If

                'Adding the variables
                For n = 0 To CustomVariablesDatabases(CustomVariablesDatabasePath).CustomVariableNames.Count - 1
                    Dim VariableName = CustomVariablesDatabases(CustomVariablesDatabasePath).CustomVariableNames(n)
                    If CustomVariablesDatabases(CustomVariablesDatabasePath).CustomVariableTypes(n) = VariableTypes.Categorical Then
                        NewComponent.CategoricalVariables.Add(VariableName, CustomVariablesDatabases(CustomVariablesDatabasePath).GetVariableValue(DbId, VariableName))
                    ElseIf CustomVariablesDatabases(CustomVariablesDatabasePath).CustomVariableTypes(n) = VariableTypes.Numeric Then
                        NewComponent.NumericVariables.Add(VariableName, CustomVariablesDatabases(CustomVariablesDatabasePath).GetVariableValue(DbId, VariableName))
                    ElseIf CustomVariablesDatabases(CustomVariablesDatabasePath).CustomVariableTypes(n) = VariableTypes.Boolean Then
                        NewComponent.NumericVariables.Add(VariableName, CustomVariablesDatabases(CustomVariablesDatabasePath).GetVariableValue(DbId, VariableName))
                    Else
                        Throw New NotImplementedException("Variable type not implemented!")
                    End If
                Next
            End If

            'Adds further component data
            Dim OrderedChildren = InputFileSupport.InputFileBooleanValueParsing(SplitRow(index), False, FilePath)
            If OrderedChildren IsNot Nothing Then NewComponent.OrderedChildren = OrderedChildren
            index += 1

            NewComponent.MediaFolder = InputFileSupport.InputFilePathValueParsing(SplitRow(index), CurrentTestRootPath, False)
            index += 1

            NewComponent.MaskerFolder = InputFileSupport.InputFilePathValueParsing(SplitRow(index), CurrentTestRootPath, False)
            index += 1

            NewComponent.BackgroundNonspeechFolder = InputFileSupport.InputFilePathValueParsing(SplitRow(index), CurrentTestRootPath, False)
            index += 1

            NewComponent.BackgroundSpeechFolder = InputFileSupport.InputFilePathValueParsing(SplitRow(index), CurrentTestRootPath, False)
            index += 1

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

    Public Function GetClosestAncestorComponent(ByVal RequestedParentComponentLevel As SpeechMaterialComponent.LinguisticLevels) As SpeechMaterialComponent

        If ParentComponent Is Nothing Then Return Nothing

        If ParentComponent.LinguisticLevel = RequestedParentComponentLevel Then
            Return ParentComponent
        Else
            Return ParentComponent.GetClosestAncestorComponent(RequestedParentComponentLevel)
        End If

    End Function

    Public Function GetAllDescenentsAtLevel(ByVal RequestedDescendentComponentLevel As SpeechMaterialComponent.LinguisticLevels) As List(Of SpeechMaterialComponent)

        Dim OutputList As New List(Of SpeechMaterialComponent)

        For Each child In ChildComponents

            If child.LinguisticLevel = RequestedDescendentComponentLevel Then
                OutputList.Add(child)
            Else
                OutputList.AddRange(child.GetAllDescenentsAtLevel(RequestedDescendentComponentLevel))
            End If
        Next

        Return OutputList

    End Function

    Public Function GetClosestAncestorWithSoundMedia() As SpeechMaterialComponent

        If ParentComponent Is Nothing Then Return Nothing

        If ParentComponent.MediaFolder <> "" Then
            Return ParentComponent
        Else
            Return ParentComponent.GetClosestAncestorWithSoundMedia()
        End If

    End Function


    ''' <summary>
    ''' Converts the speech material component to a new SpeechMaterialAnnotation object prepared for manual segmentation.
    ''' </summary>
    ''' <returns></returns>
    Public Function ConvertToSMA() As Audio.Sound.SpeechMaterialAnnotation

        If Me.LinguisticLevel = LinguisticLevels.ListCollection Then
            MsgBox("Cannot convert a component at the ListCollection linguistic level to a SMA object. The highest level which can be stored in a SMA object is LinguisticLevels.List." & vbCrLf & "Aborting conversion!")
            Return Nothing
        End If

        Dim NewSMA = New Audio.Sound.SpeechMaterialAnnotation With {.SegmentationCompleted = False}

        'Creating a (mono) channel level SmaComponent
        NewSMA.AddChannelData(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.CHANNEL, Nothing))

        'Adjusting to the right level
        If Me.LinguisticLevel = LinguisticLevels.Phoneme Then

            'We need to add all levels: Sentence, Word, Phone
            NewSMA.ChannelData(1).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE, NewSMA.ChannelData(1)))
            NewSMA.ChannelData(1)(0).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD, NewSMA.ChannelData(1)(0)))
            NewSMA.ChannelData(1)(0)(0).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, NewSMA.ChannelData(1)(0)(0)))

            'Calling AddSmaValues, which recursively adds all lower level components
            AddSmaValues(NewSMA.ChannelData(1)(0)(0)(0))

        ElseIf Me.LinguisticLevel = LinguisticLevels.Word Then

            'We need to add all levels: Sentence, Word
            NewSMA.ChannelData(1).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE, NewSMA.ChannelData(1)))
            NewSMA.ChannelData(1)(0).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD, NewSMA.ChannelData(1)(0)))

            'Calling AddSmaValues, which recursively adds all lower level components
            AddSmaValues(NewSMA.ChannelData(1)(0)(0))

        ElseIf Me.LinguisticLevel = LinguisticLevels.Sentence Then

            'We need to add all levels: Sentence
            NewSMA.ChannelData(1).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE, NewSMA.ChannelData(1)))
            AddSmaValues(NewSMA.ChannelData(1)(0))

        ElseIf Me.LinguisticLevel = LinguisticLevels.List Then

            'No need to add any levels. Calling AddSmaValues, which recursively adds all lower level components
            AddSmaValues(NewSMA.ChannelData(1))

        Else

            MsgBox("Unknown value for the LinguisticLevels Enum" & vbCrLf & "Aborting conversion!")
            Return Nothing

        End If

        Return NewSMA

    End Function

    Private Sub AddSmaValues(ByRef SmaComponent As Audio.Sound.SpeechMaterialAnnotation.SmaComponent)

        'Attemption to get the spelling and transcription from the custom variables
        Dim MySpelling As String = ""
        Dim SpellingCandidateVariableNames As New List(Of String) From {"Spelling", "OrthographicForm"}
        For Each vn In SpellingCandidateVariableNames
            Dim SpellingCandidate = GetCategoricalVariableValue(vn)
            If SpellingCandidate.Trim <> "" Then
                MySpelling = SpellingCandidate
                Exit For
            End If
        Next

        Dim MyTranscription As String = ""
        Dim TranscriptionCandidateVariableNames As New List(Of String) From {"Transcription", "PhonemicForm", "PhoneticForm", "PhoneticTranscription", "PhonemicTranscription"}
        For Each vn In TranscriptionCandidateVariableNames
            Dim TranscriptionCandidate = GetCategoricalVariableValue(vn)
            If TranscriptionCandidate.Trim <> "" Then
                MyTranscription = TranscriptionCandidate
                Exit For
            End If
        Next

        'Using the PrimaryStringRepresentation instead of the spelling it was not found
        If MySpelling = "" Then MySpelling = Me.PrimaryStringRepresentation

        'Using MySpelling in place of the transcription of it was not found
        If MyTranscription = "" Then MyTranscription = MySpelling

        SmaComponent.OrthographicForm = MySpelling
        SmaComponent.PhoneticForm = MyTranscription

        For Each child In Me.ChildComponents

            Dim NewChildComponent = New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(SmaComponent.ParentSMA, SmaComponent.SmaTag + 1, SmaComponent)
            SmaComponent.Add(NewChildComponent)
            child.AddSmaValues(NewChildComponent)

        Next

    End Sub

    Public Sub WriteSpeechMaterialComponenFile(Optional ByVal FilePath As String = "")

        If FilePath = "" Then
            FilePath = Utils.GetSaveFilePath(,, {".txt"}, "Save speech material component file as.")
        End If

        WriteSpeechMaterialComponenFile(FilePath, True)

    End Sub


    Private Sub WriteSpeechMaterialComponenFile(ByVal FilePath As String, ByVal ExportCustomVariablesAtThisLevel As Boolean, Optional ByRef CustomVariablesExportList As SortedList(Of String, List(Of String)) = Nothing)

        If CustomVariablesExportList Is Nothing Then CustomVariablesExportList = New SortedList(Of String, List(Of String))

        Dim HeadingString As String = "// LinguisticLevel" & vbTab & "Id" & vbTab & "ParentId" & vbTab & "PrimaryStringRepresentation" & vbTab & "CustomVariablesDatabase" & vbTab & "TestSituationDatabase" & vbTab & "DbId" & vbTab &
                "OrderedChildren" & vbTab & "MediaFolder" & vbTab &
                "MaskerFolder" & vbTab & "BackgroundNonspeechFolder" & vbTab & "BackgroundSpeechFolder"

        Dim Main_List As New List(Of String)

        'Linguistic Level
        Main_List.Add(LinguisticLevel.ToString)

        'Id
        Main_List.Add(Id)

        'ParentId 
        If ParentComponent IsNot Nothing Then
            Main_List.Add(ParentComponent.Id)
        Else
            Main_List.Add("")
        End If

        'PrimaryStringRepresentation
        Main_List.Add(PrimaryStringRepresentation)

        'CustomVariablesDatabase 
        If CustomVariablesDatabasePath <> "" Then
            Dim CurrentDataBasePath = IO.Path.GetFileName(CustomVariablesDatabasePath)
            Main_List.Add(CurrentDataBasePath)
        Else
            Main_List.Add("")
        End If

        'TestSituationDatabase
        If TestSituationDatabaseSubPath <> "" Then
            Dim CurrentDataBasePath = IO.Path.GetFileName(TestSituationDatabaseSubPath)
            Main_List.Add(CurrentDataBasePath)
            'TODO: If this functionality is going to be used, then we need to add code for exporting these variables here
        Else
            Main_List.Add("")
        End If

        'DbId 
        Main_List.Add(DbId)

        'OrderedChildren 
        Main_List.Add(OrderedChildren.ToString)

        'MediaFolder 
        Main_List.Add(MediaFolder)
        'If MediaFolder <> "" Then
        '    Main_List.Add(Id)
        'Else
        '    Main_List.Add("")
        'End If

        'MaskerFolder 
        Main_List.Add(MaskerFolder)

        'BackgroundNonspeechFolder 
        Main_List.Add(BackgroundNonspeechFolder)

        'BackgroundSpeechFolder 
        Main_List.Add(BackgroundSpeechFolder)

        Dim OutputList As New List(Of String)
        OutputList.Add(HeadingString)
        OutputList.Add(String.Join(vbTab, Main_List))
        OutputList.Add("") 'Adding an empty line between components

        'Writing to file
        Utils.SendInfoToLog(String.Join(vbCrLf, OutputList), IO.Path.GetFileNameWithoutExtension(FilePath), IO.Path.GetDirectoryName(FilePath), True, True)


        'Custom variables
        If CustomVariablesDatabasePath <> "" Then

            Dim CurrentCustomVariablesOutputList As New List(Of String)
            If CustomVariablesExportList.ContainsKey(CustomVariablesDatabasePath) = False Then
                CustomVariablesExportList.Add(CustomVariablesDatabasePath, CurrentCustomVariablesOutputList)
            Else
                CurrentCustomVariablesOutputList = CustomVariablesExportList(CustomVariablesDatabasePath)
            End If

            Dim CustomVariableNames As New List(Of String)
            Dim CustomVariableTypes As New List(Of String)
            Dim CustomVariablesValues As New List(Of String)

            'Getting variable names, type and value
            For Each CustomVariable In CategoricalVariables
                CustomVariableNames.Add(CustomVariable.Key)
                CustomVariableTypes.Add("C")
                CustomVariablesValues.Add(CustomVariable.Value)
            Next

            For Each CustomVariable In NumericVariables
                CustomVariableNames.Add(CustomVariable.Key)
                CustomVariableTypes.Add("N")
                CustomVariablesValues.Add(CustomVariable.Value)
            Next

            'Adding headings only if not already present
            If CurrentCustomVariablesOutputList.Count = 0 Then
                Dim VariableNames = String.Join(vbTab, CustomVariableNames).Trim
                If VariableNames <> "" Then CurrentCustomVariablesOutputList.Add(String.Join(vbTab, CustomVariableNames))

                Dim VariableTypes = String.Join(vbTab, CustomVariableTypes).Trim
                If VariableTypes <> "" Then CurrentCustomVariablesOutputList.Add(VariableTypes)
            End If
            Dim VariableValues = String.Join(vbTab, CustomVariablesValues).Trim
            If VariableValues <> "" Then CurrentCustomVariablesOutputList.Add(VariableValues)

        End If

        'Cascading to all child components
        For Each ChildComponent In Me.ChildComponents
            ChildComponent.WriteSpeechMaterialComponenFile(FilePath, False, CustomVariablesExportList)
        Next

        If ExportCustomVariablesAtThisLevel = True Then
            'Exporting custom variables
            For Each item In CustomVariablesExportList
                Utils.SendInfoToLog(String.Join(vbCrLf, item.Value), IO.Path.GetFileNameWithoutExtension(item.Key), IO.Path.GetDirectoryName(FilePath), True, True)
            Next
        End If

    End Sub

    Public Sub SummariseNumericVariables(ByVal SourceLevels As SpeechMaterialComponent.LinguisticLevels, ByVal CustomVariableName As String, ByRef MetricType As NumericSummaryMetricTypes)

        If Me.LinguisticLevel < SourceLevels Then

            Dim Descendants = GetAllDescenentsAtLevel(SourceLevels)

            Dim ValueList As New List(Of Double)
            For Each d In Descendants
                ValueList.Add(d.GetNumericVariableValue(CustomVariableName))
            Next

            Select Case MetricType
                Case NumericSummaryMetricTypes.ArithmeticMean

                    'Storing the result
                    Dim SummaryResult As Double = ValueList.Average
                    Me.SetNumericWordMetricValue("Average_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.StandardDeviation

                    'Storing the result
                    Dim SummaryResult As Double = MathNet.Numerics.Statistics.Statistics.StandardDeviation(ValueList)
                    Me.SetNumericWordMetricValue("SD_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.Maximum

                    'Storing the result
                    Dim SummaryResult As Double = ValueList.Max
                    Me.SetNumericWordMetricValue("Max_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.Minimum

                    'Storing the result
                    Dim SummaryResult As Double = ValueList.Min
                    Me.SetNumericWordMetricValue("Min_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.Median

                    Dim SummaryResult As Double = MathNet.Numerics.Statistics.Statistics.Median(ValueList)
                    Me.SetNumericWordMetricValue("Md_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.InterquartileRange

                    Dim SummaryResult As Double = MathNet.Numerics.Statistics.Statistics.InterquartileRange(ValueList)
                    Me.SetNumericWordMetricValue("IQR_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.CoefficientOfVariation

                    'Storing the result
                    Dim SummaryResult As Double = Utils.CoefficientOfVariation(ValueList)
                    Me.SetNumericWordMetricValue("CV_" & CustomVariableName, SummaryResult)

            End Select


            'Cascading calculations to lower levels
            'Calling this from within the conditional statment, as no descendants should exist at more than one
            'level, and if Me.LinguisticLevel >= SourceLevels no other descendants should be either, and thus the recursive calls can stop here.
            For Each Child In ChildComponents
                Child.SummariseNumericVariables(SourceLevels, CustomVariableName, MetricType)
            Next

        End If

    End Sub

    Public Enum NumericSummaryMetricTypes
        ArithmeticMean
        StandardDeviation
        Maximum
        Minimum
        Median
        InterquartileRange
        CoefficientOfVariation
    End Enum


    Public Sub SummariseCategoricalVariables(ByVal SourceLevels As SpeechMaterialComponent.LinguisticLevels, ByVal CustomVariableName As String, ByRef MetricType As CategoricalSummaryMetricTypes)

        If Me.LinguisticLevel < SourceLevels Then

            Dim Descendants = GetAllDescenentsAtLevel(SourceLevels)

            Dim ValueList As New SortedList(Of String, Integer)
            For Each d In Descendants
                Dim VariableValue As String = d.GetCategoricalVariableValue(CustomVariableName)

                'Adding missing variable values
                If ValueList.ContainsKey(VariableValue) = False Then ValueList.Add(d.GetCategoricalVariableValue(CustomVariableName), 0)

                'Counting the occurence of the specific variable value
                ValueList(d.GetCategoricalVariableValue(CustomVariableName)) += 1
            Next

            Select Case MetricType
                Case CategoricalSummaryMetricTypes.Mode

                    If ValueList.Count > 0 Then

                        'Getting the most common value (TODO: the following lines are probably rather inefficient way and could possibly need opimization with larger datasets...)
                        Dim MaxOccurences = ValueList.Values.Max
                        Dim ModeList As New List(Of String)
                        For Each CurrentValue In ValueList
                            If CurrentValue.Value = MaxOccurences Then ModeList.Add(CurrentValue.Key)
                        Next

                        'If there are more than one mode value (i.e. equal number of occurences) they are returned as comma separated strings
                        Dim ModeValuesString As String = String.Join(",", ModeList)

                        'Storing the result
                        Me.SetCategoricalVariableValue("Mode_" & CustomVariableName, ModeValuesString)
                    Else

                        'Storing the an empty string as result, as there was no item in ValueList
                        Me.SetCategoricalVariableValue("Mode_" & CustomVariableName, "")
                    End If

                Case CategoricalSummaryMetricTypes.Distribution

                    If ValueList.Count > 0 Then

                        'Getting the most common value
                        Dim DistributionList As New List(Of String)
                        For Each CurrentValue In ValueList
                            DistributionList.Add(CurrentValue.Key & "," & CurrentValue.Value)
                        Next

                        'The distribution are returned as vertical bar (|) separatered key value pairs of value and number of occurences
                        '(this rather akward format choice is selected in order to be able to use tab delimited files. In this way, the whole
                        'distribution may be put in a single cell, for instance in Excel (given that the maximum number of cell characters in not reached...)) 
                        Dim DistributionString As String = String.Join("|", DistributionList)

                        'TODO: This should really be sorted in freuency!

                        'Storing the result
                        Me.SetCategoricalVariableValue("Distribution_" & CustomVariableName, DistributionString)
                    Else

                        'Storing the an empty string as result, as there was no item in ValueList
                        Me.SetCategoricalVariableValue("Distribution_" & CustomVariableName, "")
                    End If

            End Select

            'Cascading calculations to lower levels
            'Calling this from within the conditional statment, as no descendants should exist at more than one
            'level, and if Me.LinguisticLevel >= SourceLevels no other descendants should be either, and thus the recursive calls can stop here.
            For Each Child In ChildComponents
                Child.SummariseCategoricalVariables(SourceLevels, CustomVariableName, MetricType)
            Next

        End If

    End Sub

    Public Enum CategoricalSummaryMetricTypes
        Mode
        Distribution
    End Enum

End Class


''' <summary>
''' A class for looking up custom variables for speech material components.
''' </summary>
Public Class CustomVariablesDatabase

    Private CustomVariablesData As New Dictionary(Of String, SortedList(Of String, Object))

    Public CustomVariableNames As New List(Of String)
    Public CustomVariableTypes As New List(Of VariableTypes)

    Public CaseInvariantSpellings As Boolean

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

            'Assuming that there is no data, if the first line is empty!
            If InputLines(0).Trim = "" Then
                Return False
            End If

            'First line should be variable names
            Dim FirstLineData() As String = InputLines(0).Trim(vbTab).Split(vbTab)
            For c = 0 To FirstLineData.Length - 1
                CustomVariableNames.Add(FirstLineData(c).Trim)
            Next

            'Second line should be variable types (N for Numeric or C for Categorical)
            Dim SecondLineData() As String = InputLines(1).Trim(vbTab).Split(vbTab)
            For c = 0 To SecondLineData.Length - 1
                If SecondLineData(c).Trim.ToLower = "n" Then
                    CustomVariableTypes.Add(VariableTypes.Numeric)
                ElseIf SecondLineData(c).Trim.ToLower = "c" Then
                    CustomVariableTypes.Add(VariableTypes.Categorical)
                ElseIf SecondLineData(c).Trim.ToLower = "b" Then
                    CustomVariableTypes.Add(VariableTypes.Boolean)
                Else
                    Throw New Exception("The type for the custom variable " & CustomVariableNames(c) & " in the file " & FilePath & " must be either N for numeric or C for categorical, or B for Boolean.")
                End If
            Next

            'Reading data
            For i = 2 To InputLines.Length - 1

                Dim LineSplit() As String = InputLines(i).Split(vbTab)

                Dim UniqueIdentifier As String = LineSplit(0).Trim

                CustomVariablesData.Add(UniqueIdentifier, New SortedList(Of String, Object))

                'Adding variables (getting only as many as there are variables, or tabs)
                For c = 0 To Math.Min(LineSplit.Length - 1, CustomVariableNames.Count - 1)

                    Dim ValueString As String = LineSplit(c).Trim
                    If CustomVariableTypes(c) = VariableTypes.Numeric Then

                        'Adding the data as a Double
                        Dim NumericValue As Double
                        If Double.TryParse(ValueString.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, NumericValue) Then
                            'Adds the variable and its data only if a value has been parsed
                            CustomVariablesData(UniqueIdentifier).Add(CustomVariableNames(c), NumericValue)
                        Else
                            'Throws an error if parsing failed even though the string was not empty
                            If ValueString.Trim <> "" Then
                                Throw New Exception("Unable to parse the string " & NumericValue & " given for the variable " & CustomVariableNames(c) & " in the file: " & FilePath & " as a numeric value.")
                            Else
                                'Stores a NaN to mark that the input data was missing / NaN
                                CustomVariablesData(UniqueIdentifier).Add(CustomVariableNames(c), Double.NaN)
                            End If
                        End If

                    ElseIf CustomVariableTypes(c) = VariableTypes.Boolean Then

                        'Adding the data as a boolean
                        Dim BooleanValue As Boolean
                        If Boolean.TryParse(ValueString, BooleanValue) Then
                            'Adds the variable and its data only if a value has been parsed
                            CustomVariablesData(UniqueIdentifier).Add(CustomVariableNames(c), BooleanValue)
                        Else
                            'Throws an error if parsing failed even though the string was not empty
                            If ValueString.Trim <> "" Then
                                Throw New Exception("Unable to parse the string " & BooleanValue & " given for the variable " & CustomVariableNames(c) & " in the file: " & FilePath & " as a boolean value (True or False).")
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

            MsgBox("The following exception occurred while reading a custom variables file: " & ex.ToString)
            'TODO What here?
            Return False
        End Try


    End Function


    Public Enum LookupMathOptions
        MatchBySpelling
        MatchByTranscription
        MatchBySpellingAndTranscription
    End Enum

    Public Function LoadTabDelimitedFile(ByVal FilePath As String, ByVal MatchBy As LookupMathOptions,
                                         Optional ByVal SpellingVariableName As String = "", Optional ByVal TranscriptionVariableName As String = "",
                                         Optional ByVal IncludeItems As SortedSet(Of String) = Nothing) As Boolean


        ''Gets a file path from the user if none is supplied
        'If FilePath = "" Then FilePath = Utils.GetOpenFilePath(,, {".txt"}, "Please open a tab delimited word metrics .txt file.")
        'If FilePath = "" Then
        '    MsgBox("No file selected!")
        '    Return Nothing
        'End If


        'Checking arguments
        Select Case MatchBy
            Case LookupMathOptions.MatchBySpellingAndTranscription
                If SpellingVariableName = "" Then
                    MsgBox("Missing spelling variable name", MsgBoxStyle.Information, "Loading custom variables file")
                    Return False
                End If

                If TranscriptionVariableName = "" Then
                    MsgBox("Missing transcription variable name", MsgBoxStyle.Information, "Loading custom variables file")
                    Return False
                End If

            Case LookupMathOptions.MatchBySpelling
                If SpellingVariableName = "" Then
                    MsgBox("Missing spelling variable name", MsgBoxStyle.Information, "Loading custom variables file")
                    Return False
                End If

            Case LookupMathOptions.MatchByTranscription
                If TranscriptionVariableName = "" Then
                    MsgBox("Missing transcription variable name", MsgBoxStyle.Information, "Loading custom variables file")
                    Return False
                End If

        End Select

        Try

            CustomVariablesData.Clear()
            CustomVariableNames.Clear()
            CustomVariableTypes.Clear()

            'Parses the input file
            Dim InputLines() As String = System.IO.File.ReadAllLines(FilePath, Text.Encoding.UTF8)

            'Stores the file path used for loading the word metric data
            Me.FilePath = FilePath

            'Assuming that there is no data, if the first line is empty!
            If InputLines(0).Trim = "" Then
                Return False
            End If

            'First line should be variable names
            Dim FirstLineData() As String = InputLines(0).Trim(vbTab).Split(vbTab)
            For c = 0 To FirstLineData.Length - 1
                CustomVariableNames.Add(FirstLineData(c).Trim)
            Next

            'Looing for the column indices where the unique identifiers (spealling and/or transcription) are
            Dim SpellingColumnIndex As Integer = -1
            Dim TranscriptionColumnIndex As Integer = -1
            For c = 0 To FirstLineData.Length - 1
                If FirstLineData(c).Trim() = "" Then Continue For
                If FirstLineData(c).Trim = SpellingVariableName Then SpellingColumnIndex = c
                If FirstLineData(c).Trim = TranscriptionVariableName Then TranscriptionColumnIndex = c
            Next

            'Checking that the needed columns were found
            Select Case MatchBy
                Case LookupMathOptions.MatchBySpellingAndTranscription
                    If SpellingColumnIndex = -1 Then
                        MsgBox("No variable named " & SpellingVariableName & " could be found in the database file.", MsgBoxStyle.Information, "Missing variable")
                        Return False
                    End If
                    If TranscriptionColumnIndex = -1 Then
                        MsgBox("No variable named " & TranscriptionVariableName & " could be found in the database file.", MsgBoxStyle.Information, "Missing variable")
                        Return False
                    End If

                Case LookupMathOptions.MatchBySpelling
                    If SpellingColumnIndex = -1 Then
                        MsgBox("No variable named " & SpellingVariableName & " could be found in the database file.", MsgBoxStyle.Information, "Missing variable")
                        Return False
                    End If

                Case LookupMathOptions.MatchByTranscription
                    If TranscriptionColumnIndex = -1 Then
                        MsgBox("No variable named " & TranscriptionVariableName & " could be found in the database file.", MsgBoxStyle.Information, "Missing variable")
                        Return False
                    End If
            End Select


            'Second line should be variable types (N for Numeric or C for Categorical)
            Dim SecondLineData() As String = InputLines(1).Trim(vbTab).Split(vbTab)
            For c = 0 To SecondLineData.Length - 1
                If SecondLineData(c).Trim.ToLower = "n" Then
                    CustomVariableTypes.Add(VariableTypes.Numeric)
                ElseIf SecondLineData(c).Trim.ToLower = "c" Then
                    CustomVariableTypes.Add(VariableTypes.Categorical)
                ElseIf SecondLineData(c).Trim.ToLower = "b" Then
                    CustomVariableTypes.Add(VariableTypes.Boolean)
                Else
                    Throw New Exception("The type for the custom variable " & CustomVariableNames(c) & " in the file " & FilePath & " must be either N for numeric or C for categorical, or B for Boolean.")
                End If
            Next

            'Reading data
            For i = 2 To InputLines.Length - 1

                Dim LineSplit() As String = InputLines(i).Split(vbTab)

                'Gets the unique identifier
                Dim UniqueIdentifier As String = ""
                Select Case MatchBy
                    Case LookupMathOptions.MatchBySpellingAndTranscription
                        Dim Spelling As String = LineSplit(SpellingColumnIndex).Trim
                        If CaseInvariantSpellings = True Then
                            Spelling = Spelling.ToLower
                        End If
                        UniqueIdentifier = Spelling & vbTab & LineSplit(TranscriptionColumnIndex).Trim
                    Case LookupMathOptions.MatchBySpelling
                        Dim Spelling As String = LineSplit(SpellingColumnIndex).Trim
                        If CaseInvariantSpellings = True Then
                            Spelling = Spelling.ToLower
                        End If
                        UniqueIdentifier = Spelling
                    Case LookupMathOptions.MatchByTranscription
                        UniqueIdentifier = LineSplit(TranscriptionColumnIndex).Trim
                End Select

                'Skipping if IncludeItems has been set and the UniqueIdentifier is not in it (this is to save memory with very large databases!!!)
                If IncludeItems IsNot Nothing Then
                    If IncludeItems.Contains(UniqueIdentifier) = False Then Continue For
                End If

                If CustomVariablesData.ContainsKey(UniqueIdentifier) Then
                    Select Case MatchBy
                        Case LookupMathOptions.MatchBySpellingAndTranscription
                            MsgBox("There exist more than one instance of the spelling-transcription combination " & UniqueIdentifier & " in the lexical database. Unable to select the one to use!", MsgBoxStyle.Information, "Duplicate look-up keys")
                        Case LookupMathOptions.MatchBySpelling
                            MsgBox("There exist more than one instance of the spellings " & UniqueIdentifier & " in the lexical database. Unable to select the one to use!", MsgBoxStyle.Information, "Duplicate look-up keys")
                        Case LookupMathOptions.MatchByTranscription
                            MsgBox("There exist more than one instance of the transcription " & UniqueIdentifier & " in the lexical database. Unable to select the one to use!", MsgBoxStyle.Information, "Duplicate look-up keys")
                    End Select
                    Return False
                End If

                'Adding the unique identifier
                CustomVariablesData.Add(UniqueIdentifier, New SortedList(Of String, Object))

                'Adding variables (getting only as many as there are variables, or tabs)
                For c = 0 To Math.Min(LineSplit.Length - 1, CustomVariableNames.Count - 1)

                    Dim ValueString As String = LineSplit(c).Trim
                    If CustomVariableTypes(c) = VariableTypes.Numeric Then

                        'Adding the data as a Double
                        Dim NumericValue As Double
                        If Double.TryParse(ValueString.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, NumericValue) Then
                            'Adds the variable and its data only if a value has been parsed
                            CustomVariablesData(UniqueIdentifier).Add(CustomVariableNames(c), NumericValue)
                        Else
                            'Throws an error if parsing failed even though the string was not empty
                            If ValueString.Trim <> "" Then
                                Throw New Exception("Unable to parse the string " & ValueString & " given for the variable " & CustomVariableNames(c) & " in the file: " & FilePath & " as a numeric value.")
                            Else
                                'Stores a NaN to mark that the input data was missing / NaN
                                CustomVariablesData(UniqueIdentifier).Add(CustomVariableNames(c), Double.NaN)
                            End If
                        End If

                    ElseIf CustomVariableTypes(c) = VariableTypes.Boolean Then

                        'Adding the data as a boolean
                        Dim BooleanValue As Boolean
                        If Boolean.TryParse(ValueString, BooleanValue) Then
                            'Adds the variable and its data only if a value has been parsed
                            CustomVariablesData(UniqueIdentifier).Add(CustomVariableNames(c), BooleanValue)
                        Else
                            'Throws an error if parsing failed even though the string was not empty
                            If ValueString.Trim <> "" Then
                                Throw New Exception("Unable to parse the string " & BooleanValue & " given for the variable " & CustomVariableNames(c) & " in the file: " & FilePath & " as a boolean value (True or False).")
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

            MsgBox("The following exception occurred while reading a custom variables file: " & ex.ToString)
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

    Public Function UniqueIdentifierIsPresent(ByVal UniqueIdentifier As String) As Boolean
        If CustomVariablesData.ContainsKey(UniqueIdentifier) Then
            Return True
        Else
            Return False
        End If
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
    [Boolean]
End Enum
