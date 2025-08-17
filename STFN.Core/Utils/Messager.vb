
Public Class Messager

    Public Shared Event OnNewMessage(ByVal Title As String, ByVal Message As String, ByVal CancelButtonText As String)
    Public Shared Event OnNewAsyncMessage As EventHandler(Of MessageEventArgs)
    Public Shared Event OnNewQuestion As EventHandler(Of QuestionEventArgs)
    Public Shared Event OnGetSaveFilePath As EventHandler(Of PathEventArgs)
    Public Shared Event OnGetFolder As EventHandler(Of PathEventArgs)
    Public Shared Event OnGetOpenFilePath As EventHandler(Of PathEventArgs)
    Public Shared Event OnGetOpenFilePaths As EventHandler(Of PathsEventArgs)
    Public Shared Event OnCloseAppRequest()


    Public Enum MsgBoxStyle
        Information
        Exclamation
        Critical
    End Enum

    ''' <summary>
    ''' A message box that will relay any messages to the GUI currently used, but requires that the GUI listens to and handles the OnNewMessage event.
    ''' </summary>
    ''' <param name="Message"></param>
    ''' <param name="Style"></param>
    ''' <param name="Title"></param>
    ''' <param name="CancelButtonText"></param>
    Public Shared Sub MsgBox(ByVal Message As String, Optional ByVal Style As MsgBoxStyle = MsgBoxStyle.Information, Optional ByVal Title As String = "", Optional ByVal CancelButtonText As String = "OK")

        Select Case Style
            Case MsgBoxStyle.Information

                RaiseEvent OnNewMessage(Title, Message, CancelButtonText)

            Case MsgBoxStyle.Exclamation

                'TODO, add some "Exclamation" notice...
                RaiseEvent OnNewMessage(Title, Message, CancelButtonText)

        End Select

    End Sub


    ''' <summary>
    ''' A message box that will relay any messages to the GUI currently used, but requires that the GUI listens to and handles the OnNewAsyncMessage event.
    ''' </summary>
    ''' <param name="Message"></param>
    ''' <param name="Style"></param>
    ''' <param name="Title"></param>
    ''' <param name="CancelButtonText"></param>
    ''' <returns>Awaits the response but the boolean value returned is meaningless.</returns>
    Public Shared Async Function MsgBoxAsync(ByVal Message As String, Optional ByVal Style As MsgBoxStyle = MsgBoxStyle.Information, Optional ByVal Title As String = "", Optional ByVal CancelButtonText As String = "OK") As Task(Of Boolean)

        'TODO: This function could perhaps be rewritten to wait for the response without having to return a Task of Boolean...

        Dim tcs As New TaskCompletionSource(Of Boolean)

        Select Case Style
            Case MsgBoxStyle.Information

                RaiseEvent OnNewAsyncMessage(Nothing, New MessageEventArgs(Title, Message, CancelButtonText, tcs))

            Case MsgBoxStyle.Exclamation

                'TODO, add some "Exclamation" notice...
                RaiseEvent OnNewAsyncMessage(Nothing, New MessageEventArgs(Title, Message, CancelButtonText, tcs))

        End Select

        Return Await tcs.Task

    End Function

    Public Shared Async Function MsgBoxAcceptQuestion(ByVal Question As String, Optional ByVal Title As String = "",
                                          Optional ByVal AcceptButtonText As String = "Yes", Optional ByVal CancelButtonText As String = "No") As Task(Of Boolean)

        Dim tcs As New TaskCompletionSource(Of Boolean)()

        RaiseEvent OnNewQuestion(Nothing, New QuestionEventArgs(Title, Question, AcceptButtonText, CancelButtonText, tcs))

        Return Await tcs.Task

    End Function


    ''' <summary>
    ''' Asks the user to supply a file path by using a save file dialog box.
    ''' </summary>
    ''' <param name="directory">Optional initial Directory.</param>
    ''' <param name="fileName">Optional suggested file name</param>
    ''' <param name="fileExtensions">Optional possible extensions</param>
    ''' <param name="Title">The message/title on the file dialog box</param>
    ''' <returns>Returns the file path, or nothing if a file path could not be created.</returns>
    Public Shared Async Function GetSaveFilePath(Optional directory As String = "", Optional fileName As String = "", Optional fileExtensions() As String = Nothing, Optional Title As String = "") As Task(Of String)

        Dim tcs As New TaskCompletionSource(Of String)()
        RaiseEvent OnGetSaveFilePath(Nothing, New PathEventArgs(tcs, "OK", "Cancel", directory, fileName, fileExtensions, Title))
        Return Await tcs.Task

    End Function

    ''' <summary>
    ''' Asks the user to select a folder using a folder browser dialog.
    ''' </summary>
    ''' <param name="directory">Optional initial Directory.</param>
    ''' <param name="Title">The message/title on the file dialog box</param>
    ''' <returns>Returns the folder path, or an empty string if a folder path was not selected.</returns>
    Public Shared Async Function GetFolder(Optional directory As String = "", Optional Title As String = "") As Task(Of String)

        Dim tcs As New TaskCompletionSource(Of String)()
        RaiseEvent OnGetFolder(Nothing, New PathEventArgs(tcs, "OK", "Cancel", directory,,, Title))
        Return Await tcs.Task

    End Function

    ''' <summary>
    ''' Asks the user to supply a file path by using an open file dialog box.
    ''' </summary>
    ''' <param name="directory">Optional initial Directory.</param>
    ''' <param name="fileName">Optional suggested file name</param>
    ''' <param name="fileExtensions">Optional possible extensions</param>
    ''' <param name="Title">The message/title on the file dialog box</param>
    ''' <returns>Returns the file path, or nothing if a file path could not be created.</returns>
    Public Shared Async Function GetOpenFilePath(Optional directory As String = "", Optional fileName As String = "", Optional fileExtensions() As String = Nothing, Optional Title As String = "", Optional ReturnEmptyStringOnCancel As Boolean = False) As Task(Of String)

        Dim tcs As New TaskCompletionSource(Of String)()
        RaiseEvent OnGetOpenFilePath(Nothing, New PathEventArgs(tcs, "OK", "Cancel", directory, fileName, fileExtensions, Title, ReturnEmptyStringOnCancel))
        Return Await tcs.Task

    End Function

    ''' <summary>
    ''' Asks the user to supply one or more file paths by using an open file dialog box.
    ''' </summary>
    ''' <param name="Directory">Optional initial Directory.</param>
    ''' <param name="fileName">Optional suggested file name</param>
    ''' <param name="FileExtensions">Optional possible extensions</param>
    ''' <param name="Title">The message/title on the file dialog box</param>
    ''' <returns>Returns the file path, or nothing if a file path could not be created.</returns>
    Public Shared Async Function GetOpenFilePaths(Optional Directory As String = "", Optional FileExtensions() As String = Nothing, Optional Title As String = "", Optional ReturnEmptyStringArrayOnCancel As Boolean = False) As Task(Of String())

        Dim tcs As New TaskCompletionSource(Of String())()
        RaiseEvent OnGetOpenFilePaths(Nothing, New PathsEventArgs(tcs, "OK", "Cancel", Directory, FileExtensions, Title, ReturnEmptyStringArrayOnCancel))
        Return Await tcs.Task

    End Function

    ''' <summary>
    ''' Launches a save file dialog and asks the user for a file path.
    ''' </summary>
    ''' <param name="directory"></param>
    ''' <param name="fileName"></param>
    ''' <param name="fileFormat"></param>
    ''' <returns>The file path to which the sound file should be written.</returns>
    Public Shared Async Function SaveSoundFileDialog(Optional Directory As String = "", Optional FileName As String = "", Optional ByVal FileFormat As Audio.SoundFileFormats = Audio.SoundFileFormats.wav) As Task(Of String)

        Dim SelectedExtension As String() = {}
        Select Case FileFormat
            Case Audio.BasicAudioEnums.SoundFileFormats.wav
                SelectedExtension = {".wav"}
            Case Audio.BasicAudioEnums.SoundFileFormats.ptwf
                SelectedExtension = {".ptwf"}
        End Select
        Return Await GetSaveFilePath(Directory, FileName, SelectedExtension, "Save sound file")

    End Function


    ''' <summary>
    ''' Launches an open file dialog and asks the user for a sound file.
    ''' </summary>
    ''' <param name="directory"></param>
    ''' <param name="fileName"></param>
    ''' <returns></returns>
    Public Shared Async Function OpenSoundFileDialog(Optional Directory As String = "", Optional FileName As String = "") As Task(Of String)
        Return Await GetOpenFilePath(Directory, FileName, {".wav", ".ptwf"})
    End Function

    ''' <summary>
    ''' A method that can be used to cloase the app from lower level libraries, but requires that the GUI listens to and handles the OnCloseAppRequest event.
    ''' </summary>
    Public Shared Sub RequestCloseApp()
        'TODO, this function shuld probably be defined elsewhere, in a more app specific Module.
        RaiseEvent OnCloseAppRequest()
    End Sub

End Class


Public Class QuestionEventArgs
    Inherits EventArgs

    Public Property Title As String
    Public Property Question As String
    Public Property AcceptButtonText As String
    Public Property CancelButtonText As String
    Public Property TaskCompletionSource As TaskCompletionSource(Of Boolean)

    Public Sub New(Title As String, Question As String, AcceptButtonText As String, CancelButtonText As String, Tcs As TaskCompletionSource(Of Boolean))
        Me.Title = Title
        Me.Question = Question
        Me.AcceptButtonText = AcceptButtonText
        Me.CancelButtonText = CancelButtonText
        Me.TaskCompletionSource = Tcs
    End Sub
End Class

Public Class MessageEventArgs
    Inherits EventArgs

    Public Property Title As String
    Public Property Message As String
    Public Property CancelButtonText As String
    Public Property TaskCompletionSource As TaskCompletionSource(Of Boolean)

    Public Sub New(Title As String, Question As String, CancelButtonText As String, Tcs As TaskCompletionSource(Of Boolean))
        Me.Title = Title
        Me.Message = Question
        Me.CancelButtonText = CancelButtonText
        Me.TaskCompletionSource = Tcs
    End Sub
End Class

Public Class PathEventArgs
    Inherits EventArgs

    Public Property Title As String

    Public Property Directory As String
    Public Property FileName As String
    Public Property FileExtensions As String()
    Public Property ReturnEmptyStringOnCancel As Boolean

    Public Property AcceptButtonText As String
    Public Property CancelButtonText As String
    Public Property TaskCompletionSource As TaskCompletionSource(Of String)

    Public Sub New(tcs As TaskCompletionSource(Of String), ByVal AcceptButtonText As String, ByVal CancelButtonText As String,
                   Optional ByVal Directory As String = "",
                   Optional ByRef FileName As String = "",
                   Optional FileExtensions() As String = Nothing,
                   Optional Title As String = "",
                   Optional ReturnEmptyStringOnCancel As Boolean = False)

        Me.Title = Title
        Me.Directory = Directory
        Me.FileName = FileName
        Me.FileExtensions = FileExtensions
        Me.ReturnEmptyStringOnCancel = ReturnEmptyStringOnCancel

        Me.AcceptButtonText = AcceptButtonText
        Me.CancelButtonText = CancelButtonText
        Me.TaskCompletionSource = tcs
    End Sub
End Class

Public Class PathsEventArgs
    Inherits EventArgs

    Public Property Title As String

    Public Property Directory As String
    Public Property FileName As String
    Public Property FileExtensions As String()
    Public Property ReturnEmptyStringOnCancel As Boolean

    Public Property AcceptButtonText As String
    Public Property CancelButtonText As String
    Public Property TaskCompletionSource As TaskCompletionSource(Of String())

    Public Sub New(tcs As TaskCompletionSource(Of String()), ByVal AcceptButtonText As String, ByVal CancelButtonText As String,
                   Optional ByVal Directory As String = "",
                   Optional FileExtensions() As String = Nothing,
                   Optional Title As String = "",
                   Optional ReturnEmptyStringOnCancel As Boolean = False)

        Me.Title = Title
        Me.Directory = Directory
        Me.FileExtensions = FileExtensions
        Me.ReturnEmptyStringOnCancel = ReturnEmptyStringOnCancel

        Me.AcceptButtonText = AcceptButtonText
        Me.CancelButtonText = CancelButtonText
        Me.TaskCompletionSource = tcs
    End Sub
End Class


