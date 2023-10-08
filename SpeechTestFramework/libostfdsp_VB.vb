Imports System.Drawing
Imports System.Runtime.InteropServices

Public Class LibOstfDsp_VB

    <DllImport("libostfdsp.dll", CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function multiplyArrayBy(values As Single(), size As Integer, factor As Single) As Boolean
    End Function

    <DllImport("libostfdsp.dll", CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function addTwoFloatArrays(array1 As Single(), array2 As Single(), size As Integer) As Boolean
    End Function

    <DllImport("libostfdsp.dll", CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Sub fft_complex(real As Double(), imag As Double(), size As Integer, Optional direction As Integer = 1, Optional reorder As Boolean = True, Optional scaleForwardTransform As Boolean = True)
    End Sub

End Class