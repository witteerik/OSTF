﻿using System;
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

namespace STFM
{

    public static class StfmBase
    {

       public static STFN.Audio.SoundPlayers.iSoundPlayer SoundPlayer = null;

        
        public static async Task InitiateSTFM(Microsoft.Maui.Controls.VerticalStackLayout ParentContainer)
        {

            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {

            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {

                //var b = System.IO.Directory.Exists("/usr/share");
                OstfBase.MediaRootDirectory = "/storage/emulated/0/OstfMedia";

                await CheckPermissions();

                bool askForMediaFolder = true;
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
                    //askForMediaFolder = true;
                    //throw;
                }

                //askForMediaFolder = true;
                if (askForMediaFolder)
                {
                    await PickMediaFolder();
                }

            }
            else
            {
                OstfBase.MediaRootDirectory = "C:\\EriksDokument\\source\\repos\\OSTF\\OSTFMedia";
            }


            //FileSaver.Default.SaveAsync

            //var file = await FilePicker.PickAsync(PickOptions.Default);

            //await PickAndShow( PickOptions.Default);

            //var ExistingFiles =System.IO.Directory.GetFiles(OstfBase.MediaRootDirectory);

            //System.IO.Directory.GetFileSystemEntries(file.FullPath)

            // Initializing OSTF
            OstfBase.InitializeOSTF(OstfBase.MediaRootDirectory);

            //string tempFilePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

            //string sharedDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //sharedDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            //Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            //System.IO.Directory.GetDirectories("/data/user/0/");

            //var x = 1;

            // Initializing the sound player
            SoundPlayer = new STFM.SoundPlayer(ParentContainer);


        }

        public static Sound testSound = null;

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

            testSound = TestWords[1].GetSound(ref selectedMediaSet, 0, 1, null, null, null, ref InitialMargin, false, false, false, null, ref SmaList,false);

            SoundPlayer.SwapOutputSounds(ref testSound);

            //SipTest.SipMeasurement SiPMeasurement = new SipTest.SipMeasurement("TrialSoundGeneration", SpeechMaterial.ParentTestSpecification);

            //// Clearing any trials that may have been planned by a previous call
            //SiPMeasurement.ClearTrials();

            //SiPMeasurement.TestProcedure.TestParadigm = Testparadigm.Slow;

            }

        public static void playAgain()
        {
            if (testSound != null )
            {
                SoundPlayer.SwapOutputSounds(ref testSound);
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

        async static Task<FileResult> PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        using var stream = await result.OpenReadAsync();
                        var image = ImageSource.FromStream(() => stream);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }

            return null;
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