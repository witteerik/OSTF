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

Imports System.IO
Imports STFN.Core


Namespace Utils

    Public Class GeneralIO
        Inherits STFN.Core.Utils.GeneralIO


        ''' <summary>
        ''' Asks the user to supply one or more file paths by using an open file dialog box.
        ''' </summary>
        ''' <param name="directory">Optional initial directory.</param>
        ''' <param name="fileName">Optional suggested file name</param>
        ''' <param name="fileExtensions">Optional possible extensions</param>
        ''' <param name="BoxTitle">The message/title on the file dialog box</param>
        ''' <returns>Returns the file path, or nothing if a file path could not be created.</returns>
        Public Shared Function GetOpenFilePaths(Optional directory As String = "",
                                    Optional fileExtensions() As String = Nothing,
                                    Optional BoxTitle As String = "",
                                    Optional ReturnEmptyStringArrayOnCancel As Boolean = False) As String()

            Return Messager.GetOpenFilePaths(directory, fileExtensions, BoxTitle, ReturnEmptyStringArrayOnCancel).Result

            'The OstfGui.GetOpenFilePaths should do the following:

            '            Dim filePaths As String() = {}

            'SavingFile: Dim ofd As New OpenFileDialog

            '            'Enables multi select
            '            ofd.Multiselect = True
            '            ofd.CheckFileExists = True
            '            ofd.CheckPathExists = True

            '            'Creating a filterstring
            '            If fileExtensions IsNot Nothing Then
            '                Dim filter As String = ""
            '                For ext = 0 To fileExtensions.Length - 1
            '                    filter &= fileExtensions(ext).Trim(".") & " files (*." & fileExtensions(ext).Trim(".") & ")|*." & fileExtensions(ext).Trim(".") & "|"
            '                Next
            '                filter = filter.TrimEnd("|")
            '                ofd.Filter = filter
            '            End If

            '            If Not directory = "" Then ofd.InitialDirectory = directory
            '            If Not fileName = "" Then ofd.FileName = fileName
            '            If Not BoxTitle = "" Then ofd.Title = BoxTitle

            '            Dim result As DialogResult = ofd.ShowDialog()
            '            If result = DialogResult.OK Then
            '                filePaths = ofd.FileNames
            '                Return filePaths
            '            Else
            '                'Returns en empty string if cancel was pressed and ReturnEmptyStringOnCancel = True 
            '                If ReturnEmptyStringArrayOnCancel = True Then Return {}

            '                Dim boxResult As MsgBoxResult = MsgBox("An error occurred choosing file name.", MsgBoxStyle.RetryCancel, "Warning!")
            '                If boxResult = MsgBoxResult.Retry Then
            '                    GoTo SavingFile
            '                Else
            '                    Return Nothing
            '                End If
            '            End If

        End Function

        Public Shared Function GetFilesIncludingAllSubdirectories(ByVal Directory As String) As String()

            Dim FileList As New List(Of String)
            AddFiles(FileList, Directory)
            Dim Output() As String = FileList.ToArray
            Return Output

        End Function

        Private Shared Sub AddFiles(ByRef MyList As List(Of String), ByVal Directory As String)

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


        Public Shared Function GetFilesIncludingAllSubdirectories(ByVal Directory As String, ByVal SubDirectoryLevelsToInclude As Integer) As String()

            Dim CurrentSubDirectoryLevel As Integer = 0

            Dim FileList As New List(Of String)
            AddFiles(FileList, Directory, CurrentSubDirectoryLevel, SubDirectoryLevelsToInclude)
            Dim Output() As String = FileList.ToArray
            Return Output

        End Function

        Private Shared Sub AddFiles(ByRef MyList As List(Of String), ByVal Directory As String,
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
        ''' Compares two tab delimited files and detects any differences
        ''' </summary>
        ''' <param name="IgnoreFile2ColumnIndex"></param>
        Public Shared Sub CompareTwoTabDelimitedTxtFiles(Optional IgnoreFile2ColumnIndex As Integer? = Nothing)

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


        Public Shared Sub SaveDoubleArrayToFile(ByRef DoubleMatrix() As Double, Optional ByVal FilePath As String = "")

            If FilePath = "" Then FilePath = GetSaveFilePath()

            Dim SaveFolder As String = Path.GetDirectoryName(FilePath)
            If Not Directory.Exists(SaveFolder) Then Directory.CreateDirectory(SaveFolder)

            Dim writer As New IO.StreamWriter(FilePath)

            For Row_j = 0 To DoubleMatrix.Length - 1
                writer.WriteLine(DoubleMatrix(Row_j))
            Next

            writer.Close()

        End Sub

        Public Shared Sub SaveMatrixToFile(ByRef DoubleMatrix(,) As Double, Optional ByVal FilePath As String = "")

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
        Public Shared Sub SaveMatrixToFile(ByRef DoubleMatrix(,,) As Double, Optional ByVal FilePath As String = "", Optional ByVal ZdimensionToSave As Integer = 0)

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
        Public Shared Function ReadTxtFileToString(Optional filePath As String = "", Optional ByVal initialDirectory As String = "",
                                                   Optional ByVal dialogueBoxCommand As String = "Please select the file to load",
                                            Optional ByVal readType As TextReadType = TextReadType.readAllLines) As String()
            'Selecting the file path of the txt file, if not allready set
            If filePath = "" Then
                filePath = Messager.GetOpenFilePath(initialDirectory,,, dialogueBoxCommand).Result
                If filePath = "" Then Return Nothing
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

                Logging.SendInfoToLog(inputArray.Length & "lines from the file: " & filePath & " loaded into Array.")

                Return inputArray


            Catch ex As Exception
                MsgBox(ex.ToString)
                Return Nothing
            End Try

            Return Nothing

        End Function

        Public Shared Function SaveStringArrayToTxtFile(ByRef input() As String, ByRef saveDirectory As String, ByRef saveFileName As String, Optional AppendData As Boolean = True) As Boolean
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

        Public Shared Function SaveStringListArrayToTxtFile(ByRef input() As List(Of String), ByRef saveDirectory As String, ByRef saveFileName As String) As Boolean

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

        Public Shared Sub SaveListOfStringToTxtFile(ByRef InputList As List(Of String), Optional ByRef saveDirectory As String = "", Optional ByRef saveFileName As String = "ListOfStringOutput",
                                                Optional BoxTitle As String = "Choose location to store the List of String export file...")

            Try

                Logging.SendInfoToLog("Attempts to save List of string to .txt file.")

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

                Logging.SendInfoToLog("   List of String data were successfully saved to .txt file: " & filepath)

            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try

        End Sub

        Public Shared Function CompareBatchOfFiles(ByVal Folder1 As String, ByVal Folder2 As String, Optional Method As FileComparisonMethods = FileComparisonMethods.CompareWaveFileData, Optional ByVal ShowDifferences As Boolean = True) As Boolean

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

        Public Shared Function CompareFiles(ByVal FilePath1 As String, ByVal FilePath2 As String, Optional Method As FileComparisonMethods = FileComparisonMethods.CompareBytes, Optional ByVal ShowDifferences As Boolean = False) As Boolean

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

                    Dim Sound1 = STFN.Core.Audio.Sound.LoadWaveFile(FilePath1)
                    Dim Sound2 = STFN.Core.Audio.Sound.LoadWaveFile(FilePath2)

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



        Public Shared Function CopySubFolderContent(ByVal SourceFolder As String, ByVal TargetFolder As String,
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



        ''' <summary>
        ''' Merges (by row concatenation) the content of text files in the specified directory into a single text file.
        ''' </summary>
        ''' <param name="AddFileNameColumn">Adds the file name from which the data was added as a tab delimited first coumn on each row.</param>
        ''' <param name="SkipRows">Skips this number of rows in all files.</param>
        ''' <param name="Encoding">The encoding used to read the text files.</param>
        ''' <param name="HeadingLine">The zero based index of the column headings, read only from the first file. If left to -1, the add-heading-line functionality is ignored.</param>
        ''' <param name="Directory">The directory from which files should be read. If left empty, the directory in which the application file is located will be used.</param>
        ''' <returns></returns>
        Public Shared Function MergeTextFiles(ByVal AddFileNameColumn As Boolean, ByVal SkipRows As Integer, ByVal Encoding As Text.Encoding, Optional ByVal HeadingLine As Integer = -1, Optional ByVal Directory As String = "") As Boolean

            Try

                Dim TextFiles As New List(Of String)

                Dim AllIncludedTextLines As New List(Of String)

                If Directory = "" Then Directory = AppContext.BaseDirectory

                Dim AllTextFiles() As String = IO.Directory.GetFiles(Directory, "*.txt")

                Dim FileNameOfFirstFile As String = ""
                If AllTextFiles.Length > 0 Then FileNameOfFirstFile = IO.Path.GetFileNameWithoutExtension(AllTextFiles(0))

                For i = 0 To AllTextFiles.Length - 1

                    Dim FileLines = IO.File.ReadAllLines(AllTextFiles(i), Encoding)

                    For j = 0 To FileLines.Length - 1

                        'Adding heading line only from the first file and only if specified
                        If HeadingLine > -1 Then
                            If i = 0 Then
                                If j = HeadingLine Then
                                    If AddFileNameColumn = False Then
                                        AllIncludedTextLines.Add(FileLines(j))
                                    Else
                                        AllIncludedTextLines.Add("FileName" & vbTab & FileLines(j))
                                    End If
                                    Continue For
                                End If
                            End If
                        End If

                        'Skips rows in all non-initial files
                        If j < SkipRows Then Continue For

                        'Adding the row
                        If AddFileNameColumn = False Then
                            AllIncludedTextLines.Add(FileLines(j))
                        Else
                            AllIncludedTextLines.Add(IO.Path.GetFileNameWithoutExtension(AllTextFiles(i)) & vbTab & FileLines(j))
                        End If

                    Next
                Next

                If AllIncludedTextLines.Count > 0 Then

                    Dim OutputDirectory As String = Path.Combine(Directory, "MergedTextFiles")
                    Logging.SendInfoToLog(String.Join(vbCrLf, AllIncludedTextLines), "MergedTextFiles", OutputDirectory, True, False, True)

                End If

                Return True

            Catch ex As Exception
                Return False
            End Try

        End Function

    End Class

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





End Namespace