Imports System.Windows.Forms.VisualStyles

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

        RandomOrder_RadioButton.Checked = True

    End Sub

    Private Sub RearrangeAcrossLists_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles RearrangeAcrossLists_RadioButton.CheckedChanged

        ListDescriptives_GroupBox.Enabled = True
        RandomOrder_RadioButton.Enabled = True
        OriginalOrder_RadioButton.Enabled = True
        BalancedOrder_RadioButton.Enabled = True

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
        CustomVariablesSelection_TableLayoutPanel.Enabled = False
        CustomVariablesSelection_GroupBox.Visible = False
    End Sub

    Private Sub OriginalOrder_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles OriginalOrder_RadioButton.CheckedChanged
        CustomVariablesSelection_TableLayoutPanel.Enabled = False
        CustomVariablesSelection_GroupBox.Visible = False
    End Sub

    Private Sub BalancedOrder_RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles BalancedOrder_RadioButton.CheckedChanged

        If BalancedOrder_RadioButton.Checked = True Then
            CustomVariablesSelection_TableLayoutPanel.Enabled = True
            CustomVariablesSelection_GroupBox.Visible = True
            AddCustomVariablesToSelectionBox()
        Else
            CustomVariablesSelection_TableLayoutPanel.Enabled = False
            CustomVariablesSelection_GroupBox.Visible = False
            CustomVariablesSelection_TableLayoutPanel.Controls.Clear()
        End If

    End Sub

    Private Sub AddCustomVariablesToSelectionBox()

        CustomVariablesSelection_TableLayoutPanel.SuspendLayout()

        CustomVariablesSelection_TableLayoutPanel.Controls.Clear()

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
                NewVariableControl.VariableName = VariableName
                NewVariableControl.LinguisticLevel = LinguisticLevel

                'Determining if the variable is numeric or not, and setting the corresponding variable selection control value
                Dim IsNumericVariable As Boolean = CustomVariable.Value
                NewVariableControl.IsNumericVariable = IsNumericVariable

                'Setting a random background color on the control
                NewVariableControl.BackColor = Drawing.Color.FromArgb(20, CSng(rnd.Next(10, 255)), CSng(rnd.Next(10, 255)), CSng(rnd.Next(10, 255)))

                'Adding the variable selection control
                NewVariableControl.Dock = Windows.Forms.DockStyle.Fill
                CustomVariablesSelection_TableLayoutPanel.Controls.Add(NewVariableControl)

            Next
        Next

        CustomVariablesSelection_TableLayoutPanel.Controls.Add(New Windows.Forms.Panel With {.Dock = Windows.Forms.DockStyle.Fill})

        CustomVariablesSelection_TableLayoutPanel.RowStyles.Clear()
        For r = 0 To CustomVariablesSelection_TableLayoutPanel.Controls.Count - 1
            CustomVariablesSelection_TableLayoutPanel.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Absolute, 26))
        Next

        CustomVariablesSelection_TableLayoutPanel.ResumeLayout()


    End Sub

    Private Class VariableSelectionCheckBox
        Inherits Windows.Forms.CheckBox

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

        Dim BalancedVariables As New List(Of String)
        If OrderType = OrderType.Balanced Then
            For Each Control In CustomVariablesSelection_TableLayoutPanel.Controls
                Dim CastControl = TryCast(Control, VariableSelectionCheckBox)
                If CastControl IsNot Nothing Then
                    If CastControl.Checked = True Then
                        BalancedVariables.Add(CastControl.VariableName)
                        'CastControl.IsNumericVariable is also available if needed
                    End If
                End If
            Next

            If BalancedVariables.Count = 0 Then
                MsgBox("You must select at least one variable to balance between lists!")
                Exit Sub
            End If
        End If

        'Rearranging lists (TODO: the following code should probably be moved to the MediaSet class?
        Rearrange(ReArrangeAcrossLists, OrderType, TargetListLength, BalancedVariables, NewMediasSetName, NewSpeechMaterialName, ListNamePrefix)



    End Sub

    Public Sub Rearrange(ByVal ReArrangeAcrossLists As Boolean, ByVal OrderType As OrderType, ByVal TargetListLength As Integer, ByVal BalancedVariables As List(Of String), ByVal NewMediasSetName As String, ByVal NewSpeechMaterialName As String, ByVal ListNamePrefix As String, Optional ByVal SentencePrefix As String = "Sentence")

        Dim rnd = New Random

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
                End Select
            Next

        Else

            'Mixing across lists

            'Creating a variable that holds the current output list index
            Dim CurrentOutputListIndex As Integer = -1

            'Getting the all sentence level SMCs in the whole speech material
            Dim AllSentences = SourceMediaSet.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)

            'Getting a vector of indices, sequential or random
            Dim CurrentSentenceOrder() As Integer
            Select Case OrderType
                Case OrderType.Random, OrderType.Balanced
                    CurrentSentenceOrder = Utils.SampleWithoutReplacement(AllSentences.Count, 0, AllSentences.Count, rnd)
                Case OrderType.Original
                    CurrentSentenceOrder = Utils.GetSequence(0, AllSentences.Count - 1, 1)
                Case Else
                    Throw New NotImplementedException("Unkown order type")
            End Select

            'Balancing order 
            If OrderType = OrderType.Balanced Then

                Dim MaxIterations As Integer = 1000 'TODO this should be a method paramenter
                Dim NumberOfSwapsInEachIteration As Integer = 5 ' This could be a method parameter

                'Setting OptimalOrder initially to CurrentSentenceOrder
                Dim OptimalOrder() As Integer = CurrentSentenceOrder
                Dim LowestListImbalance As Double = Double.MaxValue

                For Iteration = 0 To MaxIterations

                    '1. Creating a new candidate order by swapping some random indices
                    Dim CandidateOrder() As Integer
                    CandidateOrder = OptimalOrder.Clone 'TODO does this work?
                    If Iteration = 0 Then
                        'Using the start order in the first iteration
                    Else
                        For i = 1 To NumberOfSwapsInEachIteration
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
                    For s = 0 To AllSentences.Count - 1

                        'Adding a new list at multiples of TargetListLength
                        If s Mod TargetListLength = 0 Then
                            CandidateListIndex += 1
                            RearrangedMaterial.Add(CandidateListIndex, New List(Of Tuple(Of SpeechMaterialComponent, List(Of Audio.Sound))))
                        End If

                        'Adding the sentence to list CandidateListIndex 
                        CandidateListComposition(CandidateListIndex).Add(AllSentences(CandidateOrder(s)))
                    Next

                    '2. Evaluating the order
                    'Calculating the im-balance measure
                    Dim CurrentListImbalance = CalculateListImbalance(CandidateListComposition, BalancedVariables)

                    'Comparing the imbalance value to the value from the previous best iteration
                    If CurrentListImbalance < LowestListImbalance Then
                        'The new order inproved the balance 

                        'Storing the new LowestListImbalance value
                        LowestListImbalance = CurrentListImbalance

                        'Storing the candidate order as the new optimal order
                        OptimalOrder = CandidateOrder

                        'X. Reporting progress
                        Console.WriteLine("Iteration:" & Iteration & vbTab & "Lower imbalance detected: " & LowestListImbalance)

                    Else
                        'The new order did not improve the balance, ignoring it and keeps the optimal order

                        'X. Reporting progress in with regular intervals
                        If Iteration Mod 100 = 0 Then
                            Console.WriteLine("Iteration:" & Iteration)
                        End If

                    End If

                    'X. Quitting if MaxIterations is reached
                    If Iteration >= MaxIterations Then Exit For

                Next

                'When iterations has stopped we take the sentence order in the selected optimal iteration
                CurrentSentenceOrder = OptimalOrder

            End If

            'Picking sentences in the order specified in CurrentSentenceOrder
            For s = 0 To AllSentences.Count - 1

                'Adding a new list at multiples of TargetListLength
                If s Mod TargetListLength = 0 Then
                    CurrentOutputListIndex += 1
                    RearrangedMaterial.Add(CurrentOutputListIndex, New List(Of Tuple(Of SpeechMaterialComponent, List(Of Audio.Sound))))
                End If

                'Getting the sentence level sound with modified SMA chunk
                Dim SentenceSounds = GetSentenceSounds(AllSentences(CurrentSentenceOrder(s)), SoundChannel)

                'Adding the SMC and its sounds
                RearrangedMaterial(CurrentOutputListIndex).Add(New Tuple(Of SpeechMaterialComponent, List(Of Audio.Sound))(AllSentences(CurrentSentenceOrder(s)), SentenceSounds))

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
                Sentence.Item1.Id = "L" & i.ToString("00") & "S" & s.ToString("00")
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

        OutputSMC.WriteSpeechMaterialToFile(OutputSMC.ParentTestSpecification, OutputSMC.ParentTestSpecification.GetTestRootPath)

        NewMediaSet.WriteToFile()

        NewMediaSet.WriteCustomVariables()

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


    Private Function CalculateListImbalance(ByVal CandidateListComposition As SortedList(Of Integer, List(Of SpeechMaterialComponent)), ByVal BalancedVariables As List(Of String)) As Double




    End Function

    Public Enum OrderType
        Original
        Random
        Balanced
    End Enum

End Class
