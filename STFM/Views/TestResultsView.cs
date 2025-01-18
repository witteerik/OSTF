using STFN;

namespace STFM.Views;

public abstract class TestResultsView : ContentView
{
	public TestResultsView()
	{

	}

    public abstract void ShowTestResults(string results);


}