using STFN.Core;

public static class ScreenShooter
{

    public static async Task<ImageSource> TakeScreenshotAsync()
    {

        if (Screenshot.Default.IsCaptureSupported)
        {
             IScreenshotResult screen = await Screenshot.Default.CaptureAsync();

            Stream stream = await screen.OpenReadAsync();

            return ImageSource.FromStream(() => stream);
        }

        return null;
    }

    public static async Task TakeScreenshotAndSaveAsync(string filePath)
    {
        try
        {
            if (Screenshot.Default.IsCaptureSupported)
            {
                IScreenshotResult screen = await Screenshot.Default.CaptureAsync();
                Stream stream = await screen.OpenReadAsync();

                // Creating the directory if it doesn't exist
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));

                // Save the screenshot to a file
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
            else
            {
                switch (SharedSpeechTestObjects.GuiLanguage)
                {
                    case STFN.Core.Utils.Constants.Languages.Swedish:
                        await Messager.MsgBoxAsync("Skärmdumpar stöds inte på denna enhet!", Messager.MsgBoxStyle.Information, "Kunde inte ta skärmdump!");
                        break;
                    default:
                        await Messager.MsgBoxAsync("Screen shots are not supported on this device", Messager.MsgBoxStyle.Information, "Could not take a screen shot!");
                        break;
                }
            }
        }
        catch (Exception e)
        {

            switch (SharedSpeechTestObjects.GuiLanguage)
            {
                case STFN.Core.Utils.Constants.Languages.Swedish:
                    await Messager.MsgBoxAsync("Misslyckades med att ta skärmdump!\n\n" + e.ToString(),  Messager.MsgBoxStyle.Information, "Något gick fel!");
                    break;
                default:
                    await Messager.MsgBoxAsync("Failed to take a screen shot!\n\n" + e.ToString(), Messager.MsgBoxStyle.Information, "Something went wrong!");
                    break;
            }

        }


    }

}
