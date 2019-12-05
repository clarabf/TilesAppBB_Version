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
                        btAssociate.BackgroundColor = Color.Black;
                        btAssociate.IsEnabled = true;
                        break;
                    case "Assemble":
                        btAssemble.BackgroundColor = Color.Black;
                        btAssemble.IsEnabled = true;
                        break;
                    case "Checkinout":
                        btCheckInOut.BackgroundColor = Color.Black;
                        btCheckInOut.IsEnabled = true;
                        break;
                    case "Checkpoint":
                        btCheckpoint.BackgroundColor = Color.Black;
                        btCheckpoint.IsEnabled = true;
                        break;
                    case "QC":
                        btQC.BackgroundColor = Color.Black;
                        btQC.IsEnabled = true;
                        break;
                    case "Report":
                        btReport.BackgroundColor = Color.Black;
                        btReport.IsEnabled = true;
                        break;
                }
            }
        }

        private async void Logout_Command(object sender, EventArgs args)
        {
            //Register logout
            await Navigation.PopModalAsync(true);
        }

        private async void Associate_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new SACOAssociate());
        }

        private async void Assemble_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new SACOAssemble());
        }

        private async void Checkpoint_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new SACOCheckpoint());
        }

        private async void QC_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new SACOQC());
        }

        private async void Report_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new SACOTakePhoto());
        }
    }
}
