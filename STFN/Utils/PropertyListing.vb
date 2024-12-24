Imports System.Reflection

<AttributeUsage(AttributeTargets.Property)>
Public Class ExludeFromPropertyListingAttribute
    Inherits Attribute
End Class


Namespace Utils


    Partial Public Module Logging

        Public Function ListObjectPropertyValues(ByVal T As Type, ByRef Obj As Object) As SortedList(Of String, Object)

            Dim OutputList As New SortedList(Of String, Object)

            Dim properties As PropertyInfo() = T.GetProperties()

            ' Iterating through each property
            For Each [property] As PropertyInfo In properties

                ' Getting the name of the property
                Dim propertyName As String = [property].Name

                ' Getting the value of the property for the current instance 
                Dim propertyValue As Object = [property].GetValue(Obj)

                If [property].GetCustomAttribute(Of ExludeFromPropertyListingAttribute)() IsNot Nothing Then
                    ' Skip this property
                    Continue For
                End If

                If propertyValue IsNot Nothing Then

                    ' Check if the property value is a List(Of T)
                    Dim propertyType As Type = propertyValue.GetType()
                    If propertyType.IsGenericType AndAlso propertyType.GetGenericTypeDefinition() = GetType(List(Of)) Then

                        ' Iterate through the List(Of T) and call ToString on each item
                        Dim enumerable As IEnumerable = DirectCast(propertyValue, IEnumerable)
                        Dim Index As Integer = 0
                        For Each item As Object In enumerable

                            If item IsNot Nothing Then
                                If item.GetType.IsValueType Then
                                    ' Handle value type items, simply copying the value
                                    ' Joining the items with hypens
                                    OutputList.Add(propertyName & "-" & Index, item)
                                Else
                                    ' Handle reference type items (calling ToString to get a value instead of a reference)
                                    ' Joining the items with hypens
                                    OutputList.Add(propertyName & "-" & Index, item.ToString())
                                End If
                            Else
                                OutputList.Add(propertyName & "-" & Index, "NA")
                            End If

                            Index += 1

                        Next
                    Else
                        If propertyType.IsValueType Then
                            ' Handle value type items, simply copying the value
                            OutputList.Add(propertyName, propertyValue)
                        Else
                            ' Handle reference type items (calling ToString to get a value instead of a reference)
                            OutputList.Add(propertyName, propertyValue.ToString())
                        End If
                    End If

                Else
                    'Properties with null values
                    OutputList.Add(propertyName, "NA")
                End If

            Next

            Return OutputList

        End Function

    End Module

End Namespace
