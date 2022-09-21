'This software is available under the following license:
'MIT/X11 License
'
'Copyright (c) 2017 Erik Witte
'
'Permission is hereby granted, free of charge, to any person obtaining a copy
'of this software and associated documentation files (the ''Software''), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'copies of the Software, and to permit persons to whom the Software is
'furnished to do so, subject to the following conditions:
'
'The above copyright notice and this permission notice shall be included in all
'copies or substantial portions of the Software.
'
'THE SOFTWARE IS PROVIDED ''AS IS'', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
'SOFTWARE.

Imports System.Windows.Forms

Public Class LicenseBox

    Public ReadOnly SelectedLicense As AvailableLicenses
    Public ReadOnly LicenseAdditions As New List(Of AvailableLicenseAdditions)

    Public Enum AvailableLicenses
        MIT_X11
    End Enum

    Public Enum AvailableLicenseAdditions
        PortAudio
        MathNet
        InTheHand
        Wierstorf
        SwedishSipRecordings
    End Enum

    Public Sub New(ByVal License As AvailableLicenses, ByVal Additions As List(Of AvailableLicenseAdditions))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        'Setting the currently displayed license text
        Me.SelectedLicense = License
        Me.LicenseAdditions = Additions

    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub LicenseBox_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Select Case Me.SelectedLicense
            Case AvailableLicenses.MIT_X11

                LicenseRichTextBox.Text =
"Unless otherwise specified, this application and its source code (available at https://github.com/witteerik/OSTF) is licensed under the following terms:

MIT/X11 License

Copyright (c) " & DateTime.Now.Year & " Erik Witte

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE."

            Case Else
                Throw New NotImplementedException("Unavailable license text")
        End Select

        If LicenseAdditions.Count > 0 Then
            LicenseRichTextBox.Text &= "

This application uses the following software libraries and sound file collections of which some are licensed separately under a different license (as indicated below):

"
        End If

        If LicenseAdditions.Contains(AvailableLicenseAdditions.PortAudio) Then
            LicenseRichTextBox.Text &= "

 - PortAudio
This application uses PortAudio Portable Real-Time Audio Library, v. 19, 
Copyright (c) 1999-2011, Ross Bencina and Phil Burk.
"
        End If

        If LicenseAdditions.Contains(AvailableLicenseAdditions.MathNet) Then
            LicenseRichTextBox.Text &= "
- MathNet
This application uses MathNet.Numerics, v 5.0.0,
Copyright (c), Christoph Ruegg, Marcus Cuda, Jurgen Van Gael.
"
        End If

        If LicenseAdditions.Contains(AvailableLicenseAdditions.InTheHand) Then
            LicenseRichTextBox.Text &= "
- InTheHand -
This application uses InTheHand.Net.Bluetooth, 4.0.4,
Copyright (c), Peter Foot.
"
        End If

        If LicenseAdditions.Contains(AvailableLicenseAdditions.Wierstorf) Then
            LicenseRichTextBox.Text &= "
- Wierstorf et al. (2011)
The set of room impulse responses shipped with this software was developed by Wierstorf et al. (2011) and is separately licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 3.0 license. This means NOT FOR COMMERCIAL USE! If the source code or software itself is to be used commersially, these recordings must first be removed from the distributed files!
http://creativecommons.org/licenses/by-nc-sa/3.0/
Copyright (c) 2011 
Quality & Usability Lab 
Assessment of IP-based Applications
Deutsche Telekom Laboratories, TU Berlin
Ernst-Reuter-Platz 7, 10587 Berlin, Germany
"
        End If

        If LicenseAdditions.Contains(AvailableLicenseAdditions.SwedishSipRecordings) Then
            LicenseRichTextBox.Text &= "
- SiP-test recordings
This application uses a set of recordings for the Swedish SiP-test (available at https://osf.io/q4rb3/), These recordings are separately licensed under the Creative Commons Attribution-NonCommercial 4.0 International License. This means NOT FOR COMMERCIAL USE! If the source code or software itself is to be used commersially, these recordings must first be removed from the distributed files!
http://creativecommons.org/licenses/by-nc/4.0/ .
Copyright (c) Erik Witte
"
        End If



    End Sub

End Class
