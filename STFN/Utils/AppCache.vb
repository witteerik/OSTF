
Namespace Utils

    Public Module AppCache

        Public Event OnAppCacheVariableExists As EventHandler(Of AppCacheEventArgs)
        Public Event OnSetAppCacheStringVariableValue As EventHandler(Of AppCacheEventArgs)
        Public Event OnSetAppCacheIntegerVariableValue As EventHandler(Of AppCacheEventArgs)
        Public Event OnSetAppCacheDoubleVariableValue As EventHandler(Of AppCacheEventArgs)
        Public Event OnGetAppCacheStringVariableValue As EventHandler(Of AppCacheEventArgs)
        Public Event OnGetAppCacheDoubleVariableValue As EventHandler(Of AppCacheEventArgs)
        Public Event OnRemoveAppCacheVariable As EventHandler(Of AppCacheEventArgs)
        Public Event OnClearAppCache As EventHandler(Of EventArgs)

        Public Function AppCacheVariableExists(VariableName As String) As Boolean
            Dim e = New AppCacheEventArgs(VariableName)
            RaiseEvent OnAppCacheVariableExists(Nothing, e)
            Return e.Result
        End Function

        Public Sub SetAppCacheVariableValue(VariableName As String, VariableValue As String)
            RaiseEvent OnSetAppCacheStringVariableValue(Nothing, New AppCacheEventArgs(VariableName, VariableValue))
        End Sub

        Public Sub SetAppCacheVariableValue(VariableName As String, VariableValue As Integer)
            RaiseEvent OnSetAppCacheIntegerVariableValue(Nothing, New AppCacheEventArgs(VariableName, VariableValue))
        End Sub

        Public Sub SetAppCacheVariableValue(VariableName As String, VariableValue As Double)
            RaiseEvent OnSetAppCacheDoubleVariableValue(Nothing, New AppCacheEventArgs(VariableName, VariableValue))
        End Sub

        Public Function GetAppCacheStringVariableValue(VariableName As String) As String
            Dim e = New AppCacheEventArgs(VariableName)
            RaiseEvent OnGetAppCacheStringVariableValue(Nothing, e)
            Return e.VariableStringValue
        End Function

        Public Function GetAppCacheIntegerVariableValue(VariableName As String) As Integer?
            Dim e = New AppCacheEventArgs(VariableName)
            RaiseEvent OnGetAppCacheDoubleVariableValue(Nothing, e)
            Return e.VariableIntegerValue
        End Function

        Public Function GetAppCacheDoubleVariableValue(VariableName As String) As Double?
            Dim e = New AppCacheEventArgs(VariableName)
            RaiseEvent OnGetAppCacheDoubleVariableValue(Nothing, e)
            Return e.VariableDoubleValue
        End Function

        Public Sub RemoveAppCacheVariable(VariableName As String)
            RaiseEvent OnRemoveAppCacheVariable(Nothing, New AppCacheEventArgs(VariableName))
        End Sub

        Public Sub ClearAppCache()
            RaiseEvent OnClearAppCache(Nothing, New EventArgs)
        End Sub

        Public Class AppCacheEventArgs
            Inherits EventArgs
            Public Property VariableName As String
            Public Property VariableStringValue As String
            Public Property VariableIntegerValue As Integer?
            Public Property VariableDoubleValue As Double?
            Public Property Result As Boolean

            Public Sub New(VariableName As String)
                Me.VariableName = VariableName
            End Sub

            Public Sub New(VariableName As String, VariableValue As String)
                Me.VariableName = VariableName
                Me.VariableStringValue = VariableValue
            End Sub
            Public Sub New(VariableName As String, VariableValue As Integer)
                Me.VariableName = VariableName
                Me.VariableIntegerValue = VariableValue
            End Sub

            Public Sub New(VariableName As String, VariableValue As Double)
                Me.VariableName = VariableName
                Me.VariableDoubleValue = VariableValue
            End Sub

        End Class

    End Module

End Namespace
