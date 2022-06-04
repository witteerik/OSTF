<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CustomVariableSelectControl
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
        Me.VariabelName_CheckBox = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.NumericSummaryMethodsBox = New System.Windows.Forms.FlowLayoutPanel()
        Me.ArithmeticMean_CheckBox = New System.Windows.Forms.CheckBox()
        Me.SD_CheckBox = New System.Windows.Forms.CheckBox()
        Me.Max_CheckBox = New System.Windows.Forms.CheckBox()
        Me.Min_CheckBox = New System.Windows.Forms.CheckBox()
        Me.Median_CheckBox = New System.Windows.Forms.CheckBox()
        Me.IQR_CheckBox = New System.Windows.Forms.CheckBox()
        Me.CV_CheckBox = New System.Windows.Forms.CheckBox()
        Me.CategorialSummaryMethodsBox = New System.Windows.Forms.FlowLayoutPanel()
        Me.Mode_CheckBox = New System.Windows.Forms.CheckBox()
        Me.Distribution_CheckBox = New System.Windows.Forms.CheckBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.NumericSummaryMethodsBox.SuspendLayout()
        Me.CategorialSummaryMethodsBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.292135!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 93.70786!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.VariabelName_CheckBox, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBox1, 1, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(818, 102)
        Me.TableLayoutPanel1.TabIndex = 1
        '
        'VariabelName_CheckBox
        '
        Me.VariabelName_CheckBox.BackColor = System.Drawing.Color.Transparent
        Me.TableLayoutPanel1.SetColumnSpan(Me.VariabelName_CheckBox, 2)
        Me.VariabelName_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.VariabelName_CheckBox.Location = New System.Drawing.Point(3, 3)
        Me.VariabelName_CheckBox.Name = "VariabelName_CheckBox"
        Me.VariabelName_CheckBox.Size = New System.Drawing.Size(812, 19)
        Me.VariabelName_CheckBox.TabIndex = 0
        Me.VariabelName_CheckBox.Text = "CheckBox1"
        Me.VariabelName_CheckBox.UseVisualStyleBackColor = False
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox1.Controls.Add(Me.FlowLayoutPanel1)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(54, 28)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(761, 71)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Select summary metrics"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.AutoScroll = True
        Me.FlowLayoutPanel1.Controls.Add(Me.NumericSummaryMethodsBox)
        Me.FlowLayoutPanel1.Controls.Add(Me.CategorialSummaryMethodsBox)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(3, 16)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(755, 52)
        Me.FlowLayoutPanel1.TabIndex = 0
        '
        'NumericSummaryMethodsBox
        '
        Me.NumericSummaryMethodsBox.AutoSize = True
        Me.NumericSummaryMethodsBox.Controls.Add(Me.ArithmeticMean_CheckBox)
        Me.NumericSummaryMethodsBox.Controls.Add(Me.SD_CheckBox)
        Me.NumericSummaryMethodsBox.Controls.Add(Me.Max_CheckBox)
        Me.NumericSummaryMethodsBox.Controls.Add(Me.Min_CheckBox)
        Me.NumericSummaryMethodsBox.Controls.Add(Me.Median_CheckBox)
        Me.NumericSummaryMethodsBox.Controls.Add(Me.IQR_CheckBox)
        Me.NumericSummaryMethodsBox.Controls.Add(Me.CV_CheckBox)
        Me.NumericSummaryMethodsBox.Location = New System.Drawing.Point(3, 3)
        Me.NumericSummaryMethodsBox.Name = "NumericSummaryMethodsBox"
        Me.NumericSummaryMethodsBox.Size = New System.Drawing.Size(698, 23)
        Me.NumericSummaryMethodsBox.TabIndex = 7
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
        Me.IQR_CheckBox.Location = New System.Drawing.Point(447, 3)
        Me.IQR_CheckBox.Name = "IQR_CheckBox"
        Me.IQR_CheckBox.Size = New System.Drawing.Size(111, 17)
        Me.IQR_CheckBox.TabIndex = 5
        Me.IQR_CheckBox.Text = "Interquartile range"
        Me.IQR_CheckBox.UseVisualStyleBackColor = True
        '
        'CV_CheckBox
        '
        Me.CV_CheckBox.AutoSize = True
        Me.CV_CheckBox.Location = New System.Drawing.Point(564, 3)
        Me.CV_CheckBox.Name = "CV_CheckBox"
        Me.CV_CheckBox.Size = New System.Drawing.Size(131, 17)
        Me.CV_CheckBox.TabIndex = 6
        Me.CV_CheckBox.Text = "Coefficient of variation"
        Me.CV_CheckBox.UseVisualStyleBackColor = True
        '
        'CategorialSummaryMethodsBox
        '
        Me.CategorialSummaryMethodsBox.AutoSize = True
        Me.CategorialSummaryMethodsBox.Controls.Add(Me.Mode_CheckBox)
        Me.CategorialSummaryMethodsBox.Controls.Add(Me.Distribution_CheckBox)
        Me.CategorialSummaryMethodsBox.Location = New System.Drawing.Point(3, 32)
        Me.CategorialSummaryMethodsBox.Name = "CategorialSummaryMethodsBox"
        Me.CategorialSummaryMethodsBox.Size = New System.Drawing.Size(143, 23)
        Me.CategorialSummaryMethodsBox.TabIndex = 8
        '
        'Mode_CheckBox
        '
        Me.Mode_CheckBox.AutoSize = True
        Me.Mode_CheckBox.Location = New System.Drawing.Point(3, 3)
        Me.Mode_CheckBox.Name = "Mode_CheckBox"
        Me.Mode_CheckBox.Size = New System.Drawing.Size(53, 17)
        Me.Mode_CheckBox.TabIndex = 0
        Me.Mode_CheckBox.Text = "Mode"
        Me.Mode_CheckBox.UseVisualStyleBackColor = True
        '
        'Distribution_CheckBox
        '
        Me.Distribution_CheckBox.AutoSize = True
        Me.Distribution_CheckBox.Location = New System.Drawing.Point(62, 3)
        Me.Distribution_CheckBox.Name = "Distribution_CheckBox"
        Me.Distribution_CheckBox.Size = New System.Drawing.Size(78, 17)
        Me.Distribution_CheckBox.TabIndex = 9
        Me.Distribution_CheckBox.Text = "Distribution"
        Me.Distribution_CheckBox.UseVisualStyleBackColor = True
        '
        'CustomVariableSelectControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "CustomVariableSelectControl"
        Me.Size = New System.Drawing.Size(818, 102)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.NumericSummaryMethodsBox.ResumeLayout(False)
        Me.NumericSummaryMethodsBox.PerformLayout()
        Me.CategorialSummaryMethodsBox.ResumeLayout(False)
        Me.CategorialSummaryMethodsBox.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents VariabelName_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents FlowLayoutPanel1 As Windows.Forms.FlowLayoutPanel
    Friend WithEvents NumericSummaryMethodsBox As Windows.Forms.FlowLayoutPanel
    Friend WithEvents ArithmeticMean_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents SD_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Max_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Min_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Median_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents IQR_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents CV_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents CategorialSummaryMethodsBox As Windows.Forms.FlowLayoutPanel
    Friend WithEvents Mode_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Distribution_CheckBox As Windows.Forms.CheckBox
End Class
