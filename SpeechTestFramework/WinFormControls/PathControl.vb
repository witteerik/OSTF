Imports System.Windows.Forms
Imports System.ComponentModel

<Serializable>
Public Class PathControl
    Inherits TableLayoutPanel

    Private _Description As String = ""

    Public Property Description As String
        Get
            Return _Description
        End Get
        Set(value As String)
            _Description = value
            DescriptionLabel.Text = value
        End Set
    End Property

    '<DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
    Protected DescriptionLabel As New Label

    '<DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
    Protected WithEvents PathTextBox As New PathTextBox

    '<DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
    Protected WithEvents BrowseButton As New Button

    Public ReadOnly Property SelectedPath As String
        Get
            Return PathTextBox.SelectedPath
        End Get
    End Property

    Public Sub New()

        Me.BorderStyle = Windows.Forms.BorderStyle.FixedSingle

        'Adding controls
        Me.RowCount = 2
        MyBase.ColumnCount = 3
        Me.GrowStyle = Windows.Forms.TableLayoutPanelGrowStyle.AddColumns
        Me.RowStyles.Clear()
        Me.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Percent, 50))
        Me.ColumnStyles.Clear()
        Me.ColumnStyles.Add(New Windows.Forms.ColumnStyle(Windows.Forms.SizeType.Percent, 50))
        Me.ColumnStyles.Add(New Windows.Forms.ColumnStyle(Windows.Forms.SizeType.Absolute, 30))
        Me.ColumnStyles.Add(New Windows.Forms.ColumnStyle(Windows.Forms.SizeType.Absolute, 30))

        Me.Controls.Add(DescriptionLabel, 0, 0)
        Me.SetColumnSpan(DescriptionLabel, 3)
        DescriptionLabel.AutoSize = False
        DescriptionLabel.Dock = DockStyle.Fill
        DescriptionLabel.TextAlign = Drawing.ContentAlignment.MiddleLeft

        Me.Controls.Add(PathTextBox, 0, 1)
        PathTextBox.Dock = DockStyle.Fill

        Me.Controls.Add(BrowseButton, 1, 1)
        BrowseButton.Dock = DockStyle.Fill
        BrowseButton.Text = "Browse"
        Me.SetColumnSpan(BrowseButton, 2)

        PathTextBox.Margin = BrowseButton.Margin

    End Sub

    Private Sub StartBrowse() Handles BrowseButton.Click

        Select Case PathTextBox.DirectionType
            Case PathTextBox.DirectionTypes.Load

                Select Case PathTextBox.PathType
                    Case PathTextBox.PathTypes.File
                        'Open a load file dialog
                        Dim fdg As New OpenFileDialog()
                        fdg.Title = "Select file"
                        fdg.Filter = PathTextBox.GetFileExtensionFilter
                        Dim result = fdg.ShowDialog()
                        If result = DialogResult.OK Then
                            PathTextBox.Text = fdg.FileName
                        End If

                    Case PathTextBox.PathTypes.Folder
                        'Open a select input folder dialog
                        Dim fbd As New FolderBrowserDialog()
                        fbd.Description = "Select input folder "
                        Dim result = fbd.ShowDialog()
                        If result = DialogResult.OK Then
                            PathTextBox.Text = fbd.SelectedPath
                        End If

                End Select

            Case PathTextBox.DirectionTypes.Save

                Select Case PathTextBox.PathType
                    Case PathTextBox.PathTypes.File
                        'Open a save file dialog
                        'Open a load file dialog
                        Dim fdg As New SaveFileDialog()
                        fdg.Title = "Select file name"
                        fdg.Filter = PathTextBox.GetFileExtensionFilter
                        Dim result = fdg.ShowDialog()
                        If result = DialogResult.OK Then
                            PathTextBox.Text = fdg.FileName
                        End If

                    Case PathTextBox.PathTypes.Folder
                        'Open a select output folder dialog

                        'Open a select input folder dialog
                        Dim fbd As New FolderBrowserDialog()
                        fbd.Description = "Select ouput folder "
                        Dim result = fbd.ShowDialog()
                        If result = DialogResult.OK Then
                            PathTextBox.Text = fbd.SelectedPath
                        End If

                End Select

        End Select


    End Sub

End Class