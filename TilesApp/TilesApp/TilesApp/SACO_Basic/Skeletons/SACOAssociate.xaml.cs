using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TilesApp.Rfid;
using TilesApp.Rfid.Models;
using TilesApp.Rfid.ViewModels;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOAssociate : BasePage
    {

        private string lastValue;
        private string appName;

        public ObservableCollection<string> InputDataValues { get; set; } = new ObservableCollection<string>();
        public SACOAssociate(string name)
        {
            InitializeComponent();
            BindingContext = this;
            appName = name;
            lblTest.Text = appName + " (Associate)";
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public override void ScannerReadDetected(Dictionary<string, object> input)
        {
            lblBarcode.IsVisible = true;
            btnSaveAndFinish.IsVisible = true;
            //barcode.Text = input[nameof(InputDataProps.ReaderType)].ToString() +"|"+ input[nameof(InputDataProps.Value)].ToString();
            InputDataValues.Add(input[nameof(InputDataProps.Value)].ToString());
            lastValue = input[nameof(InputDataProps.Value)].ToString();

        }


        private async void SaveAndFinish(object sender, EventArgs args)
        {
            //Update info in DB
            await DisplayAlert("Component added successfully!", "<" + lastValue + "> stored in DB.", "OK");
            await Navigation.PopModalAsync(true);
        }

        private async void Cancel(object sender, EventArgs args)
        {
            MessagingCenter.Unsubscribe<Application, String>(Application.Current, "BarcodeScanned");
            await Navigation.PopModalAsync(true);
        }


    }
}
