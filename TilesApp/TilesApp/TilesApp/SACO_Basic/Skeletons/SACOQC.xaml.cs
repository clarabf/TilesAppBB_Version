using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TilesApp.SACO
{
    public partial class SACOQC : BasePage
    {
        private string appName;

        public SACOQC(string name)
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            appName = name;
            lblTest.Text = appName + " (QC)";
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
            await DisplayAlert("Check-out of the component", message, "Ok");
            await Navigation.PopModalAsync(true);
        }

        private async void Cancel(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

    }
}