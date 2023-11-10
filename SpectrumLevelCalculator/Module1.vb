Module Module1

    Sub Main()

        SpeechTestFramework.OstfBase.UseOptimizationLibraries = True
        SpeechTestFramework.Audio.DSP.AudioBatchFunctions.CalculateSpectrumLevelsOfSoundFiles()
        Console.WriteLine("Finished! Press enter to close.")
        Console.ReadLine()

    End Sub

End Module
