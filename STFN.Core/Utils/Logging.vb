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

Partial Public Class Logging

    ''' <summary>
    ''' Can be used to block all logging, for example when run on a web-server. But rather than blocking from within Utils.SendInfoToLog, logging should be blocked
    ''' from the calling code.
    ''' </summary>
    Public Shared GeneralLogIsActive As Boolean = True
    Public Shared LogFileDirectory As String = "C:\SpeechTestFrameworkLog\"
    Public Shared ShowErrors As Boolean = True
    Public Shared LogErrors As Boolean = True
    Public Shared LogIsMultiThreadApplication As Boolean = False

    Private Shared LoggingSpinLock As New Threading.SpinLock

    Public Shared Sub SendInfoToLog(ByVal message As String,
                             Optional ByVal LogFileNameWithoutExtension As String = "",
                             Optional LogFileTemporaryPath As String = "",
                             Optional ByVal OmitDateInsideLog As Boolean = False,
                             Optional ByVal OmitDateInFileName As Boolean = False,
                             Optional ByVal OverWrite As Boolean = False,
                             Optional ByVal AddDateOnEveryRow As Boolean = False,
                             Optional ByVal SkipIfEmpty As Boolean = False)

        'Skipping directly if message is empty
        If SkipIfEmpty = True Then
            If message = "" Then Exit Sub
        End If

        Dim SpinLockTaken As Boolean = False

        'Skipping logging if the demo participant ID is set
        If CurrentParticipantID = NoTestId Then Exit Sub

        Try

            'Blocks logging if GeneralLogIsActive is False
            If GeneralLogIsActive = False Then Exit Sub

            'Attempts to enter a spin lock to avoid multiple thread conflicts when saving to the same file
            LoggingSpinLock.Enter(SpinLockTaken)

            If LogFileTemporaryPath = "" Then LogFileTemporaryPath = LogFileDirectory

            Dim FileNameToUse As String = ""

            If OmitDateInFileName = False Then
                If LogFileNameWithoutExtension = "" Then
                    FileNameToUse = "log-" & CreateDateTimeStringForFileNames() & ".txt"
                Else
                    FileNameToUse = LogFileNameWithoutExtension & "-" & CreateDateTimeStringForFileNames() & ".txt"
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
            If LogIsMultiThreadApplication = True Then
                Dim TreadName As String = Thread.CurrentThread.ManagedThreadId
                OutputFilePath &= "ThreadID_" & TreadName
            End If

            'Adds the file to AddFileToCopy for later copying of test results
            AddFileToCopy(OutputFilePath)

            Try
                'If File.Exists(logFilePathway) Then File.Delete(logFilePathway)
                If Not Directory.Exists(LogFileTemporaryPath) Then Directory.CreateDirectory(LogFileTemporaryPath)

                If OverWrite = True Then
                    'Deleting the files before writing if overwrite is true
                    If IO.File.Exists(OutputFilePath) Then IO.File.Delete(OutputFilePath)
                End If

                Dim Writer As New StreamWriter(OutputFilePath, FileMode.Append)
                If OmitDateInsideLog = False Then
                    If AddDateOnEveryRow = False Then
                        Writer.WriteLine(DateTime.Now.ToString & vbCrLf & message)
                    Else

                        Dim lineBreaks As String() = {vbCr, vbCrLf, vbLf}
                        Dim MessageSplit = message.Split(lineBreaks, StringSplitOptions.None)
                        Dim MessageRowsWithDate As New List(Of String)
                        For Each MessageRow In MessageSplit
                            If SkipIfEmpty = True And MessageRow = "" Then Continue For
                            MessageRowsWithDate.Add(DateTime.Now.ToString & vbTab & MessageRow)
                        Next
                        Writer.WriteLine(String.Join(vbCrLf, MessageRowsWithDate))

                    End If
                Else
                    Writer.WriteLine(message)
                End If
                Writer.Close()

            Catch ex As Exception
                Errors(ex.ToString, "Error saving to log file!")
            End Try

        Finally

            'Releases any spinlock
            If SpinLockTaken = True Then LoggingSpinLock.Exit()
        End Try

    End Sub

    ''' <summary>
    ''' Creates a string containing year month day hours minutes seconds and milliseconds sutiable for use in filenames.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function CreateDateTimeStringForFileNames() As String
        Return DateTime.Now.ToString("yyyyMMdd_HHmmss_fff")
    End Function

    Public Shared Sub Errors(ByVal errorText As String, Optional ByVal errorTitle As String = "Error")

        If ShowErrors = True Then
            MsgBox(errorText, MsgBoxStyle.Critical, errorTitle)
        End If

        If LogErrors = True Then
            SendInfoToLog("The following error occurred: " & vbCrLf & errorTitle & errorText, "Errors")
        End If

    End Sub

    Public Shared FilesToCopy As New SortedSet(Of String)

    ''' <summary>
    ''' Add file paths that should later be copied to a specific folder using the public sub CopyFilesToFolder.
    ''' </summary>
    ''' <param name="FullFilePath"></param>
    Public Shared Sub AddFileToCopy(ByVal FullFilePath As String)
        If Not FilesToCopy.Contains(FullFilePath) Then
            FilesToCopy.Add(FullFilePath)
        End If
    End Sub

    Private Shared SoundFileExportFilenameLogLock As New Object

    ''' <summary>
    ''' Creates the path needed for file export
    ''' </summary>
    ''' <param name="RoutingString">This string should contain the output routing for the channels, in the format 
    ''' 1_2-6_5-6 meaning wave channels 1 goes to hardware channel 1, wave channel 2 goes to  hardware channels 2 and 6, and wave channel 3 goes to hardware channel 5 and 6...</param>
    ''' <returns></returns>
    Public Shared Function GetSoundFileExportLogPath(ByVal RoutingString As String) As String

        SyncLock SoundFileExportFilenameLogLock

            Return IO.Path.Combine(LogFileDirectory, "PlayedSoundsLog", CreateDateTimeStringForFileNames() & "_" & RoutingString & ".wav")

            'Sleeps two milliseconds two prevent the same log file name being created twice
            Thread.Sleep(2)

        End SyncLock

    End Function

End Class

