
Imports System.Windows.Forms
Imports System.Drawing


Public Class ForcedChoiceTesteeControl
    Implements ITesteeControl

    'Declaring delegate subs used for invoking across threads
    Delegate Sub NoArgReturningVoidDelegate()
    Delegate Sub StringArgReturningVoidDelegate([String] As String)
    Delegate Sub ListOfStringArgReturningVoidDelegate(StringList As List(Of String))
    Delegate Sub ListOfStringLocationTupleArgReturningVoidDelegate(StringList As List(Of Tuple(Of String, Audio.PortAudioVB.DuplexMixer.SoundSourceLocation)))
    Delegate Sub ProgressBarArgReturningVoidDelegate(ByVal Value As Integer, ByVal Maximum As Integer, ByVal Minimum As Integer)


    ''' <summary>
    ''' Sets or returns the color of the background of the test word labels, as well as the color of the auditory presentation circle.
    ''' </summary>
    ''' <returns></returns>
    Public Property ItemColor As Color = Drawing.Color.FromArgb(255, 255, 128)
    Private DarkGreyBackColor As Color = Drawing.Color.FromArgb(40, 40, 40)
    Friend WithEvents TestSurfacePictureBox As New PictureBox
    Private CircleBrush As Drawing.SolidBrush = New Drawing.SolidBrush(ItemColor)

    Friend WithEvents StartButton As New TestWordLabel(Me, ItemColor) With {.Text = "START"}
    Friend WithEvents PauseButton As New TestWordLabel(Me, ItemColor, 14) With {.Text = "Paus", .Visible = False}
    Friend MessageLabel As New TestWordLabel(Me, ItemColor) With {.Visible = False}
    Friend WithEvents TestFormProgressBar As New ProgressBar_Old(Me, ItemColor, DarkGreyBackColor) With {.Visible = True, .Dock = DockStyle.Bottom}

    Private WithEvents HideResponseButtonsAfterClickTimer As New Timers.Timer With {.Interval = 500}

    Public Event StartedByTestee(sender As Object, e As EventArgs) Implements ITesteeControl.StartedByTestee
    Public Event ResponseGiven(ByVal Response As String) Implements ITesteeControl.ResponseGiven


    Private Sub ForcedChoiceTesteeControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        'Layout
        Me.BackColor = DarkGreyBackColor
        TestSurfacePictureBox.Dock = DockStyle.Fill
        TestSurfacePictureBox.BackColor = DarkGreyBackColor
        Me.Controls.Add(TestSurfacePictureBox)

        TestSurfacePictureBox.Controls.Add(StartButton)
        'TestSurfacePictureBox.Controls.Add(PauseButton)
        'TestSurfacePictureBox.Controls.Add(MessageLabel)
        TestSurfacePictureBox.Controls.Add(TestFormProgressBar)


    End Sub


    Private Sub StartButton_Click(sender As Object, e As EventArgs) Handles StartButton.Click
        RaiseEvent StartedByTestee(sender, e)
    End Sub


#Region "Trial presentation"

    Private DrawVisualQue As Boolean = False

    ''' <summary>
    ''' Shows the visual que on the test control.
    ''' </summary>
    Private Sub ShowVisualQue() Implements ITesteeControl.ShowVisualQue
        DrawVisualQue = True
        RefreshTestSurfacePictureBox()
    End Sub

    ''' <summary>
    ''' Removes the visual que on the test control.
    ''' </summary>
    Private Sub HideVisualQue() Implements ITesteeControl.HideVisualQue
        DrawVisualQue = False
        RefreshTestSurfacePictureBox()
    End Sub

    ''' <summary>
    ''' Displays the response alternatives in a thread safe way.
    ''' </summary>
    ''' <param name="ResponseAlternatives"></param>
    Private Sub ShowResponseAlternatives(ByVal ResponseAlternatives As List(Of Tuple(Of String, Audio.PortAudioVB.DuplexMixer.SoundSourceLocation))) Implements ITesteeControl.ShowResponseAlternatives

        Try

            If Me.TestSurfacePictureBox.InvokeRequired Then
                Dim d As New ListOfStringLocationTupleArgReturningVoidDelegate(AddressOf ShowResponseAlternatives_UnSafe)
                Me.Invoke(d, New Object() {ResponseAlternatives})
            Else
                Me.ShowResponseAlternatives_UnSafe(ResponseAlternatives)
            End If

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
        End Try

    End Sub

    Private Sub ShowResponseAlternatives_UnSafe(ByVal ResponseAlternatives As List(Of Tuple(Of String, Audio.PortAudioVB.DuplexMixer.SoundSourceLocation)))

        ResetTestItemPanel()

        'Adding new test words labels to the TestWordPanel
        For i = 0 To ResponseAlternatives.Count - 1

            'Creating labels that will hold the response alternatives
            Dim SpellingLabel As New TestWordLabel(TestSurfacePictureBox, ItemColor) With {.Text = ResponseAlternatives(i).Item1}

            'Adding the current test word label
            TestSurfacePictureBox.Controls.Add(SpellingLabel)

            'Setting size of the label and positioning it within the test word panel
            SpellingLabel.Width = (6 / 15) * TestSurfacePictureBox.Width
            SpellingLabel.Height = (6 / 15) * TestSurfacePictureBox.Height

            'Setting left
            Select Case i
                Case 0
                    SpellingLabel.Left = (1 / 15) * TestSurfacePictureBox.Width
                Case 1, 3
                    SpellingLabel.Left = (8 / 15) * TestSurfacePictureBox.Width
                Case 2
                    Select Case ResponseAlternatives.Count
                        Case 3
                            SpellingLabel.Left = (TestSurfacePictureBox.Width / 2) - (SpellingLabel.Width / 2)
                        Case 4
                            SpellingLabel.Left = (1 / 15) * TestSurfacePictureBox.Width
                        Case Else
                            Throw New NotImplementedException("The current testee GUI only supports 1-4 test words.")
                    End Select
            End Select

            'Setting top
            Select Case i
                Case 0, 1
                    SpellingLabel.Top = (1 / 15) * TestSurfacePictureBox.Height
                Case 2, 3
                    SpellingLabel.Top = (8 / 15) * TestSurfacePictureBox.Height
            End Select


        Next

        RefreshTestSurfacePictureBox()

        'Activating the response event handlers
        ActivateResponseEventHandlers()

    End Sub

    ''' <summary>
    ''' Activating the event handlers needed to enable response. (This should be done after the sound has been played so the controls cannot be clicked too early.)
    ''' </summary>
    Private Sub ActivateResponseEventHandlers()

        'Adds event handlers to the labels in the controlpanel. This is done after the sound is played so they cannot be clicked too early
        For Each Control In TestSurfacePictureBox.Controls

            Dim CurrentControl = TryCast(Control, TestWordLabel)
            If CurrentControl IsNot Nothing Then
                AddHandler CurrentControl.MouseDown, AddressOf Me.TestWordLabel_MouseDown
            End If

        Next

    End Sub

    ''' <summary>
    ''' Inactivating the event handlers needed to enable response.
    ''' </summary>
    Private Sub InactivateResponseEventHandlers()

        'Removes the eventhandlers so that the labels cannot be clicked again
        For Each Control In TestSurfacePictureBox.Controls

            Dim CurrentControl = TryCast(Control, TestWordLabel)
            If CurrentControl IsNot Nothing Then
                RemoveHandler CurrentControl.MouseDown, AddressOf Me.TestWordLabel_MouseDown
            End If
        Next

    End Sub

    'Section on response

    ''' <summary>
    ''' Sub for handling the testee response.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TestWordLabel_MouseDown(sender As System.Object, e As System.EventArgs)

        'Inactivates the event handlers
        InactivateResponseEventHandlers()

        'Getting the clicked label
        Dim ClickedLabel = TryCast(sender, TestWordLabel)

        'Hides the labels that were not clicked
        For Each Control In TestSurfacePictureBox.Controls

            Dim CurrentControl = TryCast(Control, TestWordLabel)
            If CurrentControl IsNot Nothing Then
                If CurrentControl IsNot ClickedLabel Then CurrentControl.Visible = False
            End If

        Next

        'Updates the TestSurfacePictureBox layout
        TestSurfacePictureBox.Update()

        'Starts the timer that will remove also the clicked button
        HideResponseButtonsAfterClickTimer.Start()

        'Sending result to controller

        RaiseEvent ResponseGiven(ClickedLabel.Text)

    End Sub


    Private Sub HideResponseButtonsAfterClickTimer_Tick() Handles HideResponseButtonsAfterClickTimer.Elapsed
        HideResponseButtonsAfterClickTimer.Stop()
        ResetTestItemPanel()
    End Sub


    Private Sub ResponseTimesOut() Implements ITesteeControl.ResponseTimesOut

        Try

            If Me.TestSurfacePictureBox.InvokeRequired Then
                Dim d As New NoArgReturningVoidDelegate(AddressOf ResponseTimesOut_UnSafe)
                Me.Invoke(d)
            Else
                Me.ResponseTimesOut_UnSafe()
            End If

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
        End Try

    End Sub


    ''' <summary>
    ''' This sub should be called by the controller, when the response time has run out
    ''' </summary>
    Private Sub ResponseTimesOut_UnSafe()

        'Inactivates the event handlers
        InactivateResponseEventHandlers()

        'Changes the backgroundcolour of all labels if no answer has been given
        For Each Control In TestSurfacePictureBox.Controls

            Dim CurrentControl = TryCast(Control, TestWordLabel)
            If CurrentControl IsNot Nothing Then
                CurrentControl.BackColor = Color.Red
            End If

        Next

    End Sub

    ''' <summary>
    ''' Removes all TestWordLabel controls from the TestSurfacePictureBox in a thread-safe way. 
    ''' </summary>
    Private Sub ResetTestItemPanel() Implements ITesteeControl.ResetTestItemPanel

        Try

            If Me.TestSurfacePictureBox.InvokeRequired Then
                Dim d As New NoArgReturningVoidDelegate(AddressOf ResetTestItemPanel)
                Me.Invoke(d)
            Else
                Me.ResetTestWordPanel_Unsafe()
            End If

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
        End Try

    End Sub


    ''' <summary>
    ''' Removes all TestWordLabel controls from the TestSurfacePictureBox. 
    ''' </summary>
    Private Sub ResetTestWordPanel_Unsafe()

        Dim InitialControlCount As Integer = TestSurfacePictureBox.Controls.Count
        Dim ControlsToRetain As New List(Of Control)

        For Each CurrentControl In TestSurfacePictureBox.Controls
            If CurrentControl.GetType IsNot GetType(TestWordLabel) Then ControlsToRetain.Add(CurrentControl)
        Next

        TestSurfacePictureBox.Controls.Clear()

        For Each Control In ControlsToRetain
            TestSurfacePictureBox.Controls.Add(Control)
        Next

        TestSurfacePictureBox.Update()

    End Sub

    ''' <summary>
    ''' Refreshes the TestSurfacePictureBox in a thread-safe way.
    ''' </summary>
    Private Sub RefreshTestSurfacePictureBox()

        Try

            If Me.InvokeRequired Then

                Dim d As New NoArgReturningVoidDelegate(AddressOf RefreshTestSurfacePictureBox)
                Me.Invoke(d)

            Else
                Me.TestSurfacePictureBox.Refresh()
            End If

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
        End Try

    End Sub


#End Region

#Region "Test control"

    'Key events
    Dim PauseTime As DateTime

    Public Sub UpdateTestFormProgressbar(ByVal Value As Integer, ByVal Maximum As Integer, Optional ByVal Minimum As Integer = 0) Implements ITesteeControl.UpdateTestFormProgressbar
        Try

            If Me.TestSurfacePictureBox.InvokeRequired Then
                Dim d As New ProgressBarArgReturningVoidDelegate(AddressOf UpdateTestFormProgressbarUnsafe)
                Me.Invoke(d, {Value, Maximum, Minimum})
            Else
                Me.UpdateTestFormProgressbarUnsafe(Value, Maximum, Minimum)
            End If

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
        End Try

    End Sub


    Private Sub UpdateTestFormProgressbarUnsafe(ByVal Value As Integer,
                                                ByVal Maximum As Integer,
                                                Optional ByVal Minimum As Integer = 0)

        Try

            TestFormProgressBar.Minimum = Minimum
            TestFormProgressBar.Maximum = Maximum
            TestFormProgressBar.Value = Value

            TestFormProgressBar.Visible = True
            TestFormProgressBar.Refresh()

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
        End Try

    End Sub



#End Region

#Region "Graphics and layout"

    Private Sub Resized(sender As System.Object, e As System.EventArgs) Handles MyBase.Resize

        StartButton.Height = Me.ClientRectangle.Height / 2
        StartButton.Width = Me.ClientRectangle.Width / 2
        StartButton.Top = Me.ClientRectangle.Height / 2 - StartButton.Height / 2
        StartButton.Left = Me.ClientRectangle.Width / 2 - StartButton.Width / 2

        PauseButton.Height = 25
        PauseButton.Width = 80
        PauseButton.Top = 1
        PauseButton.Left = Me.ClientRectangle.Width / 2 - PauseButton.Width / 2

        MessageLabel.Height = Me.ClientRectangle.Height / 2
        MessageLabel.Top = Me.ClientRectangle.Height / 2 - StartButton.Height / 2
        MessageLabel.Width = (4 / 5) * Me.Width
        MessageLabel.Left = (1 / 10) * Me.Width

    End Sub


    Private Sub DrawCircle(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles TestSurfacePictureBox.Paint

        If DrawVisualQue = True Then

            ' Create a local version of the graphics object for the PictureBox.
            Dim g As System.Drawing.Graphics = e.Graphics

            Dim Circle_Width As Integer = Me.ClientRectangle.Height / 2
            Dim Circle_Height As Integer = Circle_Width
            Dim Circle_Top As Integer = Me.ClientRectangle.Height / 2 - Circle_Height / 2
            Dim Circle_Left As Integer = Me.ClientRectangle.Width / 2 - Circle_Width / 2

            Dim CirecleRectangle As New Rectangle(Circle_Left, Circle_Top, Circle_Width, Circle_Height)
            g.FillEllipse(CircleBrush, CirecleRectangle)

        End If

    End Sub


    ''' <summary>
    ''' Shows the response confidence buttons in a thread-safe way.
    ''' </summary>
    Private Sub ShowMessage(ByVal Message As String) Implements ITesteeControl.ShowMessage

        Try

            If Me.TestSurfacePictureBox.InvokeRequired Then
                Dim d As New StringArgReturningVoidDelegate(AddressOf ShowMessage_UnSafe)
                Me.Invoke(d, New Object() {Message})
            Else
                Me.ShowMessage_UnSafe(Message)
            End If

        Catch ex As Exception
            Utils.SendInfoToLog(ex.ToString, "ExceptionsDuringTesting")
        End Try

    End Sub

    Private Sub ShowMessage_UnSafe(ByVal Message As String)

        'Clearing the panel
        ResetTestItemPanel()

        'Storing the message in the MessageLabel 
        MessageLabel.Text = Message

        'Showing the MessageLabel
        MessageLabel.Visible = True

        'Displaying the message label
        TestSurfacePictureBox.Controls.Add(MessageLabel)

        'Resfreshing it TestSurfacePictureBox so that it will always show the message.
        TestSurfacePictureBox.Refresh()

    End Sub


#End Region


End Class
