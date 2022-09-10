Public Module CriticalDifferences

    Public Enum OutputUnits
        TrialCount
        Proportions
        Percentages
    End Enum

    Public Enum Language
        English
        Swedish
    End Enum


    ''' <summary>
    ''' Creates a tab delimited table of critical differences.
    ''' </summary>
    ''' <param name="n1">Length of list 1</param>
    ''' <param name="n2">Length of list 2</param>
    ''' <param name="ConfidenceLevel">Nominal confidence level</param>
    ''' <param name="OutputUnit">The unit of output</param>
    ''' <param name="ShortHeadings">Set to false to abbreviate headings</param>
    ''' <param name="Decimals">Can be set in order to round the output to a number of decimals. If left to Nothing, no rounding will be applied.</param>
    ''' <returns></returns>
    Public Function GetCriticalDifferenceTable(ByVal n1 As Integer, ByVal n2 As Integer,
                                               ByVal ConfidenceLevel As Double, ByVal OutputUnit As OutputUnits,
                                               Optional ByVal ShortHeadings As Boolean = False,
                                               Optional ByVal Decimals As Integer? = Nothing,
                                               Optional ByVal AgrestiCaffoCorrection As Boolean = False,
                                               Optional ByVal Language As Language = Language.English) As String

        If ConfidenceLevel <> 0.95 And AgrestiCaffoCorrection = True Then
            Select Case Language
                Case Language.Swedish
                    Return "Agresti-Caffo korrektion är bara tillgängligt för en konfidensnivå på 95 %!"
                Case Else
                    Return "Agresti-Caffo correction is only available for a confidence level of 95 %!"
            End Select
        End If

        Dim CR As Object = Nothing

        'Calculating critical differences
        Select Case OutputUnit
            Case OutputUnits.TrialCount
                CR = CalculateCriticalDifferenceScores(n1, n2, ConfidenceLevel, AgrestiCaffoCorrection)
            Case OutputUnits.Proportions, OutputUnits.Percentages
                CR = CalculateCriticalDifferenceScores_Proportions(n1, n2, ConfidenceLevel, AgrestiCaffoCorrection)
            Case Else
                Select Case Language
                    Case Language.Swedish
                        Return "Ogiltigt utdataformat!"
                    Case Else
                        Return "Invalid output type!"
                End Select
        End Select


        'Formatting as a string
        If CR IsNot Nothing Then

            Dim OutputList As New List(Of String)

            'Adding headings
            Select Case Language
                Case Language.Swedish
                    OutputList.Add("Kritiska skillnader för resultat på taluppfattningstest:")
                    OutputList.Add("")
                    OutputList.Add("Längd på lista 1 ( n1 ) = " & n1)
                    OutputList.Add("Längd på lista ( n2 ) = " & n2)
                    OutputList.Add("Konfidensnivå = " & ConfidenceLevel)

                Case Else
                    OutputList.Add("Critical differences for speech perception scores:")
                    OutputList.Add("")
                    OutputList.Add("List length 1 ( n1 ) = " & n1)
                    OutputList.Add("List length 2 ( n2 ) = " & n2)
                    OutputList.Add("Confidence level = " & ConfidenceLevel)

            End Select


            Select Case OutputUnit
                Case OutputUnits.TrialCount
                    Select Case Language
                        Case Language.Swedish
                            OutputList.Add("Enhet: Antal rätta svar")
                        Case Else
                            OutputList.Add("Output unit: Number of correct trials")
                    End Select
                Case OutputUnits.Proportions
                    Select Case Language
                        Case Language.Swedish
                            OutputList.Add("Enhet: Andel rätta svar")
                        Case Else
                            OutputList.Add("Output unit: Proportion correct trials")
                    End Select
                Case OutputUnits.Percentages
                    Select Case Language
                        Case Language.Swedish
                            OutputList.Add("Enhet: Procent korrekt")
                        Case Else
                            OutputList.Add("Output unit: Percent correct trials")
                    End Select
            End Select

            OutputList.Add("")

            Select Case Language
                Case Language.Swedish
                    OutputList.Add("Tolka resultaten enligt följande:")
                    OutputList.Add("För att skillnaden mellan två testresultat ska antas vara statistiskt signifikanta ska resultatet av det andra testet antingen a) vara högre än värdet som anges i den tredje kolumnen ('Övre'), eller b) lägre än värdet som agnes i den andra kolumnen ('Nedre')." &
                           "Den rad på vilken dessa värden ska avläsas anges av resultatet på det första testet, vilket ges i den första kolumnen.")
                    OutputList.Add("")

                    Select Case AgrestiCaffoCorrection
                        Case True
                            OutputList.Add("Använd metod: Agresti-Caffo corrected normal approximation to the binomial. För detaljer, se:" & vbCrLf &
                                   "    •  Agresti, A., & Caffo, B. (2000). Simple and effective confidence intervals for proportions and differences of proportions result from adding two successes and two failures. The American Statistician, 54(4), 280-288. doi:10.1080/00031305.2000.10474560" & vbCrLf &
                                   "    •  Fagerland, M. W., Lydersen, S., & Laake, P. (2015). Recommended confidence intervals for two independent binomial proportions. Statistical Methods in Medical Research, 24(2), 224-254. doi:10.1177/0962280211415469")
                        Case Else
                            OutputList.Add("Använd metod: Normal approximation to the binomial. För detaljer, se:" & vbCrLf &
                                   "    •  Carney, E., & Schlauch, R. S. (2007). Critical difference table for word recognition testing derived using computer simulation. Journal of Speech, Language, and Hearing Research, 50(5), 1203-1209. doi:10.1044/1092-4388(2007/084)")
                    End Select

                Case Else
                    OutputList.Add("Interpret the results accordingly:")
                    OutputList.Add("For the difference between two test results to be concidered statistically significant, the result of the second test must either a) exceed the value given in the third column 'Upper', or b) be less than the value given in the second column ('Lower')." &
                           "The line from which to read off the values is indicated by the result of the first test, as given in the first column.")
                    OutputList.Add("")

                    Select Case AgrestiCaffoCorrection
                        Case True
                            OutputList.Add("Method used: Agresti-Caffo corrected normal approximation to the binomial. For details see:" & vbCrLf &
                                   "    •  Agresti, A., & Caffo, B. (2000). Simple and effective confidence intervals for proportions and differences of proportions result from adding two successes and two failures. The American Statistician, 54(4), 280-288. doi:10.1080/00031305.2000.10474560" & vbCrLf &
                                   "    •  Fagerland, M. W., Lydersen, S., & Laake, P. (2015). Recommended confidence intervals for two independent binomial proportions. Statistical Methods in Medical Research, 24(2), 224-254. doi:10.1177/0962280211415469")
                        Case Else
                            OutputList.Add("Method used: Normal approximation to the binomial. For details see:" & vbCrLf &
                                   "    •  Carney, E., & Schlauch, R. S. (2007). Critical difference table for word recognition testing derived using computer simulation. Journal of Speech, Language, and Hearing Research, 50(5), 1203-1209. doi:10.1044/1092-4388(2007/084)")
                    End Select
            End Select


            OutputList.Add("")

            Select Case Language
                Case Language.Swedish

                    OutputList.Add("Resultat")
                    OutputList.Add("" & vbTab & "Gränser")

                    If ShortHeadings = True Then
                        Select Case OutputUnit
                            Case OutputUnits.TrialCount
                                OutputList.Add("Antal rätt" & vbTab & "Nedre" & vbTab & "Övre")
                            Case OutputUnits.Proportions
                                OutputList.Add("Andel rätt" & vbTab & "Nedre" & vbTab & "Övre")
                            Case OutputUnits.Percentages
                                OutputList.Add("% rätt" & vbTab & "Nedre" & vbTab & "Övre")
                        End Select
                    Else
                        Select Case OutputUnit
                            Case OutputUnits.TrialCount
                                OutputList.Add("Antal rätt test 1" & vbTab & "Nedre gräns test 2" & vbTab & "Övre gräns test 2")
                            Case OutputUnits.Proportions
                                OutputList.Add("Andel rätt test 1" & vbTab & "Nedre gräns test 2" & vbTab & "Övre gräns test 2")
                            Case OutputUnits.Percentages
                                OutputList.Add("% rätt test 1" & vbTab & "Nedre gräns test 2 (%)" & vbTab & "Övre gräns test 2 (%)")
                        End Select
                    End If

                Case Else

                    OutputList.Add("Results")
                    OutputList.Add("" & vbTab & "Limits")

                    If ShortHeadings = True Then
                        Select Case OutputUnit
                            Case OutputUnits.TrialCount
                                OutputList.Add("Score" & vbTab & "Lower" & vbTab & "Upper")
                            Case OutputUnits.Proportions
                                OutputList.Add("Score (Prop)" & vbTab & "Lower" & vbTab & "Upper")
                            Case OutputUnits.Percentages
                                OutputList.Add("% Score" & vbTab & "Lower" & vbTab & "Upper")
                        End Select
                    Else
                        Select Case OutputUnit
                            Case OutputUnits.TrialCount
                                OutputList.Add("Score Test 1" & vbTab & "Lower limit Test 2" & vbTab & "Upper limit Test 2")
                            Case OutputUnits.Proportions
                                OutputList.Add("Score Test 1 (Prop)" & vbTab & "Lower limit Test 2" & vbTab & "Upper limit Test 2")
                            Case OutputUnits.Percentages
                                OutputList.Add("% Score Test 1" & vbTab & "% Lower limit Test 2" & vbTab & "% Upper limit Test 2")
                        End Select
                    End If
            End Select


            'Adding data
            For Each Row In CR
                Select Case OutputUnit
                    Case OutputUnits.TrialCount
                        OutputList.Add(Row.Key & vbTab & Row.Value.Item1 & vbTab & Row.Value.Item2)

                    Case OutputUnits.Proportions
                        If Decimals Is Nothing Then
                            OutputList.Add(Row.Key & vbTab & Row.Value.Item1 & vbTab & Row.Value.Item2)
                        Else
                            OutputList.Add(Math.Round(Row.Key, Decimals.Value) & vbTab &
                                   Math.Round(Row.Value.Item1, Decimals.Value) & vbTab &
                                   Math.Round(Row.Value.Item2, Decimals.Value))
                        End If

                    Case OutputUnits.Percentages
                        If Decimals Is Nothing Then
                            OutputList.Add(100 * Row.Key & vbTab & 100 * Row.Value.Item1 & vbTab & 100 * Row.Value.Item2)
                        Else
                            OutputList.Add(Math.Round(100 * Row.Key, Decimals.Value) & vbTab &
                                   Math.Round(100 * Row.Value.Item1, Decimals.Value) & vbTab &
                                   Math.Round(100 * Row.Value.Item2, Decimals.Value))
                        End If

                End Select

            Next

            Return String.Join(vbCrLf, OutputList)
        Else
            Select Case Language
                Case Language.Swedish
                    Return "Ett fel uppstod. Kan inte beräkna kritiska gränser för de angivna listlängderna och/eller konfidensinå."
                Case Else
                    Return "An error occured. Unable to calculate critical diffrences for the requested list lengths and confidence level."
            End Select

        End If

    End Function

    Public Function CalculateCriticalDifferenceScores(ByVal n1 As Integer, ByVal n2 As Integer, ByVal ConfidenceLevel As Double, Optional ByVal AgrestiCaffoCorrection As Boolean = False) As SortedList(Of Integer, Tuple(Of Integer, Integer))

        Dim Output As New SortedList(Of Integer, Tuple(Of Integer, Integer))

        ' Looping through Each score 1
        For Score1 = 0 To n1

            'Getting the critical differences For score 1, given the indicated confidence level  
            Dim CurrentCriticalDifference = GetCriticalDifferenceLimits(n1, n2, Score1, ConfidenceLevel, AgrestiCaffoCorrection)

            'Storing the result 
            Output.Add(Score1, CurrentCriticalDifference)

        Next

        'Returning the critical differences
        Return Output

    End Function


    Public Function CalculateCriticalDifferenceScores_Proportions(ByVal n1 As Integer, ByVal n2 As Integer,
                                                                  ByVal ConfidenceLevel As Double, Optional ByVal AgrestiCaffoCorrection As Boolean = False) As SortedList(Of Double, Tuple(Of Double, Double))

        Dim Output As New SortedList(Of Double, Tuple(Of Double, Double))

        ' Looping through Each score 1
        For Score1 = 0 To n1

            'Getting the critical differences For score 1, given the indicated confidence level  
            Dim CurrentCriticalDifference = GetCriticalDifferenceLimits(n1, n2, Score1, ConfidenceLevel, AgrestiCaffoCorrection)

            'Storing the result 
            Output.Add(Score1 / n1, New Tuple(Of Double, Double)(CurrentCriticalDifference.Item1 / n2, CurrentCriticalDifference.Item2 / n2))
        Next

        'Returning the critical differences
        Return Output

    End Function


    ' Returns the limits of the non-significant region in SignificanceList.
    Private Function GetCriticalDifferenceLimits(ByVal n1 As Integer, ByVal n2 As Integer, ByVal Score1 As Integer, ByVal ConfidenceLevel As Double, Optional ByVal AgrestiCaffoCorrection As Boolean = False) As Tuple(Of Integer, Integer)

        Dim LowerBound As Integer = 0
        Dim UpperBound As Integer = n2

        'Getting the limits of the current score, given the current confidence level
        Dim SignificanceList(n2) As Boolean

        ' Searching for the lower bound of the critical interval
        For Score2 = 0 To n2
            If IsNotSignificantlyDifferent(n1, n2, Score1, Score2, ConfidenceLevel, AgrestiCaffoCorrection) = True Then
                LowerBound = Score2
                Exit For
            End If
        Next

        ' Searching for the upper bound of the critical interval
        For InvScore2 = 0 To n2
            Dim Score2 As Integer = n2 - InvScore2
            If IsNotSignificantlyDifferent(n1, n2, Score1, Score2, ConfidenceLevel, AgrestiCaffoCorrection) = True Then
                UpperBound = Score2
                Exit For
            End If
        Next

        Return (New Tuple(Of Integer, Integer)(LowerBound, UpperBound))

    End Function

    ''' <summary>
    ''' Using the method of normal appriximation to the binomial in oder to determines if Score 1 is NOT significantly different from Score 2, given n1, n2, and the indicated confidence level.
    ''' </summary>
    ''' <param name="n1">Length of list / test 1</param>
    ''' <param name="n2">Length of list / test 2</param>
    ''' <param name="Score1">Score of test 1 (in number correct trials)</param>
    ''' <param name="Score2">Score of test 2 (in number correct trials)</param>
    ''' <param name="ConfidenceLevel">The confidence level (0-1)</param>
    ''' <param name="AgrestiCaffoCorrection">Set to True in order to use Agresti-Caffo correction.</param>
    ''' <returns></returns>
    Public Function IsNotSignificantlyDifferent(ByVal n1 As Integer, ByVal n2 As Integer, ByVal Score1 As Integer, ByVal Score2 As Integer, ByVal ConfidenceLevel As Object, Optional ByVal AgrestiCaffoCorrection As Boolean = False) As Boolean

        If ConfidenceLevel <> 0.95 And AgrestiCaffoCorrection = True Then
            Throw New ArgumentException("Agresti-Caffo correction is only available for a confidence level of 0.95!")
        End If

        If AgrestiCaffoCorrection = False Then

            ' Using traditional normal approximation without any correction.

            ' Calculating proportion correct
            Dim p1 = Score1 / n1
            Dim p2 = Score2 / n2

            ' Approximating the standard deviation of the binomial difference distribution given n1, n2, p1 and p2
            Dim sd = Math.Sqrt(((p1 * (1 - p1)) / n1) + ((p2 * (1 - p2)) / n2))

            ' Getting the standard score divided by two (Za_div_2) appropriate for the current confidence level
            Dim Za_div_2 = MathNet.Numerics.Distributions.Normal.InvCDF(0, 1, 1 - ((1 - ConfidenceLevel) / 2))
            ' Equivalent R command: Za_div_2 = qnorm(1 - ((1 - ConfidenceLevel) / 2))

            ' Calculating the test value
            Dim CriticalValue = Za_div_2 * sd

            ' Calculating estimated proportion correct difference
            Dim d_Est = p1 - p2

            Dim lowerCiBoundary = d_Est - CriticalValue
            Dim upperCiBoundary = d_Est + CriticalValue

            If lowerCiBoundary > 0 Or upperCiBoundary < 0 Then

                'if (lowerCiBoundary > 0 | upperCiBoundary < 0) {
                ' Returns FALSE to indicate a significant difference in proportion correct responses
                Return False

            Else
                ' Returns TRUE to indicate no significant difference in proportion correct responses
                Return True

            End If

            ' N.B. The above is equivalent to the method used in Carney and Schaugh 2007


        Else

            ' Using the Agresti-Caffo approximative method, as presented in Fagerland et al. 2015.

            ' Calculating adjusted ns
            Dim n1_br = n1 + 2
            Dim n2_br = n2 + 2

            ' Calculating adjusted proportion correct
            Dim p1_br = (Score1 + 1) / n1_br
            Dim p2_br = (Score2 + 1) / n2_br

            ' Approximating the standard deviation of the adjusted binomial difference distribution given n1_br, n2_br, p1_br and p2_br
            Dim sd = Math.Sqrt(((p1_br * (1 - p1_br)) / n1_br) + ((p2_br * (1 - p2_br)) / n2_br))

            ' Getting the standard score divided by two (Za_div_2) appropriate for the current confidence level
            Dim Za_div_2 = MathNet.Numerics.Distributions.Normal.InvCDF(0, 1, 1 - ((1 - ConfidenceLevel) / 2))
            ' Equivalent R command: Za_div_2 = qnorm(1 - ((1 - ConfidenceLevel) / 2))

            ' Calculating the test value
            Dim CriticalValue = Za_div_2 * sd

            ' Calculating estimated proportion correct difference
            Dim d_Est = p1_br - p2_br

            Dim lowerCiBoundary = d_Est - CriticalValue
            Dim upperCiBoundary = d_Est + CriticalValue

            If lowerCiBoundary > 0 Or upperCiBoundary < 0 Then

                'if (lowerCiBoundary > 0 | upperCiBoundary < 0) {
                ' Returns FALSE to indicate a significant difference in proportion correct responses
                Return False
            Else
                ' Returns TRUE to indicate no significant difference in proportion correct responses
                Return True

            End If

        End If

    End Function


    Public Function pmax(ByVal X As Double(), ByVal Limit As Double) As Double()
        Dim Y(X.Length - 1) As Double
        For i = 0 To X.Length - 1
            Y(i) = Math.Max(X(i), Limit)
        Next
        Return Y
    End Function

    Public Function pmin(ByVal X As Double(), ByVal Limit As Double) As Double()
        Dim Y(X.Length - 1) As Double
        For i = 0 To X.Length - 1
            Y(i) = Math.Min(X(i), Limit)
        Next
        Return Y
    End Function

    Public Function pabs(ByVal X As Double()) As Double()
        Dim Y(X.Length - 1) As Double
        For i = 0 To X.Length - 1
            Y(i) = Math.Abs(X(i))
        Next
        Return Y
    End Function

    Public Function getAdjustedSuccessProbabilities(ByVal X As Double(), ByVal TargetScore As Double, Optional tol As Double = 10 ^ -14, Optional floor As Double = 1 / 3, Optional ByVal MaxIterations As Integer = 10000) As Double()

        If TargetScore < floor Then
            Return Utils.Repeat(TargetScore, X.Length)
        End If

        ' Limiting the range of probabilities to 10^-12 through 1 - 10^-12, to avoid p = 0 and p = 1, which can never be increased by the equation 10
        X = pmax(X, floor + 10 ^ (-12))
        X = pmin(X, 1 - 10 ^ (-12))

        ' Returning X if its mean is the target score
        If TargetScore = X.Average Then Return X

        ' The code below implements equation 10


        '(Note that this is not the optimization used in the R-code of Witte's thesis: I.E.   a_optim <- optimize(AdjSP_OptimF, c(-100, 100), tol = tol, X = X, TargetScore = TargetScore) )
        Dim CurrentStep As Double = 200
        Dim CurrentSampleX As Double = -100
        Dim CurrentSlopeWidth As Double = tol
        Dim Point1X As Double
        Dim Point2X As Double

        Dim Iterations As Integer = 0

        Do

            Iterations += 1

            If Iterations > MaxIterations Then Exit Do

            Point1X = CurrentSampleX - CurrentSlopeWidth / 2
            Point2X = CurrentSampleX + CurrentSlopeWidth / 2

            Dim Point1Y As Double = AdjSP_OptimF(Point1X, X, TargetScore, floor)
            Dim Point2Y As Double = AdjSP_OptimF(Point2X, X, TargetScore, floor)

            'Dim AverageSampleValue As Double = (Point1Y + Point2Y) / 2
            Dim DeltaX As Double = Point2X - Point1X
            Dim DeltaY As Double = Point2Y - Point1Y

            If DeltaY = 0 Then
                CurrentSlopeWidth += CurrentStep / 10
                Continue Do
            End If

            Dim SampleSlope As Double = DeltaY / DeltaX

            If CurrentStep < tol Then
                Exit Do
            End If

            If SampleSlope < 0 Then
                CurrentSlopeWidth = tol
                CurrentSampleX += CurrentStep
                CurrentStep /= 2
            ElseIf SampleSlope > 0 Then
                CurrentSlopeWidth = tol
                CurrentSampleX -= CurrentStep
                CurrentStep /= 2
            Else
                'This point should never be reached, but it's kept here anyway just in case...
                CurrentSlopeWidth += CurrentStep / 10
            End If

        Loop

        Console.WriteLine(String.Concat("Detected function minimum of " & AdjSP_OptimF(CurrentSampleX, X, TargetScore, floor) &
                                        " within the range " & Point1X & " to " & Point2X &
                                        " in: ", Iterations & " iterations"))


        Dim Y = AdjSP_F(CurrentSampleX, X, floor)

        If Math.Abs(TargetScore - Y.Average) > tol Then
            MsgBox("Warning! Optimization algorithm failure! Average adjusted score deviates from the target score by " & 100 * Math.Abs(TargetScore - Y.Average) & " percentage points.", MsgBoxStyle.Exclamation, "Critical difference calculations!")
        End If


        'Dim AList As New List(Of Double)
        'Dim ValueList As New List(Of Double)
        'For a As Double = -100 To 100 Step 0.001
        '    AList.Add(a)
        '    ValueList.Add(AdjSP_OptimF(a, X, TargetScore, floor))
        'Next

        'Dim a_optim_minimum As Double = AList(ValueList.IndexOf(ValueList.Min))

        'Dim Y = AdjSP_F(a_optim_minimum, X, floor)

        ' Returns the adjusted probability vector
        Return Y

    End Function

    Private Function AdjSP_OptimF(ByVal a As Double, ByVal X As Double(), ByVal TargetScore As Double, ByVal floor As Double) As Double
        Return Math.Abs(TargetScore - AdjSP_F(a, X, floor).Average)
    End Function

    Private Function AdjSP_F(ByVal a As Double, ByVal X As Double(), ByVal floor As Double) As Double()
        Dim Y(X.Length - 1) As Double
        For i = 0 To X.Length - 1
            Y(i) = floor + (1 - floor) * (1 / (1 + ((1 / ((X(i) - floor) / (1 - floor))) - 1) * Math.Exp(a)))
            '  Or simplified as:
            '  Y(i) = floor + (1-floor) / ( 1 + ( ( (1-floor)/(X(i)-floor) )  -1 ) * exp(a)  )
        Next
        Return Y
    End Function


    ' getAdjustedSuccessProbabilities <- function(X, TargetScore, tol = 10^-14, logPath = "./CovProbExports/SiPCovProb_adj_log.txt", floor){
    ' 
    '   
    '   if (TargetScore < floor) {
    '     return(rep(TargetScore, length(X)))
    '   }
    '     
    '   #Limiting the range of probabilities to 10^-12 through 1 - 10^-12, to avoid p = 0 and p = 1, which can never be increased by the equation 10
    '   X <- pmax(X, floor + 10^(-12))
    '   X <- pmin(X, 1-10^(-12))
    ' 
    '   # Returning X if its mean is the target score
    '   if (TargetScore == mean(X)) {
    '     return(X)
    '   }
    '   
    '   # The code below implements equation 10
    '   
    '   AdjSP_OptimF <- function(a,X,TargetScore){
    '     return(abs(TargetScore - mean(AdjSP_F(a,X))))
    '   }
    '   
    '   AdjSP_F <- function(a,X) {
    '     Y <- floor + (1-floor) * (1 / (1 + ((1/ ( (X-floor)/(1-floor) ) ) - 1) * exp(a)))
    '     return(Y)
    '   }
    '   
    '   # Or simplified as:
    '   # AdjSP_F_Simpl <- function(a,X,floor) {
    '   #   Y <- floor + (1-floor) / ( 1 + ( ( (1-floor)/(X-floor) )  -1 ) * exp(a)  )
    '   #   return(Y)
    '   # }
    '   
    '   
    '   a_optim <- optimize(AdjSP_OptimF, c(-100, 100), tol = tol, X = X, TargetScore = TargetScore)
    '   
    '   #plot(AdjSP_F(a_optim$minimum, X), ylim = c(0,1))
    '   #mean(AdjSP_F(a_optim$minimum, X))
    '   
    '   # Prints the difference if remaining difference is above a threshold value
    '   Y <- AdjSP_F(a_optim$minimum, X)
    '   if (abs(mean(Y) - TargetScore) > 10^-7 ) {
    '     #print(paste("Optimization mismatch above 10^-7 occurred. Mismatch = ", abs(mean(Y) - TargetScore)))
    '     
    '     Message <- paste("getAdjustedSuccessProbabilities \tOptimization mismatch above 10^-7 occurred. \tMismatch =", abs(mean(Y) - TargetScore), "\tn =", length(X))
    '     print(Message)
    '     if (logPath != "") {
    '       write.table(x = Message, file = logPath, append = TRUE,sep = "\t", col.names = FALSE, row.names = FALSE)
    '     }
    '   }
    '   
    '   # Returns the adjusted probability vector
    '   return(Y)
    '   
    ' }



End Module


