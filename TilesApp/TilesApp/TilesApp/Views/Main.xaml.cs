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
using Nito.AsyncEx;
using TilesApp.Models.DataModels;
using TilesApp.Views.Other_Functionalities;
using Xamarin.Essentials;
using Newtonsoft.Json;

namespace TilesApp.Views
{
    public partial class Main : ContentPage
    {
        private bool rememberUser;

        public Main()
        {
            InitializeComponent();
            Setup();
            
            // Get last user from data base
            if (App.User.GivenName == null)
            {
                AsyncContext.Run(() => GetLastUserFromDB()); 
            }

            // Check if the user has a valid token
            NavigationPage.SetHasNavigationBar(this, false);

            if (App.Current.Properties.ContainsKey("username"))
            {
                usernameEntry.Text = App.Current.Properties["username"] as string;
            }
            else
            {
                usernameEntry.Placeholder = "username";
            }
            if (App.Current.Properties.ContainsKey("password"))
            {
                passwordEntry.Text = App.Current.Properties["password"] as string;
                LoginBtn.IsEnabled = true;
            }
            else
            {
                passwordEntry.Placeholder = "password";
            }
            if (App.Current.Properties.ContainsKey("current_project_name"))
            {
                App.CurrentProjectName = App.Current.Properties["current_project_name"] as string;
            }
            if (App.Current.Properties.ContainsKey("current_project_slug"))
            {
                App.CurrentProjectSlug = App.Current.Properties["current_project_slug"] as string;
            }
            lblVersion.Text = VersionTracking.CurrentVersion.ToString();

            MessagingCenter.Subscribe<Application, string>(Application.Current, "Error", async (s, errorMessage) => {
                await DisplayAlert("Error", errorMessage, "Ok");
            });
            if (!App.IsConnected) DisplayAlert("WARNING: You are offline.", "The performed operations will be stored, and will be uploaded when internet is connected.", "Ok");
            else
            {
                int count = App.Database._database.Table<PendingOperation>().Count();
                if (count > 0) DisplayAlert("WARNING: There are operations pending to store!", "They will be send when the application goes background.", "Ok");
            }
            
        }

        private void GetLastUserFromDB() {
            // Get the last logeed in user
            User tempUser = App.Database.GetLastLoggedInUser();
            if (tempUser != null) App.User = tempUser;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();           
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private async void LoginClicked(object sender, EventArgs args)
        {
            // Check the RememberUser flag to decide wether to store their data or not
            if (rememberUser)
            {
                if (Application.Current.Properties.ContainsKey("username")) Application.Current.Properties["username"] = usernameEntry.Text;
                else Application.Current.Properties.Add("username", usernameEntry.Text);
                if (Application.Current.Properties.ContainsKey("password")) Application.Current.Properties["password"] = passwordEntry.Text;
                else Application.Current.Properties.Add("password", passwordEntry.Text);

                await Application.Current.SavePropertiesAsync();
            }
    
            bool success = false;
            //ONLINE
            if (App.IsConnected)
            {
                App.ActiveSession = await AuthHelper.Login(usernameEntry.Text, passwordEntry.Text);
                if (App.ActiveSession) success = true;

            }
            //OFFLINE
            else
            {
                User tempUser = App.Database.GetUser(usernameEntry.Text, SHAEncription.GenerateSHA256String(passwordEntry.Text));
                if (tempUser != null)
                {
                    App.User = tempUser;
                    success = true;
                }
            }
            if (success)
            {
                //success = await PHPApi.GetConfigFiles(App.User.MSID, App.User.OBOToken);
                if (success)
                {
                    try
                    {
                        string result = await Api.GetProjectsList();
                        App.Projects = JsonConvert.DeserializeObject<List<Web_Project>>(result);
                    }
                    catch {
                        App.Projects = new List<Web_Project>();
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        //Navigation.PushModalAsync(new AppPage());
                        Navigation.PushModalAsync(new FamilyAndGroups());
                    });
                }
                else
                {
                    await DisplayAlert("Error", "Failed to recover the user apps. Please, try again.", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Login Error", "User name and/or password may be incorrect.", "Ok");
            }
        }

        private async void Reader_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new Rfid.Views.MainPage());
        }
        void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            rememberUser = e.Value;
        }
        private void Setup()
        {
            this.BindWithLifecycle(App.ViewModel.Inventory);
            App.IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
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
