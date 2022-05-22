Public Class OstfSettings


    ' Program location
    Public Property RootPath As String = "C:\OSTA\Tests\SwedishSiPTest" 'Indicates the root path. Other paths given in the project setting files are relative (subpaths) to this path only if they begin with .\ otherwise they are taken as absolute paths.

    Public Property SpeechMaterialComponentsPath As String = ".\ProjectFiles\SpeechMaterialComponents.txt"

    '' SMA-format
    'Public Property MediaFolderItemLevel As MediaFolderItemLevels = MediaFolderItemLevels.Word ' The linguistic level that Each recording In the MediaFolder corresponds To. Possible values: Sentence, WordList, Word,

    'Public Enum MediaFolderItemLevels
    '    Sentence
    '    WordList
    '    Word
    'End Enum

    ' Test situations
    Public Property AvailableTestSituationsFilePath As String = ".\Tests\SwedishSiPTest\ProjectFiles\TestSituationsFile.txt"

    ' Test presets
    Public Property TestPresetsFilePath As String = ".\Tests\SwedishSiPTest\ProjectFiles\TestPresets.txt"


End Class
