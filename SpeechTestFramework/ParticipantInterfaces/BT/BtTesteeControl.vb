﻿
Public Class BtTesteeControl
    Implements ITesteeControl, IBtTestController

    Public BtTabletTalker As BtTabletTalker


    Public Event StartedByTestee() Implements ITesteeControl.StartedByTestee
    Public Event TestWordResponse(ByVal Response As String) Implements ITesteeControl.TestWordResponse


    Public ReadOnly Property InvokeRequired As Boolean Implements IBtTestController.InvokeRequired
        Get
            Return False
            'TODO: Always returning false here, may have to change?? 
            'Throw New NotImplementedException()
        End Get
    End Property


    Public Function Initialize(ByVal UUID As String, ByVal PIN As String, ByVal Language As Utils.Languages) As Boolean

        Dim Failed As Boolean = False

        If BtTabletTalker Is Nothing Then

            BtTabletTalker = New BtTabletTalker(Me, UUID, PIN, Language)
            If BtTabletTalker.EstablishBtConnection() = False Then Failed = True

        Else
            If BtTabletTalker.TrySendData() = False Then Failed = True
        End If

        If Failed = False Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Sub StartNewTestSession()

        'Showing the start button
        ShowStartButton()

    End Sub


    Public Sub ShowStartButton()
        BtTabletTalker.SendBtMessage("S")
    End Sub


    Public Sub ShowResponseAlternatives(ResponseAlternatives As List(Of String)) Implements ITesteeControl.ShowResponseAlternatives

        Dim ResponseAlternativeMessage As String = "TW|" & String.Join("|", ResponseAlternatives)

        BtTabletTalker.SendBtMessage(ResponseAlternativeMessage)

    End Sub

    Public Sub ShowVisualQue() Implements ITesteeControl.ShowVisualQue
        BtTabletTalker.SendBtMessage("C")
    End Sub

    Public Sub HideVisualQue() Implements ITesteeControl.HideVisualQue
        BtTabletTalker.SendBtMessage("B")
    End Sub

    Public Sub ResponseTimesOut() Implements ITesteeControl.ResponseTimesOut
        BtTabletTalker.SendBtMessage("F")
    End Sub

    Public Sub ResetTestWordPanel() Implements ITesteeControl.ResetTestWordPanel
        BtTabletTalker.SendBtMessage("B")
    End Sub


    Public Sub UpdateTestFormProgressbar(Value As Integer, Maximum As Integer, Optional Minimum As Integer = 0) Implements ITesteeControl.UpdateTestFormProgressbar
        BtTabletTalker.SendBtMessage("P|" & Value.ToString & "|" & Maximum.ToString)
    End Sub

    Public Sub ShowMessage(Message As String) Implements ITesteeControl.ShowMessage
        If Message <> "" Then BtTabletTalker.SendBtMessage("Msg|" & Message)
    End Sub

    Public Sub ShowAvaliableDevices(DeviceNames As List(Of String)) Implements IBtTestController.ShowAvaliableDevices
        Throw New NotImplementedException()
    End Sub

    Public Sub IncomingBtMessage(MessageString As String) Implements IBtTestController.IncomingBtMessage

        'We use a § character as end-of-message character
        Dim Messages() = MessageString.TrimEnd("§").Split("§")

        'Going through each incoming message in the order they arrived
        For Each Message In Messages

            If Message.StartsWith("TW|") Then

                'Incoming test-word response
                Dim Response As String = Message.Split("|")(1)
                RaiseEvent TestWordResponse(Response)

            Else
                Select Case Message
                    Case "StartTest"
                        RaiseEvent StartedByTestee()

                    Case Else
                        MsgBox("Recevied unhandled BT message" & Message)
                End Select

            End If

        Next


    End Sub

    Public Function Invoke(method As [Delegate], ParamArray args() As Object) As Object Implements IBtTestController.Invoke
        Throw New NotImplementedException()
    End Function
End Class