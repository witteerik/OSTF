Public Module IPA

    Public Const [Long] As String = "ː"
    Public Const HalfLong As String = "ˑ"
    Public Const PrimaryStress As String = "ˈ"
    Public Const SecondaryStress As String = "ˌ"
    Public Const SyllableBoundary As String = "."
    Public Const ExtraShort As String = ChrW(774)

    Public Const Ejective As String = "’"

    Public Const ZeroPhoneme As String = "∅"
    Public Const PrimaryStress_SwedishAccent2 As String = "²"


    Public Vowels As New List(Of String) From {"i", "y", "ɨ", "ʉ", "ɯ", "u", "ɪ", "ʏ", "ʊ", "e", "ø", "ɘ", "ɵ", "ɤ", "o", "ə", "ɛ", "œ", "ɜ", "ɞ", "ʌ", "ɔ", "æ", "ɐ", "a", "ɶ", "ɑ", "ɒ"}

    ''' <summary>
    ''' Returns a copy of the input phoneme with all IPA length markings removed
    ''' </summary>
    ''' <param name="InputPhoneme"></param>
    ''' <returns></returns>
    Public Function RemoveLengthMarkers(ByRef InputPhoneme As String) As String
        Return InputPhoneme.Replace(IPA.Long, "").Replace(IPA.HalfLong, "").Replace(IPA.ExtraShort, "")
    End Function

End Module
