using System;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOCheckpoint : BasePage
    {
        private double width = 0;
        private double height = 0;

        public SACOCheckpoint()
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
            barcode.Text = code.ToString();
        }
        public override void TransponderDetected(string transponder)
        {

        }
        private async void Come_Back(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

    }
}
