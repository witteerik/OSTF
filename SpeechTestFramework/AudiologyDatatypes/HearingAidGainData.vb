
Public Class HearingAidGainData

    Public Enum GainTypes
        NoGain
        Fig6
        Measured
    End Enum

    Public ReadOnly GainType As GainTypes

    Public Enum Sides
        Left
        Right
    End Enum

    Public LeftSideGain() As Single
    Public RightSideGain() As Single
    ''' <summary>
    ''' Critical band centre frequencies according to table 1 in ANSI S3.5-1997
    ''' </summary>
    Public Frequencies() As Single = {150, 250, 350, 450, 570, 700, 840, 1000, 1170, 1370, 1600, 1850, 2150, 2500, 2900, 3400, 4000, 4800, 5800, 7000, 8500}

    ''' <summary>
    ''' Returns a list of available gain types. As more types are implemented, this function should be modified!
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetAvailableGainTypes() As List(Of GainTypes)

        Return New List(Of GainTypes) From {GainTypes.NoGain, GainTypes.Fig6}

    End Function

    ''' <summary>
    ''' Creates a new instance of HearingAidGainData. Note that you have to call one of the overloads of CalculateGain() before gain values can be retreived!
    ''' </summary>
    ''' <param name="GainType"></param>
    Public Sub New(ByVal GainType As GainTypes)

        Me.GainType = GainType

    End Sub

    ''' <summary>
    ''' Calculates hearing-aid gain for the SII critical band frequencies based on real-ear measurement data.
    ''' </summary>
    ''' <param name="MeasuresData"></param>
    Public Sub CalculateGain(ByRef MeasuresData As RealEarData)

        If Me.GainType <> GainTypes.Measured Then Throw New Exception("You probably called the incorrect overlaod of CalculateGain")

        Throw New NotImplementedException("Real-ear gain in not yet supported.")

        'TODO, implement real-ear gain support!

    End Sub

    ''' <summary>
    ''' Calculates hearing-aid gain for the SII critical band frequencies based on audiogram data.
    ''' </summary>
    ''' <param name="AudiogramData"></param>
    ''' <param name="ReferenceLevel"></param>
    Public Sub CalculateGain(ByRef AudiogramData As AudiogramData, ByVal ReferenceLevel As Double)

        Select Case GainType
            Case GainTypes.Measured
                Throw New Exception("You probably called the incorrect overlaod of CalculateGain")
            Case GainTypes.NoGain

                'Setting gain arrays to 0 for all frequencies
                LeftSideGain = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                RightSideGain = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}

            Case GainTypes.Fig6

                'Calculating critical band thresholds if not calculated
                If AudiogramData.Cb_Left_AC.Length = 0 Or AudiogramData.Cb_Right_AC.Length = 0 Then
                    AudiogramData.CalculateCriticalBandValues()
                End If

                'Calculating Fig6 for each critical band
                LeftSideGain = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                For n = 0 To LeftSideGain.Length - 1
                    LeftSideGain(n) = GetFig6Interpolation(AudiogramData.Cb_Left_AC(n), ReferenceLevel)
                Next

                RightSideGain = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                For n = 0 To LeftSideGain.Length - 1
                    RightSideGain(n) = GetFig6Interpolation(AudiogramData.Cb_Right_AC(n), ReferenceLevel)
                Next

            Case Else

                Throw New NotImplementedException("Unimplemented gain type")

        End Select

    End Sub


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
    ''' This class holds measurement data from a real-ear measurement
    ''' </summary>
    Public Class RealEarData



    End Class


End Class

