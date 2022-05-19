Imports System.Windows.Forms
Imports System.Drawing

Public Class ResponseGuiItemTable
    Inherits TableLayoutPanel

    Public Property StackedControls As Boolean
    Public Property StackOrientation As VisualStackOrientations

    Public Enum VisualStackOrientations
        Horizontal
        Vertical
    End Enum


    Private WordWrapFactor As Double? = 4

    ''' <summary>
    ''' This sub adjusts the controls if manually added. But it only works if the controls are added into a table of the same size as created by the function AddResponseControls.
    ''' </summary>
    Public Sub AdjustControls()

        Dim AllControls As TableLayoutControlCollection = Me.Controls
        Dim ControlList As New List(Of Control)

        For i = 0 To AllControls.Count - 1
            ControlList.Add(AllControls.Item(i))
        Next

        AddResponseControls(ControlList)

    End Sub

    Public Sub AddResponseControls(ByRef ControlsToAdd As List(Of Windows.Forms.Control))

        If ControlsToAdd.Count > 0 Then

            'Removing all previously added controls
            Me.Controls.Clear()


            If StackedControls = False Then

                'Calculating number of columns and rows needed
                Dim ColCellCount = Math.Ceiling(Math.Max(1, Math.Sqrt(ControlsToAdd.Count)))
                Dim RowCount = Math.Ceiling(ControlsToAdd.Count / ColCellCount)
                Dim ColumnCount = ColCellCount * 2
                Dim LastRowAdjustment As Integer = (ColCellCount * RowCount) - ControlsToAdd.Count

                'Setting the row and column counts
                Me.RowCount = RowCount
                Me.ColumnCount = ColumnCount
                Me.Dock = Windows.Forms.DockStyle.Fill
                Me.GrowStyle = Windows.Forms.TableLayoutPanelGrowStyle.FixedSize

                'Adding controls to cells
                Dim ControlIndex As Integer = 0
                For row = 0 To RowCount - 1
                    For col = 0 To ColumnCount - 1 Step 2
                        If ControlIndex >= ControlsToAdd.Count Then Exit For

                        If row = RowCount - 1 Then
                            Me.Controls.Add(ControlsToAdd(ControlIndex), col + LastRowAdjustment, row)
                            ControlIndex += 1
                        Else
                            Me.Controls.Add(ControlsToAdd(ControlIndex), col, row)
                            ControlIndex += 1
                        End If
                    Next
                Next

                'Expanding controls to two cells
                For Each control In Me.Controls
                    Me.SetColumnSpan(control, 2)
                Next

            Else

                'Noting the number of columns and rows needed
                Dim RowCount As Integer
                Dim ColumnCount As Integer

                If StackOrientation = VisualStackOrientations.Horizontal Then
                    RowCount = 1
                    ColumnCount = ControlsToAdd.Count
                Else
                    RowCount = ControlsToAdd.Count
                    ColumnCount = 1
                End If

                'Setting the row and column counts
                Me.RowCount = RowCount
                Me.ColumnCount = ColumnCount
                Me.Dock = Windows.Forms.DockStyle.Fill
                Me.GrowStyle = Windows.Forms.TableLayoutPanelGrowStyle.FixedSize

                'Adding controls to cells
                Dim ControlIndex As Integer = 0
                For row = 0 To RowCount - 1
                    For col = 0 To ColumnCount - 1
                        Me.Controls.Add(ControlsToAdd(ControlIndex), col, row)
                        ControlIndex += 1
                    Next
                Next

            End If

            'Docking the controls 
            For Each control In Me.Controls
                control.Dock = Windows.Forms.DockStyle.Fill
            Next

        End If

        AdjustRowAndColumnStyles()

    End Sub

    Public Sub SetResponseControlVisibility(ByVal Visible As Boolean)

        For Each control As Control In Me.Controls
            control.Visible = Visible
            For Each innerControl As Control In control.Controls
                innerControl.Visible = Visible
                For Each evenMoreControl As Control In innerControl.Controls
                    evenMoreControl.Visible = Visible
                Next
            Next
        Next

    End Sub

    Private Function HasText() As Boolean

        For Each item As ResponseGuiItem In Me.Controls
            If item.HasText = True Then Return True
        Next

        Return False

    End Function

    Private Sub Me_Rezise() Handles Me.Resize
        AdjustRowAndColumnStyles()
        MaximizeFontSize()
    End Sub

    Private Sub AdjustRowAndColumnStyles()

        Dim AvailableHeight = Me.ClientRectangle.Height
        Dim AvailableWidth = Me.ClientRectangle.Width
        Me.ColumnStyles.Clear()
        For col = 0 To Me.ColumnCount - 1
            Me.ColumnStyles.Add(New Windows.Forms.ColumnStyle(Windows.Forms.SizeType.Absolute, AvailableWidth / Me.ColumnCount))
        Next
        Me.RowStyles.Clear()
        For row = 0 To Me.RowCount - 1
            Me.RowStyles.Add(New Windows.Forms.RowStyle(Windows.Forms.SizeType.Absolute, AvailableHeight / Me.RowCount))
        Next

    End Sub


    Public Sub MaximizeFontSize()

        If HasText() = True Then

            'Maximizing the font size
            Dim Texts As New List(Of String)

            'Getting usable width and height (based on the size of the first control (they should all be the same size)
            Dim UsableControlWidth = Me.Controls(0).Width * 0.9 ' Using 5% as margin
            Dim UsableControlHeight = Me.Controls(0).Height * 0.9

            'Reading texts from the controls
            For Each control As ResponseGuiItem In Me.Controls
                Texts.Add(control.Text)
            Next

            'Word wrapping the text
            If WordWrapFactor.HasValue Then

                Dim SplitTexts As New List(Of String)
                For Each t In Texts

                    Dim CurrentlySplitText = t.Split(" ").ToList

                    Dim WordsPerTextRow = Math.Ceiling(CurrentlySplitText.Count / WordWrapFactor.Value)
                    Dim LineBreakInsertIndices As New List(Of Integer)
                    For i As Integer = WordsPerTextRow To CurrentlySplitText.Count - 1 Step WordsPerTextRow
                        LineBreakInsertIndices.Add(i)
                    Next

                    For i = 0 To LineBreakInsertIndices.Count - 1
                        Dim InverseIndex = LineBreakInsertIndices(LineBreakInsertIndices.Count - 1 - i)
                        CurrentlySplitText.Insert(InverseIndex, vbCrLf)
                    Next

                    SplitTexts.Add(String.Concat(CurrentlySplitText))

                Next

                Texts = SplitTexts

            End If


            Dim MaxFontSize As Single? = 70
            Dim StartFontSize As Single = 6
            Dim CurrentFont = Me.Controls(0).Font
            CurrentFont = New Drawing.Font(CurrentFont.FontFamily, StartFontSize, Drawing.GraphicsUnit.Point)

            Dim BreakOut As Boolean = False
            For s As Single = StartFontSize To MaxFontSize.Value Step 0.5

                For Each textString In Texts

                    Dim textSize = Windows.Forms.TextRenderer.MeasureText(textString, CurrentFont)

                    If textSize.Height > UsableControlHeight Then
                        BreakOut = True
                        Exit For
                    End If
                    If textSize.Width > UsableControlWidth Then
                        BreakOut = True
                        Exit For
                    End If

                Next

                If BreakOut = True Then Exit For

                CurrentFont = New Drawing.Font(CurrentFont.FontFamily, s, Drawing.GraphicsUnit.Point)

            Next

            For Each control As ResponseGuiItem In Me.Controls
                control.Font = CurrentFont
            Next

        End If

    End Sub



End Class
