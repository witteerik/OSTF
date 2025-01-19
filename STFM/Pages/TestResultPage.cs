namespace STFM.Pages;

public class TestResultPage : ContentPage
{

	STFM.Views.TestResultsView CurrentTestResultsView;

    public TestResultPage(ref STFM.Views.TestResultsView currentTestResultsView)

    {

        CurrentTestResultsView = currentTestResultsView;

        Content = CurrentTestResultsView;

        // Prevent closing: https://learn.microsoft.com/en-us/answers/questions/1336207/how-to-remove-close-and-maximize-button-for-a-maui

    }
}