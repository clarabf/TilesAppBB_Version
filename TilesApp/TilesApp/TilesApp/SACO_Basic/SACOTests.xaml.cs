using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOTests : ContentPage
    {
        private double width = 0;
        private double height = 0;
        Dictionary<string, object> userInfo;

        public SACOTests()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;
        }

        public SACOTests(Dictionary<string, object> userInf)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;
            userInfo = userInf;
            user.Text = userInfo["name"].ToString();
        }

        private async void Logout(object sender, EventArgs args)
        {
            //Register logout

            await Navigation.PopModalAsync(true);
        }

        private async void TakePhoto(object sender, EventArgs args)
        {
            //Register logout
            await Navigation.PushModalAsync(new SACOTakePhoto());
        }

    }
}
