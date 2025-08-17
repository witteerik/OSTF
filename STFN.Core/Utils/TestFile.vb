
Imports System.Runtime.CompilerServices

Namespace MyNamespace.Base

    Public Class MyContentClass

        Public Shared Sub Sub1()
            Console.WriteLine("Sub1 called")
        End Sub

        Public Sub InstanceMethod()
            Console.WriteLine("InstanceMethod in base class")
        End Sub

    End Class

    Public Class MyTestClass
        Public Sub New()

            MyContentClass.Sub1()

            Dim obj As New MyContentClass()
            obj.InstanceMethod()

        End Sub
    End Class

End Namespace

Namespace MyNamespace


    Public Class MyContentClass
        Inherits MyNamespace.Base.MyContentClass

        Public Shared Sub Sub2()
            Console.WriteLine("Sub2 called")
        End Sub


    End Class

    Public Module Extensions

        <Extension()>
        Public Sub Sub3(obj As MyNamespace.Base.MyContentClass)
            Console.WriteLine("Sub3 (extension method) called on MyContentClass instance")
        End Sub

    End Module


End Namespace



