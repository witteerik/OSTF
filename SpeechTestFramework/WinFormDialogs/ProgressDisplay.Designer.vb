
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ProgressDisplay
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.ProgressLabel = New System.Windows.Forms.Label()
        Me.SpeedLabel = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(12, 12)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(392, 23)
        Me.ProgressBar1.TabIndex = 0
        '
        'ProgressLabel
        '
        Me.ProgressLabel.AutoSize = True
        Me.ProgressLabel.Location = New System.Drawing.Point(12, 38)
        Me.ProgressLabel.Name = "ProgressLabel"
        Me.ProgressLabel.Size = New System.Drawing.Size(39, 13)
        Me.ProgressLabel.TabIndex = 2
        Me.ProgressLabel.Text = "Label2"
        '
        'SpeedLabel
        '
        Me.SpeedLabel.AutoSize = True
        Me.SpeedLabel.Location = New System.Drawing.Point(12, 65)
        Me.SpeedLabel.Name = "SpeedLabel"
        Me.SpeedLabel.Size = New System.Drawing.Size(39, 13)
        Me.SpeedLabel.TabIndex = 3
        Me.SpeedLabel.Text = "Label2"
        '
        'ProgressDisplay
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(416, 90)
        Me.Controls.Add(Me.SpeedLabel)
        Me.Controls.Add(Me.ProgressLabel)
        Me.Controls.Add(Me.ProgressBar1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "ProgressDisplay"
        Me.Text = "ProgressDisplay"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ProgressLabel As System.Windows.Forms.Label
    Public WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents SpeedLabel As System.Windows.Forms.Label
End Class
