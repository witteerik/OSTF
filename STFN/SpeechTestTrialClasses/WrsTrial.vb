Imports System.Reflection

Public Class WrsTrial
    Inherits TestTrial

    Public Property SpeechLevel As Double

    Public Property MaskerLevel As Double

    Public Property ContralateralMaskerLevel As Double

    Public Property AdaptiveValue As Double

    Public Property TestEar As Utils.SidesWithBoth

    Public ReadOnly Property SNR As Double
        Get
            Return SpeechLevel - MaskerLevel
        End Get
    End Property

    Public Property LinguisticResponse As String = ""

    Public Overrides Function TestResultColumnHeadings() As String

        Dim OutputList As New List(Of String)
        OutputList.AddRange(BaseClassTestResultColumnHeadings())

        'Adding property names
        Dim properties As PropertyInfo() = GetType(SrtTrial).GetProperties()

        ' Iterating through each property
        For Each [property] As PropertyInfo In properties

            ' Getting the name of the property
            Dim propertyName As String = [property].Name
            OutputList.Add(propertyName)

        Next

        Return String.Join(vbTab, OutputList)

    End Function

    Public Overrides Function TestResultAsTextRow() As String

        Dim OutputList As New List(Of String)
        OutputList.AddRange(BaseClassTestResultAsTextRow())

        Dim properties As PropertyInfo() = GetType(SrtTrial).GetProperties()

        ' Iterating through each property
        For Each [property] As PropertyInfo In properties

            ' Getting the name of the property
            Dim propertyName As String = [property].Name

            ' Getting the value of the property for the current instance 
            Dim propertyValue As Object = [property].GetValue(Me)

            'If TypeOf propertyValue Is String Then
            '    Dim stringValue As String = DirectCast(propertyValue, String)
            'ElseIf TypeOf propertyValue Is Integer Then
            '    Dim intValue As Integer = DirectCast(propertyValue, Integer)
            'Else
            'End If

            OutputList.Add(propertyValue.ToString)

        Next

        Return String.Join(vbTab, OutputList)

    End Function


End Class
