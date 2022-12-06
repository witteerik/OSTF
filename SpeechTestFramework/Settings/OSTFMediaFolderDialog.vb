Imports System.Windows.Forms

Public Class OSTFMediaFolderDialog

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

        If IO.Directory.Exists(OSTFMedia_PathControl.SelectedPath) = True Then
            'Setting the MediaRootDirectory 
            MediaRootDirectory = OSTFMedia_PathControl.SelectedPath

            'Checks that it seems to be the right (media) folder
            If IO.Directory.Exists(IO.Path.Combine(MediaRootDirectory, AudioSystemSubDirectory)) = False Then

                'Resetting the MediaRootDirectory and then throws an exception that asks the user to restart.
                MsgBox("It seems like you have selected an incorrect OSTF media folder. The OSTF media folder should among others contain the folder " & AudioSystemSubDirectory & vbCrLf &
                                    "Please locate the correct folder! The application will not be able to start without it.")
                Exit Sub
            End If

            'Stores the MediaRootDirectory in the local_settings file
            OstfBase.StoreMediaRootDirectory(MediaRootDirectory)

        Else
            MsgBox("Unable to locate the selected folder: " & OSTFMedia_PathControl.SelectedPath, MsgBoxStyle.Exclamation, "Set OSTF media folder")
            Exit Sub
        End If

        'Closes the dialog
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub OSTFMediaFolderDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim local_settings_FilePath As String = IO.Path.Combine(Application.StartupPath, "local_settings.txt")
        Dim local_settings_Input = IO.File.ReadAllLines(local_settings_FilePath)

        For Each item In local_settings_Input
            If item.Trim = "" Then Continue For
            If item.Trim.StartsWith("//") Then Continue For

            Dim SplitItem = item.Trim.Split("=")
            If SplitItem.Length < 2 Then Continue For

            If SplitItem(0).Trim.ToLower.StartsWith("MediaRootDirectory".ToLower) Then MediaRootDirectory = SplitItem(1).Trim

        Next

        Me.OSTFMedia_PathControl.SetSelectedPath(MediaRootDirectory)

        Me.OSTFMedia_PathControl.BorderStyle = BorderStyle.None

    End Sub

    Private Sub OSTFMediaFolderDialog_HelpButtonClicked(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles MyBase.HelpButtonClicked

        MsgBox("Use this dialog to set the path to the OSTF media folder you wish to use. A possible such path could be 'C:\OSTFMedia', depending on where you have stored the OSTF Media folder on your computer." & vbCrLf & vbCrLf &
               "Preferably, you should store the OSTF media files locally on your computer and not access them over a network, as that may not be fast enough for the available OSTF appplications.", MsgBoxStyle.MsgBoxHelp, "Set OSTF media folder")

    End Sub


End Class
