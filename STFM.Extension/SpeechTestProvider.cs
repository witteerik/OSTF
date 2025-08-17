using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STFN.Core;
using STFM.Views;
using STFM.Extension.Views;

namespace STFM.Extension
{
    public class SpeechTestProvider : STFM.SpeechTestProvider
    {

        public override SpeechTestInitiator? GetSpeechTestInitiator(string SelectedTestName)
        {

            // Calling the base class method first, and returns it's result if not null
            var baseResult = base.GetSpeechTestInitiator(SelectedTestName);
            if (baseResult != null){ return baseResult; }

            // If the base class call did not return a SpeechTestInitiator, the current method tries to return it instead
            SpeechTestInitiator speechTestInitiator = new SpeechTestInitiator();

            switch (SelectedTestName)
            {

                case "SiP-testet (Adaptivt-BILD)":

                    // Selecting the speech material name
                    speechTestInitiator.SelectedSpeechMaterialName = "Swedish SiP-test"; // Leave as an empty string if the user should select manually

                    // Creating the speech test instance, and also stors it in SharedSpeechTestObjects
                    speechTestInitiator.SpeechTest = new STFN.AdaptiveSiP_BILD(speechTestInitiator.SelectedSpeechMaterialName);
                    SharedSpeechTestObjects.CurrentSpeechTest = speechTestInitiator.SpeechTest;

                    // Creating a test options view
                    speechTestInitiator.TestOptionsView = new OptionsViewAll(speechTestInitiator.SpeechTest);

                    // Creating a test results view
                    speechTestInitiator.TestResultsView = new TestResultView_AdaptiveSiP();

                    // Determining the GuiLayoutState
                    if (OstfBase.CurrentPlatForm == OstfBase.Platforms.WinUI & OstfBase.UseExtraWindows == true)
                    {
                        speechTestInitiator.GuiLayoutState = SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOffForm;
                        speechTestInitiator.UseExtraWindow = true;
                        switch (SharedSpeechTestObjects.GuiLanguage)
                        {
                            case STFN.Core.Utils.EnumCollection.Languages.Swedish:
                                speechTestInitiator.ExtraWindowTitle = "Testresultat";
                                break;
                            default:
                                speechTestInitiator.ExtraWindowTitle = "Test Results Window";
                                break;
                        }
                    }
                    else
                    {
                        speechTestInitiator.GuiLayoutState = SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm;
                        speechTestInitiator.UseExtraWindow = false;
                    }

                    return speechTestInitiator;

                case "Hagermans meningar (Matrix) - Användarstyrt":


                    // Selecting the speech material name
                    speechTestInitiator.SelectedSpeechMaterialName = "Swedish Matrix Test (Hagerman)"; // Leave as an empty string if the user should select manually

                    // Creating the speech test instance, and also stors it in SharedSpeechTestObjects
                    speechTestInitiator.SpeechTest = new MatrixSpeechTest(speechTestInitiator.SelectedSpeechMaterialName);

                    // Setting to free recall
                    speechTestInitiator.SpeechTest.IsFreeRecall = false;

                    SharedSpeechTestObjects.CurrentSpeechTest = speechTestInitiator.SpeechTest;

                    // Creating a test options view
                    speechTestInitiator.TestOptionsView = new OptionsViewAll(speechTestInitiator.SpeechTest);

                    // Creating a test results view
                    speechTestInitiator.TestResultsView = new TestResultView_Adaptive();

                    // Determining the GuiLayoutState
                    if (OstfBase.CurrentPlatForm == OstfBase.Platforms.WinUI & OstfBase.UseExtraWindows == true)
                    {
                        speechTestInitiator.GuiLayoutState = SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOffForm;
                        speechTestInitiator.UseExtraWindow = true;
                        switch (SharedSpeechTestObjects.GuiLanguage)
                        {
                            case STFN.Core.Utils.EnumCollection.Languages.Swedish:
                                speechTestInitiator.ExtraWindowTitle = "Testresultat";
                                break;
                            default:
                                speechTestInitiator.ExtraWindowTitle = "Test Results Window";
                                break;
                        }
                    }
                    else
                    {
                        speechTestInitiator.GuiLayoutState = SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm;
                        speechTestInitiator.UseExtraWindow = false;
                    }

                    return speechTestInitiator;

                case "SiP-testet (TSFC)":

                    // Selecting the speech material name
                    speechTestInitiator.SelectedSpeechMaterialName = "Swedish SiP-test"; // Leave as an empty string if the user should select manually

                    // Creating the speech test instance, and also stors it in SharedSpeechTestObjects
                    speechTestInitiator.SpeechTest = new AdaptiveSiP(speechTestInitiator.SelectedSpeechMaterialName);
                    SharedSpeechTestObjects.CurrentSpeechTest = speechTestInitiator.SpeechTest;

                    // Creating a test options view
                    speechTestInitiator.TestOptionsView = new OptionsViewAll(speechTestInitiator.SpeechTest);

                    // Creating a test results view
                    speechTestInitiator.TestResultsView = new TestResultView_AdaptiveSiP();

                    // Determining the GuiLayoutState
                    if (OstfBase.CurrentPlatForm == OstfBase.Platforms.WinUI & OstfBase.UseExtraWindows == true)
                    {
                        speechTestInitiator.GuiLayoutState = SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOffForm;
                        speechTestInitiator.UseExtraWindow = true;
                        switch (SharedSpeechTestObjects.GuiLanguage)
                        {
                            case STFN.Core.Utils.EnumCollection.Languages.Swedish:
                                speechTestInitiator.ExtraWindowTitle = "Testresultat";
                                break;
                            default:
                                speechTestInitiator.ExtraWindowTitle = "Test Results Window";
                                break;
                        }
                    }
                    else
                    {
                        speechTestInitiator.GuiLayoutState = SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm;
                        speechTestInitiator.UseExtraWindow = false;
                    }

                    return speechTestInitiator;


                case "Talaudiometri":

                    // Only setting the GuiLayoutState to allow the user to pick speech material
                    speechTestInitiator.GuiLayoutState = SpeechTestView.GuiLayoutStates.SpeechMaterialSelection;

                    return speechTestInitiator;



                default:
                    return null;
            }
        }

        public override ResponseView? GetSpeechTestResponseView(string SelectedTestName, SpeechTest CurrentSpeechTest, double InitialWidth, double InitialHeight)
        {

            // Calling the base class method first, and returns it's result if not null
            var baseResult = base.GetSpeechTestResponseView(SelectedTestName, CurrentSpeechTest, InitialWidth, InitialHeight);
            if (baseResult != null) { return baseResult; }

            // If the base class call did not return a ResponseView, the current method tries to return one instead
            switch (SelectedTestName)
            {

                case "SiP-testet (Adaptivt-BILD)":

                    return new ResponseView_AdaptiveSiP();

                case "Talaudiometri":

                    // Pick appropriate response view
                    if (CurrentSpeechTest.IsFreeRecall)
                    {
                        return new ResponseView_FreeRecall();
                    }
                    else
                    {
                        return new ResponseView_Mafc();

                        // We have to choose between:
                        //CurrentResponseView = new ResponseView_Matrix();

                        //CurrentResponseView = new ResponseView_FreeRecallWithHistory(TestReponseGrid.Width, TestReponseGrid.Height, CurrentSpeechTest.HistoricTrialCount);
                        //Which also requires:
                        // CurrentResponseView.ResponseHistoryUpdated += ResponseHistoryUpdate;

                        //CurrentResponseView = new ResponseView_SiP_SF();

                    }

                case "Hagermans meningar (Matrix) - Användarstyrt":

                    // Response view
                    if (CurrentSpeechTest.IsFreeRecall)
                    {
                        return new ResponseView_FreeRecall();
                    }
                    else
                    {
                        return new ResponseView_Matrix();
                    }


                case "SiP-testet (TSFC)":

                    return new ResponseView_TSFC();

                default:
                    return null;
            }

        }


    }
}
