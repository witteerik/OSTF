
using Microsoft.Maui.Controls;
using STFN;

namespace STFM.Views
{

    public partial class ResponseView_AdaptiveSiP : ResponseView
    {
        public ResponseView_AdaptiveSiP()
        {
            // Loading xaml content manually since for some reason does not InitializeComponent exist
            this.LoadFromXaml(typeof(ResponseView_AdaptiveSiP));
            //InitializeComponent();
        }

        public override void AddSourceAlternatives(STFM.Views.ResponseView.VisualizedSoundSource[] soundSources)
        {
            //throw new NotImplementedException();
        }

        public override void HideAllItems()
        {
            //throw new NotImplementedException();
        }

        public override void HideVisualCue()
        {
            //throw new NotImplementedException();
        }

        public override void InitializeNewTrial()
        {
            //throw new NotImplementedException();
        }

        public override void ResponseTimesOut()
        {
            //throw new NotImplementedException();
        }

        public override void ShowMessage(string Message)
        {
            //throw new NotImplementedException();
        }

        public override void ShowResponseAlternativePositions(List<List<SpeechTestResponseAlternative>> ResponseAlternatives)
        {
            //throw new NotImplementedException();
        }

        public override void ShowResponseAlternatives(List<List<SpeechTestResponseAlternative>> ResponseAlternatives)
        {
            //throw new NotImplementedException();
        }

        public override void ShowVisualCue()
        {
            //throw new NotImplementedException();
        }

        public override void StopAllTimers()
        {
            //throw new NotImplementedException();
        }

        public override void UpdateTestFormProgressbar(int Value, int Maximum, int Minimum)
        {
            //throw new NotImplementedException();
        }
    }

}
