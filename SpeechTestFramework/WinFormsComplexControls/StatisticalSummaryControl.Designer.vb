<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class StatisticalSummaryControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LoadedSpeechMaterialName_TextBox = New System.Windows.Forms.TextBox()
        Me.Variables_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.Calculate_Button = New System.Windows.Forms.Button()
        Me.SaveButton = New System.Windows.Forms.Button()
        Me.SourceLevel_ComboBox = New System.Windows.Forms.ComboBox()
        Me.ViewVariables_Button = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.LevelSelection_GroupBox = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.LoadOstaTestSpecificationControl1 = New SpeechTestFramework.LoadOstaTestSpecificationControl()
        Me.GroupBox2.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.LevelSelection_GroupBox.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.TableLayoutPanel3)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox2.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(362, 109)
        Me.GroupBox2.TabIndex = 7
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Search for and then load a speech test"
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.ColumnCount = 2
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.64307!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.35693!))
        Me.TableLayoutPanel3.Controls.Add(Me.Label1, 0, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.LoadedSpeechMaterialName_TextBox, 1, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.LoadOstaTestSpecificationControl1, 0, 0)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 3
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(356, 90)
        Me.TableLayoutPanel3.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 63)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(131, 25)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Loaded speech material:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LoadedSpeechMaterialName_TextBox
        '
        Me.LoadedSpeechMaterialName_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LoadedSpeechMaterialName_TextBox.Location = New System.Drawing.Point(140, 66)
        Me.LoadedSpeechMaterialName_TextBox.Name = "LoadedSpeechMaterialName_TextBox"
        Me.LoadedSpeechMaterialName_TextBox.ReadOnly = True
        Me.LoadedSpeechMaterialName_TextBox.Size = New System.Drawing.Size(213, 20)
        Me.LoadedSpeechMaterialName_TextBox.TabIndex = 2
        Me.LoadedSpeechMaterialName_TextBox.Text = "No speech material loaded"
        '
        'Variables_TableLayoutPanel
        '
        Me.Variables_TableLayoutPanel.AutoScroll = True
        Me.Variables_TableLayoutPanel.ColumnCount = 1
        Me.TableLayoutPanel1.SetColumnSpan(Me.Variables_TableLayoutPanel, 2)
        Me.Variables_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Variables_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Variables_TableLayoutPanel.Location = New System.Drawing.Point(3, 118)
        Me.Variables_TableLayoutPanel.Name = "Variables_TableLayoutPanel"
        Me.Variables_TableLayoutPanel.RowCount = 1
        Me.Variables_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Variables_TableLayoutPanel.Size = New System.Drawing.Size(731, 371)
        Me.Variables_TableLayoutPanel.TabIndex = 8
        '
        'Calculate_Button
        '
        Me.Calculate_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Calculate_Button.Location = New System.Drawing.Point(3, 495)
        Me.Calculate_Button.Name = "Calculate_Button"
        Me.Calculate_Button.Size = New System.Drawing.Size(362, 34)
        Me.Calculate_Button.TabIndex = 9
        Me.Calculate_Button.Text = "Run summary calculations"
        Me.Calculate_Button.UseVisualStyleBackColor = True
        '
        'SaveButton
        '
        Me.SaveButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SaveButton.Location = New System.Drawing.Point(371, 495)
        Me.SaveButton.Name = "SaveButton"
        Me.SaveButton.Size = New System.Drawing.Size(363, 34)
        Me.SaveButton.TabIndex = 10
        Me.SaveButton.Text = "Save"
        Me.SaveButton.UseVisualStyleBackColor = True
        '
        'SourceLevel_ComboBox
        '
        Me.SourceLevel_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SourceLevel_ComboBox.FormattingEnabled = True
        Me.SourceLevel_ComboBox.Location = New System.Drawing.Point(112, 3)
        Me.SourceLevel_ComboBox.Name = "SourceLevel_ComboBox"
        Me.SourceLevel_ComboBox.Size = New System.Drawing.Size(242, 21)
        Me.SourceLevel_ComboBox.TabIndex = 11
        '
        'ViewVariables_Button
        '
        Me.TableLayoutPanel2.SetColumnSpan(Me.ViewVariables_Button, 2)
        Me.ViewVariables_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ViewVariables_Button.Location = New System.Drawing.Point(3, 29)
        Me.ViewVariables_Button.Name = "ViewVariables_Button"
        Me.ViewVariables_Button.Size = New System.Drawing.Size(351, 58)
        Me.ViewVariables_Button.TabIndex = 12
        Me.ViewVariables_Button.Text = "View variables"
        Me.ViewVariables_Button.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(3, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(103, 26)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "Select source level:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBox2, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Variables_TableLayoutPanel, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Calculate_Button, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.SaveButton, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.LevelSelection_GroupBox, 1, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 115.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(737, 532)
        Me.TableLayoutPanel1.TabIndex = 14
        '
        'LevelSelection_GroupBox
        '
        Me.LevelSelection_GroupBox.Controls.Add(Me.TableLayoutPanel2)
        Me.LevelSelection_GroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LevelSelection_GroupBox.Location = New System.Drawing.Point(371, 3)
        Me.LevelSelection_GroupBox.Name = "LevelSelection_GroupBox"
        Me.LevelSelection_GroupBox.Size = New System.Drawing.Size(363, 109)
        Me.LevelSelection_GroupBox.TabIndex = 11
        Me.LevelSelection_GroupBox.TabStop = False
        Me.LevelSelection_GroupBox.Text = "Select variable source level (and summarise to all higher levels)"
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.81232!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.18768!))
        Me.TableLayoutPanel2.Controls.Add(Me.ViewVariables_Button, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.SourceLevel_ComboBox, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.Label2, 0, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 2
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(357, 90)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'LoadOstaTestSpecificationControl1
        '
        Me.TableLayoutPanel3.SetColumnSpan(Me.LoadOstaTestSpecificationControl1, 2)
        Me.LoadOstaTestSpecificationControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LoadOstaTestSpecificationControl1.Location = New System.Drawing.Point(3, 3)
        Me.LoadOstaTestSpecificationControl1.Name = "LoadOstaTestSpecificationControl1"
        Me.LoadOstaTestSpecificationControl1.SelectedTestSpecification = Nothing
        Me.LoadOstaTestSpecificationControl1.Size = New System.Drawing.Size(350, 57)
        Me.LoadOstaTestSpecificationControl1.TabIndex = 3
        '
        'StatisticalSummaryControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "StatisticalSummaryControl"
        Me.Size = New System.Drawing.Size(737, 532)
        Me.GroupBox2.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TableLayoutPanel3.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.LevelSelection_GroupBox.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox2 As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel3 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents LoadedSpeechMaterialName_TextBox As Windows.Forms.TextBox
    Friend WithEvents Variables_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents Calculate_Button As Windows.Forms.Button
    Friend WithEvents SaveButton As Windows.Forms.Button
    Friend WithEvents SourceLevel_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents ViewVariables_Button As Windows.Forms.Button
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents LevelSelection_GroupBox As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel2 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents LoadOstaTestSpecificationControl1 As LoadOstaTestSpecificationControl
End Class
