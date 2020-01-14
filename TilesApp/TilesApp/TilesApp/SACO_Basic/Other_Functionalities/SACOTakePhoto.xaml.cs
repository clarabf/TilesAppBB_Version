using Plugin.Media;
using System;
using System.Collections.Generic;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using TilesApp.Services;
using TilesApp.Models;

namespace TilesApp.SACO
{
    public partial class SACOTakePhoto : ContentPage
    {
        private Stream photo;
        private List<Stream> photoList = new List<Stream>();
        Dictionary<string, Stream> photosInfo = new Dictionary<string, Stream>();
        private string photoPath;
        private string appName;

        public SACOTakePhoto(string name)
        {
            InitializeComponent();
            appName = name;
            lblTest.Text = appName + " (QCRich)";
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += Show_Images;
            hyper.GestureRecognizers.Add(tapGestureRecognizer);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await CrossMedia.Current.Initialize();
                var bindingContext = BindingContext as PhotoData;

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", "No camera available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    //SaveToAlbum = true,
                    Directory = "Sample",
                    Name = "test.jpg"
                });

                if (file != null)
                {
                    photoList.Add(file.GetStream());
                    photoPath = file.Path;
                    //bindingContext.TakenPhotos.Add(new PhotoData.PhotoInfo() { Path = file.Path, Time = DateTime.Now.ToShortTimeString(), ImageSource = "delete.png" });
                }
                else
                {
                    return;
                }

                btnSaveAndFinish.BackgroundColor = Color.Black;
                btnSaveAndFinish.IsEnabled = true;
                lblListPhotos.IsVisible = true;
                numPhotos.Text = photoList.Count.ToString();

                await DisplayAlert("Photo taken correctly!", "Photo storen in <" + photoPath + ">", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Permission denied", "Error: " + ex.Message, "OK");
            }
        }

        private async void SaveAndFinish(object sender, EventArgs args)
        {
            //Update info in DB
            List<Dictionary<string, string>> results = StreamToAzure.WriteJPEGStreams(photoList, appName);
            await DisplayAlert("Photos updated successfully!", "<" + photoList.Count + "> photos were stored in DB.", "OK");
            //await Navigation.PopModalAsync(true);
        }

        private async void Show_Images(object sender, EventArgs args)
        {
            //await Navigation.PushModalAsync(new PhotoDetail(photosInfo));
        }

        private async void Cancel(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

    }
}