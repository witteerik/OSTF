<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SpectrogramSettingsDialog
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
        Me.Label9 = New System.Windows.Forms.Label()
        Me.AWOS_Box = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.PreAWS_Box = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.PreKernel_Box = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.AWS_box = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.CF_Box = New System.Windows.Forms.TextBox()
        Me.FFT_Box = New System.Windows.Forms.Label()
        Me.FFTSize_Box = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.WindowingTypeComboBox = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.UsePreFftFilteringBox = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.PreFFT_AR_Box = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel1.SuspendLayout()
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
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(221, 257)
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
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(12, 120)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(83, 13)
        Me.Label9.TabIndex = 49
        Me.Label9.Text = "Windowing type"
        '
        'AWOS_Box
        '
        Me.AWOS_Box.Location = New System.Drawing.Point(268, 38)
        Me.AWOS_Box.Name = "AWOS_Box"
        Me.AWOS_Box.Size = New System.Drawing.Size(100, 20)
        Me.AWOS_Box.TabIndex = 48
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(12, 225)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(214, 13)
        Me.Label8.TabIndex = 47
        Me.Label8.Text = "Pre FFT filter analysis window size (samples)"
        '
        'PreAWS_Box
        '
        Me.PreAWS_Box.Location = New System.Drawing.Point(268, 222)
        Me.PreAWS_Box.Name = "PreAWS_Box"
        Me.PreAWS_Box.Size = New System.Drawing.Size(100, 20)
        Me.PreAWS_Box.TabIndex = 46
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(12, 199)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(167, 13)
        Me.Label7.TabIndex = 45
        Me.Label7.Text = "Pre FFT filter kernel size (samples)"
        '
        'PreKernel_Box
        '
        Me.PreKernel_Box.Location = New System.Drawing.Point(268, 196)
        Me.PreKernel_Box.Name = "PreKernel_Box"
        Me.PreKernel_Box.Size = New System.Drawing.Size(100, 20)
        Me.PreKernel_Box.TabIndex = 44
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 15)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(157, 13)
        Me.Label4.TabIndex = 43
        Me.Label4.Text = "Analysis windows size (samples)"
        '
        'AWS_box
        '
        Me.AWS_box.Location = New System.Drawing.Point(268, 12)
        Me.AWS_box.Name = "AWS_box"
        Me.AWS_box.Size = New System.Drawing.Size(100, 20)
        Me.AWS_box.TabIndex = 42
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 68)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(157, 13)
        Me.Label3.TabIndex = 41
        Me.Label3.Text = "Spectrogram cut frequency (Hz)"
        '
        'CF_Box
        '
        Me.CF_Box.Location = New System.Drawing.Point(268, 65)
        Me.CF_Box.Name = "CF_Box"
        Me.CF_Box.Size = New System.Drawing.Size(100, 20)
        Me.CF_Box.TabIndex = 40
        '
        'FFT_Box
        '
        Me.FFT_Box.AutoSize = True
        Me.FFT_Box.Location = New System.Drawing.Point(12, 94)
        Me.FFT_Box.Name = "FFT_Box"
        Me.FFT_Box.Size = New System.Drawing.Size(242, 13)
        Me.FFT_Box.TabIndex = 39
        Me.FFT_Box.Text = "FFT window size / frequency resolution (samples) "
        '
        'FFTSize_Box
        '
        Me.FFTSize_Box.Location = New System.Drawing.Point(268, 91)
        Me.FFTSize_Box.Name = "FFTSize_Box"
        Me.FFTSize_Box.Size = New System.Drawing.Size(100, 20)
        Me.FFTSize_Box.TabIndex = 38
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 41)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(190, 13)
        Me.Label1.TabIndex = 37
        Me.Label1.Text = "Analysis window overlap size (samples)"
        '
        'WindowingTypeComboBox
        '
        Me.WindowingTypeComboBox.FormattingEnabled = True
        Me.WindowingTypeComboBox.Location = New System.Drawing.Point(159, 117)
        Me.WindowingTypeComboBox.Name = "WindowingTypeComboBox"
        Me.WindowingTypeComboBox.Size = New System.Drawing.Size(209, 21)
        Me.WindowingTypeComboBox.TabIndex = 36
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 146)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(102, 13)
        Me.Label6.TabIndex = 35
        Me.Label6.Text = "Use pre FFT filtering"
        '
        'UsePreFftFilteringBox
        '
        Me.UsePreFftFilteringBox.FormattingEnabled = True
        Me.UsePreFftFilteringBox.Location = New System.Drawing.Point(268, 144)
        Me.UsePreFftFilteringBox.Name = "UsePreFftFilteringBox"
        Me.UsePreFftFilteringBox.Size = New System.Drawing.Size(100, 21)
        Me.UsePreFftFilteringBox.TabIndex = 34
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(12, 173)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(204, 13)
        Me.Label5.TabIndex = 33
        Me.Label5.Text = "Pre FFT filter attenuation rate (dB/octave)"
        '
        'PreFFT_AR_Box
        '
        Me.PreFFT_AR_Box.Location = New System.Drawing.Point(268, 170)
        Me.PreFFT_AR_Box.Name = "PreFFT_AR_Box"
        Me.PreFFT_AR_Box.Size = New System.Drawing.Size(100, 20)
        Me.PreFFT_AR_Box.TabIndex = 32
        '
        'SpectrogramSettingsDialog
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(379, 298)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.AWOS_Box)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.PreAWS_Box)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.PreKernel_Box)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.AWS_box)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.CF_Box)
        Me.Controls.Add(Me.FFT_Box)
        Me.Controls.Add(Me.FFTSize_Box)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.WindowingTypeComboBox)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.UsePreFftFilteringBox)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.PreFFT_AR_Box)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SpectrogramSettingsDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Spectrogram settings"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents AWOS_Box As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents PreAWS_Box As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents PreKernel_Box As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents AWS_box As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents CF_Box As System.Windows.Forms.TextBox
    Friend WithEvents FFT_Box As System.Windows.Forms.Label
    Friend WithEvents FFTSize_Box As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents WindowingTypeComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents UsePreFftFilteringBox As System.Windows.Forms.ComboBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents PreFFT_AR_Box As System.Windows.Forms.TextBox
End Class
