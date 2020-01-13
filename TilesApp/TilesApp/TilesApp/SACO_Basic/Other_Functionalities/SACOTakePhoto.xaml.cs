using Plugin.Media;
using System;
using System.Collections.Generic;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using TilesApp.Azure;

namespace TilesApp.SACO
{

    public partial class SACOTakePhoto : ContentPage
    {
        private Stream photo;
        private string photoPath;
        private string appName;

        public SACOTakePhoto(string name)
        {
            InitializeComponent();
            appName = name;
            lblTest.Text = appName + " (QCRich)";
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", "No camera available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    //SaveToAlbum=true
                    Directory = "Sample",
                    Name = "test.jpg"
                });
                
                if (file == null)
                    return;

                btnSaveAndFinish.BackgroundColor = Color.Black;
                btnSaveAndFinish.IsEnabled = true;

                photo = file.GetStream();
                photoPath = file.AlbumPath;

                //PhotoImage.Source = ImageSource.FromStream(() =>
                //{
                //    photo = file.GetStream();
                //    photoPath = file.AlbumPath;
                //    return photo;
                //});

                await DisplayAlert("Photo taken correctly", file.AlbumPath, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Permission denied", "Error: " + ex.Message, "OK");
            }
        }

        private async void SaveAndFinish(object sender, EventArgs args)
        {
            //Update info in DB
            StreamToAzure.WriteJPEGStream(photo, appName);

            await DisplayAlert("Photo updated successfully!", "<" + photoPath + "> stored in DB.", "OK");
            //await Navigation.PopModalAsync(true);
        }

        private async void Cancel(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

    }
}