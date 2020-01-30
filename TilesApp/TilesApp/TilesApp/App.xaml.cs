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

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace TilesApp
{
    public partial class App : Application
    {
        public JSONParser jsParser = new JSONParser();
        public static string DeviceSerialNumber { get; set; }
        public static Models.Location GeoLocation { get; set; }
        public static string Station { get; set; }
        public static ObservableCollection<Dictionary<string, object>> Inventory { get; set; } = new ObservableCollection<Dictionary<string, object>>();
        public App()
        {
            
            InitializeComponent();
            ////testing SACO app
            MainPage = new NavigationPage(new Login());
            //MainPage = new NavigationPage(new QC("App_QC_TestTakePhoto"));

            ////testing generated JSON
            //string json = jsParser.GenerateJSON();
            //MainPage = new NavigationPage(new JSONPage(json));

            ////testing JSON scanned from QR
            //MainPage = new NavigationPage(new JSONPageInit());

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

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

       

    }
}