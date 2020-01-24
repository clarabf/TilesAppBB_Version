using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TilesApp.Services;
using TilesApp.Models.Skeletons;
using Xamarin.Forms;
using TilesApp.Models;

namespace TilesApp.Views
{
    public partial class Join : BasePage
    {
        public JoinMetaData MetaData { get; set; }
        public Join(string tag)
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            try
            {
                MetaData = new JoinMetaData(OdooXMLRPC.GetAppConfig(tag));
                string[] appNameArr = tag.Split('_');
                MetaData.AppType = appNameArr[1];
                MetaData.AppName = appNameArr[2];
                lblTest.Text = appNameArr[2] + " (Assemble)";
            }
            catch
            {
                DisplayAlert("Error", "Config file is not valid. Maybe there are syntax issues or one or several field names are duplicated.", "Ok");
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PopModalAsync(true);
                });
            }
        }

        public override void ScannerReadDetected(Dictionary<string, object> input)
        {
            if (MetaData.ParentUUID != null)
            {
                if (MetaData.ParentUUID == input[nameof(BaseMetaData.InputDataProps.Value)].ToString()) return;
            }          
            foreach (string item in ViewableReads)
            {
                if (item.Equals(input[nameof(BaseMetaData.InputDataProps.Value)].ToString()))
                {
                    return;
                }
            }
            Dictionary<string, object>  processedInput = MetaData.ProcessScannerRead(input);
            bool isParent = false;
            try
            {
                isParent = (bool)processedInput["IsParent"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            if (isParent)
            {
                lblParentBarcode.IsVisible = true;
                lblParentBarcode.Text = "Parent is: "+input[nameof(BaseMetaData.InputDataProps.Value)].ToString();
                btnSaveAndFinish.IsVisible = true;
            }
            else {
                ViewableReads.Add(input[nameof(BaseMetaData.InputDataProps.Value)].ToString());
                lblComponents.IsVisible = true;
            }
            
        }

        private void Delete_ScannerRead(object sender, EventArgs args)
        {
            ImageButton button = (ImageButton)sender;
            string removedObject = button.ClassId;
            // Remove from both the viewable list and the ScannerReads 
            ViewableReads.Remove(button.ClassId);
            if (ViewableReads.Count == 0)
            {
                btnSaveAndFinish.IsVisible = false;
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
                if (CosmosDBManager.InsertOneObject(MetaData)) {
                    string message = "";
                    foreach (Dictionary<string, object> item in MetaData.ScannerReads)
                    {
                        message += item[nameof(BaseMetaData.InputDataProps.Value)].ToString() + " - ";
                    }
                    await DisplayAlert(MetaData.ParentUUID + " was assembled successfully!", message.Substring(0, message.Length - 2), "OK");
                }
                else
                    await DisplayAlert(MetaData.ParentUUID + " was NOT assembled successfully!", "We could not connect to the Database Server", "OK");
            }
            else
            {
                await DisplayAlert("Error processing Meta Data!", "Please contact your Odoo administrator", "OK");
            }
            await Navigation.PopModalAsync(true);
        }

        private async void Cancel(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

    }
}