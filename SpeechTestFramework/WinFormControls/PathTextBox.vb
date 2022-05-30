Imports System.Windows.Forms
Imports System.ComponentModel

''' <summary>
''' A TextBox that can be used to set or get file of folder paths from the user. It performes a range of checks on input text and reportes back to the user using text color. 
''' Red text color indicates invalid value, and default color indicates valid value. Valid values should be retrieved from the property SelectedPath.
''' </summary>
<Serializable>
Public Class PathTextBox
    Inherits TextBox

    Private _DirectionType As DirectionTypes = DirectionTypes.Load
    ''' <summary>
    ''' Determines whether the control is to be used for reading/loading or writing/saving paths
    ''' </summary>
    ''' <returns></returns>
    Public Property DirectionType As DirectionTypes
        Get
            Return _DirectionType
        End Get
        Set(value As DirectionTypes)
            _DirectionType = value

            OnTextChanged(Nothing)

        End Set
    End Property
    Public Enum DirectionTypes
        Load
        Save
    End Enum

    Private _PathType As PathTypes = PathTypes.File
    Public Property PathType As PathTypes
        Get
            Return _PathType
        End Get
        Set(value As PathTypes)
            _PathType = value

            OnTextChanged(Nothing)

        End Set
    End Property

    Public Enum PathTypes
        File
        Folder
    End Enum

    Private _SelectedPath As String = String.Empty
    Public ReadOnly Property SelectedPath As String
        Get
            Return _SelectedPath
        End Get
    End Property


    Public Property AllowedFileExtensions As String() = {}

    Public Function GetFileExtensionFilter() As String

        'Creating a filterstring
        If AllowedFileExtensions IsNot Nothing Then
            Dim filter As String = ""
            For ext = 0 To AllowedFileExtensions.Length - 1
                filter &= AllowedFileExtensions(ext).Trim(".") & " files (*." & AllowedFileExtensions(ext).Trim(".") & ")|*." & AllowedFileExtensions(ext).Trim(".") & "|"
            Next
            filter = filter.TrimEnd("|")
            Return filter
        Else
            Return ""
        End If

    End Function

    Private DefaultTextColor As Drawing.Color

    Public Sub New()

        'Stores the default color
        DefaultTextColor = Me.ForeColor

    End Sub

    Public Sub New(ByVal PathType As PathTypes, ByVal DirectionType As DirectionTypes)

        'Stores the default color
        DefaultTextColor = Me.ForeColor

        'Stores the path type
        Me.PathType = PathType

        'Stores the EnablePathCheck value
        Me.DirectionType = DirectionType

    End Sub


    Protected Overrides Sub OnTextChanged(e As EventArgs)
        MyBase.OnTextChanged(e)


        Select Case DirectionType
            Case DirectionTypes.Load

                'Tries to parse the text as an existing path
                Select Case PathType
                    Case PathTypes.File

                        If IO.File.Exists(Me.Text.Trim) Then
                            Me.ForeColor = DefaultTextColor
                            Me._SelectedPath = Me.Text.Trim
                        Else
                            Me.ForeColor = Drawing.Color.Red
                            Me._SelectedPath = String.Empty
                        End If

                    Case PathTypes.Folder

                        If IO.Directory.Exists(Me.Text.Trim) Then
                            Me.ForeColor = DefaultTextColor
                            Me._SelectedPath = Me.Text.Trim
                        Else
                            Me.ForeColor = Drawing.Color.Red
                            Me._SelectedPath = String.Empty
                        End If

                End Select


            Case DirectionTypes.Save

                Dim AllOk As Boolean = True
                Dim TrimmedInput As String = Me.Text.Trim

                'Checking that there is an input string
                If String.IsNullOrEmpty(TrimmedInput) = True Then AllOk = False

                Dim Folder As String = ""
                If AllOk = True Then
                    ' Checks the folder name
                    Folder = IO.Path.GetDirectoryName(TrimmedInput)
                    If String.IsNullOrEmpty(Folder) = True Then
                        AllOk = False
                    Else
                        'Checks for invalid characters in the folder name
                        If PathType = PathTypes.Folder Then
                            'Checks for invalid characters
                            Dim InvalidChars() As Char = IO.Path.GetInvalidPathChars
                            For Each c In InvalidChars
                                If Folder.Contains(c) Then
                                    AllOk = False
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                End If

                'Checking that the folder exists
                If AllOk = True Then If IO.Directory.Exists(Folder) = False Then AllOk = False

                If AllOk = True Then
                    If PathType = PathTypes.File Then
                        'Checks the file name
                        Dim FileName As String = IO.Path.GetFileName(TrimmedInput)
                        If String.IsNullOrEmpty(FileName) = True Then
                            AllOk = False
                        Else
                            'Checks for invalid characters in the file name
                            If PathType = PathTypes.File Then
                                'Checks for invalid characters
                                Dim InvalidChars() As Char = IO.Path.GetInvalidFileNameChars
                                For Each c In InvalidChars
                                    If FileName.Contains(c) Then
                                        AllOk = False
                                        Exit For
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If

                'Finally adjusts the color and selected path values
                If AllOk = True Then
                    Me.ForeColor = DefaultTextColor
                    Me._SelectedPath = TrimmedInput
                Else
                    Me.ForeColor = Drawing.Color.Red
                    Me._SelectedPath = String.Empty
                End If

        End Select

    End Sub

End Class