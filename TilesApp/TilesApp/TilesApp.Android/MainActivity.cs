using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Plugin.CurrentActivity;
using Plugin.Media;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using TilesApp.Rfid;

namespace TilesApp.Droid
{
    using Android.Bluetooth;
    using Android.Content;
    using Android.Hardware.Usb;
    using Android.Views.InputMethods;
    using System.Linq;
    using TechnologySolutions.Rfid.AsciiProtocol.Extensions;
    using TechnologySolutions.Rfid.AsciiProtocol.Transports;

    [Activity(Label = "TilesApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        List<string> code = new List<string>();
        private IAndroidLifecycle lifecyle;
        DeviceMonitor monitor;
        public static UsbDevice device;
        UsbManager manager;
        protected async override void OnCreate(Bundle savedInstanceState)
        {


            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            await CrossMedia.Current.Initialize();
            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);
            LoadApplication(new App());
            monitor = new DeviceMonitor();
            ScanBluetoothDevices();
            ScanSerialDevices();
           
        }
        private IAndroidLifecycle TslLifecycle
        {
            get
            {
                if (this.lifecyle == null)
                {
                    AsciiTransportsManager manager = Locator.Default.Locate<IAsciiTransportsManager>() as AsciiTransportsManager;

                    // AndrdoidLifecycleNone provides a no action IAndroidLifecycle instance to call in OnPause, OnResume so we don't keep
                    // attempting to resolve the AsciiTransportManager as the IAndroidLifecycle if it is not being used in this project
                    this.lifecyle = (IAndroidLifecycle)manager ?? new AndroidLifecycleNone();

                    // If the HostBarcodeHandler has been registered with the locator then it will be the Android type that needs IAndroidLifecycle calls
                    // Register the HostBarcodeHandler lifecycle with the AsciiTransportsManager
                    manager.RegisterLifecycle(Locator.Default.Locate<IHostBarcodeHandler>() as HostBarcodeHandler);
                }

                return this.lifecyle;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            (this.Lifecycle as IDisposable).Dispose();
        }

        protected override void OnPause()
        {
            base.OnPause();

            this.TslLifecycle.OnPause();
            //UnregisterReceiver(monitor);
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.TslLifecycle.OnResume(this);
            //RegisterReceiver(monitor, new IntentFilter(UsbManager.ActionUsbDeviceAttached));
            //RegisterReceiver(monitor, new IntentFilter(UsbManager.ActionUsbDeviceDetached));

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (e.KeyCode.ToString() != "Enter")
            {
                code.Add(e.KeyCode.ToString());
            }
            else
            {
                string barcode = TranslateKeyCodes(code.ToArray());
                code.Clear();
                if (barcode.Length == 8) MessagingCenter.Send(Xamarin.Forms.Application.Current, "UserScanned", barcode);
                else MessagingCenter.Send(Xamarin.Forms.Application.Current, "BarcodeScanned", barcode);
            }
            return base.OnKeyDown(keyCode, e);
        }

        private string TranslateKeyCodes(string[] keyCodes)
        {
            String result = "";
            for (int i = 0; i < keyCodes.Length; i++)
            {
                if (keyCodes[i].Length == 1)
                {
                    if (i >= 1)
                    {
                        if (keyCodes[i - 1] == "ShiftLeft")
                        {
                            result += keyCodes[i].ToUpper();
                        }
                        else
                        {
                            result += keyCodes[i].ToLower();
                        }
                    }
                    else
                    {
                        result += keyCodes[i].ToLower();
                    }
                }
                else if (keyCodes[i].Contains("Num"))
                {
                    result += keyCodes[i].Substring(keyCodes[i].Length - 1);
                }
                else if (keyCodes[i] == "Slash")
                {
                    result += "/";
                }
            }
            return result;
        }

        private void ScanBluetoothDevices(){

            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            foreach (BluetoothDevice bluetoothDevice in adapter.BondedDevices)
            {
                if (bluetoothDevice.Type == BluetoothDeviceType.Le)
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "BluetoothDeviceFound", bluetoothDevice);
            }
        }

        private void ScanSerialDevices()
        {
            manager = (UsbManager)Android.App.Application.Context.GetSystemService(Context.UsbService);
            try
            {
                device = MainActivity.device = (manager.DeviceList.Values.ToArray())[0];
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "DeviceAttached", device);
            }
            catch (Exception) { }
        }





    }
}