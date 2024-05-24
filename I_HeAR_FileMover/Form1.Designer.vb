<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Me.ID_TextBox = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.MoveFiles_Button = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ID_Error_Label = New System.Windows.Forms.Label()
        Me.Results_TextBox = New System.Windows.Forms.TextBox()
        Me.Info_Button = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'ID_TextBox
        '
        Me.ID_TextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ID_TextBox.Location = New System.Drawing.Point(224, 64)
        Me.ID_TextBox.Name = "ID_TextBox"
        Me.ID_TextBox.Size = New System.Drawing.Size(100, 29)
        Me.ID_TextBox.TabIndex = 0
        Me.ID_TextBox.WordWrap = False
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(8, 23)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(546, 23)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Fyll i forskningspersonens pseudonymiseringskod och klicka på 'Flytta filer'!"
        '
        'MoveFiles_Button
        '
        Me.MoveFiles_Button.Enabled = False
        Me.MoveFiles_Button.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MoveFiles_Button.Location = New System.Drawing.Point(200, 118)
        Me.MoveFiles_Button.Name = "MoveFiles_Button"
        Me.MoveFiles_Button.Size = New System.Drawing.Size(146, 29)
        Me.MoveFiles_Button.TabIndex = 2
        Me.MoveFiles_Button.Text = "Flytta filer"
        Me.MoveFiles_Button.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(175, 70)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(43, 23)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Kod:"
        '
        'ID_Error_Label
        '
        Me.ID_Error_Label.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ID_Error_Label.Location = New System.Drawing.Point(330, 70)
        Me.ID_Error_Label.Name = "ID_Error_Label"
        Me.ID_Error_Label.Size = New System.Drawing.Size(209, 23)
        Me.ID_Error_Label.TabIndex = 5
        '
        'Results_TextBox
        '
        Me.Results_TextBox.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Results_TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Results_TextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Results_TextBox.Location = New System.Drawing.Point(12, 187)
        Me.Results_TextBox.Multiline = True
        Me.Results_TextBox.Name = "Results_TextBox"
        Me.Results_TextBox.Size = New System.Drawing.Size(527, 151)
        Me.Results_TextBox.TabIndex = 6
        Me.Results_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Info_Button
        '
        Me.Info_Button.Location = New System.Drawing.Point(476, 344)
        Me.Info_Button.Name = "Info_Button"
        Me.Info_Button.Size = New System.Drawing.Size(75, 23)
        Me.Info_Button.TabIndex = 7
        Me.Info_Button.Text = "Info"
        Me.Info_Button.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.ClientSize = New System.Drawing.Size(551, 368)
        Me.Controls.Add(Me.Info_Button)
        Me.Controls.Add(Me.Results_TextBox)
        Me.Controls.Add(Me.ID_Error_Label)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.MoveFiles_Button)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ID_TextBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.HelpButton = True
        Me.Name = "Form1"
        Me.Text = "I HeAR - Filflyttaren"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ID_TextBox As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents MoveFiles_Button As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents ID_Error_Label As Label
    Friend WithEvents Results_TextBox As TextBox
    Friend WithEvents Info_Button As Button
End Class
