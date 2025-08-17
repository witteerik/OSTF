Imports System.Runtime.CompilerServices

Public Module Extensions

    ''' <summary>
    ''' Create and returns a new sound that can be used for debugging etc.
    ''' </summary>
    ''' <param name="obj"></param>
    <Extension()>
    Public Function GetTestSound(obj As STFN.Core.Audio.Sound)

        Dim TestSound = DSP.CreateSineWave(New STFN.Core.Audio.Formats.WaveFormat(48000, 32, 1, , STFN.Core.Audio.Formats.WaveFormat.WaveFormatEncodings.IeeeFloatingPoints), 1, 500, 0.1, 3)

        TestSound.SMA = New STFN.Core.Audio.Sound.SpeechMaterialAnnotation
        TestSound.SMA.AddChannelData(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.CHANNEL, Nothing) With {.OrthographicForm = "Test sound, channel 1", .PhoneticForm = "Test sound, channel 1 (phonetic form)"})

        TestSound.SMA.ChannelData(1).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE, TestSound.SMA.ChannelData(1)) With {.OrthographicForm = "Sentence 1 (orthographic form)", .PhoneticForm = "Sentence 1 (phonetic form)"})
        TestSound.SMA.ChannelData(1)(0).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD, TestSound.SMA.ChannelData(1)(0)) With {.OrthographicForm = "Word 1 (orthographic form)", .PhoneticForm = "Word 1 (phonetic form)"})
        TestSound.SMA.ChannelData(1)(0).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD, TestSound.SMA.ChannelData(1)(0)) With {.OrthographicForm = "Word 2 (orthographic form)", .PhoneticForm = "Word 2 (phonetic form)"})
        TestSound.SMA.ChannelData(1)(0).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD, TestSound.SMA.ChannelData(1)(0)) With {.OrthographicForm = "Word 3 (orthographic form)", .PhoneticForm = "Word 3 (phonetic form)"})

        TestSound.SMA.ChannelData(1)(0)(0).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(0)) With {.OrthographicForm = "G1", .PhoneticForm = "P1"})
        TestSound.SMA.ChannelData(1)(0)(0).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(0)) With {.OrthographicForm = "G2", .PhoneticForm = "P2"})
        TestSound.SMA.ChannelData(1)(0)(0).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(0)) With {.OrthographicForm = "G3", .PhoneticForm = "P3"})

        TestSound.SMA.ChannelData(1)(0)(1).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(1)) With {.OrthographicForm = "G4", .PhoneticForm = "P4"})
        TestSound.SMA.ChannelData(1)(0)(1).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(1)) With {.OrthographicForm = "G5", .PhoneticForm = "P5"})
        TestSound.SMA.ChannelData(1)(0)(1).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(1)) With {.OrthographicForm = "G6", .PhoneticForm = "P6"})

        TestSound.SMA.ChannelData(1)(0)(2).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(2)) With {.OrthographicForm = "G7", .PhoneticForm = "P7"})
        TestSound.SMA.ChannelData(1)(0)(2).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(2)) With {.OrthographicForm = "G8", .PhoneticForm = "P8"})
        TestSound.SMA.ChannelData(1)(0)(2).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(2)) With {.OrthographicForm = "G9", .PhoneticForm = "P9"})
        TestSound.SMA.ChannelData(1)(0)(2).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(0)(2)) With {.OrthographicForm = "G10", .PhoneticForm = "P10"})

        TestSound.SMA.ChannelData(1).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.SENTENCE, TestSound.SMA.ChannelData(1)) With {.OrthographicForm = "Sentence 2 (orthographic form)", .PhoneticForm = "Sentence 2 (phonetic form)"})
        TestSound.SMA.ChannelData(1)(1).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD, TestSound.SMA.ChannelData(1)(1)) With {.OrthographicForm = "Word 4 (orthographic form)", .PhoneticForm = "Word 4 (phonetic form)"})
        TestSound.SMA.ChannelData(1)(1).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD, TestSound.SMA.ChannelData(1)(1)) With {.OrthographicForm = "Word 5 (orthographic form)", .PhoneticForm = "Word 5 (phonetic form)"})
        TestSound.SMA.ChannelData(1)(1).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.WORD, TestSound.SMA.ChannelData(1)(1)) With {.OrthographicForm = "Word 6 (orthographic form)", .PhoneticForm = "Word 6 (phonetic form)"})

        TestSound.SMA.ChannelData(1)(1)(0).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(0)) With {.OrthographicForm = "G11", .PhoneticForm = "P11"})
        TestSound.SMA.ChannelData(1)(1)(0).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(0)) With {.OrthographicForm = "G12", .PhoneticForm = "P12"})
        TestSound.SMA.ChannelData(1)(1)(0).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(0)) With {.OrthographicForm = "G13", .PhoneticForm = "P13"})

        TestSound.SMA.ChannelData(1)(1)(1).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(1)) With {.OrthographicForm = "G14", .PhoneticForm = "P14"})
        TestSound.SMA.ChannelData(1)(1)(1).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(1)) With {.OrthographicForm = "G15", .PhoneticForm = "P15"})
        TestSound.SMA.ChannelData(1)(1)(1).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(1)) With {.OrthographicForm = "G16", .PhoneticForm = "P16"})

        TestSound.SMA.ChannelData(1)(1)(2).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(2)) With {.OrthographicForm = "G17", .PhoneticForm = "P17"})
        TestSound.SMA.ChannelData(1)(1)(2).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(2)) With {.OrthographicForm = "G18", .PhoneticForm = "P18"})
        TestSound.SMA.ChannelData(1)(1)(2).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(2)) With {.OrthographicForm = "G19", .PhoneticForm = "P19"})
        TestSound.SMA.ChannelData(1)(1)(2).Add(New STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaComponent(TestSound.SMA, STFN.Core.Audio.Sound.SpeechMaterialAnnotation.SmaTags.PHONE, TestSound.SMA.ChannelData(1)(1)(2)) With {.OrthographicForm = "G20", .PhoneticForm = "P20"})

        Return TestSound

    End Function

End Module