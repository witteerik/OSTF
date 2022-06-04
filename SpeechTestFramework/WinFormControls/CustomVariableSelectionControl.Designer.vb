<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class CustomVariableSelectionControl
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
        Me.OriginalVariabelName_CheckBox = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.RenameTo_TextBox = New System.Windows.Forms.TextBox()
        Me.TypeLabel = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 4
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.39483!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.59748!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 43.01887!))
        Me.TableLayoutPanel1.Controls.Add(Me.OriginalVariabelName_CheckBox, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.RenameTo_TextBox, 3, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TypeLabel, 1, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(890, 27)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'OriginalVariabelName_CheckBox
        '
        Me.OriginalVariabelName_CheckBox.BackColor = System.Drawing.Color.Transparent
        Me.OriginalVariabelName_CheckBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.OriginalVariabelName_CheckBox.Location = New System.Drawing.Point(3, 3)
        Me.OriginalVariabelName_CheckBox.Name = "OriginalVariabelName_CheckBox"
        Me.OriginalVariabelName_CheckBox.Size = New System.Drawing.Size(323, 21)
        Me.OriginalVariabelName_CheckBox.TabIndex = 0
        Me.OriginalVariabelName_CheckBox.Text = "CheckBox1"
        Me.OriginalVariabelName_CheckBox.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(427, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(117, 27)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "(Optionally) Rename to:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'RenameTo_TextBox
        '
        Me.RenameTo_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RenameTo_TextBox.Location = New System.Drawing.Point(550, 3)
        Me.RenameTo_TextBox.Name = "RenameTo_TextBox"
        Me.RenameTo_TextBox.Size = New System.Drawing.Size(337, 20)
        Me.RenameTo_TextBox.TabIndex = 2
        '
        'TypeLabel
        '
        Me.TypeLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TypeLabel.Location = New System.Drawing.Point(332, 0)
        Me.TypeLabel.Name = "TypeLabel"
        Me.TypeLabel.Size = New System.Drawing.Size(89, 27)
        Me.TypeLabel.TabIndex = 3
        Me.TypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CustomVariableSelectionControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "CustomVariableSelectionControl"
        Me.Size = New System.Drawing.Size(890, 27)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents OriginalVariabelName_CheckBox As Windows.Forms.CheckBox
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents RenameTo_TextBox As Windows.Forms.TextBox
    Friend WithEvents TypeLabel As Windows.Forms.Label
End Class
