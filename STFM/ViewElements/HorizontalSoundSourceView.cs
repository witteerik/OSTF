using STFN;
using STFN.Audio.SoundScene;


namespace STFM.Views;

public class HorizontalSoundSourceView : AbsoluteLayout
{


    // Define a bindable property for SoundSources
    public static readonly BindableProperty SoundSourcesProperty =
        BindableProperty.Create(nameof(SoundSources), typeof(List<VisualSoundSourceLocation>), typeof(HorizontalSoundSourceView), null, BindingMode.TwoWay);

    // .NET property wrapper for the bindable property
    public List<VisualSoundSourceLocation> SoundSources
    {
        get
        {
            return (List<VisualSoundSourceLocation>)GetValue(SoundSourcesProperty);
        }
        set
        {

            SetValue(SoundSourcesProperty, value);

            AddSoundSources();

        }
    }


    public event EventHandler ValueChanged;

    public HorizontalSoundSourceView() {

        this.WidthRequest = 200;
        this.HeightRequest = 200;
        this.BackgroundColor = Colors.Red;

    }
     


    private void AddSoundSources()
    {

        foreach (VisualSoundSourceLocation source in SoundSources)
        {
            source.CalculateXY();
        }

        // Scaling to window
        double max = double.MinValue;
        foreach (VisualSoundSourceLocation source in SoundSources)
        {
            max = Math.Max(max, Math.Abs(source.X));
            max = Math.Max(max, Math.Abs(source.Y));
        }

        foreach (VisualSoundSourceLocation source in SoundSources)
        {
            source.Scale(0.9 / (2*max));
        }

        foreach (VisualSoundSourceLocation source in SoundSources)
        {
            source.Shift(0.5);
        }

        for (int i = 0; i < SoundSources.Count; i++)
        {

            var source = SoundSources[i];
            
            var sourceBotton = new VisualSoundSourceSelectionButton(ref source)
            {
                Text = source.ParentSoundSourceLocation.HorizontalAzimuth.ToString(),
                //Padding = 10
            };

            sourceBotton.TextColor = Color.FromRgb(40, 40, 40);
            //sourceBotton.FontSize = 20;
            sourceBotton.FontAutoScalingEnabled = true;

            sourceBotton.Clicked += button_Clicked;

            sourceBotton.Rotation = source.Rotation + 90;
            this.Children.Add(sourceBotton);

            this.SetLayoutBounds(sourceBotton, new Rect(source.X, source.Y, source.Width, source.Height));
            this.SetLayoutFlags(sourceBotton, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.All);

            // We need to use a visual control that can hold a reference to the original VisualSoundSourceLocation, and swap the Selected Value when clicked

        }
    }

    private void button_Clicked(object sender, EventArgs e)
    {

        VisualSoundSourceSelectionButton castButton = (VisualSoundSourceSelectionButton)sender;

        //Swapping the value
        castButton.IsSelected = !castButton.IsSelected;

        //valueChanged

    }


    private void valueChanged(object sender, EventArgs e)
    {

        //Raising the ValueChanged event (which can be listened to by an external class)
        ValueChanged?.Invoke(sender, e);

    }


}

public class VisualSoundSourceSelectionButton : Button
{

    VisualSoundSourceLocation visualSoundSourceLocation;
    
    public VisualSoundSourceSelectionButton(ref VisualSoundSourceLocation visualSoundSourceLocation)
    {
        this.visualSoundSourceLocation = visualSoundSourceLocation;
        UpdateColor();
    }

    public bool IsSelected {
        get 
        { 
            return visualSoundSourceLocation.Selected; 
        }
        set {
            visualSoundSourceLocation.Selected = value;
            UpdateColor();
        } 
    }

    void UpdateColor()
    {
        if (visualSoundSourceLocation.Selected)
        {
            BackgroundColor = Colors.LightGreen;
        }
        else
        {
            BackgroundColor = Colors.WhiteSmoke;
        }
    }
}
