using System;
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
        public ObservableCollection<PhotoData> TakenPhotos { get; set; } = new ObservableCollection<PhotoData>();
        public QC(string tag)
        {
            InitializeComponent();
            BindingContext = this;
            try
            {
                MetaData = new QCMetaData(OdooXMLRPC.GetAppConfig(tag));
                string[] appNameArr = tag.Split('_');
                MetaData.AppType = appNameArr[1];
                MetaData.AppName = appNameArr[2];
                if (MetaData.Station == null) MetaData.Station = App.Station;
                lblTest.Text = appNameArr[2].ToUpper() + " (QC)";
                appName = appNameArr[2];
            }
            catch
            {
                DisplayAlert("Error", "Config file is not valid. Maybe there are syntax issues or one or several field names are duplicated.", "Ok");
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PopModalAsync(true);
                });
            }
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += Show_Images;
            hyper.GestureRecognizers.Add(tapGestureRecognizer);

            MessagingCenter.Subscribe<PhotoDetail, ObservableCollection<PhotoData>>(this, "SendPhotos", (s, a) => {
                TakenPhotos = a;
                setPhotosList();
                if (TakenPhotos.Count > 0) numPhotos.Text = TakenPhotos.Count.ToString();
                //else
                //{ hyper.IsVisible = false; } 
            });

            NavigationPage.SetHasNavigationBar(this, false);
        }

        public override void ScannerReadDetected(Dictionary<string, object> input)
        {
            foreach (Dictionary<string, object> item in MetaData.ScannerReads)
            {
                if (item[nameof(BaseMetaData.InputDataProps.Value)].ToString() == input[nameof(BaseMetaData.InputDataProps.Value)].ToString())
                {
                    return;
                }
            }
            Dictionary<string,object> returnedData = MetaData.ProcessScannerRead(input);
            if (returnedData.Count > 0)
            {
                btPass.IsVisible = true;
                btFail.IsVisible = true;
                lblTitle.IsVisible = true;
                lblTitleLine.IsVisible = true;
                btTakePicture.IsVisible = true;
                hyper.IsVisible = true;
                ViewableReads.Add(input[nameof(BaseMetaData.InputDataProps.Value)].ToString());
            }
            //QR has been scanned
            else
            {
                if (ViewableReads.Count > 0) {
                    if (MetaData.IsValid())
                    {
                        btPass.IsVisible = false;
                        btFail.IsVisible = false;
                        btnSaveAndFinish.IsVisible = true;
                        DisplayAlert("Success!", "QR scanned successfully!", "Ok");
                    }
                    else
                    {
                        DisplayAlert("Warning", "QR scanned successfully but some fields missing in config file...", "Ok");
                    }
                }
            }
            lblEmptyView.IsVisible = false;
            lblEmptyViewAnimation.IsVisible = false;
        }

        private void Delete_ScannerRead(object sender, EventArgs args)
        {
            Button button = (Button)sender;
            string removedObject = button.ClassId;
            // Remove from both the viewable list and the ScannerReads 
            ViewableReads.Remove(button.ClassId);
            if (ViewableReads.Count == 0)
            {
                btPass.IsVisible = false;
                btFail.IsVisible = false;
                btnSaveAndFinish.IsVisible = false;
                lblTitle.IsVisible = false;
                lblTitleLine.IsVisible = false;
                btTakePicture.IsVisible = false;
                hyper.IsVisible = false;
                lblEmptyView.IsVisible = true;
                lblEmptyViewAnimation.IsVisible = true;
            }
            foreach (Dictionary<string, object> item in MetaData.ScannerReads)
            {
                if (item[nameof(BaseMetaData.InputDataProps.Value)].ToString() == removedObject)
                {
                    MetaData.ScannerReads.Remove(item);
                    return;
                }
            }
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
                        FileContent = file.GetStream()
                    });
                    photoList.Add(file.GetStream());
                }
                else
                {
                    return;
                }
                
                numPhotos.Text = TakenPhotos.Count.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Permission denied", "Error: " + ex.Message, "OK");
            }
        }

        private async void PassOrFail(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            if (b.Text == "SAVE AS PASS") MetaData.QCPass = true;
            else if (b.Text == "SAVE AS FAIL") MetaData.QCPass = false;
            if (MetaData.IsValid())
            {
                Collection<string> urls = new Collection<string>();
                if (photoList.Count > 0) urls = StreamToAzure.UpdateJPEGStreams(photoList, appName);
                MetaData.Images = urls;
                bool submitted = CosmosDBManager.InsertOneObject(MetaData);
                if (submitted)
                {
                    string message = "";
                    foreach (Dictionary<string, object> item in MetaData.ScannerReads)
                    {
                        message += item[nameof(BaseMetaData.InputDataProps.Value)].ToString() + " - ";
                    }
                    await DisplayAlert("Report was delivered successfully!", message.Substring(0, message.Length - 2), "OK");
                    btPass.IsVisible = false;
                    btFail.IsVisible = false;
                    btnSaveAndFinish.IsVisible = false;
                    lblTitle.IsVisible = false;
                    lblTitleLine.IsVisible = false;
                    btTakePicture.IsVisible = false;
                    hyper.IsVisible = false;
                    lblEmptyView.IsVisible = true;
                    lblEmptyViewAnimation.IsVisible = true;
                    ViewableReads.Clear();
                    MetaData.ScannerReads.Clear();
                }
                else
                    await DisplayAlert("Report was NOT delivered successfully...", "We could not connect to the Database Server", "OK");
            }
            else if (MetaData.QCResultDetails == null)
            {
                await DisplayAlert("Error:", "Please scan operation QR!", "OK");
                return;
            }
            else
            {
                await DisplayAlert("Error processing Meta Data!", "Please contact your Odoo administrator", "OK");
                await Navigation.PopModalAsync(true);
            }
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

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PopModalAsync(true);
            });
            return true;
        }

    }
}