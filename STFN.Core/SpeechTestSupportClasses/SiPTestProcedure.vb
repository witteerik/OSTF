Imports STFN.Core.Audio.SoundScene
Imports STFN.Core.Audio.Formats
Imports System.Reflection
Imports System.Runtime.Serialization

Namespace SipTest



    Public Class SiPTestProcedure
        Public Property AdaptiveType As AdaptiveTypes
        Public Property TestParadigm As SiPTestparadigm

        Public Enum AdaptiveTypes
            SimpleUpDown
            Fixed
        End Enum

        Public Enum SiPTestparadigm
            Quick
            Slow
            Directional2
            Directional3
            Directional5
            FlexibleLocations
            BMLD
        End Enum

        Public Property LengthReduplications As Integer?
        Public Property RandomizeOrder As Boolean = True

        Public ReadOnly Property TargetStimulusLocations As New SortedList(Of SiPTestparadigm, SoundSourceLocation())

        Public ReadOnly Property MaskerLocations As New SortedList(Of SiPTestparadigm, SoundSourceLocation())

        Public ReadOnly Property BackgroundLocations As New SortedList(Of SiPTestparadigm, SoundSourceLocation())

        Public Sub New(ByVal AdaptiveType As AdaptiveTypes, ByVal TestParadigm As SiPTestparadigm)
            Me.AdaptiveType = AdaptiveType
            Me.TestParadigm = TestParadigm

            'Setting up TargetStimulusLocations
            'TestParadigm Slow
            TargetStimulusLocations.Add(SiPTestparadigm.Slow, {New SoundSourceLocation With {.HorizontalAzimuth = 0}})
            MaskerLocations.Add(SiPTestparadigm.Slow, {
                                New SoundSourceLocation With {.HorizontalAzimuth = -30, .Elevation = 0, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1}})
            BackgroundLocations.Add(SiPTestparadigm.Slow, {
                                New SoundSourceLocation With {.HorizontalAzimuth = -30, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1}})

            ''Testparadigm.Quick
            'TargetStimulusLocations.Add(Testparadigm.Quick, {New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45}})
            'MaskerLocations.Add(Testparadigm.Quick, {
            '                    New SoundSourceLocation With {.HorizontalAzimuth = 180, .Distance = 1.45}})
            'BackgroundLocations.Add(Testparadigm.Quick, {
            '                    New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1.45},
            '                    New SoundSourceLocation With {.HorizontalAzimuth = 180, .Distance = 1.45}})

            TargetStimulusLocations.Add(SiPTestparadigm.Directional2, {
            New SoundSourceLocation With {.HorizontalAzimuth = -15, .Distance = 1},
            New SoundSourceLocation With {.HorizontalAzimuth = 15, .Distance = 1}})
            MaskerLocations.Add(SiPTestparadigm.Directional2, {
                                New SoundSourceLocation With {.HorizontalAzimuth = -30, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1}})
            BackgroundLocations.Add(SiPTestparadigm.Directional2, {
                                New SoundSourceLocation With {.HorizontalAzimuth = 45, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 135, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = -135, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = -45, .Distance = 1}})


            TargetStimulusLocations.Add(SiPTestparadigm.Directional3, {
            New SoundSourceLocation With {.HorizontalAzimuth = -30, .Distance = 1},
            New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1},
            New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1}})
            MaskerLocations.Add(SiPTestparadigm.Directional3, {
                                New SoundSourceLocation With {.HorizontalAzimuth = -30, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1}})
            BackgroundLocations.Add(SiPTestparadigm.Directional3, {
                                New SoundSourceLocation With {.HorizontalAzimuth = 45, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 135, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = -135, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = -45, .Distance = 1}})


            TargetStimulusLocations.Add(SiPTestparadigm.Directional5, {
            New SoundSourceLocation With {.HorizontalAzimuth = -90, .Distance = 1},
            New SoundSourceLocation With {.HorizontalAzimuth = -30, .Distance = 1},
            New SoundSourceLocation With {.HorizontalAzimuth = 0, .Distance = 1},
            New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1},
            New SoundSourceLocation With {.HorizontalAzimuth = 90, .Distance = 1}})
            MaskerLocations.Add(SiPTestparadigm.Directional5, {
                                New SoundSourceLocation With {.HorizontalAzimuth = -30, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 30, .Distance = 1}})
            BackgroundLocations.Add(SiPTestparadigm.Directional5, {
                                New SoundSourceLocation With {.HorizontalAzimuth = 45, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = 135, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = -135, .Distance = 1},
                                New SoundSourceLocation With {.HorizontalAzimuth = -45, .Distance = 1}})


        End Sub

        ''' <summary>
        ''' Sets the supplied horizontal azimuths to the TestParadigm key in the TargetStimulusLocations object, overwriting any previous values.
        ''' </summary>
        ''' <param name="Testparadigm"></param>
        ''' <param name="HorizontalAzimuths"></param>
        Public Sub SetTargetStimulusLocations(ByVal TestParadigm As SiPTestparadigm, ByVal HorizontalAzimuths As List(Of Double),
                                              Optional ByVal Elevations As List(Of Double) = Nothing, Optional ByVal Distances As List(Of Double) = Nothing)

            If _TargetStimulusLocations.ContainsKey(TestParadigm) Then
                _TargetStimulusLocations.Remove(TestParadigm)
            End If
            Dim SourceLocationArrayList As New List(Of SoundSourceLocation)

            If Elevations IsNot Nothing Then
                If Elevations.Count <> HorizontalAzimuths.Count Then Throw New ArgumentException("Unequal lengths of parameters!")
            End If

            If Distances IsNot Nothing Then
                If Distances.Count <> HorizontalAzimuths.Count Then Throw New ArgumentException("Unequal lengths of parameters!")
            End If

            For i = 0 To HorizontalAzimuths.Count - 1
                Dim NewLocation = New SoundSourceLocation With {.HorizontalAzimuth = HorizontalAzimuths(i)}
                If Elevations IsNot Nothing Then NewLocation.Elevation = Elevations(i)
                If Distances IsNot Nothing Then NewLocation.Distance = Distances(i)
                SourceLocationArrayList.Add(NewLocation)
            Next

            _TargetStimulusLocations.Add(TestParadigm, SourceLocationArrayList.ToArray)
        End Sub

        ''' <summary>
        ''' Sets the supplied Locations to the TestParadigm key in the TargetStimulusLocations object, overwriting any previous values.
        ''' </summary>
        ''' <param name="Testparadigm"></param>
        ''' <param name="Locations"></param>
        Public Sub SetTargetStimulusLocations(ByVal TestParadigm As SiPTestparadigm, ByRef Locations As List(Of SoundSourceLocation))

            If _TargetStimulusLocations.ContainsKey(TestParadigm) Then
                _TargetStimulusLocations.Remove(TestParadigm)
            End If
            _TargetStimulusLocations.Add(TestParadigm, Locations.ToArray)
        End Sub

        ''' <summary>
        ''' Sets the supplied horizontal azimuths to the TestParadigm key in the MaskerLocations object, overwriting any previous values.
        ''' </summary>
        ''' <param name="Testparadigm"></param>
        ''' <param name="HorizontalAzimuths"></param>
        Public Sub SetMaskerLocations(ByVal TestParadigm As SiPTestparadigm, ByVal HorizontalAzimuths As List(Of Double), Optional ByVal Elevations As List(Of Double) = Nothing, Optional ByVal Distances As List(Of Double) = Nothing)
            If _MaskerLocations.ContainsKey(TestParadigm) Then
                _MaskerLocations.Remove(TestParadigm)
            End If
            Dim SourceLocationArrayList As New List(Of SoundSourceLocation)

            If Elevations IsNot Nothing Then
                If Elevations.Count <> HorizontalAzimuths.Count Then Throw New ArgumentException("Unequal lengths of parameters!")
            End If

            If Distances IsNot Nothing Then
                If Distances.Count <> HorizontalAzimuths.Count Then Throw New ArgumentException("Unequal lengths of parameters!")
            End If

            For i = 0 To HorizontalAzimuths.Count - 1
                Dim NewLocation = New SoundSourceLocation With {.HorizontalAzimuth = HorizontalAzimuths(i)}
                If Elevations IsNot Nothing Then NewLocation.Elevation = Elevations(i)
                If Distances IsNot Nothing Then NewLocation.Distance = Distances(i)
                SourceLocationArrayList.Add(NewLocation)
            Next

            _MaskerLocations.Add(TestParadigm, SourceLocationArrayList.ToArray)
        End Sub

        ''' <summary>
        ''' Sets the supplied Locations to the TestParadigm key in the MaskerLocations object, overwriting any previous values.
        ''' </summary>
        ''' <param name="Testparadigm"></param>
        ''' <param name="Locations"></param>
        Public Sub SetMaskerLocations(ByVal TestParadigm As SiPTestparadigm, ByRef Locations As List(Of SoundSourceLocation))

            If _MaskerLocations.ContainsKey(TestParadigm) Then
                _MaskerLocations.Remove(TestParadigm)
            End If
            _MaskerLocations.Add(TestParadigm, Locations.ToArray)
        End Sub

        ''' <summary>
        ''' Sets the supplied horizontal azimuths to the TestParadigm key in the BackgroundLocations object, overwriting any previous values.
        ''' </summary>
        ''' <param name="Testparadigm"></param>
        ''' <param name="HorizontalAzimuths"></param>
        Public Sub SetBackgroundLocations(ByVal TestParadigm As SiPTestparadigm, ByVal HorizontalAzimuths As List(Of Double), Optional ByVal Elevations As List(Of Double) = Nothing, Optional ByVal Distances As List(Of Double) = Nothing)
            If _BackgroundLocations.ContainsKey(TestParadigm) Then
                _BackgroundLocations.Remove(TestParadigm)
            End If
            Dim SourceLocationArrayList As New List(Of SoundSourceLocation)

            If Elevations IsNot Nothing Then
                If Elevations.Count <> HorizontalAzimuths.Count Then Throw New ArgumentException("Unequal lengths of parameters!")
            End If

            If Distances IsNot Nothing Then
                If Distances.Count <> HorizontalAzimuths.Count Then Throw New ArgumentException("Unequal lengths of parameters!")
            End If

            For i = 0 To HorizontalAzimuths.Count - 1
                Dim NewLocation = New SoundSourceLocation With {.HorizontalAzimuth = HorizontalAzimuths(i)}
                If Elevations IsNot Nothing Then NewLocation.Elevation = Elevations(i)
                If Distances IsNot Nothing Then NewLocation.Distance = Distances(i)
                SourceLocationArrayList.Add(NewLocation)
            Next

            _BackgroundLocations.Add(TestParadigm, SourceLocationArrayList.ToArray)
        End Sub

        ''' <summary>
        ''' Sets the supplied Locations to the TestParadigm key in the BackgroundLocations object, overwriting any previous values.
        ''' </summary>
        ''' <param name="Testparadigm"></param>
        ''' <param name="Locations"></param>
        Public Sub SetBackgroundLocations(ByVal TestParadigm As SiPTestparadigm, ByRef Locations As List(Of SoundSourceLocation))

            If _BackgroundLocations.ContainsKey(TestParadigm) Then
                _BackgroundLocations.Remove(TestParadigm)
            End If
            _BackgroundLocations.Add(TestParadigm, Locations.ToArray)
        End Sub

    End Class


End Namespace

