using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOTakePhoto : ContentPage
    {
        private double width = 0;
        private double height = 0;
        private Stream photo;
        private string photoPath;

        public SACOTakePhoto()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;
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
                    SaveToAlbum=true
                    //Directory = "Sample",
                    //Name = "test.jpg"
                });

                if (file == null)
                    return;

                btnSaveAndFinish.BackgroundColor = Color.Black;
                btnSaveAndFinish.IsEnabled = true;

                PhotoImage.Source = ImageSource.FromStream(() =>
                {
                    photo = file.GetStream();
                    photoPath = file.AlbumPath;
                    return photo;
                });

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
            await DisplayAlert("Photo updated successfully!", "<" + photoPath + "> stored in DB.", "OK");
            await Navigation.PopModalAsync(true);
        }

        private async void Cancel(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

    }
}
