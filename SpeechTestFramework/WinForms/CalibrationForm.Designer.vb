<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CalibrationForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CalibrationForm))
        Me.CalibrationSignal_RichTextBox = New System.Windows.Forms.RichTextBox()
        Me.CalibrationLevel_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.StopSignal_Button = New System.Windows.Forms.Button()
        Me.PlaySignal_Button = New System.Windows.Forms.Button()
        Me.CalibrationSignal_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Close_Button = New System.Windows.Forms.Button()
        Me.SelectedHardWareOutputChannel_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Transducer_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.SoundSystem_RichTextBox = New System.Windows.Forms.RichTextBox()
        Me.Splitter1 = New System.Windows.Forms.Splitter()
        Me.Help_Button = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.FrequencyWeighting_ComboBox = New System.Windows.Forms.ComboBox()
        Me.DirectionalSimulationSet_Label = New System.Windows.Forms.Label()
        Me.DirectionalSimulationSet_ComboBox = New System.Windows.Forms.ComboBox()
        Me.SimulatedDistance_Label = New System.Windows.Forms.Label()
        Me.SimulatedDistance_ComboBox = New System.Windows.Forms.ComboBox()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewAvailableSoundDevicesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RightChannel_Label = New System.Windows.Forms.Label()
        Me.SelectedHardWareOutputChannel_Right_ComboBox = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'CalibrationSignal_RichTextBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.CalibrationSignal_RichTextBox, 2)
        Me.CalibrationSignal_RichTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CalibrationSignal_RichTextBox.Location = New System.Drawing.Point(3, 271)
        Me.CalibrationSignal_RichTextBox.Name = "CalibrationSignal_RichTextBox"
        Me.CalibrationSignal_RichTextBox.Size = New System.Drawing.Size(240, 144)
        Me.CalibrationSignal_RichTextBox.TabIndex = 19
        Me.CalibrationSignal_RichTextBox.Text = ""
        '
        'CalibrationLevel_ComboBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.CalibrationLevel_ComboBox, 2)
        Me.CalibrationLevel_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CalibrationLevel_ComboBox.FormattingEnabled = True
        Me.CalibrationLevel_ComboBox.Location = New System.Drawing.Point(249, 107)
        Me.CalibrationLevel_ComboBox.Name = "CalibrationLevel_ComboBox"
        Me.CalibrationLevel_ComboBox.Size = New System.Drawing.Size(240, 21)
        Me.CalibrationLevel_ComboBox.TabIndex = 18
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label3, 2)
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(3, 104)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(240, 26)
        Me.Label3.TabIndex = 17
        Me.Label3.Text = "Select calibration signal level:"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'StopSignal_Button
        '
        Me.StopSignal_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.StopSignal_Button.Location = New System.Drawing.Point(372, 211)
        Me.StopSignal_Button.Name = "StopSignal_Button"
        Me.StopSignal_Button.Size = New System.Drawing.Size(117, 24)
        Me.StopSignal_Button.TabIndex = 16
        Me.StopSignal_Button.Text = "Stop signal"
        '
        'PlaySignal_Button
        '
        Me.PlaySignal_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PlaySignal_Button.Location = New System.Drawing.Point(249, 211)
        Me.PlaySignal_Button.Name = "PlaySignal_Button"
        Me.PlaySignal_Button.Size = New System.Drawing.Size(117, 24)
        Me.PlaySignal_Button.TabIndex = 15
        Me.PlaySignal_Button.Text = "Play signal"
        '
        'CalibrationSignal_ComboBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.CalibrationSignal_ComboBox, 2)
        Me.CalibrationSignal_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CalibrationSignal_ComboBox.FormattingEnabled = True
        Me.CalibrationSignal_ComboBox.Location = New System.Drawing.Point(249, 29)
        Me.CalibrationSignal_ComboBox.Name = "CalibrationSignal_ComboBox"
        Me.CalibrationSignal_ComboBox.Size = New System.Drawing.Size(240, 21)
        Me.CalibrationSignal_ComboBox.TabIndex = 14
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label2, 2)
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(3, 26)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(240, 26)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "Select calibration signal type:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Close_Button
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.Close_Button, 4)
        Me.Close_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Close_Button.Location = New System.Drawing.Point(3, 421)
        Me.Close_Button.Name = "Close_Button"
        Me.Close_Button.Size = New System.Drawing.Size(486, 24)
        Me.Close_Button.TabIndex = 10
        Me.Close_Button.Text = "Close"
        '
        'SelectedHardWareOutputChannel_ComboBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.SelectedHardWareOutputChannel_ComboBox, 2)
        Me.SelectedHardWareOutputChannel_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SelectedHardWareOutputChannel_ComboBox.FormattingEnabled = True
        Me.SelectedHardWareOutputChannel_ComboBox.Location = New System.Drawing.Point(249, 159)
        Me.SelectedHardWareOutputChannel_ComboBox.Name = "SelectedHardWareOutputChannel_ComboBox"
        Me.SelectedHardWareOutputChannel_ComboBox.Size = New System.Drawing.Size(240, 21)
        Me.SelectedHardWareOutputChannel_ComboBox.TabIndex = 12
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label1, 2)
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 156)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(240, 26)
        Me.Label1.TabIndex = 11
        Me.Label1.Text = "Select calibration signal output channel:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 4
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.StopSignal_Button, 3, 8)
        Me.TableLayoutPanel1.Controls.Add(Me.CalibrationLevel_ComboBox, 2, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.PlaySignal_Button, 2, 8)
        Me.TableLayoutPanel1.Controls.Add(Me.SelectedHardWareOutputChannel_ComboBox, 2, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.Label5, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Transducer_ComboBox, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 0, 10)
        Me.TableLayoutPanel1.Controls.Add(Me.Label6, 2, 10)
        Me.TableLayoutPanel1.Controls.Add(Me.CalibrationSignal_RichTextBox, 0, 11)
        Me.TableLayoutPanel1.Controls.Add(Me.SoundSystem_RichTextBox, 2, 11)
        Me.TableLayoutPanel1.Controls.Add(Me.Close_Button, 0, 12)
        Me.TableLayoutPanel1.Controls.Add(Me.Splitter1, 0, 9)
        Me.TableLayoutPanel1.Controls.Add(Me.Help_Button, 0, 8)
        Me.TableLayoutPanel1.Controls.Add(Me.Label7, 0, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.FrequencyWeighting_ComboBox, 2, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.DirectionalSimulationSet_Label, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.DirectionalSimulationSet_ComboBox, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.SimulatedDistance_Label, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.SimulatedDistance_ComboBox, 2, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.CalibrationSignal_ComboBox, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.RightChannel_Label, 0, 7)
        Me.TableLayoutPanel1.Controls.Add(Me.SelectedHardWareOutputChannel_Right_ComboBox, 2, 7)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 24)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 13
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(492, 448)
        Me.TableLayoutPanel1.TabIndex = 20
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label5, 2)
        Me.Label5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label5.Location = New System.Drawing.Point(3, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(240, 26)
        Me.Label5.TabIndex = 21
        Me.Label5.Text = "Select sound system:"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Transducer_ComboBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.Transducer_ComboBox, 2)
        Me.Transducer_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Transducer_ComboBox.FormattingEnabled = True
        Me.Transducer_ComboBox.Location = New System.Drawing.Point(249, 3)
        Me.Transducer_ComboBox.Name = "Transducer_ComboBox"
        Me.Transducer_ComboBox.Size = New System.Drawing.Size(240, 21)
        Me.Transducer_ComboBox.TabIndex = 22
        '
        'Label4
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label4, 2)
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Location = New System.Drawing.Point(3, 248)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(240, 20)
        Me.Label4.TabIndex = 20
        Me.Label4.Text = "Signal description"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'Label6
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label6, 2)
        Me.Label6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label6.Location = New System.Drawing.Point(249, 248)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(240, 20)
        Me.Label6.TabIndex = 23
        Me.Label6.Text = "Sound system specifications"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'SoundSystem_RichTextBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.SoundSystem_RichTextBox, 2)
        Me.SoundSystem_RichTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SoundSystem_RichTextBox.Location = New System.Drawing.Point(249, 271)
        Me.SoundSystem_RichTextBox.Name = "SoundSystem_RichTextBox"
        Me.SoundSystem_RichTextBox.Size = New System.Drawing.Size(240, 144)
        Me.SoundSystem_RichTextBox.TabIndex = 24
        Me.SoundSystem_RichTextBox.Text = ""
        '
        'Splitter1
        '
        Me.Splitter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TableLayoutPanel1.SetColumnSpan(Me.Splitter1, 4)
        Me.Splitter1.Cursor = System.Windows.Forms.Cursors.HSplit
        Me.Splitter1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Splitter1.Location = New System.Drawing.Point(3, 241)
        Me.Splitter1.Name = "Splitter1"
        Me.Splitter1.Size = New System.Drawing.Size(486, 3)
        Me.Splitter1.TabIndex = 25
        Me.Splitter1.TabStop = False
        '
        'Help_Button
        '
        Me.Help_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Help_Button.Location = New System.Drawing.Point(3, 211)
        Me.Help_Button.Name = "Help_Button"
        Me.Help_Button.Size = New System.Drawing.Size(117, 24)
        Me.Help_Button.TabIndex = 26
        Me.Help_Button.Text = "Help"
        Me.Help_Button.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label7, 2)
        Me.Label7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label7.Location = New System.Drawing.Point(3, 130)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(240, 26)
        Me.Label7.TabIndex = 27
        Me.Label7.Text = "Frequency weighting:"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FrequencyWeighting_ComboBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.FrequencyWeighting_ComboBox, 2)
        Me.FrequencyWeighting_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FrequencyWeighting_ComboBox.FormattingEnabled = True
        Me.FrequencyWeighting_ComboBox.Location = New System.Drawing.Point(249, 133)
        Me.FrequencyWeighting_ComboBox.Name = "FrequencyWeighting_ComboBox"
        Me.FrequencyWeighting_ComboBox.Size = New System.Drawing.Size(240, 21)
        Me.FrequencyWeighting_ComboBox.TabIndex = 28
        '
        'DirectionalSimulationSet_Label
        '
        Me.DirectionalSimulationSet_Label.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.DirectionalSimulationSet_Label, 2)
        Me.DirectionalSimulationSet_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DirectionalSimulationSet_Label.Location = New System.Drawing.Point(3, 52)
        Me.DirectionalSimulationSet_Label.Name = "DirectionalSimulationSet_Label"
        Me.DirectionalSimulationSet_Label.Size = New System.Drawing.Size(240, 26)
        Me.DirectionalSimulationSet_Label.TabIndex = 29
        Me.DirectionalSimulationSet_Label.Text = "Directional simulation set (optional):"
        Me.DirectionalSimulationSet_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'DirectionalSimulationSet_ComboBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.DirectionalSimulationSet_ComboBox, 2)
        Me.DirectionalSimulationSet_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DirectionalSimulationSet_ComboBox.FormattingEnabled = True
        Me.DirectionalSimulationSet_ComboBox.Location = New System.Drawing.Point(249, 55)
        Me.DirectionalSimulationSet_ComboBox.Name = "DirectionalSimulationSet_ComboBox"
        Me.DirectionalSimulationSet_ComboBox.Size = New System.Drawing.Size(240, 21)
        Me.DirectionalSimulationSet_ComboBox.TabIndex = 30
        '
        'SimulatedDistance_Label
        '
        Me.SimulatedDistance_Label.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.SimulatedDistance_Label, 2)
        Me.SimulatedDistance_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SimulatedDistance_Label.Location = New System.Drawing.Point(3, 78)
        Me.SimulatedDistance_Label.Name = "SimulatedDistance_Label"
        Me.SimulatedDistance_Label.Size = New System.Drawing.Size(240, 26)
        Me.SimulatedDistance_Label.TabIndex = 31
        Me.SimulatedDistance_Label.Text = "Simulated sound source distance (optional):"
        Me.SimulatedDistance_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SimulatedDistance_ComboBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.SimulatedDistance_ComboBox, 2)
        Me.SimulatedDistance_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SimulatedDistance_ComboBox.FormattingEnabled = True
        Me.SimulatedDistance_ComboBox.Location = New System.Drawing.Point(249, 81)
        Me.SimulatedDistance_ComboBox.Name = "SimulatedDistance_ComboBox"
        Me.SimulatedDistance_ComboBox.Size = New System.Drawing.Size(240, 21)
        Me.SimulatedDistance_ComboBox.TabIndex = 32
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(492, 24)
        Me.MenuStrip1.TabIndex = 21
        Me.MenuStrip1.Text = "File"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HelpToolStripMenuItem, Me.ViewAvailableSoundDevicesToolStripMenuItem, Me.AboutToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(226, 22)
        Me.HelpToolStripMenuItem.Text = "Help"
        '
        'ViewAvailableSoundDevicesToolStripMenuItem
        '
        Me.ViewAvailableSoundDevicesToolStripMenuItem.Name = "ViewAvailableSoundDevicesToolStripMenuItem"
        Me.ViewAvailableSoundDevicesToolStripMenuItem.Size = New System.Drawing.Size(226, 22)
        Me.ViewAvailableSoundDevicesToolStripMenuItem.Text = "View available sound devices"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(226, 22)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(226, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'RightChannel_Label
        '
        Me.RightChannel_Label.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.RightChannel_Label, 2)
        Me.RightChannel_Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RightChannel_Label.Location = New System.Drawing.Point(3, 182)
        Me.RightChannel_Label.Name = "RightChannel_Label"
        Me.RightChannel_Label.Size = New System.Drawing.Size(240, 26)
        Me.RightChannel_Label.TabIndex = 33
        Me.RightChannel_Label.Text = "Select calibration signal output channel (Right):"
        Me.RightChannel_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SelectedHardWareOutputChannel_Right_ComboBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.SelectedHardWareOutputChannel_Right_ComboBox, 2)
        Me.SelectedHardWareOutputChannel_Right_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SelectedHardWareOutputChannel_Right_ComboBox.FormattingEnabled = True
        Me.SelectedHardWareOutputChannel_Right_ComboBox.Location = New System.Drawing.Point(249, 185)
        Me.SelectedHardWareOutputChannel_Right_ComboBox.Name = "SelectedHardWareOutputChannel_Right_ComboBox"
        Me.SelectedHardWareOutputChannel_Right_ComboBox.Size = New System.Drawing.Size(240, 21)
        Me.SelectedHardWareOutputChannel_Right_ComboBox.TabIndex = 34
        '
        'CalibrationForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(492, 472)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "CalibrationForm"
        Me.Text = "OSTF - Calibration form"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents CalibrationSignal_RichTextBox As Windows.Forms.RichTextBox
    Friend WithEvents CalibrationLevel_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents StopSignal_Button As Windows.Forms.Button
    Friend WithEvents PlaySignal_Button As Windows.Forms.Button
    Friend WithEvents CalibrationSignal_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents Close_Button As Windows.Forms.Button
    Friend WithEvents SelectedHardWareOutputChannel_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents Transducer_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents SoundSystem_RichTextBox As Windows.Forms.RichTextBox
    Friend WithEvents Splitter1 As Windows.Forms.Splitter
    Friend WithEvents Help_Button As Windows.Forms.Button
    Friend WithEvents MenuStrip1 As Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ViewAvailableSoundDevicesToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents Label7 As Windows.Forms.Label
    Friend WithEvents FrequencyWeighting_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents DirectionalSimulationSet_Label As Windows.Forms.Label
    Friend WithEvents DirectionalSimulationSet_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents SimulatedDistance_Label As Windows.Forms.Label
    Friend WithEvents SimulatedDistance_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents RightChannel_Label As Windows.Forms.Label
    Friend WithEvents SelectedHardWareOutputChannel_Right_ComboBox As Windows.Forms.ComboBox
End Class
