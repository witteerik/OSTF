Imports System.Runtime.Serialization

<Serializable>
Public Class SpeechMaterialSpecification

    Public Const FormatFlag As String = "{OSTF_SPEECH_MATERIAL_SPECIFICATION_FILE}"
    Public Const SpeechMaterialsDirectory As String = "SpeechMaterials"
    Public Const AvailableSpeechMaterialsDirectory As String = "AvailableSpeechMaterials"
    Public Const AvailableMediaSetsDirectory As String = "AvailableMediaSets"

    Public ReadOnly Property Name As String = ""

    Public ReadOnly Property DirectoryName As String = ""


    Public Property TestPresetsSubFilePath As String = ""

    ''' <summary>
    ''' Once the SpeechMaterialSpecification text file has been read from file, this property contains the file name from which the SpeechMaterialSpecification text file was read.
    ''' </summary>
    ''' <returns></returns>
    Public Property TestSpecificationFileName As String = ""

    Public Property SpeechMaterial As SpeechMaterialComponent

    Public Property MediaSets As New MediaSetLibrary


    Public Function GetTestsDirectory() As String
        Return IO.Path.Combine(OstfBase.MediaRootDirectory, SpeechMaterialsDirectory)
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
        Return IO.Path.Combine(GetTestRootPath, AvailableMediaSetsDirectory)
    End Function


    Public Sub New(ByVal Name As String, ByVal DirectoryName As String)
        Me.Name = Name
        Me.DirectoryName = DirectoryName
    End Sub

    Public Shared Function LoadSpecificationFile(ByVal TextFileName As String) As SpeechMaterialSpecification

        Dim FullFilePath As String = IO.Path.Combine(OstfBase.MediaRootDirectory, OstfBase.AvailableSpeechMaterialsSubFolder, TextFileName)

        If IO.File.Exists(FullFilePath) = False Then
            MsgBox("Unable to load the file " & FullFilePath, MsgBoxStyle.Critical, "Loading OSTF test specification file")
            Return Nothing
        End If

        Dim Input = IO.File.ReadAllLines(FullFilePath, Text.Encoding.UTF8)

        Dim FormatFlagDetected As Boolean = False

        Dim Name As String = ""
        Dim DirectoryName As String = ""
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

        Dim Output As New SpeechMaterialSpecification(Name, DirectoryName)
        Output.TestPresetsSubFilePath = TestPresetsSubFilePath

        Output.TestSpecificationFileName = TextFileName

        Return Output

    End Function



    Public Sub LoadAvailableMediaSetSpecifications()

        'Clears any test situations previously loaded before adding new ones
        MediaSets.Clear()

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
            Dim NewSituationTestSpecification = MediaSet.LoadMediaSetSpecification(Me, TextFilePath)
            If NewSituationTestSpecification IsNot Nothing Then

                'Adding the test situation
                MediaSets.Add(NewSituationTestSpecification)
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

End Class

