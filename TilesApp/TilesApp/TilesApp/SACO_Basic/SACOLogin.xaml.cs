using System;
using TilesApp.Models;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOLogin : ContentPage
    {
        private double width = 0;
        private double height = 0;

        public SACOLogin()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;

        }

        private async void GoToScan(object sender, EventArgs args)
        {
            Tile t = new Tile();
            t.id = 2;
            await Navigation.PushModalAsync(new SACOTests());
        }

    }
}
