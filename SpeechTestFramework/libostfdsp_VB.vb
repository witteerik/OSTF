Imports System.Drawing
Imports System.Runtime.InteropServices

Public Class LibOstfDsp_VB

    <DllImport("libostfdsp_x64.dll", EntryPoint:="multiplyArrayBy", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function multiplyArrayBy_64(values As Single(), size As Integer, factor As Single) As Boolean
    End Function

    <DllImport("libostfdsp_x64.dll", EntryPoint:="addTwoFloatArrays", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function addTwoFloatArrays_64(array1 As Single(), array2 As Single(), size As Integer) As Boolean
    End Function

    Public Shared Function multiplyArrayBy(values As Single(), size As Integer, factor As Single) As Boolean
        'Checking whether a 32-bit or 64-bit environment is running
        If IntPtr.Size = 4 Then
            Return multiplyArrayBy_32(values, size, factor)
        Else
            Return multiplyArrayBy_64(values, size, factor)
        End If
    End Function


    <DllImport("libostfdsp_x64.dll", EntryPoint:="fft_complex", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Sub fft_complex_64(real As Double(), imag As Double(), size As Integer, Optional direction As Integer = 1, Optional reorder As Boolean = True, Optional scaleForwardTransform As Boolean = True)
    End Sub

    <DllImport("libostfdsp_x86.dll", EntryPoint:="multiplyArrayBy", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function multiplyArrayBy_32(values As Single(), size As Integer, factor As Single) As Boolean
    End Function

    Public Shared Function addTwoFloatArrays(array1 As Single(), array2 As Single(), size As Integer) As Boolean
        'Checking whether a 32-bit or 64-bit environment is running
        If IntPtr.Size = 4 Then
            Return addTwoFloatArrays_32(array1, array2, size)
        Else
            Return addTwoFloatArrays_64(array1, array2, size)
        End If
    End Function


    <DllImport("libostfdsp_x86.dll", EntryPoint:="addTwoFloatArrays", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function addTwoFloatArrays_32(array1 As Single(), array2 As Single(), size As Integer) As Boolean
    End Function

    <DllImport("libostfdsp_x86.dll", EntryPoint:="fft_complex", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Sub fft_complex_32(real As Double(), imag As Double(), size As Integer, Optional direction As Integer = 1, Optional reorder As Boolean = True, Optional scaleForwardTransform As Boolean = True)
    End Sub

    Public Shared Sub fft_complex(real As Double(), imag As Double(), size As Integer, Optional direction As Integer = 1, Optional reorder As Boolean = True, Optional scaleForwardTransform As Boolean = True)
        'Checking whether a 32-bit or 64-bit environment is running
        If IntPtr.Size = 4 Then
            fft_complex_32(real, imag, size, direction, reorder, scaleForwardTransform)
        Else
            fft_complex_64(real, imag, size, direction, reorder, scaleForwardTransform)
        End If
    End Sub


End Class