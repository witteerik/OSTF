Public Class SrtAsha1979
    Inherits TestProtocol

    Public Overrides ReadOnly Property Name As String
        Get
            Return "SRT ASHA 1979"
        End Get
    End Property

    Private _StoppingCriterium As StoppingCriteria = StoppingCriteria.ThresholdReached
    Public Overrides Property StoppingCriterium As StoppingCriteria
        Get
            Return _StoppingCriterium
        End Get
        Set(value As StoppingCriteria)
            _StoppingCriterium = value
        End Set
    End Property

    Public Overrides Sub InitializeProtocol(ByRef InitialTaskInstruction As NextTaskInstruction)
        Throw New NotImplementedException()
    End Sub

    Public Overrides Function NewResponse(ByRef TrialHistory As TrialHistory) As NextTaskInstruction
        Throw New NotImplementedException()
    End Function

    Public Overrides Function GetResults(ByRef TrialHistory As TrialHistory) As TestResults
        Throw New NotImplementedException()
    End Function
End Class