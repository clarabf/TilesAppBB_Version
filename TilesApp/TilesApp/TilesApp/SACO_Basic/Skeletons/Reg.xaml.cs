using System;
using System.Collections.Generic;
using TilesApp.Azure;
using TilesApp.Models.Skeletons;
using TilesApp.Odoo;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class Reg : BasePage
    {

        private string appName;
        public RegMetaData MetaData { get; set; }
        public Reg(string tag)
        {
            InitializeComponent();
            BindingContext = this;
            MetaData = new RegMetaData(OdooXMLRPC.appsConfigs[tag]);
            string[] appNameArr = tag.Split('_');
            BaseData.AppType = appNameArr[1];
            BaseData.AppName = appNameArr[2];
            lblTest.Text = appNameArr[2] + " (Checkpoint)";
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public override void ScannerReadDetected(Dictionary<string, object> input)
        {
            lblBarcode.IsVisible = true;
            barcode.Text = input[nameof(InputDataProps.Value)].ToString();

            // Formulate the JSON
            Dictionary<string, Object> json = new Dictionary<string, object>();
            json.Add("barcode", input[nameof(InputDataProps.Value)].ToString());
            json.Add("base", BaseData);
            json.Add("meta", MetaData);
            bool success = CosmosDBManager.InsertOneObject(json);

        }

        private async void Come_Back(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

    }
}