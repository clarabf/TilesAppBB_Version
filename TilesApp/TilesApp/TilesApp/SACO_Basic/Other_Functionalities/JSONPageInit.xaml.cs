using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class JSONPageInit : ContentPage
    {
        public JSONPageInit()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void ShowScanning(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new SACOScan("SCAN YOUR QR WITH JSON", 2, null));
        }


    }
}
