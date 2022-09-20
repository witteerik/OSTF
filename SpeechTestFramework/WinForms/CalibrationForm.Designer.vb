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
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'CalibrationSignal_RichTextBox
        '
        Me.CalibrationSignal_RichTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CalibrationSignal_RichTextBox.Location = New System.Drawing.Point(449, 28)
        Me.CalibrationSignal_RichTextBox.Name = "CalibrationSignal_RichTextBox"
        Me.TableLayoutPanel1.SetRowSpan(Me.CalibrationSignal_RichTextBox, 4)
        Me.CalibrationSignal_RichTextBox.Size = New System.Drawing.Size(196, 114)
        Me.CalibrationSignal_RichTextBox.TabIndex = 19
        Me.CalibrationSignal_RichTextBox.Text = ""
        '
        'CalibrationLevel_ComboBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.CalibrationLevel_ComboBox, 2)
        Me.CalibrationLevel_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CalibrationLevel_ComboBox.FormattingEnabled = True
        Me.CalibrationLevel_ComboBox.Location = New System.Drawing.Point(213, 58)
        Me.CalibrationLevel_ComboBox.Name = "CalibrationLevel_ComboBox"
        Me.CalibrationLevel_ComboBox.Size = New System.Drawing.Size(230, 21)
        Me.CalibrationLevel_ComboBox.TabIndex = 18
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(3, 55)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(204, 30)
        Me.Label3.TabIndex = 17
        Me.Label3.Text = "Select calibration signal level (dB SPL):"
        '
        'StopSignal_Button
        '
        Me.StopSignal_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.StopSignal_Button.Enabled = False
        Me.StopSignal_Button.Location = New System.Drawing.Point(331, 118)
        Me.StopSignal_Button.Name = "StopSignal_Button"
        Me.StopSignal_Button.Size = New System.Drawing.Size(112, 24)
        Me.StopSignal_Button.TabIndex = 16
        Me.StopSignal_Button.Text = "Stop signal"
        '
        'PlaySignal_Button
        '
        Me.PlaySignal_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PlaySignal_Button.Enabled = False
        Me.PlaySignal_Button.Location = New System.Drawing.Point(213, 118)
        Me.PlaySignal_Button.Name = "PlaySignal_Button"
        Me.PlaySignal_Button.Size = New System.Drawing.Size(112, 24)
        Me.PlaySignal_Button.TabIndex = 15
        Me.PlaySignal_Button.Text = "Play signal"
        '
        'CalibrationSignal_ComboBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.CalibrationSignal_ComboBox, 2)
        Me.CalibrationSignal_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CalibrationSignal_ComboBox.FormattingEnabled = True
        Me.CalibrationSignal_ComboBox.Location = New System.Drawing.Point(213, 28)
        Me.CalibrationSignal_ComboBox.Name = "CalibrationSignal_ComboBox"
        Me.CalibrationSignal_ComboBox.Size = New System.Drawing.Size(230, 21)
        Me.CalibrationSignal_ComboBox.TabIndex = 14
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(3, 25)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(204, 30)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "Select calibration signal type:"
        '
        'Close_Button
        '
        Me.Close_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Close_Button.Enabled = False
        Me.Close_Button.Location = New System.Drawing.Point(449, 148)
        Me.Close_Button.Name = "Close_Button"
        Me.Close_Button.Size = New System.Drawing.Size(196, 34)
        Me.Close_Button.TabIndex = 10
        Me.Close_Button.Text = "Close"
        '
        'SelectedChannel_ComboBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.SelectedChannel_ComboBox, 2)
        Me.SelectedChannel_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SelectedChannel_ComboBox.FormattingEnabled = True
        Me.SelectedChannel_ComboBox.Location = New System.Drawing.Point(213, 88)
        Me.SelectedChannel_ComboBox.Name = "SelectedChannel_ComboBox"
        Me.SelectedChannel_ComboBox.Size = New System.Drawing.Size(230, 21)
        Me.SelectedChannel_ComboBox.TabIndex = 12
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 85)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(204, 30)
        Me.Label1.TabIndex = 11
        Me.Label1.Text = "Select calibration signal output channel:"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 4
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.02962!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.02962!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.94076!))
        Me.TableLayoutPanel1.Controls.Add(Me.StopSignal_Button, 2, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.CalibrationLevel_ComboBox, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.PlaySignal_Button, 1, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.SelectedChannel_ComboBox, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.CalibrationSignal_ComboBox, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.CalibrationSignal_RichTextBox, 3, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Close_Button, 3, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 3, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 6
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(648, 185)
        Me.TableLayoutPanel1.TabIndex = 20
        '
        'Label4
        '
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Location = New System.Drawing.Point(449, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(196, 25)
        Me.Label4.TabIndex = 20
        Me.Label4.Text = "Signal description"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'CalibrationForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(648, 185)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "CalibrationForm"
        Me.Text = "CalibrationForm"
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
End Class
