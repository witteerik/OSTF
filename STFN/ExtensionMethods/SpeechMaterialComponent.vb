Imports System.Runtime.CompilerServices
Imports STFN.Core

'This file contains extension methods for STFN.Core.SpeechMaterialComponent

Partial Public Module Extensions

    <Extension>
    Private Sub WriteSpeechMaterialComponenFile(obj As SpeechMaterialComponent, ByVal OutputSpeechMaterialFolder As String, ByVal ExportAtThisLevel As Boolean, Optional ByRef CustomVariablesExportList As SortedList(Of String, List(Of String)) = Nothing,
                                             Optional ByRef NumericCustomVariableNames As SortedList(Of SpeechMaterialComponent.LinguisticLevels, SortedSet(Of String)) = Nothing,
                                                Optional ByRef CategoricalCustomVariableNames As SortedList(Of SpeechMaterialComponent.LinguisticLevels, SortedSet(Of String)) = Nothing)

        If CustomVariablesExportList Is Nothing Then CustomVariablesExportList = New SortedList(Of String, List(Of String))
        If NumericCustomVariableNames Is Nothing Then NumericCustomVariableNames = obj.GetToplevelAncestor.GetNumericCustomVariableNamesByLinguicticLevel()
        If CategoricalCustomVariableNames Is Nothing Then CategoricalCustomVariableNames = obj.GetToplevelAncestor.GetCategoricalCustomVariableNamesByLinguicticLevel()


        Dim OutputList As New List(Of String)
        If ExportAtThisLevel = True Then
            OutputList.Add("// Setup")

            'Writing Setup values
            OutputList.Add("SequentiallyOrderedLists = " & obj.SequentiallyOrderedLists.ToString)
            OutputList.Add("SequentiallyOrderedSentences = " & obj.SequentiallyOrderedSentences.ToString)
            OutputList.Add("SequentiallyOrderedWords = " & obj.SequentiallyOrderedWords.ToString)
            OutputList.Add("SequentiallyOrderedPhonemes = " & obj.SequentiallyOrderedPhonemes.ToString)
            OutputList.Add("PresetLevel = " & obj.PresetLevel.ToString)
            For Each Item In obj.PresetSpecifications
                OutputList.Add("Preset = " & Item.Item1.Trim & ": " & String.Join(", ", Item.Item2))
            Next

            'Writing components
            OutputList.Add("")
            OutputList.Add("// Components")
        End If

        Dim HeadingString As String = "// LinguisticLevel" & vbTab & "Id" & vbTab & "ParentId" & vbTab & "PrimaryStringRepresentation" & vbTab & "IsPractiseComponent"

        Dim Main_List As New List(Of String)

        'Linguistic Level
        Main_List.Add(obj.LinguisticLevel.ToString)

        'Id
        Main_List.Add(obj.Id)

        'ParentId 
        If obj.ParentComponent IsNot Nothing Then
            Main_List.Add(obj.ParentComponent.Id)
        Else
            Main_List.Add("")
        End If

        'PrimaryStringRepresentation
        Main_List.Add(obj.PrimaryStringRepresentation)

        ''CustomVariablesDatabase 
        'If CustomVariablesDatabasePath <> "" Then
        '    Dim CurrentDataBasePath = IO.Path.GetFileName(CustomVariablesDatabasePath)
        '    Main_List.Add(CurrentDataBasePath)
        'Else
        '    Main_List.Add("")
        'End If

        'OrderedChildren 
        'Main_List.Add(OrderedChildren.ToString) 'Removed!

        'IsPractiseComponent
        Main_List.Add(obj.IsPractiseComponent.ToString)

        'The media folders are removed and moved to the MediaSet class
        'MediaFolder 
        'Main_List.Add(GetMediaFolderName)
        'MaskerFolder 
        'Main_List.Add(MaskerFolder)
        'BackgroundNonspeechFolder 
        'Main_List.Add(BackgroundNonspeechFolder)
        'BackgroundSpeechFolder 
        'Main_List.Add(BackgroundSpeechFolder)

        If ExportAtThisLevel = True Then
            OutputList.Add(HeadingString)
        End If
        If obj.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.List Or obj.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.ListCollection Then
            OutputList.Add("") 'Adding an empty line between list or list collection level components
        End If

        OutputList.Add(String.Join(vbTab, Main_List))


        'Writing to file
        Logging.SendInfoToLog(String.Join(vbCrLf, OutputList), IO.Path.GetFileNameWithoutExtension(SpeechMaterialComponent.SpeechMaterialComponentFileName), OutputSpeechMaterialFolder, True, True, ExportAtThisLevel)

        'Custom variables
        Dim CustomVariablesDatabasePath As String = SpeechMaterialComponent.GetDatabaseFileName(obj.LinguisticLevel)

        If CustomVariablesDatabasePath <> "" Then

            'Getting the right collection into which to store custom variable values
            Dim CurrentCustomVariablesOutputList As New List(Of String)
            If CustomVariablesExportList.ContainsKey(CustomVariablesDatabasePath) = False Then
                CustomVariablesExportList.Add(CustomVariablesDatabasePath, CurrentCustomVariablesOutputList)
            Else
                CurrentCustomVariablesOutputList = CustomVariablesExportList(CustomVariablesDatabasePath)
            End If

            'Getting the variable names and types
            Dim CategoricalVariableNames = CategoricalCustomVariableNames(obj.LinguisticLevel).ToList
            Dim NumericVariableNames = NumericCustomVariableNames(obj.LinguisticLevel).ToList

            'Writing headings only on the first line
            If CurrentCustomVariablesOutputList.Count = 0 Then
                Dim CustomVariableNamesList As New List(Of String)
                CustomVariableNamesList.AddRange(CategoricalVariableNames)
                CustomVariableNamesList.AddRange(NumericVariableNames)
                Dim CustomVariableNames = String.Join(vbTab, CustomVariableNamesList)

                'Writing types only if there are any headings
                If CustomVariableNames.Trim <> "" Then
                    CurrentCustomVariablesOutputList.Add(CustomVariableNames)

                    Dim CategoricalVariableTypes = DSP.Repeat("C", CategoricalVariableNames.Count).ToList
                    Dim NumericVariableTypes = DSP.Repeat("N", NumericVariableNames.Count).ToList
                    Dim CustomVariableTypesList As New List(Of String)
                    CustomVariableTypesList.AddRange(CategoricalVariableTypes)
                    CustomVariableTypesList.AddRange(NumericVariableTypes)
                    Dim VariableTypes = String.Join(vbTab, CustomVariableTypesList)
                    If VariableTypes.Trim <> "" Then CurrentCustomVariablesOutputList.Add(VariableTypes)

                End If
            End If

            'Looking up values
            'First categorical values
            Dim CustomVariableValues As New List(Of String)
            For Each VarName In CategoricalVariableNames
                Dim CurrentValue = obj.GetCategoricalVariableValue(VarName)
                CustomVariableValues.Add(CurrentValue)
            Next
            'Then numeric values
            For Each VarName In NumericCustomVariableNames(obj.LinguisticLevel)
                Dim CurrentValue = obj.GetNumericVariableValue(VarName)
                If CurrentValue IsNot Nothing Then
                    CustomVariableValues.Add(CurrentValue)
                Else
                    'Adds an empty string for missing value
                    CustomVariableValues.Add("")
                End If
            Next
            'Storing the value string
            Dim CustomVariableValuesString = String.Join(vbTab, CustomVariableValues)
            If CustomVariableValuesString.Trim <> "" Then CurrentCustomVariablesOutputList.Add(CustomVariableValuesString)

        End If

        'Cascading to all child components
        For Each ChildComponent In obj.ChildComponents
            ChildComponent.WriteSpeechMaterialComponenFile(OutputSpeechMaterialFolder, False, CustomVariablesExportList)
        Next

        If ExportAtThisLevel = True Then
            'Exporting custom variables
            For Each item In CustomVariablesExportList
                Logging.SendInfoToLog(String.Join(vbCrLf, item.Value), IO.Path.GetFileNameWithoutExtension(item.Key), OutputSpeechMaterialFolder, True, True, ExportAtThisLevel)
            Next
        End If

    End Sub


    <Extension>
    Public Sub SummariseCategoricalVariables(obj As SpeechMaterialComponent, ByVal SourceLevels As SpeechMaterialComponent.LinguisticLevels, ByVal CustomVariableName As String, ByRef MetricType As CategoricalSummaryMetricTypes)

        If obj.LinguisticLevel < SourceLevels Then

            Dim Descendants = obj.GetAllDescenentsAtLevel(SourceLevels)

            Dim VariableNameSourceLevelPrefix = SourceLevels.ToString & "_Level_"

            Dim ValueList As New SortedList(Of String, Integer)
            For Each d In Descendants
                Dim VariableValue As String = d.GetCategoricalVariableValue(CustomVariableName)

                'Adding missing variable values
                If ValueList.ContainsKey(VariableValue) = False Then ValueList.Add(d.GetCategoricalVariableValue(CustomVariableName), 0)

                'Counting the occurence of the specific variable value
                ValueList(d.GetCategoricalVariableValue(CustomVariableName)) += 1
            Next

            Select Case MetricType
                Case CategoricalSummaryMetricTypes.Mode

                    If ValueList.Count > 0 Then

                        'Getting the most common value (TODO: the following lines are probably rather inefficient way and could possibly need opimization with larger datasets...)
                        Dim MaxOccurences = ValueList.Values.Max
                        Dim ModeList As New List(Of String)
                        For Each CurrentValue In ValueList
                            If CurrentValue.Value = MaxOccurences Then ModeList.Add(CurrentValue.Key)
                        Next

                        'If there are more than one mode value (i.e. equal number of occurences) they are returned as comma separated strings
                        Dim ModeValuesString As String = String.Join(",", ModeList)

                        'Storing the result
                        obj.SetCategoricalVariableValue(VariableNameSourceLevelPrefix & "Mode_" & CustomVariableName, ModeValuesString)
                    Else

                        'Storing the an empty string as result, as there was no item in ValueList
                        obj.SetCategoricalVariableValue(VariableNameSourceLevelPrefix & "Mode_" & CustomVariableName, "")
                    End If

                Case CategoricalSummaryMetricTypes.Distribution

                    If ValueList.Count > 0 Then

                        'Getting the most common value
                        Dim DistributionList As New List(Of String)
                        For Each CurrentValue In ValueList
                            DistributionList.Add(CurrentValue.Key & "," & CurrentValue.Value)
                        Next

                        'The distribution are returned as vertical bar (|) separatered key value pairs of value and number of occurences
                        '(this rather akward format choice is selected in order to be able to use tab delimited files. In this way, the whole
                        'distribution may be put in a single cell, for instance in Excel (given that the maximum number of cell characters in not reached...)) 
                        Dim DistributionString As String = String.Join("|", DistributionList)

                        'TODO: This should really be sorted in freuency!

                        'Storing the result
                        obj.SetCategoricalVariableValue(VariableNameSourceLevelPrefix & "Distribution_" & CustomVariableName, DistributionString)
                    Else

                        'Storing the an empty string as result, as there was no item in ValueList
                        obj.SetCategoricalVariableValue(VariableNameSourceLevelPrefix & "Distribution_" & CustomVariableName, "")
                    End If

            End Select

            'Cascading calculations to lower levels
            'Calling this from within the conditional statment, as no descendants should exist at more than one
            'level, and if Me.LinguisticLevel >= SourceLevels no other descendants should be either, and thus the recursive calls can stop here.
            For Each Child In obj.ChildComponents
                Child.SummariseCategoricalVariables(SourceLevels, CustomVariableName, MetricType)
            Next

        End If

    End Sub

    Public Enum CategoricalSummaryMetricTypes
        Mode
        Distribution
    End Enum

    <Extension>
    Public Sub SummariseNumericVariables(obj As SpeechMaterialComponent, ByVal SourceLevels As SpeechMaterialComponent.LinguisticLevels, ByVal CustomVariableName As String, ByRef MetricType As NumericSummaryMetricTypes)

        If obj.LinguisticLevel < SourceLevels Then

            Dim Descendants = obj.GetAllDescenentsAtLevel(SourceLevels)

            Dim VariableNameSourceLevelPrefix = SourceLevels.ToString & "_Level_"

            Dim ValueList As New List(Of Double)
            For Each d In Descendants
                ValueList.Add(d.GetNumericVariableValue(CustomVariableName))
            Next

            Select Case MetricType
                Case NumericSummaryMetricTypes.ArithmeticMean

                    'Storing the result
                    Dim SummaryResult As Double = ValueList.Average
                    obj.SetNumericVariableValue(VariableNameSourceLevelPrefix & "Mean_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.StandardDeviation

                    'Storing the result
                    Dim SummaryResult As Double = MathNet.Numerics.Statistics.Statistics.StandardDeviation(ValueList)
                    obj.SetNumericVariableValue(VariableNameSourceLevelPrefix & "SD_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.Maximum

                    'Storing the result
                    Dim SummaryResult As Double = ValueList.Max
                    obj.SetNumericVariableValue(VariableNameSourceLevelPrefix & "Max_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.Minimum

                    'Storing the result
                    Dim SummaryResult As Double = ValueList.Min
                    obj.SetNumericVariableValue(VariableNameSourceLevelPrefix & "Min_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.Median

                    Dim SummaryResult As Double = MathNet.Numerics.Statistics.Statistics.Median(ValueList)
                    obj.SetNumericVariableValue(VariableNameSourceLevelPrefix & "MD_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.InterquartileRange

                    Dim SummaryResult As Double = MathNet.Numerics.Statistics.Statistics.InterquartileRange(ValueList)
                    obj.SetNumericVariableValue(VariableNameSourceLevelPrefix & "IQR_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.CoefficientOfVariation

                    'Storing the result
                    Dim SummaryResult As Double = DSP.CoefficientOfVariation(ValueList)
                    obj.SetNumericVariableValue(VariableNameSourceLevelPrefix & "CV_" & CustomVariableName, SummaryResult)

            End Select


            'Cascading calculations to lower levels
            'Calling this from within the conditional statment, as no descendants should exist at more than one
            'level, and if Me.LinguisticLevel >= SourceLevels no other descendants should be either, and thus the recursive calls can stop here.
            For Each Child In obj.ChildComponents
                Child.SummariseNumericVariables(SourceLevels, CustomVariableName, MetricType)
            Next

        End If

    End Sub

    Public Enum NumericSummaryMetricTypes
        ArithmeticMean
        StandardDeviation
        Maximum
        Minimum
        Median
        InterquartileRange
        CoefficientOfVariation
    End Enum



End Module