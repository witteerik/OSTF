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
Imports System.Threading

Namespace Utils
    Public Module Logging

        ''' <summary>
        ''' Can be used to block all logging, for example when run on a web-server. But rather than blocking from within Utils.SendInfoToLog, logging should be blocked
        ''' from the calling code.
        ''' </summary>
        Public GeneralLogIsActive As Boolean = True
        Public logFilePath As String = "C:\SpeechTestFrameworkLog\"
        Public showErrors As Boolean = True
        Public logErrors As Boolean = True
        Public logIsMultiThreadApplication As Boolean = False

        Public LoggingSpinLock As New Threading.SpinLock

        Public Sub SendInfoToLog(ByVal message As String,
                             Optional ByVal LogFileNameWithoutExtension As String = "",
                             Optional LogFileTemporaryPath As String = "",
                             Optional ByVal OmitDateInsideLog As Boolean = False,
                             Optional ByVal OmitDateInFileName As Boolean = False)
            'Optional ByRef SpinLock As Threading.SpinLock = Nothing)

            Dim SpinLockTaken As Boolean = False

            Try

                'Blocks logging if GeneralLogIsActive is False
                If GeneralLogIsActive = False Then Exit Sub

                'Attempts to enter a spin lock to avoid multiple thread conflicts when saving to the same file
                LoggingSpinLock.Enter(SpinLockTaken)

                If LogFileTemporaryPath = "" Then LogFileTemporaryPath = Utils.logFilePath

                Dim FileNameToUse As String = ""

                If OmitDateInFileName = False Then
                    If LogFileNameWithoutExtension = "" Then
                        FileNameToUse = "log-" & DateTime.Now.ToShortDateString.Replace("/", "-") & ".txt"
                    Else
                        FileNameToUse = LogFileNameWithoutExtension & "-" & DateTime.Now.ToShortDateString.Replace("/", "-") & ".txt"
                    End If
                Else
                    If LogFileNameWithoutExtension = "" Then
                        FileNameToUse = "log.txt"
                    Else
                        FileNameToUse = LogFileNameWithoutExtension & ".txt"
                    End If

                End If

                Dim OutputFilePath As String = Path.Combine(LogFileTemporaryPath, FileNameToUse)

                'Adds a thread ID if in multi thread app
                If logIsMultiThreadApplication = True Then
                    Dim TreadName As String = Thread.CurrentThread.ManagedThreadId
                    OutputFilePath &= "ThreadID_" & TreadName
                End If

                'Adds the file to AddFileToCopy for later copying of test results
                AddFileToCopy(OutputFilePath)

                Try
                    'If File.Exists(logFilePathway) Then File.Delete(logFilePathway)
                    If Not Directory.Exists(LogFileTemporaryPath) Then Directory.CreateDirectory(LogFileTemporaryPath)
                    Dim samplewriter As New StreamWriter(OutputFilePath, FileMode.Append)
                    If OmitDateInsideLog = False Then
                        samplewriter.WriteLine(DateTime.Now.ToString & vbCrLf & message)
                    Else
                        samplewriter.WriteLine(message)
                    End If
                    samplewriter.Close()

                Catch ex As Exception
                    Errors(ex.ToString, "Error saving to log file!")
                End Try

            Finally

                'Releases any spinlock
                If SpinLockTaken = True Then LoggingSpinLock.Exit()
            End Try

        End Sub

        Public Sub Errors(ByVal errorText As String, Optional ByVal errorTitle As String = "Error")

            If showErrors = True Then
                MsgBox(errorText, MsgBoxStyle.Critical, errorTitle)
            End If

            If logErrors = True Then
                Utils.SendInfoToLog("The following error occurred: " & vbCrLf & errorTitle & errorText, "Errors")
            End If

        End Sub


        Private FilesToCopy As New SortedSet(Of String)

        ''' <summary>
        ''' Add file paths that should later be copied to a specific folder using the public sub CopyFilesToFolder.
        ''' </summary>
        ''' <param name="FullFilePath"></param>
        Public Sub AddFileToCopy(ByVal FullFilePath As String)
            If Not FilesToCopy.Contains(FullFilePath) Then
                FilesToCopy.Add(FullFilePath)
            End If
        End Sub

        ''' <summary>
        ''' Copies files at the indicated file paths to the indicated Output folder. 
        ''' </summary>
        ''' <param name="OutputFolder"></param>
        ''' <param name="NewFilesToCopy">A list containing the file paths to copy. If left to nothing the files stored by using AddFileToCopy will be used instead.</param>
        '''<return>Returns True is all files were sucessfully copied, or if no output files existed, and False if one or more files were failed to be copied.</return>
        Public Function CopyFilesToFolder(ByRef OutputFolder As String, Optional ByRef NewFilesToCopy As SortedSet(Of String) = Nothing) As Boolean

            If NewFilesToCopy Is Nothing Then NewFilesToCopy = FilesToCopy
            If NewFilesToCopy Is Nothing Then Return True

            Dim FailedFiles As Integer = 0

            For Each CurrentFile In NewFilesToCopy

                Try
                    'Makes sure that the output folder exists
                    If Not Directory.Exists(OutputFolder) Then Directory.CreateDirectory(OutputFolder)

                    Dim OutputFilePath As String = Path.Combine(OutputFolder, Path.GetFileName(CurrentFile))

                    'Ensures that an old file with the same filename is not overwritten by adding a number to existing files
                    OutputFilePath = CheckFileNameConflict(OutputFilePath)

                    'Copies the file
                    File.Copy(CurrentFile, OutputFilePath)

                Catch ex As Exception
                    Utils.SendInfoToLog("Could not copy the file " & CurrentFile & " to the folder: " & OutputFolder & vbCrLf & ex.ToString)
                    FailedFiles += 1
                End Try
            Next

            If FailedFiles = 0 Then
                Return True
            Else
                Return False
            End If

        End Function

    End Module

End Namespace