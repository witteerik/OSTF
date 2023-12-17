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
    Public Module Math

        Public Function Repeat(ByVal Value As Integer, ByVal Length As Integer) As Integer()
            Dim Output As New List(Of Integer)
            For i = 1 To Length
                Output.Add(Value)
            Next
            Return Output.ToArray
        End Function

        Public Function Repeat(ByVal Value As Double, ByVal Length As Integer) As Double()
            Dim Output As New List(Of Double)
            For i = 1 To Length
                Output.Add(Value)
            Next
            Return Output.ToArray
        End Function

        Public Function Repeat(ByVal Value As String, ByVal Length As Integer) As String()
            Dim Output As New List(Of String)
            For i = 1 To Length
                Output.Add(Value)
            Next
            Return Output.ToArray
        End Function


        ''' <summary>
        ''' Returns a vector of length n, with random integers sampled in from the range of min (includive) to max (exclusive).
        ''' </summary>
        ''' <returns></returns>
        Public Function SampleWithoutReplacement(ByVal n As Integer, ByVal min As Integer, ByVal max As Integer,
                                             Optional randomSource As Random = Nothing) As Integer()

            If randomSource Is Nothing Then randomSource = New Random()

            If n > max - min Then Throw New ArgumentException("max minus min must be equal to or greater than n")

            Dim SampleData As New HashSet(Of Integer)
            Dim NewSample As Integer

            'Sampling data until the length of SampleData equals n
            Do Until SampleData.Count >= n

                'Getting a random sample
                NewSample = randomSource.Next(min, max)

                'Adding the sample only if it is not already present in SampleData 
                If Not SampleData.Contains(NewSample) Then SampleData.Add(NewSample)
            Loop

            Return SampleData.ToArray

        End Function

        Public Function Shuffle(ByVal Input As List(Of String), ByRef Randomizer As Random) As List(Of String)
            Dim SampleOrder = SampleWithoutReplacement(Input.Count, 0, Input.Count, Randomizer)
            Dim Output As New List(Of String)
            For Each RandomIndex In SampleOrder
                Output.Add(Input(RandomIndex))
            Next
            Return Output
        End Function


        Public Function Shuffle(ByVal Input As List(Of SpeechTestResponseAlternative), ByRef Randomizer As Random) As List(Of SpeechTestResponseAlternative)
            Dim SampleOrder = SampleWithoutReplacement(Input.Count, 0, Input.Count, Randomizer)
            Dim Output As New List(Of SpeechTestResponseAlternative)
            For Each RandomIndex In SampleOrder
                Output.Add(Input(RandomIndex))
            Next
            Return Output
        End Function


        Public Function getBase_n_Log(ByVal value As Double, Optional ByVal n As Double = 2) As Double

            Return System.Math.Log10(value) / System.Math.Log10(n)

        End Function

        ''' <summary>
        ''' Gets the highest value in an array of nullable of Doubles, or Nothing, if no values exist.
        ''' </summary>
        ''' <param name="InputArray"></param>
        ''' <returns></returns>
        Public Function GetMax(ByVal InputArray() As Double?) As Double?

            Dim CurrentMax As Double? = Nothing
            For Each Item In InputArray
                If Item.HasValue = True Then
                    If CurrentMax.HasValue Then
                        CurrentMax = System.Math.Max(Item.Value, CurrentMax.Value)
                    Else
                        CurrentMax = Item.Value
                    End If
                End If
            Next
            Return CurrentMax

        End Function

        Public Enum roundingMethods
            getClosestValue
            alwaysDown
            alwaysUp
            donNotRound
        End Enum

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="inputValue"></param>
        ''' <param name="roundingMethod"></param>
        ''' <param name="DecimalsInReturnsString"></param>
        ''' <param name="SkipRounding">If set to true, the rounding function is inactivated and the input value is returned unalterred.</param>
        ''' <returns></returns>
        Public Function Rounding(ByVal inputValue As Object, Optional ByVal roundingMethod As roundingMethods = roundingMethods.getClosestValue,
                             Optional DecimalsInReturnsString As Integer? = Nothing, Optional ByVal SkipRounding As Boolean = False,
                             Optional MinimumNonDecimalsInReturnString As Integer? = Nothing)

            'Returns the input value, if SkipRounding is true
            If SkipRounding = True Then Return inputValue

            Try

                Dim ReturnValue As Double = inputValue

                Select Case roundingMethod
                    Case roundingMethods.alwaysDown
                        ReturnValue = Int(ReturnValue)

                    Case roundingMethods.alwaysUp
                        If Not inputValue - Int(inputValue) = 0 Then
                            ReturnValue = Int(ReturnValue) + 1
                        Else
                            ReturnValue = ReturnValue
                        End If

                    Case roundingMethods.donNotRound
                        ReturnValue = ReturnValue

                    Case roundingMethods.getClosestValue

                        If DecimalsInReturnsString Is Nothing Then
                            ReturnValue = (System.Math.Round(ReturnValue))
                            'If not midpoint rounding is done below
                        End If

                    Case Else
                        Throw New Exception("The " & roundingMethod & " rounding method enumerator is not valid.")
                        Return Nothing
                End Select

                Dim RetString As String = ""
                If DecimalsInReturnsString IsNot Nothing Or MinimumNonDecimalsInReturnString IsNot Nothing Then

                    If DecimalsInReturnsString < 0 Then Throw New ArgumentException("DecimalsInReturnsString cannot be lower than 0.")
                    If MinimumNonDecimalsInReturnString < 0 Then Throw New ArgumentException("MinimumNonDecimalsInReturnString cannot be lower than 0.")

                    'Adding decimals to format
                    Dim NumberFormat As String = "0"
                    If DecimalsInReturnsString IsNot Nothing Then
                        For n = 0 To DecimalsInReturnsString - 1
                            If n = 0 Then NumberFormat &= "."
                            NumberFormat &= "0"
                        Next
                    End If

                    'Adding non-decimals to format
                    If MinimumNonDecimalsInReturnString IsNot Nothing Then
                        For n = 0 To MinimumNonDecimalsInReturnString - 2 ' -2 as one 0 has already been added above
                            NumberFormat = "0" & NumberFormat
                        Next
                    End If

                    RetString = ReturnValue.ToString(NumberFormat).TrimEnd("0").Trim(".").Trim(",")
                    If RetString = "" Then RetString = "0"

                    Return RetString

                End If

                Return ReturnValue

            Catch ex As Exception
                MsgBox(ex.ToString)
                Return Nothing
            End Try

        End Function

        Public Function RoundToNearestIntegerMultiple(ByVal InputValue As Double, ByVal IntegerMultiple As Integer, Optional ByVal roundingMethod As roundingMethods = roundingMethods.getClosestValue) As Integer

            Dim x = Int(InputValue / IntegerMultiple)

            Select Case roundingMethod
                Case roundingMethods.alwaysUp

                    If InputValue Mod IntegerMultiple = 0 Then
                        'No rounding needed, the input value is already a integer multiple of IntegerMultiple
                        Return InputValue
                    Else
                        'Rounding downwards
                        Return (x + 1) * IntegerMultiple
                    End If

                Case roundingMethods.alwaysDown
                    If InputValue Mod IntegerMultiple = 0 Then
                        'No rounding needed, the input value is already a integer multiple of IntegerMultiple
                        Return InputValue
                    Else
                        'Rounding upwards
                        Return x * IntegerMultiple
                    End If

                Case roundingMethods.getClosestValue

                    If InputValue Mod IntegerMultiple = 0 Then
                        'No rounding needed, the input value is already a integer multiple of IntegerMultiple
                        Return InputValue
                    Else

                        If (InputValue Mod IntegerMultiple) < (IntegerMultiple / 2) Then
                            'Rounding upwards
                            Return (x * IntegerMultiple)
                        Else
                            'Rounding downwards
                            Return (x + 1) * IntegerMultiple
                        End If
                    End If

                Case roundingMethods.donNotRound
                    Return InputValue
                Case Else
                    Throw New NotImplementedException
            End Select

        End Function

        ''' <summary>
        ''' Rounds the Frequency value to the nearest log2 frequency (Suitable to round audiogram frequencies).
        ''' </summary>
        ''' <param name="Frequency">The array on valid output frequencies (to round to). If not supplied the default value will be standard audiogram frequencies.</param>
        ''' <returns></returns>
        Public Function RoundToLog2Frequency(ByVal Frequency As Double, Optional ValidFrequencies As SortedSet(Of Double) = Nothing)

            If ValidFrequencies Is Nothing Then
                ValidFrequencies = New SortedSet(Of Double) From {125, 250, 500, 750, 1000, 1500, 2000, 3000, 4000, 6000, 8000}
            End If

            Dim ValidFrequenciesLog2 As New SortedSet(Of Double)
            For Each f In ValidFrequencies
                ValidFrequenciesLog2.Add(Utils.getBase_n_Log(f, 2))
            Next
            Dim NearestIndex = Utils.GetNearestIndex(Utils.getBase_n_Log(Frequency, 2), ValidFrequenciesLog2, True)
            Dim RoundedValue = ValidFrequencies(NearestIndex)
            Return RoundedValue

        End Function

        Public Function RoundToAudiogramLevel(ByVal Level As Double, Optional ByVal MaxLevel As Double = 110, Optional ByVal MinLevel As Double = -10)

            Dim RoundedValue = Utils.RoundToNearestIntegerMultiple(Level, 5)
            RoundedValue = System.Math.Max(MinLevel, RoundedValue)
            RoundedValue = System.Math.Min(MaxLevel, RoundedValue)

            Return RoundedValue

        End Function

        ''' <summary>
        ''' Calculates Zipf Value from raw word type frequency.
        ''' </summary>
        ''' <param name="RawWordTypeFrequency">The total number of times a tokens exists in the corpus used.</param>
        ''' <param name="CorpusTotalTokenCount">The total number of tokens in the corpus used to set the raw word type frequency.</param>
        ''' <param name="CorpusTotalWordTypeCount">The total number of word types in the corpus used to set the raw word type frequency.</param>
        ''' <param name="PositionTerm"></param>
        Public Function CalculateZipfValue(ByRef RawWordTypeFrequency As Long, ByVal CorpusTotalTokenCount As Long, ByVal CorpusTotalWordTypeCount As Integer, Optional ByVal PositionTerm As Integer = 3)

            Return System.Math.Log10((RawWordTypeFrequency + 1) / ((CorpusTotalTokenCount + CorpusTotalWordTypeCount) / 1000000)) + PositionTerm

        End Function

        Public Function ConvertZipfValueToRawFrequency(ByRef ZipfValue As Double, ByVal CorpusTotalTokenCount As Long, ByVal CorpusTotalWordTypeCount As Integer, Optional ByVal PositionTerm As Integer = 3) As Integer

            Try
                Return System.Math.Round(10 ^ (ZipfValue - PositionTerm) * ((CorpusTotalTokenCount + CorpusTotalWordTypeCount) / 1000000) - 1)
            Catch ex As Exception
                Return 0
            End Try

        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="RelativeWordTypeFrequency">The total number of times a tokens exists in the corpus used.</param>
        ''' <param name="CorpusTotalTokenCount">The total number of tokens in the corpus used to set the raw word type frequency.</param>
        ''' <param name="CorpusTotalWordTypeCount">The total number of word types in the corpus used to set the raw word type frequency.</param>
        ''' <param name="PositionTerm"></param>
        ''' <returns></returns>
        Public Function CalculateZipfValueFromRelativeFrequency(ByRef RelativeWordTypeFrequency As Double, ByVal CorpusTotalTokenCount As Long, ByVal CorpusTotalWordTypeCount As Long, Optional ByVal PositionTerm As Integer = 3)

            Return System.Math.Log10(((RelativeWordTypeFrequency * 1000000) + 1) / (((CorpusTotalTokenCount + CorpusTotalWordTypeCount) / 1000000))) + PositionTerm

        End Function

        ''' <summary>
        ''' Returns the geometric mean in a list of Doubles.
        ''' </summary>
        ''' <param name="InputList"></param>
        ''' <returns>Returns the geometric mean of the vaules in the input list.</returns>
        Public Function GeometricMean(InputList As List(Of Double))

            Dim n As Integer = InputList.Count

            Dim ListProduct As Double = 1
            For i = 0 To InputList.Count - 1
                ListProduct *= InputList(i)
            Next

            Return ListProduct ^ (1 / n)

        End Function

        ''' <summary>
        ''' Returns the geometric mean in an array of Doubles.
        ''' </summary>
        ''' <param name="InputArray"></param>
        ''' <returns>Returns the geometric mean of the vaules in the input array.</returns>
        Public Function GeometricMean(InputArray() As Double, Optional UseSummedLogs As Boolean = False, Optional ByVal IgnoreZeroValues As Boolean = False) As Double

            If UseSummedLogs = False Then

                If IgnoreZeroValues = False Then

                    Dim n As Integer = InputArray.Length

                    Dim ListProduct As Double = 1
                    For i = 0 To InputArray.Length - 1
                        ListProduct *= InputArray(i)
                    Next

                    Return ListProduct ^ (1 / n)

                Else

                    Dim n As Integer = 0

                    Dim ListProduct As Double = 1
                    For i = 0 To InputArray.Length - 1

                        If InputArray(i) = 0 Then Continue For

                        ListProduct *= InputArray(i)
                        n += 1
                    Next

                    Return ListProduct ^ (1 / n)

                End If

            Else

                If IgnoreZeroValues = False Then

                    Dim n As Integer = InputArray.Length

                    Dim SummedLogs As Double = 0
                    For i = 0 To InputArray.Length - 1
                        SummedLogs += System.Math.Log(InputArray(i))
                    Next

                    Return System.Math.Exp(SummedLogs / n)

                Else

                    Dim n As Integer = 0

                    Dim SummedLogs As Double = 0
                    For i = 0 To InputArray.Length - 1

                        If InputArray(i) = 0 Then Continue For

                        SummedLogs += System.Math.Log(InputArray(i))
                        n += 1
                    Next

                    Return System.Math.Exp(SummedLogs / n)

                End If

            End If

        End Function


        ''' <summary>
        ''' Calculates the the coefficient of variation of a set of input values. Also sum, mean, sum of squares, variance and standard deviation can be attained by using the optional parameters.
        ''' </summary>
        ''' <param name="InputListOfDouble"></param>
        ''' <param name="Sum">Upon return of the function, this variable will contain the arithmetric mean.</param>
        ''' <param name="ArithmetricMean">Upon return of the function, this variable will contain the arithmetric mean.</param>
        ''' <param name="SumOfSquares">Upon return of the function, this variable will contain the SumOfSquares.</param>
        ''' <param name="Variance">Upon return of the function, this variable will contain the variance.</param>
        ''' <param name="StandardDeviation">Upon return of the function, this variable will contain the standard deviation.</param>
        ''' <param name="InputValueType">Default calculation type (Population) uses N in the variance calculation denominator. If Sample type is used, the denominator is N-1.</param>
        ''' <returns>Returns the coefficient of variation.</returns>
        Public Function CoefficientOfVariation(ByRef InputListOfDouble As List(Of Double),
                                      Optional ByRef Sum As Double = Nothing,
                                      Optional ByRef ArithmetricMean As Double = Nothing,
                                      Optional ByRef SumOfSquares As Double = Nothing,
                                      Optional ByRef Variance As Double = Nothing,
                                      Optional ByRef StandardDeviation As Double = Nothing,
                                           Optional ByRef InputValueType As StandardDeviationTypes = StandardDeviationTypes.Population) As Double
            Try

                'Notes the number of values in the input list
                Dim n As Integer = InputListOfDouble.Count

                'Calculates the sum of the values in the input list
                Sum = 0
                For i = 0 To InputListOfDouble.Count - 1
                    Sum += InputListOfDouble(i)
                Next

                'Calculates the arithemtric mean of the values in the input list
                ArithmetricMean = Sum / n

                'Calculates the sum of squares of the values in the input list
                SumOfSquares = 0
                For i = 0 To InputListOfDouble.Count - 1
                    SumOfSquares += (InputListOfDouble(i) - ArithmetricMean) ^ 2
                Next

                'Calculates the variance of the values in the input list
                Select Case InputValueType
                    Case StandardDeviationTypes.Population
                        Variance = (1 / (n)) * SumOfSquares
                    Case StandardDeviationTypes.Sample
                        Variance = (1 / (n - 1)) * SumOfSquares
                End Select

                'Calculates, the standard deviation of the values in the input list
                StandardDeviation = System.Math.Sqrt(Variance)

                'Calculates and returns the coefficient of variation
                Return StandardDeviation / ArithmetricMean

            Catch ex As Exception
                Errors("The following exception occured: " & ex.ToString)
                Return Nothing
            End Try

        End Function


        ''' <summary>
        ''' Standardizes the values in the input list.
        ''' </summary>
        ''' <param name="InputValueType">Default calculation type (Population) uses N in the variance calculation denominator. If Sample type is used, the denominator is N-1.</param>
        ''' <param name="SetMeanTo">An optional term that can be used to adjust the mean to a desired value. Can be used to avoid negative values within the distribution.</param>
        ''' <param name="ExcludeNegativeValues">If set to true, all negative values will be ignored when calculating mean and standard deviation. All negative values will be standardized based on the mean and standard deviation of non negative values.</param>
        ''' <param name="InputListOfDouble"></param>
        ''' <param name="SetToZeroOnNoVariance">Sets all values in the input list to zero if no variance is detected.</param>
        Public Sub Standardization(ByRef InputListOfDouble As List(Of Double),
                              Optional ByRef SetMeanTo As Double = 0,
                               Optional ByRef ExcludeNegativeValues As Boolean = False,
                               Optional ByRef InputValueType As StandardDeviationTypes = StandardDeviationTypes.Population,
                               Optional ByVal SetToZeroOnNoVariance As Boolean = True)


            'Notes the number of values in the input list
            'Dim n As Integer = InputListOfDouble.Count

            'Calculates the sum of the values in the input list
            Dim Sum As Double = 0
            Dim n As Integer = 0
            For i = 0 To InputListOfDouble.Count - 1
                If ExcludeNegativeValues = True Then
                    If InputListOfDouble(i) >= 0 Then
                        Sum += InputListOfDouble(i)
                        n += 1
                    End If
                Else
                    Sum += InputListOfDouble(i)
                    n += 1
                End If
            Next

            'Calculates the arithemtric mean of the values in the input list
            Dim ArithmetricMean As Double = Sum / n

            'Calculates the sum of squares of the values in the input list
            Dim SumOfSquares As Double = 0
            Dim n_SumOfSquares As Integer = 0
            For i = 0 To InputListOfDouble.Count - 1
                If ExcludeNegativeValues = True Then
                    If InputListOfDouble(i) >= 0 Then
                        SumOfSquares += (InputListOfDouble(i) - ArithmetricMean) ^ 2
                    End If
                Else
                    SumOfSquares += (InputListOfDouble(i) - ArithmetricMean) ^ 2
                End If
            Next

            'Calculates the variance of the values in the input list
            Dim Variance As Double
            Select Case InputValueType
                Case StandardDeviationTypes.Population
                    Variance = (1 / (n)) * SumOfSquares
                Case StandardDeviationTypes.Sample
                    Variance = (1 / (n - 1)) * SumOfSquares
            End Select

            'Setting to zero on no variance, then exits sub (The reason this is needed is that we will get the square root of 0 in the next step if variance is 0.)
            If Variance = 0 Then
                If SetToZeroOnNoVariance = True Then
                    For n = 0 To InputListOfDouble.Count - 1
                        InputListOfDouble(n) = 0
                    Next
                    Exit Sub
                End If
            End If

            'Calculates, the standard deviation of the values in the input list
            Dim StandardDeviation As Double = System.Math.Sqrt(Variance)

            'Standardizes the values in the input list
            For n = 0 To InputListOfDouble.Count - 1
                InputListOfDouble(n) = ((InputListOfDouble(n) - ArithmetricMean) / StandardDeviation) + SetMeanTo
            Next

        End Sub


        Public Enum StandardDeviationTypes
            Population
            Sample
        End Enum


        ''' <summary>
        ''' Calculates the Pearson correlation coefficient between two arrays of the same length
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns></returns>
        Public Function PearsonsCorrelation(ByVal x As Single(), ByVal y As Single()) As Double

            If x.Length <> y.Length Then Throw New ArgumentException("Input arrays must have the same length.")

            'Checking if one or both of the arrays are all zeroes. If both are all zeroes they are considerred to be fully correlated,
            'If only one is all zeroes, then they are considerred not at all correlated and returns 0
            'Actually also returning if the values are extremely small (so that multiplication does not work due to too low decimal accuracy)
            If (x.Max < 10 ^ -10 And x.Min > -10 ^ -10) And (y.Max < 10 ^ -10 And y.Min > -10 ^ -10) Then
                Return 1
            ElseIf (x.Max < 10 ^ -10 And x.Min > -10 ^ -10) Or (y.Max < 10 ^ -10 And y.Min > -10 ^ -10) Then
                Return 0
            End If

            'If (x.Max = 0 And x.Min = 0) And (y.Max = 0 And y.Min = 0) Then
            'Return 1
            'ElseIf (x.Max = 0 And x.Min = 0) Or (y.Max = 0 And y.Min = 0)
            'Return 0
            'End If

            Dim n As Integer = x.Length
            Dim SumXY As Double = 0
            For i = 0 To n - 1
                SumXY += x(i) * y(i)
            Next

            Dim SumXSq As Double = 0
            For i = 0 To n - 1
                SumXSq += x(i) * x(i)
            Next

            Dim SumYSq As Double = 0
            For i = 0 To n - 1
                SumYSq += y(i) * y(i)
            Next

            'Dim Xsum As Double = x.Sum
            'Dim Ysum As Double = y.Sum

            'Dim sx As Double = Math.Sqrt(n * SumXSq - (x.Sum * x.Sum))
            'Dim sy As Double = Math.Sqrt(n * SumYSq - (y.Sum * y.Sum))
            'Dim r As Double = (n * SumXY - x.Sum * y.Sum) / (sx * sy)

            'Dim A As Double = (SumXY - ((x.Sum * y.Sum) / n))
            'Dim B As Double = Math.Sqrt((SumXSq - ((x.Sum * x.Sum) / n)) * (SumYSq - ((y.Sum * y.Sum) / n)))
            'Dim r2 As Double = A / B

            Dim r As Double = (SumXY - ((x.Sum * y.Sum) / n)) /
        System.Math.Sqrt((SumXSq - ((x.Sum * x.Sum) / n)) * (SumYSq - ((y.Sum * y.Sum) / n)))
            Return r


        End Function


#Region "Distance"

        Public Function GetEuclideanDistance(ByRef Array1 As Single(), ByRef Array2 As Single()) As Double

            'Checking that the arrays have the same lengths
            If Array1.Length <> Array2.Length Then Throw New ArgumentException("Input arrays must have the same length.")

            Dim Sum As Double = 0
            For n = 0 To Array1.Length - 1
                Sum += (Array1(n) - Array2(n)) ^ 2
            Next

            Return System.Math.Sqrt(Sum)

        End Function

        Public Function GetEuclideanDistance(ByRef Array1 As Double(), ByRef Array2 As Double()) As Double

            'Checking that the arrays have the same lengths
            If Array1.Length <> Array2.Length Then Throw New ArgumentException("Input arrays must have the same length.")

            Dim Sum As Double = 0
            For n = 0 To Array1.Length - 1
                Sum += (Array1(n) - Array2(n)) ^ 2
            Next

            Return System.Math.Sqrt(Sum)

        End Function


        ''' <summary>
        ''' Calculates the Euclidian distance of two arrays. However, differences within dimensions are limited to MaximumDifferenceThreshold.
        ''' </summary>
        ''' <param name="Array1"></param>
        ''' <param name="Array2"></param>
        ''' <param name="MaximumDifferenceThreshold"></param>
        ''' <returns></returns>
        Public Function GetEuclideanDistance(ByRef Array1 As Single(), ByRef Array2 As Single(), ByVal MaximumDifferenceThreshold As Single) As Double

            'Checking that the arrays have the same lengths
            If Array1.Length <> Array2.Length Then Throw New ArgumentException("Input arrays must have the same length.")

            Dim Sum As Double = 0
            For n = 0 To Array1.Length - 1
                Sum += (System.Math.Min(MaximumDifferenceThreshold, Array1(n) - Array2(n))) ^ 2
            Next

            Return System.Math.Sqrt(Sum)

        End Function

        ''' <summary>
        ''' Calculates the Euclidian distance of two arrays. However, differences within dimensions are limited to MaximumDifferenceThreshold.
        ''' </summary>
        ''' <param name="Array1"></param>
        ''' <param name="Array2"></param>
        ''' <param name="MaximumDifferenceThreshold"></param>
        ''' <returns></returns>
        Public Function GetEuclideanDistance(ByRef Array1 As Double(), ByRef Array2 As Double(), ByVal MaximumDifferenceThreshold As Single) As Double

            'Checking that the arrays have the same lengths
            If Array1.Length <> Array2.Length Then Throw New ArgumentException("Input arrays must have the same length.")

            Dim Sum As Double = 0
            For n = 0 To Array1.Length - 1
                Sum += (System.Math.Min(MaximumDifferenceThreshold, Array1(n) - Array2(n))) ^ 2
            Next

            Return System.Math.Sqrt(Sum)

        End Function

        Public Function GetManhattanDistance(ByRef Array1 As Single(), ByRef Array2 As Single()) As Double

            'Checking that the arrays have the same lengths
            If Array1.Length <> Array2.Length Then Throw New ArgumentException("Input arrays must have the same length.")

            'Subtracting the arrays
            Dim SubtractionArray(Array1.Length - 1) As Single
            For k = 0 To SubtractionArray.Length - 1
                SubtractionArray(k) = System.Math.Abs(Array1(k) - Array2(k))
            Next

            Dim TotalSubtractionValue As Double = SubtractionArray.Average
            Return TotalSubtractionValue

        End Function

        Public Function GetManhattanDistance(ByRef Array1 As Double(), ByRef Array2 As Double()) As Double

            'Checking that the arrays have the same lengths
            If Array1.Length <> Array2.Length Then Throw New ArgumentException("Input arrays must have the same length.")

            'Subtracting the arrays
            Dim SubtractionArray(Array1.Length - 1) As Double
            For k = 0 To SubtractionArray.Length - 1
                SubtractionArray(k) = System.Math.Abs(Array1(k) - Array2(k))
            Next

            Dim TotalSubtractionValue As Double = SubtractionArray.Average
            Return TotalSubtractionValue

        End Function

#End Region


        ''' <summary>
        ''' Multiplies the values in the input array with a common factor so that the value farthest from zero becomes 1 (or NormalizeMaxvalueTo). If all values are 0, the array will be left unalterred.
        ''' </summary>
        ''' <param name="InputListOfDouble"></param>
        ''' <param name="NormalizeMaxvalueTo">Optional. This parameter can be use to normalize the array to another value than 1.</param>
        ''' <param name="DetectedMaxValue">Upon return, this value will contain the input value farthest from zero.</param>
        ''' <param name="NormalizationFactor">Upon return, this value will contain the value by which all other values have been multiplied in the normalization process.</param>
        Public Sub Normalize(ByRef InputListOfDouble As List(Of Double),
                              Optional ByRef NormalizeMaxvalueTo As Double = 1,
                              Optional ByRef DetectedMaxValue As Double = Nothing,
                              Optional ByRef NormalizationFactor As Double? = Nothing)

            'Getting the value farthest from zero
            DetectedMaxValue = InputListOfDouble.Max
            If System.Math.Abs(InputListOfDouble.Min) > System.Math.Abs(DetectedMaxValue) Then DetectedMaxValue = InputListOfDouble.Min

            'Calculating the normalization factor
            NormalizationFactor = 1 / DetectedMaxValue

            'Doing the normalization
            For Each Value In InputListOfDouble
                Value = Value * NormalizationFactor
            Next

        End Sub


        ''' <summary>
        ''' Calculats the bark filter band width at a specified centre frequency, based on Zwicker and Fastl(1999), Phsycho-acoustics, p 164
        ''' </summary>
        ''' <param name="CentreFrequency"></param>
        ''' <returns></returns>
        Public Function CenterFrequencyToBarkFilterBandwidth(ByVal CentreFrequency As Double)

            Return 25 + 75 * (1 + 1.4 * (CentreFrequency / 1000) ^ 2) ^ 0.69

        End Function

        ''' <summary>
        ''' Performs linear interpolation to get a value for either X or Y. If A value for X is needed, InputX should be set to Nothing and a value for InputY should be supplied.
        ''' Reversely, if A value for Y is needed, InputY should be set to Nothing and a value for InputX should be supplied. X1, Y1, X2 and Y2 are the points between which the interpolation takes place.
        ''' </summary>
        ''' <param name="InputValue"></param>
        ''' <param name="X1"></param>
        ''' <param name="Y1"></param>
        ''' <param name="X2"></param>
        ''' <param name="Y2"></param>
        ''' <param name="GetY">If set to true, the input value is assumed to be an x, and hence an interpolated y is returned. 
        ''' If set to False, the input value is assumed to be an y, and hence an interpolated x is returned.</param>
        ''' <returns></returns>
        Public Function LinearInterpolation(ByRef InputValue As Double,
                                        ByVal X1 As Double, ByVal Y1 As Double, ByVal X2 As Double, ByVal Y2 As Double,
                                        ByVal GetY As Boolean) As Double

            'Getting the linear function that fit to the points 1 and 2
            'y = kx + m
            Dim k As Double = (Y1 - Y2) / (X1 - X2)
            If k = Double.NaN Then Throw New ArgumentException("Not possible to interpolate from the input values.")
            Dim m As Double = Y1 - k * X1

            If GetY = True Then

                'Returning y
                Return k * InputValue + m

            Else

                'Returning x
                Return (InputValue - m) / k

            End If

        End Function

        ''' <summary>
        ''' Detects the AvailableValues array indices that have the first values nearest to the InputValue. (N.B. The data in AvailableValues need to be orderred in either ascending or descending order.) 
        ''' If the input value exists in AvailableValues, NearestLowerIndex and NearestHigherIndex will have the same value (which may be tested for). If the input value is higher than the highest value
        ''' in AvailableValues, NearestHigherIndex will be Nothing. And if input value is lower than the lowest value in AvailableValues, NearestLowerIndex will be Nothing.
        ''' </summary>
        ''' <param name="InputValue"></param>
        ''' <param name="AvailableValues"></param>
        ''' <returns></returns>
        Public Function GetNearestIndices(ByVal InputValue As Double, ByRef AvailableValues As Double()) As NearestIndices

            Dim Output As New NearestIndices

            'Checking if the data is in ascending or descending order
            If AvailableValues(0) < AvailableValues(AvailableValues.Length - 1) Then

                'Assuming ascending order
                For n = 0 To AvailableValues.Length - 1

                    If InputValue < AvailableValues(n) Then

                        If n > 0 Then
                            If InputValue = AvailableValues(n - 1) Then
                                Output.NearestLowerIndex = n - 1
                                Output.NearestHigherIndex = n - 1
                                Return Output
                            End If
                        End If

                        If n = 0 Then
                            Output.NearestLowerIndex = Nothing
                            Output.NearestHigherIndex = n
                            Return Output

                        End If

                        Output.NearestHigherIndex = n
                        Output.NearestLowerIndex = n - 1
                        Return Output

                    End If
                Next

                If AvailableValues.Length > 0 Then
                    If InputValue = AvailableValues(AvailableValues.Length - 1) Then
                        Output.NearestLowerIndex = AvailableValues.Length - 1
                        Output.NearestHigherIndex = AvailableValues.Length - 1
                        Return Output
                    End If
                End If

                Output.NearestLowerIndex = AvailableValues.Length - 1
                Output.NearestHigherIndex = Nothing
                Return Output

            Else
                'Assuming descending order
                Throw New NotImplementedException


            End If


        End Function

        Public Class NearestIndices
            Public NearestLowerIndex As Integer? = Nothing
            Public NearestHigherIndex As Integer? = Nothing
        End Class

        ''' <summary>
        ''' Detects the AvailableValues array index that have the values nearest to the InputValue.
        ''' </summary>
        ''' <param name="InputValue"></param>
        ''' <param name="AvailableValues"></param>
        ''' <returns></returns>
        Public Function GetNearestIndex(ByVal InputValue As Double, ByRef AvailableValues As SortedSet(Of Double),
                                    Optional ByRef MidpointUpwardsRounding As Boolean = True) As Integer

            Dim TempValues As Double() = AvailableValues.ToArray

            For n = 0 To TempValues.Count - 1

                If InputValue < TempValues(n) Then

                    If n > 0 Then
                        If InputValue = TempValues(n - 1) Then
                            'The value exists in the array, returns it's index
                            Return n - 1
                        End If
                    End If

                    If n = 0 Then 'Or n = TempValues.Length - 1 Then
                        Return n
                    Else

                        'Calculating which of the two closest values is nearest to the input value, and returning its index
                        Dim LowerValue = TempValues(n - 1)
                        Dim HigherValue = TempValues(n)

                        Dim DistToLowerValue = System.Math.Abs(InputValue - LowerValue)
                        Dim DistToHigherValue = System.Math.Abs(InputValue - HigherValue)

                        If MidpointUpwardsRounding = True Then
                            If DistToLowerValue < DistToHigherValue Then
                                Return n - 1
                            Else
                                Return n
                            End If
                        Else
                            If DistToHigherValue < DistToLowerValue Then
                                Return n
                            Else
                                Return n - 1
                            End If
                        End If
                    End If
                End If
            Next

            'Returns the last index if all values were smaller than the input value
            Return TempValues.Length - 1

        End Function

        Public Enum VericalDirections
            Descending
            Ascending
        End Enum

        Public Function FirstCaseInterpolation(ByRef InputY As Double, ByRef InterPolationList As SortedList(Of Double, Double),
                                           Optional ByVal SearchDirection As VericalDirections = VericalDirections.Descending) As Double

            'Skriv en funktion som kontrollerar punktpar för punktpar om det sökta värdet finns mellan, och interpolerar baserat på fösta påträffade fallet, med start från högsta värdet och nedåt, 

            Dim LocalInterplationList As New List(Of Tuple(Of Double, Double))
            For Each Point In InterPolationList
                LocalInterplationList.Add(New Tuple(Of Double, Double)(Point.Key, Point.Value))
            Next

            'Returning the end point values if InputY are outside the boundaries of the InterPolationList
            If InputY >= LocalInterplationList(LocalInterplationList.Count - 1).Item2 Then Return LocalInterplationList(LocalInterplationList.Count - 1).Item1
            If InputY <= LocalInterplationList(0).Item2 Then Return LocalInterplationList(0).Item1

            If SearchDirection = VericalDirections.Descending Then

                'Swapping the direction of points
                Dim Temp_LocalInterplationList As New List(Of Tuple(Of Double, Double))
                For n = 1 To LocalInterplationList.Count
                    Temp_LocalInterplationList.Add(LocalInterplationList(LocalInterplationList.Count - n))
                Next
                LocalInterplationList = Temp_LocalInterplationList

                For n = 1 To LocalInterplationList.Count - 1

                    'Checking if the n point is lower than InputY
                    If InputY >= LocalInterplationList(n).Item2 Then

                        'Goes on to interpolate between the points n and n-1
                        Return LinearInterpolation(InputY,
                                               LocalInterplationList(n - 1).Item1, LocalInterplationList(n - 1).Item2,
                                               LocalInterplationList(n).Item1, LocalInterplationList(n).Item2,
                                               False)
                    End If
                Next

                'Returns the minimum value if nothing else has been returned above (actually not necessary since this will have been returned above)
                Return LocalInterplationList(LocalInterplationList.Count - 1).Item2

            ElseIf SearchDirection = VericalDirections.Ascending Then

                For n = 1 To LocalInterplationList.Count - 1

                    'Checking if the n point is lower than InputY
                    If InputY <= LocalInterplationList(n).Item2 Then

                        'Goes on to interpolate between the points n and n-1
                        Return LinearInterpolation(InputY,
                                               LocalInterplationList(n - 1).Item1, LocalInterplationList(n - 1).Item2,
                                               LocalInterplationList(n).Item1, LocalInterplationList(n).Item2,
                                               False)
                    End If
                Next

                'Returns the maximum value if nothing else has been returned above (actually not necessary since this will have been returned above)
                Return LocalInterplationList(LocalInterplationList.Count - 1).Item2

            Else
                Throw New NotImplementedException
            End If

        End Function


        ''' <summary>
        ''' Interpolates a value for X using the input function and the input Y.
        ''' </summary>
        ''' <param name="InputY"></param>
        ''' <param name="InterPolationList">A sorted list of sets of X, Y values.</param>
        ''' <returns></returns>
        Public Function LinearInterpolation(ByRef InputY As Double, ByRef InterPolationList As SortedList(Of Double, Double),
                                        Optional SendInfoToLogWhenOutsideInterpolationListValues As Boolean = False) As Double

            'Returns the X if its Y is in the list
            If InterPolationList.ContainsValue(InputY) Then
                Return InterPolationList.Keys.ToArray(InterPolationList.IndexOfValue(InputY))
            End If

            'Interpolates X

            'Getting the indices closest to the input value
            Dim Y = GetNearestIndices(InputY, InterPolationList.Values.ToArray)
            If Y.NearestLowerIndex Is Nothing Then
                'Returning the lowest value in the list
                If SendInfoToLogWhenOutsideInterpolationListValues = True Then Utils.SendInfoToLog("Input value below Interpolation list values!")

                Return InterPolationList.Keys.ToArray(Y.NearestHigherIndex)
            End If
            If Y.NearestHigherIndex Is Nothing Then
                'Returning the highest value in the list

                If SendInfoToLogWhenOutsideInterpolationListValues = True Then Utils.SendInfoToLog("Input value below Interpolation list values!")

                Return InterPolationList.Keys.ToArray(Y.NearestLowerIndex)
            End If

            'Should be unneccesary
            If Y.NearestLowerIndex = Y.NearestHigherIndex Then
                Return InterPolationList.Keys.ToArray(Y.NearestLowerIndex)
            End If

            Return LinearInterpolation(InputY, InterPolationList.Keys.ToArray(Y.NearestLowerIndex), InterPolationList.Values.ToArray(Y.NearestLowerIndex),
                                           InterPolationList.Keys.ToArray(Y.NearestHigherIndex), InterPolationList.Values.ToArray(Y.NearestHigherIndex), False)

        End Function


        ''' <summary>
        ''' Interpolates a value for Y using the input function and the input X.
        ''' </summary>
        ''' <param name="InputX"></param>
        ''' <param name="InterPolationList">A sorted list of sets of X, Y values.</param>
        ''' <returns></returns>
        Public Function LinearInterpolation_GetY(ByRef InputX As Double, ByRef InterPolationList As SortedList(Of Double, Double),
                                        Optional SendInfoToLogWhenOutsideInterpolationListValues As Boolean = False) As Double

            'Returns the Y if its X is in the list
            If InterPolationList.ContainsKey(InputX) Then
                Return InterPolationList.Values.ToArray(InterPolationList.IndexOfKey(InputX))
            End If

            'Interpolates Y

            'Getting the indices closest to the input value
            Dim X = GetNearestIndices(InputX, InterPolationList.Keys.ToArray)
            If X.NearestLowerIndex Is Nothing Then
                'Returning the lowest value in the list
                If SendInfoToLogWhenOutsideInterpolationListValues = True Then Utils.SendInfoToLog("Input value below Interpolation list values!")

                Return InterPolationList.Values.ToArray(X.NearestHigherIndex)
            End If
            If X.NearestHigherIndex Is Nothing Then
                'Returning the highest value in the list

                If SendInfoToLogWhenOutsideInterpolationListValues = True Then Utils.SendInfoToLog("Input value below Interpolation list values!")

                Return InterPolationList.Values.ToArray(X.NearestLowerIndex)
            End If

            'Should be unneccesary
            If X.NearestLowerIndex = X.NearestHigherIndex Then
                Return InterPolationList.Values.ToArray(X.NearestLowerIndex)
            End If

            Return LinearInterpolation(InputX, InterPolationList.Keys.ToArray(X.NearestLowerIndex), InterPolationList.Values.ToArray(X.NearestLowerIndex),
                                           InterPolationList.Keys.ToArray(X.NearestHigherIndex), InterPolationList.Values.ToArray(X.NearestHigherIndex), True)

        End Function


        ''' <summary>
        ''' Checks if the InputData is either monotonically rising or falling.
        ''' </summary>
        ''' <param name="InputData"></param>
        ''' <returns></returns>
        Public Function IsMonotonic(ByRef InputData() As Double) As Boolean

            'Checks if we have a rising, falling or flat function

            If InputData(0) = InputData(InputData.Length - 1) Then
                'Assuming flat function, and requireing all data points to be equal

                'Checking that all data points are equal
                For n = 0 To InputData.Length - 2
                    If InputData(n) <> InputData(n + 1) Then Return False
                Next

            ElseIf InputData(0) < InputData(InputData.Length - 1) Then
                'Assuming rising function

                'Checking that adjacent data points are not falling (allows them to either be equal or rising)
                For n = 0 To InputData.Length - 2
                    If InputData(n) > InputData(n + 1) Then Return False
                Next

            Else
                'Assuming falling function

                'Checking that adjacent data points are not rising (allows them to either be equal or falling)
                For n = 0 To InputData.Length - 2
                    If InputData(n) < InputData(n + 1) Then Return False
                Next

            End If

            'Returns true, if not returned false by any of the above checks
            Return True

        End Function

        Public Function GetAverageOfSections(Input() As Double, SectionLength As Integer, StepSize As Integer) As Double()

            'Returning the average of the input if the input array is shorter that the section length
            If Input.Length < SectionLength Then
                Return {Input.Average}
            End If

            Dim OutputLength As Integer = Int((Input.Length - SectionLength) / StepSize) + 1
            Dim Output(OutputLength - 1) As Double

            Dim AverageArray(SectionLength - 1) As Double
            For n = 0 To Output.Length - 1

                For i = 0 To AverageArray.Length - 1
                    AverageArray(i) = Input(n * StepSize + i)
                Next
                Output(n) = AverageArray.Average
            Next

            Return Output

        End Function

        Public Class DynamicTimeWarping


            Public Function RunDynamicTimeWarping(ByRef X As Double(), ByRef Y As Double(),
                                              Optional ByVal NormalizationType As DtwNormalizationTypes = DtwNormalizationTypes.StepCountNormalization,
                                              Optional ByVal GlobalRestriction As GlobalRestrictionTypes = GlobalRestrictionTypes.Diamond,
                                              Optional ByVal RepeatedWarpsAllowed As Integer = 1,
                                              Optional ByVal DiagonalWeight As Double = 1, ' 1 is used instead of 2 here, since doubling the cost biases the selected path through the distance matrix to deviate away from the centre/diagonal, which in my opinion should be undesirable.
                                              Optional ByVal HorizontalWeight As Double = 1,
                                              Optional ByVal VerticalWeight As Double = 1,
                                              Optional ByVal LogDetails As Boolean = True,
                                              Optional ByVal LogFolder As String = "",
                                              Optional ByVal ExportID As String = "dtw") As DtwResult

                If LogFolder = "" Then LogFolder = Utils.logFilePath

                Try

                    Dim Output As New DtwResult


                    'Setting up a distance matrix containing, the size if X * Y
                    Dim ColumnCount As Integer = X.Count
                    Dim RowCount As Integer = Y.Count

                    'Setting up a dtw matrix, the size if X * Y
                    '(The first window is only used for initial comparison (as if it repressented data prior to the first window), whereby the matrix need to be...)
                    Dim DtwMatrix(ColumnCount - 1, RowCount - 1) As DtwPoint

                    'Measuring distances between time windows. 
                    For Column_x = 0 To ColumnCount - 1
                        For Row_y = 0 To RowCount - 1

                            DtwMatrix(Column_x, Row_y) = New DtwPoint(Column_x, Row_y, 0)

                            If GlobalRestriction <> GlobalRestrictionTypes.NoLocalRestrictions Then
                                'Blocking windows are marked by TransitionDirections.BlockedPoint
                                If IsWithinMeasurementRegion(ColumnCount - 1, Column_x, RowCount - 1, Row_y, GlobalRestriction) = False Then
                                    DtwMatrix(Column_x, Row_y).TransitionDirection = TransitionDirections.BlockedPoint
                                    Continue For
                                End If
                            End If

                            'Getting the distance 
                            DtwMatrix(Column_x, Row_y).X_Y_Distance = System.Math.Abs(X(Column_x) - Y(Row_y))

                        Next
                    Next


                    'Calculating symmetric type 0 DTW (For reference, see Sakoe and Chiba 1978, Optimization for spoken word recognition). Adapted with a restriction that warping is only allowed if the previous value wasn't created by a warping
                    For Column_x = 0 To ColumnCount - 1
                        For Row_y = 0 To RowCount - 1

                            'Skipping measurements outside the measurement window
                            If DtwMatrix(Column_x, Row_y).TransitionDirection = TransitionDirections.BlockedPoint Then
                                Continue For
                            End If

                            'Declaring blockers
                            Dim BlockHorizontalMove As Boolean = False
                            Dim BlockVerticalMove As Boolean = False

                            'Blocking moves from anything to the left of the first column
                            If Column_x = 0 Then
                                BlockHorizontalMove = True
                            End If

                            'Blocking moves from anything above the first row
                            If Row_y = 0 Then
                                BlockVerticalMove = True
                            End If


                            'Enforcing restrictions concerning the curvature of the selected path.
                            If RepeatedWarpsAllowed > 0 Then

                                'Direction restrictions, blocks the horizontal or vertical directions if any of the RepeatedWarpsAllowed preceding points in the selected path prior to 
                                'the horizontally or vertically preceding cell is it self derived from a horizontal or vertical direction
                                'This means that repeated warps in the same direction is allowed every RepeatedWarpsAllowed move in the selected path.

                                If Column_x > 0 Then
                                    If DtwMatrix(Column_x - 1, Row_y).History.Count >= DtwMatrix(Column_x - 1, Row_y).History.Count - (RepeatedWarpsAllowed) And
                                    DtwMatrix(Column_x - 1, Row_y).History.Count - (RepeatedWarpsAllowed) > -1 Then

                                        BlockHorizontalMove = True
                                        For h = 1 To RepeatedWarpsAllowed
                                            If DtwMatrix(Column_x - 1, Row_y).History(DtwMatrix(Column_x - 1, Row_y).History.Count - (h)).TransitionDirection <> TransitionDirections.Horizontal Then
                                                BlockHorizontalMove = False
                                                Exit For
                                            End If
                                        Next

                                    End If
                                End If

                                If Row_y > 0 Then
                                    If DtwMatrix(Column_x, Row_y - 1).History.Count >= DtwMatrix(Column_x, Row_y - 1).History.Count - (RepeatedWarpsAllowed) And
                                    DtwMatrix(Column_x, Row_y - 1).History.Count - (RepeatedWarpsAllowed) > -1 Then

                                        BlockVerticalMove = True
                                        For h = 1 To RepeatedWarpsAllowed
                                            If DtwMatrix(Column_x, Row_y - 1).History(DtwMatrix(Column_x, Row_y - 1).History.Count - (RepeatedWarpsAllowed)).TransitionDirection <> TransitionDirections.Vertical Then
                                                BlockVerticalMove = False
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If

                            End If



                            'Getting the possible transition sums
                            Dim DiagonalPathValue As Double = Double.PositiveInfinity
                            If Column_x > 0 And Row_y > 0 Then
                                If Not DtwMatrix(Column_x - 1, Row_y - 1).TransitionDirection = TransitionDirections.BlockedPoint Then
                                    DiagonalPathValue = DtwMatrix(Column_x - 1, Row_y - 1).AccumulatedDistance + DiagonalWeight * DtwMatrix(Column_x, Row_y).X_Y_Distance
                                End If
                            End If

                            Dim VerticalPathValue As Double = Double.PositiveInfinity
                            If Row_y > 0 Then
                                If Not DtwMatrix(Column_x, Row_y - 1).TransitionDirection = TransitionDirections.BlockedPoint Then
                                    VerticalPathValue = DtwMatrix(Column_x, Row_y - 1).AccumulatedDistance + VerticalWeight * DtwMatrix(Column_x, Row_y).X_Y_Distance
                                End If
                            End If

                            Dim HorizintalPathValue As Double = Double.PositiveInfinity
                            If Column_x > 0 Then
                                If Not DtwMatrix(Column_x - 1, Row_y).TransitionDirection = TransitionDirections.BlockedPoint Then
                                    HorizintalPathValue = DtwMatrix(Column_x - 1, Row_y).AccumulatedDistance + HorizontalWeight * DtwMatrix(Column_x, Row_y).X_Y_Distance
                                End If
                            End If

                            'Selecting the best path
                            If Column_x = 0 And Row_y = 0 Then
                                'The special case of position 0,0
                                'No move at all, just adding the distance value
                                DtwMatrix(Column_x, Row_y).AccumulatedDistance = DiagonalWeight * DtwMatrix(Column_x, Row_y).X_Y_Distance
                                DtwMatrix(Column_x, Row_y).TransitionDirection = TransitionDirections.NoDirection

                            ElseIf (DiagonalPathValue < VerticalPathValue And DiagonalPathValue < HorizintalPathValue) Or
                                    (BlockVerticalMove = True And DiagonalPathValue < HorizintalPathValue) Or
                                    (BlockHorizontalMove = True And DiagonalPathValue < VerticalPathValue) Or
                                    (BlockVerticalMove = True And BlockHorizontalMove = True And DiagonalPathValue < Double.PositiveInfinity) Or
                                    (DiagonalPathValue = 0 Or (DiagonalPathValue = VerticalPathValue And DiagonalPathValue = HorizintalPathValue)) Then 'N.B. the preferred path is always diagonal if the DiagonalPathValue = 0, or if all path values are equal (which would mean that we're in a silent window.)

                                'A diagonal move
                                DiagonalDtwMove(DtwMatrix, Column_x, Row_y, DiagonalPathValue)

                            ElseIf BlockVerticalMove = False And
                                (VerticalPathValue < HorizintalPathValue Or (BlockHorizontalMove = True And VerticalPathValue < Double.PositiveInfinity)) Then

                                'A vertical move
                                VerticalDtwMove(DtwMatrix, Column_x, Row_y, VerticalPathValue)

                            ElseIf BlockHorizontalMove = False And
                                (HorizintalPathValue < VerticalPathValue Or (BlockVerticalMove = True And HorizintalPathValue < Double.PositiveInfinity)) Then

                                'A horizontal move
                                HorizontalDtwMove(DtwMatrix, Column_x, Row_y, HorizintalPathValue)

                            ElseIf BlockHorizontalMove = False And BlockVerticalMove = False And HorizintalPathValue = VerticalPathValue Then

                                'Horizontal and vertical moves are equally good and none is blocked. Selecting a move in the direction of the longest array, or a random path if the arrays are of equal length.

                                If ColumnCount > RowCount Then
                                    'A horizontal move
                                    HorizontalDtwMove(DtwMatrix, Column_x, Row_y, HorizintalPathValue)

                                ElseIf RowCount > ColumnCount Then
                                    'A vertical move
                                    VerticalDtwMove(DtwMatrix, Column_x, Row_y, VerticalPathValue)

                                Else
                                    'Selecting a random path
                                    Dim rnd As New Random
                                    If rnd.Next(CInt(0), CInt(2)) = 0 Then
                                        'A horizontal move
                                        HorizontalDtwMove(DtwMatrix, Column_x, Row_y, HorizintalPathValue)
                                    Else
                                        'A vertical move
                                        VerticalDtwMove(DtwMatrix, Column_x, Row_y, VerticalPathValue)
                                    End If

                                End If

                                'Overriding blockings
                            ElseIf DiagonalPathValue = Double.PositiveInfinity And BlockVerticalMove = True And HorizintalPathValue = Double.PositiveInfinity Then

                                'Overriding the vertical block since it is the only available value (this happens when conflicts occur between area and direction blockings)
                                'A vertical move
                                VerticalDtwMove(DtwMatrix, Column_x, Row_y, VerticalPathValue)

                            ElseIf DiagonalPathValue = Double.PositiveInfinity And VerticalPathValue = Double.PositiveInfinity And BlockHorizontalMove = True Then

                                'Overriding the horizontal block since it is the only available value (this happens when conflicts occur between area and direction blockings)
                                'A horizontal move
                                HorizontalDtwMove(DtwMatrix, Column_x, Row_y, HorizintalPathValue)

                            ElseIf (BlockHorizontalMove = True And BlockVerticalMove = True And HorizintalPathValue = VerticalPathValue) Or
                            (HorizintalPathValue = Double.PositiveInfinity And VerticalPathValue = Double.PositiveInfinity) Then

                                'These could possibly be included in the first ElseIf conditions above?

                                'Horizontal and vertical moves are equally good (or bad) but both are blocked. Or, both are outside the measurement region.
                                'Forcing a diagonal move
                                DiagonalDtwMove(DtwMatrix, Column_x, Row_y, DiagonalPathValue)

                            Else
                                MsgBox("Something Is wrong!")

                            End If

                            'Adding the current cell to it's own history
                            DtwMatrix(Column_x, Row_y).History.Add(DtwMatrix(Column_x, Row_y))

                        Next
                    Next

                    'Storing the winner path
                    Output.SelectedPath = DtwMatrix(ColumnCount - 1, RowCount - 1).History

                    'Calculating the average unweighted distance (should be unnecessary?)
                    Dim AvDistList As New List(Of Double)
                    For Each Point In Output.SelectedPath
                        AvDistList.Add(Point.X_Y_Distance)
                    Next
                    Output.AverageUnweightedDistance = AvDistList.Average


                    If LogDetails = True Then

                        'Exporting the history of the "winner" in list form
                        Dim HistoryOutputList As New SortedSet(Of String)
                        For Each CurrentPTWPoint In DtwMatrix(ColumnCount - 1, RowCount - 1).History
                            HistoryOutputList.Add(CurrentPTWPoint.Column_x & ", " & CurrentPTWPoint.Row_y)
                        Next
                        Utils.SendInfoToLog("Dynamic time warping results, indices of the selected winner path" & vbCrLf & String.Join(vbCrLf, HistoryOutputList), ExportID & "_WinnerHistoryList", LogFolder)

                        'Exporting the distance matrix
                        Dim DistanceMatrixOutputList As New List(Of String)
                        For Row_j = 0 To DtwMatrix.GetUpperBound(1)
                            Dim CurrentRow As String = ""
                            For Column_i = 0 To DtwMatrix.GetUpperBound(0)
                                CurrentRow &= DtwMatrix(Column_i, Row_j).X_Y_Distance & vbTab
                            Next
                            DistanceMatrixOutputList.Add(CurrentRow)
                        Next
                        Utils.SendInfoToLog("Dynamic time warping results, distance matrix" & vbCrLf & String.Join(vbCrLf, DistanceMatrixOutputList), ExportID & "_DistanceMatrix", LogFolder)


                        'Exporting the dtw matrix
                        Dim DtwMatrixOutputList As New List(Of String)
                        For Row_j = 0 To DtwMatrix.GetUpperBound(1)
                            Dim CurrentRow As String = ""
                            For Column_i = 0 To DtwMatrix.GetUpperBound(0)
                                CurrentRow &= DtwMatrix(Column_i, Row_j).AccumulatedDistance & vbTab
                            Next
                            DtwMatrixOutputList.Add(CurrentRow)
                        Next
                        Utils.SendInfoToLog("Dynamic time warping results, accumulated distances." & vbCrLf & String.Join(vbCrLf, DtwMatrixOutputList), ExportID & "_AccumulatedDistanceMatrix", LogFolder)

                        'Exporting the dtw matrix with the winner path marked
                        Dim DtwMatrixWinnerMarkedOutputList As New List(Of String)
                        For Row_j = 0 To DtwMatrix.GetUpperBound(1)
                            Dim CurrentRow As String = ""
                            For Column_i = 0 To DtwMatrix.GetUpperBound(0)

                                If HistoryOutputList.Contains(Column_i & ", " & Row_j) Then
                                    'Putting the items in the winner path in brackets
                                    CurrentRow &= "[" & DtwMatrix(Column_i, Row_j).AccumulatedDistance & "]" & vbTab
                                Else
                                    CurrentRow &= DtwMatrix(Column_i, Row_j).AccumulatedDistance & vbTab
                                End If

                            Next
                            DtwMatrixWinnerMarkedOutputList.Add(CurrentRow)
                        Next
                        Utils.SendInfoToLog("Dynamic time warping results, with winner path marked." & vbCrLf & String.Join(vbCrLf, DtwMatrixWinnerMarkedOutputList), ExportID & "_DtwMatrixWithWinnerPath", LogFolder)

                    End If


                    Select Case NormalizationType
                        Case DtwNormalizationTypes.NoNormalization
                            Output.Distance = DtwMatrix(ColumnCount - 1, RowCount - 1).AccumulatedDistance

                        Case DtwNormalizationTypes.XYNormalization

                            'This is the type of normalization of White and Neely, as reported in Sakoe and Chiba 1978, Optimization for spoken word recognition
                            Output.Distance = (DtwMatrix(ColumnCount - 1, RowCount - 1).AccumulatedDistance) / (ColumnCount - 1 + RowCount - 1)

                        Case DtwNormalizationTypes.StepCountNormalization

                            'Normalizes the result by the number of steps that 
                            'MsgBox("HistoryOutputList.Count: " & HistoryOutputList.Count & " A+B: " & ColumnCount - 1 + RowCount - 1)
                            Output.Distance = (DtwMatrix(ColumnCount - 1, RowCount - 1).AccumulatedDistance) / Output.SelectedPath.Count
                        Case Else
                            Throw New Exception("Invalid DtwNormalizationType.")
                    End Select

                    Return Output

                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try

            End Function

            Public Enum GlobalRestrictionTypes
                ''' <summary>
                ''' The whole X*Y area included.
                ''' </summary>
                NoLocalRestrictions
                ''' <summary>
                ''' Includes a diamond shaped area with the width of 1 at start and stop, and increasing towards mid-time. 
                ''' </summary>
                Diamond
                ''' <summary>
                ''' Includes an area delimited by paralell lines, 30 % away from the diagonal.
                ''' </summary>
                QuarterWidth
            End Enum


            Private Function IsWithinMeasurementRegion(ByRef Capital_X As Integer, ByRef x As Integer,
                                                       ByRef Capital_Y As Integer, ByRef y As Integer,
                                                   ByRef GlobalRestrictionType As GlobalRestrictionTypes) As Boolean
                Select Case GlobalRestrictionType
                    Case GlobalRestrictionTypes.Diamond
                        If IsBelowBlockingAreaBoundary(Capital_X, x, Capital_Y, y, GlobalRestrictionType) = False Then Return False
                        If IsRightOfBlockingAreaBoundary(Capital_X, x, Capital_Y, y, GlobalRestrictionType) = False Then Return False

                    Case GlobalRestrictionTypes.QuarterWidth
                        If y < Rounding((Capital_Y / Capital_X) * x - 0.3 * Capital_Y, roundingMethods.alwaysUp) Then Return False
                        If y > Int((Capital_Y / Capital_X) * x + 0.3 * Capital_Y) Then Return False

                    Case Else
                        Throw New NotImplementedException("Invalid GlobalRestrictionType")
                End Select


                Return True

            End Function

            Private Function IsBelowBlockingAreaBoundary(ByRef Capital_X As Integer, ByRef x As Integer,
                                                         ByRef Capital_Y As Integer, ByRef y As Integer,
                                                     ByRef GlobalRestrictionType As GlobalRestrictionTypes) As Boolean

                'As of now, GlobalRestrictionType is used in this function, but is retained as it may be used in the future.

                Select Case GlobalRestrictionType
                    Case GlobalRestrictionTypes.Diamond
                        If x < Int(((Capital_X * y) / (2 * Capital_Y))) Then Return False
                        If y > Rounding(((Capital_Y * x) / (2 * Capital_X)) + (Capital_Y / 2), roundingMethods.alwaysUp) Then Return False
                End Select

                Return True

            End Function

            Private Function IsRightOfBlockingAreaBoundary(ByRef Capital_X As Integer, ByRef x As Integer,
                                                           ByRef Capital_Y As Integer, ByRef y As Integer,
                                                       ByRef GlobalRestrictionType As GlobalRestrictionTypes) As Boolean

                'As of now, GlobalRestrictionType is used in this function, but is retained as it may be used in the future.

                Select Case GlobalRestrictionType
                    Case GlobalRestrictionTypes.Diamond
                        If y < Int(((Capital_Y * x) / (2 * Capital_X))) Then Return False
                        If x > Rounding(((Capital_X * y) / (2 * Capital_Y)) + (Capital_X / 2), roundingMethods.alwaysUp) Then Return False
                End Select

                Return True

            End Function

            Private Sub DiagonalDtwMove(ByRef DtwMatrix(,) As DtwPoint, Column_x As Integer, Row_y As Integer, DiagonalPathValue As Double)

                'A diagonal move
                DtwMatrix(Column_x, Row_y).AccumulatedDistance = DiagonalPathValue
                DtwMatrix(Column_x, Row_y).TransitionDirection = TransitionDirections.Diagonal

                'Storing history from the diagonally preceding cell
                For Each PrecedingItem In DtwMatrix(Column_x - 1, Row_y - 1).History
                    DtwMatrix(Column_x, Row_y).History.Add(PrecedingItem)
                Next

            End Sub

            Private Sub HorizontalDtwMove(ByRef DtwMatrix(,) As DtwPoint, Column_x As Integer, Row_y As Integer, HorizontalPathValue As Double)

                'A horizontal move
                DtwMatrix(Column_x, Row_y).AccumulatedDistance = HorizontalPathValue
                DtwMatrix(Column_x, Row_y).TransitionDirection = TransitionDirections.Horizontal

                'Storing history from the horizontally preceding cell
                For Each PrecedingItem In DtwMatrix(Column_x - 1, Row_y).History
                    DtwMatrix(Column_x, Row_y).History.Add(PrecedingItem)
                Next

            End Sub

            Private Sub VerticalDtwMove(ByRef DtwMatrix(,) As DtwPoint, Column_x As Integer, Row_y As Integer, VerticalPathValue As Double)

                'A vertical move
                DtwMatrix(Column_x, Row_y).AccumulatedDistance = VerticalPathValue
                DtwMatrix(Column_x, Row_y).TransitionDirection = TransitionDirections.Vertical

                'Storing history from the vertically preceding cell
                For Each PrecedingItem In DtwMatrix(Column_x, Row_y - 1).History
                    DtwMatrix(Column_x, Row_y).History.Add(PrecedingItem)
                Next

            End Sub


            Public Class DtwResult
                ''' <summary>
                ''' Holds the time warped distance between the input arrays.
                ''' </summary>
                Public Distance As Double
                Public SelectedPath As List(Of DtwPoint)

                ''' <summary>
                ''' Contains the average unweighted time warped distance between the input arrays
                ''' </summary>
                Public AverageUnweightedDistance As Double

            End Class

            Public Class DtwPoint
                Public AccumulatedDistance As Double
                Public X_Y_Distance As Double
                Public Column_x As Integer
                Public Row_y As Integer
                Public History As New List(Of DtwPoint)
                Public TransitionDirection As TransitionDirections

                Public Sub New(Column_x As Integer, Row_y As Integer, AccumulatedDistance As Double)
                    Me.AccumulatedDistance = AccumulatedDistance
                    Me.Column_x = Column_x
                    Me.Row_y = Row_y
                End Sub

            End Class

            Public Enum TransitionDirections
                Diagonal
                Vertical
                Horizontal
                NoDirection
                BlockedPoint
            End Enum

            Public Enum DtwNormalizationTypes
                ''' <summary>
                ''' Does no normalization of the final DTX distance. (I.e. summed time warped distance is reported.)
                ''' </summary>
                NoNormalization
                ''' <summary>
                ''' Normalizes the final DTX distance by the number of transitions was summed to create that distance. (I.e. average time warped distance is reported.)
                ''' </summary>
                StepCountNormalization
                ''' <summary>
                '''Normalizes the final DTX distance by the X-1+Y-1. This is the type of normalization of White and Neely, as reported in Sakoe and Chiba 1978, Optimization for spoken word recognition.
                ''' </summary>
                XYNormalization
            End Enum

        End Class

        Public Function Degrees2Radians(ByVal Degrees As Double) As Double
            Return Degrees * System.Math.PI / 180
        End Function

        Public Function Radians2Degrees(ByVal Radians As Double) As Double
            Return Radians * 180 / System.Math.PI
        End Function

        ''' <summary>
        ''' Unwraps the indicated angle into the range -180 (is lower than) Azimuth (which is equal to or lower than) 180 degrees.
        ''' </summary>
        ''' <param name="Angle">The angle in degrees</param>
        ''' <returns></returns>
        Public Function UnwrapAngle(ByVal Angle As Integer) As Integer

            'Gets the remainder when dividing by 360
            Dim UnwrappedAngle As Integer
            Dim Div = System.Math.DivRem(Angle, 360, UnwrappedAngle)

            'Sets the Azimuth in the following range: -180 < Azimuth <= 180
            If UnwrappedAngle > 180 Then UnwrappedAngle -= 360

            Return UnwrappedAngle
        End Function

        ''' <summary>
        ''' Unwraps the indicated angle into the range -180 (is lower than) Azimuth (which is equal to or lower than) 180 degrees.
        ''' </summary>
        ''' <param name="Angle">The angle in degrees</param>
        ''' <returns></returns>
        Public Function UnwrapAngle(ByVal Angle As Double) As Double

            'Gets the remainder when dividing by 360
            Dim UnwrappedAngle As Double = Angle Mod 360

            'Sets the Azimuth in the following range: -180 < Azimuth <= 180
            If UnwrappedAngle > 180 Then UnwrappedAngle -= 360

            Return UnwrappedAngle
        End Function


    End Module

End Namespace