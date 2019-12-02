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
            List<string> tags = (List<string>)userInfo["tags"];
            foreach (string tag in tags)
            {
                switch(tag)
                {
                    case "Associate":
                        btAssociate.BackgroundColor = Color.Red;
                        btAssociate.IsEnabled = true;
                        break;
                    case "Assemble":
                        btAssemble.BackgroundColor = Color.Red;
                        btAssemble.IsEnabled = true;
                        break;
                    case "Checkinout":
                        btCheckInOut.BackgroundColor = Color.Red;
                        btCheckInOut.IsEnabled = true;
                        break;
                    case "QC":
                        btQC.BackgroundColor = Color.Red;
                        btQC.IsEnabled = true;
                        break;
                    case "Report":
                        btReport.BackgroundColor = Color.Red;
                        btReport.IsEnabled = true;
                        break;
                }
            }
        }

        private async void Logout(object sender, EventArgs args)
        {
            //Register logout
            await Navigation.PopModalAsync(true);
        }

        private async void TakePhoto(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new SACOTakePhoto());
        }

        private async void Assemble_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new SACOAssemble());
        }

        private async void Associate_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new SACOAssociate());
        }
    }
}
