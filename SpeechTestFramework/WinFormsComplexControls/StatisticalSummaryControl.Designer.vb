<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class StatisticalSummaryControl
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
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LoadedSpeechMaterialName_TextBox = New System.Windows.Forms.TextBox()
        Me.Variables_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.Calculate_Button = New System.Windows.Forms.Button()
        Me.SaveButton = New System.Windows.Forms.Button()
        Me.SourceLevel_ComboBox = New System.Windows.Forms.ComboBox()
        Me.ViewVariables_Button = New System.Windows.Forms.Button()
        Me.LoadSpeechMaterial_LoadFileControl = New SpeechTestFramework.LoadFileControl()
        Me.GroupBox2.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.TableLayoutPanel3)
        Me.GroupBox2.Location = New System.Drawing.Point(13, 14)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(344, 144)
        Me.GroupBox2.TabIndex = 7
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Load a speech material component file"
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.ColumnCount = 2
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.64307!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.35693!))
        Me.TableLayoutPanel3.Controls.Add(Me.LoadSpeechMaterial_LoadFileControl, 0, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.Label1, 0, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.LoadedSpeechMaterialName_TextBox, 1, 1)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 3
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(338, 125)
        Me.TableLayoutPanel3.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 63)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(124, 25)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Loaded speech material:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LoadedSpeechMaterialName_TextBox
        '
        Me.LoadedSpeechMaterialName_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LoadedSpeechMaterialName_TextBox.Location = New System.Drawing.Point(133, 66)
        Me.LoadedSpeechMaterialName_TextBox.Name = "LoadedSpeechMaterialName_TextBox"
        Me.LoadedSpeechMaterialName_TextBox.ReadOnly = True
        Me.LoadedSpeechMaterialName_TextBox.Size = New System.Drawing.Size(202, 20)
        Me.LoadedSpeechMaterialName_TextBox.TabIndex = 2
        Me.LoadedSpeechMaterialName_TextBox.Text = "No speech material loaded"
        '
        'Variables_TableLayoutPanel
        '
        Me.Variables_TableLayoutPanel.AutoScroll = True
        Me.Variables_TableLayoutPanel.ColumnCount = 1
        Me.Variables_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Variables_TableLayoutPanel.Location = New System.Drawing.Point(13, 164)
        Me.Variables_TableLayoutPanel.Name = "Variables_TableLayoutPanel"
        Me.Variables_TableLayoutPanel.RowCount = 1
        Me.Variables_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Variables_TableLayoutPanel.Size = New System.Drawing.Size(700, 312)
        Me.Variables_TableLayoutPanel.TabIndex = 8
        '
        'Calculate_Button
        '
        Me.Calculate_Button.Location = New System.Drawing.Point(50, 482)
        Me.Calculate_Button.Name = "Calculate_Button"
        Me.Calculate_Button.Size = New System.Drawing.Size(203, 23)
        Me.Calculate_Button.TabIndex = 9
        Me.Calculate_Button.Text = "Run summary calculations"
        Me.Calculate_Button.UseVisualStyleBackColor = True
        '
        'SaveButton
        '
        Me.SaveButton.Location = New System.Drawing.Point(351, 482)
        Me.SaveButton.Name = "SaveButton"
        Me.SaveButton.Size = New System.Drawing.Size(159, 23)
        Me.SaveButton.TabIndex = 10
        Me.SaveButton.Text = "Save"
        Me.SaveButton.UseVisualStyleBackColor = True
        '
        'SourceLevel_ComboBox
        '
        Me.SourceLevel_ComboBox.FormattingEnabled = True
        Me.SourceLevel_ComboBox.Location = New System.Drawing.Point(395, 59)
        Me.SourceLevel_ComboBox.Name = "SourceLevel_ComboBox"
        Me.SourceLevel_ComboBox.Size = New System.Drawing.Size(145, 21)
        Me.SourceLevel_ComboBox.TabIndex = 11
        '
        'ViewVariables_Button
        '
        Me.ViewVariables_Button.Location = New System.Drawing.Point(408, 112)
        Me.ViewVariables_Button.Name = "ViewVariables_Button"
        Me.ViewVariables_Button.Size = New System.Drawing.Size(132, 23)
        Me.ViewVariables_Button.TabIndex = 12
        Me.ViewVariables_Button.Text = "View variables"
        Me.ViewVariables_Button.UseVisualStyleBackColor = True
        '
        'LoadSpeechMaterial_LoadFileControl
        '
        Me.LoadSpeechMaterial_LoadFileControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LoadSpeechMaterial_LoadFileControl.ColumnCount = 2
        Me.TableLayoutPanel3.SetColumnSpan(Me.LoadSpeechMaterial_LoadFileControl, 2)
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadSpeechMaterial_LoadFileControl.Description = "Select file and press Load"
        Me.LoadSpeechMaterial_LoadFileControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LoadSpeechMaterial_LoadFileControl.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns
        Me.LoadSpeechMaterial_LoadFileControl.Location = New System.Drawing.Point(3, 3)
        Me.LoadSpeechMaterial_LoadFileControl.Name = "LoadSpeechMaterial_LoadFileControl"
        Me.LoadSpeechMaterial_LoadFileControl.RowCount = 2
        Me.LoadSpeechMaterial_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadSpeechMaterial_LoadFileControl.Size = New System.Drawing.Size(332, 57)
        Me.LoadSpeechMaterial_LoadFileControl.TabIndex = 0
        '
        'StatisticalSummaryControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.ViewVariables_Button)
        Me.Controls.Add(Me.SourceLevel_ComboBox)
        Me.Controls.Add(Me.SaveButton)
        Me.Controls.Add(Me.Calculate_Button)
        Me.Controls.Add(Me.Variables_TableLayoutPanel)
        Me.Controls.Add(Me.GroupBox2)
        Me.Name = "StatisticalSummaryControl"
        Me.Size = New System.Drawing.Size(737, 532)
        Me.GroupBox2.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TableLayoutPanel3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox2 As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel3 As Windows.Forms.TableLayoutPanel
    Friend WithEvents LoadSpeechMaterial_LoadFileControl As LoadFileControl
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents LoadedSpeechMaterialName_TextBox As Windows.Forms.TextBox
    Friend WithEvents Variables_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents Calculate_Button As Windows.Forms.Button
    Friend WithEvents SaveButton As Windows.Forms.Button
    Friend WithEvents SourceLevel_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents ViewVariables_Button As Windows.Forms.Button
End Class
