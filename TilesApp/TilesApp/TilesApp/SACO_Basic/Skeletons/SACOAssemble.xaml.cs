using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TilesApp.SACO
{
    public partial class SACOAssemble : BasePage
    {
        private double width = 0;
        private double height = 0;
        private bool mainScanned = false;
        private string mainCode;
        private List<string> barcodes = new List<string>();
        public ObservableCollection<string> BarcodesScanned { get; set; } = new ObservableCollection<string>();

        public SACOAssemble()
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;
            //BarcodesScanned.Add("Main item <" + mainCode + "> scanned (" + DateTime.Now.ToShortTimeString() + ")");
        }

        public override void BarcodeDetected(string code)
        {
            if (!mainScanned)
            {
                mainScanned = true;
                mainCode = code.ToString();
                lblTitle.Text = "Scan barcode of the other components";
                BarcodesScanned.Add("Main item <" + mainCode + "> scanned (" + DateTime.Now.ToShortTimeString() + ")");
                btnSaveAndFinish.IsVisible = true;
            }
            else
            {
                barcodes.Add(code.ToString());
                lblTitle.Text = "Scan barcode of the other components (" + barcodes.Count + ")";
                BarcodesScanned.Add("Item <" + code.ToString() + "> scanned (" + DateTime.Now.ToShortTimeString() + ")");
            }
        }
        public override void TransponderDetected(string transponder)
        {

        }
        private async void SaveAndFinish(object sender, EventArgs args)
        {
            //Update info in DB
            string message="";
            foreach (string code in barcodes) message += code + " - ";
            await DisplayAlert(mainCode  + " was assembled successfully!", message.Substring(0, message.Length-2), "OK");
            await Navigation.PopModalAsync(true);
        }

        private async void Cancel(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

    }
}
