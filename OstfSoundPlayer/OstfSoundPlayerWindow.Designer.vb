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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Duration_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Play_AudioButton = New SpeechTestFramework.WinFormControls.AudioButton()
        Me.Stop_AudioButton = New SpeechTestFramework.WinFormControls.AudioButton()
        Me.AddSounds_Button = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.SoundSource_FlowLayoutPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.About_Button = New System.Windows.Forms.Button()
        Me.Transducer_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 5
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Duration_ComboBox, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Play_AudioButton, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Stop_AudioButton, 3, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.AddSounds_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBox1, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.About_Button, 4, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Transducer_ComboBox, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 1, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(1009, 422)
        Me.TableLayoutPanel1.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
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
        'AddSounds_Button
        '
        Me.AddSounds_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AddSounds_Button.Enabled = False
        Me.AddSounds_Button.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AddSounds_Button.Location = New System.Drawing.Point(3, 3)
        Me.AddSounds_Button.Name = "AddSounds_Button"
        Me.AddSounds_Button.Size = New System.Drawing.Size(149, 34)
        Me.AddSounds_Button.TabIndex = 4
        Me.AddSounds_Button.Text = "Add sounds"
        Me.AddSounds_Button.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.GroupBox1, 5)
        Me.GroupBox1.Controls.Add(Me.SoundSource_FlowLayoutPanel)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(3, 43)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(1003, 336)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        '
        'SoundSource_FlowLayoutPanel
        '
        Me.SoundSource_FlowLayoutPanel.AutoScroll = True
        Me.SoundSource_FlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SoundSource_FlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.SoundSource_FlowLayoutPanel.Location = New System.Drawing.Point(3, 16)
        Me.SoundSource_FlowLayoutPanel.Name = "SoundSource_FlowLayoutPanel"
        Me.SoundSource_FlowLayoutPanel.Size = New System.Drawing.Size(997, 317)
        Me.SoundSource_FlowLayoutPanel.TabIndex = 0
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
        Me.TableLayoutPanel1.SetColumnSpan(Me.Transducer_ComboBox, 2)
        Me.Transducer_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Transducer_ComboBox.FormattingEnabled = True
        Me.Transducer_ComboBox.Location = New System.Drawing.Point(313, 3)
        Me.Transducer_ComboBox.Name = "Transducer_ComboBox"
        Me.Transducer_ComboBox.Size = New System.Drawing.Size(304, 21)
        Me.Transducer_ComboBox.TabIndex = 7
        '
        'Label2
        '
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label2.Location = New System.Drawing.Point(158, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(149, 23)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Select audio output:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'OstfSoundPlayerWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1009, 422)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.MinimumSize = New System.Drawing.Size(980, 200)
        Me.Name = "OstfSoundPlayerWindow"
        Me.Text = "OSTF sound player"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents Label1 As Label
    Friend WithEvents Duration_ComboBox As ComboBox
    Friend WithEvents Play_AudioButton As SpeechTestFramework.WinFormControls.AudioButton
    Friend WithEvents Stop_AudioButton As SpeechTestFramework.WinFormControls.AudioButton
    Friend WithEvents AddSounds_Button As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents SoundSource_FlowLayoutPanel As FlowLayoutPanel
    Friend WithEvents About_Button As Button
    Friend WithEvents Transducer_ComboBox As ComboBox
    Friend WithEvents Label2 As Label
End Class
