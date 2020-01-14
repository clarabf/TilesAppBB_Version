﻿using System;
using System.Collections.Generic;
using TilesApp.Services;
using TilesApp.Models.Skeletons;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Plugin.Media;
using System.IO;
using TilesApp.Models;

namespace TilesApp.Views
{
    public partial class QC : BasePage
    {
        private string appName;
        private List<Stream> photoList = new List<Stream>();
        public QCMetaData MetaData { get; set; }
        public ObservableCollection<string> InputDataValues { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<PhotoData> TakenPhotos { get; set; } = new ObservableCollection<PhotoData>();
        public QC(string tag)
        {
            InitializeComponent();
            BindingContext = this;
            
            //MetaData = new QCMetaData(OdooXMLRPC.GetAppConfig(tag));
            string[] appNameArr = tag.Split('_');
            //MetaData.AppType = appNameArr[1];
            //MetaData.AppName = appNameArr[2];
            lblTest.Text = appNameArr[2] + " (QC)";
            appName = appNameArr[2];
            
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += Show_Images;
            hyper.GestureRecognizers.Add(tapGestureRecognizer);

            MessagingCenter.Subscribe<PhotoDetail, ObservableCollection<PhotoData>>(this, "SendPhotos", (s, a) => {
                TakenPhotos = a;
                setPhotosList();
                numPhotos.Text = a.Count.ToString();
            });

            NavigationPage.SetHasNavigationBar(this, false);
        }

        public override void ScannerReadDetected(Dictionary<string, object> input)
        {
            InputDataValues.Add(input[nameof(BaseData.InputDataProps.Value)].ToString());
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
                    SaveToAlbum = true,
                    Name = "test.jpg"
                });

                if (file != null)
                {
                    string[] fileName = file.Path.Split('/');
                    TakenPhotos.Add(new PhotoData() { 
                        FileName = fileName[fileName.Length-1], 
                        DateAndTime = DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString(), 
                        ImageSource = "delete.png",
                        FileContent = file.GetStream()
                    });;
                }
                else
                {
                    return;
                }

                lblListPhotos.IsVisible = true;
                numPhotos.Text = TakenPhotos.Count.ToString();

                await DisplayAlert("Photo taken correctly!", "Photo storen in <" + file.Path + ">", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Permission denied", "Error: " + ex.Message, "OK");
            }
        }
        private async void PassOrFail(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            string messageTitle;
            string messageContent;
            if (b.Text == "PASS")
            {
                //Update PASS info
                messageTitle = "Success!";
                messageContent = "Component(s) passed successfully to the next step!";
                List<Dictionary<string, string>> results = StreamToAzure.WriteJPEGStreams(photoList, appName);
            }
            else
            {
                //Update FAIL info
                messageTitle = "Failure";
                messageContent = "Component(s) failed the Quality Control...";
            }
            // Formulate the JSON
            //if (MetaData.IsValid())
            //{
            //    Dictionary<string, Object> json = new Dictionary<string, object>();
            //    json.Add("barcode", "123456");
            //    json.Add("base", BaseData);
            //    json.Add("meta", MetaData);
            //    bool success = CosmosDBManager.InsertOneObject(json);

            //    await DisplayAlert("Check-out of the component", message, "Ok");

            //}
            //else
            //{
            //    await DisplayAlert("Error fetching Meta Data!", "Please contact your Odoo administrator", "OK");
            //}
            await DisplayAlert(messageTitle, messageContent, "OK");
            await Navigation.PopModalAsync(true);
        }
        private async void Show_Images(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new PhotoDetail(TakenPhotos));
        }
        private async void Cancel(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }
        private void setPhotosList()
        {
            photoList.Clear();
            foreach (PhotoData phD in TakenPhotos)
            {
                photoList.Add(phD.FileContent);
            }
        }

    }
}