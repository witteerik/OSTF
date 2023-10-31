

Public Interface iOstfGui

    ''' <summary>
    ''' Launches an open file dialog and asks the user for a sound file.
    ''' </summary>
    ''' <param name="directory"></param>
    ''' <param name="fileName"></param>
    ''' <returns></returns>
    Function OpenSoundFileDialog(Optional ByRef directory As String = "", Optional ByRef fileName As String = "") As String

    ''' <summary>
    ''' Launches a save file dialog and asks the user for a file path.
    ''' </summary>
    ''' <param name="directory"></param>
    ''' <param name="fileName"></param>
    ''' <param name="fileFormat"></param>
    ''' <returns>The file path to which the sound file should be written.</returns>
    Function SaveSoundFileDialog(Optional ByRef directory As String = "", Optional ByRef fileName As String = "", Optional ByVal fileFormat As Audio.SoundFileFormats = Audio.SoundFileFormats.wav) As String

    ''' <summary>
    ''' Asks the user to supply a file path by using a save file dialog box.
    ''' </summary>
    ''' <param name="directory">Optional initial directory.</param>
    ''' <param name="fileName">Optional suggested file name</param>
    ''' <param name="fileExtensions">Optional possible extensions</param>
    ''' <param name="BoxTitle">The message/title on the file dialog box</param>
    ''' <returns>Returns the file path, or nothing if a file path could not be created.</returns>
    Function GetSaveFilePath(Optional ByRef directory As String = "", Optional ByRef fileName As String = "", Optional fileExtensions() As String = Nothing, Optional BoxTitle As String = "") As String

    ''' <summary>
    ''' Asks the user to select a folder using a folder browser dialog.
    ''' </summary>
    ''' <param name="directory">Optional initial directory.</param>
    ''' <param name="BoxTitle">The message/title on the file dialog box</param>
    ''' <returns>Returns the folder path, or an empty string if a folder path was not selected.</returns>
    Function GetFolder(Optional ByRef directory As String = "", Optional BoxTitle As String = "") As String

    ''' <summary>
    ''' Asks the user to supply a file path by using an open file dialog box.
    ''' </summary>
    ''' <param name="directory">Optional initial directory.</param>
    ''' <param name="fileName">Optional suggested file name</param>
    ''' <param name="fileExtensions">Optional possible extensions</param>
    ''' <param name="BoxTitle">The message/title on the file dialog box</param>
    ''' <returns>Returns the file path, or nothing if a file path could not be created.</returns>
    Function GetOpenFilePath(Optional directory As String = "", Optional fileName As String = "", Optional fileExtensions() As String = Nothing, Optional BoxTitle As String = "", Optional ReturnEmptyStringOnCancel As Boolean = False) As String

    ''' <summary>
    ''' Asks the user to supply one or more file paths by using an open file dialog box.
    ''' </summary>
    ''' <param name="directory">Optional initial directory.</param>
    ''' <param name="fileName">Optional suggested file name</param>
    ''' <param name="fileExtensions">Optional possible extensions</param>
    ''' <param name="BoxTitle">The message/title on the file dialog box</param>
    ''' <returns>Returns the file path, or nothing if a file path could not be created.</returns>
    Function GetOpenFilePaths(Optional directory As String = "", Optional fileName As String = "", Optional fileExtensions() As String = Nothing, Optional BoxTitle As String = "", Optional ReturnEmptyStringArrayOnCancel As Boolean = False) As String()



End Interface



