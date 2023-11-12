<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OstfSoundPlayerSourceControl
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
        Me.Channel_Label = New System.Windows.Forms.Label()
        Me.SoundSource_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Level_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Repeat_CheckBox = New System.Windows.Forms.CheckBox()
        Me.Remove_Button = New System.Windows.Forms.Button()
        Me.FileName_TextBox = New System.Windows.Forms.TextBox()
        Me.Content_TableLayoutPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'Content_TableLayoutPanel
        '
        Me.Content_TableLayoutPanel.ColumnCount = 6
        Me.Content_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667!))
        Me.Content_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667!))
        Me.Content_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667!))
        Me.Content_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667!))
        Me.Content_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667!))
        Me.Content_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667!))
        Me.Content_TableLayoutPanel.Controls.Add(Me.Channel_Label, 1, 0)
        Me.Content_TableLayoutPanel.Controls.Add(Me.SoundSource_ComboBox, 2, 0)
        Me.Content_TableLayoutPanel.Controls.Add(Me.Level_ComboBox, 3, 0)
        Me.Content_TableLayoutPanel.Controls.Add(Me.Repeat_CheckBox, 4, 0)
        Me.Content_TableLayoutPanel.Controls.Add(Me.Remove_Button, 5, 0)
        Me.Content_TableLayoutPanel.Controls.Add(Me.FileName_TextBox, 0, 0)
        Me.Content_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Content_TableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.Content_TableLayoutPanel.Margin = New System.Windows.Forms.Padding(0)
        Me.Content_TableLayoutPanel.Name = "Content_TableLayoutPanel"
        Me.Content_TableLayoutPanel.RowCount = 1
        Me.Content_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.Content_TableLayoutPanel.Size = New System.Drawing.Size(900, 30)
        Me.Content_TableLayoutPanel.TabIndex = 2
        '
        'Channel_Label
        '
        Me.Channel_Label.BackColor = System.Drawing.SystemColors.Control
        Me.Channel_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Channel_Label.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Channel_Label.Location = New System.Drawing.Point(153, 0)
        Me.Channel_Label.Name = "Channel_Label"
        Me.Channel_Label.Size = New System.Drawing.Size(144, 30)
        Me.Channel_Label.TabIndex = 1
        Me.Channel_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'SoundSource_ComboBox
        '
        Me.SoundSource_ComboBox.BackColor = System.Drawing.SystemColors.Control
        Me.SoundSource_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SoundSource_ComboBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SoundSource_ComboBox.FormattingEnabled = True
        Me.SoundSource_ComboBox.Location = New System.Drawing.Point(303, 3)
        Me.SoundSource_ComboBox.Name = "SoundSource_ComboBox"
        Me.SoundSource_ComboBox.Size = New System.Drawing.Size(144, 24)
        Me.SoundSource_ComboBox.TabIndex = 2
        '
        'Level_ComboBox
        '
        Me.Level_ComboBox.BackColor = System.Drawing.SystemColors.Control
        Me.Level_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Level_ComboBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Level_ComboBox.FormattingEnabled = True
        Me.Level_ComboBox.Location = New System.Drawing.Point(453, 3)
        Me.Level_ComboBox.Name = "Level_ComboBox"
        Me.Level_ComboBox.Size = New System.Drawing.Size(144, 24)
        Me.Level_ComboBox.TabIndex = 5
        '
        'Repeat_CheckBox
        '
        Me.Repeat_CheckBox.AutoSize = True
        Me.Repeat_CheckBox.BackColor = System.Drawing.SystemColors.Control
        Me.Repeat_CheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.Repeat_CheckBox.Checked = True
        Me.Repeat_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.Repeat_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Repeat_CheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Repeat_CheckBox.Location = New System.Drawing.Point(603, 3)
        Me.Repeat_CheckBox.Name = "Repeat_CheckBox"
        Me.Repeat_CheckBox.Padding = New System.Windows.Forms.Padding(5, 0, 0, 0)
        Me.Repeat_CheckBox.Size = New System.Drawing.Size(144, 24)
        Me.Repeat_CheckBox.TabIndex = 6
        Me.Repeat_CheckBox.UseVisualStyleBackColor = False
        '
        'Remove_Button
        '
        Me.Remove_Button.BackColor = System.Drawing.SystemColors.Control
        Me.Remove_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Remove_Button.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Remove_Button.Location = New System.Drawing.Point(753, 3)
        Me.Remove_Button.Name = "Remove_Button"
        Me.Remove_Button.Size = New System.Drawing.Size(144, 24)
        Me.Remove_Button.TabIndex = 7
        Me.Remove_Button.Text = "Remove"
        Me.Remove_Button.UseVisualStyleBackColor = False
        '
        'FileName_TextBox
        '
        Me.FileName_TextBox.BackColor = System.Drawing.SystemColors.Control
        Me.FileName_TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.FileName_TextBox.Cursor = System.Windows.Forms.Cursors.Default
        Me.FileName_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FileName_TextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FileName_TextBox.Location = New System.Drawing.Point(0, 7)
        Me.FileName_TextBox.Margin = New System.Windows.Forms.Padding(0, 7, 0, 0)
        Me.FileName_TextBox.Name = "FileName_TextBox"
        Me.FileName_TextBox.ReadOnly = True
        Me.FileName_TextBox.Size = New System.Drawing.Size(150, 16)
        Me.FileName_TextBox.TabIndex = 8
        Me.FileName_TextBox.WordWrap = False
        '
        'OstfSoundPlayerSourceControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Content_TableLayoutPanel)
        Me.Margin = New System.Windows.Forms.Padding(0)
        Me.Name = "OstfSoundPlayerSourceControl"
        Me.Size = New System.Drawing.Size(900, 30)
        Me.Content_TableLayoutPanel.ResumeLayout(False)
        Me.Content_TableLayoutPanel.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Content_TableLayoutPanel As TableLayoutPanel
    Friend WithEvents Channel_Label As Label
    Friend WithEvents SoundSource_ComboBox As ComboBox
    Friend WithEvents Level_ComboBox As ComboBox
    Friend WithEvents Repeat_CheckBox As CheckBox
    Friend WithEvents Remove_Button As Button
    Friend WithEvents FileName_TextBox As TextBox
End Class
