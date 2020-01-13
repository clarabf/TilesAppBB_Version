using System;
using System.Collections.Generic;
using TilesApp.Services;
using TilesApp.Models.Skeletons;
using Xamarin.Forms;

namespace TilesApp.SACO
{
    public partial class QC : BasePage
    {
        private string appName;
        public QCMetaData MetaData { get; set; }
        public QC(string tag)
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            MetaData = new QCMetaData(OdooXMLRPC.GetAppConfig(tag));
            string[] appNameArr = tag.Split('_');
            BaseData.AppType = appNameArr[1];
            BaseData.AppName = appNameArr[2];
            lblTest.Text = appNameArr[2] + " (QC)";
        }
        public override void ScannerReadDetected(Dictionary<string, object> input)
        {
            lblBarcode.IsVisible = true;
            btFail.IsVisible = true;
            btPass.IsVisible = true;
            barcode.Text = input[nameof(InputDataProps.Value)].ToString();

        }
        private async void PassOrFail(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            string message;
            if (b.Text == "PASS")
            {
                //Update PASS info
                message = "<" + barcode.Text + "> has passed successfully to the next step!";
            }
            else
            {
                //Update FAIL info
                message = "<" + barcode.Text + "> has failed the Quality Control...";
            }
            // Formulate the JSON
            if (MetaData.IsValid())
            {
                Dictionary<string, Object> json = new Dictionary<string, object>();
                json.Add("barcode", barcode.Text);
                json.Add("base", BaseData);
                json.Add("meta", MetaData);
                bool success = CosmosDBManager.InsertOneObject(json);

                await DisplayAlert("Check-out of the component", message, "Ok");

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