<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AudiogramEditControl
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
        Me.Content_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.EditMode_GroupBox = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel6 = New System.Windows.Forms.TableLayoutPanel()
        Me.EnableEdit_RadioButton = New System.Windows.Forms.RadioButton()
        Me.DiableEdit_RadioButton = New System.Windows.Forms.RadioButton()
        Me.Misc_GroupBox = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.NotHeard_CheckBox = New System.Windows.Forms.CheckBox()
        Me.Overheard_CheckBox = New System.Windows.Forms.CheckBox()
        Me.Masked_CheckBox = New System.Windows.Forms.CheckBox()
        Me.Conduction_GroupBox = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
        Me.AirConduction_RadioButton = New System.Windows.Forms.RadioButton()
        Me.BoneConduction_RadioButton = New System.Windows.Forms.RadioButton()
        Me.Side_GroupBox = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
        Me.RightSide_RadioButton = New System.Windows.Forms.RadioButton()
        Me.LeftSide_RadioButton = New System.Windows.Forms.RadioButton()
        Me.Content_TableLayoutPanel.SuspendLayout()
        Me.EditMode_GroupBox.SuspendLayout()
        Me.TableLayoutPanel6.SuspendLayout()
        Me.Misc_GroupBox.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.Conduction_GroupBox.SuspendLayout()
        Me.TableLayoutPanel4.SuspendLayout()
        Me.Side_GroupBox.SuspendLayout()
        Me.TableLayoutPanel5.SuspendLayout()
        Me.SuspendLayout()
        '
        'Content_TableLayoutPanel
        '
        Me.Content_TableLayoutPanel.ColumnCount = 1
        Me.Content_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.Content_TableLayoutPanel.Controls.Add(Me.EditMode_GroupBox, 0, 0)
        Me.Content_TableLayoutPanel.Controls.Add(Me.Misc_GroupBox, 0, 3)
        Me.Content_TableLayoutPanel.Controls.Add(Me.Conduction_GroupBox, 0, 2)
        Me.Content_TableLayoutPanel.Controls.Add(Me.Side_GroupBox, 0, 1)
        Me.Content_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Content_TableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.Content_TableLayoutPanel.Name = "Content_TableLayoutPanel"
        Me.Content_TableLayoutPanel.RowCount = 5
        Me.Content_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50.0!))
        Me.Content_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50.0!))
        Me.Content_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50.0!))
        Me.Content_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50.0!))
        Me.Content_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.Content_TableLayoutPanel.Size = New System.Drawing.Size(461, 200)
        Me.Content_TableLayoutPanel.TabIndex = 2
        '
        'EditMode_GroupBox
        '
        Me.EditMode_GroupBox.Controls.Add(Me.TableLayoutPanel6)
        Me.EditMode_GroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.EditMode_GroupBox.Location = New System.Drawing.Point(3, 3)
        Me.EditMode_GroupBox.Name = "EditMode_GroupBox"
        Me.EditMode_GroupBox.Size = New System.Drawing.Size(455, 44)
        Me.EditMode_GroupBox.TabIndex = 3
        Me.EditMode_GroupBox.TabStop = False
        Me.EditMode_GroupBox.Text = "Edit mode"
        '
        'TableLayoutPanel6
        '
        Me.TableLayoutPanel6.ColumnCount = 2
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel6.Controls.Add(Me.EnableEdit_RadioButton, 0, 0)
        Me.TableLayoutPanel6.Controls.Add(Me.DiableEdit_RadioButton, 1, 0)
        Me.TableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel6.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel6.Name = "TableLayoutPanel6"
        Me.TableLayoutPanel6.RowCount = 1
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel6.Size = New System.Drawing.Size(449, 25)
        Me.TableLayoutPanel6.TabIndex = 2
        '
        'EnableEdit_RadioButton
        '
        Me.EnableEdit_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.EnableEdit_RadioButton.Location = New System.Drawing.Point(3, 3)
        Me.EnableEdit_RadioButton.Name = "EnableEdit_RadioButton"
        Me.EnableEdit_RadioButton.Size = New System.Drawing.Size(218, 19)
        Me.EnableEdit_RadioButton.TabIndex = 0
        Me.EnableEdit_RadioButton.Text = "Enable editing"
        Me.EnableEdit_RadioButton.UseVisualStyleBackColor = True
        '
        'DiableEdit_RadioButton
        '
        Me.DiableEdit_RadioButton.Checked = True
        Me.DiableEdit_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DiableEdit_RadioButton.Location = New System.Drawing.Point(227, 3)
        Me.DiableEdit_RadioButton.Name = "DiableEdit_RadioButton"
        Me.DiableEdit_RadioButton.Size = New System.Drawing.Size(219, 19)
        Me.DiableEdit_RadioButton.TabIndex = 1
        Me.DiableEdit_RadioButton.TabStop = True
        Me.DiableEdit_RadioButton.Text = "Disable editing"
        Me.DiableEdit_RadioButton.UseVisualStyleBackColor = True
        '
        'Misc_GroupBox
        '
        Me.Misc_GroupBox.Controls.Add(Me.TableLayoutPanel3)
        Me.Misc_GroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Misc_GroupBox.Enabled = False
        Me.Misc_GroupBox.Location = New System.Drawing.Point(3, 153)
        Me.Misc_GroupBox.Name = "Misc_GroupBox"
        Me.Misc_GroupBox.Size = New System.Drawing.Size(455, 44)
        Me.Misc_GroupBox.TabIndex = 2
        Me.Misc_GroupBox.TabStop = False
        Me.Misc_GroupBox.Text = "Miscellaneous"
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.ColumnCount = 3
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel3.Controls.Add(Me.NotHeard_CheckBox, 0, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.Overheard_CheckBox, 1, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.Masked_CheckBox, 0, 0)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 1
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(449, 25)
        Me.TableLayoutPanel3.TabIndex = 2
        '
        'NotHeard_CheckBox
        '
        Me.NotHeard_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NotHeard_CheckBox.Location = New System.Drawing.Point(152, 3)
        Me.NotHeard_CheckBox.Name = "NotHeard_CheckBox"
        Me.NotHeard_CheckBox.Size = New System.Drawing.Size(143, 19)
        Me.NotHeard_CheckBox.TabIndex = 2
        Me.NotHeard_CheckBox.Text = "Not heard"
        Me.NotHeard_CheckBox.UseVisualStyleBackColor = True
        '
        'Overheard_CheckBox
        '
        Me.Overheard_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Overheard_CheckBox.Location = New System.Drawing.Point(301, 3)
        Me.Overheard_CheckBox.Name = "Overheard_CheckBox"
        Me.Overheard_CheckBox.Size = New System.Drawing.Size(145, 19)
        Me.Overheard_CheckBox.TabIndex = 1
        Me.Overheard_CheckBox.Text = "Overheard"
        Me.Overheard_CheckBox.UseVisualStyleBackColor = True
        '
        'Masked_CheckBox
        '
        Me.Masked_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Masked_CheckBox.Location = New System.Drawing.Point(3, 3)
        Me.Masked_CheckBox.Name = "Masked_CheckBox"
        Me.Masked_CheckBox.Size = New System.Drawing.Size(143, 19)
        Me.Masked_CheckBox.TabIndex = 0
        Me.Masked_CheckBox.Text = "Masked"
        Me.Masked_CheckBox.UseVisualStyleBackColor = True
        '
        'Conduction_GroupBox
        '
        Me.Conduction_GroupBox.Controls.Add(Me.TableLayoutPanel4)
        Me.Conduction_GroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Conduction_GroupBox.Enabled = False
        Me.Conduction_GroupBox.Location = New System.Drawing.Point(3, 103)
        Me.Conduction_GroupBox.Name = "Conduction_GroupBox"
        Me.Conduction_GroupBox.Size = New System.Drawing.Size(455, 44)
        Me.Conduction_GroupBox.TabIndex = 1
        Me.Conduction_GroupBox.TabStop = False
        Me.Conduction_GroupBox.Text = "Conduction"
        '
        'TableLayoutPanel4
        '
        Me.TableLayoutPanel4.ColumnCount = 2
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.Controls.Add(Me.AirConduction_RadioButton, 0, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.BoneConduction_RadioButton, 1, 0)
        Me.TableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel4.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
        Me.TableLayoutPanel4.RowCount = 1
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(449, 25)
        Me.TableLayoutPanel4.TabIndex = 4
        '
        'AirConduction_RadioButton
        '
        Me.AirConduction_RadioButton.Checked = True
        Me.AirConduction_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AirConduction_RadioButton.Location = New System.Drawing.Point(3, 3)
        Me.AirConduction_RadioButton.Name = "AirConduction_RadioButton"
        Me.AirConduction_RadioButton.Size = New System.Drawing.Size(218, 19)
        Me.AirConduction_RadioButton.TabIndex = 2
        Me.AirConduction_RadioButton.TabStop = True
        Me.AirConduction_RadioButton.Text = "Air"
        Me.AirConduction_RadioButton.UseVisualStyleBackColor = True
        '
        'BoneConduction_RadioButton
        '
        Me.BoneConduction_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BoneConduction_RadioButton.Location = New System.Drawing.Point(227, 3)
        Me.BoneConduction_RadioButton.Name = "BoneConduction_RadioButton"
        Me.BoneConduction_RadioButton.Size = New System.Drawing.Size(219, 19)
        Me.BoneConduction_RadioButton.TabIndex = 3
        Me.BoneConduction_RadioButton.TabStop = True
        Me.BoneConduction_RadioButton.Text = "Bone"
        Me.BoneConduction_RadioButton.UseVisualStyleBackColor = True
        '
        'Side_GroupBox
        '
        Me.Side_GroupBox.Controls.Add(Me.TableLayoutPanel5)
        Me.Side_GroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Side_GroupBox.Enabled = False
        Me.Side_GroupBox.Location = New System.Drawing.Point(3, 53)
        Me.Side_GroupBox.Name = "Side_GroupBox"
        Me.Side_GroupBox.Size = New System.Drawing.Size(455, 44)
        Me.Side_GroupBox.TabIndex = 0
        Me.Side_GroupBox.TabStop = False
        Me.Side_GroupBox.Text = "Side"
        '
        'TableLayoutPanel5
        '
        Me.TableLayoutPanel5.ColumnCount = 2
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel5.Controls.Add(Me.RightSide_RadioButton, 0, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.LeftSide_RadioButton, 1, 0)
        Me.TableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 1
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(449, 25)
        Me.TableLayoutPanel5.TabIndex = 2
        '
        'RightSide_RadioButton
        '
        Me.RightSide_RadioButton.Checked = True
        Me.RightSide_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RightSide_RadioButton.Location = New System.Drawing.Point(3, 3)
        Me.RightSide_RadioButton.Name = "RightSide_RadioButton"
        Me.RightSide_RadioButton.Size = New System.Drawing.Size(218, 19)
        Me.RightSide_RadioButton.TabIndex = 1
        Me.RightSide_RadioButton.TabStop = True
        Me.RightSide_RadioButton.Text = "Right"
        Me.RightSide_RadioButton.UseVisualStyleBackColor = True
        '
        'LeftSide_RadioButton
        '
        Me.LeftSide_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LeftSide_RadioButton.Location = New System.Drawing.Point(227, 3)
        Me.LeftSide_RadioButton.Name = "LeftSide_RadioButton"
        Me.LeftSide_RadioButton.Size = New System.Drawing.Size(219, 19)
        Me.LeftSide_RadioButton.TabIndex = 0
        Me.LeftSide_RadioButton.Text = "Left"
        Me.LeftSide_RadioButton.UseVisualStyleBackColor = True
        '
        'AudiogramEditControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Content_TableLayoutPanel)
        Me.Name = "AudiogramEditControl"
        Me.Size = New System.Drawing.Size(461, 200)
        Me.Content_TableLayoutPanel.ResumeLayout(False)
        Me.EditMode_GroupBox.ResumeLayout(False)
        Me.TableLayoutPanel6.ResumeLayout(False)
        Me.Misc_GroupBox.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.Conduction_GroupBox.ResumeLayout(False)
        Me.TableLayoutPanel4.ResumeLayout(False)
        Me.Side_GroupBox.ResumeLayout(False)
        Me.TableLayoutPanel5.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Content_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents EditMode_GroupBox As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel6 As Windows.Forms.TableLayoutPanel
    Friend WithEvents EnableEdit_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents DiableEdit_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents Misc_GroupBox As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel3 As Windows.Forms.TableLayoutPanel
    Friend WithEvents NotHeard_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Overheard_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Masked_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Conduction_GroupBox As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel4 As Windows.Forms.TableLayoutPanel
    Friend WithEvents AirConduction_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents BoneConduction_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents Side_GroupBox As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel5 As Windows.Forms.TableLayoutPanel
    Friend WithEvents LeftSide_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents RightSide_RadioButton As Windows.Forms.RadioButton
End Class
