'This software is available under the following license:
'MIT/X11 License
'
'Copyright (c) 2017 Erik Witte
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
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace Audio

    <Serializable>
    Public Class FftData

        Protected _FrequencyDomainRealData As New List(Of List(Of TimeWindow))
        Protected _FrequencyDomainImaginaryData As New List(Of List(Of TimeWindow))


        <Serializable>
        Public Class TimeWindow
            Public WindowData As Double()
            Public Property WindowingType As DSP.WindowingType?
            Public Property ZeroPadding As Integer?
            Public TotalPower As Double?

            Public Sub CalculateTotalPower()
                TotalPower = WindowData.Sum
            End Sub

        End Class

        Public ReadOnly Property WindowCount(ByVal channel As Integer) As Integer
            Get
                CheckChannelValue(channel, _FrequencyDomainRealData.Count)
                Return _FrequencyDomainRealData(channel - 1).Count
            End Get
        End Property

        Public ReadOnly Property ChannelCount As Integer
            Get
                Return _FrequencyDomainRealData.Count
            End Get
        End Property



        Public ReadOnly Property Waveformat As Audio.Formats.WaveFormat
        Public ReadOnly Property FftFormat As Audio.Formats.FftFormat

        Public Property FrequencyDomainRealData(ByVal channel As Integer, ByVal windowNumber As Integer) As TimeWindow
            Get
                CheckChannelValue(channel, _FrequencyDomainRealData.Count)
                If windowNumber > _FrequencyDomainRealData(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_FrequencyDomainRealData, windowNumber - _FrequencyDomainRealData(channel - 1).Count + 1)
                Return _FrequencyDomainRealData(channel - 1)(windowNumber)
            End Get
            Set(value As TimeWindow)
                CheckChannelValue(channel, _FrequencyDomainRealData.Count)
                If windowNumber > _FrequencyDomainRealData(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_FrequencyDomainRealData, windowNumber - _FrequencyDomainRealData(channel - 1).Count + 1)
                _FrequencyDomainRealData(channel - 1)(windowNumber) = value

            End Set
        End Property

        Public Property FrequencyDomainRealData(ByVal channel As Integer) As List(Of TimeWindow)
            Get
                CheckChannelValue(channel, _FrequencyDomainRealData.Count)
                Return _FrequencyDomainRealData(channel - 1)
            End Get
            Set(value As List(Of TimeWindow))
                CheckChannelValue(channel, _FrequencyDomainRealData.Count)
                _FrequencyDomainRealData(channel - 1) = value

            End Set
        End Property

        Public Property FrequencyDomainImaginaryData(ByVal channel As Integer, ByVal windowNumber As Integer) As TimeWindow
            Get
                CheckChannelValue(channel, _FrequencyDomainImaginaryData.Count)
                If windowNumber > _FrequencyDomainImaginaryData(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_FrequencyDomainImaginaryData, windowNumber - _FrequencyDomainImaginaryData(channel - 1).Count + 1)
                Return _FrequencyDomainImaginaryData(channel - 1)(windowNumber)
            End Get
            Set(value As TimeWindow)
                CheckChannelValue(channel, _FrequencyDomainImaginaryData.Count)
                If windowNumber > _FrequencyDomainImaginaryData(channel - 1).Count - 1 Then ExtendFrequencyDomainData(_FrequencyDomainImaginaryData, windowNumber - _FrequencyDomainImaginaryData(channel - 1).Count + 1)
                _FrequencyDomainImaginaryData(channel - 1)(windowNumber) = value

            End Set
        End Property

        Public Property FrequencyDomainImaginaryData(ByVal channel As Integer) As List(Of TimeWindow)
            Get
                CheckChannelValue(channel, _FrequencyDomainImaginaryData.Count)
                Return _FrequencyDomainImaginaryData(channel - 1)
            End Get
            Set(value As List(Of TimeWindow))
                CheckChannelValue(channel, _FrequencyDomainImaginaryData.Count)
                _FrequencyDomainImaginaryData(channel - 1) = value

            End Set
        End Property


        Public Sub New(ByVal setWaveFormat As Audio.Formats.WaveFormat, ByVal setFftFormat As Audio.Formats.FftFormat)

            Waveformat = setWaveFormat
            FftFormat = setFftFormat

            For n = 0 To Waveformat.Channels - 1
                Dim ChannelFftRealData As New List(Of TimeWindow)
                _FrequencyDomainRealData.Add(ChannelFftRealData)
                Dim ChannelFftImaginaryData As New List(Of TimeWindow)
                _FrequencyDomainImaginaryData.Add(ChannelFftImaginaryData)
            Next

        End Sub

        ''' <summary>
        ''' This private sub is intended to be used only when an object of the current class is cloned by Xml serialization, such as with CreateCopy. 
        ''' </summary>
        Private Sub New()

        End Sub

        ''' <summary>
        ''' This method is only intended for internal use within FftData, but exposed publically to enable use in extension methods.
        ''' </summary>
        ''' <param name="dataToExtend"></param>
        ''' <param name="windowCountToAdd"></param>
        Public Sub ExtendFrequencyDomainData(ByRef dataToExtend As List(Of List(Of TimeWindow)), ByVal windowCountToAdd As Integer)

            For channel = 0 To dataToExtend.Count - 1
                For n = 0 To windowCountToAdd - 1
                    Dim NewTimeWindow As New TimeWindow
                    Dim newDoubleArray(FftFormat.FftWindowSize - 1) As Double
                    NewTimeWindow.WindowData = newDoubleArray
                    dataToExtend(channel).Add(NewTimeWindow)
                Next
            Next

        End Sub


        ''' <summary>
        ''' Creates a new FftData which is a deep copy of the original, by using serialization.
        ''' </summary>
        ''' <returns></returns>
        Public Function CreateCopy() As FftData

            'Creating an output object
            Dim NewObject As FftData

            'Serializing to memorystream
            Dim serializedMe As New MemoryStream
            Dim serializer As New XmlSerializer(GetType(FftData))
            serializer.Serialize(serializedMe, Me)

            'Deserializing to new object
            serializedMe.Position = 0
            NewObject = CType(serializer.Deserialize(serializedMe), FftData)
            serializedMe.Close()

            'Returning the new object
            Return NewObject
        End Function


    End Class


End Namespace