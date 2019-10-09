using System;
using TilesApp.Models;
using Xamarin.Forms;

namespace TilesApp
{
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void GoToScan(object sender, EventArgs args)
        {
            Tile t = new Tile();
            t.id = 2;
            await Navigation.PushModalAsync(new ScanQR(t));
        }

    }
}
