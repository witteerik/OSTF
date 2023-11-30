Public Interface IOstfGuiPopup

    Event ShowMessage(ByVal Message As String)




End Interface

Public Module Messager

    Public Event NewMessage(ByVal Title As String, ByVal Message As String, ByVal CancelButtonText As String)

    Public Event NewQuestion(ByVal Title As String, ByVal Question As String, ByVal AcceptButtonText As String, ByVal CancelButtonText As String, ByRef Result As Boolean)

    Public Event QuestionSent As EventHandler(Of QuestionEventArgs)

    'Public Event x As Task(Of Boolean)

    Public Enum MsgBoxStyle
        OkOnly
        OkCancel
        AbortRetryIgnore
        YesNoCancel
        YesNo
        RetryCancel
        Critical
        Question
        Exclamation
        Information
        DefaultButton1
        DefaultButton2
        DefaultButton3
        ApplicationModal
        SystemModal
        MsgBoxHelp
        MsgBoxRight
        MsgBoxRtlReading
        MsgBoxSetForeground
    End Enum

    ''' <summary>
    ''' A message box that will relay any messages to the GUI currently used, but requires that the GUI listens to and handles the NewMessage event.
    ''' </summary>
    ''' <param name="Message"></param>
    ''' <param name="Style"></param>
    ''' <param name="Title"></param>
    ''' <param name="CancelButtonText"></param>
    Public Sub MsgBox(ByVal Message As String, Optional ByVal Style As MsgBoxStyle = MsgBoxStyle.Information, Optional ByVal Title As String = "", Optional ByVal CancelButtonText As String = "OK")

        ' For now just the ignoring style argument
        RaiseEvent NewMessage(Title, Message, CancelButtonText)

    End Sub

    Public Function MsgBoxAcceptQuestion(ByVal Question As String, Optional ByVal Style As MsgBoxStyle = MsgBoxStyle.Information, Optional ByVal Title As String = "",
                                          Optional ByVal AcceptButtonText As String = "Yes", Optional ByVal CancelButtonText As String = "No") As Boolean

        Dim Result As Boolean = SendQuestionAndWait(Title, Question, AcceptButtonText, CancelButtonText).Result

        Return Result

    End Function


    Public Function SendQuestionAndWait(title As String, question As String, acceptButtonText As String, cancelButtonText As String) As Task(Of Boolean)

        Dim tcs As New TaskCompletionSource(Of Boolean)()

        RaiseEvent QuestionSent(Nothing, New QuestionEventArgs(title, question, acceptButtonText, cancelButtonText, tcs))

        Return tcs.Task

    End Function


End Module

Public Class QuestionEventArgs
    Inherits EventArgs

    Public Property Title As String
    Public Property Question As String
    Public Property AcceptButtonText As String
    Public Property CancelButtonText As String
    Public Property TaskCompletionSource As TaskCompletionSource(Of Boolean)

    Public Sub New(title As String, question As String, acceptButtonText As String, cancelButtonText As String, tcs As TaskCompletionSource(Of Boolean))
        Me.Title = title
        Me.Question = question
        Me.AcceptButtonText = acceptButtonText
        Me.CancelButtonText = cancelButtonText
        Me.TaskCompletionSource = tcs
    End Sub
End Class

