Imports STFN.Core.Audio.SoundScene
Imports STFN.Core.Audio.Formats
Imports System.Reflection
Imports System.Runtime.Serialization

Namespace SipTest



    <Serializable>
    Public Class SiPTestMeasurementHistory

        Public Const NewMeasurementMarker As String = "<New OSTF measurement>"

        Public Property Measurements As New List(Of SipMeasurement)

        Public Sub SaveToFile(Optional ByVal FilePath As String = "", Optional ByVal SaveOnlyLast As Boolean = False)

            'Gets a file path from the user if none is supplied
            If SaveOnlyLast = False Then
                If FilePath = "" Then FilePath = Utils.GetSaveFilePath(,, {".txt"}, "Save stuctured measurement history .txt file as...")
            Else
                If FilePath = "" Then FilePath = Utils.GetSaveFilePath(,, {".txt"}, "Save stuctured measurement (.txt) file as...")
            End If
            If FilePath = "" Then
                MsgBox("No file selected!")
                Exit Sub
            End If

            Dim Output As New List(Of String)

            If SaveOnlyLast = False Then
                For Each Measurement In Measurements
                    Output.Add(Measurement.CreateExportString)
                Next
            Else
                If Measurements.Count > 0 Then Output.Add(Measurements(Measurements.Count - 1).CreateExportString)
            End If

            Utils.SendInfoToLog(String.Join(vbCrLf, Output), IO.Path.GetFileNameWithoutExtension(FilePath), IO.Path.GetDirectoryName(FilePath), True, True)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="FilePath"></param>
        ''' <returns>Returns Nothing if an error occured during loading.</returns>
        Public Function LoadMeasurements(ByRef ParentTestSpecification As SpeechMaterialSpecification, ByRef Randomizer As Random, Optional ByVal FilePath As String = "", Optional ByVal ParticipantID As String = "") As SiPTestMeasurementHistory

            Dim Output As New SiPTestMeasurementHistory

            'Gets a file path from the user if none is supplied
            If FilePath = "" Then FilePath = Utils.GetOpenFilePath(,, {".txt"}, "Please open a stuctured measurement history .txt file.")
            If FilePath = "" Then
                MsgBox("No file selected!")
                Return Nothing
            End If

            'Parses the input file
            Dim InputLines() As String = System.IO.File.ReadAllLines(FilePath, Text.Encoding.UTF8)

            'Splits InputLines into separate measurements based on the SipMeasurement.NewMeasurementMarker string
            Dim ImportedMeasurementList As New List(Of String())
            Dim CurrentlyImportedMeasurement As List(Of String) = Nothing
            For Each Line In InputLines
                If Line.Trim.StartsWith(SiPTestMeasurementHistory.NewMeasurementMarker) Then
                    If CurrentlyImportedMeasurement Is Nothing Then
                        CurrentlyImportedMeasurement = New List(Of String)
                    Else
                        ImportedMeasurementList.Add(CurrentlyImportedMeasurement.ToArray)
                        CurrentlyImportedMeasurement.Clear()
                    End If

                    'Skips adding the NewMeasurementMarker
                    Continue For
                End If
                'Adds the current line to the current measurement
                CurrentlyImportedMeasurement.Add(Line)
            Next

            'Also adding the last measurement
            If CurrentlyImportedMeasurement IsNot Nothing Then ImportedMeasurementList.Add(CurrentlyImportedMeasurement.ToArray)


            'Parsing each measurement
            For Each MeasurementStringArray In ImportedMeasurementList
                Dim LoadedMeasurement = SipMeasurement.ParseImportLines(MeasurementStringArray, Randomizer, ParentTestSpecification, ParticipantID)
                If LoadedMeasurement Is Nothing Then Return Nothing
                Output.Measurements.Add(LoadedMeasurement)
            Next

            'Returns the loaded measurements
            Return Output

        End Function

    End Class




End Namespace
