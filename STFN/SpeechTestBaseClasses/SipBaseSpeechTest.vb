Imports STFN.Audio
Imports STFN.SipTest

Public MustInherit Class SipBaseSpeechTest
    Inherits SpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "SipTest"


    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub


    Public Sub ApplyTestSpecificSettings()
        'Add test specific settings common for all SiP tests here

        AvailableFixedResponseAlternativeCounts = New List(Of Integer) From {3}

        ReferenceLevel = 68.34
        MinimumReferenceLevel = 50
        MaximumReferenceLevel = 90

        'MinimumLevel_Targets = 0
        'MaximumLevel_Targets = 80
        'MinimumLevel_Maskers = 0
        'MaximumLevel_Maskers = 80
        'MinimumLevel_Background = 0
        'MaximumLevel_Background = 80
        'MinimumLevel_ContralateralMaskers = 0
        'MaximumLevel_ContralateralMaskers = 80

        SoundOverlapDuration = 0.5

    End Sub

    Public Overrides ReadOnly Property ShowGuiChoice_TargetLevel As Boolean = False

    Public Overrides ReadOnly Property ShowGuiChoice_MaskingLevel As Boolean = False

    Public Overrides ReadOnly Property ShowGuiChoice_BackgroundLevel As Boolean = False

    Public Overrides ReadOnly Property ShowGuiChoice_ContralateralMasking As Boolean = False


    Protected CurrentSipTestMeasurement As SipMeasurement

    Public SelectedSoundPropagationType As SoundPropagationTypes = SoundPropagationTypes.SimulatedSoundField

    Protected SelectedTestparadigm As Testparadigm = Testparadigm.Quick

    Protected MinimumStimulusOnsetTime As Double = 0.3
    Protected MaximumStimulusOnsetTime As Double = 0.8
    Protected TrialSoundMaxDuration As Double = 10
    Protected UseBackgroundSpeech As Boolean = False
    Protected MaximumResponseTime As Double = 4
    Protected PretestSoundDuration As Double = 5
    Protected UseVisualQue As Boolean = False
    Protected ResponseAlternativeDelay As Double = 0.5
    Protected DirectionalSimulationSet As String = "ARC - Harcellen - HATS - SiP"



    Public MustOverride Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

    Public MustOverride Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

    Public MustOverride Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)

    Public MustOverride Overrides Sub FinalizeTest()

    Public MustOverride Overrides Function GetResultStringForGui() As String

    Public MustOverride Overrides Function GetTestResultsExportString() As String

    Public MustOverride Overrides Function GetTestTrialResultExportString() As String

    Public MustOverride Overrides Function CreatePreTestStimulus() As Tuple(Of Sound, String)


End Class