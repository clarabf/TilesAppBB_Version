using System;
using System.Collections.Generic;
using TilesApp.Services;
using TilesApp.Rfid;
using Xamarin.Forms;
using TilesApp.Models;
using Android.OS;
using Android.Bluetooth;
using Android.Hardware.Usb;
using Android.Views;
using System.Linq;
using TilesApp.Rfid.ViewModels;

namespace TilesApp.Views
{
    public partial class Main : ContentPage
    {
        public Main()
        {
            InitializeComponent();
            //OdooXMLRPC.Start();
            Setup();
            // Check if the user has a valid token
            NavigationPage.SetHasNavigationBar(this, false);
            if (AuthHelper.CheckIfTokenIsValid()) {
                Device.BeginInvokeOnMainThread(() =>
                {
                    App.ActiveSession = true;
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new AppPage());
                });
            }
                MessagingCenter.Subscribe<Application, String>(Application.Current, "UserScanned", async (s, a) => {
                if (PHPApi.users.ContainsKey(a.ToString()))
                {

                    PHPApi.SetCurrentUser(a.ToString()); // SETS THE INFORMATION OF THE USER ON APPLICATION LEVEL
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
            MessagingCenter.Subscribe<AppPage>(this, "OdooConnection", (s) => {
                PHPApi.Start();
                Setup();
                App.Station = null;
            });
            MessagingCenter.Subscribe<Application, string>(Application.Current, "Error", async (s, errorMessage) => {
                await DisplayAlert("Error", errorMessage, "Ok");
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CosmosDBManager.Init();
            usernameEntry.Text = "";
            usernameEntry.Placeholder = "username";
            passwordEntry.Text = "";
            passwordEntry.Placeholder = "password";
        }
        private async void GoToScan(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new Scan("SCAN YOUR EMPLOYEE CARD", 1, PHPApi.users));
        }

        private async void LoginClicked(object sender, EventArgs args)
        {
            App.ActiveSession = await AuthHelper.Login(usernameEntry.Text, passwordEntry.Text);
            if (App.ActiveSession) {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new AppPage());
                });
            }
        }

        private async void Reader_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new Rfid.Views.MainPage());
        }

        private void Setup()
        {
            this.BindWithLifecycle(App.ViewModel.Inventory);
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("You are closing the application", "Are you sure you want to leave?", "OK", "Cancel"))
                {
                    base.OnBackButtonPressed();
                    Process.KillProcess(Process.MyPid());
                }
            });
            return true;
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            if (usernameEntry.Text != "" && passwordEntry.Text != "") LoginBtn.IsEnabled = true;
            else LoginBtn.IsEnabled = false;
        }
    }
}
