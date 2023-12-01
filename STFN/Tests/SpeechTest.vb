Public MustInherit Class SpeechTest

    ''' <summary>
    ''' An object shared between all instances of Speechtest that hold every loaded SpeechtestSpecification and 
    ''' Speech Material component to prevent the need for re-loading between tests. 
    ''' (Note that this also means that test specifications and speech material components should not be altered once loaded.
    ''' </summary>
    ''' <returns></returns>
    Private Shared Property LoadedSpeechMaterialSpecifications As New SortedList(Of String, SpeechMaterialSpecification)

    'A shared function to load tests
    Public Shared Function GetAvailableSpeechMaterialSpecifications() As List(Of String)
        Dim OutputList As New List(Of String)
        OstfBase.LoadAvailableSpeechMaterialSpecifications()
        For Each test In OstfBase.AvailableSpeechMaterials
            OutputList.Add(test.Name)
        Next
        Return OutputList
    End Function

    ''' <summary>
    ''' The SpeechMaterialName of the currently implemented speech material specification
    ''' </summary>
    ''' <returns></returns>
    Public Property SpeechMaterialName As String


    Public Property SpeechMaterialSpecification As SpeechMaterialSpecification
        Get
            If LoadedSpeechMaterialSpecifications.ContainsKey(SpeechMaterialName) Then
                Return LoadedSpeechMaterialSpecifications(SpeechMaterialName)
            Else
                Return Nothing
            End If
        End Get
        Set(value As SpeechMaterialSpecification)
            LoadedSpeechMaterialSpecifications(SpeechMaterialName) = value
        End Set
    End Property

    Public ReadOnly Property SpeechMaterial As SpeechMaterialComponent
        Get
            If SpeechMaterialSpecification Is Nothing Then
                Return Nothing
            Else
                If SpeechMaterialSpecification.SpeechMaterial Is Nothing Then
                    SpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(SpeechMaterialSpecification.GetSpeechMaterialFilePath(), SpeechMaterialSpecification.GetTestRootPath())
                    SpeechMaterialSpecification.SpeechMaterial = SpeechMaterial
                    SpeechMaterial.ParentTestSpecification = SpeechMaterialSpecification
                End If

                If SpeechMaterialSpecification.SpeechMaterial Is Nothing Then
                    Return Nothing
                Else
                    Return SpeechMaterialSpecification.SpeechMaterial
                End If
            End If
        End Get
    End Property

    Public Sub New(ByVal SpeechMaterialName As String, ByRef AvailableTestProtocols As TestProtocols)
        Me.SpeechMaterialName = SpeechMaterialName
        LoadSpeechMaterialSpecification(SpeechMaterialName)
        Me.AvailableTestProtocols = AvailableTestProtocols

    End Sub

    Private Function LoadSpeechMaterialSpecification(ByVal SpeechMaterialName As String, Optional ByVal EnforceReloading As Boolean = False) As Boolean

        If LoadedSpeechMaterialSpecifications.ContainsKey(SpeechMaterialName) = False Or EnforceReloading = True Then

            'Removes the SpeechMaterial with SpeechMaterialName if already present
            LoadedSpeechMaterialSpecifications.Remove(SpeechMaterialName)

            'Looking for the speech material
            OstfBase.LoadAvailableSpeechMaterialSpecifications()
            For Each Test In OstfBase.AvailableSpeechMaterials
                If Test.Name = SpeechMaterialName Then
                    'Adding it if found
                    LoadedSpeechMaterialSpecifications.Add(SpeechMaterialName, Test)
                    Exit For
                End If
            Next
        End If

        'Returns true if added (or already present) or false if not found
        Return LoadedSpeechMaterialSpecifications.ContainsKey(SpeechMaterialName)

    End Function

    Public Function GetAvailableMediasetNames() As List(Of String)
        Return GetAvailableMediasets.GetNames
    End Function

    Public Function GetAvailableMediasets() As MediaSetLibrary
        SpeechMaterial.ParentTestSpecification.LoadAvailableMediaSetSpecifications()
        Return SpeechMaterial.ParentTestSpecification.MediaSets
    End Function

    Public Shared Randomizer As Random = New Random


#Region "Test protocol"

    Protected Property AvailableTestProtocols As TestProtocols
    Protected Property SelectedTestProtocol As TestProtocol

    Public Function GetAvailableTestProtocols() As List(Of TestProtocol)
        Return AvailableTestProtocols
    End Function

    Public Sub SelectTestProtocol(ByRef SelectedTestProtocol As TestProtocol)
        Me.SelectedTestProtocol = SelectedTestProtocol
    End Sub


#End Region



#Region "RunningTest"

    Public CurrentTestTrial As TestTrial

#End Region

#Region "TestResults"

    Public Shared Function GetAverageScore(ByVal Trials As IEnumerable(Of TestTrial)) As Double?

        Dim ScoreList As New List(Of Integer)
        For Each Trial In Trials
            ScoreList.Add(Trial.Score)
        Next
        If ScoreList.Count > 0 Then
            Return ScoreList.Average
        Else
            Return Nothing
        End If

    End Function

    Public Shared Function GetNumbersOfCorrectTrials(ByVal Trials As IEnumerable(Of TestTrial)) As Double?

        Dim ScoreList As New List(Of Integer)
        For Each Trial In Trials
            ScoreList.Add(Trial.Score)
        Next
        If ScoreList.Count > 0 Then
            Return ScoreList.Average
        Else
            Return Nothing
        End If

    End Function



#End Region

#Region "Settings"

    Public Method As SpeechTestMethods
    Public RandomizeWordsWithinLists As Boolean = True
    Public IsFreeRecall As Boolean
    Public FixedResponseAlternativeCount As Integer = 0

    Public Enum SpeechTestMethods
        Adaptive
        ConstantStimuli
    End Enum


#End Region


#Region "MustOverride members used in derived classes"

    Public MustOverride Function InitializeCurrentTest() As Boolean

    ''' <summary>
    ''' This method must be implemented in the derived class and must return a decision on what steps to take next. If the next step to take involves a new test trial this method is also responsible for referencing the next test trial in the CurrentTestTrial field.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <returns></returns>
    Public MustOverride Function GetSpeechTestReply(sender As Object, e As SpeechTestInputEventArgs) As SpeechTestReplies

    Public Enum SpeechTestReplies
        ContinueTrial
        GotoNextTrial
        TestIsCompleted
        AbortTest
    End Enum


    Public MustOverride Function GetResults() As TestResults

    Public MustOverride Function SaveResults(TestResults As TestResults) As Boolean

#End Region


End Class

