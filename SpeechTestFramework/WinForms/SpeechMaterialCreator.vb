Public Class SpeechMaterialCreator

    Private DisposeSoundPlayerOnClose As Boolean
    Private UserType As Utils.UserTypes

    Public Sub New()
        MyClass.New(Utils.Constants.UserTypes.Research, True)
    End Sub

    ''' <summary>
    ''' Creates a new instance of SpeechMaterialCreator.
    ''' </summary>
    ''' <param name="DisposeSoundPlayerOnClose">Set to True if the new form/class is started as a standalone application. This will dispose the SoundPlayer when the new form/class is closed. 
    ''' If the form/class is launched from within another OSTF application that uses the SoundPlayer, that application is instead responsible for disposing the SoundPlayer when closed.</param>
    Public Sub New(ByVal UserType As Utils.UserTypes, ByVal DisposeSoundPlayerOnClose As Boolean)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.UserType = UserType
        Me.DisposeSoundPlayerOnClose = DisposeSoundPlayerOnClose

    End Sub

    Private Sub SpeechMaterialCreator_FormClosing(sender As Object, e As Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        'Disposing the sound player.
        If DisposeSoundPlayerOnClose = True Then If SoundPlayerIsInitialized() = True Then SoundPlayer.Dispose()

    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        Dim AboutBox = New AboutBox_WithLicenseButton
        AboutBox.SelectedLicense = LicenseBox.AvailableLicenses.MIT_X11
        AboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.PortAudio)
        AboutBox.LicenseAdditions.Add(LicenseBox.AvailableLicenseAdditions.MathNet)
        AboutBox.ShowDialog()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class