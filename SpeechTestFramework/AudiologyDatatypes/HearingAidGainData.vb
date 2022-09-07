
Public Class HearingAidGainData

    Public Property Name As String = ""

    Public LeftSideGain() As Single
    Public RightSideGain() As Single

    ''' <summary>
    ''' Critical band centre frequencies according to table 1 in ANSI S3.5-1997
    ''' </summary>
    Public Frequencies() As Single = {150, 250, 350, 450, 570, 700, 840, 1000, 1170, 1370, 1600, 1850, 2150, 2500, 2900, 3400, 4000, 4800, 5800, 7000, 8500}


    Public Shared Function CreateNewNoGainData()

        Dim Output = New HearingAidGainData

        'Setting gain arrays to 0 for all frequencies
        Output.LeftSideGain = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        Output.RightSideGain = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}

        Return Output

    End Function

    Public Shared Function CreateNewFig6GainData(ByRef AudiogramData As AudiogramData, ByVal ReferenceLevel As Double)

        Dim Output = New HearingAidGainData

        'Calculating critical band thresholds if not calculated
        If AudiogramData.Cb_Left_AC.Length = 0 Or AudiogramData.Cb_Right_AC.Length = 0 Then
            AudiogramData.CalculateCriticalBandValues()
        End If

        'Calculating Fig6 for each critical band
        Output.LeftSideGain = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        For n = 0 To Output.LeftSideGain.Length - 1
            Output.LeftSideGain(n) = Output.GetFig6Interpolation(AudiogramData.Cb_Left_AC(n), ReferenceLevel)
        Next

        Output.RightSideGain = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        For n = 0 To Output.LeftSideGain.Length - 1
            Output.RightSideGain(n) = Output.GetFig6Interpolation(AudiogramData.Cb_Right_AC(n), ReferenceLevel)
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

    Public Overrides Function ToString() As String

        If Name = "" Then
            Return DateTime.Now.ToShortDateString & ": " & DateTime.Now.ToShortTimeString
        Else
            Return Name
        End If

    End Function

End Class

