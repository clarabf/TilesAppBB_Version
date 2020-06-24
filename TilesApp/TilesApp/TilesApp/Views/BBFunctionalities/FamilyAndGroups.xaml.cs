﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesApp.Models;
using TilesApp.Models.DataModels;
using TilesApp.Services;
using TilesApp.Views.Other_Functionalities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TilesApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FamilyAndGroups : ContentPage
    {

        public ObservableCollection<string> FamGroupData { get; set; } = new ObservableCollection<string>();

        public FamilyAndGroups()
        {
            InitializeComponent();
            BindingContext = this;
            FamGroupData.Add("Family 1");
            FamGroupData.Add("Family 2");
            FamGroupData.Add("Family 3");
            FamGroupData.Add("Family 4");
            FamGroupData.Add("Family 5");
            FamGroupData.Add("Family 6");
            FamGroupData.Add("Family 7");
            FamGroupData.Add("Family 8");
        }

        private async void Config_Command(object sender, EventArgs args)
        {
            //await Navigation.PushModalAsync(new Configuration(this));
            await DisplayAlert("Waring", "Page still in progres...", "Ok");
        }

        private async void Pending_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new PendingOperations());
        }
        private async void Reader_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new Rfid.Views.MainPage());
        }

        private async void Logout_Command(object sender, EventArgs args)
        {
            if (await DisplayAlert("You are abandoning this page", "Are you sure you want to logout?", "OK", "Cancel"))
            {
                if (App.IsConnected)
                {
                    CosmosDBManager.InsertOneObject(new AppBasicOperation(AppBasicOperation.OperationType.Logout)); // Register the logout! 
                }
                //timer.Stop();
                App.User.UserTokenExpiresAt = DateTime.Now;
                int res = App.Database.SaveUser(App.User);
                Device.BeginInvokeOnMainThread(() =>
                {
                    App.User = new User();
                    App.ActiveSession = false;
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new Main());
                });
            }
        }

        private async void Cancel(object sender, EventArgs args)
        {
            await DisplayAlert("Edition complete", "No changes have been made.", "Ok");
            await Navigation.PopModalAsync(true);
        }

        private async void SaveAndFinish(object sender, EventArgs args)
        {
            //MessagingCenter.Send(this, "SendPhotos", TakenPhotos);
            await DisplayAlert("Edition complete", "Photos have been successfully deleted!", "Ok");
            await Navigation.PopModalAsync(true);
        }

    }
}