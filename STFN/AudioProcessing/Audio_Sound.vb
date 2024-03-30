'This software is available under the following license:
'MIT/X11 License
'
'Copyright (c) 2020 Erik Witte
'
'Permission is hereby granted, free of charge, to any person obtaining a copy
'of this software and associated documentation files (the ''Software''), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'copies of the Software, and to permit persons to whom the Software is
'furnished to do so, subject to the following conditions:
'
'The above copyright notice and this permission notice shall be included in all
'copies or substantial portions of the Software.
'
'THE SOFTWARE IS PROVIDED ''AS IS'', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
'SOFTWARE.

Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Xml
Imports System.Globalization.CultureInfo
Imports System.Xml.Serialization

Namespace Audio

    ''' <summary>
    ''' A class used to hold audio data.
    ''' </summary>
    <Serializable>
    Public Class Sound

        Public Description As String = ""

        Private _fileName As String
        Public Property FileName As String
            Get
                Return _fileName
            End Get
            Set(value As String)
                'Removes file extensions
                _fileName = Path.GetFileNameWithoutExtension(value)
            End Set
        End Property

        ''' <summary>
        ''' If the Sound object was read from file, SourcePath should contain the full path of the source audio file.
        ''' </summary>
        Public SourcePath As String = ""

        ''' <summary>
        ''' Holds the wave sample data of the current sound.
        ''' </summary>
        ''' <returns></returns>
        Public Property WaveData As LocalWaveData

        ''' <summary>
        ''' Gets the wave format of the current sound.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property WaveFormat As Formats.WaveFormat

        Private _SMA As SpeechMaterialAnnotation

        ''' <summary>
        ''' Holds SMA type data (Speech Material Annotation)
        ''' </summary>
        ''' <returns></returns>
        Public Property SMA As SpeechMaterialAnnotation
            Get
                Return _SMA
            End Get
            Set(value As SpeechMaterialAnnotation)
                _SMA = value
                If _SMA IsNot Nothing Then _SMA.ParentSound = Me
            End Set
        End Property

        Public Property FFT As FftData

        ''' <summary>
        ''' With the exception of the SMA node, this list contains the raw xml of all iXML sub-nodes read from an audiofile. iXML tags/keywords as keys (xml Name), and data as values (xml InnerXml).
        ''' </summary>
        ''' <returns></returns>
        Public Property iXmlNodes As List(Of Tuple(Of String, String))

        ''' <summary>
        ''' A list that holds wave file chunks that were never parsed, but instead just kept as byte arrays. Thses should never be modified, lest wave file writing will break!
        ''' </summary>
        ''' <returns></returns>
        Private Property UnparsedWaveChunks As New List(Of Byte())


        ''' <summary>
        ''' Creates a new instance of the Sound class.
        ''' </summary>
        ''' <param name="Waveformat"></param>
        ''' <param name="FileName"></param>
        Public Sub New(ByVal Waveformat As Formats.WaveFormat, Optional FileName As String = "")

            Me.FileName = FileName
            Me.WaveFormat = Waveformat
            WaveData = New LocalWaveData(Waveformat.Channels)
            SMA = New SpeechMaterialAnnotation()

            'Adding Waveformat.Channels channels and one sentence in each channel
            For n = 1 To Waveformat.Channels
                SMA.AddChannelData(New Sound.SpeechMaterialAnnotation.SmaComponent(SMA, SpeechMaterialAnnotation.SmaTags.CHANNEL, Nothing))
                SMA.ChannelData(n).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(SMA, SpeechMaterialAnnotation.SmaTags.SENTENCE, SMA.ChannelData(n)))
            Next

        End Sub

        ''' <summary>
        ''' Stores the current stage as the unchanged state. The current state can then later be compared to a following state to determine whether the Wave or SMA data has changed or not.
        ''' </summary>
        Public Sub StoreUnchangedState()
            If Me.SMA IsNot Nothing Then Me.SMA.StoreUnchangedState()
            If Me.WaveData IsNot Nothing Then Me.WaveData.StoreUnchangedState()
        End Sub

        Private ManuallySetIsChangedValue As Boolean?

        ''' <summary>
        ''' The sub can be used to manually override the value returned by IsChanged
        ''' </summary>
        ''' <param name="Value"></param>
        Public Sub SetIsChangedManually(ByVal Value As Boolean?)
            ManuallySetIsChangedValue = Value
        End Sub

        Public Function IsChanged() As Boolean?

            If ManuallySetIsChangedValue.HasValue Then
                Return ManuallySetIsChangedValue
            End If

            Dim SmaIsChanged As Boolean? = Nothing
            Dim WaveDataIsChanged As Boolean? = Nothing

            If SMA IsNot Nothing Then
                SmaIsChanged = SMA.IsChanged
                If SmaIsChanged.HasValue Then
                    If SmaIsChanged = True Then Return True
                End If
            End If
            If WaveData IsNot Nothing Then
                WaveDataIsChanged = WaveData.IsChanged
                If WaveDataIsChanged.HasValue Then
                    If WaveDataIsChanged = True Then Return True
                End If
            End If

            If SmaIsChanged.HasValue = False Or WaveDataIsChanged.HasValue = False Then
                Return Nothing
            Else
                Return False
            End If

        End Function

        Public Sub RemoveUnparsedWaveChunks()
            UnparsedWaveChunks.Clear()
        End Sub


        ''' <summary>
        ''' Creates a new Sound which is a deep copy of the original, by using serialization.
        ''' </summary>
        ''' <returns></returns>
        Public Function CreateCopy() As Sound

            'Creating an output object
            Dim newSound As Sound

            'Serializing to memorystream
            Dim serializedMe As New MemoryStream
            Dim serializer As New XmlSerializer(GetType(Sound))
            serializer.Serialize(serializedMe, Me)

            'Deserializing to new object
            serializedMe.Position = 0
            newSound = CType(serializer.Deserialize(serializedMe), Sound)
            serializedMe.Close()

            'Returning the new object
            Return newSound
        End Function


        ''' <summary>
        ''' Creates a new sound with a copy (not a reference) of the original sample data. The WaveFormat in the new sound, however, is a reference to the wave format in the original sound.
        ''' </summary>
        ''' <returns></returns>
        Public Function CreateSoundDataCopy() As Sound

            'Creating a new sound
            Dim NewSound As New Sound(WaveFormat)

            'Copying sound data
            For c = 1 To WaveFormat.Channels
                Dim NewChannelArray(WaveData.SampleData(c).Length - 1) As Single
                Array.Copy(WaveData.SampleData(c), NewChannelArray, NewChannelArray.Length)
                NewSound.WaveData.SampleData(c) = NewChannelArray
            Next

            'Returning the new sound
            Return NewSound

        End Function

        ''' <summary>
        ''' Copies the sample data of one channel in the original sound to a new sound.
        ''' </summary>
        ''' <param name="Channel"></param>
        ''' <returns></returns>
        Public Function CopyChannelToMonoSound(ByVal Channel As Integer) As Sound

            Dim OutputSound As New Sound(New Formats.WaveFormat(WaveFormat.SampleRate, WaveFormat.BitDepth, 1,, WaveFormat.Encoding), FileName)

            Dim NewChannelArray(WaveData.SampleData(Channel).Length - 1) As Single
            Array.Copy(WaveData.SampleData(Channel), NewChannelArray, NewChannelArray.Length)
            OutputSound.WaveData.SampleData(1) = NewChannelArray

            Return OutputSound

        End Function



        ''' <summary>
        ''' Copies a section of the sample data of one channel in the original sound to a new sound.
        ''' </summary>
        ''' <param name="Channel"></param>
        ''' <returns></returns>
        Public Function CopySection(ByVal Channel As Integer, ByVal StartSample As Integer, ByVal Length As Integer) As Sound

            Dim OutputSound As New Sound(New Formats.WaveFormat(WaveFormat.SampleRate, WaveFormat.BitDepth, 1,, WaveFormat.Encoding), FileName)

            Dim NewChannelArray(Length - 1) As Single
            Array.Copy(WaveData.SampleData(Channel), StartSample, NewChannelArray, 0, Length)
            OutputSound.WaveData.SampleData(1) = NewChannelArray

            Return OutputSound

        End Function


        ''' <summary>
        ''' Creates a multi channel sound out of a mono sound (or the first channel in a multichannel sound).
        ''' </summary>
        ''' <param name="NewChannelCount"></param>
        ''' <returns></returns>
        Public Function ConvertMonoToMultiChannel(Optional NewChannelCount As Integer = 2, Optional ShallowChannelCopies As Boolean = False) As Sound

            Dim OutputSound As New Sound(New Formats.WaveFormat(WaveFormat.SampleRate, WaveFormat.BitDepth, NewChannelCount,, WaveFormat.Encoding))

            If ShallowChannelCopies = True Then
                For c = 1 To NewChannelCount
                    OutputSound.WaveData.SampleData(c) = WaveData.SampleData(1)
                Next
            Else
                For c = 1 To NewChannelCount
                    OutputSound.WaveData.SampleData(c) = WaveData.SampleData(1).Take(WaveData.SampleData(1).Length).ToArray
                Next
            End If

            Return OutputSound

        End Function

        ''' <summary>
        ''' Converts a sound in which the audio data is stored in 16bit PCM to 32 bit IEEE float format.
        ''' </summary>
        ''' <returns></returns>
        Public Function Convert16to32bitSound() As Sound

            Dim NewWaveFormat = New Formats.WaveFormat(Me.WaveFormat.SampleRate, 32, Me.WaveFormat.Channels,, Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints)
            Dim ConvertedSound As New Sound(NewWaveFormat)

            'Copies SMA and UnparsedWaveChunks
            If SMA IsNot Nothing Then ConvertedSound.SMA = SMA.CreateCopy
            If Me.UnparsedWaveChunks IsNot Nothing Then ConvertedSound.UnparsedWaveChunks = Me.UnparsedWaveChunks

            'Copies and converts wave data
            For c = 1 To Me.WaveFormat.Channels
                If Me.WaveData.SampleData(c) IsNot Nothing Then
                    Dim ChannelLength As Integer = Me.WaveData.SampleData(c).Length
                    Dim ConvertedChannelArray(ChannelLength - 1) As Single
                    Dim FS As Single = Me.WaveFormat.PositiveFullScale
                    Array.Copy(Me.WaveData.SampleData(c), ConvertedChannelArray, ChannelLength)
                    For s = 0 To ChannelLength - 1
                        ConvertedChannelArray(s) /= FS
                    Next
                    ConvertedSound.WaveData.SampleData(c) = ConvertedChannelArray
                End If
            Next

            Return ConvertedSound

        End Function

        ''' <summary>
        ''' Creates a new sound converted to the specified format. The SMA object neither copied or referenced.
        ''' </summary>
        ''' <returns></returns>
        Public Function BitDepthConversion(ByRef NewWaveFormat As Formats.WaveFormat) As Sound

            'Setting the conversion factor 
            Dim SampleConversionFactor As Double = 1
            Select Case WaveFormat.BitDepth
                Case 16
                    Select Case NewWaveFormat.BitDepth
                        Case 16
                            'Same, does nothing
                        Case 32
                            SampleConversionFactor = 1 / WaveFormat.PositiveFullScale
                        Case Else
                            Throw New NotImplementedException
                    End Select

                Case 32
                    Select Case NewWaveFormat.BitDepth
                        Case 16
                            SampleConversionFactor = 1 * NewWaveFormat.PositiveFullScale
                        Case 32
                            'Same, does nothing
                        Case Else
                            Throw New NotImplementedException
                    End Select
                Case Else
                    Throw New NotImplementedException
            End Select

            'Creating an output sound
            Dim OutputSound As New Sound(NewWaveFormat)

            For c = 1 To OutputSound.WaveFormat.Channels

                'Copying and converting the sample data
                Dim NewChannelArray(WaveData.SampleData(c).Length - 1) As Single
                OutputSound.WaveData.SampleData(c) = NewChannelArray
                For n = 0 To OutputSound.WaveData.SampleData(c).Length - 1
                    OutputSound.WaveData.SampleData(c)(n) = WaveData.SampleData(1)(n) * SampleConversionFactor
                Next
            Next

            Return OutputSound

        End Function


        ''' <summary>
        ''' Creates a zero-padded copy of the current sound.
        ''' <param name="PreSoundPaddingLength">The length in samples of the silence to be added prior to the existing sound.</param>
        ''' <param name="PostSoundPaddingLength">The length in samples of the silence to be added after the existing sound.</param> 
        ''' <param name="CreateNewSound">If set to false, nothing will be returned, but instead the original sound will be zero padded.</param>
        ''' </summary>
        ''' <returns>Returns a new sound, which is a zero-padded copy of the current sound.</returns>
        Public Function ZeroPad(Optional ByVal PreSoundPaddingLength As Integer? = Nothing, Optional ByVal PostSoundPaddingLength As Integer? = Nothing, Optional ByVal CreateNewSound As Boolean = False) As Sound

            'Determining the length of the pre sound panning
            If PreSoundPaddingLength Is Nothing Then PreSoundPaddingLength = 0

            'Determining the length of the post sound panning
            If PostSoundPaddingLength Is Nothing Then PostSoundPaddingLength = 0

            Dim OutputSound As Sound = Nothing
            If CreateNewSound = True Then
                'Creates a copy
                OutputSound = Me.CreateCopy
            End If

            For c = 1 To WaveFormat.Channels
                'Create new sound channel arrays
                Dim NewChannelArray(PreSoundPaddingLength + WaveData.SampleData(c).Length + PostSoundPaddingLength - 1) As Single

                'Copying the sound from the current instance to the appropriate place in the new channel array
                For s = 0 To WaveData.SampleData(c).Length - 1
                    NewChannelArray(s + PreSoundPaddingLength) = WaveData.SampleData(c)(s)
                Next

                If CreateNewSound = True Then
                    'Adding the new channel array
                    OutputSound.WaveData.SampleData(c) = NewChannelArray
                Else
                    'Replacing the sound array in the original sound
                    WaveData.SampleData(c) = NewChannelArray
                End If

            Next

            If CreateNewSound = True Then

                'Returning the new sound
                Return OutputSound
            Else
                'Returning the current instance of Sound.
                Return Me
            End If

        End Function


        ''' <summary>
        ''' Creates a zero-padded copy of the current sound.
        ''' <param name="PreSoundPadding">The length in seconds of the silence to be added prior to the existing sound.</param>
        ''' <param name="PostSoundPadding">The length in seconds of the silence to be added after the existing sound.</param> 
        ''' <param name="CreateNewSound">If set to false, nothing will be returned, but instead the original sound will be zero padded.</param>
        ''' </summary>
        ''' <returns>Returns a new sound, which is a zero-padded copy of the current sound.</returns>
        Public Function ZeroPad(Optional ByVal PreSoundPadding As Double? = Nothing, Optional ByVal PostSoundPadding As Double? = Nothing, Optional ByVal CreateNewSound As Boolean = False) As Sound

            'Determining the length of the pre sound panning
            Dim PreSoundPaddingLength As Integer = 0
            If PreSoundPadding IsNot Nothing Then
                PreSoundPaddingLength = PreSoundPadding * WaveFormat.SampleRate
            End If

            'Determining the length of the post sound panning
            Dim PostSoundPaddingLength As Integer = 0
            If PostSoundPadding IsNot Nothing Then
                PostSoundPaddingLength = PostSoundPadding * WaveFormat.SampleRate
            End If

            'Returns the new sound, by calling ZeroPad with sample parameters
            Return ZeroPad(PreSoundPaddingLength, PostSoundPaddingLength, CreateNewSound)

        End Function

        ''' <summary>
        ''' Creates copy of the current sound which is zero-padded to a certain duration (in seconds).
        ''' <param name="TotalDuration">The duration in seconds to which the sound should be zero padded. If the sound is longer than this duration, no padding is done, but instead a copy of the original sound will be returned.</param>
        ''' <param name="CreateNewSound">If set to false, nothing will be returned, but instead the original sound will be zero padded.</param>
        ''' </summary>
        ''' <returns>Returns a new sound, which is a zero-padded copy of the current sound.</returns>
        Public Function ZeroPad(ByVal TotalDuration As Double, Optional ByVal CreateNewSound As Boolean = False) As Sound

            Dim TotalLength As Integer = TotalDuration * WaveFormat.SampleRate
            Dim PaddingLength As Integer = TotalLength - WaveData.ShortestChannelSampleCount

            If PaddingLength <= 0 Then
                If CreateNewSound = True Then
                    Return Me.CreateCopy
                Else
                    Return Nothing
                End If
            Else
                Return ZeroPad(0, PaddingLength, CreateNewSound)
            End If

        End Function

        ''' <summary>
        ''' Creates copy of the current sound which is zero-padded to a certain length (in samples).
        ''' <param name="TotalLength">The length in samples to which the sound should be zero padded. If the shortest channel of the sound is longer than this length, no padding is done, but instead a copy of the original sound will be returned.</param>
        ''' <param name="CreateNewSound">If set to false, nothing will be returned, but instead the original sound will be zero padded.</param>
        ''' </summary>
        ''' <returns>Returns a new sound, which is a zero-padded copy of the current sound.</returns>
        Public Function ZeroPad(ByVal TotalLength As Integer, Optional ByVal CreateNewSound As Boolean = False) As Sound

            Dim PaddingLength As Integer = TotalLength - WaveData.ShortestChannelSampleCount

            If PaddingLength <= 0 Then
                If CreateNewSound = True Then
                    Return Me.CreateCopy
                Else
                    Return Nothing
                End If
            Else
                Return ZeroPad(0, PaddingLength, CreateNewSound)
            End If

        End Function

        ''' <summary>
        ''' Inverts the sound in the indicated channel. Throws an error if the indicated channel number is higher than the channel count in the WaveFormat object of the sound.
        ''' </summary>
        ''' <param name="Channel"></param>
        Public Sub InvertChannel(ByVal Channel As Integer)

            If Channel < 1 Then Throw New ArgumentException("The channel value cannot be lower than 1!")
            If Channel > WaveFormat.Channels Then Throw New ArgumentException("The current sound object does not have " & Channel & " channels!")

            Dim ArrayToInvert = WaveData.SampleData(Channel)
            For s = 0 To ArrayToInvert.Length - 1
                ArrayToInvert(s) = -ArrayToInvert(s)
            Next

        End Sub


        <Serializable>
        Class LocalWaveData

            <NonSerialized>
            Private ChangeDetector As Utils.ObjectChangeDetector
            Public Sub StoreUnchangedState()
                ChangeDetector = New Utils.ObjectChangeDetector(Me)
                ChangeDetector.SetUnchangedState()
            End Sub

            Public Function IsChanged() As Boolean?
                If ChangeDetector IsNot Nothing Then
                    Return ChangeDetector.IsChanged()
                Else
                    Return Nothing
                End If
            End Function

            Private _sampleData As List(Of Single())

            ''' <summary>
            ''' Determines whether some none empty channels in the current instance of LocalWaveData has unequal lengths.
            ''' </summary>
            ''' <returns>Returns true if unequal nonempty channel lengths were detected, and false if not.</returns>
            Public Function HasUnequalNonZeroChannelLength() As Boolean

                If _sampleData.Count = 0 Then Return False

                Dim LastNonZeroChannelLength As Integer
                Dim NonZeroLengthDetected As Boolean = False
                For i = 0 To _sampleData.Count - 1

                    'Skips if length is zero
                    If _sampleData(i).Length = 0 Then Continue For

                    'Stores the length if its the first non-zero length detected
                    If NonZeroLengthDetected = False Then
                        LastNonZeroChannelLength = _sampleData(i).Length
                        NonZeroLengthDetected = True
                        Continue For
                    End If

                    'Compares all other cases to the last non-zero length detected
                    If _sampleData(i).Length <> LastNonZeroChannelLength Then
                        Return True
                    Else
                        LastNonZeroChannelLength = _sampleData(i).Length
                    End If
                Next

                Return False

            End Function

            Public ReadOnly Property LongestChannelSampleCount As Integer
                Get
                    Return DetermineLongestChannelLength()
                End Get
            End Property

            Public ReadOnly Property ShortestChannelSampleCount As Integer
                Get
                    Return DetermineShortestChannelLength()
                End Get
            End Property


            ''' <summary>
            ''' Gets or sets or the sample data of the specified channel (Channel indices are 1-based).
            ''' </summary>
            ''' <param name="channel"></param>
            ''' <returns></returns>
            Public Property SampleData(Optional ByVal channel As Integer = 1) As Single()
                Get
                    Return _sampleData(channel - 1)
                End Get
                Set(value As Single())
                    _sampleData(channel - 1) = value
                End Set
            End Property


            Private Function DetermineLongestChannelLength()

                'Determining max length
                Dim maxLength As Integer = 0
                For c = 0 To _sampleData.Count - 1
                    If _sampleData(c).Length > maxLength Then maxLength = _sampleData(c).Length
                Next
                Return maxLength

            End Function


            Private Function DetermineShortestChannelLength()

                'Determining min length
                Dim minLength As Integer = Integer.MaxValue
                If _sampleData.Count = 0 Then
                    minLength = 0
                Else
                    For c = 0 To _sampleData.Count - 1
                        If _sampleData(c).Length < minLength Then minLength = _sampleData(c).Length
                    Next
                End If
                Return minLength

            End Function

            ''' <summary>
            '''Enforcing equal channel length, by extending all channels to the length of the longest
            ''' </summary>
            Public Sub EnforceEqualChannelLength()

                'Determining max length
                Dim maxLength As Integer = LongestChannelSampleCount

                'Setting all channel arrays to the max length, using ReDim Preserve
                For c = 0 To _sampleData.Count - 1
                    If _sampleData(c).Length < maxLength Then
                        ReDim Preserve _sampleData(c)(maxLength - 1)
                    End If
                Next

            End Sub

            ''' <summary>
            ''' Determines whether all channels have equal length.
            ''' </summary>
            ''' <returns>Returns False if unequal length is detected, or True if all channel lengths are equal or no channels exist.</returns>
            Public Function IsEqualChannelLength() As Boolean

                Dim ChannelLengthList As New List(Of Integer)

                For c = 0 To _sampleData.Count - 1
                    ChannelLengthList.Add(_sampleData(c).Length)
                Next

                If ChannelLengthList.Count > 0 Then
                    Return ChannelLengthList.All(Function(x) x = ChannelLengthList(0))
                Else
                    'Returns true if no channels exist
                    Return True
                End If

            End Function

            ''' <summary>
            ''' Creates a new instance of LocalWaveData
            ''' </summary>
            ''' <param name="Channels">The number of audio channels to initialize.</param>
            Public Sub New(ByVal Channels As Integer)

                _sampleData = New List(Of Single())
                For n = 0 To Channels - 1
                    Dim ChannelSoundData As Single() = {}
                    _sampleData.Add(ChannelSoundData)
                Next

            End Sub

        End Class


#Region "IO"


        ''' <summary>
        ''' Reads a sound (.wav or .ptwf) from file and stores it in a new Sounds object.
        ''' </summary>
        ''' <param name="filePath">The file path to the file to read. If left empty a open file dialogue box will appear.</param>
        ''' <param name="startReadTime"></param>
        ''' <param name="stopReadTime"></param>
        ''' <param name="inputTimeFormat"></param>
        ''' <param name="StoreSourcePath">If set to True, the full path of the audio file from which the Sound object was created is stored in Sound.SourcePath.</param>
        ''' <returns>Returns a new Sound containing the sound data from the input sound file.</returns>
        Public Shared Function LoadWaveFile_Original(ByVal filePath As String,
                                            Optional ByVal startReadTime As Decimal = 0,
                                            Optional ByVal stopReadTime As Decimal = 0,
                                            Optional ByVal inputTimeFormat As TimeUnits = TimeUnits.seconds,
                                            Optional ByVal StoreSourcePath As Boolean = True,
                                            Optional ByVal StoreUnchangedState As Boolean = False) As Sound

            Try

                Dim fileName As String = ""

                'Finds out the filename
                If Not filePath = "" Then fileName = Path.GetFileNameWithoutExtension(filePath)

                'Creates a variable to hold data chunk size
                Dim dataSize As UInteger = 0

                Dim fileStreamRead As FileStream = New FileStream(filePath, FileMode.Open)
                Dim reader As BinaryReader = New BinaryReader(fileStreamRead, Text.Encoding.UTF8)

                'Starts reading

                Dim chunkID As String = reader.ReadChars(4)
                Dim fileSize As UInteger = reader.ReadUInt32
                Dim riffType As String = reader.ReadChars(4)
                'Abort if riffType is not WAVE
                If Not riffType = "WAVE" Then
                    Throw New Exception("The file is not a wave-file!")
                End If

                Dim fmtID As String
                Dim fmtSize As UInteger
                Dim fmtCode As UShort
                Dim channels As UShort
                Dim sampleRate As UInteger
                Dim fmtAvgBPS As UInteger
                Dim fmtBlockAlign As UShort
                Dim bitDepth As UShort

                Dim sound As Sound = Nothing
                Dim FormatChunkIsRead As Boolean = False 'THis variable is used to ensure that the format chunk is read before the ptwf and the data chunks.

                Dim UnparsedWaveChunks As New List(Of Byte())

                'Chunks to ignore
                Dim dataChunkFound As Boolean
                While dataChunkFound = False

                    Dim IDOfNextChunk As String = reader.ReadChars(4)
                    Dim sizeOfNextChunk As UInteger = reader.ReadUInt32
                    Select Case IDOfNextChunk

                        Case "fmt "

                            Dim fmtChunkStartPosition As Integer = reader.BaseStream.Position

                            ' Reading the format chunk (not all data is stored)
                            fmtID = IDOfNextChunk ' reader.ReadChars(4)
                            fmtSize = sizeOfNextChunk ' reader.ReadUInt32
                            fmtCode = reader.ReadUInt16
                            channels = reader.ReadUInt16
                            sampleRate = reader.ReadUInt32
                            fmtAvgBPS = reader.ReadUInt32
                            fmtBlockAlign = reader.ReadUInt16
                            bitDepth = reader.ReadUInt16

                            sound = New Sound(New Formats.WaveFormat(sampleRate, bitDepth, channels,, fmtCode))

                            'Stores the source file path
                            If StoreSourcePath = True Then
                                sound.SourcePath = filePath
                            End If

                            'Checks to see if the whole of subchunk1 has been read
                            While reader.BaseStream.Position < fmtChunkStartPosition + fmtSize
                                reader.ReadByte()
                            End While

                            'Noting that the format chunk is read
                            FormatChunkIsRead = True


                        Case "iXML"

                            Dim iXMLDataStartReadPosition As Integer = reader.BaseStream.Position

                            'Copying iXML data to a new stream
                            Dim iXMLStream As New MemoryStream
                            For s = 0 To sizeOfNextChunk - 1
                                iXMLStream.WriteByte(fileStreamRead.ReadByte)
                            Next
                            iXMLStream.Position = 0

                            'Parsing the iXML data
                            Dim iXMLdata = ParseiXMLString(iXMLStream)

                            'Storing the data
                            If iXMLdata.Item1 IsNot Nothing Then
                                sound.SMA = iXMLdata.Item1
                            End If
                            If iXMLdata.Item2 IsNot Nothing Then
                                sound.iXmlNodes = iXMLdata.Item2
                            End If

                            'Checks if a padding byte needs to be read
                            Dim currentBaseStreamPosition As Integer = reader.BaseStream.Position
                            If Not currentBaseStreamPosition Mod 2 = 0 Then
                                reader.ReadByte()
                            End If

                        Case "data"

                            'Aborting if the format chink has not yet been read
                            If FormatChunkIsRead = False Then
                                AudioError("The wave file has an unsupported internal structure.")
                                Return Nothing
                            End If

                            dataChunkFound = True
                            dataSize = sizeOfNextChunk

                        Case Else
                            Dim SizeOfUnknownChunk As UInteger = sizeOfNextChunk

                            'Reads to the end of the chunk but does not save the data
                            'Stores the unknown chunk so that it can be retained upon save
                            UnparsedWaveChunks.Add(reader.ReadBytes(SizeOfUnknownChunk))

                            'Reads any padding bytes
                            If SizeOfUnknownChunk Mod 2 = 1 Then
                                reader.ReadByte()
                            End If

                    End Select

                End While

                'Stores any detected UnparsedWaveChunks 
                sound.UnparsedWaveChunks = UnparsedWaveChunks

                Dim startReadDataPoint As Integer
                Dim stopReadDataPoint As Integer

                Select Case inputTimeFormat
                    Case TimeUnits.seconds
                        startReadDataPoint = startReadTime * sound.WaveFormat.SampleRate * sound.WaveFormat.Channels
                        stopReadDataPoint = stopReadTime * sound.WaveFormat.SampleRate * sound.WaveFormat.Channels

                    Case TimeUnits.samples
                        startReadDataPoint = startReadTime * sound.WaveFormat.Channels
                        stopReadDataPoint = stopReadTime * sound.WaveFormat.Channels

                End Select

                Dim soundIndexOfDataPoints As Integer = dataSize / (sound.WaveFormat.BitDepth / 8)

                If stopReadTime = 0 Then
                    stopReadDataPoint = soundIndexOfDataPoints - 1
                End If

                If stopReadDataPoint > soundIndexOfDataPoints Then
                    stopReadDataPoint = soundIndexOfDataPoints - 1
                End If

                Dim numberOfDataPointsToRead As Integer = stopReadDataPoint + 1 - startReadDataPoint
                Dim soundDataArray(numberOfDataPointsToRead - 1) As Double

                If numberOfDataPointsToRead > 0 Then
                    Select Case sound.WaveFormat.Encoding
                        Case = Formats.WaveFormat.WaveFormatEncodings.PCM
                            Select Case sound.WaveFormat.BitDepth
                                Case 16
                                    For n = 0 To startReadDataPoint - 1
                                        reader.ReadInt16()
                                    Next
                                    For n = startReadDataPoint To stopReadDataPoint '- 1?
                                        soundDataArray(n - startReadDataPoint) = reader.ReadInt16()
                                        'MsgBox("Reading" & n - startReadDataPoint & " " & soundDataArray(n - startReadDataPoint))
                                    Next

                                Case 32
                                    For n = 0 To startReadDataPoint - 1
                                        reader.ReadInt32()
                                    Next
                                    For n = startReadDataPoint To stopReadDataPoint '- 1?
                                        soundDataArray(n - startReadDataPoint) = reader.ReadInt32()
                                        'MsgBox("Reading" & n - startReadDataPoint & " " & soundDataArray(n - startReadDataPoint))
                                    Next

                                Case Else
                                    Throw New NotImplementedException("Reading of " & sound.WaveFormat.BitDepth & " bits PCM format is not yet supported.")
                            End Select
                        Case = Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints
                            Select Case sound.WaveFormat.BitDepth
                                Case 32
                                    For n = 0 To startReadDataPoint - 1
                                        reader.ReadSingle()
                                    Next
                                    For n = startReadDataPoint To stopReadDataPoint '- 1?
                                        soundDataArray(n - startReadDataPoint) = reader.ReadSingle()
                                        'MsgBox("Reading" & n - startReadDataPoint & " " & soundDataArray(n - startReadDataPoint))
                                    Next
                                Case Else
                                    Throw New NotImplementedException("Reading of " & sound.WaveFormat.BitDepth & " bits IEEE floating points format is not yet supported.")
                            End Select
                    End Select

                Else
                    If numberOfDataPointsToRead < 0 Then Throw New Exception("The number of data points to read was below zero.")
                End If

                fileStreamRead.Close()

                'Dim  As Integer = sound.waveFormat.channels
                If Not numberOfDataPointsToRead Mod channels = 0 Then Throw New Exception("ReadWaveFile detected unequal number of samples between the channels.")
                Dim numberofDataPointsIneachChannelarray = (numberOfDataPointsToRead / channels)

                For c = 1 To channels
                    Dim channelData((numberofDataPointsIneachChannelarray) - 1) As Single

                    If numberOfDataPointsToRead > channels Then
                        Dim counter As Integer = 0
                        For n = c - 1 To soundDataArray.Length - 1 Step channels
                            channelData(counter) = soundDataArray(n)
                            'MsgBox("Sorting channel " & c & counter & " " & channelData(counter))
                            counter += 1
                        Next
                    Else
                        If numberOfDataPointsToRead < 0 Then Throw New Exception("The number of data points to read was below zero.")
                    End If

                    sound.WaveData.SampleData(c) = channelData

                Next

                'Adding the input file name
                sound.FileName = fileName

                'Storing the read version as serialized
                If StoreUnchangedState = True Then sound.StoreUnchangedState()

                Return sound

            Catch ex As Exception
                AudioError(ex.ToString)
                Return Nothing
            End Try

        End Function

        ''' <summary>
        ''' Reads a sound (.wav or .ptwf) from file and stores it in a new Sounds object.
        ''' </summary>
        ''' <param name="filePath">The file path to the file to read. If left empty a open file dialogue box will appear.</param>
        ''' <param name="startReadTime"></param>
        ''' <param name="stopReadTime"></param>
        ''' <param name="inputTimeFormat"></param>
        ''' <param name="StoreSourcePath">If set to True, the full path of the audio file from which the Sound object was created is stored in Sound.SourcePath.</param>
        ''' <returns>Returns a new Sound containing the sound data from the input sound file.</returns>
        Public Shared Function LoadWaveFile(ByVal filePath As String,
                                            Optional ByVal StoreSourcePath As Boolean = True,
                                            Optional ByVal StoreUnchangedState As Boolean = False) As Sound

            Try

                Dim fileName As String = ""

                'Finds out the filename
                If Not filePath = "" Then fileName = Path.GetFileNameWithoutExtension(filePath)

                'Creates a variable to hold data chunk size
                Dim dataSize As UInteger = 0

                Dim fileStreamRead As FileStream = New FileStream(filePath, FileMode.Open)
                Dim reader As BinaryReader = New BinaryReader(fileStreamRead, Text.Encoding.UTF8)

                'Starts reading

                Dim chunkID As String = reader.ReadChars(4)
                Dim fileSize As UInteger = reader.ReadUInt32
                Dim riffType As String = reader.ReadChars(4)
                'Abort if riffType is not WAVE
                If Not riffType = "WAVE" Then
                    Throw New Exception("The file is not a wave-file!")
                End If

                Dim fmtID As String
                Dim fmtSize As UInteger
                Dim fmtCode As UShort
                Dim channels As UShort
                Dim sampleRate As UInteger
                Dim fmtAvgBPS As UInteger
                Dim fmtBlockAlign As UShort
                Dim bitDepth As UShort

                Dim sound As Sound = Nothing
                Dim FormatChunkIsRead As Boolean = False 'THis variable is used to ensure that the format chunk is read before the ptwf and the data chunks.

                Dim UnparsedWaveChunks As New List(Of Byte())

                'Chunks to ignore
                Dim dataChunkFound As Boolean
                While dataChunkFound = False

                    Dim IDOfNextChunk As String = reader.ReadChars(4)
                    Dim sizeOfNextChunk As UInteger = reader.ReadUInt32
                    Select Case IDOfNextChunk

                        Case "fmt "

                            Dim fmtChunkStartPosition As Integer = reader.BaseStream.Position

                            ' Reading the format chunk (not all data is stored)
                            fmtID = IDOfNextChunk ' reader.ReadChars(4)
                            fmtSize = sizeOfNextChunk ' reader.ReadUInt32
                            fmtCode = reader.ReadUInt16
                            channels = reader.ReadUInt16
                            sampleRate = reader.ReadUInt32
                            fmtAvgBPS = reader.ReadUInt32
                            fmtBlockAlign = reader.ReadUInt16
                            bitDepth = reader.ReadUInt16

                            sound = New Sound(New Formats.WaveFormat(sampleRate, bitDepth, channels,, fmtCode))

                            'Stores the source file path
                            If StoreSourcePath = True Then
                                sound.SourcePath = filePath
                            End If

                            'Checks to see if the whole of subchunk1 has been read
                            While reader.BaseStream.Position < fmtChunkStartPosition + fmtSize
                                reader.ReadByte()
                            End While

                            'Noting that the format chunk is read
                            FormatChunkIsRead = True


                        Case "iXML"

                            Dim iXMLDataStartReadPosition As Integer = reader.BaseStream.Position

                            'Copying iXML data to a new stream
                            Dim iXMLStream As New MemoryStream
                            For s = 0 To sizeOfNextChunk - 1
                                iXMLStream.WriteByte(fileStreamRead.ReadByte)
                            Next
                            iXMLStream.Position = 0

                            'Parsing the iXML data
                            Dim iXMLdata = ParseiXMLString(iXMLStream)

                            'Storing the data
                            If iXMLdata.Item1 IsNot Nothing Then
                                sound.SMA = iXMLdata.Item1
                            End If
                            If iXMLdata.Item2 IsNot Nothing Then
                                sound.iXmlNodes = iXMLdata.Item2
                            End If

                            'Checks if a padding byte needs to be read
                            Dim currentBaseStreamPosition As Integer = reader.BaseStream.Position
                            If Not currentBaseStreamPosition Mod 2 = 0 Then
                                reader.ReadByte()
                            End If

                        Case "data"

                            'Aborting if the format chink has not yet been read
                            If FormatChunkIsRead = False Then
                                AudioError("The wave file has an unsupported internal structure.")
                                Return Nothing
                            End If

                            dataChunkFound = True
                            dataSize = sizeOfNextChunk

                        Case Else
                            Dim SizeOfUnknownChunk As UInteger = sizeOfNextChunk

                            'Reads to the end of the chunk but does not save the data
                            'Stores the unknown chunk so that it can be retained upon save
                            UnparsedWaveChunks.Add(reader.ReadBytes(SizeOfUnknownChunk))

                            'Reads any padding bytes
                            If SizeOfUnknownChunk Mod 2 = 1 Then
                                reader.ReadByte()
                            End If

                    End Select

                End While

                'Stores any detected UnparsedWaveChunks 
                sound.UnparsedWaveChunks = UnparsedWaveChunks

                'Reads sound sample data
                'Reads the sound bytes
                Dim SoundDataBuffer(dataSize - 1) As Byte
                reader.BaseStream.ReadExactly(SoundDataBuffer, 0, dataSize)

                'Closing the files stream
                fileStreamRead.Close()

                Dim InterleavedSoundArray() As Single

                If dataSize > 0 Then
                    Select Case sound.WaveFormat.Encoding
                        Case = Formats.WaveFormat.WaveFormatEncodings.PCM
                            Select Case sound.WaveFormat.BitDepth
                                Case 16

                                    'Converts the byte array to the approproate format
                                    Dim TempInterleavedSoundArray(SoundDataBuffer.Length / 2 - 1) As Short
                                    Buffer.BlockCopy(SoundDataBuffer, 0, TempInterleavedSoundArray, 0, 2 * (Math.Floor(SoundDataBuffer.Length / 2)))

                                    'Copies the data to an array of Single
                                    Dim TempSingleArray(TempInterleavedSoundArray.Length - 1) As Single
                                    For i = 0 To TempSingleArray.Length - 1
                                        TempSingleArray(i) = TempInterleavedSoundArray(i)
                                    Next
                                    InterleavedSoundArray = TempSingleArray

                                Case 32

                                    'Converts the byte array to the approproate format
                                    Dim TempInterleavedSoundArray(SoundDataBuffer.Length / 4 - 1) As Int32
                                    Buffer.BlockCopy(SoundDataBuffer, 0, TempInterleavedSoundArray, 0, 4 * (Math.Floor(SoundDataBuffer.Length / 4)))
                                    'Copies the data to an array of Single
                                    Dim TempSingleArray(TempInterleavedSoundArray.Length - 1) As Single
                                    For i = 0 To TempSingleArray.Length - 1
                                        TempSingleArray(i) = TempInterleavedSoundArray(i)
                                    Next
                                    InterleavedSoundArray = TempSingleArray
                                Case Else
                                    Throw New NotImplementedException("Reading of " & sound.WaveFormat.BitDepth & " bits PCM format is not yet supported.")
                            End Select
                        Case = Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints
                            Select Case sound.WaveFormat.BitDepth
                                Case 32

                                    'Converts the byte array to the approproate format
                                    Dim TempInterleavedSoundArray(SoundDataBuffer.Length / 4 - 1) As Single
                                    Buffer.BlockCopy(SoundDataBuffer, 0, TempInterleavedSoundArray, 0, 4 * (Math.Floor(SoundDataBuffer.Length / 4)))
                                    InterleavedSoundArray = TempInterleavedSoundArray
                                Case Else
                                    Throw New NotImplementedException("Reading of " & sound.WaveFormat.BitDepth & " bits IEEE floating points format is not yet supported.")
                            End Select
                        Case Else
                            Throw New NotImplementedException("Unsupported sound file format.")
                    End Select

                    If sound.WaveFormat.Channels = 1 Then
                        'Storing the InterleavedSoundArray right away as it's not actually interleaved (since it's only one channel)
                        sound.WaveData.SampleData(1) = InterleavedSoundArray

                    Else

                        'Deinterleaving data
                        Dim ChannelLength As Integer = InterleavedSoundArray.Length / sound.WaveFormat.Channels
                        If InterleavedSoundArray.Length Mod sound.WaveFormat.Channels <> 0 Then
                            ChannelLength -= 1
                        End If

                        Dim ConcatenatedChannelArrays(sound.WaveFormat.Channels * ChannelLength - 1) As Single
                        Utils.Math.DeinterleaveSoundArray(InterleavedSoundArray, sound.WaveFormat.Channels, ChannelLength, ConcatenatedChannelArrays)

                        For c As Integer = 1 To channels
                            Dim channelData(ChannelLength - 1) As Single
                            Array.Copy(ConcatenatedChannelArrays, (c - 1) * ChannelLength, channelData, 0, ChannelLength)
                            sound.WaveData.SampleData(c) = channelData
                        Next

                    End If

                End If

                'Adding the input file name
                sound.FileName = fileName

                'Storing the read version as serialized
                If StoreUnchangedState = True Then sound.StoreUnchangedState()

                Return sound

            Catch ex As Exception
                AudioError(ex.ToString)
                Return Nothing
            End Try

        End Function


        ''' <summary>
        ''' Saves the current instance of Sound to a wave file.
        ''' </summary>
        ''' <param name="filePath">The filepath where the file should be saved. If left empty a windows forms save file dialogue box will appaear,
        ''' in which the user may enter file name and storing location.</param>
        ''' <param name="startSample">This parameter enables saving of only a part of the file. StartSample indicates the first sample to be saved.</param>
        ''' <param name="length">This parameter enables saving of only a part of the file. Length indicates the length in samples of the file to be saved.</param>
        ''' <returns>Returns true if save succeded, and False if save failed.</returns>
        Public Function WriteWaveFile(Optional ByRef filePath As String = "",
                                       Optional ByVal startSample As Integer = Nothing, Optional ByVal length As Integer? = Nothing,
                                       Optional CreatePath As Boolean = True) As Boolean

            'Ensures that the extension is .wav
            Dim FileExtension As String = Path.GetExtension(filePath)
            If FileExtension <> ".wav" Then
                filePath = filePath.TrimEnd(FileExtension)
                filePath &= ".wav"
            End If

            'Writes the wave data to a memory stream
            Dim WaveStream = WriteWaveFileStream(startSample, length)

            If WaveStream IsNot Nothing Then
                WaveStream.Position = 0
                Try

                    'Creating the path if it doesn't exist
                    If CreatePath = True Then
                        Dim TempDirectory As String = IO.Path.GetDirectoryName(filePath)
                        If Not IO.Directory.Exists(TempDirectory) Then IO.Directory.CreateDirectory(TempDirectory)
                    End If

                    'Saves the file
                    Dim theFile As FileStream = File.Create(filePath)
                    WaveStream.WriteTo(theFile)
                    theFile.Close()

                    Return True
                Catch ex As Exception
                    AudioError("Error saving to wave file:" & vbCrLf & ex.ToString)
                    Return False
                End Try
            Else
                AudioError("Error saving to wave file.")
                Return False
            End If

        End Function


        ''' <summary>
        ''' Writes the current Sound to a MemoryStream structures as a RIFF WAVE file.
        ''' </summary>
        ''' <param name="startSample"></param>
        ''' <param name="length"></param>
        ''' <param name="checkForDistorsion"></param>
        ''' <returns></returns>
        Public Function WriteWaveFileStream(Optional ByVal startSample As Integer = Nothing, Optional ByVal length As Integer? = Nothing,
                                         Optional checkForDistorsion As Boolean = True) As MemoryStream

            Try

                'Checking that bit depth and encoding is supported before starting to write
                Select Case WaveFormat.Encoding
                    Case Formats.WaveFormat.WaveFormatEncodings.PCM
                        Select Case WaveFormat.BitDepth
                            Case 8, 16
                            Case Else
                                Throw New NotImplementedException("Writing of " & WaveFormat.BitDepth & " bits PCM format is not yet supported.")
                        End Select
                    Case Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints
                        Select Case WaveFormat.BitDepth
                            Case 32
                            Case Else
                                Throw New NotImplementedException("Writing of " & WaveFormat.BitDepth & " bits IEEE floating point format is not yet supported.")
                        End Select
                    Case Else
                        Throw New NotImplementedException("Writing of the format " & WaveFormat.Encoding.ToString & " is not yet supported.")
                End Select

                'Enforcing equal channel length
                WaveData.EnforceEqualChannelLength()

                'And checking the values of startSample and length
                CheckAndCorrectSectionLength(WaveData.ShortestChannelSampleCount, startSample, length)

                'Writing the sound stream
                Dim WaveStream As New MemoryStream
                Dim writer As BinaryWriter = New BinaryWriter(WaveStream, Text.Encoding.UTF8)

                ' Write the header
                writer.Write(WaveFormat.MainChunkID.ToCharArray)
                Dim fileSize As UInteger = 0
                writer.Write(fileSize)
                writer.Write(WaveFormat.RiffType.ToCharArray)

                ' Write the format chunk
                writer.Write(WaveFormat.FmtID.ToCharArray)
                writer.Write(WaveFormat.FmtSize)
                writer.Write(WaveFormat.FmtCode)
                writer.Write(WaveFormat.Channels)
                writer.Write(WaveFormat.SampleRate)
                writer.Write(WaveFormat.FmtAvgBPS)
                writer.Write(WaveFormat.FmtBlockAlign)
                writer.Write(WaveFormat.BitDepth)

                Dim HasiXmlNodes As Boolean = False
                If iXmlNodes IsNot Nothing Then
                    If iXmlNodes.Count > 0 Then
                        HasiXmlNodes = True
                    End If
                End If

                'Writing iXML chunk
                If (HasiXmlNodes = True) Or SMA IsNot Nothing Then

                    'Noting start position
                    Dim iXMLChunkStartByte As Integer = writer.BaseStream.Position

                    'Writes the chunk head
                    writer.Write("iXML".ToCharArray)
                    Dim TempSize As UInteger = 0
                    writer.Write(TempSize)

                    'Writes the iXML to the base stream
                    If WriteiXmlToStream(WaveStream) = True Then

                        Dim iXMLChunkEndByte As Integer = writer.BaseStream.Position

                        'Writing chunk-final zero-padding
                        Dim currentBaseStreamPosition As Integer = writer.BaseStream.Position
                        If Not currentBaseStreamPosition Mod 2 = 0 Then
                            Dim zeroPad As Byte = 0
                            writer.Write(zeroPad)
                        End If

                        Dim iXMLChunkEndByte_InklPadding As Integer = writer.BaseStream.Position

                        'Seeking position for iXML chunk size, calculating iXML chunk size, and then writes it to the stream
                        writer.Seek(iXMLChunkStartByte + 4, SeekOrigin.Begin)
                        Dim iXmlChunkSize As UInteger = 0
                        iXmlChunkSize = iXMLChunkEndByte - iXMLChunkStartByte - 8  'According to the RIFF specification size should not include any zero-padding bytes. But since this does not open in some wave readers, the zero-pad bytes are included in the chunk size here.
                        writer.Write(iXmlChunkSize)

                        'Going back to the current position, to continue writing the next chunk
                        writer.Seek(iXMLChunkEndByte_InklPadding, SeekOrigin.Begin)

                    Else
                        'If for some reason writing of iXML failed, the stream rolls back so that any data already written to the stream by WriteiXmlToStream can be overwritten
                        writer.Seek(iXMLChunkStartByte, SeekOrigin.Begin)

                    End If
                End If

                'Adds any unparsed chunks that were read from wave file (they need to be completely unmodified for write to work!)
                If UnparsedWaveChunks IsNot Nothing Then
                    For n = 0 To UnparsedWaveChunks.Count - 1

                        writer.Write(UnparsedWaveChunks(n))

                        'Writing chunk-final zero-padding
                        Dim currentBaseStreamPosition As Integer = writer.BaseStream.Position
                        If Not currentBaseStreamPosition Mod 2 = 0 Then
                            Dim zeroPad As Byte = 0
                            writer.Write(zeroPad)
                        End If

                    Next
                End If

                ' Write the data chunk. Converts the data from Single to Short, and checks the sound data for distorsion
                writer.Write(WaveFormat.DataID.ToCharArray)

                'Writes data size
                If startSample = Nothing Then startSample = 0
                If length Is Nothing Then length = WaveData.ShortestChannelSampleCount
                If length > WaveData.ShortestChannelSampleCount Then length = WaveData.ShortestChannelSampleCount
                If startSample > WaveData.ShortestChannelSampleCount - 1 Then startSample = WaveData.ShortestChannelSampleCount - 1

                Dim numberOfDataPoints As Integer = length * WaveFormat.Channels
                Dim dataSize As UInteger = numberOfDataPoints * (WaveFormat.BitDepth / 8)
                writer.Write(dataSize)

                'Time measurement code
                'Dim startTime As DateTime = DateTime.Now

                'Prepares the data section, by creating an array with all channels interleaved
                Dim interleavedSoundArray() As Single = {}
                Try
                    ReDim interleavedSoundArray(numberOfDataPoints - 1)
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

                For c = 1 To WaveFormat.Channels

                    '   If length <waveData.SampleCount Then

                    'Dim channelCount As Integer = waveFormat.channels
                    'Dim channelArray() As Single = waveData.sampleData(c)
                    Dim counter As Integer = 0
                    For n = startSample To startSample + length - 1
                        interleavedSoundArray(c - 1 + counter) = WaveData.SampleData(c)(n)
                        counter += WaveFormat.Channels
                    Next

                    'Else
                    '    Dim counter As Integer = 0
                    '    For Each dataPoint In waveData.sampleData(c)
                    '    interleavedSoundArray(c - 1 + counter) = dataPoint
                    '    counter += waveFormat.channels
                    'Next
                    'End If
                Next

                Dim distorsion As Boolean = False
                Dim distorsionSampleCount As Integer = 0

                If checkForDistorsion = False Then
                    For Each dataPoint In interleavedSoundArray
                        writer.Write(dataPoint)
                    Next
                Else

                    Select Case WaveFormat.Encoding
                        Case Formats.WaveFormat.WaveFormatEncodings.PCM
                            Select Case WaveFormat.BitDepth
                                Case 8
                                    Dim dataPointSByteValue As Byte
                                    For Each dataPoint In interleavedSoundArray
                                        If dataPoint < Byte.MinValue Then
                                            dataPoint = Byte.MinValue
                                            distorsion = True
                                            distorsionSampleCount += 1
                                        End If

                                        If dataPoint > Byte.MaxValue Then
                                            dataPoint = Byte.MaxValue
                                            distorsion = True
                                            distorsionSampleCount += 1
                                        End If

                                        dataPointSByteValue = dataPoint
                                        writer.Write(dataPointSByteValue)
                                    Next

                                Case 16

                                    Dim dataPointShortValue As Short
                                    For Each dataPoint In interleavedSoundArray

                                        If dataPoint < Short.MinValue Then
                                            dataPoint = Short.MinValue
                                            distorsion = True
                                            distorsionSampleCount += 1
                                        End If

                                        If dataPoint > Short.MaxValue Then
                                            dataPoint = Short.MaxValue
                                            distorsion = True
                                            distorsionSampleCount += 1
                                        End If

                                        dataPointShortValue = dataPoint
                                        writer.Write(dataPointShortValue)
                                    Next
                            End Select

                        Case Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints
                            Select Case WaveFormat.BitDepth
                                Case 32

                                    Dim dataPointValue As Single = 0
                                    For Each dataPoint In interleavedSoundArray

                                        If dataPoint < Single.MinValue Then
                                            dataPoint = Single.MinValue
                                            distorsion = True
                                            distorsionSampleCount += 1
                                        End If

                                        If dataPoint > Single.MaxValue Then
                                            dataPoint = Single.MaxValue
                                            distorsion = True
                                            distorsionSampleCount += 1
                                        End If

                                        dataPointValue = dataPoint
                                        writer.Write(dataPointValue)
                                    Next
                            End Select
                    End Select

                    'Case PCM 16 bit 'This alternative piece of code throws an exception every time distorsion occurs. The code is faster than checking each sample
                    'if distorsion occurs only for a relatively small number of samples. But much slower if much distorsion occurs!
                    '   Dim dataPointShortValue As Short
                    '  For Each dataPoint In interleavedSoundArray 
                    '
                    'Try
                    'dataPointShortValue = dataPoint
                    'Catch ex As Exception
                    'Try
                    'If dataPoint < Short.MinValue Then
                    'dataPointShortValue = Short.MinValue
                    'distorsion = True
                    'distorsionSampleCount += 1
                    'End If
                    '
                    'If dataPoint > Short.MaxValue Then
                    'dataPointShortValue = Short.MaxValue
                    'distorsion = True
                    'distorsionSampleCount += 1
                    'End If
                    '
                    'Catch ex2 As Exception
                    'Errors("Error writing sound stream. " & ex2.ToString)
                    'End Try
                    'End Try
                    '
                    'writer.Write(dataPointShortValue)
                    'Next

                End If

                Dim dataChunkZeroPaddingCount As Integer = 0
                If numberOfDataPoints Mod 2 = 1 Then
                    'MsgBox("writing one pad byte")
                    dataChunkZeroPaddingCount += 1
                    Dim zeroPad As Byte = 0
                    writer.Write(zeroPad)
                End If

                writer.Seek(4, SeekOrigin.Begin)
                fileSize = writer.BaseStream.Length - dataChunkZeroPaddingCount - 8
                writer.Write(fileSize)

                Dim warnForDistorsion As Boolean = True
                If warnForDistorsion = True Then
                    If distorsion = True Then
                        AudioError("Distorsion occurred for " & distorsionSampleCount & " samples in WriteSoundStream!" & vbTab & "Sound file name: " & FileName)
                    End If
                End If


                'Storing the soundStream 
                Return WaveStream


            Catch ex As Exception
                AudioError(ex.ToString)
                Return Nothing
            End Try

        End Function

        ''' <summary>
        ''' Writes iXML data to the indicated MemoryStream.
        ''' </summary>
        ''' <param name="TargetStream"></param>
        ''' <returns>Returns True if write was succesful, or false if writing failed.</returns>
        Private Function WriteiXmlToStream(ByRef TargetStream As MemoryStream) As Boolean

            Try

                'Creating XmlWriterSettings
                Dim settings As XmlWriterSettings = New XmlWriterSettings()
                settings.Indent = True
                settings.Encoding = Text.Encoding.UTF8

                'Writing xml data to the stream
                Using writer As XmlWriter = XmlWriter.Create(TargetStream, settings)

                    ' Begin writing.
                    writer.WriteStartDocument()
                    writer.WriteStartElement("BWFXML") ' Root.
                    writer.WriteElementString("IXML_VERSION", "2.10")

                    'Writing any xml tags and data stored in the Sound.iXmlNodes object
                    If iXmlNodes IsNot Nothing Then
                        For Each CurrentNode In iXmlNodes
                            writer.WriteStartElement(CurrentNode.Item1)
                            writer.WriteRaw(CurrentNode.Item2)
                            writer.WriteEndElement()
                        Next
                    End If


                    'Writing the SMA chunk
                    '(N.B. The SMA decimal separator should always be point (.), not comma)

                    writer.WriteStartElement("SMA")
                    writer.WriteElementString("SMA_VERSION", Sound.SpeechMaterialAnnotation.CurrentVersion) 'I.e. SMA version number
                    If SMA.NominalLevel IsNot Nothing Then
                        writer.WriteElementString("NOMINAL_LEVEL", SMA.NominalLevel.Value.ToString(InvariantCulture))
                    End If

                    For channel As Integer = 1 To SMA.ChannelCount

                        writer.WriteStartElement("CHANNEL")

                        writer.WriteElementString("SEGMENTATION_COMPLETED", SMA.ChannelData(channel).SegmentationCompleted.ToString(InvariantCulture))

                        If SMA.ChannelData(channel).OrthographicForm <> "" Then
                            writer.WriteElementString("ORTHOGRAPHIC_FORM", SMA.ChannelData(channel).OrthographicForm)
                        End If
                        If SMA.ChannelData(channel).PhoneticForm <> "" Then
                            writer.WriteElementString("PHONETIC_FORM", SMA.ChannelData(channel).PhoneticForm)
                        End If

                        writer.WriteElementString("START_SAMPLE", SMA.ChannelData(channel).StartSample)
                        writer.WriteElementString("LENGTH", SMA.ChannelData(channel).Length)

                        If SMA.ChannelData(channel).UnWeightedLevel IsNot Nothing Then
                            writer.WriteElementString("UNWEIGHTED_LEVEL", SMA.ChannelData(channel).UnWeightedLevel.Value.ToString(InvariantCulture))
                        End If

                        If SMA.ChannelData(channel).UnWeightedPeakLevel IsNot Nothing Then
                            writer.WriteElementString("UNWEIGHTED_PEAKLEVEL", SMA.ChannelData(channel).UnWeightedPeakLevel.Value.ToString(InvariantCulture))
                        End If

                        If SMA.ChannelData(channel).WeightedLevel IsNot Nothing Then
                            writer.WriteElementString("WEIGHTED_LEVEL", SMA.ChannelData(channel).WeightedLevel.Value.ToString(InvariantCulture))
                        End If
                        writer.WriteElementString("FREQUENCY_WEIGHTING", SMA.ChannelData(channel).GetFrequencyWeighting.ToString)
                        writer.WriteElementString("TIME_WEIGHTING", SMA.ChannelData(channel).GetTimeWeighting.ToString(InvariantCulture))

                        writer.WriteElementString("INITIAL_PEAK", SMA.ChannelData(channel).InitialPeak.ToString(InvariantCulture))
                        writer.WriteElementString("START_TIME", SMA.ChannelData(channel).StartTime.ToString(InvariantCulture))

                        writer.WriteElementString("BAND_LEVELS", SMA.ChannelData(channel).GetBandLevelsString)
                        writer.WriteElementString("CENTRE_FREQUENCIES", SMA.ChannelData(channel).GetCentreFrequenciesString)
                        writer.WriteElementString("BAND_WIDTHS", SMA.ChannelData(channel).GetBandWidthsString)
                        writer.WriteElementString("BAND_INTEGRATION_TIMES", SMA.ChannelData(channel).GetBandIntegrationTimesString)

                        For sentence As Integer = 0 To SMA.ChannelData(channel).Count - 1

                            writer.WriteStartElement("SENTENCE")

                            'Writing sentence data
                            writer.WriteElementString("SEGMENTATION_COMPLETED", SMA.ChannelData(channel)(sentence).SegmentationCompleted.ToString(InvariantCulture))

                            If SMA.ChannelData(channel)(sentence).OrthographicForm <> "" Then
                                writer.WriteElementString("ORTHOGRAPHIC_FORM", SMA.ChannelData(channel)(sentence).OrthographicForm)
                            End If
                            If SMA.ChannelData(channel)(sentence).PhoneticForm <> "" Then
                                writer.WriteElementString("PHONETIC_FORM", SMA.ChannelData(channel)(sentence).PhoneticForm)
                            End If

                            writer.WriteElementString("START_SAMPLE", SMA.ChannelData(channel)(sentence).StartSample)
                            writer.WriteElementString("LENGTH", SMA.ChannelData(channel)(sentence).Length)
                            If SMA.ChannelData(channel)(sentence).UnWeightedLevel IsNot Nothing Then
                                writer.WriteElementString("UNWEIGHTED_LEVEL", SMA.ChannelData(channel)(sentence).UnWeightedLevel.Value.ToString(InvariantCulture))
                            End If

                            If SMA.ChannelData(channel)(sentence).UnWeightedPeakLevel IsNot Nothing Then
                                writer.WriteElementString("UNWEIGHTED_PEAKLEVEL", SMA.ChannelData(channel)(sentence).UnWeightedPeakLevel.Value.ToString(InvariantCulture))
                            End If

                            If SMA.ChannelData(channel)(sentence).WeightedLevel IsNot Nothing Then
                                writer.WriteElementString("WEIGHTED_LEVEL", SMA.ChannelData(channel)(sentence).WeightedLevel.Value.ToString(InvariantCulture))
                            End If
                            writer.WriteElementString("FREQUENCY_WEIGHTING", SMA.ChannelData(channel)(sentence).GetFrequencyWeighting.ToString)
                            writer.WriteElementString("TIME_WEIGHTING", SMA.ChannelData(channel)(sentence).GetTimeWeighting.ToString(InvariantCulture))

                            writer.WriteElementString("INITIAL_PEAK", SMA.ChannelData(channel)(sentence).InitialPeak.ToString(InvariantCulture))
                            writer.WriteElementString("START_TIME", SMA.ChannelData(channel)(sentence).StartTime.ToString(InvariantCulture))

                            writer.WriteElementString("BAND_LEVELS", SMA.ChannelData(channel)(sentence).GetBandLevelsString)
                            writer.WriteElementString("CENTRE_FREQUENCIES", SMA.ChannelData(channel)(sentence).GetCentreFrequenciesString)
                            writer.WriteElementString("BAND_WIDTHS", SMA.ChannelData(channel)(sentence).GetBandWidthsString)
                            writer.WriteElementString("BAND_INTEGRATION_TIMES", SMA.ChannelData(channel)(sentence).GetBandIntegrationTimesString)


                            For word = 0 To SMA.ChannelData(channel)(sentence).Count - 1

                                writer.WriteStartElement("WORD")

                                'writing word data
                                writer.WriteElementString("SEGMENTATION_COMPLETED", SMA.ChannelData(channel)(sentence)(word).SegmentationCompleted.ToString(InvariantCulture))

                                If SMA.ChannelData(channel)(sentence)(word).OrthographicForm <> "" Then
                                    writer.WriteElementString("ORTHOGRAPHIC_FORM", SMA.ChannelData(channel)(sentence)(word).OrthographicForm)
                                End If
                                If SMA.ChannelData(channel)(sentence)(word).PhoneticForm <> "" Then
                                    writer.WriteElementString("PHONETIC_FORM", SMA.ChannelData(channel)(sentence)(word).PhoneticForm)
                                End If

                                writer.WriteElementString("START_SAMPLE", SMA.ChannelData(channel)(sentence)(word).StartSample)
                                writer.WriteElementString("LENGTH", SMA.ChannelData(channel)(sentence)(word).Length)

                                If SMA.ChannelData(channel)(sentence)(word).UnWeightedLevel IsNot Nothing Then
                                    writer.WriteElementString("UNWEIGHTED_LEVEL", SMA.ChannelData(channel)(sentence)(word).UnWeightedLevel.Value.ToString(InvariantCulture))
                                End If

                                If SMA.ChannelData(channel)(sentence)(word).UnWeightedPeakLevel IsNot Nothing Then
                                    writer.WriteElementString("UNWEIGHTED_PEAKLEVEL", SMA.ChannelData(channel)(sentence)(word).UnWeightedPeakLevel.Value.ToString(InvariantCulture))
                                End If

                                If SMA.ChannelData(channel)(sentence)(word).WeightedLevel IsNot Nothing Then
                                    writer.WriteElementString("WEIGHTED_LEVEL", SMA.ChannelData(channel)(sentence)(word).WeightedLevel.Value.ToString(InvariantCulture))
                                End If
                                writer.WriteElementString("FREQUENCY_WEIGHTING", SMA.ChannelData(channel)(sentence)(word).GetFrequencyWeighting.ToString)
                                writer.WriteElementString("TIME_WEIGHTING", SMA.ChannelData(channel)(sentence)(word).GetTimeWeighting.ToString(InvariantCulture))

                                writer.WriteElementString("INITIAL_PEAK", SMA.ChannelData(channel)(sentence)(word).InitialPeak.ToString(InvariantCulture))
                                writer.WriteElementString("START_TIME", SMA.ChannelData(channel)(sentence)(word).StartTime.ToString(InvariantCulture))

                                writer.WriteElementString("BAND_LEVELS", SMA.ChannelData(channel)(sentence)(word).GetBandLevelsString)
                                writer.WriteElementString("CENTRE_FREQUENCIES", SMA.ChannelData(channel)(sentence)(word).GetCentreFrequenciesString)
                                writer.WriteElementString("BAND_WIDTHS", SMA.ChannelData(channel)(sentence)(word).GetBandWidthsString)
                                writer.WriteElementString("BAND_INTEGRATION_TIMES", SMA.ChannelData(channel)(sentence)(word).GetBandIntegrationTimesString)


                                'writing phone data
                                For phone = 0 To SMA.ChannelData(channel)(sentence)(word).Count - 1

                                    writer.WriteStartElement("PHONE")

                                    writer.WriteElementString("SEGMENTATION_COMPLETED", SMA.ChannelData(channel)(sentence)(word)(phone).SegmentationCompleted.ToString(InvariantCulture))

                                    If SMA.ChannelData(channel)(sentence)(word)(phone).OrthographicForm <> "" Then
                                        writer.WriteElementString("ORTHOGRAPHIC_FORM", SMA.ChannelData(channel)(sentence)(word)(phone).OrthographicForm)
                                    End If
                                    If SMA.ChannelData(channel)(sentence)(word)(phone).PhoneticForm <> "" Then
                                        writer.WriteElementString("PHONETIC_FORM", SMA.ChannelData(channel)(sentence)(word)(phone).PhoneticForm)
                                    End If

                                    writer.WriteElementString("START_SAMPLE", SMA.ChannelData(channel)(sentence)(word)(phone).StartSample)
                                    writer.WriteElementString("LENGTH", SMA.ChannelData(channel)(sentence)(word)(phone).Length)

                                    If SMA.ChannelData(channel)(sentence)(word)(phone).UnWeightedLevel IsNot Nothing Then
                                        writer.WriteElementString("UNWEIGHTED_LEVEL", SMA.ChannelData(channel)(sentence)(word)(phone).UnWeightedLevel.Value.ToString(InvariantCulture))
                                    End If

                                    If SMA.ChannelData(channel)(sentence)(word)(phone).UnWeightedPeakLevel IsNot Nothing Then
                                        writer.WriteElementString("UNWEIGHTED_PEAKLEVEL", SMA.ChannelData(channel)(sentence)(word)(phone).UnWeightedPeakLevel.Value.ToString(InvariantCulture))
                                    End If

                                    If SMA.ChannelData(channel)(sentence)(word)(phone).WeightedLevel IsNot Nothing Then
                                        writer.WriteElementString("WEIGHTED_LEVEL", SMA.ChannelData(channel)(sentence)(word)(phone).WeightedLevel.Value.ToString(InvariantCulture))
                                    End If
                                    writer.WriteElementString("FREQUENCY_WEIGHTING", SMA.ChannelData(channel)(sentence)(word)(phone).FrequencyWeighting.ToString)
                                    writer.WriteElementString("TIME_WEIGHTING", SMA.ChannelData(channel)(sentence)(word)(phone).TimeWeighting.ToString(InvariantCulture))

                                    writer.WriteElementString("INITIAL_PEAK", SMA.ChannelData(channel)(sentence)(word)(phone).InitialPeak.ToString(InvariantCulture))
                                    writer.WriteElementString("START_TIME", SMA.ChannelData(channel)(sentence)(word)(phone).StartTime.ToString(InvariantCulture))

                                    writer.WriteElementString("BAND_LEVELS", SMA.ChannelData(channel)(sentence)(word)(phone).GetBandLevelsString)
                                    writer.WriteElementString("CENTRE_FREQUENCIES", SMA.ChannelData(channel)(sentence)(word)(phone).GetCentreFrequenciesString)
                                    writer.WriteElementString("BAND_WIDTHS", SMA.ChannelData(channel)(sentence)(word)(phone).GetBandWidthsString)
                                    writer.WriteElementString("BAND_INTEGRATION_TIMES", SMA.ChannelData(channel)(sentence)(word)(phone).GetBandIntegrationTimesString)


                                    'End of phone
                                    writer.WriteEndElement()

                                Next

                                'End of word
                                writer.WriteEndElement()

                            Next

                            'End of sentence
                            writer.WriteEndElement()

                        Next

                        'End of channel
                        writer.WriteEndElement()
                    Next

                    ' End document.
                    writer.WriteEndElement()
                    writer.WriteEndElement()
                    writer.WriteEndDocument()

                    writer.Close()

                End Using

                Return True

            Catch ex As Exception

                MsgBox("Failed to write iXML data.")
                Return False
            End Try

        End Function

        Public Shared Function ParseiXMLString(ByVal fileStream As MemoryStream, Optional ByVal SourceFilePath As String = "") As Tuple(Of Sound.SpeechMaterialAnnotation, List(Of Tuple(Of String, String)))

            Dim NewSMA As New Sound.SpeechMaterialAnnotation()


            Try


                'Storing the current culture decimal separator, in order for parsing of Double values to work (independently of current culture). (N.B. The SMA decimal separator should always be point (.), not comma)
                Dim CDS = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator

                'Writing the SMA chunk

                'Creating XmlWriterSettings
                Dim settings As XmlReaderSettings = New XmlReaderSettings

                Dim BWFXML_detected As Boolean = False
                Dim iXML_Verison As String = ""

                Dim unUsediXMLNodes As New List(Of Tuple(Of String, String))

                ' Create XmlWriter.
                Using reader As XmlReader = XmlReader.Create(fileStream, settings)

                    While reader.Read

                        ' Check for start elements.
                        If reader.IsStartElement() Then

                            If reader.Name = "BWFXML" Then
                                BWFXML_detected = True

                            ElseIf reader.Name = "IXML_VERSION" Then
                                If reader.Read() Then iXML_Verison = reader.Value.Trim()

                                'Parsing SMA head data

                            ElseIf reader.Name = "SMA" Then

                                'Declaring variables used when parsing the SMA subtree
                                Dim SMA_Version As String = ""

                                'Indices that keep track of the current channel, sentence, word and phone (all increase by 1 when detected)
                                Dim CurrentChannel As Integer = 0
                                Dim CurrentSentence As Integer = -1
                                Dim CurrentWord As Integer = -1
                                Dim CurrentPhone As Integer = -1

                                'Reading the SMA subtree
                                Dim smaReader = reader.ReadSubtree()
                                While smaReader.Read

                                    ' Check for start elements.
                                    If smaReader.IsStartElement() Then

                                        If smaReader.Name = "SMA" Then
                                            'Just ignores the head node

                                        ElseIf smaReader.Name = "SMA_VERSION" Then
                                            If smaReader.Read() Then NewSMA.ReadFromVersion = smaReader.Value.Trim()

                                        ElseIf smaReader.Name = "NOMINAL_LEVEL" Then
                                            Dim value As Double
                                            If smaReader.Read() Then
                                                If Double.TryParse(smaReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                    NewSMA.NominalLevel = value
                                                End If
                                            End If

                                        ElseIf smaReader.Name = "CHANNEL" Then

                                            'resets the CurrentSentence value
                                            CurrentSentence = -1

                                            'New channel
                                            CurrentChannel += 1
                                            NewSMA.AddChannelData(SourceFilePath)

                                            'Reading the SMA channel subtree
                                            Dim smaChannelReader = smaReader.ReadSubtree()
                                            While smaChannelReader.Read

                                                ' Check for start elements.
                                                If smaChannelReader.IsStartElement() Then

                                                    If smaChannelReader.Name = "CHANNEL" Then
                                                        'Just ignores the head node

                                                    ElseIf smaChannelReader.Name = "SEGMENTATION_COMPLETED" Then
                                                        Dim value As Boolean
                                                        If smaChannelReader.Read() Then
                                                            If Boolean.TryParse(smaChannelReader.Value.Trim(), value) = True Then
                                                                NewSMA.ChannelData(CurrentChannel).SetSegmentationCompleted(value, False, False)
                                                            End If
                                                        End If

                                                    ElseIf smaChannelReader.Name = "ORTHOGRAPHIC_FORM" Then
                                                        If smaChannelReader.Read() Then
                                                            NewSMA.ChannelData(CurrentChannel).OrthographicForm = smaChannelReader.Value.Trim()
                                                        End If

                                                    ElseIf smaChannelReader.Name = "PHONETIC_FORM" Then
                                                        If smaChannelReader.Read() Then
                                                            NewSMA.ChannelData(CurrentChannel).PhoneticForm = smaChannelReader.Value.Trim()
                                                        End If

                                                    ElseIf smaChannelReader.Name = "START_SAMPLE" Then
                                                        Dim value As Integer
                                                        If smaChannelReader.Read() Then
                                                            If Integer.TryParse(smaChannelReader.Value.Trim(), value) = True Then
                                                                NewSMA.ChannelData(CurrentChannel).StartSample = value
                                                            End If
                                                        End If

                                                    ElseIf smaChannelReader.Name = "LENGTH" Then
                                                        Dim value As Integer
                                                        If smaChannelReader.Read() Then
                                                            If Integer.TryParse(smaChannelReader.Value.Trim(), value) = True Then
                                                                NewSMA.ChannelData(CurrentChannel).Length = value
                                                            End If
                                                        End If

                                                    ElseIf smaChannelReader.Name = "UNWEIGHTED_LEVEL" Then
                                                        Dim value As Double
                                                        If smaChannelReader.Read() Then
                                                            If Double.TryParse(smaChannelReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                NewSMA.ChannelData(CurrentChannel).UnWeightedLevel = value
                                                            End If
                                                        End If

                                                    ElseIf smaChannelReader.Name = "UNWEIGHTED_PEAKLEVEL" Then
                                                        Dim value As Double
                                                        If smaChannelReader.Read() Then
                                                            If Double.TryParse(smaChannelReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                NewSMA.ChannelData(CurrentChannel).UnWeightedPeakLevel = value
                                                            End If
                                                        End If

                                                    ElseIf smaChannelReader.Name = "WEIGHTED_LEVEL" Then
                                                        Dim value As Double
                                                        If smaChannelReader.Read() Then
                                                            If Double.TryParse(smaChannelReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                NewSMA.ChannelData(CurrentChannel).WeightedLevel = value
                                                            End If
                                                        End If

                                                    ElseIf smaChannelReader.Name = "FREQUENCY_WEIGHTING" Then
                                                        Dim value As FrequencyWeightings
                                                        If smaChannelReader.Read() Then
                                                            If [Enum].TryParse(smaChannelReader.Value.Trim(), True, value) = True Then
                                                                NewSMA.ChannelData(CurrentChannel).SetFrequencyWeighting(value, False)
                                                            End If
                                                        End If

                                                    ElseIf smaChannelReader.Name = "TIME_WEIGHTING" Then
                                                        Dim value As Double
                                                        If smaChannelReader.Read() Then
                                                            If Double.TryParse(smaChannelReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                NewSMA.ChannelData(CurrentChannel).SetTimeWeighting(value, False)
                                                            End If
                                                        End If

                                                    ElseIf smaChannelReader.Name = "INITIAL_PEAK" Then
                                                        Dim value As Double
                                                        If smaChannelReader.Read() Then
                                                            If Double.TryParse(smaChannelReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                NewSMA.ChannelData(CurrentChannel).InitialPeak = value
                                                            End If
                                                        End If

                                                    ElseIf smaChannelReader.Name = "START_TIME" Then
                                                        Dim value As Double
                                                        If smaChannelReader.Read() Then
                                                            If Double.TryParse(smaChannelReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                NewSMA.ChannelData(CurrentChannel).StartTime = value
                                                            End If
                                                        End If

                                                    ElseIf smaChannelReader.Name = "BAND_LEVELS" Then
                                                        If smaChannelReader.Read() Then
                                                            NewSMA.ChannelData(CurrentChannel).SetBandLevelsFromString(smaChannelReader.Value.Trim())
                                                        End If

                                                    ElseIf smaChannelReader.Name = "CENTRE_FREQUENCIES" Then
                                                        If smaChannelReader.Read() Then
                                                            NewSMA.ChannelData(CurrentChannel).SetCentreFrequenciesFromString(smaChannelReader.Value.Trim())
                                                        End If

                                                    ElseIf smaChannelReader.Name = "BAND_WIDTHS" Then
                                                        If smaChannelReader.Read() Then
                                                            NewSMA.ChannelData(CurrentChannel).SetBandWidthsFromString(smaChannelReader.Value.Trim())
                                                        End If

                                                    ElseIf smaChannelReader.Name = "BAND_INTEGRATION_TIMES" Then
                                                        If smaChannelReader.Read() Then
                                                            NewSMA.ChannelData(CurrentChannel).SetBandIntegrationTimesFromString(smaChannelReader.Value.Trim())
                                                        End If


                                                    ElseIf smaChannelReader.Name = "SENTENCE" Then

                                                        'New sentence
                                                        CurrentSentence += 1
                                                        NewSMA.ChannelData(CurrentChannel).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, SpeechMaterialAnnotation.SmaTags.SENTENCE, NewSMA.ChannelData(CurrentChannel), SourceFilePath))

                                                        'Reading the SMA sentence subtree
                                                        Dim smaSentenceReader = smaChannelReader.ReadSubtree()
                                                        While smaSentenceReader.Read

                                                            ' Check for start elements.
                                                            If smaSentenceReader.IsStartElement() Then

                                                                If smaSentenceReader.Name = "SENTENCE" Then
                                                                    'Just ignores the head node

                                                                    'Resets CurrentWord
                                                                    CurrentWord = -1

                                                                ElseIf smaSentenceReader.Name = "SEGMENTATION_COMPLETED" Then
                                                                    Dim value As Boolean
                                                                    If smaSentenceReader.Read() Then
                                                                        If Boolean.TryParse(smaSentenceReader.Value.Trim(), value) = True Then
                                                                            NewSMA.ChannelData(CurrentChannel)(CurrentSentence).SetSegmentationCompleted(value, False, False)
                                                                        End If
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "ORTHOGRAPHIC_FORM" Then
                                                                    If smaSentenceReader.Read() Then
                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence).OrthographicForm = smaSentenceReader.Value.Trim()
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "PHONETIC_FORM" Then
                                                                    If smaSentenceReader.Read() Then
                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence).PhoneticForm = smaSentenceReader.Value.Trim()
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "START_SAMPLE" Then
                                                                    Dim value As Integer
                                                                    If smaSentenceReader.Read() Then
                                                                        If Integer.TryParse(smaSentenceReader.Value.Trim(), value) = True Then
                                                                            NewSMA.ChannelData(CurrentChannel)(CurrentSentence).StartSample = value
                                                                        End If
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "LENGTH" Then
                                                                    Dim value As Integer
                                                                    If smaSentenceReader.Read() Then
                                                                        If Integer.TryParse(smaSentenceReader.Value.Trim(), value) = True Then
                                                                            NewSMA.ChannelData(CurrentChannel)(CurrentSentence).Length = value
                                                                        End If
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "UNWEIGHTED_LEVEL" Then
                                                                    Dim value As Double
                                                                    If smaSentenceReader.Read() Then
                                                                        If Double.TryParse(smaSentenceReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                            NewSMA.ChannelData(CurrentChannel)(CurrentSentence).UnWeightedLevel = value
                                                                        End If
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "UNWEIGHTED_PEAKLEVEL" Then
                                                                    Dim value As Double
                                                                    If smaSentenceReader.Read() Then
                                                                        If Double.TryParse(smaSentenceReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                            NewSMA.ChannelData(CurrentChannel)(CurrentSentence).UnWeightedPeakLevel = value
                                                                        End If
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "WEIGHTED_LEVEL" Then
                                                                    Dim value As Double
                                                                    If smaSentenceReader.Read() Then
                                                                        If Double.TryParse(smaSentenceReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                            NewSMA.ChannelData(CurrentChannel)(CurrentSentence).WeightedLevel = value
                                                                        End If
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "FREQUENCY_WEIGHTING" Then
                                                                    Dim value As FrequencyWeightings
                                                                    If smaSentenceReader.Read() Then
                                                                        If [Enum].TryParse(smaSentenceReader.Value.Trim(), True, value) = True Then
                                                                            NewSMA.ChannelData(CurrentChannel)(CurrentSentence).SetFrequencyWeighting(value, False)
                                                                        End If
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "TIME_WEIGHTING" Then
                                                                    Dim value As Double
                                                                    If smaSentenceReader.Read() Then
                                                                        If Double.TryParse(smaSentenceReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                            NewSMA.ChannelData(CurrentChannel)(CurrentSentence).SetTimeWeighting(value, False)
                                                                        End If
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "INITIAL_PEAK" Then
                                                                    Dim value As Double
                                                                    If smaSentenceReader.Read() Then
                                                                        If Double.TryParse(smaSentenceReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                            NewSMA.ChannelData(CurrentChannel)(CurrentSentence).InitialPeak = value
                                                                        End If
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "START_TIME" Then
                                                                    Dim value As Double
                                                                    If smaSentenceReader.Read() Then
                                                                        If Double.TryParse(smaSentenceReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                            NewSMA.ChannelData(CurrentChannel)(CurrentSentence).StartTime = value
                                                                        End If
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "BAND_LEVELS" Then
                                                                    If smaSentenceReader.Read() Then
                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence).SetBandLevelsFromString(smaSentenceReader.Value.Trim())
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "CENTRE_FREQUENCIES" Then
                                                                    If smaSentenceReader.Read() Then
                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence).SetCentreFrequenciesFromString(smaSentenceReader.Value.Trim())
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "BAND_WIDTHS" Then
                                                                    If smaSentenceReader.Read() Then
                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence).SetBandWidthsFromString(smaSentenceReader.Value.Trim())
                                                                    End If

                                                                ElseIf smaSentenceReader.Name = "BAND_INTEGRATION_TIMES" Then
                                                                    If smaSentenceReader.Read() Then
                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence).SetBandIntegrationTimesFromString(smaSentenceReader.Value.Trim())
                                                                    End If


                                                                ElseIf smaSentenceReader.Name = "WORD" Then

                                                                    ' A new word
                                                                    CurrentWord += 1
                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, SpeechMaterialAnnotation.SmaTags.WORD, NewSMA.ChannelData(CurrentChannel)(CurrentSentence), SourceFilePath))

                                                                    'Reading the SMA word subtree
                                                                    Dim smaWordReader = smaSentenceReader.ReadSubtree()
                                                                    While smaWordReader.Read

                                                                        ' Check for start elements.
                                                                        If smaWordReader.IsStartElement() Then

                                                                            If smaWordReader.Name = "WORD" Then
                                                                                'Just ignores the head node

                                                                                'Resets CurrentPhone
                                                                                CurrentPhone = -1

                                                                            ElseIf smaWordReader.Name = "SEGMENTATION_COMPLETED" Then
                                                                                Dim value As Boolean
                                                                                If smaWordReader.Read() Then
                                                                                    If Boolean.TryParse(smaWordReader.Value.Trim(), value) = True Then
                                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).SetSegmentationCompleted(value, False, False)
                                                                                    End If
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "ORTHOGRAPHIC_FORM" Then
                                                                                If smaWordReader.Read() Then
                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).OrthographicForm = smaWordReader.Value.Trim()
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "PHONETIC_FORM" Then
                                                                                If smaWordReader.Read() Then
                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).PhoneticForm = smaWordReader.Value.Trim()
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "START_SAMPLE" Then
                                                                                Dim value As Integer
                                                                                If smaWordReader.Read() Then
                                                                                    If Integer.TryParse(smaWordReader.Value.Trim(), value) = True Then
                                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).StartSample = value
                                                                                    End If
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "LENGTH" Then
                                                                                Dim value As Integer
                                                                                If smaWordReader.Read() Then
                                                                                    If Integer.TryParse(smaWordReader.Value.Trim(), value) = True Then
                                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).Length = value
                                                                                    End If
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "UNWEIGHTED_LEVEL" Then
                                                                                Dim value As Double
                                                                                If smaWordReader.Read() Then
                                                                                    If Double.TryParse(smaWordReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).UnWeightedLevel = value
                                                                                    End If
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "UNWEIGHTED_PEAKLEVEL" Then
                                                                                Dim value As Double
                                                                                If smaWordReader.Read() Then
                                                                                    If Double.TryParse(smaWordReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).UnWeightedPeakLevel = value
                                                                                    End If
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "WEIGHTED_LEVEL" Then
                                                                                Dim value As Double
                                                                                If smaWordReader.Read() Then
                                                                                    If Double.TryParse(smaWordReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).WeightedLevel = value
                                                                                    End If
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "FREQUENCY_WEIGHTING" Then
                                                                                Dim value As FrequencyWeightings
                                                                                If smaWordReader.Read() Then
                                                                                    If [Enum].TryParse(smaWordReader.Value.Trim(), True, value) = True Then
                                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).SetFrequencyWeighting(value, False)
                                                                                    End If
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "TIME_WEIGHTING" Then
                                                                                Dim value As Double
                                                                                If smaWordReader.Read() Then
                                                                                    If Double.TryParse(smaWordReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).SetTimeWeighting(value, False)
                                                                                    End If
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "INITIAL_PEAK" Then
                                                                                Dim value As Double
                                                                                If smaWordReader.Read() Then
                                                                                    If Double.TryParse(smaWordReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).InitialPeak = value
                                                                                    End If
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "START_TIME" Then
                                                                                Dim value As Double
                                                                                If smaWordReader.Read() Then
                                                                                    If Double.TryParse(smaWordReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                                        NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).StartTime = value
                                                                                    End If
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "BAND_LEVELS" Then
                                                                                If smaWordReader.Read() Then
                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).SetBandLevelsFromString(smaWordReader.Value.Trim())
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "CENTRE_FREQUENCIES" Then
                                                                                If smaWordReader.Read() Then
                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).SetCentreFrequenciesFromString(smaWordReader.Value.Trim())
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "BAND_WIDTHS" Then
                                                                                If smaWordReader.Read() Then
                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).SetBandWidthsFromString(smaWordReader.Value.Trim())
                                                                                End If

                                                                            ElseIf smaWordReader.Name = "BAND_INTEGRATION_TIMES" Then
                                                                                If smaWordReader.Read() Then
                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).SetBandIntegrationTimesFromString(smaWordReader.Value.Trim())
                                                                                End If


                                                                            ElseIf smaWordReader.Name = "PHONE" Then

                                                                                'A new phone
                                                                                CurrentPhone += 1
                                                                                NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord).Add(New Sound.SpeechMaterialAnnotation.SmaComponent(NewSMA, SpeechMaterialAnnotation.SmaTags.PHONE, NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord), SourceFilePath))

                                                                                'Reading the SMA phone subtree
                                                                                Dim smaPhoneReader = smaWordReader.ReadSubtree()
                                                                                While smaPhoneReader.Read

                                                                                    ' Check for start elements.
                                                                                    If smaPhoneReader.IsStartElement() Then

                                                                                        If smaPhoneReader.Name = "PHONE" Then
                                                                                            'Just ignores the head node

                                                                                        ElseIf smaPhoneReader.Name = "SEGMENTATION_COMPLETED" Then
                                                                                            Dim value As Boolean
                                                                                            If smaPhoneReader.Read() Then
                                                                                                If Boolean.TryParse(smaPhoneReader.Value.Trim(), value) = True Then
                                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).SetSegmentationCompleted(value, False, False)
                                                                                                End If
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "ORTHOGRAPHIC_FORM" Then
                                                                                            If smaPhoneReader.Read() Then
                                                                                                NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).OrthographicForm = smaPhoneReader.Value.Trim()
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "PHONETIC_FORM" Then
                                                                                            If smaPhoneReader.Read() Then
                                                                                                NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).PhoneticForm = smaPhoneReader.Value.Trim()
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "START_SAMPLE" Then
                                                                                            Dim value As Integer
                                                                                            If smaPhoneReader.Read() Then
                                                                                                If Integer.TryParse(smaPhoneReader.Value.Trim(), value) = True Then
                                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).StartSample = value
                                                                                                End If
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "LENGTH" Then
                                                                                            Dim value As Integer
                                                                                            If smaPhoneReader.Read() Then
                                                                                                If Integer.TryParse(smaPhoneReader.Value.Trim(), value) = True Then
                                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).Length = value
                                                                                                End If
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "UNWEIGHTED_LEVEL" Then
                                                                                            Dim value As Double
                                                                                            If smaPhoneReader.Read() Then
                                                                                                If Double.TryParse(smaPhoneReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).UnWeightedLevel = value
                                                                                                End If
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "UNWEIGHTED_PEAKLEVEL" Then
                                                                                            Dim value As Double
                                                                                            If smaPhoneReader.Read() Then
                                                                                                If Double.TryParse(smaPhoneReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).UnWeightedPeakLevel = value
                                                                                                End If
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "WEIGHTED_LEVEL" Then
                                                                                            Dim value As Double
                                                                                            If smaPhoneReader.Read() Then
                                                                                                If Double.TryParse(smaPhoneReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).WeightedLevel = value
                                                                                                End If
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "FREQUENCY_WEIGHTING" Then
                                                                                            Dim value As FrequencyWeightings
                                                                                            If smaPhoneReader.Read() Then
                                                                                                If [Enum].TryParse(smaPhoneReader.Value.Trim(), True, value) = True Then
                                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).FrequencyWeighting = value
                                                                                                End If
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "TIME_WEIGHTING" Then
                                                                                            Dim value As Double
                                                                                            If smaPhoneReader.Read() Then
                                                                                                If Double.TryParse(smaPhoneReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).TimeWeighting = value
                                                                                                End If
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "INITIAL_PEAK" Then
                                                                                            Dim value As Double
                                                                                            If smaPhoneReader.Read() Then
                                                                                                If Double.TryParse(smaPhoneReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).InitialPeak = value
                                                                                                End If
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "START_TIME" Then
                                                                                            Dim value As Double
                                                                                            If smaPhoneReader.Read() Then
                                                                                                If Double.TryParse(smaPhoneReader.Value.Trim().Replace(",", CDS).Replace(".", CDS), value) = True Then
                                                                                                    NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).StartTime = value
                                                                                                End If
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "BAND_LEVELS" Then
                                                                                            If smaPhoneReader.Read() Then
                                                                                                NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).SetBandLevelsFromString(smaPhoneReader.Value.Trim())
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "CENTRE_FREQUENCIES" Then
                                                                                            If smaPhoneReader.Read() Then
                                                                                                NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).SetCentreFrequenciesFromString(smaPhoneReader.Value.Trim())
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "BAND_WIDTHS" Then
                                                                                            If smaPhoneReader.Read() Then
                                                                                                NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).SetBandWidthsFromString(smaPhoneReader.Value.Trim())
                                                                                            End If

                                                                                        ElseIf smaPhoneReader.Name = "BAND_INTEGRATION_TIMES" Then
                                                                                            If smaPhoneReader.Read() Then
                                                                                                NewSMA.ChannelData(CurrentChannel)(CurrentSentence)(CurrentWord)(CurrentPhone).SetBandIntegrationTimesFromString(smaPhoneReader.Value.Trim())
                                                                                            End If


                                                                                        Else
                                                                                            MsgBox("Unknown SMA phone node ignored " & "(" & smaPhoneReader.Name & ")!")
                                                                                        End If
                                                                                    End If
                                                                                End While
                                                                            Else
                                                                                MsgBox("Unknown SMA word node ignored " & "(" & smaWordReader.Name & ")!")
                                                                            End If
                                                                        End If
                                                                    End While
                                                                Else
                                                                    MsgBox("Unknown SMA sentence node ignored " & "(" & smaSentenceReader.Name & ")!")
                                                                End If
                                                            End If
                                                        End While
                                                    Else
                                                        MsgBox("Unknown SMA channel node ignored " & "(" & smaChannelReader.Name & ")!")
                                                    End If
                                                End If
                                            End While
                                        Else
                                            MsgBox("Unknown SMA head node ignored " & "(" & smaReader.Name & ")!")
                                        End If
                                    End If
                                End While
                            Else

                                'Adding any unparsed nodes to the unUsediXMLNodes list 
                                unUsediXMLNodes.Add(New Tuple(Of String, String)(reader.Name, reader.ReadInnerXml))

                            End If
                        End If
                    End While

                End Using

                'Sets the value of SegmentationCompleted to true at all levels if loaded from version 1.0, as the SegmentationCompleted property was introcudes in version 1.1
                If NewSMA.ReadFromVersion = "1.0" Then
                    For c = 1 To NewSMA.ChannelCount
                        NewSMA.ChannelData(c).SetSegmentationCompleted(True, False, True)
                    Next
                End If

                'Sets the value of NominalLevel
                NewSMA.InferNominalLevelToAllDescendants()

                Return New Tuple(Of Sound.SpeechMaterialAnnotation, List(Of Tuple(Of String, String)))(NewSMA, unUsediXMLNodes)

            Catch ex As Exception
                MsgBox(ex.ToString)
                Return Nothing
            End Try

        End Function


#End Region

        ''' <summary>
        ''' Create and returns a new sound that can be used for debugging etc.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetTestSound() As Sound

            Dim TestSound = Audio.GenerateSound.CreateSineWave(New Formats.WaveFormat(48000, 32, 1, , Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints), 1, 500, 0.1, 3)

            TestSound.SMA = New SpeechMaterialAnnotation
            TestSound.SMA.AddChannelData(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.CHANNEL, Nothing) With {.OrthographicForm = "Test sound, channel 1", .PhoneticForm = "Test sound, channel 1 (phonetic form)"})

            TestSound.SMA.ChannelData(1).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.SENTENCE, TestSound.SMA.ChannelData(1)) With {.OrthographicForm = "Sentence 1 (orthographic form)", .PhoneticForm = "Sentence 1 (phonetic form)"})
            TestSound.SMA.ChannelData(1)(0).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.WORD, TestSound.SMA.ChannelData(1)(0)) With {.OrthographicForm = "Word 1 (orthographic form)", .PhoneticForm = "Word 1 (phonetic form)"})
            TestSound.SMA.ChannelData(1)(0).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.WORD, TestSound.SMA.ChannelData(1)(0)) With {.OrthographicForm = "Word 2 (orthographic form)", .PhoneticForm = "Word 2 (phonetic form)"})
            TestSound.SMA.ChannelData(1)(0).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.WORD, TestSound.SMA.ChannelData(1)(0)) With {.OrthographicForm = "Word 3 (orthographic form)", .PhoneticForm = "Word 3 (phonetic form)"})

            TestSound.SMA.ChannelData(1)(0)(0).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(0)) With {.OrthographicForm = "G1", .PhoneticForm = "P1"})
            TestSound.SMA.ChannelData(1)(0)(0).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(0)) With {.OrthographicForm = "G2", .PhoneticForm = "P2"})
            TestSound.SMA.ChannelData(1)(0)(0).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(0)) With {.OrthographicForm = "G3", .PhoneticForm = "P3"})

            TestSound.SMA.ChannelData(1)(0)(1).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(1)) With {.OrthographicForm = "G4", .PhoneticForm = "P4"})
            TestSound.SMA.ChannelData(1)(0)(1).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(1)) With {.OrthographicForm = "G5", .PhoneticForm = "P5"})
            TestSound.SMA.ChannelData(1)(0)(1).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(1)) With {.OrthographicForm = "G6", .PhoneticForm = "P6"})

            TestSound.SMA.ChannelData(1)(0)(2).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(2)) With {.OrthographicForm = "G7", .PhoneticForm = "P7"})
            TestSound.SMA.ChannelData(1)(0)(2).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(2)) With {.OrthographicForm = "G8", .PhoneticForm = "P8"})
            TestSound.SMA.ChannelData(1)(0)(2).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(2)) With {.OrthographicForm = "G9", .PhoneticForm = "P9"})
            TestSound.SMA.ChannelData(1)(0)(2).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(2)) With {.OrthographicForm = "G10", .PhoneticForm = "P10"})

            TestSound.SMA.ChannelData(1).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.SENTENCE, TestSound.SMA.ChannelData(1)) With {.OrthographicForm = "Sentence 2 (orthographic form)", .PhoneticForm = "Sentence 2 (phonetic form)"})
            TestSound.SMA.ChannelData(1)(1).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.WORD, TestSound.SMA.ChannelData(1)(1)) With {.OrthographicForm = "Word 4 (orthographic form)", .PhoneticForm = "Word 4 (phonetic form)"})
            TestSound.SMA.ChannelData(1)(1).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.WORD, TestSound.SMA.ChannelData(1)(1)) With {.OrthographicForm = "Word 5 (orthographic form)", .PhoneticForm = "Word 5 (phonetic form)"})
            TestSound.SMA.ChannelData(1)(1).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.WORD, TestSound.SMA.ChannelData(1)(1)) With {.OrthographicForm = "Word 6 (orthographic form)", .PhoneticForm = "Word 6 (phonetic form)"})

            TestSound.SMA.ChannelData(1)(1)(0).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(0)) With {.OrthographicForm = "G11", .PhoneticForm = "P11"})
            TestSound.SMA.ChannelData(1)(1)(0).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(0)) With {.OrthographicForm = "G12", .PhoneticForm = "P12"})
            TestSound.SMA.ChannelData(1)(1)(0).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(0)) With {.OrthographicForm = "G13", .PhoneticForm = "P13"})

            TestSound.SMA.ChannelData(1)(1)(1).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(1)) With {.OrthographicForm = "G14", .PhoneticForm = "P14"})
            TestSound.SMA.ChannelData(1)(1)(1).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(1)) With {.OrthographicForm = "G15", .PhoneticForm = "P15"})
            TestSound.SMA.ChannelData(1)(1)(1).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(1)) With {.OrthographicForm = "G16", .PhoneticForm = "P16"})

            TestSound.SMA.ChannelData(1)(1)(2).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(2)) With {.OrthographicForm = "G17", .PhoneticForm = "P17"})
            TestSound.SMA.ChannelData(1)(1)(2).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(2)) With {.OrthographicForm = "G18", .PhoneticForm = "P18"})
            TestSound.SMA.ChannelData(1)(1)(2).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(2)) With {.OrthographicForm = "G19", .PhoneticForm = "P19"})
            TestSound.SMA.ChannelData(1)(1)(2).Add(New SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(2)) With {.OrthographicForm = "G20", .PhoneticForm = "P20"})

            Return TestSound

        End Function

        Public Overrides Function ToString() As String
            If Description <> "" Then
                Return Description
            Else
                Return MyBase.ToString
            End If
        End Function

    End Class

End Namespace

