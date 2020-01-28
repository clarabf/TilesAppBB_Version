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
        public static string DeviceSerialNumber { get; private set; }
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
            Subscribe();

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

        private void Subscribe()
        {

            // SUBSCRIBE TO ALL THE EVENTS THAT MIGHT AFFECT THE PAGE
            MessagingCenter.Subscribe<Application, string>(Application.Current, "FetchedDeviceSerialNumber", async (s, sn) => {
                if (sn != null)
                {
                    DeviceSerialNumber = sn;
                }
            });
            MessagingCenter.Subscribe<Application, UsbDevice>(Application.Current, "DeviceDetached", (s, device) => {
                if (device != null)
                {
                    foreach (var serialDevice in App.ViewModel.Readers.SerialReaders.ToList())
                    {
                        if (serialDevice.SerialNumber == device.SerialNumber)
                        {
                            App.ViewModel.Readers.SerialReaders.Remove(serialDevice);
                        }
                    }
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceFound", (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice pairedDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Paired);
                    foreach (var compDevice in App.ViewModel.Readers.BluetoothCameraReaders.ToList())
                    {
                        if (compDevice.Device.Address == pairedDevice.Device.Address)
                        {
                            int i = App.ViewModel.Readers.BluetoothCameraReaders.IndexOf(compDevice);
                            if (i != -1)
                                App.ViewModel.Readers.BluetoothCameraReaders[i].State = pairedDevice.State;
                            return;
                        }
                    }
                    App.ViewModel.Readers.BluetoothCameraReaders.Add(pairedDevice);
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceConnected", (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice activeDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Active);
                    foreach (var compDevice in App.ViewModel.Readers.BluetoothCameraReaders.ToList())
                    {
                        if (compDevice.Device.Address == activeDevice.Device.Address)
                        {
                            int i = App.ViewModel.Readers.BluetoothCameraReaders.IndexOf(compDevice);
                            if (i != -1)
                                App.ViewModel.Readers.BluetoothCameraReaders[i].State = activeDevice.State;
                            return;
                        }
                    }
                    App.ViewModel.Readers.BluetoothCameraReaders.Add(activeDevice);
                }
            });
            MessagingCenter.Subscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceDisconnected", (s, device) => {
                if (device != null)
                {
                    ComplexBluetoothDevice inActiveDevice = new ComplexBluetoothDevice(device, ComplexBluetoothDevice.States.Disconnected);
                    foreach (var compDevice in App.ViewModel.Readers.BluetoothCameraReaders.ToList())
                    {
                        if (compDevice.Device.Address == inActiveDevice.Device.Address)
                        {
                            int i = App.ViewModel.Readers.BluetoothCameraReaders.IndexOf(compDevice);
                            if (i != -1)
                                App.ViewModel.Readers.BluetoothCameraReaders[i].State = inActiveDevice.State;
                        }
                    }
                }
            });
            MessagingCenter.Subscribe<Application, bool>(Application.Current, "ChargerConnected", (s, chargerConnected) => {
                if (chargerConnected)
                {
                    Console.WriteLine("============Charger was connected!===========");
                }
            });
            MessagingCenter.Subscribe<Application, bool>(Application.Current, "ChargerDisconnected", (s, chargerDisconnected) => {
                if (chargerDisconnected)
                {
                    Console.WriteLine("============Charger was disconnected!===========");
                }
            });

        }
        /*
        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<Application, UsbDevice>(Application.Current, "FetchedDeviceSerialNumber");
            MessagingCenter.Unsubscribe<Application, UsbDevice>(Application.Current, "DeviceAttached");
            MessagingCenter.Unsubscribe<Application, UsbDevice>(Application.Current, "DeviceDetached");
            MessagingCenter.Unsubscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceFound");
            MessagingCenter.Unsubscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceConnected");
            MessagingCenter.Unsubscribe<Application, BluetoothDevice>(Application.Current, "BluetoothDeviceDisconnected");
            MessagingCenter.Unsubscribe<Application, BluetoothDevice>(Application.Current, "ChargerConnected");
            MessagingCenter.Unsubscribe<Application, BluetoothDevice>(Application.Current, "ChargerDisconnected");
        }*/

    }
}