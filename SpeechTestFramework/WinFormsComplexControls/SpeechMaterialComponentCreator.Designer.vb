﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SpeechMaterialComponentCreator
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
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.EditRichTextBox = New System.Windows.Forms.RichTextBox()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.SelectFontToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.NameTextBox = New System.Windows.Forms.TextBox()
        Me.SoundFileLevelComboBox = New System.Windows.Forms.ComboBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.TranscriptionVariableNameTextBox = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.SpellingVariableNameTextBox = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TranscriptionDatabase_FilePathControl = New SpeechTestFramework.ExistingFilePathControl()
        Me.CaseInvariantLookupCheckBox = New System.Windows.Forms.CheckBox()
        Me.TranscriptionLookupButton = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.PhoneTrimChars_TextBox = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.WordTrimChars_TextBox = New System.Windows.Forms.TextBox()
        Me.OrderedSentencesCheckBox = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.CheckInputButton = New System.Windows.Forms.Button()
        Me.CreateSpeechMaterialComponentFile_Button = New System.Windows.Forms.Button()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.IsSplitterFixed = True
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer2)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.FlowLayoutPanel1)
        Me.SplitContainer1.Size = New System.Drawing.Size(819, 555)
        Me.SplitContainer1.SplitterDistance = 452
        Me.SplitContainer1.TabIndex = 0
        '
        'SplitContainer2
        '
        Me.SplitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.SplitContainer3)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.TableLayoutPanel1)
        Me.SplitContainer2.Size = New System.Drawing.Size(819, 452)
        Me.SplitContainer2.SplitterDistance = 484
        Me.SplitContainer2.TabIndex = 0
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.IsSplitterFixed = True
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        Me.SplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.Label1)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.EditRichTextBox)
        Me.SplitContainer3.Panel2.Controls.Add(Me.MenuStrip1)
        Me.SplitContainer3.Size = New System.Drawing.Size(482, 450)
        Me.SplitContainer3.SplitterDistance = 25
        Me.SplitContainer3.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(0, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(482, 25)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Speech material input box"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'EditRichTextBox
        '
        Me.EditRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.EditRichTextBox.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.EditRichTextBox.Location = New System.Drawing.Point(0, 24)
        Me.EditRichTextBox.Name = "EditRichTextBox"
        Me.EditRichTextBox.Size = New System.Drawing.Size(482, 397)
        Me.EditRichTextBox.TabIndex = 0
        Me.EditRichTextBox.Text = ""
        Me.EditRichTextBox.WordWrap = False
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SelectFontToolStripMenuItem})
        Me.MenuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.MenuStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.MenuStrip1.Size = New System.Drawing.Size(482, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'SelectFontToolStripMenuItem
        '
        Me.SelectFontToolStripMenuItem.Name = "SelectFontToolStripMenuItem"
        Me.SelectFontToolStripMenuItem.Size = New System.Drawing.Size(75, 20)
        Me.SelectFontToolStripMenuItem.Text = "Select font"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.31722!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 54.68278!))
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.NameTextBox, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.SoundFileLevelComboBox, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBox1, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.Label6, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.WordTrimChars_TextBox, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.OrderedSentencesCheckBox, 0, 2)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 6
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 220.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(329, 450)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(3, 25)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(143, 25)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Sound files at linguistic level:"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(3, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(143, 25)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Name:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'NameTextBox
        '
        Me.NameTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NameTextBox.Location = New System.Drawing.Point(152, 3)
        Me.NameTextBox.Name = "NameTextBox"
        Me.NameTextBox.Size = New System.Drawing.Size(174, 20)
        Me.NameTextBox.TabIndex = 1
        '
        'SoundFileLevelComboBox
        '
        Me.SoundFileLevelComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SoundFileLevelComboBox.FormattingEnabled = True
        Me.SoundFileLevelComboBox.Location = New System.Drawing.Point(152, 28)
        Me.SoundFileLevelComboBox.Name = "SoundFileLevelComboBox"
        Me.SoundFileLevelComboBox.Size = New System.Drawing.Size(174, 21)
        Me.SoundFileLevelComboBox.TabIndex = 3
        '
        'GroupBox1
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.GroupBox1, 2)
        Me.GroupBox1.Controls.Add(Me.TableLayoutPanel2)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(3, 106)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(323, 214)
        Me.GroupBox1.TabIndex = 4
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Transcription database"
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.TranscriptionVariableNameTextBox, 1, 3)
        Me.TableLayoutPanel2.Controls.Add(Me.Label5, 0, 3)
        Me.TableLayoutPanel2.Controls.Add(Me.SpellingVariableNameTextBox, 1, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.Label4, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.TranscriptionDatabase_FilePathControl, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.CaseInvariantLookupCheckBox, 0, 2)
        Me.TableLayoutPanel2.Controls.Add(Me.TranscriptionLookupButton, 0, 5)
        Me.TableLayoutPanel2.Controls.Add(Me.Label7, 0, 4)
        Me.TableLayoutPanel2.Controls.Add(Me.PhoneTrimChars_TextBox, 1, 4)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 6
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(317, 195)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'TranscriptionVariableNameTextBox
        '
        Me.TranscriptionVariableNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TranscriptionVariableNameTextBox.Location = New System.Drawing.Point(161, 116)
        Me.TranscriptionVariableNameTextBox.Name = "TranscriptionVariableNameTextBox"
        Me.TranscriptionVariableNameTextBox.Size = New System.Drawing.Size(153, 20)
        Me.TranscriptionVariableNameTextBox.TabIndex = 5
        Me.TranscriptionVariableNameTextBox.Text = "Transcription"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label5.Location = New System.Drawing.Point(3, 113)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(152, 25)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Transcription variable name:"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SpellingVariableNameTextBox
        '
        Me.SpellingVariableNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SpellingVariableNameTextBox.Location = New System.Drawing.Point(161, 65)
        Me.SpellingVariableNameTextBox.Name = "SpellingVariableNameTextBox"
        Me.SpellingVariableNameTextBox.Size = New System.Drawing.Size(153, 20)
        Me.SpellingVariableNameTextBox.TabIndex = 2
        Me.SpellingVariableNameTextBox.Text = "Spelling"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Location = New System.Drawing.Point(3, 62)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(152, 25)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "Spelling variable name:"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TranscriptionDatabase_FilePathControl
        '
        Me.TranscriptionDatabase_FilePathControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TranscriptionDatabase_FilePathControl.ColumnCount = 2
        Me.TableLayoutPanel2.SetColumnSpan(Me.TranscriptionDatabase_FilePathControl, 2)
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75.21368!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.78633!))
        Me.TranscriptionDatabase_FilePathControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 9.0!))
        Me.TranscriptionDatabase_FilePathControl.Description = "File path"
        Me.TranscriptionDatabase_FilePathControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TranscriptionDatabase_FilePathControl.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns
        Me.TranscriptionDatabase_FilePathControl.Location = New System.Drawing.Point(3, 3)
        Me.TranscriptionDatabase_FilePathControl.Name = "TranscriptionDatabase_FilePathControl"
        Me.TranscriptionDatabase_FilePathControl.RowCount = 2
        Me.TranscriptionDatabase_FilePathControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TranscriptionDatabase_FilePathControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TranscriptionDatabase_FilePathControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TranscriptionDatabase_FilePathControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TranscriptionDatabase_FilePathControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TranscriptionDatabase_FilePathControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TranscriptionDatabase_FilePathControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TranscriptionDatabase_FilePathControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TranscriptionDatabase_FilePathControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TranscriptionDatabase_FilePathControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TranscriptionDatabase_FilePathControl.Size = New System.Drawing.Size(311, 56)
        Me.TranscriptionDatabase_FilePathControl.TabIndex = 0
        '
        'CaseInvariantLookupCheckBox
        '
        Me.CaseInvariantLookupCheckBox.AutoSize = True
        Me.TableLayoutPanel2.SetColumnSpan(Me.CaseInvariantLookupCheckBox, 2)
        Me.CaseInvariantLookupCheckBox.Location = New System.Drawing.Point(3, 90)
        Me.CaseInvariantLookupCheckBox.Name = "CaseInvariantLookupCheckBox"
        Me.CaseInvariantLookupCheckBox.Size = New System.Drawing.Size(131, 17)
        Me.CaseInvariantLookupCheckBox.TabIndex = 3
        Me.CaseInvariantLookupCheckBox.Text = "Case-invariant look up"
        Me.CaseInvariantLookupCheckBox.UseVisualStyleBackColor = True
        '
        'TranscriptionLookupButton
        '
        Me.TableLayoutPanel2.SetColumnSpan(Me.TranscriptionLookupButton, 2)
        Me.TranscriptionLookupButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TranscriptionLookupButton.Location = New System.Drawing.Point(3, 169)
        Me.TranscriptionLookupButton.Name = "TranscriptionLookupButton"
        Me.TranscriptionLookupButton.Size = New System.Drawing.Size(311, 24)
        Me.TranscriptionLookupButton.TabIndex = 6
        Me.TranscriptionLookupButton.Text = "Look up transcriptions"
        Me.TranscriptionLookupButton.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label7.Location = New System.Drawing.Point(3, 138)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(152, 28)
        Me.Label7.TabIndex = 7
        Me.Label7.Text = "Phoneme level trimmed chars:"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PhoneTrimChars_TextBox
        '
        Me.PhoneTrimChars_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PhoneTrimChars_TextBox.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PhoneTrimChars_TextBox.Location = New System.Drawing.Point(161, 141)
        Me.PhoneTrimChars_TextBox.Name = "PhoneTrimChars_TextBox"
        Me.PhoneTrimChars_TextBox.Size = New System.Drawing.Size(153, 22)
        Me.PhoneTrimChars_TextBox.TabIndex = 8
        '
        'Label6
        '
        Me.Label6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label6.Location = New System.Drawing.Point(3, 75)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(143, 28)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "Word level trimmed chars:"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'WordTrimChars_TextBox
        '
        Me.WordTrimChars_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WordTrimChars_TextBox.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WordTrimChars_TextBox.Location = New System.Drawing.Point(152, 78)
        Me.WordTrimChars_TextBox.Name = "WordTrimChars_TextBox"
        Me.WordTrimChars_TextBox.Size = New System.Drawing.Size(174, 22)
        Me.WordTrimChars_TextBox.TabIndex = 7
        '
        'OrderedSentencesCheckBox
        '
        Me.OrderedSentencesCheckBox.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.OrderedSentencesCheckBox, 2)
        Me.OrderedSentencesCheckBox.Location = New System.Drawing.Point(3, 53)
        Me.OrderedSentencesCheckBox.Name = "OrderedSentencesCheckBox"
        Me.OrderedSentencesCheckBox.Size = New System.Drawing.Size(173, 17)
        Me.OrderedSentencesCheckBox.TabIndex = 5
        Me.OrderedSentencesCheckBox.Text = "Lists have ordered constituents"
        Me.OrderedSentencesCheckBox.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Controls.Add(Me.CheckInputButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.CreateSpeechMaterialComponentFile_Button)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(819, 99)
        Me.FlowLayoutPanel1.TabIndex = 0
        '
        'CheckInputButton
        '
        Me.CheckInputButton.Location = New System.Drawing.Point(3, 3)
        Me.CheckInputButton.Name = "CheckInputButton"
        Me.CheckInputButton.Size = New System.Drawing.Size(173, 42)
        Me.CheckInputButton.TabIndex = 0
        Me.CheckInputButton.Text = "Check speech material input"
        Me.CheckInputButton.UseVisualStyleBackColor = True
        '
        'CreateSpeechMaterialComponentFile_Button
        '
        Me.CreateSpeechMaterialComponentFile_Button.Enabled = False
        Me.CreateSpeechMaterialComponentFile_Button.Location = New System.Drawing.Point(182, 3)
        Me.CreateSpeechMaterialComponentFile_Button.Name = "CreateSpeechMaterialComponentFile_Button"
        Me.CreateSpeechMaterialComponentFile_Button.Size = New System.Drawing.Size(173, 42)
        Me.CreateSpeechMaterialComponentFile_Button.TabIndex = 1
        Me.CreateSpeechMaterialComponentFile_Button.Text = "Create speech-material component file"
        Me.CreateSpeechMaterialComponentFile_Button.UseVisualStyleBackColor = True
        '
        'SpeechMaterialComponentCreator
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "SpeechMaterialComponentCreator"
        Me.Size = New System.Drawing.Size(819, 555)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        Me.SplitContainer3.Panel2.PerformLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SplitContainer1 As Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer2 As Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer3 As Windows.Forms.SplitContainer
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents EditRichTextBox As Windows.Forms.RichTextBox
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents NameTextBox As Windows.Forms.TextBox
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents SoundFileLevelComboBox As Windows.Forms.ComboBox
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel2 As Windows.Forms.TableLayoutPanel
    Friend WithEvents TranscriptionDatabase_FilePathControl As ExistingFilePathControl
    Friend WithEvents TranscriptionVariableNameTextBox As Windows.Forms.TextBox
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents SpellingVariableNameTextBox As Windows.Forms.TextBox
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents CaseInvariantLookupCheckBox As Windows.Forms.CheckBox
    Friend WithEvents TranscriptionLookupButton As Windows.Forms.Button
    Friend WithEvents FlowLayoutPanel1 As Windows.Forms.FlowLayoutPanel
    Friend WithEvents CheckInputButton As Windows.Forms.Button
    Friend WithEvents CreateSpeechMaterialComponentFile_Button As Windows.Forms.Button
    Friend WithEvents OrderedSentencesCheckBox As Windows.Forms.CheckBox
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents WordTrimChars_TextBox As Windows.Forms.TextBox
    Friend WithEvents Label7 As Windows.Forms.Label
    Friend WithEvents PhoneTrimChars_TextBox As Windows.Forms.TextBox
    Friend WithEvents MenuStrip1 As Windows.Forms.MenuStrip
    Friend WithEvents SelectFontToolStripMenuItem As Windows.Forms.ToolStripMenuItem
End Class