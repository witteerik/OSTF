using STFM.Views;
using STFN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STFM
{
    public class SpeechTestProvider
    {


        public SpeechTestInitiator GetSpeechTestInitiator(string SelectedTestName)
        {

            SpeechTestInitiator speechTestInitiator = new SpeechTestInitiator();

            switch (SelectedTestName)
            {
                case "Svenska HINT":

                    // Selecting the speech material name
                    speechTestInitiator.SelectedSpeechMaterialName = "Swedish HINT"; // Leave as an empty string if the user should select manually

                    // Creating the speech test instance, and also stors it in SharedSpeechTestObjects
                    speechTestInitiator.SpeechTest = new HintSpeechTest(speechTestInitiator.SelectedSpeechMaterialName);
                    STFN.SharedSpeechTestObjects.CurrentSpeechTest = speechTestInitiator.SpeechTest;

                    // Creating a test options view
                    speechTestInitiator.TestOptionsView = new OptionsViewAll(speechTestInitiator.SpeechTest);

                    // Creating a test results view
                    speechTestInitiator.TestResultsView = new TestResultView_Adaptive();

                    // Determining the GuiLayoutState
                    speechTestInitiator.GuiLayoutState =  SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm;
                    speechTestInitiator.UseExtraWindow = false;

                    return speechTestInitiator;

                case "Hagermans meningar (Matrix)":


                    // Selecting the speech material name
                    speechTestInitiator.SelectedSpeechMaterialName = "Swedish Matrix Test (Hagerman)"; // Leave as an empty string if the user should select manually

                    // Creating the speech test instance, and also stors it in SharedSpeechTestObjects
                    speechTestInitiator.SpeechTest = new MatrixSpeechTest(speechTestInitiator.SelectedSpeechMaterialName);
                    STFN.SharedSpeechTestObjects.CurrentSpeechTest = speechTestInitiator.SpeechTest;

                    // Creating a test options view
                    speechTestInitiator.TestOptionsView = new OptionsViewAll(speechTestInitiator.SpeechTest);

                    // Creating a test results view
                    speechTestInitiator.TestResultsView = new TestResultView_Adaptive();

                    // Determining the GuiLayoutState
                    if (OstfBase.CurrentPlatForm == OstfBase.Platforms.WinUI & OstfBase.UseExtraWindows == true)
                    {
                        speechTestInitiator.GuiLayoutState = SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOffForm;
                        speechTestInitiator.UseExtraWindow = true;
                        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
                        {
                            case STFN.Utils.Constants.Languages.Swedish:
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


                case "Hörtröskel för tal (HTT)":

                    // Selecting the speech material name
                    speechTestInitiator.SelectedSpeechMaterialName = "Swedish Spondees 23"; // Leave as an empty string if the user should select manually

                    // Creating the speech test instance, and also stors it in SharedSpeechTestObjects
                    speechTestInitiator.SpeechTest = new HTT23SpeechTest(speechTestInitiator.SelectedSpeechMaterialName);
                    STFN.SharedSpeechTestObjects.CurrentSpeechTest = speechTestInitiator.SpeechTest;

                    // Creating a test options view
                    speechTestInitiator.TestOptionsView = new OptionsViewAll(speechTestInitiator.SpeechTest);

                    // Creating a test results view
                    speechTestInitiator.TestResultsView = new TestResultView_Adaptive();

                    // Determining the GuiLayoutState
                    speechTestInitiator.GuiLayoutState = SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm;
                    speechTestInitiator.UseExtraWindow = false;

                    return speechTestInitiator;


                case "Manuell TP i brus":

                    // Selecting the speech material name
                    speechTestInitiator.SelectedSpeechMaterialName = "SwedishTP50"; // Leave as an empty string if the user should select manually

                    // Creating the speech test instance, and also stors it in SharedSpeechTestObjects
                    speechTestInitiator.SpeechTest = new TP50SpeechTest(speechTestInitiator.SelectedSpeechMaterialName);
                    STFN.SharedSpeechTestObjects.CurrentSpeechTest = speechTestInitiator.SpeechTest;

                    // Creating a test options view
                    speechTestInitiator.TestOptionsView = new OptionsViewAll(speechTestInitiator.SpeechTest);

                    // Creating a test results view
                    speechTestInitiator.TestResultsView = new TestResultView_ConstantStimuli();

                    // Determining the GuiLayoutState
                    speechTestInitiator.GuiLayoutState = SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOnForm;
                    speechTestInitiator.UseExtraWindow = false;

                    return speechTestInitiator;

                case "Quick SiP":

                    // Selecting the speech material name
                    speechTestInitiator.SelectedSpeechMaterialName = "Swedish SiP-test"; // Leave as an empty string if the user should select manually

                    // Creating the speech test instance, and also stors it in SharedSpeechTestObjects
                    speechTestInitiator.SpeechTest = new QuickSiP(speechTestInitiator.SelectedSpeechMaterialName);
                    STFN.SharedSpeechTestObjects.CurrentSpeechTest = speechTestInitiator.SpeechTest;

                    // Creating a test options view
                    speechTestInitiator.TestOptionsView = new OptionsViewAll(speechTestInitiator.SpeechTest);

                    // Creating a test results view
                    speechTestInitiator.TestResultsView = new TestResultView_QuickSiP();

                    // Determining the GuiLayoutState
                    if (OstfBase.CurrentPlatForm == OstfBase.Platforms.WinUI & OstfBase.UseExtraWindows == true)
                    {
                        speechTestInitiator.GuiLayoutState = SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOffForm;
                        speechTestInitiator.UseExtraWindow = true;
                        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
                        {
                            case STFN.Utils.Constants.Languages.Swedish:
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

                case "SiP-testet (Adaptivt)":

                    // Selecting the speech material name
                    speechTestInitiator.SelectedSpeechMaterialName = "Swedish SiP-test"; // Leave as an empty string if the user should select manually

                    // Creating the speech test instance, and also stors it in SharedSpeechTestObjects
                    speechTestInitiator.SpeechTest = new AdaptiveSiP(speechTestInitiator.SelectedSpeechMaterialName);
                    STFN.SharedSpeechTestObjects.CurrentSpeechTest = speechTestInitiator.SpeechTest;

                    // Creating a test options view
                    speechTestInitiator.TestOptionsView = new OptionsViewAll(speechTestInitiator.SpeechTest);

                    // Creating a test results view
                    speechTestInitiator.TestResultsView = new TestResultView_AdaptiveSiP();

                    // Determining the GuiLayoutState
                    if (OstfBase.CurrentPlatForm == OstfBase.Platforms.WinUI & OstfBase.UseExtraWindows == true)
                    {
                        speechTestInitiator.GuiLayoutState = SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOffForm;
                        speechTestInitiator.UseExtraWindow = true;
                        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
                        {
                            case STFN.Utils.Constants.Languages.Swedish:
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


                case "SiP-testet (Adaptivt) - Övning":

                    // Selecting the speech material name
                    speechTestInitiator.SelectedSpeechMaterialName = "Swedish SiP-test"; // Leave as an empty string if the user should select manually

                    // Creating the speech test instance, and also stors it in SharedSpeechTestObjects
                    speechTestInitiator.SpeechTest = new AdaptiveSiP(speechTestInitiator.SelectedSpeechMaterialName);
                    speechTestInitiator.SpeechTest.IsPractiseTest = true;
                    STFN.SharedSpeechTestObjects.CurrentSpeechTest = speechTestInitiator.SpeechTest;

                    // Creating a test options view
                    speechTestInitiator.TestOptionsView = new OptionsViewAll(speechTestInitiator.SpeechTest);

                    // Creating a test results view
                    speechTestInitiator.TestResultsView = new TestResultView_AdaptiveSiP();

                    // Determining the GuiLayoutState
                    if (OstfBase.CurrentPlatForm == OstfBase.Platforms.WinUI & OstfBase.UseExtraWindows == true)
                    {
                        speechTestInitiator.GuiLayoutState = SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOffForm;
                        speechTestInitiator.UseExtraWindow = true;
                        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
                        {
                            case STFN.Utils.Constants.Languages.Swedish:
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
                    STFN.SharedSpeechTestObjects.CurrentSpeechTest = speechTestInitiator.SpeechTest;

                    // Creating a test options view
                    speechTestInitiator.TestOptionsView = new OptionsViewAll(speechTestInitiator.SpeechTest);

                    // Creating a test results view
                    speechTestInitiator.TestResultsView = new TestResultView_AdaptiveSiP();

                    // Determining the GuiLayoutState
                    if (OstfBase.CurrentPlatForm == OstfBase.Platforms.WinUI & OstfBase.UseExtraWindows == true)
                    {
                        speechTestInitiator.GuiLayoutState = SpeechTestView.GuiLayoutStates.TestOptions_StartButton_TestResultsOffForm;
                        speechTestInitiator.UseExtraWindow = true;
                        switch (STFN.SharedSpeechTestObjects.GuiLanguage)
                        {
                            case STFN.Utils.Constants.Languages.Swedish:
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


        public class SpeechTestInitiator
        {
            //public string Name { get; set; }

            public string SelectedSpeechMaterialName { get; set; } = "";

            public STFN.SpeechTest SpeechTest { get; set; }

            public OptionsViewAll TestOptionsView {  get; set; }

            public TestResultsView TestResultsView { get; set; }

            public Views.SpeechTestView.GuiLayoutStates GuiLayoutState { get; set; }

            public bool UseExtraWindow { get; set; } = false;

            public string ExtraWindowTitle { get; set; } = "";

            public SpeechTestInitiator() { }


        }

    }

}
