using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TilesApp.Services;
using TilesApp.Models.Skeletons;
using Xamarin.Forms;
using TilesApp.Models;

namespace TilesApp.SACO
{
    public partial class Link : BasePage
    {

        private string lastValue;
        public LinkMetaData MetaData { get; set; }

        public ObservableCollection<string> InputDataValues { get; set; } = new ObservableCollection<string>();
        public Link(string tag)
        {
            InitializeComponent();
            BindingContext = this;
            MetaData = new LinkMetaData(OdooXMLRPC.GetAppConfig(tag));
            string[] appNameArr = tag.Split('_');
            MetaData.AppType = appNameArr[1];
            MetaData.AppName = appNameArr[2];
            lblTest.Text = appNameArr[2] + " (Associate)";
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public override void ScannerReadDetected(Dictionary<string, object> input)
        {
            lblBarcode.IsVisible = true;
            btnSaveAndFinish.IsVisible = true;
            //barcode.Text = input[nameof(InputDataProps.ReaderType)].ToString() +"|"+ input[nameof(InputDataProps.Value)].ToString();
            InputDataValues.Add(input[nameof(BaseData.InputDataProps.Value)].ToString());
            lastValue = input[nameof(BaseData.InputDataProps.Value)].ToString();

        }


        private async void SaveAndFinish(object sender, EventArgs args)
        {
            //MetaData.ScannerReads = ScannerReads;
            if (MetaData.IsValid())
            {
                bool success = CosmosDBManager.InsertOneObject(MetaData);
                await DisplayAlert("Component added successfully!", "<" + lastValue + "> stored in DB.", "OK");
            }
            else
            {
                await DisplayAlert("Error fetching Meta Data!", "Please contact your Odoo administrator", "OK");
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
