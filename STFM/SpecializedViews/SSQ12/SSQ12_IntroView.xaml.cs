namespace STFM.SpecializedViews.SSQ12;

public partial class SSQ12_IntroView : ContentView
{
	public SSQ12_IntroView()
	{
		InitializeComponent();
	}


    private void HaQuestionRadioButton1_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        HaHistoryGrid.IsVisible = false;
    }

    private void HaQuestionRadioButton234_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        HaHistoryGrid.IsVisible = true;
    }


}