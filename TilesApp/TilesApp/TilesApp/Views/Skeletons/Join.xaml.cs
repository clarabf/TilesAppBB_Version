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
            MetaData = new JoinMetaData(OdooXMLRPC.GetAppConfig(tag));
            string[] appNameArr = tag.Split('_');
            MetaData.AppType = appNameArr[1];
            MetaData.AppName = appNameArr[2];           
            lblTest.Text = appNameArr[2] + " (Assemble)";
            NavigationPage.SetHasNavigationBar(this, false);
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