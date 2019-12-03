using System;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOCheckpoint : ContentPage
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
            MessagingCenter.Subscribe<Application, String>(Application.Current, "BarcodeScanned", (s, a) => {
                lblBarcode.IsVisible = true;
                barcode.Text = a.ToString();
            });
        }

        private async void Come_Back(object sender, EventArgs args)
        {
            MessagingCenter.Unsubscribe<Application, String>(Application.Current, "BarcodeScanned");
            await Navigation.PopModalAsync(true);
        }

    }
}
