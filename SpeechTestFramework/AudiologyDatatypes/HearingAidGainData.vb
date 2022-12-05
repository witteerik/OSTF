
Public Class HearingAidGainData

    Public Property Name As String = ""

    Public LeftSideGain As New List(Of GainPoint)
    Public RightSideGain As New List(Of GainPoint)

    Public Class GainPoint
        Public Frequency As Integer
        Public Gain As Double
    End Class

    ''' <summary>
    ''' Returns the gain values for the corresponding left side for frequency values
    ''' </summary>
    ''' <returns></returns>
    Public Function GetLeftSideGain() As Single()
        Dim Output As New List(Of Single)
        For Each GainPoint In LeftSideGain
            Output.Add(GainPoint.Gain)
        Next
        Return Output.ToArray
    End Function

    ''' <summary>
    ''' Returns the gain values for the corresponding right side for frequency values
    ''' </summary>
    ''' <returns></returns>
    Public Function GetRightSideGain() As Single()
        Dim Output As New List(Of Single)
        For Each GainPoint In RightSideGain
            Output.Add(GainPoint.Gain)
        Next
        Return Output.ToArray
    End Function

    ''' <summary>
    ''' Returns the frequency values for the corresponding left side for gain values
    ''' </summary>
    ''' <returns></returns>
    Public Function GetLeftSideFrequencies() As Single()
        Dim Output As New List(Of Single)
        For Each GainPoint In LeftSideGain
            Output.Add(GainPoint.Frequency)
        Next
        Return Output.ToArray
    End Function

    ''' <summary>
    ''' Returns the frequency values for the corresponding right side for gain values
    ''' </summary>
    ''' <returns></returns>
    Public Function GetRightSideFrequencies() As Single()
        Dim Output As New List(Of Single)
        For Each GainPoint In RightSideGain
            Output.Add(GainPoint.Frequency)
        Next
        Return Output.ToArray
    End Function


    Public Shared Function CreateNewNoGainData()

        Dim Output = New HearingAidGainData

        'Setting gain arrays to 0 for all frequencies
        For Each f In Audio.DSP.SiiCriticalBands.CentreFrequencies
            Output.LeftSideGain.Add(New GainPoint With {.Frequency = f, .Gain = 0})
            Output.RightSideGain.Add(New GainPoint With {.Frequency = f, .Gain = 0})
        Next

        Return Output

    End Function

    Public Shared Function CreateNewFig6GainData(ByRef AudiogramData As AudiogramData, ByVal ReferenceLevel As Double) As HearingAidGainData

        Dim Output = New HearingAidGainData

        'Calculating critical band thresholds if not calculated
        If AudiogramData.Cb_Left_AC.Length = 0 Or AudiogramData.Cb_Right_AC.Length = 0 Then
            If AudiogramData.CalculateCriticalBandValues() = False Then
                Return Nothing
            End If
        End If

        'Calculating Fig6 for each critical band
        For n = 0 To Audio.DSP.SiiCriticalBands.CentreFrequencies.Length - 1
            Output.LeftSideGain.Add(New GainPoint With {.Frequency = Audio.DSP.SiiCriticalBands.CentreFrequencies(n), .Gain = Output.GetFig6Interpolation(AudiogramData.Cb_Left_AC(n), ReferenceLevel)})
        Next

        For n = 0 To Audio.DSP.SiiCriticalBands.CentreFrequencies.Length - 1
            Output.RightSideGain.Add(New GainPoint With {.Frequency = Audio.DSP.SiiCriticalBands.CentreFrequencies(n), .Gain = Output.GetFig6Interpolation(AudiogramData.Cb_Right_AC(n), ReferenceLevel)})
        Next

        Return Output

    End Function


    Private Enum Fig6LevelTypes
        Lowlevel40
        MidLevel65
        HighLevel95
    End Enum

    Private Function GetFig6Interpolation(ByVal HearingLoss As Double, ByVal ReferenceLevel As Double) As Double


        'Interpolating gain for the ReferenceLevel linearly based on the gain for low (40 dB), mid (65 dB) and high (95 dB) levels 

        If ReferenceLevel < 40 Then

            Return GetFig6Point(HearingLoss, Fig6LevelTypes.Lowlevel40)

        ElseIf ReferenceLevel > 95 Then

            Return GetFig6Point(HearingLoss, Fig6LevelTypes.HighLevel95)

        Else

            If ReferenceLevel < 65 Then

                Dim LowLevelValue = GetFig6Point(HearingLoss, Fig6LevelTypes.Lowlevel40)
                Dim MidLevelValue = GetFig6Point(HearingLoss, Fig6LevelTypes.MidLevel65)

                Return LowLevelValue + (MidLevelValue - LowLevelValue) * (ReferenceLevel - 40) / 45

            Else

                'I.e => 65
                Dim MidLevelValue = GetFig6Point(HearingLoss, Fig6LevelTypes.MidLevel65)
                Dim HighLevelValue = GetFig6Point(HearingLoss, Fig6LevelTypes.HighLevel95)

                Return MidLevelValue + (HighLevelValue - MidLevelValue) * (ReferenceLevel - 65) / 30

            End If
        End If

    End Function

    Private Function GetFig6Point(ByVal HearingLoss As Double, LevelType As Fig6LevelTypes) As Double

        Select Case LevelType
            Case Fig6LevelTypes.Lowlevel40
                If HearingLoss < 20 Then
                    Return 0
                ElseIf HearingLoss < 60 Then
                    Return HearingLoss - 20
                Else
                    Return 0.5 * HearingLoss + 10

                    'Or as sometimes written (equivalent)
                    'Return HearingLoss - 20 - 0.5 * (HearingLoss - 60)
                End If

            Case Fig6LevelTypes.MidLevel65
                If HearingLoss < 20 Then
                    Return 0
                ElseIf HearingLoss < 60 Then
                    Return 0.6 * (HearingLoss - 20)
                Else
                    Return 0.8 * HearingLoss - 23
                End If

            Case Fig6LevelTypes.HighLevel95
                If HearingLoss < 40 Then
                    Return 0
                Else
                    Return 0.1 * (HearingLoss - 40) ^ 1.4
                End If

            Case Else
                Throw New ArgumentException("Incorrect LevelType")
        End Select


    End Function

    ''' <summary>
    ''' Checks the gain data and returns True if at least one GainPoint has a non-zero gain value. Otherwise, returns False.
    ''' </summary>
    ''' <returns></returns>
    Public Function HasGain() As Boolean

        For Each GainPoint In LeftSideGain
            If GainPoint.Gain <> 0 Then Return True
        Next

        For Each GainPoint In RightSideGain
            If GainPoint.Gain <> 0 Then Return True
        Next

        Return False

    End Function

    Public Overrides Function ToString() As String

        If Name = "" Then
            Return DateTime.Now.ToShortDateString & ": " & DateTime.Now.ToShortTimeString
        Else
            Return Name
        End If

    End Function

End Class

