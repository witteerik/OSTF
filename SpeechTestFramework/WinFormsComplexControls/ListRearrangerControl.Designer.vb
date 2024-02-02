<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ListRearrangerControl
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.RearrangeAcrossLists_RadioButton = New System.Windows.Forms.RadioButton()
        Me.RearrangeWithinLists_RadioButton = New System.Windows.Forms.RadioButton()
        Me.ListDescriptives_GroupBox = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ListNamePrefix_TextBox = New System.Windows.Forms.TextBox()
        Me.ListLength_IntegerParsingTextBox = New SpeechTestFramework.IntegerParsingTextBox()
        Me.Order_GroupBox = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.RandomOrder_RadioButton = New System.Windows.Forms.RadioButton()
        Me.OriginalOrder_RadioButton = New System.Windows.Forms.RadioButton()
        Me.BalancedOrder_RadioButton = New System.Windows.Forms.RadioButton()
        Me.CustomVariablesSelection_GroupBox = New System.Windows.Forms.GroupBox()
        Me.CustomVariablesSelection_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.NewMediaSetName_TextBox = New System.Windows.Forms.TextBox()
        Me.ReArrangeButton = New System.Windows.Forms.Button()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.NewSpeechMaterialName_TextBox = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.ListDescriptives_GroupBox.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.Order_GroupBox.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.CustomVariablesSelection_GroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBox1, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.ListDescriptives_GroupBox, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Order_GroupBox, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.ReArrangeButton, 0, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.NewMediaSetName_TextBox, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.NewSpeechMaterialName_TextBox, 1, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 6
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 76.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 77.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(544, 518)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'GroupBox1
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.GroupBox1, 2)
        Me.GroupBox1.Controls.Add(Me.RearrangeAcrossLists_RadioButton)
        Me.GroupBox1.Controls.Add(Me.RearrangeWithinLists_RadioButton)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(3, 53)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(538, 70)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Scope"
        '
        'RearrangeAcrossLists_RadioButton
        '
        Me.RearrangeAcrossLists_RadioButton.AutoSize = True
        Me.RearrangeAcrossLists_RadioButton.Location = New System.Drawing.Point(7, 43)
        Me.RearrangeAcrossLists_RadioButton.Name = "RearrangeAcrossLists_RadioButton"
        Me.RearrangeAcrossLists_RadioButton.Size = New System.Drawing.Size(132, 17)
        Me.RearrangeAcrossLists_RadioButton.TabIndex = 1
        Me.RearrangeAcrossLists_RadioButton.TabStop = True
        Me.RearrangeAcrossLists_RadioButton.Text = "Re-arrange across lists"
        Me.RearrangeAcrossLists_RadioButton.UseVisualStyleBackColor = True
        '
        'RearrangeWithinLists_RadioButton
        '
        Me.RearrangeWithinLists_RadioButton.AutoSize = True
        Me.RearrangeWithinLists_RadioButton.Location = New System.Drawing.Point(7, 19)
        Me.RearrangeWithinLists_RadioButton.Name = "RearrangeWithinLists_RadioButton"
        Me.RearrangeWithinLists_RadioButton.Size = New System.Drawing.Size(128, 17)
        Me.RearrangeWithinLists_RadioButton.TabIndex = 0
        Me.RearrangeWithinLists_RadioButton.TabStop = True
        Me.RearrangeWithinLists_RadioButton.Text = "Re-arrange within lists"
        Me.RearrangeWithinLists_RadioButton.UseVisualStyleBackColor = True
        '
        'ListDescriptives_GroupBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.ListDescriptives_GroupBox, 2)
        Me.ListDescriptives_GroupBox.Controls.Add(Me.TableLayoutPanel2)
        Me.ListDescriptives_GroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListDescriptives_GroupBox.Location = New System.Drawing.Point(3, 129)
        Me.ListDescriptives_GroupBox.Name = "ListDescriptives_GroupBox"
        Me.ListDescriptives_GroupBox.Size = New System.Drawing.Size(538, 71)
        Me.ListDescriptives_GroupBox.TabIndex = 1
        Me.ListDescriptives_GroupBox.TabStop = False
        Me.ListDescriptives_GroupBox.Text = "List names and lengths"
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.Label2, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.Label3, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.ListNamePrefix_TextBox, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.ListLength_IntegerParsingTextBox, 1, 1)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 2
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(532, 52)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(3, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(260, 25)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "List name prefix"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(3, 25)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(260, 27)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "Target list length"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ListNamePrefix_TextBox
        '
        Me.ListNamePrefix_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListNamePrefix_TextBox.Location = New System.Drawing.Point(269, 3)
        Me.ListNamePrefix_TextBox.Name = "ListNamePrefix_TextBox"
        Me.ListNamePrefix_TextBox.Size = New System.Drawing.Size(260, 20)
        Me.ListNamePrefix_TextBox.TabIndex = 2
        '
        'ListLength_IntegerParsingTextBox
        '
        Me.ListLength_IntegerParsingTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListLength_IntegerParsingTextBox.ForeColor = System.Drawing.Color.Red
        Me.ListLength_IntegerParsingTextBox.Location = New System.Drawing.Point(269, 28)
        Me.ListLength_IntegerParsingTextBox.Name = "ListLength_IntegerParsingTextBox"
        Me.ListLength_IntegerParsingTextBox.Size = New System.Drawing.Size(260, 20)
        Me.ListLength_IntegerParsingTextBox.TabIndex = 3
        '
        'Order_GroupBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.Order_GroupBox, 2)
        Me.Order_GroupBox.Controls.Add(Me.TableLayoutPanel3)
        Me.Order_GroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Order_GroupBox.Location = New System.Drawing.Point(3, 206)
        Me.Order_GroupBox.Name = "Order_GroupBox"
        Me.Order_GroupBox.Size = New System.Drawing.Size(538, 274)
        Me.Order_GroupBox.TabIndex = 2
        Me.Order_GroupBox.TabStop = False
        Me.Order_GroupBox.Text = "Order"
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.ColumnCount = 2
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel3.Controls.Add(Me.RandomOrder_RadioButton, 0, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.OriginalOrder_RadioButton, 0, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.BalancedOrder_RadioButton, 0, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.CustomVariablesSelection_GroupBox, 1, 0)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 4
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(532, 255)
        Me.TableLayoutPanel3.TabIndex = 4
        '
        'RandomOrder_RadioButton
        '
        Me.RandomOrder_RadioButton.AutoSize = True
        Me.RandomOrder_RadioButton.Location = New System.Drawing.Point(3, 3)
        Me.RandomOrder_RadioButton.Name = "RandomOrder_RadioButton"
        Me.RandomOrder_RadioButton.Size = New System.Drawing.Size(92, 17)
        Me.RandomOrder_RadioButton.TabIndex = 0
        Me.RandomOrder_RadioButton.TabStop = True
        Me.RandomOrder_RadioButton.Text = "Random order"
        Me.RandomOrder_RadioButton.UseVisualStyleBackColor = True
        '
        'OriginalOrder_RadioButton
        '
        Me.OriginalOrder_RadioButton.AutoSize = True
        Me.OriginalOrder_RadioButton.Location = New System.Drawing.Point(3, 29)
        Me.OriginalOrder_RadioButton.Name = "OriginalOrder_RadioButton"
        Me.OriginalOrder_RadioButton.Size = New System.Drawing.Size(87, 17)
        Me.OriginalOrder_RadioButton.TabIndex = 1
        Me.OriginalOrder_RadioButton.TabStop = True
        Me.OriginalOrder_RadioButton.Text = "Original order"
        Me.OriginalOrder_RadioButton.UseVisualStyleBackColor = True
        '
        'BalancedOrder_RadioButton
        '
        Me.BalancedOrder_RadioButton.AutoSize = True
        Me.BalancedOrder_RadioButton.Location = New System.Drawing.Point(3, 54)
        Me.BalancedOrder_RadioButton.Name = "BalancedOrder_RadioButton"
        Me.BalancedOrder_RadioButton.Size = New System.Drawing.Size(210, 17)
        Me.BalancedOrder_RadioButton.TabIndex = 2
        Me.BalancedOrder_RadioButton.TabStop = True
        Me.BalancedOrder_RadioButton.Text = "Balance custom variables between lists"
        Me.BalancedOrder_RadioButton.UseVisualStyleBackColor = True
        '
        'CustomVariablesSelection_GroupBox
        '
        Me.CustomVariablesSelection_GroupBox.Controls.Add(Me.CustomVariablesSelection_TableLayoutPanel)
        Me.CustomVariablesSelection_GroupBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CustomVariablesSelection_GroupBox.Location = New System.Drawing.Point(269, 3)
        Me.CustomVariablesSelection_GroupBox.Name = "CustomVariablesSelection_GroupBox"
        Me.TableLayoutPanel3.SetRowSpan(Me.CustomVariablesSelection_GroupBox, 4)
        Me.CustomVariablesSelection_GroupBox.Size = New System.Drawing.Size(260, 249)
        Me.CustomVariablesSelection_GroupBox.TabIndex = 3
        Me.CustomVariablesSelection_GroupBox.TabStop = False
        Me.CustomVariablesSelection_GroupBox.Text = "Select variables to balance"
        '
        'CustomVariablesSelection_TableLayoutPanel
        '
        Me.CustomVariablesSelection_TableLayoutPanel.AutoScroll = True
        Me.CustomVariablesSelection_TableLayoutPanel.ColumnCount = 1
        Me.CustomVariablesSelection_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.CustomVariablesSelection_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CustomVariablesSelection_TableLayoutPanel.Location = New System.Drawing.Point(3, 16)
        Me.CustomVariablesSelection_TableLayoutPanel.Name = "CustomVariablesSelection_TableLayoutPanel"
        Me.CustomVariablesSelection_TableLayoutPanel.RowCount = 1
        Me.CustomVariablesSelection_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.CustomVariablesSelection_TableLayoutPanel.Size = New System.Drawing.Size(254, 230)
        Me.CustomVariablesSelection_TableLayoutPanel.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(3, 25)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(266, 25)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Name of new media set"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'NewMediaSetName_TextBox
        '
        Me.NewMediaSetName_TextBox.Location = New System.Drawing.Point(275, 28)
        Me.NewMediaSetName_TextBox.Name = "NewMediaSetName_TextBox"
        Me.NewMediaSetName_TextBox.Size = New System.Drawing.Size(266, 20)
        Me.NewMediaSetName_TextBox.TabIndex = 4
        '
        'ReArrangeButton
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.ReArrangeButton, 2)
        Me.ReArrangeButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReArrangeButton.Location = New System.Drawing.Point(3, 486)
        Me.ReArrangeButton.Name = "ReArrangeButton"
        Me.ReArrangeButton.Size = New System.Drawing.Size(538, 29)
        Me.ReArrangeButton.TabIndex = 5
        Me.ReArrangeButton.Text = "Re-arrage"
        Me.ReArrangeButton.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Location = New System.Drawing.Point(3, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(266, 25)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Name of new speech material"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'NewSpeechMaterialName_TextBox
        '
        Me.NewSpeechMaterialName_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NewSpeechMaterialName_TextBox.Location = New System.Drawing.Point(275, 3)
        Me.NewSpeechMaterialName_TextBox.Name = "NewSpeechMaterialName_TextBox"
        Me.NewSpeechMaterialName_TextBox.Size = New System.Drawing.Size(266, 20)
        Me.NewSpeechMaterialName_TextBox.TabIndex = 7
        '
        'ListRearrangerControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "ListRearrangerControl"
        Me.Size = New System.Drawing.Size(544, 518)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ListDescriptives_GroupBox.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.Order_GroupBox.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TableLayoutPanel3.PerformLayout()
        Me.CustomVariablesSelection_GroupBox.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents ListDescriptives_GroupBox As Windows.Forms.GroupBox
    Friend WithEvents Order_GroupBox As Windows.Forms.GroupBox
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents NewMediaSetName_TextBox As Windows.Forms.TextBox
    Friend WithEvents ReArrangeButton As Windows.Forms.Button
    Friend WithEvents RearrangeWithinLists_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents RearrangeAcrossLists_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents TableLayoutPanel2 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents ListNamePrefix_TextBox As Windows.Forms.TextBox
    Friend WithEvents ListLength_IntegerParsingTextBox As IntegerParsingTextBox
    Friend WithEvents BalancedOrder_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents OriginalOrder_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents RandomOrder_RadioButton As Windows.Forms.RadioButton
    Friend WithEvents TableLayoutPanel3 As Windows.Forms.TableLayoutPanel
    Friend WithEvents CustomVariablesSelection_GroupBox As Windows.Forms.GroupBox
    Friend WithEvents BackgroundWorker1 As ComponentModel.BackgroundWorker
    Friend WithEvents CustomVariablesSelection_TableLayoutPanel As Windows.Forms.TableLayoutPanel
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents NewSpeechMaterialName_TextBox As Windows.Forms.TextBox
End Class
