Imports System
Imports System.IO

Module Program
    Sub Main(args As String())
        Console.WriteLine("This program merges all text files in the a specified directory into a single text file located in a new subdirectory.")

        Console.WriteLine(vbCrLf & "Please enter the directory, and press Enter:")
        Dim TargetDirectory = Console.ReadLine
        If IO.Directory.Exists(TargetDirectory) = False Then
            Console.WriteLine(vbCrLf & "The entered directory does not exist. Press any key to close the application.")
            Console.ReadKey()
            Exit Sub
        End If

        Dim AvailableTextFiles = IO.Directory.GetFiles(TargetDirectory, "*.txt")
        If AvailableTextFiles.Length = 0 Then
            Console.WriteLine(vbCrLf & "There are no text files in the entered directory. Press any key to close the application.")
            Console.ReadKey()
            Exit Sub
        End If

        Console.WriteLine(vbCrLf & "You can choose to skip any number of rows from the beginning of each text file.")
        Console.WriteLine(vbCrLf & "Please enter the number of rows to skip in each text file, and then press Enter")
        Dim LinesToSkipKeyInput = Console.ReadLine
        Dim LinesToSkip As Integer = 0
        If Integer.TryParse(LinesToSkipKeyInput, LinesToSkip) = False Then
            Console.WriteLine(vbCrLf & "You have entered an invalid value. It must be a positive integer. Press any key to close the application.")
            Console.ReadKey()
            Exit Sub
        End If

        If LinesToSkip < 0 Then
            Console.WriteLine(vbCrLf & "You have entered an invalid value. It must be a positive integer. Press any key to close the application.")
            Console.ReadKey()
            Exit Sub
        End If

        Dim HeadingLineIndex As Integer = -1
        If LinesToSkip > 0 Then
            Console.WriteLine(vbCrLf & "If the heading line is removed by skipping file initial rows it can be added from the first file by specifying its zero based row index.")
            Console.WriteLine(vbCrLf & "Please enter the heading line index or -1 to skip the addition of a heading line, and then press Enter")
            Dim HeadingLineIndexInput = Console.ReadLine
            If Integer.TryParse(HeadingLineIndexInput, HeadingLineIndex) = False Then
                Console.WriteLine(vbCrLf & "You have entered an invalid value. Press any key to close the application.")
                Console.ReadKey()
                Exit Sub
            End If
        End If

        Console.WriteLine(vbCrLf & "Working on it...")

        If STFN.Utils.MergeTextFiles(LinesToSkip, Text.Encoding.UTF8, HeadingLineIndex, TargetDirectory) = True Then
            Console.WriteLine(vbCrLf & vbCrLf & "Merging completed. Press any key to close the application.")
        Else
            Console.WriteLine(vbCrLf & vbCrLf & "An error occurred during merging. Press any key to close the application.")
        End If

        Console.ReadKey()

    End Sub
End Module
