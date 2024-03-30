Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        STFN.UseOptimizationLibraries = True

        Dim OutputText As New List(Of String)

        For startIndex = 0 To 99

            For Length = 1 To 100 - startIndex


                Dim TimeSpanList As New List(Of String)
                Dim StopWatch As New Stopwatch
                StopWatch.Start()

                Dim rnd As New Random(42)
                Dim ArrayLength = 100 '10 ^ 8
                Dim Array1(ArrayLength - 1) As Single
                Dim Array2(ArrayLength - 1) As Single
                For i = 0 To ArrayLength - 1
                    Array1(i) = 2 ' rnd.NextDouble
                    Array2(i) = rnd.NextDouble
                Next


                StopWatch.Stop()
                TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
                StopWatch.Reset()
                StopWatch.Start()

                'Dim SoS1 = STFN.Utils.Math.CalculateSumOfSquare(Array1)

                StopWatch.Stop()
                TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
                StopWatch.Reset()
                StopWatch.Start()


                Dim SoS2 = STFN.Utils.CalculateSumOfSquare(Array1, startIndex, Length)

                StopWatch.Stop()
                TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
                StopWatch.Reset()
                StopWatch.Start()

                Dim SoS3 = STFN.LibOstfDsp_VB.CalculateSumOfSquare(Array1, startIndex, Length)

                OutputText.Add(startIndex & vbTab & Length & vbTab & SoS2 & vbTab & SoS3 & vbCrLf)

                StopWatch.Stop()
                TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
                StopWatch.Reset()

                'Testing using SIMD

                'Dim f As Single = 1
                'Dim Array4 = Array1.Select(Function(x) x * f)

                If SoS2 = SoS3 = False Then

                    MsgBox(String.Join(vbCrLf, TimeSpanList))

                End If

            Next

        Next

        TextBox1.Text = String.Concat(OutputText)


    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        STFN.UseOptimizationLibraries = True

        Dim TimeSpanList As New List(Of String)
        Dim StopWatch As New Stopwatch
        StopWatch.Start

        Dim factor As Single = 2.87654

        Dim rnd As New Random(42)
        Dim ArrayLength = 10 ^ 8
        Dim Array1(ArrayLength - 1) As Single
        Dim Array2(ArrayLength - 1) As Single
        Dim Array3(ArrayLength - 1) As Single
        Dim Array4(ArrayLength - 1) As Single
        For i = 0 To ArrayLength - 1
            Array1(i) = rnd.NextDouble
            Array2(i) = rnd.NextDouble
            Array3(i) = Array1(i)
            Array4(i) = Array1(i)
        Next

        StopWatch.Stop
        'TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
        StopWatch.Reset
        StopWatch.Start

        'Dim SoS1 = STFN.Utils.Math.CalculateSumOfSquare(Array1)

        'STFN.Utils.Math.MultiplyArray(Array1, factor)

        'STFN.Utils.Math.MultiplyArray(Array1, factor)
        STFN.Utils.AddTwoArrays(Array1, Array2)

        StopWatch.Stop
        TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
        StopWatch.Reset
        StopWatch.Start

        'Dim SoS2 = STFN.Utils.CalculateSumOfSquare(Array1, 0, Array1.Length)

        'STFN.Utils.Math.MultiplyArray(Array4, factor, 0, Array4.Length - 1)

        For s = 0 To Array4.Length - 1
            Array4(s) += Array2(s)
        Next

        StopWatch.Stop
        TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
        StopWatch.Reset
        StopWatch.Start

        'Dim SoS3 = STFN.LibOstfDsp_VB.CalculateSumOfSquare(Array1, Array1.Length, 0, Array1.Length)

        'STFN.LibOstfDsp_VB.MultiplyArraySection(Array3, Array3.Length, factor, 0, Array3.Length - 1)

        STFN.LibOstfDsp_VB.AddTwoFloatArrays(Array3, Array2)

        StopWatch.Stop
        TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
        StopWatch.Reset

        'Testing using SIMD

        'Dim Diffs As Integer = 0
        'For j = 0 To Array1.Length - 1
        '    If (Array1(j) = Array3(j)) = False Then Diffs += 1
        'Next

        'Dim f As Single = 1
        'Dim Array4 = Array1.Select(Function(x) x * f)

        'MsgBox(Array1.Last & vbTab & Array3.Last & vbTab & Array1.Sum & vbTab & Array3.Sum)
        'MsgBox(Array1.First & vbTab & Array3.First)

        'MsgBox(Array1.Last & vbTab & Array4.Last & vbTab & Array3.Last)
        'MsgBox(Array1.First & vbTab & Array4.First & vbTab & Array3.First)

        'If Array1.Last <> Array2.Last Or Array1.Last <> Array3.Last Then
        '    MsgBox("Not equal")
        'End If,

        'If Array1.First <> Array2.First Or Array1.First <> Array3.First Then
        '    MsgBox("Not equal")
        'End If


        MsgBox(String.Join(vbCrLf, TimeSpanList))


    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        STFN.Utils.SendInfoToLog(String.Join(vbCrLf, STFN.Audio.DSP.GetArrays(2 ^ 12, STFN.Audio.DSP.FftDirections.Forward).Item1))

        Dim Length As Integer = 2 ^ 13 - 1
        Dim Loops = 1

        Dim xx(Length) As Double
        Dim yy(Length) As Double

        Dim x1(Length) As Double
        Dim y1(Length) As Double
        Dim x2(Length) As Double
        Dim y2(Length) As Double
        Dim rnd As New Random(42)

        For i = 0 To x1.Length - 1
            x1(i) = rnd.NextDouble
            x2(i) = x1(i)
        Next

        'STFN.Audio.DSP.GetRadix2TrigonomerticValues(1024, STFN.Audio.DSP.TransformationsExt.FftDirections.Forward, True)

        STFN.Audio.DSP.FftRadix2_TrigDict(xx, yy, STFN.Audio.DSP.FftDirections.Backward)

        Dim TimeSpanList As New List(Of String)
        Dim StopWatch As New Stopwatch
        StopWatch.Start()

        For i = 0 To Loops - 1
            STFN.Audio.DSP.FftRadix2(x1, y1, STFN.Audio.DSP.FftDirections.Backward)
        Next

        StopWatch.Stop()
        TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
        StopWatch.Reset()
        StopWatch.Start()

        For i = 0 To Loops - 1
            STFN.Audio.DSP.FftRadix2_TrigDict(x2, y2, STFN.Audio.DSP.FftDirections.Backward)
        Next

        StopWatch.Stop()
        TimeSpanList.Add(StopWatch.ElapsedMilliseconds)
        StopWatch.Reset()

        Dim Diffs = 0
        For j = 0 To x1.Length - 1
            If x1(j) = x2(j) = False Then Diffs += 1
        Next


        MsgBox(Diffs & " " & String.Join(vbCrLf, TimeSpanList))

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

        STFN.Audio.Sound.LoadWaveFile("C:\Temp400\M_000_000_blund.wav")

    End Sub
End Class
