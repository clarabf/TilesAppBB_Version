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
            
            NavigationPage.SetHasNavigationBar(this, false);

            AsyncContext.Run(() => SetUserAndPassword());

            if (App.Current.Properties.ContainsKey("current_project_name"))
            {
                App.CurrentProjectName = App.Current.Properties["current_project_name"] as string;
            }
            if (App.Current.Properties.ContainsKey("current_project_slug"))
            {
                App.CurrentProjectSlug = App.Current.Properties["current_project_slug"] as string;
            }
            //lblVersion.Text = VersionTracking.CurrentVersion.ToString();

            MessagingCenter.Subscribe<Application, string>(Application.Current, "Error", async (s, errorMessage) => {
                await DisplayAlert("", errorMessage, "Ok");
            });
            if (!App.IsConnected) DisplayAlert("WARNING: You are offline.", "The filled forms will be stored, and will be uploaded when internet is connected.", "Ok");
            else
            {
                int count = App.Database.GetOfflineOperationsCount();
                if (count > 0) DisplayAlert("WARNING: There are forms pending to update!", "They will be updated when the application goes background.", "Ok");
            }
            
        }

        private async void SetUserAndPassword()
        {
            string username = await SecureStorage.GetAsync("username");
            string password = await SecureStorage.GetAsync("password");
            if (username != null) usernameEntry.Text = username;
            else usernameEntry.Placeholder = "username";

            if (password != null) passwordEntry.Text = password;
            else passwordEntry.Placeholder = "password";

            LoginBtn.IsEnabled = (username != null && password != null) ? true : false;
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
            LoadingPopUp.IsVisible = true;
            loading.IsRunning = true;
            if (rememberUser)
            {
                //Store the user credentials in Key Store
                SecureStorage.Remove("username");
                SecureStorage.Remove("password");
                await SecureStorage.SetAsync("username", usernameEntry.Text);
                await SecureStorage.SetAsync("password", passwordEntry.Text);
            }
    
            bool success = false;
            //ONLINE
            if (App.IsConnected)
            {
                App.ActiveSession = await AuthHelper.Login(usernameEntry.Text, passwordEntry.Text);
                if (App.ActiveSession)
                {
                    //CosmosDBManager.InsertOneObject(new AppBasicOperation(AppBasicOperation.OperationType.Login));
                    success = true;
                }
            }
            //OFFLINE
            else
            {
                //Recovering info of OBOToken stored
                string oauthToken = await SecureStorage.GetAsync("oauth_token");
                if (oauthToken != null && oauthToken != "")
                {
                    success = AuthHelper.FillDataWithOBOToken(oauthToken);
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
                        if (result != "") App.Projects = JsonConvert.DeserializeObject<List<Web_Project>>(result);
                        else App.Projects = new List<Web_Project>();
                    }
                    catch 
                    {
                        App.Projects = new List<Web_Project>();
                    }

                    try
                    {
                        string result = await Api.GetPhases();
                        if (result != "") App.Phases = JsonConvert.DeserializeObject<Dictionary<string, Phase>>(result);
                        else App.Phases = new Dictionary<string, Phase>();
                    }
                    catch
                    {
                        App.Phases = new Dictionary<string, Phase>();
                    }

                    try
                    {
                        string result = await Api.GetPrimitiveTypes();
                        if (result != "")
                        {
                            App.PrimitiveTypes = JsonConvert.DeserializeObject<Dictionary<string, PrimitiveType>>(result);
                            //Store primitives type for offline purposes
                            App.Database.DeleteAllPrimitiveTypes();
                            foreach (KeyValuePair<string, PrimitiveType> kv in App.PrimitiveTypes)
                            {
                                PrimitiveType pt = kv.Value;
                                pt.Id = int.Parse(kv.Key);
                                App.Database.SavePrimitiveType(pt);
                            }
                        }
                        else
                        {
                            App.PrimitiveTypes = new Dictionary<string, PrimitiveType>();
                            //Recover the stored primitive types
                            List<PrimitiveType> ptList = App.Database.GetPrimitiveTypes();
                            foreach (PrimitiveType pt in ptList) App.PrimitiveTypes.Add(pt.Id.ToString(), pt);
                        }
                    }
                    catch
                    {
                        App.PrimitiveTypes = new Dictionary<string, PrimitiveType>();
                        //Recover the stored primitive types
                        List<PrimitiveType> ptList = App.Database.GetPrimitiveTypes();
                        foreach (PrimitiveType pt in ptList) App.PrimitiveTypes.Add(pt.Id.ToString(), pt);
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        LoadingPopUp.IsVisible = false;
                        loading.IsRunning = false;
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new FamilyAndGroups());
                    });
                }
                else
                {
                    await DisplayAlert("Error", "Failed to recover the user apps. Please, try again.", "Ok");
                    LoadingPopUp.IsVisible = false;
                    loading.IsRunning = false;
                }
            }
            else
            {
                await DisplayAlert("Login Error", "User name and/or password may be incorrect.", "Ok");
                LoadingPopUp.IsVisible = false;
                loading.IsRunning = false;
            }
        }

        private async void Reader_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new Rfid.Views.ReadersMainTabbedPage());
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
