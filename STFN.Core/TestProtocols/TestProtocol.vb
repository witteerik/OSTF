Imports STFN.Core.SpeechTest
Imports STFN.Core.SrtSpeechTest

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

    Public MustOverride Function NewResponse(ByRef TrialHistory As TestTrialCollection) As NextTaskInstruction

    Public Function NewResponse(ByRef TrialHistory As List(Of SipTest.SipTrial)) As NextTaskInstruction

        Dim NewTrialHistory As New TestTrialCollection
        For Each Trial In TrialHistory
            NewTrialHistory.Add(Trial)
        Next
        Return NewResponse(NewTrialHistory)

    End Function



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


    ''' <summary>
    ''' When possible, this method calculates the test results if the test is aborted before completion.
    ''' </summary>
    ''' <param name="TrialHistory"></param>
    Public MustOverride Sub AbortAheadOfTime(ByRef TrialHistory As TestTrialCollection)

    'Public Function AbortAheadOfTime(ByRef TrialHistory As List(Of SipTest.SipTrial)) 

    '    Dim NewTrialHistory As New TrialHistory
    '    For Each Trial In TrialHistory
    '        NewTrialHistory.Add(Trial)
    '    Next
    '    Return NewResponse(NewTrialHistory)

    'End Function

    ''' <summary>
    ''' Should hold the target score in adaptive tests, i.e. 50 % for the SNR where 50 % of items are correctly repeated.
    ''' </summary>
    ''' <returns></returns>
    Public Property TargetScore As Double? = Nothing

    Public MustOverride Function GetCurrentAdaptiveValue() As Double?

    Public MustOverride Sub OverrideCurrentAdaptiveValue(ByVal NewValue As Double)


    ''' <summary>
    ''' Should hold a text description of the type of the value stored in the final result, e.g. SRT, WRS, etc
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function GetFinalResultType() As String

    ''' <summary>
    ''' Should hold the final result of the test protocol or Nothing if the final result has not yet been attained
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function GetFinalResultValue() As Double?

    ''' <summary>
    ''' Instantiates a new instance of (the derived type of) a the current TestProtocol with the default settings.
    ''' </summary>
    ''' <returns></returns>
    Public Function ProduceFreshInstance() As TestProtocol
        Return Activator.CreateInstance(Me.GetType)
    End Function


    Public Overrides Function ToString() As String
        Return Name
    End Function

End Class


