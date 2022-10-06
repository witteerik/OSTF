<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OSTF_suite_R
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
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Launch_SiP_SE_R_Button = New System.Windows.Forms.Button()
        Me.Launch_SpeechMaterialCreator_Button = New System.Windows.Forms.Button()
        Me.Launch_SoundLevelCalibration_Button = New System.Windows.Forms.Button()
        Me.MenuStrip1.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(739, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem, Me.CloseToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.CloseToolStripMenuItem.Text = "Exit"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.Controls.Add(Me.Launch_SiP_SE_R_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Launch_SpeechMaterialCreator_Button, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Launch_SoundLevelCalibration_Button, 2, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 24)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(739, 271)
        Me.TableLayoutPanel1.TabIndex = 1
        '
        'Launch_SiP_SE_R_Button
        '
        Me.Launch_SiP_SE_R_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Launch_SiP_SE_R_Button.Image = Global.OSTF_suite_R.My.Resources.Resources.ic_launcher
        Me.Launch_SiP_SE_R_Button.Location = New System.Drawing.Point(3, 3)
        Me.Launch_SiP_SE_R_Button.Name = "Launch_SiP_SE_R_Button"
        Me.Launch_SiP_SE_R_Button.Size = New System.Drawing.Size(240, 265)
        Me.Launch_SiP_SE_R_Button.TabIndex = 0
        Me.Launch_SiP_SE_R_Button.Text = "Swedish SiP-test (Research version)"
        Me.Launch_SiP_SE_R_Button.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Launch_SiP_SE_R_Button.UseVisualStyleBackColor = True
        '
        'Launch_SpeechMaterialCreator_Button
        '
        Me.Launch_SpeechMaterialCreator_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Launch_SpeechMaterialCreator_Button.Location = New System.Drawing.Point(249, 3)
        Me.Launch_SpeechMaterialCreator_Button.Name = "Launch_SpeechMaterialCreator_Button"
        Me.Launch_SpeechMaterialCreator_Button.Size = New System.Drawing.Size(240, 265)
        Me.Launch_SpeechMaterialCreator_Button.TabIndex = 1
        Me.Launch_SpeechMaterialCreator_Button.Text = "OSTF - Speech Material Creator"
        Me.Launch_SpeechMaterialCreator_Button.UseVisualStyleBackColor = True
        '
        'Launch_SoundLevelCalibration_Button
        '
        Me.Launch_SoundLevelCalibration_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Launch_SoundLevelCalibration_Button.Location = New System.Drawing.Point(495, 3)
        Me.Launch_SoundLevelCalibration_Button.Name = "Launch_SoundLevelCalibration_Button"
        Me.Launch_SoundLevelCalibration_Button.Size = New System.Drawing.Size(241, 265)
        Me.Launch_SoundLevelCalibration_Button.TabIndex = 2
        Me.Launch_SoundLevelCalibration_Button.Text = "OSTF - Sound level calibration (Research version)"
        Me.Launch_SoundLevelCalibration_Button.UseVisualStyleBackColor = True
        '
        'OSTF_suite_R
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(739, 295)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "OSTF_suite_R"
        Me.Text = "Open Speech Test Framwork (OSTF) Suite - Research Version - BETA"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CloseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents Launch_SiP_SE_R_Button As Button
    Friend WithEvents Launch_SpeechMaterialCreator_Button As Button
    Friend WithEvents Launch_SoundLevelCalibration_Button As Button
End Class
