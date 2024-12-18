﻿Imports STFN.SpeechTest
Imports STFN.SrtSpeechTest

Public MustInherit Class TestProtocol

    Public Overridable Property IsInPretestMode As Boolean = False

    Public MustOverride ReadOnly Property Name As String

    Public MustOverride ReadOnly Property Information As String

    Public MustOverride Function GetPatientInstructions() As String

    ''' <summary>
    ''' Determines and returns the start level to use with the protocol.
    ''' </summary>
    ''' <param name="ReferenceValue">A test-specific reference value that may be ignored, optional or needed by derived TestProtocol class.</param>
    ''' <returns></returns>
    Public MustOverride Function GetSuggestedStartlevel(Optional ByVal ReferenceValue As Nullable(Of Double) = Nothing) As Double

    Public MustOverride Property StoppingCriterium As StoppingCriteria

    Public Enum StoppingCriteria
        ThresholdReached
        AllIncorrect
        AllCorrect
        TrialCount
    End Enum

    ''' <summary>
    ''' This function should return the number of trials remaining or -1 if not possible to determine.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function TotalTrialCount() As Integer

    Public MustOverride Function InitializeProtocol(ByRef InitialTaskInstruction As NextTaskInstruction) As Boolean

    Public MustOverride Function NewResponse(ByRef TrialHistory As TrialHistory) As NextTaskInstruction

    Public Class NextTaskInstruction
        Public Decision As SpeechTestReplies
        ''' <summary>
        ''' Only used with adaptive test protocols. Upon initiation of a new adatrive TestProtocol, should contain the start value of a variable adaptively modified by the TestProtocol according to an adaptive procedure. Upon return from a TestProtocol, should contain the updated value of the adaptively modified variable.
        ''' </summary>
        Public AdaptiveValue As Double? = Nothing
        Public AdaptiveStepSize As Double? = Nothing
        Public TestStage As Integer
        Public TestBlock As Integer

        ''' <summary>
        ''' Only primarily with fixed length test protocols. Enables the use of the test protocol with different (fixed) test lengths, such as 50 or 25 words. Upon initiation of a new TestProtocol, should contain the number of trials to be presented in the test stage (i.e. not practise trials) of the test.
        ''' </summary>
        Public TestLength As Integer
    End Class

    Public MustOverride Sub FinalizeProtocol(ByRef TrialHistory As TrialHistory)

    Public MustOverride Function GetFinalResult() As Double?

    Public Overrides Function ToString() As String
        Return Name
    End Function

End Class
