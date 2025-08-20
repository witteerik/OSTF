Imports System.Runtime.CompilerServices
Imports STFN.Core
Imports STFN.Core.Audio.Sound.SpeechMaterialAnnotation

'This file contains extension methods for STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent

Partial Public Module Extensions


    ''' <summary>
    ''' Measures sound levels for each channel, sentence, word and phone of the current SMA object.
    ''' </summary>
    ''' <param name="c">The channel index (1-based)</param>
    ''' <param name="AttemptedMeasurementCount">The number of attempted measurements</param>
    ''' <param name="SuccesfullMeasurementsCount">The number of successful measurements</param>
    <Extension>
    Public Sub MeasureSoundLevels(obj As SmaComponent, ByVal IncludeCriticalBandLevels As Boolean, ByVal c As Integer, ByRef AttemptedMeasurementCount As Integer, ByRef SuccesfullMeasurementsCount As Integer,
                                              Optional BandInfo As DSP.BandBank = Nothing,
                                         Optional FftFormat As STFN.Core.Audio.Formats.FftFormat = Nothing)

        Dim ParentSound = obj.ParentSMA.ParentSound

        'Checks that parent sound has enough channels
        If ParentSound.WaveFormat.Channels >= c Then

            'Checks that parent the current channel of the parent sound contains sounds
            If ParentSound.WaveData.SampleData(c).Length > 0 Then

                'Measuring sound levels

                'Measuring UnWeightedLevel
                obj.UnWeightedLevel = Nothing
                obj.UnWeightedLevel = DSP.MeasureSectionLevel(ParentSound, c, obj.StartSample, obj.Length, DSP.SoundDataUnit.dB)
                AttemptedMeasurementCount += 1
                If obj.UnWeightedLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

                'Meaures UnWeightedPeakLevel
                obj.UnWeightedPeakLevel = Nothing
                obj.UnWeightedPeakLevel = DSP.MeasureSectionLevel(ParentSound, c, obj.StartSample, obj.Length, DSP.SoundDataUnit.dB, DSP.SoundMeasurementType.AbsolutePeakAmplitude)
                AttemptedMeasurementCount += 1
                If obj.UnWeightedPeakLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

                'Measures weighted level
                obj.WeightedLevel = Nothing
                If obj.GetTimeWeighting() <> 0 Then
                    obj.WeightedLevel = DSP.GetLevelOfLoudestWindow(ParentSound, c,
                                                                                     obj.GetTimeWeighting() * ParentSound.WaveFormat.SampleRate,
                                                                                      obj.StartSample, obj.Length, , obj.GetFrequencyWeighting, True)
                Else
                    obj.WeightedLevel = DSP.MeasureSectionLevel(ParentSound, c, obj.StartSample, obj.Length, DSP.SoundDataUnit.dB, DSP.SoundMeasurementType.RMS, obj.GetFrequencyWeighting)
                End If
                AttemptedMeasurementCount += 1
                If obj.WeightedLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

                'Measures critical band levels
                If IncludeCriticalBandLevels = True Then

                    'Gets the sound section
                    Dim SoundFileSection = obj.GetSoundFileSection(c)

                    'Calculating and storing the band levels
                    Dim TempBandLevelList = DSP.CalculateBandLevels(SoundFileSection, 1, BandInfo)
                    AttemptedMeasurementCount += 1
                    If TempBandLevelList IsNot Nothing Then
                        SuccesfullMeasurementsCount += 1
                        obj.BandLevels = TempBandLevelList.ToArray
                        obj.CentreFrequencies = BandInfo.GetCentreFrequencies
                        obj.BandWidths = BandInfo.GetBandWidths
                    Else
                        obj.BandLevels = {}
                        obj.CentreFrequencies = {}
                        obj.BandWidths = {}
                    End If

                End If

            Else
                'Notes a missing measurement
                AttemptedMeasurementCount += 1
            End If
        Else
            'Notes a missing measurement
            AttemptedMeasurementCount += 1
        End If

        'Cascading to lower levels
        For Each childComponent In obj
            childComponent.MeasureSoundLevels(IncludeCriticalBandLevels, c, AttemptedMeasurementCount, SuccesfullMeasurementsCount, BandInfo, FftFormat)
        Next

    End Sub


    ''' <summary>
    ''' Calculates the SII spectrum levels based on the band levels stored in BandLevels (N.B. requires precalculation of band levels)
    ''' </summary>
    ''' <returns></returns>
    <Extension>
    Public Function GetSpectrumLevels(obj As SmaComponent, Optional ByVal dBSPL_FSdifference As Double? = Nothing) As Double()

        'Setting default dBSPL_FSdifference 
        If dBSPL_FSdifference Is Nothing Then dBSPL_FSdifference = DSP.Standard_dBFS_dBSPL_Difference

        Dim SpectrumLevelList As New List(Of Double)
        If obj.BandLevels.Length = obj.BandWidths.Length Then
            'Calculating the levels only if the lengths of the level and centre frequency vectors agree
            For i = 0 To obj.BandLevels.Length - 1

                'Converting dB FS to dB SPL
                Dim BandLevel_SPL As Double = obj.BandLevels(i) + dBSPL_FSdifference

                'Calculating spectrum level according to equation 3 in ANSI S3.5-1997 (The SII-standard)
                'Dim SpectrumLevel As Double = BandLevel_SPL - 10 * Math.Log10(BandWidths(i) / 1)
                Dim SpectrumLevel As Double = DSP.BandLevel2SpectrumLevel(BandLevel_SPL, obj.BandWidths(i))
                SpectrumLevelList.Add(SpectrumLevel)
            Next
        End If

        Return SpectrumLevelList.ToArray

    End Function



    ''' <summary>
    ''' Measures the unalterred (original recording) absolute peak amplitude (linear value) within the word parts of a segmented audio recording. 
    ''' At least WordStartSample and WordLength of all words must be set prior to calling this function.
    ''' </summary>
    ''' <param name="MeasurementSound">The sound to measure.</param>
    ''' <param name="c">The channel index (1-based)</param>
    ''' <param name="AttemptedMeasurementCount">The number of attempted measurements</param>
    ''' <param name="SuccesfullMeasurementsCount">The number of successful measurements</param>
    <Extension>
    Public Sub SetInitialPeakAmplitudes(obj As SmaComponent, ByVal MeasurementSound As STFN.Core.Audio.Sound, ByVal c As Integer, ByRef AttemptedMeasurementCount As Integer, ByRef SuccesfullMeasurementsCount As Integer)

        'Checks that parent sound has enough channels
        If MeasurementSound.WaveFormat.Channels >= c Then

            'Checks that parent the current channel of the parent sound contains sounds
            If MeasurementSound.WaveData.SampleData(c).Length > 0 Then

                If obj.InitialPeak <> -1 Then
                    'Aborting measurements if the current channel InitialPeak is already set (-1 is the default/unmeasured value)
                    'Measures UnWeightedPeakLevel of each word using Z-weighting
                    Dim UnWeightedPeakLevel As Double?
                    UnWeightedPeakLevel = DSP.MeasureSectionLevel(MeasurementSound, c, obj.StartSample, obj.Length, DSP.SoundDataUnit.linear, DSP.SoundMeasurementType.AbsolutePeakAmplitude)

                    AttemptedMeasurementCount += 1
                    If UnWeightedPeakLevel IsNot Nothing Then SuccesfullMeasurementsCount += 1

                    'Setting InitialPeak to the highest detected so far
                    obj.InitialPeak = Math.Max(obj.InitialPeak, CDbl(UnWeightedPeakLevel))
                End If

            Else
                'Notes a missing measurement
                AttemptedMeasurementCount += 1
            End If
        Else
            'Notes a missing measurement
            AttemptedMeasurementCount += 1
        End If

        'Cascading to lower levels
        For Each childComponent In obj
            childComponent.SetInitialPeakAmplitudes(MeasurementSound, c, AttemptedMeasurementCount, SuccesfullMeasurementsCount)
        Next


    End Sub

    ''' <summary>
    ''' Cretates a identifier string that points to the sound section of the sound file containing the audio data
    ''' </summary>
    ''' <param name="SoundFilePathString">As the SmaComponent may not always know the wave file from which is was read, the calling code can insteda supply the path here.</param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateUniqueSoundSectionIdentifier(obj As SmaComponent, Optional ByVal SoundFilePathString As String = "") As String

        Dim OutputList As New List(Of String)

        If SoundFilePathString = "" Then
            If obj.SourceFilePath <> "" Then
                OutputList.Add(obj.SourceFilePath)
            Else
                'Attempts to find the path from the sound file
                If obj.ParentSMA Is Nothing Then
                    OutputList.Add("NoParent")
                Else
                    If obj.ParentSMA.ParentSound Is Nothing Then
                        OutputList.Add("NoParentSound")
                    Else
                        OutputList.Add(obj.ParentSMA.ParentSound.FileName)
                    End If
                End If
            End If
        Else
            'Using the string supplied by the calling code
            OutputList.Add(SoundFilePathString)
        End If

        Dim HierarchicalSelfIndexSerie As New List(Of String)
        obj.GetHierarchicalSelfIndexSerie(HierarchicalSelfIndexSerie)
        OutputList.Add(String.Concat(HierarchicalSelfIndexSerie))

        Return String.Join("_", OutputList)

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Gain"></param>
    <Extension>
    Public Sub ApplyGain(obj As SmaComponent, ByVal Gain As Double, Optional ByVal CurrentChannel As Integer = 1, Optional ByVal SupressWarnings As Boolean = False, Optional SoftEdgeGain As Boolean = True, Optional SoftEdgeSamples As Integer = 100)

        If obj.CheckSoundDataAvailability(CurrentChannel, SupressWarnings) = True Then

            If SoftEdgeGain = True Then

                'Calculates the amount of possible fade length, limiting it by SoftEdgeSamples and a tenth of the length of the SmaComponent
                Dim FadeLength As Integer = Math.Min(SoftEdgeSamples, Math.Floor(obj.Length / 10))
                If FadeLength = 0 Then
                    'Amplifies the whole section
                    DSP.AmplifySection(obj.ParentSMA.ParentSound, Gain, CurrentChannel, obj.StartSample, obj.Length)
                Else

                    'Fades in and out during fade length number of samples
                    DSP.Fade(obj.ParentSMA.ParentSound, 0, -Gain, CurrentChannel, obj.StartSample, FadeLength)
                    DSP.AmplifySection(obj.ParentSMA.ParentSound, Gain, CurrentChannel, obj.StartSample + FadeLength, obj.Length - 2 * FadeLength)
                    DSP.Fade(obj.ParentSMA.ParentSound, 0, -Gain, CurrentChannel, obj.StartSample + obj.Length - FadeLength, FadeLength)

                End If

            Else
                'Applies gain
                DSP.AmplifySection(obj.ParentSMA.ParentSound, Gain, CurrentChannel, obj.StartSample, obj.Length)
            End If

            obj.ParentSMA.ParentSound.SetIsChangedManually(True)

        Else
            MsgBox("Unable to apply gain to the SMA component " & obj.GetStringRepresentation())
        End If

    End Sub

    ''' <summary>
    ''' Compares the current peak amplitude with the InitialPeak of the word segments in the current channel of the audio recording 
    ''' and returns the gain that has been applied to the audio since the initial recording.
    ''' </summary>
    ''' <returns>Returns the gain applied since recoring, or Nothing if measurements failed.</returns>
    <Extension>
    Public Function GetCurrentGain(obj As SmaComponent, ByVal MeasurementSound As STFN.Core.Audio.Sound, ByVal MeasurementChannel As Integer) As Double?

        If obj.CheckStartAndLength() = False Then
            Return Nothing
        End If

        Dim soundLength As Integer = MeasurementSound.WaveData.ShortestChannelSampleCount
        If obj.StartSample + obj.Length > soundLength Then
            Return Nothing
        End If

        'Getting the current peak amplitude
        'Meaures UnWeightedPeakLevel
        Dim CurrentPeakAmplitude As Double? = DSP.MeasureSectionLevel(MeasurementSound, MeasurementChannel, obj.StartSample, obj.Length, DSP.SoundDataUnit.dB, DSP.SoundMeasurementType.AbsolutePeakAmplitude)

        'Returns nothing if measurement failed 
        If CurrentPeakAmplitude Is Nothing Then
            Return Nothing
        End If

        'Converting the initial peak amplitude to dB
        Dim InitialPeakLevel As Double = DSP.dBConversion(obj.InitialPeak, DSP.dBConversionDirection.to_dB, MeasurementSound.WaveFormat)

        'Getting the currently applied gain
        Dim Gain As Double = CurrentPeakAmplitude - InitialPeakLevel

        Return Gain

    End Function


    <Extension>
    Private Function CheckStartAndLength(obj As SmaComponent) As Boolean

        'Checks to see that start sample and length are assigned before sound measurements
        If (obj.StartSample < 0 Or obj.Length < 0) Then
            'Warnings("Cannot measure sound since one or both of StartSample or Length has not been assigned a value.")
            Return False
        Else
            Return True
        End If

    End Function

    ''' <summary>
    ''' Checks the SegmentationCompleted value of all child SmaComponents is True. Otherwise returns False.
    ''' </summary>
    ''' <returns></returns>
    <Extension>
    Public Function AllChildSegmentationsCompleted(obj As SmaComponent) As Boolean

        For Each c In obj
            If c.SegmentationCompleted = False Then Return False
        Next

        Return True

    End Function


    ''' <summary>
    ''' Checks the SegmentationCompleted value of all sibling SmaComponents is True. Otherwise returns False.
    ''' </summary>
    ''' <returns></returns>
    <Extension>
    Public Function AllSiblingSegmentationsCompleted(obj As SmaComponent) As Boolean

        Dim MySiblings = obj.GetSiblings()

        If MySiblings Is Nothing Then
            Return False
        Else
            For Each c In MySiblings
                If c.SegmentationCompleted = False Then Return False
            Next
        End If

        Return True

    End Function





    ''' <summary>
    ''' Infers the lengths of each sibling segment from the startposition of the next sibling (naturally, this does not set the length of the last sibling).
    ''' </summary>
    <Extension>
    Public Sub InferSiblingLengths(obj As SmaComponent)

        Dim Siblings = obj.GetSiblings()
        If Siblings IsNot Nothing Then
            For i = 0 To Siblings.Count - 2

                Dim PreviousLength As Integer = Siblings(i).Length

                'Locks the length of sibling i to the start position (-1) of the sibling i+1
                Siblings(i).Length = Siblings(i + 1).StartSample - Siblings(i).StartSample

                If Siblings(i).Length <> PreviousLength Then
                    'Invalidating segmentation if the length was changed
                    Siblings(i).SetSegmentationCompleted(False, True)
                End If

            Next
        End If

    End Sub

    <Extension>
    Public Sub GetHierarchicalSelfIndexSerie(obj As SmaComponent, ByRef HierarchicalSelfIndexSerie As List(Of String))

        If obj.ParentComponent Is Nothing Then
            Exit Sub
        Else

            'Calls GetHierarchicalSelfIndexSerie on the parent
            obj.ParentComponent.GetHierarchicalSelfIndexSerie(HierarchicalSelfIndexSerie)

        End If

        Dim LinguisticLevelString As String = ""
        Select Case obj.SmaTag
            Case SmaTags.CHANNEL
                LinguisticLevelString = "C"
            Case SmaTags.SENTENCE
                LinguisticLevelString = "S"
            Case SmaTags.WORD
                LinguisticLevelString = "W"
            Case SmaTags.PHONE
                LinguisticLevelString = "P"
        End Select

        HierarchicalSelfIndexSerie.Add(LinguisticLevelString & obj.GetSelfIndex)

    End Sub

    ''' <summary>
    ''' Figures out and returns at what index in the parent component the component itself is stored, or Nothing if there is no parent compoment, or if (for some unexpected reason) unable to establish the index.
    ''' </summary>
    ''' <returns></returns>
    <Extension>
    Public Function GetSelfIndex(obj As SmaComponent) As Integer?

        If obj.ParentComponent Is Nothing Then Return Nothing

        Dim Siblings = obj.GetSiblings()
        For s = 0 To Siblings.Count - 1
            If Siblings(s) Is obj Then Return s
        Next

        Return Nothing

    End Function

    ''' <summary>
    ''' Aligns the end of a segmentation set on the current level to the ends of dependent segmentation on all other levels, by adjusting their lengths, and if needed also their start positions, in case lengths gets reduced to zero.
    ''' </summary>
    ''' <param name="NewExclusiveEndSample"></param>
    ''' <param name="HierarchicalDirection"></param>
    <Extension>
    Private Sub AlignSegmentationEnds(obj As SmaComponent, ByVal NewExclusiveEndSample As Integer, ByVal HierarchicalDirection As HierarchicalDirections)

        ' Moving the end, unless the current level channel
        If obj.SmaTag <> SmaTags.CHANNEL Then

            Dim TempStartSample As Integer = Math.Max(0, obj.StartSample) ' Needed since default (onset) StartSample value is -1
            Dim MyExclusiveEndSample As Integer = TempStartSample + obj.Length

            Dim EndSampleChange As Integer = NewExclusiveEndSample - MyExclusiveEndSample
            'A positive value of EndSampleChange represent a forward shift, and vice versa

            'Changing the length
            MyExclusiveEndSample += EndSampleChange

            Dim NewLengthValue As Integer = (MyExclusiveEndSample - 1) - TempStartSample

            'Moves the start sample (and sets length to zero) if the new length would have to be reduced below zero.
            If NewLengthValue < 0 Then
                TempStartSample += NewLengthValue
                NewLengthValue = 0
            End If

            'Storing the new values
            obj.StartSample = TempStartSample
            obj.Length = NewLengthValue

        End If


        'Cascading up or down
        Select Case HierarchicalDirection
            Case HierarchicalDirections.Upwards

                Dim SelfIndex = obj.GetSelfIndex()
                If SelfIndex.HasValue Then
                    Dim SiblingCount = obj.GetSiblings.Count
                    If SelfIndex = SiblingCount - 1 Then
                        'The current instance is the last of its same level components
                        obj.ParentComponent.AlignSegmentationEnds(NewExclusiveEndSample, HierarchicalDirection)
                    End If
                End If

            Case HierarchicalDirections.Downwards

                If obj.Count > 0 Then
                    obj(obj.Count - 1).AlignSegmentationEnds(NewExclusiveEndSample, HierarchicalDirection)
                End If

        End Select


    End Sub

    Public Enum HierarchicalDirections
        Upwards
        Downwards
    End Enum

    ''' <summary>
    '''Aligns the start of a segmentation set on the current level to dependent segmentation starts on all other levels, without moving the end of each segmentation (unless it the length would have to be reduced below zero, which is not allowed)
    ''' </summary>
    ''' <param name="NewStartSample"></param>
    ''' <param name="SoundLength"></param>
    ''' <param name="HierarchicalDirection"></param>
    <Extension>
    Private Sub AlignSegmentationStarts(obj As SmaComponent, ByVal NewStartSample As Integer, ByVal SoundLength As Integer, ByVal HierarchicalDirection As HierarchicalDirections)

        ' Moving the start, unless the current level channel
        If obj.SmaTag <> SmaTags.CHANNEL Then
            obj.MoveStart(NewStartSample, SoundLength)
        End If

        'Cascading up or down
        Select Case HierarchicalDirection
            Case HierarchicalDirections.Upwards

                Dim SelfIndex = obj.GetSelfIndex()
                If SelfIndex.HasValue Then
                    If SelfIndex = 0 Then obj.ParentComponent.AlignSegmentationStarts(NewStartSample, SoundLength, HierarchicalDirection)
                End If

            Case HierarchicalDirections.Downwards

                If obj.Count > 0 Then
                    obj(0).AlignSegmentationStarts(NewStartSample, SoundLength, HierarchicalDirection)
                End If

        End Select

    End Sub

    <Extension>
    Public Sub AlignSegmentationStartsAcrossLevels(obj As SmaComponent, ByVal SoundLength As Integer)

        'Aligning the start of a segmentation set on the current level to dependent segmentation starts on all other levels, without moving the end of each segmentation (unless it the length would have to be reduced below zero, which is not allowed)
        'These functions may reduce the length of a segmentation to zero
        obj.AlignSegmentationStarts(obj.StartSample, SoundLength, HierarchicalDirections.Upwards)
        obj.AlignSegmentationStarts(obj.StartSample, SoundLength, HierarchicalDirections.Downwards)

    End Sub

    <Extension>
    Public Sub AlignSegmentationEndsAcrossLevels(obj As SmaComponent)

        'TODO: I'm not entirely clear as to if the ends or starts should be aligned first....????

        'Aligning the end of a segmentation set on the current level to the ends of dependent segmentation on all other levels, by adjusting their lengths, and if needed also their start positions, in case lengths gets reduced to zero.
        Dim ExclusiveEndSample As Integer = obj.StartSample + obj.Length
        'These functions may move the start of a segmentation to an earlier time
        obj.AlignSegmentationEnds(ExclusiveEndSample, HierarchicalDirections.Upwards)
        obj.AlignSegmentationEnds(ExclusiveEndSample, HierarchicalDirections.Downwards)

    End Sub

    ''' <summary>
    ''' Moves the start sample of a segmentation without changing the end sample (unless the length would be reduced below zero).
    ''' </summary>
    ''' <param name="NewStartSample"></param>
    <Extension>
    Public Sub MoveStart(obj As SmaComponent, ByVal NewStartSample As Integer, ByVal SoundLength As Integer)

        Dim TempStartSample As Integer = Math.Max(0, obj.StartSample) ' Needed since default (onset) StartSample value is -1
        Dim ExclusiveEndSample As Integer = TempStartSample + obj.Length

        Dim StartSampleChange As Integer = NewStartSample - TempStartSample
        'A positive value of StartSampleChange represent a forward shift, and vice versa

        'Changing the start value
        TempStartSample += StartSampleChange

        'Making sure the start value stays withing the sound file
        TempStartSample = Math.Min(SoundLength - 1, TempStartSample)
        TempStartSample = Math.Max(0, TempStartSample)

        'Storing the difference in start sampe
        Dim DifferenceSamples As Integer = TempStartSample - obj.StartSample

        'Storing the new StartSample
        obj.StartSample = TempStartSample

        'Adjusting the Length
        obj.Length = ExclusiveEndSample - obj.StartSample

        'Also adjusts StartTime
        If obj.ParentSMA.ParentSound IsNot Nothing Then
            Dim DurationDifference As Double = DifferenceSamples / obj.ParentSMA.ParentSound.WaveFormat.SampleRate
            obj.StartTime += DurationDifference
        End If

    End Sub

    <Extension>
    Public Sub GetUnbrokenLineOfFirstbornDescendents(obj As SmaComponent, ByRef ResultList As List(Of SmaComponent))

        If obj.Count > 0 Then
            ResultList.Add(obj(0))
            obj(0).GetUnbrokenLineOfFirstbornDescendents(ResultList)
        End If

    End Sub

    <Extension>
    Public Sub GetUnbrokenLineOfLastbornDescendents(obj As SmaComponent, ByRef ResultList As List(Of SmaComponent))

        If obj.Count > 0 Then
            ResultList.Add(obj(obj.Count - 1))
            obj(obj.Count - 1).GetUnbrokenLineOfLastbornDescendents(ResultList)
        End If

    End Sub

    <Extension>
    Public Function GetClosestAncestorComponent(obj As SmaComponent, ByVal RequestedParentComponentType As SmaTags) As SmaComponent

        If obj.ParentComponent Is Nothing Then Return Nothing

        If obj.ParentComponent.SmaTag = RequestedParentComponentType Then
            Return obj.ParentComponent
        Else
            Return obj.ParentComponent.GetClosestAncestorComponent(RequestedParentComponentType)
        End If

    End Function

    <Extension>
    Public Function GetTargetFromLineOfInitialComponents(obj As SmaComponent, ByVal TargetLevel As SpeechMaterialComponent.LinguisticLevels) As SmaComponent

        If obj.GetCorrespondingSpeechMaterialComponentLinguisticLevel() = TargetLevel Then

            'Returning Me
            Return obj

        Else

            'Not the right level, tries to call GetTargetFromLineOfInitialComponents on the first child
            If obj.Count > 0 Then

                Return obj(0).GetTargetFromLineOfInitialComponents(TargetLevel)

            Else
                'There are no child components
                Return Nothing

            End If

        End If

    End Function

    <Extension>
    Public Function GetCorrespondingSpeechMaterialComponentLinguisticLevel(obj As SmaComponent) As SpeechMaterialComponent.LinguisticLevels

        Select Case obj.SmaTag
            Case SmaTags.CHANNEL
                Return SpeechMaterialComponent.LinguisticLevels.List
            Case SmaTags.SENTENCE
                Return SpeechMaterialComponent.LinguisticLevels.Sentence
            Case SmaTags.WORD
                Return SpeechMaterialComponent.LinguisticLevels.Word
            Case SmaTags.PHONE
                Return SpeechMaterialComponent.LinguisticLevels.Phoneme
            Case Else
                Throw New Exception("Unable to convert the SmaTag value " & obj.SmaTag & " to SpeechMaterial.LinguisticLevels.")
        End Select

    End Function

    <Extension>
    Public Function GetStringRepresentation(obj As SmaComponent) As String

        If obj.OrthographicForm = "" And obj.PhoneticForm = "" Then
            Return ""
        Else
            If obj.OrthographicForm <> "" And obj.PhoneticForm <> "" Then
                If obj.PhoneticForm.StartsWith("[") Then
                    Return obj.OrthographicForm & vbCrLf & obj.PhoneticForm
                Else
                    'Adds square brackets to phonetic forms that lacks them
                    Return obj.OrthographicForm & vbCrLf & "[" & obj.PhoneticForm & "]"
                End If
            ElseIf obj.OrthographicForm <> "" Then
                Return obj.OrthographicForm
            Else
                If obj.PhoneticForm.StartsWith("[") Then
                    Return obj.PhoneticForm
                Else
                    'Adds square brackets to phonetic forms that lacks them
                    Return "[" & obj.PhoneticForm & "]"
                End If
            End If
        End If

    End Function


    ''' <summary>
    ''' Resets the sound levels of the component and cascading resets to all lower levels.
    ''' </summary>
    <Extension>
    Public Sub ResetSoundLevels(obj As SmaComponent)

        obj.UnWeightedLevel = Nothing
        obj.UnWeightedPeakLevel = Nothing
        obj.WeightedLevel = Nothing

        For Each childcomponent In obj
            childcomponent.ResetSoundLevels()
        Next

    End Sub

    ''' <summary>
    ''' Resets the segmentation time data (startSample and Length) of the component and cascading resets to all lower levels.
    ''' </summary>
    <Extension>
    Public Sub ResetTemporalData(obj As SmaComponent)

        obj.StartSample = -1
        obj.Length = 0

        For Each childcomponent In obj
            childcomponent.ResetTemporalData()
        Next

    End Sub

    ''' <summary>
    ''' Converts all instances of a specified phone to a new phone
    ''' </summary>
    ''' <param name="CurrentPhone"></param>
    ''' <param name="NewPhone"></param>
    <Extension>
    Public Sub ConvertPhone(obj As SmaComponent, ByVal CurrentPhone As String, ByVal NewPhone As String)
        For Each childComponent In obj

            'Replacing the phone
            childComponent.PhoneticForm = childComponent.PhoneticForm.Replace(CurrentPhone, NewPhone)

            'Cascading to all lower levels
            childComponent.ConvertPhone(CurrentPhone, NewPhone)

        Next

    End Sub


    ''' <summary>
    ''' Can be used to get the phonetic form, if no orthographic form is set, the phonetic form of the closest child in an unbroken line of single children.
    ''' </summary>
    ''' <returns></returns>
    <Extension>
    Public Function FindPhoneticForm(obj As SmaComponent) As String

        If obj.PhoneticForm <> "" Then
            Return obj.PhoneticForm
        Else
            If obj.Count = 1 Then
                Return obj(0).FindPhoneticForm
            Else
                Return ""
            End If
        End If

    End Function

    ''' <summary>
    ''' Can be used to get the orthographic form, if no orthographic form is set, the orthographic form of the closest child in an unbroken line of single children.
    ''' </summary>
    ''' <returns></returns>
    <Extension>
    Public Function FindOrthographicForm(obj As SmaComponent) As String

        If obj.OrthographicForm <> "" Then
            Return obj.OrthographicForm
        Else
            If obj.Count = 1 Then
                Return obj(0).FindOrthographicForm
            Else
                Return ""
            End If
        End If

    End Function

    '''' <summary>
    '''' Returns all SmaComponents that locically share the same StartSample value as the current instance of SmaComponent 
    '''' </summary>
    '''' <returns></returns>
    'Public Function GetDependentSegmentationsStarts() As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
    '    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
    '    OutputList.AddRange(GetDependentSegmentationsStarts(HierarchicalDirections.Upwards))
    '    OutputList.AddRange(GetDependentSegmentationsStarts(HierarchicalDirections.Downwards))
    '    Return OutputList
    'End Function

    'Private Function GetDependentSegmentationsStarts(ByVal HierarchicalDirection As HierarchicalDirections) As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

    '    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

    '    'Cascading up or down
    '    Select Case HierarchicalDirection
    '        Case HierarchicalDirections.Upwards

    '            If Me.SmaTag = SmaTags.CHANNEL Then Return OutputList

    '            Dim SelfIndex = GetSelfIndex()
    '            If SelfIndex.HasValue Then
    '                If SelfIndex = 0 Then
    '                    OutputList.AddRange(ParentComponent.GetDependentSegmentationsStarts(HierarchicalDirection))
    '                End If
    '            End If

    '        Case HierarchicalDirections.Downwards

    '            If Me.Count > 0 Then
    '                OutputList.AddRange(Me(0).GetDependentSegmentationsStarts(HierarchicalDirection))
    '            End If

    '    End Select

    '    Return OutputList

    'End Function

    '''' <summary>
    '''' Returns all SmaComponents that locically share the same end time as the current instance of SmaComponent 
    '''' </summary>
    '''' <returns></returns>
    'Public Function GetDependentSegmentationsEnds() As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
    '    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
    '    OutputList.AddRange(GetDependentSegmentationsEnds(HierarchicalDirections.Upwards))
    '    OutputList.AddRange(GetDependentSegmentationsEnds(HierarchicalDirections.Downwards))
    '    Return OutputList
    'End Function

    'Public Function GetDependentSegmentationsEnds(ByVal HierarchicalDirection As HierarchicalDirections) As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

    '    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

    '    'Cascading up or down
    '    Select Case HierarchicalDirection
    '        Case HierarchicalDirections.Upwards

    '            Dim SelfIndex = GetSelfIndex()
    '            If SelfIndex.HasValue Then
    '                Dim SiblingCount = GetSiblings.Count
    '                If SelfIndex = SiblingCount - 1 Then
    '                    'The current instance is the last of its same level components
    '                    OutputList.AddRange(ParentComponent.GetDependentSegmentationsEnds(HierarchicalDirection))
    '                End If
    '            End If

    '        Case HierarchicalDirections.Downwards

    '            If Me.Count > 0 Then
    '                OutputList.AddRange(Me(Me.Count - 1).GetDependentSegmentationsEnds(HierarchicalDirection))
    '            End If

    '    End Select

    '    Return OutputList

    'End Function

    'Public Function GetUnbrokenLineOfAncestorsWithoutSiblings() As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)

    '    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
    '    If ParentComponent IsNot Nothing Then
    '        If ParentComponent.GetNumberOfSiblingsExcludingSelf = 0 Then
    '            OutputList.AddRange(ParentComponent)
    '            OutputList.AddRange(ParentComponent.GetUnbrokenLineOfAncestorsWithoutSiblings)
    '        End If
    '    End If

    '    Return OutputList

    'End Function

End Module