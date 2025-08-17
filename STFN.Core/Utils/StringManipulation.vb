Namespace Utils

    Public Class StringManipulation

        Public Shared Function SplitStringByLines(input As String) As String()

            Return input.Split({vbCrLf, vbLf, vbCr}, StringSplitOptions.None)

            'As an alternative using regex, this expression should match any combination of \r and \n
            'Return Regex.Split(input, "\r\n|\n\r|\r|\n")
        End Function

    End Class

End Namespace