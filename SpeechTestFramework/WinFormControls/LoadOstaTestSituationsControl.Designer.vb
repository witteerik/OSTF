<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LoadOstaTestSituationsControl
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
        Me.DescriptionLabel = New System.Windows.Forms.Label()
        Me.SelectTest_Button = New System.Windows.Forms.Button()
        Me.TestSelection_ComboBox = New System.Windows.Forms.ComboBox()
        Me.SearchForTests_Button = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 98.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.DescriptionLabel, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.SelectTest_Button, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.TestSelection_ComboBox, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.SearchForTests_Button, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(407, 54)
        Me.TableLayoutPanel1.TabIndex = 5
        '
        'DescriptionLabel
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.DescriptionLabel, 3)
        Me.DescriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DescriptionLabel.Location = New System.Drawing.Point(3, 0)
        Me.DescriptionLabel.Name = "DescriptionLabel"
        Me.DescriptionLabel.Size = New System.Drawing.Size(401, 25)
        Me.DescriptionLabel.TabIndex = 3
        Me.DescriptionLabel.Text = "Select and load test situation"
        Me.DescriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SelectTest_Button
        '
        Me.SelectTest_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SelectTest_Button.Location = New System.Drawing.Point(312, 28)
        Me.SelectTest_Button.Name = "SelectTest_Button"
        Me.SelectTest_Button.Size = New System.Drawing.Size(92, 23)
        Me.SelectTest_Button.TabIndex = 2
        Me.SelectTest_Button.Text = "Load situation"
        Me.SelectTest_Button.UseVisualStyleBackColor = True
        '
        'TestSelection_ComboBox
        '
        Me.TestSelection_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestSelection_ComboBox.FormattingEnabled = True
        Me.TestSelection_ComboBox.Location = New System.Drawing.Point(73, 28)
        Me.TestSelection_ComboBox.Name = "TestSelection_ComboBox"
        Me.TestSelection_ComboBox.Size = New System.Drawing.Size(233, 21)
        Me.TestSelection_ComboBox.TabIndex = 0
        '
        'SearchForTests_Button
        '
        Me.SearchForTests_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SearchForTests_Button.Location = New System.Drawing.Point(3, 28)
        Me.SearchForTests_Button.Name = "SearchForTests_Button"
        Me.SearchForTests_Button.Size = New System.Drawing.Size(64, 23)
        Me.SearchForTests_Button.TabIndex = 1
        Me.SearchForTests_Button.Text = "Search"
        Me.SearchForTests_Button.UseVisualStyleBackColor = True
        '
        'LoadOstaTestSituationsControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "LoadOstaTestSituationsControl"
        Me.Size = New System.Drawing.Size(407, 54)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents DescriptionLabel As Windows.Forms.Label
    Friend WithEvents SelectTest_Button As Windows.Forms.Button
    Friend WithEvents TestSelection_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents SearchForTests_Button As Windows.Forms.Button
End Class
