
Imports System.Runtime.CompilerServices

Namespace MyBaseClassNamespace

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

Namespace MyExtensionClassNamespace


    Public Class MyContentClass
        Inherits MyBaseClassNamespace.MyContentClass

        Public Shared Sub Sub2()
            Console.WriteLine("Sub2 called")
        End Sub


    End Class

    Public Module Extensions

        <Extension()>
        Public Sub Sub3(obj As MyBaseClassNamespace.MyContentClass)
            Console.WriteLine("Sub3 (extension method) called on MyContentClass instance")
        End Sub

    End Module


    Public Class MyTestClass
        Public Sub New()

            MyBaseClassNamespace.MyContentClass.Sub1()
            MyExtensionClassNamespace.MyContentClass.Sub1()
            MyContentClass.Sub1()

            MyContentClass.Sub2()

            ' But Sub3 is an extension method, so needs an instance
            Dim obj As New MyBaseClassNamespace.MyContentClass()
            obj.Sub3()

        End Sub
    End Class


End Namespace
