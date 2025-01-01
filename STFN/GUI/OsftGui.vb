

Public Interface IOstfGui

    ''' <summary>
    ''' Should display a message to the user
    ''' </summary>
    ''' <param name="Title"></param>
    Function DisplayMessage(ByVal Title As String, ByVal Message As String, ByVal CancelButtonText As String) As Task

    ''' <summary>
    ''' Should display a message to the user
    ''' </summary>
    ''' <param name="Title"></param>
    Function DisplayMessageAsync(ByVal sender As Object, ByVal e As MessageEventArgs) As Task

    ''' <summary>
    ''' Should ask the user to respond to a question
    ''' </summary>
    Function DisplayBooleanQuestion(ByVal sender As Object, ByVal e As QuestionEventArgs) As Task

    ''' <summary>
    ''' Should ask the user to supply a file path by using a save file dialog box.
    Sub GetSaveFilePath(ByVal sender As Object, ByVal e As PathEventArgs)

    ''' <summary>
    ''' Should asks the user to select a folder using a folder browser dialog.
    ''' </summary>
    Sub GetFolder(ByVal sender As Object, ByVal e As PathEventArgs)

    ''' <summary>
    ''' Should ask the user to supply a file path by using an open file dialog box.
    ''' </summary>
    Sub GetOpenFilePath(ByVal sender As Object, ByVal e As PathEventArgs)

    ''' <summary>
    ''' Should asks the user to supply one or more file paths by using an open file dialog box.
    Sub GetOpenFilePaths(ByVal sender As Object, ByVal e As PathsEventArgs)


End Interface


