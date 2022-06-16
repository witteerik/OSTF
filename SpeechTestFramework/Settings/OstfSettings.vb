Public Module OstfSettings

    ' Program location
    Public Property RootDirectory As String = "C:\OSTF" 'Indicates the root path. Other paths given in the project setting files are relative (subpaths) to this path only if they begin with .\ otherwise they are taken as absolute paths.

    Public Property AvailableTestsSubFolder As String = "AvailableTests"

    Public Property AvailableTests As New List(Of TestSpecification)


    Public Sub LoadAvailableTestSpecifications()

        Dim TestSpecificationFolder As String = IO.Path.Combine(RootDirectory, AvailableTestsSubFolder)

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
