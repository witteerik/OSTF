Imports STFN.Core.MyNamespace

Namespace OtherNamespace2

    Public Class MyTestClass
        Public Sub New()

            MyContentClass.Sub1()
            MyContentClass.Sub2()

            ' But Sub3 is an extension method, so needs an instance
            Dim obj As New MyContentClass()
            obj.Sub3()

        End Sub
    End Class

End Namespace