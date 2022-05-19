Imports System.Drawing

Namespace WinFormControls

    <Serializable>
    Public Class PsychometricFunctionDiagram
        Inherits PlotBase

        Public Sub New()
            MyBase.New

            'Setting up audiogram properties
            PlotAreaRelativeMarginLeft = 0.1
            PlotAreaRelativeMarginRight = 0.03
            PlotAreaRelativeMarginTop = 0.05
            PlotAreaRelativeMarginBottom = 0.1

            XlimMin = -15
            XlimMax = 15

            Xlog = False
            XlogBase = 2

            YlimMin = 0.3
            YlimMax = 1
            Yreversed = False
            Ylog = False
            YlogBase = 10

            PlotAreaBorderColor = Color.DarkGray
            PlotAreaBorder = True
            GridLineColor = Color.Gray

            XaxisGridLinePositions = New List(Of Single)
            XaxisDashedGridLinePositions = New List(Of Single) From {-10, -5, 0, 5, 10}
            XaxisDrawTop = False
            XaxisDrawBottom = True
            XaxisTickPositions = New List(Of Single) From {-15, -14, -13, -12, -11, -10, -9, -8, -7, -6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15}
            XaxisTickHeight = 1.5
            XaxisTextPositions = New List(Of Single) From {-15, -10, -5, 0, 5, 10, 15}
            XaxisTextValues = {"-15", "-10", "-5", "0", "5", "10", "15"}
            XaxisTextSize = 8
            XAxisTextBrush = Brushes.Black

            YaxisGridLinePositions = New List(Of Single) From {0.4, 0.6, 0.8}
            YaxisDashedGridLinePositions = New List(Of Single) From {0.3, 0.5, 0.7, 0.9}
            YaxisDrawLeft = True
            YaxisDrawRight = False
            YaxisTickPositions = New List(Of Single) From {0.3, 0.35, 0.45, 0.55, 0.65, 0.75, 0.85, 0.95}
            YaxisTickWidth = 1.5
            YaxisTextPositions = New List(Of Single) From {0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1}
            YaxisTextValues = {"30", "40", "50", "60", "70", "80", "90", "100"}
            YaxisTextSize = 8
            YaxisTextBrush = Brushes.Black

        End Sub



    End Class

End Namespace
