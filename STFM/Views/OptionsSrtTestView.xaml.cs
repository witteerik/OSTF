namespace STFM.Views;

public partial class OptionsSrtTestView : ContentView
{

    public double SelectedStartLevel= 0;

    public OptionsSrtTestView()
	{
		InitializeComponent();

        // Adding methos
        MethodPicker.Items.Add("Adaptiv HTT");
        MethodPicker.Items.Add("Konstanta stimuli");

        // Adding lists
        for (int i = 0; i < 10; i++)
        {
            ListPicker.Items.Add("Lista " + i.ToString());
        }

        startLevelLabel.Text = SelectedStartLevel.ToString();
        startLevelSlider.Value = SelectedStartLevel;

    }

    void OnStartLevelSliderValueChanged(object sender, ValueChangedEventArgs args)
    {
        SelectedStartLevel = STFN.Utils.Math.RoundToNearestIntegerMultiple(args.NewValue, 5, STFN.Utils.Math.roundingMethods.getClosestValue);
        startLevelLabel.Text = String.Format("SNR = {0}", SelectedStartLevel);
    }

}