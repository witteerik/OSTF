﻿Imports System.Windows.Forms.VisualStyles

Public Class ListRearrangerControl

    Private Property SourceMediaSet As MediaSet

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Enabled = False
    End Sub

    Private Sub ListRearrangerControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub RearrangeWithinLists_CheckedChanged(sender As Object, e As EventArgs) Handles RearrangeWithinLists_RadioButton.CheckedChanged

        ListDescriptives_GroupBox.Enabled = False
        RandomOrder_RadioButton.Enabled = True
        OriginalOrder_RadioButton.Enabled = True
        BalancedOrder_RadioButton.Enabled = False
        BalanceItarations_Label.Enabled = False
        BalanceItarations_IntegerParsingTextBox.Enabled = False
        BalanceProportion_Label.Enabled = False
        FixedBalancePercentage_IntegerParsingTextBox.Enabled = False
        BalanceNumericDistributions_CheckBox.Enabled = False
        UseGroupingVariable_CheckBox.Enabled = False

        CustomOrder_RadioButton.Enabled = False
        CustomPsrOrder_RadioButton.Enabled = False
        OverrideSentenceByFirstWord_CheckBox.Enabled = False

        RandomOrder_RadioButton.Checked = True

    End Sub

    Private Sub RearrangeAcrossLists_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles RearrangeAcrossLists_RadioButton.CheckedChanged

        ListDescriptives_GroupBox.Enabled = True
        RandomOrder_RadioButton.Enabled = True
        OriginalOrder_RadioButton.Enabled = True
        BalancedOrder_RadioButton.Enabled = True
        BalanceItarations_Label.Enabled = True
        BalanceItarations_IntegerParsingTextBox.Enabled = True
        BalanceProportion_Label.Enabled = True
        FixedBalancePercentage_IntegerParsingTextBox.Enabled = True
        BalanceNumericDistributions_CheckBox.Enabled = True
        UseGroupingVariable_CheckBox.Enabled = True

        CustomOrder_RadioButton.Enabled = True
        CustomPsrOrder_RadioButton.Enabled = True
        OverrideSentenceByFirstWord_CheckBox.Enabled = True

    End Sub


    ''' <summary>
    ''' Initiating the control by setting its media set to the referenced media set.
    ''' </summary>
    ''' <param name="MediaSet"></param>
    Public Sub InitiateControl(ByRef MediaSet As MediaSet)
        SourceMediaSet = MediaSet
        Me.Enabled = True
        'Checking default radio button
        RearrangeWithinLists_RadioButton.Checked = True

        'Setting a default list length value based on the current length of the first list
        Dim AllLists = SourceMediaSet.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)
        If AllLists.Count > 0 Then
            ListLength_IntegerParsingTextBox.Text = AllLists(0).ChildComponents.Count.ToString
        End If
    End Sub

    ''' <summary>
    ''' Clears the control from the loaded media set and disables its graphical interface.
    ''' </summary>
    Public Sub DisableControl()
        Me.Enabled = False
        SourceMediaSet = Nothing
    End Sub

    Private Sub RandomOrder_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles RandomOrder_RadioButton.CheckedChanged
        OrderInput_TableLayoutPanel.Enabled = False
        OrderInputHeading_GroupBox.Visible = False
    End Sub

    Private Sub OriginalOrder_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles OriginalOrder_RadioButton.CheckedChanged
        OrderInput_TableLayoutPanel.Enabled = False
        OrderInputHeading_GroupBox.Visible = False
    End Sub

    Private Sub BalancedOrder_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles BalancedOrder_RadioButton.CheckedChanged

        OrderInput_TableLayoutPanel.Controls.Clear()

        If BalancedOrder_RadioButton.Checked = True Then
            OrderInput_TableLayoutPanel.Enabled = True
            OrderInputHeading_GroupBox.Visible = True
            AddCustomVariablesToSelectionBox()

            GroupingVariable_TableLayoutPanel.Enabled = True
            GroupingVariable_GroupBox.Visible = True
            AddCustomVariablesToGroupingVariableSelectionBox()

            UpdateGroupingVariable_GroupBox_Visibility()

        Else
            If CustomOrder_RadioButton.Checked = False And CustomPsrOrder_RadioButton.Checked = False Then
                OrderInput_TableLayoutPanel.Enabled = False
                OrderInputHeading_GroupBox.Visible = False
                OrderInput_TableLayoutPanel.Controls.Clear()

            End If

            GroupingVariable_TableLayoutPanel.Enabled = False
            GroupingVariable_GroupBox.Visible = False
            GroupingVariable_TableLayoutPanel.Controls.Clear()

        End If

    End Sub

    Private Sub UseGroupingVariable_CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles UseGroupingVariable_CheckBox.CheckedChanged
        UpdateGroupingVariable_GroupBox_Visibility()
    End Sub

    Private Sub UpdateGroupingVariable_GroupBox_Visibility()
        If UseGroupingVariable_CheckBox.Checked = True Then
            GroupingVariable_GroupBox.Visible = True
        Else
            GroupingVariable_GroupBox.Visible = False
        End If
    End Sub

    Private CustomOrderInputBox As New Windows.Forms.TextBox With {.Multiline = True, .Dock = Windows.Forms.DockStyle.Fill, .ScrollBars = Windows.Forms.ScrollBars.Vertical}

    Private Sub CustomOrder_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles CustomOrder_RadioButton.CheckedChanged, CustomPsrOrder_RadioButton.CheckedChanged

        OrderInput_TableLayoutPanel.Controls.Clear()

        If CustomOrder_RadioButton.Checked = True Or CustomPsrOrder_RadioButton.Checked = True Then
            If CustomOrder_RadioButton.Checked = True Then
                OrderInputHeading_GroupBox.Text = "Specify the custom (sentence level) SMC order (one Id per row)"
            Else
                OrderInputHeading_GroupBox.Text = "Specify the custom (sentence level) SMC order (one spelling per row)"
            End If

            OrderInput_TableLayoutPanel.Controls.Add(CustomOrderInputBox)
            OrderInput_TableLayoutPanel.Enabled = True
            OrderInputHeading_GroupBox.Visible = True
        Else
            If BalancedOrder_RadioButton.Checked = False Then
                OrderInput_TableLayoutPanel.Enabled = False
                OrderInputHeading_GroupBox.Visible = False
                OrderInput_TableLayoutPanel.Controls.Clear()
            End If
        End If

    End Sub

    Private Sub AddCustomVariablesToSelectionBox()

        OrderInputHeading_GroupBox.Text = "Select variables to balance"

        OrderInput_TableLayoutPanel.SuspendLayout()

        Dim LingusticLevels As New List(Of SpeechMaterialComponent.LinguisticLevels) From {SpeechMaterialComponent.LinguisticLevels.Phoneme, SpeechMaterialComponent.LinguisticLevels.Word, SpeechMaterialComponent.LinguisticLevels.Sentence}

        For Each LinguisticLevel In LingusticLevels

            'Getting all variable names and types at the source linguistic level
            Dim AllCustomVariables As SortedList(Of String, Boolean) = SourceMediaSet.ParentTestSpecification.SpeechMaterial.GetCustomVariableNameAndTypes(LinguisticLevel) ' Variable name, IsNumeric

            Dim rnd As New Random(CInt(LinguisticLevel))

            For Each CustomVariable In AllCustomVariables

                Dim NewVariableControl As New VariableSelectionCheckBox

                'Setting the variable name in the variable selection control
                Dim VariableName = CustomVariable.Key
                NewVariableControl.Text = VariableName & " (" & LinguisticLevel.ToString & " level)"

                'Storing variable information
                NewVariableControl.VariableSpecification.VariableName = VariableName
                NewVariableControl.VariableSpecification.LinguisticLevel = LinguisticLevel

                'Determining if the variable is numeric or not, and setting the corresponding variable selection control value
                Dim IsNumericVariable As Boolean = CustomVariable.Value
                NewVariableControl.VariableSpecification.IsNumericVariable = IsNumericVariable

                'Setting a random background color on the control
                NewVariableControl.BackColor = Drawing.Color.FromArgb(20, CSng(rnd.Next(10, 255)), CSng(rnd.Next(10, 255)), CSng(rnd.Next(10, 255)))

                'Adding the variable selection control
                NewVariableControl.Dock = Windows.Forms.DockStyle.Fill
                OrderInput_TableLayoutPanel.Controls.Add(NewVariableControl)

            Next
        Next

        OrderInput_TableLayoutPanel.Controls.Add(New Windows.Forms.Panel With {.Dock = Windows.Forms.DockStyle.Fill})

        OrderInput_TableLayoutPanel.RowStyles.Clear()
        For r = 0 To OrderInput_TableLayoutPanel.Controls.Count - 1
            OrderInput_TableLayoutPanel.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Absolute, 26))
        Next

        OrderInput_TableLayoutPanel.ResumeLayout()


    End Sub

    Private Sub AddCustomVariablesToGroupingVariableSelectionBox()

        GroupingVariable_TableLayoutPanel.SuspendLayout()

        Dim LingusticLevels As New List(Of SpeechMaterialComponent.LinguisticLevels) From {SpeechMaterialComponent.LinguisticLevels.Phoneme, SpeechMaterialComponent.LinguisticLevels.Word, SpeechMaterialComponent.LinguisticLevels.Sentence}

        For Each LinguisticLevel In LingusticLevels

            'Getting all variable names and types at the source linguistic level
            Dim AllCustomVariables As SortedList(Of String, Boolean) = SourceMediaSet.ParentTestSpecification.SpeechMaterial.GetCustomVariableNameAndTypes(LinguisticLevel) ' Variable name, IsNumeric

            Dim rnd As New Random(CInt(LinguisticLevel))

            For Each CustomVariable In AllCustomVariables

                'Determining if the variable is numeric or not
                Dim IsNumericVariable As Boolean = CustomVariable.Value

                'Including only categorical variables
                If IsNumericVariable = True Then Continue For

                Dim NewVariableControl As New VariableSelectionCheckBox

                'Setting the variable name in the variable selection control
                Dim VariableName = CustomVariable.Key
                NewVariableControl.Text = VariableName & " (" & LinguisticLevel.ToString & " level)"

                'Storing variable information
                NewVariableControl.VariableSpecification.VariableName = VariableName
                NewVariableControl.VariableSpecification.LinguisticLevel = LinguisticLevel

                'Setting the IsNumericVariable value of the variable selection control
                NewVariableControl.VariableSpecification.IsNumericVariable = IsNumericVariable

                'Setting a random background color on the control
                NewVariableControl.BackColor = Drawing.Color.FromArgb(20, CSng(rnd.Next(10, 255)), CSng(rnd.Next(10, 255)), CSng(rnd.Next(10, 255)))

                'Adding the variable selection control
                NewVariableControl.Dock = Windows.Forms.DockStyle.Fill
                GroupingVariable_TableLayoutPanel.Controls.Add(NewVariableControl)

            Next
        Next

        GroupingVariable_TableLayoutPanel.Controls.Add(New Windows.Forms.Panel With {.Dock = Windows.Forms.DockStyle.Fill})

        GroupingVariable_TableLayoutPanel.RowStyles.Clear()
        For r = 0 To GroupingVariable_TableLayoutPanel.Controls.Count - 1
            GroupingVariable_TableLayoutPanel.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Absolute, 26))
        Next

        GroupingVariable_TableLayoutPanel.ResumeLayout()


    End Sub

    Private Class VariableSelectionCheckBox
        Inherits Windows.Forms.CheckBox

        Public Property VariableSpecification As New CustomVariableSpecification

    End Class

    Public Class CustomVariableSpecification
        Public Property LinguisticLevel As SpeechMaterialComponent.LinguisticLevels
        Public Property VariableName As String
        Public Property IsNumericVariable As Boolean
    End Class

    Private Sub ReArrangeButton_Click(sender As Object, e As EventArgs) Handles ReArrangeButton.Click

        'Getting values
        Dim ReArrangeAcrossLists As Boolean = RearrangeAcrossLists_RadioButton.Checked

        Dim ListNamePrefix As String = ListNamePrefix_TextBox.Text.Trim & " "

        Dim TargetListLength As Integer? = ListLength_IntegerParsingTextBox.Value
        If TargetListLength.HasValue = False Then
            MsgBox("You must enter a target list length!")
            Exit Sub
        End If
        If TargetListLength < 1 Then
            MsgBox("You must enter a positive integer value for target list length!")
            Exit Sub
        End If

        Dim OrderType As OrderType
        If OriginalOrder_RadioButton.Checked = True Then
            OrderType = OrderType.Original
        ElseIf RandomOrder_RadioButton.Checked = True Then
            OrderType = OrderType.Random
        ElseIf BalancedOrder_RadioButton.Checked = True Then
            OrderType = OrderType.Balanced
        ElseIf CustomOrder_RadioButton.Checked = True Then
            OrderType = OrderType.CustomId
        ElseIf CustomPsrOrder_RadioButton.Checked = True Then
            OrderType = OrderType.CustomPrimaryStringRepresentation
        Else
            MsgBox("You must select the intended in item order type!")
            Exit Sub
        End If

        Dim NewSpeechMaterialName As String = NewSpeechMaterialName_TextBox.Text.Trim
        If NewSpeechMaterialName = "" Then
            MsgBox("You must enter a name for the new speech material!")
            Exit Sub
        End If

        Dim NewMediasSetName As String = NewMediaSetName_TextBox.Text.Trim
        If NewMediasSetName = "" Then
            MsgBox("You must enter a name for the new media set!")
            Exit Sub
        End If

        Dim BalancedVariables As New List(Of CustomVariableSpecification)
        Dim CustomOrderStrings As New List(Of String)
        Dim BalanceIterations As Integer
        Dim FixedbalancePercentage As Integer? = Nothing
        Dim GroupingVariables As New List(Of CustomVariableSpecification)
        If OrderType = OrderType.Balanced Then
            For Each Control In OrderInput_TableLayoutPanel.Controls
                Dim CastControl = TryCast(Control, VariableSelectionCheckBox)
                If CastControl IsNot Nothing Then
                    If CastControl.Checked = True Then
                        BalancedVariables.Add(CastControl.VariableSpecification)
                        'CastControl.IsNumericVariable is also available if needed
                    End If
                End If
            Next

            If BalancedVariables.Count = 0 Then
                MsgBox("You must select at least one variable to balance between lists!")
                Exit Sub
            End If

            If BalanceItarations_IntegerParsingTextBox.Value Is Nothing Then
                MsgBox("You must enter a number of iterations för list balancing!")
                Exit Sub
            Else
                BalanceIterations = BalanceItarations_IntegerParsingTextBox.Value
                If BalanceIterations < 1 Then
                    MsgBox("Balancing iterations must be higher than 0!")
                    Exit Sub
                End If
            End If

            If FixedBalancePercentage_IntegerParsingTextBox.Value IsNot Nothing Then
                FixedbalancePercentage = FixedBalancePercentage_IntegerParsingTextBox.Value
                If FixedbalancePercentage < 1 Or FixedbalancePercentage > 100 Then
                    MsgBox("If specified, fixed balancing throw percentage must be higher than 0 and equal or less than 100!")
                    Exit Sub
                End If
            End If

            'Getting grouping variable
            If UseGroupingVariable_CheckBox.Checked Then
                For Each Control In GroupingVariable_TableLayoutPanel.Controls
                    Dim CastControl = TryCast(Control, VariableSelectionCheckBox)
                    If CastControl IsNot Nothing Then
                        If CastControl.Checked = True Then
                            GroupingVariables.Add(CastControl.VariableSpecification)
                            'CastControl.IsNumericVariable is also available if needed
                        End If
                    End If
                Next

                'Checking that one and only one grouping variable is selected
                If GroupingVariables.Count = 0 Then
                    MsgBox("You must select one grouping variable!")
                    Exit Sub
                End If
                If GroupingVariables.Count > 1 Then
                    MsgBox("Only one grouping variable can be selected!!")
                    Exit Sub
                End If
            End If


        ElseIf OrderType = OrderType.CustomId Then
            Dim CustomOrderInput = CustomOrderInputBox.Lines

            For Each Line In CustomOrderInput
                If Line.Trim <> "" Then
                    CustomOrderStrings.Add(Line.Trim)
                End If
            Next

            If CustomOrderStrings.Count = 0 Then
                MsgBox("You must specify a custom order! The custom order should consist of sentence level SpeechMaterialComponent Ids, one Id per row.")
                Exit Sub
            End If

            'Checking the validity of the SentenceIds
            Dim AllSentences = SourceMediaSet.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
            Dim SentenceIds As New SortedSet(Of String)
            If OverrideSentenceByFirstWord_CheckBox.Checked = False Then
                For Each Sentence In AllSentences
                    SentenceIds.Add(Sentence.Id)
                Next
            Else
                For Each Sentence In AllSentences
                    If Sentence.ChildComponents.Count > 0 Then
                        SentenceIds.Add(Sentence.ChildComponents(0).Id)
                    Else
                        MsgBox("Unable to override sentence by first word, since the sentence " & Sentence.Id & " has no word components.")
                        Exit Sub
                    End If
                Next
            End If


            For Each CustomOrderId In CustomOrderStrings
                If SentenceIds.Contains(CustomOrderId) = False Then
                    MsgBox("Detected the following sentence level SpeechMaterialComponent Id which do not exist in the selected speech material: " & CustomOrderId & vbCrLf & "Correct it and try again!")
                    Exit Sub
                End If
            Next

        ElseIf OrderType = OrderType.CustomPrimaryStringRepresentation Then
            Dim CustomOrderInput = CustomOrderInputBox.Lines

            For Each Line In CustomOrderInput
                If Line.Trim <> "" Then
                    CustomOrderStrings.Add(Line.Trim)
                End If
            Next

            If CustomOrderStrings.Count = 0 Then
                MsgBox("You must specify a custom order! The custom order should consist of sentence level SpeechMaterialComponent PrimaryStringRepresentations, one item per row.")
                Exit Sub
            End If

            'Checking the validity of the PrimaryStringsRepresentations
            Dim AllSentences = SourceMediaSet.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
            Dim SentencePrimaryStringsRepresentations As New SortedSet(Of String)

            If OverrideSentenceByFirstWord_CheckBox.Checked = False Then
                For Each Sentence In AllSentences
                    SentencePrimaryStringsRepresentations.Add(Sentence.PrimaryStringRepresentation)
                Next
            Else
                For Each Sentence In AllSentences
                    If Sentence.ChildComponents.Count > 0 Then
                        SentencePrimaryStringsRepresentations.Add(Sentence.ChildComponents(0).PrimaryStringRepresentation)
                    Else
                        MsgBox("Unable to override sentence by first word, since the sentence " & Sentence.PrimaryStringRepresentation & " has no word components.")
                        Exit Sub
                    End If
                Next
            End If

            For Each CustomOrderString In CustomOrderStrings
                If SentencePrimaryStringsRepresentations.Contains(CustomOrderString) = False Then
                    MsgBox("Detected the following sentence level SpeechMaterialComponent PrimaryStringRepresentation which do not exist in the selected speech material: " & CustomOrderString & vbCrLf & "Correct it and try again!")
                    Exit Sub
                End If
            Next

        End If

        Dim RandomSeed As Integer = -1 ' Using -1 as default not-set value

        If RandomSeed_IntegerParsingTextBox.Value.HasValue Then
            RandomSeed = RandomSeed_IntegerParsingTextBox.Value
            If RandomSeed < 0 Then
                MsgBox("Random seed must be a non-negative integer, or empty.")
                Exit Sub
            End If
        End If

        Dim MaxListCount As Integer = -1
        If MaxListCount_IntegerParsingTextBox.Value.HasValue Then
            MaxListCount = MaxListCount_IntegerParsingTextBox.Value
            If MaxListCount < 1 Then
                MsgBox("Maximum number of lists must be a positive integer number, or empty.")
                Exit Sub
            End If
        End If

        'Rearranging lists (TODO: the following code should probably be moved to the MediaSet class?
        Rearrange(ReArrangeAcrossLists, OrderType, TargetListLength, MaxListCount, BalancedVariables, BalanceIterations, FixedbalancePercentage, BalanceNumericDistributions_CheckBox.Checked, GroupingVariables, CustomOrderStrings,
                  OverrideSentenceByFirstWord_CheckBox.Checked, NewMediasSetName, NewSpeechMaterialName, ListNamePrefix, RandomSeed)


    End Sub

    Public Sub Rearrange(ByVal ReArrangeAcrossLists As Boolean, ByVal OrderType As OrderType, ByVal TargetListLength As Integer, ByVal MaxListCount As Integer,
                         ByVal BalancedVariables As List(Of CustomVariableSpecification), ByVal BalanceIterations As Integer, ByVal FixedbalancePercentage As Integer?,
                         ByVal BalanceNumericDistributions As Boolean, ByVal GroupingVariables As List(Of CustomVariableSpecification),
                         ByVal CustomOrderStrings As List(Of String), ByVal OverrideSentenceByFirstWord As Boolean,
                         ByVal NewMediasSetName As String, ByVal NewSpeechMaterialName As String, ByVal ListNamePrefix As String, ByVal RandomSeed As Integer,
                         Optional ByVal SentencePrefix As String = "")

        Log_TextBox.Text = ""

        If SentencePrefix = "" Then SentencePrefix = SpeechMaterialComponent.DefaultSentencePrefix

        Dim LogList As New List(Of String)

        Dim rnd As Random
        If RandomSeed < 0 Then
            rnd = New Random
        Else
            rnd = New Random(RandomSeed)
        End If

        'Clears previously loaded sounds
        SpeechMaterialComponent.ClearAllLoadedSounds()

        'Setting read sound channel
        Dim SoundChannel As Integer = 1

        'Creating an object to hold lists, with list index, SMC and recorded sounds
        Dim RearrangedMaterial As New SortedList(Of Integer, List(Of Tuple(Of SpeechMaterialComponent, List(Of Audio.Sound))))

        If ReArrangeAcrossLists = False Then

            'All mixing is within lists only

            'Getting all lists
            Dim AllLists = SourceMediaSet.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)

            'Processing one list at a time
            For CurrentListIndex = 0 To AllLists.Count - 1

                'Adding a new list in the RearrangedMaterial object
                RearrangedMaterial.Add(CurrentListIndex, New List(Of Tuple(Of SpeechMaterialComponent, List(Of Audio.Sound))))

                'Getting the sentence level SMCs in the current list
                Dim SentenceSMCs = AllLists(CurrentListIndex).GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)

                'Storing a local temporary copy of TargetListLength, if this should have to be limited
                Dim CurrentTargetListLength As Integer = TargetListLength

                'Limiting CurrentTargetListLength  to SentenceSMCs.Count
                If CurrentTargetListLength > SentenceSMCs.Count Then
                    CurrentTargetListLength = SentenceSMCs.Count
                End If

                'Mixing lists and getting their sounds
                Select Case OrderType
                    Case OrderType.Original, OrderType.Random

                        'Getting a vector of indices, sequential or random
                        Dim CurrentSentenceOrder() As Integer
                        If OrderType = OrderType.Random Then
                            CurrentSentenceOrder = Utils.SampleWithoutReplacement(CurrentTargetListLength, 0, CurrentTargetListLength, rnd)
                        Else
                            CurrentSentenceOrder = Utils.GetSequence(0, CurrentTargetListLength - 1, 1)
                        End If

                        'Picking sentences in the order specified in CurrentSentenceOrder
                        For s = 0 To CurrentTargetListLength - 1
                            'Getting the current index
                            Dim CurrentIndex = CurrentSentenceOrder(s)

                            'Getting the sentence level sound with modified SMA chunk
                            Dim SentenceSounds = GetSentenceSounds(SentenceSMCs(CurrentIndex), SoundChannel)

                            'Adding the sentence SMC and its sounds
                            RearrangedMaterial(CurrentListIndex).Add(New Tuple(Of SpeechMaterialComponent, List(Of Audio.Sound))(SentenceSMCs(CurrentIndex), SentenceSounds))
                        Next

                    Case OrderType.Balanced
                        Throw New NotImplementedException("Balancing between lists is not compatible with re-arranging within lists.")
                    Case OrderType.CustomId
                        Throw New NotImplementedException("CustomId order is not compatible with re-arranging within lists.")
                    Case OrderType.CustomPrimaryStringRepresentation
                        Throw New NotImplementedException("CustomPrimaryStringRepresentation order is not compatible with re-arranging within lists.")
                End Select
            Next

        Else

            'Mixing across lists

            'Creating a variable that holds the current output list index
            Dim CurrentOutputListIndex As Integer = -1

            'Getting the all sentence level SMCs in the whole speech material
            Dim AllSentences = SourceMediaSet.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)

            'Filtering items not assigned to a group if GroupingVariables are used
            If GroupingVariables.Count > 0 Then

                Dim TempAlliSentences As New List(Of SpeechMaterialComponent)

                For i = 0 To AllSentences.Count - 1

                    'Getting the grouping variable value (TODO: NB! This code only support a single grouping variable! If more that one grouping variable is to be used in the future the structure of this code bit needs to be changed!
                    Dim GroupID As String = AllSentences(i).GetCategoricalVariableValue(GroupingVariables(0).VariableName).Trim ' Indexing the first item, and ignoring any other items in the list!

                    'Adding only if the item has been assigned to a group.
                    If GroupID <> "" Then
                        TempAlliSentences.Add(AllSentences(i))
                    End If
                Next
                AllSentences = TempAlliSentences
            End If


            'Getting a vector of indices, sequential or random
            Dim CurrentSentenceOrder() As Integer
            Select Case OrderType
                Case OrderType.Random, OrderType.Balanced
                    CurrentSentenceOrder = Utils.SampleWithoutReplacement(AllSentences.Count, 0, AllSentences.Count, rnd)
                Case OrderType.Original
                    CurrentSentenceOrder = Utils.GetSequence(0, AllSentences.Count - 1, 1)
                Case OrderType.CustomId

                    'Adding the custom order by looking up the index of each sentence level SCM
                    Dim CurrentSentenceOrderList As New List(Of Integer)
                    For Each Id In CustomOrderStrings
                        For s = 0 To AllSentences.Count - 1

                            If OverrideSentenceByFirstWord = False Then
                                If AllSentences(s).Id = Id Then
                                    CurrentSentenceOrderList.Add(s)
                                    Exit For
                                End If
                            Else
                                If AllSentences(s).ChildComponents(0).Id = Id Then
                                    CurrentSentenceOrderList.Add(s)
                                    Exit For
                                End If
                            End If

                        Next
                    Next
                    CurrentSentenceOrder = CurrentSentenceOrderList.ToArray

                Case OrderType.CustomPrimaryStringRepresentation

                    'Adding the custom order by looking up the index of each sentence level SCM
                    Dim CurrentSentenceOrderList As New List(Of Integer)
                    For Each PrimaryStringRepresentation In CustomOrderStrings
                        For s = 0 To AllSentences.Count - 1

                            If OverrideSentenceByFirstWord = False Then
                                If AllSentences(s).PrimaryStringRepresentation = PrimaryStringRepresentation Then
                                    CurrentSentenceOrderList.Add(s)
                                    Exit For
                                End If
                            Else
                                If AllSentences(s).ChildComponents(0).PrimaryStringRepresentation = PrimaryStringRepresentation Then
                                    CurrentSentenceOrderList.Add(s)
                                    Exit For
                                End If
                            End If

                        Next
                    Next
                    CurrentSentenceOrder = CurrentSentenceOrderList.ToArray

                Case Else
                    Throw New NotImplementedException("Unkown order type")
            End Select

            'Balancing order 
            If OrderType = OrderType.Balanced Then

                Log_TextBox.Text = "Balancing lists..."

                Dim MaxIterations As Integer = BalanceIterations

                'Starting a progress window
                Dim myProgress As New ProgressDisplay
                myProgress.Initialize(MaxIterations, 0, "Balancing words lists...")
                myProgress.Show()

                'Logging balanced variables as log headings
                Dim LogListHeadings As New List(Of String)
                LogListHeadings.Add("Iteration")
                LogListHeadings.Add("NumberOfSwapsInIteration")

                'Setting OptimalOrder initially to CurrentSentenceOrder
                Dim OptimalOrder() As Integer = CurrentSentenceOrder.Clone
                Dim LowestListImbalanceList As New List(Of Double)
                For Each Variable In BalancedVariables
                    If Variable.IsNumericVariable = False Then
                        LowestListImbalanceList.Add(Double.MaxValue)
                        LogListHeadings.Add(Variable.VariableName)
                    Else
                        'Adding two values for each numeric variable (one representing distribution and one representing average)
                        If BalanceNumericDistributions = True Then
                            LowestListImbalanceList.Add(Double.MaxValue)
                            LogListHeadings.Add(Variable.VariableName & "Dispersion")
                        End If
                        LowestListImbalanceList.Add(Double.MaxValue)
                        LogListHeadings.Add(Variable.VariableName & "Mean")
                    End If
                Next

                LogList.Add(String.Join(vbTab, LogListHeadings))

                For Iteration = 0 To MaxIterations

                    '1. Determining the number of swaps in the current iteration
                    Dim ThrowProportion As Double
                    If FixedbalancePercentage.HasValue Then
                        ThrowProportion = 0.01 * FixedbalancePercentage
                    Else
                        'Decreasing the number of throws linearly to from 50% over the first third of the iteration, and then linearly from about 5% in the second third and then one at a time in the last third.
                        'In practise this is implemented by getting the highest value of two paralell values one going from 50 to 0 % in the first third and one going from 10 to 0% across the first two thirds, after that the number of swaps is held at one by the row calculatuing NumberOfSwapsInIteration below.

                        Dim IterationProgress1 As Double = Iteration / ((1 / 3) * MaxIterations)
                        Dim ThrowProportion1 = (1 - IterationProgress1) * 0.5

                        Dim IterationProgress2 As Double = Iteration / ((2 / 3) * MaxIterations)
                        Dim ThrowProportion2 = (1 - IterationProgress2) * 0.1

                        ThrowProportion = Math.Max(0, Math.Max(ThrowProportion1, ThrowProportion2))

                        'If Iteration Mod 100 = 0 Then Console.WriteLine(vbTab & "ThrowProportion:" & ThrowProportion)
                    End If
                    Dim NumberOfSwapsInIteration As Integer = Math.Max(1, Math.Ceiling(AllSentences.Count * ThrowProportion))

                    '1. Creating a new candidate order by swapping some random indices
                    Dim CandidateOrder() As Integer
                    CandidateOrder = OptimalOrder.Clone
                    If Iteration = 0 Then
                        'Using the start order in the first iteration
                    Else
                        For i = 1 To NumberOfSwapsInIteration
                            'Swapping values (this code can be optimized, if needed)
                            Dim Index1 As Integer = rnd.Next(CandidateOrder.Length)
                            Dim Index2 As Integer = rnd.Next(CandidateOrder.Length)
                            Dim Index1Value = CandidateOrder(Index1)
                            Dim Index2Value = CandidateOrder(Index2)
                            CandidateOrder(Index1) = Index2Value
                            CandidateOrder(Index2) = Index1Value
                        Next
                    End If

                    '2. Creating a list composition with the current candidate order
                    Dim CandidateListComposition As New SortedList(Of Integer, List(Of SpeechMaterialComponent))
                    'Creating a variable that holds the current candidate list index
                    Dim CandidateListIndex As Integer = -1

                    'Picking sentences in the order specified in CandidateOrder
                    Dim AddedSentences As Integer = 0

                    Dim AddedGroups As New SortedSet(Of String)
                    For s = 0 To AllSentences.Count - 1

                        'Adding a new list when each list has been filled up with TargetListLength items
                        If AddedSentences Mod TargetListLength = 0 Then
                            If CandidateListComposition.Count = 0 Then
                                'Adding always if there is no list yet
                                CandidateListIndex += 1
                                CandidateListComposition.Add(CandidateListIndex, New List(Of SpeechMaterialComponent))
                            Else
                                'If there is already a list, adding only if that list is not empty
                                If CandidateListComposition(CandidateListIndex).Count > 0 Then
                                    CandidateListIndex += 1
                                    CandidateListComposition.Add(CandidateListIndex, New List(Of SpeechMaterialComponent))
                                End If
                            End If
                        End If

                        If GroupingVariables.Count > 0 Then

                            'Getting the grouping variable value (TODO: NB! This code only support a single grouping variable! If more that one grouping variable is to be used in the future the structure of this code bit needs to be changed!
                            Dim GroupID As String = AllSentences(CandidateOrder(s)).GetCategoricalVariableValue(GroupingVariables(0).VariableName).Trim ' Indexing the first item, and ignoring any other items in the list!

                            'Skipping if the item has not been assigned to a group.
                            If GroupID = "" Then
                                Throw New Exception("An items not assigned to a group has passed through the initial filter that should have removed such items. This must be a bug!")
                            End If

                            If AddedGroups.Contains(GroupID) = False Then
                                'Adding the sentence to list CandidateListIndex, only if no other item in the same group has been added
                                CandidateListComposition(CandidateListIndex).Add(AllSentences(CandidateOrder(s)))
                                AddedSentences += 1
                                AddedGroups.Add(GroupID)
                            End If

                        Else

                            'Adding the sentence to list CandidateListIndex 
                            CandidateListComposition(CandidateListIndex).Add(AllSentences(CandidateOrder(s)))
                            AddedSentences += 1

                        End If

                    Next

                    ''Excluding empty lists
                    'Dim TempCandidateListComposition As New SortedList(Of Integer, List(Of SpeechMaterialComponent))
                    'Dim TempAddedListCount As Integer = 0
                    'For Each kvp In CandidateListComposition

                    '    'Exiting the loop if MaxListCount have allready been added
                    '    If MaxListCount > 0 Then
                    '        If TempCandidateListComposition.Count = MaxListCount Then Exit For
                    '    End If

                    '    'Adding only non-empty lists
                    '    If kvp.Value.Count > 0 Then
                    '        TempCandidateListComposition.Add(TempAddedListCount, kvp.Value)
                    '        TempAddedListCount += 1
                    '    End If
                    'Next
                    'CandidateListComposition = TempCandidateListComposition

                    '2. Evaluating the order
                    'Calculating the im-balance measure
                    Dim CurrentListImbalanceList = CalculateListImbalance(CandidateListComposition, BalancedVariables, BalanceNumericDistributions)

                    'Checking if all values are equal or better
                    Dim AllAreEqualOrBetter As Boolean = True
                    For n = 0 To CurrentListImbalanceList.Count - 1
                        If CurrentListImbalanceList(n) > LowestListImbalanceList(n) Then
                            AllAreEqualOrBetter = False
                            Exit For
                        End If
                    Next

                    'Comparing the imbalance value to the value from the previous best iteration
                    If AllAreEqualOrBetter = True Then
                        'The new order inproved the balance 

                        'Storing the new LowestListImbalanceList value
                        LowestListImbalanceList = CurrentListImbalanceList

                        'Storing the candidate order as the new optimal order
                        OptimalOrder = CandidateOrder.Clone

                        'X. Reporting progress
                        'Console.WriteLine("Iteration: " & Iteration & " Current throw count: " & NumberOfSwapsInIteration & vbTab & "Lower (or equal) imbalance detected (space delimited imbalance values): " & String.Join(" ", LowestListImbalanceList))
                        'Log_TextBox.Text = "Iteration: " & Iteration & " Current throw count: " & NumberOfSwapsInIteration & vbTab & "Lower (or equal) imbalance detected (space delimited imbalance values): " & String.Join(" ", LowestListImbalanceList) & vbCrLf
                        'Log_TextBox.Text = Log_TextBox.Text & "Iteration: " & Iteration & " Current throw count: " & NumberOfSwapsInIteration & vbTab & "Lower (or equal) imbalance detected (space delimited imbalance values): " & String.Join(" ", LowestListImbalanceList) & vbCrLf
                        'Log_TextBox.SelectionStart = Log_TextBox.Text.Length
                        'Log_TextBox.ScrollToCaret()
                        'Me.Invalidate()
                        'Me.Update()
                        'System.Windows.Forms.Application.DoEvents()

                        'Updating progress
                        myProgress.UpdateProgress(Iteration,,,, "Completed iteration ")
                        myProgress.UpdateExtraInfoLabel("Lower (or equal) imbalance detected at iteration " & Iteration & " (Throw count: " & NumberOfSwapsInIteration & ")")

                        LogList.Add(Iteration & vbTab & NumberOfSwapsInIteration & vbTab & String.Join(vbTab, LowestListImbalanceList))
                    Else
                        'The new order did not improve the balance, ignoring it and keeps the optimal order

                        'X. Reporting progress in with regular intervals
                        If Iteration Mod 100 = 0 Then
                            'Console.WriteLine("Iteration: " & Iteration & " Current throw count: " & NumberOfSwapsInIteration)
                            'Log_TextBox.Text = "Iteration: " & Iteration & " Current throw count: " & NumberOfSwapsInIteration & vbCrLf
                            'Log_TextBox.Text = Log_TextBox.Text & "Iteration: " & Iteration & " Current throw count: " & NumberOfSwapsInIteration & vbCrLf
                            'Log_TextBox.SelectionStart = Log_TextBox.Text.Length
                            'Log_TextBox.ScrollToCaret()
                            'Log_TextBox.Invalidate()
                            'Log_TextBox.Update()
                            'System.Windows.Forms.Application.DoEvents()

                            'Updating progress
                            myProgress.UpdateProgress(Iteration,,,, "Completed iteration ")

                        End If

                    End If

                    'X. Quitting if MaxIterations is reached
                    If Iteration >= MaxIterations Then Exit For

                Next

                'When iterations has stopped we take the sentence order in the selected optimal iteration
                CurrentSentenceOrder = OptimalOrder.Clone

                'Closing the progress display
                myProgress.Close()

            End If

            Log_TextBox.Text = Log_TextBox.Text & "Editing sounds..." & vbCrLf
            Log_TextBox.SelectionStart = Log_TextBox.Text.Length
            Log_TextBox.ScrollToCaret()
            Log_TextBox.Invalidate()
            Log_TextBox.Update()

            'Picking sentences in the order specified in CurrentSentenceOrder
            Dim PickedSentences As Integer = 0
            Dim PickedGroups As New SortedSet(Of String)
            For s = 0 To AllSentences.Count - 1

                If s > CurrentSentenceOrder.Count - 1 Then
                    Exit For
                End If

                'Adding a new list at multiples of TargetListLength
                If PickedSentences Mod TargetListLength = 0 Then

                    'Exiting the loop if MaxListCount have allready been added
                    If MaxListCount > 0 Then
                        If RearrangedMaterial.Count = MaxListCount Then
                            Exit For
                        End If
                    End If

                    If RearrangedMaterial.Count = 0 Then
                        'Adding always if there is no list yet
                        CurrentOutputListIndex += 1
                        RearrangedMaterial.Add(CurrentOutputListIndex, New List(Of Tuple(Of SpeechMaterialComponent, List(Of Audio.Sound))))
                    Else
                        'If there is already a list, adding only if that list is not empty
                        If RearrangedMaterial(CurrentOutputListIndex).Count > 0 Then
                            CurrentOutputListIndex += 1
                            RearrangedMaterial.Add(CurrentOutputListIndex, New List(Of Tuple(Of SpeechMaterialComponent, List(Of Audio.Sound))))
                        End If
                    End If

                End If

                If GroupingVariables.Count > 0 Then

                    'Getting the grouping variable value (TODO: NB! This code only support a single grouping variable! If more that one grouping variable is to be used in the future the structure of this code bit needs to be changed!
                    Dim GroupID As String = AllSentences(CurrentSentenceOrder(s)).GetCategoricalVariableValue(GroupingVariables(0).VariableName).Trim ' Indexing the first item, and ignoring any other items in the list!

                    If PickedGroups.Contains(GroupID) = False Then
                        'Adding the sentence, only if no other item in the same group has been added

                        'Getting the sentence level sound with modified SMA chunk
                        Dim SentenceSounds = GetSentenceSounds(AllSentences(CurrentSentenceOrder(s)), SoundChannel)

                        'Adding the SMC and its sounds
                        RearrangedMaterial(CurrentOutputListIndex).Add(New Tuple(Of SpeechMaterialComponent, List(Of Audio.Sound))(AllSentences(CurrentSentenceOrder(s)), SentenceSounds))
                        PickedSentences += 1
                        PickedGroups.Add(GroupID)

                    End If

                Else

                    'Getting the sentence level sound with modified SMA chunk
                    Dim SentenceSounds = GetSentenceSounds(AllSentences(CurrentSentenceOrder(s)), SoundChannel)

                    'Adding the SMC and its sounds
                    RearrangedMaterial(CurrentOutputListIndex).Add(New Tuple(Of SpeechMaterialComponent, List(Of Audio.Sound))(AllSentences(CurrentSentenceOrder(s)), SentenceSounds))
                    PickedSentences += 1

                End If

            Next

        End If


        'Clearing loaded sounds again
        SpeechMaterialComponent.ClearAllLoadedSounds()

        'Creating a new Speech material
        Dim OutputSMC As New SpeechMaterialComponent(rnd)
        OutputSMC.Id = NewSpeechMaterialName
        OutputSMC.PrimaryStringRepresentation = NewSpeechMaterialName
        OutputSMC.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.ListCollection
        OutputSMC.ParentTestSpecification = New SpeechMaterialSpecification(NewSpeechMaterialName, NewSpeechMaterialName.Replace(" ", "_"))
        OutputSMC.ParentTestSpecification.SpeechMaterial = OutputSMC

        'Creating a deep copy of the current media set
        Dim NewMediaSet = SourceMediaSet.CreateCopy

        'Setting changed values
        NewMediaSet.MediaSetName = NewMediasSetName
        NewMediaSet.AudioFileLinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Sentence

        'Inferring other values
        If NewMediaSet.BackgroundNonspeechParentFolder <> "" Then NewMediaSet.BackgroundNonspeechParentFolder = IO.Path.Combine("Media", NewMediaSet.MediaSetName, "BackgroundNonspeech")
        If NewMediaSet.BackgroundSpeechParentFolder <> "" Then NewMediaSet.BackgroundSpeechParentFolder = IO.Path.Combine("Media", NewMediaSet.MediaSetName, "BackgroundSpeech")
        NewMediaSet.CustomVariablesFolder = IO.Path.Combine("Media", NewMediaSet.MediaSetName, "CustomVariables")
        NewMediaSet.MaskerParentFolder = IO.Path.Combine("Media", NewMediaSet.MediaSetName, "Maskers")
        NewMediaSet.MediaParentFolder = IO.Path.Combine("Media", NewMediaSet.MediaSetName, "TestWordRecordings")
        If NewMediaSet.PrototypeMediaParentFolder <> "" Then NewMediaSet.PrototypeMediaParentFolder = IO.Path.Combine("Media", NewMediaSet.MediaSetName, "PrototypeRecordings")
        NewMediaSet.ParentTestSpecification = OutputSMC.ParentTestSpecification

        Dim RearrageHistory As New List(Of String)
        RearrageHistory.Add("This speech material (" & NewSpeechMaterialName & ") was created by rearranging the speech material: " & SourceMediaSet.ParentTestSpecification.Name)
        RearrageHistory.Add("Below the corresponding old and new speech material components (sentence level) are listed.")
        RearrageHistory.Add("OldSentenceId" & vbTab & "NewSentenceId")

        For i = 0 To RearrangedMaterial.Count - 1

            'Setting up a new list component
            Dim ListSCM = New SpeechMaterialComponent(rnd)
            OutputSMC.ChildComponents.Add(ListSCM)
            ListSCM.ParentComponent = OutputSMC
            ListSCM.Id = "L" & i.ToString("00")
            ListSCM.PrimaryStringRepresentation = ListNamePrefix & "" & i.ToString
            ListSCM.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.List

            'Adding sentence level SMCs
            For s = 0 To RearrangedMaterial(i).Count - 1
                Dim Sentence = RearrangedMaterial(i)(s)

                ListSCM.ChildComponents.Add(Sentence.Item1)
                Sentence.Item1.ParentComponent = ListSCM
                Sentence.Item1.PrimaryStringRepresentation = SentencePrefix & s.ToString("00")

                'Setting the ID and PrimaryStringRepresentation (this also ensures a new correct sound file path)
                'Storing the old and new sentence Ids
                Dim OldSentenceId = Sentence.Item1.Id
                Dim NewSentenceId = "L" & i.ToString("00") & "S" & s.ToString("00")
                RearrageHistory.Add(OldSentenceId & vbTab & NewSentenceId)

                Sentence.Item1.Id = NewSentenceId

                For w = 0 To Sentence.Item1.ChildComponents.Count - 1
                    Sentence.Item1.ChildComponents(w).Id = "L" & i.ToString("00") & "S" & s.ToString("00") & "W" & w.ToString("00")
                    For p = 0 To Sentence.Item1.ChildComponents(w).ChildComponents.Count - 1
                        Sentence.Item1.ChildComponents(w).ChildComponents(p).Id = "L" & i.ToString("00") & "S" & s.ToString("00") & "W" & w.ToString("00") & "P" & p.ToString("00")
                    Next
                Next

                'Injecting the sound in the SoundlIbrary as if it were loaded from file
                For soundIndex = 0 To NewMediaSet.MediaAudioItems - 1
                    'Determine the new sound path
                    Dim SoundPath As String = IO.Path.Combine(NewMediaSet.GetFullMediaParentFolder, Sentence.Item1.GetMediaFolderName, "Sound" & soundIndex.ToString("00"))
                    SpeechMaterialComponent.SoundLibrary.Add(SoundPath, Sentence.Item2(soundIndex))
                Next
            Next
        Next


        'Writing the sounds
        SpeechMaterialComponent.SaveAllLoadedSounds()

        'Updating custom variables Id values
        OutputSMC.SetIdAsCategoricalCustumVariable(True)

        OutputSMC.WriteSpeechMaterialToFile(OutputSMC.ParentTestSpecification, OutputSMC.ParentTestSpecification.GetTestRootPath)

        NewMediaSet.WriteToFile()

        NewMediaSet.WriteCustomVariables()

        'Storing the conversion data
        Utils.SendInfoToLog(String.Join(vbCrLf, RearrageHistory), "SpeechMaterialRearragementResult", OutputSMC.ParentTestSpecification.GetTestRootPath)

        'Logging
        Utils.SendInfoToLog(String.Join(vbCrLf, LogList), "SpeechMaterialRearragementResultLog", OutputSMC.ParentTestSpecification.GetTestRootPath)

    End Sub


    Private Function GetSentenceSounds(ByRef SpeechMaterialComponent As SpeechMaterialComponent, ByVal SoundChannel As Integer) As List(Of Audio.Sound)

        'Getting the sound
        Dim SentenceSounds As New List(Of Audio.Sound)
        For r = 0 To SourceMediaSet.MediaAudioItems - 1
            Dim SmaComponents As New List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)
            Dim CurrentSentenceSound = SpeechMaterialComponent.GetSound(SourceMediaSet, r, SoundChannel,,,,,,,,, SmaComponents)
            If SmaComponents.Count > 1 Then MsgBox("An error has been detected in the SMA components. The process will likely fail.")
            CurrentSentenceSound.SMA.ChannelData(SoundChannel).Clear()
            CurrentSentenceSound.SMA.ChannelData(SoundChannel).Add(SmaComponents(0))
            SmaComponents(0).ParentComponent = CurrentSentenceSound.SMA.ChannelData(SoundChannel)
            'Shifting
            CurrentSentenceSound.SMA.ChannelData(SoundChannel)(0).TimeShift(-CurrentSentenceSound.SMA.ChannelData(SoundChannel)(0).StartSample)
            SentenceSounds.Add(CurrentSentenceSound)
        Next

        Return SentenceSounds

    End Function


    Private Function CalculateListImbalance(ByVal CandidateListComposition As SortedList(Of Integer, List(Of SpeechMaterialComponent)), ByVal BalancedVariables As List(Of CustomVariableSpecification), ByVal IncludeNumericDistributionControl As Boolean) As List(Of Double)

        Dim CategoricalTargetDistributions As New List(Of Tuple(Of CustomVariableSpecification, SortedList(Of String, Double))) 'This lists the averall variable distributions for the whole material
        Dim CategoricalListDistributions As New List(Of List(Of Tuple(Of CustomVariableSpecification, SortedList(Of String, Double)))) 'This lists the list specific variable distributions for the whole material (list index is in the top level list)

        Dim NumericTargetDistributions As New List(Of Tuple(Of CustomVariableSpecification, SortedList(Of Double, Double))) 'The first double value represents upper interval limits, and the second double value represents the number within each interval
        Dim NumericListDistributions As New List(Of List(Of Tuple(Of CustomVariableSpecification, SortedList(Of Double, Double))))

        Dim NumericVariableGrandAverages As New List(Of Tuple(Of CustomVariableSpecification, Double))
        Dim NumericVariableGrandListAverages As New List(Of List(Of Tuple(Of CustomVariableSpecification, Double)))


        Dim AllSentenceList As New List(Of SpeechMaterialComponent) 'TODO: This step could be done before calling this function to optimize processing
        For Each List In CandidateListComposition.Values
            AllSentenceList.AddRange(List)
        Next

        For Each Variable In BalancedVariables
            If Variable.IsNumericVariable = False Then

                'Calculating and adding the distribution of all items
                CategoricalTargetDistributions.Add(New Tuple(Of CustomVariableSpecification, SortedList(Of String, Double))(Variable, GetCategoricalDistribution(AllSentenceList, Variable, Nothing)))

                'Calculating and adding the distributions within lists
                CategoricalListDistributions.Add(New List(Of Tuple(Of CustomVariableSpecification, SortedList(Of String, Double))))
                For Each List In CandidateListComposition.Values
                    CategoricalListDistributions.Last.Add(New Tuple(Of CustomVariableSpecification, SortedList(Of String, Double))(Variable, GetCategoricalDistribution(List, Variable, CategoricalTargetDistributions.Last.Item2.Keys.ToList)))
                Next

            Else

                If IncludeNumericDistributionControl = True Then
                    'Determining the number of bins (intervals) to use to describe the distribution. Using number of items in each list, divided by 10, and rounding upwards 
                    Dim ItemsPerList As Integer = CandidateListComposition.Values(0).Count ' Picking the list length from the first candidate list
                    Dim BinCount As Integer = Math.Ceiling(ItemsPerList / 10)

                    'Calculating and adding the distribution of all items
                    NumericTargetDistributions.Add(New Tuple(Of CustomVariableSpecification, SortedList(Of Double, Double))(Variable, GetNumericDistribution(AllSentenceList, Variable, BinCount, Nothing)))

                    'Calculating and adding the distributions within lists
                    NumericListDistributions.Add(New List(Of Tuple(Of CustomVariableSpecification, SortedList(Of Double, Double))))
                    For Each List In CandidateListComposition.Values
                        NumericListDistributions.Last.Add(New Tuple(Of CustomVariableSpecification, SortedList(Of Double, Double))(Variable, GetNumericDistribution(List, Variable, BinCount, NumericTargetDistributions.Last.Item2.Keys.ToList)))
                    Next
                End If

                'Calculating and adding the distribution of all items
                NumericVariableGrandAverages.Add(New Tuple(Of CustomVariableSpecification, Double)(Variable, GetNumericVariableAverage(AllSentenceList, Variable)))

                'Calculating and adding the distributions within lists
                NumericVariableGrandListAverages.Add(New List(Of Tuple(Of CustomVariableSpecification, Double)))
                For Each List In CandidateListComposition.Values
                    NumericVariableGrandListAverages.Last.Add(New Tuple(Of CustomVariableSpecification, Double)(Variable, GetNumericVariableAverage(List, Variable)))
                Next

            End If
        Next

        'NB TODO: 'Numeric distribution interval limits are no longer important, and could be replaced by categorical values so that the same RMS error algorithm below can be used for both types

        'Comparing distributions by RMS error for the categorical distributions
        Dim Distances As New List(Of Double) ' This list contains the average RMS error for each variable

        For i = 0 To CategoricalTargetDistributions.Count - 1

            Dim RmsErrorsPerList As New List(Of Double)

            'Dividing the target distribution by ListCount to get the same scale
            Dim TargetValues = CategoricalTargetDistributions(i).Item2.Values.ToArray
            Dim TotalObservations = TargetValues.Sum

            'Iterating over Lists
            For j = 0 To CategoricalListDistributions(i).Count - 1

                'We now have one list, compared to all lists
                Dim ListValues = CategoricalListDistributions(i)(j).Item2.Values.ToArray

                'Normalizing each list 
                Dim ListObservations = ListValues.Sum
                Dim CurrentScale As Double = TotalObservations / ListObservations

                'Calculating RMS error
                Dim SquaredErrors(ListValues.Length - 1) As Double

                'Iterating over variable values
                For q = 0 To SquaredErrors.Length - 1
                    SquaredErrors(q) = (CurrentScale * ListValues(q) - TargetValues(q)) ^ 2
                Next

                Dim RmsError = Math.Sqrt(SquaredErrors.Average)
                RmsErrorsPerList.Add(RmsError)
            Next

            'Adding the average RMS error across lists as the distance measure
            Distances.Add(RmsErrorsPerList.Average)

        Next

        If IncludeNumericDistributionControl = True Then

            'Comparing distributions by RMS error for the numeric distributions
            For i = 0 To NumericTargetDistributions.Count - 1

                Dim RmsErrorsPerList As New List(Of Double)

                'Dividing the target distribution ny ListCount to get the same scale
                Dim TargetValues = NumericTargetDistributions(i).Item2.Values.ToArray
                Dim TotalObservations = TargetValues.Sum

                'Iterating over Lists
                For j = 0 To NumericListDistributions(i).Count - 1

                    'We now have one list, compared to all lists
                    Dim ListValues = NumericListDistributions(i)(j).Item2.Values.ToArray

                    'Normalizing each list
                    Dim ListObservations = ListValues.Sum
                    Dim CurrentScale As Double = TotalObservations / ListObservations

                    'Calculating RMS error
                    Dim SquaredErrors(ListValues.Length - 1) As Double

                    'Iterating over variable values
                    For q = 0 To SquaredErrors.Length - 1
                        SquaredErrors(q) = (CurrentScale * ListValues(q) - TargetValues(q)) ^ 2
                    Next


                    Dim RmsError = Math.Sqrt(SquaredErrors.Average)
                    RmsErrorsPerList.Add(RmsError)
                Next

                'Adding the average RMS error across lists as the distance measure
                Distances.Add(RmsErrorsPerList.Average)

            Next
        End If

        'Comparing distributions by RMS error for the averages

        'Iterating over variables
        For i = 0 To NumericVariableGrandAverages.Count - 1

            Dim TargetVariableValue = NumericVariableGrandAverages(i).Item2

            'Iterating over Lists
            Dim SquaredErrors As New List(Of Double)
            For j = 0 To NumericVariableGrandListAverages(i).Count - 1

                'We now have one list, compared to all lists
                Dim ListValue = NumericVariableGrandListAverages(i)(j).Item2
                Dim SquaredError As Double = (ListValue - TargetVariableValue) ^ 2
                SquaredErrors.Add(SquaredError)
            Next

            'Calculating RMS error
            Dim RmsError = Math.Sqrt(SquaredErrors.Average)

            'Adding the RMS error of the list means as the distance measure
            Distances.Add(RmsError)

        Next


        'Returning the distances
        Return Distances

    End Function

    Private Function GetCategoricalDistribution(ByVal List As List(Of SpeechMaterialComponent), ByVal VariableSpecification As CustomVariableSpecification, Optional ByVal PossibleValuesList As List(Of String) = Nothing) As SortedList(Of String, Double)

        If VariableSpecification.IsNumericVariable = True Then Return Nothing

        Dim Output As New SortedList(Of String, Double)

        If PossibleValuesList IsNot Nothing Then
            'Filling up Output with all keys in PossibleValuesList so that they contain the equivalent keys as the target distribution
            For Each Key In PossibleValuesList
                Output.Add(Key, 0)
            Next
        End If

        For Each Sentence In List
            If VariableSpecification.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Sentence Then
                AddCategoricalVariableValue(Output, Sentence, VariableSpecification.VariableName)
            ElseIf VariableSpecification.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Word Then
                For Each Word In Sentence.ChildComponents
                    AddCategoricalVariableValue(Output, Word, VariableSpecification.VariableName)
                Next
            ElseIf VariableSpecification.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Phoneme Then
                For Each Word In Sentence.ChildComponents
                    For Each Phoneme In Word.ChildComponents
                        AddCategoricalVariableValue(Output, Phoneme, VariableSpecification.VariableName)
                    Next
                Next
            End If
        Next

        Return Output

    End Function

    Private Sub AddCategoricalVariableValue(ByRef TargetCollection As SortedList(Of String, Double), ByRef SpeechMaterialComponent As SpeechMaterialComponent, ByVal VariableName As String)

        Dim VariableValue = SpeechMaterialComponent.GetCategoricalVariableValue(VariableName).Trim
        If TargetCollection.ContainsKey(VariableValue) = False Then
            TargetCollection.Add(VariableValue, 1)
        Else
            TargetCollection(VariableValue) += 1
        End If

    End Sub

    Private Function GetNumericDistribution(ByVal List As List(Of SpeechMaterialComponent), ByVal VariableSpecification As CustomVariableSpecification, ByVal BinCount As Integer, Optional ByVal UpperIntervalLimits As List(Of Double) = Nothing) As SortedList(Of Double, Double)

        If VariableSpecification.IsNumericVariable = False Then Return Nothing

        Dim Output As New SortedList(Of Double, Double)

        If UpperIntervalLimits Is Nothing Then

            UpperIntervalLimits = New List(Of Double)

            'Determining min and max values of the full (target) distribution
            Dim AllvaluesList As New List(Of Double)
            For Each Sentence In List
                If VariableSpecification.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Sentence Then
                    Dim VariableValue = Sentence.GetNumericVariableValue(VariableSpecification.VariableName)
                    If VariableValue IsNot Nothing Then AllvaluesList.Add(VariableValue)
                ElseIf VariableSpecification.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Word Then
                    For Each Word In Sentence.ChildComponents
                        Dim VariableValue = Word.GetNumericVariableValue(VariableSpecification.VariableName)
                        If VariableValue IsNot Nothing Then AllvaluesList.Add(VariableValue)
                    Next
                ElseIf VariableSpecification.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Phoneme Then
                    For Each Word In Sentence.ChildComponents
                        For Each Phoneme In Word.ChildComponents
                            Dim VariableValue = Phoneme.GetNumericVariableValue(VariableSpecification.VariableName)
                            If VariableValue IsNot Nothing Then AllvaluesList.Add(VariableValue)
                        Next
                    Next
                End If
            Next

            'Sorting AllvaluesList
            AllvaluesList.Sort()

            'Adding intervals
            Dim DistributionMin As Double = AllvaluesList.Min
            Dim DistributionMax As Double = AllvaluesList.Max
            Dim IntervalRange As Double = DistributionMax - DistributionMin
            Dim IntervalWidth As Double = IntervalRange / BinCount

            For n = 1 To BinCount - 1
                UpperIntervalLimits.Add(DistributionMin + n * IntervalWidth)
            Next

            'Adding also double max value to get all values above the 9:th interval
            UpperIntervalLimits.Add(Double.MaxValue)

        End If

        'Filling up Output with interval limits
        For Each Key In UpperIntervalLimits
            Output.Add(Key, 0)
        Next

        For Each Sentence In List
            If VariableSpecification.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Sentence Then
                AddNumericVariableToDistribution(Output, Sentence, VariableSpecification.VariableName)
            ElseIf VariableSpecification.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Word Then
                For Each Word In Sentence.ChildComponents
                    AddNumericVariableToDistribution(Output, Word, VariableSpecification.VariableName)
                Next
            ElseIf VariableSpecification.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Phoneme Then
                For Each Word In Sentence.ChildComponents
                    For Each Phoneme In Word.ChildComponents
                        AddNumericVariableToDistribution(Output, Phoneme, VariableSpecification.VariableName)
                    Next
                Next
            End If
        Next

        Return Output

    End Function

    Private Sub AddNumericVariableToDistribution(ByRef TargetCollection As SortedList(Of Double, Double), ByRef SpeechMaterialComponent As SpeechMaterialComponent, ByVal VariableName As String)

        Dim IntervalList As List(Of Double) = TargetCollection.Keys.ToList

        Dim VariableValue = SpeechMaterialComponent.GetNumericVariableValue(VariableName)

        'Determining in which interval the value falls
        For i = 0 To IntervalList.Count - 1
            If VariableValue < IntervalList(i) Then
                'All keys should already have been added to TargetCollection, and there is thus no need to check that here
                TargetCollection(IntervalList(i)) += 1
                'Exiting loop after the value has been added to its interval
                Exit For
            End If
        Next

    End Sub

    Private Function GetNumericVariableAverage(ByVal List As List(Of SpeechMaterialComponent), ByVal VariableSpecification As CustomVariableSpecification) As Double

        If VariableSpecification.IsNumericVariable = False Then Throw New Exception("Detected non-numeric variable where numeric variable was expected")

        Dim AllvaluesList As New List(Of Double)
        For Each Sentence In List
            If VariableSpecification.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Sentence Then
                Dim VariableValue = Sentence.GetNumericVariableValue(VariableSpecification.VariableName)
                If VariableValue IsNot Nothing Then AllvaluesList.Add(VariableValue)
            ElseIf VariableSpecification.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Word Then
                For Each Word In Sentence.ChildComponents
                    Dim VariableValue = Word.GetNumericVariableValue(VariableSpecification.VariableName)
                    If VariableValue IsNot Nothing Then AllvaluesList.Add(VariableValue)
                Next
            ElseIf VariableSpecification.LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.Phoneme Then
                For Each Word In Sentence.ChildComponents
                    For Each Phoneme In Word.ChildComponents
                        Dim VariableValue = Phoneme.GetNumericVariableValue(VariableSpecification.VariableName)
                        If VariableValue IsNot Nothing Then AllvaluesList.Add(VariableValue)
                    Next
                Next
            End If
        Next

        Return AllvaluesList.Average

    End Function



    Public Enum OrderType
        Original
        Random
        Balanced
        CustomId
        CustomPrimaryStringRepresentation
    End Enum


End Class
