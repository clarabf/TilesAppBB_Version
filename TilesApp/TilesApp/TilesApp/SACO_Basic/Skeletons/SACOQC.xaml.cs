using System;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOQC : BasePage
    {
        private double width = 0;
        private double height = 0;

        public SACOQC()
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;
        }
        public override void BarcodeDetected(string code)
        {
            lblBarcode.IsVisible = true;
            btFail.IsVisible = true;
            btPass.IsVisible = true;
            barcode.Text = code.ToString();
        }
        public override void TransponderDetected(string transponder)
        {
            
        }
        private async void PassOrFail(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            string message = "";
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
