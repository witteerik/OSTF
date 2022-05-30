Imports System.Windows.Forms
Imports System.ComponentModel

Public Class ExistingFilePathControl
    Inherits PathControl

    Public Sub New()

        MyBase.New

        Me.PathTextBox.DirectionType = PathTextBox.DirectionTypes.Load
        Me.PathTextBox.PathType = PathTextBox.PathTypes.File

    End Sub


End Class
