<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AudiogramWithEditControls
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AudiogramWithEditControls))
        Me.Content_TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.AudiogramEditControl = New SpeechTestFramework.AudiogramEditControl()
        Me.Audiogram = New SpeechTestFramework.WinFormControls.Audiogram()
        Me.Content_TableLayoutPanel.SuspendLayout()
        CType(Me.Audiogram, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Content_TableLayoutPanel
        '
        Me.Content_TableLayoutPanel.ColumnCount = 1
        Me.Content_TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.Content_TableLayoutPanel.Controls.Add(Me.AudiogramEditControl, 0, 1)
        Me.Content_TableLayoutPanel.Controls.Add(Me.Audiogram, 0, 0)
        Me.Content_TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Content_TableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.Content_TableLayoutPanel.Name = "Content_TableLayoutPanel"
        Me.Content_TableLayoutPanel.RowCount = 2
        Me.Content_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.Content_TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 155.0!))
        Me.Content_TableLayoutPanel.Size = New System.Drawing.Size(489, 577)
        Me.Content_TableLayoutPanel.TabIndex = 1
        '
        'AudiogramEditControl
        '
        Me.AudiogramEditControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AudiogramEditControl.Location = New System.Drawing.Point(3, 425)
        Me.AudiogramEditControl.Name = "AudiogramEditControl"
        Me.AudiogramEditControl.ShowEditEnabledOptions = False
        Me.AudiogramEditControl.Size = New System.Drawing.Size(483, 149)
        Me.AudiogramEditControl.TabIndex = 0
        '
        'Audiogram
        '
        Me.Audiogram.AudiogramData = Nothing
        Me.Audiogram.BackColor = System.Drawing.SystemColors.Window
        Me.Audiogram.DashedGridLineColor = System.Drawing.Color.Gray
        Me.Audiogram.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Audiogram.GridLineColor = System.Drawing.Color.Gray
        Me.Audiogram.HideAudiogramLines = False
        Me.Audiogram.Location = New System.Drawing.Point(3, 3)
        Me.Audiogram.Name = "Audiogram"
        Me.Audiogram.PlotAreaBorder = True
        Me.Audiogram.PlotAreaBorderColor = System.Drawing.Color.DarkGray
        Me.Audiogram.PlotAreaRelativeMarginBottom = 0.05!
        Me.Audiogram.PlotAreaRelativeMarginLeft = 0.1!
        Me.Audiogram.PlotAreaRelativeMarginRight = 0.1!
        Me.Audiogram.PlotAreaRelativeMarginTop = 0.1!
        Me.Audiogram.Size = New System.Drawing.Size(483, 416)
        Me.Audiogram.TabIndex = 0
        Me.Audiogram.TabStop = False
        Me.Audiogram.XaxisDashedGridLinePositions = CType(resources.GetObject("Audiogram.XaxisDashedGridLinePositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram.XaxisDrawBottom = False
        Me.Audiogram.XaxisDrawTop = True
        Me.Audiogram.XaxisGridLinePositions = CType(resources.GetObject("Audiogram.XaxisGridLinePositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram.XaxisTextPositions = CType(resources.GetObject("Audiogram.XaxisTextPositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram.XaxisTextSize = 8.0!
        Me.Audiogram.XaxisTextValues = New String() {"125", "250", "500", "1k", "2k", "4k", "8k"}
        Me.Audiogram.XaxisTickHeight = 2.0!
        Me.Audiogram.XaxisTickPositions = CType(resources.GetObject("Audiogram.XaxisTickPositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram.XlimMax = 8000.0!
        Me.Audiogram.XlimMin = 125.0!
        Me.Audiogram.Xlog = True
        Me.Audiogram.XlogBase = 2.0!
        Me.Audiogram.YaxisDashedGridLinePositions = CType(resources.GetObject("Audiogram.YaxisDashedGridLinePositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram.YaxisDrawLeft = True
        Me.Audiogram.YaxisDrawRight = True
        Me.Audiogram.YaxisGridLinePositions = CType(resources.GetObject("Audiogram.YaxisGridLinePositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram.YaxisTextPositions = CType(resources.GetObject("Audiogram.YaxisTextPositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram.YaxisTextSize = 8.0!
        Me.Audiogram.YaxisTextValues = New String() {"0", "20", "40", "60", "80", "100"}
        Me.Audiogram.YaxisTickPositions = CType(resources.GetObject("Audiogram.YaxisTickPositions"), System.Collections.Generic.List(Of Single))
        Me.Audiogram.YaxisTickWidth = 2.0!
        Me.Audiogram.YlimMax = 110.0!
        Me.Audiogram.YlimMin = -10.0!
        Me.Audiogram.Ylog = False
        Me.Audiogram.YlogBase = 10.0!
        Me.Audiogram.Yreversed = True
        '
        'AudiogramWithEditControls
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Content_TableLayoutPanel)
        Me.Name = "AudiogramWithEditControls"
        Me.Size = New System.Drawing.Size(489, 577)
        Me.Content_TableLayoutPanel.ResumeLayout(False)
        CType(Me.Audiogram, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Audiogram As WinFormControls.Audiogram
    Friend WithEvents AudiogramEditControl As AudiogramEditControl
    Friend WithEvents Content_TableLayoutPanel As Windows.Forms.TableLayoutPanel
End Class
