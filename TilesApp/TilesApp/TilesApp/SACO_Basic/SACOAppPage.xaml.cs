using System;
using System.Collections.Generic;
using TilesApp.Rfid;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOAppPage : ContentPage
    {
        private double width = 0;
        private double height = 0;
        Dictionary<string, object> userInfo;

        public SACOAppPage()
        {
            InitializeComponent();
            this.BindWithLifecycle(App.ViewModel.Inventory);
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;
        }

        public SACOAppPage(Dictionary<string, object> userInf)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;
            userInfo = userInf;
            List<string> tags = (List<string>)userInfo["tags"];
            foreach (string tag in tags)
            {
                switch (tag)
                {
                    case "Associate":
                        btAssociate.IsEnabled = true;
                        break;
                    case "Assemble":
                        btAssemble.IsEnabled = true;
                        break;
                    case "Checkpoint":
                        btCheckpoint.IsEnabled = true;
                        break;
                    case "CheckpointRich":
                        btCheckpointR.IsEnabled = true;
                        break;
                    case "QC":
                        btQC.IsEnabled = true;
                        break;
                    case "Report":
                        btQCR.IsEnabled = true;
                        break;
                }
            }
        }

        // Applications

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

        private async void CheckpointRich_Command(object sender, EventArgs args)
        {
            await DisplayAlert("WORK IN PROGRESS", "We are still working on it...", "OK");
        }

        private async void QC_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new SACOQC());
        }

        private async void QCRich_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new SACOTakePhoto());
        }

        // Bottom bar

        private async void Config_Command(object sender, EventArgs args)
        {
            await DisplayAlert("CONFIGURATION", "Config...", "OK");
        }

        private async void Reader_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new Rfid.Views.MainPage());
        }

        private async void Logout_Command(object sender, EventArgs args)
        {
            //Register logout
            await Navigation.PopModalAsync(true);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width != this.width || height != this.height)
            {
                this.width = width;
                this.height = height;
                if (width > height)
                {
                    FASignOut.Margin = new Thickness(0, 0, 0, 5);
                    FABarcode.Margin = new Thickness(80, 0, 0, 5);
                    FAFileTextO.Margin = new Thickness(64, 0, 0, 7);
                }
                else
                {
                    FASignOut.Margin = new Thickness(0, 0, 0, 13);
                    FABarcode.Margin = new Thickness(80, 0, 0, 13);
                    FAFileTextO.Margin = new Thickness(64, 0, 0, 15);
                }
            }
        }

    }
}