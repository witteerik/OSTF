<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LoadOstaMediaSetControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.DescriptionLabel = New System.Windows.Forms.Label()
        Me.SelectMediaSet_Button = New System.Windows.Forms.Button()
        Me.MediaSetSelection_ComboBox = New System.Windows.Forms.ComboBox()
        Me.SearchForMediaSets_Button = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 98.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.DescriptionLabel, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.SelectMediaSet_Button, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.MediaSetSelection_ComboBox, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.SearchForMediaSets_Button, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(407, 54)
        Me.TableLayoutPanel1.TabIndex = 5
        '
        'DescriptionLabel
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.DescriptionLabel, 3)
        Me.DescriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DescriptionLabel.Location = New System.Drawing.Point(3, 0)
        Me.DescriptionLabel.Name = "DescriptionLabel"
        Me.DescriptionLabel.Size = New System.Drawing.Size(401, 25)
        Me.DescriptionLabel.TabIndex = 3
        Me.DescriptionLabel.Text = "Select and load media set"
        Me.DescriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SelectMediaSet_Button
        '
        Me.SelectMediaSet_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SelectMediaSet_Button.Location = New System.Drawing.Point(312, 28)
        Me.SelectMediaSet_Button.Name = "SelectMediaSet_Button"
        Me.SelectMediaSet_Button.Size = New System.Drawing.Size(92, 23)
        Me.SelectMediaSet_Button.TabIndex = 2
        Me.SelectMediaSet_Button.Text = "Load media set"
        Me.SelectMediaSet_Button.UseVisualStyleBackColor = True
        '
        'MediaSetSelection_ComboBox
        '
        Me.MediaSetSelection_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MediaSetSelection_ComboBox.FormattingEnabled = True
        Me.MediaSetSelection_ComboBox.Location = New System.Drawing.Point(73, 28)
        Me.MediaSetSelection_ComboBox.Name = "MediaSetSelection_ComboBox"
        Me.MediaSetSelection_ComboBox.Size = New System.Drawing.Size(233, 21)
        Me.MediaSetSelection_ComboBox.TabIndex = 0
        '
        'SearchForMediaSets_Button
        '
        Me.SearchForMediaSets_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SearchForMediaSets_Button.Location = New System.Drawing.Point(3, 28)
        Me.SearchForMediaSets_Button.Name = "SearchForMediaSets_Button"
        Me.SearchForMediaSets_Button.Size = New System.Drawing.Size(64, 23)
        Me.SearchForMediaSets_Button.TabIndex = 1
        Me.SearchForMediaSets_Button.Text = "Search"
        Me.SearchForMediaSets_Button.UseVisualStyleBackColor = True
        '
        'LoadOstaMediaSetControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "LoadOstaMediaSetControl"
        Me.Size = New System.Drawing.Size(407, 54)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents DescriptionLabel As Windows.Forms.Label
    Friend WithEvents SelectMediaSet_Button As Windows.Forms.Button
    Friend WithEvents MediaSetSelection_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents SearchForMediaSets_Button As Windows.Forms.Button
End Class
