using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TilesApp.Azure;
using TilesApp.Models.Skeletons;
using TilesApp.Odoo;
using Xamarin.Forms;

namespace TilesApp.SACO
{
    public partial class SACOAssemble : BasePage
    {
        private bool mainScanned = false;
        private string mainCode;
        private List<string> barcodes = new List<string>();
        public ObservableCollection<string> BarcodesScanned { get; set; } = new ObservableCollection<string>();
        public JoinMetaData MetaData { get; set; } = new JoinMetaData();
        public SACOAssemble(string appName)
        {
            InitializeComponent();
            BindingContext = this;
            string[] appNameArr = appName.Split('-');
            if (appNameArr.Length > 2)
            {
                BaseData.AppType = appNameArr[0] + "-" + appNameArr[1];
                BaseData.AppName = appNameArr[2];
            }
            else {
                BaseData.AppType = "JOIN";
                BaseData.AppName = appName;
            }
           
            lblTest.Text = appName + " (Assemble)";
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public override void ScannerReadDetected(Dictionary<string, object> input)
        {
            if (!mainScanned)
            {
                mainScanned = true;
                mainCode = input[nameof(InputDataProps.Value)].ToString();
                lblTitle.Text = "Scan barcode of the other components";
                BarcodesScanned.Add("Main item <" + mainCode + "> scanned (" + DateTime.Now.ToShortTimeString() + ")");
                btnSaveAndFinish.IsVisible = true;
            }
            else
            {
                barcodes.Add(input[nameof(InputDataProps.Value)].ToString());
                lblTitle.Text = "Scan barcode of the other components (" + barcodes.Count + ")";
                BarcodesScanned.Add("Item <" + input[nameof(InputDataProps.Value)].ToString() + "> scanned (" + DateTime.Now.ToShortTimeString() + ")");
            }
        }
        private async void SaveAndFinish(object sender, EventArgs args)
        {
            // Formulate the JSON
            Dictionary<string, Object> json = new Dictionary<string, object>();
            json.Add("barcodes", barcodes);
            json.Add("base", BaseData);
            json.Add("meta", MetaData);
            bool success = CosmosDBManager.InsertOneObject(json);
            //Update info in DB
            string message = "";
            foreach (string code in barcodes) message += code + " - ";
            await DisplayAlert(mainCode + " was assembled successfully!", message.Substring(0, message.Length - 2), "OK");
            await Navigation.PopModalAsync(true);
        }

        private async void Cancel(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

    }
}