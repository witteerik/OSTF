using STFN.Core;

namespace STFM.Views;

public abstract class TestResultsView : ContentView
{

    public event EventHandler StartedFromTestResultView;

    protected virtual void OnStartedFromTestResultView(EventArgs e)
    {
        EventHandler handler = StartedFromTestResultView;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    public event EventHandler PausedFromTestResultView;

    protected virtual void OnPausedFromTestResultView(EventArgs e)
    {
        EventHandler handler = PausedFromTestResultView;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    public event EventHandler StoppedFromTestResultView;

    protected virtual void OnStoppedFromTestResultView(EventArgs e)
    {

        EventHandler handler = StoppedFromTestResultView;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    public TestResultsView()
	{

	}

    public abstract void UpdateStartButtonText(string text);

    public abstract void ShowTestResults(string results);

    public abstract void ShowTestResults(SpeechTest speechTest);

    public abstract void SetGuiLayoutState(SpeechTestView.GuiLayoutStates currentTestPlayState);

    public async void TakeScreenShot()
    {

        if (SharedSpeechTestObjects.CurrentSpeechTest != null)
        {
            // Taking screen shot
            string savePath = SharedSpeechTestObjects.CurrentSpeechTest.GetTestResultScreenDumpExportPath();

            await ScreenShooter.TakeScreenshotAndSaveAsync(savePath);

            // Sleeping 200 to prevent focus shifting to other windows during save
            Thread.Sleep(200);

            switch (SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Core.Utils.EnumCollection.Languages.Swedish:
                    Messager.MsgBox("Resultaten sparades till:\n\n " + savePath, Messager.MsgBoxStyle.Information, "Resultaten sparades!");

                    break;
                default:

                    Messager.MsgBox("The test results were saved to;\n\n " + savePath, Messager.MsgBoxStyle.Information, "The results were saved!");
                    break;
            }
        }
    }
}