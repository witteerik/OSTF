﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class LexicalVariablesEditor
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.ProcessingGroupBox = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.Variables_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.AddAndSave_Button = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel6 = New System.Windows.Forms.TableLayoutPanel()
        Me.HasVowelContrast_CheckBox = New System.Windows.Forms.CheckBox()
        Me.ContrastedPhonemeIndex_CheckBox = New System.Windows.Forms.CheckBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
        Me.MatchBoth_RadioButton = New System.Windows.Forms.RadioButton()
        Me.MatchBySpellingOnly_RadioButton = New System.Windows.Forms.RadioButton()
        Me.MatchByTranscriptionOnly_RadioButton = New System.Windows.Forms.RadioButton()
        Me.LoadDatabase_GroupBox = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.TranscriptionVariableNameTextBox = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.SpellingVariableNameTextBox = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LoadDatabase_LoadFileControl = New SpeechTestFramework.LoadFileControl()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.IgnoreZeroPhonemesCheckBox = New System.Windows.Forms.CheckBox()
        Me.CaseInvariantLookupCheckBox = New System.Windows.Forms.CheckBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LoadedSpeechMaterialName_TextBox = New System.Windows.Forms.TextBox()
        Me.LoadOstaTestSpecificationControl1 = New SpeechTestFramework.LoadOstaTestSpecificationControl()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.ProcessingGroupBox.SuspendLayout()
        Me.TableLayoutPanel4.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TableLayoutPanel6.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.TableLayoutPanel5.SuspendLayout()
        Me.LoadDatabase_GroupBox.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.ProcessingGroupBox, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBox4, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.LoadDatabase_GroupBox, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBox3, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBox2, 0, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 107.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 79.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(880, 650)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'ProcessingGroupBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.ProcessingGroupBox, 3)
        Me.ProcessingGroupBox.Controls.Add(Me.TableLayoutPanel4)
        Me.ProcessingGroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ProcessingGroupBox.Location = New System.Drawing.Point(3, 189)
        Me.ProcessingGroupBox.Name = "ProcessingGroupBox"
        Me.ProcessingGroupBox.Size = New System.Drawing.Size(874, 458)
        Me.ProcessingGroupBox.TabIndex = 7
        Me.ProcessingGroupBox.TabStop = False
        '
        'TableLayoutPanel4
        '
        Me.TableLayoutPanel4.ColumnCount = 1
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel4.Controls.Add(Me.GroupBox5, 0, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.AddAndSave_Button, 0, 2)
        Me.TableLayoutPanel4.Controls.Add(Me.GroupBox1, 0, 1)
        Me.TableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel4.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
        Me.TableLayoutPanel4.RowCount = 3
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(868, 439)
        Me.TableLayoutPanel4.TabIndex = 0
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.Variables_TableLayoutPanel)
        Me.GroupBox5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox5.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(862, 293)
        Me.GroupBox5.TabIndex = 6
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Select lexical variables to add"
        '
        'Variables_TableLayoutPanel
        '
        Me.Variables_TableLayoutPanel.AutoScroll = True
        Me.Variables_TableLayoutPanel.ColumnCount = 1
        Me.Variables_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Variables_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Variables_TableLayoutPanel.Location = New System.Drawing.Point(3, 16)
        Me.Variables_TableLayoutPanel.Name = "Variables_TableLayoutPanel"
        Me.Variables_TableLayoutPanel.RowCount = 1
        Me.Variables_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Variables_TableLayoutPanel.Size = New System.Drawing.Size(856, 274)
        Me.Variables_TableLayoutPanel.TabIndex = 0
        '
        'AddAndSave_Button
        '
        Me.AddAndSave_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AddAndSave_Button.Location = New System.Drawing.Point(3, 402)
        Me.AddAndSave_Button.Name = "AddAndSave_Button"
        Me.AddAndSave_Button.Size = New System.Drawing.Size(862, 34)
        Me.AddAndSave_Button.TabIndex = 7
        Me.AddAndSave_Button.Text = "Add lexical data and save to files"
        Me.AddAndSave_Button.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.TableLayoutPanel6)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(3, 302)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(862, 94)
        Me.GroupBox1.TabIndex = 8
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Add logically derived variables"
        '
        'TableLayoutPanel6
        '
        Me.TableLayoutPanel6.ColumnCount = 1
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel6.Controls.Add(Me.HasVowelContrast_CheckBox, 0, 0)
        Me.TableLayoutPanel6.Controls.Add(Me.ContrastedPhonemeIndex_CheckBox, 0, 1)
        Me.TableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel6.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel6.Name = "TableLayoutPanel6"
        Me.TableLayoutPanel6.RowCount = 2
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel6.Size = New System.Drawing.Size(856, 75)
        Me.TableLayoutPanel6.TabIndex = 0
        '
        'HasVowelContrast_CheckBox
        '
        Me.HasVowelContrast_CheckBox.AutoSize = True
        Me.HasVowelContrast_CheckBox.Checked = True
        Me.HasVowelContrast_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.HasVowelContrast_CheckBox.Location = New System.Drawing.Point(3, 3)
        Me.HasVowelContrast_CheckBox.Name = "HasVowelContrast_CheckBox"
        Me.HasVowelContrast_CheckBox.Size = New System.Drawing.Size(537, 17)
        Me.HasVowelContrast_CheckBox.TabIndex = 0
        Me.HasVowelContrast_CheckBox.Text = "Create variable HasVowelContrast (For one word sentences containing minimal phone" &
    "tic/phonemic variation)"
        Me.HasVowelContrast_CheckBox.UseVisualStyleBackColor = True
        '
        'ContrastedPhonemeIndex_CheckBox
        '
        Me.ContrastedPhonemeIndex_CheckBox.AutoSize = True
        Me.ContrastedPhonemeIndex_CheckBox.Checked = True
        Me.ContrastedPhonemeIndex_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ContrastedPhonemeIndex_CheckBox.Location = New System.Drawing.Point(3, 40)
        Me.ContrastedPhonemeIndex_CheckBox.Name = "ContrastedPhonemeIndex_CheckBox"
        Me.ContrastedPhonemeIndex_CheckBox.Size = New System.Drawing.Size(574, 17)
        Me.ContrastedPhonemeIndex_CheckBox.TabIndex = 1
        Me.ContrastedPhonemeIndex_CheckBox.Text = "Create variable ContrastingPhonemeIndex (For one word sentences containing minima" &
    "l phonetic/phonemic variation)"
        Me.ContrastedPhonemeIndex_CheckBox.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.TableLayoutPanel5)
        Me.GroupBox4.Location = New System.Drawing.Point(353, 3)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(174, 101)
        Me.GroupBox4.TabIndex = 4
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Match words by:"
        '
        'TableLayoutPanel5
        '
        Me.TableLayoutPanel5.ColumnCount = 1
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel5.Controls.Add(Me.MatchBoth_RadioButton, 0, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.MatchBySpellingOnly_RadioButton, 0, 1)
        Me.TableLayoutPanel5.Controls.Add(Me.MatchByTranscriptionOnly_RadioButton, 0, 2)
        Me.TableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 3
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(168, 82)
        Me.TableLayoutPanel5.TabIndex = 0
        '
        'MatchBoth_RadioButton
        '
        Me.MatchBoth_RadioButton.AutoSize = True
        Me.MatchBoth_RadioButton.Location = New System.Drawing.Point(3, 3)
        Me.MatchBoth_RadioButton.Name = "MatchBoth_RadioButton"
        Me.MatchBoth_RadioButton.Size = New System.Drawing.Size(143, 17)
        Me.MatchBoth_RadioButton.TabIndex = 0
        Me.MatchBoth_RadioButton.TabStop = True
        Me.MatchBoth_RadioButton.Text = "Spelling and transcription"
        Me.MatchBoth_RadioButton.UseVisualStyleBackColor = True
        '
        'MatchBySpellingOnly_RadioButton
        '
        Me.MatchBySpellingOnly_RadioButton.AutoSize = True
        Me.MatchBySpellingOnly_RadioButton.Location = New System.Drawing.Point(3, 30)
        Me.MatchBySpellingOnly_RadioButton.Name = "MatchBySpellingOnly_RadioButton"
        Me.MatchBySpellingOnly_RadioButton.Size = New System.Drawing.Size(84, 17)
        Me.MatchBySpellingOnly_RadioButton.TabIndex = 1
        Me.MatchBySpellingOnly_RadioButton.TabStop = True
        Me.MatchBySpellingOnly_RadioButton.Text = "Spelling only"
        Me.MatchBySpellingOnly_RadioButton.UseVisualStyleBackColor = True
        '
        'MatchByTranscriptionOnly_RadioButton
        '
        Me.MatchByTranscriptionOnly_RadioButton.AutoSize = True
        Me.MatchByTranscriptionOnly_RadioButton.Location = New System.Drawing.Point(3, 57)
        Me.MatchByTranscriptionOnly_RadioButton.Name = "MatchByTranscriptionOnly_RadioButton"
        Me.MatchByTranscriptionOnly_RadioButton.Size = New System.Drawing.Size(108, 17)
        Me.MatchByTranscriptionOnly_RadioButton.TabIndex = 2
        Me.MatchByTranscriptionOnly_RadioButton.TabStop = True
        Me.MatchByTranscriptionOnly_RadioButton.Text = "Transcription only"
        Me.MatchByTranscriptionOnly_RadioButton.UseVisualStyleBackColor = True
        '
        'LoadDatabase_GroupBox
        '
        Me.LoadDatabase_GroupBox.Controls.Add(Me.TableLayoutPanel2)
        Me.LoadDatabase_GroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LoadDatabase_GroupBox.Location = New System.Drawing.Point(533, 3)
        Me.LoadDatabase_GroupBox.Name = "LoadDatabase_GroupBox"
        Me.TableLayoutPanel1.SetRowSpan(Me.LoadDatabase_GroupBox, 2)
        Me.LoadDatabase_GroupBox.Size = New System.Drawing.Size(344, 180)
        Me.LoadDatabase_GroupBox.TabIndex = 5
        Me.LoadDatabase_GroupBox.TabStop = False
        Me.LoadDatabase_GroupBox.Text = "Load a lexical database"
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.TranscriptionVariableNameTextBox, 1, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.Label5, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.SpellingVariableNameTextBox, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.Label4, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.LoadDatabase_LoadFileControl, 0, 2)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 4
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(338, 161)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'TranscriptionVariableNameTextBox
        '
        Me.TranscriptionVariableNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TranscriptionVariableNameTextBox.Location = New System.Drawing.Point(172, 28)
        Me.TranscriptionVariableNameTextBox.Name = "TranscriptionVariableNameTextBox"
        Me.TranscriptionVariableNameTextBox.Size = New System.Drawing.Size(163, 20)
        Me.TranscriptionVariableNameTextBox.TabIndex = 5
        Me.TranscriptionVariableNameTextBox.Text = "Transcription"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label5.Location = New System.Drawing.Point(3, 25)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(163, 25)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Transcription variable name:"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SpellingVariableNameTextBox
        '
        Me.SpellingVariableNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SpellingVariableNameTextBox.Location = New System.Drawing.Point(172, 3)
        Me.SpellingVariableNameTextBox.Name = "SpellingVariableNameTextBox"
        Me.SpellingVariableNameTextBox.Size = New System.Drawing.Size(163, 20)
        Me.SpellingVariableNameTextBox.TabIndex = 2
        Me.SpellingVariableNameTextBox.Text = "Spelling"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Location = New System.Drawing.Point(3, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(163, 25)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "Spelling variable name:"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LoadDatabase_LoadFileControl
        '
        Me.LoadDatabase_LoadFileControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LoadDatabase_LoadFileControl.ColumnCount = 2
        Me.TableLayoutPanel2.SetColumnSpan(Me.LoadDatabase_LoadFileControl, 2)
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.62791!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.37209!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.LoadDatabase_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96.0!))
        Me.LoadDatabase_LoadFileControl.Description = "Select file and press Load"
        Me.LoadDatabase_LoadFileControl.DirectionType = SpeechTestFramework.PathTextBox.DirectionTypes.Load
        Me.LoadDatabase_LoadFileControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LoadDatabase_LoadFileControl.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns
        Me.LoadDatabase_LoadFileControl.Location = New System.Drawing.Point(3, 53)
        Me.LoadDatabase_LoadFileControl.Name = "LoadDatabase_LoadFileControl"
        Me.LoadDatabase_LoadFileControl.PathType = SpeechTestFramework.PathTextBox.PathTypes.File
        Me.LoadDatabase_LoadFileControl.RowCount = 2
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadDatabase_LoadFileControl.Size = New System.Drawing.Size(332, 57)
        Me.LoadDatabase_LoadFileControl.TabIndex = 6
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.IgnoreZeroPhonemesCheckBox)
        Me.GroupBox3.Controls.Add(Me.CaseInvariantLookupCheckBox)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox3.Location = New System.Drawing.Point(353, 110)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.GroupBox3.Size = New System.Drawing.Size(174, 73)
        Me.GroupBox3.TabIndex = 5
        Me.GroupBox3.TabStop = False
        '
        'IgnoreZeroPhonemesCheckBox
        '
        Me.IgnoreZeroPhonemesCheckBox.AutoSize = True
        Me.IgnoreZeroPhonemesCheckBox.Location = New System.Drawing.Point(6, 39)
        Me.IgnoreZeroPhonemesCheckBox.Name = "IgnoreZeroPhonemesCheckBox"
        Me.IgnoreZeroPhonemesCheckBox.Size = New System.Drawing.Size(162, 17)
        Me.IgnoreZeroPhonemesCheckBox.TabIndex = 4
        Me.IgnoreZeroPhonemesCheckBox.Text = "Ignore phonetic character ∅"
        Me.IgnoreZeroPhonemesCheckBox.UseVisualStyleBackColor = True
        '
        'CaseInvariantLookupCheckBox
        '
        Me.CaseInvariantLookupCheckBox.AutoSize = True
        Me.CaseInvariantLookupCheckBox.Checked = True
        Me.CaseInvariantLookupCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CaseInvariantLookupCheckBox.Location = New System.Drawing.Point(6, 15)
        Me.CaseInvariantLookupCheckBox.Name = "CaseInvariantLookupCheckBox"
        Me.CaseInvariantLookupCheckBox.Size = New System.Drawing.Size(131, 17)
        Me.CaseInvariantLookupCheckBox.TabIndex = 3
        Me.CaseInvariantLookupCheckBox.Text = "Case-invariant look up"
        Me.CaseInvariantLookupCheckBox.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.TableLayoutPanel3)
        Me.GroupBox2.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox2.Name = "GroupBox2"
        Me.TableLayoutPanel1.SetRowSpan(Me.GroupBox2, 2)
        Me.GroupBox2.Size = New System.Drawing.Size(344, 144)
        Me.GroupBox2.TabIndex = 6
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Search for and then load a speech test"
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.ColumnCount = 2
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.64307!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.35693!))
        Me.TableLayoutPanel3.Controls.Add(Me.Label1, 0, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.LoadedSpeechMaterialName_TextBox, 1, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.LoadOstaTestSpecificationControl1, 0, 0)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 3
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(338, 125)
        Me.TableLayoutPanel3.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 60)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(124, 28)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Loaded speech material:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LoadedSpeechMaterialName_TextBox
        '
        Me.LoadedSpeechMaterialName_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LoadedSpeechMaterialName_TextBox.Location = New System.Drawing.Point(133, 63)
        Me.LoadedSpeechMaterialName_TextBox.Name = "LoadedSpeechMaterialName_TextBox"
        Me.LoadedSpeechMaterialName_TextBox.ReadOnly = True
        Me.LoadedSpeechMaterialName_TextBox.Size = New System.Drawing.Size(202, 20)
        Me.LoadedSpeechMaterialName_TextBox.TabIndex = 2
        Me.LoadedSpeechMaterialName_TextBox.Text = "No speech material loaded"
        '
        'LoadOstaTestSpecificationControl1
        '
        Me.TableLayoutPanel3.SetColumnSpan(Me.LoadOstaTestSpecificationControl1, 2)
        Me.LoadOstaTestSpecificationControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LoadOstaTestSpecificationControl1.Location = New System.Drawing.Point(3, 3)
        Me.LoadOstaTestSpecificationControl1.Name = "LoadOstaTestSpecificationControl1"
        Me.LoadOstaTestSpecificationControl1.SelectedTestSpecification = Nothing
        Me.LoadOstaTestSpecificationControl1.Size = New System.Drawing.Size(332, 54)
        Me.LoadOstaTestSpecificationControl1.TabIndex = 8
        '
        'LexicalVariablesEditor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "LexicalVariablesEditor"
        Me.Size = New System.Drawing.Size(880, 650)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ProcessingGroupBox.ResumeLayout(False)
        Me.TableLayoutPanel4.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.TableLayoutPanel6.ResumeLayout(False)
        Me.TableLayoutPanel6.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.TableLayoutPanel5.ResumeLayout(False)
        Me.TableLayoutPanel5.PerformLayout()
        Me.LoadDatabase_GroupBox.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TableLayoutPanel3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents LoadDatabase_GroupBox As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel2 As Windows.Forms.TableLayoutPanel
    Friend WithEvents TranscriptionVariableNameTextBox As Windows.Forms.TextBox
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents SpellingVariableNameTextBox As Windows.Forms.TextBox
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents CaseInvariantLookupCheckBox As Windows.Forms.CheckBox
    Friend WithEvents LoadDatabase_LoadFileControl As LoadFileControl
    Friend WithEvents GroupBox2 As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel3 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents LoadedSpeechMaterialName_TextBox As Windows.Forms.TextBox
    Friend WithEvents ProcessingGroupBox As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel4 As Windows.Forms.TableLayoutPanel
    Friend WithEvents GroupBox4 As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel5 As Windows.Forms.TableLayoutPanel
    Friend WithEvents MatchBoth_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents MatchBySpellingOnly_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents MatchByTranscriptionOnly_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents GroupBox3 As Windows.Forms.GroupBox
    Friend WithEvents GroupBox5 As Windows.Forms.GroupBox
    Friend WithEvents Variables_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents AddAndSave_Button As Windows.Forms.Button
    Friend WithEvents LoadOstaTestSpecificationControl1 As LoadOstaTestSpecificationControl
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel6 As Windows.Forms.TableLayoutPanel
    Friend WithEvents HasVowelContrast_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents ContrastedPhonemeIndex_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents IgnoreZeroPhonemesCheckBox As Windows.Forms.CheckBox
End Class
