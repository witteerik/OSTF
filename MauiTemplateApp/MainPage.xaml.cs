using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Numerics;

namespace MauiTemplateApp
{
    public partial class MainPage : ContentPage
    {
        // Declare an ObservableCollection
        public ObservableCollection<string> Items { get; set; }
        public ObservableCollection<Frame> ButtonItems { get; set; }

        public MainPage()
        {
            InitializeComponent();

            AddButtons1();



            //Content = new StackLayout
            //{
            //    Children = { addButton ,collectionView }
            //};

        }

        int addedAlready = 0;

        private void AddButtons1()
        {

            var words = new List<string>
            {
                "Ord 1",
                "Ord 2",
                "Ord 3",
                "Ord 4",
                "Ord 5",
                "Ord 6"
            };

            List<Frame> frames = new List<Frame>();

            for (int i = 0; i < words.Count; i++)
            {
                Button label = new Button
                {
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                label.Clicked += setButtonColor;
                label.SetBinding(Button.TextProperty, ".");
                Frame frame = new Frame
                {
                    Content = label,
                    WidthRequest = 200, // Set width smaller than the CarouselView's width
                    HeightRequest = 150, // Set width smaller than the CarouselView's width
                                         //Margin = new Thickness(10), // Margin around each frame
                                         //CornerRadius = 10,
                    BorderColor = Colors.Black,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                frames.Add(frame);
            }


            // Initialize the ObservableCollection
            ButtonItems = new ObservableCollection<Frame>();

            ButtonItems.Add(frames[0]);
            addedAlready += 1;

            for (int i = 0; i < frames.Count ; i++)
            {
                ButtonItems.Add(frames[i]);
            }

            // Create the CollectionView
            var collectionView = new CollectionView
            {
                // Set ItemsSource to the ObservableCollection
                ItemsSource = ButtonItems
            };

            var addButton = new Button
            {
                Text = "Add New Item",
                Command = new Command(() =>
                {

                    ButtonItems.Add(frames[addedAlready]);
                    addedAlready += 1;
                    // Scroll to the new item
                    collectionView.ScrollTo(frames[addedAlready], position: ScrollToPosition.MakeVisible, animate: true);
                })
            };

            // Set the main content of the page to include the CollectionView and the add button
            MyHorizontalStackLayout.Children.Add(addButton);
            MyHorizontalStackLayout.Children.Add(collectionView);

        }

        private void AddButtons2()
        {

            // Initialize the ObservableCollection
            Items = new ObservableCollection<string>
        {
            "Item 1",
            "Item 2",
            "Item 3"
        };



            // Create the CollectionView
            var collectionView = new CollectionView
            {
                // Define item template using DataTemplate
                ItemTemplate = new DataTemplate(() =>
                {
                    // Create a button for each item
                    var button = new Button
                    {
                        Margin = new Thickness(10),
                        HeightRequest = 44,
                        CornerRadius = 10,
                        BackgroundColor = Colors.LightGray,
                        TextColor = Colors.Black
                    };

                    // Bind the button text to the item it represents
                    button.SetBinding(Button.TextProperty, ".");

                    button.Clicked += ButtonClicked;

                    return button;
                }),
                // Set ItemsSource to the ObservableCollection
                ItemsSource = Items
            };

            var addButton = new Button
            {
                Text = "Add New Item",
                Command = new Command(() =>
                {
                    var newItem = $"Item {Items.Count + 1}";
                    Items.Add(newItem);
                    // Scroll to the new item
                    collectionView.ScrollTo(newItem, position: ScrollToPosition.MakeVisible, animate: true);
                })
            };

            // Set the main content of the page to include the CollectionView and the add button
            MyHorizontalStackLayout.Children.Add(addButton);
            MyHorizontalStackLayout.Children.Add(collectionView);

        }


        private void ButtonClicked(object? sender, EventArgs e)
        {
            // Handle button click events
            Button castButton = (Button)sender;

            DisplayAlert("Button Clicked", "Button" + castButton.Text +" was clicked", "OK");
        }

        CarouselView carouselView;

        private void NextBtn_Clicked(object sender, EventArgs e)
        {

            carouselView.ScrollTo(carouselView.Position+1, position: ScrollToPosition.MakeVisible);

        }

        private void PreviousBtn_Clicked(object sender, EventArgs e)
        {

            carouselView.ScrollTo(carouselView.Position -1);

        }

        private void setButtonColor(object? sender, EventArgs e)
        {
            Button castSender = (Button)sender;
            castSender.BackgroundColor = Colors.Red;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {

            var words = new List<string>
            {
                "Ord 1",
                "Ord 2",
                "Ord 3",
                "Ord 4",
                "Ord 5",
                "Ord 6"
            };

            List<Frame> frames = new List<Frame>();

            for (int i = 0; i < words.Count; i++)
            {
                Button label = new Button
                {
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                label.Clicked += setButtonColor;
                label.SetBinding(Button.TextProperty, ".");
                Frame frame = new Frame
                {
                    Content = label,
                    WidthRequest = 200, // Set width smaller than the CarouselView's width
                    HeightRequest = 150, // Set width smaller than the CarouselView's width
                                         //Margin = new Thickness(10), // Margin around each frame
                                         //CornerRadius = 10,
                    BorderColor = Colors.Black,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                frames.Add(frame);
            }

            carouselView = new CarouselView
            {
                PeekAreaInsets = new Thickness(200, 0), // Show parts of the next and previous items
            };

            carouselView.ItemsSource = frames;

            carouselView.WidthRequest = 400;

            carouselView.ItemsLayout.ItemSpacing = 10;
            carouselView.ItemsLayout.SnapPointsType = SnapPointsType.MandatorySingle;

            //carouselView.IsSwipeEnabled = false;
            //carouselView.IsBounceEnabled = false;
            //carouselView.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
            carouselView.BackgroundColor = Colors.Beige;
            //carouselView.IsScrollAnimated = false;

            // Add the carouselView to the page's layout
            //Content = new StackLayout { Children = { carouselView } };

            MyHorizontalStackLayout.Children.Add(carouselView);



            return;

            float factor = 1.5f;

            List<string> TimeSpanList = new List<string>();
            Stopwatch StopWatch = new Stopwatch();
            StopWatch.Start();

            Random rnd = new Random(42);
            int ArrayLength = (int)Math.Pow(10, 4) + 3;
            int[] Array1 = new int[ArrayLength];
            //float[] Array1 = new float[ArrayLength];
            float[] Array2 = new float[ArrayLength];
            for (int i = 0; i < ArrayLength; i++)
            {
                Array1[i] = (int)rnd.Next(100);
                //Array1[i] = (float)rnd.NextDouble();
                Array2[i] = (float)rnd.NextDouble();
            }

            StopWatch.Stop();
            TimeSpanList.Add(StopWatch.ElapsedMilliseconds.ToString());
            StopWatch.Reset();
            StopWatch.Start();

            // Performing array zip with indexed loop
            float[] ResultArray = new float[ArrayLength];
            for (int i = 0; i < Array1.Length; i++)
            {
                ResultArray[i] = Array1[i] * factor;
                //ResultArray[i] = Array1[i] * Array2[i];
            }

            StopWatch.Stop();
            TimeSpanList.Add(StopWatch.ElapsedMilliseconds.ToString());
            StopWatch.Reset();
            StopWatch.Start();

            // Performing array zip with LINQ
            //var ResultArray3 = Array1.Zip(Array2, (a, b) => a * b);
            //var ArraySum = ResultArray3.Sum();

            var result = Array1.Select(x => x * factor).ToArray();

            StopWatch.Stop();
            TimeSpanList.Add(StopWatch.ElapsedMilliseconds.ToString());
            StopWatch.Reset();
            StopWatch.Start();

            //MultiplyArrayByFactor(Array1, factor);

            StopWatch.Stop();
            TimeSpanList.Add(StopWatch.ElapsedMilliseconds.ToString());
            StopWatch.Reset();
            StopWatch.Start();

            var Sum1 = SumArray(Array1);

            StopWatch.Stop();
            TimeSpanList.Add(StopWatch.ElapsedMilliseconds.ToString());
            StopWatch.Reset();
            StopWatch.Start();

            var Sum2 = Array1.Sum();

            StopWatch.Stop();
            TimeSpanList.Add(StopWatch.ElapsedMilliseconds.ToString());
            StopWatch.Reset();
            StopWatch.Start();

            // Output elapsed milliseconds for each operation
            CounterBtn.Text = string.Join("\n", TimeSpanList);

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        public static void MultiplyArrayByFactor(float[] array, float factor)
        {
            int vectorSize = Vector<float>.Count;
            var factorVector = new Vector<float>(factor);

            int i;
            for (i = 0; i <= array.Length - vectorSize; i += vectorSize)
            {
                var v = new Vector<float>(array, i);
                v = v * factorVector;
                v.CopyTo(array, i);
            }

            // Handle any remaining elements at the end that don't fit into a full vector.
            for (; i < array.Length; i++)
            {
                array[i] *= factor;
            }
        }

        public static float SumArray(float[] array)
        {
            int vectorSize = Vector<float>.Count;
            Vector<float> sumVector = Vector<float>.Zero;

            int i;
            for (i = 0; i <= array.Length - vectorSize; i += vectorSize)
            {
                var v = new Vector<float>(array, i);
                sumVector += v;
            }

            float sum = 0;
            for (int j = 0; j < vectorSize; j++)
            {
                sum += sumVector[j];
            }

            // Handle any remaining elements at the end that don't fit into a full vector.
            for (; i < array.Length; i++)
            {
                sum += array[i];
            }

            return sum;
        }

        public static int SumArray(int[] array)
        {
            int vectorSize = Vector<int>.Count;
            Vector<int> sumVector = Vector<int>.Zero;

            int i;
            for (i = 0; i <= array.Length - vectorSize; i += vectorSize)
            {
                var v = new Vector<int>(array, i);
                sumVector += v;
            }

            int sum = 0;
            for (int j = 0; j < vectorSize; j++)
            {
                sum += sumVector[j];
            }

            // Handle any remaining elements at the end that don't fit into a full vector.
            for (; i < array.Length; i++)
            {
                sum += array[i];
            }

            return sum;
        }

    }

}
