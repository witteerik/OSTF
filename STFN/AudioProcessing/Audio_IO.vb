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

Namespace Audio

    Namespace AudioIOs

        Partial Public Module AudioIO
            ''' <summary>
            ''' Reads a sound (.wav or .ptwf) from file and stores it in a new Sounds object.
            ''' </summary>
            ''' <param name="filePath">The file path to the file to read. If left empty a open file dialogue box will appear.</param>
            ''' <returns>Returns a new Sound containing the sound data from the input sound file.</returns>
            Public Function LoadWaveFile(ByVal filePath As String) As STFN.Core.Audio.Sound

                Return STFN.Core.Audio.Sound.LoadWaveFile(filePath)

            End Function

            ''' <summary>
            ''' Saves the current instance of Sound to a wave file.
            ''' </summary>
            ''' <param name="sound">The sound to be saved.</param>
            ''' <param name="filePath">The filepath where the file should be saved. If left empty a windows forms save file dialogue box will appaear,
            ''' in which the user may enter file name and storing location.</param>
            ''' <param name="startSample">This parameter enables saving of only a part of the file. StartSample indicates the first sample to be saved.</param>
            ''' <param name="length">This parameter enables saving of only a part of the file. Length indicates the length in samples of the file to be saved.</param>
            ''' <returns>Returns true if save succeded, and Flase if save failed.</returns>
            Public Function WriteWaveFile(ByRef sound As STFN.Core.Audio.Sound, ByRef filePath As String,
                                           Optional ByVal startSample As Integer = Nothing, Optional ByVal length As Integer? = Nothing,
                                           Optional CreatePath As Boolean = True) As Boolean

                Return sound.WriteWaveFile(filePath, startSample, length, CreatePath)

            End Function

            Public Sub RemoveWaveChunksBatch(ByVal Folder1 As String)

                Dim Files1 = IO.Directory.GetFiles(Folder1)

                For n = 0 To Files1.Length - 1

                    'Loads the file
                    Dim LoadedSound = STFN.Core.Audio.Sound.LoadWaveFile(Files1(n))

                    'Removes unparsed wave chunks and the SMA object
                    LoadedSound.RemoveUnparsedWaveChunks()
                    LoadedSound.SMA = Nothing

                    'Overwrites the original file
                    LoadedSound.WriteWaveFile(Files1(n))

                Next


            End Sub


        End Module


    End Namespace


End Namespace