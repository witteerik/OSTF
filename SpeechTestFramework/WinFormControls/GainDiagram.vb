Imports System.Drawing

Namespace WinFormControls


    <Serializable>
    Public Class GainDiagram
        Inherits PlotBase

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


        Public Sub UpdateGainValues(ByRef HearingAidGainData As HearingAidGainData)

            'Clearing any previously stored data
            PointSeries.Clear()
            Lines.Clear()

            'Adding LinesSeries
            Lines.Add(New Line With {.Color = Color.Red, .DashPattern = {0.001, 0.499, 0.499, 0.001}, .Dashed = True, .LineWidth = 5, .XValues = HearingAidGainData.Frequencies, .YValues = HearingAidGainData.RightSideGain})
            Lines.Add(New Line With {.Color = Color.Blue, .DashPattern = {0.499, 0.001, 0.001, 0.499}, .Dashed = True, .LineWidth = 5, .XValues = HearingAidGainData.Frequencies, .YValues = HearingAidGainData.LeftSideGain})

            'Updates the layout
            Invalidate()
            Update()

        End Sub

    End Class

End Namespace
