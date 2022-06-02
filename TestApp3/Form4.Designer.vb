<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form4
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.LoadFileControl1 = New SpeechTestFramework.LoadFileControl()
        Me.ResponseGuiItemTable1 = New SpeechTestFramework.ResponseGuiItemTable()
        Me.ResponseGuiItem1 = New SpeechTestFramework.ResponseGuiItem()
        Me.ResponseGuiItem2 = New SpeechTestFramework.ResponseGuiItem()
        Me.ResponseGuiItem3 = New SpeechTestFramework.ResponseGuiItem()
        Me.ResponseGuiItem4 = New SpeechTestFramework.ResponseGuiItem()
        Me.ResponseGuiItem5 = New SpeechTestFramework.ResponseGuiItem()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.CustomVariableSelectionControl1 = New SpeechTestFramework.CustomVariableSelectionControl()
        Me.CustomVariableSelectionControl2 = New SpeechTestFramework.CustomVariableSelectionControl()
        Me.ResponseGuiItemTable1.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(289, 15)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(451, 15)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 3
        Me.Button2.Text = "Button2"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(370, 15)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(75, 23)
        Me.Button3.TabIndex = 4
        Me.Button3.Text = "Button3"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'LoadFileControl1
        '
        Me.LoadFileControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LoadFileControl1.ColumnCount = 2
        Me.LoadFileControl1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadFileControl1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadFileControl1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadFileControl1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadFileControl1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadFileControl1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.LoadFileControl1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.01492!))
        Me.LoadFileControl1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.98507!))
        Me.LoadFileControl1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 134.0!))
        Me.LoadFileControl1.Description = "TExt"
        Me.LoadFileControl1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns
        Me.LoadFileControl1.Location = New System.Drawing.Point(370, 139)
        Me.LoadFileControl1.Name = "LoadFileControl1"
        Me.LoadFileControl1.RowCount = 2
        Me.LoadFileControl1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadFileControl1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LoadFileControl1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.96273!))
        Me.LoadFileControl1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 54.03727!))
        Me.LoadFileControl1.Size = New System.Drawing.Size(405, 54)
        Me.LoadFileControl1.TabIndex = 5
        '
        'ResponseGuiItemTable1
        '
        Me.ResponseGuiItemTable1.ColumnCount = 3
        Me.ResponseGuiItemTable1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82.0!))
        Me.ResponseGuiItemTable1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82.0!))
        Me.ResponseGuiItemTable1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82.0!))
        Me.ResponseGuiItemTable1.Controls.Add(Me.ResponseGuiItem1, 0, 0)
        Me.ResponseGuiItemTable1.Controls.Add(Me.ResponseGuiItem2, 1, 0)
        Me.ResponseGuiItemTable1.Controls.Add(Me.ResponseGuiItem3, 0, 1)
        Me.ResponseGuiItemTable1.Controls.Add(Me.ResponseGuiItem4, 2, 0)
        Me.ResponseGuiItemTable1.Controls.Add(Me.ResponseGuiItem5, 2, 1)
        Me.ResponseGuiItemTable1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize
        Me.ResponseGuiItemTable1.Location = New System.Drawing.Point(22, 12)
        Me.ResponseGuiItemTable1.Name = "ResponseGuiItemTable1"
        Me.ResponseGuiItemTable1.RowCount = 2
        Me.ResponseGuiItemTable1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 98.0!))
        Me.ResponseGuiItemTable1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 98.0!))
        Me.ResponseGuiItemTable1.Size = New System.Drawing.Size(246, 196)
        Me.ResponseGuiItemTable1.StackedControls = False
        Me.ResponseGuiItemTable1.StackOrientation = SpeechTestFramework.ResponseGuiItemTable.VisualStackOrientations.Horizontal
        Me.ResponseGuiItemTable1.TabIndex = 2
        '
        'ResponseGuiItem1
        '
        Me.ResponseGuiItem1.Location = New System.Drawing.Point(3, 3)
        Me.ResponseGuiItem1.Name = "ResponseGuiItem1"
        Me.ResponseGuiItem1.NonSelectedColor = System.Drawing.Color.Empty
        Me.ResponseGuiItem1.ResponseString = Nothing
        Me.ResponseGuiItem1.SelectedColor = System.Drawing.Color.Empty
        Me.ResponseGuiItem1.Size = New System.Drawing.Size(75, 23)
        Me.ResponseGuiItem1.TabIndex = 0
        Me.ResponseGuiItem1.Text = "ResponseGuiItem1"
        '
        'ResponseGuiItem2
        '
        Me.ResponseGuiItem2.Location = New System.Drawing.Point(85, 3)
        Me.ResponseGuiItem2.Name = "ResponseGuiItem2"
        Me.ResponseGuiItem2.NonSelectedColor = System.Drawing.Color.Empty
        Me.ResponseGuiItem2.ResponseString = Nothing
        Me.ResponseGuiItem2.SelectedColor = System.Drawing.Color.Empty
        Me.ResponseGuiItem2.Size = New System.Drawing.Size(75, 23)
        Me.ResponseGuiItem2.TabIndex = 1
        Me.ResponseGuiItem2.Text = "ResponseGuiItem2"
        '
        'ResponseGuiItem3
        '
        Me.ResponseGuiItem3.Location = New System.Drawing.Point(3, 101)
        Me.ResponseGuiItem3.Name = "ResponseGuiItem3"
        Me.ResponseGuiItem3.NonSelectedColor = System.Drawing.Color.Empty
        Me.ResponseGuiItem3.ResponseString = Nothing
        Me.ResponseGuiItem3.SelectedColor = System.Drawing.Color.Empty
        Me.ResponseGuiItem3.Size = New System.Drawing.Size(75, 23)
        Me.ResponseGuiItem3.TabIndex = 2
        Me.ResponseGuiItem3.Text = "ResponseGuiItem3"
        '
        'ResponseGuiItem4
        '
        Me.ResponseGuiItem4.Location = New System.Drawing.Point(167, 3)
        Me.ResponseGuiItem4.Name = "ResponseGuiItem4"
        Me.ResponseGuiItem4.NonSelectedColor = System.Drawing.Color.Empty
        Me.ResponseGuiItem4.ResponseString = Nothing
        Me.ResponseGuiItem4.SelectedColor = System.Drawing.Color.Empty
        Me.ResponseGuiItem4.Size = New System.Drawing.Size(75, 23)
        Me.ResponseGuiItem4.TabIndex = 3
        Me.ResponseGuiItem4.Text = "ResponseGuiItem4"
        '
        'ResponseGuiItem5
        '
        Me.ResponseGuiItem5.Location = New System.Drawing.Point(167, 101)
        Me.ResponseGuiItem5.Name = "ResponseGuiItem5"
        Me.ResponseGuiItem5.NonSelectedColor = System.Drawing.Color.Empty
        Me.ResponseGuiItem5.ResponseString = Nothing
        Me.ResponseGuiItem5.SelectedColor = System.Drawing.Color.Empty
        Me.ResponseGuiItem5.Size = New System.Drawing.Size(75, 23)
        Me.ResponseGuiItem5.TabIndex = 4
        Me.ResponseGuiItem5.Text = "ResponseGuiItem5"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Controls.Add(Me.CustomVariableSelectionControl1)
        Me.FlowLayoutPanel1.Controls.Add(Me.CustomVariableSelectionControl2)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(319, 214)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(693, 307)
        Me.FlowLayoutPanel1.TabIndex = 6
        '
        'CustomVariableSelectionControl1
        '
        Me.CustomVariableSelectionControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.CustomVariableSelectionControl1.DefaultTextColor = System.Drawing.Color.Empty
        Me.CustomVariableSelectionControl1.Location = New System.Drawing.Point(3, 3)
        Me.CustomVariableSelectionControl1.Name = "CustomVariableSelectionControl1"
        Me.CustomVariableSelectionControl1.NewVariableName = ""
        Me.CustomVariableSelectionControl1.OriginalVariableName = ""
        Me.CustomVariableSelectionControl1.Size = New System.Drawing.Size(682, 122)
        Me.CustomVariableSelectionControl1.TabIndex = 0
        '
        'CustomVariableSelectionControl2
        '
        Me.CustomVariableSelectionControl2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.CustomVariableSelectionControl2.DefaultTextColor = System.Drawing.Color.Empty
        Me.CustomVariableSelectionControl2.Location = New System.Drawing.Point(3, 131)
        Me.CustomVariableSelectionControl2.Name = "CustomVariableSelectionControl2"
        Me.CustomVariableSelectionControl2.NewVariableName = ""
        Me.CustomVariableSelectionControl2.OriginalVariableName = ""
        Me.CustomVariableSelectionControl2.Size = New System.Drawing.Size(682, 122)
        Me.CustomVariableSelectionControl2.TabIndex = 1
        '
        'Form4
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(1094, 526)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.LoadFileControl1)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.ResponseGuiItemTable1)
        Me.Controls.Add(Me.Button1)
        Me.Name = "Form4"
        Me.Text = "Form4"
        Me.ResponseGuiItemTable1.ResumeLayout(False)
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Button1 As Button
    Friend WithEvents ResponseGuiItemTable1 As SpeechTestFramework.ResponseGuiItemTable
    Friend WithEvents ResponseGuiItem1 As SpeechTestFramework.ResponseGuiItem
    Friend WithEvents ResponseGuiItem2 As SpeechTestFramework.ResponseGuiItem
    Friend WithEvents ResponseGuiItem3 As SpeechTestFramework.ResponseGuiItem
    Friend WithEvents ResponseGuiItem4 As SpeechTestFramework.ResponseGuiItem
    Friend WithEvents ResponseGuiItem5 As SpeechTestFramework.ResponseGuiItem
    Friend WithEvents Button2 As Button
    Friend WithEvents Button3 As Button
    Friend WithEvents LoadFileControl1 As SpeechTestFramework.LoadFileControl
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents CustomVariableSelectionControl1 As SpeechTestFramework.CustomVariableSelectionControl
    Friend WithEvents CustomVariableSelectionControl2 As SpeechTestFramework.CustomVariableSelectionControl
End Class
