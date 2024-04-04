using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using STFN;
using STFN.Audio;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace STFM
{

    public static class StfmBase
    {

        public static IServiceProvider Services { get; set; }


        public static bool IsInitialized = false;

        public static async Task InitializeSTFM(Microsoft.Maui.Controls.VerticalStackLayout ParentContainer, STFN.OstfBase.MediaPlayerTypes MediaPlayerType = OstfBase.MediaPlayerTypes.Default)
        {

            // Returning if already called
            if (IsInitialized == true)
            {
                return;
            }
            IsInitialized = true;

            await CheckAndSetMediaRootDirectory();

            await CheckAndSetTestResultsRootFolder();

            // Initializing OSTF
            OstfBase.InitializeOSTF(GetCurrentPlatform(), MediaPlayerType, OstfBase.MediaRootDirectory);
            if (OstfBase.CurrentMediaPlayerType == OstfBase.MediaPlayerTypes.MctBased)
            {

                if (DeviceInfo.Current.Platform == DevicePlatform.Android) {

                    var _backgroundService = STFM.StfmBase.Services.GetRequiredService<Microsoft.Extensions.Hosting.IHostedService>();

                    //var _backgroundService = STFM.StfmBase.Services.GetService<STFM.AndroidAudioTrackPlayer>();

                    OstfBase.SoundPlayer = (STFM.AndroidAudioTrackPlayer)_backgroundService;
                    //STFN.Audio.SoundScene.DuplexMixer Mixer = new STFN.Audio.SoundScene.DuplexMixer();
                    //int[] OutputChannels = new int[] { 1, 2 };
                    //Mixer.DirectMonoSoundToOutputChannels(ref OutputChannels);
                    //OstfBase.SoundPlayer = new STFM.AndroidAudioTrackPlayer(ref Mixer);
                }
                else
                {
                    throw new NotImplementedException("Sound player not implemented for the current platform!");
                    //OstfBase.SoundPlayer = new STFM.MauiCtBasedSoundPlayer(ParentContainer);
                }
            }
        }

        static OstfBase.Platforms GetCurrentPlatform() {

            if (DeviceInfo.Current.Platform == DevicePlatform.iOS) {return OstfBase.Platforms.iOS;}
            else if (DeviceInfo.Current.Platform == DevicePlatform.WinUI) { return OstfBase.Platforms.WinUI; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.UWP) { return OstfBase.Platforms.UWP; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Tizen) { return OstfBase.Platforms.Tizen; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.tvOS) { return OstfBase.Platforms.tvOS; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst) { return OstfBase.Platforms.MacCatalyst; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.macOS) { return OstfBase.Platforms.macOS; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.watchOS) { return OstfBase.Platforms.watchOS; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Unknown) { return OstfBase.Platforms.Unknown; }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Android) { return OstfBase.Platforms.Android; }
            else { throw new Exception("Failed to resolve the current platform type."); }
           
        }


        async static Task CheckAndSetMediaRootDirectory()
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
                await CheckPermissions();
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

        async static Task PickMediaFolder()
        {
            var result = await FolderPicker.PickAsync(CancellationToken.None);
            //var result = await FolderPicker.Default.PickAsync(CancellationToken.None);
            if (result.IsSuccessful)
            {
                await Toast.Make($"The folder was picked: Name - {result.Folder.Name}, Path - {result.Folder.Path}", ToastDuration.Long).Show(CancellationToken.None);
                OstfBase.MediaRootDirectory = result.Folder.Path;
            }
            else
            {
                await Toast.Make($"The folder was not picked with error: {result.Exception.Message}").Show(CancellationToken.None);
            }
        }



        static string ReadMediaRootDirectory()
        {
            if (Preferences.Default.ContainsKey("media_root_directory"))
            {
                return Preferences.Default.Get("media_root_directory", "");
            }
            else { 
                return ""; 
            }
        }

        static void StoreMediaRootDirectory(string mediaRootDirectory)
        {
            Preferences.Default.Set("media_root_directory", mediaRootDirectory);
        }

        static void ClearMediaRootDirectoryFromPreferences(string mediaRootDirectory)
        {
            Preferences.Default.Remove("media_root_directory");
        }



        async static Task CheckAndSetTestResultsRootFolder()
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
                await CheckPermissions();
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


        async static Task PickTestResultsRootFolder()
        {
            var result = await FolderPicker.PickAsync(CancellationToken.None);
            //var result = await FolderPicker.Default.PickAsync(CancellationToken.None);
            if (result.IsSuccessful)
            {
                await Toast.Make($"The folder was picked: Name - {result.Folder.Name}, Path - {result.Folder.Path}", ToastDuration.Long).Show(CancellationToken.None);
                SharedSpeechTestObjects.TestResultsRootFolder = result.Folder.Path;
            }
            else
            {
                await Toast.Make($"The folder was not picked with error: {result.Exception.Message}").Show(CancellationToken.None);
            }
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

        static void StoreTestResultRootDirectory(string TestResultRootDirectory)
        {
            Preferences.Default.Set("test_result_root_directory", TestResultRootDirectory);
        }

        static void ClearTestResultRootDirectoryFromPreferences(string TestResultRootDirectory)
        {
            Preferences.Default.Remove("test_result_root_directory");
        }


        async static Task CheckPermissions()
        {

            bool hasStorageReadPermission = await Permissions.CheckStatusAsync<Permissions.StorageRead>() == PermissionStatus.Granted;
            bool hasStorageWritePermission = await Permissions.CheckStatusAsync<Permissions.StorageWrite>() == PermissionStatus.Granted;
            bool hasMediaPermission = await Permissions.CheckStatusAsync<Permissions.Media>() == PermissionStatus.Granted;
            bool hasPhotoPermission = await Permissions.CheckStatusAsync<Permissions.Photos>() == PermissionStatus.Granted;

            if (hasStorageReadPermission == false)
            {
                var status = await Permissions.RequestAsync<Permissions.StorageRead>();
            }

            if (hasStorageWritePermission == false)
            {
                var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            if (hasMediaPermission == false)
            {
                var status = await Permissions.RequestAsync<Permissions.Media>();
            }

            if (hasPhotoPermission == false)
            {
                var status = await Permissions.RequestAsync<Permissions.Photos>();
            }

        }

    }

}
