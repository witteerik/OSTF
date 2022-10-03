<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GainEditControl
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
        Me.Content_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.EditMode_GroupBox = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel6 = New System.Windows.Forms.TableLayoutPanel()
        Me.EnableEdit_RadioButton = New System.Windows.Forms.RadioButton()
        Me.DiableEdit_RadioButton = New System.Windows.Forms.RadioButton()
        Me.Side_GroupBox = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
        Me.RightSide_RadioButton = New System.Windows.Forms.RadioButton()
        Me.LeftSide_RadioButton = New System.Windows.Forms.RadioButton()
        Me.Content_TableLayoutPanel.SuspendLayout()
        Me.EditMode_GroupBox.SuspendLayout()
        Me.TableLayoutPanel6.SuspendLayout()
        Me.Side_GroupBox.SuspendLayout()
        Me.TableLayoutPanel5.SuspendLayout()
        Me.SuspendLayout()
        '
        'Content_TableLayoutPanel
        '
        Me.Content_TableLayoutPanel.ColumnCount = 1
        Me.Content_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.Content_TableLayoutPanel.Controls.Add(Me.EditMode_GroupBox, 0, 0)
        Me.Content_TableLayoutPanel.Controls.Add(Me.Side_GroupBox, 0, 1)
        Me.Content_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Content_TableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.Content_TableLayoutPanel.Name = "Content_TableLayoutPanel"
        Me.Content_TableLayoutPanel.RowCount = 2
        Me.Content_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Content_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.Content_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.Content_TableLayoutPanel.Size = New System.Drawing.Size(555, 105)
        Me.Content_TableLayoutPanel.TabIndex = 3
        '
        'EditMode_GroupBox
        '
        Me.EditMode_GroupBox.Controls.Add(Me.TableLayoutPanel6)
        Me.EditMode_GroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.EditMode_GroupBox.Location = New System.Drawing.Point(3, 3)
        Me.EditMode_GroupBox.Name = "EditMode_GroupBox"
        Me.EditMode_GroupBox.Size = New System.Drawing.Size(549, 46)
        Me.EditMode_GroupBox.TabIndex = 3
        Me.EditMode_GroupBox.TabStop = False
        Me.EditMode_GroupBox.Text = "Edit mode"
        '
        'TableLayoutPanel6
        '
        Me.TableLayoutPanel6.ColumnCount = 2
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel6.Controls.Add(Me.EnableEdit_RadioButton, 0, 0)
        Me.TableLayoutPanel6.Controls.Add(Me.DiableEdit_RadioButton, 1, 0)
        Me.TableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel6.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel6.Name = "TableLayoutPanel6"
        Me.TableLayoutPanel6.RowCount = 1
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel6.Size = New System.Drawing.Size(543, 27)
        Me.TableLayoutPanel6.TabIndex = 2
        '
        'EnableEdit_RadioButton
        '
        Me.EnableEdit_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.EnableEdit_RadioButton.Location = New System.Drawing.Point(3, 3)
        Me.EnableEdit_RadioButton.Name = "EnableEdit_RadioButton"
        Me.EnableEdit_RadioButton.Size = New System.Drawing.Size(265, 21)
        Me.EnableEdit_RadioButton.TabIndex = 0
        Me.EnableEdit_RadioButton.Text = "Enable editing"
        Me.EnableEdit_RadioButton.UseVisualStyleBackColor = True
        '
        'DiableEdit_RadioButton
        '
        Me.DiableEdit_RadioButton.Checked = True
        Me.DiableEdit_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DiableEdit_RadioButton.Location = New System.Drawing.Point(274, 3)
        Me.DiableEdit_RadioButton.Name = "DiableEdit_RadioButton"
        Me.DiableEdit_RadioButton.Size = New System.Drawing.Size(266, 21)
        Me.DiableEdit_RadioButton.TabIndex = 1
        Me.DiableEdit_RadioButton.TabStop = True
        Me.DiableEdit_RadioButton.Text = "Disable editing"
        Me.DiableEdit_RadioButton.UseVisualStyleBackColor = True
        '
        'Side_GroupBox
        '
        Me.Side_GroupBox.Controls.Add(Me.TableLayoutPanel5)
        Me.Side_GroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Side_GroupBox.Enabled = False
        Me.Side_GroupBox.Location = New System.Drawing.Point(3, 55)
        Me.Side_GroupBox.Name = "Side_GroupBox"
        Me.Side_GroupBox.Size = New System.Drawing.Size(549, 47)
        Me.Side_GroupBox.TabIndex = 0
        Me.Side_GroupBox.TabStop = False
        Me.Side_GroupBox.Text = "Side"
        '
        'TableLayoutPanel5
        '
        Me.TableLayoutPanel5.ColumnCount = 2
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel5.Controls.Add(Me.RightSide_RadioButton, 0, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.LeftSide_RadioButton, 1, 0)
        Me.TableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 1
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(543, 28)
        Me.TableLayoutPanel5.TabIndex = 2
        '
        'RightSide_RadioButton
        '
        Me.RightSide_RadioButton.Checked = True
        Me.RightSide_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RightSide_RadioButton.Location = New System.Drawing.Point(3, 3)
        Me.RightSide_RadioButton.Name = "RightSide_RadioButton"
        Me.RightSide_RadioButton.Size = New System.Drawing.Size(265, 22)
        Me.RightSide_RadioButton.TabIndex = 1
        Me.RightSide_RadioButton.TabStop = True
        Me.RightSide_RadioButton.Text = "Right"
        Me.RightSide_RadioButton.UseVisualStyleBackColor = True
        '
        'LeftSide_RadioButton
        '
        Me.LeftSide_RadioButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LeftSide_RadioButton.Location = New System.Drawing.Point(274, 3)
        Me.LeftSide_RadioButton.Name = "LeftSide_RadioButton"
        Me.LeftSide_RadioButton.Size = New System.Drawing.Size(266, 22)
        Me.LeftSide_RadioButton.TabIndex = 0
        Me.LeftSide_RadioButton.Text = "Left"
        Me.LeftSide_RadioButton.UseVisualStyleBackColor = True
        '
        'GainEditControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Content_TableLayoutPanel)
        Me.Name = "GainEditControl"
        Me.Size = New System.Drawing.Size(555, 105)
        Me.Content_TableLayoutPanel.ResumeLayout(False)
        Me.EditMode_GroupBox.ResumeLayout(False)
        Me.TableLayoutPanel6.ResumeLayout(False)
        Me.Side_GroupBox.ResumeLayout(False)
        Me.TableLayoutPanel5.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Content_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents EditMode_GroupBox As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel6 As Windows.Forms.TableLayoutPanel
    Friend WithEvents EnableEdit_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents DiableEdit_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents Side_GroupBox As Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel5 As Windows.Forms.TableLayoutPanel
    Friend WithEvents RightSide_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents LeftSide_RadioButton As Windows.Forms.RadioButton
End Class
