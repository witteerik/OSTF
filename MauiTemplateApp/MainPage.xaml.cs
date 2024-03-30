using Microsoft.Maui.Primitives;
using System.Diagnostics;
using System.Numerics;

namespace MauiTemplateApp
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {

            float factor = 1.5f;

            List<string> TimeSpanList = new List<string>();
            Stopwatch StopWatch = new Stopwatch();
            StopWatch.Start();

            Random rnd = new Random(42);
            int ArrayLength = (int)Math.Pow(10, 4)+3;
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
