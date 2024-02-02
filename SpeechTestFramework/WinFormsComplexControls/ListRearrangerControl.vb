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
        Rearrange(ReArrangeAcrossLists, OrderType, TargetListLength, NewMediasSetName, NewSpeechMaterialName, ListNamePrefix)



    End Sub

    Public Sub Rearrange(ByVal ReArrangeAcrossLists As Boolean, ByVal OrderType As OrderType, ByVal TargetListLength As Integer, ByVal NewMediasSetName As String, ByVal NewSpeechMaterialName As String, ByVal ListNamePrefix As String, Optional ByVal SentencePrefix As String = "Sentence")

        Dim rnd = New Random

        'Clears previously loaded sounds
        SpeechMaterialComponent.ClearAllLoadedSounds()

        'Setting read sound channel
        Dim SoundChannel As Integer = 1

        Dim MaterialToRearrange As New SortedList(Of Integer, List(Of SpeechMaterialComponent))
        If ReArrangeAcrossLists = True Then
            Dim AllSentences = SourceMediaSet.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
            MaterialToRearrange.Add(0, AllSentences)
        Else
            Dim AllLists = SourceMediaSet.ParentTestSpecification.SpeechMaterial.GetAllRelativesAtLevel(SpeechMaterialComponent.LinguisticLevels.List)
            For i = 0 To AllLists.Count - 1
                Dim AllSentences = AllLists(i).GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence)
                MaterialToRearrange.Add(i, AllSentences)
            Next
        End If

        Dim RearrangedMaterial As New SortedList(Of Integer, List(Of Tuple(Of SpeechMaterialComponent, List(Of Audio.Sound))))

        For i = 0 To MaterialToRearrange.Count - 1

            RearrangedMaterial.Add(i, New List(Of Tuple(Of SpeechMaterialComponent, List(Of Audio.Sound))))

            'Getting the current SMCs
            Dim CurrentSMCs = MaterialToRearrange(i)

            'Overriding list length in some cases
            If ReArrangeAcrossLists = False Then
                'We keep the same list length
                TargetListLength = CurrentSMCs.Count
            End If

            'Mixing lists and getting their sounds
            Select Case OrderType
                Case OrderType.Original, OrderType.Random

                    Dim CurrentIndices() As Integer

                    If OrderType = OrderType.Random Then
                        CurrentIndices = Utils.SampleWithoutReplacement(TargetListLength, 0, TargetListLength, rnd)
                    Else
                        CurrentIndices = Utils.GetSequence(0, TargetListLength - 1, 1)
                    End If

                    For s = 0 To TargetListLength - 1
                        Dim CurrentIndex = CurrentIndices(s)

                        'Exiting loop if no more SMCs are available (i.e. target list length set too high)
                        If CurrentIndex > CurrentSMCs.Count - 1 Then Exit For

                        'Getting the sound
                        Dim SentenceSounds As New List(Of Audio.Sound)
                        For r = 0 To SourceMediaSet.MediaAudioItems - 1
                            Dim SmaComponents As New List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)
                            Dim CurrentSentenceSound = CurrentSMCs(CurrentIndex).GetSound(SourceMediaSet, r, SoundChannel,,,,,,,,, SmaComponents)
                            If SmaComponents.Count > 1 Then MsgBox("An error has been detected in the SMA components. The process will likely fail.")
                            CurrentSentenceSound.SMA.ChannelData(SoundChannel).Clear()
                            CurrentSentenceSound.SMA.ChannelData(SoundChannel).Add(SmaComponents(0))
                            SmaComponents(0).ParentComponent = CurrentSentenceSound.SMA.ChannelData(SoundChannel)
                            'Shifting
                            CurrentSentenceSound.SMA.ChannelData(SoundChannel)(0).TimeShift(-CurrentSentenceSound.SMA.ChannelData(SoundChannel)(0).StartSample)
                            SentenceSounds.Add(CurrentSentenceSound)
                        Next

                        'Adding the SMC and its sounds
                        RearrangedMaterial(i).Add(New Tuple(Of SpeechMaterialComponent, List(Of Audio.Sound))(CurrentSMCs(CurrentIndex), SentenceSounds))

                    Next

                Case OrderType.Balanced

                    Throw New NotImplementedException
            End Select

        Next

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

    Public Enum OrderType
        Original
        Random
        Balanced
    End Enum

End Class
