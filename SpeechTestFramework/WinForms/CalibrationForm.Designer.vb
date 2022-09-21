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
        Me.CalibrationSignal_RichTextBox = New System.Windows.Forms.RichTextBox()
        Me.CalibrationLevel_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.StopSignal_Button = New System.Windows.Forms.Button()
        Me.PlaySignal_Button = New System.Windows.Forms.Button()
        Me.CalibrationSignal_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Close_Button = New System.Windows.Forms.Button()
        Me.SelectedChannel_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Transducer_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.SoundSystem_RichTextBox = New System.Windows.Forms.RichTextBox()
        Me.Splitter1 = New System.Windows.Forms.Splitter()
        Me.Help_Button = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'CalibrationSignal_RichTextBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.CalibrationSignal_RichTextBox, 2)
        Me.CalibrationSignal_RichTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CalibrationSignal_RichTextBox.Location = New System.Drawing.Point(3, 167)
        Me.CalibrationSignal_RichTextBox.Name = "CalibrationSignal_RichTextBox"
        Me.CalibrationSignal_RichTextBox.Size = New System.Drawing.Size(240, 272)
        Me.CalibrationSignal_RichTextBox.TabIndex = 19
        Me.CalibrationSignal_RichTextBox.Text = ""
        '
        'CalibrationLevel_ComboBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.CalibrationLevel_ComboBox, 2)
        Me.CalibrationLevel_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CalibrationLevel_ComboBox.FormattingEnabled = True
        Me.CalibrationLevel_ComboBox.Location = New System.Drawing.Point(249, 55)
        Me.CalibrationLevel_ComboBox.Name = "CalibrationLevel_ComboBox"
        Me.CalibrationLevel_ComboBox.Size = New System.Drawing.Size(240, 21)
        Me.CalibrationLevel_ComboBox.TabIndex = 18
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label3, 2)
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(3, 52)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(240, 26)
        Me.Label3.TabIndex = 17
        Me.Label3.Text = "Select calibration signal level (dB SPL):"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'StopSignal_Button
        '
        Me.StopSignal_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.StopSignal_Button.Location = New System.Drawing.Point(372, 107)
        Me.StopSignal_Button.Name = "StopSignal_Button"
        Me.StopSignal_Button.Size = New System.Drawing.Size(117, 24)
        Me.StopSignal_Button.TabIndex = 16
        Me.StopSignal_Button.Text = "Stop signal"
        '
        'PlaySignal_Button
        '
        Me.PlaySignal_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PlaySignal_Button.Location = New System.Drawing.Point(249, 107)
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
        Me.Close_Button.Location = New System.Drawing.Point(3, 445)
        Me.Close_Button.Name = "Close_Button"
        Me.Close_Button.Size = New System.Drawing.Size(486, 24)
        Me.Close_Button.TabIndex = 10
        Me.Close_Button.Text = "Close"
        '
        'SelectedChannel_ComboBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.SelectedChannel_ComboBox, 2)
        Me.SelectedChannel_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SelectedChannel_ComboBox.FormattingEnabled = True
        Me.SelectedChannel_ComboBox.Location = New System.Drawing.Point(249, 81)
        Me.SelectedChannel_ComboBox.Name = "SelectedChannel_ComboBox"
        Me.SelectedChannel_ComboBox.Size = New System.Drawing.Size(240, 21)
        Me.SelectedChannel_ComboBox.TabIndex = 12
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label1, 2)
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 78)
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
        Me.TableLayoutPanel1.Controls.Add(Me.StopSignal_Button, 3, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.CalibrationLevel_ComboBox, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.PlaySignal_Button, 2, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.SelectedChannel_ComboBox, 2, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.CalibrationSignal_ComboBox, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label5, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Transducer_ComboBox, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 0, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.Label6, 2, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.CalibrationSignal_RichTextBox, 0, 7)
        Me.TableLayoutPanel1.Controls.Add(Me.SoundSystem_RichTextBox, 2, 7)
        Me.TableLayoutPanel1.Controls.Add(Me.Close_Button, 0, 8)
        Me.TableLayoutPanel1.Controls.Add(Me.Splitter1, 0, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.Help_Button, 0, 4)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 9
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(492, 472)
        Me.TableLayoutPanel1.TabIndex = 20
        '
        'Label4
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label4, 2)
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Location = New System.Drawing.Point(3, 144)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(240, 20)
        Me.Label4.TabIndex = 20
        Me.Label4.Text = "Signal description"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter
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
        'Label6
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.Label6, 2)
        Me.Label6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label6.Location = New System.Drawing.Point(249, 144)
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
        Me.SoundSystem_RichTextBox.Location = New System.Drawing.Point(249, 167)
        Me.SoundSystem_RichTextBox.Name = "SoundSystem_RichTextBox"
        Me.SoundSystem_RichTextBox.Size = New System.Drawing.Size(240, 272)
        Me.SoundSystem_RichTextBox.TabIndex = 24
        Me.SoundSystem_RichTextBox.Text = ""
        '
        'Splitter1
        '
        Me.Splitter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TableLayoutPanel1.SetColumnSpan(Me.Splitter1, 4)
        Me.Splitter1.Cursor = System.Windows.Forms.Cursors.HSplit
        Me.Splitter1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Splitter1.Location = New System.Drawing.Point(3, 137)
        Me.Splitter1.Name = "Splitter1"
        Me.Splitter1.Size = New System.Drawing.Size(486, 3)
        Me.Splitter1.TabIndex = 25
        Me.Splitter1.TabStop = False
        '
        'Help_Button
        '
        Me.Help_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Help_Button.Location = New System.Drawing.Point(3, 107)
        Me.Help_Button.Name = "Help_Button"
        Me.Help_Button.Size = New System.Drawing.Size(117, 24)
        Me.Help_Button.TabIndex = 26
        Me.Help_Button.Text = "Help"
        Me.Help_Button.UseVisualStyleBackColor = True
        '
        'CalibrationForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(492, 472)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "CalibrationForm"
        Me.Text = "Calibration form"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents CalibrationSignal_RichTextBox As Windows.Forms.RichTextBox
    Friend WithEvents CalibrationLevel_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents StopSignal_Button As Windows.Forms.Button
    Friend WithEvents PlaySignal_Button As Windows.Forms.Button
    Friend WithEvents CalibrationSignal_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents Close_Button As Windows.Forms.Button
    Friend WithEvents SelectedChannel_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents Transducer_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents SoundSystem_RichTextBox As Windows.Forms.RichTextBox
    Friend WithEvents Splitter1 As Windows.Forms.Splitter
    Friend WithEvents Help_Button As Windows.Forms.Button
End Class
