Imports System.Xml.Serialization

<Serializable>
Public Class AudiogramData

    Public Property Name As String = ""

    Public Property AC_Right As New List(Of TonePoint)
    Public Property AC_Left As New List(Of TonePoint)
    Public Property BC_Right As New List(Of TonePoint)
    Public Property BC_Left As New List(Of TonePoint)

    Public Property AC_Right_Masked As New List(Of TonePoint)
    Public Property AC_Left_Masked As New List(Of TonePoint)
    Public Property BC_Right_Masked As New List(Of TonePoint)
    Public Property BC_Left_Masked As New List(Of TonePoint)

    Public Property UCL_Left As New List(Of TonePoint)
    Public Property UCL_Right As New List(Of TonePoint)

    Public Property Cb_Left_AC As Double() = {}
    Public Property Cb_Left_BC As Double() = {}
    Public Property Cb_Right_AC As Double() = {}
    Public Property Cb_Right_BC As Double() = {}

    Public Property Cb_Left_UCL As Double() = {}
    Public Property Cb_Right_UCL As Double() = {}

    <Serializable>
    Public Class TonePoint
        Property StimulusFrequency As Integer
        Property StimulusLevel As Integer
        Property Overheard As Boolean = False
        Property NoResponse As Boolean = False
    End Class

    Private _BPTAH As Double?
    Public ReadOnly Property BPTAH As Double?
        Get
            If _BPTAH.HasValue = False Then
                UpdateBPTAH()
            End If
            Return _BPTAH
        End Get
    End Property

    ''' <summary>
    ''' Updates the value returned by the ReadOnly Property BPTAH, based on interpolation of existing audiogram points.
    ''' </summary>
    Public Sub UpdateBPTAH()

        Dim LeftPTAH As Double? = Nothing
        Dim RightPTAH As Double? = Nothing

        If AC_Left.Count > 0 Or AC_Left_Masked.Count > 0 Then
            Dim Left_AC = GetFullAudiogramPointSerie(AC_Left_Masked, AC_Left)
            LeftPTAH = (Left_AC(3000).StimulusLevel + Left_AC(4000).StimulusLevel + Left_AC(6000).StimulusLevel) / 3
        End If

        If AC_Right.Count > 0 Or AC_Right_Masked.Count > 0 Then
            Dim Right_AC = GetFullAudiogramPointSerie(AC_Right_Masked, AC_Right)
            RightPTAH = (Right_AC(3000).StimulusLevel + Right_AC(4000).StimulusLevel + Right_AC(6000).StimulusLevel) / 3
        End If

        Dim Local_BPTAH As Double? = Nothing
        If LeftPTAH.HasValue And RightPTAH.HasValue Then
            Local_BPTAH = Math.Min(LeftPTAH.Value, RightPTAH.Value)
        ElseIf LeftPTAH.HasValue = True And RightPTAH.HasValue = False Then
            Local_BPTAH = LeftPTAH.Value
        ElseIf LeftPTAH.HasValue = False And RightPTAH.HasValue = True Then
            Local_BPTAH = RightPTAH.Value
        Else
            Local_BPTAH = Nothing
        End If

        _BPTAH = Local_BPTAH

    End Sub

    Private _PTA3_L As Double?
    Public ReadOnly Property PTA3_L As Double?
        Get
            If _PTA3_L.HasValue = False Then
                UpdatePTA3_L()
            End If
            Return _PTA3_L
        End Get
    End Property

    ''' <summary>
    ''' Updates the value returned by the ReadOnly Property PTA3_L, based on interpolation of existing audiogram points.
    ''' </summary>
    Public Sub UpdatePTA3_L()

        If AC_Left.Count > 0 Or AC_Left_Masked.Count > 0 Then
            Dim Left_AC = GetFullAudiogramPointSerie(AC_Left_Masked, AC_Left)
            _PTA3_L = (Left_AC(500).StimulusLevel + Left_AC(1000).StimulusLevel + Left_AC(2000).StimulusLevel) / 3
        End If

    End Sub

    ''' <summary>
    ''' Can be used to set PTA3_L when there are no audiogram points stored.
    ''' </summary>
    ''' <param name="NewValue"></param>
    Public Sub OverridePTA3_L(ByVal NewValue As Double)
        _PTA3_L = NewValue
    End Sub

    Private _PTA3_R As Double?
    Public ReadOnly Property PTA3_R As Double?
        Get
            If _PTA3_R.HasValue = False Then
                UpdatePTA3_R()
            End If
            Return _PTA3_R
        End Get
    End Property

    ''' <summary>
    ''' Updates the value returned by the ReadOnly Property PTA3_R, based on interpolation of existing audiogram points.
    ''' </summary>
    Public Sub UpdatePTA3_R()

        If AC_Right.Count > 0 Or AC_Right_Masked.Count > 0 Then
            Dim Right_AC = GetFullAudiogramPointSerie(AC_Right_Masked, AC_Right)
            _PTA3_R = (Right_AC(500).StimulusLevel + Right_AC(1000).StimulusLevel + Right_AC(2000).StimulusLevel) / 3
        End If

    End Sub

    ''' <summary>
    ''' Can be used to set PTA3_R when there are no audiogram points stored.
    ''' </summary>
    ''' <param name="NewValue"></param>
    Public Sub OverridePTA3_R(ByVal NewValue As Double)
        _PTA3_R = NewValue
    End Sub


    'A debugging function that creates random audiogram data in the current instance of AudiogramData
    Public Sub CreateRandomAudiogramData(Optional ByVal IncludeUclData As Boolean = False)

        Dim fs() As Integer = {125, 250, 500, 750, 1000, 1500, 2000, 3000, 4000, 6000, 8000}

        Dim rnd As New Random

        For Each f In fs
            AC_Right.Add(New TonePoint With {.StimulusFrequency = f, .StimulusLevel = 5 * rnd.Next(0, 18)})
            AC_Left.Add(New TonePoint With {.StimulusFrequency = f, .StimulusLevel = 5 * rnd.Next(0, 18)})
            BC_Right.Add(New TonePoint With {.StimulusFrequency = f, .StimulusLevel = 5 * rnd.Next(0, 18)})
            BC_Left.Add(New TonePoint With {.StimulusFrequency = f, .StimulusLevel = 5 * rnd.Next(0, 18)})
            AC_Right_Masked.Add(New TonePoint With {.StimulusFrequency = f, .StimulusLevel = 5 * rnd.Next(0, 18)})
            AC_Left_Masked.Add(New TonePoint With {.StimulusFrequency = f, .StimulusLevel = 5 * rnd.Next(0, 18)})
            BC_Right_Masked.Add(New TonePoint With {.StimulusFrequency = f, .StimulusLevel = 5 * rnd.Next(0, 18)})
            BC_Left_Masked.Add(New TonePoint With {.StimulusFrequency = f, .StimulusLevel = 5 * rnd.Next(0, 18)})
            If IncludeUclData = True Then
                UCL_Left.Add(New TonePoint With {.StimulusFrequency = f, .StimulusLevel = 5 * rnd.Next(14, 18)})
                UCL_Right.Add(New TonePoint With {.StimulusFrequency = f, .StimulusLevel = 5 * rnd.Next(14, 18)})
            End If
        Next

    End Sub

    Public Enum BisgaardAudiograms
        NH
        N1
        N2
        N3
        N4
        N5
        N6
        N7
        S1
        S2
        S3
    End Enum

    Public Enum DebuggingAudiograms
        Flat10
        Flat50
        Flat85
        Flat95
        PlusFivePerFrequency
    End Enum

    ''' <summary>
    ''' Creating Bisgaard type audiograms.
    ''' </summary>
    ''' <param name="AudiogramType"></param>
    ''' <param name="Include375Hz">Set to true to include values for 375 Hz.</param>
    ''' <param name="AirBoneGap">An optional list of air-bone gaps. The list can either contain 1 value, repressenting a global air-bone-gap, or one value per frequency (including 125 and 8k Hz)</param>
    Public Sub CreateTypicalAudiogramData(ByVal AudiogramType As BisgaardAudiograms,
                                              Optional ByVal Include375Hz As Boolean = False,
                                              Optional ByVal AirBoneGap As List(Of Double) = Nothing,
                                              Optional IncludeMasking As Boolean = False)

        ' Audiograms from Bisgaard et al (2010) Standard Audiograms... Trends In Amplification
        ' 125 kHz are Set To the same As Bisgaard's 250 kHz value
        ' 8 kHz are Set To the same As Bisgaard's 6 kHz value

        'Setting a name based on the audiogram type
        Name = AudiogramType.ToString

        Dim fs() As Integer = {125, 250, 375, 500, 750, 1000, 1500, 2000, 3000, 4000, 6000, 8000}

        Dim TempAirBonegapArray(fs.Length - 1) As Double
        If AirBoneGap IsNot Nothing Then

            If AirBoneGap.Count = fs.Length - 1 Then

                For n = 1 To fs.Length - 1
                    AirBoneGap.Add(AirBoneGap(0))
                Next

            ElseIf AirBoneGap.Count = fs.Length - 1 Then

                'Nothing needs to be done here

            ElseIf AirBoneGap.Count = fs.Length - 2 Then

                'Inserts a 0 into AirBoneGap at the index corresponding to the missing air-bone-gap value for 375 Hz.
                AirBoneGap.Insert(2, 0)

            Else
                Throw New ArgumentException("The number of items in AirBoneGap must be either 11 or 12 (depending on whether 375 Hz is included).")
            End If

            TempAirBonegapArray = AirBoneGap.ToArray

        End If

        Dim TempAudiograms As New SortedList(Of BisgaardAudiograms, Double())

        TempAudiograms.Add(BisgaardAudiograms.NH, {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0})
        TempAudiograms.Add(BisgaardAudiograms.N1, {10, 10, 10, 10, 10, 10, 10, 15, 20, 30, 40, 40})
        TempAudiograms.Add(BisgaardAudiograms.N2, {20, 20, 20, 20, 22.5, 25, 30, 35, 40, 45, 50, 50})
        TempAudiograms.Add(BisgaardAudiograms.N3, {35, 35, 35, 35, 35, 40, 45, 50, 55, 60, 65, 65})
        TempAudiograms.Add(BisgaardAudiograms.N4, {55, 55, 55, 55, 55, 55, 60, 65, 70, 75, 80, 80})
        TempAudiograms.Add(BisgaardAudiograms.N5, {65, 65, 67.5, 70, 72.5, 75, 80, 80, 80, 80, 80, 80})
        TempAudiograms.Add(BisgaardAudiograms.N6, {75, 75, 77.5, 80, 82.5, 85, 90, 90, 95, 100, 100, 100})
        TempAudiograms.Add(BisgaardAudiograms.N7, {90, 90, 92.5, 95, 100, 105, 105, 105, 105, 105, 105, 105})
        TempAudiograms.Add(BisgaardAudiograms.S1, {10, 10, 10, 10, 10, 10, 10, 15, 30, 55, 70, 70})
        TempAudiograms.Add(BisgaardAudiograms.S2, {20, 20, 20, 20, 22.5, 25, 35, 55, 75, 95, 95, 95})
        TempAudiograms.Add(BisgaardAudiograms.S3, {30, 30, 30, 35, 47.5, 60, 70, 75, 80, 80, 85, 85})

        Dim SelectedAudiogram = TempAudiograms(AudiogramType)

        For n = 0 To fs.Length - 1

            If Include375Hz = False Then
                If fs(n) = 375 Then
                    Continue For
                End If
            End If

            AC_Right.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n)})
            AC_Left.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n)})
            BC_Right.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n) + TempAirBonegapArray(n)})
            BC_Left.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n) + TempAirBonegapArray(n)})

            If IncludeMasking = True Then

                AC_Right_Masked.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n)})
                AC_Left_Masked.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n)})
                BC_Right_Masked.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n) + TempAirBonegapArray(n)})
                BC_Left_Masked.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n) + TempAirBonegapArray(n)})
            End If

        Next

        InterpolateCriticalBandValues()

    End Sub


    ''' <summary>
    ''' Creating audiograms usful for debugging of code.
    ''' </summary>
    ''' <param name="AudiogramType"></param>
    ''' <param name="Include375Hz">Set to true to include values for 375 Hz.</param>
    ''' <param name="AirBoneGap">An optional list of air-bone gaps. The list can either contain 1 value, repressenting a global air-bone-gap, or one value per frequency (including 125 and 8k Hz)</param>
    Public Sub CreateDebuggingAudiogramData(ByVal AudiogramType As DebuggingAudiograms,
                                              Optional ByVal Include375Hz As Boolean = False,
                                              Optional ByVal AirBoneGap As List(Of Double) = Nothing,
                                              Optional IncludeMasking As Boolean = False)

        'Setting a name based on the audiogram type
        Name = AudiogramType.ToString

        Dim fs() As Integer = {125, 250, 375, 500, 750, 1000, 1500, 2000, 3000, 4000, 6000, 8000}

        Dim TempAirBonegapArray(fs.Length - 1) As Double
        If AirBoneGap IsNot Nothing Then

            If AirBoneGap.Count = fs.Length - 1 Then

                For n = 1 To fs.Length - 1
                    AirBoneGap.Add(AirBoneGap(0))
                Next

            ElseIf AirBoneGap.Count = fs.Length - 1 Then

                'Nothing needs to be done here

            ElseIf AirBoneGap.Count = fs.Length - 2 Then

                'Inserts a 0 into AirBoneGap at the index corresponding to the missing air-bone-gap value for 375 Hz.
                AirBoneGap.Insert(2, 0)

            Else
                Throw New ArgumentException("The number of items in AirBoneGap must be either 11 or 12 (depending on whether 375 Hz is included).")
            End If

            TempAirBonegapArray = AirBoneGap.ToArray

        End If

        Dim TempAudiograms As New SortedList(Of DebuggingAudiograms, Double())

        TempAudiograms.Add(DebuggingAudiograms.Flat10, {10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10})
        TempAudiograms.Add(DebuggingAudiograms.Flat50, {50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50})
        TempAudiograms.Add(DebuggingAudiograms.Flat85, {85, 85, 85, 85, 85, 85, 85, 85, 85, 85, 85, 85})
        TempAudiograms.Add(DebuggingAudiograms.Flat95, {95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95})
        TempAudiograms.Add(DebuggingAudiograms.PlusFivePerFrequency, {0, 5, 10, 15, 20, 25, 30, 45, 50, 55, 60, 65})

        Dim SelectedAudiogram = TempAudiograms(AudiogramType)

        For n = 0 To fs.Length - 1

            If Include375Hz = False Then
                If fs(n) = 375 Then
                    Continue For
                End If
            End If

            AC_Right.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n)})
            AC_Left.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n)})
            BC_Right.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n) + TempAirBonegapArray(n)})
            BC_Left.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n) + TempAirBonegapArray(n)})

            If IncludeMasking = True Then

                AC_Right_Masked.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n)})
                AC_Left_Masked.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n)})
                BC_Right_Masked.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n) + TempAirBonegapArray(n)})
                BC_Left_Masked.Add(New TonePoint With {.StimulusFrequency = fs(n), .StimulusLevel = SelectedAudiogram(n) + TempAirBonegapArray(n)})
            End If

        Next

        InterpolateCriticalBandValues()

    End Sub

    Public Sub CreateIncompleteAudiogramData(ByVal AudiogramType As BisgaardAudiograms, Optional ByVal PointsToRemove As Integer = 3)

        CreateTypicalAudiogramData(AudiogramType,,, True)

        Dim MyRandom = New Random
        For n = 1 To PointsToRemove
            _AC_Left.RemoveAt(MyRandom.Next(AC_Left.Count))
            _AC_Right.RemoveAt(MyRandom.Next(AC_Right.Count))
            _AC_Left_Masked.RemoveAt(MyRandom.Next(AC_Left_Masked.Count))
            _AC_Right_Masked.RemoveAt(MyRandom.Next(AC_Right_Masked.Count))

        Next

        InterpolateCriticalBandValues()

    End Sub

    ''' <summary>
    ''' Interpolates the levels (thresholds, etc) stored for the audiogram frequencies to the SII critical band frequencies by linear interpolation.
    ''' </summary>
    ''' <returns></returns>
    Public Function InterpolateCriticalBandValues() As Boolean

        'Overriding the opposite side if AC is not given for both sides
        If AC_Left.Count > 0 And AC_Right.Count = 0 Then
            For Each Point In AC_Left
                AC_Right.Add(New TonePoint With {.StimulusFrequency = Point.StimulusFrequency, .StimulusLevel = Point.StimulusLevel, .NoResponse = Point.NoResponse, .Overheard = Point.Overheard})
            Next
        End If
        If AC_Right.Count > 0 And AC_Left.Count = 0 Then
            For Each Point In AC_Right
                AC_Left.Add(New TonePoint With {.StimulusFrequency = Point.StimulusFrequency, .StimulusLevel = Point.StimulusLevel, .NoResponse = Point.NoResponse, .Overheard = Point.Overheard})
            Next
        End If

        'Calculates appropriate values for AC and BC for left side.
        Dim Left_AC = GetFullAudiogramPointSerie(AC_Left_Masked, AC_Left)

        'Clears the Cb arrays and returns False if unsuccessful (which will happen when too few audiogram points exist)
        If Left_AC Is Nothing Then
            Cb_Left_AC = {}
            Cb_Left_BC = {}
            Cb_Left_UCL = {}
            Cb_Right_AC = {}
            Cb_Right_BC = {}
            Cb_Right_UCL = {}
            Return False
        End If

        Dim Left_BC = GetFullAudiogramPointSerie(BC_Left_Masked, BC_Left)
        If Left_BC Is Nothing Then Left_BC = New SortedList(Of Integer, TonePoint)

        'Overrides lacking BC-data with the AC data
        Left_BC = GetFullAudiogramPointSerie(Left_BC.Values.ToList, Left_AC.Values.ToList)


        'Calculates appropriate values for AC and BC for right side.
        Dim Right_AC = GetFullAudiogramPointSerie(AC_Right_Masked, AC_Right)

        'Clears the Cb arrays and returns False if unsuccessful (which will happen when too few audiogram points exist)
        If Right_AC Is Nothing Then
            Cb_Left_AC = {}
            Cb_Left_BC = {}
            Cb_Right_AC = {}
            Cb_Right_BC = {}
            Return False
        End If

        Dim Right_BC = GetFullAudiogramPointSerie(BC_Right_Masked, BC_Right)
        If Right_BC Is Nothing Then Right_BC = New SortedList(Of Integer, TonePoint)

        'Overrides lacking BC-data with the AC data
        Right_BC = GetFullAudiogramPointSerie(Right_BC.Values.ToList, Right_AC.Values.ToList)



        'Calculates critical band values
        If Left_AC IsNot Nothing And Left_BC IsNot Nothing Then
            'Interpolates the audiogram frequency values to critical band values
            Cb_Left_AC = InterpolateAudiogramToCriticalBandFrequencies(Left_AC)
            Cb_Left_BC = InterpolateAudiogramToCriticalBandFrequencies(Left_BC)
        Else
            Cb_Left_AC = {}
            Cb_Left_BC = {}
        End If

        If Right_AC IsNot Nothing And Right_BC IsNot Nothing Then
            'Interpolates the audiogram frequency values to critical band values
            Cb_Right_AC = InterpolateAudiogramToCriticalBandFrequencies(Right_AC)
            Cb_Right_BC = InterpolateAudiogramToCriticalBandFrequencies(Right_BC)
        Else
            Cb_Right_AC = {}
            Cb_Right_BC = {}
        End If

        If UCL_Left.Count > 0 Then
            Dim Left_UCL = GetFullAudiogramPointSerie(UCL_Left, New List(Of TonePoint))

            If UCL_Left IsNot Nothing Then
                'Interpolates the audiogram frequency values to critical band values
                Cb_Left_UCL = InterpolateAudiogramToCriticalBandFrequencies(Left_UCL)
            Else
                Cb_Left_UCL = {}
            End If
        End If

        If UCL_Right.Count > 0 Then
            Dim Right_UCL = GetFullAudiogramPointSerie(UCL_Right, New List(Of TonePoint))

            If UCL_Right IsNot Nothing Then
                'Interpolates the audiogram frequency values to critical band values
                Cb_Right_UCL = InterpolateAudiogramToCriticalBandFrequencies(Right_UCL)
            Else
                Cb_Right_UCL = {}
            End If
        End If

        Return True

    End Function


    Public Enum AudiogramDecibelTypes
        dBSPL
        dBHL
    End Enum

    Public Property AudiogramDecibelType As AudiogramDecibelTypes = AudiogramDecibelTypes.dBHL


    ''' <summary>
    ''' Converts all values in the audiogram to from dB HL to dB SPL. If values are already in dB SPL no convertion is performed.
    ''' </summary>
    Public Sub ConvertToSplAudiogram()

        If AudiogramDecibelType = AudiogramDecibelTypes.dBSPL Then Exit Sub

        'Setting type SPL
        AudiogramDecibelType = AudiogramDecibelTypes.dBSPL

        For Each TonePiont In Me.AC_Left
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel += ConversionValue
        Next

        For Each TonePiont In Me.AC_Right
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel += ConversionValue
        Next

        For Each TonePiont In Me.AC_Left_Masked
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel += ConversionValue
        Next

        For Each TonePiont In Me.AC_Right_Masked
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel += ConversionValue
        Next

        For Each TonePiont In Me.BC_Left
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel += ConversionValue
        Next

        For Each TonePiont In Me.BC_Right
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel += ConversionValue
        Next

        For Each TonePiont In Me.BC_Left_Masked
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel += ConversionValue
        Next

        For Each TonePiont In Me.BC_Right_Masked
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel += ConversionValue
        Next

        For Each TonePiont In Me.UCL_Left
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel += ConversionValue
        Next

        For Each TonePiont In Me.UCL_Right
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel += ConversionValue
        Next

    End Sub

    Public Sub FillUpUCLs(Optional ByVal DefaultValue As Double = 95)

        Dim AudiogramFrequencies() As Double = {125, 250, 500, 750, 1000, 1500, 2000, 3000, 4000, 6000, 8000}

        Dim LeftSideValues As New SortedList(Of Double, TonePoint)
        Dim RightSideValues As New SortedList(Of Double, TonePoint)

        For Each TP In Me.UCL_Left
            LeftSideValues.Add(TP.StimulusFrequency, TP)
        Next

        For Each TP In Me.UCL_Right
            RightSideValues.Add(TP.StimulusFrequency, TP)
        Next

        For Each Frequency In AudiogramFrequencies
            If LeftSideValues.ContainsKey(Frequency) = False Then
                LeftSideValues.Add(Frequency, New TonePoint With {.StimulusFrequency = Frequency, .StimulusLevel = DefaultValue})
            End If

            If RightSideValues.ContainsKey(Frequency) = False Then
                RightSideValues.Add(Frequency, New TonePoint With {.StimulusFrequency = Frequency, .StimulusLevel = DefaultValue})
            End If
        Next

        Me.UCL_Left = LeftSideValues.Values.ToList
        Me.UCL_Right = RightSideValues.Values.ToList

    End Sub

    ''' <summary>
    ''' Converts all values in the audiogram to from dB SPL to dB HL. If values are already in dB HL no convertion is performed.
    ''' </summary>
    Public Sub ConvertToHLAudiogram()

        If AudiogramDecibelType = AudiogramDecibelTypes.dBHL Then Exit Sub

        'Setting type HL
        AudiogramDecibelType = AudiogramDecibelTypes.dBHL

        For Each TonePiont In Me.AC_Left
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel -= ConversionValue
        Next

        For Each TonePiont In Me.AC_Right
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel -= ConversionValue
        Next

        For Each TonePiont In Me.AC_Left_Masked
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel -= ConversionValue
        Next

        For Each TonePiont In Me.AC_Right_Masked
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel -= ConversionValue
        Next

        For Each TonePiont In Me.BC_Left
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel -= ConversionValue
        Next

        For Each TonePiont In Me.BC_Right
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel -= ConversionValue
        Next

        For Each TonePiont In Me.BC_Left_Masked
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel -= ConversionValue
        Next

        For Each TonePiont In Me.BC_Right_Masked
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel -= ConversionValue
        Next

        For Each TonePiont In Me.UCL_Left
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel -= ConversionValue
        Next

        For Each TonePiont In Me.UCL_Right
            Dim ConversionValue = Audio.GlobalAudioData.HL_2_SPL(TonePiont.StimulusFrequency)
            TonePiont.StimulusLevel -= ConversionValue
        Next

    End Sub

    <Obsolete>
    Public Function CompensateCbLevelsForInternalNoiseSpectrumLevels(ByVal Binaural As Boolean) As Boolean

        'TODO: I'm very much in doubt this function is even reasonably good. Likely it should be removed
        'TODO: this is a bad method name, come up with something better if the method is helpful...

        If Cb_Left_AC.Length = 0 Or Cb_Right_AC.Length = 0 Or Cb_Left_UCL.Length = 0 Or Cb_Right_UCL.Length = 0 Then
            Return False
        End If

        '  Reference internal noise spectrum (According to the SII standard)
        Dim X As Double() = {1.5, -3.9, -7.2, -8.9, -10.3, -11.4, -12.0, -12.5, -13.2, -14.0, -15.4, -16.9, -18.8, -21.2, -23.2, -24.9, -25.9, -24.2, -19.0, -11.7, -6.0}

        If Binaural = True Then
            For i = 0 To 20
                Cb_Left_AC(i) -= 1.7
                Cb_Right_AC(i) -= 1.7

                'And applying the same compensation to UCLs
                Cb_Left_UCL(i) -= 1.7
                Cb_Right_UCL(i) -= 1.7

            Next
        End If

        '  # Calculating Equivalent internal noise spectrum
        For i = 0 To 20
            Cb_Left_AC(i) += X(i)
            Cb_Right_AC(i) += X(i)

            'And applying the same compensation to UCLs
            Cb_Left_UCL(i) += X(i)
            Cb_Right_UCL(i) += X(i)

        Next

        Return True

    End Function




    ''' <summary>
    ''' Completes an audiogram curve through linear interpolation, based on primary and secondary audiogram points.
    ''' </summary>
    ''' <param name="PrimaryPoints">The primary points to be used. Is never overridden.</param>
    ''' <param name="SecondaryPoints">Points that are only used if no valid primary point exists for the specific frequency.</param>
    ''' <returns></returns>
    Private Function GetFullAudiogramPointSerie(ByRef PrimaryPoints As List(Of TonePoint), ByRef SecondaryPoints As List(Of TonePoint)) As SortedList(Of Integer, TonePoint)

        Dim fs() As Integer = {125, 250, 500, 750, 1000, 1500, 2000, 3000, 4000, 6000, 8000}

        'Creating an 11-point standard frequency audiogram
        Dim TempPoints As New SortedList(Of Integer, TonePoint)
        For Each tp In PrimaryPoints
            'Ignoring overheard and noresponse data points
            If tp.Overheard = True Then Continue For
            If tp.NoResponse = True Then Continue For
            'Ignores any duplicate data points, which generally should not be the case
            If TempPoints.ContainsKey(tp.StimulusFrequency) Then Continue For
            TempPoints.Add(tp.StimulusFrequency, tp)
        Next
        For Each tp In SecondaryPoints
            'Ignoring overheard and noresponse data points
            If tp.Overheard = True Then Continue For
            If tp.NoResponse = True Then Continue For
            'Ignores datapoints for which there are already a masked value
            If TempPoints.ContainsKey(tp.StimulusFrequency) Then Continue For
            TempPoints.Add(tp.StimulusFrequency, tp)
        Next

        If TempPoints.Count < 2 Then
            Return Nothing
        End If

        'Making sure we have a value at 8000, by copying the closest threshold
        If Not TempPoints.ContainsKey(8000) Then
            'Copying the closest value
            TempPoints.Add(8000, New TonePoint() With {.StimulusFrequency = 8000, .StimulusLevel = TempPoints.Values(0).StimulusLevel})
        End If
        'Making sure we have a value at 125, by copying the closest threshold
        If Not TempPoints.ContainsKey(125) Then
            'Copying the closest value
            TempPoints.Add(125, New TonePoint() With {.StimulusFrequency = 125, .StimulusLevel = TempPoints.Values(0).StimulusLevel})
        End If

        'Interpolating so that all points in fs exists
        For i = 1 To fs.Length - 1

            If Not TempPoints.ContainsKey(fs(i)) Then

                Dim LowerPoint = TempPoints(fs(i - 1))
                Dim UpperPoint = TempPoints.Values(i)
                Dim InterpolatedThreshold As Integer = Math.Round(
             LowerPoint.StimulusLevel + (UpperPoint.StimulusLevel - LowerPoint.StimulusLevel) * ((fs(i) - LowerPoint.StimulusFrequency) / (UpperPoint.StimulusFrequency - LowerPoint.StimulusFrequency)))

                TempPoints.Add(fs(i), New TonePoint With {.StimulusFrequency = fs(i), .StimulusLevel = InterpolatedThreshold})

            End If

        Next

        Return TempPoints

    End Function

    Private Function InterpolateAudiogramToCriticalBandFrequencies(ByRef AudiogramThresholds As SortedList(Of Integer, TonePoint)) As Double()

        'Dim AudiogramFrequencies() As Integer = {125, 250, 500, 750, 1000, 1500, 2000, 3000, 4000, 6000, 8000}

        ' Critical band centre frequencies according to table 1 in ANSI S3.5-1997
        'Dim CbFrequencies() As Single = {150, 250, 350, 450, 570, 700, 840, 1000, 1170, 1370, 1600, 1850, 2150, 2500, 2900, 3400, 4000, 4800, 5800, 7000, 8500}

        Dim TempList As New List(Of Double)
        TempList.Add(LinearInterpolation(150, 125, 250, AudiogramThresholds(125).StimulusLevel, AudiogramThresholds(250).StimulusLevel))
        TempList.Add(AudiogramThresholds(250).StimulusLevel)
        TempList.Add(LinearInterpolation(350, 250, 500, AudiogramThresholds(250).StimulusLevel, AudiogramThresholds(500).StimulusLevel))
        TempList.Add(LinearInterpolation(450, 250, 500, AudiogramThresholds(250).StimulusLevel, AudiogramThresholds(500).StimulusLevel))
        TempList.Add(LinearInterpolation(570, 500, 750, AudiogramThresholds(500).StimulusLevel, AudiogramThresholds(750).StimulusLevel))
        TempList.Add(LinearInterpolation(700, 500, 750, AudiogramThresholds(500).StimulusLevel, AudiogramThresholds(750).StimulusLevel))
        TempList.Add(LinearInterpolation(840, 750, 1000, AudiogramThresholds(750).StimulusLevel, AudiogramThresholds(1000).StimulusLevel))
        TempList.Add(AudiogramThresholds(1000).StimulusLevel)
        TempList.Add(LinearInterpolation(1170, 1000, 1500, AudiogramThresholds(1000).StimulusLevel, AudiogramThresholds(1500).StimulusLevel))
        TempList.Add(LinearInterpolation(1370, 1000, 1500, AudiogramThresholds(1000).StimulusLevel, AudiogramThresholds(1500).StimulusLevel))
        TempList.Add(LinearInterpolation(1600, 1500, 2000, AudiogramThresholds(1500).StimulusLevel, AudiogramThresholds(2000).StimulusLevel))
        TempList.Add(LinearInterpolation(1850, 1500, 2000, AudiogramThresholds(1500).StimulusLevel, AudiogramThresholds(2000).StimulusLevel))
        TempList.Add(LinearInterpolation(2150, 2000, 3000, AudiogramThresholds(2000).StimulusLevel, AudiogramThresholds(3000).StimulusLevel))
        TempList.Add(LinearInterpolation(2500, 2000, 3000, AudiogramThresholds(2000).StimulusLevel, AudiogramThresholds(3000).StimulusLevel))
        TempList.Add(LinearInterpolation(2900, 2000, 3000, AudiogramThresholds(2000).StimulusLevel, AudiogramThresholds(3000).StimulusLevel))
        TempList.Add(LinearInterpolation(3400, 3000, 4000, AudiogramThresholds(3000).StimulusLevel, AudiogramThresholds(4000).StimulusLevel))
        TempList.Add(AudiogramThresholds(4000).StimulusLevel)
        TempList.Add(LinearInterpolation(4800, 4000, 6000, AudiogramThresholds(4000).StimulusLevel, AudiogramThresholds(6000).StimulusLevel))
        TempList.Add(LinearInterpolation(5800, 4000, 6000, AudiogramThresholds(4000).StimulusLevel, AudiogramThresholds(6000).StimulusLevel))
        TempList.Add(LinearInterpolation(7000, 6000, 8000, AudiogramThresholds(6000).StimulusLevel, AudiogramThresholds(8000).StimulusLevel))
        TempList.Add(AudiogramThresholds(8000).StimulusLevel)

        Return TempList.ToArray

    End Function

    Public Function ContainsAcData() As Boolean

        If AC_Left.Count = 0 And AC_Right.Count = 0 Then
            Return False
        Else
            Return True
        End If

    End Function

    Public Function ContainsCbData() As Boolean

        If Cb_Left_AC.Length = 0 Or Cb_Right_AC.Length = 0 Or Cb_Left_BC.Length = 0 Or Cb_Right_BC.Length = 0 Then
            Return False
        Else
            Return True
        End If

    End Function


    ''' <summary>
    ''' Performs linear interpolation to get a value for Y.
    ''' </summary>
    Private Function LinearInterpolation(ByRef X As Double, ByVal LowerX As Double, ByVal UpperX As Double, ByVal LowerY As Double, ByVal UpperY As Double) As Double

        'Getting the linear function that fit to the points 1 and 2
        'y = kx + m
        Dim k As Double = (LowerY - UpperY) / (LowerX - UpperX)
        If k = Double.NaN Then Return Double.NaN
        Dim m As Double = LowerY - k * LowerX

        'Returning y
        Return k * X + m

    End Function

    Public Overrides Function ToString() As String

        If Name = "" Then
            Return DateTime.Now.ToShortDateString & ": " & DateTime.Now.ToShortTimeString
        Else
            Return Name
        End If

    End Function

    ''' <summary>
    ''' Creates a new AudiogramData which is a deep copy of the original, by using serialization.
    ''' </summary>
    ''' <returns></returns>
    Public Function CreateCopy() As AudiogramData

        'Creating an output object
        Dim newAudiogramData As AudiogramData

        'Serializing to memorystream
        Dim serializedMe As New IO.MemoryStream
        Dim serializer As New XmlSerializer(GetType(AudiogramData))
        serializer.Serialize(serializedMe, Me)

        'Deserializing to new object
        serializedMe.Position = 0
        newAudiogramData = CType(serializer.Deserialize(serializedMe), AudiogramData)
        serializedMe.Close()

        'Returning the new object
        Return newAudiogramData
    End Function

    'Copies all data from the current instance of AudiogramData to the references TargetAudiogramData instance.
    Public Sub CopyData(ByRef TargetAudiogramData As AudiogramData)

        TargetAudiogramData.Name = Name

        TargetAudiogramData.AC_Right = AC_Right
        TargetAudiogramData.AC_Left = AC_Left
        TargetAudiogramData.BC_Right = BC_Right
        TargetAudiogramData.BC_Left = BC_Left

        TargetAudiogramData.AC_Right_Masked = AC_Right_Masked
        TargetAudiogramData.AC_Left_Masked = AC_Left_Masked
        TargetAudiogramData.BC_Right_Masked = BC_Right_Masked
        TargetAudiogramData.BC_Left_Masked = BC_Left_Masked

        TargetAudiogramData.UCL_Left = UCL_Left
        TargetAudiogramData.UCL_Right = UCL_Right

        TargetAudiogramData.Cb_Left_AC = Cb_Left_AC
        TargetAudiogramData.Cb_Left_BC = Cb_Left_BC
        TargetAudiogramData.Cb_Right_AC = Cb_Right_AC
        TargetAudiogramData.Cb_Right_BC = Cb_Right_BC

    End Sub

End Class


