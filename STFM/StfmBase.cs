using CommunityToolkit.Maui.Storage;
using STFN;
using System.IO.Compression;
using static STFN.Utils.AppCache;


#if ANDROID
using Android.OS;
using Android.App;
using Android.Content;
using Android.Provider;
#endif


namespace STFM
{

    public static class StfmBase
    {

        public static bool IsInitialized = false;

        public static async Task InitializeSTFM(STFN.OstfBase.MediaPlayerTypes MediaPlayerType = OstfBase.MediaPlayerTypes.Default, bool RequestExternalStoragePermission = true)
        {

            // Returning if already called
            if (IsInitialized == true)
            {
                return;
            }
            IsInitialized = true;

            //Setting up app cache callbacks
            STFN.Utils.AppCache.OnAppCacheVariableExists += AppCacheVariableExists;
            STFN.Utils.AppCache.OnSetAppCacheStringVariableValue += SetAppCacheStringVariableValue;
            STFN.Utils.AppCache.OnSetAppCacheIntegerVariableValue += SetAppCacheIntegerVariableValue;
            STFN.Utils.AppCache.OnSetAppCacheDoubleVariableValue += SetAppCacheDoubleVariableValue;
            STFN.Utils.AppCache.OnGetAppCacheStringVariableValue += GetAppCacheStringVariableValue;
            STFN.Utils.AppCache.OnGetAppCacheDoubleVariableValue += GetAppCacheDoubleVariableValue;
            STFN.Utils.AppCache.OnGetAppCacheIntegerVariableValue += GetAppCacheIntegerVariableValue;
            STFN.Utils.AppCache.OnRemoveAppCacheVariable += RemoveAppCacheVariable;
            STFN.Utils.AppCache.OnClearAppCache += ClearAppCache;


            await CheckAndSetOstfLogRootFolder(RequestExternalStoragePermission);

            await SetupMediaFromPackage();

            await CheckAndSetMediaRootDirectory(RequestExternalStoragePermission);

            await CheckAndSetTestResultsRootFolder(RequestExternalStoragePermission);


            // Initializing OSTF
            OstfBase.InitializeOSTF(GetCurrentPlatform(), MediaPlayerType, OstfBase.MediaRootDirectory);

            if (OstfBase.CurrentMediaPlayerType == OstfBase.MediaPlayerTypes.AudioTrackBased)
            {
#if ANDROID

                // We now need to check that the requested devices exist, which could not be done in STFN, since the Android AudioTrack do not exist there.
                bool AudioTrackBasedPlayerInitResult = await AndroidAudioTrackPlayer.InitializeAudioTrackBasedPlayer();

                if (AudioTrackBasedPlayerInitResult == false)
                {
                    Messager.RequestCloseApp();
                }

#endif
            }
        }


        static OstfBase.Platforms GetCurrentPlatform()
        {

            if (DeviceInfo.Current.Platform == DevicePlatform.iOS) { return OstfBase.Platforms.iOS; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.WinUI) { return OstfBase.Platforms.WinUI; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Tizen) { return OstfBase.Platforms.Tizen; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.tvOS) { return OstfBase.Platforms.tvOS; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst) { return OstfBase.Platforms.MacCatalyst; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.macOS) { return OstfBase.Platforms.macOS; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.watchOS) { return OstfBase.Platforms.watchOS; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Unknown) { return OstfBase.Platforms.Unknown; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Android) { return OstfBase.Platforms.Android; }
            else { throw new Exception("Failed to resolve the current platform type."); }

        }


        async static Task CheckAndSetMediaRootDirectory(bool RequestExternalStoragePermission)
        {
            // Trying to read the MediaRootDirectory from pevious app sessions
            OstfBase.MediaRootDirectory = ReadMediaRootDirectory();

            string previouslyStoredMediaRootDirectory = OstfBase.MediaRootDirectory;

            bool askForMediaFolder = true;

            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                throw new NotImplementedException("Media folder location is not yet implemented for iOS");
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                // Setting default folder name
                if (OstfBase.MediaRootDirectory == "")
                {
                    //var b = System.IO.Directory.Exists("/usr/share");
                    //OstfBase.MediaRootDirectory = "/storage/emulated/0/OstfMedia";
                }

                // Checking for permissions
                await CheckPermissions(RequestExternalStoragePermission);
            }

            // Checking if it seems to be the correct folder
            try
            {
                if (System.IO.Directory.Exists(OstfBase.MediaRootDirectory))
                {
                    var fse = System.IO.Directory.GetFileSystemEntries(OstfBase.MediaRootDirectory);
                    for (int i = 0; i < fse.Length; i++)
                    {
                        if (fse[i].EndsWith("AvailableSpeechMaterials"))
                        {
                            askForMediaFolder = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //askForTestResultsRootFolder = true;
                //throw;
            }

            if (OstfBase.MediaRootDirectory == "")
            {
                askForMediaFolder = true;
            }

            //askForTestResultsRootFolder = true;
            if (askForMediaFolder)
            {
                await PickMediaFolder();
            }

            if (previouslyStoredMediaRootDirectory != OstfBase.MediaRootDirectory)
            {
                // Storing the MediaRootDirectory for future instances of the app, but only if it was changed
                StoreMediaRootDirectory(OstfBase.MediaRootDirectory);
            }
        }




        async static Task CheckAndSetTestResultsRootFolder(bool RequestExternalStoragePermission)
        {
            // Trying to read the TestResultsRootFolder from pevious app sessions
            SharedSpeechTestObjects.TestResultsRootFolder = ReadTestResultRootDirectory();

            string previouslyStoredTestResultsRootFolder = SharedSpeechTestObjects.TestResultsRootFolder;

            bool askForTestResultsRootFolder = true;

            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                throw new NotImplementedException("Test results root folder location is not yet implemented for iOS");
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                // Checking for permissions
                await CheckPermissions(RequestExternalStoragePermission);
            }

            // Checking if the folder exists
            try
            {
                if (System.IO.Directory.Exists(SharedSpeechTestObjects.TestResultsRootFolder))
                {
                    askForTestResultsRootFolder = false;
                }
            }
            catch (Exception)
            {
                askForTestResultsRootFolder = true;
            }

            if (SharedSpeechTestObjects.TestResultsRootFolder == "")
            {
                askForTestResultsRootFolder = true;
            }

            if (askForTestResultsRootFolder)
            {
                await PickTestResultsRootFolder();
            }

            if (previouslyStoredTestResultsRootFolder != SharedSpeechTestObjects.TestResultsRootFolder)
            {
                // Storing the TestResultsRootFolder for future instances of the app, but only if it was changed
                StoreTestResultRootDirectory(SharedSpeechTestObjects.TestResultsRootFolder);
            }
        }

        async static Task SetupMediaFromPackage()
        {
            // This method should:
            // - Check if MediaRootDirectory exists,
            //  - If already set, just exit
            //  - If not set, ask the user if he/she wants to setup a the media package using a prefabricated zip file
            //   - If not, just exit, and the user will have to supply the MediaRootDirectory later and manually place the needed files there
            //   - If so:
            //    - Ask the user for the zip file to use
            //    - Unpack the zip file and store the unzipped files in a designated location
            //          - On Windows: C:\OSTF\OSTFMedia
            //          - On Android: The private storage space of the app (for which no permissions are needed, but cannot be reached from outside the app)


            // Returns if a MediaRootDirectory is set and exists (assuming that it has already been properly setup)
            if (ReadMediaRootDirectory() != "")
            {
                if (System.IO.Directory.Exists(ReadMediaRootDirectory()))
                {
                    return;
                }
            }


            // Asks the user if setup should be made with a zipped media file, and returns if not.
            bool demoModeQuestionResult = await Messager.MsgBoxAcceptQuestion("No MediaRootDirectory has yet been set. At this stage you may setup the contents of the MediaRootDirectory from a zipped media package file. \n\n Do you want to setup the content with a zip file?", "MediaRootDirectory content setup", "Yes, use zip file", "No, setup manually");
            if (demoModeQuestionResult == false)
            {
                return;
            }

            // Asks the user for a zip file path
            string mediaPackageFile = await PickMediaPackageFile();
            if (mediaPackageFile == "")
            {
                await Messager.MsgBoxAsync("No media package file was selected! Restart the application or continue to setup media files manually.", Messager.MsgBoxStyle.Information, "Warning!");
                return;
            }

            // Getting an OS specific file path to where the unzipped media files should be stored
            string TargetDataFolder = "";

            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                throw new NotImplementedException("Using zipped media file is not yet implemented for iOS");
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                // As we us the apps own storage spave here, we do not need to request user permission.
                TargetDataFolder = Path.Combine(FileSystem.AppDataDirectory, "Media");

                // Checking for permissions
                // await CheckPermissions();
                //#if ANDROID
                //                // Get the real public Documents folder on Android
                //                TargetDataFolder = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "Documents");
                //#endif

            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                TargetDataFolder = Path.Combine("C:", "OSTF", STFN.OstfBase.DefaultMediaRootFolderName);
            }

            // Closing the app if no TargetDataFolder could be determined
            if (TargetDataFolder == "")
            {
                await Messager.MsgBoxAsync("No location to store the media package could be determined. Your system may be unsupported? Closing the application.", Messager.MsgBoxStyle.Information, "Warning!");
                Messager.RequestCloseApp();
                return;
            }

            // Setting up the OSTF media folder structure and store it's files in the intended location of the device
            // Plus, on Windows, making sure to prevent overwriting an existing folder,


            // Ensuring that they do not already exist (only on Windows, as on other devices this folder will likely be unavailable anyway)
            // Actually, in most cases, this is redundant as the method returns above if a MediaRootDirectory exists above. If the TargetDataFolder should differ from the stored MediaRootDirectory, however, the check may be relevant. Leaving it therefore in place.
            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                if (System.IO.Directory.Exists(TargetDataFolder))
                {
                    await Messager.MsgBoxAsync("A media root folder already exists at " + TargetDataFolder + ". To prevent the loss of data, you should manually delete this folder and its contents and then try again. Closing the application.");
                    Messager.RequestCloseApp();
                }
            }

            // Unzipping the zip file and copying its contents to the TargetDataFolder
            await Messager.MsgBoxAsync("Unzipping the media files may take several minutes. You will be notified when unzipping is complete. Press OK to start!");
            await Task.Delay(50); // allowing time to update the GUI
            try
            {
                ZipFile.ExtractToDirectory(mediaPackageFile, TargetDataFolder, true);
            }
            catch (Exception ex)
            {
                await Messager.MsgBoxAsync("The following error occurred when unzipping the media package files from the file " + mediaPackageFile + " to the folder " + TargetDataFolder + "\n\n" + ex.ToString() + ". Closing the application.", Messager.MsgBoxStyle.Information, "Error!");
                Messager.RequestCloseApp();
                return;
            }

            await Messager.MsgBoxAsync("Finished unzipping media files. Press OK to continue!");

            // Verifying that the new folders exist
            if (System.IO.Directory.Exists(TargetDataFolder) == false)
            {

                if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
                {
                    await Messager.MsgBoxAsync("Something went wrong when unzipping the media files. Please manually remove the folder " + TargetDataFolder + " and try again. Closing the application.");
                }
                else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
                {
                    await Messager.MsgBoxAsync("Something went wrong when unzipping the media files. Try to reset the app data and/or reinstall the app. You may have to factory reset the device first. Closing the application.");
                }
                else if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
                {
                    throw new NotImplementedException("Using zipped media file is not yet implemented for iOS");
                }

                Messager.RequestCloseApp();

            }

            // Finally storing the MediaRootDirectory in apps memory
            OstfBase.MediaRootDirectory = TargetDataFolder;
            StoreMediaRootDirectory(OstfBase.MediaRootDirectory);

        }


        async static Task CheckAndSetOstfLogRootFolder(bool RequestExternalStoragePermission)
        {

            // Trying to read the logFilePath from pevious app sessions
            STFN.Utils.Logging.logFilePath = ReadOstfLogRootDirectory();

            string previouslyStoredOstfLogRootFolder = STFN.Utils.Logging.logFilePath;

            bool askForOstfLogRootFolder = true;

            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                throw new NotImplementedException("Setting OSTF log root folder location is not yet implemented for iOS");
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                // Checking for permissions
                await CheckPermissions(RequestExternalStoragePermission);
            }

            // Checking if the folder exists
            try
            {
                if (System.IO.Directory.Exists(STFN.Utils.Logging.logFilePath))
                {
                    askForOstfLogRootFolder = false;
                }
            }
            catch (Exception)
            {
                askForOstfLogRootFolder = true;
            }

            if (STFN.Utils.Logging.logFilePath == "")
            {
                askForOstfLogRootFolder = true;
            }

            if (askForOstfLogRootFolder)
            {
                await PickOstfLogRootFolder();
            }

            if (previouslyStoredOstfLogRootFolder != STFN.Utils.Logging.logFilePath)
            {
                // Storing the logFilePath for future instances of the app, but only if it was changed
                StoreOstfLogRootDirectory(STFN.Utils.Logging.logFilePath);
            }
        }


        async static Task<string> PickMediaPackageFile()
        {

            var pickOptions = new PickOptions
            {
                PickerTitle = "Select the media package (.zip) file to use",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/zip" } },
                    { DevicePlatform.iOS, new[] { "com.pkware.zip-archive" } },
                    { DevicePlatform.WinUI, new[] { ".zip" } }
                })
            };

            var result = await FilePicker.PickAsync(pickOptions);

            if (result == null)
            {
                return "";
            }
            else
            {
                return result.FullPath;
            }

        }


        async static Task PickMediaFolder()
        {
            await Messager.MsgBoxAsync("The location of the media folder (MediaRootDirectory) has not yet been set. Please click OK and indicate where the OSTFMedia folder is located on your device using the dialog that appears.", Messager.MsgBoxStyle.Information, "Set media folder");
            var result = await FolderPicker.PickAsync(CancellationToken.None);
            //var result = await FolderPicker.Default.PickAsync(CancellationToken.None);
            if (result.IsSuccessful)
            {
                //await Toast.Make($"The folder was picked: Name - {result.Folder.Name}, Path - {result.Folder.Path}", ToastDuration.Long).Show(CancellationToken.None);
                OstfBase.MediaRootDirectory = result.Folder.Path;
                await Messager.MsgBoxAsync("You have picked the following MediaRootDirectory folder location: " + OstfBase.MediaRootDirectory);
            }
            else
            {
                await Messager.MsgBoxAsync("Unable to selected the picked folder (" + OstfBase.MediaRootDirectory + ") shutting down the application.");
                Messager.RequestCloseApp();
                //await Toast.Make($"The folder was not picked with error: {result.Exception.Message}").Show(CancellationToken.None);
            }
        }


        async static Task PickTestResultsRootFolder()
        {
            await Messager.MsgBoxAsync("No test results folder (TestResultsRootFolder) has yet been set. Please click OK and select a test results folder in the dialog that appears.", Messager.MsgBoxStyle.Information, "Set test results folder");
            var result = await FolderPicker.PickAsync(CancellationToken.None);
            if (result.IsSuccessful)
            {
                //await Toast.Make($"The folder was picked: Name - {result.Folder.Name}, Path - {result.Folder.Path}", ToastDuration.Long).Show(CancellationToken.None);
                SharedSpeechTestObjects.TestResultsRootFolder = result.Folder.Path;
                await Messager.MsgBoxAsync("You have picked the following test results folder: " + SharedSpeechTestObjects.TestResultsRootFolder);

            }
            else
            {
                await Messager.MsgBoxAsync("Unable to selected the picked folder (" + SharedSpeechTestObjects.TestResultsRootFolder + ") shutting down the application.");
                Messager.RequestCloseApp();
                //await Toast.Make($"The folder was not picked with error: {result.Exception.Message}").Show(CancellationToken.None);
            }
        }

        async static Task PickOstfLogRootFolder()
        {
            await Messager.MsgBoxAsync("No application log folder has yet been set. Please click OK and select a log folder in the dialog that appears.", Messager.MsgBoxStyle.Information, "Set application log folder");
            var result = await FolderPicker.PickAsync(CancellationToken.None);
            if (result.IsSuccessful)
            {
                //await Toast.Make($"The folder was picked: Name - {result.Folder.Name}, Path - {result.Folder.Path}", ToastDuration.Long).Show(CancellationToken.None);
                STFN.Utils.Logging.logFilePath = result.Folder.Path;
                await Messager.MsgBoxAsync("You have picked the following log folder: " + STFN.Utils.Logging.logFilePath);

            }
            else
            {
                await Messager.MsgBoxAsync("Unable to selected the picked folder (" + STFN.Utils.Logging.logFilePath + ") shutting down the application.");
                Messager.RequestCloseApp();
                //await Toast.Make($"The folder was not picked with error: {result.Exception.Message}").Show(CancellationToken.None);
            }
        }

        static string ReadMediaRootDirectory()
        {
            if (Preferences.Default.ContainsKey("media_root_directory"))
            {
                return Preferences.Default.Get("media_root_directory", "");
            }
            else
            {
                return "";
            }
        }

        public static void StoreMediaRootDirectory(string mediaRootDirectory)
        {
            Preferences.Default.Set("media_root_directory", mediaRootDirectory);
        }

        static void ClearMediaRootDirectoryFromPreferences(string mediaRootDirectory)
        {
            Preferences.Default.Remove("media_root_directory");
        }

        static string ReadTestResultRootDirectory()
        {
            if (Preferences.Default.ContainsKey("test_result_root_directory"))
            {
                return Preferences.Default.Get("test_result_root_directory", "");
            }
            else
            {
                return "";
            }
        }

        public static void StoreTestResultRootDirectory(string TestResultRootDirectory)
        {
            Preferences.Default.Set("test_result_root_directory", TestResultRootDirectory);
        }

        static void ClearTestResultRootDirectoryFromPreferences()
        {
            Preferences.Default.Remove("test_result_root_directory");
        }



        static string ReadOstfLogRootDirectory()
        {
            if (Preferences.Default.ContainsKey("ostf_log_root_directory"))
            {
                return Preferences.Default.Get("ostf_log_root_directory", "");
            }
            else
            {
                return "";
            }
        }

        public static void StoreOstfLogRootDirectory(string OstfLogRootDirectory)
        {
            Preferences.Default.Set("ostf_log_root_directory", OstfLogRootDirectory);
        }

        static void ClearOstfLogRootDirectoryFromPreferences()
        {
            Preferences.Default.Remove("ostf_log_root_directory");
        }


        /// <summary>
        /// Determines if the OSTF directories needed for the app to run has all been stored in the apps memory.
        /// </summary>
        /// <returns>True if all directories are stored and false in at least one is missing.</returns>
        public static bool AllDirectoriesStored()
        {
            if (ReadOstfLogRootDirectory() == "") { return false; }
            if (ReadMediaRootDirectory() == "") { return false; }
            if (ReadTestResultRootDirectory() == "") { return false; }
            return true;
        }


        async static Task CheckPermissions(bool RequestExternalStoragePermission)
        {

            bool hasMicrophonePermission = await Permissions.CheckStatusAsync<Permissions.Microphone>() == PermissionStatus.Granted;

            if (hasMicrophonePermission == false)
            {
                var status = await Permissions.RequestAsync<Permissions.Microphone>();
                if (status != PermissionStatus.Granted)
                {
                    await Messager.MsgBoxAsync("You chose to not allow microphone use. The application does not work without it. The application will now shut down. Please restart it and try again.", Messager.MsgBoxStyle.Information, "A needed permission was denied");
                    Messager.RequestCloseApp();
                }
            }

            if (RequestExternalStoragePermission)
            {
                bool hasStorageReadPermission = await Permissions.CheckStatusAsync<Permissions.StorageRead>() == PermissionStatus.Granted;
                bool hasStorageWritePermission = await Permissions.CheckStatusAsync<Permissions.StorageWrite>() == PermissionStatus.Granted;

                if (hasStorageReadPermission == false)
                {
                    var status = await Permissions.RequestAsync<Permissions.StorageRead>();
                    if (status != PermissionStatus.Granted)
                    {
                        await Messager.MsgBoxAsync("You chose to not allow reading from storage. The application does not work without it. The application will now shut down. Please restart it and try again.", Messager.MsgBoxStyle.Information, "A needed permission was denied");
                        Messager.RequestCloseApp();
                    }
                }

                if (hasStorageWritePermission == false)
                {
                    var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                    if (status != PermissionStatus.Granted)
                    {
                        await Messager.MsgBoxAsync("You chose to not allow writing to storage. The application does not work without it. The application will now shut down. Please restart it and try again.", Messager.MsgBoxStyle.Information, "A needed permission was denied");
                        Messager.RequestCloseApp();
                    }
                }

#if ANDROID

            // Temporarily leaves kiosk mode to allow the system dialog to appear
            var context = Platform.CurrentActivity ?? Android.App.Application.Context;
            var dpm = (Android.App.Admin.DevicePolicyManager)context.GetSystemService(Android.Content.Context.DevicePolicyService);
            bool wasInKioskMode = false;
            Activity activity = context as Activity;
            if (dpm.IsDeviceOwnerApp(context.PackageName) && activity != null)
            {
                // Temporarily allow user to leave kiosk mode
                activity.StopLockTask(); // unlocks temporarily
                wasInKioskMode = true;
            }

            await RequestAllFilesAccessPermission();

            // Checking if the user switched to allow all files (on android)
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {

                bool hasAllFilesAccess = await WaitForAllFilesAccessAsync(30); // wait up to 30 seconds

                if (!hasAllFilesAccess)
                {
                    await Messager.MsgBoxAsync("The app does not have permission to manage all files. You must allow access to manage all files in the system settings for the app to function correctly. The application will now shut down. Please restart it and try again.", Messager.MsgBoxStyle.Information, "A needed permission was denied");
                    Messager.RequestCloseApp();
                }

            }

            // Re-enter kiosk mode
            if (wasInKioskMode && activity != null)
            {
                activity.StartLockTask();
            }

#endif

            }

        }

        public async static Task RequestAllFilesAccessPermission()
        {

#if ANDROID

            if (Build.VERSION.SdkInt >= BuildVersionCodes.R && !Android.OS.Environment.IsExternalStorageManager)
            {
                // Notifying the user of coming manual approval of "all files"
                await Messager.MsgBoxAsync("This app need to have permission to manage all files to function correctly. Press OK to open a system settings where you can 'Allow access to manage all files'.", Messager.MsgBoxStyle.Information, "Manual approval of permission to access all files");

                Intent intent = new Intent(Settings.ActionManageAppAllFilesAccessPermission);
                intent.SetData(Android.Net.Uri.Parse($"package:{Android.App.Application.Context.PackageName}"));
                intent.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            }
#endif
        }

        public static async Task<bool> WaitForAllFilesAccessAsync(int timeoutSeconds = 30)
        {
#if ANDROID
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                int elapsed = 0;
                while (!Android.OS.Environment.IsExternalStorageManager && elapsed < timeoutSeconds)
                {
                    await Task.Delay(1000);
                    elapsed++;
                }
                return Android.OS.Environment.IsExternalStorageManager;
            }
#endif
            return true; // If not Android R+, assume it's not needed
        }

        static void AppCacheVariableExists(object? sender, AppCacheEventArgs? e)
        {
            e.Result = Preferences.ContainsKey(e.VariableName);
        }

        static void SetAppCacheStringVariableValue(object? sender, AppCacheEventArgs? e)
        {
            Preferences.Default.Set(e.VariableName, e.VariableStringValue);
        }

        static void SetAppCacheIntegerVariableValue(object? sender, AppCacheEventArgs? e)
        {
            if (e.VariableIntegerValue != null)
            {
                Preferences.Default.Set(e.VariableName, e.VariableIntegerValue.Value);
            }
            else
            {
                throw new Exception("Unable to store null values in the app cache.");
            }
        }

        static void SetAppCacheDoubleVariableValue(object? sender, AppCacheEventArgs? e)
        {
            if (e.VariableDoubleValue != null)
            {
                Preferences.Default.Set(e.VariableName, e.VariableDoubleValue.Value);
            }
            else
            {
                throw new Exception("Unable to store null values in the app cache.");
            }
        }

        static void GetAppCacheStringVariableValue(object? sender, AppCacheEventArgs? e)
        {
            e.VariableStringValue = Preferences.Default.Get(e.VariableName, "");
        }

        static void GetAppCacheDoubleVariableValue(object? sender, AppCacheEventArgs? e)
        {
            if (Preferences.ContainsKey(e.VariableName))
            {
                e.VariableDoubleValue = Preferences.Default.Get(e.VariableName, double.NaN);
            }
            else
            {
                e.VariableDoubleValue = null;
            }
        }

        static void GetAppCacheIntegerVariableValue(object? sender, AppCacheEventArgs? e)
        {
            if (Preferences.ContainsKey(e.VariableName))
            {
                e.VariableIntegerValue = Preferences.Default.Get(e.VariableName, -1);
            }
            else
            {
                e.VariableIntegerValue = null;
            }
        }


        static void RemoveAppCacheVariable(object? sender, AppCacheEventArgs? e)
        {
            Preferences.Default.Remove(e.VariableName);
        }

        static void ClearAppCache(object? sender, EventArgs? e)
        {
            Preferences.Default.Clear();
        }



    }
}
