using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOCheckpoint : BasePage
    {

        private string appName;

        public SACOCheckpoint(string name)
        {
            InitializeComponent();
            BindingContext = this;
            appName = name;
            lblTest.Text = appName + " (Checkpoint)";
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public override void ScannerReadDetected(Dictionary<string, object> input)
        {
            lblBarcode.IsVisible = true;
            barcode.Text = input[nameof(InputDataProps.Value)].ToString();

        }

        private async void Come_Back(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

    }
}