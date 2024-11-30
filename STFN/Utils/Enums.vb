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


Namespace Utils

    Public Module Constants 'TODO: This should be renamed!!! It no longer contains any constants!!! ;-)

        Public Enum ElevationChange
            Ascending
            Unchanged
            Descendning
        End Enum

        Public Enum TriState
            [True]
            [False]
            [Optional]
        End Enum

        Public Enum Sides
            Left
            Right
        End Enum

        Public Enum SidesWithBoth
            Left
            Right
            Both
        End Enum

        Public Enum RightLeftLocations
            Left
            Right
            Centralized
            None
        End Enum

        Public Enum AudiometryConductionTypes
            AirConduction
            BoneConduction
        End Enum

        Public Enum AudiometryTransducerTypes
            SupraAuralPhones
            InsertPhones
            BoneConductor
            None
        End Enum

        Public Enum UserTypes
            Research
            Clinical
        End Enum

        Public Enum Languages
            English
            Swedish
        End Enum

        Public Enum ResponseModes
            MouseClick
            TabletTouch
        End Enum

    End Module

End Namespace