Imports System.Globalization
Imports SpeechTestFramework.Audio.Sound.SpeechMaterialAnnotation

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

    Public Function GetTestSituationVariableValue()
        'This function should somehow returns the requested variable values from the indicated test situation, or even offer an option to create/calculate that data if not present.
        Throw New NotImplementedException
    End Function

    <Obsolete>
    Public Function OrderedChildren() As Boolean

        'This function is to be removed!

        If ChildComponents Is Nothing Then
            Return False
        Else
            Return ChildComponents(0).IsSequentiallyOrdered
        End If
    End Function

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
    Private _Presets As SortedList(Of String, List(Of SpeechMaterialComponent))

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

    Public Property Presets As SortedList(Of String, List(Of SpeechMaterialComponent))
        Get
            If Me.ParentComponent IsNot Nothing Then
                Return Me.ParentComponent.Presets
            Else
                Return Me._Presets
            End If
        End Get
        Set(value As SortedList(Of String, List(Of SpeechMaterialComponent)))
            If Me.ParentComponent IsNot Nothing Then
                Me.ParentComponent.Presets = value
            Else
                Me._Presets = value
            End If
        End Set
    End Property


    Public Property IsPractiseComponent As Boolean = False

    ''' <summary>
    ''' Returns the expected name of the media folder of the current component
    ''' </summary>
    ''' <returns></returns>
    Public Function GetMediaFolderName() As String

        'If LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.ListCollection Then
        '    Throw New ArgumentException("The linguistic level " & SpeechMaterialComponent.LinguisticLevels.ListCollection.ToString & " (" & SpeechMaterialComponent.LinguisticLevels.ListCollection & " ) does not support media folders. (Media items can only be specified for lower levels.)")
        'End If

        Return Id & "_" & PrimaryStringRepresentation.Replace(" ", "_")

    End Function

    Public Function GetMaskerFolderName() As String

        'If LinguisticLevel = SpeechMaterialComponent.LinguisticLevels.ListCollection Then
        '    Throw New ArgumentException("The linguistic level " & SpeechMaterialComponent.LinguisticLevels.ListCollection.ToString & " (" & SpeechMaterialComponent.LinguisticLevels.ListCollection & " ) does not support media folders. (Media items can only be specified for lower levels.)")
        'End If

        Return Id & "_" & PrimaryStringRepresentation.Replace(" ", "_")

    End Function

    Private Randomizer As Random

    'Shared stuff used to keep media items in memory instead of re-loading on every use
    Public Shared AudioFileLoadMode As MediaFileLoadModes = MediaFileLoadModes.LoadOnFirstUse
    Public Shared ImageFileLoadMode As MediaFileLoadModes = MediaFileLoadModes.LoadOnFirstUse
    Public Shared SoundLibrary As New SortedList(Of String, Audio.Sound)
    Public Shared ImageLibrary As New SortedList(Of String, Drawing.Bitmap)
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
                Throw New ArgumentException("Unkown SpeechMaterialComponent.LinguisticLevel")
        End Select
    End Function

    Public Sub New(ByRef rnd As Random)
        Me.Randomizer = rnd
    End Sub

    Public Function GetRandomNumber() As Double
        Return Randomizer.NextDouble()
    End Function

    Public Enum MediaTypes
        Audio
        Image
    End Enum

    Public Sub SetIdAsCategoricalCustumVariable(ByVal CascadeToLowerLevels As Boolean)

        Me.SetCategoricalVariableValue("Id", Me.Id)

        If CascadeToLowerLevels = True Then
            For Each ChildComponent In Me.ChildComponents
                ChildComponent.SetIdAsCategoricalCustumVariable(CascadeToLowerLevels)
            Next
        End If

    End Sub


    Private Function GetAvailableFiles(ByVal Folder As String, ByVal MediaType As MediaTypes) As List(Of String)

        'Getting files in that folder
        Dim AvailableFiles = IO.Directory.GetFiles(Folder)

        Dim IncludedFiles As New List(Of String)

        Dim AllowedFileExtensions As New List(Of String)

        Select Case MediaType
            Case MediaTypes.Audio
                AllowedFileExtensions.Add(".wav")

            Case MediaTypes.Image
                AllowedFileExtensions.Add(".png")

            Case Else
                Throw New Exception("Unknown value For MediaType")
        End Select

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
    ''' <returns></returns>
    Public Function GetSound(ByRef MediaSet As MediaSet, ByVal Index As Integer, ByVal SoundChannel As Integer,
                             Optional ByVal CrossFadeLength As Integer? = Nothing,
                             Optional ByVal Paddinglength As Integer? = Nothing,
                             Optional ByVal InterComponentlength As Integer? = Nothing,
                             Optional ByRef InitialMargin As Integer = 0,
                             Optional ByVal RectifySmaComponents As Boolean = False,
                             Optional ByVal SupressWarnings As Boolean = False) As Audio.Sound

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
            For i = 0 To CorrespondingSmaComponentList.Count - 1

                Dim SmaComponent = CorrespondingSmaComponentList(i)

                'Getting the sound
                Dim CurrentComponentSound = SmaComponent.GetSoundFileSection(SoundChannel,, InitialMargin)

                'Creating a padding sound if needed
                If Paddinglength.HasValue Then
                    If PaddingSound Is Nothing Then
                        PaddingSound = Audio.GenerateSound.CreateSilence(CurrentComponentSound.WaveFormat,, Paddinglength.Value, Audio.BasicAudioEnums.TimeUnits.samples)
                    End If
                    If i = 0 Then
                        'Adding initial padding sound
                        SoundList.Add(PaddingSound)
                        'Shifts the time by the length of the inserted silence
                        CumulativeTimeShift += PaddingSound.WaveData.SampleData(SoundChannel).Length
                    End If
                End If

                'Adding sound and Sma component
                SoundList.Add(CurrentComponentSound)
                If RectifySmaComponents = True Then
                    Dim IsolatedSMA = SmaComponent.ReturnIsolatedSMA
                    IsolatedSMA.TimeShift(CumulativeTimeShift)
                    SmaList.Add(IsolatedSMA)
                End If
                'Shifts the time with the length of the added sound
                CumulativeTimeShift += CurrentComponentSound.WaveData.SampleData(SoundChannel).Length

                'Adding inter-stimulus sound if needed, but not after the last component
                If InterComponentlength.HasValue Then
                    If i = CorrespondingSmaComponentList.Count - 1 Then
                        'Creating a inter-stimulus sound if needed
                        If SilentInterStimulusSound Is Nothing Then
                            SilentInterStimulusSound = Audio.GenerateSound.CreateSilence(CurrentComponentSound.WaveFormat,, InterComponentlength.Value, Audio.BasicAudioEnums.TimeUnits.samples)
                        End If

                        'Adding the interstimulus sound
                        SoundList.Add(SilentInterStimulusSound)
                        'Shifts the time by the length of the inserted silence
                        CumulativeTimeShift += SilentInterStimulusSound.WaveData.SampleData(SoundChannel).Length
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

                        'No need to do anything?

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

            If RectifySmaComponents = True Then

                'TODO: This is not finished.
                'We also need to 

            End If

            If SoundList.Count > 0 Then
                ReturnSound = Audio.DSP.ConcatenateSounds(SoundList, ,,,,, CrossFadeLength)
                If ReturnSound IsNot Nothing Then
                    If RectifySmaComponents = True Then ReturnSound.SMA = RectifiedSMA
                End If
            End If

        End If

        'Changing InitialMargin to 0 if it was never set
        If InitialMargin < 0 Then InitialMargin = 0

        Return ReturnSound

    End Function

    ''' <summary>
    ''' Returns the wave file format of the first located speech material component at the AudioFileLinguisticLevel of the selected Mediaset, or Nothing is no sound file is found.
    ''' </summary>
    ''' <param name="MediaSet"></param>
    ''' <returns></returns>
    Public Function GetWavefileFormat(ByRef MediaSet As MediaSet) As Audio.Formats.WaveFormat

        Dim FirstRelativeWithSound = GetFirstRelativeWithSound(MediaSet)

        Dim ComponentSound = FirstRelativeWithSound.GetSound(MediaSet, 0, 1)

        If ComponentSound IsNot Nothing Then
            Return ComponentSound.WaveFormat
        Else
            Return Nothing
        End If

    End Function


    Public Function GetDurationOfContrastingComponents(ByRef MediaSet As MediaSet,
                                                               ByVal ContrastLevel As SpeechMaterialComponent.LinguisticLevels,
                                                       ByVal MediaItemIndex As Integer,
                                                               ByVal SoundChannel As Integer) As List(Of Double)


        Dim TargetComponents = Me.GetAllDescenentsAtLevel(ContrastLevel, True)


        'Get the SMA components representing the sound sections of all target components
        Dim CurrentSmaComponentList As New List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)

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
            Case Else
                'If it's a ListCollection, the ComponentIndices are simply returned as it will have neither an index nor a parent
                Return ComponentIndices
        End Select

        'Calls FindSelfIndices on the parent
        Me.ParentComponent.FindSelfIndices(ComponentIndices)

        Return ComponentIndices

    End Function

    Public Class ComponentIndices
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
            If ListIndex > -1 And SentenceIndex > -1 And WordIndex > -1 And PhoneIndex > -1 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function HasWordIndex() As Boolean
            If ListIndex > -1 And SentenceIndex > -1 And WordIndex > -1 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function HasSentenceIndex() As Boolean
            If ListIndex > -1 And SentenceIndex > -1 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function HasListIndex() As Boolean
            If ListIndex > -1 Then
                Return True
            Else
                Return False
            End If
        End Function

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


    ''' <summary>
    ''' Locates and returns the first speech material component that has (or is planned should have) a sound recording, based on the AudioFileLinguisticLevel of the selected Mediaset.
    ''' </summary>
    ''' <param name="MediaSet"></param>
    ''' <returns></returns>
    Public Function GetFirstRelativeWithSound(ByRef MediaSet As MediaSet) As SpeechMaterialComponent

        If Me.LinguisticLevel = MediaSet.AudioFileLinguisticLevel Then

            Return Me

        ElseIf Me.LinguisticLevel > MediaSet.AudioFileLinguisticLevel Then

            'E.g. SMC is a Word and AudioFileLinguisticLevel is a Sentence
            Return GetAncestorAtLevel(MediaSet.AudioFileLinguisticLevel)

        ElseIf Me.LinguisticLevel < MediaSet.AudioFileLinguisticLevel Then

            'E.g. SMC is a List and AudioFileLinguisticLevel is a sentence
            Dim SmcsWithSoundFile = GetAllDescenentsAtLevel(MediaSet.AudioFileLinguisticLevel)
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
    Public Function GetConcatenatedComponentsSound(ByRef MediaSet As MediaSet,
                                                   ByVal SegmentsLevel As SpeechMaterialComponent.LinguisticLevels,
                                                   ByVal OnlyLinguisticallyContrastingSegments As Boolean,
                                                   ByVal SoundChannel As Integer,
                                                   ByVal SkipPractiseComponents As Boolean,
                                                   Optional ByVal MinimumSegmentDuration As Double = 0,
                                                   Optional ByVal ComponentCrossFadeDuration As Double = 0.001,
                                                   Optional ByVal FadeConcatenatedSound As Boolean = True,
                                                   Optional ByVal RemoveDcComponent As Boolean = True,
                                                   Optional ByVal IncludeSelf As Boolean = False) As Audio.Sound


        Dim TargetComponents = Me.GetAllDescenentsAtLevel(SegmentsLevel, IncludeSelf)

        'Get the SMA components representing the sound sections of all target components
        Dim CurrentSmaComponentList As New List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)

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
        Dim SoundSectionList As New List(Of Audio.Sound)
        Dim WaveFormat As Audio.Formats.WaveFormat = Nothing
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
        Dim ConcatenatedSound = Audio.DSP.ConcatenateSounds(SoundSectionList, False,,,,, ComponentCrossFadeDuration * WaveFormat.SampleRate, False, 10, True)

        'Fading very slightly to avoid initial and final impulses
        If FadeConcatenatedSound = True Then
            Audio.DSP.Fade(ConcatenatedSound, Nothing, 0,,, ConcatenatedSound.WaveFormat.SampleRate * 0.01, Audio.DSP.FadeSlopeType.Linear)
            Audio.DSP.Fade(ConcatenatedSound, 0, Nothing,, ConcatenatedSound.WaveData.SampleData(1).Length - ConcatenatedSound.WaveFormat.SampleRate * 0.01,, Audio.DSP.FadeSlopeType.Linear)
        End If

        'Removing DC-component
        If RemoveDcComponent = True Then Audio.DSP.RemoveDcComponent(ConcatenatedSound)

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
    Public Function GetConcatenatedComponentsSounds(ByRef MediaSet As MediaSet,
                                                   ByVal SegmentsLevel As SpeechMaterialComponent.LinguisticLevels,
                                                   ByVal OnlyLinguisticallyContrastingSegments As Boolean,
                                                   ByVal SoundChannel As Integer,
                                                   ByVal SkipPractiseComponents As Boolean,
                                                   Optional ByVal MinimumSegmentDuration As Double = 0,
                                                   Optional ByVal ComponentCrossFadeDuration As Double = 0.001,
                                                   Optional ByVal FadeConcatenatedSound As Boolean = True,
                                                   Optional ByVal RemoveDcComponent As Boolean = True) As List(Of Audio.Sound)

        Throw New Exception("This function has not yet been debugged!")

        Dim DescenentComponents = Me.GetAllDescenentsAtLevel(SegmentsLevel)

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

        Dim OutputSounds As New List(Of Audio.Sound)
        For Each kvp In CousinComponentList

            Dim TargetComponents = kvp.Value

            'Get the SMA components representing the sound sections of all target components
            Dim CurrentSmaComponentList As New List(Of Audio.Sound.SpeechMaterialAnnotation.SmaComponent)

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
            Dim SoundSectionList As New List(Of Audio.Sound)
            Dim WaveFormat As Audio.Formats.WaveFormat = Nothing
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
            Dim ConcatenatedSound = Audio.DSP.ConcatenateSounds(SoundSectionList, False,,,,, ComponentCrossFadeDuration * WaveFormat.SampleRate, False, 10, True)

            'Fading very slightly to avoid initial and final impulses
            If FadeConcatenatedSound = True Then
                Audio.DSP.Fade(ConcatenatedSound, Nothing, 0,,, ConcatenatedSound.WaveFormat.SampleRate * 0.01, Audio.DSP.FadeSlopeType.Linear)
                Audio.DSP.Fade(ConcatenatedSound, 0, Nothing,, ConcatenatedSound.WaveData.SampleData(1).Length - ConcatenatedSound.WaveFormat.SampleRate * 0.01,, Audio.DSP.FadeSlopeType.Linear)
            End If

            'Removing DC-component
            If RemoveDcComponent = True Then Audio.DSP.RemoveDcComponent(ConcatenatedSound)

            OutputSounds.Add(ConcatenatedSound)

        Next

        If OutputSounds.Count = 0 Then Return Nothing

        Return OutputSounds

    End Function


    Public Function GetBackgroundNonspeechPath(ByRef MediaSet As MediaSet, ByVal Index As Integer) As String

        Dim CurrentTestRootPath As String = ParentTestSpecification.GetTestRootPath
        Dim FolderPath = IO.Path.Combine(CurrentTestRootPath, MediaSet.BackgroundNonspeechParentFolder)

        Dim AvailablePaths = GetAvailableFiles(FolderPath, MediaTypes.Audio)

        If Index > AvailablePaths.Count - 1 Then
            Return Nothing
        Else
            Return AvailablePaths(Index)
        End If

    End Function

    Public Function GetBackgroundSpeechPath(ByRef MediaSet As MediaSet, ByVal Index As Integer) As String

        Dim CurrentTestRootPath As String = ParentTestSpecification.GetTestRootPath
        Dim FolderPath = IO.Path.Combine(CurrentTestRootPath, MediaSet.BackgroundSpeechParentFolder)

        Dim AvailablePaths = GetAvailableFiles(FolderPath, MediaTypes.Audio)

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

            Return GetAvailableFiles(FullMaskerFolderPath, MediaTypes.Audio)(Index)

        End If

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

            Return GetAvailableFiles(FullMediaFolderPath, MediaTypes.Audio)(Index)

        End If

    End Function


    Public Function GetSoundFile(ByVal Path As String) As Audio.Sound

        Select Case AudioFileLoadMode
            Case MediaFileLoadModes.LoadEveryTime
                Return Audio.Sound.LoadWaveFile(Path)

            Case MediaFileLoadModes.LoadOnFirstUse

                If SoundLibrary.ContainsKey(Path) Then
                    Return SoundLibrary(Path)
                Else
                    Dim NewSound As Audio.Sound = Audio.Sound.LoadWaveFile(Path)
                    SoundLibrary.Add(Path, NewSound)
                    Return SoundLibrary(Path)
                End If

            Case Else
                Throw New NotImplementedException
        End Select

    End Function

    Public Sub ClearAllLoadedSounds()
        SoundLibrary.Clear()
    End Sub

    Public Function GetAllLoadedSounds() As SortedList(Of String, Audio.Sound)
        Return SoundLibrary
    End Function

    ''' <summary>
    ''' Saves all loaded sounds to their original location.
    ''' </summary>
    ''' <param name="SaveOnlyModified"></param>
    Public Sub SaveAllLoadedSounds(Optional ByVal SaveOnlyModified As Boolean = True)

        For Each CurrentSound In SoundLibrary
            If SaveOnlyModified = True Then
                If CurrentSound.Value.IsChanged = False Then
                    Continue For
                End If
            End If

            CurrentSound.Value.WriteWaveFile(CurrentSound.Key)
        Next

    End Sub

    Public Function GetImage(ByVal Path) As Drawing.Bitmap

        Select Case ImageFileLoadMode
            Case MediaFileLoadModes.LoadEveryTime

                Return Drawing.Bitmap.FromFile(Path)

            Case MediaFileLoadModes.LoadOnFirstUse

                If ImageLibrary.ContainsKey(Path) Then
                    Return ImageLibrary(Path)
                Else
                    Dim NewImage As Drawing.Bitmap = Drawing.Bitmap.FromFile(Path)
                    ImageLibrary.Add(Path, NewImage)
                    Return ImageLibrary(Path)
                End If

            Case Else
                Throw New NotImplementedException
        End Select

    End Function


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
    ''' Returns all numeric custom variable names separately at different linguistic levels for the current instance of SpeechMaterialComponent and all its descendants.
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


    Public Sub CreateVariable_HasVowelContrast(ByVal SummaryLevel As SpeechMaterialComponent.LinguisticLevels,
                                                Optional ByVal PhoneticTranscriptionVariableName As String = "Transcription",
                                                Optional VariableName As String = "HasVowelContrast")

        If SummaryLevel = SpeechMaterialComponent.LinguisticLevels.Sentence Or SummaryLevel = SpeechMaterialComponent.LinguisticLevels.List Then
            'These are supported
        Else
            Throw New ArgumentException("Only 'Sentence' and 'List' are supported as values for SummaryLevel.")
        End If

        'Gets all summarty components into which the new variable should be stored
        Dim SummaryComponents = Me.GetToplevelAncestor.GetAllRelativesAtLevel(SummaryLevel)

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

        'Finally writes the results to file
        'Ask if overwrite or save to new location
        Dim res = MsgBox("Do you want to overwrite the existing files? Select NO to save the new files to a new location?", MsgBoxStyle.YesNo, "Overwrite existing files?")
        If res = MsgBoxResult.Yes Then

            'Saving updated files
            Me.GetToplevelAncestor.WriteSpeechMaterialToFile(Me.ParentTestSpecification, Me.ParentTestSpecification.GetTestRootPath)
            MsgBox("Your speech material file and corresponding custom variable files should now have been saved to " & Me.ParentTestSpecification.GetSpeechMaterialFolder & vbCrLf & "Click OK to continue.",
                   MsgBoxStyle.Information, "Files saved")

        Else

            'Saving updated files
            Me.GetToplevelAncestor.WriteSpeechMaterialToFile(Me.ParentTestSpecification)
            MsgBox("Your speech material file and corresponding custom variable files should now have been saved to the selected folder. Click OK to continue.", MsgBoxStyle.Information, "Files saved")

        End If

    End Sub

    Public Sub CreateVariable_ContrastedPhonemeIndex(ByVal SummaryLevel As SpeechMaterialComponent.LinguisticLevels,
                                                Optional ByVal PhoneticTranscriptionVariableName As String = "Transcription",
                                                Optional VariableName As String = "ContrastedPhonemeIndex")

        If SummaryLevel = SpeechMaterialComponent.LinguisticLevels.Sentence Or SummaryLevel = SpeechMaterialComponent.LinguisticLevels.List Then
            'These are supported
        Else
            Throw New ArgumentException("Only 'Sentence' and 'List' are supported as values for SummaryLevel.")
        End If

        'Gets all summarty components into which the new variable should be stored
        Dim SummaryComponents = Me.GetToplevelAncestor.GetAllRelativesAtLevel(SummaryLevel)

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

        'Finally writes the results to file
        'Ask if overwrite or save to new location
        Dim res = MsgBox("Do you want to overwrite the existing files? Select NO to save the new files to a new location?", MsgBoxStyle.YesNo, "Overwrite existing files?")
        If res = MsgBoxResult.Yes Then

            'Saving updated files
            Me.GetToplevelAncestor.WriteSpeechMaterialToFile(Me.ParentTestSpecification, Me.ParentTestSpecification.GetTestRootPath)
            MsgBox("Your speech material file and corresponding custom variable files should now have been saved to " & Me.ParentTestSpecification.GetSpeechMaterialFolder & vbCrLf & "Click OK to continue.",
                   MsgBoxStyle.Information, "Files saved")

        Else

            'Saving updated files
            Me.GetToplevelAncestor.WriteSpeechMaterialToFile(Me.ParentTestSpecification)
            MsgBox("Your speech material file and corresponding custom variable files should now have been saved to the selected folder. Click OK to continue.", MsgBoxStyle.Information, "Files saved")

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

        If ParentComponent Is Nothing Then Return Nothing

        Dim Siblings = GetSiblings()
        For s = 0 To Siblings.Count - 1
            If Siblings(s) Is Me Then Return s
        Next

        Return Nothing

    End Function


    Public Function GetSamePlaceCousins() As List(Of SpeechMaterialComponent)

        Dim SelfIndex = GetSelfIndex()

        Dim MySiblingCount As Integer = GetSiblings.Count

        If SelfIndex Is Nothing Then Return Nothing

        If ParentComponent Is Nothing Then Return Nothing

        'Checks that the parent has ordered children
        If ParentComponent.OrderedChildren = False Then Throw New Exception("Cannot Return same-place cousins from unordered components. (Component id: " & ParentComponent.Id & ")")

        Dim Aunties = ParentComponent.GetSiblingsExcludingSelf

        If Aunties Is Nothing Then Return Nothing

        Dim OutputList As New List(Of SpeechMaterialComponent)

        For Each auntie In Aunties

            'Checks that the aunties have ordered children
            If auntie.OrderedChildren = False Then Throw New Exception("Cannot return same-place cousins from unordered components. (Component id: " & auntie.Id & ")")

            'Checks that the number of sibling copmonents are the same
            Dim AuntiesChildCount As Integer = auntie.ChildComponents.Count
            If MySiblingCount <> AuntiesChildCount Then Throw New Exception("Cannot return same-place cousins from cousin groups that differ in count. (Component ids: " & ParentComponent.Id & " vs. " & auntie.Id & ")")

            'Finally adding the same place cousin
            OutputList.Add(auntie.ChildComponents(SelfIndex))

        Next

        Return OutputList

    End Function

    ''' <summary>
    ''' Returns the second cousin components that are stored at the same hierachical index orders.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSamePlaceSecondCousins() As List(Of SpeechMaterialComponent)

        Dim SelfIndex = GetSelfIndex()

        If SelfIndex Is Nothing Then Return Nothing

        Dim SiblingCount As Integer = ParentComponent.GetSiblings.Count

        If ParentComponent Is Nothing Then Return Nothing

        Dim ParentSelfIndex = ParentComponent.GetSelfIndex()

        Dim ParentSiblingCount As Integer = ParentComponent.GetSiblings.Count

        If ParentSelfIndex Is Nothing Then Return Nothing

        If ParentComponent.ParentComponent Is Nothing Then Return Nothing

        'Checks that the grand-parent has ordered children
        If ParentComponent.ParentComponent.OrderedChildren = False Then Throw New Exception("Cannot return same-place second cousins from unordered components. (Component id: " & ParentComponent.ParentComponent.Id & ")")

        Dim ParentAunties = ParentComponent.ParentComponent.GetSiblingsExcludingSelf

        If ParentAunties Is Nothing Then Return Nothing

        Dim OutputList As New List(Of SpeechMaterialComponent)

        For Each parentAuntie In ParentAunties

            'Checks that the parent auntie have ordered children
            If parentAuntie.OrderedChildren = False Then Throw New Exception("Cannot return same-place cousins from unordered components. (Component id: " & parentAuntie.Id & ")")

            'Checks that the number of sibling components are the same
            Dim ParentAuntieChildCount As Integer = parentAuntie.ChildComponents.Count
            If ParentSiblingCount <> ParentAuntieChildCount Then Throw New Exception("Cannot return same-place cousins from cousin groups that differ in count. (Component ids: " & ParentComponent.Id & " vs. " & parentAuntie.Id & ")")

            'Getting the same place second cousins
            Dim SamePlaceParentCousin = parentAuntie.ChildComponents(ParentSelfIndex)

            'Checks that the SamePlaceParentCousin have ordered children
            If SamePlaceParentCousin.OrderedChildren = False Then Throw New Exception("Cannot return same-place cousins from unordered components. (Component id: " & SamePlaceParentCousin.Id & ")")

            'Checks that the number of sibling components are the same
            Dim SamePlaceParentCousinChildCount As Integer = SamePlaceParentCousin.ChildComponents.Count
            If SiblingCount <> SamePlaceParentCousinChildCount Then Throw New Exception("Cannot return same-place second cousins from cousin groups that differ in count. (Component ids: " & ParentComponent.Id & " vs. " & SamePlaceParentCousin.Id & ")")

            OutputList.Add(SamePlaceParentCousin.ChildComponents(SelfIndex))

        Next

        Return OutputList

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
    ''' Recursively searches for the SpeechMaterialComponent at the top of the heirachy
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

    Public Function GetAllRelativesAtLevel(ByVal Level As SpeechMaterialComponent.LinguisticLevels) As List(Of SpeechMaterialComponent)
        'Creates a list
        Dim OutputList As New List(Of SpeechMaterialComponent)
        Dim AllRelatives = GetAllRelatives()
        For Each Component In AllRelatives
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

    Public Shared Function LoadSpeechMaterial(ByVal SpeechMaterialComponentFilePath As String, ByVal TestRootPath As String) As SpeechMaterialComponent

        'Gets a file path from the user if none is supplied
        If SpeechMaterialComponentFilePath = "" Then SpeechMaterialComponentFilePath = Utils.GetOpenFilePath(,, {".txt"}, "Please open a stuctured speech material component .txt file.")
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

        Presets = New SortedList(Of String, List(Of SpeechMaterialComponent))

        If PresetSpecifications.Count > 0 Then

            For Each PresetSpecification In PresetSpecifications
                Dim SelectedComponentsList As New SortedList(Of String, SpeechMaterialComponent) ' Where String is SpeechMaterialComponent.Id

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

                Presets.Add(PresetSpecification.Item1, SelectedComponentsList.Values.ToList)

            Next

        Else

            'If no presets have been defined, a default preset containing all components (except practise components) at the PresetLevel is added
            Dim SelectedComponentsList As New SortedList(Of String, SpeechMaterialComponent) ' Where String is SpeechMaterialComponent.Id
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
            Presets.Add("All items", SelectedComponentsList.Values.ToList)
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
    ''' <param name="IncludeSelf">Set to true, in order to also include the current instance of SpeechMaterialComponent, in case its LinguisticLevel equals the specified linguistic level.</param>
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

        Dim RandomIndices = Utils.SampleWithoutReplacement(n, 0, AllDescenents.Count, Randomizer)
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



    ''' <summary>
    ''' Converts the speech material component to a new SpeechMaterialAnnotation object prepared for manual segmentation.
    ''' </summary>
    ''' <returns></returns>
    Public Function ConvertToSMA() As Audio.Sound.SpeechMaterialAnnotation

        If Me.LinguisticLevel = LinguisticLevels.ListCollection Then
            MsgBox("Cannot convert a component at the ListCollection linguistic level to a SMA object. The highest level which can be stored in a SMA object is LinguisticLevels.List." & vbCrLf & "Aborting conversion!")
            Return Nothing
        End If

        Dim NewSMA = New Audio.Sound.SpeechMaterialAnnotation With {.SegmentationCompleted = False}

        'Creating a (mono) channel level SmaComponent
        NewSMA.AddChannelData(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.CHANNEL, Nothing))

        'Adjusting to the right level
        If Me.LinguisticLevel = LinguisticLevels.Phoneme Then

            'We need to add all levels: Sentence, Word, Phone
            NewSMA.ChannelData(1).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE, NewSMA.ChannelData(1)))
            NewSMA.ChannelData(1)(0).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD, NewSMA.ChannelData(1)(0)))
            NewSMA.ChannelData(1)(0)(0).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, NewSMA.ChannelData(1)(0)(0)))

            'Calling AddSmaValues, which recursively adds all lower level components
            AddSmaValues(NewSMA.ChannelData(1)(0)(0)(0))

        ElseIf Me.LinguisticLevel = LinguisticLevels.Word Then

            'We need to add all levels: Sentence, Word
            NewSMA.ChannelData(1).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE, NewSMA.ChannelData(1)))
            NewSMA.ChannelData(1)(0).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD, NewSMA.ChannelData(1)(0)))

            'Calling AddSmaValues, which recursively adds all lower level components
            AddSmaValues(NewSMA.ChannelData(1)(0)(0))

        ElseIf Me.LinguisticLevel = LinguisticLevels.Sentence Then

            'We need to add all levels: Sentence
            NewSMA.ChannelData(1).Add(New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, Audio.Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE, NewSMA.ChannelData(1)))
            AddSmaValues(NewSMA.ChannelData(1)(0))

        ElseIf Me.LinguisticLevel = LinguisticLevels.List Then

            'No need to add any levels. Calling AddSmaValues, which recursively adds all lower level components
            AddSmaValues(NewSMA.ChannelData(1))

        Else

            MsgBox("Unknown value for the LinguisticLevels Enum" & vbCrLf & "Aborting conversion!")
            Return Nothing

        End If

        Return NewSMA

    End Function

    Private Sub AddSmaValues(ByRef SmaComponent As Audio.Sound.SpeechMaterialAnnotation.SmaComponent)

        'Attemption to get the spelling and transcription from the custom variables
        Dim MySpelling As String = ""
        Dim SpellingCandidateVariableNames As New List(Of String) From {"Spelling", "OrthographicForm"}
        For Each vn In SpellingCandidateVariableNames
            Dim SpellingCandidate = GetCategoricalVariableValue(vn)
            If SpellingCandidate.Trim <> "" Then
                MySpelling = SpellingCandidate
                Exit For
            End If
        Next

        Dim MyTranscription As String = ""
        Dim TranscriptionCandidateVariableNames As New List(Of String) From {"Transcription", "PhonemicForm", "PhoneticForm", "PhoneticTranscription", "PhonemicTranscription"}
        For Each vn In TranscriptionCandidateVariableNames
            Dim TranscriptionCandidate = GetCategoricalVariableValue(vn)
            If TranscriptionCandidate.Trim <> "" Then
                MyTranscription = TranscriptionCandidate
                Exit For
            End If
        Next

        'Using the PrimaryStringRepresentation instead of the spelling it was not found
        If MySpelling = "" Then MySpelling = Me.PrimaryStringRepresentation

        'Using MySpelling in place of the transcription of it was not found
        If MyTranscription = "" Then MyTranscription = MySpelling

        SmaComponent.OrthographicForm = MySpelling
        SmaComponent.PhoneticForm = MyTranscription

        For Each child In Me.ChildComponents

            Dim NewChildComponent = New Audio.Sound.SpeechMaterialAnnotation.SmaComponent(SmaComponent.ParentSMA, SmaComponent.SmaTag + 1, SmaComponent)
            SmaComponent.Add(NewChildComponent)
            child.AddSmaValues(NewChildComponent)

        Next

    End Sub

    ''' <summary>
    ''' Writes the speech material components file and associated custom variables files to the indicated OutputSpeechMaterialFolder.
    ''' </summary>
    ''' <param name="OutputParentFolder"></param>
    Public Sub WriteSpeechMaterialToFile(ByRef CurrentTestSpecification As SpeechMaterialSpecification, Optional ByVal OutputParentFolder As String = "")

        Dim OutputSpeechMaterialFolder As String

        'Getting a save folder if not supplied by the calling code
        If OutputParentFolder = "" Then
            Dim fbd As New Windows.Forms.FolderBrowserDialog
            fbd.Description = "Select a folder in which to save the output files"
            If fbd.ShowDialog() = Windows.Forms.DialogResult.OK Then
                OutputParentFolder = fbd.SelectedPath
            Else
                MsgBox("No output folder selected.", MsgBoxStyle.Exclamation, "Saving speech material")
                Exit Sub
            End If

            If OutputParentFolder.Trim = "" Then
                MsgBox("No output folder selected.", MsgBoxStyle.Exclamation, "Saving speech material")
                Exit Sub
            End If

            'Getting the subdirectory in which to store the speech material components files and the custom variables files
            OutputSpeechMaterialFolder = IO.Path.Combine(OutputParentFolder, SpeechMaterialSpecification.SpeechMaterialsDirectory, CurrentTestSpecification.DirectoryName, SpeechMaterialComponent.SpeechMaterialFolderName)

        Else

            If OutputParentFolder.Trim = "" Then
                MsgBox("No output folder selected.", MsgBoxStyle.Exclamation, "Saving speech material")
                Exit Sub
            End If

            'Getting the subdirectory in which to store the speech material components files and the custom variables files
            OutputSpeechMaterialFolder = IO.Path.Combine(OutputParentFolder, SpeechMaterialComponent.SpeechMaterialFolderName)

        End If

        'Calls WriteSpeechMaterialComponenFile shich saves the files
        WriteSpeechMaterialComponenFile(OutputSpeechMaterialFolder, True)


    End Sub


    Private Sub WriteSpeechMaterialComponenFile(ByVal OutputSpeechMaterialFolder As String, ByVal ExportAtThisLevel As Boolean, Optional ByRef CustomVariablesExportList As SortedList(Of String, List(Of String)) = Nothing,
                                             Optional ByRef NumericCustomVariableNames As SortedList(Of SpeechMaterialComponent.LinguisticLevels, SortedSet(Of String)) = Nothing,
                                                Optional ByRef CategoricalCustomVariableNames As SortedList(Of SpeechMaterialComponent.LinguisticLevels, SortedSet(Of String)) = Nothing)

        If CustomVariablesExportList Is Nothing Then CustomVariablesExportList = New SortedList(Of String, List(Of String))
        If NumericCustomVariableNames Is Nothing Then NumericCustomVariableNames = Me.GetToplevelAncestor.GetNumericCustomVariableNamesByLinguicticLevel()
        If CategoricalCustomVariableNames Is Nothing Then CategoricalCustomVariableNames = Me.GetToplevelAncestor.GetCategoricalCustomVariableNamesByLinguicticLevel()


        Dim OutputList As New List(Of String)
        If ExportAtThisLevel = True Then
            OutputList.Add("// Setup")

            'Writing Setup values
            OutputList.Add("SequentiallyOrderedLists = " & SequentiallyOrderedLists.ToString)
            OutputList.Add("SequentiallyOrderedSentences = " & SequentiallyOrderedSentences.ToString)
            OutputList.Add("SequentiallyOrderedWords = " & SequentiallyOrderedWords.ToString)
            OutputList.Add("SequentiallyOrderedPhonemes = " & SequentiallyOrderedPhonemes.ToString)
            OutputList.Add("PresetLevel = " & PresetLevel.ToString)
            For Each Item In PresetSpecifications
                OutputList.Add("Preset = " & Item.Item1.Trim & ": " & String.Join(", ", Item.Item2))
            Next

            'Writing components
            OutputList.Add("")
            OutputList.Add("// Components")
        End If

        Dim HeadingString As String = "// LinguisticLevel" & vbTab & "Id" & vbTab & "ParentId" & vbTab & "PrimaryStringRepresentation" & vbTab & "IsPractiseComponent"

        Dim Main_List As New List(Of String)

        'Linguistic Level
        Main_List.Add(LinguisticLevel.ToString)

        'Id
        Main_List.Add(Id)

        'ParentId 
        If ParentComponent IsNot Nothing Then
            Main_List.Add(ParentComponent.Id)
        Else
            Main_List.Add("")
        End If

        'PrimaryStringRepresentation
        Main_List.Add(PrimaryStringRepresentation)

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
        Main_List.Add(IsPractiseComponent.ToString)

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
        If Me.LinguisticLevel = LinguisticLevels.List Or Me.LinguisticLevel = LinguisticLevels.ListCollection Then
            OutputList.Add("") 'Adding an empty line between list or list collection level components
        End If

        OutputList.Add(String.Join(vbTab, Main_List))


        'Writing to file
        Utils.SendInfoToLog(String.Join(vbCrLf, OutputList), IO.Path.GetFileNameWithoutExtension(SpeechMaterialComponentFileName), OutputSpeechMaterialFolder, True, True, ExportAtThisLevel)

        'Custom variables
        Dim CustomVariablesDatabasePath As String = SpeechMaterialComponent.GetDatabaseFileName(LinguisticLevel)

        If CustomVariablesDatabasePath <> "" Then

            'Getting the right collection into which to store custom variable values
            Dim CurrentCustomVariablesOutputList As New List(Of String)
            If CustomVariablesExportList.ContainsKey(CustomVariablesDatabasePath) = False Then
                CustomVariablesExportList.Add(CustomVariablesDatabasePath, CurrentCustomVariablesOutputList)
            Else
                CurrentCustomVariablesOutputList = CustomVariablesExportList(CustomVariablesDatabasePath)
            End If

            'Getting the variable names and types
            Dim CategoricalVariableNames = CategoricalCustomVariableNames(Me.LinguisticLevel).ToList
            Dim NumericVariableNames = NumericCustomVariableNames(Me.LinguisticLevel).ToList

            'Writing headings only on the first line
            If CurrentCustomVariablesOutputList.Count = 0 Then
                Dim CustomVariableNamesList As New List(Of String)
                CustomVariableNamesList.AddRange(CategoricalVariableNames)
                CustomVariableNamesList.AddRange(NumericVariableNames)
                Dim CustomVariableNames = String.Join(vbTab, CustomVariableNamesList)

                'Writing types only if there are any headings
                If CustomVariableNames.Trim <> "" Then
                    CurrentCustomVariablesOutputList.Add(CustomVariableNames)

                    Dim CategoricalVariableTypes = Utils.Repeat("C", CategoricalVariableNames.Count).ToList
                    Dim NumericVariableTypes = Utils.Repeat("N", NumericVariableNames.Count).ToList
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
                Dim CurrentValue = Me.GetCategoricalVariableValue(VarName)
                CustomVariableValues.Add(CurrentValue)
            Next
            'Then numeric values
            For Each VarName In NumericCustomVariableNames(Me.LinguisticLevel)
                Dim CurrentValue = Me.GetNumericVariableValue(VarName)
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
        For Each ChildComponent In Me.ChildComponents
            ChildComponent.WriteSpeechMaterialComponenFile(OutputSpeechMaterialFolder, False, CustomVariablesExportList)
        Next

        If ExportAtThisLevel = True Then
            'Exporting custom variables
            For Each item In CustomVariablesExportList
                Utils.SendInfoToLog(String.Join(vbCrLf, item.Value), IO.Path.GetFileNameWithoutExtension(item.Key), OutputSpeechMaterialFolder, True, True, ExportAtThisLevel)
            Next
        End If

    End Sub


    Public Sub SummariseNumericVariables(ByVal SourceLevels As SpeechMaterialComponent.LinguisticLevels, ByVal CustomVariableName As String, ByRef MetricType As NumericSummaryMetricTypes)

        If Me.LinguisticLevel < SourceLevels Then

            Dim Descendants = GetAllDescenentsAtLevel(SourceLevels)

            Dim VariableNameSourceLevelPrefix = SourceLevels.ToString & "_Level_"

            Dim ValueList As New List(Of Double)
            For Each d In Descendants
                ValueList.Add(d.GetNumericVariableValue(CustomVariableName))
            Next

            Select Case MetricType
                Case NumericSummaryMetricTypes.ArithmeticMean

                    'Storing the result
                    Dim SummaryResult As Double = ValueList.Average
                    Me.SetNumericVariableValue(VariableNameSourceLevelPrefix & "Mean_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.StandardDeviation

                    'Storing the result
                    Dim SummaryResult As Double = MathNet.Numerics.Statistics.Statistics.StandardDeviation(ValueList)
                    Me.SetNumericVariableValue(VariableNameSourceLevelPrefix & "SD_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.Maximum

                    'Storing the result
                    Dim SummaryResult As Double = ValueList.Max
                    Me.SetNumericVariableValue(VariableNameSourceLevelPrefix & "Max_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.Minimum

                    'Storing the result
                    Dim SummaryResult As Double = ValueList.Min
                    Me.SetNumericVariableValue(VariableNameSourceLevelPrefix & "Min_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.Median

                    Dim SummaryResult As Double = MathNet.Numerics.Statistics.Statistics.Median(ValueList)
                    Me.SetNumericVariableValue(VariableNameSourceLevelPrefix & "MD_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.InterquartileRange

                    Dim SummaryResult As Double = MathNet.Numerics.Statistics.Statistics.InterquartileRange(ValueList)
                    Me.SetNumericVariableValue(VariableNameSourceLevelPrefix & "IQR_" & CustomVariableName, SummaryResult)

                Case NumericSummaryMetricTypes.CoefficientOfVariation

                    'Storing the result
                    Dim SummaryResult As Double = Utils.CoefficientOfVariation(ValueList)
                    Me.SetNumericVariableValue(VariableNameSourceLevelPrefix & "CV_" & CustomVariableName, SummaryResult)

            End Select


            'Cascading calculations to lower levels
            'Calling this from within the conditional statment, as no descendants should exist at more than one
            'level, and if Me.LinguisticLevel >= SourceLevels no other descendants should be either, and thus the recursive calls can stop here.
            For Each Child In ChildComponents
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


    Public Sub SummariseCategoricalVariables(ByVal SourceLevels As SpeechMaterialComponent.LinguisticLevels, ByVal CustomVariableName As String, ByRef MetricType As CategoricalSummaryMetricTypes)

        If Me.LinguisticLevel < SourceLevels Then

            Dim Descendants = GetAllDescenentsAtLevel(SourceLevels)

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
                        Me.SetCategoricalVariableValue(VariableNameSourceLevelPrefix & "Mode_" & CustomVariableName, ModeValuesString)
                    Else

                        'Storing the an empty string as result, as there was no item in ValueList
                        Me.SetCategoricalVariableValue(VariableNameSourceLevelPrefix & "Mode_" & CustomVariableName, "")
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
                        Me.SetCategoricalVariableValue(VariableNameSourceLevelPrefix & "Distribution_" & CustomVariableName, DistributionString)
                    Else

                        'Storing the an empty string as result, as there was no item in ValueList
                        Me.SetCategoricalVariableValue(VariableNameSourceLevelPrefix & "Distribution_" & CustomVariableName, "")
                    End If

            End Select

            'Cascading calculations to lower levels
            'Calling this from within the conditional statment, as no descendants should exist at more than one
            'level, and if Me.LinguisticLevel >= SourceLevels no other descendants should be either, and thus the recursive calls can stop here.
            For Each Child In ChildComponents
                Child.SummariseCategoricalVariables(SourceLevels, CustomVariableName, MetricType)
            Next

        End If

    End Sub

    Public Enum CategoricalSummaryMetricTypes
        Mode
        Distribution
    End Enum

End Class


''' <summary>
''' A class for looking up custom variables for speech material components.
''' </summary>
Public Class CustomVariablesDatabase

    Private CustomVariablesData As New Dictionary(Of String, SortedList(Of String, Object))

    Public CustomVariableNames As New List(Of String)
    Public CustomVariableTypes As New List(Of VariableTypes)

    Public CaseInvariantSpellings As Boolean

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


    Public Enum LookupMathOptions
        MatchBySpelling
        MatchByTranscription
        MatchBySpellingAndTranscription
    End Enum

    Public Function LoadTabDelimitedFile(ByVal FilePath As String, ByVal MatchBy As LookupMathOptions,
                                         Optional ByVal SpellingVariableName As String = "", Optional ByVal TranscriptionVariableName As String = "",
                                         Optional ByVal IncludeItems As SortedList(Of String, String) = Nothing) As Boolean


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

                If CustomVariablesData.ContainsKey(UniqueIdentifier) Then
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
                CustomVariablesData.Add(OriginalIdentifier, New SortedList(Of String, Object))

                'Adding variables (getting only as many as there are variables, or tabs)
                For c = 0 To Math.Min(LineSplit.Length - 1, CustomVariableNames.Count - 1)

                    Dim ValueString As String = LineSplit(c).Trim
                    If CustomVariableTypes(c) = VariableTypes.Numeric Then

                        'Adding the data as a Double
                        Dim NumericValue As Double
                        If Double.TryParse(ValueString.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, NumericValue) Then
                            'Adds the variable and its data only if a value has been parsed
                            CustomVariablesData(OriginalIdentifier).Add(CustomVariableNames(c), NumericValue)
                        Else
                            'Throws an error if parsing failed even though the string was not empty
                            If ValueString.Trim <> "" Then
                                Throw New Exception("Unable to parse the string " & ValueString & " given for the variable " & CustomVariableNames(c) & " in the file: " & FilePath & " as a numeric value.")
                            Else
                                'Stores a NaN to mark that the input data was missing / NaN
                                CustomVariablesData(OriginalIdentifier).Add(CustomVariableNames(c), Double.NaN)
                            End If
                        End If

                    ElseIf CustomVariableTypes(c) = VariableTypes.Boolean Then

                        'Adding the data as a boolean
                        Dim BooleanValue As Boolean
                        If Boolean.TryParse(ValueString, BooleanValue) Then
                            'Adds the variable and its data only if a value has been parsed
                            CustomVariablesData(OriginalIdentifier).Add(CustomVariableNames(c), BooleanValue)
                        Else
                            'Throws an error if parsing failed even though the string was not empty
                            If ValueString.Trim <> "" Then
                                Throw New Exception("Unable to parse the string " & BooleanValue & " given for the variable " & CustomVariableNames(c) & " in the file: " & FilePath & " as a boolean value (True or False).")
                            End If
                        End If

                    Else
                        'Adding the data as a String
                        CustomVariablesData(OriginalIdentifier).Add(CustomVariableNames(c), ValueString)
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

    Public Function UniqueIdentifierIsPresent(ByVal UniqueIdentifier As String) As Boolean
        If CustomVariablesData.ContainsKey(UniqueIdentifier) Then
            Return True
        Else
            Return False
        End If
    End Function

End Class

Public Class InputFileSupport


#Region "InputFileSupport"

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

    '''' <summary>
    '''' Parses the InputLine as a list of the indicated EnumType. Returns a list of the Integer equivalent of the Enum values or Nothing if no value was given.
    '''' </summary>
    '''' <param name="InputLine"></param>
    '''' <param name="EnumType"></param>
    '''' <returns></returns>
    'Public Shared Function InputFileEnumListValueParsing(ByVal InputLine As String, ByVal EnumType As Type, Optional SourceTextFile As String = "", Optional SourceVariableName As String = "") As List(Of Integer)

    '    Dim ListedStringValues = InputFileListOfStringParsing(InputLine)

    '    If ListedStringValues IsNot Nothing Then

    '        Dim EnumList As New List(Of Integer)

    '        For Each Value In ListedStringValues

    '            Dim NewEnumValue = InputFileEnumValueParsing(Value, EnumType, SourceTextFile, SourceVariableName)

    '            If NewEnumValue IsNot Nothing Then
    '                EnumList.Add(NewEnumValue)
    '            End If
    '        Next

    '        If EnumList.Count > 0 Then
    '            Return EnumList
    '        Else
    '            Return Nothing
    '        End If
    '    Else
    '        Return Nothing
    '    End If

    'End Function


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

    '''' <summary>
    '''' Parses a member-word line in the speech-material type input file
    '''' </summary>
    '''' <param name="InputLine">The line as an unmodified string</param>
    '''' <param name="WordMetricNames">A list containing the names of all word metrics supplied</param>
    '''' <param name="WordMetricTypes">A list containing the types of all categorical word metrics supplied</param>
    '''' <param name="WML">A word metrics library that in which all input word are looked up. Is set to Nothing, no lookup is performed.</param>
    '''' <returns>Returns a tuple. Item1: Id, Item2: Sound files subfolder, Item3: Maskers sounds file subfolder, Item4: Spelling, Item5: Phonetic form, Item6: Numeric word metrics, Item7: Categorical word metrics</returns>
    'Public Shared Function InputFileTestWordValueParsing(ByVal InputLine As String,
    '                                                     ByVal WordMetricNames As List(Of String),
    '                                                     ByVal WordMetricTypes As List(Of VariableTypes),
    '                                                     ByRef WML As CustomVariablesDatabase) As Tuple(Of String, String, String, String, String, SortedList(Of String, Double), SortedList(Of String, String))


    '    Dim Id As String = ""
    '    Dim SoundFileSubFolder As String = ""
    '    Dim MaskerSoundFileSubFolder As String = ""
    '    Dim Spelling As String = ""
    '    Dim PhoneticTranscription As String = ""
    '    Dim NumericWordMetrics As New SortedList(Of String, Double)
    '    Dim CategoricalWordMetrics As New SortedList(Of String, String)

    '    Const PreWordMetricColumns As Integer = 5

    '    'Getting Id, spelling, transcription, and word metrics available
    '    'Trimming off comments
    '    Dim InputLineSplit() As String = InputLine.Split({"//"}, StringSplitOptions.None) 'A comment can be added after // in the input file
    '    Dim ValueString As String = InputLineSplit(0)
    '    'Gets the values
    '    Dim Values As List(Of String) = ValueString.Split(vbTab).ToList

    '    'Chaing that at least PreWordMetricColumns columns are supplied
    '    If Values.Count < PreWordMetricColumns Then
    '        Throw New Exception("Too few columns in the speech material file at the row: " & ValueString)
    '    End If


    '    'Reads the pre-word meric data
    '    For i = 0 To PreWordMetricColumns - 1

    '        Select Case i
    '            Case 0
    '                'Reads Id
    '                Id = Values(i).Trim

    '            Case 1
    '                'Reads the soundfile subfolder
    '                SoundFileSubFolder = Values(i).Trim

    '            Case 2
    '                'Reads the masker soundfile subfolder
    '                MaskerSoundFileSubFolder = Values(i).Trim

    '            Case 3
    '                'Reads Spelling
    '                Spelling = Values(i).Trim
    '                'Spelling = Spelling.Trim 'converts to lower ' This was done prior to 2022-03-30. Why?

    '            Case 4
    '                'Reads Phonetic transcription
    '                PhoneticTranscription = Values(i).Trim
    '                PhoneticTranscription = PhoneticTranscription.TrimStart("[").Trim
    '                PhoneticTranscription = PhoneticTranscription.TrimEnd("]").Trim

    '                'Supplies default values:
    '                'If the Id column contains only a hyphen "-", a default Id on the form Spelling [ PhoneticTranscription ] is created, and if no transcription exists only the spelling is used.
    '                If Id = "-" Then
    '                    If PhoneticTranscription.Trim = "" Then
    '                        Id = Spelling
    '                    Else
    '                        Id = Spelling & " [ " & PhoneticTranscription.Trim & " ]"
    '                    End If
    '                End If

    '        End Select

    '    Next

    '    'Reads word metrics
    '    'If a word metrics library is supplied, this will be used to lookup the requested word metrics, and all metrics supplied in the speech material file will be ignored.
    '    If WML Is Nothing Then

    '        'This part of the code is used when word metric data is to be supplied directly from the speech material file
    '        For i = PreWordMetricColumns To Values.Count - 1

    '            'Checking that the right number of data is given for each word 
    '            Dim RequiredLength As Integer = PreWordMetricColumns + WordMetricNames.Count
    '            If Values.Count < RequiredLength Then
    '                Throw New Exception("Too few columns in the speech material file at the row: " & ValueString)
    '            End If
    '            If Values.Count > RequiredLength Then
    '                Throw New Exception("Too many columns in the speech material file at the row: " & ValueString)
    '            End If

    '            'Determines the type of word metric supplied
    '            If WordMetricTypes(i - PreWordMetricColumns) = VariableTypes.Numeric Then

    '                'Parses the word metric as a Double and throws an error if parsing was not successful.
    '                Dim NumericValue As Double
    '                Dim NumericValueString As String = Values(i).Trim.Replace(",", ".")
    '                If NumericValueString.Trim = "" Then
    '                    Throw New Exception("Missing numeric " & WordMetricNames(i - PreWordMetricColumns) & " word metric value for the word Id " & Id & " in the speech-material file. (As long as the variable name of a numeric word metrics variable is supplied, values are mandatory for all words!")
    '                End If

    '                If Double.TryParse(NumericValueString, NumericValue) = False Then
    '                    Throw New Exception("Unable to parse the string " & NumericValueString & " as a " & WordMetricNames(i - PreWordMetricColumns) & " word-metric value for the word Id " & Id & " in the speech-material file.")
    '                End If

    '                NumericWordMetrics.Add(WordMetricNames(i - PreWordMetricColumns), NumericValue)

    '            ElseIf WordMetricTypes(i - PreWordMetricColumns) = VariableTypes.Categorical Then

    '                CategoricalWordMetrics.Add(WordMetricNames(i - PreWordMetricColumns), Values(i).Trim)

    '            Else
    '                Throw New Exception("The word metric type " & WordMetricTypes(i - PreWordMetricColumns) & " is not valid (It should be Numeric or Categorical). Please check your speech-material file at Id: " & Id)
    '            End If

    '        Next


    '    Else

    '        'This part of the code is used to supply test words with word metric data
    '        For i = 0 To WordMetricNames.Count - 1

    '            Dim LoopupResult As Object = WML.GetWordMetricValue(Id, WordMetricNames(i))
    '            If LoopupResult = Nothing Then
    '                Throw New Exception("Unable to find the Id " & Id & "in the word metrics library: " & WML.FilePath)
    '            End If

    '            'Determines the type of word metric supplied
    '            If WordMetricTypes(i) = VariableTypes.Numeric Then
    '                NumericWordMetrics.Add(WordMetricNames(i), LoopupResult)
    '            ElseIf WordMetricTypes(i) = VariableTypes.Categorical Then
    '                CategoricalWordMetrics.Add(WordMetricNames(i), LoopupResult)
    '            Else
    '                Throw New Exception("The word metric type " & WordMetricTypes(i) & " is not valid (It should be Numeric or Categorical). Please check your speech-material file and your word metrics library file at Id: " & Id)
    '            End If

    '        Next


    '    End If

    '    Return New Tuple(Of String, String, String, String, String, SortedList(Of String, Double), SortedList(Of String, String))(Id, SoundFileSubFolder, MaskerSoundFileSubFolder, Spelling, PhoneticTranscription, NumericWordMetrics, CategoricalWordMetrics)

    'End Function

#End Region



End Class


Public Enum VariableTypes
    Numeric
    Categorical
    [Boolean]
End Enum
