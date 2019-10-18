using System;
using Xamarin.Forms;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using TilesApp.Models;

namespace TilesApp.V1
{
    public partial class TestTableOrder : ContentPage
    {

        public TestTableOrder(string user_name)
        {
            InitializeComponent();
            user.Text = user_name;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void GoToStep(object sender, EventArgs args)
        {
            Tile t = new Tile(); t.id = 2;
            await Navigation.PushModalAsync(new StepsPage(t, 2, 5, "user", "http://oboria.net/docs/pdf/ftp/2/3.PDF",3));
        }

    }
}
