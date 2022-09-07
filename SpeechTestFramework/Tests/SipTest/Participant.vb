Namespace SipTest

    Public Class Participant

        'ID etc
        Public ReadOnly Property CreateDate As DateTime

        Property Sessions As New List(Of Measurement)

        Public ReadOnly Property ID As String

        Public ReadOnly Property FirstName As String

        Public ReadOnly Property LastName As String


        Public Sub New(ByVal ID As String)
            'Storing the date and time when the instance of this session was created
            CreateDate = DateTime.Now

            Me.ID = ID

        End Sub

        ''' <summary>
        ''' Returns a reference to the current session as the last one stored in the patient, or nothing if no sessions have been added
        ''' </summary>
        ''' <returns></returns>
        Public Function GetCurrentSession() As Measurement
            If Sessions Is Nothing Then Return Nothing

            If Sessions.Count = 0 Then
                Return Nothing
            Else
                Return Sessions(Sessions.Count - 1)
            End If

        End Function

    End Class

End Namespace

