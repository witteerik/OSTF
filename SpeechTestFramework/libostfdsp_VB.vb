Imports System.Runtime.InteropServices

Public Class LibOstfDsp_VB

    <DllImport("libostfdsp_x64.dll", EntryPoint:="multiplyFloatArray", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function multiplyFloatArray_64(values As Single(), size As Integer, factor As Single) As Boolean
    End Function
    <DllImport("libostfdsp_Win32.dll", EntryPoint:="multiplyFloatArray", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function multiplyFloatArray_32(values As Single(), size As Integer, factor As Single) As Boolean
    End Function

    Public Shared Function multiplyFloatArray(values As Single(), size As Integer, factor As Single) As Boolean
        'Checking whether a 32-bit or 64-bit environment is running
        If IntPtr.Size = 4 Then
            Return multiplyFloatArray_32(values, size, factor)
        Else
            Return multiplyFloatArray_64(values, size, factor)
        End If
    End Function

    <DllImport("libostfdsp_x64.dll", EntryPoint:="multiplyFloatArraySection", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function multiplyFloatArraySection_64(values As Single(), arraySize As Integer, factor As Single, startIndex As Integer, length As Integer) As Integer
    End Function
    <DllImport("libostfdsp_Win32.dll", EntryPoint:="multiplyFloatArraySection", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function multiplyFloatArraySection_32(values As Single(), arraySize As Integer, factor As Single, startIndex As Integer, length As Integer) As Integer
    End Function

    Public Shared Function multiplyFloatArraySection(values As Single(), arraySize As Integer, factor As Single, startIndex As Integer, length As Integer) As Integer

        Dim ClippedSamplesCount As Integer = 0

        'Checking whether a 32-bit or 64-bit environment is running
        If IntPtr.Size = 4 Then
            ClippedSamplesCount = multiplyFloatArraySection_32(values, arraySize, factor, startIndex, length)
        Else
            ClippedSamplesCount = multiplyFloatArraySection_64(values, arraySize, factor, startIndex, length)
        End If

        Return ClippedSamplesCount

    End Function


    <DllImport("libostfdsp_x64.dll", EntryPoint:="calculateFloatSumOfSquare", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function calculateFloatSumOfSquare_64(values As Single(), arraySize As Integer, startIndex As Integer, sectionLength As Integer) As Double
    End Function
    <DllImport("libostfdsp_Win32.dll", EntryPoint:="calculateFloatSumOfSquare", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function calculateFloatSumOfSquare_32(values As Single(), arraySize As Integer, startIndex As Integer, sectionLength As Integer) As Double
    End Function

    Public Shared Function calculateFloatSumOfSquare(values As Single(), arraySize As Integer, startIndex As Integer, sectionLength As Integer) As Double

        Dim SumOfSquare As Double = 0

        'Checking whether a 32-bit or 64-bit environment is running
        If IntPtr.Size = 4 Then
            SumOfSquare = calculateFloatSumOfSquare_32(values, arraySize, startIndex, sectionLength)
        Else
            SumOfSquare = calculateFloatSumOfSquare_64(values, arraySize, startIndex, sectionLength)
        End If

        Return SumOfSquare

    End Function


    <DllImport("libostfdsp_x64.dll", EntryPoint:="addTwoFloatArrays", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function addTwoFloatArrays_64(array1 As Single(), array2 As Single(), size As Integer) As Boolean
    End Function

    <DllImport("libostfdsp_Win32.dll", EntryPoint:="addTwoFloatArrays", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function addTwoFloatArrays_32(array1 As Single(), array2 As Single(), size As Integer) As Boolean
    End Function

    Public Shared Function addTwoFloatArrays(array1 As Single(), array2 As Single(), size As Integer) As Boolean
        'Checking whether a 32-bit or 64-bit environment is running
        If IntPtr.Size = 4 Then
            Return addTwoFloatArrays_32(array1, array2, size)
        Else
            Return addTwoFloatArrays_64(array1, array2, size)
        End If
    End Function


    <DllImport("libostfdsp_x64.dll", EntryPoint:="fft_complex", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Sub fft_complex_64(real As Double(), imag As Double(), size As Integer, Optional direction As Integer = 1, Optional reorder As Boolean = True, Optional scaleForwardTransform As Boolean = True)
    End Sub

    <DllImport("libostfdsp_Win32.dll", EntryPoint:="fft_complex", CallingConvention:=CallingConvention.Cdecl)>
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