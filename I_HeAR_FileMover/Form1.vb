Imports System.IO

Public Class Form1

    Private VerifiedId As String = ""
    Private FileDestinationDirectory As String = "C:\AMTASdata"
    Private SourceDirectory1 As String = "C:\ProgramData\Interacoustics\Affinity Suite\AUD\AmtasData"
    Private SourceDirectory2 As String = "C:\ProgramData\Interacoustics\Affinity Suite\Data\"

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Results_TextBox.Text = "Filerna har inte flyttats ännu!"
        Results_TextBox.ForeColor = Color.DarkRed

    End Sub

    Private Sub ID_TextBox_TextChanged(sender As Object, e As EventArgs) Handles ID_TextBox.TextChanged

        If ID_TextBox.Text.Length > 6 Then
            ID_Error_Label.Text = "Koden är för lång!"
            MoveFiles_Button.Enabled = False
            Exit Sub
        End If

        If ID_TextBox.Text.Length = 6 Then

            If CheckID(ID_TextBox.Text) = True Then
                ID_Error_Label.Text = ""
                VerifiedId = ID_TextBox.Text
                MoveFiles_Button.Enabled = True
                MoveFiles_Button.Focus()
            Else
                ID_Error_Label.Text = "Koden har felaktigt format!"
                MoveFiles_Button.Enabled = False
            End If
        Else
            ID_Error_Label.Text = ""
            MoveFiles_Button.Enabled = False
        End If

    End Sub

    Private Function CheckID(ByVal ID As String) As Boolean

        'Checks if the ID string has the required length of 6
        If ID.Length <> 6 Then Return False

        'Checks if the first two are letters and the last four are digits
        If Char.IsLetter(ID.Substring(0, 1)) = False Then Return False
        If Char.IsLetter(ID.Substring(1, 1)) = False Then Return False
        If Char.IsDigit(ID.Substring(2, 1)) = False Then Return False
        If Char.IsDigit(ID.Substring(3, 1)) = False Then Return False
        If Char.IsDigit(ID.Substring(4, 1)) = False Then Return False
        If Char.IsDigit(ID.Substring(5, 1)) = False Then Return False

        'All checks passed, returns True
        Return True

    End Function

    Private Sub MoveFiles_Button_Click(sender As Object, e As EventArgs) Handles MoveFiles_Button.Click

        Dim CopyFilesErrorMessage = CopyFiles()

        If CopyFilesErrorMessage <> "" Then
            Results_TextBox.ForeColor = Color.DarkRed
            Results_TextBox.Text = CopyFilesErrorMessage
            Results_TextBox.BorderStyle = BorderStyle.FixedSingle
        Else
            Results_TextBox.ForeColor = Color.DarkGreen
            Results_TextBox.Text = "Filerna har flyttats till " & FileDestinationDirectory & vbCrLf & vbCrLf & "Du kan nu stänga programmet."
            Results_TextBox.BorderStyle = BorderStyle.None
            MoveFiles_Button.Enabled = False
            ID_TextBox.Enabled = False
        End If

    End Sub


    Private Function CopyFiles() As String

        Try

            'Creates the destination directory if it doesn't yet exist
            If IO.Directory.Exists(FileDestinationDirectory) = False Then IO.Directory.CreateDirectory(FileDestinationDirectory)

            'Creates destination file paths
            Dim DestinationFile1 As String = IO.Path.Combine(FileDestinationDirectory, "AMTASdata1_" & VerifiedId & "_" & DateTime.Now.ToShortDateString.Replace("/", "-") & ".zip")
            Dim DestinationFile2 As String = IO.Path.Combine(FileDestinationDirectory, "AMTASdata2_" & VerifiedId & "_" & DateTime.Now.ToShortDateString.Replace("/", "-") & ".zip")

            'Checks that the destination files do not already exist
            If IO.File.Exists(DestinationFile1) = True Then Return "Varing!" & vbCrLf & vbCrLf & "Filen " & DestinationFile1 & " existerar redan. " & vbCrLf & vbCrLf & " Filerna har INTE flyttats!"
            If IO.File.Exists(DestinationFile2) = True Then Return "Varing!" & vbCrLf & vbCrLf & "Filen " & DestinationFile2 & " existerar redan. " & vbCrLf & vbCrLf & " Filerna har INTE flyttats!"

            'Compressed the files
            System.IO.Compression.ZipFile.CreateFromDirectory(SourceDirectory1, DestinationFile1)
            System.IO.Compression.ZipFile.CreateFromDirectory(SourceDirectory2, DestinationFile2)

            'Verifying that DestinationFile1 and DestinationFile2 exists, and returns if not
            If IO.File.Exists(DestinationFile1) = False Then Return "Kunde inte zippa filerna i mappen: " & DestinationFile1
            If IO.File.Exists(DestinationFile2) = False Then Return "Kunde inte zippa filerna i mappen: " & DestinationFile2

            'Proceeds with removing the data in the "C:\ProgramData\Interacoustics\Affinity Suite\Data\" directory
            Dim FilesForDeletion As String() = Directory.GetFiles(SourceDirectory2)
            For Each file As String In FilesForDeletion
                IO.File.Delete(file)
            Next

            'Returns an empty string to indicate success
            Return ""

        Catch ex As Exception
            Return "Följande fel inträffade: " & ex.ToString
        End Try

    End Function

    Private Sub Info_Button_Click(sender As Object, e As EventArgs) Handles Info_Button.Click

        MsgBox("Detta program zippar filerna i följande två mappar:" & vbCrLf & vbCrLf &
               SourceDirectory1 & vbCrLf &
               SourceDirectory2 & vbCrLf & vbCrLf &
               "och placerar de zippade filerna i mappen:" & vbCrLf & vbCrLf &
               FileDestinationDirectory, MsgBoxStyle.Information, "Vad gör detta program?")

    End Sub
End Class
