

Namespace Utils

    Public Module Math

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

        Public Function Shuffle(ByVal Input As List(Of String), ByRef Randomizer As Random) As List(Of String)
            Dim SampleOrder = SampleWithoutReplacement(Input.Count, 0, Input.Count, Randomizer)
            Dim Output As New List(Of String)
            For Each RandomIndex In SampleOrder
                Output.Add(Input(RandomIndex))
            Next
            Return Output
        End Function

        Public Function Shuffle(ByVal Input As List(Of Object), ByRef Randomizer As Random) As List(Of Object)
            Dim SampleOrder = SampleWithoutReplacement(Input.Count, 0, Input.Count, Randomizer)
            Dim Output As New List(Of Object)
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

        Public Function Shuffle(ByVal Input As List(Of Double), ByRef Randomizer As Random) As List(Of Double)
            Dim SampleOrder = SampleWithoutReplacement(Input.Count, 0, Input.Count, Randomizer)
            Dim Output As New List(Of Double)
            For Each RandomIndex In SampleOrder
                Output.Add(Input(RandomIndex))
            Next
            Return Output
        End Function


        Public Sub ComplexMultiplication(Real1 As Double(), Imag1 As Double(), Real2 As Double(), Imag2 As Double())

            If Real1.Length <> Imag1.Length Or Real1.Length <> Imag1.Length Or Real1.Length <> Real2.Length Or Real1.Length <> Imag2.Length Then
                Throw New ArgumentException("Unequal length of input arrays")
            End If

            If OstfBase.UseOptimizationLibraries = False Then

                'Performs complex multiplications
                Dim TempValue As Double = 0
                For n = 0 To Real1.Length - 1
                    TempValue = Real1(n) 'stores this value so that it does not get overwritten in the following line (it needs to be used also two lines below)
                    Real1(n) = TempValue * Real2(n) - Imag1(n) * Imag2(n)
                    Imag1(n) = TempValue * Imag2(n) + Imag1(n) * Real2(n)
                Next

            Else

                LibOstfDsp_VB.ComplexMultiplication(Real1, Imag1, Real2, Imag2)

            End If

        End Sub


        Public Sub CopyToDouble(SourceArray As Single(), TargetArray As Double())

            If TargetArray.Length < SourceArray.Length Then Throw New ArgumentException("TargetArray cannot be shorter than SourceArray")

            If OstfBase.UseOptimizationLibraries = False Then
                For i = 0 To SourceArray.Length - 1
                    TargetArray(i) = SourceArray(i)
                Next
            Else
                LibOstfDsp_VB.CopyToDouble(SourceArray, TargetArray)
            End If

        End Sub

        ''' <summary>
        ''' Multiplies each element in the Array1 array with the Factor using fast SIMD (Single Instruction, Multiple Data) operations
        ''' </summary>
        ''' <param name="Values"></param>
        ''' <returns></returns>
        Public Sub MultiplyArray(Values() As Double, Factor As Double)

            If OstfBase.UseOptimizationLibraries = False Then

                Dim VectorSize As Integer = System.Numerics.Vector(Of Double).Count
                Dim FactorVector = New System.Numerics.Vector(Of Double)(Factor)

                Dim i As Integer
                For i = 0 To Values.Length - VectorSize Step VectorSize
                    Dim v As New System.Numerics.Vector(Of Double)(Values, i)
                    v = v * FactorVector
                    v.CopyTo(Values, i)
                Next

                ' Handle any remaining elements at the end that don't fit into a full vector.
                For i = i To Values.Length - 1
                    Values(i) *= Factor
                Next

            Else

                LibOstfDsp_VB.MultiplyArray(Values, Factor)

            End If

        End Sub

        ''' <summary>
        ''' Multiplies each element in the Array1 array with the Factor using fast SIMD (Single Instruction, Multiple Data) operations
        ''' </summary>
        ''' <param name="Values"></param>
        ''' <returns></returns>
        Public Sub MultiplyArray(Values() As Single, Factor As Single)

            If OstfBase.UseOptimizationLibraries = False Then

                Dim VectorSize As Integer = System.Numerics.Vector(Of Single).Count
                Dim FactorVector = New System.Numerics.Vector(Of Single)(Factor)

                Dim i As Integer
                For i = 0 To Values.Length - VectorSize Step VectorSize
                    Dim v As New System.Numerics.Vector(Of Single)(Values, i)
                    v = v * FactorVector
                    v.CopyTo(Values, i)
                Next

                ' Handle any remaining elements at the end that don't fit into a full vector.
                For i = i To Values.Length - 1
                    Values(i) *= Factor
                Next

            Else

                LibOstfDsp_VB.MultiplyArray(Values, Factor)

            End If

        End Sub

        ''' <summary>
        ''' Multiplies each element in a section of the Array1 array with the Factor using fast SIMD (Single Instruction, Multiple Data) operations
        ''' </summary>
        ''' <param name="Values">The input array</param>
        ''' <param name="StartIndex">The start index of the section</param>
        ''' <param name="SectionLength">The length of the section</param>
        ''' <returns>The sum of squares of the specified section</returns>
        Public Sub MultiplyArray(Values() As Single, Factor As Single, StartIndex As Integer, SectionLength As Integer)

            If OstfBase.UseOptimizationLibraries = False Then

                Dim VectorSize As Integer = System.Numerics.Vector(Of Single).Count
                Dim FactorVector = New System.Numerics.Vector(Of Single)(Factor)

                ' Ensure we do not exceed the array bounds
                Dim endIndex As Integer = System.Math.Min(StartIndex + SectionLength, Values.Length)

                Dim i As Integer = StartIndex
                While i < endIndex AndAlso i + VectorSize <= endIndex ' Make sure there's enough room for a full vector
                    Dim v As New System.Numerics.Vector(Of Single)(Values, i)
                    v = v * FactorVector
                    v.CopyTo(Values, i)
                    i += VectorSize
                End While

                ' Handle any remaining elements at the end that don't fit into a full vector.
                While i < endIndex
                    Values(i) *= Factor
                    i += 1
                End While

            Else

                LibOstfDsp_VB.MultiplyArraySection(Values, Factor, StartIndex, SectionLength)

            End If

        End Sub

        ''' <summary>
        ''' Adds the two arrays. Either fast SIMD (Single Instruction, Multiple Data) operations are used, or if OstfBase.UseOptimizationLibraries is True using the LibOstfDsp.
        ''' Arrays need to be the same lengths, otherwise an exception is thrown.
        ''' </summary>
        ''' <param name="Array1">The first input/output data array. Upon return this corresponding data array contains the sum of the values in array1 And array2</param>
        ''' <param name="Array2">The the input data array containing the values which should be added to array1</param>
        ''' <returns></returns>
        Public Sub AddTwoArrays(Array1() As Single, Array2() As Single)

            If Array1.Length <> Array2.Length Then Throw New ArgumentException("Arrays 1 and 2 need to have the same lengths.")

            If OstfBase.UseOptimizationLibraries = False Then

                Dim VectorSize As Integer = System.Numerics.Vector(Of Single).Count

                Dim i As Integer
                For i = 0 To Array1.Length - VectorSize Step VectorSize
                    Dim v1 As New System.Numerics.Vector(Of Single)(Array1, i)
                    Dim v2 As New System.Numerics.Vector(Of Single)(Array2, i)
                    v1 += v2
                    v1.CopyTo(Array1, i)
                Next

                ' Handle any remaining elements at the end that don't fit into a full vector.
                For i = i To Array1.Length - 1
                    Array1(i) += Array2(i)
                Next


                'Untested paralell processing alternative
                'Parallel.For(0, Array1.Length \ VectorSize, Sub(i)
                '                                                Dim offset = i * VectorSize
                '                                                Dim v1 As New System.Numerics.Vector(Of Single)(Array1, offset)
                '                                                Dim v2 As New System.Numerics.Vector(Of Single)(Array2, offset)
                '                                                v1 += v2
                '                                                v1.CopyTo(Array1, offset)
                '                                            End Sub)

                '' Handle any remaining elements
                'For i = (Array1.Length \ VectorSize) * VectorSize To Array1.Length - 1
                '    Array1(i) += Array2(i)
                'Next

            Else
                LibOstfDsp_VB.AddTwoFloatArrays(Array1, Array2)
            End If

        End Sub

        ''' <summary>
        ''' Calculates the sum-of-square value of a section of an array using fast SIMD (Single Instruction, Multiple Data) operations
        ''' </summary>
        ''' <param name="Values">The input array</param>
        ''' <param name="startIndex">The start index of the section</param>
        ''' <param name="sectionLength">The length of the section</param>
        ''' <returns>The sum of squares of the specified section</returns>
        Public Function CalculateSumOfSquare(Values() As Single, startIndex As Integer, sectionLength As Integer) As Single

            If OstfBase.UseOptimizationLibraries = False Then

                Dim VectorSize As Integer = System.Numerics.Vector(Of Single).Count
                Dim SumOfSquaresVector As System.Numerics.Vector(Of Single) = System.Numerics.Vector(Of Single).Zero

                ' Ensure we do not exceed the array bounds
                Dim endIndex As Integer = System.Math.Min(startIndex + sectionLength, Values.Length)

                Dim i As Integer = startIndex
                While i < endIndex AndAlso i + VectorSize <= endIndex ' Make sure there's enough room for a full vector
                    Dim v As New System.Numerics.Vector(Of Single)(Values, i)
                    SumOfSquaresVector += v * v
                    i += VectorSize
                End While

                Dim SumOfSquares As Single = 0

                For j As Integer = 0 To VectorSize - 1
                    SumOfSquares += SumOfSquaresVector(j)
                Next

                ' Handle any remaining elements at the end that don't fit into a full vector.
                While i < endIndex
                    SumOfSquares += Values(i) ^ 2
                    i += 1
                End While

                Return SumOfSquares

            Else

                'Calculating the sum of sqares in libostfdsp
                Return LibOstfDsp_VB.CalculateSumOfSquare(Values, startIndex, sectionLength)

            End If

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
        ''' Detects the AvailableValues Array1 index that have the values nearest to the InputValue.
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
                            'The value exists in the Array1, returns it's index
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

        ''' <summary>
        ''' Detects the AvailableValues Array1 indices that have the first values nearest to the InputValue. (N.B. The data in AvailableValues need to be orderred in either ascending or descending order.) 
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
        ''' Calculats the bark filter band width at a specified centre frequency, based on Zwicker and Fastl(1999), Phsycho-acoustics, p 164
        ''' </summary>
        ''' <param name="CentreFrequency"></param>
        ''' <returns></returns>
        Public Function CenterFrequencyToBarkFilterBandwidth(ByVal CentreFrequency As Double)

            Return 25 + 75 * (1 + 1.4 * (CentreFrequency / 1000) ^ 2) ^ 0.69

        End Function

        Public Function getBase_n_Log(ByVal value As Double, Optional ByVal n As Double = 2) As Double

            Return System.Math.Log10(value) / System.Math.Log10(n)

        End Function

        Public Sub DeinterleaveSoundArray(interleavedArray As Single(), channelCount As Integer, channelLength As Integer, concatenatedArrays As Single())

            If OstfBase.UseOptimizationLibraries = False Or OstfBase.CurrentPlatForm = Platforms.WinUI Then 'TODO: Change this when windows opimization dlls implement this function

                ' Takes a flattened matrix in which each channel Is put after each other, And interleaves the channels values
                Dim targetIndex As Integer = 0
                For s = 0 To channelLength - 1
                    For c = 0 To channelCount - 1
                        concatenatedArrays(c * channelLength + s) = interleavedArray(targetIndex)
                        targetIndex += 1
                    Next
                Next

            Else
                LibOstfDsp_VB.DeinterleaveSoundArray(interleavedArray, channelCount, channelLength, concatenatedArrays)
            End If

        End Sub

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
            If UnwrappedAngle <= -180 Then UnwrappedAngle += 360

            Return UnwrappedAngle
        End Function

        Public Function Degrees2Radians(ByVal Degrees As Double) As Double
            Return Degrees * System.Math.PI / 180
        End Function

        Public Function Radians2Degrees(ByVal Radians As Double) As Double
            Return Radians * 180 / System.Math.PI
        End Function


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

        Public Enum StandardDeviationTypes
            Population
            Sample
        End Enum

        ''' <summary>
        ''' Calculates the the coefficient of variation of a set of input values. Also SumOfSquares, mean, SumOfSquares of squares, variance and standard deviation can be attained by using the optional parameters.
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

                'Calculates the SumOfSquares of the values in the input list
                Sum = 0
                For i = 0 To InputListOfDouble.Count - 1
                    Sum += InputListOfDouble(i)
                Next

                'Calculates the arithemtric mean of the values in the input list
                ArithmetricMean = Sum / n

                'Calculates the SumOfSquares of squares of the values in the input list
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


    End Module



End Namespace