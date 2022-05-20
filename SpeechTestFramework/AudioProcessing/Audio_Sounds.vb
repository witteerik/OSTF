Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Windows.Forms

Namespace Audio

    <Serializable>
    Public Class Sounds

        Inherits List(Of Sound)
        Public Property SampleSound As Audio.Sound
        Public Property DataIsChanged As Boolean = True
        Public Property CurrentSoundIndex As Integer
        Public Property Name As String = "newProject"
        Public Property InnerSounds As Audio.Sounds

        ''' <summary>
        ''' Stores the current instance of Sounds to a binary (.snds) file.
        ''' </summary>
        ''' <param name="filePath">The file path where the file is saved.</param>
        ''' <returns>Returns True if the save procedure completed, and False is saving failed.</returns>
        Public Function SaveToBinaryFile(Optional ByVal filePath As String = "") As Boolean

            Try

                If filePath = "" Then

SavingFile:         Dim sfd As New SaveFileDialog
                    'Saving project file
                    Dim filter As String = "Text Files (*.snds)|*.snds"
                    sfd.Filter = filter
                    Dim result As DialogResult = sfd.ShowDialog()
                    If result = DialogResult.OK Then
                        filePath = sfd.FileName
                    Else
                        Dim errorSaving As MsgBoxResult = MsgBox("An error occurred choosing file name.", MsgBoxStyle.RetryCancel, "Warning!")
                        If errorSaving = MsgBoxResult.Retry Then
                            GoTo SavingFile
                        End If
                        If errorSaving = MsgBoxResult.Cancel Then
                            Return Nothing
                        End If
                    End If

                End If

                Dim DataFileStream As Stream = File.Create(filePath)
                Dim serializer As New BinaryFormatter
                serializer.Serialize(DataFileStream, Me)
                DataFileStream.Close()
                Return True
            Catch ex As Exception
                MsgBox(ex.ToString)
                Return False
            End Try

        End Function

    End Class

End Namespace