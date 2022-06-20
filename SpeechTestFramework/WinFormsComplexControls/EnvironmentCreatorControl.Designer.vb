<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EnvironmentCreatorControl
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
        Me.NewTestSituationControl1 = New SpeechTestFramework.EditTestSituationControl()
        Me.LoadedSpeechMaterialName_TextBox = New System.Windows.Forms.TextBox()
        Me.LoadOstaTestSpecificationControl1 = New SpeechTestFramework.LoadOstaTestSpecificationControl()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.NewTestSituationControl1, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.LoadedSpeechMaterialName_TextBox, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.LoadOstaTestSpecificationControl1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 57.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(1153, 672)
        Me.TableLayoutPanel1.TabIndex = 1
        '
        'NewTestSituationControl1
        '
        Me.NewTestSituationControl1.AutoScroll = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.NewTestSituationControl1, 2)
        Me.NewTestSituationControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NewTestSituationControl1.Location = New System.Drawing.Point(3, 85)
        Me.NewTestSituationControl1.Name = "NewTestSituationControl1"
        Me.NewTestSituationControl1.SelectedTestSituation = Nothing
        Me.NewTestSituationControl1.Size = New System.Drawing.Size(1147, 584)
        Me.NewTestSituationControl1.TabIndex = 2
        '
        'LoadedSpeechMaterialName_TextBox
        '
        Me.LoadedSpeechMaterialName_TextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LoadedSpeechMaterialName_TextBox.Location = New System.Drawing.Point(579, 60)
        Me.LoadedSpeechMaterialName_TextBox.Name = "LoadedSpeechMaterialName_TextBox"
        Me.LoadedSpeechMaterialName_TextBox.ReadOnly = True
        Me.LoadedSpeechMaterialName_TextBox.Size = New System.Drawing.Size(571, 20)
        Me.LoadedSpeechMaterialName_TextBox.TabIndex = 3
        Me.LoadedSpeechMaterialName_TextBox.Text = "No speech material loaded"
        '
        'LoadOstaTestSpecificationControl1
        '
        Me.LoadOstaTestSpecificationControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TableLayoutPanel1.SetColumnSpan(Me.LoadOstaTestSpecificationControl1, 2)
        Me.LoadOstaTestSpecificationControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LoadOstaTestSpecificationControl1.Location = New System.Drawing.Point(3, 3)
        Me.LoadOstaTestSpecificationControl1.Name = "LoadOstaTestSpecificationControl1"
        Me.LoadOstaTestSpecificationControl1.SelectedTestSpecification = Nothing
        Me.LoadOstaTestSpecificationControl1.Size = New System.Drawing.Size(1147, 51)
        Me.LoadOstaTestSpecificationControl1.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 57)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(570, 25)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Loaded speech material:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'EnvironmentCreatorControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "EnvironmentCreatorControl"
        Me.Size = New System.Drawing.Size(1153, 672)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LoadOstaTestSpecificationControl1 As LoadOstaTestSpecificationControl
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents LoadedSpeechMaterialName_TextBox As Windows.Forms.TextBox
    Friend WithEvents NewTestSituationControl1 As EditTestSituationControl
End Class
