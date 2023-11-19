namespace STFM.Views;

public partial class OptionsSipTestView : ContentView
{

    public double SelectedPNR = 0;

	public OptionsSipTestView()
	{
		InitializeComponent();

        pnrLabel.Text = SelectedPNR.ToString();
        pnrSlider.Value = SelectedPNR;

    }

    void OnPnrSliderValueChanged(object sender, ValueChangedEventArgs args)
    {
        SelectedPNR = System.Math.Round( args.NewValue);
        pnrLabel.Text = String.Format("PNR = {0}", SelectedPNR);
    }

}