﻿Imports STFN.SpeechTest
Imports STFN.SrtSpeechTest

Public Class TestProtocols
    Inherits List(Of TestProtocol)

    Public Shared Function GetSrtProtocols() As TestProtocols
        Dim Output = New TestProtocols

        'Adding suitable protocols
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
        Output.Add(New SrtExperimentalProtocol)

        Return Output

    End Function

    Public Shared Function GetAllProtocols() As TestProtocols
        Dim Output = New TestProtocols

        'Adding suitable protocols
        Output.Add(New SrtExperimentalProtocol)

        Return Output

    End Function

End Class

Public MustInherit Class TestProtocol

    Public MustOverride ReadOnly Property Name As String

    Public MustOverride Sub InitializeProtocol(ByVal InitialTaskInstruction As NextTaskInstruction)

    Public MustOverride Function NewResponse(ByRef TrialHistory As TrialHistory) As NextTaskInstruction

    Public Class NextTaskInstruction
        Public Decision As SpeechTestReplies
        Public AdaptiveValue As Double
        Public TestStage As UInteger
        Public TestBlock As UInteger
    End Class

    Public MustOverride Function GetResults(ByRef TrialHistory As TrialHistory) As TestResults

    Public Overrides Function ToString() As String
        Return Name
    End Function

End Class