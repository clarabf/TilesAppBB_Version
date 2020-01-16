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
        private bool mainScanned = false;
        private string mainCode;
        private List<string> barcodes = new List<string>();
        public ObservableCollection<string> BarcodesScanned { get; set; } = new ObservableCollection<string>();
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
            if (!mainScanned)
            {
                mainScanned = true;
                mainCode = input[nameof(BaseData.InputDataProps.Value)].ToString();
                lblTitle.Text = "Scan barcode of the other components";
                BarcodesScanned.Add("Main item <" + mainCode + "> scanned (" + DateTime.Now.ToShortTimeString() + ")");
                btnSaveAndFinish.IsVisible = true;
            }
            else
            {
                barcodes.Add(input[nameof(BaseData.InputDataProps.Value)].ToString());
                lblTitle.Text = "Scan barcode of the other components (" + barcodes.Count + ")";
                BarcodesScanned.Add("Item <" + input[nameof(BaseData.InputDataProps.Value)].ToString() + "> scanned (" + DateTime.Now.ToShortTimeString() + ")");
            }
        }
        private async void SaveAndFinish(object sender, EventArgs args)
        {
            // Iterate over the scanned codes and process them
            /*foreach (var scannerRead in ScannerReads)
            {
                MetaData.ProcessInput(scannerRead);
            }*/
            MetaData.ScannerReads = ScannerReads;
            if (MetaData.IsValid()||true)
            {                
                bool success = CosmosDBManager.InsertOneObject(MetaData);
                string message = "";
                foreach (string code in barcodes) message += code + " - ";
                await DisplayAlert(mainCode + " was assembled successfully!", message.Substring(0, message.Length - 2), "OK");

            }
            else
            {
                await DisplayAlert("Error fetching Meta Data!", "Please contact your Odoo administrator", "OK");
            }
            await Navigation.PopModalAsync(true);
        }

        private async void Cancel(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

    }
}