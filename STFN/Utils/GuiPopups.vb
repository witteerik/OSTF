Public Interface IOstfGuiPopup

    Event ShowMessage(ByVal Message As String)




End Interface

Public Module Messager

    Public Event NewMessage(ByVal Title As String, ByVal Message As String, ByVal CancelButtonText As String)

    Public Event NewQuestion(ByVal Title As String, ByVal Question As String, ByVal AcceptButtonText As String, ByVal CancelButtonText As String)

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

    Public Function MsgBoxBooleanQuestion(ByVal Question As String, Optional ByVal Style As MsgBoxStyle = MsgBoxStyle.Information, Optional ByVal Title As String = "",
                                          Optional ByVal AcceptButtonText As String = "Yes", Optional ByVal CancelButtonText As String = "No") As Boolean

        RaiseEvent NewQuestion(Title, Question, AcceptButtonText, CancelButtonText)


    End Function



End Module

