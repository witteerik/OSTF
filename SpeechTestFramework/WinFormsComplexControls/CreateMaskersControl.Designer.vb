<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CreateMaskersControl
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CreateMaskersControl))
        Me.TableLayoutPanel10 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.Label46 = New System.Windows.Forms.Label()
        Me.TargetSNR_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.NoiseType_ComboBox = New System.Windows.Forms.ComboBox()
        Me.CreateNoise_Button = New System.Windows.Forms.Button()
        Me.NoiseFrequencyWheighting_ComboBox = New System.Windows.Forms.ComboBox()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.Label50 = New System.Windows.Forms.Label()
        Me.NumberOfOverlays_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.NoiseDuration_DoubleParsingTextBox = New SpeechTestFramework.DoubleParsingTextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel10.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel10
        '
        Me.TableLayoutPanel10.ColumnCount = 2
        Me.TableLayoutPanel10.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.94643!))
        Me.TableLayoutPanel10.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.05357!))
        Me.TableLayoutPanel10.Controls.Add(Me.Label42, 0, 0)
        Me.TableLayoutPanel10.Controls.Add(Me.TextBox1, 0, 1)
        Me.TableLayoutPanel10.Controls.Add(Me.Label43, 0, 2)
        Me.TableLayoutPanel10.Controls.Add(Me.Label46, 0, 4)
        Me.TableLayoutPanel10.Controls.Add(Me.TargetSNR_IntegerParsingTextBox, 1, 2)
        Me.TableLayoutPanel10.Controls.Add(Me.NoiseType_ComboBox, 1, 4)
        Me.TableLayoutPanel10.Controls.Add(Me.CreateNoise_Button, 0, 7)
        Me.TableLayoutPanel10.Controls.Add(Me.NoiseFrequencyWheighting_ComboBox, 1, 5)
        Me.TableLayoutPanel10.Controls.Add(Me.Label48, 0, 5)
        Me.TableLayoutPanel10.Controls.Add(Me.Label50, 0, 6)
        Me.TableLayoutPanel10.Controls.Add(Me.NumberOfOverlays_IntegerParsingTextBox, 1, 6)
        Me.TableLayoutPanel10.Controls.Add(Me.NoiseDuration_DoubleParsingTextBox, 1, 3)
        Me.TableLayoutPanel10.Controls.Add(Me.Label1, 0, 3)
        Me.TableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel10.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel10.Name = "TableLayoutPanel10"
        Me.TableLayoutPanel10.RowCount = 8
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35.0!))
        Me.TableLayoutPanel10.Size = New System.Drawing.Size(450, 358)
        Me.TableLayoutPanel10.TabIndex = 1
        '
        'Label42
        '
        Me.TableLayoutPanel10.SetColumnSpan(Me.Label42, 2)
        Me.Label42.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label42.Location = New System.Drawing.Point(3, 0)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(444, 26)
        Me.Label42.TabIndex = 3
        Me.Label42.Text = "Instructions"
        Me.Label42.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBox1
        '
        Me.TableLayoutPanel10.SetColumnSpan(Me.TextBox1, 2)
        Me.TextBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBox1.Location = New System.Drawing.Point(3, 29)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(444, 161)
        Me.TextBox1.TabIndex = 0
        Me.TextBox1.Text = resources.GetString("TextBox1.Text")
        '
        'Label43
        '
        Me.Label43.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label43.Location = New System.Drawing.Point(3, 193)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(277, 26)
        Me.Label43.TabIndex = 4
        Me.Label43.Text = "SNR (in relation to the speech material nominal level)"
        Me.Label43.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label46
        '
        Me.Label46.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label46.Location = New System.Drawing.Point(3, 245)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(277, 26)
        Me.Label46.TabIndex = 7
        Me.Label46.Text = "Masking noise type"
        Me.Label46.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TargetSNR_IntegerParsingTextBox
        '
        Me.TargetSNR_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TargetSNR_IntegerParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.TargetSNR_IntegerParsingTextBox.Location = New System.Drawing.Point(286, 196)
        Me.TargetSNR_IntegerParsingTextBox.Name = "TargetSNR_IntegerParsingTextBox"
        Me.TargetSNR_IntegerParsingTextBox.Size = New System.Drawing.Size(161, 20)
        Me.TargetSNR_IntegerParsingTextBox.TabIndex = 9
        Me.TargetSNR_IntegerParsingTextBox.Text = "0"
        '
        'NoiseType_ComboBox
        '
        Me.NoiseType_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NoiseType_ComboBox.FormattingEnabled = True
        Me.NoiseType_ComboBox.Location = New System.Drawing.Point(286, 248)
        Me.NoiseType_ComboBox.Name = "NoiseType_ComboBox"
        Me.NoiseType_ComboBox.Size = New System.Drawing.Size(161, 21)
        Me.NoiseType_ComboBox.TabIndex = 13
        '
        'CreateNoise_Button
        '
        Me.TableLayoutPanel10.SetColumnSpan(Me.CreateNoise_Button, 2)
        Me.CreateNoise_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CreateNoise_Button.Location = New System.Drawing.Point(3, 326)
        Me.CreateNoise_Button.Name = "CreateNoise_Button"
        Me.CreateNoise_Button.Size = New System.Drawing.Size(444, 29)
        Me.CreateNoise_Button.TabIndex = 14
        Me.CreateNoise_Button.Text = "Create masking noise"
        Me.CreateNoise_Button.UseVisualStyleBackColor = True
        '
        'NoiseFrequencyWheighting_ComboBox
        '
        Me.NoiseFrequencyWheighting_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NoiseFrequencyWheighting_ComboBox.FormattingEnabled = True
        Me.NoiseFrequencyWheighting_ComboBox.Location = New System.Drawing.Point(286, 274)
        Me.NoiseFrequencyWheighting_ComboBox.Name = "NoiseFrequencyWheighting_ComboBox"
        Me.NoiseFrequencyWheighting_ComboBox.Size = New System.Drawing.Size(161, 21)
        Me.NoiseFrequencyWheighting_ComboBox.TabIndex = 15
        '
        'Label48
        '
        Me.Label48.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label48.Location = New System.Drawing.Point(3, 271)
        Me.Label48.Name = "Label48"
        Me.Label48.Size = New System.Drawing.Size(277, 26)
        Me.Label48.TabIndex = 16
        Me.Label48.Text = "Noise level frequency weighting"
        Me.Label48.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label50
        '
        Me.Label50.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label50.Location = New System.Drawing.Point(3, 297)
        Me.Label50.Name = "Label50"
        Me.Label50.Size = New System.Drawing.Size(277, 26)
        Me.Label50.TabIndex = 19
        Me.Label50.Text = "Number of overlays (for generation of SW noise)"
        Me.Label50.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'NumberOfOverlays_IntegerParsingTextBox
        '
        Me.NumberOfOverlays_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NumberOfOverlays_IntegerParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.NumberOfOverlays_IntegerParsingTextBox.Location = New System.Drawing.Point(286, 300)
        Me.NumberOfOverlays_IntegerParsingTextBox.Name = "NumberOfOverlays_IntegerParsingTextBox"
        Me.NumberOfOverlays_IntegerParsingTextBox.Size = New System.Drawing.Size(161, 20)
        Me.NumberOfOverlays_IntegerParsingTextBox.TabIndex = 20
        Me.NumberOfOverlays_IntegerParsingTextBox.Text = "5000"
        '
        'NoiseDuration_DoubleParsingTextBox
        '
        Me.NoiseDuration_DoubleParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NoiseDuration_DoubleParsingTextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.NoiseDuration_DoubleParsingTextBox.Location = New System.Drawing.Point(286, 222)
        Me.NoiseDuration_DoubleParsingTextBox.Name = "NoiseDuration_DoubleParsingTextBox"
        Me.NoiseDuration_DoubleParsingTextBox.Size = New System.Drawing.Size(161, 20)
        Me.NoiseDuration_DoubleParsingTextBox.TabIndex = 21
        Me.NoiseDuration_DoubleParsingTextBox.Text = "60"
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 219)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(277, 26)
        Me.Label1.TabIndex = 22
        Me.Label1.Text = "Noise duration (in seconds)"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CreateMaskersControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel10)
        Me.Name = "CreateMaskersControl"
        Me.Size = New System.Drawing.Size(450, 358)
        Me.TableLayoutPanel10.ResumeLayout(False)
        Me.TableLayoutPanel10.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel10 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Label42 As Windows.Forms.Label
    Friend WithEvents TextBox1 As Windows.Forms.TextBox
    Friend WithEvents Label46 As Windows.Forms.Label
    Friend WithEvents TargetSNR_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents NoiseType_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents CreateNoise_Button As Windows.Forms.Button
    Friend WithEvents NoiseFrequencyWheighting_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents Label48 As Windows.Forms.Label
    Friend WithEvents Label50 As Windows.Forms.Label
    Friend WithEvents NumberOfOverlays_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents NoiseDuration_DoubleParsingTextBox As DoubleParsingTextBox
    Friend WithEvents Label43 As Windows.Forms.Label
    Friend WithEvents Label1 As Windows.Forms.Label
End Class
