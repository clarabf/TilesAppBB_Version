using TilesApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TilesApp.Rfid.ViewModels;
using System.Linq;
using TilesApp.Services;
using System;
using TilesApp.Models;
using Android.Bluetooth;
using Android.Hardware.Usb;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using TilesApp.Models.DataModels;
using Xamarin.Essentials;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace TilesApp
{
    public partial class App : Application
    {
        public JSONParser jsParser = new JSONParser();
        public static string DeviceSerialNumber { get; set; }
        public static Models.Location GeoLocation { get; set; }
        public static string Station { get; set; }
        public static User User { get; set; } = new User();
        public static bool ActiveSession { get; set; } = false;
        public static bool IsConnected { get; set; } = false;
        public static bool FirstLaunch { get; set; } = true;
        public static ObservableCollection<Dictionary<string, object>> Inventory { get; set; } = new ObservableCollection<Dictionary<string, object>>();
        private static LocalDatabase database;
        public static LocalDatabase Database
        {
            get
            {
                if (database == null)
                {
                 database = new LocalDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Local.db3"));
                }
                return database;
            }
        }
        public App()
        {
            
            InitializeComponent();
            
            MainPage = new NavigationPage(new Main());

            NavigationPage.SetHasNavigationBar(this, false);

        }

        /// <summary>
        /// Change the displayed tab to the one titled "Find Tag"
        /// </summary>
        public static void ShowFind()
        {
            var main = App.Current.MainPage as Rfid.Views.MainPage;
            var target = main.Children.Where(x => x.Title == "Find Tag").FirstOrDefault() as NavigationPage;
            main.CurrentPage = target;
            target.CurrentPage.Focus();
        }

        /// <summary>
        /// Change the displayed tab to the one titled "Read Write"
        /// </summary>
        public static void ShowReadWrite()
        {
           var main = App.Current.MainPage as Rfid.Views.MainPage;
            var target = main.Children.Where(x => x.Title == "Read Write").FirstOrDefault() as NavigationPage;
            main.CurrentPage = target;
            target.CurrentPage.Focus();
        }


        /// <summary>
        /// Gets the <see cref="ViewModels.ViewModelLocator"/> that will return a ViewModel for a View
        /// </summary>
        public static ViewModelLocator ViewModel { get; } = new ViewModelLocator();

        protected async override void OnStart()
        {
            if (App.IsConnected)
            {
                CosmosDBManager.Init();               
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }


    }
}