'This software is available under the following license:
'MIT/X11 License
'
'Copyright (c) 2017 Erik Witte
'
'Permission is hereby granted, free of charge, to any person obtaining a copy
'of this software and associated documentation files (the ''Software''), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'copies of the Software, and to permit persons to whom the Software is
'furnished to do so, subject to the following conditions:
'
'The above copyright notice and this permission notice shall be included in all
'copies or substantial portions of the Software.
'
'THE SOFTWARE IS PROVIDED ''AS IS'', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
'SOFTWARE.

Namespace Utils
    Public Module StringArrayExclusionMethods

        Public Sub ExcludeStringArrayMembersDueToLength(ByRef input() As String, ByVal upperStringLengthInclusionLimit As Integer)

            Dim inclusionCount As Integer = 0

            For n = 0 To input.Length - 1

                Dim split = input(n).Split(vbTab)

                input(n) = split(0)

                If split(0).Length <= upperStringLengthInclusionLimit Then inclusionCount += 1
            Next

            Dim output(inclusionCount - 1) As String

            Dim outputCounter As Integer = 0
            For n = 0 To input.Length - 1
                If input(n).Length <= upperStringLengthInclusionLimit Then
                    output(outputCounter) = input(n)
                    outputCounter += 1
                End If
            Next

            input = output

        End Sub


        ''' <summary>
        ''' Removes all exact duplicates of a string in an array of String.
        ''' </summary>
        ''' <param name="inputArray"></param>
        Public Sub RemoveStringArrayDuplicates(ByRef inputArray() As String, Optional ByVal LogActive As Boolean = False, Optional TrimBlankSpaces As Boolean = True, Optional UseToLower As Boolean = True)

            Dim startTime As DateTime = DateTime.Now

            Dim originalArrayLength As Integer = inputArray.Length

            If LogActive = True Then
                Utils.SendInfoToLog("Initializing removal of string array duplicates.")
            End If

            'Putting unique strings in a temporary SortedSet (and selectively applying trimming and/or to lower)
            Dim TempSortedSet As New SortedSet(Of String)
            If UseToLower = True Then
                If TrimBlankSpaces = True Then
                    For inputArrayIndex = 0 To inputArray.Length - 1
                        If Not TempSortedSet.Contains(inputArray(inputArrayIndex).Trim.ToLower) Then
                            TempSortedSet.Add(inputArray(inputArrayIndex).Trim.ToLower)
                        End If
                    Next
                Else
                    For inputArrayIndex = 0 To inputArray.Length - 1
                        If Not TempSortedSet.Contains(inputArray(inputArrayIndex).ToLower) Then
                            TempSortedSet.Add(inputArray(inputArrayIndex).ToLower)
                        End If
                    Next
                End If
            Else
                If TrimBlankSpaces = True Then
                    For inputArrayIndex = 0 To inputArray.Length - 1
                        If Not TempSortedSet.Contains(inputArray(inputArrayIndex).Trim) Then
                            TempSortedSet.Add(inputArray(inputArrayIndex).Trim)
                        End If
                    Next
                Else
                    For inputArrayIndex = 0 To inputArray.Length - 1
                        If Not TempSortedSet.Contains(inputArray(inputArrayIndex)) Then
                            TempSortedSet.Add(inputArray(inputArrayIndex))
                        End If
                    Next
                End If
            End If


            'Putting all unique strings in a string array
            Dim outputArray(TempSortedSet.Count - 1) As String
            Dim outputArrayIndex As Integer = 0
            For Each item As String In TempSortedSet
                outputArray(outputArrayIndex) = item
                outputArrayIndex += 1
            Next

            inputArray = outputArray

            If LogActive = True Then
                Utils.SendInfoToLog("     " & originalArrayLength - outputArray.Length & " duplicate strings were removed. " & outputArray.Length & " strings remain in the array." & " Processing time: " & (DateTime.Now - startTime).TotalSeconds & " seconds.")
            End If

        End Sub


        ''' <summary>
        ''' Removes all duplicates strings in an array of string, as long as the duplicates come in a straight order after each other. 
        ''' </summary>
        ''' <param name="inputArray"></param>
        Public Sub RemoveSortedStringArrayDuplicates(ByRef inputArray() As String)

            Dim originalArrayLength As Long = inputArray.Length

            Dim tempArray(inputArray.Length - 1) As String

            tempArray(0) = inputArray(0)
            Dim tempArrayIndex As Long = 1
            For inputArrayIndex = 1 To inputArray.Length - 1
                If Not tempArray(tempArrayIndex - 1) = (inputArray(inputArrayIndex)) Then
                    tempArray(tempArrayIndex) = inputArray(inputArrayIndex)
                    tempArrayIndex += 1
                End If
            Next
            ReDim Preserve tempArray(tempArrayIndex - 1)

            inputArray = tempArray

            'Log originalArrayLength
            'Log tempArray.Length
            'MsgBox(originalArrayLength & " " & tempArray.Length)

        End Sub

        ''' <summary>
        ''' Returns a string array with all strings in the InputArray that also exist in the ListArray.
        ''' </summary>
        ''' <param name="InputArray"></param>
        ''' <param name="ListArray"></param>
        Public Function RemoveStringsNotInList(ByVal InputArray() As String, ByVal ListArray As String()) As String()

            Dim SortedListArraySet As New SortedSet(Of String)
            For Each Item In ListArray
                If Not SortedListArraySet.Contains(Item) Then
                    SortedListArraySet.Add(Item)
                End If
            Next

            Dim OutputList As New List(Of String)

            For n = 0 To InputArray.Length - 1
                If SortedListArraySet.Contains(InputArray(n)) Then
                    OutputList.Add(InputArray(n))
                End If
            Next

            Return OutputList.ToArray

        End Function

    End Module

End Namespace