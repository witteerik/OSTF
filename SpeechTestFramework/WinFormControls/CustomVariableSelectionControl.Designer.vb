<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CustomVariableSelectionControl
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
        Me.OriginalVariabelName_CheckBox = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.RenameTo_TextBox = New System.Windows.Forms.TextBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.ArithmeticMean_CheckBox = New System.Windows.Forms.CheckBox()
        Me.SD_CheckBox = New System.Windows.Forms.CheckBox()
        Me.Max_CheckBox = New System.Windows.Forms.CheckBox()
        Me.Min_CheckBox = New System.Windows.Forms.CheckBox()
        Me.Median_CheckBox = New System.Windows.Forms.CheckBox()
        Me.IQR_CheckBox = New System.Windows.Forms.CheckBox()
        Me.CV_CheckBox = New System.Windows.Forms.CheckBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 4
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.022556!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.29323!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.1203!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.37594!))
        Me.TableLayoutPanel1.Controls.Add(Me.OriginalVariabelName_CheckBox, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.RenameTo_TextBox, 3, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBox1, 1, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(505, 98)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'OriginalVariabelName_CheckBox
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.OriginalVariabelName_CheckBox, 2)
        Me.OriginalVariabelName_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.OriginalVariabelName_CheckBox.Location = New System.Drawing.Point(3, 3)
        Me.OriginalVariabelName_CheckBox.Name = "OriginalVariabelName_CheckBox"
        Me.OriginalVariabelName_CheckBox.Size = New System.Drawing.Size(252, 19)
        Me.OriginalVariabelName_CheckBox.TabIndex = 0
        Me.OriginalVariabelName_CheckBox.Text = "CheckBox1"
        Me.OriginalVariabelName_CheckBox.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(261, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(110, 25)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "(Optionally) Rename to:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'RenameTo_TextBox
        '
        Me.RenameTo_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RenameTo_TextBox.Location = New System.Drawing.Point(377, 3)
        Me.RenameTo_TextBox.Name = "RenameTo_TextBox"
        Me.RenameTo_TextBox.Size = New System.Drawing.Size(125, 20)
        Me.RenameTo_TextBox.TabIndex = 2
        '
        'GroupBox1
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.GroupBox1, 3)
        Me.GroupBox1.Controls.Add(Me.FlowLayoutPanel1)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(48, 28)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(454, 67)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Calculate at higher levels"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Controls.Add(Me.ArithmeticMean_CheckBox)
        Me.FlowLayoutPanel1.Controls.Add(Me.SD_CheckBox)
        Me.FlowLayoutPanel1.Controls.Add(Me.Max_CheckBox)
        Me.FlowLayoutPanel1.Controls.Add(Me.Min_CheckBox)
        Me.FlowLayoutPanel1.Controls.Add(Me.Median_CheckBox)
        Me.FlowLayoutPanel1.Controls.Add(Me.IQR_CheckBox)
        Me.FlowLayoutPanel1.Controls.Add(Me.CV_CheckBox)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(3, 16)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(448, 48)
        Me.FlowLayoutPanel1.TabIndex = 0
        '
        'ArithmeticMean_CheckBox
        '
        Me.ArithmeticMean_CheckBox.AutoSize = True
        Me.ArithmeticMean_CheckBox.Location = New System.Drawing.Point(3, 3)
        Me.ArithmeticMean_CheckBox.Name = "ArithmeticMean_CheckBox"
        Me.ArithmeticMean_CheckBox.Size = New System.Drawing.Size(101, 17)
        Me.ArithmeticMean_CheckBox.TabIndex = 0
        Me.ArithmeticMean_CheckBox.Text = "Arithmetic mean"
        Me.ArithmeticMean_CheckBox.UseVisualStyleBackColor = True
        '
        'SD_CheckBox
        '
        Me.SD_CheckBox.AutoSize = True
        Me.SD_CheckBox.Location = New System.Drawing.Point(110, 3)
        Me.SD_CheckBox.Name = "SD_CheckBox"
        Me.SD_CheckBox.Size = New System.Drawing.Size(115, 17)
        Me.SD_CheckBox.TabIndex = 3
        Me.SD_CheckBox.Text = "Standard deviation"
        Me.SD_CheckBox.UseVisualStyleBackColor = True
        '
        'Max_CheckBox
        '
        Me.Max_CheckBox.AutoSize = True
        Me.Max_CheckBox.Location = New System.Drawing.Point(231, 3)
        Me.Max_CheckBox.Name = "Max_CheckBox"
        Me.Max_CheckBox.Size = New System.Drawing.Size(70, 17)
        Me.Max_CheckBox.TabIndex = 1
        Me.Max_CheckBox.Text = "Maximum"
        Me.Max_CheckBox.UseVisualStyleBackColor = True
        '
        'Min_CheckBox
        '
        Me.Min_CheckBox.AutoSize = True
        Me.Min_CheckBox.Location = New System.Drawing.Point(307, 3)
        Me.Min_CheckBox.Name = "Min_CheckBox"
        Me.Min_CheckBox.Size = New System.Drawing.Size(67, 17)
        Me.Min_CheckBox.TabIndex = 2
        Me.Min_CheckBox.Text = "Minimum"
        Me.Min_CheckBox.UseVisualStyleBackColor = True
        '
        'Median_CheckBox
        '
        Me.Median_CheckBox.AutoSize = True
        Me.Median_CheckBox.Location = New System.Drawing.Point(380, 3)
        Me.Median_CheckBox.Name = "Median_CheckBox"
        Me.Median_CheckBox.Size = New System.Drawing.Size(61, 17)
        Me.Median_CheckBox.TabIndex = 4
        Me.Median_CheckBox.Text = "Median"
        Me.Median_CheckBox.UseVisualStyleBackColor = True
        '
        'IQR_CheckBox
        '
        Me.IQR_CheckBox.AutoSize = True
        Me.IQR_CheckBox.Location = New System.Drawing.Point(3, 26)
        Me.IQR_CheckBox.Name = "IQR_CheckBox"
        Me.IQR_CheckBox.Size = New System.Drawing.Size(111, 17)
        Me.IQR_CheckBox.TabIndex = 5
        Me.IQR_CheckBox.Text = "Interquartile range"
        Me.IQR_CheckBox.UseVisualStyleBackColor = True
        '
        'CV_CheckBox
        '
        Me.CV_CheckBox.AutoSize = True
        Me.CV_CheckBox.Location = New System.Drawing.Point(120, 26)
        Me.CV_CheckBox.Name = "CV_CheckBox"
        Me.CV_CheckBox.Size = New System.Drawing.Size(131, 17)
        Me.CV_CheckBox.TabIndex = 6
        Me.CV_CheckBox.Text = "Coefficient of variation"
        Me.CV_CheckBox.UseVisualStyleBackColor = True
        '
        'CustomVariableSelectionControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "CustomVariableSelectionControl"
        Me.Size = New System.Drawing.Size(505, 98)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents OriginalVariabelName_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents RenameTo_TextBox As Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents FlowLayoutPanel1 As Windows.Forms.FlowLayoutPanel
    Friend WithEvents ArithmeticMean_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Max_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Min_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents SD_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Median_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents IQR_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents CV_CheckBox As Windows.Forms.CheckBox
End Class
