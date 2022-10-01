Imports System.Windows.Forms

Public Class AudiogramDialog

    Private OriginalAudiogramData As AudiogramData

    Public Sub New(ByRef AudiogramData As AudiogramData)

        ' This call is required by the designer.
        InitializeComponent()

        'Stores a reference to AudiogramData, used when retreiving the modifications using the function GetAudiogramData
        OriginalAudiogramData = AudiogramData

        ' Add any initialization after the InitializeComponent() call.
        AudiogramWithEditControls.Audiogram.AudiogramData = AudiogramData.CreateCopy

    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    ''' <summary>
    ''' Copies all changed made in the editor to the originally referenced AudiogramData (and also returns a reference to it, even if this is not needed if such a reference is kept in the calling code).
    ''' </summary>
    ''' <returns></returns>
    Public Function GetAudiogramData() As AudiogramData

        'Copies the data from the deep copy used in the AudiogramWithEditControls to the originally referenced audiogram in order not to break the reference to the originally referenced AudiogramData object 
        AudiogramWithEditControls.Audiogram.AudiogramData.CopyData(OriginalAudiogramData)

        Return OriginalAudiogramData

    End Function

End Class
