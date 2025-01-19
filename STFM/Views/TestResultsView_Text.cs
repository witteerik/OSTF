using Microsoft.Maui.Platform;
using STFN;

namespace STFM.Views;

public class TestResultsView_Text : TestResultsView
{

    ScrollView MainScrollView;
    Label ResultsLabel;

    public TestResultsView_Text()
	{

        MainScrollView = new Microsoft.Maui.Controls.ScrollView();
        ResultsLabel = new Label();
        MainScrollView.Content = ResultsLabel;

        //MainTestResultsGrid = new Grid
        //{
        //    RowDefinitions = { new RowDefinition { Height = new GridLength(1, GridUnitType.Star) } },
        //    ColumnDefinitions = { new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } }
        //};

        Content = MainScrollView;
    }

    public override void ShowTestResults(string results)
    {
        ResultsLabel.Text = results;
     }

    public override void ShowTestResults(SpeechTest speechTest)
    {

    }

    public override void UpdateStartButtonText(string text) 
    {
        // Ignores this
    }

}