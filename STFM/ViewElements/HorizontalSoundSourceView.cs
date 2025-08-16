using STFN.Core.Audio.SoundScene;

namespace STFM.Views;

public class HorizontalSoundSourceView : Frame
{

    AbsoluteLayout SoundSourceLayout = new AbsoluteLayout();
    Grid ContentGrid = new Grid();
    Label TitleLabel = new Label();

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(String), typeof(HorizontalSoundSourceView), null, BindingMode.TwoWay);

    public String Title
    {
        get
        {
            return (String)GetValue(TitleProperty);
        }
        set
        {
            SetValue(TitleProperty, value);
        }
    }

    public static readonly BindableProperty MaxSelectedProperty =
    BindableProperty.Create(nameof(MaxSelected), typeof(int), typeof(HorizontalSoundSourceView), 1, BindingMode.OneWay);

    int maxSelected;
    public int MaxSelected
    {
        get
        {
            //return maxSelected;
            return (int)GetValue(MaxSelectedProperty);
        }
        set
        {
            //maxSelected = value;
            SetValue(MaxSelectedProperty, value);
        }
    }

    public static readonly BindableProperty MinSelectedProperty =
    BindableProperty.Create(nameof(MinSelected), typeof(int), typeof(HorizontalSoundSourceView), 1, BindingMode.OneWay);

    int minSelected;
    public int MinSelected
    {
        get
        {
            //return minSelected;
            return (int)GetValue(MinSelectedProperty);
        }
        set
        {
            //minSelected = value;
            SetValue(MinSelectedProperty, value);
        }
    }


    STFN.Core.Audio.SoundScene.SoundSceneItem.SoundSceneItemRoles roleType;
    public STFN.Core.Audio.SoundScene.SoundSceneItem.SoundSceneItemRoles RoleType
    {
        get
        {
            return roleType;
        }
        set
        {
            roleType = value;
        }
    }

    public static readonly BindableProperty IsSoundFieldSimulationProperty =
        BindableProperty.Create(nameof(IsSoundFieldSimulation), typeof(bool), typeof(HorizontalSoundSourceView), false, BindingMode.TwoWay);

        public bool IsSoundFieldSimulation
        {
            get
            {
                return (bool)GetValue(IsSoundFieldSimulationProperty);
            }
            set
            {
                SetValue(IsSoundFieldSimulationProperty, value);
            }
        }


    public static readonly BindableProperty SoundSourcesProperty =
        BindableProperty.Create(nameof(SoundSources), typeof(List<VisualSoundSourceLocation>), typeof(HorizontalSoundSourceView), null, BindingMode.TwoWay);

    public List<VisualSoundSourceLocation> SoundSources
    {
        get
        {
            return (List<VisualSoundSourceLocation>)GetValue(SoundSourcesProperty);
        }
        set
        {
            SetValue(SoundSourcesProperty, value);
            UpdateSoundSources();
        }
    }

    private List<VisualSoundSourceSelectionButton> ButtonList = new List<VisualSoundSourceSelectionButton>();


    private bool isHeadPhones = false;

    public event EventHandler ValueChanged;

    public HorizontalSoundSourceView() {

        this.WidthRequest = 250;
        this.HeightRequest = 250 * (9/8);

        this.BackgroundColor = Colors.White;

        ContentGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(8, GridUnitType.Star) });
        ContentGrid.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        TitleLabel.TextColor = Colors.Black; //Color.FromRgb(80, 80, 80);
        TitleLabel.Text = Title;
        TitleLabel.HorizontalTextAlignment = TextAlignment.Center;
        TitleLabel.VerticalTextAlignment= TextAlignment.Center;
        ContentGrid.Add(TitleLabel, 0, 1);
        ContentGrid.Add(SoundSourceLayout, 0, 0);

        Content = ContentGrid;
        BorderColor = Colors.Black;
        HasShadow = false;
        Padding = 5;

    }



    private void UpdateSoundSources()
    {

        //Resetting isHeadPhones 
        isHeadPhones = false;

        // Clearing SoundSourceLayout
        SoundSourceLayout.Children.Clear();
        ButtonList.Clear();

        // Settign Title
        TitleLabel.Text = Title;

        // Adding a centre point
        Label label1 = new Label();
        label1.BackgroundColor = Color.FromRgb(80, 80, 80);
        SoundSourceLayout.Children.Add(label1);
        SoundSourceLayout.SetLayoutBounds(label1, new Rect(0.5, 0.5, 0.1, 0.03));
        SoundSourceLayout.SetLayoutFlags(label1, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.All);

        Label label2 = new Label();
        label2.BackgroundColor = Color.FromRgb(80, 80, 80);
        SoundSourceLayout.Children.Add(label2);
        SoundSourceLayout.SetLayoutBounds(label2, new Rect(0.5, 0.5, 0.03, 0.1));
        SoundSourceLayout.SetLayoutFlags(label2, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.All);

        Label labelL = new Label();
        labelL.TextColor = Colors.Blue;
        labelL.Text = "L";
        labelL.HorizontalTextAlignment = TextAlignment.Center;
        labelL.VerticalTextAlignment= TextAlignment.Center;
        labelL.FontSize = 18;

        SoundSourceLayout.Children.Add(labelL);
        SoundSourceLayout.SetLayoutBounds(labelL, new Rect(0.05, 0.05, 0.15, 0.15));
        SoundSourceLayout.SetLayoutFlags(labelL, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.All);

        Label labelR = new Label();
        labelR.TextColor = Colors.Red;
        labelR.Text = "R";
        labelR.HorizontalTextAlignment = TextAlignment.Center;
        labelR.VerticalTextAlignment = TextAlignment.Center;
        labelR.FontSize = 18;

        SoundSourceLayout.Children.Add(labelR);
        SoundSourceLayout.SetLayoutBounds(labelR, new Rect(0.95, 0.05, 0.15, 0.15));
        SoundSourceLayout.SetLayoutFlags(labelR, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.All);

        if (SoundSources.Count > 0)
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

            if (max > 0)
            {
                foreach (VisualSoundSourceLocation source in SoundSources)
                {
                    source.Scale(0.9 / (2 * max));
                }
            }
            else
            {
                // Noting that we have head phones /TODO: Note that this will fail, if we have other channels going out to feedback to the test adiminstrator...
                if (SoundSources.Count == 2) { isHeadPhones = true; }
                
                // This will occur when we have headphones, but should not otherwise occur. Overriding the distance value to get spacial separation of left and right headphone (which would otherwise be on top of each other).
                // Also increaing the height to look a little more like headsets
                foreach (VisualSoundSourceLocation source in SoundSources)
                {
                    if (source.ParentSoundSourceLocation.HorizontalAzimuth == -90) { 
                        source.X = -0.2;
                        source.Height = 0.3;
                        source.Width = 0.15;
                    }
                    if (source.ParentSoundSourceLocation.HorizontalAzimuth == 90) { 
                        source.X = 0.2;
                        source.Height = 0.3;
                        source.Width = 0.15;
                    }
                }
            }

            foreach (VisualSoundSourceLocation source in SoundSources)
            {
                source.Shift(0.5);
            }


            //if (isHeadPhones)
            //{

            //    // Actually skipping the arc since its rather misleading, as the control looks like we watch a person from the front instead of from above....
            //    // Creating an Ellipse that should look like the headphone arc
            //    var ellipse = new Microsoft.Maui.Controls.Shapes.Ellipse
            //    {
            //        Stroke = Brush.SlateGray,
            //        WidthRequest = 120,
            //        HeightRequest = 120,
            //        StrokeDashArray = new DoubleCollection { 46 },
            //        StrokeLineCap = Microsoft.Maui.Controls.Shapes.PenLineCap.Round,
            //        StrokeThickness = 4,
            //        //Margin = new Thickness(2)
            //    };

            //    SoundSourceLayout.Children.Add(ellipse);
            //    SoundSourceLayout.SetLayoutBounds(ellipse, new Rect(0.50, 0.50, 1, 1));
            //    SoundSourceLayout.SetLayoutFlags(ellipse, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.All);

            //}



            for (int i = 0; i < SoundSources.Count; i++)
            {

                var source = SoundSources[i];

                var sourceBotton = new VisualSoundSourceSelectionButton(ref source, roleType)
                {
                    //Text = source.ParentSoundSourceLocation.HorizontalAzimuth.ToString(),
                    //Padding = 10
                };

                // Selecting the first sound source as default, if Target in headphones, not simulated sound field
                if (i == 0 && isHeadPhones == true && IsSoundFieldSimulation == false && RoleType == SoundSceneItem.SoundSceneItemRoles.Target)
                {
                    sourceBotton.IsSelected = true;
                }

                // Selects front location for target type as default if sound field (real or simulated) (i.e. not headphones without sound field simulation)
                if (isHeadPhones == false & RoleType == SoundSceneItem.SoundSceneItemRoles.Target)
                {
                    if (source.ParentSoundSourceLocation.Distance > 0 && source.ParentSoundSourceLocation.HorizontalAzimuth == 0)
                    {
                        sourceBotton.IsSelected = true;
                    }
                }

                //sourceBotton.TextColor = Color.FromRgb(40, 40, 40);
                //sourceBotton.FontSize = 20;
                //sourceBotton.FontAutoScalingEnabled = true;

                sourceBotton.Clicked += button_Clicked;

                sourceBotton.Rotation = source.Rotation + 90;
                SoundSourceLayout.Children.Add(sourceBotton);

                SoundSourceLayout.SetLayoutBounds(sourceBotton, new Rect(source.X, source.Y, source.Width, source.Height));
                SoundSourceLayout.SetLayoutFlags(sourceBotton, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.All);

                ButtonList.Add(sourceBotton);

            }

        }
    }


    private void button_Clicked(object sender, EventArgs e)
    {

        VisualSoundSourceSelectionButton castButton = (VisualSoundSourceSelectionButton)sender;

        // Limiting the number of selected sound sources
        int selectedCount = GetSelectedCount();
        if (castButton.IsSelected == false)
        {
            // Checking if max number of selected buttons has been reached, and if so, deselects the first item (not in order, as the order of selection is not stored)
            if (selectedCount >= MaxSelected) { DeSelectedFirstButton(); }
            castButton.IsSelected = true;
        }
        else
        {
            //Swapping the value, only if we at lest have MinSelected after the deselection
            if (selectedCount > MinSelected) 
            {
                castButton.IsSelected = false;
            }
        }
    }

    private void DeSelectedFirstButton()
    {
        foreach (var item in ButtonList)
        {
            if (item.IsSelected == true) {
                item.IsSelected = false;
                return;
            }
        }
    }

    private int GetSelectedCount()
    {
        int selectedCount = 0;
        foreach (var item in SoundSources)
        {
            if (item.Selected == true) { selectedCount += 1;}
        }
        return selectedCount;
    }


    private void valueChanged(object sender, EventArgs e)
    {

        //Raising the ValueChanged event (which can be listened to by an external class)
        ValueChanged?.Invoke(sender, e);

    }

}

public class VisualSoundSourceSelectionButton : Button
{

    VisualSoundSourceLocation VisualSoundSourceLocation;
    STFN.Core.Audio.SoundScene.SoundSceneItem.SoundSceneItemRoles RoleType;


    public VisualSoundSourceSelectionButton(ref VisualSoundSourceLocation visualSoundSourceLocation, STFN.Core.Audio.SoundScene.SoundSceneItem.SoundSceneItemRoles roleType)
    {
        this.VisualSoundSourceLocation = visualSoundSourceLocation;
        RoleType = roleType;
        UpdateColor();
    }

    public bool IsSelected {
        get 
        { 
            return VisualSoundSourceLocation.Selected; 
        }
        set {
            VisualSoundSourceLocation.Selected = value;
            UpdateColor();
        } 
    }

    void UpdateColor()
    {
        if (VisualSoundSourceLocation.Selected)
        {
            switch (RoleType)
            {   
                case SoundSceneItem.SoundSceneItemRoles.Target:
                    BackgroundColor = Colors.LightGreen;
                    break;
                case SoundSceneItem.SoundSceneItemRoles.Masker:
                    BackgroundColor = Colors.Red;
                    break;
                case SoundSceneItem.SoundSceneItemRoles.BackgroundNonspeech:
                    BackgroundColor = Colors.Pink;
                    break;
                case SoundSceneItem.SoundSceneItemRoles.BackgroundSpeech:
                    BackgroundColor = Colors.BlueViolet;
                    break;
                default:
                    BackgroundColor = Colors.Black;
                    break;
            }
        }
        else
        {
            BackgroundColor = Colors.WhiteSmoke;
        }
    }
}
