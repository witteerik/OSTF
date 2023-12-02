Imports STFN.SpeechTest
Imports STFN.SrtSpeechTest

Public Class TestProtocols
    Inherits List(Of TestProtocol)

    Public Shared Function GetSrtProtocols() As TestProtocols
        Dim Output = New TestProtocols

        'Adding suitable protocols
        Output.Add(New SrtChaiklinVentry1964)
        Output.Add(New SrtExperimentalProtocol)

        Return Output

    End Function

    Public Shared Function GetSipProtocols() As TestProtocols

        Dim Output = New TestProtocols
        'Adding suitable protocols
        'Output.Add(New SrtExperimentalProtocol)
        Return Output

    End Function

    Public Shared Function GetThresholdProtocols() As TestProtocols
        Dim Output = New TestProtocols

        'Adding suitable protocols
        Output.Add(New SrtChaiklinVentry1964)
        Output.Add(New SrtExperimentalProtocol)

        Return Output

    End Function

    Public Shared Function GetAllProtocols() As TestProtocols
        Dim Output = New TestProtocols

        'Adding suitable protocols
        Output.Add(New SrtChaiklinVentry1964)
        Output.Add(New SrtExperimentalProtocol)

        Return Output

    End Function

End Class

Public MustInherit Class TestProtocol

    Public MustOverride ReadOnly Property Name As String

    Public MustOverride Property StoppingCriterium As StoppingCriteria

    Public Enum StoppingCriteria
        ThresholdReached
        AllIncorrect
        AllCorrect
        TrialCount
    End Enum

    Public MustOverride Sub InitializeProtocol(ByRef InitialTaskInstruction As NextTaskInstruction)

    Public MustOverride Function NewResponse(ByRef TrialHistory As TrialHistory) As NextTaskInstruction

    Public Class NextTaskInstruction
        Public Decision As SpeechTestReplies
        ''' <summary>
        ''' Only used with adaptive test protocols. Upon initiation of a new adatrive TestProtocol, should contain the start value of a variable adaptively modified by the TestProtocol according to an adaptive procedure. Upon return from a TestProtocol, should contain the updated value of the adaptively modified variable.
        ''' </summary>
        Public AdaptiveValue As Double? = Nothing
        Public AdaptiveStepSize As Double? = Nothing
        Public TestStage As UInteger
        Public TestBlock As UInteger
    End Class

    Public MustOverride Function GetResults(ByRef TrialHistory As TrialHistory) As TestResults

    Public Overrides Function ToString() As String
        Return Name
    End Function

End Class
