Imports SpeechTestFramework
Public Class OstfSoundPlayerSourceControl

    Public ReadOnly SoundFilePath As String
    Public ReadOnly SoundChannel As Integer
    Private ColumnStyleList As List(Of ColumnStyle)

    Public Event Remove(ByRef SourceControl As OstfSoundPlayerSourceControl)

    Public Sub New(ByVal ColumnStyleList As List(Of ColumnStyle),
                   ByVal SoundFilePath As String,
                   ByVal SoundChannel As Integer,
                   ByVal AvailableSoundSourceLocations As List(Of Audio.SoundScene.SoundSourceLocation),
                   ByVal Levels As List(Of Integer))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.SoundFilePath = SoundFilePath
        Me.SoundChannel = SoundChannel

        FileName_TextBox.Text = System.IO.Path.GetFileNameWithoutExtension(SoundFilePath)
        Channel_Label.Text = SoundChannel

        For Each SoundSource In AvailableSoundSourceLocations
            SoundSource_ComboBox.Items.Add(SoundSource)
        Next
        If SoundSource_ComboBox.Items.Count > 0 Then
            SoundSource_ComboBox.SelectedIndex = 0
        End If

        For Each Level In Levels
            Level_ComboBox.Items.Add(Level)
        Next
        If Levels.Count > 29 Then
            Level_ComboBox.SelectedIndex = 30
        ElseIf Levels.Count > 0 Then
            Level_ComboBox.SelectedIndex = 0
        End If

        Me.ColumnStyleList = ColumnStyleList

    End Sub

    Private Sub OstfSoundPlayerSourceControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Content_TableLayoutPanel.ColumnStyles.Clear()
        For Each Style In ColumnStyleList
            Content_TableLayoutPanel.ColumnStyles.Add(New ColumnStyle(Style.SizeType, Style.Width))
        Next
        Content_TableLayoutPanel.Invalidate()

    End Sub

    Public Function GetLevel() As Double
        Return Level_ComboBox.SelectedItem
    End Function
    Public Function GetSoundSourceLocation() As Audio.SoundScene.SoundSourceLocation
        Return SoundSource_ComboBox.SelectedItem
    End Function

    Public Function ShouldRepeat() As Boolean
        Return Repeat_CheckBox.Checked
    End Function

    Private Sub Remove_Button_Click(sender As Object, e As EventArgs) Handles Remove_Button.Click
        RaiseEvent Remove(Me)
    End Sub

    Private Sub Content_TableLayoutPanel_Paint(sender As Object, e As PaintEventArgs) Handles Content_TableLayoutPanel.Paint

    End Sub
End Class
