using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STFN;

namespace STFM
{

    public static class StfmBase
    {

       public static STFN.Audio.SoundPlayers.iSoundPlayer SoundPlayer = null;

        public static void InitiateSTFM(Microsoft.Maui.Controls.AbsoluteLayout ParentContainer, string MediaRootDirectory)
        {

            // Initializing OSTF
            OstfBase.InitializeOSTF(MediaRootDirectory);

            // Initializing the sound player
            SoundPlayer = new STFM.SoundPlayer(ParentContainer);

            
        }

        public static void SetupTest()
        {

            Random rnd = new Random();

            // Initializing all components
            OstfBase.LoadAvailableTestSpecifications();

            string SpeechMaterialName = "Swedish SiP-test";

            SpeechMaterialSpecification SelectedTest = null;
            foreach (var ts in OstfBase.AvailableTests)
            {
                if (ts.Name == SpeechMaterialName)
                {
                    SelectedTest = ts;
                    break;
                }
            }

            SpeechMaterialComponent SpeechMaterial = SpeechMaterialComponent.LoadSpeechMaterial(
                SelectedTest.GetSpeechMaterialFilePath(), SelectedTest.GetTestRootPath());
            SpeechMaterial.ParentTestSpecification = SelectedTest;
            SelectedTest.SpeechMaterial = SpeechMaterial;

            // Loading media sets
            SpeechMaterial.ParentTestSpecification.LoadAvailableMediaSetSpecifications();
            var AvailableMediaSets = SpeechMaterial.ParentTestSpecification.MediaSets;
            var selectedMediaSet = AvailableMediaSets[0];

            var x = 1;

            var TestWords = SpeechMaterial.GetAllDescenentsAtLevel(SpeechMaterialComponent.LinguisticLevels.Sentence);

            int InitialMargin = 0;

            List<STFN.Audio.Sound.SpeechMaterialAnnotation.SmaComponent> SmaList = new List<STFN.Audio.Sound.SpeechMaterialAnnotation.SmaComponent>();

            STFN.Audio.Sound sound1 = TestWords[0].GetSound(ref selectedMediaSet, 1, 1, null, null, null, ref InitialMargin, false, false, false, null, ref SmaList,false);

            SoundPlayer.SwapOutputSounds(ref sound1);


            //SipTest.SipMeasurement SiPMeasurement = new SipTest.SipMeasurement("TrialSoundGeneration", SpeechMaterial.ParentTestSpecification);

            //// Clearing any trials that may have been planned by a previous call
            //SiPMeasurement.ClearTrials();

            //SiPMeasurement.TestProcedure.TestParadigm = Testparadigm.Slow;



        }


    }
}
