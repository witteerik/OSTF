Imports System.Windows.Forms
Imports System.ComponentModel

<Serializable>
Public Class LoadFileControl
    Inherits PathControl

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
    Private WithEvents LoadButton As New Button

    Public Event LoadFile(ByVal FileToLoad As String)

    Public Sub New()

        MyBase.New

        Me.PathTextBox.DirectionType = PathTextBox.DirectionTypes.Load
        Me.PathTextBox.PathType = PathTextBox.PathTypes.File

        Me.Controls.Add(LoadButton, 2, 1)
        LoadButton.Dock = DockStyle.Fill
        LoadButton.Text = "Load"

    End Sub

    Private Sub LoadFileButton_Click() Handles LoadButton.Click

        If PathTextBox.SelectedPath <> "" Then
            RaiseEvent LoadFile(PathTextBox.SelectedPath)
        Else
            MsgBox("Select a valid file path!", MsgBoxStyle.Information, "Invalid file path!")
        End If

    End Sub

End Class
