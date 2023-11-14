<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OstfSoundPlayerWindow
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
        Me.Toplevel_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.AddSounds_Button = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Duration_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Play_AudioButton = New SpeechTestFramework.WinFormControls.AudioButton()
        Me.Stop_AudioButton = New SpeechTestFramework.WinFormControls.AudioButton()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.SoundSource_FlowLayoutPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.About_Button = New System.Windows.Forms.Button()
        Me.Transducer_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Toplevel_TableLayoutPanel.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SoundSource_FlowLayoutPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'Toplevel_TableLayoutPanel
        '
        Me.Toplevel_TableLayoutPanel.ColumnCount = 6
        Me.Toplevel_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155.0!))
        Me.Toplevel_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155.0!))
        Me.Toplevel_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155.0!))
        Me.Toplevel_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155.0!))
        Me.Toplevel_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155.0!))
        Me.Toplevel_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.Toplevel_TableLayoutPanel.Controls.Add(Me.AddSounds_Button, 0, 0)
        Me.Toplevel_TableLayoutPanel.Controls.Add(Me.Label1, 0, 2)
        Me.Toplevel_TableLayoutPanel.Controls.Add(Me.Duration_ComboBox, 1, 2)
        Me.Toplevel_TableLayoutPanel.Controls.Add(Me.Play_AudioButton, 2, 2)
        Me.Toplevel_TableLayoutPanel.Controls.Add(Me.Stop_AudioButton, 3, 2)
        Me.Toplevel_TableLayoutPanel.Controls.Add(Me.GroupBox1, 0, 1)
        Me.Toplevel_TableLayoutPanel.Controls.Add(Me.About_Button, 5, 0)
        Me.Toplevel_TableLayoutPanel.Controls.Add(Me.Transducer_ComboBox, 3, 0)
        Me.Toplevel_TableLayoutPanel.Controls.Add(Me.Label2, 1, 0)
        Me.Toplevel_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Toplevel_TableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.Toplevel_TableLayoutPanel.Name = "Toplevel_TableLayoutPanel"
        Me.Toplevel_TableLayoutPanel.RowCount = 3
        Me.Toplevel_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.Toplevel_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.Toplevel_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.Toplevel_TableLayoutPanel.Size = New System.Drawing.Size(1009, 422)
        Me.Toplevel_TableLayoutPanel.TabIndex = 1
        '
        'AddSounds_Button
        '
        Me.AddSounds_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AddSounds_Button.Enabled = False
        Me.AddSounds_Button.FlatAppearance.BorderSize = 2
        Me.AddSounds_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.AddSounds_Button.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AddSounds_Button.Location = New System.Drawing.Point(3, 3)
        Me.AddSounds_Button.Name = "AddSounds_Button"
        Me.AddSounds_Button.Size = New System.Drawing.Size(149, 34)
        Me.AddSounds_Button.TabIndex = 7
        Me.AddSounds_Button.Text = "Add sounds"
        Me.AddSounds_Button.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(3, 382)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(149, 40)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Play duration (s):"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Duration_ComboBox
        '
        Me.Duration_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Duration_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.Duration_ComboBox.Enabled = False
        Me.Duration_ComboBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Duration_ComboBox.FormattingEnabled = True
        Me.Duration_ComboBox.Location = New System.Drawing.Point(158, 385)
        Me.Duration_ComboBox.Name = "Duration_ComboBox"
        Me.Duration_ComboBox.Size = New System.Drawing.Size(149, 32)
        Me.Duration_ComboBox.TabIndex = 1
        '
        'Play_AudioButton
        '
        Me.Play_AudioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Play_AudioButton.Enabled = False
        Me.Play_AudioButton.Location = New System.Drawing.Point(313, 385)
        Me.Play_AudioButton.Name = "Play_AudioButton"
        Me.Play_AudioButton.Size = New System.Drawing.Size(149, 34)
        Me.Play_AudioButton.TabIndex = 2
        Me.Play_AudioButton.UseVisualStyleBackColor = True
        Me.Play_AudioButton.ViewMode = SpeechTestFramework.WinFormControls.AudioButton.ViewModes.Play
        '
        'Stop_AudioButton
        '
        Me.Stop_AudioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Stop_AudioButton.Enabled = False
        Me.Stop_AudioButton.Location = New System.Drawing.Point(468, 385)
        Me.Stop_AudioButton.Name = "Stop_AudioButton"
        Me.Stop_AudioButton.Size = New System.Drawing.Size(149, 34)
        Me.Stop_AudioButton.TabIndex = 3
        Me.Stop_AudioButton.UseVisualStyleBackColor = True
        Me.Stop_AudioButton.ViewMode = SpeechTestFramework.WinFormControls.AudioButton.ViewModes.[Stop]
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.Transparent
        Me.Toplevel_TableLayoutPanel.SetColumnSpan(Me.GroupBox1, 6)
        Me.GroupBox1.Controls.Add(Me.SoundSource_FlowLayoutPanel)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(3, 43)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(6)
        Me.GroupBox1.Size = New System.Drawing.Size(1003, 336)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        '
        'SoundSource_FlowLayoutPanel
        '
        Me.SoundSource_FlowLayoutPanel.AutoScroll = True
        Me.SoundSource_FlowLayoutPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SoundSource_FlowLayoutPanel.Controls.Add(Me.Button2)
        Me.SoundSource_FlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SoundSource_FlowLayoutPanel.Location = New System.Drawing.Point(6, 19)
        Me.SoundSource_FlowLayoutPanel.Name = "SoundSource_FlowLayoutPanel"
        Me.SoundSource_FlowLayoutPanel.Size = New System.Drawing.Size(991, 311)
        Me.SoundSource_FlowLayoutPanel.TabIndex = 0
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.SystemColors.Control
        Me.Button2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Button2.Enabled = False
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button2.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button2.Location = New System.Drawing.Point(3, 3)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(149, 0)
        Me.Button2.TabIndex = 8
        Me.Button2.Text = "Add sounds"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'About_Button
        '
        Me.About_Button.Dock = System.Windows.Forms.DockStyle.Right
        Me.About_Button.FlatAppearance.BorderSize = 2
        Me.About_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.About_Button.Location = New System.Drawing.Point(944, 3)
        Me.About_Button.Name = "About_Button"
        Me.About_Button.Size = New System.Drawing.Size(62, 34)
        Me.About_Button.TabIndex = 6
        Me.About_Button.Text = "About"
        Me.About_Button.UseVisualStyleBackColor = True
        '
        'Transducer_ComboBox
        '
        Me.Toplevel_TableLayoutPanel.SetColumnSpan(Me.Transducer_ComboBox, 2)
        Me.Transducer_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Transducer_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.Transducer_ComboBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Transducer_ComboBox.FormattingEnabled = True
        Me.Transducer_ComboBox.Location = New System.Drawing.Point(468, 3)
        Me.Transducer_ComboBox.Name = "Transducer_ComboBox"
        Me.Transducer_ComboBox.Size = New System.Drawing.Size(304, 32)
        Me.Transducer_ComboBox.TabIndex = 7
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Toplevel_TableLayoutPanel.SetColumnSpan(Me.Label2, 2)
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(158, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(304, 40)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Select audio output:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'OstfSoundPlayerWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1009, 422)
        Me.Controls.Add(Me.Toplevel_TableLayoutPanel)
        Me.MinimumSize = New System.Drawing.Size(980, 200)
        Me.Name = "OstfSoundPlayerWindow"
        Me.Text = "OSTF sound player"
        Me.Toplevel_TableLayoutPanel.ResumeLayout(False)
        Me.Toplevel_TableLayoutPanel.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.SoundSource_FlowLayoutPanel.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Toplevel_TableLayoutPanel As TableLayoutPanel
    Friend WithEvents Label1 As Label
    Friend WithEvents Duration_ComboBox As ComboBox
    Friend WithEvents Play_AudioButton As SpeechTestFramework.WinFormControls.AudioButton
    Friend WithEvents Stop_AudioButton As SpeechTestFramework.WinFormControls.AudioButton
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents SoundSource_FlowLayoutPanel As FlowLayoutPanel
    Friend WithEvents About_Button As Button
    Friend WithEvents Transducer_ComboBox As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents AddSounds_Button As Button
    Friend WithEvents Button2 As Button
End Class
