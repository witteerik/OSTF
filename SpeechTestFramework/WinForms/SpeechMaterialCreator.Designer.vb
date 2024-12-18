﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SpeechMaterialCreator
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SpeechMaterialCreator))
        Me.MainTabControl = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.MySpeechMaterialComponentCreator = New SpeechTestFramework.SpeechMaterialComponentCreator()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.LexicalVariablesEditor1 = New SpeechTestFramework.LexicalVariablesEditor()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.StatisticalSummaryControl1 = New SpeechTestFramework.StatisticalSummaryControl()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.MediaSetSetupControl1 = New SpeechTestFramework.MediaSetSetupControl()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainTabControl.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MainTabControl
        '
        Me.MainTabControl.Controls.Add(Me.TabPage1)
        Me.MainTabControl.Controls.Add(Me.TabPage2)
        Me.MainTabControl.Controls.Add(Me.TabPage3)
        Me.MainTabControl.Controls.Add(Me.TabPage4)
        Me.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MainTabControl.Location = New System.Drawing.Point(0, 24)
        Me.MainTabControl.Name = "MainTabControl"
        Me.MainTabControl.SelectedIndex = 0
        Me.MainTabControl.Size = New System.Drawing.Size(1093, 612)
        Me.MainTabControl.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.MySpeechMaterialComponentCreator)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(955, 498)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Speech material components (SMC)"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'MySpeechMaterialComponentCreator
        '
        Me.MySpeechMaterialComponentCreator.BackColor = System.Drawing.SystemColors.Control
        Me.MySpeechMaterialComponentCreator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.MySpeechMaterialComponentCreator.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MySpeechMaterialComponentCreator.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MySpeechMaterialComponentCreator.Location = New System.Drawing.Point(3, 3)
        Me.MySpeechMaterialComponentCreator.Name = "MySpeechMaterialComponentCreator"
        Me.MySpeechMaterialComponentCreator.Size = New System.Drawing.Size(949, 492)
        Me.MySpeechMaterialComponentCreator.TabIndex = 0
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.LexicalVariablesEditor1)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(955, 498)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Add lexical variables to SMC"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'LexicalVariablesEditor1
        '
        Me.LexicalVariablesEditor1.BackColor = System.Drawing.SystemColors.Control
        Me.LexicalVariablesEditor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LexicalVariablesEditor1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LexicalVariablesEditor1.Location = New System.Drawing.Point(3, 3)
        Me.LexicalVariablesEditor1.Name = "LexicalVariablesEditor1"
        Me.LexicalVariablesEditor1.Size = New System.Drawing.Size(949, 492)
        Me.LexicalVariablesEditor1.TabIndex = 0
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.StatisticalSummaryControl1)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(955, 498)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Summary statistics"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'StatisticalSummaryControl1
        '
        Me.StatisticalSummaryControl1.BackColor = System.Drawing.SystemColors.Control
        Me.StatisticalSummaryControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.StatisticalSummaryControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.StatisticalSummaryControl1.Location = New System.Drawing.Point(3, 3)
        Me.StatisticalSummaryControl1.Name = "StatisticalSummaryControl1"
        Me.StatisticalSummaryControl1.Size = New System.Drawing.Size(949, 492)
        Me.StatisticalSummaryControl1.TabIndex = 0
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.MediaSetSetupControl1)
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(1085, 586)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "Add/edit media set"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'MediaSetSetupControl1
        '
        Me.MediaSetSetupControl1.BackColor = System.Drawing.SystemColors.Control
        Me.MediaSetSetupControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.MediaSetSetupControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MediaSetSetupControl1.Location = New System.Drawing.Point(3, 3)
        Me.MediaSetSetupControl1.Name = "MediaSetSetupControl1"
        Me.MediaSetSetupControl1.Size = New System.Drawing.Size(1079, 580)
        Me.MediaSetSetupControl1.TabIndex = 0
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1093, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'SpeechMaterialCreator
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1093, 636)
        Me.Controls.Add(Me.MainTabControl)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "SpeechMaterialCreator"
        Me.Text = "OSTF - Speech Material Creator"
        Me.MainTabControl.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage4.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MainTabControl As Windows.Forms.TabControl
    Friend WithEvents TabPage1 As Windows.Forms.TabPage
    Friend WithEvents TabPage2 As Windows.Forms.TabPage
    Friend WithEvents MySpeechMaterialComponentCreator As SpeechMaterialComponentCreator
    Friend WithEvents LexicalVariablesEditor1 As LexicalVariablesEditor
    Friend WithEvents TabPage3 As Windows.Forms.TabPage
    Friend WithEvents StatisticalSummaryControl1 As StatisticalSummaryControl
    Friend WithEvents TabPage4 As Windows.Forms.TabPage
    Friend WithEvents MediaSetSetupControl1 As MediaSetSetupControl
    Friend WithEvents MenuStrip1 As Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As Windows.Forms.ToolStripMenuItem
End Class
