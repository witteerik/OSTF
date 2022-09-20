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
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.SelectedChannel_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'CalibrationSignal_RichTextBox
        '
        Me.CalibrationSignal_RichTextBox.Location = New System.Drawing.Point(478, 157)
        Me.CalibrationSignal_RichTextBox.Name = "CalibrationSignal_RichTextBox"
        Me.CalibrationSignal_RichTextBox.Size = New System.Drawing.Size(196, 104)
        Me.CalibrationSignal_RichTextBox.TabIndex = 19
        Me.CalibrationSignal_RichTextBox.Text = ""
        '
        'CalibrationLevel_ComboBox
        '
        Me.CalibrationLevel_ComboBox.FormattingEnabled = True
        Me.CalibrationLevel_ComboBox.Location = New System.Drawing.Point(322, 184)
        Me.CalibrationLevel_ComboBox.Name = "CalibrationLevel_ComboBox"
        Me.CalibrationLevel_ComboBox.Size = New System.Drawing.Size(140, 21)
        Me.CalibrationLevel_ComboBox.TabIndex = 18
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(127, 189)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(191, 13)
        Me.Label3.TabIndex = 17
        Me.Label3.Text = "Select calibration signal level (dB SPL):"
        '
        'StopSignal_Button
        '
        Me.StopSignal_Button.Enabled = False
        Me.StopSignal_Button.Location = New System.Drawing.Point(395, 238)
        Me.StopSignal_Button.Name = "StopSignal_Button"
        Me.StopSignal_Button.Size = New System.Drawing.Size(67, 23)
        Me.StopSignal_Button.TabIndex = 16
        Me.StopSignal_Button.Text = "Stop signal"
        '
        'PlaySignal_Button
        '
        Me.PlaySignal_Button.Enabled = False
        Me.PlaySignal_Button.Location = New System.Drawing.Point(322, 238)
        Me.PlaySignal_Button.Name = "PlaySignal_Button"
        Me.PlaySignal_Button.Size = New System.Drawing.Size(67, 23)
        Me.PlaySignal_Button.TabIndex = 15
        Me.PlaySignal_Button.Text = "Play signal"
        '
        'CalibrationSignal_ComboBox
        '
        Me.CalibrationSignal_ComboBox.FormattingEnabled = True
        Me.CalibrationSignal_ComboBox.Location = New System.Drawing.Point(322, 157)
        Me.CalibrationSignal_ComboBox.Name = "CalibrationSignal_ComboBox"
        Me.CalibrationSignal_ComboBox.Size = New System.Drawing.Size(140, 21)
        Me.CalibrationSignal_ComboBox.TabIndex = 14
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(127, 162)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(144, 13)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "Select calibration signal type:"
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Enabled = False
        Me.OK_Button.Location = New System.Drawing.Point(607, 270)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 10
        Me.OK_Button.Text = "OK"
        '
        'SelectedChannel_ComboBox
        '
        Me.SelectedChannel_ComboBox.FormattingEnabled = True
        Me.SelectedChannel_ComboBox.Location = New System.Drawing.Point(322, 211)
        Me.SelectedChannel_ComboBox.Name = "SelectedChannel_ComboBox"
        Me.SelectedChannel_ComboBox.Size = New System.Drawing.Size(140, 21)
        Me.SelectedChannel_ComboBox.TabIndex = 12
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(127, 216)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(195, 13)
        Me.Label1.TabIndex = 11
        Me.Label1.Text = "Select calibration signal output channel:"
        '
        'CalibrationForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.CalibrationSignal_RichTextBox)
        Me.Controls.Add(Me.CalibrationLevel_ComboBox)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.StopSignal_Button)
        Me.Controls.Add(Me.PlaySignal_Button)
        Me.Controls.Add(Me.CalibrationSignal_ComboBox)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.OK_Button)
        Me.Controls.Add(Me.SelectedChannel_ComboBox)
        Me.Controls.Add(Me.Label1)
        Me.Name = "CalibrationForm"
        Me.Text = "CalibrationForm"
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
    Friend WithEvents OK_Button As Windows.Forms.Button
    Friend WithEvents SelectedChannel_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label1 As Windows.Forms.Label
End Class
