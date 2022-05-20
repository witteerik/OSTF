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



Public Class SpectrogramSettingsDialog

    Private SpectrogramCutFrequency As Single = 8000
    Private AnalysisWindowSize As Integer = 1024
    Private OverlapSize As Integer = 512
    Private FftWindowSize As Integer = 512
    Private WindowingType As Audio.WindowingType = Audio.WindowingType.Hanning

    Private UsePreFftFiltering As Boolean = True
    Private PreFftFilteringAttenuationRate As Single = 6
    Private PreFftFilteringKernelSize As Integer = 2000
    Private PreFftFilteringAnalysisWindowSize As Integer = 1024

    Property NewSpectrogramFormat As Audio.Formats.SpectrogramFormat

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Try
            SpectrogramCutFrequency = CF_Box.Text
            AnalysisWindowSize = AWS_box.Text
            OverlapSize = AWOS_Box.Text
            FftWindowSize = FFTSize_Box.Text

            PreFftFilteringAttenuationRate = PreFFT_AR_Box.Text
            PreFftFilteringKernelSize = PreKernel_Box.Text
            PreFftFilteringAnalysisWindowSize = PreAWS_Box.Text

            Select Case UsePreFftFilteringBox.SelectedItem
                Case "Yes"
                    UsePreFftFiltering = True
                Case Else
                    UsePreFftFiltering = False
            End Select

        Catch ex As Exception
            MsgBox(ex.ToString)
            Exit Sub
        End Try

        Try
            WindowingType = [Enum].Parse(GetType(Audio.WindowingType), WindowingTypeComboBox.SelectedItem)

        Catch ex As Exception
            MsgBox(ex.ToString)
            Exit Sub

        End Try

        Try
            NewSpectrogramFormat = New Audio.Formats.SpectrogramFormat(SpectrogramCutFrequency, AnalysisWindowSize, FftWindowSize,
OverlapSize, WindowingType, UsePreFftFiltering, PreFftFilteringAttenuationRate, PreFftFilteringKernelSize, PreFftFilteringAnalysisWindowSize, True)

        Catch ex As Exception
            Exit Sub

        End Try

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub SpectrogramSettingsDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Load default values
        CF_Box.Text = SpectrogramCutFrequency
        AWS_box.Text = AnalysisWindowSize
        AWOS_Box.Text = OverlapSize
        FFTSize_Box.Text = FftWindowSize
        PreFFT_AR_Box.Text = PreFftFilteringAttenuationRate
        PreKernel_Box.Text = PreFftFilteringKernelSize
        PreAWS_Box.Text = PreFftFilteringAnalysisWindowSize

        'Load values for comboBoxes
        Dim windowingTypes() As Integer = [Enum].GetValues(GetType(Audio.WindowingType))
        For i = 0 To windowingTypes.Length - 1
            WindowingTypeComboBox.Items.Add([Enum].GetName(GetType(Audio.WindowingType), windowingTypes(i)).ToString)
        Next
        WindowingTypeComboBox.SelectedItem = Audio.WindowingType.Hanning.ToString

        UsePreFftFilteringBox.Items.Add("Yes")
        UsePreFftFilteringBox.Items.Add("No")
        UsePreFftFilteringBox.SelectedItem = "Yes"

    End Sub


    Private Sub CheckIntegerValue(sender As Object, e As EventArgs) Handles AWS_box.TextChanged, AWOS_Box.TextChanged, FFTSize_Box.TextChanged, PreKernel_Box.TextChanged, PreAWS_Box.TextChanged

        Dim senderTextBox As TextBox = TryCast(sender, TextBox)

        If IsNumeric(sender.Text) Then
            If Not CDbl(sender.Text) = CInt(sender.Text) Then

                sender.text = CInt(sender.Text)
                MsgBox("Values for values for window sizes, overlap size, and kernel size can only be integer numbers.")

            End If

        Else
            Select Case senderTextBox.Name
                Case "Analysis windows size (samples)"
                    sender.text = 1024
                Case "Analysis window overlap size (samples)"
                    sender.text = 512
                Case "FFT window size / frequency resolution (samples) "
                    sender.text = 512
                Case "Pre FFT filter kernel size (samples)"
                    sender.text = 1024
                Case "Pre FFT filter analysis window size (samples)"
            End Select

            MsgBox("Use only integers as values for window, overlap, and kernel sizes.")

        End If

    End Sub

    Private Sub CheckNumericValue(sender As Object, e As EventArgs) Handles CF_Box.TextChanged, PreFFT_AR_Box.TextChanged

        Dim senderTextBox As TextBox = TryCast(sender, TextBox)

        If Not IsNumeric(sender.Text) Then

            MsgBox("Values for Spectrogram cut frequency and Pre FFT filter attenuation rate must be numeric.")
            Select Case senderTextBox.Name
                Case "Spectrogram cut frequency (Hz)"
                    sender.text = 8000
                Case "Pre FFT filter attenuation rate (dB/octave)"
                    sender.text = 6
            End Select

        End If

    End Sub

    Private Sub UsePreFftFilteringBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles UsePreFftFilteringBox.SelectedIndexChanged

        Select Case UsePreFftFilteringBox.SelectedItem
            Case "Yes"
                PreFFT_AR_Box.Enabled = True
                PreKernel_Box.Enabled = True
                PreAWS_Box.Enabled = True

            Case Else
                PreFFT_AR_Box.Enabled = False
                PreKernel_Box.Enabled = False
                PreAWS_Box.Enabled = False

        End Select

    End Sub

End Class

