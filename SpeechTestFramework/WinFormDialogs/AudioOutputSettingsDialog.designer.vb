<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class AudioOutputSettingsDialog
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.settingsTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.audioSettingsTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.sampleRateLabel = New System.Windows.Forms.Label()
        Me.driverTypeLabel = New System.Windows.Forms.Label()
        Me.driverTypeComboBox = New System.Windows.Forms.ComboBox()
        Me.sampleRateComboBox = New System.Windows.Forms.ComboBox()
        Me.audioSettingsPanel = New System.Windows.Forms.Panel()
        Me.settingsGroupBox = New System.Windows.Forms.GroupBox()
        Me.inputTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.outputDeviceComboBox = New System.Windows.Forms.ComboBox()
        Me.outputDeviceLabel = New System.Windows.Forms.Label()
        Me.bufferSizeComboBox = New System.Windows.Forms.ComboBox()
        Me.bufferSizeLabel = New System.Windows.Forms.Label()
        Me.deviceSettingsButton = New System.Windows.Forms.Button()
        Me.latencyLabel = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.OutputChannelCountLabel = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.settingsTableLayoutPanel.SuspendLayout()
        Me.audioSettingsTableLayoutPanel.SuspendLayout()
        Me.audioSettingsPanel.SuspendLayout()
        Me.settingsGroupBox.SuspendLayout()
        Me.inputTableLayoutPanel.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
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
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(259, 242)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 25)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 19)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 19)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'settingsTableLayoutPanel
        '
        Me.settingsTableLayoutPanel.ColumnCount = 1
        Me.settingsTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.settingsTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.settingsTableLayoutPanel.Controls.Add(Me.audioSettingsTableLayoutPanel, 0, 0)
        Me.settingsTableLayoutPanel.Controls.Add(Me.TableLayoutPanel1, 0, 1)
        Me.settingsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.settingsTableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.settingsTableLayoutPanel.Name = "settingsTableLayoutPanel"
        Me.settingsTableLayoutPanel.RowCount = 2
        Me.settingsTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88.57143!))
        Me.settingsTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.42857!))
        Me.settingsTableLayoutPanel.Size = New System.Drawing.Size(408, 270)
        Me.settingsTableLayoutPanel.TabIndex = 2
        '
        'audioSettingsTableLayoutPanel
        '
        Me.audioSettingsTableLayoutPanel.AutoSize = True
        Me.audioSettingsTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.audioSettingsTableLayoutPanel.ColumnCount = 2
        Me.audioSettingsTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.audioSettingsTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.audioSettingsTableLayoutPanel.Controls.Add(Me.sampleRateLabel, 0, 2)
        Me.audioSettingsTableLayoutPanel.Controls.Add(Me.driverTypeLabel, 0, 0)
        Me.audioSettingsTableLayoutPanel.Controls.Add(Me.driverTypeComboBox, 1, 0)
        Me.audioSettingsTableLayoutPanel.Controls.Add(Me.sampleRateComboBox, 1, 2)
        Me.audioSettingsTableLayoutPanel.Controls.Add(Me.audioSettingsPanel, 1, 1)
        Me.audioSettingsTableLayoutPanel.Controls.Add(Me.latencyLabel, 1, 3)
        Me.audioSettingsTableLayoutPanel.Controls.Add(Me.GroupBox1, 0, 1)
        Me.audioSettingsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.audioSettingsTableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.audioSettingsTableLayoutPanel.Margin = New System.Windows.Forms.Padding(0)
        Me.audioSettingsTableLayoutPanel.Name = "audioSettingsTableLayoutPanel"
        Me.audioSettingsTableLayoutPanel.RowCount = 4
        Me.audioSettingsTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 37.5!))
        Me.audioSettingsTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 158.0!))
        Me.audioSettingsTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34.88372!))
        Me.audioSettingsTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 27.90698!))
        Me.audioSettingsTableLayoutPanel.Size = New System.Drawing.Size(408, 239)
        Me.audioSettingsTableLayoutPanel.TabIndex = 4
        '
        'sampleRateLabel
        '
        Me.sampleRateLabel.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.sampleRateLabel.AutoSize = True
        Me.sampleRateLabel.Location = New System.Drawing.Point(26, 195)
        Me.sampleRateLabel.Name = "sampleRateLabel"
        Me.sampleRateLabel.Size = New System.Drawing.Size(66, 13)
        Me.sampleRateLabel.TabIndex = 3
        Me.sampleRateLabel.Text = "Sample rate:"
        '
        'driverTypeLabel
        '
        Me.driverTypeLabel.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.driverTypeLabel.AutoSize = True
        Me.driverTypeLabel.Location = New System.Drawing.Point(3, 8)
        Me.driverTypeLabel.Name = "driverTypeLabel"
        Me.driverTypeLabel.Size = New System.Drawing.Size(89, 13)
        Me.driverTypeLabel.TabIndex = 0
        Me.driverTypeLabel.Text = "Audio driver type:"
        '
        'driverTypeComboBox
        '
        Me.driverTypeComboBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.driverTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.driverTypeComboBox.FormattingEnabled = True
        Me.driverTypeComboBox.Location = New System.Drawing.Point(100, 5)
        Me.driverTypeComboBox.Margin = New System.Windows.Forms.Padding(5)
        Me.driverTypeComboBox.Name = "driverTypeComboBox"
        Me.driverTypeComboBox.Size = New System.Drawing.Size(303, 21)
        Me.driverTypeComboBox.TabIndex = 1
        '
        'sampleRateComboBox
        '
        Me.sampleRateComboBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.sampleRateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.sampleRateComboBox.FormattingEnabled = True
        Me.sampleRateComboBox.Location = New System.Drawing.Point(100, 193)
        Me.sampleRateComboBox.Margin = New System.Windows.Forms.Padding(5)
        Me.sampleRateComboBox.Name = "sampleRateComboBox"
        Me.sampleRateComboBox.Size = New System.Drawing.Size(303, 21)
        Me.sampleRateComboBox.TabIndex = 4
        '
        'audioSettingsPanel
        '
        Me.audioSettingsPanel.AutoSize = True
        Me.audioSettingsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.audioSettingsPanel.Controls.Add(Me.settingsGroupBox)
        Me.audioSettingsPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.audioSettingsPanel.Location = New System.Drawing.Point(98, 33)
        Me.audioSettingsPanel.Name = "audioSettingsPanel"
        Me.audioSettingsPanel.Size = New System.Drawing.Size(307, 152)
        Me.audioSettingsPanel.TabIndex = 5
        '
        'settingsGroupBox
        '
        Me.settingsGroupBox.Controls.Add(Me.inputTableLayoutPanel)
        Me.settingsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.settingsGroupBox.Location = New System.Drawing.Point(0, 0)
        Me.settingsGroupBox.Name = "settingsGroupBox"
        Me.settingsGroupBox.Size = New System.Drawing.Size(307, 152)
        Me.settingsGroupBox.TabIndex = 4
        Me.settingsGroupBox.TabStop = False
        Me.settingsGroupBox.Text = "MME settings"
        '
        'inputTableLayoutPanel
        '
        Me.inputTableLayoutPanel.ColumnCount = 3
        Me.inputTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15.0!))
        Me.inputTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.inputTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15.0!))
        Me.inputTableLayoutPanel.Controls.Add(Me.outputDeviceComboBox, 1, 1)
        Me.inputTableLayoutPanel.Controls.Add(Me.outputDeviceLabel, 1, 0)
        Me.inputTableLayoutPanel.Controls.Add(Me.bufferSizeComboBox, 1, 3)
        Me.inputTableLayoutPanel.Controls.Add(Me.bufferSizeLabel, 1, 2)
        Me.inputTableLayoutPanel.Controls.Add(Me.deviceSettingsButton, 1, 4)
        Me.inputTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.inputTableLayoutPanel.Location = New System.Drawing.Point(3, 16)
        Me.inputTableLayoutPanel.Name = "inputTableLayoutPanel"
        Me.inputTableLayoutPanel.RowCount = 5
        Me.inputTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.inputTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.inputTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.inputTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.inputTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.inputTableLayoutPanel.Size = New System.Drawing.Size(301, 133)
        Me.inputTableLayoutPanel.TabIndex = 0
        '
        'outputDeviceComboBox
        '
        Me.outputDeviceComboBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.outputDeviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.outputDeviceComboBox.FormattingEnabled = True
        Me.outputDeviceComboBox.Location = New System.Drawing.Point(18, 29)
        Me.outputDeviceComboBox.Name = "outputDeviceComboBox"
        Me.outputDeviceComboBox.Size = New System.Drawing.Size(265, 21)
        Me.outputDeviceComboBox.TabIndex = 2
        '
        'outputDeviceLabel
        '
        Me.outputDeviceLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.outputDeviceLabel.Location = New System.Drawing.Point(18, 8)
        Me.outputDeviceLabel.Name = "outputDeviceLabel"
        Me.outputDeviceLabel.Size = New System.Drawing.Size(265, 18)
        Me.outputDeviceLabel.TabIndex = 4
        Me.outputDeviceLabel.Text = "Output device:"
        '
        'bufferSizeComboBox
        '
        Me.bufferSizeComboBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bufferSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bufferSizeComboBox.FormattingEnabled = True
        Me.bufferSizeComboBox.Location = New System.Drawing.Point(18, 81)
        Me.bufferSizeComboBox.Name = "bufferSizeComboBox"
        Me.bufferSizeComboBox.Size = New System.Drawing.Size(265, 21)
        Me.bufferSizeComboBox.TabIndex = 5
        '
        'bufferSizeLabel
        '
        Me.bufferSizeLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bufferSizeLabel.Location = New System.Drawing.Point(18, 60)
        Me.bufferSizeLabel.Name = "bufferSizeLabel"
        Me.bufferSizeLabel.Size = New System.Drawing.Size(265, 18)
        Me.bufferSizeLabel.TabIndex = 6
        Me.bufferSizeLabel.Text = "Callback buffer size (samples):"
        '
        'deviceSettingsButton
        '
        Me.deviceSettingsButton.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.deviceSettingsButton.Location = New System.Drawing.Point(158, 107)
        Me.deviceSettingsButton.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.deviceSettingsButton.Name = "deviceSettingsButton"
        Me.deviceSettingsButton.Size = New System.Drawing.Size(125, 23)
        Me.deviceSettingsButton.TabIndex = 7
        Me.deviceSettingsButton.Text = "Device settings..."
        Me.deviceSettingsButton.UseVisualStyleBackColor = True
        '
        'latencyLabel
        '
        Me.latencyLabel.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.latencyLabel.AutoSize = True
        Me.latencyLabel.Location = New System.Drawing.Point(241, 221)
        Me.latencyLabel.Name = "latencyLabel"
        Me.latencyLabel.Size = New System.Drawing.Size(164, 13)
        Me.latencyLabel.TabIndex = 6
        Me.latencyLabel.Text = "Minimum callback latency: XX ms"
        Me.latencyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.TableLayoutPanel2)
        Me.GroupBox1.Location = New System.Drawing.Point(3, 33)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(86, 152)
        Me.GroupBox1.TabIndex = 7
        Me.GroupBox1.TabStop = False
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 1
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.Label1, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.OutputChannelCountLabel, 0, 1)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 4
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.59183!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28163!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28163!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28163!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(80, 133)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'OutputChannelCountLabel
        '
        Me.OutputChannelCountLabel.AutoSize = True
        Me.OutputChannelCountLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.OutputChannelCountLabel.Location = New System.Drawing.Point(3, 53)
        Me.OutputChannelCountLabel.Name = "OutputChannelCountLabel"
        Me.OutputChannelCountLabel.Size = New System.Drawing.Size(74, 26)
        Me.OutputChannelCountLabel.TabIndex = 6
        Me.OutputChannelCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(74, 53)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Available channels:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'AudioOutputSettingsDialog
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(408, 270)
        Me.Controls.Add(Me.settingsTableLayoutPanel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AudioOutputSettingsDialog"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "AudioSettingsDialog"
        Me.TopMost = True
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.settingsTableLayoutPanel.ResumeLayout(False)
        Me.settingsTableLayoutPanel.PerformLayout()
        Me.audioSettingsTableLayoutPanel.ResumeLayout(False)
        Me.audioSettingsTableLayoutPanel.PerformLayout()
        Me.audioSettingsPanel.ResumeLayout(False)
        Me.settingsGroupBox.ResumeLayout(False)
        Me.inputTableLayoutPanel.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents settingsTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
    Private WithEvents audioSettingsTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
    Private WithEvents sampleRateLabel As System.Windows.Forms.Label
    Private WithEvents driverTypeLabel As System.Windows.Forms.Label
    Private WithEvents driverTypeComboBox As System.Windows.Forms.ComboBox
    Private WithEvents sampleRateComboBox As System.Windows.Forms.ComboBox
    Private WithEvents audioSettingsPanel As System.Windows.Forms.Panel
    Private WithEvents settingsGroupBox As System.Windows.Forms.GroupBox
    Private WithEvents inputTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
    Private WithEvents outputDeviceComboBox As System.Windows.Forms.ComboBox
    Private WithEvents outputDeviceLabel As System.Windows.Forms.Label
    Private WithEvents bufferSizeComboBox As System.Windows.Forms.ComboBox
    Private WithEvents bufferSizeLabel As System.Windows.Forms.Label
    Private WithEvents latencyLabel As System.Windows.Forms.Label
    Private WithEvents deviceSettingsButton As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents OutputChannelCountLabel As System.Windows.Forms.Label
End Class
