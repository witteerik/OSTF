Imports System.Reflection

<AttributeUsage(AttributeTargets.Property)>
Public Class ExludeFromPropertyListingAttribute
    Inherits Attribute
End Class


Partial Public Class Logging

    Public Shared Function ListObjectPropertyValues(ByVal T As Type, ByRef Obj As Object) As SortedList(Of String, Object)

        'Dim TimingList As New List(Of Tuple(Of String, Long))
        'Dim MyStopWatch As New Stopwatch
        'MyStopWatch.Start()

        Dim OutputList As New SortedList(Of String, Object)

        'TimingList.Add(New Tuple(Of String, Long)("Created OutputList", MyStopWatch.ElapsedTicks))
        'MyStopWatch.Restart()

        Dim properties As PropertyInfo() = T.GetProperties()

        'TimingList.Add(New Tuple(Of String, Long)("GetProperties", MyStopWatch.ElapsedTicks))
        'MyStopWatch.Restart()

        ' Iterating through each property
        For Each [property] As PropertyInfo In properties

            'MyStopWatch.Restart()

            ' Getting the name of the property
            Dim propertyName As String = [property].Name

            If [property].GetCustomAttribute(Of ExludeFromPropertyListingAttribute)() IsNot Nothing Then

                'TimingList.Add(New Tuple(Of String, Long)("Skipped: " & propertyName, MyStopWatch.ElapsedTicks))

                ' Skip this property
                Continue For
            End If

            'Note that, if a property should be excluded, it's important for performance to skip to next BEFORE the property value is read, as this takes lots of time, especially for large properties!
            ' Getting the value of the property for the current instance 
            Dim propertyValue As Object = [property].GetValue(Obj)

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
                                OutputList.Add(propertyName & "-" & Index, item.ToString().Replace(vbCrLf, "\n").Replace(vbCr, "\n").Replace(vbLf, "\n").Replace(vbTab, "\t"))
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
                        OutputList.Add(propertyName, propertyValue.ToString().Replace(vbCrLf, "\n").Replace(vbCr, "\n").Replace(vbLf, "\n").Replace(vbTab, "\t"))
                    End If
                End If

            Else
                'Properties with null values
                OutputList.Add(propertyName, "NA")
            End If

            'TimingList.Add(New Tuple(Of String, Long)("Stored: " & propertyName, MyStopWatch.ElapsedTicks))

        Next

        'Dim TimingStringList As New List(Of String)
        'For Each Item In TimingList
        '    TimingStringList.Add(Item.Item1 & vbTab & Item.Item2.ToString)
        'Next
        'SendInfoToLog(String.Join(vbCrLf, TimingStringList), "ListedPropertyReflectionTimes")

        Return OutputList

    End Function

End Class

