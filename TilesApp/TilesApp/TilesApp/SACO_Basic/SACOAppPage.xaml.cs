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
                string simplifiedTag = tag.Substring(4);
                int underScore = simplifiedTag.IndexOf("_");
                if (tag.Contains("App_Associate_"))
                {
                    btAssociate.Text = simplifiedTag.Substring(underScore+1);
                    btAssociate.IsEnabled = true;
                }
                else if (tag.Contains("App_Assemble_"))
                {
                    btAssemble.Text = simplifiedTag.Substring(underScore + 1);
                    btAssemble.IsEnabled = true;
                }
                else if (tag.Contains("App_Checkpoint_"))
                {
                    btCheckpoint.Text = simplifiedTag.Substring(underScore + 1);
                    btCheckpoint.IsEnabled = true;
                }
                else if (tag.Contains("App_CheckpointRich_"))
                {
                    btCheckpointR.Text = simplifiedTag.Substring(underScore + 1);
                    btCheckpointR.IsEnabled = true;
                }
                else if (tag.Contains("App_QC_"))
                {
                    btQC.Text = simplifiedTag.Substring(underScore + 1);
                    btQC.IsEnabled = true;
                }
                else if (tag.Contains("App_QCRich_"))
                {
                    btQCR.Text = simplifiedTag.Substring(underScore + 1);
                    btQCR.IsEnabled = true;
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
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PopModalAsync(true);
                Navigation.PushModalAsync(new SACOLogin());
            });
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