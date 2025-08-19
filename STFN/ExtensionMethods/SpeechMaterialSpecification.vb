Imports System.Runtime.CompilerServices
Imports STFN.Core
Imports STFN.Core.SpeechMaterialSpecification

'This file contains extension methods for STFN.Core.SpeechMaterialSpecification

Partial Public Module Extensions

    <Extension>
    Public Function GetAvailableTestSituationNames(obj As SpeechMaterialSpecification) As List(Of String)
        Dim OutputList As New List(Of String)
        For Each TestSituation In obj.MediaSets
            OutputList.Add(TestSituation.ToString)
        Next
        Return OutputList
    End Function

    <Extension>
    Public Sub LoadSpeechMaterialComponentsFile(obj As SpeechMaterialSpecification)

        Try
            obj.SpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(obj.GetSpeechMaterialFilePath, obj.GetTestRootPath)

            'Referencing the Me as ParentTestSpecification in the loaded SpeechMaterial 
            If obj.SpeechMaterial IsNot Nothing Then
                obj.SpeechMaterial.ParentTestSpecification = obj
            End If

        Catch ex As Exception
            MsgBox("Failed to load speech material file: " & obj.GetSpeechMaterialFilePath())
        End Try

    End Sub

    <Extension>
    Public Sub WriteTextFile(obj As SpeechMaterialSpecification, Optional FilePath As String = "")

        If FilePath = "" Then
            FilePath = Utils.GeneralIO.GetSaveFilePath(,, {".txt"}, "Save OSTF test specification file as")
        End If

        Dim OutputList As New List(Of String)
        OutputList.Add("// This file is an OSTF test specification file. Its first non-empty line which is not commented out (using double slashes) must be exacly " & FormatFlag)
        OutputList.Add("// In order to make the test that this file specifies available in OSTF, put this file in the OSTF sub folder named: " & OstfBase.AvailableSpeechMaterialsSubFolder & ", and the restart the OSTF software.")
        OutputList.Add("")
        OutputList.Add(FormatFlag)
        OutputList.Add("")

        If obj.Name.Trim = "" Then
            OutputList.Add("// Name = [Add name here, and remove the double slashes]")
        Else
            OutputList.Add("Name = " & obj.Name)
        End If

        If obj.DirectoryName.Trim = "" Then
            OutputList.Add("// DirectoryName = Tests\ [Add the DirectoryName here, and remove the double slashes]")
        Else
            OutputList.Add("DirectoryName = " & obj.DirectoryName)
        End If

        If obj.TestPresetsSubFilePath.Trim = "" Then
            OutputList.Add("// TestPresetsSubFilePath =  [Add the file path to the TestPresets file here, and remove the double slashes]")
        Else
            OutputList.Add("TestPresetsSubFilePath = TestPresetsSubFilePath")
        End If

        Logging.SendInfoToLog(String.Join(vbCrLf, OutputList), IO.Path.GetFileNameWithoutExtension(FilePath), IO.Path.GetDirectoryName(FilePath), True, True)

    End Sub



End Module

