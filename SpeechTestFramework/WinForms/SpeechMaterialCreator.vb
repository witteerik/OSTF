Public Class SpeechMaterialCreator

    Private UserType As Utils.UserTypes

    Public ReadOnly Property IsStandAlone As Boolean

    Public Sub New()
        MyClass.New(Utils.Constants.UserTypes.Research, True)
    End Sub

    ''' <summary>
    ''' Creates a new instance of SpeechMaterialCreator.
    ''' </summary>
    ''' <param name="UserType"></param>
    ''' <param name="IsStandAlone">Set to true if called from within another OSTF application, and False if run as a standalone OSTF application. </param>
    Public Sub New(ByVal UserType As Utils.UserTypes, ByVal IsStandAlone As Boolean)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.UserType = UserType
        Me.IsStandAlone = IsStandAlone

    End Sub

    Private Sub SpeechMaterialCreator_FormClosing(sender As Object, e As Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If IsStandAlone = True Then OstfBase.TerminateOSTF()
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        Dim AboutBox = New AboutBox_WithLicenseButton
        AboutBox.SelectedLicense = LicenseBox.AvailableLicenses.MIT_X11
        AboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.PortAudio)
        AboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.MathNet)
        AboutBox.ShowDialog()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        If IsStandAlone = True Then OstfBase.TerminateOSTF()
        Me.Close()
    End Sub

    Private Sub MySpeechMaterialComponentCreator_Load(sender As Object, e As EventArgs) Handles MySpeechMaterialComponentCreator.Load

    End Sub
End Class