Imports System.IO
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace Utils

    Public Class GeneralIO

        ''' <summary>
        ''' Asks the user to supply a file path by using a save file dialog box.
        ''' </summary>
        ''' <param name="directory">Optional initial directory.</param>
        ''' <param name="fileName">Optional suggested file name</param>
        ''' <param name="fileExtensions">Optional possible extensions</param>
        ''' <param name="BoxTitle">The message/title on the file dialog box</param>
        ''' <returns>Returns the file path, or nothing if a file path could not be created.</returns>
        Public Shared Function GetSaveFilePath(Optional ByRef directory As String = "",
                                Optional ByRef fileName As String = "",
                                    Optional fileExtensions() As String = Nothing,
                                    Optional BoxTitle As String = "") As String

            Return Messager.GetSaveFilePath(directory, fileName, fileExtensions, BoxTitle).Result

        End Function

        ''' <summary>
        ''' Asks the user to supply a file path by using an open file dialog box.
        ''' </summary>
        ''' <param name="directory">Optional initial directory.</param>
        ''' <param name="fileName">Optional suggested file name</param>
        ''' <param name="fileExtensions">Optional possible extensions</param>
        ''' <param name="BoxTitle">The message/title on the file dialog box</param>
        ''' <returns>Returns the file path, or nothing if a file path could not be created.</returns>
        Public Shared Function GetOpenFilePath(Optional directory As String = "",
                                    Optional fileName As String = "",
                                    Optional fileExtensions() As String = Nothing,
                                    Optional BoxTitle As String = "",
                                    Optional ReturnEmptyStringOnCancel As Boolean = False) As String

            Return Messager.GetOpenFilePath(directory, fileName, fileExtensions, BoxTitle, ReturnEmptyStringOnCancel).Result

        End Function



        Public Shared Function NormalizeCrossPlatformPath(ByVal InputPath As String) As String

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

        ''' <summary>
        ''' Checks if the input filename exists in the specified folder. If it doesn't exist, the input filename is returned, 
        ''' but if it already exists, a new numeral suffix is added to the file name. The file name extension is never changed.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function CheckFileNameConflict(ByVal InputFilePath As String) As String

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
                Dim serializer As New XmlSerializer(GetType(ObjectChangeDetector))
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
                Dim serializer As New XmlSerializer(GetType(ObjectChangeDetector))
                serializer.Serialize(serializedMe, TargetObject)
                UnchangedState = serializedMe.ToArray()

            End Sub

        End Class

    End Class

End Namespace