using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TilesApp.Services;
using TilesApp.Models.Skeletons;
using Xamarin.Forms;
using TilesApp.Models;

namespace TilesApp.Views
{
    public partial class Link : BasePage
    {
        public LinkMetaData MetaData { get; set; }
        public Link(string tag)
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            try
            {
                MetaData = new LinkMetaData(OdooXMLRPC.GetAppConfig(tag));
                string[] appNameArr = tag.Split('_');
                MetaData.AppType = appNameArr[1];
                MetaData.AppName = appNameArr[2];
                lblTest.Text = appNameArr[2].ToUpper() + " (LINK)";
            }
            catch
            {
                DisplayAlert("Error", "Config file is not valid. Maybe there are syntax issues or one or several field names are duplicated.", "Ok");
            }
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
            lblBarcode.IsVisible = true;
            lblEmptyView.IsVisible = false;
            lblBarcodeLine.IsVisible = true;
            btnSaveAndFinish.IsVisible = true;
            MetaData.ProcessScannerRead(input);
            ViewableReads.Add(input[nameof(BaseMetaData.InputDataProps.Value)].ToString());
        }

        private void Delete_ScannerRead(object sender, EventArgs args)
        {
            Button button = (Button)sender;
            string removedObject = button.ClassId;
            // Remove from both the viewable list and the ScannerReads 
            ViewableReads.Remove(button.ClassId);
            if (ViewableReads.Count == 0)
            {
                lblBarcode.IsVisible = false;
                lblBarcodeLine.IsVisible = false;
                btnSaveAndFinish.IsVisible = false;
                lblEmptyView.IsVisible = true;
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
        private async void SaveAndFinish(object sender, EventArgs args)
        {
            if (MetaData.IsValid())
            {
                if (CosmosDBManager.InsertOneObject(MetaData))
                {
                    string message = "";
                    foreach (Dictionary<string, object> item in MetaData.ScannerReads)
                    {
                        message += item[nameof(BaseMetaData.InputDataProps.Value)].ToString() + " - ";
                    }
                    await DisplayAlert(" Component/s were associated to their barcodes successfully!", message.Substring(0, message.Length - 2), "OK");
                }
                else
                    await DisplayAlert(" Component/s were NOT associated to their barcodes successfully!", "We could not connect to the Database Server", "OK");
            }
            else
            {
                await DisplayAlert("Error processing Meta Data!", "Please contact your Odoo administrator", "OK");
            }
            await Navigation.PopModalAsync(true);
        }

        private async void Cancel(object sender, EventArgs args)
        {
            MessagingCenter.Unsubscribe<Application, String>(Application.Current, "BarcodeScanned");
            await Navigation.PopModalAsync(true);
        }
    }
}
