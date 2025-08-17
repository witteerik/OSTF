'This software is available under the following license:
'MIT/X11 License
'
'Copyright (c) 2020 Erik Witte
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

Namespace Audio
    Public Module AudioLog

        Public showAudioErrors As Boolean = True
        Public logAudioErrors As Boolean = True
        Public AudioLogIsInMultiThreadApplication As Boolean = False
        Public AudioLogSpinLock As New Threading.SpinLock

        Public Sub SendInfoToAudioLog(ByVal message As String,
                         Optional ByVal LogFileNameWithoutExtension As String = "",
                         Optional LogFileTemporaryPath As String = "",
                         Optional ByVal OmitDateInsideLog As Boolean = False,
                         Optional ByVal OmitDateInFileName As Boolean = False)

            Dim SpinLockTaken As Boolean = False

            Try

                'Getting the application path
                Dim logFilePath As String = IO.Path.Combine(IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName, "AudioLog")

                'Attempts to enter a spin lock to avoid multiple thread conflicts when saving to the same file
                AudioLogSpinLock.Enter(SpinLockTaken)

                If LogFileTemporaryPath = "" Then LogFileTemporaryPath = Logging.LogFileDirectory

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
                If AudioLogIsInMultiThreadApplication = True Then
                    Dim TreadName As String = Thread.CurrentThread.ManagedThreadId
                    OutputFilePath &= "ThreadID_" & TreadName
                End If

                Try
                    If Not Directory.Exists(LogFileTemporaryPath) Then Directory.CreateDirectory(LogFileTemporaryPath)
                    Dim samplewriter As New StreamWriter(OutputFilePath, FileMode.Append)
                    If OmitDateInsideLog = False Then
                        samplewriter.WriteLine(DateTime.Now.ToString & vbCrLf & message)
                    Else
                        samplewriter.WriteLine(message)
                    End If
                    samplewriter.Close()

                Catch ex As Exception
                    'Just ignores any errors here...
                End Try
            Finally

                'Releases any spinlock
                If SpinLockTaken = True Then AudioLogSpinLock.Exit()
            End Try

        End Sub

        Public Sub AudioError(ByVal errorText As String, Optional ByVal errorTitle As String = "Error")

            If showAudioErrors = True Then
                MsgBox(errorText, MsgBoxStyle.Critical, errorTitle)
            End If

            If logAudioErrors = True Then
                SendInfoToAudioLog("The following error occurred: " & vbCrLf & errorTitle & errorText, "Errors")
            End If

        End Sub

    End Module

End Namespace