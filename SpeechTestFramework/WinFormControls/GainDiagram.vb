Imports System.Windows.Forms
Imports System.Drawing

Namespace WinFormControls


    <Serializable>
    Public Class GainDiagram
        Inherits PlotBase


        Public Property HideLines As Boolean = False

        Public Property AllowPointRemoval As Boolean = False

        Private _GainData As HearingAidGainData = Nothing
        Public Property GainData As HearingAidGainData
            Get
                Return _GainData
            End Get
            Set(value As HearingAidGainData)
                _GainData = value
                UpdateLineAndPointData()
            End Set
        End Property


        Public Event DataChanged()

        Public Sub New()
            MyBase.New

            'Setting up audiogram properties
            PlotAreaRelativeMarginLeft = 0.1
            PlotAreaRelativeMarginRight = 0.03
            PlotAreaRelativeMarginTop = 0.05
            PlotAreaRelativeMarginBottom = 0.1

            XlimMin = 125
            XlimMax = 8000

            Xlog = True
            XlogBase = 2

            YlimMin = -10
            YlimMax = 50
            Yreversed = False
            Ylog = False
            YlogBase = 10

            PlotAreaBorderColor = Color.DarkGray
            PlotAreaBorder = True
            GridLineColor = Color.Gray

            XaxisGridLinePositions = New List(Of Single) From {125, 250, 500, 1000, 2000, 4000, 8000}
            XaxisDashedGridLinePositions = New List(Of Single) From {750, 1500, 3000, 6000}
            XaxisDrawTop = False
            XaxisDrawBottom = True
            XaxisTickPositions = New List(Of Single)
            XaxisTickHeight = 2
            XaxisTextPositions = New List(Of Single) From {125, 250, 500, 1000, 2000, 4000, 8000}
            XaxisTextValues = {"125", "250", "500", "1k", "2k", "4k", "8k"}
            XaxisTextSize = 8
            XAxisTextBrush = Brushes.Black

            YaxisGridLinePositions = New List(Of Single) From {0, 10, 20, 30, 40, 50, 60, 70}
            YaxisDashedGridLinePositions = New List(Of Single) From {-5, 5, 15, 25, 35, 45, 55, 65}
            YaxisDrawLeft = True
            YaxisDrawRight = False
            YaxisTickPositions = New List(Of Single)
            YaxisTickWidth = 2
            YaxisTextPositions = New List(Of Single) From {-10, 0, 10, 20, 30, 40, 50, 60, 80, 90, 100}
            YaxisTextValues = {"-10", "0", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100"}
            YaxisTextSize = 8
            YaxisTextBrush = Brushes.Black

        End Sub



#Region "Diagram editing"

        Private Enum WriteModes
            Write
            Remove
        End Enum

        Private Enum Sides
            Left
            Right
        End Enum

        Private Enum PointTypes
            Left
            Right
        End Enum

        Private EnableEditing As Boolean
        Private CurrentPointType As PointTypes
        Private MyAudiogramSymbolDialog = New AudiogramSymbolDialog

        Private Sub Audiogram_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick

            Select Case e.Button
                Case MouseButtons.Left

                    If EnableEditing = False Then Exit Sub

                    ' Rounding to valid critical band frequenies and gain
                    Dim ValidFrequencies As New SortedSet(Of Double)
                    For Each f In Audio.DSP.SiiCriticalBands.CentreFrequencies
                        ValidFrequencies.Add(f)
                    Next

                    Dim Frequency = Utils.RoundToLog2Frequency(CoordinateToXValue(e.X), ValidFrequencies)
                    Dim StimulusLevel = Utils.RoundToAudiogramLevel(CoordinateToYValue(e.Y))

                    'Setting the value
                    EditGainValue(CurrentPointType, Frequency, StimulusLevel)

                    'Draws it
                    UpdateLineAndPointData()

                    RaiseEvent DataChanged()


                Case MouseButtons.Right

                    MyAudiogramSymbolDialog.Location = e.Location
                    Dim DialogResult = MyAudiogramSymbolDialog.ShowDialog()
                    If DialogResult = DialogResult.OK Then

                        EnableEditing = MyAudiogramSymbolDialog.EditEnabled

                        If MyAudiogramSymbolDialog.LeftSide = True Then
                            CurrentPointType = PointTypes.Left
                        Else
                            'Right side
                            CurrentPointType = PointTypes.Right
                        End If
                    Else
                        EnableEditing = False
                    End If

            End Select

        End Sub

        Private Sub EditGainValue(ByVal PointType As PointTypes, ByVal Frequency As Integer, ByVal Gain As Integer)

            'Creating a new instance of HearingAidGainData data if none exists
            If _GainData Is Nothing Then _GainData = New HearingAidGainData

            'Finding the right array to change
            Dim ModArray As New List(Of HearingAidGainData.GainPoint)
            Select Case PointType
                Case PointTypes.Left
                    ModArray = _GainData.LeftSideGain
                Case PointTypes.Right
                    ModArray = _GainData.RightSideGain
            End Select

            'Putting points into a SortedList to finding the right frequency
            Dim ModGainPoint As HearingAidGainData.GainPoint
            Dim FrequencyList As New SortedList(Of Integer, HearingAidGainData.GainPoint)
            For Each tp In ModArray
                FrequencyList.Add(tp.Frequency, tp)
            Next

            'Modifying the value
            If FrequencyList.ContainsKey(Frequency) = False Then
                ModGainPoint = New HearingAidGainData.GainPoint With {.Frequency = Frequency}
                FrequencyList.Add(ModGainPoint.Frequency, ModGainPoint)
                FrequencyList(Frequency).Gain = Gain
            Else

                'Contains the frequency. If the new Gain value is the same as the old (the user clicked an existing point), the point is removed if AllowPointRemoval = True, otherwise it's changed/updated
                If FrequencyList(Frequency).Gain <> Gain Then
                    FrequencyList(Frequency).Gain = Gain
                Else
                    If AllowPointRemoval = True Then
                        FrequencyList.Remove(Frequency)
                    Else
                        FrequencyList(Frequency).Gain = Gain
                    End If
                End If
            End If

            'Storing the value (overwriting the ModArray, as the ModPoint may have been inserted or removed, via FrequencyList)
            ModArray.Clear()
            For Each Point In FrequencyList.Values
                ModArray.Add(Point)
            Next

        End Sub

        Private Sub UpdateLineAndPointData()

            If Me._GainData Is Nothing Then Exit Sub

            'Clearing any previously stored data
            PointSeries.Clear()
            Lines.Clear()

            'Draw points


            If HideLines = False Then
                'Adding LinesSeries
                Lines.Add(New Line With {.Color = Color.Red, .DashPattern = {0.001, 0.499, 0.499, 0.001}, .Dashed = True, .LineWidth = 5, .XValues = _GainData.GetRightSideFrequencies, .YValues = _GainData.GetRightSideGain})
                Lines.Add(New Line With {.Color = Color.Blue, .DashPattern = {0.499, 0.001, 0.001, 0.499}, .Dashed = True, .LineWidth = 5, .XValues = _GainData.GetLeftSideFrequencies, .YValues = _GainData.GetLeftSideGain})
            End If

            'Updates the layout
            Invalidate()
            Update()

        End Sub


#End Region


    End Class

End Namespace
