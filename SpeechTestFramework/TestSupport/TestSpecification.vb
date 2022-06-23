
Public Class TestSpecification

    Public Const FormatFlag As String = "{OSTF_TEST_SPECIFICATION_FILE}"
    Public Const TestsDirectory As String = "Tests"
    Public Const TestSpecificationDirectory As String = "AvailableTests"
    Public Const MediaSetSpecificationDirectory As String = "AvailableMediaSets"

    Public ReadOnly Property Name As String = ""

    Public ReadOnly Property DirectoryName As String = ""


    Public Property TestPresetsSubFilePath As String = ""

    ''' <summary>
    ''' Once the TestSpecification text file has been read from file, this property contains the file name from which the TestSpecification text file was read.
    ''' </summary>
    ''' <returns></returns>
    Public Property TestSpecificationFileName As String = ""

    Public Property SpeechMaterial As SpeechMaterialComponent

    Public Property TestSituations As New MediaSetLibrary

    Public Function GetAvailableTestSituationNames() As List(Of String)
        Dim OutputList As New List(Of String)
        For Each TestSituation In TestSituations
            OutputList.Add(TestSituation.ToString)
        Next
        Return OutputList
    End Function


    Public Function GetTestsDirectory() As String
        Return IO.Path.Combine(OstfSettings.RootDirectory, TestsDirectory)
    End Function

    Public Function GetTestRootPath() As String
        Return IO.Path.Combine(GetTestsDirectory, DirectoryName)
    End Function

    Public Function GetSpeechMaterialFolder() As String
        Return IO.Path.Combine(GetTestRootPath, SpeechMaterialComponent.SpeechMaterialFolderName)
    End Function

    Public Function GetSpeechMaterialFilePath() As String
        Return IO.Path.Combine(GetSpeechMaterialFolder, SpeechMaterialComponent.SpeechMaterialComponentFileName)
    End Function

    Public Function GetAvailableTestSituationsDirectory() As String
        Return IO.Path.Combine(GetTestRootPath, MediaSetSpecificationDirectory)
    End Function


    Public Sub New(ByVal Name As String, ByVal DirectoryName As String)
        Me.Name = Name
        Me.DirectoryName = DirectoryName
    End Sub

    Public Shared Function LoadTestSpecificationFile(ByVal TextFileName As String) As TestSpecification

        Dim FullFilePath As String = IO.Path.Combine(OstfSettings.RootDirectory, OstfSettings.AvailableTestsSubFolder, TextFileName)

        If IO.File.Exists(FullFilePath) = False Then
            MsgBox("Unable to load the file " & FullFilePath, MsgBoxStyle.Critical, "Loading OSTF test specification file")
            Return Nothing
        End If

        Dim Input = IO.File.ReadAllLines(FullFilePath, Text.Encoding.UTF8)

        Dim FormatFlagDetected As Boolean = False

        Dim Name As String = ""
        Dim DirectoryName As String = ""
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

            If Input(line).Trim.StartsWith("DirectoryName") Then
                DirectoryName = InputFileSupport.GetInputFileValue(Input(line).Trim, True)
            End If

            If Input(line).Trim.StartsWith("TestPresetsSubFilePath") Then
                TestPresetsSubFilePath = InputFileSupport.GetInputFileValue(Input(line).Trim, True)
            End If

        Next

        Dim Output As New TestSpecification(Name, DirectoryName)
        Output.TestPresetsSubFilePath = TestPresetsSubFilePath

        Output.TestSpecificationFileName = TextFileName

        Return Output

    End Function

    Public Sub LoadSpeechMaterialComponentsFile()

        Try
            SpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(GetSpeechMaterialFilePath, GetTestRootPath)

            'Referencing the Me as ParentTestSpecification in the loaded SpeechMaterial 
            If SpeechMaterial IsNot Nothing Then
                SpeechMaterial.ParentTestSpecification = Me
            End If

        Catch ex As Exception
            MsgBox("Failed to load speech material file: " & GetSpeechMaterialFilePath())
        End Try

    End Sub

    Public Sub LoadAvailableTestSituationSpecifications()

        'Clears any test situations previously loaded before adding new ones
        TestSituations.Clear()

        'Looks in the appropriate folder for test situation specification files
        Dim TestSituationSpecificationFolder As String = GetAvailableTestSituationsDirectory()

        'Siliently exits if the TestSituationSpecificationFolder doesn't exist (which will happen when no TestSituationSpecifications have been created)
        If IO.Directory.Exists(TestSituationSpecificationFolder) = False Then Exit Sub

        'Getting .txt files in that folder
        Dim ExistingFiles = IO.Directory.GetFiles(TestSituationSpecificationFolder)
        Dim TextFiles As New List(Of String)
        For Each FullFilePath In ExistingFiles
            If FullFilePath.EndsWith(".txt") Then
                TextFiles.Add(FullFilePath)
            End If
        Next

        For Each TextFilePath In TextFiles
            'Tries to use the text file in order to create a new test specification object, and just skipps it if unsuccessful
            Dim NewSituationTestSpecification = MediaSet.LoadMediaSet(TextFilePath)
            If NewSituationTestSpecification IsNot Nothing Then

                'Setting the ParentTestSpecification
                NewSituationTestSpecification.ParentTestSpecification = Me

                'Adding the test situation
                TestSituations.Add(NewSituationTestSpecification)
            End If
        Next

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
            FilePath = Utils.GetSaveFilePath(,, {".txt"}, "Save OSTF test specification file as")
        End If

        Dim OutputList As New List(Of String)
        OutputList.Add("// This file is an OSTF test specification file. Its first non-empty line which is not commented out (using double slashes) must be exacly " & FormatFlag)
        OutputList.Add("// In order to make the test that this file specifies available in OSTF, put this file in the OSTF sub folder named: " & OstfSettings.AvailableTestsSubFolder & ", and the restart the OSTF software.")
        OutputList.Add("")
        OutputList.Add(FormatFlag)
        OutputList.Add("")

        If Name.Trim = "" Then
            OutputList.Add("// Name = [Add name here, and remove the double slashes]")
        Else
            OutputList.Add("Name = " & Name)
        End If

        If DirectoryName.Trim = "" Then
            OutputList.Add("// DirectoryName = Tests\ [Add the DirectoryName here, and remove the double slashes]")
        Else
            OutputList.Add("DirectoryName = " & DirectoryName)
        End If

        If TestPresetsSubFilePath.Trim = "" Then
            OutputList.Add("// TestPresetsSubFilePath =  [Add the file path to the TestPresets file here, and remove the double slashes]")
        Else
            OutputList.Add("TestPresetsSubFilePath = TestPresetsSubFilePath")
        End If

        Utils.SendInfoToLog(String.Join(vbCrLf, OutputList), IO.Path.GetFileNameWithoutExtension(FilePath), IO.Path.GetDirectoryName(FilePath), True, True)

    End Sub

End Class

