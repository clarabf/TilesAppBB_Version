using System;
using Xamarin.Forms;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using TilesApp.Models;
using TilesApp.ExpandableView;

namespace TilesApp
{
    public partial class TableOrder : ContentPage
    {
        private Boolean InfoRow = false;

        public TableOrder()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public TableOrder(string user_name)
        {
            InitializeComponent();
            user.Text = user_name;
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = new ListViewPageModel();
        }

        private async void GoToStep(object sender, EventArgs args)
        {
            Tile t = new Tile(); t.id = 2;
            await Navigation.PushModalAsync(new StepsPage(t, 2, 9, "user", "http://oboria.net/docs/pdf/ftp/2/3.PDF",3));
        }

        private void Logout_Pressed(object sender, EventArgs args)
        {
            //LOGOUTView.IsVisible = true;
        }

        private void Logout_Cancel(object sender, EventArgs args)
        {
            //LOGOUTView.IsVisible = false;
        }

        private async void Logout_Accept(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }
    }
}