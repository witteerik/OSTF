Imports System.Globalization
Imports STFN.Core.Audio.Sound.SpeechMaterialAnnotation


<Serializable>
Public Class SpeechMaterialComponent

    ''' <summary>
    ''' This constant holds the file expected file name of the standard speech material components tab delimeted text file.
    ''' </summary>
    Public Const SpeechMaterialComponentFileName As String = "SpeechMaterialComponents.txt"

    Private _ParentTestSpecification As SpeechMaterialSpecification
    Public Property ParentTestSpecification As SpeechMaterialSpecification
        Get
            'Recursively redirects to the speech material component at the highest level, so that the ParentTestSpecification only exists at one single place in each speech material component hierarchy.
            If Me.ParentComponent Is Nothing Then
                Return _ParentTestSpecification
            Else
                Return Me.ParentComponent.ParentTestSpecification
            End If
        End Get
        Set(value As SpeechMaterialSpecification)
            'Recursively redirects to the speech material component at the highest level, so that the ParentTestSpecification only exists at one single place in each speech material component hierarchy.
            If Me.ParentComponent Is Nothing Then
                _ParentTestSpecification = value
            Else
                Me.ParentComponent.ParentTestSpecification = value
            End If
        End Set
    End Property

    Public Property LinguisticLevel As LinguisticLevels

    Public Enum LinguisticLevels
        ListCollection ' Represents a collection of test lists which together forms a speech material. This level cannot have sound recordings.
        List ' Represents a full speech test list. Should always have one or more sentences as child components. This level may have sound recordings.
        Sentence ' Represents a sentence, may have one or more words as child components (in a word list, each sentence will always have only one single word). This level may have sound recordings.
        Word ' Represents a word in a sentence, may have one or more phonemes as child components. This level may have sound recordings.
        Phoneme ' Represents a phoneme in a word. This level may have sound recordings.
    End Enum

    Public Property Id As String

    Public Property ParentComponent As SpeechMaterialComponent

    Public Property PrimaryStringRepresentation As String

    Public Property ChildComponents As New List(Of SpeechMaterialComponent)

    'These two should contain the data defined in the LinguisticDatabase associated to the component in the speech material file.
    Private NumericVariables As New SortedList(Of String, Double)
    Private CategoricalVariables As New SortedList(Of String, String)


    Public Function IsSequentiallyOrdered() As Boolean
        Select Case Me.LinguisticLevel
            Case LinguisticLevels.ListCollection
                'Always returns false for list collections as they do not support sequential ordering
                Return False
            Case LinguisticLevels.List
                Return Me.SequentiallyOrderedLists
            Case LinguisticLevels.Sentence
                Return Me.SequentiallyOrderedSentences
            Case LinguisticLevels.Word
                Return Me.SequentiallyOrderedWords
            Case LinguisticLevels.Phoneme
                Return Me.SequentiallyOrderedPhonemes
            Case Else
                Throw New Exception("Unknown Linguistic level of component " & Me.PrimaryStringRepresentation)
        End Select
    End Function

    Private _SequentiallyOrderedLists As Boolean
    Private _SequentiallyOrderedSentences As Boolean
    Private _SequentiallyOrderedWords As Boolean
    Private _SequentiallyOrderedPhonemes As Boolean
    Private _PresetLevel As LinguisticLevels
    Private _PresetSpecifications As New List(Of Tuple(Of String, Boolean, List(Of String)))
    Private _Presets As SmcPresets

    Public Property SequentiallyOrderedLists As Boolean
        Get
            If Me.ParentComponent IsNot Nothing Then
                Return Me.ParentComponent.SequentiallyOrderedLists
            Else
                Return Me._SequentiallyOrderedLists
            End If
        End Get
        Set(value As Boolean)
            If Me.ParentComponent IsNot Nothing Then
                Me.ParentComponent.SequentiallyOrderedLists = value
            Else
                Me._SequentiallyOrderedLists = value
            End If
        End Set
    End Property

    Public Property SequentiallyOrderedSentences As Boolean
        Get
            If Me.ParentComponent IsNot Nothing Then
                Return Me.ParentComponent.SequentiallyOrderedSentences
            Else
                Return Me._SequentiallyOrderedSentences
            End If
        End Get
        Set(value As Boolean)
            If Me.ParentComponent IsNot Nothing Then
                Me.ParentComponent.SequentiallyOrderedSentences = value
            Else
                Me._SequentiallyOrderedSentences = value
            End If
        End Set
    End Property

    Public Property SequentiallyOrderedWords As Boolean
        Get
            If Me.ParentComponent IsNot Nothing Then
                Return Me.ParentComponent.SequentiallyOrderedWords
            Else
                Return Me._SequentiallyOrderedWords
            End If
        End Get
        Set(value As Boolean)
            If Me.ParentComponent IsNot Nothing Then
                Me.ParentComponent.SequentiallyOrderedWords = value
            Else
                Me._SequentiallyOrderedWords = value
            End If
        End Set
    End Property

    Public Property SequentiallyOrderedPhonemes As Boolean
        Get
            If Me.ParentComponent IsNot Nothing Then
                Return Me.ParentComponent.SequentiallyOrderedPhonemes
            Else
                Return Me._SequentiallyOrderedPhonemes
            End If
        End Get
        Set(value As Boolean)
            If Me.ParentComponent IsNot Nothing Then
                Me.ParentComponent.SequentiallyOrderedPhonemes = value
            Else
                Me._SequentiallyOrderedPhonemes = value
            End If
        End Set
    End Property

    Public Property PresetLevel As LinguisticLevels
        Get
            If Me.ParentComponent IsNot Nothing Then
                Return Me.ParentComponent.PresetLevel
            Else
                Return Me._PresetLevel
            End If
        End Get
        Set(value As LinguisticLevels)
            If Me.ParentComponent IsNot Nothing Then
                Me.ParentComponent.PresetLevel = value
            Else
                Me._PresetLevel = value
            End If
        End Set
    End Property

    Public Property PresetSpecifications As List(Of Tuple(Of String, Boolean, List(Of String)))
        Get
            If Me.ParentComponent IsNot Nothing Then
                Return Me.ParentComponent.PresetSpecifications
            Else
                Return Me._PresetSpecifications
            End If
        End Get
        Set(value As List(Of Tuple(Of String, Boolean, List(Of String))))
            If Me.ParentComponent IsNot Nothing Then
                Me.ParentComponent.PresetSpecifications = value
            Else
                Me._PresetSpecifications = value
            End If
        End Set
    End Property

    Public Property Presets As SmcPresets
        Get
            If Me.ParentComponent IsNot Nothing Then
                Return Me.ParentComponent.Presets
            Else
                Return Me._Presets
            End If
        End Get
        Set(value As SmcPresets)
            If Me.ParentComponent IsNot Nothing Then
                Me.ParentComponent.Presets = value
            Else
                Me._Presets = value
            End If
        End Set
    End Property

    Public Property IsPractiseComponent As Boolean = False


    ''' <summary>
    ''' Searches among the numeric custom variables for a variable named IsKeyComponent and returns its value (1=True and 0=False), or True if no such numeric variable exists.
    ''' </summary>
    ''' <returns></returns>
    Public Function IsKeyComponent() As Boolean

        Dim IsKeyComponentNumeric = GetNumericVariableValue("IsKeyComponent")

        Dim ReturnValue As Boolean = True 'Using True as the default return value
        If IsKeyComponentNumeric.HasValue Then
            If IsKeyComponentNumeric = 1 Then
                ReturnValue = True
            Else
                ReturnValue = False
            End If
        End If

        Return ReturnValue

    End Function

    ''' <summary>
    ''' Returns the expected name of the media folder of the current component
    ''' </summary>
    ''' <returns></returns>
    Public Function GetMediaFolderName() As String

        Return (Id & "_" & PrimaryStringRepresentation).Replace(" ", "_")

    End Function

    Public Function GetMaskerFolderName() As String

        Return (Id & "_" & PrimaryStringRepresentation).Replace(" ", "_")

    End Function

    Public Function GetContralateralMaskerFolderName() As String

        Return (Id & "_" & PrimaryStringRepresentation).Replace(" ", "_")

    End Function

    Public Randomizer As Random

    'Shared stuff used to keep media items in memory instead of re-loading on every use
    Public Shared AudioFileLoadMode As MediaFileLoadModes = MediaFileLoadModes.LoadOnFirstUse
    Public Shared SoundLibrary As New SortedList(Of String, Audio.Sound)
    Public Enum MediaFileLoadModes
        LoadEveryTime
        LoadOnFirstUse
    End Enum

    'Declares a constans sub-folder name, under which the speech material component file and the correcponding custom variables files should be put.
    Public Const SpeechMaterialFolderName As String = "SpeechMaterial"

    'Setting up some default strings
    Public Shared DefaultSpellingVariableName As String = "Spelling"
    Public Shared DefaultTranscriptionVariableName As String = "Transcription"
    Public Shared DefaultIsKeyComponentVariableName As String = "IsKeyComponent"


    Public Function GetDatabaseFileName()
        Return GetDatabaseFileName(Me.LinguisticLevel)
    End Function

    Public Shared Function GetDatabaseFileName(ByVal LinguisticLevel As SpeechMaterialComponent.LinguisticLevels)
        'Defining default names for database files
        Select Case LinguisticLevel
            Case LinguisticLevels.ListCollection
                Return "SpeechMaterialLevelVariables.txt"
            Case LinguisticLevels.List
                Return "ListLevelVariables.txt"
            Case LinguisticLevels.Sentence
                Return "SentenceLevelVariables.txt"
            Case LinguisticLevels.Word
                Return "WordLevelVariables.txt"
            Case LinguisticLevels.Phoneme
                Return "PhonemeLevelVariables.txt"
            Case Else
                Throw New ArgumentException("Unkown SpeechMaterial.LinguisticLevel")
        End Select
    End Function

    Public Sub New(ByRef rnd As Random)
        Me.Randomizer = rnd
    End Sub


    Private Function GetAvailableFiles(ByVal Folder As String) As List(Of String)

        'Getting files in that folder
        Dim AvailableFiles = IO.Directory.GetFiles(Folder)
        Dim IncludedFiles As New List(Of String)

        Dim AllowedFileExtensions As New List(Of String)
        AllowedFileExtensions.Add(".wav")

        For Each file In AvailableFiles
            For Each ext In AllowedFileExtensions
                If file.EndsWith(ext) Then
                    IncludedFiles.Add(file)
                    'Exits the inner loop, as the files is now added
                    Exit For
                End If
            Next
        Next

        Return IncludedFiles

    End Function


    ''' <summary>
    ''' Returns the sound representing the maskers for the current speech material component and mediaset as a new Audio.Sound. 
    ''' </summary>
    ''' <param name="MediaSet"></param>
    ''' <param name="Index"></param>
    ''' <returns></returns>
    Public Function GetMaskerSound(ByRef MediaSet As MediaSet, ByVal Index As Integer) As Audio.Sound

        Dim MaskerPath = GetMaskerPath(MediaSet, Index)

        Return GetSoundFile(MaskerPath)

    End Function

    ''' <summary>
    ''' Returns the sound representing the contralateral maskers for the current speech material component and mediaset as a new Audio.Sound. 
    ''' </summary>
    ''' <param name="MediaSet"></param>
    ''' <param name="Index"></param>
    ''' <returns></returns>
    Public Function GetContralateralMaskerSound(ByRef MediaSet As MediaSet, ByVal Index As Integer) As Audio.Sound

        Dim MaskerPath = GetContralateralMaskerPath(MediaSet, Index)

        Return GetSoundFile(MaskerPath)

    End Function

    ''' <summary>
    ''' Returns the sound representing background non-speech used with the current speech material component and mediaset as a new Audio.Sound. 
    ''' </summary>
    ''' <param name="MediaSet"></param>
    ''' <param name="Index"></param>
    ''' <returns></returns>
    Public Function GetBackgroundNonspeechSound(ByRef MediaSet As MediaSet, ByVal Index As Integer) As Audio.Sound

        Dim SoundPath = GetBackgroundNonspeechPath(MediaSet, Index)

        Return GetSoundFile(SoundPath)

    End Function


    ''' <summary>
    ''' Returns the sound representing background speech used with the current speech material component and mediaset as a new Audio.Sound. 
    ''' </summary>
    ''' <param name="MediaSet"></param>
    ''' <param name="Index"></param>
    ''' <returns></returns>
    Public Function GetBackgroundSpeechSound(ByRef MediaSet As MediaSet, ByVal Index As Integer) As Audio.Sound

        Dim SoundPath = GetBackgroundSpeechPath(MediaSet, Index)

        Return GetSoundFile(SoundPath)

    End Function




    ''' <summary>
    ''' Returns the audio representing the speech material component as a new Audio.Sound. If the recordings of the component is split between different sound files, a concatenated (using optional overlap/crossfade period) sound is returned.
    ''' </summary>
    ''' <param name="MediaSet"></param>
    ''' <param name="Index"></param>
    ''' <param name="SoundChannel"></param>
    ''' <param name="CrossFadeLength">The length (in sample) of a cross-fade section.</param>
    ''' <param name="InitialMargin">If referenced in the calling code, returns the number of samples prior to the first sample in the first sound file used in for returning the SMC soundv.</param>
    ''' <param name="SoundSourceSmaComponents">If referenced in the calling code, contains a list of the source sma components from which the sound was loaded.</param>
    ''' <param name="RemoveInterComponentSound">If set to True, and the component has segmented child components, the concatenated sound of these child components will be returned, and all intermediate sound sections (which should most often be silence) are removed. If set to true, the whole sound, including possibly silent section between child components will be returned.</param>
    ''' <returns></returns>
    Public Function GetSound(ByRef MediaSet As MediaSet, ByVal Index As Integer, ByVal SoundChannel As Integer,
                             Optional ByVal CrossFadeLength As Integer? = Nothing,
                             Optional ByVal Paddinglength As Integer? = Nothing,
                             Optional ByVal InterComponentlength As Integer? = Nothing,
                             Optional ByRef InitialMargin As Integer = 0,
                             Optional ByVal RectifySmaComponents As Boolean = False,
                             Optional ByVal SupressWarnings As Boolean = False,
                             Optional ByVal RandomizeOrder As Boolean = False,
                             Optional ByVal RandomSeed As Integer? = Nothing,
                             Optional ByRef SoundSourceSmaComponents As List(Of SmaComponent) = Nothing,
                                          Optional RemoveInterComponentSound As Boolean = False) As Audio.Sound

        'Setting initial margin to -1 to signal that it has not been set
        InitialMargin = -1

        Dim ReturnSound As Audio.Sound = Nothing
        Dim CumulativeTimeShift As Integer = 0

        If Paddinglength.HasValue Then
            If Paddinglength <= 0 Then Paddinglength = Nothing
        End If

        If InterComponentlength.HasValue Then
            If InterComponentlength <= 0 Then InterComponentlength = Nothing
        End If

        Dim CorrespondingSmaComponentList = GetCorrespondingSmaComponent(MediaSet, Index, SoundChannel, True)

        If RemoveInterComponentSound = True Then
            Dim ChildAdded As Boolean = False
            Dim ParentAdded As Boolean = False
            Dim MixedLevelsAdded As Boolean = False
            Dim TempSmaList = New List(Of SmaComponent)
            For Each SmaComponent In CorrespondingSmaComponentList
                If SmaComponent.Count > 0 Then
                    For Each ChildComponent In SmaComponent
                        'Adding child components instead of
                        TempSmaList.Add(ChildComponent)
                        ChildAdded = True
                        If ParentAdded = True Then MixedLevelsAdded = True
                    Next
                Else
                    TempSmaList.Add(SmaComponent)
                    ParentAdded = True
                    If ChildAdded = True Then MixedLevelsAdded = True
                End If
                CorrespondingSmaComponentList = TempSmaList
            Next

            If MixedLevelsAdded = True Then
                If SupressWarnings = False Then
                    MsgBox("Mixed SMA component levels in GetSound! This is probably caused by segmentations of different linguistic depths in the material, and is not allowed!")
                    Return Nothing
                End If
            End If

        End If

        If CorrespondingSmaComponentList.Count = 0 Then
            Return Nothing

            'This section should not be necessary, as the below block does the same (and also with padding)
            'ElseIf CorrespondingSmaComponentList.Count = 1 Then

            '    ReturnSound = CorrespondingSmaComponentList(0).GetSoundFileSection(SoundChannel, SupressWarnings, InitialMargin)

            '    If ReturnSound IsNot Nothing Then
            '        If RectifySmaComponents = True Then ReturnSound.SMA = CorrespondingSmaComponentList(0).ReturnIsolatedSMA
            '    End If

        Else
            Dim SoundList As New List(Of Audio.Sound)
            Dim SmaList As New List(Of Audio.Sound.SpeechMaterialAnnotation)
            Dim PaddingSound As Audio.Sound = Nothing
            Dim SilentInterStimulusSound As Audio.Sound = Nothing

            If RandomizeOrder = True Then
                'Randomizing order of Sma components
                Dim TempSummaryComponents As New List(Of SmaComponent)
                Dim Randomizer As New Random(RandomSeed)
                Do Until CorrespondingSmaComponentList.Count = 0
                    Dim RandomIndex As Integer = Randomizer.Next(0, CorrespondingSmaComponentList.Count)
                    TempSummaryComponents.Add(CorrespondingSmaComponentList(RandomIndex))
                    CorrespondingSmaComponentList.RemoveAt(RandomIndex)
                Loop
                CorrespondingSmaComponentList = TempSummaryComponents
            End If

            For i = 0 To CorrespondingSmaComponentList.Count - 1

                Dim SmaComponent = CorrespondingSmaComponentList(i)

                'Stores the Source Sma component
                If SoundSourceSmaComponents IsNot Nothing Then SoundSourceSmaComponents.Add(SmaComponent)

                'Getting the sound
                Dim TempInitialMargin As Integer = -1
                Dim CurrentComponentSound = SmaComponent.GetSoundFileSection(SoundChannel, SupressWarnings, TempInitialMargin)

                'Returns nothing if any sound could not be loaded
                If CurrentComponentSound Is Nothing Then Return Nothing

                'Setting this only the first time, TempInitialMargin can be used locally in every loop 
                If InitialMargin < 0 Then InitialMargin = TempInitialMargin

                'Creating a padding sound if needed
                If Paddinglength.HasValue Then
                    If PaddingSound Is Nothing Then
                        PaddingSound = DSP.CreateSilence(CurrentComponentSound.WaveFormat,, Paddinglength.Value, Audio.BasicAudioEnums.TimeUnits.samples)
                    End If
                    If i = 0 Then
                        'Adding initial padding sound
                        SoundList.Add(PaddingSound)

                        'Shifts the time by the length of the inserted silence
                        CumulativeTimeShift += PaddingSound.WaveData.SampleData(SoundChannel).Length

                        'We should not time shift the first sound..., or?
                        'If CrossFadeLength.HasValue Then
                        '    'Shifts the time backwards by CrossFadeLength
                        '    CumulativeTimeShift -= CrossFadeLength
                        'End If

                    End If
                End If

                'Adding sound and Sma component
                SoundList.Add(CurrentComponentSound)

                If CrossFadeLength.HasValue Then
                    'Shifts the time backwards by CrossFadeLength
                    CumulativeTimeShift -= CrossFadeLength
                End If

                'Shifts the current SMA
                If RectifySmaComponents = True Then
                    Dim IsolatedSMA = SmaComponent.ReturnIsolatedSMA
                    'Shifts time in the SMA based on the CumulativeTimeShift minus the current SMA margin
                    IsolatedSMA.TimeShift(CumulativeTimeShift - TempInitialMargin)
                    SmaList.Add(IsolatedSMA)
                End If

                'Shifts the time with the length of the added sound
                CumulativeTimeShift += CurrentComponentSound.WaveData.SampleData(SoundChannel).Length

                'Adding inter-stimulus sound if needed, but not after the last component
                If InterComponentlength.HasValue Then
                    If i <> CorrespondingSmaComponentList.Count - 1 Then
                        'Creating a inter-stimulus sound if needed
                        If SilentInterStimulusSound Is Nothing Then
                            SilentInterStimulusSound = DSP.CreateSilence(CurrentComponentSound.WaveFormat,, InterComponentlength.Value, Audio.BasicAudioEnums.TimeUnits.samples)
                        End If

                        'Adding the interstimulus sound
                        SoundList.Add(SilentInterStimulusSound)

                        'Shifts the time by the length of the inserted silence
                        CumulativeTimeShift += SilentInterStimulusSound.WaveData.SampleData(SoundChannel).Length

                        If CrossFadeLength.HasValue Then
                            'Shifts the time backwards by CrossFadeLength
                            CumulativeTimeShift -= CrossFadeLength
                        End If

                    End If
                End If

            Next

            'Adding the final padding sound if needed
            If Paddinglength.HasValue Then
                SoundList.Add(PaddingSound)
            End If

            Dim RectifiedSMA As Audio.Sound.SpeechMaterialAnnotation = Nothing
            If RectifySmaComponents = True Then

                'Referencing the first item in the sma list as the SMA
                RectifiedSMA = SmaList(0)

                Select Case CorrespondingSmaComponentList(0).SmaTag
                    Case SmaTags.CHANNEL

                        'No need to do anything here?

                    Case SmaTags.SENTENCE

                        For i = 1 To SmaList.Count - 1
                            'Adding the remaining items on the sentence level
                            Dim CurrentSentence = SmaList(i).ChannelData(SoundChannel)(0)
                            'Referencing the correct objects
                            RectifiedSMA.ChannelData(SoundChannel).Add(CurrentSentence)
                            CurrentSentence.ParentComponent = RectifiedSMA.ChannelData(SoundChannel)
                            CurrentSentence.ParentSMA = RectifiedSMA
                        Next

                    Case SmaTags.WORD

                        For i = 1 To SmaList.Count - 1
                            'Adding the remaining items on the word level
                            Dim CurrentWord = SmaList(i).ChannelData(SoundChannel)(0)
                            'Referencing the correct objects
                            RectifiedSMA.ChannelData(SoundChannel)(0).Add(CurrentWord)
                            CurrentWord.ParentComponent = RectifiedSMA.ChannelData(SoundChannel)(0)
                            CurrentWord.ParentSMA = RectifiedSMA
                        Next

                    Case SmaTags.PHONE

                        For i = 1 To SmaList.Count - 1
                            'Adding the remaining items on the phone level
                            Dim CurrentPhone = SmaList(i).ChannelData(SoundChannel)(0)(0)
                            'Referencing the correct objects
                            RectifiedSMA.ChannelData(SoundChannel)(0)(0).Add(CurrentPhone)
                            CurrentPhone.ParentComponent = RectifiedSMA.ChannelData(SoundChannel)(0)(0)
                            CurrentPhone.ParentSMA = RectifiedSMA
                        Next

                End Select
            End If

            'Creating the concatenated output sound
            If SoundList.Count = 1 Then
                ReturnSound = SoundList(0) ' Directly referencing the return sound
            ElseIf SoundList.Count > 0 Then
                ReturnSound = DSP.ConcatenateSounds(SoundList, ,,,,, CrossFadeLength)
            End If

            If ReturnSound IsNot Nothing Then
                If RectifySmaComponents = True Then

                    'Referencing the RectifiedSMA as the SMA object of the ReturnSound, and the ReturnSound as the ParentSound of the RectifiedSMA object.
                    ReturnSound.SMA = RectifiedSMA
                    RectifiedSMA.ParentSound = ReturnSound

                    'Setting channel start sample to 0 and length to the actual sound length
                    RectifiedSMA.ChannelData(SoundChannel).StartSample = 0
                    RectifiedSMA.ChannelData(SoundChannel).Length = RectifiedSMA.ParentSound.WaveData.SampleData(SoundChannel).Length

                End If
            End If

        End If

        'Changing InitialMargin to 0 if it was never set
        If InitialMargin < 0 Then InitialMargin = 0

        Return ReturnSound

    End Function


    Public Function FindSelfIndices(Optional ByRef ComponentIndices As ComponentIndices = Nothing) As ComponentIndices

        If ComponentIndices Is Nothing Then ComponentIndices = New ComponentIndices

        'Determining the component level self index and then the self indices at all higher linguistic levels
        Select Case Me.LinguisticLevel
            Case LinguisticLevels.Phoneme
                ComponentIndices.PhoneIndex = Me.GetSelfIndex
            Case LinguisticLevels.Word
                ComponentIndices.WordIndex = Me.GetSelfIndex
            Case LinguisticLevels.Sentence
                ComponentIndices.SentenceIndex = Me.GetSelfIndex
            Case LinguisticLevels.List
                ComponentIndices.ListIndex = Me.GetSelfIndex
                'Case LinguisticLevels.ListCollection
                '    ComponentIndices.ListCollectionIndex = Me.GetSelfIndex
            Case Else
                'If it's a ListCollection, the ComponentIndices are simply returned as it will have neither an index nor a parent
                Return ComponentIndices
        End Select

        'Calls FindSelfIndices on the parent
        Me.ParentComponent.FindSelfIndices(ComponentIndices)

        Return ComponentIndices

    End Function

    Public Class ComponentIndices

        'Public Property ListCollectionIndex As Integer
        '    Get
        '        Return IndexList(LinguisticLevels.ListCollection)
        '    End Get
        '    Set(value As Integer)
        '        IndexList(LinguisticLevels.ListCollection) = value
        '    End Set
        'End Property

        Public Property ListIndex As Integer
            Get
                Return IndexList(LinguisticLevels.List)
            End Get
            Set(value As Integer)
                IndexList(LinguisticLevels.List) = value
            End Set
        End Property

        Public Property SentenceIndex As Integer
            Get
                Return IndexList(LinguisticLevels.Sentence)
            End Get
            Set(value As Integer)
                IndexList(LinguisticLevels.Sentence) = value
            End Set
        End Property

        Public Property WordIndex As Integer
            Get
                Return IndexList(LinguisticLevels.Word)
            End Get
            Set(value As Integer)
                IndexList(LinguisticLevels.Word) = value
            End Set
        End Property

        Public Property PhoneIndex As Integer
            Get
                Return IndexList(LinguisticLevels.Phoneme)
            End Get
            Set(value As Integer)
                IndexList(LinguisticLevels.Phoneme) = value
            End Set
        End Property

        Public IndexList As New SortedList(Of SpeechMaterialComponent.LinguisticLevels, Integer)

        Public Sub New()

            'IndexList.Add(LinguisticLevels.ListCollection, -1)
            IndexList.Add(LinguisticLevels.List, -1)
            IndexList.Add(LinguisticLevels.Sentence, -1)
            IndexList.Add(LinguisticLevels.Word, -1)
            IndexList.Add(LinguisticLevels.Phoneme, -1)

        End Sub

        Public Function HasPhoneIndex() As Boolean
            'If ListCollectionIndex > -1 And ListIndex > -1 And SentenceIndex > -1 And WordIndex > -1 And PhoneIndex > -1 Then
            If ListIndex > -1 And SentenceIndex > -1 And WordIndex > -1 And PhoneIndex > -1 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function HasWordIndex() As Boolean
            'If ListCollectionIndex > -1 And ListIndex > -1 And SentenceIndex > -1 And WordIndex > -1 Then
            If ListIndex > -1 And SentenceIndex > -1 And WordIndex > -1 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function HasSentenceIndex() As Boolean
            'If ListCollectionIndex > -1 And ListIndex > -1 And SentenceIndex > -1 Then
            If ListIndex > -1 And SentenceIndex > -1 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function HasListIndex() As Boolean
            'If ListCollectionIndex > -1 And ListIndex > -1 Then
            If ListIndex > -1 Then
                Return True
            Else
                Return False
            End If
        End Function

        'Public Function HasListCollectionIndex() As Boolean
        '    If ListCollectionIndex > -1 Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function

    End Class

    ''' <summary>
    ''' Locates and returns any existing SMA object, or objects, that represent the current Speech Material Component, and returns them in a list of SmaComponent.
    ''' </summary>
    ''' <param name="MediaSet"></param>
    ''' <param name="Index"></param>
    ''' <param name="SoundChannel"></param>
    ''' <param name="UniquePrimaryStringRepresenations">If set to true, only the first occurence of a set of components that have the same PrimaryStringRepresentation will be included. This can be used to include multiple instantiations of the same component one once.</param>
    ''' <returns></returns>
    Public Function GetCorrespondingSmaComponent(ByRef MediaSet As MediaSet, ByVal Index As Integer, ByVal SoundChannel As Integer,
                                                 ByVal IncludePractiseComponents As Boolean, Optional ByVal UniquePrimaryStringRepresenations As Boolean = False) As List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)

        Dim SelfIndices = FindSelfIndices()

        Dim SmcsWithSoundFile As New List(Of SpeechMaterialComponent)

        If Me.LinguisticLevel = MediaSet.AudioFileLinguisticLevel Then

            SmcsWithSoundFile.Add(Me)

        ElseIf Me.LinguisticLevel > MediaSet.AudioFileLinguisticLevel Then

            'E.g. SMC is a Word and AudioFileLinguisticLevel is a Sentence
            SmcsWithSoundFile.Add(GetAncestorAtLevel(MediaSet.AudioFileLinguisticLevel))

        ElseIf Me.LinguisticLevel < MediaSet.AudioFileLinguisticLevel Then

            'E.g. SMC is a List and AudioFileLinguisticLevel is a sentence
            SmcsWithSoundFile.AddRange(GetAllDescenentsAtLevel(MediaSet.AudioFileLinguisticLevel))

        End If

        'Removing practise components
        If IncludePractiseComponents = False Then
            Dim TempList As New List(Of SpeechMaterialComponent)
            For Each Component In SmcsWithSoundFile
                If Component.IsPractiseComponent = False Then
                    TempList.Add(Component)
                End If
            Next
            SmcsWithSoundFile = TempList
        End If

        If UniquePrimaryStringRepresenations = True Then
            Dim TempList As New List(Of SpeechMaterialComponent)
            Dim Included_PrimaryStringRepresenations As New SortedSet(Of String)

            For Each Component In SmcsWithSoundFile
                If Not Included_PrimaryStringRepresenations.Contains(Component.PrimaryStringRepresentation) Then
                    Included_PrimaryStringRepresenations.Add(Component.PrimaryStringRepresentation)
                    TempList.Add(Component)
                End If
            Next

            SmcsWithSoundFile = TempList
        End If

        Dim Output As New List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)

        For Each SmcWithSoundFile In SmcsWithSoundFile

            Dim SoundPath = SmcWithSoundFile.GetSoundPath(MediaSet, Index)
            Dim SoundFileObject As Audio.Sound = SmcWithSoundFile.GetSoundFile(SoundPath)
            Dim CurrentSmaComponent = SoundFileObject.SMA.GetSmaComponentByIndexSeries(SelfIndices, MediaSet.AudioFileLinguisticLevel, SoundChannel)
            Output.Add(CurrentSmaComponent)

            'Stores / updates the file paths from which the SMA was read
            CurrentSmaComponent.SourceFilePath = SoundPath

        Next

        Return Output

    End Function



    Public Function GetBackgroundNonspeechPath(ByRef MediaSet As MediaSet, ByVal Index As Integer) As String

        Dim CurrentTestRootPath As String = ParentTestSpecification.GetTestRootPath
        Dim FolderPath = IO.Path.Combine(CurrentTestRootPath, MediaSet.BackgroundNonspeechParentFolder)

        Dim AvailablePaths = GetAvailableFiles(FolderPath)

        If Index > AvailablePaths.Count - 1 Then
            Return Nothing
        Else
            Return AvailablePaths(Index)
        End If

    End Function

    Public Function GetBackgroundSpeechPath(ByRef MediaSet As MediaSet, ByVal Index As Integer) As String

        Dim CurrentTestRootPath As String = ParentTestSpecification.GetTestRootPath
        Dim FolderPath = IO.Path.Combine(CurrentTestRootPath, MediaSet.BackgroundSpeechParentFolder)

        Dim AvailablePaths = GetAvailableFiles(FolderPath)

        If Index > AvailablePaths.Count - 1 Then
            Return Nothing
        Else
            Return AvailablePaths(Index)
        End If

    End Function


    Public Function GetMaskerPath(ByRef MediaSet As MediaSet, ByVal Index As Integer, Optional ByVal SearchAncestors As Boolean = True) As String

        If MediaSet.MaskerAudioItems = 0 And SearchAncestors = True Then

            If ParentComponent IsNot Nothing Then
                Return ParentComponent.GetMaskerPath(MediaSet, Index, SearchAncestors)
            Else
                Return ""
            End If

        Else

            If Index > MediaSet.MaskerAudioItems - 1 Then
                Throw New ArgumentException("Requested (zero-based) sound index (" & Index & " ) is higher than the number of available masker sound recordings of the current speech material component (" & Me.PrimaryStringRepresentation & ").")
            End If

            Dim CurrentTestRootPath As String = ParentTestSpecification.GetTestRootPath

            Dim SharedMaskersLevelComponent As SpeechMaterialComponent = Nothing
            If Me.LinguisticLevel = MediaSet.SharedMaskersLevel Then
                SharedMaskersLevelComponent = Me
            ElseIf Me.LinguisticLevel > MediaSet.SharedMaskersLevel Then
                SharedMaskersLevelComponent = Me.GetAncestorAtLevel(MediaSet.SharedMaskersLevel)
            Else
                Throw New Exception("Unable to locate the masker files!")
            End If

            Dim FullMaskerFolderPath = IO.Path.Combine(CurrentTestRootPath, MediaSet.MaskerParentFolder, SharedMaskersLevelComponent.GetMaskerFolderName())

            Return GetAvailableFiles(FullMaskerFolderPath)(Index)

        End If

    End Function


    Public Function GetContralateralMaskerPath(ByRef MediaSet As MediaSet, ByVal Index As Integer, Optional ByVal SearchAncestors As Boolean = True) As String

        If MediaSet.ContralateralMaskerAudioItems = 0 And SearchAncestors = True Then

            If ParentComponent IsNot Nothing Then
                Return ParentComponent.GetContralateralMaskerPath(MediaSet, Index, SearchAncestors)
            Else
                Return ""
            End If

        Else

            If Index > MediaSet.ContralateralMaskerAudioItems - 1 Then
                Throw New ArgumentException("Requested (zero-based) sound index (" & Index & " ) is higher than the number of available contralateral masker sound recordings of the current speech material component (" & Me.PrimaryStringRepresentation & ").")
            End If

            Dim CurrentTestRootPath As String = ParentTestSpecification.GetTestRootPath

            Dim SharedContralateralMaskersLevelComponent As SpeechMaterialComponent = Nothing
            If Me.LinguisticLevel = MediaSet.SharedContralateralMaskersLevel Then
                SharedContralateralMaskersLevelComponent = Me
            ElseIf Me.LinguisticLevel > MediaSet.SharedContralateralMaskersLevel Then
                SharedContralateralMaskersLevelComponent = Me.GetAncestorAtLevel(MediaSet.SharedContralateralMaskersLevel)
            Else
                Throw New Exception("Unable to locate the contralateral masker files!")
            End If

            Dim FullContralateralMaskerFolderPath = IO.Path.Combine(CurrentTestRootPath, MediaSet.ContralateralMaskerParentFolder, SharedContralateralMaskersLevelComponent.GetContralateralMaskerFolderName())

            Return GetAvailableFiles(FullContralateralMaskerFolderPath)(Index)

        End If

    End Function

    Public Function GetCalibrationSignalPath(ByRef MediaSet As MediaSet, ByVal Index As Integer) As String

        Dim CurrentTestRootPath As String = ParentTestSpecification.GetTestRootPath
        Dim FullCalibrationSignalParentFolderPath = IO.Path.Combine(CurrentTestRootPath, MediaSet.CalibrationSignalParentFolder)
        Dim AvailableFiles = GetAvailableFiles(FullCalibrationSignalParentFolderPath)

        If AvailableFiles.Count = 0 Then
            'Returning an empty string if no calibration sound file was found
            Return ""
        End If

        If Index > AvailableFiles.Count - 1 Then
            'Returning the last file by default, if the index is too high
            Return AvailableFiles.Last
        Else
            'Returning the requested index
            Return AvailableFiles(Index)
        End If

    End Function


    ''' <summary>
    ''' Returns the calibration sound used with the current speech material component and media set as a new Audio.Sound. 
    ''' </summary>
    ''' <param name="Index"></param>
    ''' <returns></returns>
    Public Function GetCalibrationSignalSound(ByRef MediaSet As MediaSet, ByVal Index As Integer) As Audio.Sound

        Dim SoundPath = GetCalibrationSignalPath(MediaSet, Index)

        Return GetSoundFile(SoundPath)

    End Function

    Public Function GetSoundPath(ByRef MediaSet As MediaSet, ByVal Index As Integer, Optional ByVal SearchAncestors As Boolean = True) As String


        If MediaSet.MediaAudioItems = 0 And SearchAncestors = True Then

            If ParentComponent IsNot Nothing Then
                Return ParentComponent.GetSoundPath(MediaSet, Index, SearchAncestors)
            Else
                Return ""
            End If

        Else

            If Index > MediaSet.MediaAudioItems - 1 Then
                Throw New ArgumentException("Requested (zero-based) sound index (" & Index & " ) is higher than the number of available sound recordings of the current speech material component (" & Me.PrimaryStringRepresentation & ").")
            End If

            Dim CurrentTestRootPath As String = ParentTestSpecification.GetTestRootPath
            Dim FullMediaFolderPath = IO.Path.Combine(CurrentTestRootPath, MediaSet.MediaParentFolder, GetMediaFolderName)

            Return GetAvailableFiles(FullMediaFolderPath)(Index)

        End If

    End Function


    Private SoundFileLock As New Object()

    Public Function GetSoundFile(ByVal Path As String) As Audio.Sound

        Select Case AudioFileLoadMode
            Case MediaFileLoadModes.LoadEveryTime
                Return Audio.Sound.LoadWaveFile(Path)

            Case MediaFileLoadModes.LoadOnFirstUse

                SyncLock SoundFileLock

                    'Enterring a synclock since multiple threads may call this method very close in time, which will cause an exception if it takes longer to load the file than the time between the calls (since an attempt will be mande to add it twice to the SoundLibrary)
                    If SoundLibrary.ContainsKey(Path) Then
                        Return SoundLibrary(Path)
                    Else
                        Dim NewSound As Audio.Sound = Audio.Sound.LoadWaveFile(Path)
                        SoundLibrary.Add(Path, NewSound)
                        Return SoundLibrary(Path)
                    End If

                End SyncLock

            Case Else
                Throw New NotImplementedException
        End Select

    End Function


    Public Shared Sub ClearAllLoadedSounds()
        SoundLibrary.Clear()
    End Sub



    ''' <summary>
    ''' Searches first among the numeric variable types and then among the categorical for the indicated VariableName. If found, returns the value. 
    ''' The calling codes need to parse the value as it is returned as an object. If the variable type is known, it is better to use either GetNumericVariableValue or GetCategoricalVariableValue instead.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <returns></returns>
    Public Function GetVariableValue(ByVal VariableName As String) As Object

        'Looks first among the numeric metrics
        If NumericVariables.Keys.Contains(VariableName) Then
            Return NumericVariables(VariableName)
        End If

        'If not found, looks among the categorical metrics
        If CategoricalVariables.Keys.Contains(VariableName) Then
            Return CategoricalVariables(VariableName)
        End If

        Return Nothing

    End Function



    ''' <summary>
    ''' Returns all variable names at any component at the indicated Linguistic level and their type (as a boolean IsNumeric), and checks that no variable name is used as both numeric and categorical.
    ''' </summary>
    ''' <param name="LinguisticLevel"></param>
    ''' <returns></returns>
    Public Function GetCustomVariableNameAndTypes(ByVal LinguisticLevel As SpeechMaterialComponent.LinguisticLevels) As SortedList(Of String, Boolean)

        Dim AllCustomVariables As New SortedList(Of String, Boolean) ' Variable name, IsNumeric
        Dim AllTargetLevelComponents = GetAllRelativesAtLevel(LinguisticLevel)

        For Each Component In AllTargetLevelComponents

            Dim CategoricalVariableNames = Component.GetCategoricalVariableNames
            Dim NumericVariableNames = Component.GetNumericVariableNames

            For Each CatName In CategoricalVariableNames

                'Checking that the variable name is not used as both numeric and categorical.
                If NumericVariableNames.Contains(CatName) Then
                    MsgBox("The variable name " & CatName & " exist as both categorical and numeric at the linguistic level " & LinguisticLevel &
                               " in the speech material component id: " & Component.Id & " ( " & Component.PrimaryStringRepresentation & " ). This is not allowed!", MsgBoxStyle.Information, "Getting custom variable names and types")
                    Return Nothing
                End If

                'Stores the variable name, and the IsNumeric value of False
                If AllCustomVariables.ContainsKey(CatName) = False Then AllCustomVariables.Add(CatName, False)
            Next

            For Each CatName In NumericVariableNames

                'Checking that the variable name is not used as both numeric and categorical.
                If CategoricalVariableNames.Contains(CatName) Then
                    MsgBox("The variable name " & CatName & " exist as both numeric and categorical at the linguistic level " & LinguisticLevel &
                               " in the speech material component id: " & Component.Id & " ( " & Component.PrimaryStringRepresentation & " ). This is not allowed!", MsgBoxStyle.Information, "Getting custom variable names and types")
                    Return Nothing
                End If

                'Stores the variable name, and the IsNumeric value of False
                If AllCustomVariables.ContainsKey(CatName) = False Then AllCustomVariables.Add(CatName, True)
            Next

        Next

        Return AllCustomVariables

    End Function

    ''' <summary>
    ''' Searches among the numeric variable types for the indicated VariableName. If found returns word metric value, otherwise returns Nothing.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <returns></returns>
    Public Function GetNumericVariableValue(ByVal VariableName As String) As Double?

        If NumericVariables.Keys.Contains(VariableName) Then
            Return NumericVariables(VariableName)
        End If

        Return Nothing

    End Function

    ''' <summary>
    ''' Searches among the categorical variable types for the indicated VariableName. If found returns the value stored as a string, otherwise an empty string is returned.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <returns></returns>
    Public Function GetCategoricalVariableValue(ByVal VariableName As String) As String

        If CategoricalVariables.Keys.Contains(VariableName) Then
            Return CategoricalVariables(VariableName)
        End If

        Return ""

    End Function

    ''' <summary>
    ''' Returns the names of all categorical custom variabels
    ''' </summary>
    ''' <returns></returns>
    Public Function GetCategoricalVariableNames() As List(Of String)
        Return CategoricalVariables.Keys.ToList
    End Function


    ''' <summary>
    ''' Returns the names of all numeric custom variabels
    ''' </summary>
    ''' <returns></returns>
    Public Function GetNumericVariableNames() As List(Of String)
        Return NumericVariables.Keys.ToList
    End Function

    ''' <summary>
    ''' Returns all numeric custom variable names separately at different linguistic levels for the current instance of SpeechMaterial and all its descendants.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetNumericCustomVariableNamesByLinguicticLevel() As SortedList(Of SpeechMaterialComponent.LinguisticLevels, SortedSet(Of String))

        Dim Output As New SortedList(Of SpeechMaterialComponent.LinguisticLevels, SortedSet(Of String))
        Dim TargetComponents = GetAllDescenents()
        'Also adding me
        TargetComponents.Add(Me)
        For Each Component In TargetComponents
            If Output.ContainsKey(Component.LinguisticLevel) = False Then Output.Add(Component.LinguisticLevel, New SortedSet(Of String))
            For Each VarNam In Component.GetNumericVariableNames
                If Output(Component.LinguisticLevel).Contains(VarNam) = False Then Output(Component.LinguisticLevel).Add(VarNam)
            Next
        Next
        Return Output

    End Function

    Public Function GetCategoricalCustomVariableNamesByLinguicticLevel() As SortedList(Of SpeechMaterialComponent.LinguisticLevels, SortedSet(Of String))

        Dim Output As New SortedList(Of SpeechMaterialComponent.LinguisticLevels, SortedSet(Of String))
        Dim TargetComponents = GetAllDescenents()
        'Also adding me
        TargetComponents.Add(Me)
        For Each Component In TargetComponents
            If Output.ContainsKey(Component.LinguisticLevel) = False Then Output.Add(Component.LinguisticLevel, New SortedSet(Of String))
            For Each VarNam In Component.GetCategoricalVariableNames
                If Output(Component.LinguisticLevel).Contains(VarNam) = False Then Output(Component.LinguisticLevel).Add(VarNam)
            Next
        Next
        Return Output

    End Function


    ''' <summary>
    ''' Adds the indicated Value to the indicated VariableName in the collection of CategoricalVariables. Adds the variable name if not already present.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <param name="Value"></param>
    Public Sub SetNumericVariableValue(ByVal VariableName As String, ByVal Value As Double)
        If NumericVariables.Keys.Contains(VariableName) = True Then
            NumericVariables(VariableName) = Value
        Else
            NumericVariables.Add(VariableName, Value)
        End If
    End Sub


    ''' <summary>
    ''' Adds the indicated Value to the indicated VariableName in the collection of CategoricalVariables. Adds the variable name if not already present.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <param name="Value"></param>
    Public Sub SetCategoricalVariableValue(ByVal VariableName As String, ByVal Value As String)
        If CategoricalVariables.Keys.Contains(VariableName) = True Then
            CategoricalVariables(VariableName) = Value
        Else
            CategoricalVariables.Add(VariableName, Value)
        End If
    End Sub


    ''' <summary>
    ''' Searches first among the numeric media set variable types and then among the categorical for the indicated VariableName. If found, returns the value. 
    ''' The calling codes need to parse the value as it is returned as an object. If the variable type is known, it is better to use either GetNumericMediaSetVariableValue or GetCategoricalMediaSetVariableValue instead.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <returns></returns>
    Public Function GetMediaSetVariableValue(ByRef MediaSet As MediaSet, ByVal VariableName As String) As Object

        'Looks first among the numeric metrics
        If MediaSet.NumericVariables.Keys.Contains(Me.Id) Then
            If MediaSet.NumericVariables(Me.Id).Keys.Contains(VariableName) Then
                Return MediaSet.NumericVariables(Me.Id)(VariableName)
            End If
        End If

        'If not found, looks among the categorical metrics
        If MediaSet.CategoricalVariables.Keys.Contains(Me.Id) Then
            If MediaSet.CategoricalVariables(Me.Id).Keys.Contains(VariableName) Then
                Return MediaSet.CategoricalVariables(Me.Id)(VariableName)
            End If
        End If

        Return Nothing

    End Function

    ''' <summary>
    ''' Searches among the numeric media set variable types for the indicated VariableName. If found returns word metric value, otherwise returns Nothing.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <returns></returns>
    Public Function GetNumericMediaSetVariableValue(ByRef MediaSet As MediaSet, ByVal VariableName As String) As Double?

        If MediaSet.NumericVariables.Keys.Contains(Me.Id) Then
            If MediaSet.NumericVariables(Me.Id).Keys.Contains(VariableName) Then
                Return MediaSet.NumericVariables(Me.Id)(VariableName)
            End If
        End If

        Return Nothing

    End Function

    ''' <summary>
    ''' Searches among the categorical media set variable types for the indicated VariableName. If found returns word metric value, otherwise returns Nothing.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <returns></returns>
    Public Function GetCategoricalMediaSetVariableValue(ByRef MediaSet As MediaSet, ByVal VariableName As String) As String

        If MediaSet.CategoricalVariables.Keys.Contains(Me.Id) Then
            If MediaSet.CategoricalVariables(Me.Id).Keys.Contains(VariableName) Then
                Return MediaSet.CategoricalVariables(Me.Id)(VariableName)
            End If
        End If

        Return Nothing

    End Function

    ''' <summary>
    ''' Returns the names of all numeric custom media set variabels
    ''' </summary>
    ''' <returns></returns>
    Public Function GetNumericMediaSetVariableNames(ByRef MediaSet As MediaSet) As List(Of String)

        If MediaSet.NumericVariables.Keys.Contains(Me.Id) Then
            Return MediaSet.NumericVariables(Me.Id).Keys.ToList
        End If

        Return New List(Of String)

    End Function


    ''' <summary>
    ''' Returns the names of all numeric custom media set variabels
    ''' </summary>
    ''' <returns></returns>
    Public Function GetCategoricalMediaSetVariableNames(ByRef MediaSet As MediaSet) As List(Of String)

        If MediaSet.CategoricalVariables.Keys.Contains(Me.Id) Then
            Return MediaSet.CategoricalVariables(Me.Id).Keys.ToList
        End If

        Return New List(Of String)

    End Function


    ''' <summary>
    ''' Adds the indicated Value to the indicated VariableName in the collection of media set NumericVariables. Adds the variable name if not already present.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <param name="Value"></param>
    Public Sub SetNumericMediaSetVariableValue(ByRef MediaSet As MediaSet, ByVal VariableName As String, ByVal Value As Double)

        If MediaSet.NumericVariables.Keys.Contains(Me.Id) = False Then
            MediaSet.NumericVariables.Add(Me.Id, New SortedList(Of String, Double))
        End If

        If MediaSet.NumericVariables(Me.Id).Keys.Contains(VariableName) = True Then
            MediaSet.NumericVariables(Me.Id)(VariableName) = Value
        Else
            MediaSet.NumericVariables(Me.Id).Add(VariableName, Value)
        End If

    End Sub

    ''' <summary>
    ''' Adds the indicated Value to the indicated VariableName in the collection of media set CategoricalVariables. Adds the variable name if not already present.
    ''' </summary>
    ''' <param name="VariableName"></param>
    ''' <param name="Value"></param>
    Public Sub SetCategoricalMediaSetVariableValue(ByRef MediaSet As MediaSet, ByVal VariableName As String, ByVal Value As String)

        If MediaSet.CategoricalVariables.Keys.Contains(Me.Id) = False Then
            MediaSet.CategoricalVariables.Add(Me.Id, New SortedList(Of String, String))
        End If

        If MediaSet.CategoricalVariables(Me.Id).Keys.Contains(VariableName) = True Then
            MediaSet.CategoricalVariables(Me.Id)(VariableName) = Value
        Else
            MediaSet.CategoricalVariables(Me.Id).Add(VariableName, Value)
        End If
    End Sub


    Public Function GetChildren() As List(Of SpeechMaterialComponent)
        Return ChildComponents
    End Function

    Public Function GetParent() As List(Of SpeechMaterialComponent)
        If ParentComponent IsNot Nothing Then
            Dim OutputList As New List(Of SpeechMaterialComponent)
            OutputList.Add(ParentComponent)
            Return OutputList
        Else
            Return Nothing
        End If
    End Function

    Public Function GetSiblings() As List(Of SpeechMaterialComponent)
        If ParentComponent IsNot Nothing Then
            Return ParentComponent.GetChildren
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Figures out and returns at what index in the parent component the component itself is stored, or Nothing if there is no parent compoment, or if (for some unexpected reason) unable to establish the index.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSelfIndex() As Integer?

        If ParentComponent Is Nothing Then
            'If Me.LinguisticLevel = LinguisticLevels.ListCollection Then
            '    Return 0
            'Else
            Return Nothing
            'End If
        End If

        Dim Siblings = GetSiblings()
        For s = 0 To Siblings.Count - 1
            If Siblings(s) Is Me Then Return s
        Next

        Return Nothing

    End Function



    Public Function GetDescendantAtIndexSeries(ByVal HierachicalSelftIndices As SortedList(Of SpeechMaterialComponent.LinguisticLevels, Integer))


        If HierachicalSelftIndices(Me.LinguisticLevel) > ChildComponents.Count - 1 Then

            'Returns Nothing if there is no component at the specified index
            Return Nothing
        Else


            If HierachicalSelftIndices.Keys.Max = Me.LinguisticLevel Then

                Return ChildComponents(HierachicalSelftIndices(Me.LinguisticLevel))

            Else

                Return GetDescendantAtIndexSeries(HierachicalSelftIndices)

            End If

        End If

    End Function


    Public Function GetParentOfFirstNonSequentialAncestorWithSiblings() As SpeechMaterialComponent

        'Returns the parent component of the first detected anscestor component which is both non-sequential and has siblings

        If ParentComponent Is Nothing Then

            'Returns nothing if there is no parent
            Return Nothing

        Else

            If Me.IsSequentiallyOrdered = True Then

                'Sequentially ordered, calling the parent instead
                Return ParentComponent.GetParentOfFirstNonSequentialAncestorWithSiblings

            Else

                'Not sequentially ordered

                'Determines if there are siblings
                If ParentComponent.ChildComponents.Count > 1 Then

                    'Returns the parent component
                    Return ParentComponent
                Else

                    'Calling the parent instead
                    Return ParentComponent.GetParentOfFirstNonSequentialAncestorWithSiblings

                End If
            End If
        End If

    End Function

    Public Function FindDescendantIndexSerie(ByRef TargetDescendant As SpeechMaterialComponent, Optional ByRef IndexList As List(Of Integer) = Nothing) As List(Of Integer)

        If IndexList Is Nothing Then IndexList = New List(Of Integer)

        'Determines if the TargetDescendant is a descendant
        Dim AllDescendants = Me.GetAllDescenents

        'Checks if the TargetDescendant is a descendant of me
        If AllDescendants.Contains(TargetDescendant) Then

            'If so, adds the self index of Me
            IndexList.Add(Me.GetSelfIndex)

            'Goes through each child and calls FindDescendantIndex recursicely on each child to determines the hiearachical index serie of the TargetDescendant
            For Each child In Me.ChildComponents
                child.FindDescendantIndexSerie(TargetDescendant, IndexList)
            Next

        End If

        'Adds the selfindex of the TargetDescendant if it is me.
        If TargetDescendant Is Me Then
            IndexList.Add(Me.GetSelfIndex)
            Return IndexList
        End If

        Return IndexList

    End Function

    Public Function GetDescendantByIndexSerie(ByVal IndexSerie As List(Of Integer), Optional ByVal i As Integer = 0) As SpeechMaterialComponent

        'Checks that does not go outside the IndexSerie array
        If i > IndexSerie.Count - 1 Then Return Nothing

        'Checks that IndexSerie(i) does not go beyond the lengths of Me.ChildComponents
        If IndexSerie(i) > Me.ChildComponents.Count - 1 Then Return Nothing

        'Determines if we're at the last index. If so, the child component at the last
        If i = IndexSerie.Count - 1 Then

            'Return the component
            Return Me.ChildComponents(IndexSerie(i))

        Else

            'Calls descendants recursively
            Return Me.ChildComponents(IndexSerie(i)).GetDescendantByIndexSerie(IndexSerie, i + 1)

        End If

    End Function

    ''' <summary>
    ''' Determines if a component contrasts to all other cousin components (given some restictions).
    ''' </summary>
    ''' <param name="ComparisonVariableName"></param>
    ''' <param name="NumberOfContrasts">Returns the number of contrasting component (including the component itself), given that the returns value is True.</param>
    ''' <param name="ContrastingComponents">If an initialized object is supplied by the calling code, the actual contrasting components (including the component itself) are returned. </param>
    ''' <returns></returns>
    Public Function IsContrastingComponent(Optional ByVal ComparisonVariableName As String = "Transcription",
                                           Optional ByRef NumberOfContrasts As Integer? = Nothing, Optional ByRef ContrastingComponents As List(Of SpeechMaterialComponent) = Nothing) As Boolean

        'Gets the ancestor component at the level from which the data is supposed to be compared
        Dim ViewPointComponent = Me.GetParentOfFirstNonSequentialAncestorWithSiblings()

        'Returns false if there is no component at the level from which the data is supposed to be compared
        If ViewPointComponent Is Nothing Then
            Return False
        End If

        Dim MyIndexSeries = ViewPointComponent.FindDescendantIndexSerie(Me)

        'Returns false if no indices were found (i.e. nothing to compare with)
        If MyIndexSeries.Count = 0 Then Return False

        'Removes the first index from MyIndexSeries as it refers to the self index of ViewPointComponent
        If MyIndexSeries.Count > 0 Then
            MyIndexSeries.RemoveAt(0)
        End If

        'Returns false if no indices were found (i.e. nothing to compare with)
        If MyIndexSeries.Count = 0 Then Return False

        Dim ComparisonCousins As New List(Of SpeechMaterialComponent)
        For c = 0 To ViewPointComponent.ChildComponents.Count - 1

            'Adjusting the first index to get all different comparison components
            MyIndexSeries(0) = c

            'Getting the component
            ComparisonCousins.Add(ViewPointComponent.GetDescendantByIndexSerie(MyIndexSeries))

        Next

        'Comparing the components
        Dim OnlyContrastingComponents = ContainsOnlyContrastingComponents(ComparisonCousins, ComparisonVariableName)

        If OnlyContrastingComponents = True Then
            NumberOfContrasts = ComparisonCousins.Count
        End If

        If OnlyContrastingComponents = True Then
            If ContrastingComponents IsNot Nothing Then
                For Each ComparisonCousin In ComparisonCousins
                    ContrastingComponents.Add(ComparisonCousin)
                Next
            End If
        End If


        Return OnlyContrastingComponents

    End Function


    Private Shared Function ContainsOnlyContrastingComponents(ByRef ComparisonList As List(Of SpeechMaterialComponent),
                                           Optional ByVal ComparisonVariableName As String = "Transcription") As Boolean

        For i = 0 To ComparisonList.Count - 1
            For j = 0 To ComparisonList.Count - 1

                'Skips comparison when i = j 
                If i = j Then Continue For

                If ComparisonList(i).IsEqualComponent_ByCategoricalVariableValue(ComparisonList(j), ComparisonVariableName) = True Then
                    Return False
                End If
            Next
        Next

        Return True

    End Function

    Public Function IsEqualComponent_ByCategoricalVariableValue(ByRef ComparisonComponent As SpeechMaterialComponent,
                                     ByVal ComparisonVariableName As String) As Boolean

        If Me.CategoricalVariables.ContainsKey(ComparisonVariableName) And ComparisonComponent.CategoricalVariables.ContainsKey(ComparisonVariableName) Then
            If Me.GetCategoricalVariableValue(ComparisonVariableName) = ComparisonComponent.GetCategoricalVariableValue(ComparisonVariableName) Then
                Return True
            Else
                Return False
            End If
        Else
            Throw New Exception("Unable to compare speech material components " & Me.PrimaryStringRepresentation & " " & ComparisonComponent.PrimaryStringRepresentation & " since the variable named " & ComparisonVariableName & " must exist for both components.")
        End If

    End Function


    Public Function GetSiblingsExcludingSelf() As List(Of SpeechMaterialComponent)
        Dim OutputList As New List(Of SpeechMaterialComponent)
        If ParentComponent IsNot Nothing Then
            For Each child In ParentComponent.ChildComponents
                If child IsNot Me Then
                    OutputList.Add(child)
                End If
            Next
        End If
        Return OutputList
    End Function

    ''' <summary>
    ''' Recursively searches for the SpeechMaterial at the top of the heirachy
    ''' </summary>
    ''' <returns></returns>
    Public Function GetToplevelAncestor() As SpeechMaterialComponent
        If ParentComponent IsNot Nothing Then
            Return ParentComponent.GetToplevelAncestor
        Else
            Return Me
        End If
    End Function

    Public Function GetAllRelatives() As List(Of SpeechMaterialComponent)
        'Creates a list
        Dim OutputList As New List(Of SpeechMaterialComponent)

        'Gets the top level ancestor
        Dim TopLevelAncestor = GetToplevelAncestor()

        'Adds the top level ancestor
        OutputList.Add(TopLevelAncestor)

        'Adds all descendants to the top level ancestor
        TopLevelAncestor.AddDescendents(OutputList)

        Return OutputList
    End Function

    Public Function GetAllRelativesAtLevel(ByVal Level As SpeechMaterialComponent.LinguisticLevels,
                                           Optional ByVal ExcludePractiseComponents As Boolean = False,
                                           Optional ByVal ExcludeTestComponents As Boolean = False) As List(Of SpeechMaterialComponent)
        'Creates a list
        Dim OutputList As New List(Of SpeechMaterialComponent)
        Dim AllRelatives = GetAllRelatives()
        For Each Component In AllRelatives

            If ExcludePractiseComponents = True Then
                If Component.IsPractiseComponent = True Then Continue For
            End If

            If ExcludeTestComponents = True Then
                If Component.IsPractiseComponent = False Then Continue For
            End If

            If Component.LinguisticLevel = Level Then OutputList.Add(Component)
        Next
        Return OutputList
    End Function


    ''' <summary>
    ''' Recursively adds all descentents to the DescendentsList
    ''' </summary>
    ''' <param name="DescendentsList"></param>
    Private Sub AddDescendents(ByRef DescendentsList As List(Of SpeechMaterialComponent))

        For Each child In ChildComponents
            DescendentsList.Add(child)
            child.AddDescendents(DescendentsList)
        Next

    End Sub

    Public Function GetAllRelativesExludingSelf() As List(Of SpeechMaterialComponent)

        Dim RelativesList = GetAllRelatives()
        Dim OutputList As New List(Of SpeechMaterialComponent)
        For Each item In RelativesList
            If item IsNot Me Then
                OutputList.Add(item)
            End If
        Next
        Return OutputList

    End Function

    Public Function GetAllRelativesAtLevelExludingSelf(ByVal Level As SpeechMaterialComponent.LinguisticLevels,
                                           Optional ByVal ExcludePractiseComponents As Boolean = False,
                                           Optional ByVal ExcludeTestComponents As Boolean = False) As List(Of SpeechMaterialComponent)

        Dim RelativesList = GetAllRelativesAtLevel(Level, ExcludePractiseComponents, ExcludeTestComponents)
        Dim OutputList As New List(Of SpeechMaterialComponent)
        For Each item In RelativesList
            If item IsNot Me Then
                OutputList.Add(item)
            End If
        Next
        Return OutputList

    End Function


    Public Shared Function LoadSpeechMaterial(ByVal SpeechMaterialComponentFilePath As String, ByVal TestRootPath As String) As SpeechMaterialComponent

        'Gets a file path from the user if none is supplied
        If SpeechMaterialComponentFilePath = "" Then SpeechMaterialComponentFilePath = Utils.GeneralIO.GetOpenFilePath(,, {".txt"}, "Please open a stuctured speech material component .txt file.")
        If SpeechMaterialComponentFilePath = "" Then
            MsgBox("No file selected!")
            Return Nothing
        End If

        'Creates a new random that will be references in all speech material components
        Dim rnd As New Random

        Dim Output As SpeechMaterialComponent = Nothing

        'Parses the input file
        Dim InputLines() As String = System.IO.File.ReadAllLines(InputFileSupport.InputFilePathValueParsing(SpeechMaterialComponentFilePath, TestRootPath, False), Text.Encoding.UTF8)

        Dim CustomVariablesDatabases As New SortedList(Of String, CustomVariablesDatabase)

        Dim IdsUsed As New SortedSet(Of String)

        Dim SequentiallyOrderedLists As Boolean = False
        Dim SequentiallyOrderedSentences As Boolean = False
        Dim SequentiallyOrderedWords As Boolean = True
        Dim SequentiallyOrderedPhonemes As Boolean = True
        Dim PresetLevel As LinguisticLevels = LinguisticLevels.List ' Using List as default, as this work with the SiP-test, but it should preferably be specified in the SpeechMaterialComponents.txt file

        Dim PresetSpecifications As New List(Of Tuple(Of String, Boolean, List(Of String))) 'Preset name, IsContrasting, List Of PrimaryStringRepresentation

        For Each Line In InputLines

            'Skipping blank lines
            If Line.Trim = "" Then Continue For

            'Also skipping commentary only lines 
            If Line.Trim.StartsWith("//") Then Continue For

            'Checking for and reading setup commands
            If Line.Trim.StartsWith("SequentiallyOrderedLists") Then
                SequentiallyOrderedLists = InputFileSupport.InputFileBooleanValueParsing(Line, True, SpeechMaterialComponentFilePath)
                Continue For
            ElseIf Line.Trim.StartsWith("SequentiallyOrderedSentences") Then
                SequentiallyOrderedSentences = InputFileSupport.InputFileBooleanValueParsing(Line, True, SpeechMaterialComponentFilePath)
                Continue For
            ElseIf Line.Trim.StartsWith("SequentiallyOrderedWords") Then
                SequentiallyOrderedWords = InputFileSupport.InputFileBooleanValueParsing(Line, True, SpeechMaterialComponentFilePath)
                Continue For
            ElseIf Line.Trim.StartsWith("SequentiallyOrderedPhonemes") Then
                SequentiallyOrderedPhonemes = InputFileSupport.InputFileBooleanValueParsing(Line, True, SpeechMaterialComponentFilePath)
                Continue For
            ElseIf Line.Trim.StartsWith("PresetLevel") Then
                Dim TempPresetLevel = InputFileSupport.InputFileEnumValueParsing(Line, GetType(LinguisticLevels), SpeechMaterialComponentFilePath, True)
                If TempPresetLevel IsNot Nothing Then
                    PresetLevel = TempPresetLevel
                End If
                Continue For
            ElseIf Line.Trim.StartsWith("Preset ") Or Line.Trim.StartsWith("Preset=") Then ' The two alternatives here exist in order to distinguish the key 'Preset' from 'PresetLevel'
                'Parsing and adding the preset
                Dim PresetData = InputFileSupport.GetInputFileValue(Line, True)
                Dim PresetDataSplit = PresetData.Split(":")
                Dim PresetKey As String = PresetDataSplit(0).Trim
                If PresetDataSplit.Length > 1 Then
                    Dim PresetList = InputFileSupport.InputFileListOfStringParsing(PresetDataSplit(1).Trim, False, False)
                    PresetSpecifications.Add(New Tuple(Of String, Boolean, List(Of String))(PresetKey, False, PresetList))
                End If
                Continue For
            ElseIf Line.Trim.StartsWith("ContrastPreset") Then
                'Parsing and adding the preset
                Dim PresetData = InputFileSupport.GetInputFileValue(Line, True)
                Dim PresetDataSplit = PresetData.Split(":")
                Dim PresetKey As String = PresetDataSplit(0).Trim
                If PresetDataSplit.Length > 1 Then
                    Dim PresetList = InputFileSupport.InputFileListOfStringParsing(PresetDataSplit(1).Trim, False, False)
                    PresetSpecifications.Add(New Tuple(Of String, Boolean, List(Of String))(PresetKey, True, PresetList))
                End If
                Continue For
            End If


            'Reading components
            Dim SplitRow = Line.Split(vbTab)

            If SplitRow.Length < 5 Then Throw New ArgumentException("Not enough data columns in the file " & SpeechMaterialComponentFilePath & vbCrLf & "At the line: " & Line)

            Dim NewComponent As New SpeechMaterialComponent(rnd)

            'Adds component data
            Dim index As Integer = 0

            'Linguistic Level
            Dim LinguisticLevel = InputFileSupport.InputFileEnumValueParsing(SplitRow(index), GetType(LinguisticLevels), SpeechMaterialComponentFilePath, False)
            If LinguisticLevel IsNot Nothing Then
                NewComponent.LinguisticLevel = LinguisticLevel
            Else
                Throw New Exception("Missing value for LinguisticLevel detected in the speech material file. A value for LinguisticLevel is obligatory for all speech material components. Line: " & vbCrLf & Line & vbCrLf &
                                        "Possible values are:" & vbCrLf & String.Join(" ", [Enum].GetNames(GetType(LinguisticLevels))))
            End If
            index += 1

            NewComponent.Id = InputFileSupport.GetInputFileValue(SplitRow(index), False)
            index += 1

            'Checking that the Id is not already used (Ids can only be used once throughout all speech component levels!!!)
            If IdsUsed.Contains(NewComponent.Id) Then
                Throw New ArgumentException("Re-used Id (" & NewComponent.Id & ")! Speech material components must only be used once throughout the whole speech material!")
            Else
                'Adding the Id to IdsUsed
                IdsUsed.Add(NewComponent.Id)
            End If

            ' Reading ParentId (which is used below
            Dim ParentId As String = InputFileSupport.GetInputFileValue(SplitRow(index), False)
            index += 1

            ' PrimaryStringRepresentation
            NewComponent.PrimaryStringRepresentation = InputFileSupport.GetInputFileValue(SplitRow(index), False)
            index += 1

            ' Getting the custom variables path

            Dim CustomVariablesDatabaseSubPath As String = SpeechMaterialComponent.GetDatabaseFileName(NewComponent.LinguisticLevel)
            Dim CustomVariablesDatabasePath As String = IO.Path.Combine(TestRootPath, SpeechMaterialComponent.SpeechMaterialFolderName, CustomVariablesDatabaseSubPath)

            ' Adding the custom variables
            If CustomVariablesDatabaseSubPath.Trim <> "" Then
                If CustomVariablesDatabases.ContainsKey(CustomVariablesDatabasePath) = False Then
                    'Loading the database
                    Dim NewDatabase As New CustomVariablesDatabase
                    NewDatabase.LoadTabDelimitedFile(CustomVariablesDatabasePath)
                    CustomVariablesDatabases.Add(CustomVariablesDatabasePath, NewDatabase)
                End If

                'Adding the variables
                For n = 0 To CustomVariablesDatabases(CustomVariablesDatabasePath).CustomVariableNames.Count - 1
                    Dim VariableName = CustomVariablesDatabases(CustomVariablesDatabasePath).CustomVariableNames(n)
                    Dim VariableValue = CustomVariablesDatabases(CustomVariablesDatabasePath).GetVariableValue(NewComponent.Id, VariableName)
                    'Stores the value only if it was not nothing.
                    If VariableValue IsNot Nothing Then
                        If CustomVariablesDatabases(CustomVariablesDatabasePath).CustomVariableTypes(n) = VariableTypes.Categorical Then
                            NewComponent.CategoricalVariables.Add(VariableName, VariableValue)
                        ElseIf CustomVariablesDatabases(CustomVariablesDatabasePath).CustomVariableTypes(n) = VariableTypes.Numeric Then
                            NewComponent.NumericVariables.Add(VariableName, VariableValue)
                        ElseIf CustomVariablesDatabases(CustomVariablesDatabasePath).CustomVariableTypes(n) = VariableTypes.Boolean Then
                            NewComponent.NumericVariables.Add(VariableName, VariableValue)
                        Else
                            Throw New NotImplementedException("Variable type not implemented!")
                        End If
                    End If
                Next
            End If

            'Adds further component data
            'Dim OrderedChildren = InputFileSupport.InputFileBooleanValueParsing(SplitRow(index), False, SpeechMaterialComponentFilePath)
            'If OrderedChildren IsNot Nothing Then NewComponent.OrderedChildren = OrderedChildren
            'index += 1

            Dim IsPractiseComponent = InputFileSupport.InputFileBooleanValueParsing(SplitRow(index), False, SpeechMaterialComponentFilePath)
            If IsPractiseComponent IsNot Nothing Then NewComponent.IsPractiseComponent = IsPractiseComponent
            index += 1

            ' The MediaFolder column has been removed and the same info is instead retrived from the MediaSet
            'NewComponent.GetMediaFolderName = InputFileSupport.InputFilePathValueParsing(SplitRow(index), TestRootPath, False)
            'index += 1

            ' The MaskerFolder column has been removed and the same info is instead retrived from the MediaSet
            'NewComponent.MaskerFolder = InputFileSupport.InputFilePathValueParsing(SplitRow(index), TestRootPath, False)
            'index += 1

            ' The BackgroundNonspeechFolder column has been removed and the same info is instead retrived from the MediaSet
            'NewComponent.BackgroundNonspeechFolder = InputFileSupport.InputFilePathValueParsing(SplitRow(index), TestRootPath, False)
            'index += 1

            ' The BackgroundSpeechFolder column has been removed and the same info is instead retrived from the MediaSet
            'NewComponent.BackgroundSpeechFolder = InputFileSupport.InputFilePathValueParsing(SplitRow(index), TestRootPath, False)
            'index += 1

            'Adds the component
            If Output Is Nothing Then
                Output = NewComponent
            Else
                If Output.AddComponent(NewComponent, ParentId) = False Then
                    Throw New ArgumentException("Failed to add speech material component defined by the following line in the file : " & SpeechMaterialComponentFilePath & vbCrLf & Line)
                End If
            End If

        Next

        'Storing the setup variables
        Output.SequentiallyOrderedLists = SequentiallyOrderedLists
        Output.SequentiallyOrderedSentences = SequentiallyOrderedSentences
        Output.SequentiallyOrderedWords = SequentiallyOrderedWords
        Output.SequentiallyOrderedPhonemes = SequentiallyOrderedPhonemes
        Output.PresetLevel = PresetLevel
        Output.PresetSpecifications = PresetSpecifications

        'Creating actual presets
        Output.InitializePresets()

        ''Writing the loaded data to UpdatedOutputFilePath if supplied and valid
        'If UpdatedOutputFilePath <> "" Then
        '    Output.WriteSpeechMaterialFile(UpdatedOutputFilePath)
        'End If

        Return Output

    End Function

    ''' <summary>
    ''' Creates actual presents based on the data in PresetSpecifications
    ''' </summary>
    Public Sub InitializePresets()

        Presets = New SmcPresets

        If PresetSpecifications.Count > 0 Then

            For Each PresetSpecification In PresetSpecifications
                Dim SelectedComponentsList As New SortedList(Of String, SpeechMaterialComponent) ' Where String is SpeechMaterial.Id

                Dim AllRelatives = GetAllRelatives()

                If PresetSpecification.Item3 IsNot Nothing Then

                    For Each Component In AllRelatives
                        'Ignoring the component if its at the wrong level
                        If PresetSpecification.Item3.Contains(Component.PrimaryStringRepresentation) Then

                            If PresetSpecification.Item2 = True Then
                                If Component.IsContrastingComponent = False Then Continue For
                            End If

                            'Getting related components at the PresetLevel 
                            Dim RelatedPresetLevelComponents = Component.GetSelfOrAncestorOrDescendentsAtLevel(PresetLevel)
                            If RelatedPresetLevelComponents IsNot Nothing Then

                                'Adding the PresetLevelComponent if not already added
                                For Each PresetLevelComponent In RelatedPresetLevelComponents
                                    If SelectedComponentsList.Keys.Contains(PresetLevelComponent.Id) = False Then
                                        SelectedComponentsList.Add(PresetLevelComponent.Id, PresetLevelComponent)
                                    End If
                                Next
                            End If
                        End If
                    Next

                Else

                    'In case the preset component list is empty, all components (except practise components) at the PresetLevel should be added
                    For Each Component In AllRelatives
                        'Skipping practise components
                        If Component.IsPractiseComponent = True Then
                            Continue For
                        End If

                        If Component.LinguisticLevel = PresetLevel Then
                            If SelectedComponentsList.Keys.Contains(Component.Id) = False Then
                                SelectedComponentsList.Add(Component.Id, Component)
                            End If
                        End If
                    Next

                End If

                Presets.Add(New SmcPresets.Preset With {.Name = PresetSpecification.Item1, .Members = SelectedComponentsList.Values.ToList})

            Next

        Else

            'If no presets have been defined, a default preset containing all components (except practise components) at the PresetLevel is added
            Dim SelectedComponentsList As New SortedList(Of String, SpeechMaterialComponent) ' Where String is SpeechMaterial.Id
            Dim AllRelatives = GetAllRelatives()

            For Each Component In AllRelatives
                'Skipping practise components
                If Component.IsPractiseComponent = True Then
                    Continue For
                End If

                If Component.LinguisticLevel = PresetLevel Then
                    If SelectedComponentsList.Keys.Contains(Component.Id) = False Then
                        SelectedComponentsList.Add(Component.Id, Component)
                    End If
                End If
            Next
            Presets.Add(New SmcPresets.Preset With {.Name = "All items", .Members = SelectedComponentsList.Values.ToList})
        End If

    End Sub

    Public Function GetComponentById(ByVal Id As String) As SpeechMaterialComponent

        Dim AllComponents = GetAllRelatives()
        For Each Component In AllComponents
            If Component.Id = Id Then Return Component
        Next

        'Returns nothing if the component was not found
        Return Nothing

    End Function

    Public Function AddComponent(ByRef NewComponent As SpeechMaterialComponent, ByVal ParentId As String) As Boolean

        Dim ParentComponent = GetComponentById(ParentId)
        If ParentComponent IsNot Nothing Then
            'Assigning the parent
            NewComponent.ParentComponent = ParentComponent
            'Storing the child
            ParentComponent.ChildComponents.Add(NewComponent)
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetAncestorAtLevel(ByVal RequestedParentComponentLevel As SpeechMaterialComponent.LinguisticLevels) As SpeechMaterialComponent

        If ParentComponent Is Nothing Then Return Nothing

        If ParentComponent.LinguisticLevel = RequestedParentComponentLevel Then
            Return ParentComponent
        Else
            Return ParentComponent.GetAncestorAtLevel(RequestedParentComponentLevel)
        End If

    End Function

    ''' <summary>
    ''' Gets all descendants at the specified linguistic level.
    ''' </summary>
    ''' <param name="RequestedDescendentComponentLevel">The specified linguistic level.</param>
    ''' <param name="IncludeSelf">Set to true, in order to also include the current instance of SpeechMaterial, in case its LinguisticLevel equals the specified linguistic level.</param>
    ''' <returns></returns>
    Public Function GetAllDescenentsAtLevel(ByVal RequestedDescendentComponentLevel As SpeechMaterialComponent.LinguisticLevels, Optional IncludeSelf As Boolean = False) As List(Of SpeechMaterialComponent)

        Dim OutputList As New List(Of SpeechMaterialComponent)

        If IncludeSelf = True Then
            If Me.LinguisticLevel = RequestedDescendentComponentLevel Then OutputList.Add(Me)
        End If

        For Each child In ChildComponents

            If child.LinguisticLevel = RequestedDescendentComponentLevel Then
                OutputList.Add(child)
            Else
                OutputList.AddRange(child.GetAllDescenentsAtLevel(RequestedDescendentComponentLevel, False))
            End If
        Next

        Return OutputList

    End Function

    ''' <summary>
    ''' Draws random descendents at the indicated level.
    ''' </summary>
    ''' <param name="Max">The maximum number of descendents to draw. If Max descentants do not exist, all available descendents will be drawn.</param>
    ''' <param name="RequestedDescendentComponentLevel"></param>
    ''' <param name="IncludeSelf"></param>
    ''' <param name="ExcludedDescendents">If supplied by the calling code, contains all descendents not drawn.</param>
    ''' <returns></returns>
    Public Function DrawRandomDescendentsAtLevel(ByVal Max As Integer, ByVal RequestedDescendentComponentLevel As SpeechMaterialComponent.LinguisticLevels,
                                                 Optional ByVal IncludeSelf As Boolean = False, Optional ByRef Randomizer As Random = Nothing,
                                                 Optional ByRef ExcludedDescendents As List(Of SpeechMaterialComponent) = Nothing) As List(Of SpeechMaterialComponent)

        If Randomizer Is Nothing Then
            Randomizer = Me.Randomizer
        End If

        Dim AllDescenents = GetAllDescenentsAtLevel(RequestedDescendentComponentLevel, IncludeSelf)

        Dim Output As New List(Of SpeechMaterialComponent)

        Dim n As Integer = Math.Min(AllDescenents.Count, Max)

        Dim RandomIndices = DSP.SampleWithoutReplacement(n, 0, AllDescenents.Count, Randomizer)
        For i = 0 To RandomIndices.Length - 1
            Output.Add(AllDescenents(RandomIndices(i)))
        Next

        If ExcludedDescendents IsNot Nothing Then
            'Adding the descendents not drawn
            For i = 0 To AllDescenents.Count - 1
                If RandomIndices.Contains(i) Then Continue For
                ExcludedDescendents.Add(AllDescenents(i))
            Next
        End If

        Return Output

    End Function

    Public Function GetSelfOrAncestorOrDescendentsAtLevel(ByVal RequestedDescendentComponentLevel As SpeechMaterialComponent.LinguisticLevels) As List(Of SpeechMaterialComponent)

        If LinguisticLevel = RequestedDescendentComponentLevel Then Return New List(Of SpeechMaterialComponent) From {Me}

        Dim AncestorCandidate = GetAncestorAtLevel(RequestedDescendentComponentLevel)
        If AncestorCandidate IsNot Nothing Then Return New List(Of SpeechMaterialComponent) From {AncestorCandidate}

        Dim DescendantCandidates = GetAllDescenentsAtLevel(RequestedDescendentComponentLevel, False)
        If DescendantCandidates.Count > 0 Then
            Return DescendantCandidates
        Else
            Return Nothing
        End If

    End Function

    Public Function GetAllDescenents(Optional ByRef OutputList As List(Of SpeechMaterialComponent) = Nothing) As List(Of SpeechMaterialComponent)

        If OutputList Is Nothing Then OutputList = New List(Of SpeechMaterialComponent)

        For Each child In ChildComponents

            'Adds the child
            OutputList.Add(child)

            'Calls GetAllDescenents on the child
            child.GetAllDescenents(OutputList)

        Next

        Return OutputList

    End Function



    Public Overrides Function ToString() As String
        Return Me.Id
    End Function


End Class

''' <summary>
''' A class for holding combinations of SpeechMaterialComponents that make up different preselected components to include in a test.
''' </summary>
Public Class SmcPresets
    Inherits SortedSet(Of Preset)

    Public Function GetPreset(ByVal PresetName As String) As Preset

        For Each Preset In Me
            If Preset.Name = PresetName Then Return Preset
        Next
        Return Nothing

    End Function

    Public Class Preset
        Implements IComparable(Of Preset)

        Public Name As String
        Public Members As List(Of SpeechMaterialComponent)

        Public Function CompareTo(other As Preset) As Integer Implements IComparable(Of Preset).CompareTo
            Return String.Compare(Me.Name, other.Name, StringComparison.OrdinalIgnoreCase)
        End Function

        Public Overrides Function ToString() As String
            Return Name
        End Function

    End Class

End Class


''' <summary>
''' A class for looking up custom variables for speech material components.
''' </summary>
Public Class CustomVariablesDatabase

    Public CustomVariablesData As New Dictionary(Of String, SortedList(Of String, Object))

    Public CustomVariableNames As New List(Of String)
    Public CustomVariableTypes As New List(Of VariableTypes)


    Public FilePath As String = ""


    Public Function LoadTabDelimitedFile(ByVal FilePath As String) As Boolean

        Try

            ''Gets a file path from the user if none is supplied
            'If FilePath = "" Then FilePath = Utils.GetOpenFilePath(,, {".txt"}, "Please open a tab delimited word metrics .txt file.")
            'If FilePath = "" Then
            '    MsgBox("No file selected!")
            '    Return Nothing
            'End If

            CustomVariablesData.Clear()
            CustomVariableNames.Clear()
            CustomVariableTypes.Clear()

            'Parses the input file
            Dim InputLines() As String = System.IO.File.ReadAllLines(FilePath, Text.Encoding.UTF8)

            'Stores the file path used for loading the word metric data
            Me.FilePath = FilePath

            'Assuming that there is no data, if the first line is empty!
            If InputLines(0).Trim = "" Then
                Return False
            End If

            'First line should be variable names
            Dim FirstLineData() As String = InputLines(0).Trim(vbTab).Split(vbTab)
            For c = 0 To FirstLineData.Length - 1
                CustomVariableNames.Add(FirstLineData(c).Trim)
            Next

            'Second line should be variable types (N for Numeric or C for Categorical)
            Dim SecondLineData() As String = InputLines(1).Trim(vbTab).Split(vbTab)
            For c = 0 To SecondLineData.Length - 1
                If SecondLineData(c).Trim.ToLower = "n" Then
                    CustomVariableTypes.Add(VariableTypes.Numeric)
                ElseIf SecondLineData(c).Trim.ToLower = "c" Then
                    CustomVariableTypes.Add(VariableTypes.Categorical)
                ElseIf SecondLineData(c).Trim.ToLower = "b" Then
                    CustomVariableTypes.Add(VariableTypes.Boolean)
                Else
                    Throw New Exception("The type for the custom variable " & CustomVariableNames(c) & " in the file " & FilePath & " must be either N for numeric or C for categorical, or B for Boolean.")
                End If
            Next

            'Reading data
            For i = 2 To InputLines.Length - 1

                Dim LineSplit() As String = InputLines(i).Split(vbTab)

                Dim UniqueIdentifier As String = LineSplit(0).Trim

                If UniqueIdentifier = "" Then Continue For

                CustomVariablesData.Add(UniqueIdentifier, New SortedList(Of String, Object))

                'Adding variables (getting only as many as there are variables, or tabs)
                For c = 0 To Math.Min(LineSplit.Length - 1, CustomVariableNames.Count - 1)

                    Dim ValueString As String = LineSplit(c).Trim
                    If CustomVariableTypes(c) = VariableTypes.Numeric Then

                        'Adding the data as a Double
                        Dim NumericValue As Double
                        If Double.TryParse(ValueString.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, NumericValue) Then
                            'Adds the variable and its data only if a value has been parsed
                            CustomVariablesData(UniqueIdentifier).Add(CustomVariableNames(c), NumericValue)
                        Else
                            'Throws an error if parsing failed even though the string was not empty
                            If ValueString.Trim <> "" Then
                                Throw New Exception("Unable to parse the string " & NumericValue & " given for the variable " & CustomVariableNames(c) & " in the file: " & FilePath & " as a numeric value.")
                            Else
                                'Stores as Nothing to signal that the input data was missing
                                CustomVariablesData(UniqueIdentifier).Add(CustomVariableNames(c), Nothing)
                            End If
                        End If

                    ElseIf CustomVariableTypes(c) = VariableTypes.Boolean Then

                        'Adding the data as a boolean
                        Dim BooleanValue As Boolean
                        If Boolean.TryParse(ValueString, BooleanValue) Then
                            'Adds the variable and its data only if a value has been parsed
                            CustomVariablesData(UniqueIdentifier).Add(CustomVariableNames(c), BooleanValue)
                        Else
                            'Throws an error if parsing failed even though the string was not empty
                            If ValueString.Trim <> "" Then
                                Throw New Exception("Unable to parse the string " & BooleanValue & " given for the variable " & CustomVariableNames(c) & " in the file: " & FilePath & " as a boolean value (True or False).")
                            End If
                        End If

                    Else
                        'Adding the data as a String
                        CustomVariablesData(UniqueIdentifier).Add(CustomVariableNames(c), ValueString)
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




    Public Function GetVariableValue(ByVal UniqueIdentifier As String, ByVal VariableName As String) As Object

        If CustomVariablesData.ContainsKey(UniqueIdentifier) Then
            If CustomVariablesData(UniqueIdentifier).ContainsKey(VariableName) Then
                Return CustomVariablesData(UniqueIdentifier)(VariableName)
            End If
        End If

        'Returns Nothing is the UniqueIdentifier or VariableName was not found
        Return Nothing

    End Function


End Class

Public Class InputFileSupport


    Public Shared Function GetInputFileValue(ByVal InputData As String, ByVal ContainsVariableName As Boolean) As String

        'Trimming off any comments
        Dim InputLineSplit() As String = InputData.Split({"//"}, StringSplitOptions.None) 'A comment can be added after // in the input file
        Dim DataPrecedingComments As String = InputLineSplit(0).Trim

        If ContainsVariableName = True Then
            Dim VariebleDataSplit() As String = InputLineSplit(0).Split("=")
            If VariebleDataSplit.Length > 1 Then
                Return VariebleDataSplit(1).Trim
            Else
                Return ""
            End If
        Else
            Return DataPrecedingComments
        End If

    End Function

    Public Shared Function InputFileDoubleValueParsing(ByVal InputData As String, ByVal ContainsVariableName As Boolean, ByVal SourceTextFile As String) As Double?

        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        Dim OutputValue As Double? = Nothing

        Dim ValueString As String = TrimmedData.Replace(",", ".")

        If ValueString = "" Then Return OutputValue

        Try
            OutputValue = Double.Parse(ValueString.Trim, System.Globalization.CultureInfo.InvariantCulture)
        Catch ex As Exception
            Throw New Exception("Non-numeric data ( " & InputData & ") found where numeric data was expected in the file: " & SourceTextFile)
        End Try

        Return OutputValue

    End Function

    Public Shared Function InputFileIntegerValueParsing(ByVal InputData As String, ByVal ContainsVariableName As Boolean, ByVal SourceTextFile As String) As Integer?

        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        Dim OutputValue As Integer? = Nothing

        Dim ValueString As String = TrimmedData.Replace(",", ".")

        Try
            OutputValue = Integer.Parse(ValueString.Trim, System.Globalization.CultureInfo.InvariantCulture)
        Catch ex As Exception
            Throw New Exception("Non-numeric data ( " & InputData & ") found where numeric data was expected in the file: " & SourceTextFile)
        End Try

        Return OutputValue

    End Function


    Public Shared Function InputFilePathValueParsing(ByVal InputData As String, ByVal RootPath As String, ByVal ContainsVariableName As Boolean) As String

        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        If TrimmedData = "" Then
            Return ""
        Else
            If TrimmedData.StartsWith(".\") Then
                Return IO.Path.Combine(RootPath, TrimmedData)
            Else
                Return TrimmedData
            End If
        End If

    End Function

    ''' <summary>
    ''' Parses the InputLine as a the indicated EnumType. Returns the Integer equivalent of the Enum value or Nothing if no value was given.
    ''' </summary>
    ''' <param name="InputData"></param>
    ''' <param name="EnumType"></param>
    ''' <returns></returns>
    Public Shared Function InputFileEnumValueParsing(ByVal InputData As String, ByVal EnumType As Type, ByVal SourceTextFile As String, ByVal ContainsVariableName As Boolean) As Integer?

        'Checks if the type given in EnumType is an enum
        If EnumType.IsEnum = False Then
            Throw New ArgumentException("The EnumType argument (" & EnumType.Name & ") supplied to InputFileEnumValueParsing is not an Enum.")
        End If

        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        If TrimmedData <> "" Then
            Try
                Return DirectCast([Enum].Parse(EnumType, TrimmedData), Integer)
            Catch ex As Exception
                Throw New Exception("Unable to parse the value " & InputData & " in the " & SourceTextFile & " file as a " & EnumType.Name)
            End Try
        Else
            Return Nothing
        End If

    End Function


    Public Shared Function InputFileSortedSetOfIntegerValueParsing(ByVal InputData As String, ByVal ContainsVariableName As Boolean, ByVal SourceTextFile As String) As SortedSet(Of Integer)

        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        If TrimmedData = "" Then Return Nothing

        Dim ValueSplit() As String = TrimmedData.Split(",")
        Dim ValueList As New SortedSet(Of Integer)
        For Each Value In ValueSplit

            Dim CastValue = InputFileIntegerValueParsing(Value, False, SourceTextFile)
            If CastValue IsNot Nothing = True Then ValueList.Add(CastValue.Value)

        Next

        If ValueList.Count > 0 Then
            Return ValueList
        Else
            Return Nothing
        End If

    End Function

    Public Shared Function InputFileListOfStringParsing(ByVal InputData As String, ByVal IncludeEmptyStrings As Boolean, ByVal ContainsVariableName As Boolean) As List(Of String)

        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        If TrimmedData = "" Then Return Nothing

        Dim ValueSplit() As String = TrimmedData.Split(",")
        Dim ValueList As New List(Of String)
        For Each Value In ValueSplit
            If IncludeEmptyStrings = True Then
                ValueList.Add(Value)
            Else
                If Value.Trim <> "" = True Then ValueList.Add(Value.Trim)
            End If
        Next

        If ValueList.Count > 0 Then
            Return ValueList
        Else
            Return Nothing
        End If

    End Function

    Public Shared Function InputFileListOfDoubleParsing(ByVal InputData As String, ByVal ContainsVariableName As Boolean, ByVal SourceTextFile As String) As List(Of Double)

        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        If TrimmedData = "" Then Return Nothing

        Dim ValueSplit() As String = TrimmedData.Split(",")
        Dim ValueList As New List(Of Double)
        For Each Value In ValueSplit
            Try
                ValueList.Add(Double.Parse(Value.Trim, System.Globalization.CultureInfo.InvariantCulture))
            Catch ex As Exception
                Throw New Exception("Non-numeric data ( " & InputData & ") found where numeric data was expected in the file: " & SourceTextFile)
            End Try
        Next

        If ValueList.Count > 0 Then
            Return ValueList
        Else
            Return Nothing
        End If

    End Function

    Public Shared Function InputFileListOfIntegerParsing(ByVal InputData As String, ByVal ContainsVariableName As Boolean, ByVal SourceTextFile As String) As List(Of Integer)

        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        If TrimmedData = "" Then Return Nothing

        Dim ValueSplit() As String = TrimmedData.Split(",")
        Dim ValueList As New List(Of Integer)
        For Each Value In ValueSplit
            Try
                ValueList.Add(Integer.Parse(Value.Trim, System.Globalization.CultureInfo.InvariantCulture))
            Catch ex As Exception
                Throw New Exception("Non-numeric data ( " & InputData & ") found where numeric data was expected in the file: " & SourceTextFile)
            End Try
        Next

        If ValueList.Count > 0 Then
            Return ValueList
        Else
            Return Nothing
        End If

    End Function

    Public Shared Function InputFileSortedSetOfStringParsing(ByVal InputData As String, ByVal IncludeEmptyStrings As Boolean, ByVal ContainsVariableName As Boolean) As SortedSet(Of String)

        Dim ListedValues = InputFileListOfStringParsing(InputData, IncludeEmptyStrings, ContainsVariableName)
        If ListedValues IsNot Nothing Then
            Dim OutputValue As New SortedSet(Of String)
            For Each value In ListedValues
                OutputValue.Add(value)
            Next
            Return OutputValue
        Else
            Return Nothing
        End If

    End Function


    Public Shared Function InputFileBooleanValueParsing(ByVal InputData As String, ByVal ContainsVariableName As Boolean, ByVal SourceTextFile As String) As Boolean?


        Dim TrimmedData = GetInputFileValue(InputData, ContainsVariableName)

        If TrimmedData <> "" Then

            Dim OutputValue As Boolean

            If Boolean.TryParse(TrimmedData, OutputValue) = True Then
                Return OutputValue
            Else
                Throw New ArgumentException("Unable to parse the data in the string '" & InputData & "' in the file: " & SourceTextFile & "as a boolean value (True or False).")
            End If
        End If

        Return Nothing

    End Function

End Class


Public Enum VariableTypes
    Numeric
    Categorical
    [Boolean]
End Enum
