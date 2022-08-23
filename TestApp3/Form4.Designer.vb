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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form4))
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.Audiogram1 = New SpeechTestFramework.WinFormControls.Audiogram()
        Me.CustomVariableSelectionControl1 = New SpeechTestFramework.CustomVariableSelectionControl()
        Me.CustomVariableSelectionControl2 = New SpeechTestFramework.CustomVariableSelectionControl()
        Me.ResponseGuiItemTable1 = New SpeechTestFramework.ResponseGuiItemTable()
        Me.ResponseGuiItem1 = New SpeechTestFramework.ResponseGuiItem()
        Me.ResponseGuiItem2 = New SpeechTestFramework.ResponseGuiItem()
        Me.ResponseGuiItem3 = New SpeechTestFramework.ResponseGuiItem()
        Me.ResponseGuiItem4 = New SpeechTestFramework.ResponseGuiItem()
        Me.ResponseGuiItem5 = New SpeechTestFramework.ResponseGuiItem()
        Me.SiP_Button = New System.Windows.Forms.Button()
        Me.FlowLayoutPanel1.SuspendLayout()
        CType(Me.Audiogram1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ResponseGuiItemTable1.SuspendLayout()
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
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Controls.Add(Me.CustomVariableSelectionControl1)
        Me.FlowLayoutPanel1.Controls.Add(Me.CustomVariableSelectionControl2)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(22, 214)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(342, 307)
        Me.FlowLayoutPanel1.TabIndex = 6
        '
        'Audiogram1
        '
        Me.Audiogram1.AudiogramData = Nothing
        Me.Audiogram1.BackColor = System.Drawing.SystemColors.Window
        Me.Audiogram1.DashedGridLineColor = System.Drawing.Color.Gray
        Me.Audiogram1.GridLineColor = System.Drawing.Color.Gray
        Me.Audiogram1.HideAudiogramLines = False
        Me.Audiogram1.Location = New System.Drawing.Point(534, 55)
        Me.Audiogram1.Name = "Audiogram1"
        Me.Audiogram1.PlotAreaBorder = True
        Me.Audiogram1.PlotAreaBorderColor = System.Drawing.Color.DarkGray
        Me.Audiogram1.PlotAreaRelativeMarginBottom = 0.05!
        Me.Audiogram1.PlotAreaRelativeMarginLeft = 0.1!
        Me.Audiogram1.PlotAreaRelativeMarginRight = 0.1!
        Me.Audiogram1.PlotAreaRelativeMarginTop = 0.1!
        Me.Audiogram1.Size = New System.Drawing.Size(489, 446)
        Me.Audiogram1.TabIndex = 7
        Me.Audiogram1.TabStop = False
        Me.Audiogram1.XaxisDashedGridLinePositions = CType(resources.GetObject("Audiogram1.XaxisDashedGridLinePositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram1.XaxisDrawBottom = False
        Me.Audiogram1.XaxisDrawTop = True
        Me.Audiogram1.XaxisGridLinePositions = CType(resources.GetObject("Audiogram1.XaxisGridLinePositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram1.XaxisTextPositions = CType(resources.GetObject("Audiogram1.XaxisTextPositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram1.XaxisTextSize = 8.0!
        Me.Audiogram1.XaxisTextValues = New String() {"125", "250", "500", "1k", "2k", "4k", "8k"}
        Me.Audiogram1.XaxisTickHeight = 2.0!
        Me.Audiogram1.XaxisTickPositions = CType(resources.GetObject("Audiogram1.XaxisTickPositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram1.XlimMax = 8000.0!
        Me.Audiogram1.XlimMin = 125.0!
        Me.Audiogram1.Xlog = True
        Me.Audiogram1.XlogBase = 2.0!
        Me.Audiogram1.YaxisDashedGridLinePositions = CType(resources.GetObject("Audiogram1.YaxisDashedGridLinePositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram1.YaxisDrawLeft = True
        Me.Audiogram1.YaxisDrawRight = True
        Me.Audiogram1.YaxisGridLinePositions = CType(resources.GetObject("Audiogram1.YaxisGridLinePositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram1.YaxisTextPositions = CType(resources.GetObject("Audiogram1.YaxisTextPositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram1.YaxisTextSize = 8.0!
        Me.Audiogram1.YaxisTextValues = New String() {"0", "20", "40", "60", "80", "100"}
        Me.Audiogram1.YaxisTickPositions = CType(resources.GetObject("Audiogram1.YaxisTickPositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram1.YaxisTickWidth = 2.0!
        Me.Audiogram1.YlimMax = 110.0!
        Me.Audiogram1.YlimMin = -10.0!
        Me.Audiogram1.Ylog = False
        Me.Audiogram1.YlogBase = 10.0!
        Me.Audiogram1.Yreversed = True
        '
        'CustomVariableSelectionControl1
        '
        Me.CustomVariableSelectionControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.CustomVariableSelectionControl1.IsNumericVariable = True
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
        Me.CustomVariableSelectionControl2.IsNumericVariable = True
        Me.CustomVariableSelectionControl2.Location = New System.Drawing.Point(3, 131)
        Me.CustomVariableSelectionControl2.Name = "CustomVariableSelectionControl2"
        Me.CustomVariableSelectionControl2.NewVariableName = ""
        Me.CustomVariableSelectionControl2.OriginalVariableName = ""
        Me.CustomVariableSelectionControl2.Size = New System.Drawing.Size(682, 122)
        Me.CustomVariableSelectionControl2.TabIndex = 1
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
        'SiP_Button
        '
        Me.SiP_Button.Location = New System.Drawing.Point(289, 70)
        Me.SiP_Button.Name = "SiP_Button"
        Me.SiP_Button.Size = New System.Drawing.Size(75, 23)
        Me.SiP_Button.TabIndex = 8
        Me.SiP_Button.Text = "SiP-test"
        Me.SiP_Button.UseVisualStyleBackColor = True
        '
        'Form4
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(1094, 526)
        Me.Controls.Add(Me.SiP_Button)
        Me.Controls.Add(Me.Audiogram1)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.ResponseGuiItemTable1)
        Me.Controls.Add(Me.Button1)
        Me.Name = "Form4"
        Me.Text = "Form4"
        Me.FlowLayoutPanel1.ResumeLayout(False)
        CType(Me.Audiogram1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResponseGuiItemTable1.ResumeLayout(False)
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
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents CustomVariableSelectionControl1 As SpeechTestFramework.CustomVariableSelectionControl
    Friend WithEvents CustomVariableSelectionControl2 As SpeechTestFramework.CustomVariableSelectionControl
    Friend WithEvents Audiogram1 As SpeechTestFramework.WinFormControls.Audiogram
    Friend WithEvents SiP_Button As Button
End Class
