﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SpeechMaterialCreator
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
        Me.MainTabControl = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.MySpeechMaterialComponentCreator = New SpeechTestFramework.SpeechMaterialComponentCreator()
        Me.LexicalVariablesEditor1 = New SpeechTestFramework.LexicalVariablesEditor()
        Me.MainTabControl.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.SuspendLayout()
        '
        'MainTabControl
        '
        Me.MainTabControl.Controls.Add(Me.TabPage1)
        Me.MainTabControl.Controls.Add(Me.TabPage2)
        Me.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MainTabControl.Location = New System.Drawing.Point(0, 0)
        Me.MainTabControl.Name = "MainTabControl"
        Me.MainTabControl.SelectedIndex = 0
        Me.MainTabControl.Size = New System.Drawing.Size(845, 450)
        Me.MainTabControl.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.MySpeechMaterialComponentCreator)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(837, 424)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Speech material components"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.LexicalVariablesEditor1)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(837, 424)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "TabPage2"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'MySpeechMaterialComponentCreator
        '
        Me.MySpeechMaterialComponentCreator.BackColor = System.Drawing.SystemColors.Control
        Me.MySpeechMaterialComponentCreator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.MySpeechMaterialComponentCreator.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MySpeechMaterialComponentCreator.Location = New System.Drawing.Point(3, 3)
        Me.MySpeechMaterialComponentCreator.Name = "MySpeechMaterialComponentCreator"
        Me.MySpeechMaterialComponentCreator.Size = New System.Drawing.Size(831, 418)
        Me.MySpeechMaterialComponentCreator.TabIndex = 0
        '
        'LexicalVariablesEditor1
        '
        Me.LexicalVariablesEditor1.BackColor = System.Drawing.SystemColors.Control
        Me.LexicalVariablesEditor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LexicalVariablesEditor1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LexicalVariablesEditor1.Location = New System.Drawing.Point(3, 3)
        Me.LexicalVariablesEditor1.Name = "LexicalVariablesEditor1"
        Me.LexicalVariablesEditor1.Size = New System.Drawing.Size(831, 418)
        Me.LexicalVariablesEditor1.TabIndex = 0
        '
        'SpeechMaterialCreator
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(845, 450)
        Me.Controls.Add(Me.MainTabControl)
        Me.Name = "SpeechMaterialCreator"
        Me.Text = "SpeechMaterialCreator"
        Me.MainTabControl.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents MainTabControl As Windows.Forms.TabControl
    Friend WithEvents TabPage1 As Windows.Forms.TabPage
    Friend WithEvents TabPage2 As Windows.Forms.TabPage
    Friend WithEvents MySpeechMaterialComponentCreator As SpeechMaterialComponentCreator
    Friend WithEvents LexicalVariablesEditor1 As LexicalVariablesEditor
End Class