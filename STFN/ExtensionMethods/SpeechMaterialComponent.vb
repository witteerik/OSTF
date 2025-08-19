Imports System.Runtime.CompilerServices
Imports STFN.Core
Imports System.Globalization
Imports STFN.Core.Audio.Sound.SpeechMaterialAnnotation
Imports STFN.Core.SpeechMaterialComponent

'This file contains extension methods for STFN.Core.SpeechMaterialComponent

Partial Public Module Extensions

    <Extension>
    Public Function GetTestSituationVariableValue(obj As SpeechMaterialComponent)
        'This function should somehow returns the requested variable values from the indicated test situation, or even offer an option to create/calculate that data if not present.
        Throw New NotImplementedException
    End Function

    <Extension>
    Public Function GetRandomNumber(obj As SpeechMaterialComponent) As Double
        Return obj.Randomizer.NextDouble()
    End Function

    <Extension>
    Public Sub SetIdAsCategoricalCustumVariable(obj As SpeechMaterialComponent, ByVal CascadeToLowerLevels As Boolean)

        obj.SetCategoricalVariableValue("Id", obj.Id)

        If CascadeToLowerLevels = True Then
            For Each ChildComponent In obj.ChildComponents
                ChildComponent.SetIdAsCategoricalCustumVariable(CascadeToLowerLevels)
            Next
        End If

    End Sub

    '''' <summary>
    '''' Returns the audio representing the speech material component as a new Audio.Sound. If the recordings of the component is split between different sound files, a concatenated (using optional overlap/crossfade period) sound is returned.
    '''' </summary>
    '''' <param name="MediaSet"></param>
    '''' <param name="Index"></param>
    '''' <param name="SoundChannel"></param>
    '''' <param name="CrossFadeLength">The length (in sample) of a cross-fade section.</param>
    '''' <param name="InitialMargin">If referenced in the calling code, returns the number of samples prior to the first sample in the first sound file used in for returning the SMC soundv.</param>
    '''' <param name="SoundSourceSmaComponents">If referenced in the calling code, contains a list of the source sma components from which the sound was loaded.</param>
    '''' <returns></returns>
    'Public Function GetSound(ByRef MediaSet As MediaSet, ByVal Index As Integer, ByVal SoundChannel As Integer, Optional CrossFadeLength As Integer? = Nothing, Optional ByRef InitialMargin As Integer = 0,
    '                         Optional ByRef SoundSourceSmaComponents As List(Of SmaComponent) = Nothing, Optional ByVal SupressWarnings As Boolean = False, Optional RemoveInterComponentSound As Boolean = False) As Audio.Sound

    '    'Setting initial margin to -1 to signal that it has not been set
    '    InitialMargin = -1

    '    Dim CorrespondingSmaComponentList = GetCorrespondingSmaComponent(MediaSet, Index, SoundChannel, True)

    '    If RemoveInterComponentSound = True Then
    '        Dim ChildAdded As Boolean = False
    '        Dim ParentAdded As Boolean = False
    '        Dim MixedLevelsAdded As Boolean = False
    '        Dim TempSmaList = New List(Of SmaComponent)
    '        For Each SmaComponent In CorrespondingSmaComponentList
    '            If SmaComponent.Count > 0 Then
    '                For Each ChildComponent In SmaComponent
    '                    'Adding child components instead of
    '                    TempSmaList.Add(ChildComponent)
    '                    ChildAdded = True
    '                    If ParentAdded = True Then MixedLevelsAdded = True
    '                Next
    '            Else
    '                TempSmaList.Add(SmaComponent)
    '                ParentAdded = True
    '                If ChildAdded = True Then MixedLevelsAdded = True
    '            End If
    '            CorrespondingSmaComponentList = TempSmaList
    '        Next

    '        If MixedLevelsAdded = True Then
    '            If SupressWarnings = False Then
    '                MsgBox("Mixed SMA component levels in GetSound! This is probably caused by segmentations of different linguistic depths in the material, and is not allowed!")
    '                Return Nothing
    '            End If
    '        End If

    '    End If

    '    'Referencing CorrespondingSmaComponentList
    '    SoundSourceSmaComponents = CorrespondingSmaComponentList

    '    If CorrespondingSmaComponentList.Count = 0 Then
    '        Return Nothing
    '    ElseIf CorrespondingSmaComponentList.Count = 1 Then
    '        Return CorrespondingSmaComponentList(0).GetSoundFileSection(SoundChannel, SupressWarnings, InitialMargin)
    '    Else
    '        Dim SoundList As New List(Of Audio.Sound)
    '        For Each SmaComponent In CorrespondingSmaComponentList
    '            Dim CurrentSound = SmaComponent.GetSoundFileSection(SoundChannel, SupressWarnings, InitialMargin)
    '            'Returns nothing if any sound could not be loaded
    '            If CurrentSound Is Nothing Then Return Nothing
    '            SoundList.Add(CurrentSound)
    '        Next
    '        Return Audio.DSP.ConcatenateSounds(SoundList, ,,,,, CrossFadeLength)
    '    End If

    '    'Changing InitialMargin to 0 if it was never set
    '    If InitialMargin < 0 Then InitialMargin = 0

    'End Function

    <Extension>
    Public Function GetDurationOfContrastingComponents(obj As SpeechMaterialComponent, ByRef MediaSet As MediaSet,
                                                               ByVal ContrastLevel As SpeechMaterialComponent.LinguisticLevels,
                                                       ByVal MediaItemIndex As Integer,
                                                               ByVal SoundChannel As Integer) As List(Of Double)


        Dim TargetComponents = obj.GetAllDescenentsAtLevel(ContrastLevel, True)


        'Get the SMA components representing the sound sections of all target components
        Dim CurrentSmaComponentList As New List(Of STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent)

        For c = 0 To TargetComponents.Count - 1

            'Determine if is contraisting component??
            If TargetComponents(c).IsContrastingComponent = False Then
                Continue For
            End If

            CurrentSmaComponentList.AddRange(TargetComponents(c).GetCorrespondingSmaComponent(MediaSet, MediaItemIndex, SoundChannel, True))

        Next

        Dim DurationList As New List(Of Double)

        'Getting the actual sound sections and measures their durations
        For Each SmaComponent In CurrentSmaComponentList
            DurationList.Add(SmaComponent.Length / SmaComponent.ParentSMA.ParentSound.WaveFormat.SampleRate)
        Next

        Return DurationList

    End Function


    ''' <summary>
    ''' Locates and returns the first speech material component that has (or is planned should have) a sound recording, based on the AudioFileLinguisticLevel of the selected Mediaset.
    ''' </summary>
    ''' <param name="MediaSet"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GetFirstRelativeWithSound(obj As SpeechMaterialComponent, ByRef MediaSet As MediaSet) As SpeechMaterialComponent

        If obj.LinguisticLevel = MediaSet.AudioFileLinguisticLevel Then

            Return obj

        ElseIf obj.LinguisticLevel > MediaSet.AudioFileLinguisticLevel Then

            'E.g. SMC is a Word and AudioFileLinguisticLevel is a Sentence
            Return obj.GetAncestorAtLevel(MediaSet.AudioFileLinguisticLevel)

        ElseIf obj.LinguisticLevel < MediaSet.AudioFileLinguisticLevel Then

            'E.g. SMC is a List and AudioFileLinguisticLevel is a sentence
            Dim SmcsWithSoundFile = obj.GetAllDescenentsAtLevel(MediaSet.AudioFileLinguisticLevel)
            If SmcsWithSoundFile.Count > 0 Then
                Return SmcsWithSoundFile(0)
            Else
                Return Nothing
            End If

        Else
            'This is odd, VB warns for lacking return path without this line, but the above should cover all paths...
            Return Nothing
        End If

    End Function

    ''' <summary>
    ''' Returns the wave file format of the first located speech material component at the AudioFileLinguisticLevel of the selected Mediaset, or Nothing is no sound file is found.
    ''' </summary>
    ''' <param name="MediaSet"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GetWavefileFormat(obj As SpeechMaterialComponent, ByRef MediaSet As MediaSet) As STFN.Core.Audio.Formats.WaveFormat

        Dim FirstRelativeWithSound = obj.GetFirstRelativeWithSound(MediaSet)

        Dim ComponentSound = FirstRelativeWithSound.GetSound(MediaSet, 0, 1)

        If ComponentSound IsNot Nothing Then
            Return ComponentSound.WaveFormat
        Else
            Return Nothing
        End If

    End Function


    ''' <summary>
    ''' Returns a sound containing a concatenation of all sound recordings at the specified SectionsLevel within the current speech material component.
    ''' </summary>
    ''' <param name="SegmentsLevel">The (lower) linguistic level from which the sections to be concatenaded are taken.</param>
    ''' <param name="OnlyLinguisticallyContrastingSegments">If set to true, only contrasting speech material components (e.g. contrasting phonemes in minimal pairs) will be included in the spectrum level calculations.</param>
    ''' <param name="SoundChannel">The audio / wave file channel in which the speech is recorded (channel 1, for mono sounds).</param>
    ''' <param name="SkipPractiseComponents">If set to true, speech material components marksed as practise components will be skipped in the spectrum level calculations.</param>
    ''' <param name="MinimumSegmentDuration">An optional minimum duration (in seconds) of each included component. If the recorded sound of a component is shorter, it will be zero-padded to the indicated duration.</param>
    ''' <param name="ComponentCrossFadeDuration">A duration by which the sections for concatenations will be cross-faded prior to spectrum level calculations.</param>
    ''' <param name="FadeConcatenatedSound">If set to true, the concatenated sounds will be slightly faded initially and finally (in order to avoid impulse-like onsets and offsets) prior to spectrum level calculations.</param>
    ''' <param name="RemoveDcComponent">If set to true, the DC component of the concatenated sounds will be set to zero prior to spectrum level calculations.</param>
    <Extension>
    Public Function GetConcatenatedComponentsSound(obj As SpeechMaterialComponent, ByRef MediaSet As MediaSet,
                                                   ByVal SegmentsLevel As SpeechMaterialComponent.LinguisticLevels,
                                                   ByVal OnlyLinguisticallyContrastingSegments As Boolean,
                                                   ByVal SoundChannel As Integer,
                                                   ByVal SkipPractiseComponents As Boolean,
                                                   Optional ByVal MinimumSegmentDuration As Double = 0,
                                                   Optional ByVal ComponentCrossFadeDuration As Double = 0.001,
                                                   Optional ByVal FadeConcatenatedSound As Boolean = True,
                                                   Optional ByVal RemoveDcComponent As Boolean = True,
                                                   Optional ByVal IncludeSelf As Boolean = False) As STFN.Core.Audio.Sound


        Dim TargetComponents = obj.GetAllDescenentsAtLevel(SegmentsLevel, IncludeSelf)

        'Get the SMA components representing the sound sections of all target components
        Dim CurrentSmaComponentList As New List(Of STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent)

        For c = 0 To TargetComponents.Count - 1

            If SkipPractiseComponents = True Then
                If TargetComponents(c).IsPractiseComponent = True Then
                    Continue For
                End If
            End If

            If OnlyLinguisticallyContrastingSegments = True Then
                'Determine if is contraisting component??
                If TargetComponents(c).IsContrastingComponent = False Then
                    Continue For
                End If
            End If

            For i = 0 To MediaSet.MediaAudioItems - 1
                CurrentSmaComponentList.AddRange(TargetComponents(c).GetCorrespondingSmaComponent(MediaSet, i, SoundChannel, Not SkipPractiseComponents))
            Next

        Next

        'Skipping to next Summary component if no
        If CurrentSmaComponentList.Count = 0 Then Return Nothing

        'Getting the actual sound sections
        Dim SoundSectionList As New List(Of STFN.Core.Audio.Sound)
        Dim WaveFormat As STFN.Core.Audio.Formats.WaveFormat = Nothing
        For Each SmaComponent In CurrentSmaComponentList

            Dim SoundSegment = SmaComponent.GetSoundFileSection(SoundChannel)
            If MinimumSegmentDuration > 0 Then
                SoundSegment.ZeroPad(MinimumSegmentDuration, True)
            End If
            SoundSectionList.Add(SoundSegment)

            'Getting the WaveFormat from the first available sound
            If WaveFormat Is Nothing Then WaveFormat = SoundSegment.WaveFormat

        Next

        'Concatenates the sounds
        Dim ConcatenatedSound = DSP.ConcatenateSounds(SoundSectionList, False,,,,, ComponentCrossFadeDuration * WaveFormat.SampleRate, False, 10, True)

        'Fading very slightly to avoid initial and final impulses
        If FadeConcatenatedSound = True Then
            DSP.Fade(ConcatenatedSound, Nothing, 0,,, ConcatenatedSound.WaveFormat.SampleRate * 0.01, DSP.FadeSlopeType.Linear)
            DSP.Fade(ConcatenatedSound, 0, Nothing,, ConcatenatedSound.WaveData.SampleData(1).Length - ConcatenatedSound.WaveFormat.SampleRate * 0.01,, DSP.FadeSlopeType.Linear)
        End If

        'Removing DC-component
        If RemoveDcComponent = True Then DSP.RemoveDcComponent(ConcatenatedSound)

        Return ConcatenatedSound

    End Function


    ''' <summary>
    ''' Returns a list of sounds containing a concatenations of all sound recordings at the specified SectionsLevel within the current speech material component.
    ''' </summary>
    ''' <param name="SegmentsLevel">The (lower) linguistic level from which the sections to be concatenaded are taken.</param>
    ''' <param name="OnlyLinguisticallyContrastingSegments">If set to true, only contrasting speech material components (e.g. contrasting phonemes in minimal pairs) will be included in the spectrum level calculations.</param>
    ''' <param name="SoundChannel">The audio / wave file channel in which the speech is recorded (channel 1, for mono sounds).</param>
    ''' <param name="SkipPractiseComponents">If set to true, speech material components marksed as practise components will be skipped in the spectrum level calculations.</param>
    ''' <param name="MinimumSegmentDuration">An optional minimum duration (in seconds) of each included component. If the recorded sound of a component is shorter, it will be zero-padded to the indicated duration.</param>
    ''' <param name="ComponentCrossFadeDuration">A duration by which the sections for concatenations will be cross-faded prior to spectrum level calculations.</param>
    ''' <param name="FadeConcatenatedSound">If set to true, the concatenated sounds will be slightly faded initially and finally (in order to avoid impulse-like onsets and offsets) prior to spectrum level calculations.</param>
    ''' <param name="RemoveDcComponent">If set to true, the DC component of the concatenated sounds will be set to zero prior to spectrum level calculations.</param>
    <Extension>
    Public Function GetConcatenatedComponentsSounds(obj As SpeechMaterialComponent, ByRef MediaSet As MediaSet,
                                                   ByVal SegmentsLevel As SpeechMaterialComponent.LinguisticLevels,
                                                   ByVal OnlyLinguisticallyContrastingSegments As Boolean,
                                                   ByVal SoundChannel As Integer,
                                                   ByVal SkipPractiseComponents As Boolean,
                                                   Optional ByVal MinimumSegmentDuration As Double = 0,
                                                   Optional ByVal ComponentCrossFadeDuration As Double = 0.001,
                                                   Optional ByVal FadeConcatenatedSound As Boolean = True,
                                                   Optional ByVal RemoveDcComponent As Boolean = True) As List(Of STFN.Core.Audio.Sound)

        Throw New Exception("This function has not yet been debugged!")

        Dim DescenentComponents = obj.GetAllDescenentsAtLevel(SegmentsLevel)

        Dim CousinComponentList As New SortedList(Of Integer, List(Of SpeechMaterialComponent))

        'Splitting up into list of cousin components
        For c = 0 To DescenentComponents.Count - 1
            Dim SelfIndex = DescenentComponents(c).GetSelfIndex()
            If SelfIndex IsNot Nothing Then
                If CousinComponentList.ContainsKey(SelfIndex) = False Then CousinComponentList.Add(SelfIndex, New List(Of SpeechMaterialComponent))
                CousinComponentList(SelfIndex).Add(DescenentComponents(c))
            Else
                CousinComponentList.Add(-1, New List(Of SpeechMaterialComponent) From {DescenentComponents(c)})
            End If
        Next

        Dim OutputSounds As New List(Of STFN.Core.Audio.Sound)
        For Each kvp In CousinComponentList

            Dim TargetComponents = kvp.Value

            'Get the SMA components representing the sound sections of all target components
            Dim CurrentSmaComponentList As New List(Of STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent)

            For c = 0 To TargetComponents.Count - 1

                If SkipPractiseComponents = True Then
                    If TargetComponents(c).IsPractiseComponent = True Then
                        Continue For
                    End If
                End If

                If OnlyLinguisticallyContrastingSegments = True Then
                    'Determine if is contraisting component??
                    If TargetComponents(c).IsContrastingComponent = False Then
                        Continue For
                    End If
                End If

                For i = 0 To MediaSet.MediaAudioItems - 1
                    CurrentSmaComponentList.AddRange(TargetComponents(c).GetCorrespondingSmaComponent(MediaSet, i, SoundChannel, Not SkipPractiseComponents))
                Next

            Next

            'Skipping to next Summary component if no
            If CurrentSmaComponentList.Count = 0 Then Continue For

            'Getting the actual sound sections
            Dim SoundSectionList As New List(Of STFN.Core.Audio.Sound)
            Dim WaveFormat As STFN.Core.Audio.Formats.WaveFormat = Nothing
            For Each SmaComponent In CurrentSmaComponentList

                Dim SoundSegment = SmaComponent.GetSoundFileSection(SoundChannel)
                If MinimumSegmentDuration > 0 Then
                    SoundSegment.ZeroPad(MinimumSegmentDuration, True)
                End If
                SoundSectionList.Add(SoundSegment)

                'Getting the WaveFormat from the first available sound
                If WaveFormat Is Nothing Then WaveFormat = SoundSegment.WaveFormat

            Next

            'Concatenates the sounds
            Dim ConcatenatedSound = DSP.ConcatenateSounds(SoundSectionList, False,,,,, ComponentCrossFadeDuration * WaveFormat.SampleRate, False, 10, True)

            'Fading very slightly to avoid initial and final impulses
            If FadeConcatenatedSound = True Then
                DSP.Fade(ConcatenatedSound, Nothing, 0,,, ConcatenatedSound.WaveFormat.SampleRate * 0.01, DSP.FadeSlopeType.Linear)
                DSP.Fade(ConcatenatedSound, 0, Nothing,, ConcatenatedSound.WaveData.SampleData(1).Length - ConcatenatedSound.WaveFormat.SampleRate * 0.01,, DSP.FadeSlopeType.Linear)
            End If

            'Removing DC-component
            If RemoveDcComponent = True Then DSP.RemoveDcComponent(ConcatenatedSound)

            OutputSounds.Add(ConcatenatedSound)

        Next

        If OutputSounds.Count = 0 Then Return Nothing

        Return OutputSounds

    End Function


    <Extension>
    Public Function GetAllLoadedSounds(obj As SpeechMaterialComponent) As SortedList(Of String, STFN.Core.Audio.Sound)
        Return SoundLibrary
    End Function


    ''' <summary>
    ''' Saves all loaded sounds to their original location.
    ''' </summary>
    ''' <param name="SaveOnlyModified"></param>
    <Extension>
    Public Sub SaveAllLoadedSounds(obj As SpeechMaterialComponent, Optional ByVal SaveOnlyModified As Boolean = True)

        For Each CurrentSound In SoundLibrary
            If SaveOnlyModified = True Then
                If CurrentSound.Value.IsChanged = False Then
                    Continue For
                End If
            End If

            CurrentSound.Value.WriteWaveFile(CurrentSound.Key)
        Next

    End Sub


    ''' <summary>
    ''' Sets the nominal level property of all loaded sound files to TargetSoundLevel
    ''' </summary>
    ''' <param name="TargetSoundLevel"></param>
    <Extension>
    Public Sub StoreNominalLevelValueInAllLoadedSounds(obj As SpeechMaterialComponent, ByVal TargetSoundLevel As Double?)
        For Each CurrentSound In SoundLibrary
            If CurrentSound.Value.SMA IsNot Nothing Then
                CurrentSound.Value.SMA.NominalLevel = TargetSoundLevel
                CurrentSound.Value.SMA.InferNominalLevelToAllDescendants()
            End If
        Next
    End Sub

    <Extension>
    Public Function GetImage(obj As SpeechMaterialComponent, ByVal Path As String) As String 'Drawing.Bitmap

        Return Path ' Drawing.Bitmap.FromFile(Path)

    End Function


    <Extension>
    Public Sub CreateVariable_HasVowelContrast(obj As SpeechMaterialComponent, ByVal SummaryLevel As SpeechMaterialComponent.LinguisticLevels,
                                                Optional ByVal PhoneticTranscriptionVariableName As String = "Transcription",
                                                Optional VariableName As String = "HasVowelContrast")

        If SummaryLevel = SpeechMaterialComponent.LinguisticLevels.Sentence Or SummaryLevel = SpeechMaterialComponent.LinguisticLevels.List Then
            'These are supported
        Else
            Throw New ArgumentException("Only 'Sentence' and 'List' are supported as values for SummaryLevel.")
        End If

        'Gets all summarty components into which the new variable should be stored
        Dim SummaryComponents = obj.GetToplevelAncestor.GetAllRelativesAtLevel(SummaryLevel)

        'Determine varoable value for each of the summary components
        For Each SummaryComponent In SummaryComponents

            'Gets the target components which to evaluate
            Dim TargetComponents = SummaryComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Phoneme)

            For c = 0 To TargetComponents.Count - 1

                'Determine if it is a (phonetically/phonemically) contrasting component
                If TargetComponents(c).IsContrastingComponent = False Then
                    'Skips to next if not
                    Continue For
                End If

                'Determines if the transciption of the contrasting component is a vowel, based on the IPA transcription standard  (otherwise is should logically be a consonant)
                Dim TranscriptionVariableValue = TargetComponents(c).GetCategoricalVariableValue(PhoneticTranscriptionVariableName)
                If TranscriptionVariableValue = "" Then
                    'Skips without checking if a transcription could not be retrieved
                    Continue For
                Else

                    'Determine if the transcription string contains an IPA vowel symbol. (Essentially this trims everything else, such as length markings and other vowel modifiers)
                    Dim ContainsVowel As Boolean = False
                    For Each ch In TranscriptionVariableValue.ToCharArray
                        If IPA.Vowels.Contains(ch) Then
                            ContainsVowel = True
                            Exit For
                        End If
                    Next

                    'Stortes the value
                    If ContainsVowel = True Then
                        SummaryComponent.SetNumericVariableValue(VariableName, 1)
                    Else
                        SummaryComponent.SetNumericVariableValue(VariableName, 0)
                    End If

                End If

                'Skips directly to the next SummaryComponent (the remaining TargetComponents should have the same value for Consonant / Vowel)
                Exit For

            Next
        Next

        ''Finally writes the results to file
        ''Ask if overwrite or save to new location
        'Dim res = MsgBox("Do you want to overwrite any existing files? Select NO to save the new files to a new location?", MsgBoxStyle.YesNo, "Overwrite existing files?")
        'If res = MsgBoxResult.Yes Then

        '    'Saving updated files
        '    Me.GetToplevelAncestor.WriteSpeechMaterialToFile(Me.ParentTestSpecification, Me.ParentTestSpecification.GetTestRootPath)
        '    MsgBox("Your speech material file and corresponding custom variable files should now have been saved to " & Me.ParentTestSpecification.GetSpeechMaterialFolder & vbCrLf & "Click OK to continue.",
        '           MsgBoxStyle.Information, "Files saved")

        'Else

        '    'Saving updated files
        '    Me.GetToplevelAncestor.WriteSpeechMaterialToFile(Me.ParentTestSpecification)
        '    MsgBox("Your speech material file and corresponding custom variable files should now have been saved to the selected folder. Click OK to continue.", MsgBoxStyle.Information, "Files saved")

        'End If

    End Sub


    <Extension>
    Public Sub CreateVariable_ContrastedPhonemeIndex(obj As SpeechMaterialComponent, ByVal SummaryLevel As SpeechMaterialComponent.LinguisticLevels,
                                                Optional ByVal PhoneticTranscriptionVariableName As String = "Transcription",
                                                Optional VariableName As String = "ContrastedPhonemeIndex")

        If SummaryLevel = SpeechMaterialComponent.LinguisticLevels.Sentence Or SummaryLevel = SpeechMaterialComponent.LinguisticLevels.List Then
            'These are supported
        Else
            Throw New ArgumentException("Only 'Sentence' and 'List' are supported as values for SummaryLevel.")
        End If

        'Gets all summarty components into which the new variable should be stored
        Dim SummaryComponents = obj.GetToplevelAncestor.GetAllRelativesAtLevel(SummaryLevel)

        'Determine varoable value for each of the summary components
        For Each SummaryComponent In SummaryComponents

            'Gets the target components which to evaluate
            Dim TargetComponents = SummaryComponent.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Phoneme)

            For c = 0 To TargetComponents.Count - 1

                'Determine if it is a (phonetically/phonemically) contrasting component
                If TargetComponents(c).IsContrastingComponent = False Then
                    'Skips to next if not
                    Continue For
                End If

                'Gets and stores the self index of the first contrasting phoneme (the rest should have the same self index value), and then continues directly to the next SummaryComponent
                Dim ContrastingPhonemeIndex As Integer = TargetComponents(c).GetSelfIndex
                SummaryComponent.SetNumericVariableValue(VariableName, ContrastingPhonemeIndex)

                Exit For

            Next
        Next

        ''Finally writes the results to file
        ''Ask if overwrite or save to new location
        'Dim res = MsgBox("Do you want to overwrite any existing files? Select NO to save the new files to a new location?", MsgBoxStyle.YesNo, "Overwrite existing files?")
        'If res = MsgBoxResult.Yes Then

        '    'Saving updated files
        '    Me.GetToplevelAncestor.WriteSpeechMaterialToFile(Me.ParentTestSpecification, Me.ParentTestSpecification.GetTestRootPath)
        '    MsgBox("Your speech material file and corresponding custom variable files should now have been saved to " & Me.ParentTestSpecification.GetSpeechMaterialFolder & vbCrLf & "Click OK to continue.",
        '           MsgBoxStyle.Information, "Files saved")

        'Else

        '    'Saving updated files
        '    Me.GetToplevelAncestor.WriteSpeechMaterialToFile(Me.ParentTestSpecification)
        '    MsgBox("Your speech material file and corresponding custom variable files should now have been saved to the selected folder. Click OK to continue.", MsgBoxStyle.Information, "Files saved")

        'End If

    End Sub



    <Obsolete>
    <Extension>
    Public Function OrderedChildren(obj As SpeechMaterialComponent) As Boolean

        'This function is to be removed!

        If obj.ChildComponents Is Nothing Then
            Return False
        Else
            Return obj.ChildComponents(0).IsSequentiallyOrdered
        End If
    End Function

    <Extension>
    Public Function GetSamePlaceCousins(obj As SpeechMaterialComponent) As List(Of SpeechMaterialComponent)

        Dim SelfIndex = obj.GetSelfIndex()

        Dim MySiblingCount As Integer = obj.GetSiblings.Count

        If SelfIndex Is Nothing Then Return Nothing

        If obj.ParentComponent Is Nothing Then Return Nothing

        'Checks that the parent has ordered children
        If obj.ParentComponent.OrderedChildren = False Then Throw New Exception("Cannot Return same-place cousins from unordered components. (Component id: " & obj.ParentComponent.Id & ")")

        Dim Aunties = obj.ParentComponent.GetSiblingsExcludingSelf

        If Aunties Is Nothing Then Return Nothing

        Dim OutputList As New List(Of SpeechMaterialComponent)

        For Each auntie In Aunties

            'Checks that the aunties have ordered children
            If auntie.OrderedChildren = False Then Throw New Exception("Cannot return same-place cousins from unordered components. (Component id: " & auntie.Id & ")")

            'Checks that the number of sibling copmonents are the same
            Dim AuntiesChildCount As Integer = auntie.ChildComponents.Count
            If MySiblingCount <> AuntiesChildCount Then Throw New Exception("Cannot return same-place cousins from cousin groups that differ in count. (Component ids: " & obj.ParentComponent.Id & " vs. " & auntie.Id & ")")

            'Finally adding the same place cousin
            OutputList.Add(auntie.ChildComponents(SelfIndex))

        Next

        Return OutputList

    End Function

    ''' <summary>
    ''' Returns the second cousin components that are stored at the same hierachical index orders.
    ''' </summary>
    ''' <returns></returns>
    <Extension>
    Public Function GetSamePlaceSecondCousins(obj As SpeechMaterialComponent) As List(Of SpeechMaterialComponent)

        Dim SelfIndex = obj.GetSelfIndex()

        If SelfIndex Is Nothing Then Return Nothing

        Dim SiblingCount As Integer = obj.ParentComponent.GetSiblings.Count

        If obj.ParentComponent Is Nothing Then Return Nothing

        Dim ParentSelfIndex = obj.ParentComponent.GetSelfIndex()

        Dim ParentSiblingCount As Integer = obj.ParentComponent.GetSiblings.Count

        If ParentSelfIndex Is Nothing Then Return Nothing

        If obj.ParentComponent.ParentComponent Is Nothing Then Return Nothing

        'Checks that the grand-parent has ordered children
        If obj.ParentComponent.ParentComponent.OrderedChildren = False Then Throw New Exception("Cannot return same-place second cousins from unordered components. (Component id: " & obj.ParentComponent.ParentComponent.Id & ")")

        Dim ParentAunties = obj.ParentComponent.ParentComponent.GetSiblingsExcludingSelf

        If ParentAunties Is Nothing Then Return Nothing

        Dim OutputList As New List(Of SpeechMaterialComponent)

        For Each parentAuntie In ParentAunties

            'Checks that the parent auntie have ordered children
            If parentAuntie.OrderedChildren = False Then Throw New Exception("Cannot return same-place cousins from unordered components. (Component id: " & parentAuntie.Id & ")")

            'Checks that the number of sibling components are the same
            Dim ParentAuntieChildCount As Integer = parentAuntie.ChildComponents.Count
            If ParentSiblingCount <> ParentAuntieChildCount Then Throw New Exception("Cannot return same-place cousins from cousin groups that differ in count. (Component ids: " & obj.ParentComponent.Id & " vs. " & parentAuntie.Id & ")")

            'Getting the same place second cousins
            Dim SamePlaceParentCousin = parentAuntie.ChildComponents(ParentSelfIndex)

            'Checks that the SamePlaceParentCousin have ordered children
            If SamePlaceParentCousin.OrderedChildren = False Then Throw New Exception("Cannot return same-place cousins from unordered components. (Component id: " & SamePlaceParentCousin.Id & ")")

            'Checks that the number of sibling components are the same
            Dim SamePlaceParentCousinChildCount As Integer = SamePlaceParentCousin.ChildComponents.Count
            If SiblingCount <> SamePlaceParentCousinChildCount Then Throw New Exception("Cannot return same-place second cousins from cousin groups that differ in count. (Component ids: " & obj.ParentComponent.Id & " vs. " & SamePlaceParentCousin.Id & ")")

            OutputList.Add(SamePlaceParentCousin.ChildComponents(SelfIndex))

        Next

        Return OutputList

    End Function


    ''' <summary>
    ''' Converts the speech material component to a new SpeechMaterialAnnotation object prepared for manual segmentation.
    ''' </summary>
    ''' <returns></returns>
    <Extension>
    Public Function ConvertToSMA(obj As SpeechMaterialComponent) As STFN.Core.Audio.Sound.SpeechMaterialAnnotation

        If obj.LinguisticLevel = LinguisticLevels.ListCollection Then
            MsgBox("Cannot convert a component at the ListCollection linguistic level to a SMA object. The highest level which can be stored in a SMA object is LinguisticLevels.List." & vbCrLf & "Aborting conversion!")
            Return Nothing
        End If

        Dim NewSMA = New STFN.Core.Audio.Sound.SpeechMaterialAnnotation With {.SegmentationCompleted = False}

        'Creating a (mono) channel level SmaComponent
        NewSMA.AddChannelData(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.CHANNEL, Nothing))

        'Adjusting to the right level
        If obj.LinguisticLevel = LinguisticLevels.Phoneme Then

            'We need to add all levels: Sentence, Word, Phone
            NewSMA.ChannelData(1).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE, NewSMA.ChannelData(1)))
            NewSMA.ChannelData(1)(0).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD, NewSMA.ChannelData(1)(0)))
            NewSMA.ChannelData(1)(0)(0).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, NewSMA.ChannelData(1)(0)(0)))

            'Calling AddSmaValues, which recursively adds all lower level components
            obj.AddSmaValues(NewSMA.ChannelData(1)(0)(0)(0))

        ElseIf obj.LinguisticLevel = LinguisticLevels.Word Then

            'We need to add all levels: Sentence, Word
            NewSMA.ChannelData(1).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE, NewSMA.ChannelData(1)))
            NewSMA.ChannelData(1)(0).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD, NewSMA.ChannelData(1)(0)))

            'Calling AddSmaValues, which recursively adds all lower level components
            obj.AddSmaValues(NewSMA.ChannelData(1)(0)(0))

        ElseIf obj.LinguisticLevel = LinguisticLevels.Sentence Then

            'We need to add all levels: Sentence
            NewSMA.ChannelData(1).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE, NewSMA.ChannelData(1)))
            obj.AddSmaValues(NewSMA.ChannelData(1)(0))

        ElseIf obj.LinguisticLevel = LinguisticLevels.List Then

            'No need to add any levels. Calling AddSmaValues, which recursively adds all lower level components
            obj.AddSmaValues(NewSMA.ChannelData(1))

        Else

            MsgBox("Unknown value for the LinguisticLevels Enum" & vbCrLf & "Aborting conversion!")
            Return Nothing

        End If

        Return NewSMA

    End Function


    <Extension>
    Private Sub AddSmaValues(obj As SpeechMaterialComponent, ByRef SmaComponent As STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent)

        'Attemption to get the spelling and transcription from the custom variables
        Dim MySpelling As String = ""
        Dim SpellingCandidateVariableNames As New List(Of String) From {"Spelling", "OrthographicForm"}
        For Each vn In SpellingCandidateVariableNames
            Dim SpellingCandidate = obj.GetCategoricalVariableValue(vn)
            If SpellingCandidate.Trim <> "" Then
                MySpelling = SpellingCandidate
                Exit For
            End If
        Next

        Dim MyTranscription As String = ""
        Dim TranscriptionCandidateVariableNames As New List(Of String) From {"Transcription", "PhonemicForm", "PhoneticForm", "PhoneticTranscription", "PhonemicTranscription"}
        For Each vn In TranscriptionCandidateVariableNames
            Dim TranscriptionCandidate = obj.GetCategoricalVariableValue(vn)
            If TranscriptionCandidate.Trim <> "" Then
                MyTranscription = TranscriptionCandidate
                Exit For
            End If
        Next

        'Using the PrimaryStringRepresentation instead of the spelling it was not found
        If MySpelling = "" Then MySpelling = obj.PrimaryStringRepresentation

        'Using MySpelling in place of the transcription of it was not found
        If MyTranscription = "" Then MyTranscription = MySpelling

        SmaComponent.OrthographicForm = MySpelling
        SmaComponent.PhoneticForm = MyTranscription

        For Each child In obj.ChildComponents

            Dim NewChildComponent = New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(SmaComponent.ParentSMA, SmaComponent.SmaTag + 1, SmaComponent)
            SmaComponent.Add(NewChildComponent)
            child.AddSmaValues(NewChildComponent)

        Next

    End Sub


    'Private Sub WriteSpeechMaterialComponenFile(ByVal OutputSpeechMaterialFolder As String, ByVal ExportAtThisLevel As Boolean, Optional ByRef CustomVariablesExportList As SortedList(Of String, List(Of String)) = Nothing,
    '                                         Optional ByRef NumericCustomVariableNames As SortedList(Of SpeechMaterialComponent.LinguisticLevels, SortedSet(Of String)) = Nothing,
    '                                            Optional ByRef CategoricalCustomVariableNames As SortedList(Of SpeechMaterialComponent.LinguisticLevels, SortedSet(Of String)) = Nothing)

    '    If CustomVariablesExportList Is Nothing Then CustomVariablesExportList = New SortedList(Of String, List(Of String))
    '    If NumericCustomVariableNames Is Nothing Then NumericCustomVariableNames = Me.GetToplevelAncestor.GetNumericCustomVariableNamesByLinguicticLevel()
    '    If CategoricalCustomVariableNames Is Nothing Then CategoricalCustomVariableNames = Me.GetToplevelAncestor.GetCategoricalCustomVariableNamesByLinguicticLevel()


    '    Dim OutputList As New List(Of String)
    '    If ExportAtThisLevel = True Then
    '        OutputList.Add("// Setup")

    '        'Writing Setup values
    '        OutputList.Add("SequentiallyOrderedLists = " & SequentiallyOrderedLists.ToString)
    '        OutputList.Add("SequentiallyOrderedSentences = " & SequentiallyOrderedSentences.ToString)
    '        OutputList.Add("SequentiallyOrderedWords = " & SequentiallyOrderedWords.ToString)
    '        OutputList.Add("SequentiallyOrderedPhonemes = " & SequentiallyOrderedPhonemes.ToString)
    '        OutputList.Add("PresetLevel = " & PresetLevel.ToString)
    '        For Each Item In PresetSpecifications
    '            OutputList.Add("Preset = " & Item.Item1.Trim & ": " & String.Join(", ", Item.Item2))
    '        Next

    '        'Writing components
    '        OutputList.Add("")
    '        OutputList.Add("// Components")
    '    End If

    '    Dim HeadingString As String = "// LinguisticLevel" & vbTab & "Id" & vbTab & "ParentId" & vbTab & "PrimaryStringRepresentation" & vbTab & "IsPractiseComponent"

    '    Dim Main_List As New List(Of String)

    '    'Linguistic Level
    '    Main_List.Add(LinguisticLevel.ToString)

    '    'Id
    '    Main_List.Add(Id)

    '    'ParentId 
    '    If ParentComponent IsNot Nothing Then
    '        Main_List.Add(ParentComponent.Id)
    '    Else
    '        Main_List.Add("")
    '    End If

    '    'PrimaryStringRepresentation
    '    Main_List.Add(PrimaryStringRepresentation)

    '    ''CustomVariablesDatabase 
    '    'If CustomVariablesDatabasePath <> "" Then
    '    '    Dim CurrentDataBasePath = IO.Path.GetFileName(CustomVariablesDatabasePath)
    '    '    Main_List.Add(CurrentDataBasePath)
    '    'Else
    '    '    Main_List.Add("")
    '    'End If

    '    'OrderedChildren 
    '    'Main_List.Add(OrderedChildren.ToString) 'Removed!

    '    'IsPractiseComponent
    '    Main_List.Add(IsPractiseComponent.ToString)

    '    'The media folders are removed and moved to the MediaSet class
    '    'MediaFolder 
    '    'Main_List.Add(GetMediaFolderName)
    '    'MaskerFolder 
    '    'Main_List.Add(MaskerFolder)
    '    'BackgroundNonspeechFolder 
    '    'Main_List.Add(BackgroundNonspeechFolder)
    '    'BackgroundSpeechFolder 
    '    'Main_List.Add(BackgroundSpeechFolder)

    '    If ExportAtThisLevel = True Then
    '        OutputList.Add(HeadingString)
    '    End If
    '    If Me.LinguisticLevel = LinguisticLevels.List Or Me.LinguisticLevel = LinguisticLevels.ListCollection Then
    '        OutputList.Add("") 'Adding an empty line between list or list collection level components
    '    End If

    '    OutputList.Add(String.Join(vbTab, Main_List))


    '    'Writing to file
    '    Logging.SendInfoToLog(String.Join(vbCrLf, OutputList), IO.Path.GetFileNameWithoutExtension(SpeechMaterialComponentFileName), OutputSpeechMaterialFolder, True, True, ExportAtThisLevel)

    '    'Custom variables
    '    Dim CustomVariablesDatabasePath As String = SpeechMaterialComponent.GetDatabaseFileName(LinguisticLevel)

    '    If CustomVariablesDatabasePath <> "" Then

    '        'Getting the right collection into which to store custom variable values
    '        Dim CurrentCustomVariablesOutputList As New List(Of String)
    '        If CustomVariablesExportList.ContainsKey(CustomVariablesDatabasePath) = False Then
    '            CustomVariablesExportList.Add(CustomVariablesDatabasePath, CurrentCustomVariablesOutputList)
    '        Else
    '            CurrentCustomVariablesOutputList = CustomVariablesExportList(CustomVariablesDatabasePath)
    '        End If

    '        'Getting the variable names and types
    '        Dim CategoricalVariableNames = CategoricalCustomVariableNames(Me.LinguisticLevel).ToList
    '        Dim NumericVariableNames = NumericCustomVariableNames(Me.LinguisticLevel).ToList

    '        'Writing headings only on the first line
    '        If CurrentCustomVariablesOutputList.Count = 0 Then
    '            Dim CustomVariableNamesList As New List(Of String)
    '            CustomVariableNamesList.AddRange(CategoricalVariableNames)
    '            CustomVariableNamesList.AddRange(NumericVariableNames)
    '            Dim CustomVariableNames = String.Join(vbTab, CustomVariableNamesList)

    '            'Writing types only if there are any headings
    '            If CustomVariableNames.Trim <> "" Then
    '                CurrentCustomVariablesOutputList.Add(CustomVariableNames)

    '                Dim CategoricalVariableTypes = DSP.Repeat("C", CategoricalVariableNames.Count).ToList
    '                Dim NumericVariableTypes = DSP.Repeat("N", NumericVariableNames.Count).ToList
    '                Dim CustomVariableTypesList As New List(Of String)
    '                CustomVariableTypesList.AddRange(CategoricalVariableTypes)
    '                CustomVariableTypesList.AddRange(NumericVariableTypes)
    '                Dim VariableTypes = String.Join(vbTab, CustomVariableTypesList)
    '                If VariableTypes.Trim <> "" Then CurrentCustomVariablesOutputList.Add(VariableTypes)

    '            End If
    '        End If

    '        'Looking up values
    '        'First categorical values
    '        Dim CustomVariableValues As New List(Of String)
    '        For Each VarName In CategoricalVariableNames
    '            Dim CurrentValue = Me.GetCategoricalVariableValue(VarName)
    '            CustomVariableValues.Add(CurrentValue)
    '        Next
    '        'Then numeric values
    '        For Each VarName In NumericCustomVariableNames(Me.LinguisticLevel)
    '            Dim CurrentValue = Me.GetNumericVariableValue(VarName)
    '            If CurrentValue IsNot Nothing Then
    '                CustomVariableValues.Add(CurrentValue)
    '            Else
    '                'Adds an empty string for missing value
    '                CustomVariableValues.Add("")
    '            End If
    '        Next
    '        'Storing the value string
    '        Dim CustomVariableValuesString = String.Join(vbTab, CustomVariableValues)
    '        If CustomVariableValuesString.Trim <> "" Then CurrentCustomVariablesOutputList.Add(CustomVariableValuesString)

    '    End If

    '    'Cascading to all child components
    '    For Each ChildComponent In Me.ChildComponents
    '        ChildComponent.WriteSpeechMaterialComponenFile(OutputSpeechMaterialFolder, False, CustomVariablesExportList)
    '    Next

    '    If ExportAtThisLevel = True Then
    '        'Exporting custom variables
    '        For Each item In CustomVariablesExportList
    '            Logging.SendInfoToLog(String.Join(vbCrLf, item.Value), IO.Path.GetFileNameWithoutExtension(item.Key), OutputSpeechMaterialFolder, True, True, ExportAtThisLevel)
    '        Next
    '    End If

    'End Sub


    'Public Sub SummariseNumericVariables(ByVal SourceLevels As SpeechMaterialComponent.LinguisticLevels, ByVal CustomVariableName As String, ByRef MetricType As NumericSummaryMetricTypes)

    '    If Me.LinguisticLevel < SourceLevels Then

    '        Dim Descendants = GetAllDescenentsAtLevel(SourceLevels)

    '        Dim VariableNameSourceLevelPrefix = SourceLevels.ToString & "_Level_"

    '        Dim ValueList As New List(Of Double)
    '        For Each d In Descendants
    '            ValueList.Add(d.GetNumericVariableValue(CustomVariableName))
    '        Next

    '        Select Case MetricType
    '            Case NumericSummaryMetricTypes.ArithmeticMean

    '                'Storing the result
    '                Dim SummaryResult As Double = ValueList.Average
    '                Me.SetNumericVariableValue(VariableNameSourceLevelPrefix & "Mean_" & CustomVariableName, SummaryResult)

    '            Case NumericSummaryMetricTypes.StandardDeviation

    '                'Storing the result
    '                Dim SummaryResult As Double = MathNet.Numerics.Statistics.Statistics.StandardDeviation(ValueList)
    '                Me.SetNumericVariableValue(VariableNameSourceLevelPrefix & "SD_" & CustomVariableName, SummaryResult)

    '            Case NumericSummaryMetricTypes.Maximum

    '                'Storing the result
    '                Dim SummaryResult As Double = ValueList.Max
    '                Me.SetNumericVariableValue(VariableNameSourceLevelPrefix & "Max_" & CustomVariableName, SummaryResult)

    '            Case NumericSummaryMetricTypes.Minimum

    '                'Storing the result
    '                Dim SummaryResult As Double = ValueList.Min
    '                Me.SetNumericVariableValue(VariableNameSourceLevelPrefix & "Min_" & CustomVariableName, SummaryResult)

    '            Case NumericSummaryMetricTypes.Median

    '                Dim SummaryResult As Double = MathNet.Numerics.Statistics.Statistics.Median(ValueList)
    '                Me.SetNumericVariableValue(VariableNameSourceLevelPrefix & "MD_" & CustomVariableName, SummaryResult)

    '            Case NumericSummaryMetricTypes.InterquartileRange

    '                Dim SummaryResult As Double = MathNet.Numerics.Statistics.Statistics.InterquartileRange(ValueList)
    '                Me.SetNumericVariableValue(VariableNameSourceLevelPrefix & "IQR_" & CustomVariableName, SummaryResult)

    '            Case NumericSummaryMetricTypes.CoefficientOfVariation

    '                'Storing the result
    '                Dim SummaryResult As Double = DSP.CoefficientOfVariation(ValueList)
    '                Me.SetNumericVariableValue(VariableNameSourceLevelPrefix & "CV_" & CustomVariableName, SummaryResult)

    '        End Select


    '        'Cascading calculations to lower levels
    '        'Calling this from within the conditional statment, as no descendants should exist at more than one
    '        'level, and if Me.LinguisticLevel >= SourceLevels no other descendants should be either, and thus the recursive calls can stop here.
    '        For Each Child In ChildComponents
    '            Child.SummariseNumericVariables(SourceLevels, CustomVariableName, MetricType)
    '        Next

    '    End If

    'End Sub

    'Public Enum NumericSummaryMetricTypes
    '    ArithmeticMean
    '    StandardDeviation
    '    Maximum
    '    Minimum
    '    Median
    '    InterquartileRange
    '    CoefficientOfVariation
    'End Enum


    'Public Sub SummariseCategoricalVariables(ByVal SourceLevels As SpeechMaterialComponent.LinguisticLevels, ByVal CustomVariableName As String, ByRef MetricType As CategoricalSummaryMetricTypes)

    '    If Me.LinguisticLevel < SourceLevels Then

    '        Dim Descendants = GetAllDescenentsAtLevel(SourceLevels)

    '        Dim VariableNameSourceLevelPrefix = SourceLevels.ToString & "_Level_"

    '        Dim ValueList As New SortedList(Of String, Integer)
    '        For Each d In Descendants
    '            Dim VariableValue As String = d.GetCategoricalVariableValue(CustomVariableName)

    '            'Adding missing variable values
    '            If ValueList.ContainsKey(VariableValue) = False Then ValueList.Add(d.GetCategoricalVariableValue(CustomVariableName), 0)

    '            'Counting the occurence of the specific variable value
    '            ValueList(d.GetCategoricalVariableValue(CustomVariableName)) += 1
    '        Next

    '        Select Case MetricType
    '            Case CategoricalSummaryMetricTypes.Mode

    '                If ValueList.Count > 0 Then

    '                    'Getting the most common value (TODO: the following lines are probably rather inefficient way and could possibly need opimization with larger datasets...)
    '                    Dim MaxOccurences = ValueList.Values.Max
    '                    Dim ModeList As New List(Of String)
    '                    For Each CurrentValue In ValueList
    '                        If CurrentValue.Value = MaxOccurences Then ModeList.Add(CurrentValue.Key)
    '                    Next

    '                    'If there are more than one mode value (i.e. equal number of occurences) they are returned as comma separated strings
    '                    Dim ModeValuesString As String = String.Join(",", ModeList)

    '                    'Storing the result
    '                    Me.SetCategoricalVariableValue(VariableNameSourceLevelPrefix & "Mode_" & CustomVariableName, ModeValuesString)
    '                Else

    '                    'Storing the an empty string as result, as there was no item in ValueList
    '                    Me.SetCategoricalVariableValue(VariableNameSourceLevelPrefix & "Mode_" & CustomVariableName, "")
    '                End If

    '            Case CategoricalSummaryMetricTypes.Distribution

    '                If ValueList.Count > 0 Then

    '                    'Getting the most common value
    '                    Dim DistributionList As New List(Of String)
    '                    For Each CurrentValue In ValueList
    '                        DistributionList.Add(CurrentValue.Key & "," & CurrentValue.Value)
    '                    Next

    '                    'The distribution are returned as vertical bar (|) separatered key value pairs of value and number of occurences
    '                    '(this rather akward format choice is selected in order to be able to use tab delimited files. In this way, the whole
    '                    'distribution may be put in a single cell, for instance in Excel (given that the maximum number of cell characters in not reached...)) 
    '                    Dim DistributionString As String = String.Join("|", DistributionList)

    '                    'TODO: This should really be sorted in freuency!

    '                    'Storing the result
    '                    Me.SetCategoricalVariableValue(VariableNameSourceLevelPrefix & "Distribution_" & CustomVariableName, DistributionString)
    '                Else

    '                    'Storing the an empty string as result, as there was no item in ValueList
    '                    Me.SetCategoricalVariableValue(VariableNameSourceLevelPrefix & "Distribution_" & CustomVariableName, "")
    '                End If

    '        End Select

    '        'Cascading calculations to lower levels
    '        'Calling this from within the conditional statment, as no descendants should exist at more than one
    '        'level, and if Me.LinguisticLevel >= SourceLevels no other descendants should be either, and thus the recursive calls can stop here.
    '        For Each Child In ChildComponents
    '            Child.SummariseCategoricalVariables(SourceLevels, CustomVariableName, MetricType)
    '        Next

    '    End If

    'End Sub

    'Public Enum CategoricalSummaryMetricTypes
    '    Mode
    '    Distribution
    'End Enum


    Public Enum LookupMathOptions
        MatchBySpelling
        MatchByTranscription
        MatchBySpellingAndTranscription
    End Enum



    <Extension>
    Public Function LoadTabDelimitedFile(obj As CustomVariablesDatabase, ByVal FilePath As String, ByVal MatchBy As LookupMathOptions,
                                         Optional ByVal SpellingVariableName As String = "", Optional ByVal TranscriptionVariableName As String = "",
                                         Optional ByVal IncludeItems As SortedList(Of String, String) = Nothing,
                                         Optional ByVal CaseInvariantSpellings As Boolean = False) As Boolean


        ''Gets a file path from the user if none is supplied
        'If FilePath = "" Then FilePath = Utils.GetOpenFilePath(,, {".txt"}, "Please open a tab delimited word metrics .txt file.")
        'If FilePath = "" Then
        '    MsgBox("No file selected!")
        '    Return Nothing
        'End If


        'Checking arguments
        Select Case MatchBy
            Case LookupMathOptions.MatchBySpellingAndTranscription
                If SpellingVariableName = "" Then
                    MsgBox("Missing spelling variable name", MsgBoxStyle.Information, "Loading custom variables file")
                    Return False
                End If

                If TranscriptionVariableName = "" Then
                    MsgBox("Missing transcription variable name", MsgBoxStyle.Information, "Loading custom variables file")
                    Return False
                End If

            Case LookupMathOptions.MatchBySpelling
                If SpellingVariableName = "" Then
                    MsgBox("Missing spelling variable name", MsgBoxStyle.Information, "Loading custom variables file")
                    Return False
                End If

            Case LookupMathOptions.MatchByTranscription
                If TranscriptionVariableName = "" Then
                    MsgBox("Missing transcription variable name", MsgBoxStyle.Information, "Loading custom variables file")
                    Return False
                End If

        End Select

        Try

            obj.CustomVariablesData.Clear()
            obj.CustomVariableNames.Clear()
            obj.CustomVariableTypes.Clear()

            'Parses the input file
            Dim InputLines() As String = System.IO.File.ReadAllLines(FilePath, Text.Encoding.UTF8)

            'Stores the file path used for loading the word metric data
            obj.FilePath = FilePath

            'Assuming that there is no data, if the first line is empty!
            If InputLines(0).Trim = "" Then
                Return False
            End If

            'First line should be variable names
            Dim FirstLineData() As String = InputLines(0).Trim(vbTab).Split(vbTab)
            For c = 0 To FirstLineData.Length - 1
                obj.CustomVariableNames.Add(FirstLineData(c).Trim)
            Next

            'Looing for the column indices where the unique identifiers (spealling and/or transcription) are
            Dim SpellingColumnIndex As Integer = -1
            Dim TranscriptionColumnIndex As Integer = -1
            For c = 0 To FirstLineData.Length - 1
                If FirstLineData(c).Trim() = "" Then Continue For
                If FirstLineData(c).Trim = SpellingVariableName Then SpellingColumnIndex = c
                If FirstLineData(c).Trim = TranscriptionVariableName Then TranscriptionColumnIndex = c
            Next

            'Checking that the needed columns were found
            Select Case MatchBy
                Case LookupMathOptions.MatchBySpellingAndTranscription
                    If SpellingColumnIndex = -1 Then
                        MsgBox("No variable named " & SpellingVariableName & " could be found in the database file.", MsgBoxStyle.Information, "Missing variable")
                        Return False
                    End If
                    If TranscriptionColumnIndex = -1 Then
                        MsgBox("No variable named " & TranscriptionVariableName & " could be found in the database file.", MsgBoxStyle.Information, "Missing variable")
                        Return False
                    End If

                Case LookupMathOptions.MatchBySpelling
                    If SpellingColumnIndex = -1 Then
                        MsgBox("No variable named " & SpellingVariableName & " could be found in the database file.", MsgBoxStyle.Information, "Missing variable")
                        Return False
                    End If

                Case LookupMathOptions.MatchByTranscription
                    If TranscriptionColumnIndex = -1 Then
                        MsgBox("No variable named " & TranscriptionVariableName & " could be found in the database file.", MsgBoxStyle.Information, "Missing variable")
                        Return False
                    End If
            End Select


            'Second line should be variable types (N for Numeric or C for Categorical)
            Dim SecondLineData() As String = InputLines(1).Trim(vbTab).Split(vbTab)
            For c = 0 To SecondLineData.Length - 1
                If SecondLineData(c).Trim.ToLower = "n" Then
                    obj.CustomVariableTypes.Add(VariableTypes.Numeric)
                ElseIf SecondLineData(c).Trim.ToLower = "c" Then
                    obj.CustomVariableTypes.Add(VariableTypes.Categorical)
                ElseIf SecondLineData(c).Trim.ToLower = "b" Then
                    obj.CustomVariableTypes.Add(VariableTypes.Boolean)
                Else
                    Throw New Exception("The type for the custom variable " & obj.CustomVariableNames(c) & " in the file " & FilePath & " must be either N for numeric or C for categorical, or B for Boolean.")
                End If
            Next

            'Reading data
            For i = 2 To InputLines.Length - 1

                Dim LineSplit() As String = InputLines(i).Split(vbTab)

                'Gets the unique identifier
                Dim UniqueIdentifier As String = ""
                Select Case MatchBy
                    Case LookupMathOptions.MatchBySpellingAndTranscription
                        Dim Spelling As String = LineSplit(SpellingColumnIndex).Trim
                        If CaseInvariantSpellings = True Then
                            Spelling = Spelling.ToLower
                        End If
                        UniqueIdentifier = Spelling & vbTab & LineSplit(TranscriptionColumnIndex).Trim
                    Case LookupMathOptions.MatchBySpelling
                        Dim Spelling As String = LineSplit(SpellingColumnIndex).Trim
                        If CaseInvariantSpellings = True Then
                            Spelling = Spelling.ToLower
                        End If
                        UniqueIdentifier = Spelling
                    Case LookupMathOptions.MatchByTranscription
                        UniqueIdentifier = LineSplit(TranscriptionColumnIndex).Trim
                End Select

                'Skipping if IncludeItems has been set and the UniqueIdentifier is not in it (this is to save memory with very large databases!!!)
                If IncludeItems IsNot Nothing Then
                    If IncludeItems.Keys.Contains(UniqueIdentifier) = False Then Continue For
                End If

                'Getting the original identifier if altered before lookup
                Dim OriginalIdentifier As String = IncludeItems(UniqueIdentifier)

                If obj.CustomVariablesData.ContainsKey(UniqueIdentifier) Then
                    Select Case MatchBy
                        Case LookupMathOptions.MatchBySpellingAndTranscription
                            MsgBox("There exist more than one instance of the spelling-transcription combination " & UniqueIdentifier & " in the lexical database. Unable to select the one to use!", MsgBoxStyle.Information, "Duplicate look-up keys")
                        Case LookupMathOptions.MatchBySpelling
                            MsgBox("There exist more than one instance of the spellings " & UniqueIdentifier & " in the lexical database. Unable to select the one to use!", MsgBoxStyle.Information, "Duplicate look-up keys")
                        Case LookupMathOptions.MatchByTranscription
                            MsgBox("There exist more than one instance of the transcription " & UniqueIdentifier & " in the lexical database. Unable to select the one to use!", MsgBoxStyle.Information, "Duplicate look-up keys")
                    End Select
                    Return False
                End If

                'Adding the unique identifier
                obj.CustomVariablesData.Add(OriginalIdentifier, New SortedList(Of String, Object))

                'Adding variables (getting only as many as there are variables, or tabs)
                For c = 0 To Math.Min(LineSplit.Length - 1, obj.CustomVariableNames.Count - 1)

                    Dim ValueString As String = LineSplit(c).Trim
                    If obj.CustomVariableTypes(c) = VariableTypes.Numeric Then

                        'Adding the data as a Double
                        Dim NumericValue As Double
                        If Double.TryParse(ValueString.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, NumericValue) Then
                            'Adds the variable and its data only if a value has been parsed
                            obj.CustomVariablesData(OriginalIdentifier).Add(obj.CustomVariableNames(c), NumericValue)
                        Else
                            'Throws an error if parsing failed even though the string was not empty
                            If ValueString.Trim <> "" Then
                                Throw New Exception("Unable to parse the string " & ValueString & " given for the variable " & obj.CustomVariableNames(c) & " in the file: " & FilePath & " as a numeric value.")
                            Else
                                'Stores a NaN to mark that the input data was missing / NaN
                                obj.CustomVariablesData(OriginalIdentifier).Add(obj.CustomVariableNames(c), Double.NaN)
                            End If
                        End If

                    ElseIf obj.CustomVariableTypes(c) = VariableTypes.Boolean Then

                        'Adding the data as a boolean
                        Dim BooleanValue As Boolean
                        If Boolean.TryParse(ValueString, BooleanValue) Then
                            'Adds the variable and its data only if a value has been parsed
                            obj.CustomVariablesData(OriginalIdentifier).Add(obj.CustomVariableNames(c), BooleanValue)
                        Else
                            'Throws an error if parsing failed even though the string was not empty
                            If ValueString.Trim <> "" Then
                                Throw New Exception("Unable to parse the string " & BooleanValue & " given for the variable " & obj.CustomVariableNames(c) & " in the file: " & FilePath & " as a boolean value (True or False).")
                            End If
                        End If

                    Else
                        'Adding the data as a String
                        obj.CustomVariablesData(OriginalIdentifier).Add(obj.CustomVariableNames(c), ValueString)
                    End If

                Next

            Next

            Return True

        Catch ex As Exception

            MsgBox("The following exception occurred while reading a custom variables file: " & ex.ToString)
            'TODO What here?
            Return False
        End Try


    End Function


    Public Function UniqueIdentifierIsPresent(obj As CustomVariablesDatabase, ByVal UniqueIdentifier As String) As Boolean
        If obj.CustomVariablesData.ContainsKey(UniqueIdentifier) Then
            Return True
        Else
            Return False
        End If
    End Function

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