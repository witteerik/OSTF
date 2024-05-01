using STFN;

namespace STFM.Views;

public abstract class TestResultsView : ContentView
{
	public TestResultsView()
	{
		Content = new VerticalStackLayout
		{
			Children = {
				new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
				}
			}
		};
	}

    public abstract void ShowTestResults(string results);


}