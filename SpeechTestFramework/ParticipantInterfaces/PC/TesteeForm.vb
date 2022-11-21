Imports System.Windows.Forms
Imports System.Drawing

Public Class PcTesteeForm

    Public WithEvents ParticipantControl As ITesteeControl

    Public Property CurrentTaskType As TaskType
    ''' <summary>
    ''' Hold the currently used ForcedChoiceDirection type TargetDirections
    ''' </summary>
    ''' <returns></returns>
    Public Property CurrentTargetDirections As List(Of Double)

    Public Enum TaskType
        ForcedChoice
        DirectionTask
        SerialChoice
        ForcedChoiceDirection
    End Enum

    Public Sub New(ByVal TaskType As TaskType, Optional ByVal TargetDirections As List(Of Double) = Nothing)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        UpdateType(TaskType, TargetDirections)

    End Sub


    Public Sub UpdateType(ByVal TaskType As TaskType, Optional ByVal TargetDirections As List(Of Double) = Nothing)

        CurrentTaskType = TaskType

        Me.CurrentTargetDirections = TargetDirections

        Me.Controls.Clear()

        Select Case TaskType
            Case TaskType.ForcedChoice
                ParticipantControl = New ForcedChoiceTesteeControl

            Case TaskType.DirectionTask
                ParticipantControl = New SpatializedStilmuliTesteeControl

            Case TaskType.SerialChoice
                ParticipantControl = New SerialChoiceTesteeControl

            Case TaskType.ForcedChoiceDirection
                ParticipantControl = New DirectionalForcedChoiceControl(TargetDirections)

            Case Else
                Throw New NotImplementedException
        End Select

        Me.Controls.Add(ParticipantControl)
        Me.Controls(0).Dock = DockStyle.Fill

    End Sub

    Public Sub UpdateTestFormPosition(ByVal TestPresentationScreenIndex As Integer, Optional ByVal SetWindowState As Boolean = True)

        'This sub puts the test form on the screen with the indicated zero based index.
        'If the indicated index is higher than the number of available screens, the test form will be
        'put on the screen with the highest index.

        Dim screens() As Object = Screen.AllScreens

        If TestPresentationScreenIndex > screens.Count - 1 Then
            MsgBox("There is only " & screens.Count & " screen on the system." & vbCr & vbCr &
                       "Putting the test form on screen " & screens.Count - 1)
            TestPresentationScreenIndex = screens.Count - 1
        End If

        Me.Location() = New Point(screens(TestPresentationScreenIndex).WorkingArea.Left, screens(TestPresentationScreenIndex).WorkingArea.Top)

        'If the selected screen is the primary screen, the test is put in a sizable window, so that the control window can also be seen.
        If SetWindowState = True Then
            If screens(TestPresentationScreenIndex) Is Screen.PrimaryScreen Then
                Me.FormBorderStyle = FormBorderStyle.Sizable
                Me.WindowState = FormWindowState.Normal
            Else
                Me.FormBorderStyle = FormBorderStyle.None
                Me.WindowState = FormWindowState.Maximized
            End If
        End If

    End Sub


    Public Sub ChangeTestFormScreen(ByVal TestPresentationScreenIndex As Integer)

        Me.WindowState = FormWindowState.Normal

        'Increasing the TestPresentationScreenIndex by 1
        TestPresentationScreenIndex += 1

        'Checking that its not too high, if so falling back to index 0
        If TestPresentationScreenIndex > Screen.AllScreens.Count - 1 Then TestPresentationScreenIndex = 0

        'Setting the test form screen 
        UpdateTestFormPosition(TestPresentationScreenIndex, True)

    End Sub

    Public Sub SetResponseMode(ByVal ResponseMode As Utils.ResponseModes)

        'Adjusts cursor depending on response mode
        Select Case ResponseMode
            Case Utils.ResponseModes.MouseClick

                Me.Cursor = Cursors.Hand

            Case Utils.ResponseModes.TabletTouch

                'Hiding the cursor by swapping it to an invisible cursor loaded from file
                'Dim InvisibleCursorPath As String = IO.Path.Combine(Application.StartupPath, "Resources", "InvisibleCursor.cur")
                Dim InvisibleCursorPath As String = "InvisibleCursor.cur"
                Me.Cursor = New Cursor(InvisibleCursorPath)

                'Cursor.Hide()

                'LockCursorToForm()
        End Select


    End Sub

    ''' <summary>
    ''' Puts the mouse on the test form and limits its movements to the test form boundaries.
    ''' </summary>
    Public Sub LockCursorToForm()

        'Puts the mouse on the test form
        Cursor.Position = New Point(Me.ClientRectangle.Left + Me.ClientRectangle.Width / 2, Me.ClientRectangle.Height / 2)

        'Limits mouse movements to the form boundaries
        Cursor.Clip = Me.Bounds

    End Sub

End Class