using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TilesApp.Services;
using TilesApp.Rfid;
using Xamarin.Forms;
using XmlRpc;

namespace TilesApp.SACO
{
    public partial class Login : BasePage
    {
        public Login()
        {
            InitializeComponent();
            Setup();
            NavigationPage.SetHasNavigationBar(this, false);

            if (OdooXMLRPC.users.ContainsKey("error"))
            {
                string message="";
                if (OdooXMLRPC.users["error"].ToString() == "internet") message = "Please, check that internet is turned on in your mobile and restart the application.";
                else if (OdooXMLRPC.users["error"].ToString() == "odoo") message = "Odoo connection failed. Please, restart the application.";
                DisplayAlert("Error recovering users", message, "OK");
            }
            MessagingCenter.Subscribe<Application, String>(Application.Current, "UserScanned", async (s, a) => {
                await DisplayAlert("User <" + a.ToString() + "> scanned", "Please, wait until your App Page loads", "OK");
                if(OdooXMLRPC.users.ContainsKey(a.ToString()))
                {
                    OdooXMLRPC.SetCurrentUser(a.ToString()); // SETS THE INFORMATION OF THE USER ON APPLICATION LEVEL
                    Device.BeginInvokeOnMainThread(() =>
                    {                        
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new AppPage());
                    });
                }
                else
                {
                    await DisplayAlert("Error scanning badge", "User not found in DB...", "Ok");
                }
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Setup();
        }

        private void GoToScan(object sender, EventArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PopModalAsync(true);
                Navigation.PushModalAsync(new SACOScan("SCAN YOUR EMPLOYEE CARD", 1, OdooXMLRPC.users));
            });
        }

        private async void Reader_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new Rfid.Views.MainPage());
        }

        private void Setup()
        {
            this.BindWithLifecycle(App.ViewModel.Inventory);
        }
    }
}
