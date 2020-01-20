using System;
using System.Collections.Generic;
using TilesApp.Services;
using TilesApp.Rfid;
using Xamarin.Forms;

namespace TilesApp.Views
{
    public partial class AppPage : ContentPage
    {
        Dictionary<string, object> userInfo;

        public AppPage()
        {
            InitializeComponent();
            this.BindWithLifecycle(App.ViewModel.Inventory);
            NavigationPage.SetHasNavigationBar(this, false);
            int row = 0;
            foreach (string tag in OdooXMLRPC.userAppsList)
            {
                string[] tagArr = tag.Split('_');
                string appType = tagArr[1];
                string appName = tagArr[2];
                string icon = "";
                buttonsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                Button button = new Button
                {
                    TextColor = Color.FromHex("#ffffff"),
                    BackgroundColor = Color.FromHex("#bc0000"),
                    FontSize = 18,
                    FontFamily = Application.Current.Resources["FontIcon"].ToString(),
                    WidthRequest = 500,
                    CornerRadius = 5,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    BorderColor = Color.FromHex("#796f6f"),
                    BorderWidth = 3,
                    ClassId = tag
                };
                switch (appType)
                {
                    case "Link":
                        icon = "\uf5a1";
                        button.Clicked += Link_Command;
                        break;
                    case "Join":
                        icon = "\uf6fa";
                        button.Clicked += Join_Command;
                        break;
                    case "Reg":
                        icon = "\uf753";
                        button.Clicked += Reg_Command;
                        break;
                    case "QC":
                        icon = "\uf186";
                        button.Clicked += QC_Command;
                        break;
                    default:
                        break;
                }
                button.Text = appName + " " +  icon;
                buttonsGrid.Children.Add(button, 0, row);
                row++;           
            }
        }

        // Applications

        private async void Link_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            await Navigation.PushModalAsync(new Link(b.ClassId));
        }

        private async void Join_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            await Navigation.PushModalAsync(new Join(b.ClassId));
        }

        private async void Reg_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            await Navigation.PushModalAsync(new Reg(b.ClassId));
        }

        private async void QC_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            await Navigation.PushModalAsync(new QC(b.ClassId));
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
            await DisplayAlert("You are abandoning this page", "Please, wait until Login page appears.", "OK");
            MessagingCenter.Send(this, "OdooConnection");
            await Navigation.PopModalAsync(true);
        }

    }
}