<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AudiogramSymbolDialog
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
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
        Me.LeftSide_RadioButton = New System.Windows.Forms.RadioButton()
        Me.RightSide_RadioButton = New System.Windows.Forms.RadioButton()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.TableLayoutPanel6.SuspendLayout()
        Me.Misc_GroupBox.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.Conduction_GroupBox.SuspendLayout()
        Me.TableLayoutPanel4.SuspendLayout()
        Me.Side_GroupBox.SuspendLayout()
        Me.TableLayoutPanel5.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(277, 267)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 1
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.GroupBox4, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.Misc_GroupBox, 0, 3)
        Me.TableLayoutPanel2.Controls.Add(Me.Conduction_GroupBox, 0, 2)
        Me.TableLayoutPanel2.Controls.Add(Me.Side_GroupBox, 0, 1)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 4
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(435, 254)
        Me.TableLayoutPanel2.TabIndex = 1
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.TableLayoutPanel6)
        Me.GroupBox4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox4.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(429, 57)
        Me.GroupBox4.TabIndex = 3
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Edit mode"
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
        Me.TableLayoutPanel6.Size = New System.Drawing.Size(423, 38)
        Me.TableLayoutPanel6.TabIndex = 2
        '
        'EnableEdit_RadioButton
        '
        Me.EnableEdit_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.EnableEdit_RadioButton.Location = New System.Drawing.Point(3, 3)
        Me.EnableEdit_RadioButton.Name = "EnableEdit_RadioButton"
        Me.EnableEdit_RadioButton.Size = New System.Drawing.Size(205, 32)
        Me.EnableEdit_RadioButton.TabIndex = 0
        Me.EnableEdit_RadioButton.Text = "Enable editing"
        Me.EnableEdit_RadioButton.UseVisualStyleBackColor = True
        '
        'DiableEdit_RadioButton
        '
        Me.DiableEdit_RadioButton.Checked = True
        Me.DiableEdit_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DiableEdit_RadioButton.Location = New System.Drawing.Point(214, 3)
        Me.DiableEdit_RadioButton.Name = "DiableEdit_RadioButton"
        Me.DiableEdit_RadioButton.Size = New System.Drawing.Size(206, 32)
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
        Me.Misc_GroupBox.Location = New System.Drawing.Point(3, 192)
        Me.Misc_GroupBox.Name = "Misc_GroupBox"
        Me.Misc_GroupBox.Size = New System.Drawing.Size(429, 59)
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
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(423, 40)
        Me.TableLayoutPanel3.TabIndex = 2
        '
        'NotHeard_CheckBox
        '
        Me.NotHeard_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NotHeard_CheckBox.Location = New System.Drawing.Point(144, 3)
        Me.NotHeard_CheckBox.Name = "NotHeard_CheckBox"
        Me.NotHeard_CheckBox.Size = New System.Drawing.Size(135, 34)
        Me.NotHeard_CheckBox.TabIndex = 2
        Me.NotHeard_CheckBox.Text = "Not heard"
        Me.NotHeard_CheckBox.UseVisualStyleBackColor = True
        '
        'Overheard_CheckBox
        '
        Me.Overheard_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Overheard_CheckBox.Location = New System.Drawing.Point(285, 3)
        Me.Overheard_CheckBox.Name = "Overheard_CheckBox"
        Me.Overheard_CheckBox.Size = New System.Drawing.Size(135, 34)
        Me.Overheard_CheckBox.TabIndex = 1
        Me.Overheard_CheckBox.Text = "Overheard"
        Me.Overheard_CheckBox.UseVisualStyleBackColor = True
        '
        'Masked_CheckBox
        '
        Me.Masked_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Masked_CheckBox.Location = New System.Drawing.Point(3, 3)
        Me.Masked_CheckBox.Name = "Masked_CheckBox"
        Me.Masked_CheckBox.Size = New System.Drawing.Size(135, 34)
        Me.Masked_CheckBox.TabIndex = 0
        Me.Masked_CheckBox.Text = "Masked"
        Me.Masked_CheckBox.UseVisualStyleBackColor = True
        '
        'Conduction_GroupBox
        '
        Me.Conduction_GroupBox.Controls.Add(Me.TableLayoutPanel4)
        Me.Conduction_GroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Conduction_GroupBox.Enabled = False
        Me.Conduction_GroupBox.Location = New System.Drawing.Point(3, 129)
        Me.Conduction_GroupBox.Name = "Conduction_GroupBox"
        Me.Conduction_GroupBox.Size = New System.Drawing.Size(429, 57)
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
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(423, 38)
        Me.TableLayoutPanel4.TabIndex = 4
        '
        'AirConduction_RadioButton
        '
        Me.AirConduction_RadioButton.Checked = True
        Me.AirConduction_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AirConduction_RadioButton.Location = New System.Drawing.Point(3, 3)
        Me.AirConduction_RadioButton.Name = "AirConduction_RadioButton"
        Me.AirConduction_RadioButton.Size = New System.Drawing.Size(205, 32)
        Me.AirConduction_RadioButton.TabIndex = 2
        Me.AirConduction_RadioButton.TabStop = True
        Me.AirConduction_RadioButton.Text = "Air"
        Me.AirConduction_RadioButton.UseVisualStyleBackColor = True
        '
        'BoneConduction_RadioButton
        '
        Me.BoneConduction_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BoneConduction_RadioButton.Location = New System.Drawing.Point(214, 3)
        Me.BoneConduction_RadioButton.Name = "BoneConduction_RadioButton"
        Me.BoneConduction_RadioButton.Size = New System.Drawing.Size(206, 32)
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
        Me.Side_GroupBox.Location = New System.Drawing.Point(3, 66)
        Me.Side_GroupBox.Name = "Side_GroupBox"
        Me.Side_GroupBox.Size = New System.Drawing.Size(429, 57)
        Me.Side_GroupBox.TabIndex = 0
        Me.Side_GroupBox.TabStop = False
        Me.Side_GroupBox.Text = "Side"
        '
        'TableLayoutPanel5
        '
        Me.TableLayoutPanel5.ColumnCount = 2
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel5.Controls.Add(Me.LeftSide_RadioButton, 0, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.RightSide_RadioButton, 1, 0)
        Me.TableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 1
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(423, 38)
        Me.TableLayoutPanel5.TabIndex = 2
        '
        'LeftSide_RadioButton
        '
        Me.LeftSide_RadioButton.Checked = True
        Me.LeftSide_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LeftSide_RadioButton.Location = New System.Drawing.Point(3, 3)
        Me.LeftSide_RadioButton.Name = "LeftSide_RadioButton"
        Me.LeftSide_RadioButton.Size = New System.Drawing.Size(205, 32)
        Me.LeftSide_RadioButton.TabIndex = 0
        Me.LeftSide_RadioButton.TabStop = True
        Me.LeftSide_RadioButton.Text = "Left"
        Me.LeftSide_RadioButton.UseVisualStyleBackColor = True
        '
        'RightSide_RadioButton
        '
        Me.RightSide_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RightSide_RadioButton.Location = New System.Drawing.Point(214, 3)
        Me.RightSide_RadioButton.Name = "RightSide_RadioButton"
        Me.RightSide_RadioButton.Size = New System.Drawing.Size(206, 32)
        Me.RightSide_RadioButton.TabIndex = 1
        Me.RightSide_RadioButton.TabStop = True
        Me.RightSide_RadioButton.Text = "Right"
        Me.RightSide_RadioButton.UseVisualStyleBackColor = True
        '
        'AudiogramSymbolDialog
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(435, 308)
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AudiogramSymbolDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Select Audiogram Editing Mode"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.TableLayoutPanel6.ResumeLayout(False)
        Me.Misc_GroupBox.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.Conduction_GroupBox.ResumeLayout(False)
        Me.TableLayoutPanel4.ResumeLayout(False)
        Me.Side_GroupBox.ResumeLayout(False)
        Me.TableLayoutPanel5.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanel2 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Misc_GroupBox As Windows.Forms.GroupBox
    Friend WithEvents Conduction_GroupBox As Windows.Forms.GroupBox
    Friend WithEvents Side_GroupBox As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel3 As Windows.Forms.TableLayoutPanel
    Friend WithEvents NotHeard_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Overheard_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Masked_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents TableLayoutPanel4 As Windows.Forms.TableLayoutPanel
    Friend WithEvents AirConduction_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents BoneConduction_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents TableLayoutPanel5 As Windows.Forms.TableLayoutPanel
    Friend WithEvents LeftSide_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents RightSide_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents GroupBox4 As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel6 As Windows.Forms.TableLayoutPanel
    Friend WithEvents EnableEdit_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents DiableEdit_RadioButton As Windows.Forms.RadioButton
End Class
