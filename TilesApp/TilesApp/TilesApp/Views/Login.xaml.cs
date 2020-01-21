using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TilesApp.Services;
using TilesApp.Rfid;
using Xamarin.Forms;
using XmlRpc;
using TilesApp.Models;

namespace TilesApp.Views
{
    public partial class Login : BasePage
    {
        public Login()
        {
            InitializeComponent();
            OdooXMLRPC.Start();
            Setup();
            NavigationPage.SetHasNavigationBar(this, false);
            MessagingCenter.Subscribe<Application, String>(Application.Current, "UserScanned", async (s, a) => {
                if(OdooXMLRPC.users.ContainsKey(a.ToString()))
                {
                    CosmosDBManager.InsertOneObject(new AppBasicOperation(AppBasicOperation.OperationType.Login)); // Register the login!
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
            MessagingCenter.Subscribe<AppPage>(this, "OdooConnection", async (s) => {
                OdooXMLRPC.Start();
                Setup();
                App.Station = null;
                await DisplayAlert("Welcome back to login!","Connection to Odoo sucessful!", "Ok");
            });
        }

        private void GoToScan(object sender, EventArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PopModalAsync(true);
                Navigation.PushModalAsync(new Scan("SCAN YOUR EMPLOYEE CARD", 1, OdooXMLRPC.users));
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
