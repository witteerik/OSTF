Namespace SipTest

    Public Class Patient

        'ID etc
        Public ReadOnly Property CreateDate As DateTime

        Property Sessions As New List(Of TestSession)

        Public ReadOnly Property SSNumber As String

        Public ReadOnly Property FirstName As String

        Public ReadOnly Property LastName As String


        Public Sub New(ByVal SSNumber As String)
            'Storing the date and time when the instance of this session was created
            CreateDate = DateTime.Now

            'TODO: Somewhere SSNumber should be checked for validity, prior to creating the patient!
            Me.SSNumber = SSNumber

        End Sub

        ''' <summary>
        ''' Returns a reference to the current session as the last one stored in the patient, or nothing if no sessions have been added
        ''' </summary>
        ''' <returns></returns>
        Public Function GetCurrentSession() As TestSession
            If Sessions Is Nothing Then Return Nothing

            If Sessions.Count = 0 Then
                Return Nothing
            Else
                Return Sessions(Sessions.Count - 1)
            End If

        End Function

    End Class

End Namespace

