Imports System.Windows.Forms
Imports System.ComponentModel

<Serializable>
Public Class LoadFileControl
    Inherits PathControl

    '<DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
    Private WithEvents LoadButton As New Button

    Public Event LoadFile(ByVal FileToLoad As String)

    Public Sub New()

        MyBase.New

        Me.PathTextBox.DirectionType = PathTextBox.DirectionTypes.Load
        Me.PathTextBox.PathType = PathTextBox.PathTypes.File

        Me.SetColumnSpan(MyBase.BrowseButton, 1)

        LoadButton.Text = "Load"
        MyBase.Controls.Add(LoadButton, 2, 1)
        LoadButton.Dock = DockStyle.Fill

        Me.ColumnStyles.Clear()
        Me.ColumnStyles.Add(New Windows.Forms.ColumnStyle(Windows.Forms.SizeType.Percent, 50))
        Me.ColumnStyles.Add(New Windows.Forms.ColumnStyle(Windows.Forms.SizeType.Absolute, 60))
        Me.ColumnStyles.Add(New Windows.Forms.ColumnStyle(Windows.Forms.SizeType.Absolute, 60))

    End Sub

    Private Sub LoadFileButton_Click() Handles LoadButton.Click

        If PathTextBox.SelectedPath <> "" Then
            RaiseEvent LoadFile(PathTextBox.SelectedPath)
        Else
            MsgBox("Select a valid file path!", MsgBoxStyle.Information, "Invalid file path!")
        End If

    End Sub

End Class
