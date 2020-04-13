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
                MetaData = new LinkMetaData(PHPApi.GetAppConfig(tag));
                string[] appNameArr = tag.Split('_');
                MetaData.AppType = appNameArr[1];
                MetaData.AppName = appNameArr[2];
                if (MetaData.Station == null) MetaData.Station = App.Station;
                lblTest.Text = appNameArr[2].ToUpper() + " (LINK)";
            }
            catch
            {
                DisplayAlert("Error", "Config file is not valid. Maybe there are syntax issues or one or several field names are duplicated.", "Ok");
                Device.BeginInvokeOnMainThread(async () =>
                {
                    CleanReaders();
                    await Navigation.PopModalAsync(true);
                });
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
            Dictionary<string, object> returnedData = MetaData.ProcessScannerRead(input);
            if (returnedData.Count > 1)
            {
                lblBarcode.IsVisible = true;
                lblEmptyView.IsVisible = false;
                lblEmptyViewAnimation.IsVisible = false;
                lblBarcodeLine.IsVisible = true;
                btnSaveAndFinish.IsVisible = true;
                ViewableReads.Add(input[nameof(BaseMetaData.InputDataProps.Value)].ToString());
            }
            //Additional data has been scanned
            else
            {
                if (ViewableReads.Count > 0)
                {
                    if (returnedData.Count == 0)
                    {
                        List<string> errorsList = MetaData.IsValid();
                        if (errorsList.Count == 0)
                        {
                            btnSaveAndFinish.IsVisible = true;
                            DisplayAlert("Success!", "Additional data scanned successfully!", "Ok");
                        }
                        else
                        {
                            string message = "Additional data scanned successfully but some fields missing in config file:\n";
                            foreach (string error in errorsList) message += error + ", ";
                            DisplayAlert("Warning", message.Substring(0, message.Length - 2), "OK");
                        }
                    }
                    else
                    {
                        try
                        {
                            DisplayAlert("Error", returnedData["Error"].ToString(), "Ok");
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private void Delete_ScannerRead(object sender, EventArgs args)
        {
            App.ViewModel.Inventory.ClearCommand.Execute(null);
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
        private async void SaveAndFinish(object sender, EventArgs args)
        {
            List<string> errorsList = MetaData.IsValid();
            if (errorsList.Count==0)
            {
                if (!App.IsConnected) MetaData.DoneOffline = true;
                KeyValuePair<string, string> resultInsertion = CosmosDBManager.InsertOneObject(MetaData);
                if (resultInsertion.Key == "Success") 
                { 
                    string message = "";
                    foreach (Dictionary<string, object> item in MetaData.ScannerReads)
                    {
                        message += item[nameof(BaseMetaData.InputDataProps.Value)].ToString() + " - ";
                    }
                    await DisplayAlert("Component/s linked successfully! (" + resultInsertion.Value + ")", message.Substring(0, message.Length - 2), "OK");
                    lblBarcode.IsVisible = false;
                    lblBarcodeLine.IsVisible = false;
                    btnSaveAndFinish.IsVisible = false;
                    lblEmptyView.IsVisible = true;
                    lblEmptyViewAnimation.IsVisible = true;
                    ViewableReads.Clear();
                    MetaData.ScannerReads.Clear();
                }
                else
                    await DisplayAlert("Component/s NOT linked successfully...", "Something went wrong in the operation.", "OK");
            }
            else
            {
                //CleanReaders();
                string message = "The following fields are not completed:\n";
                foreach (string error in errorsList) message += error + ", ";
                await DisplayAlert("Error processing Meta Data!", message.Substring(0, message.Length - 2), "OK");
                //await Navigation.PopModalAsync(true);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                CleanReaders();
                await Navigation.PopModalAsync(true);
            });
            return true;
        }
    }
}
