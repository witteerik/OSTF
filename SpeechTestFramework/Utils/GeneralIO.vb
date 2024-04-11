'This software is available under the following license:
'MIT/X11 License
'
'Copyright (c) 2017 Erik Witte
'
'Permission is hereby granted, free of charge, to any person obtaining a copy
'of this software and associated documentation files (the ''Software''), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'copies of the Software, and to permit persons to whom the Software is
'furnished to do so, subject to the following conditions:
'
'The above copyright notice and this permission notice shall be included in all
'copies or substantial portions of the Software.
'
'THE SOFTWARE IS PROVIDED ''AS IS'', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
'SOFTWARE.

Imports System.Windows.Forms
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

Namespace Utils

    Public Module GeneralIO


        ''' <summary>
        ''' Asks the user to supply a file path by using a save file dialog box.
        ''' </summary>
        ''' <param name="directory">Optional initial directory.</param>
        ''' <param name="fileName">Optional suggested file name</param>
        ''' <param name="fileExtensions">Optional possible extensions</param>
        ''' <param name="BoxTitle">The message/title on the file dialog box</param>
        ''' <returns>Returns the file path, or nothing if a file path could not be created.</returns>
        Public Function GetSaveFilePath(Optional ByRef directory As String = "",
                                Optional ByRef fileName As String = "",
                                    Optional fileExtensions() As String = Nothing,
                                    Optional BoxTitle As String = "") As String

            Dim filePath As String = ""
            'Asks the user for a file path using the SaveFileDialog box.
SavingFile: Dim sfd As New SaveFileDialog

            'Creating a filterstring
            If fileExtensions IsNot Nothing Then
                Dim filter As String = ""
                For ext = 0 To fileExtensions.Length - 1

                    filter &= fileExtensions(ext).Trim(".") & " files (*." & fileExtensions(ext).Trim(".") & ")|*." & fileExtensions(ext).Trim(".") & "|"
                Next
                filter = filter.TrimEnd("|")
                sfd.Filter = filter
            End If

            If Not directory = "" Then sfd.InitialDirectory = directory
            If Not fileName = "" Then sfd.FileName = fileName
            If Not BoxTitle = "" Then sfd.Title = BoxTitle

            Dim result As DialogResult = sfd.ShowDialog()
            If result = DialogResult.OK Then
                filePath = sfd.FileName

                'Updats input variables to contain the selected
                directory = Path.GetDirectoryName(filePath)
                fileName = Path.GetFileName(filePath)

                Return filePath
            Else
                Dim errorSaving As MsgBoxResult = MsgBox("An error occurred choosing file name.", MsgBoxStyle.RetryCancel, "Warning!")
                If errorSaving = MsgBoxResult.Retry Then
                    GoTo SavingFile
                Else
                    Return Nothing
                End If
            End If

        End Function

        ''' <summary>
        ''' Asks the user to supply a file path by using an open file dialog box.
        ''' </summary>
        ''' <param name="directory">Optional initial directory.</param>
        ''' <param name="fileName">Optional suggested file name</param>
        ''' <param name="fileExtensions">Optional possible extensions</param>
        ''' <param name="BoxTitle">The message/title on the file dialog box</param>
        ''' <returns>Returns the file path, or nothing if a file path could not be created.</returns>
        Public Function GetOpenFilePath(Optional directory As String = "",
                                    Optional fileName As String = "",
                                    Optional fileExtensions() As String = Nothing,
                                    Optional BoxTitle As String = "",
                                    Optional ReturnEmptyStringOnCancel As Boolean = False) As String

            Dim filePath As String = ""

SavingFile: Dim ofd As New OpenFileDialog
            'Creating a filterstring
            If fileExtensions IsNot Nothing Then
                Dim filter As String = ""
                For ext = 0 To fileExtensions.Length - 1
                    filter &= fileExtensions(ext).Trim(".") & " files (*." & fileExtensions(ext).Trim(".") & ")|*." & fileExtensions(ext).Trim(".") & "|"
                Next
                filter = filter.TrimEnd("|")
                ofd.Filter = filter
            End If

            If Not directory = "" Then ofd.InitialDirectory = directory
            If Not fileName = "" Then ofd.FileName = fileName
            If Not BoxTitle = "" Then ofd.Title = BoxTitle

            Dim result As DialogResult = ofd.ShowDialog()
            If result = DialogResult.OK Then
                filePath = ofd.FileName
                Return filePath
            Else
                'Returns en empty string if cancel was pressed and ReturnEmptyStringOnCancel = True 
                If ReturnEmptyStringOnCancel = True Then Return ""

                Dim boxResult As MsgBoxResult = MsgBox("An error occurred choosing file name.", MsgBoxStyle.RetryCancel, "Warning!")
                If boxResult = MsgBoxResult.Retry Then
                    GoTo SavingFile
                Else
                    Return Nothing
                End If
            End If

        End Function

        Public Sub RemoveStringFromFileNames(ByVal StringToRemove As String, Optional ByVal InitialDirectory As String = "",
                                              Optional fileExtensions() As String = Nothing)

            Dim FileNames = GetOpenFilePaths(InitialDirectory, fileExtensions, "Select files to rename", True)

            'Exiting on cancel press
            If FileNames.Length = 0 Then Exit Sub

            For Each FilePath In FileNames

                'Creates a new name based on the old and the StringToRemove
                Dim NewName As String = IO.Path.GetFileName(FilePath).Replace(StringToRemove, "")

                'Skips renameing if the new name is ""
                If NewName = "" Or NewName.Length = 0 Then Continue For

                'Renames the file
                My.Computer.FileSystem.RenameFile(FilePath, NewName)

            Next

            MsgBox("Finished renaming the files!")

        End Sub

        Public Sub AddFileNamePrefix(ByVal StringToAdd As String, Optional ByVal InitialDirectory As String = "",
                                              Optional fileExtensions() As String = Nothing)

            Dim FileNames = GetOpenFilePaths(InitialDirectory, fileExtensions, "Select files to rename", True)

            'Exiting on cancel press
            If FileNames.Length = 0 Then Exit Sub

            For Each FilePath In FileNames

                'Creates a new name based on the old and the StringToRemove
                Dim NewName As String = StringToAdd & IO.Path.GetFileName(FilePath)

                'Skips renameing if the new name is ""
                If NewName = "" Or NewName.Length = 0 Then Continue For

                'Renames the file
                My.Computer.FileSystem.RenameFile(FilePath, NewName)

            Next

            MsgBox("Finished renaming the files!")

        End Sub

        ''' <summary>
        ''' Asks the user to supply one or more file paths by using an open file dialog box.
        ''' </summary>
        ''' <param name="directory">Optional initial directory.</param>
        ''' <param name="fileExtensions">Optional possible extensions</param>
        ''' <param name="BoxTitle">The message/title on the file dialog box</param>
        ''' <returns>Returns the file path, or nothing if a file path could not be created.</returns>
        Public Function GetOpenFilePaths(Optional directory As String = "",
                                    Optional fileExtensions() As String = Nothing,
                                    Optional BoxTitle As String = "",
                                    Optional ReturnEmptyStringArrayOnCancel As Boolean = False) As String()

            Dim filePaths As String() = {}

SavingFile: Dim ofd As New OpenFileDialog

            'Enables multi select
            ofd.Multiselect = True
            ofd.CheckFileExists = True
            ofd.CheckPathExists = True

            'Creating a filterstring
            If fileExtensions IsNot Nothing Then
                Dim filter As String = ""
                For ext = 0 To fileExtensions.Length - 1
                    filter &= fileExtensions(ext).Trim(".") & " files (*." & fileExtensions(ext).Trim(".") & ")|*." & fileExtensions(ext).Trim(".") & "|"
                Next
                filter = filter.TrimEnd("|")
                ofd.Filter = filter
            End If

            If Not directory = "" Then ofd.InitialDirectory = directory
            If Not BoxTitle = "" Then ofd.Title = BoxTitle

            Dim result As DialogResult = ofd.ShowDialog()
            If result = DialogResult.OK Then
                filePaths = ofd.FileNames
                Return filePaths
            Else
                'Returns en empty string if cancel was pressed and ReturnEmptyStringOnCancel = True 
                If ReturnEmptyStringArrayOnCancel = True Then Return {}

                Dim boxResult As MsgBoxResult = MsgBox("An error occurred choosing file name.", MsgBoxStyle.RetryCancel, "Warning!")
                If boxResult = MsgBoxResult.Retry Then
                    GoTo SavingFile
                Else
                    Return Nothing
                End If
            End If

        End Function

        Public Function GetFilesIncludingAllSubdirectories(ByVal Directory As String) As String()

            Dim FileList As New List(Of String)
            AddFiles(FileList, Directory)
            Dim Output() As String = FileList.ToArray
            Return Output

        End Function

        Private Sub AddFiles(ByRef MyList As List(Of String), ByVal Directory As String)

            'Adding the current level files
            Dim CurrentLevelFiles() As String = IO.Directory.GetFiles(Directory)
            For Each File In CurrentLevelFiles
                MyList.Add(File)
            Next

            'Getting current subdirectories
            Dim Subdirectories() As String = IO.Directory.GetDirectories(Directory)
            For Each Subdirectory In Subdirectories
                AddFiles(MyList, Subdirectory)
            Next

        End Sub


        Public Function GetFilesIncludingAllSubdirectories(ByVal Directory As String, ByVal SubDirectoryLevelsToInclude As Integer) As String()

            Dim CurrentSubDirectoryLevel As Integer = 0

            Dim FileList As New List(Of String)
            AddFiles(FileList, Directory, CurrentSubDirectoryLevel, SubDirectoryLevelsToInclude)
            Dim Output() As String = FileList.ToArray
            Return Output

        End Function

        Private Sub AddFiles(ByRef MyList As List(Of String), ByVal Directory As String,
                         ByRef CurrentSubDirectoryLevel As Integer, ByRef LowestSubDirectoryLevelToInclude As Integer)


            'Adding the current level files
            Dim CurrentLevelFiles() As String = IO.Directory.GetFiles(Directory)
            For Each File In CurrentLevelFiles
                MyList.Add(File)
            Next

            'We're going down one level
            CurrentSubDirectoryLevel += 1

            If CurrentSubDirectoryLevel > LowestSubDirectoryLevelToInclude Then
                'We're going back up to the level above
                CurrentSubDirectoryLevel -= 1
                Exit Sub
            End If

            'Getting current subdirectories
            Dim Subdirectories() As String = IO.Directory.GetDirectories(Directory)
            For Each Subdirectory In Subdirectories
                AddFiles(MyList, Subdirectory, CurrentSubDirectoryLevel, LowestSubDirectoryLevelToInclude)
            Next

        End Sub

        ''' <summary>
        ''' Checks if the input filename exists in the specified folder. If it doesn't exist, the input filename is returned, 
        ''' but if it already exists, a new numeral suffix is added to the file name. The file name extension is never changed.
        ''' </summary>
        ''' <returns></returns>
        Public Function CheckFileNameConflict(ByVal InputFilePath As String) As String

            Dim WorkingFilePath As String = InputFilePath

0:

            If File.Exists(WorkingFilePath) Then

                Dim Folder As String = Path.GetDirectoryName(WorkingFilePath)
                Dim FileNameExtension As String = Path.GetExtension(WorkingFilePath)
                Dim FileNameWithoutExtension As String = Path.GetFileNameWithoutExtension(WorkingFilePath)

                'Getting any numeric end, separated by a _, in the input file name
                Dim NumericEnd As String = ""
                Dim FileNameSplit() As String = FileNameWithoutExtension.Split("_")
                Dim NewFileNameWithoutNumericString As String = ""
                If Not IsNumeric(FileNameSplit(FileNameSplit.Length - 1)) Then

                    'Creates a new WorkingFilePath with a numeric suffix, and checks if it also exists
                    WorkingFilePath = Path.Combine(Folder, FileNameWithoutExtension & "_000" & FileNameExtension)

                    'Checking the new file name
                    GoTo 0

                Else

                    'Creates a new WorkingFilePath with a iterated numeric suffix, and checks if it also exists by restarting at 0

                    'Reads the current numeric value, stored after the last _
                    Dim NumericValue As Integer = CInt(FileNameSplit(FileNameSplit.Length - 1))

                    'Increases the value of the numeric suffix by 1
                    Dim NewNumericString As String = (NumericValue + 1).ToString("000")

                    'Creates a new WorkingFilePath with the increased numeric suffix, and checks if it also exists by restarting at 0
                    If FileNameSplit.Length > 1 Then
                        For n = 0 To FileNameSplit.Length - 2
                            NewFileNameWithoutNumericString &= FileNameSplit(n)
                        Next
                    End If
                    WorkingFilePath = Path.Combine(Folder, NewFileNameWithoutNumericString & "_" & NewNumericString & FileNameExtension)

                    'Checking the new file name
                    GoTo 0
                End If

            Else
                Return WorkingFilePath
            End If

        End Function

        ''' <summary>
        ''' Compares two tab delimited files and detects any differences
        ''' </summary>
        ''' <param name="IgnoreFile2ColumnIndex"></param>
        Public Sub CompareTwoTabDelimitedTxtFiles(Optional IgnoreFile2ColumnIndex As Integer? = Nothing)

            Dim FilePath1 As String = GetOpenFilePath(,,, "Select file 1")
            Dim FilePath2 As String = GetOpenFilePath(,,, "Select file 2")

            Dim inputArray1() As String = System.IO.File.ReadAllLines(FilePath1)
            Dim inputArray2() As String = System.IO.File.ReadAllLines(FilePath2)

            If inputArray1.Length <> inputArray2.Length Then
                MsgBox("The files have different number of lines." & vbCrLf &
                                                                "File 1 has " & inputArray1.Length & " lines, and" & vbCrLf &
                                                                "File 2 has " & inputArray2.Length & " lines.")
                MsgBox("The file comparison is now terminated.")
                Exit Sub
            End If

            Dim FilesAreDifferent As Boolean = False

            For line = 0 To inputArray1.Length - 1

                Dim File1LineSplit() As String = inputArray1(line).Split(vbTab)
                Dim File2LineSplit() As String = inputArray2(line).Split(vbTab)

                If IgnoreFile2ColumnIndex IsNot Nothing And IgnoreFile2ColumnIndex <= File1LineSplit.Length - 1 Then 'TODO: This line may be wrong!?! What happens when IgnoreFile2ColumnIndex is equal to File1LineSplit.Length ? Hasn't been tested.
                    If File1LineSplit.Length <> File2LineSplit.Length - 1 Then
                        MsgBox("The row count differs on line " & line & ":" & vbCrLf & vbCrLf &
                           "File 1 has " & File1LineSplit.Length & " columns, and" & vbCrLf &
                           "File 2 has " & File2LineSplit.Length & " columns (not counting column " & IgnoreFile2ColumnIndex & ").")
                        MsgBox("The file comparison is now terminated.")
                        Exit Sub
                    End If
                Else
                    If File1LineSplit.Length <> File2LineSplit.Length Then
                        MsgBox("The row count differs on line " & line & ":" & vbCrLf & vbCrLf &
                           "File 1 has " & File1LineSplit.Length & " columns, and" & vbCrLf &
                           "File 2 has " & File2LineSplit.Length & " columns.")
                        MsgBox("The file comparison is now terminated.")
                        Exit Sub
                    End If
                End If

                Dim File1TestColumnIndex As Integer = 0
                Dim File2TestColumnIndex As Integer = 0

                For column = 0 To File1LineSplit.Length - 1
                    If File1LineSplit(File1TestColumnIndex) <> File2LineSplit(File2TestColumnIndex) Then FilesAreDifferent = True

                    File1TestColumnIndex += 1
                    If IgnoreFile2ColumnIndex IsNot Nothing Then
                        If File2TestColumnIndex + 1 = IgnoreFile2ColumnIndex Then
                            File2TestColumnIndex += 2
                        Else
                            File2TestColumnIndex += 1
                        End If
                    Else
                        File2TestColumnIndex += 1
                    End If

                    If FilesAreDifferent = True Then
                        MsgBox("Files are different on line " & line & vbCrLf & vbCrLf &
                           "File 1: " & inputArray1(line) & vbCrLf & vbCrLf &
                           "File 2: " & inputArray2(line))
                        MsgBox("The file comparison is now terminated.")
                        Exit Sub
                    End If
                Next
            Next

            If IgnoreFile2ColumnIndex IsNot Nothing Then
                MsgBox("The two files are identical on all " & inputArray1.Length & " lines, if column " & IgnoreFile2ColumnIndex & " is ignored in file 2.")
            Else
                MsgBox("The two files are identical on all " & inputArray1.Length & " lines.")
            End If

        End Sub


        Public Sub SaveDoubleArrayToFile(ByRef DoubleMatrix() As Double, Optional ByVal FilePath As String = "")

            If FilePath = "" Then FilePath = GetSaveFilePath()

            Dim SaveFolder As String = Path.GetDirectoryName(FilePath)
            If Not Directory.Exists(SaveFolder) Then Directory.CreateDirectory(SaveFolder)

            Dim writer As New IO.StreamWriter(FilePath)

            For Row_j = 0 To DoubleMatrix.Length - 1
                writer.WriteLine(DoubleMatrix(Row_j))
            Next

            writer.Close()

        End Sub

        Public Sub SaveMatrixToFile(ByRef DoubleMatrix(,) As Double, Optional ByVal FilePath As String = "")

            If FilePath = "" Then FilePath = GetSaveFilePath()

            Dim SaveFolder As String = Path.GetDirectoryName(FilePath)
            If Not Directory.Exists(SaveFolder) Then Directory.CreateDirectory(SaveFolder)

            Dim writer As New IO.StreamWriter(FilePath)

            For Row_j = 0 To DoubleMatrix.GetUpperBound(1)

                Dim CurrentRow As New List(Of String)
                'Dim CurrentRow As String = ""

                For Column_i = 0 To DoubleMatrix.GetUpperBound(0)

                    CurrentRow.Add(DoubleMatrix(Column_i, Row_j))
                    'CurrentRow &= DoubleMatrix(Column_i, Row_j) & vbTab

                Next

                'writer.WriteLine(CurrentRow)
                writer.WriteLine(String.Join(vbTab, CurrentRow))

            Next

            writer.Close()

        End Sub

        ''' <summary>
        ''' Saves the first contents of a matrix to file.
        ''' </summary>
        ''' <param name="DoubleMatrix"></param>
        ''' <param name="FilePath"></param>
        Public Sub SaveMatrixToFile(ByRef DoubleMatrix(,,) As Double, Optional ByVal FilePath As String = "", Optional ByVal ZdimensionToSave As Integer = 0)

            If FilePath = "" Then FilePath = GetSaveFilePath()

            Dim SaveFolder As String = Path.GetDirectoryName(FilePath)
            If Not Directory.Exists(SaveFolder) Then Directory.CreateDirectory(SaveFolder)

            Dim writer As New IO.StreamWriter(FilePath)

            For Row_j = 0 To DoubleMatrix.GetUpperBound(1)

                Dim CurrentRow As String = ""

                For Column_i = 0 To DoubleMatrix.GetUpperBound(0)

                    CurrentRow &= DoubleMatrix(Column_i, Row_j, ZdimensionToSave) & vbTab

                Next

                writer.WriteLine(CurrentRow)

            Next

            writer.Close()

        End Sub

        'Moved from Speechmaterials
        Public Enum TextReadType
            readAllLines
            readAllText
        End Enum

        ''' <summary>
        ''' Opens a txt file and saves it to an array of string
        ''' </summary>
        ''' <param name="filePath">The file path to a text file.</param>
        ''' <param name="initialDirectory">The directory that the open file dialogue box starts on if a file path is not set.</param>
        ''' <param name="dialogueBoxCommand">The title of the open file dialogue box starts on if a file path is not set.</param>
        ''' <returns>Returns a string array</returns>
        Public Function ReadTxtFileToString(Optional filePath As String = "", Optional ByVal initialDirectory As String = "",
                                                   Optional ByVal dialogueBoxCommand As String = "Please select the file to load",
                                            Optional ByVal readType As TextReadType = TextReadType.readAllLines) As String()
            'Selecting the file path of the txt file, if not allready set
            If filePath = "" Then
                Dim ofd As New OpenFileDialog
                If initialDirectory = "" Then initialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
                ofd.InitialDirectory = initialDirectory '"C: \Users\ewi024\Documents\Doktorandstudier\Talmaterial\NST_mod"
                ofd.Title = dialogueBoxCommand
                If ofd.ShowDialog = DialogResult.OK Then
                    filePath = ofd.FileName
                Else
                    Return Nothing
                End If
            End If

            'Trying to read the txt-file
            Try
                Dim inputArray() As String = {}
                Select Case readType
                    Case TextReadType.readAllLines
                        inputArray = System.IO.File.ReadAllLines(filePath)

                    Case TextReadType.readAllText
                        Dim InputString As String = System.IO.File.ReadAllText(filePath)
                        inputArray = InputString.Split(vbLf)

                End Select

                Utils.SendInfoToLog(inputArray.Length & "lines from the file: " & filePath & " loaded into Array.")

                Return inputArray


            Catch ex As Exception
                MsgBox(ex.ToString)
                Return Nothing
            End Try

            Return Nothing

        End Function

        Public Function SaveStringArrayToTxtFile(ByRef input() As String, ByRef saveDirectory As String, ByRef saveFileName As String, Optional AppendData As Boolean = True) As Boolean
            Try
                If Not saveDirectory.Substring(saveDirectory.Length - 1) = "\" Then saveDirectory = saveDirectory & "\"
                If Not Directory.Exists(saveDirectory) Then Directory.CreateDirectory(saveDirectory)

                Dim writer As New StreamWriter(saveDirectory & saveFileName & ".txt", AppendData, Text.Encoding.UTF8)

                For index = 0 To input.Length - 1
                    writer.WriteLine(input(index))
                Next

                writer.Close()
                Return True
            Catch ex As Exception
                MsgBox(ex.ToString)
                Return False
            End Try
        End Function

        Public Function SaveStringListArrayToTxtFile(ByRef input() As List(Of String), ByRef saveDirectory As String, ByRef saveFileName As String) As Boolean

            Try
                If Not saveDirectory.Substring(saveDirectory.Length - 1) = "\" Then saveDirectory = saveDirectory & "\"
                If Not Directory.Exists(saveDirectory) Then Directory.CreateDirectory(saveDirectory)

                Dim writer As New StreamWriter(saveDirectory & saveFileName & ".txt", True, Text.Encoding.UTF8)

                For index = 0 To input.Length - 1
                    Dim row As String = ""
                    For n = 0 To input(index).Count - 1
                        row = row & input(index)(n) & " "
                    Next
                    row = row.TrimEnd(" ")
                    writer.WriteLine(row)
                Next

                writer.Close()
                Return True
            Catch ex As Exception
                MsgBox(ex.ToString)
                Return False
            End Try

        End Function

        Public Sub SaveListOfStringToTxtFile(ByRef InputList As List(Of String), Optional ByRef saveDirectory As String = "", Optional ByRef saveFileName As String = "ListOfStringOutput",
                                                Optional BoxTitle As String = "Choose location to store the List of String export file...")

            Try

                Utils.SendInfoToLog("Attempts to save List of string to .txt file.")

                'Choosing file location
                Dim filepath As String = ""
                'Ask the user for file path if not incomplete file path is given
                If saveDirectory = "" Or saveFileName = "" Then
                    filepath = GetSaveFilePath(saveDirectory, saveFileName, {"txt"}, BoxTitle)
                Else
                    filepath = Path.Combine(saveDirectory, saveFileName & ".txt")
                    If Not Directory.Exists(Path.GetDirectoryName(filepath)) Then Directory.CreateDirectory(Path.GetDirectoryName(filepath))
                End If

                'Save it to file
                Dim writer As New StreamWriter(filepath, False, Text.Encoding.UTF8)

                For Each CurrentItem In InputList
                    writer.Write(CurrentItem)
                Next

                writer.Close()

                Utils.SendInfoToLog("   List of String data were successfully saved to .txt file: " & filepath)

            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try

        End Sub

        Public Function CompareBatchOfFiles(ByVal Folder1 As String, ByVal Folder2 As String, Optional Method As FileComparisonMethods = FileComparisonMethods.CompareWaveFileData, Optional ByVal ShowDifferences As Boolean = True) As Boolean

            Dim Files1 = IO.Directory.GetFiles(Folder1)
            Dim Files2 = IO.Directory.GetFiles(Folder2)

            If Files1.Length <> Files2.Length Then Return False

            For n = 0 To Files1.Length - 1
                If IO.Path.GetFileName(Files1(n)) <> IO.Path.GetFileName(Files2(n)) Then Return False
                If CompareFiles(Files1(n), Files2(n), Method, ShowDifferences) = False Then Return False

                Console.WriteLine("Comparing file: " & n + 1 & " " & IO.Path.GetFileName(Files1(n)))

            Next

            Return True

        End Function

        Public Enum FileComparisonMethods
            CompareBytes
            CompareWaveFileData
        End Enum

        Public Function CompareFiles(ByVal FilePath1 As String, ByVal FilePath2 As String, Optional Method As FileComparisonMethods = FileComparisonMethods.CompareBytes, Optional ByVal ShowDifferences As Boolean = False) As Boolean

            Select Case Method
                Case FileComparisonMethods.CompareBytes

                    'Checks if its the same file, then they must be identical
                    If FilePath1 = FilePath2 Then Return True

                    'Reads the files
                    Dim InputFileStream1 As FileStream = New FileStream(FilePath1, FileMode.Open)
                    Dim InputFileStream2 As FileStream = New FileStream(FilePath2, FileMode.Open)

                    'Returns false if the streams are of unequal length
                    If InputFileStream1.Length <> InputFileStream2.Length Then Return False

                    'Checks that each byte in the stream are the same
                    For n = 0 To InputFileStream1.Length - 1
                        If InputFileStream1.ReadByte() <> InputFileStream2.ReadByte() Then
                            If ShowDifferences = True Then MsgBox("Detected difference at byte: " & n & " between the following files:" & vbCrLf & vbCrLf & FilePath1 & vbCrLf & FilePath2)
                            Return False
                        End If
                    Next

                    'Returns true if no differences were found
                    Return True

                Case FileComparisonMethods.CompareWaveFileData

                    'Checks if its the same file, then they must be identical
                    If FilePath1 = FilePath2 Then Return True

                    Dim Sound1 = Audio.Sound.LoadWaveFile(FilePath1)
                    Dim Sound2 = Audio.Sound.LoadWaveFile(FilePath2)

                    If Sound1.WaveFormat.Channels <> Sound2.WaveFormat.Channels Then Return False

                    For c = 1 To Sound1.WaveFormat.Channels

                        If Sound1.WaveData.SampleData.Length <> Sound2.WaveData.SampleData.Length Then Return False
                        For s = 0 To Sound1.WaveData.SampleData.Length - 1
                            If Sound1.WaveData.SampleData(c)(s) <> Sound2.WaveData.SampleData(c)(s) Then
                                Return False
                            End If
                        Next
                    Next

                    'Returns true if no differences were found
                    Return True

                Case Else
                    Throw New NotImplementedException
            End Select


        End Function



        Public Function CopySubFolderContent(ByVal SourceFolder As String, ByVal TargetFolder As String,
                                         ByVal MaxNumberOfFilesPerSubFolder As Integer?,
                                         Optional ByVal ExcludeIfFileNameContains As String = "") As Boolean

            Try
                Dim Directories = Directory.GetDirectories(SourceFolder)

                For Each CurrentSourceSubFolder In Directories

                    Dim SourceDirectorySplit As String() = CurrentSourceSubFolder.Split(Path.DirectorySeparatorChar)
                    Dim SourceSubDirectoryName As String = SourceDirectorySplit(SourceDirectorySplit.Length - 1)
                    Dim CurrentTargetSubFolder As String = Path.Combine(TargetFolder, SourceSubDirectoryName)

                    If Not Directory.Exists(CurrentSourceSubFolder) Then
                        MsgBox("Cannot find the directory " & CurrentSourceSubFolder)
                        Return False
                    End If

                    Dim ReadPaths = Directory.GetFiles(CurrentSourceSubFolder, "*", SearchOption.TopDirectoryOnly).ToList

                    If ExcludeIfFileNameContains <> "" Then
                        Dim TempPaths As New List(Of String)
                        For Each Path In ReadPaths
                            If IO.Path.GetFileNameWithoutExtension(Path).Contains(ExcludeIfFileNameContains) Then Continue For

                            TempPaths.Add(Path)
                        Next
                        ReadPaths = TempPaths
                    End If

                    ReadPaths.Sort()

                    Directory.CreateDirectory(CurrentTargetSubFolder)

                    Dim NumberOfFilesToCopy As Integer = ReadPaths.Count
                    If MaxNumberOfFilesPerSubFolder IsNot Nothing Then
                        NumberOfFilesToCopy = System.Math.Min(MaxNumberOfFilesPerSubFolder.Value, ReadPaths.Count)
                    End If

                    For n = 0 To NumberOfFilesToCopy - 1
                        Dim OutputPath As String = Path.Combine(CurrentTargetSubFolder, Path.GetFileName(ReadPaths(n)))
                        File.Copy(ReadPaths(n), OutputPath)
                    Next

                Next

                Return True

            Catch ex As Exception
                MsgBox("The following exception occured:" & vbCrLf & ex.ToString)
                Return False
            End Try

        End Function


        Public Sub ReplaceCharsInFileSystemEntries(ByVal StartDirectory As String, Optional ByVal ReplacementList As SortedList(Of String, String) = Nothing)
            If ReplacementList Is Nothing Then
                ' Initialize the ReplacementList if not provided
                ReplacementList = New SortedList(Of String, String)
                ReplacementList.Add("å", "aa")
                ReplacementList.Add("ä", "ae")
                ReplacementList.Add("ö", "oe")
            End If

            ' Replace characters in the StartDirectory
            ReplaceCharsInDirectory(StartDirectory, ReplacementList)
        End Sub

        Private Sub ReplaceCharsInDirectory(ByVal directoryPath As String, ByVal replacementList As SortedList(Of String, String))
            ' Process files in the current directory
            For Each filePath As String In Directory.GetFiles(directoryPath)
                Dim fileName As String = Path.GetFileName(filePath)
                Dim updatedFileName As String = ReplaceCharacters(fileName, replacementList)
                If fileName <> updatedFileName Then
                    Dim newFilePath As String = Path.Combine(directoryPath, updatedFileName)
                    File.Move(filePath, newFilePath)
                End If
            Next

            ' Process subdirectories recursively
            For Each subdirectoryPath As String In Directory.GetDirectories(directoryPath)
                Dim updatedSubdirectoryPath As String = ReplaceCharacters(subdirectoryPath, replacementList)
                If subdirectoryPath <> updatedSubdirectoryPath Then
                    Directory.Move(subdirectoryPath, updatedSubdirectoryPath)
                End If

                ' Recursive call to process subdirectory
                ReplaceCharsInDirectory(updatedSubdirectoryPath, replacementList)
            Next
        End Sub

        Private Function ReplaceCharacters(ByVal input As String, ByVal replacementList As SortedList(Of String, String)) As String
            Dim output As String = input
            For Each kvp In replacementList
                output = output.Replace(kvp.Key, kvp.Value)
            Next
            Return output
        End Function


        Public Function NormalizeCrossPlatformPath(ByVal InputPath As String) As String

            Dim InputHasInitialDoubleBackslash As Boolean = False
            If InputPath.StartsWith("\\") Then InputHasInitialDoubleBackslash = True

            InputPath = InputPath.Replace("\", "/")
            InputPath = InputPath.Replace("//", "/")
            InputPath = InputPath.Replace("///", "/")

            InputPath = IO.Path.Combine(InputPath.Split("/"))

            If InputHasInitialDoubleBackslash = True Then
                'Reinserting the initial double backslash
                InputPath = "\\" & InputPath
            End If

            Return InputPath

        End Function

    End Module


    Public Class Utf8ToByteStringConverter

        ''' <summary>
        ''' Converts an UTF8 string to a numeric byte string array that can be converted back to the original UTF8 string by the function ConvertByteStringToUtf8String
        ''' </summary>
        ''' <param name="UTF8String"></param>
        ''' <returns></returns>
        Public Function ConvertUtf8StringToByteString(ByVal UTF8String As String) As String
            Dim Bytes = System.Text.Encoding.UTF8.GetBytes(UTF8String)
            Return String.Join("_", Bytes)
        End Function

        ''' <summary>
        ''' Converts an numeric byte string array created by ConvertUtf8StringToByteString to a UTF8 string 
        ''' </summary>
        ''' <param name="ByteString"></param>
        ''' <returns></returns>
        Public Function ConvertByteStringToUtf8String(ByVal ByteString As String) As String

            Dim NewStringSplit = ByteString.Split("_")
            Dim NewBytesArray(NewStringSplit.Length - 1) As Byte
            For n = 0 To NewStringSplit.Length - 1
                NewBytesArray(n) = Byte.Parse(NewStringSplit(n))
            Next
            Return System.Text.Encoding.UTF8.GetString(NewBytesArray)

        End Function

    End Class


    <Serializable>
    Public Class ObjectChangeDetector
        Private UnchangedState() As Byte = {}

        Private Property TargetObject As Object

        Public Sub New(ByRef TargetObject As Object)
            Me.TargetObject = TargetObject
        End Sub


        ''' <summary>
        ''' Determines if the object has changed since it was read from file by comparing serialized versions.
        ''' </summary>
        Public Function IsChanged() As Boolean

            Dim serializedMe As New MemoryStream
            Dim serializer As New BinaryFormatter
            serializer.Serialize(serializedMe, TargetObject)
            Dim MeAsByteArray = serializedMe.ToArray()

            If MeAsByteArray.SequenceEqual(UnchangedState) = True Then
                Return False
            Else
                Return True
            End If

        End Function

        Public Sub SetUnchangedState()

            Dim serializedMe As New MemoryStream
            Dim serializer As New BinaryFormatter
            serializer.Serialize(serializedMe, TargetObject)
            UnchangedState = serializedMe.ToArray()

        End Sub

    End Class

End Namespace