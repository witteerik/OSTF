
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

                // Save the screenshot to a file
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
        }
        catch (Exception)
        {
            // Ignoring any error here
            //throw;
        }


    }

}
