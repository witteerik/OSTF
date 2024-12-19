Imports STFN.Audio

Public Class SpeechAudiometryTest
    Inherits SpeechTest

    Public Overrides ReadOnly Property FilePathRepresentation As String
        Get
            Return "SpeechAudiometry"
        End Get
    End Property


#Region "Settings"

    Public Overrides ReadOnly Property TesterInstructions As String
        Get
            Return ""
        End Get
    End Property

    Public Overrides ReadOnly Property ParticipantInstructions As String
        Get
            Return ""
        End Get
    End Property


    Public Overrides ReadOnly Property HasOptionalPractiseTest As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsUseRetsplChoice As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualPreSetSelection As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualStartListSelection As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualMediaSetSelection As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualSpeechLevelSelection As Boolean
        Get
            If CanHaveTargets = True Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualMaskingLevelSelection As Boolean
        Get
            If CanHaveMaskers = True Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualBackgroundLevelSelection As Boolean
        Get
            If CanHaveBackgroundNonSpeech = True Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public Overrides ReadOnly Property SupportsPrelistening As Boolean
        Get
            Return True
        End Get
    End Property


    Public Overrides ReadOnly Property UseSoundFieldSimulation As Utils.TriState
        Get
            Return Utils.TriState.Optional
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestModes As List(Of TestModes)
        Get
            'Return New List(Of TestModes) From {TestModes.AdaptiveSpeech, TestModes.AdaptiveNoise}
            Return New List(Of TestModes) From {TestModes.ConstantStimuli, TestModes.AdaptiveNoise, TestModes.AdaptiveSpeech}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableTestProtocols As List(Of TestProtocol)
        Get
            Return New List(Of TestProtocol) From {
                New SrtSwedishHint2018_TestProtocol,
                New BrandKollmeier2002_TestProtocol,
                New FixedLengthWordsInNoise_WithPreTestLevelAdjustment_TestProtocol,
                New HagermanKinnefors1995_TestProtocol,
                New SrtAsha1979_TestProtocol,
                New SrtChaiklinFontDixon1967_TestProtocol,
                New SrtChaiklinVentry1964_TestProtocol,
                New SrtIso8253_TestProtocol,
                New SrtSwedishHint2018_TestProtocol}
        End Get
    End Property

    Public Overrides ReadOnly Property AvailableFixedResponseAlternativeCounts As List(Of Integer)
        Get
            Return New List(Of Integer)
        End Get
    End Property

    Public Overrides ReadOnly Property AvailablePhaseAudiometryTypes As List(Of BmldModes)
        Get
            Return New List(Of BmldModes) From {BmldModes.RightOnly, BmldModes.LeftOnly, BmldModes.BinauralSamePhase, BmldModes.BinauralPhaseInverted, BmldModes.BinauralUncorrelated}
        End Get
    End Property

#End Region

    Public Overrides ReadOnly Property MaximumSoundFieldSpeechLocations As Integer
        Get
            Return 1
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSoundFieldMaskerLocations As Integer
        Get
            Return 1000
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSoundFieldBackgroundNonSpeechLocations As Integer
        Get
            Return 1000
        End Get
    End Property

    Public Overrides ReadOnly Property MaximumSoundFieldBackgroundSpeechLocations As Integer
        Get
            Return 1000
        End Get
    End Property

    Public Overrides ReadOnly Property MinimumSoundFieldSpeechLocations As Integer
        Get
            Return 1
        End Get
    End Property

    Public Overrides ReadOnly Property MinimumSoundFieldMaskerLocations As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property MinimumSoundFieldBackgroundNonSpeechLocations As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property MinimumSoundFieldBackgroundSpeechLocations As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property AllowsManualReferenceLevelSelection As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanHaveTargets As Boolean
        Get
            If SpeechMaterial IsNot Nothing Then
                If TestOptions.SelectedMediaSet IsNot Nothing Then
                    If TestOptions.SelectedMediaSet.MediaAudioItems > 0 Then
                        Return True
                    End If
                End If
            End If
            'Returns False otherwise
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanHaveMaskers As Boolean
        Get
            If SpeechMaterial IsNot Nothing Then
                If TestOptions.SelectedMediaSet IsNot Nothing Then
                    If TestOptions.SelectedMediaSet.MaskerAudioItems > 0 Then
                        Return True
                    End If
                End If
            End If
            'Returns False otherwise
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanHaveBackgroundNonSpeech As Boolean
        Get
            If SpeechMaterial IsNot Nothing Then
                If TestOptions.SelectedMediaSet IsNot Nothing Then
                    'TODO: This is not a good solution, as it doesn't really specify the number of available sound files. Consider adding BackgroundNonspeechAudioItems to the MediaSet specification
                    If TestOptions.SelectedMediaSet.BackgroundNonspeechParentFolder.Trim <> "" Then
                        Return True
                    End If
                End If
            End If
            'Returns False otherwise
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanHaveBackgroundSpeech As Boolean
        Get
            If SpeechMaterial IsNot Nothing Then
                If TestOptions.SelectedMediaSet IsNot Nothing Then
                    'TODO: This is not a good solution, as it doesn't really specify the number of available sound files. Consider adding BackgroundSpeechAudioItems to the MediaSet specification
                    If TestOptions.SelectedMediaSet.BackgroundSpeechParentFolder.Trim <> "" Then
                        Return True
                    End If
                End If
            End If
            'Returns False otherwise
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property UseKeyWordScoring As Utils.TriState
        Get
            Return Utils.Constants.TriState.True
        End Get
    End Property

    Public Overrides ReadOnly Property UseListOrderRandomization As Utils.TriState
        Get
            Return Utils.Constants.TriState.Optional
        End Get
    End Property

    Public Overrides ReadOnly Property UseWithinListRandomization As Utils.TriState
        Get
            Return Utils.Constants.TriState.Optional
        End Get
    End Property

    Public Overrides ReadOnly Property UseAcrossListRandomization As Utils.TriState
        Get
            Return Utils.Constants.TriState.Optional
        End Get
    End Property

    Public Overrides ReadOnly Property UseFreeRecall As Utils.TriState
        Get
            Return Utils.TriState.Optional
        End Get
    End Property

    Public Overrides ReadOnly Property UseDidNotHearAlternative As Utils.TriState
        Get
            Return Utils.Constants.TriState.Optional
        End Get
    End Property

    Public Overrides ReadOnly Property UseContralateralMasking As Utils.TriState
        Get
            If SpeechMaterial IsNot Nothing Then
                If TestOptions.SelectedMediaSet IsNot Nothing Then
                    If TestOptions.SelectedMediaSet.ContralateralMaskerAudioItems > 0 Then
                        Return Utils.Constants.TriState.Optional
                    End If
                End If
            End If
            'Returns False otherwise
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property UsePhaseAudiometry As Utils.TriState
        Get
            Return Utils.Constants.TriState.False
        End Get
    End Property

    Public Overrides ReadOnly Property LevelStepSize As Double
        Get
            Return 1
        End Get
    End Property

    Public Overrides ReadOnly Property HistoricTrialCount As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property SupportsManualPausing As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides Property SoundOverlapDuration As Double = 0.1

    Public Overrides ReadOnly Property DefaultReferenceLevel As Double = 65
    Public Overrides ReadOnly Property DefaultSpeechLevel As Double = 65
    Public Overrides ReadOnly Property DefaultMaskerLevel As Double = 65
    Public Overrides ReadOnly Property DefaultBackgroundLevel As Double = 50
    Public Overrides ReadOnly Property DefaultContralateralMaskerLevel As Double = 25

    Public Overrides ReadOnly Property MinimumReferenceLevel As Double = 0
    Public Overrides ReadOnly Property MaximumReferenceLevel As Double = 80

    Public Overrides ReadOnly Property MinimumLevel_Targets As Double = 0
    Public Overrides ReadOnly Property MaximumLevel_Targets As Double = 80

    Public Overrides ReadOnly Property MinimumLevel_Maskers As Double = 0
    Public Overrides ReadOnly Property MaximumLevel_Maskers As Double = 80

    Public Overrides ReadOnly Property MinimumLevel_Background As Double = 0
    Public Overrides ReadOnly Property MaximumLevel_Background As Double = 80

    Public Overrides ReadOnly Property MinimumLevel_ContralateralMaskers As Double = 0
    Public Overrides ReadOnly Property MaximumLevel_ContralateralMaskers As Double = 80

    Public Overrides ReadOnly Property AvailableExperimentNumbers As Integer()
        Get
            Return {}
        End Get
    End Property

    Public Sub New(ByVal SpeechMaterialName As String)
        MyBase.New(SpeechMaterialName)
    End Sub

    Public Overrides Function InitializeCurrentTest() As Tuple(Of Boolean, String)
        'Throw New NotImplementedException()
        Return New Tuple(Of Boolean, String)(False, "")
    End Function

    Public Overrides Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies
        'Throw New NotImplementedException()
        Return New SpeechTestReplies
    End Function

    Public Overrides Sub UpdateHistoricTrialResults(sender As Object, e As SpeechTestInputEventArgs)
        'Throw New NotImplementedException()
    End Sub

    Public Overrides Sub FinalizeTest()
        'Throw New NotImplementedException()
    End Sub

    Public Overrides Function GetResultStringForGui() As String
        'Throw New NotImplementedException()
        Return ""
    End Function

    Public Overrides Function GetTestResultsExportString() As String
        'Throw New NotImplementedException()
        Return ""
    End Function

    Public Overrides Function GetTestTrialResultExportString() As String
        'Throw New NotImplementedException()
        Return ""
    End Function

    Public Overrides Function CreatePreTestStimulus() As Tuple(Of Sound, String)
        'Throw New NotImplementedException()
        Return New Tuple(Of Sound, String)(New Sound(New Formats.WaveFormat(48000, 32, 1)), "")
    End Function
End Class



