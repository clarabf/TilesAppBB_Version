using System;
using System.Collections.Generic;
using TilesApp.Services;
using TilesApp.Rfid;
using Xamarin.Forms;

namespace TilesApp.SACO
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
                buttonsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                Button button = new Button
                {
                    Text = appName,
                    TextColor = Color.FromHex("#ffffff"),
                    BackgroundColor = Color.FromHex("#bc0000"),
                    FontSize = 18,
                    WidthRequest = 500,
                    CornerRadius = 5,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    BorderColor = Color.FromHex("#796f6f"),
                    BorderWidth = 3
                };

                switch (appType)
                {
                    case "Link":
                        button.Clicked += Link_Command;
                        break;
                    case "Join":
                        button.Clicked += Join_Command;
                        break;
                    case "Reg":
                        button.Clicked += Reg_Command;
                        break;
                    case "QC":
                        button.Clicked += QC_Command;
                        break;
                    default:
                        break;
                }
                buttonsGrid.Children.Add(button, 0, row);
                row++;           
            }
        }

        // Applications

        private async void Link_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            foreach (var tag in (OdooXMLRPC.userAppsList))
            {
                if (tag.Contains(b.Text)&& tag.Contains("Link"))
                {
                    await Navigation.PushModalAsync(new Link(tag));
                    break;
                }
            }            
        }

        private async void Join_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            foreach (var tag in (OdooXMLRPC.userAppsList))
            {
                if (tag.Contains(b.Text) && tag.Contains("Join"))
                {
                    await Navigation.PushModalAsync(new Join(tag));
                    break;
                }
            }
        }

        private async void Reg_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            foreach (var tag in (OdooXMLRPC.userAppsList))
            {
                if (tag.Contains(b.Text) && tag.Contains("Reg"))
                {
                    await Navigation.PushModalAsync(new Reg(tag));
                    break;
                }
            }
        }


        private async void QC_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            foreach (var tag in (OdooXMLRPC.userAppsList))
            {
                if (tag.Contains(b.Text) && tag.Contains("QC"))
                {
                    await Navigation.PushModalAsync(new QC(tag));
                    break;
                }
            }
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
                Navigation.PushModalAsync(new Login());
            }); 
        }

    }
}