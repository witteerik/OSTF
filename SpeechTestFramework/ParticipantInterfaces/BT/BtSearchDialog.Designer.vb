<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BtSearchDialog
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.SearchButton = New System.Windows.Forms.Button()
        Me.BtDeviceListBox = New System.Windows.Forms.ListBox()
        Me.AvaliableDevicesLabel = New System.Windows.Forms.Label()
        Me.SelectDeviceLabel = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(277, 260)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'SearchButton
        '
        Me.SearchButton.Location = New System.Drawing.Point(13, 13)
        Me.SearchButton.Name = "SearchButton"
        Me.SearchButton.Size = New System.Drawing.Size(410, 46)
        Me.SearchButton.TabIndex = 1
        Me.SearchButton.Text = "Search"
        Me.SearchButton.UseVisualStyleBackColor = True
        '
        'BtDeviceListBox
        '
        Me.BtDeviceListBox.FormattingEnabled = True
        Me.BtDeviceListBox.HorizontalScrollbar = True
        Me.BtDeviceListBox.Location = New System.Drawing.Point(13, 85)
        Me.BtDeviceListBox.Name = "BtDeviceListBox"
        Me.BtDeviceListBox.Size = New System.Drawing.Size(410, 147)
        Me.BtDeviceListBox.TabIndex = 3
        '
        'AvaliableDevicesLabel
        '
        Me.AvaliableDevicesLabel.AutoSize = True
        Me.AvaliableDevicesLabel.Location = New System.Drawing.Point(13, 69)
        Me.AvaliableDevicesLabel.Name = "AvaliableDevicesLabel"
        Me.AvaliableDevicesLabel.Size = New System.Drawing.Size(39, 13)
        Me.AvaliableDevicesLabel.TabIndex = 4
        Me.AvaliableDevicesLabel.Text = "Label1"
        '
        'SelectDeviceLabel
        '
        Me.SelectDeviceLabel.AutoSize = True
        Me.SelectDeviceLabel.Location = New System.Drawing.Point(13, 235)
        Me.SelectDeviceLabel.Name = "SelectDeviceLabel"
        Me.SelectDeviceLabel.Size = New System.Drawing.Size(39, 13)
        Me.SelectDeviceLabel.TabIndex = 5
        Me.SelectDeviceLabel.Text = "Label1"
        '
        'BtSearchDialog
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(435, 301)
        Me.Controls.Add(Me.SelectDeviceLabel)
        Me.Controls.Add(Me.AvaliableDevicesLabel)
        Me.Controls.Add(Me.BtDeviceListBox)
        Me.Controls.Add(Me.SearchButton)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "BtSearchDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "BtSearchDialog"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents SearchButton As Windows.Forms.Button
    Friend WithEvents BtDeviceListBox As Windows.Forms.ListBox
    Friend WithEvents AvaliableDevicesLabel As Windows.Forms.Label
    Friend WithEvents SelectDeviceLabel As Windows.Forms.Label
End Class
