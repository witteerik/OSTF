Imports STFN.Core.Audio

Public MustInherit Class SpeechAudiometryTest
    Inherits SpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String = "SpeechAudiometry"


    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
        ApplyTestSpecificSettings()
    End Sub


    Public Sub ApplyTestSpecificSettings()
        'Add test specific settings here

    End Sub

    Protected ObservedTrials As New TestTrialCollection


    Public MustOverride Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)

    Public MustOverride Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

    Public MustOverride Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)

    Public MustOverride Overrides Sub FinalizeTestAheadOfTime()


    Public Overrides Function GetObservedTestTrials() As IEnumerable(Of TestTrial)
        Return ObservedTrials
    End Function

    Public MustOverride Overrides Function GetSelectedExportVariables() As List(Of String)


    Public MustOverride Overrides Function GetResultStringForGui() As String


    Public MustOverride Overrides Function CreatePreTestStimulus() As Tuple(Of Sound, String)

    ''' <summary>
    ''' This method returns a calibration sound mixed to the currently set target level, presented from the first indicated target sound source.
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function CreateCalibrationCheckSignal() As Tuple(Of Audio.Sound, String)

        Return MixStandardCalibrationSound(True, CalibrationCheckLevelTypes.TargetLevel)

    End Function



End Class



