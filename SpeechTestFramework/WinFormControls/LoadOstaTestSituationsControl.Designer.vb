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
        Me.SelectTestSituation_Button = New System.Windows.Forms.Button()
        Me.TestSituationSelection_ComboBox = New System.Windows.Forms.ComboBox()
        Me.SearchForTestSituations_Button = New System.Windows.Forms.Button()
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
        Me.TableLayoutPanel1.Controls.Add(Me.SelectTestSituation_Button, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.TestSituationSelection_ComboBox, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.SearchForTestSituations_Button, 0, 1)
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
        'SelectTestSituation_Button
        '
        Me.SelectTestSituation_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SelectTestSituation_Button.Location = New System.Drawing.Point(312, 28)
        Me.SelectTestSituation_Button.Name = "SelectTestSituation_Button"
        Me.SelectTestSituation_Button.Size = New System.Drawing.Size(92, 23)
        Me.SelectTestSituation_Button.TabIndex = 2
        Me.SelectTestSituation_Button.Text = "Load situation"
        Me.SelectTestSituation_Button.UseVisualStyleBackColor = True
        '
        'TestSituationSelection_ComboBox
        '
        Me.TestSituationSelection_ComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TestSituationSelection_ComboBox.FormattingEnabled = True
        Me.TestSituationSelection_ComboBox.Location = New System.Drawing.Point(73, 28)
        Me.TestSituationSelection_ComboBox.Name = "TestSituationSelection_ComboBox"
        Me.TestSituationSelection_ComboBox.Size = New System.Drawing.Size(233, 21)
        Me.TestSituationSelection_ComboBox.TabIndex = 0
        '
        'SearchForTestSituations_Button
        '
        Me.SearchForTestSituations_Button.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SearchForTestSituations_Button.Location = New System.Drawing.Point(3, 28)
        Me.SearchForTestSituations_Button.Name = "SearchForTestSituations_Button"
        Me.SearchForTestSituations_Button.Size = New System.Drawing.Size(64, 23)
        Me.SearchForTestSituations_Button.TabIndex = 1
        Me.SearchForTestSituations_Button.Text = "Search"
        Me.SearchForTestSituations_Button.UseVisualStyleBackColor = True
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
    Friend WithEvents SelectTestSituation_Button As Windows.Forms.Button
    Friend WithEvents TestSituationSelection_ComboBox As Windows.Forms.ComboBox
    Friend WithEvents SearchForTestSituations_Button As Windows.Forms.Button
End Class
