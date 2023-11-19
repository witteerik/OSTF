Public MustInherit Class SpeechTest

    Public MustOverride Function PrepareNextTrial() As Boolean

    Public MustOverride Function StartNextTrial() As Boolean

    Public MustOverride Function HandleResponse(sender As Object, e As ResponseGivenEventArgs) As HandleResponseOutcomes

    Public Enum HandleResponseOutcomes
        ContinueTest
        CompletedTest
    End Enum


End Class

