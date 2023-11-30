Public Interface IOstfGuiPopup

    Event ShowMessage(ByVal Message As String)




End Interface

Public Module Messager

    Public Event NewMessage(ByVal Title As String, ByVal Message As String, ByVal CancelButtonText As String)

    Public Event NewQuestion As EventHandler(Of QuestionEventArgs)

    Public Enum MsgBoxStyle
        Information
        Exclamation
        Critical
    End Enum

    ''' <summary>
    ''' A message box that will relay any messages to the GUI currently used, but requires that the GUI listens to and handles the NewMessage event.
    ''' </summary>
    ''' <param name="Message"></param>
    ''' <param name="Style"></param>
    ''' <param name="Title"></param>
    ''' <param name="CancelButtonText"></param>
    Public Sub MsgBox(ByVal Message As String, Optional ByVal Style As MsgBoxStyle = MsgBoxStyle.Information, Optional ByVal Title As String = "", Optional ByVal CancelButtonText As String = "OK")

        Select Case Style
            Case MsgBoxStyle.Information

                RaiseEvent NewMessage(Title, Message, CancelButtonText)

            Case MsgBoxStyle.Exclamation

                'TODO, add some "Exclamation" notice...
                RaiseEvent NewMessage(Title, Message, CancelButtonText)

        End Select


    End Sub

    Public Async Function MsgBoxAcceptQuestion(ByVal Question As String, Optional ByVal Title As String = "",
                                          Optional ByVal AcceptButtonText As String = "Yes", Optional ByVal CancelButtonText As String = "No") As Task(Of Boolean)

        Dim tcs As New TaskCompletionSource(Of Boolean)()

        RaiseEvent NewQuestion(Nothing, New QuestionEventArgs(Title, Question, AcceptButtonText, CancelButtonText, tcs))

        Return Await tcs.Task

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

