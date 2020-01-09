using System;
using System.Collections.Generic;
using TilesApp.Rfid;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOAppPage : ContentPage
    {
        Dictionary<string, object> userInfo;

        public SACOAppPage()
        {
            InitializeComponent();
            this.BindWithLifecycle(App.ViewModel.Inventory);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public SACOAppPage(Dictionary<string, object> userInf)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            userInfo = userInf;
            List<string> tags = (List<string>)userInfo["tags"];
            int row = 0;
            foreach (string tag in tags)
            {
                string simplifiedTag = tag.Substring(4);
                int underScore = simplifiedTag.IndexOf("_");
                buttonsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                Button button = new Button
                {
                    Text = simplifiedTag.Substring(underScore + 1),
                    TextColor = Color.FromHex("#F8F9FA"),
                    BackgroundColor = Color.FromHex("#DC3545"),
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16,
                    WidthRequest = 190,
                    CornerRadius = 8,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                };
                if (tag.Contains("App_Associate_")) button.Clicked += Associate_Command;
                else if (tag.Contains("App_Assemble_")) button.Clicked += Assemble_Command;
                else if (tag.Contains("App_Checkpoint_")) button.Clicked += Checkpoint_Command;
                else if (tag.Contains("App_CheckpointRich_")) button.Clicked += CheckpointRich_Command;
                else if (tag.Contains("App_QC_")) button.Clicked += QC_Command;
                else if (tag.Contains("App_QCRich_")) button.Clicked += QCRich_Command;
                buttonsGrid.Children.Add(button, 0, row);
                row++;
            }
        }

        // Applications

        private async void Associate_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            await Navigation.PushModalAsync(new SACOAssociate(b.Text));
        }

        private async void Assemble_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            await Navigation.PushModalAsync(new SACOAssemble(b.Text));
        }

        private async void Checkpoint_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            await Navigation.PushModalAsync(new SACOCheckpoint(b.Text));
        }

        private async void CheckpointRich_Command(object sender, EventArgs args)
        {
            await DisplayAlert("WORK IN PROGRESS", "We are still working on it...", "OK");
        }

        private async void QC_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            await Navigation.PushModalAsync(new SACOQC(b.Text));
        }

        private async void QCRich_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            await Navigation.PushModalAsync(new SACOTakePhoto(b.Text));
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

    }
}