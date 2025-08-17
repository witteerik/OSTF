Imports STFN.Core.SipTest

Public Class SiPTestUnit
    Inherits STFN.Core.SipTest.SiPTestUnit

    Public Sub New(ByRef ParentMeasurement As SipMeasurement, Optional Description As String = "")
        MyBase.New(ParentMeasurement, Description)
    End Sub
End Class
