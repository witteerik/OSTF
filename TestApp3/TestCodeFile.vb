Public Class GuiForm
    Inherits Windows.Forms.Form
    Implements IGuiForm

    Public Event InitiateBackend(ByRef GuiForm As IGuiForm) Implements IGuiForm.InitiateBackend

    Public Sub New()
        Dim MyBackend As New MyBackend(Me)
    End Sub

    Sub Call_InitiateBackend()
        RaiseEvent InitiateBackend(Me)
    End Sub


    Public Event SendMessageToBackend(Message As String) Implements IGuiForm.SendMessageToBackend

    Public Sub SendMessageToGui(Message As String) Implements IGuiForm.SendMessageToGui

        MsgBox(Message)

    End Sub

    Public Sub CallBackend()

        RaiseEvent SendMessageToBackend("Blabla")

    End Sub

End Class

Public Interface IGuiForm

    Event InitiateBackend(ByRef GuiForm As IGuiForm)

    Event SendMessageToBackend(ByVal Message As String)
    Sub SendMessageToGui(ByVal Message As String)

End Interface

Public Class MyBackend

    Public WithEvents MyForm As IGuiForm

    Public Sub New(ByRef GuiForm As IGuiForm)
        Me.MyForm = MyForm
    End Sub
    Public Sub Initiate() Handles MyForm.InitiateBackend
        'Other initiation
    End Sub

    Public Sub RecieveMessageFromGuiForm(ByVal Message As String) Handles MyForm.SendMessageToBackend

        'Do something with the message in the backend

    End Sub

    Public Sub CallMyForm()

        MyForm.SendMessageToGui("My message")

    End Sub


End Class