Public Class AudiogramWithEditControls

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Audiogram.AudiogramData = New AudiogramData()

        Audiogram.EnableEditing = True
        UpdatePointType()

    End Sub

    Public Sub New(ByRef AudiogramData As AudiogramData)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        If AudiogramData IsNot Nothing Then
            Audiogram.AudiogramData = AudiogramData
        Else
            Audiogram.AudiogramData = New AudiogramData()
        End If

        Audiogram.EnableEditing = True
        UpdatePointType()

    End Sub

    Private Sub UpdatePointType() Handles AudiogramEditControl.SettingsUpdated

        Audiogram.CurrentOverheard = AudiogramEditControl.Overheard
        Audiogram.CurrentWriteNotHeard = AudiogramEditControl.NotHeard
        Audiogram.CurrentPointType = AudiogramEditControl.GetPointType

    End Sub

    Public Function GetAudiogramData() As AudiogramData
        Return Audiogram.AudiogramData
    End Function

End Class
