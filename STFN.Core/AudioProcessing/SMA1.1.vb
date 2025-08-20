Imports System.IO
Imports System.Globalization.CultureInfo
Imports System.Xml.Serialization

Namespace Audio

    Partial Public Class Sound


        ''' <summary>
        ''' A class used to store data related to the segmentation of speech material sound files. All data can be contained and saved in a wave file.
        ''' </summary>
        <Serializable>
        Public Class SpeechMaterialAnnotation

            <XmlIgnore>
            Private ChangeDetector As Utils.GeneralIO.ObjectChangeDetector

            Public Sub StoreUnchangedState()
                ChangeDetector = New Utils.GeneralIO.ObjectChangeDetector(Me)
                ChangeDetector.SetUnchangedState()
            End Sub

            Public Function IsChanged() As Boolean?
                If ChangeDetector IsNot Nothing Then
                    Return ChangeDetector.IsChanged()
                Else
                    Return Nothing
                End If
            End Function

            <XmlIgnore>
            Public ParentSound As Sound

            Public Const CurrentVersion As String = "1.1"
            ''' <summary>
            ''' Holds the SMA version of the file that the data was loaded from, or CurrentVersion if the data was not loaded from file.
            ''' </summary>
            Public ReadFromVersion As String = CurrentVersion ' Using CurrentVersion as default

            ''' <summary>
            ''' The nominal level describes the speech material level with correction applied, thus not necessarily representing the actual level of the speech material, but should exactly represents the level of the calibration signal intended for use with the material.
            ''' </summary>
            Public Property NominalLevel As Double? = Nothing

            Public Property SegmentationCompleted As Boolean = True

            Public ReadOnly Property ChannelCount As Integer
                Get
                    Return _ChannelData.Count
                End Get
            End Property

            Private _ChannelData As New List(Of SmaComponent)
            Property ChannelData(ByVal Channel As Integer) As SmaComponent
                Get
                    Return _ChannelData(Channel - 1)
                End Get
                Set(value As SmaComponent)
                    _ChannelData(Channel - 1) = value
                End Set
            End Property

            Public Sub AddChannelData(Optional ByVal SourceFilePath As String = "")
                _ChannelData.Add(New SmaComponent(Me, SmaTags.CHANNEL, Nothing, SourceFilePath))
            End Sub

            Public Sub AddChannelData(ByRef NewSmaChannelData As SmaComponent)
                _ChannelData.Add(NewSmaChannelData)
            End Sub

            ''' <summary>
            ''' This private sub is intended to be used only when an object of the current class is cloned by Xml serialization, such as with CreateCopy. 
            ''' </summary>
            Private Sub New()

            End Sub

            ''' <summary>
            ''' Creates a new instance of SpeechMaterialAnnotation
            ''' </summary>
            Public Sub New(Optional ByVal DefaultFrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z,
                       Optional ByVal DefaultTimeWeighting As Double = 0)
                SetFrequencyWeighting(DefaultFrequencyWeighting, False)
                SetTimeWeighting(DefaultTimeWeighting, False)
            End Sub

            ''' <summary>
            ''' Enforcing the nominal level set in the current instance of SpeechMaterialAnnotation to all descendant channels, sentences, words, and phones.
            ''' This method should be called when loading SMA from wav files and after the nominal level has been set in the SpeechMaterialAnnotation object.
            ''' </summary>
            Public Sub InferNominalLevelToAllDescendants()
                'N.B. The reason that this is not instead doe in the set method of the NominalLevel property is that when the NominalLevel property value is set, not all descendant SmaComponents are necessarily loaded/attached.
                For Each c In _ChannelData
                    c.InferNominalLevelToAllDescendants(NominalLevel)
                Next
            End Sub

            Private FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z
            Public Function GetFrequencyWeighting() As FrequencyWeightings
                Return FrequencyWeighting
            End Function

            Public Sub SetFrequencyWeighting(ByVal FrequencyWeighting As FrequencyWeightings, ByVal EnforceOnAllDescendents As Boolean)

                'Setting only SMA top level frequency weighting
                Me.FrequencyWeighting = FrequencyWeighting

                If EnforceOnAllDescendents = True Then
                    'Enforcing the same frequency weighting on all descendant channels, sentences, words, and phones
                    For Each c In _ChannelData
                        c.SetFrequencyWeighting(FrequencyWeighting, EnforceOnAllDescendents)
                    Next
                End If
            End Sub

            Private TimeWeighting As Double = 0
            Public Function GetTimeWeighting() As Double
                Return TimeWeighting
            End Function

            Public Sub SetTimeWeighting(ByVal TimeWeighting As Double, ByVal EnforceOnAllDescendents As Boolean)

                'Setting only SMA top level Time weighting
                Me.TimeWeighting = TimeWeighting

                If EnforceOnAllDescendents = True Then
                    'Enforcing the same Time weighting on all descendant channels, sentences, words, and phones
                    For Each c In _ChannelData
                        c.SetTimeWeighting(TimeWeighting, EnforceOnAllDescendents)
                    Next
                End If
            End Sub


            Public Shadows Function ToString(ByVal IncludeHeadings As Boolean) As String

                Dim HeadingList As New List(Of String)
                Dim OutputList As New List(Of String)

                If IncludeHeadings = True Then
                    HeadingList.Add("SMA")
                    HeadingList.Add("SMA_VERSION")
                    OutputList.Add("")
                    OutputList.Add(Sound.SpeechMaterialAnnotation.CurrentVersion) 'I.e. SMA version number
                End If

                For c = 0 To _ChannelData.Count - 1

                    HeadingList.Add("CHANNEL")
                    OutputList.Add(c + 1)

                    _ChannelData(c).ToString(HeadingList, OutputList)
                Next

                If IncludeHeadings = True Then
                    Return String.Join(vbTab, HeadingList) & vbCrLf & String.Join(vbCrLf, OutputList)
                Else
                    Return String.Join(vbTab, OutputList)
                End If

            End Function


            Public Function CreateCopy(ByRef ParentSoundReference As Audio.Sound) As SpeechMaterialAnnotation

                'Creating an output object
                Dim newSmaData As SpeechMaterialAnnotation

                'Serializing to memorystream
                Dim serializedMe As New MemoryStream
                Dim serializer As New XmlSerializer(GetType(SpeechMaterialAnnotation))
                serializer.Serialize(serializedMe, Me)

                'Deserializing to new object
                serializedMe.Position = 0
                newSmaData = CType(serializer.Deserialize(serializedMe), SpeechMaterialAnnotation)
                serializedMe.Close()

                If ParentSoundReference IsNot Nothing Then
                    'Rerefereinging the parent sound
                    newSmaData.ParentSound = ParentSoundReference
                End If

                'Returning the new object
                Return newSmaData
            End Function

            Public Enum SmaTags
                CHANNEL
                SENTENCE
                WORD
                PHONE
            End Enum



            <Serializable>
            Public Class SmaComponent
                Inherits List(Of SmaComponent)

                Public Property ParentSMA As SpeechMaterialAnnotation

                Public Property ParentComponent As SmaComponent

                ''' <summary>
                ''' The nominal level describes the speech material level with correction applied, thus not necessarily representing the actual level of the speech material, but exaclty represent the level of the calibration signal intended for use with the material.
                ''' The value of nominal level is infered from the parent SMA object when read from a wave file, but never written back to wave files. Thus, to permenently alter the value of the nominal level, the nominal level value of the parent SMA object needs to be modified.
                ''' </summary>
                Public Property NominalLevel As Double? = Nothing

                Public Property SmaTag As SpeechMaterialAnnotation.SmaTags

                Private _SegmentationCompleted As Boolean = False

                Public Property SegmentationCompleted As Boolean
                    Get

                        'Always returns True for channel level Sma tags, at these need not be validated as they should always correspond to the start (i.e. 0) and length the wave channel data array
                        If Me.SmaTag = SmaTags.CHANNEL Then
                            _SegmentationCompleted = True
                        End If

                        Return _SegmentationCompleted

                    End Get
                    Set(value As Boolean)
                        _SegmentationCompleted = value
                    End Set
                End Property

                Public Property OrthographicForm As String = ""
                Public Property PhoneticForm As String = ""
                Private _StartSample As Integer = -1
                Public Property StartSample As Integer
                    Get
                        If Me.SmaTag = SmaTags.CHANNEL Then
                            'Channels start should always be 0
                            Return 0
                        Else
                            Return _StartSample
                        End If
                    End Get
                    Set(value As Integer)
                        _StartSample = value
                    End Set
                End Property

                Private _Length As Integer = 0
                Public Property Length As Integer
                    Get
                        'Channels should return sound channel length, or 0 if no sound exist
                        Dim LocalLength As Integer = _Length
                        If Me.SmaTag = SmaTags.CHANNEL Then
                            If ParentSMA IsNot Nothing Then
                                If ParentSMA.ParentSound IsNot Nothing Then
                                    Dim SmaChannel As Integer? = 1 '= Me.GetSelfIndex' TODO: Here we should specifiy the appropriate sound channel, but as of now, there is no way to get it. Use channel = 1 instead. This will break down if Sma annotations have multiple channels!
                                    If SmaChannel.HasValue Then
                                        LocalLength = ParentSMA.ParentSound.WaveData.SampleData(SmaChannel).Length
                                    End If
                                End If
                            End If
                        End If
                        Return LocalLength
                    End Get
                    Set(value As Integer)
                        _Length = value
                    End Set
                End Property

                'Sound level properties. Nothing if never set
                Public Property UnWeightedLevel As Double? = Nothing
                Public Property UnWeightedPeakLevel As Double? = Nothing

                Public Property FrequencyWeighting As FrequencyWeightings = FrequencyWeightings.Z

                Public Property TimeWeighting As Double = 0 'A time weighting of 0 indicates "no time weighting", thus the average RMS level is indicated.

                Public Property WeightedLevel As Double? = Nothing

                ''' <summary>
                ''' The level of a set of frequency bands defined in BandCentreFrequencies and BandWidths. (Introduced in SMA v 1.1)
                ''' </summary>
                ''' <returns></returns>
                Public Property BandLevels As Double() = Nothing
                ''' <summary>
                ''' The centre frequencies of the bands for which the level is given in BandLevels. (Introduced in SMA v 1.1)
                ''' </summary>
                ''' <returns></returns>
                Public Property CentreFrequencies As Double() = Nothing
                ''' <summary>
                ''' The band widths of the bands for which the level is given in BandLevels. (Introduced in SMA v 1.1)
                ''' </summary>
                ''' <returns></returns>
                Public Property BandWidths As Double() = Nothing

                ''' <summary>
                ''' The integration times used for each band when calculating the levels given in BandLevels. (Introduced in SMA v 1.1)
                ''' </summary>
                ''' <returns></returns>
                Public Property BandIntegrationTimes As Double() = Nothing

                ''' <summary>
                ''' Indicates the initial absolute peak amplitude, i.e. the absolute value of the most negative or the positive sample value, of the segment. The initial peak value should only be stored once for each segment, 
                ''' directly after segment segmentation, but prior to any gain modification. As such, the initial peak value may be utilized to calculate any gain changes applied to the sound section associated with a specific segment since the initial recording.
                ''' Its default value is -1. Before any gain is applied to such a segment the InitialPeak should be measured using the method SetInitialPeakAmplitude. And once it has been measured (i.e. has a value other than -1), it should not be measured again. 
                ''' In version 1.0, this value was only allowed for sentence tags, since verion 1.1, it is allowed for every type of segment.
                ''' </summary>
                ''' <returns></returns>
                Public Property InitialPeak As Double = -1

                ''' <summary>
                ''' Time in seconds relative to a user defined point in time.
                ''' In version 1.0, this value was only allowed for sentence tags, since verion 1.1, it is allowed for every type of segment.
                ''' </summary>
                ''' <returns></returns>
                Public Property StartTime As Double


                Private Shared DefaultNotMeasuredValue As String = "Not measured"

                ''' <summary>
                ''' If applicable, holds the file path to the file from which the current instance of SmaComponent was read
                ''' </summary>
                Public SourceFilePath As String = ""

                Public Sub New(ByRef ParentSMA As SpeechMaterialAnnotation, ByVal SmaLevel As SmaTags, ByRef ParentComponent As SmaComponent, Optional ByVal SourceWaveFilePath As String = "")
                    Me.ParentSMA = ParentSMA
                    Me.ParentComponent = ParentComponent
                    Me.SmaTag = SmaLevel
                    Me.FrequencyWeighting = ParentSMA.GetFrequencyWeighting
                    Me.TimeWeighting = ParentSMA.GetTimeWeighting
                    Me.SourceFilePath = SourceWaveFilePath
                End Sub

                Public Sub InferNominalLevelToAllDescendants(ByVal NominalLevel As Double?)

                    'Setting the time weighting
                    Me.NominalLevel = NominalLevel

                    'Enforcing the same nominal level on all descendants
                    For Each child In Me
                        child.InferNominalLevelToAllDescendants(NominalLevel)
                    Next
                End Sub

                Public Function GetFrequencyWeighting() As FrequencyWeightings
                    Return FrequencyWeighting
                End Function

                Public Sub SetFrequencyWeighting(ByVal FrequencyWeighting As FrequencyWeightings, ByVal EnforceOnAllDescendents As Boolean)

                    'Setting the frequency weighting
                    Me.FrequencyWeighting = FrequencyWeighting

                    If EnforceOnAllDescendents = True Then
                        'Enforcing the same frequency weighting on all descendant phones
                        For Each child In Me
                            child.SetFrequencyWeighting(FrequencyWeighting, EnforceOnAllDescendents)
                        Next
                    End If
                End Sub

                Public Function GetTimeWeighting() As Double
                    Return TimeWeighting
                End Function

                Public Sub SetTimeWeighting(ByVal TimeWeighting As Double, ByVal EnforceOnAllDescendents As Boolean)

                    'Setting the time weighting
                    Me.TimeWeighting = TimeWeighting

                    If EnforceOnAllDescendents = True Then

                        'Enforcing the same Time weighting on all descendant phones
                        For Each child In Me
                            child.SetTimeWeighting(TimeWeighting, EnforceOnAllDescendents)
                        Next
                    End If
                End Sub

                ''' <summary>
                ''' Returns a comma separated string representing the BandLevels
                ''' </summary>
                ''' <returns></returns>
                Public Function GetBandLevelsString() As String
                    Return GetCommaSeparatedArray(BandLevels)
                End Function

                ''' <summary>
                ''' Returns a comma separated string representing the CentreFrequencies
                ''' </summary>
                ''' <returns></returns>
                Public Function GetCentreFrequenciesString() As String
                    Return GetCommaSeparatedArray(CentreFrequencies)
                End Function

                ''' <summary>
                ''' Returns a comma separated string representing the BandWidths
                ''' </summary>
                ''' <returns></returns>
                Public Function GetBandWidthsString() As String
                    Return GetCommaSeparatedArray(BandWidths)
                End Function


                ''' <summary>
                ''' Returns a comma separated string representing the BandIntegrationTimes
                ''' </summary>
                ''' <returns></returns>
                Public Function GetBandIntegrationTimesString() As String
                    Return GetCommaSeparatedArray(BandIntegrationTimes)
                End Function


                Private Function GetCommaSeparatedArray(ByRef InputArray() As Double) As String

                    If InputArray Is Nothing Then Return ""
                    If InputArray.Count = 0 Then Return ""
                    Dim OutputList As New List(Of String)
                    For Each v In InputArray
                        OutputList.Add(v.ToString(InvariantCulture))
                    Next
                    Return String.Join(",", OutputList)

                End Function

                Public Sub SetBandLevelsFromString(ByVal CommaSeparatedValues As String)
                    BandLevels = GetValuesFromCommaSeparatedString(CommaSeparatedValues)
                End Sub

                Public Sub SetCentreFrequenciesFromString(ByVal CommaSeparatedValues As String)
                    CentreFrequencies = GetValuesFromCommaSeparatedString(CommaSeparatedValues)
                End Sub

                Public Sub SetBandWidthsFromString(ByVal CommaSeparatedValues As String)
                    BandWidths = GetValuesFromCommaSeparatedString(CommaSeparatedValues)
                End Sub

                Public Sub SetBandIntegrationTimesFromString(ByVal CommaSeparatedValues As String)
                    BandIntegrationTimes = GetValuesFromCommaSeparatedString(CommaSeparatedValues)
                End Sub


                Private Function GetValuesFromCommaSeparatedString(ByVal CommaSeparatedValues As String) As Double()

                    Dim CDS = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator

                    If CommaSeparatedValues Is Nothing Then Return {}
                    If CommaSeparatedValues.Length = 0 Then Return {}
                    Dim OutputList As New List(Of Double)
                    Dim SplitList() As String = CommaSeparatedValues.Trim.Split(",")
                    For Each v In SplitList

                        Dim value As Double
                        If Double.TryParse(v.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                            OutputList.Add(value)
                        Else
                            Throw New Exception("Unable to parse the following string as a list of double: " & CommaSeparatedValues & " (This error may be cause by corrupt SMA specifications in iXML wave file chunks.)")
                        End If

                    Next
                    Return OutputList.ToArray

                End Function



                Public Shadows Sub ToString(ByRef HeadingList As List(Of String), ByRef OutputList As List(Of String))

                    'Writing headings
                    HeadingList.Add("SEGMENTATION_COMPLETED")
                    HeadingList.Add("ORTHOGRAPHIC_FORM")
                    HeadingList.Add("PHONETIC_FORM")
                    HeadingList.Add("START_SAMPLE")
                    HeadingList.Add("LENGTH")
                    HeadingList.Add("UNWEIGHTED_LEVEL")
                    HeadingList.Add("UNWEIGHTED_PEAKLEVEL")
                    HeadingList.Add("WEIGHTED_LEVEL")
                    HeadingList.Add("FREQUENCY_WEIGHTING")
                    HeadingList.Add("TIME_WEIGHTING")
                    HeadingList.Add("INITIAL_PEAK")
                    HeadingList.Add("START_TIME")

                    'Writing data
                    OutputList.Add(SegmentationCompleted.ToString)
                    OutputList.Add(OrthographicForm)
                    OutputList.Add(PhoneticForm)
                    OutputList.Add(StartSample)
                    OutputList.Add(Length)
                    If UnWeightedLevel IsNot Nothing Then
                        OutputList.Add(UnWeightedLevel.Value.ToString(InvariantCulture))
                    Else
                        OutputList.Add(DefaultNotMeasuredValue)
                    End If

                    If UnWeightedPeakLevel IsNot Nothing Then
                        OutputList.Add(UnWeightedPeakLevel.Value.ToString(InvariantCulture))
                    Else
                        OutputList.Add(DefaultNotMeasuredValue)
                    End If

                    If WeightedLevel IsNot Nothing Then
                        OutputList.Add(WeightedLevel.Value.ToString(InvariantCulture))
                    Else
                        OutputList.Add(DefaultNotMeasuredValue)
                    End If
                    OutputList.Add(GetFrequencyWeighting.ToString)
                    OutputList.Add(GetTimeWeighting.ToString(InvariantCulture))

                    OutputList.Add(InitialPeak.ToString(InvariantCulture))
                    OutputList.Add(StartTime.ToString(InvariantCulture))

                    For n = 0 To Me.Count - 1

                        HeadingList.Add(Me(n).SmaTag.ToString)
                        OutputList.Add(n)

                        'Cascading to lower levels
                        Me(n).ToString(HeadingList, OutputList)

                    Next

                End Sub



                ''' <summary>
                ''' Returns all descendant SmaComponents in the current instance of SmaComponent 
                ''' </summary>
                ''' <returns></returns>
                Public Function GetAllDescentantComponents() As List(Of SmaComponent)

                    Dim AllComponents As New List(Of SmaComponent)

                    For Each child In Me
                        AllComponents.Add(child)
                    Next

                    For Each child In Me
                        AllComponents.AddRange(child.GetAllDescentantComponents())
                    Next

                    Return AllComponents

                End Function



                Public Function GetSiblingsExcludingSelf() As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                    Dim OutputList As New List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                    If ParentComponent IsNot Nothing Then
                        For Each child In ParentComponent
                            If child IsNot Me Then
                                OutputList.Add(child)
                            End If
                        Next
                    End If
                    Return OutputList
                End Function

                Public Function GetNumberOfSiblingsExcludingSelf() As Integer

                    Dim Siblings = GetSiblingsExcludingSelf()
                    If Siblings IsNot Nothing Then
                        Return Siblings.Count
                    Else
                        Return 0
                    End If

                End Function



                Public Sub GetUnbrokenLineOfAncestorsWithSingleChild(ByRef ResultList As List(Of Sound.SpeechMaterialAnnotation.SmaComponent))

                    If Me.GetNumberOfSiblingsExcludingSelf = 0 Then
                        If Me.ParentComponent IsNot Nothing Then
                            ResultList.Add(Me.ParentComponent)
                            Me.ParentComponent.GetUnbrokenLineOfAncestorsWithSingleChild(ResultList)
                        End If
                    End If

                End Sub


                Public Sub GetUnbrokenLineOfDescendentsWithSingleChild(ByRef ResultList As List(Of Sound.SpeechMaterialAnnotation.SmaComponent))

                    If Me.Count = 1 Then
                        ResultList.Add(Me(0))
                        Me(0).GetUnbrokenLineOfDescendentsWithSingleChild(ResultList)
                    End If

                End Sub


                Public Function GetSiblings() As List(Of Sound.SpeechMaterialAnnotation.SmaComponent)
                    If ParentComponent IsNot Nothing Then
                        Return ParentComponent.ToList
                    Else
                        Return Nothing
                    End If
                End Function



                ''' <summary>
                ''' 
                ''' </summary>
                ''' <param name="CurrentChannel"></param>
                ''' <returns>Returns True if all checks passed, and False if any check did not pass.</returns>
                Public Function CheckSoundDataAvailability(ByVal CurrentChannel As Integer, ByVal SupressWarnings As Boolean) As Boolean

                    'Checks that the needed instances exist
                    If ParentSMA Is Nothing Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: The current instance of SmaComponent does not have a ParentSMA object.")
                        Return False
                    End If
                    If ParentSMA.ParentSound Is Nothing Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: The ParentSMA does not have a ParentSound object.")
                        Return False
                    End If
                    If CurrentChannel > ParentSMA.ParentSound.WaveFormat.Channels Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: The ParentSMA.ParentSound object does not have " & CurrentChannel & " channels!")
                        Return False
                    End If
                    If CurrentChannel > ParentSMA.ChannelCount Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: The ParentSMA object does not have " & CurrentChannel & " channels!")
                        Return False
                    End If

                    'Checks that sound data exists at the indicated sampleindices
                    If StartSample < 0 Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: Start sample cannot be lower than zero!")
                        Return False
                    End If

                    If Length < 0 Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: Length of a section cannot lower than zero!")
                        Return False
                    End If

                    If Length = 0 Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: Length of a section cannot be zero!")
                        Return False
                    End If

                    If StartSample + Length > ParentSMA.ParentSound.WaveData.SampleData(CurrentChannel).Length Then
                        If SupressWarnings = False Then MsgBox("GetSoundFileSection: The requested data is outside the range of the sound parent sound!")
                        Return False
                    End If

                    Return True

                End Function


                ''' <summary>
                ''' Creates new (mono) sound containing the portion of the ParentSound that the current instance of SmaComponent represent, based on the available segmentation data. (The output sound does not contain the SMA object.)
                ''' </summary>
                ''' <param name="InitialMargin">If referenced by the calling code, and holds a negative number, returns the number of samples prior to the first sample. </param>
                ''' <returns></returns>
                Public Function GetSoundFileSection(ByVal CurrentChannel As Integer, Optional ByVal SupressWarnings As Boolean = False, Optional ByRef InitialMargin As Integer = -1) As Audio.Sound

                    If CheckSoundDataAvailability(CurrentChannel, SupressWarnings) = False Then
                        Return Nothing
                    End If

                    'Creates a new sound
                    Dim OutputSound = New Audio.Sound(New Formats.WaveFormat(ParentSMA.ParentSound.WaveFormat.SampleRate, ParentSMA.ParentSound.WaveFormat.BitDepth, 1, , ParentSMA.ParentSound.WaveFormat.Encoding))

                    'Stores the initial margin (i.e., the number of samples prior to the first sample retrieved), only if it has not been set
                    If InitialMargin < 0 Then InitialMargin = StartSample

                    'Copies data
                    OutputSound.WaveData.SampleData(1) = ParentSMA.ParentSound.WaveData.SampleData(CurrentChannel).ToList.GetRange(StartSample, Length).ToArray

                    'Also copies the Nominal level
                    OutputSound.SMA.NominalLevel = NominalLevel
                    OutputSound.SMA.InferNominalLevelToAllDescendants()

                    'Returns the sound
                    Return OutputSound

                End Function



                Public Function ReturnIsolatedSMA() As SpeechMaterialAnnotation

                    Dim SmaCopy = Me.ParentSMA.CreateCopy(Nothing)
                    SmaCopy._ChannelData.Clear()
                    SmaCopy.AddChannelData()

                    Dim ParentChannelCopy As SmaComponent = Nothing

                    Select Case Me.SmaTag
                        Case SmaTags.PHONE

                            Dim SmaPhoneCopy = Me.CreateCopy
                            SmaPhoneCopy.ParentSMA = SmaCopy

                            Dim ParentWordCopy = Me.ParentComponent.CreateCopy
                            ParentWordCopy.ParentSMA = SmaCopy
                            ParentWordCopy.Clear()

                            Dim ParentSentenceCopy = Me.ParentComponent.ParentComponent.CreateCopy
                            ParentSentenceCopy.ParentSMA = SmaCopy
                            ParentSentenceCopy.Clear()

                            ParentChannelCopy = Me.ParentComponent.ParentComponent.ParentComponent.CreateCopy
                            ParentChannelCopy.ParentSMA = SmaCopy
                            ParentChannelCopy.Clear()

                            SmaPhoneCopy.ParentComponent = ParentWordCopy
                            ParentWordCopy.ParentComponent = ParentSentenceCopy
                            ParentSentenceCopy.ParentComponent = ParentChannelCopy

                            ParentWordCopy.Add(SmaPhoneCopy)
                            ParentSentenceCopy.Add(ParentWordCopy)
                            ParentChannelCopy.Add(ParentSentenceCopy)

                        Case SmaTags.WORD

                            Dim ParentWordCopy = Me.CreateCopy
                            ParentWordCopy.ParentSMA = SmaCopy

                            Dim ParentSentenceCopy = Me.ParentComponent.CreateCopy
                            ParentSentenceCopy.ParentSMA = SmaCopy
                            ParentSentenceCopy.Clear()
                            ParentSentenceCopy.Add(ParentWordCopy)

                            ParentChannelCopy = Me.ParentComponent.ParentComponent.CreateCopy
                            ParentChannelCopy.ParentSMA = SmaCopy
                            ParentChannelCopy.Clear()
                            ParentChannelCopy.Add(ParentSentenceCopy)

                            ParentWordCopy.ParentComponent = ParentSentenceCopy
                            ParentSentenceCopy.ParentComponent = ParentChannelCopy


                        Case SmaTags.SENTENCE

                            Dim ParentSentenceCopy = Me.CreateCopy
                            ParentSentenceCopy.ParentSMA = SmaCopy

                            ParentChannelCopy = Me.ParentComponent.CreateCopy
                            ParentChannelCopy.ParentSMA = SmaCopy
                            ParentChannelCopy.Clear()
                            ParentChannelCopy.Add(ParentSentenceCopy)

                            ParentSentenceCopy.ParentComponent = ParentChannelCopy


                        Case SmaTags.CHANNEL

                            ParentChannelCopy = Me.ParentComponent.CreateCopy
                            ParentChannelCopy.ParentSMA = SmaCopy

                    End Select

                    'Connecting to the SMA channel data
                    SmaCopy.ChannelData(1) = ParentChannelCopy

                    Return SmaCopy

                End Function



                ''' <summary>
                ''' Creates a new SmaComponent which is a deep copy of the original, by using serialization.
                ''' </summary>
                ''' <returns></returns>
                Public Function CreateCopy() As SmaComponent

                    'Creating an output object
                    Dim newSmaComponent As SmaComponent

                    'Serializing to memorystream
                    Dim serializedMe As New MemoryStream
                    Dim serializer As New XmlSerializer(GetType(SmaComponent))
                    serializer.Serialize(serializedMe, Me)

                    'Deserializing to new object
                    serializedMe.Position = 0
                    newSmaComponent = CType(serializer.Deserialize(serializedMe), SmaComponent)
                    serializedMe.Close()

                    'Returning the new object
                    Return newSmaComponent
                End Function

                'Moves the start position of all child sma components nSamples without concidering the actual sound boundaries
                Public Sub TimeShift(ByRef nSamples As Integer)

                    StartSample += nSamples

                    For Each ChildComponent In Me
                        ChildComponent.TimeShift(nSamples)
                    Next

                End Sub


                ''' <summary>
                ''' Sets the value of the SegmentationCompleted property and optinally cascades that value to all descendant components.
                ''' </summary>
                ''' <param name="Value"></param>
                ''' <param name="CascadeToAllDescendants"></param>
                ''' <param name="LowestSmaLevel">The linguistic level at which to stop validating (highest is CHANNEL and lowest is PHONE)</param>
                Public Sub SetSegmentationCompleted(ByVal Value As Boolean, Optional ByVal InferToDependentComponents As Boolean = True,
                                                    Optional ByVal CascadeToAllDescendants As Boolean = False,
                                                    Optional ByVal LowestSmaLevel As SmaTags = SmaTags.PHONE,
                                                    Optional ByVal HighestSmaLevel As SmaTags = SmaTags.CHANNEL)

                    'Skipping setting the value if SmaTag is Channel, since channels should always be validated
                    If Me.SmaTag <> SmaTags.CHANNEL Then

                        If Me.SmaTag <= LowestSmaLevel And Me.SmaTag >= HighestSmaLevel Then
                            'Changing the validation value only if the current linguistic level is at or below HighestSmaLevel and at or above LowestSmaLevel
                            Me.SegmentationCompleted = Value
                        End If
                    End If

                    If CascadeToAllDescendants = True Then
                        For Each child In Me
                            child.SetSegmentationCompleted(Value, False, CascadeToAllDescendants, LowestSmaLevel, HighestSmaLevel)
                        Next
                    End If

                    If InferToDependentComponents = True Then

                        If InferToDependentComponents Then

                            Me.SetValidationValueOfDependentAncestors(Value)
                            Me.SetValidationValueOfDependentDescendents(Value)

                        End If


                        ''Gets all dependent segmentations starts 
                        'Dim DependentSegmentations = Me.GetDependentSegmentationsStarts

                        ''And all dependent segmentations ends
                        'DependentSegmentations.AddRange(Me.GetDependentSegmentationsEnds)

                        'For Each DependentSegmentation In DependentSegmentations
                        '    DependentSegmentation.SetSegmentationCompleted(Value, False, False)
                        'Next
                    End If

                End Sub

                Private Sub SetValidationValueOfDependentAncestors(ByVal ValidationValue As Boolean)

                    'Validates all members of an UnbrokenLineOfAncestorsWithSingleChild 
                    Dim UnbrokenLineOfAncestorsWithSingleChild As New List(Of SmaComponent)
                    Me.GetUnbrokenLineOfAncestorsWithSingleChild(UnbrokenLineOfAncestorsWithSingleChild)
                    For Each SmaComponent In UnbrokenLineOfAncestorsWithSingleChild
                        SmaComponent.SetSegmentationCompleted(ValidationValue, False, False)
                    Next

                    'Sets validation values upwards
                    Dim Siblings = Me.GetSiblings()
                    If Siblings IsNot Nothing Then
                        'Means there is a parent

                        'Exiting if ValidationValue = True either the firstborn or lastborn sibling component is not validated
                        If ValidationValue = True Then
                            If Siblings(0).SegmentationCompleted = False Or Siblings(Siblings.Count - 1).SegmentationCompleted = False Then
                                'Not both of the start and end of the sibling series are validated.
                                Exit Sub
                            End If
                        End If

                        'Setting parent validation value
                        Me.ParentComponent.SetSegmentationCompleted(ValidationValue, False, False)

                        'Validating recursively upwards
                        Me.ParentComponent.SetValidationValueOfDependentAncestors(ValidationValue)

                    End If

                End Sub

                Private Sub SetValidationValueOfDependentDescendents(ByVal ValidationValue As Boolean)

                    If ValidationValue = True Then

                        'Sets validation value of all members of an UnbrokenLineOfDescendentsWithSingleChild
                        Dim UnbrokenLineOfDescendentsWithSingleChild As New List(Of SmaComponent)
                        Me.GetUnbrokenLineOfDescendentsWithSingleChild(UnbrokenLineOfDescendentsWithSingleChild)
                        For Each SmaComponent In UnbrokenLineOfDescendentsWithSingleChild
                            SmaComponent.SetSegmentationCompleted(ValidationValue, False, False)
                        Next

                    Else

                        'Invalidates all descendents
                        Dim AllDescendents = Me.GetAllDescentantComponents()
                        For Each SmaComponent In AllDescendents
                            SmaComponent.SetSegmentationCompleted(ValidationValue, False, False)
                        Next

                    End If

                End Sub

            End Class

            Public Function GetSmaComponentByIndexSeries(ByVal IndexSeries As SpeechMaterialComponent.ComponentIndices, ByVal AudioFileLinguisticLevel As SpeechMaterialComponent.LinguisticLevels, ByVal SoundChannel As Integer) As SmaComponent

                'Correcting the indices below AudioFileLinguisticLevel
                'If AudioFileLinguisticLevel >= SpeechMaterial.LinguisticLevels.ListCollection Then IndexSeries.ListCollectionIndex = 0 'There is only list collection per recording
                If AudioFileLinguisticLevel >= SpeechMaterialComponent.LinguisticLevels.List Then IndexSeries.ListIndex = 0 'There is only list per recording
                If AudioFileLinguisticLevel >= SpeechMaterialComponent.LinguisticLevels.Sentence Then IndexSeries.SentenceIndex = 0 'There is only one sentence per recording
                If AudioFileLinguisticLevel >= SpeechMaterialComponent.LinguisticLevels.Word Then IndexSeries.WordIndex = 0 'There is only word per recording
                If AudioFileLinguisticLevel >= SpeechMaterialComponent.LinguisticLevels.Phoneme Then IndexSeries.PhoneIndex = 0 'There is only phoneme per recording

                If SoundChannel > Me.ChannelCount Then
                    Return Nothing
                End If

                'If IndexSeries.ListCollectionIndex <> 0 Then Return Nothing

                If IndexSeries.ListIndex <> 0 Then Return Nothing

                If IndexSeries.SentenceIndex > Me.ChannelData(SoundChannel).Count - 1 Then
                    Return Nothing
                ElseIf IndexSeries.SentenceIndex < 0 Then
                    Return Me.ChannelData(SoundChannel)
                End If

                If IndexSeries.WordIndex > Me.ChannelData(SoundChannel)(IndexSeries.SentenceIndex).Count - 1 Then
                    Return Nothing
                ElseIf IndexSeries.WordIndex < 0 Then
                    Return Me.ChannelData(SoundChannel)(IndexSeries.SentenceIndex)
                End If

                If IndexSeries.PhoneIndex > Me.ChannelData(SoundChannel)(IndexSeries.SentenceIndex)(IndexSeries.WordIndex).Count - 1 Then
                    Return Nothing
                ElseIf IndexSeries.PhoneIndex < 0 Then
                    Return Me.ChannelData(SoundChannel)(IndexSeries.SentenceIndex)(IndexSeries.WordIndex)
                Else
                    Return Me.ChannelData(SoundChannel)(IndexSeries.SentenceIndex)(IndexSeries.WordIndex)(IndexSeries.PhoneIndex)
                End If


            End Function

            'Moves the start position of all child sma components nSamples without concidering the actual sound boundaries
            Public Sub TimeShift(ByRef nSamples As Integer)

                For Channel = 1 To Me.ChannelCount
                    Me.ChannelData(1).TimeShift(nSamples)
                Next

            End Sub


        End Class

    End Class



End Namespace
