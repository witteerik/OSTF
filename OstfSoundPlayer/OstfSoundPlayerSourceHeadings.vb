Public Class OstfSoundPlayerSourceHeadings

    Private ColumnStyleList As List(Of ColumnStyle)

    Public Sub New(ByVal ColumnStyleList As List(Of ColumnStyle))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.ColumnStyleList = ColumnStyleList

    End Sub


    Private Sub OstfSoundPlayerSourceHeadings_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Content_TableLayoutPanel.ColumnStyles.Clear()
        For Each Style In ColumnStyleList
            Content_TableLayoutPanel.ColumnStyles.Add(New ColumnStyle(Style.SizeType, Style.Width))
        Next
        Content_TableLayoutPanel.Invalidate()

    End Sub

End Class
