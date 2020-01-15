using System;
using System.Collections.Generic;
using TilesApp.Services;
using TilesApp.Models.Skeletons;
using Xamarin.Forms;
using TilesApp.Models;

namespace TilesApp.Views
{
    public partial class Reg : BasePage
    {

        private string appName;
        public RegMetaData MetaData { get; set; }
        public Reg(string tag)
        {
            InitializeComponent();
            BindingContext = this;
            MetaData = new RegMetaData(OdooXMLRPC.GetAppConfig(tag));
            string[] appNameArr = tag.Split('_');
            MetaData.AppType = appNameArr[1];
            MetaData.AppName = appNameArr[2];
            lblTest.Text = appNameArr[2] + " (Checkpoint)";
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public override void ScannerReadDetected(Dictionary<string, object> input)
        {
            lblBarcode.IsVisible = true;
            barcode.Text = input[nameof(BaseData.InputDataProps.Value)].ToString();
            btnSaveAndFinish.IsVisible = true;
        }

        private async void SaveAndFinish(object sender, EventArgs args)
        {
            // Iterate over the scanned codes and process them
            foreach (var scannerRead in ScannerReads)
            {
                MetaData.ProcessInput(scannerRead);
            }
            if (MetaData.IsValid()||true)
            {
                bool success = CosmosDBManager.InsertOneObject(MetaData);
                await DisplayAlert(barcode.Text + " was regitered successfully!", barcode.Text, "OK");
            }
            else
            {
                await DisplayAlert("Error fetching Meta Data!", "Please contact your Odoo administrator", "OK");
            }
            await Navigation.PopModalAsync(true);
        }
        private async void Come_Back(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

    }
}