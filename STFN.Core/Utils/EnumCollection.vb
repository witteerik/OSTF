


Namespace Utils

    Public Module EnumCollection

        Public Enum Sides
            Left
            Right
        End Enum

        Public Enum ElevationChange
            Ascending
            Unchanged
            Descendning
        End Enum

        Public Enum SidesWithBoth
            Left
            Right
            Both
        End Enum

        Public Enum Languages
            English
            Swedish
        End Enum

        Public Enum UserTypes
            Research
            Clinical
        End Enum

        Public Enum SoundPropagationTypes
            PointSpeakers
            SimulatedSoundField
            Ambisonics
        End Enum

    End Module

End Namespace


'Enums directly available under STFN.Core
Public Enum Platforms ' These reflects the platform names currently specified in NET MAUI. Not all will work with OSTF.
    iOS
    WinUI
    UWP
    Tizen
    tvOS
    MacCatalyst
    macOS
    watchOS
    Unknown
    Android
End Enum
