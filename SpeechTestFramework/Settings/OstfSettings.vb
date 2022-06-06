Public Module OstfSettings

    ' Program location
    Public Property RootDirectory As String = "C:\OSTF" 'Indicates the root path. Other paths given in the project setting files are relative (subpaths) to this path only if they begin with .\ otherwise they are taken as absolute paths.

    Public Property TestSpecificationSubFolder As String = "AvailableTests"

    Public Property AvailableTests As New List(Of TestSpecification)


    'Public Sub New()

    '    'AvailableTests.Add(New TestSpecification("HINT_Kort", "HINT_Kort", "ProjectFiles\SpeechMaterialComponents.txt"))
    '    'AvailableTests.Add(New TestSpecification("SwedishSiPTest", "SwedishSiPTest", "ProjectFiles\SpeechMaterialComponents.txt"))

    '    'CurrentlySelectedTest = AvailableTests(0)

    'End Sub

    Public Sub LoadAvailableTestSpecifications()

        Dim TestSpecificationFolder As String = IO.Path.Combine(RootDirectory, TestSpecificationSubFolder)

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
        OstfSettings.AvailableTests.Clear()

        For Each TextFileName In TextFileNames
            'Ties to use the text file in order to create a new test specification object, and just skipps it if unsuccessful
            Dim NewTestSpecification = TestSpecification.LoadTestSpecificationFile(TextFileName)
            If NewTestSpecification IsNot Nothing Then
                OstfSettings.AvailableTests.Add(NewTestSpecification)
            End If
        Next

    End Sub

End Module

Public Class TestSpecification

    Public Const FormatFlag As String = "{OSTF_TEST_SPECIFICATION_FILE}"

    Public ReadOnly Property Name As String = ""

    Public ReadOnly Property SubDirectory As String = ""

    Public ReadOnly Property SpeechMaterialComponentsSubFilePath As String = ""

    Public Property AvailableTestSituationsSubDirectory As String = ""

    Public Property TestPresetsSubFilePath As String = ""

    ''' <summary>
    ''' Once the TestSpecification text file has been read from file, this property contains the file name from which the TestSpecification text file was read.
    ''' </summary>
    ''' <returns></returns>
    Public Property TestSpecificationFileName As String = ""

    Public Property SpeechMaterial As SpeechMaterialComponent

    Public Property TestSituations As New MediaSetLibrary

    Public Property AvailableTestSituationNames As New List(Of String)


    Public Function TestRootPath() As String
        Return IO.Path.Combine(OstfSettings.RootDirectory, SubDirectory)
    End Function

    Public Function GetSpeechMaterialFilePath() As String
        Return IO.Path.Combine(TestRootPath, SpeechMaterialComponentsSubFilePath)
    End Function

    Public Sub New(ByVal Name As String, ByVal SubDirectory As String, ByVal SpeechMaterialComponentsSubFilePath As String)
        Me.Name = Name
        Me.SubDirectory = SubDirectory
        Me.SpeechMaterialComponentsSubFilePath = SpeechMaterialComponentsSubFilePath
    End Sub

    Public Shared Function LoadTestSpecificationFile(ByVal TextFileName As String) As TestSpecification

        Dim FullFilePath As String = IO.Path.Combine(OstfSettings.RootDirectory, OstfSettings.TestSpecificationSubFolder, TextFileName)

        If IO.File.Exists(FullFilePath) = False Then
            MsgBox("Unable to load the file " & FullFilePath, MsgBoxStyle.Critical, "Loading OSTF test specification file")
            Return Nothing
        End If

        Dim Input = IO.File.ReadAllLines(FullFilePath, Text.Encoding.UTF8)

        Dim FormatFlagDetected As Boolean = False

        Dim Name As String = ""
        Dim SubDirectory As String = ""
        Dim SpeechMaterialComponentsSubFilePath As String = ""
        Dim AvailableTestSituationsSubDirectory As String = ""
        Dim TestPresetsSubFilePath As String = ""

        For line = 0 To Input.Length - 1

            If Input(line).Trim = "" Then Continue For
            If Input(line).Trim.StartsWith("//") Then Continue For

            'Reads content
            'Checks that the first content line is the format flag
            If FormatFlagDetected = False Then
                If Input(line).Trim.StartsWith(FormatFlag) Then
                    FormatFlagDetected = True
                    Continue For
                Else
                    'The first content line in the file does not contain the format flag. Silently returns Nothing, and assumes that the file is not a OSTF test specification file
                    Return Nothing
                End If
            End If

            'Reads the remaining content
            If Input(line).Trim.StartsWith("Name") Then
                Name = InputFileSupport.GetInputFileValue(Input(line).Trim, True)
            End If

            If Input(line).Trim.StartsWith("SubDirectory") Then
                SubDirectory = InputFileSupport.GetInputFileValue(Input(line).Trim, True)
            End If

            If Input(line).Trim.StartsWith("SpeechMaterialComponentsSubFilePath") Then
                SpeechMaterialComponentsSubFilePath = InputFileSupport.GetInputFileValue(Input(line).Trim, True)
            End If

            If Input(line).Trim.StartsWith("AvailableTestSituationsSubDirectory") Then
                AvailableTestSituationsSubDirectory = InputFileSupport.GetInputFileValue(Input(line).Trim, True)
            End If

            If Input(line).Trim.StartsWith("TestPresetsSubFilePath") Then
                TestPresetsSubFilePath = InputFileSupport.GetInputFileValue(Input(line).Trim, True)
            End If

        Next

        Dim Output As New TestSpecification(Name, SubDirectory, SpeechMaterialComponentsSubFilePath)
        Output.AvailableTestSituationsSubDirectory = AvailableTestSituationsSubDirectory
        Output.TestPresetsSubFilePath = TestPresetsSubFilePath

        Output.TestSpecificationFileName = TextFileName

        Return Output

    End Function

    Public Sub LoadSpeechMaterialComponentsFile()

        Try
            SpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(GetSpeechMaterialFilePath, TestRootPath)
        Catch ex As Exception
            MsgBox("Failed to load speech material file: " & GetSpeechMaterialFilePath())
        End Try

    End Sub

    Public Sub LoadTestSituation(ByVal TestSituationName As String)

        'Store new test situation as MediaSet in TestSituations


    End Sub

    ''' <summary>
    ''' Overrides the default ToString method and returns the name of the test specification
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String

        Return Name

    End Function

    Public Sub WriteTextFile(Optional FilePath As String = "")

        If FilePath = "" Then
            FilePath = Utils.GetSaveFilePath(,, {".txt"}, "Save speech material component file as.")
        End If

        Dim OutputList As New List(Of String)
        OutputList.Add("// This file is a OSTF test specification file. Its first non-empty line which is not commented out (using double slashes) must be exacly " & FormatFlag)

        If Name.Trim = "" Then
            OutputList.Add("// Name = [Add name here, and remove the double slashes]")
        Else
            OutputList.Add("Name = " & Name)
        End If

        If SubDirectory.Trim = "" Then
            OutputList.Add("// SubDirectory = Tests\ [Add the subdirectory here, and remove the double slashes]")
        Else
            OutputList.Add("SubDirectory = " & SubDirectory)
        End If

        If SpeechMaterialComponentsSubFilePath.Trim = "" Then
            OutputList.Add("// SpeechMaterialComponentsSubFilePath = [Add the file path to the SpeechMaterialComponents file here, and remove the double slashes]")
        Else
            OutputList.Add("SpeechMaterialComponentsSubFilePath = " & SpeechMaterialComponentsSubFilePath)
        End If

        If AvailableTestSituationsSubDirectory.Trim = "" Then
            OutputList.Add("// AvailableTestSituationsSubDirectory = [Add the AvailableTestSituationsSubDirectory here, and remove the double slashes]")

        Else
            OutputList.Add("AvailableTestSituationsSubDirectory = " & AvailableTestSituationsSubDirectory)
        End If

        If TestPresetsSubFilePath.Trim = "" Then
            OutputList.Add("// TestPresetsSubFilePath =  [Add the file path to the TestPresets file here, and remove the double slashes]")
        Else
            OutputList.Add("TestPresetsSubFilePath = TestPresetsSubFilePath")
        End If

        Utils.SendInfoToLog(String.Join(vbCrLf, OutputList), IO.Path.GetFileNameWithoutExtension(FilePath), IO.Path.GetDirectoryName(FilePath), True, True)

    End Sub

End Class

