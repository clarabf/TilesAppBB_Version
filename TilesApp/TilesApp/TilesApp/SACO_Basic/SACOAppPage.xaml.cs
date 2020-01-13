using System;
using System.Collections.Generic;
using TilesApp.Odoo;
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
                if (OdooXMLRPC.appsConfigs.ContainsKey(tag)) {
                    string[] tagArr = tag.Split('_');
                    string appType = tagArr[1];
                    string appName = tagArr[2];
                    buttonsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    Button button = new Button
                    {
                        Text = appName,
                        TextColor = Color.FromHex("#F8F9FA"),
                        BackgroundColor = Color.FromHex("#DC3545"),
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16,
                        WidthRequest = 190,
                        CornerRadius = 8,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
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
        }

        // Applications

        private async void Link_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            foreach (var tag in (List<string>)userInfo["tags"])
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
            foreach (var tag in (List<string>)userInfo["tags"])
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
            foreach (var tag in (List<string>)userInfo["tags"])
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
            foreach (var tag in (List<string>)userInfo["tags"])
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
                Navigation.PushModalAsync(new SACOLogin());
            }); 
        }

    }
}